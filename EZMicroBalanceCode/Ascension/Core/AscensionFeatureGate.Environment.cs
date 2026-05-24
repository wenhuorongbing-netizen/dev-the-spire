namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionFeatureGate
{
    public static int DebugLevel
    {
        get
        {
            var rawValue = FirstRawEnvironmentValue(
                DebugLevelEnvironmentVariable,
                LegacyDebugLevelEnvironmentVariable)?.Trim();
            if (!int.TryParse(rawValue, out var level))
            {
                return 0;
            }

            return Math.Clamp(level, 0, MaxSupportedAscensionLevel);
        }
    }

    public static bool IsPublicSelectionEnabled =>
        !IsTruthyEnvironmentVariable(
            DisablePublicSelectionEnvironmentVariable,
            LegacyDisablePublicSelectionEnvironmentVariable);

    public static bool IsPublicGateEnabled =>
        IsPublicSelectionEnabled;

    public static bool IsMultiplayerSelectionDisabled =>
        IsTruthyEnvironmentVariable(
            DisableMultiplayerSelectionEnvironmentVariable,
            LegacyDisableMultiplayerSelectionEnvironmentVariable);

    public static bool IsDiagnosticsEnabled =>
        IsTruthyEnvironmentVariable(
            DiagnosticsEnvironmentVariable,
            LegacyDiagnosticsEnvironmentVariable);

    public static bool IsMultiplayerDiagnosticsEnabled =>
        IsTruthyEnvironmentVariable(
            MultiplayerDiagnosticsEnvironmentVariable,
            LegacyMultiplayerDiagnosticsEnvironmentVariable);

    private static string? FirstRawEnvironmentValue(params string[] names)
    {
        foreach (var name in names)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    private static bool IsTruthyEnvironmentVariable(params string[] names) =>
        names.Any(name => IsTruthy(Environment.GetEnvironmentVariable(name)));

    private static bool IsTruthy(string? value)
    {
        return AscensionExpansionConfig.IsTruthy(value);
    }
}
