# PRD / Spec — beta.135 Runtime-Baseline-Gated Refactor Entry

Status: PLANNING (docs only). Track: refactor. Round state: **BLOCKED-on-baseline**.
Author lane: refactor. Reconciled against HEAD `b0f0b33` (`docs/testing/beta135-runtime-baseline.md` landed by debug, capture still `pending-owner-run`).

This PRD governs three documentation deliverables that build the **hard entry gate**
for any future refactor of an already-migrated module. It produces no `.cs` changes and
no behavior changes this round.

---

## 1. Problem / why this exists

The baselib -> RitsuLib migration is closed: 169 `IPatchMethod` patch classes in source,
**168 applied at runtime** (one is compile-gated off), raw `[HarmonyPatch]` in code = 0,
static build 0/0. The next natural step is structural cleanup of the migrated modules
(dedupe, extract, rename, folder moves). That work claims "behavior unchanged" — but the
**beta.135 runtime baseline has not been captured yet** (it is debug's owner-run gate). With
no baseline log to diff against, "behavior unchanged" is unfalsifiable. Starting a refactor
slice now would be a claim with no oracle.

So this round does not refactor anything. It builds the gate that makes the *next* round
provably safe: a documented evidence path + before/after diff contract such that every
migrated-module refactor can show the runtime log, patch-apply count, feature-gate state,
loader path, and key event counts are byte/markers-identical before and after.

## 2. Scope

In scope (this round, docs only):
1. A refactor **entry-gate** document — the HARD GATE: required baseline evidence path,
   required before/after diff fields, allowed migrated-module scope, blocked surfaces,
   owner-approval rule, and the three no-merge conditions.
2. A migrated-module refactor **candidate MAP** (markdown table) ranked by lowest runtime
   blast radius, populated by reading the code.
3. **One** first candidate proposal — the single smallest/safest unit — as a *proposal only*,
   with its expected "no behavior change" observables written out. Not implemented.
4. This PRD + a folder README tying the three together.

Explicitly **out of scope (non-goals)**:
- Any `.cs` edit, any behavior change, any patch add/move/delete.
- Implementing the first candidate (that is Slice D, blocked on the debug baseline).
- Launching the game / capturing the baseline (debug owns that; owner-run).
- Touching migration close-out code, `Sts1Events/`, public API, `Core/Features/**` (LOCKED),
  or any locked source area.
- Re-deriving or "correcting" the debug baseline doc — this PRD *consumes* it as the upstream
  contract; it does not edit it.
- Committing or pushing (coordinator integrates).

## 3. Reconciliation with current repo truth (read now, not from old snapshot)

| Fact | Value | Source (verified this round) |
| --- | --- | --- |
| Package | `v0.1.0-private-beta.135` | `docs/testing/beta135-runtime-baseline.md` |
| Game | STS2 `0.107.1` | same |
| RitsuLib | `0.4.34` (compat branch `0.107.1`) | same |
| Source patch classes | **169** `IPatchMethod` | `docs/patch-inventory.md`; grep `: IPatchMethod` = 169 / 68 files |
| **Runtime applied patch count** | **168** | `Sts1ReplacementPrototype.cs:18` `#if REPLACEMENT_PROTOTYPE_ENABLED` (symbol undefined in any `.csproj`/`.props`); `beta135-runtime-baseline.md` `-ExpectedPatchCount 168` |
| Raw `[HarmonyPatch]` in code | 0 | grep over `EZMicroBalanceCode/` (the one hit is README prose) |
| Patch register entry | `SpirePlusRitsuLibPatchRegistry.RegisterAll(ModPatcher)` | `Core/Integrations/RitsuLib/SpirePlusRitsuLibPatchRegistry.cs:10` |
| Patch apply + log line | `RitsuLibFramework.ApplyRequiredPatcher(...)` -> logs `ModPatcher applied {AppliedPatchCount} patches ({RegisteredPatchCount} registered).` | `Core/Integrations/RitsuLib/RitsuLibBootstrap.cs:13,42,50` |
| Loader entry | `[ModInitializer]` `MainFile.Initialize()` (ModId `EZMicroBalance`) | `EZMicroBalanceCode/MainFile.cs:12,20` |
| Feature gate state source | `FeatureRegistry.InitializeAll()` + `LogFeatureSummary()` over 6 modules | `Core/Features/FeatureRegistry.cs:79-85`; `SpirePlusFeatureRegistry.cs` |
| Baseline capture status | **pending-owner-run** (scaffold + checker exist, no real log yet) | `beta135-runtime-baseline.md` "Owner-Run Boundary" |
| Build | 0 warnings / 0 errors (documented; not re-run this round) | `PROJECT_STATE.md:11-12` |
| Git HEAD | `b0f0b33` | `git rev-parse HEAD` |

**Critical load-bearing number:** the gate and candidate map must use **168** as the
expected patch-apply count, never the 169 source figure. A refactor that flips a unit from
compiled-out to compiled-in (or vice versa) would change 168 and is by definition a behavior
change, not a refactor.

## 4. Upstream dependency (explicit)

This entire gate is downstream of debug. Refactor cannot manufacture the baseline.

- Debug delivers: a captured `godot.log.after-launch` + passing
  `runtime-baseline-log-check.json` (`BaselineLogCheckStatus = pass`,
  `TrustAnchorMode = canonical-configured`) under `.tools/runtime-evidence/`, via
  `scripts/check-beta135-runtime-baseline-log.ps1`.
- Until that exists, every refactor slice is **BLOCKED-on-baseline**. The gate doc states
  this as a hard precondition, not a recommendation.

## 5. File plan (all inside Allowed `docs/**`)

| File | Action | Purpose |
| --- | --- | --- |
| `docs/features/refactor-entry-gate-beta135/refactor-entry-gate-beta135-prd.md` | NEW (this file) | Spec + acceptance |
| `docs/features/refactor-entry-gate-beta135/refactor-entry-gate-beta135.md` | NEW | The HARD GATE doc (deliverable 1) |
| `docs/features/refactor-entry-gate-beta135/refactor-candidate-map-beta135.md` | NEW | Candidate MAP + first proposal (deliverables 2 + 3) |
| `docs/features/refactor-entry-gate-beta135/README.md` | NEW | Folder map |

No other path is touched. No `.cs`. No `Core/Features/**`. No `Sts1Events/**`.

## 6. Acceptance criteria (how each deliverable is "fully implemented")

D1 — Entry-gate doc:
- [ ] States plainly: **no refactor slice may START until debug delivers the beta.135 runtime baseline.**
- [ ] Names the required baseline evidence path (the debug artifacts + check script + pass status).
- [ ] Lists the required before/after diff fields, at minimum: godot.log events, patch-apply
      count (168), feature-gate state, loader path, key event counts.
- [ ] Defines allowed migrated-module scope and blocked surfaces (Core/Features LOCKED,
      Sts1Events, public API, migration close-out, high-risk patch seams).
- [ ] Defines owner-approval rule.
- [ ] States the three no-merge conditions: no baseline -> slice may not start;
      baseline diff mismatch -> no merge; any gameplay/log/patch-count delta -> revert or
      reclassify as a behavior change.

D2 — Candidate MAP:
- [ ] Markdown table with columns: module | current RitsuLib patch class | runtime-baseline
      dependency | expected "no behavior change" observable | slice size | rollback path.
- [ ] Populated from reading the actual code (not guessed).
- [ ] Ranked by LOWEST runtime blast radius first.
- [ ] Uses 168 as the patch-apply invariant where relevant.

D3 — First candidate proposal:
- [ ] Exactly one unit chosen — the smallest/safest.
- [ ] Justifies why it is lowest-risk (no patch registration, not on the menu path, single
      caller, no logging at baseline).
- [ ] Writes the expected unchanged observables for the before/after runtime diff.
- [ ] Marked PROPOSAL ONLY / do-not-implement-until-baseline.

Process:
- [ ] Final independent review subagent confirms zero `.cs`/behavior change and that no
      deliverable smuggles a behavior assumption into a "structural" proposal.
- [ ] BOARD refactor row + dependency note updated to reflect the gate (coordinator commits).

## 7. Risks, dependencies, rollback

- **Risk: doc encodes a stale patch count.** Mitigation: pin 168 with the compile-gate
  citation; cross-check against `beta135-runtime-baseline.md` `-ExpectedPatchCount`.
- **Risk: a "safe" candidate is actually on the menu log path.** Mitigation: candidate map
  was built by scanning unconditional `MainFile.Logger` emitters; the Architecture canary
  files (`ArchitectureCanaryBootstrap`, `RewardPipeline`, etc.) that DO emit at menu are
  explicitly flagged as higher-risk, not picked.
- **Dependency: debug baseline (owner-run).** Hard blocker; surfaced in the gate doc and BOARD.
- **Rollback:** docs-only; `git checkout -- docs/features/refactor-entry-gate-beta135/`
  fully reverts. No build or runtime impact.

## 8. Lifecycle mapping

- Section 1 (Spec): this PRD.
- Section 2 (Build): the two deliverable docs + README, produced with parallel read-only
  analysis subagents feeding the candidate map.
- Section 3 (Review): independent QA/red-team subagent verifies acceptance + zero behavior
  change; loop back if any check fails.
- Stop condition: all acceptance boxes green AND review passes; OR hard-stop = the only
  remaining work is Slice D, which is correctly blocked on the debug baseline (owner-run).
