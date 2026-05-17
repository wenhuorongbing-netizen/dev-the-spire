namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Common;

internal static class AncientFeatureGate
{
    public static string? FirstRawEnvironmentValue(params string[] names)
    {
        foreach (var name in names)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (value is not null)
            {
                return value;
            }
        }

        return null;
    }

    public static string? FirstNonBlankEnvironmentValue(params string[] names)
    {
        foreach (var name in names)
        {
            var value = Environment.GetEnvironmentVariable(name)?.Trim();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    public static bool IsForcedAncient(string? forcedAncient, string value) =>
        string.Equals(forcedAncient?.Trim(), value, StringComparison.OrdinalIgnoreCase);

    public static bool IsTruthyEnvironmentVariable(string name, bool trimValue = false) =>
        IsTruthy(Environment.GetEnvironmentVariable(name), trimValue);

    private static bool IsTruthy(string? value, bool trimValue)
    {
        var candidate = trimValue ? value?.Trim() : value;
        return !string.IsNullOrWhiteSpace(candidate) &&
            (candidate.Equals("1", StringComparison.OrdinalIgnoreCase) ||
             candidate.Equals("true", StringComparison.OrdinalIgnoreCase) ||
             candidate.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
             candidate.Equals("on", StringComparison.OrdinalIgnoreCase));
    }
}
