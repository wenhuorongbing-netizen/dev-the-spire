using MegaCrit.Sts2.Core.Map;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

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
            MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopFeature(
                runState,
                "UrdaRootEyes",
                "Root Eyes room-type routing mutates shared map RNG state") ||
            runState.Players.Count > 1 ||
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
                MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopFeature(
                    runState,
                    "UrdaRootEyes",
                    "Root Eyes room model routing mutates shared map RNG state") ||
                runState.Players.Count > 1 ||
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
                    ReleaseEvidenceLog.Log(
                        "UrdaRootEyes",
                        "node_entered",
                        runState: runState,
                        data: new Dictionary<string, object?>
                        {
                            ["coord"] = preview.Coord,
                            ["roomType"] = preview.RoomType,
                            ["modelId"] = preview.ModelId
                        });
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
                ReleaseEvidenceLog.Log(
                    "UrdaRootEyes",
                    "node_entered",
                    runState: runState,
                    data: new Dictionary<string, object?>
                    {
                        ["coord"] = preview.Coord,
                        ["roomType"] = preview.RoomType,
                        ["modelId"] = preview.ModelId
                    });
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
        catch (Exception ex)
        {
            MainFile.Logger.Warn(
                $"[EZMicroBalance] Urda Root Eyes preview entry failed for {preview.RoomType} {preview.ModelId} at {preview.Coord}: {ex.GetType().Name}: {ex.Message}");
            return false;
        }
    }
}
