# Next Overnight Run - RitsuLib Migration

## Run Date

TBD (next available runtime/manual QA session)

## Objective

**Manual QA + Batch 4c Owner Review After Clean RitsuLib-Only Loader Proof.**

The old hard blocker was missing or non-clean RitsuLib runtime smoke. That blocker is now closed for the current `v0.107.1` / beta.91 RitsuLib-only loader/registration target: historical Slay the Spire 2 `v0.106.1` Off, CanaryOnly, and AdditiveBatch1 diagnostic evidence is clean; retained beta.85/beta.87 `v0.107.0` evidence remains previous-package context; the first `v0.107.1` beta.87 recapture failed clean audit because BaseLib `v3.2.1` logged 2 patch failures; beta.88 later passed only as previous BaseLib-backed context. Current beta.91 proof under `.tools/runtime-evidence/v01071-beta91-ritsulib0428-off-direct-20260620/` and `.tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/` reached main menu with exactly STS2-RitsuLib `v0.4.28` and Spire Plus loaded, 25/25 patches, clean audits, Off packet 43 / 0, AdditiveBatch1 enabled-mode verifier 31 / 0, and AdditiveBatch1 packet 61 / 0. The next run should move to gameplay, render, save-load, replacement, multiplayer, QA, and handoff proof while keeping beta.91 RitsuLib-only package parity intact. It is not approval to migrate more patches.

Coordination boundary: do not run overlapping validation, package/release, runtime/game smoke, staging, commit, or push steps. Assign one controlled lane, verify `git status --short --branch` first, and record the resulting HEAD/worktree status before using evidence for handoff.

## Current State

- Current repository HEAD must be checked at run start. Use `git log -1 --oneline --decorate` and `git status --short --branch` as the source of truth; older run-start hashes from prior follow-ups are historical notes and must not be reused for handoff.
- Any dirty files after the latest pushed HEAD are post-baseline follow-up scope. Classify them before any validation claim, package handoff, commit, or push.
- Latest beta.91 no-game/package validation is summarized in `PROJECT_STATE.md` and `docs/reviews/current-validation.md`: build/publish/package refresh, installed-package parity, runtime preflight, source-workspace check, and RitsuLib-only Off/AdditiveBatch1 packet verification passed for the current dependency target. Revalidate current HEAD in a clean single validation lane again before package handoff if the worktree changes.
- New no-launch evidence folders should retain the generated `environment.json` git handoff fields (`GitPushedHead`, `GitHeadMatchesUpstream`, branch status, and latest commit) as supporting provenance. Still run `git status --short --branch` and `git log -1 --oneline --decorate` directly at handoff time; collector metadata is not a substitute for final recapture.
- Patch state remains 25 migrated `IPatchMethod` classes, 146 raw `[HarmonyPatch]` declarations, 171 tracked patch units total.
- Historical `v0.106.1` loader-gate runtime proof exists:
  - Off: `.tools\runtime-evidence\smoke-k1-off-20260602-145938`, 0 Sts1Events registrations, clean audit.
  - CanaryOnly: `.tools\runtime-evidence\smoke-k1-canary3-20260602-151104`, exactly 4 canary registrations in the old source shape, clean audit. Current source expects 4 event types through 6 registration calls.
  - AdditiveBatch1: `.tools\runtime-evidence\additive-batch1-20260602-150445`, 10 event types through the old 11 registration calls, clean audit. Current source expects 10 event types through 14 registration calls.
  - Recorded smokes loaded BaseLib, RitsuLib, and Spire Plus, applied 25/25 migrated patches, and observed 30 SavedSpireFields.
