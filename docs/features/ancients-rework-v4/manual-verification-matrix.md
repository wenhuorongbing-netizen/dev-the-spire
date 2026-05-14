# Manual Verification Matrix

Prepared: 2026-05-07

Status: automated gates passed; current normal Steam-client startup/log verification passed; refreshed normal Steam-client Mod Settings UI list screenshot shows Spire Plus; historical page-level Mod Settings UI passed under the old display name; A0/A10/A20 single-player DevConsole combat smoke passed; A11 Act 1 map/save-load spot check and saved-map boss-reachability graph proof passed; A11 Act 2/3 map-surface observation passed; targeted A14 Rootblight English/ZHS hover/starter-notice spot checks passed. Latest Urda UI/art, Lotha, and Vakuu package refresh reran build/publish plus normal and opt-in artifact tests, but did not rerun live verification. Full live Ancient reward gameplay, Rootblight combat-end behavior/notices, natural route-click first-node checks beyond the A11 spot check, Ancient save/load, natural A11 click-by-click traversal, and multiplayer verification are still pending.

Focused regression notes:

- Velvet Choker: card-library/card-compendium sorting must not crash when it computes costs for canonical cards with no owner. The soft-limit tax must apply only to real in-combat hand cards.
- Distinguished Cape: when the player cannot pay the v4.3 max-HP cost, Vakuu must still show three normal reward options; an otherwise rolled Cape should be replaced by a payable Pool 2 option, with a localized locked Cape only as a defensive fallback.
- Prismatic Gem: triggered reward screens should show the reward-screen banner hint when available. If the banner cannot be located, `godot.log` should contain `PrismaticGem reward-screen hint fallback`, and testers should use the relic hover count plus every visible reward card being off-color as fallback evidence.
- Pumpkin Candle: this is now a vanilla/no-EZMB-override spot check only. EZMB must not add an Act 3 extinguish-upgrade patch, sentinel, or localization override.
- Quality Flame / Brightest Flame: `BrightestFlame` should visibly show Exhaust, draw one more card than vanilla, and use dynamic text so unupgraded draw is 3 and upgraded draw is 4.
- Urda, Loamweaver: custom Ancient icon/run-history/background scene paths are now mod-owned after the 2026-05-13 negative A14 probe exposed missing vanilla-derived asset paths. The installed-PCK resource-load check at `.tools/runtime-evidence/urda-pck-resource-load-20260513-123345` resolves the custom scene/icon with 0 `ERROR` / `WARNING` lines. Post-fix live Urda and Rootblight visual/gameplay verification remains pending.

## Automated Prerequisites

| Check | Result |
| --- | --- |
| `dotnet build` | Pass, 0 warnings, 0 errors |
| `dotnet publish` | Pass, copied `EZMicroBalance.dll`, `EZMicroBalance.json`, and exported `EZMicroBalance.pck` |
| PCK package audit | Pass, 70 total PCK directory entries, 44 active non-Godot/project entries, and 0 legacy/source/docs/art/archive entries |
| English JSON parse | Pass |
| Simplified Chinese UTF-8 JSON parse | Pass |
| Simplified Chinese banned-English localization guard | Pass after 2026-05-06 localization sprint |
| Simplified Chinese no-space numeric formatting guard | Pass after v4.3 localization refresh |
| Beautiful Bracelet `Swift` zhs regression guard | Pass after 2026-05-06 localization sprint; retained text expects `迅速2` |
| Jeweled Mask custom enchantment zhs guard | Pass after 2026-05-06 localization sprint |
| Jewelry Box non-Innate source/serialization guard | Pass, automated source guard confirms marked-only `Apotheosis` handling |
| Ancient and Ascension source guard tests | Pass, expanded `dotnet test EZMicroBalance.sln --no-build` guard suite |
| Private-beta package | Pass for package refresh and release-artifact tests, `publish\SpirePlus-v0.1.0-private-beta.0.zip`, SHA256 `681929E84F694A3C644070F3562F00377C3ED6E00804FF250E469A2F75DADB84` |

## Runtime Load Checklist

