using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed partial class RootBudCombatHook
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        var tracker = GetTracker(state);
        await AscensionCombatModifierService.AfterPlayerTurnStart(state, tracker);
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        await AscensionCombatModifierService.AfterDamageReceived(state, GetTracker(state), target, result, props, dealer, cardSource);
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        await AscensionCombatModifierService.AfterCurrentHpChanged(state, GetTracker(state), creature, delta);
    }

    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        await AscensionCombatModifierService.AfterShuffle(state, GetTracker(state), shuffler);
    }

    public override async Task BeforeFlush(PlayerChoiceContext choiceContext, Player player)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        await AscensionCombatModifierService.BeforeFlush(state, GetTracker(state), player);
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, ICombatState combatState)
    {
        if (combatState is not CombatState state)
        {
            return;
        }

        await AscensionCombatModifierService.BeforeSideTurnStart(state, GetTracker(state), side);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        await AscensionCombatModifierService.AfterTurnEnd(state, GetTracker(state), side);
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        var state = CurrentCombatState();
        if (state == null || wasRemovalPrevented)
        {
            return;
        }

        await AscensionCombatModifierService.AfterDeath(state, GetTracker(state), creature, wasRemovalPrevented);

        if (creature.Player == null)
        {
            return;
        }

        GetTracker(state).DiedPlayers.Add(creature.Player);
        MainFile.Logger.Info("[EZMicroBalance] Ascension Blight Sprout tracked: player death clears combat-only Blight Sprout growth.");
    }
}
