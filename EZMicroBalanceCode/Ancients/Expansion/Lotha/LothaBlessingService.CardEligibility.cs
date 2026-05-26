using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private static bool TryConsumeAutoPlayModifierBlock(CardModel card, LothaCombatState combatState)
    {
        if (!ReferenceEquals(combatState.AutoPlayCardPendingModifier, card))
        {
            return false;
        }

        combatState.AutoPlayCardPendingModifier = null;
        return true;
    }

    private static bool IsEligibleCard(CardModel card) =>
        card.Type is CardType.Attack or CardType.Skill && !card.IsClone;

    private static bool IsDeferredVerdictConsumerCard(CardModel card) =>
        card.Type != CardType.Status && !card.IsClone;

    private static bool IsDeferredVerdictExtraPlayCard(CardModel card) =>
        IsEligibleCard(card);

    private static void LogExtraPlayAttempt(
        Player player,
        string blessing,
        CardModel card,
        bool allowed,
        string reason,
        int extraPlayCount) =>
        ReleaseEvidenceLog.Log(
            "AncientExtraPlay",
            "lotha_extra_play_attempt",
            player,
            new Dictionary<string, object?>
            {
                ["ancient"] = "Lotha",
                ["blessing"] = blessing,
                ["card"] = card.Id.Entry,
                ["cardType"] = card.Type,
                ["isClone"] = card.IsClone,
                ["allowed"] = allowed,
                ["reason"] = reason,
                ["extraPlayCount"] = extraPlayCount
            });
}
