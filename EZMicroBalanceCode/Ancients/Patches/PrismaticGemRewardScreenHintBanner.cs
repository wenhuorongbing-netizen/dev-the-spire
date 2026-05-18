namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static partial class PrismaticGemRewardScreenHintPatch
{
    private const string BannerNodePath = "UI/Banner";

    private static readonly System.Reflection.FieldInfo? BannerField =
        AccessTools.Field(typeof(MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen), "_banner");

    private static bool BannerFieldSuccessLogged;

    private static bool BannerFieldFailureLogged;

    private static bool BannerNodeFallbackLogged;

    private static bool BannerNodeConfirmationLogged;

    private static bool BannerNodeConfirmationFailureLogged;

    private static bool BannerUnavailableLogged;

    private static bool TryApplyBannerFieldHint(
        MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen screen,
        string hintText)
    {
        if (!TryGetCompatibleBannerField(out var bannerField, out var reason))
        {
            WarnOnce(
                ref BannerFieldFailureLogged,
                $"[EZMicroBalance] PrismaticGem reward-screen hint fallback: private _banner field unavailable ({reason}); trying {BannerNodePath}.");
            return false;
        }

        try
        {
            if (bannerField.GetValue(screen) is not MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner banner)
            {
                WarnOnce(
                    ref BannerFieldFailureLogged,
                    $"[EZMicroBalance] PrismaticGem reward-screen hint fallback: private _banner field resolved but did not contain a banner instance; trying {BannerNodePath}.");
                return false;
            }

            if (!banner.IsInsideTree())
            {
                WarnOnce(
                    ref BannerFieldFailureLogged,
                    $"[EZMicroBalance] PrismaticGem reward-screen hint fallback: private _banner field resolved to a detached banner; trying {BannerNodePath}.");
                return false;
            }

            banner.ChangeText(hintText);
            InfoOnce(
                ref BannerFieldSuccessLogged,
                "[EZMicroBalance] PrismaticGem reward-screen hint applied through the guarded private _banner field; visual placement still requires manual gameplay verification.");
            return true;
        }
        catch (Exception exception)
        {
            WarnOnce(
                ref BannerFieldFailureLogged,
                $"[EZMicroBalance] PrismaticGem reward-screen hint fallback: private _banner access failed with {exception.GetType().Name}; trying {BannerNodePath}.");
            return false;
        }
    }

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

    private static bool TryGetCompatibleBannerField(
        out System.Reflection.FieldInfo bannerField,
        out string reason)
    {
        if (BannerField == null)
        {
            bannerField = null!;
            reason = "field not found";
            return false;
        }

        if (!typeof(MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner).IsAssignableFrom(BannerField.FieldType))
        {
            bannerField = null!;
            reason = $"field type was {BannerField.FieldType.FullName}";
            return false;
        }

        bannerField = BannerField;
        reason = string.Empty;
        return true;
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

    private static void InfoOnce(ref bool logged, string message)
    {
        if (logged)
        {
            return;
        }

        logged = true;
        MainFile.Logger.Info(message);
    }

    private static void WarnOnce(ref bool logged, string message)
    {
        if (logged)
        {
            return;
        }

        logged = true;
        MainFile.Logger.Warn(message);
    }
}
