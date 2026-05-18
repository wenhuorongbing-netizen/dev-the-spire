using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static IEnumerable<MapPoint> GetReachablePointsAtRow(MapPoint start, int row)
    {
        var visited = new HashSet<MapPoint>();
        var queue = new Queue<MapPoint>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var point = queue.Dequeue();
            if (!visited.Add(point))
            {
                continue;
            }

            if (point.coord.row == row)
            {
                yield return point;
                continue;
            }

            foreach (var child in point.Children.Where(child => child.coord.row <= row))
            {
                queue.Enqueue(child);
            }
        }
    }

    private static bool IsDeepBranchRouteSafe(SerializableActMap saved, DeepBranchPlan plan)
    {
        var pointsByCoord = BuildSerializableLookup(saved);
        var excludedBranchCoords = plan.BranchCoords.ToHashSet();
        return pointsByCoord.TryGetValue(plan.ParentCoord, out var parent) &&
            pointsByCoord.TryGetValue(plan.BranchCoords[0], out var firstBranchPoint) &&
            pointsByCoord.TryGetValue(plan.BranchCoords[^1], out var lastBranchPoint) &&
            HasSerializablePath(saved.StartingPoint, firstBranchPoint.Coord, pointsByCoord) &&
            HasSerializablePath(lastBranchPoint, saved.BossPoint.Coord, pointsByCoord) &&
            HasSerializablePathAvoiding(parent, plan.ReconnectCoord, pointsByCoord, excludedBranchCoords) &&
            HasSerializablePathAvoiding(saved.StartingPoint, saved.BossPoint.Coord, pointsByCoord, excludedBranchCoords);
    }
}
