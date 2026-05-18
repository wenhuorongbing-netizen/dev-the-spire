namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static partial class PrismaticGemRewardPatch
{
    private static IEnumerable<CardModel> GetOffColorRewardPool(
        Player player,
        CardRarity? rarity,
        CardType? type,
        ISet<ModelId> excludedIds)
    {
        var homePool = player.Character.CardPool;

        return ModelDb.AllCharacterCardPools
            .Where(pool => !pool.Id.Equals(homePool.Id) && !pool.IsColorless)
            .SelectMany(pool => pool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint))
            .Where(card => rarity == null || card.Rarity == rarity)
            .Where(card => type == null || card.Type == type)
            .Where(card => card.Type is not CardType.Curse and not CardType.Status and not CardType.Quest)
            .Where(card => card.CanBeGeneratedByModifiers)
            .Where(card => !excludedIds.Contains(card.Id))
            .DistinctBy(card => card.Id);
    }

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

    private static CardModel? CreateOffColorReplacement(
        Player player,
        CardModel originalCard,
        ISet<ModelId> excludedIds,
        int slotIndex,
        int counterAtDecision)
    {
        var replacementCanonical = GetOffColorRewardPool(player, originalCard.Rarity, originalCard.Type, excludedIds)
            .ToList()
            .StableShuffle(player.PlayerRng.Rewards)
            .FirstOrDefault();
        if (replacementCanonical == null)
        {
            MainFile.Logger.Warn($"[EZMicroBalance] PrismaticGem fallback: no unique off-color {originalCard.Rarity} {originalCard.Type} card available for slot {slotIndex + 1} on normal reward {counterAtDecision}; relaxing rarity only.");

            replacementCanonical = GetOffColorRewardPool(player, null, originalCard.Type, excludedIds)
                .ToList()
                .StableShuffle(player.PlayerRng.Rewards)
                .FirstOrDefault();
        }

        if (replacementCanonical == null)
        {
            MainFile.Logger.Warn($"[EZMicroBalance] PrismaticGem fallback: no unique off-color {originalCard.Type} card available for slot {slotIndex + 1} on normal reward {counterAtDecision}; relaxing type only.");

            replacementCanonical = GetOffColorRewardPool(player, originalCard.Rarity, null, excludedIds)
                .ToList()
                .StableShuffle(player.PlayerRng.Rewards)
                .FirstOrDefault();
        }

        if (replacementCanonical == null)
        {
            MainFile.Logger.Warn($"[EZMicroBalance] PrismaticGem fallback: no unique off-color {originalCard.Rarity} card available for slot {slotIndex + 1} on normal reward {counterAtDecision}; relaxing rarity and type.");

            replacementCanonical = GetOffColorRewardPool(player, null, null, excludedIds)
                .ToList()
                .StableShuffle(player.PlayerRng.Rewards)
                .FirstOrDefault();
        }

        if (replacementCanonical == null)
        {
            MainFile.Logger.Warn($"[EZMicroBalance] PrismaticGem skipped slot {slotIndex + 1} on normal reward {counterAtDecision}: no unique valid off-color replacement was available.");
            return null;
        }

        return player.RunState.CreateCard(replacementCanonical, player);
    }

    private static void PreserveUpgradeState(CardModel originalCard, CardModel replacement)
    {
        if (originalCard.IsUpgraded && replacement.IsUpgradable)
        {
            CardCmd.Upgrade(replacement);
        }
    }
}
