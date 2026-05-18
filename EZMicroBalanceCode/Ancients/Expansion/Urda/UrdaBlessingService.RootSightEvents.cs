using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static EventModel? TryPeekNextValidEvent(IRunState runState, MapPoint point)
    {
        if (runState is not RunState concreteRunState ||
            TryGetActRoomSet(runState.Act) is not { events.Count: > 0 } rooms)
        {
            return null;
        }

        var nextEvent = PeekRootSightNextValidEvent(concreteRunState, rooms, point);
        return nextEvent == null
            ? null
            : Hook.ModifyNextEvent(concreteRunState, nextEvent);
    }

    private static EventModel? PeekRootSightNextValidEvent(RunState runState, RoomSet rooms, MapPoint point)
    {
        var candidates = new List<EventModel>();
        for (var i = 0; i < rooms.events.Count; i++)
        {
            var candidate = rooms.events[(rooms.eventsVisited + i) % rooms.events.Count];
            if (candidate.IsAllowed(runState) && !runState.VisitedEventIds.Contains(candidate.Id))
            {
                candidates.Add(candidate);
            }
        }

        if (candidates.Count == 0)
        {
            var repeatCandidates = new List<EventModel>();
            for (var i = 0; i < rooms.events.Count; i++)
            {
                var candidate = rooms.events[(rooms.eventsVisited + i) % rooms.events.Count];
                if (candidate.IsAllowed(runState))
                {
                    repeatCandidates.Add(candidate);
                }
            }

            var repeatUnreserved = FilterRootSightReservedEventCandidates(runState, point, repeatCandidates);
            return repeatUnreserved.Count > 0
                ? repeatUnreserved[0]
                : rooms.NextEvent;
        }

        candidates = FilterRootSightReservedEventCandidates(runState, point, candidates).ToList();
        var fork = CreateRootSightPreviewRng(runState, point, "event");
        return fork.NextItem(candidates);
    }

    private static IReadOnlyList<EventModel> FilterRootSightReservedEventCandidates(
        RunState runState,
        MapPoint point,
        IReadOnlyList<EventModel> candidates)
    {
        var reservedIds = GetReservedRootSightModelIds(runState, RoomType.Event, FormatCoord(point.coord))
            .ToHashSet();
        if (reservedIds.Count == 0)
        {
            return candidates;
        }

        var unreserved = candidates
            .Where(candidate => !reservedIds.Contains(candidate.Id))
            .ToList();
        return unreserved.Count > 0 ? unreserved : candidates;
    }

    private static bool IsRootSightEventStillValidForEntry(RunState runState, EventModel eventModel)
    {
        if (!eventModel.IsAllowed(runState))
        {
            return false;
        }

        if (!runState.VisitedEventIds.Contains(eventModel.Id))
        {
            return true;
        }

        return TryGetActRoomSet(runState.Act) is { } rooms &&
            AreRootSightUniqueEventsExhausted(runState, rooms);
    }

    private static bool AreRootSightUniqueEventsExhausted(RunState runState, RoomSet rooms)
    {
        return rooms.events.Count > 0 &&
            !rooms.events.Any(candidate =>
                candidate.IsAllowed(runState) &&
                !runState.VisitedEventIds.Contains(candidate.Id));
    }

    private static void CommitRootSightEventQueueForEntry(RunState runState, EventModel eventModel)
    {
        if (TryGetActRoomSet(runState.Act) is not { } rooms)
        {
            return;
        }

        rooms.EnsureNextEventIsValid(runState);
        if (rooms.events.Count == 0)
        {
            return;
        }

        var currentIndex = rooms.eventsVisited % rooms.events.Count;
        var selectedIndex = FindRootSightEventIndex(rooms.events, eventModel.Id, currentIndex);
        if (selectedIndex < 0 || selectedIndex == currentIndex)
        {
            return;
        }

        (rooms.events[currentIndex], rooms.events[selectedIndex]) = (rooms.events[selectedIndex], rooms.events[currentIndex]);
    }

    private static int FindRootSightEventIndex(
        IReadOnlyList<EventModel> events,
        ModelId eventId,
        int startIndex)
    {
        for (var offset = 0; offset < events.Count; offset++)
        {
            var index = (startIndex + offset) % events.Count;
            if (events[index].Id == eventId)
            {
                return index;
            }
        }

        return -1;
    }
}
