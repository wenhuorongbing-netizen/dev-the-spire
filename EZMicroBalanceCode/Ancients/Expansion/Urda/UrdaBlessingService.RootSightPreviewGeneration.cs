using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Random;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static bool TryCreateRootSightPreview(
        IRunState runState,
        MapPoint point,
        out RootSightPreview preview)
    {
        preview = new RootSightPreview(0, string.Empty, MapPointType.Unassigned, RoomType.Unassigned, string.Empty);
        var roomType = point.PointType switch
        {
            MapPointType.Monster => RoomType.Monster,
            MapPointType.Elite => RoomType.Elite,
            MapPointType.Unknown => RollRootSightUnknownRoomType(runState, point),
            _ => RoomType.Unassigned
        };
        var modelId = roomType switch
        {
            RoomType.Monster or RoomType.Elite => TryPeekRootSightEncounterForPoint(runState, point, roomType)?.Id.ToString() ?? string.Empty,
            RoomType.Event => TryPeekNextValidEvent(runState, point)?.Id.ToString() ?? string.Empty,
            _ => string.Empty
        };
        if (string.IsNullOrWhiteSpace(modelId))
        {
            return false;
        }

        preview = new RootSightPreview(
            runState.CurrentActIndex,
            FormatCoord(point.coord),
            point.PointType,
            roomType,
            modelId);
        ReleaseEvidenceLog.Log(
            "UrdaRootEyes",
            "preview_generated",
            runState: runState,
            data: new Dictionary<string, object?>
            {
                ["coord"] = FormatCoord(point.coord),
                ["pointType"] = point.PointType,
                ["roomType"] = roomType,
                ["modelId"] = modelId
            });
        return true;
    }

    private static RoomSet? TryGetActRoomSet(ActModel act) =>
        AccessTools.Field(typeof(ActModel), "_rooms")?.GetValue(act) as RoomSet;

    private static Rng CreateRootSightPreviewRng(IRunState runState, MapPoint point, string scope)
    {
        var source = runState.Rng.UnknownMapPoint;
        return new Rng(
            source.Seed,
            $"root_sight_{scope}_act_{runState.CurrentActIndex}_coord_{point.coord.col}_{point.coord.row}_counter_{source.Counter}");
    }
}
