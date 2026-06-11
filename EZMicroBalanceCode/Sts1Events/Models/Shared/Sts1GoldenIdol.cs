using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Golden Idol event: take the idol (trap branch with Injury/HP damage/max HP loss)
/// or leave.
/// </summary>
public sealed class Sts1GoldenIdol : EventModel
{
    private const decimal SmashDamagePctNormal = 0.25m;
    private const decimal SmashDamagePctA15 = 0.35m;
    private const decimal HideMaxHpPctNormal = 0.08m;
    private const decimal HideMaxHpPctA15 = 0.10m;

    public override bool IsShared => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(SmashDamagePctNormal * 100m, MegaCrit.Sts2.Core.ValueProps.ValueProp.Unblockable),
        new MaxHpVar(0m)
    };

    // StS2 has no direct "UnfavorableEvents" ascension; use AscensionLevel >= 15
    // as a proxy for the StS1 A15 harder-event behavior.
    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;

    private decimal SmashDamagePct => HasA15 ? SmashDamagePctA15 : SmashDamagePctNormal;
    private decimal HideMaxHpPct => HasA15 ? HideMaxHpPctA15 : HideMaxHpPctNormal;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, TakeIdol, InitialOptionKey("TAKE")),
            new EventOption(this, Leave, InitialOptionKey("LEAVE"))
        };
    }

    private async Task TakeIdol()
    {
        if (Owner is not { } owner) return;
        var relic = RelicFactory.PullNextRelicFromFront(owner).ToMutable();
        await RelicCmd.Obtain(relic, owner);
        SetEventState(
            L10NLookup("STS1_GOLDEN_IDOL.pages.TRAP.description"),
            GenerateTrapOptions());
    }

    private Task Leave()
    {
        SetEventFinished(L10NLookup("STS1_GOLDEN_IDOL.pages.LEAVE.description"));
        return Task.CompletedTask;
    }

    private IReadOnlyList<EventOption> GenerateTrapOptions()
    {
        if (Owner is not { } owner) return System.Array.Empty<EventOption>();
        var smashDamage = (int)(owner.Creature.MaxHp * SmashDamagePct);
        var hideMaxHpLoss = (int)(owner.Creature.MaxHp * HideMaxHpPct);

        return new EventOption[]
        {
            new EventOption(this, Outrun,
                OptionKey("TRAP", "OUTRUN")),
            new EventOption(this, () => Smash(smashDamage),
                OptionKey("TRAP", "SMASH"))
                .ThatDoesDamage(smashDamage),
            new EventOption(this, () => Hide(hideMaxHpLoss),
                OptionKey("TRAP", "HIDE"))
                .ThatDecreasesMaxHp(hideMaxHpLoss)
        };
    }

    private async Task Outrun()
    {
        if (Owner is not { } owner) return;
        await CardPileCmd.AddCursesToDeck(
            new[] { ModelDb.Card<Injury>() }, owner);
        SetEventFinished(L10NLookup("STS1_GOLDEN_IDOL.pages.OUTRUN.description"));
    }

    private async Task Smash(int damage)
    {
        if (Owner is not { } owner) return;
        var damageVar = new DamageVar(
            (decimal)damage,
            MegaCrit.Sts2.Core.ValueProps.ValueProp.Unblockable | MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered);
        await CreatureCmd.Damage(
            new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
            owner.Creature, damageVar, (CardModel?)null!);
        SetEventFinished(L10NLookup("STS1_GOLDEN_IDOL.pages.SMASH.description"));
    }

    private async Task Hide(int maxHpLoss)
    {
        if (Owner is not { } owner) return;
        await CreatureCmd.LoseMaxHp(
            new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
            owner.Creature, maxHpLoss, isFromCard: false);
        SetEventFinished(L10NLookup("STS1_GOLDEN_IDOL.pages.HIDE.description"));
    }

    private string OptionKey(string pageName, string optionName)
    {
        return $"{StringHelper.Slugify(GetType().Name)}.pages.{pageName}.options.{optionName}";
    }
}
