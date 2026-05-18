using Godot;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static class UrdaRootSightMapPreviewVisuals
{
    internal static void ApplyPreviewIcon(NNormalMapPoint pointNode)
    {
        if (pointNode.Point.PointType != MapPointType.Unknown ||
            !UrdaBlessingService.TryGetRootSightPreviewRoomType(pointNode.Point, out var roomType))
        {
            return;
        }

        var icon = pointNode.GetNodeOrNull<TextureRect>("%Icon");
        var outline = pointNode.GetNodeOrNull<TextureRect>("%Outline");
        if (icon != null)
        {
            icon.Texture = ResourceLoader.Load<Texture2D>(
                UnknownIconPath(roomType),
                null,
                ResourceLoader.CacheMode.Reuse);
        }

        if (outline != null)
        {
            outline.Texture = ResourceLoader.Load<Texture2D>(
                UnknownOutlinePath(roomType),
                null,
                ResourceLoader.CacheMode.Reuse);
        }
    }

    internal static void ApplyQuestIcon(NNormalMapPoint pointNode)
    {
        var questIcon = pointNode.GetNodeOrNull<TextureRect>("%QuestIcon");
        if (questIcon == null)
        {
            return;
        }

        var hasRootSightMarker = pointNode.Point.Quests.Any(quest => quest is UrdaRootSightMapQuestMarker);
        var hasOtherMarker = pointNode.Point.Quests.Any(quest => quest is not UrdaRootSightMapQuestMarker);
        if ((!hasRootSightMarker && !UrdaBlessingService.CanRootSightTarget(pointNode.Point)) ||
            hasOtherMarker)
        {
            if (!hasRootSightMarker && pointNode.Point.Quests.Count == 0)
            {
                questIcon.Visible = false;
            }

            return;
        }

        questIcon.Visible = true;
        questIcon.Texture = ResourceLoader.Load<Texture2D>(
            UrdaAssetPaths.RootSightOptionIcon,
            null,
            ResourceLoader.CacheMode.Reuse);
    }

    private static string UnknownIconPath(RoomType roomType)
    {
        var iconName = roomType switch
        {
            RoomType.Monster => "unknown_monster",
            RoomType.Elite => "unknown_elite",
            RoomType.Treasure => "unknown_chest",
            RoomType.Shop => "unknown_shop",
            _ => "unknown"
        };

        return ImageHelper.GetImagePath($"atlases/ui_atlas.sprites/map/icons/map_{iconName}.tres");
    }

    private static string UnknownOutlinePath(RoomType roomType)
    {
        var outlineName = roomType switch
        {
            RoomType.Monster => "map_monster",
            RoomType.Elite => "map_elite",
            RoomType.Treasure => "map_chest",
            RoomType.Shop => "map_shop",
            _ => "map_unknown"
        };

        return ImageHelper.GetImagePath($"atlases/compressed.sprites/map/{outlineName}_outline.tres");
    }
}
