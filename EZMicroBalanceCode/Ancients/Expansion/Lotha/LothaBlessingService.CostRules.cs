using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    public static bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return false;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        HydrateDeathReprieveState(player, combatState);
        if (IsDeathReprieveCostFree(player, combatState))
        {
            modifiedCost = 0;
            return true;
        }

        if (IsPowerReplacementCostZeroCard(card, player, combatState))
        {
            combatState.PowerReplacementCardPendingBenefit = card;
            modifiedCost = 0;
            return true;
        }

        return false;
    }

    public static bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return false;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        HydrateDeathReprieveState(player, combatState);
        if (IsDeathReprieveCostFree(player, combatState))
        {
            modifiedCost = 0;
            return true;
        }

        if (IsPowerReplacementCostZeroCard(card, player, combatState))
        {
            combatState.PowerReplacementCardPendingBenefit = card;
            modifiedCost = 0;
            return true;
        }

        return false;
    }
}
