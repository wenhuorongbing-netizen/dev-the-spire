# Refactor Entry Gate — beta.135 (Runtime-Baseline-Gated)

Status: **HARD GATE — active, refactor lane is BLOCKED-on-baseline.**
Applies to: any structural/mechanical refactor of an already-migrated module in `EZMicroBalanceCode/`.
Reconciled against HEAD `b0f0b33`. Upstream contract: `docs/testing/beta135-runtime-baseline.md` (debug-owned).

> **The one sentence:** No refactor slice may START until debug delivers a passing beta.135
> runtime baseline. With no baseline, "behavior unchanged" has no oracle, so it is not a
> claim — it is a guess, and guesses do not merge.

This is a gate, not a guideline. Each rule below is a precondition or a stop condition. If a
rule is unmet, the slice does not start, or does not merge.

---

## 0. Gate state right now

| Condition | State |
| --- | --- |
| beta.135 runtime baseline captured? | **NO — `pending-owner-run`** (scaffold + offline checker exist; no real `godot.log.after-launch` yet) |
| Refactor slices allowed to start? | **NO** |
| What unblocks it | Debug runs the owner smoke + `check-beta135-runtime-baseline-log.ps1` and lands a packet with `BaselineLogCheckStatus = pass`, `TrustAnchorMode = canonical-configured` |

Until the table's first row reads "YES", this lane produces planning artifacts only
(this gate, the candidate map, proposals). No `.cs`. No behavior change.

---

## 1. Required baseline evidence path (the precondition)

A refactor slice may begin only when ALL of the following exist and pass, produced by the
**debug** lane (refactor does not create them; refactor consumes them as the regression oracle):

1. An evidence directory under `.tools/runtime-evidence/beta135-runtime-baseline-YYYYMMDD-HHMMSS/`
   scaffolded by `scripts/new-beta135-runtime-baseline-evidence.ps1`.
2. A real `godot.log.after-launch` captured by the owner from a live Steam beta.135 session
   (RitsuLib + EZMicroBalance only, reached main menu). **Not** fabricated, **not** the
   `-SelfTest` sample, **not** a beta.128/older drift log.
3. A passing checker report from
   `scripts/check-beta135-runtime-baseline-log.ps1 ... -FailOnMismatch`, i.e.
   `runtime-baseline-log-check.json` with `BaselineLogCheckStatus = pass` and the retained
   `godot-log-audit.json` clean, schema v2, full named-signature vector present.
4. Trust anchors green: `TrustAnchorMode = canonical-configured` (a
   `noncanonical-override-test-only` packet is fixture-only and **does not** satisfy this gate),
   and the scaffold Git/worktree binding matches the HEAD the baseline was taken at.

This baseline is the **before** snapshot. Honesty boundary inherited from the upstream doc:
it is **marker-only** evidence (`LogOriginProofStatus = marker-only-origin-not-proven-by-offline-checker`)
and proves startup/main-menu markers only — never release, gameplay, save-load, or co-op
readiness. The refactor gate inherits the same ceiling: a passing baseline authorizes a
*structural* slice to be diffed; it does not bless gameplay claims.

## 2. Required before/after diff fields (the merge contract)

For every refactor slice, capture the same evidence packet **before** (the baseline above)
and **after** (a fresh capture on the refactored build, same scenario, same RitsuLib + mod-only
setup, same path to main menu). The slice may merge only if **every** field below is unchanged.

| # | Diff field | Source of truth | Pass = |
| --- | --- | --- | --- |
| 1 | **godot.log events** | `godot.log.after-launch` filtered to `[Spire Plus]` + RitsuLib bootstrap lines | Same line set, same order, same text. No new/missing/reordered lines. No new `ERROR`/`WARN`. |
| 2 | **Patch-apply count** | `RitsuLibBootstrap` log line `ModPatcher applied {N} patches ({M} registered).` | `N == 168` before AND after (and `M` unchanged). Any drift = behavior change. |
| 3 | **Feature-gate state** | `FeatureRegistry.LogFeatureSummary()` per-module `bootstrap=…, live=…, reason=…` lines (6 modules) | Identical enabled/disabled/reason for every module. |
| 4 | **Loader path** | `MainFile.Initialize()` call sequence markers (bootstrap -> saved-state register -> config -> content registration -> feature init -> summary) + no loader/initializer exception | Same markers, same order, no `TypeLoadException` / `MissingMethodException` / `TargetInvocationException` / initializer exception / loader failure. |
| 5 | **Key event counts** | checker's `godot-log-audit.json` native audit array/bool/int shape + release-blocking signature vector; discovered manifest id set (`EZMicroBalance`, `STS2-RitsuLib`, no extras) | Same counts, same clean signature set, same manifest id set, no retired BaseLib markers. |

