# A11-A20 Development Checklist v2.0

Document type: executable feature GDD / development checklist  
Target version: A11-A20 high-Ascension expansion update  
Implementation style: modular, gated, phased, and suitable for Codex/program-agent execution  
Status: v2.0 implementation pass is in progress on 2026-05-07. Milestones 0-6 are build/source-guard proven behind independent feature flags and private-beta default-off A11-A20 selection. Milestone 7 now has source-guarded boss-specific Royal Seal hooks, reward metadata, and Boss-map hover text; all Royal Seal behavior still needs live boss verification. Milestone 8 uses the single-player vanilla double-boss map path for final-act Boss 1/Boss 2 reveal, Boss 2 Brand metadata/parameters, Boss-map Brand hover text, Boss 1 post-combat recovery, one Boss card reward after Boss 1, Boss 1 reward-screen intermission wording, and a fixed default-layout courtyard event inserted from the terminal reward proceed path with an immediate pre-finished-room save. A bespoke full custom intermission screen remains deferred. Live gameplay verification is still pending.

This v2.0 checklist supersedes the older v1.0 design direction for future development. Existing code still contains prototype slices that must be audited and migrated toward this spec before any release-readiness claim.

## Core Principle

Do not rewrite monster action tables. Do not rewrite Boss action tables.

Use map nodes, combat buffs, reward modifiers, status/curse cards, rest-site hooks, and event hooks to upgrade difficulty while keeping the system maintainable during Early Access.

## 1. Update Goal

A11-A20 should not be another round of HP and damage increases. It should create five clear high-Ascension systems:

| System | Gameplay responsibility |
| --- | --- |
| Rootblight | Long-term deck pollution and in-combat cleanup tempo |
| Firemarked Elites | High-risk, high-reward elite routing |
| Fission Enchantments | Reward judgment instead of auto-picking strong cards |
| Banner Rooms | Normal-fight objectives, formation pressure, and route choice |
| Boss Seals | Boss-specific mechanic upgrades without action-table rewrites |
| Dual King Brands | A20 endgame pressure through second-boss seal upgrades |

The ideal loss should be readable:

- I got greedy with Firemarked Elites.
- I ignored Rootblight and my deck became too dirty.
- I took Fission cards but lacked late-game sustain.
- I entered a Deep Branch without enough damage.
- I knew the second Boss had a Brand, but did not prepare for it in Act 3.

Avoid these outcomes:

- Opening hand disaster from Ascender's Bane + Rootblight + Blight Sprout.
- Bosses becoming stronger without clear player-facing explanation.
- Firemarked Elites feeling unrewarding.
- Every rest site becoming forced healing.
- Doormaker-style hard counters that make some decks feel unplayable.

## 2. A11-A20 Table

| Ascension | Name | Final effect |
| --- | --- | --- |
| A11 | Wide Tower, Long Road / 宽塔长路 | Maps become wider and longer. Act 1 gains +1 row, Act 2 gains +1 row, and Act 3 gains +2 rows to support later high-risk nodes. |
| A12 | Firemarked Elite Pack / 火印精英群 | Each act generates 2-3 Firemarked Elite candidates, spread out and avoidable. |
| A13 | Fission Enchantment / 裂变附魔 | Some Attack/Skill rewards can become Fission cards: cost -1, Exhaust after play. |
| A14 | Rootblight Begins / 根蚀初生 | Start with Rootblight I. Rootblight cards are real master-deck pollution and worsen after combat if ignored. |
| A15 | Boss Blight Sprout / 首领根芽 | Act 2/3 Boss fights bury two Blight Sprouts. They sprout on turns 3 and 4; each seen and unplayed Sprout adds Rootblight I after combat. |
| A16 | Banner Rooms / 战旗房 | Visible enhanced normal fights appear on the map with public combat rules. |
| A17 | Deep Branches / 深层支线 | Acts 2/3 contain optional high-risk, high-reward side branches. |
| A18 | Elite Blight Sprout / 精英根芽 | Mid/late Act 2 and Act 3 elites bury a Blight Sprout. |
| A19 | Boss Royal Seals / Boss 专属王印 | Each Boss gains a bespoke Royal Seal that strengthens its core mechanic without changing the action table. |
| A20 | Dual King Brands / 双王烙印 | Act 3 double-Boss information is revealed early; the second Boss's seal upgrades into a Brand; a fixed courtyard sits between the two Bosses. |

## 3. Implementation Boundaries

### 3.1 Do Not Implement

| Do not | Reason |
| --- | --- |
| Reduce route connections | Route choice is core Spire gameplay. Do not add difficulty by closing routes. |
| Reduce rest-site count | This forces resting and compresses deck-building space. |
| Reduce potion slots | Already covered by earlier Ascension pressure. |
| Keep raising shop/remove costs | Already covered and harms recovery. |
| Globally increase enemy HP/damage | Already covered by A8/A9-style axes. |
| Rewrite normal enemy AI | Too expensive during Early Access updates. |
| Rewrite Boss action tables | Every Boss update would require a full retest. |
| Read and counter the player's deck | Feels targeted and unfair. |
| Attach high penalties to high-value rewards | Makes rewards feel like traps. |
| Bind Act 4 / Shattered Star content to A20 | That should be an independent feature. |

