using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static class MorviFeatureGate
{
    public const string EnableEnvironmentVariable = "EZMB_ENABLE_MORVI_V22";
    public const string ForceBlessingEnvironmentVariable = "EZMB_FORCE_MORVI_BLESSING";

    public static string? ForcedBlessing => Environment.GetEnvironmentVariable(ForceBlessingEnvironmentVariable);

    public static bool IsMorviEnabled(UnlockState _)
    {
        var value = Environment.GetEnvironmentVariable(EnableEnvironmentVariable);
        return IsTruthy(value);
    }

    private static bool IsTruthy(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        (value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
         value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
         value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
         value.Equals("on", StringComparison.OrdinalIgnoreCase));
}

