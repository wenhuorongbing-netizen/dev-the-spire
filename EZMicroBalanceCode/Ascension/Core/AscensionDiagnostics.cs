using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class AscensionDiagnostics
{
    public static void LogRunState(RunState runState, string phase)
    {
        if (!AscensionFeatureGate.IsDiagnosticsEnabled)
        {
            return;
        }

        MainFile.Logger.Info(
            $"[Spire Plus] Ascension diagnostics: {phase}; ascension={runState.AscensionLevel}; actIndex={runState.CurrentActIndex}; debugLevel={AscensionFeatureGate.DebugLevel}; publicGate={AscensionFeatureGate.IsPublicGateEnabled}.");

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            LogPlayerRootState(runState, player, phase);
        }
    }

    public static void LogCombatState(CombatState combatState, string phase)
    {
        if (!AscensionFeatureGate.IsDiagnosticsEnabled)
        {
            return;
        }

        var roomType = combatState.RunState.CurrentRoom?.RoomType.ToString() ?? "<none>";
        MainFile.Logger.Info(
            $"[Spire Plus] Ascension diagnostics: {phase}; roomType={roomType}; round={combatState.RoundNumber}; bossBudGate={AscensionFeatureGate.IsEnabledFor(combatState.RunState, AscensionFeatureGate.BossRootBudLevel)}; eliteBudGate={AscensionFeatureGate.IsEnabledFor(combatState.RunState, AscensionFeatureGate.EliteRootBudLevel)}.");

        foreach (var player in combatState.Players.Where(player => player.IsActiveForHooks))
        {
            LogPlayerRootState(combatState.RunState, player, phase);
            var rootBudCount = CountCardsInCombatPiles<RootBud>(player);
            MainFile.Logger.Info(
                $"[Spire Plus] Ascension diagnostics: {phase}; player={combatState.RunState.GetPlayerSlotIndex(player)}; combatBlightSprouts={rootBudCount}.");
        }
    }

    private static void LogPlayerRootState(IRunState runState, Player player, string phase)
    {
        var rootFamilyCards = RootDeckService.FindRootFamilyCards(player);
        MainFile.Logger.Info(
            $"[Spire Plus] Ascension diagnostics: {phase}; player={runState.GetPlayerSlotIndex(player)}; rootBeginsApplied={AscensionSavedStateFields.RootBeginsApplied[player]}; rootblightLevel={RootDeckService.GetRootblightLevel(player)}; rootblightCards={rootFamilyCards.Count}.");
    }

    private static int CountCardsInCombatPiles<TCard>(Player player)
        where TCard : CardModel
    {
        return player.Piles
            .SelectMany(pile => pile.Cards)
            .OfType<TCard>()
            .Count();
    }
}
