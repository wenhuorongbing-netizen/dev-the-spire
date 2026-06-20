namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

internal static partial class LothaBlessingService
{
    public static bool ShouldDieLate(Creature creature)
    {
        if (!creature.IsPlayer)
        {
            return true;
        }

        if (creature.Player is not { } player)
        {
            return true;
        }

        if (GetSelectedBlessing(player) != LothaBlessingIds.DeathReprieve)
        {
            return true;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        HydrateDeathReprieveState(player, combatState);
        if (combatState.DeathReprieveActive || combatState.DeathReprievePendingStart)
        {
            return false;
        }

        return GetProgress(player).DeathReprieveUsed;
    }

    public static bool ShouldDie(Creature creature)
    {
        if (!creature.IsPlayer ||
            creature.Player is not { } player ||
            GetSelectedBlessing(player) != LothaBlessingIds.DeathReprieve)
        {
            return true;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        HydrateDeathReprieveState(player, combatState);
        return !(combatState.DeathReprieveActive || combatState.DeathReprievePendingStart);
    }

    public static async Task AfterPreventingDeath(Creature creature)
    {
        if (!creature.IsPlayer)
        {
            return;
        }

        if (creature.Player is not { } player)
        {
            return;
        }

        if (GetSelectedBlessing(player) != LothaBlessingIds.DeathReprieve)
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        HydrateDeathReprieveState(player, combatState);
        var progress = GetProgress(player);
        if (progress.DeathReprieveUsed)
        {
            if (combatState.DeathReprieveActive || combatState.DeathReprievePendingStart)
            {
                ReleaseEvidenceLog.Log(
                    "LothaDeathReprieve",
                    "duplicate_prevented",
                    player,
                    DeathReprieveDiagnostics(player, combatState, progress, "duplicate lethal damage during reprieve"));
                await CreatureCmd.SetCurrentHp(creature, 1m);
                MainFile.Logger.Info("[Spire Plus] Lotha Death Reprieve kept the player at 1 HP during the reprieve turn.");
            }

            return;
        }

        await CreatureCmd.SetCurrentHp(creature, 1m);

        if (creature.CombatState?.CurrentSide == CombatSide.Player &&
            CombatManager.Instance.IsPartOfPlayerTurn(player))
        {
            var activeProgress = progress with
            {
                DeathReprieveUsed = true,
                DeathReprievePhase = DeathReprievePhase.Active
            };
            SetProgress(player, activeProgress);
            const string source = "current player turn after lethal damage";
            ReleaseEvidenceLog.Log("LothaDeathReprieve", "active_entered", player, DeathReprieveDiagnostics(player, combatState, activeProgress, source));
            await StartDeathReprieveTurn(new ThrowingPlayerChoiceContext(), player, combatState, source);
        }
        else
        {
            var pendingProgress = progress with
            {
                DeathReprieveUsed = true,
                DeathReprievePhase = DeathReprievePhase.PendingStart
            };
            SetProgress(player, pendingProgress);
            combatState.DeathReprievePendingStart = true;
            combatState.DeathReprieveActive = true;
            await EnsureDeathReprievePower(new ThrowingPlayerChoiceContext(), player);
            ReleaseEvidenceLog.Log(
                "LothaDeathReprieve",
                "pending_created",
                player,
                DeathReprieveDiagnostics(player, combatState, pendingProgress, "enemy or non-player turn lethal damage"));
            MainFile.Logger.Info("[Spire Plus] Lotha Death Reprieve prevented lethal damage; reprieve turn is pending at the next player turn.");
        }
    }

    private static Dictionary<string, object?> DeathReprieveDiagnostics(
        Player player,
        LothaCombatState combatState,
        Progress progress,
        string source,
        bool forcedDeath = false) =>
        new()
        {
            ["source"] = source,
            ["hp"] = player.Creature.CurrentHp,
            ["currentSide"] = player.Creature.CombatState?.CurrentSide.ToString() ?? "none",
            ["used"] = progress.DeathReprieveUsed,
            ["phase"] = progress.DeathReprievePhase,
            ["active"] = combatState.DeathReprieveActive,
            ["pendingStart"] = combatState.DeathReprievePendingStart,
            ["started"] = combatState.DeathReprieveStarted,
            ["forcedDeath"] = forcedDeath
        };
}
