# StS1 Localization Gap Closure Plan

Date: 2026-06-11
Scope: static planning only. This plan changes no localization/resource files.

## Boundary

Do not add resource keys while the validation coordination pause is active. Adding or changing entries in `EZMicroBalance/localization/eng/sts1_events.json` or `EZMicroBalance/localization/zhs/sts1_events.json` is a player-visible localization/resource change. That pass must increment the Spire Plus package version, refresh package/handoff docs, and run the required build/publish/package validation before handoff.

Do not copy original StS1 event text. Write concise behavior-derived English and Simplified Chinese copy from the current Spire Plus source behavior and the local player-text rules.

## Current Gap

`localization-source-gap-scan-20260611.md` and `scripts/check-sts1-localization-gap-baseline.ps1` currently guard 33 source-referenced result-page keys missing from both EN and ZHS:

- 1 direct enabled-mode blocker: `STS1_GOLDEN_IDOL.pages.LEAVE.description`.
- 6 simple/later RegisterAll keys.
- 9 CardService/later RegisterAll keys.
- 6 blocked-combat keys.
- 11 custom-UI/later RegisterAll keys.

The files still have EN/ZHS file-level parity at 397 keys each. That parity is not source-reference completeness and not runtime render proof.

## Source Behavior Cues

Use these cues to write final EN/ZHS result-page copy during the validated localization/resource pass. These are behavior summaries, not approved final strings.

| Key | Source behavior cue |
| --- | --- |
| `STS1_GOLDEN_IDOL.pages.LEAVE.description` | Leave Golden Idol without the current random-relic substitute and without trap penalty. |
| `STS1_ANCIENT_WRITING.pages.ELEGANCE.description` | Open a card-upgrade selection. |
| `STS1_ANCIENT_WRITING.pages.SIMPLICITY.description` | Open a card-removal selection. |
| `STS1_AUGMENTER.pages.TRANSFORM.description` | Open a two-card transform selection. |
| `STS1_AUGMENTER.pages.MUTATE.description` | Open a card-upgrade selection. |
| `STS1_MOAI_HEAD.pages.WORSHIP.description` | Gain 1 max HP. |
| `STS1_MOAI_HEAD.pages.OFFER.description` | Spend 50 gold and gain 3 max HP. |
| `STS1_FALLING.pages.LET_GO.description` | Open a card-removal selection. |
| `STS1_FALLING.pages.HOLD_ON.description` | Lose 30% max HP as HP damage; A15+ uses 40%. |
| `STS1_FALLING.pages.FLY.description` | Open a card-transform selection. |
| `STS1_KNOWING_SKULL.pages.QUESTION_1.description` | Lose 6 HP; A15+ loses 10 HP. |
| `STS1_KNOWING_SKULL.pages.QUESTION_2.description` | Lose 6 HP; A15+ loses 10 HP. |
| `STS1_KNOWING_SKULL.pages.QUESTION_3.description` | Lose 6 HP, then add a random rare card; A15+ loses 10 HP. |
| `STS1_MIND_BLOOM.pages.WAR.description` | Blocked combat placeholder for random Act 1 boss fight. |
| `STS1_MIND_BLOOM.pages.AWAKE.description` | Upgrade every upgradable card in the deck. |
| `STS1_MIND_BLOOM.pages.RICH.description` | Gain 999 gold and add Normality curses; A15+ adds 3 instead of 2. |
| `STS1_MASKED_BANDITS.pages.PAY.description` | Spend 75 gold. |
| `STS1_MASKED_BANDITS.pages.FIGHT.description` | Blocked combat placeholder for 3 bandits with gold plus relic reward. |
| `STS1_MYSTERIOUS_SPHERE.pages.OPEN.description` | Blocked combat placeholder for 2 Orb Walkers with relic reward. |
| `STS1_SCORPION_NEST.pages.INVESTIGATE.description` | Blocked combat placeholder for 3 Louses with relic reward. |
| `STS1_TREASURE_OOZE.pages.OFFER.description` | Spend 50 gold and gain a random relic. |
| `STS1_TREASURE_OOZE.pages.FIGHT.description` | Blocked combat placeholder for large slime with gold plus relic reward. |
| `STS1_DESIGNER.pages.UPGRADE.description` | Open a card-upgrade selection. |
| `STS1_DESIGNER.pages.REMOVE.description` | Spend 50 gold and open a card-removal selection. |
| `STS1_DESIGNER.pages.TRANSFORM.description` | Open a two-card transform selection. |
| `STS1_FORGOTTEN_ALTAR.pages.PRAY.description` | Gain max HP and add Doubt; A15+ lowers the max HP gain. |
| `STS1_FORGOTTEN_ALTAR.pages.OFFER.description` | Spend 50 gold and gain max HP; A15+ lowers the max HP gain. |
| `STS1_FORGOTTEN_ALTAR.pages.DESECRATE.description` | Gain a random relic and lose 10% max HP. |
| `STS1_TOMB_OF_LORD_RED_MASK.pages.OFFER_50.description` | Spend 50 gold and gain a random relic. |
| `STS1_TOMB_OF_LORD_RED_MASK.pages.OFFER_ALL.description` | Spend all gold and gain a random relic. |
| `STS1_WINDING_HALLS.pages.EMBRACE.description` | Add Debt substitutes and lose max HP; A15+ adds more Debt and loses more max HP. |
| `STS1_WINDING_HALLS.pages.RETREAT.description` | Lose 20% max HP as HP damage; A15+ uses 30%. |
| `STS1_WINDING_HALLS.pages.CONTINUE.description` | Lose 10% max HP; A15+ uses 15%. |

