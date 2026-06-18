using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class BossSealCatalog
{
    private static void AddActOneDefinitions(Dictionary<ModelId, BossSealDefinition> definitions)
    {
        definitions[EncounterId("CEREMONIAL_BEAST_BOSS")] = new(
            BossSealId.HolyDaze,
            "Holy Daze",
            "During the first stun, each hit deals at most 1 damage; afterward the Boss gains 1 Strength.",
            BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
            "Observed through PlowPower removal, CeremonialBeast.NextMove, settled damage hooks, and a custom damage-cap power; exact first-stun timing still needs live trace verification.",
            "After the first stun ends, the Boss gains 2 Strength.");
        definitions[EncounterId("THE_KIN_BOSS")] = new(
            BossSealId.MartyrOath,
            "Martyr Oath",
            "Follower deaths give Kin Priest Martyr Oath, up to 2. Its next debuff lasts longer, or each hit of its next attack deals 3 extra damage per Oath.",
            BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
            "Uses the source-confirmed two KinFollower deaths, custom next-debuff/next-attack powers, and final displayed Artifact correction for the same-turn Brand bonus; live encounter ownership verification pending.",
            "Each Oath adds 4 damage to each hit instead. If both followers die in one player turn, Kin Priest gains exactly 1 Artifact.");
        definitions[EncounterId("VANTOM_BOSS")] = new(
            BossSealId.InkReturn,
            "Ink Return",
            "The first full Slippery removal restores 25% of the cleared Slippery, minimum 3 and maximum 12, on the next enemy turn.",
            BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
            "Uses settled damage hooks plus player/enemy turn state scans for spent SlipperyPower, stores the displayed cleared amount, and corrects enemy Slippery multiplayer scaling back to the final displayed restore value; live power-removal timing verification pending.",
            "Restores 35% of the cleared Slippery instead, minimum 5 and maximum 18. Still triggers once per combat.");
        definitions[EncounterId("LAGAVULIN_MATRIARCH_BOSS")] = new(
            BossSealId.StartledShell,
            "Plating Wake",
            "Player-hit wake grants 4 base Plating; natural wake grants 8. Multiplayer uses the game's boss Plating scaling. The first Soul Siphon removes half of current Plating.",
            BossSealImplementationStatus.SourceGuardedPendingLiveVerification,
            "Uses settled damage, enemy move observation, turn-end state scans, and source PlatingPower scaling instead of final-value correction; live wake-source timing verification pending.",
            "Player-hit wake grants 6 base Plating, natural wake grants 10, and the first Soul Siphon removes only one-third.");
    }
}
