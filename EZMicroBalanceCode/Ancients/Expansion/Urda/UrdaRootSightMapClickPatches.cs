using Godot;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed class UrdaRootSightMapPointClickPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "urda-root-sight-map-point-click";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Commit Root Sight selection when a travel-enabled map point is released";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NMapPoint), "OnRelease")];

    [HarmonyPrefix]
    private static bool Prefix(NMapPoint __instance)
    {
        if (!TryConsumeRootSightMapPointRelease(__instance, setInputHandled: false))
        {
            return true;
        }

        return false;
    }

    internal static bool TryConsumeRootSightMapPointRelease(
        NMapPoint mapPoint,
        bool setInputHandled)
    {
        if (!UrdaBlessingService.IsRootSightSelectionActive)
        {
            return false;
        }

        _ = TaskHelper.RunSafely(UrdaBlessingService.TryCommitRootSightSelection(mapPoint.Point));
        if (setInputHandled)
        {
            mapPoint.GetViewport()?.SetInputAsHandled();
        }

        return true;
    }
}

internal sealed class UrdaRootSightDisabledMapPointClickPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "urda-root-sight-disabled-map-point-click";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Commit Root Sight selection from disabled map points that still receive mouse release input";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NClickableControl), nameof(NClickableControl._GuiInput), [typeof(InputEvent)])];

    [HarmonyPrefix]
    private static bool Prefix(NClickableControl __instance, InputEvent inputEvent)
    {
        if (__instance is not NMapPoint mapPoint ||
            inputEvent is not InputEventMouseButton { ButtonIndex: MouseButton.Left } mouseButton ||
            mouseButton.IsPressed())
        {
            return true;
        }

        // NMapPoint disables untravelable future nodes, and NClickableControl
        // normally drops disabled mouse releases. Root Sight is a deliberate
        // map-inspection mode, so it consumes only the left-button release that
        // would otherwise be lost and lets every other input continue normally.
        if (!UrdaRootSightMapPointClickPatch.TryConsumeRootSightMapPointRelease(
            mapPoint,
            setInputHandled: true))
        {
            return true;
        }

        return false;
    }
}

internal sealed class UrdaRootSightMapClosePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "urda-root-sight-map-close";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Cancel pending Root Sight selection when the map screen closes";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NMapScreen), nameof(NMapScreen.Close), [typeof(bool)])];

    [HarmonyPrefix]
    private static void Prefix() =>
        UrdaBlessingService.CancelRootSightSelection();
}
