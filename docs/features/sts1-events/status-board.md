# StS1 Events Status Board

> Last updated: 2026-06-11 (canary source/localization corrections, simple-batch source parity guards, AdditiveBatch1 spec inventory guard, and beta.85 Off loader proof)
> Audit standard: strict v18 - no generic "Done", only evidence-backed statuses

## Allowed Statuses

```
planned → spec-drafted → wiki-verified → api-verified → implemented → compiled → test-guarded → asset-mapped → loc-render-verified → manual-verified → save-load-verified
blocked | temporary-substitute | compile-excluded | special-stub | duplicate-wiki-entry
```

`historical-loader-verified` means the row has old enabled-mode loader-gate evidence only. Current beta.85 has clean `v0.107.0` Off loader proof, but no fresh current CanaryOnly/AdditiveBatch1 enabled-mode proof, gameplay, event rendering, save-load, image/license status, replacement-pool behavior, multiplayer disposition, or StS1 parity proof.

## Overall Summary

| Metric | Count | Evidence |
|--------|-------|----------|
| Public wiki baseline | 52 | `docs/goals/event.md` external unknown-room target |
| Canonical audit rows | 54 | 52 public baseline + 2 local special stubs in canonical-event-matrix.csv |
| Runtime registry entries | 50 | registry-reconciliation.md |
| Registration calls (RegisterAll) | 57 | Sts1EventRegistrationService.cs; Big Fish, Golden Idol, The Cleric, and Shining Light register to Overgrowth and Underdocks |
| Registration calls (AdditiveBatch1) | 14 / 10 event types | Sts1EventRegistrationService.cs; Big Fish, Golden Idol, The Cleric, and Shining Light register to Overgrowth and Underdocks |
| Shared event registrations | 14 | Sts1EventRegistrationService.cs (`RegisterAll` shared-event calls; Big Fish, Golden Idol, and The Cleric moved to Act 1 registration) |
| Model files (C#) | 48 | Models/ directory (1 compile-excluded: Duplicator) |
| Compiling models | 47 | dotnet build (1 compile-excluded) |
| EN localization keys | 397 | eng/sts1_events.json; The Lab unused LEAVE keys removed |
| ZHS localization keys | 397 (0 placeholder) | zhs/sts1_events.json verified; The Lab unused LEAVE keys removed |
| Event images | 0 | No redistributable art available |
| Guard tests | source-guarded | Sts1EventFeatureGuardTests.cs |
| Build | last validated 0 errors / 0 warnings | 2026-06-10 Revision L project build after expanded Sts1Events owner guards; prior 70 nullable warnings are cleared in current dirty source. June 11 StS1 event source/test/resource/doc changes have not been build-validated. |
| Tests | last validated 464 passed / 0 failed / 21 skipped (485 total) | Current test-project lane and exact solution-level `dotnet test EZMicroBalance.sln --no-build` passed after the cross-thread validation overlap was cleared; 21 skipped are `[ReleaseArtifactFact]`-gated. June 11 Divine Fountain, Big Fish, Golden Idol, The Lab, Old Beggar, Shining Light, Golden Shrine, The Cleric, and simple-batch spec inventory guards are newer than this run and have not been executed. |
| Format | passed | `dotnet format --verify-no-changes` clean |
| Diff check | passed | `git diff --check` clean (no CRLF warning) |

## Phase Status

| Phase | Events | Compiled | Blocked | Status |
|-------|--------|----------|---------|--------|
| Canary (4) | Big Fish, Golden Idol, The Lab, Divine Fountain | 4 | 0 | compiled, test-guarded, source/API verified, current beta.85 default-Off loader proof clean; current `v0.107.0` CanaryOnly enabled-mode proof, gameplay proof, and Act/parity audit closure are blocked/pending |
| AdditiveBatch1 verified scope (10 event types) | Big Fish, Golden Idol, The Lab, Divine Fountain, Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar, Shining Light | 10 | 0 | compiled, source-guarded, current beta.85 default-Off loader proof clean; current `v0.107.0` AdditiveBatch1 enabled-mode proof and per-event gameplay/render/save-load proof are blocked/pending |
| Simple (22 registry entries; 21 compiling) | Shining Light, Mushrooms, Joust, The Ssssserpent, Altar, Drug Dealer, The Library, Ancient Writing, Augmenter, Sensory Stone, Moai Head, Transmogrifier, Upgrade Shrine, The Cleric, Golden Wing, Living Wall, Old Beggar, Bonfire Spirits, Fountain of Cleansing, Purifier, Golden Shrine, Duplicator | 21 | 0 | compiled/source-guarded except Duplicator compile-excluded; Lab and Divine Fountain are canary metadata |
| CardService (9) | Face Trader, The Mausoleum, Council of Ghosts, Cursed Tome, Knowing Skull, Nest, Vampires, Falling, Mind Bloom | 9 | 0 | compiled (4 temporary-substitute plus Mind Bloom War partial) |
| Combat (5) | Dead Adventurer, Scorpion Nest, Treasure Ooze, Masked Bandits, Mysterious Sphere | 5 | 5 | blocked pending encounter-model/runtime parity proof |
| CustomUI (8) | The Woman in Blue, Wheel of Change, Designer, Forgotten Altar, The Ghost, N'loth, Tomb of Lord Red Mask, Winding Halls | 8 | 1 | compiled (1 blocked: N'loth) |
| Special (2) | Neow, Combat Start | 0 | 2 | special-stub (no unknown-room model) |

