namespace EZMicroBalance.EZMicroBalanceCode.Diagnostics;

internal static class SpirePlusDebug
{
    public const string DebugLogsEnvironmentVariable = "SPIREPLUS_ENABLE_DEBUG_LOGS";
    public const string LegacyDebugLogsEnvironmentVariable = "EZMB_ENABLE_DEBUG_LOGS";

    public static void Log(string category, string message)
    {
        if (IsDebugLoggingEnabled)
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

    private static bool IsDebugLoggingEnabled =>
        IsTruthyEnvironmentVariable(DebugLogsEnvironmentVariable) ||
        IsTruthyEnvironmentVariable(LegacyDebugLogsEnvironmentVariable);

    private static bool IsTruthyEnvironmentVariable(string name) =>
        IsTruthy(Environment.GetEnvironmentVariable(name));

    private static bool IsTruthy(string? value)
    {
        var normalized = value?.Trim();
        return !string.IsNullOrWhiteSpace(normalized) &&
            !string.Equals(normalized, "0", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(normalized, "false", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(normalized, "off", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(normalized, "no", StringComparison.OrdinalIgnoreCase);
    }
}
