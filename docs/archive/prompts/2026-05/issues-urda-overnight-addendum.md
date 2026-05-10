# Archived prompt (2026-05)

- Original path: 'docs/issues-urda-overnight-addendum.md'
- Archived path: 'docs/archive/prompts/2026-05/issues-urda-overnight-addendum.md'
- Reason: Urda issue addendum captured to keep active planning out of the current docs path.
- Archived date: 2026-05-10

---

# docs/issues.md addendum �?Urda Act 1 Ancient playable implementation

Append this section near the top of `docs/issues.md` under `## Open`, before older RC1 blockers.

---

## Urda Overnight Implementation Block �?2026-05-09

This block scopes the next overnight build to **one new Act 1 Ancient only**: **息壤织母·乌尔�?/ Urda, Loamweaver**.

Do **not** implement Morvi, Lotha, or Vakuu in this pass. Their design notes may be preserved as future planning only, but they must not appear in live offer pools, Mod Settings, localization as active content, package release notes, or manual test pass/fail tables.

The goal is a directly playable Urda vertical slice. The build may ship with only safely implemented Urda blessings in the active Urda pool. Any blessing that cannot be implemented safely overnight must stay disabled and must not appear in-game.

### Common closure evidence for all Urda issues

Each Urda issue can only be closed when all relevant evidence exists:

- Current `source code/src/Core/**` v0.105.0 source inspected first.
- API evidence recorded in `docs/features/ancient-expansion-urda/api-research.md`.
- Implementation notes recorded in `docs/features/ancient-expansion-urda/work-log.md`.
- English and Simplified Chinese localization added.
- Manual rows added to `docs/features/ancient-expansion-urda/manual-test-checklist.md`.
- Source guard tests added.
- `dotnet build EZMicroBalance.sln`: 0 warnings / 0 errors.
- `dotnet test EZMicroBalance.sln`: 0 failed.
- `dotnet test EZMicroBalance.sln --no-build`: 0 failed.
- `dotnet publish EZMicroBalance.sln`: passed if code/resources/localization changed.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: 0 failed after package refresh.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed, only known CRLF normalization warnings allowed.
- Clean normal Steam-client log with BaseLib + EZMicroBalance only, or explicit pending live blocker.
- No release-ready claim unless live gates actually pass.

---

### ISSUE-2026-05-09-URDA-ACT1-ANCIENT-FRAMEWORK

Priority: P0

Status: open; overnight implementation requested

Area: Ancient expansion framework / Act 1 Ancient registration / blessing offer pool

Player goal:

- Add one new playable Act 1 Ancient: **息壤织母·乌尔�?/ Urda, Loamweaver**.
- Urda should appear at Act 1 Ancient selection / Neow-replacement surface, depending on v0.105.0 source architecture.
- Choosing Urda should offer **4 blessings** from Urda's active blessing pool.
- Choosing a blessing should grant a blessing relic / persistent blessing state and should survive save/load.
- Morvi, Lotha, and Vakuu must not be implemented or exposed in this pass.

Required source research:

Search current local source:

- `Ancient`
- `AncientModel`
- `AncientEvent`
- `AncientReward`
- `AncientCard`
- `Neow`
- `Blessing`
- `RelicReward`
- `StartRun`
- `ActEnter`
- `CardReward`
- `CardRewardSkipped`
- `RewardScreen`
- `RunState`
- `SavedSpireField`
- `ModelDb`
- `BaseLib`
- existing EZMB `Ancients/**` patches

Implementation requirements:

- Create `docs/features/ancient-expansion-urda/` with:
  - `README.md`
  - `source-design.md`
  - `implementation-plan.md`
  - `api-research.md`
  - `manual-test-checklist.md`
  - `work-log.md`
- Create an Urda registry / pool system using source-proven API.
- Use stable IDs:
  - Ancient id: `EZMB_URDA`
  - English name: `Urda, Loamweaver`
  - Chinese name: `息壤织母·乌尔妲`
- Offer count: 4.
- Only implemented and enabled Urda blessings can appear.
- Disabled/unsafe blessings must stay out of the live pool.
- Save/load must preserve selected Urda blessing and cross-combat counters.
- Add debug/test helper if source-proven and safe, e.g. force Urda / force blessing by env var:
  - `EZMB_FORCE_ANCIENT=URDA`
  - `EZMB_FORCE_URDA_BLESSING=<id>`
  These must be documented and default-off.

