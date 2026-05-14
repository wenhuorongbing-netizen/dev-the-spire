namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static class AncientCardHelpers
{
    public const int FiddleHandLimit = 7;

    public static void EnsureKeywordsInitialized(CardModel card)
    {
        _ = card.Keywords.Count;
    }

    public static void ApplyKeywords(CardModel card, params CardKeyword[] keywords)
    {
        EnsureKeywordsInitialized(card);
        CardCmd.ApplyKeyword(card, keywords);
    }

    public static void RemoveKeywords(CardModel card, params CardKeyword[] keywords)
    {
        EnsureKeywordsInitialized(card);
        CardCmd.RemoveKeyword(card, keywords);
    }

    public static void ApplyTemporaryCostReduction(CardModel card, int amount)
    {
        if (!card.EnergyCost.CostsX)
        {
            card.EnergyCost.AddThisTurnOrUntilPlayed(-amount, reduceOnly: true);
        }

        if (!card.HasStarCostX && card.CurrentStarCost > 0)
        {
            card.SetStarCostThisTurn(card.CurrentStarCost - amount);
        }
    }

    public static int EffectiveCost(CardModel card)
    {
        var energyCost = card.EnergyCost.CostsX
            ? card.Owner.PlayerCombatState?.Energy ?? 0
            : card.EnergyCost.GetWithModifiers(CostModifiers.All);
        var starCost = card.HasStarCostX
            ? card.Owner.PlayerCombatState?.Stars ?? 0
            : Math.Max(0, card.GetStarCostWithModifiers());
        return energyCost + starCost;
    }

    public static Creature? GetPreferredTarget(CardModel card, ICombatState combatState, Player owner)
    {
        return card.TargetType switch
        {
            TargetType.AnyEnemy => combatState.HittableEnemies.OrderByDescending(creature => creature.CurrentHp).FirstOrDefault(),
            TargetType.AnyAlly => combatState.Allies.FirstOrDefault(creature => creature.IsAlive && creature.IsPlayer && creature != owner.Creature),
            TargetType.AnyPlayer => owner.Creature,
            _ => null
        };
    }

    public static bool IsJeweledMaskPower(CardModel card)
    {
        return card.Enchantment is JeweledMaskFreePower;
    }

    public static void RemoveUnpiledRunCard(CardModel card)
    {
        if (card.Pile != null)
        {
            return;
        }

        var owner = card.Owner;
        if (owner?.RunState.ContainsCard(card) == true)
        {
            owner.RunState.RemoveCard(card);
        }
    }

    public static void RemoveUnpiledCombatCard(CardModel card, ICombatState? combatState = null)
    {
        if (card.Pile != null)
        {
            return;
        }

        var state = combatState ?? card.Owner?.Creature.CombatState;
        if (state?.ContainsCard(card) == true)
        {
            state.RemoveCard(card);
        }
    }

    public static async Task<CardPileAddResult?> TryAddGeneratedCardToCombat(
        CardModel card,
        PileType pileType,
        Player creator)
    {
        return await TryAddGeneratedCardToCombat(card, pileType, creator, CardPilePosition.Bottom);
    }

    public static async Task<CardPileAddResult?> TryAddGeneratedCardToCombat(
        CardModel card,
        PileType pileType,
        Player creator,
        CardPilePosition position)
    {
        if (CombatManager.Instance.IsOverOrEnding ||
            !CombatManager.Instance.IsInProgress ||
            card.Owner?.Creature.CombatState == null)
        {
            RemoveUnpiledCombatCard(card);
            return null;
        }

        var results = await CardPileCmd.AddGeneratedCardsToCombat([card], pileType, creator, position);
        var result = results.FirstOrDefault();
        if (result.cardAdded == null || !result.success)
        {
            RemoveUnpiledCombatCard(card);
            return null;
        }

        return result;
    }
}

