# The Lab - Event Specification

Status: source-verified / runtime-pending

## StS1 Wiki Behavior

**Wiki acts:** 1, 2, 3 unknown-room pool (shared event)
**Wiki:** https://slay-the-spire.fandom.com/wiki/The_Lab

**Current Spire Plus registration:** shared event through `content.SharedEvent<Sts1TheLab>()`. Runtime selection proof is still pending.

### Options

| Option | Effect |
|--------|--------|
| Open | Obtain 3 random potions. At A15+, obtain 2 random potions. |

### Ascension Differences

| Ascension | Potions |
|-----------|---------|
| A0-A14 | 3 |
| A15+ | 2 |

## Option Table

| Page | Option | Effect | Dependencies |
|------|--------|--------|--------------|
| INITIAL | Open | Loop `Sts1EventHelpers.GrantRandomPotion(owner, Rng)` `HasA15 ? 2 : 3` times | Potion system |

## Dependencies

- `PotionFactory.CreateRandomPotionOutOfCombat(owner, rng)` through `Sts1EventHelpers.GrantRandomPotion`.
- `PotionCmd.TryToProcure(potion, owner)`.
- No custom models are required for this event.

## Localization Keys

```text
STS1_THE_LAB.title
STS1_THE_LAB.pages.INITIAL.description
STS1_THE_LAB.pages.INITIAL.options.OPEN.title
STS1_THE_LAB.pages.INITIAL.options.OPEN.description
STS1_THE_LAB.pages.OPEN.description
```

## Asset Path Plan

- Portrait: `EZMicroBalance/images/events/sts1_the_lab.png`
- Current tracked event images: none for StS1 events.
- Source decision: do not copy original StS art into tracked files unless redistribution permission is confirmed and documented; use a redistributable replacement or local-only extraction.

## StS2 Implementation

### Class: `Sts1TheLab`

- **Base:** `EventModel`
- **IsShared:** `true`
- **Registration:** `content.SharedEvent<Sts1TheLab>()`
- **Availability:** no event-specific `IsAllowed(IRunState)` override
- **LocTable:** event localization keys under `STS1_THE_LAB`

### Current Source Shape

```csharp
public sealed class Sts1TheLab : EventModel
{
    public override bool IsShared => true;

    private bool HasA15 => (Owner?.RunState?.AscensionLevel ?? 0) >= 15;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Open, InitialOptionKey("OPEN"))
        };
    }

    private async Task Open()
    {
        if (Owner is not { } owner) return;
        int count = HasA15 ? 2 : 3;
        for (int i = 0; i < count; i++)
            await Sts1EventHelpers.GrantRandomPotion(owner, Rng);
        SetEventFinished(L10NLookup("STS1_THE_LAB.pages.OPEN.description"));
    }
}
```

## Manual Evidence Checklist

- [ ] Current beta.91 `v0.107.1` AdditiveBatch1 loader proof exists; recapture CanaryOnly if the claim depends on CanaryOnly specifically.
- [ ] Debug-spawn or naturally encounter The Lab.
- [ ] Verify only the Open option is visible.
- [ ] Select Open at A0-A14 and verify 3 potions are procured, subject to potion-slot behavior.
- [ ] Select Open at A15+ and verify 2 potions are procured, subject to potion-slot behavior.
- [ ] EN text renders correctly.
- [ ] ZHS text renders correctly.
- [ ] Event portrait or approved placeholder renders.
- [ ] Save after obtaining potions, reload, and verify potions persist.
- [ ] Save during event, reload, and verify event state is correct.

## Save/Load Notes

- Event state persistence still requires direct runtime proof.
- Potions granted before save should persist in player inventory.
- Full potion bar behavior still requires direct runtime proof.
