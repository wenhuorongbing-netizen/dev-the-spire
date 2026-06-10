using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Fountain of Cleansing event: remove all curses but lose max HP, or leave.
/// </summary>
public sealed class Sts1FountainOfCleansing : EventModel
{
    private const decimal MaxHpLossPctNormal = 0.10m;
    private const decimal MaxHpLossPctA15 = 0.15m;

    public override bool IsShared => true;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;
    private decimal MaxHpLossPct => HasA15 ? MaxHpLossPctA15 : MaxHpLossPctNormal;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Drink, InitialOptionKey("DRINK")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Drink()
    {
        if (Owner is not { } owner) return;
        var curses = new List<CardModel>();
        foreach (var card in owner.Deck.Cards)
        {
            if (card.Type == MegaCrit.Sts2.Core.Entities.Cards.CardType.Curse)
                curses.Add(card);
        }
        if (curses.Count > 0)
            await CardPileCmd.RemoveFromDeck(curses, showPreview: false);

        var maxHpLoss = (int)(owner.Creature.MaxHp * MaxHpLossPct);
        if (maxHpLoss > 0)
        {
            await CreatureCmd.LoseMaxHp(
                new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
                owner.Creature, maxHpLoss, isFromCard: false);
        }
        SetEventFinished(L10NLookup("STS1_FOUNTAIN_OF_CLEANSING.pages.DRINK.description"));
    }
}
