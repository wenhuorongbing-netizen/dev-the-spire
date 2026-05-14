# Ancient Expansion v2.2 Work Log

## 2026-05-14 - Active source-local art promotion

- Promoted the curated manual ChatGPT UI small-art batch into active `EZMicroBalance/` resources for the next art-testing round: 40 direct option/icon/power/fight PNGs plus 12 small/big card portrait PNGs. The optional first-preview Lotha event-background crop remains review-only under `.tools/art-generation/promotion-candidates/proposed/`.
- Updated Urda Seedling, Withered Husk, Morvi Archive Pages, Morvi Red Ink Overdraft, Morvi Waste Paper, and Vakuu Temptation to use unique card portrait paths instead of the shared generic `card.png` / `big/card.png` fallback.
- Updated `export_presets.cfg`, `art-asset-manifest.json`, art direction docs, the integration plan, and guard tests. Manifest source status for the promoted art is `source_local_generated`, not `final_generated`, because this pass used the manual ChatGPT UI fallback rather than audited Image API/GPTimage2 bytes.
- Generated active preview sheets: `.tools/art-generation/chatgpt/ancient-art-active-round1-preview.png` and `.tools/art-generation/chatgpt/ancient-art-active-round1-target-size-preview.png`.
- Validation: `scripts\audit-ancient-art-assets.ps1 -FailOnMissingExport -FailOnInvalidGenerationMode -FailOnHashMismatch` passed with 0 missing targets, 0 hash mismatches, 0 missing exports, 0 invalid generation modes, and 5 remaining generic fallback art records; targeted art/player-facing/release guards passed; `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; `dotnet test EZMicroBalance.sln --no-build` passed with 151 passed and 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF normalization warnings only; `dotnet publish EZMicroBalance.sln --no-restore` passed with the known nested `source code/project.godot` warning; `scripts\package-spire-plus.ps1` refreshed `publish\SpirePlus-v0.1.0-private-beta.0.zip`; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 169 passed and 0 skipped.

## 2026-05-14 - First-preview style anchor and small-art review sheets

- User review corrected the art direction: the first manual ChatGPT preview, `.tools/art-generation/chatgpt/crystal-throne-of-shattered-visions.png` (`Crystal throne of shattered visions`, 1672x941), is now the primary visual anchor. The later source-calibrated mirror iterations are useful only for aspect ratio, Neow-like silhouette, and simplification notes; they should not drive the final look because they overcorrected toward darker, emptier, less appealing compositions.
- Updated `art-generation-prompts.md` and `AncientArtAssetHygieneGuardTests` so future passes preserve the first preview's dark mirror-card finish, readable group silhouette, thick acrylic/marker/gouache paint surface, and small saturated accents while still treating local source-code images as read-only calibration.
- Generated small-option-art review sheets in the existing Edge ChatGPT conversation after uploading only the first manual preview as the style reference. Review artifacts were recovered and cropped under `.tools/art-generation/chatgpt/`: `lotha-option-relics-gothic-fantasy-icon-set.png` plus `lotha-option-relics-contact-128.png`, `morvi-option-relics-arcane-relics-and-mystical-seals.png` plus `morvi-option-relics-contact-128.png`, and `urda-option-relics-mystical-relics-and-glowing-shards.png` plus `urda-option-relics-contact-128.png`.
- Generated the remaining small-art review sheets from the same first-preview style anchor. `remaining-icons-mystic-emblems-of-dark-enchantment.png` was recovered, cropped into Lotha verdict power, Vakuu fight option, and Urda/Morvi/Lotha filled/outline identity-icon review crops, and summarized as `remaining-icons-contact-128.png`. `card-portraits-mystical-artifacts-and-dark-magic.png` was recovered, cropped into Urda Seedling, Withered Husk, Morvi Archive Pages, Morvi Red Ink Overdraft, Morvi Waste Paper, and Vakuu Temptation review crops, and summarized as `card-portraits-contact-160.png`. A combined visual review sheet was also written to `ancient-small-art-review-contact.png`.
- Strict review rejected weak crops instead of carrying them forward. Urda Humus Pact, Trial Branch, Shallow-Root Relic, Root-Sight, Seedling, and Withered Husk were regenerated into `urda-corrections.png` with crops under `urda-corrections/`; identity icons were regenerated into `identity-icons-corrections-stylized-fantasy-emblem-icon-set.png` with crops under `identity-icons-corrections/`; the too-dark Lotha Deferred Verdict and related Lotha outline/icon weak spots were regenerated into `lotha-corrections-dark-fantasy-emblem-collection.png` with crops under `lotha-corrections/`.
- Added target-size review outputs for the curated set. `ancient-small-art-curated-v3-review-contact.png`, `ancient-small-art-curated-v3-target-size-contact.png`, and `ancient-small-art-curated-v3-audit.json` now record the current best review candidates after the correction pass. The v3 audit no longer reports a small-size watch flag; the prior v2 audit had flagged `lotha_deferred_verdict_option_relic` as too dark at target size.
- Added a non-integrated promotion-candidate package and blueprint preview for the next step: `.tools/art-generation/promotion-candidates/promotion-candidates-manifest.json`, `.tools/art-generation/chatgpt/ancient-art-blueprint-preview-v1.png`, and `.tools/art-generation/chatgpt/ancient-art-promotion-target-preview-v1.png`. The candidate manifest currently contains 40 ready candidates, 12 card-portrait items that need code-path changes, and 1 Lotha event-background crop that needs user review before promotion.
- Added `art-testing-integration-plan.md` to classify event backgrounds, option relics, identity icons, power/fight icons, and card portraits; it records the exact integration order, prompt definitions for any follow-up image pass, and the distinction between source/gameplay testing with temporary art versus final-art visual testing after resource promotion.
- Validation for this review-only pass ran targeted `AncientArtAssetHygieneGuardTests` (9 passed), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `scripts\audit-ancient-art-assets.ps1 -FailOnMissingExport -FailOnInvalidGenerationMode`, and `git diff --check` on the touched docs/tests/tooling. Art audit still reports 57 final-art-pending entries because these review PNGs have not been promoted to active resources.
- These sheets are local review artifacts only. They were not copied into `EZMicroBalance/`, no active resource bytes changed, no manifest hash/export/package state changed, and no official source assets were copied.

## 2026-05-14 - Source-code art calibration and ChatGPT preview iteration

- Re-read `PROJECT_STATE.md` before continuing the art pass, then used local source-code images only as read-only visual calibration references. No original Slay the Spire 2 assets were copied, traced, remixed into generated output, or moved into `EZMicroBalance/`.
- Compared `source code/images/events/reflections.png`, `source code/images/events/crystal_sphere.png`, and Ancient placeholders including `darv_placeholder.png`, `vakuu_placeholder.png`, and `orobas_placeholder.png` against the first manual ChatGPT preview. The key mismatches were source aspect ratio and density: source event backgrounds are about 2.13:1, much darker, flatter, sparser, and more negative-space-heavy than the first 16:9 `Crystal throne of shattered visions` preview.
- Updated `art-generation-prompts.md` with a source-code visual calibration section, a read-only source-asset rule, the 2.13:1 final-background target, darker event-background constraints, fewer equally detailed mirror portraits, reduced glossy crystal/key-art language, and the manual ChatGPT fallback rule to omit paths, filenames, and save-directory instructions.
- Updated `AncientArtAssetHygieneGuardTests` so future prompt-pack edits must preserve the source-code visual calibration, final-background aspect-ratio rule, manual ChatGPT path-free fallback rule, and `60-80% quiet dark area` event-background constraint.
- Generated a second manual ChatGPT UI preview in the existing Edge project conversation using the path-free, source-calibrated prompt. The recovered PNG is `.tools/art-generation/chatgpt/crystalline-shrine-of-fractured-souls.png`, titled `Crystalline shrine of fractured souls`, at 1829x860 (2.127:1). It is a review artifact only and was not integrated into active resources.
- Quantitative image checks support the direction change: `reflections.png` is 3440x1616 (2.129:1), average value 0.102, 91.5% dark sample pixels; `crystal_sphere.png` is 3440x1616 (2.129:1), average value 0.065, 91.7% dark sample pixels; the first manual preview was 1672x941 (1.777:1), average value 0.195, 50.8% dark; the second preview is 1829x860 (2.127:1), average value 0.096, 84.6% dark.
- Corrected the manual reference-upload workflow after an initial wrong upload of prior preview/style-background files. The successful reference pass uploaded only the intended Ancient-node shape references, including `ancient_node_neow.png` as the required leftmost mirror whale/tower silhouette source.
- Generated and reviewed two more manual ChatGPT UI iterations from the shape-reference prompt. `Ritual in a shattered mirror hall` was recovered to `.tools/art-generation/chatgpt/ritual-in-a-shattered-mirror-hall.png` at 1832x859 (2.133:1); it fixed the Neow direction but remained too polished. `The mirror hall of forgotten souls` was recovered to `.tools/art-generation/chatgpt/the-mirror-hall-of-forgotten-souls.png` at 1830x859 (2.130:1); it was initially treated as a stronger source-calibrated direction because the left mirror reads as a simplified Neow-like whale/tower silhouette, the palette is limited, the composition is flat/dark, and the acrylic/marker/thick-paint texture is stronger. Later user review superseded this: the first `Crystal throne of shattered visions` preview remains the preferred style anchor, and these later iterations are review artifacts only.
- Latest image stats: `Ritual in a shattered mirror hall` average value 0.104 with 88.0% dark sample pixels; `The mirror hall of forgotten souls` average value 0.115 with 77.2% dark sample pixels and 0.0% bright sample pixels, keeping the event-background darkness target while allowing the left Neow mirror to remain readable.
- Validation ran the targeted `AncientArtAssetHygieneGuardTests` (9 passed), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `scripts\audit-ancient-art-assets.ps1 -FailOnMissingExport -FailOnInvalidGenerationMode`, and `git diff --check` on the touched docs/tests. No publish/package/live game verification was run because no active resource, runtime source, localization, export preset, or package artifact was changed.

## 2026-05-14 - gpt4free image request wrapper hardening and live probe

- Re-read `PROJECT_STATE.md`, `AGENTS.md`, the active test-ready goal, current issue docs, v2.2 docs, art manifest/prompt pack, script index, art audit helper, art guard tests, style guidance, and repo-local StS2/Godot development reference before editing. No archive prompt dump was used as current implementation input.
- Added `scripts/invoke-ancient-art-gpt4free.ps1` as the repository-controlled remote image request helper. It reads `art-generation-prompts.md` and `art-asset-manifest.json`, extracts the target asset concept, and forces `generation_mode`, `mode`, and `semantic_model` to `GPTimage2` in the request payload.
- Updated the prompt pack, art direction, issue index, script index, and guard tests so future gpt4free calls use the canonical Slay-the-Spire-2-style prompt contract instead of older ignored `art_pipeline/prompts/*.md` calibration prompts or generic image defaults.
- Follow-up probe hardening added `prompt_id: lotha_event_background` to the Lotha event-background manifest record, fixed the gpt4free wrapper so single-concept event-background prompt blocks are not rejected as missing concept keys, and made the top-level prompt contract explicitly require `mode: GPTimage2` and `semantic_model: GPTimage2`.
- Live-probed the local g4f service at `http://127.0.0.1:8081`: `/chat/` returned 200, `/v1/models` returned model metadata, and `/v1/chat/completions` with `provider=OpenaiChat` plus `gpt-4o-mini` and `gpt-4o` returned `OK`, confirming the refreshed ChatGPT session works for text. `har_and_cookies/auth_OpenaiChat.json` was not read or printed.
- Source-probed the g4f OpenAPI schema: image routes are `/v1/images/generations`, `/v1/images/generate`, and `/api/{provider}/images/generations`; `ImageGenerationConfig` accepts `prompt`, `model`, `provider`, `response_format`, dimensions, and related fields. `OpenaiChat` provider metadata exposes the image transport model as `gpt-image`, not as a literal `GPTimage2` model id, so the wrapper now maps transport `model` to `gpt-image` by default while keeping `GPTimage2` as the audited generation mode.
- Tried real image calls without overwriting resources: `lotha_mirror_rebuttal_option_relic` reached g4f but returned upstream `HTTP Error 401` through `/v1/images/generations?provider=OpenaiChat` with `model=GPTimage2`, the same route with `model=gpt-image`, and `/api/OpenaiChat/images/generations` with `model=gpt-image`; chat-completion prompts that requested image generation also returned 401. The follow-up `lotha_event_background` request assembled the mirror-event prompt with the core Slay-the-Spire-2-style suffix and `GPTimage2` mode fields, but `/v1/images/generations` still returned 401 for `OpenaiChat/gpt-image`, `OpenaiAccount/gpt-image`, and `OpenaiChat/gpt-image-1.5`. Plain text g4f prompts continued to work. Conclusion: the current local g4f session is valid for text, but OpenAI-backed image generation remains blocked by image auth/entitlement/model routing.
- Rechecked the updated local gpt4free fork after the refreshed ChatGPT token: local `upstream/main` matched remote HEAD `8ba8697e`, the active fork HEAD was `24ef6bb2`, and an explicit `.venv` API instance on `127.0.0.1:8082` reproduced the same result as the existing `8081` service. `OpenaiChat/gpt-4o-mini` text returned `OK`; `/v1/images/generate` with `model=flux` succeeded through `PollinationsImage`, proving the generic image API works; `/v1/images/generate` and `/v1/media/generate` with `provider=OpenaiChat` plus `model=gpt-image` still failed. Sanitized server logs place the failure in `OpenaiChat.wss_media` at `https://chatgpt.com/backend-api/celsius/ws/user` with `HTTP Error 401`. Conclusion: the remaining blocker is the ChatGPT web image/celsius auth or entitlement path, not the Spire Plus prompt pack, endpoint spelling, or wrapper payload shape.
- Manual ChatGPT UI fallback in the user's existing Edge profile was also tested. Temporary Chat explicitly refused image generation, so a regular `ez的日常` project conversation was used with a prompt that omitted target paths and filenames. ChatGPT produced an actual image titled `Crystal throne of shattered visions`; the original PNG was recovered from Edge cache to `.tools/art-generation/chatgpt/crystal-throne-of-shattered-visions.png` at 1672x941 for local review only. It was not copied into `EZMicroBalance/`, no manifest hash was updated, and this does not close final-art integration or Image API/gpt4free automation blockers.
- Dry-run/request JSON files were written under `.tools/art-generation/gpt4free/`, including `lotha_event_background.request.json` with `generation_mode`, `mode`, and `semantic_model` all set to `GPTimage2`, `model=gpt-image`, and the canonical mirror-event prompt. No remote image bytes, resource files, manifest hashes, publish artifacts, live game, save-load, death/failure-path, A11 traversal, Rootblight visual proof, or co-op verification changed in this pass.
- Validation ran `scripts\audit-ancient-art-assets.ps1 -FailOnMissingExport -FailOnInvalidGenerationMode`, `dotnet build EZMicroBalance.sln --no-restore`, `dotnet test EZMicroBalance.sln --no-build`, `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, and `git diff --check`. Art audit reported 0 missing targets, 0 hash mismatches, 0 missing exports, 0 invalid generation modes, and preserved 57 final-art-pending records; build passed with 0 warnings/errors; tests passed with 151 passed, 18 skipped, 0 failed; format passed; diff-check passed with CRLF normalization warnings only.

## 2026-05-14 - Player text scrub and Ancient art fit guard

- Re-read `PROJECT_STATE.md`, `AGENTS.md`, the active test-ready goal, current issue docs, v2.2 docs, art manifest, style guidance, and repo-local StS2/Godot development reference before editing. No archived prompt dump was used as current implementation input.
- Scrubbed active English and Simplified Chinese player text for the current feedback list: Trial Branch, Rooted Route, Seed Bank, A12-A20 Ascension descriptions, Firemarked Elite terminology, Blight Sprout timing, Banner Room rewards, Royal Seal/King Brand, Holy Daze, Struggle Bait, Residual Sample, Morvi Open-Book wording, and key Lotha option/relic hovers.
- Updated source fallback strings and power names that can surface to players, including Firemarked enemy terminology and concrete Holy Daze/Struggle Bait/Residual Sample summaries.
- Patched Urda, Morvi, and Lotha clicked Ancient background scenes to use cover-style TextureRect fitting so the event illustration fills the UI frame more predictably while preserving the separate event-background resource path.
- Hardened guard coverage so active player-facing text rejects stale implementation terms, Ancient background scenes use event art with cover fit, option relic art does not route through event/map/run-history paths, and manifest dimensions match current PNG bytes. `scripts\audit-ancient-art-assets.ps1 -FailOnMissingExport` still reports 0 missing targets, 0 hash mismatches, and 0 missing exports while preserving 57 final-art-pending records.
- Validation ran JSON parse checks, `dotnet build EZMicroBalance.sln`, `dotnet test EZMicroBalance.sln --no-build`, `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `dotnet publish EZMicroBalance.sln --no-restore`, `scripts\package-spire-plus.ps1`, a post-hash rebuild/test, and opt-in artifact tests after the source-audited text correction pass. Build passed with 0 warnings/errors; normal tests passed with 150 passed, 18 skipped, 0 failed; format passed; diff-check passed with CRLF normalization warnings only; publish/package passed with the known nested `source code/project.godot` warning; opt-in artifact tests passed with 168 passed, 0 skipped, 0 failed. Current hashes are zip `FEEE3DF6148B9F4023F97CAF50A871C9F17F182595B06FF52167C0DF70BB5D7E`, DLL `AF46CBAF27B4344ED98A4DE213B06096F1B4E8FD7802556F62A3CB371A3A5ED8`, PCK `19891AB898EF8FCF7084F0244AE067C917AC0EC51737C9D2884C697D9F6D5D57`, manifest `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`, and README `5B1194440F6B212471E05F0EE117EE7F30E597FAAA916DF91F9378CD529DDCBB`.
- No final bespoke Image API art was generated or integrated in this pass; live clicked Ancient UI, gameplay, save-load, death/failure-path, A11 traversal, Rootblight visual proof, and co-op verification remain pending.

## 2026-05-14 - Art prompt contract cleanup

- Re-read `PROJECT_STATE.md`, `AGENTS.md`, the active test-ready goal, current issue docs, v2.2 README/art docs, art manifest, audit helper, and art guard tests before editing. No archive prompt dump was used as current implementation input.
- Made `docs/features/ancient-expansion-v2.2/art-generation-prompts.md` the single operational prompt pack for future final Ancient art. It now explicitly requires `generation_mode: GPTimage2` and `model: GPTimage2`, rejects generic imagegen/default fallback, and records the user-supplied Slay-the-Spire-2-style dark hand-painted prompt core.
- Simplified `art-direction.md` by removing the duplicated hash table and old prompt TODO dump; `art-asset-manifest.json` remains the source of truth for paths, dimensions, hashes, and temporary/final status.
- Shortened `docs/test-ready-development-goal.md` so it points to the prompt pack instead of duplicating full prompt templates.
- Hardened `scripts/audit-ancient-art-assets.ps1` to report final-generated manifest entries that do not record `GPTimage2`; no image bytes were generated or replaced in this pass.

## 2026-05-14 - Ancient player-facing polish and art-readiness

- Re-read the active goal, `PROJECT_STATE.md`, current v2.2 docs, art manifest/prompt pack, manual checklist, style guidance, and repo-local StS2/Godot development reference before editing. No archive prompt dumps were used as implementation input.
- Fixed concrete player-facing text issues: legacy Urda option relic localization no longer says it is an option art marker or unobtainable; it now describes the actual Seedbed, Humus Pact, Molting, and Moss Map behavior in English and Simplified Chinese.
- Tightened readable rich-text highlights without changing mechanics: Urda `Compost Reward`/`Store Seed`, Morvi `Borrowed`, `Archive Pages`, `Waste Paper`, `red-ink debt`, and Lotha `Rebuttal Card` prompts are highlighted in the active English/zhs option, relic, or power text. Missing numeric blue markup in affected zhs prompts and Red Ink debt text was repaired.
- Added guard coverage for the new polish: active Ancient localization rejects development wording, legacy/canonical option relic descriptions reject marker/unobtainable implementation text, and focused concept-rich-text checks cover the touched Urda/Morvi/Lotha strings.
- Rechecked art routing and ran `scripts\audit-ancient-art-assets.ps1 -FailOnMissingExport`: no missing targets, hash mismatches, or missing export coverage were found. No generated or bespoke final art was integrated; all temporary/final-art-pending records remain pending.
- Validation ran `git status --short --branch`, `git log -1 --oneline --decorate`, `scripts\audit-ancient-art-assets.ps1 -FailOnMissingExport`, `dotnet build EZMicroBalance.sln --no-restore`, `dotnet test EZMicroBalance.sln --no-build`, `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `dotnet publish EZMicroBalance.sln --no-restore`, `scripts\package-spire-plus.ps1`, post-package `dotnet build EZMicroBalance.sln --no-restore`, and opt-in release artifact tests. Normal tests passed with 149 passed, 18 skipped, 0 failed; the first opt-in artifact run exposed stale hash docs, and the rebuilt rerun passed with 167 passed, 0 skipped, 0 failed. Current package hashes are zip `B97FF7B84AFAD394705004F35B21FF7A0A5271DF76C277BC3780FC793A422E8F`, DLL `DF83EA7A7D0DAACAF2DC33416FEC63A04436D9A36070561DEB00F0C40DA1AF21`, PCK `2937930EBA8CCED6577D44F87698D49CB1DB744249666096EE194A152FD1B60C`, manifest `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`, and README `5B1194440F6B212471E05F0EE117EE7F30E597FAAA916DF91F9378CD529DDCBB`. No live game, clicked Ancient UI, gameplay, save-load, death/failure-path, co-op testing, or Image API art generation was run.

## 2026-05-14 - Ancient UI/art resource-routing hardening

- Re-read the active Ancient UI/art hardening goal, current project state, active v2.2 docs, art manifest/prompt pack, style guidance, and repo-local StS2/Godot development reference before editing. No archive prompt dumps were used as implementation input.
- Reviewed current Urda, Morvi, Lotha, and Vakuu UI/art source plus local Core `NAncientEventLayout`, `EventModel`, `EventOption`, `RelicModel`, `ImageHelper`, and run-history map icon source. No concrete resource-routing bug was found: Urda, Morvi, and Lotha clicked Ancient backgrounds still route through `CustomScenePath` to Control-root `.tscn` scenes that reference `EZMicroBalance/images/events/*.png`, while map/run-history icons and option marker relic art remain separate `EZMicroBalance/images/ancients/**` resources. Vakuu fight option art continues through `EventOption.FromRelic(...)`.
- Hardened static guard coverage: `AncientUiReadinessGuardTests` now checks active Ancient art roles stay separated and option marker relics do not fall back to the generic shared relic icon; `AncientArtAssetHygieneGuardTests` now checks every manifest resource target that should be exported is present in `export_presets.cfg`.
- Hardened `scripts/audit-ancient-art-assets.ps1` so it reports missing export coverage from the manifest and supports `-FailOnMissingExport`. Current audit output reports 0 missing targets, 0 hash mismatches, and 0 missing exports, while preserving the documented 9 temporary duplicate groups and final-art-pending records.
- Updated current docs to keep the status honest: this was resource-guard/docs hardening only, no final bespoke art was generated because `OPENAI_API_KEY` is not set, package hashes were not refreshed, and live clicked UI/gameplay/save-load/death/failure-path/co-op verification remains pending.
- Validation ran `git status --short --branch`, `git log -1 --oneline --decorate`, `scripts/audit-ancient-art-assets.ps1`, `scripts/audit-ancient-art-assets.ps1 -FailOnMissingExport`, `dotnet build EZMicroBalance.sln --no-restore`, `dotnet test EZMicroBalance.sln --no-build`, `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, and `git diff --check`. Build passed with 0 warnings/errors, normal tests passed with 144 passed and 18 skipped, format passed, and diff-check passed with CRLF normalization warnings only. No `dotnet publish`, package refresh, or opt-in release-artifact tests were required because runtime source, resources, localization, export presets, and package inputs did not change.

## 2026-05-14 - Morvi reward/state lifecycle hardening

- Re-read the active v2.2 docs, Morvi source/localization/tests, `AncientCardHelpers`, `AncientPlayerState`, and local Core reward/card-pile/combat/damage source before editing. BaseLib `SavedSpireField` docs were used only for the existing save/load risk stance; live save/load remains pending.
- Fixed the shared generated-card helper so it guards combat-not-in-progress and missing owner combat state before calling Core, calls `AddGeneratedCardsToCombat([card], ...)` directly instead of Core's single-card wrapper that indexes `[0]`, and removes generated cards on empty/null/unsuccessful add results.
- Hardened Red Ink Overdraft against Core hand-full redirection by skipping generation when the hand is full, verifying the generated card actually lands in the hand, and removing/logging wrong-pile results. Red Ink unpaid debt now uses the same nonlethal HP fallback as Debt Settlement.
- Shared Morvi combat-end HP fallback through `DamagePlayerNonlethal(...)`, keeping Debt Settlement and Red Ink from reducing the player below 1 HP during `AfterCombatEnd`.
- Updated English and Simplified Chinese Red Ink option/relic/power text to mention the hand-space condition and nonlethal HP fallback, and added source/localization guards for the helper and Red Ink lifecycle paths.
- Validation ran `git status --short --branch`, `dotnet build EZMicroBalance.sln --no-restore`, `dotnet test EZMicroBalance.sln --no-build`, `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `dotnet publish EZMicroBalance.sln --no-restore`, `scripts/package-spire-plus.ps1`, a rebuild for updated release guard constants, and opt-in release artifact tests. Normal tests passed with 142 passed, 18 skipped, 0 failed; opt-in artifact tests passed with 160 passed, 0 skipped, 0 failed after one stale compiled-guard run; package hashes are zip `A147B2850C011DDF04D1D12F6817DFC89BDE58193192B524D5B2385986706C72`, DLL `EAFBAB44B8AB70C1DC81CC878B1ED1E9C270E799AA2637EEABA16F76E3CBC911`, PCK `F279CD94C6BFB0D92B675E5546D937A08C1A121D7B8284549FAD1FD527272377`, manifest `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`, and README `5B1194440F6B212471E05F0EE117EE7F30E597FAAA916DF91F9378CD529DDCBB`. No live game, clicked Ancient UI, gameplay, save-load, death/failure-path, co-op testing, or Image API art generation was run.

## 2026-05-14 - Ancient state mirror source guard coverage

- Audited `AncientPlayerState`, `AncientSavedStateFields`, and the active Urda, Morvi, and Lotha run hooks against the documented `SavedSpireField<Player,string>` risk. The helper already read runtime state first, restored runtime state from owned/non-removed deck-card mirrors when runtime was empty, wrote runtime plus deck mirrors on set, and used the same recovery path through `SyncDeck`.
- Verified active encoded state reads and writes funnel through each hook's `GetSelectedBlessing(...)`, `GetProgress(...)`, `SetProgress(...)`/`SetState(...)`, and `SyncPersistentState(...)` paths, with recurrent `AfterCardChangedPiles(...)` deck mirror sync calls. No direct source indexing of `UrdaStateKey`, `UrdaDeckStateKey`, `MorviStateKey`, `MorviDeckStateKey`, `LothaStateKey`, or `LothaDeckStateKey` was found outside the helper.
- Added focused guard coverage for the helper contract, owner/removed-card deck filters, run-hook helper usage, recurrent sync hooks, direct encoded-field bypass rejection, and docs keeping live save/load pending.
- Updated current risk/API/manual/issue docs to record stronger source guard coverage without closing live save/load rows.
- Validation before the final status-doc update: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors, and `dotnet test EZMicroBalance.sln --no-build` passed with 142 passed, 18 skipped, 0 failed. Final format/diff-check results are recorded in `PROJECT_STATE.md`.
- Runtime source, resources, localization, export presets, and package inputs were not changed in this pass. No publish/package refresh, live game, clicked UI, gameplay, save-load, death/failure-path, co-op testing, or Image API art generation was run.

## 2026-05-14 - Lotha Public Evidence debuff policy hardening

- Re-read the active v2.2 docs, Lotha source/localization, and local Core power source for `PowerType.Debuff`, `PowerModel.GetTypeForAmount(...)`, `PoisonPower`, `WeakPower`, `VulnerablePower`, `FrailPower`, and source-proven damage/kill Debuffs.
- Tightened `lotha_public_evidence` so the Lotha-only power hooks still use Core Debuff classification as the base gate, but exclude source-proven damage-like Debuffs: Poison, Constrict, Demise, Disintegration, Doom, Magic Bomb, Strangle, and The Gambit. Weak, Vulnerable, Frail, and other non-damaging negative statuses remain eligible under the source gate.
- Updated English and Simplified Chinese Public Evidence option/relic text to say non-damaging negative statuses and explicitly exclude Poison, damage-over-time, and countdown damage. Enlightenment power text remains unchanged because it only describes turn-start consumption.
- Added source guards for the classifier helper, Poison exclusion, Weak/Vulnerable/Frail inclusion evidence, strict player-to-enemy/enemy-to-player ownership gates, localization not claiming Poison is doubled, and risk-register/source-design/manual-checklist status.
- Validation ran `git status --short --branch`, `dotnet build EZMicroBalance.sln --no-restore`, `dotnet test EZMicroBalance.sln --no-build`, `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` after one mechanical format fix, `git diff --check`, `dotnet publish EZMicroBalance.sln --no-restore`, `scripts/package-spire-plus.ps1`, and opt-in release artifact tests. The first opt-in artifact run exposed stale hash docs; after updating current docs/tests, the rerun passed with 155 passed, 0 skipped, 0 failed. Current package hashes are zip `A147B2850C011DDF04D1D12F6817DFC89BDE58193192B524D5B2385986706C72`, DLL `EAFBAB44B8AB70C1DC81CC878B1ED1E9C270E799AA2637EEABA16F76E3CBC911`, PCK `F279CD94C6BFB0D92B675E5546D937A08C1A121D7B8284549FAD1FD527272377`, manifest `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`, and README `5B1194440F6B212471E05F0EE117EE7F30E597FAAA916DF91F9378CD529DDCBB`. No live game, save-load, clicked UI, death/failure-path, co-op testing, or Image API art generation was run.

## 2026-05-14 - Save-risk source hardening and status consistency

- Audited active docs for the stale package hashes called out by the goal. The only stale current-status claim found outside archives and the active goal file was the top note in `docs/issues.md`; current package hashes now point to zip `A147B2850C011DDF04D1D12F6817DFC89BDE58193192B524D5B2385986706C72`, DLL `EAFBAB44B8AB70C1DC81CC878B1ED1E9C270E799AA2637EEABA16F76E3CBC911`, PCK `F279CD94C6BFB0D92B675E5546D937A08C1A121D7B8284549FAD1FD527272377`, manifest `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`, and README `5B1194440F6B212471E05F0EE117EE7F30E597FAAA916DF91F9378CD529DDCBB`.
- Reviewed local Core save/death/event-combat source again. The source stance remains unchanged: `CombatRoom.ToSerializable()` rejects unfinished parent-linked combat rooms, prefinished parent restore is source-shaped through `RunManager`, `CreatureCmd.Kill(force: true)` bypasses death prevention, and player serialization still does not source-prove `SavedSpireField<Player,string>` persistence. Deck-card saved fields remain the source-safe mirror carrier; live save/load rows stay open.
- Added source-visible Lotha Death Reprieve restore logging that reports the restored pending/active phase, whether the protection power was already present, and that active-turn save/load continuation remains live-pending. Added a Vakuu victory fallback log for the ownerless restored path. No enemy-turn interruption rewrite, new blessing, or state-carrier migration was attempted.
- Strengthened guards for Lotha phase writes before current-turn/pending reprieve handling, duplicate reprieve start prevention, Vakuu's no-normal-reward/no-`LinkedRewardSet` surface, ownerless fallback logging, and current package hash docs.
- Validation: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; the first `dotnet test EZMicroBalance.sln --no-build` exposed one brittle new guard, then the rerun passed with 136 passed, 18 skipped, 0 failed; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` first exposed one whitespace issue, then passed after formatting the touched test file; `git diff --check` passed with CRLF normalization warnings only.
- External Codex did not run `dotnet publish` or package refresh, but parent audit refreshed the installed/staging/versioned/zip artifacts after reviewing the runtime C# changes so manual testing uses the new DLL. This package has since been superseded by the Lotha Public Evidence debuff-policy package refresh. No live game, clicked Ancient UI, gameplay, save-load, death/failure-path, co-op testing, or Image API art generation was run.

## 2026-05-14 - Lotha Single Sentence residual-risk source-guard closure

- Reviewed `lotha_single_sentence` in `LothaRunHook.cs` against v2.2 and local Core card-play evidence (`CardModel.OnPlayWrapper`, `CardPlay`, and `CardCmd.AutoPlay`). Runtime source already matched the intended behavior: each player turn resets the ruling state; the first player-driven Attack/Skill gets two additional executions; extra executions/autoplay/clones do not recursively consume the ruling or four-card cap; the ruling card itself does not count against the remaining four normal player-played cards; and the first Power fallback is available only before the Attack/Skill ruling.
- Verified the post-ruling Power branch is closed: `TryResolveSingleSentencePowerFallback` and `CanUseSingleSentencePowerReplacement` both require `!SingleSentenceUsedThisTurn`, exclude non-Power cards, exclude clones through `IsPowerCard`, and only draw 1 with no Energy refund. Status, Curse, and other non-eligible cards do not trigger the ruling because `IsEligibleCard` is Attack/Skill-only.
- Added branch-specific source guards in `LothaPolishGuardTests` for `TryResolveSingleSentencePowerFallback`, `CanUseSingleSentencePowerReplacement`, `TrackSingleSentenceRemainingPlays`, `ShouldPlay`, Attack/Skill vs Power eligibility, EN/zhs Single Sentence text, and stale old one-card/Block/Strength/Energy-refund wording. Mirror Rebuttal's full-hand fallback guard remains present and unchanged.
- Validation: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; `dotnet test EZMicroBalance.sln --no-build` passed with 133 passed, 18 skipped, 0 failed; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF normalization warnings only.
- No runtime C# source, resources, localization, publish/package artifacts, or hashes changed in this follow-up. No live game, clicked Ancient UI, gameplay, save-load, death-path, co-op testing, or Image API art generation was run.

## 2026-05-14 - Lotha Mirror Rebuttal full-hand guard

- Fixed the remaining source-review risk in `TryMoveMirrorRebuttalCardToHand`: local Core `CardPileCmd.Add(..., PileType.Hand)` redirects full-hand adds into discard, so the old implementation could silently move the selected Rebuttal card away from the promised hand location while logging success.
- If the player's hand is already full when Mirror Rebuttal tries to pull the selected card, the card is now placed on top of the draw pile instead and the log states that the hand was full. Successful hand moves now verify the resulting pile before logging success.
- Added a source guard so this full-hand fallback remains visible.
- No live game, clicked Ancient UI, save-load, death-path, or co-op testing was run for this narrow source fix.

## 2026-05-14 - Next test-ready player-facing polish and package refresh

- Checked `OPENAI_API_KEY` without printing it; it was not set, so no final bespoke Image API art was generated. The art manifest now also tracks exported Rootblight/Blight Sprout portraits and generic power/relic fallback art.
- Added option hover previews for named generated/temporary cards where the event option UI supports hovers: Urda Seedling/Withered Husk, Morvi Overdraft/Archive Pages/Waste Paper, and Vakuu Temptation.
- Tightened English and Simplified Chinese text for Urda Seed Bank, Rootblight notices, Forge Token, Vakuu Temptation timing, and Lotha Power replacement cost wording. Seed Bank text no longer claims the second selected Seed becomes Trial Plant; Vakuu text now says Temptation is placed after the hand draw on turns 1/3/5+; Lotha Power text says cost 0 for the play rather than only 0 Energy because source also zeroes Star cost.
- Refreshed current package hashes after `dotnet publish` and `scripts/package-spire-plus.ps1`: zip `A147B2850C011DDF04D1D12F6817DFC89BDE58193192B524D5B2385986706C72`, DLL `EAFBAB44B8AB70C1DC81CC878B1ED1E9C270E799AA2637EEABA16F76E3CBC911`, PCK `F279CD94C6BFB0D92B675E5546D937A08C1A121D7B8284549FAD1FD527272377`, manifest `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`, README `5B1194440F6B212471E05F0EE117EE7F30E597FAAA916DF91F9378CD529DDCBB`.
- Validation: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; `dotnet test EZMicroBalance.sln --no-build` passed with 132 passed, 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `dotnet publish EZMicroBalance.sln --no-restore` passed with the known nested `source code/project.godot` warning; post-publish normal tests passed with 132 passed, 18 skipped; `scripts/package-spire-plus.ps1` passed; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 150 passed, 0 skipped; final normal tests passed with 132 passed, 18 skipped; `scripts/audit-ancient-art-assets.ps1` reported 68 manifest entries, 0 missing targets, 0 hash mismatches, 9 duplicate groups, and 57 final-art-pending entries; `git diff --check` passed with CRLF normalization warnings only.
- No live game launch, clicked Ancient UI, gameplay, save-load, failure/death-path, or co-op testing was run.

## 2026-05-14 - Lotha Power replacement eligibility bugfix

- Fixed the Lotha Power replacement follow-up from the polish review without touching art/resources/package output.
- Removed first-Power-in-hand gating for Mirror Hall Echo, Deferred Verdict, and Single Sentence. Their cost-zero preview now follows blessing state, and after-play resolution rechecks the actual player-played Power card instead of depending on `PowerReplacementCardPendingBenefit`.
- Kept Mirror Rebuttal tied to the marked deck card only. Power cards still use replacement draw/Energy benefits and are not extra-played; autoplay, clones, and non-first executions remain excluded.
- Added source guards against reintroducing `FirstOrDefault(IsPowerCard)`, `IsCurrentEligiblePowerInHand`, or after-play dependence on the pending preview marker.
- Validation: exact `dotnet build EZMicroBalance.sln --no-restore` and exact `dotnet test EZMicroBalance.sln --no-build` hit the known local `Godot.NET.Sdk/4.5.1` resolver issue without `NUGET_PACKAGES`; reruns with `NUGET_PACKAGES=C:\Users\Jack\.nuget\packages` passed with 0 build warnings/errors and 126 passed, 18 skipped. `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed. `git diff --check` passed with CRLF normalization warnings only.
- No publish, package refresh, live game launch, clicked Ancient UI, save-load, death-path, or co-op testing was run.

## 2026-05-14 - Lotha Power-card replacement polish

- Tightened active Lotha Power replacement semantics to the v2.2 source design. Power cards are no longer extra-played: Mirror Rebuttal makes the marked Power cost 0 for that play, then grants 2 Energy and draws 2; Mirror Hall Echo, Deferred Verdict, and Single Sentence make the current eligible Power cost 0 and draw 1 with no Energy gain.
- Added cost-zero coverage through `TryModifyEnergyCostInCombat(...)` and `TryModifyStarCost(...)` so eligible Power replacement paths can preview and pay as 0-cost cards, while Attack/Skill extra-play paths still use `ModifyCardPlayCount(...)`.
- Updated English and Simplified Chinese Lotha option text, option relic hover text, and Verdict power text so Ancient options and relic hovers stay paired and no longer describe the old generic Power Energy refund.
- Re-ran the Ancient art manifest audit. It reported 0 missing targets, 0 hash mismatches, and 9 duplicate temporary/source-derived groups. No art was generated or replaced: `OPENAI_API_KEY` was unset and no repo-local `image_gen.py` helper was found, so final bespoke option relic/power/card art remains pending.
- Rebuilt the package from a workspace-local publish redirect because sandbox permissions blocked writing to the real Steam mods folder. That historical package was superseded by the later next test-ready implementation package refresh.
- Validation: the exact `dotnet build EZMicroBalance.sln --no-restore` first failed because `Godot.NET.Sdk/4.5.1` was not visible without the local NuGet cache; rerun with `NUGET_PACKAGES=C:\Users\Jack\.nuget\packages` passed. `dotnet test EZMicroBalance.sln --no-build` first exposed one over-broad source guard, then rerun passed with 125 passed, 18 skipped, 0 failed. Exact real Steam-folder publish failed on sandbox write permissions; redirected workspace-local publish passed. `scripts/package-spire-plus.ps1 -GameRoot .tools\publish-redirect2\game-root` rebuilt the package. `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` failed because the opt-in installed-artifact tests require the real Steam mod folder and live log directory. `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` and `git diff --check` passed; diff-check printed CRLF normalization warnings only.

## 2026-05-14 - Ancient art asset-hygiene manifest pass

- Checked `OPENAI_API_KEY` without printing it; it was not set. A local imagegen helper path exists, but no final art was generated or integrated because credentials were unavailable.
- Added `art-asset-manifest.json` to list active Urda/Morvi/Lotha/Vakuu event backgrounds, map/run-history icons, option/relic art, `lotha_verdict` power art, and generic temporary Ancient card/status portrait usage with SHA256, dimensions, source status, prompt ids, and final-required flags.
- Added `art-generation-prompts.md` as the operational final-art prompt pack and `scripts/audit-ancient-art-assets.ps1` as a safe default-informational manifest auditor for missing files, hash drift, duplicate bytes, and temporary/missing records.
- Added guard coverage so active docs do not call temporary art final, duplicate temporary groups remain documented, generic card portrait use remains explicitly temporary, and the audit helper remains non-destructive by default.
- No publish/package refresh or live game validation was run because this pass did not integrate new image bytes or change export/package inputs.

## 2026-05-14 - Vakuu/Lotha save-risk reduction pass

- Re-read current v2.2 docs, `PROJECT_STATE.md`, `AGENTS.md`, `docs/test-ready-development-goal.md`, and `docs/skills/sts2-godot-mod-development.md`, then inspected local Core source and current Vakuu/Lotha/Common implementation files for the active save/load blockers.
- Recorded exact Core evidence in `api-research.md`: `CombatRoom.ToSerializable()` rejects unfinished parent-linked combat rooms, `RunManager.EnterRoomWithoutExitingCurrentRoom(...)` and `ProceedFromTerminalRewardsScreen()` support parent resume after combat, and run saves carry only an optional `PreFinishedRoom`.
- Changed Vakuu's active fight room shape so `StartFight(...)` no longer assigns `ParentEventId` while the combat is unfinished. A narrow `CombatRoom.ToSerializable()` postfix records the Vakuu parent only after the Vakuu trial combat is prefinished, preserving the source-shaped parent restore path without keeping the old Core-rejected active shape.
- Changed Lotha Death Reprieve progress from a once-per-run boolean to a deck-mirrored phase state: `None`, `PendingStart`, `Active`, and `Resolved`. The hook now rehydrates pending/active protection state from encoded progress and marks the reprieve resolved at turn end, combat end, and before the forced failure death path.
- Updated `api-research.md`, `risk-register.md`, `manual-test-checklist.md`, and `docs/issues.md` to keep live/gameplay/save-load/co-op blockers open while recording the narrowed risk: Vakuu active combat avoids the known unfinished parent-linked serialization blocker; Lotha reduces duplicate-trigger/lost-protection risk, but exact active-turn restore still needs live proof.
- Added source guards for the Core parent-linked serialization rule, the new Vakuu prefinished parent-recording patch, Lotha phase-backed persistence, forced-death bypass, and active docs not making false save-safe claims.
- Validation:
  - `git status --short --branch` before edits: `main...origin/main` with an already dirty worktree.
  - `dotnet build EZMicroBalance.sln --no-restore`: passed with 0 warnings and 0 errors.
  - `dotnet test EZMicroBalance.sln --no-build`: passed with 119 passed, 18 skipped, 0 failed.
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
  - `git diff --check`: passed with CRLF normalization warnings only.
  - Follow-up package refresh was run because the pass changed the compiled DLL: `dotnet publish EZMicroBalance.sln --no-restore` passed, `scripts/package-spire-plus.ps1` rebuilt `publish/SpirePlus-v0.1.0-private-beta.0.zip`, the first opt-in artifact test run exposed stale documented hashes, and after updating active release/status docs the rerun passed with 137 passed, 0 skipped, 0 failed.
  - Superseded package zip SHA256 from this historical pass is replaced by the later next test-ready implementation package refresh.
- No live game launch, live save/load run, death-path run, or co-op validation was performed in this pass.

## 2026-05-14 - Clicked Ancient UI preparation and black-screen hardening

- Searched scripts, tests, docs, and source for an automated clicked-Ancient UI path. Existing repo helpers can prepare/restore live sessions, isolate BaseLib plus Spire Plus, preserve current-run files, preflight foreground windows, and audit copied logs, but no safe script currently opens/clicks an Ancient screen and captures a screenshot.
- Documented a manual force-evidence protocol in `manual-test-checklist.md` using `.tools/runtime-evidence/ancient-ui-click-smoke-YYYYMMDD-HHMMSS`, `SPIREPLUS_FORCE_ANCIENT`, `SPIREPLUS_FORCE_VAKUU_FIGHT`, `scripts/spire-plus-live-session.ps1`, `scripts/check-spire-window-preflight.ps1`, and `scripts/audit-godot-log.ps1`.
- Hardened Urda/Morvi/Lotha option generation fallback logging for invalid forced blessing ids, empty source-backed option pools, and undersized option pools. Urda now presents four initial options; Morvi and Lotha remain at three.
- Tightened source/UI readiness guard coverage for Control-root Ancient scenes, separation of event art from map/run-history icons, option marker relic art/localization, force gates, fallback source shape, and active-doc false clicked-UI claims.
- Updated Urda English/zhs initial text from three to four living bargains and polished the English Vakuu fight marker relic sentence. No new art was generated and no live click/open evidence was collected in this pass.
- Validation:
  - `dotnet build EZMicroBalance.sln --no-restore`: passed with 0 warnings and 0 errors.
  - `dotnet test EZMicroBalance.sln --no-build`: passed with 114 passed, 18 skipped, 0 failed.
  - `dotnet publish EZMicroBalance.sln --no-restore`: passed; Godot emitted the known nested `source code/project.godot` warning and regenerated ignored test `.uid` metadata.
  - Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed with 114 passed, 18 skipped, 0 failed.
  - `scripts/package-spire-plus.ps1`: rebuilt `publish/SpirePlus-v0.1.0-private-beta.0.zip`.
  - First `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` exposed stale documented package hashes; after updating hash docs, the rerun passed with 132 passed, 0 skipped, 0 failed.
  - `git diff --check`: passed with CRLF normalization warnings only.
  - No live game, clicked UI, gameplay, save-load, failure/death-path, or co-op testing was run in this pass.

## 2026-05-14 - Current-package smoke/log/resource verification

- Verified current package hash parity under `.tools/runtime-evidence/current-package-smoke-20260514-015901`: installed, staging, versioned, and zip-entry DLL/PCK/manifest/README hashes match the documented `SpirePlus-v0.1.0-private-beta.0` package hashes after syncing the installed README to the package/staging copy.
- Checked Ancient UI/art resource coverage for Urda, Morvi, Lotha, and Vakuu: Urda/Morvi/Lotha event/background scenes are Control-root scenes; map icons, event art, run-history icons, and option marker relic art use separate mod-owned paths; Lotha event art is the large event illustration rather than a map thumbnail; all checked option marker art paths exist/export; EN and zhs localization keys referenced by scenes/options exist.
- Ran headless Godot against the installed `EZMicroBalance.pck`; `godot-ancient-resource-load-summary.json` reports exit code 0, `HasOkMarker: true`, 0 error lines, and 0 warning lines while loading 3 Ancient scenes and 43 Ancient textures.
- Ran the normal Steam live-session helper with only BaseLib plus Spire Plus / `EZMicroBalance` enabled. The copied `godot.log` records BaseLib `177 patches successfully, 0 failed`, `Registered config for mod EZMicroBalance`, `Finished mod initialization for 'Spire Plus' (EZMicroBalance)`, `Loaded 2 mods (2 total)`, `Found 22 SavedSpireFields`, and `Time to main menu: 14,045ms`; audit/manual scans found 0 release-blocking, missing-resource, or Ancient scene-load hits.
- Restore stopped `SlayTheSpire2`, restored settings to the original hash, restored 24 moved mod entries and 2 current-run files, preserved Steam-rehydrated test current-run files under evidence, and left 0 `SlayTheSpire2` processes. No live gameplay, clicked Ancient UI, save-load, failure/death-path, or co-op verification was run.

## 2026-05-14 - Source red-team hardening and text cleanup

- Hardened Morvi generated combat cards by routing Archive Pages, Red Ink Overdraft, and Waste Paper through `AncientCardHelpers.TryAddGeneratedCardToCombat(...)`.
- Hardened Morvi restore-sensitive state: Forbidden Loan now checks the deck add result before recording borrowed-card progress; Open-Book sealed cards get a saved card marker and are recovered by scanning combat cards; Red Ink combat-end debt uses the visible Overdraft power as a fallback source; Debt Settlement HP fallback is capped as nonlethal.
- Removed a stray Urda Seed Bank assignment that marked the second settled Seed Bank card as a Trial Branch plant.
- Added `SPIREPLUS_FORCE_ANCIENT`, `SPIREPLUS_DISABLE_URDA`, and `SPIREPLUS_FORCE_URDA_BLESSING` aliases to the Urda gate while preserving the legacy `EZMB_*` names.
- Clarified EN/zhs text for Lotha Mirror Rebuttal eligibility, Morvi Blueprint Proof Curse exclusion, Urda Root-Sight immediate timing, Vakuu victory fallback, Draw/Discard/Exhaust pile highlighting, Waste Paper's draw-pile condition, and Debt Settlement nonlethal HP fallback.
- Source review found larger live/save-load blockers that remain open: unfinished parent-linked Vakuu child combat serialization, Lotha Death Reprieve phase persistence, and player-level `SavedSpireField<Player,string>` persistence proof.
- Validation for this pass is recorded in `PROJECT_STATE.md` after the final build/test/format/publish loop.

## 2026-05-14 - Vakuu Temptation source-backed gameplay slice

- Added hidden Status card `EZMB_VAKUU_TEMPTATION` / Temptation with `Ethereal` and `Unplayable`, Status rarity/pool, `showInCardLibrary: false`, no normal combat generation, no modifier generation, English and Simplified Chinese localization, and hover tips for Ethereal, Unplayable, and Energy.
- Implemented Temptation exhaust behavior through a source-backed `AfterCardExhausted` override: when that card exhausts, its owner gains 1 Energy and loses 3 HP through `PlayerCmd.GainEnergy(...)` and unblockable/unpowered `CreatureCmd.Damage(...)`. The handler checks `card == this`, so it does not depend on hand location and should not softlock if exhausted from another pile.
- Added a dedicated Vakuu fight run hook registered from `MainFile`. It is active only when the Vakuu fight gate is enabled for a single-player run, only in `EzmbVakuuTrialEncounter`, and injects one Temptation onto the top of the draw pile after the normal player-turn draw on turns 1, 3, 5, and onward. The hook logs when injection succeeds or fails.
- Kept Vakuu's existing victory flow: `ShouldGiveRewards` remains false, the parent event resumes after victory, and victory offers up to three unowned non-Vakuu Act 3 Ancient blessings with a continue fallback.
- Checked `OPENAI_API_KEY` without printing it; it was not set. Temptation uses the existing generic custom card portrait as a temporary original asset path. No official assets were copied and no final bespoke Image API art was generated.
- Added source guards for Temptation registration/pool/library/generation shape, exhaust effects, Vakuu-only injection cadence and scope, EN/ZHS localization, exported generic portrait coverage, and active docs describing Temptation as implemented gameplay.
- Validation:
  - `git status --short --branch`: branch `main...origin/main` with the existing broad dirty worktree plus the current Vakuu Temptation/code-doc-test/package changes.
  - `git log -1 --oneline --decorate`: `a2183ee (HEAD -> main, origin/main, origin/HEAD) 1`.
  - `dotnet build EZMicroBalance.sln --no-restore`: passed with 0 warnings and 0 errors.
  - `dotnet test EZMicroBalance.sln --no-build`: first run exposed two stale source/release coverage guards, then passed with 109 passed, 18 skipped, 0 failed.
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
  - `git diff --check`: passed with CRLF normalization warnings only.
  - `dotnet publish EZMicroBalance.sln --no-restore`: passed; Godot emitted only the known nested `source code/project.godot` warning.
  - Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed with 109 passed, 18 skipped, 0 failed.
  - `scripts/package-spire-plus.ps1`: refreshed `publish/SpirePlus-v0.1.0-private-beta.0.zip` from installed artifacts after publish.
  - First `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` exposed stale documented package hashes; after updating release/status docs, it passed with 127 passed, 0 skipped, 0 failed.
- No live game, save-load, failure/death-path, or co-op testing was run in this pass.

## 2026-05-13 - Ancient player-facing polish and Vakuu source hardening

- Rewrote active Urda, Morvi, Lotha, and Vakuu visible Ancient text away from empty dialogue slots, source/testing jargon, and overly mechanical rules language. English and Simplified Chinese active localization now has guard coverage for nonempty dialogue, key parity, raw token leaks, mojibake fragments, and banned development terms.
- Normalized active dialogue declarations to use `AncientDialogueLine.sfxFallbackPath` instead of literal empty strings while keeping dialogue body text localization-backed. Added Morvi and Lotha character-specific dialogue keys for the current active character set.
- Tightened card/relic/power hover text for Seedbed, Humus, Molting, Moss Map, Trial Plant, Shallow Root, Root Mark, Root Eye, Seed Bank, Forbidden Loan, Misprint, Overdraft, Archive Page, Open Book, Paperstorm, Proofread, Debt, Rebuttal, Echo, Innocent, Closed Court, Verdict, Death Reprieve, Single Sentence, Evidence/Enlightenment, Vakuu Fight, and the pending Temptation scope. Custom card bodies no longer duplicate canonical Exhaust/Ethereal/Unplayable keyword tags.
- Hardened the Vakuu fight source shape: the fight option now awaits `RunManager.Instance.EnterRoomWithoutExitingCurrentRoom(...)`, player text explicitly says this is a real fight with no normal combat rewards and death-ending failure, and the victory path falls back to a single continue option if fewer than three unowned non-Vakuu Act 3 blessing relics are available.
- Checked `OPENAI_API_KEY` without printing it; it was not set. No final bespoke Image API art was generated. Art docs now clearly mark Urda/Morvi/Lotha/Vakuu option/icon art, generic custom card portraits, and Vakuu fight art as temporary where applicable, with final-art generation still pending.
- Subagent review split: Beauvoir reviewed text/localization/dialogue, Volta reviewed art/export/provenance, and Hooke reviewed Vakuu/reward/save-load source risk. Findings were integrated into source/docs/tests; no subagent edited files directly.
- Validation:
  - `git status --short --branch`: branch `main...origin/main` with the existing dirty worktree and current polish/package changes.
  - `git log -1 --oneline --decorate`: `a2183ee (HEAD -> main, origin/main, origin/HEAD) 1`.
  - `dotnet build EZMicroBalance.sln --no-restore`: passed with 0 warnings and 0 errors.
  - `dotnet test EZMicroBalance.sln --no-build`: first run exposed one stale art-direction guard sentence, then passed with 104 passed, 18 skipped, 0 failed.
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
  - `git diff --check`: passed with CRLF normalization warnings only.
  - `dotnet publish EZMicroBalance.sln --no-restore`: passed; Godot emitted only the known nested `source code/project.godot` warning.
  - Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed with 104 passed, 18 skipped, 0 failed.
  - `scripts/package-spire-plus.ps1`: refreshed `publish/SpirePlus-v0.1.0-private-beta.0.zip` from installed artifacts after publish.
  - `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed with 122 passed, 0 skipped, 0 failed.
- No live game, save-load, failure/death-path, or co-op testing was run in this pass.

## 2026-05-13 - Urda v2.2 ten-blessing source completion

- Promoted Urda to a default-on ten-blessing source slice with option relic art, English/zhs localization, source guards, and manual rows for `urda_seedbed`, `urda_humus_pact`, `urda_molting`, `urda_moss_map`, `urda_trial_branch`, `urda_shallow_root_relic`, `urda_rooted_route`, `urda_after_rain`, `urda_root_sight`, and `urda_seed_bank`.
- Existing four Urda blessings remain intact: Seedbed accept-only counting and no-heal max HP payoff, Humus Pact explicit `Compost Reward` payoff resolver, Molting / Withered Husk Act 2 cleanup, and Moss Map first room-type rewards.
- Implemented the remaining six with source-safe deviations where local Core evidence did not prove richer UI: Trial Branch uses a simple 4-card picker; Shallow-Root Relic uses Act 2 removal/refund rather than a `lose 6 Max HP` choice; Rooted Route auto-marks a reachable normal-combat node and does not mutate graph edges; Root-Sight auto-marks non-Boss nodes instead of adding a map button; Seed Bank stores by consuming the card reward.
- Added `UrdaTrialPlantCard`, extended Urda encoded/mirrored state, and added source guards for constants, feature gates, reward alternatives, map-marker shape, death-prevention constants, localization rich text/mojibake, and resource/export coverage.
- Added temporary source-derived Urda option icons for the six new blessings. No Image API generation was run because `OPENAI_API_KEY` was not set; bespoke relic-style icon prompts are recorded in `art-direction.md`.
- Validation:
  - `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
  - `dotnet test EZMicroBalance.sln --no-build`: first run exposed one stale art-direction guard string, then passed with 98 passed, 18 skipped, 0 failed.
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
  - `git diff --check`: passed with CRLF normalization warnings only.
  - `dotnet publish EZMicroBalance.sln`: passed; Godot emitted the known nested `source code/project.godot` ignore warning and regenerated `.uid` metadata for existing new Lotha/Morvi test files while importing the six new Urda option PNGs.
  - Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed with 98 passed, 18 skipped, 0 failed.
- No live game, save-load, or co-op testing was run in this pass.

## 2026-05-13 - Morvi v2.2 source-ready default-on slice

- Promoted Morvi from the old `EZMB_ENABLE_MORVI_V22` prototype to a default-on Act 2 private-beta test slice with `EZMB_DISABLE_MORVI` / `SPIREPLUS_DISABLE_MORVI`, force-Ancient gates, force-blessing gates, custom event art, option art, English/zhs localization, hover powers, and all eight v2.2 blessing ids.
- Copied the generated Morvi background from `art_pipeline/generated/ancient_morvi_bg_v1_v001.png` to `EZMicroBalance/images/events/ezmb_morvi.png`, added the Morvi background scene, and wired compact source-derived temporary map/run-history/option icons. These crops are not final bespoke art.
- Implemented Forbidden Loan using the source-discovered class Ancient-card pool and Borrowed Ancient deck-card marker. Source-safe deviation: the Act 2 boss follow-up auto-settles by paying 180 Gold if possible, otherwise removing the borrowed card; no post-boss choice UI is claimed.
- Implemented Misprint Press with `ModifyCardPlayCount` on the first player-driven Attack/Skill each turn. It creates no copied hand card, ignores Power/Status/Curse/autoplay/generated executions, and does not recurse.
- Implemented Red Ink Overdraft as a temporary 0-cost action card because no source-safe native Ancient combat button was proven. The card is playable only at 0 Energy, once per turn, and combat end pays 12 Gold per debt or 3 HP for each unpaid debt.
- Implemented Overdue Library's six temporary Archive Pages, Open-Book Exam's turn-1 draw/seal and turn-3 return path, Paperstorm's draw-pile Status conversion with a two-per-turn cap, Blueprint Proof's three-stack non-Status card handling, and Debt Settlement's 220/320/40/3-per-10 payment model.
- Added source guards for Morvi default-on/disabling/forcing, all eight ids, no Power-card replay/copy behavior, debt constants and decrement, Open Book constants, Archive Page ids/text, Paperstorm status conversion limit, Blueprint Proof stack behavior, localization rich text, zhs mojibake, and Morvi resource/export coverage.
- Validation in this run:
  - `dotnet build EZMicroBalance.sln`: passed with 0 warnings/errors.
  - `dotnet test EZMicroBalance.sln --no-build`: first run exposed two stale static guard strings, then passed with 98 passed, 18 skipped, 0 failed.
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
  - `git diff --check`: passed; output was CRLF normalization warnings only.
  - First `dotnet publish EZMicroBalance.sln`: failed because `export_presets.cfg` had a UTF-8 BOM before `[preset.0]`, making Godot reject `BasicExport`.
  - Removed the export preset BOM and reran `dotnet publish EZMicroBalance.sln`: passed; Godot emitted the known nested `source code/project.godot` ignore warning and imported Morvi image resources.
  - Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed with 98 passed, 18 skipped, 0 failed.
- Live game, save/load, and co-op testing were not run.

## 2026-05-13 - Lotha v2.2 corrective polish and rich-text guard slice

- Corrected the previous Lotha polish deviation: `lotha_mirror_rebuttal` now selects and marks one real non-Curse, non-Status deck card, moves the matching combat card to hand at combat start when needed, and resolves on that marked card rather than on unblocked damage.
- Replaced the old `lotha_mirror_hall_echo` copy-card behavior. It now records the last player-played non-Status Attack/Skill/Power at player-turn end and lets the next turn's first matching player-played card consume the echo. Attack/Skill adds one play; current 2026-05-14 semantics make Power cost 0 for that play and draw 1 with no Energy gain; autoplay and clones are excluded.
- Replaced the old `lotha_presumption` opening-Block placeholder. Combat start now applies visible Innocent state; each player turn while Innocent draws 2, grants 1 Energy, and grants 8 Block. Conservative break detection uses local Core damage evidence for unblocked enemy `ValueProp.Move` damage with no card source, removes Innocent, and applies immediate 8 HP loss.
- Replaced the old `lotha_closed_court` 1-Energy/2-card placeholder. Closed Court now removes only combat `CardReward` rewards for the rest of the run, leaves gold/potion/relic rewards intact, draws to the 10-card hand cap on the first player turn each combat, grants 4 Energy, and discounts the first three player-played hand cards by 1 Energy for that play.
- Corrected `lotha_deferred_verdict` to use player-owned turn-4 Verdict stacks: draw 4, gain 4 Energy, gain 3 Verdict, then consume one stack for each next non-Status card that turn. Attack/Skill cards play one additional time; current 2026-05-14 semantics make Powers cost 0 for that play and draw 1 with no Energy gain. Early combat end before turn 4 heals 4 HP.
- Replaced the old `lotha_death_reprieve` 25% heal-only placeholder. The source-safe implementation prevents death once per run, sets HP to 1, starts a reprieve player turn with draw 10, Energy 10, cost-0 cards, and temporary death prevention, then force-kills the player at that player-turn end if enemies remain. Documented deviation: enemy-turn lethal starts the reprieve on the next player turn; player-turn lethal starts immediately.
- Reworked `lotha_single_sentence` so the first player-driven Attack/Skill each turn plays two additional times, then `ShouldPlay` caps the rest of that turn at four more normal player-played cards. The first Power before that ruling costs 0 for that play, draws 1, and does not consume the sentence.
- Corrected `lotha_public_evidence` to use source power-amount hooks: player-applied Debuffs to enemies double and grant Enlightenment; enemy-applied Debuffs to the player double and remove Enlightenment; turn start consumes up to 3 Enlightenment for draw and Block.
- Removed Lotha Mirror Rebuttal generated autoplay replay copies, the unblocked-damage trigger, Deferred Verdict damage consumption/enemy Verdict main mechanic, old Single Sentence end-turn Block/Strength behavior, and stale hidden-damage text.
- Updated English and Simplified Chinese Lotha option, relic, Innocent, Death Reprieve, Verdict, and Enlightenment power text with rich-text highlights for Attack/Skill/Power, Energy, Verdict, Debuff, Enlightenment, and Block.
- Added guard coverage for all eight Lotha blessings, including rejection of old Mirror Hall copy behavior, Presumption opening-Block placeholder, Closed Court 1-Energy/2-card placeholder, Death Reprieve 25% heal-only placeholder, Mirror Rebuttal 2/2 Power fallback, Deferred Verdict 4/4/3 constants, hover/localization rich text, zhs mojibake, and stale old-mechanic wording.
- Validation in this run:
  - `dotnet build EZMicroBalance.sln`: passed with 0 warnings/errors.
  - `dotnet test EZMicroBalance.sln --no-build`: first run exposed one corrected zhs relic wording mismatch, then passed with 93 passed, 18 skipped, 0 failed.
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
  - `git diff --check`: passed; output was CRLF normalization warnings only.
  - `dotnet publish EZMicroBalance.sln`: passed; Godot emitted the known nested `source code/project.godot` ignore warning.
  - Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed with 93 passed, 18 skipped, 0 failed.
- Live gameplay, save/load, lethal-path, and co-op testing were not run.

## 2026-05-13 - Documentation cleanup and next-development TODO consolidation

- Moved the bulky v2.2 audit matrix folder from `docs/features/ancient-expansion-v2.2/audit/` to `docs/archive/feature-audits/ancient-expansion-v2.2/2026-05-13/`.
- Added `docs/archive/implementation-records/2026-05-13-spire-plus-source-test-ready-pass.md` so completed implementation status is preserved outside the active reading path.
- Rewrote `docs/README.md`, `docs/PROJECT_MAP.md`, and `docs/doc-inventory.md` to make the active reading path explicit: `PROJECT_STATE.md`, `AGENTS.md`, `docs/test-ready-development-goal.md`, `docs/issues.md`, and the v2.2 feature README/source docs.
- Kept manual checklist and completion-audit files in place because automated guards still read them, but marked them as support evidence rather than default next-development inputs.
- Updated `docs/issues.md` and `docs/issues/ancient-expansion-v2.2.md` with the next implementation TODOs: final art generation, Lotha v2.2 polish, rich text/tooltips, full Morvi decision/implementation, remaining Urda blessings, Vakuu polish, and release-ready evidence gates.
- No gameplay source, resources, package artifacts, live game, save/load, or co-op testing was changed in this cleanup pass.

## 2026-05-13 - Lotha visual asset replacement pass

- Replaced the geometric Lotha event placeholder at `EZMicroBalance/images/events/ezmb_lotha.png` with the local generated Lotha background source `art_pipeline/generated/ancient_lotha_bg_v1_v001.png`.
- Replaced the small 2KB Lotha map/run-history icon placeholders with source-derived crops from the same local Lotha background.
- Replaced the eight small Lotha blessing/relic option placeholders with source-derived temporary crops so the player no longer sees flat geometric option art.
- Replaced `EZMicroBalance/images/powers/lotha_verdict.png` with a source-derived crop.
- Added the current image-generation prompt contract to `docs/test-ready-development-goal.md` and clarified in `art-direction.md` that the source-derived Lotha option crops are temporary; bespoke Image API generation remains pending because `OPENAI_API_KEY` was not set in this environment.
- Added release-coverage guard checks so Lotha event, map, and option art cannot regress to the earlier tiny placeholders.
- Repaired active zhs localization JSON files that still contained mojibake/unterminated strings (`encounters.json`, `events.json`, `powers.json`, `relics.json`, `rest_site_ui.json`, `settings_ui.json`) and added a guard that parses every active localization JSON file.
- No live game, save/load, co-op, package refresh, or Image API generation was performed in this pass.

## 2026-05-13 - Next test-ready directive consolidation

- Replaced `docs/test-ready-development-goal.md` with the single active next-development directive.
- Archived the duplicate v2.2 implementation prompt to `docs/archive/prompts/2026-05/codex-ancient-expansion-v22-next-development-prompt.md` and removed it from the active v2.2 README/project-map path.
- Recorded the new requested implementation targets: custom Ancient UI/art repair, Lotha full test implementation, Vakuu fight implementation, Ancient dialogue/option art, and SpirePlus package/technical identity migration planning.
- Preserved the current source truth: Urda is source-backed but live-pending, Morvi remains a default-off prototype, and Lotha/Vakuu are not active in current source until the next implementation pass changes code.
- No gameplay source, resources, package artifacts, or runtime evidence were changed in this documentation consolidation pass.

## 2026-05-13 - Urda UI repair and Lotha source-complete slice

- Converted active custom Ancient background scenes to `Control` roots and added separate Urda/Lotha map and run-history icons so the large event portrait is not reused as a thumbnail.
- Added original procedural Lotha event/background/icon/option assets under `EZMicroBalance/images/events/`, `EZMicroBalance/images/ancients/lotha/`, and `EZMicroBalance/images/powers/`, plus the `ezmb_lotha.tscn` custom Ancient background scene and export entries.
- Implemented Lotha as a default-on Act 3 Ancient with `EZMB_DISABLE_LOTHA` / `SPIREPLUS_DISABLE_LOTHA`, force-Ancient gates, force-blessing gates, marker relic option art, English/zhs localization, and all eight v2.2 blessing ids.
- Implemented `lotha_death_reprieve` through the source-backed `ShouldDieLate` / `AfterPreventingDeath` death-prevention path. Live lethal-path, save/load, and co-op verification remain pending.
- Implemented the first single-player Vakuu fight source slice with a marker-relic option, `EZMB_DISABLE_VAKUU_FIGHT` / `SPIREPLUS_DISABLE_VAKUU_FIGHT`, force-Ancient gates, force-fight gates, a custom `RoomType.Event` encounter, parent-event resume on victory, and three non-Vakuu Act 3 Ancient blessing choices. Live UI/gameplay, save/load, failure/death, and co-op verification remain pending.

## 2026-05-13 - Test-ready validation guard refresh

- Re-ran the current validation chain after the Spire Plus display-name package and release docs refresh.
- Fixed `ReleaseCoverageGuardTests` so the opt-in release guard now enforces the current evidence split: Spire Plus normal Steam startup/log evidence is current, and refreshed Mod Settings UI list evidence now shows the current `Spire Plus` display name while older page-level screenshots remain historical under the old `EZ Micro Balance` display name.
- Validation:
  - `git status --short --branch`: branch `main...origin/main` with the existing dirty worktree.
  - `git log -1 --oneline --decorate`: `a2183ee (HEAD -> main, origin/main, origin/HEAD) 1`.
  - `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
  - `dotnet test EZMicroBalance.sln --no-build`: passed, 81 passed, 17 skipped after the private-beta release completion audit guard refresh.
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
  - `git diff --check`: passed with CRLF normalization warnings only.
  - `dotnet publish EZMicroBalance.sln`: passed.
  - Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed, 81 passed, 17 skipped after the private-beta release completion audit guard refresh.
  - `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed, 98 passed, 0 skipped.
  - `scripts/check-installed-ezmb-package.ps1`: passed; installed DLL, manifest, and PCK hashes match `docs/private-beta-verification-handoff.md`.
  - `dotnet test EZMicroBalance.sln -c Release`: passed, 81 passed, 17 skipped.
  - `scripts/audit-godot-log.ps1` on `.tools/runtime-evidence/current-spire-plus-normal-steam-20260513-054241/godot.log`: clean with 0 release-blocking signature hits.
  - Normal Steam-client Mod Settings UI capture under `.tools/runtime-evidence/current-spire-plus-modsettings-20260513-111342`: `02-mod-config-list.png` shows `Spire Plus`, `godot.log` has `Loaded 2 mods (2 total)`, `Found 16 SavedSpireFields`, 0 `ERROR` lines, and settings/moved mods were restored.
- No new live gameplay, save/load, or co-op testing was performed in this validation refresh.

## 2026-05-12 - Morvi hardening and Lotha/art blocker review

- Re-audited Morvi against local Core card-play and reward flows.
- Hardened Misprint Press to use `AncientCardHelpers.TryAddGeneratedCardToCombat(...)` so a failed generated-copy add removes the unpiled clone from combat state before returning.
- Hardened Debt Settlement payoff cleanup so `DebtRewardPending` is cleared from freshly read progress only after the payoff reward resolver succeeds.
- Clarified Morvi Debt Settlement English/zhs text to say missing Gold falls back to nonlethal HP.
- Added source guards for Morvi generated-copy cleanup, clone/reentry/Power-card safety, Debt Settlement nonlethal HP fallback, delayed payoff reward UI, and event-art pending status.
- Rechecked local Act 3 Ancient source: `Glory.GetUnlockedAncients(...)` returns `AllAncients.ToList()` with no native extension hook, so any Lotha insertion would need the same narrow Harmony-postfix shape currently used by Urda/Morvi.
- Rechecked local event visuals: `NAncientEventLayout.InitializeVisuals()` loads an Ancient background scene through `AncientEventModel.CreateBackgroundScene()`, and `EventModel.GetAssetPaths(...)` preloads `BackgroundScenePath` for `EventLayoutType.Ancient`. BaseLib exposes `CustomAncientModel.CustomScenePath`, but this pass has no explicit local Morvi/Lotha source art or custom scene file to bind.
- No explicit local source file was found for `EZMicroBalance/images/events/ezmb_morvi.png` or `EZMicroBalance/images/events/ezmb_lotha.png`; no placeholder art, `.import`, or export-preset entry was added.
- Lotha and Vakuu gameplay remain planning-only in this pass. No future Urda blessing, A21-A30, or custom-character content was added.
- Validation:
  - `git status --short --branch`: branch `main...origin/main` with a pre-existing dirty worktree.
  - `git log -1 --oneline --decorate`: `c8bcaa9 (HEAD -> main, origin/main, origin/HEAD) update`.
  - `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
  - `dotnet test EZMicroBalance.sln --no-build`: passed, 77 passed, 16 skipped.
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
  - `dotnet publish EZMicroBalance.sln`: passed because localization changed; Godot emitted the known nested `source code/project.godot` ignore warning.
  - Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed, 77 passed, 16 skipped.
  - `git diff --check`: passed with CRLF normalization warnings only.
- Release artifact tests were not run because release artifact logic was not changed.
- Live game, save/load, and co-op testing remain pending.

## 2026-05-12 - Morvi/Lotha art direction and next prompt

- Recorded user-approved event-art direction for Morvi and Lotha in `art-direction.md`.
- Target paths are `EZMicroBalance/images/events/ezmb_morvi.png` and `EZMicroBalance/images/events/ezmb_lotha.png`.
- Did not copy unverified temporary image files into active resources; final image bytes still need explicit local source confirmation before export.
- Added `next-development-prompt.md` for the next implementation pass.
- Historical status at that point had Lotha/Vakuu gameplay planning-only and Morvi disabled; current Morvi is default-on/source-backed and later entries supersede this note.

## 2026-05-12 - Morvi default-off prototype

- Added `EZMB_ENABLE_MORVI_V22=1` gated Act 2 Morvi registration.
- Added `EZMB_FORCE_MORVI_BLESSING` for focused local testing.
- Added default-off source-backed prototypes for Misprint Press, Open-Book Exam, and Debt Settlement.
- Misprint Press uses Attack/Skill-only generated-copy autoplay with clone/reentry guards and Power-card exclusion.
- Open-Book Exam upgrades one Attack or Skill option in normal Act 2 combat card rewards.
- Debt Settlement grants 75 Gold on selection and adds a `Repay Debt` reward alternative for three Act 2 normal combat rewards; payoff is an upgraded card reward after the third repayment.
- Lotha, Vakuu fight, future six Urda blessings, A21-A30, and custom characters remain unimplemented.
- Live game, save/load, and co-op testing remain pending.

## 2026-05-12 - Urda acceptance hardening only

- Limited this pass to current Urda acceptance/stability work. No Morvi, Lotha, Vakuu, or future Urda blessing gameplay was added.
- Hardened Humus Pact's third payoff so `HumusCompletionPending` is cleared only after payoff resolver success; payoff card generation now happens before optional removals so a no-card fallback cannot consume removals or silently drop the payoff.
- Added/strengthened guards for Humus no `CardReward.OnSkipped`, Humus option localization, Seedbed accept-only counting, future six Urda ids not active, Morvi/Lotha/Vakuu not active, and docs not claiming Urda live/save-load verification.
- Updated local API research with negative evidence for `SavedSpireField<Player,string>` persistence: local Core `Player` serialization uses a fixed `SerializablePlayer` shape and inspected `SavedProperties` usage is card/relic/modifier-oriented, so player-field save/load remains pending live proof.
- `git status --short --branch`: branch `main...origin/main` with a pre-existing dirty worktree.
- `git log -1 --oneline --decorate`: `c8bcaa9 (HEAD -> main, origin/main, origin/HEAD) update`.
- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 75 passed, 16 skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with CRLF normalization warnings only.
- `dotnet publish EZMicroBalance.sln`: not run; this pass did not change resources, localization, export presets, or package logic.
- Live gameplay, save/load, and co-op checks were not run.

## 2026-05-12

Urda stabilization pass.

- Reviewed current Urda code against v2.2 docs, Urda docs, issue docs, local Core source, BaseLib docs, and the tutorial index as secondary orientation.
- Confirmed current reviewed HEAD before edits: `c8bcaa9 (HEAD -> main, origin/main, origin/HEAD) update`.
- Confirmed the worktree already had unrelated modified files before this pass.
- Removed the Humus Pact dependency on a global `CardReward.OnSkipped` postfix because local Core source shows skipped reward finalization can occur during reward-set abandonment or room exit.
- Added an explicit Humus Pact reward alternative and moved third-trigger removal/payoff resolution to `AfterRewardTaken`.
- Guarded Seedbed so it only counts accepted choices, is not offered when max HP cannot safely pay the cost, and uses `SetMaxHp` for the completion bonus so the documented +10 max HP does not also heal.
- Added Humus Pact reward-option localization and source/localization guards.
- Kept Morvi, Lotha, Vakuu fight, and the six future Urda blessings out of active source.
- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- Validation for this pass:
  - `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
  - `dotnet test EZMicroBalance.sln --no-build`: passed, 74 passed, 16 skipped.
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
  - `git diff --check`: passed with CRLF normalization warnings only.
  - `dotnet publish EZMicroBalance.sln`: passed because localization/resources changed. Godot emitted the known nested `source code/project.godot` ignore warning during export.
- Release artifact tests were not run because release artifact logic was not changed.
- Live gameplay, save/load, and co-op checks are still pending for this pass.

Planning ingestion pass.

- Read `PROJECT_STATE.md`, `AGENTS.md`, docs indexes, current Urda docs, Urda source files, and the user-provided v2.2 prompt/addendum.
- Confirmed current reviewed HEAD before edits: `c8bcaa9 (HEAD -> main, origin/main, origin/HEAD) update`.
- Confirmed the worktree already had unrelated modified files before this docs pass.
- Created a planning-only v2.2 feature folder and compact issue file.
- Did not implement Morvi, Lotha, Vakuu fight, new Urda blessings, Ascension, Rootblight, Boss Seal, Fission, or multiplayer gameplay.
- Did not publish/package.
