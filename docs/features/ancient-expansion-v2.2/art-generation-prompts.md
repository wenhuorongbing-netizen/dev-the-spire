# Ancient Expansion v2.2 Art Generation Prompts

This is the single operational prompt pack for replacing temporary Ancient art. Do not use the older `art_pipeline/prompts/*.md` calibration prompts as current generation input.

Do not mark any output final until the PNG exists in the target path, has been visually inspected, and `art-asset-manifest.json` records source, prompt id, target path, dimensions, SHA256, and visual notes.

## Generation Mode Contract

Every final image-generation request for this feature must explicitly use:

```text
generation_mode: GPTimage2
mode: GPTimage2
semantic_model: GPTimage2
model: GPTimage2
```

If the available generator cannot select `GPTimage2`, stop and update the workflow before generating. Do not fall back to generic imagegen defaults, `codex_builtin_imagegen`, DALL-E-style defaults, or generic epic-fantasy prompt wording. For gpt4free, an API transport model such as `gpt-image` is acceptable only when the request still records `generation_mode`, `mode`, and `semantic_model` as `GPTimage2`.

## Core Style Prompt

The visual core is:

```text
dark hand-painted roguelike card game art, Slay the Spire 2 inspired, painterly flat colors, rough oil-gouache brushwork, strong black silhouettes, stylized grotesque fantasy creatures, simple readable shapes, limited palette, deep navy shadows, sharp colored highlights, uneven ink outlines, slightly messy hand-drawn texture, not realistic, not anime, not 3D
```

Append this exact style suffix to every event, character, monster, relic, option-art, card-art, and power-art prompt:

```text
Slay the Spire 2 inspired dark fantasy roguelike card-game art, hand-painted 2D illustration, rough gouache, acrylic paint, and marker texture, painterly flat colors, strong black silhouette, uneven ink outline, transparent background for UI icons, clear storybook shapes, low line density, muted navy-purple shadows, small saturated highlights, grotesque but charming fantasy design, readable at small size, not realistic, not anime, not 3D, not overpolished.
```

Use these acceptance rules when inspecting output:

- Strong silhouette first, decorative detail second.
- Low detail and high recognizability are better than polished concept-art rendering.
- Keep bases dark: deep navy, black-purple, gray-green, dirty brown.
- Reserve saturated color for eyes, candles, gems, hearts, seals, and runes.
- Lines and edges must feel hand-painted and imperfect, not clean vector or mobile-game polish.
- Relics should look like cursed tabletop tokens or card-game icons, not product renders.
- Small UI icons, option relics, map markers, run-history icons, and power icons must be transparent PNGs with no opaque black, navy, or paper square behind the symbol.
- Use flat readable color blocks with thick acrylic/marker paint; keep line density low and avoid fine decorative hatching.
- Do not use official Slay the Spire 2 assets, web images, logos, UI, release numbers, watermarks, or visible text.

## Source-Code Visual Calibration

Use local source-code art only as read-only calibration. Do not copy, trace, remix, or paste original Slay the Spire 2 assets into `EZMicroBalance/` or generated outputs.

## Manual ChatGPT Style Anchor

Primary manual ChatGPT style anchor: `.tools/art-generation/chatgpt/crystal-throne-of-shattered-visions.png`.

This first manual preview is the current best user-approved style direction for both the mirror-event illustration and the smaller Ancient option/relic/card art review pass. Use it as the main look target: dark mirror-card finish, readable character silhouettes inside crystal shards, thick hand-painted acrylic and marker texture, restrained navy-purple base, dirty gold and cold blue accents, and a polished-but-not-glossy illustrated surface.

Do not overcorrect toward later darker and emptier iterations. `crystalline-shrine-of-fractured-souls.png`, `ritual-in-a-shattered-mirror-hall.png`, and `the-mirror-hall-of-forgotten-souls.png` are review artifacts for aspect-ratio, Neow-like silhouette, and simplification lessons only; they are not the current style target. Small option relic sheets should inherit the first preview's dark mirror-card finish and broad painted material language, then simplify only enough to remain readable as small icons.

