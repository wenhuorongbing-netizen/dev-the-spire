# Ascension 11-20 Source Design v1.0

Current checklist: `development-checklist-v2.md` is now a compact active triage file. The full v2.0 planning draft from 2026-05-07 is archived at `docs/archive/feature-inputs/ascension-11-20/development-checklist-v2-full-20260518.md`. This v1.0 source-design document remains historical design context for existing prototype slices.

Document type: Feature GDD / local source design
Project: Spire Plus workspace (`EZMicroBalance` manifest id), with legacy EzDailyContent scaffold preserved for traceability
Original target game: Slay the Spire 2 public beta v0.104.0, 2026.04.23
Current implementation target: Slay the Spire 2 public beta v0.107.1 runtime, with historical v0.106.1 source-refreshed context from 2026-05-22 and retained v0.107.0 loader context from beta.85/beta.87
Current dependency baseline: STS2-RitsuLib v0.4.28 / lib 0.107.1; BaseLib is historical/previous-package context only
Status: design source only; not implementation proof

## 1. One-line Goal

A11-A20 should expand high-Ascension difficulty through visible route pressure, long-term deck pressure, optional high-risk nodes, and boss modifiers, without rewriting enemy AI or boss action tables.

Final design sentence:

> This A11-A20 system compresses mistake tolerance, not deck-building space. It adds visible risk, long-term tradeoffs, and route decisions, not hidden punishment, AI rewrites, or menu hell.

## 2. Design Boundaries

Do not duplicate A1-A10 pressure axes:

| Avoid | Reason |
| --- | --- |
| More potion-slot loss | Already covered by early Ascension pressure. |
| More gold reduction | Already covered by early Ascension pressure. |
| More shop/remove inflation | Already covered and harms recovery space. |
| Fewer rest sites | Compresses upgrade space too hard. |
| Global enemy HP increase | Already an early difficulty axis. |
| Global enemy damage increase | Already an early difficulty axis. |
| Third full Act 3 boss | A10 already has double boss pressure. |
| Large enemy AI rewrites | Early Access update risk is too high. |
| Boss action-table rewrites | Boss update maintenance cost is too high. |

## 3. Design Principles

- Difficulty must be visible before commitment whenever possible.
- High-risk nodes should be avoidable, not forced into the only path.
- The player should fail because of route, resource, deck, and risk decisions, not because of hidden post-choice randomness.
- Early turns should not become death lotteries.
- Recovery and rebuild space must remain available for skilled play.
- Multiplayer penalties must be capped so 3-4 player runs do not become shared punishment loops.
- Prefer hook-based dedicated modifiers over enemy AI or action-table rewrites.

## 4. A11-A20 Overview

| Ascension | Name | Player-facing effect | Design role |
| --- | --- | --- | --- |
| A11 | Wide Tower, Long Road / 宽塔长路 | Maps become wider and longer: width +1, Act 1 +1 route row, Act 2 +1 route row, and Act 3 +2 route rows. No A11-specific marker/icon/hover tooltip. | Creates route and growth space for later risk nodes while ordinary route nodes still look vanilla. |
| A12 | Firemarked Elites | About 3 visible optional firemarked elites per act, route-selected so a normal path should contain at most 1; defeating one grants a Forge Token. | Optional high-risk/high-reward node. |
| A13 | Fission Enchantments | Some Attack and Skill rewards cost 1 energy less and gain Exhaust. | Reward judgment pressure. |
| A14 | Root Begins | Start with one removable Root affliction in the deck. | Long-term deck pressure. |
| A15 | Boss Root Bud | Bosses bury a Root Bud that sprouts around turn 3. | Boss rhythm and long-term tradeoff. |
| A16 | Banner Rooms | Visible enhanced normal fights appear on the map. | Route pressure in normal combats. |
| A17 | Deep Branches | Acts 2/3 can contain optional high-risk/high-reward branches. | Growth ceiling and route mastery. |
| A18 | Elite Root Bud | Act 2/3 elites bury a Root Bud. | Elite fights create long-term risk. |
| A19 | Boss Dedicated Abilities | Each Boss gains its own visible dedicated ability; boss card rewards improve. | Bosses ask more of their own existing mechanics. |
| A20 | Branded Form | Act 3 double bosses are revealed; only the second Boss upgrades its dedicated ability into Branded Form; intermission exists between bosses. | Endgame sustained build test with one strengthened final Boss. |

## 5. Core Systems

### 5.1 Root

