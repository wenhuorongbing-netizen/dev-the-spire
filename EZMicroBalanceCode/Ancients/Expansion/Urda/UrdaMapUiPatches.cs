using Godot;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed class UrdaRootSightMapQuestIconInputPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "urda-root-sight-map-point-ready";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Refresh Root Sight map visuals after normal map point setup";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NNormalMapPoint), nameof(NNormalMapPoint._Ready))];

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

internal sealed class UrdaRootSightMapPreviewIconPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "urda-root-sight-map-refresh-state";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Restore Root Sight preview and quest icons after map point state refresh";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NNormalMapPoint), "RefreshState")];

    [HarmonyPriority(Priority.Last)]
    [HarmonyPostfix]
    private static void Postfix(NNormalMapPoint __instance)
    {
        UrdaRootSightMapPreviewVisuals.ApplyPreviewIcon(__instance);
        UrdaRootSightMapPreviewVisuals.ApplyQuestIcon(__instance);
    }
}

internal sealed class UrdaRootSightMapQuestIconPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "urda-root-sight-map-quest-icon-refresh";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Restore Root Sight quest icon overlay after vanilla marker visibility refresh";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NNormalMapPoint), "RefreshMarkedIconVisibility")];

    [HarmonyPriority(Priority.Last)]
    [HarmonyPostfix]
    private static void Postfix(NNormalMapPoint __instance) =>
        UrdaRootSightMapPreviewVisuals.ApplyQuestIcon(__instance);
}
