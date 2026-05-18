using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static SerializableMapPoint CloneMapPoint(SerializableMapPoint point)
    {
        return new SerializableMapPoint
        {
            Coord = point.Coord,
            PointType = point.PointType,
            CanBeModified = point.CanBeModified,
            ChildCoords = point.ChildCoords?.ToList()
        };
    }

    private static SerializableMapPoint TransformMapPoint(SerializableMapPoint point, Func<MapCoord, MapCoord> transform)
    {
        return new SerializableMapPoint
        {
            Coord = transform(point.Coord),
            PointType = point.PointType,
            CanBeModified = point.CanBeModified,
            ChildCoords = point.ChildCoords?
                .Select(transform)
                .ToList()
        };
    }

    private static MapCoord ShiftA11WidthCoord(MapCoord coord)
    {
        return coord.col < A11InsertedColumn
            ? coord
            : new MapCoord
            {
                col = coord.col + AscensionFeatureGate.A11ExtraMapColumns,
                row = coord.row
            };
    }

    private static MapCoord ShiftRowCoord(MapCoord coord, int insertionRow, int rowCount)
    {
        return coord.row < insertionRow
            ? coord
            : new MapCoord
            {
                col = coord.col,
                row = coord.row + rowCount
            };
    }

    private static SerializableMapPoint GetOrCreateBridgePoint(
        List<SerializableMapPoint> points,
        Dictionary<MapCoord, SerializableMapPoint> pointsByCoord,
        MapCoord coord,
        MapPointType pointType,
        bool canBeModified = true)
    {
        if (pointsByCoord.TryGetValue(coord, out var point))
        {
            return point;
        }

        point = new SerializableMapPoint
        {
            Coord = coord,
            PointType = pointType,
            CanBeModified = canBeModified
        };
        points.Add(point);
        pointsByCoord.Add(coord, point);
        return point;
    }

    private static void AddChild(SerializableMapPoint point, MapCoord childCoord)
    {
        point.ChildCoords ??= new List<MapCoord>();
        if (!point.ChildCoords.Contains(childCoord))
        {
            point.ChildCoords.Add(childCoord);
        }
    }

    private static void RemoveChild(SerializableMapPoint point, MapCoord childCoord)
    {
        point.ChildCoords?.RemoveAll(coord => coord.Equals(childCoord));
        if (point.ChildCoords?.Count == 0)
        {
            point.ChildCoords = null;
        }
    }
}
