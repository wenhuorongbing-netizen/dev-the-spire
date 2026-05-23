using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal sealed partial class EzmbLotha
{
    private EventOption MirrorRebuttalSelectionOption =>
        OptionWithRelic<LothaMirrorRebuttalOptionRelic>(
            LothaBlessingIds.MirrorRebuttal,
            [
                HoverTipFactory.Static(StaticHoverTip.ReplayStatic),
                HoverTipFactory.Static(StaticHoverTip.Energy)
            ]);

    private EventOption MirrorHallEchoSelectionOption =>
        OptionWithRelic<LothaMirrorHallEchoOptionRelic>(
            LothaBlessingIds.MirrorHallEcho,
            [
                HoverTipFactory.Static(StaticHoverTip.ReplayStatic),
                HoverTipFactory.Static(StaticHoverTip.Energy)
            ]);

    private EventOption PresumptionSelectionOption =>
        OptionWithRelic<LothaPresumptionOptionRelic>(
            LothaBlessingIds.Presumption,
            [
                HoverTipFactory.FromPower<LothaPresumptionPower>(),
                HoverTipFactory.Static(StaticHoverTip.Energy),
                HoverTipFactory.Static(StaticHoverTip.Block)
            ]);

    private EventOption ClosedCourtSelectionOption =>
        OptionWithRelic<LothaClosedCourtOptionRelic>(
            LothaBlessingIds.ClosedCourt,
            [HoverTipFactory.Static(StaticHoverTip.Energy)]);

    private EventOption DeferredVerdictSelectionOption =>
        OptionWithRelic<LothaDeferredVerdictOptionRelic>(
            LothaBlessingIds.DeferredVerdict,
            [
                HoverTipFactory.FromPower<LothaVerdictPower>(),
                HoverTipFactory.Static(StaticHoverTip.ReplayStatic),
                HoverTipFactory.Static(StaticHoverTip.Energy)
            ]);

    private EventOption DeathReprieveSelectionOption =>
        OptionWithRelic<LothaDeathReprieveOptionRelic>(
            LothaBlessingIds.DeathReprieve,
            [HoverTipFactory.FromPower<LothaDeathReprievePower>()]);

    private EventOption SingleSentenceSelectionOption =>
        OptionWithRelic<LothaSingleSentenceOptionRelic>(
            LothaBlessingIds.SingleSentence,
            [
                HoverTipFactory.Static(StaticHoverTip.ReplayStatic),
                HoverTipFactory.Static(StaticHoverTip.Energy)
            ]);

    private EventOption PublicEvidenceSelectionOption =>
        OptionWithRelic<LothaPublicEvidenceOptionRelic>(
            LothaBlessingIds.PublicEvidence,
            [
                HoverTipFactory.FromPower<LothaEnlightenmentPower>(),
                HoverTipFactory.Static(StaticHoverTip.Block)
            ]);

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
            await LothaRewardSelectionService.SelectBlessing<T>(Owner, blessingId);
            AncientSelectionEvidenceLog.LogBlessingSelected(
                Owner,
                "Lotha",
                blessingId,
                typeof(T).Name,
                !string.IsNullOrWhiteSpace(LothaFeatureGate.ForcedBlessing));
        }
        Done();
    }
}
