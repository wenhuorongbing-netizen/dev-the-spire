using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;

/// <summary>
/// StS1 Council of Ghosts event (Act 2): gain Apparitions but lose max HP.
/// A15: 3 Apparitions instead of 5.
/// </summary>
public sealed class Sts1CouncilOfGhosts : EventModel
{
    private const decimal MaxHpLossPct = 0.50m;
    private const int ApparitionsNormal = 5;
    private const int ApparitionsA15 = 3;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Accept, InitialOptionKey("ACCEPT")),
            new EventOption(this, null, InitialOptionKey("REFUSE"))
        };
    }

    private async Task Accept()
    {
        if (Owner is not { } owner) return;
        var count = HasA15 ? ApparitionsA15 : ApparitionsNormal;
        var apparitions = new List<CardModel>();
        for (int i = 0; i < count; i++)
            apparitions.Add(ModelDb.Card<Apparition>());
        await CardPileCmd.Add(apparitions, MegaCrit.Sts2.Core.Entities.Cards.PileType.Deck);

        var maxHpLoss = (int)(owner.Creature.MaxHp * MaxHpLossPct);
        if (maxHpLoss > 0)
        {
            await CreatureCmd.LoseMaxHp(
                new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
                owner.Creature, maxHpLoss, isFromCard: false);
        }
        SetEventFinished(L10NLookup("STS1_COUNCIL_OF_GHOSTS.pages.ACCEPT.description"));
    }
}
