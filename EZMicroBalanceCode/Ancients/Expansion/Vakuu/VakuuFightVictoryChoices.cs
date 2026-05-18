using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static partial class VakuuFightService
{
    private sealed record VictoryRelicChoice(RelicModel Relic, Func<Task> OnChosen);

    private static IEnumerable<VictoryRelicChoice> GetNonVakuuAct3AncientRewardChoices(Player owner)
    {
        return GetSourceAct3AncientRelicChoices(owner)
            .Concat(GetLothaAct3AncientRelicChoices(owner));
    }

    private static IEnumerable<VictoryRelicChoice> GetSourceAct3AncientRelicChoices(Player owner)
    {
        var sourceAncients = new AncientEventModel[]
        {
            ModelDb.AncientEvent<Nonupeipe>(),
            ModelDb.AncientEvent<Tanx>()
        };

        return sourceAncients
            .SelectMany(ancient => ancient.AllPossibleOptions)
            .Select(option => option.Relic?.CanonicalInstance)
            .OfType<RelicModel>()
            .Where(relic => IsEligibleSourceAncientReward(owner, relic))
            .Where(relic => owner.GetRelicById(relic.Id) is null)
            .Select(relic =>
            {
                var mutableRelic = relic.ToMutable();
                return new VictoryRelicChoice(
                    mutableRelic,
                    async () => await RelicCmd.Obtain(mutableRelic, owner));
            });
    }

    private static bool IsEligibleSourceAncientReward(Player owner, RelicModel relic)
    {
        if (relic is BeautifulBracelet)
        {
            return owner.Deck.Cards.Count(ModelDb.Enchantment<Swift>().CanEnchant) >= 4;
        }

        if (relic is TriBoomerang)
        {
            return owner.Deck.Cards.Count(card => ModelDb.Enchantment<Instinct>().CanEnchant(card)) >= 3;
        }

        return true;
    }

    private static IEnumerable<VictoryRelicChoice> GetLothaAct3AncientRelicChoices(Player owner)
    {
        if (!LothaFeatureGate.IsLothaEnabled(owner.RunState.UnlockState) ||
            !string.IsNullOrWhiteSpace(LothaBlessingService.GetSelectedBlessing(owner)))
        {
            return [];
        }

        return
        [
            .. LothaChoice<LothaMirrorRebuttalOptionRelic>(owner, LothaBlessingIds.MirrorRebuttal),
            .. LothaChoice<LothaMirrorHallEchoOptionRelic>(owner, LothaBlessingIds.MirrorHallEcho),
            .. LothaChoice<LothaPresumptionOptionRelic>(owner, LothaBlessingIds.Presumption),
            .. LothaChoice<LothaClosedCourtOptionRelic>(owner, LothaBlessingIds.ClosedCourt),
            .. LothaChoice<LothaDeferredVerdictOptionRelic>(owner, LothaBlessingIds.DeferredVerdict),
            .. LothaChoice<LothaDeathReprieveOptionRelic>(owner, LothaBlessingIds.DeathReprieve),
            .. LothaChoice<LothaSingleSentenceOptionRelic>(owner, LothaBlessingIds.SingleSentence),
            .. LothaChoice<LothaPublicEvidenceOptionRelic>(owner, LothaBlessingIds.PublicEvidence)
        ];
    }

    private static IEnumerable<VictoryRelicChoice> LothaChoice<T>(Player owner, string blessingId)
        where T : RelicModel
    {
        if (!IsEligibleLothaVictoryChoice(owner, blessingId))
        {
            return [];
        }

        if (owner.GetRelic<T>() is not null)
        {
            return [];
        }

        var relic = ModelDb.Relic<T>().ToMutable();
        relic.Owner = owner;
        return
        [
            new VictoryRelicChoice(
                relic,
                () => LothaRewardSelectionService.SelectBlessing<T>(owner, blessingId))
        ];
    }

    private static bool IsEligibleLothaVictoryChoice(Player owner, string blessingId) =>
        blessingId != LothaBlessingIds.MirrorRebuttal ||
        LothaBlessingService.HasMirrorRebuttalCandidates(owner);
}
