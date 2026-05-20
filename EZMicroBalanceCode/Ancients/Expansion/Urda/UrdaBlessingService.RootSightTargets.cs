using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    internal static bool CanRootSightTarget(MapPoint point) =>
        GetActiveRootSightSelectionPlayer() is { } player &&
        IsRootSightTarget(player, point);

    private static bool IsRootSightTarget(Player player, MapPoint point)
    {
        var progress = GetProgress(player);
        if (GetSelectedBlessing(player) != UrdaBlessingIds.RootSight ||
            progress.RootSightEyes <= 0 ||
            point.PointType is not (MapPointType.Monster or MapPointType.Unknown or MapPointType.Elite) ||
            IsRootSightMarked(progress, player.RunState.CurrentActIndex, FormatCoord(point.coord)))
        {
            return false;
        }

        return IsFutureReachableRootSightTarget(player, point);
    }

    private static bool IsFutureReachableRootSightTarget(Player player, MapPoint point)
    {
        return IsFutureReachableRootSightTarget(player.RunState, point);
    }

    private static bool IsFutureReachableRootSightTarget(IRunState runState, MapPoint point)
    {
        var current = runState.CurrentMapPoint ?? runState.Map.StartingMapPoint;
        if (point.coord.row <= current.coord.row)
        {
            return false;
        }

        var visited = new HashSet<MapPoint>();
        var pending = new Queue<MapPoint>(current.Children);
        while (pending.Count > 0)
        {
            var candidate = pending.Dequeue();
            if (!visited.Add(candidate))
            {
                continue;
            }

            if (SameCoord(candidate.coord, point.coord))
            {
                return true;
            }

            foreach (var child in candidate.Children)
            {
                pending.Enqueue(child);
            }
        }

        return false;
    }
}
