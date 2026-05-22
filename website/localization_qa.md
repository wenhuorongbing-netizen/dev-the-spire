# Spire Plus Website Localization QA

Date: 2026-05-22

Scope: `website/` player site localization for Simplified Chinese and English.

## Review 1: Rendered Page QA

Result: pass.

Checks:

- Chinese and English update pages render the expected navigation and page title.
- The language switch changes `document.documentElement.lang` and visible page strings.
- Updates, install, forum, and known-issues routes were checked in both languages.
- No relevant console warnings or errors were observed.
- No broken images were reported by the browser pass.

Screenshot evidence:

- `.tools/website-qa/i18n_zh_updates_v2.png`
- `.tools/website-qa/i18n_en_updates_v2.png`
- `.tools/website-qa/i18n_en_install_v1.png`
- `.tools/website-qa/i18n_en_forum_v1.png`
- `.tools/website-qa/i18n_en_issues_v1.png`

## Review 2: Static Localization Coverage QA

Result: pass after one fix.

Fix made during this pass:

- Added the English tag translation `消耗 -> Exhaust` for the Brightest Flame update entry.

Checks:

- 93 Chinese update items and 93 English update items resolve to non-empty title, vanilla text, and current text.
- English item overrides cover 50 explicitly authored entries; the rest resolve through English localization JSON.
- Localization namespaces match between Chinese and English: `ancients`, `ascension`, `cards`, `powers`, `relics`.
- No unresolved title or description keys were found.
- No `undefined`, `null`, `{{ }}`, or `}}` placeholder output was found.
- English display text has no remaining CJK characters in update items, static labels, install text, forum text, known issues, or changelog.

Latest rendered confirmation:

- `.tools/website-qa/i18n_en_updates_v3.png`
- `cardCount = 93`
- `document.documentElement.lang = en`
- `Exhaust` present
- `消耗` absent
- browser console warnings/errors: none

## Validation Commands

```powershell
node --check website\app.js
node --check website\content-data.js
dotnet build
```

Latest build result:

- `dotnet build` passed with 0 warnings and 0 errors.

## Review 3: Vanilla Effect Specificity QA

Result: pass.

Source basis:

- Vanilla relic text checked against `source code/localization/eng/relics.json` and `source code/localization/zhs/relics.json`.
- Vanilla numeric values checked against `source code/src/Core/Models/Relics/*.cs`.
- Brightest Flame checked against `source code/src/Core/Models/Cards/BrightestFlame.cs` and `source code/localization/*/cards.json`.
- Meat Cleaver's Cook details checked against `source code/src/Core/Entities/RestSite/CookRestSiteOption.cs`.

Fix made during this pass:

- Replaced broad vanilla summaries such as `fixed card-play cap`, `multiple cards`, and `several cards` with source-backed numbers and trigger conditions.
- Examples: Velvet Choker is now `+1 Energy / 6-card cap`, Preserved Fog is `remove 3 / add Folly`, Pael's Tooth is `remove 5 / return 1 upgraded after combat`, and Meat Cleaver is `Cook: remove 2 / gain 9 Max HP`.

Post-fix text scan:

- No remaining broad placeholder phrases were found in website data: `fixed`, `several`, `multiple`, `固定`, `若干`, `多张`.

Rendered confirmation:

- `.tools/website-qa/vanilla_specific_zh_v1.png`
- `.tools/website-qa/vanilla_specific_en_v1.png`
- Both languages rendered 93 update cards.
- Checked examples were present: Velvet Choker, Preserved Fog, and Meat Cleaver.
- Browser console warnings/errors: none.

## Review 4: Change-Log Clarity QA

Result: pass.

Fixes made during this pass:

- Removed Pumpkin Candle from the update list because current package restores vanilla behavior and it is not a current player-facing change.
- Rewrote the existing Ancient reward group as direct vanilla/current comparisons instead of broad descriptions.
- Blood-Soaked Rose now states the real card change: Enthralled gains 10 Block when played and still forces itself first.
- Brightest Flame now states the real patch: adds Exhaust and draws 1 more card than vanilla.
- Current entries now keep unchanged relic bodies aligned with vanilla when the actual change is only in a generated card or attached rule.

Rendered confirmation:

- `.tools/website-qa/clear_diffs_zh_v1.png`
- `.tools/website-qa/clear_diffs_en_v1.png`
- Both languages rendered 92 update cards after removing Pumpkin Candle.
- Checked examples were present: Enthralled 10 Block, Brightest Flame Exhaust, Velvet Choker soft cap, Meat Cleaver Butcher.
- Pumpkin Candle was absent in both languages.
- Browser console warnings/errors: none.

Targeted screenshot evidence:

- `.tools/website-qa/target_blood_rose_zh_v1.png`
- `.tools/website-qa/target_brightest_flame_zh_v1.png`

## Review 5: Website Release Fix QA

Result: pass.

Fixes checked during this pass:

- Corrected the creator name to `温火容命` in Chinese and `Wenhuo Rongming` in English.
- Replaced public vanilla placeholders with site-owned simplified SVG icons under `website/assets/vanilla-icons/`.
- Added separate card-change entries for `愚行 / Folly` and `执迷 / Enthralled`.
- Kept `至亮之焰 / Brightest Flame` as the public-facing English title and verified its Exhaust/draw change appears.
- Updated the install page package size and SHA-256 for `SpirePlus-v0.1.0-private-beta.0.zip`.
- Changed the public release page link to the concrete tag URL `v0.1.0-private-beta.0`.

Static checks:

- `node --check website/content-data.js` passed.
- `node --check website/app.js` passed.
- `git diff --check -- website/content-data.js website/app.js website/README.md` passed.
- `dotnet build` passed with 0 warnings and 0 errors.

Rendered confirmation:

- `output/playwright/spire-plus-updates-icons-v1.png`
- `output/playwright/spire-plus-filter-enthralled-v1.png`
- `output/playwright/spire-plus-install-release-v1.png`
- `output/playwright/spire-plus-en-updates-v1.png`

Rendered checks:

- Chinese and English update pages render 106 update cards.
- 28 vanilla-related entries use `assets/vanilla-icons/`.
- `.source-art-placeholder` count is 0.
- Search for `执迷` returns the Blood-Soaked Rose relic row and the Enthralled card row.
- Local install page points the main download button to `../publish/SpirePlus-v0.1.0-private-beta.0.zip` and the release button to the concrete GitHub tag URL.
- Public install page points the main download button to `releases/download/v0.1.0-private-beta.0/SpirePlus-v0.1.0-private-beta.0.zip`; this avoids GitHub `latest` returning 404 for prerelease builds.
- Browser console warnings/errors: none.
