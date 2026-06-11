# Divine Fountain — Event Specification

Status: source-guarded / runtime-pending

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool — shared event)
**Wiki:** https://slay-the-spire.fandom.com/wiki/The_Divine_Fountain

### Options

| Option | Effect |
|--------|--------|
| Drink | Remove all Curses from your deck. |
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
| INITIAL | Drink | Remove all curses from deck | Must have at least 1 curse in deck |
| INITIAL | Leave | `SetEventFinished(...)` | None |

### Availability Condition

Divine Fountain should only appear if the player has at least one curse in their deck. This is a StS1 wiki-verified condition filter. Current source overrides `IsAllowed(IRunState)` and, because the event is shared in co-op, requires every run participant to have at least one curse before the shared event can enter the pool. Runtime selection proof is still pending.

## Dependencies

- Curse detection: `Player.Deck` must be queryable for curse-type cards
- Curse removal: `CardCmd.RemoveCard` or equivalent batch removal API
- **Condition filter**: Event must not appear if the single-player deck has 0 curses; in multiplayer, every participant must have at least 1 curse for the shared event to be valid.

## Localization Key Plan

```
STS1_DIVINE_FOUNTAIN.title
STS1_DIVINE_FOUNTAIN.pages.INITIAL.description
STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.DRINK.title
STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.DRINK.description
STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.LEAVE.title
STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.LEAVE.description
STS1_DIVINE_FOUNTAIN.pages.DRINK.description
```

## Asset Path Plan

- Portrait: `EZMicroBalance/images/events/sts1_divine_fountain.png`
- Source: Extract from local StS1 installation via `extract-sts1-event-assets.ps1`
- Format: 1024×600 PNG

## StS2 Implementation

### Class: `Sts1DivineFountain`
- **Base:** `EventModel`
- **Registration:** `content.SharedEvent<Sts1DivineFountain>()`
- **Layout:** Default event layout
- **LocTable:** "events"
- **Availability:** `IsAllowed(IRunState)` checks `runState.Players` and requires `HasCurse(player)` for each player.

### Current Source Shape

```csharp
public sealed class Sts1DivineFountain : EventModel
{
    public override bool IsAllowed(IRunState runState)
    {
        foreach (var player in runState.Players)
        {
            if (!HasCurse(player))
                return false;
        }

        return runState.Players.Count > 0;
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Drink, InitialOptionKey("DRINK")),
            new EventOption(this, null, InitialOptionKey("LEAVE"))
        };
    }

    private async Task Drink()
    {
        if (Owner is not { } owner) return;
        var curses = new List<CardModel>();
        foreach (var card in owner.Deck.Cards)
        {
            if (card.Type == CardType.Curse)
                curses.Add(card);
        }
        if (curses.Count > 0)
            await CardPileCmd.RemoveFromDeck(curses, showPreview: false);
        SetEventFinished(L10NLookup("STS1_DIVINE_FOUNTAIN.pages.DRINK.description"));
    }

    private static bool HasCurse(Player player)
    {
        foreach (var card in player.Deck.Cards)
        {
            if (card.Type == CardType.Curse)
                return true;
        }

        return false;
    }
}
```

## Manual Evidence Checklist

- [ ] Debug-spawn Divine Fountain with 0 curses — verify event does NOT appear (or appears with Drink locked)
- [ ] Debug-spawn Divine Fountain with 1+ curses — verify event appears
- [ ] Select "Drink" — verify all curses removed from deck
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
