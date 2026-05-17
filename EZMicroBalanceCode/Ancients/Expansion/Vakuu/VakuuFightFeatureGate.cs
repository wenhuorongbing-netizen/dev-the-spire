using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static class VakuuFightFeatureGate
{
    public const string EnableEnvironmentVariable = "EZMB_ENABLE_VAKUU_FIGHT";
    public const string SpirePlusEnableEnvironmentVariable = "SPIREPLUS_ENABLE_VAKUU_FIGHT";
    public const string DisableEnvironmentVariable = "EZMB_DISABLE_VAKUU_FIGHT";
    public const string SpirePlusDisableEnvironmentVariable = "SPIREPLUS_DISABLE_VAKUU_FIGHT";
    public const string ForceAncientEnvironmentVariable = "EZMB_FORCE_ANCIENT";
    public const string SpirePlusForceAncientEnvironmentVariable = "SPIREPLUS_FORCE_ANCIENT";
    public const string ForceFightEnvironmentVariable = "EZMB_FORCE_VAKUU_FIGHT";
    public const string SpirePlusForceFightEnvironmentVariable = "SPIREPLUS_FORCE_VAKUU_FIGHT";

    public static string? ForcedAncient =>
        AncientFeatureGate.FirstRawEnvironmentValue(SpirePlusForceAncientEnvironmentVariable, ForceAncientEnvironmentVariable);

    public static bool ShouldForceVakuu =>
        IsForcedAncient("VAKUU") || IsForcedAncient("EZMB_VAKUU");

    public static bool ShouldForceFight =>
        AncientFeatureGate.IsTruthyEnvironmentVariable(ForceFightEnvironmentVariable) ||
        AncientFeatureGate.IsTruthyEnvironmentVariable(SpirePlusForceFightEnvironmentVariable);

    public static bool ShouldEnableFight =>
        ShouldForceFight ||
        AncientFeatureGate.IsTruthyEnvironmentVariable(EnableEnvironmentVariable) ||
        AncientFeatureGate.IsTruthyEnvironmentVariable(SpirePlusEnableEnvironmentVariable);

    public static bool IsFightEnabled(UnlockState _) =>
        ShouldEnableFight &&
        !AncientFeatureGate.IsTruthyEnvironmentVariable(DisableEnvironmentVariable) &&
        !AncientFeatureGate.IsTruthyEnvironmentVariable(SpirePlusDisableEnvironmentVariable);

    public static bool IsFightEnabledForRun(IRunState runState) =>
        IsFightEnabled(runState.UnlockState) && runState.Players.Count == 1;

    private static bool IsForcedAncient(string value) =>
        AncientFeatureGate.IsForcedAncient(ForcedAncient, value);
}
