# EZ Micro Balance Issues

This file tracks player-reported and runtime-observed issues. Do not mark an item release-ready unless source validation and live verification both support it.

## Open

### Current Open Blocker Audit - 2026-05-08 RC1

The remaining open issues are not blocked by the automated build/test/package loop. They require one of the following evidence classes before they can be closed:

- **Explicitly deferred work:** Rootblight visual feedback, Rootblight independent card art, and bespoke A11/A17 map feedback should remain pending until the user asks to resume that slice.
- **Two-client Steam evidence:** multiplayer HP 0 / Neow blocked, Save & Quit propagation, run-start black screen, A20 TypeLoad retest, A11-A20 selection, A20 warning, and the full co-op matrix require host and client `godot.log` captures from live Steam-client runs.
- **Single-player live gameplay evidence:** A11 natural route traversal and boss reachability, A12 rich-text tooltip rendering, A13/A16 Fission reward frequency, Rootblight/Blight Sprout behavior, and inherited marker regressions require targeted live route/combat/reward checks.
- **Resolved dependency gate retained for traceability:** the BaseLib `Creature.get_ShowsInfiniteHp` API-drift blocker is resolved for the dependency/runtime gate and no longer blocks single-player smoke; remaining multiplayer retests are tracked by the separate co-op issues in this Open section.

Minimum evidence packet for closing a live issue:

- Normal Steam-client launch, not `--force-steam off`.
- BaseLib + EZ Micro Balance only unless the issue explicitly asks for a multi-mod compatibility run.
- Screenshot or log line proving the selected Ascension/debug gate.
- `%APPDATA%\SlayTheSpire2\logs\godot.log` copied before another run overwrites it.
- For co-op issues, both host and client logs from the same attempt, plus the lobby/run start timing and selected Ascension.
- Explicit scan result for release-blocking signatures: `Creature.get_ShowsInfiniteHp`, `BaseLib.Patches.UI.HealthBarForecastPatch`, BaseLib patch failures, non-EZMB mod stack traces, EZMB error/exception, `TypeLoadException`, and `MissingMethodException`.
- Recommended scanner: run `scripts/audit-godot-log.ps1 -Path <copied godot.log> -OutFile <evidence-dir>\godot-log-audit.json -FailOnHit` for clean-log gates, or omit `-FailOnHit` when collecting known-failing diagnostic logs.

Open issue closure checklist:

| Issue | Missing evidence before close |
| --- | --- |
| `ISSUE-2026-05-08-PENDING-VISUALS-AND-DIAGNOSTICS` | User resumes or cancels Rootblight visual feedback, Rootblight card art, A11 diagnostics, multiplayer matrix, and Ancient/co-op save/load backlog. |
| `ISSUE-2026-05-08-MULTIPLAYER-A11-A20-RUN-START-HP0-NEOW-BLOCKED` | Two-client Steam retest with `EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS=1`, plus `EZMB_ASCENSION_DISABLE_ALL_SYSTEMS=1` and vanilla A10 comparison logs. |
| `ISSUE-2026-05-08-MULTIPLAYER-SAVE-QUIT-NOT-PROPAGATING` | Same-attempt host/client co-op logs around Save & Quit proving whether disconnect propagation or UI return fails. |
| `ISSUE-2026-05-08-MULTIPLAYER-RUN-START-BLACK-SCREEN` | Fresh host/client run-start logs that distinguish HP0/Neow, transport sync, timeout, and runtime exception causes. |
| `ISSUE-2026-05-08-MULTIPLAYER-A20-BLACK-SCREEN-OPTIONAL-BOSS-TYPELOAD` | Host/client A20 retest showing no `DoormakerBoss`/Door Wedge type-load crash and no replacement EZMB run-start exception. |
| `ISSUE-2026-05-08-ASCENSION-PUBLIC-SELECTION-DEFAULT-ON-FOR-MP-TEST` | Normal Steam single-player and host-multiplayer selector checks for default-on, public-disable, multiplayer-disable, and A20 warning paths. |
| `ISSUE-2026-05-07-A11-MAP-LENGTH-NOT-PLAYER-VISIBLE` | Natural route traversal through A11 map nodes to boss reachability; existing DevConsole act jumps only prove map surfaces. |
| `ISSUE-2026-05-07-A11-MAP-CHANGE-ANIMATION` | User decision on whether current A11 geometry/A17 hover feedback is acceptable or whether bespoke visual feedback should be implemented. |
| `ISSUE-2026-05-07-A12-TOOLTIP-RICHTEXT-COLORS` | Live tooltip screenshots/logs for Forge Token, Firemark, Banner, A12/A13 rows, rest-site text, and Chinese wrapping with no raw tags. |
| `ISSUE-2026-05-07-A13-FISSION-TOO-RARE-AT-HIGH-ASCENSION` | A13/A16 live reward-frequency sampling for normal combat, Banner Room, Firemarked Elite, and boss reward screens. |
| `ISSUE-2026-05-07-ROOTBUD-ROOTBLIGHT-REWORK` | Live A14/A15/A18 Rootblight and Blight Sprout behavior checks, plus user-resumed visual feedback and independent card art work. |
| `ISSUE-2026-05-07-MULTIPLAYER-A11-A20-SELECTION-BLOCKED` | Two-client host lobby selection checks for A11-A20 default-on and disable flags, then A11/A12/A14/A16/A20 run-start/desync checks. |
| `ISSUE-2026-05-07-A20-MULTIPLAYER-SELECTION-WARNING-MISSING` | Host multiplayer A20 selection and run-start logs proving the downgrade warning appears before/after client join. |
| `ISSUE-2026-05-07-LIVE-COOP-A11-A20-MATRIX-PENDING` | Full two-client matrix with host/client logs, screenshots, save/load rows, ownership checks, and desync scan results. |

