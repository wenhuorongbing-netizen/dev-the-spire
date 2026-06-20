using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    public static async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player)
        {
            return;
        }

        var activeCombatState = CombatManager.Instance.DebugOnlyGetState();
        if (activeCombatState == null)
        {
            return;
        }

        foreach (var player in activeCombatState.Players.Where(player => player.IsActiveForHooks))
        {
            var combatState = CombatStates.GetOrCreateValue(player);
            var selectedBlessing = GetSelectedBlessing(player);

            if (selectedBlessing == LothaBlessingIds.MirrorHallEcho)
            {
                RecordMirrorHallEchoType(player, combatState);
            }

            if (selectedBlessing == LothaBlessingIds.DeferredVerdict &&
                player.Creature.CombatState?.RoundNumber == DeferredVerdictTurn)
            {
                combatState.DeferredVerdictActiveThisTurn = false;
                await PowerCmd.Remove<LothaVerdictPower>(player.Creature);
                MainFile.Logger.Info("[Spire Plus] Lotha Deferred Verdict removed Verdict at turn end.");
            }

            if (selectedBlessing == LothaBlessingIds.DeathReprieve && combatState.DeathReprieveActive)
            {
                await ResolveDeathReprieveTurnEnd(player, combatState);
            }
        }
    }
}