### 3.2 Allowed Hook Surfaces

| Hook surface | Usage |
| --- | --- |
| `OnRunStart` | A14 Rootblight initial state |
| `OnMapGenerate` | A11, A12, A16, A17 map-node generation |
| `OnCombatStart` | Firemarks, banners, Boss seals, Blight Sprouts |
| `OnTurnStart` | Blight Sprout sprouting; ongoing Firemark/Banner effects |
| `OnFirstShuffle` | Light Banner/Seal interference |
| `OnCardEnterHand` | Blight Sprout tracking |
| `OnCardPlayed` | Rootblight cleanup, Sprout prevention, Seal/Banner triggers |
| `OnCombatEnd` | Blight Sprout growth and Rootblight state updates |
| `OnRewardGenerate` | Fission, Firemark rewards, Boss rewards |
| `OnRestSiteAction` | Forge Token payout and resting Rootblight cleanup |
| `OnHpThreshold` | Firemark/Royal Seal threshold effects |
| `OnMinionDeath` | Kin Priest, Queen, Kaiser Crab, and similar mechanics |
| `OnBossPhaseChange` | Test Subject, Doormaker, Ceremonial Beast, and similar mechanics |

## 4. Core Terms

### 4.1 Rootblight

Rootblight is a long-term deck pollution state. Multiple Rootblight cards can exist after repeated ignored Root Buds or ignored Rootblight III cards.

| Level | Master-deck display | Cost | Play effect |
| --- | --- | ---: | --- |
| 0 | None | - | - |
| I | Rootblight I / 根蚀 I | 2 | Remove this card after play |
| II | Rootblight II / 根蚀 II | 3 | Remove this card and add Rootblight I after combat |
| III | Rootblight III / 根蚀 III | 4 | Remove this card and add Rootblight II after combat |

Player text:

- Rootblight I: `Play: Exhaust. Permanently remove this card from your master deck. Growth: if not played or removed this combat, it becomes Rootblight II after combat.`
- Rootblight II: `Play: Exhaust. Permanently remove this card from your master deck and add Rootblight I after combat. Growth: if not played or removed, it becomes Rootblight III after combat.`
- Rootblight III: `Play: Exhaust. Permanently remove this card from your master deck and add Rootblight II after combat. Growth: if not played or removed, it adds one Rootblight I once.`

Cleanup rules:

| Action | Effect |
| --- | --- |
| Play Rootblight in combat | Remove that master-deck card and queue the downgrade described by the card text |
| Rest at a rest site | Remove one highest-stage Rootblight card |
| Remove Rootblight at a shop | Remove the selected Rootblight card |
| Special cleansing event | May set to 0 or reduce by 1, depending on event strength |
| Exhaust by a non-play effect | No level reduction |
| Discard | No level reduction |
| Transform | Defaults to clearing Rootblight, but must be balance-tested |

### 4.2 Blight Sprout

Blight Sprout is a temporary combat card.

It is not a Rootblight card and never persists in the master deck. If mishandled, it adds Rootblight I to the master deck.

Player text:

`Cost 2. Play: Exhaust. Sprout 3/4: At the start of that turn, if this card has not entered your hand, put it on top of your draw pile. At combat end, if this card entered your hand and was not played, add one Rootblight I to your master deck.`

Resolution table:

| State | Result |
| --- | --- |
| Never entered hand before combat ends | Withers; no growth |
| Entered hand and was played | No growth |
| Entered hand and was not played | Add Rootblight I |
| Discarded after entering hand, not played | Add Rootblight I |
| Exhausted by a non-play effect after entering hand | Add Rootblight I |
| Existing Rootblight already includes III | Add Rootblight I; Sprout still pressures 2 energy |

Design reason:

Blight Sprout starts in discard to avoid opening-hand death lotteries. Correct pacing is turns 1-2 setup, turn 3 sprout, then a decision between paying 2 energy to clear it or preserving combat tempo and accepting Rootblight growth.

### 4.3 Forge Token

Forge Token is an upgrade-tempo protection reward from high-risk nodes.

Max held: 1.

Duplicate tokens convert into small healing or small gold.

Rest-site payout:

| Rest-site action | Forge Token effect |
| --- | --- |
| Rest | After resting, upgrade 1 Common/Uncommon upgradable card |
| Smith | After smithing, heal a small amount |
| Special rest-site action | Deferred in the current build until a safe runtime API is proven |

Design purpose:

After beating a Firemarked Elite, a player who lost HP should not be forced into only resting for the rest of the act.

### 4.4 Fission Enchantment

Fission is a reward-card modifier.

Text:

`Cost -1. Exhaust after play.`

