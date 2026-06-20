# Spire Plus Manual Test Checklist

This checklist is for the independent private-beta mod: manifest id `EZMicroBalance`, user-facing name `Spire Plus`.

Keep the legacy `EzDailyContent` scaffold disabled or absent during these checks to avoid duplicate Ancient patches.

Do not treat the feature as private-beta ready until these checks are complete on the target Slay the Spire 2 public beta version.

## Environment

- Game branch: public beta
- Target game version: public beta `v0.107.1`, source snapshot refreshed locally on `2026-06-20` per `docs/dev-environment.md` and `PROJECT_STATE.md`
- Legacy baselines: `v0.104.0` (`2026.04.23`) and the later `v0.106.1` / BaseLib validation lane are historical only and are not the target for this checklist.
- Runtime framework: `STS2-RitsuLib` `v0.4.28` with `lib\0.107.1`
- Expected RitsuLib folder: `<GameRoot>\mods\STS2-RitsuLib`
- Expected mod folder: `<GameRoot>\mods\EZMicroBalance`

## Pre-flight

- Close any running Slay the Spire 2 process before publish or Release install-copy builds so installed mod files can be overwritten. Default Debug `dotnet build` does not overwrite installed release artifacts.
- Run `dotnet build`.
- Run `dotnet publish`.
- Launch the game.
- Open Settings -> Mod Settings.
- Confirm RitsuLib appears.
- Confirm RitsuLib is enabled.
- Confirm `Spire Plus` appears with manifest id `EZMicroBalance`.
- Confirm `Spire Plus` is enabled.
- Confirm legacy Easy Content / EzDailyContent is disabled or absent.
- Check `godot.log` for `EZMicroBalance`, `STS2-RitsuLib`, `BaseLib`, `EzDailyContent`, `error`, and `exception`. BaseLib may appear only as unrelated previous-package/other-mod context; it must not be required by the current Spire Plus package.

## High-priority Ancient reward checks

Record for each item:

- Ancient encountered
- Option selected
- Expected behavior
- Actual behavior
- Screenshot or log note
- PASS / FAIL
- Bug notes

### Pael's Horn

- Expected: on pickup, add 1 `Relax` / `放松` and 1 `Relax+` / `放松+` to deck.
- Verify deck count and upgraded state.

### Pael's Tooth

- Expected: remove/store cards on pickup according to implemented behavior.
- Verify after 1 non-boss combat.
- Verify after 2 non-boss combats.
- Verify upgraded returned card selection.
- Verify act boss cleanup.
- Verify save/load if possible.

### Prismatic Gem

- Expected: Every second standard card reward contains only off-color cards.
- Verify first normal reward: no replacement.
- Verify second normal reward: every visible option is off-color / `异色牌`.
- Verify non-normal reward does not increment or misfire.
- Verify reroll/reward screen behavior preserves the same all-off-color trigger state and does not duplicate or remove wrong cards where alternatives exist.
- If the trigger reward banner is not visible, check `godot.log` for `PrismaticGem reward-screen hint fallback` and confirm the fallback evidence: the relic hover count plus every visible reward card being off-color.
- Verify relic hover count shows `棱彩计数：0/2` for next normal reward and `棱彩计数：1/2` for next all-off-color reward in zhs.
- Verify trigger reward banner shows `棱彩奖励：本次只出现异色牌。` in zhs if the banner is visible.

### Distinguished Cape

- Expected: pickup uses `lose 30% of current Max HP, at least 18`; the option is not selectable unless current Max HP is greater than the calculated cost.
- Verify current HP is clamped to the new max HP without a damage hit, block loss, Intangible interaction, or damage-number mitigation.
- Verify exactly 3 Apparition / `灵体` cards are added.
- Verify Vakuu still shows three reward options when current max HP is not greater than the v4.3 cost. If Distinguished Cape would roll, it should be replaced by the other Pool 2 option; only the defensive fallback should show a locked Cape with an explicit low-Max-HP reason.

### Velvet Choker

- Expected: +1 Energy with no hard six-card cap.
- Verify the first 6 manually played cards from hand have no Choker tax.
- Verify the 7th and later manually played cards from hand cost +1 after other cost changes, including a 0-cost card becoming 1-cost.
- Verify an X-cost card pays the extra energy without increasing X.
- Verify copied, autoplayed, and repeated play instances do not advance the Choker counter.
- Verify the counter resets at each player turn start.

### Jeweled Mask

- Expected: choose a Power on pickup; selected Power permanently costs 0 and is moved from draw pile to hand at combat start.
- Verify no eligible Power fallback behavior.
- Verify combat start behavior.
- Verify save/load if possible.

### Meat Cleaver

- Expected: Cleaver / 切肉 option removes 2 removable cards and costs 5 HP.
- Verify option disabled when too few removable cards.
- Verify option disabled when HP is not greater than 5.
- Verify no other rest-site source is affected unexpectedly.

### Debt / Seal of Gold

