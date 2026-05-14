using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static class LothaFeatureGate
{
    public const string DisableEnvironmentVariable = "EZMB_DISABLE_LOTHA";
    public const string SpirePlusDisableEnvironmentVariable = "SPIREPLUS_DISABLE_LOTHA";
    public const string ForceAncientEnvironmentVariable = "EZMB_FORCE_ANCIENT";
    public const string SpirePlusForceAncientEnvironmentVariable = "SPIREPLUS_FORCE_ANCIENT";
    public const string ForceBlessingEnvironmentVariable = "EZMB_FORCE_LOTHA_BLESSING";
    public const string SpirePlusForceBlessingEnvironmentVariable = "SPIREPLUS_FORCE_LOTHA_BLESSING";

    public static string? ForcedAncient =>
        Environment.GetEnvironmentVariable(SpirePlusForceAncientEnvironmentVariable) ??
        Environment.GetEnvironmentVariable(ForceAncientEnvironmentVariable);

    public static string? ForcedBlessing =>
        Environment.GetEnvironmentVariable(SpirePlusForceBlessingEnvironmentVariable) ??
        Environment.GetEnvironmentVariable(ForceBlessingEnvironmentVariable);

    public static bool ShouldForceLotha =>
        IsForcedAncient("LOTHA") || IsForcedAncient("EZMB_LOTHA");

    public static bool IsLothaEnabled(UnlockState _) =>
        !IsTruthy(Environment.GetEnvironmentVariable(DisableEnvironmentVariable)) &&
        !IsTruthy(Environment.GetEnvironmentVariable(SpirePlusDisableEnvironmentVariable));

    private static bool IsForcedAncient(string value) =>
        string.Equals(ForcedAncient?.Trim(), value, StringComparison.OrdinalIgnoreCase);

    private static bool IsTruthy(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        (value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
         value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
         value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
         value.Equals("on", StringComparison.OrdinalIgnoreCase));
}
