using System.Collections.Generic;
using System.Linq;

using EZMicroBalance.EZMicroBalanceCode.Ancients;
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
            EliteRootSelectionOption,
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
            return TakeFallbackOptions(options, includeReroll: true);
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

        MainFile.Logger.Warn($"[Spire Plus] Urda forced blessing '{forcedBlessing}' did not match any option; showing fallback options.");
        return TakeFallbackOptions(options, includeReroll: true);
    }

    private IReadOnlyList<EventOption> TakeFallbackOptions(
        List<EventOption> options,
        bool includeReroll,
        IReadOnlySet<string>? excludedTextKeys = null)
    {
        if (options.Count == 0)
        {
            MainFile.Logger.Warn("[Spire Plus] Urda has no source-backed Ancient options to show; the event will finish instead of presenting a blank Ancient screen.");
            return [];
        }

        if (options.Count < ExpectedInitialOptionCount)
        {
            MainFile.Logger.Warn($"[Spire Plus] Urda only has {options.Count} source-backed option(s), expected {ExpectedInitialOptionCount}; showing all available options.");
        }

        var candidates = excludedTextKeys is { Count: > 0 }
            ? options.Where(option => !excludedTextKeys.Contains(option.TextKey)).ToList()
            : options;
        if (candidates.Count < ExpectedInitialOptionCount)
        {
            candidates = options;
        }

        var selected = candidates.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList();
        if (includeReroll && AncientInitialOptionReroll.CanOffer(this, options.Count, ExpectedInitialOptionCount))
        {
            selected.Add(AncientInitialOptionReroll.CreateOption(
                this,
                InitialOptionKey(AncientInitialOptionReroll.OptionId),
                RerollInitialOptions));
        }

        return selected;
    }

    private Task RerollInitialOptions()
    {
        if (!AncientInitialOptionReroll.TrySpend(this))
        {
            return Task.CompletedTask;
        }

        var previousChoices = CurrentOptions
            .Where(option => option.TextKey != InitialOptionKey(AncientInitialOptionReroll.OptionId))
            .Select(option => option.TextKey)
            .ToHashSet(StringComparer.Ordinal);
        var options = AllPossibleOptions.ToList();
        var rerolled = TakeFallbackOptions(options, includeReroll: false, previousChoices);
        AncientInitialOptionReroll.ReplaceGeneratedOptionsForHistory(this, rerolled);
        SetEventState(InitialDescription, rerolled);
        MainFile.Logger.Info("[Spire Plus] Urda initial Ancient rewards rerolled once.");
        return Task.CompletedTask;
    }
}
