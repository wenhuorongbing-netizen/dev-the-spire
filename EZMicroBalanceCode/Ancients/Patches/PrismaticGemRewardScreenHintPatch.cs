namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(
    typeof(MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen),
    nameof(MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen.RefreshOptions))]
internal static partial class PrismaticGemRewardScreenHintPatch
{
    [HarmonyPostfix]
    private static void Postfix(
        MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen __instance,
        IReadOnlyList<CardCreationResult> options)
    {
        if (!PrismaticGemRewardPatch.HasPrismaticAllOffColorHint(options))
        {
            return;
        }

        ApplyRewardScreenHint(__instance);
    }

    private static void ApplyRewardScreenHint(
        MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen screen)
    {
        var hintText = new LocString("relics", "PRISMATIC_GEM.rewardScreenHint").GetFormattedText();
        if (TryApplyBannerFieldHint(screen, hintText))
        {
            ConfirmBannerNodeHintAfterFieldSuccess(screen, hintText);
            return;
        }

        if (TryApplyBannerNodeHint(screen, hintText))
        {
            return;
        }

        WarnOnce(
            ref BannerUnavailableLogged,
            "[EZMicroBalance] PrismaticGem reward-screen hint unavailable: private _banner and UI/Banner fallback both failed; visible all-off-color cards and the Prismatic Gem relic hover count remain available for manual confirmation.");
    }
}
