using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

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

        var player = RootSightSelectionPlayer;
        RootSightSelectionPlayer = null;
        NMapScreen.Instance?.RefreshAllPointVisuals();
        ReleaseEvidenceLog.Log("UrdaRootEyes", "selection_cancelled", player);
        MainFile.Logger.Info("[Spire Plus] Urda Root Eyes selection cancelled.");
    }

    internal static bool TryBeginRootSightSelection(Player player)
    {
        var progress = GetProgress(player);
        if (GetSelectedBlessing(player) != UrdaBlessingIds.RootSight ||
            progress.RootSightEyes <= 0)
        {
            return false;
        }

        var hasMultiplayerRunState = player.RunState.Players.Count > 1;
        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopFeature(
            player.RunState,
            "UrdaRootEyes",
            "Root Eyes shared map preview mutation is pending host-authoritative sync proof") ||
            hasMultiplayerRunState)
        {
            MainFile.Logger.Warn("[Spire Plus] Urda Root Eyes preview is single-player only until host-authoritative map preview sync is implemented.");
            return false;
        }

        var mapScreen = NMapScreen.Instance;
        if (mapScreen == null)
        {
            MainFile.Logger.Warn("[Spire Plus] Urda Root Eyes selection could not start because the map screen is not available.");
            return false;
        }

        RootSightSelectionPlayer = player;
        RefreshRootSightRelicStatus(player);
        mapScreen.Open(isOpenedFromTopBar: true);
        mapScreen.RefreshAllPointVisuals();
        ReleaseEvidenceLog.Log("UrdaRootEyes", "selection_opened", player);
        MainFile.Logger.Info("[Spire Plus] Urda Root Eyes selection started; choose a future reachable Monster, Unknown, or Elite map node.");
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
            ReleaseEvidenceLog.Log(
                "UrdaRootEyes",
                "node_selected",
                player,
                new Dictionary<string, object?>
                {
                    ["coord"] = $"{point.coord.col},{point.coord.row}",
                    ["valid"] = false,
                    ["pointType"] = point.PointType
                });
            MainFile.Logger.Info(
                $"[Spire Plus] Urda Root Eyes ignored invalid map target {point.coord.col},{point.coord.row} ({point.PointType}).");
            return;
        }

        ReleaseEvidenceLog.Log(
            "UrdaRootEyes",
            "node_selected",
            player,
            new Dictionary<string, object?>
            {
                ["coord"] = $"{point.coord.col},{point.coord.row}",
                ["valid"] = true,
                ["pointType"] = point.PointType
            });

        if (!TryCreateRootSightPreview(player.RunState, point, out var preview))
        {
            MainFile.Logger.Warn(
                $"[Spire Plus] Urda Root Eyes could not create a preview for {point.coord.col},{point.coord.row} ({point.PointType}).");
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
        ReleaseEvidenceLog.Log(
            "UrdaRootEyes",
            "preview_saved",
            player,
            new Dictionary<string, object?>
            {
                ["coord"] = coord,
                ["roomType"] = preview.RoomType,
                ["modelId"] = preview.ModelId,
                ["eyesLeft"] = progress.RootSightEyes
            });
        EnsureQuestMarker<UrdaRootSightMapQuestMarker>(point);
        RefreshRootSightRelicStatus(player);
        NMapScreen.Instance?.RefreshAllPointVisuals();

        if (!progress.RootSightFirstPotionGranted)
        {
            await TryGivePotion(player);
            SetProgress(player, GetProgress(player) with { RootSightFirstPotionGranted = true });
        }

        MainFile.Logger.Info(
            $"[Spire Plus] Urda Root Eyes previewed {preview.RoomType} {preview.ModelId} at {point.coord.col},{point.coord.row}; eyes left={progress.RootSightEyes}.");
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
            ReleaseEvidenceLog.Log(
                "UrdaRootEyes",
                "selection_cleared_context_changed",
                player,
                new Dictionary<string, object?>
                {
                    ["hasRunState"] = runState != null
                });
            MainFile.Logger.Info("[Spire Plus] Urda Root Eyes selection cleared after run context changed.");
            return null;
        }

        return player;
    }
}
