# Ancient Rework v4 Manual Verification Matrix

Prepared: 2026-05-07
Updated: 2026-06-21

Status: automated gates passed for the current RitsuLib-only beta.96 package shape; current beta.96 Off loader proof reaches main menu with exactly STS2-RitsuLib and Spire Plus loaded, but it is startup/loading evidence only. Previous beta.93 AdditiveBatch1 proof is previous-package loader/registration evidence only. Earlier normal Steam-client startup/log verification, page-level Mod Settings UI, A0/A10/A20 DevConsole combat smoke, A11 map/save-load spot checks, and targeted A14 Rootblight hover/starter-notice checks are historical package contexts until rerun on beta.96.

Latest Urda UI/art, Lotha, Vakuu, Root Eyes, and package refresh passes reran build/publish plus tests and real installed-folder package checks, but did not rerun live verification. Earlier opt-in artifact work exposed stale installed-folder risk while the game was running; the current package hash check uses the refreshed real installed folder and game-root ZIP. Full live Ancient reward gameplay, Rootblight combat-end behavior/notices, natural route-click first-node checks beyond the historical A11 spot check, Ancient save/load, natural A11 click-by-click traversal, and multiplayer verification are still pending.

## Automated Prerequisites

| Check | Result |
| --- | --- |
| `dotnet build` | Pass, 0 warnings, 0 errors |
| `dotnet publish` | Pass |
| PCK package audit | Pass, active package entries exclude legacy/source/docs/art/archive files |
| English JSON parse | Pass |
| Simplified Chinese UTF-8 JSON parse | Pass |
| Simplified Chinese no-space numeric formatting guard | Pass |
| Ancient and Ascension source guard tests | Pass |
| Private-beta package | Pass for package refresh and real installed-folder checker, `publish\SpirePlus-v0.1.0-private-beta.96.zip`, SHA256 `6E313D383E49B750E3C5809E92D7795CC5E196B5A7511707D2AB4357E24D4265`; current beta.96 Off loader proof is clean for startup/loading only |

## Runtime Load Checklist

| Step | Expected Result | Result |
| --- | --- | --- |
| Confirm STS2-RitsuLib and `EZMicroBalance` are installed in the game `mods` folder | `STS2-RitsuLib\mod_manifest.json`, `STS2-RitsuLib\lib\0.107.1\STS2-RitsuLib.dll`, `EZMicroBalance.json`, `EZMicroBalance.dll`, and `EZMicroBalance.pck` are present | Prepared |
| Launch Slay the Spire 2 public/main branch | Game reaches main menu | Current direct beta.96 Off loader proof passed; normal Steam-client gameplay proof still pending |
| Open Settings -> Mod Settings | STS2-RitsuLib appears and Spire Plus appears with id `EZMicroBalance` | Current beta.96 RitsuLib Mod Settings proof captured |
| Open Compendium -> Card Library | Card lists display and filtering/sorting works without `VelvetChokerSoftLimitTracker.ShouldTax` or `CanonicalModelException` errors in `godot.log` | Pending |
| Start a run with Spire Plus enabled | No startup exception in `godot.log` for `EZMicroBalance`. Natural route-click first-node path remains pending. | Pending gameplay proof |
| Disable Spire Plus and restart | Spire Plus patches do not apply while disabled or plugged off | Pending gameplay proof |
| Keep legacy `EzDailyContent` disabled for private beta testing | No duplicate Ancient Harmony patches from legacy local artifacts | Pending normal user-profile check |

Result: pending.

## Prismatic Gem

v4.3 is current. All-slot behavior is retained with safer hook ordering.

### First Normal Reward, Reroll

1. Start or load a run with Prismatic Gem and standard reward counter `0`.
2. Complete one normal monster combat with a normal card reward.
3. Open the reward screen.
4. Record all visible card slots, color, and rarity.
5. Confirm the relic hover shows `初始计数1/2` in zhs, or the English equivalent.
6. Reroll if available and confirm rerolling does not force off-color replacements.

Expected result: the first normal reward increments the counter and stays non-triggered through rerolls.

### Second Normal Reward, Reroll

