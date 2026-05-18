namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Hooks.Hook), nameof(MegaCrit.Sts2.Core.Hooks.Hook.TryModifyCardRewardOptions))]
internal static partial class PrismaticGemRewardPatch
{
    private sealed class RewardScreenState
    {
        public bool HasTriggerDecision { get; set; }

        public bool ShouldReplaceAllSlots { get; set; }

        public int CounterAtDecision { get; set; }
    }

    private sealed class RewardResultHintState
    {
        public CardModel? PrismaticReplacement { get; set; }
    }

    private static readonly ConditionalWeakTable<CardReward, RewardScreenState> RewardStates = new();

    private static readonly ConditionalWeakTable<CardCreationResult, RewardResultHintState> RewardResultHints = new();

    [HarmonyPrefix]
    private static bool Prefix(
        IRunState runState,
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions,
        ref List<AbstractModel> modifiers,
        ref bool __result)
    {
        var prismaticGem = player.Relics.OfType<PrismaticGem>().FirstOrDefault(relic => !relic.IsMelted);
        if (prismaticGem == null)
        {
            return true;
        }

        var modified = false;
        modifiers = [];
        foreach (var listener in runState.IterateHookListeners(null))
        {
            modified = listener.TryModifyCardRewardOptions(player, cardRewardOptions, creationOptions) || modified;
            modifiers.Add(listener);
        }

        modified = TryReplaceNormalRewardScreen(prismaticGem, player, cardRewardOptions, creationOptions) || modified;

        foreach (var listener in runState.IterateHookListeners(null))
        {
            modified = listener.TryModifyCardRewardOptionsLate(player, cardRewardOptions, creationOptions) || modified;
            modifiers.Add(listener);
        }

        CleanupSupersededPrismaticReplacements(cardRewardOptions);
        __result = modified;
        return false;
    }

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
                MainFile.Logger.Warn("[EZMicroBalance] PrismaticGem skipped: normal card reward modification had no CardReward screen context.");
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
                MainFile.Logger.Info("[EZMicroBalance] PrismaticGem ignored non-normal card reward screen; no counter increment.");
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
                MainFile.Logger.Info($"[EZMicroBalance] PrismaticGem applied: counted normal card reward {screenState.CounterAtDecision}; no replacement for this reward screen.");
            }

            return false;
        }

        if (!ReplaceAllRewardSlots(prismaticGem, player, cardRewardOptions, screenState.CounterAtDecision))
        {
            RestoreCounterAfterFailedReplacement(prismaticGem, screenState);
            MainFile.Logger.Warn($"[EZMicroBalance] PrismaticGem skipped: all reward slots could not be replaced on normal reward {screenState.CounterAtDecision}.");
            return false;
        }

        prismaticGem.Flash();
        MainFile.Logger.Info($"[EZMicroBalance] PrismaticGem applied: replaced every visible reward slot with off-color cards on normal reward {screenState.CounterAtDecision}.");
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

    public static bool HasPrismaticAllOffColorHint(IReadOnlyList<CardCreationResult> options)
    {
        return options.Any(option => RewardResultHints.TryGetValue(option, out _));
    }
}
