namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private const int MirrorHallEchoExtraPlayCount = 1;

    private static async Task TryResolveMirrorHallEchoPowerFallback(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        LothaCombatState combatState)
    {
        if (combatState.MirrorHallEchoConsumedThisTurn ||
            cardPlay.IsAutoPlay ||
            !cardPlay.IsFirstInSeries ||
            !CanUseMirrorHallEchoPowerReplacement(cardPlay.Card, combatState))
        {
            return;
        }

        combatState.MirrorHallEchoConsumedThisTurn = true;
        combatState.MirrorHallEchoArmedType = null;
        combatState.PowerReplacementCardPendingBenefit = null;
        await ApplyPowerReplacementBenefit(choiceContext, cardPlay.Card.Owner);
        MainFile.Logger.Info("[Spire Plus] Lotha Mirror Hall Echo used the Power-card replacement benefit: cost 0 and draw 1.");
    }

    private static void RecordMirrorHallEchoType(Player player, LothaCombatState combatState)
    {
        var lastPlayedType = CombatManager.Instance.History.CardPlaysFinished
            .Where(entry =>
                entry.Actor == player.Creature &&
                entry.CardPlay.IsFirstInSeries &&
                !entry.CardPlay.IsAutoPlay &&
                !entry.CardPlay.Card.IsClone &&
                IsMirrorHallEchoRecordableType(entry.CardPlay.Card.Type) &&
                entry.HappenedThisTurn(player.Creature.CombatState))
            .Select(entry => (CardType?)entry.CardPlay.Card.Type)
            .LastOrDefault();

        combatState.MirrorHallEchoRecordedType = lastPlayedType;
        if (lastPlayedType.HasValue)
        {
            MainFile.Logger.Info($"[Spire Plus] Lotha Mirror Hall Echo recorded {lastPlayedType.Value} for next turn.");
        }
    }

    private static bool CanUseMirrorHallEchoPowerReplacement(CardModel card, LothaCombatState combatState) =>
        !combatState.MirrorHallEchoConsumedThisTurn &&
        combatState.MirrorHallEchoArmedType == CardType.Power &&
        IsPowerCard(card);

    private static bool IsMirrorHallEchoRecordableType(CardType type) =>
        type is CardType.Attack or CardType.Skill or CardType.Power;
}
