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
            hoverTip = new HoverTip(title, new LocString("ancients", "EZMB_URDA.root_sight.map_hover.preview_description"));
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
                    if (IsRootSightPreviewStillValidForEntry(runState, preview))
                    {
                        return true;
                    }

                    ClearStaleRootSightPreview(player, runState.CurrentActIndex, preview.Coord, point);
                    break;
                }
            }
        }

        preview = new RootSightPreview(0, string.Empty, MapPointType.Unassigned, RoomType.Unassigned, string.Empty);
        return false;
    }

    private static bool TryGetRootSightPreviewTitle(RootSightPreview preview, out LocString title)
    {
        title = new LocString("ancients", "EZMB_URDA.root_sight.map_hover.title");
        try
        {
            var id = ModelId.Deserialize(preview.ModelId);
            if (preview.RoomType == RoomType.Event)
            {
                var eventModel = ModelDb.GetByIdOrNull<EventModel>(id);
                if (eventModel == null)
                {
                    return false;
                }

                title = eventModel.Title;
                return true;
            }

            var encounter = ModelDb.GetByIdOrNull<EncounterModel>(id);
            if (encounter == null)
            {
                return false;
            }

            title = encounter.Title;
            return true;
        }
        catch
        {
            return false;
        }
    }
}