- Expected: Seal of Gold adds playable `Debt` / `债务` curses.
- Expected: Debt exhausts and loses up to 5 gold when exhausted.
- Verify play behavior.
- Verify exhaust behavior.
- Verify zero-gold behavior.
- Watch for freezes or command-order bugs.

### Music Box

- Expected: each turn, first Attack creates a copy in hand with temporary cost -1, Ethereal, and Exhaust.
- Verify only once per turn.
- Verify reset next turn.
- Verify combat end cleanup.

### Crossbow

- Expected: at turn start, offer a random temporary Attack; accepted card gets cost -1, Ethereal, and Exhaust.
- Verify accept path.
- Verify skip path.
- Verify skipped generated card does not linger.

### Fiddle

- Expected: start-of-turn draw up to 7 cards, and player-turn draw effects cannot exceed 7 cards in hand.
- Verify normal hand draw.
- Verify additional draw card while hand has 7 or more cards.
- Verify enemy-side or non-player-side draw is not incorrectly blocked.

### Whispering Earring

- Expected: first 3 turns, after drawing, automatically play one highest-cost playable card.
- Verify targeting.
- Verify self-targeting and no-target playable cards are not incorrectly skipped by enemy/ally targeting checks.
- Verify unplayable hand behavior.
- Verify turn 4 no longer triggers.

### Toasty Mittens

- Expected: before drawing, view top draw-pile card and may exhaust it to gain Strength.
- Verify choose exhaust.
- Verify skip/keep path.
- Verify empty draw pile behavior.

### Other pickup-only rewards

Verify pickup behavior and player-facing text for:

- Black Star
- War Hammer
- Sozu
- Ectoplasm
- Vakuu's Sere Talon / 瓦库原初之爪: choose 1 of 4 Curses, then gain that Curse, 2 Wish, and 1 Wish+.
- Tanx Claws / 坦克斯利爪: transforms selected cards into upgraded Maul+ / 撕咬+.
- Jewelry Box
- Preserved Fog
- Beautiful Bracelet
- Iron Club
- Brilliant Scarf

### Jewelry Box focused regression

- Pick up Jewelry Box.
- Inspect the deck immediately after pickup.
- Expected: the added `Apotheosis` / `神化` does not show `Innate` / `固有`.
- Enter the next combat without save/loading first.
- Expected: the Jewelry Box `Apotheosis` / `神化` is not forced into the opening hand by `Innate`.
- Save/load or advance a room if possible, then inspect the same card again.
- Expected: the Jewelry Box `Apotheosis` / `神化` still does not show `Innate` / `固有`.
- If another source creates `Apotheosis` / `神化`, record the source separately.
- Expected: non-Jewelry Box copies keep the base game's normal `Innate` / `固有` behavior unless a future design explicitly changes that source.

## Localization checks

- English relic text matches behavior.
- Simplified Chinese relic text matches behavior.
- English card text matches behavior.
- Simplified Chinese card text matches behavior.
- Rest-site Cleaver text appears correctly in English.
- Rest-site 切肉 text appears correctly in Simplified Chinese.
- In Simplified Chinese, Beautiful Bracelet shows `迅速2` and not raw `Swift 2`.
- In Simplified Chinese, Jewelry Box shows `神化` and not raw `Apotheosis`.
- In Simplified Chinese, Pael's Horn shows `放松` / `放松+` and not raw `Relax`.
- In Simplified Chinese, Vakuu's Sere Talon shows `许愿`; Tanx Claws shows `撕咬+` and not Wish text.
- In Simplified Chinese, Preserved Fog shows `愚行` and not raw `Folly`.
- In Simplified Chinese, Blood-Soaked Rose shows `执迷` and not raw `Enthralled`.
- In Simplified Chinese, Seal of Gold and the card title show `债务` and not `Debt` or `欠款`.
- In Simplified Chinese, Pael's Tooth uses `首领` and not raw `Boss`.
- In Simplified Chinese, keyword text uses `保留`, `虚无`, `消耗`, `固有`, `永恒`, and `力量`.
- In Simplified Chinese, Jeweled Mask's custom enchantment tooltip uses `宝石面具` text.
- In Simplified Chinese, player-facing numbers have no spaces between Chinese text, numbers, and units, e.g. `获得1点能量`, `第7张`, `失去18点`.

## Failure report format

For every failed check, record:

```text
Item:
Expected:
Actual:
Steps to reproduce:
Screenshot/log:
Likely area:
Severity:
```

## Completion gate

The feature is private-beta ready only when:

- Build passes.
- Publish passes.
- Mod loads in game.
- No crash occurs during tested Ancient option selection.
- Every high-priority item above has PASS or a documented accepted known issue.
- `godot.log` has no new unhandled exception from Spire Plus under technical id `EZMicroBalance`.
- The private-beta package has been smoke-inspected for `EZMicroBalance.dll`, `EZMicroBalance.json`, `EZMicroBalance.pck`, and `README_INSTALL.txt`.
