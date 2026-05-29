# Divine Fountain — Event Specification

Status: spec-drafted / source-verified

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool — shared event)
**Wiki:** https://slay-the-spire.fandom.com/wiki/Divine_Fountain

### Options

| Option | Effect |
|--------|--------|
| Pray | Remove all Curses from your deck. |
| Leave | Nothing happens. Event ends. |

### Ascension Differences
None — same at all Ascension levels.

## Normal Values

| Value | Amount |
|-------|--------|
| Curses removed | All curses in deck |

## A15 Values

No A15 differences.

## Option Table

| Page | Option | Effect | Dependencies |
|------|--------|--------|-------------|
| INITIAL | Pray | Remove all curses from deck | Must have at least 1 curse in deck |
| INITIAL | Leave | `SetEventFinished(...)` | None |

### Availability Condition

Divine Fountain should only appear if the player has at least one curse in their deck. This is a StS1 wiki-verified condition filter. Implementation: override `IsAllowed(IRunState)` to check for curse cards in deck.

## Dependencies

- Curse detection: `Player.Deck` must be queryable for curse-type cards
- Curse removal: `CardCmd.RemoveCard` or equivalent batch removal API
- **Condition filter**: Event must not appear if deck has 0 curses

## Localization Key Plan

```
STS1_DIVINE_FOUNTAIN.title
STS1_DIVINE_FOUNTAIN.pages.INITIAL.description
STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.PRAY.title
STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.PRAY.description
STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.LEAVE.title
STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.LEAVE.description
STS1_DIVINE_FOUNTAIN.pages.PRAY.description
```

## Asset Path Plan

- Portrait: `EZMicroBalance/images/events/sts1_divine_fountain.png`
- Source: Extract from local StS1 installation via `extract-sts1-event-assets.ps1`
- Format: 1024×600 PNG

## StS2 Implementation

### Class: `Sts1DivineFountain`
- **Base:** `ModEventTemplate` (RitsuLib)
- **Registration:** `content.SharedEvent<Sts1DivineFountain>()`
- **Layout:** Default event layout
- **LocTable:** "events"

### Code Skeleton

```csharp
[RegisterSharedEvent]
public sealed class Sts1DivineFountain : ModEventTemplate
{
    public override bool IsAllowed(IRunState runState)
    {
        // Only appear if player has curses in deck
        return Owner?.Deck?.Any(c => c.CardType == CardType.Curse) == true;
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Pray, InitialOptionKey("PRAY")),
            new EventOption(this, Leave, InitialOptionKey("LEAVE"))
        ];
    }

    private async Task Pray()
    {
        var curses = Owner.Deck.Where(c => c.CardType == CardType.Curse).ToList();
        foreach (var curse in curses)
        {
            await CardCmd.RemoveCard(curse, Owner);
        }
        SetEventFinished(L10NLookup("STS1_DIVINE_FOUNTAIN.pages.PRAY.description"));
    }

    private Task Leave()
    {
        SetEventFinished(L10NLookup("STS1_DIVINE_FOUNTAIN.pages.LEAVE.description"));
        return Task.CompletedTask;
    }
}
```

## Manual Evidence Checklist

- [ ] Debug-spawn Divine Fountain with 0 curses — verify event does NOT appear (or appears with Pray locked)
- [ ] Debug-spawn Divine Fountain with 1+ curses — verify event appears
- [ ] Select "Pray" — verify all curses removed from deck
- [ ] Select "Leave" — verify event ends cleanly, curses remain
- [ ] EN text renders correctly
- [ ] ZHS text renders correctly
- [ ] Event portrait loads
- [ ] Save after curse removal, reload — curses stay removed
- [ ] Save during event, reload — event state correct
- [ ] With Parasite, Doubt, Normality, Decay, Writhe — verify all removed

## Save/Load Notes

- Curse removal persists after save/load (deck state is serialized).
- Event state (current page) persists with room serialization.
- Condition filter (`IsAllowed`) is re-evaluated on room entry, not on save/load.
