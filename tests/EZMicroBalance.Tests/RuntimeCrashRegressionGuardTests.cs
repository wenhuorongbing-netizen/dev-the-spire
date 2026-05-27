using Xunit;

namespace EZMicroBalance.Tests;

public sealed class RuntimeCrashRegressionGuardTests
{
    [Fact]
    public void UrdaSeedbedPlantingIsQueuedOutOfDrawPileMutationPath()
    {
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRunHook.cs");
        var seedbedState = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedbedState.cs");
        var cardPileCmd = ReadRepoText("source code", "src", "Core", "Commands", "CardPileCmd.cs");

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
            cardPileCmd,
            "Hook.AfterCardChangedPiles(cardAdded.Owner.RunState, cardAdded.CombatState, cardAdded, item3.oldPile?.Type ?? PileType.None, clonedBy);");
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
            "EndSeedbedDraw(player)");
        Assert.DoesNotContain("await UrdaBlessingService.TryPlantSeedbedCardFromHand(card, \"card entered hand\")", runHook, StringComparison.Ordinal);
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
