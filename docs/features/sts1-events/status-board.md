# StS1 Events Status Board

> Last updated: 2026-06-21 (previous beta.93 `v0.107.1` RitsuLib-only Off and AdditiveBatch1 verifier packets captured and passed for loader/registration only; latest CanaryOnly-mode packet remains beta.85 previous-package context; gameplay and release proof remain pending)
> Audit standard: strict v19 - no generic "Done", only evidence-backed statuses

## Allowed Statuses

```
planned -> spec-drafted -> wiki-verified -> api-verified -> implemented -> compiled -> test-guarded -> asset-mapped -> loc-render-verified -> manual-verified -> save-load-verified
blocked | temporary-substitute | compile-excluded | special-stub | duplicate-wiki-entry
```

`historical-loader-verified` means the row has old enabled-mode loader-gate evidence only. Retained beta.87 has clean `v0.107.0` AdditiveBatch1 direct loader proof with 10 event types / 14 registration calls and exact tuple parity. Previous beta.88 has clean previous package `v0.107.1` AdditiveBatch1 direct loader/registration proof. Previous beta.93 has clean RitsuLib-only `v0.107.1` Off and AdditiveBatch1 direct loader/registration proof with STS2-RitsuLib `v0.4.31`, 25/25 Spire Plus patches applied, 10 event types / 14 registration calls, and exact tuple parity. Beta.85 Off and CanaryOnly packets remain previous-package loader context. Gameplay, event rendering, save-load, image/license status, replacement-pool behavior, multiplayer disposition, and StS1 parity proof remain pending.

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
| EN localization keys | 397 | eng/sts1_events.json; file-parity only. Static source scan found 33 source-referenced keys missing in both EN and ZHS. |
| ZHS localization keys | 397 | zhs/sts1_events.json; file-parity only. Static source scan found 33 source-referenced keys missing in both EN and ZHS. |
| Event images | 0 | No redistributable art available |
| Guard tests | source-guarded | Sts1EventFeatureGuardTests.cs |
| Build | beta.93 validated 0 errors / 0 warnings | `PROJECT_STATE.md` and `docs/dev-environment.md` record the beta.93 RitsuLib-only build as 0 errors / 0 warnings. This remains no-game build validation, not event gameplay proof. |
| Tests | beta.93 current guard/focused lanes passed; retained split lane passed | `PROJECT_STATE.md` and `docs/reviews/current-validation.md` record the trusted split test strategy, migration-focused guard totals, current package/artifact validation status, and the latest runtime packet-checker recapture. This proves automated guard/package coverage only, not enabled-mode or gameplay proof. |
| Format | beta.93 post-migration format passed | `PROJECT_STATE.md` and `docs/reviews/current-validation.md` record format/diff-check/patch-inventory/batch-classifier checks passing after the beta.93 RitsuLib-only migration validation and current-doc alignment. This remains no-game validation and should be recaptured after future code, resource, package, or handoff changes. |
| Diff check | static-pass | `git diff --check --` exits clean; current pause-safe reruns emit CRLF normalization warnings only for existing tracked files and no whitespace errors. |
| Pause-safe static v19 guards | 2026-06-15 static pass | `docs/reviews/current-validation.md` records `check-sts1-event-static-suite.ps1` 14 static steps / 0 suite failures with the known 33-key localization gap, then-current `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` 872 / 0, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` 11 / 0, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` 531 / 0, and `git diff --check --` exit 0 with CRLF warnings only. This is no-launch/static evidence only. |
| Pause-safe subagent packet checklist | 2026-06-15 static pass | `docs/features/sts1-events/v19-subagent-coverage.md` now records future post-pause packet requirements for CanaryOnly, AdditiveBatch1, gameplay, localization/resource, replacement, multiplayer, QA, and release-doc role owners; `scripts/check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 63 / 0, the aggregate static suite stayed 14 / 0, current-doc claims returned 872 / 0, static-file hygiene returned 11 / 0, v19 gate ledger returned 531 / 0, and `git diff --check --` exited 0 with CRLF warnings only. This is no-launch/static evidence only. |

## Phase Status

| Phase | Events | Compiled | Blocked | Status |
|-------|--------|----------|---------|--------|
| Canary (4) | Big Fish, Golden Idol, The Lab, Divine Fountain | 4 | 0 | compiled, test-guarded, source/API verified, retained beta.85 CanaryOnly enabled-mode proof clean as previous-package context; gameplay proof and Act/parity audit closure are blocked/pending |
| AdditiveBatch1 verified scope (10 event types) | Big Fish, Golden Idol, The Lab, Divine Fountain, Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar, Shining Light | 10 | 0 | compiled, source-guarded, previous beta.93 RitsuLib-only direct AdditiveBatch1 enabled-mode packet passed 10 event types / 14 registration calls on `v0.107.1`; per-event gameplay/render/save-load proof remains blocked/pending |
| Simple (22 registry entries; 21 compiling) | Shining Light, Mushrooms, Joust, The Ssssserpent, Altar, Drug Dealer, The Library, Ancient Writing, Augmenter, Sensory Stone, Moai Head, Transmogrifier, Upgrade Shrine, The Cleric, Golden Wing, Living Wall, Old Beggar, Bonfire Spirits, Fountain of Cleansing, Purifier, Golden Shrine, Duplicator | 21 | 0 | compiled/source-guarded except Duplicator compile-excluded; Lab and Divine Fountain are canary metadata |
| CardService (9) | Face Trader, The Mausoleum, Council of Ghosts, Cursed Tome, Knowing Skull, Nest, Vampires, Falling, Mind Bloom | 9 | 0 | compiled (4 temporary-substitute plus Mind Bloom War partial) |
| Combat (5) | Dead Adventurer, Scorpion Nest, Treasure Ooze, Masked Bandits, Mysterious Sphere | 5 | 5 | blocked pending encounter-model/runtime parity proof |
| CustomUI (8) | The Woman in Blue, Wheel of Change, Designer, Forgotten Altar, The Ghost, N'loth, Tomb of Lord Red Mask, Winding Halls | 8 | 1 | compiled (1 blocked: N'loth) |
| Special (2) | Neow, Combat Start | 0 | 2 | special-stub (no unknown-room model) |

## Per-Event Status

### Canary Events (4)

| Event | Status | TODOs | IsShared | Parity Gap | Proof |
|-------|--------|-------|----------|------------|-------|
| Big Fish | implemented, compiled, test-guarded, source/API verified, loader-verified | Encounter screenshot/result log, save-load, EN/ZHS render, image/license/render | true | Source registration targets Act 1 buckets; retained beta.85 CanaryOnly proof and previous beta.93 AdditiveBatch1 proof cover loader registration only, while event-pool/UI proof remains pending. Source/localization now use wiki-aligned Box option identity; runtime UI proof remains pending | canary-source-api-proof.md, Sts1EventFeatureGuardTests.cs, beta.85 CanaryOnly verifier packet; beta.93 AdditiveBatch1 verifier packet |
| Golden Idol | implemented, compiled, test-guarded, source/API verified, loader-verified | Golden Idol relic parity decision, encounter screenshot/result log, save-load, EN/ZHS render, image/license/render | true | Source registration targets Act 1 buckets; retained beta.85 CanaryOnly proof and previous beta.93 AdditiveBatch1 proof cover loader registration only, while event-pool/UI proof remains pending. Trap source/localization now use Outrun, Smash, and Hide branch identities; runtime UI/result proof remains pending. Take currently grants a random relic because no Golden Idol relic model is implemented | canary-source-api-proof.md, Sts1EventFeatureGuardTests.cs, beta.85 CanaryOnly verifier packet; beta.93 AdditiveBatch1 verifier packet |
| The Lab | implemented, compiled, test-guarded, source/API verified, loader-verified | Encounter screenshot/result log, save-load, EN/ZHS render, image/license/render | true | Retained beta.85 CanaryOnly proof and previous beta.93 AdditiveBatch1 proof cover loader registration only. Source/localization now expose only Open and document 3 potions / 2 at A15+; runtime UI/result proof remains pending | canary-source-api-proof.md, Sts1EventFeatureGuardTests.cs, beta.85 CanaryOnly verifier packet; beta.93 AdditiveBatch1 verifier packet |
| Divine Fountain | implemented, compiled, test-guarded, source/API verified, loader-verified | Encounter screenshot/result log, save-load, EN/ZHS render, image/license/render | true | Retained beta.85 CanaryOnly proof and previous beta.93 AdditiveBatch1 proof cover loader registration only. Curse prerequisite is now source-guarded through `IsAllowed(IRunState)` and the option identity is aligned to Drink; runtime selection/UI proof remains pending | canary-source-api-proof.md, Sts1EventFeatureGuardTests.cs, beta.85 CanaryOnly verifier packet; beta.93 AdditiveBatch1 verifier packet |

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
| Duplicator | CardSelectCmd.FromDeckForRewards and CardSelectorPrefs.DuplicateSelectionPrompt are unavailable in the current game/RitsuLib API surface |

### Special Stubs (2)

| Event | Reason |
|-------|--------|
| Neow | Start-of-run only; handled by base game Neow class |
| Combat Start | Tutorial flow; no unknown-room model needed |

## Runtime Gates (historical v16 / `v0.106.1`; retained `v0.107.0` beta.87 AdditiveBatch1 proof clean; current `v0.107.1` beta.93 RitsuLib-only loader clean)

| Gate | Status | Blocker |
|------|--------|---------|
| Runtime path report (O21) | **current prerequisite pass** | E-drive game root, STS2-RitsuLib v0.4.34 direct NuGet runtime layout, and Spire Plus beta.99 package-parity install are present. previous package is previous-package/other-mod context only. |
| STS2-RitsuLib installed (O22) | **current prerequisite pass** | `STS2-RitsuLib` `v0.4.34` direct NuGet runtime layout is installed on E-drive. |
| Active `godot.log` generated (O23) | **current packet exists** | Beta.93 AdditiveBatch1 direct smoke generated `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/godot.log.current-iteration` on `v0.107.1` and audited clean. Retained beta.87 `v0.107.0` proof and beta.87 `v0.107.1` previous package `v3.2.1` failure remain historical context. |
| Loader proof (O24) | **previous pass for loader/registration** | Beta.93 AdditiveBatch1 direct smoke on `v0.107.1` / RitsuLib `v0.4.31` reached main menu, reported `v0.1.0-private-beta.93`, applied 25/34 Spire Plus ModPatcher patches, has a clean `godot-log-audit.json`, passed enabled-mode verifier 31 / 0, and passed packet verifier 61 / 0. This is not beta.96 loader or gameplay proof. |
| Default Off runtime state (O13) | **previous-package pass** | Beta.85 Off smoke logs `Feature Sts1Events ... bootstrap=disabled, live=Disabled`; no enabled StS1 event registrations are claimed from this Off run. |
| CanaryOnly source identity (O14) | **static-pass / previous-package runtime pass** | Static registry-shape/current-doc guards expect 4 canary event types through 6 registration calls: Sts1BigFish and Sts1GoldenIdol in both Act 1 buckets, plus shared Sts1TheLab and Sts1DivineFountain. Retained beta.85 `v0.107.0` CanaryOnly enabled-mode smoke is O25 and passes as previous-package loader proof. |
| AdditiveBatch1 source identity (O15) | **static-pass / current loader pass** | Static registry-shape/current-doc guards expect 10 event types through 14 registration calls after Big Fish, Golden Idol, The Cleric, and Shining Light moved to both Act 1 buckets. Beta.93 `v0.107.1` direct AdditiveBatch1 enabled-mode smoke is O33 and passes as current loader proof. |
| AdditiveAllDraft unsafe (O16) | **pass (source-guarded)** | Requires `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`; test-gated. |
| ReplacementPrototype fail-closed (O17/O18) | **pass (source-guarded)** | Requires `#if REPLACEMENT_PROTOTYPE_ENABLED` + unsafe override; test-gated. |
| Canary runtime launch (O25) | **previous-package pass** | Fresh beta.85 CanaryOnly enabled-mode smoke at `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/` reached main menu, audited clean, and retained `enabled-mode-log-check.json` plus `runtime-evidence-packet-check.json` with 0 mismatches. |
| Canary event screenshots (O26-O29) | **blocked** | Requires in-game event encounter screenshots (Big Fish, Golden Idol, Lab, Divine Fountain). |
| Canary save/load proof (O34) | **blocked** | Requires save during/after event, reload, state stable. |
| Canary EN/ZHS render (O35-O36) | **blocked** | Requires in-game EN/ZHS text render screenshots. |
| Canary image/license render (O37) | **blocked** | No redistributable art; requires extraction/placeholder decision. |
| AdditiveBatch1 runtime launch (O33) | **current loader/registration pass** | Beta.93 RitsuLib-only AdditiveBatch1 direct smoke at `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/` reached main menu on `v0.107.1`, audited clean, and retained verifiers passed with 10 event types / 14 registration calls and exact act/shared tuple parity. Beta.88 remains previous-package context, and beta.87 `v0.107.1` failure at `.tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/` is previous package `v3.2.1` root-cause history. |
| Simple batch event proofs (O42-O47) | **blocked** | Requires per-event in-game encounter screenshots and result logs. |
| Simple batch save/load (O48) | **blocked** | Requires save/load proof for simple batch events. |
| Simple batch EN/ZHS render (O49) | **blocked** | Requires in-game EN/ZHS text render screenshots. |
| Simple batch image/license render (O50) | **blocked** | No redistributable art; requires extraction/placeholder decision. |
| Replacement functional proof (O54-O57) | **blocked** | Requires debug symbol, explicit unsafe env gate, game launch, seeded unknown-room replacement proof. |
| Multiplayer fail-closed (O58) | **blocked** | Requires multiplayer session or runtime fail-closed proof. |
| QA Red-Team (O65) | **blocked** | Independent QA cannot pass while event encounter, save/load, image, replacement, and multiplayer gates remain blocked. |

