# Localization Validation

Validation date: 2026-05-06

Scope: `EZMicroBalance/localization` and localization-only tooltip strings in `EZMicroBalanceCode`.

v4.3 is current. v4.2 rightmost-slot Prismatic Gem is historical only. v4.2 Distinguished Cape 40% min15 is historical only.

## JSON Parse Results

All active localization JSON files parse as explicit UTF-8:

- `eng/ancients.json`
- `eng/cards.json`
- `eng/card_keywords.json`
- `eng/powers.json`
- `eng/relics.json`
- `eng/rest_site_ui.json`
- `eng/static_hover_tips.json`
- `zhs/cards.json`
- `zhs/relics.json`
- `zhs/rest_site_ui.json`

## Key Matching

- English and Simplified Chinese relic override keys match.
- English and Simplified Chinese card override keys match.
- English and Simplified Chinese rest-site UI override keys match.
- Legacy `EzDailyContent` localization is not compiled or packaged by `EZMicroBalance`.

## Simplified Chinese Glossary

| English/internal term | Official zhs term if found | Proposed zhs term if not found | Source/evidence | Confidence | Where used |
| --- | --- | --- | --- | --- | --- |
| Swift | 迅速 | n/a | Base game `localization/zhs/enchantments.json`, `SWIFT.title` and tooltip. Some relic text uses `迅捷`; the enchantment table is the tooltip authority. | High | Beautiful Bracelet, Swift enchantment tooltip |
| Apotheosis | 神化 | n/a | Base game `localization/zhs/cards.json`, `APOTHEOSIS.title`. | High | Jewelry Box |
| Enthralled | 执迷 | n/a | Base game `localization/zhs/cards.json`, `ENTHRALLED.title`; `relics.json`, Blood-Soaked Rose text. | High | Blood-Soaked Rose, Enthralled card |
| Wish | 许愿 | n/a | Base game `localization/zhs/cards.json`, `WISH.title`. | High | Claws |
| Relax | 放松 | n/a | Base game `localization/zhs/cards.json`, `RELAX.title`; Pael's Horn base text. | High | Pael's Horn |
| Folly | 愚行 | n/a | Base game `localization/zhs/cards.json`, `FOLLY.title`; Preserved Fog base text. | High | Preserved Fog, Folly card |
| Debt | 债务 | n/a | Base game `localization/zhs/cards.json`, `DEBT.title`. | High | Seal of Gold, Debt card |
| off-color | n/a | 异色牌 | No base-game zhs match found locally. Existing mod zhs text already used `异色牌`; keep as temporary project glossary term. NEEDS_OFFICIAL_TERM before public release if the game adds an official term. | Medium | Prismatic Gem |
| Boss | n/a | 首领 | Base `ROOM_BOSS.title` is still raw `Boss`; zhs descriptions call it the stage endpoint. User requirement permits `首领`, so player-facing mod text uses `首领` instead of raw English. | Medium | Pael's Tooth |
| Cook | 烹饪 | n/a | Base game `localization/zhs/static_hover_tips.json`, `COOK.title`. | High | Meat Cleaver, rest site UI |
| Power | 能力牌 | n/a | Base game card text such as `CREATIVE_AI.description` and `WHITE_NOISE.description`. | High | Jeweled Mask, Crossbow/Music Box contrast |
| Attack | 攻击牌 | n/a | Base game relic/card text such as Shuriken and Crossbow. | High | Crossbow, Music Box |
| Rare | 稀有牌 | n/a | Base game relic/card text such as Arcane Scroll and White Star. | High | Choices Paradox |
| Retain | 保留 | n/a | Base game `localization/zhs/card_keywords.json`, `RETAIN.title`. | High | Choices Paradox |
| Ethereal | 虚无 | n/a | Base game `localization/zhs/card_keywords.json`, `ETHEREAL.title`. | High | Crossbow, Music Box |
| Exhaust | 消耗 | n/a | Base game `localization/zhs/card_keywords.json`, `EXHAUST.title`. | High | Crossbow, Music Box, Debt, Toasty Mittens |
| Innate | 固有 | n/a | Base game `localization/zhs/card_keywords.json`, `INNATE.title`. | High | Jewelry Box, Folly |
| Eternal | 永恒 | n/a | Base game `localization/zhs/card_keywords.json`, `ETERNAL.title`. | High | Enthralled, Folly |
| Strength | 力量 | n/a | Base game relic/card text such as Shuriken and Vajra. | High | Toasty Mittens |
| Jeweled Mask | 宝石面具 | n/a | Base game `localization/zhs/relics.json`, `JEWELED_MASK.title`. | High | Jeweled Mask custom enchantment tooltip |

