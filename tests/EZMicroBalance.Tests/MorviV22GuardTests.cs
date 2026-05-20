using Xunit;

namespace EZMicroBalance.Tests;

public sealed class MorviV22GuardTests
{
    private static readonly string[] BlessingIds =
    [
        "morvi_forbidden_loan",
        "morvi_misprint_press",
        "morvi_red_ink_overdraft",
        "morvi_overdue_library",
        "morvi_open_book_exam",
        "morvi_paperstorm",
        "morvi_blueprint_proof",
        "morvi_debt_settlement"
    ];

    private static readonly string[] MojibakeFragments =
    [
        "閻熶椒绀?",
        "鐎殿喒鍋?",
        "闁衡偓",
        "閸婂搫濮?"
    ];

    [Fact]
    public void CombatLifecycleUsesScopedCombatStateInsteadOfGlobalRunStateLookup()
    {
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviRunHook.cs");

        Assert.DoesNotContain("RunManager.Instance.DebugOnlyGetState()", runHook, StringComparison.Ordinal);
        AssertSourceContains(
            runHook,
            "CombatManager.Instance.DebugOnlyGetState()",
            "activeCombatState.Players.Where(player => player.IsActiveForHooks)",
            "room.CombatState.Players.Where(player => player.IsActiveForHooks)");
    }

