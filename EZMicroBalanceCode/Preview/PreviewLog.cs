using EZMicroBalance.EZMicroBalanceCode.Config;

namespace EZMicroBalance.EZMicroBalanceCode.Preview;

internal static class PreviewLog
{
    public static void Debug(string message)
    {
        if (EZMicroBalanceModConfig.ShowPreviewDebugLogs)
        {
            MainFile.Logger.Info("[EZMicroBalance] Preview: " + message);
        }
    }

    public static void Warn(string message)
    {
        MainFile.Logger.Warn("[EZMicroBalance] Preview: " + message);
    }
}
