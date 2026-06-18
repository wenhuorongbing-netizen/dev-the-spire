using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientHighRiskSourceGuardTests
{
    [Fact]
    public void UrdaRootSightPreviewsMapNodesWithoutConsumingRealUnknownRoomState()
    {
        var rootSight = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightSelection.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightSelectionCommit.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightMarkers.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightTargets.cs"));
        var rootSightSelection = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightSelection.cs");
        var rootSightSelectionCommit = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightSelectionCommit.cs");
        var rootSightRouting = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightRouting.cs");
        var rootSightEntryLookup = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightEntryLookup.cs");
        var rootSightEntryCommit = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightEntryCommit.cs");
        var rootSightRoutingSource = string.Join(
            Environment.NewLine,
            rootSightRouting,
            rootSightEntryLookup,
            rootSightEntryCommit);
        var rootSightStatus = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightStatus.cs");
        var rootSightHover = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightHover.cs");
        var rootSightHoverText = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightHoverText.cs");
        var mapHoverComposer = ReadRepoText("EZMicroBalanceCode", "Map", "SpirePlusMapPointHoverComposer.cs");
        var rootSightPreview = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightPreviewGeneration.cs");
        var rootSightEncounters = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightEncounters.cs");
        var rootSightEvents = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightEvents.cs");
        var rootSightReservations = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightReservations.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightReservedIds.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightReservationQueues.cs"));
        var rootSightUnknown = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightUnknown.cs");
        var rootSightUnknownOdds = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightUnknownOdds.cs");
        var rootSightRoomPatches = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRootSightRoomPatches.cs");
        var rootSightPreviewSource = string.Join(
            Environment.NewLine,
            rootSightPreview,
            rootSightEncounters,
            rootSightEvents,
            rootSightUnknown,
            rootSightUnknownOdds);
        var runLifecycle = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RunLifecycle.cs");
        var urdaRunHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRunHook.cs");
        var rootSightRoomTypeLookup = SliceBetween(
            rootSightRouting,
            "internal static bool TryGetRootSightRoomTypeForCurrentPoint",
            "internal static bool TryGetRootSightModelForCurrentPoint");

        AssertSourceContains(
            rootSightPreviewSource,
            "RootSightUnknownBlacklist",
            "RootSightAllowedUnknownRoomTypes",
            "PeekRootSightUnknownRoomType(runState, point, blacklist)",
            "Hook.ModifyUnknownMapPointRoomTypes(runState, roomTypes)",
            ".Where(RootSightAllowedUnknownRoomTypes.Contains)",
            "CreateRootSightPreviewRng(runState, point, \"unknown_room_type\")",
            "GetRootSightUnknownRoomOdds(runState)",
            "TryPeekRootSightEncounterForPoint(runState, point, roomType)",
            "rooms.normalEncounters",
            "rooms.normalEncountersVisited",
            "rooms.eliteEncounters",
            "rooms.eliteEncountersVisited",
            "GetRootSightEncounterPreviewCandidates(rooms, roomType)",
            "FilterRootSightReservedEncounterCandidates(",
            "FilterRootSightReservedEventCandidates(",
            "GetReservedRootSightModelIds(runState, roomType, FormatCoord(point.coord))",
            "GetReservedRootSightModelIds(runState, RoomType.Event, FormatCoord(point.coord))",
            "Where(candidate => !reservedIds.Contains(candidate.Id))",
            "return unreserved.Count > 0 ? unreserved : candidates",
            "return candidates[0]",
            "var startIndex = visited % source.Count",
            "source[(startIndex + offset) % source.Count]",
            "CommitRootSightEncounterQueueForEntry(",
            "FindRootSightEncounterIndex(encounters, encounter.Id, currentIndex)",
            "CommitRootSightUnknownRoomType(runState, committedRoomType, blacklist)",
            "runState.Rng.UnknownMapPoint.NextFloat()",
            "AccessTools.Field(typeof(UnknownMapPointOdds), \"_baseOdds\")",
            "Hook.ModifyOddsIncreaseForUnrolledRoomType(runState, roomType, baseOdds)",
            "Hook.ModifyNextEvent(concreteRunState, nextEvent)",
            "PeekRootSightNextValidEvent(concreteRunState, rooms, point)",
            "IsRootSightEventStillValidForEntry(RunState runState, EventModel eventModel)",
            "AreRootSightUniqueEventsExhausted(runState, rooms)",
            "!rooms.events.Any(candidate",
            "rooms.EnsureNextEventIsValid(runState)",
            "FindRootSightEventIndex(rooms.events, eventModel.Id, currentIndex)",
            "(rooms.events[currentIndex], rooms.events[selectedIndex]) = (rooms.events[selectedIndex], rooms.events[currentIndex])",
            "new Rng(",
            "runState.Rng.UnknownMapPoint",
            "root_sight_{scope}_act_{runState.CurrentActIndex}_coord_{point.coord.col}_{point.coord.row}_counter_{source.Counter}");
        Assert.DoesNotContain(".Odds.UnknownMapPoint.Roll(", rootSightPreviewSource, StringComparison.Ordinal);
        Assert.DoesNotContain("runState.Odds.UnknownMapPoint.Roll(", rootSightPreviewSource, StringComparison.Ordinal);
        Assert.DoesNotContain(".Roll(", rootSightPreviewSource, StringComparison.Ordinal);
        Assert.DoesNotContain("PullNextEncounter", rootSightPreviewSource, StringComparison.Ordinal);
        Assert.DoesNotContain("PullNextEvent", rootSightPreviewSource, StringComparison.Ordinal);

        AssertSourceContains(
            rootSight,
            "var mapScreen = NMapScreen.Instance",
            "selection could not start because the map screen is not available",
            "return false",
            "mapScreen.Open(isOpenedFromTopBar: true)",
            "mapScreen.RefreshAllPointVisuals()",
            "GetActiveRootSightSelectionPlayer()",
            "selection cleared after run context changed",
            "!ReferenceEquals(player.RunState, runState)",
            "!runState.Players.Contains(player)",
            "ClearStaleRootSightPreview(",
            "RootSightEyes = Math.Min(RootSightStartingEyes, progress.RootSightEyes + 1)",
            "RemoveQuestMarker<UrdaRootSightMapQuestMarker>(point)",
            "FormatRootSightMarkedCoord(player.RunState.CurrentActIndex, coord)",
            "TryFindRootSightPreview(progress, actIndex, coord, out _)",
            "RestoreRootSightPreviewMarkers(ActMap map, int actIndex)",
            "GetRootSightPreviews(GetProgress(player).RootSightPreviewRecords)",
            "map.GetPoint(coord)",
            "ClearUnreachableRootSightPreviews(Player player, IRunState runState)",
            "!IsFutureReachableRootSightTarget(runState, point)",
            "!IsRootSightPreviewStillValidForEntry(concreteRunState, preview)",
            "point.PointType != preview.PointType",
            "EnsureQuestMarker<UrdaRootSightMapQuestMarker>(point)",
            "point.PointType is not (MapPointType.Monster or MapPointType.Unknown or MapPointType.Elite)",
            "IsFutureReachableRootSightTarget(player, point)",
            "IsFutureReachableRootSightTarget(IRunState runState, MapPoint point)",
            "new Queue<MapPoint>(current.Children)",
            "point.coord.row <= current.coord.row");
        Assert.DoesNotContain("HasBlockingRootSightQuestMarker(point)", rootSight, StringComparison.Ordinal);
        Assert.DoesNotContain("player.RunState.CurrentActIndex != 0", rootSight, StringComparison.Ordinal);
        Assert.DoesNotContain("RootSightPreviewRecords =", rootSightSelection, StringComparison.Ordinal);
        Assert.DoesNotContain("mapScreen.Open", rootSightSelectionCommit, StringComparison.Ordinal);

        AssertSourceContains(
            runLifecycle,
            "public static void AfterMapGenerated(ActMap map, int actIndex)",
            "RestoreRootSightPreviewMarkers(map, actIndex)");
        AssertSourceContains(
            runLifecycle,
            "ClearUnreachableRootSightPreviews(player, runState)");
        AssertBefore(
            runLifecycle,
            "ClearUnreachableRootSightPreviews(player, runState)",
            "if (runState.CurrentActIndex != 0)");
        AssertSourceContains(
            urdaRunHook,
            "public override Task AfterMapGenerated(ActMap map, int actIndex)",
            "UrdaBlessingService.AfterMapGenerated(map, actIndex)");

        AssertSourceContains(
            rootSightRoutingSource,
            "if (pointType == MapPointType.Unknown)",
            "CommitRootSightUnknownRoomType(runManager, preview.RoomType)",
            "IsRootSightPreviewStillValidForEntry(runState, preview)",
            "runState.Players.Count > 1",
            "TryMarkRootSightCommittedForCurrentPoint(runState)",
            "CommitRootSightEncounterQueueForEntry(runState, roomType, encounter)",
            "ConditionalWeakTable<RunState, HashSet<string>>",
            "RootSightCommittedEntryKeys.GetOrCreateValue(runState)",
            "committedForRun.Add",
            "catch (Exception ex)",
            "Urda Root Eyes preview entry failed",
            "IsRootSightEventStillValidForEntry(runState, eventModel)",
            "CommitRootSightEventQueueForEntry(runState, eventModel)",
            "runState.AddVisitedEvent(eventModel)",
            "ConsumeRootSightPreviewForCurrentPoint(runState, preview)",
            "private static void ConsumeRootSightPreviewForCurrentPoint(RunState runState, RootSightPreview preview)",
            "RootSightPreviewRecords = FormatRootSightPreviews(previews)",
            "RemoveQuestMarker<UrdaRootSightMapQuestMarker>(current)");
        Assert.DoesNotContain("CommitRootSightUnknownRoomType", rootSightRoomTypeLookup, StringComparison.Ordinal);
        Assert.DoesNotContain(".Odds.UnknownMapPoint.Roll(", rootSightRoutingSource, StringComparison.Ordinal);

        AssertSourceContains(
            rootSightReservations,
            "AvoidRootSightReservedModelForCurrentNonPreviewRoom",
            "TryFindRootSightPreviewForCurrentPoint(runManager, pointType, out var currentPreview)",
            "MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopFeature",
            "runState.Players.Count > 1",
            "GetReservedRootSightModelIds(runState, roomType, currentCoord).ToHashSet()",
            "preview.Coord != currentCoord",
            "TryMoveReservedRootSightEncounterOffQueueHead(runState, roomType, reservedIds)",
            "TryMoveReservedRootSightEventOffQueueHead(runState, reservedIds)",
            "reservedIds.Contains(encounters[currentIndex].Id)",
            "(encounters[currentIndex], encounters[candidateIndex]) = (encounters[candidateIndex], encounters[currentIndex])",
            "rooms.EnsureNextEventIsValid(runState)",
            "AreRootSightUniqueEventsExhausted(runState, rooms)",
            "TryDeserializeModelId(preview.ModelId, out var id)");
        AssertSourceContains(
            rootSightRoomPatches,
            "UrdaBlessingService.AvoidRootSightReservedModelForCurrentNonPreviewRoom(__instance, roomType, mapPointType)");

        AssertSourceContains(
            rootSightHover,
            "TryGetRootSightPreviewTitle(preview, out var title)",
            "IsRootSightPreviewStillValidForEntry(runState, preview)");
        AssertSourceContains(
            rootSightHoverText,
            "private static bool TryGetRootSightPreviewTitle(RootSightPreview preview, out LocString title)",
            "private static bool TryGetRootSightPreviewDescription(RootSightPreview preview, out LocString description)",
            "var id = ModelId.Deserialize(preview.ModelId)",
            "ModelDb.GetByIdOrNull<EventModel>(id)",
            "title = eventModel.Title",
            "ModelDb.GetByIdOrNull<EncounterModel>(id)",
            "title = encounter.Title",
            "description = new LocString(\"ancients\", \"EZMB_URDA.root_sight.map_hover.preview_description\")",
            "eventModel.GameInfoOptions",
            "NormalizeRootSightEventOptionPreview(option.GetFormattedText())",
            ".Distinct(StringComparer.Ordinal)",
            ".Take(3)",
            "if (optionPreview.Length > 220)",
            "optionPreview = optionPreview[..217] + \"...\";",
            "description = new LocString(\"ancients\", \"EZMB_URDA.root_sight.map_hover.event_preview_description\")",
            "description.Add(\"Options\", optionPreview)",
            ".Replace(\"\\r\", \" \", StringComparison.Ordinal)",
            ".Replace(\"\\n\", \" \", StringComparison.Ordinal)");
        Assert.DoesNotContain("ClearStaleRootSightPreview(", rootSightHover, StringComparison.Ordinal);
        AssertSourceContains(
            mapHoverComposer,
            "SpirePlusMapPointHoverComposer",
            "CollectHoverTips(__instance).ToList()",
            "UrdaBlessingService.TryGetRootSightHoverTip(pointNode.Point, out var rootSightTip)",
            "FiremarkedEliteMapHoverPatch.TryCreateHoverTip(pointNode.Point, out var firemarkTip)",
            "BannerRoomMapHoverPatch.TryCreateHoverTip(pointNode.Point, out var bannerTip)",
            "TryCreateDeepBranchHoverTip(pointNode.Point, out var deepBranchTip)",
            "NHoverTipSet.Remove(__instance)",
            "NHoverTipSet.CreateAndShow(__instance, hoverTips)");

        AssertSourceContains(
            rootSightStatus,
            "ResetRootSightTransientState()",
            "relic.Status = progress.RootSightEyes > 0",
            "RelicStatus.Active",
            "RelicStatus.Disabled");
        Assert.DoesNotContain("CurrentActIndex == 0", rootSightStatus, StringComparison.Ordinal);
    }
}
