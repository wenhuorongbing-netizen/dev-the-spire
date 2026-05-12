using System;

using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static class UrdaFeatureGate
{
    public const string ForceAncientEnvironmentVariable = "EZMB_FORCE_ANCIENT";
    public const string DisableAncientEnvironmentVariable = "EZMB_DISABLE_URDA";
    public const string ForceBlessingEnvironmentVariable = "EZMB_FORCE_URDA_BLESSING";

    public static string? ForcedBlessing =>
        Environment.GetEnvironmentVariable(ForceBlessingEnvironmentVariable)?.Trim();

    public static bool IsUrdaEnabled(UnlockState _unlockState) =>
        !IsTruthy(Environment.GetEnvironmentVariable(DisableAncientEnvironmentVariable));

    private static bool IsTruthy(string? value)
    {
        var normalized = value?.Trim();
        return string.Equals(normalized, "1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalized, "true", StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalized, "yes", StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalized, "on", StringComparison.OrdinalIgnoreCase);
    }
}
