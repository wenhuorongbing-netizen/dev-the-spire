namespace EZMicroBalance.EZMicroBalanceCode.Core.Features;

internal sealed partial class FeatureRegistry
{
    private static FeatureGateResult ApplyEnvironmentOverrides(IFeatureModule module, FeatureGateResult gate)
    {
        var forceKey = FirstTruthyEnvironmentKey(module.ForceEnvKeys);
        if (forceKey is not null)
        {
            return FeatureGateResult.Enabled($"forced by {forceKey}; original gate: {gate.Reason}");
        }

        var disableKey = FirstTruthyEnvironmentKey(module.DisableEnvKeys);
        if (disableKey is not null)
        {
            return FeatureGateResult.Disabled($"disabled by {disableKey}; original gate: {gate.Reason}");
        }

        return gate;
    }

    private static string? FirstTruthyEnvironmentKey(IEnumerable<string> keys)
    {
        foreach (var key in keys)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            var value = Environment.GetEnvironmentVariable(key);
            if (IsTruthyEnvironmentValue(value))
            {
                return key;
            }
        }

        return null;
    }

    private static bool IsTruthyEnvironmentValue(string? value) =>
        !string.IsNullOrWhiteSpace(value)
        && !string.Equals(value, "0", StringComparison.OrdinalIgnoreCase)
        && !string.Equals(value, "false", StringComparison.OrdinalIgnoreCase)
        && !string.Equals(value, "off", StringComparison.OrdinalIgnoreCase)
        && !string.Equals(value, "no", StringComparison.OrdinalIgnoreCase);
}
