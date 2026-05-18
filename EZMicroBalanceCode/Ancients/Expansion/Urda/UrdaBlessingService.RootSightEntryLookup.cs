using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static bool TryFindRootSightPreviewForCurrentPoint(
        RunManager runManager,
        MapPointType pointType,
        out RootSightPreview preview)
    {
        preview = new RootSightPreview(0, string.Empty, MapPointType.Unassigned, RoomType.Unassigned, string.Empty);
        var runState = runManager.DebugOnlyGetState();
        var current = runState?.CurrentMapPoint;
        if (runState == null || current == null)
        {
            return false;
        }

        var coord = FormatCoord(current.coord);
        foreach (var player in runState.Players.Where(player => GetSelectedBlessing(player) == UrdaBlessingIds.RootSight))
        {
            var progress = GetProgress(player);
            if (TryFindRootSightPreview(progress, runState.CurrentActIndex, coord, out preview) &&
                preview.PointType == pointType)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsRootSightPreviewStillValidForEntry(
        RunState runState,
        RootSightPreview preview)
    {
        try
        {
            var id = ModelId.Deserialize(preview.ModelId);
            if (preview.RoomType == RoomType.Event)
            {
                var eventModel = ModelDb.GetByIdOrNull<EventModel>(id);
                return eventModel != null &&
                    IsRootSightEventStillValidForEntry(runState, eventModel);
            }

            return ModelDb.GetByIdOrNull<EncounterModel>(id) != null;
        }
        catch
        {
            return false;
        }
    }
}
