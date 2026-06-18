using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
{
    [Fact]
    public void TemporaryGeneratedCardPathsCleanUpOrSelfExpire()
    {
        var turnSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var vakuSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            turnSource,
            "CardFactory.GetDistinctForCombat(owner, attackPool, 1, owner.RunState.Rng.CombatCardGeneration)",
            "AncientCardHelpers.ApplyTemporaryCostReduction(generated, 1)",
            "AncientCardHelpers.ApplyKeywords(generated, CardKeyword.Ethereal, CardKeyword.Exhaust)",
            "await AncientCardHelpers.TryAddGeneratedCardToCombat(generated, PileType.Hand, owner)",
            "AncientCardHelpers.RemoveUnpiledCombatCard(generated, combatState)",
            "ModPatchTarget(typeof(Crossbow), nameof(Crossbow.AfterSideTurnStart)",
            "__result = Task.CompletedTask",
            "[HarmonyPatch(typeof(ToastyMittens), nameof(ToastyMittens.BeforeHandDraw))]",
            "CardSelectCmd.FromChooseACardScreen(choiceContext, new[] { topCard }, player, canSkip: true)",
            "if (selected != topCard)",
            "await CardCmd.Exhaust(choiceContext, topCard)",
            "PowerCmd.Apply<StrengthPower>");

        AssertSourceContains(
            vakuSource,
            "var copy = cardPlay.Card.CreateClone()",
            "AncientCardHelpers.ApplyTemporaryCostReduction(copy, 1)",
            "AncientCardHelpers.ApplyKeywords(copy, CardKeyword.Ethereal, CardKeyword.Exhaust)",
            "await AncientCardHelpers.TryAddGeneratedCardToCombat(copy, PileType.Hand, musicBox.Owner)",
            "ConditionalWeakTable<MusicBox, State>",
            "MusicBoxStateTracker.MarkUsed(musicBox)",
            "MusicBoxStateTracker.Reset(__instance)");

        Assert.Contains("skipped card does not linger", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Top draw-pile card can be exhausted for Strength or kept.", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Creates a discounted Ethereal Exhaust copy.", manualMatrix, StringComparison.Ordinal);
    }
}
