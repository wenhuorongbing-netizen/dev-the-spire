using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AscensionV2MilestoneGuardTests
{
    [Fact]
    public void Milestone1RootblightAndBlightSproutUseV2NamingStateAndHooks()
    {
        var cardsSource = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Cards", "RootCards.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Cards", "RootBudCard.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ascension", "Cards", "RootFamilyCard.cs"));
        var deckService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var combatHook = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Combat");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionSavedStateFields.cs");
        var englishCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var zhsCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");

        Assert.Equal("Rootblight I", englishCards["EZMB_ROOT.title"]);
        Assert.Equal("Rootblight II", englishCards["EZMB_DEEP_ROOT.title"]);
        Assert.Equal("Rootblight III", englishCards["EZMB_ROOTBLIGHT_III.title"]);
        Assert.Equal("Blight Sprout", englishCards["EZMB_ROOT_BUD.title"]);
        Assert.Contains("If seen and not played, add a [gold]Rootblight I[/gold] after combat.", englishCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);
        Assert.Contains("if never drawn, it withers away.", englishCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);
        Assert.Contains("stays as [gold]Rootblight III[/gold]", englishCards["EZMB_ROOTBLIGHT_III.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("your deck has no Rootblight", englishCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);

        foreach (var key in new[] { "EZMB_ROOT.title", "EZMB_DEEP_ROOT.title", "EZMB_ROOTBLIGHT_III.title", "EZMB_ROOT_BUD.title" })
        {
            Assert.True(zhsCards.TryGetValue(key, out var value), $"Missing zhs card localization: {key}");
            Assert.False(string.IsNullOrWhiteSpace(value), $"Empty zhs card localization: {key}");
        }

        AssertSourceContains(
            cardsSource,
            "public sealed class RootblightIII",
            "rootblightLevel: 3",
            "public const int DefaultSproutRound = 3",
            "public const int BossSecondSproutRound = 4",
            "AscensionSavedStateFields.RootBudSproutRound[this]",
            "AscensionSavedStateFields.RootblightWasPresentAtCombatStart[this]",
            "AscensionSavedStateFields.RootblightHasSplit[this]",
            "public override IEnumerable<CardKeyword> CanonicalKeywords => ExhaustKeyword",
            "ExhaustOnNextPlay = true");

        AssertSourceContains(
            savedFields,
            "SavedSpireField<Player, int> RootblightLevel",
            "SavedSpireField<Player, string> RootblightPendingCombatDowngrades",
            "EZMicroBalanceAscensionRootblightPendingCombatDowngrades",
            "SavedSpireField<RootFamilyCard, bool> RootblightWasPresentAtCombatStart",
            "SavedSpireField<RootFamilyCard, bool> RootblightHasSplit",
            "SavedSpireField<RootBud, bool> RootBudEnteredHand",
            "SavedSpireField<RootBud, bool> RootBudPlayed",
            "SavedSpireField<RootBud, bool> RootBudSprouted",
            "SavedSpireField<RootBud, int> RootBudSproutRound");

        AssertSourceContains(
            deckService,
            "MaxRootblightLevel = 3",
            "MaxRootblightCards = 4",
            "TrimRootblightDeckToCap(player",
            "FindRootFamilyCards(player)",
            "MarkCombatStartRootblight",
            "TryFindRootblightDeckVersion(player, card)",
            "had no unique master-deck card",
            "matchingLevel.Count == 1",
            "matchingSplitState.Count == 1 ? matchingSplitState[0] : null",
            "QueuePendingCombatDowngrade(player, downgradedLevel, splitState)",
            "ReadPendingCombatDowngrades(player)",
            "WritePendingCombatDowngrades(player, pending)",
            "ClearPendingCombatDowngrades(player)",
            "card.RootblightLevel - 1",
            "new RootblightCardToAdd(level, parts[1] == \"1\")",
            "rootFamilyCard.HasSplit = hasSplit",
            "if (!card.HasSplit)",
            "card.HasSplit = true;",
            "ignored Rootblight III split once",
            "ignored Rootblight III already split once; no Rootblight IV",
            "await AddRootblightCard(player, 1, preferOverlayNotice: true)",
            "ThenBy(entry => entry.Index)",
            "ShowRootSystemFull(player)",
            "RemoveHighestRootblight",
            "await CardPileCmd.RemoveFromDeck(card, showPreview: false)",
            "CreateRootblightCard(player, level)");

        AssertSourceContains(
            combatHook,
            "await CardPileCmd.AddGeneratedCardToCombat(bud, PileType.Discard, player, CardPilePosition.Bottom)",
            "await CardPileCmd.Add(bud, PileType.Draw, CardPilePosition.Top)",
            "SproutDueBudsBeforeHandDraw(state, player)",
            "AfterCardChangedPiles(CardModel card, PileType oldPileType",
            "AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)",
            "AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)",
            "await AscensionCombatModifierService.AfterCardEnteredHand(state, tracker, card)",
            "await AscensionCombatModifierService.AfterCardPlayed(state, tracker, cardPlay)",
            "FindKnownBuds(state)",
            "bud.HasEnteredHand && !bud.WasPlayed",
            "await RootDeckService.AddRootblightI(bud.Owner, \"Blight Sprout\")",
            "!tracker.DiedPlayers.Contains(bud.Owner)",
            "GetRootBudCountForCurrentRoom(state)",
            "GetRootBudSproutRoundForCurrentRoom(state, i)",
            "RootBud.BossSecondSproutRound",
            "RoomType.Boss when IsActTwoOrThree(state)",
            "RoomType.Elite when IsEligibleEliteSproutFight(state)");
    }

    [Fact]
    public void RootblightAndBlightSproutV22StateMachineIsSourceGuarded()
    {
        var deckService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var combatHook = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Combat");
        var englishCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var zhsCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");

        AssertSourceContains(
            deckService,
            "MaxRootblightCards = 4",
            "card.WasPresentAtCombatStart = false;",
            "if (!card.HasSplit)",
            "card.HasSplit = true;",
            "ignored Rootblight III split once",
            "ignored Rootblight III already split once; no Rootblight IV",
            "RootblightPendingCombatDowngrades[player]",
            "QueuePendingCombatDowngrade(player, downgradedLevel, splitState)",
            "ReadPendingCombatDowngrades(player)",
            "await TrimRootblightDeckToCap(player, \"pre-add cap check\")",
            "OrderByDescending(entry => entry.Card.RootblightLevel)",
            "ThenBy(entry => entry.Index)",
            "kept {MaxRootblightCards} highest/oldest Rootblight card(s)",
            "Rootblight removed through a deck-removal API",
            "remaining Rootblight cards are preserved");

        AssertSourceContains(
            combatHook,
            "return state.RunState.CurrentRoom?.RoomType == RoomType.Boss",
            "? 2",
            ": 1",
            "NormalizeExistingRootBudRounds(state, existingBuds)",
            "for (var i = 0; i < existingBuds.Count; i++)",
            "existingBuds[i].SproutRound = targetRounds[i]",
            "RootBud.BossSecondSproutRound",
            "RoomType.Boss when IsActTwoOrThree(state)",
            "RoomType.Elite when IsEligibleEliteSproutFight(state)",
            "return state.RunState.CurrentActIndex is 1 or 2;",
            "currentRow >= 3",
            "bud.HasEnteredHand && !bud.WasPlayed",
            "await RootDeckService.AddRootblightI(bud.Owner, \"Blight Sprout\")");

        Assert.Contains("first time this occurs", englishCards["EZMB_ROOTBLIGHT_III.description"], StringComparison.Ordinal);
        Assert.Contains("if never drawn, it withers away.", englishCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);
        Assert.Contains("\u9996\u6B21\u53D1\u751F\u8BE5\u6076\u5316", zhsCards["EZMB_ROOTBLIGHT_III.description"], StringComparison.Ordinal);
        Assert.Contains("\u82E5\u672C\u573A\u4ECE\u672A\u62BD\u5230\uFF0C\u5219\u4F1A\u67AF\u840E\u6D88\u901D", zhsCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);

        Assert.Contains("Rootblight IV never appears.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("If the four-card cap blocks a Rootblight III split, the failed add does not consume that card's split marker", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Rootblight cards added during combat-end resolution do not grow again until the next combat.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("If Blight Sprout enters hand and is discarded or exhausted by a non-play effect, it still adds Rootblight I after combat.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Act 2 elites in the first 3 route rows do not add Blight Sprout.", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("MaxRootblightCards = 1", deckService, StringComparison.Ordinal);
        Assert.DoesNotContain("NormalizeRootblightDeck", deckService, StringComparison.Ordinal);
        Assert.DoesNotContain("downgradedLevel == MaxRootblightLevel && splitState", deckService, StringComparison.Ordinal);
        Assert.DoesNotContain("targetRounds.Contains(bud.SproutRound)", combatHook, StringComparison.Ordinal);
        Assert.DoesNotContain("your deck has no Rootblight", englishCards["EZMB_ROOT_BUD.description"], StringComparison.Ordinal);
    }
}
