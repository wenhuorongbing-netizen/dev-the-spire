using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class AncientUiReadinessGuardTests
{
    private sealed record SceneExpectation(string Ancient, string ScenePath, string RootNode, string EventArtPath);

    private sealed record OptionMarker(
        string RelicClass,
        string AssetMember,
        string AssetPath,
        string RelicKey);

    private sealed record AncientArtRoleSet(
        string Ancient,
        string SourcePath,
        string AssetPrefix,
        string BackgroundScenePath,
        string EventArtPath,
        string MapIconPath,
        string MapIconOutlinePath,
        string RunHistoryIconPath,
        string RunHistoryIconOutlinePath);

    private static readonly SceneExpectation[] ActiveAncientScenes =
    [
        new("Urda", "EZMicroBalance/scenes/events/background_scenes/ezmb_urda.tscn", "EzmbUrdaBackground", "EZMicroBalance/images/events/ezmb_urda.png"),
        new("Morvi", "EZMicroBalance/scenes/events/background_scenes/ezmb_morvi.tscn", "EzmbMorviBackground", "EZMicroBalance/images/events/ezmb_morvi.png"),
        new("Lotha", "EZMicroBalance/scenes/events/background_scenes/ezmb_lotha.tscn", "EzmbLothaBackground", "EZMicroBalance/images/events/ezmb_lotha.png")
    ];

    private static readonly (string Name, string SourcePath, int ExpectedCount)[] SourceOptionCounts =
    [
        ("Urda", "EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaAncient.cs", 4),
        ("Morvi", "EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviAncient.cs", 3),
        ("Lotha", "EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaAncient.cs", 3)
    ];

    private static readonly AncientArtRoleSet[] ActiveAncientArtRoles =
    [
        new(
            "Urda",
            "EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaAncient.cs",
            "UrdaAssetPaths",
            "EZMicroBalance/scenes/events/background_scenes/ezmb_urda.tscn",
            "EZMicroBalance/images/events/ezmb_urda.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon_outline.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon_outline.png"),
        new(
            "Morvi",
            "EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviAncient.cs",
            "MorviAssetPaths",
            "EZMicroBalance/scenes/events/background_scenes/ezmb_morvi.tscn",
            "EZMicroBalance/images/events/ezmb_morvi.png",
            "EZMicroBalance/images/ancients/morvi/ezmb_morvi_map_icon.png",
            "EZMicroBalance/images/ancients/morvi/ezmb_morvi_map_icon_outline.png",
            "EZMicroBalance/images/ancients/morvi/ezmb_morvi_run_history_icon.png",
            "EZMicroBalance/images/ancients/morvi/ezmb_morvi_run_history_icon_outline.png"),
        new(
            "Lotha",
            "EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaAncient.cs",
            "LothaAssetPaths",
            "EZMicroBalance/scenes/events/background_scenes/ezmb_lotha.tscn",
            "EZMicroBalance/images/events/ezmb_lotha.png",
            "EZMicroBalance/images/ancients/lotha/ezmb_lotha_map_icon.png",
            "EZMicroBalance/images/ancients/lotha/ezmb_lotha_map_icon_outline.png",
            "EZMicroBalance/images/ancients/lotha/ezmb_lotha_run_history_icon.png",
            "EZMicroBalance/images/ancients/lotha/ezmb_lotha_run_history_icon_outline.png")
    ];

    private static readonly OptionMarker[] OptionMarkers =
    [
        new("AncientInitialRerollOptionRelic", "AncientRerollAssetPaths.OptionIcon", "EZMicroBalance/images/ancients/common/ancient_reroll_die.png", "EZMICROBALANCE-ANCIENT_INITIAL_REROLL_OPTION_RELIC"),
        new("UrdaSeedbedOptionRelic", "UrdaAssetPaths.SeedbedOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_seedbed.png", "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC"),
        new("UrdaHumusPactOptionRelic", "UrdaAssetPaths.HumusPactOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_humus_pact.png", "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC"),
        new("UrdaMoltingOptionRelic", "UrdaAssetPaths.MoltingOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_molting.png", "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC"),
        new("UrdaMossMapOptionRelic", "UrdaAssetPaths.MossMapOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_moss_map.png", "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC"),
        new("UrdaTrialBranchOptionRelic", "UrdaAssetPaths.TrialBranchOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_trial_branch.png", "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC"),
        new("UrdaShallowRootRelicOptionRelic", "UrdaAssetPaths.ShallowRootRelicOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_shallow_root_relic.png", "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC"),
        new("UrdaRootedRouteOptionRelic", "UrdaAssetPaths.RootedRouteOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_rooted_route.png", "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC"),
        new("UrdaAfterRainOptionRelic", "UrdaAssetPaths.AfterRainOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_after_rain.png", "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC"),
        new("UrdaRootSightOptionRelic", "UrdaAssetPaths.RootSightOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_root_sight.png", "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC"),
        new("UrdaSeedBankOptionRelic", "UrdaAssetPaths.SeedBankOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_seed_bank.png", "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC"),
        new("MorviForbiddenLoanOptionRelic", "MorviAssetPaths.ForbiddenLoanOptionIcon", "EZMicroBalance/images/ancients/morvi/options/morvi_forbidden_loan.png", "EZMICROBALANCE-MORVI_FORBIDDEN_LOAN_OPTION_RELIC"),
        new("MorviMisprintPressOptionRelic", "MorviAssetPaths.MisprintPressOptionIcon", "EZMicroBalance/images/ancients/morvi/options/morvi_misprint_press.png", "EZMICROBALANCE-MORVI_MISPRINT_PRESS_OPTION_RELIC"),
        new("MorviRedInkOverdraftOptionRelic", "MorviAssetPaths.RedInkOverdraftOptionIcon", "EZMicroBalance/images/ancients/morvi/options/morvi_red_ink_overdraft.png", "EZMICROBALANCE-MORVI_RED_INK_OVERDRAFT_OPTION_RELIC"),
        new("MorviOverdueLibraryOptionRelic", "MorviAssetPaths.OverdueLibraryOptionIcon", "EZMicroBalance/images/ancients/morvi/options/morvi_overdue_library.png", "EZMICROBALANCE-MORVI_OVERDUE_LIBRARY_OPTION_RELIC"),
        new("MorviOpenBookExamOptionRelic", "MorviAssetPaths.OpenBookExamOptionIcon", "EZMicroBalance/images/ancients/morvi/options/morvi_open_book_exam.png", "EZMICROBALANCE-MORVI_OPEN_BOOK_EXAM_OPTION_RELIC"),
        new("MorviPaperstormOptionRelic", "MorviAssetPaths.PaperstormOptionIcon", "EZMicroBalance/images/ancients/morvi/options/morvi_paperstorm.png", "EZMICROBALANCE-MORVI_PAPERSTORM_OPTION_RELIC"),
        new("MorviBlueprintProofOptionRelic", "MorviAssetPaths.BlueprintProofOptionIcon", "EZMicroBalance/images/ancients/morvi/options/morvi_blueprint_proof.png", "EZMICROBALANCE-MORVI_BLUEPRINT_PROOF_OPTION_RELIC"),
        new("MorviDebtSettlementOptionRelic", "MorviAssetPaths.DebtSettlementOptionIcon", "EZMicroBalance/images/ancients/morvi/options/morvi_debt_settlement.png", "EZMICROBALANCE-MORVI_DEBT_SETTLEMENT_OPTION_RELIC"),
        new("LothaMirrorRebuttalOptionRelic", "LothaAssetPaths.MirrorRebuttalOptionIcon", "EZMicroBalance/images/ancients/lotha/options/lotha_mirror_rebuttal.png", "EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC"),
        new("LothaMirrorHallEchoOptionRelic", "LothaAssetPaths.MirrorHallEchoOptionIcon", "EZMicroBalance/images/ancients/lotha/options/lotha_mirror_hall_echo.png", "EZMICROBALANCE-LOTHA_MIRROR_HALL_ECHO_OPTION_RELIC"),
        new("LothaPresumptionOptionRelic", "LothaAssetPaths.PresumptionOptionIcon", "EZMicroBalance/images/ancients/lotha/options/lotha_presumption.png", "EZMICROBALANCE-LOTHA_PRESUMPTION_OPTION_RELIC"),
        new("LothaClosedCourtOptionRelic", "LothaAssetPaths.ClosedCourtOptionIcon", "EZMicroBalance/images/ancients/lotha/options/lotha_closed_court.png", "EZMICROBALANCE-LOTHA_CLOSED_COURT_OPTION_RELIC"),
        new("LothaDeferredVerdictOptionRelic", "LothaAssetPaths.DeferredVerdictOptionIcon", "EZMicroBalance/images/ancients/lotha/options/lotha_deferred_verdict.png", "EZMICROBALANCE-LOTHA_DEFERRED_VERDICT_OPTION_RELIC"),
        new("LothaDeathReprieveOptionRelic", "LothaAssetPaths.DeathReprieveOptionIcon", "EZMicroBalance/images/ancients/lotha/options/lotha_death_reprieve.png", "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_OPTION_RELIC"),
        new("LothaSingleSentenceOptionRelic", "LothaAssetPaths.SingleSentenceOptionIcon", "EZMicroBalance/images/ancients/lotha/options/lotha_single_sentence.png", "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_OPTION_RELIC"),
        new("LothaPublicEvidenceOptionRelic", "LothaAssetPaths.PublicEvidenceOptionIcon", "EZMicroBalance/images/ancients/lotha/options/lotha_public_evidence.png", "EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC"),
        new("VakuuFightOptionRelic", "VakuuFightAssetPaths.OptionIcon", "EZMicroBalance/images/ancients/vakuu/options/vakuu_fight.png", "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC")
    ];

    [Fact]
    public void ActiveAncientBackgroundScenesUseControlRootsAndEventArt()
    {
        var exportPreset = ReadRepoText("export_presets.cfg");

        foreach (var scene in ActiveAncientScenes)
        {
            AssertRepoFileExists(scene.ScenePath.Split('/'));
            AssertRepoFileExists(scene.EventArtPath.Split('/'));

            var sceneSource = ReadRepoText(scene.ScenePath.Split('/'));
            Assert.Contains($"[node name=\"{scene.RootNode}\" type=\"Control\"]", sceneSource, StringComparison.Ordinal);
            Assert.Contains("type=\"TextureRect\"", sceneSource, StringComparison.Ordinal);
            Assert.Contains($"path=\"res://{scene.EventArtPath}\"", sceneSource, StringComparison.Ordinal);
            var artworkSource = ExtractNodeBlock(sceneSource, "[node name=\"Artwork\" type=\"TextureRect\" parent=\".\"]");
            Assert.DoesNotContain("anchor_left = ", artworkSource, StringComparison.Ordinal);
            Assert.DoesNotContain("anchor_top = ", artworkSource, StringComparison.Ordinal);
            Assert.Contains("anchor_right = 1.0", artworkSource, StringComparison.Ordinal);
            Assert.Contains("anchor_bottom = 1.0", artworkSource, StringComparison.Ordinal);
            Assert.Contains("expand_mode = 1", sceneSource, StringComparison.Ordinal);
            Assert.Contains("stretch_mode = 5", sceneSource, StringComparison.Ordinal);
            Assert.DoesNotContain("stretch_mode = 6", sceneSource, StringComparison.Ordinal);
            Assert.DoesNotContain("images/ancients", sceneSource, StringComparison.Ordinal);
            Assert.DoesNotContain("map_icon", sceneSource, StringComparison.Ordinal);
            Assert.DoesNotContain("run_history", sceneSource, StringComparison.Ordinal);

            Assert.Contains($"res://{scene.ScenePath}", exportPreset, StringComparison.Ordinal);
            Assert.Contains($"res://{scene.EventArtPath}", exportPreset, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ActiveAncientArtRolesStaySeparated()
    {
        var exportPreset = ReadRepoText("export_presets.cfg");

        foreach (var roleSet in ActiveAncientArtRoles)
        {
            var source = roleSet.Ancient is "Urda" or "Morvi" or "Lotha"
                ? ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", roleSet.Ancient)
                : ReadRepoText(roleSet.SourcePath.Split('/'));
            AssertSourceContains(
                source,
                $"CustomScenePath => {roleSet.AssetPrefix}.BackgroundScene",
                $"CustomMapIconPath => {roleSet.AssetPrefix}.MapIcon",
                $"CustomMapIconOutlinePath => {roleSet.AssetPrefix}.MapIconOutline",
                $"CustomRunHistoryIconPath => {roleSet.AssetPrefix}.RunHistoryIcon",
                $"CustomRunHistoryIconOutlinePath => {roleSet.AssetPrefix}.RunHistoryIconOutline");

            foreach (var (member, path) in new[]
            {
                ("MapIcon", roleSet.MapIconPath),
                ("MapIconOutline", roleSet.MapIconOutlinePath),
                ("RunHistoryIcon", roleSet.RunHistoryIconPath),
                ("RunHistoryIconOutline", roleSet.RunHistoryIconOutlinePath)
            })
            {
                Assert.StartsWith("EZMicroBalance/images/ancients/", path, StringComparison.Ordinal);
                Assert.DoesNotContain("/images/events/", path, StringComparison.Ordinal);
                Assert.NotEqual(roleSet.EventArtPath, path);
                Assert.NotEqual(roleSet.BackgroundScenePath, path);
                Assert.Contains($"{member} => $\"{{MainFile.ResPath}}/{path["EZMicroBalance/".Length..]}\"", source, StringComparison.Ordinal);
                AssertRepoFileExists(path.Split('/'));
                Assert.Contains($"res://{path}", exportPreset, StringComparison.Ordinal);
            }

            Assert.StartsWith("EZMicroBalance/scenes/events/background_scenes/", roleSet.BackgroundScenePath, StringComparison.Ordinal);
            Assert.EndsWith(".tscn", roleSet.BackgroundScenePath, StringComparison.Ordinal);
            Assert.StartsWith("EZMicroBalance/images/events/", roleSet.EventArtPath, StringComparison.Ordinal);
            AssertRepoFileExists(roleSet.BackgroundScenePath.Split('/'));
            AssertRepoFileExists(roleSet.EventArtPath.Split('/'));
            Assert.Contains($"BackgroundScene => $\"{{MainFile.ResPath}}/{roleSet.BackgroundScenePath["EZMicroBalance/".Length..]}\"", source, StringComparison.Ordinal);
            Assert.Contains($"res://{roleSet.BackgroundScenePath}", exportPreset, StringComparison.Ordinal);
            Assert.Contains($"res://{roleSet.EventArtPath}", exportPreset, StringComparison.Ordinal);
        }
    }

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
    public void AncientAndVakuuArtAssetsUseStableUiSizedRoles()
    {
        foreach (var scene in ActiveAncientScenes)
        {
            Assert.Equal((1920, 1080), ReadPngDimensions(RepoPath(scene.EventArtPath.Split('/'))));
        }

        foreach (var roleSet in ActiveAncientArtRoles)
        {
            Assert.Equal((128, 128), ReadPngDimensions(RepoPath(roleSet.MapIconPath.Split('/'))));
            Assert.Equal((128, 128), ReadPngDimensions(RepoPath(roleSet.MapIconOutlinePath.Split('/'))));
            Assert.Equal((128, 128), ReadPngDimensions(RepoPath(roleSet.RunHistoryIconPath.Split('/'))));
            Assert.Equal((128, 128), ReadPngDimensions(RepoPath(roleSet.RunHistoryIconOutlinePath.Split('/'))));
        }

        foreach (var marker in OptionMarkers)
        {
            Assert.Equal((128, 128), ReadPngDimensions(RepoPath(marker.AssetPath.Split('/'))));
        }

        Assert.Equal((1920, 1080), ReadPngDimensions(RepoPath("EZMicroBalance", "images", "encounters", "vakuu_trial_backdrop.png")));
        Assert.Equal((512, 384), ReadPngDimensions(RepoPath("EZMicroBalance", "images", "monsters", "vakuu_trial.png")));
        Assert.Equal((250, 190), ReadPngDimensions(RepoPath("EZMicroBalance", "images", "card_portraits", "vakuu_temptation.png")));
        Assert.Equal((1000, 760), ReadPngDimensions(RepoPath("EZMicroBalance", "images", "card_portraits", "big", "vakuu_temptation.png")));
    }

    [Fact]
    public void OptionMarkerRelicsHaveArtAndBilingualLocalizationCoverage()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        foreach (var marker in OptionMarkers)
        {
            AssertSourceContains(source, marker.RelicClass, marker.AssetMember);
            Assert.StartsWith("EZMicroBalance/images/ancients/", marker.AssetPath, StringComparison.Ordinal);
            Assert.DoesNotContain("/images/events/", marker.AssetPath, StringComparison.Ordinal);
            Assert.DoesNotContain("map_icon", marker.AssetPath, StringComparison.Ordinal);
            Assert.DoesNotContain("run_history", marker.AssetPath, StringComparison.Ordinal);
            Assert.NotEqual("EZMicroBalance/images/relics/relic.png", marker.AssetPath);
            Assert.DoesNotContain($"{marker.AssetMember} => $\"{{MainFile.ResPath}}/images/relics/relic.png\"", source, StringComparison.Ordinal);
            AssertRepoFileExists(marker.AssetPath.Split('/'));
            Assert.Contains($"res://{marker.AssetPath}", exportPreset, StringComparison.Ordinal);

            foreach (var suffix in new[] { ".title", ".description", ".flavor" })
            {
                AssertLocalizedValue(engRelics, marker.RelicKey + suffix);
                AssertLocalizedValue(zhsRelics, marker.RelicKey + suffix);
            }
        }
    }

    [Fact]
    public void InitialAncientRewardsExposeOneUseRerollOption()
    {
        var reroll = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientInitialOptionReroll.cs");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var urda = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaAncient.Options.cs");
        var morvi = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviAncient.Options.cs");
        var lotha = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaAncient.Options.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");

        AssertSourceContains(
            reroll,
            "OptionId = \"ezmb_reroll_initial_options\"",
            "AncientInitialOptionRerollStateKey",
            "BuildEventKey",
            "ThatWontSaveToChoiceHistory",
            "AncientRerollAssetPaths.OptionIcon",
            "AncientInitialRerollOptionRelic");
        Assert.Contains("SavedSpireField<Player, string> AncientInitialOptionRerollStateKey", savedFields, StringComparison.Ordinal);

        foreach (var source in new[] { urda, morvi, lotha })
        {
            AssertSourceContains(
                source,
                "AncientInitialOptionReroll.CanOffer",
                "AncientInitialOptionReroll.CreateOption",
                "AncientInitialOptionReroll.TrySpend",
                "RerollInitialOptions",
                "includeReroll: false");
        }

        foreach (var key in new[]
        {
            "EZMB_URDA.pages.INITIAL.options.ezmb_reroll_initial_options",
            "EZMB_MORVI.pages.INITIAL.options.ezmb_reroll_initial_options",
            "EZMB_LOTHA.pages.INITIAL.options.ezmb_reroll_initial_options"
        })
        {
            AssertLocalizedValue(engAncients, key + ".title");
            AssertLocalizedValue(engAncients, key + ".description");
            AssertLocalizedValue(zhsAncients, key + ".title");
            AssertLocalizedValue(zhsAncients, key + ".description");
        }
    }

    [Fact]
    public void VakuuFightHasDedicatedEncounterSceneMonsterAndLocalization()
    {
        var encounter = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightEncounter.cs");
        var monster = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuTrialMonster.cs");
        var assetPaths = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightAssetPaths.cs");
        var scene = ReadRepoText("EZMicroBalance", "scenes", "encounters", "ezmb_vakuu_trial.tscn");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var engMonsters = JsonStringMap("EZMicroBalance", "localization", "eng", "monsters.json");
        var zhsMonsters = JsonStringMap("EZMicroBalance", "localization", "zhs", "monsters.json");

        AssertSourceContains(
            encounter,
            "CustomScenePath => VakuuFightAssetPaths.EncounterScene",
            "HasScene => true",
            "Slots => [VakuuSlot]",
            "ModelDb.Monster<EzmbVakuuTrialMonster>()");
        Assert.DoesNotContain("OwlMagistrate", encounter, StringComparison.Ordinal);
        AssertSourceContains(
            monster,
            "CustomMonsterModel",
            "CustomVisualPath => VakuuFightAssetPaths.MonsterVisual",
            "VisualScale = 1.25f",
            "GenerateMoveStateMachine",
            "OpeningOfferMove",
            "KnifeRainMove",
            "GildedHideMove",
            "DebtCallMove");
        AssertSourceContains(
            assetPaths,
            "OptionIcon => $\"{MainFile.ResPath}/images/ancients/vakuu/options/vakuu_fight.png\"",
            "MonsterVisual => $\"{MainFile.ResPath}/images/monsters/vakuu_trial.png\"");
        AssertSourceContains(
            scene,
            "res://EZMicroBalance/images/encounters/vakuu_trial_backdrop.png",
            "[node name=\"EzmbVakuuTrialEncounter\" type=\"Control\"]",
            "offset_right = 1920.0",
            "offset_bottom = 1080.0",
            "[node name=\"Vakuu\" type=\"Marker2D\" parent=\".\"]");
        Assert.DoesNotContain("images/card_portraits/big/vakuu_temptation.png", scene, StringComparison.Ordinal);

        AssertLocalizedValue(engMonsters, "EZMB_VAKUU_TRIAL_MONSTER.name");
        AssertLocalizedValue(zhsMonsters, "EZMB_VAKUU_TRIAL_MONSTER.name");
        Assert.Contains("res://EZMicroBalance/images/monsters/vakuu_trial.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/images/encounters/vakuu_trial_backdrop.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/scenes/encounters/ezmb_vakuu_trial.tscn", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/localization/eng/monsters.json", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/localization/zhs/monsters.json", exportPreset, StringComparison.Ordinal);
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
    public void ForceAncientGatesAreSourceBackedAndDocumentedForManualClickedUiEvidence()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion");
        AssertSourceContains(
            source,
            "EZMB_FORCE_ANCIENT",
            "SPIREPLUS_FORCE_ANCIENT",
            "EZMB_FORCE_URDA_BLESSING",
            "SPIREPLUS_FORCE_URDA_BLESSING",
            "EZMB_FORCE_MORVI_BLESSING",
            "SPIREPLUS_FORCE_MORVI_BLESSING",
            "EZMB_FORCE_LOTHA_BLESSING",
            "SPIREPLUS_FORCE_LOTHA_BLESSING",
            "EZMB_DISABLE_URDA",
            "SPIREPLUS_DISABLE_URDA",
            "EZMB_DISABLE_MORVI",
            "SPIREPLUS_DISABLE_MORVI",
            "EZMB_DISABLE_LOTHA",
            "SPIREPLUS_DISABLE_LOTHA",
            "EZMB_ENABLE_VAKUU_FIGHT",
            "SPIREPLUS_ENABLE_VAKUU_FIGHT",
            "EZMB_DISABLE_VAKUU_FIGHT",
            "SPIREPLUS_DISABLE_VAKUU_FIGHT");

        var docs = string.Join(
            Environment.NewLine,
            ReadRepoText("docs", "private-beta-verification-handoff.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md"));
        AssertSourceContains(
            docs,
            "scripts/collect-ancient-ui-evidence.ps1",
            "ancient-ui-evidence-plan.json",
            "manual-instructions.md",
            "spireplus_test_ancient URDA confirm",
            "spireplus_test_ancient MORVI confirm",
            "spireplus_test_ancient LOTHA confirm",
            "spireplus_test_ancient VAKUU confirm",
            "spireplus_test_ancient VAKUU confirm fight",
            "starts an unsaved single-player test run",
            "Expected visible option counts are Urda 4, Morvi 3, Lotha 3",
            "Vakuu 3 by default",
            "one fight option",
            "ancient EZMB_URDA",
            "ancient EZMB_MORVI",
            "ancient EZMB_LOTHA",
            "ancient VAKUU");
    }

    [Fact]
    public void AncientClickedUiEvidenceHelperIsSourceGuardedAndDocumented()
    {
        var helper = ReadRepoText("scripts", "collect-ancient-ui-evidence.ps1");
        AssertRepoFileExists("scripts", "collect-ancient-ui-evidence.ps1");
        AssertSourceContains(
            helper,
            "[ValidateSet('URDA', 'MORVI', 'LOTHA', 'VAKUU')]",
            "ancient-ui-evidence-plan.json",
            "manual-instructions.md",
            "SPIREPLUS_FORCE_ANCIENT",
            "EZMB_FORCE_ANCIENT",
            "SPIREPLUS_FORCE_VAKUU_FIGHT",
            "EZMB_FORCE_VAKUU_FIGHT",
            "PreferredUnsavedDevConsoleCommand",
            "spireplus_test_ancient URDA confirm",
            "spireplus_test_ancient VAKUU confirm fight",
            "capture-spire-window.ps1",
            "URDA = 4",
            "MORVI = 3",
            "LOTHA = 3",
            "VakuuNormal = 3",
            "VakuuFightOptInSinglePlayer = 4",
            "VakuuForceFight = 1",
            "ExpectedOptionCountForThisRun",
            "check-spire-window-preflight.ps1",
            "spire-plus-live-session.ps1",
            "-NoPreflight",
            "-ForceVakuuFight is valid only when -Ancient VAKUU is used.",
            "-Mode', 'Restore'");

        var docs = string.Join(
            Environment.NewLine,
            ReadRepoText("scripts", "README.md"),
            ReadRepoText("PROJECT_STATE.md"),
            ReadRepoText("docs", "issues.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md"));
        AssertSourceContains(
            docs,
            "scripts/collect-ancient-ui-evidence.ps1",
            "ancient-ui-evidence-plan.json",
            "manual-instructions.md",
            "spireplus_test_ancient URDA confirm",
            "This helper and command prepare UI evidence",
            "Keep this section pending until Urda, Morvi, Lotha, and Vakuu clicked-screen screenshots/logs are captured.");

        var issues = ReadRepoText("docs", "issues.md");
        AssertSourceContains(
            issues,
            "ANCIENT-CLICKED-UI/LIVE-GAMEPLAY",
            "scripts/collect-ancient-ui-evidence.ps1");

        Assert.DoesNotContain("helper verifies clicked UI", docs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("helper proves clicked UI", docs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("clicked UI verified by the helper", docs, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SpirePlusAncientUiSmokeCommandStartsOnlyUnsavedFreshRuns()
    {
        var command = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Diagnostics", "SpirePlusAncientLiveTestConsoleCmd.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs"));
        AssertSourceContains(
            command,
            "SpirePlusAncientLiveTestConsoleCmd",
            "CmdName => \"spireplus_test_ancient\"",
            "ConfirmationToken = \"confirm\"",
            "FightToken = \"fight\"",
            "RunManager.Instance.IsInProgress",
            "Return to the main menu before using it.",
            "shouldSave: false",
            "ModelDb.Character<Ironclad>()",
            "ActModel.GetDefaultList()",
            "RoomType.Event",
            "MapPointType.Ancient",
            "ModelDb.AncientEvent<EzmbUrda>()",
            "ModelDb.AncientEvent<EzmbMorvi>()",
            "ModelDb.AncientEvent<EzmbLotha>()",
            "ModelDb.AncientEvent<Vakuu>()",
            "RunManager.Instance.DebugOnlyGetState()",
            "VakuuFightFeatureGate.ArmCommandForceFight(commandForceFightRunState)",
            "VakuuFightFeatureGate.ClearCommandForceFight(commandForceFightRunState)",
            "var forceFight = VakuuFightFeatureGate.ShouldForceFightForRun(runState)",
            "VakuuFightFeatureGate.IsFightEnabledForRun(runState, forceFight)",
            "VakuuFightFeatureGate.ConsumeCommandForceFightForRun(runState)",
            "[HarmonyPatch(typeof(EventModel), nameof(EventModel.BeginEvent))]",
            "VakuuFightFeatureGate.HasCommandForceFightForRun(runState)",
            "VakuuFightFeatureGate.ClearCommandForceFightWhenBeginEventCompletes(__result, runState)",
            "finally",
            "commandForcedFightRun = new WeakReference<IRunState>(runState)",
            "ReferenceEquals(target, runState)");

        var vakuuCommandAndFeatureSource = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Diagnostics", "SpirePlusAncientLiveTestConsoleCmd.cs"),
            ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu"));
        AssertNoProcessEnvironmentMutationForVakuuCommand(vakuuCommandAndFeatureSource);

        Assert.DoesNotContain("shouldSave: true", command, StringComparison.Ordinal);

        var helperDocs = string.Join(
            Environment.NewLine,
            ReadRepoText("scripts", "README.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md"));
        AssertSourceContains(
            helperDocs,
            "starts an unsaved single-player test run",
            "refuses to run over an existing run",
            "only UI smoke");
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
            "EZMB_RELEASE_EVIDENCE_LOG=1",
            "Ancient reward/fight option selection logs include the Ancient, blessing id or option id, selected marker relic type, forced flag, run id, player slot, and network mode.");
    }

    private static void AssertLocalizedValue(IReadOnlyDictionary<string, string> values, string key)
    {
        Assert.True(values.TryGetValue(key, out var value), $"Missing localization key: {key}");
        Assert.False(string.IsNullOrWhiteSpace(value), $"Empty localization key: {key}");
        Assert.DoesNotContain("TODO", value, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\uFFFD", value, StringComparison.Ordinal);
    }

    private static string ExtractNodeBlock(string sceneSource, string nodeHeader)
    {
        var start = sceneSource.IndexOf(nodeHeader, StringComparison.Ordinal);
        Assert.True(start >= 0, $"Missing scene node: {nodeHeader}");
        var next = sceneSource.IndexOf("\n[node ", start + nodeHeader.Length, StringComparison.Ordinal);
        return next < 0 ? sceneSource[start..] : sceneSource[start..next];
    }

    private static void AssertNoProcessEnvironmentMutationForVakuuCommand(string source)
    {
        Assert.False(
            Regex.IsMatch(
                source,
                @"\b(?:System\s*\.\s*)?Environment\s*\.\s*SetEnvironmentVariable\s*\(",
                RegexOptions.CultureInvariant),
            "Vakuu command-scoped force fight must use a run-scoped marker, not process environment mutation.");
        Assert.DoesNotContain("ForceVakuuFightEnvironmentForCommand", source, StringComparison.Ordinal);
    }

}
