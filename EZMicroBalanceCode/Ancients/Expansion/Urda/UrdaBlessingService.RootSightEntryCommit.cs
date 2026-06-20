using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static readonly ConditionalWeakTable<RunState, HashSet<string>> RootSightCommittedEntryKeys = new();

    private static void ResetRootSightCommittedEntryKeys() =>
        RootSightCommittedEntryKeys.Clear();

    private static bool TryMarkRootSightCommittedForCurrentPoint(RunState runState)
    {
        var current = runState.CurrentMapPoint;
        if (current == null)
        {
            return false;
        }

        var committedForRun = RootSightCommittedEntryKeys.GetOrCreateValue(runState);
        return committedForRun.Add($"{runState.CurrentActIndex}:{FormatCoord(current.coord)}");
    }

    private static void ConsumeRootSightPreviewForCurrentPoint(RunState runState, RootSightPreview preview)
    {
        var current = runState.CurrentMapPoint;
        if (current == null)
        {
            return;
        }

        var coord = FormatCoord(current.coord);
        foreach (var player in runState.Players.Where(player => GetSelectedBlessing(player) == UrdaBlessingIds.RootSight))
        {
            var progress = GetProgress(player);
            if (!TryFindRootSightPreview(progress, runState.CurrentActIndex, coord, out var existing) ||
                existing.RoomType != preview.RoomType ||
                existing.ModelId != preview.ModelId)
            {
                continue;
            }

            var previews = GetRootSightPreviews(progress.RootSightPreviewRecords)
                .Where(candidate => candidate.ActIndex != runState.CurrentActIndex || candidate.Coord != coord)
                .ToList();
            var marked = GetCoordSet(progress.RootSightMarkedCoords);
            marked.Remove(FormatRootSightMarkedCoord(runState.CurrentActIndex, coord));
            if (runState.CurrentActIndex == 0)
            {
                marked.Remove(coord);
            }

            SetProgress(player, progress with
            {
                RootSightMarkedCoords = string.Join("|", marked),
                RootSightPreviewRecords = FormatRootSightPreviews(previews)
            });
            ReleaseEvidenceLog.Log(
                "UrdaRootEyes",
                "preview_consumed",
                player,
                new Dictionary<string, object?>
                {
                    ["coord"] = coord,
                    ["roomType"] = preview.RoomType,
                    ["modelId"] = preview.ModelId
                });
            RefreshRootSightRelicStatus(player);
        }

        RemoveQuestMarker<UrdaRootSightMapQuestMarker>(current);
        NMapScreen.Instance?.RefreshAllPointVisuals();
        MainFile.Logger.Info(
            $"[Spire Plus] Urda Root Eyes consumed preview {preview.RoomType} {preview.ModelId} at {coord}.");
    }
}
