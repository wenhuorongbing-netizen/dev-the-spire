namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

/// <summary>
/// Controls which StS1 events are registered with RitsuLib.
/// Default is <see cref="Off"/> to ensure zero impact on the live Spire Plus experience.
/// </summary>
internal enum Sts1EventRegistrationMode
{
    /// <summary>
    /// No StS1 events registered. Default mode — zero behavior impact on Spire Plus.
    /// </summary>
    Off = 0,

    /// <summary>
    /// Registers only the four canary events: Big Fish, Golden Idol, Lab, Divine Fountain.
    /// Used for source/API verification and manual testing.
    /// </summary>
    CanaryOnly = 1,

    /// <summary>
    /// Registers all drafted StS1 events additively alongside existing StS2 events.
    /// Not a replacement for the StS2 event pool — events coexist.
    /// </summary>
    AdditiveAllDraft = 2,

    /// <summary>
    /// Debug-only prototype: replaces unknown room events with StS1 events.
    /// Must not be enabled in release builds without explicit debug override.
    /// </summary>
    ReplaceUnknownEventsPrototype = 3,
}
