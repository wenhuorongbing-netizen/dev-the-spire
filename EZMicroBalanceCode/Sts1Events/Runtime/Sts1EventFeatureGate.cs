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
/// - <see cref="Sts1EventRegistrationMode.AdditiveAllDraft"/> registers all drafted events
///   additively (not replacing StS2 events).
/// - <see cref="Sts1EventRegistrationMode.ReplaceUnknownEventsPrototype"/> is debug-only.
/// - Mode is determined by environment variable SPIREPLUS_STS1_EVENT_MODE.
/// </summary>
internal static class Sts1EventFeatureGate
{
    private const string ModeEnvKey = "SPIREPLUS_STS1_EVENT_MODE";

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
            Sts1EventRegistrationMode.AdditiveAllDraft => FeatureGateResult.Enabled(
                "StS1 events AdditiveAllDraft mode: registering all drafted events."),
            Sts1EventRegistrationMode.ReplaceUnknownEventsPrototype => FeatureGateResult.Enabled(
                "StS1 events ReplaceUnknownEventsPrototype mode: debug-only replacement."),
            _ => FeatureGateResult.Disabled(
                $"StS1 events unknown mode '{mode}'; defaulting to Off."),
        };
    }

    /// <summary>
    /// Returns the canonical set of canary event IDs for <see cref="Sts1EventRegistrationMode.CanaryOnly"/>.
    /// </summary>
    public static IReadOnlyList<string> CanaryEventIds { get; } =
    [
        "sts1_big_fish",
        "sts1_golden_idol",
        "sts1_the_lab",
        "sts1_divine_fountain",
    ];
}