Root is a permanent affliction pressure card.

Initial design:

- Cost: 2
- Type: Affliction card
- Text: Remove this from your deck. Visible Exhaust comes from the card keyword line, not repeated body text.
- Permanent removal sources: playing Root, card removal, rest-site rest cleanup, compatible events.
- Non-permanent sources: normal Exhaust effects, discard, temporary combat removal.

Design purpose:

- Let the player choose between spending combat tempo, rest-site value, card removal, or delayed deck pollution.

### 5.2 Deep Root

Deep Root is the upgraded Root state.

Initial design:

- Cost: 3
- Type: Affliction card
- Text: When played, remove this from your deck. Visible Exhaust comes from the card keyword line, not repeated body text.
- Max one Root-family permanent card at a time.
- Root growth upgrades Root to Deep Root, but Deep Root does not stack further.

### 5.3 Root Bud

Root Bud is a temporary combat pressure card used by boss/elite encounters.

Initial design:

- Cost: 2
- Type: temporary affliction card
- Starts in discard pile, not opening hand.
- Sprout 3: around turn 3, if it has not entered hand, move it to the top of draw pile.
- If combat ends before it enters hand, it withers and does not grow Root.
- If it enters hand and is not played before combat ends, Root grows.
- Playing Root Bud prevents that combat's Root growth.

Reason for discard-pile start:

- Avoid opening-hand death lottery with existing Ascension curse pressure.
- Let turns 1-2 establish normal combat rhythm.
- Keep turn 3 as a clear pressure point.

### 5.4 Root Growth

| Master deck state | Growth result |
| --- | --- |
| No Root | Add one Root. |
| Root exists | Upgrade Root to Deep Root. |
| Deep Root exists | No further upgrade; the combat pressure already mattered. |

### 5.5 Forge Token

Forge Token protects upgrade tempo after taking firemarked elite risk.

Initial design:

- Source: defeating a firemarked elite.
- Max held: 1.
- Visible status: Forge Token is shown as a one-count Event-rarity status relic so the player can hover it before the next rest site.
- Duplicate conversion: 15 gold.
- On next rest site:
  - If resting: randomly upgrade one upgradable common/uncommon card.
  - If upgrading: heal 7 HP.
  - If resting has no valid upgrade target: heal 5 HP instead.
  - Other special rest actions are intentionally deferred until the generic rest-option lifecycle is proven.

### 5.6 Fission Enchantment

Fission is a generic reward-card modifier.

Initial design:

- Energy cost -1.
- Gains Exhaust.
- Uses a dedicated enchantment icon and Exhaust hover tip.
- Only applies to eligible reward cards.
- Source chance: normal combat reward 10%, Banner Room reward 15%, Firemarked Elite reward 20%, Boss reward 5%.
- Each reward screen may contain at most one Fission card.

Eligibility hypothesis:

- Energy cost >= 1.
- Attack or Skill only; Powers are excluded.
- Non-X-cost.
- No star-cost card.
- Does not already have Exhaust or one-turn exhaust-on-play behavior.
- Non-quest, non-special, non-unmodifiable card.
- No existing incompatible high-impact enchantment.

### 5.7 Firemarked Elite

A visible elite map node modifier.

Current v3.2 firemark types:

| Firemark | Effect | Trigger |
| --- | --- | --- |
| Might | One firemarked enemy gains +1/+2/+4 Strength by act. Unblocked attack damage builds Heat; 2 Heat makes the next attack deal +1/+2/+4 extra damage. | OnCombatStart / OnDamage |
| Giant | One firemarked enemy gains +20%/+30%/+45% max/current HP by act. At half HP it exposes Molten Core; enough damage during the window removes 10% max HP, otherwise it gains 1 Artifact. | OnCombatStart / OnDamage / OnTurnEnd |
| Firemark Overflow | One Firemark Host receives the full mark. Overflow affects at most one secondary non-summon enemy at a time: Might gives 1/1/2 temporary Strength to an attacker, Giant core break deals 6/12/24 splash damage, Forge Armor gives 3/6/12 Block, Constant Heal restores 2/4/8 HP to one damaged ally. | BeforeSideTurnStart / AfterPlayerTurnStart / damage and heal windows |
| Forge Armor | The Firemark Host gains 8/14/24 Molten Armor at player turn start. If the host has no Block at turn end, the next Molten Armor is skipped. This can happen at most twice per combat. | AfterPlayerTurnStart / AfterSideTurnEnd |
| Constant Heal | One firemarked enemy heals 4/8/16 HP at enemy turn end. Dealing 18/36/72 damage to it before its next heal interrupts that heal. | AfterSideTurnEnd |

