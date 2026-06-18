using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class BossSealCatalog
{
    private static void AddActTwoDefinitions(Dictionary<ModelId, BossSealDefinition> definitions)
    {
        definitions[EncounterId("SOUL_FYSH_BOSS")] = new(
            BossSealId.SoulTide,
            "Soul Tide",
            "Intangible grants 1 final Artifact. Beckons left in hand give Soul Fysh capped Block at the next player turn start.",
            BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
            "Uses enemy/player turn state scans for IntangiblePower entries, final Artifact correction, pre-flush Beckon-in-hand scans, and shared Block caps by player count; live Beckon timing verification pending.",
            "Each Beckon gives 3 Block instead of 2 and uses higher team caps.");
        definitions[EncounterId("WATERFALL_GIANT_BOSS")] = new(
            BossSealId.BoilingCritical,
            "Unweakenable",
            "On the explosion turn, clear Weak and attack reduction. The explosion ignores Weak and Strength loss, and affected players gain 1 Vulnerable.",
            BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
            "Uses WaterfallGiant.NextMove EXPLODE_MOVE, ArtifactPower final-display correction, VulnerablePower, Weak cleanup, and negative-Strength cleanup; live terminal-flow verification pending.",
            "Affected players gain 2 Vulnerable on the explosion turn. Base explosion damage is not increased.");
        definitions[EncounterId("KAISER_CRAB_BOSS")] = new(
            BossSealId.MisalignedShell,
            "Claw Calibration",
            "At player turn end, if the two claws' HP percentages differ by at least 35%, the higher-HP claw gains Calibration. At 2 Calibration, each hit of its next attack deals 4 extra damage.",
            BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
            "Uses the source-confirmed Crusher/Rocket pair, shared end-of-player-turn HP-ratio checks, and per-claw once-per-combat next-attack powers; live direction and timing verification pending.",
            "The HP gap threshold becomes 30%, and each hit of the calibrated attack deals 5 extra damage.");
        definitions[EncounterId("KNOWLEDGE_DEMON_BOSS")] = new(
            BossSealId.MarginalNote,
            "Marginal Note",
            "Curse of Knowledge adds 1 temporary Marginal Note to each player's discard pile. Unplayed Notes become Deep Thought, which adds side costs to the next Knowledge curse.",
            BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
            "Uses post-Curse-of-Knowledge move observation, combat-only note cards, per-round Deep Thought cap, and source-backed Disintegration/Mind Rot/Sloth/Waste Away side effects. The v4.1 side-cost design deliberately does not depend on exact unchosen curse identity, avoiding a brittle patch of KnowledgeDemon.ChooseCurse local state; live timing verification pending.",
            "Deep Thought cap rises to 3. Sloth and Waste Away side costs still resolve once per Knowledge curse.");
    }
}
