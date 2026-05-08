# A11-A20 Multiplayer Test Runbook

Date: 2026-05-08  
Scope: private-beta multiplayer test candidate for EZ Micro Balance Ascension 11-20 selection and source-patched gameplay slices.

A11-A20 selection is now default-on in this private-beta multiplayer test candidate.

Do not treat this runbook as release evidence until results are filled in from real Steam-client multiplayer testing. Controlled smoke passed is not the same as normal Steam-client Mod Settings or live co-op verification.

## Recommended Multiplayer Setup

Best release test setup:

- Two physical PCs.
- Two Steam accounts.
- Both own and can launch Slay the Spire 2.
- Same game branch and game version/date.
- Same EZ Micro Balance package hash.
- Same BaseLib runtime version and files under `<GameRoot>\mods\BaseLib`.
- Same enabled mod set: BaseLib plus EZ Micro Balance only unless a row explicitly tests compatibility.

Same-PC multi-open is not reliable for real Steam multiplayer and should not be the primary release test. It can be useful for rough local investigation only if Steam permits it, but it does not replace the two-PC matrix.

`--force-steam off` is valid for controlled loader smoke only. It is not a replacement for real multiplayer lobby, Steam-client Mod Settings, save/load, or co-op desync testing.

## Environment Variables

Default test after this change:

```text
No Ascension environment variable is needed.
```

Gate-off comparison:

```text
EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1
```

Multiplayer-only disable comparison:

```text
EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1
```

Optional diagnostics:

```text
EZMB_ASCENSION_DIAGNOSTICS=1
```

Legacy compatibility:

```text
EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1
```

`EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.

## Windows Env Var Commands

PowerShell user env set:

```powershell
[Environment]::SetEnvironmentVariable('EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION','1','User')
[Environment]::SetEnvironmentVariable('EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION','1','User')
[Environment]::SetEnvironmentVariable('EZMB_ASCENSION_DIAGNOSTICS','1','User')
```

PowerShell user env clear:

```powershell
[Environment]::SetEnvironmentVariable('EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION',$null,'User')
[Environment]::SetEnvironmentVariable('EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION',$null,'User')
[Environment]::SetEnvironmentVariable('EZMB_ASCENSION_DIAGNOSTICS',$null,'User')
```

After changing User env vars, fully restart Steam and the game on the affected machine before testing.

## Exact Multiplayer Test Matrix

### Mod Settings / Load Checks

- Launch through the normal Steam client on host and client.
- Confirm BaseLib appears in Mod Settings on both machines.
- Confirm BaseLib is enabled on both machines.
- Confirm EZ Micro Balance appears as `EZMicroBalance` on both machines.
- Confirm EZ Micro Balance is enabled on both machines.
- Confirm legacy `EzDailyContent` is disabled or absent on both machines.
- Confirm both machines use the same package hash and same BaseLib version.
- Inspect both `godot.log` files for startup errors, missing localization keys, `CanonicalModelException`, and EZ Micro Balance exceptions.

### Gate Default-On Checks

- With no Ascension env vars on host or client, open single-player character select and confirm A11-A20 can be selected.
- With no Ascension env vars, host a multiplayer lobby and confirm host can select A11-A20.
- Confirm client sees the host-selected A11-A20 value after joining.
- Confirm leaving and reopening the lobby does not write A11-A20 into vanilla preferred Ascension paths.

### Gate-Off Comparison Checks

- Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` on the host.
- Fully restart Steam and the game.
- Confirm single-player selection returns to vanilla A1-A10.
- Confirm host-multiplayer selection returns to vanilla A1-A10.
- Clear the env var, fully restart, and confirm A11-A20 selection is available again.

### Multiplayer-Only Disable Checks

- Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` on the host.
- Fully restart Steam and the game.
- Confirm single-player A11-A20 selection remains available.
- Confirm host-multiplayer selection returns to the vanilla cap.
- Clear the env var, fully restart, and confirm host-multiplayer A11-A20 selection is available again.

### Multiplayer Selection Checks

- Host creates a lobby, selects A11, and client joins.
- Confirm client view does not clamp host selection back to A10.
- Repeat for A12, A14, A16, and A20.
- Confirm `godot.log` has no progress-save warnings from writing A11-A20 to vanilla preferred progress.

### A11 Map Checks

- Start a two-player A11 run.
- Confirm map width increases by one column.
- Confirm Act 1 gains +1 route row, Act 2 gains +1 route row, and Act 3 gains +2 route rows when reached.
- Confirm ordinary route nodes look vanilla.
- Confirm no A11-specific marker, icon, or hover tooltip appears.
- Confirm both host and client see consistent route geometry and no route desync.

### A12 Firemarked Elite Marker Checks

- Start or continue a two-player A12+ run.
- Confirm Firemarked Elite marker appears before route commitment.
- Confirm host and client agree on marked elite nodes.
- Enter a Firemarked Elite and confirm one visible Firemark Host receives the Firemark.
- Confirm defeating the Firemarked Elite grants Forge Token behavior only to the intended player state.

### A16 Banner Marker / Hover Checks

- Start or continue a two-player A16+ run.
- Confirm Banner marker appears before route commitment.
- Confirm hover text names the Banner rule without raw localization keys.
- Confirm host and client agree on the marked node.
- Enter the Banner Room and confirm the combat rule applies only to that combat.

### A14/A15/A18 Rootblight / Blight Sprout Ownership Checks

- Start or continue a two-player A14+ run.
- Confirm each player gets only their own Rootblight state/card.
- One player plays Rootblight; confirm only that player's master-deck state changes.
- Start A15 Act 2/3 Boss combat and confirm Blight Sprouts are player-owned.
- Start A18 eligible elite combat and confirm Blight Sprout ownership remains per-player.
- Confirm one player's Rootblight/Blight Sprout state does not alter the other player's deck.
- Inspect host and client logs for ownership warnings.

### A20 Warning / Downgrade Checks

- Host creates a multiplayer lobby.
- Host selects A20 before any client joins.
- Confirm `godot.log` immediately records that multiplayer A20 selection is enabled for development testing.
- Confirm the same warning says Dual King Brands / second-boss Brand gameplay is currently disabled or downgraded in co-op pending live verification.
- Confirm the warning says A11-A19 inherited systems may still apply if their gates are enabled, subject to live verification.
- Client joins without changing Ascension.
- Start the A20 run.
- Confirm the A20 warning is logged again at run start.
- Confirm testers do not treat A20 multiplayer selection as full A20 co-op support unless Boss 1, courtyard, Boss 2, save/load, and victory/defeat flow are actually verified live.

### Save / Load Checks

- Save and reload an A11 co-op run after map generation.
- Confirm A11 map geometry and selected route remain consistent.
- Save and reload after a Firemarked Elite marker is visible but before entering it.
- Save and reload with Rootblight in one player's deck.
- Save and reload with Blight Sprout seeded in combat if the game safely allows mid-combat save/load.
- Save and reload during or immediately after the A20 inter-boss courtyard only if live A20 co-op boss flow is intentionally being tested.
- Confirm reloaded host/client state does not duplicate Rootblight, Blight Sprout, Forge Token, boss rewards, or A20 intermission rewards.

### godot.log Checks

- Collect host and client `godot.log`.
- Confirm no EZ Micro Balance error or exception lines.
- Confirm no ownership warning for Rootblight, Blight Sprout, Forge Token, Firemark, Banner, Royal Seal, or Brand state.
- Confirm no checksum, desync, disconnect, lobby clamp, or save/load exception lines.
- Confirm A20 downgrade warning appears on host-only selection and on run start when A20 is selected in multiplayer.
- Keep unrelated local invalid-manifest errors separate from EZ Micro Balance findings.

## Result Recording Template

Copy this block for each run:

```text
Date/time:
Tester:
Game version/date:
Package hash:
Host machine/account:
Client machine/account:
BaseLib version/hash:
Env vars:
Test row(s):
Result:
godot.log findings:
Screenshot/video path:
Pass/fail/blocker:
Notes:
```
