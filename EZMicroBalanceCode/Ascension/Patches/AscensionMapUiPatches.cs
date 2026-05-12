using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

[HarmonyPatch(typeof(NNormalMapPoint), "RefreshMarkedIconVisibility")]
internal static class FiremarkedEliteMapIconPatch
{
    private static readonly System.Reflection.FieldInfo QuestIconField =
        AccessTools.Field(typeof(NNormalMapPoint), "_questIcon");

    [HarmonyPostfix]
    private static void Postfix(NNormalMapPoint __instance)
    {
        if (QuestIconField.GetValue(__instance) is not TextureRect questIcon)
        {
            return;
        }

        var metadata = AscensionMapService.TryGetMetadata(__instance.Point);
        var texturePath = metadata?.IsDeepBranchEntry == true &&
            __instance.Point.Quests.Any(quest => quest is AscensionMapQuestMarker)
                ? AscensionAssetPaths.DeepBranchEntryIndicator
                : __instance.Point.Quests.Any(quest => quest is FiremarkedEliteMapQuestMarker)
                    ? AscensionAssetPaths.FiremarkedEliteIndicator
                    : __instance.Point.Quests.Any(quest => quest is BannerRoomMapQuestMarker)
                        ? AscensionAssetPaths.BannerRoomIndicator
                        : null;
        if (texturePath == null)
        {
            return;
        }

        questIcon.Texture = ResourceLoader.Load<Texture2D>(
            texturePath,
            null,
            ResourceLoader.CacheMode.Reuse);
    }
}

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

[HarmonyPatch(typeof(NNormalMapPoint), "OnFocus")]
internal static class BannerRoomMapHoverPatch
{
    [HarmonyPostfix]
    private static void Postfix(NNormalMapPoint __instance)
    {
        if (!__instance.Point.Quests.Any(quest => quest is BannerRoomMapQuestMarker))
        {
            return;
        }

        var metadata = AscensionMapService.TryGetMetadata(__instance.Point);
        if (metadata?.Banner == null)
        {
            return;
        }

        var hoverTipSet = NHoverTipSet.CreateAndShow(__instance, CreateHoverTip(metadata.Banner.Value));
        if (hoverTipSet != null)
        {
            Callable.From(() => hoverTipSet.SetAlignment(__instance, HoverTip.GetHoverTipAlignment(__instance))).CallDeferred();
        }
    }

    private static HoverTip CreateHoverTip(BannerKind banner)
    {
        var locKey = banner switch
        {
            BannerKind.Vanguard => "BANNER_VANGUARD",
            BannerKind.ShieldFormation => "BANNER_SHIELD_FORMATION",
            BannerKind.Bounty => "BANNER_BOUNTY",
            _ => "BANNER_ROOM"
        };

        return new HoverTip(
            new LocString("ascension", $"{locKey}.title"),
            new LocString("ascension", $"{locKey}.description"));
    }
}

[HarmonyPatch(typeof(NNormalMapPoint), "OnFocus")]
internal static class AscensionGenericMapHoverPatch
{
    [HarmonyPostfix]
    private static void Postfix(NNormalMapPoint __instance)
    {
        if (!__instance.Point.Quests.Any(quest => quest is AscensionMapQuestMarker))
        {
            return;
        }

        var metadata = AscensionMapService.TryGetMetadata(__instance.Point);
        if (metadata == null ||
            metadata.Banner.HasValue ||
            metadata.Firemark.HasValue)
        {
            return;
        }

        HoverTip? hoverTip = null;
        if (metadata.DeepBranch.HasValue)
        {
            hoverTip = CreateDeepBranchHoverTip(metadata.DeepBranch.Value, metadata.IsDeepBranchEntry);
        }

        if (hoverTip == null)
        {
            return;
        }

        var hoverTipSet = NHoverTipSet.CreateAndShow(__instance, hoverTip);
        if (hoverTipSet != null)
        {
            Callable.From(() => hoverTipSet.SetAlignment(__instance, HoverTip.GetHoverTipAlignment(__instance))).CallDeferred();
        }
    }

    private static HoverTip CreateDeepBranchHoverTip(DeepBranchNodeKind kind, bool isEntry)
    {
        var key = isEntry
            ? "DEEP_BRANCH_ENTRY"
            : kind == DeepBranchNodeKind.EnhancedReward
            ? "DEEP_BRANCH_REWARD"
            : "DEEP_BRANCH_RISK";

        return new HoverTip(
            new LocString("ascension", $"{key}.title"),
            new LocString("ascension", $"{key}.description"));
    }
}

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
