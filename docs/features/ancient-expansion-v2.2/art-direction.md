# Ancient Expansion v2.2 Art Direction

Status: active small UI, option relic, fallback relic/power, Ascension UI, and custom card portrait art now has browser ChatGPT/GPTimage2 final-generation provenance for the private-beta art pass. Urda, Morvi, and Lotha event backgrounds are now restored to the user-approved 16:9 source images because live clicked-UI screenshots showed the prior 2.13 cover-fit repair cropped the Ancient screens incorrectly. The machine-readable source of truth for asset paths, hashes, dimensions, and final status is `art-asset-manifest.json`.

## Operational Sources

- Canonical generation prompt pack: `docs/features/ancient-expansion-v2.2/art-generation-prompts.md`.
- Required generation mode for new final image requests: `GPTimage2`.
- Active asset manifest: `docs/features/ancient-expansion-v2.2/art-asset-manifest.json`.
- Audit helper: `scripts/audit-ancient-art-assets.ps1`.
- Remote gpt4free request helper: `scripts/invoke-ancient-art-gpt4free.ps1`.

Do not use older ignored `art_pipeline/prompts/*.md` calibration prompts for new final art. Do not generate with a generic imagegen/default model. Remote gpt4free calls must carry `generation_mode`, `mode`, and `semantic_model` as `GPTimage2` and must assemble prompt text from `art-generation-prompts.md`. The current local `OpenaiChat` image transport exposes `gpt-image` as the API model name, so `scripts/invoke-ancient-art-gpt4free.ps1` maps the transport `model` to `gpt-image` by default while keeping `GPTimage2` as the audited generation mode. If `GPTimage2` mode is unavailable, leave final art pending instead of generating mismatched assets.

## Current Art Stance

- Final browser GPTimage2 small art generated this pass: Urda/Morvi/Lotha/Vakuu option relics, Ancient identity icons, Lotha verdict power art, Ascension indicators/banners/status icons, neutral fallback power/relic assets, and six custom Ancient card portraits.
- Morvi event background: Active Morvi event art uses the recovered user-uploaded blue-eye court source archived at `.tools/art-generation/event-background-repair-20260515-live-feedback/sources/morvi-blue-eye-court-upload-source.png`.
- Lotha event background: Active event art now uses the corrected user-uploaded horizontal mirror-ensemble source recovered from Edge cache at `.tools/art-generation/lotha-background-repair-20260515-feedback/sources/lotha-horizontal-mirror-ensemble-upload-source.png`; the older `crystal-throne-of-shattered-visions.png` file is a similar but rejected composition.
- Urda event background: Active event art is the original user-accepted 16:9 Urda middle-draft at `.tools/art-generation/event-background-reframe-20260515/head-backup/EZMicroBalance/images/events/ezmb_urda.png`.
- Urda, Morvi, Lotha, Vakuu, fallback, and Ascension small art uses browser ChatGPT/GPTimage2 oil-repaint transparent PNGs with target-size review contact sheets under `.tools/art-generation/chatgpt/oil-rebuild-20260515/`.
- Urda, Morvi, and Lotha option/icon art uses browser ChatGPT/GPTimage2 rebuilt transparent PNGs; the current oil-repaint pass is the latest version of that rebuilt-art line.
- Vakuu fight option art uses the same browser GPTimage2 rebuild pass.
- Custom card portraits now use browser GPTimage2 rebuilt files for Urda Seedling, Withered Husk, Morvi Archive Pages, Red Ink Overdraft, Waste Paper, and Vakuu Temptation.
- No `generic_temporary` or `final_required_before_release` art blockers remain in the manifest after this pass.
- The current manifest tracks 95 assets, including 90 `final_generated` entries. Ascension small-art assets are now manifest-tracked rather than only indirectly covered by package/export tests.

## Style Contract

All new generated Ancient art must follow the core style in `art-generation-prompts.md`:

```text
Slay the Spire 2 inspired dark fantasy roguelike card-game art, hand-painted 2D illustration, rough gouache, acrylic paint, and marker texture, painterly flat colors, strong black silhouette, uneven ink outline, transparent background for UI icons, clear storybook shapes, low line density, muted navy-purple shadows, small saturated highlights, grotesque but charming fantasy design, readable at small size, not realistic, not anime, not 3D, not overpolished.
```

