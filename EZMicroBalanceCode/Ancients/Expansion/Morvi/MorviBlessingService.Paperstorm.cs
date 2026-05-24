using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    private const int PaperstormWastePaperCount = 4;
    private const int PaperstormStatusTriggersPerTurn = 2;

    private static async Task StartPaperstormCombat(Player player, MorviCombatState combatState)
    {
        combatState.PaperstormTriggersRemainingThisTurn = PaperstormStatusTriggersPerTurn;
        await AddWastePapers(player);
        await SetCounterPower<MorviPaperstormPower>(
            new ThrowingPlayerChoiceContext(),
            player,
            PaperstormStatusTriggersPerTurn);
    }

    private static async Task ResetPaperstormTurnCounter(
        PlayerChoiceContext choiceContext,
        Player player,
        MorviCombatState combatState)
    {
        combatState.PaperstormTriggersRemainingThisTurn = PaperstormStatusTriggersPerTurn;
        await SetCounterPower<MorviPaperstormPower>(
            choiceContext,
            player,
            PaperstormStatusTriggersPerTurn);
    }

    public static async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card)
    {
        var player = card.Owner;
        if (player == null ||
            !player.IsActiveForHooks ||
            GetSelectedBlessing(player) != MorviBlessingIds.Paperstorm ||
            card.Type != CardType.Status)
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        if (combatState.PaperstormTriggersRemainingThisTurn <= 0 &&
            player.Creature.GetPower<MorviPaperstormPower>() is { Amount: > 0 } paperstormPower)
        {
            combatState.PaperstormTriggersRemainingThisTurn = Math.Min(
                PaperstormStatusTriggersPerTurn,
                paperstormPower.Amount);
        }

        if (combatState.PaperstormTriggersRemainingThisTurn <= 0 ||
            card.Pile?.Type != PileType.Hand)
        {
            return;
        }

        combatState.PaperstormTriggersRemainingThisTurn--;
        await SetCounterPower<MorviPaperstormPower>(
            choiceContext,
            player,
            combatState.PaperstormTriggersRemainingThisTurn);
        await CardCmd.Exhaust(choiceContext, card, skipVisuals: true);
        await CardPileCmd.Draw(choiceContext, 1m, player);
        await PlayerCmd.GainEnergy(1m, player);
        MainFile.Logger.Info($"[Spire Plus] Morvi Paperstorm converted drawn Status {card.Id.Entry}; remaining this turn={combatState.PaperstormTriggersRemainingThisTurn}.");
    }

    private static async Task AddWastePapers(Player player)
    {
        if (player.Creature.CombatState == null)
        {
            return;
        }

        for (var index = 0; index < PaperstormWastePaperCount; index++)
        {
            var waste = player.Creature.CombatState.CreateCard<MorviWastePaper>(player);
            await AncientCardHelpers.TryAddGeneratedCardToCombat(waste, PileType.Draw, player, CardPilePosition.Random);
        }

        MainFile.Logger.Info("[Spire Plus] Morvi Paperstorm shuffled 4 Waste Paper Status cards into the draw pile.");
    }
}
