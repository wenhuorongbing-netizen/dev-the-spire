using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act3;

/// <summary>
/// StS1 Sensory Stone event (Act 3): choose 1 of 3 rare cards.
/// </summary>
public sealed class Sts1SensoryStone : EventModel
{
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Touch, InitialOptionKey("TOUCH")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Touch()
    {
        if (Owner is not { } owner) return;
        var options = new CardCreationOptions(
            new[] { owner.Character.CardPool },
            CardCreationSource.Other,
            CardRarityOddsType.Uniform,
            (CardModel c) => c.Rarity == CardRarity.Rare
        ).WithFlags(CardCreationFlags.NoUpgradeRoll);

        var cards = CardFactory.CreateForReward(owner, 3, options).Select(r => r.Card).ToList();
        var chosen = await CardSelectCmd.FromChooseACardScreen(
            new MegaCrit.Sts2.Core.GameActions.Multiplayer.BlockingPlayerChoiceContext(),
            cards, owner, canSkip: false);
        if (chosen != null)
            await CardPileCmd.Add(chosen, PileType.Deck);
        SetEventFinished(L10NLookup("STS1_SENSORY_STONE.pages.TOUCH.description"));
    }
}
