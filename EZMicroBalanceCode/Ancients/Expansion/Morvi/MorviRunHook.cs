using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed class MorviRunHook : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override Task BeforeCombatStart() =>
        MorviBlessingService.BeforeCombatStart();

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        MorviBlessingService.SyncPersistentState(card.Owner);
        return Task.CompletedTask;
    }

    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player) =>
        MorviBlessingService.AfterPlayerTurnStart(choiceContext, player);

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side) =>
        MorviBlessingService.AfterTurnEnd(choiceContext, side);

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType) =>
        MorviBlessingService.ShouldPlay(card, autoPlayType);

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount) =>
        MorviBlessingService.ModifyCardPlayCount(card, playCount);

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost) =>
        MorviBlessingService.TryModifyEnergyCostInCombat(card, originalCost, out modifiedCost);

    public override Task BeforeCardPlayed(CardPlay cardPlay) =>
        MorviBlessingService.BeforeCardPlayed(cardPlay);

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay) =>
        MorviBlessingService.AfterCardPlayed(choiceContext, cardPlay);

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw) =>
        MorviBlessingService.AfterCardDrawn(choiceContext, card);

    public override Task AfterCombatEnd(CombatRoom room) =>
        MorviBlessingService.AfterCombatEnd(room);
}

internal static class MorviBlessingService
{
    private const char ProgressSeparator = ';';
    private const int ForbiddenLoanKeepGoldCost = 180;
    private const int ForbiddenLoanAttackSkillHpLoss = 1;
    private const int ForbiddenLoanPowerHpLoss = 8;
    private const int MisprintExtraPlayCount = 1;
    private const int MisprintDrawCostThreshold = 1;
    private const int RedInkOverdraftDraw = 2;
    private const int RedInkOverdraftEnergy = 1;
    private const int RedInkOverdraftGoldPerDebt = 12;
    private const int RedInkOverdraftHpPerUnpaidDebt = 3;
    private const int OverdueLibraryPageCount = 3;
    private const int OpenBookDraw = 5;
    private const int OpenBookEnergy = 2;
    private const int OpenBookSealTurn = 1;
    private const int OpenBookReturnTurn = 3;
    private const int PaperstormWastePaperCount = 4;
    private const int PaperstormStatusTriggersPerTurn = 2;
    private const int BlueprintProofStacks = 3;
    private const int BlueprintProofCostReduction = 1;
    private const int BlueprintProofBlock = 4;
    private const int DebtSettlementImmediateGold = 220;
    private const int DebtSettlementStartingDebt = 320;
    private const int DebtSettlementCombatDue = 40;
    private const int DebtSettlementHpPerTenShortfall = 3;

    private sealed class MorviCombatState
    {
        public bool MisprintUsedThisTurn { get; set; }

        public HashSet<CardModel> MisprintDrawAfterCards { get; } = [];

        public CardModel? AutoPlayCardPendingModifier { get; set; }

        public bool RedInkUsedThisTurn { get; set; }

        public int RedInkDebtsThisCombat { get; set; }

        public bool OverdueLibraryDiscountArmed { get; set; }

        public CardModel? OverdueLibraryDiscountSourceCard { get; set; }

        public bool OpenBookResolved { get; set; }

        public HashSet<CardModel> OpenBookDrawnCards { get; } = [];

        public List<CardModel> OpenBookSealedCards { get; } = [];

        public int PaperstormTriggersRemainingThisTurn { get; set; }

        public int ProofreadRemaining { get; set; }

        public bool BlueprintProofInitializedThisCombat { get; set; }

        public HashSet<CardModel> BlueprintTemporaryUpgradeCards { get; } = [];

        public HashSet<CardModel> BlueprintDrawAfterCards { get; } = [];

        public HashSet<CardModel> BlueprintBlockAfterCards { get; } = [];
    }

    private sealed record Progress(
        int DebtRemaining,
        string BorrowedCardId,
        bool BorrowedSettled)
    {
        public static Progress Default => new(0, string.Empty, false);
    }

    private static readonly Type[] ArchivePageTypes =
    [
        typeof(MorviArchiveDrawPage),
        typeof(MorviArchiveVeilPage),
        typeof(MorviArchiveBurnPage),
        typeof(MorviArchiveDiscountPage),
        typeof(MorviArchiveBraveryPage),
        typeof(MorviArchiveDexterityPage)
    ];

