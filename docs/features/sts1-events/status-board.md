# StS1 Events Status Board

> Last updated: 2026-06-10 (Revision L runtime-drift correction)
> Audit standard: strict v18 - no generic "Done", only evidence-backed statuses

## Allowed Statuses

```
planned → spec-drafted → wiki-verified → api-verified → implemented → compiled → test-guarded → asset-mapped → loc-render-verified → manual-verified → save-load-verified
blocked | temporary-substitute | compile-excluded | special-stub | duplicate-wiki-entry
```

`historical-loader-verified` means the row has old loader-gate evidence only. It does not prove current `v0.107.0` compatibility, gameplay, event rendering, save-load, image/license status, replacement-pool behavior, multiplayer disposition, or StS1 parity.

## Overall Summary

| Metric | Count | Evidence |
|--------|-------|----------|
| Public wiki baseline | 52 | `docs/goals/event.md` external unknown-room target |
| Canonical audit rows | 54 | 52 public baseline + 2 local special stubs in canonical-event-matrix.csv |
| Runtime registry entries | 50 | registry-reconciliation.md |
| Registration calls (RegisterAll) | 56 | Sts1EventRegistrationService.cs; Big Fish, Golden Idol, and Shining Light register to Overgrowth and Underdocks |
| Registration calls (AdditiveBatch1) | 13 / 10 event types | Sts1EventRegistrationService.cs; Big Fish, Golden Idol, and Shining Light register to Overgrowth and Underdocks |
| Shared event registrations | 15 | Sts1EventRegistrationService.cs (`RegisterAll` shared-event calls; Big Fish and Golden Idol moved to Act 1 registration) |
| Model files (C#) | 48 | Models/ directory (1 compile-excluded: Duplicator) |
| Compiling models | 47 | dotnet build (1 compile-excluded) |
| EN localization keys | 399 | eng/sts1_events.json |
| ZHS localization keys | 399 (0 placeholder) | zhs/sts1_events.json verified |
| Event images | 0 | No redistributable art available |
| Guard tests | source-guarded | Sts1EventFeatureGuardTests.cs |
| Build | 0 errors / 0 warnings | 2026-06-10 Revision L project build after expanded Sts1Events owner guards; prior 70 nullable warnings are cleared in current dirty source |
| Tests | 464 passed / 0 failed / 21 skipped (485 total) | Current test-project lane and exact solution-level `dotnet test EZMicroBalance.sln --no-build` passed after the cross-thread validation overlap was cleared; 21 skipped are `[ReleaseArtifactFact]`-gated |
| Format | passed | `dotnet format --verify-no-changes` clean |
| Diff check | passed | `git diff --check` clean (no CRLF warning) |

## Phase Status

| Phase | Events | Compiled | Blocked | Status |
|-------|--------|----------|---------|--------|
| Canary (4) | Big Fish, Golden Idol, The Lab, Divine Fountain | 4 | 0 | compiled, test-guarded, source/API verified, historical `v0.106.1` loader-gate proof only; current `v0.107.0` loader reproof, gameplay proof, and Act/parity audit closure are blocked/pending |
| AdditiveBatch1 verified scope (10 event types) | Big Fish, Golden Idol, The Lab, Divine Fountain, Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar, Shining Light | 10 | 0 | compiled, source-guarded, historical `v0.106.1` loader-gate proof only; current `v0.107.0` loader reproof and per-event gameplay/render/save-load proof are blocked/pending |
| Simple (22 registry entries; 21 compiling) | Shining Light, Mushrooms, Joust, The Ssssserpent, Altar, Drug Dealer, The Library, Ancient Writing, Augmenter, Sensory Stone, Moai Head, Transmogrifier, Upgrade Shrine, The Cleric, Golden Wing, Living Wall, Old Beggar, Bonfire Spirits, Fountain of Cleansing, Purifier, Golden Shrine, Duplicator | 21 | 0 | compiled/source-guarded except Duplicator compile-excluded; Lab and Divine Fountain are canary metadata |
| CardService (9) | Face Trader, The Mausoleum, Council of Ghosts, Cursed Tome, Knowing Skull, Nest, Vampires, Falling, Mind Bloom | 9 | 0 | compiled (4 temporary-substitute plus Mind Bloom War partial) |
| Combat (5) | Dead Adventurer, Scorpion Nest, Treasure Ooze, Masked Bandits, Mysterious Sphere | 5 | 5 | blocked pending encounter-model/runtime parity proof |
| CustomUI (8) | The Woman in Blue, Wheel of Change, Designer, Forgotten Altar, The Ghost, N'loth, Tomb of Lord Red Mask, Winding Halls | 8 | 1 | compiled (1 blocked: N'loth) |
| Special (2) | Neow, Combat Start | 0 | 2 | special-stub (no unknown-room model) |

## Per-Event Status

### Canary Events (4)

