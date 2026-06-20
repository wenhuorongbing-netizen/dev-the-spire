namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionRewardService
{
    private const int FiremarkedEliteRewardTargetOptionCount = 4;
    private const int BossRewardTargetOptionCount = 4;

    private static bool TryAddFiremarkedEliteRewardOption(
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions)
    {
        var metadata = AscensionMapService.TryGetCurrentMetadata(player.RunState);
        if (metadata?.Firemark == null ||
            player.RunState.CurrentRoom?.RoomType != RoomType.Elite ||
            creationOptions.Source != CardCreationSource.Encounter ||
            creationOptions.RarityOdds != CardRarityOddsType.EliteEncounter ||
            cardRewardOptions.Count >= FiremarkedEliteRewardTargetOptionCount)
        {
            return false;
        }

        var existingIds = cardRewardOptions
            .Select(option => option.Card.Id)
            .ToHashSet();

        var duplicateTokenReward = ForgeTokenService.HasToken(player);
        var pool = creationOptions.GetPossibleCards(player)
            .Where(card => !existingIds.Contains(card.Id))
            .Where(card => card.Rarity is CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare)
            .Where(card => !duplicateTokenReward || card.IsUpgradable)
            .ToList();

        if (pool.Count == 0)
        {
            pool = creationOptions.GetPossibleCards(player)
                .Where(card => !existingIds.Contains(card.Id))
                .Where(card => card.Rarity is CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare)
                .ToList();
            if (pool.Count == 0)
            {
                return false;
            }
        }

        var extraOptions = new CardCreationOptions(pool, CardCreationSource.Other, CardRarityOddsType.Uniform)
            .WithFlags(CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications);

        if (creationOptions.RngOverride != null)
        {
            extraOptions.WithRngOverride(creationOptions.RngOverride);
        }

        var extraCard = CardFactory.CreateForReward(player, 1, extraOptions).FirstOrDefault()?.Card;
        if (extraCard == null)
        {
            return false;
        }

        if (duplicateTokenReward && extraCard.IsUpgradable)
        {
            CardCmd.Upgrade(extraCard);
            MainFile.Logger.Info(
                $"[Spire Plus] Ascension A12 applied: upgraded duplicate-token Firemarked Elite card reward option {extraCard.Id.Entry}.");
        }

        cardRewardOptions.Add(new CardCreationResult(extraCard));
        MainFile.Logger.Info(
            $"[Spire Plus] Ascension A12 applied: added fourth Firemarked Elite card reward option {extraCard.Id.Entry}.");
        return true;
    }

    private static bool TryAddBossSealRewardOption(
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions)
    {
        if (creationOptions.Source != CardCreationSource.Encounter ||
            creationOptions.RarityOdds != CardRarityOddsType.BossEncounter ||
            cardRewardOptions.Count >= BossRewardTargetOptionCount)
        {
            return false;
        }

        var existingIds = cardRewardOptions
            .Select(option => option.Card.Id)
            .ToHashSet();

        var pool = creationOptions.GetPossibleCards(player)
            .Where(card => !existingIds.Contains(card.Id))
            .Where(card => card.Rarity == CardRarity.Rare)
            .ToList();

        if (pool.Count == 0)
        {
            pool = creationOptions.GetPossibleCards(player)
                .Where(card => !existingIds.Contains(card.Id))
                .Where(card => card.Rarity is CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare)
                .ToList();
        }

        if (pool.Count == 0)
        {
            return false;
        }

        var extraOptions = new CardCreationOptions(pool, CardCreationSource.Other, CardRarityOddsType.Uniform)
            .WithFlags(CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications);

        if (creationOptions.RngOverride != null)
        {
            extraOptions.WithRngOverride(creationOptions.RngOverride);
        }

        var extraCard = CardFactory.CreateForReward(player, 1, extraOptions).FirstOrDefault()?.Card;
        if (extraCard == null)
        {
            return false;
        }

        cardRewardOptions.Add(new CardCreationResult(extraCard));
        MainFile.Logger.Info(
            $"[Spire Plus] Ascension A19 applied: added fourth boss card reward option {extraCard.Id.Entry}.");
        return true;
    }
}