Generation rules:

- Target 2 in Act 1 and 3 in Acts 2-3 when enough safe nodes exist.
- Firemarked selections avoid same-floor and directly adjacent elite nodes. Selection spreads across routes where possible, but can relax route exclusivity before dropping below target count.
- Not before Act 1 first rest site.
- Not forced into the only route.
- Clearly visible on map with a dedicated firemark indicator, not the generic quest marker used by Fur Coat / Spoils-style markers.
- Firemark combat type is visible as a power on the single Firemark Host.
- Firemarked elite card rewards have one extra card option.
- Defeating grants Forge Token.

### 5.8 Banner Room

A visible enhanced normal combat node.

Current v3.2 banner types:

| Banner | Effect | Trigger |
| --- | --- | --- |
| Vanguard Banner | All primary enemies gain +1/+2/+4 temporary Strength by act for the first two rounds. | OnCombatStart / OnTurnStart |
| Shield Formation Banner | Multi-enemy only. One bannerbearer protects other enemies for 3/7/14 Block by act each enemy turn; on death, the others gain 5/10/20 Block. | OnCombatStart / OnTurnStart / OnDeath |
| Blood Prize Banner | Mark one bounty enemy. Killing it by the end of round 3 grants 15/30/55 Gold; missing the deadline gives it sustained retaliation. | OnCombatStart / OnTurnEnd / OnReward |
| Pressing Line Banner | From each player's 4th card each turn, build up to 3 pressure layers; the highest two players resolve Block and possible extra attack damage. | OnCardPlayed / OnTurnEnd |
| Last Stand Banner | Multi-enemy only. The first primary enemy death gives remaining primary enemies 6/12/24 Block and +1/+2/+4 next-turn Strength by act. | OnDeath |

Generation:

- Banner rooms may appear as visible enhanced normal combats. Shield Formation and Last Stand require multi-enemy fights; if a single-enemy fight is reached, they convert to Blood Prize.
- Does not stack with firemarked elites.
- Banner rule is visible on map hover before commitment.
- Bounty's direct bonus reward is the first implementation-batch reward and must use room reward APIs, not monster action table edits.

### 5.9 Deep Branch

Optional high-risk/high-reward branch in Acts 2/3.

Structure:

- 3-4 nodes.
- At least one risk node: Banner Room or Firemarked Elite.
- At least one enhanced reward node.
- Reconnects to main route.
- Must preserve a parallel safer route.

### 5.10 A19/A20 Boss Dedicated Abilities

A19 gives each Boss one visible dedicated ability. It is not a shared generic seal. A20 only strengthens the second Act 3 Boss.

| Boss | A19 ability | A20 brand change |
| --- | --- | --- |
| Kin Priest | Martyr Oath: follower deaths strengthen the next debuff or attack. | Higher attack bonus; killing both followers in one turn grants 1 Artifact. |
| Vantom | Ink Return: the first full Slippery removal returns a percentage next enemy turn. | Higher percentage and cap. |
| Lagavulin Matriarch | Plating Wake: wake-up grants Plating; first Soul Siphon trims it. | More Plating; Soul Siphon trims less. |
| Soul Fysh | Soul Tide: unanswered Beckons are counted before their turn-end in-hand damage resolves, then grant Block after Soul Fysh's turn so the Block is visible before the next player turn begins; Intangible grants 1 Artifact. | More Block per Beckon and a higher team cap. |
| Waterfall Giant | Unweakenable: explosion ignores Weak/attack-down and applies Vulnerable. | Vulnerable lasts longer. |
| Kaiser | Claw Calibration: uneven claw HP strengthens the healthier claw's next attack. | Lower HP-gap threshold and higher attack bonus. |
| Knowledge Demon | Marginal Note: unplayed notes become Deep Thought for the next Knowledge curse. | Higher Deep Thought cap with per-turn gain cap. |
| The Insatiable | Escape Fatigue: ability-made Frantic Escapes grant Vigor after enough are played. | More Vigor. |
| Aeonglass | Time Sand Reflow: Ebb creates Time Sand; leftover sand adds extra Wither. | More Time Sand and limited extra Eye Lasers hits. |
| Queen | Royal Decree: one Bound card avoids penalty; wrong or missed Bound cards give Majesty. | Majesty cap increases. |
| Test Subject | Experimental Record: the previous phase leaves a residual sample. | Two different samples per phase change. |

