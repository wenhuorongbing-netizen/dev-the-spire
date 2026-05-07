namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(RelicModel), nameof(RelicModel.AfterObtained))]
internal static class AncientPickupBalancePatch
{
    [HarmonyPrefix]
    private static bool Prefix(RelicModel __instance, ref Task __result)
    {
        switch (__instance)
        {
            case WarHammer warHammer:
                __result = UpgradeTwoCardsOnWarHammerPickup(warHammer);
                return false;
            case Sozu sozu:
                __result = FillPotionSlotsForSozu(sozu);
                return false;
            case Ectoplasm ectoplasm:
                __result = GainInitialGoldForEctoplasm(ectoplasm);
                return false;
            case SealOfGold sealOfGold:
                __result = AddDebtsForSealOfGold(sealOfGold);
                return false;
            case Claws claws:
                __result = ChooseCurseAndAddWishes(claws);
                return false;
            case JeweledMask jeweledMask:
                __result = ChoosePermanentFreePower(jeweledMask);
                return false;
            default:
                return true;
        }
    }

    private static async Task UpgradeTwoCardsOnWarHammerPickup(WarHammer warHammer)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 2);
        var cards = (await CardSelectCmd.FromDeckForUpgrade(warHammer.Owner, prefs)).ToList();
        CardCmd.Upgrade(cards, CardPreviewStyle.HorizontalLayout);
        MainFile.Logger.Info($"[EZMicroBalance] WarHammer applied: upgraded {cards.Count} card(s) on pickup.");
    }

    private static async Task FillPotionSlotsForSozu(Sozu sozu)
    {
        var generated = new List<PotionModel>();
        SozuPotionGatePatch.BeginInitialPotionFill(sozu.Owner);
        try
        {
            while (sozu.Owner.HasOpenPotionSlots)
            {
                var potion = PotionFactory.CreateRandomPotionOutOfCombat(
                    sozu.Owner,
                    sozu.Owner.PlayerRng.Rewards,
                    sozu.Owner.Potions.Concat(generated)).ToMutable();
                generated.Add(potion);

                var result = await PotionCmd.TryToProcure(potion, sozu.Owner);
                if (!result.success)
                {
                    break;
                }
            }
        }
        finally
        {
            SozuPotionGatePatch.EndInitialPotionFill(sozu.Owner);
        }

        MainFile.Logger.Info($"[EZMicroBalance] Sozu applied: filled {generated.Count} potion slot(s) on pickup.");
    }

    private static async Task GainInitialGoldForEctoplasm(Ectoplasm ectoplasm)
    {
        EctoplasmGoldGatePatch.BeginInitialGold(ectoplasm.Owner);
        try
        {
            await PlayerCmd.GainGold(250m, ectoplasm.Owner);
        }
        finally
        {
            EctoplasmGoldGatePatch.EndInitialGold(ectoplasm.Owner);
        }

        MainFile.Logger.Info("[EZMicroBalance] Ectoplasm applied: gained 250 initial gold.");
    }

    private static async Task AddDebtsForSealOfGold(SealOfGold sealOfGold)
    {
        var results = new List<CardPileAddResult>();
        for (var i = 0; i < 2; i++)
        {
            var debt = sealOfGold.Owner.RunState.CreateCard<Debt>(sealOfGold.Owner);
            DebtCardPatch.ConfigureDebt(debt);
            results.Add(await CardPileCmd.Add(debt, PileType.Deck));
        }

        CardCmd.PreviewCardPileAdd(results, 2f);
        MainFile.Logger.Info("[EZMicroBalance] SealOfGold applied: added 2 Debt cards on pickup.");
    }

    public static async Task ChooseCurseAndAddWishes(Claws claws)
    {
        var curseDraft = CreateClawsCurseDraft(claws.Owner);
        var selectedCurse = (await CardSelectCmd.FromChooseABundleScreen(
                claws.Owner,
                curseDraft.Select(card => (IReadOnlyList<CardModel>)new[] { card }).ToList()))
            .FirstOrDefault();

        var addedCards = new List<CardPileAddResult>();
        if (selectedCurse != null)
        {
            var addResult = await CardPileCmd.Add(selectedCurse, PileType.Deck);
            addedCards.Add(addResult);
            if (!addResult.success)
            {
                AncientCardHelpers.RemoveUnpiledRunCard(selectedCurse);
            }
        }

        foreach (var unselected in curseDraft.Where(card => card != selectedCurse))
        {
            claws.Owner.RunState.RemoveCard(unselected);
        }

        for (var i = 0; i < 2; i++)
        {
            addedCards.Add(await CardPileCmd.Add(claws.Owner.RunState.CreateCard<Wish>(claws.Owner), PileType.Deck));
        }

        var upgradedWish = claws.Owner.RunState.CreateCard<Wish>(claws.Owner);
        CardCmd.Upgrade(upgradedWish);
        addedCards.Add(await CardPileCmd.Add(upgradedWish, PileType.Deck));

        CardCmd.PreviewCardPileAdd(addedCards, 2f);
        MainFile.Logger.Info($"[EZMicroBalance] Claws applied: added curse {selectedCurse?.Id.Entry ?? "NONE"}, 2 Wish, and 1 upgraded Wish+.");
    }

    private static List<CardModel> CreateClawsCurseDraft(Player owner)
    {
        return new CardModel[]
            {
                ModelDb.Card<BadLuck>(),
                ModelDb.Card<Clumsy>(),
                ModelDb.Card<Decay>(),
                ModelDb.Card<Doubt>(),
                ModelDb.Card<Guilty>(),
                ModelDb.Card<Injury>(),
                ModelDb.Card<Normality>(),
                ModelDb.Card<Regret>(),
                ModelDb.Card<Shame>(),
                ModelDb.Card<Writhe>()
            }
            .ToList()
            .StableShuffle(owner.PlayerRng.Rewards)
            .Take(4)
            .Select(canonical => owner.RunState.CreateCard(canonical, owner))
            .ToList();
    }

    private static async Task ChoosePermanentFreePower(JeweledMask jeweledMask)
    {
        var owner = jeweledMask.Owner;
        var deckSelectionPrefs = new CardSelectorPrefs(new LocString("relics", "JEWELED_MASK.ezSelectionScreenPrompt"), 0, 1)
        {
            Cancelable = true,
            RequireManualConfirmation = true
        };
        var selected = (await CardSelectCmd.FromDeckGeneric(
                owner,
                deckSelectionPrefs,
                card => card.Type == CardType.Power && card.Enchantment == null))
            .FirstOrDefault();

        if (selected == null)
        {
            selected = await DraftGeneratedPowerForJeweledMask(owner);
        }

        if (selected == null)
        {
            MainFile.Logger.Warn("[EZMicroBalance] JeweledMask skipped: no eligible unenchanted deck or generated power target.");
            return;
        }

        CardCmd.Enchant<JeweledMaskFreePower>(selected, 1m);
        MainFile.Logger.Info($"[EZMicroBalance] JeweledMask applied: marked {selected.Id.Entry} as permanent 0-cost combat-start power.");
    }

    private static async Task<CardModel?> DraftGeneratedPowerForJeweledMask(Player owner)
    {
        var pool = owner.Character.CardPool
            .GetUnlockedCards(owner.UnlockState, owner.RunState.CardMultiplayerConstraint)
            .Where(card => card.Type == CardType.Power && card.CanBeGeneratedByModifiers)
            .ToList()
            .StableShuffle(owner.PlayerRng.Rewards)
            .Take(3)
            .Select(canonical => owner.RunState.CreateCard(canonical, owner))
            .ToList();

        if (pool.Count == 0)
        {
            return null;
        }

        var selected = (await CardSelectCmd.FromChooseABundleScreen(
                owner,
                pool.Select(card => (IReadOnlyList<CardModel>)new[] { card }).ToList()))
            .FirstOrDefault();

        foreach (var unselected in pool.Where(card => card != selected))
        {
            owner.RunState.RemoveCard(unselected);
        }

        if (selected != null)
        {
            var addResult = await CardPileCmd.Add(selected, PileType.Deck);
            if (!addResult.success)
            {
                AncientCardHelpers.RemoveUnpiledRunCard(selected);
                return null;
            }

            selected = addResult.cardAdded;
        }

        return selected;
    }
}

