# Spire Plus Private Beta Release Completion Audit

Date: 2026-05-14

This audit maps the active goal, "finish whole project off," to concrete private-beta release deliverables. It is stricter than `docs/test-ready-completion-audit.md`: test-ready means source-complete and packaged for manual testing; private-beta release-ready requires live gameplay, save/load, and multiplayer evidence or an explicit release decision to defer each missing gate.

## Objective Restated

Concrete private-beta completion criteria:

- Keep `EZMicroBalance` as the stable manifest/package id and `Spire Plus` as the player-facing name.
- Ship an independent package that can be enabled without the legacy `EzDailyContent` scaffold.
- Keep out-of-scope systems out of this cycle: no A21-A30, no custom character, no manifest-id migration, no copied official assets, and no large copied game-source bodies.
- Package the current private-beta surfaces: Ancient reward rebalance v4.3, Urda default-on ten-blessing slice, Morvi default-on source-complete/live-pending slice, Lotha default-on source-complete/live-pending slice, the single-player Vakuu fight source-complete/live-pending slice, Ascension A11-A20 slices, Rootblight/Blight Sprout behavior and art, English/zhs localization, release docs, and tester handoff.
- Prove the installed package builds, publishes, hashes, loads through Steam with only BaseLib plus Spire Plus, and has no release-blocking loader errors.
- Complete or explicitly release-note every live/manual gate: Ancient reward runtime matrix, save/load-sensitive rows, disable-mod gameplay, post-fix Urda/Rootblight gameplay and visuals, natural A11 traversal, and two-client co-op/multiplayer.
- Leave a clean release handoff: worktree/commit/push state must be intentional, and push requires explicit user approval.

## Prompt-To-Artifact Checklist

