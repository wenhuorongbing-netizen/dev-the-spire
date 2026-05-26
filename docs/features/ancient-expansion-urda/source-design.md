# Urda Ancient Source Design v1

Status / authority note, 2026-05-25: this file is retained as Urda support evidence for the original vertical slice. Current behavior is governed by current source, `docs/test-ready-development-goal.md`, `docs/issues.md`, and `docs/features/ancient-expansion-v2.2/source-design.md`. In particular, v3.3 Seedbed and After the Rain supersede the older behavior in this document; fewer than 3 Act 1 After the Rain triggers grants 75 Gold.

## 1. One-line goal

Add a directly playable Urda vertical slice for private beta:

- one new Act 1 Ancient, `Urda, Loamweaver`;
- a safe active blessing pool;
- save/load-aware blessing state;
- no dependence on Morvi, Lotha, or Vakuu.

## 2. Scope boundaries

In scope:

- `EZMB_URDA` registration and visibility path.
- Urda blessing pool registration and gating.
- Blessing-specific hooks, save/load fields, command-safe effects.
- English and Simplified Chinese localization for active Urda items.
- Release-safe docs and manual verification matrix updates.

Out of scope:

- Morvi, Lotha, or Vakuu implementations.
- Ascension 11-20.
- Custom character systems.
- Additional unknown Ancient systems not explicitly listed below.

## 3. Urda Ancient Design

### 3.1 Urda identity

Ancient:

- Stable id: `EZMB_URDA`.
- English: `Urda, Loamweaver`.
- Simplified Chinese: `息壤织母·乌尔达`.
- Offer target: Act 1.

Acting rule:

- Urda should be offered through the current vanilla Ancient surface or a proven local custom-Ancient registration path.
- If direct custom-Ancient registration is not source-safe, Urda may remain behind a default-off debug/test forcing path with explicit documentation.

### 3.2 Blessings

The active v2.2 source pool contains eleven Urda blessings:

1. Seedbed (`urda_seedbed`, 苗床)
2. Humus Pact (`urda_humus_pact`, 腐殖约定)
3. Molting (`urda_molting`, 脱壳)
4. Moss Map (`urda_moss_map`, 苔痕地图)
5. Trial Branch (`urda_trial_branch`, 试炼枝条)
6. Shallow-Root Relic (`urda_shallow_root_relic`, 浅根遗物)
7. Elite Root (`urda_elite_root`, 精英根须)
8. Rooted Route (`urda_rooted_route`, 扎根路线)
9. After the Rain (`urda_after_rain`, 雨后)
10. Root-Sight (`urda_root_sight`, 根眼)
11. Seed Bank (`urda_seed_bank`, 种子库)

All eleven remain disableable through the Urda feature gate. Runtime testing must use the current source-safe behavior and deviations documented in the active goal/issues/v2.2/v3.3 docs rather than richer unproven UI from the original design.

### 3.3 Blessing behavior

#### Seedbed (`urda_seedbed`)

- Triggered by normal Act 1 combat card rewards.
- Offers a Seedbed reward alternative while the player has more than 2 max HP and fewer than four accepted Seedbed rewards.
- Tracks four accepted Seedbed choices; reward generation, reroll, and screen refresh do not spend a check by themselves.
- On accepting a reward, lose 2 max HP.
- On all four accepted rewards, set the transformed latch and gain +10 max HP with no heal.
- Current v3.3 behavior: the reward alternative adds the `Seedbed` card itself.
- In combat, `Seedbed` gives 8 / 12 Block and sets up 2 / 3 total Seedbed slots. Immediate planting can spend 1 / 2 of those slots on eligible draw/discard cards; any remaining slots catch later pollution.
- Eligible planted cards are Temporary Status cards, Temporary Curse cards, Blight Sprouts, and Rootblight. Permanent Curses, Withered Husk, and beneficial temporary pages are not planted.
- Planting does not play, discard, or exhaust the card and does not trigger those synergies. Each planted card adds one Withered Husk.
- A planted Blight Sprout is treated as handled for that combat and adds no Rootblight I after combat.
- A planted Rootblight pauses its combat-end check for that combat only. It stays in the master deck at the same stage and does not improve, worsen, split, get removed, downgrade, or get cleansed after combat.

#### Humus Pact (`urda_humus_pact`)

- Triggered by an explicit `Compost Reward` alternative on normal Act 1 combat card rewards.
- On each composted reward, gain 15 Gold.
- At three completed composts: after the card reward has completed, open a remove flow for 0, 1, or 2 card removals, then offer one unskippable upgraded card reward.
- The third payoff keeps a pending latch until payoff resolution succeeds; payoff card generation happens before optional removals so a no-card fallback cannot consume removals or silently drop the payoff.
- Ordinary reward-set skip/proceed and room-exit cleanup must not trigger Humus Pact.
- Apply once; do not repeat past completion.

#### Molting (`urda_molting`)

- On selection, remove one Strike and one Defend from deck, then add two `Withered Husk` cards.
- `Withered Husk` is a 0-cost Temporary Curse with Ethereal and Exhaust.
- Deck `Withered Husk` cards are removed at Act 2 start.
- When `Withered Husk` is exhausted, it grants 3 Block.

#### Moss Map (`urda_moss_map`)

