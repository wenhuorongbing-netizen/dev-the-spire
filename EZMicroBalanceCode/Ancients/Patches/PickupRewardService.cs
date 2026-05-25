namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static class PickupRewardService
{
    public static async Task UpgradeTwoCardsOnWarHammerPickup(WarHammer warHammer)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 2);
        var cards = (await CardSelectCmd.FromDeckForUpgrade(warHammer.Owner, prefs)).ToList();
        CardCmd.Upgrade(cards, CardPreviewStyle.HorizontalLayout);
        MainFile.Logger.Info($"[Spire Plus] WarHammer applied: upgraded {cards.Count} card(s) on pickup.");
    }

    public static async Task FillPotionSlotsForSozu(Sozu sozu)
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

        MainFile.Logger.Info($"[Spire Plus] Sozu applied: filled {generated.Count} potion slot(s) on pickup.");
    }

    public static async Task GainInitialGoldForEctoplasm(Ectoplasm ectoplasm)
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

        MainFile.Logger.Info("[Spire Plus] Ectoplasm applied: gained 250 initial gold.");
    }

    public static async Task AddDebtsForSealOfGold(SealOfGold sealOfGold)
    {
        var results = new List<CardPileAddResult>();
        for (var i = 0; i < 2; i++)
        {
            var debt = sealOfGold.Owner.RunState.CreateCard<Debt>(sealOfGold.Owner);
            DebtCardPatch.ConfigureDebt(debt);
            results.Add(await CardPileCmd.Add(debt, PileType.Deck));
        }

        SpirePlusFeedback.PreviewDeckAdds(results, sealOfGold, 2f);
        MainFile.Logger.Info("[Spire Plus] SealOfGold applied: added 2 Debt cards on pickup.");
    }

    public static async Task ChoosePermanentFreePower(JeweledMask jeweledMask)
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
            MainFile.Logger.Warn("[Spire Plus] JeweledMask skipped: no eligible unenchanted deck or generated power target.");
            return;
        }

        CardCmd.Enchant<JeweledMaskFreePower>(selected, 1m);
        MainFile.Logger.Info($"[Spire Plus] JeweledMask applied: marked {selected.Id.Entry} as permanent 0-cost combat-start power.");
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
