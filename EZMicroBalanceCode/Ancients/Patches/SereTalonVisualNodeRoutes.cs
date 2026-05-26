using System;
using Godot;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.Relics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static class SereTalonVisualNodeRoutes
{
    internal static void TryApplyEventOptionButton(NEventOptionButton button)
    {
        if (button.Option?.Relic is not SereTalon)
        {
            return;
        }

        var iconNode = button.GetNodeOrNull<TextureRect>("%RelicIcon");
        if (iconNode is null)
        {
            SereTalonVisualRouteLog.MissingEventButtonIconOnce();
            return;
        }

        var texture = SereTalonVisualTextures.LoadPackedTexture();
        if (texture is null)
        {
            return;
        }

        iconNode.Texture = texture;
        var outline = iconNode.GetNodeOrNull<TextureRect>("%Outline");
        if (outline is not null)
        {
            outline.Texture = texture;
        }

        iconNode.Visible = true;
        SereTalonVisualRouteLog.EventButtonRouteOnce();
    }

    internal static void TryApplyRelicNode(NRelic relicNode)
    {
        if (!relicNode.IsNodeReady() || relicNode.Icon is null || relicNode.Outline is null)
        {
            return;
        }

        RelicModel model;
        try
        {
            model = relicNode.Model;
        }
        catch (InvalidOperationException)
        {
            return;
        }

        if (model is not SereTalon)
        {
            return;
        }

        // NRelic keeps its IconSize private. After Reload(), Outline.Visible is
        // the stable Core-visible distinction between the relic bar and large
        // inspect-style node, so this fallback follows that already-applied
        // state instead of reaching into private fields.
        if (relicNode.Outline.Visible)
        {
            var texture = SereTalonVisualTextures.LoadPackedTexture();
            if (texture is null)
            {
                return;
            }

            relicNode.Icon.Texture = texture;
            relicNode.Outline.Texture = texture;
            SereTalonVisualRouteLog.RelicNodeSmallRouteOnce();
            return;
        }

        var bigTexture = SereTalonVisualTextures.LoadBigTexture();
        if (bigTexture is null)
        {
            return;
        }

        relicNode.Icon.Texture = bigTexture;
        SereTalonVisualRouteLog.RelicNodeLargeRouteOnce();
    }
}
