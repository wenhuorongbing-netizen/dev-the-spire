# StS1 Event Port Strict Audit + June 2026 Monthly Dev Spec

Generated: 2026-05-28 Europe/Berlin

## Executive verdict

**Not complete.** The delivered work is a useful research/scaffold package, but it is not a playable StS1 event migration and should not be described as “all Slay the Spire 1 events implemented.”

Highest-confidence completion state:

| Area | Audit result | Reason |
| --- | --- | --- |
| Wiki catalog | Partial / inconsistent | Package manifest has 52 rows and 52 spec files, but the pasted work log claims 48 specs cover all unique events, which conflicts with the canonical 52-event Wiki grouping. |
| Per-event specs | Partial | Files exist, but most are generic templates, not exact option-by-option specs. Dependencies/tests are blank placeholders. |
| Code | Prototype scaffold only | All Sts1Events code is behind `#if STS1_EVENT_PORT_PROTOTYPE`; 52 source files contain TODOs; most event classes only expose a Leave/Done stub. |
| Canary events | Not complete | Big Fish / Golden Idol have partial code, missing curse/relic application, mutability fixes, source-verified helpers, asset proof, localization parity, save/load proof, and manual tests. |
| Assets/images | Not complete | No StS1 event images are present. Extraction script only has TODO source mappings for two events, not 52. |
| Localization | Skeleton only | English and Chinese files exist, but many entries are `[PLACEHOLDER]` / `[占位]`, not parity text or final rewritten text. |
| Tests | Not complete | Only a manifest-count test template exists. No build/test/publish evidence, no live spawn evidence, no screenshot evidence. |
| Event pool parity | Not started | Additive registration is not the same as replacing StS2 unknown-room events with the StS1 event pool. |

## Evidence-based checks

### 1. Pasted work log claims

The pasted log states “48 spec files created” and justifies this by saying the 52 catalog rows contain four act-specific duplicates. It then marks `event-specs/ (48 unique events)` as Done and marks Big Fish / Golden Idol source + localization Done, with assets and tests still Pending. It also lists blockers such as Regret, Injury, random relic helper, card removal/transform/upgrade UI, and combat encounter models.

Strict finding: these claims are internally inconsistent and too optimistic. Marking Phase 0 as Done is not justified unless the manifest, spec count, source verification, current-project integration, and test guard all agree.

### 2. Extracted package metrics

I inspected `sts1_event_port_research_and_scaffold.zip` locally. It contains:

- 52 rows in `manifests/sts1_events_manifest.csv`.
- 52 files under `docs/features/sts1-events/event-specs/`.
- 57 C# files under `code/EZMicroBalanceCode/Sts1Events/`.
- 52 event source files containing TODOs.
- 260 EN localization keys and 260 ZHS localization keys.
- 104 placeholder localization values in each language file.
- 52 asset manifest rows, all marked as local extraction / do-not-commit original StS1 assets.

Strict finding: the package is better than the pasted log on file count, but still not implementation-complete.

### 3. Wiki scope check

The Slay the Spire Wiki groups events as 16 shared events, 12 Act 1 exclusive events, 16 Act 2 exclusive events, and 8 Act 3 exclusive events. That totals 52 unique event entries. The Wiki also states events are selected by random chance and current Act, do not occur in Act 4, and Ascension 15 makes unfavorable events more likely or more intense.

Strict finding: the next docs must use “52 unique Wiki event entries” as the canonical target unless a deliberate product decision excludes something like Neow or A Note For Yourself.

### 4. Source/API check

StS2 v0.106.0 source confirms the core implementation direction is basically right:

- `EventModel` is the correct base abstraction for events.
- `EventOption` supports option callbacks, locked options, relic display, damage death warning, max-HP-loss warning, and choice-history controls.
- `ActModel.GenerateRooms` adds act events plus shared events into `RoomSet.events`.
- `ActModel.PullNextEvent` validates the next event, then calls `Hook.ModifyNextEvent`.
- RitsuLib 0.3.3 exposes `RegisterSharedEvent<T>()` and `RegisterActEvent<TAct,TEvent>()`, plus event asset override hooks.

Strict finding: additive registration is sufficient for debug/prototype testing, but not sufficient for “StS1-only unknown room parity.” That needs an explicit event-pool replacement service or a narrowly documented patch.

### 5. Code defect samples