[HarmonyPatch(typeof(RelicCmd), nameof(RelicCmd.Obtain), typeof(RelicModel), typeof(Player), typeof(int))]
internal static class BlackStarObtainPatch
{
    [HarmonyPostfix]
    private static void Postfix(ref Task<RelicModel> __result)
    {
        __result = GrantActThreeCompensationAfterObtain(__result);
    }

    private static async Task<RelicModel> GrantActThreeCompensationAfterObtain(Task<RelicModel> original)
    {
        var obtained = await original;
        if (obtained is not BlackStar blackStar)
        {
            return obtained;
        }

        if (blackStar.Owner.RunState.CurrentActIndex < 2)
        {
            MainFile.Logger.Info(
                $"[EZMicroBalance] BlackStar skipped: pickup compensation requires act 3+, currentActIndex={blackStar.Owner.RunState.CurrentActIndex}.");
            return obtained;
        }

        var relic = RelicFactory.PullNextRelicFromFront(blackStar.Owner).ToMutable();
        await RelicCmd.Obtain(relic, blackStar.Owner);
        MainFile.Logger.Info($"[EZMicroBalance] BlackStar applied: act 3+ immediate relic {relic.Id.Entry}.");
        return obtained;
    }
}

[HarmonyPatch(typeof(Claws), nameof(Claws.AfterObtained))]
internal static class ClawsAfterObtainedPatch
{
    [HarmonyPrefix]
    private static bool Prefix(Claws __instance, ref Task __result)
    {
        __result = AncientPickupBalancePatch.ChooseCurseAndAddWishes(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Sozu), nameof(Sozu.ShouldProcurePotion))]
internal static class SozuPotionGatePatch
{
    private static readonly HashSet<Player> InitialPotionFillOwners = [];

    public static void BeginInitialPotionFill(Player player)
    {
        InitialPotionFillOwners.Add(player);
    }

    public static void EndInitialPotionFill(Player player)
    {
        InitialPotionFillOwners.Remove(player);
    }

    [HarmonyPrefix]
    private static bool Prefix(Sozu __instance, Player player, ref bool __result)
    {
        if (InitialPotionFillOwners.Contains(player) && player == __instance.Owner)
        {
            __result = true;
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(Ectoplasm), nameof(Ectoplasm.ShouldGainGold))]
internal static class EctoplasmGoldGatePatch
{
    private static readonly HashSet<Player> InitialGoldOwners = [];

    public static void BeginInitialGold(Player player)
    {
        InitialGoldOwners.Add(player);
    }

    public static void EndInitialGold(Player player)
    {
        InitialGoldOwners.Remove(player);
    }

    [HarmonyPrefix]
    private static bool Prefix(Ectoplasm __instance, Player player, ref bool __result)
    {
        if (InitialGoldOwners.Contains(player) && player == __instance.Owner)
        {
            __result = true;
            return false;
        }

        return true;
    }
}