Manual closure:

- New run can select Urda.
- Urda displays 4 implemented blessings.
- Selecting a blessing grants correct effect.
- Save/load after selection preserves selected blessing.
- No Morvi/Lotha/Vakuu active content appears.

---

### ISSUE-2026-05-09-URDA-BLESSING-DATA-AND-STYLE

Priority: P0

Status: open

Area: Urda blessing ids / localization / style / docs

Required Urda blessing IDs:

- `urda_trial_branch`
- `urda_shallow_root_relic`
- `urda_rooted_route`
- `urda_seedbed`
- `urda_molting`
- `urda_after_rain`
- `urda_root_sight`
- `urda_humus_pact`
- `urda_seed_bank`
- `urda_moss_map`

English / ZHS naming target:

| ID | EN | ZHS |
| --- | --- | --- |
| `urda_trial_branch` | Trial Branch | 试种枝条 |
| `urda_shallow_root_relic` | Shallow-Root Relic | 浅根遗物 |
| `urda_rooted_route` | Rooted Route | 缠根路线 |
| `urda_seedbed` | Seedbed | 苗床 |
| `urda_molting` | Molting | 脱壳 |
| `urda_after_rain` | After the Rain | 雨后苏生 |
| `urda_root_sight` | Root-Sight | 根下千里�?|
| `urda_humus_pact` | Humus Pact | 腐殖约定 |
| `urda_seed_bank` | Seed Bank | 种子银行 |
| `urda_moss_map` | Moss Map | 苔痕地图 |

Style requirements:

- Follow `docs/style/card-localization-style-guide.md`.
- No duplicate visible keywords in body text.
- Use `[gold]` for custom blessing/resource/card names where official style supports it.
- Use `[blue]` for numbers where existing EZMB style does.
- English and ZHS behavior must match exactly.
- No mojibake.
- No raw tags in live UI.
- Avoid saying an unimplemented blessing is available.

---

### ISSUE-2026-05-09-URDA-SEEDBED

Priority: P0

Status: open; recommended minimum playable blessing

Area: Act 1 card reward extension / extra seedling offer

Design:

�?4 次普通战斗后，正常卡牌奖励结算后，额外展�?1 张苗牌。玩家可以失�?2 HP 拿走它，或跳过它。第一次拿苗牌时自动升级。若 4 次都失去 HP 拿走苗牌，苗床变�?**???的使�?*，获�?10 最大生命，不回复当前生命�?
Implementation requirements:

- Only active in Act 1.
- Only after normal combat card reward flow.
- Must not replace normal reward.
- Track:
  - `seedbedTriggers`
  - `seedbedAccepted`
  - `hasUpgradedFirstSeedling`
  - `transformed`
- Generate one normal/appropriate class card as Seedling.
- Choice:
  - accept: lose 2 HP, add card; first accepted card is upgraded.
  - skip: no HP loss, no card.
- If `triggers == 4 && accepted == 4 && !transformed`:
  - set transformed true
  - blessing display name becomes `???'s Herald` / `???的使者`
  - gain +10 Max HP, no heal
- Save/load preserves counters.
- Do not softlock card reward screen.
- Must handle reroll/reward regeneration safely.

Manual tests:

- Four normal fights trigger Seedbed exactly four times.
- First accepted Seedling upgrades.
- Accepting all four grants +10 Max HP and renames blessing.
- Skipping any trigger prevents hidden reward.
- Save/load after trigger 2 preserves counters.
- No HP payment if player skips.

---

### ISSUE-2026-05-09-URDA-HUMUS-PACT

Priority: P0

Status: open; recommended minimum playable blessing

Area: card reward skip listener / card removal / upgraded reward

Design:

一幕中，前 3 次跳过普通战斗卡牌奖励时，获�?1 层腐殖并获得 15 金币。获�?3 层腐殖时，移除最�?2 张牌，然后获�?1 次升级卡牌奖励�?
Implementation requirements:

- Act 1 only.
- Listen to normal combat card reward skipped.
- If `humus < 3`:
  - humus +1
  - gain 15 gold
