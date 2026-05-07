using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class AscensionMapService
{
    private const int ActOneFiremarkedEliteTargetCount = 2;
    private const int LaterActFiremarkedEliteTargetCount = 3;
    private const int MinimumFiremarkedEliteFallbackCount = 2;
    private const int VanillaMapColumns = 7;
    private const int A11InsertedColumn = 4;
    private const int DeepBranchMinLength = 3;
    private const int DeepBranchMaxLength = 4;

    private static readonly ConditionalWeakTable<MapPoint, AscensionNodeMetadata> MetadataByPoint = new();
    private static readonly ConditionalWeakTable<ActMap, AppliedMapMarker> AppliedMaps = new();

    public static ActMap Apply(IRunState runState, ActMap map, int actIndex)
    {
        if (!AscensionFeatureGate.IsAnyImplementedSliceEnabled(runState) &&
            !AscensionFeatureGate.IsDiagnosticsEnabled)
        {
            return map;
        }

        var marker = AppliedMaps.GetValue(map, _ => new AppliedMapMarker());
        if (marker.Applied)
        {
            return map;
        }

        marker.Applied = true;

        map = ApplyMapGeometry(runState, map, actIndex);
        MarkDeepBranch(runState, map, actIndex);
        MarkFiremarkedElite(runState, map, actIndex);
        MarkBannerRooms(runState, map, actIndex);
        MarkBossSeals(runState, map, actIndex);

        AppliedMaps.GetValue(map, _ => new AppliedMapMarker()).Applied = true;

        return map;
    }

    public static AscensionNodeMetadata? TryGetMetadata(MapPoint? point)
    {
        if (point == null)
        {
            return null;
        }

        return MetadataByPoint.TryGetValue(point, out var metadata) && metadata.HasAny
            ? metadata
            : null;
    }

    public static AscensionNodeMetadata? TryGetCurrentMetadata(IRunState runState)
    {
        var appliedMap = Apply(runState, runState.Map, runState.CurrentActIndex);
        if (!ReferenceEquals(appliedMap, runState.Map))
        {
            runState.Map = appliedMap;
        }

        return TryGetMetadata(runState.CurrentMapPoint);
    }

    private static ActMap ApplyMapGeometry(IRunState runState, ActMap map, int actIndex)
    {
        if (AscensionFeatureGate.IsMapGeometryEnabled(runState))
        {
            var adjustedMap = TryApplyA11MapShape(runState, map, actIndex);
            if (adjustedMap != map)
            {
                MainFile.Logger.Info(
                    $"[EZMicroBalance] Ascension A11 applied: expanded map width by {AscensionFeatureGate.A11ExtraMapColumns} column with a reachable optional route and inserted {GetA11ExtraRouteRowsForAct(actIndex)} late route row(s); actIndex={actIndex}; columns={adjustedMap.GetColumnCount()}; rows={adjustedMap.GetRowCount()}.");
                map = adjustedMap;
            }
            else
            {
                MainFile.Logger.Info(
                    $"[EZMicroBalance] Ascension A11 gate active: map already adjusted or unsupported for safe v2.0 width/length shaping; actIndex={actIndex}; columns={map.GetColumnCount()}; rows={map.GetRowCount()}.");
            }
        }

        if (AscensionFeatureGate.IsDeepBranchesEnabled(runState))
        {
            var branchedMap = TryInsertDeepBranch(runState, map, actIndex);
            if (branchedMap != map)
            {
                MainFile.Logger.Info(
                    $"[EZMicroBalance] Ascension A17 applied: inserted one optional {DeepBranchMinLength}-{DeepBranchMaxLength} node Deep Branch with safe-route reconnect; actIndex={actIndex}; columns={branchedMap.GetColumnCount()}; rows={branchedMap.GetRowCount()}.");
                map = branchedMap;
            }
            else
            {
                MainFile.Logger.Info(
                    $"[EZMicroBalance] Ascension A17 gate active: Deep Branch already present or unsupported for safe insertion; actIndex={actIndex}; columns={map.GetColumnCount()}; rows={map.GetRowCount()}.");
            }
        }

        return map;
    }

    private static ActMap TryApplyA11MapShape(IRunState runState, ActMap map, int actIndex)
    {
        if (map is not StandardActMap and not SavedActMap)
        {
            return map;
        }

        var vanillaRows = runState.Act.GetNumberOfRooms(runState.Players.Count > 1) + 1;
        var targetRows = vanillaRows + GetA11ExtraRouteRowsForAct(actIndex);
        var targetColumns = VanillaMapColumns + AscensionFeatureGate.A11ExtraMapColumns;
        if (map.GetColumnCount() == targetColumns &&
            map.GetRowCount() == targetRows)
        {
            return map;
        }

        if ((map.GetColumnCount() != VanillaMapColumns && map.GetColumnCount() != targetColumns) ||
            map.GetRowCount() < vanillaRows ||
            map.GetRowCount() > targetRows ||
            map.GetRowCount() < 4)
        {
            return map;
        }

        var saved = SerializableActMap.FromActMap(map);
        if (saved.GridWidth == VanillaMapColumns)
        {
            ExpandA11MapWidth(saved);
        }

        var missingRows = targetRows - saved.GridHeight;
        if (missingRows > 0 && !TryInsertRouteRowsBeforeBossRest(saved, missingRows))
        {
            return map;
        }

        if (!TryInsertA11WidthChoice(saved))
        {
            return map;
        }

        return new SavedActMap(saved);
    }

    private static int GetA11ExtraRouteRowsForAct(int actIndex)
    {
        return actIndex switch
        {
            0 => AscensionFeatureGate.A11ActOneExtraMapRows,
            1 => AscensionFeatureGate.A11ActTwoExtraMapRows,
            2 => AscensionFeatureGate.A11ActThreeExtraMapRows,
            _ => AscensionFeatureGate.A11ActTwoExtraMapRows
        };
    }

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
                HasSerializablePath(saved.StartingPoint, saved.BossPoint.Coord, allPointsByCoord))
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

    private static bool HasA11InsertedColumnRouteChoice(
        SerializableActMap saved,
        IReadOnlyDictionary<MapCoord, SerializableMapPoint> pointsByCoord)
    {
        return saved.Points
            .Where(point => point.Coord.col == A11InsertedColumn)
            .Any(point =>
                HasSerializablePath(saved.StartingPoint, point.Coord, pointsByCoord) &&
                HasSerializablePath(point, saved.BossPoint.Coord, pointsByCoord));
    }

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
            metadata.DeepBranch = IsDeepBranchRewardIndex(i, plan.BranchCoords.Count)
                ? DeepBranchNodeKind.EnhancedReward
                : DeepBranchNodeKind.Risk;

            if (point.PointType == MapPointType.Monster &&
                AscensionFeatureGate.IsBannerRoomEnabled(runState))
            {
                metadata.Banner = (BannerKind)((actIndex + i) % Enum.GetValues<BannerKind>().Length);
                EnsureQuestMarker<BannerRoomMapQuestMarker>(point);
            }
            else if (point.PointType == MapPointType.Elite &&
                AscensionFeatureGate.IsFiremarkedEliteEnabled(runState))
            {
                metadata.Firemark = (FiremarkKind)((actIndex + i) % Enum.GetValues<FiremarkKind>().Length);
                EnsureQuestMarker<FiremarkedEliteMapQuestMarker>(point);
            }
            else
            {
                EnsureQuestMarker<AscensionMapQuestMarker>(point);
            }
        }

        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension A17 applied: Deep Branch metadata restored; actIndex={actIndex}; parent={plan.ParentCoord}; reconnect={plan.ReconnectCoord}; nodes={plan.BranchCoords.Count}.");
    }

    private static DeepBranchPlan? CreateDeepBranchPlan(ActMap map, int actIndex)
    {
        if (!IsDeepBranchAct(actIndex) ||
            map.GetColumnCount() <= A11InsertedColumn)
        {
            return null;
        }

        for (var branchLength = DeepBranchMaxLength; branchLength >= DeepBranchMinLength; branchLength--)
        {
            var firstParentRow = Math.Max(2, map.GetRowCount() / 3);
            var lastParentRow = map.GetRowCount() - branchLength - 2;
            for (var parentRow = firstParentRow; parentRow <= lastParentRow; parentRow++)
            {
                if (!TryGetDeepBranchCoords(map, parentRow, branchLength, out var branchCoords))
                {
                    continue;
                }

                foreach (var parent in map.GetPointsInRow(parentRow)
                    .OrderBy(point => Math.Abs(point.coord.col - A11InsertedColumn))
                    .ThenBy(point => point.coord.col))
                {
                    var reconnect = GetReachablePointsAtRow(parent, parentRow + branchLength + 1)
                        .OrderBy(point => Math.Abs(point.coord.col - A11InsertedColumn))
                        .ThenBy(point => point.coord.col)
                        .FirstOrDefault();
                    if (reconnect == null)
                    {
                        continue;
                    }

                    return new DeepBranchPlan(
                        parent.coord,
                        reconnect.coord,
                        branchCoords);
                }
            }
        }

        return null;
    }

    private static DeepBranchPlan? FindExistingDeepBranch(ActMap map, int actIndex)
    {
        if (!IsDeepBranchAct(actIndex) ||
            map.GetColumnCount() <= A11InsertedColumn)
        {
            return null;
        }

        foreach (var parent in map.GetAllMapPoints()
            .OrderBy(point => point.coord.row)
            .ThenBy(point => point.coord.col))
        {
            for (var branchLength = DeepBranchMaxLength; branchLength >= DeepBranchMinLength; branchLength--)
            {
                var branchCoords = BuildDeepBranchCoords(parent.coord.row, branchLength);
                var branchPoints = branchCoords
                    .Select(map.GetPoint)
                    .ToList();
                var existingBranchPoints = branchPoints.OfType<MapPoint>().ToList();
                if (existingBranchPoints.Count != branchLength ||
                    !parent.Children.Contains(existingBranchPoints[0]) ||
                    existingBranchPoints.Where((point, index) =>
                        point.PointType != GetDeepBranchPointType(index, branchLength) ||
                        point.CanBeModified).Any())
                {
                    continue;
                }

                var lastBranchPoint = existingBranchPoints[^1];
                var reconnect = lastBranchPoint!.Children
                    .FirstOrDefault(point => point.coord.row == parent.coord.row + branchLength + 1);
                if (reconnect == null ||
                    !HasPathAvoiding(parent, reconnect, existingBranchPoints))
                {
                    continue;
                }

                return new DeepBranchPlan(
                    parent.coord,
                    reconnect.coord,
                    branchCoords);
            }
        }

        return null;
    }

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

    private static bool TryGetDeepBranchCoords(ActMap map, int parentRow, int branchLength, out List<MapCoord> branchCoords)
    {
        branchCoords = BuildDeepBranchCoords(parentRow, branchLength);
        return branchCoords.All(coord => !map.HasPoint(coord));
    }

    private static List<MapCoord> BuildDeepBranchCoords(int parentRow, int branchLength)
    {
        var coords = new List<MapCoord>(branchLength);
        for (var i = 0; i < branchLength; i++)
        {
            coords.Add(new MapCoord
            {
                col = A11InsertedColumn,
                row = parentRow + i + 1
            });
        }

        return coords;
    }

    private static bool IsDeepBranchAct(int actIndex)
    {
        return actIndex is 1 or 2;
    }

    private static MapPointType GetDeepBranchPointType(int index, int branchLength)
    {
        if (index == 0)
        {
            return MapPointType.Monster;
        }

        if (index == 1)
        {
            return MapPointType.Elite;
        }

        return IsDeepBranchRewardIndex(index, branchLength)
            ? MapPointType.Treasure
            : MapPointType.Shop;
    }

    private static bool IsDeepBranchRewardIndex(int index, int branchLength)
    {
        return index == branchLength - 1;
    }

    private static void MarkFiremarkedElite(IRunState runState, ActMap map, int actIndex)
    {
        if (!AscensionFeatureGate.IsFiremarkedEliteEnabled(runState))
        {
            return;
        }

        var candidates = map.GetAllMapPoints()
            .Where(point => point.PointType == MapPointType.Elite)
            .Where(point => point.CanBeModified)
            .Where(point => IsAfterActOneFirstRestSite(map, point, actIndex))
            .Where(point => HasPathAvoiding(map.StartingMapPoint, map.BossMapPoint, point))
            .OrderBy(point => point.coord.row)
            .ThenBy(point => point.coord.col)
            .ToList();

        var desiredCount = GetFiremarkedEliteTargetCount(actIndex);
        var markedCount = 0;
        foreach (var point in PickFiremarkedElitesByAct(candidates, desiredCount, actIndex))
        {
            var kind = (FiremarkKind)((actIndex + markedCount) % Enum.GetValues<FiremarkKind>().Length);
            GetOrCreateMetadata(point).Firemark = kind;
            EnsureQuestMarker<FiremarkedEliteMapQuestMarker>(point);
            markedCount++;

            MainFile.Logger.Info(
                $"[EZMicroBalance] Ascension A12 applied: marked {point} as firemarked elite ({kind}).");
        }

        if (markedCount == 0)
        {
            MainFile.Logger.Info(
                $"[EZMicroBalance] Ascension A12 gate active: no optional elite node was safe to firemark on actIndex={actIndex}.");
            return;
        }

        if (markedCount < desiredCount)
        {
            MainFile.Logger.Info(
                $"[EZMicroBalance] Ascension A12 gate active: marked {markedCount}/{desiredCount} firemarked elites on actIndex={actIndex}; minimum fallback target is {MinimumFiremarkedEliteFallbackCount} when safe candidates exist.");
        }
    }

    private static void MarkBannerRooms(IRunState runState, ActMap map, int actIndex)
    {
        if (!AscensionFeatureGate.IsBannerRoomEnabled(runState))
        {
            return;
        }

        var desiredCount = actIndex == 0 ? 1 : 2;
        var preferredMinimumRow = Math.Max(GetFirstRestSiteRow(map) + 1, 1);
        var candidates = map.GetAllMapPoints()
            .Where(point => point.PointType == MapPointType.Monster)
            .Where(point => point.CanBeModified)
            .Where(point => point.coord.row >= preferredMinimumRow)
            .Where(point => point.Quests.All(quest => quest is not FiremarkedEliteMapQuestMarker))
            .Where(point => HasPathAvoiding(map.StartingMapPoint, map.BossMapPoint, point))
            .OrderBy(point => point.coord.row)
            .ThenBy(point => point.coord.col)
            .ToList();

        if (candidates.Count < desiredCount && actIndex > 0)
        {
            candidates = map.GetAllMapPoints()
                .Where(point => point.PointType == MapPointType.Monster)
                .Where(point => point.CanBeModified)
                .Where(point => point.Quests.All(quest => quest is not FiremarkedEliteMapQuestMarker))
                .Where(point => HasPathAvoiding(map.StartingMapPoint, map.BossMapPoint, point))
                .OrderBy(point => point.coord.row)
                .ThenBy(point => point.coord.col)
                .ToList();
        }

        var markedCount = 0;
        foreach (var point in PickDistinctByAct(candidates, desiredCount, actIndex + 1))
        {
            var kind = (BannerKind)((actIndex + markedCount) % Enum.GetValues<BannerKind>().Length);
            GetOrCreateMetadata(point).Banner = kind;
            EnsureQuestMarker<BannerRoomMapQuestMarker>(point);
            markedCount++;

            MainFile.Logger.Info(
                $"[EZMicroBalance] Ascension A16 applied: marked {point} as banner room ({kind}).");
        }

        if (markedCount < desiredCount)
        {
            MainFile.Logger.Info(
                $"[EZMicroBalance] Ascension A16 gate active: marked {markedCount}/{desiredCount} optional banner rooms on actIndex={actIndex}.");
        }
    }

    private static void MarkBossSeals(IRunState runState, ActMap map, int actIndex)
    {
        var bossSealsEnabled = AscensionFeatureGate.IsBossSealsEnabled(runState);
        var dualKingBrandsEnabled = AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(runState);
        if (!bossSealsEnabled && !dualKingBrandsEnabled)
        {
            return;
        }

        if (bossSealsEnabled)
        {
            var bossSeal = BossSealCatalog.TryGetForEncounter(runState.Act.BossEncounter);
            if (bossSeal == null)
            {
                MainFile.Logger.Info(
                    $"[EZMicroBalance] Ascension A19 gate active: no Boss Royal Seal definition was found for {runState.Act.BossEncounter.Id}.");
            }
            else
            {
                var bossMetadata = GetOrCreateMetadata(map.BossMapPoint);
                bossMetadata.BossSeal = bossSeal;
                bossMetadata.IsBossBrand = false;
                MainFile.Logger.Info(
                    $"[EZMicroBalance] Ascension A19 armed: boss node marked with {bossSeal.Name} ({bossSeal.Id}); status={bossSeal.Status}.");
            }
        }

        if (!dualKingBrandsEnabled)
        {
            return;
        }

        if (map.SecondBossMapPoint == null)
        {
            MainFile.Logger.Info(
                "[EZMicroBalance] Ascension A20 gate active: no second boss map point exists, so Boss 2 Brand metadata, reveal, courtyard, and intermission remain inactive.");
            return;
        }

        var secondBossSeal = BossSealCatalog.TryGetForEncounter(runState.Act.SecondBossEncounter);
        if (secondBossSeal == null)
        {
            MainFile.Logger.Info(
                "[EZMicroBalance] Ascension A20 gate active: second boss map point exists, but no second boss Royal Seal definition was found.");
            return;
        }

        var secondBossMetadata = GetOrCreateMetadata(map.SecondBossMapPoint);
        secondBossMetadata.BossSeal = secondBossSeal;
        secondBossMetadata.IsBossBrand = true;
        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension A20 armed: second boss node marked with {secondBossSeal.Name} Brand ({secondBossSeal.Id}); vanilla boss map icons reveal the boss order, and the fixed courtyard event is ready after Boss 1 rewards.");
    }

    private static AscensionNodeMetadata GetOrCreateMetadata(MapPoint point)
    {
        return MetadataByPoint.GetValue(point, _ => new AscensionNodeMetadata());
    }

    private static void EnsureQuestMarker<TMarker>(MapPoint point)
        where TMarker : AbstractModel
    {
        if (point.Quests.Any(quest => quest is TMarker))
        {
            return;
        }

        point.AddQuest(ModelDb.GetById<TMarker>(ModelDb.GetId<TMarker>()));
    }

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

    private static IEnumerable<MapPoint> PickDistinctByAct(IReadOnlyList<MapPoint> candidates, int count, int offset)
    {
        if (candidates.Count == 0 || count <= 0)
        {
            yield break;
        }

        var used = new HashSet<MapPoint>();
        for (var i = 0; i < candidates.Count && used.Count < count; i++)
        {
            var point = candidates[Math.Abs(offset + i) % candidates.Count];
            if (used.Add(point))
            {
                yield return point;
            }
        }
    }

    private static int GetFiremarkedEliteTargetCount(int actIndex)
    {
        return actIndex == 0
            ? ActOneFiremarkedEliteTargetCount
            : LaterActFiremarkedEliteTargetCount;
    }

    private static IEnumerable<MapPoint> PickFiremarkedElitesByAct(
        IReadOnlyList<MapPoint> candidates,
        int count,
        int offset)
    {
        if (candidates.Count == 0 || count <= 0)
        {
            yield break;
        }

        var selected = new List<MapPoint>();
        foreach (var point in EnumerateFromOffset(candidates, offset))
        {
            if (HasHardFiremarkPlacementConflict(selected, point) ||
                selected.Any(existing => IsOnSameRoute(existing, point)))
            {
                continue;
            }

            selected.Add(point);
            if (selected.Count >= count)
            {
                break;
            }
        }

        if (selected.Count < count)
        {
            foreach (var point in EnumerateFromOffset(candidates, offset))
            {
                if (selected.Contains(point) ||
                    HasHardFiremarkPlacementConflict(selected, point))
                {
                    continue;
                }

                selected.Add(point);
                if (selected.Count >= count)
                {
                    break;
                }
            }
        }

        foreach (var point in selected)
        {
            yield return point;
        }
    }

    private static IEnumerable<MapPoint> EnumerateFromOffset(IReadOnlyList<MapPoint> candidates, int offset)
    {
        for (var i = 0; i < candidates.Count; i++)
        {
            yield return candidates[Math.Abs(offset + i) % candidates.Count];
        }
    }

    private static bool HasHardFiremarkPlacementConflict(IReadOnlyCollection<MapPoint> selected, MapPoint point)
    {
        return selected.Any(existing =>
            existing.coord.row == point.coord.row ||
            AreAdjacent(existing, point));
    }

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

    private sealed record DeepBranchPlan(
        MapCoord ParentCoord,
        MapCoord ReconnectCoord,
        List<MapCoord> BranchCoords);

    private sealed class AppliedMapMarker
    {
        public bool Applied { get; set; }
    }
}
