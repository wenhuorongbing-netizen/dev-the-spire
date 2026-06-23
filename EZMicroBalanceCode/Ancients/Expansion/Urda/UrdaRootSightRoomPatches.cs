namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

using MegaCrit.Sts2.Core.Map;
using STS2RitsuLib.Patching.Models;

internal sealed class UrdaRootSightRollRoomTypePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "urda-root-sight-roll-room-type";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Apply committed Root Sight preview room type during room rolling";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RunManager), "RollRoomTypeFor")];

    [HarmonyPrefix]
    private static bool Prefix(RunManager __instance, MapPointType pointType, ref RoomType __result)
    {
        if (!UrdaBlessingService.TryGetRootSightRoomTypeForCurrentPoint(__instance, pointType, out var roomType))
        {
            return true;
        }

        __result = roomType;
        return false;
    }
}

internal sealed class UrdaRootSightCreateRoomPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "urda-root-sight-create-room";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Apply committed Root Sight preview room model during room creation";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RunManager), "CreateRoom")];

    [HarmonyPrefix]
    private static void Prefix(
        RunManager __instance,
        RoomType roomType,
        MapPointType mapPointType,
        ref AbstractModel? model)
    {
        if (model != null)
        {
            return;
        }

        if (UrdaBlessingService.TryGetRootSightModelForCurrentPoint(__instance, roomType, mapPointType, out var previewModel))
        {
            model = previewModel;
            return;
        }

        UrdaBlessingService.AvoidRootSightReservedModelForCurrentNonPreviewRoom(__instance, roomType, mapPointType);
    }
}