## Exact Text Replacements

Active `EZMicroBalance/localization/zhs/relics.json`:

- `Swift 2` -> `迅速2`
- `Enthralled` -> `执迷`
- `Wish` / `Wish+` -> `许愿` / `许愿+`
- `Relax` / `Relax+` -> `放松` / `放松+`
- `非 Boss 战斗` -> `非首领战斗`
- `本幕 Boss` -> `本幕首领`
- `Folly` -> `愚行`
- `欠款诅咒` -> `债务诅咒`
- `Distinguished Cape` text now describes losing current max HP by 30%, at least 18, then adding 3 `灵体` cards.
- `Velvet Choker` text now describes +1 Energy and the 7th+ from-hand card soft-limit cost.
- `Prismatic Gem` text now says Every second standard card reward contains only off-color cards.
- `Prismatic Gem` hover text adds `棱彩计数：{Count}/{Cycle}` with next-reward descriptions for `0/2` and `1/2`.
- `Prismatic Gem` reward-screen hint adds `棱彩奖励：本次只出现异色牌。`.

Active card localization:

- Added `DEBT.title` / `ENTHRALLED.title` / `FOLLY.title` in English and zhs for key parity.
- `ENTHRALLED.description` now uses the official zhs phrasing pattern `必须优先打出这张牌`.

Tooltip/localization-only code:

- `JeweledMaskFreePower` now returns zhs `CardModifierLoc` strings when `LocManager.Instance.Language == "zhs"`.

## Tooltip Findings

- `card_keywords.json` controls keyword tooltip names/descriptions. No EZ Micro Balance keyword override is required because the base game already localizes `Retain`, `Ethereal`, `Exhaust`, `Innate`, and `Eternal` in zhs.
- `static_hover_tips.json` controls static hover tips and dynamic-var hover surfaces, but Swift is not a static hover tip or a card keyword.
- Swift is an enchantment. Its zhs name and tooltip come from base game `localization/zhs/enchantments.json` (`SWIFT.title = 迅速`). Beautiful Bracelet text should reference the localized term, not raw `Swift`.
- `DynamicVar("Swift", 2m)` remains a numeric variable provider for relic text if a `{Swift}` placeholder is used. It is not the localized display name and does not need replacement for this pass.
- BaseLib `ILocalizationProvider` is language-agnostic unless the mod supplies language-aware strings. `ModelLocPatch` injects the provider strings into the `enchantments` loc table, so the Jeweled Mask custom enchantment needed a zhs branch in code.

## Number Formatting

v4.3 retains player-facing Simplified Chinese number normalization with no spaces between Chinese text, numbers, and units. Examples in active zhs surfaces use `选择3张牌`, `获得1点能量`, `第7张`, `迅速2`, `至少18点`, `棱彩计数：1/2`, `设为0`, and `费用为0`.

## Automated Guards

`tests/EZMicroBalance.Tests/ReleaseArtifactTests.cs` now covers:

- Active localization JSON parses as UTF-8 JSON.
- English/zhs key parity for cards, relics, and rest-site UI.
- zhs string values contain no banned English leftovers for the sprint term list.
- The zhs English-term whitelist is explicit and empty for this release.
- Beautiful Bracelet specifically contains `迅速2` and not `Swift`.
- Player-facing zhs numbers have no spaces around digits in active localization or custom tooltip strings.
- Jeweled Mask custom enchantment localization contains zhs strings.

## Remaining Manual Validation

Runtime language verification is still required:

- Launch once in English and inspect changed relic/card/rest-site text.
- Launch once in Simplified Chinese and inspect changed relic/card/rest-site text.
- Inspect Prismatic Gem hover text in Simplified Chinese and confirm the count hint displays `棱彩计数：0/2` or `棱彩计数：1/2`.
- Inspect a triggered Prismatic Gem reward screen in Simplified Chinese and confirm the banner displays `棱彩奖励：本次只出现异色牌。` if the banner is visible; if not, confirm `godot.log` records `PrismaticGem reward-screen hint fallback` and use the relic hover count plus visible off-color cards as fallback evidence.
- Inspect Beautiful Bracelet after applying the enchantment and confirm the card/enchantment tooltip displays `迅速`.
- Inspect Jeweled Mask's permanent 0-cost enchantment tooltip in Simplified Chinese.
- Confirm no missing localization keys appear in `godot.log`.