Before any small-art review candidate is promoted to an active resource, inspect it at target size: 96px identity icons, 128px power/fight/standard option icons, 160x120 Lotha option relics, and 250x190 card portraits. If a candidate loses its silhouette or becomes mostly black at target size, regenerate only that weak asset or weak group; do not restart the whole batch. Identity outline icons should be simple UI glyphs with hollow centers, uneven painted strokes, transparent PNG with no opaque black, navy, or paper square behind the symbol, flat readable color blocks with thick acrylic/marker paint, low line density, and no paper texture, pseudo-writing, or label-like detail. Keep review contact sheets and target-size audit sheets under `.tools/art-generation/chatgpt/` until the selected PNGs are integrated and the manifest is updated.

## Browser GPTimage2 Oil-Repaint Batch Rules

The 2026-05-15 oil repaint pass used the existing logged-in Edge ChatGPT project conversation and recovered PNGs from Edge cache under `.tools/art-generation/chatgpt/oil-rebuild-20260515/`. Do not upload official game atlas art. The local `source code/images/atlases/relic_atlas.png` and `power_atlas.png` may be inspected only as read-only visual calibration: refined oil-painted object masses, black contours, broad material blocks, and no dense hatching.

Prompt ids used by the active manifest:

- `oil_rebuild_20260515_batch1_urda_vakuu`: Urda option relics, Vakuu fight icon, and neutral relic fallback.
- `oil_rebuild_20260515_batch2_morvi_lotha`: Morvi and Lotha option relics.
- `oil_rebuild_20260515_batch3_identity_power_ascension`: Ancient identity icons, Lotha Verdict, neutral power fallback, boss/firemark/fission/forge icons.
- `oil_rebuild_20260515_batch4_banners`: Banner Room, Vanguard, Shield Formation, and Bounty banner icons.
- `oil_rebuild_20260515_batch5_card_portraits`: Urda Seedling, Withered Husk, Morvi Archive Pages, Red Ink Overdraft, Waste Paper, and Vakuu Temptation card portraits.

For transparent-ready icon sheets, ask for isolated icons only, no scene, no character group, no UI, no labels, and no readable text. Use a flat chroma-key magenta background (`#ff00ff`) when true alpha is unavailable, then remove it locally. The required finish is: refined dark roguelike atlas icons, oil/acrylic/gouache painted, thick black outer contour, broad matte color blocks, low saturation, one small accent glow, no glossy mobile-game rendering, no neon color dominance, no tiny cracks/vines/scratch noise, and no simplified doodle/vector look.

For card portraits, use a 3x2 sheet with dark painted backgrounds inside each cell. Keep subjects large, simple, and readable at 250x190; no fake writing, no card frame, no captions, no glossy cinematic polish.

After slicing, preserve transparent padding during promotion. Do not crop 128x128 processed icons back to the alpha bounds and stretch them to the canvas edge. Run both the manifest audit and a pixel-alpha/edge-padding audit before packaging.

Reviewed style anchors:

- `source code/images/events/reflections.png`: extremely dark mirror-event composition, about 2.13:1, one small readable focal object, very large near-black shard shapes, red/orange accent only at the mirror crack.
- `source code/images/events/crystal_sphere.png`: extremely dark oracle/crystal event composition, about 2.13:1, small lower-center figure, huge framing curtains/shards, sparse star-line texture, bright orb as the only major glow.
- `source code/images/ancients/darv_placeholder.png`: character-heavy Ancient scene with broad flat masses, dirty muted browns/greens/purples, limited pink rim accents, and softened hand-painted edges.
- `source code/images/ancients/vakuu_placeholder.png`: large exaggerated body silhouette with simple readable anatomy, warm backlight, scroll props, and muted red-violet chamber shapes.
- `source code/images/ancients/orobas_placeholder.png`: single iconic eye creature against broad dark teal masses, few bright accents, low micro-detail, readable from a distance.
- `source code/images/packed/map/ancients/ancient_node_neow.png`: compact white Ancient-node symbol with a large whale/tower silhouette and hole-punched face. Use only as silhouette language for a new original mirror reflection, not as copied icon art.

Style conclusions for prompt review:

