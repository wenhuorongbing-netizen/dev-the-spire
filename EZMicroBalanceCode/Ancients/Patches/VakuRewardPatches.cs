namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(JewelryBox), nameof(JewelryBox.AfterObtained))]
internal static class JewelryBoxPatch
{
    [HarmonyPrefix]
    private static bool Prefix(JewelryBox __instance, ref Task __result)
    {
        __result = AddNonInnateApotheosis(__instance);
        return false;
    }

    private static async Task AddNonInnateApotheosis(JewelryBox jewelryBox)
    {
        var card = CreateNonInnateApotheosis(jewelryBox.Owner);
        var result = await CardPileCmd.Add(card, PileType.Deck, source: jewelryBox);
        JewelryBoxApotheosisMarker.Mark(result.cardAdded);
        CardCmd.PreviewCardPileAdd(result, 2f);
        MainFile.Logger.Info("[EZMicroBalance] JewelryBox applied: added Apotheosis without Innate.");
    }

    public static CardModel CreateNonInnateApotheosis(Player owner)
    {
        var card = owner.RunState.CreateCard<Apotheosis>(owner);
        JewelryBoxApotheosisMarker.Mark(card);
        return card;
    }

    public static IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> CreateNonInnateApotheosisHoverTips()
    {
        var preview = ModelDb.Card<Apotheosis>().ToMutable();
        JewelryBoxApotheosisMarker.Mark(preview);
        return new[] { MegaCrit.Sts2.Core.HoverTips.HoverTipFactory.FromCard(preview) }.Concat(preview.HoverTips);
    }
}

[HarmonyPatch(typeof(Apotheosis), "get_CanonicalKeywords")]
internal static class JewelryBoxApotheosisCanonicalKeywordsPatch
{
    [HarmonyPostfix]
    private static void RemoveInnateForMarkedJewelryBoxApotheosis(Apotheosis __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (JewelryBoxApotheosisMarker.IsMarked(__instance))
        {
            __result = __result.Where(keyword => keyword != CardKeyword.Innate).ToArray();
        }
    }
}

internal static class JewelryBoxApotheosisMarker
{
    private sealed class MarkerState;

    private static readonly ConditionalWeakTable<CardModel, MarkerState> MarkedCards = new();

    public static void Mark(CardModel card)
    {
        if (card is not Apotheosis)
        {
            return;
        }

        MarkedCards.GetValue(card, _ => new MarkerState());
        AncientSavedStateFields.JewelryBoxNonInnateApotheosis[card] = true;
        AncientCardHelpers.RemoveKeywords(card, CardKeyword.Innate);
    }

    public static bool IsMarked(CardModel card)
    {
        return card is Apotheosis &&
            (MarkedCards.TryGetValue(card, out _) || AncientSavedStateFields.JewelryBoxNonInnateApotheosis[card]);
    }
}

[HarmonyPatch(typeof(JewelryBox), "get_ExtraHoverTips")]
internal static class JewelryBoxExtraHoverTipsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> __result)
    {
        __result = JewelryBoxPatch.CreateNonInnateApotheosisHoverTips();
        return false;
    }
}

[HarmonyPatch(typeof(RelicModel), "get_HoverTips")]
internal static class JewelryBoxHoverTipsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(RelicModel __instance, ref IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> __result)
    {
        if (__instance is not JewelryBox)
        {
            return true;
        }

        __result = new MegaCrit.Sts2.Core.HoverTips.IHoverTip[] { __instance.HoverTip }
            .Concat(JewelryBoxPatch.CreateNonInnateApotheosisHoverTips());
        return false;
    }
}

[HarmonyPatch(typeof(RelicModel), "get_HoverTipsExcludingRelic")]
internal static class JewelryBoxHoverTipsExcludingRelicPatch
{
    [HarmonyPrefix]
    private static bool Prefix(RelicModel __instance, ref IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> __result)
    {
        if (__instance is not JewelryBox)
        {
            return true;
        }

        __result = JewelryBoxPatch.CreateNonInnateApotheosisHoverTips();
        return false;
    }
}

