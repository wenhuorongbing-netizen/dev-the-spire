using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static Player? RootSightSelectionPlayer;

    private static void ResetRootSightTransientState()
    {
        RootSightSelectionPlayer = null;
        ResetRootSightCommittedEntryKeys();
    }

    internal static int GetRootSightEyes(Player player) =>
        GetProgress(player).RootSightEyes;

    internal static bool IsRootSightSelectionActive =>
        GetActiveRootSightSelectionPlayer() != null;

    internal static void CancelRootSightSelection()
    {
        if (RootSightSelectionPlayer == null)
        {
            return;
        }

        RootSightSelectionPlayer = null;
        NMapScreen.Instance?.RefreshAllPointVisuals();
        MainFile.Logger.Info("[EZMicroBalance] Urda Root Eyes selection cancelled.");
    }

    internal static bool TryBeginRootSightSelection(Player player)
    {
        var progress = GetProgress(player);
        if (GetSelectedBlessing(player) != UrdaBlessingIds.RootSight ||
            progress.RootSightEyes <= 0)
        {
            return false;
        }

        if (player.RunState.Players.Count > 1)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Urda Root Eyes preview is single-player only until host-authoritative map preview sync is implemented.");
            return false;
        }

        var mapScreen = NMapScreen.Instance;
        if (mapScreen == null)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Urda Root Eyes selection could not start because the map screen is not available.");
            return false;
        }

        RootSightSelectionPlayer = player;
        RefreshRootSightRelicStatus(player);
        mapScreen.Open(isOpenedFromTopBar: true);
        mapScreen.RefreshAllPointVisuals();
        MainFile.Logger.Info("[EZMicroBalance] Urda Root Eyes selection started; choose a future reachable Monster, Unknown, or Elite map node.");
        return true;
    }

    internal static async Task TryCommitRootSightSelection(MapPoint point)
    {
        var player = GetActiveRootSightSelectionPlayer();
        if (player == null)
        {
            return;
        }

        if (!IsRootSightTarget(player, point))
        {
            MainFile.Logger.Info(
                $"[EZMicroBalance] Urda Root Eyes ignored invalid map target {point.coord.col},{point.coord.row} ({point.PointType}).");
            return;
        }

        if (!TryCreateRootSightPreview(player.RunState, point, out var preview))
        {
            MainFile.Logger.Warn(
                $"[EZMicroBalance] Urda Root Eyes could not create a preview for {point.coord.col},{point.coord.row} ({point.PointType}).");
            return;
        }

        RootSightSelectionPlayer = null;
        var progress = GetProgress(player);
        var coord = FormatCoord(point.coord);
        var marked = GetCoordSet(progress.RootSightMarkedCoords);
        marked.Add(FormatRootSightMarkedCoord(player.RunState.CurrentActIndex, coord));

        var previews = GetRootSightPreviews(progress.RootSightPreviewRecords)
            .Where(existing => existing.ActIndex != player.RunState.CurrentActIndex || existing.Coord != coord)
            .Append(preview)
            .ToList();
        progress = progress with
        {
            RootSightEyes = Math.Max(0, progress.RootSightEyes - 1),
            RootSightMarkedCoords = string.Join("|", marked),
            RootSightPreviewRecords = FormatRootSightPreviews(previews)
        };
        SetProgress(player, progress);
        EnsureQuestMarker<UrdaRootSightMapQuestMarker>(point);
        RefreshRootSightRelicStatus(player);
        NMapScreen.Instance?.RefreshAllPointVisuals();

        if (!progress.RootSightFirstPotionGranted)
        {
            await TryGivePotion(player);
            SetProgress(player, GetProgress(player) with { RootSightFirstPotionGranted = true });
        }

        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Root Eyes previewed {preview.RoomType} {preview.ModelId} at {point.coord.col},{point.coord.row}; eyes left={progress.RootSightEyes}.");
    }

    private static Player? GetActiveRootSightSelectionPlayer()
    {
        var player = RootSightSelectionPlayer;
        if (player == null)
        {
            return null;
        }

        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null ||
            !ReferenceEquals(player.RunState, runState) ||
            !runState.Players.Contains(player))
        {
            RootSightSelectionPlayer = null;
            MainFile.Logger.Info("[EZMicroBalance] Urda Root Eyes selection cleared after run context changed.");
            return null;
        }

        return player;
    }
}
