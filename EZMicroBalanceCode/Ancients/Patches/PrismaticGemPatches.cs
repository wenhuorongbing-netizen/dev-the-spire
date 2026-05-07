namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(PrismaticGem), nameof(PrismaticGem.ModifyCardRewardCreationOptions))]
internal static class PrismaticGemPoolPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardCreationOptions options, ref CardCreationOptions __result)
    {
        __result = options;
        return false;
    }
}

[HarmonyPatch(typeof(CardReward), nameof(CardReward.Populate))]
internal static class PrismaticGemRewardScreenContextPatch
{
    [ThreadStatic]
    private static Stack<CardReward>? PopulateStack;

    internal static CardReward? CurrentReward =>
        PopulateStack is { Count: > 0 } ? PopulateStack.Peek() : null;

    [HarmonyPrefix]
    private static void Prefix(CardReward __instance)
    {
        (PopulateStack ??= new Stack<CardReward>()).Push(__instance);
    }

    [HarmonyFinalizer]
    private static void Finalizer(CardReward __instance)
    {
        if (PopulateStack is not { Count: > 0 })
        {
            return;
        }

        if (ReferenceEquals(PopulateStack.Peek(), __instance))
        {
            PopulateStack.Pop();
            return;
        }

        PopulateStack.Clear();
    }
}

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.TryModifyCardRewardOptions))]
internal static class PrismaticGemRewardPatch
{
    private sealed class RewardScreenState
    {
        public bool HasTriggerDecision { get; set; }

        public bool ShouldReplaceAllSlots { get; set; }

        public int CounterAtDecision { get; set; }
    }

    private sealed class RewardResultHintState;

    private static readonly ConditionalWeakTable<CardReward, RewardScreenState> RewardStates = new();

    private static readonly ConditionalWeakTable<CardCreationResult, RewardResultHintState> RewardResultHints = new();

    [HarmonyPrefix]
    private static bool Prefix(
        AbstractModel __instance,
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions,
        ref bool __result)
    {
        if (__instance is not PrismaticGem prismaticGem)
        {
            return true;
        }

        __result = TryReplaceNormalRewardScreen(prismaticGem, player, cardRewardOptions, creationOptions);
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
            MainFile.Logger.Warn($"[EZMicroBalance] PrismaticGem skipped: all reward slots could not be replaced on normal reward {screenState.CounterAtDecision}.");
            return false;
        }

        prismaticGem.Flash();
        MainFile.Logger.Info($"[EZMicroBalance] PrismaticGem applied: replaced every visible reward slot with off-color cards on normal reward {screenState.CounterAtDecision}.");
        return true;
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

