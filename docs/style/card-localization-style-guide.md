# Card Localization Style Guide (Source-Format)

Project: EZ Micro Balance
Applies to: custom card text under `EZMicroBalance/localization/{eng,zhs}/cards.json` and matching `CardModel` source.

This guide is based on the local Slay the Spire 2 `v0.105.0` source and localization snapshot under `source code/`.

## Source Evidence

- Official card text highlights referenced card names and pile names with `[gold]...[/gold]`. Examples: `GRAVE_WARDEN.description` says `Add a [gold]Soul[/gold] into your [gold]Draw Pile[/gold].`; ZHS says `将一张[gold]灵魂[/gold]放入你的[gold]抽牌堆[/gold]中。`
- Official cards that reference generated cards use card hover tips. `GraveWarden`, `CaptureSpirit`, `Reave`, `Severance`, `Seance`, and `SoulStorm` override `ExtraHoverTips` and call `HoverTipFactory.FromCard<Soul>()`.
- `HoverTipFactory.FromCard<T>()` obtains the canonical card through `ModelDb.Card<T>()` and creates a `CardHoverTip`. Use this for card previews. Avoid `FromCardWithCardHoverTips<T>()` when cards reference each other, because it adds the referenced card's own hover tips too.
- `CardModel.HoverTips` already appends visible keyword hover tips for `CanonicalKeywords`, including `CardKeyword.Exhaust`. Descriptions must not manually duplicate a keyword that the card model already exposes.
- Dynamic values in official localization use dynamic vars such as `{Cards:diff()}`, `{Damage:diff()}`, `{Block:diff()}`, and `{Energy:energyIcons()}` when values can change.

Source-format conventions derived from official cards:

- Keep sentences short.
- Split long behavior into separate `\n` lines, as localizations do for combat-time and conditional text.
- Keep punctuation simple (`.` / `，` / `。`) and avoid compact but ambiguous phrasing.

## Rules

1. Visible keyword rule

If a card exposes Exhaust, Retain, Innate, Eternal, Sly, Ethereal, or another visible keyword through `CanonicalKeywords` or a source-proven equivalent, do not repeat the same keyword as manual body text.

This is the duplicate Exhaust prevention rule for cards like Rootblight and Blight Sprout.

Anti-pattern: `Play: Exhaust.` or `打出：消耗。` on a card whose `CanonicalKeywords` already includes `CardKeyword.Exhaust`.

2. Rich-text rule

Use `[gold]...[/gold]` for important card names, pile names, keywords used as in-line actions, and official named concepts when matching official examples.

Examples:

- English: `[gold]Rootblight I[/gold]`, `[gold]Draw Pile[/gold]`
- Simplified Chinese: `[gold]根蚀 I[/gold]`, `[gold]抽牌堆[/gold]`

3. Dynamic variable rule

Use dynamic vars when values can change through upgrade, enchantment, modifier, or balance tuning. Prefer `{Cards:diff()}`, `{Damage:diff()}`, `{Block:diff()}`, `{Energy:energyIcons()}`, `{Cards:plural:card|cards}`, and source-proven variants over hard-coded mutable values.

Hard-coded fixed values are acceptable only when the source constant is intentionally fixed and tests guard the text.

4. Preview rule

If text says "add a card", "becomes a card", "put a card into a pile", or the Simplified Chinese equivalent, add a safe card preview when source APIs support it. For EZMB custom cards, prefer:

```csharp
protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<SomeCard>()];
```

Preview cards must come from canonical `ModelDb` paths through `HoverTipFactory.FromCard<T>()`; they must not create runtime cards, mutate piles, consume RNG, write saves, or change multiplayer state.

When you add a preview card in text, ensure the source path is also guarded by a safe fallback if owner/run context is unavailable.

5. English and Simplified Chinese consistency rule

English and Simplified Chinese must describe the same behavior, counts, timing, and conditions. Do not over-compress Chinese text so far that play/unplayed outcomes or one-time limits become ambiguous.

6. Terminology rule

- Rootblight = 根蚀
- Blight Sprout / Root Bud = 根芽
- Draw Pile = 抽牌堆
- Discard Pile = 弃牌堆
- Exhaust Pile = 消耗牌堆
- Deck / master deck = 牌组 / 主牌组, using 主牌组 when distinguishing permanent deck cards from combat piles

7. Manual checklist rule

Each card text change must update the relevant manual checklist rows for hover preview, visible keyword count, rich-text rendering, raw-tag checks, and English/Simplified Chinese parity.
