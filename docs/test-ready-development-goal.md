# Spire Plus Test-Ready Development Goal
Goal: keep the current `Spire Plus` workspace at a user-test-ready manual test build, with source, resources, package, docs, and automated guards aligned for the user's manual test pass.
Current stop line: Codex should not chase release-ready evidence in this pass. The user will run live/manual testing. This is not a release-ready claim. Current package target is `publish/SpirePlus-v0.1.0-private-beta.44.zip`. The stable technical manifest id remains `EZMicroBalance`; do not edit it in place.
## Current State
- Urda, Morvi, and Lotha are default-on test slices with visible marker relics for selected Ancient rewards.
- Vakuu fight is hidden by default and can be enabled only through explicit fight gates. It has a dedicated source enemy and scene, but victory return, no-black-screen, save-load, failure/death path, and co-op behavior still need live proof.
- Ancient reward selections should remain visible from the relic bar whenever the design grants a lasting reward.
- Final browser GPTimage2 small art generated this pass is the current small-art baseline. No `generic_temporary` or `final_required_before_release` art blockers remain. Event backgrounds are active middle-draft resources. Live clicked-UI review remains unresolved.
- The latest Steam-client loader smoke under `.tools/runtime-evidence/beta19-loader-smoke-20260525-213336` reported `v0.1.0-private-beta.19`, `Found 30 SavedSpireFields`, only BaseLib plus Spire Plus loaded, clean log audit, startup completion, game stop, and restored mod isolation for the beta.19 package. The current beta.44 package still needs fresh loader proof. Older beta.17, beta.13, `20260524-161744`, 22-field, and `20260523-current` loader smokes remain historical evidence only.
- Preview tools are now part of the single `Spire Plus` mod. Crystal Sphere peek and transform preview live under the technical `EZMicroBalanceCode/Preview/` source folder; they run in co-op as local UI-only previews, but live two-client proof is still pending.
- Multiplayer gameplay is fail-closed after the 2026-05-25 crash logs. A11-A20 co-op selection/gameplay, Ascension map/reward mutations, Ancient offers/selections/run hooks, Urda reward alternatives, and combat hooks are disabled by default until two-client proof exists. Preview tools are the narrow exception: Crystal Sphere peek and transform preview are allowed because they only affect local UI and do not add choices, rewards, or real RNG calls.
- The two crash logs are latest Spire Plus content evidence even if the in-game beta label is stale. Single-player remains enabled; co-op debug opt-ins should log `coop_gameplay_disabled` or `coop_combat_hook_disabled` when left unset. Preview tools should log local UI evidence such as `coop_local_ui_preview_enabled` or `prediction_prepared_multiplayer_ui_only`.

Manual test controls do not prove live behavior by themselves: `SPIREPLUS_FORCE_ANCIENT=URDA|MORVI|LOTHA|VAKUU`, `SPIREPLUS_FORCE_MORVI_BLESSING=morvi_forbidden_loan`, `SPIREPLUS_FORCE_LOTHA_BLESSING=lotha_death_reprieve`, `SPIREPLUS_DISABLE_URDA=1`, `SPIREPLUS_ENABLE_VAKUU_FIGHT=1`, `SPIREPLUS_RELEASE_EVIDENCE_LOG=1`, and the gameplay/combat `SPIREPLUS_ALLOW_UNVERIFIED_COOP_*` env vars only for deliberate two-client debugging. Legacy `EZMB_*` aliases still work. Ancient reward/fight option selection logs include the Ancient, blessing id or option id, selected marker relic type, forced flag, run id, player slot, and network mode.
## Required Reading
Read before code changes: `PROJECT_STATE.md`, `AGENTS.md`, `docs/issues.md`, `docs/review.md`, and this file.

Read feature docs only when touching that feature: Ancient docs under `docs/features/ancient-expansion-v2.2/`, Ascension docs under `docs/features/ascension-11-20/`, localization rules in `docs/style/card-localization-style-guide.md`, and the local modding reference at `docs/skills/sts2-godot-mod-development.md`.

For Ascension map, UI, reward, combat, save-load, or hook behavior, `source code/src/Core/**` is the primary source evidence. Historical prompt dumps and archived audits are not default reading.

## Current Refactor Plan

Approach: fix proven logic issues first, then reduce coupling in small source-preserving cuts. Each cut must build, keep guard coverage, and avoid broad rewrites while the worktree is dirty.

Active documents have narrow jobs: `docs/issues.md` tracks open blockers, `docs/toreview.md` tracks fixed items awaiting user retest, `docs/review.md` tracks compact current source-review findings and manual-proof focus, and this file tracks only the current goal and boundaries.

Next source cleanup candidates: keep Root Sight, Seed Bank, Seedbed, Trial Branch, Rooted Route, Morvi card-state, Lotha combat-state, Vakuu child-combat, Banner, Firemark, and boss dedicated ability ownership split by feature. Extract one helper group at a time, preserve save-field formats unless a bug requires migration, replace single-file assertions with source-tree assertions before moving partial files, and keep release artifact tests opt-in when they depend on installed package files.