- Retained `v0.107.0` beta.85 package runtime proof is clean for loader/patch application under `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/`; installed package parity passed for that package line.
- Retained CanaryOnly enabled-mode proof is clean under `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/`: 4 event types / 6 registration lines, retained `enabled-mode-log-check.json` and `runtime-evidence-packet-check.json`, both 0 mismatches.
- Current AdditiveBatch1 enabled-mode proof is clean under `.tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/`: 10 event types / 14 registration lines, retained enabled-mode report 31 / 0 and runtime packet verifier 61 / 0. The earlier `.tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/` failure is BaseLib `v3.2.1` root-cause history, and beta.88 is previous BaseLib-backed context.
- Dependency decision is recorded: beta.91 intentionally aligns the compile package and manifest minimum on STS2-RitsuLib `v0.4.28`, uses `lib\0.107.1`, removes BaseLib from current Spire Plus requirements, and uses package version `v0.1.0-private-beta.91`.
- Batch 4c is proposal-only. The current candidate list is `docs/features/ritsulib-migration/batch-4c-candidates.md`; the 2026-06-18 static recapture confirmed 10 low-risk candidates, no forbidden high-risk categories, and no migration performed. Migration requires explicit owner approval and fresh validation.
- A historical no-launch Mod Settings UI scaffold exists at `.tools/runtime-evidence/mod-settings-current-display-20260618-223145/` with package `v0.1.0-private-beta.87`. Treat it as a template only; refresh or replace it for beta.91 before using it as current UI evidence. It is not screenshot, log/audit, gameplay, or handoff proof.
- A historical no-launch manual-test handoff scaffold exists at `.tools/runtime-evidence/manual-test-handoff-20260619-120202/` on pushed HEAD `2400ec4b`, with 21 required live rows, 21 expected pending failures, 0 warnings, package ZIP `D547847874919EE923E2281A495D5389BAB22BBDB9F1090DC57B77033668A36D`, and `GitHeadMatchesUpstream=true`. Use it as a template only; current beta.91 handoff must use the package hashes in `docs/private-beta-verification-handoff.md` / `docs/release-evidence-status.md` and recapture HEAD/worktree status. No game was launched for that scaffold.
- Gameplay, Mod Settings UI page refresh, event screenshots, save-load, image/render, replacement functional proof, multiplayer fail-closed proof, independent QA, and tester-package handoff remain pending. Current-version clean loader proof is available for beta.91/RitsuLib-only; future handoff must recapture HEAD and worktree status after any later edits.
- Release-ready and live-ready remain no.

## Pre-Run Checklist

1. Confirm branch, HEAD, and dirty state:

```powershell
git status --short --branch
git log -1 --oneline --decorate
```

2. Confirm or refresh no-game validation for the current HEAD:

