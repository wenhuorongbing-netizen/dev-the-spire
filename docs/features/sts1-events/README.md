# StS1 Events Migration

Track the Slay the Spire 1 public event baseline inside Spire Plus (`EZMicroBalance`) as a compact, integrated feature within the existing mod structure.

## Goal

Account for the public 52-event StS1 Wiki baseline and the internal source reconciliation: `54` canonical rows, `50` registry identities, `48` model files, `47` compiling models, `57` RegisterAll calls, and `14` AdditiveBatch1 calls. Events live inside `EZMicroBalanceCode/Sts1Events/` and localization inside `EZMicroBalance/localization/`.

Current beta.88 `v0.107.1` AdditiveBatch1 proof under `.tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/` covers loader/registration only: main-menu startup, 25/25 Spire Plus patches, 10 event types, 14 registration calls, clean audit, retained enabled-mode verifier 31 / 0, and runtime packet verifier 0 mismatches. Retained beta.85 Off/CanaryOnly and beta.87 AdditiveBatch1 logs remain previous-package/game-version loader context. Runtime gameplay, save-load, image rendering, replacement-pool behavior, multiplayer behavior, independent QA, tester handoff, and game-native AutoSlay/monkey batch proof remain unverified.

Localization file parity is not full localization coverage: `localization-source-gap-scan-20260611.md` records 33 source-referenced result-page keys missing from both EN and ZHS. Use `localization-gap-closure-plan.md` for the validated resource-pass order; it does not change shipped localization resources by itself.

## Registration Authority

Current registration authority is `Sts1EventRegistrationService`: event models inherit `EventModel` and are registered through RitsuLib content-builder calls such as `content.ActEvent<TAct,TEvent>()` and `content.SharedEvent<TEvent>()`.

Per-event spec registration lines have been refreshed to reference the central service and current RitsuLib calls. Use `registry-reconciliation.md`, `status-board.md`, `source-research/sts2-act-event-registration.md`, and the source service as the current registration truth if a future note drifts.

## Canary Events

| Event | Act | Key Behavior |
|-------|-----|--------------|
| Big Fish | Act 1 | Heal 1/3 max HP, +5 max HP, or random relic + Regret curse |
| Golden Idol | Act 1 | Current source grants a random relic substitute, then trap branch with Injury/HP damage/max HP loss |
| The Lab | All | Open for random potions; A15+ grants fewer potions |
| Divine Fountain | All | Drink to remove all curses; appears only when every player has a curse in shared mode |

## AdditiveBatch1 Simple Events

| Event | Act | Key Behavior |
|-------|-----|--------------|
| Purifier | All | Free card removal |
| Upgrade Shrine | Act 3 | Choose a card to upgrade |
| Golden Shrine | All | Pray for gold, Desecrate for more gold plus Regret, or Leave |
| The Cleric | Act 1 | Pay gold to heal or remove a card; A15+ increases Purify cost |
| Old Beggar / Pleading Vagrant | All | Pay 75 gold to remove a card; option is disabled when underfunded |
| Shining Light | Act 1 | Take damage to upgrade two random cards |

## Phases

1. **Canary**: Big Fish, Golden Idol, The Lab, Divine Fountain.
2. **AdditiveBatch1 simple batch**: Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar / Pleading Vagrant, Shining Light.
3. **Card service batch**: Events that add, remove, or transform cards.
4. **Combat batch**: Events that trigger combat encounters.
5. **Custom UI batch**: Events requiring minigame layouts.
6. **Pool replacement**: StS1-only event pool to replace StS2 events in Unknown rooms.

## Documentation

| File | Purpose |
|------|---------|
| `goal.md` | Migration goal and success criteria |
| `status-board.md` | Current evidence-backed event status |
| `v19-gate-evidence-map.md` | Current O0-O76 gate range status and remaining proof map |
| `v19-gate-ledger.csv` | Machine-readable O0-O76 per-gate status ledger |
| `v20-final-gate-overlay.csv` | Machine-readable O76-O84 final documentation and handoff overlay from the v20 stop condition |
| `hard-stop-blocker-report-v20-coordination-pause-20260617.md` | Current v20 coordination-pause hard-stop report and next-run start point |
| `v19-subagent-coverage.md` | Static coverage ledger for the 15-role v20 subagent coverage shape while retaining the v19 filename |
| `registry-reconciliation.md` | Current count and registration reconciliation |
| `wiki-event-catalog.md` | Public 52-event baseline plus internal count reconciliation |
| `implementation-plan.md` | Phased implementation roadmap |
| `source-research/sts2-event-engine.md` | StS2 event engine research |
| `source-research/sts2-act-event-registration.md` | Current act mapping and registration source evidence |
| `event-specs/README.md` | Current per-spec registration authority note and bucket map |
| `event-specs/*.md` | Per-event specification documents |
| `assets.md` | Asset extraction pipeline |
| `localization.md` | Localization key conventions |
| `localization-source-gap-scan-20260611.md` | Current 33-key source-reference gap scan |
| `localization-gap-closure-plan.md` | Static closure plan for the next validated localization/resource pass |
| `test-plan.md` | Testing strategy |

## References

- [StS1 Wiki Events](https://slay-the-spire.fandom.com/wiki/Events)
- [RitsuLib Custom Events Guide](https://sts2-ritsulib.ritsukage.com/guide/custom-events)
- [RitsuLib Content Authoring](https://sts2-ritsulib.ritsukage.com/guide/content-authoring-toolkit)
