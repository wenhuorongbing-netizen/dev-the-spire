# A11-A20 Multiplayer Test Runbook

Date: 2026-05-08  
Scope: private-beta multiplayer test candidate for Spire Plus (`EZMicroBalance` manifest id) Ascension 11-20 selection and source-patched gameplay slices.

A11-A20 selection is now default-on in this private-beta multiplayer test candidate.

Do not treat this runbook as release evidence until results are filled in from real Steam-client multiplayer testing. Normal Steam-client Mod Settings has separate RC1 evidence; controlled smoke passed is not the same as live co-op verification.

## Recommended Multiplayer Setup

Best release test setup:

- Two physical PCs.
- Two Steam accounts.
- Both own and can launch Slay the Spire 2.
- Same game branch and game version/date.
- Same Spire Plus / `EZMicroBalance` package hash.
- Same BaseLib runtime version and files under `<GameRoot>\mods\BaseLib`.
- Same enabled mod set: BaseLib plus Spire Plus only unless a row explicitly tests compatibility.

Same-PC multi-open is not reliable for real Steam multiplayer and should not be the primary release test. It can be useful for rough local investigation only if Steam permits it, but it does not replace the two-PC matrix.

**Known-incompatible mods:** earlier v0.105.x logs showed DamageMeter calling removed `Creature.get_ShowsInfiniteHp()` and interrupting combat startup. Do not enable DamageMeter during EZMB v0.106.0 testing until an updated version is confirmed compatible.

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

Multiplayer run-start/Neow/save-quit diagnostics (P0 investigation):

```text
EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS=1
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
[Environment]::SetEnvironmentVariable('EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS','1','User')
```

PowerShell user env clear:

```powershell
[Environment]::SetEnvironmentVariable('EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION',$null,'User')
[Environment]::SetEnvironmentVariable('EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION',$null,'User')
[Environment]::SetEnvironmentVariable('EZMB_ASCENSION_DIAGNOSTICS',$null,'User')
[Environment]::SetEnvironmentVariable('EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS',$null,'User')
```

After changing User env vars, fully restart Steam and the game on the affected machine before testing.

## Exact Multiplayer Test Matrix

### Mod Settings / Load Checks

- Launch through the normal Steam client on host and client.
- Confirm BaseLib appears in Mod Settings on both machines.
- Confirm BaseLib is enabled on both machines.
- Confirm Spire Plus appears as `EZMicroBalance` on both machines.
- Confirm Spire Plus is enabled on both machines.
- Confirm legacy `EzDailyContent` is disabled or absent on both machines.
- Confirm both machines use the same package hash and same BaseLib version.
- Inspect both `godot.log` files for startup errors, missing localization keys, `CanonicalModelException`, and Spire Plus / `EZMicroBalance` exceptions.

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
- Confirm the same warning says A20 Branded Form / second-boss enhanced dedicated ability gameplay is currently disabled or downgraded in co-op pending live verification.
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
- Copy each log before another run overwrites it, then run `scripts/audit-godot-log.ps1 -Path <copied godot.log> -OutFile <evidence-dir>\host-godot-log-audit.json -FailOnHit` and repeat for the client log. Omit `-FailOnHit` only when preserving a known-failing diagnostic attempt.
- If a client sees `NETWORK_ERROR.VERSION_MISMATCH` / `你试图加入的游戏与您的杀戮尖塔2的版本不同。`, inspect the client log before assuming the visible game version is different:
  - `Version mismatch. Host: ... Ours: ...` means the handshake version strings differ.
  - `Our version ... matches the host's, but our Model ID hash does not` or `ModelDb hash mismatch` means the visible version string matched, but the multiplayer model serialization table differed.
  - Record both `Got initial game info message. Version: ... Hash: ...` and local `ModelIdSerializationCache initialized... Hash: ...` lines.
  - Compare both machines' `release_info.json`, `Loaded X mods (Y total)`, BaseLib/EZMB versions, and installed package hashes.
- Confirm no Spire Plus / `EZMicroBalance` error or exception lines.
- Confirm no ownership warning for Rootblight, Blight Sprout, Forge Token, Firemark, Banner, dedicated ability, or Branded Form state.
- Confirm no checksum, desync, disconnect, lobby clamp, or save/load exception lines.
- Confirm A20 downgrade warning appears on host-only selection and on run start when A20 is selected in multiplayer.
- Keep unrelated local invalid-manifest errors separate from Spire Plus / `EZMicroBalance` findings.

## P0 Triage Matrix — Multiplayer A11-A20 Run-Start / Neow / Black Screen / Save-Quit