    [Fact]
    public void MorviIsDefaultOnDisableableForceableAndHasEightBlessings()
    {
        var gate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviFeatureGate.cs");
        var ancient = ReadMorviSource();
        var blessings = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingIds.cs");
        var initializer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviInitializer.cs");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var mainFile = ReadRepoText("EZMicroBalanceCode", "MainFile.cs");

        AssertSourceContains(
            gate,
            "EZMB_DISABLE_MORVI",
            "SPIREPLUS_DISABLE_MORVI",
            "EZMB_FORCE_ANCIENT",
            "SPIREPLUS_FORCE_ANCIENT",
            "EZMB_FORCE_MORVI_BLESSING",
            "SPIREPLUS_FORCE_MORVI_BLESSING",
            "ShouldForceMorvi",
            "AncientFeatureGate.IsTruthyEnvironmentVariable(DisableEnvironmentVariable)");
        Assert.DoesNotContain("return IsTruthy(value);", gate, StringComparison.Ordinal);

        AssertSourceContains(
            ancient,
            "CustomAncientModel",
            "HarmonyPatch(typeof(Hive), nameof(Hive.GetUnlockedAncients))",
            "MorviFeatureGate.ShouldForceMorvi",
            "unlockedAncients = [morvi]",
            "list.Add(morvi)",
            "ExpectedInitialOptionCount = 3",
            "options.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList()",
            "MorviAssetPaths.BackgroundScene");
        Assert.Contains("MorviInitializer.Initialize();", mainFile, StringComparison.Ordinal);
        AssertSourceContains(
            initializer,
            "ModHelper.SubscribeForRunStateHooks",
            "ModHelper.SubscribeForCombatStateHooks",
            "ModelDb.GetById<MorviRunHook>",
            "ModelDb.GetById<MorviCombatHook>",
            "default-on");
        AssertSourceContains(
            savedFields,
            "SavedSpireField<Player, string> MorviStateKey",
            "SavedSpireField<CardModel, string> MorviDeckStateKey",
            "SavedSpireField<CardModel, bool> MorviBorrowedAncientCard",
            "SavedSpireField<CardModel, bool> MorviOpenBookSealedCard");

        foreach (var id in BlessingIds)
        {
            Assert.Contains(id, blessings, StringComparison.Ordinal);
            Assert.Contains(id, ancient, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void MisprintPressUsesPlayCountNotHandCopiesAndBlocksPowerAutoplayRecursion()
    {
        var runHook = ReadMorviSource();
        var misprint = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.MisprintPress.cs");

        AssertSourceContains(
            misprint,
            "MorviBlessingIds.MisprintPress",
            "combatState.MisprintUsedThisTurn",
            "TryConsumeAutoPlayModifierBlock(card, combatState)",
            "!IsNaturalPlayerCombatCard(card)",
            "card.Type is not (CardType.Attack or CardType.Skill)",
            "combatState.MisprintDrawAfterCards.Add(card)",
            "return playCount + MisprintExtraPlayCount;");
        AssertSourceContains(
            runHook,
            "card.DeckVersion != null",
            "!card.IsClone",
            "card.Type is not CardType.Status and not CardType.Curse",
            "public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)",
            "cardPlay.IsAutoPlay");
        Assert.DoesNotContain("CreateClone", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryAddGeneratedCardToCombat(copy", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("CardCmd.AutoPlay", runHook, StringComparison.Ordinal);
    }

    [Fact]
    public void MorviSourceConstantsAndStatefulBlessingsMatchV22Numbers()
    {
        var runHook = ReadMorviSource();

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
            "AncientSavedStateFields.MorviBorrowedAncientCard[borrowedCard] = true",
            "player.RunState.CurrentActIndex == 1",
            "AutoSettleForbiddenLoan");

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
        var redInk = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.RedInkOverdraft.cs");
        var debtSettlement = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.DebtSettlement.cs");
        var payments = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.Payments.cs");
        var ancient = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi");
        var cards = ReadMorviSource();
        var state = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.State.cs");
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviRunHook.cs");

        AssertSourceContains(
            forbiddenLoan,
            "internal static bool HasForbiddenLoanCandidates(Player player)",
            "private static async Task<Progress?> TrySelectForbiddenLoanCard",
            "private static async Task ResolveBorrowedAncientPlayCost",
            "private static async Task AutoSettleForbiddenLoan",
            "private static void ClearBorrowedAncientCards");
        AssertSourceContains(
            redInk,
            "public static bool CanUseRedInkOverdraft(Player player)",
            "public static async Task UseRedInkOverdraft(PlayerChoiceContext choiceContext, Player player)",
            "private static async Task AddRedInkOverdraftCard",
            "private static async Task PayRedInkOverdraftDebts");
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
            forbiddenLoan,
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
        Assert.DoesNotContain("public static async Task AutoSettleForbiddenLoan", forbiddenLoan, StringComparison.Ordinal);
        Assert.DoesNotContain("public static async Task DamagePlayerNonlethal", payments, StringComparison.Ordinal);
    }

    [Fact]
    public void OverdueLibraryCardsPowersAndCleanupAreSourceBacked()
    {
        var runHook = ReadMorviSource();
        var cards = ReadMorviSource();
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviPowers.cs");
        var engCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var zhsCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");

        AssertSourceContains(
            runHook,
            "OverdueLibraryPageCount = 3",
            "ArchivePageTypes",
            "MorviArchiveDrawPage",
            "MorviArchiveVeilPage",
            "MorviArchiveBurnPage",
            "MorviArchiveDiscountPage",
            "MorviArchiveBraveryPage",
            "MorviArchiveDexterityPage",
            "CleanupMorviTemporaryCards",
            "AncientCardHelpers.TryAddGeneratedCardToCombat(page, PileType.Hand, player)",
            "AncientCardHelpers.TryAddGeneratedCardToCombat(card, PileType.Hand, player)",
            "CardPileCmd.RemoveFromCombat(cards, skipVisuals: true)");
        Assert.DoesNotContain("CardPileCmd.AddGeneratedCardToCombat", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("CardPileCmd.AddGeneratedCardsToCombat", runHook, StringComparison.Ordinal);
        AssertSourceContains(
            cards,
            "CardKeyword.Ethereal",
            "CardKeyword.Exhaust",
            "CanBeGeneratedInCombat => false",
            "CanBeGeneratedByModifiers => false",
            "new CardsVar(2)",
            "CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner)",
            "CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay)",
            "TargetingAllOpponents(CombatState)",
            "ArmOverdueLibraryDiscount(Owner, this)",
            "PowerCmd.Apply<MorviBraveryPagePower>",
            "PowerCmd.Apply<MorviDexterityPagePower>");
        AssertSourceContains(
            runHook,
            "ArmOverdueLibraryDiscount(Player player, CardModel sourceCard)",
            "combatState.OverdueLibraryDiscountSourceCard = sourceCard");
        Assert.DoesNotContain("MorviArchiveDiscountPage.CardId)", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("new IntVar(\"Cards\"", cards, StringComparison.Ordinal);
        AssertSourceContains(
            powers,
            "MorviBraveryPagePower : CustomTemporaryPowerModelWrapper<MorviArchiveBraveryPage, StrengthPower>",
            "MorviDexterityPagePower : CustomTemporaryPowerModelWrapper<MorviArchiveDexterityPage, DexterityPower>",
            "CustomPackedIconPath => MorviAssetPaths.ArchivePagePowerIcon",
            "CustomBigIconPath => MorviAssetPaths.ArchivePagePowerBigIcon");
        Assert.Contains("ArchivePagePowerIcon", ReadMorviSource(), StringComparison.Ordinal);
        Assert.Contains("{StrengthPower:diff()}", engCards["EZMB_MORVI_ARCHIVE_BRAVERY_PAGE.description"], StringComparison.Ordinal);
        Assert.Contains("{DexterityPower:diff()}", engCards["EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE.description"], StringComparison.Ordinal);
        Assert.Contains("{StrengthPower:diff()}", zhsCards["EZMB_MORVI_ARCHIVE_BRAVERY_PAGE.description"], StringComparison.Ordinal);
        Assert.Contains("{DexterityPower:diff()}", zhsCards["EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("{Strength:diff()}", engCards["EZMB_MORVI_ARCHIVE_BRAVERY_PAGE.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("{Dexterity:diff()}", engCards["EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE.description"], StringComparison.Ordinal);

        foreach (var key in new[]
        {
            "EZMB_MORVI_ARCHIVE_DRAW_PAGE.title",
            "EZMB_MORVI_ARCHIVE_VEIL_PAGE.title",
            "EZMB_MORVI_ARCHIVE_BURN_PAGE.title",
            "EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE.title",
            "EZMB_MORVI_ARCHIVE_BRAVERY_PAGE.title",
            "EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE.title",
            "EZMB_MORVI_WASTE_PAPER.title"
        })
        {
            Assert.True(engCards.ContainsKey(key), $"Missing English Morvi card localization: {key}");
            Assert.True(zhsCards.ContainsKey(key), $"Missing zhs Morvi card localization: {key}");
        }
    }

    [Fact]
    public void MorviDebtCountersAreVisibleCountersNotArtifactBlockedDebuffs()
    {
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviPowers.cs");
        var redInk = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.RedInkOverdraft.cs");
        var debtSettlement = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.DebtSettlement.cs");

        var debtPower = SliceBetween(powers, "internal sealed class MorviDebtPower", "internal sealed class MorviProofreadPower");
        var overdraftPower = SliceBetween(powers, "internal sealed class MorviOverdraftPower", "internal sealed class MorviPaperstormPower");

        Assert.Contains("public override PowerType Type => PowerType.Buff", debtPower, StringComparison.Ordinal);
        Assert.Contains("public override PowerType Type => PowerType.Buff", overdraftPower, StringComparison.Ordinal);
        Assert.DoesNotContain("public override PowerType Type => PowerType.Debuff", debtPower, StringComparison.Ordinal);
        Assert.DoesNotContain("public override PowerType Type => PowerType.Debuff", overdraftPower, StringComparison.Ordinal);

        AssertSourceContains(
            redInk,
            "await SetCounterPower<MorviOverdraftPower>(choiceContext, player, combatState.RedInkDebtsThisCombat)",
            "visibleDebtCount = player.Creature.GetPower<MorviOverdraftPower>()?.Amount ?? 0");
        AssertSourceContains(
            debtSettlement,
            "await SetCounterPower<MorviDebtPower>(",
            "nextProgress.DebtRemaining");
    }

    [Fact]
    public void BlueprintProofHasPerCombatLateInitializationGuard()
    {
        var runHook = ReadMorviSource();
        var beforeCombat = SliceBetween(runHook, "public static async Task BeforeCombatStart", "public static async Task AfterPlayerTurnStart");
        var costHook = SliceBetween(runHook, "public static bool TryModifyEnergyCostInCombat", "public static async Task BeforeCardPlayed");
        var beforeCard = SliceBetween(runHook, "public static async Task BeforeCardPlayed", "public static async Task AfterCardPlayed");
        var initialization = SliceBetween(runHook, "private static async Task EnsureBlueprintProofInitialized", "private static async Task CleanupMorviTemporaryCards");

        AssertSourceContains(
            runHook,
            "public bool BlueprintProofInitializedThisCombat { get; set; }",
            "combatState.BlueprintProofInitializedThisCombat = false;");
        AssertSourceContains(
            beforeCombat,
            "case MorviBlessingIds.BlueprintProof:",
            "await EnsureBlueprintProofInitialized(player, combatState, \"combat start\");");
        AssertSourceContains(
            costHook,
            "TryInitializeBlueprintProofState(player, combatState, \"energy-cost guard\")",
            "combatState.ProofreadRemaining > 0 && card.IsUpgraded",
            "modifiedCost = Math.Max(0, originalCost - BlueprintProofCostReduction)");
        AssertSourceContains(
            beforeCard,
            "!cardPlay.IsFirstInSeries || cardPlay.IsAutoPlay",
            "await EnsureBlueprintProofInitialized(player, combatState, \"before-card-play guard\");",
            "combatState.ProofreadRemaining--;",
            "CardCmd.Upgrade(card, CardPreviewStyle.None)",
            "combatState.BlueprintDrawAfterCards.Add(card)",
            "combatState.BlueprintBlockAfterCards.Add(card)");
        AssertSourceContains(
            initialization,
            "combatState.BlueprintProofInitializedThisCombat",
            "player.PlayerCombatState == null",
            "player.Creature.CombatState == null",
            "visibleProofread = player.Creature.GetPower<MorviProofreadPower>()?.Amount ?? 0",
            "? visibleProofread",
            ": BlueprintProofStacks",
            "combatState.BlueprintProofInitializedThisCombat = true",
            "Morvi Blueprint Proof initialized");
    }

    [Fact]
    public void MorviLocalizationAssetsAndHoverSupportArePresentAndReadable()
    {
        var ancient = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi");
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviPowers.cs");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var engPowers = JsonStringMap("EZMicroBalance", "localization", "eng", "powers.json");
        var zhsPowers = JsonStringMap("EZMicroBalance", "localization", "zhs", "powers.json");

        AssertSourceContains(
            ancient,
            "HoverTipFactory.FromPower<MorviOverdraftPower>()",
            "HoverTipFactory.FromPower<MorviOpenBookPower>()",
            "HoverTipFactory.FromPower<MorviPaperstormPower>()",
            "HoverTipFactory.FromPower<MorviProofreadPower>()",
            "HoverTipFactory.FromPower<MorviDebtPower>()",
            "HoverTipFactory.Static(StaticHoverTip.Energy)",
            "HoverTipFactory.Static(StaticHoverTip.Block)");
        AssertSourceContains(
            powers,
            "internal sealed class MorviDebtPower",
            "internal sealed class MorviProofreadPower",
            "internal sealed class MorviOpenBookPower",
            "internal sealed class MorviOverdraftPower",
            "internal sealed class MorviPaperstormPower");

        foreach (var id in BlessingIds)
        {
            var key = $"EZMB_MORVI.pages.INITIAL.options.{id}.description";
            Assert.True(engAncients.TryGetValue(key, out var engDescription), $"Missing English Morvi ancient localization: {key}");
            Assert.True(zhsAncients.TryGetValue(key, out var zhsDescription), $"Missing zhs Morvi ancient localization: {key}");
            AssertNoMojibake(engDescription, MojibakeFragments);
            AssertNoMojibake(zhsDescription, MojibakeFragments);
            Assert.Contains("[blue]", engDescription, StringComparison.Ordinal);
            Assert.Contains("[blue]", zhsDescription, StringComparison.Ordinal);
        }

        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_misprint_press.description"],
            "Once each turn",
            "manually played deck",
            "[gold]Attack[/gold]",
            "[gold]Skill[/gold]",
            "[gold]Power[/gold]",
            "[gold]Energy[/gold]",
            "generated cards do not trigger");
        Assert.DoesNotContain("Borrow one upgraded", engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_forbidden_loan.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("借一张", zhsAncients["EZMB_MORVI.pages.INITIAL.options.morvi_forbidden_loan.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("借来的牌", zhsRelics["EZMICROBALANCE-MORVI_FORBIDDEN_LOAN_OPTION_RELIC.description"], StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_red_ink_overdraft.description"],
            "[gold]Overdraft[/gold]",
            "[gold]Energy[/gold]",
            "nonlethal HP");
        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_blueprint_proof.description"],
            "manually played deck cards",
            "draw [blue]1[/blue]",
            "gain [blue]4[/blue] [gold]Block[/gold]",
            "[gold]Proofread[/gold]",
            "[gold]Block[/gold]");
        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.description"],
            "Take [blue]320[/blue] [gold]Debt[/gold]",
            "repay [blue]40[/blue] [gold]Gold[/gold]",
            "for each [blue]10[/blue] short",
            "lose [blue]3[/blue] nonlethal HP");
        AssertSourceContains(
            zhsAncients["EZMB_MORVI.pages.INITIAL.options.morvi_misprint_press.description"],
            "每回合一次",
            "手动打出",
            "生成牌不触发");
        AssertSourceContains(
            zhsAncients["EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.description"],
            "获得[blue]320[/blue]点[gold]债务[/gold]",
            "每场战斗后偿还[blue]40[/blue][gold]金币[/gold]",
            "每短缺[blue]10[/blue][gold]金币[/gold]",
            "失去[blue]3[/blue]点非致命生命");
        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_open_book_exam.description"],
            "sealed in the [gold]Exhaust Pile[/gold]");

        AssertLocalizedKeys(MorviRelicKeys(), engRelics, zhsRelics, "Morvi option relic localization", value => AssertNoMojibake(value, MojibakeFragments));
        AssertLocalizedKeys(MorviPowerKeys(), engPowers, zhsPowers, "Morvi power localization", value => AssertNoMojibake(value, MojibakeFragments));

        foreach (var relativePath in MorviResourcePaths())
        {
            AssertRepoFileExists(relativePath.Split('/'));
            Assert.Contains($"res://{relativePath}", exportPreset, StringComparison.Ordinal);
        }
    }

    private static IEnumerable<string> MorviRelicKeys()
    {
        foreach (var key in new[]
        {
            "FORBIDDEN_LOAN",
            "MISPRINT_PRESS",
            "RED_INK_OVERDRAFT",
            "OVERDUE_LIBRARY",
            "OPEN_BOOK_EXAM",
            "PAPERSTORM",
            "BLUEPRINT_PROOF",
            "DEBT_SETTLEMENT"
        })
        {
            yield return $"EZMICROBALANCE-MORVI_{key}_OPTION_RELIC.title";
            yield return $"EZMICROBALANCE-MORVI_{key}_OPTION_RELIC.description";
            yield return $"EZMICROBALANCE-MORVI_{key}_OPTION_RELIC.flavor";
        }
    }

    private static IEnumerable<string> MorviPowerKeys()
    {
        foreach (var key in new[]
        {
            "DEBT",
            "PROOFREAD",
            "OPEN_BOOK",
            "OVERDRAFT",
            "PAPERSTORM"
        })
        {
            yield return $"EZMICROBALANCE-MORVI_{key}_POWER.title";
            yield return $"EZMICROBALANCE-MORVI_{key}_POWER.description";
            yield return $"EZMICROBALANCE-MORVI_{key}_POWER.smartDescription";
        }
    }

    private static IEnumerable<string> MorviResourcePaths()
    {
        yield return "EZMicroBalance/images/events/ezmb_morvi.png";
        yield return "EZMicroBalance/images/ancients/morvi/ezmb_morvi_map_icon.png";
        yield return "EZMicroBalance/images/ancients/morvi/ezmb_morvi_map_icon_outline.png";
        yield return "EZMicroBalance/images/ancients/morvi/ezmb_morvi_run_history_icon.png";
        yield return "EZMicroBalance/images/ancients/morvi/ezmb_morvi_run_history_icon_outline.png";
        foreach (var id in BlessingIds)
        {
            yield return $"EZMicroBalance/images/ancients/morvi/options/{id}.png";
        }

        yield return "EZMicroBalance/scenes/events/background_scenes/ezmb_morvi.tscn";
    }

    private static string ReadMorviSource() =>
        ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi");

}
