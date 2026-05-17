using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static class MorviFeatureGate
{
    public const string DisableEnvironmentVariable = "EZMB_DISABLE_MORVI";
    public const string SpirePlusDisableEnvironmentVariable = "SPIREPLUS_DISABLE_MORVI";
    public const string ForceAncientEnvironmentVariable = "EZMB_FORCE_ANCIENT";
    public const string SpirePlusForceAncientEnvironmentVariable = "SPIREPLUS_FORCE_ANCIENT";
    public const string ForceBlessingEnvironmentVariable = "EZMB_FORCE_MORVI_BLESSING";
    public const string SpirePlusForceBlessingEnvironmentVariable = "SPIREPLUS_FORCE_MORVI_BLESSING";

    public static string? ForcedAncient =>
        AncientFeatureGate.FirstRawEnvironmentValue(SpirePlusForceAncientEnvironmentVariable, ForceAncientEnvironmentVariable);

    public static string? ForcedBlessing =>
        AncientFeatureGate.FirstRawEnvironmentValue(SpirePlusForceBlessingEnvironmentVariable, ForceBlessingEnvironmentVariable);

    public static bool ShouldForceMorvi =>
        IsForcedAncient("MORVI") || IsForcedAncient("EZMB_MORVI");

    public static bool IsMorviEnabled(UnlockState _) =>
        !AncientFeatureGate.IsTruthyEnvironmentVariable(DisableEnvironmentVariable) &&
        !AncientFeatureGate.IsTruthyEnvironmentVariable(SpirePlusDisableEnvironmentVariable);

    private static bool IsForcedAncient(string value) =>
        AncientFeatureGate.IsForcedAncient(ForcedAncient, value);
}
