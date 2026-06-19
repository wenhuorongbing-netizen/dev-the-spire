using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class MorviV22GuardTests
{
    [Fact]
    public void MorviSourceConstantsAndStatefulBlessingsMatchV22Numbers()
    {
        var runHook = ReadMorviSource();
        var forbiddenLoanBorrowedCardState = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.ForbiddenLoanBorrowedCardState.cs");

        AssertSourceContains(
            runHook,
            "ForbiddenLoanKeepGoldCost = 180",
            "ForbiddenLoanAttackSkillHpLoss = 1",
            "ForbiddenLoanPowerHpLoss = 8",
            "HasForbiddenLoanCandidates(Player player)",
            "TrySelectForbiddenLoanCard(player)",
            "if (forbiddenLoanProgress == null)",
            "return false",
            "player.Character.CardPool",
            "card.Rarity == CardRarity.Ancient",
            "CardSelectCmd.FromChooseACardScreen",
            "var addResult = await CardPileCmd.Add(selected, PileType.Deck)",
            "if (!addResult.success)",
            "MarkBorrowedAncientCard(borrowedCard)",
            "player.RunState.CurrentActIndex == 1",
            "AutoSettleForbiddenLoan");
        AssertSourceContains(
            forbiddenLoanBorrowedCardState,
            "private static void MarkBorrowedAncientCard(CardModel card) =>",
            "AncientSavedStateFields.MorviBorrowedAncientCard[card] = true");

        AssertSourceContains(
            runHook,
            "RedInkOverdraftDraw = 2",
            "RedInkOverdraftEnergy = 1",
            "RedInkOverdraftGoldPerDebt = 12",
            "RedInkOverdraftHpPerUnpaidDebt = 3",
            "MorviRedInkOverdraftCard",
            "CanUseRedInkOverdraft",
            "player.PlayerCombatState?.Energy != 0",
            "combatState.RedInkUsedThisTurn",
            "hand.Cards.Count >= CardPile.MaxCardsInHand",
            "card.Pile?.Type != PileType.Hand",
            "CardPileCmd.RemoveFromCombat(result.cardAdded, skipVisuals: true)",
            "visibleDebtCount = player.Creature.GetPower<MorviOverdraftPower>()?.Amount ?? 0",
            "debtCount = Math.Max(combatState.RedInkDebtsThisCombat, visibleDebtCount)",
            "DamagePlayerNonlethal(player, RedInkOverdraftHpPerUnpaidDebt)");

        AssertSourceContains(
            runHook,
            "OpenBookDraw = 5",
            "OpenBookEnergy = 2",
            "OpenBookSealTurn = 1",
            "OpenBookReturnTurn = 3",
            "CardPileCmd.Draw(choiceContext, OpenBookDraw, player)",
            "CardPileCmd.Add(card, PileType.Exhaust)",
            "AncientSavedStateFields.MorviOpenBookSealedCard[addResult.cardAdded] = true",
            "FindOpenBookSealedCards(player, combatState)",
            "addResult.cardAdded.SetToFreeThisTurn()");

        AssertSourceContains(
            runHook,
            "PaperstormWastePaperCount = 4",
            "PaperstormStatusTriggersPerTurn = 2",
            "AncientCardHelpers.TryAddGeneratedCardToCombat(waste, PileType.Draw, player, CardPilePosition.Random)",
            "card.Type != CardType.Status",
            "player.Creature.GetPower<MorviPaperstormPower>() is { Amount: > 0 } paperstormPower",
            "card.Pile?.Type != PileType.Hand",
            "CardCmd.Exhaust(choiceContext, card, skipVisuals: true)",
            "PlayerCmd.GainEnergy(1m, player)");

        AssertSourceContains(
            runHook,
            "BlueprintProofStacks = 3",
            "BlueprintProofCostReduction = 1",
            "BlueprintProofBlock = 4",
            "CardCmd.Upgrade(card, CardPreviewStyle.None)",
            "CardCmd.Downgrade(card)",
            "combatState.BlueprintDrawAfterCards.Add(card)",
            "combatState.BlueprintBlockAfterCards.Add(card)");

        AssertSourceContains(
            runHook,
            "DebtSettlementImmediateGold = 220",
            "DebtSettlementStartingDebt = 320",
            "DebtSettlementCombatDue = 40",
            "DebtSettlementHpPerTenShortfall = 3",
            "CardSelectCmd.FromDeckForRemoval",
            "CardSelectCmd.FromDeckForUpgrade",
            "Math.Ceiling(shortfall / 10m)",
            "DamagePlayerNonlethal(player, calculatedHpLoss)",
            "maximumNonlethalHpLoss = Math.Max(0m, player.Creature.CurrentHp - 1m)",
            "hpLoss = Math.Min(calculatedHpLoss, maximumNonlethalHpLoss)",
            "DebtRemaining = Math.Max(0, progress.DebtRemaining - due)");
    }

    [Fact]
    public void MorviPaymentSplitKeepsPublicApiBoundarySmall()
    {
        var forbiddenLoan = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.ForbiddenLoan.cs");
        var forbiddenLoanBorrowedCards = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.ForbiddenLoanBorrowedCards.cs");
        var forbiddenLoanBorrowedCardState = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.ForbiddenLoanBorrowedCardState.cs");
        var redInk = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.RedInkOverdraft.cs");
        var openBook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.OpenBook.cs");
        var openBookState = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.OpenBookState.cs");
        var debtSettlement = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.DebtSettlement.cs");
        var payments = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.Payments.cs");
        var ancient = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi");
        var cards = ReadMorviSource();
        var combatState = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.CombatState.cs");
        var state = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.State.cs");
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviRunHook.cs");

        AssertSourceContains(
            forbiddenLoan,
            "internal static bool HasForbiddenLoanCandidates(Player player)",
            "private static async Task<Progress?> TrySelectForbiddenLoanCard");
        AssertSourceContains(
            forbiddenLoanBorrowedCards,
            "private static async Task ResolveBorrowedAncientPlayCost",
            "private static async Task AutoSettleForbiddenLoan",
            "ClearBorrowedAncientCardMarker(borrowed)");
        AssertSourceContains(
            forbiddenLoanBorrowedCardState,
            "private static void ClearBorrowedAncientCards",
            "private static bool IsBorrowedAncientDeckCard",
            "private static bool IsBorrowedAncientCombatCard",
            "card.DeckVersion is { } deckCard",
            "AncientSavedStateFields.MorviBorrowedAncientCard[");
        AssertSourceContains(
            redInk,
            "public static bool CanUseRedInkOverdraft(Player player)",
            "public static async Task UseRedInkOverdraft(PlayerChoiceContext choiceContext, Player player)",
            "private static async Task AddRedInkOverdraftCard",
            "private static async Task PayRedInkOverdraftDebts");
        AssertSourceContains(
            openBook,
            "private static async Task ResolveOpenBookTurnStart",
            "private static async Task TrySealOpenBookAtTurnEnd",
            "private static async Task SealOpenBookCards",
            "private static async Task ReturnOpenBookCards");
        AssertSourceContains(
            openBookState,
            "private static List<CardModel> FindOpenBookSealedCards",
            "private static void ClearOpenBookMarkers",
            "ReleaseEvidenceLog.Log(");
        AssertSourceContains(
            debtSettlement,
            "private static async Task ResolveDebtSettlementPickup",
            "private static async Task PayDebtSettlementDue");
        AssertSourceContains(
            payments,
            "private static async Task DamagePlayerNonlethal");
        AssertSourceContains(
            cards,
            "MorviBlessingService.CanUseRedInkOverdraft(Owner)",
            "MorviBlessingService.UseRedInkOverdraft(choiceContext, Owner)");
        Assert.Contains("MorviBlessingService.HasForbiddenLoanCandidates(Owner)", ancient, StringComparison.Ordinal);
        AssertSourceContains(
            state,
            "private const char ProgressSeparator = ';'",
            "TrySelectForbiddenLoanCard(player)",
            "ResolveDebtSettlementPickup(player)");
        AssertSourceContains(
            combatState,
            "private sealed class MorviCombatState",
            "private static readonly ConditionalWeakTable<Player, MorviCombatState> CombatStates = new();",
            "public HashSet<CardModel> OpenBookDrawnCards { get; } = []",
            "public HashSet<CardModel> BlueprintBlockAfterCards { get; } = []");
        Assert.DoesNotContain("private sealed class MorviCombatState", state, StringComparison.Ordinal);
        AssertSourceContains(
            forbiddenLoanBorrowedCards,
            "private const int ForbiddenLoanKeepGoldCost = 180",
            "private const int ForbiddenLoanAttackSkillHpLoss = 1",
            "private const int ForbiddenLoanPowerHpLoss = 8");
        AssertSourceContains(
            redInk,
            "private const int RedInkOverdraftDraw = 2",
            "private const int RedInkOverdraftEnergy = 1",
            "private const int RedInkOverdraftGoldPerDebt = 12",
            "private const int RedInkOverdraftHpPerUnpaidDebt = 3");
        AssertSourceContains(
            openBook,
            "private const int OpenBookDraw = 5",
            "private const int OpenBookEnergy = 2",
            "private const int OpenBookSealTurn = 1",
            "private const int OpenBookReturnTurn = 3");
        AssertSourceContains(
            debtSettlement,
            "private const int DebtSettlementImmediateGold = 220",
            "private const int DebtSettlementStartingDebt = 320",
            "private const int DebtSettlementCombatDue = 40",
            "private const int DebtSettlementHpPerTenShortfall = 3");
        Assert.DoesNotContain("ForbiddenLoanKeepGoldCost", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("RedInkOverdraftDraw", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("DebtSettlementImmediateGold", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("ProgressSeparator", runHook, StringComparison.Ordinal);

        Assert.DoesNotContain("public static async Task ResolveDebtSettlementPickup", debtSettlement, StringComparison.Ordinal);
        Assert.DoesNotContain("public static async Task PayDebtSettlementDue", debtSettlement, StringComparison.Ordinal);
        Assert.DoesNotContain("public static async Task AutoSettleForbiddenLoan", forbiddenLoanBorrowedCards, StringComparison.Ordinal);
        Assert.DoesNotContain("public static async Task DamagePlayerNonlethal", payments, StringComparison.Ordinal);
        Assert.DoesNotContain("ReleaseEvidenceLog.Log(", openBook, StringComparison.Ordinal);
        Assert.DoesNotContain("AncientSavedStateFields.MorviBorrowedAncientCard[", forbiddenLoan, StringComparison.Ordinal);
        Assert.DoesNotContain("AncientSavedStateFields.MorviBorrowedAncientCard[", forbiddenLoanBorrowedCards, StringComparison.Ordinal);
    }
}
