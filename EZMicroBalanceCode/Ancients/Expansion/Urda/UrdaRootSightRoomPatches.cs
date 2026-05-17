namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

using MegaCrit.Sts2.Core.Map;

[HarmonyPatch(typeof(RunManager), "RollRoomTypeFor")]
internal static class UrdaRootSightRollRoomTypePatch
{
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

[HarmonyPatch(typeof(RunManager), "CreateRoom")]
internal static class UrdaRootSightCreateRoomPatch
{
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
        }
    }
}
