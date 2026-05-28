using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act3;

/// <summary>
/// StS1 Winding Halls event (Act 3): Embrace Madness (Madness cards + HP loss),
/// Retreat (HP damage), or Continue On (HP loss).
/// A15: all outcomes worse.
/// </summary>
public sealed class Sts1WindingHalls : EventModel
{
    private const int MadnessNormal = 2;
    private const int MadnessA15 = 3;
    private const decimal EmbraceMaxHpLossPctNormal = 0.05m;
    private const decimal EmbraceMaxHpLossPctA15 = 0.10m;
    private const decimal RetreatDamagePctNormal = 0.20m;
    private const decimal RetreatDamagePctA15 = 0.30m;
    private const decimal ContinueMaxHpLossPctNormal = 0.10m;
    private const decimal ContinueMaxHpLossPctA15 = 0.15m;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Embrace, InitialOptionKey("EMBRACE")),
            new EventOption(this, Retreat, InitialOptionKey("RETREAT")),
            new EventOption(this, Continue, InitialOptionKey("CONTINUE"))
        };
    }

    private async Task Embrace()
    {
        // StS1 Madness curse doesn't exist in StS2; using Debt as substitute
        var curseCount = HasA15 ? MadnessA15 : MadnessNormal;
        var curses = new List<CardModel>();
        for (int i = 0; i < curseCount; i++)
            curses.Add(ModelDb.Card<Debt>());
        await CardPileCmd.AddCursesToDeck(curses, Owner);

        var maxHpLossPct = HasA15 ? EmbraceMaxHpLossPctA15 : EmbraceMaxHpLossPctNormal;
        var maxHpLoss = (int)((Owner?.Creature.MaxHp ?? 0m) * maxHpLossPct);
        if (maxHpLoss > 0)
        {
            await CreatureCmd.LoseMaxHp(
                new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
                Owner.Creature, maxHpLoss, isFromCard: false);
        }
        SetEventFinished(L10NLookup("STS1_WINDING_HALLS.pages.EMBRACE.description"));
    }

    private async Task Retreat()
    {
        var damagePct = HasA15 ? RetreatDamagePctA15 : RetreatDamagePctNormal;
        var damage = (int)((Owner?.Creature.MaxHp ?? 0m) * damagePct);
        await CreatureCmd.Damage(
            new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
            Owner.Creature, (decimal)damage,
            MegaCrit.Sts2.Core.ValueProps.ValueProp.Unblockable | MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered,
            null, null);
        SetEventFinished(L10NLookup("STS1_WINDING_HALLS.pages.RETREAT.description"));
    }

    private async Task Continue()
    {
        var maxHpLossPct = HasA15 ? ContinueMaxHpLossPctA15 : ContinueMaxHpLossPctNormal;
        var maxHpLoss = (int)((Owner?.Creature.MaxHp ?? 0m) * maxHpLossPct);
        if (maxHpLoss > 0)
        {
            await CreatureCmd.LoseMaxHp(
                new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
                Owner.Creature, maxHpLoss, isFromCard: false);
        }
        SetEventFinished(L10NLookup("STS1_WINDING_HALLS.pages.CONTINUE.description"));
    }
}