Eligibility:

| Condition | Requirement |
| --- | --- |
| Card type | Attack or Skill only |
| Power cards | Not allowed |
| Original cost | At least 1 |
| Existing Exhaust | Not allowed |
| X-cost | Not allowed |
| Special/quest cards | Not allowed |
| Unmodifiable cards | Not allowed |
| Existing strong enchantment | Default not allowed |

## 5. A11: Wide Tower, Long Road

Player text:

`Maps are wider. Act 2 and Act 3 routes are longer.`

Map changes:

| Act | Change |
| --- | --- |
| Act 1 | Width +1, no total floor increase |
| Act 2 | Width +1, total floors +1 |
| Act 3 | Width +1, total floors +1; may become +2 if run length is acceptable |

Protection rules:

- No extra pressure in the first 5 Act 1 floors.
- No forced elite before the first rest site.
- No forced Firemarked Elite before the first rest site.
- No forced Banner Room before the first rest site.
- Added floors should mostly be mid/late.
- Each act must retain at least one low-risk route.
- The inserted width column must contain at least one reachable optional route node, not only wider spacing.

Design purpose:

A11 may be slightly easier by itself. Its job is to create enough space for A12-A20 risks without compressing the player.

## 6. A12: Firemarked Elite Pack

Player text:

`Each act contains multiple Firemarked Elites. Firemarked Elites have powerful Firemarks and grant better rewards when defeated.`

Candidate counts:

| Act | Firemarked Elite candidates |
| --- | ---: |
| Act 1 | 2 |
| Act 2 | 3 |
| Act 3 | 3 |

If map placement fails, the minimum fallback is 2 candidates. These are candidates, not forced fights.

Hard placement rules:

1. Never before the first Act 1 rest site.
2. Never on the only route.
3. Never two on the same floor.
4. Firemarked Elites cannot be adjacent to each other.
5. A route cannot force a Firemarked Elite before the first rest site.

Soft placement goals:

| Goal | Description |
| --- | --- |
| Spread | Prefer different routes |
| Greedy route | At least one route can plan for 2 Firemarked Elites |
| Safe route | At least one route can fight 0-1 Firemarked Elites |
| Deep Branches | Deep Branches may contain Firemarks but cannot absorb all Firemarks |

### 6.1 Firemark Host

Only one enemy in a Firemarked Elite fight is the Firemark Host.

The host receives the complete Firemark. Other enemies do not, unless the Firemark explicitly defines secondary-target effects.

Encounter config field:

```json
{
  "encounter_id": "elite_xxx",
  "fire_mark_host": "enemy_main"
}
```

Fallback host rules:

1. Highest max HP non-summon enemy.
2. If tied, choose the enemy with an elite tag.
3. If still tied, choose the leftmost non-summon enemy.

Production content should use explicit config wherever possible.

### 6.2 Firemark Types and Tuning

Firemarks should be clearly strong. Do not use weak effects like turn-5 +1 Strength.

| Firemark | Act 1 | Act 2 | Act 3 |
| --- | ---: | ---: | ---: |
| Might Mark / 烈力火印 | Host starts with +2 Strength | +3 Strength | +4 Strength |
| Giant Mark / 巨躯火印 | Host max HP +30% | +30% | +30% |
| Forge Armor Mark / 铸甲火印 | Host gains 8 Block at end of each turn | 13 Block | 18 Block |
| Constant Heal Mark / 恒愈火印 | Host heals 6 HP at end of each turn | 10 HP | 14 HP |

Notes:

- Constant Heal is fixed healing, not a regeneration concept.
- Forge Armor is fixed recurring Block, not one-time Block.
- In multi-enemy fights, non-host enemies do not receive the full Firemark.
- Might Mark may optionally give non-summon secondary targets +1 Strength in Acts 2/3; keep disabled for first implementation.

### 6.3 Firemark Rewards

Each Firemarked Elite gives:

1. Normal elite rewards.
2. Card reward +1 option.
3. Firemark reward settlement.

Settlement:

| State | Reward |
| --- | --- |
| Player has no Forge Token | Gain 1 Forge Token |
| Player already has a Forge Token | At least 1 upgraded card in the reward and small gold |
| Third Firemarked Elite in the act | Slightly increased rare-card weight, but no extra Forge Token |

## 7. A13: Fission Enchantment

Player text:

`Some Attack or Skill rewards may appear with Fission. Fission cards cost less, but Exhaust after play.`

Initial chance table:

| Source | Fission candidate chance |
| --- | ---: |
| Normal combat reward | 10% |
| Banner Room reward | 15% |
| Firemarked Elite reward | 20% |
| Boss reward | 5% |

Each reward screen may contain at most one Fission card.

UI requirements:

- Cracked enchantment frame.
- Fission keyword.
- Clear cost-change display.
- Clear Exhaust display.

Design purpose:

Fission is a reward decision, not a punishment. The player evaluates whether the reduced cost solves an immediate need, whether Exhaust harms long-term sustain, whether it is a transition tool or a core-loop pollutant, and who should take a one-shot key card in multiplayer.