## Patch Order

### Pass A - Enabled-Mode Unblocker

Purpose: remove the one localization blocker that directly affects current CanaryOnly/AdditiveBatch1 UI proof.

Key:

```text
STS1_GOLDEN_IDOL.pages.LEAVE.description
```

Source behavior: `Sts1GoldenIdol.Leave()` ends the event without granting the current random-relic substitute and without applying a trap penalty.

Required changes in that validated resource pass:

1. Add the key to both EN and ZHS localization files.
2. Keep the copy behavior-derived and short; do not copy original StS1 event text.
3. Update `check-sts1-localization-gap-baseline.ps1` from 33 known missing keys to 32, with direct enabled-mode missing keys reduced from 1 to 0.
4. Update `localization-source-gap-scan-20260611.md`, `localization.md`, `status-board.md`, `test-plan.md`, and handoff/release notes to say the direct enabled-mode localization blocker is closed while the remaining 32 keys are still pending.
5. Increment the Spire Plus private-beta package version and align package names, hashes, release docs, and tester handoff docs.
6. After the coordination pause is lifted, run the required build, publish, package, and static checks for a localization/resource change.

Pass A does not by itself prove CanaryOnly or AdditiveBatch1 gameplay, save-load, or render behavior. It only removes the direct source-referenced missing-key blocker for those modes. It also does not replace retained enabled-mode log verifier or runtime evidence packets. O25 and O33 have loader/registration proof only; gameplay, localization render, save-load, image/render, replacement, multiplayer, QA, and handoff rows stay open.

### Pass B - Full Source-Reference Closure

Purpose: close the remaining 32 keys before any source-complete StS1 localization claim or broad RegisterAll/additive gameplay proof.

Remaining source buckets:

| Bucket | Count | Events |
| --- | ---: | --- |
| Simple/later RegisterAll | 6 | Ancient Writing, Augmenter, Moai Head |
| CardService/later RegisterAll | 9 | Falling, Knowing Skull, Mind Bloom |
| Blocked combat | 6 | Masked Bandits, Mysterious Sphere, Scorpion Nest, Treasure Ooze |
| Custom UI/later RegisterAll | 11 | Designer, Forgotten Altar, Tomb of Lord Red Mask, Winding Halls |

Required changes in that validated resource pass:

1. Add all remaining keys to both EN and ZHS localization files, or change source to use existing keys only when the existing key exactly matches the result behavior.
2. Remove or update the 33-key gap baseline guard so it no longer permits missing source-referenced keys.
3. Run:

```powershell
.\scripts\check-sts1-localization-source-keys.ps1 -FailOnMissing
.\scripts\check-sts1-event-static-suite.ps1 -FailOnLocalizationMissing
```

4. Run build/publish/package validation and refresh package/handoff docs because the pass changes shipped localization resources.
5. Keep combat-event runtime proof blocked until encounter models exist; localization source-reference closure is not combat parity proof.

## Runtime Acceptance

After resource validation succeeds, localization still needs runtime evidence before it can be called gameplay-complete:

1. Fresh current-version CanaryOnly enabled-mode smoke with 4 event types / 6 source registration calls if CanaryOnly-specific localization claims are made.
2. Current beta.91 `v0.107.1` AdditiveBatch1 enabled-mode smoke with 10 event types / 14 source registration calls, or a fresher package-matched recapture if the worktree/package changes.
3. EN and ZHS screenshots for options and result pages.
4. Evidence that placeholders render as values, not raw tokens.
5. Clean `godot.log` and `godot-log-audit.json` for the same sessions.
