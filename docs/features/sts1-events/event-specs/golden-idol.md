# Golden Idol - Event Specification

Status: source-verified / temporary-substitute / runtime-pending

## StS1 Wiki Behavior

**Acts:** 1 (Unknown room pool)
**Wiki:** https://slay-the-spire.fandom.com/wiki/Golden_Idol_%28Event%29

### Initial Page Options

| Option | Effect |
|--------|--------|
| Take | Obtain Golden Idol relic. Go to TRAP page. |
| Leave | Nothing happens. Event ends. |

### Trap Page

| Option | Effect | A15+ Change |
|--------|--------|-------------|
| Outrun | Obtain Injury curse | Same |
| Smash | Take damage equal to 25% max HP | Take damage equal to 35% max HP |
| Hide | Lose 8% max HP | Lose 10% max HP |

Current Spire Plus source does not implement the Golden Idol relic model yet. The Take branch grants a random relic as a temporary substitute, then opens the TRAP page. Treat this as non-parity until a Golden Idol relic model/effect exists.

## Current StS2 Implementation

### Class: `Sts1GoldenIdol`

- **Base:** `EventModel`
- **Registration:** `content.ActEvent<Overgrowth, Sts1GoldenIdol>()` and `content.ActEvent<Underdocks, Sts1GoldenIdol>()`
- **Layout:** Default event layout
- **LocTable:** `events`
- **IsShared:** `true` for shared co-op vote semantics
- **Temporary substitute:** `TakeIdol()` grants a random relic with `RelicFactory.PullNextRelicFromFront(owner).ToMutable()` and `RelicCmd.Obtain(...)`, then moves to the TRAP page.

## Source Values

| Value | Normal | A15+ |
|-------|--------|------|
| Smash HP damage | 25% of max HP | 35% of max HP |
| Hide max HP loss | 8% of max HP | 10% of max HP |
| Outrun curse | 1 Injury added to deck | Same |
| Take relic | Random relic temporary substitute | Same |

## Option Table

| Page | Option | Current Source Effect | Dependencies |
|------|--------|-----------------------|--------------|
| INITIAL | Take | Pull and obtain a random relic, then show TRAP page | Random relic pool; Golden Idol relic model pending |
| INITIAL | Leave | Finish event with LEAVE text | None |
| TRAP | Outrun | Add `Injury` curse to deck | Native `Injury` curse model |
| TRAP | Smash | Deal unblockable/unpowered HP damage based on max HP percent | Damage command |
| TRAP | Hide | Lose max HP based on max HP percent | Max HP loss command |

## Localization Keys

```text
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

## Runtime Evidence Checklist

- [ ] Debug-spawn or encounter Golden Idol in an Act 1 bucket.
- [ ] Select "Leave" and verify the event ends with no reward or penalty.
- [ ] Select "Take" and verify the current random relic substitute is obtained and the TRAP page appears.
- [ ] Keep Golden Idol relic parity gap open until a Golden Idol relic model/effect is implemented.
- [ ] Select "Outrun" and verify an Injury curse is added to the deck.
- [ ] Select "Smash" and verify 25% max HP as HP damage, or 35% at A15+.
- [ ] Select "Hide" and verify 8% max HP loss, or 10% at A15+.
- [ ] Verify EN text renders correctly.
- [ ] Verify ZHS text renders correctly.
- [ ] Verify option dynamic variables show correct damage/max HP values.
- [ ] Save after Take and reload; verify the current random relic substitute persists.
- [ ] Save after Outrun and reload; verify Injury persists.
- [ ] Save after Smash and reload; verify HP loss persists.
- [ ] Save after Hide and reload; verify max HP loss persists.
- [ ] Future parity check: Golden Idol relic icon displays correctly after a Golden Idol relic model/effect is implemented.

## Save/Load Notes

- Current random relic substitute persists after save/load.
- Curse additions persist after save/load.
- HP and max HP changes persist after save/load.
- Event state should persist with room serialization if saved on the TRAP page; runtime proof remains pending.

## Non-Claims

- This spec does not claim current Golden Idol relic parity.
- This spec does not prove current `v0.107.1` CanaryOnly enabled-mode spawn behavior; beta.92 AdditiveBatch1 loader proof covers registration shape only.
- This spec does not prove gameplay, save/load, EN/ZHS render, image/license, or multiplayer behavior.