- Event/Ancient background source assets are wider than 16:9: event images are 3440x1616 and Ancient placeholders are 2560x1200, both about 2.13:1. Prefer a 2.13:1 wide composition for final backgrounds; 16:9 previews are acceptable only for manual ChatGPT tests, not as the final integration target.
- Keep 60-80% of the image quiet and dark for final event backgrounds where the Ancient UI needs negative space. Do not apply this as a global small-icon rule, and do not let it erase the readable mirror-character ensemble that made the first manual preview work.
- Avoid noisy equal-detail character grids. Secondary creatures should read as silhouettes, reflections, or icon-like clues, while preserving the first preview's clear group-image structure and memorable mirror inhabitants.
- Reduce glossy crystal facets and high-frequency highlights. Source crystal/mirror scenes are graphic and flat, with only a few sharp accents.
- Prefer source-like color discipline: deep blue-black, black-purple, gray-green, dirty brown, muted red-violet, plus tiny warm orange/red or cold blue-violet highlights.
- If the generated result looks like generic high-budget fantasy key art, it is too polished. Push it back toward the first preview's flat block shadows, rough gouache/acrylic texture, uneven outlines, simpler silhouettes, and hand-painted marker-like edges without making the image empty.
- For mirror-scene iterations, upload only the intended mirror-character shape references. Do not upload prior generated previews or event backgrounds as character references unless the prompt explicitly needs edit/style comparison.
- If a Neow-like whale reflection is requested, the leftmost mirror should read as an original simplified whale-tower silhouette with a hole-punched face, simple lines, limited color, and acrylic paint and marker texture. It must not become a realistic whale, fish, shark, dragon, or polished monster portrait.

## Prompt Assembly Rules

1. Set `generation_mode: GPTimage2`, `mode: GPTimage2`, and `semantic_model: GPTimage2`; set `model: GPTimage2` unless an audited gpt4free transport mapping requires an API model id such as `gpt-image`.
2. Choose the role template below: event background, character, boss, relic/item, or card portrait.
3. Fill in the bracketed concept with the asset-specific concept from the prompt block.
4. Append the core style suffix exactly.
5. Append the role's negative prompt.
6. Save PNG outputs exactly to the target paths listed under each prompt block.
7. Record prompt id, source path, target path, dimensions, SHA256, `generation_mode`, `mode`, `semantic_model`, and visual inspection notes in `docs/features/ancient-expansion-v2.2/art-asset-manifest.json`.

For manual ChatGPT UI fallback, do not include target paths, filenames, or save-directory instructions in the chat prompt. Ask only for an actual displayed image, then recover or download the PNG for review before any repository integration.

## Remote gpt4free Invocation

Use `scripts/invoke-ancient-art-gpt4free.ps1` for repository-driven remote calls. It reads this prompt pack plus `art-asset-manifest.json`, builds a per-asset request, and sends a payload with all of these fields forced to the same required value:

```text
generation_mode: GPTimage2
mode: GPTimage2
semantic_model: GPTimage2
```

The local g4f `OpenaiChat` provider currently exposes its image transport model as `gpt-image`; the helper therefore sends `model: gpt-image` by default while preserving `GPTimage2` as the generation mode and semantic model in the request and manifest audit trail. Override only with `GPT4FREE_IMAGE_MODEL` when the provider exposes a better exact transport id.

2026-05-14 live recheck: updated local gpt4free still accepts `OpenaiChat` text calls and generic `flux` image calls, but `OpenaiChat/gpt-image` image calls fail at the ChatGPT web image websocket auth path with `HTTP Error 401`. `flux`/`PollinationsImage` is useful only as an API health check and is not an acceptable final-art substitute for the required `GPTimage2` workflow.

Required local setup for a real call:

```powershell
$env:GPT4FREE_IMAGE_ENDPOINT = "http://127.0.0.1:8081/v1/images/generate"
$env:GPT4FREE_PROVIDER = "OpenaiChat"
$env:GPT4FREE_IMAGE_MODEL = "gpt-image"
$env:GPT4FREE_API_KEY = "<optional bearer token>"
.\scripts\invoke-ancient-art-gpt4free.ps1 -AssetId lotha_mirror_rebuttal_option_relic
```

Without `GPT4FREE_IMAGE_ENDPOINT`, the script writes only a dry-run request JSON under `.tools/art-generation/gpt4free/`. Do not hand-write ad hoc remote payloads from older `art_pipeline/prompts/*.md` files. If a different remote wrapper is used, it must pass the same `GPTimage2` fields and prompt text assembled from this file.