[HarmonyPatch(typeof(DistinguishedCape), "get_CanonicalVars")]
internal static class DistinguishedCapeVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[]
        {
            new DynamicVar("HpPercent", 30m),
            new HpLossVar(DistinguishedCapePickupPatch.MinimumMaxHpLoss),
            new CardsVar(DistinguishedCapePickupPatch.ApparitionsToAdd)
        };
        return false;
    }
}

[HarmonyPatch(typeof(Vakuu), "GenerateInitialOptions")]
internal static class DistinguishedCapeEventOptionPatch
{
    [HarmonyPostfix]
    private static void ReplaceUnaffordableCapeWithPayableVakuuOption(
        Vakuu __instance,
        ref IReadOnlyList<MegaCrit.Sts2.Core.Events.EventOption> __result)
    {
        var owner = __instance.Owner;
        if (owner == null || DistinguishedCapePickupPatch.CanPayMaxHpCost(owner.Creature.MaxHp))
        {
            return;
        }

        var options = __result.ToList();
        var capeIndex = options.FindIndex(option => option.Relic is DistinguishedCape);
        if (capeIndex < 0)
        {
            return;
        }

        var replacement = CreateVakuuSecondPoolReplacement(__instance, options);
        if (replacement != null)
        {
            options[capeIndex] = replacement;
            __result = options.ToArray();
            MainFile.Logger.Info(
                $"[EZMicroBalance] DistinguishedCape replaced in Vakuu options: current max HP {owner.Creature.MaxHp} cannot pay max HP cost {DistinguishedCapePickupPatch.CalculateMaxHpLoss(owner.Creature.MaxHp)}.");
            return;
        }

        options[capeIndex] = CreateLockedCapeOption(__instance, options[capeIndex], owner.Creature.MaxHp);
        __result = options.ToArray();
        MainFile.Logger.Warn(
            $"[EZMicroBalance] DistinguishedCape shown locked in Vakuu options: no same-pool replacement was available for current max HP {owner.Creature.MaxHp}.");
    }

    private static MegaCrit.Sts2.Core.Events.EventOption? CreateVakuuSecondPoolReplacement(
        Vakuu vakuu,
        IReadOnlyCollection<MegaCrit.Sts2.Core.Events.EventOption> currentOptions)
    {
        var currentKeys = currentOptions
            .Select(option => option.TextKey)
            .ToHashSet(StringComparer.Ordinal);

        var candidates = vakuu.AllPossibleOptions
            .Where(IsPayableVakuuSecondPoolOption)
            .Where(option => !currentKeys.Contains(option.TextKey))
            .ToList();

        return candidates.Count == 0
            ? null
            : vakuu.Rng.NextItem(candidates);
    }

    private static bool IsPayableVakuuSecondPoolOption(MegaCrit.Sts2.Core.Events.EventOption option)
    {
        return option.Relic is PreservedFog or SereTalon;
    }

    private static MegaCrit.Sts2.Core.Events.EventOption CreateLockedCapeOption(
        Vakuu eventModel,
        MegaCrit.Sts2.Core.Events.EventOption originalOption,
        int currentMaxHp)
    {
        var description = new LocString("relics", "DISTINGUISHED_CAPE.unpayableOption");
        description.Add("Cost", (decimal)DistinguishedCapePickupPatch.CalculateMaxHpLoss(currentMaxHp));

        var lockedOption = new MegaCrit.Sts2.Core.Events.EventOption(
            eventModel,
            null,
            originalOption.Title,
            description,
            originalOption.TextKey,
            originalOption.HoverTips);

        if (originalOption.Relic != null)
        {
            lockedOption.WithRelic(originalOption.Relic);
        }

        return lockedOption;
    }
}

[HarmonyPatch(typeof(DistinguishedCape), nameof(DistinguishedCape.AfterObtained))]
internal static class DistinguishedCapePickupPatch
{
    public const decimal MaxHpLossPercent = 0.30m;

    public const int MinimumMaxHpLoss = 18;

    public const int ApparitionsToAdd = 3;

    [HarmonyPrefix]
    private static bool Prefix(DistinguishedCape __instance, ref Task __result)
    {
        __result = LoseMaxHpAndAddApparitions(__instance);
        return false;
    }

