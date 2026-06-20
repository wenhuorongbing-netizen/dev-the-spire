namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static partial class PrismaticGemRewardPatch
{
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
            MainFile.Logger.Warn($"[Spire Plus] PrismaticGem fallback: no unique off-color {originalCard.Rarity} {originalCard.Type} card available for slot {slotIndex + 1} on normal reward {counterAtDecision}; relaxing rarity only.");

            replacementCanonical = GetOffColorRewardPool(player, null, originalCard.Type, excludedIds)
                .ToList()
                .StableShuffle(player.PlayerRng.Rewards)
                .FirstOrDefault();
        }

        if (replacementCanonical == null)
        {
            MainFile.Logger.Warn($"[Spire Plus] PrismaticGem fallback: no unique off-color {originalCard.Type} card available for slot {slotIndex + 1} on normal reward {counterAtDecision}; relaxing type only.");

            replacementCanonical = GetOffColorRewardPool(player, originalCard.Rarity, null, excludedIds)
                .ToList()
                .StableShuffle(player.PlayerRng.Rewards)
                .FirstOrDefault();
        }

        if (replacementCanonical == null)
        {
            MainFile.Logger.Warn($"[Spire Plus] PrismaticGem fallback: no unique off-color {originalCard.Rarity} card available for slot {slotIndex + 1} on normal reward {counterAtDecision}; relaxing rarity and type.");

            replacementCanonical = GetOffColorRewardPool(player, null, null, excludedIds)
                .ToList()
                .StableShuffle(player.PlayerRng.Rewards)
                .FirstOrDefault();
        }

        if (replacementCanonical == null)
        {
            MainFile.Logger.Warn($"[Spire Plus] PrismaticGem skipped slot {slotIndex + 1} on normal reward {counterAtDecision}: no unique valid off-color replacement was available.");
            return null;
        }

        return player.RunState.CreateCard(replacementCanonical, player);
    }

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
}
