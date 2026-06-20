using MegaCrit.Sts2.Core.Entities.Cards;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class RootDeckService
{
    private static readonly ConditionalWeakTable<CardModel, InternalSyncMarker> InternalSyncRemovals = new();

    private static RootFamilyCard? TryFindRootblightDeckVersion(Player player, RootFamilyCard card)
    {
        if (card.DeckVersion is RootFamilyCard deckVersion)
        {
            return deckVersion;
        }

        var matchingLevel = EnumerateRootFamilyCards(player)
            .Where(entry => entry.Card.RootblightLevel == card.RootblightLevel)
            .Select(entry => entry.Card)
            .ToList();
        if (matchingLevel.Count == 1)
        {
            return matchingLevel[0];
        }

        var matchingSplitState = matchingLevel
            .Where(deckCard => deckCard.HasSplit == card.HasSplit)
            .ToList();
        return matchingSplitState.Count == 1 ? matchingSplitState[0] : null;
    }

    private static async Task ReplaceRootblightCard(Player player, RootFamilyCard card, int nextLevel, bool hasSplit)
    {
        InternalSyncRemovals.GetValue(card, _ => new InternalSyncMarker());
        await CardPileCmd.RemoveFromDeck(card, showPreview: false);
        await AddRootblightCard(player, nextLevel, hasSplit, preferOverlayNotice: true);
    }

    private static async Task<bool> AddRootblightCard(Player player, int level, bool hasSplit = false, bool preferOverlayNotice = false)
    {
        await TrimRootblightDeckToCap(player, "pre-add cap check");
        if (FindRootFamilyCards(player).Count >= MaxRootblightCards)
        {
            ReleaseEvidenceLog.Log(
                "Rootblight",
                "deck_cap_enforced",
                player,
                new Dictionary<string, object?>
                {
                    ["level"] = level,
                    ["cap"] = MaxRootblightCards
                });
            return false;
        }

        var rootblightCard = CreateRootblightCard(player, level);
        if (rootblightCard is RootFamilyCard rootFamilyCard)
        {
            rootFamilyCard.HasSplit = hasSplit;
            rootFamilyCard.WasPresentAtCombatStart = false;
        }

        var addResult = await CardPileCmd.Add(rootblightCard, PileType.Deck, CardPilePosition.Bottom, clonedBy: null, skipVisuals: true);
        if (!addResult.success)
        {
            return false;
        }

        ShowRootblightAdded(player, preferOverlayNotice);
        return true;
    }

    private static IEnumerable<(RootFamilyCard Card, int Index)> EnumerateRootFamilyCards(Player player)
    {
        return player.Deck.Cards
            .Select((card, index) => (Card: card, Index: index))
            .Where(entry => entry.Card is RootFamilyCard)
            .Select(entry => (Card: (RootFamilyCard)entry.Card, Index: entry.Index));
    }

    private static async Task TrimRootblightDeckToCap(Player player, string reason)
    {
        var cards = EnumerateRootFamilyCards(player).ToList();
        if (cards.Count <= MaxRootblightCards)
        {
            SetDiagnosticLevelFromDeck(player);
            return;
        }

        var cardsToKeep = cards
            .OrderByDescending(entry => entry.Card.RootblightLevel)
            .ThenBy(entry => entry.Index)
            .Take(MaxRootblightCards)
            .Select(entry => entry.Card)
            .ToHashSet();
        var cardsToRemove = cards
            .Where(entry => !cardsToKeep.Contains(entry.Card))
            .OrderByDescending(entry => entry.Index)
            .Select(entry => entry.Card)
            .ToList();
        foreach (var duplicate in cardsToRemove)
        {
            InternalSyncRemovals.GetValue(duplicate, _ => new InternalSyncMarker());
            await CardPileCmd.RemoveFromDeck(duplicate, showPreview: false);
        }

        SetDiagnosticLevelFromDeck(player);
        ReleaseEvidenceLog.Log(
            "Rootblight",
            "deck_cap_enforced",
            player,
            new Dictionary<string, object?>
            {
                ["removed"] = cardsToRemove.Count,
                ["reason"] = reason,
                ["cap"] = MaxRootblightCards
            });
        MainFile.Logger.Info(
            $"[Spire Plus] Ascension Rootblight trimmed by {reason}: kept {MaxRootblightCards} highest/oldest Rootblight card(s) and removed {cardsToRemove.Count} excess Rootblight card(s) for player {player.RunState.GetPlayerSlotIndex(player)}.");
    }

    private static CardModel CreateRootblightCard(Player player, int level)
    {
        return level switch
        {
            1 => player.RunState.CreateCard<Root>(player),
            2 => player.RunState.CreateCard<DeepRoot>(player),
            _ => player.RunState.CreateCard<RootblightIII>(player),
        };
    }

    private sealed class InternalSyncMarker
    {
    }
}
