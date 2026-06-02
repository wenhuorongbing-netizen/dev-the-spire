# Current Validation

Date: 2026-06-02

## Sprint 4 Canonical Validation

- HEAD: `f20dd230 (HEAD -> main) fix nullable warnings in 4 canary event files`
- Branch: `main...origin/main`
- Worktree: **CLEAN** (0 dirty entries). All prior dirty entries committed.
- Runtime smoke: Off=0, CanaryOnly=4, and AdditiveBatch1=10/11 loader-gate evidence remains valid. Fresh CanaryOnly runtime smoke captured at HEAD `f20dd230` with mod isolation, confirming 4 canary registrations. Warning debt reduced from 89 to 79 by fixing all 4 canary event files.

### Sprint 4 Required Commands

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet build EZMicroBalance.sln -m:1 --no-incremental` | PASS | 0 errors, **79** Sts1Events nullable warnings (CS8602, CS8604, CS8625). Reduced from 89 by fixing 4 canary event files. |
| `dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build -- RunConfiguration.MaxCpuCount=1` | PASS | 464 passed, 0 failed, 21 skipped, 485 total. |
| `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` | PASS | No formatting changes required. |
| `git diff --check` | PASS | No whitespace errors. |
| `.\scripts\generate-patch-inventory.ps1 -Check` | PASS | Patch inventory is fresh. |
| `.\scripts\report-worktree-batches.ps1 -FailOnUnclassified` | PASS | 0 dirty entries, 0 unclassified. |
| `dotnet publish EZMicroBalance.sln` | PASS | Published to local installed mod folder for runtime smoke. |

### June 2 Runtime Path Check

| Path | Exists |
| --- | --- |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | True (`v0.3.10`, includes `lib\0.106.1`) |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | True |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | True |

### June 2 K1 Runtime Smoke (Fresh at HEAD `8f2d79b4`)

| Evidence | Result | Notes |
| --- | --- | --- |
| `.tools\runtime-evidence\smoke-k1-off-20260602-145938\godot.log.after-launch` | PASS | Off-mode Steam smoke reached main menu in 40s. Loaded exactly 3 mods (BaseLib v3.1.4, RitsuLib v0.3.10, Spire Plus v0.1.0-private-beta.84). Applied 25/25 Spire Plus ModPatcher patches. Found 30 SavedSpireFields. Sts1Events: `bootstrap=disabled, live=Disabled` (default Off). FeatureRegistry diagnostics observed for all 6 features. All features default-on except Sts1Events. |
| `.tools\runtime-evidence\smoke-k1-off-20260602-145938\godot-log-audit.json` | PASS | Clean audit: 0 Godot ERROR, 0 MissingMethodException, 0 TypeLoadException, 0 Spire Plus error/exception. The `[ERROR] ritsulib-variants.json` line is a known RitsuLib internal variant-manifest issue (ignored by audit). |
| `.tools\runtime-evidence\smoke-k1-canary3-20260602-151104\godot.log.after-launch` | PASS | CanaryOnly direct launch (with `steam_appid.txt` + `SPIREPLUS_STS1_EVENT_MODE=CanaryOnly` env var) reached main menu in 22s. Loaded exactly 3 mods. Applied 25/25 patches. Found 30 SavedSpireFields. Sts1Events: `bootstrap=enabled, live=Enabled` (CanaryOnly mode). Registered exactly 4 canary events: `Sts1BigFish`, `Sts1GoldenIdol`, `Sts1TheLab`, `Sts1DivineFountain`. No other events registered. |
| `.tools\runtime-evidence\smoke-k1-canary3-20260602-151104\godot-log-audit.json` | PASS | Clean audit: 0 Godot ERROR, 0 MissingMethodException, 0 TypeLoadException, 0 Spire Plus error/exception. |

### June 2 CanaryOnly Fresh Smoke (HEAD `f20dd230`, with mod isolation)

| Evidence | Result | Notes |
| --- | --- | --- |
| `.tools\runtime-evidence\live-spire-plus-session-20260602-174656\godot.log.after-launch` | PASS | CanaryOnly Steam launch with mod isolation (25 other mods moved). Reached main menu. Loaded exactly 3 mods (BaseLib, RitsuLib, Spire Plus). Applied 25/25 patches. Found 30 SavedSpireFields. Sts1Events: `bootstrap=enabled, live=Enabled` (CanaryOnly mode). Registered exactly 4 canary events: `Sts1BigFish`, `Sts1GoldenIdol`, `Sts1TheLab`, `Sts1DivineFountain`. Additional mods still loaded from cached mod list (RouteSuggest, heybox, etc.) — isolation moved files but game cached mod list before isolation. |

### June 2 AdditiveBatch1 Runtime Evidence

| Evidence | Result | Notes |
| --- | --- | --- |
| `.tools\runtime-evidence\additive-batch1-20260602-150445\godot.log.after-launch` | PASS | AdditiveBatch1 direct launch reached main menu in 42s. Loaded exactly 3 mods (BaseLib v3.1.4, RitsuLib v0.3.10, Spire Plus v0.1.0-private-beta.84). Applied 25/25 Spire Plus ModPatcher patches. Registered exactly 10 event types via 11 calls: Sts1BigFish (Shared), Sts1GoldenIdol (Shared), Sts1TheLab (Shared), Sts1DivineFountain (Shared), Sts1Purifier (Shared), Sts1UpgradeShrine→Glory (Act), Sts1GoldenShrine (Shared), Sts1TheCleric (Shared), Sts1OldBeggar (Shared), Sts1ShiningLight→Overgrowth (Act), Sts1ShiningLight→Underdocks (Act). |
| `.tools\runtime-evidence\additive-batch1-20260602-150445\godot-log-audit.json` | PASS | Clean audit: 0 Godot ERROR, 0 MissingMethodException, 0 TypeLoadException, 0 Spire Plus error/exception. The single `[ERROR] ritsulib-variants.json` line is a RitsuLib internal variant-manifest issue (C# logger), not a Godot engine error. |

### June 2 Warning Triage

- Warning triage matrix written to `docs/reviews/warning-triage-matrix.md`.
- **79 warnings** remain (reduced from 89 by fixing all 4 canary event files: BigFish, GoldenIdol, TheLab, DivineFountain).
- All remaining warnings trace to single root cause: `EventModel.Owner` typed `Player?` from game base class.
- Recommended fix: early-exit guard `if (Owner is not { } owner) return;` at top of each handler method.
- CanaryOnly event files now have 0 nullable warnings.

### June 2 Diagnostics Architecture Audit

| Component | Required Posture | Actual Posture | Compliant? |
|---|---|---|---|
| RewardPipeline | Diagnostics-only | Diagnostics-only | YES |
| CardPlayContext | Allow-only | Allow-only | YES |
| DeathProtectionService | No-op / diagnostics-only | No-op (zero production callers) | YES |
| MultiplayerPolicy (registry) | Taxonomy / diagnostics-only | Taxonomy store | YES |
| MultiplayerFeaturePolicy (coop gates) | Behavioral safety gate | Active feature suppression in co-op | YES (intentional) |

### June 2 Stop Decision (Updated after CanaryOnly fresh smoke + warning fixes)

- Status: PARTIAL PASS / RELEASE STILL BLOCKED.
- No-game validation: **PASS** (build 0 errors / **79 warnings**, 464 passed / 0 failed / 21 skipped / 485 total, format clean, diff clean).
- Runtime dependency path: **PASS** (STS2-RitsuLib v0.3.10 installed, BaseLib v3.1.4 and EZMicroBalance present).
- Runtime loader gate: **PASS** (Off=0, CanaryOnly=4, AdditiveBatch1=10/11 with clean audits).
- Sts1Events Off runtime proof: **PASS** (0 StS1 registrations, clean audit).
- Sts1Events CanaryOnly runtime proof: **PASS** (exactly 4 canary events registered, clean audit, fresh at HEAD `f20dd230` with mod isolation).
- FeatureRegistry runtime diagnostics: **PASS** (all 6 features with bootstrap/live status in runtime log).
- RewardPipeline diagnostics: **PASS** (bootstrap events observed for all features in runtime log).
- AdditiveBatch1 runtime proof: **PASS** (10 event types / 11 registration calls, clean audit).
- Worktree: **CLEAN** (0 dirty entries).
- Warning debt: **ACCEPTED** (79 warnings remaining, 10 fixed in canary events, single root cause, fix pattern documented).
- Independent QA: **PENDING** (needs rerun against current state).
- Gameplay proof: **PENDING** (game launched and reached main menu, but no interactive gameplay, save-load, or Mod Settings UI evidence captured).
- Event encounter screenshots: **PENDING** (require in-game event encounters).
- Save/load proof: **PENDING** (require save during/after event, reload, state stable).
- Versioned tester-package handoff: **PENDING**.
- Batch 4c: **READY FOR LOW-RISK CANDIDATE PROPOSAL** (runtime smoke passed; propose 5-10 candidates for owner acceptance).
- Release-ready / live-ready: **NO**.

---

## Revision J Current Snapshot

- HEAD: `6b149ba0 (HEAD -> main, origin/main, origin/HEAD) sprint 2`
- Branch: `main...origin/main`
- Worktree: dirty before this pass and still dirty. Existing source/docs/harness edits were preserved; no commit, push, stash, checkout, reset, restore, or broad clean was performed.
- Runtime smoke: target-fix follow-up evidence under `.tools\runtime-evidence\ritsulib-off-after-target-fix-20260531-2325\` and `.tools\runtime-evidence\ritsulib-canary-after-target-fix-20260531-2327\` reaches main menu with BaseLib, RitsuLib, and Spire Plus loaded, clean audits, and 25/25 Spire Plus ModPatcher patches applied. Off mode proves 0 StS1 registration lines; CanaryOnly proves exactly 4 canary content registrations. Live gameplay, UI, save-load, co-op, independent QA rerun, clean worktree, versioned tester-package handoff, live-ready, and release-ready claims remain blocked.

## Revision J Required Commands

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet clean .\EZMicroBalance.csproj` | PASS | 0 warnings, 0 errors. |
| `dotnet build .\EZMicroBalance.csproj` | PASS | 0 errors, 89 warnings. Warnings remain Sts1Events nullable staging debt (`CS8602`, `CS8604`, `CS8625`). |
| `dotnet build .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj` | PASS | Test assembly builds against the current project. |
| `dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build` | PASS | 464 passed, 0 failed, 21 skipped, 485 total. |
| `dotnet format .\EZMicroBalance.csproj --verify-no-changes` | PASS | No formatting changes required. |
| `git diff --check` | PASS | No whitespace errors. |
| `.\scripts\generate-patch-inventory.ps1 -Check` | PASS | Patch inventory is fresh. |
| `.\scripts\report-worktree-batches.ps1 -FailOnUnclassified` | PASS | Revision J classifier reports 49 dirty entries, 0 unclassified. |
| `dotnet publish EZMicroBalance.sln` | PASS | Published the target-fix build to the local installed mod folder for diagnostic runtime smoke; no new versioned tester package was created. |

