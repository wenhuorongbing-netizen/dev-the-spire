namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed record UrdaStateSnapshot(string SelectedBlessing, UrdaProgress Progress)
{
    public static UrdaStateSnapshot Default => new(string.Empty, UrdaProgress.Default);
}

internal static class UrdaStateCodec
{
    private const char ProgressSeparator = ';';
    private const int LegacyMinimumPartCount = 8;
    private const int LegacyBaseIndex = 8;
    private const int CurrentBaseIndex = 9;

    public static UrdaStateSnapshot Decode(string? raw)
    {
        if (string.IsNullOrEmpty(raw))
        {
            return UrdaStateSnapshot.Default;
        }

        var parts = raw.Split(ProgressSeparator);
        var selectedBlessing = parts.Length == 0 ? string.Empty : parts[0];
        if (parts.Length < LegacyMinimumPartCount)
        {
            return new UrdaStateSnapshot(selectedBlessing, UrdaProgress.Default);
        }

        // Keep the historical wire format stable. The current format inserted
        // HumusCompletionPending before MoltingActive; older full states have 28 parts.
        var hasHumusPendingField = parts.Length >= CurrentBaseIndex && parts.Length != 28;
        var baseIndex = hasHumusPendingField ? CurrentBaseIndex : LegacyBaseIndex;

        return new UrdaStateSnapshot(
            selectedBlessing,
            new UrdaProgress(
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
                ParseInt(GetPart(parts, baseIndex + 19))));
    }

    public static string Encode(UrdaStateSnapshot state)
    {
        var progress = state.Progress;
        return string.Join(
            ProgressSeparator,
            state.SelectedBlessing,
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
            progress.SeedbedCombatSlots);
    }

    private static string GetPart(string[] parts, int index) =>
        index >= 0 && index < parts.Length ? parts[index] : string.Empty;

    private static string SanitizeStateField(string value) =>
        (value ?? string.Empty).Replace(ProgressSeparator, '_');

    private static int ParseInt(string value) =>
        int.TryParse(value, out var parsed) ? Math.Max(0, parsed) : 0;

    private static bool ParseBool(string value) =>
        value == "1" || bool.TryParse(value, out var parsed) && parsed;
}
