# Big Fish — Event Specification

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool)
**Wiki:** https://slay-the-spire.fandom.com/wiki/Big_Fish

### Options

| Option | Effect |
|--------|--------|
| Banana | Heal 1/3 of max HP |
| Donut | Gain 5 max HP |
| Shoe | Obtain 1 random relic. Obtain Regret curse. |

### Ascension Differences
None — same at all Ascension levels.

## StS2 Implementation

### Class: `Sts1BigFish`
- **Base:** `ModEventTemplate` (RitsuLib)
- **Registration:** `[RegisterSharedEvent]` (shared across acts)
- **Layout:** Default event layout
- **LocTable:** "events" (default)

### Localization Keys

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

### Dynamic Variables

| Variable | Type | Base | Variance |
|----------|------|------|----------|
| HealAmount | HealVar | maxHP / 3 | 0 |
| MaxHpGain | MaxHpVar | 5 | 0 |

### Dependencies
- Regret curse card (must exist or be created)
- Random relic reward helper

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

### Asset Requirements
- Portrait: `EZMicroBalance/images/events/sts1_big_fish.png`
- Source: Extract from local StS1 installation via `extract-sts1-event-assets.ps1`