| Step | Expected Result | Result |
| --- | --- | --- |
| Confirm `D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` exists | `BaseLib.json`, `BaseLib.dll`, and `BaseLib.pck` are present | Prepared |
| Confirm `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` exists | `EZMicroBalance.json`, `EZMicroBalance.dll`, and `EZMicroBalance.pck` are present | Prepared |
| Launch Slay the Spire 2 public beta | Game reaches main menu | Pass in controlled `--force-steam off` smoke profile and current isolated normal Steam-client startup log; normal Steam-client A0/A10/A20 combat smoke also reached live combat |
| Open Settings -> Mod Settings | BaseLib appears and is enabled | Pass, normal Steam-client screenshots and clean log captured during the isolated `095137` recheck |
| Open Settings -> Mod Settings | Spire Plus appears with id `EZMicroBalance` and can be enabled | Pass for visible mod list: current normal Steam-client evidence at `.tools\runtime-evidence\current-spire-plus-modsettings-20260513-111342\02-mod-config-list.png` shows `Spire Plus`; the same log confirms `Spire Plus (EZMicroBalance)`, `Registered config for mod EZMicroBalance`, and 0 release-blocking signatures. Historical page-level UI evidence remains available for the same manifest id under the old EZ Micro Balance display name. |
| Open Compendium -> Card Library / card encyclopedia with only BaseLib and EZ Micro Balance enabled | Card lists display and filtering/sorting works without `VelvetChokerSoftLimitTracker.ShouldTax` or `CanonicalModelException` errors in `godot.log` | Pending |
| Start a run with EZ Micro Balance enabled | No startup exception in `godot.log` for `EZMicroBalance` | Pass for normal Steam-client A0/A10/A20 Ironclad standard run starts followed by DevConsole `fight CULTISTS_NORMAL`; logs show 0 EZMB error/exception pattern hits. Natural route-click first-node path remains pending. |
| Disable Spire Plus / `EZMicroBalance` and restart | Spire Plus / `EZMicroBalance` patches do not apply while disabled or plugged off | Pass for BaseLib-only normal Steam plug-off startup/log evidence at `.tools\runtime-evidence\live-spire-plus-disabled-session-20260513-143020`; gameplay pass pending |
| Ensure legacy `EzDailyContent` is disabled for private beta testing | No duplicate Ancient Harmony patches from legacy local artifacts | Pass for the isolated Mod Settings recheck because only BaseLib and EZ Micro Balance were present in the temporary mods directory; normal user-profile tests should still keep EzDailyContent disabled or absent |

Automated smoke attempts on 2026-05-05 did not reach mod loading:

- Direct exe launch failed before mod loading with Steamworks `No appID found`.
- Direct exe launch with temporary `steam_appid.txt` value `2868840` failed before mod loading with Steamworks `ConnectToGlobalUser failed`; the temporary file was removed.
- `steam.exe -applaunch 2868840` did not start a detectable game process during the bounded smoke-test window.

Runtime result: normal Steam-client startup/log verification has been refreshed for the current Spire Plus display-name package under `.tools\runtime-evidence\current-spire-plus-normal-steam-20260513-054241`. Current normal Steam-client Mod Settings UI list evidence under `.tools\runtime-evidence\current-spire-plus-modsettings-20260513-111342` shows `Spire Plus` for the current display-name package; historical page-level Mod Settings UI evidence remains available for the same manifest id under the old display name. A0/A10/A20 single-player combat-smoke screenshots were collected through normal Steam-client standard runs plus DevConsole `fight CULTISTS_NORMAL`; these confirm draw/energy/combat initialization but do not replace natural route-click or feature-specific manual checks. Targeted A14 Rootblight English/ZHS hover/starter-notice screenshots were collected under `.tools\runtime-evidence\rootblight-a14-hover-eng-20260509-044010`, `.tools\runtime-evidence\rootblight-a14-ui-eng-20260509-033516`, and `.tools\runtime-evidence\rootblight-a14-notice-zhs-step-20260509-040455`.

Controlled smoke update:

