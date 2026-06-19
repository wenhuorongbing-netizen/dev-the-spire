using System.Collections.Generic;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class UrdaReleaseCoverageGuardTests
{
    private static void AssertUrdaLocalizationCoverage(
        string engCards,
        string zhsCards,
        string engCardRewardUi,
        string zhsCardRewardUi,
        IReadOnlyDictionary<string, string> engCardRewardUiMap,
        IReadOnlyDictionary<string, string> zhsCardRewardUiMap,
        IReadOnlyDictionary<string, string> engRelics,
        IReadOnlyDictionary<string, string> zhsRelics,
        IReadOnlyDictionary<string, string> engAncients,
        IReadOnlyDictionary<string, string> zhsAncients)
    {
        AssertSourceContains(
            engCards,
            "EZMB_URDA_SEEDLING.title",
            "EZMB_WITHERED_HUSK.title");
        AssertSourceContains(
            zhsCards,
            "EZMB_URDA_SEEDLING.title",
            "EZMB_WITHERED_HUSK.title");
        Assert.Contains("OPTION_EZMB_URDA_SEEDBED.name", engCardRewardUi, StringComparison.Ordinal);
        Assert.Contains("OPTION_EZMB_URDA_SEEDBED.name", zhsCardRewardUi, StringComparison.Ordinal);
        Assert.Equal("Compost Reward", engCardRewardUiMap["OPTION_EZMB_URDA_HUMUS_PACT.name"]);
        AssertLocalizedKeys(
            [
                "OPTION_EZMB_URDA_SEEDBED.name",
                "OPTION_EZMB_URDA_HUMUS_PACT.name",
                "OPTION_EZMB_URDA_SEED_BANK_STORE.name"
            ],
            engCardRewardUiMap,
            zhsCardRewardUiMap,
            "Urda card-reward option localization");
        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_ELITE_ROOT_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_ELITE_ROOT_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_ELITE_ROOT_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.flavor"
            ],
            engRelics,
            zhsRelics,
            "Urda option relic localization");
        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-EZMB_URDA.talk.firstVisitEver.0-0.ancient",
                "EZMICROBALANCE-EZMB_URDA.talk.ANY.0-0r.ancient",
                "EZMB_URDA.pages.INITIAL.options.urda_seedbed.title",
                "EZMB_URDA.pages.INITIAL.options.urda_seedbed.description",
                "EZMB_URDA.pages.INITIAL.options.urda_humus_pact.title",
                "EZMB_URDA.pages.INITIAL.options.urda_humus_pact.description",
                "EZMB_URDA.pages.INITIAL.options.urda_molting.title",
                "EZMB_URDA.pages.INITIAL.options.urda_molting.description",
                "EZMB_URDA.pages.INITIAL.options.urda_moss_map.title",
                "EZMB_URDA.pages.INITIAL.options.urda_moss_map.description",
                "EZMB_URDA.pages.INITIAL.options.urda_trial_branch.title",
                "EZMB_URDA.pages.INITIAL.options.urda_trial_branch.description",
                "EZMB_URDA.pages.INITIAL.options.urda_trial_branch.selectionScreenPrompt",
                "EZMB_URDA.pages.INITIAL.options.urda_shallow_root_relic.title",
                "EZMB_URDA.pages.INITIAL.options.urda_shallow_root_relic.description",
                "EZMB_URDA.pages.INITIAL.options.urda_elite_root.title",
                "EZMB_URDA.pages.INITIAL.options.urda_elite_root.description",
                "EZMB_URDA.pages.INITIAL.options.urda_rooted_route.title",
                "EZMB_URDA.pages.INITIAL.options.urda_rooted_route.description",
                "EZMB_URDA.pages.INITIAL.options.urda_after_rain.title",
                "EZMB_URDA.pages.INITIAL.options.urda_after_rain.description",
                "EZMB_URDA.pages.INITIAL.options.urda_root_sight.title",
                "EZMB_URDA.pages.INITIAL.options.urda_root_sight.description",
                "EZMB_URDA.root_sight.hover.title",
                "EZMB_URDA.root_sight.hover.description",
                "EZMB_URDA.root_sight.selection_hover.title",
                "EZMB_URDA.root_sight.selection_hover.description",
                "EZMB_URDA.root_sight.map_hover.title",
                "EZMB_URDA.root_sight.map_hover.description",
                "EZMB_URDA.root_sight.map_hover.preview_description",
                "EZMB_URDA.root_sight.map_hover.event_preview_description",
                "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.title",
                "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description",
                "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.storeSelectionPrompt",
                "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.settlementSelectionPrompt"
            ],
            engAncients,
            zhsAncients,
            "Urda ancient localization");
    }
}