    public static int CalculateMaxHpLoss(int currentMaxHp)
    {
        var proportionalLoss = (int)Math.Ceiling(currentMaxHp * MaxHpLossPercent);
        return Math.Max(proportionalLoss, MinimumMaxHpLoss);
    }

    public static bool CanPayMaxHpCost(int currentMaxHp)
    {
        return currentMaxHp > CalculateMaxHpLoss(currentMaxHp);
    }

    private static async Task LoseMaxHpAndAddApparitions(DistinguishedCape cape)
    {
        var creature = cape.Owner.Creature;
        var maxHpLoss = CalculateMaxHpLoss(creature.MaxHp);
        if (!CanPayMaxHpCost(creature.MaxHp))
        {
            MainFile.Logger.Warn($"[EZMicroBalance] DistinguishedCape blocked: current max HP {creature.MaxHp} cannot pay max HP cost {maxHpLoss}.");
            return;
        }

        var newMaxHp = creature.MaxHp - maxHpLoss;

        if (creature.CurrentHp > newMaxHp)
        {
            await CreatureCmd.SetCurrentHp(creature, newMaxHp);
        }

        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), creature, maxHpLoss, isFromCard: false);

        var results = new List<CardPileAddResult>();
        for (var i = 0; i < ApparitionsToAdd; i++)
        {
            var apparition = cape.Owner.RunState.CreateCard<Apparition>(cape.Owner);
            results.Add(await CardPileCmd.Add(apparition, PileType.Deck));
        }

        CardCmd.PreviewCardPileAdd(results, 2f);
        MainFile.Logger.Info($"[EZMicroBalance] DistinguishedCape applied: lost {maxHpLoss} max HP and added {results.Count} Apparition card(s).");
    }
}

[HarmonyPatch(typeof(PreservedFog), nameof(PreservedFog.AfterObtained))]
internal static class PreservedFogPatch
{
    [HarmonyPrefix]
    private static bool Prefix(PreservedFog __instance, ref Task __result)
    {
        __result = RemoveFourCardsAndAddPersistentFolly(__instance);
        return false;
    }

    private static async Task RemoveFourCardsAndAddPersistentFolly(PreservedFog preservedFog)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 4);
        foreach (var card in await CardSelectCmd.FromDeckForRemoval(preservedFog.Owner, prefs))
        {
            await CardPileCmd.RemoveFromDeck(card);
        }

        var folly = preservedFog.Owner.RunState.CreateCard<Folly>(preservedFog.Owner);
        AncientCardHelpers.RemoveKeywords(folly, CardKeyword.Ethereal, CardKeyword.Retain);
        var result = await CardPileCmd.Add(folly, PileType.Deck);
        CardCmd.PreviewCardPileAdd(result, 2f);
        MainFile.Logger.Info("[EZMicroBalance] PreservedFog applied: removed up to 4 cards and added Folly without Ethereal/Retain.");
    }
}

[HarmonyPatch(typeof(Folly), "get_CanonicalKeywords")]
internal static class FollyKeywordsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        __result = new[] { CardKeyword.Unplayable, CardKeyword.Eternal, CardKeyword.Innate };
        return false;
    }
}

