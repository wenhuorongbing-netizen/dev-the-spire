using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientUiReadinessGuardTests
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

    private static string RitsuLibDefaultRelicKey(string relicClass)
    {
        var builder = new List<char>("EZ_MICRO_BALANCE_RELIC_");
        for (var i = 0; i < relicClass.Length; i++)
        {
            var current = relicClass[i];
            if (i > 0 && char.IsUpper(current))
            {
                var previous = relicClass[i - 1];
                var nextIsLower = i + 1 < relicClass.Length && char.IsLower(relicClass[i + 1]);
                if (char.IsLower(previous) || char.IsDigit(previous) || nextIsLower)
                {
                    builder.Add('_');
                }
            }

            builder.Add(char.ToUpperInvariant(current));
        }

        return new string(builder.ToArray());
    }

    private static string AncientLocalizationStem(string ancient) =>
        $"EZMB_{ancient.ToUpperInvariant()}";

    private static string RitsuLibDefaultAncientEventKey(string ancient) =>
        $"EZ_MICRO_BALANCE_EVENT_{AncientLocalizationStem(ancient)}";

    private static string RitsuLibDefaultPowerKey(string legacyKey)
    {
        const string legacyPrefix = "EZMICROBALANCE-";
        var suffixIndex = legacyKey.IndexOf('.');
        Assert.True(suffixIndex > legacyPrefix.Length, $"Unexpected power localization key: {legacyKey}");
        return "EZ_MICRO_BALANCE_POWER_" + legacyKey[legacyPrefix.Length..suffixIndex] + legacyKey[suffixIndex..];
    }
}