- When humus reaches 3:
  - open remove-card flow allowing 0/1/2 removals
  - then offer 3 class cards, all upgraded before add
  - player may pick one or skip
  - mark completed
- Completed blessing must not trigger again.
- Save/load preserves humus and completed.
- Must not trigger on non-combat rewards unless source proves intended.

Manual tests:

- Skip reward 1/2 grants 15 gold each and humus count increments.
- Third skip opens remove flow.
- Removing 0, 1, or 2 cards all works.
- Upgraded card reward can be picked or skipped.
- Later skips do nothing.

---

### ISSUE-2026-05-09-URDA-MOLTING-WITHERED-HUSK

Priority: P0/P1

Status: open; recommended minimum playable blessing

Area: card removal / temporary status card / act transition cleanup

Design:

移除 1 �?Strike 类牌�?1 �?Defend 类牌。将 2 �?**Withered Husk / 枯皮** 加入牌组。进入二幕时，所有枯皮自动移除�?
Withered Husk / 枯皮:

- Status
- Unplayable
- Ethereal / 虚无
- When exhausted, gain 3 Block.
- Act 1 only: cannot remove, transform, or upgrade if source APIs allow prevention safely.
- Removed automatically on Act 2 enter.

Implementation requirements:

- Create custom status card:
  - ID: `EZMB_WITHERED_HUSK`
  - EN: `Withered Husk`
  - ZHS: `枯皮`
- On blessing selection:
  - choose/remove one Strike-type and one Defend-type card.
  - add two Withered Husk.
- If deck lacks Strike/Defend, fallback must not softlock:
  - allow choosing from available starter cards; or
  - disable blessing if selection cannot be satisfied.
- Hook OnExhaust to gain 3 Block.
- Remove all Withered Husk on entering Act 2.
- Save/load preserves cards and cleanup.

Manual tests:

- Blessing removes one Strike and one Defend.
- Adds two Withered Husk.
- Withered Husk exhausts at end of turn and grants 3 Block.
- Act 2 start removes all Withered Husk.
- Save/load before Act 2 preserves them; save/load after Act 2 does not restore them.

---

### ISSUE-2026-05-09-URDA-AFTER-RAIN

Priority: P1

Status: open; safe if source-proven death prevention exists

Area: Act 1 lethal prevention / elite gold bonus / act transition reward

Design:

一幕中第一次将要死亡时，改为保�?1 HP，获�?15 格挡，抽 1 张牌。然后将 2 �?Wound 加入弃牌堆，并失�?3 最大生命。若一幕结束前没有触发，进入二幕时回复 8 HP，获�?75 金币。触发前，每击败一幕精英获�?20 金币，最�?2 次�?
Implementation requirements:

- Act 1 only.
- Source-proven lethal damage hook required. Do not implement by guessing direct HP mutation.
- If no safe lethal hook exists overnight, keep blessing disabled and mark blocker.
- Before triggered:
  - first Act 1 elite kill grants 20 gold, max 2.
- On first lethal in Act 1:
  - prevent death
  - set/leave HP at 1 via source-proven command/API
  - gain 15 Block
  - draw 1
  - add 2 Wound to discard
  - lose 3 Max HP
  - mark triggered
- On Act 2 enter if not triggered:
  - heal 8
  - gain 75 gold
- Save/load preserves triggered and elite gold count.

Manual tests:

- Lethal damage leaves player at 1 HP and applies block/draw/wounds/max HP loss.
- No second save from lethal.
- Elite gold bonus only before trigger and max twice.
- If untriggered by Act 2, heal/gold reward occurs once.

---

### ISSUE-2026-05-09-URDA-MOSS-MAP

Priority: P1

Status: open; recommended minimum playable blessing if room-type detection is source-safe

Area: Act 1 room-type first-entry rewards

Design:

一幕中第一次进入每种房间时获得奖励�?
- Normal combat: 25 gold
- Unknown/event: heal 5 HP
- Shop: random potion
- Elite: upgrade 1 random card
- Rest site: +3 Max HP

Implementation requirements:

- Act 1 only.
- Track room types already rewarded.
- Trigger once per type.
- Use source-proven room type IDs; do not rely on display text.
- Normal combat should not double-trigger on elite/boss.
- Save/load preserves rewarded types.

Manual tests:

