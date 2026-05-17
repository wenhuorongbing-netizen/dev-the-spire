using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

[HarmonyPatch(typeof(NNormalMapPoint), nameof(NNormalMapPoint._Ready))]
internal static class UrdaRootSightMapQuestIconInputPatch
{
    [HarmonyPostfix]
    private static void Postfix(NNormalMapPoint __instance)
    {
        var questIcon = __instance.GetNodeOrNull<TextureRect>("%QuestIcon");
        if (questIcon != null)
        {
            questIcon.MouseFilter = Control.MouseFilterEnum.Ignore;
        }
    }
}

[HarmonyPatch(typeof(NNormalMapPoint), "OnFocus")]
internal static class UrdaRootSightMapHoverPatch
{
    [HarmonyPostfix]
    private static void Postfix(NNormalMapPoint __instance)
    {
        if (!UrdaBlessingService.TryGetRootSightHoverTip(__instance.Point, out var hoverTip))
        {
            return;
        }

        NHoverTipSet.Remove(__instance);
        var hoverTipSet = NHoverTipSet.CreateAndShow(
            __instance,
            hoverTip);
        if (hoverTipSet != null)
        {
            Callable.From(() => hoverTipSet.SetAlignment(__instance, HoverTip.GetHoverTipAlignment(__instance))).CallDeferred();
        }
    }
}

[HarmonyPatch(typeof(NMapPoint), "OnRelease")]
internal static class UrdaRootSightMapPointClickPatch
{
    [HarmonyPrefix]
    private static bool Prefix(NMapPoint __instance)
    {
        if (!UrdaBlessingService.IsRootSightSelectionActive)
        {
            return true;
        }

        _ = TaskHelper.RunSafely(UrdaBlessingService.TryCommitRootSightSelection(__instance.Point));
        return false;
    }
}
