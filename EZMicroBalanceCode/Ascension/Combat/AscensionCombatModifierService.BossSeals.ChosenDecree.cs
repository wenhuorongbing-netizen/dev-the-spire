using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
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
