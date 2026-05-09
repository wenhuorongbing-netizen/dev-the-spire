# EZ Micro Balance Issues

This file tracks player-reported and runtime-observed issues. Do not mark an item release-ready unless source validation and live verification both support it.

## Open

### Current Open Blocker Audit - 2026-05-08 RC1 / 2026-05-09 Rootblight Source Pass

The remaining open issues are not blocked by the automated build/test/package loop. They require one of the following evidence classes before they can be closed:

- **Rootblight UX source pass completed on 2026-05-09:** Rootblight/Blight Sprout card text style, duplicate Exhaust wording, card previews, add-to-deck notice, and generated portrait art now have source, localization, docs, package, and guard coverage. English and Simplified Chinese live hover/text passed for the four Rootblight-family cards, and the A14 Neow starter add notice passed in both languages after the event-room fallback. Combat-end add notices are source-hardened with a top-level overlay path but still need clean non-paused/Blight Sprout/co-op verification; generated-art live visual verification also remains pending.
- **Two-client Steam evidence:** multiplayer HP 0 / Neow blocked, Save & Quit propagation, run-start black screen, A20 TypeLoad retest, A11-A20 selection, A20 warning, and the full co-op matrix require host and client `godot.log` captures from live Steam-client runs.
- **Single-player live gameplay evidence:** A11 natural route traversal and boss reachability, A12 rich-text tooltip rendering, A13/A16 Fission reward frequency, Rootblight/Blight Sprout behavior/text/previews/notices, and inherited marker regressions require targeted live route/combat/reward checks.
- **Resolved dependency gate retained for traceability:** the BaseLib `Creature.get_ShowsInfiniteHp` API-drift blocker is resolved for the dependency/runtime gate and no longer blocks single-player smoke; remaining multiplayer retests are tracked by the separate co-op issues in this Open section.

Minimum evidence packet for closing a live issue:

- Normal Steam-client launch, not `--force-steam off`.
- BaseLib + EZ Micro Balance only unless the issue explicitly asks for a multi-mod compatibility run.
- Screenshot or log line proving the selected Ascension/debug gate.
- `%APPDATA%\SlayTheSpire2\logs\godot.log` copied before another run overwrites it.
- For co-op issues, both host and client logs from the same attempt, plus the lobby/run start timing and selected Ascension.
- Explicit scan result for release-blocking signatures: `Creature.get_ShowsInfiniteHp`, `BaseLib.Patches.UI.HealthBarForecastPatch`, BaseLib patch failures, non-EZMB mod stack traces, EZMB error/exception, `TypeLoadException`, and `MissingMethodException`.
- Recommended scanner: run `scripts/audit-godot-log.ps1 -Path <copied godot.log> -OutFile <evidence-dir>\godot-log-audit.json -FailOnHit` for clean-log gates, or omit `-FailOnHit` when collecting known-failing diagnostic logs.

Open issue closure checklist:

| Issue | Missing evidence before close |
| --- | --- |
| `ISSUE-2026-05-09-ROOTBLIGHT-CARD-TEXT-STYLE-PREVIEW-DUPLICATE-EXHAUST` | English and Simplified Chinese live hover checks passed: Rootblight/Blight Sprout show one visible Exhaust keyword, no raw `[gold]` tags, and expected preview cards under `.tools\runtime-evidence\rootblight-a14-hover-eng-20260509-044010` and `.tools\runtime-evidence\rootblight-a14-ui-eng-20260509-033516`. Keep source/localization/package guards active while broader Rootblight behavior remains open. |
| `ISSUE-2026-05-09-CARD-TEXT-STYLE-GUIDE-FOR-EZMB` | Source/docs guard is complete, and the English/ZHS Rootblight hover passes found no follow-up localization drift. Keep the guide enforced during future card text changes. |
| `ISSUE-2026-05-09-ROOTBLIGHT-ADD-TO-DECK-NOTICE-MISSING` | A14 Neow starter notice passed in English under `.tools\runtime-evidence\rootblight-a14-hover-eng-20260509-044010` and in Simplified Chinese under `.tools\runtime-evidence\rootblight-a14-notice-zhs-step-20260509-040455`. Combat-end additions from Rootblight III split / Blight Sprout seen-unplayed outcomes now use the source-hardened top-level overlay notice path, but clean non-paused timing, Blight Sprout, and co-op ownership/desync notice behavior still need live verification. Source/localization/tests/package are patched. |
| `ISSUE-2026-05-09-ROOTBLIGHT-CARD-ART-PENDING` | Source/package fixed with generated art; close after live visual verification confirms the in-game portraits render as intended. |
| `ISSUE-2026-05-08-PENDING-VISUALS-AND-DIAGNOSTICS` | Remaining backlog now excludes source implementation for Rootblight text/preview/starter-notice work, generated Rootblight-family card art, and the A14 English/ZHS hover/starter-notice proof. Combat-end Rootblight notices are source-hardened but still need full non-paused/Blight Sprout/co-op verification. Generated-art visual verification, bespoke A11/A17 feedback, multiplayer matrix, and Ancient/co-op save/load backlog remain. |
| `ISSUE-2026-05-08-MULTIPLAYER-A11-A20-RUN-START-HP0-NEOW-BLOCKED` | Two-client Steam retest with `EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS=1`, plus `EZMB_ASCENSION_DISABLE_ALL_SYSTEMS=1` and vanilla A10 comparison logs. |
| `ISSUE-2026-05-08-MULTIPLAYER-SAVE-QUIT-NOT-PROPAGATING` | Same-attempt host/client co-op logs around Save & Quit proving whether disconnect propagation or UI return fails. |
| `ISSUE-2026-05-08-MULTIPLAYER-RUN-START-BLACK-SCREEN` | Fresh host/client run-start logs that distinguish HP0/Neow, transport sync, timeout, and runtime exception causes. |
| `ISSUE-2026-05-08-MULTIPLAYER-A20-BLACK-SCREEN-OPTIONAL-BOSS-TYPELOAD` | Host/client A20 retest showing no `DoormakerBoss`/Door Wedge type-load crash and no replacement EZMB run-start exception. |
| `ISSUE-2026-05-08-ASCENSION-PUBLIC-SELECTION-DEFAULT-ON-FOR-MP-TEST` | Normal Steam single-player and host-multiplayer selector checks for default-on, public-disable, multiplayer-disable, and A20 warning paths. |
| `ISSUE-2026-05-07-A11-MAP-LENGTH-NOT-PLAYER-VISIBLE` | Natural route traversal through A11 map nodes to boss reachability; existing DevConsole act jumps only prove map surfaces. |
| `ISSUE-2026-05-07-A11-MAP-CHANGE-ANIMATION` | User decision on whether current A11 geometry/A17 hover feedback is acceptable or whether bespoke visual feedback should be implemented. |
| `ISSUE-2026-05-07-A12-TOOLTIP-RICHTEXT-COLORS` | Live tooltip screenshots/logs for Forge Token, Firemark, Banner, A12/A13 rows, rest-site text, and Chinese wrapping with no raw tags. |
| `ISSUE-2026-05-07-A13-FISSION-TOO-RARE-AT-HIGH-ASCENSION` | A13/A16 live reward-frequency sampling for normal combat, Banner Room, Firemarked Elite, and boss reward screens. |
| `ISSUE-2026-05-07-ROOTBUD-ROOTBLIGHT-REWORK` | Live A14/A15/A18 Rootblight and Blight Sprout behavior checks, plus user-resumed visual feedback and independent card art work. |
| `ISSUE-2026-05-07-MULTIPLAYER-A11-A20-SELECTION-BLOCKED` | Two-client host lobby selection checks for A11-A20 default-on and disable flags, then A11/A12/A14/A16/A20 run-start/desync checks. |
| `ISSUE-2026-05-07-A20-MULTIPLAYER-SELECTION-WARNING-MISSING` | Host multiplayer A20 selection and run-start logs proving the downgrade warning appears before/after client join. |
| `ISSUE-2026-05-07-LIVE-COOP-A11-A20-MATRIX-PENDING` | Full two-client matrix with host/client logs, screenshots, save/load rows, ownership checks, and desync scan results. |

### ISSUE-2026-05-09-ROOTBLIGHT-CARD-TEXT-STYLE-PREVIEW-DUPLICATE-EXHAUST

Priority: P1

Status: source-patched and package-guarded; English and Simplified Chinese live hover/text verification passed for the four Rootblight-family cards. Keep guards active while broader Rootblight combat behavior remains open.

