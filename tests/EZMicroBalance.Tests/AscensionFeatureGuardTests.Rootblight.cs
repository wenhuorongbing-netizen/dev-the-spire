using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AscensionFeatureGuardTests
{
    [Fact]
    public void RootStarterUsesSavedPlayerMarkerAndCommandDeckMutation()
    {
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionSavedStateFields.cs");
        var service = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "RootRunHook.cs");

        AssertSourceContains(
            savedFields,
            "SavedSpireField<Player, bool> RootBeginsApplied",
            "EZMicroBalanceAscensionRootBeginsApplied",
            "SavedSpireField<Player, string> RootblightPendingCombatDowngrades",
            "EZMicroBalanceAscensionRootblightPendingCombatDowngrades");

        AssertSourceContains(
            service,
            "AscensionSavedStateFields.RootblightLevel[player]",
            "AscensionSavedStateFields.RootBeginsApplied[player] = true;",
            "player.RunState.CreateCard<Root>(player)",
            "player.RunState.CreateCard<DeepRoot>(player)",
            "player.RunState.CreateCard<RootblightIII>(player)",
            "private static async Task<bool> AddRootblightCard(Player player, int level, bool hasSplit = false, bool preferOverlayNotice = false)",
            "MaxRootblightCards = 4",
            "TrimRootblightDeckToCap(player",
            "CardPileCmd.Add(rootblightCard, PileType.Deck, CardPilePosition.Bottom, clonedBy: null, skipVisuals: true)",
            "if (!addResult.success)",
            "ShowRootblightAdded(player, preferOverlayNotice)",
            "LocalContext.IsMe(player)",
            "new LocString(\"ascension\", \"ROOTBLIGHT_ADDED\")",
            "preferOverlayNotice && TryShowRunOverlayNotice(line)",
            "AddRootblightCard(player, cardToAdd.Level, cardToAdd.HasSplit, preferOverlayNotice: true)",
            "TryShowTopLevelRunNotice(line) || TryShowGlobalRunNotice(line)",
            "NGame.Instance",
            "bubble.MouseFilter = Control.MouseFilterEnum.Ignore",
            "bubble.ZIndex = 4096",
            "player.Creature.GetVfxContainer()",
            "TryShowEventRoomNotice(line)",
            "NEventRoom.Instance?.VfxContainer",
            "NThoughtBubbleVfx.Create(line.GetFormattedText(), DialogueSide.Left, RootblightNoticeSeconds)",
            "TryFindRootblightDeckVersion(player, card)",
            "had no unique master-deck card",
            "matchingLevel.Count == 1",
            "matchingSplitState.Count == 1 ? matchingSplitState[0] : null",
            "QueuePendingCombatDowngrade(player, downgradedLevel, splitState)",
            "ReadPendingCombatDowngrades(player)",
            "ClearPendingCombatDowngrades(player)",
            "ignored Rootblight III split once",
            "ignored Rootblight III already split once; no Rootblight IV",
            "ThenBy(entry => entry.Index)",
            "await CardPileCmd.RemoveFromDeck(card, showPreview: false)");
        Assert.DoesNotContain("VisitedMapCoords", service, StringComparison.Ordinal);

        AssertSourceContains(
            runHook,
            "public RootRunHook()",
            "AfterActEntered()",
            "BeforeRoomEntered(AbstractRoom room)",
            "HandleBeforeRoomEntered(AbstractRoom room)",
            "RunManager.Instance.DebugOnlyGetState()",
            "AscensionFeatureGate.IsRootblightEnabled(runState)");
        Assert.DoesNotContain("player.RunState.CurrentMapCoord.HasValue", service, StringComparison.Ordinal);
        Assert.DoesNotContain("player.RunState.MapPointHistory.Any", service, StringComparison.Ordinal);
        Assert.DoesNotContain("player.RunState.ActFloor > 0", service, StringComparison.Ordinal);

        var initializer = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionInitializer.cs");
        Assert.Contains("ModelDb.GetById<RootRunHook>(ModelDb.GetId<RootRunHook>())", initializer, StringComparison.Ordinal);
        Assert.DoesNotContain("new RootRunHook(", initializer, StringComparison.Ordinal);
    }

    [Fact]
    public void RootStarterDoesNotMistakeFirstEnteredRoomForAppliedRoot()
    {
        var serviceState = ReadRepoText("EZMicroBalanceCode", "Ascension", "Rewards", "RootDeckService.State.cs");
        var serviceLifecycle = ReadRepoText("EZMicroBalanceCode", "Ascension", "Rewards", "RootDeckService.Lifecycle.cs");
        var serviceCombatLifecycle = ReadRepoText("EZMicroBalanceCode", "Ascension", "Rewards", "RootDeckService.CombatLifecycle.cs");
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "RootRunHook.cs");
        var combatHookLifecycle = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.Lifecycle.cs");

        AssertSourceContains(
            runHook,
            "public override Task BeforeRoomEntered(AbstractRoom room)",
            "HandleBeforeRoomEntered(room)",
            "await RootDeckService.EnsureStartingRoot(runState)");
        AssertSourceContains(
            serviceState,
            "AscensionSavedStateFields.RootBeginsApplied[player]",
            "FindRootFamilyCards(player).Count > 0");
        Assert.DoesNotContain("CurrentMapCoord", serviceState, StringComparison.Ordinal);
        Assert.DoesNotContain("MapPointHistory", serviceState, StringComparison.Ordinal);
        Assert.DoesNotContain("ActFloor", serviceState, StringComparison.Ordinal);

        var firstApplyBlock = SliceBetween(
            serviceLifecycle,
            "if (!HasRootBeginsApplied(player))",
            "MainFile.Logger.Info(");
        AssertBefore(firstApplyBlock, "addedStartingRoot = await AddRootblightCard(player, 1);", "MarkRootBeginsApplied(player);");
        Assert.Contains("the next room/act hook will retry", firstApplyBlock, StringComparison.Ordinal);

        var blightSproutAddBlock = SliceFrom(
            serviceLifecycle,
            "public static async Task AddRootblightI(Player player, string source)");
        AssertBefore(blightSproutAddBlock, "if (!await AddRootblightCard(player, 1, preferOverlayNotice: true))", "MarkRootBeginsApplied(player);");
        Assert.Contains("hadRootblightBeforeAdd || FindRootFamilyCards(player).Count > 0", blightSproutAddBlock, StringComparison.Ordinal);
        Assert.Equal(2, CountOccurrences(blightSproutAddBlock, "MarkRootBeginsApplied(player);"));
        AssertSourceContains(
            serviceCombatLifecycle,
            "public static void MarkCombatStartRootblight(Player player)",
            "public static async Task ResolveCombatEndRootblight(Player player)",
            "WasPresentAtCombatStart = false",
            "ClearPendingCombatDowngrades(player)");

        var combatStartBlock = SliceBetween(
            combatHookLifecycle,
            "public override async Task BeforeCombatStart()",
            "if (!IsGameplayEnabledForCurrentRoom(state))");
        AssertBefore(
            combatStartBlock,
            "await RootDeckService.EnsureStartingRoot(state.RunState);",
            "RootDeckService.MarkCombatStartRootblight(player);");
        Assert.Contains("last safe repair point before combat-end growth bookkeeping", combatStartBlock, StringComparison.Ordinal);
    }

    [Fact]
    public void RootBudSeedingUsesExistingPileScanAndSavedPerCardFlags()
    {
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionSavedStateFields.cs");
        var combatHookMain = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.cs");
        var combatHookCardFlow = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.CardFlow.cs");
        var combatHookCombatEnd = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.CombatEnd.cs");
        var combatHookCombatEvents = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.CombatEvents.cs");
        var combatHookHelpers = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.Helpers.cs");
        var combatHookLifecycle = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.Lifecycle.cs");
        var combatHookRoomRules = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.RoomRules.cs");
        var combatHook = string.Join(
            Environment.NewLine,
            combatHookMain,
            combatHookCardFlow,
            combatHookCombatEnd,
            combatHookCombatEvents,
            combatHookHelpers,
            combatHookLifecycle,
            combatHookRoomRules);
        var rootBudCard = ReadRepoText("EZMicroBalanceCode", "Ascension", "Cards", "RootBudCard.cs");

        AssertSourceContains(
            savedFields,
            "SavedSpireField<RootBud, bool> RootBudEnteredHand",
            "SavedSpireField<RootBud, bool> RootBudPlayed",
            "SavedSpireField<RootBud, bool> RootBudSprouted",
            "SavedSpireField<RootBud, int> RootBudSproutRound");

        AssertSourceContains(
            combatHook,
            "public RootBudCombatHook()",
            "public override bool ShouldReceiveCombatHooks => true",
            "CurrentCombatState()",
            "CombatManager.Instance.DebugOnlyGetState()",
            "var existingBuds = FindRootBudsInCombat(player)",
            "GetRootBudCountForCurrentRoom(state)",
            "NormalizeExistingRootBudRounds(state, existingBuds)",
            "for (var i = 0; i < existingBuds.Count; i++)",
            "existingBuds[i].SproutRound = targetRounds[i]",
            "GetRootBudSproutRoundForCurrentRoom(state, i)",
            "RootBud.BossSecondSproutRound",
            "player.Piles",
            "SelectMany(pile => pile.Cards)",
            "await CardPileCmd.AddGeneratedCardToCombat(bud, PileType.Discard, player, CardPilePosition.Bottom)",
            "await CardPileCmd.Add(bud, PileType.Draw, CardPilePosition.Top)",
            "AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)",
            "public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)",
            "await AscensionCombatModifierService.BeforeTurnEnd(state, GetTracker(state), side, participants)",
            "MarkEnteredHand(state, bud)",
            "Trackers.Remove(state)");

        AssertSourceContains(
            combatHookMain,
            "internal sealed partial class RootBudCombatHook : AbstractModel",
            "public override bool ShouldReceiveCombatHooks => true");
        AssertSourceContains(
            combatHookCardFlow,
            "internal sealed partial class RootBudCombatHook",
            "public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)",
            "public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)");
        AssertSourceContains(
            combatHookCombatEvents,
            "internal sealed partial class RootBudCombatHook",
            "public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)",
            "public override async Task BeforeSideTurnStart",
            "IReadOnlyList<Creature> participants",
            "ICombatState combatState",
            "public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)");
        AssertSourceContains(
            combatHookLifecycle,
            "internal sealed partial class RootBudCombatHook",
            "public override async Task BeforeCombatStart()");
        Assert.DoesNotContain("public override async Task AfterCombatEnd", combatHookLifecycle, StringComparison.Ordinal);
        AssertSourceContains(
            combatHookCombatEnd,
            "internal sealed partial class RootBudCombatHook",
            "public override async Task AfterCombatEnd(CombatRoom room)");

        AssertSourceContains(
            combatHookHelpers,
            "internal sealed partial class RootBudCombatHook",
            "private static CombatState? CurrentCombatState()",
            "private static IReadOnlyList<RootBud> FindRootBudsInCombat(Player player)",
            "private static async Task ResolveRootblightForCombatEnd(CombatState state)");
        Assert.DoesNotContain("targetRounds.Contains(bud.SproutRound)", combatHookHelpers, StringComparison.Ordinal);
        Assert.DoesNotContain("usedRounds.Add(bud.SproutRound)", combatHookHelpers, StringComparison.Ordinal);
        Assert.DoesNotContain("public override", combatHookHelpers, StringComparison.Ordinal);
        Assert.DoesNotContain(": AbstractModel", combatHookHelpers, StringComparison.Ordinal);
        AssertSourceContains(
            combatHookRoomRules,
            "internal sealed partial class RootBudCombatHook",
            "private static bool IsGameplayEnabledForCurrentRoom(CombatState state)",
            "RoomType.Elite when IsEligibleEliteSproutFight(state)",
            "private static int GetRootBudCountForCurrentRoom(CombatState state)",
            "private static void NormalizeExistingRootBudRounds(CombatState state, IReadOnlyList<RootBud> existingBuds)",
            "private static int GetRootBudSproutRoundForCurrentRoom(CombatState state, int budIndex)");
        Assert.DoesNotContain("public override", combatHookRoomRules, StringComparison.Ordinal);
        Assert.DoesNotContain(": AbstractModel", combatHookRoomRules, StringComparison.Ordinal);

        var beforeCombatStart = SliceBetween(
            combatHookLifecycle,
            "public override async Task BeforeCombatStart()",
            "AscensionDiagnostics.LogCombatState(state, \"before combat start after root bud seed\");");
        AssertSourceContains(
            beforeCombatStart,
            "var state = CurrentCombatState();",
            "var tracker = GetTracker(state);",
            "var targetBudCount = GetRootBudCountForCurrentRoom(state);",
            "NormalizeExistingRootBudRounds(state, existingBuds)",
            "await CardPileCmd.AddGeneratedCardToCombat(bud, PileType.Discard, player, CardPilePosition.Bottom)");

        var beforeHandDraw = SliceBetween(
            combatHookCardFlow,
            "public override async Task BeforeHandDraw",
            "public override async Task AfterCardChangedPiles");
        AssertSourceContains(
            beforeHandDraw,
            "combatState is not CombatState state",
            "await SproutDueBudsBeforeHandDraw(state, player)");

        var beforeSideTurnStart = SliceBetween(
            combatHookCombatEvents,
            "public override async Task BeforeSideTurnStart",
            "public override async Task AfterSideTurnEnd");
        AssertSourceContains(
            beforeSideTurnStart,
            "combatState is not CombatState state",
            "await AscensionCombatModifierService.BeforeSideTurnStart(state, GetTracker(state), side)");
        Assert.DoesNotContain("CurrentCombatState();", beforeSideTurnStart, StringComparison.Ordinal);

        var afterCardDrawn = SliceBetween(
            combatHookCardFlow,
            "public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)",
            "public override async Task AfterCardPlayed");
        AssertSourceContains(
            afterCardDrawn,
            "MarkEnteredHand(state, bud)",
            "await AscensionCombatModifierService.AfterCardEnteredHand(state, tracker, card)");

        var afterCombatEnd = SliceFrom(
            combatHookCombatEnd,
            "public override async Task AfterCombatEnd(CombatRoom room)");
        AssertSourceContains(
            afterCombatEnd,
            "await ResolveRootblightForCombatEnd(state)",
            "await RootDeckService.AddRootblightI(bud.Owner, \"Blight Sprout\")",
            ".Where(bud => !bud.PlantedInSeedbed)",
            "foreach (var bud in FindKnownBuds(state).Where(bud => bud.PlantedInSeedbed))",
            "bud.PlantedInSeedbed = false;",
            "Trackers.Remove(state)");

        AssertSourceContains(
            rootBudCard,
            "get => AscensionSavedStateFields.RootBudEnteredHand[this]",
            "get => AscensionSavedStateFields.RootBudPlayed[this]",
            "get => AscensionSavedStateFields.RootBudSprouted[this]",
            "get => Math.Max(DefaultSproutRound, AscensionSavedStateFields.RootBudSproutRound[this])",
            "ExhaustOnNextPlay = true");

        var initializer = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionInitializer.cs");
        Assert.Contains("ModelDb.GetById<RootBudCombatHook>(ModelDb.GetId<RootBudCombatHook>())", initializer, StringComparison.Ordinal);
        Assert.DoesNotContain("new RootBudCombatHook(", initializer, StringComparison.Ordinal);
    }

    [Fact]
    public void RootBudGameplayGateProtectsDiagnosticsActOneElitesAndPlayerDeath()
    {
        var combatHook = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.CardFlow.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.CombatEnd.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.CombatEvents.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.Helpers.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.Lifecycle.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.RoomRules.cs"));
        var apiResearch = ReadRepoText("docs", "features", "ascension-11-20", "api-research.md");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");

        AssertSourceContains(
            combatHook,
            "IsGameplayEnabledForCurrentRoom(state)",
            "RoomType.Elite when IsEligibleEliteSproutFight(state)",
            "return state.RunState.CurrentActIndex is 1 or 2;",
            "currentRow >= 3",
            "after combat end without Blight Sprout growth",
            "AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)",
            "GetTracker(state).DiedPlayers.Add(creature.Player)",
            "!tracker.DiedPlayers.Contains(bud.Owner)");

        Assert.Contains("Act 1 bosses and Act 1 elites are excluded from the current Blight Sprout slice.", apiResearch, StringComparison.Ordinal);
        Assert.Contains("Diagnostics-only mode must not raise Rootblight from restored Blight Sprout cards.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Knockout/revive should not raise Rootblight from that combat's Blight Sprout.", manualChecklist, StringComparison.Ordinal);
    }

    [Fact]
    public void RootFamilyCardsAreLocalizedAndGuardedAgainstKnownRandomGenerationPaths()
    {
        var rootCards = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Cards", "RootCards.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Cards", "RootBudCard.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Cards", "RootFamilyCard.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Cards", "RootPortraitPaths.cs"));
        var apiResearch = ReadRepoText("docs", "features", "ascension-11-20", "api-research.md");
        var englishCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var simplifiedChineseCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");

        Assert.Equal("Rootblight I", englishCards["EZMB_ROOT.title"]);
        Assert.Equal("Rootblight II", englishCards["EZMB_DEEP_ROOT.title"]);
        Assert.Equal("Rootblight III", englishCards["EZMB_ROOTBLIGHT_III.title"]);
        Assert.Equal("Blight Sprout", englishCards["EZMB_ROOT_BUD.title"]);

        foreach (var key in new[] { "EZMB_ROOT.description", "EZMB_DEEP_ROOT.description", "EZMB_ROOTBLIGHT_III.description", "EZMB_ROOT_BUD.description" })
        {
            Assert.DoesNotContain("Play: Exhaust", englishCards[key], StringComparison.Ordinal);
            Assert.DoesNotContain("If not played or removed this combat", englishCards[key], StringComparison.Ordinal);
            Assert.DoesNotContain("\u6253\u51fa\uff1a\u6d88\u8017", simplifiedChineseCards[key], StringComparison.Ordinal);
            Assert.DoesNotContain("\u672a\u6253\u51fa\u6216\u79fb\u9664", simplifiedChineseCards[key], StringComparison.Ordinal);
        }

        Assert.Contains("still in your deck after combat", englishCards["EZMB_ROOT.description"], StringComparison.Ordinal);
        Assert.Contains("still in your deck after combat", englishCards["EZMB_DEEP_ROOT.description"], StringComparison.Ordinal);
        Assert.Contains("still in your deck after combat", englishCards["EZMB_ROOTBLIGHT_III.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Rootblight II[/gold]", englishCards["EZMB_ROOT.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Rootblight I[/gold]", englishCards["EZMB_DEEP_ROOT.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Rootblight III[/gold]", englishCards["EZMB_DEEP_ROOT.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Rootblight II[/gold]", englishCards["EZMB_ROOTBLIGHT_III.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Rootblight III[/gold]", englishCards["EZMB_ROOTBLIGHT_III.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Draw Pile[/gold]", englishCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Rootblight I[/gold]", englishCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);

        Assert.Contains("\u82e5\u6218\u6597\u7ed3\u675f\u65f6\u672c\u724c\u4ecd\u5728\u4f60\u7684\u4e3b\u724c\u7ec4\u4e2d", simplifiedChineseCards["EZMB_ROOT.description"], StringComparison.Ordinal);
        Assert.Contains("\u82e5\u6218\u6597\u7ed3\u675f\u65f6\u672c\u724c\u4ecd\u5728\u4f60\u7684\u4e3b\u724c\u7ec4\u4e2d", simplifiedChineseCards["EZMB_DEEP_ROOT.description"], StringComparison.Ordinal);
        Assert.Contains("\u82e5\u6218\u6597\u7ed3\u675f\u65f6\u672c\u724c\u4ecd\u5728\u4f60\u7684\u4e3b\u724c\u7ec4\u4e2d", simplifiedChineseCards["EZMB_ROOTBLIGHT_III.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]\u6839\u8680 II[/gold]", simplifiedChineseCards["EZMB_ROOT.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]\u6839\u8680 I[/gold]", simplifiedChineseCards["EZMB_DEEP_ROOT.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]\u6839\u8680 III[/gold]", simplifiedChineseCards["EZMB_DEEP_ROOT.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]\u6839\u8680 II[/gold]", simplifiedChineseCards["EZMB_ROOTBLIGHT_III.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]\u6839\u8680 III[/gold]", simplifiedChineseCards["EZMB_ROOTBLIGHT_III.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]\u62bd\u724c\u5806[/gold]", simplifiedChineseCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]\u6839\u8680 I[/gold]", simplifiedChineseCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);

        foreach (var key in new[] { "EZMB_ROOT.title", "EZMB_ROOT.description", "EZMB_DEEP_ROOT.title", "EZMB_DEEP_ROOT.description", "EZMB_ROOTBLIGHT_III.title", "EZMB_ROOTBLIGHT_III.description", "EZMB_ROOT_BUD.title", "EZMB_ROOT_BUD.description" })
        {
            Assert.True(simplifiedChineseCards.ContainsKey(key), $"Missing zhs card key: {key}");
        }

        Assert.Equal(4, CountOccurrences(rootCards, "[Pool(typeof(CurseCardPool))]"));
        AssertSourceContains(
            rootCards,
            "using Godot;",
            "using MegaCrit.Sts2.Core.HoverTips;",
            "internal static class RootPortraitPaths",
            "public sealed class RootBud : CustomCardModel",
            "public abstract class RootFamilyCard : CustomCardModel",
            "ResourceLoader.Exists(candidate) ? candidate : fallback",
            "rootblight_i",
            "rootblight_ii",
            "rootblight_iii",
            "blight_sprout.png",
            "AncientCardHelpers.TemporaryHoverTip()",
            "HoverTipFactory.FromCard<Root>()",
            "1 => [HoverTipFactory.FromCard<DeepRoot>()]",
            "2 => [HoverTipFactory.FromCard<Root>(), HoverTipFactory.FromCard<RootblightIII>()]",
            "_ => [HoverTipFactory.FromCard<Root>(), HoverTipFactory.FromCard<DeepRoot>()]");
        Assert.Equal(2, CountOccurrences(rootCards, "public override bool CanBeGeneratedInCombat => false;"));
        Assert.Equal(2, CountOccurrences(rootCards, "public override bool CanBeGeneratedByModifiers => false;"));
        Assert.Contains("CurseCardPool", apiResearch, StringComparison.Ordinal);
        Assert.Contains("HoverTipFactory.FromCard<Soul>()", apiResearch, StringComparison.Ordinal);
        Assert.Equal("[gold]Rootblight[/gold] added.", JsonStringMap("EZMicroBalance", "localization", "eng", "ascension.json")["ROOTBLIGHT_ADDED"]);
        Assert.Equal("[gold]\u6839\u8680[/gold]\u5df2\u52a0\u5165\u3002", JsonStringMap("EZMicroBalance", "localization", "zhs", "ascension.json")["ROOTBLIGHT_ADDED"]);
        Assert.Contains("Runtime registration and random transform/reward exclusion pending", apiResearch, StringComparison.Ordinal);
    }
}
