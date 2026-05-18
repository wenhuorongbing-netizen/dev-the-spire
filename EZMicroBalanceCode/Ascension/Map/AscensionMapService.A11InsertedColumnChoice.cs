using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static bool TryInsertA11WidthChoice(SerializableActMap saved)
    {
        var savedPointsByCoord = saved.Points.ToDictionary(point => point.Coord);
        var allPointsByCoord = BuildSerializableLookup(saved);
        if (HasA11InsertedColumnRouteChoice(saved, allPointsByCoord))
        {
            return true;
        }

        var preferredMiddleRow = saved.GridHeight / 2;
        foreach (var parent in saved.Points
            .Where(point => point.ChildCoords?.Count > 0)
            .Where(point => point.Coord.row >= 1 && point.Coord.row <= saved.GridHeight - 3)
            .Where(point => Math.Abs(point.Coord.col - A11InsertedColumn) <= 1)
            .OrderBy(point => Math.Abs(point.Coord.row - preferredMiddleRow))
            .ThenBy(point => Math.Abs(point.Coord.col - A11InsertedColumn))
            .ThenBy(point => point.Coord.col))
        {
            var branchCoord = new MapCoord
            {
                col = A11InsertedColumn,
                row = parent.Coord.row + 1
            };
            if (allPointsByCoord.ContainsKey(branchCoord) ||
                !HasSerializablePath(saved.StartingPoint, parent.Coord, allPointsByCoord))
            {
                continue;
            }

            var reconnect = GetSerializableReachablePointsAtRow(parent, parent.Coord.row + 2, allPointsByCoord)
                .Where(point => Math.Abs(point.Coord.col - A11InsertedColumn) <= 1)
                .Where(point => HasSerializablePath(point, saved.BossPoint.Coord, allPointsByCoord))
                .OrderBy(point => Math.Abs(point.Coord.col - A11InsertedColumn))
                .ThenBy(point => point.Coord.col)
                .FirstOrDefault();
            if (reconnect == null)
            {
                continue;
            }

            var branchPoint = GetOrCreateBridgePoint(
                saved.Points,
                savedPointsByCoord,
                branchCoord,
                MapPointType.Monster);
            allPointsByCoord[branchCoord] = branchPoint;
            AddChild(parent, branchCoord);
            AddChild(branchPoint, reconnect.Coord);

            if (HasSerializablePath(saved.StartingPoint, branchCoord, allPointsByCoord) &&
                HasSerializablePath(branchPoint, saved.BossPoint.Coord, allPointsByCoord) &&
                HasSerializablePath(saved.StartingPoint, saved.BossPoint.Coord, allPointsByCoord) &&
                HasSerializablePathAvoiding(
                    saved.StartingPoint,
                    saved.BossPoint.Coord,
                    allPointsByCoord,
                    new HashSet<MapCoord> { branchCoord }))
            {
                return true;
            }

            RemoveChild(parent, branchCoord);
            saved.Points.Remove(branchPoint);
            savedPointsByCoord.Remove(branchCoord);
            allPointsByCoord.Remove(branchCoord);
        }

        return false;
    }
}
