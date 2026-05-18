using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
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

        UrdaRootSightMapPreviewVisuals.ApplyPreviewIcon(__instance);
        UrdaRootSightMapPreviewVisuals.ApplyQuestIcon(__instance);
    }
}

[HarmonyPatch(typeof(NNormalMapPoint), "OnFocus")]
internal static class UrdaRootSightMapHoverPatch
{
    [HarmonyPriority(Priority.Last)]
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

[HarmonyPatch(typeof(NNormalMapPoint), "RefreshState")]
internal static class UrdaRootSightMapPreviewIconPatch
{
    [HarmonyPriority(Priority.Last)]
    [HarmonyPostfix]
    private static void Postfix(NNormalMapPoint __instance)
    {
        UrdaRootSightMapPreviewVisuals.ApplyPreviewIcon(__instance);
        UrdaRootSightMapPreviewVisuals.ApplyQuestIcon(__instance);
    }
}

[HarmonyPatch(typeof(NNormalMapPoint), "RefreshMarkedIconVisibility")]
internal static class UrdaRootSightMapQuestIconPatch
{
    [HarmonyPriority(Priority.Last)]
    [HarmonyPostfix]
    private static void Postfix(NNormalMapPoint __instance) =>
        UrdaRootSightMapPreviewVisuals.ApplyQuestIcon(__instance);
}
