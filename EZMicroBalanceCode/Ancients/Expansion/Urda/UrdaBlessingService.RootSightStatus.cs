namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private const int RootSightStartingEyes = 5;

    public static async Task ApplyRootSight(Player player)
    {
        ResetRootSightTransientState();
        SetProgress(player, GetProgress(player) with
        {
            RootSightEyes = RootSightStartingEyes,
            RootSightFirstPotionGranted = false,
            RootSightMarkedCoords = string.Empty,
            RootSightPreviewRecords = string.Empty
        });
        RefreshRootSightRelicStatus(player);
        await Task.CompletedTask;
    }

    private static void RefreshRootSightRelicStatus(Player player)
    {
        var relic = player.Relics.OfType<UrdaRootSightOptionRelic>().FirstOrDefault();
        if (relic == null)
        {
            return;
        }

        var progress = GetProgress(player);
        relic.Status = progress.RootSightEyes > 0
            ? RelicStatus.Active
            : RelicStatus.Disabled;
        relic.RefreshRootSightDisplay();
    }
}
