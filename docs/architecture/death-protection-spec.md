# Death Protection Service — Spec

## Purpose

Document the existing death-protection lifecycle in Spire Plus and define the contract for a future `DeathProtectionService` seam. This spec covers the Lotha Death Reprieve feature, forced unavoidable death, the `inReprieve` flag, and co-op owner attribution.

## Current Implementation

Death protection is implemented across several partial classes in `LothaBlessingService`:

| File | Responsibility |
| --- | --- |
| `LothaBlessingService.DeathReprieve.cs` | `ShouldDie`, `ShouldDieLate`, `AfterPreventingDeath` — entry points from combat hooks |
| `LothaBlessingService.DeathReprieveState.cs` | `HydrateDeathReprieveState`, `ResolveDeathReprieveProgress` — deck-mirrored phase state |
| `LothaBlessingService.DeathReprieveTurn.cs` | `StartDeathReprieveTurn`, `EnsureDeathReprievePower`, `ResolveDeathReprieveTurnEnd` — reprieve turn lifecycle |
| `LothaCombatHook.cs` | Combat hook adapter — delegates to `LothaBlessingService` |
| `LothaPowers.cs` | `LothaDeathReprievePower` — visual buff applied during reprieve turn |

### Lifecycle

```
Player takes lethal damage
  |
  v
ShouldDie(creature) called
  |-> If not player, or not DeathReprieve blessing: return true (die)
  |-> Hydrate state from deck mirror
  |-> If DeathReprieveActive or DeathReprievePendingStart: return false (already in reprieve)
  |-> If DeathReprieveUsed: return true (reprieve spent, die)
  |
  v
AfterPreventingDeath(creature) called
  |-> SetCurrentHp(creature, 1) — always 1 HP
  |-> If current player turn: enter Active phase immediately
  |     -> StartDeathReprieveTurn: draw 10, gain 10 energy
  |-> If enemy/non-player turn: enter PendingStart phase
  |     -> Set DeathReprievePendingStart = true, apply power
  |     -> Reprieve starts at next AfterPlayerTurnStartEarly
  |
  v
Reprieve turn plays out (10 cards, 10 energy)
  |
  v
ResolveDeathReprieveTurnEnd called
  |-> Remove power
  |-> If enemies alive: kill player (forced death)
  |-> If enemies dead: continue run
```

### State Mirrors

Death Reprieve state survives save/load through deck-mirrored blessing progress:

- `Progress.DeathReprieveUsed` (bool) — whether the reprieve has been consumed
- `Progress.DeathReprievePhase` (enum: None, PendingStart, Active, Resolved) — current lifecycle phase
- `LothaCombatState.DeathReprieveActive` (bool) — hydrated from progress on combat start
- `LothaCombatState.DeathReprievePendingStart` (bool) — hydrated from progress on combat start
- `LothaCombatState.DeathReprieveStarted` (bool) — whether the reprieve turn has begun

### inReprieve Flag

The "in reprieve" state is determined by:

```csharp
combatState.DeathReprieveActive || combatState.DeathReprievePendingStart
```

When either flag is true:
- `ShouldDie` returns false (player cannot die)
- `ShouldDieLate` returns false (player cannot die from late lethal checks)
- Duplicate lethal damage during reprieve keeps player at 1 HP

When both flags are false and `DeathReprieveUsed` is true:
- `ShouldDie` returns true (reprieve spent, normal death)

### Forced Unavoidable Death

After the reprieve turn ends, if enemies are still alive:

```csharp
ResolveDeathReprieveTurnEnd → kill player
```

This is the only path where death is forced after reprieve protection. The `forcedDeath` parameter in `DeathReprieveDiagnostics` tracks this case for evidence logging.

### Co-op Owner Attribution

Current co-op behavior:
- `LothaCombatHook` checks `LothaRunHook.ShouldSkipCoopCombat(runState)` before every hook
- If co-op and not overridden, all Lotha combat hooks are no-ops
- Death protection does NOT apply in co-op unless `SPIREPLUS_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS=1`

Future `DeathProtectionService` must:
1. Attribute death protection to the owning player in multiplayer
2. Not protect non-owner players unless explicitly designed for shared protection
3. Log co-op evidence when death protection activates in multiplayer

## Future DeathProtectionService Contract

When extracted into a dedicated service:

```csharp
internal interface IDeathProtectionProvider
{
    /// <summary>
    /// Whether this provider should prevent death for the given creature.
    /// </summary>
    bool ShouldPreventDeath(Creature creature);

    /// <summary>
    /// Called after death is prevented. Must set HP and enter any required state.
    /// </summary>
    Task AfterPreventingDeath(Creature creature);

    /// <summary>
    /// Whether the creature is currently in a reprieve state.
    /// </summary>
    bool IsInReprieve(Creature creature);

    /// <summary>
    /// Whether forced unavoidable death should bypass this provider.
    /// </summary>
    bool CanBypassForcedDeath { get; }
}
```

### Design Rules

1. **Provider priority**: Multiple providers may exist. First-match wins.
2. **Forced death bypass**: Some deaths (e.g., reprieve turn ending with enemies alive) must bypass protection.
3. **Co-op ownership**: Only the death-protected player's provider should activate.
4. **State hydration**: Providers must hydrate state from save-compatible mirrors, not in-memory only.
5. **Evidence logging**: Every death prevention must log to `ReleaseEvidenceLog`.

## Acceptance Criteria

- [ ] `ShouldDie` and `ShouldDieLate` return correct values for all phase combinations
- [ ] Duplicate lethal damage during reprieve keeps player at 1 HP
- [ ] Reprieve turn draws 10 cards and grants 10 energy
- [ ] Forced death after reprieve turn is not preventable
- [ ] Co-op death protection is gated behind env override
- [ ] Save/load preserves DeathReprieve phase across reload
- [ ] Guard tests verify lifecycle state transitions

## Related Tests

- `LothaDeathReprieveGuardTests` — source-level guard tests for the existing implementation
- `MultiplayerPolicyGuardTests` — co-op gating for combat hooks
