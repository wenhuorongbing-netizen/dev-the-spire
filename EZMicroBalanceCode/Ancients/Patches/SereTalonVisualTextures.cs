using Godot;
using MegaCrit.Sts2.Core.Assets;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static class SereTalonVisualTextures
{
    internal static Texture2D? LoadPackedTexture()
    {
        if (!CanUsePath(SereTalonVisualAssetPaths.PackedIcon))
        {
            return null;
        }

        var texture = ResourceLoader.Load<Texture2D>(SereTalonVisualAssetPaths.PackedIcon, null, ResourceLoader.CacheMode.Reuse);
        if (texture is null)
        {
            SereTalonVisualRouteLog.SkippedPathOnce(SereTalonVisualAssetPaths.PackedIcon, "texture did not load");
        }

        return texture;
    }

    internal static Texture2D? LoadBigTexture()
    {
        if (!CanUsePath(SereTalonVisualAssetPaths.BigIcon))
        {
            return null;
        }

        var texture = PreloadManager.Cache.GetTexture2D(SereTalonVisualAssetPaths.BigIcon);
        if (texture is null)
        {
            SereTalonVisualRouteLog.SkippedPathOnce(SereTalonVisualAssetPaths.BigIcon, "texture did not load");
        }

        return texture;
    }

    internal static bool CanUsePath(string iconPath)
    {
        if (ResourceLoader.Exists(iconPath))
        {
            return true;
        }

        SereTalonVisualRouteLog.SkippedPathOnce(iconPath, "resource path does not exist");
        return false;
    }
}