Area: A14/A15/A18 Rootblight / Blight Sprout card text, visible keyword display, rich text, card previews, localization

Player report (2026-05-09):

- Fission / 裂变 icon is currently acceptable and should not be changed in this pass.
- Firemarked Elite marker is currently acceptable and should not be changed in this pass.
- Rootblight I/II/III and Blight Sprout are mechanically present, but their card descriptions are confusing.
- Rootblight cards already display the visible Exhaust / 消耗 keyword, but the description text also manually says `Play: Exhaust` / `打出：消耗`, causing duplicate Exhaust / 消耗 display.
- Rootblight text lacks the official card-description style: important card names and pile names are not highlighted, and player-facing card references do not show previews.
- “After combat, add Rootblight I/II” should show a card preview so the player can inspect the resulting card, similar to official cards that add Soul / 灵魂.

Original source evidence that triggered the fix:

- `EZMicroBalanceCode/Ascension/Cards/RootCards.cs`
  - `RootFamilyCard.CanonicalKeywords => ExhaustKeyword`.
  - `RootBud.CanonicalKeywords => ExhaustKeyword`.
  - `RootFamilyCard` and `RootBud` therefore already expose visible Exhaust / 消耗.
- `EZMicroBalance/localization/eng/cards.json`
  - Original Rootblight descriptions included `Play: Exhaust`.
- `EZMicroBalance/localization/zhs/cards.json`
  - Original Rootblight descriptions included `打出：消耗`.
- This combination causes duplicate keyword presentation and should be removed.

Implementation notes (2026-05-09):

- English and Simplified Chinese Rootblight/Blight Sprout descriptions no longer manually repeat Exhaust wording; visible Exhaust remains provided by `CanonicalKeywords`.
- Important Rootblight card names and Draw Pile text now use `[gold]...[/gold]`.
- Rootblight previews are implemented with `HoverTipFactory.FromCard<T>()`, following the source-backed official Soul preview pattern.
- Automated guards cover duplicate Exhaust prevention, `[gold]` terms, preview source shape, localization parity, and package artifact freshness.
- Normal Steam-client BaseLib+EZMB-only A14 English hover screenshots under `.tools\runtime-evidence\rootblight-a14-hover-eng-20260509-044010` and ZHS hover screenshots under `.tools\runtime-evidence\rootblight-a14-ui-eng-20260509-033516` verified Rootblight I/II/III and Blight Sprout show one visible Exhaust keyword, no raw `[gold]` tags, and expected Rootblight previews.

Required source research:

- Use current v0.105.0 `source code/src/Core/**` as primary evidence.
- Search official cards/localization and card models for:
  - `GRAVE_WARDEN`
  - `SOUL`
  - `Soul`
  - `REAVE`
  - `CAPTURE_SPIRIT`
  - `GLIMPSE_BEYOND`
  - `DIRGE`
  - `SEVERANCE`
  - `CardPreview`
  - `PreviewCard`
  - `HoverTips`
  - `CanonicalCards`
  - `CanonicalVars`
  - `CanonicalKeywords`
  - `DynamicVar`
  - `CardModel`
  - `ModelDb.Card`
- Record source evidence in `docs/features/ascension-11-20/api-research.md` or `docs/features/ascension-11-20/work-log.md`.
- Do not copy large source bodies. Summarize exact class/key/API names and conclusions only.

Official style target:

- Follow the same pattern used by official cards that create or add another card, especially Grave Warden / Soul examples.
- Important card names should be gold-highlighted.
- Important pile names such as Draw Pile / 抽牌堆 should be gold-highlighted.
- Referenced cards should show previews where source-proven preview APIs allow it.
- Do not manually write keywords already provided by `CanonicalKeywords`, such as Exhaust / 消耗.

Required behavior:

1. Remove duplicate manual Exhaust wording from Rootblight I/II/III and Blight Sprout descriptions.
   - English must not contain `Play: Exhaust`.
   - Simplified Chinese must not contain `打出：消耗`.
   - Keep visible `CardKeyword.Exhaust` through source-backed `CanonicalKeywords`.
2. Rewrite English and Simplified Chinese descriptions in shorter official-style lines.
3. Use `[gold]...[/gold]` for important card names and pile names.
4. Add source-backed card previews:
   - Rootblight I previews Rootblight II.
   - Rootblight II previews Rootblight I and Rootblight III.
   - Rootblight III previews Rootblight I and Rootblight II.
   - Blight Sprout previews Rootblight I.
5. Preview cards must not pollute deck, run state, RNG, save data, or multiplayer state.
6. Preview implementation must be safe in card library / hover / combat / reward contexts. If owner/run state is unavailable, use a safe fallback or no preview, not a crash.

Suggested final English copy, after verifying exact behavior:

- `EZMB_ROOT.description`:
  - `Remove this from your deck.`
  - `After combat, if this was not played or removed, it becomes [gold]Rootblight II[/gold].`
- `EZMB_DEEP_ROOT.description`:
  - `When played, remove this from your deck. After combat, add a [gold]Rootblight I[/gold].`
  - `If not played or removed this combat, it becomes [gold]Rootblight III[/gold].`
- `EZMB_ROOTBLIGHT_III.description`:
  - `When played, remove this from your deck. After combat, add a [gold]Rootblight II[/gold].`
  - `If not played or removed this combat, add a [gold]Rootblight I[/gold] after combat once.`
- `EZMB_ROOT_BUD.description`:
  - `Sprout 3/4: at that round's start, if this has not entered your hand, put it on top of your [gold]Draw Pile[/gold].`
  - `If seen and not played, after combat add a [gold]Rootblight I[/gold].`

Suggested final Simplified Chinese copy, after verifying exact behavior:

- `EZMB_ROOT.description`:
  - `将本牌从你的主牌组中移除。`
  - `战斗后，若本牌未被打出或移除，变为[gold]根蚀 II[/gold]。`
- `EZMB_DEEP_ROOT.description`:
  - `打出时，将本牌从你的主牌组中移除；战斗后，加入1张[gold]根蚀 I[/gold]。`
  - `若本战未打出或移除，战斗后变为[gold]根蚀 III[/gold]。`
- `EZMB_ROOTBLIGHT_III.description`:
  - `打出时，将本牌从你的主牌组中移除；战斗后，加入1张[gold]根蚀 II[/gold]。`
  - `若本战未打出或移除，战斗后加入1张[gold]根蚀 I[/gold]，仅一次。`
- `EZMB_ROOT_BUD.description`:
  - `萌发3/4：对应回合开始时，若本牌还未进入手牌，将其置于你的[gold]抽牌堆[/gold]顶部。`
  - `若见到后未打出，战斗后加入1张[gold]根蚀 I[/gold]。`

Text correctness notes:

- Do not use “upgrade” / “升级” unless the actual mechanic is a card upgrade. Prefer “becomes” / “变为” for Rootblight stage changes.
- Do not describe both played and unplayed outcomes as if they happen together.
- Rootblight II and III played outcomes should clearly say “When played...” / “打出时...” and unplayed growth should be a separate sentence.
- If `打出时` itself appears with the visible keyword, that is acceptable; do not include `消耗` in the same phrase.

Tests required:

- English/ZHS descriptions must not contain `Play: Exhaust` or `打出：消耗`.
- English/ZHS descriptions must contain `[gold]Rootblight I[/gold]`, `[gold]Rootblight II[/gold]`, `[gold]Rootblight III[/gold]`, `[gold]根蚀 I[/gold]`, `[gold]根蚀 II[/gold]`, `[gold]根蚀 III[/gold]` where applicable.
- Blight Sprout text must contain `[gold]Draw Pile[/gold]` and `[gold]抽牌堆[/gold]`.
- RootFamilyCard and RootBud must still expose `CardKeyword.Exhaust` via `CanonicalKeywords` or source-proven equivalent.
- Add a source guard for preview implementation covering the preview matrix above.
- Add a localization guard so raw unsupported tags or duplicated keywords cannot reappear.

Manual verification required:

- Hover Rootblight I: one visible Exhaust keyword only; Rootblight II preview visible.
- Hover Rootblight II: one visible Exhaust keyword only; Rootblight I and III previews visible.
- Hover Rootblight III: one visible Exhaust keyword only; Rootblight I and II previews visible.
- Hover Blight Sprout: one visible Exhaust keyword only; Rootblight I preview visible.
- English and Simplified Chinese render [gold] markup correctly, with no raw tags.

