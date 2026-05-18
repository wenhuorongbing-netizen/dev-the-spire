using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private const int ClosedCourtEnergy = 4;
    private const int ClosedCourtDiscountCount = 3;

    private static async Task TryOpenClosedCourtFirstTurn(
        PlayerChoiceContext choiceContext,
        Player player,
        LothaCombatState combatState,
        string selectedBlessing)
    {
        if (selectedBlessing != LothaBlessingIds.ClosedCourt || combatState.ClosedCourtUsed)
        {
            return;
        }

        combatState.ClosedCourtUsed = true;
        combatState.ClosedCourtDiscountActiveThisTurn = true;
        combatState.ClosedCourtDiscountsRemainingThisTurn = ClosedCourtDiscountCount;

        var cardsToDraw = Math.Max(0, CardPile.MaxCardsInHand - PileType.Hand.GetPile(player).Cards.Count);
        if (cardsToDraw > 0)
        {
            await CardPileCmd.Draw(choiceContext, cardsToDraw, player);
        }

        await PlayerCmd.GainEnergy(ClosedCourtEnergy, player);
        MainFile.Logger.Info("[EZMicroBalance] Lotha Closed Court filled the hand, granted Energy 4, and armed three Energy-cost discounts.");
    }

    public static bool TryModifyRewardsLate(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (!player.IsActiveForHooks ||
            GetSelectedBlessing(player) != LothaBlessingIds.ClosedCourt ||
            room is not CombatRoom)
        {
            return false;
        }

        var removed = rewards.RemoveAll(reward => reward is CardReward);
        if (removed <= 0)
        {
            return false;
        }

        MainFile.Logger.Info($"[EZMicroBalance] Lotha Closed Court suppressed {removed} post-combat card reward(s); gold, potion, and relic rewards remain.");
        return true;
    }

    private static bool TryApplyClosedCourtEnergyDiscount(
        CardModel card,
        decimal originalCost,
        LothaCombatState combatState,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        var player = card.Owner;
        if (player == null ||
            GetSelectedBlessing(player) != LothaBlessingIds.ClosedCourt ||
            !combatState.ClosedCourtDiscountActiveThisTurn ||
            combatState.ClosedCourtDiscountsRemainingThisTurn <= 0 ||
            card.Pile?.Type != PileType.Hand)
        {
            return false;
        }

        modifiedCost = Math.Max(0, originalCost - 1);
        if (modifiedCost == originalCost)
        {
            return false;
        }

        combatState.ClosedCourtDiscountedCardsThisTurn.Add(card);
        return true;
    }

    private static void TrackClosedCourtDiscountUse(CardPlay cardPlay, LothaCombatState combatState)
    {
        if (!combatState.ClosedCourtDiscountActiveThisTurn ||
            combatState.ClosedCourtDiscountsRemainingThisTurn <= 0 ||
            !cardPlay.IsFirstInSeries ||
            cardPlay.IsAutoPlay ||
            !combatState.ClosedCourtDiscountedCardsThisTurn.Remove(cardPlay.Card))
        {
            return;
        }

        combatState.ClosedCourtDiscountsRemainingThisTurn--;
        MainFile.Logger.Info($"[EZMicroBalance] Lotha Closed Court consumed a first-turn discount; {combatState.ClosedCourtDiscountsRemainingThisTurn} remain.");
    }
}
