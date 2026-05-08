# EZ Micro Balance Issues

This file tracks player-reported and runtime-observed issues. Do not mark an item release-ready unless source validation and live verification both support it.

## Open

### ISSUE-2026-05-08-MULTIPLAYER-A11-A20-RUN-START-HP0-NEOW-BLOCKED

Priority: P0

Status: investigating; diagnostics patch pending; unsolved

Area: multiplayer A11-A20 run start / Neow initialization / player HP

Player report (v0.105.0, 2026.05.08, co-op):
- Two-player co-op entered Neow screen with Ascension >10 selected.
- Local player HP displayed as 0/80.
- Cannot select Neow blessing.
- Singleplayer works fine with the same Ascension level.

Current source analysis:

- `AncientEventModel.BeforeEventStarted` (source code/src/Core/Models/AncientEventModel.cs:143-156) sets player HP to 0 via `SetCurrentHpInternal(0m)`, then heals via `CreatureCmd.Heal` to full (or 80% for A2+ WearyTraveler). This works in singleplayer.
- Vanilla `AscensionManager` (`source code/src/Core/Entities/Ascension/AscensionManager.cs`) has `maxAscensionAllowed = 10` and only handles A4 (TightBelt -1 potion) and A10 (AscendersBane). No HP effects.
- `RunManager.InitializeNewRun()` → `ApplyAscensionEffects(player)` → `AscensionManager.ApplyEffectsTo(player)` does not touch HP.
- `Player.CreateForNewRun()` uses `character.StartingHp` for both current and max HP.
- No EZMB gameplay slice touches player HP during run start or Neow.

Hypotheses (in priority order):
1. Vanilla multiplayer `CreatureCmd.Heal` or `SetCurrentHpInternal` fails/skips for the non-host player when `RunState.AscensionLevel > 10`, possibly because `NetService.Type.IsMultiplayer()` bypasses some initialization path.
2. v0.105.0 API drift: the `AncientEventModel` or `CreatureCmd.Heal` code path changed between the local v0.104.0 source snapshot and the installed v0.105.0 game, altering heal behavior.
3. Our `AscensionSelectionPatches` expand `maxMultiplayerAscensionUnlocked` during `UpdateMaxMultiplayerAscension` in a way that corrupts some lobby/player state before the run starts. Our patches do not touch `BeginRunForAllPlayers` directly (only log a warning).
4. A20 Dual King Brands warning patch or some other EZMB Harmony patch interferes with lobby cleanup or run setup in a non-obvious way.
5. The Neow event fails to start properly in multiplayer, so `BeforeEventStarted` never fires, and HP remains at whatever value was set during player creation (which should still be `StartingHp`).

Required evidence:
- Run with `EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS=1` in co-op to capture lobby state, player HP at run start, `BeginRunLocally` HP, `AfterActEntered` HP, and Neow `BeforeEventStarted` HP.
- Bisect via `EZMB_ASCENSION_DISABLE_ALL_SYSTEMS=1` to confirm whether EZMB gameplay slices are involved.
- Test with `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` + vanilla A10 as control.

### ISSUE-2026-05-08-MULTIPLAYER-SAVE-QUIT-NOT-PROPAGATING

Priority: P0/P1

Status: investigating; source evidence pending

Area: multiplayer save-and-quit / disconnect / host-client sync

Player report: in co-op, when one player saves and quits, the other machine does not synchronously quit, disconnect, or return to menu.

Current source analysis needed:
- `NSaveAndQuitButton.cs` exists but contains only whitespace in the local v0.104.0 source snapshot; save/quit flow may be in other files.
- `NGame.Quit()` (source code/src/Core/Nodes/NGame.cs) saves settings and calls `GetTree().Quit()` but does not send a disconnect message to remote peers.
- `StartRunLobby.CleanUp(bool disconnectSession)` can disconnect the network session.
- `RunManager.LocalPlayerDisconnected` handles peer disconnection events.
- `NetService.Disconnect(NetError.Quit)` may not propagate to remote clients properly.

