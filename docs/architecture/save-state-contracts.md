# Save-State Contracts

Purpose: identify which Spire Plus and Future Peek states must survive reload, which states are transient, and what proof is still required.

## Contract Table

| Feature | Durable state | Transient state | Live proof needed |
| --- | --- | --- | --- |
| Root Eyes | Root Sight progress, marked coords, preview records, selected blessing. | Active map selection player and per-run committed-entry set. | Save with a marked future node, reload, hover/click/entry, stale refund. |
| Seed Bank | Stored seed cards and marker relic state. | Current extraction UI choice. | Store, save, reload, click relic, extract, enter boss. |
| Trial Branch | Trial cards and combat counters. | Combat-local play tracking. | Three-combat prove/remove sequence across save/load. |
| Seedbed | Selected blessing/progress and combat capacity. | Current combat hand interception state. | Root/status/curse interception before and after reload. |
| Morvi Debt Settlement | Debt remaining and selected blessing. | Current combat payment timing. | Battle end payment with gold and HP fallback across reload. |
| Morvi Blueprint/Open Book/Overdue | Deck/player mirrors plus combat-local lists. | Temporary upgrades, discount source, generated pages. | Play cards after save/load without freeze. |
| Lotha Death Reprieve | Deck-mirrored phase and used flag. | Current combat active/pending-start booleans. | Lethal hit, save before/after reprieve start, reload, resolve. |
| Vakuu fight | Parent event/fight completion markers and room stack state. | Active child combat transition helpers. | Active fight save/load, prefinished combat restore, victory/failure/death. |
| A20 dual boss/courtyard | Current act/boss-chain markers and any courtyard completion flags. | Active reward-screen transition helpers. | Defeat first boss, save around reward/courtyard, reload, finish second boss. |
| Reward alternatives | Vanilla reward ids, alternative selection indexes, any mirrored blessing state. | Reward-screen node hints and transient UI banners. | Save on reward screen, reload, select each alternative without freeze. |
| Ascension selector | Preferred ascension only when vanilla can represent it. | Temporary unlock/max-ascension overrides. | Start singleplayer A20, attempt multiplayer A20, verify downgrade/warning. |
| Rootblight | Deck card markers and capped state. | Combat-end overlay state. | Play/discard/end combat around save/load. |
| Future Peek | None intended for preview buttons. | Crystal Sphere UI peek state and transform prediction snapshots. | Close/reopen screens without leaked preview state. |

## Guard Boundary

Automated tests may verify serializers, state mirrors, forbidden API calls, and source shape. They do not replace live save/load proof because the game owns runtime room, reward, UI, and multiplayer serialization behavior.

## Co-op Boundary

Any gameplay state that affects rewards, cards, HP, gold, map movement, or combat must either be host-authoritative and synchronized through game commands or explicitly unsupported for co-op release.