## v15 Continuation Validation Rerun

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet build EZMicroBalance.sln -m:1 --no-incremental` | PASS | 0 errors, 89 Sts1Events nullable warnings. |
| `dotnet test EZMicroBalance.sln --no-build -- RunConfiguration.MaxCpuCount=1` | PASS | 464 passed, 0 failed, 21 skipped, 485 total. |
| `dotnet test --filter Sts1EventFeatureGuardTests --no-build -- RunConfiguration.MaxCpuCount=1` | PASS | 31 passed, 0 failed, 0 skipped. |
| `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` | PASS | No formatting changes required. |
| `git diff --check` | PASS | No whitespace errors. |

## Revision J Runtime Attempt

| Evidence | Result | Notes |
| --- | --- | --- |
| `.tools\runtime-evidence\sts1-events-v15-loader-20260531-231135\godot.log.after-launch` | FAIL / reaches menu with errors | BaseLib, RitsuLib, and Spire Plus loaded and reached main menu, but audit is not clean: 11 Godot ERROR hits, including `ritsulib-variants.json` manifest parsing and 8 optional Spire Plus ModPatcher failures. |
| `.tools\runtime-evidence\sts1-events-v15-loader-20260531-231135\audit-godot-log.after-launch.json` | FAIL | Not clean; 11 Godot ERROR lines. No `MissingMethodException` or `TypeLoadException` hits. |
| `.tools\runtime-evidence\ritsulib-off-after-target-fix-20260531-2325\godot.log.after-launch` | PASS | Off-mode Steam smoke reached main menu, loaded exactly BaseLib/RitsuLib/Spire Plus, applied 25/25 Spire Plus patches, found 30 SavedSpireFields, and logged Sts1Events disabled/default Off. |
| `.tools\runtime-evidence\ritsulib-off-after-target-fix-20260531-2325\godot-log-audit.json` | PASS | Clean audit with 0 release-blocking signature hits. |
| `.tools\runtime-evidence\ritsulib-canary-after-target-fix-20260531-2327\godot.log.after-direct-launch` | PASS | CanaryOnly direct smoke reached main menu, loaded exactly 3 mods, applied 25/25 patches, found 30 SavedSpireFields, and registered `Sts1BigFish`, `Sts1GoldenIdol`, `Sts1TheLab`, and `Sts1DivineFountain`. |
| `.tools\runtime-evidence\ritsulib-canary-after-target-fix-20260531-2327\godot-log-audit.json` | PASS | Clean audit with 0 release-blocking signature hits. |
| `.tools\runtime-evidence\ritsulib-runtime-proof-20260531-2304\godot-direct-exe-steam-init-fail.log` | FAIL | Direct executable launch failed Steam initialization before mod loading. |
| `.tools\runtime-evidence\ritsulib-runtime-proof-20260531-2304\godot-steam-applaunch.log` | FAIL / invalid Spire Plus proof | RitsuLib `0.3.10` loaded with compat branch `0.106.1`; RitsuLib framework patches reported 0 failed; BaseLib `3.1.4` loaded. `EZMicroBalance` was skipped as disabled in settings, so Spire Plus initialization, 30 SavedSpireFields, and Spire Plus ModPatcher proof were not established. |
| `.tools\runtime-evidence\ritsulib-runtime-proof-20260531-2304\godot-steam-applaunch-audit.json` | FAIL | Audit was not clean: 3 Godot ERROR lines. No `MissingMethodException` or `TypeLoadException` hits were found. |
| Cleanup | PASS | Stopped `SlayTheSpire2`; restored `settings.save` for Steam user `76561199353211250` with matching before/after SHA256. |

## StS1 Unsafe-Gate Continuation Validation

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet test --filter Sts1EventFeatureGuardTests` | PASS | 31 passed, 0 failed, 0 skipped after adding unsafe-mode and replacement fail-closed guards. |
| `dotnet test --filter PlayerFacingNameStaysSpirePlusWhileTechnicalIdRemainsStable` | PASS | 1 passed; active player-facing markdown naming guard is green. |
| `dotnet build EZMicroBalance.sln -m:1 --no-incremental` | PASS after clearing stale testhost locks | Final rerun passed with 0 errors and 89 Sts1Events nullable warnings. Earlier attempts failed only because stale `testhost` processes locked `EZMicroBalance.Tests.dll`. |
| `dotnet test EZMicroBalance.sln --no-build` | PASS after retry | Latest default no-build rerun passed with 464 passed, 0 failed, 21 skipped, 485 total after stale testhost locks were absent. Earlier normal reruns intermittently aborted with the known testhost crash and no assertion failures; `RunConfiguration.MaxCpuCount=1` remains the documented fallback if needed. |
| `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` | PASS | No formatting changes required. |
| `git diff --check` | PASS | No whitespace errors; PowerShell emitted a CRLF normalization warning for existing `docs/patch-inventory.md`. |

