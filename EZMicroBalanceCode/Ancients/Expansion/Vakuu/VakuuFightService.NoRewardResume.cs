using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Rooms;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static partial class VakuuFightService
{
    public static async Task ProceedFromNoRewardVictory(CombatRoom combatRoom)
    {
        ReleaseEvidenceLog.Log(
            "VakuuFight",
            "victory_rewards_suppressed",
            runState: combatRoom.CombatState.RunState);
        MainFile.Logger.Info(
            "[EZMicroBalance] Vakuu fight has no normal combat rewards; resuming the Vakuu event reward choice.");

        if (combatRoom.ShouldResumeParentEventAfterCombat &&
            combatRoom.CombatState.RunState.CurrentRoomCount > 1)
        {
            ReleaseEvidenceLog.Log(
                "VakuuFight",
                "parent_event_resume_attempted",
                runState: combatRoom.CombatState.RunState);
            await Cmd.Wait(1f);
            await RunManager.Instance.ProceedFromTerminalRewardsScreen();
            ReleaseEvidenceLog.Log(
                "VakuuFight",
                "parent_event_resume_success",
                runState: combatRoom.CombatState.RunState);
            return;
        }

        await ProceedFromMissingParentStackNoRewardVictory(combatRoom);
    }

    private static async Task ProceedFromMissingParentStackNoRewardVictory(CombatRoom combatRoom)
    {
        MainFile.Logger.Warn(
            "[EZMicroBalance] Vakuu fight no-reward resume found no valid parent event stack; opening the map instead of leaving a finished combat screen.");

        await Cmd.Wait(0.2f);
        NMapScreen.Instance?.SetTravelEnabled(enabled: true);
        NMapScreen.Instance?.Open();
        ReleaseEvidenceLog.Log(
            "VakuuFight",
            "fallback_map_exit",
            runState: combatRoom.CombatState.RunState);
    }
}
