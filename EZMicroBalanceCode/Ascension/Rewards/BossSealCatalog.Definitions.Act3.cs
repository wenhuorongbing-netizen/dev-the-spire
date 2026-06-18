using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class BossSealCatalog
{
    private static void AddActThreeDefinitions(Dictionary<ModelId, BossSealDefinition> definitions)
    {
        definitions[EncounterId("THE_INSATIABLE_BOSS")] = new(
            BossSealId.StruggleBait,
            "Escape Fatigue",
            "When The Insatiable gains Strength or advances Sandpit, add 1 ability-made Frantic Escape to an affected player's discard. Every 3 such Escapes played gives Vigor.",
            BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
            "Uses Strength/Sandpit observations, Sandpit target ownership when available, generated-card tracking so vanilla Frantic Escape does not count, and source VigorPower; live source-classification verification pending.",
            "Every 3 ability-made Frantic Escapes played gives 3 Vigor instead of 2.");
        definitions[EncounterId("AEONGLASS_BOSS")] = new(
            BossSealId.AeonglassHourglass,
            "Time Sand Reflow",
            "After Ebb, create 2 shared Time Sand. Each energy spent removes 1; remaining Time Sand makes the next Increasing Intensity add extra Wither.",
            BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
            "v0.106.1 Aeonglass source uses EBB_MOVE, EYE_LASERS_MOVE, and INCREASING_INTENSITY_MOVE; this seal watches Ebb, tracks shared Time Sand through AfterEnergySpent, converts leftovers to extra Wither, and arms up to two branded Eye Laser extra-hit powers with an intent-label patch. Live energy and move-order verification pending.",
            "Ebb creates 3 Time Sand. If Eye Lasers starts while Time Sand remains, Eye Lasers hits 1 extra time, up to twice per combat.");
        definitions[EncounterId("QUEEN_BOSS")] = new(
            BossSealId.ChosenDecree,
            "Royal Decree",
            "One Bound card becomes a Royal Decree. Playing it has no extra penalty. Playing a wrong Bound card gives Majesty; playing no Bound card gives Majesty and Torch Head Strength.",
            BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
            "Uses Bound-card hand observations, deterministic combat-card-selection RNG for the marked Bound card, a temporary card enchantment, Majesty next-block power, and round caps for Majesty and Torch Head Strength; live card timing verification pending.",
            "Majesty cap rises to 3; each defense action can spend at most 2 Majesty.");
        definitions[EncounterId("TEST_SUBJECT_BOSS")] = new(
            BossSealId.ResidualSample,
            "Experimental Record",
            "At phase change, keep 1 Residual Sample based on the previous phase: Strength Residue, Skill Adaptation, Attack Adaptation, Antibody Sample, or Contaminated Sample.",
            BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
            "Uses TestSubject AdaptablePower death observation, phase card/debuff counters, pending sample replay after respawn, and custom sample powers for next-phase behavior; exact phase UI timing needs live verification.",
            "Each phase change keeps 2 different Residual Samples.");
    }
}
