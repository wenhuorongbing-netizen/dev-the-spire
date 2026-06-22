namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

internal static partial class Sts1EventFeatureGate
{
    /// <summary>
    /// Canonical canary event IDs for reporting and validation.
    /// RitsuLib registration remains hardcoded in Sts1EventRegistrationService.Canary.cs.
    /// </summary>
    public static IReadOnlyList<string> CanaryEventIds { get; } =
    [
        "sts1_big_fish",
        "sts1_golden_idol",
        "sts1_the_lab",
        "sts1_divine_fountain",
    ];

    /// <summary>
    /// Canonical AdditiveBatch1 event IDs for reporting and validation.
    /// Keep this list in sync with Sts1EventRegistrationService.AdditiveBatch1.cs.
    /// </summary>
    public static IReadOnlyList<string> AdditiveBatch1EventIds { get; } =
    [
        "sts1_big_fish",
        "sts1_golden_idol",
        "sts1_the_lab",
        "sts1_divine_fountain",
        "sts1_purifier",
        "sts1_upgrade_shrine",
        "sts1_golden_shrine",
        "sts1_the_cleric",
        "sts1_old_beggar",
        "sts1_shining_light",
    ];
}