    private static IEnumerable<CardModel> GetOffColorRewardPool(
        Player player,
        CardRarity? rarity,
        ISet<ModelId> excludedIds)
    {
        var homePool = player.Character.CardPool;

        return ModelDb.AllCharacterCardPools
            .Where(pool => !pool.Id.Equals(homePool.Id) && !pool.IsColorless)
            .SelectMany(pool => pool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint))
            .Where(card => rarity == null || card.Rarity == rarity)
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
                return false;
            }

            PreserveUpgradeState(originalCard, replacement);
            replacements.Add((reward, originalCard, replacement));
            excludedIds.Add(replacement.Id);
        }

        foreach (var (reward, originalCard, replacement) in replacements)
        {
            reward.ModifyCard(replacement, prismaticGem);
            RewardResultHints.GetValue(reward, _ => new RewardResultHintState());

            if (player.RunState.ContainsCard(originalCard))
            {
                player.RunState.RemoveCard(originalCard);
            }
        }

        return true;
    }

    private static CardModel? CreateOffColorReplacement(
        Player player,
        CardModel originalCard,
        ISet<ModelId> excludedIds,
        int slotIndex,
        int counterAtDecision)
    {
        var replacementCanonical = GetOffColorRewardPool(player, originalCard.Rarity, excludedIds)
            .ToList()
            .StableShuffle(player.PlayerRng.Rewards)
            .FirstOrDefault();
        if (replacementCanonical == null)
        {
            MainFile.Logger.Warn($"[EZMicroBalance] PrismaticGem fallback: no unique off-color {originalCard.Rarity} card available for slot {slotIndex + 1} on normal reward {counterAtDecision}; relaxing rarity only.");

            // Safest fallback: keep all color/type/pool exclusions and duplicate protection,
            // but relax rarity before giving up. This preserves the intended off-color
            // screen without adding slots, crashing, or duplicating an already offered card.
            replacementCanonical = GetOffColorRewardPool(player, null, excludedIds)
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

    public static bool HasPrismaticAllOffColorHint(IReadOnlyList<CardCreationResult> options)
    {
        return options.Any(option => RewardResultHints.TryGetValue(option, out _));
    }

    public static MegaCrit.Sts2.Core.HoverTips.IHoverTip CreateCountHoverTip(PrismaticGem prismaticGem)
    {
        var count = AncientSavedStateFields.PrismaticGemNormalRewardCounter[prismaticGem] % 2;
        var title = new LocString("relics", "PRISMATIC_GEM.countHint.title");
        title.Add("Count", (decimal)count);
        title.Add("Cycle", 2m);

        var descriptionKey = count == 0
            ? "PRISMATIC_GEM.countHint.nextNormal"
            : "PRISMATIC_GEM.countHint.nextOffColor";
        return new MegaCrit.Sts2.Core.HoverTips.HoverTip(title, new LocString("relics", descriptionKey));
    }
}

[HarmonyPatch(typeof(RelicModel), "get_HoverTips")]
internal static class PrismaticGemHoverTipsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(RelicModel __instance, ref IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> __result)
    {
        if (__instance is not PrismaticGem prismaticGem)
        {
            return true;
        }

        __result = new[]
        {
            __instance.HoverTip,
            PrismaticGemRewardPatch.CreateCountHoverTip(prismaticGem)
        };
        return false;
    }
}

[HarmonyPatch(typeof(RelicModel), "get_HoverTipsExcludingRelic")]
internal static class PrismaticGemHoverTipsExcludingRelicPatch
{
    [HarmonyPrefix]
    private static bool Prefix(RelicModel __instance, ref IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> __result)
    {
        if (__instance is not PrismaticGem prismaticGem)
        {
            return true;
        }

        __result = new[] { PrismaticGemRewardPatch.CreateCountHoverTip(prismaticGem) };
        return false;
    }
}

[HarmonyPatch(
    typeof(MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen),
    nameof(MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen.RefreshOptions))]
internal static class PrismaticGemRewardScreenHintPatch
{
    private const string BannerNodePath = "UI/Banner";

    private static readonly System.Reflection.FieldInfo? BannerField =
        AccessTools.Field(typeof(MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen), "_banner");

    private static bool BannerFieldSuccessLogged;

    private static bool BannerFieldFailureLogged;

    private static bool BannerNodeFallbackLogged;

    private static bool BannerNodeConfirmationLogged;

    private static bool BannerNodeConfirmationFailureLogged;

    private static bool BannerUnavailableLogged;

    [HarmonyPostfix]
    private static void Postfix(
        MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen __instance,
        IReadOnlyList<CardCreationResult> options)
    {
        if (!PrismaticGemRewardPatch.HasPrismaticAllOffColorHint(options))
        {
            return;
        }

        ApplyRewardScreenHint(__instance);
    }

    private static void ApplyRewardScreenHint(
        MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen screen)
    {
        var hintText = new LocString("relics", "PRISMATIC_GEM.rewardScreenHint").GetFormattedText();
        if (TryApplyBannerFieldHint(screen, hintText))
        {
            ConfirmBannerNodeHintAfterFieldSuccess(screen, hintText);
            return;
        }

        if (TryApplyBannerNodeHint(screen, hintText))
        {
            return;
        }

        WarnOnce(
            ref BannerUnavailableLogged,
            "[EZMicroBalance] PrismaticGem reward-screen hint unavailable: private _banner and UI/Banner fallback both failed; visible all-off-color cards and the Prismatic Gem relic hover count remain available for manual confirmation.");
    }

