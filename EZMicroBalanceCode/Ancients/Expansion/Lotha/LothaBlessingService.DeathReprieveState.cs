using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private static void HydrateDeathReprieveState(Player player, LothaCombatState combatState)
    {
        if (GetSelectedBlessing(player) != LothaBlessingIds.DeathReprieve)
        {
            return;
        }

        var progress = GetProgress(player);
        if (!progress.DeathReprieveUsed ||
            !IsRecoverableDeathReprievePhase(progress.DeathReprievePhase) ||
            combatState.DeathReprieveActive ||
            combatState.DeathReprievePendingStart)
        {
            return;
        }

        var alreadyHasPower = player.Creature.GetPower<LothaDeathReprievePower>() != null;
        combatState.DeathReprieveActive = true;
        combatState.DeathReprievePendingStart = progress.DeathReprievePhase == DeathReprievePhase.PendingStart;
        combatState.DeathReprieveStarted = progress.DeathReprievePhase == DeathReprievePhase.Active && alreadyHasPower;
        ReleaseEvidenceLog.Log(
            "LothaDeathReprieve",
            "save_hydrate",
            player,
            new Dictionary<string, object?>
            {
                ["phase"] = progress.DeathReprievePhase,
                ["pendingStart"] = combatState.DeathReprievePendingStart,
                ["powerAlreadyPresent"] = alreadyHasPower
            });
        MainFile.Logger.Info(
            $"[Spire Plus] Lotha Death Reprieve restored {progress.DeathReprievePhase} combat state from deck-mirrored blessing progress; " +
            $"pendingStart={combatState.DeathReprievePendingStart}, powerAlreadyPresent={alreadyHasPower}. Active-turn save/load continuation remains live-pending.");
    }

    private static bool IsRecoverableDeathReprievePhase(DeathReprievePhase phase) =>
        phase is DeathReprievePhase.PendingStart or DeathReprievePhase.Active;

    private static void ResolveDeathReprieveProgress(Player player)
    {
        var progress = GetProgress(player);
        if (progress.DeathReprieveUsed && progress.DeathReprievePhase != DeathReprievePhase.Resolved)
        {
            SetProgress(player, progress with { DeathReprievePhase = DeathReprievePhase.Resolved });
            ReleaseEvidenceLog.Log("LothaDeathReprieve", "state_cleared", player);
        }
    }
}
