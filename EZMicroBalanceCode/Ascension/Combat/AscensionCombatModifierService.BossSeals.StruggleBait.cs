using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task TrackStruggleBaitObservations(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        var insatiable = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TheInsatiable);
        if (insatiable == null)
        {
            return;
        }

        var strength = insatiable.GetPowerAmount<StrengthPower>();
        var sandpit = insatiable.Powers
            .OfType<SandpitPower>()
            .Sum(power => power.Amount);

        if (!tracker.StruggleBaitBaselineCaptured)
        {
            tracker.StruggleBaitBaselineCaptured = true;
            tracker.LastInsatiableStrengthAmount = strength;
            tracker.LastInsatiableSandpitAmount = sandpit;
            return;
        }

        var shouldAddEscape = !tracker.SuppressStruggleBaitStrengthTrigger &&
            (strength > tracker.LastInsatiableStrengthAmount ||
                sandpit > tracker.LastInsatiableSandpitAmount);

        tracker.LastInsatiableStrengthAmount = strength;
        tracker.LastInsatiableSandpitAmount = sandpit;

        if (shouldAddEscape)
        {
            await AddStruggleBaitEscape(combatState, tracker, metadata);
        }
    }

    private static async Task AddStruggleBaitEscape(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        var insatiable = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TheInsatiable);
        if (insatiable == null)
        {
            return;
        }

        foreach (var player in combatState.Players.Where(player => player.IsActiveForHooks))
        {
            var escape = combatState.CreateCard<FranticEscape>(player);
            await CardPileCmd.AddGeneratedCardToCombat(escape, PileType.Discard, player, CardPilePosition.Bottom);
            if (metadata.IsBossBrand)
            {
                tracker.StruggleBaitBrandEscapeAges[escape] = 0;
            }
        }

        if (tracker.FranticEscapesPlayed >= 3)
        {
            tracker.SuppressStruggleBaitStrengthTrigger = true;
            await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), insatiable, 1m, insatiable, null);
            tracker.SuppressStruggleBaitStrengthTrigger = false;
        }

        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Struggle Bait added Frantic Escape pressure.");
    }

    private static async Task SettleStruggleBaitBrandEscapes(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.BossSeal?.Id != BossSealId.StruggleBait ||
            !metadata.IsBossBrand ||
            tracker.StruggleBaitBrandEscapeAges.Count == 0)
        {
            return;
        }

        var combatCards = combatState.Players
            .Where(player => player.IsActiveForHooks)
            .SelectMany(player => player.Piles)
            .SelectMany(pile => pile.Cards)
            .ToHashSet();

        var maturedEscapes = new List<CardModel>();
        foreach (var card in tracker.StruggleBaitBrandEscapeAges.Keys.ToArray())
        {
            if (!combatCards.Contains(card))
            {
                tracker.StruggleBaitBrandEscapeAges.Remove(card);
                continue;
            }

            var age = tracker.StruggleBaitBrandEscapeAges[card] + 1;
            if (age >= 2)
            {
                maturedEscapes.Add(card);
            }
            else
            {
                tracker.StruggleBaitBrandEscapeAges[card] = age;
            }
        }

        if (maturedEscapes.Count == 0)
        {
            return;
        }

        foreach (var card in maturedEscapes)
        {
            tracker.StruggleBaitBrandEscapeAges.Remove(card);
        }

        var insatiable = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TheInsatiable);
        if (insatiable == null)
        {
            return;
        }

        var block = maturedEscapes.Count * 5m;
        await CreatureCmd.GainBlock(insatiable, block, ValueProp.Move, null, fast: true);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A20 applied: Struggle Bait Brand converted unplayed Frantic Escape pressure into Block.");
    }
}