    private static bool TryApplyBannerFieldHint(
        MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen screen,
        string hintText)
    {
        if (!TryGetCompatibleBannerField(out var bannerField, out var reason))
        {
            WarnOnce(
                ref BannerFieldFailureLogged,
                $"[EZMicroBalance] PrismaticGem reward-screen hint fallback: private _banner field unavailable ({reason}); trying {BannerNodePath}.");
            return false;
        }

        try
        {
            if (bannerField.GetValue(screen) is not MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner banner)
            {
                WarnOnce(
                    ref BannerFieldFailureLogged,
                    $"[EZMicroBalance] PrismaticGem reward-screen hint fallback: private _banner field resolved but did not contain a banner instance; trying {BannerNodePath}.");
                return false;
            }

            if (!banner.IsInsideTree())
            {
                WarnOnce(
                    ref BannerFieldFailureLogged,
                    $"[EZMicroBalance] PrismaticGem reward-screen hint fallback: private _banner field resolved to a detached banner; trying {BannerNodePath}.");
                return false;
            }

            banner.ChangeText(hintText);
            InfoOnce(
                ref BannerFieldSuccessLogged,
                "[EZMicroBalance] PrismaticGem reward-screen hint applied through the guarded private _banner field; visual placement still requires manual gameplay verification.");
            return true;
        }
        catch (Exception exception)
        {
            WarnOnce(
                ref BannerFieldFailureLogged,
                $"[EZMicroBalance] PrismaticGem reward-screen hint fallback: private _banner access failed with {exception.GetType().Name}; trying {BannerNodePath}.");
            return false;
        }
    }

    private static bool TryApplyBannerNodeHint(
        MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen screen,
        string hintText)
    {
        try
        {
            var banner = screen.GetNodeOrNull<MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner>(BannerNodePath);
            if (banner == null)
            {
                WarnOnce(
                    ref BannerNodeFallbackLogged,
                    $"[EZMicroBalance] PrismaticGem reward-screen hint fallback unavailable: {BannerNodePath} node was not found.");
                return false;
            }

            banner.ChangeText(hintText);
            InfoOnce(
                ref BannerNodeFallbackLogged,
                $"[EZMicroBalance] PrismaticGem reward-screen hint fallback applied through {BannerNodePath} node lookup.");
            return true;
        }
        catch (Exception exception)
        {
            WarnOnce(
                ref BannerNodeFallbackLogged,
                $"[EZMicroBalance] PrismaticGem reward-screen hint fallback through {BannerNodePath} failed with {exception.GetType().Name}.");
            return false;
        }
    }

    private static bool TryGetCompatibleBannerField(
        out System.Reflection.FieldInfo bannerField,
        out string reason)
    {
        if (BannerField == null)
        {
            bannerField = null!;
            reason = "field not found";
            return false;
        }

        if (!typeof(MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner).IsAssignableFrom(BannerField.FieldType))
        {
            bannerField = null!;
            reason = $"field type was {BannerField.FieldType.FullName}";
            return false;
        }

        bannerField = BannerField;
        reason = string.Empty;
        return true;
    }

    private static void ConfirmBannerNodeHintAfterFieldSuccess(
        MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen screen,
        string hintText)
    {
        // Reflection is a private API dependency; also update the public node path when
        // available so a stale reflected field cannot be the only hint surface.
        try
        {
            var banner = screen.GetNodeOrNull<MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner>(BannerNodePath);
            if (banner == null)
            {
                WarnOnce(
                    ref BannerNodeConfirmationFailureLogged,
                    $"[EZMicroBalance] PrismaticGem reward-screen hint confirmation unavailable after private _banner update: {BannerNodePath} node was not found; visual placement still requires manual gameplay verification.");
                return;
            }

            banner.ChangeText(hintText);
            InfoOnce(
                ref BannerNodeConfirmationLogged,
                $"[EZMicroBalance] PrismaticGem reward-screen hint also applied through {BannerNodePath} node lookup after private _banner field path; visual placement still requires manual gameplay verification.");
        }
        catch (Exception exception)
        {
            WarnOnce(
                ref BannerNodeConfirmationFailureLogged,
                $"[EZMicroBalance] PrismaticGem reward-screen hint confirmation after private _banner update failed with {exception.GetType().Name}; visual placement still requires manual gameplay verification.");
        }
    }

    private static void InfoOnce(ref bool logged, string message)
    {
        if (logged)
        {
            return;
        }

        logged = true;
        MainFile.Logger.Info(message);
    }

    private static void WarnOnce(ref bool logged, string message)
    {
        if (logged)
        {
            return;
        }

        logged = true;
        MainFile.Logger.Warn(message);
    }
}
