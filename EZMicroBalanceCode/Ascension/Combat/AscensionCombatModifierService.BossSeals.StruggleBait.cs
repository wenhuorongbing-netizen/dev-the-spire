using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task TrackStruggleBaitObservations(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        var insatiable = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TheInsatiable);
        if (insatiable == null)
        {
            return;
        }

        var strength = insatiable.GetPowerAmount<StrengthPower>();
        var sandpits = insatiable.Powers
            .OfType<SandpitPower>()
            .Where(power => power.Target?.Player is { } player && player.IsActiveForHooks)
            .ToList();

        if (!tracker.StruggleBaitBaselineCaptured)
        {
            tracker.StruggleBaitBaselineCaptured = true;
            tracker.LastInsatiableStrengthAmount = strength;
            foreach (var sandpit in sandpits)
            {
                if (sandpit.Target?.Player is { } player)
                {
                    tracker.LastInsatiableSandpitByPlayer[player] = sandpit.Amount;
                }
            }

            return;
        }

        var targetPlayers = new HashSet<Player>();
        if (!tracker.SuppressStruggleBaitStrengthTrigger &&
            strength > tracker.LastInsatiableStrengthAmount)
        {
            foreach (var player in CurrentInsatiableTargetPlayers(combatState, sandpits))
            {
                targetPlayers.Add(player);
            }
        }

        tracker.LastInsatiableStrengthAmount = strength;

        foreach (var sandpit in sandpits)
        {
            if (sandpit.Target?.Player is not { } player)
            {
                continue;
            }

            tracker.LastInsatiableSandpitByPlayer.TryGetValue(player, out var previousAmount);
            if (sandpit.Amount > previousAmount)
            {
                targetPlayers.Add(player);
            }

            tracker.LastInsatiableSandpitByPlayer[player] = sandpit.Amount;
        }

        foreach (var player in targetPlayers.Take(1))
        {
            await AddStruggleBaitEscape(combatState, tracker, metadata, player);
        }
    }

    private static IEnumerable<Player> CurrentInsatiableTargetPlayers(CombatState combatState, IReadOnlyList<SandpitPower> sandpits)
    {
        var sandpitTargets = sandpits
            .Select(power => power.Target?.Player)
            .Where(player => player?.IsActiveForHooks == true)
            .Cast<Player>()
            .ToList();
        return sandpitTargets.Count > 0
            ? sandpitTargets
            : combatState.Players.Where(player => player.IsActiveForHooks).Take(1);
    }

    private static async Task AddStruggleBaitEscape(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Player player)
    {
        var escape = combatState.CreateCard<FranticEscape>(player);
        await CardPileCmd.AddGeneratedCardToCombat(escape, PileType.Discard, player, CardPilePosition.Bottom);
        tracker.StruggleBaitGeneratedEscapes.Add(escape);
        AscensionSavedStateFields.StruggleBaitGeneratedEscape[escape] = true;
        MainFile.Logger.Info("[Spire Plus] Ascension A19 applied: Escape Fatigue added a dedicated-ability Frantic Escape to the affected player.");
    }

    private static async Task TrackRoyalEscapePlayed(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        FranticEscape escape)
    {
        var generatedByDedicatedAbility =
            tracker.StruggleBaitGeneratedEscapes.Remove(escape) ||
            AscensionSavedStateFields.StruggleBaitGeneratedEscape[escape];
        if (!generatedByDedicatedAbility)
        {
            return;
        }

        AscensionSavedStateFields.StruggleBaitGeneratedEscape[escape] = false;
        tracker.RoyalEscapesPlayed++;
        if (tracker.RoyalEscapesPlayed % 3 != 0 ||
            tracker.StruggleBaitVigorGainRound == combatState.RoundNumber)
        {
            return;
        }

        var insatiable = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TheInsatiable);
        if (insatiable == null)
        {
            return;
        }

        tracker.StruggleBaitVigorGainRound = combatState.RoundNumber;
        var vigorGain = metadata.IsBossBrand ? 3m : 2m;
        await PowerCmd.Apply<VigorPower>(new BlockingPlayerChoiceContext(), insatiable, vigorGain, insatiable, null);
        MainFile.Logger.Info("[Spire Plus] Ascension A19 applied: Escape Fatigue converted three dedicated-ability escapes into Vigor.");
    }
}