### ISSUE-2026-05-08-PENDING-VISUALS-AND-DIAGNOSTICS

Priority: P2/P3

Status: pending; not implemented in the current build/test-green pass.

Area: Rootblight visuals / A11 diagnostics / manual verification backlog

Pending items deliberately left out of the current fix pass:
- Rootblight animation feedback.
- Rootblight I/II/III and Blight Sprout independent card art.
- Broader A11 map geometry diagnostics and natural traversal checks beyond the Act 1/2/3 width/row spot checks. Current normal Steam-client A11 map evidence is recorded in `docs/rc1-live-validation-log.md`.
- Multiplayer matrix and Ancient/co-op save/load verification.

### ISSUE-2026-05-08-MULTIPLAYER-A11-A20-RUN-START-HP0-NEOW-BLOCKED

Priority: P0

Status: diagnostics patch exists and is default-off; unsolved until live co-op retest. Controlled BaseLib+EZMB loader smoke is clean on BaseLib `v3.1.2`; host/client co-op Neow HP still needs live retest.

Area: multiplayer A11-A20 run start / Neow initialization / player HP

Player report (v0.105.0, 2026.05.08, co-op):
- Two-player co-op entered Neow screen with Ascension >10 selected.
- Local player HP displayed as 0/80.
- Cannot select Neow blessing.
- Singleplayer works fine with the same Ascension level.

Current source analysis:

- `AncientEventModel.BeforeEventStarted` (source code/src/Core/Models/AncientEventModel.cs:143-156) sets player HP to 0 via `SetCurrentHpInternal(0m)`, then heals via `CreatureCmd.Heal` to full (or 80% for A2+ WearyTraveler). This works in singleplayer.
- Vanilla `AscensionManager` (`source code/src/Core/Entities/Ascension/AscensionManager.cs`) has `maxAscensionAllowed = 10` and only handles A4 (TightBelt -1 potion) and A10 (AscendersBane). No HP effects.
- `RunManager.InitializeNewRun()` -> `ApplyAscensionEffects(player)` -> `AscensionManager.ApplyEffectsTo(player)` does not touch HP.
- `Player.CreateForNewRun()` uses `character.StartingHp` for both current and max HP.
- No EZMB gameplay slice touches player HP during run start or Neow.

Hypotheses (in priority order):
1. Vanilla multiplayer `CreatureCmd.Heal` or `SetCurrentHpInternal` fails/skips for the non-host player when `RunState.AscensionLevel > 10`, possibly because `NetService.Type.IsMultiplayer()` bypasses some initialization path.
2. A multiplayer-specific runtime path prevents the v0.105.0 `AncientEventModel.BeforeEventStarted` / `CreatureCmd.Heal` flow from applying to the affected client, despite the refreshed source still showing the vanilla full-heal path.
3. Our `AscensionSelectionPatches` expand `maxMultiplayerAscensionUnlocked` during `UpdateMaxMultiplayerAscension` in a way that corrupts some lobby/player state before the run starts. Our patches do not touch `BeginRunForAllPlayers` directly (only log a warning).
4. A20 Dual King Brands warning patch or some other EZMB Harmony patch interferes with lobby cleanup or run setup in a non-obvious way.
5. The Neow event fails to start properly in multiplayer, so `BeforeEventStarted` never fires, and HP remains at whatever value was set during player creation (which should still be `StartingHp`).

Required evidence:
- Run with `EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS=1` in co-op to capture lobby state, player HP at run start, `BeginRunLocally` HP, `AfterActEntered` HP, and Neow `BeforeEventStarted` HP.
- Bisect via `EZMB_ASCENSION_DISABLE_ALL_SYSTEMS=1` to confirm whether EZMB gameplay slices are involved.
- Test with `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` + vanilla A10 as control.

### ISSUE-2026-05-08-MULTIPLAYER-SAVE-QUIT-NOT-PROPAGATING

Priority: P0/P1

Status: source-investigated; vanilla source path should propagate disconnect, but live co-op evidence is still pending

Area: multiplayer save-and-quit / disconnect / host-client sync

Player report: in co-op, when one player saves and quits, the other machine does not synchronously quit, disconnect, or return to menu.

Current v0.105.0 source notes:
- Pause-menu save-and-quit is handled by `NPauseMenu.OnSaveAndQuitButtonPressed()` / `CloseToMenu()`, which calls `NGame.ReturnToMainMenu()`.
- `NGame.ReturnToMainMenu()` fades out, loads common/main-menu assets, calls `RunManager.Instance.CleanUp()`, and loads the main menu.
- `NGame.Quit()` (source code/src/Core/Nodes/NGame.cs) saves settings/profile data and calls `GetTree().Quit()` but does not send a disconnect message to remote peers.
- `RunManager.CleanUp(bool graceful = true)` disposes run synchronizers and calls `NetService.Disconnect(NetError.Quit, !graceful)`.
- `NetHostGameService.Disconnect(...)` calls the active transport's `StopHost(...)`.
- `SteamHost.StopHost(...)` closes every client connection with the quit reason, leaves the Steam lobby, and then reports local disconnection.
- `ENetHost.StopHost(...)` sends an ENet disconnection packet to each client when not immediate, then disconnects each peer and reports local disconnection.
- `RunLobby.OnDisconnected(...)` calls `RunManager.LocalPlayerDisconnected(...)`; for non-`QuitGameOver` reasons during an active run, `RunManager.LocalPlayerDisconnected(...)` queues `ReturnToMainMenuWithError(...)`.
- `NErrorPopup.Create(...)` suppresses a popup only for self-initiated `Quit`; remote peer disconnects should still have a non-self-initiated reason.
- Current EZMB Ascension patches do not patch `NPauseMenu`, `RunManager.CleanUp`, `RunLobby.OnDisconnected`, `NetHostGameService`, `NetClientGameService`, `SteamHost`, or `ENetHost`.

