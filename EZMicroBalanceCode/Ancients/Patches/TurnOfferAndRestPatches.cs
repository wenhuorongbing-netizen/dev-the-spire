namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.BeforeSideTurnStart))]
internal static class CrossbowOfferPatch
{
    [HarmonyPrefix]
    private static bool Prefix(AbstractModel __instance, PlayerChoiceContext choiceContext, CombatSide side, ICombatState combatState, ref Task __result)
    {
        if (__instance is not Crossbow crossbow)
        {
            return true;
        }

        if (side != crossbow.Owner.Creature.Side)
        {
            __result = Task.CompletedTask;
            return false;
        }

        __result = OfferTemporaryAttack(crossbow, choiceContext, combatState);
        return false;
    }

    private static async Task OfferTemporaryAttack(Crossbow crossbow, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        var owner = crossbow.Owner;
        var attackPool = owner.Character.CardPool
            .GetUnlockedCards(owner.UnlockState, owner.RunState.CardMultiplayerConstraint)
            .Where(card => card.Type == CardType.Attack && card.CanBeGeneratedInCombat)
            .ToList();
        var generated = CardFactory.GetDistinctForCombat(owner, attackPool, 1, owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (generated == null)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Crossbow skipped: no eligible attack generated.");
            return;
        }

        AncientCardHelpers.ApplyTemporaryCostReduction(generated, 1);
        AncientCardHelpers.ApplyKeywords(generated, CardKeyword.Ethereal, CardKeyword.Exhaust);
        var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, new[] { generated }, owner, canSkip: true);
        if (selected == generated)
        {
            crossbow.Flash();
            var addResult = await AncientCardHelpers.TryAddGeneratedCardToCombat(generated, PileType.Hand, owner);
            if (addResult is { success: true })
            {
                MainFile.Logger.Info($"[EZMicroBalance] Crossbow applied: accepted temporary attack {generated.Id.Entry}.");
                return;
            }

            MainFile.Logger.Warn($"[EZMicroBalance] Crossbow skipped: accepted temporary attack {generated.Id.Entry} could not be added to combat.");
            return;
        }

        AncientCardHelpers.RemoveUnpiledCombatCard(generated, combatState);
        MainFile.Logger.Info($"[EZMicroBalance] Crossbow applied: skipped temporary attack {generated.Id.Entry}.");
    }
}

[HarmonyPatch(typeof(Crossbow), nameof(Crossbow.AfterSideTurnStart))]
internal static class CrossbowVanillaAfterTurnPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(ToastyMittens), nameof(ToastyMittens.BeforeHandDraw))]
internal static class ToastyMittensPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ToastyMittens __instance, Player player, PlayerChoiceContext choiceContext, ICombatState combatState, ref Task __result)
    {
        if (player != __instance.Owner)
        {
            return true;
        }

        __result = OfferTopCardExhaust(__instance, player, choiceContext, combatState);
        return false;
    }

    private static async Task OfferTopCardExhaust(ToastyMittens mittens, Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        await CardPileCmd.ShuffleIfNecessary(choiceContext, player);
        var cards = PileType.Draw.GetPile(player).Cards;
        var topCard = combatState.RoundNumber == 1
            ? cards.FirstOrDefault(card => !card.Keywords.Contains(CardKeyword.Innate))
            : null;
        topCard ??= cards.FirstOrDefault();

        if (topCard == null)
        {
            MainFile.Logger.Info("[EZMicroBalance] ToastyMittens skipped: no draw-pile card to offer.");
            return;
        }

        var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, new[] { topCard }, player, canSkip: true);
        if (selected != topCard)
        {
            MainFile.Logger.Info($"[EZMicroBalance] ToastyMittens applied: kept top card {topCard.Id.Entry}.");
            return;
        }

        mittens.Flash();
        await CardCmd.Exhaust(choiceContext, topCard);
        await PowerCmd.Apply<StrengthPower>(choiceContext, player.Creature, mittens.DynamicVars.Strength.BaseValue, player.Creature, null);
        MainFile.Logger.Info($"[EZMicroBalance] ToastyMittens applied: exhausted {topCard.Id.Entry} and gained Strength.");
    }
}

[HarmonyPatch(typeof(WhisperingEarring), nameof(WhisperingEarring.AfterAutoPrePlayPhaseEnteredLate))]
internal static class WhisperingEarringPatch
{
    [HarmonyPrefix]
    private static bool Prefix(WhisperingEarring __instance, PlayerChoiceContext choiceContext, Player player, ref Task __result)
    {
        if (player != __instance.Owner)
        {
            return true;
        }

        __result = AutoPlayOneHighestCostCard(__instance, choiceContext, player);
        return false;
    }

    private static async Task AutoPlayOneHighestCostCard(WhisperingEarring earring, PlayerChoiceContext choiceContext, Player player)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        if (combatState.RoundNumber > 3)
        {
            return;
        }

        var card = PileType.Hand.GetPile(player).Cards
            .Select((card, index) => new { Card = card, Index = index })
            .Where(item => VelvetChokerSoftLimitTracker.SuppressCostFor(item.Card, item.Card.CanPlay))
            .OrderByDescending(item => VelvetChokerSoftLimitTracker.SuppressCostFor(item.Card, () => AncientCardHelpers.EffectiveCost(item.Card)))
            .ThenBy(item => item.Index)
            .Select(item => item.Card)
            .FirstOrDefault();
        if (card == null)
        {
            return;
        }

