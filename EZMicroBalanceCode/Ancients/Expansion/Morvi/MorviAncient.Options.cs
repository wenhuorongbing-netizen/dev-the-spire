using MegaCrit.Sts2.Core.Events;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed partial class EzmbMorvi
{
    private const int ExpectedInitialOptionCount = 3;

    public override IEnumerable<EventOption> AllPossibleOptions =>
        [
            ForbiddenLoanSelectionOption,
            MisprintPressSelectionOption,
            RedInkOverdraftSelectionOption,
            OverdueLibrarySelectionOption,
            OpenBookExamSelectionOption,
            PaperstormSelectionOption,
            BlueprintProofSelectionOption,
            DebtSettlementSelectionOption
        ];

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var options = AllPossibleOptions
            .Where(IsCurrentlyAvailableOption)
            .ToList();
        var forcedBlessing = MorviFeatureGate.ForcedBlessing;
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

        MainFile.Logger.Warn($"[EZMicroBalance] Morvi forced blessing '{forcedBlessing}' did not match any option; showing fallback options.");
        return TakeFallbackOptions(options);
    }

    private IReadOnlyList<EventOption> TakeFallbackOptions(List<EventOption> options)
    {
        if (options.Count == 0)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Morvi has no source-backed Ancient options to show; the event will finish instead of presenting a blank Ancient screen.");
            return [];
        }

        if (options.Count < ExpectedInitialOptionCount)
        {
            MainFile.Logger.Warn($"[EZMicroBalance] Morvi only has {options.Count} source-backed option(s), expected {ExpectedInitialOptionCount}; showing all available options.");
        }

        return options.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList();
    }

    private bool IsCurrentlyAvailableOption(EventOption option)
    {
        if (Owner == null)
        {
            return true;
        }

        return !option.TextKey.EndsWith($".{MorviBlessingIds.ForbiddenLoan}", StringComparison.OrdinalIgnoreCase) ||
            MorviBlessingService.HasForbiddenLoanCandidates(Owner);
    }
}
