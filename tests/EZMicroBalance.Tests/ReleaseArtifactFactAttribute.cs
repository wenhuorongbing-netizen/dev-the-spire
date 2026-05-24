using Xunit;

namespace EZMicroBalance.Tests;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ReleaseArtifactFactAttribute : FactAttribute
{
    public const string EnvironmentVariable = "SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS";
    public const string LegacyEnvironmentVariable = "EZMB_RUN_RELEASE_ARTIFACT_TESTS";

    public ReleaseArtifactFactAttribute()
    {
        if (!IsEnabled)
        {
            Skip =
                $"{EnvironmentVariable}=1 is not set. Legacy alias {LegacyEnvironmentVariable}=1 also works. Skipping release artifact/runtime checks that require ignored publish, package, installed DLL/PCK, or local smoke-log artifacts. " +
                "Run `dotnet publish EZMicroBalance.sln`, refresh the package staging/zip artifacts as documented, then rerun with the environment variable set.";
        }
    }

    public static bool IsEnabled =>
        IsTruthy(Environment.GetEnvironmentVariable(EnvironmentVariable)) ||
        IsTruthy(Environment.GetEnvironmentVariable(LegacyEnvironmentVariable));

    private static bool IsTruthy(string? value)
    {
        return !string.IsNullOrWhiteSpace(value) &&
            (value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
             value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
             value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
             value.Equals("on", StringComparison.OrdinalIgnoreCase));
    }
}
