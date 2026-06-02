using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;

/// <summary>
/// StS1 Golden Idol event: take the idol (trap branch with Injury/HP damage/max HP loss)
/// or leave.
/// </summary>
public sealed class Sts1GoldenIdol : EventModel
{
    private const decimal JumpPctNormal = 0.25m;
    private const decimal JumpPctA15 = 0.35m;
    private const decimal DestroyPctNormal = 0.10m;
    private const decimal DestroyPctA15 = 0.15m;

    public override bool IsShared => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(JumpPctNormal * 100m, MegaCrit.Sts2.Core.ValueProps.ValueProp.Unblockable),
        new MaxHpVar(0m)
    };

    // StS2 has no direct "UnfavorableEvents" ascension; use AscensionLevel >= 15
    // as a proxy for the StS1 A15 harder-event behavior.
    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;

    private decimal JumpPct => HasA15 ? JumpPctA15 : JumpPctNormal;
    private decimal DestroyPct => HasA15 ? DestroyPctA15 : DestroyPctNormal;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, TakeIdol, InitialOptionKey("TAKE")),
            new EventOption(this, Leave, InitialOptionKey("LEAVE"))
        };
    }

    private Task TakeIdol()
    {
        SetEventState(
            L10NLookup("STS1_GOLDEN_IDOL.pages.TRAP.description"),
            GenerateTrapOptions());
        return Task.CompletedTask;
    }

    private Task Leave()
    {
        SetEventFinished(L10NLookup("STS1_GOLDEN_IDOL.pages.LEAVE.description"));
        return Task.CompletedTask;
    }

    private IReadOnlyList<EventOption> GenerateTrapOptions()
    {
        if (Owner is not { } owner) return System.Array.Empty<EventOption>();
        var jumpDamage = (int)(owner.Creature.CurrentHp * JumpPct);
        var destroyMaxHp = (int)(owner.Creature.MaxHp * DestroyPct);

        return new EventOption[]
        {
            new EventOption(this, Smash,
                OptionKey("TRAP", "SMASH")),
            new EventOption(this, () => Jump(jumpDamage),
                OptionKey("TRAP", "JUMP"))
                .ThatDoesDamage(jumpDamage),
            new EventOption(this, () => Destroy(destroyMaxHp),
                OptionKey("TRAP", "DESTROY"))
                .ThatDecreasesMaxHp(destroyMaxHp)
        };
    }

    private async Task Smash()
    {
        if (Owner is not { } owner) return;
        await CardPileCmd.AddCursesToDeck(
            new[] { ModelDb.Card<Injury>() }, owner);
        SetEventFinished(L10NLookup("STS1_GOLDEN_IDOL.pages.SMASH.description"));
    }

    private async Task Jump(int damage)
    {
        if (Owner is not { } owner) return;
        var damageVar = new DamageVar(
            (decimal)damage,
            MegaCrit.Sts2.Core.ValueProps.ValueProp.Unblockable | MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered);
        await CreatureCmd.Damage(
            new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
            owner.Creature, damageVar, (CardModel?)null!);
        SetEventFinished(L10NLookup("STS1_GOLDEN_IDOL.pages.JUMP.description"));
    }

    private async Task Destroy(int maxHpLoss)
    {
        if (Owner is not { } owner) return;
        await CreatureCmd.LoseMaxHp(
            new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
            owner.Creature, maxHpLoss, isFromCard: false);
        SetEventFinished(L10NLookup("STS1_GOLDEN_IDOL.pages.DESTROY.description"));
    }

    private string OptionKey(string pageName, string optionName)
    {
        return $"{StringHelper.Slugify(GetType().Name)}.pages.{pageName}.options.{optionName}";
    }
}