1. Start or load a run with Prismatic Gem and standard reward counter `1`.
2. Complete one normal monster combat with a normal card reward.
3. Open the reward screen.
4. Confirm every visible option is off-color / `异色牌`, preserving original type and rarity when a matching replacement exists.
5. Reroll if available and confirm the reward remains all off-color.
6. Confirm the relic hover shows `初始计数0/2` or `初始计数1/2`, depending on the live reset timing.
7. If the reward banner is visible, confirm it shows `本次标准卡牌奖励只会出现异色牌。`.
8. Review `godot.log`. If the trigger reward banner is not visible, confirm `PrismaticGem reward-screen hint fallback` is logged with the `_banner` reason or the `UI/Banner fallback`. If both banner paths fail, relic hover count plus every visible reward card being off-color remain the available confirmation surfaces; the log should say visible all-off-color cards and relic hover count remain the available confirmation surfaces.

Expected result: the second normal reward triggers all-slot off-color replacement and reroll preserves that triggered state.

### Non-Normal Rewards

1. Open elite, boss, event, shop, colorless-only, and other non-normal rewards when practical.
2. Confirm none of these screens increment Prismatic Gem's standard reward counter.
3. Confirm none of these screens replace visible card options.

Expected result: only standard monster card rewards advance and trigger Prismatic Gem.
## Ancient Reward Matrix

| Ancient / Relic | Manual Check | Expected Result | Result |
| --- | --- | --- | --- |
| Pael's Horn | Pick up the reward and inspect the added cards. | Adds one `Relax` / `放松` and one `Relax+` / `放松+`. Regression text includes `抽1张牌`, `每回合7张`, `至少18点`. | Pending |
| Black Star | Pick up in Act 3 or later, then kill an elite. | Pickup immediately grants one random relic; normal elite bonus remains. | Pending |
| War Hammer | Pick up reward, then kill an elite. | Pickup chooses two cards to upgrade; elite kill upgrades remain. | Pending |
| Jewelry Box | Pick up reward, inspect deck, then save/load or enter the next combat. | Adds `Apotheosis` / `神化` without `Innate` / `固有`; the added card must not start in opening hand from Innate. | Pending |
| Preserved Fog / Folly | Pick up reward, remove four cards, inspect Folly. | Adds `Folly` / `愚行` with Unplayable, `固有`, and `永恒`. | Pending |
| Vakuu's Sere Talon | Pick up Vakuu reward (`瓦库原初之爪`) and inspect deck. | Shows a 4-Curse choice; after choosing 1 Curse, deck gains that Curse, two `Wish` / `许愿`, and one upgraded `Wish+` / `许愿+`; verify event-option art, relic-bar art, inspect art, hover text, and surface-specific log routes are not Tanx Claws. | Pending |
| Tanx Claws | Pick up Tanx reward (`坦克斯利爪`). | Transforms selected cards into upgraded `Maul+` / `撕咬+`. | Pending |
| Choices Paradox | Start combat after pickup. | Five rare choices are offered, retained, and combat temporary. | Pending |
| Jeweled Mask | Select or draft a power, save/load, then enter combat. | Selected power permanently costs 0 and starts in hand; the `宝石面具` tooltip explains the permanent 0-cost effect. | Pending |
| Prismatic Gem | Run the exact tests above and inspect `godot.log`. | Counter increments once per standard reward screen; every second eligible screen makes every visible option off-color, shows the count hover hint, applies the localized banner, and reroll preserves trigger state. | Pending |
| Distinguished Cape | Inspect Vakuu options and pick up the reward at max HP values around 80, 70, 30, 19, 18, and 10 if practical. | Uses `lose 30% of current Max HP, at least 18`; cannot be selected when current Max HP is not greater than the v4.3 cost; Vakuu must still show three normal reward options; localized locked Cape only as a defensive fallback; low-Max-HP Vakuu still shows three normal choices; zhs text should include `至少18点`; max HP loss is not damage; adds exactly 3 `Apparition` / `灵体` cards. | Pending |
| Velvet Choker | Play 6 cards manually from hand, then inspect/play the 7th+ card. | No hard six-card cap; the 7th+ from-hand manual cards cost +1 after other cost changes; copied, autoplayed, or repeated plays do not advance the counter. | Pending |
| Pael's Tooth | Remove five cards, run two non-boss combats, then act transition. | One stored card returns upgraded every two non-boss combats; remaining stored cards clear after act boss transition. | Pending |
| Sovereign Blade / Forge | Forge a temporary Sovereign Blade, hover Forge/锻造, then play Sovereign Blade. | Forge hover explains the blade's added boons. Forged temporary Sovereign Blade has Exhaust; permanent Refine Blade is unchanged. Playing Sovereign Blade grants 3 Strength, 3 Dexterity, 3 Plating, 3 Regen, and 3 Vigor. | Pending |
| Seal of Gold / Debt | Pick up reward, draw/play/exhaust Debt. | Grants energy and two playable `Debt` / `债务` curses; gold loss occurs only on exhaust. | Pending |
| Sozu | Pick up with empty potion slots, then attempt future potion gain. | Empty potion slots fill on pickup, then future potion gain is blocked. | Pending |
| Ectoplasm | Pick up, then complete later gold rewards. | Grants immediate gold, then blocks future gold gain. | Pending |
| Fiddle | Draw many cards in combat. | Draw stops while `每回合7张`; no card should be lost or stuck. | Pending |
| Iron Club | Play Ironclad attacks and inspect dynamic text. | Uses current variable text; source guard expects 5 cards where applicable. | Pending |
| Brilliant Scarf | Draw from the reward and inspect dynamic text. | Uses current variable text; source guard expects 6 cards where applicable. | Pending |
| Beautiful Bracelet | Apply to selected cards and inspect enchantment. | Selected cards gain `Swift 2` / `迅捷2`; zhs text does not show raw `Swift`. | Pending |
| Music Box | Play an eligible card in combat. | Creates a discounted Ethereal Exhaust copy. | Pending |
| Crossbow | Start enemy turn with Crossbow. | Generated attack is temporary; skipped card does not linger. | Pending |
| Toasty Mittens | Start turn with a draw pile. | Top draw-pile card can be exhausted for Strength or kept. | Pending |
| Whispering Earring | Start turn with eligible high-cost cards. | Auto-plays one highest-cost card once per turn. | Pending |
| Brilliant Flame / Brightest Flame | Pick up Brilliant Flame, inspect BrightestFlame, then play upgraded and unupgraded versions. | BrightestFlame visibly has Exhaust and draws one more card than vanilla through dynamic text. | Pending |
| Meat Cleaver | Visit a rest site with at least two removable cards and more than 5 HP. | Adds a Cleaver option that removes 2 cards and loses 5 HP; disabled below the requirement. | Pending |
| Blood-Soaked Rose / Enthralled | Pick up reward, draw Enthralled, then play it. | Adds `Enthralled` / `执迷`; it must be played before other cards while in hand, gives Block, and is Eternal. | Pending |

