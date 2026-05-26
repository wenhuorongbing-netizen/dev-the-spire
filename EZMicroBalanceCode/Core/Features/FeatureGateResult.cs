namespace EZMicroBalance.EZMicroBalanceCode.Core.Features;

internal readonly record struct FeatureGateResult(bool IsEnabled, string Reason)
{
    public static FeatureGateResult Enabled(string reason) => new(true, reason);

    public static FeatureGateResult EnabledByDefault(string reason) => Enabled(reason);

    public static FeatureGateResult Disabled(string reason) => new(false, reason);
}
