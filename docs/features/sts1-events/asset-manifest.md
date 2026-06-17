# StS1 Events Asset Manifest

> Created: 2026-05-29
> Updated: 2026-06-11 for the current 48-model / 47-compiling-model inventory.
> Status: No StS1 event images exist in tracked files. All rows require asset creation, local extraction, or confirmed redistribution permission before image/render proof can close.

## Summary

| Metric | Count |
|--------|-------|
| Tracked StS1 event model rows | 48 model files (47 compiling + compile-excluded Duplicator) |
| Runtime image proof targets | 47 compiling models |
| Images available | 0 |
| Images in `images/events/` | 3 Ancient portraits only: `ezmb_lotha`, `ezmb_morvi`, `ezmb_urda` |
| Redistribution permission | Not confirmed for StS1 original art |

Static reproduction:

```powershell
.\scripts\check-sts1-event-asset-safety.ps1 -FailOnMismatch
```

## Asset Policy

Per AGENTS.md hard rules:

- Original StS1 art may not be committed without confirmed redistribution permission.
- Acceptable sources are Spire Plus-owned/generated replacement art, local extraction outside the repository, or confirmed redistributable sources.
- Current approach is no custom StS1 event portrait in tracked files until the asset policy is resolved.

## Per-Event Image Status

| # | Event ID | Model Class | Image File | Status | Path |
|---|----------|-------------|------------|--------|------|
| 1 | sts1_big_fish | Sts1BigFish | -- | missing | -- |
| 2 | sts1_the_cleric | Sts1TheCleric | -- | missing | -- |
| 3 | sts1_golden_idol | Sts1GoldenIdol | -- | missing | -- |
| 4 | sts1_golden_wing | Sts1GoldenWing | -- | missing | -- |
| 5 | sts1_living_wall | Sts1LivingWall | -- | missing | -- |
| 6 | sts1_old_beggar | Sts1OldBeggar | -- | missing | -- |
| 7 | sts1_the_woman_in_blue | Sts1TheWomanInBlue | -- | missing | -- |
| 8 | sts1_bonfire_spirits | Sts1BonfireSpirits | -- | missing | -- |
| 9 | sts1_divine_fountain | Sts1DivineFountain | -- | missing | -- |
| 10 | sts1_duplicator | Sts1Duplicator | -- | compile-excluded | -- |
| 11 | sts1_face_trader | Sts1FaceTrader | -- | missing | -- |
| 12 | sts1_fountain_of_cleansing | Sts1FountainOfCleansing | -- | missing | -- |
| 13 | sts1_the_mausoleum | Sts1TheMausoleum | -- | missing | -- |
| 14 | sts1_wheel_of_change | Sts1WheelOfChange | -- | missing | -- |
| 15 | sts1_designer | Sts1Designer | -- | missing | -- |
| 16 | sts1_the_lab | Sts1TheLab | -- | missing | -- |
| 17 | sts1_joust | Sts1Joust | -- | missing | -- |
| 18 | sts1_the_ssssserpent | Sts1TheSsssserpent | -- | missing | -- |
| 19 | sts1_shining_light | Sts1ShiningLight | -- | missing | -- |
| 20 | sts1_dead_adventurer | Sts1DeadAdventurer | -- | missing; combat blocked | -- |
| 21 | sts1_mushrooms | Sts1Mushrooms | -- | missing | -- |
| 22 | sts1_scorpion_nest | Sts1ScorpionNest | -- | missing; combat blocked | -- |
| 23 | sts1_treasure_ooze | Sts1TreasureOoze | -- | missing; combat blocked | -- |
| 24 | sts1_altar | Sts1Altar | -- | missing | -- |
| 25 | sts1_council_of_ghosts | Sts1CouncilOfGhosts | -- | missing | -- |
| 26 | sts1_cursed_tome | Sts1CursedTome | -- | missing | -- |
| 27 | sts1_drug_dealer | Sts1DrugDealer | -- | missing | -- |
| 28 | sts1_forgotten_altar | Sts1ForgottenAltar | -- | missing | -- |
| 29 | sts1_the_ghost | Sts1TheGhost | -- | missing | -- |
| 30 | sts1_knowing_skull | Sts1KnowingSkull | -- | missing | -- |
| 31 | sts1_nest | Sts1Nest | -- | missing | -- |
| 32 | sts1_the_library | Sts1TheLibrary | -- | missing | -- |
| 33 | sts1_masked_bandits | Sts1MaskedBandits | -- | missing; combat blocked | -- |
| 34 | sts1_nloth | Sts1Nloth | -- | missing; relic-select blocked | -- |
| 35 | sts1_vampires | Sts1Vampires | -- | missing | -- |
| 36 | sts1_ancient_writing | Sts1AncientWriting | -- | missing | -- |
| 37 | sts1_augmenter | Sts1Augmenter | -- | missing | -- |
| 38 | sts1_sensory_stone | Sts1SensoryStone | -- | missing | -- |
| 39 | sts1_falling | Sts1Falling | -- | missing | -- |
| 40 | sts1_mind_bloom | Sts1MindBloom | -- | missing | -- |
| 41 | sts1_moai_head | Sts1MoaiHead | -- | missing | -- |
| 42 | sts1_mysterious_sphere | Sts1MysteriousSphere | -- | missing; combat blocked | -- |
| 43 | sts1_tomb_of_lord_red_mask | Sts1TombOfLordRedMask | -- | missing | -- |
| 44 | sts1_winding_halls | Sts1WindingHalls | -- | missing | -- |
| 45 | sts1_transmogrifier | Sts1Transmogrifier | -- | missing | -- |
| 46 | sts1_upgrade_shrine | Sts1UpgradeShrine | -- | missing | -- |
| 47 | sts1_purifier | Sts1Purifier | -- | missing | -- |
| 48 | sts1_golden_shrine | Sts1GoldenShrine | -- | missing | -- |

## Blocker

No redistributable StS1 art source is confirmed. Runtime-loadable images cannot be claimed until one of these is true:

1. Redistribution permission exists for original StS1 event art.
2. Spire Plus-owned replacement art is generated or commissioned.
3. A local extraction path is documented and the extracted art stays outside tracked files.

O12 verdict: blocked. There are no StS1 event images to hash, load, or screenshot in tracked files.
