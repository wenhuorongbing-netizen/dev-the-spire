using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static void TryAssignChosenDecree(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        CardModel card)
    {
        if (metadata.BossSeal?.Id != BossSealId.ChosenDecree ||
            tracker.ChosenDecreeCard != null ||
            card.Affliction is not Bound ||
            card.Enchantment != null)
        {
            return;
        }

        CardCmd.Enchant<RoyalDecreeEnchantment>(card, 1m);
        tracker.ChosenDecreeCard = card;
        tracker.ChosenDecreePlayed = false;
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Royal Decree marked one Bound card.");
    }

    private static void TryAssignChosenDecreeInHands(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        var boundCards = combatState.Players
                     .Where(player => player.IsActiveForHooks)
                     .SelectMany(player => player.Piles)
                     .Where(pile => pile.Type == PileType.Hand)
                     .SelectMany(pile => pile.Cards)
                     .Where(card => card.Affliction is Bound && card.Enchantment == null)
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

        TryAssignChosenDecree(combatState, tracker, metadata, chosenCard);
    }

    private static void TrackChosenDecreePlayed(AscensionCombatTracker tracker, CardModel card)
    {
        if (card.Affliction is Bound)
        {
            tracker.ChosenDecreeAnyBoundPlayed = true;
        }

        if (tracker.ChosenDecreeCard == card ||
            card.Enchantment is RoyalDecreeEnchantment)
        {
            tracker.ChosenDecreePlayed = true;
        }
    }

    private static async Task SettleChosenDecree(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.BossSeal?.Id != BossSealId.ChosenDecree)
        {
            return;
        }

        TryAssignChosenDecreeInHands(combatState, tracker, metadata);
        if (tracker.ChosenDecreeCard == null)
        {
            return;
        }

        var queen = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is Queen);
        var amalgam = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TorchHeadAmalgam);
        if (tracker.ChosenDecreePlayed)
        {
            MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Royal Decree was obeyed; no extra penalty was applied.");
        }
        else if (tracker.ChosenDecreeAnyBoundPlayed)
        {
            await AddQueenMajesty(combatState, tracker, metadata, queen, 1);
            MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: non-Decree Bound card granted Queen Majesty.");
        }
        else
        {
            await AddQueenMajesty(combatState, tracker, metadata, queen, 1);
            if (amalgam != null && tracker.ChosenDecreeAmalgamStrengthThisRound < 2)
            {
                tracker.ChosenDecreeAmalgamStrengthThisRound++;
                await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), amalgam, 1m, queen, null);
            }

            MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: missed Royal Decree granted Majesty and Torch Head Strength.");
        }

        if (tracker.ChosenDecreeCard.Enchantment is RoyalDecreeEnchantment)
        {
            CardCmd.ClearEnchantment(tracker.ChosenDecreeCard);
        }

        tracker.ChosenDecreeCard = null;
        tracker.ChosenDecreePlayed = false;
        tracker.ChosenDecreeAnyBoundPlayed = false;
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

    private static void ResetChosenDecreeRoundCaps(AscensionCombatTracker tracker)
    {
        tracker.ChosenDecreeMajestyGainedThisRound = 0;
        tracker.ChosenDecreeAmalgamStrengthThisRound = 0;
    }
}
