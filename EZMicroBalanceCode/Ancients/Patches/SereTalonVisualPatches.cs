using Godot;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.Relics;

using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class SereTalonIconPathPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "s-e-r-e-t-a-l-o-n-i-c-o-n-p-a-t-h-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch RelicModel.get_IconPath";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), "get_IconPath", HarmonyLib.MethodType.Getter)];
{
    private static void Postfix(RelicModel __instance, ref string __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPath(__instance, SereTalonVisualAssetPaths.PackedIcon, ref __result);
    }
}

internal sealed class SereTalonPackedIconPathPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "s-e-r-e-t-a-l-o-n-p-a-c-k-e-d-i-c-o-n-p-a-t-h-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch RelicModel.get_PackedIconPath";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), "get_PackedIconPath", HarmonyLib.MethodType.Getter)];
{
    private static void Postfix(RelicModel __instance, ref string __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPath(__instance, SereTalonVisualAssetPaths.PackedIcon, ref __result);
    }
}

internal sealed class SereTalonPackedIconOutlinePathPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "s-e-r-e-t-a-l-o-n-p-a-c-k-e-d-i-c-o-n-o-u-t-l-i-n-e-p-a-t-h-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch RelicModel.get_PackedIconOutlinePath";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), "get_PackedIconOutlinePath", HarmonyLib.MethodType.Getter)];
{
    private static void Postfix(RelicModel __instance, ref string __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPath(__instance, SereTalonVisualAssetPaths.PackedIcon, ref __result);
    }
}

internal sealed class SereTalonBigIconPathPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "s-e-r-e-t-a-l-o-n-b-i-g-i-c-o-n-p-a-t-h-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch RelicModel.get_BigIconPath";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), "get_BigIconPath", HarmonyLib.MethodType.Getter)];
{
    private static void Postfix(RelicModel __instance, ref string __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPath(__instance, SereTalonVisualAssetPaths.BigIcon, ref __result);
    }
}

internal sealed class SereTalonIconTexturePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "s-e-r-e-t-a-l-o-n-i-c-o-n-t-e-x-t-u-r-e-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch RelicModel.get_Icon";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), "get_Icon", HarmonyLib.MethodType.Getter)];
{
    private static void Postfix(RelicModel __instance, ref Texture2D __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPackedTexture(__instance, ref __result);
    }
}

internal sealed class SereTalonIconOutlineTexturePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "s-e-r-e-t-a-l-o-n-i-c-o-n-o-u-t-l-i-n-e-t-e-x-t-u-r-e-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch RelicModel.get_IconOutline";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), "get_IconOutline", HarmonyLib.MethodType.Getter)];
{
    private static void Postfix(RelicModel __instance, ref Texture2D __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyPackedTexture(__instance, ref __result);
    }
}

internal sealed class SereTalonBigIconTexturePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "s-e-r-e-t-a-l-o-n-b-i-g-i-c-o-n-t-e-x-t-u-r-e-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch RelicModel.get_BigIcon";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicModel), "get_BigIcon", HarmonyLib.MethodType.Getter)];
{
    private static void Postfix(RelicModel __instance, ref Texture2D __result)
    {
        SereTalonVisualRelicModelRoutes.TryApplyTexture(__instance, SereTalonVisualAssetPaths.BigIcon, ref __result);
    }
}

internal sealed class SereTalonAncientEventOptionButtonPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "s-e-r-e-t-a-l-o-n-a-n-c-i-e-n-t-e-v-e-n-t-o-p-t-i-o-n-b-u-t-t-o-n-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch NEventOptionButton._Ready";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NEventOptionButton), nameof(NEventOptionButton._Ready))];
{
    private static void Postfix(NEventOptionButton __instance)
    {
        // Ancient option buttons assign the relic icon directly during _Ready().
        // Keep this surface explicit so a loader/UI drift report can name the
        // exact surface instead of grouping it with normal RelicModel getters.
        SereTalonVisualNodeRoutes.TryApplyEventOptionButton(__instance);
    }
}

internal sealed class SereTalonRelicNodeReloadPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "s-e-r-e-t-a-l-o-n-r-e-l-i-c-n-o-d-e-r-e-l-o-a-d-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch NRelic.Reload";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NRelic), nameof(NRelic.Reload))];
{
    private static void Postfix(NRelic __instance)
    {
        // Relic-bar and inspect nodes can reload after the model texture getters
        // have already run. Reapply only to SereTalon so Tanx Claws keeps the
        // source Maul-transform visuals.
        SereTalonVisualNodeRoutes.TryApplyRelicNode(__instance);
    }
}


