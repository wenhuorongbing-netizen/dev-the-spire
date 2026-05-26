using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    internal static bool HasForbiddenLoanCandidates(Player player) =>
        GetForbiddenLoanCandidates(player).Any();

    private static IReadOnlyList<CardModel> GetForbiddenLoanCandidates(Player player) =>
        player.Character.CardPool
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(IsForbiddenLoanCandidate)
            .DistinctBy(card => card.Id)
            .ToList();

    private static async Task<Progress?> TrySelectForbiddenLoanCard(Player player)
    {
        var ancientPool = GetForbiddenLoanCandidates(player);

        if (ancientPool.Count == 0)
        {
            MainFile.Logger.Warn("[Spire Plus] Morvi Forbidden Loan skipped: no unlocked class Ancient cards were found.");
            return null;
        }

        var offered = ancientPool
            .TakeRandom(Math.Min(3, ancientPool.Count), player.PlayerRng.Rewards)
            .Select(card => player.RunState.CreateCard(card, player))
            .ToList();

        foreach (var card in offered.Where(card => card.IsUpgradable))
        {
            CardCmd.Upgrade(card, CardPreviewStyle.None);
        }

        var selected = await CardSelectCmd.FromChooseACardScreen(
            new BlockingPlayerChoiceContext(),
            offered,
            player);

        foreach (var card in offered)
        {
            if (card == selected)
            {
                continue;
            }

            card.RemoveFromState();
        }

        if (selected == null)
        {
            MainFile.Logger.Warn("[Spire Plus] Morvi Forbidden Loan selection returned no card.");
            return null;
        }

        var addResult = await CardPileCmd.Add(selected, PileType.Deck);
        if (!addResult.success)
        {
            AncientCardHelpers.RemoveUnpiledRunCard(selected);
            MainFile.Logger.Warn($"[Spire Plus] Morvi Forbidden Loan failed to add borrowed Ancient card {selected.Id.Entry}; progress was left unchanged.");
            return null;
        }

        var borrowedCard = addResult.cardAdded;
        AncientSavedStateFields.MorviBorrowedAncientCard[borrowedCard] = true;
        SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<MorviForbiddenLoanOptionRelic>(), 2f);
        MainFile.Logger.Info($"[Spire Plus] Morvi Forbidden Loan added upgraded borrowed Ancient card {borrowedCard.Id.Entry}.");
        return Progress.Default with
        {
            BorrowedCardId = borrowedCard.Id.Entry,
            BorrowedSettled = false
        };
    }

    private static bool IsForbiddenLoanCandidate(CardModel card) =>
        card.Rarity == CardRarity.Ancient &&
        card.Type is CardType.Attack or CardType.Skill or CardType.Power &&
        card.CanBeGeneratedByModifiers;
}
