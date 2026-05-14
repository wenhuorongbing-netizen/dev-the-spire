# Multiplayer Risk Matrix

Reviewed source baseline: `a2183ee`. Any "unknown" entry is a release blocker until a two-client Steam test or source proof resolves it.

| Feature | Host-only safe? | Client-safe? | Shared run mutation | Local UI only | `LocalContext.IsMe` | `Player.IsActiveForHooks` | `RunState.Players` use | Needs network command replication | Direct mutation/desync risk | Card/reward/power authority | Map metadata consistency | Save/load impact | Test row needed |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Ancient reward rebalance v4 | Partial | Unknown | Yes | Some UI hints | Some paths use local UI guards | Mixed by feature | Mixed | Yes | Medium | Rewards/cards/powers via commands and patches | N/A | Yes | Full Ancient co-op reward matrix if multiplayer supported. |
| Urda Ancient offer/selection | Unknown | Unknown | Yes | Event UI | Not proven in selection path | Not proven | Per selected player | Yes | Medium | Deck/state mutation on option selection | N/A | Yes | Co-op Act 1 Ancient selection, host/client deck/state comparison. |
| Urda Seedbed | Unknown | Unknown | Yes | Reward alternative UI | Not explicit in `UrdaRunHook` path | Not explicit | Per reward player | Yes | High | Adds deck card, loses max HP, reward alternative selection by index | N/A | High | Host/client normal combat reward, accept/skip/reroll/save-load. |
| Urda Humus Pact | Unknown | Unknown | Yes | Reward alternative plus deck selector | Not explicit | Not explicit | Per reward player | Yes | High | Gold, deck removal, custom card reward | N/A | High | Three composts, resolver fail/reload, host/client deck/gold. |
| Urda Molting | Unknown | Unknown | Yes | Card preview | Not explicit | Not explicit | Selection player; cleanup loops all players | Yes | High | Removes starters, adds Withered Husks, later removes husks | N/A | High | Co-op selection and Act 2 transition. |
| Urda Moss Map | No proof | Unknown | Yes | No | No visible local guard | No visible active-player guard | Loops all players with selected blessing | Yes | High | Gold/heal/potion/upgrade/max HP commands may run on all clients | N/A | High | Co-op first room type rewards; duplicate application check. |
| Morvi Misprint Press | No proof | Unknown | Combat pile/card play | Combat visuals | Not explicit | Not explicit | Combat reset loops all players | Yes | High | Generated clone plus autoplay | N/A | Medium | Default-off only; co-op first Attack/Skill replay if testing enabled. |
| Morvi Open-Book Exam | No proof | Unknown | Reward option mutation | Reward UI | Not explicit | Not explicit | Per reward player | Yes | Medium | Upgrades reward option | N/A | Medium | Default-off reward determinism test. |
| Morvi Debt Settlement | No proof | Unknown | Gold/HP/custom reward | Reward alternative UI | Not explicit | Not explicit | Per reward player | Yes | High | Alternative payment and payoff reward | N/A | High | Default-off debt matrix if Morvi testing enabled. |
| Future Morvi Red Ink | No | No | Likely | Active UI | Not designed | Not designed | Unknown | Yes | Very high | Cost/debt/card UI authority unknown | N/A | High | Do not implement yet. |
| Future Lotha death/verdict effects | No | No | Yes | Some UI | Not designed | Not designed | Unknown | Yes | Very high | Damage/death authority | N/A | Very high | Do not implement yet. |
| Vakuu Fight | No | No | Yes | Event/combat UI | Not designed | Not designed | Unknown | Yes | Very high | Combat transition/reward/failure authority | N/A | Very high | Do not implement yet. |
| RootDeckService / Rootblight | Partial | Unknown | Yes | Notices local | Uses local notice guards in key paths | Uses active player checks in key paths | Multiple player scans exist | Yes | Medium | Adds/removes deck cards and combat generated cards | N/A | High | A14/A15/A18 co-op deck count/save-load. |
| Blight Sprout generated cards | Partial | Unknown | Combat piles/deck after combat | Notices/card art | Some local owner paths | Some active checks | Per owner | Yes | Medium | Generated cards and Rootblight insertion | N/A | High | Co-op boss/elite with host/client deck comparison. |
| Firemark map metadata | Unknown | Unknown | Map metadata/combat effects | Hover UI | Hover UI local | Combat target must be active | Map-wide metadata | Yes | Medium | Enemy powers/rewards | Weak-table metadata must regenerate identically | Medium | A12 co-op map preview/combat/reward. |
| Banner rooms | Unknown | Unknown | Map metadata/combat/rewards | Hover UI | Hover UI local | Combat target must be active | Map-wide metadata | Yes | Medium | Enemy powers/player rewards | Weak-table metadata must regenerate identically | Medium | A16 co-op map preview/combat/reward. |
| Boss Seal A19 | Unknown | Unknown | Boss metadata/combat/rewards | Boss hover UI | Hover UI local | Combat target must be active | Map-wide metadata | Yes | High | Boss powers/rewards | Weak-table metadata must regenerate identically | Medium | A19 co-op boss preview/combat/reward. |
| A20 Boss Seal/Brand/second boss | Downgraded/gated for multiplayer | Not release-safe | Boss flow/map/reward transition | Intermission UI | UI local | Unknown | Map-wide | Yes | Very high | Boss reward/intermission/transition authority | Second boss metadata must match | High | A20 co-op should verify warning/downgrade, not full support. |
| A11 map shape | Unknown | Unknown | Generated map | Map UI | Local UI only | N/A | Map-wide | Yes | Medium | Route availability | Saved map rebuilt from generated map | Medium | A11 co-op route traversal/save-quit. |
| Multiplayer selection/warning | Host-controlled | Client display unknown | Lobby prefs/run launch | Lobby UI | Local UI | N/A | Lobby players | Yes | Medium | Ascension value propagation | N/A | Preference writes intentionally guarded | A10/A11/A20 host/client lobby tests. |
| ModelDb/mod-list diagnostics | Yes | Yes | Logs only | N/A | N/A | N/A | N/A | No | Low | Diagnostic only | N/A | No | Host/client mismatch log capture. |

## Multiplayer Conclusions

- Reward alternatives are the highest current content risk because the game synchronizes reward choice by index. Any host/client divergence in available alternatives can select the wrong action.
- Urda Moss Map is the most suspicious active Urda path because it loops `RunState.Players` without a visible `IsActiveForHooks` or `LocalContext.IsMe` filter in the audited source.
- Rootblight is better guarded than Urda, but still requires co-op proof because it mutates deck/combat piles and uses combat-end timing.
- A20 second boss support should remain single-player-gated or downgraded for multiplayer until direct host/client evidence proves otherwise.

