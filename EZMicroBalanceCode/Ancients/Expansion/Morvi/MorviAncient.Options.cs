using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;

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

    private EventOption ForbiddenLoanSelectionOption =>
        OptionWithRelic<MorviForbiddenLoanOptionRelic>(
            MorviBlessingIds.ForbiddenLoan);

    private EventOption MisprintPressSelectionOption =>
        OptionWithRelic<MorviMisprintPressOptionRelic>(
            MorviBlessingIds.MisprintPress,
            [
                HoverTipFactory.Static(StaticHoverTip.ReplayStatic),
                HoverTipFactory.Static(StaticHoverTip.Energy)
            ]);

    private EventOption RedInkOverdraftSelectionOption =>
        OptionWithRelic<MorviRedInkOverdraftOptionRelic>(
            MorviBlessingIds.RedInkOverdraft,
            [
                .. HoverTipFactory.FromCardWithCardHoverTips<MorviRedInkOverdraftCard>(),
                HoverTipFactory.FromPower<MorviOverdraftPower>(),
                HoverTipFactory.Static(StaticHoverTip.Energy)
            ]);

    private EventOption OverdueLibrarySelectionOption =>
        OptionWithRelic<MorviOverdueLibraryOptionRelic>(
            MorviBlessingIds.OverdueLibrary,
            [
                .. HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveDrawPage>(),
                .. HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveVeilPage>(),
                .. HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveBurnPage>(),
                .. HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveDiscountPage>(),
                .. HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveBraveryPage>(),
                .. HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveDexterityPage>(),
                HoverTipFactory.Static(StaticHoverTip.Energy),
                HoverTipFactory.Static(StaticHoverTip.Block)
            ]);

    private EventOption OpenBookExamSelectionOption =>
        OptionWithRelic<MorviOpenBookExamOptionRelic>(
            MorviBlessingIds.OpenBookExam,
            [
                HoverTipFactory.FromPower<MorviOpenBookPower>(),
                HoverTipFactory.Static(StaticHoverTip.Energy)
            ]);

    private EventOption PaperstormSelectionOption =>
        OptionWithRelic<MorviPaperstormOptionRelic>(
            MorviBlessingIds.Paperstorm,
            [
                .. HoverTipFactory.FromCardWithCardHoverTips<MorviWastePaper>(),
                HoverTipFactory.FromPower<MorviPaperstormPower>(),
                HoverTipFactory.Static(StaticHoverTip.Energy)
            ]);

    private EventOption BlueprintProofSelectionOption =>
        OptionWithRelic<MorviBlueprintProofOptionRelic>(
            MorviBlessingIds.BlueprintProof,
            [
                HoverTipFactory.FromPower<MorviProofreadPower>(),
                HoverTipFactory.Static(StaticHoverTip.Block)
            ]);

    private EventOption DebtSettlementSelectionOption =>
        OptionWithRelic<MorviDebtSettlementOptionRelic>(
            MorviBlessingIds.DebtSettlement,
            [HoverTipFactory.FromPower<MorviDebtPower>()]);

    private EventOption OptionWithRelic<T>(string blessingId, IEnumerable<IHoverTip>? hoverTips = null) where T : RelicModel
    {
        var relic = ModelDb.Relic<T>().ToMutable();
        if (Owner != null)
        {
            relic.Owner = Owner;
        }

        var option = EventOption.FromRelic(relic, this, () => SelectBlessing<T>(blessingId), InitialOptionKey(blessingId));
        option.HoverTips = option.HoverTips.Concat(hoverTips ?? []).ToList();
        return option;
    }

    private async Task SelectBlessing<T>(string blessingId)
        where T : RelicModel
    {
        if (Owner != null)
        {
            if (!await MorviBlessingService.TrySetSelectedBlessing(Owner, blessingId))
            {
                MainFile.Logger.Warn($"[EZMicroBalance] Morvi blessing selection failed before completion: {blessingId}.");
                SetEventState(InitialDescription, GenerateInitialOptions());
                return;
            }

            await AncientRewardRelicService.ObtainSelectionRelicIfMissing<T>(Owner, blessingId);
        }

        MainFile.Logger.Info($"[EZMicroBalance] Morvi blessing selected: {blessingId}.");
        Done();
    }
}
