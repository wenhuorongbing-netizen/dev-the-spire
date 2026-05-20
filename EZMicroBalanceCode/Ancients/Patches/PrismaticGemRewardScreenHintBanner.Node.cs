namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static partial class PrismaticGemRewardScreenHintPatch
{
    private static bool BannerNodeFallbackLogged;

    private static bool BannerNodeConfirmationLogged;

    private static bool BannerNodeConfirmationFailureLogged;

    private static bool TryApplyBannerNodeHint(
        MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen screen,
        string hintText)
    {
        try
        {
            var banner = screen.GetNodeOrNull<MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner>(BannerNodePath);
            if (banner == null)
            {
                WarnOnce(
                    ref BannerNodeFallbackLogged,
                    $"[EZMicroBalance] PrismaticGem reward-screen hint fallback unavailable: {BannerNodePath} node was not found.");
                return false;
            }

            banner.ChangeText(hintText);
            InfoOnce(
                ref BannerNodeFallbackLogged,
                $"[EZMicroBalance] PrismaticGem reward-screen hint fallback applied through {BannerNodePath} node lookup.");
            return true;
        }
        catch (Exception exception)
        {
            WarnOnce(
                ref BannerNodeFallbackLogged,
                $"[EZMicroBalance] PrismaticGem reward-screen hint fallback through {BannerNodePath} failed with {exception.GetType().Name}.");
            return false;
        }
    }

    private static void ConfirmBannerNodeHintAfterFieldSuccess(
        MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen screen,
        string hintText)
    {
        // Reflection is a private API dependency; also update the public node path when
        // available so a stale reflected field cannot be the only hint surface.
        try
        {
            var banner = screen.GetNodeOrNull<MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner>(BannerNodePath);
            if (banner == null)
            {
                WarnOnce(
                    ref BannerNodeConfirmationFailureLogged,
                    $"[EZMicroBalance] PrismaticGem reward-screen hint confirmation unavailable after private _banner update: {BannerNodePath} node was not found; visual placement still requires manual gameplay verification.");
                return;
            }

            banner.ChangeText(hintText);
            InfoOnce(
                ref BannerNodeConfirmationLogged,
                $"[EZMicroBalance] PrismaticGem reward-screen hint also applied through {BannerNodePath} node lookup after private _banner field path; visual placement still requires manual gameplay verification.");
        }
        catch (Exception exception)
        {
            WarnOnce(
                ref BannerNodeConfirmationFailureLogged,
                $"[EZMicroBalance] PrismaticGem reward-screen hint confirmation after private _banner update failed with {exception.GetType().Name}; visual placement still requires manual gameplay verification.");
        }
    }
}
