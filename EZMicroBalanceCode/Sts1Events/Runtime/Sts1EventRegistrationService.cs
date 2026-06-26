namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

/// <summary>
/// Registers StS1 events through RitsuLib content packs.
///
/// Keep this service on RitsuLib APIs only. The feature is intentionally gated,
/// and new event batches should extend the mode-specific partial registration
/// files instead of adding dynamic registration loops here.
///
/// StS2 act mapping (verified from ActModel.Index):
///   StS1 Act 1 events -> Overgrowth + Underdocks (both Index == 0)
///   StS1 Act 2 events -> Hive (Index == 1)
///   StS1 Act 3 events -> Glory (Index == 2)
/// </summary>
internal static partial class Sts1EventRegistrationService
{
    /// <summary>
    /// Registers events gated by mode. Called from Sts1EventsFeatureModule.Initialize().
    /// </summary>
    public static void RegisterGated(string modId, Sts1EventRegistrationMode mode)
    {
        switch (mode)
        {
            case Sts1EventRegistrationMode.Off:
                return;
            case Sts1EventRegistrationMode.CanaryOnly:
                RegisterCanaryOnly(modId);
                return;
            case Sts1EventRegistrationMode.AdditiveBatch1:
                RegisterAdditiveBatch1(modId);
                return;
            case Sts1EventRegistrationMode.AdditiveAllDraft:
                RegisterAll(modId);
                return;
            case Sts1EventRegistrationMode.ReplaceUnknownEventsPrototype:
#if REPLACEMENT_PROTOTYPE_ENABLED
                RegisterAll(modId);
#else
                MainFile.Logger.Warn("[StS1 Events] ReplaceUnknownEventsPrototype requested, but REPLACEMENT_PROTOTYPE_ENABLED is not defined; no StS1 events registered.");
#endif
                return;
        }
    }
}
