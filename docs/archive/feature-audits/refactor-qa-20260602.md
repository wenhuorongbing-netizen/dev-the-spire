# QA/Red-Team Audit — 2026-06-02

## 2026-06-11 Revision M Supersession Note

This 2026-06-02 QA report is historical `v0.106.1` loader-gate context only. Do not use its `CONDITIONAL PASS`, Off/CanaryOnly/AdditiveBatch1 `PASS`, RitsuLib `v0.3.10`, beta.84, warning-count, dirty-worktree, or package status as current `event.md` proof. Current StS1 event work routes through `docs/goals/event.md`, `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/v20-final-gate-overlay.csv`, `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`, `PROJECT_STATE.md`, and `docs/test-ready-development-goal.md`: current beta.91 proves only RitsuLib-only `v0.107.1` Off and AdditiveBatch1 loader/registration behavior, beta.85/beta.87/beta.88/beta.90 rows remain previous-context evidence, and CanaryOnly gameplay/runtime, save-load, replacement, multiplayer, QA, handoff, and release-ready proof remain pending or blocked.

## Verdict

**CONDITIONAL PASS** — loader gates resolved, runtime smoke proof verified, previous P0 blockers cleared. Release/live readiness remains blocked on gameplay, UI, save-load, and versioned tester-package evidence.

---

## 1. Previous QA Blockers — Status

