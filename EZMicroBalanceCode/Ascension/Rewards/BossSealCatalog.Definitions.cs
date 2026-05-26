using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class BossSealCatalog
{
    private const string EncounterCategory = "ENCOUNTER";

    private static readonly IReadOnlyDictionary<ModelId, BossSealDefinition> DefinitionsByEncounter =
        new Dictionary<ModelId, BossSealDefinition>
        {
            [EncounterId("CEREMONIAL_BEAST_BOSS")] = new(
                BossSealId.HolyDaze,
                "Holy Daze",
                "During the first stun, each hit deals at most 1 damage; afterward the Boss gains 1 Strength.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Observed through PlowPower removal, CeremonialBeast.NextMove, settled damage hooks, and a custom damage-cap power; exact first-stun timing still needs live trace verification.",
                "After the first stun ends, the Boss gains 2 Strength."),
            [EncounterId("THE_KIN_BOSS")] = new(
                BossSealId.MartyrOath,
                "Martyr Oath",
                "Follower deaths give Kin Priest Martyr Oath, up to 2. Its next debuff lasts longer, or each hit of its next attack deals 3 extra damage per Oath.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses the source-confirmed two KinFollower deaths, custom next-debuff/next-attack powers, and final displayed Artifact correction for the same-turn Brand bonus; live encounter ownership verification pending.",
                "Each Oath adds 4 damage to each hit instead. If both followers die in one player turn, Kin Priest gains exactly 1 Artifact."),
            [EncounterId("VANTOM_BOSS")] = new(
                BossSealId.InkReturn,
                "Ink Return",
                "The first full Slippery removal restores 25% of the cleared Slippery, minimum 3 and maximum 12, on the next enemy turn.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses settled damage hooks plus player/enemy turn state scans for spent SlipperyPower, stores the displayed cleared amount, and corrects enemy Slippery multiplayer scaling back to the final displayed restore value; live power-removal timing verification pending.",
                "Restores 35% of the cleared Slippery instead, minimum 5 and maximum 18. Still triggers once per combat."),
            [EncounterId("LAGAVULIN_MATRIARCH_BOSS")] = new(
                BossSealId.StartledShell,
                "Plating Wake",
                "Player-hit wake grants 4 base Plating; natural wake grants 8. Multiplayer uses the game's boss Plating scaling. The first Soul Siphon removes half of current Plating.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses settled damage, enemy move observation, turn-end state scans, and source PlatingPower scaling instead of final-value correction; live wake-source timing verification pending.",
                "Player-hit wake grants 6 base Plating, natural wake grants 10, and the first Soul Siphon removes only one-third."),
            [EncounterId("SOUL_FYSH_BOSS")] = new(
                BossSealId.SoulTide,
                "Soul Tide",
                "Intangible grants 1 final Artifact. Beckons left in hand give Soul Fysh capped Block at the next player turn start.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses enemy/player turn state scans for IntangiblePower entries, final Artifact correction, pre-flush Beckon-in-hand scans, and shared Block caps by player count; live Beckon timing verification pending.",
                "Each Beckon gives 3 Block instead of 2 and uses higher team caps."),
            [EncounterId("WATERFALL_GIANT_BOSS")] = new(
                BossSealId.BoilingCritical,
                "Unweakenable",
                "On the explosion turn, clear Weak and attack reduction. The explosion ignores Weak and Strength loss, and affected players gain 1 Vulnerable.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses WaterfallGiant.NextMove EXPLODE_MOVE, ArtifactPower final-display correction, VulnerablePower, Weak cleanup, and negative-Strength cleanup; live terminal-flow verification pending.",
                "Affected players gain 2 Vulnerable on the explosion turn. Base explosion damage is not increased."),
            [EncounterId("KAISER_CRAB_BOSS")] = new(
                BossSealId.MisalignedShell,
                "Claw Calibration",
                "At player turn end, if the two claws' HP percentages differ by at least 35%, the higher-HP claw gains Calibration. At 2 Calibration, each hit of its next attack deals 4 extra damage.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses the source-confirmed Crusher/Rocket pair, shared end-of-player-turn HP-ratio checks, and per-claw once-per-combat next-attack powers; live direction and timing verification pending.",
                "The HP gap threshold becomes 30%, and each hit of the calibrated attack deals 5 extra damage."),
            [EncounterId("KNOWLEDGE_DEMON_BOSS")] = new(
                BossSealId.MarginalNote,
                "Marginal Note",
                "Curse of Knowledge adds 1 temporary Marginal Note to each player's discard pile. Unplayed Notes become Deep Thought, which adds side costs to the next Knowledge curse.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses post-Curse-of-Knowledge move observation, combat-only note cards, per-round Deep Thought cap, and source-backed Disintegration/Mind Rot/Sloth/Waste Away side effects. The v4.1 side-cost design deliberately does not depend on exact unchosen curse identity, avoiding a brittle patch of KnowledgeDemon.ChooseCurse local state; live timing verification pending.",
                "Deep Thought cap rises to 3. Sloth and Waste Away side costs still resolve once per Knowledge curse."),
            [EncounterId("THE_INSATIABLE_BOSS")] = new(
                BossSealId.StruggleBait,
                "Escape Fatigue",
                "When The Insatiable gains Strength or advances Sandpit, add 1 ability-made Frantic Escape to an affected player's discard. Every 3 such Escapes played gives Vigor.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses Strength/Sandpit observations, Sandpit target ownership when available, generated-card tracking so vanilla Frantic Escape does not count, and source VigorPower; live source-classification verification pending.",
                "Every 3 ability-made Frantic Escapes played gives 3 Vigor instead of 2."),
            [EncounterId("AEONGLASS_BOSS")] = new(
                BossSealId.AeonglassHourglass,
                "Time Sand Reflow",
                "After Ebb, create 2 shared Time Sand. Each energy spent removes 1; remaining Time Sand makes the next Increasing Intensity add extra Wither.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "v0.106.0 Aeonglass source uses EBB_MOVE, EYE_LASERS_MOVE, and INCREASING_INTENSITY_MOVE; this seal watches Ebb, tracks shared Time Sand through AfterEnergySpent, converts leftovers to extra Wither, and arms up to two branded Eye Laser extra-hit powers with an intent-label patch. Live energy and move-order verification pending.",
                "Ebb creates 3 Time Sand. If Eye Lasers starts while Time Sand remains, Eye Lasers hits 1 extra time, up to twice per combat."),
            [EncounterId("QUEEN_BOSS")] = new(
                BossSealId.ChosenDecree,
                "Royal Decree",
                "One Bound card becomes a Royal Decree. Playing it has no extra penalty. Playing a wrong Bound card gives Majesty; playing no Bound card gives Majesty and Torch Head Strength.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses Bound-card hand observations, deterministic combat-card-selection RNG for the marked Bound card, a temporary card enchantment, Majesty next-block power, and round caps for Majesty and Torch Head Strength; live card timing verification pending.",
                "Majesty cap rises to 3; each defense action can spend at most 2 Majesty."),
            [EncounterId("TEST_SUBJECT_BOSS")] = new(
                BossSealId.ResidualSample,
                "Experimental Record",
                "At phase change, keep 1 Residual Sample based on the previous phase: Strength Residue, Skill Adaptation, Attack Adaptation, Antibody Sample, or Contaminated Sample.",
                BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
                "Uses TestSubject AdaptablePower death observation, phase card/debuff counters, pending sample replay after respawn, and custom sample powers for next-phase behavior; exact phase UI timing needs live verification.",
                "Each phase change keeps 2 different Residual Samples.")
        };

    private static ModelId EncounterId(string entry)
    {
        return new ModelId(EncounterCategory, entry);
    }
}
