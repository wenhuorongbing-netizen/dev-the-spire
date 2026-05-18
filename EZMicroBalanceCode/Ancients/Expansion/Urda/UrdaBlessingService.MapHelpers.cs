using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static string FormatCoord(MapCoord coord) => $"{coord.col}:{coord.row}";

    private static bool TryParseCoord(string value, out MapCoord coord)
    {
        coord = default;
        var parts = value.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2 ||
            !int.TryParse(parts[0], out var col) ||
            !int.TryParse(parts[1], out var row))
        {
            return false;
        }

        coord.col = col;
        coord.row = row;
        return true;
    }

    private static bool SameCoord(MapCoord left, MapCoord right) =>
        left.col == right.col && left.row == right.row;

    private static HashSet<string> GetCoordSet(string value) =>
        SplitList(value, '|').ToHashSet(StringComparer.Ordinal);

    private static void EnsureQuestMarker<TMarker>(MapPoint point)
        where TMarker : AbstractModel
    {
        if (point.Quests.Any(quest => quest is TMarker))
        {
            return;
        }

        point.AddQuest(ModelDb.GetById<TMarker>(ModelDb.GetId<TMarker>()));
    }

    private static void RemoveQuestMarker<TMarker>(MapPoint point)
        where TMarker : AbstractModel
    {
        var marker = point.Quests.FirstOrDefault(quest => quest is TMarker);
        if (marker != null)
        {
            point.RemoveQuest(marker);
        }
    }
}
