namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private const char ProgressSeparator = ';';

    private sealed record Progress(
        int SeedbedChecks,
        int SeedbedAccepted,
        bool SeedbedTransformed,
        int HumusSkips,
        bool HumusCompleted,
        bool HumusCompletionPending,
        bool MoltingActive,
        int MossRoomMask,
        int TrialCombats,
        int TrialSuccessfulCombats,
        bool TrialPlayedThisCombat,
        bool TrialSettled,
        bool ShallowRelicPending,
        bool ShallowRelicRooted,
        string ShallowRelicId,
        string RootedRouteCoord,
        bool RootedRouteResolved,
        bool RootedRouteWithered,
        bool AfterRainSpent,
        bool AfterRainCompensated,
        int AfterRainEliteGoldCount,
        int RootSightEyes,
        bool RootSightFirstPotionGranted,
        string RootSightMarkedCoords,
        string SeedBankCardIds,
        bool SeedBankSettled,
        string RootSightPreviewRecords,
        int SeedbedCombatSlots)
    {
        public static Progress Default => new(
            0,
            0,
            false,
            0,
            false,
            false,
            false,
            0,
            0,
            0,
            false,
            false,
            false,
            false,
            string.Empty,
            string.Empty,
            false,
            false,
            false,
            false,
            0,
            0,
            false,
            string.Empty,
            string.Empty,
            false,
            string.Empty,
            0);
    }
}
