using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

internal static partial class LothaBlessingService
{
    private const char ProgressSeparator = ';';

    private enum DeathReprievePhase
    {
        None = 0,
        PendingStart = 1,
        Active = 2,
        Resolved = 3
    }

    private sealed record Progress(bool DeathReprieveUsed, DeathReprievePhase DeathReprievePhase)
    {
        public static Progress Default => new(false, DeathReprievePhase.None);
    }

    public static void SetSelectedBlessing(Player player, string blessingId)
    {
        if (blessingId != LothaBlessingIds.MirrorRebuttal)
        {
            ClearMirrorRebuttalMarkedCards(player);
        }

        SetState(player, blessingId, Progress.Default);
    }

    public static string GetSelectedBlessing(Player player)
    {
        var state = AncientPlayerState.Get(
            player,
            AncientSavedStateFields.LothaStateKey,
            AncientSavedStateFields.LothaDeckStateKey);
        var separatorIndex = state.IndexOf(ProgressSeparator);
        return separatorIndex < 0 ? state : state[..separatorIndex];
    }

    public static void SyncPersistentState(Player? player)
    {
        if (player == null)
        {
            return;
        }

        AncientPlayerState.SyncDeck(
            player,
            AncientSavedStateFields.LothaStateKey,
            AncientSavedStateFields.LothaDeckStateKey);
    }

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
            $"[EZMicroBalance] Lotha Death Reprieve restored {progress.DeathReprievePhase} combat state from deck-mirrored blessing progress; " +
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

    private static Progress GetProgress(Player player)
    {
        var state = AncientPlayerState.Get(
            player,
            AncientSavedStateFields.LothaStateKey,
            AncientSavedStateFields.LothaDeckStateKey);
        var parts = state.Split(ProgressSeparator);
        if (parts.Length < 2)
        {
            return Progress.Default;
        }

        var used = ParseBool(parts[1]);
        var phase = parts.Length >= 3
            ? ParseDeathReprievePhase(parts[2], used)
            : used ? DeathReprievePhase.Resolved : DeathReprievePhase.None;
        if (!used)
        {
            phase = DeathReprievePhase.None;
        }

        return new Progress(used, phase);
    }

    private static void SetProgress(Player player, Progress progress)
    {
        var selectedBlessing = GetSelectedBlessing(player);
        if (!string.IsNullOrWhiteSpace(selectedBlessing))
        {
            SetState(player, selectedBlessing, progress);
        }
    }

    private static void SetState(Player player, string blessingId, Progress progress)
    {
        AncientPlayerState.Set(
            player,
            string.Join(
                ProgressSeparator,
                blessingId,
                progress.DeathReprieveUsed ? 1 : 0,
                (int)progress.DeathReprievePhase),
            AncientSavedStateFields.LothaStateKey,
            AncientSavedStateFields.LothaDeckStateKey);
    }

    private static bool ParseBool(string value) =>
        value == "1" || bool.TryParse(value, out var parsed) && parsed;

    private static DeathReprievePhase ParseDeathReprievePhase(string value, bool used)
    {
        if (int.TryParse(value, out var numeric) &&
            Enum.IsDefined(typeof(DeathReprievePhase), numeric))
        {
            return (DeathReprievePhase)numeric;
        }

        if (Enum.TryParse(value, ignoreCase: true, out DeathReprievePhase parsed))
        {
            return parsed;
        }

        return used ? DeathReprievePhase.Resolved : DeathReprievePhase.None;
    }
}