## Current Gate Alignment Notes

- `CanaryOnly` remains exactly Big Fish, Golden Idol, The Lab, and Divine Fountain.
- `AdditiveBatch1` is now a separate mode and must not be described as `AdditiveAllDraft`.
- `AdditiveBatch1` now registers 10 event types through 14 registration calls because Big Fish, Golden Idol, The Cleric, and Shining Light are available in both StS2 Act 1 buckets: Overgrowth and Underdocks.
- Dedicated event-spec pages now exist for all six AdditiveBatch1 simple-batch events, including Purifier and Golden Shrine.
- Old Beggar Offer Gold is source-gated on 75+ gold so underfunded players cannot buy card removal through `PlayerCmd.LoseGold` clamping. Runtime UI/result proof remains pending with the rest of AdditiveBatch1.
- Shining Light now source-upgrades random upgradable deck cards with event RNG and no manual upgrade picker. Runtime result proof remains pending with the rest of AdditiveBatch1.
- Golden Shrine source/localization now use StS1-aligned Pray/Desecrate/Leave options: Pray grants 100 gold, 50 at A15+; Desecrate grants 275 gold and adds Regret. Runtime UI/result proof remains pending with the rest of AdditiveBatch1.
- The Cleric source/localization now guard the 35+ gold event eligibility, A15+ Purify cost increase from 50 to 75 gold, and Act 1 bucket registration. Runtime UI/result/bucket proof remains pending with the rest of AdditiveBatch1.
- Static localization source-reference scan found 33 result-page keys referenced by source but missing from both EN and ZHS. One key (`STS1_GOLDEN_IDOL.pages.LEAVE.description`) affects current CanaryOnly/AdditiveBatch1 directly; the other 32 are later RegisterAll/draft or blocked-combat surfaces. See `localization-source-gap-scan-20260611.md` and `localization-gap-closure-plan.md`; this blocks any source-complete localization claim. Closing only `STS1_GOLDEN_IDOL.pages.LEAVE.description` remains a localization unblocker; it does not prove gameplay, and it does not replace `enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` verifier reports.
- Beta.85 Off and CanaryOnly loader proof remains previous-package context at `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/` and `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/`. Retained beta.87 AdditiveBatch1 enabled-mode proof is clean for `v0.107.0` at `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/`; the beta.87 `v0.107.1` previous package `v3.2.1` failure is root-cause history; beta.88 AdditiveBatch1 proof is clean for `v0.107.1` at `.tools/runtime-evidence/v01071-beta88-previous-package330-additive-batch1-direct-cleanlog-20260619-103937/`. These loader rows do not prove event encounter gameplay, save-load, rendering, replacement, multiplayer, or QA gates.
- `AdditiveAllDraft` remains unsafe/dev-only and now requires `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` in addition to `SPIREPLUS_STS1_EVENT_MODE=AdditiveAllDraft`.
- `ReplaceUnknownEventsPrototype` remains debug-only and does not register events unless compiled with `REPLACEMENT_PROTOTYPE_ENABLED` and explicitly allowed with `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`.
- Joust and The Ssssserpent are source-classified as non-combat Act 1 events; encounter-model blockers apply only to actual combat-entry events.
- **Revision M correction**: Clean historical `v0.106.1` loader evidence exists for Off, CanaryOnly, and AdditiveBatch1 modes. The beta.84 `v0.107.0` package smoke at `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` remains root-cause evidence for stale Spire Plus API targets; beta.85 Off smoke is clean previous-package context and beta.87 AdditiveBatch1 is retained `v0.107.0` enabled-mode loader proof.
- Remaining blocked/current-pending gates: canary encounter screenshots (O26-O29), canary result/pre-post/save-load/render/image/parity/audit/docs/owner rows (O31-O32 and O34-O41), simple-batch event proofs and save/render/image/audit/QA rows (O42-O52), replacement functional proof (O54-O57), multiplayer runtime/ZHS rows (O58 and O64), independent QA (O65), and final owner/handoff rows (O72-O75). Static classification/safety rows (O59-O63), documentation-in-progress rows (O66-O71), and O76 do not close runtime or completion gates.