[HarmonyPatch(typeof(ChoicesParadox), nameof(ChoicesParadox.AfterPlayerTurnStart))]
internal static class ChoicesParadoxPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ChoicesParadox __instance, PlayerChoiceContext choiceContext, Player player, ref Task __result)
    {
        if (player != __instance.Owner)
        {
            return true;
        }

        __result = ChooseRareTemporaryCard(__instance, choiceContext, player);
        return false;
    }

    private static async Task ChooseRareTemporaryCard(ChoicesParadox choicesParadox, PlayerChoiceContext choiceContext, Player player)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null || combatState.RoundNumber != 1)
        {
            return;
        }

        var pool = ModelDb.AllCharacterCardPools
            .Concat(new[] { ModelDb.CardPool<ColorlessCardPool>() })
            .SelectMany(cardPool => cardPool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint))
            .Where(IsChoicesParadoxEligibleRare)
            .Distinct()
            .ToList();
        var generated = CardFactory.GetDistinctForCombat(
                player,
                pool,
                choicesParadox.DynamicVars.Cards.IntValue,
                player.RunState.Rng.CombatCardGeneration)
            .ToList();

        if (generated.Count == 0)
        {
            MainFile.Logger.Warn("[EZMicroBalance] ChoicesParadox skipped: no eligible rare combat cards generated.");
            return;
        }

        choicesParadox.Flash();
        foreach (var card in generated)
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Retain);
        }

        var selected = (await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                generated,
                player,
                new CardSelectorPrefs(new LocString("relics", "CHOICES_PARADOX.selectionScreenPrompt"), 1)))
            .FirstOrDefault();

        foreach (var card in generated.Where(card => card != selected))
        {
            combatState.RemoveCard(card);
        }

        if (selected != null)
        {
            await AncientCardHelpers.TryAddGeneratedCardToCombat(selected, PileType.Hand, player);
        }

        MainFile.Logger.Info($"[EZMicroBalance] ChoicesParadox applied: offered {generated.Count} rare card(s), selected {selected?.Id.Entry ?? "NONE"}.");
    }

    private static bool IsChoicesParadoxEligibleRare(CardModel card)
    {
        return card.Rarity == CardRarity.Rare &&
            card.Type is not CardType.Curse and not CardType.Status and not CardType.Quest &&
            !card.Keywords.Contains(CardKeyword.Unplayable) &&
            card.CanBeGeneratedInCombat &&
            card.CanBeGeneratedByModifiers;
    }
}

[HarmonyPatch(typeof(JeweledMask), nameof(JeweledMask.BeforeHandDraw))]
internal static class JeweledMaskCombatStartPatch
{
    [HarmonyPrefix]
    private static bool Prefix(JeweledMask __instance, Player player, ICombatState combatState, ref Task __result)
    {
        if (player != __instance.Owner || combatState.RoundNumber > 1)
        {
            return true;
        }

        __result = PullMarkedPowerToHand(__instance, player);
        return false;
    }

    private static async Task PullMarkedPowerToHand(JeweledMask jeweledMask, Player player)
    {
        var drawPile = PileType.Draw.GetPile(player);
        var markedPower = drawPile.Cards.FirstOrDefault(AncientCardHelpers.IsJeweledMaskPower);
        if (markedPower != null)
        {
            jeweledMask.Flash();
            await CardPileCmd.Add(markedPower, PileType.Hand);
            MainFile.Logger.Info($"[EZMicroBalance] JeweledMask applied: moved marked power {markedPower.Id.Entry} from draw pile to hand.");
            return;
        }

        if (PileType.Hand.GetPile(player).Cards.Any(AncientCardHelpers.IsJeweledMaskPower))
        {
            MainFile.Logger.Info("[EZMicroBalance] JeweledMask skipped pull: marked power already in hand.");
            return;
        }

        MainFile.Logger.Info("[EZMicroBalance] JeweledMask skipped pull: no marked power in draw pile or hand.");
    }
}

[HarmonyPatch(typeof(Fiddle), "get_CanonicalVars")]
internal static class FiddleVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(AncientCardHelpers.FiddleHandLimit) };
        return false;
    }
}

[HarmonyPatch(typeof(Fiddle), nameof(Fiddle.ModifyHandDrawLate))]
internal static class FiddleHandDrawPatch
{
    [HarmonyPrefix]
    private static bool Prefix(Fiddle __instance, Player player, ref decimal __result)
    {
        if (player != __instance.Owner)
        {
            return true;
        }

        var handCount = PileType.Hand.GetPile(player).Cards.Count;
        __result = Math.Max(0, AncientCardHelpers.FiddleHandLimit - handCount);
        return false;
    }
}

[HarmonyPatch(typeof(Fiddle), nameof(Fiddle.ShouldDraw))]
internal static class FiddleShouldDrawPatch
{
    [HarmonyPrefix]
    private static bool Prefix(Fiddle __instance, Player player, ref bool __result)
    {
        if (player != __instance.Owner)
        {
            return true;
        }

        __result = true;
        return false;
    }
}