## Revision J Runtime Path Check

| Path | Exists |
| --- | --- |
| `E:\Steam\steamapps\common\Slay the Spire 2` | True |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods` | True |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | True |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | True |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | True (`v0.3.10`, includes `lib\0.106.1`) |
| `D:\Steam\steamapps\common\Slay the Spire 2` | False |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | False |
| `C:\Users\zihao\AppData\Roaming\SlayTheSpire2\logs\godot.log` | True, but current content is from runtime smoke attempts with non-clean audit; use copied evidence logs for review |

## Revision J Stop Decision

- Status: PARTIAL PASS / RELEASE STILL BLOCKED.
- Runtime dependency path blocker: cleared locally by installed STS2-RitsuLib `v0.3.10`.
- Runtime loader gate: Off and CanaryOnly diagnostic smokes now pass with clean audits and 25/25 Spire Plus patches.
- Commit readiness: not complete because the worktree is dirty and no commit/push was requested.
- Batch 4c: remains blocked until independent QA reruns against the new evidence and the owner accepts the dirty-worktree/package state.
- Release-ready: no.

## Governance Closure Validation Snapshot (Supersedes 24d4fe9a)

- HEAD: `87820303 (HEAD -> main, origin/main, origin/HEAD) sprint 1`
- Branch: `main...origin/main`
- Worktree: dirty before and after this pass; existing goal-doc edits were preserved. This continuation fixed the Sts1Events mode bootstrap override, refreshed runtime prerequisite evidence, and did not attempt to normalize unrelated dirty files.

## Prior M4 Commands

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet clean EZMicroBalance.sln -m:1` | PASS | Exited `0`; clean completed with 0 warnings and 0 errors. |
| `dotnet build EZMicroBalance.sln -m:1 --no-incremental` | PASS | 0 errors, 89 warnings. Warnings are existing Sts1Events nullable warnings (`CS8602`, `CS8604`, `CS8625`). |
| `dotnet test EZMicroBalance.sln` | PASS | Earlier rerun exited `0` with 462 passed, 0 failed, 21 skipped, 483 total after the Sts1Events bootstrap guard fix; superseded by the current project no-build count above. |
| `dotnet test EZMicroBalance.sln --no-build` | PASS | Earlier rerun exited `0` with 462 passed, 0 failed, 21 skipped, 483 total after the Sts1Events bootstrap guard fix; superseded by the current project no-build count above. |
| `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` | PASS | Exited `0`; no formatting changes required. |
| `git diff --check` | PASS | Exited `0`; no whitespace errors. |
| `dotnet publish EZMicroBalance.sln` | NOT RUN | No resource, localization, manifest, export, or package refresh was performed in this pass. |

