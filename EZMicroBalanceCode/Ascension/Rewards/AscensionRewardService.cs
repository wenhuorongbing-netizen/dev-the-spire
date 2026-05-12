namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class AscensionRewardService
{
    private const int FiremarkedEliteRewardTargetOptionCount = 4;
    private const int BossRewardTargetOptionCount = 4;
    private const int NormalFissionChancePercent = 10;
    private const int BannerFissionChancePercent = 15;
    private const int FiremarkedEliteFissionChancePercent = 20;
    private const int BossFissionChancePercent = 5;

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
        MainFile.Logger.Info("[EZMicroBalance] Ascension A17 applied: Deep Branch treasure added an Uncommon relic reward.");
        return true;
    }

    private static bool TryAddA20BossOneCardReward(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        var runState = player.RunState;
        if (!AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(runState) ||
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
            "[EZMicroBalance] Ascension A20 applied: Boss 1 terminal rewards added one Boss card reward before the second Boss.");
        return true;
    }

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

        var pool = creationOptions.GetPossibleCards(player)
            .Where(card => !existingIds.Contains(card.Id))
            .Where(card => card.Rarity is CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare)
            .ToList();

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
            $"[EZMicroBalance] Ascension A12 applied: added fourth Firemarked Elite card reward option {extraCard.Id.Entry}.");
        return true;
    }

    private static bool TryApplyFission(
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions)
    {
        if (creationOptions.Source != CardCreationSource.Encounter ||
            creationOptions.Flags.HasFlag(CardCreationFlags.NoCardModelModifications) ||
            creationOptions.Flags.HasFlag(CardCreationFlags.NoModifyHooks))
        {
            return false;
        }

        if (cardRewardOptions.Any(option => option.Card.Enchantment is FissionEnchantment))
        {
            return false;
        }

        var chancePercent = GetFissionChancePercent(player.RunState, creationOptions, out var sourceLabel);
        if (chancePercent <= 0)
        {
            return false;
        }

        var candidates = cardRewardOptions
            .Where(option => !option.HasBeenModified)
            .Where(option => IsFissionEligible(option.Card))
            .ToList();
        if (candidates.Count == 0)
        {
            LogFissionDiagnostics(sourceLabel, chancePercent, 0, roll: null, applied: false, cardId: null);
            return false;
        }

        var roll = player.PlayerRng.Rewards.NextInt(100);
        if (roll >= chancePercent)
        {
            LogFissionDiagnostics(sourceLabel, chancePercent, candidates.Count, roll, applied: false, cardId: null);
            return false;
        }

        var candidate = player.PlayerRng.Rewards.NextItem(candidates);
        if (candidate == null)
        {
            LogFissionDiagnostics(sourceLabel, chancePercent, candidates.Count, roll, applied: false, cardId: null);
            return false;
        }

        var modifiedCard = player.RunState.CloneCard(candidate.Card);
        CardCmd.Enchant<FissionEnchantment>(modifiedCard, 1m);
        candidate.ModifyCard(modifiedCard);

        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension A13 applied: added Fission to {modifiedCard.Id.Entry} in a {sourceLabel} card reward ({chancePercent}% source chance).");
        LogFissionDiagnostics(sourceLabel, chancePercent, candidates.Count, roll, applied: true, modifiedCard.Id.Entry);
        return true;
    }

    private static void LogFissionDiagnostics(
        string sourceLabel,
        int chancePercent,
        int eligibleCandidateCount,
        int? roll,
        bool applied,
        string? cardId)
    {
        if (!AscensionFeatureGate.IsDiagnosticsEnabled)
        {
            return;
        }

        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension diagnostics: Fission reward roll; sourceLabel={sourceLabel}; chancePercent={chancePercent}; eligibleCandidateCount={eligibleCandidateCount}; roll={(roll.HasValue ? roll.Value.ToString() : "<none>")}; applied={applied}; cardId={cardId ?? "<none>"}.");
    }

    private static int GetFissionChancePercent(
        IRunState runState,
        CardCreationOptions creationOptions,
        out string sourceLabel)
    {
        var roomType = runState.CurrentRoom?.RoomType;
        var metadata = AscensionMapService.TryGetCurrentMetadata(runState);

        if (roomType == RoomType.Boss ||
            creationOptions.RarityOdds == CardRarityOddsType.BossEncounter)
        {
            sourceLabel = "boss";
            return BossFissionChancePercent;
        }

        if (roomType == RoomType.Elite &&
            metadata?.Firemark.HasValue == true &&
            AscensionFeatureGate.IsFiremarkedEliteEnabled(runState))
        {
            sourceLabel = "firemarked elite";
            return FiremarkedEliteFissionChancePercent;
        }

        if (roomType == RoomType.Monster &&
            metadata?.Banner.HasValue == true &&
            AscensionFeatureGate.IsBannerRoomEnabled(runState))
        {
            sourceLabel = "banner room";
            return BannerFissionChancePercent;
        }

        if (roomType == RoomType.Monster ||
            creationOptions.RarityOdds == CardRarityOddsType.RegularEncounter)
        {
            sourceLabel = "normal combat";
            return NormalFissionChancePercent;
        }

        sourceLabel = "unsupported";
        return 0;
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
            $"[EZMicroBalance] Ascension A19 applied: added fourth boss card reward option {extraCard.Id.Entry}.");
        return true;
    }

    private static bool IsFissionEligible(CardModel card)
    {
        return ModelDb.Enchantment<FissionEnchantment>().CanEnchant(card) &&
            card.Type is CardType.Attack or CardType.Skill &&
            IsFissionEligibleRarity(card.Rarity) &&
            !card.EnergyCost.CostsX &&
            !card.HasStarCostX &&
            card.CurrentStarCost <= 0 &&
            card.EnergyCost.Canonical > 0 &&
            card.EnergyCost.GetWithModifiers(CostModifiers.None) > 0 &&
            !card.Keywords.Contains(CardKeyword.Exhaust) &&
            !card.ExhaustOnNextPlay &&
            card.Enchantment == null &&
            card.CanBeGeneratedByModifiers;
    }

    private static bool IsFissionEligibleRarity(CardRarity rarity)
    {
        return rarity is CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare;
    }
}
