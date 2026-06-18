using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
{
    [Fact]
    public void AncientDirectDeckGainFeedbackFlashesSourceRelicAndCardPreview()
    {
        var feedbackSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "SpirePlusFeedback.cs");
        var ancientSource = ReadSourceTree("EZMicroBalanceCode", "Ancients");

        AssertSourceContains(
            feedbackSource,
            "RelicTriggerSfx = \"event:/sfx/ui/relic_activate_general\"",
            "sourceRelic.Flash()",
            "NRelicFlashVfx.Create(sourceRelic)",
            "AboveTopBarVfxContainer.AddChildSafely(flashVfx)",
            "public static void ConfirmRelicPayoff(RelicModel? sourceRelic)",
            "models.Insert(0, sourceRelic)",
            "CardCmd.PreviewCardPileAdd(successfulAdds, seconds)",
            "NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short)");

        AssertSourceContains(
            ancientSource,
            "SpirePlusFeedback.PreviewDeckAdds(results, paelsHorn, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(result, jewelryBox, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(result, preservedFog, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(results, cape, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(results, sealOfGold, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, paelsTooth)",
            "SpirePlusFeedback.PreviewDeckAdds(successfulAdds, sereTalon, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResults, player.GetRelic<UrdaMoltingOptionRelic>(), 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<UrdaSeedbedOptionRelic>(), 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<UrdaTrialBranchOptionRelic>(), 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<UrdaSeedBankOptionRelic>(), 2f)",
            "SpirePlusFeedback.ConfirmRelicPayoff(eliteRoot)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<MorviForbiddenLoanOptionRelic>(), 2f)");
    }
}
