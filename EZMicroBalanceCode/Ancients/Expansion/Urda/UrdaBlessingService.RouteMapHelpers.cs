using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static IEnumerable<MapPoint> EnumerateReachable(MapPoint start)
    {
        var seen = new HashSet<MapPoint>();
        var queue = new Queue<MapPoint>();
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            var point = queue.Dequeue();
            foreach (var child in point.Children.OrderBy(child => child.coord.row).ThenBy(child => child.coord.col))
            {
                if (!seen.Add(child))
                {
                    continue;
                }

                yield return child;
                queue.Enqueue(child);
            }
        }
    }

    private static MapPoint? FindPointByCoord(IRunState runState, string coordText)
    {
        return TryParseCoord(coordText, out var col, out var row)
            ? runState.Map.GetPoint(col, row)
            : null;
    }

    private static bool SameCoordString(MapCoord coord, string coordText) =>
        TryParseCoord(coordText, out var col, out var row) &&
        coord.col == col &&
        coord.row == row;

    private static bool TryParseCoord(string value, out int col, out int row)
    {
        col = 0;
        row = 0;
        var parts = value.Split(':');
        return parts.Length == 2 &&
            int.TryParse(parts[0], out col) &&
            int.TryParse(parts[1], out row);
    }
}
