using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
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
        AncientFeatureGate.FirstNonBlankEnvironmentValue(ForceAncientEnvironmentVariable, ForceAncientAliasEnvironmentVariable);

    public static string? ForcedBlessing =>
        AncientFeatureGate.FirstNonBlankEnvironmentValue(ForceBlessingEnvironmentVariable, ForceBlessingAliasEnvironmentVariable);

    public static bool ShouldForceUrda =>
        ForcedAncient is { Length: > 0 } forcedAncient &&
        (string.Equals(forcedAncient, "URDA", StringComparison.OrdinalIgnoreCase) ||
         string.Equals(forcedAncient, "EZMB_URDA", StringComparison.OrdinalIgnoreCase));

    public static bool IsUrdaEnabled(UnlockState _unlockState) =>
        !AncientFeatureGate.IsTruthyEnvironmentVariable(DisableAncientEnvironmentVariable, trimValue: true) &&
        !AncientFeatureGate.IsTruthyEnvironmentVariable(DisableAncientAliasEnvironmentVariable, trimValue: true);
}
