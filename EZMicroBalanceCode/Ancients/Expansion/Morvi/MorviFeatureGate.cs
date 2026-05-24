using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static class MorviFeatureGate
{
    public const string DisableEnvironmentVariable = "SPIREPLUS_DISABLE_MORVI";
    public const string LegacyDisableEnvironmentVariable = "EZMB_DISABLE_MORVI";
    public const string ForceAncientEnvironmentVariable = "SPIREPLUS_FORCE_ANCIENT";
    public const string LegacyForceAncientEnvironmentVariable = "EZMB_FORCE_ANCIENT";
    public const string ForceBlessingEnvironmentVariable = "SPIREPLUS_FORCE_MORVI_BLESSING";
    public const string LegacyForceBlessingEnvironmentVariable = "EZMB_FORCE_MORVI_BLESSING";

    public static string? ForcedAncient =>
        AncientFeatureGate.FirstRawEnvironmentValue(ForceAncientEnvironmentVariable, LegacyForceAncientEnvironmentVariable);

    public static string? ForcedBlessing =>
        AncientFeatureGate.FirstRawEnvironmentValue(ForceBlessingEnvironmentVariable, LegacyForceBlessingEnvironmentVariable);

    public static bool ShouldForceMorvi =>
        IsForcedAncient("MORVI") || IsForcedAncient("EZMB_MORVI");

    public static bool IsMorviEnabled(UnlockState _) =>
        !AncientFeatureGate.IsTruthyEnvironmentVariable(DisableEnvironmentVariable) &&
        !AncientFeatureGate.IsTruthyEnvironmentVariable(LegacyDisableEnvironmentVariable);

    private static bool IsForcedAncient(string value) =>
        AncientFeatureGate.IsForcedAncient(ForcedAncient, value);
}
