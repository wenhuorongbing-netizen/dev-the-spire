using MegaCrit.Sts2.Core.Entities.Cards;

using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class RootDeckService
{
    public static bool CanHoldRootblightBySeedbed(RootFamilyCard card)
    {
        return card.Owner is { } player &&
            AscensionFeatureGate.IsRootblightEnabled(player.RunState) &&
            TryFindRootblightDeckVersion(player, card) != null;
    }

    public static bool TryHoldRootblightBySeedbed(RootFamilyCard card)
    {
        var player = card.Owner;
        if (player == null || !AscensionFeatureGate.IsRootblightEnabled(player.RunState))
        {
            return false;
        }

        var deckCard = TryFindRootblightDeckVersion(player, card);
        if (deckCard == null)
        {
            MainFile.Logger.Warn(
                $"[Spire Plus] Ascension Rootblight skipped: Seedbed could not find a unique master-deck level {card.RootblightLevel} card for player {player.RunState.GetPlayerSlotIndex(player)}.");
            return false;
        }

        deckCard.PlantedInSeedbed = true;
        card.PlantedInSeedbed = true;
        ReleaseEvidenceLog.Log(
            "Rootblight",
            "seedbed_hold",
            player,
            new Dictionary<string, object?>
            {
                ["level"] = deckCard.RootblightLevel
            });
        MainFile.Logger.Info(
            $"[Spire Plus] Ascension Rootblight held by Seedbed: level {deckCard.RootblightLevel} will not grow or downgrade at combat end for player {player.RunState.GetPlayerSlotIndex(player)}.");
        return true;
    }

    public static async Task ApplyPlayedRootblightCard(RootFamilyCard card)
    {
        var player = card.Owner;
        if (player == null || !AscensionFeatureGate.IsRootblightEnabled(player.RunState))
        {
            return;
        }

        var deckCard = TryFindRootblightDeckVersion(player, card);
        if (deckCard == null)
        {
            MainFile.Logger.Warn(
                $"[Spire Plus] Ascension Rootblight skipped: played level {card.RootblightLevel} for player {player.RunState.GetPlayerSlotIndex(player)} had no unique master-deck card.");
            return;
        }

        var splitState = deckCard.HasSplit;
        InternalSyncRemovals.GetValue(deckCard, _ => new InternalSyncMarker());
        await CardPileCmd.RemoveFromDeck(deckCard, showPreview: false);

        var downgradedLevel = card.RootblightLevel - 1;
        if (downgradedLevel > 0)
        {
            QueuePendingCombatDowngrade(player, downgradedLevel, splitState);
        }

        SetDiagnosticLevelFromDeck(player);

        MainFile.Logger.Info(
            $"[Spire Plus] Ascension Rootblight applied: played level {card.RootblightLevel}; removed its master-deck card and queued {downgradedLevel} downgrade level for player {player.RunState.GetPlayerSlotIndex(player)}.");
    }

    public static async Task RemoveHighestRootblight(Player player, string reason)
    {
        if (!AscensionFeatureGate.IsRootblightEnabled(player.RunState))
        {
            return;
        }

        var rootblightCard = EnumerateRootFamilyCards(player)
            .OrderByDescending(entry => entry.Card.RootblightLevel)
            .ThenBy(entry => entry.Index)
            .Select(entry => entry.Card)
            .FirstOrDefault();
        if (rootblightCard == null)
        {
            return;
        }

        InternalSyncRemovals.GetValue(rootblightCard, _ => new InternalSyncMarker());
        await CardPileCmd.RemoveFromDeck(rootblightCard, showPreview: false);
        SetDiagnosticLevelFromDeck(player);

        MainFile.Logger.Info(
            $"[Spire Plus] Ascension Rootblight removed by {reason}: removed level {rootblightCard.RootblightLevel} for player {player.RunState.GetPlayerSlotIndex(player)}.");
    }

    public static Task BeforeCardRemoved(CardModel card)
    {
        if (card.Owner == null ||
            card is not RootFamilyCard ||
            InternalSyncRemovals.TryGetValue(card, out _))
        {
            return Task.CompletedTask;
        }

        var player = card.Owner;
        SetLevelFromCards(
            player,
            FindRootFamilyCards(player)
                .Where(deckCard => !ReferenceEquals(deckCard, card)));
        MarkRootBeginsApplied(player);

        MainFile.Logger.Info(
            $"[Spire Plus] Ascension Rootblight removed through a deck-removal API for player {player.RunState.GetPlayerSlotIndex(player)}; remaining Rootblight cards are preserved.");
        return Task.CompletedTask;
    }
}
