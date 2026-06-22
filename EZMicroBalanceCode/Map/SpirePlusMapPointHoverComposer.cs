using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Map;

internal sealed class SpirePlusMapPointHoverComposer : IPatchMethod
{
    static string IPatchMethod.PatchId => "spire-plus-map-point-hover-composer";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Merge Spire Plus map hover tips after normal map point focus";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(NNormalMapPoint), "OnFocus")];

    [HarmonyPriority(Priority.Last)]
    [HarmonyPostfix]
    private static void Postfix(NNormalMapPoint __instance)
    {
        var hoverTips = CollectHoverTips(__instance).ToList();
        if (hoverTips.Count == 0)
        {
            return;
        }

        // NHoverTipSet stores one active set per owner. Spire Plus map systems
        // therefore merge their entries before showing the set instead of each
        // feature patch racing to create or remove its own tooltip.
        NHoverTipSet.Remove(__instance);
        var hoverTipSet = NHoverTipSet.CreateAndShow(__instance, hoverTips);
        if (hoverTipSet != null)
        {
            Callable.From(() => hoverTipSet.SetAlignment(__instance, HoverTip.GetHoverTipAlignment(__instance))).CallDeferred();
        }
    }

    private static IEnumerable<IHoverTip> CollectHoverTips(NNormalMapPoint pointNode)
    {
        if (UrdaBlessingService.TryGetRootSightHoverTip(pointNode.Point, out var rootSightTip))
        {
            yield return rootSightTip;
        }

        if (FiremarkedEliteMapHoverPatch.TryCreateHoverTip(pointNode.Point, out var firemarkTip))
        {
            yield return firemarkTip;
        }

        if (BannerRoomMapHoverPatch.TryCreateHoverTip(pointNode.Point, out var bannerTip))
        {
            yield return bannerTip;
        }

        if (TryCreateDeepBranchHoverTip(pointNode.Point, out var deepBranchTip))
        {
            yield return deepBranchTip;
        }
    }

    private static bool TryCreateDeepBranchHoverTip(MapPoint point, out HoverTip hoverTip)
    {
        hoverTip = default;
        if (!point.Quests.Any(quest => quest is AscensionMapQuestMarker))
        {
            return false;
        }

        var metadata = AscensionMapService.TryGetMetadata(point);
        if (metadata?.DeepBranch == null)
        {
            return false;
        }

        hoverTip = CreateDeepBranchHoverTip(metadata.DeepBranch.Value, metadata.IsDeepBranchEntry);
        return true;
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
