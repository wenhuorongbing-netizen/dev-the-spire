using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static class UrdaFeatureGate
{
    public const string ForceAncientEnvironmentVariable = "SPIREPLUS_FORCE_ANCIENT";
    public const string LegacyForceAncientEnvironmentVariable = "EZMB_FORCE_ANCIENT";
    public const string DisableAncientEnvironmentVariable = "SPIREPLUS_DISABLE_URDA";
    public const string LegacyDisableAncientEnvironmentVariable = "EZMB_DISABLE_URDA";
    public const string ForceBlessingEnvironmentVariable = "SPIREPLUS_FORCE_URDA_BLESSING";
    public const string LegacyForceBlessingEnvironmentVariable = "EZMB_FORCE_URDA_BLESSING";

    public static string? ForcedAncient =>
        AncientFeatureGate.FirstNonBlankEnvironmentValue(ForceAncientEnvironmentVariable, LegacyForceAncientEnvironmentVariable);

    public static string? ForcedBlessing =>
        AncientFeatureGate.FirstNonBlankEnvironmentValue(ForceBlessingEnvironmentVariable, LegacyForceBlessingEnvironmentVariable);

    public static bool ShouldForceUrda =>
        ForcedAncient is { Length: > 0 } forcedAncient &&
        (string.Equals(forcedAncient, "URDA", StringComparison.OrdinalIgnoreCase) ||
         string.Equals(forcedAncient, "EZMB_URDA", StringComparison.OrdinalIgnoreCase));

    public static bool IsUrdaEnabled(UnlockState _unlockState) =>
        !AncientFeatureGate.IsTruthyEnvironmentVariable(DisableAncientEnvironmentVariable, trimValue: true) &&
        !AncientFeatureGate.IsTruthyEnvironmentVariable(LegacyDisableAncientEnvironmentVariable, trimValue: true);
}
