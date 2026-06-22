using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
{
    [Fact]
    public void SovereignBladeJadeBoonsApplyOnPlayAndAreExplainedByForge()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SovereignBladeForgePatches.cs");
        var cardsEng = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var cardsZhs = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");
        var staticEng = JsonStringMap("EZMicroBalance", "localization", "eng", "static_hover_tips.json");
        var staticZhs = JsonStringMap("EZMicroBalance", "localization", "zhs", "static_hover_tips.json");

        AssertSourceContains(
            source,
            "public const decimal Amount = 3m",
            "[HarmonyPatch(typeof(SovereignBlade), \"OnPlay\")]",
            "await original;",
            "PowerCmd.Apply<StrengthPower>(choiceContext, owner, Amount, owner, blade)",
            "PowerCmd.Apply<DexterityPower>(choiceContext, owner, Amount, owner, blade)",
            "PowerCmd.Apply<PlatingPower>(choiceContext, owner, Amount, owner, blade)",
            "PowerCmd.Apply<RegenPower>(choiceContext, owner, Amount, owner, blade)",
            "PowerCmd.Apply<VigorPower>(choiceContext, owner, Amount, owner, blade)",
            "static string IPatchMethod.PatchId => \"sovereign-blade-jade-boons-hover-tips\"",
            "new ModPatchTarget(typeof(CardModel), nameof(CardModel.HoverTips), MethodType.Getter)",
            "HoverTipFactory.FromPower<StrengthPower>((int)Amount)",
            "HoverTipFactory.FromPower<DexterityPower>((int)Amount)",
            "HoverTipFactory.FromPower<PlatingPower>((int)Amount)",
            "HoverTipFactory.FromPower<RegenPower>((int)Amount)",
            "HoverTipFactory.FromPower<VigorPower>((int)Amount)");

        AssertSovereignBladeText(cardsEng["SOVEREIGN_BLADE.description"], "Strength", "Dexterity", "Plating", "Regen", "Vigor");
        AssertSovereignBladeText(cardsZhs["SOVEREIGN_BLADE.description"], "\u529b\u91cf", "\u654f\u6377", "\u8986\u7532", "\u518d\u751f", "\u6d3b\u529b");
        AssertSovereignBladeText(staticEng["FORGE.description"], "Sovereign Blade", "Strength", "Dexterity", "Plating", "Regen", "Vigor");
        AssertSovereignBladeText(staticZhs["FORGE.description"], "\u541b\u738b\u4e4b\u5251", "\u529b\u91cf", "\u654f\u6377", "\u8986\u7532", "\u518d\u751f", "\u6d3b\u529b");
    }

    private static void AssertSovereignBladeText(string value, params string[] requiredTerms)
    {
        Assert.True(CountOccurrences(value, "[blue]3[/blue]") >= 5, "Sovereign Blade text should show all five 3-point jade boons.");
        foreach (var term in requiredTerms)
        {
            Assert.Contains(term, value, StringComparison.Ordinal);
        }
    }
}