- One-time per room type bonus within Act 1.
- Rewards are room-type keyed and source-backed.
- Bonus table:
  - normal combat: +25 Gold;
  - unknown/event: heal 5 HP;
  - shop: add one random potion if a potion slot is open;
  - elite: upgrade one random card if an upgradable card exists;
  - rest site: +3 max HP.
- Safe room-type resolution is required before release claiming.

#### Trial Branch (`urda_trial_branch`)

- On selection, offer four rare class cards through the source-safe card grid.
- Upgrade the chosen card, add it to the deck, and apply a visible Trial Branch enchantment backed by the `UrdaTrialPlantCard` marker.
- Track the next three combats; each combat succeeds only if the marked deck card is player-played at least once.
- Missing any one of those three combats removes the marked card. Three successful combats keep the card and clear the marker/enchantment.

#### Shallow-Root Relic (`urda_shallow_root_relic`)

- On selection, offer two common relics and grant the chosen relic plus 75 Gold.
- If the player defeats an Act 1 elite, root the relic permanently and grant 35 Gold.
- Source-safe deviation: if Act 2 starts before rooting, remove the pending relic and refund 75 Gold. The preferred `lose 6 Max HP to keep it` settlement UI is not exposed until source-safe.

#### Elite Root (`urda_elite_root`)

- First-tier Urda option relic.
- After each Elite combat, including Firemarked Elite combats, heal 10 HP.
- The heal uses a visible relic-payoff cue before the normal Core heal command.
- Source guard prevents healing a dead player, matching the safety shape of vanilla post-combat healing relics.

#### Rooted Route (`urda_rooted_route`)

- On selection, automatically mark a reachable normal-combat node in the first seven floors.
- Do not mutate the map graph.
- Reaching the mark grants three card rewards, gives a random potion when a slot exists, and upgrades the first generated reward card.
- If the route becomes unreachable before the mark resolves, the root withers: lose 8 HP and gain 25 Gold.

#### After the Rain (`urda_after_rain`)

- Act 1 only.
- Current v3.3 behavior: after the first unblocked enemy attack damage each combat, add one `Rain Breath` to hand.
- `Rain Breath` is Temporary, gains 5 Block, draws 1, and exhausts.
- At Act 2 start, fewer than 3 Act 1 triggers grants 75 Gold. Three or more triggers heals 8 HP and upgrades 1 card.

#### Root-Sight (`urda_root_sight`)

- Gain 5 Root Eyes.
- Current source path: clicking the Root Eyes relic opens map selection. The player may choose any future reachable Monster, Unknown, or Elite room. The chosen room stores its concrete enemy group or event and spends one Root Eye.
- The first mark grants one random potion if a slot exists.

#### Seed Bank (`urda_seed_bank`)

- During Act 1 normal combat card rewards, add a `Store Seed` alternative while fewer than three Seeds are stored.
- Source-safe deviation: storing consumes the current card reward and stores the selected reward card; it does not store one unchosen card after the player also takes another card.
- Before the Act 1 Boss, choose up to two Seeds. The first chosen Seed is upgraded and added to deck; any second chosen Seed is added without Trial Plant marking. Unchosen Seeds disappear.
- Unchosen Seeds disappear and settlement does not repeat.

## 4. State and persistence design

Current source packs Urda state into `AncientSavedStateFields.UrdaStateKey` on `Player` and mirrors that encoded string onto deck cards through `AncientSavedStateFields.UrdaDeckStateKey`. `AncientPlayerState` reads the Player field first, falls back to the first nonblank deck marker, and mirrors the restored state back to the deck.

Encoded fields:

- selected blessing id;
- Seedbed accepted-check count;
- Seedbed accepted reward count;
- Seedbed transformed latch;
- Humus compost count;
- Humus completed latch;
- Humus completion-pending latch;
- Molting active latch;
- Moss Map per-room-type reward flags;
- Trial Branch combat/success counters, played-this-combat latch, and settlement latch;
- Shallow-Root pending/rooted/relic id;
- Rooted Route coordinate/resolved/withered state;
- After the Rain per-combat trigger latch, Act 2 compensation latch, and trigger count;
- Root-Sight eye count, first-potion latch, and marked coordinates;
- Seed Bank stored card ids and settlement latch.

The parser accepts the prior eight-field shape for migration. `SavedSpireField<Player,string>` persistence is still not source-proven by this pass, and the card-backed mirror is a mitigation rather than release proof. State must survive live save/load before this design can be marked release-ready.

## 5. Localization and terms

All active Urda text must include EN + ZHS entries.

- Enforced by `docs/style/card-localization-style-guide.md` conventions.
- Use clean `[gold]` and no raw tags.
- Ensure visible keyword count and dynamic variables are mirrored.

## 6. Risk register

1. Unsafe Ancient registration API in v0.105.x can block release-ready claims.
2. Room-type identity changes can misfire across non-standard rooms.
3. `Withered Husk` temporary card behavior must not soft-lock removal, transformation, or upgrade.
4. Reward screen mutation must preserve reroll, skip, proceed, and room-exit flows.
5. Player-owned encoded state must be proven by live save/load or moved to a source-proven persisted carrier.

## 7. Out-of-scope release assertions

Urda is a private-beta playable slice.
Do not claim release-ready for Urda until:

- Urda registration and blessing pool are verified in live Act 1 selection;
- each active blessing passes manual checks;
- logs and save/load evidence are attached.
