# StS1 Events Status Board

> Last updated: 2026-06-02 (v16 runtime-verification pass)
> Audit standard: strict v15 - no generic "Done", only evidence-backed statuses

## Allowed Statuses

```
planned → spec-drafted → wiki-verified → api-verified → implemented → compiled → test-guarded → asset-mapped → loc-render-verified → manual-verified → save-load-verified
blocked | temporary-substitute | compile-excluded | special-stub | duplicate-wiki-entry
```

## Overall Summary

| Metric | Count | Evidence |
|--------|-------|----------|
| Public wiki baseline | 52 | `docs/goals/event.md` external unknown-room target |
| Canonical audit rows | 54 | 52 public baseline + 2 local special stubs in canonical-event-matrix.csv |
| Runtime registry entries | 50 | registry-reconciliation.md |
| Registration calls (RegisterAll) | 54 | Sts1EventRegistrationService.cs |
| Registration calls (AdditiveBatch1) | 11 / 10 event types | Sts1EventRegistrationService.cs; Shining Light registers to Overgrowth and Underdocks |
| Shared event registrations | 17 | Sts1EventRegistrationService.cs (RegisterGated path) |
| Model files (C#) | 48 | Models/ directory (1 compile-excluded: Duplicator) |
| Compiling models | 47 | dotnet build (1 compile-excluded) |
| EN localization keys | 399 | eng/sts1_events.json |
| ZHS localization keys | 399 (0 placeholder) | zhs/sts1_events.json verified |
| Event images | 0 | No redistributable art available |
| Guard tests | source-guarded | Sts1EventFeatureGuardTests.cs |
| Build | 0 errors / 79 warnings | 2026-06-02 v16 solution build; warnings are Sts1Events nullable staging warnings (budget documented in `docs/goals/warning-ledger.md`); reduced from 89 by Golden Idol parity fixes |
| Tests | 464 passed / 0 failed / 21 skipped (485 total) | 2026-06-02 v16 no-build validation with single VSTest worker; 21 skipped are `[ReleaseArtifactFact]`-gated (require `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1`) |
| Format | passed | `dotnet format --verify-no-changes` clean |
| Diff check | passed | `git diff --check` clean (no CRLF warning) |

## Phase Status

| Phase | Events | Compiled | Blocked | Status |
|-------|--------|----------|---------|--------|
| Canary (4) | Big Fish, Golden Idol, The Lab, Divine Fountain | 4 | 0 | compiled, test-guarded, source/API verified, **runtime-verified** (clean audit: 0 Godot ERROR, exact 4 registrations confirmed in godot.log) |
| AdditiveBatch1 verified scope (10 event types) | Big Fish, Golden Idol, The Lab, Divine Fountain, Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar, Shining Light | 10 | 0 | compiled, source-guarded, **runtime-verified** (clean audit: 0 Godot ERROR, exact 10 event types / 11 registration calls confirmed in godot.log) |
| Simple (22 registry entries; 21 compiling) | Shining Light, Mushrooms, Joust, The Ssssserpent, Altar, Drug Dealer, The Library, Ancient Writing, Augmenter, Sensory Stone, Moai Head, Transmogrifier, Upgrade Shrine, The Cleric, Golden Wing, Living Wall, Old Beggar, Bonfire Spirits, Fountain of Cleansing, Purifier, Golden Shrine, Duplicator | 21 | 0 | compiled/source-guarded except Duplicator compile-excluded; Lab and Divine Fountain are canary metadata |
| CardService (9) | Face Trader, The Mausoleum, Council of Ghosts, Cursed Tome, Knowing Skull, Nest, Vampires, Falling, Mind Bloom | 9 | 0 | compiled (4 temporary-substitute plus Mind Bloom War partial) |
| Combat (5) | Dead Adventurer, Scorpion Nest, Treasure Ooze, Masked Bandits, Mysterious Sphere | 5 | 5 | blocked pending encounter-model/runtime parity proof |
| CustomUI (8) | The Woman in Blue, Wheel of Change, Designer, Forgotten Altar, The Ghost, N'loth, Tomb of Lord Red Mask, Winding Halls | 8 | 1 | compiled (1 blocked: N'loth) |
| Special (2) | Neow, Combat Start | 0 | 2 | special-stub (no unknown-room model) |

## Per-Event Status

### Canary Events (4)

| Event | Status | TODOs | IsShared | Parity Gap | Proof |
|-------|--------|-------|----------|------------|-------|
| Big Fish | compiled, test-guarded, source/API verified, runtime-verified | none | true | 1 minor: option key "Shoe" vs wiki "Box" (cosmetic naming) | canary-source-api-proof.md, godot.log |
| Golden Idol | compiled, test-guarded, source/API verified, runtime-verified | none | true | 2 remaining: option keys "SMASH/JUMP/DESTROY" vs wiki "Outrun/Smash/Hide" (cosmetic naming); no Golden Idol relic in StS2 (grants random relic instead) | canary-source-api-proof.md, godot.log |
| The Lab | compiled, test-guarded, source/API verified, runtime-verified | none | true | 1 minor: extra Leave option not in wiki (StS2 UX improvement) | canary-source-api-proof.md, godot.log |
| Divine Fountain | compiled, test-guarded, source/API verified, runtime-verified | none | true | 2 minor: option key "Pray" vs wiki "Drink" (cosmetic); curse prerequisite not checked in model | canary-source-api-proof.md, godot.log |

