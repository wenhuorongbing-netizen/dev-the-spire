using System.Text;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class VakuuLothaSaveRiskGuardTests
{
    [Fact]
    public void CoreSourceStillRejectsActiveParentEventIdCombatSerialization()
    {
        var combatRoom = ReadRepoText("source code", "src", "Core", "Rooms", "CombatRoom.cs");
        var runManager = ReadRepoText("source code", "src", "Core", "Runs", "RunManager.cs");
        var ancientEventModel = ReadRepoText("source code", "src", "Core", "Models", "AncientEventModel.cs");
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
            ancientEventModel,
            "protected override async Task BeforeEventStarted(bool isPreFinished)",
            "if (!isPreFinished)",
            "await CreatureCmd.Heal(base.Owner.Creature, amount, playAnim: false)");
        AssertSourceContains(
            apiResearch,
            "source code/src/Core/Rooms/CombatRoom.cs",
            "throws in `ToSerializable()`",
            "`ParentEventId`",
            "not pre-finished",
            "known active `ParentEventId` serialization blocker");
    }

    [Fact]
    public void VakuuFightDoesNotSilentlyExposeUnsafeParentLinkedChildCombatByDefault()
    {
        var patch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");
        var gate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs");
        var issueIndex = ReadRepoText("docs", "issues.md");
        var sourceDesign = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "source-design.md");
        var manualChecklist = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md");

        var startFight = SliceBetween(patch, "private static async Task StartFight", "public static async Task AfterCreatureAddedToCombat");
        var usesUnsafeActiveParentEventIdShape =
            startFight.Contains("ParentEventId =", StringComparison.Ordinal) ||
            startFight.Contains("EnterCombatWithoutExitingEventMethod.Invoke", StringComparison.Ordinal);

        var exposesDefaultFightOption =
            !gate.Contains("ShouldEnableFight", StringComparison.Ordinal) &&
            patch.Contains("__result = __result.Concat([fightOption]).ToList()", StringComparison.Ordinal);
        var hasExplicitUnsafeForceGate =
            gate.Contains("EnableEnvironmentVariable", StringComparison.Ordinal) &&
            gate.Contains("SpirePlusEnableEnvironmentVariable", StringComparison.Ordinal) &&
            gate.Contains("ShouldEnableFight", StringComparison.Ordinal) &&
            gate.Contains("ShouldForceFight", StringComparison.Ordinal);

        Assert.False(
            usesUnsafeActiveParentEventIdShape && exposesDefaultFightOption,
            "Vakuu still exposes the active ParentEventId child-combat shape through the normal default option path. " +
            "Remove the active ParentEventId room shape, or make the fight unavailable by default " +
            "and keep any unsafe remainder behind an explicit force/debug-only gate.");
        Assert.False(
            usesUnsafeActiveParentEventIdShape,
            "Vakuu StartFight must not assign ParentEventId while the combat room is active; only the prefinished ToSerializable postfix may record the parent event.");
        Assert.True(
            !usesUnsafeActiveParentEventIdShape || hasExplicitUnsafeForceGate,
            "If Vakuu retains the active ParentEventId child combat only for local testing, the gate/source names must make that save-risk/debug-only status explicit.");

        AssertSourceContains(
            patch,
            "[HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.ToSerializable))]",
            "[HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.OfferRoomEndRewards))]",
            "EventNodeBackingField",
            "ClearEventNode(vakuu)",
            "EnterRoomWithoutExitingCurrentRoom(combatRoom, fadeToBlack: true)",
            "ShouldResumeParentEventAfterCombat = true",
            "PreserveVakuuParentForPreFinishedSave",
            "ArmPrefinishedParentRestoreHealSkip",
            "[HarmonyPatch(typeof(AncientEventModel), \"BeforeEventStarted\")]",
            "ShouldSkipPrefinishedParentRestoreHeal",
            "__result = Task.CompletedTask",
            "skipped duplicate Ancient heal",
            "ProceedFromNoRewardVictory",
            "combatRoom.Encounter is not EzmbVakuuTrialEncounter",
            "!combatRoom.IsPreFinished",
            "serializableRoom.ParentEventId =",
            "serializableRoom.ShouldResumeParentEvent = true");
        Assert.DoesNotContain("ParentEventId =", startFight, StringComparison.Ordinal);
        Assert.DoesNotContain("EnterCombatWithoutExitingEventMethod", patch, StringComparison.Ordinal);
        AssertSourceContains(
            string.Join(Environment.NewLine, issueIndex, sourceDesign, manualChecklist),
            "active fight no longer assigns `ParentEventId`",
            "save/load",
            "live");
    }

    [Fact]
    public void VakuuActiveFightAvoidsCoreRejectedParentEventIdShapeAndDocsStayAccurate()
    {
        var eventModel = ReadRepoText("source code", "src", "Core", "Models", "EventModel.cs");
        var combatRoom = ReadRepoText("source code", "src", "Core", "Rooms", "CombatRoom.cs");
        var patch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");
        var currentDocs = string.Join(
            Environment.NewLine,
            ReadRepoText("docs", "test-ready-development-goal.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "source-design.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "risk-register.md"),
            ReadRepoText("docs", "issues.md"));

        AssertSourceContains(
            eventModel,
            "protected void EnterCombatWithoutExitingEvent(EncounterModel mutableEncounter, IReadOnlyList<Reward> extraRewards, bool shouldResumeAfterCombat)",
            "if (!IsShared)",
            "Node = null",
            "ShouldResumeParentEventAfterCombat = shouldResumeAfterCombat",
            "ParentEventId = base.Id");
        AssertSourceContains(
            combatRoom,
            "if (ParentEventId != null && !IsPreFinished)",
            "Cannot serialize a CombatRoom with a ParentEventId that is not pre-finished.");
        AssertSourceContains(
            patch,
            "EnterRoomWithoutExitingCurrentRoom(combatRoom, fadeToBlack: true)",
            "ClearEventNode(vakuu)",
            "EventNodeBackingField",
            "ShouldResumeParentEventAfterCombat = true",
            "PreserveParentEventForPreFinishedSave");
        var startFight = SliceBetween(patch, "private static async Task StartFight", "public static async Task AfterCreatureAddedToCombat");
        Assert.DoesNotContain("ParentEventId =", startFight, StringComparison.Ordinal);
        Assert.DoesNotContain("EnterCombatWithoutExitingEventMethod", patch, StringComparison.Ordinal);
        AssertSourceContains(
            currentDocs,
            "does not call Core's `EnterCombatWithoutExitingEvent(...)`",
            "clears the parent event `Node`",
            "direct `EnterRoomWithoutExitingCurrentRoom(...)`",
            "does not store `ParentEventId` while the combat room is active",
            "active fight no longer assigns `ParentEventId`",
            "live");

        Assert.DoesNotContain("still creates an active parent-linked combat room", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("still uses an active parent-linked combat shape", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("active parent-linked shape remains a source-level blocker", currentDocs, StringComparison.Ordinal);
    }

    [Fact]
    public void VakuuFightKeepsNoNormalRewardAndFallbackSurfacesSourceVisible()
    {
        var patch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");
        var encounter = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightEncounter.cs");
        var apiResearch = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "api-research.md");
        var vakuuSource = string.Join(
            Environment.NewLine,
            Directory.GetFiles(
                    RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu"),
                    "*.cs",
                    SearchOption.TopDirectoryOnly)
                .Select(path => File.ReadAllText(path, Encoding.UTF8)));
        var startFight = SliceBetween(patch, "private static async Task StartFight", "public static async Task AfterCreatureAddedToCombat");
        var createVictoryOptions = SliceBetween(patch, "private static IEnumerable<EventOption> CreateVictoryOptions", "private static EventOption CreateVictoryFallbackOption");

        AssertSourceContains(
            encounter,
            "public override bool ShouldGiveRewards => false",
            "base(RoomType.Monster, autoAdd: false)");
        AssertSourceContains(
            startFight,
            "AncientRewardRelicService.ObtainSelectionRelicIfMissing<VakuuFightOptionRelic>",
            "ModelDb.Encounter<EzmbVakuuTrialEncounter>().ToMutable()",
            "new CombatRoom(encounter, vakuu.Owner.RunState)",
            "ShouldResumeParentEventAfterCombat = true",
            "ClearEventNode(vakuu)",
            "EnterRoomWithoutExitingCurrentRoom(combatRoom, fadeToBlack: true)");
        Assert.DoesNotContain("ParentEventId =", startFight, StringComparison.Ordinal);
        Assert.DoesNotContain("EnterCombatWithoutExitingEventMethod", startFight, StringComparison.Ordinal);
        Assert.DoesNotContain("Array.Empty<Reward>()", startFight, StringComparison.Ordinal);

        Assert.DoesNotContain("LinkedRewardSet", vakuuSource, StringComparison.Ordinal);
        Assert.DoesNotContain("AddExtraReward", vakuuSource, StringComparison.Ordinal);
        Assert.DoesNotContain("RewardsCmd", vakuuSource, StringComparison.Ordinal);
        Assert.DoesNotContain(".ThatWillKillPlayerIf(_ => true)", vakuuSource, StringComparison.Ordinal);
        AssertSourceContains(
            patch,
            "[HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.OfferRoomEndRewards))]",
            "SkipVakuuLoadedTerminalRewards",
            "__instance.Encounter is not EzmbVakuuTrialEncounter",
            "__result = VakuuFightService.ProceedFromNoRewardVictory(__instance)",
            "combatRoom.CombatState.RunState.CurrentRoomCount <= 1",
            "RunManager.Instance.ProceedFromTerminalRewardsScreen()");
        AssertSourceContains(
            createVictoryOptions,
            "owner is null",
            "using the explicit fallback path",
            "Live restore for this path remains pending",
            "targetChoiceCount = encounter.VictoryChoiceCount",
            "encounter.VictoryGold",
            "options.Count > 0 ? options : [CreateVictoryFallbackOption(vakuu, combatRoom)]");
        AssertSourceContains(
            patch,
            "IsEligibleSourceAncientReward(owner, relic)",
            "BeautifulBracelet",
            "ModelDb.Enchantment<Swift>().CanEnchant",
            "TriBoomerang",
            "ModelDb.Enchantment<Instinct>().CanEnchant");
        AssertSourceContains(
            apiResearch,
            "CombatRoom.OfferRoomEndRewards()",
            "does not itself respect `Encounter.ShouldGiveRewards`",
            "VakuuFightNoRewardRestorePatch");
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

}
