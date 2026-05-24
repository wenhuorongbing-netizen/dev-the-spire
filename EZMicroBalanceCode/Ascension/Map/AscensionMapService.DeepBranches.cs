using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static ActMap TryInsertDeepBranch(IRunState runState, ActMap map, int actIndex)
    {
        if (!IsDeepBranchAct(actIndex) ||
            runState.Players.Count > 1 ||
            map is not StandardActMap and not SavedActMap ||
            FindExistingDeepBranch(map, actIndex) != null)
        {
            return map;
        }

        var plan = CreateDeepBranchPlan(map, actIndex);
        if (plan == null)
        {
            return map;
        }

        var saved = SerializableActMap.FromActMap(map);
        var pointsByCoord = saved.Points.ToDictionary(point => point.Coord);
        if (!pointsByCoord.TryGetValue(plan.ParentCoord, out var parentPoint) ||
            !pointsByCoord.ContainsKey(plan.ReconnectCoord))
        {
            return map;
        }

        var previousPoint = parentPoint;
        for (var i = 0; i < plan.BranchCoords.Count; i++)
        {
            var branchCoord = plan.BranchCoords[i];
            var branchPoint = GetOrCreateBridgePoint(
                saved.Points,
                pointsByCoord,
                branchCoord,
                GetDeepBranchPointType(i, plan.BranchCoords.Count),
                canBeModified: false);
            AddChild(previousPoint, branchCoord);
            previousPoint = branchPoint;
        }

        AddChild(previousPoint, plan.ReconnectCoord);
        if (!IsDeepBranchRouteSafe(saved, plan))
        {
            return map;
        }

        return new SavedActMap(saved);
    }

    private static void MarkDeepBranch(IRunState runState, ActMap map, int actIndex)
    {
        if (!AscensionFeatureGate.IsDeepBranchesEnabled(runState))
        {
            return;
        }

        var plan = FindExistingDeepBranch(map, actIndex);
        if (plan == null)
        {
            return;
        }

        for (var i = 0; i < plan.BranchCoords.Count; i++)
        {
            var point = map.GetPoint(plan.BranchCoords[i]);
            if (point == null)
            {
                continue;
            }

            var metadata = GetOrCreateMetadata(point);
            metadata.IsDeepBranchEntry = i == 0;
            metadata.DeepBranch = IsDeepBranchRewardIndex(i, plan.BranchCoords.Count)
                ? DeepBranchNodeKind.EnhancedReward
                : DeepBranchNodeKind.Risk;

            if (metadata.IsDeepBranchEntry)
            {
                EnsureQuestMarker<AscensionMapQuestMarker>(point);
            }

            if (point.PointType == MapPointType.Monster &&
                AscensionFeatureGate.IsBannerRoomEnabled(runState))
            {
                metadata.Banner = GetKindFromActOrder<BannerKind>(
                    runState,
                    BannerMarkerFamily,
                    actIndex,
                    point.coord,
                    i);
                EnsureQuestMarker<BannerRoomMapQuestMarker>(point);
                LogMapAssignment(actIndex, point.coord, BannerMarkerFamily, metadata.Banner.Value);
            }
            else if (point.PointType == MapPointType.Elite &&
                AscensionFeatureGate.IsFiremarkedEliteEnabled(runState))
            {
                metadata.Firemark = GetKindFromActOrder<FiremarkKind>(
                    runState,
                    FiremarkMarkerFamily,
                    actIndex,
                    point.coord,
                    i);
                EnsureQuestMarker<FiremarkedEliteMapQuestMarker>(point);
                LogMapAssignment(actIndex, point.coord, FiremarkMarkerFamily, metadata.Firemark.Value);
            }
            else
            {
                EnsureQuestMarker<AscensionMapQuestMarker>(point);
            }
        }

        MainFile.Logger.Info(
            $"[Spire Plus] Ascension A17 applied: Deep Branch metadata restored; actIndex={actIndex}; parent={plan.ParentCoord}; reconnect={plan.ReconnectCoord}; nodes={plan.BranchCoords.Count}.");
    }

}
