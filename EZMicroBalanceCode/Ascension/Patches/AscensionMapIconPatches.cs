using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class FiremarkedEliteMapIconPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "ascension-map-marker-icon-refresh";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Restore Ascension map marker icons after vanilla marker visibility refresh";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NNormalMapPoint), "RefreshMarkedIconVisibility")];

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
