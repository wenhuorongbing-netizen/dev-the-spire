using Xunit;

namespace EZMicroBalance.Tests;

public sealed class RuntimeCrashRegressionGuardTests
{
    [Fact]
    public void UrdaSeedbedPlantingIsQueuedOutOfDrawPileMutationPath()
    {
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRunHook.cs");
        var seedbedCombat = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedbedCombat.cs");
        var seedbedState = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedbedState.cs");
        var seedbedAfterCardDrawnPatch = ReadRepoText(
            "EZMicroBalanceCode",
            "Ancients",
            "Expansion",
            "Urda",
            "UrdaSeedbedAfterCardDrawnPatch.cs");

        AssertSourceContains(
            runHook,
            "_ = UrdaBlessingService.QueueSeedbedPlantFromHand(card, \"card entered hand\")",
            "UrdaBlessingService.SyncPersistentState(card.Owner)");
        AssertSourceContains(
            seedbedState,
            "await Task.Yield();",
            "while (IsSeedbedDrawInProgress(player))",
            "!IsSeedbedSeedableCard(card)",
            "while (HasAny(player, out var request))",
            "if (!await PlantSeedbedCard(request.Card, seedbedState, request.Source))",
            "state.IsProcessing = false;",
            "TaskHelper.RunSafely(ProcessSeedbedPlantingQueue(player, pending))");
        AssertSourceContains(
            seedbedCombat,
            "if (IsSeedbedDrawInProgress(player))",
            "return await QueueSeedbedPlantFromHand(card, source)");
        var seedbedDrawPatch = ReadRepoText(
            "EZMicroBalanceCode",
            "Ancients",
            "Expansion",
            "Urda",
            "UrdaSeedbedCardPileDrawPatch.cs");
        AssertSourceContains(
            seedbedDrawPatch,
            "HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Draw), typeof(PlayerChoiceContext), typeof(decimal), typeof(Player), typeof(bool))",
            "BeginSeedbedDraw(player)",
            "EndSeedbedDraw(player)",
            "try",
            "finally",
            "EndSeedbedDraw(player)");
        AssertSourceContains(
            seedbedAfterCardDrawnPatch,
            "HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardDrawn))",
            "__result = Task.CompletedTask",
            "return false;");
        Assert.DoesNotContain("private static bool Prefix(CardModel card)", seedbedAfterCardDrawnPatch, StringComparison.Ordinal);
        Assert.DoesNotContain("await UrdaBlessingService.TryPlantSeedbedCardFromHand(card, \"card entered hand\")", runHook, StringComparison.Ordinal);
    }

    [LocalSourceFact]
    public void CardPileDrawCallsAfterCardChangedPilesAfterOwnershipMove()
    {
        var cardPileCmd = ReadLocalCoreText("Commands", "CardPileCmd.cs");

        AssertSourceContains(
            cardPileCmd,
            "Hook.AfterCardChangedPiles(cardAdded.Owner.RunState, cardAdded.CombatState, cardAdded, item3.oldPile?.Type ?? PileType.None, clonedBy);");
    }

    [Fact]
    public void RoyalDecreeDoesNotTryToEnchantUnplayableBoundCards()
    {
        var source = ReadRepoText(
            "EZMicroBalanceCode",
            "Ascension",
            "Combat",
            "AscensionCombatModifierService.BossSeals.ChosenDecree.Cards.cs");

        Assert.Contains("card.Type is CardType.Attack or CardType.Skill or CardType.Power", source, StringComparison.Ordinal);
        Assert.Contains("!card.Keywords.Contains(CardKeyword.Unplayable)", source, StringComparison.Ordinal);
        Assert.Contains("ModelDb.Enchantment<RoyalDecreeEnchantment>().CanEnchant(card)", source, StringComparison.Ordinal);
        Assert.Contains("catch (InvalidOperationException ex)", source, StringComparison.Ordinal);
        Assert.Contains("skipped Royal Decree mark for un-enchantable Bound card", source, StringComparison.Ordinal);
    }

    [Fact]
    public void CombatHandInputIgnoresOnlyTheObservedStaleIndexCrash()
    {
        var source = ReadRepoText(
            "EZMicroBalanceCode",
            "Ascension",
            "Patches",
            "CombatHandInputSafetyPatches.cs");

        Assert.Contains("HarmonyPatch(typeof(NPlayerHand), nameof(NPlayerHand._UnhandledInput))", source, StringComparison.Ordinal);
        Assert.Contains("__exception is ArgumentOutOfRangeException", source, StringComparison.Ordinal);
        Assert.Contains("return null;", source, StringComparison.Ordinal);
        Assert.Contains("return __exception;", source, StringComparison.Ordinal);
    }
}