## Per-Event Status

### Canary Events (4)

| Event | Status | TODOs | IsShared | Parity Gap | Proof |
|-------|--------|-------|----------|------------|-------|
| Big Fish | implemented, compiled, test-guarded, source/API verified, historical-loader-verified | Current `v0.107.0` CanaryOnly enabled-mode reproof, encounter screenshot/result log, save-load, EN/ZHS render, image/license/render | true | Source registration now targets Act 1 buckets; runtime bucket proof remains pending. Source/localization now use wiki-aligned Box option identity; runtime UI proof remains pending | canary-source-api-proof.md, Sts1EventFeatureGuardTests.cs (Box guard added; not run), historical godot.log; beta.85 default-Off log only proves disabled-mode loading |
| Golden Idol | implemented, compiled, test-guarded, source/API verified, historical-loader-verified | Golden Idol relic parity decision, current `v0.107.0` CanaryOnly enabled-mode reproof, encounter screenshot/result log, save-load, EN/ZHS render, image/license/render | true | Source registration now targets Act 1 buckets; runtime bucket proof remains pending. Trap source/localization now use Outrun, Smash, and Hide branch identities; runtime UI/result proof remains pending. Take currently grants a random relic because no Golden Idol relic model is implemented | canary-source-api-proof.md, Sts1EventFeatureGuardTests.cs (Golden Idol trap guard added; not run), historical godot.log; beta.85 default-Off log only proves disabled-mode loading |
| The Lab | implemented, compiled, test-guarded, source/API verified, historical-loader-verified | Current `v0.107.0` CanaryOnly enabled-mode reproof, encounter screenshot/result log, save-load, EN/ZHS render, image/license/render | true | Source/localization now expose only Open and document 3 potions / 2 at A15+; runtime UI/result proof remains pending | canary-source-api-proof.md, Sts1EventFeatureGuardTests.cs (The Lab guard added; not run), historical godot.log; beta.85 default-Off log only proves disabled-mode loading |
| Divine Fountain | implemented, compiled, test-guarded, source/API verified, historical-loader-verified | Current `v0.107.0` CanaryOnly enabled-mode reproof, encounter screenshot/result log, save-load, EN/ZHS render, image/license/render | true | Curse prerequisite is now source-guarded through `IsAllowed(IRunState)` and the option identity is aligned to Drink; runtime selection/UI proof remains pending | canary-source-api-proof.md, Sts1EventFeatureGuardTests.cs, historical godot.log; beta.85 default-Off log only proves disabled-mode loading |

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

### Temporary Substitutes (6)

| Event | Substitute | Parity Gap |
|-------|------------|------------|
| Golden Idol | Random relic instead of Golden Idol relic | Golden Idol relic model/effect not implemented; trap branch names are source/localization aligned |
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

## Runtime Gates (historical v16 / `v0.106.1`; current beta.85 Off proof clean)