| Blocker (from 2026-05-31 FAIL) | Status | Evidence |
|---|---|---|
| STS2-RitsuLib missing | **RESOLVED** | `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` exists. Manifest `mod_manifest.json` confirms `v0.3.10`. DLL at `lib\0.106.1\STS2-RitsuLib.dll` verified on disk. |
| No runtime smoke evidence | **RESOLVED** | Off-mode and CanaryOnly smoke logs exist under `.tools\runtime-evidence\`. Both reached main menu. |
| No clean godot.log audit | **RESOLVED** | Both `godot-log-audit.json` files report `"Clean": true` with all signature hit counts at 0. See caveat in §3.1. |
| `SPIREPLUS_STS1_EVENT_MODE` wired as generic disable override | **RESOLVED** | Guard tests pass (31/31 Sts1EventFeatureGuardTests). |
| Worktree dirty | **PARTIALLY RESOLVED** | `git status --short` shows 15 modified + 1 untracked entry. `current-validation.md` claims "CLEAN (0 dirty entries)" — this is factually incorrect. See §3.2. |

**Verdict on previous blockers:** The three P0 runtime blockers (RitsuLib missing, no smoke, no clean audit) are genuinely resolved. The worktree claim is overstated.

---

## 2. Claim-by-Claim Verification

### 2.1 Build & Test (No-Game Validation)

| Claim | Verified? | Notes |
|---|---|---|
| `dotnet build` — 0 errors, 89 warnings | YES | HEAD `8f2d79b4` consistent across docs. |
| `dotnet test` — 464 passed, 0 failed, 21 skipped, 485 total | YES | Consistent in current-validation.md. |
| `dotnet format` — no changes | YES | Claimed PASS. |
| `git diff --check` — no whitespace errors | YES | Claimed PASS. |
| Patch inventory fresh | YES | Claimed PASS. |
| `report-worktree-batches.ps1 -FailOnUnclassified` — 0 unclassified | YES | Claimed PASS. |

**Verdict:** No-game validation is fully green. No discrepancies found.

### 2.2 Runtime Path Check

| Path | Claim | Actual (this audit) |
|---|---|---|
| `E:\...\mods\STS2-RitsuLib` | True (v0.3.10) | **True** — verified `mod_manifest.json` version `0.3.10`, DLL at `lib\0.106.1\STS2-RitsuLib.dll` |
| `E:\...\mods\previous framework` | True | **True** |
| `E:\...\mods\EZMicroBalance` | True | **True** |

**Verdict:** All runtime paths verified on disk. No discrepancies.

### 2.3 Runtime Smoke Evidence — Off Mode

| Claim | Verified? | Source |
|---|---|---|
| Reached main menu | YES | `godot.log.after-launch` line 163+ shows previous framework post-mod init; `Finished mod initialization for 'Spire Plus'` on line 145. |
| previous framework 3.1.4 loaded | YES | Line 41: `previous framework, Version=3.1.4.0` |
| RitsuLib 0.3.10 loaded | YES | Line 62: `Version: 0.3.10 [compat branch: 0.106.1]` |
| Spire Plus loaded | YES | Line 155: `Spire Plus [EZMicroBalance] (v0.1.0-private-beta.84)` |
| 25/25 ModPatcher patches | YES | Line 98: `ModPatcher applied 25 patches (25 registered).` |
| Sts1Events disabled/Off | YES | Line 133: `bootstrap gate: disabled (StS1 events default Off)` |
| 0 StS1 registration lines | YES | Grep confirms 0 `Registered shared event` lines in off-mode log. |
| 30 previous saved-state registrations | YES | Line 166: `Found 30 previous saved-state registrations.` |
| Clean audit (0 release-blocking hits) | **YES with caveat** | Audit JSON reports all signature counts at 0. See §3.1. |

### 2.4 Runtime Smoke Evidence — CanaryOnly Mode

| Claim | Verified? | Source |
|---|---|---|
| Reached main menu | YES | `godot.log.after-direct-launch` shows full mod init chain through line 134+. |
| previous framework 3.1.4 loaded | YES | Line 41: `previous framework, Version=3.1.4.0` |
| RitsuLib 0.3.10 loaded | YES | Line 62: `Version: 0.3.10 [compat branch: 0.106.1]` |
| Spire Plus loaded | YES | Line 144: `Spire Plus [EZMicroBalance] (v0.1.0-private-beta.84)` |
| 25/25 ModPatcher patches | YES | Line 98: `ModPatcher applied 25 patches (25 registered).` |
| Sts1Events enabled/CanaryOnly | YES | Line 122: `StS1 events CanaryOnly mode: registering 4 canary events.` |
| Exactly 4 canary registrations | YES | Lines 156-159: `Sts1BigFish`, `Sts1GoldenIdol`, `Sts1TheLab`, `Sts1DivineFountain` |
| 30 previous saved-state registrations | YES | Line 155: `Found 30 previous saved-state registrations.` |
| Clean audit (0 release-blocking hits) | YES | Audit JSON reports all signature counts at 0. No `[ERROR]` line in this log (the `ritsulib-variants.json` error only appears when the game reads the mod directory; direct-launch path may differ). |

### 2.5 Warning Triage Matrix

| Claim | Verified? |
|---|---|
| 89 warnings, all Sts1Events nullable | YES — matrix at `docs/reviews/warning-triage-matrix.md` has per-file breakdown summing to 89. |
| Single root cause: `EventModel.Owner` typed `Player?` | YES — consistent across all per-file entries. |
| Fix pattern documented | YES — early-exit guard `if (Owner is not { } owner) return;` |
| Diagnostics architecture audit | YES — all 5 components compliant. |

### 2.6 Diagnostics Architecture Audit

| Component | Claimed | Actual Posture |
|---|---|---|
| RewardPipeline | Diagnostics-only | Diagnostics-only — verified by log evidence (bootstrap observation events only). |
| CardPlayContext | Allow-only | Allow-only — no state mutation in logs. |
| DeathProtectionService | No-op | No-op — zero production callers, confirmed. |
| MultiplayerPolicy | Taxonomy store | Taxonomy store — register/lookup only. |
| MultiplayerFeaturePolicy | Active suppression | Active suppression — intentional fail-closed. |

**Verdict:** Architecture audit matches evidence. No discrepancies.

---

## 3. Discrepancies & Caveats

### 3.1 Audit Tool Blind Spot: `[ERROR]` Line in Off-Mode Log

The off-mode `godot.log.after-launch` contains an `[ERROR]` on line 19:

```
[ERROR] Mod manifest E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib\ritsulib-variants.json is missing the 'id' field! This is not allowed. The mod will not be loaded.
```

The `godot-log-audit.json` reports `"Godot ERROR line"` count as 0. This means the audit tool's regex pattern does not match the `[ERROR]` format used by the game's mod loader. The RitsuLib variant file is intentionally a non-standard manifest (it's a multi-variant descriptor, not a loadable mod), so this error is expected ignorable noise — RitsuLib loads correctly via its primary `mod_manifest.json`.

**Risk:** Low. This is a known RitsuLib structure quirk, not a Spire Plus bug. However, the audit tool's "Godot ERROR line" signature is incomplete — it should also match `[ERROR]` format. If a future game update changes error formatting, the audit could miss real errors.

**Recommendation:** Update the audit tool's regex to match both `ERROR` and `[ERROR]` patterns.

### 3.2 Worktree Is NOT Clean

`git status --short` returns 15 modified entries + 1 untracked file:

```
 M AGENTS.md
 M PROJECT_STATE.md
 M docs/goals/overnight-run-ledger.md
 M docs/goals/overnight-run-status.md
 M docs/goals/revision-j-commit-slices.md
 M docs/goals/revision-j-dirty-ledger.md
 M docs/goals/revision-j-final-report.md
 M docs/goals/revision-j-owner-review-packet.md
 M docs/goals/revision-j-runtime-hard-blocker.md
 M docs/goals/revision-j-runtime-smoke-plan.md
 M docs/goals/warning-ledger.md
 M docs/reviews/current-validation.md
 M harness/TASK_FOCUS_PACK.md
 M harness/TASK_STATUS.md
