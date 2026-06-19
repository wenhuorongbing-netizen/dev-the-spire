using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientExpansionReleaseCoverageGuardTests
{
    [Fact]
    public void MorviV22IsDefaultOnGatedLocalizedAndPowerSafe()
    {
        var featureRegistry = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "SpirePlusFeatureRegistry.cs");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var morviGate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviFeatureGate.cs");
        var morviInitializer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviInitializer.cs");
        var morviAncient = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviAncient.cs");
        var morviOptionRelics = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviOptionRelics.cs");
        var morviPowers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviPowers.cs");
        var morviBlessings = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingIds.cs");
        var morviSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi");
        var morviCards = morviSource;
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var zhsCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");

        Assert.Contains("MorviFeatureModule", featureRegistry, StringComparison.Ordinal);
        Assert.Contains("MorviStateKey", savedFields, StringComparison.Ordinal);
        Assert.Contains("EZMicroBalanceMorviStateKey", savedFields, StringComparison.Ordinal);
        Assert.Contains("MorviBorrowedAncientCard", savedFields, StringComparison.Ordinal);
        Assert.Contains("MorviOpenBookSealedCard", savedFields, StringComparison.Ordinal);

        AssertSourceContains(
            morviGate,
            "DisableEnvironmentVariable = \"SPIREPLUS_DISABLE_MORVI\"",
            "LegacyDisableEnvironmentVariable = \"EZMB_DISABLE_MORVI\"",
            "ForceAncientEnvironmentVariable = \"SPIREPLUS_FORCE_ANCIENT\"",
            "LegacyForceAncientEnvironmentVariable = \"EZMB_FORCE_ANCIENT\"",
            "ForceBlessingEnvironmentVariable = \"SPIREPLUS_FORCE_MORVI_BLESSING\"",
            "LegacyForceBlessingEnvironmentVariable = \"EZMB_FORCE_MORVI_BLESSING\"",
            "ShouldForceMorvi",
            "AncientFeatureGate.IsTruthyEnvironmentVariable(DisableEnvironmentVariable)",
            "AncientFeatureGate.IsTruthyEnvironmentVariable(LegacyDisableEnvironmentVariable)");
        Assert.DoesNotContain("return IsTruthy(value);", morviGate, StringComparison.Ordinal);

        AssertSourceContains(
            morviInitializer,
            "ModHelper.SubscribeForRunStateHooks",
            "ModHelper.SubscribeForCombatStateHooks",
            "default-on",
            "ModelDb.GetById<MorviRunHook>",
            "ModelDb.GetById<MorviCombatHook>");
        AssertSourceContains(
            morviSource,
            "CustomAncientModel",
            "HarmonyPatch(typeof(Hive), nameof(Hive.GetUnlockedAncients))",
            "MorviFeatureGate.IsMorviEnabled(unlockState)",
            "MorviFeatureGate.ShouldForceMorvi",
            "ModelDb.AncientEvent<EzmbMorvi>()",
            "ExpectedInitialOptionCount = 3",
            "Where(IsCurrentlyAvailableOption)",
            "MorviBlessingService.HasForbiddenLoanCandidates(Owner)",
            "MorviBlessingService.TrySetSelectedBlessing",
            "candidates.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList()",
            "AncientInitialOptionReroll.CanOffer",
            "OptionWithRelic<MorviForbiddenLoanOptionRelic>",
            "MorviBlessingIds.ForbiddenLoan",
            "MorviBlessingIds.MisprintPress",
            "MorviBlessingIds.RedInkOverdraft",
            "MorviBlessingIds.OverdueLibrary",
            "MorviBlessingIds.OpenBookExam",
            "MorviBlessingIds.Paperstorm",
            "MorviBlessingIds.BlueprintProof",
            "MorviBlessingIds.DebtSettlement",
            "MorviAssetPaths.MapIcon",
            "MorviAssetPaths.BackgroundScene");
        Assert.DoesNotContain("Glory.GetUnlockedAncients", morviSource, StringComparison.Ordinal);

        AssertSourceContains(
            morviBlessings,
            "morvi_forbidden_loan",
            "morvi_misprint_press",
            "morvi_red_ink_overdraft",
            "morvi_overdue_library",
            "morvi_open_book_exam",
            "morvi_paperstorm",
            "morvi_blueprint_proof",
            "morvi_debt_settlement");
        AssertSourceContains(
            morviSource,
            "public override bool ShouldReceiveCombatHooks => true",
            "BeforeCombatStart",
            "player.IsActiveForHooks",
            "CardType.Attack or CardType.Skill",
            "!card.IsClone",
            "CardCmd.Upgrade(card, CardPreviewStyle.None)",
            "CardCmd.Downgrade(card)",
            "ForbiddenLoanKeepGoldCost = 180",
            "RedInkOverdraftGoldPerDebt = 12",
            "OverdueLibraryPageCount = 3",
            "OpenBookDraw = 5",
            "PaperstormWastePaperCount = 4",
            "BlueprintProofStacks = 3",
            "DebtSettlementImmediateGold = 220",
            "DebtSettlementStartingDebt = 320",
            "DebtSettlementCombatDue = 40",
            "maximumNonlethalHpLoss = Math.Max(0m, player.Creature.CurrentHp - 1m)",
            "visibleDebtCount = player.Creature.GetPower<MorviOverdraftPower>()?.Amount ?? 0",
            "FindOpenBookSealedCards(player, combatState)",
            "DebtRemaining = Math.Max(0, progress.DebtRemaining - due)");
        Assert.DoesNotContain("CreateClone", morviSource, StringComparison.Ordinal);
        Assert.DoesNotContain("TryAddGeneratedCardToCombat(copy", morviSource, StringComparison.Ordinal);
        Assert.DoesNotContain("CardCmd.AutoPlay", morviSource, StringComparison.Ordinal);

        AssertSourceContains(
            morviCards,
            "MorviArchiveDrawPage",
            "MorviArchiveVeilPage",
            "MorviArchiveBurnPage",
            "MorviArchiveDiscountPage",
            "MorviArchiveBraveryPage",
            "MorviArchiveDexterityPage",
            "MorviRedInkOverdraftCard",
            "MorviWastePaper");
        AssertSourceContains(
            morviOptionRelics,
            "MorviForbiddenLoanOptionRelic",
            "MorviDebtSettlementOptionRelic",
            "IsAllowed(IRunState runState) => false");
        AssertSourceContains(
            morviPowers,
            "MorviDebtPower",
            "MorviProofreadPower",
            "MorviOpenBookPower",
            "MorviOverdraftPower",
            "MorviPaperstormPower");

        foreach (var key in new[]
        {
            "EZMB_MORVI.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_forbidden_loan.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_misprint_press.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_red_ink_overdraft.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_overdue_library.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_open_book_exam.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_paperstorm.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_blueprint_proof.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.title"
        })
        {
            Assert.True(engAncients.ContainsKey(key), $"Missing English Morvi localization: {key}");
            Assert.True(zhsAncients.ContainsKey(key), $"Missing zhs Morvi localization: {key}");
        }

        foreach (var key in new[]
        {
            "EZMB_MORVI_ARCHIVE_DRAW_PAGE.title",
            "EZMB_MORVI_ARCHIVE_VEIL_PAGE.title",
            "EZMB_MORVI_ARCHIVE_BURN_PAGE.title",
            "EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE.title",
            "EZMB_MORVI_ARCHIVE_BRAVERY_PAGE.title",
            "EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE.title",
            "EZMB_MORVI_RED_INK_OVERDRAFT.title",
            "EZMB_MORVI_WASTE_PAPER.title"
        })
        {
            Assert.True(engCards.ContainsKey(key), $"Missing English Morvi card localization: {key}");
            Assert.True(zhsCards.ContainsKey(key), $"Missing zhs Morvi card localization: {key}");
        }
    }
}
