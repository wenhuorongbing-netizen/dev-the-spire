using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static readonly HashSet<RoomType> RootSightUnknownBlacklist =
    [
        RoomType.Shop,
        RoomType.Treasure,
        RoomType.RestSite,
        RoomType.Boss
    ];

    private static readonly HashSet<RoomType> RootSightAllowedUnknownRoomTypes =
    [
        RoomType.Monster,
        RoomType.Elite,
        RoomType.Event
    ];

    private static RoomType RollRootSightUnknownRoomType(IRunState runState, MapPoint point)
    {
        var blacklist = RunManager.BuildRoomTypeBlacklist(runState.CurrentMapPointHistoryEntry, point.Children)
            .Concat(RootSightUnknownBlacklist)
            .ToHashSet();
        return PeekRootSightUnknownRoomType(runState, point, blacklist);
    }

    private static RoomType PeekRootSightUnknownRoomType(IRunState runState, MapPoint point, IEnumerable<RoomType> blacklist)
    {
        if (runState.UnlockState.NumberOfRuns == 0)
        {
            var unknownVisits = runState.MapPointHistory
                .SelectMany(entries => entries)
                .Count(entry => entry.MapPointType == MapPointType.Unknown);
            if (unknownVisits < 2)
            {
                return RoomType.Event;
            }

            if (unknownVisits == 2)
            {
                return RoomType.Monster;
            }
        }

        var roomTypes = new[]
            {
                RoomType.Monster,
                RoomType.Elite,
                RoomType.Treasure,
                RoomType.Shop,
                RoomType.Event
            }
            .Except(blacklist)
            .ToHashSet();
        roomTypes = Hook.ModifyUnknownMapPointRoomTypes(runState, roomTypes)
            .Where(RootSightAllowedUnknownRoomTypes.Contains)
            .ToHashSet();
        if (roomTypes.Count == 0)
        {
            return RoomType.Unassigned;
        }

        var roomType = roomTypes.Contains(RoomType.Event)
            ? RoomType.Event
            : roomTypes.Order().First();
        var fork = CreateRootSightPreviewRng(runState, point, "unknown_room_type");
        var roll = fork.NextFloat();
        var cumulative = 0f;
        foreach (var (candidate, odds) in GetRootSightUnknownRoomOdds(runState))
        {
            if (roomTypes.Contains(candidate) && odds >= 0f)
            {
                cumulative += odds;
                if (roll <= cumulative)
                {
                    roomType = candidate;
                    break;
                }
            }
        }

        return roomType;
    }

    private static void CommitRootSightUnknownRoomType(RunManager runManager, RoomType committedRoomType)
    {
        var runState = runManager.DebugOnlyGetState();
        var current = runState?.CurrentMapPoint;
        if (runState == null || current == null)
        {
            return;
        }

        var blacklist = RunManager.BuildRoomTypeBlacklist(runState.CurrentMapPointHistoryEntry, current.Children)
            .Concat(RootSightUnknownBlacklist)
            .ToHashSet();
        CommitRootSightUnknownRoomType(runState, committedRoomType, blacklist);
    }

    private static void CommitRootSightUnknownRoomType(
        RunState runState,
        RoomType committedRoomType,
        IEnumerable<RoomType> blacklist)
    {
        if (runState.UnlockState.NumberOfRuns == 0)
        {
            var unknownVisits = runState.MapPointHistory
                .SelectMany(entries => entries)
                .Count(entry => entry.MapPointType == MapPointType.Unknown);
            if (unknownVisits <= 2)
            {
                return;
            }
        }

        var roomTypes = new[]
            {
                RoomType.Monster,
                RoomType.Elite,
                RoomType.Treasure,
                RoomType.Shop,
                RoomType.Event
            }
            .Except(blacklist)
            .ToHashSet();
        roomTypes = Hook.ModifyUnknownMapPointRoomTypes(runState, roomTypes)
            .Where(RootSightAllowedUnknownRoomTypes.Contains)
            .ToHashSet();
        if (roomTypes.Count == 0)
        {
            return;
        }

        runState.Rng.UnknownMapPoint.NextFloat();
        foreach (var (roomType, baseOdds) in GetRootSightUnknownRoomBaseOdds(runState))
        {
            if (committedRoomType == roomType)
            {
                SetRootSightUnknownRoomOdds(runState, roomType, baseOdds);
            }
            else if (roomTypes.Contains(roomType))
            {
                var currentOdds = GetRootSightUnknownRoomOdds(runState, roomType);
                var increase = Hook.ModifyOddsIncreaseForUnrolledRoomType(runState, roomType, baseOdds);
                SetRootSightUnknownRoomOdds(runState, roomType, currentOdds + increase);
            }
        }
    }
}