Attack-changing abilities must show final intent. This includes Martyr Oath, Claw Calibration, Vigor, Aeonglass's extra Eye Lasers hit, and any future Brand damage changes.

Multiplayer scaling rules:

- Strength, Artifact, Vigor, shared counters, Time Sand, Majesty, and Martyr Oath do not multiply by player count.
- Slippery, Plating, Boss HP, and player-hand cards use the current fight's actual values.
- Per-player triggers such as Beckon, Marginal Note, Frantic Escape, Bound, and Test Subject phase statistics use team caps so 3-4 player runs do not scale linearly.

### 5.11 A20 Intermission

Between Act 3 boss 1 and boss 2:

- Heal 25% of missing HP.
- Grant a boss card reward.
- Apply A19 reward option increase if active.
- Re-show boss 2 dedicated ability and Branded Form state.
- No complex menu.

## 6. Multiplayer Rules

- Root is individual per player.
- Root Bud is individual per player by default, but may need capped targeting if 3-4 player pressure is too high.
- Team members cannot play another player's Root, but can support by defending/healing.
- Knockout/revive should clear combat-only Root Buds but not permanent Root.
- Dazed/hand-pollution effects should target at most:
  - 1 player in 1-2 player games.
  - 2 players in 3-4 player games.
- Forge Token is individual.
- Deep Branch and high-risk route selection should use route voting; ties prefer safer route.

## 7. UI / UX Requirements

Needed UI surfaces or equivalents:

- Firemark elite map border/icon.
- Banner room map icon.
- Deep branch/rift route marker.
- Boss dedicated ability and Branded Form map/boss preview marker.
- A20 intermission marker.
- Root Bud combat notice.
- Root growth combat-end notice.
- Fission keyword/tooltip/card visual marker.
- Forge Token rest-site status.

UI can be deferred behind console/log/manual validation for prototypes, but private-beta acceptance needs player-readable explanation.

## 8. Initial Tuning Values

| System | Initial value |
| --- | --- |
| Root cost | 2 |
| Deep Root cost | 3 |
| Root Bud cost | 2 |
| Root Bud sprout timing | Turn 3 |
| Max Root family card | 1 permanent Root/Deep Root |
| Forge Token cap | 1 |
| Forge Token rest fallback heal | 5 HP |
| Forge Token Smith heal | 7 HP |
| Forge Token special rest-site heal | 5 HP |
| Firemark Might | +1/+2/+4 Strength by act; Heat burst +1/+2/+4 damage |
| Firemark Giant | +20%/+30%/+45% max/current HP; Molten Core damage window 20%/25%/30% original max HP |
| Firemark Forge Armor | 8/14/24 Molten Armor by act |
| Firemark Constant Heal | 4/8/16 HP by act; interrupt threshold 18/36/72 damage |
| Vanguard Banner | +1/+2/+4 temporary Strength by act |
| Shieldwall Banner | 3/7/14 turn Block; 5/10/20 death Block |
| Blood Prize Banner | 15/30/55 Gold; retaliation +1/+2/+4 Strength and 1/1/2 Artifact |
| Pressing Line Banner | 4/8/16 partial Block; 6/12/24 full Block; +1/+2/+4 extra damage |
| Last Stand Banner | 6/12/24 Block and +1/+2/+4 temporary Strength |
| A20 intermission heal | 25% missing HP |

All values are prototypes, not final balance.

## 9. Engineering Module Sketch

Expected modules, subject to API research:

- Ascension gate/status module.
- Map-generation modifier module.
- Combat modifier module.
- Root card family module.
- Root Bud tracking module.
- Fission card modifier/enchantment module.
- Forge Token saved run-state module.
- Firemark elite node module.
- Banner room node module.
- Deep branch map template module.
- Boss dedicated ability module.
- A20 intermission flow module.
- Localization module.
- Debug/dev-console testing notes.

## 10. Required API Research Before Implementation

Research must answer before coding each system:

