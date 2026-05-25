using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class FiremarkedEliteMapHoverPatch
{
    internal static bool TryCreateHoverTip(MapPoint point, out HoverTip hoverTip)
    {
        hoverTip = default;
        if (!point.Quests.Any(quest => quest is FiremarkedEliteMapQuestMarker))
        {
            return false;
        }

        var metadata = AscensionMapService.TryGetMetadata(point);
        if (metadata?.Firemark == null)
        {
            return false;
        }

        hoverTip = CreateHoverTip(metadata.Firemark.Value);
        return true;
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
                description.Add("OverflowStrength", ActValue(actIndex, 1m, 1m, 2m));
                break;
            case FiremarkKind.Giant:
                description.Add("MaxHpPercent", ActValue(actIndex, 20m, 30m, 45m));
                description.Add("OverflowDamage", ActValue(actIndex, 6m, 12m, 24m));
                break;
            case FiremarkKind.ForgeArmor:
                description.Add("Armor", ActValue(actIndex, 8m, 14m, 24m));
                description.Add("OverflowBlock", ActValue(actIndex, 3m, 6m, 12m));
                break;
            case FiremarkKind.ConstantHeal:
                description.Add("Heal", ActValue(actIndex, 4m, 8m, 16m));
                description.Add("InterruptDamage", ActValue(actIndex, 18m, 36m, 72m));
                description.Add("OverflowHeal", ActValue(actIndex, 2m, 4m, 8m));
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
