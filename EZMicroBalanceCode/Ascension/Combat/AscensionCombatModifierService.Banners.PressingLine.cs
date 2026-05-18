namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private const int PressingLineCardThreshold = 4;
    private const int PressingLineMaxLayersPerPlayer = 3;
    private const int PressingLineMaxPlayersResolved = 2;

    private static Task AfterBannerCardPlayed(
        CombatState combatState,
        AscensionCombatTracker tracker,
        BannerKind banner,
        CardPlay cardPlay)
    {
        if (banner != BannerKind.PressingLine ||
            cardPlay.Card.Owner is not { } player ||
            !player.IsActiveForHooks)
        {
            return Task.CompletedTask;
        }

        if (tracker.PressingLineRound != combatState.RoundNumber)
        {
            tracker.PressingLineRound = combatState.RoundNumber;
            tracker.PressingLineCardsPlayed.Clear();
            tracker.PressingLineLayers.Clear();
        }

        tracker.PressingLineCardsPlayed[player] = tracker.PressingLineCardsPlayed.TryGetValue(player, out var played)
            ? played + 1
            : 1;
        if (tracker.PressingLineCardsPlayed[player] >= PressingLineCardThreshold)
        {
            tracker.PressingLineLayers[player] = Math.Min(
                PressingLineMaxLayersPerPlayer,
                tracker.PressingLineLayers.TryGetValue(player, out var layers) ? layers + 1 : 1);
        }

        return Task.CompletedTask;
    }

    private static async Task ResolvePressingLineTurnEnd(
        CombatState combatState,
        AscensionCombatTracker tracker)
    {
        if (tracker.PressingLineLastResolvedRound == combatState.RoundNumber)
        {
            return;
        }

        tracker.PressingLineLastResolvedRound = combatState.RoundNumber;
        var topLayers = tracker.PressingLineLayers.Values
            .Where(layer => layer > 0)
            .OrderByDescending(layer => layer)
            .Take(PressingLineMaxPlayersResolved)
            .ToList();
        if (topLayers.Count == 0)
        {
            return;
        }

        foreach (var layer in topLayers)
        {
            var block = layer >= PressingLineMaxLayersPerPlayer
                ? GetPressingLineFullBlock(combatState)
                : GetPressingLinePartialBlock(combatState);
            await ApplyBlockToEnemies(PrimaryAliveEnemies(combatState), block);
            if (layer >= PressingLineMaxLayersPerPlayer)
            {
                await ApplyPressingLineStrike(combatState);
            }
        }

        tracker.PressingLineCardsPlayed.Clear();
        tracker.PressingLineLayers.Clear();
        MainFile.Logger.Info("[EZMicroBalance] Ascension A16 applied: Pressing Line banner resolved this player turn.");
    }

    private static async Task ApplyPressingLineStrike(CombatState combatState)
    {
        var damage = GetPressingLineExtraDamage(combatState);
        foreach (var enemy in PrimaryAliveEnemies(combatState))
        {
            await PowerCmd.Apply<PressingLineStrikePower>(new BlockingPlayerChoiceContext(), enemy, damage, enemy, null);
        }
    }
}
