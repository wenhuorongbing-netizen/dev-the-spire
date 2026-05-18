using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

[HarmonyPatch(typeof(NBossMapPoint), "OnFocus")]
internal static class BossMapPointHoverPatch
{
    [HarmonyPostfix]
    private static void Postfix(NBossMapPoint __instance)
    {
        if (__instance.State == MapPointState.Traveled)
        {
            return;
        }

        var metadata = AscensionMapService.TryGetMetadata(__instance.Point);
        if (metadata?.BossSeal == null)
        {
            return;
        }

        var hoverTipSet = NHoverTipSet.CreateAndShow(__instance, CreateHoverTip(metadata.BossSeal, metadata.IsBossBrand));
        if (hoverTipSet != null)
        {
            Callable.From(() => hoverTipSet.SetAlignment(__instance, HoverTip.GetHoverTipAlignment(__instance))).CallDeferred();
        }
    }

    private static HoverTip CreateHoverTip(BossSealDefinition definition, bool isBossBrand)
    {
        var locKey = isBossBrand ? "BOSS_KING_BRAND" : "BOSS_ROYAL_SEAL";
        var sealKey = BossSealCatalog.GetLocalizationKey(definition.Id);
        var baseDescription = new LocString("ascension", $"{locKey}.description").GetFormattedText();
        var sealTitle = new LocString("ascension", $"{sealKey}.title").GetFormattedText();
        var sealDescriptionKey = isBossBrand ? "brand" : "summary";
        var sourceFallbackDescription = isBossBrand ? definition.BrandSummary : definition.Summary;
        var sealDescription = GetLocalizedOrFallback($"{sealKey}.{sealDescriptionKey}", sourceFallbackDescription);
        return new HoverTip(
            new LocString("ascension", $"{locKey}.title"),
            $"{baseDescription}\n{sealTitle}: {sealDescription}");
    }

    private static string GetLocalizedOrFallback(string key, string fallback)
    {
        var localized = new LocString("ascension", key).GetFormattedText();
        return string.IsNullOrWhiteSpace(localized) || localized.Equals(key, StringComparison.Ordinal)
            ? fallback
            : localized;
    }
}
