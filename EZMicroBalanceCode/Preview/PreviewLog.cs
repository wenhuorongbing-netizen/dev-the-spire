using EZMicroBalance.EZMicroBalanceCode.Config;

namespace EZMicroBalance.EZMicroBalanceCode.Preview;

internal static class PreviewLog
{
    public static void Debug(string message)
    {
        if (EZMicroBalanceModConfig.ShowPreviewDebugLogs)
        {
            MainFile.Logger.Info("[Spire Plus] Preview: " + message);
        }
    }

    public static void Warn(string message)
    {
        MainFile.Logger.Warn("[Spire Plus] Preview: " + message);
    }
}
