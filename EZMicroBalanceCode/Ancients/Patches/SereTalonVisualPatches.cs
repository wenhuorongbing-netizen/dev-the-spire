using Godot;
using MegaCrit.Sts2.Core.Models.Relics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(RelicModel), "get_IconPath")]
internal static class SereTalonIconPathPatch
{
    private static void Postfix(RelicModel __instance, ref string __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPath(__instance, SereTalonVisualAssetPaths.PackedIcon, ref __result);
    }
}

[HarmonyPatch(typeof(RelicModel), "get_PackedIconPath")]
internal static class SereTalonPackedIconPathPatch
{
    private static void Postfix(RelicModel __instance, ref string __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPath(__instance, SereTalonVisualAssetPaths.PackedIcon, ref __result);
    }
}

[HarmonyPatch(typeof(RelicModel), "get_PackedIconOutlinePath")]
internal static class SereTalonPackedIconOutlinePathPatch
{
    private static void Postfix(RelicModel __instance, ref string __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPath(__instance, SereTalonVisualAssetPaths.PackedIcon, ref __result);
    }
}

[HarmonyPatch(typeof(RelicModel), "get_BigIconPath")]
internal static class SereTalonBigIconPathPatch
{
    private static void Postfix(RelicModel __instance, ref string __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPath(__instance, SereTalonVisualAssetPaths.BigIcon, ref __result);
    }
}

[HarmonyPatch(typeof(RelicModel), "get_Icon")]
internal static class SereTalonIconTexturePatch
{
    private static void Postfix(RelicModel __instance, ref Texture2D __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPackedTexture(__instance, ref __result);
    }
}

[HarmonyPatch(typeof(RelicModel), "get_IconOutline")]
internal static class SereTalonIconOutlineTexturePatch
{
    private static void Postfix(RelicModel __instance, ref Texture2D __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPackedTexture(__instance, ref __result);
    }
}

[HarmonyPatch(typeof(RelicModel), "get_BigIcon")]
internal static class SereTalonBigIconTexturePatch
{
    private static void Postfix(RelicModel __instance, ref Texture2D __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyTexture(__instance, SereTalonVisualAssetPaths.BigIcon, ref __result);
    }
}
