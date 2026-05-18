using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static IEnumerable<MapPoint> PickDistinctByStableOrder(
        IReadOnlyList<MapPoint> candidates,
        int count,
        IRunState runState,
        string markerFamily,
        int actIndex)
    {
        if (candidates.Count == 0 || count <= 0)
        {
            yield break;
        }

        var used = new HashSet<MapPoint>();
        foreach (var point in EnumerateByStableMarkerOrder(candidates, runState, markerFamily, actIndex))
        {
            if (used.Add(point))
            {
                yield return point;
                if (used.Count >= count)
                {
                    yield break;
                }
            }
        }
    }

    private static IEnumerable<MapPoint> PickFiremarkedElitesByAct(
        IReadOnlyList<MapPoint> candidates,
        int count,
        IRunState runState,
        string markerFamily,
        int actIndex,
        MapPoint start,
        MapPoint boss)
    {
        if (candidates.Count == 0 || count <= 0)
        {
            yield break;
        }

        var selected = new List<MapPoint>();
        foreach (var point in EnumerateByStableMarkerOrder(candidates, runState, markerFamily, actIndex))
        {
            if (HasHardFiremarkPlacementConflict(selected, point) ||
                selected.Any(existing => IsOnSameRoute(existing, point)) ||
                !KeepsFiremarksOptional(start, boss, selected, point))
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
            foreach (var point in EnumerateByStableMarkerOrder(candidates, runState, markerFamily, actIndex))
            {
                if (selected.Contains(point) ||
                    HasHardFiremarkPlacementConflict(selected, point) ||
                    !KeepsFiremarksOptional(start, boss, selected, point))
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

    private static bool KeepsFiremarksOptional(
        MapPoint start,
        MapPoint boss,
        IReadOnlyCollection<MapPoint> selected,
        MapPoint candidate)
    {
        return HasPathAvoiding(start, boss, selected.Append(candidate));
    }

    private static bool HasHardFiremarkPlacementConflict(IReadOnlyCollection<MapPoint> selected, MapPoint point)
    {
        return selected.Any(existing =>
            existing.coord.row == point.coord.row ||
            AreAdjacent(existing, point));
    }
}
