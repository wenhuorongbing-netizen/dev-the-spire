using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    private const int OpenBookDraw = 5;
    private const int OpenBookEnergy = 2;
    private const int OpenBookSealTurn = 1;
    private const int OpenBookReturnTurn = 3;

    private static async Task ResolveOpenBookTurnStart(
        PlayerChoiceContext choiceContext,
        Player player,
        MorviCombatState combatState)
    {
        if (player.Creature.CombatState?.RoundNumber == OpenBookSealTurn &&
            !combatState.OpenBookResolved)
        {
            combatState.OpenBookResolved = true;
            var drawn = (await CardPileCmd.Draw(choiceContext, OpenBookDraw, player)).ToList();
            foreach (var card in drawn)
            {
                combatState.OpenBookDrawnCards.Add(card);
            }

            await PlayerCmd.GainEnergy(OpenBookEnergy, player);
            await SetCounterPower<MorviOpenBookPower>(choiceContext, player, drawn.Count);
            MainFile.Logger.Info($"[Spire Plus] Morvi Open-Book Exam drew {drawn.Count} cards and granted {OpenBookEnergy} Energy on turn 1.");
        }

        if (player.Creature.CombatState?.RoundNumber == OpenBookReturnTurn &&
            FindOpenBookSealedCards(player, combatState).Count > 0)
        {
            await ReturnOpenBookCards(player, combatState);
        }
    }

    private static async Task TrySealOpenBookAtTurnEnd(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (GetSelectedBlessing(player) != MorviBlessingIds.OpenBookExam ||
            player.Creature.CombatState?.RoundNumber != OpenBookSealTurn)
        {
            return;
        }

        await SealOpenBookCards(choiceContext, player, CombatStates.GetOrCreateValue(player));
    }

    private static async Task SealOpenBookCards(
        PlayerChoiceContext choiceContext,
        Player player,
        MorviCombatState combatState)
    {
        var toSeal = PileType.Hand.GetPile(player)
            .Cards
            .Where(combatState.OpenBookDrawnCards.Contains)
            .ToList();

        combatState.OpenBookSealedCards.Clear();
        foreach (var card in toSeal)
        {
            var addResult = await CardPileCmd.Add(card, PileType.Exhaust);
            if (!addResult.success)
            {
                continue;
            }

            AncientSavedStateFields.MorviOpenBookSealedCard[addResult.cardAdded] = true;
            combatState.OpenBookSealedCards.Add(addResult.cardAdded);
        }

        combatState.OpenBookDrawnCards.Clear();
        await SetCounterPower<MorviOpenBookPower>(choiceContext, player, combatState.OpenBookSealedCards.Count);
        MainFile.Logger.Info($"[Spire Plus] Morvi Open-Book Exam sealed {combatState.OpenBookSealedCards.Count} cards into exhaust-pile holding until turn 3.");
    }

    private static async Task ReturnOpenBookCards(Player player, MorviCombatState combatState)
    {
        var hand = PileType.Hand.GetPile(player);
        var returned = 0;
        foreach (var card in FindOpenBookSealedCards(player, combatState))
        {
            AncientSavedStateFields.MorviOpenBookSealedCard[card] = false;
            if (hand.Cards.Count >= CardPile.MaxCardsInHand ||
                card.Pile?.Type.IsCombatPile() != true ||
                card.HasBeenRemovedFromState)
            {
                continue;
            }

            var addResult = await CardPileCmd.Add(card, PileType.Hand);
            if (!addResult.success)
            {
                continue;
            }

            addResult.cardAdded.SetToFreeThisTurn();
            returned++;
        }

        combatState.OpenBookSealedCards.Clear();
        await SetCounterPower<MorviOpenBookPower>(new ThrowingPlayerChoiceContext(), player, 0);
        MainFile.Logger.Info($"[Spire Plus] Morvi Open-Book Exam returned {returned} sealed cards on turn 3 and made them cost 0 this turn.");
    }

}