**⚠️ Dependency Compatibility Gate — must pass BEFORE any A11-A20 testing:**

1. Disable ALL mods except BaseLib + EZMicroBalance. Explicitly disable/remove DamageMeter, RouteSuggest, AnimeWaifuSilent, AncientWaifus, BetterSpire2Lite, Act4Heart, ModConfig, QuickLink, SpeedX, The-Watcher, and all skin/character/replacement mods.
2. Start singleplayer A0 with Defect (or any character that starts with an active relic like Cracked Core). Enter first combat.
3. Expected: draw cards normally, energy not stuck at 0, no `Creature.get_ShowsInfiniteHp` in `godot.log`.
4. Start singleplayer A10. Same expectations.
5. Start singleplayer A20. Same expectations.
6. `godot.log` must have 0 BaseLib patch failures and 0 `Creature.get_ShowsInfiniteHp` lines.
7. If any of 2-6 fails, stop here. The environment is not ready for EZMB A11-A20 testing. Update BaseLib or roll back game version.
8. Only after passing steps 1-7, proceed to the triage rows below.

Execute these rows to isolate the root cause of the reported 0/80 HP, Neow blocked, save-quit not propagating, and black screen issues. Each row is a separate co-op run with the specified environment variables on both host and client.

**Prerequisites for all rows:**
- Host and client both launch through Steam client.
- Only BaseLib + Spire Plus enabled.
- Restart Steam and game fully after changing env vars.
- Collect `godot.log` from both machines after each run.
- Use `EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS=1` combined with each row to capture lobby state, player HP at run start, Neow HP, and save/quit diagnostics.

### A. Vanilla Control (A10)

Purpose: confirm that base multiplayer works with the mod loaded and vanilla A10.

```text
EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1
```

- Host creates co-op lobby, sets Ascension to A10 (or vanilla max available).
- Client joins.
- Both ready up and start the run.
- Expected: Normal HP, Neow blessings selectable, no black screen.
- Record: Host/client HP at Neow, blessing availability.

### B. Selection-Only Isolation (A20, all gameplay slices off)

Purpose: determine whether EZMB gameplay slices (Rootblight, Firemarks, etc.) cause the HP/Neow issue, or if it's purely in the selection/run-start path.

```text
EZMB_ASCENSION_DISABLE_ALL_SYSTEMS=1
```

- No disable-public env var. A11-A20 selection is default-on.
- Host creates co-op lobby, selects A20.
- Client joins.
- Both ready up and start the run.
- Expected (if issue is in gameplay slices): HP normal, Neow works.
- Expected (if issue is in run-start path): HP 0/80, Neow blocked.
- Critical: Observations here determine whether the fix targets gameplay slices or the run-start infrastructure.

### C. A11 Minimal

Purpose: test the lowest A11-A20 value to see if the issue starts at A11 or only at higher levels.

```text
(no env vars — default-on)
```

- Host selects A11.
- Client joins.
- Start run.
- Record: HP, Neow, black screen, save-quit behavior.

### D. A14 (Rootblight)

Purpose: test whether A14 Rootblight slice triggers the issue.

```text
(no env vars — default-on)
```

- Host selects A14.
- Client joins.
- Start run.
- Record: HP, Neow, Rootblight card presence, ownership.

### E. A20 (Full)

Purpose: test the full A20 with all systems enabled.

```text
(no env vars — default-on)
```

- Host selects A20.
- Client joins.
- Start run.
- Record: A20 warning in log, HP, Neow, black screen, save-quit behavior.

### F. Individual System Disable (used if B shows gameplay slices are involved)

If Row B passes (HP normal with all systems disabled), disable systems one at a time to find which one causes the issue:

1. `EZMB_ASCENSION_ENABLE_ROOTBLIGHT=0` — A14 default
2. `EZMB_ASCENSION_ENABLE_BLIGHT_SPROUT=0` — A15/A18 default
3. `EZMB_ASCENSION_ENABLE_MAP_GEOMETRY=0` — A11/A17 default
4. `EZMB_ASCENSION_ENABLE_FIRE_MARK_ELITES=0` — A12 default
5. `EZMB_ASCENSION_ENABLE_BANNER_ROOMS=0` — A16 default
6. `EZMB_ASCENSION_ENABLE_BOSS_SEALS=0` — A19 default
7. `EZMB_ASCENSION_ENABLE_DUAL_KING_BRANDS=0` — A20 default

Note: These env vars default to enabled when unset. Setting to `0` disables them. Confirm behavior via `AscensionExpansionConfig.IsEnabled`.

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
