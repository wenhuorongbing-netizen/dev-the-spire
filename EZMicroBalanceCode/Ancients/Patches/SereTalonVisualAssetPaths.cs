using System;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.Relics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static class SereTalonVisualAssetPaths
{
    internal const string PackedIcon = MainFile.ResPath + "/images/relics/sere_talon_spire_plus.png";
    internal const string BigIcon = MainFile.ResPath + "/images/relics/big/sere_talon_spire_plus.png";

    private static bool _loggedPackedIconPathRoute;
    private static bool _loggedBigIconPathRoute;
    private static bool _loggedPackedTextureRoute;
    private static bool _loggedBigTextureRoute;
    private static bool _loggedEventButtonRoute;
    private static bool _loggedRelicNodeSmallRoute;
    private static bool _loggedRelicNodeLargeRoute;
    private static bool _loggedMissingPackedIcon;
    private static bool _loggedMissingBigIcon;
    private static bool _loggedMissingEventButtonIcon;

    internal static bool TryApply(RelicModel relic, string iconPath, ref string result)
    {
        if (relic is not SereTalon)
        {
            return false;
        }

        if (!CanUsePath(iconPath))
        {
            return false;
        }

        result = iconPath;
        if (iconPath == BigIcon)
        {
            LogBigIconPathRouteOnce();
        }
        else
        {
            LogPackedIconPathRouteOnce();
        }

        return true;
    }

    internal static bool TryApplyTexture(RelicModel relic, string iconPath, ref Texture2D result)
    {
        if (relic is not SereTalon)
        {
            return false;
        }

        if (!CanUsePath(iconPath))
        {
            return false;
        }

        var texture = PreloadManager.Cache.GetTexture2D(iconPath);
        if (texture is null)
        {
            LogSkippedPathOnce(iconPath, "texture did not load");
            return false;
        }

        result = texture;
        LogBigTextureRouteOnce();
        return true;
    }

    internal static bool TryApplyPackedTexture(RelicModel relic, ref Texture2D result)
    {
        if (relic is not SereTalon)
        {
            return false;
        }

        var texture = LoadPackedTexture();
        if (texture is null)
        {
            return false;
        }

        result = texture;
        LogPackedTextureRouteOnce();
        return true;
    }

    internal static void TryApplyEventOptionButton(NEventOptionButton button)
    {
        if (button.Option?.Relic is not SereTalon)
        {
            return;
        }

        var iconNode = button.GetNodeOrNull<TextureRect>("%RelicIcon");
        if (iconNode is null)
        {
            LogMissingEventButtonIconOnce();
            return;
        }

        var texture = LoadPackedTexture();
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
        LogEventButtonRouteOnce();
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
            var texture = LoadPackedTexture();
            if (texture is null)
            {
                return;
            }

            relicNode.Icon.Texture = texture;
            relicNode.Outline.Texture = texture;
            LogRelicNodeSmallRouteOnce();
            return;
        }

        var bigTexture = LoadBigTexture();
        if (bigTexture is null)
        {
            return;
        }

        relicNode.Icon.Texture = bigTexture;
        LogRelicNodeLargeRouteOnce();
    }

    private static Texture2D? LoadPackedTexture()
    {
        if (!CanUsePath(PackedIcon))
        {
            return null;
        }

        var texture = ResourceLoader.Load<Texture2D>(PackedIcon, null, ResourceLoader.CacheMode.Reuse);
        if (texture is null)
        {
            LogSkippedPathOnce(PackedIcon, "texture did not load");
        }

        return texture;
    }

    private static Texture2D? LoadBigTexture()
    {
        if (!CanUsePath(BigIcon))
        {
            return null;
        }

        var texture = PreloadManager.Cache.GetTexture2D(BigIcon);
        if (texture is null)
        {
            LogSkippedPathOnce(BigIcon, "texture did not load");
        }

        return texture;
    }

    private static bool CanUsePath(string iconPath)
    {
        if (ResourceLoader.Exists(iconPath))
        {
            return true;
        }

        LogSkippedPathOnce(iconPath, "resource path does not exist");
        return false;
    }

    private static void LogSkippedPathOnce(string iconPath, string reason)
    {
        if (iconPath == PackedIcon)
        {
            if (_loggedMissingPackedIcon)
            {
                return;
            }

            _loggedMissingPackedIcon = true;
        }
        else if (iconPath == BigIcon)
        {
            if (_loggedMissingBigIcon)
            {
                return;
            }

            _loggedMissingBigIcon = true;
        }

        MainFile.Logger.Warn($"[Spire Plus] Vakuu Sere Talon icon route skipped because {reason}: {iconPath}");
    }

    private static void LogMissingEventButtonIconOnce()
    {
        if (_loggedMissingEventButtonIcon)
        {
            return;
        }

        _loggedMissingEventButtonIcon = true;
        MainFile.Logger.Warn("[Spire Plus] Vakuu Sere Talon Ancient option icon route skipped because %RelicIcon was not found.");
    }

    private static void LogPackedIconPathRouteOnce()
    {
        if (_loggedPackedIconPathRoute)
        {
            return;
        }

        _loggedPackedIconPathRoute = true;
        LogRoute("RelicModel packed icon path");
    }

    private static void LogBigIconPathRouteOnce()
    {
        if (_loggedBigIconPathRoute)
        {
            return;
        }

        _loggedBigIconPathRoute = true;
        LogRoute("RelicModel big icon path");
    }

    private static void LogPackedTextureRouteOnce()
    {
        if (_loggedPackedTextureRoute)
        {
            return;
        }

        _loggedPackedTextureRoute = true;
        LogRoute("RelicModel packed icon texture");
    }

    private static void LogBigTextureRouteOnce()
    {
        if (_loggedBigTextureRoute)
        {
            return;
        }

        _loggedBigTextureRoute = true;
        LogRoute("RelicModel big icon texture");
    }

    private static void LogEventButtonRouteOnce()
    {
        if (_loggedEventButtonRoute)
        {
            return;
        }

        _loggedEventButtonRoute = true;
        LogRoute("Ancient event option button");
    }

    private static void LogRelicNodeSmallRouteOnce()
    {
        if (_loggedRelicNodeSmallRoute)
        {
            return;
        }

        _loggedRelicNodeSmallRoute = true;
        LogRoute("NRelic small node");
    }

    private static void LogRelicNodeLargeRouteOnce()
    {
        if (_loggedRelicNodeLargeRoute)
        {
            return;
        }

        _loggedRelicNodeLargeRoute = true;
        LogRoute("NRelic large node");
    }

    private static void LogRoute(string surface)
    {
        MainFile.Logger.Info($"[Spire Plus] Vakuu Sere Talon icon route active on {surface}; SereTalon uses Spire Plus art and Tanx Claws is untouched.");
    }
}
