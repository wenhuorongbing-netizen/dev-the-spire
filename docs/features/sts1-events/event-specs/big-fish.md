# Big Fish — Event Specification

Status: spec-drafted / source-verified

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool — shared event)
**Wiki:** https://slay-the-spire.fandom.com/wiki/Big_Fish

### Options

| Option | Effect |
|--------|--------|
| Banana | Heal 1/3 of max HP (floor division) |
| Donut | Gain 5 max HP |
| Shoe | Obtain 1 random relic + obtain Regret curse |

### Ascension Differences
None — same at all Ascension levels.

## Normal Values

| Value | Amount |
|-------|--------|
| Banana heal | `floor(MaxHp / 3)` |
| Donut max HP gain | 5 |
| Shoe relic | 1 random relic from current pool |
| Shoe curse | 1 Regret added to deck |

## A15 Values

No A15 differences for Big Fish.

## Option Table

| Page | Option | Effect | Dependencies |
|------|--------|--------|-------------|
| INITIAL | Banana | `CreatureCmd.Heal(Owner.Creature, MaxHp / 3)` | None |
| INITIAL | Donut | `CreatureCmd.GainMaxHp(Owner.Creature, 5)` | None |
| INITIAL | Shoe | `RelicCmd.ObtainRandom(Owner)` + `CardPileCmd.AddCursesToDeck([Regret])` | Regret curse, random relic helper |

## Dependencies

- **Regret curse card**: StS2 has native `Regret` — verify `ModelDb.Card<Regret>()` compiles
- **Random relic helper**: `RelicCmd.ObtainRandom(Owner)` — available in RitsuLib/StS2 command API
- No custom models required for this event

## Localization Key Plan

```
STS1_BIG_FISH.title
STS1_BIG_FISH.pages.INITIAL.description
STS1_BIG_FISH.pages.INITIAL.options.BANANA.title
STS1_BIG_FISH.pages.INITIAL.options.BANANA.description
STS1_BIG_FISH.pages.INITIAL.options.DONUT.title
STS1_BIG_FISH.pages.INITIAL.options.DONUT.description
STS1_BIG_FISH.pages.INITIAL.options.SHOE.title
STS1_BIG_FISH.pages.INITIAL.options.SHOE.description
STS1_BIG_FISH.pages.BANANA.description
STS1_BIG_FISH.pages.DONUT.description
STS1_BIG_FISH.pages.SHOE.description
```

## Asset Path Plan

- Portrait: `EZMicroBalance/images/events/sts1_big_fish.png`
- Source: Extract from local StS1 installation via `extract-sts1-event-assets.ps1`
- Format: 1024×600 PNG
- Phobia mode: `sts1_big_fish_phobia_mode.png` (optional)

## StS2 Implementation

### Class: `Sts1BigFish`
- **Base:** `ModEventTemplate` (RitsuLib)
- **Registration:** `content.ActEvent<Overgrowth, Sts1BigFish>()` and `content.ActEvent<Underdocks, Sts1BigFish>()`
- **Layout:** Default event layout
- **LocTable:** "events"

### Dynamic Variables

| Variable | Type | Base | Variance |
|----------|------|------|----------|
| HealAmount | HealVar | `floor(MaxHp / 3)` | 0 |
| MaxHpGain | MaxHpVar | 5 | 0 |

### Code Skeleton

```csharp
[RegisterSharedEvent]
public sealed class Sts1BigFish : ModEventTemplate
{
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Banana, InitialOptionKey("BANANA")),
            new EventOption(this, Donut, InitialOptionKey("DONUT")),
            new EventOption(this, Shoe, InitialOptionKey("SHOE"))
        ];
    }

    private async Task Banana()
    {
        var healAmount = Owner.Creature.MaxHp / 3;
        await CreatureCmd.Heal(Owner.Creature, healAmount);
        SetEventFinished(L10NLookup("STS1_BIG_FISH.pages.BANANA.description"));
    }

    private async Task Donut()
    {
        await CreatureCmd.GainMaxHp(Owner.Creature, 5);
        SetEventFinished(L10NLookup("STS1_BIG_FISH.pages.DONUT.description"));
    }

    private async Task Shoe()
    {
        await RelicCmd.ObtainRandom(Owner);
        await CardPileCmd.AddCursesToDeck([ModelDb.Card<Regret>()], Owner);
        SetEventFinished(L10NLookup("STS1_BIG_FISH.pages.SHOE.description"));
    }
}
```

## Manual Evidence Checklist

- [ ] Debug-spawn Big Fish in Act 1, Act 2, Act 3
- [ ] Select "Banana" — verify HP heals to `MaxHp / 3` (floor)
- [ ] Select "Donut" — verify max HP increases by 5
- [ ] Select "Shoe" — verify relic obtained + Regret added to deck
- [ ] EN text renders correctly
- [ ] ZHS text renders correctly
- [ ] Event portrait loads
- [ ] Dynamic variables show correct values in option tooltips
- [ ] Save after each option, reload — state persists
- [ ] Save during event, reload — event state correct
- [ ] Regret curse appears in deck view after Shoe

## Save/Load Notes

- HP changes persist after save/load (player state is serialized).
- Max HP changes persist after save/load.
- Relic obtained persists after save/load.
- Regret curse in deck persists after save/load.
- Event state (current page) persists with room serialization.
