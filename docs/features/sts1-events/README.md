# StS1 Events Migration

Migrate all Slay the Spire 1 events into Spire Plus (EZMicroBalance) as a
compact, integrated feature within the existing mod structure.

## Goal

Port all 52 StS1 Wiki events to StS2 using RitsuLib `ModEventTemplate` and
`RegisterActEvent`/`RegisterSharedEvent` attributes. Events live inside
`EZMicroBalanceCode/Sts1Events/` and localization inside
`EZMicroBalance/localization/`.

## Canary Events (Phase 1)

| Event | Act | Key Behavior |
|-------|-----|-------------|
| Big Fish | Act 1 | Heal 1/3 max HP, +5 max HP, or random relic + Regret curse |
| Golden Idol | Act 1 | Take Golden Idol → trap branch with Injury/HP damage/max HP loss |

## Phases

1. **Canary**: Big Fish, Golden Idol — prove the pattern works
2. **Simple batch**: Events with heal/damage/gold/card rewards only
3. **Card service batch**: Events that add/remove/transform cards
4. **Combat batch**: Events that trigger combat encounters
5. **Custom UI batch**: Events requiring minigame layouts (e.g., Match and Keep)
6. **Pool replacement**: StS1-only event pool to replace StS2 events in Unknown rooms

## Documentation

| File | Purpose |
|------|---------|
| `goal.md` | Migration goal and success criteria |
| `wiki-event-catalog.md` | Full 52-event catalog from StS1 Wiki |
| `implementation-plan.md` | Phased implementation roadmap |
| `source-research/sts2-event-engine.md` | StS2 event engine research |
| `event-specs/*.md` | Per-event specification documents |
| `assets.md` | Asset extraction pipeline |
| `localization.md` | Localization key conventions |
| `test-plan.md` | Testing strategy |

## References

- [StS1 Wiki Events](https://slay-the-spire.fandom.com/wiki/Events)
- [RitsuLib Custom Events Guide](https://sts2-ritsulib.ritsukage.com/guide/custom-events)
- [RitsuLib Content Authoring](https://sts2-ritsulib.ritsukage.com/guide/content-authoring-toolkit)
