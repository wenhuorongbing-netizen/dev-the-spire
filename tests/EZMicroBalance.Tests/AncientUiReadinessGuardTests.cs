using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientUiReadinessGuardTests
{
    [Fact]
    public void ActiveAncientInitialOptionsHaveExpectedCountsAndLoggedFallbacks()
    {
        foreach (var (name, path, expectedCount) in SourceOptionCounts)
        {
            var source = name is "Urda" or "Morvi" or "Lotha"
                ? ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", name)
                : ReadRepoText(path.Split('/'));
            AssertSourceContains(
                source,
                $"private const int ExpectedInitialOptionCount = {expectedCount};",
                "TakeFallbackOptions(options, includeReroll: true)",
                "forced blessing",
                "did not match any option; showing fallback options.",
                "options.Count == 0",
                "event will finish instead of presenting a blank Ancient screen",
                "source-backed option(s), expected",
                "candidates.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList()",
                "AncientInitialOptionReroll.CanOffer");
            Assert.DoesNotContain("Take(3).ToList()", source, StringComparison.Ordinal);
            Assert.Contains(name, source, StringComparison.Ordinal);
        }

        var vakuuPatch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");
        var vakuuVictory = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightVictory.cs");
        var vakuuGate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs");
        AssertSourceContains(
            vakuuPatch,
            "var forceFight = VakuuFightFeatureGate.ShouldForceFightForRun(runState)",
            "VakuuFightFeatureGate.IsFightEnabledForRun(runState, forceFight)",
            "VakuuFightFeatureGate.ConsumeCommandForceFightForRun(runState)",
            "[HarmonyPatch(typeof(EventModel), nameof(EventModel.BeginEvent))]",
            "VakuuFightFeatureGate.HasCommandForceFightForRun(runState)",
            "VakuuFightFeatureGate.ClearCommandForceFightWhenBeginEventCompletes(__result, runState)",
            "if (forceFight)",
            "__result = [fightOption]",
            "__result = __result.Concat([fightOption]).ToList()");
        AssertSourceContains(
            vakuuVictory,
            "targetChoiceCount = encounter.VictoryChoiceCount",
            "options.Count > 0 ? options : [CreateVictoryFallbackOption(vakuu, combatRoom)]",
            "CreateVictoryFallbackOption");
        AssertSourceContains(
            vakuuGate,
            "runState.Players.Count == 1",
            "EZMB_ENABLE_VAKUU_FIGHT",
            "SPIREPLUS_ENABLE_VAKUU_FIGHT",
            "EZMB_FORCE_VAKUU_FIGHT",
            "SPIREPLUS_FORCE_VAKUU_FIGHT",
            "private static WeakReference<IRunState>? commandForcedFightRun",
            "ShouldForceFightForRun(IRunState runState)",
            "ArmCommandForceFight(IRunState runState)",
            "ClearCommandForceFight(IRunState runState)",
            "ConsumeCommandForceFightForRun(IRunState runState)",
            "HasCommandForceFightForRun(IRunState runState)",
            "ClearCommandForceFightWhenBeginEventCompletes(Task beginEventTask, IRunState runState)",
            "finally",
            "ReferenceEquals(target, runState)",
            "ShouldEnableFight");
    }

    [Fact]
    public void InitialAncientRewardsExposeOneUseRerollOption()
    {
        var reroll = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientInitialOptionReroll.cs");
        var neowReroll = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "NeowInitialOptionRerollPatch.cs");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var urda = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaAncient.Options.cs");
        var morvi = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviAncient.Options.cs");
        var lotha = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaAncient.Options.cs");
        var feedback = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "SpirePlusFeedback.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");

        AssertSourceContains(
            reroll,
            "OptionId = \"ezmb_reroll_initial_options\"",
            "AncientInitialOptionRerollStateKey",
            "BuildEventKey",
            "ThatWontSaveToChoiceHistory",
            "AncientRerollAssetPaths.OptionIcon",
            "AncientInitialRerollOptionRelic",
            "IsFirstActAncientReward",
            "ancient is Neow",
            "CurrentActIndex == 0",
            "ReplaceGeneratedOptionsAndRefreshScreen",
            "SetEventStateMethod");
        Assert.Contains("SavedSpireField<Player, string> AncientInitialOptionRerollStateKey", savedFields, StringComparison.Ordinal);

        AssertSourceContains(
            neowReroll,
            "[HarmonyPatch(typeof(Neow), \"GenerateInitialOptions\")]",
            "ExpectedNeowOptionCount = 3",
            "NEOW.pages.INITIAL.options.",
            "RunState.Modifiers.Count > 0",
            "AncientInitialOptionReroll.CanOffer",
            "AncientInitialOptionReroll.CreateOption",
            "AncientInitialOptionReroll.TrySpend",
            "ReplaceGeneratedOptionsAndRefreshScreen",
            "SpirePlusFeedback.ConfirmChoiceRefresh();");

        foreach (var source in new[] { urda, morvi, lotha })
        {
            AssertSourceContains(
                source,
                "AncientInitialOptionReroll.CanOffer",
                "AncientInitialOptionReroll.CreateOption",
                "AncientInitialOptionReroll.TrySpend",
                "RerollInitialOptions",
                "includeReroll: false",
                "SpirePlusFeedback.ConfirmChoiceRefresh();");
        }

        AssertSourceContains(
            feedback,
            "public static void ConfirmChoiceRefresh()",
            "SfxCmd.Play(RelicTriggerSfx)",
            "NRelicFlashVfx.Create(sourceRelic)",
            "NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short)");

        foreach (var key in new[]
        {
            "EZMB_URDA.pages.INITIAL.options.ezmb_reroll_initial_options",
            "NEOW.pages.INITIAL.options.ezmb_reroll_initial_options",
            "EZMB_MORVI.pages.INITIAL.options.ezmb_reroll_initial_options",
            "EZMB_LOTHA.pages.INITIAL.options.ezmb_reroll_initial_options"
        })
        {
            AssertLocalizedValue(engAncients, key + ".title");
            AssertLocalizedValue(engAncients, key + ".description");
            AssertLocalizedValue(zhsAncients, key + ".title");
            AssertLocalizedValue(zhsAncients, key + ".description");
        }

        Assert.Contains("Act [blue]1[/blue]", engAncients["NEOW.pages.INITIAL.options.ezmb_reroll_initial_options.description"], StringComparison.Ordinal);
        Assert.Contains("第[blue]1[/blue]幕", zhsAncients["NEOW.pages.INITIAL.options.ezmb_reroll_initial_options.description"], StringComparison.Ordinal);
    }

    [Fact]
    public void AncientRewardSelectionsObtainVisibleMarkerRelics()
    {
        var rewardService = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientRewardRelicService.cs");
        AssertSourceContains(
            rewardService,
            "ObtainSelectionRelicIfMissing<T>",
            "owner.GetRelic<T>() is not null",
            "ModelDb.Relic<T>().ToMutable()",
            "await RelicCmd.Obtain(relic, owner)");

        var urdaAncient = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Urda");
        AssertSourceContains(
            urdaAncient,
            "() => SelectBlessing<T>(blessingId)",
            "private async Task SelectBlessing<T>(string blessingId)",
            "where T : RelicModel",
            "ModelDb.Relic<T>().ToMutable()",
            "EventOption.FromRelic(relic, this, () => SelectBlessing<T>(blessingId), InitialOptionKey(blessingId))",
            "option.HoverTips = option.HoverTips.Concat(hoverTips ?? []).ToList()",
            "UrdaRewardSelectionService.SelectBlessing<T>",
            "await AncientRewardRelicService.ObtainSelectionRelicIfMissing<T>(owner, blessingId)");
        Assert.DoesNotContain("() => SelectBlessing(blessingId)", urdaAncient, StringComparison.Ordinal);
        Assert.DoesNotContain("new EventOption(this, () => SelectBlessing<T>(blessingId), InitialOptionKey(blessingId), hoverTips ?? [])", urdaAncient, StringComparison.Ordinal);

        var morviAncientSelection = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi");
        AssertSourceContains(
            morviAncientSelection,
            "() => SelectBlessing<T>(blessingId)",
            "private async Task SelectBlessing<T>(string blessingId)",
            "where T : RelicModel",
            "ModelDb.Relic<T>().ToMutable()",
            "EventOption.FromRelic(relic, this, () => SelectBlessing<T>(blessingId), InitialOptionKey(blessingId))",
            "option.HoverTips = option.HoverTips.Concat(hoverTips ?? []).ToList()",
            "await AncientRewardRelicService.ObtainSelectionRelicIfMissing<T>(Owner, blessingId)");
        Assert.DoesNotContain("() => SelectBlessing(blessingId)", morviAncientSelection, StringComparison.Ordinal);
        Assert.DoesNotContain("new EventOption(this, () => SelectBlessing<T>(blessingId), InitialOptionKey(blessingId), hoverTips ?? [])", morviAncientSelection, StringComparison.Ordinal);

        var lothaAncient = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha");
        AssertSourceContains(
            lothaAncient,
            "() => SelectBlessing<T>(blessingId)",
            "private async Task SelectBlessing<T>(string blessingId)",
            "where T : RelicModel",
            "ModelDb.Relic<T>().ToMutable()",
            "EventOption.FromRelic(relic, this, () => SelectBlessing<T>(blessingId), InitialOptionKey(blessingId))",
            "option.HoverTips = option.HoverTips.Concat(hoverTips ?? []).ToList()",
            ".Where(IsCurrentlyAvailableOption)",
            "LothaBlessingService.HasMirrorRebuttalCandidates(Owner)",
            "LothaRewardSelectionService.SelectBlessing<T>",
            "await AncientRewardRelicService.ObtainSelectionRelicIfMissing<T>(owner, blessingId)");
        Assert.DoesNotContain("() => SelectBlessing(blessingId)", lothaAncient, StringComparison.Ordinal);
        Assert.DoesNotContain("new EventOption(this, () => SelectBlessing<T>(blessingId), InitialOptionKey(blessingId), hoverTips ?? [])", lothaAncient, StringComparison.Ordinal);

        var morviAncient = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi");
        Assert.Contains("SetEventState(InitialDescription, GenerateInitialOptions())", morviAncient, StringComparison.Ordinal);
        var lothaMirror = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.MirrorRebuttal.cs");
        Assert.Contains("internal static bool HasMirrorRebuttalCandidates(Player player)", lothaMirror, StringComparison.Ordinal);

        var vakuuSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu");
        AssertSourceContains(
            vakuuSource,
            "await AncientRewardRelicService.ObtainSelectionRelicIfMissing<VakuuFightOptionRelic>",
            "FightOptionKey",
            "EnterRoomWithoutExitingCurrentRoom(combatRoom, fadeToBlack: true)",
            "ClearEventNode(vakuu)",
            "GetLothaAct3AncientRelicChoices",
            "LothaRewardSelectionService.SelectBlessing<T>");
    }

    [Fact]
    public void ActiveDocsDoNotClaimClickedAncientUiVerifiedWithoutRuntimeEvidence()
    {
        var activeDocs = string.Join(
            Environment.NewLine,
            ReadRepoText("PROJECT_STATE.md"),
            ReadRepoText("docs", "issues.md"),
            ReadRepoText("docs", "test-ready-development-goal.md"),
            ReadRepoText("docs", "private-beta-verification-handoff.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "work-log.md"));

        Assert.Contains("clicked Ancient UI", activeDocs, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("pending", activeDocs, StringComparison.OrdinalIgnoreCase);
        foreach (var prohibited in new[]
        {
            "clicked Ancient UI verified",
            "clicked UI verified",
            "Ancient UI verified",
            "clicked Ancient UI passed",
            "clicked live Ancient UI passed",
            "clicked UI verification passed"
        })
        {
            Assert.DoesNotContain(prohibited, activeDocs, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void AncientSelectionLogsCarryRunPlayerAndForcedContext()
    {
        var helper = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSelectionEvidenceLog.cs");
        var urdaRows = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaAncient.OptionRows.cs");
        var morviRows = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviAncient.OptionRows.cs");
        var lothaRows = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaAncient.OptionRows.cs");
        var vakuuEntry = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.Entry.cs");
        var testReadyGoal = ReadRepoText("docs", "test-ready-development-goal.md");

        AssertSourceContains(
            helper,
            "ReleaseEvidenceLog.Log",
            "\"AncientSelection\"",
            "\"blessing_selected\"",
            "\"blessing_selection_failed\"",
            "\"option_selected\"",
            "[\"ancient\"] = ancientId",
            "[\"blessing\"] = blessingId",
            "[\"option\"] = optionId",
            "[\"relic\"] = relicType",
            "[\"forced\"] = forced",
            "playerSlot={PlayerSlot(player)}",
            "run={RunId(player)}",
            "player.RunState.GetPlayerSlotIndex(player)");
        AssertSourceContains(
            urdaRows,
            "AncientSelectionEvidenceLog.LogBlessingSelected",
            "\"Urda\"",
            "typeof(T).Name",
            "!string.IsNullOrWhiteSpace(UrdaFeatureGate.ForcedBlessing)");
        AssertSourceContains(
            morviRows,
            "AncientSelectionEvidenceLog.LogBlessingSelected",
            "AncientSelectionEvidenceLog.LogBlessingSelectionFailed",
            "\"Morvi\"",
            "selection_rejected",
            "!string.IsNullOrWhiteSpace(MorviFeatureGate.ForcedBlessing)");
        AssertSourceContains(
            lothaRows,
            "AncientSelectionEvidenceLog.LogBlessingSelected",
            "\"Lotha\"",
            "typeof(T).Name",
            "!string.IsNullOrWhiteSpace(LothaFeatureGate.ForcedBlessing)");
        AssertSourceContains(
            vakuuEntry,
            "var forcedOption = vakuu.Owner?.RunState is RunState runState",
            "VakuuFightFeatureGate.ShouldForceFightForRun(runState)",
            "() => StartFight(vakuu, forcedOption)",
            "AncientSelectionEvidenceLog.LogOptionSelected",
            "\"Vakuu\"",
            "nameof(VakuuFightOptionRelic)",
            "forcedOption");
        AssertSourceContains(
            testReadyGoal,
            "SPIREPLUS_FORCE_ANCIENT=URDA",
            "SPIREPLUS_FORCE_MORVI_BLESSING=morvi_forbidden_loan",
            "SPIREPLUS_FORCE_LOTHA_BLESSING=lotha_death_reprieve",
            "SPIREPLUS_DISABLE_URDA=1",
            "SPIREPLUS_ENABLE_VAKUU_FIGHT=1",
            "SPIREPLUS_RELEASE_EVIDENCE_LOG=1",
            "Ancient reward/fight option selection logs include the Ancient, blessing id or option id, selected marker relic type, forced flag, run id, player slot, and network mode.");
    }

    private static void AssertLocalizedValue(IReadOnlyDictionary<string, string> values, string key)
    {
        Assert.True(values.TryGetValue(key, out var value), $"Missing localization key: {key}");
        Assert.False(string.IsNullOrWhiteSpace(value), $"Empty localization key: {key}");
        Assert.DoesNotContain("TODO", value, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\uFFFD", value, StringComparison.Ordinal);
    }
}
