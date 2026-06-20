namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionRewardService
{
    private static bool TryAddA20BossOneCardReward(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        var runState = player.RunState;
        if (!AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(runState) ||
            room?.RoomType != RoomType.Boss ||
            runState.CurrentActIndex != runState.Acts.Count - 1 ||
            runState.Map.SecondBossMapPoint == null ||
            runState.CurrentMapCoord != runState.Map.BossMapPoint.coord ||
            rewards.OfType<CardReward>().Any())
        {
            return false;
        }

        rewards.Add(new CardReward(CardCreationOptions.ForRoom(player, RoomType.Boss), 3, player));
        MainFile.Logger.Info(
            "[Spire Plus] Ascension A20 applied: Boss 1 terminal rewards added one Boss card reward before the second Boss.");
        return true;
    }

    private static bool TryAddDeepBranchTreasureReward(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (!AscensionFeatureGate.IsDeepBranchesEnabled(player.RunState) ||
            room?.RoomType != RoomType.Treasure)
        {
            return false;
        }

        var metadata = AscensionMapService.TryGetCurrentMetadata(player.RunState);
        if (metadata?.DeepBranch != DeepBranchNodeKind.EnhancedReward ||
            rewards.OfType<RelicReward>().Any(reward => reward.Rarity == RelicRarity.Uncommon))
        {
            return false;
        }

        rewards.Add(new RelicReward(RelicRarity.Uncommon, player));
        MainFile.Logger.Info("[Spire Plus] Ascension A17 applied: Deep Branch treasure added an Uncommon relic reward.");
        return true;
    }
}
