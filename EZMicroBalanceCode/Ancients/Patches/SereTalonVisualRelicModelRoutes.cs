using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Models.Relics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static class SereTalonVisualRelicModelRoutes
{
    internal static bool TryApplyPath(RelicModel relic, string iconPath, ref string result)
    {
        if (relic is not SereTalon)
        {
            return false;
        }

        if (!SereTalonVisualTextures.CanUsePath(iconPath))
        {
            return false;
        }

        result = iconPath;
        if (iconPath == SereTalonVisualAssetPaths.BigIcon)
        {
            SereTalonVisualRouteLog.BigIconPathRouteOnce();
        }
        else
        {
            SereTalonVisualRouteLog.PackedIconPathRouteOnce();
        }

        return true;
    }

    internal static bool TryApplyTexture(RelicModel relic, string iconPath, ref Texture2D result)
    {
        if (relic is not SereTalon)
        {
            return false;
        }

        if (!SereTalonVisualTextures.CanUsePath(iconPath))
        {
            return false;
        }

        var texture = PreloadManager.Cache.GetTexture2D(iconPath);
        if (texture is null)
        {
            SereTalonVisualRouteLog.SkippedPathOnce(iconPath, "texture did not load");
            return false;
        }

        result = texture;
        SereTalonVisualRouteLog.BigTextureRouteOnce();
        return true;
    }

    internal static bool TryApplyPackedTexture(RelicModel relic, ref Texture2D result)
    {
        if (relic is not SereTalon)
        {
            return false;
        }

        var texture = SereTalonVisualTextures.LoadPackedTexture();
        if (texture is null)
        {
            return false;
        }

        result = texture;
        SereTalonVisualRouteLog.PackedTextureRouteOnce();
        return true;
    }
}