### ISSUE-2026-05-09-CARD-TEXT-STYLE-GUIDE-FOR-EZMB

Priority: P1/P2

Status: source-implemented and guard-covered; English and Simplified Chinese Rootblight live hover/text passes found no localization drift.

Area: card localization style, naming conventions, dynamic variables, preview rules, bilingual text consistency

Problem:

- Rootblight text repeated a visible keyword that the card already displays from `CanonicalKeywords`.
- Rootblight text did not use official-style rich text for important card/pile names.
- Rootblight references to generated/added cards did not provide previews.
- This class of mistake can reappear in future EZMB card text unless a local style guide exists.

Required behavior:

- Create `docs/style/card-localization-style-guide.md`.
- If `docs/style/` does not exist, create it.
- Add a short pointer in `AGENTS.md` or `docs/skills/sts2-godot-mod-development.md`.
- The guide must be based on current v0.105.0 source/localization evidence, not only intuition.

Required source research:

- Search official `source code` localization and card models for the official patterns used by cards that create/add/preview other cards.
- Include Grave Warden / Soul examples if source confirms them.
- Record exact localization keys/classes discovered, such as `GRAVE_WARDEN`, `SOUL`, and related card model preview APIs.
- Include English and Simplified Chinese examples.

The guide must cover at least:

1. **Visible keyword rule**
   - If a card already exposes Exhaust / Retain / Innate / Eternal / similar through `CanonicalKeywords` or a source-proven keyword API, do not manually repeat the same keyword in the description.
   - Example anti-pattern: `Play: Exhaust.` / `打出：消耗。` on a card that already has visible Exhaust.
2. **Rich-text rule**
   - Important card names, pile names, and official named concepts should use `[gold]...[/gold]` when official examples do so.
   - Numbers should use dynamic vars where possible.
3. **Dynamic variable rule**
   - Use `{Cards:diff()}`, `{Damage:diff()}`, `{Energy:energyIcons()}`, etc. when values can change through upgrades/modifiers.
   - Do not hard-code values that can become wrong after upgrade.
4. **Preview rule**
   - If text says “add a card”, “becomes a card”, “put a card into pile”, or equivalent, provide a card preview if a source-proven safe API exists.
   - Preview cards must not alter game state, RNG, save data, or piles.
5. **English/ZHS consistency rule**
   - Behavior, counts, and conditions must match across English and Simplified Chinese.
   - Do not over-compress Chinese to the point of ambiguity.
6. **Terminology rule**
   - Rootblight = 根蚀
   - Blight Sprout / Root Bud = 根芽
   - Draw Pile = 抽牌堆
   - Discard Pile = 弃牌堆
   - Exhaust Pile = 消耗牌堆
   - Deck / master deck = 牌组 / 主牌组 depending on source context; use consistently.
7. **Manual checklist rule**
   - Each card text change must update manual checklist rows for hover, preview, text rendering, and raw-tag checks.

Tests required:

- Source/document guard checks that `docs/style/card-localization-style-guide.md` exists.
- Guard checks that the guide mentions:
  - `CanonicalKeywords`
  - duplicate Exhaust prevention
  - `[gold]`
  - card preview
  - English/Simplified Chinese consistency
  - Rootblight terminology
- Guard checks that AGENTS or the repo skill points to the guide.

Implementation notes (2026-05-09):

- Added `docs/style/card-localization-style-guide.md`.
- Indexed the guide from `AGENTS.md`, `docs/README.md`, and `docs/skills/sts2-godot-mod-development.md`.
- Added guard coverage for duplicate-keyword prevention, `[gold]`, card previews, English/Simplified Chinese consistency, and Rootblight terminology.

### ISSUE-2026-05-09-ROOTBLIGHT-ADD-TO-DECK-NOTICE-MISSING

Priority: P1/P2

Status: source-patched and package-refreshed; A14 Neow starter notice live retests passed in English and Simplified Chinese after the event-room fallback. Combat-end notice source now prefers a top-level run overlay with high z-order, input passthrough, and a longer display duration before falling back to the run global UI container. Full combat-end behavior, Blight Sprout, non-paused notice timing, and co-op ownership/desync notice checks remain pending before close.

Area: A14/A15/A18 Rootblight add-to-deck feedback, card gain notice, combat-end feedback, multiplayer safety

Player report:

- When Rootblight is added to the deck, the player receives no clear prompt, animation, curse-style notice, or visible feedback.
- Player may not realize a Rootblight card was added.

Current source evidence to verify:

- `RootDeckService.AddRootblightCard(...)` currently adds cards with:
  - `CardPileCmd.Add(rootblightCard, PileType.Deck, CardPilePosition.Bottom, source: null, skipVisuals: true)`
- This likely suppresses the standard card-add visual path.
- `ShowRootSystemFull(...)` already uses `ThinkCmd.Play(new LocString("ascension", "ROOT_SYSTEM_FULL"), player.Creature, 2.0)` for cap notices, proving a lightweight notice path exists.

Required source research:

- Search current v0.105.0 source for:
  - `CardPileCmd.Add`
  - `skipVisuals`
  - `ThinkCmd.Play`
  - card reward pickup
  - curse added to deck
  - event cards added to deck
  - generated temporary card added to hand/draw/discard
  - card obtain popup / preview / notification APIs
- Record source evidence in `api-research.md` or `work-log.md`.
- Identify which API is multiplayer-safe and command-safe.

Required behavior:

- When Rootblight I/II/III is successfully added to the player’s master deck, the affected player should receive a short player-facing notice or animation.
- Notice should not spam or duplicate excessively if multiple Rootblights are added in one resolution; if multiple additions can happen, pick a clear but not overwhelming policy.
- Notice must be per-player in co-op; it must not falsely show for other players.
- Notice must not mutate game state other than the intended add-to-deck action.
- If full card-add animation is source-proven safe, use it.
- If not, keep `skipVisuals: true` but add a `ThinkCmd.Play` notice after successful add.

Implementation notes (2026-05-09):

- `RootDeckService.AddRootblightCard` now checks the `CardPileCmd.Add` result before returning success.
- The add path keeps `skipVisuals: true` and shows a localized `ROOTBLIGHT_ADDED` notice only for `LocalContext.IsMe(player)`.
- The notice uses the vanilla `ThinkCmd.Play(...)` path when the player's creature has a VFX container. A14 Neow runtime evidence showed this path is silent in event rooms, so the source now falls back to `NEventRoom.Instance?.VfxContainer` with `NThoughtBubbleVfx.Create(...)`, then to a run overlay notice if needed.
- Combat-end additions pass `preferOverlayNotice: true`; that path now tries a top-level `NGame.Instance` thought bubble first, sets `MouseFilterEnum.Ignore`, sets `ZIndex = 4096`, uses a 5-second display duration, and falls back to `NRun.Instance.GlobalUi.AboveTopBarVfxContainer`.
- English and Simplified Chinese notice localization are present; automated guards require the notice path and keys.
- Normal Steam-client A14 English retest under `.tools\runtime-evidence\rootblight-a14-hover-eng-20260509-044010` verified `07-after-confirm-a14-neow.png`: Rootblight I is added at Neow, the deck count is 11, and the localized Rootblight-added bubble is visible.
- Normal Steam-client A14 ZHS retest under `.tools\runtime-evidence\rootblight-a14-notice-zhs-step-20260509-040455` verified `07-run-start-06.png`: Rootblight I is added at Neow, the deck count is 11, and the localized Rootblight-added bubble is visible.
- Pre-final-hardening combat-end probe `.tools\runtime-evidence\rootblight-combat-end-overlay-eng-20260509-053834` captured the Rootblight III split notice above the loot/pause overlay and then restored settings/saves/22 moved mods. That probe is not enough to close the full combat-end requirement because it did not complete Blight Sprout coverage or a clean non-paused timing check.

Suggested notice localization:

- English:
  - `ROOTBLIGHT_ADDED`: `Rootblight added.`
  - Optional level-specific if easy: `Rootblight {Level} added.`
- Simplified Chinese:
  - `ROOTBLIGHT_ADDED`: `根蚀已加入。`
  - Optional level-specific if easy: `根蚀 {Level} 已加入。`

Implementation options:

- **Option A:** Set `skipVisuals: false` only if source evidence proves this displays a safe standard card-add animation and does not break combat/save/multiplayer.
- **Option B:** Keep `skipVisuals: true`, then call `ThinkCmd.Play(...)` with a short localized message for the affected player.
- **Option C:** Use a source-proven card obtain popup / preview notice, if available and safe.