## 8. A14: Rootblight Begins

Player text:

`At the start of a run, gain Rootblight I.`

Rule:

```text
rootblight_level += 1
```

At run start, add Rootblight I to the master deck. Later Root Buds add additional Rootblight I cards when seen and ignored.

## 9. A15: Boss Blight Sprout

Player text:

`At the start of Act 2 and Act 3 Boss combats, bury 2 Blight Sprouts in the discard pile. They sprout on turns 3 and 4; each one you see and do not play adds Rootblight I after combat.`

Trigger table:

| Scene | Add Blight Sprout |
| --- | --- |
| Act 1 Boss | No |
| Act 2 Boss | Yes |
| Act 3 Boss | Yes |
| A20 double-Boss first fight | Yes |
| A20 double-Boss second fight | No, to avoid double imprisonment |

## 10. A16: Banner Rooms

Player text:

`Banner Rooms appear on the map. Banner Rooms are enhanced normal combats with public combat rules.`

Spawn counts:

| Act | Count |
| --- | ---: |
| Act 1 | At most 1, not before the first rest site |
| Act 2 | 1-2 |
| Act 3 | 1-2 |

Banner types:

1. Vanguard Banner: At combat start, all enemies gain 2 temporary Strength. Remove this temporary Strength at the start of turn 3.
2. Shield Formation Banner: At combat start, choose a random non-summon enemy as bannerbearer. While alive, other enemies gain a small amount of Block at the start of each turn. When the bannerbearer dies, other enemies gain a small one-time Block.
3. Bounty Banner: At combat start, mark one bounty enemy. If players kill it before the end of turn 3, gain extra gold after combat. If not, the bounty enemy gains small Block and 1 Artifact.
4. Pressure Banner: Each turn, when the player plays the 6th card, all enemies gain a small amount of Block. Once per turn.
5. Rout Banner: When the first enemy dies, remaining enemies gain Rout: Block this turn and small Strength next turn. Once per combat.

First implementation batch:

1. Vanguard Banner.
2. Shield Formation Banner.
3. Bounty Banner.

## 11. A17: Deep Branches

Player text:

`Acts 2 and 3 contain Deep Branches. Deep Branches are higher risk and higher reward.`

Rules:

- Generate 1 Deep Branch in Act 2 and 1 in Act 3.
- Branch length: 3-4 nodes.
- Entrance uses a rift border.
- Contains at least 1 risk node: Banner Room or Firemarked Elite.
- Contains at least 1 enhanced reward node.
- Exit must reconnect to the main map.
- The same act must retain a normal route that skips the branch.

Example branches:

```text
Normal Combat -> Banner Room -> Firemarked Elite -> Chest -> Rejoin main route
```

```text
Banner Room -> Shop -> Firemarked Elite -> Enhanced Card Reward -> Rejoin main route
```

Deep rewards:

| Reward node | Enhancement |
| --- | --- |
| Card reward | +1 option or higher upgraded-card odds |
| Chest | Higher mid/high-quality relic odds |
| Shop | May show 1 discount slot without changing all prices |
| Firemarked Elite | Normal Firemark reward |
| Banner Room | Slightly higher Fission chance |

## 12. A18: Elite Blight Sprout

Player text:

`At the start of mid/late Act 2 and Act 3 Elite combats, bury 1 Blight Sprout in the discard pile.`

Trigger table:

| Condition | Trigger |
| --- | --- |
| Act 1 Elite | No |
| First 3 Act 2 floors | No |
| Mid/late Act 2 Elite | Yes |
| Act 3 Elite | Yes |
| Firemarked Elite | Yes, but at most 1 Blight Sprout per combat |

## 13. A19: Boss Royal Seals

Player text:

`Each Boss gains a Royal Seal. Royal Seals strengthen the Boss's core mechanics without changing its original action logic.`

Royal Seals only listen to existing mechanics:

- Stun.
- Intangible.
- Wake-up.
- Slippery removal.
- Minion death.
- Summon death.
- Phase change.
- Door broken.
- Bound / Chains.
- Sandpit / Escape.
- Steam / Explosion.

Royal Seals must not:

- Write new Boss action tables.
- Force Boss action order changes.
- Add extra draw prevention.
- Add extra card-cost increases.
- Add extra Exhaust on played cards.
- Read the player's deck.

### 13.1 Act 1 Overgrowth Boss Seals

Ceremonial Beast, Holy Daze / 圣昏:

- Trigger: first stun.
- Effect: during the first stun turn, all damage taken becomes 1. Remove at end of turn. After Holy Daze ends, Boss gains 1 Strength.
- Intent: the first stun becomes a setup window instead of a burst window.

Kin Priest, Martyr Oath / 殉誓:

