using EZMicroBalance.EZMicroBalanceCode.Core.Features;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

/// <summary>
/// Feature gate for StS1 event registration.
/// Controls whether and how StS1 events are registered with RitsuLib.
///
/// Design decisions:
/// - Default mode is <see cref="Sts1EventRegistrationMode.Off"/> (zero registrations).
/// - <see cref="Sts1EventRegistrationMode.CanaryOnly"/> targets exactly 4 events:
///   Big Fish, Golden Idol, Lab, Divine Fountain.
/// - <see cref="Sts1EventRegistrationMode.AdditiveBatch1"/> targets exactly the verified 10-event prototype scope.
/// - <see cref="Sts1EventRegistrationMode.AdditiveAllDraft"/> is unsafe/dev-only and
///   requires an explicit unsafe-mode override before registering drafted events.
/// - <see cref="Sts1EventRegistrationMode.ReplaceUnknownEventsPrototype"/> is debug-only,
///   compile-symbol gated, and requires the unsafe-mode override.
/// - Mode is determined by environment variable SPIREPLUS_STS1_EVENT_MODE.
/// </summary>
internal static partial class Sts1EventFeatureGate
{
    private const string ModeEnvKey = "SPIREPLUS_STS1_EVENT_MODE";
    private const string UnsafeModeEnvKey = "SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES";

    /// <summary>
    /// Resolves the current registration mode from environment or default.
    /// </summary>
    public static Sts1EventRegistrationMode ResolveMode()
    {
        var envValue = System.Environment.GetEnvironmentVariable(ModeEnvKey);
        if (string.IsNullOrWhiteSpace(envValue))
        {
            return Sts1EventRegistrationMode.Off;
        }

        return System.Enum.TryParse<Sts1EventRegistrationMode>(envValue, ignoreCase: true, out var mode)
            ? mode
            : Sts1EventRegistrationMode.Off;
    }

    /// <summary>
    /// Returns the feature gate result for the registry based on resolved mode.
    /// </summary>
    public static FeatureGateResult EvaluateGate()
    {
        var mode = ResolveMode();
        return mode switch
        {
            Sts1EventRegistrationMode.Off => FeatureGateResult.Disabled(
                "StS1 events default Off; set SPIREPLUS_STS1_EVENT_MODE to enable."),
            Sts1EventRegistrationMode.CanaryOnly => FeatureGateResult.Enabled(
                "StS1 events CanaryOnly mode: registering 4 canary events."),
            Sts1EventRegistrationMode.AdditiveBatch1 => FeatureGateResult.Enabled(
                "StS1 events AdditiveBatch1 mode: registering 10 verified-scope events."),
            Sts1EventRegistrationMode.AdditiveAllDraft => EvaluateAdditiveAllDraftGate(),
            Sts1EventRegistrationMode.ReplaceUnknownEventsPrototype => EvaluateReplacementPrototypeGate(),
            _ => FeatureGateResult.Disabled(
                $"StS1 events unknown mode '{mode}'; defaulting to Off."),
        };
    }

    private static FeatureGateResult EvaluateAdditiveAllDraftGate() =>
        IsUnsafeModeAllowed()
            ? FeatureGateResult.Enabled(
                "StS1 events AdditiveAllDraft mode: registering all drafted events with explicit unsafe dev override.")
            : FeatureGateResult.Disabled(
                "StS1 events AdditiveAllDraft mode is unsafe/dev-only; set SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1 to enable.");

    private static FeatureGateResult EvaluateReplacementPrototypeGate()
    {
#if REPLACEMENT_PROTOTYPE_ENABLED
        return IsUnsafeModeAllowed()
            ? FeatureGateResult.Enabled(
                "StS1 events ReplaceUnknownEventsPrototype mode: debug-only replacement compiled with explicit unsafe override.")
            : FeatureGateResult.Disabled(
                "StS1 events ReplaceUnknownEventsPrototype mode is unsafe/debug-only; set SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1 to enable.");
#else
        return FeatureGateResult.Disabled(
            "StS1 events ReplaceUnknownEventsPrototype requested, but REPLACEMENT_PROTOTYPE_ENABLED is not defined; no StS1 events registered.");
#endif
    }

    private static bool IsUnsafeModeAllowed() =>
        IsTruthyEnvironmentValue(System.Environment.GetEnvironmentVariable(UnsafeModeEnvKey));

    private static bool IsTruthyEnvironmentValue(string? value) =>
        !string.IsNullOrWhiteSpace(value)
        && !string.Equals(value, "0", StringComparison.OrdinalIgnoreCase)
        && !string.Equals(value, "false", StringComparison.OrdinalIgnoreCase)
        && !string.Equals(value, "off", StringComparison.OrdinalIgnoreCase)
        && !string.Equals(value, "no", StringComparison.OrdinalIgnoreCase);

}
