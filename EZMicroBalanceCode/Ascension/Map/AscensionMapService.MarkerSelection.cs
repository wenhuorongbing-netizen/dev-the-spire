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
}