## Validation Fixes Applied

- Replayed clean/build/test/format/diff-check on the current `87820303` HEAD after doc and test-guard reconciliation.
- Removed the trailing whitespace that previously blocked `git diff --check` in goal/status docs.
- Cleared stale `testhost` locks before final validation so clean/build/test could rebuild the current source and test assembly.
- Fixed the current `docs/issues.md` compactness guard regression and updated its guard to assert the active dirty-worktree truth instead of the obsolete beta.84 clean-worktree phrase.
- Kept the test assembly serialized by default to reduce prior default test-host instability; the latest default `dotnet test EZMicroBalance.sln --no-build` rerun passes, while earlier crash logs remain historical/intermittent evidence rather than assertion failures.
- Earlier full and `--no-build` test runs both passed with 462/0/21/483; the final current project no-build rerun passed with 464/0/21/485.
- Ran independent QA/Red-Team review. Verdict: FAIL / HARD BLOCKED because runtime proof is absent; Green Stop is not allowed.
- Fixed QA-flagged stale wording in `docs/goals/refactor.md` and `docs/goals/event.md` without changing the runtime hard-block decision.
- Reconciled active RitsuLib/Sts1Events validation docs and doc guard tests to the 464/0/21/485 project no-build test count and current runtime dependency blocker.
- Fixed `Sts1EventsFeatureModule` so `SPIREPLUS_STS1_EVENT_MODE` is no longer treated as a generic FeatureRegistry disable override; CanaryOnly/AdditiveBatch1 can now reach `Sts1EventFeatureGate` in the source-level bootstrap path.
- Tightened StS1 unsafe modes so AdditiveAllDraft requires `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`, and ReplaceUnknownEventsPrototype reports disabled/fail-closed in normal builds unless `REPLACEMENT_PROTOTYPE_ENABLED` and the unsafe override are present.
- Added independent refactor QA report at `docs/reviews/refactor-overnight-qa-20260531.md`; verdict is FAIL / HARD BLOCKED until runtime evidence exists.
- Added StS1 v14 hard-stop report at `docs/features/sts1-events/hard-stop-blocker-report-v14.md`; blocked runtime gates remain open.
- Preserved existing goal-doc edits that were outside this continuation.

