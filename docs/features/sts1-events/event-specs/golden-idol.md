# Golden Idol — Event Specification

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool)
**Wiki:** https://slay-the-spire.fandom.com/wiki/Golden_Idol_(Event)

### Options

**Initial Page:**

| Option | Effect |
|--------|--------|
| Take | Obtain Golden Idol relic. Go to TRAP page. |
| Leave | Nothing happens. Event ends. |

**Trap Page (after taking):**

| Option | Effect | A15+ Change |
|--------|--------|-------------|
| Smash | Obtain Injury curse | Same |
| Jump | Lose 25% current HP | Lose 35% current HP |
| Destroy | Lose 10% max HP | Lose 15% max HP |

### Ascension Differences
- A15+: Jump damage increases from 25% to 35% current HP
- A15+: Destroy max HP loss increases from 10% to 15%

## StS2 Implementation

### Class: `Sts1GoldenIdol`
- **Base:** `ModEventTemplate` (RitsuLib)
- **Registration:** `[RegisterSharedEvent]` (shared across acts)
- **Layout:** Default event layout
- **LocTable:** "events" (default)

### Localization Keys

```
STS1_GOLDEN_IDOL.title
STS1_GOLDEN_IDOL.pages.INITIAL.description
STS1_GOLDEN_IDOL.pages.INITIAL.options.TAKE.title
STS1_GOLDEN_IDOL.pages.INITIAL.options.TAKE.description
STS1_GOLDEN_IDOL.pages.INITIAL.options.LEAVE.title
STS1_GOLDEN_IDOL.pages.INITIAL.options.LEAVE.description
STS1_GOLDEN_IDOL.pages.TRAP.description
STS1_GOLDEN_IDOL.pages.TRAP.options.SMASH.title
STS1_GOLDEN_IDOL.pages.TRAP.options.SMASH.description
STS1_GOLDEN_IDOL.pages.TRAP.options.JUMP.title
STS1_GOLDEN_IDOL.pages.TRAP.options.JUMP.description
STS1_GOLDEN_IDOL.pages.TRAP.options.DESTROY.title
STS1_GOLDEN_IDOL.pages.TRAP.options.DESTROY.description
STS1_GOLDEN_IDOL.pages.SMASH.description
STS1_GOLDEN_IDOL.pages.JUMP.description
STS1_GOLDEN_IDOL.pages.DESTROY.description
```

### Dynamic Variables

| Variable | Type | Base | A15+ |
|----------|------|------|------|
| JumpDamagePct | DamageVar | 25% | 35% |
| DestroyMaxHpPct | MaxHpVar | 10% | 15% |

### Dependencies
- Golden Idol relic model (check if StS2 has one; if not, create)
- Injury curse card model

### Code Skeleton

```csharp
[RegisterSharedEvent]
public sealed class Sts1GoldenIdol : ModEventTemplate
{
    private bool _tookIdol;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, TakeIdol, InitialOptionKey("TAKE")),
            new EventOption(this, Leave, InitialOptionKey("LEAVE"))
        ];
    }

    private Task TakeIdol()
    {
        _tookIdol = true;
        // Grant Golden Idol relic
        return GoToPage("TRAP");
    }

    private Task Leave()
    {
        SetEventFinished(L10NLookup("STS1_GOLDEN_IDOL.pages.LEAVE.description"));
        return Task.CompletedTask;
    }

    private IReadOnlyList<EventOption> GenerateTrapOptions()
    {
        var jumpPct = HasAscension(15) ? 0.35m : 0.25m;
        var destroyPct = HasAscension(15) ? 0.15m : 0.10m;

        return
        [
            new EventOption(this, Smash, "STS1_GOLDEN_IDOL.pages.TRAP.options.SMASH"),
            new EventOption(this, () => Jump(jumpPct), "STS1_GOLDEN_IDOL.pages.TRAP.options.JUMP")
                .ThatDoesDamage(Owner.Creature.CurrentHp * jumpPct),
            new EventOption(this, () => Destroy(destroyPct), "STS1_GOLDEN_IDOL.pages.TRAP.options.DESTROY")
                .ThatDecreasesMaxHp(Owner.Creature.MaxHp * destroyPct)
        ];
    }

    private async Task Smash()
    {
        await CardPileCmd.AddCursesToDeck([ModelDb.Card<Injury>()], Owner);
        SetEventFinished(L10NLookup("STS1_GOLDEN_IDOL.pages.SMASH.description"));
    }

    private async Task Jump(decimal pct)
    {
        var damage = (int)(Owner.Creature.CurrentHp * pct);
        await CreatureCmd.Damage(null, Owner.Creature, damage, null, null);
        SetEventFinished(L10NLookup("STS1_GOLDEN_IDOL.pages.JUMP.description"));
    }

    private async Task Destroy(decimal pct)
    {
        var maxHpLoss = (int)(Owner.Creature.MaxHp * pct);
        await CreatureCmd.LoseMaxHp(Owner.Creature, maxHpLoss);
        SetEventFinished(L10NLookup("STS1_GOLDEN_IDOL.pages.DESTROY.description"));
    }
}
```

### Asset Requirements
- Portrait: `EZMicroBalance/images/events/sts1_golden_idol.png`
- Source: Extract from local StS1 installation via `extract-sts1-event-assets.ps1`
