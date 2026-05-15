using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

[HarmonyPatch(typeof(NNormalMapPoint), "OnFocus")]
internal static class UrdaRootSightMapHoverPatch
{
    [HarmonyPostfix]
    private static void Postfix(NNormalMapPoint __instance)
    {
        if (!__instance.Point.Quests.Any(quest => quest is UrdaRootSightMapQuestMarker))
        {
            return;
        }

        var hoverTipSet = NHoverTipSet.CreateAndShow(
            __instance,
            new HoverTip(
                new LocString("ancients", "EZMB_URDA.root_sight.map_hover.title"),
                new LocString("ancients", "EZMB_URDA.root_sight.map_hover.description")));
        if (hoverTipSet != null)
        {
            Callable.From(() => hoverTipSet.SetAlignment(__instance, HoverTip.GetHoverTipAlignment(__instance))).CallDeferred();
        }
    }
}
