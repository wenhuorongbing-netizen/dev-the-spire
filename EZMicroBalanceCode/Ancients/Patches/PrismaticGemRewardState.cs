namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed partial class PrismaticGemRewardPatch
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

    public static bool HasPrismaticAllOffColorHint(IReadOnlyList<CardCreationResult> options)
    {
        return options.Any(option => RewardResultHints.TryGetValue(option, out _));
    }
}
