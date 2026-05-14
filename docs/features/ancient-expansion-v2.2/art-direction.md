# Ancient Expansion v2.2 Art Direction

Status: active art is usable for source/manual testing but still not final bespoke art. This file is the compact direction summary; the machine-readable source of truth for asset paths, hashes, dimensions, and final/temporary status is `art-asset-manifest.json`.

## Operational Sources

- Canonical generation prompt pack: `docs/features/ancient-expansion-v2.2/art-generation-prompts.md`.
- Required generation mode for new final image requests: `GPTimage2`.
- Active asset manifest: `docs/features/ancient-expansion-v2.2/art-asset-manifest.json`.
- Audit helper: `scripts/audit-ancient-art-assets.ps1`.
- Remote gpt4free request helper: `scripts/invoke-ancient-art-gpt4free.ps1`.

Do not use older ignored `art_pipeline/prompts/*.md` calibration prompts for new final art. Do not generate with a generic imagegen/default model. Remote gpt4free calls must carry `generation_mode`, `mode`, and `semantic_model` as `GPTimage2` and must assemble prompt text from `art-generation-prompts.md`. The current local `OpenaiChat` image transport exposes `gpt-image` as the API model name, so `scripts/invoke-ancient-art-gpt4free.ps1` maps the transport `model` to `gpt-image` by default while keeping `GPTimage2` as the audited generation mode. If `GPTimage2` mode is unavailable, leave final art pending instead of generating mismatched assets.

## Current Art Stance

- Final bespoke Image API art generated this pass: none.
- Morvi event background: Active Morvi event art uses `art_pipeline/generated/ancient_morvi_bg_v1_v001.png`.
- Lotha event background: Active event art now uses `art_pipeline/generated/ancient_lotha_bg_v1_v001.png`.
- Urda event background: Urda source provenance must be corrected before any final-art claim.
- Urda, Morvi, and Lotha option/icon art is now source-local reviewed option/icon/card art promoted from the manual ChatGPT UI review batch.
- Vakuu fight option art is source-local reviewed art promoted from the same small-art review batch.
- Custom card portraits now use source-local reviewed files for Urda Seedling, Withered Husk, Morvi Archive Pages, Red Ink Overdraft, Waste Paper, and Vakuu Temptation.
- These promoted files are active next-round testing art, not `final_generated` Image API/GPTimage2 records.

## Style Contract

All new generated Ancient art must follow the core style in `art-generation-prompts.md`:

```text
Slay the Spire 2 inspired dark fantasy roguelike card-game art, hand-painted 2D illustration, rough gouache and oil brush texture, painterly flat colors, strong black silhouette, uneven ink outline, muted navy-purple shadows, small saturated highlights, grotesque but charming fantasy design, readable at small size, not realistic, not anime, not 3D, not overpolished.
```

The practical control points are:

- Strong silhouettes are more important than detail density.
- Low-detail but high-recognition shapes are preferred.
- Use deep navy, black-purple, gray-green, and dirty brown bases, with saturated highlights only on eyes, candles, gems, hearts, seals, or runes.
- Edges must feel rough, hand-painted, and imperfect.
- Relics must read as cursed tabletop tokens or card-game icons, not product renders.
- No visible text, logos, UI, official game assets, watermarks, or release numbers.

## Event Art Direction

| Ancient | Target resource path | Current direction |
| --- | --- | --- |
| Morvi, the Lender-Scribe | `EZMicroBalance/images/events/ezmb_morvi.png` | Blue-lit lender-scribe court, sealed contract, skeletal hands, ledger/typewriter, one central blue eye. Fits debt, archive, and borrowed-power rules. |
| Lotha, the Judge | `EZMicroBalance/images/events/ezmb_lotha.png` | Mirror tribunal/event chamber with crystal panes, grotesque reflected figures, evidence/relic shards, and a central judge/oracle mirror-heart motif. Fits verdict, evidence, rebuttal, and judgment rules. |
| Urda, Loamweaver | `EZMicroBalance/images/events/ezmb_urda.png` | Root nursery, seedbed, trial growth, soil, moss, route hints, and ancient root-mask silhouette. Current source provenance still needs correction before final-art claims. |

## Temporary Art Risks

- Urda, Morvi, and Lotha map and run-history pairs may intentionally share source-local reviewed filled/outline bytes until a live UI test proves separate variants are needed.
- Five generic fallback assets remain temporary: `images/powers/power.png`, `images/powers/big/power.png`, `images/relics/relic.png`, `images/relics/big/relic.png`, and `images/relics/relic_outline.png`.
- Lotha's first user-preferred mirror-ensemble event crop remains in `.tools/art-generation/promotion-candidates/proposed/` for review and is not active.
- These are acceptable only for manual-test candidate work. They must not be called final art.

## Asset Hygiene Rules

- Use original generated or user-supplied art only.
- Do not copy official Slay the Spire 2 assets.
- Do not use images with visible logos, UI text, watermarks, or unofficial release numbers.
- Do not use placeholder art for Morvi or future active Ancients just to satisfy the export list.
- When final image bytes are available, copy them to target resource paths, let Godot generate `.import` files, add any new PNG paths to `export_presets.cfg`, update `art-asset-manifest.json`, and run the art audit plus build/test/publish validation.

## Next Integration Checklist

- [x] Promote reviewed option relics, identity icons, Lotha verdict power, Vakuu fight art, and six custom card portraits into active resource paths for the next art-testing round.
- [ ] Generate final art only through `GPTimage2` using `art-generation-prompts.md` if the private-beta art policy requires `final_generated` provenance instead of source-local reviewed art.
- [ ] Decide whether to keep the current active Lotha event background or replace it with a true 2.13:1 version of the first user-preferred mirror-ensemble preview.
- [ ] Replace or explicitly accept the five generic fallback power/relic assets before any final-art release claim.
- [ ] Correct Urda event source provenance: either record the real source for `EZMicroBalance/images/events/ezmb_urda.png` or replace it with the documented generated source and update hashes.
- [ ] Replace duplicated map/run-history icon pairs only if live UI evidence proves the shared source-local reviewed filled/outline art is insufficient.
- [ ] Verify no release doc claims live gameplay/save-load readiness before runtime evidence exists.
