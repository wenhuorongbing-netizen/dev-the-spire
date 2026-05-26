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
        await ApplyPowerWithFinalDisplayedGain<ArtifactPower>(soulFysh, 1, soulFysh, null);
        MainFile.Logger.Info("[Spire Plus] Ascension A19 applied: Soul Tide added Artifact on Intangible entry.");
    }

    private static void TrackSoulTideBeckonsBeforePlayerTurnEnd(
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

        // Beckon moves itself out of hand while resolving its turn-end damage.
        // Count it before Core runs turn-end in-hand effects, then wait until
        // the next player side starts. That is the first moment after Soul
        // Fysh's enemy turn where the Block should become visible.
        var beckonsInHand = player.Piles
            .Where(pile => pile.Type == PileType.Hand)
            .SelectMany(pile => pile.Cards)
            .Count(card => card is Beckon);

        var blockPerBeckon = metadata.IsBossBrand ? 3m : 2m;
        var cap = SoulTideBlockCap(combatState, metadata.IsBossBrand);
        tracker.PendingSoulTideBlock = Math.Min(cap, tracker.PendingSoulTideBlock + beckonsInHand * blockPerBeckon);
    }

    private static int SoulTideBlockCap(CombatState combatState, bool isBossBrand)
    {
        var playerCount = combatState.Players.Count(player => player.IsActiveForHooks);
        if (isBossBrand)
        {
            return playerCount <= 1 ? 12 : playerCount == 2 ? 16 : 20;
        }

        return playerCount <= 1 ? 8 : playerCount == 2 ? 12 : 16;
    }

    private static async Task ApplySoulTidePendingBlock(CombatState combatState, AscensionCombatTracker tracker, AscensionNodeMetadata metadata)
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
        await CreatureCmd.GainBlock(soulFysh, block, ValueProp.Move, null);
        MainFile.Logger.Info($"[Spire Plus] Ascension A19 applied: Soul Tide converted Beckon hand pressure into {block} Block at player turn start.");
    }
}