Required investigation:
- Live two-client logs still need to confirm whether the remote peer receives `NetError.Quit`, whether `RunLobby.OnDisconnected(...)` fires, and whether `ReturnToMainMenuWithError(...)` completes.
- Run co-op with `EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS=1` and collect both host/client logs around Save & Quit.
- If the remote peer never receives the disconnect, the defect is likely transport/session-state or vanilla runtime behavior rather than an EZMB save/quit patch.
- If the remote peer receives the disconnect but does not return to menu, inspect `RunManager.LocalPlayerDisconnected(...)` and active UI state at that moment.
- Do not add a speculative EZMB multiplayer save/quit fix without live evidence identifying which branch fails.

### ISSUE-2026-05-08-MULTIPLAYER-RUN-START-BLACK-SCREEN

Priority: P0/P1

Status: dependency errors removed; still investigating until fresh host/client live logs prove whether the black screen is tied to HP0/Neow, A20 startup, transport sync, or another runtime exception.

Area: multiplayer run start / screen transition / mod load / dependency compatibility

Player report: multiplayer run start can still black-screen, even after the earlier `DoormakerBoss` TypeLoadException fix.

Current status:
- `ISSUE-2026-05-08-MULTIPLAYER-A20-BLACK-SCREEN-OPTIONAL-BOSS-TYPELOAD` was fixed by making `BossSealCatalog` use runtime-safe `ModelId` strings. This fixed the TypeLoadException for `DoormakerBoss`.
- But the player report suggests black screen can still occur, potentially from other causes.

Hypotheses:
1. HP 0/80 -> Neow blocked -> screen transition never completes (same root cause as HP0-Neow issue).
2. A different TypeLoadException or missing model for a different v0.105.0 API.
3. Network desync during run start - host reaches Act 0 but client never receives the transition.
4. Missing localization or model that causes a silent failure during lobby cleanup or run scene setup.

Required evidence:
- Collect host AND client `godot.log` covering the 200 lines before and after run start.
- Look for exceptions, missing models, missing localization, network disconnect, desync, or timeout.
- If black screen follows from HP0/Neow blocked, fix that root cause first.
- If independent, add separate `EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS` entries for screen transition sync.

### ISSUE-2026-05-08-MULTIPLAYER-A20-BLACK-SCREEN-OPTIONAL-BOSS-TYPELOAD

Priority: P0

Status: source-patched and published locally; live co-op retest pending

Area: A20 multiplayer run start / A19 Boss Royal Seal catalog / Early Access API compatibility

Player report: starting a multiplayer A20 run can black-screen after the lobby begins the run.

Observed log evidence:

- Latest `godot.log` shows host multiplayer A20 run start reached `NGame.StartNewMultiplayerRun(...)` with Ascension 20.
- Act 1 map generation applied A11/A12/A16 metadata, then failed in `AscensionMapService.MarkBossSeals(...)`.
- Fatal mod stack: `System.TypeLoadException: Could not load type 'MegaCrit.Sts2.Core.Models.Encounters.DoormakerBoss'` from `BossSealCatalog..cctor()`.
- The same local log also contains unrelated local-mod/BaseLib compatibility errors, but the A20 run-start abort is the `DoormakerBoss` type-load failure in EZ Micro Balance.

Root cause:

- Earlier source/API evidence proved optional Early Access boss and power types are not safe to reference directly; the refreshed v0.105.0 source does not expose the previously crashing `DoormakerBoss` type.
- `BossSealCatalog` used hard generic references like `ModelDb.GetId<DoormakerBoss>()`; static initialization therefore crashed before the run could finish generating the first map.
- Current build also proved adjacent API drift: direct `Doormaker` / `HungerPower` / `ScrutinyPower` / `GraspPower` references and direct `PumpkinCandle.ActiveAct` access are not safe against the installed DLL.

- Earlier source fix:

- `BossSealCatalog` previously used runtime-safe `ModelId` strings such as `ENCOUNTER.DOORMAKER_BOSS` instead of hard references to optional boss encounter classes.
- Door Wedge combat checks previously used runtime `ModelId` checks for the Doormaker monster and phase powers, so missing optional types did not block compile/load.
- v0.105.0 source later replaced the active Doormaker/Door Wedge scope with `AEONGLASS_BOSS`; current active EZMB source has no Door Wedge implementation and applies the temporary Aeonglass +5 Strength seal instead.
- Debt patching was adjusted to avoid direct compile/accessibility assumptions that broke against the current installed game API.
- Pumpkin Candle EZMB patching was removed; vanilla Pumpkin Candle behavior is restored for the v0.105.0 package, so no Pumpkin-only Harmony target participates in `PatchAll()`.
- Added source guard tests to prevent reintroducing hard optional `DoormakerBoss` / `Doormaker` type references in the Boss Seal startup path.

Manual retest:

