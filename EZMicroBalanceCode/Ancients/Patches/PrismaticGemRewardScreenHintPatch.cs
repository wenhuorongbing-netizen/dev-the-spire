using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed partial class PrismaticGemRewardScreenHintPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "prismatic-gem-reward-screen-hint";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Show Prismatic Gem all-off-color reward-screen banner hints after card reward options refresh";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
    [
        new ModPatchTarget(
            typeof(NCardRewardSelectionScreen),
            nameof(NCardRewardSelectionScreen.RefreshOptions),
            [typeof(IReadOnlyList<CardCreationResult>), typeof(IReadOnlyList<CardRewardAlternative>)])
    ];

    [HarmonyPostfix]
    private static void Postfix(
        NCardRewardSelectionScreen __instance,
        IReadOnlyList<CardCreationResult> options)
    {
        if (!PrismaticGemRewardPatch.HasPrismaticAllOffColorHint(options))
        {
            return;
        }

        ApplyRewardScreenHint(__instance);
    }

    private static void ApplyRewardScreenHint(NCardRewardSelectionScreen screen)
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
            "[Spire Plus] PrismaticGem reward-screen hint unavailable: private _banner and UI/Banner fallback both failed; visible all-off-color cards and the Prismatic Gem relic hover count remain available for manual confirmation.");
    }
}

