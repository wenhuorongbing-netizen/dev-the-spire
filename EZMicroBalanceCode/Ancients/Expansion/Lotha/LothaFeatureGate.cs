using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
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
        AncientFeatureGate.FirstRawEnvironmentValue(SpirePlusForceAncientEnvironmentVariable, ForceAncientEnvironmentVariable);

    public static string? ForcedBlessing =>
        AncientFeatureGate.FirstRawEnvironmentValue(SpirePlusForceBlessingEnvironmentVariable, ForceBlessingEnvironmentVariable);

    public static bool ShouldForceLotha =>
        IsForcedAncient("LOTHA") || IsForcedAncient("EZMB_LOTHA");

    public static bool IsLothaEnabled(UnlockState _) =>
        !AncientFeatureGate.IsTruthyEnvironmentVariable(DisableEnvironmentVariable) &&
        !AncientFeatureGate.IsTruthyEnvironmentVariable(SpirePlusDisableEnvironmentVariable);

    private static bool IsForcedAncient(string value) =>
        AncientFeatureGate.IsForcedAncient(ForcedAncient, value);
}
