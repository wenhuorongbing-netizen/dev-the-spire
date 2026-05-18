using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task AddMarginalNotes(CombatState combatState, AscensionNodeMetadata metadata)
    {
        foreach (var player in combatState.Players.Where(player => player.IsActiveForHooks))
        {
            var noteCount = metadata.IsBossBrand ? 2 : 1;
            for (var index = 0; index < noteCount; index++)
            {
                var note = combatState.CreateCard<MarginalNote>(player);
                await CardPileCmd.AddGeneratedCardToCombat(note, PileType.Discard, player, CardPilePosition.Bottom);
            }
        }

        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Marginal Note pressure was shuffled into discard after Curse of Knowledge.");
    }

    private static void TrackKnowledgeDemonEnemyMove(CombatState combatState, AscensionCombatTracker tracker)
    {
        tracker.KnowledgeDemonCurseMoveActive = AliveEnemies(combatState)
            .Any(enemy => enemy.Monster is KnowledgeDemon &&
                enemy.Monster.NextMove.StateId == "CURSE_OF_KNOWLEDGE_MOVE");
    }

    private static async Task SettleMarginalNotes(CombatState combatState, AscensionNodeMetadata metadata)
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

        var notesInHand = combatState.Players
            .Where(player => player.IsActiveForHooks)
            .SelectMany(player => player.Piles)
            .Where(pile => pile.Type == PileType.Hand)
            .SelectMany(pile => pile.Cards)
            .Count(card => card is MarginalNote);

        if (notesInHand > 0)
        {
            await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), demon, notesInHand, demon, null);
            MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: unplayed Marginal Note granted Knowledge Demon Strength.");
        }
    }
}