- A temporary default-profile settings edit enabled only `BaseLib` and `EZMicroBalance` and disabled other discovered local mods, including legacy `EzDailyContent`; original settings were restored afterward.
- First controlled smoke exposed two invalid Harmony targets. `SealOfGoldMaxEnergyPatch` and `CrossbowOfferPatch` were retargeted to `AbstractModel` hook methods.
- Final controlled smoke loaded BaseLib, loaded `EZMicroBalance.dll`, loaded `EZMicroBalance.pck`, finished EZ Micro Balance initialization, and reached main menu.
- Controlled disable smoke enabled BaseLib, explicitly disabled EZ Micro Balance, skipped loading `EZMicroBalance`, did not load its DLL, and reached main menu.
- After the Release solution mapping fix, an isolated controlled smoke enabled only BaseLib and EZ Micro Balance, loaded exactly 2 mods, loaded the installed Release `EZMicroBalance.dll`, finished both initializers, and reached main menu. Original default-profile settings were restored afterward.
- Current normal Steam-client helper startup/log/resource pass under `.tools\runtime-evidence\current-package-smoke-20260514-015901` launched through Steam, loaded exactly BaseLib and Spire Plus / `EZMicroBalance`, logged `Registered config for mod EZMicroBalance`, reported `Found 22 SavedSpireFields`, reached main menu, audited clean, restored settings/moved mods/current-run files, and left 0 `SlayTheSpire2` processes.
- The same current-package evidence includes headless installed-PCK resource loading for Urda/Morvi/Lotha scenes and 43 Ancient textures with 0 errors/warnings, but clicked live Ancient UI screens and gameplay/manual rows remain pending.
- Current BaseLib-only normal Steam plug-off startup/log pass under `.tools\runtime-evidence\live-spire-plus-disabled-session-20260513-143020` temporarily isolated `EZMicroBalance` out of the mods folder, launched through Steam, loaded `1 mods (1 total)`, initialized BaseLib only, did not initialize Spire Plus / `EZMicroBalance`, audited clean, and restored settings, the current-run save, and 25 moved entries. The earlier settings-only disabled attempt under `.tools\runtime-evidence\live-spire-plus-disabled-session-20260513-142835` is invalid plug-off evidence because Spire Plus still initialized.
- Manual reward behavior tests below are still pending.

## Prismatic Gem Exact Tests

v4.3 is current. v4.2 rightmost-slot Prismatic Gem is historical only.

Run with BaseLib and EZ Micro Balance enabled. Disable other mods that alter card rewards. Avoid custom-pool, filtered-pool, colorless-only, no-model-modification, elite, boss, and event rewards unless the step explicitly asks for them.

### 1. First Normal Reward, Reroll

1. Start or load a run that has Prismatic Gem and a saved normal reward counter of `0`.
2. Complete one normal monster combat that grants a normal card reward.
3. Open the card reward screen.
4. Record all three card slots, including each slot's color and rarity.
5. Confirm the relic hover text shows `棱彩计数：1/2` after this reward screen opens if the current language is zhs, or the English equivalent in English; this means the next standard card reward should be all off-color.
6. Reroll the same reward screen.
7. Record the three card slots again.
8. Repeat the reroll at least two more times if rerolls are available.
9. Leave the reward screen, then reach the next normal monster card reward.

Expected result: the first reward screen stays non-triggered through every reroll. Rerolling must not force any slot into an off-color replacement. The next normal monster reward remains the every-second trigger.

Result: pending.

### 2. Second Normal Reward, Reroll

1. Start or load a run that has Prismatic Gem and a saved normal reward counter of `1`, or use the next normal monster reward after completing test 1.
2. Complete a normal monster combat that grants a normal card reward.
3. Open the card reward screen.
4. Confirm every visible option is off-color / `棱彩奖励：本次只出现异色牌。` with each slot preserving its original rarity when a same-rarity off-color replacement is available.
5. Reroll the same reward screen.
6. Confirm the rerolled reward screen still has every visible option off-color and does not duplicate any visible card where an alternative exists.
7. Repeat the reroll at least two more times if rerolls are available.
8. Confirm the relic hover text shows `棱彩计数：2/2` after this reward screen opens if the current language is zhs, or the English equivalent in English; this means the next standard card reward should be normal.
9. If the reward banner text is visible in zhs, confirm it shows `棱彩奖励：本次只出现异色牌。`.
10. Review `godot.log` after the screen. Confirm the banner hint used the guarded `_banner` field or the `UI/Banner fallback`; if both failed, the log must say the visible all-off-color cards and relic hover count remain the available confirmation surfaces.

Expected result: the second normal reward screen stays triggered through every reroll. Rerolling must keep every visible option off-color, with no added reward slot and no duplicate card options where an alternative exists. The localized banner hint should render when either the guarded private banner field or the `UI/Banner` fallback is available; visual placement remains manual.

Result: pending.

### 3. Non-Normal Rewards

1. Start or load a run that has Prismatic Gem and a known saved normal reward counter.
2. Open an elite card reward.
3. Confirm no Prismatic Gem off-color replacement occurs.
4. Reroll the elite reward if the UI allows it.
5. Confirm rerolling still does not apply a Prismatic Gem replacement.
6. Open a boss card reward and repeat the same checks.
7. Open an event card reward and repeat the same checks.
8. Open a shop reward or colorless-only reward if available and repeat the same checks.
9. Open the next normal monster card reward.