The practical control points are:

- Strong silhouettes are more important than detail density.
- Low-detail but high-recognition shapes are preferred.
- Use deep navy, black-purple, gray-green, and dirty brown bases, with saturated highlights only on eyes, candles, gems, hearts, seals, or runes.
- Edges must feel rough, hand-painted, and imperfect.
- Relics must read as cursed tabletop tokens or card-game icons, not product renders.
- UI icon resources must use transparent PNG padding, not opaque black/navy square backgrounds.
- Promotion must preserve transparent padding; do not crop final icons back to their alpha bounds and stretch them to the target edge.
- Keep small resources flat, vivid, and logical: one symbol, thick outline, low line density, and acrylic/marker texture.
- No visible text, logos, UI, official game assets, watermarks, or release numbers.

## Event Art Direction

| Ancient | Target resource path | Current direction |
| --- | --- | --- |
| Morvi, the Lender-Scribe | `EZMicroBalance/images/events/ezmb_morvi.png` | Blue-lit lender-scribe court, sealed contract, skeletal hands, ledger/typewriter, one central blue eye. Fits debt, archive, and borrowed-power rules. |
| Lotha, the Judge | `EZMicroBalance/images/events/ezmb_lotha.png` | Corrected user-uploaded 16:9 horizontal mirror-ensemble image: vertical crystal panes, Neow-like whale, grotesque reflected figures, central obsidian oracle, and handheld heart mirror. Fits verdict, evidence, rebuttal, and judgment rules. |
| Urda, Loamweaver | `EZMicroBalance/images/events/ezmb_urda.png` | Original user-accepted 16:9 root nursery with seedbed, soil, moss, route-hint standing stones, and ancient root-mask silhouette. Fits growth, burial, route, and Max HP trade rules. |

## Remaining Art Risks

- Urda, Morvi, and Lotha map and run-history pairs intentionally share final browser GPTimage2 oil-repaint filled/outline bytes until a live UI test proves separate variants are needed.
- The map and run-history pairs intentionally share final browser GPTimage2 filled/outline bytes; the current files are the oil-repaint replacements recorded in the manifest.
- Neutral fallback assets are no longer temporary blockers: `images/powers/power.png`, `images/powers/big/power.png`, `images/relics/relic.png`, `images/relics/big/relic.png`, and `images/relics/relic_outline.png` were rebuilt from browser GPTimage2 sources.
- Event backgrounds are active middle-draft resources, not final generated event art. The background scenes use keep-aspect centered fitting so 16:9 art is not cropped in the clicked Ancient UI. Beta.108 smoke-level clicked UI proof exists for forced Urda, Morvi, Lotha, and normal Vakuu; art readiness still requires live hover/readability review and gated Vakuu fight-option checks before release-ready claims.

## Asset Hygiene Rules

- Use original generated or user-supplied art only.
- Do not copy official Slay the Spire 2 assets.
- Do not use images with visible logos, UI text, watermarks, or unofficial release numbers.
- Do not use placeholder art for Morvi or future active Ancients just to satisfy the export list.
- When final image bytes are available, copy them to target resource paths, let Godot generate `.import` files, add any new PNG paths to `export_presets.cfg`, update `art-asset-manifest.json`, and run the art audit plus build/test/publish validation.

## Next Integration Checklist

- [x] Promote reviewed option relics, identity icons, Lotha verdict power, Vakuu fight art, and six custom card portraits into active resource paths for the next art-testing round.
- [x] Generate final small-art replacements through browser ChatGPT/GPTimage2 and record `final_generated` provenance, hashes, dimensions, source paths, and review sheets in the manifest.
- [x] Add Ascension indicator/banner/status icons to the manifest and export audit coverage.
- [x] Replace the rejected similarly named Lotha `crystal-throne` image with the correct user-uploaded horizontal mirror-ensemble source recovered from Edge cache after live/user review.
- [x] Replace the five generic fallback power/relic assets with final neutral browser GPTimage2 art.
- [x] Correct Urda event source provenance by restoring the original user-accepted 16:9 middle-draft source and active hash.
- [ ] Replace duplicated map/run-history icon pairs only if live UI evidence proves the shared final filled/outline art is insufficient.
- [ ] Verify no release doc claims live gameplay/save-load readiness before runtime evidence exists.
