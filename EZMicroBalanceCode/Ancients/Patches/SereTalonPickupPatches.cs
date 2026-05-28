using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class SereTalonPickupPatches : IPatchMethod
{
    static string IPatchMethod.PatchId => "s-e-r-e-t-a-l-o-n-p-i-c-k-u-p-p-a-t-c-h-e-s";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch SereTalon.AfterObtained";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(SereTalon), nameof(SereTalon.AfterObtained))];
{
    private const int CurseOfferCount = 4;
    private const int CursePickCount = 1;
    private const int NormalWishCount = 2;
    private const int UpgradedWishCount = 1;

    [HarmonyPrefix]
    private static bool ChooseCurseAndAddWishes(SereTalon __instance, ref Task __result)
    {
        __result = ApplyPickup(__instance);
        return false;
    }

    private static async Task ApplyPickup(SereTalon sereTalon)
    {
        var owner = sereTalon.Owner;
        if (owner == null)
        {
            return;
        }

        var offeredCurses = CreateCurseOffer(owner);
        var curseResults = await ChooseAndAddCurse(sereTalon, owner, offeredCurses);
        PreviewAdds(curseResults, sereTalon);
        await Cmd.Wait(0.75f);

        var wishResults = await AddWishes(owner);
        PreviewAdds(wishResults, sereTalon);
        await Cmd.Wait(0.75f);

        MainFile.Logger.Info(
            $"[Spire Plus] Vakuu Sere Talon applied: offered {offeredCurses.Count} curse(s), " +
            $"added {curseResults.Count(result => result.success)} curse, {NormalWishCount} Wish, and {UpgradedWishCount} Wish+.");
    }

    private static List<CardModel> CreateCurseOffer(Player owner)
    {
        var availableCurses = ModelDb.CardPool<CurseCardPool>()
            .GetUnlockedCards(owner.UnlockState, owner.RunState.CardMultiplayerConstraint)
            .Where(card => card.CanBeGeneratedByModifiers)
            .OrderBy(card => card.Id)
            .ToList();

        var offered = new List<CardModel>();
        for (var index = 0; index < Math.Min(CurseOfferCount, availableCurses.Count); index++)
        {
            var curse = owner.RunState.Rng.Niche.NextItem(availableCurses);
            if (curse == null)
            {
                break;
            }

            availableCurses.Remove(curse);
            offered.Add(owner.RunState.CreateCard(curse, owner));
        }

        return offered;
    }

    private static async Task<List<CardPileAddResult>> ChooseAndAddCurse(
        SereTalon sereTalon,
        Player owner,
        List<CardModel> offeredCurses)
    {
        if (offeredCurses.Count == 0)
        {
            MainFile.Logger.Warn("[Spire Plus] Vakuu Sere Talon skipped curse choice: no eligible generated Curses.");
            return [];
        }

        sereTalon.Flash();
        var selected = (await CardSelectCmd.FromSimpleGrid(
                new BlockingPlayerChoiceContext(),
                offeredCurses,
                owner,
                new CardSelectorPrefs(new LocString("relics", "SERE_TALON.selectionScreenPrompt"), CursePickCount)
                {
                    Cancelable = false,
                    RequireManualConfirmation = true
                }))
            .FirstOrDefault();

        foreach (var curse in offeredCurses.Where(curse => curse != selected))
        {
            AncientCardHelpers.RemoveUnpiledRunCard(curse);
        }

        if (selected == null)
        {
            return [];
        }

        var result = await CardPileCmd.Add(selected, PileType.Deck);
        if (!result.success)
        {
            AncientCardHelpers.RemoveUnpiledRunCard(selected);
            MainFile.Logger.Warn($"[Spire Plus] Vakuu Sere Talon failed to add selected curse {selected.Id.Entry}.");
            return [];
        }

        return [result];
    }

    private static async Task<List<CardPileAddResult>> AddWishes(Player owner)
    {
        var results = new List<CardPileAddResult>();
        for (var index = 0; index < NormalWishCount; index++)
        {
            results.Add(await CardPileCmd.Add(owner.RunState.CreateCard<Wish>(owner), PileType.Deck));
        }

        for (var index = 0; index < UpgradedWishCount; index++)
        {
            var wish = owner.RunState.CreateCard<Wish>(owner);
            if (wish.IsUpgradable && !wish.IsUpgraded)
            {
                CardCmd.Upgrade(wish, CardPreviewStyle.None);
            }

            results.Add(await CardPileCmd.Add(wish, PileType.Deck));
        }

        return results;
    }

    private static void PreviewAdds(IEnumerable<CardPileAddResult> results, SereTalon sereTalon)
    {
        var successfulAdds = results.Where(result => result.success).ToList();
        if (successfulAdds.Count > 0)
        {
            SpirePlusFeedback.PreviewDeckAdds(successfulAdds, sereTalon, 2f);
        }
    }
}