| Gate | Status | Blocker |
|------|--------|---------|
| Runtime path report (O21) | **current prerequisite pass** | E-drive game root, BaseLib v3.1.4, STS2-RitsuLib v0.4.16 with `lib\0.107.0`, and Spire Plus beta.85 package-parity install are present. |
| STS2-RitsuLib installed (O22) | **current prerequisite pass** | `STS2-RitsuLib` `v0.4.16` with `lib\0.107.0\STS2-RitsuLib.dll` is installed on E-drive. |
| Active `godot.log` generated (O23) | **current pass** | Current beta.85 Off smoke generated `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/godot.log.after-launch`. |
| Loader proof (O24) | **current Off pass / enabled modes pending** | Current beta.85 Off smoke on `v0.107.0` / RitsuLib `v0.4.16` reached main menu, reported `v0.1.0-private-beta.85`, applied 25/25 Spire Plus ModPatcher patches, and has a clean `godot-log-audit.json` with 0 blocking signature hits. |
| Default Off runtime state (O13) | **current pass** | Current beta.85 Off smoke logs `Feature Sts1Events ... bootstrap=disabled, live=Disabled`; no enabled StS1 event registrations are claimed from this Off run. |
| CanaryOnly exact 4 (O14) | **historical pass / current pending** | Historical CanaryOnly audit: exactly Sts1BigFish, Sts1GoldenIdol, Sts1TheLab, Sts1DivineFountain registered. Fresh beta.85 `v0.107.0` CanaryOnly smoke has not been captured in this thread. |
| AdditiveBatch1 exact 10/14 (O15 historical) | **historical pass / source changed / current pending** | Historical AdditiveBatch1 audit proved the older 10 event types via 11 registration calls. Current source now has 10 event types via 14 calls after Big Fish, Golden Idol, The Cleric, and Shining Light moved to Act 1 buckets. |
| AdditiveBatch1 exact 10/14 (O15 current) | **current pending** | Current `v0.107.0` AdditiveBatch1 proof still requires a fresh beta.85 enabled-mode smoke. |
| AdditiveAllDraft unsafe (O16) | **pass (source-guarded)** | Requires `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`; test-gated. |
| ReplacementPrototype fail-closed (O17/O18) | **pass (source-guarded)** | Requires `#if REPLACEMENT_PROTOTYPE_ENABLED` + unsafe override; test-gated. |
| Canary runtime launch (O25) | **historical pass / current pending** | CanaryOnly launch confirmed in historical godot.log only; current beta.85 Off proof is clean, but a fresh beta.85 CanaryOnly enabled-mode smoke has not been captured in this thread. |
| Canary event screenshots (O26-O29) | **blocked** | Requires in-game event encounter screenshots (Big Fish, Golden Idol, Lab, Divine Fountain). |
| Canary save/load proof (O30) | **blocked** | Requires save during/after event, reload, state stable. |
| Canary EN/ZHS render (O31) | **blocked** | Requires in-game EN/ZHS text render screenshots. |
| Canary image/license render (O32) | **blocked** | No redistributable art; requires extraction/placeholder decision. |
| AdditiveBatch1 runtime launch (O33) | **historical pass / current pending** | AdditiveBatch1 launch confirmed in historical godot.log; current beta.85 Off proof is clean, but a fresh beta.85 AdditiveBatch1 enabled-mode smoke has not been captured in this thread. |
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
- `AdditiveBatch1` now registers 10 event types through 14 registration calls because Big Fish, Golden Idol, The Cleric, and Shining Light are available in both StS2 Act 1 buckets: Overgrowth and Underdocks.
- Dedicated event-spec pages now exist for all six AdditiveBatch1 simple-batch events, including Purifier and Golden Shrine.
- Old Beggar Offer Gold is source-gated on 75+ gold so underfunded players cannot buy card removal through `PlayerCmd.LoseGold` clamping. Runtime UI/result proof remains pending with the rest of AdditiveBatch1.
- Shining Light now source-upgrades random upgradable deck cards with event RNG and no manual upgrade picker. Runtime result proof remains pending with the rest of AdditiveBatch1.
- Golden Shrine source/localization now use StS1-aligned Pray/Desecrate/Leave options: Pray grants 100 gold, 50 at A15+; Desecrate grants 275 gold and adds Regret. Runtime UI/result proof remains pending with the rest of AdditiveBatch1.
- The Cleric source/localization now guard the 35+ gold event eligibility, A15+ Purify cost increase from 50 to 75 gold, and Act 1 bucket registration. Runtime UI/result/bucket proof remains pending with the rest of AdditiveBatch1.
- Current beta.85 Off loader proof is clean at `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/`. This proves only the default-Off loader path; it does not prove CanaryOnly/AdditiveBatch1 enabled modes, event encounter gameplay, save-load, rendering, replacement, multiplayer, or QA gates.
- `AdditiveAllDraft` remains unsafe/dev-only and now requires `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` in addition to `SPIREPLUS_STS1_EVENT_MODE=AdditiveAllDraft`.
- `ReplaceUnknownEventsPrototype` remains debug-only and does not register events unless compiled with `REPLACEMENT_PROTOTYPE_ENABLED` and explicitly allowed with `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`.
- Joust and The Ssssserpent are source-classified as non-combat Act 1 events; encounter-model blockers apply only to actual combat-entry events.
- **Revision M correction**: Clean historical `v0.106.1` loader evidence exists for Off, CanaryOnly, and AdditiveBatch1 modes. The beta.84 `v0.107.0` package smoke at `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` remains root-cause evidence for stale Spire Plus API targets; the current beta.85 Off smoke is clean.
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
| **v19 beta.85 Off mode clean log** | `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/godot.log.after-launch` |
| **v19 beta.85 Off mode clean audit** | `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/godot-log-audit.json` |
| Warning budget | `docs/goals/warning-ledger.md` |
