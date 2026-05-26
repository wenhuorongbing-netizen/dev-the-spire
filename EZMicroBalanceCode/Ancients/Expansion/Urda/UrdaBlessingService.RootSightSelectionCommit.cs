using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
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
}
