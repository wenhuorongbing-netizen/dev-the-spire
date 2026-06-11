# Big Fish - Event Specification

Status: source-verified / runtime-pending

## StS1 Wiki Behavior

**Wiki acts:** 1, 2, 3 unknown-room pool (shared event)
**Wiki:** https://slay-the-spire.fandom.com/wiki/Big_Fish

**Current Spire Plus registration:** StS2 Act 1 buckets only (`Overgrowth`, `Underdocks`) through `content.ActEvent<..., Sts1BigFish>()`. Runtime bucket proof is still pending.

### Options

| Option | Effect |
|--------|--------|
| Banana | Heal 1/3 of max HP |
| Donut | Gain 5 max HP |
| Box | Obtain 1 random relic + obtain Regret curse |

### Ascension Differences

None. Big Fish has the same values at all Ascension levels.

## Normal Values

| Value | Amount |
|-------|--------|
| Banana heal | `floor(MaxHp / 3)` via `DynamicVars.Heal.IntValue` |
| Donut max HP gain | 5 |
| Box relic | 1 random relic from current pool |
| Box curse | 1 Regret added to deck |

## Option Table

| Page | Option | Effect | Dependencies |
|------|--------|--------|--------------|
| INITIAL | Banana | `CreatureCmd.Heal(owner.Creature, healAmount)` | Owner creature |
| INITIAL | Donut | `CreatureCmd.GainMaxHp(owner.Creature, DynamicVars.MaxHp.BaseValue)` | Owner creature |
| INITIAL | Box | `RelicFactory.PullNextRelicFromFront(owner)` + `RelicCmd.Obtain(relic, owner)` + `CardPileCmd.AddCursesToDeck([Regret])` | Regret curse, random relic pool |

## Dependencies

- **Regret curse card**: `ModelDb.Card<Regret>()`.
- **Random relic helper**: `RelicFactory.PullNextRelicFromFront(owner).ToMutable()` followed by `RelicCmd.Obtain(relic, owner)`.
- **Registration**: `Sts1EventRegistrationService` registers Big Fish to `Overgrowth` and `Underdocks` in CanaryOnly, AdditiveBatch1, and RegisterAll.
- No custom models are required for this event.

## Localization Keys

```text
STS1_BIG_FISH.title
STS1_BIG_FISH.pages.INITIAL.description
STS1_BIG_FISH.pages.INITIAL.options.BANANA.title
STS1_BIG_FISH.pages.INITIAL.options.BANANA.description
STS1_BIG_FISH.pages.INITIAL.options.DONUT.title
STS1_BIG_FISH.pages.INITIAL.options.DONUT.description
STS1_BIG_FISH.pages.INITIAL.options.BOX.title
STS1_BIG_FISH.pages.INITIAL.options.BOX.description
STS1_BIG_FISH.pages.BANANA.description
STS1_BIG_FISH.pages.DONUT.description
STS1_BIG_FISH.pages.BOX.description
```

## Asset Path Plan

- Portrait: `EZMicroBalance/images/events/sts1_big_fish.png`
- Current tracked event images: none for StS1 events.
- Source decision: do not copy original StS art into tracked files unless redistribution permission is confirmed and documented; use a redistributable replacement or local-only extraction.

## StS2 Implementation

### Class: `Sts1BigFish`

- **Base:** `EventModel`
- **IsShared:** `true`
- **Registration:** `content.ActEvent<Overgrowth, Sts1BigFish>()` and `content.ActEvent<Underdocks, Sts1BigFish>()`
- **Availability:** no event-specific `IsAllowed(IRunState)` override
- **LocTable:** event localization keys under `STS1_BIG_FISH`

### Dynamic Variables

| Variable | Type | Base | Notes |
|----------|------|------|-------|
| Heal | `HealVar` | computed from `Owner?.Creature.MaxHp / 3m` | Display text currently uses static "1/3 max HP" wording |
| MaxHp | `MaxHpVar` | 5 | Display text currently uses static "5 max HP" wording |

### Current Source Shape

```csharp
public sealed class Sts1BigFish : EventModel
{
    public override bool IsShared => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Banana, InitialOptionKey("BANANA")),
            new EventOption(this, Donut, InitialOptionKey("DONUT")),
            new EventOption(this, Box, InitialOptionKey("BOX"))
        };
    }

    private async Task Box()
    {
        if (Owner is not { } owner) return;
        var relic = RelicFactory.PullNextRelicFromFront(owner).ToMutable();
        await RelicCmd.Obtain(relic, owner);
        await CardPileCmd.AddCursesToDeck(
            new[] { ModelDb.Card<Regret>() }, owner);
        SetEventFinished(L10NLookup("STS1_BIG_FISH.pages.BOX.description"));
    }
}
```

## Manual Evidence Checklist

- [ ] Current `v0.107.0` clean loader proof exists before gameplay proof.
- [ ] Debug-spawn or naturally encounter Big Fish from the current Act 1 buckets.
- [ ] Select "Banana" and verify HP heals by `MaxHp / 3`.
- [ ] Select "Donut" and verify max HP increases by 5.
- [ ] Select "Box" and verify relic obtained + Regret added to deck.
- [ ] EN text renders correctly.
- [ ] ZHS text renders correctly.
- [ ] Event portrait or approved placeholder renders.
- [ ] Save after each option, reload, and verify state persists.
- [ ] Save during event, reload, and verify event state is correct.
- [ ] Regret curse appears in deck view after Box.

## Save/Load Notes

- HP changes persist after save/load through player state.
- Max HP changes persist after save/load through player state.
- Relic obtained persists after save/load through relic state.
- Regret curse in deck persists after save/load through deck state.
- Event state persistence still requires direct runtime proof.
