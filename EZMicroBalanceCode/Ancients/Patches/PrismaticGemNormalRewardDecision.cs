namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static partial class PrismaticGemRewardPatch
{
    private static bool TryReplaceNormalRewardScreen(
        PrismaticGem prismaticGem,
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions)
    {
        if (player != prismaticGem.Owner || cardRewardOptions.Count == 0)
        {
            return false;
        }

        var isNormalCardReward = IsNormalCardReward(creationOptions);
        var rewardScreen = PrismaticGemRewardScreenContextPatch.CurrentReward;
        if (rewardScreen == null)
        {
            if (isNormalCardReward)
            {
                MainFile.Logger.Warn("[Spire Plus] PrismaticGem skipped: normal card reward modification had no CardReward screen context.");
            }

            return false;
        }

        var screenState = RewardStates.GetValue(rewardScreen, _ => new RewardScreenState());
        var madeTriggerDecision = !screenState.HasTriggerDecision;
        if (madeTriggerDecision)
        {
            screenState.HasTriggerDecision = true;
            if (!isNormalCardReward)
            {
                MainFile.Logger.Info("[Spire Plus] PrismaticGem ignored non-normal card reward screen; no counter increment.");
                return false;
            }

            screenState.CounterAtDecision = AncientSavedStateFields.PrismaticGemNormalRewardCounter[prismaticGem] + 1;
            AncientSavedStateFields.PrismaticGemNormalRewardCounter[prismaticGem] = screenState.CounterAtDecision;
            screenState.ShouldReplaceAllSlots = screenState.CounterAtDecision % 2 == 0;
        }
        else if (!isNormalCardReward)
        {
            return false;
        }

        if (!screenState.ShouldReplaceAllSlots)
        {
            if (madeTriggerDecision)
            {
                MainFile.Logger.Info($"[Spire Plus] PrismaticGem applied: counted normal card reward {screenState.CounterAtDecision}; no replacement for this reward screen.");
            }

            return false;
        }

        if (!ReplaceAllRewardSlots(prismaticGem, player, cardRewardOptions, screenState.CounterAtDecision))
        {
            RestoreCounterAfterFailedReplacement(prismaticGem, screenState);
            MainFile.Logger.Warn($"[Spire Plus] PrismaticGem skipped: all reward slots could not be replaced on normal reward {screenState.CounterAtDecision}.");
            return false;
        }

        prismaticGem.Flash();
        MainFile.Logger.Info($"[Spire Plus] PrismaticGem applied: replaced every visible reward slot with off-color cards on normal reward {screenState.CounterAtDecision}.");
        return true;
    }

    private static void RestoreCounterAfterFailedReplacement(PrismaticGem prismaticGem, RewardScreenState screenState)
    {
        AncientSavedStateFields.PrismaticGemNormalRewardCounter[prismaticGem] = Math.Max(0, screenState.CounterAtDecision - 1);
        screenState.ShouldReplaceAllSlots = false;
    }

    private static bool IsNormalCardReward(CardCreationOptions creationOptions)
    {
        if (!creationOptions.Flags.HasFlag(CardCreationFlags.IsCardReward))
        {
            return false;
        }

        if (creationOptions.Flags.HasFlag(CardCreationFlags.NoCardPoolModifications) ||
            creationOptions.Flags.HasFlag(CardCreationFlags.NoCardModelModifications))
        {
            return false;
        }

        return creationOptions.Source == CardCreationSource.Encounter &&
            creationOptions.RarityOdds == CardRarityOddsType.RegularEncounter &&
            creationOptions.CustomCardPool == null &&
            creationOptions.CardPoolFilter == null &&
            creationOptions.CardPools.Count > 0 &&
            !creationOptions.CardPools.All(pool => pool.IsColorless);
    }
}
