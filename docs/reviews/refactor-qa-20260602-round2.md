# QA/Red-Team Audit — Round 2 (2026-06-02 17:54)

## 2026-06-11 Revision M Supersession Note

This 2026-06-02 Round 2 QA report is historical `v0.106.1` loader-gate context only. Do not use its `CONDITIONAL PASS`, Off/CanaryOnly/AdditiveBatch1 `PASS`, RitsuLib `v0.3.10`, beta.84, warning-count, dirty-worktree, mod-isolation, or package status as current `event.md` proof. Current StS1 event work routes through `docs/goals/event.md`, `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/v20-final-gate-overlay.csv`, `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`, `PROJECT_STATE.md`, and `docs/test-ready-development-goal.md`: beta.85 proves current `v0.107.0` default-Off and CanaryOnly loader behavior, beta.86 proves AdditiveBatch1 loader/registration behavior, and gameplay, save-load, replacement, multiplayer, QA, handoff, and release-ready proof remain pending or blocked.

## Verdict

**CONDITIONAL PASS** — loader gates verified, canary warning fix confirmed, worktree cleanliness claim still factually wrong. Same release blockers persist.

---

## 1. Independent Verification Results

### 1.1 Worktree State

| Claim | Actual |
|---|---|
| `current-validation.md` line 9: "Worktree: **CLEAN** (0 dirty entries)" | **FALSE**. `git status --short` returns 10 modified + 6 untracked = **16 dirty entries**. |

Dirty files at time of audit:

```
 M AGENTS.md
 M EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1GoldenIdol.cs
 M EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1TheLab.cs
 M docs/features/ritsulib-migration/monthly-dev-spec.md
 M docs/goals/overnight-run-ledger.md
 M docs/goals/overnight-run-status.md
 M docs/goals/warning-ledger.md
 M docs/reviews/current-validation.md
 M harness/TASK_FOCUS_PACK.md
 M harness/TASK_STATUS.md
?? docs/goals/m5-week1-commit-slices.md
?? docs/goals/m5-week1-dirty-ledger.md
?? docs/goals/m5-week1-owner-review-packet.md
?? docs/goals/m5-week1-runtime-hard-blocker.md
?? docs/goals/m5-week1-runtime-smoke-plan.md
?? docs/goals/m5-week1-warning-ledger.md
```

This is the **same issue flagged in Round 1 QA**. The claim was not corrected. 16 dirty entries persist. Notably, 2 of the modified files are source code (`Sts1GoldenIdol.cs`, `Sts1TheLab.cs`) — these are the warning-fix files but they are not committed.

**Severity: P1 (Medium)**. The HEAD commit (`f20dd230`) does not represent the full current working state.

### 1.2 HEAD

| Claim | Actual |
|---|---|
| HEAD: `f20dd230` | **TRUE**. `git log --oneline -1` = `f20dd230 fix nullable warnings in 4 canary event files` |

### 1.3 Runtime Dependency Paths

| Path | Claim | Actual |
|---|---|---|
| `E:\...\mods\STS2-RitsuLib` | True (v0.3.10) | **TRUE**. Directory exists, `mod_manifest.json` present. |
| `E:\...\mods\BaseLib` | True | **TRUE** |
| `E:\...\mods\EZMicroBalance` | True | **TRUE** |

### 1.4 Build, Test, Format — Verified Live

| Command | Claim | Actual (this audit) |
|---|---|---|
| `dotnet build` | 0 errors, 79 warnings | **CONFIRMED**: 0 errors, 79 warnings |
| `dotnet test` | 464 passed, 0 failed, 21 skipped, 485 total | **CONFIRMED**: 464 passed, 0 failed, 21 skipped, 485 total |
| `dotnet format --verify-no-changes` | PASS | **CONFIRMED**: exit code 0 |

### 1.5 Canary Event Warning Fix

| Claim | Actual |
|---|---|
| CanaryOnly event files now have 0 nullable warnings | **TRUE**. Build output contains zero warnings referencing `Sts1BigFish.cs`, `Sts1GoldenIdol.cs`, `Sts1TheLab.cs`, or `Sts1DivineFountain.cs`. |

The warning-triage-matrix.md still shows the pre-fix total of 89. It was not updated to reflect the 79 count. This is a **stale doc**, not a factual error in current-validation.md.

### 1.6 CanaryOnly Runtime Smoke (Fresh at HEAD `f20dd230`)

