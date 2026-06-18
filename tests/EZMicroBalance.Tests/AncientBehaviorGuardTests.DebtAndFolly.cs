using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
{
    [Fact]
    public void DebtAndFollyPlayerTextMatchSourceBehavior()
    {
        var debtSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "DebtAndCardPatches.cs");
        var vakuSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var cards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");

        Assert.Equal("Debt", cards["DEBT.title"]);
        Assert.Equal("Exhaust. When Exhausted, lose 5 Gold.", cards["DEBT.description"]);
        Assert.DoesNotContain("turn", cards["DEBT.description"], StringComparison.OrdinalIgnoreCase);

        Assert.Equal("Folly", cards["FOLLY.title"]);
        Assert.Equal("Unplayable. Innate. Eternal.", cards["FOLLY.description"]);
        Assert.DoesNotContain("Ethereal", cards["FOLLY.description"], StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Retain", cards["FOLLY.description"], StringComparison.OrdinalIgnoreCase);

        AssertSourceContains(
            debtSource,
            "DebtFromSavePatch",
            "__result = new CardKeyword[] { CardKeyword.Exhaust }",
            "__result = new DynamicVar[] { new GoldVar(5) }",
            "DebtTurnEndEffectPatch",
            "__result = false",
            "DebtTurnEndInHandPatch",
            "__result = Task.CompletedTask",
            "debt.ExhaustOnNextPlay = true",
            "Math.Min(5, debt.Owner.Gold)",
            "PlayerCmd.LoseGold(goldToLose, debt.Owner)");

        AssertSourceContains(
            vakuSource,
            "new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 4)",
            "AncientCardHelpers.RemoveKeywords(folly, CardKeyword.Ethereal, CardKeyword.Retain)",
            "__result = new[] { CardKeyword.Unplayable, CardKeyword.Eternal, CardKeyword.Innate }");
    }
}
