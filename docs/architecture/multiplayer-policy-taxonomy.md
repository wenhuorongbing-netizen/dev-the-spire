# Multiplayer Policy Taxonomy

## Purpose

Classify every Spire Plus feature by its multiplayer safety category. This taxonomy extends the existing `MultiplayerFeaturePolicy` operational gates with a formal classification system for documentation, review, and release decisions.

## Existing Infrastructure

`MultiplayerFeaturePolicy` (238 lines across 2 partial files) provides:
- `IsSingleplayer`, `IsHost`, `IsClient`, `CanMutateSharedRunState` — net mode queries
- `ShouldDisableUnverifiedCoopFeature` — hard disable for unverified features
- `ShouldDisableUnverifiedCoopGameplay` — disable with env override for gameplay
- `ShouldDisableUnverifiedCoopCombatHook` — disable with env override for combat hooks
- `LogCoopEvidence` — evidence logging for manual co-op testing

63 call sites across the codebase use these gates.

## Categories

### LocalUiOnly

**Rule**: Changes only local UI presentation. No gameplay state is read or mutated.

| Feature | Current gate | Evidence |
| --- | --- | --- |
| Crystal Sphere preview mask/button | None needed (pure UI) | Visual only |
| Transform preview prediction | None needed (forked RNG, no mutation) | Read-only |
| Map hover/icon composition | None needed (overlay only) | Visual only |
| Mod Settings UI | None needed (local config) | Config only |
| Localization display | None needed (text only) | Text only |

**Co-op safe**: Yes, always. No state is shared.

### LocalPlayerOnly

**Rule**: Mutates only the local player's state. Does not affect other players, shared run state, or combat synchronization.

| Feature | Current gate | Evidence |
| --- | --- | --- |
| Seed Bank extraction | `ShouldDisableUnverifiedCoopGameplay` | Deck mutation, local player only |
| Rootblight deck management | `ShouldDisableUnverifiedCoopGameplay` | Deck mutation, local player only |
| Forge Token visible relic | `ShouldDisableUnverifiedCoopGameplay` | Relic UI, local player only |
| Ascension preference storage | `ShouldDisableUnverifiedCoopFeature` | Local preference only |
| SavedSpireField read/write | None needed (local save fields) | Per-player save data |

**Co-op safe**: Yes for host (who owns the local player). Requires verification for client.

### HostAuthoritative

**Rule**: Mutates shared state but only from the host path. Client requests must be rejected or forwarded to host.

| Feature | Current gate | Evidence |
| --- | --- | --- |
| Ascension A11-A20 selection | `ShouldDisableUnverifiedCoopFeature` | Lobby state, host-only |
| A20 dual boss chain | `ShouldDisableUnverifiedCoopFeature` | Run state, host-only |
| Reward alternative injection | `ShouldDisableUnverifiedCoopFeature` | Reward screen, host-only |
| Boss seal application | `ShouldDisableUnverifiedCoopFeature` | Run state, host-only |

**Co-op safe**: Yes, when host-only execution is enforced. Client must not mutate.

### SharedRunState

**Rule**: Reads or mutates state shared across all players in a run. Requires synchronization through game commands.

| Feature | Current gate | Evidence |
| --- | --- | --- |
| Root Eyes map preview | `ShouldDisableUnverifiedCoopFeature` | Map state, shared |
| Root Eyes room commit | `ShouldDisableUnverifiedCoopFeature` | Map navigation, shared |
| Deep Branch treasure | `ShouldDisableUnverifiedCoopFeature` | Room rewards, shared |
| A20 courtyard transition | `ShouldDisableUnverifiedCoopFeature` | Act progression, shared |
| Morvi debt settlement | `ShouldDisableUnverifiedCoopGameplay` | Player HP/gold, potentially shared |

**Co-op safe**: Requires explicit two-client proof before enabling. Current default: disabled.

### CombatCommandReplicated

**Rule**: Affects combat state through the game's combat command system. Commands are replicated to all clients automatically.

| Feature | Current gate | Evidence |
| --- | --- | --- |
| Lotha card cost modification | `ShouldDisableUnverifiedCoopCombatHook` | Card model, replicated |
| Lotha power amount modification | `ShouldDisableUnverifiedCoopCombatHook` | Power model, replicated |
| Lotha extra play count | `ShouldDisableUnverifiedCoopCombatHook` | Card play, replicated |
| Morvi energy cost modification | `ShouldDisableUnverifiedCoopCombatHook` | Card model, replicated |
| Banner combat effects | `ShouldDisableUnverifiedCoopCombatHook` | Power application, replicated |
| Firemark combat windows | `ShouldDisableUnverifiedCoopCombatHook` | Power tracking, replicated |
| Ascension combat modifiers | `ShouldDisableUnverifiedCoopCombatHook` | Combat powers, replicated |

**Co-op safe**: Likely yes (commands are replicated), but needs two-client proof for each hook.

### UnsafeInMultiplayer

**Rule**: Cannot safely run in multiplayer under any current design. Must be disabled or redesigned.

| Feature | Current gate | Evidence |
| --- | --- | --- |
| Vakuu child combat flow | `ShouldDisableUnverifiedCoopFeature` | Complex room stack, untested |
| Direct state mutation in patches | N/A (should not exist) | Forbidden by patch rules |
| Reward bypass without vanilla authority | N/A (should not exist) | Forbidden by patch rules |

**Co-op safe**: No. Disabled by default with no env override.

## Env Override Map

| Override variable | Effect | Category affected |
| --- | --- | --- |
| `SPIREPLUS_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS=1` | Enable unverified combat hooks in co-op | CombatCommandReplicated |
| `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY=1` | Enable unverified gameplay mutations in co-op | LocalPlayerOnly, SharedRunState |
| (none) | No override available | HostAuthoritative, UnsafeInMultiplayer |

## Evidence Requirements

| Category | Source guard | Manual evidence | Release gate |
| --- | --- | --- | --- |
| LocalUiOnly | Optional | Visual verification | None |
| LocalPlayerOnly | Recommended | Single-client playtest | None |
| HostAuthoritative | Required | Host+client playtest | Two-client proof |
| SharedRunState | Required | Two-client playtest with save/load | Two-client proof + save/load |
| CombatCommandReplicated | Required | Two-client combat playtest | Two-client proof |
| UnsafeInMultiplayer | N/A | N/A | Disabled until redesigned |

## Migration Guide

When migrating a patch to RitsuLib:

1. Classify the patch into one of the 6 categories above.
2. Add the appropriate `ShouldDisableUnverified*` gate call.
3. Add a source guard test verifying the gate is present.
4. Document the category in `docs/patch-inventory.md`.
5. If higher than LocalPlayerOnly, add an evidence row to `patch-boundaries.md`.

## References

- `EZMicroBalanceCode/Ascension/Core/MultiplayerFeaturePolicy.cs` — operational gates
- `EZMicroBalanceCode/Ascension/Core/MultiplayerFeaturePolicy.CoopGates.cs` — co-op gating logic
- `docs/architecture/patch-boundaries.md` — high-risk surface owners
- `docs/architecture/bounded-contexts.md` — context boundaries and dependency rules
- `tests/EZMicroBalance.Tests/MultiplayerPolicyGuardTests.cs` — guard tests
