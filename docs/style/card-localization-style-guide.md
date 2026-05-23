# Card Localization Style Guide (Source-Format)

Project: Spire Plus (`EZMicroBalance` manifest id)
Applies to: `EZMicroBalance/localization/{eng,zhs}/cards.json` and matching `CardModel` source.

This guide follows local Slay the Spire 2 `v0.106.0` source behavior.

## Source evidence

- `CardModel.HoverTips` via `HoverTipFactory.FromCard<T>()` is used for card previews.
- `CardModel.CanonicalKeywords` already supplies visible keyword UI, so description text must not duplicate the same visible keyword.
- Use `ModelDb` APIs for canonical model references and avoid direct canonical `AbstractModel` construction.
- Official localization patterns use `[gold]...[/gold]` for named concepts and `[blue]...[/blue]` for numeric values.
- Use dynamic variables when values can change by upgrade or runtime effects.

## Source format rules

- Keep descriptions short and split conditional lines with `\n`.
- Use one behavior per sentence where possible.
- Prefer plain punctuation (`.` / `;`) over full-width punctuation.

## 1) Visible keyword rule

- Do not repeat a keyword in description text when it is already exposed via `CardModel.CanonicalKeywords`.
- Applies to: Exhaust, Retain, Innate, Sly, Ethereal, Eternal, and similar visible flags.
- duplicate Exhaust wording in body text is a rule violation.

Anti-pattern:
- `Play: Exhaust.`
- `...` includes `Play: Exhaust` while `CardKeyword.Exhaust` is already active.

## 2) Rich text rule

- Use `[gold]...[/gold]` for:
  - important card names (Rootblight, Blight Sprout, etc.)
  - important pile names (`Draw Pile`, `Discard Pile`, `Exhaust Pile`)
  - key source-style named combat concepts when applicable
- Use `[blue]...[/blue]` for numeric values and variable values.

## 3) Dynamic variable rule

- Use dynamic vars when values can change via upgrade or tuning.
- Prefer dynamic placeholders over hard-coded literals when behavior is mutable.

## 4) Preview rule

- If text says "add", "becomes", or "put into pile", include `HoverTipFactory.FromCard<T>()` preview entries when API-safe.
- Preview cards must be state-free:
  - no runtime card creation
  - no save/data mutation
  - no RNG side effects

Preview source pattern:

```csharp
protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [HoverTipFactory.FromCard<SomeCard>()];
```

## 5) English/Simplified Chinese parity rule

- English and Simplified Chinese must describe identical behavior, timing, and limits.
- Keep played/unplayed and one-time cap behavior aligned across both locales.

## 6) Terminology rule

- Rootblight = 根蚀
- Blight Sprout / Root Bud = 根芽
- Withered Husk = 枯壳
- Root Eyes = 根眼
- Draw Pile = 抽牌堆
- Discard Pile = 弃牌堆
- Exhaust Pile = 消耗牌堆
- Deck = 牌组, master deck = 主牌组

## 7) Manual checklist rule

Every card text change should validate:
- hover-preview (eng + zhs)
- duplicate visible keyword checks
- rich-text rendering checks for `[gold]` / `[blue]`
- English/Simplified Chinese parity checks
