# StS1 Events Status Board

> Last updated: 2026-05-31 (v11 gate alignment)
> Audit standard: strict v11 — no generic "Done", only evidence-backed statuses

## Allowed Statuses

```
planned → spec-drafted → wiki-verified → api-verified → implemented → compiled → test-guarded → asset-mapped → loc-render-verified → manual-verified → save-load-verified
blocked | temporary-substitute | compile-excluded | special-stub | duplicate-wiki-entry
```

## Overall Summary

| Metric | Count | Evidence |
|--------|-------|----------|
| Wiki event entries | 54 | canonical-event-matrix.csv |
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
| Build | 0 errors / 89 warnings | 2026-05-31 clean solution build; warnings are Sts1Events nullable staging warnings |
| Tests | 452 passed / 0 failed / 21 skipped | 2026-05-31 `dotnet test EZMicroBalance.sln --no-build` |

## Phase Status

| Phase | Events | Compiled | Blocked | Status |
|-------|--------|----------|---------|--------|
| Canary (4) | Big Fish, Golden Idol, The Lab, Divine Fountain | 4 | 0 | compiled, test-guarded, source/API verified |
| AdditiveBatch1 verified scope (10 event types) | Big Fish, Golden Idol, The Lab, Divine Fountain, Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar, Shining Light | 10 | 0 | compiled, source-guarded; runtime unverified |
| Simple (17 outside canary) | Shining Light, Mushrooms, Altar, Drug Dealer, The Library, Ancient Writing, Augmenter, Sensory Stone, Moai Head, Transmogrifier, Upgrade Shrine, The Cleric, Golden Wing, Living Wall, Old Beggar, Bonfire Spirits, Fountain of Cleansing, Purifier, Golden Shrine | 19 total simple-batch candidates; Lab and Divine Fountain are canary metadata | 0 | compiled, source-guarded |
| CardService (9) | Face Trader, The Mausoleum, Council of Ghosts, Cursed Tome, Knowing Skull, Nest, Vampires, Falling, Mind Bloom | 9 | 0 | compiled (3 temporary-substitute) |
| Combat (7) | Dead Adventurer, Scorpion Nest, Treasure Ooze, Joust, The Ssssserpent, Masked Bandits, Mysterious Sphere | 7 | 5 | compiled (5 blocked by missing encounter models) |
| CustomUI (8) | The Woman in Blue, Wheel of Change, Designer, Forgotten Altar, The Ghost, N'loth, Tomb of Lord Red Mask, Winding Halls | 8 | 1 | compiled (1 blocked: N'loth) |
| Special (2) | Neow, Combat Start | 0 | 2 | special-stub (no unknown-room model) |

## Per-Event Status

### Canary Events (4)

| Event | Status | TODOs | IsShared | Parity Gap | Proof |
|-------|--------|-------|----------|------------|-------|
| Big Fish | compiled, test-guarded, source-API-verified | none | true | none | canary-source-api-proof.md |
| Golden Idol | compiled, test-guarded, source-API-verified | none | true | none | canary-source-api-proof.md |
| The Lab | compiled, test-guarded, source-API-verified | none | true | none | canary-source-api-proof.md |
| Divine Fountain | compiled, test-guarded, source-API-verified | none | true | none | canary-source-api-proof.md |

### Blocked Events (7)

| Event | Status | Blocker |
|-------|--------|---------|
| Dead Adventurer | blocked | Missing encounter model (random elite) |
| Scorpion Nest | blocked | Missing encounter model (3 Louses) |
| Treasure Ooze | blocked | Missing encounter model (large slime) |
| Masked Bandits | blocked | Missing encounter model (3 bandits) |
| Mysterious Sphere | blocked | Missing encounter model (2 Orb Walkers) |
| Mind Bloom (War option) | temporary-substitute | War blocked; Awake/Rich implemented |
| N'loth | blocked | No RelicSelectCmd API in StS2 |

### Temporary Substitutes (4)

| Event | Substitute | Parity Gap |
|-------|------------|------------|
| Face Trader | Random relic instead of face relics | Face relic models don't exist in StS2 |
| Nest | Clumsy curse instead of Parasite | Parasite curse doesn't exist in StS2 |
| Vampires | Removes Strikes but cannot add Bite | Bite card doesn't exist in StS2 |
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

## Runtime Unverified (requires game launch)

| Gate | Status | Blocker |
|------|--------|---------|
| Canary debug spawn (O15) | **unverified** | Requires game launch + SPIREPLUS_STS1_EVENT_MODE env var |
| Canary save/load (O16) | **unverified** | Requires game launch |
| Canary images (O12) | **unverified** | No redistributable art; requires runtime load test |
| Simple batch playable (O18) | **unverified** | Requires game launch |
| Replacement functional (O19) | **unverified** | Requires game launch + seeded run proof |
| Multiplayer co-op (O22) | **unverified** | Requires multiplayer session |
| QA Red-Team (O23) | **unverified** | Requires independent verification |

## v11 Gate Alignment Notes

- `CanaryOnly` remains exactly Big Fish, Golden Idol, The Lab, and Divine Fountain.
- `AdditiveBatch1` is now a separate mode and must not be described as `AdditiveAllDraft`.
- `AdditiveBatch1` registers 10 event types through 11 registration calls because Shining Light is available in both StS2 Act 1 buckets: Overgrowth and Underdocks.
- `AdditiveAllDraft` remains unsafe/dev-only and is the only non-prototype all-draft additive mode.
- `ReplaceUnknownEventsPrototype` remains debug-only and does not register events unless compiled with `REPLACEMENT_PROTOTYPE_ENABLED`.
- Runtime gameplay, image rendering, save/load, ReplacementPrototype functional proof, multiplayer proof, and independent QA remain blocked/unverified.

## Evidence Files

| Evidence | Path |
|----------|------|
| Build log | .tools/runtime-evidence/sts1-events-overnight-202606/o1-build-full.log |
| Test log | .tools/runtime-evidence/sts1-events-overnight-202606/o2-test-full.log |
| Git snapshot | .tools/runtime-evidence/sts1-events-overnight-202606/o0-*.txt |
| Canonical matrix | docs/features/sts1-events/canonical-event-matrix.csv |
| Registry reconciliation | docs/features/sts1-events/registry-reconciliation.md |
| IsShared matrix | docs/features/sts1-events/multiplayer-is-shared-matrix.md |
| Content parity gaps | docs/features/sts1-events/content-parity-gaps.md |
| Canary source/API proof | docs/features/sts1-events/canary-source-api-proof.md |
| Combat blockers report | docs/features/sts1-events/combat-blockers-report.md |