## Role Templates

### Character Portrait

```text
Create an original Slay the Spire 2 inspired character portrait.

Character concept:
[character concept]

Visual design:
The character has a strong silhouette, exaggerated proportions, strange posture, asymmetrical body, grotesque but charming expression, and a memorable fantasy costume. The body should feel hand-designed rather than anatomically realistic. Use large simple shapes, sharp readable edges, and a few iconic details.

Art style:
Dark hand-painted roguelike card game art, painterly flat color, rough oil and gouache brush texture, visible brush strokes, thick uneven ink outlines, muted shadows, deep navy background, limited palette, one or two saturated accent colors, dramatic rim light, ancient cursed atmosphere.

Rendering:
Simple shadow blocks, stylized highlights, rough hand-painted surface, slightly messy but intentional. Avoid tiny decorative noise. The character must remain readable as a small game portrait.

Negative prompt:
photorealistic, 3D render, anime, smooth digital painting, realistic anatomy, over-detailed armor, glossy fantasy illustration, clean vector art, modern sci-fi, symmetrical perfect body, AI-polished texture, text, logo, UI, watermark, official game asset.
```

### Boss Monster

```text
An original boss monster design inspired by Slay the Spire 2 dark fantasy card art.

A towering ancient monster with a strong central silhouette, made of [crystal / wax / bone / shadow / roots / masks / broken mirrors]. Its body is not fully human, with distorted limbs, ceremonial shapes, and an eerie divine presence. It should look like a strange rule-keeper of the spire, half sacred and half evil.

The design should be grotesque but readable: large simple body masses, exaggerated head shape, sharp crown-like elements, dark cloak-like lower body, unnatural hands, and one iconic object such as a mirror, relic, lantern, heart, mask, or staff.

Hand-painted 2D dark fantasy style, Slay the Spire 2 inspired, rough oil-gouache brushwork, painterly flat colors, uneven black outlines, deep navy and purple shadows, sharp blue crystal highlights, muted palette with small glowing accents. The monster should look like a card-game boss illustration, not a realistic creature.

Keep the composition simple and iconic. Strong silhouette, readable at small size, no excessive micro-details.

Negative prompt:
photorealism, 3D, anime, realistic monster anatomy, smooth airbrush, hyper-detailed skin pores, overly polished fantasy art, symmetrical clean design, sci-fi robot, cute mascot, text, logo, UI, watermark, official game asset.
```

### Relic Or Option Item

```text
An original relic item illustration inspired by Slay the Spire 2 roguelike card game art.

A strange ancient relic: [item concept].

The relic should be centered as a transparent PNG with a strong readable silhouette and iconic shape. It should look like a small game item icon, not a realistic object. Use exaggerated proportions, slight asymmetry, old worn materials, cracks, stains, small glowing details, and cursed fantasy symbolism. Do not place an opaque black, navy, paper, or rectangular background behind the symbol.

Hand-painted 2D item art, rough gouache/acrylic and marker texture, painterly flat colors, thick uneven dark outline, limited palette, flat readable color blocks, low line density, small saturated highlight, ancient spire atmosphere. Readable at small size.

Negative prompt:
photorealistic object, 3D render, product photography, clean vector icon, opaque square background, black background box, paper card background, pseudo-writing, tiny decorative hatching, overly detailed ornament, realistic metal, modern object, smooth polished digital painting, anime, text, logo, UI, watermark, official game asset.
```

### Card Portrait

```text
A dark fantasy card illustration inspired by Slay the Spire 2.

Scene:
[single clear card scene]

Composition:
A compact card-art composition with one clear action, strong silhouette, exaggerated movement, and readable shapes. The scene should be dramatic but simple, with a dark background and a few sharp highlights. The image must work as a small card illustration.

Art style:
Hand-painted 2D roguelike card game art, rough oil-gouache brush texture, flat painterly colors, uneven dark outlines, stylized shadows, muted navy-purple palette, small intense accent color, strange ancient fantasy atmosphere, slightly dirty hand-drawn look.

Negative prompt:
photorealism, anime, 3D render, clean comic lineart, high-detail cinematic painting, realistic anatomy, overcomplicated background, too many particles, too much glow, readable letters, logo, UI, watermark, official game asset.
```