- Trigger: Kin Follower death.
- Effect: each Follower death gives Kin Priest 12 Block and 1 Strength. If Kin Priest is below 50% HP, also gain 1 Artifact. Max 2 triggers per combat.
- Intent: killing followers lowers board pressure but strengthens Priest.

Vantom, Ink Return / 回墨:

- Trigger: Slippery fully removed for the first time.
- Effect: at the start of next turn, Vantom restores 2 Slippery and gains 1 Strength. Once per combat.
- Intent: strengthen the slippery theme and require multi-hit/burst planning.

### 13.2 Act 1 Underdocks Boss Seals

Lagavulin Matriarch, Startled Shell / 惊醒壳:

- Trigger: Boss wakes up.
- Effect: if naturally awakened, gain 8 Plating layers. If awakened early by the player, gain 4 Plating layers. After the first Soul Siphon, halve Plating layers.
- Intent: natural wake-up gives more preparation but a harder shell; early wake-up is riskier but softer.

Soul Fysh, Soul Tide / 灵潮:

- Trigger: entering Intangible and Beckon turn settlement.
- Effect: each time Soul Fysh enters Intangible, gain 1 Artifact. At end of turn, for each Beckon that settles in player hand, Soul Fysh gains 2 Block at the start of next turn, up to 12 Block per turn.
- Intent: strengthen Beckon / Intangible rhythm without generic Block spam.

Waterfall Giant, Boiling Critical / 沸腾临界:

- Trigger: Steam Eruption threshold and death explosion.
- Effect: every 12 Steam Eruption stacks, Boss gains 1 Boiling. Death explosion deals additional damage equal to Boiling x 2. At the start of the explosion turn, the player gains temporary Block equal to Boiling x 2.
- Intent: long fights make explosion more dangerous, but the explosion turn is clearly telegraphed and slightly compensated.

### 13.3 Act 2 Boss Seals

Kaiser Crab, Misaligned Shell / 错位甲:

- Trigger: back attack hit and claw death.
- Effect: each turn's first back attack hit gives the target claw 6 Block. When the first claw dies, the other gains 1 Artifact. If both claws die in the same turn, extra Artifact does not trigger.

Knowledge Demon, Marginal Note / 旁注:

- Trigger: player chooses Curse of Knowledge.
- Effect: after each curse choice, another unchosen curse becomes a Marginal Note and is shuffled into discard.
- Marginal Note: 0 cost, Retain. Play: Exhaust. Draw 1. Removed after combat.
- If Marginal Note ends the turn in hand unplayed, Knowledge Demon gains 1 Strength.

The Insatiable, Struggle Bait / 挣扎饵:

- Trigger: Boss gains Strength, heals, or devour-like self-enhancement triggers.
- Effect: each self-enhancement shuffles 1 Frantic Escape into player discard. It cannot be drawn this turn. If the player has already played 3+ Frantic Escapes this combat, The Insatiable gains 1 extra Strength.

### 13.4 Act 3 Glory Boss Seals

Doormaker, Door Wedge / 门楔:

- Trigger: after a Door is broken and Doormaker is revealed.
- Effect: during Doormaker's first revealed turn, gain Door Wedge. While active, Doormaker can take at most 40 damage per hit. Every 3rd Attack played removes Door Wedge. Once per reveal.
- Constraint: do not add draw, cost, or Exhaust restrictions.

Queen, Chosen Decree / 择令:

- Trigger: Queen applies Chains of Binding / Bound.
- Effect: one of the 3 Bound cards gains Royal Decree. If the player plays it this turn, Torch Head Amalgam's next Strength gain is reduced by 1, minimum 0. If not, Queen gains 10 Block and Amalgam gains 1 Strength.
- Multiplayer cap: Amalgam can gain at most 2 Strength per turn from this.

Test Subject, Residual Sample / 残留样本:

- Trigger: phase change.
- Effect: on each new phase, keep 1 weakened sample from the previous phase. Show samples in UI before they apply. Max 2 retained samples per combat.

Sample pool:

| Previous phase tendency | Weakened sample |
| --- | --- |
| Defensive | New phase starts with small Block |
| Offensive | New phase gains 1 Strength on turn 2 |
| Interference | First shuffle adds 1 short-lived status card |
| Adaptive | First time receiving a debuff, gain 1 Artifact |

## 14. A20: Dual King Brands

Player text:

`Act 3 double-Boss information is revealed early. The second Boss's Royal Seal upgrades into a Brand. A fixed courtyard sits between the two Bosses.`

A20 does three things:

1. Reveal Act 3 Boss 1 and Boss 2 information at the start of Act 3.
2. Upgrade the second Boss's Royal Seal into a Brand.
3. Add a fixed courtyard between Boss 1 and Boss 2.

Current implementation note: Boss 1/Boss 2 reveal, Boss 2 Brand metadata/parameters, Boss 1 recovery, Boss card reward, Boss 2 warning text, fixed courtyard event, immediate pre-finished-room save, and duplicate Boss 2 Blight Sprout suppression are source-guarded through the single-player vanilla double-boss map/reward path. Boss 2 Brand metadata is gated by A20 itself and is not dependent on the A19 Boss Seal feature flag. The current courtyard is a default-layout event room inserted from the terminal reward proceed path; a bespoke full custom intermission screen remains deferred until live runtime verification proves it is needed and safe.

