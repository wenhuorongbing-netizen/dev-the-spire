using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    internal static bool TryGetRootSightRoomTypeForCurrentPoint(
        RunManager runManager,
        MapPointType pointType,
        out RoomType roomType)
    {
        roomType = RoomType.Unassigned;
        if (!TryFindRootSightPreviewForCurrentPoint(runManager, pointType, out var preview))
        {
            return false;
        }

        if (runManager.DebugOnlyGetState() is not { } runState ||
            !IsRootSightPreviewStillValidForEntry(runState, preview))
        {
            return false;
        }

        roomType = preview.RoomType;
        return true;
    }

    internal static bool TryGetRootSightModelForCurrentPoint(
        RunManager runManager,
        RoomType roomType,
        MapPointType pointType,
        out AbstractModel? model)
    {
        model = null;
        if (!TryFindRootSightPreviewForCurrentPoint(runManager, pointType, out var preview) ||
            preview.RoomType != roomType)
        {
            return false;
        }

        try
        {
            if (runManager.DebugOnlyGetState() is not { } runState ||
                !IsRootSightPreviewStillValidForEntry(runState, preview))
            {
                return false;
            }

            var id = ModelId.Deserialize(preview.ModelId);
            if (roomType == RoomType.Event)
            {
                var eventModel = ModelDb.GetByIdOrNull<EventModel>(id);
                if (eventModel == null)
                {
                    return false;
                }

                if (TryMarkRootSightCommittedForCurrentPoint(runState))
                {
                    if (pointType == MapPointType.Unknown)
                    {
                        CommitRootSightUnknownRoomType(runManager, preview.RoomType);
                    }

                    CommitRootSightEventQueueForEntry(runState, eventModel);
                    runState.AddVisitedEvent(eventModel);
                    ConsumeRootSightPreviewForCurrentPoint(runState, preview);
                }

                model = eventModel;
                return true;
            }

            var encounter = ModelDb.GetByIdOrNull<EncounterModel>(id);
            if (encounter == null)
            {
                return false;
            }

            if (TryMarkRootSightCommittedForCurrentPoint(runState))
            {
                CommitRootSightEncounterQueueForEntry(runState, roomType, encounter);
                if (pointType == MapPointType.Unknown)
                {
                    CommitRootSightUnknownRoomType(runManager, preview.RoomType);
                }

                ConsumeRootSightPreviewForCurrentPoint(runState, preview);
            }

            model = encounter.ToMutable();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
