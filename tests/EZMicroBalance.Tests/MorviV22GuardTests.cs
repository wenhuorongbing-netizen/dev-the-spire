using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class MorviV22GuardTests
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
        var runLifecycle = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.CombatLifecycle.cs");

        Assert.DoesNotContain("RunManager.Instance.DebugOnlyGetState()", runLifecycle, StringComparison.Ordinal);
        AssertSourceContains(
            runLifecycle,
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
        var featureRegistry = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "SpirePlusFeatureRegistry.cs");

        AssertSourceContains(
            gate,
            "DisableEnvironmentVariable = \"SPIREPLUS_DISABLE_MORVI\"",
            "LegacyDisableEnvironmentVariable = \"EZMB_DISABLE_MORVI\"",
            "ForceAncientEnvironmentVariable = \"SPIREPLUS_FORCE_ANCIENT\"",
            "LegacyForceAncientEnvironmentVariable = \"EZMB_FORCE_ANCIENT\"",
            "ForceBlessingEnvironmentVariable = \"SPIREPLUS_FORCE_MORVI_BLESSING\"",
            "LegacyForceBlessingEnvironmentVariable = \"EZMB_FORCE_MORVI_BLESSING\"",
            "ShouldForceMorvi",
            "FirstRawEnvironmentValue(ForceAncientEnvironmentVariable, LegacyForceAncientEnvironmentVariable)",
            "FirstRawEnvironmentValue(ForceBlessingEnvironmentVariable, LegacyForceBlessingEnvironmentVariable)",
            "AncientFeatureGate.IsTruthyEnvironmentVariable(DisableEnvironmentVariable)",
            "AncientFeatureGate.IsTruthyEnvironmentVariable(LegacyDisableEnvironmentVariable)");
        Assert.DoesNotContain("return IsTruthy(value);", gate, StringComparison.Ordinal);

        AssertSourceContains(
            ancient,
            "ModAncientEventTemplate",
            "HarmonyPatch(typeof(Hive), nameof(Hive.GetUnlockedAncients))",
            "MorviFeatureGate.ShouldForceMorvi",
            "unlockedAncients = [morvi]",
            "list.Add(morvi)",
            "ExpectedInitialOptionCount = 3",
            "candidates.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList()",
            "AncientInitialOptionReroll.CanOffer",
            "MorviAssetPaths.BackgroundScene");
        Assert.Contains("MorviFeatureModule", featureRegistry, StringComparison.Ordinal);
        AssertSourceContains(
            initializer,
            "ModHelper.SubscribeForRunStateHooks",
            "ModHelper.SubscribeForCombatStateHooks",
            "ModelDb.GetById<MorviRunHook>",
            "ModelDb.GetById<MorviCombatHook>",
            "default-on");
        AssertSourceContains(
            savedFields,
            "SavedAttachedState<Player, string> MorviStateKey",
            "SavedAttachedState<CardModel, string> MorviDeckStateKey",
            "SavedAttachedState<CardModel, bool> MorviBorrowedAncientCard",
            "SavedAttachedState<CardModel, bool> MorviOpenBookSealedCard");

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
            "MorviBraveryPagePower : ModTemporaryAppliedPowerTemplate<MorviArchiveBraveryPage, StrengthPower>",
            "MorviDexterityPagePower : ModTemporaryAppliedPowerTemplate<MorviArchiveDexterityPage, DexterityPower>",
            "CustomIconPath => MorviAssetPaths.ArchivePagePowerIcon",
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

    private static string ReadMorviSource() =>
        ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi");

}
