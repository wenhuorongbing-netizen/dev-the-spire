namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(Claws), nameof(Claws.AfterObtained))]
internal static class ClawsAfterObtainedPatch
{
    [HarmonyPrefix]
    private static bool Prefix(Claws __instance, ref Task __result)
    {
        __result = ChooseCurseAndAddWishes(__instance);
        return false;
    }

    private static async Task ChooseCurseAndAddWishes(Claws claws)
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
        MainFile.Logger.Info($"[EZMicroBalance] Claws applied: added curse {selectedCurse?.Id.Entry ?? "NONE"}, 2 Wish, and 1 upgraded Wish.");
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
}