- `Sts1BigFish.Box()` obtains a relic from `RelicFactory.PullNextRelicFromFront(Owner)` and passes it to `RelicCmd.Obtain` without `.ToMutable()`. In StS2 source, `RelicCmd.Obtain` asserts the relic is mutable, so this likely fails at runtime.
- `Sts1BigFish.Box()` does not add Regret.
- `Sts1GoldenIdol.Take()` does not obtain Golden Idol.
- `Sts1GoldenIdol.Outrun()` does not add Injury.
- `Sts1DivineFountain.IsAllowed()` uses `c.IsCurse`; StS2 card source shows `CardModel.Type` and curse rarity/type patterns, but not an `IsCurse` property in the checked source. This likely fails when prototype compilation is enabled.
- Most event files are placeholder classes that immediately finish or only expose Leave.

Strict finding: do not move to “Phase 2 simple batch implementation” yet. First fix canary compile/runtime correctness.

## Corrected phase status

| Phase | Correct status | Notes |
| --- | --- | --- |
| 0. Documentation/inventory | Partial | Need correct 52-event canonical list, source matrix, status board truthfulness, repo integration. |
| 1. Infrastructure | Partial | Feature gate and registry scaffold exist, but disabled and not integrated; no debug spawn; no build proof. |
| 2. Asset pipeline | Partial | Manifest exists; extraction map incomplete; no images; no validation evidence. |
| 3. Canary events | Not complete | Big Fish / Golden Idol / Lab / Divine Fountain not production-ready. |
| 4. Simple events | Not started | Specs only; no implemented behavior. |
| 5. Card service events | Not started | No card select/remove/upgrade/transform service. |
| 6. Combat events | Not started | No StS1 encounters/rewards/resume flow. |
| 7. Custom UI events | Not started | No Match and Keep / Wheel of Change UI. |
| 8. StS1-only pool | Not started | Needed for “完全一模一样”的 unknown-room event distribution. |
| 9. QA/release evidence | Not started | No build/publish/live screenshots/save-load evidence. |

## Monthly Dev Spec: June 2026

### Month goal

By 2026-06-30, deliver a **StS1 Event Port Prototype Batch 1** for Spire Plus/EZMicroBalance that is honest, buildable, and testable:

1. The prototype flag OFF path has zero behavior change.
2. The prototype flag ON path compiles.
3. Four canary events are fully playable through debug spawn: Big Fish, Golden Idol, Lab, Divine Fountain.
4. At least six simple events are implemented after canaries: Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar, Shining Light.
5. No claim of full parity is made.

### Non-goals for June

- Do not claim 52/52 full parity.
- Do not implement/ship StS1 original images in a public package unless permission is documented.
- Do not ship custom UI events as final parity.
- Do not ship combat events as final parity.
- Do not replace all unknown-room events in release mode without save/load and multiplayer guard proof.

### Weekly plan

#### Week 0: 2026-05-28 to 2026-05-31 — audit repair and repo integration

Deliverables:

- `docs/features/sts1-events/audit-2026-05-28.md`
- corrected `status-board.md`
- corrected `wiki-event-catalog.md`
- `docs/features/sts1-events/source-research/api-command-matrix.md`
- update `docs/README.md`, `docs/PROJECT_MAP.md`, `docs/features/README.md`

Acceptance:

- Status board has only these states: `planned`, `spec-drafted`, `source-verified`, `implemented`, `asset-verified`, `manual-verified`, `blocked`.
- No event marked implemented unless it has source file, no TODO in implementation path, loc keys, asset path, and test row.
- Canonical target says 52 unique Wiki events.

#### Week 1: 2026-06-01 to 2026-06-07 — infrastructure and compile-on gate

Deliverables:

- `Sts1EventFeatureGate` with explicit modes: Disabled, CanaryOnly, AdditiveAll, ReplaceUnknownEventsPrototype.
- `Sts1EventRegistry` integrated into current Spire Plus initialization.
- `Sts1EventAssetProvider` using RitsuLib event portrait override or `ModEventTemplate` asset override.
- `Sts1EventDebugSpawnCommand` or equivalent dev console path.
- Test coverage for manifest, loc keys, spec coverage, asset manifest coverage.

Acceptance:

- `dotnet build` passes with prototype flag OFF.
- `dotnet build` passes with prototype flag ON.
- Running with flag OFF registers no StS1 events.
- Running with CanaryOnly registers exactly the canary events.

#### Week 2: 2026-06-08 to 2026-06-14 — four canaries fully playable

Events:

- Big Fish
- Golden Idol
- Lab
- Divine Fountain

Required support:

- `Sts1RewardService`: random relic, potion generation, curse add, curse detection/removal.
- `Sts1HpService`: heal, gain max HP, damage by max HP percent, lose max HP percent, death-warning option helper.
- `Sts1AscensionRules`: StS1 A15 event-difficulty mode mapped from Spire Plus/StS2 ascension setting.
- `GoldenIdolRelic` if StS2 has no compatible Golden Idol relic.

Acceptance:

- Big Fish: Banana heals floor(maxHP/3), Donut grants +5 max HP and heals gained HP, Box gives a mutable random relic and adds Regret.
- Golden Idol: Take grants Golden Idol, then Outrun adds Injury, Smash deals 25%/35% max HP damage, Hide loses 8%/10% max HP, Leave does nothing.
- Lab: grants exactly three potions or documented StS2-compatible equivalents.
- Divine Fountain: allowed only with curses; removes all curses from deck.
- All four have EN/ZHS non-placeholder keys and local screenshot evidence.
- Save/load after the trap page or reward page does not duplicate rewards.

#### Week 3: 2026-06-15 to 2026-06-21 — simple batch 1

Events:

- Purifier
- Upgrade Shrine
- Golden Shrine
- The Cleric
- Old Beggar
- Shining Light

Required support:

- card removal/select service
- card upgrade select service
- gold command helper
- HP-loss and heal helpers
- option lock helper when deck/gold/card prerequisites are absent

Acceptance:

- Six events debug-spawn verified.
- Each event has normal values and A15 differences if applicable.
- Implemented event files contain no TODOs in reachable code.
- Implemented localization values are not placeholders.
- Each event has one manual test row and one screenshot row.

#### Week 4: 2026-06-22 to 2026-06-28 — batch hardening and replacement-pool prototype

Deliverables:

- `Sts1EventPoolService` design and prototype tests.
- Additive vs ReplaceUnknownEventsPrototype documented.
- Save-state fields for shuffled event bag and visited event ids.
- Manual test pass for all implemented events.
- Asset extraction map filled for implemented events.

Acceptance:

- Additive mode remains the only recommended manual testing mode unless replacement-pool save/load proof exists.
- Replacement mode is debug-only and multiplayer fail-closed.
- No StS2 event appears in a replacement-mode unknown room during a controlled test run for the current act bucket.

#### Week 5 buffer: 2026-06-29 to 2026-06-30 — package and handoff

Deliverables:

- package version increment if any player-visible behavior is included.
- updated release notes that call it prototype/batch, not full parity.
- `docs/features/sts1-events/monthly-review-2026-06.md`.
- commit and push if validation succeeds.

Acceptance:

- Build, publish, tests, and manual evidence are recorded.
- Open blockers are explicit and not hidden in archive docs.
- A tester can reproduce debug spawn and asset validation from the docs.

## Issue backlog IDs

| ID | Task | Priority | Depends on | Exit criteria |
| --- | --- | --- | --- | --- |
| STS1-MONTH-001 | Correct catalog/status board | P0 | none | 52 unique target, no false Done states |
| STS1-MONTH-002 | Integrate docs into repo | P0 | 001 | docs index/project map/features index updated |
| STS1-MONTH-003 | API command matrix | P0 | uploaded source/RitsuLib | HP/gold/card/relic/potion APIs documented |
| STS1-MONTH-004 | Prototype flag ON build | P0 | 003 | build passes with StS1 code included |
| STS1-MONTH-005 | Asset override + extraction for canaries | P0 | 003 | images load locally, no original assets committed |
| STS1-MONTH-006 | Big Fish implemented | P0 | 004,005 | all 3 options verified |
| STS1-MONTH-007 | Golden Idol implemented | P0 | 004,005 | all branches + A15 verified |
| STS1-MONTH-008 | Lab implemented | P0 | 004,005 | 3 potion reward verified |
| STS1-MONTH-009 | Divine Fountain implemented | P0 | 004,005 | curse detection/removal verified |
| STS1-MONTH-010 | Simple batch 1 implemented | P1 | 006-009 | six simple events verified |
| STS1-MONTH-011 | Replacement-pool prototype | P1 | 006-010 | debug-only save/load-safe pool prototype |
| STS1-MONTH-012 | Monthly QA handoff | P0 | all | evidence, package, honest docs |

## Release language rule

Allowed language for June builds:

- `StS1 Event Port Prototype`
- `Canary Batch`
- `Batch 1`
- `Additive test mode`

Forbidden language until full validation:

- `StS1 full parity`
- `all events implemented`
- `same as Slay the Spire 1`
- `complete migration`
