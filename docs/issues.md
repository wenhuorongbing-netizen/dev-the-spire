# EZ Micro Balance Issues

This file tracks player-reported and runtime-observed issues. Do not mark an item release-ready unless source validation and live verification both support it.

## Open

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

- A11-A20 selection is available in multiplayer when the explicit public/development Ascension gate is enabled.
- Multiplayer selection must not patch or corrupt vanilla A1-A10 progress.
- Earlier Ascension effects still inherit normally at higher levels.
- Per-player systems such as Rootblight and Blight Sprout remain independent and do not desync.
- A21-A30 remains out of scope.

Implementation notes:

- Local source inspection found that `StartRunLobby.UpdateMaxMultiplayerAscension()` computes the multiplayer cap from each `LobbyPlayer.maxMultiplayerAscensionUnlocked`, while `UpdatePreferredAscension()` writes host selections to `PreferredMultiplayerAscension`.
- Current source patch expands only host multiplayer lobbies when `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is set, temporarily raises in-memory lobby unlock caps only during max recomputation, restores them in a finalizer, and skips A11-A20 preferred-progress writes.
- Host-multiplayer A11-A20 selection is independently disableable with `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`.
- A11-A20 gameplay, per-player Rootblight/Blight Sprout ownership, and desync behavior still require live co-op verification.
- A20 Dual King Brands gameplay is still single-player gated through `IsDualKingBrandsSinglePlayerEnabled(...)`; the host multiplayer selector/start path now logs a development-testing downgrade warning, but live co-op verification is still pending.

Manual retest:

- Host a multiplayer lobby with BaseLib and EZ Micro Balance enabled.
- Confirm A1-A10 behavior is unchanged with the gate disabled.
- Enable the development/public A11-A20 gate and confirm the lobby can select A11-A20.
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

- In a host multiplayer lobby with `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1`, select A20 before any client joins.
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

- Gate off: multiplayer selection remains vanilla A1-A10.
- Gate on: host can select A11-A20 and client sees the selected value.
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
- `godot.log` showed `Loaded 2 mods (19 total)`, BaseLib initialization, EZ Micro Balance DLL/PCK load/init, `Found 12 SavedSpireFields`, main menu in `12,886ms`, and 0 EZ Micro Balance error/exception lines.
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

Status: needs refresh after current commit/push state is known

Area: release handoff / repository status docs

Audit finding: handoff and audit docs can become stale when they say "No commit or push has been made" or "worktree dirty." Final release handoff must re-check the current status rather than relying on old wording from an earlier local snapshot.

2026-05-08 update:

- Local `main` was observed at `212ba0d (HEAD -> main, origin/main, origin/HEAD) fix2` before the A20 warning-condition follow-up changes.
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