Expected result: elite, boss, event, shop, colorless-only, and other non-normal rewards do not increment Prismatic Gem's normal reward counter and do not replace any slot. The next normal monster reward should behave exactly as it would have before opening the non-normal reward.

Result: pending.

## Ancient Reward Matrix

| Ancient / Relic | Manual Check | Expected Result | Result |
| --- | --- | --- | --- |
| Pael's Horn | Pick up the reward. | Adds one `Relax` / `放松` and one upgraded `Relax+` / `放松+`; for regression checks, shows `获得1点能量`, `手牌有7张`, `至少18点`. | Pending |
| Black Star | Pick up in act 3 or later, then kill an elite. | Pickup immediately grants one random relic; normal elite bonus remains. | Pending |
| War Hammer | Pick up reward, then kill an elite. | Pickup chooses two cards to upgrade; elite kill upgrades remain. | Pending |
| Jewelry Box | Pick up reward, inspect deck, then save/load or enter the next combat. | Adds `Apotheosis` / `神化` without `Innate` / `固有`; the added card must not start in opening hand from Innate. Other non-Jewelry Box Apotheosis sources are expected to keep their normal Innate behavior. Save/load persistence is now guarded by the marker serialization hook but still needs runtime verification. | Pending manual gameplay; automated source/serialization guard passed |
| Preserved Fog / Folly | Pick up reward, remove four cards, inspect Folly. | Adds `Folly` / `愚行` with Unplayable, `Innate` / `固有`, and `Eternal` / `永恒`. | Pending |
| Claws | Pick up reward and choose curse. | Chooses one curse from four and adds two `Wish` / `许愿` plus one upgraded `Wish+` / `许愿+`. | Pending |
| Choices Paradox | Start combat after pickup. | Five rare choices are offered, retained, and combat temporary. | Pending |
| Jeweled Mask | Select or draft a power, save/load, then enter combat. | Selected power permanently costs 0 and starts in hand instead of draw pile; zhs custom enchantment tooltip uses `宝石面具`. | Pending |
| Prismatic Gem | Run the exact tests above and inspect `godot.log`. | Counter increments once per standard reward screen; every second eligible screen makes every visible option off-color / `棱彩奖励：本次只出现异色牌。` shows the count hover hint, applies the localized banner through the guarded `_banner` field or `UI/Banner fallback`, and reroll preserves trigger state. | Pending |
| Distinguished Cape | Inspect Vakuu options and pick up the reward at max HP values around 80, 70, 30, 19, 18, and 10 if practical. | Uses `lose 30% of current Max HP, at least 18`; shows `至少18点`; cannot be selected when current Max HP is not greater than the v4.3 cost; an unaffordable Cape roll is replaced by a payable Vakuu Pool 2 option so low-Max-HP Vakuu still shows three normal choices; if current HP exceeds new max, max HP loss is not damage; adds exactly 3 `Apparition` / `幽灵` cards. | Pending |
| Velvet Choker | Play 6 cards manually from hand, then inspect/play the 7th+ card; also test an autoplay and a repeated/copy play. | No hard six-card cap; the 7th+ from-hand manual cards cost +1 after other cost changes; copied, autoplayed, or repeated plays do not advance the counter; counter resets on each player turn. | Pending |
| Pael's Tooth | Remove five cards, run two non-boss combats, then act transition. | One stored card returns upgraded every two non-boss / 非首领 combats; remaining stored cards clear after act boss / 首领 transition. | Pending |
| Sovereign Blade / Forge | Forge a temporary Sovereign Blade; inspect permanent Refine Blade. | Forged temporary Sovereign Blade has Exhaust; permanent Refine Blade is unchanged. | Pending |
| Seal of Gold / Debt | Pick up reward, draw/play/exhaust Debt. | Grants energy and two playable Debt curses; gold loss occurs only on exhaust. | Pending |
| Sozu | Pick up with empty potion slots, then attempt future potion gain. | Empty potion slots fill on pickup, then future potion gain is blocked. | Pending |
| Ectoplasm | Pick up reward, then attempt future gold gain. | Grants 250 gold on pickup, then future gold gain is blocked. | Pending |
| Fiddle | Start turns with hand below/above seven. | Draws toward seven and caps player-turn draw above seven; expects `手牌有7张` at the threshold. | Pending |
| Iron Club | Play five cards in combat. | Draws one card every five cards played. | Pending |
| Brilliant Scarf | Play six cards in one turn. | Sixth card each turn costs 0. | Pending |
| Beautiful Bracelet | Apply to selected cards and inspect enchantment. | Selected cards gain `Swift 2` / `迅速2`; zhs text does not show raw `Swift 2`. | Pending |
| Music Box | Play first attack each turn. | Creates a discounted Ethereal Exhaust copy. | Pending |
| Crossbow | Start turn, accept offer; repeat and skip offer. | Generated attack offer can be accepted or skipped; skipped card does not linger. | Pending |
| Toasty Mittens | Start turn with draw pile. | Top draw-pile card can be exhausted for Strength or kept. | Pending |
| Whispering Earring | Start turns 1, 2, and 3 with playable cards in hand, including self-targeting or no-target cards. | Auto-plays one highest-cost playable hand card after draw; self-targeting and no-target playable cards are not filtered out by enemy/ally targeting checks. | Pending |
| Pumpkin Candle vanilla spot check | Obtain Pumpkin Candle and progress through the normal vanilla Act 3 extinguish timing. | No EZMB `internal static class PumpkinCandlePatch`, `ExtinguishedSentinel`, or active `PUMPKIN_CANDLE.description` override is present; behavior and text should match the current vanilla game. | Pending |
| Quality Flame / Brightest Flame | Obtain Quality Flame through Storybook or another source, inspect unupgraded/upgraded previews, then play it. | The card visibly has Exhaust; unupgraded text and behavior draw 3, upgraded text and behavior draw 4, it gains the vanilla dynamic Energy amount, loses 1 Max HP, and exhausts after play. Pumpkin Candle relic behavior remains vanilla. | Pending |
| Meat Cleaver | Open rest site with/without valid cards and HP. | Cook removes two cards and loses five current HP; disabled when unavailable. | Pending |
| Blood-Soaked Rose / Enthralled | Gain Enthralled and observe combat priority. | `Enthralled` / `执迷` gains 10 Block while preserving forced-priority behavior. | Pending |

