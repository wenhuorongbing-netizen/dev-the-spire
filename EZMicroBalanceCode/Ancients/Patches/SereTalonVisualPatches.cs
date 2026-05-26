using Godot;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.Relics;

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

[HarmonyPatch(typeof(NEventOptionButton), nameof(NEventOptionButton._Ready))]
internal static class SereTalonAncientEventOptionButtonPatch
{
    private static void Postfix(NEventOptionButton __instance)
    {
        // Ancient option buttons assign the relic icon directly during _Ready().
        // Keep this surface explicit so a loader/UI drift report can name the
        // exact surface instead of grouping it with normal RelicModel getters.
        SereTalonVisualNodeRoutes.TryApplyEventOptionButton(__instance);
    }
}

[HarmonyPatch(typeof(NRelic), "Reload")]
internal static class SereTalonRelicNodeReloadPatch
{
    private static void Postfix(NRelic __instance)
    {
        // Relic-bar and inspect nodes can reload after the model texture getters
        // have already run. Reapply only to SereTalon so Tanx Claws keeps the
        // source Maul-transform visuals.
        SereTalonVisualNodeRoutes.TryApplyRelicNode(__instance);
    }
}
