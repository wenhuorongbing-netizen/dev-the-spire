using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.Entities.Gold;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    private const int ForbiddenLoanKeepGoldCost = 180;
    private const int ForbiddenLoanAttackSkillHpLoss = 1;
    private const int ForbiddenLoanPowerHpLoss = 8;

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
            MainFile.Logger.Info("[Spire Plus] Morvi Forbidden Loan auto-settled after Act 2 boss: paid 180 Gold and kept the borrowed card.");
            return;
        }

        await CardPileCmd.RemoveFromDeck(borrowed);
        SetProgress(player, progress with { BorrowedSettled = true });
        MainFile.Logger.Info("[Spire Plus] Morvi Forbidden Loan auto-settled after Act 2 boss: insufficient Gold, removed the borrowed card.");
    }

    private static void ClearBorrowedAncientCards(Player player)
    {
        foreach (var card in player.Deck.Cards.Where(card => card.Owner == player))
        {
            AncientSavedStateFields.MorviBorrowedAncientCard[card] = false;
        }
    }

    private static bool IsBorrowedAncientDeckCard(CardModel card) =>
        AncientSavedStateFields.MorviBorrowedAncientCard[card];

    private static bool IsBorrowedAncientCombatCard(CardModel card) =>
        card.DeckVersion is { } deckCard
            ? AncientSavedStateFields.MorviBorrowedAncientCard[deckCard]
            : AncientSavedStateFields.MorviBorrowedAncientCard[card];
}
