using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class AncientHighRiskSourceGuardTests
{
    [Fact]
    public void AncientRunAndCombatHooksKeepSingleDispatchOwnership()
    {
        var morviRunHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviRunHook.cs");
        var morviCombatHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviCombatHook.cs");
        var lothaRunHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");
        var lothaCombatHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaCombatHook.cs");
        var urdaRunHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRunHook.cs");
        var urdaCombatHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaCombatHook.cs");

        AssertSourceContains(
            morviRunHook,
            "BeforeCombatStart",
            "AfterCardChangedPiles",
            "AfterCombatEnd");
        Assert.DoesNotContain("AfterPlayerTurnStartEarly", morviRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("AfterTurnEnd", morviRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("ModifyCardPlayCount", morviRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryModifyEnergyCostInCombat", morviRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("BeforeCardPlayed", morviRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("AfterCardPlayed", morviRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("AfterCardDrawn", morviRunHook, StringComparison.Ordinal);
        AssertSourceContains(
            morviCombatHook,
            "AfterPlayerTurnStartEarly",
            "AfterTurnEnd",
            "ModifyCardPlayCount",
            "TryModifyEnergyCostInCombat",
            "BeforeCardPlayed",
            "AfterCardPlayed",
            "AfterCardDrawn");
        AssertRepoPathDoesNotExist("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviHooks.cs");

        AssertSourceContains(
            lothaRunHook,
            "BeforeCombatStart",
            "AfterCardChangedPiles",
            "AfterCombatEnd",
            "AfterDamageReceived",
            "TryModifyRewardsLate",
            "ShouldDieLate",
            "ShouldDie",
            "AfterPreventingDeath");
        foreach (var combatOnly in new[]
        {
            "AfterPlayerTurnStartEarly",
            "AfterTurnEnd",
            "ModifyCardPlayCount",
            "ShouldPlay",
            "AfterCardPlayed",
            "TryModifyEnergyCostInCombat",
            "TryModifyStarCost",
            "ModifyPowerAmountGivenAdditive",
            "TryModifyPowerAmountReceived",
            "AfterPowerAmountChanged"
        })
        {
            Assert.DoesNotContain(combatOnly, lothaRunHook, StringComparison.Ordinal);
        }

        AssertSourceContains(
            lothaCombatHook,
            "AfterPlayerTurnStartEarly",
            "AfterTurnEnd",
            "ModifyCardPlayCount",
            "ShouldPlay",
            "AfterCardPlayed",
            "TryModifyEnergyCostInCombat",
            "TryModifyStarCost",
            "ModifyPowerAmountGivenAdditive",
            "TryModifyPowerAmountReceived",
            "AfterPowerAmountChanged");

        AssertRepoPathDoesNotExist("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaHooks.cs");

        Assert.DoesNotContain("public override Task AfterCardPlayed", urdaRunHook, StringComparison.Ordinal);
        Assert.Contains("public override Task AfterCardPlayed", urdaCombatHook, StringComparison.Ordinal);
    }

    [Fact]
    public void UrdaCombatVictoryUsesRoomScopedRunState()
    {
        var lifecycle = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RunLifecycle.cs");
        var afterCombatVictory = SliceFrom(lifecycle, "public static async Task AfterCombatVictory(CombatRoom room)");

        AssertSourceContains(
            afterCombatVictory,
            "var runState = room.CombatState.RunState;",
            "runState.Players.Where(player => player.IsActiveForHooks)");
        Assert.DoesNotContain("RunManager.Instance.DebugOnlyGetState()", afterCombatVictory, StringComparison.Ordinal);
    }

    [Fact]
    public void UrdaStateCleanupAvoidsGuessingAndRefreshesVisibleTrialBranchState()
    {
        var seedbed = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.Seedbed.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedbedCombat.cs"));
        var seedBank = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBank.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtraction.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtractionCommit.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtractionState.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankStatus.cs"));
        var state = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.State.cs");

        Assert.DoesNotContain("FirstOrDefault(candidate => candidate.RootblightLevel == rootblight.RootblightLevel)", seedbed, StringComparison.Ordinal);
        Assert.DoesNotContain("RootDeckService.FindRootFamilyCards(card.Owner)", seedbed, StringComparison.Ordinal);
        AssertSourceContains(
            seedbed,
            "card is WitheredHusk",
            "card is RootFamilyCard rootblight",
            "RootDeckService.CanHoldRootblightBySeedbed(rootblight)",
            "RootDeckService.TryHoldRootblightBySeedbed(rootblight)",
            "card.DeckVersion == null",
            "Planting skipped play, discard, and Exhaust synergies");
        AssertSourceContains(
            seedBank,
            "try",
            "finally",
            "foreach (var card in cards)",
            "AncientCardHelpers.RemoveUnpiledRunCard(card)",
            "player.RunState.Players.Count > 1",
            "single-player only until host-authoritative reward selection sync is implemented");
        AssertSourceContains(
            state,
            "AncientPlayerState.SyncDeck(",
            "GetSelectedBlessing(player) == UrdaBlessingIds.TrialBranch",
            "RefreshTrialBranchEnchantment(player)");
    }

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
            "IsRootSightPreviewStillValidForEntry(runState, preview)",
            "ModelDb.GetByIdOrNull<EventModel>(id)",
            "title = eventModel.Title",
            "ModelDb.GetByIdOrNull<EncounterModel>(id)",
            "title = encounter.Title");
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

    [Fact]
    public void PickupRewardCompensationAndLockoutPatchesStayScoped()
    {
        var hornSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PaelsHornPhase1Patch.cs");
        var pickupSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var pickupDispatch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PickupRewardPatches.cs");
        var sereTalonVisualSource = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualAssetPaths.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualRelicModelRoutes.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualNodeRoutes.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualTextures.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualRouteLog.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualPatches.cs"));
        var tanxClawsTuningSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "TanxClawsMaulTuningPatches.cs");
        var sealSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SealOfGoldPatches.cs");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");

        AssertSourceContains(
            hornSource,
            "owner.RunState.CreateCard<Relax>(owner)",
            "CardCmd.Upgrade(upgradedRelax)",
            "await CardPileCmd.Add(normalRelax, PileType.Deck)",
            "await CardPileCmd.Add(upgradedRelax, PileType.Deck)");

        AssertSourceContains(
            pickupSource,
            "if (blackStar.Owner.RunState.CurrentActIndex < 2)",
            "RelicFactory.PullNextRelicFromFront(blackStar.Owner).ToMutable()",
            "await RelicCmd.Obtain(relic, blackStar.Owner)",
            "new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 2)",
            "CardSelectCmd.FromDeckForUpgrade(warHammer.Owner, prefs)",
            "CardCmd.Upgrade(cards, CardPreviewStyle.HorizontalLayout)",
            "SozuPotionGatePatch.BeginInitialPotionFill(sozu.Owner)",
            "SozuPotionGatePatch.EndInitialPotionFill(sozu.Owner)",
            "while (sozu.Owner.HasOpenPotionSlots)",
            "PotionFactory.CreateRandomPotionOutOfCombat",
            "PotionCmd.TryToProcure(potion, sozu.Owner)",
            "if (InitialPotionFillOwners.Contains(player) && player == __instance.Owner)",
            "EctoplasmGoldGatePatch.BeginInitialGold(ectoplasm.Owner)",
            "await PlayerCmd.GainGold(250m, ectoplasm.Owner)",
            "EctoplasmGoldGatePatch.EndInitialGold(ectoplasm.Owner)",
            "if (InitialGoldOwners.Contains(player) && player == __instance.Owner)",
            "for (var i = 0; i < 2; i++)",
            "sealOfGold.Owner.RunState.CreateCard<Debt>(sealOfGold.Owner)",
            "DebtCardPatch.ConfigureDebt(debt)",
            "CardPileCmd.Add(debt, PileType.Deck)");
        Assert.DoesNotContain("case Claws", pickupDispatch, StringComparison.Ordinal);
        AssertSourceContains(
            pickupSource,
            "[HarmonyPatch(typeof(SereTalon), nameof(SereTalon.AfterObtained))]",
            "private const int CurseOfferCount = 4",
            "private const int CursePickCount = 1",
            "private const int NormalWishCount = 2",
            "private const int UpgradedWishCount = 1",
            "CardSelectCmd.FromSimpleGrid",
            "new BlockingPlayerChoiceContext()",
            "SERE_TALON.selectionScreenPrompt",
            "Cancelable = false",
            "RequireManualConfirmation = true",
            "AncientCardHelpers.RemoveUnpiledRunCard(curse)",
            "CardCmd.Upgrade(wish, CardPreviewStyle.None)");
        AssertSourceContains(
            tanxClawsTuningSource,
            "[HarmonyPatch(typeof(Claws), nameof(Claws.AfterObtained))]",
            "CardSelectCmd.FromDeckForTransformation",
            "owner.RunState.CreateCard<Maul>(owner)",
            "maul.UpgradeInternal()",
            "CardCmd.Upgrade(maul, CardPreviewStyle.None)",
            "CardCmd.Transform(transformations, owner.PlayerRng.Transformations)");
        Assert.DoesNotContain("SereTalon", tanxClawsTuningSource, StringComparison.Ordinal);
        AssertSourceContains(
            sereTalonVisualSource,
            "get_IconPath",
            "get_PackedIconPath",
            "get_PackedIconOutlinePath",
            "get_BigIconPath",
            "get_Icon",
            "get_IconOutline",
            "get_BigIcon",
            "NEventOptionButton._Ready",
            "NRelic keeps its IconSize private",
            "Outline.Visible is",
            "Ancient option buttons assign the relic icon directly during _Ready()",
            "Relic-bar and inspect nodes can reload after the model texture getters",
            "TryApply",
            "TryApplyPath",
            "TryApplyEventOptionButton",
            "TryApplyRelicNode",
            "IsNodeReady()",
            "InvalidOperationException",
            "CanUsePath(iconPath)",
            "ResourceLoader.Exists(iconPath)",
            "TryApplyPackedTexture",
            "LoadPackedTexture",
            "TryApplyTexture",
            "ResourceLoader.Load<Texture2D>(SereTalonVisualAssetPaths.PackedIcon",
            "GetNodeOrNull<TextureRect>(\"%RelicIcon\")",
            "button.Option?.Relic is not SereTalon",
            "PreloadManager.Cache.GetTexture2D(iconPath)",
            "texture is null",
            "SereTalonVisualRouteLog.SkippedPathOnce(iconPath, \"texture did not load\")",
            "Vakuu Sere Talon Ancient option icon route skipped",
            "resource path does not exist",
            "icon route skipped because",
            "relic is not SereTalon",
            "sere_talon_spire_plus.png",
            "RelicModel packed icon path",
            "RelicModel big icon path",
            "RelicModel packed icon texture",
            "RelicModel big icon texture",
            "Ancient event option button",
            "Vakuu Sere Talon icon route active");
        Assert.DoesNotContain("__instance is Claws", sereTalonVisualSource, StringComparison.Ordinal);
        Assert.True(
            File.Exists(RepoPath("EZMicroBalance", "images", "relics", "sere_talon_spire_plus.png")),
            "Sere Talon needs a Spire Plus-owned in-game icon so it is not visually confused with Tanx Claws.");
        Assert.True(
            File.Exists(RepoPath("EZMicroBalance", "images", "relics", "big", "sere_talon_spire_plus.png")),
            "Sere Talon needs a separate big relic icon for inspect/hover surfaces.");

        AssertSourceContains(
            sealSource,
            "__result += sealOfGold.DynamicVars.Energy.BaseValue",
            "__result = Task.CompletedTask");

        Assert.Contains("immediately obtain 1 random Relic", relics["BLACK_STAR.description"], StringComparison.Ordinal);
        Assert.Contains("fill all empty Potion slots", relics["SOZU.description"], StringComparison.Ordinal);
        Assert.Contains("gain 250 Gold", relics["ECTOPLASM.description"], StringComparison.Ordinal);
        Assert.Contains("Add 2 playable Debt", relics["SEAL_OF_GOLD.description"], StringComparison.Ordinal);
    }

    [LocalSourceFact]
    public void CoreAncientRelicRoutesAndIconSurfacesStayCompatible()
    {
        var vakuuEventSource = ReadLocalCoreText("Models", "Events", "Vakuu.cs");
        var tanxEventSource = ReadLocalCoreText("Models", "Events", "Tanx.cs");
        var sereTalonSource = ReadLocalCoreText("Models", "Relics", "SereTalon.cs");
        var clawsSource = ReadLocalCoreText("Models", "Relics", "Claws.cs");
        var coreNRelicSource = ReadLocalCoreText("Nodes", "Relics", "NRelic.cs");
        var coreEventOptionButtonSource = ReadLocalCoreText("Nodes", "Events", "NEventOptionButton.cs");
        var coreRelicRewardSource = ReadLocalCoreText("Rewards", "RelicReward.cs");
        var coreInspectRelicScreenSource = ReadLocalCoreText("Nodes", "Screens", "InspectScreens", "NInspectRelicScreen.cs");

        AssertSourceContains(vakuuEventSource, "RelicOption<SereTalon>()");
        Assert.DoesNotContain("RelicOption<Claws>()", vakuuEventSource, StringComparison.Ordinal);
        AssertSourceContains(tanxEventSource, "RelicOption<Claws>()");
        Assert.DoesNotContain("RelicOption<SereTalon>()", tanxEventSource, StringComparison.Ordinal);
        AssertSourceContains(
            sereTalonSource,
            "public sealed class SereTalon : RelicModel",
            "new DynamicVar(\"Curses\", 2m)",
            "new DynamicVar(\"Wishes\", 3m)",
            "base.Owner.RunState.Rng.Niche.NextItem(availableCurses)",
            "base.Owner.RunState.CreateCard(ModelDb.Card<Wish>(), base.Owner)");
        AssertSourceContains(
            clawsSource,
            "public sealed class Claws : RelicModel",
            "new CardsVar(6)",
            "HoverTipFactory.FromCardWithCardHoverTips<Maul>()",
            "new CardTransformation(c, CreateMaulFromOriginal(c, forPreview: true))",
            "base.Owner.RunState.CreateCard<Maul>(base.Owner)",
            "CardCmd.Transform(transformations, base.Owner.PlayerRng.Transformations)");
        AssertSourceContains(
            coreEventOptionButtonSource,
            "Option.Relic.Icon",
            "Option.Relic.IconOutline",
            "GetNode<TextureRect>(\"%RelicIcon\")");
        AssertSourceContains(
            coreNRelicSource,
            "case IconSize.Small:",
            "Icon.Texture = Model.Icon",
            "Outline.Visible = true",
            "Outline.Texture = Model.IconOutline",
            "case IconSize.Large:",
            "Icon.Texture = Model.BigIcon",
            "Outline.Visible = false");
        AssertSourceContains(
            coreRelicRewardSource,
            "public override TextureRect CreateIcon()",
            "textureRect.Texture = _relic.BigIcon");
        AssertSourceContains(
            coreInspectRelicScreenSource,
            "_relicImage.Texture = relicModel.BigIcon");
    }

    [Fact]
    public void DraftAndGeneratedCardFlowsRemoveUnselectedTemporaryCards()
    {
        var pickupSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var vakuSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var debtSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "DebtAndCardPatches.cs");
        var cards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");

        Assert.DoesNotContain("CreateSereTalonCurseDraft", pickupSource, StringComparison.Ordinal);

        AssertSourceContains(
            vakuSource,
            "combatState.RoundNumber != 1",
            "ModelDb.AllCharacterCardPools",
            "ModelDb.CardPool<ColorlessCardPool>()",
            ".Where(IsChoicesParadoxEligibleRare)",
            ".Distinct()",
            "CardFactory.GetDistinctForCombat",
            "CardCmd.ApplyKeyword(card, CardKeyword.Retain)",
            "foreach (var card in generated.Where(card => card != selected))",
            "combatState.RemoveCard(card)",
            "await AncientCardHelpers.TryAddGeneratedCardToCombat(selected, PileType.Hand, player)",
            "card.Rarity == CardRarity.Rare",
            "card.Type is not CardType.Curse and not CardType.Status and not CardType.Quest",
            "!card.Keywords.Contains(CardKeyword.Unplayable)",
            "card.CanBeGeneratedInCombat",
            "card.CanBeGeneratedByModifiers");

        AssertSourceContains(
            debtSource,
            "case Enthralled enthralled:",
            "await CreatureCmd.GainBlock(enthralled.Owner.Creature, 10m",
            "DebtCardPatch.ConfigureDebt(debt)",
            "debt.RemoveKeyword(CardKeyword.Unplayable)",
            "debt.AddKeyword(CardKeyword.Exhaust)",
            "debt.EnergyCost.SetCustomBaseCost(1)");

        Assert.Equal("If this is in your hand, you must play it before other cards. Gain 10 Block. Eternal.", cards["ENTHRALLED.description"]);
        Assert.Equal("Exhaust. When Exhausted, lose 5 Gold.", cards["DEBT.description"]);
    }

    [Fact]
    public void QualityFlameUsesDynamicDrawAndVisibleExhaustKeyword()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "BrightestFlameExhaustDrawPatch.cs");
        var englishCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var simplifiedChineseCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");
        var apiDiscovery = ReadRepoText("docs", "features", "ancients-rework-v4", "api-discovery.md");

        AssertSourceContains(
            source,
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.CanonicalKeywords), MethodType.Getter)",
            "__instance is not BrightestFlame",
            "CardKeyword.Exhaust",
            "ModPatchTarget(typeof(BrightestFlame), \"CanonicalVars\", MethodType.Getter)",
            "dynamicVar is CardsVar cards",
            "new CardsVar(cards.IntValue + ExtraDraw)",
            "Vanilla: Gain Energy(2), Draw(2), LoseMaxHp(1). Upgrade: Energy+1, Draw+1.",
            "upgrade draws 4",
            "Does not affect Pumpkin Candle relic vanilla behavior.");

        Assert.DoesNotContain("DrawExtraAfterVanilla", source, StringComparison.Ordinal);
        Assert.DoesNotContain("CardPileCmd.Draw(choiceContext, 1", source, StringComparison.Ordinal);

        Assert.Equal("Brilliant Flame", englishCards["BRIGHTEST_FLAME.title"]);
        Assert.Contains("{Cards:diff()}", englishCards["BRIGHTEST_FLAME.description"], StringComparison.Ordinal);
        Assert.Contains("{Cards:diff()}", simplifiedChineseCards["BRIGHTEST_FLAME.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("Draw 3 cards", englishCards["BRIGHTEST_FLAME.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("鎶?寮犵墝", simplifiedChineseCards["BRIGHTEST_FLAME.description"], StringComparison.Ordinal);
        Assert.Contains("Brilliant Flame / Brightest Flame", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("BrightestFlame", apiDiscovery, StringComparison.Ordinal);
    }

    [Fact]
    public void PrismaticGemOffColorReplacementKeepsNormalRewardBoundariesAllSlotsAndRunStateClean()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");

        AssertSourceContains(
            source,
            "[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Hooks.Hook), nameof(MegaCrit.Sts2.Core.Hooks.Hook.TryModifyCardRewardOptions))]",
            "HarmonyPrefix",
            "player.Relics.OfType<PrismaticGem>().FirstOrDefault(relic => !relic.IsMelted)",
            "foreach (var listener in runState.IterateHookListeners(null))",
            "listener.TryModifyCardRewardOptions(player, cardRewardOptions, creationOptions)",
            "if (listenerModified)",
            "modifiers.Add(listener)",
            "TryReplaceNormalRewardScreen(prismaticGem, player, cardRewardOptions, creationOptions)",
            "listener.TryModifyCardRewardOptionsLate(player, cardRewardOptions, creationOptions)",
            "if (!creationOptions.Flags.HasFlag(CardCreationFlags.IsCardReward))",
            "creationOptions.Flags.HasFlag(CardCreationFlags.NoCardPoolModifications)",
            "creationOptions.Flags.HasFlag(CardCreationFlags.NoCardModelModifications)",
            "creationOptions.Source == CardCreationSource.Encounter",
            "creationOptions.RarityOdds == CardRarityOddsType.RegularEncounter",
            "creationOptions.CustomCardPool == null",
            "creationOptions.CardPoolFilter == null",
            "creationOptions.CardPools.Count > 0",
            "!creationOptions.CardPools.All(pool => pool.IsColorless)",
            "ModelDb.AllCharacterCardPools",
            ".Where(pool => !pool.Id.Equals(homePool.Id) && !pool.IsColorless)",
            ".Where(card => rarity == null || card.Rarity == rarity)",
            ".Where(card => type == null || card.Type == type)",
            ".Where(card => card.Type is not CardType.Curse and not CardType.Status and not CardType.Quest)",
            ".Where(card => card.CanBeGeneratedByModifiers)",
            ".Where(card => !excludedIds.Contains(card.Id))",
            ".DistinctBy(card => card.Id)",
            "var excludedIds = cardRewardOptions",
            "for (var slotIndex = 0; slotIndex < cardRewardOptions.Count; slotIndex++)",
            "PreserveUpgradeState(originalCard, replacement)",
            "reward.ModifyCard(replacement, prismaticGem)",
            "RewardResultHints.GetValue(reward, _ => new RewardResultHintState())",
            "excludedIds.Add(replacement.Id)",
            "player.RunState.RemoveCard(originalCard)",
            "RemoveUnpiledReplacements(replacements)",
            "AncientCardHelpers.RemoveUnpiledRunCard(replacement)",
            "RestoreCounterAfterFailedReplacement(prismaticGem, screenState)",
            "GetOffColorRewardPool(player, originalCard.Rarity, originalCard.Type, excludedIds)",
            "GetOffColorRewardPool(player, null, originalCard.Type, excludedIds)",
            "GetOffColorRewardPool(player, originalCard.Rarity, null, excludedIds)",
            "GetOffColorRewardPool(player, null, null, excludedIds)",
            "return player.RunState.CreateCard(replacementCanonical, player)");

        Assert.DoesNotContain("var slotIndex = cardRewardOptions.Count - 1", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ReplaceRightmostRewardSlot", source, StringComparison.Ordinal);
        Assert.DoesNotContain("[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.TryModifyCardRewardOptions))]", source, StringComparison.Ordinal);
    }

    [Fact]
    public void DirectAncientRelicPatchesRespectMeltedRelics()
    {
        var prismatic = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PrismaticGemPatches.cs");
        var fiddle = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "FiddlePatches.cs");

        AssertSourceContains(
            prismatic,
            "FirstOrDefault(relic => !relic.IsMelted)");
        AssertSourceContains(
            fiddle,
            "if (__instance.IsMelted)",
            "__result = count;",
            "__result = true;",
            "player.GetRelic<Fiddle>() is not { IsMelted: false }");
    }

    [Fact]
    public void TurnStartAndAutoPlayAncientsKeepOwnerRoundAndTargetGuards()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var helpers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientCardHelpers.cs");

        AssertSourceContains(
            source,
            "if (side != crossbow.Owner.Creature.Side)",
            "card.Type == CardType.Attack && card.CanBeGeneratedInCombat",
            "AncientCardHelpers.ApplyTemporaryCostReduction(generated, 1)",
            "AncientCardHelpers.ApplyKeywords(generated, CardKeyword.Ethereal, CardKeyword.Exhaust)",
            "await AncientCardHelpers.TryAddGeneratedCardToCombat(generated, PileType.Hand, owner)",
            "AncientCardHelpers.RemoveUnpiledCombatCard(generated, combatState)",
            "if (player != __instance.Owner)",
            "await CardPileCmd.ShuffleIfNecessary(choiceContext, player)",
            "combatState.RoundNumber == 1",
            "cards.FirstOrDefault(card => !card.Keywords.Contains(CardKeyword.Innate))",
            "await CardCmd.Exhaust(choiceContext, topCard)",
            "await PowerCmd.Apply<StrengthPower>",
            "if (combatState.RoundNumber > 3)",
            ".Where(item => VelvetChokerSoftLimitTracker.SuppressCostFor(item.Card, item.Card.CanPlay))",
            ".OrderByDescending(item => VelvetChokerSoftLimitTracker.SuppressCostFor(item.Card, () => AncientCardHelpers.EffectiveCost(item.Card)))",
            ".ThenBy(item => item.Index)",
            "if (card.TargetType is TargetType.AnyEnemy or TargetType.AnyAlly && !card.CanPlayTargeting(target))",
            "await VelvetChokerSoftLimitTracker.SuppressCostFor(card, card.SpendResources)",
            "await CardCmd.AutoPlay(choiceContext, card, target, AutoPlayType.Default, skipXCapture: true)");

        AssertSourceContains(
            helpers,
            "card.EnergyCost.CostsX",
            "card.Owner.PlayerCombatState?.Energy ?? 0",
            "card.HasStarCostX",
            "card.Owner.PlayerCombatState?.Stars ?? 0",
            "TargetType.AnyEnemy => combatState.HittableEnemies.OrderByDescending(creature => creature.CurrentHp).FirstOrDefault()",
            "TargetType.AnyPlayer => owner.Creature",
            "public static async Task<CardPileAddResult?> TryAddGeneratedCardToCombat",
            "CombatManager.Instance.IsOverOrEnding",
            "!CombatManager.Instance.IsInProgress",
            "card.Owner?.Creature.CombatState == null",
            "RemoveUnpiledCombatCard(card)",
            "await CardPileCmd.AddGeneratedCardsToCombat([card], pileType, creator, position)",
            "var result = results.FirstOrDefault()",
            "result.cardAdded == null",
            "|| !result.success)");
    }

    [Fact]
    public void SavedStateKeysAreUniqueSerializableAndScopedToActiveMod()
    {
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var prismaticSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var paelsToothSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var jewelryBoxSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var playerStateSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientPlayerState.cs");
        var urdaSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Urda");
        var morviSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi");
        var lothaSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha");
        var ancientSourceWithoutPlayerStateHelper = Directory
            .GetFiles(RepoPath("EZMicroBalanceCode", "Ancients"), "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.EndsWith("AncientPlayerState.cs", StringComparison.Ordinal) &&
                           !path.EndsWith("AncientSavedStateFields.cs", StringComparison.Ordinal))
            .Select(path => File.ReadAllText(path, Encoding.UTF8));

        var keys = Regex.Matches(savedFields, "\"(?<key>EZMicroBalance[^\"]+)\"")
            .Select(match => match.Groups["key"].Value)
            .ToArray();

        Assert.Equal(14, keys.Length);
        Assert.Equal(keys.Length, keys.Distinct(StringComparer.Ordinal).Count());
        Assert.All(keys, key => Assert.StartsWith("EZMicroBalance", key, StringComparison.Ordinal));
        Assert.DoesNotContain("EzDailyContent", savedFields, StringComparison.Ordinal);

        AssertSourceContains(
            savedFields,
            "SavedSpireField<PrismaticGem, int> PrismaticGemNormalRewardCounter",
            "SavedSpireField<PaelsTooth, int> PaelsToothNonBossCombatCounter",
            "SavedSpireField<CardModel, bool> JewelryBoxNonInnateApotheosis",
            "SavedSpireField<Player, string> UrdaStateKey",
            "SavedSpireField<CardModel, string> UrdaDeckStateKey",
            "SavedSpireField<CardModel, bool> UrdaTrialPlantCard",
            "SavedSpireField<Player, string> MorviStateKey",
            "SavedSpireField<CardModel, string> MorviDeckStateKey",
            "SavedSpireField<CardModel, bool> MorviBorrowedAncientCard",
            "SavedSpireField<CardModel, bool> MorviOpenBookSealedCard",
            "SavedSpireField<Player, string> LothaStateKey",
            "SavedSpireField<CardModel, string> LothaDeckStateKey",
            "SavedSpireField<CardModel, bool> LothaMirrorRebuttalCard",
            "SavedSpireField<Player, string> AncientInitialOptionRerollStateKey",
            "\"EZMicroBalanceNormalRewardCounter\"",
            "\"EZMicroBalanceNonBossCombatCounter\"",
            "\"EZMicroBalanceJewelryBoxNonInnateApotheosis\"",
            "\"EZMicroBalanceUrdaStateKey\"",
            "\"EZMicroBalanceUrdaDeckStateKey\"",
            "\"EZMicroBalanceUrdaTrialPlantCard\"",
            "\"EZMicroBalanceMorviStateKey\"",
            "\"EZMicroBalanceMorviDeckStateKey\"",
            "\"EZMicroBalanceMorviBorrowedAncientCard\"",
            "\"EZMicroBalanceMorviOpenBookSealedCard\"",
            "\"EZMicroBalanceLothaStateKey\"",
            "\"EZMicroBalanceLothaDeckStateKey\"",
            "\"EZMicroBalanceLothaMirrorRebuttalCard\"",
            "\"EZMicroBalanceAncientInitialOptionRerollStateKey\"");

        AssertSourceContains(
            playerStateSource,
            "public static string Get(",
            "SavedSpireField<Player, string> runtimeField",
            "SavedSpireField<CardModel, string> deckField",
            "runtimeField[player] = deckState",
            "player.Deck.Cards",
            ".Where(card => card.Owner == player && !card.HasBeenRemovedFromState)",
            "deckField[card] = state",
            "!card.HasBeenRemovedFromState");

        Assert.Contains("AncientSavedStateFields.PrismaticGemNormalRewardCounter[prismaticGem]", prismaticSource, StringComparison.Ordinal);
        Assert.Contains("AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth]", paelsToothSource, StringComparison.Ordinal);
        Assert.Contains("AncientSavedStateFields.JewelryBoxNonInnateApotheosis[card]", jewelryBoxSource, StringComparison.Ordinal);
        AssertSourceContains(
            urdaSource,
            "AncientPlayerState.Get(",
            "AncientPlayerState.Set(",
            "AncientPlayerState.SyncDeck(",
            "AncientSavedStateFields.UrdaStateKey",
            "AncientSavedStateFields.UrdaDeckStateKey");
        AssertSourceContains(
            morviSource,
            "AncientPlayerState.Get(",
            "AncientPlayerState.Set(",
            "AncientPlayerState.SyncDeck(",
            "AncientSavedStateFields.MorviStateKey",
            "AncientSavedStateFields.MorviDeckStateKey");
        AssertSourceContains(
            lothaSource,
            "AncientPlayerState.Get(",
            "AncientPlayerState.Set(",
            "AncientPlayerState.SyncDeck(",
            "AncientSavedStateFields.LothaStateKey",
            "AncientSavedStateFields.LothaDeckStateKey");
        Assert.DoesNotMatch(
            @"\b(?:UrdaStateKey|UrdaDeckStateKey|MorviStateKey|MorviDeckStateKey|LothaStateKey|LothaDeckStateKey)\s*\[",
            string.Join(Environment.NewLine, ancientSourceWithoutPlayerStateHelper));
    }

    [Fact]
    public void ManualAncientRuntimeEvidenceRemainsExplicitlyPending()
    {
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");
        var ancientMatrix = SliceBetween(
            manualMatrix,
            "## Ancient Reward Matrix",
            "## Simplified Chinese Localization Spot Checks");

        Assert.Contains("- [x] Every implemented Ancient reward change has a manual checklist row.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("- [ ] Every implemented Ancient reward change has a completed manual runtime result.", releaseChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Every implemented Ancient reward change has a completed manual runtime result.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("Manual feature results are pending", releaseChecklist, StringComparison.Ordinal);

        Assert.Contains("| Prismatic Gem |", ancientMatrix, StringComparison.Ordinal);
        Assert.Contains("| Meat Cleaver |", ancientMatrix, StringComparison.Ordinal);
        Assert.Contains("| Blood-Soaked Rose / Enthralled |", ancientMatrix, StringComparison.Ordinal);
        Assert.Contains("Pending", ancientMatrix, StringComparison.Ordinal);
        Assert.DoesNotContain("| Pass", ancientMatrix, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Result: pass", ancientMatrix, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("manually verified", ancientMatrix, StringComparison.OrdinalIgnoreCase);
    }

}
