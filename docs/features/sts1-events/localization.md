# StS1 Events Localization

## Current Status

Current beta.92 resource/package coverage treats the StS1 event localization files as no-game file-parity verified, not source-reference complete and not runtime-render verified.

| File | Language | Current key count | Status |
|------|----------|-------------------|--------|
| `EZMicroBalance/localization/eng/sts1_events.json` | English | 397 | file-parity only; source-reference gaps exist |
| `EZMicroBalance/localization/zhs/sts1_events.json` | Simplified Chinese | 397 | file-parity only; source-reference gaps exist |

This proves file-level key parity only. A 2026-06-11 static scan found 33 source-referenced result-page keys missing in both EN and ZHS; see `docs/features/sts1-events/localization-source-gap-scan-20260611.md`. Use `docs/features/sts1-events/localization-gap-closure-plan.md` for the validated resource-pass order. It also does not prove in-game EN/ZHS rendering, font behavior, layout, option-lock text, save/load, or event gameplay.

## Key Convention

All StS1 event localization keys use the `STS1_` prefix to distinguish them from native StS2 events. Keys follow the standard event localization pattern:

```text
STS1_{EVENT_NAME}.title
STS1_{EVENT_NAME}.pages.INITIAL.description
STS1_{EVENT_NAME}.pages.INITIAL.options.{OPTION}.title
STS1_{EVENT_NAME}.pages.INITIAL.options.{OPTION}.description
STS1_{EVENT_NAME}.pages.{PAGE}.description
```

Event class names are slugified through `StringHelper.Slugify` for the entry prefix. Examples:

```text
Sts1BigFish -> STS1_BIG_FISH
Sts1GoldenIdol -> STS1_GOLDEN_IDOL
Sts1DivineFountain -> STS1_DIVINE_FOUNTAIN
```

## Current Guarded Corrections

The following source/localization shapes are current and guarded:

| Event | Current localization shape | Guard / evidence |
|-------|----------------------------|------------------|
| Big Fish | Uses `BOX`, not Bowl, for the random relic + Regret option. | `BigFishUsesBoxOptionName` |
| Golden Idol | Trap branch uses `OUTRUN`, `SMASH`, and `HIDE` keys, including result pages. | `GoldenIdolTrapOptionsUseWikiBranchNames` |
| The Lab | Uses only `OPEN`; unused `LEAVE` keys were removed. | `TheLabHasOnlyOpenOption` |
| Divine Fountain | Uses `DRINK` / `LEAVE`; event eligibility is source-guarded by curse ownership. | `DivineFountainRequiresEveryPlayerToHaveACurseAndUsesDrinkOption` |
| Golden Shrine | Uses `PRAY`, `DESECRATE`, and `LEAVE`; Pray is 100 gold, 50 at A15+, and Desecrate grants 275 gold plus Regret. | `GoldenShrineUsesWikiGoldAndRegretOptions` |
| The Cleric | Heal and Purify descriptions reflect 35 gold heal, 50 gold Purify, and 75 gold Purify at A15+. | `TheClericUsesA15PurifyCostAndGoldEligibility` |
| Old Beggar | Offer Gold text is tied to 75 gold affordability. | `OldBeggarOfferRequiresEnoughGold` |
| Shining Light | Enter text reflects random card upgrades without a manual picker. | `ShiningLightUpgradesRandomCardsWithoutManualSelection` |

## Source-Reference Gap

Current static source-reference coverage is not complete. Missing keys are tracked in `localization-source-gap-scan-20260611.md`, and the next validated closure order is tracked in `localization-gap-closure-plan.md`.

Reproduce the static check with:

```powershell
.\scripts\check-sts1-localization-source-keys.ps1
.\scripts\check-sts1-localization-gap-baseline.ps1 -FailOnMismatch
```

Impact split:

- 1 missing key affects current CanaryOnly/AdditiveBatch1 directly: `STS1_GOLDEN_IDOL.pages.LEAVE.description`.
- 32 missing keys are later RegisterAll/draft or blocked-combat surfaces.
- The gap-baseline guard currently expects the full split to stay `1 / 6 / 9 / 6 / 11` until a versioned localization/resource pass intentionally changes it.

Do not add these resource keys in isolation during a validation pause. Adding them changes player-visible localization resources and therefore requires package versioning, build/publish/package validation, and updated handoff docs before delivery. The closure plan separates the one direct enabled-mode blocker from the remaining 32 later/blocked keys so a future validated pass can choose either a minimal unblocker or full source-reference closure.

Closing only `STS1_GOLDEN_IDOL.pages.LEAVE.description` remains a localization unblocker; it does not prove gameplay, and it does not replace `enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` verifier reports.

## Dynamic Variables

Use the exact variable names exposed by each event model and option annotation. Do not invent new placeholder names in localization without adding matching source support and guard coverage.

Known current placeholders include:

| Placeholder | Meaning |
|-------------|---------|
| `{HealAmount}` | Heal amount |
| `{MaxHpAmount}` | Max HP loss amount |
| `{DamageAmount}` | Damage amount |
| `{GoldAmount}` | Gold amount |
| `{Cost}` | Gold or other option cost |

Some descriptions intentionally use static wording when the source displays values through option annotations or when the copy is ratio-based, such as Big Fish's one-third max HP heal.

## Runtime Proof Still Pending

Before claiming localization complete in gameplay, capture:

1. English screenshots for CanaryOnly and AdditiveBatch1 event options and result pages.
2. Simplified Chinese screenshots for the same event options and result pages.
3. Evidence that dynamic placeholders render as numbers, not raw placeholder text.
4. Evidence that locked/unavailable options fit and display correctly.
5. `godot.log` and `godot-log-audit.json` for the same run.

Current beta.92 Off/AdditiveBatch1 loader proof is not localization render proof.
