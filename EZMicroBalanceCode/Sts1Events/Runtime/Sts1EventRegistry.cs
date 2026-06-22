using System.Collections.Generic;
using System.Linq;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

/// <summary>
/// Registry of all StS1 event types for manifest verification and testing.
/// This registry is metadata only; RitsuLib registration stays in
/// Sts1EventRegistrationService.* so safe modes cannot inherit draft entries.
/// </summary>
internal static partial class Sts1EventRegistry
{
    public static IReadOnlyList<Sts1EventEntry> All => Events;

    public static IEnumerable<Sts1EventEntry> GetByPhase(Sts1EventPhase phase)
        => Events.Where(e => e.Phase == phase);

    public static IEnumerable<Sts1EventEntry> GetByAct(Sts1EventAct act)
        => Events.Where(e => e.Act == act || e.Act == Sts1EventAct.Shared);
}
