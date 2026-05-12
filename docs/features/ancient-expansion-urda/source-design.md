# Urda Ancient Source Design v1

## 1. One-line goal

Add a directly playable Urda vertical slice for private beta:

- one new Act 1 Ancient (`Urda, Loamweaver`),
- a safe active blessing pool,
- save/load-safe blessing state,
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
- Additional unknown ancient systems not explicitly listed below.

## 3. Urda ancient design

### 3.1 Urda identity

Ancient:

- Stable id: `EZMB_URDA`
- English: `Urda, Loamweaver`
- Simplified Chinese: `息壤织母·乌尔达`
- Offer target: Act 1.

Acting rule:

- Urda should be offered through the current vanilla ancient surface or a proven local custom-ancient registration path.
- If direct custom-ancient registration is not source-safe, Urda may remain behind a default-off debug/test forcing path with explicit documentation.

### 3.2 Blessings

The initial active pool is limited to:

1. Seedbed (`urda_seedbed`, 苗床)
2. Humus Pact (`urda_humus_pact`, 腐殖约定)
3. Molting (`urda_molting`, 脱壳)
4. Moss Map (`urda_moss_map`, 苔痕地图)

Unsafe or unfinished blessings must be excluded from live pools.

### 3.3 Blessing behavior

#### Seedbed (`urda_seedbed`)

- Triggered by normal Act 1 combat card rewards.
- Offers a Seedbed reward alternative while the player has more than 2 max HP and fewer than four accepted Seedbed rewards.
- Tracks four accepted Seedbed choices; reward generation, reroll, and screen refresh do not spend a check by themselves.
- On accepting a reward, lose 2 max HP.
- On all four accepted rewards, set the transformed latch and gain +10 max HP with no heal.
- A visible `Seedbed's Herald` display state is not implemented in the current source slice.

#### Humus Pact (`urda_humus_pact`)

- Triggered by an explicit `Compost Reward` alternative on normal Act 1 combat card rewards.
- On each composted reward, gain 15 gold.
- At three completed composts: after the card reward has completed, open a remove flow (0/1/2 card removals), then offer one unskippable upgraded card reward.
- The third payoff keeps a pending latch until payoff resolution succeeds; payoff card generation happens before optional removals so a no-card fallback cannot consume removals or silently drop the payoff.
- Ordinary reward-set skip/proceed and room-exit cleanup must not trigger Humus Pact.
- Apply once; do not repeat past completion.

#### Molting (`urda_molting`)

- On selection, remove one Strike and one Defend from deck, then add two `Withered Husk` cards.
- `Withered Husk` is a temporary status-like effect card.
- Deck `Withered Husk` cards are removed at Act 2 start.
- `Withered Husk` is non-playable for long-term deck loops except its exhaust-to-block behavior.

#### Moss Map (`urda_moss_map`)

- One-time per room type bonus within Act 1.
- Rewards are room-type keyed and source-backed.
- Bonus table:
  - normal combat: +25 gold,
  - unknown/event: heal 5 HP,
  - shop: add one random potion if a potion slot is open,
  - elite: upgrade one random card if an upgradable card exists,
  - rest site: +3 max HP.
- Safe room-type resolution is required before release claiming.

## 4. State and persistence design

Current source packs Urda state into `AncientSavedStateFields.UrdaStateKey` on `Player` to avoid increasing the SavedSpireField count during the prototype slice.

Encoded fields:

- selected blessing id,
- Seedbed accepted-check count,
- Seedbed accepted reward count,
- Seedbed transformed latch,
- Humus compost count,
- Humus completed latch,
- Humus completion-pending latch,
- Molting active latch,
- Moss Map per-room-type reward flags.

The parser accepts the prior eight-field shape for migration, but `SavedSpireField<Player,string>` persistence is not source-proven by this pass. State must survive live save/load before this design can be marked release-ready.

## 5. Localization and terms

All active Urda text must include EN + ZHS entries.

- Enforced by `docs/style/card-localization-style-guide.md` conventions.
- Use clean `[gold]` and no raw tags.
- Ensure visible keyword count and dynamic variables are mirrored.

## 6. Risk register

1. Unsafe ancient registration API in v0.105.x can block release-ready claims.
2. Room-type identity changes can misfire across non-standard rooms.
3. `Withered Husk` temporary card behavior must not soft-lock removal, transformation, or upgrade.
4. Reward screen mutation must preserve reroll, skip, proceed, and room-exit flows.
5. Player-owned encoded state must be proven by live save/load or moved to a source-proven persisted carrier.

## 7. Out-of-scope release assertions

Urda is a private-beta playable slice.
Do not claim release-ready for Urda until:

- Urda registration and blessing pool are verified in live act 1 selection,
- each active blessing passes manual checks,
- logs and save/load evidence are attached.
