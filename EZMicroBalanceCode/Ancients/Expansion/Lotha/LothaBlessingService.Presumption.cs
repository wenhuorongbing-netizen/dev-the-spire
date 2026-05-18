using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private const int PresumptionCards = 2;
    private const int PresumptionEnergy = 1;
    private const int PresumptionBlock = 8;
    private const int PresumptionHpLoss = 8;

    private static async Task TryApplyPresumptionCombatStart(Player player)
    {
        if (GetSelectedBlessing(player) != LothaBlessingIds.Presumption)
        {
            return;
        }

        await PowerCmd.Apply<LothaPresumptionPower>(
            new ThrowingPlayerChoiceContext(),
            player.Creature,
            1,
            player.Creature,
            null);
        MainFile.Logger.Info("[EZMicroBalance] Lotha Presumption of Innocence applied Innocent at combat start.");
    }

    private static async Task TryApplyPresumptionTurnStart(
        PlayerChoiceContext choiceContext,
        Player player,
        LothaCombatState combatState,
        string selectedBlessing)
    {
        if (selectedBlessing != LothaBlessingIds.Presumption || combatState.PresumptionLost)
        {
            return;
        }

        if (player.Creature.GetPower<LothaPresumptionPower>() == null)
        {
            await PowerCmd.Apply<LothaPresumptionPower>(
                choiceContext,
                player.Creature,
                1,
                player.Creature,
                null);
        }

        await CardPileCmd.Draw(choiceContext, PresumptionCards, player);
        await PlayerCmd.GainEnergy(PresumptionEnergy, player);
        await CreatureCmd.GainBlock(player.Creature, PresumptionBlock, ValueProp.Move, null, fast: true);
        MainFile.Logger.Info("[EZMicroBalance] Lotha Presumption of Innocence granted draw 2, Energy 1, and Block 8 while Innocent.");
    }

    public static async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (!target.IsPlayer ||
            target.Player is not { } player ||
            !player.IsActiveForHooks ||
            GetSelectedBlessing(player) != LothaBlessingIds.Presumption)
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        if (combatState.PresumptionLost ||
            !IsUnblockedEnemyAttackDamage(result, props, dealer, cardSource))
        {
            return;
        }

        combatState.PresumptionLost = true;
        await PowerCmd.Remove<LothaPresumptionPower>(player.Creature);
        await CreatureCmd.Damage(
            choiceContext,
            player.Creature,
            PresumptionHpLoss,
            ValueProp.Unblockable | ValueProp.Unpowered,
            null,
            null);
        MainFile.Logger.Info("[EZMicroBalance] Lotha Presumption of Innocence broke after unblocked enemy attack damage and applied 8 HP loss.");
    }

    private static bool IsUnblockedEnemyAttackDamage(
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource) =>
        result.UnblockedDamage > 0 &&
        !result.WasFullyBlocked &&
        dealer is { IsEnemy: true } &&
        cardSource == null &&
        props.HasFlag(ValueProp.Move);
}