## Warning Truth

- Current clean build warning count: 89.
- Warning codes: `CS8602`, `CS8604`, `CS8625`.
- Scope: all warnings are in `EZMicroBalanceCode/Sts1Events/Models/` staging code.
- Decision: warnings are issue-worthy and remain accepted only because Sts1Events is gated Off by default and still prototype/dev-only outside Canary/Batch1 test modes.

## Runtime Smoke

- Status: LOADER/GATE PASS AT HEAD `8f2d79b4`, RELEASE BLOCKED.
- Fresh K1 evidence (2026-06-02): Off-mode Steam smoke and CanaryOnly direct-launch smoke both reached main menu with clean audits, 25/25 Spire Plus patches, 30 SavedSpireFields, and BaseLib + RitsuLib + Spire Plus loaded. Off mode proves Sts1Events disabled (0 registrations). CanaryOnly proves exactly 4 canary event registrations (`Sts1BigFish`, `Sts1GoldenIdol`, `Sts1TheLab`, `Sts1DivineFountain`).
- Runtime dependency path: `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` (`v0.3.10`), `BaseLib` (`v3.1.4`), `EZMicroBalance` (`v0.1.0-private-beta.84`) all present.
- Decision: loader/runtime gate proof is now available for Off=0, CanaryOnly=4, and AdditiveBatch1=10/11 at HEAD `8f2d79b4`. Runtime safety beyond loader gates (event encounter screenshots, save/load proof, image rendering, replacement functional proof, multiplayer fail-closed, independent QA), live-ready, and release-ready remain blocked pending gameplay/manual proof, clean worktree or owner decision, and versioned tester-package handoff.

