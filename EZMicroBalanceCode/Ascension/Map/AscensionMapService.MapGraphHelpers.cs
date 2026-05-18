using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static bool IsOnSameRoute(MapPoint left, MapPoint right)
    {
        return CanReach(left, right) || CanReach(right, left);
    }

    private static bool AreAdjacent(MapPoint left, MapPoint right)
    {
        return left.Children.Contains(right) || right.Children.Contains(left);
    }

    private static bool CanReach(MapPoint start, MapPoint target)
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

            if (point == target)
            {
                return true;
            }

            foreach (var child in point.Children)
            {
                queue.Enqueue(child);
            }
        }

        return false;
    }

    private static bool IsAfterActOneFirstRestSite(ActMap map, MapPoint point, int actIndex)
    {
        if (actIndex != 0)
        {
            return true;
        }

        var firstRestSiteRow = map.GetAllMapPoints()
            .Where(candidate => candidate.PointType == MapPointType.RestSite)
            .Select(candidate => (int?)candidate.coord.row)
            .Min();

        return firstRestSiteRow.HasValue && point.coord.row > firstRestSiteRow.Value;
    }

    private static int GetFirstRestSiteRow(ActMap map)
    {
        return map.GetAllMapPoints()
            .Where(candidate => candidate.PointType == MapPointType.RestSite)
            .Select(candidate => (int?)candidate.coord.row)
            .Min() ?? 0;
    }

    private static bool HasPathAvoiding(MapPoint start, MapPoint target, MapPoint excluded)
    {
        return HasPathAvoiding(start, target, new[] { excluded });
    }

    private static bool HasPathAvoiding(MapPoint start, MapPoint target, IEnumerable<MapPoint> excluded)
    {
        var excludedSet = excluded.ToHashSet();
        if (excludedSet.Contains(start) || excludedSet.Contains(target))
        {
            return false;
        }

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

            if (point == target)
            {
                return true;
            }

            foreach (var child in point.Children.Where(child => !excludedSet.Contains(child)))
            {
                queue.Enqueue(child);
            }
        }

        return false;
    }
}
