namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionFeatureGate
{
    public static int DebugLevel
    {
        get
        {
            var rawValue = Environment.GetEnvironmentVariable(DebugLevelEnvironmentVariable)?.Trim();
            if (!int.TryParse(rawValue, out var level))
            {
                return 0;
            }

            return Math.Clamp(level, 0, MaxSupportedAscensionLevel);
        }
    }

    public static bool IsPublicSelectionEnabled =>
        !IsTruthy(Environment.GetEnvironmentVariable(DisablePublicSelectionEnvironmentVariable));

    public static bool IsPublicGateEnabled =>
        IsPublicSelectionEnabled;

    public static bool IsMultiplayerSelectionDisabled =>
        IsTruthy(Environment.GetEnvironmentVariable(DisableMultiplayerSelectionEnvironmentVariable));

    public static bool IsDiagnosticsEnabled =>
        IsTruthy(Environment.GetEnvironmentVariable(DiagnosticsEnvironmentVariable));

    public static bool IsMultiplayerDiagnosticsEnabled =>
        IsTruthy(Environment.GetEnvironmentVariable(MultiplayerDiagnosticsEnvironmentVariable));

    private static bool IsTruthy(string? value)
    {
        return AscensionExpansionConfig.IsTruthy(value);
    }
}
