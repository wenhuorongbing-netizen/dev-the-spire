# Ancient Art Testing And Integration Plan

Status date: 2026-05-15

This plan separates source/gameplay testing from final-art integration. The previous source-local review PNGs were superseded by the browser ChatGPT/GPTimage2 oil-repaint pass under `.tools/art-generation/chatgpt/oil-rebuild-20260515/`. The Lotha first-preview event-background crop and the Urda 2.13:1 reframe are now active middle-draft event backgrounds.

## Readiness Decision

- Source/gameplay testing can proceed with the promoted browser GPTimage2 oil-repaint art.
- Final small-art provenance is claimed through the manifest for 69 browser ChatGPT/GPTimage2 `final_generated` records. This covers option relics, identity icons, Lotha Verdict, Vakuu fight, fallback power/relic assets, Ascension indicators/banners/status icons, and six custom card portraits.
- More small-icon generation is not needed before the next in-game preview pass unless live UI screenshots expose scale/readability issues.
- Event backgrounds are now unified to the 1831x859 / about 2.13:1 target and use full-scene cover fitting. They still need live clicked-UI screenshots before release-ready claims.

## Review Outputs

- Current active small-art contact sheet: `.tools/art-generation/chatgpt/oil-rebuild-20260515/active-small-art-contact.png`
- Current card-portrait contact sheet: `.tools/art-generation/chatgpt/oil-rebuild-20260515/processed/batch5-card-portraits-contact.png`
- Historical blueprint overview: `.tools/art-generation/chatgpt/ancient-art-blueprint-preview-v1.png`
- Historical target-size preview: `.tools/art-generation/chatgpt/ancient-art-promotion-target-preview-v1.png`
- Historical candidate manifest: `.tools/art-generation/promotion-candidates/promotion-candidates-manifest.json`
- Active event-background contact sheet: `.tools/art-generation/event-background-reframe-20260515/active-event-backgrounds-1831x859-contact.png`

Historical candidate manifest summary:

- 40 former `ready_candidate` items are active resources: option relics, identity icons, Lotha verdict power, and Vakuu fight option art.
- 12 former `needs_code_path` items are active resources: six custom card portraits, each with small and big variants; the card source now uses unique image paths instead of generic `card.png` / `big/card.png`.
- The former `needs_user_review` Lotha crop is now active as `EZMicroBalance/images/events/ezmb_lotha.png`.

## Art Categories

### Event Backgrounds

Purpose: clicked Ancient event background art.

Current state:

- Urda, Morvi, and Lotha active backgrounds are all 1831x859 source-local middle-draft resources.
- Urda is a y=80 2.13:1 reframe of the user-accepted root-mother background.
- Morvi already matched the 2.13:1 event-background target.
- Lotha now uses `.tools/art-generation/promotion-candidates/proposed/EZMicroBalance/images/events/ezmb_lotha_first_preview_crop_1831x859.png`, replacing the older silver judge background.

Decision needed before release-ready art claims:

- Capture live clicked Ancient UI screenshots for Urda, Morvi, and Lotha and verify title/text overlays do not hide the focal silhouettes.
- If the Lotha crop feels too compressed in live UI, regenerate a true 2.13:1 version using the first-preview prompt below.

### Option Relics

Purpose: Ancient choice option art and related relic hover art.

Current state:

- Lotha: 8 active final-generated option relics.
- Morvi: 8 active final-generated option relics.
- Urda: 10 active final-generated option relics.

Current active decision:

- The oil-repaint option relics are active at the matching `EZMicroBalance/images/ancients/**/options/` paths.
- Manifest records for active option relics use `final_generated` with `generation_mode`, `mode`, and `semantic_model` set to `GPTimage2`.

### Identity Icons

Purpose: map and run-history filled/outline Ancient icons.

Current state:

- Urda filled/outline candidates are 128x128.
- Morvi filled/outline candidates are 96x96.
- Lotha filled/outline candidates are 96x96.
- The map and run-history copies intentionally share the same filled or outline art unless a later UI test proves separate variants are needed.

Current active decision:

- The filled/outline candidates are active at `EZMicroBalance/images/ancients/{ancient}/`.
- Map and run-history pairs intentionally share final browser GPTimage2 oil-repaint filled/outline files unless live UI evidence proves separate variants are needed.

### Power And Fight Icons

Purpose: `lotha_verdict` power art and Vakuu fight-option art.

Current state:

- `lotha_verdict_power`: ready at 96x96.
- `vakuu_fight_option_relic`: ready at 256x256.

Current active decision:

- Direct oil-repaint PNG bytes are promoted and manifest hashes are updated.

### Card Portraits

Purpose: Urda Seedling, Withered Husk, Morvi Archive Pages, Morvi Red Ink Overdraft, Morvi Waste Paper, and Vakuu Temptation custom card art.

Current state:

- Oil-repaint portraits are active at 250x190 and 1000x760.
- Current source points these cards at unique portrait paths instead of generic card portrait paths.

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