[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Draw), typeof(PlayerChoiceContext), typeof(decimal), typeof(Player), typeof(bool))]
internal static class FiddleDrawCapPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref decimal count, Player player, bool fromHandDraw, ref Task<IEnumerable<CardModel>> __result)
    {
        if (fromHandDraw || player.GetRelic<Fiddle>() == null)
        {
            return true;
        }

        var combatState = player.Creature.CombatState;
        if (combatState == null || combatState.CurrentSide != player.Creature.Side)
        {
            return true;
        }

        var remainingRoom = AncientCardHelpers.FiddleHandLimit - PileType.Hand.GetPile(player).Cards.Count;
        if (remainingRoom <= 0)
        {
            __result = Task.FromResult<IEnumerable<CardModel>>(Array.Empty<CardModel>());
            MainFile.Logger.Info("[EZMicroBalance] Fiddle applied: prevented draw above 7-card player-turn hand cap.");
            return false;
        }

        count = Math.Min(count, remainingRoom);
        return true;
    }
}

[HarmonyPatch(typeof(IronClub), "get_CanonicalVars")]
internal static class IronClubVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(5) };
        return false;
    }
}

[HarmonyPatch(typeof(BrilliantScarf), "get_CanonicalVars")]
internal static class BrilliantScarfVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(6) };
        return false;
    }
}

[HarmonyPatch(typeof(VelvetChoker), "get_CanonicalVars")]
internal static class VelvetChokerVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(7), new EnergyVar(1) };
        return false;
    }
}

[HarmonyPatch(typeof(VelvetChoker), "get_DisplayAmount")]
internal static class VelvetChokerDisplayAmountPatch
{
    [HarmonyPrefix]
    private static bool Prefix(VelvetChoker __instance, ref int __result)
    {
        __result = VelvetChokerSoftLimitTracker.HandPlayedThisTurn(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.ShouldPlay))]
internal static class VelvetChokerShouldPlayPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}

[HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.GetWithModifiers))]
internal static class VelvetChokerEnergyCostPatch
{
    [HarmonyPostfix]
    private static void AddSoftLimitCost(CardEnergyCost __instance, CostModifiers modifiers, ref int __result)
    {
        if (!modifiers.HasFlag(CostModifiers.Global) || __result < 0)
        {
            return;
        }

        var card = VelvetChokerSoftLimitTracker.GetCard(__instance);
        if (card is { EnergyCost.CostsX: false } && VelvetChokerSoftLimitTracker.ShouldTax(card))
        {
            __result += VelvetChokerSoftLimitTracker.ExtraEnergyCost;
        }
    }
}

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
internal static class VelvetChokerXCostCanPlayPatch
{
    [HarmonyPostfix]
    private static void RequireExtraEnergyForTaxedXCost(PlayerCombatState __instance, CardModel card, ref UnplayableReason reason, ref bool __result)
    {
        if (!card.EnergyCost.CostsX ||
            !VelvetChokerSoftLimitTracker.ShouldTax(card) ||
            __instance.Energy >= VelvetChokerSoftLimitTracker.ExtraEnergyCost)
        {
            return;
        }

        reason |= UnplayableReason.EnergyCostTooHigh;
        __result = false;
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
internal static class VelvetChokerXCostSpendPatch
{
    [HarmonyPostfix]
    private static void ReduceCapturedXBySoftLimitTax(CardModel __instance, ref Task<ValueTuple<int, int>> __result)
    {
        if (__instance.EnergyCost.CostsX && VelvetChokerSoftLimitTracker.ShouldTax(__instance))
        {
            __result = ReduceCapturedXBySoftLimitTax(__instance, __result);
        }
    }

    private static async Task<ValueTuple<int, int>> ReduceCapturedXBySoftLimitTax(
        CardModel card,
        Task<ValueTuple<int, int>> originalSpend)
    {
        var result = await originalSpend;
        card.EnergyCost.CapturedXValue = Math.Max(0, result.Item1 - VelvetChokerSoftLimitTracker.ExtraEnergyCost);
        return result;
    }
}

[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterCardPlayed))]
internal static class VelvetChokerAfterCardPlayedPatch
{
    [HarmonyPrefix]
    private static bool Prefix(VelvetChoker __instance, CardPlay cardPlay, ref Task __result)
    {
        if (!cardPlay.IsAutoPlay &&
            cardPlay.IsFirstInSeries &&
            !cardPlay.Card.IsClone &&
            cardPlay.Card.Owner == __instance.Owner)
        {
            VelvetChokerSoftLimitTracker.Increment(__instance);
        }

        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.BeforeSideTurnStart))]
