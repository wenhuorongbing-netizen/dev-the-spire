using System.Collections.Generic;
using System.Linq;

using MegaCrit.Sts2.Core.Events;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed partial class EzmbUrda
{
    private const int ExpectedInitialOptionCount = 4;

    public override IEnumerable<EventOption> AllPossibleOptions =>
        [
            SeedbedSelectionOption,
            HumusSelectionOption,
            MoltingSelectionOption,
            MossMapSelectionOption,
            TrialBranchSelectionOption,
            ShallowRootRelicSelectionOption,
            RootedRouteSelectionOption,
            AfterRainSelectionOption,
            RootSightSelectionOption,
            SeedBankSelectionOption
        ];

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var options = AllPossibleOptions.ToList();
        var forcedBlessing = UrdaFeatureGate.ForcedBlessing;
        if (string.IsNullOrWhiteSpace(forcedBlessing))
        {
            return TakeFallbackOptions(options);
        }

        var normalized = forcedBlessing.Trim().ToLowerInvariant();
        var forced = options.FirstOrDefault(option =>
        {
            var optionId = option.TextKey[(option.TextKey.LastIndexOf('.') + 1)..];
            return optionId.Equals(normalized, StringComparison.OrdinalIgnoreCase);
        });

        if (forced is not null)
        {
            return [forced];
        }

        MainFile.Logger.Warn($"[EZMicroBalance] Urda forced blessing '{forcedBlessing}' did not match any option; showing fallback options.");
        return TakeFallbackOptions(options);
    }

    private IReadOnlyList<EventOption> TakeFallbackOptions(List<EventOption> options)
    {
        if (options.Count == 0)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Urda has no source-backed Ancient options to show; the event will finish instead of presenting a blank Ancient screen.");
            return [];
        }

        if (options.Count < ExpectedInitialOptionCount)
        {
            MainFile.Logger.Warn($"[EZMicroBalance] Urda only has {options.Count} source-backed option(s), expected {ExpectedInitialOptionCount}; showing all available options.");
        }

        return options.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList();
    }
}