```powershell
dotnet clean EZMicroBalance.sln
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
.\scripts\generate-patch-inventory.ps1 -Check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

3. Confirm current runtime compatibility before any loader smoke:

```powershell
Get-Content "E:\Steam\steamapps\common\Slay the Spire 2\release_info.json"
Get-ChildItem "E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib\lib"
```

4. If validation or runtime compatibility fails, record the exact command, error, and affected dirty files before editing.
5. If owner approves another dependency-floor change, move the repo package reference and manifest minimum in that same versioned package pass and follow the private-beta package-version and validation rules.

## Run Steps

### Step 1: Reconcile Loader Evidence

1. Verify the evidence folders listed above still exist or record that only documentation references remain.
2. Verify the current game version still has a matching RitsuLib variant and that the installed package under test matches the intended artifact; if either check fails, record the blocker instead of launching the game. The beta.91 package artifact currently matches and has clean `v0.107.1` RitsuLib-only loader proof under `.tools/runtime-evidence/v01071-beta91-ritsulib0428-off-direct-20260620/` and `.tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/`.
3. Re-audit the recorded logs if needed with `scripts\audit-godot-log.ps1`.
4. Rerun loader smoke only if runtime compatibility is present and HEAD/package drift makes the old logs stale for the claim being made.
5. Before any StS1 canary gameplay claim, recapture current-version CanaryOnly loader proof; `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/` is retained previous-package/game-version context only.
6. Before any AdditiveBatch1 gameplay claim, confirm the beta.91 package and STS2-RitsuLib `v0.4.28` / `lib\0.107.1` runtime are still installed and current; `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/` is retained `v0.107.0` context only, beta.88 is previous BaseLib-backed context, and `.tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/` is failed blocker evidence only.
7. If current AdditiveBatch1 loader/registration proof goes stale or cannot be recaptured, record the exact blocker instead of continuing into AdditiveBatch1 gameplay proof.
8. Keep AdditiveAllDraft and ReplaceUnknownEventsPrototype out of tester/release paths.

### Step 2: Manual Gameplay Evidence

For CanaryOnly events (`Sts1BigFish`, `Sts1GoldenIdol`, `Sts1TheLab`, `Sts1DivineFountain`), capture or explicitly block:

- spawn/route status;
- EN and ZHS rendering;
- options;
- reward/effect;
- no-softlock and exit;
- save/load;
- screenshot evidence.

### Step 3: UI And Multiplayer Evidence

1. Capture current Spire Plus Mod Settings UI evidence. Continue from `.tools/runtime-evidence/mod-settings-current-display-20260618-223145/`, rerunning that same evidence dir with `-Capture List -RequireSpireForeground` and `-Capture Page -RequireSpireForeground` after manually opening the Mods list and Spire Plus page.
2. Capture or block co-op/fail-closed proof.
3. Confirm loader-gate proof is not being treated as gameplay proof.

### Step 4: Batch 4c Candidate Review

1. Review `docs/features/ritsulib-migration/batch-4c-candidates.md`.
2. Confirm the list is 5-10 low-risk patch classes only.
3. Reject any run lifecycle, save/load, map generation, multiplayer/lobby, death, A20 boss-flow, or reward-state candidate.
4. Record owner decision: accept all, accept subset, request changes, or block. The current static recapture is not that decision.
5. Do not migrate any candidate without explicit owner acceptance and fresh validation.

### Step 5: Blocker Reporting

If a gate cannot be completed, record:

- exact gate;
- blocker reason;
- evidence path;
- attempted actions;
- owner action needed;
- why continuing would create an unsupported claim.

## Success Criteria

- [x] Current STS2-RitsuLib installed locally (`v0.4.28`, `lib\0.107.1`).
- [x] Historical `v0.106.1` Off loader gate proves 0 Sts1Events registrations.
- [x] Historical `v0.106.1` CanaryOnly loader gate proves exactly 4 canary registrations in the old source shape.
- [x] Historical `v0.106.1` AdditiveBatch1 loader gate proves 10 event types through the old 11 registration calls.
- [x] Current `v0.107.1` game install has STS2-RitsuLib `v0.4.28`; the selected compatibility branch remains `lib\0.107.1`.
- [x] Final dependency migration decision recorded: beta.91 compile/manifest floor is `0.4.28`, and BaseLib is no longer a Spire Plus project, manifest, package, or current runtime dependency.
- [x] Installed beta.85 package parity verified after the 2026-06-11 package refresh.
- [x] Installed beta.86 package parity verified after the 2026-06-18 package/source alignment pass.
- [x] Installed beta.91 package parity verified after the 2026-06-20 RitsuLib-only package refresh.
- [x] Clean `v0.107.0` beta.85 Off loader smoke captured after the prior `v0.4.16` install; retain as previous-package/game-version context.
- [x] Current HEAD validation refreshed after the beta.91 RitsuLib-only dependency pass and runtime packet-checker hardening; recheck again before handoff if any later edits appear.
- [x] Retained `v0.107.0` beta.85 CanaryOnly smoke proves 4 event types / 6 registration calls with retained verifier reports.
- [x] Retained `v0.107.0` beta.87 AdditiveBatch1 smoke proves 10 event types / 14 registration calls with retained verifier reports; beta.85 13/14 attempt remains root-cause history only.
- [x] Current `v0.107.1` clean Off proof recaptured for beta.91/RitsuLib-only at `.tools/runtime-evidence/v01071-beta91-ritsulib0428-off-direct-20260620/`.
- [x] Current `v0.107.1` clean AdditiveBatch1 proof recaptured for beta.91/RitsuLib-only at `.tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/`.
- [x] Batch 4c candidate list static review recaptured: 10 low-risk candidates, no forbidden high-risk categories, and no migration performed.
- [ ] Mod Settings UI evidence captured.
- [ ] CanaryOnly gameplay matrix completed or blocked with evidence.
- [ ] Save/load evidence captured or blocked with evidence.
- [ ] Co-op/fail-closed evidence captured or blocked with evidence.
- [ ] Batch 4c owner decision recorded.

## Hard Stops

- No release-ready, live-ready, or runtime-safe claim from loader logs alone.
- No Batch 4c migration without owner acceptance.
- No high-risk migration in this run.
- No package handoff while validation recapture, gameplay proof, handoff status recapture, or owner decision is missing.
