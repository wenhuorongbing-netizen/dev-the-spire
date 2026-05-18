using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    private static async Task SetCounterPower<T>(PlayerChoiceContext choiceContext, Player player, int amount)
        where T : PowerModel
    {
        var power = player.Creature.GetPower<T>();
        if (amount <= 0)
        {
            await PowerCmd.Remove(power);
            return;
        }

        if (power == null)
        {
            await PowerCmd.Apply<T>(choiceContext, player.Creature, amount, player.Creature, null);
            return;
        }

        var delta = amount - power.Amount;
        if (delta != 0)
        {
            await PowerCmd.Apply<T>(choiceContext, player.Creature, delta, player.Creature, null);
        }
    }

    private static bool IsNaturalPlayerCombatCard(CardModel card) =>
        card.DeckVersion != null &&
        !card.IsClone &&
        card.Type is not CardType.Status and not CardType.Curse;
}