Validation rule: code/config changes run build, normal tests, format, and diff check. Resource, localization, manifest, export, or package changes also run publish, package refresh, and opt-in artifact tests. Live claims require live evidence.

## Design Conflict Governance

When active docs disagree, use this order: current source/tests/package hashes, `docs/issues.md`, this goal, `docs/features/ancient-expansion-v2.2/source-design.md`, then feature support docs. Archive and reference-input files are history unless a current doc explicitly promotes them.

Current decisions: Preview tools are integrated into `EZMicroBalanceCode/Preview/`; old standalone Future Peek advice is superseded. v3.3 Seedbed/Vakuu/Closed Court/Mirror/Rain decisions supersede older Urda-only reward-alternative drafts. `docs/features/ancient-expansion-urda/` remains support evidence for Urda hooks and tests, not the primary design authority when it conflicts with the combined v2.2/v3.3 docs.

Do not delete or move docs that tests still read. First update the current authority note, then change tests and archive paths in a separate small cleanup.

## Current Logic Watchlist

These are source-sensitive areas. Touch them only with local source evidence and tests.

- Vakuu child combat does not call Core's `EnterCombatWithoutExitingEvent(...)`. It clears the parent event `Node`, uses direct `EnterRoomWithoutExitingCurrentRoom(...)`, and does not store `ParentEventId` while the combat room is active. The no-reward victory path resumes when the previous-room stack is valid and falls back to the map only if that stack is missing. Live victory and save/load proof remain pending.
- Lotha Death Reprieve mirrors pending/active/resolved phase through deck state. Live restore is still pending.
- Morvi Red Ink, Open Book, Blueprint Proof, Overdue Library, and Debt Settlement are source-hardened, but live restore and card-play freeze reports remain pending until user tests confirm.
- Urda Root Sight now opens map selection from the Root Eyes relic, lets the player choose any future reachable Monster/Unknown/Elite node, and stores a concrete enemy group or event on that node. Normal/elite previews pick from the generated Act room set; Unknown previews use a fork of `runState.Rng.UnknownMapPoint`, exclude Shop/Treasure/Rest/Boss, commit one live Unknown RNG/odds step only when the marked node is entered, respect event-selection hooks, reserve marked future results from earlier non-preview rooms where possible, and accept repeated events only when Core would allow repeats after unique events are exhausted. Map clicks are caught even when normal travel is disabled, selected Unknown nodes show the stored Monster/Elite/Event-style icon, saved markers restore when the map is generated or loaded, and closing the map cancels selection. Root Sight can share a node with Firemarked Elite, Banner, and Deep Branch markers; one shared map hover stack shows every contributed marker text, while the icon lane keeps the original marker and adds a small Root Eyes badge when needed. Root Sight's one-shot entry commits are scoped to the current `RunState` instance and transient state resets when Root Sight is granted. Multiplayer queue mutation remains gated until host-authoritative preview sync is proven. Seed Bank, Seedbed, Trial Branch, and Rooted Route are source-backed. Seedbed can now plant Rootblight that would enter hand; that Rootblight is removed from the current combat and its master-deck version skips combat-end growth for that battle only. Live hover, map click, Boss entry, and save-load behavior remain pending.
- Ancient RunHook cleanup is in place: Morvi, Lotha, and Urda RunHooks own run lifecycle, reward, damage, death, and cleanup paths, while CombatHooks own combat-only card, turn, cost, draw, and Power paths. Keep this ownership split guarded during later refactors.
- Inline Simplified Chinese power hover text for Banner, Firemark, and boss dedicated ability source `PowerLoc` strings is readable and should stay aligned with the current player terms.
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
- A19 says each Boss gains its own dedicated ability / 首领专属能力.
- A20 says only the second Act 3 Boss enters Branded Form / 烙印形态 and strengthens that Boss's dedicated ability.
- Holy Daze, Struggle Bait, and Residual Sample need concrete effect text, not design commentary.

## Manual Evidence Still Pending
Keep these rows open until the user supplies runtime evidence:

- clicked Ancient UI screenshots, relic-bar visibility, and hover readability for Urda, Morvi, Lotha, Vakuu normal, and Vakuu fight;
- live gameplay for Urda, Morvi, Lotha, gated Vakuu fight, Vakuu victory/no-black-screen, failure/death paths, disable-mod gameplay, and co-op disposition;
- save/load for Ancient player state, deck mirrors, Root Sight, Seed Bank, Morvi state, Lotha Death Reprieve, Vakuu child combat, and Rootblight;
- A11 natural route traversal, Ascension map hover behavior, and Rootblight visual behavior.

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
$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build
Remove-Item Env:\SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS
```

No live-game, save-load, death/failure, or co-op evidence may be claimed from these commands.

## Current Documentation Cleanup

Compact active summaries are in `docs/features/ascension-11-20/development-checklist-v2.md`, `docs/features/ancients-rework-v4/source-design.md`, and `docs/review.md`. Full historical drafts are archived under `docs/archive/feature-inputs/` or `docs/archive/feature-audits/`. Do not expand compact checklists back into GDDs.

## Final Report Requirements

Final development reports should state files changed, logic bugs fixed, coupling or document-bloat reduction, automated validation result, live checks not run, and remaining user-test blockers.