Tests required:

- Guard that Rootblight add path is not completely silent.
- If `skipVisuals: true` remains, require `ThinkCmd.Play` or equivalent after successful add.
- Require new localization keys in English and ZHS if a notice key is added.
- Guard against adding notice in unrelated card-add paths.

Manual verification required:

- A14 new run: when Rootblight I is added, player sees a notice/animation.
- Rootblight III split or RootBud seen-unplayed outcome: when Rootblight I is added after combat, player sees a notice/animation.
- If multiplayer is later tested, confirm only the affected player’s Rootblight notice is shown and no ownership/desync warning appears.

### ISSUE-2026-05-09-ROOTBLIGHT-CARD-ART-PENDING

Priority: P2

Status: source/package fixed with original generated art; live in-game visual verification pending.

Area: Rootblight I/II/III and Blight Sprout card art

Player report:

- Rootblight I/II/III and Blight Sprout previously did not have independent pictures.
- This pass generated original procedural portraits and packaged them under the documented per-card paths.
- Fission / 裂变 icon is acceptable and should not be changed in this pass.
- Firemarked Elite marker is acceptable and should not be changed in this pass.

Current source evidence:

- `RootBud.CustomPortraitPath` and `RootBud.PortraitPath` now try the documented `blight_sprout.png` / `big/blight_sprout.png` paths and fall back to generic `images/card_portraits/card.png` / `big/card.png` while art is absent.
- `RootFamilyCard.CustomPortraitPath` and `RootFamilyCard.PortraitPath` now try the documented `rootblight_i.png`, `rootblight_ii.png`, and `rootblight_iii.png` paths and fall back to generic `card.png` while art is absent.
- Therefore Rootblight I/II/III and Blight Sprout no longer display the shared placeholder art once the latest package is loaded; live visual verification still needs to confirm in-game rendering.

Required behavior after art is provided:

- Use original art only. Do not use official Slay the Spire 2 assets.
- No text, numbers, logos, or official characters in generated art.
- Add small and big portraits:
  - `EZMicroBalance/images/card_portraits/rootblight_i.png`
  - `EZMicroBalance/images/card_portraits/rootblight_ii.png`
  - `EZMicroBalance/images/card_portraits/rootblight_iii.png`
  - `EZMicroBalance/images/card_portraits/blight_sprout.png`
  - `EZMicroBalance/images/card_portraits/big/rootblight_i.png`
  - `EZMicroBalance/images/card_portraits/big/rootblight_ii.png`
  - `EZMicroBalance/images/card_portraits/big/rootblight_iii.png`
  - `EZMicroBalance/images/card_portraits/big/blight_sprout.png`
- `RootCards.cs` already resolves these paths with a generic fallback; after art is provided, add the files, import/export/package them, and remove or update the placeholder release note.
- Add/import Godot `.import` metadata as needed.
- Update `export_presets.cfg` so the files are packaged.
- Update package/PCK/hash docs after publish.
- Record art SHA256 and source/provenance in release docs.

Current pass expectation:

- If user has not provided art yet, do not invent art unless user explicitly requests image generation.
- Keep this issue open.
- Release notes should say Rootblight-family generated portrait art is included, with live visual verification pending.

Tests required after art integration:

- Rootblight I/II/III and Blight Sprout must resolve to their specific portrait files once art is supplied, rather than falling back to generic `card.png`.
- Export preset includes the new images.
- PCK contains the new images and does not contain source/docs/art_pipeline material.
- No official assets are packaged.

### ISSUE-2026-05-08-PENDING-VISUALS-AND-DIAGNOSTICS

Priority: P2/P3

Status: partially superseded by the 2026-05-09 Rootblight source pass. Rootblight text/previews have English and Simplified Chinese live hover proof, the A14 Neow starter add notice has English and Simplified Chinese live proof after the event-room fallback, and generated portrait art is packaged. Combat-end notices, generated-art live visual verification, and co-op ownership/desync notice checks remain pending.

Area: Rootblight visuals / A11 diagnostics / manual verification backlog

Pending items deliberately left out of the current fix pass or requiring user decision:
- Rootblight animation/feedback beyond the specific add-to-deck notice issue, unless implemented through `ISSUE-2026-05-09-ROOTBLIGHT-ADD-TO-DECK-NOTICE-MISSING`.
- Rootblight I/II/III and Blight Sprout generated-art live visual verification.
- Broader A11 map geometry diagnostics and natural traversal checks beyond the Act 1/2/3 width/row spot checks. Current normal Steam-client A11 map evidence is recorded in `docs/rc1-live-validation-log.md`.
- Multiplayer matrix and Ancient/co-op save/load verification.

### ISSUE-2026-05-08-MULTIPLAYER-A11-A20-RUN-START-HP0-NEOW-BLOCKED

Priority: P0

Status: diagnostics patch exists and is default-off; unsolved until live co-op retest. Controlled BaseLib+EZMB loader smoke is clean on BaseLib `v3.1.2`; host/client co-op Neow HP still needs live retest.

Area: multiplayer A11-A20 run start / Neow initialization / player HP

Player report (v0.105.0, 2026.05.08, co-op):
- Two-player co-op entered Neow screen with Ascension >10 selected.
- Local player HP displayed as 0/80.
- Cannot select Neow blessing.
- Singleplayer works fine with the same Ascension level.

Current source analysis:

- `AncientEventModel.BeforeEventStarted` (source code/src/Core/Models/AncientEventModel.cs:143-156) sets player HP to 0 via `SetCurrentHpInternal(0m)`, then heals via `CreatureCmd.Heal` to full (or 80% for A2+ WearyTraveler). This works in singleplayer.
- Vanilla `AscensionManager` (`source code/src/Core/Entities/Ascension/AscensionManager.cs`) has `maxAscensionAllowed = 10` and only handles A4 (TightBelt -1 potion) and A10 (AscendersBane). No HP effects.
- `RunManager.InitializeNewRun()` -> `ApplyAscensionEffects(player)` -> `AscensionManager.ApplyEffectsTo(player)` does not touch HP.
- `Player.CreateForNewRun()` uses `character.StartingHp` for both current and max HP.
- No EZMB gameplay slice touches player HP during run start or Neow.

Hypotheses (in priority order):
1. Vanilla multiplayer `CreatureCmd.Heal` or `SetCurrentHpInternal` fails/skips for the non-host player when `RunState.AscensionLevel > 10`, possibly because `NetService.Type.IsMultiplayer()` bypasses some initialization path.
2. A multiplayer-specific runtime path prevents the v0.105.0 `AncientEventModel.BeforeEventStarted` / `CreatureCmd.Heal` flow from applying to the affected client, despite the refreshed source still showing the vanilla full-heal path.
3. Our `AscensionSelectionPatches` expand `maxMultiplayerAscensionUnlocked` during `UpdateMaxMultiplayerAscension` in a way that corrupts some lobby/player state before the run starts. Our patches do not touch `BeginRunForAllPlayers` directly (only log a warning).
4. A20 Dual King Brands warning patch or some other EZMB Harmony patch interferes with lobby cleanup or run setup in a non-obvious way.
5. The Neow event fails to start properly in multiplayer, so `BeforeEventStarted` never fires, and HP remains at whatever value was set during player creation (which should still be `StartingHp`).

Required evidence:
- Run with `EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS=1` in co-op to capture lobby state, player HP at run start, `BeginRunLocally` HP, `AfterActEntered` HP, and Neow `BeforeEventStarted` HP.
- Bisect via `EZMB_ASCENSION_DISABLE_ALL_SYSTEMS=1` to confirm whether EZMB gameplay slices are involved.
- Test with `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` + vanilla A10 as control.

### ISSUE-2026-05-08-MULTIPLAYER-SAVE-QUIT-NOT-PROPAGATING

Priority: P0/P1

Status: source-investigated; vanilla source path should propagate disconnect, but live co-op evidence is still pending

Area: multiplayer save-and-quit / disconnect / host-client sync

Player report: in co-op, when one player saves and quits, the other machine does not synchronously quit, disconnect, or return to menu.