    private static readonly HashSet<string> TemporaryCardIds =
    [
        MorviArchiveDrawPage.CardId,
        MorviArchiveVeilPage.CardId,
        MorviArchiveBurnPage.CardId,
        MorviArchiveDiscountPage.CardId,
        MorviArchiveBraveryPage.CardId,
        MorviArchiveDexterityPage.CardId,
        MorviRedInkOverdraftCard.CardId,
        MorviWastePaper.CardId
    ];

    private static readonly ConditionalWeakTable<Player, MorviCombatState> CombatStates = new();

    public static async Task SetSelectedBlessing(Player player, string blessingId)
    {
        ClearBorrowedAncientCards(player);
        SetState(player, blessingId, Progress.Default);

        switch (blessingId)
        {
            case MorviBlessingIds.ForbiddenLoan:
                await SelectForbiddenLoanCard(player);
                break;
            case MorviBlessingIds.DebtSettlement:
                await ResolveDebtSettlementPickup(player);
                break;
        }

        SyncPersistentState(player);
    }

    public static string GetSelectedBlessing(Player player)
    {
        var state = AncientPlayerState.Get(
            player,
            AncientSavedStateFields.MorviStateKey,
            AncientSavedStateFields.MorviDeckStateKey);
        var separatorIndex = state.IndexOf(ProgressSeparator);
        return separatorIndex < 0 ? state : state[..separatorIndex];
    }

    public static void SyncPersistentState(Player? player)
    {
        if (player == null)
        {
            return;
        }

        AncientPlayerState.SyncDeck(
            player,
            AncientSavedStateFields.MorviStateKey,
            AncientSavedStateFields.MorviDeckStateKey);
    }

