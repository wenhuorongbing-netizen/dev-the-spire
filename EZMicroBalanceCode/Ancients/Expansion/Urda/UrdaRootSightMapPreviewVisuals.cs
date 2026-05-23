using Godot;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static class UrdaRootSightMapPreviewVisuals
{
    private const string RootSightOverlayIconName = "EZMBRootSightOverlayIcon";

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

        var canTargetWithRootSight = UrdaBlessingService.CanRootSightTarget(pointNode.Point);
        var hasRootSightMarker = pointNode.Point.Quests.Any(quest => quest is UrdaRootSightMapQuestMarker);
        var hasOtherMarker = pointNode.Point.Quests.Any(quest => quest is not UrdaRootSightMapQuestMarker);
        if (hasOtherMarker)
        {
            ApplyRootSightOverlay(pointNode, hasRootSightMarker || canTargetWithRootSight);
            return;
        }

        ApplyRootSightOverlay(pointNode, visible: false);
        if (!hasRootSightMarker && !canTargetWithRootSight)
        {
            if (pointNode.Point.Quests.Count == 0)
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

    private static void ApplyRootSightOverlay(NNormalMapPoint pointNode, bool visible)
    {
        var iconContainer = pointNode.GetNodeOrNull<Control>("%IconContainer");
        if (iconContainer == null)
        {
            return;
        }

        var overlay = iconContainer.GetNodeOrNull<TextureRect>(RootSightOverlayIconName);
        if (!visible)
        {
            if (overlay != null)
            {
                overlay.Visible = false;
            }

            return;
        }

        if (overlay == null)
        {
            overlay = new TextureRect
            {
                Name = RootSightOverlayIconName,
                MouseFilter = Control.MouseFilterEnum.Ignore,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                CustomMinimumSize = new Vector2(22f, 22f),
                Size = new Vector2(22f, 22f),
                Position = new Vector2(34f, 30f),
                PivotOffset = new Vector2(11f, 11f),
                ZIndex = 20
            };
            iconContainer.AddChildSafely(overlay);
        }

        overlay.Texture = ResourceLoader.Load<Texture2D>(
            UrdaAssetPaths.RootSightOptionIcon,
            null,
            ResourceLoader.CacheMode.Reuse);
        overlay.Visible = true;
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
