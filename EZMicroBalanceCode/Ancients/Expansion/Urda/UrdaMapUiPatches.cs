using Godot;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
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

internal static class UrdaRootSightMapHoverPatch
{
    // Root Eyes contributes one hover entry to the shared map hover composer.
    // Keeping a single owner tip set prevents Firemark, Banner, and Root Eyes
    // from deleting or duplicating each other's map text.
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