| Event | Status | TODOs | IsShared | Parity Gap | Proof |
|-------|--------|-------|----------|------------|-------|
| Big Fish | implemented, compiled, test-guarded, source/API verified, historical-loader-verified | Current `v0.107.0` loader reproof, encounter screenshot/result log, save-load, EN/ZHS render, image/license/render | true | Source registration now targets Act 1 buckets; runtime bucket proof remains pending. Option key "Shoe" vs wiki "Box" remains cosmetic/non-parity text | canary-source-api-proof.md, historical godot.log |
| Golden Idol | implemented, compiled, test-guarded, source/API verified, historical-loader-verified | Golden Idol relic parity decision, current `v0.107.0` loader reproof, encounter screenshot/result log, save-load, EN/ZHS render, image/license/render | true | Source registration now targets Act 1 buckets; runtime bucket proof remains pending. Take currently grants a random relic because no Golden Idol relic model is implemented; trap option labels differ from wiki names | canary-source-api-proof.md, historical godot.log |
| The Lab | compiled, test-guarded, source/API verified, historical-loader-verified | Current `v0.107.0` loader reproof, encounter screenshot/result log, save-load, EN/ZHS render, image/license/render | true | Extra Leave option not in wiki; A15/no-drawback parity remains unproven in live event flow | canary-source-api-proof.md, historical godot.log |
| Divine Fountain | compiled, test-guarded, source/API verified, historical-loader-verified | Curse-prerequisite parity decision, current `v0.107.0` loader reproof, encounter screenshot/result log, save-load, EN/ZHS render, image/license/render | true | Option key "Pray" vs wiki "Drink"; curse prerequisite is not checked in the model | canary-source-api-proof.md, historical godot.log |

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

## Runtime Gates (historical v16 / `v0.106.1`; current `v0.107.0` proof blocked)

| Gate | Status | Blocker |
|------|--------|---------|
| Runtime path report (O21) | **current prerequisite pass** | E-drive game root, BaseLib v3.1.4, STS2-RitsuLib v0.4.16 with `lib\0.107.0`, and Spire Plus beta.84 package-parity install are present. |
| STS2-RitsuLib installed (O22) | **current prerequisite pass** | `STS2-RitsuLib` `v0.4.16` with `lib\0.107.0\STS2-RitsuLib.dll` is installed on E-drive. |
| Active `godot.log` generated (O23) | **current fail** | Current Off smoke exists at `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/`, but its audit is non-clean. |
| Loader proof (O24) | **current fail** | Current Off smoke on `v0.107.0` / RitsuLib `v0.4.16` fails audit: 11 Godot ERROR lines, 1 Spire Plus error/exception, and `EctoplasmGoldGatePatch::Prefix(...)` undefined target method in beta.84. |
| Default Off runtime state (O13) | **historical pass / current fail** | Historical Off-mode audit: 0 StS1 registration lines. Current Off smoke is non-clean before it can count as default-Off proof. |
| CanaryOnly exact 4 (O14) | **historical pass / current blocked** | Historical CanaryOnly audit: exactly Sts1BigFish, Sts1GoldenIdol, Sts1TheLab, Sts1DivineFountain registered. |
| AdditiveBatch1 exact 10/13 (O15 historical) | **historical pass / source changed / current blocked** | Historical AdditiveBatch1 audit proved the older 10 event types via 11 registration calls. Current source now has 10 event types via 13 calls after Big Fish and Golden Idol moved to Act 1 buckets; current proof must wait until the non-clean `v0.107.0` Off smoke is fixed and rerun clean. |
| AdditiveBatch1 exact 10/13 (O15 current) | **current blocked** | Current `v0.107.0` AdditiveBatch1 proof must wait until Off loader proof is clean with a fixed package. |
| AdditiveAllDraft unsafe (O16) | **pass (source-guarded)** | Requires `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`; test-gated. |
| ReplacementPrototype fail-closed (O17/O18) | **pass (source-guarded)** | Requires `#if REPLACEMENT_PROTOTYPE_ENABLED` + unsafe override; test-gated. |
| Canary runtime launch (O25) | **historical pass / current blocked** | CanaryOnly launch confirmed in historical godot.log only; current Off smoke is red, so CanaryOnly was not attempted as proof. |
| Canary event screenshots (O26-O29) | **blocked** | Requires in-game event encounter screenshots (Big Fish, Golden Idol, Lab, Divine Fountain). |
| Canary save/load proof (O30) | **blocked** | Requires save during/after event, reload, state stable. |
| Canary EN/ZHS render (O31) | **blocked** | Requires in-game EN/ZHS text render screenshots. |
| Canary image/license render (O32) | **blocked** | No redistributable art; requires extraction/placeholder decision. |
| AdditiveBatch1 runtime launch (O33) | **historical pass / current blocked** | AdditiveBatch1 launch confirmed in historical godot.log; current Off smoke is red, so AdditiveBatch1 was not attempted as proof. |
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
- `AdditiveBatch1` now registers 10 event types through 13 registration calls because Big Fish, Golden Idol, and Shining Light are available in both StS2 Act 1 buckets: Overgrowth and Underdocks.
- Any O13-O25/O33 `pass` language in this file is historical `v0.106.1` loader-gate evidence only. Current `v0.107.0` Off loader proof is red for beta.84 and must be rerun only after a fixed package is installed.
- `AdditiveAllDraft` remains unsafe/dev-only and now requires `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` in addition to `SPIREPLUS_STS1_EVENT_MODE=AdditiveAllDraft`.
- `ReplaceUnknownEventsPrototype` remains debug-only and does not register events unless compiled with `REPLACEMENT_PROTOTYPE_ENABLED` and explicitly allowed with `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`.
- Joust and The Ssssserpent are source-classified as non-combat Act 1 events; encounter-model blockers apply only to actual combat-entry events.
- **Revision L correction**: Clean historical `v0.106.1` loader evidence exists for Off, CanaryOnly, and AdditiveBatch1 modes. Current `v0.107.0` beta.84 package smoke at `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` is non-clean because Spire Plus initialization still targets stale game APIs in the packaged DLL.
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
| **v18 current Off package-parity log (non-clean)** | `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/godot.log.after-launch` |
| **v18 current Off package-parity audit (non-clean)** | `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/godot-log-audit.json` |
| **v18 current loader hard stop** | `docs/features/sts1-events/hard-stop-blocker-report-v18-current-loader-20260610.md` |
| Warning budget | `docs/goals/warning-ledger.md` |
