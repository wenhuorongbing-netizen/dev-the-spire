using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    internal static bool TryGetRootSightHoverTip(MapPoint point, out HoverTip hoverTip)
    {
        if (TryFindRootSightPreviewForMapPoint(point, out var preview) &&
            TryGetRootSightPreviewTitle(preview, out var title))
        {
            hoverTip = new HoverTip(
                title,
                TryGetRootSightPreviewDescription(preview, out var description)
                    ? description
                    : new LocString("ancients", "EZMB_URDA.root_sight.map_hover.preview_description"));
            return true;
        }

        if (CanRootSightTarget(point))
        {
            hoverTip = new HoverTip(
                new LocString("ancients", "EZMB_URDA.root_sight.selection_hover.title"),
                new LocString("ancients", "EZMB_URDA.root_sight.selection_hover.description"));
            return true;
        }

        hoverTip = new HoverTip(
            new LocString("ancients", "EZMB_URDA.root_sight.map_hover.title"),
            new LocString("ancients", "EZMB_URDA.root_sight.map_hover.description"));
        return point.Quests.Any(quest => quest is UrdaRootSightMapQuestMarker);
    }

    internal static bool TryGetRootSightPreviewRoomType(MapPoint point, out RoomType roomType)
    {
        if (TryFindRootSightPreviewForMapPoint(point, out var preview))
        {
            roomType = preview.RoomType;
            return true;
        }

        roomType = RoomType.Unassigned;
        return false;
    }

    private static bool TryFindRootSightPreviewForMapPoint(MapPoint point, out RootSightPreview preview)
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState != null)
        {
            foreach (var player in runState.Players.Where(player => GetSelectedBlessing(player) == UrdaBlessingIds.RootSight))
            {
                var progress = GetProgress(player);
                if (TryFindRootSightPreview(progress, runState.CurrentActIndex, FormatCoord(point.coord), out preview))
                {
                    if (!IsFutureReachableRootSightTarget(runState, point))
                    {
                        break;
                    }

                    if (IsRootSightPreviewStillValidForEntry(runState, preview))
                    {
                        return true;
                    }

                    break;
                }
            }
        }

        preview = new RootSightPreview(0, string.Empty, MapPointType.Unassigned, RoomType.Unassigned, string.Empty);
        return false;
    }
}