- Each room type rewards once.
- Re-entering same type does not reward again.
- Save/load after two types preserves state.
- Rewards use safe command APIs.

---

### ISSUE-2026-05-09-URDA-TRIAL-BRANCH

Priority: P1/P2

Status: open; implement if persistent per-card marker is safe

Area: card selection / per-card persistent marker / combat played tracking

Design:

�?4 张普通或罕见职业牌中选择 1 张，升级后加入牌组。它获得 **Trial Plant / 试种**：接下来 3 场战斗中，若至少 2 场战斗里打出它，它会留下；否则移除它�?
Implementation requirements:

- Generate 4 class common/uncommon cards.
- Player picks one.
- Upgrade chosen card and add to deck.
- Mark card instance with `TrialPlant`.
- Track:
  - remainingCombats = 3
  - combatsPlayed = 0
- At combat end:
  - if marked card was played this combat, combatsPlayed +1
  - remainingCombats -1
- When remaining == 0:
  - if combatsPlayed >= 2: remove marker, card stays
  - else remove card from deck
- Save/load preserves marker and counters.
- If per-card persistence cannot be proven safe, disable blessing.

Manual tests:

- Playing card in 2 of next 3 combats keeps it.
- Playing in 0/1 removes it.
- Upgraded card stays upgraded.
- Save/load during countdown preserves state.

---

### ISSUE-2026-05-09-URDA-SHALLOW-ROOT-RELIC

Priority: P1/P2

Status: open; implement if temporary relic marker and Act 2 choice are safe

Area: common relic choice / elite kill / act transition choice

Design:

�?2 个普通遗物中选择 1 个，获得它和 75 金币。若你在一幕击败精英，该遗物永久保留，并获�?35 金币。否则进入二幕时选择：失去它获得 75 金币，或失去 6 最大生命保留它�?
Implementation requirements:

- Generate 2 common relic choices.
- Pick one, gain relic + 75 gold.
- Mark relic `ShallowRoot`.
- First Act 1 elite kill:
  - remove marker
  - gain 35 gold
  - relic becomes permanent
- Act 2 enter if marker remains:
  - choice A: remove relic, gain 75 gold
  - choice B: lose 6 Max HP, keep relic, remove marker
- Save/load preserves marker and choice state.
- If losing/removing a relic cannot be done safely, disable blessing.

Manual tests:

- Elite kill roots relic and grants 35 gold.
- No elite produces Act 2 choice.
- Both Act 2 choices work.
- Save/load before Act 2 preserves pending choice.

---

### ISSUE-2026-05-09-URDA-ROOTED-ROUTE

Priority: P2/P3

Status: open; high-complexity map blessing; disable if not safe overnight

Area: map node marker / route commitment / multi-reward trigger

Design:

选择一个被标记的房间作�?**Root Mark / 根标**。抵达根标时，获�?3 次卡牌奖励和 1 瓶随机药水；第一次拿的牌自动升级。若根标路线火堆过少，额外获得一次临时营火。若路线选择导致根标不可达，失去 8 HP，获�?25 Gold，移除根标�?
Implementation requirements:

- Source-proven map node marking.
- Candidate nodes:
  - within first 7 floors
  - reachable
  - non-boss
  - non-chest
  - prefer route with at least 2 reachable rest sites after mark
- Display 2 candidates; player selects 1.
- On reaching root mark:
  - 3 card rewards
  - first picked card auto-upgrades
  - one random potion
  - temporary campfire if route had too few rest sites
- If mark becomes unreachable by route choice:
  - lose 8 HP
  - gain 25 gold
  - clear mark
- If map UI or reachability is not source-proven, disable blessing and leave issue open.

Manual tests:

- Candidate generation only picks reachable nodes.
- Reaching mark triggers rewards once.
- Unreachable route penalty triggers once.
- Save/load preserves selected mark.

---

### ISSUE-2026-05-09-URDA-ROOT-SIGHT

Priority: P2/P3

Status: open; high-complexity map preview blessing; disable if not safe overnight

Area: map node preview / room content lock / root-eye resource

Design:

获得 5 **Root Eyes / 根眼**。在地图上消�?1 根眼，预知一个可见且可抵达的房间。第一次使用根眼时，获�?1 瓶随机药水�?
Preview results:

