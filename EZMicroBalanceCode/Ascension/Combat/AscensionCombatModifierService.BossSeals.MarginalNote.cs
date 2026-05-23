using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task AddMarginalNotes(CombatState combatState, AscensionNodeMetadata metadata)
    {
        foreach (var player in combatState.Players.Where(player => player.IsActiveForHooks))
        {
            var note = combatState.CreateCard<MarginalNote>(player);
            await CardPileCmd.AddGeneratedCardToCombat(note, PileType.Discard, player, CardPilePosition.Bottom);
        }

        var demon = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is KnowledgeDemon);
        await PowerCmd.Remove(demon?.GetPower<DeepThoughtPower>());

        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Marginal Note pressure was shuffled into discard after Curse of Knowledge.");
    }

    private static void TrackKnowledgeDemonEnemyMove(CombatState combatState, AscensionCombatTracker tracker)
    {
        tracker.KnowledgeDemonCurseMoveActive = AliveEnemies(combatState)
            .Any(enemy => enemy.Monster is KnowledgeDemon &&
                enemy.Monster.NextMove.StateId == "CURSE_OF_KNOWLEDGE_MOVE");
    }

    private static async Task SettleMarginalNotes(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.BossSeal?.Id != BossSealId.MarginalNote)
        {
            return;
        }

        var demon = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is KnowledgeDemon);
        if (demon == null)
        {
            return;
        }

        if (tracker.MarginalDeepThoughtRound != combatState.RoundNumber)
        {
            tracker.MarginalDeepThoughtRound = combatState.RoundNumber;
            tracker.MarginalDeepThoughtAddedThisRound = 0;
        }

        var notesInHand = combatState.Players
            .Where(player => player.IsActiveForHooks)
            .SelectMany(player => player.Piles)
            .Where(pile => pile.Type == PileType.Hand)
            .SelectMany(pile => pile.Cards)
            .Where(card => card is MarginalNote)
            .ToList();

        if (notesInHand.Count == 0)
        {
            return;
        }

        foreach (var note in notesInHand)
        {
            await CardCmd.Exhaust(new BlockingPlayerChoiceContext(), note, skipVisuals: true);
        }

        var roundRoom = Math.Max(0, 2 - tracker.MarginalDeepThoughtAddedThisRound);
        var deepThoughtGain = Math.Min(notesInHand.Count, roundRoom);
        if (deepThoughtGain <= 0)
        {
            return;
        }

        tracker.MarginalDeepThoughtAddedThisRound += deepThoughtGain;
        await PowerCmd.Apply<DeepThoughtPower>(new BlockingPlayerChoiceContext(), demon, deepThoughtGain, demon, null);
        await ClampPowerAmount<DeepThoughtPower>(demon, metadata.IsBossBrand ? 3 : 2, demon, null);

        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: unplayed Marginal Note became Deep Thought.");
    }
}
