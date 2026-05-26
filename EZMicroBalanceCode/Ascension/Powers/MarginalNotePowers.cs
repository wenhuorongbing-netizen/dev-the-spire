using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class DeepThoughtPower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.MarginalNote;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "深思",
        "下一次知识诅咒会附带额外代价。每层：崩解触发时额外造成[blue]1[/blue]点伤害；心智腐烂洗入[blue]1[/blue]张[gold]晕眩[/gold]；懒惰使本回合下一张牌费用+[blue]1[/blue]；枯竭使本回合失去[blue]1[/blue]点能量。[gold]烙印形态[/gold]下，懒惰和枯竭的附加代价每次知识诅咒最多结算一次。",
        "下一次知识诅咒更重。",
        "Deep Thought",
        "The next Knowledge curse adds a side cost. Per stack: Disintegration deals +[blue]1[/blue] when it triggers; Mind Rot shuffles [blue]1[/blue] [gold]Dazed[/gold] into discard. Sloth makes your next card this turn cost +[blue]1[/blue]; Waste Away makes you lose [blue]1[/blue] [gold]Energy[/gold] this turn. In [gold]Branded Form[/gold], Sloth and Waste Away side costs resolve at most once per Knowledge curse.",
        "The next Knowledge curse is worse.");

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (!target.IsPlayer || amount <= 0m)
        {
            return false;
        }

        modifiedAmount = canonicalPower switch
        {
            DisintegrationPower => amount + Amount,
            _ => amount
        };

        return modifiedAmount != amount;
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (amount <= 0m ||
            power.Owner?.Player is not { } player ||
            applier?.Monster is not KnowledgeDemon)
        {
            return;
        }

        if (power is MindRotPower)
        {
            var combatState = player.Creature.CombatState;
            if (combatState == null)
            {
                return;
            }

            for (var i = 0; i < Amount; i++)
            {
                var dazed = combatState.CreateCard<Dazed>(player);
                await CardPileCmd.AddGeneratedCardToCombat(dazed, PileType.Discard, player, CardPilePosition.Bottom);
            }
        }
        else if (power is SlothPower)
        {
            var sideCostLayers = GetSideCostLayers(player);
            if (sideCostLayers > 0m)
            {
                await PowerCmd.Apply<DeepThoughtCostTaxPower>(choiceContext, player.Creature, sideCostLayers, applier, cardSource);
            }
        }
        else if (power is WasteAwayPower)
        {
            var sideCostLayers = GetSideCostLayers(player);
            if (sideCostLayers > 0m)
            {
                await PlayerCmd.LoseEnergy(sideCostLayers, player);
            }
        }
    }

    private decimal GetSideCostLayers(Player player)
    {
        var layers = Math.Max(0m, Amount);
        var metadata = AscensionMapService.TryGetCurrentMetadata(player.RunState);
        return metadata is { IsBossBrand: true, BossSeal.Id: BossSealId.MarginalNote }
            ? Math.Min(layers, 1m)
            : layers;
    }
}

internal sealed class DeepThoughtCostTaxPower : BossSealPower
{
    protected override BossSealId? SealId => BossSealId.MarginalNote;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => Loc(
        "深思代价",
        "本回合下一张牌费用+[blue]{Amount}[/blue]，之后移除此效果。",
        "下一张牌费用提高。",
        "Deep Thought Cost",
        "The next card you play this turn costs +[blue]{Amount}[/blue], then this is removed.",
        "The next card costs more.");

    public override PowerType Type => PowerType.Debuff;

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card.Owner?.Creature != Owner || originalCost < 0m)
        {
            modifiedCost = originalCost;
            return false;
        }

        modifiedCost = originalCost + Amount;
        return true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == Owner)
        {
            await PowerCmd.Remove(this);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}