### Wide Event Background

```text
A wide dark fantasy event illustration inspired by Slay the Spire 2.

Scene:
[wide event scene]

Composition:
Wide 2.13:1 source-code event background composition, matching the proportions of `source code/images/events/reflections.png` and `source code/images/events/crystal_sphere.png`. Use a symmetrical but slightly irregular layout with one central boss-like figure or one clearly staged event focus, floating vertical shapes arranged left and right, and a dark reflective floor or deep void background. Keep large quiet negative-space regions for the Ancient event UI. Secondary elements should feel like reflections, silhouettes, or icon-like clues, not equally detailed character portraits.

Art style:
Slay the Spire 2 inspired dark hand-painted roguelike event art, rough oil-gouache brushwork, thick acrylic paint and marker texture, painterly flat colors, thick uneven black outlines, strong silhouettes, limited palette, deep blue and purple shadows, cold highlights, small warm accents, ancient cursed atmosphere, slightly messy hand-drawn texture, not too polished. Favor flat block shadows, visible dry-brush marks, and broad readable masses over glossy crystal rendering or high-detail fantasy key art.

Rendering rules:
Readable shapes, simple blocky shadows, sharp edges, muted colors, 60-80% quiet dark area, one or two saturated focal glows, no photorealism, no 3D, no anime, no hyper-detailed rendering, no glossy cinematic concept art, no text, no logo, no UI.
```

## Batch Order

1. `lotha_event_background`, only if the current background is replaced.
2. `lotha_option_relics`, then `lotha_power_art`.
3. `vakuu_fight_and_temptation`.
4. `morvi_option_relics`, then `morvi_card_portraits`.
5. `urda_option_relics`, `urda_card_portraits`, then `ancient_identity_icons`.

## Prompt Block: lotha_event_background

Target:

- `EZMicroBalance/images/events/ezmb_lotha.png`

Prompt:

```text
A mysterious ancient chamber filled with floating jagged crystal mirrors, following the first manual preview's readable mirror-character ensemble and dark mirror-card finish. In the center stands a tall obsidian crystal oracle with a crown of broken shards, holding a circular mirror that contains a glowing anatomical heart. Use the surrounding mirrors as clear but simplified character clues, not noisy equal-detail rendering. The leftmost large mirror should contain an original simplified Neow-like whale-tower silhouette with a hole-punched face, simple lines, limited color, and a calm strange presence. Other mirrors may show a dripping pale beast, a candle-headed scarecrow priest, a blue horned demon, a golden lion-serpent guardian, masked riders, a green forest prophet, and a floating eye relic. Keep the chamber dark, flat, thick-painted, and readable, but do not overcorrect into an empty or overly sparse composition.

Use the Wide Event Background template and append the Core Style Prompt suffix.
```

## Prompt Block: lotha_option_relics

Targets:

- `EZMicroBalance/images/ancients/lotha/options/lotha_mirror_rebuttal.png`
- `EZMicroBalance/images/ancients/lotha/options/lotha_mirror_hall_echo.png`
- `EZMicroBalance/images/ancients/lotha/options/lotha_presumption.png`
- `EZMicroBalance/images/ancients/lotha/options/lotha_closed_court.png`
- `EZMicroBalance/images/ancients/lotha/options/lotha_deferred_verdict.png`
- `EZMicroBalance/images/ancients/lotha/options/lotha_death_reprieve.png`
- `EZMicroBalance/images/ancients/lotha/options/lotha_single_sentence.png`
- `EZMicroBalance/images/ancients/lotha/options/lotha_public_evidence.png`

Use the Relic Or Option Item template. Item concepts:

