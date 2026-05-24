using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static class LothaFeatureGate
{
    public const string DisableEnvironmentVariable = "SPIREPLUS_DISABLE_LOTHA";
    public const string LegacyDisableEnvironmentVariable = "EZMB_DISABLE_LOTHA";
    public const string ForceAncientEnvironmentVariable = "SPIREPLUS_FORCE_ANCIENT";
    public const string LegacyForceAncientEnvironmentVariable = "EZMB_FORCE_ANCIENT";
    public const string ForceBlessingEnvironmentVariable = "SPIREPLUS_FORCE_LOTHA_BLESSING";
    public const string LegacyForceBlessingEnvironmentVariable = "EZMB_FORCE_LOTHA_BLESSING";

    public static string? ForcedAncient =>
        AncientFeatureGate.FirstRawEnvironmentValue(ForceAncientEnvironmentVariable, LegacyForceAncientEnvironmentVariable);

    public static string? ForcedBlessing =>
        AncientFeatureGate.FirstRawEnvironmentValue(ForceBlessingEnvironmentVariable, LegacyForceBlessingEnvironmentVariable);

    public static bool ShouldForceLotha =>
        IsForcedAncient("LOTHA") || IsForcedAncient("EZMB_LOTHA");

    public static bool IsLothaEnabled(UnlockState _) =>
        !AncientFeatureGate.IsTruthyEnvironmentVariable(DisableEnvironmentVariable) &&
        !AncientFeatureGate.IsTruthyEnvironmentVariable(LegacyDisableEnvironmentVariable);

    private static bool IsForcedAncient(string value) =>
        AncientFeatureGate.IsForcedAncient(ForcedAncient, value);
}
