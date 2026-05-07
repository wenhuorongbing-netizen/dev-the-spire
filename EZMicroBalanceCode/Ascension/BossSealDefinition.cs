using MegaCrit.Sts2.Core.Models.Encounters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal enum BossSealId
{
    HolyDaze,
    MartyrOath,
    InkReturn,
    StartledShell,
    SoulTide,
    BoilingCritical,
    MisalignedShell,
    MarginalNote,
    StruggleBait,
    DoorWedge,
    ChosenDecree,
    ResidualSample
}

internal enum BossSealImplementationStatus
{
    SourceGuardedPendingLiveVerification
}

internal sealed record BossSealDefinition(
    BossSealId Id,
    string Name,
    string Summary,
    BossSealImplementationStatus Status,
    string RuntimeEvidence,
    string BrandSummary);

internal static class BossSealCatalog
{
    private static readonly IReadOnlyDictionary<ModelId, BossSealDefinition> DefinitionsByEncounter =
        new Dictionary<ModelId, BossSealDefinition>
        {
            [ModelDb.GetId<CeremonialBeastBoss>()] = new(
                BossSealId.HolyDaze,
                "Holy Daze",
                "First stun should turn the first stun round into setup instead of burst.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Observed through PlowPower removal, CeremonialBeast.NextMove, settled damage hooks, and a custom damage-cap power; exact first-stun timing still needs live trace verification.",
                "Holy Daze would grant 2 Strength after ending."),
            [ModelDb.GetId<TheKinBoss>()] = new(
                BossSealId.MartyrOath,
                "Martyr Oath",
                "Follower deaths should strengthen Kin Priest with capped Block/Strength.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses AfterDeath on KinFollower and command APIs on the living KinPriest; live encounter ownership verification pending.",
                "Trigger cap rises to 3 follower deaths and each trigger grants 14 Block."),
            [ModelDb.GetId<VantomBoss>()] = new(
                BossSealId.InkReturn,
                "Ink Return",
                "First full Slippery removal should restore Slippery next turn.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses settled damage hooks plus player/enemy turn state scans for spent SlipperyPower; live power-removal timing verification pending.",
                "Restores 2 Slippery and grants 1 Strength when it returns."),
            [ModelDb.GetId<LagavulinMatriarchBoss>()] = new(
                BossSealId.StartledShell,
                "Startled Shell",
                "Wake-up source should tune starting Plating and later Plating reduction.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses settled damage, enemy move observation, and turn-end state scans for wake-up and Soul Siphon settlement; live wake-source timing verification pending.",
                "Wake Plating rises to 10 and Soul Siphon only trims one-third."),
            [ModelDb.GetId<SoulFyshBoss>()] = new(
                BossSealId.SoulTide,
                "Soul Tide",
                "Intangible entries and Beckon settlement should grant Artifact/Block rhythm rewards.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses enemy/player turn state scans for IntangiblePower entries and end-of-player-turn Beckon-in-hand scans without mutating Soul Fysh actions; live Beckon timing verification pending.",
                "Intangible Artifact +1 and increased Beckon Block cap."),
            [ModelDb.GetId<WaterfallGiantBoss>()] = new(
                BossSealId.BoilingCritical,
                "Boiling Critical",
                "Steam Eruption milestones should add Boiling and telegraphed explosion pressure.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses SteamEruptionPower state scans, WaterfallGiant.NextMove, and a custom additive-damage/block telegraph power; live terminal-flow verification pending.",
                "Boiling milestones trigger every 10 Steam and the explosion warning Block is reduced."),
            [ModelDb.GetId<KaiserCrabBoss>()] = new(
                BossSealId.MisalignedShell,
                "Misaligned Shell",
                "Back attacks and claw deaths should add capped shell protection.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses Crusher/Rocket back-attack powers, settled damage hooks, and delayed same-turn claw-death settlement; live direction timing verification pending.",
                "Back-attack Block rises to 8 and the surviving claw gains 2 Artifact."),
            [ModelDb.GetId<KnowledgeDemonBoss>()] = new(
                BossSealId.MarginalNote,
                "Marginal Note",
                "Curse choices should add a temporary note card with end-turn pressure.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses post-Curse-of-Knowledge move observation and combat-only note cards; exact unchosen-curse identity remains unhooked and must be live-reviewed.",
                "Curse of Knowledge adds a second Marginal Note."),
            [ModelDb.GetId<TheInsatiableBoss>()] = new(
                BossSealId.StruggleBait,
                "Struggle Bait",
                "Self-enhancement should add Frantic Escape pressure.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses Strength/Sandpit/heal observations on The Insatiable and generated Frantic Escape cards; live source-classification verification pending.",
                "Each unplayed generated Frantic Escape grants 5 Block after 2 player turns."),
            [ModelDb.GetId<DoormakerBoss>()] = new(
                BossSealId.DoorWedge,
                "Door Wedge",
                "First revealed turn should cap per-hit damage until enough Attacks remove the wedge.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses Doormaker phase-power observation and a custom damage-cap power; does not add draw, cost, or Exhaust restrictions.",
                "Removed by 4th Attack, with per-hit cap increased to 50."),
            [ModelDb.GetId<QueenBoss>()] = new(
                BossSealId.ChosenDecree,
                "Chosen Decree",
                "One Bound card should become a Royal Decree with capped Queen/Amalgam settlement.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses Bound-card hand observations, a temporary card enchantment, and a one-shot Amalgam Strength reducer; live card timing verification pending.",
                "Higher Queen Block; playing Royal Decree also grants small Block."),
            [ModelDb.GetId<TestSubjectBoss>()] = new(
                BossSealId.ResidualSample,
                "Residual Sample",
                "Phase changes should retain weakened samples from prior phases.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses TestSubject AdaptablePower death observation and a persistent custom sample power applied after respawn; exact phase UI timing needs live verification.",
                "First phase change keeps 2 weakened samples, then 1 afterward.")
        };

    public static BossSealDefinition? TryGetForEncounter(EncounterModel? encounter)
    {
        return encounter != null && DefinitionsByEncounter.TryGetValue(encounter.Id, out var definition)
            ? definition
            : null;
    }

    public static string GetLocalizationKey(BossSealId id)
    {
        return id switch
        {
            BossSealId.HolyDaze => "BOSS_SEAL_HOLY_DAZE",
            BossSealId.MartyrOath => "BOSS_SEAL_MARTYR_OATH",
            BossSealId.InkReturn => "BOSS_SEAL_INK_RETURN",
            BossSealId.StartledShell => "BOSS_SEAL_STARTLED_SHELL",
            BossSealId.SoulTide => "BOSS_SEAL_SOUL_TIDE",
            BossSealId.BoilingCritical => "BOSS_SEAL_BOILING_CRITICAL",
            BossSealId.MisalignedShell => "BOSS_SEAL_MISALIGNED_SHELL",
            BossSealId.MarginalNote => "BOSS_SEAL_MARGINAL_NOTE",
            BossSealId.StruggleBait => "BOSS_SEAL_STRUGGLE_BAIT",
            BossSealId.DoorWedge => "BOSS_SEAL_DOOR_WEDGE",
            BossSealId.ChosenDecree => "BOSS_SEAL_CHOSEN_DECREE",
            BossSealId.ResidualSample => "BOSS_SEAL_RESIDUAL_SAMPLE",
            _ => "BOSS_ROYAL_SEAL"
        };
    }
}