- Republish or confirm the installed `EZMicroBalance.dll` timestamp is newer than this fix.
- Host multiplayer with BaseLib and EZ Micro Balance only if possible.
- Select A20, let the client join, ready both players, and start the run.
- Confirm the run leaves the lobby and reaches the Act 1 map instead of black-screening.
- Inspect `godot.log` for no `EZMicroBalance` `TypeLoadException`, especially no `DoormakerBoss`, `Doormaker`, `HungerPower`, `ScrutinyPower`, or `GraspPower` load errors.
- Keep A20 Dual King Brands co-op gameplay verification pending; this fix is a crash/compatibility fix, not a full live co-op balance pass.

### ISSUE-2026-05-08-ASCENSION-PUBLIC-SELECTION-DEFAULT-ON-FOR-MP-TEST

Priority: P0

Status: source-patched; package/smoke refreshed; Steam-client/live co-op pending

Area: A11-A20 selector gate / multiplayer pre-release testing

Decision: A11-A20 selection is now default-on in this private-beta multiplayer test candidate so testers can immediately exercise single-player and host-multiplayer A11-A20 through the original lobby UI.

Required behavior:

- Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison.
- Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection while leaving single-player A11-A20 available.
- `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.
- A20 multiplayer selection is not full A20 co-op support. Dual King Brands / second-boss Brand gameplay remains disabled or downgraded in co-op pending live verification.
- Normal Steam-client Mod Settings has separate RC1 evidence; controlled smoke passed is not the same as live co-op verification.

Manual retest:

- With no Ascension env vars, confirm single-player and host multiplayer can select A11-A20.
- With `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1`, confirm single-player and multiplayer selection return to vanilla A1-A10.
- With `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`, confirm single-player A11-A20 remains available and host-multiplayer selection returns to the vanilla cap.
- Confirm host-only multiplayer A20 selection logs the downgrade warning before any client joins, then logs again on run start after a client joins.
- Keep live gameplay, save/load, and live co-op/desync verification pending until actually executed or explicitly accepted.

### ISSUE-2026-05-07-A11-MAP-LENGTH-NOT-PLAYER-VISIBLE

Priority: P1

Status: source-patched again; Act 1 normal Steam-client map/save-load spot check passed; Act 2/3 normal Steam-client map-surface observation passed; broader natural traversal and boss-reachability verification pending

Area: A11 Wide Tower, Long Road / map generation

Player report: A11 still looks like the original map size. It was longer once, then regressed.

Current source fix:

- `AscensionFeatureGate` now sets A11 rows to Act 1 `+1`, Act 2 `+1`, Act 3 `+2`.
- `AscensionMapService` now accepts old width-only adjusted maps and inserts missing late rows instead of returning early.
- A11 still expands from 7 to 8 columns and inserts a reachable optional route node.
- A11 no longer marks ordinary route nodes with a dedicated long-road marker or hover explanation; map growth is represented only by vanilla-looking rows, columns, nodes, and paths.

RC1 live evidence:

- Normal Steam-client BaseLib+EZMB-only run selected A11 through the original single-player Ascension arrows (`.tools\runtime-evidence\rc1-a11-map-save-20260508-110008\08-character-select-a11.png`).
- The Act 1 map screenshot (`11-a11-act1-map-after-neow-continue.png`) renders the widened map with normal route nodes.
- `a11-map-save-load-godot-live.log` records `Ascension A11 applied ... inserted 1 late route row(s); actIndex=0; columns=8; rows=17`.
- `a11-save-map-dimensions.json` records `MapHeight: 17`, `BossRow: 17`, `RouteRowCount: 16`, `ColumnCount: 8`, and columns `0,1,2,3,4,5,6,7`.
- After selecting the first monster node, the game wrote `current_run.save`; Save & Quit -> Continue loaded back into the A11 combat, and the map reopened with `columns=8; rows=17`.
- A later normal Steam-client BaseLib+EZMB-only run selected A11 through the original UI and used DevConsole `act 2` / `act 3` only to observe later-act map surfaces. Evidence directory: `.tools\runtime-evidence\rc1-a11-act23-map-20260508-113355`.
- `a11-act23-godot-live.log` records Act 2 `Ascension A11 applied ... inserted 1 late route row(s); actIndex=1; columns=8; rows=16` and Act 3 `Ascension A11 applied ... inserted 2 late route row(s); actIndex=2; columns=8; rows=16`, with 0 `ERROR` lines and 0 release-blocking signatures.
- Act 2 screenshot `25-a11-act2-map-clean.png` and Act 3 screenshot `27-a11-act3-map-clean.png` render normal route nodes without an A11-specific marker or hover tooltip.

Manual retest:

- Act 1 fresh A11 route-width/row/save-load spot check is complete for RC1 evidence above.
- Act 2/3 width/row/no-marker observation is complete for RC1 evidence above, using DevConsole act jumps rather than natural traversal.
- Confirm the map still has a low-risk route and boss reachability through natural route traversal.
- Future traversal helper: `win` may be used to end combats after clicking naturally reachable map nodes; do not use DevConsole `travel` as proof of reachability, because local source shows it enables jumping to any map room.

### ISSUE-2026-05-07-A11-MAP-CHANGE-ANIMATION

Priority: P3

Status: controlled-smoke refreshed; Act 1/2/3 normal Steam-client A11 map surfaces observed; bespoke animation/A17 UI feedback still pending

Area: A11/A17 map UI feedback

Player report: map and visibility changes should not feel random; the player should clearly see that something changed.

Current mitigation:

- A17 deep-branch nodes still have map hover tips.
- A11 long-road node tips were removed after player feedback; A11 now relies on normal map geometry instead of a special visible marker.

Remaining work:

- No bespoke map-generation animation or transition sequence has been implemented yet.
- Live UI pass should decide whether hover tips are enough or whether a short map pulse/overlay is needed.

### ISSUE-2026-05-07-A12-TOOLTIP-RICHTEXT-COLORS

Priority: P2

Status: source-patched; live tooltip/rich-text verification pending

Area: A12 Firemark / Forge Token / Banner tooltip text

Player report: A12 text works mechanically, but numbers should be blue and important words such as upgrade, Gold, Skill card, Firemark, Forge Token, Rest, and Smith should be gold.

Current source fix:

- `ForgeTokenRelic` English/ZHS relic text and rest-site extra text now use `[blue]` for values and `[gold]` for important terms.
- Firemark power tooltips now color values and core terms.
- Banner room power/localization strings now color values and core terms.
- Ascension panel localization for A12/A13/Banners now uses the same markup.

Manual retest:

- Hover Forge Token, Firemark powers, Banner powers, A12/A13 ascension rows, and rest-site Forge Token extra text.
- Confirm rich text renders instead of showing raw tags.
- Confirm Chinese text wraps cleanly.

### ISSUE-2026-05-07-A13-FISSION-TOO-RARE-AT-HIGH-ASCENSION

Priority: P2

Status: source-patched; live A13/A16 reward-frequency verification pending

Area: A13 Fission Enchantment / A16 inherited ascension behavior

Player report: A16 should include earlier ascension effects, but Fission nearly disappeared while testing A16.

Current source evidence:

- `AscensionFeatureGate.IsLevelEnabled(...)` uses `runState.AscensionLevel >= requiredAscensionLevel`, so A16 includes A13 when the public/debug gate is active.
- Fission source chances were raised from `10/15/20/5` to `25/35/40/15` for normal combat / Banner Room / Firemarked Elite / Boss rewards.

Manual retest:

- Test A16 with public/debug ascension enabled.
- Check repeated normal combat rewards and Banner Room rewards.
- Confirm Fission remains limited to eligible Attack/Skill cards and still appears at most once per reward screen.

### ISSUE-2026-05-07-ROOTBUD-ROOTBLIGHT-REWORK

Priority: P1

Status: source-patched; live Rootblight/Blight Sprout behavior, visual feedback, and card art verification pending

Area: A14/A15/A18 Rootblight and Blight Sprout

Player report: the old Root Bud / Rootblight wording was conceptually unclear, Boss Sprout count was too low, and Boss/Elite Sprout text was too long.

Current source fix:

- ZHS player-facing term is now `根芽`.
- Boss fights in Acts 2/3 now seed 2 Blight Sprout cards.
- Blight Sprout text is shortened: play to Exhaust; Boss sprouts use rounds 3/4 and elite sprouts use round 3; if seen and not played, add Rootblight I after combat.
- Rootblight I/II/III costs are 2/3/4.
- Played Rootblight removes its master-deck card and queues the downgrade card after combat.
- Unplayed Rootblight I/II upgrades after combat; ignored Rootblight III stays III and adds one Rootblight I only once per card.
- Rootblight is capped at 4 cards, and cap hits show `Root system full.` / `根系已满。`.
- Rest removes exactly one highest-stage Rootblight instead of clearing all Rootblight.

Manual retest:

- A14 new run starts with Rootblight I.
- A15 Act 2/3 Boss fights bury 2 Blight Sprouts.
- A18 eligible Act 2/3 Elite fights bury 1 Blight Sprout.
- Seen-but-unplayed Blight Sprout adds one Rootblight I after combat.
- Rootblight I/II/III play and post-combat behavior matches the new card text.

### ISSUE-2026-05-07-MULTIPLAYER-A11-A20-SELECTION-BLOCKED

Priority: P1

Status: source-patched; live co-op verification pending

Area: A11-A20 Ascension selection / multiplayer lobby

Player report: A11-A20 cannot be used in multiplayer, but co-op should eventually support the same expanded Ascension range instead of being single-player only.

Desired behavior:

- A11-A20 selection is available in multiplayer by default for this private-beta multiplayer test candidate.
- `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` restores vanilla A1-A10 selection for comparison.
- `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` disables only host-multiplayer A11-A20 selection.
- `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.
- Multiplayer selection must not patch or corrupt vanilla A1-A10 progress.
- Earlier Ascension effects still inherit normally at higher levels.
- Per-player systems such as Rootblight and Blight Sprout remain independent and do not desync.
- A21-A30 remains out of scope.