| Question | Required before |
| --- | --- |
| How current Ascension level is represented and checked | All systems |
| How max Ascension is defined/displayed/unlocked | A11-A20 availability |
| How map generation nodes/edges are created | A11, A12, A16, A17, A20 previews |
| How combat room type and act number are identified | Root Bud, firemarks, banners, seals |
| How cards are added to master deck and removed permanently | A14, Root, Deep Root |
| How temporary combat cards are added to discard/draw pile | Root Bud |
| How card enter-hand/play/combat-end hooks work | Root Bud tracking |
| How rest-site actions are represented | Root cleanup, Forge Token |
| How reward generation and card options are represented | Fission, A19 reward option increase |
| How card enchantments/modifiers are represented by RitsuLib/template APIs | Fission |
| How boss order and double boss flow are represented | A20 |
| How multiplayer player targeting and team state are exposed | multiplayer safety |
| Whether RitsuLib/template or game-native APIs expose safer paths for any of the above | minimize Harmony |
| Which Harmony patch points are unavoidable | only after evidence |

## 11. Prototype Order

### Phase 0: Research and architecture only

- Create API research notes.
- Inspect game/BaseLib signatures only.
- No gameplay mutation.
- Produce patch-point proof table.

### Phase 1: Ascension gate and max-level display spike

- Find how A11-A20 can be selected/unlocked/displayed.
- If UI unlock is risky, use dev-only internal gate first.

### Phase 2: Root closed loop MVP

- A14 Root Begins.
- Root card add/remove.
- Rest cleanup only after the rest-site option hook path is compile- and runtime-proven; current gated implementation does not include this cleanup.
- Manual and automated tests.

### Phase 3: Boss Root Bud MVP

- A15 only.
- Add to discard pile.
- Sprout timing.
- Combat-end growth.
- Keep A18 behind its own explicit gate. The current internal implementation includes elite Root Bud only at `SPIREPLUS_ASCENSION_DEBUG_LEVEL=18`; public A18 remains unsupported.

### Phase 4: Firemarked Elite + Forge Token MVP

- A12 one visible or logged firemarked elite path.
- Three generic combat modifiers.
- Forge Token rest-site payoff.

### Phase 5: Fission Enchantment MVP

- A13 reward-card modifier.
- Strict eligibility filter.
- Localization and card tooltip validation.

### Phase 6: Banner Room MVP

- A16 map node modifier and three banner effects.

### Phase 7: Deep Branch MVP

- A17 map generation branch template.

### Phase 8: Boss dedicated abilities and A20 MVP

- A19 seals and boss reward +1 option.
- A20 reveal/intermission/Branded Form state.

## 12. Validation Metrics

Track manually first, automate later:

- Root average retained fights.
- Root played in combat rate.
- Rest cleanup rate.
- Deep Root frequency.
- Root Bud seen/played/growth rate.
- Firemarked elite entry rate.
- Forge Token rest/upgraded/heal usage.
- Fission appearance/take/skip rate.
- Banner room entry rate and loss rate.
- Deep branch entry/death/reward rate.
- Boss seal win/loss by seal type.
- A20 boss 1 exit HP and boss 2 outcome.

## 13. Risk Register

| Risk | Impact | Mitigation |
| --- | ---: | --- |
| Root forces rest every act | High | Lower cost, delay sprout, strengthen Forge Token, reduce Root sources. |
| Root too weak | Medium | Keep Deep Root, keep seen-but-unplayed growth rule, tune sprout timing carefully. |
| Firemarked elites are traps | High | Add better rewards, reduce mark values, improve route placement. |
| Fission too strong | High | Lower appearance, restrict rarity/card types, no 0-cost floor if needed. |
| Banner rooms are annoying taxes | Medium | Add modest reward, reduce values, avoid clustering. |
| Map generation breaks routes | High | Start with logged/no-op map probe and conservative node marking. |
| A20 second boss too hard | High | Increase intermission heal, reduce Branded Form tuning, improve intermission reward. |
| Multiplayer scales punishment too hard | High | Cap target count for pollution effects. |
| Early Access API changes | High | Keep patch-point evidence docs and isolated modules. |

## 14. Release Boundary

A11-A20 should not be released as public beta until:

- A11-A20 selection/display is stable.
- Root loop is verified through save/load and combat transitions.
- Reward generation changes do not break card/relic screens.
- Map changes preserve reachable paths.
- Multiplayer behavior is at least smoke-tested or explicitly marked unsupported.
- Simplified Chinese and English text are complete.
- Disable/uninstall path is tested.

## 15. Current Decision

Implementation should not start with all ten Ascensions at once.

Recommended first sprint:

1. API research gate for Ascension, map, card, reward, rest-site, combat hooks.
2. A14 Root closed-loop MVP behind an Ascension/debug gate.
3. A15 Boss Root Bud after A14 is stable.
4. Only then proceed to map/reward systems.