Revealed information:

| Information | Display |
| --- | --- |
| Boss 1 | Name and icon |
| Boss 2 | Name and icon |
| Boss 1 Royal Seal | Name and description |
| Boss 2 Royal Seal | Name and description |
| Boss 2 Brand | A20 upgrade description |

Brand examples:

| Boss | Royal Seal | A20 Brand |
| --- | --- | --- |
| Doormaker | Door Wedge removed by 3rd Attack | Removed by 4th Attack, but per-hit cap increases to 50 |
| Queen | Missing Royal Decree gives Queen Block and Amalgam Strength | Higher Queen Block; playing Royal Decree also gives small Block reward |
| Test Subject | Each phase keeps 1 weakened sample | First phase change keeps 2 weakened samples, then 1 afterward |
| Soul Fysh | Intangible Artifact; Beckon gives Block | Intangible Artifact +1; Beckon Block cap increased |
| Ceremonial Beast | First stun Holy Daze | Holy Daze grants 2 Strength after ending |
| The Insatiable | Self-enhancement adds Frantic Escape | If new Frantic Escape is not played within 2 turns, Boss gains small Block |

Courtyard after Boss 1:

| Item | Effect |
| --- | --- |
| Heal | Restore 25% of missing HP |
| Reward | Boss card reward |
| Warning | Show Boss 2 Royal Seal and Brand again |
| Blight Sprout | In A20 double-Boss sequence, only Boss 1 gets Blight Sprout; Boss 2 does not |

## 15. Multiplayer Rules

Rootblight:

- Each player has independent Rootblight level.
- Each player resolves their own Blight Sprout.
- Teammates cannot play your Rootblight or Blight Sprout.
- Teammates can use defense, healing, and support to help you survive cleanup turns.
- Knockout/revive does not clear permanent Rootblight.
- On knockout/revive, combat-only Blight Sprouts are removed to avoid post-revive imprisonment.
- If 4-player pressure is too high, Blight Sprout may affect only `ceil(player_count / 2)` players.

Strong negative effect cap:

| Player count | Max affected players |
| --- | ---: |
| 1 | 1 |
| 2 | 1 |
| 3 | 2 |
| 4 | 2 |

Firemarks and Banners:

- Firemark Host is shared by the team.
- Banner targets are shared by the team.
- Bounty Banner reward is team-shared gold or small gold per player, depending on co-op economy rules.
- If multiplayer route voting ties, default to the lower-risk route.

## 16. Data Structure Draft

Rootblight state:

```json
{
  "player_id": "p1",
  "rootblight_level": 0,
  "has_rootblight_card": false,
  "active_sprout_id": null
}
```

Blight Sprout state:

```json
{
  "card_id": "blight_sprout_temp",
  "sprout_turn": 3,
  "has_entered_hand": false,
  "was_played": false,
  "source": "boss|elite|event"
}
```

Firemarked Elite node:

```json
{
  "node_type": "elite",
  "variant": "fire_mark_elite",
  "fire_mark_type": "might|giant|forge_armor|constant_heal",
  "fire_mark_host": "enemy_main",
  "reward_profile": "fire_mark_reward_v1"
}
```

Banner Room node:

```json
{
  "node_type": "monster",
  "variant": "banner_room",
  "banner_type": "vanguard|shield_formation|bounty|pressure|rout"
}
```

Boss Royal Seal:

```json
{
  "boss_id": "doormaker",
  "seal_id": "door_wedge",
  "hooks": ["on_door_broken", "on_boss_reveal", "on_card_played"],
  "params": {
    "single_hit_cap": 40,
    "attacks_to_remove": 3
  },
  "a20_brand_params": {
    "single_hit_cap": 50,
    "attacks_to_remove": 4
  }
}
```

## 17. Milestone Task Packages

Each milestone should be developed on its own branch, tested independently, and merged only after review.

### Milestone 0: Base Hooks and Feature Flags

Goal: establish base switches and event hook discovery for A11-A20.

Tasks:

1. Add `AscensionExpansionConfig`.
2. Add feature flags:
   - `enable_rootblight`
   - `enable_fire_mark_elites`
   - `enable_fission_enchant`
   - `enable_banner_rooms`
   - `enable_deep_branch`
   - `enable_boss_seals`
   - `enable_dual_king_brand`
3. Confirm hook availability:
   - `OnRunStart`
   - `OnMapGenerate`
   - `OnCombatStart`
   - `OnTurnStart`
   - `OnFirstShuffle`
   - `OnCardEnterHand`
   - `OnCardPlayed`
   - `OnCombatEnd`
   - `OnRewardGenerate`
   - `OnRestSiteAction`
   - `OnHpThreshold`
   - `OnBossPhaseChange`

