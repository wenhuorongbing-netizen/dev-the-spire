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

- Corrected the creator name to `温火融冰` in Chinese and `Wenhuo Rongbing` in English.
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

## Review 6: Local Redraw Icon QA

Result: superseded by Review 7.

Fixes checked during this pass:

- Updated `AGENTS.md` asset rule: original non-art game assets are still blocked; original art requires documented redistribution permission before entering tracked/public files.
- Replaced repeated vanilla relic/card placeholder usage with 25 relic-specific and 3 card-specific site-owned SVG redraw icons under `website/assets/vanilla-icons/`.
- Confirmed update entries no longer rely on local `source code/` image paths for vanilla relic/card display.

Static checks:

- `node --check website/content-data.js` passed.
- `node --check website/app.js` passed.
- Update-page icon existence scan passed with no missing icon files.

## Review 7: Source Relic Icon QA

Result: superseded by Review 8.

Fixes checked during this pass:

- Removed the relic-specific SVG redraw icons generated for the previous website pass.
- Changed vanilla relic entries to load their local preview icons from `../source code/images/relics/*.png`.
- Kept public-site behavior from shipping original base-game art by default; GitHub Pages falls back through the existing source-art guard.

Static checks:

- `node --check website/content-data.js` passed.
- `node --check website/app.js` passed.
- Source relic icon existence scan passed for all 25 vanilla relic update entries.

## Review 8: Published Vanilla Relic Icon QA

Result: superseded by Review 9.

Permission record:

- Project owner confirmed on 2026-05-23 that the vanilla relic icons may be distributed on the public website.

Fixes checked during this pass:

- Copied only the 25 vanilla relic PNG files referenced by the update page from `source code/images/relics/` to `website/assets/vanilla-icons/relics/`.
- Changed vanilla relic entries to use the shipped public-site PNG paths.
- Confirmed no generated relic SVG files remain in `website/assets/vanilla-icons/relics/`.

Static checks:

- `node --check website/content-data.js` passed.
- `node --check website/app.js` passed.
- Site asset scan found 25 shipped relic PNG files and no relic SVG files.

## Review 9: Published Vanilla Card Portrait QA

Result: superseded by Review 10.

Permission record:

- Project owner confirmed on 2026-05-23 that original game art may be distributed on the public website.

Fixes checked during this pass:

- Copied `brightest_flame.png`, `enthralled.png`, and `folly.png` from `source code/images/packed/card_portraits/` to `website/assets/vanilla-icons/cards/`.
- Changed Brightest Flame, Enthralled, and Folly update entries to use the shipped public-site PNG paths.
- Deleted the generated card SVG files for these entries.

Static checks:

- `node --check website/content-data.js` passed.
- `node --check website/app.js` passed.
- Asset scan found 25 relic PNG files, 3 card portrait PNG files, and no relic/card SVG replacement files.

## Review 10: Source Art Asset Layout QA

Result: pass.

Fixes checked during this pass:

- Moved the approved vanilla art files from `website/assets/vanilla-icons/` into `website/assets/source-art/`.
- Updated the update page to read relic icons from `assets/source-art/relics/`.
- Updated Brightest Flame, Enthralled, and Folly to read portraits from `assets/source-art/card_portraits/`.

Static checks:

- `node --check website/content-data.js` passed.
- `node --check website/app.js` passed.
- Source-art hash scan confirmed the 25 relic PNGs and 3 card portrait PNGs match their `source code/images/` originals.

## Review 11: Supabase Forum Pivot QA

Result: pending live Supabase project.

Fixes checked during this pass:

- Replaced the GitHub feedback draft page with a real forum entry page.
- Replaced the earlier Render/Node deployment plan with GitHub Pages + Supabase.
- `forum/` now builds a static React forum into `website/forum/`.
- Added `forum/supabase/schema.sql` with RLS, column-level grants, insert policies, reply-count trigger, URL-count limit, and client-id frequency checks.

Static checks:

- `npm test` under `forum/` passed the schema guards.
- `npm run build` under `forum/` passed and generated `website/forum/`.
- `node --check website/app.js` passed.
- `node --check website/content-data.js` passed.
- `git diff --check -- forum website .github\workflows\spire-plus-site.yml docs\features\forum .gitignore website\localization_qa.md` passed.

Rendered checks:

- `output/playwright/website-forum-supabase-entry-v3.png`
- `output/playwright/forum-supabase-unconfigured-v4.png`
- `output/playwright/forum-supabase-unconfigured-mobile-v4.png`
- `output/playwright/website-forum-supabase-entry-v4.png`
- `output/playwright/forum-supabase-unconfigured-v5.png`
- `output/playwright/forum-supabase-unconfigured-mobile-v5.png`

Follow-up fixes from rendered QA:

- Disabled the forum post button visually when Supabase is not configured.
- Replaced stale public-facing Node/PostgreSQL deployment copy with Supabase setup copy.
- Added `npm test` to the GitHub Pages forum build workflow.
- Added `docs/features/forum/go-live-checklist.md` and linked it from the forum spec, forum README, website README, and docs index.
- Added optional `npm run test:live` for real Supabase post/reply/read verification after repository Variables are configured.

Live checks still required:

- Create a Supabase project.
- Run `forum/supabase/schema.sql`.
- Configure `SPIRE_PLUS_SUPABASE_URL` and `SPIRE_PLUS_SUPABASE_ANON_KEY` for GitHub Pages builds.
- Run `npm run test:live` with a local `SUPABASE_SERVICE_ROLE_KEY` to verify and clean up an actual anonymous post and reply.
- Verify public anonymous post and reply persistence.

## Review 12: A19/A20 Dedicated Ability Page Sync

Result: static pass, live page render still pending.

Fixes checked during this pass:

- Synced `website/assets/localization/eng/ascension.json` and `website/assets/localization/zhs/ascension.json` from the current mod localization.
- Updated the public Ascension A19/A20 details from the old wording to boss dedicated abilities and Branded Form.
- Simplified the boss-specific page data so each boss row reads the current v4.1 A19/A20 summaries from localization keys.
- Updated package size and SHA-256 in `website/content-data.js`.
- Rewrote `website/README.md` into readable maintenance notes and kept the site framed as public-info, not release-ready evidence.

Static checks:

- `node --check website/content-data.js` passed.
- `node --check website/app.js` passed.
- `dotnet test EZMicroBalance.sln --no-build --filter BossDedicatedAbilityV41GuardTests` passed.

Live checks still required:

- Render the update page in both languages and inspect A19/A20 cards.
- Confirm public GitHub Pages uses the current package metadata after deployment.
