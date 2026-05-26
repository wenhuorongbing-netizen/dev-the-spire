using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static void MarkChosenDecree(
        CombatState combatState,
        AscensionCombatTracker tracker,
        CardModel card)
    {
        var owner = card.Owner;
        if (owner == null ||
            !CanMarkChosenDecree(card))
        {
            return;
        }

        CardCmd.Enchant<RoyalDecreeEnchantment>(card, 1m);
        AscensionSavedStateFields.RoyalDecreeMarkedCard[card] = true;
        AscensionSavedStateFields.RoyalDecreePlayedCard[card] = false;
        AscensionSavedStateFields.RoyalDecreePlayedBoundCard[card] = false;
        tracker.ChosenDecreeCardsByPlayer[owner] = card;
        tracker.ChosenDecreePlayersWhoPlayedDecree.Remove(owner);
        tracker.ChosenDecreePlayersWhoPlayedAnyBound.Remove(owner);
        MainFile.Logger.Info("[Spire Plus] Ascension A19 applied: Royal Decree marked one Bound card.");
    }

    private static void TryAssignChosenDecreeInHandForPlayer(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Player player)
    {
        if (metadata.BossSeal?.Id != BossSealId.ChosenDecree ||
            !player.IsActiveForHooks)
        {
            return;
        }

        HydrateChosenDecreeFromVisibleCards(combatState, tracker);
        if (tracker.ChosenDecreePlayersWhoPlayedAnyBound.Contains(player) ||
            tracker.ChosenDecreePlayersWhoPlayedDecree.Contains(player))
        {
            return;
        }

        var boundCards = player.Piles
            .Where(pile => pile.Type == PileType.Hand)
            .SelectMany(pile => pile.Cards)
            .Where(CanMarkChosenDecree)
            .ToList();

        if (boundCards.Count == 0)
        {
            return;
        }

        var chosenCard = combatState.RunState.Rng.CombatCardSelection.NextItem(boundCards);
        if (chosenCard == null)
        {
            return;
        }

        // Bound is applied as cards are drawn. Refreshing the whole hand keeps
        // the final visible Decree random among all current Bound cards instead
        // of always marking the first Bound card that entered hand.
        foreach (var boundCard in boundCards)
        {
            if (boundCard.Enchantment is RoyalDecreeEnchantment)
            {
                CardCmd.ClearEnchantment(boundCard);
            }

            AscensionSavedStateFields.RoyalDecreeMarkedCard[boundCard] = false;
            AscensionSavedStateFields.RoyalDecreePlayedCard[boundCard] = false;
            AscensionSavedStateFields.RoyalDecreePlayedBoundCard[boundCard] = false;
        }

        tracker.ChosenDecreeCardsByPlayer.Remove(player);
        MarkChosenDecree(combatState, tracker, chosenCard);
    }

    private static bool CanMarkChosenDecree(CardModel card)
    {
        return card.Affliction is Bound &&
            card.Enchantment == null &&
            ModelDb.Enchantment<RoyalDecreeEnchantment>().CanEnchant(card);
    }

    private static void TrackChosenDecreePlayed(AscensionCombatTracker tracker, CardModel card)
    {
        if (card.Owner is not { } owner)
        {
            return;
        }

        if (card.Affliction is Bound)
        {
            tracker.ChosenDecreePlayersWhoPlayedAnyBound.Add(owner);
            AscensionSavedStateFields.RoyalDecreePlayedBoundCard[card] = true;
        }

        if (tracker.ChosenDecreeCardsByPlayer.TryGetValue(owner, out var decreeCard) && decreeCard == card ||
            card.Enchantment is RoyalDecreeEnchantment)
        {
            tracker.ChosenDecreePlayersWhoPlayedDecree.Add(owner);
            AscensionSavedStateFields.RoyalDecreePlayedCard[card] = true;
        }
    }

    private static void HydrateChosenDecreeFromVisibleCards(
        CombatState combatState,
        AscensionCombatTracker tracker)
    {
        foreach (var player in combatState.Players.Where(player => player.IsActiveForHooks))
        {
            var combatCards = GetChosenDecreeCombatCards(player).ToList();
            var markedDecree = combatCards.FirstOrDefault(card =>
                card.Affliction is Bound &&
                (card.Enchantment is RoyalDecreeEnchantment ||
                    AscensionSavedStateFields.RoyalDecreeMarkedCard[card] ||
                    AscensionSavedStateFields.RoyalDecreePlayedCard[card]));

            if (markedDecree != null && !tracker.ChosenDecreeCardsByPlayer.ContainsKey(player))
            {
                tracker.ChosenDecreeCardsByPlayer[player] = markedDecree;
                MainFile.Logger.Info("[Spire Plus] Ascension A19 recovered Royal Decree tracker from visible card marker.");
            }

            if (combatCards.Any(card => AscensionSavedStateFields.RoyalDecreePlayedBoundCard[card]))
            {
                tracker.ChosenDecreePlayersWhoPlayedAnyBound.Add(player);
            }

            if (combatCards.Any(card => AscensionSavedStateFields.RoyalDecreePlayedCard[card]))
            {
                tracker.ChosenDecreePlayersWhoPlayedDecree.Add(player);
            }
        }
    }

    private static IEnumerable<CardModel> GetChosenDecreeCombatCards(Player player) =>
        player.Piles.SelectMany(pile => pile.Cards);

    private static async Task SettleChosenDecree(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.BossSeal?.Id != BossSealId.ChosenDecree)
        {
            return;
        }

        HydrateChosenDecreeFromVisibleCards(combatState, tracker);
        var affectedPlayers = tracker.ChosenDecreeCardsByPlayer.Keys
            .Concat(tracker.ChosenDecreePlayersWhoPlayedAnyBound)
            .Concat(tracker.ChosenDecreePlayersWhoPlayedDecree)
            .Distinct()
            .ToList();
        if (affectedPlayers.Count == 0)
        {
            return;
        }

        var queen = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is Queen);
        var amalgam = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TorchHeadAmalgam);
        foreach (var player in affectedPlayers)
        {
            if (tracker.ChosenDecreePlayersWhoPlayedDecree.Contains(player))
            {
                MainFile.Logger.Info("[Spire Plus] Ascension A19 applied: Royal Decree was obeyed; no extra penalty was applied.");
            }
            else if (tracker.ChosenDecreePlayersWhoPlayedAnyBound.Contains(player))
            {
                await AddQueenMajesty(combatState, tracker, metadata, queen, 1);
                MainFile.Logger.Info("[Spire Plus] Ascension A19 applied: non-Decree Bound card granted Queen Majesty.");
            }
            else
            {
                await AddQueenMajesty(combatState, tracker, metadata, queen, 1);
                if (amalgam != null && tracker.ChosenDecreeAmalgamStrengthThisRound < 2)
                {
                    tracker.ChosenDecreeAmalgamStrengthThisRound++;
                    await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), amalgam, 1m, queen, null);
                }

                MainFile.Logger.Info("[Spire Plus] Ascension A19 applied: missed Royal Decree granted Majesty and Torch Head Strength.");
            }

            ClearChosenDecreeSavedMarkers(player);
        }

        tracker.ChosenDecreeCardsByPlayer.Clear();
        tracker.ChosenDecreePlayersWhoPlayedDecree.Clear();
        tracker.ChosenDecreePlayersWhoPlayedAnyBound.Clear();
    }

    private static void ClearChosenDecreeSavedMarkers(Player player)
    {
        foreach (var card in GetChosenDecreeCombatCards(player))
        {
            if (card.Enchantment is RoyalDecreeEnchantment)
            {
                CardCmd.ClearEnchantment(card);
            }

            AscensionSavedStateFields.RoyalDecreeMarkedCard[card] = false;
            AscensionSavedStateFields.RoyalDecreePlayedCard[card] = false;
            AscensionSavedStateFields.RoyalDecreePlayedBoundCard[card] = false;
        }
    }

    private static async Task AddQueenMajesty(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature? queen,
        int amount)
    {
        if (queen == null)
        {
            return;
        }

        if (tracker.ChosenDecreeMajestyGainedThisRound >= 2)
        {
            return;
        }

        var gain = Math.Min(amount, 2 - tracker.ChosenDecreeMajestyGainedThisRound);
        tracker.ChosenDecreeMajestyGainedThisRound += gain;
        await PowerCmd.Apply<RoyalMajestyPower>(new BlockingPlayerChoiceContext(), queen, gain, queen, null);
        await ClampPowerAmount<RoyalMajestyPower>(queen, metadata.IsBossBrand ? 3 : 2, queen, null);
    }

    private static void ResetChosenDecreeRoundCaps(AscensionCombatTracker tracker, int roundNumber)
    {
        if (tracker.ChosenDecreeRoundCapRound == roundNumber)
        {
            return;
        }

        tracker.ChosenDecreeRoundCapRound = roundNumber;
        tracker.ChosenDecreeMajestyGainedThisRound = 0;
        tracker.ChosenDecreeAmalgamStrengthThisRound = 0;
    }
}
