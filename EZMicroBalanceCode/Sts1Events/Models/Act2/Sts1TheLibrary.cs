using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 The Library event (Act 2): choose 1 of 20 cards, or rest (heal 1/3 max HP).
/// </summary>
public sealed class Sts1TheLibrary : EventModel
{
    private const int CardCount = 20;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Read, InitialOptionKey("READ")),
            new EventOption(this, Rest, InitialOptionKey("REST")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Read()
    {
        var options = new CardCreationOptions(
            new[] { Owner.Character.CardPool },
            CardCreationSource.Other,
            CardRarityOddsType.RegularEncounter
        ).WithFlags(CardCreationFlags.NoUpgradeRoll);

        var cards = CardFactory.CreateForReward(Owner, CardCount, options).Select(r => r.Card).ToList();
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 1);
        var chosen = await CardSelectCmd.FromSimpleGrid(
            new MegaCrit.Sts2.Core.GameActions.Multiplayer.BlockingPlayerChoiceContext(),
            cards, Owner, prefs);
        if (chosen != null)
        {
            foreach (var card in chosen)
                await CardPileCmd.Add(card, PileType.Deck);
        }
        SetEventFinished(L10NLookup("STS1_THE_LIBRARY.pages.READ.description"));
    }

    private async Task Rest()
    {
        var healAmount = (Owner?.Creature.MaxHp ?? 0m) / 3m;
        if (healAmount > 0)
            await CreatureCmd.Heal(Owner.Creature, healAmount);
        SetEventFinished(L10NLookup("STS1_THE_LIBRARY.pages.REST.description"));
    }
}
