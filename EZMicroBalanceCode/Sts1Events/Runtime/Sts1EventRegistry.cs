using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

/// <summary>
/// Registry of all StS1 event types for manifest verification and testing.
/// RitsuLib auto-registration handles actual event pool injection.
/// </summary>
internal static class Sts1EventRegistry
{
    private static readonly List<Sts1EventEntry> Events = new()
    {
        // Phase 1: Canary
        new("sts1_big_fish", "Big Fish", Sts1EventPhase.Canary, Sts1EventAct.Shared),
        new("sts1_golden_idol", "Golden Idol", Sts1EventPhase.Canary, Sts1EventAct.Shared),

        // Phase 2: Simple batch (placeholders — implement one by one)
        // new("sts1_the_cleric", "The Cleric", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        // new("sts1_golden_wing", "Golden Wing", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        // new("sts1_living_wall", "Living Wall", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        // new("sts1_old_beggar", "Old Beggar", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        // new("sts1_bonfire_spirits", "Bonfire Spirits", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        // new("sts1_divine_fountain", "Divine Fountain", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        // new("sts1_duplicator", "Duplicator", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        // new("sts1_fountain_of_cleansing", "Fountain of Cleansing", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        // new("sts1_the_lab", "The Lab", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        // new("sts1_shining_light", "Shining Light", Sts1EventPhase.Simple, Sts1EventAct.Act1),
        // new("sts1_mushrooms", "Mushrooms", Sts1EventPhase.Simple, Sts1EventAct.Act1),
        // new("sts1_altar", "Altar", Sts1EventPhase.Simple, Sts1EventAct.Act2),
        // new("sts1_drug_dealer", "Drug Dealer", Sts1EventPhase.Simple, Sts1EventAct.Act2),
        // new("sts1_the_library", "The Library", Sts1EventPhase.Simple, Sts1EventAct.Act2),
        // new("sts1_ancient_writing", "Ancient Writing", Sts1EventPhase.Simple, Sts1EventAct.Act2),
        // new("sts1_augmenter", "Augmenter", Sts1EventPhase.Simple, Sts1EventAct.Act2),
        // new("sts1_sensory_stone", "Sensory Stone", Sts1EventPhase.Simple, Sts1EventAct.Act3),
        // new("sts1_moai_head", "Moai Head", Sts1EventPhase.Simple, Sts1EventAct.Act3),
        // new("sts1_transmogrifier", "Transmogrifier", Sts1EventPhase.Simple, Sts1EventAct.Act3),
        // new("sts1_upgrade_shrine", "Upgrade Shrine", Sts1EventPhase.Simple, Sts1EventAct.Act3),
    };

    public static IReadOnlyList<Sts1EventEntry> All => Events;

    public static IEnumerable<Sts1EventEntry> GetByPhase(Sts1EventPhase phase)
        => Events.Where(e => e.Phase == phase);

    public static IEnumerable<Sts1EventEntry> GetByAct(Sts1EventAct act)
        => Events.Where(e => e.Act == act || e.Act == Sts1EventAct.Shared);
}

internal sealed record Sts1EventEntry(
    string Id,
    string DisplayName,
    Sts1EventPhase Phase,
    Sts1EventAct Act);

internal enum Sts1EventPhase
{
    Canary,
    Simple,
    CardService,
    Combat,
    CustomUi,
    PoolReplacement
}

internal enum Sts1EventAct
{
    Shared,
    Act1,
    Act2,
    Act3
}
