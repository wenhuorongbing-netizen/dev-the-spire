using System;

using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static class UrdaFeatureGate
{
    public const string ForceAncientEnvironmentVariable = "EZMB_FORCE_ANCIENT";
    public const string ForceAncientAliasEnvironmentVariable = "SPIREPLUS_FORCE_ANCIENT";
    public const string DisableAncientEnvironmentVariable = "EZMB_DISABLE_URDA";
    public const string DisableAncientAliasEnvironmentVariable = "SPIREPLUS_DISABLE_URDA";
    public const string ForceBlessingEnvironmentVariable = "EZMB_FORCE_URDA_BLESSING";
    public const string ForceBlessingAliasEnvironmentVariable = "SPIREPLUS_FORCE_URDA_BLESSING";

    public static string? ForcedAncient =>
        FirstEnvironmentValue(ForceAncientEnvironmentVariable, ForceAncientAliasEnvironmentVariable);

    public static string? ForcedBlessing =>
        FirstEnvironmentValue(ForceBlessingEnvironmentVariable, ForceBlessingAliasEnvironmentVariable);

    public static bool ShouldForceUrda =>
        ForcedAncient is { Length: > 0 } forcedAncient &&
        (string.Equals(forcedAncient, "URDA", StringComparison.OrdinalIgnoreCase) ||
         string.Equals(forcedAncient, "EZMB_URDA", StringComparison.OrdinalIgnoreCase));

    public static bool IsUrdaEnabled(UnlockState _unlockState) =>
        !IsTruthy(Environment.GetEnvironmentVariable(DisableAncientEnvironmentVariable)) &&
        !IsTruthy(Environment.GetEnvironmentVariable(DisableAncientAliasEnvironmentVariable));

    private static string? FirstEnvironmentValue(params string[] names)
    {
        foreach (var name in names)
        {
            var value = Environment.GetEnvironmentVariable(name)?.Trim();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    private static bool IsTruthy(string? value)
    {
        var normalized = value?.Trim();
        return string.Equals(normalized, "1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalized, "true", StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalized, "yes", StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalized, "on", StringComparison.OrdinalIgnoreCase);
    }
}
