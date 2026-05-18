using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Rewards;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private sealed class CardRewardContext
    {
        public bool IsNormalActOneCombatCardReward { get; set; }

        public bool HumusPactHandled { get; set; }

        public bool SeedBankHandled { get; set; }
    }

    private static readonly ConditionalWeakTable<CardReward, CardRewardContext> CardRewardContexts = new();

    public static bool MarkCardRewardIfNormalActOneCombat(
        Player player,
        CardCreationOptions creationOptions)
    {
        if (PrismaticGemRewardScreenContextPatch.CurrentReward is not { } currentReward ||
            currentReward.Player != player ||
            !IsNormalActOneCombatReward(player, creationOptions))
        {
            return false;
        }

        CardRewardContexts.GetValue(currentReward, _ => new CardRewardContext()).IsNormalActOneCombatCardReward = true;
        return false;
    }

    public static bool TryModifyCardRewardAlternatives(
        Player player,
        CardReward cardReward,
        List<CardRewardAlternative> alternatives)
    {
        if (!IsTrackedNormalActOneCombatReward(cardReward) || alternatives.Count >= 2)
        {
            return false;
        }

        return GetSelectedBlessing(player) switch
        {
            UrdaBlessingIds.Seedbed => TryAddSeedbedAlternative(player, alternatives),
            UrdaBlessingIds.HumusPact => TryAddHumusPactAlternative(player, cardReward, alternatives),
            UrdaBlessingIds.SeedBank => TryAddSeedBankAlternative(player, cardReward, alternatives),
            _ => false
        };
    }

    private static bool IsTrackedNormalActOneCombatReward(CardReward reward) =>
        CardRewardContexts.TryGetValue(reward, out var context) &&
        context.IsNormalActOneCombatCardReward;

    private static bool IsNormalActOneCombatReward(Player player, CardCreationOptions creationOptions)
    {
        return player.RunState.CurrentActIndex == 0 &&
            player.RunState.CurrentRoom?.RoomType == RoomType.Monster &&
            creationOptions.Source == CardCreationSource.Encounter &&
            creationOptions.RarityOdds == CardRarityOddsType.RegularEncounter &&
            creationOptions.Flags.HasFlag(CardCreationFlags.IsCardReward);
    }
}
