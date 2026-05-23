using MegaCrit.Sts2.Core.Map;
using EZMicroBalance.EZMicroBalanceCode.Ascension;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    internal static void AvoidRootSightReservedModelForCurrentNonPreviewRoom(
        RunManager runManager,
        RoomType roomType,
        MapPointType pointType)
    {
        if (roomType is not (RoomType.Monster or RoomType.Elite or RoomType.Event) ||
            runManager.DebugOnlyGetState() is not { } runState ||
            runState.CurrentMapPoint == null ||
            MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopFeature(
                runState,
                "UrdaRootEyes",
                "Root Eyes reservation queue mutation is single-player only until host-authoritative preview sync exists") ||
            runState.Players.Count > 1)
        {
            return;
        }

        var currentCoord = FormatCoord(runState.CurrentMapPoint.coord);
        if (TryFindRootSightPreviewForCurrentPoint(runManager, pointType, out var currentPreview) &&
            currentPreview.RoomType == roomType)
        {
            return;
        }

        var reservedIds = GetReservedRootSightModelIds(runState, roomType, currentCoord).ToHashSet();
        if (reservedIds.Count == 0)
        {
            return;
        }

        if (roomType == RoomType.Event)
        {
            TryMoveReservedRootSightEventOffQueueHead(runState, reservedIds);
            return;
        }

        TryMoveReservedRootSightEncounterOffQueueHead(runState, roomType, reservedIds);
    }
}
