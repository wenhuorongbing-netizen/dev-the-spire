namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static partial class PrismaticGemRewardPatch
{
    private static bool ReplaceAllRewardSlots(
        PrismaticGem prismaticGem,
        Player player,
        List<CardCreationResult> cardRewardOptions,
        int counterAtDecision)
    {
        var excludedIds = cardRewardOptions
            .Select(result => result.Card.Id)
            .ToHashSet();
        var replacements = new List<(CardCreationResult Reward, CardModel OriginalCard, CardModel Replacement)>(cardRewardOptions.Count);

        for (var slotIndex = 0; slotIndex < cardRewardOptions.Count; slotIndex++)
        {
            var reward = cardRewardOptions[slotIndex];
            var originalCard = reward.Card;
            var replacement = CreateOffColorReplacement(player, originalCard, excludedIds, slotIndex, counterAtDecision);
            if (replacement == null)
            {
                RemoveUnpiledReplacements(replacements);
                return false;
            }

            PreserveUpgradeState(originalCard, replacement);
            replacements.Add((reward, originalCard, replacement));
            excludedIds.Add(replacement.Id);
        }

        foreach (var (reward, originalCard, replacement) in replacements)
        {
            reward.ModifyCard(replacement, prismaticGem);
            TrackPrismaticReplacement(reward, replacement);

            if (player.RunState.ContainsCard(originalCard))
            {
                player.RunState.RemoveCard(originalCard);
            }
        }

        return true;
    }

    private static void PreserveUpgradeState(CardModel originalCard, CardModel replacement)
    {
        if (originalCard.IsUpgraded && replacement.IsUpgradable)
        {
            CardCmd.Upgrade(replacement);
        }
    }
}
