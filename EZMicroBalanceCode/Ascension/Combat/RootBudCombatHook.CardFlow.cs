using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed partial class RootBudCombatHook
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (combatState is not CombatState state)
        {
            return;
        }

        if (!IsGameplayEnabledForCurrentRoom(state))
        {
            return;
        }

        await SproutDueBudsBeforeHandDraw(state, player);
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        var tracker = GetTracker(state);
        if (card.Pile?.Type == PileType.Hand)
        {
            if (await UrdaBlessingService.TryCatchSeedbedCardFromHand(card, "card entered hand"))
            {
                return;
            }

            await AscensionCombatModifierService.AfterCardEnteredHand(state, tracker, card);
        }

        if (!IsGameplayEnabledForCurrentRoom(state))
        {
            return;
        }

        if (card is RootBud bud)
        {
            tracker.Buds.Add(bud);
            if (bud.Pile?.Type == PileType.Hand)
            {
                MarkEnteredHand(state, bud);
            }
        }
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        var tracker = GetTracker(state);
        if (IsGameplayEnabledForCurrentRoom(state) &&
            card is RootBud bud)
        {
            tracker.Buds.Add(bud);
            if (card.Pile?.Type == PileType.Hand &&
                !bud.PlantedInSeedbed)
            {
                MarkEnteredHand(state, bud);
            }
        }

        if (card.Pile?.Type == PileType.Hand &&
            !await UrdaBlessingService.TryCatchSeedbedCardFromHand(card, "card drawn"))
        {
            await AscensionCombatModifierService.AfterCardEnteredHand(state, tracker, card);
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var state = CurrentCombatState();
        if (state == null)
        {
            return;
        }

        var tracker = GetTracker(state);
        if (IsGameplayEnabledForCurrentRoom(state) &&
            cardPlay.Card is RootBud bud)
        {
            tracker.Buds.Add(bud);
            bud.WasPlayed = true;
            MainFile.Logger.Info("[EZMicroBalance] Ascension Blight Sprout tracked: played before combat end.");
        }

        await AscensionCombatModifierService.AfterCardPlayed(state, tracker, cardPlay);
    }
}
