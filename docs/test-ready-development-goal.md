# Spire Plus Test-Ready Development Goal

Goal: keep the current `Spire Plus` workspace at a user-test-ready manual test build, with source, resources, package, docs, and automated guards aligned for the user's manual test pass.

Current stop line: Codex should not chase release-ready evidence in this pass. The user will run live/manual testing. This is not a release-ready claim.

Current package target remains `publish/SpirePlus-v0.1.0-private-beta.0.zip`. The stable technical manifest id remains `EZMicroBalance`; do not edit it in place.

## Current State

- Urda, Morvi, and Lotha are default-on test slices with visible marker relics for selected Ancient rewards.
- Vakuu fight is hidden by default and can be enabled only through explicit fight gates. It has a dedicated source enemy and scene, but victory return, no-black-screen, save-load, failure/death path, and co-op behavior still need live proof.
- Ancient reward selections should remain visible from the relic bar whenever the design grants a lasting reward.
- Final browser GPTimage2 small art generated this pass is the current small-art baseline. No `generic_temporary` or `final_required_before_release` art blockers remain. Event backgrounds are active middle-draft resources. Live clicked-UI review remains unresolved.
- current source defines 25 SavedSpireFields. The earlier 22-field loader smoke is historical evidence only; a fresh live loader rerun is still pending for current-package runtime parity.
- Preview tools are now part of the single `Spire Plus / EZMicroBalance` mod. Crystal Sphere peek and transform preview live under `EZMicroBalanceCode/Preview/`; live proof is still pending.

## Required Reading

Read before code changes: `PROJECT_STATE.md`, `AGENTS.md`, `docs/issues.md`, `docs/review.md`, and this file.

Read feature docs only when touching that feature: Ancient docs under `docs/features/ancient-expansion-v2.2/`, Ascension docs under `docs/features/ascension-11-20/`, localization rules in `docs/style/card-localization-style-guide.md`, and the local modding reference at `docs/skills/sts2-godot-mod-development.md`.

For Ascension map, UI, reward, combat, save-load, or hook behavior, `source code/src/Core/**` is the primary source evidence. Historical prompt dumps and archived audits are not default reading.

## Current Refactor Plan

Approach: fix proven logic issues first, then reduce coupling in small source-preserving cuts. Each cut must build, keep guard coverage, and avoid broad rewrites while the worktree is dirty.

Active documents have narrow jobs: `docs/issues.md` tracks open blockers, `docs/toreview.md` tracks fixed items awaiting user retest, `docs/review.md` tracks source review and validation history, and this file tracks only the current goal and boundaries.

Next source cleanup candidates: keep Root Sight, Seed Bank, Seedbed, Trial Branch, Rooted Route, Morvi card-state, Lotha combat-state, Vakuu child-combat, Banner, Firemark, and Boss Seal ownership split by feature. Extract one helper group at a time, preserve save-field formats unless a bug requires migration, replace single-file assertions with source-tree assertions before moving partial files, and keep release artifact tests opt-in when they depend on installed package files.

Validation rule: code/config changes run build, normal tests, format, and diff check. Resource, localization, manifest, export, or package changes also run publish, package refresh, and opt-in artifact tests. Live claims require live evidence.

## Current Logic Watchlist

These are source-sensitive areas. Touch them only with local source evidence and tests.

