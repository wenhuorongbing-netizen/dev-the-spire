# HARD STOP BLOCKER REPORT

Date: 2026-05-29
Session: Mandatory Overnight Run v2

> Superseded for v13 governance: this file is historical O0-O24 context only. Current O0-O46 status is tracked by the v13 evidence/report and must not inherit this file's old "code-doable gates" wording as a v13 pass.

## Summary

**20 of 25 gates are GREEN.** 5 gates require game launch or external resources that cannot be obtained from code-only work.

---

## Blocker 1: O12 — Asset Proof

```
Gate: O12 — Canary image proof
Exact command: N/A (no source images available)
Exit code: N/A
Error excerpt: No redistributable StS1 art exists in the repository. The events/ directory
  contains only Spire Plus-owned Ancient portraits (ezmb_lotha, ezmb_morvi, ezmb_urda).
  Zero StS1 event images exist.
Files touched: docs/features/sts1-events/asset-manifest.md (documents 0/48 images)
Why no safe workaround exists: Project rules forbid copying original StS1 non-art game assets
  into the repository without redistribution permission. No permission has been confirmed.
  Cannot create placeholder images without art assets.
Next owner/action: Obtain redistribution permission for StS1 event art, OR create
  original/replacement art for all 48 events, OR accept events will display without images.
What remains red: All 48 event image slots (0% coverage)
```

## Blocker 2: O15 — Canary Debug Spawn

```
Gate: O15 — Canary runtime screenshots/logs
Exact command: Set SPIREPLUS_STS1_EVENT_MODE=CanaryOnly, launch game, navigate to
  unknown room events, screenshot Big Fish / Golden Idol / Lab / Divine Fountain
Exit code: N/A (requires game launch)
Error excerpt: Cannot launch Slay the Spire 2 from code-only environment. Runtime
  verification requires actual game client with BaseLib v3.1.4 installed.
Files touched: None (no code changes needed, only runtime evidence)
Why no safe workaround exists: Event spawning depends on game's event pool system,
  room generation, RNG seeds, and UI rendering. These cannot be verified without
  actually running the game.
Next owner/action: Owner must launch game with SPIREPLUS_STS1_EVENT_MODE=CanaryOnly,
  play until encountering each canary event, screenshot all options and results.
What remains red: 4 canary events (Big Fish, Golden Idol, Lab, Divine Fountain)
  have no runtime spawn proof
```

## Blocker 3: O16 — Canary Save/Load

```
Gate: O16 — Canary save/load proof
Exact command: During a canary event, save game, reload, verify event state persists
Exit code: N/A (requires game launch)
Error excerpt: Save/load verification requires game client to save state to disk,
  reload, and verify event options/results are preserved.
Files touched: None (no code changes needed, only runtime evidence)
Why no safe workaround exists: Save/load behavior depends on game's serialization
  system, which cannot be tested without the game runtime.
Next owner/action: Owner must save during a canary event, reload, and verify the
  event state (selected options, granted rewards) persists correctly.
What remains red: 4 canary events have no save/load persistence proof
```

## Blocker 4: O19 — Replacement Functional Proof

```
Gate: O19 — ReplaceUnknownEventsPrototype functional proof
Exact command: Enable #define REPLACEMENT_PROTOTYPE_ENABLED, set
  SPIREPLUS_STS1_EVENT_MODE=ReplaceUnknownEventsPrototype, launch game,
  verify unknown rooms only draw StS1 events (not StS2 originals)
Exit code: N/A (requires game launch)
Error excerpt: The ReplaceUnknownEventsPrototype Harmony patch (Sts1ReplacementPrototype.cs)
  exists and is correctly gated behind #if REPLACEMENT_PROTOTYPE_ENABLED. It patches
  ActModel.GenerateRooms() to filter event pool to StS1 namespace only. But functional
  proof requires running the game and verifying that unknown rooms never spawn
  StS2-original events.
Files touched: EZMicroBalanceCode/Sts1Events/Runtime/Sts1ReplacementPrototype.cs
  (untracked, compile-gated)
Why no safe workaround exists: Event pool filtering depends on game's room generation
  system, event bag state, and RNG. Cannot be verified without running the game.
Next owner/action: Owner must enable REPLACEMENT_PROTOTYPE_ENABLED, run a full act,
  and verify all unknown rooms spawn StS1 events only. Document seeded run proof,
  act bucket proof, visited/no-repeat proof, and save/load bag proof.
What remains red: ReplaceUnknownEventsPrototype has no functional runtime proof
```

## Blocker 5: O23 — QA Red-Team

```
Gate: O23 — Independent QA pass/fail report
Exact command: Launch separate QA agent to independently verify O0-O22
Exit code: N/A (requires independent agent or human reviewer)
Error excerpt: Implementer cannot self-verify. QA/Red-Team must independently
  give pass/fail for each gate without being the same agent that wrote the code.
Files touched: None (QA agent would produce its own report)
Why no safe workaround exists: The overnight run spec explicitly requires independent
  verification. Self-assessment is not acceptable for release readiness.
Next owner/action: Launch a separate agent (or human reviewer) to independently
  audit all O0-O22 gates, verify no false Dones, and produce pass/fail report.
What remains red: No independent QA pass/fail exists
```

---

## What IS Green (20 gates)

| Gate | Name | Evidence |
|------|------|----------|
| O0 | Worktree snapshot | git status/HEAD/diff saved |
| O1 | Full build | 0 errors, 0 warnings (incremental) |
| O2 | Full tests | 366 passed, 0 failed, 21 skipped |
| O3 | Status truth | No false Done in status-board |
| O4 | Canonical matrix | 54-entry CSV + reconciliation doc |
| O5 | Act mapping | 3 guard tests pass |
| O6 | Feature gate | 3 guard tests pass (Off/CanaryOnly/Additive/Replacement) |
| O7 | Registration count | Guard tests: 17 shared, 54 total, 4 canary |
| O8 | IsShared matrix | 48 events documented with reasons |
| O9 | Combat IsShared | 6 combat events verified IsShared=true |
| O10 | ZHS placeholders | 0 `待翻译` found |
| O11 | Asset manifest | 48 events mapped (0 images) |
| O13 | Canary source/API | 4 canary events PASS |
| O14 | Canary implementation | 0 TODOs in 4 canary files |
| O17 | Simple batch specs | 6 events spec'd (4 existing + 2 new) |
| O18 | Simple batch code | 6/6 code-complete (Purifier + Golden Shrine created) |
| O20 | Content parity gaps | Curse/relic/card gap matrix complete |
| O21 | Combat blockers | 7+1 blocked events documented |
| O22 | Multiplayer guard | Fail-closed behavior code-verified |
| O24 | Handoff | This document |

## Conclusion

**All code-doable gates are GREEN.** The remaining 5 blockers all require either:
1. **Game launch** (O15, O16, O19) — cannot be done from CLI
2. **External resources** (O12) — no redistributable art available
3. **Independent review** (O23) — cannot self-verify

The codebase is ready only for the next runtime-verification attempt after current validation is recaptured. Source guards and tests are tracked in `docs/reviews/current-validation.md`; runtime gameplay, save/load, image/render, replacement-pool, multiplayer, and independent QA gates remain open until live evidence exists.
