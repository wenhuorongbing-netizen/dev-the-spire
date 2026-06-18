using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class Sts1EventFeatureGuardTests
{
    [Fact]
    public void FeatureGateDefaultsToOffWhenEnvVarIsUnset()
    {
        var source = ReadSts1RuntimeSources();

        AssertSourceContains(
            source,
            "private const string ModeEnvKey = \"SPIREPLUS_STS1_EVENT_MODE\"",
            "return Sts1EventRegistrationMode.Off;",
            "string.IsNullOrWhiteSpace(envValue)",
            "Enum.TryParse<Sts1EventRegistrationMode>(envValue, ignoreCase: true, out var mode)");
    }

    [Fact]
    public void FeatureGateEvaluatesAllModes()
    {
        var source = ReadSts1RuntimeSources();

        AssertSourceContains(
            source,
            "Sts1EventRegistrationMode.Off => FeatureGateResult.Disabled(",
            "Sts1EventRegistrationMode.CanaryOnly => FeatureGateResult.Enabled(",
            "Sts1EventRegistrationMode.AdditiveBatch1 => FeatureGateResult.Enabled(",
            "Sts1EventRegistrationMode.AdditiveAllDraft => EvaluateAdditiveAllDraftGate()",
            "Sts1EventRegistrationMode.ReplaceUnknownEventsPrototype => EvaluateReplacementPrototypeGate()");
    }

    [Fact]
    public void UnsafeModesRequireExplicitUnsafeOverride()
    {
        var source = ReadSts1RuntimeSources();

        AssertSourceContains(
            source,
            "private const string UnsafeModeEnvKey = \"SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES\"",
            "set SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1 to enable",
            "IsUnsafeModeAllowed()",
            "IsTruthyEnvironmentValue(System.Environment.GetEnvironmentVariable(UnsafeModeEnvKey))");
    }

    [Fact]
    public void ReplacementPrototypeGateFailsClosedWithoutCompileSymbol()
    {
        var source = ReadSts1RuntimeSources();

        AssertSourceContains(
            source,
            "#if REPLACEMENT_PROTOTYPE_ENABLED",
            "#else",
            "FeatureGateResult.Disabled(",
            "REPLACEMENT_PROTOTYPE_ENABLED is not defined; no StS1 events registered.");
    }
}