- Vakuu child combat does not call Core's `EnterCombatWithoutExitingEvent(...)`. It clears the parent event `Node`, uses direct `EnterRoomWithoutExitingCurrentRoom(...)`, and does not store `ParentEventId` while the combat room is active. The no-reward victory path resumes when the previous-room stack is valid and falls back to the map only if that stack is missing. Live victory and save/load proof remain pending.
- Lotha Death Reprieve mirrors pending/active/resolved phase through deck state. Live restore is still pending.
- Morvi Red Ink, Open Book, Blueprint Proof, Overdue Library, and Debt Settlement are source-hardened, but live restore and card-play freeze reports remain pending until user tests confirm.
- Urda Root Sight now opens map selection from the Root Eyes relic, lets the player choose any future reachable Monster/Unknown/Elite node, and stores a concrete enemy group or event on that node. Normal/elite previews pick from the generated Act room set; Unknown previews use a fork of `runState.Rng.UnknownMapPoint`, exclude Shop/Treasure/Rest/Boss, commit one live Unknown RNG/odds step only when the marked node is entered, respect event-selection hooks, reserve marked future results from earlier non-preview rooms where possible, and accept repeated events only when Core would allow repeats after unique events are exhausted. Map clicks are caught even when normal travel is disabled, selected Unknown nodes show the stored Monster/Elite/Event-style icon, saved markers restore when the map is generated or loaded, and closing the map cancels selection. Root Sight's one-shot entry commits are scoped to the current `RunState` instance and transient state resets when Root Sight is granted. Seed Bank, Seedbed, Trial Branch, and Rooted Route are source-backed. Live hover, map click, Boss entry, and save-load behavior remain pending.
- Ancient RunHook cleanup is in place: Morvi, Lotha, and Urda RunHooks own run lifecycle, reward, damage, death, and cleanup paths, while CombatHooks own combat-only card, turn, cost, draw, and Power paths. Keep this ownership split guarded during later refactors.
- Inline Simplified Chinese power hover text for Banner, Firemark, and Boss Seal source `PowerLoc` strings is readable and should stay aligned with the v3.2 player terms.
- A11-A20 Ascension slices are development-test features. Do not claim release readiness for A11-A20 without runtime evidence.

## Player Text Rules

Visible text should read like a player-facing game description.

- Short sentences.
- One term per mechanic.
- Numbers stay blue and important gameplay terms stay gold where existing rich-text rules allow.
- Avoid implementation terms such as source-safe, fallback, host, backend, route graph, setup window, burst window, debug, candidate, and holding area.
- Chinese player-facing text must avoid negative-to-corrective contrast phrasing; prefer direct effect text.
- Tooltips should say what happens and what the player can do.

Known wording commitments:

- Trial Branch / 试炼枝条: choose 1 of 4 cards, upgrade it, add it to the deck, then prove it in the next 3 combats or it is removed.
- A12 uses `火印精英` in Ascension-level text.
- A15 says Act 2 and Act 3 Boss combats bury two Blight Sprouts.
- A16 says Banner Rooms have extra rewards.
- A17 says the special route is more dangerous and more rewarding.
- A19 says each Boss gains a Royal Seal / 王印.
- A20 says the final Act 3 Boss upgrades its Royal Seal into a King Brand / 王烙印.
- Holy Daze, Struggle Bait, and Residual Sample need concrete effect text, not design commentary.

## Manual Evidence Still Pending

Keep these rows open until the user supplies runtime evidence:

- clicked Ancient UI screenshots, relic-bar visibility, and hover readability for Urda, Morvi, Lotha, Vakuu normal, and Vakuu fight;
- live gameplay for Urda, Morvi, Lotha, gated Vakuu fight, Vakuu victory/no-black-screen, failure/death paths, disable-mod gameplay, and co-op disposition;
- save/load for Ancient player state, deck mirrors, Root Sight, Seed Bank, Morvi state, Lotha Death Reprieve, Vakuu child combat, and Rootblight;
- A11 natural route traversal, Ascension map hover behavior, Rootblight visual behavior, and fresh live loader smoke for the current 25-field package.

## Validation Commands

Use this sequence after code/config changes:

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

Use this after resource, localization, manifest, export, or package changes:

```powershell
dotnet publish EZMicroBalance.sln
.\scripts\package-spire-plus.ps1
$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build
Remove-Item Env:\EZMB_RUN_RELEASE_ARTIFACT_TESTS
```

No live-game, save-load, death/failure, or co-op evidence may be claimed from these commands.

## Current Documentation Cleanup

Compact active summaries are in `docs/features/ascension-11-20/development-checklist-v2.md`, `docs/features/ancients-rework-v4/source-design.md`, and `docs/review.md`. Full historical drafts are archived under `docs/archive/feature-inputs/` or `docs/archive/feature-audits/`. Do not expand compact checklists back into GDDs.

## Final Report Requirements

Final development reports should state files changed, logic bugs fixed, coupling or document-bloat reduction, automated validation result, live checks not run, and remaining user-test blockers.