Recommended mechanics (no new launch beyond debug's): diff the two `godot.log.after-launch`
files (normalize timestamps/PIDs only) and diff the two `runtime-baseline-log-check.json` /
`godot-log-audit.json` reports. Field 2 is the single most load-bearing assertion — **168**.

A slice that cannot produce a clean diff on all five fields is not "almost done"; it is either
not a refactor (it changed behavior) or its evidence is incomplete. Either way: no merge.

## 3. Allowed migrated-module scope

A refactor slice may touch a unit only if ALL hold:

- It is **already migrated** (lives in `EZMicroBalanceCode/`, registered through RitsuLib, no
  raw Harmony) and listed/derivable from `docs/patch-inventory.md`.
- The change is **purely structural/mechanical**: extract method, rename local/private member,
  reorder members, split/merge a file, move a folder (move-only), dedupe identical code,
  tighten access modifiers — **with no change to observable behavior, no change to any
  `PatchId`, no change to patch target/attribute, no change to the applied-patch count.**
- One slice = one bounded unit (one file or one tightly-coupled file group). If it can't
  produce a clean five-field diff in one pass, it is too big — split it.
- It carries a before/after runtime diff per Section 2.

Lowest-blast-radius units are preferred first (see the candidate map). A unit whose output is
**suppressed in the default baseline** (gated behind an off-by-default flag/env var, or only
reachable inside a run/combat) is safest because its absence in the baseline log is itself the
invariant to preserve.

## 4. Blocked surfaces (do not refactor under this gate)

| Surface | Why blocked |
| --- | --- |
| `Core/Features/**` (`FeatureRegistry`, `SpirePlusFeatureRegistry`, `IFeatureModule`, gate/order types) | **Owner-LOCKED.** Also the source of the Section-2 feature-gate log lines — touching it changes the very oracle. |
| `Sts1Events/**` | Owned by the event lane; active port surface, not closed. |
| Public API consumed across lanes | Changing it ripples into event/migration; requires a BOARD "API 变更" row + owner sign-off, which makes it not a silent refactor. |
| Migration close-out code / RitsuLib integration layer (`Core/Integrations/RitsuLib/**`) | Folder moves here must not mix with behavior changes; the patch register/apply path defines the 168 count — high blast radius. |
| Any `IPatchMethod` patch class where the change risks `PatchId`, patch target, attribute, registration, or the **168** applied count | This is the high-risk patch seam. Internal-only edits *inside* a patch body (e.g. extracting a private helper, moving a string constant) may be allowed **only** with a clean five-field diff and owner approval; anything that could move the count is blocked. |
| High-risk lifecycle seams: run, room, save, lobby, multiplayer, game lifecycle | Highest source-drift blast radius; out of scope for mechanical refactor this round. |
| `.cs` behavior, this round, anywhere | Round state is BLOCKED-on-baseline: planning only. |

## 5. Owner-approval rule

- **Owner approves the baseline.** Refactor may not self-declare the baseline ready. The
  starting baseline must be the debug-delivered, owner-run, checker-passing packet
  (`TrustAnchorMode = canonical-configured`). No owner-run baseline -> no slice.
- **Owner approves any patch-seam touch.** Any edit inside an `IPatchMethod` file (even a
  "purely internal" extraction) requires explicit owner approval in addition to a clean
  five-field diff, because the 168 count and `PatchId` set are release-affecting.
- **Owner makes the merge call.** The machine may only assert "before/after runtime diff is
  clean on all five fields"; whether the slice merges is the owner's decision. The assistant
  does not commit or push — the coordinator integrates after owner sign-off.
- **No release/playable claims** are produced by this lane. Inherited from the marker-only
  baseline ceiling.

## 6. The three no-merge conditions (hard stops)

1. **No baseline -> the slice may not START.** (Precondition, Section 1.)
2. **Baseline diff mismatch -> the slice may not MERGE.** Any of the five Section-2 fields
   differs -> stop. Do not "explain away" a delta.
3. **Any gameplay / log / patch-count delta -> REVERT or RECLASSIFY as a behavior change.**
   A refactor that changed observable behavior was never a refactor; it leaves this gate and
   goes through the normal behavior-change path (spec + reproduction + owner review), it does
   not sneak through as "cleanup".

## 7. Slice lifecycle under this gate

```
[BLOCKED-on-baseline]  <- you are here
        |
        v  (debug delivers passing canonical beta.135 baseline; owner approves)
[Slice may start]
        |
        v  pick lowest-blast-radius unit from candidate map (one bounded unit)
[Structural change only, no PatchId/target/count change]
        |
        v  capture AFTER packet, diff all 5 fields vs baseline BEFORE
   clean? --no--> condition #2 or #3: revert / reclassify
        |yes
        v  owner approves merge -> coordinator integrates
[Next slice]
```

## 8. Cross-references

- Upstream baseline contract (debug): `docs/testing/beta135-runtime-baseline.md`
- Patch inventory (169 source / 168 runtime): `docs/patch-inventory.md`
- Candidate map + first proposal: `refactor-candidate-map-beta135.md` (this folder)
- Folder/move plan: `docs/refactor-map.md`
- PRD / acceptance: `refactor-entry-gate-beta135-prd.md` (this folder)
- Auto-play acceptance gate (project CI): `../../../../dev-the-spire-harness/coordination/auto-play-gate.md`
