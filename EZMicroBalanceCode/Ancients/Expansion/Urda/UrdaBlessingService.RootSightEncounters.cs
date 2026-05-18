using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static EncounterModel? TryPeekRootSightEncounterForPoint(
        IRunState runState,
        MapPoint point,
        RoomType roomType)
    {
        if (TryGetActRoomSet(runState.Act) is not { } rooms)
        {
            return null;
        }

        var candidates = GetRootSightEncounterPreviewCandidates(rooms, roomType);
        if (runState is RunState concreteRunState)
        {
            candidates = FilterRootSightReservedEncounterCandidates(
                concreteRunState,
                point,
                roomType,
                candidates);
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        var fork = CreateRootSightPreviewRng(runState, point, $"{roomType.ToString().ToLowerInvariant()}_encounter");
        return fork.NextItem(candidates);
    }

    private static IReadOnlyList<EncounterModel> GetRootSightEncounterPreviewCandidates(
        RoomSet rooms,
        RoomType roomType)
    {
        var source = roomType switch
        {
            RoomType.Monster => rooms.normalEncounters,
            RoomType.Elite => rooms.eliteEncounters,
            _ => []
        };
        if (source.Count == 0)
        {
            return [];
        }

        var visited = roomType == RoomType.Monster
            ? rooms.normalEncountersVisited
            : rooms.eliteEncountersVisited;
        if (visited <= 0 || visited >= source.Count)
        {
            return source;
        }

        return source.Skip(visited).ToList();
    }

    private static IReadOnlyList<EncounterModel> FilterRootSightReservedEncounterCandidates(
        RunState runState,
        MapPoint point,
        RoomType roomType,
        IReadOnlyList<EncounterModel> candidates)
    {
        var reservedIds = GetReservedRootSightModelIds(runState, roomType, FormatCoord(point.coord))
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

    private static void CommitRootSightEncounterQueueForEntry(
        RunState runState,
        RoomType roomType,
        EncounterModel encounter)
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
        var selectedIndex = FindRootSightEncounterIndex(encounters, encounter.Id, currentIndex);
        if (selectedIndex < 0 || selectedIndex == currentIndex)
        {
            return;
        }

        (encounters[currentIndex], encounters[selectedIndex]) = (encounters[selectedIndex], encounters[currentIndex]);
    }

    private static int FindRootSightEncounterIndex(
        IReadOnlyList<EncounterModel> encounters,
        ModelId encounterId,
        int startIndex)
    {
        for (var offset = 0; offset < encounters.Count; offset++)
        {
            var index = (startIndex + offset) % encounters.Count;
            if (encounters[index].Id == encounterId)
            {
                return index;
            }
        }

        return -1;
    }
}