internal static class VelvetChokerTurnResetPatch
{
    [HarmonyPrefix]
    private static bool Prefix(VelvetChoker __instance, CombatSide side, ref Task __result)
    {
        if (side == __instance.Owner.Creature.Side)
        {
            VelvetChokerSoftLimitTracker.Reset(__instance);
        }

        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterRoomEntered))]
internal static class VelvetChokerRoomResetPatch
{
    [HarmonyPrefix]
    private static bool Prefix(VelvetChoker __instance, AbstractRoom room, ref Task __result)
    {
        if (room is CombatRoom)
        {
            VelvetChokerSoftLimitTracker.Reset(__instance);
        }

        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterCombatEnd))]
internal static class VelvetChokerCombatResetPatch
{
    [HarmonyPrefix]
    private static bool Prefix(VelvetChoker __instance, ref Task __result)
    {
        VelvetChokerSoftLimitTracker.Reset(__instance);
        __result = Task.CompletedTask;
        return false;
    }
}

internal static class VelvetChokerSoftLimitTracker
{
    public const int FreeHandPlaysPerTurn = 6;

    public const int ExtraEnergyCost = 1;

    private sealed class State
    {
        public int HandPlayedThisTurn { get; set; }
    }

    private static readonly ConditionalWeakTable<VelvetChoker, State> States = new();

    private static readonly HashSet<CardModel> SuppressedCostCards = [];

    private static readonly System.Reflection.FieldInfo EnergyCostCardField =
        AccessTools.Field(typeof(CardEnergyCost), "_card");

    private static readonly System.Reflection.MethodInfo InvokeDisplayAmountChangedMethod =
        AccessTools.Method(typeof(RelicModel), "InvokeDisplayAmountChanged");

    public static CardModel? GetCard(CardEnergyCost energyCost)
    {
        return EnergyCostCardField.GetValue(energyCost) as CardModel;
    }

    public static int HandPlayedThisTurn(VelvetChoker choker)
    {
        return States.GetOrCreateValue(choker).HandPlayedThisTurn;
    }

    public static bool ShouldTax(CardModel card)
    {
        if (!CombatManager.Instance.IsInProgress ||
            SuppressedCostCards.Contains(card) ||
            card.IsClone ||
            card.Pile?.Type != PileType.Hand)
        {
            return false;
        }

        var owner = TryGetOwner(card);
        var choker = owner?.GetRelic<VelvetChoker>();
        return choker is { IsMelted: false } &&
            HandPlayedThisTurn(choker) >= FreeHandPlaysPerTurn;
    }

    private static Player? TryGetOwner(CardModel card)
    {
        try
        {
            return card.Owner;
        }
        catch (MegaCrit.Sts2.Core.Models.Exceptions.CanonicalModelException)
        {
            return null;
        }
    }

    public static void Increment(VelvetChoker choker)
    {
        States.GetOrCreateValue(choker).HandPlayedThisTurn++;
        InvokeDisplayAmountChanged(choker);
    }

    public static void Reset(VelvetChoker choker)
    {
        States.GetOrCreateValue(choker).HandPlayedThisTurn = 0;
        InvokeDisplayAmountChanged(choker);
    }

    public static T SuppressCostFor<T>(CardModel card, Func<T> action)
    {
        SuppressedCostCards.Add(card);
        try
        {
            return action();
        }
        finally
        {
            SuppressedCostCards.Remove(card);
        }
    }

    public static async Task<T> SuppressCostFor<T>(CardModel card, Func<Task<T>> action)
    {
        SuppressedCostCards.Add(card);
        try
        {
            return await action();
        }
        finally
        {
            SuppressedCostCards.Remove(card);
        }
    }

