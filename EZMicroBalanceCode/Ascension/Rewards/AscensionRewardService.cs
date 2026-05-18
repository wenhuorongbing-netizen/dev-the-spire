namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionRewardService
{
    public static bool TryModifyCardRewardOptionsLate(
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions)
    {
        var modified = false;

        if (AscensionFeatureGate.IsFiremarkedEliteEnabled(player.RunState))
        {
            modified |= TryAddFiremarkedEliteRewardOption(player, cardRewardOptions, creationOptions);
        }

        if (AscensionFeatureGate.IsBossSealsEnabled(player.RunState))
        {
            modified |= TryAddBossSealRewardOption(player, cardRewardOptions, creationOptions);
        }

        if (AscensionFeatureGate.IsFissionEnabled(player.RunState))
        {
            modified |= TryApplyFission(player, cardRewardOptions, creationOptions);
        }

        return modified;
    }

    public static bool TryModifyRewardsLate(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (TryAddA20BossOneCardReward(player, rewards, room))
        {
            return true;
        }

        return TryAddDeepBranchTreasureReward(player, rewards, room);
    }

}