| Requirement | Concrete evidence inspected | Status |
| --- | --- | --- |
| Stable manifest id | `EZMicroBalance.json` remains `id: EZMicroBalance`; docs state `Spire Plus` is the player-facing name only. | Pass |
| Independent package | `EZMicroBalance/`, `EZMicroBalanceCode/`, `EZMicroBalance.csproj`, and `publish/SpirePlus-v0.1.0-private-beta.0.zip` with `EZMicroBalance/` as the install folder; release checklist records legacy `EzDailyContent` unchanged. | Pass |
| Build and publish | `dotnet build EZMicroBalance.sln` and `dotnet publish EZMicroBalance.sln` passed after the visible Ancient reward relic and package-intro pass; installed/staging/versioned/package hashes are in parity. | Pass for build/publish/package |
| Latest source/package refresh | Local source review confirmed Ancient reward selections now grant visible marker relics, and package review confirmed the short manual-test README. Package zip `EEE66FB09694E8A39D669CC8211032F35B13484E19D19E2A282D6EA01BB3C95E` includes this state plus the browser GPTimage2 small-art rebuild, custom card portraits, Rootblight/Blight Sprout state-hardening, A11 optional-route source proof, clicked-Ancient UI hardening, Vakuu/Lotha save-risk reduction, Lotha Power replacement polish, Public Evidence debuff-policy hardening, Morvi reward/state lifecycle hardening, art-manifest coverage, and player-facing localization scrub. | Pass for source/build/package artifact parity; gameplay verification pending |
| Current automated tests | `dotnet test EZMicroBalance.sln --no-build` passes with 152 passed / 18 skipped; `dotnet test EZMicroBalance.sln -c Release` previously passed with 81 passed / 18 skipped; opt-in artifact tests pass with 170 passed / 0 skipped after the latest `SpirePlus` package refresh. | Pass |
| Formatting and diff hygiene | `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passes; `git diff --check` passes with CRLF warnings only. | Pass |
| Package hash parity | `scripts/check-installed-ezmb-package.ps1` confirms installed DLL, manifest, and PCK hashes match `docs/private-beta-verification-handoff.md`. | Pass |
| Steam loader evidence | Current normal Steam helper startup/log pass at `.tools/runtime-evidence/current-package-smoke-20260514-015901` loads exactly BaseLib plus Spire Plus / `EZMicroBalance`, reports `Found 22 SavedSpireFields`, reaches main menu in `14,045ms`, restores settings/moved mods/current-run files, leaves 0 `SlayTheSpire2` processes, and audits clean with 0 `ERROR` / release-blocking / missing-resource hits. | Pass for loader |
| Plug-off loader evidence | BaseLib-only normal Steam startup/log pass at `.tools/runtime-evidence/live-spire-plus-disabled-session-20260513-143020` temporarily isolated `EZMicroBalance` out of the mods folder, loaded `1 mods (1 total)`, did not initialize Spire Plus / `EZMicroBalance`, restored settings/current-run/moved mods, and audited clean. The settings-only disabled attempt `.tools/runtime-evidence/live-spire-plus-disabled-session-20260513-142835` is invalid because Spire Plus still initialized. | Pass for loader; gameplay pending |
| Mod Settings evidence | Current list screenshot `.tools/runtime-evidence/current-spire-plus-modsettings-20260513-111342/02-mod-config-list.png` shows `Spire Plus`; historical page-level evidence remains under the old display name for the same manifest id. | Pass for visible list; current page-level display-name evidence remains historical |
| Urda asset regression | Source/package fix uses BaseLib custom Ancient asset paths; `.tools/runtime-evidence/urda-pck-resource-load-20260513-123345` resolves custom Urda scene/icon from the installed PCK with 0 `ERROR` / `WARNING` lines. | Pass for source/package/resource-load; live gameplay pending |
| Current Ancient resource smoke | `.tools/runtime-evidence/current-package-smoke-20260514-015901` verifies Urda/Morvi/Lotha Control-root scene paths, separate event/map/run-history/option art paths, large Lotha event art, exported option marker art, EN/zhs localization coverage, and headless installed-PCK loading for 3 Ancient scenes plus 43 textures with 0 errors/warnings. | Pass for resource-load; clicked live UI pending |
| Live screenshot capture hygiene | `scripts/spire-plus-live-session.ps1` now preserves test-created current-run saves during restore, and `scripts/check-spire-window-preflight.ps1` reports foreground-window state before screenshot capture. Urda and Ascension manual checklists require the helper session, foreground preflight, copied/audited `godot.log`, and preserve-on-restore path before screenshots count. Smoke evidence `.tools/runtime-evidence/live-helper-preserve-current-run-smoke-20260513-133431` passed; `.tools/runtime-evidence/window-preflight-smoke-20260513-135402` reported `VampireSurvivors` foreground and Slay the Spire 2 not running. Invalid live screenshot attempts are not counted as gameplay evidence. | Pass for tooling; live gameplay pending |
| Ancient reward gameplay | `docs/features/ancients-rework-v4/manual-verification-matrix.md` has rows for implemented Ancient systems. | Not complete: runtime results remain pending |
| Save/load behavior | Save/load rows exist for Prismatic Gem, Pael's Tooth, Jeweled Mask, Debt, Folly, Jewelry Box, Urda/Morvi state, Rootblight/Blight Sprout, and A11/A20 surfaces. | Not complete: broader live save/load evidence pending |
| Disable-mod gameplay | Release checklist requires disabling Spire Plus and confirming gameplay patches/logs are inactive. Current BaseLib-only plug-off evidence covers startup/log only. | Not complete: actual run pending |
| Rootblight visuals and behavior | English/zhs hover/starter-notice spot checks exist; generated portraits are packaged; combat-end notice pipeline is source-hardened. | Not complete: post-fix live visual/gameplay, combat-end timing, Blight Sprout, and co-op ownership pending |
| Urda gameplay | Urda has source hooks, localization, disable gate, and resource-load evidence. | Not complete: live selection, reward-screen timing, room-entry rewards, act-transition cleanup, save/load, UI, and co-op pending |
| Ascension A11-A20 | Source slices and single-player spot checks exist; A11 saved-map boss-reachability graph proof exists. | Not complete: natural click-by-click traversal, full live Ascension matrix, and co-op verification pending |
| Multiplayer/co-op | Mismatch diagnostics are source-patched and docs provide a two-client runbook. | Not complete: two-client ownership/desync/save-load matrix pending |
| Out-of-scope systems excluded | Docs/tests guard no A21-A30 and no custom character; Lotha and single-player Vakuu fight are source-complete/live-pending test slices, not release-ready claims; no official assets are packaged. | Pass |
| Worktree/release handoff | `git status --short --branch` remains dirty with many pending source/docs/test/resource changes; no commit or push performed. | Not complete |

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
- Clean intentional commit state and user-approved push.

Latest local screenshot attempts remain invalid: `.tools/runtime-evidence/live-urda-postfix-20260513-131752` stayed on the main menu and later captured the wrong surface, and `.tools/runtime-evidence/live-urda-continue-postfix-20260513-134337` was covered by another foreground app. Their clean logs may support loader health only; they do not satisfy live Urda, Rootblight, or gameplay rows.

## Conclusion

Not achieved. The current tree has the latest Urda/Morvi/Lotha/Vakuu source hardening build/publish/package-refreshed, current-package loader/resource smoke is clean, and the previous loader evidence plus restore-safe live-session helper remain useful. It is not private-beta release-ready and the active goal should not be marked complete until the live/manual blockers above are executed or explicitly release-noted as deferred by the project owner.
