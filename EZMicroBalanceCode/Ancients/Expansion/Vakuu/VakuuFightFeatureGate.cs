using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static class VakuuFightFeatureGate
{
    public const string DisableEnvironmentVariable = "EZMB_DISABLE_VAKUU_FIGHT";
    public const string SpirePlusDisableEnvironmentVariable = "SPIREPLUS_DISABLE_VAKUU_FIGHT";
    public const string ForceAncientEnvironmentVariable = "EZMB_FORCE_ANCIENT";
    public const string SpirePlusForceAncientEnvironmentVariable = "SPIREPLUS_FORCE_ANCIENT";
    public const string ForceFightEnvironmentVariable = "EZMB_FORCE_VAKUU_FIGHT";
    public const string SpirePlusForceFightEnvironmentVariable = "SPIREPLUS_FORCE_VAKUU_FIGHT";

    public static string? ForcedAncient =>
        Environment.GetEnvironmentVariable(SpirePlusForceAncientEnvironmentVariable) ??
        Environment.GetEnvironmentVariable(ForceAncientEnvironmentVariable);

    public static bool ShouldForceVakuu =>
        IsForcedAncient("VAKUU") || IsForcedAncient("EZMB_VAKUU");

    public static bool ShouldForceFight =>
        IsTruthy(Environment.GetEnvironmentVariable(ForceFightEnvironmentVariable)) ||
        IsTruthy(Environment.GetEnvironmentVariable(SpirePlusForceFightEnvironmentVariable));

    public static bool IsFightEnabled(UnlockState _) =>
        !IsTruthy(Environment.GetEnvironmentVariable(DisableEnvironmentVariable)) &&
        !IsTruthy(Environment.GetEnvironmentVariable(SpirePlusDisableEnvironmentVariable));

    public static bool IsFightEnabledForRun(IRunState runState) =>
        IsFightEnabled(runState.UnlockState) && runState.Players.Count == 1;

    private static bool IsForcedAncient(string value) =>
        string.Equals(ForcedAncient?.Trim(), value, StringComparison.OrdinalIgnoreCase);

    private static bool IsTruthy(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        (value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
         value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
         value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
         value.Equals("on", StringComparison.OrdinalIgnoreCase));
}
