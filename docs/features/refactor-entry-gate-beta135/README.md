# Refactor Entry Gate — beta.135

Planning artifacts for **Runtime-Baseline-Gated Refactor Entry**. Round state:
**BLOCKED-on-baseline** — docs only, no `.cs`/behavior change. The refactor lane may not start
any slice until the **debug** lane delivers a passing, owner-run beta.135 runtime baseline
(`docs/testing/beta135-runtime-baseline.md`, currently `pending-owner-run`).

| File | Role |
| --- | --- |
| `refactor-entry-gate-beta135-prd.md` | PRD / spec: scope, repo reconciliation, acceptance criteria, risks. |
| `refactor-entry-gate-beta135.md` | **The HARD GATE.** Required baseline evidence path, the 5 before/after diff fields, allowed scope, blocked surfaces, owner-approval rule, the 3 no-merge conditions. |
| `refactor-candidate-map-beta135.md` | Migrated-module candidate map (ranked lowest -> highest runtime blast radius) + the single first-candidate proposal (proposal only). |

## Load-bearing facts (verified at HEAD `b0f0b33`)

- Source patch classes = **169** `IPatchMethod`; **runtime applied = 168**
  (`Sts1ReplacementPrototype` is `#if REPLACEMENT_PROTOTYPE_ENABLED`, off by default).
  Use **168** as the patch-apply invariant. Raw `[HarmonyPatch]` in code = 0.
- Baseline is **marker-only / startup+main-menu only** — never a release, gameplay,
  save-load, or co-op claim.
- First proposed slice: `EZMicroBalanceCode/Preview/TransformPredictionService.cs`
  (not a patch, run-only, single in-module caller) — **proposal only, not implemented.**

Upstream contract: `docs/testing/beta135-runtime-baseline.md` (debug-owned).