Evidence path: `.tools\runtime-evidence\live-spire-plus-session-20260602-174656\`

| Claim | Verified? | Notes |
|---|---|---|
| Reached main menu | **YES** | Log shows full mod init through Spire Plus + post-init mods through line 500+. |
| Loaded exactly 3 mods (BaseLib, RitsuLib, Spire Plus) | **MISLEADING** | The session-state.json shows 25 other mods were moved to isolation. However, the game's cached mod list still loaded **24 mods** (see sort order lines 99-123). The doc acknowledges this ("Additional mods still loaded from cached mod list"). The claim "Loaded exactly 3 mods" is technically wrong — 24 mods loaded. Spire Plus itself loaded correctly among them. |
| Mod isolation attempted | **YES** | 25 mods moved per session-state.json |
| Mod isolation effective | **NO** | Game cached mod list before isolation took effect; all 24 mods still loaded |
| Applied 25/25 patches | **YES** | Line 202: `ModPatcher applied 25 patches (25 registered).` |
| CanaryOnly mode | **YES** | Line 226: `StS1 events CanaryOnly mode: registering 4 canary events.` |
| Registered exactly 4 canary events | **YES** | Lines 505-508: Sts1BigFish, Sts1GoldenIdol, Sts1TheLab, Sts1DivineFountain |
| 30 SavedSpireFields | **YES** | Line 504 |
| Clean audit | **NO AUDIT JSON EXISTS** | No `godot-log-audit.json` file in this evidence directory. The log contains **5 `[ERROR]` lines**: (1) RouteSuggestConfig.json missing id, (2) ritsulib-variants.json missing id, (3) sts2-heybox-support missing id, (4) duplicate BaseLib id, (5) heybox MethodNotFound. Errors 1, 3, 4, 5 are third-party mod issues. Error 2 is the known RitsuLib variant-manifest quirk. None are Spire Plus bugs, but an audit JSON was not generated. |
| All 6 FeatureRegistry diagnostics | **YES** | Lines 206-236 show bootstrap/live status for Ancients (Lotha, Morvi, Urda, VakuuFight), Ascension.A11A20, and Sts1Events. |

**Verdict on fresh CanaryOnly smoke:** The Spire Plus loader-gate claims are substantiated by the log (25/25 patches, 4 canary events, 30 SavedSpireFields, 6 features). The mod isolation claim is misleading — isolation was attempted but failed due to game caching. The "clean audit" claim cannot be verified because no audit JSON exists for this session.

---

## 2. Previous QA Blockers — Round 2 Status

| Blocker (from Round 1) | Round 1 Status | Round 2 Status |
|---|---|---|
| STS2-RitsuLib missing | RESOLVED | **STILL RESOLVED** |
| No runtime smoke evidence | RESOLVED | **STILL RESOLVED** |
| No clean godot.log audit | RESOLVED (with caveat) | **STILL RESOLVED** — K1 smokes have audit JSONs; fresh CanaryOnly does not |
| Worktree dirty (claimed clean) | FAIL (16 entries) | **STILL FAIL** (16 entries, claim not corrected) |
| `SPIREPLUS_STS1_EVENT_MODE` gate bug | RESOLVED | **STILL RESOLVED** |
| 89 warnings | Accepted debt | **IMPROVED** — 79 warnings, 10 fixed in canary files |

---

## 3. What Has Improved Since Round 1

| Area | Round 1 | Round 2 |
|---|---|---|
| Canary event nullable warnings | 89 total, canary files included | **79 total, canary files clean (0 warnings)** |
| Fresh CanaryOnly smoke | K1 smoke at HEAD 8f2d79b4 | **Fresh smoke at HEAD f20dd230 with mod isolation attempt** |
| Warning triage matrix | Written, pre-fix counts | **Written but stale** — shows 89, actual is 79 |
| Canary source files committed | Not committed | **NOT COMMITTED** — Sts1GoldenIdol.cs and Sts1TheLab.cs are dirty in working tree |

---

## 4. Discrepancies

### 4.1 Worktree Claim (Repeat from Round 1)

`current-validation.md` line 9: "Worktree: **CLEAN** (0 dirty entries)."

**This is factually incorrect for the second consecutive QA round.** 16 dirty entries exist. The claim conflates "0 unclassified by the batch reporter" with "0 dirty entries." They are not the same thing.

### 4.2 Fresh CanaryOnly "Loaded Exactly 3 Mods"

`current-validation.md` line 45: "Loaded exactly 3 mods (BaseLib, RitsuLib, Spire Plus)."

The log shows 24 mods in the sort order. The doc itself acknowledges this in the same sentence ("Additional mods still loaded from cached mod list"). The claim is self-contradictory.

### 4.3 Fresh CanaryOnly "Clean Audit"

`current-validation.md` line 45 does not explicitly claim a clean audit JSON for this session, but the absence of an audit JSON is also not noted. The log contains 5 `[ERROR]` lines. If an audit were generated with the existing tool, the result depends on the regex pattern — Round 1 QA already flagged the `[ERROR]` regex blind spot.

### 4.4 Warning Triage Matrix Stale

`warning-triage-matrix.md` total: 89. Actual build: 79. The matrix was written before the canary warning fix and not updated.

---

## 5. Remaining Blockers

| # | Blocker | Severity | Priority | Status |
|---|---|---|---|---|
| 1 | No gameplay proof (combat, shop, Ancient rewards) | 4 | P0 | **UNCHANGED — NOT ATTEMPTED** |
| 2 | No save-load proof | 4 | P0 | **UNCHANGED — NOT ATTEMPTED** |
| 3 | No Mod Settings UI proof | 3 | P1 | **UNCHANGED — NOT ATTEMPTED** |
| 4 | No versioned tester package | 3 | P1 | **UNCHANGED — NOT CREATED** |
| 5 | Worktree dirty (16 entries) — falsely claimed clean | 3 | P1 | **UNCHANGED — claim still wrong** |
| 6 | Canary source files not committed (Sts1GoldenIdol.cs, Sts1TheLab.cs) | 2 | P2 | **NEW** — warning fixes exist only in working tree |
| 7 | Warning triage matrix stale (shows 89, actual 79) | 2 | P2 | **NEW** — doc drift |
| 8 | Mod isolation failed in fresh CanaryOnly smoke | 2 | P2 | **NEW** — game cached mod list; isolation methodology needs review |
| 9 | No audit JSON for fresh CanaryOnly session | 2 | P2 | **NEW** — cannot independently verify clean audit claim |
| 10 | Audit tool `[ERROR]` pattern blind spot | 1 | P3 | **UNCHANGED** from Round 1 |

---

## 6. Final Decision Matrix

| Gate | Status |
|---|---|
| No-game validation (build/test/format) | **PASS** (verified live: 0 errors, 79 warnings, 464/0/21/485) |
| Runtime dependency paths | **PASS** |
| Runtime loader gate (Off mode) | **PASS** (K1 evidence with audit JSON) |
| Runtime loader gate (CanaryOnly mode) | **PASS** (K1 evidence with audit JSON + fresh log without audit JSON) |
| Canary event warning fix | **PASS** (0 warnings from 4 canary files) |
| Warning triage | **PASS** (debt reduced 89→79; matrix stale but current-validation.md correct) |
| Diagnostics architecture audit | **PASS** (from Round 1, no changes) |
| Worktree cleanliness | **FAIL** (16 dirty entries; claimed clean for 2nd round) |
| Gameplay proof | **NOT ATTEMPTED** |
| Mod Settings UI proof | **NOT ATTEMPTED** |
| Save-load proof | **NOT ATTEMPTED** |
| Multiplayer disposition | **NOT ATTEMPTED** |
| Versioned tester package | **NOT CREATED** |
| Independent QA (this report) | **CONDITIONAL PASS** |
| Release-ready | **NO** |
| Live-ready | **NO** |

---

## 7. Comparison to Round 1

| Aspect | Round 1 Verdict | Round 2 Verdict | Delta |
|---|---|---|---|
| Overall | CONDITIONAL PASS | CONDITIONAL PASS | **Same level** |
| Build/test | PASS | PASS (verified live) | **Strengthened** |
| Loader gates | PASS | PASS | Same |
| Canary warnings | Accepted (89) | **Improved (79, canary clean)** | **Better** |
| Worktree | FAIL (claimed clean) | FAIL (claimed clean) | **Same — uncorrected** |
| Gameplay/UI/Save | NOT ATTEMPTED | NOT ATTEMPTED | Same |
| Tester package | NOT CREATED | NOT CREATED | Same |

**Net improvement:** Canary warning debt reduced by 10. Build/test verified live. All other blockers unchanged.

---

## 8. Required Next Steps (Priority Order)

1. **Commit or stash dirty files.** The canary warning fixes (`Sts1GoldenIdol.cs`, `Sts1TheLab.cs`) and all doc changes need to be committed or explicitly documented as owner-approved local state. Fix the "CLEAN" claim in `current-validation.md`.
2. **Gameplay smoke.** Start a run, play first combat, visit shop, verify Ancients, save/load. This is the #1 blocker.
3. **Mod Settings UI screenshot.**
4. **Update warning-triage-matrix.md** to reflect 79 warnings.
5. **Version decision.** Either create a versioned tester package or explicitly record "local diagnostic only."
6. **Fix mod isolation methodology.** The game caches its mod list; isolation must happen before the game reads mods, or the cached list must be cleared.
7. **Generate audit JSON for fresh CanaryOnly session** (if log is still available), or re-capture with audit.
8. **Rerun QA** after gameplay/UI/save-load evidence is captured.

---

## 9. Verdict Summary

```
No-game validation:     PASS (verified live)
Runtime dependency:     PASS
Runtime loader gates:   PASS (Off=0, CanaryOnly=4, with K1 audit JSONs)
Canary warning fix:     PASS (79 warnings, canary files clean)
Warning triage:         PASS (debt reduced, matrix stale)
Diagnostics audit:      PASS
Worktree:               FAIL (16 dirty, falsely claimed clean)
Gameplay/UI/Save-load:  NOT ATTEMPTED
Tester package:         NOT CREATED
Independent QA:         CONDITIONAL PASS
Release-ready:          NO
Live-ready:             NO
```

**Round 1 was CONDITIONAL PASS. Round 2 remains CONDITIONAL PASS.** The canary warning fix is a genuine improvement and the live build verification strengthens the no-game validation evidence. However, the worktree cleanliness claim is still factually wrong, no gameplay/UI/save-load evidence has been added, and no versioned tester package exists. The project has not advanced beyond the loader-gate stage.
