using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

[HarmonyPatch(typeof(NNormalMapPoint), "OnFocus")]
internal static class BannerRoomMapHoverPatch
{
    [HarmonyPostfix]
    private static void Postfix(NNormalMapPoint __instance)
    {
        if (!__instance.Point.Quests.Any(quest => quest is BannerRoomMapQuestMarker))
        {
            return;
        }

        var metadata = AscensionMapService.TryGetMetadata(__instance.Point);
        if (metadata?.Banner == null)
        {
            return;
        }

        var hoverTipSet = NHoverTipSet.CreateAndShow(__instance, CreateHoverTip(metadata.Banner.Value));
        if (hoverTipSet != null)
        {
            Callable.From(() => hoverTipSet.SetAlignment(__instance, HoverTip.GetHoverTipAlignment(__instance))).CallDeferred();
        }
    }

    private static HoverTip CreateHoverTip(BannerKind banner)
    {
        var locKey = banner switch
        {
            BannerKind.Vanguard => "BANNER_VANGUARD",
            BannerKind.Shieldwall => "BANNER_SHIELDWALL",
            BannerKind.BloodPrize => "BANNER_BLOOD_PRIZE",
            BannerKind.PressingLine => "BANNER_PRESSING_LINE",
            BannerKind.LastStand => "BANNER_LAST_STAND",
            _ => "BANNER_ROOM"
        };

        return new HoverTip(
            new LocString("ascension", $"{locKey}.title"),
            new LocString("ascension", $"{locKey}.description"));
    }
}
