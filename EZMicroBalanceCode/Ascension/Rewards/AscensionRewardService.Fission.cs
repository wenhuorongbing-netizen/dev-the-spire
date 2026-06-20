namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionRewardService
{
    private const int NormalFissionChancePercent = 10;
    private const int BannerFissionChancePercent = 15;
    private const int FiremarkedEliteFissionChancePercent = 20;
    private const int BossFissionChancePercent = 5;

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
            .Where(option => IsFissionEligible(option.Card))
            .ToList();
        if (candidates.Count == 0)
        {
            LogFissionDiagnostics(sourceLabel, chancePercent, 0, roll: null, applied: false, cardId: null);
            return false;
        }

        var rewardRng = creationOptions.RngOverride ?? player.PlayerRng.Rewards;
        var roll = rewardRng.NextInt(100);
        if (roll >= chancePercent)
        {
            LogFissionDiagnostics(sourceLabel, chancePercent, candidates.Count, roll, applied: false, cardId: null);
            return false;
        }

        var candidate = rewardRng.NextItem(candidates);
        if (candidate == null)
        {
            LogFissionDiagnostics(sourceLabel, chancePercent, candidates.Count, roll, applied: false, cardId: null);
            return false;
        }

        var modifiedCard = player.RunState.CloneCard(candidate.Card);
        CardCmd.Enchant<FissionEnchantment>(modifiedCard, 1m);
        candidate.ModifyCard(modifiedCard);

        MainFile.Logger.Info(
            $"[Spire Plus] Ascension A13 applied: added Fission to {modifiedCard.Id.Entry} in a {sourceLabel} card reward ({chancePercent}% source chance).");
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
            $"[Spire Plus] Ascension diagnostics: Fission reward roll; sourceLabel={sourceLabel}; chancePercent={chancePercent}; eligibleCandidateCount={eligibleCandidateCount}; roll={(roll.HasValue ? roll.Value.ToString() : "<none>")}; applied={applied}; cardId={cardId ?? "<none>"}.");
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
