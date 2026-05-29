# The Lab — Event Specification

Status: spec-drafted / source-verified

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool — shared event)
**Wiki:** https://slay-the-spire.fandom.com/wiki/The_Lab

### Options

| Option | Effect |
|--------|--------|
| Open | Obtain 3 random potions. |
| Leave | Nothing happens. Event ends. |

### Ascension Differences
None — same at all Ascension levels.

## Normal Values

| Value | Amount |
|-------|--------|
| Potions granted | 3 |
| Potion pool | Random from current act potion pool |

## A15 Values

No A15 differences.

## Option Table

| Page | Option | Effect | Dependencies |
|------|--------|--------|-------------|
| INITIAL | Open | `PotionCmd.TryToProcure(Owner)` ×3 | Potion system |
| INITIAL | Leave | `SetEventFinished(...)` | None |

## Dependencies

- `PotionCmd.TryToProcure` — available in StS2 command API
- Potion pool system — must have at least 1 potion in current act pool

## Localization Key Plan

```
STS1_THE_LAB.title
STS1_THE_LAB.pages.INITIAL.description
STS1_THE_LAB.pages.INITIAL.options.OPEN.title
STS1_THE_LAB.pages.INITIAL.options.OPEN.description
STS1_THE_LAB.pages.INITIAL.options.LEAVE.title
STS1_THE_LAB.pages.INITIAL.options.LEAVE.description
STS1_THE_LAB.pages.OPEN.description
```

## Asset Path Plan

- Portrait: `EZMicroBalance/images/events/sts1_the_lab.png`
- Source: Extract from local StS1 installation via `extract-sts1-event-assets.ps1`
- Format: 1024×600 PNG

## StS2 Implementation

### Class: `Sts1TheLab`
- **Base:** `ModEventTemplate` (RitsuLib)
- **Registration:** `content.SharedEvent<Sts1TheLab>()`
- **Layout:** Default event layout
- **LocTable:** "events"

### Code Skeleton

```csharp
[RegisterSharedEvent]
public sealed class Sts1TheLab : ModEventTemplate
{
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Open, InitialOptionKey("OPEN")),
            new EventOption(this, Leave, InitialOptionKey("LEAVE"))
        ];
    }

    private async Task Open()
    {
        await PotionCmd.TryToProcure(Owner);
        await PotionCmd.TryToProcure(Owner);
        await PotionCmd.TryToProcure(Owner);
        SetEventFinished(L10NLookup("STS1_THE_LAB.pages.OPEN.description"));
    }

    private Task Leave()
    {
        SetEventFinished(L10NLookup("STS1_THE_LAB.pages.LEAVE.description"));
        return Task.CompletedTask;
    }
}
```

## Manual Evidence Checklist

- [ ] Debug-spawn The Lab in Act 1, Act 2, Act 3
- [ ] Select "Open" — verify 3 potions appear in potion bar
- [ ] Select "Leave" — verify event ends cleanly
- [ ] EN text renders correctly
- [ ] ZHS text renders correctly
- [ ] Event portrait loads
- [ ] Save after obtaining potions, reload — potions persist
- [ ] Save during event, reload — event state correct
- [ ] With full potion bar — verify behavior (overwrite or block)

## Save/Load Notes

- Event state (current page) persists with room serialization.
- Potions granted before save persist in player inventory.
- If player saves mid-event (after opening but before seeing all potions), reload should restore to event page.
