using Godot;
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
        var hasFiremarkMarker = __instance.Point.Quests.Any(quest => quest is FiremarkedEliteMapQuestMarker);
        var hasBannerMarker = __instance.Point.Quests.Any(quest => quest is BannerRoomMapQuestMarker);
        var texturePath = metadata?.IsDeepBranchEntry == true &&
            __instance.Point.Quests.Any(quest => quest is AscensionMapQuestMarker)
                ? AscensionAssetPaths.DeepBranchEntryIndicator
                : hasFiremarkMarker && metadata?.Firemark is { } firemark
                    ? AscensionAssetPaths.GetFiremarkIndicator(firemark)
                    : hasBannerMarker && metadata?.Banner is { } banner
                        ? AscensionAssetPaths.GetBannerIndicator(banner)
                        : hasFiremarkMarker
                            ? AscensionAssetPaths.FiremarkedEliteIndicator
                            : hasBannerMarker
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