Current v0.105.0 source notes:
- Pause-menu save-and-quit is handled by `NPauseMenu.OnSaveAndQuitButtonPressed()` / `CloseToMenu()`, which calls `NGame.ReturnToMainMenu()`.
- `NGame.ReturnToMainMenu()` fades out, loads common/main-menu assets, calls `RunManager.Instance.CleanUp()`, and loads the main menu.
- `NGame.Quit()` (source code/src/Core/Nodes/NGame.cs) saves settings/profile data and calls `GetTree().Quit()` but does not send a disconnect message to remote peers.
- `RunManager.CleanUp(bool graceful = true)` disposes run synchronizers and calls `NetService.Disconnect(NetError.Quit, !graceful)`.
- `NetHostGameService.Disconnect(...)` calls the active transport's `StopHost(...)`.
- `SteamHost.StopHost(...)` closes every client connection with the quit reason, leaves the Steam lobby, and then reports local disconnection.
- `ENetHost.StopHost(...)` sends an ENet disconnection packet to each client when not immediate, then disconnects each peer and reports local disconnection.
- `RunLobby.OnDisconnected(...)` calls `RunManager.LocalPlayerDisconnected(...)`; for non-`QuitGameOver` reasons during an active run, `RunManager.LocalPlayerDisconnected(...)` queues `ReturnToMainMenuWithError(...)`.
- `NErrorPopup.Create(...)` suppresses a popup only for self-initiated `Quit`; remote peer disconnects should still have a non-self-initiated reason.
- Current EZMB Ascension patches do not patch `NPauseMenu`, `RunManager.CleanUp`, `RunLobby.OnDisconnected`, `NetHostGameService`, `NetClientGameService`, `SteamHost`, or `ENetHost`.

Required investigation:
- Live two-client logs still need to confirm whether the remote peer receives `NetError.Quit`, whether `RunLobby.OnDisconnected(...)` fires, and whether `ReturnToMainMenuWithError(...)` completes.
- Run co-op with `EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS=1` and collect both host/client logs around Save & Quit.
- If the remote peer never receives the disconnect, the defect is likely transport/session-state or vanilla runtime behavior rather than an EZMB save/quit patch.
- If the remote peer receives the disconnect but does not return to menu, inspect `RunManager.LocalPlayerDisconnected(...)` and active UI state at that moment.
- Do not add a speculative EZMB multiplayer save/quit fix without live evidence identifying which branch fails.

### ISSUE-2026-05-08-MULTIPLAYER-RUN-START-BLACK-SCREEN

Priority: P0/P1

Status: dependency errors removed; still investigating until fresh host/client live logs prove whether the black screen is tied to HP0/Neow, A20 startup, transport sync, or another runtime exception.

Area: multiplayer run start / screen transition / mod load / dependency compatibility

Player report: multiplayer run start can still black-screen, even after the earlier `DoormakerBoss` TypeLoadException fix.

Current status:
- `ISSUE-2026-05-08-MULTIPLAYER-A20-BLACK-SCREEN-OPTIONAL-BOSS-TYPELOAD` was fixed by making `BossSealCatalog` use runtime-safe `ModelId` strings. This fixed the TypeLoadException for `DoormakerBoss`.
- But the player report suggests black screen can still occur, potentially from other causes.

Hypotheses:
1. HP 0/80 -> Neow blocked -> screen transition never completes (same root cause as HP0-Neow issue).
2. A different TypeLoadException or missing model for a different v0.105.0 API.
3. Network desync during run start - host reaches Act 0 but client never receives the transition.
4. Missing localization or model that causes a silent failure during lobby cleanup or run scene setup.

Required evidence:
- Collect host AND client `godot.log` covering the 200 lines before and after run start.
- Look for exceptions, missing models, missing localization, network disconnect, desync, or timeout.
- If black screen follows from HP0/Neow blocked, fix that root cause first.
- If independent, add separate `EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS` entries for screen transition sync.

### ISSUE-2026-05-08-MULTIPLAYER-A20-BLACK-SCREEN-OPTIONAL-BOSS-TYPELOAD

Priority: P0

Status: source-patched and published locally; live co-op retest pending

Area: A20 multiplayer run start / A19 Boss Royal Seal catalog / Early Access API compatibility

Player report: starting a multiplayer A20 run can black-screen after the lobby begins the run.

Observed log evidence:

- Latest `godot.log` shows host multiplayer A20 run start reached `NGame.StartNewMultiplayerRun(...)` with Ascension 20.
- Act 1 map generation applied A11/A12/A16 metadata, then failed in `AscensionMapService.MarkBossSeals(...)`.
- Fatal mod stack: `System.TypeLoadException: Could not load type 'MegaCrit.Sts2.Core.Models.Encounters.DoormakerBoss'` from `BossSealCatalog..cctor()`.
- The same local log also contains unrelated local-mod/BaseLib compatibility errors, but the A20 run-start abort is the `DoormakerBoss` type-load failure in EZ Micro Balance.

Root cause:

- Earlier source/API evidence proved optional Early Access boss and power types are not safe to reference directly; the refreshed v0.105.0 source does not expose the previously crashing `DoormakerBoss` type.
- `BossSealCatalog` used hard generic references like `ModelDb.GetId<DoormakerBoss>()`; static initialization therefore crashed before the run could finish generating the first map.
- Current build also proved adjacent API drift: direct `Doormaker` / `HungerPower` / `ScrutinyPower` / `GraspPower` references and direct `PumpkinCandle.ActiveAct` access are not safe against the installed DLL.

- Earlier source fix:

- `BossSealCatalog` previously used runtime-safe `ModelId` strings such as `ENCOUNTER.DOORMAKER_BOSS` instead of hard references to optional boss encounter classes.
- Door Wedge combat checks previously used runtime `ModelId` checks for the Doormaker monster and phase powers, so missing optional types did not block compile/load.
- v0.105.0 source later replaced the active Doormaker/Door Wedge scope with `AEONGLASS_BOSS`; current active EZMB source has no Door Wedge implementation and applies the temporary Aeonglass +5 Strength seal instead.
- Debt patching was adjusted to avoid direct compile/accessibility assumptions that broke against the current installed game API.
- Pumpkin Candle EZMB patching was removed; vanilla Pumpkin Candle behavior is restored for the v0.105.0 package, so no Pumpkin-only Harmony target participates in `PatchAll()`.
- Added source guard tests to prevent reintroducing hard optional `DoormakerBoss` / `Doormaker` type references in the Boss Seal startup path.

Manual retest:

- Republish or confirm the installed `EZMicroBalance.dll` timestamp is newer than this fix.
- Host multiplayer with BaseLib and EZ Micro Balance only if possible.
- Select A20, let the client join, ready both players, and start the run.
- Confirm the run leaves the lobby and reaches the Act 1 map instead of black-screening.
- Inspect `godot.log` for no `EZMicroBalance` `TypeLoadException`, especially no `DoormakerBoss`, `Doormaker`, `HungerPower`, `ScrutinyPower`, or `GraspPower` load errors.
- Keep A20 Dual King Brands co-op gameplay verification pending; this fix is a crash/compatibility fix, not a full live co-op balance pass.

### ISSUE-2026-05-08-ASCENSION-PUBLIC-SELECTION-DEFAULT-ON-FOR-MP-TEST

Priority: P0

Status: source-patched; package/smoke refreshed; Steam-client/live co-op pending

Area: A11-A20 selector gate / multiplayer pre-release testing

Decision: A11-A20 selection is now default-on in this private-beta multiplayer test candidate so testers can immediately exercise single-player and host-multiplayer A11-A20 through the original lobby UI.

Required behavior:

- Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison.
- Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection while leaving single-player A11-A20 available.
- `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.
- A20 multiplayer selection is not full A20 co-op support. Dual King Brands / second-boss Brand gameplay remains disabled or downgraded in co-op pending live verification.
- Normal Steam-client Mod Settings has separate RC1 evidence; controlled smoke passed is not the same as live co-op verification.

Manual retest:

- With no Ascension env vars, confirm single-player and host multiplayer can select A11-A20.
- With `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1`, confirm single-player and multiplayer selection return to vanilla A1-A10.
- With `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`, confirm single-player A11-A20 remains available and host-multiplayer selection returns to the vanilla cap.
- Confirm host-only multiplayer A20 selection logs the downgrade warning before any client joins, then logs again on run start after a client joins.
- Keep live gameplay, save/load, and live co-op/desync verification pending until actually executed or explicitly accepted.

### ISSUE-2026-05-07-A11-MAP-LENGTH-NOT-PLAYER-VISIBLE

Priority: P1

Status: source-patched again; Act 1 normal Steam-client map/save-load spot check passed; Act 2/3 normal Steam-client map-surface observation passed; broader natural traversal and boss-reachability verification pending

Area: A11 Wide Tower, Long Road / map generation

Player report: A11 still looks like the original map size. It was longer once, then regressed.

Current source fix:

- `AscensionFeatureGate` now sets A11 rows to Act 1 `+1`, Act 2 `+1`, Act 3 `+2`.
- `AscensionMapService` now accepts old width-only adjusted maps and inserts missing late rows instead of returning early.
- A11 still expands from 7 to 8 columns and inserts a reachable optional route node.
- A11 no longer marks ordinary route nodes with a dedicated long-road marker or hover explanation; map growth is represented only by vanilla-looking rows, columns, nodes, and paths.

RC1 live evidence:

- Normal Steam-client BaseLib+EZMB-only run selected A11 through the original single-player Ascension arrows (`.tools\runtime-evidence\rc1-a11-map-save-20260508-110008\08-character-select-a11.png`).
- The Act 1 map screenshot (`11-a11-act1-map-after-neow-continue.png`) renders the widened map with normal route nodes.
- `a11-map-save-load-godot-live.log` records `Ascension A11 applied ... inserted 1 late route row(s); actIndex=0; columns=8; rows=17`.
- `a11-save-map-dimensions.json` records `MapHeight: 17`, `BossRow: 17`, `RouteRowCount: 16`, `ColumnCount: 8`, and columns `0,1,2,3,4,5,6,7`.
- After selecting the first monster node, the game wrote `current_run.save`; Save & Quit -> Continue loaded back into the A11 combat, and the map reopened with `columns=8; rows=17`.
- A later normal Steam-client BaseLib+EZMB-only run selected A11 through the original UI and used DevConsole `act 2` / `act 3` only to observe later-act map surfaces. Evidence directory: `.tools\runtime-evidence\rc1-a11-act23-map-20260508-113355`.
- `a11-act23-godot-live.log` records Act 2 `Ascension A11 applied ... inserted 1 late route row(s); actIndex=1; columns=8; rows=16` and Act 3 `Ascension A11 applied ... inserted 2 late route row(s); actIndex=2; columns=8; rows=16`, with 0 `ERROR` lines and 0 release-blocking signatures.
- Act 2 screenshot `25-a11-act2-map-clean.png` and Act 3 screenshot `27-a11-act3-map-clean.png` render normal route nodes without an A11-specific marker or hover tooltip.

Manual retest:

- Act 1 fresh A11 route-width/row/save-load spot check is complete for RC1 evidence above.
- Act 2/3 width/row/no-marker observation is complete for RC1 evidence above, using DevConsole act jumps rather than natural traversal.
- Confirm the map still has a low-risk route and boss reachability through natural route traversal.
- Future traversal helper: `win` may be used to end combats after clicking naturally reachable map nodes; do not use DevConsole `travel` as proof of reachability, because local source shows it enables jumping to any map room.

### ISSUE-2026-05-07-A11-MAP-CHANGE-ANIMATION

Priority: P3

Status: controlled-smoke refreshed; Act 1/2/3 normal Steam-client A11 map surfaces observed; bespoke animation/A17 UI feedback still pending

Area: A11/A17 map UI feedback

Player report: map and visibility changes should not feel random; the player should clearly see that something changed.

Current mitigation:

- A17 deep-branch nodes still have map hover tips.
- A11 long-road node tips were removed after player feedback; A11 now relies on normal map geometry instead of a special visible marker.

Remaining work:

- No bespoke map-generation animation or transition sequence has been implemented yet.
- Live UI pass should decide whether hover tips are enough or whether a short map pulse/overlay is needed.

### ISSUE-2026-05-07-A12-TOOLTIP-RICHTEXT-COLORS

Priority: P2

Status: source-patched; live tooltip/rich-text verification pending

Area: A12 Firemark / Forge Token / Banner tooltip text

Player report: A12 text works mechanically, but numbers should be blue and important words such as upgrade, Gold, Skill card, Firemark, Forge Token, Rest, and Smith should be gold.

Current source fix:

- `ForgeTokenRelic` English/ZHS relic text and rest-site extra text now use `[blue]` for values and `[gold]` for important terms.
- Firemark power tooltips now color values and core terms.
- Banner room power/localization strings now color values and core terms.
- Ascension panel localization for A12/A13/Banners now uses the same markup.

Manual retest:

- Hover Forge Token, Firemark powers, Banner powers, A12/A13 ascension rows, and rest-site Forge Token extra text.
- Confirm rich text renders instead of showing raw tags.
- Confirm Chinese text wraps cleanly.

### ISSUE-2026-05-07-A13-FISSION-TOO-RARE-AT-HIGH-ASCENSION

Priority: P2

Status: source-patched; live A13/A16 reward-frequency verification pending

Area: A13 Fission Enchantment / A16 inherited ascension behavior

Player report: A16 should include earlier ascension effects, but Fission nearly disappeared while testing A16.

Current source evidence:

- `AscensionFeatureGate.IsLevelEnabled(...)` uses `runState.AscensionLevel >= requiredAscensionLevel`, so A16 includes A13 when the public/debug gate is active.
- Fission source chances were raised from `10/15/20/5` to `25/35/40/15` for normal combat / Banner Room / Firemarked Elite / Boss rewards.

Manual retest:

- Test A16 with public/debug ascension enabled.
- Check repeated normal combat rewards and Banner Room rewards.
- Confirm Fission remains limited to eligible Attack/Skill cards and still appears at most once per reward screen.

### ISSUE-2026-05-07-ROOTBUD-ROOTBLIGHT-REWORK

Priority: P1

Status: source-patched; English/Simplified Chinese hover/text and the A14 Neow starter add notice have live spot-check evidence; generated portrait art is packaged; full Rootblight/Blight Sprout behavior, combat-end notices, co-op ownership/desync, and generated-art visual verification remain pending

Area: A14/A15/A18 Rootblight and Blight Sprout

Player report: the old Root Bud / Rootblight wording was conceptually unclear, Boss Sprout count was too low, and Boss/Elite Sprout text was too long.

Current source fix:

- ZHS player-facing term is now `根芽`.
- Boss fights in Acts 2/3 now seed 2 Blight Sprout cards.
- Blight Sprout text is shortened: play to Exhaust; Boss sprouts use rounds 3/4 and elite sprouts use round 3; if seen and not played, add Rootblight I after combat.
- Rootblight I/II/III costs are 2/3/4.
- Played Rootblight removes its master-deck card and queues the downgrade card after combat.
- Unplayed Rootblight I/II upgrades after combat; ignored Rootblight III stays III and adds one Rootblight I only once per card.
- Rootblight is capped at 4 cards, and cap hits show `Root system full.` / `根系已满。`.
- Rest removes exactly one highest-stage Rootblight instead of clearing all Rootblight.

Manual retest:

- A14 new run starts with Rootblight I.
- A15 Act 2/3 Boss fights bury 2 Blight Sprouts.
- A18 eligible Act 2/3 Elite fights bury 1 Blight Sprout.
- Seen-but-unplayed Blight Sprout adds one Rootblight I after combat.
- Rootblight I/II/III play and post-combat behavior matches the new card text.

### ISSUE-2026-05-07-MULTIPLAYER-A11-A20-SELECTION-BLOCKED

Priority: P1

Status: source-patched; live co-op verification pending

Area: A11-A20 Ascension selection / multiplayer lobby

Player report: A11-A20 cannot be used in multiplayer, but co-op should eventually support the same expanded Ascension range instead of being single-player only.

Desired behavior:

- A11-A20 selection is available in multiplayer by default for this private-beta multiplayer test candidate.
- `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` restores vanilla A1-A10 selection for comparison.
- `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` disables only host-multiplayer A11-A20 selection.
- `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.
- Multiplayer selection must not patch or corrupt vanilla A1-A10 progress.
- Earlier Ascension effects still inherit normally at higher levels.
- Per-player systems such as Rootblight and Blight Sprout remain independent and do not desync.
- A21-A30 remains out of scope.

Implementation notes:

- Local source inspection found that `StartRunLobby.UpdateMaxMultiplayerAscension()` computes the multiplayer cap from each `LobbyPlayer.maxMultiplayerAscensionUnlocked`, while `UpdatePreferredAscension()` writes host selections to `PreferredMultiplayerAscension`.
- Current source patch expands host multiplayer lobbies by default unless `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` or `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` is set. It temporarily raises in-memory lobby unlock caps only during max recomputation, restores them in a finalizer, and skips A11-A20 preferred-progress writes.
- Host-multiplayer A11-A20 selection is independently disableable with `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`.
- A11-A20 gameplay, per-player Rootblight/Blight Sprout ownership, and desync behavior still require live co-op verification.
- A20 Dual King Brands gameplay is still single-player gated through `IsDualKingBrandsSinglePlayerEnabled(...)`; the host multiplayer selector/start path now logs a development-testing downgrade warning, but live co-op verification is still pending.

