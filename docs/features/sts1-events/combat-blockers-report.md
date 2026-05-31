# Combat Events — Blocker Report

> Generated from source code inspection on 2026-05-29; v13 status alignment refreshed on 2026-05-31.
> Source of truth: `EZMicroBalanceCode/Sts1Events/Models/` and `docs/features/sts1-events/status-board.md`.

## Summary

**5 full combat events plus 1 partial option** are blocked by missing encounter/monster models. All combat blockers share the same root cause: the `EnterCombatWithoutExitingEvent` API requires a concrete `EncounterModel` (and its constituent `MonsterModel` entries) that do not yet exist. No combat encounter models have been authored for StS1 event fights.

| Metric | Count |
|--------|-------|
| Canonical audit rows | 54 |
| Fully blocked combat events | 5 |
| Partially blocked events | 1 (Mind Bloom — War option only) |
| Combat options that short-circuit | 6 |
| Encounter models available | 0 |

---

## Per-Event Blockers

### 1. Dead Adventurer (Act 1)

- **Wiki behavior**: Search a dead adventurer's body. 25% chance (50% at A15) of fighting a random Act 1 elite; 25% chance of finding a random relic; 50% chance of finding 30–50 gold.
- **Current code state** (`Sts1DeadAdventurer.cs:40`): `// TODO: Enter combat with random elite`. The elite fight branch calls `SetEventFinished` directly without entering combat. Gold and relic branches are fully functional.
- **Missing**: An encounter model that selects a random Act 1 elite for the current act. Requires either a pool of elite `EncounterModel` instances or a dynamic encounter generator.
- **IsShared**: `true` (line 16 — required for `EnterCombatWithoutExitingEvent`).
- **Unblock requires**:
  1. A mechanism to resolve a random Act 1 elite encounter at event runtime (e.g., query the act's elite encounter pool via `ActModel` or `ModelDb`).
  2. A call to `EnterCombatWithoutExitingEvent(encounter, Owner)` before `SetEventFinished`.
  3. Post-combat reward: the elite's native reward (gold + relic) should already be handled by the encounter's `ShouldGiveRewards` path.

---

### 2. Scorpion Nest (Act 1)

- **Wiki behavior**: Fight 3 Louses (the "Scorpion" enemies from StS1, which map to Louse-family monsters). Winning grants a random relic.
- **Current code state** (`Sts1ScorpionNest.cs:27`): `// TODO: Enter combat with 3 Louses, reward: random relic`. The `Investigate` method short-circuits to `SetEventFinished` with no combat.
- **Missing**: A custom `EncounterModel` defining a 3-slot encounter with Louse-type monsters.
- **IsShared**: `true` (line 14).
- **Unblock requires**:
  1. A `CustomEncounterModel` subclass (e.g., `Sts1ScorpionNestEncounter`) with 3 Louse monster slots.
  2. Monster model references: determine whether StS2 Louse monsters can be pulled from `ModelDb` or need custom `MonsterModel` wrappers.
  3. Post-combat reward: `Sts1EventHelpers.GrantRandomRelic(Owner)` called after combat victory via event flow or `ShouldGiveRewards` override.
  4. `EnterCombatWithoutExitingEvent(encounter, Owner)` call in `Investigate()`.

---

### 3. Treasure Ooze (Act 1)

- **Wiki behavior**: A large slime guards treasure. Option A: pay 50 gold for a relic peacefully. Option B: fight the slime for gold + relic. Option C: leave.
- **Current code state** (`Sts1TreasureOoze.cs:40`): `// TODO: Enter combat with large slime, reward: gold + relic`. The `Offer` path (pay 50g for relic) is fully functional. The `Fight` path short-circuits.
- **Missing**: A custom `EncounterModel` with a single large slime monster (likely a "large" variant of the Slime enemy family).
- **IsShared**: `true` (line 16).
- **Unblock requires**:
  1. A `CustomEncounterModel` subclass (e.g., `Sts1TreasureOozeEncounter`) with 1 large-slime slot.
  2. Monster model: determine StS2 equivalent for StS1's "Large Slime" or use a custom `MonsterModel`.
  3. Post-combat reward: gold amount + random relic, granted after victory.
  4. `EnterCombatWithoutExitingEvent(encounter, Owner)` call in `Fight()`.

---

### 4. Joust (Act 1)

- **Wiki behavior**: Two knights joust. Bet 50 gold on yourself or opponent; 50/50 win 200 gold. At A15, lose an additional 100 gold on failure. **No combat in StS1.**
- **Current code state** (`Sts1Joust.cs`): Fully implemented as a pure gold-bet event. No TODO comments. No combat branch exists.
- **Missing**: **Nothing — this event is NOT a combat event.** The Joust event is a gambling mechanic, not a fight.
- **IsShared**: Not overridden (defaults to `false`) — correct, since no combat is needed.
- **Unblock requires**: N/A. This event is playable as-is. If the status board entry is a reference to a planned StS2 adaptation (e.g., a Lagavulin fight replacing the joust), that would be a design decision, not a code blocker.

> **v13 note**: Earlier docs classified Joust as a combat blocker. Source inspection (`Sts1Joust.cs`) shows no combat logic; v13 status now treats it as a compiled non-combat Act 1 event.

---

### 5. The Ssssserpent (Act 1)

- **Wiki behavior**: A giant serpent offers a deal: accept 150 gold + 2 Doubt curses (3 at A15), or refuse. **In StS1, there is no combat option.**
- **Current code state** (`Sts1TheSsssserpent.cs`): Fully implemented. Accept grants 150g + Doubt curses. Refuse ends the event. No TODO comments. No combat branch.
- **Missing**: **Nothing — this event is NOT a combat event in StS1.** No combat encounter is needed.
- **IsShared**: Not overridden (defaults to `false`) — correct for a non-combat event.
- **Unblock requires**: N/A. This event is playable as-is.

> **v13 note**: Earlier docs classified The Ssssserpent as a combat blocker. Source inspection (`Sts1TheSsssserpent.cs`) shows no combat logic; v13 status now treats it as a compiled non-combat Act 1 event.

---

### 6. Masked Bandits (Act 2)

- **Wiki behavior**: Three masked bandits demand gold. Pay 75 gold to avoid a fight, or fight 3 bandits for gold + a random relic.
- **Current code state** (`Sts1MaskedBandits.cs:37`): `// TODO: Enter combat with 3 bandits, reward: gold + relic`. The `Pay` path is fully functional. The `Fight` path short-circuits.
- **Missing**: A custom `EncounterModel` defining a 3-slot encounter with bandit-type monsters.
- **IsShared**: `true` (line 16).
- **Unblock requires**:
  1. A `CustomEncounterModel` subclass (e.g., `Sts1MaskedBanditsEncounter`) with 3 bandit slots.
  2. Monster model: StS1 bandits (Bear, Pointy, Romeo) — determine StS2 equivalents or create custom `MonsterModel` wrappers.
  3. Post-combat reward: gold + random relic after victory.
  4. `EnterCombatWithoutExitingEvent(encounter, Owner)` call in `Fight()`.

---

### 7. Mysterious Sphere (Act 3)

- **Wiki behavior**: A floating energy sphere contains creatures. Open it to fight 2 Orb Walkers; winning grants a random relic. Leave to avoid.
- **Current code state** (`Sts1MysteriousSphere.cs:27`): `// TODO: Enter combat with 2 Orb Walkers, reward: random relic`. The `Open` method short-circuits.
- **Missing**: A custom `EncounterModel` defining a 2-slot encounter with Orb Walker monsters.
- **IsShared**: `true` (line 14).
- **Unblock requires**:
  1. A `CustomEncounterModel` subclass (e.g., `Sts1MysteriousSphereEncounter`) with 2 Orb Walker slots.
  2. Monster model: StS1 Orb Walker — determine StS2 equivalent or create custom `MonsterModel`.
  3. Post-combat reward: random relic after victory.
  4. `EnterCombatWithoutExitingEvent(encounter, Owner)` call in `Open()`.

---

### 8. Mind Bloom — War Option (Act 3, partial)

- **Wiki behavior**: Three choices. "I Am War": fight a random Act 1 boss for a relic. "I Am Awake": upgrade all cards. "I Am Rich": gain 999 gold + 2 Normality curses (3 at A15).
- **Current code state** (`Sts1MindBloom.cs:40-41`): `// BLOCKED: Enter combat with random Act 1 boss requires encounter model.` and `// TODO: Implement when combat encounter system is available.` The `War()` method short-circuits. `Awake()` and `Rich()` are fully functional.
- **Missing**: An encounter model that selects a random Act 1 boss encounter. Requires a pool of boss `EncounterModel` instances or dynamic resolution from the act's boss encounter list.
- **IsShared**: `true` (line 19).
- **Unblock requires**:
  1. A mechanism to resolve a random Act 1 boss encounter at event runtime (similar to Dead Adventurer's elite resolution).
  2. Boss `EncounterModel` references: determine whether StS2 Act 1 bosses can be queried from `ModelDb` or `ActModel`.
  3. Post-combat reward: random relic after victory.
  4. `EnterCombatWithoutExitingEvent(encounter, Owner)` call in `War()`.

---

## What Remains Red

The following items must be completed to make all combat events playable. They are listed in dependency order.

### 1. Combat Entry-Point Pattern (prerequisite for all)

No existing StS1 event demonstrates `EnterCombatWithoutExitingEvent`. A reference implementation is needed:

- **File**: Create a helper method in `Sts1EventHelpers` (or a new `Sts1CombatEventHelpers`) that wraps `EnterCombatWithoutExitingEvent` with post-combat reward logic.
- **API to verify**: The exact signature and behavior of `EventModel.EnterCombatWithoutExitingEvent(EncounterModel, RunStateOwner)` — confirm it exists in the StS2 event engine, what it returns, and how post-combat flow resumes.
- **Evidence needed**: Local game source under `source code/src/Core/` for the event combat entry API.

### 2. Monster Model Resolution (prerequisite for encounters)

Each encounter needs `MonsterModel` references. Two approaches:

| Approach | Description | Effort |
|----------|-------------|--------|
| **A: Query ModelDb** | Use `ModelDb.Monster<T>()` for existing StS2 monsters that match StS1 enemies | Low — if equivalents exist |
| **B: Custom MonsterModel** | Create `CustomMonsterModel` subclasses for StS1-specific enemies | High — needs monster stats, intents, movesets |

**Monsters needed**:

| Event | Monster(s) | StS2 Equivalent? |
|-------|-----------|-------------------|
| Dead Adventurer | Random Act 1 elite | Likely queryable from act elite pool |
| Scorpion Nest | 3× Louse | StS2 has Louse enemies — verify `ModelDb` entries |
| Treasure Ooze | 1× Large Slime | StS2 has Slime enemies — verify large variant |
| Masked Bandits | 3× Bandit (Bear, Pointy, Romeo) | No direct StS2 equivalent — likely custom |
| Mysterious Sphere | 2× Orb Walker | No direct StS2 equivalent — likely custom |
| Mind Bloom (War) | 1× Random Act 1 boss | Queryable from act boss pool |

### 3. Encounter Model Authoring (per event)

Create `CustomEncounterModel` subclasses following the pattern in `EzmbVakuuTrialEncounter`:

| Encounter | Slots | Monster Source | Reward Logic |
|-----------|-------|----------------|--------------|
| `Sts1DeadAdventurerEncounter` | 1 (random elite) | Act elite pool | Elite native rewards |
| `Sts1ScorpionNestEncounter` | 3 (Louses) | `ModelDb` or custom | Random relic |
| `Sts1TreasureOozeEncounter` | 1 (Large Slime) | `ModelDb` or custom | Gold + random relic |
| `Sts1MaskedBanditsEncounter` | 3 (Bandits) | Custom | Gold + random relic |
| `Sts1MysteriousSphereEncounter` | 2 (Orb Walkers) | Custom | Random relic |
| `Sts1MindBloomWarEncounter` | 1 (random Act 1 boss) | Act boss pool | Random relic |

### 4. Post-Combat Reward Integration

After combat victory, the event must grant rewards and call `SetEventFinished`. Options:

- **Option A**: Override `ShouldGiveRewards` on the encounter model and handle rewards in the encounter itself.
- **Option B**: Use the event's post-combat callback to grant rewards before finishing the event page.

The Vakuu encounter (`EzmbVakuuTrialEncounter`) uses `ShouldGiveRewards => false` and handles rewards manually — this pattern is likely correct for event combats.

### 5. Status Board Corrections

Two entries in `status-board.md` (lines 64–65) reference combat TODOs that do not exist in source code:

| Event | Status Board Says | Source Code Shows |
|-------|-------------------|-------------------|
| Joust | "Enter combat with Lagavulin" | No combat logic; pure gold-bet event |
| The Ssssserpent | "Enter combat with 3 Ssssents" | No combat logic; accept/refuse gold+curses |

These should be corrected or moved to a "potential StS2 adaptations" section if they represent future design intent.

---

## Dependency Graph

```
Combat Entry-Point Pattern (API verification + helper)
    ├── Monster Model Resolution (ModelDb query or custom models)
    │       ├── Encounter Model Authoring (per event)
    │       │       ├── Dead Adventurer
    │       │       ├── Scorpion Nest
    │       │       ├── Treasure Ooze
    │       │       ├── Masked Bandits
    │       │       ├── Mysterious Sphere
    │       │       └── Mind Bloom (War)
    │       └── Post-Combat Reward Integration
    └── Event code updates (replace TODOs with EnterCombatWithoutExitingEvent calls)
```

## Estimated Scope

| Work Item | Estimate |
|-----------|----------|
| API verification (EnterCombatWithoutExitingEvent) | 1 session |
| Sts1CombatEventHelpers reference implementation | 1 session |
| Monster model resolution (ModelDb queries) | 1 session |
| Custom MonsterModels for Bandits + Orb Walkers | 2–3 sessions |
| 6 EncounterModel subclasses | 2 sessions |
| Post-combat reward wiring | 1 session |
| Event code updates (replace TODOs) | 1 session |
| Testing + verification | 2 sessions |
| **Total** | **~10–12 sessions** |
