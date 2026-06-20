using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static void TryMoveReservedRootSightEncounterOffQueueHead(
        RunState runState,
        RoomType roomType,
        ISet<ModelId> reservedIds)
    {
        if (TryGetActRoomSet(runState.Act) is not { } rooms)
        {
            return;
        }

        var encounters = roomType switch
        {
            RoomType.Monster => rooms.normalEncounters,
            RoomType.Elite => rooms.eliteEncounters,
            _ => []
        };
        if (encounters.Count == 0)
        {
            return;
        }

        var visited = roomType == RoomType.Monster
            ? rooms.normalEncountersVisited
            : rooms.eliteEncountersVisited;
        var currentIndex = visited % encounters.Count;
        if (!reservedIds.Contains(encounters[currentIndex].Id))
        {
            return;
        }

        var remainingCount = visited < encounters.Count
            ? encounters.Count - visited
            : encounters.Count;
        for (var offset = 1; offset < remainingCount; offset++)
        {
            var candidateIndex = (currentIndex + offset) % encounters.Count;
            if (reservedIds.Contains(encounters[candidateIndex].Id))
            {
                continue;
            }

            (encounters[currentIndex], encounters[candidateIndex]) = (encounters[candidateIndex], encounters[currentIndex]);
            MainFile.Logger.Info(
                $"[Spire Plus] Urda Root Eyes kept reserved {roomType} preview {encounters[candidateIndex].Id} for a future marked room.");
            return;
        }
    }

    private static void TryMoveReservedRootSightEventOffQueueHead(
        RunState runState,
        ISet<ModelId> reservedIds)
    {
        if (TryGetActRoomSet(runState.Act) is not { events.Count: > 0 } rooms)
        {
            return;
        }

        rooms.EnsureNextEventIsValid(runState);
        var currentIndex = rooms.eventsVisited % rooms.events.Count;
        if (!reservedIds.Contains(rooms.events[currentIndex].Id))
        {
            return;
        }

        var uniqueEventsExhausted = AreRootSightUniqueEventsExhausted(runState, rooms);
        for (var offset = 1; offset < rooms.events.Count; offset++)
        {
            var candidateIndex = (currentIndex + offset) % rooms.events.Count;
            var candidate = rooms.events[candidateIndex];
            if (reservedIds.Contains(candidate.Id) ||
                !candidate.IsAllowed(runState) ||
                (!uniqueEventsExhausted && runState.VisitedEventIds.Contains(candidate.Id)))
            {
                continue;
            }

            (rooms.events[currentIndex], rooms.events[candidateIndex]) = (rooms.events[candidateIndex], rooms.events[currentIndex]);
            MainFile.Logger.Info(
                $"[Spire Plus] Urda Root Eyes kept reserved event preview {rooms.events[candidateIndex].Id} for a future marked room.");
            return;
        }
    }
}
