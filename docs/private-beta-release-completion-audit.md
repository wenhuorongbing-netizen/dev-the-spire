# Spire Plus Private Beta Release Completion Audit

Date: 2026-05-15

This audit maps the previous broad finish-project goal to concrete private-beta release deliverables. It is not the current Codex target after the 2026-05-15 reset to user-run manual testing. It is stricter than `docs/test-ready-completion-audit.md`: test-ready means source-complete and packaged for manual testing; private-beta release-ready requires live gameplay, save/load, and multiplayer evidence or an explicit release decision to defer each missing gate.

## Objective Restated

Concrete private-beta completion criteria:

- Keep `EZMicroBalance` as the stable manifest/package id and `Spire Plus` as the player-facing name.
- Ship one Spire Plus package as the only active mod surface.
- Keep out-of-scope systems out of this cycle: no A21-A30, no custom character, no manifest-id migration, no copied official assets, and no large copied game-source bodies.
- Package the current private-beta surfaces: Ancient reward rebalance v4.3, Urda default-on eleven-blessing slice, Morvi default-on source-complete/live-pending slice, Lotha default-on source-complete/live-pending slice, the hidden-by-default dedicated Vakuu fight slice, Ascension A11-A20 slices, Rootblight/Blight Sprout behavior and art, English/zhs localization, release docs, and tester handoff.
- Prove the installed package builds, publishes, hashes, loads through Steam with only BaseLib plus Spire Plus, and has no release-blocking loader errors.
- Complete or explicitly release-note every live/manual gate: Ancient reward runtime matrix, save/load-sensitive rows, disable-mod gameplay, post-fix Urda/Rootblight gameplay and visuals, natural A11 traversal, and two-client co-op/multiplayer.
- Leave a clean release handoff: worktree/commit/push state must be intentional, and validated implementation passes should be pushed.

## Objective Coverage Recheck

| Active objective item | Evidence inspected | Status |
| --- | --- | --- |
| Vakuu dedicated combat loop | Source now has `VakuuTrialMonster.cs`, `VakuuFightEncounter.cs`, `VakuuFightPatch.cs`, `VakuuFightRunHook.cs`, `VakuuFightCombatHook.cs`, `VakuuContractService.cs`, a custom encounter scene, no-normal-reward routing, Contract pressure, and hidden-by-default enable/force gates. `VakuuLothaSaveRiskGuardTests` checks the Core event-combat shape and verifies active `ParentEventId` stays out of the live combat room while prefinished parent restore remains guarded. | Source/package guarded; not release-ready until live victory return, no-black-screen, active-fight/pre-finished save-load, and failure/death rows pass. |
| Ancient reward visibility | Urda/Morvi/Lotha/Vakuu selections route through visible option marker relics, and package/test docs require relic-bar hover verification. | Source guarded; live relic-bar visibility and hover readability remain pending. |
| Player text, UI, and resource routing | EN/zhs localization guards reject stale development wording and mojibake; art/resource guards keep clicked backgrounds, map/run-history icons, option relic art, cards, powers, and encounter art on separate paths; installed-PCK resource smoke exists. | Static/resource guarded; clicked Ancient screenshots, combat-scene screenshots, and live tooltip fit remain pending. |
| Automation and release evidence | `dotnet build`, normal tests, format, diff-check, opt-in artifact tests, publish, and package refresh pass for the current source/package. `scripts/verify-spire-plus-release-evidence.ps1` hashes the zip, rejects duplicate rows, wrong row kinds, empty/invalid files, invalid notes, undersized PNGs, evidence dirs outside the evidence root, and required-file/screenshot paths that escape their row evidence dir. Fresh path-containment negative smokes failed as designed under `.tools/runtime-evidence/release-ready-path-containment-smoke*`; the current beta.53 evidence audit still needs fresh loader/gameplay/UI/save-load/co-op proof and fails closed with 20 manual rows until evidence is added. | Automation gates work; release evidence is not complete. |
| Documentation and blockers | `docs/test-ready-development-goal.md`, `docs/issues.md`, `docs/release-checklist.md`, this audit, the Ancient manual checklist, and the release evidence verifier all keep manual rows open. | Correctly blocks release-ready claims. |
| Clean handoff | Earlier dirty implementation batches have been split, validated, committed, and pushed on `main`. Final private-beta release handoff must still recapture `git status --short --branch`, current HEAD, and push state after the last validation pass. | Not complete |

