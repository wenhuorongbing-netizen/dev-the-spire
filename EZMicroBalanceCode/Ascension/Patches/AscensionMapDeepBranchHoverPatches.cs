using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

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
