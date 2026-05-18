using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

[HarmonyPatch(typeof(NNormalMapPoint), "OnFocus")]
internal static class FiremarkedEliteMapHoverPatch
{
    [HarmonyPostfix]
    private static void Postfix(NNormalMapPoint __instance)
    {
        if (!__instance.Point.Quests.Any(quest => quest is FiremarkedEliteMapQuestMarker))
        {
            return;
        }

        var metadata = AscensionMapService.TryGetMetadata(__instance.Point);
        if (metadata?.Firemark == null)
        {
            return;
        }

        var hoverTipSet = NHoverTipSet.CreateAndShow(__instance, CreateHoverTip(metadata.Firemark.Value));
        if (hoverTipSet != null)
        {
            Callable.From(() => hoverTipSet.SetAlignment(__instance, HoverTip.GetHoverTipAlignment(__instance))).CallDeferred();
        }
    }

    private static HoverTip CreateHoverTip(FiremarkKind firemark)
    {
        var locKey = firemark switch
        {
            FiremarkKind.Might => "FIREMARK_MIGHT",
            FiremarkKind.Giant => "FIREMARK_GIANT",
            FiremarkKind.ForgeArmor => "FIREMARK_FORGE_ARMOR",
            FiremarkKind.ConstantHeal => "FIREMARK_CONSTANT_HEAL",
            _ => "FIREMARK_ELITE"
        };

        return new HoverTip(
            new LocString("ascension", $"{locKey}.title"),
            new LocString("ascension", $"{locKey}.description"));
    }
}
