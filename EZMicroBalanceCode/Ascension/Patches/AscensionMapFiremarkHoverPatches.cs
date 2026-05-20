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

        var description = new LocString("ascension", $"{locKey}.description");
        AddCurrentActFiremarkValues(description, firemark);
        return new HoverTip(
            new LocString("ascension", $"{locKey}.title"),
            description);
    }

    private static void AddCurrentActFiremarkValues(LocString description, FiremarkKind firemark)
    {
        var actIndex = Math.Clamp(RunManager.Instance.DebugOnlyGetState()?.CurrentActIndex ?? 0, 0, 2);
        switch (firemark)
        {
            case FiremarkKind.Might:
                description.Add("Strength", ActValue(actIndex, 1m, 2m, 4m));
                break;
            case FiremarkKind.Giant:
                description.Add("MaxHpPercent", ActValue(actIndex, 20m, 30m, 45m));
                break;
            case FiremarkKind.ForgeArmor:
                description.Add("Armor", ActValue(actIndex, 5m, 10m, 20m));
                break;
            case FiremarkKind.ConstantHeal:
                description.Add("Heal", ActValue(actIndex, 4m, 8m, 16m));
                description.Add("InterruptDamage", ActValue(actIndex, 12m, 24m, 48m));
                break;
        }
    }

    private static decimal ActValue(int actIndex, decimal actOne, decimal actTwo, decimal actThree)
    {
        return actIndex switch
        {
            0 => actOne,
            1 => actTwo,
            _ => actThree
        };
    }
}