## Evidence Files

| Evidence | Path |
|----------|------|
| Current validation summary | `PROJECT_STATE.md`, `docs/dev-environment.md`, `docs/reviews/current-validation.md` |
| Current package/checksum summary | `docs/issues.md`, `docs/toreview.md`, `docs/private-beta-verification-handoff.md`, `docs/release-checklist.md` |
| Historical v13 build log | `.tools/runtime-evidence/sts1-events-v13/o1-build-full.log` |
| Historical v13 test log | `.tools/runtime-evidence/sts1-events-v13/o2-test-full.log` |
| Historical v13 test count/skips | `.tools/runtime-evidence/sts1-events-v13/o3-o4-test-count-and-skips.md` |
| Historical v13 git snapshot | `.tools/runtime-evidence/sts1-events-v13/o0-*.txt` |
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
| **v19 beta.85 Off packet verifier report** | `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/runtime-evidence-packet-check.json` (no-launch rerun with explicit package/Ritsu/game targets: Off packet checks=34 / mismatches=0; nested log verifier checks=10 / mismatches=0; default-Off evidence only) |
| **v20 beta.85 CanaryOnly clean log** | `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/godot.log.after-launch` |
| **v20 beta.85 CanaryOnly verifier reports** | `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` (retained enabled-mode log checks=20 / mismatches=0; packet checks=45 / mismatches=0; tuple-aware dry-run 21 / 0) |
| **v20 beta.85 AdditiveBatch1 mismatch log** | `.tools/runtime-evidence/v01070-beta85-additive-batch1-20260617-233759/godot.log.after-launch` |
| **v20 beta.85 AdditiveBatch1 verifier reports** | `.tools/runtime-evidence/v01070-beta85-additive-batch1-20260617-233759/enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` (retained reports both failed with one mismatch: 13 observed registered-event lines vs 14 expected; tuple-aware dry-run 21 / 2 also reports missing `ActEvent:Overgrowth:Sts1TheCleric` and `ActEvent:Underdocks:Sts1TheCleric` plus unexpected `SharedEvent:Shared:Sts1TheCleric`) |
| **v21 beta.86 AdditiveBatch1 diagnostic Steam-client log** | `.tools/runtime-evidence/v01070-beta86-additive-batch1-20260618-031043/godot.log.after-launch` (main menu and clean audit, but StS1 stayed disabled because the already-running Steam client did not propagate the transient environment; diagnostic only) |
| **v21 beta.87 AdditiveBatch1 clean log** | `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/godot.log.after-launch` |
| **v21 beta.87 AdditiveBatch1 verifier reports** | `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` (retained enabled-mode log checks=31 / mismatches=0; packet checks=52 / mismatches=0; exact act/shared tuple parity) |
| **v22 beta.88 AdditiveBatch1 clean log** | `.tools/runtime-evidence/v01071-beta88-previous-package330-additive-batch1-direct-cleanlog-20260619-103937/godot.log.current-iteration` |
| **v22 beta.88 AdditiveBatch1 verifier reports** | `.tools/runtime-evidence/v01071-beta88-previous-package330-additive-batch1-direct-cleanlog-20260619-103937/sts1-enabled-mode-report.json` and `sts1-runtime-evidence-packet.json` (retained enabled-mode log checks=31 / mismatches=0; packet verifier mismatches=0; current `v0.107.1` loader/registration proof only) |
| **v19 validation coordination hard stop** | `docs/features/sts1-events/hard-stop-blocker-report-v19-validation-coordination-20260611.md` |
| **v19 O0-O76 gate evidence map** | `docs/features/sts1-events/v19-gate-evidence-map.md` |
| **v19 O0-O76 per-gate ledger** | `docs/features/sts1-events/v19-gate-ledger.csv` |
| **v20 O76-O84 final-gate overlay** | `docs/features/sts1-events/v20-final-gate-overlay.csv` records final documentation, owner-action, no-unsupported-commit/push, release-claim, final-summary, and next-run boundaries from `docs/goals/event.md`; `scripts/check-sts1-v20-final-gate-overlay.ps1 -FailOnMismatch` is static/non-runtime evidence only. |
| **v20 coordination-pause hard stop** | `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md` records the current O0-O84 pause reason, exact blocked/current-pending gates, owner actions, no unsupported commit/push, and next-run start point. It is not completion or runtime proof. |
| **v19 2026-06-15 pause-safe static verification** | `docs/reviews/current-validation.md` records static suite 14 / 0, then-current current-doc claims 872 / 0, static-file hygiene 11 / 0, v19 gate ledger 531 / 0, and `git diff --check --` exit 0 with CRLF warnings only; this is static evidence only. |
| **v19 2026-06-15 subagent packet checklist guard** | `docs/features/sts1-events/v19-subagent-coverage.md` records future evidence-packet requirements plus owner/final-handoff and no-authorization boundaries; `scripts/check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 63 / 0 in that pass and remains historical static/non-runtime evidence only. |
| **v20 2026-06-17/18 pause-safe static alignment** | The retained v20 static alignment remains static-only: static suite 15 / 0, beta.86 runtime preflight 27 / 0, static-file hygiene 11 / 0, v19 gate ledger 534 / 0, v20 final-gate overlay 29 / 0, and subagent coverage 70 / 0. Later active summary cleanup aligned current-doc claims to 962 / 0 in `PROJECT_STATE.md`, `docs/goals/event.md`, `docs/reviews/current-validation.md`, this status board, `docs/features/sts1-events/v19-gate-evidence-map.md`, and the current-doc guard; runtime-monkey AutoSlay boundary/source-contract, packet-verifier, analyzer, and runtime `RuntimeLogGrowthRequired` / command-bearing `LogGrew` / no-log-growth-timeout hardening later raised the active current-doc guard to 1025 / 0 and static-file hygiene to 12 / 0 while preserving the same static-only boundary. The later pause-safe proof-mode `ExpectedAncientIds` plan/summary target-coverage hardening raised the current-doc guard to 1056 / 0 with static-file hygiene still 12 / 0, the follow-up active-doc AutoSlay proof-command target guard raised the current-doc guard to 1057 / 0, and the follow-up runtime monkey iteration-local artifact containment, packet escape-path, analyzer noncanonical-path, probe process identity, and AutoSlay malformed-path guards raised the current-doc guard to 1090 / 0. Older 956 / 0, 959 / 0, 961 / 0, 962 / 0, 974 / 0, 982 / 0, 992 / 0, 1009 / 0, 1013 / 0, 1022 / 0, 1025 / 0, 1056 / 0, 1057 / 0, 1062 / 0, 1070 / 0, 1075 / 0, and 1085 / 0 rows remain historical. Focused `git diff --check --` on the touched event-governance docs and guard script exited 0 with only CRLF warnings; this is static evidence only and does not close gameplay or game-native AutoSlay batch gates. |
| **v20 2026-06-18 `d2ff20f5` focused test-split follow-up** | `PROJECT_STATE.md` and `docs/reviews/current-validation.md` record that shared validation extracted `ReleaseEvidenceGateTests` manual evidence template tests into a partial file and added runtime-monkey analyzer coverage for log-derived owner routing. The shared lane reported build 0 warnings / 0 errors plus focused `RuntimeMonkeyStabilityGuardTests` and `ReleaseEvidenceGateTests` at 19 / 0 / 0 / 19. This event thread did not run that validation, and it does not close publish, package/release-evidence, runtime smoke, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates. |
| **v20 2026-06-17 subagent role guard** | `docs/features/sts1-events/v19-subagent-coverage.md` records the 15-role v20 subagent coverage shape while retaining the v19 filename; `scripts/check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returns 70 / 0 and remains static/non-runtime evidence only. |
| Static aggregate event suite | `scripts/check-sts1-event-static-suite.ps1` |
| Static current-doc-claims checker | `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` |
| No-launch runtime preflight checker | `scripts/check-sts1-runtime-preflight.ps1 -FailOnMismatch` reads repo and installed manifests plus source-only expected shapes; it does not launch the game or close enabled-mode/runtime gates. |
| Static feature-gate checker | `scripts/check-sts1-event-feature-gates.ps1 -FailOnMismatch` |
| Static registry-shape checker | `scripts/check-sts1-event-registry-shape.ps1 -FailOnMismatch` |
| Static event-spec registration-note checker | `scripts/check-sts1-event-spec-registration-notes.ps1 -FailOnMismatch` |
| Static parity-blocker checker | `scripts/check-sts1-event-parity-blockers.ps1 -FailOnMismatch` |
| Static asset-safety checker | `scripts/check-sts1-event-asset-safety.ps1 -FailOnMismatch` |
| Static multiplayer-shape checker | `scripts/check-sts1-event-multiplayer-shape.ps1 -FailOnMismatch` |
| Static localization source-key checker | `scripts/check-sts1-localization-source-keys.ps1` |
| Static localization gap-baseline checker | `scripts/check-sts1-localization-gap-baseline.ps1 -FailOnMismatch` |
| Static localization closure plan | `docs/features/sts1-events/localization-gap-closure-plan.md` |
| Static v19 gate-ledger checker | `scripts/check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` |
| No-launch enabled-mode log verifier | Use `scripts/check-sts1-enabled-mode-runtime-log.ps1 -Mode CanaryOnly -LogPath <future-log> -AuditPath <future-audit> -ExpectedPackageVersion <package-version> -ExpectedRitsuCompatBranch <branch> -ExpectedRitsuLibVersion <ritsulib-version> -ExpectedGameVersion <game-version> -OutFile <future-evidence-dir>\enabled-mode-log-check.json -FailOnMismatch` or the same command with `-Mode AdditiveBatch1`; enabled-mode copied logs must prove the expected Spire Plus package version, Ritsu compat branch, RitsuLib package version, and game version text in the log, and the verifier report must stay in the evidence folder. It verifies registration-call count, event class set, and observed registration tuples parsed from `Registered act event` / `Registered shared event` lines when tuple details are present. The retained audit must bind `Path`, `Length`, and `Sha256` to the copied log and match a verifier-side recomputation from that log. If future logs lose act/shared tuple detail, Act-bucket proof remains source-derived until gameplay evidence proves those targets directly. |
| No-launch runtime evidence packet verifier | Use `scripts/check-sts1-runtime-evidence-packet.ps1 -Mode CanaryOnly -EvidenceDir <future-evidence-dir> -ExpectedPackageVersion <package-version> -ExpectedRitsuCompatBranch <branch> -ExpectedRitsuLibVersion <ritsulib-version> -ExpectedGameVersion <game-version> -OutFile <future-evidence-dir>\runtime-evidence-packet-check.json -FailOnMismatch` or the same command with `-Mode AdditiveBatch1`; helper-created enabled-mode packets must record matching `Sts1EventModeEnvironment` metadata, matching `game-release-info.json`, no unsafe-mode env leakage, no `-AllowMissingSessionState` / `-AllowMissingRestoreState` bypass, retained `enabled-mode-log-check.json`, and explicit package/version target checks, and the packet verifier report must stay in the evidence folder. |
| Warning budget | `docs/goals/warning-ledger.md` |