    public static async Task BeforeCombatStart()
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null)
        {
            return;
        }

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            var combatState = CombatStates.GetOrCreateValue(player);
            ResetCombatState(combatState);

            switch (GetSelectedBlessing(player))
            {
                case MorviBlessingIds.OverdueLibrary:
                    await AddArchivePages(player);
                    break;
                case MorviBlessingIds.Paperstorm:
                    combatState.PaperstormTriggersRemainingThisTurn = PaperstormStatusTriggersPerTurn;
                    await AddWastePapers(player);
                    await SetCounterPower<MorviPaperstormPower>(
                        new ThrowingPlayerChoiceContext(),
                        player,
                        PaperstormStatusTriggersPerTurn);
                    break;
                case MorviBlessingIds.BlueprintProof:
                    await EnsureBlueprintProofInitialized(player, combatState, "combat start");
                    break;
                case MorviBlessingIds.DebtSettlement:
                    var progress = GetProgress(player);
                    if (progress.DebtRemaining > 0)
                    {
                        await SetCounterPower<MorviDebtPower>(
                            new ThrowingPlayerChoiceContext(),
                            player,
                            progress.DebtRemaining);
                    }
                    break;
            }
        }
    }

    public static async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (!player.IsActiveForHooks)
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        ResetTurnState(combatState);

        var selectedBlessing = GetSelectedBlessing(player);
        switch (selectedBlessing)
        {
            case MorviBlessingIds.RedInkOverdraft:
                await AddRedInkOverdraftCard(player);
                break;
            case MorviBlessingIds.OpenBookExam:
                if (player.Creature.CombatState?.RoundNumber == OpenBookSealTurn &&
                    !combatState.OpenBookResolved)
                {
                    combatState.OpenBookResolved = true;
                    var drawn = (await CardPileCmd.Draw(choiceContext, OpenBookDraw, player)).ToList();
                    foreach (var card in drawn)
                    {
                        combatState.OpenBookDrawnCards.Add(card);
                    }

                    await PlayerCmd.GainEnergy(OpenBookEnergy, player);
                    await SetCounterPower<MorviOpenBookPower>(choiceContext, player, drawn.Count);
                    MainFile.Logger.Info($"[EZMicroBalance] Morvi Open-Book Exam drew {drawn.Count} cards and granted {OpenBookEnergy} Energy on turn 1.");
                }

                if (player.Creature.CombatState?.RoundNumber == OpenBookReturnTurn &&
                    FindOpenBookSealedCards(player, combatState).Count > 0)
                {
                    await ReturnOpenBookCards(player, combatState);
                }

                break;
            case MorviBlessingIds.Paperstorm:
                combatState.PaperstormTriggersRemainingThisTurn = PaperstormStatusTriggersPerTurn;
                await SetCounterPower<MorviPaperstormPower>(
                    choiceContext,
                    player,
                    PaperstormStatusTriggersPerTurn);
                break;
        }
    }

    public static async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player)
        {
            return;
        }

        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null)
        {
            return;
        }

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            if (GetSelectedBlessing(player) != MorviBlessingIds.OpenBookExam ||
                player.Creature.CombatState?.RoundNumber != OpenBookSealTurn)
            {
                continue;
            }

            await SealOpenBookCards(choiceContext, player, CombatStates.GetOrCreateValue(player));
        }
    }

    public static bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return true;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        if (autoPlayType == AutoPlayType.None)
        {
            if (ReferenceEquals(combatState.AutoPlayCardPendingModifier, card))
            {
                combatState.AutoPlayCardPendingModifier = null;
            }
        }
        else
        {
            combatState.AutoPlayCardPendingModifier = card;
        }

        return true;
    }

    public static int ModifyCardPlayCount(CardModel card, int playCount)
    {
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return playCount;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        if (TryConsumeAutoPlayModifierBlock(card, combatState))
        {
            return playCount;
        }

        if (GetSelectedBlessing(player) != MorviBlessingIds.MisprintPress ||
            combatState.MisprintUsedThisTurn ||
            !IsNaturalPlayerCombatCard(card) ||
            card.Type is not (CardType.Attack or CardType.Skill))
        {
            return playCount;
        }

        combatState.MisprintUsedThisTurn = true;
        if (!card.EnergyCost.CostsX && card.EnergyCost.Canonical >= MisprintDrawCostThreshold)
        {
            combatState.MisprintDrawAfterCards.Add(card);
        }

        MainFile.Logger.Info($"[EZMicroBalance] Morvi Misprint Press added one play to {card.Id.Entry}.");
        return playCount + MisprintExtraPlayCount;
    }

    public static bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return false;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        if (combatState.OverdueLibraryDiscountArmed &&
            !ReferenceEquals(combatState.OverdueLibraryDiscountSourceCard, card) &&
            card.Pile?.Type == PileType.Hand &&
            originalCost >= 0)
        {
            modifiedCost = 0;
            return modifiedCost != originalCost;
        }

        if (GetSelectedBlessing(player) == MorviBlessingIds.BlueprintProof &&
            card.Pile?.Type == PileType.Hand &&
            IsBlueprintProofEligible(card))
        {
            TryInitializeBlueprintProofState(player, combatState, "energy-cost guard");
            if (combatState.ProofreadRemaining > 0 && card.IsUpgraded)
            {
                modifiedCost = Math.Max(0, originalCost - BlueprintProofCostReduction);
                return modifiedCost != originalCost;
            }
        }

        return false;
    }

    public static async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        var player = card.Owner;
        if (player == null || !player.IsActiveForHooks || !cardPlay.IsFirstInSeries || cardPlay.IsAutoPlay)
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        TryConsumeOverdueLibraryDiscount(card, combatState);

        if (GetSelectedBlessing(player) != MorviBlessingIds.BlueprintProof ||
            !IsBlueprintProofEligible(card))
        {
            return;
        }

        await EnsureBlueprintProofInitialized(player, combatState, "before-card-play guard");
        if (combatState.ProofreadRemaining <= 0)
        {
            return;
        }

        combatState.ProofreadRemaining--;
        await SetCounterPower<MorviProofreadPower>(
            new ThrowingPlayerChoiceContext(),
            player,
            combatState.ProofreadRemaining);

        if (card.IsUpgraded)
        {
            combatState.BlueprintBlockAfterCards.Add(card);
            MainFile.Logger.Info($"[EZMicroBalance] Morvi Blueprint Proof armed upgraded-card Block for {card.Id.Entry}.");
            return;
        }

        if (card.IsUpgradable)
        {
            CardCmd.Upgrade(card, CardPreviewStyle.None);
            combatState.BlueprintTemporaryUpgradeCards.Add(card);
            MainFile.Logger.Info($"[EZMicroBalance] Morvi Blueprint Proof temporarily upgraded {card.Id.Entry} for this play.");
        }

        combatState.BlueprintDrawAfterCards.Add(card);
    }

    public static async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (player == null || !player.IsActiveForHooks)
        {
            return;
        }

        await ResolveBorrowedAncientPlayCost(choiceContext, cardPlay);

        var combatState = CombatStates.GetOrCreateValue(player);
        if (cardPlay.IsLastInSeries && combatState.MisprintDrawAfterCards.Remove(cardPlay.Card))
        {
            await CardPileCmd.Draw(choiceContext, 1m, player);
            MainFile.Logger.Info($"[EZMicroBalance] Morvi Misprint Press drew 1 card after {cardPlay.Card.Id.Entry}.");
        }

        if (cardPlay.IsLastInSeries)
        {
            await ResolveBlueprintProofAfterPlay(choiceContext, cardPlay, combatState);
        }
    }

    public static async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card)
    {
        var player = card.Owner;
        if (player == null ||
            !player.IsActiveForHooks ||
            GetSelectedBlessing(player) != MorviBlessingIds.Paperstorm ||
            card.Type != CardType.Status)
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        if (combatState.PaperstormTriggersRemainingThisTurn <= 0 ||
            card.Pile?.Type != PileType.Hand)
        {
            return;
        }

        combatState.PaperstormTriggersRemainingThisTurn--;
        await SetCounterPower<MorviPaperstormPower>(
            choiceContext,
            player,
            combatState.PaperstormTriggersRemainingThisTurn);
        await CardCmd.Exhaust(choiceContext, card, skipVisuals: true);
        await CardPileCmd.Draw(choiceContext, 1m, player);
        await PlayerCmd.GainEnergy(1m, player);
        MainFile.Logger.Info($"[EZMicroBalance] Morvi Paperstorm converted drawn Status {card.Id.Entry}; remaining this turn={combatState.PaperstormTriggersRemainingThisTurn}.");
    }

    public static async Task AfterCombatEnd(CombatRoom room)
    {
        foreach (var player in room.CombatState.Players.Where(player => player.IsActiveForHooks))
        {
            var selectedBlessing = GetSelectedBlessing(player);
            var combatState = CombatStates.GetOrCreateValue(player);

            await CleanupMorviTemporaryCards(player);

            if (selectedBlessing == MorviBlessingIds.ForbiddenLoan &&
                room.RoomType == RoomType.Boss &&
                player.RunState.CurrentActIndex == 1)
            {
                await AutoSettleForbiddenLoan(player);
            }

            if (selectedBlessing == MorviBlessingIds.RedInkOverdraft)
            {
                await PayRedInkOverdraftDebts(player, combatState);
            }

            if (selectedBlessing == MorviBlessingIds.DebtSettlement)
            {
                await PayDebtSettlementDue(player);
            }
        }
    }

    public static bool CanUseRedInkOverdraft(Player player)
    {
        if (player == null ||
            !player.IsActiveForHooks ||
            GetSelectedBlessing(player) != MorviBlessingIds.RedInkOverdraft ||
            player.PlayerCombatState?.Energy != 0)
        {
            return false;
        }

        return !CombatStates.GetOrCreateValue(player).RedInkUsedThisTurn;
    }

    public static async Task UseRedInkOverdraft(PlayerChoiceContext choiceContext, Player player)
    {
        if (!CanUseRedInkOverdraft(player))
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        combatState.RedInkUsedThisTurn = true;
        combatState.RedInkDebtsThisCombat++;
        await CardPileCmd.Draw(choiceContext, RedInkOverdraftDraw, player);
        await PlayerCmd.GainEnergy(RedInkOverdraftEnergy, player);
        await SetCounterPower<MorviOverdraftPower>(choiceContext, player, combatState.RedInkDebtsThisCombat);
        MainFile.Logger.Info($"[EZMicroBalance] Morvi Red Ink Overdraft recorded debt {combatState.RedInkDebtsThisCombat} this combat.");
    }

    public static void ArmOverdueLibraryDiscount(Player player)
    {
        var combatState = CombatStates.GetOrCreateValue(player);
        combatState.OverdueLibraryDiscountArmed = true;
        combatState.OverdueLibraryDiscountSourceCard = player.PlayerCombatState?.AllCards.FirstOrDefault(card =>
            card.Pile?.Type == PileType.Play &&
            card.Id.Entry == MorviArchiveDiscountPage.CardId);
        MainFile.Logger.Info("[EZMicroBalance] Morvi Overdue Library armed the next-card cost-0 page.");
    }

    private static async Task SelectForbiddenLoanCard(Player player)
    {
        var ancientPool = player.Character.CardPool
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(IsForbiddenLoanCandidate)
            .DistinctBy(card => card.Id)
            .ToList();

        if (ancientPool.Count == 0)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Morvi Forbidden Loan skipped: no unlocked class Ancient cards were found.");
            return;
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
            return;
        }

        var addResult = await CardPileCmd.Add(selected, PileType.Deck);
        if (!addResult.success)
        {
            AncientCardHelpers.RemoveUnpiledRunCard(selected);
            MainFile.Logger.Warn($"[EZMicroBalance] Morvi Forbidden Loan failed to add borrowed Ancient card {selected.Id.Entry}; progress was left unchanged.");
            return;
        }

        var borrowedCard = addResult.cardAdded;
        AncientSavedStateFields.MorviBorrowedAncientCard[borrowedCard] = true;
        var progress = GetProgress(player) with
        {
            BorrowedCardId = borrowedCard.Id.Entry,
            BorrowedSettled = false
        };
        SetProgress(player, progress);
        MainFile.Logger.Info($"[EZMicroBalance] Morvi Forbidden Loan added upgraded borrowed Ancient card {borrowedCard.Id.Entry}.");
    }

    private static async Task ResolveDebtSettlementPickup(Player player)
    {
        await PlayerCmd.GainGold(DebtSettlementImmediateGold, player);

        var removalPrefs = new CardSelectorPrefs(
            new LocString("ancients", "EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.removeSelectionPrompt"),
            0,
            2);
        var removals = (await CardSelectCmd.FromDeckForRemoval(player, removalPrefs)).ToList();
        if (removals.Count > 0)
        {
            await CardPileCmd.RemoveFromDeck(removals);
        }

        var upgradePrefs = new CardSelectorPrefs(
            new LocString("ancients", "EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.upgradeSelectionPrompt"),
            0,
            2);
        var upgrades = (await CardSelectCmd.FromDeckForUpgrade(player, upgradePrefs)).ToList();
        if (upgrades.Count > 0)
        {
            CardCmd.Upgrade(upgrades, CardPreviewStyle.EventLayout);
        }

        SetProgress(player, new Progress(DebtSettlementStartingDebt, string.Empty, false));
        MainFile.Logger.Info("[EZMicroBalance] Morvi Debt Settlement granted 220 Gold, resolved optional removal/upgrade selections, and set Debt to 320.");
    }

    private static async Task AddArchivePages(Player player)
    {
        if (player.Creature.CombatState == null)
        {
            return;
        }

        var pages = new List<CardModel>();
        for (var index = 0; index < OverdueLibraryPageCount; index++)
        {
            var pageType = player.RunState.Rng.CombatCardSelection.NextItem(ArchivePageTypes) ?? typeof(MorviArchiveDrawPage);
            var canonical = ModelDb.GetById<CardModel>(ModelDb.GetId(pageType));
            pages.Add(player.Creature.CombatState.CreateCard(canonical, player));
        }

        foreach (var page in pages)
        {
            await AncientCardHelpers.TryAddGeneratedCardToCombat(page, PileType.Hand, player);
        }

        MainFile.Logger.Info("[EZMicroBalance] Morvi Overdue Library added 3 random Archive Pages to hand.");
    }

    private static async Task AddRedInkOverdraftCard(Player player)
    {
        if (player.Creature.CombatState == null)
        {
            return;
        }

        var hand = PileType.Hand.GetPile(player);
        if (hand.Cards.Count >= CardPile.MaxCardsInHand)
        {
            MainFile.Logger.Info("[EZMicroBalance] Morvi Red Ink Overdraft skipped this turn because the hand is full.");
            return;
        }

        var card = player.Creature.CombatState.CreateCard<MorviRedInkOverdraftCard>(player);
        var addResult = await AncientCardHelpers.TryAddGeneratedCardToCombat(card, PileType.Hand, player);
        if (addResult is not { success: true } result)
        {
            return;
        }

        if (result.cardAdded.Pile?.Type != PileType.Hand)
        {
            await CardPileCmd.RemoveFromCombat(result.cardAdded, skipVisuals: true);
            MainFile.Logger.Warn("[EZMicroBalance] Morvi Red Ink Overdraft generated card did not land in hand and was removed to avoid combat-pile flooding.");
        }
    }

    private static async Task AddWastePapers(Player player)
    {
        if (player.Creature.CombatState == null)
        {
            return;
        }

        for (var index = 0; index < PaperstormWastePaperCount; index++)
        {
            var waste = player.Creature.CombatState.CreateCard<MorviWastePaper>(player);
            await AncientCardHelpers.TryAddGeneratedCardToCombat(waste, PileType.Draw, player, CardPilePosition.Random);
        }

        MainFile.Logger.Info("[EZMicroBalance] Morvi Paperstorm shuffled 4 Waste Paper Status cards into the draw pile.");
    }

    private static async Task SealOpenBookCards(PlayerChoiceContext choiceContext, Player player, MorviCombatState combatState)
    {
        var toSeal = PileType.Hand.GetPile(player)
            .Cards
            .Where(combatState.OpenBookDrawnCards.Contains)
            .ToList();

        combatState.OpenBookSealedCards.Clear();
        foreach (var card in toSeal)
        {
            var addResult = await CardPileCmd.Add(card, PileType.Exhaust);
            if (!addResult.success)
            {
                continue;
            }

            AncientSavedStateFields.MorviOpenBookSealedCard[addResult.cardAdded] = true;
            combatState.OpenBookSealedCards.Add(addResult.cardAdded);
        }

        combatState.OpenBookDrawnCards.Clear();
        await SetCounterPower<MorviOpenBookPower>(choiceContext, player, combatState.OpenBookSealedCards.Count);
        MainFile.Logger.Info($"[EZMicroBalance] Morvi Open-Book Exam sealed {combatState.OpenBookSealedCards.Count} cards into exhaust-pile holding until turn 3.");
    }

    private static async Task ReturnOpenBookCards(Player player, MorviCombatState combatState)
    {
        var hand = PileType.Hand.GetPile(player);
        var returned = 0;
        foreach (var card in FindOpenBookSealedCards(player, combatState))
        {
            AncientSavedStateFields.MorviOpenBookSealedCard[card] = false;
            if (hand.Cards.Count >= CardPile.MaxCardsInHand ||
                card.Pile?.Type.IsCombatPile() != true ||
                card.HasBeenRemovedFromState)
            {
                continue;
            }

            var addResult = await CardPileCmd.Add(card, PileType.Hand);
            if (!addResult.success)
            {
                continue;
            }

            addResult.cardAdded.SetToFreeThisTurn();
            returned++;
        }

        combatState.OpenBookSealedCards.Clear();
        await SetCounterPower<MorviOpenBookPower>(new ThrowingPlayerChoiceContext(), player, 0);
        MainFile.Logger.Info($"[EZMicroBalance] Morvi Open-Book Exam returned {returned} sealed cards on turn 3 and made them cost 0 this turn.");
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

    private static async Task ResolveBlueprintProofAfterPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        MorviCombatState combatState)
    {
        var card = cardPlay.Card;
        var player = card.Owner;

        if (combatState.BlueprintTemporaryUpgradeCards.Remove(card))
        {
            CardCmd.Downgrade(card);
        }

        if (combatState.BlueprintDrawAfterCards.Remove(card))
        {
            await CardPileCmd.Draw(choiceContext, 1m, player);
            MainFile.Logger.Info($"[EZMicroBalance] Morvi Blueprint Proof drew 1 after {card.Id.Entry}.");
        }

        if (combatState.BlueprintBlockAfterCards.Remove(card))
        {
            await CreatureCmd.GainBlock(player.Creature, BlueprintProofBlock, ValueProp.Move, cardPlay, fast: true);
            MainFile.Logger.Info($"[EZMicroBalance] Morvi Blueprint Proof granted {BlueprintProofBlock} Block after upgraded card {card.Id.Entry}.");
        }
    }

    private static async Task EnsureBlueprintProofInitialized(
        Player player,
        MorviCombatState combatState,
        string reason)
    {
        if (!TryInitializeBlueprintProofState(player, combatState, reason))
        {
            return;
        }

        await SetCounterPower<MorviProofreadPower>(
            new ThrowingPlayerChoiceContext(),
            player,
            combatState.ProofreadRemaining);
    }

    private static bool TryInitializeBlueprintProofState(
        Player player,
        MorviCombatState combatState,
        string reason)
    {
        if (combatState.BlueprintProofInitializedThisCombat ||
            GetSelectedBlessing(player) != MorviBlessingIds.BlueprintProof ||
            player.PlayerCombatState == null ||
            player.Creature.CombatState == null)
        {
            return false;
        }

        var visibleProofread = player.Creature.GetPower<MorviProofreadPower>()?.Amount ?? 0;
        combatState.ProofreadRemaining = visibleProofread > 0
            ? visibleProofread
            : BlueprintProofStacks;
        combatState.BlueprintProofInitializedThisCombat = true;
        MainFile.Logger.Info($"[EZMicroBalance] Morvi Blueprint Proof initialized {combatState.ProofreadRemaining} Proofread ({reason}).");
        return true;
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

    private static async Task PayRedInkOverdraftDebts(Player player, MorviCombatState combatState)
    {
        var visibleDebtCount = player.Creature.GetPower<MorviOverdraftPower>()?.Amount ?? 0;
        var debtCount = Math.Max(combatState.RedInkDebtsThisCombat, visibleDebtCount);
        if (debtCount <= 0)
        {
            return;
        }

        for (var index = 0; index < debtCount; index++)
        {
            if (player.Gold >= RedInkOverdraftGoldPerDebt)
            {
                await PlayerCmd.LoseGold(RedInkOverdraftGoldPerDebt, player, GoldLossType.Spent);
                continue;
            }

            await DamagePlayerNonlethal(player, RedInkOverdraftHpPerUnpaidDebt);
        }

        MainFile.Logger.Info($"[EZMicroBalance] Morvi Red Ink Overdraft settled {debtCount} combat debt(s).");
        combatState.RedInkDebtsThisCombat = 0;
        await SetCounterPower<MorviOverdraftPower>(new ThrowingPlayerChoiceContext(), player, 0);
    }

    private static async Task PayDebtSettlementDue(Player player)
    {
        var progress = GetProgress(player);
        if (progress.DebtRemaining <= 0)
        {
            return;
        }

        var due = Math.Min(DebtSettlementCombatDue, progress.DebtRemaining);
        var goldPaid = Math.Min(player.Gold, due);
        if (goldPaid > 0)
        {
            await PlayerCmd.LoseGold(goldPaid, player, GoldLossType.Spent);
        }

        var shortfall = due - goldPaid;
        if (shortfall > 0)
        {
            var calculatedHpLoss = (int)Math.Ceiling(shortfall / 10m) * DebtSettlementHpPerTenShortfall;
            await DamagePlayerNonlethal(player, calculatedHpLoss);
        }

        var nextProgress = progress with { DebtRemaining = Math.Max(0, progress.DebtRemaining - due) };
        SetProgress(player, nextProgress);
        await SetCounterPower<MorviDebtPower>(
            new ThrowingPlayerChoiceContext(),
            player,
            nextProgress.DebtRemaining);
        MainFile.Logger.Info($"[EZMicroBalance] Morvi Debt Settlement paid due={due}; debt remaining={nextProgress.DebtRemaining}.");
    }

    private static async Task DamagePlayerNonlethal(Player player, decimal calculatedHpLoss)
    {
        var maximumNonlethalHpLoss = Math.Max(0m, player.Creature.CurrentHp - 1m);
        var hpLoss = Math.Min(calculatedHpLoss, maximumNonlethalHpLoss);
        if (hpLoss <= 0)
        {
            return;
        }

        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            player.Creature,
            hpLoss,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            player.Creature,
            null);
    }

    private static async Task CleanupMorviTemporaryCards(Player player)
    {
        var cards = player.PlayerCombatState?.AllCards
            .Where(card => card.Pile?.Type.IsCombatPile() == true && TemporaryCardIds.Contains(card.Id.Entry))
            .ToList();
        if (cards is { Count: > 0 })
        {
            await CardPileCmd.RemoveFromCombat(cards, skipVisuals: true);
        }

        ClearOpenBookMarkers(player);
    }

    private static void TryConsumeOverdueLibraryDiscount(CardModel card, MorviCombatState combatState)
    {
        if (!combatState.OverdueLibraryDiscountArmed ||
            ReferenceEquals(combatState.OverdueLibraryDiscountSourceCard, card))
        {
            return;
        }

        combatState.OverdueLibraryDiscountArmed = false;
        combatState.OverdueLibraryDiscountSourceCard = null;
        MainFile.Logger.Info($"[EZMicroBalance] Morvi Overdue Library consumed next-card cost-0 discount on {card.Id.Entry}.");
    }

    private static void ResetCombatState(MorviCombatState combatState)
    {
        combatState.RedInkDebtsThisCombat = 0;
        combatState.OpenBookResolved = false;
        combatState.OpenBookDrawnCards.Clear();
        combatState.OpenBookSealedCards.Clear();
        combatState.ProofreadRemaining = 0;
        combatState.BlueprintProofInitializedThisCombat = false;
        combatState.BlueprintTemporaryUpgradeCards.Clear();
        combatState.BlueprintDrawAfterCards.Clear();
        combatState.BlueprintBlockAfterCards.Clear();
        ResetTurnState(combatState);
    }

    private static void ResetTurnState(MorviCombatState combatState)
    {
        combatState.MisprintUsedThisTurn = false;
        combatState.MisprintDrawAfterCards.Clear();
        combatState.AutoPlayCardPendingModifier = null;
        combatState.RedInkUsedThisTurn = false;
        combatState.OverdueLibraryDiscountArmed = false;
        combatState.OverdueLibraryDiscountSourceCard = null;
        combatState.PaperstormTriggersRemainingThisTurn = 0;
    }

    private static bool TryConsumeAutoPlayModifierBlock(CardModel card, MorviCombatState combatState)
    {
        if (!ReferenceEquals(combatState.AutoPlayCardPendingModifier, card))
        {
            return false;
        }

        combatState.AutoPlayCardPendingModifier = null;
        return true;
    }

    private static async Task SetCounterPower<T>(PlayerChoiceContext choiceContext, Player player, int amount)
        where T : PowerModel
    {
        var power = player.Creature.GetPower<T>();
        if (amount <= 0)
        {
            await PowerCmd.Remove(power);
            return;
        }

        if (power == null)
        {
            await PowerCmd.Apply<T>(choiceContext, player.Creature, amount, player.Creature, null);
            return;
        }

        var delta = amount - power.Amount;
        if (delta != 0)
        {
            await PowerCmd.Apply<T>(choiceContext, player.Creature, delta, player.Creature, null);
        }
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

    private static bool IsNaturalPlayerCombatCard(CardModel card) =>
        card.DeckVersion != null &&
        !card.IsClone &&
        card.Type is not CardType.Status and not CardType.Curse;

    private static bool IsBlueprintProofEligible(CardModel card) =>
        IsNaturalPlayerCombatCard(card) &&
        card.Type is not CardType.Status and not CardType.Curse;

    private static bool IsBorrowedAncientDeckCard(CardModel card) =>
        AncientSavedStateFields.MorviBorrowedAncientCard[card];

    private static bool IsBorrowedAncientCombatCard(CardModel card) =>
        card.DeckVersion is { } deckCard
            ? AncientSavedStateFields.MorviBorrowedAncientCard[deckCard]
            : AncientSavedStateFields.MorviBorrowedAncientCard[card];

    private static List<CardModel> FindOpenBookSealedCards(Player player, MorviCombatState combatState)
    {
        var cards = combatState.OpenBookSealedCards
            .Concat(player.PlayerCombatState?.AllCards.Where(card => AncientSavedStateFields.MorviOpenBookSealedCard[card]) ?? [])
            .Where(card => !card.HasBeenRemovedFromState)
            .Distinct()
            .ToList();

        combatState.OpenBookSealedCards.Clear();
        combatState.OpenBookSealedCards.AddRange(cards);
        return cards;
    }

    private static void ClearOpenBookMarkers(Player player)
    {
        foreach (var card in player.PlayerCombatState?.AllCards ?? [])
        {
            AncientSavedStateFields.MorviOpenBookSealedCard[card] = false;
        }
    }

    private static Progress GetProgress(Player player)
    {
        var state = AncientPlayerState.Get(
            player,
            AncientSavedStateFields.MorviStateKey,
            AncientSavedStateFields.MorviDeckStateKey);
        var parts = state.Split(ProgressSeparator);
        if (parts.Length < 4)
        {
            return Progress.Default;
        }

        return new Progress(
            ParseInt(parts[1]),
            parts[2],
            ParseBool(parts[3]));
    }

    private static void SetProgress(Player player, Progress progress)
    {
        var selectedBlessing = GetSelectedBlessing(player);
        if (!string.IsNullOrWhiteSpace(selectedBlessing))
        {
            SetState(player, selectedBlessing, progress);
        }
    }

    private static void SetState(Player player, string blessingId, Progress progress)
    {
        AncientPlayerState.Set(
            player,
            string.Join(
                ProgressSeparator,
                blessingId,
                progress.DebtRemaining,
                progress.BorrowedCardId,
                progress.BorrowedSettled ? 1 : 0),
            AncientSavedStateFields.MorviStateKey,
            AncientSavedStateFields.MorviDeckStateKey);
    }

    private static int ParseInt(string value) =>
        int.TryParse(value, out var parsed) ? Math.Max(0, parsed) : 0;

    private static bool ParseBool(string value) =>
        value == "1" || bool.TryParse(value, out var parsed) && parsed;
}
