using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static void ExpandA11MapWidth(SerializableActMap saved)
    {
        saved.GridWidth += AscensionFeatureGate.A11ExtraMapColumns;
        saved.Points = saved.Points
            .Select(point => TransformMapPoint(point, ShiftA11WidthCoord))
            .ToList();
        saved.BossPoint = TransformMapPoint(saved.BossPoint, ShiftA11WidthCoord);
        saved.SecondBossPoint = saved.SecondBossPoint == null
            ? null
            : TransformMapPoint(saved.SecondBossPoint, ShiftA11WidthCoord);
        saved.StartingPoint = TransformMapPoint(saved.StartingPoint, ShiftA11WidthCoord);
        saved.StartMapPointCoords = saved.StartMapPointCoords?
            .Select(ShiftA11WidthCoord)
            .ToList();
    }

    private static bool TryInsertRouteRowsBeforeBossRest(SerializableActMap saved, int extraRows)
    {
        var insertionRow = saved.GridHeight - 1;
        var originalPoints = saved.Points
            .Select(CloneMapPoint)
            .ToList();
        var bridgeEdges = originalPoints
            .Where(point => point.Coord.row == insertionRow - 1 && point.ChildCoords != null)
            .SelectMany(point => point.ChildCoords!
                .Where(child => child.row == insertionRow)
                .Select(child => (Parent: point.Coord, Child: child)))
            .ToList();

        if (bridgeEdges.Count == 0)
        {
            return false;
        }

        saved.GridHeight += extraRows;
        saved.Points = originalPoints
            .Select(point => TransformMapPoint(point, coord => ShiftRowCoord(coord, insertionRow, extraRows)))
            .ToList();
        saved.BossPoint = TransformMapPoint(saved.BossPoint, coord => ShiftRowCoord(coord, insertionRow, extraRows));
        saved.SecondBossPoint = saved.SecondBossPoint == null
            ? null
            : TransformMapPoint(saved.SecondBossPoint, coord => ShiftRowCoord(coord, insertionRow, extraRows));
        saved.StartingPoint = TransformMapPoint(saved.StartingPoint, coord => ShiftRowCoord(coord, insertionRow, extraRows));
        saved.StartMapPointCoords = saved.StartMapPointCoords?
            .Select(coord => ShiftRowCoord(coord, insertionRow, extraRows))
            .ToList();

        var pointsByCoord = saved.Points.ToDictionary(point => point.Coord);
        foreach (var (parentCoord, childCoord) in bridgeEdges)
        {
            var shiftedParentCoord = ShiftRowCoord(parentCoord, insertionRow, extraRows);
            var shiftedChildCoord = ShiftRowCoord(childCoord, insertionRow, extraRows);
            if (!pointsByCoord.TryGetValue(shiftedParentCoord, out var parentPoint))
            {
                continue;
            }

            RemoveChild(parentPoint, shiftedChildCoord);

            var previousPoint = parentPoint;
            for (var rowOffset = 0; rowOffset < extraRows; rowOffset++)
            {
                var bridgeCoord = new MapCoord
                {
                    col = rowOffset == extraRows - 1 ? childCoord.col : parentCoord.col,
                    row = insertionRow + rowOffset
                };
                var bridgeType = rowOffset == 0
                    ? MapPointType.Monster
                    : MapPointType.Unknown;
                var bridgePoint = GetOrCreateBridgePoint(saved.Points, pointsByCoord, bridgeCoord, bridgeType);
                AddChild(previousPoint, bridgeCoord);
                previousPoint = bridgePoint;
            }

            AddChild(previousPoint, shiftedChildCoord);
        }

        return true;
    }
}
