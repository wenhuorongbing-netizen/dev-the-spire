namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static class SereTalonVisualRouteLog
{
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

    internal static void SkippedPathOnce(string iconPath, string reason)
    {
        if (iconPath == SereTalonVisualAssetPaths.PackedIcon)
        {
            if (_loggedMissingPackedIcon)
            {
                return;
            }

            _loggedMissingPackedIcon = true;
        }
        else if (iconPath == SereTalonVisualAssetPaths.BigIcon)
        {
            if (_loggedMissingBigIcon)
            {
                return;
            }

            _loggedMissingBigIcon = true;
        }

        MainFile.Logger.Warn($"[Spire Plus] Vakuu Sere Talon icon route skipped because {reason}: {iconPath}");
    }

    internal static void MissingEventButtonIconOnce()
    {
        if (_loggedMissingEventButtonIcon)
        {
            return;
        }

        _loggedMissingEventButtonIcon = true;
        MainFile.Logger.Warn("[Spire Plus] Vakuu Sere Talon Ancient option icon route skipped because %RelicIcon was not found.");
    }

    internal static void PackedIconPathRouteOnce()
    {
        if (_loggedPackedIconPathRoute)
        {
            return;
        }

        _loggedPackedIconPathRoute = true;
        Route("RelicModel packed icon path");
    }

    internal static void BigIconPathRouteOnce()
    {
        if (_loggedBigIconPathRoute)
        {
            return;
        }

        _loggedBigIconPathRoute = true;
        Route("RelicModel big icon path");
    }

    internal static void PackedTextureRouteOnce()
    {
        if (_loggedPackedTextureRoute)
        {
            return;
        }

        _loggedPackedTextureRoute = true;
        Route("RelicModel packed icon texture");
    }

    internal static void BigTextureRouteOnce()
    {
        if (_loggedBigTextureRoute)
        {
            return;
        }

        _loggedBigTextureRoute = true;
        Route("RelicModel big icon texture");
    }

    internal static void EventButtonRouteOnce()
    {
        if (_loggedEventButtonRoute)
        {
            return;
        }

        _loggedEventButtonRoute = true;
        Route("Ancient event option button");
    }

    internal static void RelicNodeSmallRouteOnce()
    {
        if (_loggedRelicNodeSmallRoute)
        {
            return;
        }

        _loggedRelicNodeSmallRoute = true;
        Route("NRelic small node");
    }

    internal static void RelicNodeLargeRouteOnce()
    {
        if (_loggedRelicNodeLargeRoute)
        {
            return;
        }

        _loggedRelicNodeLargeRoute = true;
        Route("NRelic large node");
    }

    private static void Route(string surface)
    {
        MainFile.Logger.Info($"[Spire Plus] Vakuu Sere Talon icon route active on {surface}; SereTalon uses Spire Plus art and Tanx Claws is untouched.");
    }
}