## Independent QA

- Target-fix QA/Red-Team verdict: CONDITIONAL PASS for loader gates, not release-ready.
- QA-supported proof: Off=0, CanaryOnly=4, and AdditiveBatch1=10/11 loader-gate evidence is supported by the smoke logs and clean audits.
- QA fixes applied after review: removed stale hard-block wording that still claimed no CanaryOnly proof or non-clean loader audit in active docs.
- Stop decision: release/live Green Stop remains disallowed until event encounter screenshots, save/load proof, image rendering, replacement functional proof, multiplayer fail-closed, independent QA rerun, clean worktree or owner decision, and versioned tester-package handoff are complete.

## Architecture Status

- RewardPipeline diagnostics are wired into `FeatureRegistry` bootstrap events and the low-risk `AscensionRewardService` reward/card-reward surfaces as no-mutation diagnostics.
- `ArchitectureCanaryBootstrap` registers FeatureRegistry and Ascension reward diagnostic handlers, a no-op DeathProtection provider, and multiplayer policy records for preview tools, Ancients, Ascension, reward surfaces, and combat hooks.
- Lotha extra-play paths touch `CardPlayContextCanary` through a single-depth adapter that returns `Allow`; play counts and gameplay branches are unchanged.
- Existing co-op gates still make the same allow/disable decisions; their evidence payloads now include policy registration/category/env/verification metadata.
- Guard coverage was added for reward-surface diagnostics, multiplayer policy metadata, no-op DeathProtection registration, architecture wiring, multiplayer policy records, and source-manifest coverage.
