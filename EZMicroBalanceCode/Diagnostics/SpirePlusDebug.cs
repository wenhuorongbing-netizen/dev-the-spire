using EZMicroBalance.EZMicroBalanceCode.Config;

namespace EZMicroBalance.EZMicroBalanceCode.Diagnostics;

internal static class SpirePlusDebug
{
    public static void Log(string category, string message)
    {
        if (SpirePlusModConfig.EnableDebugLogs)
        {
            MainFile.Logger.Info($"[Spire Plus] [{category}] {message}");
        }
    }

    public static void LogAncient(string ancient, string message)
    {
        Log($"Ancient.{ancient}", message);
    }

    public static void LogAscension(string message)
    {
        Log("Ascension", message);
    }

    public static void LogPreview(string message)
    {
        if (SpirePlusModConfig.ShowPreviewDebugLogs)
        {
            Log("Preview", message);
        }
    }

    public static void LogPatch(string patchName, string message)
    {
        Log($"Patch.{patchName}", message);
    }

    public static void LogMultiplayer(string message)
    {
        Log("Multiplayer", message);
    }

    public static void Warn(string category, string message)
    {
        MainFile.Logger.Warn($"[Spire Plus] [{category}] {message}");
    }
}