Acceptance:

- Each feature flag can be toggled independently.
- With all flags disabled, game behavior equals current behavior.
- Each hook has minimal QA-readable log output.

### Milestone 1: Rootblight and Blight Sprout

Goal: implement the core A14/A15/A18 pollution system.

Tasks:

1. Implement `RootblightState`.
2. Implement Rootblight I/II/III display cards.
3. Implement temporary Blight Sprout card.
4. Implement Sprout 3/4.
5. Implement combat-end growth.
6. Implement resting removes one highest-stage Rootblight.
7. Implement shop removal removes the selected Rootblight.
8. Ensure non-play Exhaust does not lower Rootblight level.

Acceptance cases:

| Case | Expected |
| --- | --- |
| A14 start | Rootblight I is added to the master deck |
| Play Rootblight I | Card is removed from deck with no replacement |
| Play Rootblight II | Card is removed from deck and Rootblight I is added after combat |
| Play Rootblight III | Card is removed from deck and Rootblight II is added after combat |
| Sprout never enters hand before combat ends | No growth |
| Sprout enters hand and is not played | Add Rootblight I |
| Sprout is discarded and not played | Add Rootblight I |
| Rest | One highest-stage Rootblight is removed |
| Shop-remove Rootblight | Selected Rootblight is removed |

### Milestone 2: Firemarked Elite Pack and Host

Goal: implement A12 map generation, Firemark Hosts, and Firemark rewards.

Tasks:

1. Generate Firemarked Elite nodes.
2. Use 2/3/3 candidates by act.
3. Apply spread rules.
4. Configure Firemark Host.
5. Implement four Firemarks:
   - Might
   - Giant
   - Forge Armor
   - Constant Heal
6. Implement Firemark rewards:
   - Card reward +1
   - Forge Token
   - Replacement reward if token already held

Acceptance:

- Act 1 Firemarks do not appear before the first rest site.
- Firemarks do not appear on the only route.
- Firemark Host displays correctly.
- In multi-enemy elites, only the host receives the full Firemark.
- Firemark victory rewards are correct.
- Existing Forge Token prevents stacking a second token.

### Milestone 3: Forge Token

Goal: implement upgrade-tempo protection after high-risk nodes.

Tasks:

1. Add `forge_token_count` with max 1.
2. Display Forge Token in rest-site UI/state.
3. Rest then extra-upgrade.
4. Smith then extra-heal.
5. Special action then small heal is deferred until a safe runtime API is proven.
6. Convert duplicate Forge Token.

Acceptance:

- Rest + Forge Token upgrades 1 Common/Uncommon card.
- Smith + Forge Token heals.
- Token is cleared after payout.
- Special rest-site actions do not use an unsafe private `RestSiteSynchronizer.ChooseOption` wrapper.
- Multiplayer resolves per player.

### Milestone 4: Fission Enchantment

Goal: implement A13 reward-card modification.

Tasks:

1. Card filter:
   - Attack/Skill only
   - Original cost >= 1
   - Not Power
   - Not already Exhaust
   - Not X-cost
   - Not special/quest
2. Fission modifier:
   - Cost -1
   - Add Exhaust
3. Reward-generation chance.
4. UI cracked frame and keyword.

Acceptance:

- Power cards never Fission.
- 0-cost cards never Fission.
- Existing Exhaust cards never Fission.
- Fission cards Exhaust after play.
- At most one Fission card per reward screen.

### Milestone 5: Banner Rooms

Goal: implement A16 enhanced normal-combat nodes.

First batch:

1. Vanguard Banner.
2. Shield Formation Banner.
3. Bounty Banner.

Tasks:

1. Generate Banner Rooms on map.
2. Add banner icon and hover text.
3. Attach banner modifier.
4. Mark bounty enemy and grant reward.
5. Mark bannerbearer and resolve death.

Acceptance:

- Banner Room is visible before entry.
- Vanguard temporary Strength is removed on turn 3.
- Shield Formation gives other enemies Block while bannerbearer lives.
- Bounty Banner grants extra gold if killed before turn 3 ends.
- Banners do not modify monster action tables.

### Milestone 6: Deep Branches

Goal: implement A17 high-risk map branches.

Tasks:

1. Generate Deep Branch in Acts 2/3.
2. Branch length 3-4.
3. At least one risk node.
4. At least one enhanced reward node.
5. Branch reconnects to main route.
6. Same act retains safe alternative route.

Acceptance:

- Deep Branch is not the only route.
- Branch border is clear.
- Branch contains both risk and reward.
- Does not significantly increase early Act 1 deaths.

### Milestone 7: Boss Royal Seals

Goal: implement A19 Boss-specific mechanism hooks.

Tasks:

1. Add `BossSealDefinition` data table.
2. First implement 3 low-risk prototypes:
   - Ceremonial Beast: Holy Daze
   - Soul Fysh: Soul Tide
   - Waterfall Giant: Boiling Critical