Implementation notes:

- Local source inspection found that `StartRunLobby.UpdateMaxMultiplayerAscension()` computes the multiplayer cap from each `LobbyPlayer.maxMultiplayerAscensionUnlocked`, while `UpdatePreferredAscension()` writes host selections to `PreferredMultiplayerAscension`.
- Current source patch expands host multiplayer lobbies by default unless `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` or `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` is set. It temporarily raises in-memory lobby unlock caps only during max recomputation, restores them in a finalizer, and skips A11-A20 preferred-progress writes.
- Host-multiplayer A11-A20 selection is independently disableable with `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`.
- A11-A20 gameplay, per-player Rootblight/Blight Sprout ownership, and desync behavior still require live co-op verification.
- A20 Dual King Brands gameplay is still single-player gated through `IsDualKingBrandsSinglePlayerEnabled(...)`; the host multiplayer selector/start path now logs a development-testing downgrade warning, but live co-op verification is still pending.

Manual retest:

- Host a multiplayer lobby with BaseLib and EZ Micro Balance enabled.
- Confirm A11-A20 selection is available by default with no Ascension env var.
- Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` and confirm A1-A10 behavior is restored for comparison.
- Clear the disable variable and confirm the lobby can select A11-A20 again.
- Start a co-op run at A11/A12/A14/A16/A20 and confirm all players load without desync.
- Confirm Rootblight/Blight Sprout ownership remains per-player in co-op.
- Confirm A20 multiplayer selection does not imply that Dual King Brands gameplay is live co-op verified.

### ISSUE-2026-05-07-A20-MULTIPLAYER-SELECTION-WARNING-MISSING

Priority: P2

Status: source-patched with log warning; live co-op verification pending

Area: A20 Dual King Brands / multiplayer selector messaging

Audit finding: host multiplayer can source-select A20 when the public development gate is enabled, but A20 Dual King Brands gameplay remains single-player gated by `AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(...)`.

Desired behavior:

- Multiplayer A20 selection must not make testers think A20 Dual King Brands is fully supported in co-op.
- Add a clear runtime log, UI warning, or selector-side message before multiplayer A20 testing.
- Keep A20 gameplay conservative until live co-op boss-path verification proves host/client behavior is safe.

Planning notes:

- Do not remove the current A20 single-player gameplay gate without local source evidence and live co-op test coverage.
- Keep selection support, gameplay activation, progress writes, and live co-op verification documented as separate surfaces.
- `AscensionSelectionPatches.WarnIfA20MultiplayerDowngraded(...)` now logs on host multiplayer A20 selection and host multiplayer A20 run start, including the host-only lobby case before a client joins.
- Warning text says multiplayer A20 selection is for development testing, Dual King Brands / second-boss Brand gameplay is disabled or downgraded in co-op pending live verification, and A11-A19 inherited systems may still apply if their gates are enabled.

Manual retest:

- In a host multiplayer lobby with no Ascension env vars, select A20 before any client joins.
- Confirm the tester-visible warning or log appears on host-only selection.
- Let a client join without changing Ascension, then start the A20 run.
- Confirm the tester-visible warning or log appears on selection and run start.
- Confirm the run does not silently apply single-player-only Dual King Brands behavior to co-op.

### ISSUE-2026-05-07-LIVE-COOP-A11-A20-MATRIX-PENDING

Priority: P1

Status: source-patched; live co-op matrix pending

Area: A11-A20 multiplayer runtime verification

Audit finding: source guards prove selector and ownership shapes, but no live co-op matrix has verified lobby join, client view, run start, save/load, per-player state, or desync behavior.

Minimum matrix:

- Gate default-on: with no Ascension env vars, host can select A11-A20 and client sees the selected value.
- Gate off: `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` restores vanilla A1-A10 selection.
- Disable flag: `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` restores vanilla multiplayer cap.
- A11: co-op run starts with widened/longer map and no A11 marker.
- A12: Firemarked Elite route markers remain visible and host/client agree.
- A14/A15/A18: Rootblight and Blight Sprout state remains player-owned.
- A16: Banner Room markers and combat rules remain visible and synchronized.
- A20: selection limitation or warning is visible; Dual King Brands remains treated as not live co-op verified.
- Logs: no ownership warnings, checksum divergence, or multiplayer desync lines in `godot.log`.

## Resolved / Player-Verified

### ISSUE-2026-05-07-A11-LONG-ROAD-MAP-MARKER-UNWANTED

Priority: P2

Status: resolved for A11 no-marker behavior on 2026-05-08; inherited marker regression checks remain tracked by the relevant A12/A16/A17/A19/A20 live-verification items and the open blocker audit.

Area: A11 Wide Tower, Long Road / map UI

Player report: A11 should make the map longer/wider through normal map geometry only. It should not put a special visible marker or hover tooltip on the map just to explain the extra route space.

Desired behavior:

- Remove the dedicated A11 long-road map marker/hover indicator from newly inserted route nodes.
- A11 map changes should look like vanilla map rows and paths, not like a special event or quest node.
- Keep the actual map-length/width tuning separately testable; if final tuning is only one added row, update docs/localization/changelog to avoid claiming larger route growth.

Implementation notes:

- `LongRoad` metadata, `MarkLongRoad`, and `LONG_ROAD_NODE` localization were removed from active source/resources.
- `AscensionMapQuestMarker` remains in use only for A17 Deep Branch generic markers; A12 Firemark, A16 Banner, A17 Deep Branch, A19 Seal, and A20 Brand indicators remain on their own paths.
- RC1 Act 1 screenshot `.tools\runtime-evidence\rc1-a11-map-save-20260508-110008\11-a11-act1-map-after-neow-continue.png` shows ordinary A11 route nodes without a dedicated A11 marker, icon, or hover tooltip. The after-load map screenshot `16-map-open-after-load-attempt.png` shows the same no-marker surface after Continue.
- RC1 Act 2/3 screenshots `.tools\runtime-evidence\rc1-a11-act23-map-20260508-113355\25-a11-act2-map-clean.png` and `27-a11-act3-map-clean.png` show ordinary later-act route nodes without a dedicated A11 marker, icon, or hover tooltip.

Manual retest:

- Act 1/2/3 A11 no-marker map-surface spot checks are complete for RC1 evidence above.
- Natural A11 route traversal remains tracked by `ISSUE-2026-05-07-A11-MAP-LENGTH-NOT-PLAYER-VISIBLE`.
- Firemark/Banner/Deep Branch/Boss Seal indicator checks remain tracked by their relevant Ascension live-verification items.


### ISSUE-2026-05-08-V105-BASELIB-CREATURE-SHOWSINFINITEHP-API-DRIFT

Priority: P1 environment/runtime verification

Status: resolved for the BaseLib dependency/API-drift gate on 2026-05-08; remaining multiplayer run-start/Neow/save-quit evidence is tracked by the separate P0 co-op issues in the Open section. BaseLib `v3.1.2`, a clean BaseLib+EZMB-only controlled smoke, clean normal Steam-client startup/Mod Settings log snapshots, and Codex-observed normal-Steam single-player combat smoke for A0/A10/A20 via DevConsole `fight CULTISTS_NORMAL` supersede the earlier failure. User also reports single-player A0/A10/A20 and boss/basic combats pass after the BaseLib update. EZ Micro Balance's dedicated Mod Settings page/display is now covered by the 2026-05-08 `095137` normal Steam-client recheck after adding the no-op BaseLib config page.

Area: v0.105.0 API drift / BaseLib compatibility / mod environment hygiene

Evidence from `godot2026-05-08T05.06.30.log` (v0.105.0, 2026.05.08):

1. **Test environment loaded 17 mods, not only BaseLib + EZMicroBalance:**
   - `Loaded 17 mods (19 total)`
   - Loaded `DamageMeter`, `RouteSuggest`, `AnimeWaifuSilent`, `ModConfig`, `QuickLink`, `SpeedX`, `The-Watcher`, and others.
   - This violates the release test prerequisite: only BaseLib + EZMicroBalance enabled.

2. **Superseded BaseLib v3.1.0 failure evidence:**
   - `Undefined target method for patch method ... ExhaustivePatch`
   - `Undefined target method for patch method ... PersistPatch`
   - `Undefined target method for patch method ... PurgePatch`
   - `[BaseLib] Applied 150 patches successfully, 3 failed`

3. **`Creature.get_ShowsInfiniteHp()` is missing in v0.105.0:**
   - `System.MissingMethodException: Method not found: 'Boolean MegaCrit.Sts2.Core.Entities.Creatures.Creature.get_ShowsInfiniteHp()'`
   - Called from `BaseLib.Patches.UI.HealthBarForecastPatch.RefreshForegroundOverlay(NHealthBar healthBar)`
   - Also called from `DamageMeter.Scripts.CombatDataCollector.SnapshotEnemyHp(CombatState combatState)`
   - Stack reaches `CrackedCore.BeforeSideTurnStart` and `CombatManager.StartCombatInternal()`

4. **Direct gameplay impact:**
   - The `MissingMethodException` in the combat-start/turn-start hook chain interrupts normal combat initialization.
   - Observed: singleplayer Defect A20 enters combat but does not draw cards, energy stuck at 0/3. Combat does not enter a normal player turn.
   - This is NOT an EZMB logic bug; it is a dependency/environment compatibility blocker.

Required resolution (before any EZMB fix or release claim):
- [x] Disabled/isolated all mods except BaseLib + EZMicroBalance for the RC1 normal Steam-client startup log; the moved local mod entries were restored afterward.
- [x] Updated BaseLib runtime/project package to `v3.1.2`; current controlled BaseLib+EZMB-only smoke has no `Creature.get_ShowsInfiniteHp`, BaseLib patch-failure, or DamageMeter removed-API signatures.
- [x] RC1 normal Steam-client startup log snapshot is clean for the release gate signatures. Codex temporarily isolated non-BaseLib/EZMB local mod entries, launched through Steam, reached main menu, saved `.tools\runtime-evidence\rc1-normal-steam-clean-godot-20260508-090122.log`, restored 23 moved mod entries and `settings.save`, and confirmed `Loaded 2 mods (2 total)`, BaseLib `177 patches successfully, 0 failed`, EZMB initialization, 0 `ERROR` lines in the startup snapshot, and 0 removed-API/EZMB exception signatures.
- [x] RC1 normal Steam-client Mod Settings UI recheck opened `模组配置`: BaseLib appeared and was enabled; EZ Micro Balance appeared as the localized page `微平衡` with `无可配置选项。`; main-menu/log evidence showed only `BaseLib, EZ Micro Balance` loaded. Snapshot `.tools\runtime-evidence\rc1-normal-steam-modsettings-page-godot-20260508-095137.log` has `Registered config for mod EZMicroBalance`, `Loaded 2 mods (2 total)`, 0 `ERROR` lines, and 0 release-blocking signatures.
- [x] Confirm singleplayer A0 combat draws cards and gains energy normally. Evidence: normal Steam-client BaseLib+EZMB-only DevConsole combat smoke `a0-debug-fight-clean.png` shows 80/80 HP, 3/3 energy, five-card hand, enemies, HP bars, and intents; natural route-click first-node path remains unrun if stricter coverage is required.
- [x] Confirm singleplayer A10 combat draws cards and gains energy normally. Evidence: normal Steam-client BaseLib+EZMB-only DevConsole combat smoke `a10-first-combat-clean.png` shows 64/80 HP, 3/3 energy, five-card hand, enemies, HP bars, and intents; natural route-click first-node path remains unrun if stricter coverage is required.
- [x] Confirm singleplayer A20 combat draws cards and gains energy normally. Evidence: normal Steam-client BaseLib+EZMB-only DevConsole combat smoke `a20-debug-fight-clean.png` shows 64/80 HP, 3/3 energy, five-card hand, Rootblight present, enemies, HP bars, and intents; natural route-click first-node path remains unrun if stricter coverage is required.
- User-reported on 2026-05-08: single-player A0/A10/A20 plus boss/basic combats pass after the BaseLib update. This now complements the Codex-observed combat-smoke evidence.
- [x] Normal Steam-client startup snapshot has no `Creature.get_ShowsInfiniteHp`.
- [x] Normal Steam-client startup snapshot has no BaseLib patch failures.
- [x] Normal Steam-client startup snapshot has no DamageMeter or other non-EZMB mod exceptions.
- Combat-smoke log caveat: the A0/A10/A20 debug-fight logs have 0 removed-API signatures, 0 BaseLib patch failures, 0 `TypeLoadException`, 0 `MissingMethodException`, and 0 EZMB error/exception pattern hits. They are not clean-log gate snapshots because automated test-run abandonment/window closing produced Godot exit resource-leak `ERROR` lines, and A20/A0 include a temporary save-backup delete `ERROR` from the save restoration flow. The clean-log gate remains the earlier isolated startup and Mod Settings snapshots.
- Multiplayer A11-A20 testing may resume, but co-op run-start/Neow/save-quit evidence remains required.


### ISSUE-2026-05-07-HANDOFF-GIT-STATUS-HYGIENE

Priority: P3

Status: resolved for the 2026-05-08 RC1 hygiene refresh; final handoff still must re-run git status/log after any later edits

Area: release handoff / repository status docs

Audit finding: handoff and audit docs can become stale when they say "No commit or push has been made" or "worktree dirty." Final release handoff must re-check the current status rather than relying on old wording from an earlier local snapshot.

Resolution evidence:

- `docs/private-beta-verification-handoff.md`, `docs/features/ancients-rework-v4/completion-audit.md`, and `docs/rc1-live-validation-log.md` record the current point-in-time `git log -1 --oneline --decorate`: `96bfa50 (HEAD -> main, origin/main, origin/HEAD) fix try 10`.
- Those docs record the branch as aligned with `origin/main` while the working tree remains dirty with modified files, deleted moved originals, and untracked new patch/doc/archive files.
- The handoff no longer uses stale no-commit wording for the current branch state, and it explicitly says not to describe the checkout as fully pushed until pending edits are reviewed, committed, and pushed.
- The handoff and audit docs require rerunning `git status --short --branch` and `git log -1 --oneline --decorate` before final release packaging or handoff.

### ISSUE-2026-05-07-RELEASE-ARTIFACT-TESTS-DEPEND-ON-IGNORED-PUBLISH-OUTPUT

Priority: P2

Status: resolved on 2026-05-08; normal tests pass without ignored publish artifacts, release artifact tests stay opt-in

Area: automated tests / release artifact validation

Audit finding: `.gitignore` excludes `publish/`, `*.zip`, `*.dll`, and `*.pck`, while some release guard tests require installed/staging/versioned zip artifacts. This is useful for release validation on the maintainer machine, but it can make normal `dotnet test` brittle in a clean clone unless package generation ran first.

Resolution evidence:

- Normal `dotnet test` no longer requires ignored publish/package artifacts because package/hash/runtime-smoke checks are marked with `ReleaseArtifactFactAttribute`.
- Normal package/hash/runtime-smoke checks are marked with `ReleaseArtifactFactAttribute` and skip unless `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` is set.
- Release artifact tests remain strict when opted in.
- On 2026-05-08, Codex temporarily moved the ignored `publish/` directory aside, ran `dotnet test EZMicroBalance.sln`, observed 65 passed / 16 skipped / 0 failed, and restored `publish/`.
- The refreshed docs describe the package refresh and opt-in command order.

### ISSUE-2026-05-07-CURRENT-PACKAGE-RUNTIME-SMOKE-STALE

Priority: P1

Status: resolved for loader/runtime-smoke freshness on 2026-05-08; live gameplay remains tracked by separate open issues

Area: controlled runtime smoke / SavedSpireField registration

Audit finding: several docs cited a prior controlled `--force-steam off` smoke with an obsolete SavedSpireFields count. The current source/package defines 12 SavedSpireFields after Rootblight v2.2 card-state fields.

Resolution evidence:

- Current package was published and package staging/versioned/zip artifacts were refreshed.
- Controlled `--force-steam off` smoke passed after publish/package refresh.
- Temporary profile settings enabled only `BaseLib` and `EZMicroBalance`, explicitly disabled other discovered local mods, and restored `settings.save` plus `settings.save.backup` byte-for-byte.
- `godot.log` showed `Loaded 2 mods (19 total)`, BaseLib initialization, EZ Micro Balance DLL/PCK load/init, `Found 12 SavedSpireFields`, default-on Ascension initializer wording with 0 old `Default-off gate` lines, main menu in `13,628ms`, 0 EZ Micro Balance error/exception lines, and no `Creature.get_ShowsInfiniteHp`, BaseLib patch-failure, or DamageMeter removed-API signatures.
- Later normal Steam-client isolated startup and Mod Settings snapshots also loaded only BaseLib + EZ Micro Balance with `Loaded 2 mods (2 total)`, `Found 12 SavedSpireFields`, 0 `ERROR` lines, and the localized EZ Micro Balance config page visible. Live gameplay verification is still open elsewhere.

### ISSUE-2026-05-07-A12-FORGE-TOKEN-RESTSITE-CRASH

Priority: P1

Status: player reported fixed on 2026-05-07; keep in regression list

Area: A12 Forge Token / rest-site transition

Player verification: carrying Forge Token into a rest site no longer crashes.

Regression retest:

- Enter a rest site while holding Forge Token.
- Test Rest payout and Smith payout separately.
- Confirm token relic is removed after payout.