    private static void InvokeDisplayAmountChanged(VelvetChoker choker)
    {
        InvokeDisplayAmountChangedMethod.Invoke(choker, Array.Empty<object>());
    }
}

[HarmonyPatch(typeof(BeautifulBracelet), "get_CanonicalVars")]
internal static class BeautifulBraceletVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(3), new DynamicVar("Swift", 2m) };
        return false;
    }
}

[HarmonyPatch(typeof(BeautifulBracelet), nameof(BeautifulBracelet.AfterObtained))]
internal static class BeautifulBraceletPatch
{
    [HarmonyPrefix]
    private static bool Prefix(BeautifulBracelet __instance, ref Task __result)
    {
        __result = AddSwiftTwo(__instance);
        return false;
    }

    private static async Task AddSwiftTwo(BeautifulBracelet bracelet)
    {
        var swift = ModelDb.Enchantment<Swift>();
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, bracelet.DynamicVars.Cards.IntValue);
        var cards = (await CardSelectCmd.FromDeckForEnchantment(bracelet.Owner, swift, 2, prefs)).ToList();
        foreach (var card in cards)
        {
            CardCmd.Enchant<Swift>(card, 2m);
        }

        MainFile.Logger.Info($"[EZMicroBalance] BeautifulBracelet applied: enchanted {cards.Count} card(s) with Swift 2.");
    }
}

[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.BeforeCardPlayed))]
internal static class MusicBoxBeforeCardPlayedPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }
}

internal static class MusicBoxStateTracker
{
    private sealed class State
    {
        public bool WasUsedThisTurn { get; set; }
    }

    private static readonly ConditionalWeakTable<MusicBox, State> States = new();

    public static bool WasUsedThisTurn(MusicBox musicBox)
    {
        return States.GetOrCreateValue(musicBox).WasUsedThisTurn;
    }

    public static void MarkUsed(MusicBox musicBox)
    {
        States.GetOrCreateValue(musicBox).WasUsedThisTurn = true;
    }

    public static void Reset(MusicBox musicBox)
    {
        States.GetOrCreateValue(musicBox).WasUsedThisTurn = false;
    }
}

[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.AfterCardPlayed))]
internal static class MusicBoxAfterCardPlayedPatch
{
    [HarmonyPrefix]
    private static bool Prefix(MusicBox __instance, CardPlay cardPlay, ref Task __result)
    {
        __result = AfterCardPlayed(__instance, cardPlay);
        return false;
    }

    private static async Task AfterCardPlayed(MusicBox musicBox, CardPlay cardPlay)
    {
        if (MusicBoxStateTracker.WasUsedThisTurn(musicBox) ||
            cardPlay.Card.Owner != musicBox.Owner ||
            cardPlay.Card.Type != CardType.Attack)
        {
            return;
        }

        musicBox.Flash();
        var copy = cardPlay.Card.CreateClone();
        AncientCardHelpers.ApplyTemporaryCostReduction(copy, 1);
        AncientCardHelpers.ApplyKeywords(copy, CardKeyword.Ethereal, CardKeyword.Exhaust);
        var addResult = await AncientCardHelpers.TryAddGeneratedCardToCombat(copy, PileType.Hand, musicBox.Owner);
        if (addResult is not { success: true })
        {
            MainFile.Logger.Warn("[EZMicroBalance] MusicBox skipped: generated attack copy could not be added to combat.");
            return;
        }

        MusicBoxStateTracker.MarkUsed(musicBox);
        MainFile.Logger.Info("[EZMicroBalance] MusicBox applied: created attack copy with -1 cost, Ethereal, and Exhaust.");
    }
}

[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.BeforeSideTurnStart))]
internal static class MusicBoxTurnResetPatch
{
    [HarmonyPostfix]
    private static void ResetOnTurnStart(MusicBox __instance, CombatSide side)
    {
        if (side == __instance.Owner.Creature.Side)
        {
            MusicBoxStateTracker.Reset(__instance);
        }
    }
}

[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.AfterCombatEnd))]
internal static class MusicBoxCombatResetPatch
{
    [HarmonyPostfix]
    private static void ResetAfterCombat(MusicBox __instance)
    {
        MusicBoxStateTracker.Reset(__instance);
    }
}

