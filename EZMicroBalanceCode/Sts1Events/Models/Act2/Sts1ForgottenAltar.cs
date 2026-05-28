using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 Forgotten Altar event (Act 2): pray (+HP + curse), offer gold (+HP), or desecrate (relic - HP).
/// A15: reduced HP gains.
/// </summary>
public sealed class Sts1ForgottenAltar : EventModel
{
    private const int PrayMaxHpNormal = 3;
    private const int PrayMaxHpA15 = 1;
    private const int OfferCost = 50;
    private const int OfferMaxHpNormal = 5;
    private const int OfferMaxHpA15 = 3;
    private const decimal DesecrateMaxHpLossPct = 0.10m;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Pray, InitialOptionKey("PRAY")),
            new EventOption(this, Offer, InitialOptionKey("OFFER")),
            new EventOption(this, Desecrate, InitialOptionKey("DESECRATE")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Pray()
    {
        var maxHp = HasA15 ? PrayMaxHpA15 : PrayMaxHpNormal;
        await CreatureCmd.GainMaxHp(Owner.Creature, maxHp);
        await CardPileCmd.AddCursesToDeck(
            new[] { ModelDb.Card<Doubt>() }, Owner);
        SetEventFinished(L10NLookup("STS1_FORGOTTEN_ALTAR.pages.PRAY.description"));
    }

    private async Task Offer()
    {
        await PlayerCmd.GainGold(-OfferCost, Owner);
        var maxHp = HasA15 ? OfferMaxHpA15 : OfferMaxHpNormal;
        await CreatureCmd.GainMaxHp(Owner.Creature, maxHp);
        SetEventFinished(L10NLookup("STS1_FORGOTTEN_ALTAR.pages.OFFER.description"));
    }

    private async Task Desecrate()
    {
        // TODO: Grant random relic
        var maxHpLoss = (int)((Owner?.Creature.MaxHp ?? 0m) * DesecrateMaxHpLossPct);
        if (maxHpLoss > 0)
        {
            await CreatureCmd.LoseMaxHp(
                new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
                Owner.Creature, maxHpLoss, isFromCard: false);
        }
        SetEventFinished(L10NLookup("STS1_FORGOTTEN_ALTAR.pages.DESECRATE.description"));
    }
}
