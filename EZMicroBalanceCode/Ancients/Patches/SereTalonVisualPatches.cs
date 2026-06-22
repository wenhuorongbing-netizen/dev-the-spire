using Godot;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class SereTalonIconPathPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "sere-talon-icon-path";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Route Sere Talon small icon paths to Spire Plus art without changing relic behavior";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), nameof(RelicModel.IconPath), MethodType.Getter)];

    [HarmonyPostfix]
    private static void Postfix(RelicModel __instance, ref string __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPath(__instance, SereTalonVisualAssetPaths.PackedIcon, ref __result);
    }
}

internal sealed class SereTalonPackedIconPathPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "sere-talon-packed-icon-path";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Route Sere Talon packed icon paths to Spire Plus art";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), nameof(RelicModel.PackedIconPath), MethodType.Getter)];

    [HarmonyPostfix]
    private static void Postfix(RelicModel __instance, ref string __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPath(__instance, SereTalonVisualAssetPaths.PackedIcon, ref __result);
    }
}

internal sealed class SereTalonPackedIconOutlinePathPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "sere-talon-packed-icon-outline-path";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Route Sere Talon outline icon paths to Spire Plus art";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), "PackedIconOutlinePath", MethodType.Getter)];

    [HarmonyPostfix]
    private static void Postfix(RelicModel __instance, ref string __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPath(__instance, SereTalonVisualAssetPaths.PackedIcon, ref __result);
    }
}

internal sealed class SereTalonBigIconPathPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "sere-talon-big-icon-path";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Route Sere Talon large icon paths to Spire Plus art";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), "BigIconPath", MethodType.Getter)];

    [HarmonyPostfix]
    private static void Postfix(RelicModel __instance, ref string __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPath(__instance, SereTalonVisualAssetPaths.BigIcon, ref __result);
    }
}

internal sealed class SereTalonIconTexturePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "sere-talon-icon-texture";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Route Sere Talon small icon textures to Spire Plus art";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), nameof(RelicModel.Icon), MethodType.Getter)];

    [HarmonyPostfix]
    private static void Postfix(RelicModel __instance, ref Texture2D __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPackedTexture(__instance, ref __result);
    }
}

internal sealed class SereTalonIconOutlineTexturePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "sere-talon-icon-outline-texture";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Route Sere Talon outline icon textures to Spire Plus art";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), nameof(RelicModel.IconOutline), MethodType.Getter)];

    [HarmonyPostfix]
    private static void Postfix(RelicModel __instance, ref Texture2D __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPackedTexture(__instance, ref __result);
    }
}

internal sealed class SereTalonBigIconTexturePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "sere-talon-big-icon-texture";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Route Sere Talon large icon textures to Spire Plus art";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), nameof(RelicModel.BigIcon), MethodType.Getter)];

    [HarmonyPostfix]
    private static void Postfix(RelicModel __instance, ref Texture2D __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyTexture(__instance, SereTalonVisualAssetPaths.BigIcon, ref __result);
    }
}