## Simplified Chinese Localization Spot Checks

| Area | Expected Text | Result |
| --- | --- | --- |
| Beautiful Bracelet relic text | `迅捷2`, no raw `Swift` | Pending |
| Numeric formatting | No spaces between Chinese text, numbers, and units: `抽1张牌`, `每回合7张`, `至少18点` | Pending |
| Prismatic Gem count hint | zhs hover uses `初始计数1/2` or `初始计数0/2`; trigger reward banner uses `本次标准卡牌奖励只会出现异色牌。` | Pending |
| Jewelry Box | `神化`, no raw `Apotheosis`; `宝石面具` and 0-cost text | Pending |
| Pael's Horn | `放松` and `放松+`, no raw `Relax` | Pending |
| Vakuu's Sere Talon | `许愿`, no raw `Wish`; no Tanx Claws art/title on Vakuu reward | Pending |
| Tanx Claws | `坦克斯利爪` and `撕咬+`; no `许愿+` text on Tanx reward | Pending |
| Folly | `愚行`, no raw `Folly` | Pending |
| Enthralled | `执迷`, no raw `Enthralled` | Pending |
| Debt | `债务`, no raw `Debt` | Pending |
| Pael's Tooth | `首领`, no raw `Boss` | Pending |
| Shared keywords | `固有`, `永恒`, `虚无`, `消耗`, `保留`, `临时` | Pending |
## Save/Load Sensitive Rows

| Area | Manual Check | Result |
| --- | --- | --- |
| Jeweled Mask free power | Permanent 0-cost enchantment survives save/load. | Pending |
| Prismatic Gem counter | Saved standard reward counter survives save/load and is not advanced by reroll. | Pending |
| Pael's Tooth stored cards | Stored removed cards and combat counter survive save/load. | Pending |
| Ancient reward state | Urda/Morvi/Lotha/Vakuu marker relics and saved fields survive save/load. | Pending |