## Prompt-To-Artifact Checklist

| Requirement | Concrete evidence inspected | Status |
| --- | --- | --- |
| Stable manifest id | `EZMicroBalance.json` remains `id: EZMicroBalance`; docs state `Spire Plus` is the player-facing name only. | Pass |
| Single package | `EZMicroBalance/`, `EZMicroBalanceCode/`, `EZMicroBalance.csproj`, and `publish/SpirePlus-v0.1.0-private-beta.53.zip` with `EZMicroBalance/` as the install folder; duplicate root mod surfaces are removed from the active tree. | Pass |
| Build and publish | `dotnet build EZMicroBalance.sln --no-restore` and `dotnet publish EZMicroBalance.sln --no-restore` passed after the Ascension map marker ordering split; installed/staging/versioned/package hashes are in parity. | Pass for build/publish/package |
| Latest source/package refresh | Source review confirms visible Ancient marker relics, current EN/zhs text guards, separate art/resource routes, dedicated gated Vakuu fight source, Root Eyes preview hardening, A11-A20 source guards, integrated preview-tool guards, and the short manual-test package README. Current zip SHA256 is `0EFDA36BB18A28C31474CA299341800F4F49B4B3F142D03E43EAAF1A52E07980`; detailed pass history lives in `docs/review.md` and `docs/archive/**`. | Pass for source/build/package artifact parity; gameplay verification pending |
| Current automated tests | `dotnet build EZMicroBalance.sln` passes with 0 warnings/errors; `dotnet test EZMicroBalance.sln --no-build` passes with 296 passed / 20 skipped; `dotnet test EZMicroBalance.sln -c Release` previously passed with 81 passed / 18 skipped; latest package refresh evidence includes `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` with 316 passed / 0 skipped after the beta.53 Aeonglass Hourglass split package sync. | Automated build/test/artifact pass; live/manual evidence pending |
| Formatting and diff hygiene | `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passes after the Ascension map marker ordering split; `git diff --check` passes with CRLF warnings only. | Pass |
| Package hash parity | `scripts/check-installed-spire-plus-package.ps1` confirms installed DLL, manifest, PCK, README, copied game-root ZIP hashes, Sere Talon imported textures, and Sere Talon / Tanx Claws PCK content match `docs/private-beta-verification-handoff.md`. | Pass |
| Steam loader evidence | Historical normal Steam helper startup/log evidence at `.tools/runtime-evidence/beta19-loader-smoke-20260525-213336` loaded exactly BaseLib plus Spire Plus under technical id `EZMicroBalance`, reported `v0.1.0-private-beta.19`, `Found 30 SavedSpireFields`, reached startup completion, restored settings and 24 moved mod entries, stopped the game, matched the beta.19 package hash, and audited clean with 0 `ERROR` / release-blocking hits. | Pass for beta.19 loader only; beta.53 loader and gameplay rows pending |
| Plug-off loader evidence | BaseLib-only normal Steam startup/log pass at `.tools/runtime-evidence/live-spire-plus-disabled-session-20260513-143020` temporarily isolated `EZMicroBalance` out of the mods folder, loaded `1 mods (1 total)`, did not initialize Spire Plus under technical id `EZMicroBalance`, restored settings/current-run/moved mods, and audited clean. The settings-only disabled attempt `.tools/runtime-evidence/live-spire-plus-disabled-session-20260513-142835` is invalid because Spire Plus still initialized. | Pass for loader; gameplay pending |
| Mod Settings evidence | Current list screenshot `.tools/runtime-evidence/current-spire-plus-modsettings-20260513-111342/02-mod-config-list.png` shows `Spire Plus`; historical page-level evidence remains under the old display name for the same manifest id. | Pass for visible list; current page-level display-name evidence remains historical |
| Urda asset regression | Source/package fix uses BaseLib custom Ancient asset paths; `.tools/runtime-evidence/urda-pck-resource-load-20260513-123345` resolves custom Urda scene/icon from the installed PCK with 0 `ERROR` / `WARNING` lines. | Pass for source/package/resource-load; live gameplay pending |
| Historical Ancient resource smoke | `.tools/runtime-evidence/current-package-smoke-20260514-015901` verified Urda/Morvi/Lotha Control-root scene paths, separate event/map/run-history/option art paths, large Lotha event art, exported option marker art, EN/zhs localization coverage, and headless installed-PCK loading for 3 Ancient scenes plus 43 textures with 0 errors/warnings. This remains resource-load evidence, not clicked live UI proof. | Pass for resource-load; clicked live UI pending |
| Live screenshot capture hygiene | `scripts/spire-plus-live-session.ps1` now preserves test-created current-run saves during restore, and `scripts/check-spire-window-preflight.ps1` reports foreground-window state before screenshot capture. Urda and Ascension manual checklists require the helper session, foreground preflight, copied/audited `godot.log`, and preserve-on-restore path before screenshots count. Smoke evidence `.tools/runtime-evidence/live-helper-preserve-current-run-smoke-20260513-133431` passed; `.tools/runtime-evidence/window-preflight-smoke-20260513-135402` reported `VampireSurvivors` foreground and Slay the Spire 2 not running. Invalid live screenshot attempts are not counted as gameplay evidence. | Pass for tooling; live gameplay pending |
| Ancient reward gameplay | `docs/features/ancients-rework-v4/manual-verification-matrix.md` has rows for implemented Ancient systems. | Not complete: runtime results remain pending |
| Save/load behavior | Save/load rows exist for Prismatic Gem, Pael's Tooth, Jeweled Mask, Debt, Folly, Jewelry Box, Urda/Morvi state, Rootblight/Blight Sprout, and A11/A20 surfaces. | Not complete: broader live save/load evidence pending |
| Disable-mod gameplay | Release checklist requires disabling Spire Plus and confirming gameplay patches/logs are inactive. Current BaseLib-only plug-off evidence covers startup/log only. | Not complete: actual run pending |
| Rootblight visuals and behavior | English/zhs hover/starter-notice spot checks exist; generated portraits are packaged; combat-end notice pipeline is source-hardened. | Not complete: post-fix live visual/gameplay, combat-end timing, Blight Sprout, and co-op ownership pending |
| Urda gameplay | Urda has source hooks, localization, disable gate, and resource-load evidence. | Not complete: live selection, reward-screen timing, room-entry rewards, act-transition cleanup, save/load, UI, and co-op pending |
| Ascension A11-A20 | Source slices and single-player spot checks exist; A11 saved-map boss-reachability graph proof exists. | Not complete: natural click-by-click traversal, full live Ascension matrix, and co-op verification pending |
| Multiplayer/co-op | Mismatch diagnostics are source-patched and docs provide a two-client runbook. | Not complete: two-client ownership/desync/save-load matrix pending |
| Out-of-scope systems excluded | Docs/tests guard no A21-A30 and no custom character; Lotha and single-player Vakuu fight are source-complete/live-pending test slices, not release-ready claims; no official assets are packaged. | Pass |
| Worktree/release handoff | The old dirty-worktree snapshot is no longer current; use the latest committed `main` baseline as the source of truth, then rerun `git status --short --branch` and record the pushed HEAD for the actual release handoff. | Not complete |

## Missing Or Weakly Verified Items

These block calling the whole project finished:

- Full Ancient reward runtime matrix.
- Ancient/Urda/Morvi reward-screen save/load rows.
- Disable-mod gameplay behavior in an actual run.
- Post-Lotha/Vakuu live load smoke for the latest package.
- Natural A11 click-by-click traversal.
- Post-fix live Urda selection and reward-screen behavior.
- Post-fix Rootblight generated-art visual check, combat-end behavior/notices, Blight Sprout behavior, and ownership checks.
- Two-client multiplayer/co-op matrix.
- Final release handoff capture: current `git status --short --branch`, validated HEAD, and pushed branch after the last validation pass.

Latest local screenshot attempts remain invalid: `.tools/runtime-evidence/live-urda-postfix-20260513-131752` stayed on the main menu and later captured the wrong surface, and `.tools/runtime-evidence/live-urda-continue-postfix-20260513-134337` was covered by another foreground app. Their clean logs may support loader health only; they do not satisfy live Urda, Rootblight, or gameplay rows.

## Conclusion

Not achieved. The current tree has the latest Urda/Morvi/Lotha/Vakuu source hardening build/publish/package-refreshed, historical loader/resource smoke remains useful, and the restore-safe live-session helper is ready for the next manual pass. It is not private-beta release-ready. The current Codex target is test-ready handoff; this release audit stays open until the live/manual blockers above are executed or explicitly release-noted as deferred by the project owner.
