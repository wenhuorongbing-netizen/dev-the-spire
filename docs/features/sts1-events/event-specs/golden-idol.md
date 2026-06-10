# Golden Idol — Event Specification

Status: spec-drafted / source-verified

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool — shared event)
**Wiki:** https://slay-the-spire.fandom.com/wiki/Golden_Idol_(Event)

### Initial Page Options

| Option | Effect |
|--------|--------|
| Take | Obtain Golden Idol relic. Go to TRAP page. |
| Leave | Nothing happens. Event ends. |

### Trap Page (after taking)

| Option | Effect | A15+ Change |
|--------|--------|-------------|
| Smash | Obtain Injury curse | Same |
| Jump | Lose 25% current HP | Lose 35% current HP |
| Destroy | Lose 10% max HP | Lose 15% max HP |

### Ascension Differences
- **A15+**: Jump damage increases from 25% to 35% current HP
- **A15+**: Destroy max HP loss increases from 10% to 15%

## Normal Values

| Value | Amount |
|-------|--------|
| Jump HP loss | 25% of current HP |
| Destroy max HP loss | 10% of max HP |
| Smash curse | 1 Injury added to deck |
| Take relic | Golden Idol relic obtained |

## A15 Values

| Value | Amount |
|-------|--------|
| Jump HP loss | 35% of current HP |
| Destroy max HP loss | 15% of max HP |
| Smash curse | 1 Injury (unchanged) |

## Option Table

| Page | Option | Effect | Dependencies |
|------|--------|--------|-------------|
| INITIAL | Take | `RelicCmd.Obtain(Golden Idol, Owner)` → GoToPage("TRAP") | Golden Idol relic model |
| INITIAL | Leave | `SetEventFinished(...)` | None |
| TRAP | Smash | `CardPileCmd.AddCursesToDeck([Injury])` | Injury curse model |
| TRAP | Jump | `CreatureCmd.Damage(null, Owner, CurrentHp * pct, ...)` | None |
| TRAP | Destroy | `CreatureCmd.LoseMaxHp(Owner, MaxHp * pct)` | None |

## Dependencies

- **Golden Idol relic model**: Check if StS2 has a Golden Idol relic. If not, create custom `Sts1GoldenIdolRelic : RelicModel`.
- **Injury curse card**: Check if StS2 has `Injury`. If not, create custom `Sts1Injury : CardModel`.
- **A15 check**: `HasAscension(15)` — available in `EventModel` base class.

## Localization Key Plan

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

## Asset Path Plan

- Portrait: `EZMicroBalance/images/events/sts1_golden_idol.png`
- Source: Extract from local StS1 installation via `extract-sts1-event-assets.ps1`
- Format: 1024×600 PNG
- Phobia mode: `sts1_golden_idol_phobia_mode.png` (optional)

## StS2 Implementation

### Class: `Sts1GoldenIdol`
- **Base:** `ModEventTemplate` (RitsuLib)
- **Registration:** `content.ActEvent<Overgrowth, Sts1GoldenIdol>()` and `content.ActEvent<Underdocks, Sts1GoldenIdol>()`
- **Layout:** Default event layout
- **LocTable:** "events"

### Dynamic Variables

| Variable | Type | Base | A15+ |
|----------|------|------|------|
| JumpDamagePct | DamageVar | 25% | 35% |
| DestroyMaxHpPct | MaxHpVar | 10% | 15% |

### Code Skeleton

```csharp
[RegisterSharedEvent]
public sealed class Sts1GoldenIdol : ModEventTemplate
{
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

## Manual Evidence Checklist

- [ ] Debug-spawn Golden Idol in Act 1, Act 2, Act 3
- [ ] Select "Leave" — verify event ends, no changes
- [ ] Select "Take" — verify Golden Idol relic obtained, TRAP page appears
- [ ] TRAP: Select "Smash" — verify Injury curse added to deck
- [ ] TRAP: Select "Jump" — verify 25% current HP lost (A10+: 35%)
- [ ] TRAP: Select "Destroy" — verify 10% max HP lost (A15+: 15%)
- [ ] EN text renders correctly
- [ ] ZHS text renders correctly
- [ ] Event portrait loads
- [ ] Dynamic variables show correct % in option tooltips (damage/maxHP markers)
- [ ] A15 scaling: verify Jump 35% and Destroy 15% at A15+
- [ ] Save after Take, reload — Golden Idol relic persists
- [ ] Save after Smash, reload — Injury curse persists
- [ ] Save after Jump, reload — HP loss persists
- [ ] Save after Destroy, reload — max HP loss persists
- [ ] Golden Idol relic icon displays correctly

## Save/Load Notes

- Relic obtained persists after save/load.
- Curse added to deck persists after save/load.
- HP/max HP changes persist after save/load.
- Event state (current page) persists with room serialization.
- Multi-page event: if player saves on TRAP page, reload should restore to TRAP page.