Manual retest:

- Host a multiplayer lobby with BaseLib and EZ Micro Balance enabled.
- Confirm A11-A20 selection is available by default with no Ascension env var.
- Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` and confirm A1-A10 behavior is restored for comparison.
- Clear the disable variable and confirm the lobby can select A11-A20 again.
- Start a co-op run at A11/A12/A14/A16/A20 and confirm all players load without desync.
- Confirm Rootblight/Blight Sprout ownership remains per-player in co-op.
- Confirm A20 multiplayer selection does not imply that Dual King Brands gameplay is live co-op verified.

### ISSUE-2026-05-07-A20-MULTIPLAYER-SELECTION-WARNING-MISSING

Priority: P2

Status: source-patched with log warning; live co-op verification pending

Area: A20 Dual King Brands / multiplayer selector messaging

Audit finding: host multiplayer can source-select A20 when the public development gate is enabled, but A20 Dual King Brands gameplay remains single-player gated by `AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(...)`.

Desired behavior:

- Multiplayer A20 selection must not make testers think A20 Dual King Brands is fully supported in co-op.
- Add a clear runtime log, UI warning, or selector-side message before multiplayer A20 testing.
- Keep A20 gameplay conservative until live co-op boss-path verification proves host/client behavior is safe.

Planning notes:

- Do not remove the current A20 single-player gameplay gate without local source evidence and live co-op test coverage.
- Keep selection support, gameplay activation, progress writes, and live co-op verification documented as separate surfaces.
- `AscensionSelectionPatches.WarnIfA20MultiplayerDowngraded(...)` now logs on host multiplayer A20 selection and host multiplayer A20 run start, including the host-only lobby case before a client joins.
- Warning text says multiplayer A20 selection is for development testing, Dual King Brands / second-boss Brand gameplay is disabled or downgraded in co-op pending live verification, and A11-A19 inherited systems may still apply if their gates are enabled.

Manual retest:

- In a host multiplayer lobby with no Ascension env vars, select A20 before any client joins.
- Confirm the tester-visible warning or log appears on host-only selection.
- Let a client join without changing Ascension, then start the A20 run.
- Confirm the tester-visible warning or log appears on selection and run start.
- Confirm the run does not silently apply single-player-only Dual King Brands behavior to co-op.

### ISSUE-2026-05-07-LIVE-COOP-A11-A20-MATRIX-PENDING

Priority: P1

Status: source-patched; live co-op matrix pending

Area: A11-A20 multiplayer runtime verification

Audit finding: source guards prove selector and ownership shapes, but no live co-op matrix has verified lobby join, client view, run start, save/load, per-player state, or desync behavior.

Minimum matrix:

- Gate default-on: with no Ascension env vars, host can select A11-A20 and client sees the selected value.
- Gate off: `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` restores vanilla A1-A10 selection.
- Disable flag: `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` restores vanilla multiplayer cap.
- A11: co-op run starts with widened/longer map and no A11 marker.
- A12: Firemarked Elite route markers remain visible and host/client agree.
- A14/A15/A18: Rootblight and Blight Sprout state remains player-owned.
- A16: Banner Room markers and combat rules remain visible and synchronized.
- A20: selection limitation or warning is visible; Dual King Brands remains treated as not live co-op verified.
- Logs: no ownership warnings, checksum divergence, or multiplayer desync lines in `godot.log`.

## Resolved / Player-Verified

### ISSUE-2026-05-07-A11-LONG-ROAD-MAP-MARKER-UNWANTED

Priority: P2

Status: resolved for A11 no-marker behavior on 2026-05-08; inherited marker regression checks remain tracked by the relevant A12/A16/A17/A19/A20 live-verification items and the open blocker audit.

Area: A11 Wide Tower, Long Road / map UI

Player report: A11 should make the map longer/wider through normal map geometry only. It should not put a special visible marker or hover tooltip on the map just to explain the extra route space.

Desired behavior:

- Remove the dedicated A11 long-road map marker/hover indicator from newly inserted route nodes.
- A11 map changes should look like vanilla map rows and paths, not like a special event or quest node.
- Keep the actual map-length/width tuning separately testable; if final tuning is only one added row, update docs/localization/changelog to avoid claiming larger route growth.

Implementation notes:

- `LongRoad` metadata, `MarkLongRoad`, and `LONG_ROAD_NODE` localization were removed from active source/resources.
- `AscensionMapQuestMarker` remains in use only for A17 Deep Branch generic markers; A12 Firemark, A16 Banner, A17 Deep Branch, A19 Seal, and A20 Brand indicators remain on their own paths.
- RC1 Act 1 screenshot `.tools\runtime-evidence\rc1-a11-map-save-20260508-110008\11-a11-act1-map-after-neow-continue.png` shows ordinary A11 route nodes without a dedicated A11 marker, icon, or hover tooltip. The after-load map screenshot `16-map-open-after-load-attempt.png` shows the same no-marker surface after Continue.
- RC1 Act 2/3 screenshots `.tools\runtime-evidence\rc1-a11-act23-map-20260508-113355\25-a11-act2-map-clean.png` and `27-a11-act3-map-clean.png` show ordinary later-act route nodes without a dedicated A11 marker, icon, or hover tooltip.

Manual retest:

- Act 1/2/3 A11 no-marker map-surface spot checks are complete for RC1 evidence above.
- Natural A11 route traversal remains tracked by `ISSUE-2026-05-07-A11-MAP-LENGTH-NOT-PLAYER-VISIBLE`.
- Firemark/Banner/Deep Branch/Boss Seal indicator checks remain tracked by their relevant Ascension live-verification items.


### ISSUE-2026-05-08-V105-BASELIB-CREATURE-SHOWSINFINITEHP-API-DRIFT

Priority: P1 environment/runtime verification

Status: resolved for the BaseLib dependency/API-drift gate on 2026-05-08; remaining multiplayer run-start/Neow/save-quit evidence is tracked by the separate P0 co-op issues in the Open section. BaseLib `v3.1.2`, a clean BaseLib+EZMB-only controlled smoke, clean normal Steam-client startup/Mod Settings log snapshots, and Codex-observed normal-Steam single-player combat smoke for A0/A10/A20 via DevConsole `fight CULTISTS_NORMAL` supersede the earlier failure. User also reports single-player A0/A10/A20 and boss/basic combats pass after the BaseLib update. EZ Micro Balance's dedicated Mod Settings page/display is now covered by the 2026-05-08 `095137` normal Steam-client recheck after adding the no-op BaseLib config page.

Area: v0.105.0 API drift / BaseLib compatibility / mod environment hygiene

Evidence from `godot2026-05-08T05.06.30.log` (v0.105.0, 2026.05.08):

1. **Test environment loaded 17 mods, not only BaseLib + EZMicroBalance:**
   - `Loaded 17 mods (19 total)`
   - Loaded `DamageMeter`, `RouteSuggest`, `AnimeWaifuSilent`, `ModConfig`, `QuickLink`, `SpeedX`, `The-Watcher`, and others.
   - This violates the release test prerequisite: only BaseLib + EZMicroBalance enabled.

2. **Superseded BaseLib v3.1.0 failure evidence:**
   - `Undefined target method for patch method ... ExhaustivePatch`
   - `Undefined target method for patch method ... PersistPatch`
   - `Undefined target method for patch method ... PurgePatch`
   - `[BaseLib] Applied 150 patches successfully, 3 failed`

3. **`Creature.get_ShowsInfiniteHp()` is missing in v0.105.0:**
   - `System.MissingMethodException: Method not found: 'Boolean MegaCrit.Sts2.Core.Entities.Creatures.Creature.get_ShowsInfiniteHp()'`
   - Called from `BaseLib.Patches.UI.HealthBarForecastPatch.RefreshForegroundOverlay(NHealthBar healthBar)`
   - Also called from `DamageMeter.Scripts.CombatDataCollector.SnapshotEnemyHp(CombatState combatState)`
   - Stack reaches `CrackedCore.BeforeSideTurnStart` and `CombatManager.StartCombatInternal()`

4. **Direct gameplay impact:**
   - The `MissingMethodException` in the combat-start/turn-start hook chain interrupts normal combat initialization.
   - Observed: singleplayer Defect A20 enters combat but does not draw cards, energy stuck at 0/3. Combat does not enter a normal player turn.
   - This is NOT an EZMB logic bug; it is a dependency/environment compatibility blocker.

Required resolution (before any EZMB fix or release claim):
- [x] Disabled/isolated all mods except BaseLib + EZMicroBalance for the RC1 normal Steam-client startup log; the moved local mod entries were restored afterward.
- [x] Updated BaseLib runtime/project package to `v3.1.2`; current controlled BaseLib+EZMB-only smoke has no `Creature.get_ShowsInfiniteHp`, BaseLib patch-failure, or DamageMeter removed-API signatures.
- [x] RC1 normal Steam-client startup log snapshot is clean for the release gate signatures. Codex temporarily isolated non-BaseLib/EZMB local mod entries, launched through Steam, reached main menu, saved `.tools\runtime-evidence\rc1-normal-steam-clean-godot-20260508-090122.log`, restored 23 moved mod entries and `settings.save`, and confirmed `Loaded 2 mods (2 total)`, BaseLib `177 patches successfully, 0 failed`, EZMB initialization, 0 `ERROR` lines in the startup snapshot, and 0 removed-API/EZMB exception signatures.
- [x] RC1 normal Steam-client Mod Settings UI recheck opened `模组配置`: BaseLib appeared and was enabled; EZ Micro Balance appeared as the localized page `微平衡` with `无可配置选项。`; main-menu/log evidence showed only `BaseLib, EZ Micro Balance` loaded. Snapshot `.tools\runtime-evidence\rc1-normal-steam-modsettings-page-godot-20260508-095137.log` has `Registered config for mod EZMicroBalance`, `Loaded 2 mods (2 total)`, 0 `ERROR` lines, and 0 release-blocking signatures.
- [x] Confirm singleplayer A0 combat draws cards and gains energy normally. Evidence: normal Steam-client BaseLib+EZMB-only DevConsole combat smoke `a0-debug-fight-clean.png` shows 80/80 HP, 3/3 energy, five-card hand, enemies, HP bars, and intents; natural route-click first-node path remains unrun if stricter coverage is required.
- [x] Confirm singleplayer A10 combat draws cards and gains energy normally. Evidence: normal Steam-client BaseLib+EZMB-only DevConsole combat smoke `a10-first-combat-clean.png` shows 64/80 HP, 3/3 energy, five-card hand, enemies, HP bars, and intents; natural route-click first-node path remains unrun if stricter coverage is required.
- [x] Confirm singleplayer A20 combat draws cards and gains energy normally. Evidence: normal Steam-client BaseLib+EZMB-only DevConsole combat smoke `a20-debug-fight-clean.png` shows 64/80 HP, 3/3 energy, five-card hand, Rootblight present, enemies, HP bars, and intents; natural route-click first-node path remains unrun if stricter coverage is required.
- User-reported on 2026-05-08: single-player A0/A10/A20 plus boss/basic combats pass after the BaseLib update. This now complements the Codex-observed combat-smoke evidence.
- [x] Normal Steam-client startup snapshot has no `Creature.get_ShowsInfiniteHp`.
- [x] Normal Steam-client startup snapshot has no BaseLib patch failures.
- [x] Normal Steam-client startup snapshot has no DamageMeter or other non-EZMB mod exceptions.
- Combat-smoke log caveat: the A0/A10/A20 debug-fight logs have 0 removed-API signatures, 0 BaseLib patch failures, 0 `TypeLoadException`, 0 `MissingMethodException`, and 0 EZMB error/exception pattern hits. They are not clean-log gate snapshots because automated test-run abandonment/window closing produced Godot exit resource-leak `ERROR` lines, and A20/A0 include a temporary save-backup delete `ERROR` from the save restoration flow. The clean-log gate remains the earlier isolated startup and Mod Settings snapshots.
- Multiplayer A11-A20 testing may resume, but co-op run-start/Neow/save-quit evidence remains required.


### ISSUE-2026-05-07-HANDOFF-GIT-STATUS-HYGIENE

Priority: P3

Status: resolved for the 2026-05-08 RC1 hygiene refresh; final handoff still must re-run git status/log after any later edits

Area: release handoff / repository status docs

Audit finding: handoff and audit docs can become stale when they say "No commit or push has been made" or "worktree dirty." Final release handoff must re-check the current status rather than relying on old wording from an earlier local snapshot.

Resolution evidence:

- `docs/private-beta-verification-handoff.md`, `docs/features/ancients-rework-v4/completion-audit.md`, and `docs/rc1-live-validation-log.md` record the current point-in-time `git log -1 --oneline --decorate`: `96bfa50 (HEAD -> main, origin/main, origin/HEAD) fix try 10`.
- Those docs record the branch as aligned with `origin/main` while the working tree remains dirty with modified files, deleted moved originals, and untracked new patch/doc/archive files.
- The handoff no longer uses stale no-commit wording for the current branch state, and it explicitly says not to describe the checkout as fully pushed until pending edits are reviewed, committed, and pushed.
- The handoff and audit docs require rerunning `git status --short --branch` and `git log -1 --oneline --decorate` before final release packaging or handoff.

### ISSUE-2026-05-07-RELEASE-ARTIFACT-TESTS-DEPEND-ON-IGNORED-PUBLISH-OUTPUT

Priority: P2

Status: resolved on 2026-05-08; normal tests pass without ignored publish artifacts, release artifact tests stay opt-in

Area: automated tests / release artifact validation

Audit finding: `.gitignore` excludes `publish/`, `*.zip`, `*.dll`, and `*.pck`, while some release guard tests require installed/staging/versioned zip artifacts. This is useful for release validation on the maintainer machine, but it can make normal `dotnet test` brittle in a clean clone unless package generation ran first.

Resolution evidence:

- Normal `dotnet test` no longer requires ignored publish/package artifacts because package/hash/runtime-smoke checks are marked with `ReleaseArtifactFactAttribute`.
- Normal package/hash/runtime-smoke checks are marked with `ReleaseArtifactFactAttribute` and skip unless `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` is set.
- Release artifact tests remain strict when opted in.
- On 2026-05-08, Codex temporarily moved the ignored `publish/` directory aside, ran `dotnet test EZMicroBalance.sln`, observed 65 passed / 16 skipped / 0 failed, and restored `publish/`.
- The refreshed docs describe the package refresh and opt-in command order.

### ISSUE-2026-05-07-CURRENT-PACKAGE-RUNTIME-SMOKE-STALE

Priority: P1

Status: resolved for loader/runtime-smoke freshness on 2026-05-08; live gameplay remains tracked by separate open issues

Area: controlled runtime smoke / SavedSpireField registration

Audit finding: several docs cited a prior controlled `--force-steam off` smoke with an obsolete SavedSpireFields count. The current source/package defines 12 SavedSpireFields after Rootblight v2.2 card-state fields.

Resolution evidence:

- Current package was published and package staging/versioned/zip artifacts were refreshed.
- Controlled `--force-steam off` smoke passed after publish/package refresh.
- Temporary profile settings enabled only `BaseLib` and `EZMicroBalance`, explicitly disabled other discovered local mods, and restored `settings.save` plus `settings.save.backup` byte-for-byte.
- `godot.log` showed `Loaded 2 mods (19 total)`, BaseLib initialization, EZ Micro Balance DLL/PCK load/init, `Found 12 SavedSpireFields`, default-on Ascension initializer wording with 0 old `Default-off gate` lines, main menu in `13,628ms`, 0 EZ Micro Balance error/exception lines, and no `Creature.get_ShowsInfiniteHp`, BaseLib patch-failure, or DamageMeter removed-API signatures.
- Later normal Steam-client isolated startup and Mod Settings snapshots also loaded only BaseLib + EZ Micro Balance with `Loaded 2 mods (2 total)`, `Found 12 SavedSpireFields`, 0 `ERROR` lines, and the localized EZ Micro Balance config page visible. Live gameplay verification is still open elsewhere.

### ISSUE-2026-05-07-A12-FORGE-TOKEN-RESTSITE-CRASH

Priority: P1

Status: player reported fixed on 2026-05-07; keep in regression list

Area: A12 Forge Token / rest-site transition

Player verification: carrying Forge Token into a rest site no longer crashes.

Regression retest:

- Enter a rest site while holding Forge Token.
- Test Rest payout and Smith payout separately.
- Confirm token relic is removed after payout.
