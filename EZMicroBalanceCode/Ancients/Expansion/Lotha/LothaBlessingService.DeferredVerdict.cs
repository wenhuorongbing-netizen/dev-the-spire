namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private const int DeferredVerdictTurn = 4;
    private const int DeferredVerdictStacks = 3;
    private const int DeferredVerdictEnergy = 4;
    private const int DeferredVerdictCards = 4;
    private const int DeferredVerdictExtraPlayCount = 1;
    private const int DeferredVerdictEarlyEndHeal = 4;

    private static async Task TryResolveDeferredVerdictCard(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        LothaCombatState combatState)
    {
        if (!combatState.DeferredVerdictActiveThisTurn ||
            !cardPlay.IsFirstInSeries ||
            cardPlay.IsAutoPlay ||
            !IsDeferredVerdictConsumerCard(cardPlay.Card))
        {
            return;
        }

        var player = cardPlay.Card.Owner;
        var usesPowerReplacement = CanUseDeferredVerdictPowerReplacement(cardPlay.Card, player, combatState);
        var verdict = player.Creature.GetPower<LothaVerdictPower>();
        if (verdict is not { Amount: > 0 })
        {
            return;
        }

        await PowerCmd.Decrement(verdict);
        MainFile.Logger.Info("[EZMicroBalance] Lotha Deferred Verdict consumed 1 player-owned Verdict.");

        if (usesPowerReplacement)
        {
            combatState.PowerReplacementCardPendingBenefit = null;
            await ApplyPowerReplacementBenefit(choiceContext, player);
            MainFile.Logger.Info("[EZMicroBalance] Lotha Deferred Verdict used the Power-card replacement benefit: cost 0 and draw 1.");
        }
    }

    private static bool CanUseDeferredVerdictPowerReplacement(
        CardModel card,
        Player player,
        LothaCombatState combatState) =>
        combatState.DeferredVerdictActiveThisTurn &&
        HasDeferredVerdictStacks(player) &&
        IsPowerCard(card);

    private static bool HasDeferredVerdictStacks(Player player) =>
        player.Creature.GetPower<LothaVerdictPower>() is { Amount: > 0 };
}