- `lotha_mirror_rebuttal`: cracked hand mirror reflecting a blade and a trial scroll, bronze-gold rim, dark violet glass, strong icon silhouette.
- `lotha_mirror_hall_echo`: narrow corridor of broken mirrors repeating one glowing card silhouette into darkness.
- `lotha_presumption`: judgment scale wrapped in pale protective light over a black court floor.
- `lotha_closed_court`: sealed courtroom door with a golden lock, black wax seal, and muted violet shadows.
- `lotha_deferred_verdict`: hourglass above a judge seal, delayed blue light gathering inside cracked glass.
- `lotha_death_reprieve`: cracked execution seal held above one final candle, dark background, small red accent.
- `lotha_single_sentence`: one card on a tribunal desk struck by three sharp beams of light.
- `lotha_public_evidence`: open evidence folder with glowing debuff marks, mirrored fingerprints, and blue-gold proof tokens.

## Prompt Block: lotha_power_art

Target:

- `EZMicroBalance/images/powers/lotha_verdict.png`

Use the Relic Or Option Item template with this concept:

- `lotha_verdict`: compact blue-gold verdict seal made of cracked mirror glass, three small judgment marks orbiting it, one cold candle reflection below, strong circular silhouette, readable as a 96x96 buff icon.

## Prompt Block: vakuu_fight_and_temptation

Targets:

- `EZMicroBalance/images/ancients/vakuu/options/vakuu_fight.png`
- final small Temptation portrait target to be chosen before integration.
- final large Temptation portrait target to be chosen before integration.

Use the Relic Or Option Item template for:

- `vakuu_fight`: hooked challenge blade crossing a dark Ancient mask, with three stolen blessing sparks behind it, centered fight-token shape, worn cursed metal, sharp asymmetry, tiny ember highlights.

Use the Card Portrait template for:

- `vakuu_temptation`: small black-red status card charm with a bitten blue flame, hooked shadow teeth, and a tempting energy spark inside a cracked seal. It should read as dangerous and alluring at card size, with no readable text or UI.

## Prompt Block: morvi_option_relics

Targets:

- `EZMicroBalance/images/ancients/morvi/options/morvi_forbidden_loan.png`
- `EZMicroBalance/images/ancients/morvi/options/morvi_misprint_press.png`
- `EZMicroBalance/images/ancients/morvi/options/morvi_red_ink_overdraft.png`
- `EZMicroBalance/images/ancients/morvi/options/morvi_overdue_library.png`
- `EZMicroBalance/images/ancients/morvi/options/morvi_open_book_exam.png`
- `EZMicroBalance/images/ancients/morvi/options/morvi_paperstorm.png`
- `EZMicroBalance/images/ancients/morvi/options/morvi_blueprint_proof.png`
- `EZMicroBalance/images/ancients/morvi/options/morvi_debt_settlement.png`

Use the Relic Or Option Item template. Item concepts:

- `morvi_forbidden_loan`: forbidden contract-card clamped by a red wax seal, with one bright debt hook through the corner.
- `morvi_misprint_press`: crooked hand press stamping two offset sword/card marks in wet black ink.
- `morvi_red_ink_overdraft`: cracked inkwell spilling red ink into a coin-shaped ring.
- `morvi_overdue_library`: chained archive book with three loose glowing pages.
- `morvi_open_book_exam`: open cheat-sheet book with a small blue candle and sealed exhaust-page ribbon.
- `morvi_paperstorm`: spiraling stack of torn status pages around a tiny lightning-shaped quill.
- `morvi_blueprint_proof`: blue proofing pencil correcting a card blueprint with three bright check marks.
- `morvi_debt_settlement`: heavy ledger token stamped with a broken paid seal and stacked coins.

## Prompt Block: morvi_card_portraits

Targets to choose before integration:

- Morvi Archive Pages small and large card portraits.
- Red Ink Overdraft small and large card portraits.
- Waste Paper small and large card portraits.

Use the Card Portrait template. Card scenes:

- `morvi_archive_pages`: a cluster of six haunted archive pages escaping a blue-lit ledger, each page showing a different abstract mark: draw, veil, burn, discount, bravery, dexterity. Torn paper edges, ink stains, skeletal hand shadow, one central blue eye glow.
- `morvi_red_ink_overdraft`: a cracked red inkwell overflowing into coin-shaped debt rings, a quill hooked like a claw, blue ledger light underneath. The image should feel like a risky one-turn bargain.
- `morvi_waste_paper`: a wad of cursed waste paper spinning through black-blue wind, torn red stamps, frayed page corners, and one dim energy spark caught in the paper storm.

## Prompt Block: urda_option_relics

