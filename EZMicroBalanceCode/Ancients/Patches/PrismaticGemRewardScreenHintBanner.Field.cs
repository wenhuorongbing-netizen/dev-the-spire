namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static partial class PrismaticGemRewardScreenHintPatch
{
    private static readonly System.Reflection.FieldInfo? BannerField =
        AccessTools.Field(typeof(MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen), "_banner");

    private static bool BannerFieldSuccessLogged;

    private static bool BannerFieldFailureLogged;

    private static bool TryApplyBannerFieldHint(
        MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen screen,
        string hintText)
    {
        if (!TryGetCompatibleBannerField(out var bannerField, out var reason))
        {
            WarnOnce(
                ref BannerFieldFailureLogged,
                $"[Spire Plus] PrismaticGem reward-screen hint fallback: private _banner field unavailable ({reason}); trying {BannerNodePath}.");
            return false;
        }

        try
        {
            if (bannerField.GetValue(screen) is not MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner banner)
            {
                WarnOnce(
                    ref BannerFieldFailureLogged,
                    $"[Spire Plus] PrismaticGem reward-screen hint fallback: private _banner field resolved but did not contain a banner instance; trying {BannerNodePath}.");
                return false;
            }

            if (!banner.IsInsideTree())
            {
                WarnOnce(
                    ref BannerFieldFailureLogged,
                    $"[Spire Plus] PrismaticGem reward-screen hint fallback: private _banner field resolved to a detached banner; trying {BannerNodePath}.");
                return false;
            }

            banner.ChangeText(hintText);
            InfoOnce(
                ref BannerFieldSuccessLogged,
                "[Spire Plus] PrismaticGem reward-screen hint applied through the guarded private _banner field; visual placement still requires manual gameplay verification.");
            return true;
        }
        catch (Exception exception)
        {
            WarnOnce(
                ref BannerFieldFailureLogged,
                $"[Spire Plus] PrismaticGem reward-screen hint fallback: private _banner access failed with {exception.GetType().Name}; trying {BannerNodePath}.");
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
}
