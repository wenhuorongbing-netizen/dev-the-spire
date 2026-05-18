using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task TrackSoulTideIntangible(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        var soulFysh = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is SoulFysh);
        var intangibleAmount = soulFysh?.GetPower<IntangiblePower>()?.Amount ?? 0m;
        if (soulFysh == null || intangibleAmount <= 0m)
        {
            tracker.LastSoulFyshIntangibleAmount = 0;
            return;
        }

        if (intangibleAmount <= tracker.LastSoulFyshIntangibleAmount)
        {
            tracker.LastSoulFyshIntangibleAmount = (int)intangibleAmount;
            return;
        }

        tracker.LastSoulFyshIntangibleAmount = (int)intangibleAmount;
        var artifact = metadata.IsBossBrand ? 2m : 1m;
        await PowerCmd.Apply<ArtifactPower>(new BlockingPlayerChoiceContext(), soulFysh, artifact, soulFysh, null);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Soul Tide added Artifact on Intangible entry.");
    }

    private static void TrackSoulTideBeckonsBeforeFlush(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Player player)
    {
        if (metadata.BossSeal?.Id != BossSealId.SoulTide)
        {
            return;
        }

        if (tracker.SoulTideBeckonSettlementRound != combatState.RoundNumber)
        {
            tracker.SoulTideBeckonSettlementRound = combatState.RoundNumber;
            tracker.PendingSoulTideBlock = 0m;
        }

        var beckonsInHand = player.Piles
            .Where(pile => pile.Type == PileType.Hand)
            .SelectMany(pile => pile.Cards)
            .Count(card => card is Beckon);

        var cap = metadata.IsBossBrand ? 16m : 12m;
        tracker.PendingSoulTideBlock = Math.Min(cap, tracker.PendingSoulTideBlock + (beckonsInHand * 2m));
    }

    private static async Task ApplySoulTidePendingBlock(CombatState combatState, AscensionCombatTracker tracker)
    {
        if (tracker.PendingSoulTideBlock <= 0m)
        {
            return;
        }

        var soulFysh = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is SoulFysh);
        if (soulFysh == null)
        {
            return;
        }

        var block = tracker.PendingSoulTideBlock;
        tracker.PendingSoulTideBlock = 0m;
        await CreatureCmd.GainBlock(soulFysh, block, ValueProp.Move, null, fast: true);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Soul Tide converted Beckon hand pressure into Block.");
    }
}
