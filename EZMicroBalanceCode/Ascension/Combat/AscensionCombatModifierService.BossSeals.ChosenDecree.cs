using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static void TryAssignChosenDecree(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        CardModel card)
    {
        if (metadata.BossSeal?.Id != BossSealId.ChosenDecree ||
            tracker.ChosenDecreeCard != null ||
            card.Affliction is not Bound ||
            card.Enchantment != null)
        {
            return;
        }

        CardCmd.Enchant<RoyalDecreeEnchantment>(card, 1m);
        tracker.ChosenDecreeCard = card;
        tracker.ChosenDecreePlayed = false;
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Chosen Decree marked one Bound card.");
    }

    private static void TryAssignChosenDecreeInHands(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        foreach (var card in combatState.Players
                     .Where(player => player.IsActiveForHooks)
                     .SelectMany(player => player.Piles)
                     .Where(pile => pile.Type == PileType.Hand)
                     .SelectMany(pile => pile.Cards))
        {
            TryAssignChosenDecree(combatState, tracker, metadata, card);
            if (tracker.ChosenDecreeCard != null)
            {
                return;
            }
        }
    }

    private static void TrackChosenDecreePlayed(AscensionCombatTracker tracker, CardModel card)
    {
        if (tracker.ChosenDecreeCard == card ||
            card.Enchantment is RoyalDecreeEnchantment)
        {
            tracker.ChosenDecreePlayed = true;
        }
    }

    private static async Task SettleChosenDecree(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.BossSeal?.Id != BossSealId.ChosenDecree)
        {
            return;
        }

        TryAssignChosenDecreeInHands(combatState, tracker, metadata);
        if (tracker.ChosenDecreeCard == null)
        {
            return;
        }

        var queen = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is Queen);
        var amalgam = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TorchHeadAmalgam);
        if (tracker.ChosenDecreePlayed)
        {
            if (amalgam != null)
            {
                await PowerCmd.Apply<ChosenDecreeReductionPower>(new BlockingPlayerChoiceContext(), amalgam, 1m, queen, null);
            }

            if (metadata.IsBossBrand)
            {
                foreach (var player in combatState.Players.Where(player => player.IsActiveForHooks))
                {
                    await CreatureCmd.GainBlock(player.Creature, 5m, ValueProp.Move, null, fast: true);
                }
            }

            MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Chosen Decree was obeyed.");
        }
        else
        {
            if (queen != null)
            {
                await CreatureCmd.GainBlock(queen, metadata.IsBossBrand ? 14m : 10m, ValueProp.Move, null, fast: true);
            }

            if (amalgam != null)
            {
                await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), amalgam, 1m, queen, null);
            }

            MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: missed Chosen Decree strengthened the Queen's side.");
        }

        if (tracker.ChosenDecreeCard.Enchantment is RoyalDecreeEnchantment)
        {
            CardCmd.ClearEnchantment(tracker.ChosenDecreeCard);
        }

        tracker.ChosenDecreeCard = null;
        tracker.ChosenDecreePlayed = false;
    }
}
