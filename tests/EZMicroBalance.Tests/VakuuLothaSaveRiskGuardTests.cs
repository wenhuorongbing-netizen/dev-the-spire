using System.Text;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class VakuuLothaSaveRiskGuardTests
{
    [LocalSourceFact]
    public void CoreSourceStillRejectsActiveParentEventIdCombatSerialization()
    {
        var combatRoom = ReadLocalCoreText("Rooms", "CombatRoom.cs");
        var runManager = ReadLocalCoreText("Runs", "RunManager.cs");
        var ancientEventModel = ReadLocalCoreText("Models", "AncientEventModel.cs");

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
    }

    [Fact]
    public void ApiResearchDocumentsActiveParentEventIdSerializationBlocker()
    {
        var apiResearch = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "api-research.md");

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
        var entry = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.Entry.cs");
        var parentRestore = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.ParentRestore.cs");
        var noReward = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.NoRewardResume.cs");
        var gate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs");
        var issueIndex = ReadRepoText("docs", "issues.md");
        var sourceDesign = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "source-design.md");
        var manualChecklist = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md");

        var startFight = SliceBetween(entry, "private static async Task StartFight", "private static void ClearEventNode");
        var usesUnsafeActiveParentEventIdShape =
            startFight.Contains("ParentEventId =", StringComparison.Ordinal) ||
            startFight.Contains("EnterCombatWithoutExitingEventMethod.Invoke", StringComparison.Ordinal);

        var exposesDefaultFightOption =
            !gate.Contains("ShouldEnableFight", StringComparison.Ordinal) &&
            patch.Contains("__result = __result.Concat([fightOption]).ToList()", StringComparison.Ordinal);
        var hasExplicitUnsafeForceGate =
            gate.Contains("EnableEnvironmentVariable", StringComparison.Ordinal) &&
            gate.Contains("LegacyEnableEnvironmentVariable", StringComparison.Ordinal) &&
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
            string.Join(Environment.NewLine, patch, entry, parentRestore, noReward),
            "VakuuFightPreFinishedSavePatch : IPatchMethod",
            "IPatchMethod.PatchId => \"vakuu-fight-prefinished-save-parent\"",
            "new ModPatchTarget(typeof(CombatRoom), nameof(CombatRoom.ToSerializable))",
            "VakuuFightNoRewardRestorePatch : IPatchMethod",
            "IPatchMethod.PatchId => \"vakuu-fight-no-reward-victory-restore\"",
            "new ModPatchTarget(typeof(CombatRoom), nameof(CombatRoom.OfferRoomEndRewards))",
            "EventNodeBackingField",
            "ClearEventNode(vakuu)",
            "EnterRoomWithoutExitingCurrentRoom(combatRoom, fadeToBlack: true)",
            "Core's CombatRoom.ToSerializable rejects ParentEventId while combat",
            "prefinished-save postfix restore the parent marker only after",
            "Clearing the event node first avoids",
            "ShouldResumeParentEventAfterCombat = true",
            "PreserveParentEventForPreFinishedSave",
            "This is the only place the fight writes ParentEventId.",
            "fight deliberately avoids it",
            "ArmPrefinishedParentRestoreHealSkip",
            "VakuuFightPreFinishedParentRestoreHealPatch : IPatchMethod",
            "IPatchMethod.PatchId => \"vakuu-fight-prefinished-parent-heal-skip\"",
            "new ModPatchTarget(typeof(AncientEventModel), \"BeforeEventStarted\", [typeof(bool)])",
            "ShouldSkipPrefinishedParentRestoreHeal",
            "ConditionalWeakTable<IRunState, ParentRestoreHealSkipState>",
            "ancient.Owner?.RunState",
            "ParentRestoreHealSkips.Remove(runState)",
            "__result = Task.CompletedTask",
            "skipped duplicate Ancient heal",
            "ProceedFromNoRewardVictory",
            "combatRoom.Encounter is not EzmbVakuuTrialEncounter",
            "serializableRoom.ParentEventId =",
            "serializableRoom.ShouldResumeParentEvent = true");
        AssertSourceContains(
            noReward,
            "combatRoom.ShouldResumeParentEventAfterCombat",
            "combatRoom.CombatState.RunState.CurrentRoomCount > 1",
            "ProceedFromMissingParentStackNoRewardVictory(combatRoom)",
            "NMapScreen.Instance?.Open()");
        Assert.DoesNotContain("!combatRoom.IsPreFinished", noReward, StringComparison.Ordinal);
        Assert.DoesNotContain("ProceedFromMalformedNoRewardVictory", noReward, StringComparison.Ordinal);
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
        var patch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");
        var entry = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.Entry.cs");
        var parentRestore = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.ParentRestore.cs");
        var currentDocs = string.Join(
            Environment.NewLine,
            ReadRepoText("docs", "test-ready-development-goal.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "source-design.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "risk-register.md"),
            ReadRepoText("docs", "issues.md"));

        AssertSourceContains(
            string.Join(Environment.NewLine, entry, parentRestore),
            "EnterRoomWithoutExitingCurrentRoom(combatRoom, fadeToBlack: true)",
            "ClearEventNode(vakuu)",
            "EventNodeBackingField",
            "Core's CombatRoom.ToSerializable rejects ParentEventId while combat",
            "ShouldResumeParentEventAfterCombat = true",
            "PreserveParentEventForPreFinishedSave",
            "This is the only place the fight writes ParentEventId.");
        var startFight = SliceBetween(entry, "private static async Task StartFight", "private static void ClearEventNode");
        Assert.DoesNotContain("ParentEventId =", startFight, StringComparison.Ordinal);
        Assert.DoesNotContain("EnterCombatWithoutExitingEventMethod", string.Join(Environment.NewLine, patch, entry, parentRestore), StringComparison.Ordinal);
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

    [LocalSourceFact]
    public void CoreEventCombatRoomParentEventIdShapeStillRequiresPrefinishedSerialization()
    {
        var eventModel = ReadLocalCoreText("Models", "EventModel.cs");
        var combatRoom = ReadLocalCoreText("Rooms", "CombatRoom.cs");

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
    }

    [Fact]
    public void VakuuFightKeepsNoNormalRewardAndFallbackSurfacesSourceVisible()
    {
        var patch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");
        var entry = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.Entry.cs");
        var victoryFlow = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightVictory.cs");
        var victoryChoices = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightVictoryChoices.cs");
        var noReward = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.NoRewardResume.cs");
        var encounter = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightEncounter.cs");
        var apiResearch = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "api-research.md");
        var vakuuSource = string.Join(
            Environment.NewLine,
            Directory.GetFiles(
                    RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu"),
                    "*.cs",
                    SearchOption.TopDirectoryOnly)
                .Select(path => File.ReadAllText(path, Encoding.UTF8)));
        var startFight = SliceBetween(entry, "private static async Task StartFight", "private static void ClearEventNode");
        var createVictoryOptions = SliceBetween(victoryFlow, "private static IEnumerable<EventOption> CreateVictoryOptions", "private static EventOption CreateVictoryFallbackOption");

        AssertSourceContains(
            encounter,
            "public override bool ShouldGiveRewards => false",
            "public override RoomType RoomType => RoomType.Monster");
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
            string.Join(Environment.NewLine, patch, noReward),
            "VakuuFightNoRewardRestorePatch : IPatchMethod",
            "IPatchMethod.PatchId => \"vakuu-fight-no-reward-victory-restore\"",
            "new ModPatchTarget(typeof(CombatRoom), nameof(CombatRoom.OfferRoomEndRewards))",
            "__instance.Encounter is not EzmbVakuuTrialEncounter",
            "__result = VakuuFightService.ProceedFromNoRewardVictory(__instance)",
            "combatRoom.CombatState.RunState.CurrentRoomCount > 1",
            "ProceedFromMissingParentStackNoRewardVictory(combatRoom)",
            "NMapScreen.Instance?.Open()",
            "RunManager.Instance.ProceedFromTerminalRewardsScreen()");
        AssertSourceContains(
            createVictoryOptions,
            "owner is null",
            "using the explicit fallback path",
            "Live restore for this path remains pending",
            "targetChoiceCount = encounter.VictoryChoiceCount",
            "choice.Relic.Owner = owner",
            "SettleVakuuRewards(owner, encounter)",
            "options.Count > 0 ? options : [CreateVictoryFallbackOption(vakuu, combatRoom)]");
        AssertSourceContains(
            victoryFlow,
            "encounter.VictoryGold",
            "encounter.BloodDebtShortfall",
            "CreatureCmd.SetCurrentHp");
        AssertSourceContains(
            victoryChoices,
            "IsEligibleSourceAncientReward(owner, relic)",
            "IsEligibleLothaVictoryChoice(owner, blessingId)",
            "LothaBlessingService.HasMirrorRebuttalCandidates(owner)",
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
    public void VakuuEncounterCustomStateUsesCultureInvariantSaveValues()
    {
        var encounter = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightEncounter.cs");
        var encounterState = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightEncounter.State.cs");

        AssertSourceContains(
            encounterState,
            "using System.Globalization;",
            "private const string BrokenLocksKey = \"BrokenLocks\"",
            "private const string BloodDebtKey = \"BloodDebt\"",
            "private const string DamageRoundKey = \"DamageRound\"",
            "private const string DamageThisRoundKey = \"DamageThisRound\"",
            "private const string DamageLockRoundKey = \"DamageLockRound\"",
            "private const string CashOutOfferedLockKey = \"CashOutOfferedLock\"",
            "private const string CashedOutKey = \"CashedOut\"",
            "public override Dictionary<string, string> SaveCustomState()",
            "[BrokenLocksKey] = BrokenLocks.ToString()",
            "[BloodDebtKey] = BloodDebt.ToString()",
            "[DamageRoundKey] = DamageRound.ToString()",
            "DamageThisRound.ToString(CultureInfo.InvariantCulture)",
            "[DamageLockRoundKey] = DamageLockRound.ToString()",
            "[CashOutOfferedLockKey] = CashOutOfferedLock.ToString()",
            "[CashedOutKey] = CashedOut ? \"1\" : \"0\"",
            "public override void LoadCustomState(Dictionary<string, string> state)",
            "BrokenLocks = ReadInt(state, BrokenLocksKey)",
            "BloodDebt = ReadInt(state, BloodDebtKey)",
            "DamageRound = ReadInt(state, DamageRoundKey, -1)",
            "DamageThisRound = ReadDecimal(state, DamageThisRoundKey)",
            "DamageLockRound = ReadInt(state, DamageLockRoundKey, -1)",
            "CashOutOfferedLock = ReadInt(state, CashOutOfferedLockKey)",
            "CashedOut = ReadBool(state, CashedOutKey)",
            "private static int ReadInt(IReadOnlyDictionary<string, string> state, string key, int fallback = 0)",
            ": fallback",
            "decimal.TryParse(value, CultureInfo.InvariantCulture, out var parsed)",
            ": 0m",
            "value == \"1\" || bool.TryParse(value, out var parsed) && parsed");
        AssertSourceContains(encounter, "internal sealed partial class EzmbVakuuTrialEncounter : ModEncounterTemplate");
        Assert.DoesNotContain("using System.Globalization;", encounter, StringComparison.Ordinal);
        Assert.DoesNotContain("SaveCustomState()", encounter, StringComparison.Ordinal);
        Assert.DoesNotContain("LoadCustomState(Dictionary<string, string> state)", encounter, StringComparison.Ordinal);
        Assert.DoesNotContain("DamageThisRound.ToString()", encounterState, StringComparison.Ordinal);
        Assert.DoesNotContain("decimal.TryParse(value, out var parsed)", encounterState, StringComparison.Ordinal);
    }

}
