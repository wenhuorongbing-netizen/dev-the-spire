namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

internal static partial class MorviBlessingService
{
    private const int MisprintExtraPlayCount = 1;
    private const int MisprintDrawCostThreshold = 1;

    public static bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return true;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        if (autoPlayType == AutoPlayType.None)
        {
            if (ReferenceEquals(combatState.AutoPlayCardPendingModifier, card))
            {
                combatState.AutoPlayCardPendingModifier = null;
            }
        }
        else
        {
            combatState.AutoPlayCardPendingModifier = card;
        }

        return true;
    }

    public static int ModifyCardPlayCount(CardModel card, int playCount)
    {
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return playCount;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        if (TryConsumeAutoPlayModifierBlock(card, combatState))
        {
            LogMisprintExtraPlayAttempt(player, card, allowed: false, reason: "autoplay", extraPlayCount: 0);
            return playCount;
        }

        if (GetSelectedBlessing(player) != MorviBlessingIds.MisprintPress ||
            combatState.MisprintUsedThisTurn ||
            !IsNaturalPlayerCombatCard(card) ||
            card.Type is not (CardType.Attack or CardType.Skill))
        {
            return playCount;
        }

        combatState.MisprintUsedThisTurn = true;
        if (!card.EnergyCost.CostsX && card.EnergyCost.Canonical >= MisprintDrawCostThreshold)
        {
            combatState.MisprintDrawAfterCards.Add(card);
        }

        LogMisprintExtraPlayAttempt(player, card, allowed: true, reason: "misprint_press", extraPlayCount: MisprintExtraPlayCount);
        MainFile.Logger.Info($"[EZMicroBalance] Morvi Misprint Press added one play to {card.Id.Entry}.");
        return playCount + MisprintExtraPlayCount;
    }

    private static async Task ResolveMisprintPressAfterPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        MorviCombatState combatState)
    {
        if (!cardPlay.IsLastInSeries || !combatState.MisprintDrawAfterCards.Remove(cardPlay.Card))
        {
            return;
        }

        await CardPileCmd.Draw(choiceContext, 1m, cardPlay.Card.Owner);
        MainFile.Logger.Info($"[EZMicroBalance] Morvi Misprint Press drew 1 card after {cardPlay.Card.Id.Entry}.");
    }

    private static bool TryConsumeAutoPlayModifierBlock(CardModel card, MorviCombatState combatState)
    {
        if (!ReferenceEquals(combatState.AutoPlayCardPendingModifier, card))
        {
            return false;
        }

        combatState.AutoPlayCardPendingModifier = null;
        return true;
    }

    private static void LogMisprintExtraPlayAttempt(
        Player player,
        CardModel card,
        bool allowed,
        string reason,
        int extraPlayCount) =>
        ReleaseEvidenceLog.Log(
            "AncientExtraPlay",
            "morvi_extra_play_attempt",
            player,
            new Dictionary<string, object?>
            {
                ["ancient"] = "Morvi",
                ["blessing"] = MorviBlessingIds.MisprintPress,
                ["card"] = card.Id.Entry,
                ["cardType"] = card.Type,
                ["isClone"] = card.IsClone,
                ["allowed"] = allowed,
                ["reason"] = reason,
                ["extraPlayCount"] = extraPlayCount
            });
}
