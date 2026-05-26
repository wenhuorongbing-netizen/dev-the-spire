using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private sealed record RootSightPreview(
        int ActIndex,
        string Coord,
        MapPointType PointType,
        RoomType RoomType,
        string ModelId);

    private static bool TryFindRootSightPreview(
        UrdaProgress progress,
        int actIndex,
        string coord,
        out RootSightPreview preview)
    {
        var match = GetRootSightPreviews(progress.RootSightPreviewRecords)
            .FirstOrDefault(candidate => candidate.ActIndex == actIndex && candidate.Coord == coord);
        if (match == null)
        {
            preview = new RootSightPreview(0, string.Empty, MapPointType.Unassigned, RoomType.Unassigned, string.Empty);
            return false;
        }

        preview = match;
        return true;
    }

    private static IReadOnlyList<RootSightPreview> GetRootSightPreviews(string value) =>
        SplitList(value, '|')
            .Select(TryParseRootSightPreview)
            .OfType<RootSightPreview>()
            .ToList();

    private static RootSightPreview? TryParseRootSightPreview(string value)
    {
        var parts = value.Split('~', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 5 ||
            !int.TryParse(parts[0], out var actIndex) ||
            !Enum.TryParse<MapPointType>(parts[2], out var pointType) ||
            !Enum.TryParse<RoomType>(parts[3], out var roomType) ||
            string.IsNullOrWhiteSpace(parts[1]) ||
            string.IsNullOrWhiteSpace(parts[4]))
        {
            return null;
        }

        return new RootSightPreview(actIndex, parts[1], pointType, roomType, parts[4]);
    }

    private static string FormatRootSightPreviews(IEnumerable<RootSightPreview> previews) =>
        string.Join("|", previews.Select(preview =>
            $"{preview.ActIndex}~{preview.Coord}~{preview.PointType}~{preview.RoomType}~{preview.ModelId}"));
}
