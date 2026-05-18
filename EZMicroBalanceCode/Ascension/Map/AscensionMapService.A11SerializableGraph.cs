using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static Dictionary<MapCoord, SerializableMapPoint> BuildSerializableLookup(SerializableActMap saved)
    {
        var pointsByCoord = saved.Points.ToDictionary(point => point.Coord);
        pointsByCoord[saved.StartingPoint.Coord] = saved.StartingPoint;
        pointsByCoord[saved.BossPoint.Coord] = saved.BossPoint;
        if (saved.SecondBossPoint != null)
        {
            pointsByCoord[saved.SecondBossPoint.Coord] = saved.SecondBossPoint;
        }

        return pointsByCoord;
    }

    private static A11MapGeometryGraph ToA11MapGeometryGraph(SerializableActMap saved)
    {
        var allPoints = new List<SerializableMapPoint>(saved.Points.Count + 3)
        {
            saved.StartingPoint,
            saved.BossPoint
        };
        if (saved.SecondBossPoint != null)
        {
            allPoints.Add(saved.SecondBossPoint);
        }

        allPoints.AddRange(saved.Points);

        return new A11MapGeometryGraph(
            saved.GridWidth,
            saved.GridHeight,
            ToA11Coord(saved.StartingPoint.Coord),
            ToA11Coord(saved.BossPoint.Coord),
            saved.Points.Select(point => ToA11Coord(point.Coord)),
            allPoints.Select(point => new KeyValuePair<A11MapCoord, IEnumerable<A11MapCoord>>(
                ToA11Coord(point.Coord),
                point.ChildCoords?.Select(ToA11Coord) ?? Enumerable.Empty<A11MapCoord>())));
    }

    private static A11MapCoord ToA11Coord(MapCoord coord)
    {
        return new A11MapCoord(coord.col, coord.row);
    }

    private static IEnumerable<SerializableMapPoint> GetSerializableReachablePointsAtRow(
        SerializableMapPoint start,
        int targetRow,
        IReadOnlyDictionary<MapCoord, SerializableMapPoint> pointsByCoord)
    {
        var visited = new HashSet<MapCoord>();
        var queue = new Queue<SerializableMapPoint>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var point = queue.Dequeue();
            if (!visited.Add(point.Coord))
            {
                continue;
            }

            if (point.Coord.row == targetRow)
            {
                yield return point;
                continue;
            }

            if (point.Coord.row > targetRow ||
                point.ChildCoords == null)
            {
                continue;
            }

            foreach (var childCoord in point.ChildCoords)
            {
                if (childCoord.row <= targetRow &&
                    pointsByCoord.TryGetValue(childCoord, out var child))
                {
                    queue.Enqueue(child);
                }
            }
        }
    }

    private static bool HasSerializablePath(
        SerializableMapPoint start,
        MapCoord targetCoord,
        IReadOnlyDictionary<MapCoord, SerializableMapPoint> pointsByCoord)
    {
        var visited = new HashSet<MapCoord>();
        var queue = new Queue<SerializableMapPoint>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var point = queue.Dequeue();
            if (!visited.Add(point.Coord))
            {
                continue;
            }

            if (point.Coord.Equals(targetCoord))
            {
                return true;
            }

            if (point.ChildCoords == null)
            {
                continue;
            }

            foreach (var childCoord in point.ChildCoords)
            {
                if (pointsByCoord.TryGetValue(childCoord, out var child))
                {
                    queue.Enqueue(child);
                }
            }
        }

        return false;
    }

    private static bool HasSerializablePathAvoiding(
        SerializableMapPoint start,
        MapCoord targetCoord,
        IReadOnlyDictionary<MapCoord, SerializableMapPoint> pointsByCoord,
        IReadOnlySet<MapCoord> excludedCoords)
    {
        if (excludedCoords.Contains(start.Coord) ||
            excludedCoords.Contains(targetCoord))
        {
            return false;
        }

        var visited = new HashSet<MapCoord>();
        var queue = new Queue<SerializableMapPoint>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var point = queue.Dequeue();
            if (!visited.Add(point.Coord))
            {
                continue;
            }

            if (point.Coord.Equals(targetCoord))
            {
                return true;
            }

            if (point.ChildCoords == null)
            {
                continue;
            }

            foreach (var childCoord in point.ChildCoords)
            {
                if (!excludedCoords.Contains(childCoord) &&
                    pointsByCoord.TryGetValue(childCoord, out var child))
                {
                    queue.Enqueue(child);
                }
            }
        }

        return false;
    }
}