### Blocked / Partial Events (7 rows)

| Event | Status | Blocker |
|-------|--------|---------|
| Dead Adventurer | blocked | Missing encounter model (random elite) |
| Scorpion Nest | blocked | Missing encounter model (3 Louses) |
| Treasure Ooze | blocked | Missing encounter model (large slime) |
| Masked Bandits | blocked | Missing encounter model (3 bandits) |
| Mysterious Sphere | blocked | Missing encounter model (2 Orb Walkers) |
| Mind Bloom (War option) | temporary-substitute | War blocked; Awake/Rich implemented |
| N'loth | blocked | No RelicSelectCmd API in StS2 |

### Native-Equivalent Act 1 Non-Combat Events (2)

| Event | Status | Notes |
|-------|--------|-------|
| Joust | compiled | Gold-bet event; no combat branch in current source. |
| The Ssssserpent | compiled | Gold+curse trade; no combat branch in current source. |

### Temporary Substitutes (5)

| Event | Substitute | Parity Gap |
|-------|------------|------------|
| Face Trader | Random relic instead of face relics | Face relic models don't exist in StS2 |
| Nest | Clumsy curse instead of Parasite | Parasite curse doesn't exist in StS2 |
| Vampires | Removes Strikes but cannot add Bite | Bite card doesn't exist in StS2 |
| Mind Bloom | War option blocked; Awake/Rich implemented | Random Act 1 Boss encounter model not proven |
| Winding Halls | Debt curse instead of Madness | Madness curse doesn't exist in StS2 |

### Compile-Excluded (1)

| Event | Reason |
|-------|--------|
| Duplicator | CardSelectCmd.FromDeckForRewards and CardSelectorPrefs.DuplicateSelectionPrompt don't exist in RitsuLib 0.3.2 |

### Special Stubs (2)

| Event | Reason |
|-------|--------|
| Neow | Start-of-run only; handled by base game Neow class |
| Combat Start | Tutorial flow; no unknown-room model needed |

## Runtime Gates (v16)