        var target = AncientCardHelpers.GetPreferredTarget(card, combatState, player);
        if (card.TargetType is TargetType.AnyEnemy or TargetType.AnyAlly && !card.CanPlayTargeting(target))
        {
            return;
        }

        earring.Flash();
        await VelvetChokerSoftLimitTracker.SuppressCostFor(card, card.SpendResources);
        await CardCmd.AutoPlay(choiceContext, card, target, AutoPlayType.Default, skipXCapture: true);
        MainFile.Logger.Info($"[EZMicroBalance] WhisperingEarring applied: auto-played {card.Id.Entry} on round {combatState.RoundNumber}.");
    }
}

[HarmonyPatch(typeof(PumpkinCandle), nameof(PumpkinCandle.AfterRoomEntered))]
internal static class PumpkinCandlePatch
{
    private const int ExtinguishedSentinel = -2;

    [HarmonyPrefix]
    private static bool Prefix(PumpkinCandle __instance, ref Task __result)
    {
        if (__instance.ActiveAct >= 0 &&
            __instance.Owner.RunState.CurrentActIndex >= 2 &&
            __instance.ActiveAct != __instance.Owner.RunState.CurrentActIndex)
        {
            __result = ExtinguishAndUpgrade(__instance);
            return false;
        }

        return true;
    }

    private static Task ExtinguishAndUpgrade(PumpkinCandle candle)
    {
        var cards = PileType.Deck.GetPile(candle.Owner).Cards
            .Where(card => card.IsUpgradable)
            .ToList()
            .StableShuffle(candle.Owner.RunState.Rng.Niche)
            .Take(2)
            .ToList();
        if (cards.Count > 0)
        {
            candle.Flash();
            CardCmd.Upgrade(cards, CardPreviewStyle.HorizontalLayout);
        }

        candle.ActiveAct = ExtinguishedSentinel;
        candle.Status = RelicStatus.Disabled;
        MainFile.Logger.Info($"[EZMicroBalance] PumpkinCandle applied: extinguished and upgraded {cards.Count} card(s).");
        return Task.CompletedTask;
    }
}

[HarmonyPatch(typeof(CookRestSiteOption), MethodType.Constructor, typeof(Player))]
internal static class MeatCleaverCookCtorPatch
{
    [HarmonyPostfix]
    private static void Postfix(CookRestSiteOption __instance, Player owner)
    {
        if (owner.GetRelic<MeatCleaver>() != null && !MeatCleaverCookPatch.CanCook(owner))
        {
            __instance.IsEnabled = false;
        }
    }
}

[HarmonyPatch(typeof(CookRestSiteOption), "get_Description")]
internal static class MeatCleaverCookDescriptionPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CookRestSiteOption __instance, ref LocString __result)
    {
        var owner = MeatCleaverCookPatch.GetOwner(__instance);
        if (owner.GetRelic<MeatCleaver>() == null)
        {
            return true;
        }

        __result = new LocString(
            "rest_site_ui",
            __instance.IsEnabled ? "OPTION_COOK.ezDescription" : "OPTION_COOK.ezDescriptionDisabled");
        __result.Add("Cards", MeatCleaverCookPatch.CardsToRemove);
        __result.Add("Hp", MeatCleaverCookPatch.HpToLose);
        return false;
    }
}

[HarmonyPatch(typeof(CookRestSiteOption), nameof(CookRestSiteOption.OnSelect))]
internal static class MeatCleaverCookPatch
{
    public const int CardsToRemove = 2;

    public const int HpToLose = 5;

    private static readonly System.Reflection.MethodInfo OwnerGetter =
        AccessTools.PropertyGetter(typeof(RestSiteOption), "Owner");

    [HarmonyPrefix]
    private static bool Prefix(CookRestSiteOption __instance, ref Task<bool> __result)
    {
        var owner = GetOwner(__instance);
        if (owner.GetRelic<MeatCleaver>() == null)
        {
            return true;
        }

        __result = Cook(owner);
        return false;
    }

    public static Player GetOwner(RestSiteOption option)
    {
        return (Player)OwnerGetter.Invoke(option, Array.Empty<object>())!;
    }

    public static bool CanCook(Player owner)
    {
        return owner.Creature.CurrentHp > HpToLose &&
            PileType.Deck.GetPile(owner).Cards.Count(card => card.IsRemovable) >= CardsToRemove;
    }

    private static async Task<bool> Cook(Player owner)
    {
        if (!CanCook(owner))
        {
            MainFile.Logger.Info("[EZMicroBalance] MeatCleaver skipped: cook unavailable due to HP or removable-card count.");
            return false;
        }

        var prefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, CardsToRemove)
        {
            Cancelable = true,
            RequireManualConfirmation = true
        };
        var cards = (await CardSelectCmd.FromDeckForRemoval(owner, prefs)).ToList();
        if (cards.Count != CardsToRemove)
        {
            return false;
        }

        foreach (var card in cards)
        {
            await CardPileCmd.RemoveFromDeck(card);
        }

        await CreatureCmd.SetCurrentHp(owner.Creature, owner.Creature.CurrentHp - HpToLose);
        MainFile.Logger.Info("[EZMicroBalance] MeatCleaver applied: cooked by removing 2 cards and losing 5 HP.");
        return true;
    }
}

