using Godot;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

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

[HarmonyPatch(typeof(NClickableControl), nameof(NClickableControl._GuiInput))]
internal static class UrdaRootSightDisabledMapPointClickPatch
{
    [HarmonyPrefix]
    private static bool Prefix(NClickableControl __instance, InputEvent inputEvent)
    {
        if (__instance is not NMapPoint mapPoint ||
            !UrdaBlessingService.IsRootSightSelectionActive ||
            inputEvent is not InputEventMouseButton { ButtonIndex: MouseButton.Left } mouseButton ||
            mouseButton.IsPressed())
        {
            return true;
        }

        _ = TaskHelper.RunSafely(UrdaBlessingService.TryCommitRootSightSelection(mapPoint.Point));
        __instance.GetViewport()?.SetInputAsHandled();
        return false;
    }
}

[HarmonyPatch(typeof(NMapScreen), nameof(NMapScreen.Close))]
internal static class UrdaRootSightMapClosePatch
{
    [HarmonyPrefix]
    private static void Prefix() =>
        UrdaBlessingService.CancelRootSightSelection();
}
