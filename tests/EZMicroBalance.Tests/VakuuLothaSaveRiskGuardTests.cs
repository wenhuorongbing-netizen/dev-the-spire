using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class VakuuLothaSaveRiskGuardTests
{
    [Fact]
    public void CoreSourceStillMakesUnfinishedParentLinkedCombatASaveLoadBlocker()
    {
        var combatRoom = ReadRepoText("source code", "src", "Core", "Rooms", "CombatRoom.cs");
        var runManager = ReadRepoText("source code", "src", "Core", "Runs", "RunManager.cs");
        var apiResearch = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "api-research.md");

        AssertSourceContains(
            combatRoom,
            "public override SerializableRoom ToSerializable()",
            "if (ParentEventId != null && !IsPreFinished)",
            "Cannot serialize a CombatRoom with a ParentEventId that is not pre-finished.",
            "serializableRoom.ParentEventId = ParentEventId",
            "serializableRoom.ShouldResumeParentEvent = ShouldResumeParentEventAfterCombat");
        AssertSourceContains(
            runManager,
            "CombatRoom { IsPreFinished: not false, ParentEventId: not null }",
            "EventRoom room = new EventRoom(ModelDb.GetById<EventModel>(combatRoom.ParentEventId))",
            "State.CurrentRoom is CombatRoom { ShouldResumeParentEventAfterCombat: not false }",
            "await State.CurrentRoom.Resume(abstractRoom, State)");
        AssertSourceContains(
            apiResearch,
            "source code/src/Core/Rooms/CombatRoom.cs",
            "throws in `ToSerializable()`",
            "`ParentEventId`",
            "not pre-finished",
            "unfinished parent-linked shape remains a source-level blocker");
    }

    [Fact]
    public void VakuuFightDoesNotSilentlyExposeUnsafeParentLinkedChildCombatByDefault()
    {
        var patch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");
        var gate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs");
        var issueIndex = ReadRepoText("docs", "issues.md");
        var sourceDesign = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "source-design.md");
        var manualChecklist = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md");

        var startFight = SliceFrom(patch, "private static async Task StartFight");
        var usesUnsafeParentLinkedShape =
            startFight.Contains("ParentEventId = vakuu.Id", StringComparison.Ordinal) ||
            startFight.Contains("ShouldResumeParentEventAfterCombat = true", StringComparison.Ordinal);

        var exposesDefaultFightOption =
            patch.Contains("__result = __result.Concat([fightOption]).ToList()", StringComparison.Ordinal);
        var hasExplicitUnsafeForceGate =
            Regex.IsMatch(gate, "Unsafe|SaveRisk|Debug", RegexOptions.IgnoreCase) &&
            Regex.IsMatch(gate, "FORCE|DEBUG", RegexOptions.IgnoreCase);

        Assert.False(
            usesUnsafeParentLinkedShape && exposesDefaultFightOption,
            "Vakuu still exposes the unfinished ParentEventId child-combat shape through the normal default option path. " +
            "Remove the ParentEventId/ShouldResumeParentEventAfterCombat room shape, or make the fight unavailable by default " +
            "and keep any unsafe remainder behind an explicit force/debug-only gate.");
        Assert.True(
            !usesUnsafeParentLinkedShape || hasExplicitUnsafeForceGate,
            "If Vakuu retains the parent-linked child combat only for local testing, the gate/source names must make that save-risk/debug-only status explicit.");

        AssertSourceContains(
            patch,
            "[HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.ToSerializable))]",
            "PreserveParentEventForPreFinishedSave",
            "combatRoom.Encounter is not EzmbVakuuTrialEncounter",
            "!combatRoom.IsPreFinished",
            "serializableRoom.ParentEventId =",
            "serializableRoom.ShouldResumeParentEvent = true");

        if (usesUnsafeParentLinkedShape)
        {
            AssertSourceContains(
                string.Join(Environment.NewLine, issueIndex, sourceDesign, manualChecklist),
                "unfinished parent-linked",
                "save/load",
                "not proven safe");
        }
    }

    [Fact]
    public void VakuuFightKeepsNoNormalRewardAndFallbackSurfacesSourceVisible()
    {
        var patch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");
        var encounter = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightEncounter.cs");
        var vakuuSource = string.Join(
            Environment.NewLine,
            Directory.GetFiles(
                    RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu"),
                    "*.cs",
                    SearchOption.TopDirectoryOnly)
                .Select(path => File.ReadAllText(path, Encoding.UTF8)));
        var startFight = SliceBetween(patch, "private static async Task StartFight", "public static void PreserveParentEventForPreFinishedSave");
        var createVictoryOptions = SliceBetween(patch, "private static IEnumerable<EventOption> CreateVictoryOptions", "private static EventOption CreateVictoryFallbackOption");

        AssertSourceContains(
            encounter,
            "public override bool ShouldGiveRewards => false",
            "base(RoomType.Event, autoAdd: false)");
        AssertSourceContains(
            startFight,
            "new CombatRoom(ModelDb.Encounter<EzmbVakuuTrialEncounter>().ToMutable(), vakuu.Owner.RunState)",
            "EnterRoomWithoutExitingCurrentRoom(combatRoom, fadeToBlack: true)");
        Assert.DoesNotContain("ParentEventId =", startFight, StringComparison.Ordinal);
        Assert.DoesNotContain("ShouldResumeParentEventAfterCombat = true", startFight, StringComparison.Ordinal);

        Assert.DoesNotContain("LinkedRewardSet", vakuuSource, StringComparison.Ordinal);
        Assert.DoesNotContain("AddExtraReward", vakuuSource, StringComparison.Ordinal);
        Assert.DoesNotContain("RewardsCmd", vakuuSource, StringComparison.Ordinal);
        AssertSourceContains(
            createVictoryOptions,
            "owner is null",
            "using the explicit fallback path",
            "Live restore for this path remains pending",
            "options.Count == 3 ? options : [CreateVictoryFallbackOption(vakuu)]");
    }

    [Fact]
    public void LothaDeathReprieveOncePerRunAndDuplicateReprieveGuardsStaySourceVisible()
    {
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var playerState = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientPlayerState.cs");
        var deathBlock = SliceBetween(runHook, "public static bool ShouldDieLate(Creature creature)", "private static void ResetCombatState");
        var startBlock = SliceBetween(runHook, "private static async Task StartDeathReprieveTurn", "private static async Task EnsureDeathReprievePower");

        AssertSourceContains(
            savedFields,
            "SavedSpireField<Player, string> LothaStateKey",
            "SavedSpireField<CardModel, string> LothaDeckStateKey");
        AssertSourceContains(
            playerState,
            "runtimeField[player] = deckState",
            "MirrorToDeck(player, deckField, runtimeState)",
            "deckField[card] = state");
        AssertSourceContains(
            runHook,
            "private enum DeathReprievePhase",
            "PendingStart = 1",
            "Active = 2",
            "Resolved = 3",
            "private sealed record Progress(bool DeathReprieveUsed, DeathReprievePhase DeathReprievePhase)",
            "progress.DeathReprieveUsed ? 1 : 0",
            "(int)progress.DeathReprievePhase",
            "ParseDeathReprievePhase(parts[2], used)",
            "DeathReprieveUsed = true");
        AssertSourceContains(
            deathBlock,
            "if (combatState.DeathReprieveActive || combatState.DeathReprievePendingStart)",
            "return false;",
            "return GetProgress(player).DeathReprieveUsed",
            "if (progress.DeathReprieveUsed)",
            "CreatureCmd.SetCurrentHp(creature, 1m)");
        AssertSourceContains(
            startBlock,
            "if (combatState.DeathReprieveStarted)",
            "return;",
            "combatState.DeathReprieveStarted = true",
            "combatState.DeathReprieveActive = true",
            "combatState.DeathReprievePendingStart = false",
            "DeathReprievePhase = DeathReprievePhase.Active");
        AssertSourceContains(
            runHook,
            "HydrateDeathReprieveState(player, combatState)",
            "private static void HydrateDeathReprieveState(Player player, LothaCombatState combatState)",
            "progress.DeathReprievePhase == DeathReprievePhase.Active",
            "deck-mirrored blessing progress",
            "Active-turn save/load continuation remains live-pending",
            "private static void ResolveDeathReprieveProgress(Player player)",
            "DeathReprievePhase = DeathReprievePhase.Resolved");
    }

    [Fact]
    public void LothaDeathReprieveWritesPhaseBeforeStartingOrPendingReprieve()
    {
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");
        var preventBlock = SliceBetween(runHook, "public static async Task AfterPreventingDeath(Creature creature)", "private static void ResetCombatState");
        var playerTurnBranch = SliceBetween(preventBlock, "if (creature.CombatState?.CurrentSide == CombatSide.Player", "else");
        var pendingBranch = SliceFrom(preventBlock, "DeathReprievePhase = DeathReprievePhase.PendingStart");
        var startBlock = SliceBetween(runHook, "private static async Task StartDeathReprieveTurn", "private static async Task EnsureDeathReprievePower");

        AssertSourceContains(
            playerTurnBranch,
            "SetProgress(player, progress with",
            "DeathReprieveUsed = true",
            "DeathReprievePhase = DeathReprievePhase.Active",
            "StartDeathReprieveTurn");
        AssertBefore(playerTurnBranch, "SetProgress(player, progress with", "StartDeathReprieveTurn");

        AssertSourceContains(
            pendingBranch,
            "DeathReprievePhase = DeathReprievePhase.PendingStart",
            "combatState.DeathReprievePendingStart = true",
            "EnsureDeathReprievePower");
        var pendingPhaseIndex = preventBlock.IndexOf("DeathReprievePhase = DeathReprievePhase.PendingStart", StringComparison.Ordinal);
        var pendingSetProgressIndex = preventBlock.LastIndexOf("SetProgress(player, progress with", pendingPhaseIndex, StringComparison.Ordinal);
        var pendingFlagIndex = preventBlock.IndexOf("combatState.DeathReprievePendingStart = true", pendingPhaseIndex, StringComparison.Ordinal);
        Assert.True(pendingSetProgressIndex >= 0, "Pending reprieve branch must write progress through SetProgress.");
        Assert.True(pendingSetProgressIndex < pendingPhaseIndex, "Pending reprieve progress write must wrap the PendingStart phase.");
        Assert.True(pendingPhaseIndex < pendingFlagIndex, "PendingStart phase must be written before transient pending-start flags are set.");

        AssertSourceContains(
            startBlock,
            "if (combatState.DeathReprieveStarted)",
            "return;",
            "combatState.DeathReprieveStarted = true",
            "SetProgress(player, GetProgress(player) with",
            "DeathReprieveUsed = true",
            "DeathReprievePhase = DeathReprievePhase.Active");
        AssertBefore(startBlock, "SetProgress(player, GetProgress(player) with", "CardPileCmd.Draw(choiceContext, DeathReprieveCards, player)");
    }

    [Fact]
    public void LothaDeathReprieveForceDeathAndPersistenceStanceRemainExplicit()
    {
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");
        var creatureCmd = ReadRepoText("source code", "src", "Core", "Commands", "CreatureCmd.cs");
        var apiResearch = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "api-research.md");
        var riskRegister = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "risk-register.md");
        var manualChecklist = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md");
        var deathBlock = SliceBetween(runHook, "public static bool ShouldDieLate(Creature creature)", "private static void ResetCombatState");
        var resolveBlock = SliceBetween(runHook, "private static async Task ResolveDeathReprieveTurnEnd", "private static async Task ApplyPowerReplacementBenefit");

        AssertSourceContains(
            creatureCmd,
            "public static async Task Kill(Creature creature, bool force = false)",
            "if (force || creature.MaxHp <= 0 || Hook.ShouldDie(runState, combatState, creature, out preventer))");
        AssertSourceContains(
            resolveBlock,
            "CreatureCmd.Kill(player.Creature, force: true)");
        Assert.DoesNotContain("ShouldDie(player.Creature)", resolveBlock, StringComparison.Ordinal);
        Assert.DoesNotContain("AfterPreventingDeath(player.Creature)", resolveBlock, StringComparison.Ordinal);

        var hasSourceRecovery =
            runHook.Contains("RecoverDeathReprieve", StringComparison.Ordinal) ||
            runHook.Contains("RestoreDeathReprieve", StringComparison.Ordinal) ||
            runHook.Contains("HydrateDeathReprieveState", StringComparison.Ordinal) &&
            runHook.Contains("DeathReprievePhase.PendingStart", StringComparison.Ordinal) &&
            runHook.Contains("DeathReprievePhase.Active", StringComparison.Ordinal) ||
            runHook.Contains("LothaDeathReprievePower") &&
            deathBlock.Contains("GetPower<LothaDeathReprievePower>()", StringComparison.Ordinal);
        var docsStateLimitation = string.Join(Environment.NewLine, apiResearch, riskRegister, manualChecklist);
        var docsExplicitlyLimitPendingActiveSaveLoad =
            docsStateLimitation.Contains("Death Reprieve", StringComparison.Ordinal) &&
            docsStateLimitation.Contains("pending/active", StringComparison.OrdinalIgnoreCase) &&
            docsStateLimitation.Contains("save/load", StringComparison.OrdinalIgnoreCase) &&
            (docsStateLimitation.Contains("not proven", StringComparison.OrdinalIgnoreCase) ||
             docsStateLimitation.Contains("unsupported", StringComparison.OrdinalIgnoreCase) ||
             docsStateLimitation.Contains("unverified", StringComparison.OrdinalIgnoreCase));

        Assert.True(
            hasSourceRecovery || docsExplicitlyLimitPendingActiveSaveLoad,
            "Death Reprieve must either recover active/pending reprieve state from a source-supported persistent carrier " +
            "or keep active docs/manual tests explicit that pending/active reprieve save/load is not proven safe.");
    }

    private static string SliceFrom(string value, string start)
    {
        var startIndex = value.IndexOf(start, StringComparison.Ordinal);
        Assert.True(startIndex >= 0, $"Missing start marker: {start}");
        return value[startIndex..];
    }

    private static string SliceBetween(string source, string startMarker, string endMarker)
    {
        var start = source.IndexOf(startMarker, StringComparison.Ordinal);
        Assert.True(start >= 0, $"Missing start marker: {startMarker}");

        var end = source.IndexOf(endMarker, start + startMarker.Length, StringComparison.Ordinal);
        Assert.True(end >= 0, $"Missing end marker: {endMarker}");

        return source[start..end];
    }

    private static void AssertBefore(string source, string first, string second)
    {
        var firstIndex = source.IndexOf(first, StringComparison.Ordinal);
        var secondIndex = source.IndexOf(second, StringComparison.Ordinal);
        Assert.True(firstIndex >= 0, $"Missing first marker: {first}");
        Assert.True(secondIndex >= 0, $"Missing second marker: {second}");
        Assert.True(firstIndex < secondIndex, $"Expected '{first}' to appear before '{second}'.");
    }

    private static void AssertSourceContains(string source, params string[] snippets)
    {
        var missing = snippets
            .Where(snippet => !source.Contains(snippet, StringComparison.Ordinal))
            .ToArray();

        Assert.True(missing.Length == 0, "Missing source evidence:" + Environment.NewLine + string.Join(Environment.NewLine, missing));
    }

    private static string ReadRepoText(params string[] parts)
    {
        return File.ReadAllText(RepoPath(parts), Encoding.UTF8);
    }

    private static string RepoPath(params string[] parts)
    {
        return Path.Combine(new[] { FindRepoRoot() }.Concat(parts).ToArray());
    }

    private static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "EZMicroBalance.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repository root from test output directory.");
    }
}
