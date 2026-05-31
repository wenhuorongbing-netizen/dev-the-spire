using System.Collections.Generic;
using System.Linq;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

/// <summary>
/// Registry of all StS1 event types for manifest verification and testing.
/// </summary>
internal static class Sts1EventRegistry
{
    private static readonly List<Sts1EventEntry> Events = new()
    {
        // Phase 1: Canary
        new("sts1_big_fish", "Big Fish", Sts1EventPhase.Canary, Sts1EventAct.Shared),
        new("sts1_golden_idol", "Golden Idol", Sts1EventPhase.Canary, Sts1EventAct.Shared),
        new("sts1_divine_fountain", "Divine Fountain", Sts1EventPhase.Canary, Sts1EventAct.Shared),
        new("sts1_the_lab", "The Lab", Sts1EventPhase.Canary, Sts1EventAct.Shared),

        // Phase 2: Simple batch
        new("sts1_the_cleric", "The Cleric", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        new("sts1_golden_wing", "Golden Wing", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        new("sts1_living_wall", "Living Wall", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        new("sts1_old_beggar", "Old Beggar", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        new("sts1_purifier", "Purifier", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        new("sts1_golden_shrine", "Golden Shrine", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        new("sts1_bonfire_spirits", "Bonfire Spirits", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        new("sts1_duplicator", "Duplicator", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        new("sts1_fountain_of_cleansing", "Fountain of Cleansing", Sts1EventPhase.Simple, Sts1EventAct.Shared),
        new("sts1_shining_light", "Shining Light", Sts1EventPhase.Simple, Sts1EventAct.Act1),
        new("sts1_mushrooms", "Mushrooms", Sts1EventPhase.Simple, Sts1EventAct.Act1),
        new("sts1_altar", "Altar", Sts1EventPhase.Simple, Sts1EventAct.Act2),
        new("sts1_drug_dealer", "Drug Dealer", Sts1EventPhase.Simple, Sts1EventAct.Act2),
        new("sts1_the_library", "The Library", Sts1EventPhase.Simple, Sts1EventAct.Act2),
        new("sts1_ancient_writing", "Ancient Writing", Sts1EventPhase.Simple, Sts1EventAct.Act2),
        new("sts1_augmenter", "Augmenter", Sts1EventPhase.Simple, Sts1EventAct.Act2),
        new("sts1_sensory_stone", "Sensory Stone", Sts1EventPhase.Simple, Sts1EventAct.Act3),
        new("sts1_moai_head", "Moai Head", Sts1EventPhase.Simple, Sts1EventAct.Act3),
        new("sts1_transmogrifier", "Transmogrifier", Sts1EventPhase.Simple, Sts1EventAct.Act3),
        new("sts1_upgrade_shrine", "Upgrade Shrine", Sts1EventPhase.Simple, Sts1EventAct.Act3),

        // Phase 3: Card service batch
        new("sts1_face_trader", "Face Trader", Sts1EventPhase.CardService, Sts1EventAct.Shared),
        new("sts1_the_mausoleum", "The Mausoleum", Sts1EventPhase.CardService, Sts1EventAct.Shared),
        new("sts1_council_of_ghosts", "Council of Ghosts", Sts1EventPhase.CardService, Sts1EventAct.Act2),
        new("sts1_cursed_tome", "Cursed Tome", Sts1EventPhase.CardService, Sts1EventAct.Act2),
        new("sts1_knowing_skull", "Knowing Skull", Sts1EventPhase.CardService, Sts1EventAct.Act2),
        new("sts1_nest", "Nest", Sts1EventPhase.CardService, Sts1EventAct.Act2),
        new("sts1_vampires", "Vampires", Sts1EventPhase.CardService, Sts1EventAct.Act2),
        new("sts1_falling", "Falling", Sts1EventPhase.CardService, Sts1EventAct.Act3),
        new("sts1_mind_bloom", "Mind Bloom", Sts1EventPhase.CardService, Sts1EventAct.Act3),

        // Phase 4: Combat batch
        new("sts1_dead_adventurer", "Dead Adventurer", Sts1EventPhase.Combat, Sts1EventAct.Act1),
        new("sts1_scorpion_nest", "Scorpion Nest", Sts1EventPhase.Combat, Sts1EventAct.Act1),
        new("sts1_treasure_ooze", "Treasure Ooze", Sts1EventPhase.Combat, Sts1EventAct.Act1),
        new("sts1_joust", "Joust", Sts1EventPhase.Combat, Sts1EventAct.Act1),
        new("sts1_the_ssssserpent", "The Ssssserpent", Sts1EventPhase.Combat, Sts1EventAct.Act1),
        new("sts1_masked_bandits", "Masked Bandits", Sts1EventPhase.Combat, Sts1EventAct.Act2),
        new("sts1_mysterious_sphere", "Mysterious Sphere", Sts1EventPhase.Combat, Sts1EventAct.Act3),

        // Phase 5: Custom UI batch (simplified to option-based)
        new("sts1_the_woman_in_blue", "The Woman in Blue", Sts1EventPhase.CustomUi, Sts1EventAct.Shared),
        new("sts1_wheel_of_change", "Wheel of Change", Sts1EventPhase.CustomUi, Sts1EventAct.Shared),
        new("sts1_designer", "Designer", Sts1EventPhase.CustomUi, Sts1EventAct.Shared),
        new("sts1_forgotten_altar", "Forgotten Altar", Sts1EventPhase.CustomUi, Sts1EventAct.Act2),
        new("sts1_the_ghost", "The Ghost", Sts1EventPhase.CustomUi, Sts1EventAct.Act2),
        new("sts1_nloth", "N'loth", Sts1EventPhase.CustomUi, Sts1EventAct.Act2),
        new("sts1_tomb_of_lord_red_mask", "Tomb of Lord Red Mask", Sts1EventPhase.CustomUi, Sts1EventAct.Act3),
        new("sts1_winding_halls", "Winding Halls", Sts1EventPhase.CustomUi, Sts1EventAct.Act3),

        // Special
        new("sts1_neow", "Neow", Sts1EventPhase.Special, Sts1EventAct.Act1),
        new("sts1_combat_start", "Combat Start", Sts1EventPhase.Special, Sts1EventAct.Act1),
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
    Special
}

internal enum Sts1EventAct
{
    Shared,
    Act1,
    Act2,
    Act3
}
