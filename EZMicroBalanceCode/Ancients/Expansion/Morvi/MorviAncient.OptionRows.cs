using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed partial class EzmbMorvi
{
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
            if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopGameplay(
                    Owner.RunState,
                    "MorviAncientSelection",
                    "Morvi Ancient rewards can mutate deck, reward, and combat state and are disabled in co-op until host-authoritative sync is proven."))
            {
                AncientSelectionEvidenceLog.LogBlessingSelectionFailed(
                    Owner,
                    "Morvi",
                    blessingId,
                    "coop_gameplay_disabled",
                    !string.IsNullOrWhiteSpace(MorviFeatureGate.ForcedBlessing));
                Done();
                return;
            }

            if (!await MorviBlessingService.TrySetSelectedBlessing(Owner, blessingId))
            {
                AncientSelectionEvidenceLog.LogBlessingSelectionFailed(
                    Owner,
                    "Morvi",
                    blessingId,
                    "selection_rejected",
                    !string.IsNullOrWhiteSpace(MorviFeatureGate.ForcedBlessing));
                SetEventState(InitialDescription, GenerateInitialOptions());
                return;
            }

            await AncientRewardRelicService.ObtainSelectionRelicIfMissing<T>(Owner, blessingId);
            AncientSelectionEvidenceLog.LogBlessingSelected(
                Owner,
                "Morvi",
                blessingId,
                typeof(T).Name,
                !string.IsNullOrWhiteSpace(MorviFeatureGate.ForcedBlessing));
        }
        Done();
    }
}