Targets:

- `EZMicroBalance/images/ancients/urda/options/urda_seedbed.png`
- `EZMicroBalance/images/ancients/urda/options/urda_humus_pact.png`
- `EZMicroBalance/images/ancients/urda/options/urda_molting.png`
- `EZMicroBalance/images/ancients/urda/options/urda_moss_map.png`
- `EZMicroBalance/images/ancients/urda/options/urda_trial_branch.png`
- `EZMicroBalance/images/ancients/urda/options/urda_shallow_root_relic.png`
- `EZMicroBalance/images/ancients/urda/options/urda_rooted_route.png`
- `EZMicroBalance/images/ancients/urda/options/urda_after_rain.png`
- `EZMicroBalance/images/ancients/urda/options/urda_root_sight.png`
- `EZMicroBalance/images/ancients/urda/options/urda_seed_bank.png`

Use the Relic Or Option Item template. Item concepts:

- `urda_seedbed`: living seedbed token, three moss seeds in a cracked black bowl, one seed glowing green.
- `urda_humus_pact`: rich black soil clump tied with a root contract cord and a small buried card shard.
- `urda_molting`: shed bark husk curled around a pale new sprout, with a brittle shell silhouette.
- `urda_moss_map`: moss-grown route tile with tiny root trails and one marked combat stone.
- `urda_trial_branch`: small forked sapling branch wrapped around an upgraded card shard, with two green knots glowing like counted combats.
- `urda_shallow_root_relic`: cracked common relic half-buried in pale roots, with one root tied to a tiny gold coin.
- `urda_rooted_route`: crooked map pin grown from a root, planted in a small torn route tile with three card leaves.
- `urda_after_rain`: broken rain bell sprouting from a puddle, two thorny wound drops beside a single blue shield glint.
- `urda_root_sight`: five dark root-eye seeds arranged around a tiny non-Boss room token, one eye glowing with potion light.
- `urda_seed_bank`: sealed seed coffer holding three card-shaped seeds, with the first seed bright and the second marked by a trial sprout.

## Prompt Block: urda_card_portraits

Targets to choose before integration:

- Urda Seedling small and large card portraits.
- Withered Husk small and large card portraits.

Use the Card Portrait template. Card scenes:

- `urda_seedling`: a small stubborn seedling pushing through black soil inside a cracked wooden charm, root arms raised like a defensive gesture, dim green glow, charming but eerie.
- `withered_husk`: a brittle dried plant husk shaped like a curled shield, thorn veins and hollow seed eyes, pale dust falling away, unsettling but readable at card size.

## Prompt Block: ancient_identity_icons

Targets:

- `EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon.png`
- `EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon_outline.png`
- `EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon.png`
- `EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon_outline.png`
- `EZMicroBalance/images/ancients/morvi/ezmb_morvi_map_icon.png`
- `EZMicroBalance/images/ancients/morvi/ezmb_morvi_map_icon_outline.png`
- `EZMicroBalance/images/ancients/morvi/ezmb_morvi_run_history_icon.png`
- `EZMicroBalance/images/ancients/morvi/ezmb_morvi_run_history_icon_outline.png`
- `EZMicroBalance/images/ancients/lotha/ezmb_lotha_map_icon.png`
- `EZMicroBalance/images/ancients/lotha/ezmb_lotha_map_icon_outline.png`
- `EZMicroBalance/images/ancients/lotha/ezmb_lotha_run_history_icon.png`
- `EZMicroBalance/images/ancients/lotha/ezmb_lotha_run_history_icon_outline.png`

Use the Relic Or Option Item template. Icon concepts:

- `urda_identity`: tiny readable dark fantasy map icon: ancient root spiral around a seed, moss-green glow, black soil base, uneven hand-painted outline. Make one filled icon and one clear outline/silhouette variant.
- `morvi_identity`: tiny readable dark fantasy map icon: lender-scribe ledger eye, blue contract page, skeletal quill hook, one red debt seal. Make one filled icon and one clear outline/silhouette variant.
- `lotha_identity`: tiny readable dark fantasy map icon: obsidian mirror judge shard, cracked crystal crown, tiny glowing heart reflection. Make one filled icon and one clear outline/silhouette variant.
