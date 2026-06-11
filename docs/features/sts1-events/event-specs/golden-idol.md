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
| Outrun | Obtain Injury curse | Same |
| Smash | Lose 25% max HP as HP damage | Lose 35% max HP as HP damage |
| Hide | Lose 8% max HP | Lose 10% max HP |

### Ascension Differences
- **A15+**: Smash damage increases from 25% to 35% max HP as HP damage.
- **A15+**: Hide max HP loss increases from 8% to 10%.

## Normal Values

| Value | Amount |
|-------|--------|
| Smash HP damage | 25% of max HP |
| Hide max HP loss | 8% of max HP |
| Outrun curse | 1 Injury added to deck |
| Take relic | Golden Idol relic obtained |

## A15 Values

| Value | Amount |
|-------|--------|
| Smash HP damage | 35% of max HP |
| Hide max HP loss | 10% of max HP |
| Outrun curse | 1 Injury (unchanged) |

## Option Table

| Page | Option | Effect | Dependencies |
|------|--------|--------|-------------|
| INITIAL | Take | `RelicCmd.Obtain(Golden Idol, Owner)` → GoToPage("TRAP") | Golden Idol relic model |
| INITIAL | Leave | `SetEventFinished(...)` | None |
| TRAP | Outrun | `CardPileCmd.AddCursesToDeck([Injury])` | Injury curse model |
| TRAP | Smash | `CreatureCmd.Damage(null, Owner, MaxHp * pct, ...)` | None |
| TRAP | Hide | `CreatureCmd.LoseMaxHp(Owner, MaxHp * pct)` | None |

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
STS1_GOLDEN_IDOL.pages.TRAP.options.OUTRUN.title
STS1_GOLDEN_IDOL.pages.TRAP.options.OUTRUN.description
STS1_GOLDEN_IDOL.pages.TRAP.options.SMASH.title
STS1_GOLDEN_IDOL.pages.TRAP.options.SMASH.description
STS1_GOLDEN_IDOL.pages.TRAP.options.HIDE.title
STS1_GOLDEN_IDOL.pages.TRAP.options.HIDE.description
STS1_GOLDEN_IDOL.pages.OUTRUN.description
STS1_GOLDEN_IDOL.pages.SMASH.description
STS1_GOLDEN_IDOL.pages.HIDE.description
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
| SmashDamagePct | DamageVar | 25% | 35% |
| HideMaxHpPct | MaxHpVar | 8% | 10% |

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
        var smashDamagePct = HasAscension(15) ? 0.35m : 0.25m;
        var hideMaxHpPct = HasAscension(15) ? 0.10m : 0.08m;

        return
        [
            new EventOption(this, Outrun, "STS1_GOLDEN_IDOL.pages.TRAP.options.OUTRUN"),
            new EventOption(this, () => Smash(smashDamagePct), "STS1_GOLDEN_IDOL.pages.TRAP.options.SMASH")
                .ThatDoesDamage(Owner.Creature.MaxHp * smashDamagePct),
            new EventOption(this, () => Hide(hideMaxHpPct), "STS1_GOLDEN_IDOL.pages.TRAP.options.HIDE")
                .ThatDecreasesMaxHp(Owner.Creature.MaxHp * hideMaxHpPct)
        ];
    }

    private async Task Outrun()
    {
        await CardPileCmd.AddCursesToDeck([ModelDb.Card<Injury>()], Owner);
        SetEventFinished(L10NLookup("STS1_GOLDEN_IDOL.pages.OUTRUN.description"));
    }

    private async Task Smash(decimal pct)
    {
        var damage = (int)(Owner.Creature.MaxHp * pct);
        await CreatureCmd.Damage(null, Owner.Creature, damage, null, null);
        SetEventFinished(L10NLookup("STS1_GOLDEN_IDOL.pages.SMASH.description"));
    }

    private async Task Hide(decimal pct)
    {
        var maxHpLoss = (int)(Owner.Creature.MaxHp * pct);
        await CreatureCmd.LoseMaxHp(Owner.Creature, maxHpLoss);
        SetEventFinished(L10NLookup("STS1_GOLDEN_IDOL.pages.HIDE.description"));
    }
}
```

## Manual Evidence Checklist

- [ ] Debug-spawn Golden Idol in Act 1, Act 2, Act 3
- [ ] Select "Leave" — verify event ends, no changes
- [ ] Select "Take" — verify Golden Idol relic obtained, TRAP page appears
- [ ] TRAP: Select "Outrun" — verify Injury curse added to deck
- [ ] TRAP: Select "Smash" — verify 25% max HP as HP damage (A15+: 35%)
- [ ] TRAP: Select "Hide" — verify 8% max HP lost (A15+: 10%)
- [ ] EN text renders correctly
- [ ] ZHS text renders correctly
- [ ] Event portrait loads
- [ ] Dynamic variables show correct % in option tooltips (damage/maxHP markers)
- [ ] A15 scaling: verify Smash 35% damage and Hide 10% max HP loss at A15+
- [ ] Save after Take, reload — Golden Idol relic persists
- [ ] Save after Outrun, reload — Injury curse persists
- [ ] Save after Smash, reload — HP loss persists
- [ ] Save after Hide, reload — max HP loss persists
- [ ] Golden Idol relic icon displays correctly

## Save/Load Notes

- Relic obtained persists after save/load.
- Curse added to deck persists after save/load.
- HP/max HP changes persist after save/load.
- Event state (current page) persists with room serialization.
- Multi-page event: if player saves on TRAP page, reload should restore to TRAP page.
