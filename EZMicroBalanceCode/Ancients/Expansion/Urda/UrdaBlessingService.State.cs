using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    public static void SetSelectedBlessing(Player player, string blessingId)
    {
        SetState(player, blessingId, Progress.Default);
    }

    public static string GetSelectedBlessing(Player player)
    {
        var state = AncientPlayerState.Get(
            player,
            AncientSavedStateFields.UrdaStateKey,
            AncientSavedStateFields.UrdaDeckStateKey);
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
            AncientSavedStateFields.UrdaStateKey,
            AncientSavedStateFields.UrdaDeckStateKey);
        if (GetSelectedBlessing(player) == UrdaBlessingIds.TrialBranch)
        {
            RefreshTrialBranchEnchantment(player);
        }
    }

    private static Progress GetProgress(Player player)
    {
        var state = AncientPlayerState.Get(
            player,
            AncientSavedStateFields.UrdaStateKey,
            AncientSavedStateFields.UrdaDeckStateKey);
        var parts = state.Split(ProgressSeparator);
        if (parts.Length < 8)
        {
            return Progress.Default;
        }

        var hasHumusPendingField = parts.Length >= 9;
        var baseIndex = hasHumusPendingField ? 9 : 8;
        return new Progress(
            ParseInt(parts[1]),
            ParseInt(parts[2]),
            ParseBool(parts[3]),
            ParseInt(parts[4]),
            ParseBool(parts[5]),
            hasHumusPendingField && ParseBool(parts[6]),
            ParseBool(parts[hasHumusPendingField ? 7 : 6]),
            ParseInt(parts[hasHumusPendingField ? 8 : 7]),
            ParseInt(GetPart(parts, baseIndex)),
            ParseInt(GetPart(parts, baseIndex + 1)),
            ParseBool(GetPart(parts, baseIndex + 2)),
            ParseBool(GetPart(parts, baseIndex + 3)),
            ParseBool(GetPart(parts, baseIndex + 4)),
            ParseBool(GetPart(parts, baseIndex + 5)),
            GetPart(parts, baseIndex + 6),
            GetPart(parts, baseIndex + 7),
            ParseBool(GetPart(parts, baseIndex + 8)),
            ParseBool(GetPart(parts, baseIndex + 9)),
            ParseBool(GetPart(parts, baseIndex + 10)),
            ParseBool(GetPart(parts, baseIndex + 11)),
            ParseInt(GetPart(parts, baseIndex + 12)),
            ParseInt(GetPart(parts, baseIndex + 13)),
            ParseBool(GetPart(parts, baseIndex + 14)),
            GetPart(parts, baseIndex + 15),
            GetPart(parts, baseIndex + 16),
            ParseBool(GetPart(parts, baseIndex + 17)),
            GetPart(parts, baseIndex + 18),
            ParseInt(GetPart(parts, baseIndex + 19)));
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
                progress.SeedbedChecks,
                progress.SeedbedAccepted,
                progress.SeedbedTransformed ? 1 : 0,
                progress.HumusSkips,
                progress.HumusCompleted ? 1 : 0,
                progress.HumusCompletionPending ? 1 : 0,
                progress.MoltingActive ? 1 : 0,
                progress.MossRoomMask,
                progress.TrialCombats,
                progress.TrialSuccessfulCombats,
                progress.TrialPlayedThisCombat ? 1 : 0,
                progress.TrialSettled ? 1 : 0,
                progress.ShallowRelicPending ? 1 : 0,
                progress.ShallowRelicRooted ? 1 : 0,
                SanitizeStateField(progress.ShallowRelicId),
                SanitizeStateField(progress.RootedRouteCoord),
                progress.RootedRouteResolved ? 1 : 0,
                progress.RootedRouteWithered ? 1 : 0,
                progress.AfterRainTriggeredThisCombat ? 1 : 0,
                progress.AfterRainCompensated ? 1 : 0,
                progress.AfterRainTriggerCount,
                progress.RootSightEyes,
                progress.RootSightFirstPotionGranted ? 1 : 0,
                SanitizeStateField(progress.RootSightMarkedCoords),
                SanitizeStateField(progress.SeedBankCardIds),
                progress.SeedBankSettled ? 1 : 0,
                SanitizeStateField(progress.RootSightPreviewRecords),
                progress.SeedbedCombatSlots),
            AncientSavedStateFields.UrdaStateKey,
            AncientSavedStateFields.UrdaDeckStateKey);
    }
}
