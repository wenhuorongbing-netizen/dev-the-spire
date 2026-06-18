# StS1 Event Localization Source Gap Scan

Date: 2026-06-11
Scope: static source/resource scan only; no build, test, publish, package, or runtime validation was run.

## Summary

Repro command:

```powershell
.\scripts\check-sts1-localization-source-keys.ps1
.\scripts\check-sts1-localization-gap-baseline.ps1 -FailOnMismatch
```

The EN and ZHS StS1 localization files currently have file-level parity:

- `EZMicroBalance/localization/eng/sts1_events.json`: 397 keys.
- `EZMicroBalance/localization/zhs/sts1_events.json`: 397 keys.
- EN-only keys: 0.
- ZHS-only keys: 0.

However, a direct static scan of `EZMicroBalanceCode/Sts1Events/Models/**/*.cs` found 33 source-referenced localization keys missing from both EN and ZHS. These are mostly result-page descriptions referenced by `SetEventFinished(L10NLookup(...))` in draft events.

Current checker output:

- source files scanned: 48.
- expected source keys: 428.
- missing source-referenced keys: 33.
- gap baseline checker: 12 checks / 0 mismatches, with 1 direct enabled-mode blocker, 6 simple/later keys, 9 CardService keys, 6 blocked-combat keys, 11 CustomUi keys, and closure-plan cue coverage for all 33 missing keys.

This means the current 397/397 resource parity is not full source-reference coverage. Do not claim StS1 localization is source-complete until these keys are added or the source references are changed.

## Missing Source-Referenced Keys

Missing in both EN and ZHS:

- `STS1_ANCIENT_WRITING.pages.ELEGANCE.description`
- `STS1_ANCIENT_WRITING.pages.SIMPLICITY.description`
- `STS1_AUGMENTER.pages.MUTATE.description`
- `STS1_AUGMENTER.pages.TRANSFORM.description`
- `STS1_DESIGNER.pages.REMOVE.description`
- `STS1_DESIGNER.pages.TRANSFORM.description`
- `STS1_DESIGNER.pages.UPGRADE.description`
- `STS1_FALLING.pages.FLY.description`
- `STS1_FALLING.pages.HOLD_ON.description`
- `STS1_FALLING.pages.LET_GO.description`
- `STS1_FORGOTTEN_ALTAR.pages.DESECRATE.description`
- `STS1_FORGOTTEN_ALTAR.pages.OFFER.description`
- `STS1_FORGOTTEN_ALTAR.pages.PRAY.description`
- `STS1_GOLDEN_IDOL.pages.LEAVE.description`
- `STS1_KNOWING_SKULL.pages.QUESTION_1.description`
- `STS1_KNOWING_SKULL.pages.QUESTION_2.description`
- `STS1_KNOWING_SKULL.pages.QUESTION_3.description`
- `STS1_MASKED_BANDITS.pages.FIGHT.description`
- `STS1_MASKED_BANDITS.pages.PAY.description`
- `STS1_MIND_BLOOM.pages.AWAKE.description`
- `STS1_MIND_BLOOM.pages.RICH.description`
- `STS1_MIND_BLOOM.pages.WAR.description`
- `STS1_MOAI_HEAD.pages.OFFER.description`
- `STS1_MOAI_HEAD.pages.WORSHIP.description`
- `STS1_MYSTERIOUS_SPHERE.pages.OPEN.description`
- `STS1_SCORPION_NEST.pages.INVESTIGATE.description`
- `STS1_TOMB_OF_LORD_RED_MASK.pages.OFFER_50.description`
- `STS1_TOMB_OF_LORD_RED_MASK.pages.OFFER_ALL.description`
- `STS1_TREASURE_OOZE.pages.FIGHT.description`
- `STS1_TREASURE_OOZE.pages.OFFER.description`
- `STS1_WINDING_HALLS.pages.CONTINUE.description`
- `STS1_WINDING_HALLS.pages.EMBRACE.description`
- `STS1_WINDING_HALLS.pages.RETREAT.description`

## Enabled-Mode Impact

| Impact bucket | Missing keys | Notes |
| --- | ---: | --- |
| CanaryOnly / AdditiveBatch1 direct blocker | 1 | `STS1_GOLDEN_IDOL.pages.LEAVE.description`; Golden Idol is in CanaryOnly and AdditiveBatch1, so this key must be fixed before current enabled-mode UI proof can be considered localization-safe. |
| Simple phase, later RegisterAll only | 6 | Ancient Writing, Augmenter, and Moai Head result-page keys. These are not in AdditiveBatch1. |
| CardService phase, later RegisterAll only | 9 | Falling, Knowing Skull, and Mind Bloom result-page keys. |
| Combat phase, blocked encounter events | 6 | Scorpion Nest, Treasure Ooze, Masked Bandits, and Mysterious Sphere result-page keys; runtime parity is already blocked by missing encounter models. |
| CustomUi phase, later RegisterAll only | 11 | Designer, Forgotten Altar, Tomb of Lord Red Mask, and Winding Halls result-page keys. |

Priority for the next validated localization/resource pass:

1. Fix `STS1_GOLDEN_IDOL.pages.LEAVE.description` in EN and ZHS first because it affects current CanaryOnly/AdditiveBatch1 smoke and gameplay proof.
2. Fix the remaining 32 keys before any `AdditiveAllDraft` / RegisterAll gameplay or source-complete localization claim.
3. Keep combat-event runtime proof blocked separately until encounter models exist.

Fixing the direct Golden Idol key only removes a missing-key blocker. It does not prove gameplay behavior or replace `enabled-mode-log-check.json` and `runtime-evidence-packet-check.json`.

## Required Follow-Up

1. Add EN and ZHS entries for the missing source-referenced result-page keys, or change source to use existing keys.
2. Because this changes player-visible localization resources, increment the Spire Plus package version and refresh package docs/hashes before handoff.
3. After the validation pause is lifted, run the required build/publish/package validation for localization/resource changes.
4. Later runtime proof still needs EN/ZHS in-game screenshots; source-reference coverage is not render proof.

After the missing keys are resolved, run:

```powershell
.\scripts\check-sts1-localization-source-keys.ps1 -FailOnMissing
```

Also update or remove the current gap-baseline guard in that same validated localization pass. These static checkers are not a substitute for build, publish, package validation, or in-game EN/ZHS render proof.
