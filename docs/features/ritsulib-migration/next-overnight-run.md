# Next Overnight Run — RitsuLib Migration

## Run Date

TBD (next available runtime session)

## Objective

**Runtime Proof + Governance Closure.** Runtime smoke remains the critical path blocker. Batch 4c cannot proceed until installed STS2-RitsuLib has clean runtime smoke evidence with Spire Plus actually loaded. Focus on fixing live-session isolation/settings, Off/CanaryOnly proof, and evidence capture, not patch migration.

## Current State

- HEAD: `6b149ba0` on `main` / `origin/main`.
- Build: 2026-05-31 clean/build replay passes with 0 errors, 89 Sts1Events nullable warnings (`CS8604` = 54, `CS8602` = 34, `CS8625` = 1).
- Tests: 2026-05-31 full and no-build solution tests pass with 464 passed, 0 failed, 21 skipped, 485 total. See `docs/reviews/current-validation.md`.
- Patch state: 25 migrated `IPatchMethod` classes, 142 raw `[HarmonyPatch]` declarations, 167 tracked patch units total.
- Architecture canary status: RewardPipeline, CardPlayContext, DeathProtectionService, and MultiplayerPolicy are diagnostics-only; no gameplay behavior or enforcement claim.
- Runtime smoke: **REACHES MENU WITH ERRORS / NOT CLEAN**. Official STS2-RitsuLib `v0.3.10` is installed at `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib`; E-drive BaseLib and Spire Plus package folders exist. Current best evidence `.tools\runtime-evidence\sts1-events-v15-loader-20260531-231135\godot.log.after-launch` reaches main menu with BaseLib, RitsuLib, and Spire Plus loaded, but audit reports 11 Godot ERROR hits. Supplemental retry evidence under `.tools\runtime-evidence\ritsulib-runtime-proof-20260531-2304` is invalid because direct executable launch failed Steam init and Steam `-applaunch` skipped `EZMicroBalance` as disabled.
- Sts1Events status: Off, CanaryOnly, and AdditiveBatch1 are source-guarded only. Off=0 and CanaryOnly=4 still require live `godot.log` proof.
- Release gate: no release-ready, live-ready, runtime-safe, Batch 4c, high-risk migration, or new gameplay claim.

## Pre-Run Checklist

1. Confirm current branch and dirty state with `git status --short --branch`.
2. Confirm full test suite passes: `dotnet test EZMicroBalance.sln --no-build` with 0 failed.
3. Confirm build result: `dotnet build EZMicroBalance.sln` with 0 errors; warning count must be recaptured from current source.
4. Confirm format is clean: `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`.
5. Confirm `git diff --check` passes.

## Run Steps

### Step 1: Confirm STS2-RitsuLib Install

1. Confirm official STS2-RitsuLib v0.3.10 remains installed at `<GameRoot>\mods\STS2-RitsuLib`.
2. Confirm `ritsulib-variants.json` includes `compatTarget` `0.106.1`.
3. Verify BaseLib v3.1.4 is installed at `<GameRoot>\mods\BaseLib`.
4. Verify Spire Plus package is installed at `<GameRoot>\mods\EZMicroBalance`.
5. Enable only BaseLib, STS2-RitsuLib, and Spire Plus for clean smoke, then verify the generated settings file actually marks `EZMicroBalance` enabled before launching.
6. If using `scripts\spire-plus-live-session.ps1`, pass the E-drive `-GameRoot`, E-drive `-SteamExe`, a specific `-SteamUserId`, preserve `STS2-RitsuLib` during mod isolation, and verify stale/duplicate mod folders did not reappear before launch.

### Step 2: Execute Loader Smoke

Run the loader smoke checklist per `runtime-smoke-checklist.md`:

1. Launch game via Steam.
2. Check `godot.log` for RitsuLib bootstrap, ModPatcher, BaseLib, and Spire Plus init.
3. Verify 25 ModPatcher patches applied.
4. Verify SavedSpireFields count (30).
5. Verify no `MissingMethodException`, `TypeLoadException`, or manifest dependency failure.
6. Audit `godot.log` with `scripts\audit-godot-log.ps1`.

### Step 3: Execute Sts1Events Runtime Gates

1. Off mode: unset/empty/invalid `SPIREPLUS_STS1_EVENT_MODE`; verify 0 Sts1Events registrations.
2. CanaryOnly mode: set `SPIREPLUS_STS1_EVENT_MODE=CanaryOnly`; verify exactly 4 canary registrations.
3. AdditiveBatch1 remains source/prototype-only until Off and CanaryOnly pass.
4. Do not use AdditiveAllDraft or ReplaceUnknownEventsPrototype for tester/release paths; both require `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`, and replacement also requires `REPLACEMENT_PROTOTYPE_ENABLED`.

### Step 4: If Runtime Smoke Passes

1. Update `runtime-smoke-checklist.md` with evidence paths and log excerpts.
2. Update `docs/dev-environment.md` with runtime evidence.
3. Update `docs/reviews/current-validation.md` with exact runtime proof.
4. Propose 5-10 low-risk Batch 4c candidates only as a decision list.
5. Do not migrate Batch 4c candidates unless explicitly accepted after the decision gate.

### Step 5: If Runtime Smoke Fails Or Remains Blocked

1. Document exact failure/blocker in `runtime-smoke-checklist.md` and `docs/reviews/current-validation.md`, including whether the current blocker is a non-clean controlled-loader audit or a supplemental live-session setup failure.
2. Create or update the relevant issue with evidence and next action.
3. Do not proceed to Batch 4c.
4. Keep runtime-ready/live-ready/release-ready claims blocked.

## Success Criteria

- [x] STS2-RitsuLib installed locally.
- [ ] Runtime smoke checklist completed at least through loader smoke.
- [ ] `godot.log` captured and audited.
- [ ] Off mode proves 0 Sts1Events registrations.
- [ ] CanaryOnly proves exactly 4 canary registrations.
- [ ] Batch 4c decision made as `advance` or `blocked`; no migration without explicit acceptance.

## Risk Mitigation

- **RitsuLib version mismatch**: Check manifest dependency against installed version.
- **MissingMethodException**: Check RitsuLib API changes against bootstrap code.
- **Game crash on startup**: Check `godot.log` for exact error and preserve the blocker report.
- **False green**: Treat missing `godot.log`, stale beta logs, source-only tests, or skipped release artifact tests as insufficient runtime proof.
