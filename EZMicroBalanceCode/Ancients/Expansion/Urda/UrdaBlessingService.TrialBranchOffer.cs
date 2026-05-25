using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private const int TrialBranchOfferCount = 4;

    public static async Task ApplyTrialBranch(Player player)
    {
        var offers = CreateTrialBranchOffers(player);
        if (offers.Count == 0)
        {
            MainFile.Logger.Warn("[Spire Plus] Urda Trial Branch could not create source-safe card offers.");
            return;
        }

        var selected = (await CardSelectCmd.FromSimpleGrid(
            new BlockingPlayerChoiceContext(),
            offers,
            player,
            new CardSelectorPrefs(UrdaLoc("urda_trial_branch.selectionScreenPrompt"), 1)
            {
                RequireManualConfirmation = true
            })).FirstOrDefault();

        foreach (var offer in offers.Where(offer => offer != selected))
        {
            AncientCardHelpers.RemoveUnpiledRunCard(offer);
        }

        if (selected == null)
        {
            return;
        }

        if (selected.IsUpgradable)
        {
            CardCmd.Upgrade(selected, CardPreviewStyle.None);
        }

        var addResult = await CardPileCmd.Add(selected, PileType.Deck);
        if (addResult.success)
        {
            AncientSavedStateFields.UrdaTrialPlantCard[addResult.cardAdded] = true;
            SetProgress(player, GetProgress(player) with
            {
                TrialCombats = 0,
                TrialSuccessfulCombats = 0,
                TrialPlayedThisCombat = false,
                TrialSettled = false
            });
            CardCmd.Enchant<UrdaTrialBranchEnchantment>(addResult.cardAdded, 1m);
            RefreshTrialBranchEnchantment(player);
            SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<UrdaTrialBranchOptionRelic>(), 2f);
            MainFile.Logger.Info($"[Spire Plus] Urda Trial Branch added upgraded rare card {selected.Id.Entry}.");
        }
        else
        {
            AncientCardHelpers.RemoveUnpiledRunCard(selected);
        }
    }

    private static List<CardModel> CreateTrialBranchOffers(Player player)
    {
        var pool = player.Character.CardPool
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(card =>
                card.Rarity == CardRarity.Rare &&
                card.Type is not (CardType.Status or CardType.Curse or CardType.Quest) &&
                card.CanBeGeneratedByModifiers)
            .ToList();
        if (pool.Count == 0)
        {
            return [];
        }

        var options = new CardCreationOptions(pool, CardCreationSource.Other, CardRarityOddsType.Uniform)
            .WithFlags(CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications | CardCreationFlags.NoUpgradeRoll);
        return CardFactory.CreateForReward(player, TrialBranchOfferCount, options)
            .Select(result => result.Card)
            .ToList();
    }
}
