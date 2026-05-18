using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static IEnumerable<MapPoint> EnumerateByStableMarkerOrder(
        IReadOnlyList<MapPoint> candidates,
        IRunState runState,
        string markerFamily,
        int actIndex)
    {
        return candidates
            .OrderBy(point => StableMarkerHash(runState, markerFamily, actIndex, point.coord, "point"))
            .ThenBy(point => point.coord.row)
            .ThenBy(point => point.coord.col);
    }

    private static TEnum GetKindFromActOrder<TEnum>(
        IRunState runState,
        string markerFamily,
        int actIndex,
        MapCoord coord,
        int assignmentIndex)
        where TEnum : struct, Enum
    {
        var values = Enum.GetValues<TEnum>();
        var order = values
            .OrderBy(value => StableMarkerHash(runState, markerFamily, actIndex, default, value.ToString()))
            .ThenBy(value => value.ToString(), StringComparer.Ordinal)
            .ToArray();

        var cycle = assignmentIndex / order.Length;
        var index = assignmentIndex % order.Length;
        if (cycle > 0)
        {
            index = (index + (int)(StableMarkerHash(runState, markerFamily, actIndex, coord, $"cycle:{cycle}") % (uint)order.Length)) % order.Length;
        }

        return order[index];
    }

    private static uint StableMarkerHash(
        IRunState runState,
        string markerFamily,
        int actIndex,
        MapCoord coord,
        string salt)
    {
        unchecked
        {
            var hash = 2166136261u;
            AddString(ref hash, runState.Rng.StringSeed);
            AddUInt(ref hash, runState.Rng.Seed);
            AddString(ref hash, markerFamily);
            AddInt(ref hash, actIndex);
            AddInt(ref hash, coord.col);
            AddInt(ref hash, coord.row);
            AddString(ref hash, salt);
            return hash;
        }
    }

    private static void AddString(ref uint hash, string value)
    {
        foreach (var ch in value)
        {
            AddInt(ref hash, ch);
        }
    }

    private static void AddInt(ref uint hash, int value)
    {
        AddUInt(ref hash, unchecked((uint)value));
    }

    private static void AddUInt(ref uint hash, uint value)
    {
        unchecked
        {
            for (var shift = 0; shift < 32; shift += 8)
            {
                hash ^= (value >> shift) & 0xffu;
                hash *= 16777619u;
            }
        }
    }
}
