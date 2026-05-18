using EZFuturePeek.EZFuturePeekCode.Config;

namespace EZFuturePeek.EZFuturePeekCode.Diagnostics;

internal static class FuturePeekLog
{
    public static void Debug(string message)
    {
        if (EZFuturePeekConfig.ShowDebugLogs)
        {
            MainFile.Logger.Info("[EZFuturePeek] " + message);
        }
    }

    public static void Warn(string message)
    {
        MainFile.Logger.Warn("[EZFuturePeek] " + message);
    }
}
