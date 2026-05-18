using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Rooms;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static partial class VakuuFightService
{
    public static async Task ProceedFromNoRewardVictory(CombatRoom combatRoom)
    {
        MainFile.Logger.Info(
            "[EZMicroBalance] Vakuu fight has no normal combat rewards; resuming the Vakuu event reward choice.");

        if (combatRoom.ShouldResumeParentEventAfterCombat &&
            combatRoom.CombatState.RunState.CurrentRoomCount > 1)
        {
            await Cmd.Wait(1f);
            await RunManager.Instance.ProceedFromTerminalRewardsScreen();
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
    }
}
