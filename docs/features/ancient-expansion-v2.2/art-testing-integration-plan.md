# Ancient Art Testing And Integration Plan

Status date: 2026-05-14

This plan separates source/gameplay testing from final-art integration. The current review PNGs have now been promoted into active resources for the next art-testing round, except for the optional Lotha first-preview event-background crop, which remains review-only.

## Readiness Decision

- Source/gameplay testing can proceed with the promoted source-local reviewed art.
- Final Image API/GPTimage2 provenance is still not claimed because these files came from the manual ChatGPT UI fallback, not the audited gpt4free/Image API path.
- The current small-art review set has been promoted. More small-icon generation is not needed before the next in-game preview pass.
- The Lotha event background still needs one explicit choice before replacement: keep the current active wide mirror background, or use/regenerate the first user-approved `Crystal throne of shattered visions` direction as a true 2.13:1 background.

## Review Outputs

- Blueprint overview: `.tools/art-generation/chatgpt/ancient-art-blueprint-preview-v1.png`
- Exact target-size preview: `.tools/art-generation/chatgpt/ancient-art-promotion-target-preview-v1.png`
- Candidate manifest: `.tools/art-generation/promotion-candidates/promotion-candidates-manifest.json`
- Candidate files: `.tools/art-generation/promotion-candidates/`

Current candidate manifest summary:

- 40 former `ready_candidate` items are active resources: option relics, identity icons, Lotha verdict power, and Vakuu fight option art.
- 12 former `needs_code_path` items are active resources: six custom card portraits, each with small and big variants; the card source now uses unique image paths instead of generic `card.png` / `big/card.png`.
- 1 `needs_user_review` item remains review-only: `lotha_event_background_first_preview_crop`, a 1831x859 crop of the first user-approved mirror ensemble.

## Art Categories

### Event Backgrounds

Purpose: clicked Ancient event background art.

Current state:

- Urda and Morvi active backgrounds are usable for source/gameplay testing.
- Lotha active background is functional but does not match the user-preferred first mirror-ensemble preview.
- A non-integrated Lotha candidate exists at `.tools/art-generation/promotion-candidates/proposed/EZMicroBalance/images/events/ezmb_lotha_first_preview_crop_1831x859.png`.

Decision needed before integration:

- If the crop is visually acceptable, promote it as the Lotha event background.
- If the crop feels too compressed, regenerate a true 2.13:1 version using the first-preview prompt below.

### Option Relics

Purpose: Ancient choice option art and related relic hover art.

Current state:

- Lotha: 8 ready candidates at 160x120.
- Morvi: 8 ready candidates at 128x128.
- Urda: 10 ready candidates at 128x128.

Current active decision:

- The reviewed option relics are active at the matching `EZMicroBalance/images/ancients/**/options/` paths.
- Manifest records use `source_local_generated`, not `final_generated`, because this was a manual ChatGPT UI fallback promotion.

### Identity Icons

Purpose: map and run-history filled/outline Ancient icons.

Current state:

- Urda filled/outline candidates are 128x128.
- Morvi filled/outline candidates are 96x96.
- Lotha filled/outline candidates are 96x96.
- The map and run-history copies intentionally share the same filled or outline art unless a later UI test proves separate variants are needed.

Current active decision:

- The filled/outline candidates are active at `EZMicroBalance/images/ancients/{ancient}/`.
- Map and run-history pairs intentionally share source-local reviewed filled/outline files unless live UI evidence proves separate variants are needed.

### Power And Fight Icons

Purpose: `lotha_verdict` power art and Vakuu fight-option art.

Current state:

- `lotha_verdict_power`: ready at 96x96.
- `vakuu_fight_option_relic`: ready at 256x256.

Current active decision:

- Direct PNG bytes are promoted and manifest hashes are updated.

### Card Portraits

Purpose: Urda Seedling, Withered Husk, Morvi Archive Pages, Morvi Red Ink Overdraft, Morvi Waste Paper, and Vakuu Temptation custom card art.

Current state:

- Visual candidates are prepared at 250x190 and 1000x760.
- They are not direct drop-ins because current source still points these cards at generic card portrait paths.

Current active decision:

- Unique small/big image paths exist under `EZMicroBalance/images/card_portraits/`.
- The card source files use those paths.
- `export_presets.cfg`, `art-asset-manifest.json`, and guard tests now reject regression back to generic `card.png`.

## Prompt Definitions

### Lotha Event Background, If The Crop Is Rejected

```text
Generate one wide 2.13:1 dark fantasy event illustration.

Use the first approved mirror-ensemble composition as the target: a central obsidian crystal oracle holding a circular mirror with a glowing anatomical heart, surrounded by jagged vertical crystal mirrors. The leftmost large mirror contains an original simplified Neow-like whale-tower silhouette with a hole-punched face. Other mirrors contain simplified grotesque character clues: dripping pale beast, candle-headed priest, blue horned demon, green forest prophet, floating eye relic, masked rider, and golden lion-serpent guardian.

Keep the composition wide, dark, and readable inside a Slay the Spire 2 inspired Ancient event UI. Preserve the first preview's dark mirror-card finish, thick acrylic/gouache/marker texture, strong silhouettes, uneven black outlines, restrained navy-purple base, dirty gold and cold blue accents, and small saturated focal glows. Avoid turning it into sparse empty darkness; preserve the memorable mirror inhabitants.

No photorealism, no 3D, no anime, no glossy concept art, no readable text, no labels, no UI, no watermark.
```

### Small-Icon Repair Prompt, If Target-Size Review Fails

```text
Generate only the weak icon(s), not a full batch.

Style: Slay the Spire 2 inspired dark fantasy roguelike card-game art, hand-painted 2D illustration, rough gouache and oil brush texture, thick acrylic-like paint, marker-like dark edges, painterly flat colors, strong black silhouette, uneven ink outline, muted deep navy and purple shadows, small saturated blue/violet/warm highlights, readable at 96-128 px, not realistic, not anime, not 3D.

Correction goal: make the central symbol more readable at target size without making it clean, bright, colorful, or product-like. Keep the first-preview mirror-card material language. No letters, pseudo-writing, labels, UI, watermark, or official game assets.
```

### Card Portrait Repair Prompt, If A Card Candidate Fails In Game

```text
Generate one compact card portrait for a Slay the Spire 2 inspired roguelike card.

Subject: [single card subject].

Composition: one clear object or action, large central shape, readable at 250x190, dark simple background, a few sharp highlights. Use hand-painted flat blocks, rough gouache/oil/acrylic texture, marker-like edges, strong silhouette, muted navy-purple shadows, and one small saturated accent. Avoid tiny decorative noise, cinematic rendering, readable text, anime, 3D, or photorealism.
```

## Integration Sequence

1. Run art audit and targeted art hygiene tests against the promoted active paths.
2. Run `dotnet build`, `dotnet test`, `dotnet publish`, package refresh, and release artifact tests.
3. Generate a consolidated active preview sheet from current resource paths.
4. Decide Lotha event background crop vs regeneration; promote only after visual approval.
5. Run clicked-Ancient UI preview capture after package validation.

## Non-Goals For This Pass

- Do not copy official Slay the Spire 2 assets.
- Do not change `EZMicroBalance` manifest id or package identity.
- Do not claim private-beta art final until active PNG bytes, manifest, export, package, and live UI preview evidence all match.