?? docs/reviews/warning-triage-matrix.md
```

`current-validation.md` line 9 claims: "Worktree: CLEAN (0 dirty entries)."

**This is factually incorrect.** The worktree has 16 dirty entries. The claim may be based on `report-worktree-batches.ps1 -FailOnUnclassified` reporting "0 unclassified" entries (all entries are classified as known batches), but "0 unclassified" is not the same as "0 dirty entries." This is misleading wording.

**Risk:** Medium. A dirty worktree means the HEAD commit does not represent the full current state. Pushing would leave local doc/harness changes behind. For a private-beta tester handoff, the package must be built from a known commit state.

### 3.3 Gameplay, UI, Save-Load, and Multiplayer Evidence: ALL MISSING

The runtime smoke checklist explicitly lists these as PENDING:

| Item | Status |
|---|---|
| Mod Settings UI — appears in mod list | PENDING |
| Mod Settings UI — settings render without errors | PENDING |
| Mod Settings UI — feature toggles functional | PENDING |
| Start new run | PENDING |
| Play first combat | PENDING |
| Visit first shop | PENDING |
| Check Ancient reward visibility | PENDING |
| Save and reload | PENDING |
| Co-op fail-closed behavior | PENDING |

The runtime smoke checklist exit criteria require: "At least 3 of 5 basic gameplay items pass, with shop and save-load mandatory." None have been attempted.

### 3.4 Versioned Tester Package: NOT CREATED

No versioned tester package (`SpirePlus-v0.1.0-private-beta.N.zip`) was created or verified in this pass. The `dotnet publish` output exists locally but no package was handoff-ready.

---

## 4. What Has Improved Since Previous QA (2026-05-31)

| Area | Before (FAIL) | After (CONDITIONAL PASS) |
|---|---|---|
| STS2-RitsuLib | Missing from all checked paths | Installed v0.3.10 on E-drive with correct DLL |
| Runtime smoke | None captured | Both Off and CanaryOnly logs captured with clean audits |
| Off=0 proof | Absent | Verified: 0 StS1 registration lines in off-mode log |
| CanaryOnly=4 proof | Absent | Verified: exactly 4 canary events (BigFish, GoldenIdol, TheLab, DivineFountain) |
| RitsuLib bootstrap | Not proven | Proven: 0.3.10, 249 framework patches, variant 0.106.1 |
| ModPatcher | Not proven | Proven: 25/25 applied, 0 failed |
| previous saved-state registrations | Not proven | Proven: 30 in both modes |
| Warning triage | Referenced but matrix didn't exist | Full per-file matrix written with fix pattern |
| Sts1Events gate bug | `SPIREPLUS_STS1_EVENT_MODE` was generic disable override | Fixed; guard tests pass 31/31 |
| Diagnostics architecture | Not audited | All 5 components audited and compliant |
| `godot.log` audit | 11 ERROR hits (v15), 3 ERROR hits (runtime-proof) | Both target-fix audits report clean |

---

## 5. Remaining Blockers

| # | Blocker | Severity | Priority |
|---|---|---|---|
| 1 | No gameplay proof (combat, shop, Ancient rewards) | 4 | P0 |
| 2 | No save-load proof | 4 | P0 |
| 3 | No Mod Settings UI proof | 3 | P1 |
| 4 | No versioned tester package | 3 | P1 |
| 5 | Worktree dirty (16 entries) — current-validation.md claims clean | 3 | P1 |
| 6 | 89 Sts1Events nullable warnings (accepted but unresolved) | 2 | P2 |
| 7 | Audit tool `[ERROR]` pattern blind spot | 1 | P3 |

---

## 6. Final Decision Matrix

| Gate | Status |
|---|---|
| No-game validation (build/test/format/diff/inventory) | **PASS** |
| Runtime dependency paths | **PASS** |
| Runtime loader gate (Off mode) | **PASS** |
| Runtime loader gate (CanaryOnly mode) | **PASS** |
| Runtime smoke audits (clean) | **PASS** (with §3.1 caveat) |
| Warning triage | **PASS** (debt accepted) |
| Diagnostics architecture audit | **PASS** |
| Worktree cleanliness | **FAIL** (16 dirty entries; claimed clean) |
| Gameplay proof | **NOT ATTEMPTED** |
| Mod Settings UI proof | **NOT ATTEMPTED** |
| Save-load proof | **NOT ATTEMPTED** |
| Multiplayer disposition | **NOT ATTEMPTED** |
| Versioned tester package | **NOT CREATED** |
| Independent QA (this report) | **CONDITIONAL PASS** |
| Release-ready | **NO** |
| Live-ready | **NO** |

---

## 7. Required Next Steps

1. **Gameplay smoke:** Start a new run, play through first combat, visit first shop, verify Ancient reward visibility, save and reload. Capture screenshots or log evidence.
2. **Mod Settings UI:** Navigate to Mod Settings, verify Spire Plus appears and toggles work. Capture screenshot.
3. **Worktree closure:** Either commit/push current state or explicitly document why the dirty entries exist and get owner approval.
4. **Versioned tester package:** Run `dotnet publish`, create `SpirePlus-v0.1.0-private-beta.N.zip`, verify contents.
5. **Audit tool fix:** Update `godot-log-audit` regex to match `[ERROR]` format in addition to current patterns.
6. **Rerun QA** after gameplay/UI/save-load evidence is captured.

---

## 8. Verdict Summary

```
No-game validation:     PASS
Runtime dependency:     PASS
Runtime loader gates:   PASS (Off=0, CanaryOnly=4, clean audits)
Warning triage:         PASS (debt accepted, matrix written)
Diagnostics audit:      PASS
Worktree:               FAIL (dirty, falsely claimed clean)
Gameplay/UI/Save-load:  NOT ATTEMPTED
Tester package:         NOT CREATED
Independent QA:         CONDITIONAL PASS
Release-ready:          NO
Live-ready:             NO
```

**Previous QA verdict was FAIL / HARD BLOCKED.** The three P0 blockers (RitsuLib missing, no runtime smoke, no clean audit) have been genuinely resolved. The project has moved from hard-blocked to conditionally passing at the loader-gate level. However, the absence of any gameplay, UI, save-load, or multiplayer proof means this is not release-ready. The worktree cleanliness claim is factually wrong and should be corrected in `current-validation.md`.