| Gate | Status | Blocker |
|------|--------|---------|
| Runtime path report (O21) | **pass** | E-drive game root, BaseLib v3.1.4, STS2-RitsuLib v0.3.10, and Spire Plus paths verified. |
| STS2-RitsuLib installed (O22) | **pass** | `STS2-RitsuLib` `v0.3.10` with `lib\0.106.1\STS2-RitsuLib.dll` installed on E-drive. |
| Active `godot.log` generated (O23) | **pass** | Clean logs exist for Off, CanaryOnly, and AdditiveBatch1 modes. |
| Loader proof (O24) | **pass** | 3 clean audits: Off mode (0 Godot ERROR), CanaryOnly mode (0 Godot ERROR, 3 mods loaded, 25/25 patches), AdditiveBatch1 mode (0 Godot ERROR, 3 mods loaded, 25/25 patches). The `[ERROR] ritsulib-variants.json` line is a RitsuLib internal variant-manifest issue, not a Spire Plus or Godot engine error; audit tool counts `^ERROR:` (Godot native) not `[ERROR]` (C# logger). |
| Default Off runtime state (O13) | **pass** | Clean Off-mode audit: 0 StS1 registration lines, Sts1Events disabled/default Off confirmed in godot.log. |
| CanaryOnly exact 4 (O14) | **pass** | Clean CanaryOnly audit: exactly Sts1BigFish, Sts1GoldenIdol, Sts1TheLab, Sts1DivineFountain registered. |
| AdditiveBatch1 exact 10/11 (O15) | **pass** | Clean AdditiveBatch1 audit: exactly 10 event types via 11 registration calls (Shining Light → Overgrowth + Underdocks). |
| AdditiveAllDraft unsafe (O16) | **pass (source-guarded)** | Requires `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`; test-gated. |
| ReplacementPrototype fail-closed (O17/O18) | **pass (source-guarded)** | Requires `#if REPLACEMENT_PROTOTYPE_ENABLED` + unsafe override; test-gated. |
| Canary runtime launch (O25) | **pass** | CanaryOnly launch confirmed in godot.log. |
| Canary event screenshots (O26-O29) | **blocked** | Requires in-game event encounter screenshots (Big Fish, Golden Idol, Lab, Divine Fountain). |
| Canary save/load proof (O30) | **blocked** | Requires save during/after event, reload, state stable. |
| Canary EN/ZHS render (O31) | **blocked** | Requires in-game EN/ZHS text render screenshots. |
| Canary image/license render (O32) | **blocked** | No redistributable art; requires extraction/placeholder decision. |
| AdditiveBatch1 runtime launch (O33) | **pass** | AdditiveBatch1 launch confirmed in godot.log; exact 10 event types / 11 calls. |
| Simple batch event proofs (O34-O39) | **blocked** | Requires per-event in-game encounter screenshots and result logs. |
| Simple batch save/load (O40) | **blocked** | Requires save/load proof for simple batch events. |
| Simple batch EN/ZHS render (O41) | **blocked** | Requires in-game EN/ZHS text render screenshots. |
| Simple batch image/license render (O42) | **blocked** | No redistributable art; requires extraction/placeholder decision. |
| Replacement functional proof (O43-O46) | **blocked** | Requires debug symbol, explicit unsafe env gate, game launch, seeded unknown-room replacement proof. |
| Multiplayer fail-closed (O47) | **blocked** | Requires multiplayer session or runtime fail-closed proof. |
| QA Red-Team (O51/O52) | **blocked** | Independent QA cannot pass while event encounter, save/load, image, replacement, and multiplayer gates remain blocked. |

## Current Gate Alignment Notes

- `CanaryOnly` remains exactly Big Fish, Golden Idol, The Lab, and Divine Fountain.
- `AdditiveBatch1` is now a separate mode and must not be described as `AdditiveAllDraft`.
- `AdditiveBatch1` registers 10 event types through 11 registration calls because Shining Light is available in both StS2 Act 1 buckets: Overgrowth and Underdocks.
- `AdditiveAllDraft` remains unsafe/dev-only and now requires `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` in addition to `SPIREPLUS_STS1_EVENT_MODE=AdditiveAllDraft`.
- `ReplaceUnknownEventsPrototype` remains debug-only and does not register events unless compiled with `REPLACEMENT_PROTOTYPE_ENABLED` and explicitly allowed with `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`.
- Joust and The Ssssserpent are source-classified as non-combat Act 1 events; encounter-model blockers apply only to actual combat-entry events.
- **v16 update**: Clean runtime evidence now exists for Off, CanaryOnly, and AdditiveBatch1 modes. The v15 loader audit (11 Godot ERROR hits) is superseded by the v16 clean audits (0 Godot ERROR for all three modes). The `[ERROR] ritsulib-variants.json` line is a RitsuLib internal variant-manifest issue logged by the C# logger, not a Godot engine error; the audit tool correctly counts only `^ERROR:` lines (Godot native format) and reports 0.
- Remaining blocked gates: event encounter screenshots (O26-O29, O34-O39), save/load proof (O30, O40), EN/ZHS render (O31, O41), image/license (O32, O42), replacement functional proof (O43-O46), multiplayer fail-closed (O47), and independent QA (O51/O52).

## Evidence Files

| Evidence | Path |
|----------|------|
| Build log | .tools/runtime-evidence/sts1-events-v13/o1-build-full.log |
| Test log | .tools/runtime-evidence/sts1-events-v13/o2-test-full.log |
| Test count/skips | .tools/runtime-evidence/sts1-events-v13/o3-o4-test-count-and-skips.md |
| Git snapshot | .tools/runtime-evidence/sts1-events-v13/o0-*.txt |
| Canonical matrix | docs/features/sts1-events/canonical-event-matrix.csv |
| Registry reconciliation | docs/features/sts1-events/registry-reconciliation.md |
| IsShared matrix | docs/features/sts1-events/multiplayer-is-shared-matrix.md |
| Content parity gaps | docs/features/sts1-events/content-parity-gaps.md |
| Canary source/API proof | docs/features/sts1-events/canary-source-api-proof.md |
| Combat blockers report | docs/features/sts1-events/combat-blockers-report.md |
| v14 hard stop report | docs/features/sts1-events/hard-stop-blocker-report-v14.md |
| v15 hard stop report | docs/features/sts1-events/hard-stop-blocker-report-v15.md |
| v15 loader log (historical) | .tools/runtime-evidence/sts1-events-v15-loader-20260531-231135/godot.log.after-launch |
| **v16 Off mode clean log** | `.tools/runtime-evidence/ritsulib-off-after-target-fix-20260531-2325/godot.log.after-launch` |
| **v16 Off mode clean audit** | `.tools/runtime-evidence/ritsulib-off-after-target-fix-20260531-2325/godot-log-audit.json` |
| **v16 CanaryOnly clean log** | `.tools/runtime-evidence/ritsulib-canary-after-target-fix-20260531-2327/godot.log.after-direct-launch` |
| **v16 CanaryOnly clean audit** | `.tools/runtime-evidence/ritsulib-canary-after-target-fix-20260531-2327/godot-log-audit.json` |
| **v16 AdditiveBatch1 clean log** | `.tools/runtime-evidence/additive-batch1-20260602-150445/godot.log.after-launch` |
| **v16 AdditiveBatch1 clean audit** | `.tools/runtime-evidence/additive-batch1-20260602-150445/godot-log-audit.json` |
| Warning budget | `docs/goals/warning-ledger.md` |
