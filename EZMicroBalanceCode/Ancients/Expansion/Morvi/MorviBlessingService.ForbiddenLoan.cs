using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.Entities.Gold;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    private const int ForbiddenLoanKeepGoldCost = 180;
    private const int ForbiddenLoanAttackSkillHpLoss = 1;
    private const int ForbiddenLoanPowerHpLoss = 8;

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
            MainFile.Logger.Warn("[EZMicroBalance] Morvi Forbidden Loan skipped: no unlocked class Ancient cards were found.");
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
            MainFile.Logger.Warn("[EZMicroBalance] Morvi Forbidden Loan selection returned no card.");
            return null;
        }

        var addResult = await CardPileCmd.Add(selected, PileType.Deck);
        if (!addResult.success)
        {
            AncientCardHelpers.RemoveUnpiledRunCard(selected);
            MainFile.Logger.Warn($"[EZMicroBalance] Morvi Forbidden Loan failed to add borrowed Ancient card {selected.Id.Entry}; progress was left unchanged.");
            return null;
        }

        var borrowedCard = addResult.cardAdded;
        AncientSavedStateFields.MorviBorrowedAncientCard[borrowedCard] = true;
        MainFile.Logger.Info($"[EZMicroBalance] Morvi Forbidden Loan added upgraded borrowed Ancient card {borrowedCard.Id.Entry}.");
        return Progress.Default with
        {
            BorrowedCardId = borrowedCard.Id.Entry,
            BorrowedSettled = false
        };
    }

    private static async Task ResolveBorrowedAncientPlayCost(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        var player = card.Owner;
        if (GetSelectedBlessing(player) != MorviBlessingIds.ForbiddenLoan ||
            !cardPlay.IsFirstInSeries ||
            cardPlay.IsAutoPlay ||
            !IsBorrowedAncientCombatCard(card))
        {
            return;
        }

        var hpLoss = card.Type == CardType.Power
            ? ForbiddenLoanPowerHpLoss
            : card.Type is CardType.Attack or CardType.Skill
                ? ForbiddenLoanAttackSkillHpLoss
                : 0;
        if (hpLoss <= 0)
        {
            return;
        }

        await CreatureCmd.Damage(
            choiceContext,
            player.Creature,
            hpLoss,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            player.Creature,
            card);
    }

    private static async Task AutoSettleForbiddenLoan(Player player)
    {
        var progress = GetProgress(player);
        if (progress.BorrowedSettled)
        {
            return;
        }

        var borrowed = player.Deck.Cards.FirstOrDefault(IsBorrowedAncientDeckCard);
        if (borrowed == null)
        {
            SetProgress(player, progress with { BorrowedSettled = true });
            return;
        }

        if (player.Gold >= ForbiddenLoanKeepGoldCost)
        {
            await PlayerCmd.LoseGold(ForbiddenLoanKeepGoldCost, player, GoldLossType.Spent);
            AncientSavedStateFields.MorviBorrowedAncientCard[borrowed] = false;
            SetProgress(player, progress with { BorrowedSettled = true });
            MainFile.Logger.Info("[EZMicroBalance] Morvi Forbidden Loan auto-settled after Act 2 boss: paid 180 Gold and kept the borrowed card.");
            return;
        }

        await CardPileCmd.RemoveFromDeck(borrowed);
        SetProgress(player, progress with { BorrowedSettled = true });
        MainFile.Logger.Info("[EZMicroBalance] Morvi Forbidden Loan auto-settled after Act 2 boss: insufficient Gold, removed the borrowed card.");
    }

    private static void ClearBorrowedAncientCards(Player player)
    {
        foreach (var card in player.Deck.Cards.Where(card => card.Owner == player))
        {
            AncientSavedStateFields.MorviBorrowedAncientCard[card] = false;
        }
    }

    private static bool IsForbiddenLoanCandidate(CardModel card) =>
        card.Rarity == CardRarity.Ancient &&
        card.Type is CardType.Attack or CardType.Skill or CardType.Power &&
        card.CanBeGeneratedByModifiers;

    private static bool IsBorrowedAncientDeckCard(CardModel card) =>
        AncientSavedStateFields.MorviBorrowedAncientCard[card];

    private static bool IsBorrowedAncientCombatCard(CardModel card) =>
        card.DeckVersion is { } deckCard
            ? AncientSavedStateFields.MorviBorrowedAncientCard[deckCard]
            : AncientSavedStateFields.MorviBorrowedAncientCard[card];
}