3. Validate data-driven hooks.
4. Extend to all Bosses.
5. Boss reward +1 option.

Acceptance:

- Boss action tables are not modified.
- Royal Seal trigger has UI notice.
- Each Seal binds only to its Boss.
- Boss reward +1 works.
- Doormaker Seal does not add draw/cost/Exhaust restrictions.

### Milestone 8: A20 Dual King Brands

Goal: implement Act 3 double-Boss information reveal, courtyard, and second-Boss Brand mode.

Current implementation note: the available slice uses the vanilla second-Boss map path and terminal reward-screen pause, then opens a fixed default-layout courtyard event before Boss 2. Boss 2 Brand parameters and the courtyard insertion path are source-guarded; a bespoke full custom intermission screen is not implemented.

Tasks:

1. Reveal Boss 1 / Boss 2 at Act 3 start.
2. Show both Royal Seals.
3. Switch Boss 2 Seal to A20 Brand params.
4. Boss 1 courtyard:
   - Restore 25% missing HP.
   - Boss card reward.
   - Show Boss 2 Brand again.
5. A20 double-Boss sequence adds Blight Sprout only to Boss 1.

Acceptance:

- Boss information is visible at Act 3 start.
- Boss 2 uses Brand params.
- Courtyard does not open a menu.
- Boss 2 does not bury a second Blight Sprout.
- Multiplayer heals and rewards each player independently.

## 18. Telemetry and Balance Metrics

Rootblight:

| Metric | Target |
| --- | --- |
| Average Rootblight level | Should not sit near III long-term |
| In-combat cleanup rate | Must be meaningful, not all rest-based |
| Rest-to-cleanse rate | Too high means resting is forced |
| Sprout play rate after hand entry | Checks if turn-3 decision works |
| Rootblight-related death rate | Must not create unwinnable runs |

Firemarks:

| Metric | Target |
| --- | --- |
| Average Firemarked Elites challenged per act | About 0.8-1.5; strong players can exceed |
| Firemarked Elite win rate | Not so low that they become traps |
| Next rest-site choice after Firemark | Checks if Forge Token protects upgrades |
| Win rate after 2+ Firemarks in one act | Prevent reward over-scaling |

Fission:

| Metric | Target |
| --- | --- |
| Fission appearance rate | Matches config |
| Fission pick rate | Neither auto-pick nor ignored |
| Average Fission card plays | Should feel close to one-shot burst |
| Fission-card win-rate delta | Too high means overpowered; too low means trap |

Banners:

| Metric | Target |
| --- | --- |
| Banner Room entry rate | Players should not always avoid them |
| Bounty completion rate | Variable, not always free or impossible |
| Shield Formation target choice | Check if bannerbearer is always killed first |
| Vanguard early HP loss | Must not create early death spikes |

Boss Seals:

| Metric | Target |
| --- | --- |
| Death contribution per Boss Seal | Find overpowered Seals |
| Seal trigger turn | Avoid too-early burst punishment |
| Player preparation behavior | Should affect card picks, smithing, potions |
| Doormaker Seal feedback | Must not deepen deck hard-counter feeling |

A20:

| Metric | Target |
| --- | --- |
| Average HP after Boss 1 | Measures pre-courtyard pressure |
| Boss 2 win rate after courtyard | Measures Brand strength |
| Boss 2 Brand death contribution | Finds overpowered Brands |
| A20 run duration | Must not become fatigue-length |

## 19. Recommended Development Order

1. Rootblight and Firemarks first.
   - These create the most important new feel: long-term pollution and high-risk routing.
2. Fission and Banners second.
   - These affect reward judgment and normal-combat routing without depending on Boss data.
3. Deep Branches third.
   - Only combine Firemarks and Banners into route structures after both are stable.
4. Boss Royal Seals fourth.
   - Start with 3 Boss prototypes, then expand. Doormaker should be last because it is the easiest to make oppressive.
5. A20 Dual King Brands fifth.
   - Only after Boss Seals are stable, because A20 magnifies Seal problems.

## 20. Final Concept

A11 gives space.  
A12 gives route temptation.  
A13 gives reward judgment.  
A14-A18 give the Rootblight axis.  
A16 gives normal-fight objectives.  
A19 gives Boss-theme strengthening.  
A20 gives the final Brand test.

One-sentence summary:

This update should not make enemies simply thicker and more painful. It should make players make harder, clearer, more learnable decisions about routes, rewards, deck pollution, combat objectives, and Boss preparation.

## External References Provided With Design Input

- [Mega Crit FAQ](https://www.megacrit.com/faq/)
- [PCGamesN Ascension overview](https://www.pcgamesn.com/slay-the-spire-2/ascension-levels)
- [Mobalytics boss overview](https://mobalytics.gg/slay-the-spire-2/encounters/bosses)
- [Untapped v0.103.2 patch notes](https://sts2.untapped.gg/en/articles/slay-the-spire-2-v01032-patch-notes-main-branch-update)
