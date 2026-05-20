using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    internal static void RestoreRootSightPreviewMarkers(ActMap map, int actIndex)
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null)
        {
            return;
        }

        foreach (var player in runState.Players.Where(player => GetSelectedBlessing(player) == UrdaBlessingIds.RootSight))
        {
            foreach (var preview in GetRootSightPreviews(GetProgress(player).RootSightPreviewRecords)
                .Where(preview => preview.ActIndex == actIndex))
            {
                if (!TryParseCoord(preview.Coord, out var coord))
                {
                    continue;
                }

                var point = map.GetPoint(coord);
                if (point == null ||
                    point.PointType != preview.PointType ||
                    point.PointType is not (MapPointType.Monster or MapPointType.Unknown or MapPointType.Elite))
                {
                    ClearStaleRootSightPreview(player, actIndex, preview.Coord, point);
                    continue;
                }

                if (!IsFutureReachableRootSightTarget(runState, point))
                {
                    ClearStaleRootSightPreview(player, actIndex, preview.Coord, point);
                    continue;
                }

                if (runState is RunState concreteRunState &&
                    !IsRootSightPreviewStillValidForEntry(concreteRunState, preview))
                {
                    ClearStaleRootSightPreview(player, actIndex, preview.Coord, point);
                    continue;
                }

                EnsureQuestMarker<UrdaRootSightMapQuestMarker>(point);
            }
        }
    }

    private static string FormatRootSightMarkedCoord(int actIndex, string coord) =>
        $"{actIndex}:{coord}";

    private static bool IsRootSightMarked(Progress progress, int actIndex, string coord)
    {
        var marked = GetCoordSet(progress.RootSightMarkedCoords);
        return marked.Contains(FormatRootSightMarkedCoord(actIndex, coord)) ||
            (actIndex == 0 && marked.Contains(coord)) ||
            TryFindRootSightPreview(progress, actIndex, coord, out _);
    }

    private static void ClearStaleRootSightPreview(
        Player player,
        int actIndex,
        string coord,
        MapPoint? point = null)
    {
        var progress = GetProgress(player);
        var previews = GetRootSightPreviews(progress.RootSightPreviewRecords)
            .Where(preview => preview.ActIndex != actIndex || preview.Coord != coord)
            .ToList();
        var marked = GetCoordSet(progress.RootSightMarkedCoords);
        marked.Remove(FormatRootSightMarkedCoord(actIndex, coord));
        if (actIndex == 0)
        {
            marked.Remove(coord);
        }

        SetProgress(player, progress with
        {
            RootSightEyes = Math.Min(RootSightStartingEyes, progress.RootSightEyes + 1),
            RootSightMarkedCoords = string.Join("|", marked),
            RootSightPreviewRecords = FormatRootSightPreviews(previews)
        });

        if (point != null)
        {
            RemoveQuestMarker<UrdaRootSightMapQuestMarker>(point);
        }

        RefreshRootSightRelicStatus(player);
        MainFile.Logger.Warn($"[EZMicroBalance] Urda Root Eyes cleared stale preview at {coord} and restored one eye.");
    }

    private static void ClearUnreachableRootSightPreviews(Player player, IRunState runState)
    {
        if (GetSelectedBlessing(player) != UrdaBlessingIds.RootSight)
        {
            return;
        }

        foreach (var preview in GetRootSightPreviews(GetProgress(player).RootSightPreviewRecords)
            .Where(preview => preview.ActIndex == runState.CurrentActIndex)
            .ToList())
        {
            if (!TryParseCoord(preview.Coord, out var coord))
            {
                ClearStaleRootSightPreview(player, runState.CurrentActIndex, preview.Coord);
                continue;
            }

            var point = runState.Map.GetPoint(coord);
            if (point == null || !IsFutureReachableRootSightTarget(runState, point))
            {
                ClearStaleRootSightPreview(player, runState.CurrentActIndex, preview.Coord, point);
            }
        }
    }
}