- Normal combat: concrete enemy group.
- Elite: concrete elite.
- Unknown: category combat/event/shop/chest/shrine/special.
- Boss: cannot preview.

Implementation requirements:

- Add RootEye resource = 5.
- Add safe map UI action/button or source-proven alternate selection flow.
- Only visible + reachable nodes can be previewed.
- Consume 1 RootEye.
- Roll and lock room content if not already decided.
- First use grants random potion.
- If map UI cannot be safely patched overnight, disable blessing and leave issue open.

Manual tests:

- RootEyes count decrements.
- Preview locks content.
- Boss not previewable.
- Save/load preserves RootEyes and previewed nodes.

---

### ISSUE-2026-05-09-URDA-SEED-BANK

Priority: P2

Status: open; implement if reward button / temporary card storage is safe

Area: card reward storage / Act 1 boss pre-settlement

Design:

一幕中，每次看到普通战斗卡牌奖励时，可以将其中 1 张未选择的牌存为种子。最多存 3 张。一�?Boss 前，从种子中选择最�?2 张：第一张升级后加入；第二张加入并获�?Trial Plant / 试种。未选择种子消失�?
Implementation requirements:

- Add "store as seed" action on normal combat card rewards.
- Store at most 1 unchosen card per screen.
- Max stored seeds = 3.
- Seeds are not in deck, cannot be upgraded/removed/transformed until chosen.
- Before Act 1 boss:
  - choose first seed: upgrade and add
  - optionally choose second seed: add with TrialPlant
  - discard unchosen seeds
- If pre-boss hook or reward action is unsafe overnight, disable blessing.

Manual tests:

- Can store one unchosen card from a normal reward.
- Cannot exceed 3 seeds.
- Boss pre-settlement selects up to 2.
- Save/load preserves seed bank.

---

### ISSUE-2026-05-09-URDA-PLAYABLE-POOL-SAFETY

Priority: P0

Status: open

Area: offer pool safety / disable unsafe blessings

Requirement:

The overnight build must be directly playable. Therefore, unsafe or incomplete Urda blessings must not appear in the live Urda offer pool.

Acceptance:

- If all 10 Urda blessings are safely implemented, offer pool contains all 10.
- If high-complexity blessings are not safe, active pool must still contain at least 4 safe implemented blessings.
- Recommended minimum safe pool:
  - `urda_seedbed`
  - `urda_humus_pact`
  - `urda_molting`
  - `urda_after_rain` if lethal hook safe, otherwise `urda_moss_map`
  - plus any additional safe blessing.
- Disabled blessings must be documented in issues and manual checklist.
- Disabled blessings must not show to players.

Manual test:

- Start 20 Urda selection attempts via debug/seed if possible.
- No disabled blessing appears.
- Every offered blessing can be selected and played without softlock.

---

### ISSUE-2026-05-09-URDA-TELEMETRY-AND-DIAGNOSTICS

Priority: P2

Status: open

Area: balancing data / debug logs

Requirement:

Add default-off diagnostics for Urda.

Suggested env var:

- `EZMB_URDA_DIAGNOSTICS=1`

Log when enabled:

- selected Urda blessing id
- act/floor picked
- trigger counts
- HP lost from blessing
- cards added/removed
- gold gained
- energy/draw gained if tracked
- disabled blessing attempts
- save/load restoration state

Do not collect external telemetry. Only log to local `godot.log`.

Manual test:

- With diagnostics on, selecting Seedbed/Humus/Molting logs expected state.
- With diagnostics off, no spam logs.

---

### ISSUE-2026-05-09-URDA-MANUAL-MATRIX-AND-RELEASE-GATE

Priority: P0

Status: open

Area: manual verification / release handoff

Requirement:

Add a dedicated Urda manual checklist covering:

- Urda appears in Act 1.
- Offer count = 4.
- Each implemented blessing can be selected.
- Each implemented blessing save/loads.
- Each implemented blessing has EN/ZHS text.
- No raw tags or mojibake.
- No Morvi/Lotha/Vakuu active content.
- Clean log audit passes.
- Normal Steam-client smoke passes.
- Release notes clearly say Urda is new and which blessings are active.

Do not claim release ready until Urda manual rows are executed or explicitly accepted as pending for private beta.



