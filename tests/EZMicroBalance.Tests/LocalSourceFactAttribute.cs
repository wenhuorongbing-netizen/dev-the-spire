using Xunit;

namespace EZMicroBalance.Tests;

[AttributeUsage(AttributeTargets.Method)]
public sealed class LocalSourceFactAttribute : FactAttribute
{
    public const string EnvironmentVariable = "SPIREPLUS_RUN_LOCAL_SOURCE_GUARDS";

    public LocalSourceFactAttribute()
    {
        if (!IsEnabled)
        {
            Skip =
                $"{EnvironmentVariable}=1 is not set. Skipping local-source API drift/source-shape guards that require ignored Slay the Spire 2 source under source code/src/Core/** or SPIREPLUS_LOCAL_GAME_SOURCE_ROOT. " +
                "Refresh the local source snapshot before enabling this lane.";
        }
    }

    public static bool IsEnabled => IsTruthy(Environment.GetEnvironmentVariable(EnvironmentVariable));

    private static bool IsTruthy(string? value)
    {
        return !string.IsNullOrWhiteSpace(value) &&
            (value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
             value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
             value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
             value.Equals("on", StringComparison.OrdinalIgnoreCase));
    }
}