## Simplified Chinese Localization Spot Checks

| Surface | Expected zhs Text | Result |
| --- | --- | --- |
| Beautiful Bracelet relic text | `迅速2`, no raw `Swift` | Pending |
| Numeric formatting | No spaces between Chinese text, numbers, and units, such as `拥有1张牌` and `至少18点` | Pending |
| Prismatic Gem count hint | zhs hover uses `棱彩计数：1/2` or `棱彩计数：0/2`; trigger reward banner uses `棱彩奖励：本次只出现异色牌。`; if the banner path falls back, `godot.log` names the `UI/Banner fallback` or the final unavailable diagnostic. | Pending |
| Jewelry Box relic text | `神化`, no raw `Apotheosis` | Pending |
| Pael's Horn relic text | `放松` and `放松+`, no raw `Relax` | Pending |
| Claws relic text | `许愿` and `许愿+`, no raw `Wish` | Pending |
| Preserved Fog relic text | `愚行`, no raw `Folly` | Pending |
| Blood-Soaked Rose relic text | `执迷`, no raw `Enthralled` | Pending |
| Seal of Gold relic/card text | `债务`, no raw `Debt` and no obsolete debt relic | Pending |
| Pael's Tooth relic text | `首领`, no raw `Boss` | Pending |
| Jeweled Mask custom enchantment | zhs tooltip uses `宝石面具` and 0-cost text | Pending |
| Keyword surfaces | `保留`, `虚无`, `消耗`, `固有`, `永恒`, `力量` | Pending |
## Save/Load Matrix

| Surface | Expected Result | Result |
| --- | --- | --- |
| Prismatic Gem counter | Saved standard reward counter survives save/load and is not advanced by reroll. | Pending |
| Pael's Tooth stored cards | Stored removed cards and combat counter survive save/load. | Pending |
| Jeweled Mask free power | Permanent 0-cost enchantment survives save/load. | Pending |
| Debt | Loaded Debt still loses gold only when exhausted. | Pending |
| Folly | Loaded Folly preserves Unplayable, Innate, and Eternal. | Pending |
| Jewelry Box Apotheosis | Loaded or room-transitioned Jewelry Box `Apotheosis` still lacks Innate, while other Apotheosis instances keep base Innate. | Pending |