Required investigation:
- Search `source code/src/Core/` for save-quit, disconnect, quit-to-menu flow specific to multiplayer.
- Determine if this is vanilla behavior (never intended to sync quit), mod-introduced (our patches break disconnect), or A11-A20 state-related (ascension > 10 corrupts save state).
- If vanilla behavior, document expected workflow (each player must manually quit) and do not claim fix.

### ISSUE-2026-05-08-MULTIPLAYER-RUN-START-BLACK-SCREEN

Priority: P0/P1

Status: investigating; may be same root cause as HP0-Neow issue or separate

Area: multiplayer run start / screen transition / mod load / A20 BossSeal type load

Player report: multiplayer run start can still black-screen, even after the earlier `DoormakerBoss` TypeLoadException fix.

Current status:
- `ISSUE-2026-05-08-MULTIPLAYER-A20-BLACK-SCREEN-OPTIONAL-BOSS-TYPELOAD` was fixed by making `BossSealCatalog` use runtime-safe `ModelId` strings. This fixed the TypeLoadException for `DoormakerBoss`.
- But the player report suggests black screen can still occur, potentially from other causes.

Hypotheses:
1. HP 0/80 → Neow blocked → screen transition never completes (same root cause as HP0-Neow issue).
2. A different TypeLoadException or missing model for a different v0.105.0 API.
3. Network desync during run start — host reaches Act 0 but client never receives the transition.
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

- The repository source snapshot contains newer Early Access boss types such as `DoormakerBoss`, but the currently installed Steam game DLL does not expose every same type/member.
- `BossSealCatalog` used hard generic references like `ModelDb.GetId<DoormakerBoss>()`; static initialization therefore crashed before the run could finish generating the first map.
- Current build also proved adjacent API drift: direct `Doormaker` / `HungerPower` / `ScrutinyPower` / `GraspPower` references and direct `PumpkinCandle.ActiveAct` access are not safe against the installed DLL.

Current source fix:

- `BossSealCatalog` now uses runtime-safe `ModelId` strings such as `ENCOUNTER.DOORMAKER_BOSS` instead of hard references to optional boss encounter classes.
- Door Wedge combat checks now use runtime `ModelId` checks for the Doormaker monster and phase powers, so missing optional types do not block compile/load.
- Debt and Pumpkin Candle patches were adjusted to avoid direct compile/accessibility assumptions that broke against the current installed game API.
- Pumpkin Candle room-entry patching now resolves the declared Pumpkin Candle method when present, otherwise falls back to patching `AbstractModel.AfterRoomEntered` with a Pumpkin-only guard, so `PatchAll()` does not fail when the subclass override is absent.
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

Status: source-patched; package/smoke refresh pending

Area: A11-A20 selector gate / multiplayer pre-release testing

Decision: A11-A20 selection is now default-on in this private-beta multiplayer test candidate so testers can immediately exercise single-player and host-multiplayer A11-A20 through the original lobby UI.

Required behavior:

- Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison.
- Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection while leaving single-player A11-A20 available.
- `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.
- A20 multiplayer selection is not full A20 co-op support. Dual King Brands / second-boss Brand gameplay remains disabled or downgraded in co-op pending live verification.
- Controlled smoke passed is not the same as normal Steam-client Mod Settings or live co-op verification.

Manual retest:

- With no Ascension env vars, confirm single-player and host multiplayer can select A11-A20.
- With `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1`, confirm single-player and multiplayer selection return to vanilla A1-A10.
- With `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`, confirm single-player A11-A20 remains available and host-multiplayer selection returns to the vanilla cap.
- Confirm host-only multiplayer A20 selection logs the downgrade warning before any client joins, then logs again on run start after a client joins.
- Keep normal Steam-client Mod Settings, live gameplay, save/load, and live co-op/desync verification pending until actually executed.

### ISSUE-2026-05-07-A11-MAP-LENGTH-NOT-PLAYER-VISIBLE

Priority: P1

Status: source-patched again; live verification pending

Area: A11 Wide Tower, Long Road / map generation

Player report: A11 still looks like the original map size. It was longer once, then regressed.

Current source fix:

- `AscensionFeatureGate` now sets A11 rows to Act 1 `+1`, Act 2 `+1`, Act 3 `+2`.
- `AscensionMapService` now accepts old width-only adjusted maps and inserts missing late rows instead of returning early.
- A11 still expands from 7 to 8 columns and inserts a reachable optional route node.
- A11 no longer marks ordinary route nodes with a dedicated long-road marker or hover explanation; map growth is represented only by vanilla-looking rows, columns, nodes, and paths.

Manual retest:

- Start a fresh A11 run and inspect Act 1 for one extra late route layer.
- Continue to Act 2 and Act 3; Act 3 should be visibly longer than Act 1/2.
- Confirm the map still has a low-risk route and boss reachability.

### ISSUE-2026-05-07-A11-MAP-CHANGE-ANIMATION

Priority: P3

Status: controlled-smoke refreshed; normal Steam-client runtime still pending

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

Status: source-patched; live verification pending

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

Status: source-patched; live verification pending

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

Status: source-patched; live verification pending

Area: A14/A15/A18 Rootblight and Blight Sprout

Player report: Root Bud and Rootblight were conceptually unclear, Boss Sprout count was too low, and Boss/Elite Sprout text was too long.

Current source fix:

- ZHS player-facing term is now `根芽`.
- Boss fights in Acts 2/3 now seed 2 Root Bud cards.
- Root Bud text is shortened: play to Exhaust; Boss sprouts use rounds 3/4 and elite sprouts use round 3; if seen and not played, add Rootblight I after combat.
- Rootblight I/II/III costs are 2/3/4.
- Played Rootblight removes its master-deck card and queues the downgrade card after combat.
- Unplayed Rootblight I/II upgrades after combat; ignored Rootblight III stays III and adds one Rootblight I only once per card.
- Rootblight is capped at 4 cards, and cap hits show `Root system full.` / `根系已满。`.
- Rest removes exactly one highest-stage Rootblight instead of clearing all Rootblight.

Manual retest:

- A14 new run starts with Rootblight I.
- A15 Act 2/3 Boss fights bury 2 Root Buds.
- A18 eligible Act 2/3 Elite fights bury 1 Root Bud.
- Seen-but-unplayed Root Bud adds one Rootblight I after combat.
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

### ISSUE-2026-05-07-A11-LONG-ROAD-MAP-MARKER-UNWANTED

Priority: P2

Status: source-patched; live verification pending

Area: A11 Wide Tower, Long Road / map UI

Player report: A11 should make the map longer/wider through normal map geometry only. It should not put a special visible marker or hover tooltip on the map just to explain the extra route space.

Desired behavior:

- Remove the dedicated A11 long-road map marker/hover indicator from newly inserted route nodes.
- A11 map changes should look like vanilla map rows and paths, not like a special event or quest node.
- Keep the actual map-length/width tuning separately testable; if final tuning is only one added row, update docs/localization/changelog to avoid claiming larger route growth.

Implementation notes:

- `LongRoad` metadata, `MarkLongRoad`, and `LONG_ROAD_NODE` localization were removed from active source/resources.
- `AscensionMapQuestMarker` remains in use only for A17 Deep Branch generic markers; A12 Firemark, A16 Banner, A17 Deep Branch, A19 Seal, and A20 Brand indicators remain on their own paths.

Manual retest:

- Start A11 and inspect all acts' maps.
- Confirm the map has the intended route geometry change.
- Confirm no A11-specific icon, marker, or hover tooltip appears on ordinary route nodes.
- Confirm Firemark/Banner/Deep Branch/Boss Seal indicators still appear when their own Ascensions are enabled.

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

### ISSUE-2026-05-07-RELEASE-ARTIFACT-TESTS-DEPEND-ON-IGNORED-PUBLISH-OUTPUT

Priority: P2

Status: source-patched and locally validated; clean-clone verification pending

Area: automated tests / release artifact validation

Audit finding: `.gitignore` excludes `publish/`, `*.zip`, `*.dll`, and `*.pck`, while some release guard tests require installed/staging/versioned zip artifacts. This is useful for release validation on the maintainer machine, but it can make normal `dotnet test` brittle in a clean clone unless package generation ran first.

Desired behavior:

- Normal `dotnet test` should pass in a clean clone or clearly skip release artifact tests when ignored package artifacts are absent.
- Release artifact tests should run behind an explicit opt-in such as `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1`, or after a documented package refresh command.
- Release docs should state the exact command order for artifact validation.

Implementation notes:

- Normal `dotnet test` no longer requires ignored publish/package artifacts because package/hash/runtime-smoke checks are marked with `ReleaseArtifactFactAttribute`.
- Release artifact tests are opt-in with `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1`.
- If the environment variable is set, package hash checks remain strict and missing artifacts fail with the test's missing-file assertion.
- Do not weaken source/localization/package coverage silently; everyday source/localization/docs guards remain normal tests.

### ISSUE-2026-05-07-CURRENT-PACKAGE-RUNTIME-SMOKE-STALE

Priority: P1

Status: controlled-smoke refreshed; Steam-client and live gameplay verification pending

Area: controlled runtime smoke / SavedSpireField registration

Audit finding: several docs cited a prior controlled `--force-steam off` smoke with an obsolete SavedSpireFields count. The current source/package defines 12 SavedSpireFields after Rootblight v2.2 card-state fields.

2026-05-08 update:

- Current controlled `--force-steam off` smoke passed after publish/package refresh.
- Temporary profile settings enabled only `BaseLib` and `EZMicroBalance`, explicitly disabled other discovered local mods, and restored `settings.save` plus `settings.save.backup` byte-for-byte.
- `godot.log` showed `Loaded 2 mods (19 total)`, BaseLib initialization, EZ Micro Balance DLL/PCK load/init, `Found 12 SavedSpireFields`, default-on Ascension initializer wording with 0 old `Default-off gate` lines, main menu in `13,201ms`, and 0 EZ Micro Balance error/exception lines.
- Normal Steam-client Mod Settings and live gameplay verification remain pending.

Required verification:

- Publish the current package.
- Launch controlled `--force-steam off` with only BaseLib and EZ Micro Balance enabled.
- Inspect `godot.log`.
- Record the current SavedSpireField count.
- Confirm no EZ Micro Balance startup error or exception.
- Keep normal Steam-client Mod Settings verification as a separate pending gate.

### ISSUE-2026-05-07-HANDOFF-GIT-STATUS-HYGIENE

Priority: P3

Status: refreshed for current default-on follow-up; recheck before commit/push

Area: release handoff / repository status docs

Audit finding: handoff and audit docs can become stale when they say "No commit or push has been made" or "worktree dirty." Final release handoff must re-check the current status rather than relying on old wording from an earlier local snapshot.

2026-05-08 update:

- Local `main` was observed at `77da0ed (HEAD -> main, origin/main, origin/HEAD) fix2` before the default-on multiplayer-test-candidate follow-up changes.
- The previously untracked spec, `docs/skills/`, and `ReleaseArtifactFactAttribute.cs` are no longer untracked in this checkout.

Required release-pass action:

- Run `git status --short --branch` and `git log -1 --oneline --decorate`.
- Update `docs/private-beta-verification-handoff.md` and `docs/features/ancients-rework-v4/completion-audit.md` with the actual current commit/worktree state.
- Do not fabricate commit or push status.

## Resolved / Player-Verified

### ISSUE-2026-05-07-A12-FORGE-TOKEN-RESTSITE-CRASH

Priority: P1

Status: player reported fixed on 2026-05-07; keep in regression list

Area: A12 Forge Token / rest-site transition

Player verification: carrying Forge Token into a rest site no longer crashes.

Regression retest:

- Enter a rest site while holding Forge Token.
- Test Rest payout and Smith payout separately.
- Confirm token relic is removed after payout.
