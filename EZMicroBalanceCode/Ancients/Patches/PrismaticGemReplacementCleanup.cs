namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static partial class PrismaticGemRewardPatch
{
    private static void TrackPrismaticReplacement(CardCreationResult reward, CardModel replacement)
    {
        RewardResultHints.GetValue(reward, _ => new RewardResultHintState()).PrismaticReplacement = replacement;
    }

    private static void CleanupSupersededPrismaticReplacements(IEnumerable<CardCreationResult> rewards)
    {
        foreach (var reward in rewards)
        {
            if (!RewardResultHints.TryGetValue(reward, out var hintState) ||
                hintState.PrismaticReplacement == null ||
                ReferenceEquals(reward.Card, hintState.PrismaticReplacement))
            {
                continue;
            }

            AncientCardHelpers.RemoveUnpiledRunCard(hintState.PrismaticReplacement);
            hintState.PrismaticReplacement = null;
        }
    }

    private static void RemoveUnpiledReplacements(IEnumerable<(CardCreationResult Reward, CardModel OriginalCard, CardModel Replacement)> replacements)
    {
        foreach (var (_, _, replacement) in replacements)
        {
            AncientCardHelpers.RemoveUnpiledRunCard(replacement);
        }
    }
}
