namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    private static async Task DamagePlayerNonlethal(Player player, decimal calculatedHpLoss)
    {
        var maximumNonlethalHpLoss = Math.Max(0m, player.Creature.CurrentHp - 1m);
        var hpLoss = Math.Min(calculatedHpLoss, maximumNonlethalHpLoss);
        if (hpLoss <= 0)
        {
            return;
        }

        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            player.Creature,
            hpLoss,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            player.Creature,
            null);
    }
}
