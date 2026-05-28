# StS1 Events Localization

## Key Convention

All StS1 event localization keys use the prefix `STS1_` to distinguish from
native StS2 events. Keys follow the standard StS2 event localization pattern.

## Key Structure

```
STS1_{EVENT_NAME}.title                              — Event title
STS1_{EVENT_NAME}.pages.INITIAL.description           — Initial page text
STS1_{EVENT_NAME}.pages.INITIAL.options.{OPT}.title   — Option title
STS1_{EVENT_NAME}.pages.INITIAL.options.{OPT}.description — Option description
STS1_{EVENT_NAME}.pages.{PAGE}.description            — Result page text
```

## Dynamic Variables

Dynamic variables use StS2's `{variable_name}` syntax in localization text:
- `{HealAmount}` — heal amount
- `{MaxHpGain}` — max HP gain
- `{DamageAmount}` — damage amount
- `{GoldAmount}` — gold amount
- `{RelicName}` — relic name
- `{CardName}` — card name

## Files

| File | Language | Purpose |
|------|----------|---------|
| `EZMicroBalance/localization/eng/sts1_events.json` | English | Primary |
| `EZMicroBalance/localization/zhs/sts1_events.json` | Chinese | Translation |

## RitsuLib Slugify Rule

Event class names are slugified via `StringHelper.Slugify` for the entry key:
- `Sts1BigFish` → `STS1_BIG_FISH`
- `Sts1GoldenIdol` → `STS1_GOLDEN_IDOL`

The localization key uses this slugified form.

## Example (English)

```json
{
  "STS1_BIG_FISH.title": "Big Fish",
  "STS1_BIG_FISH.pages.INITIAL.description": "You come across a giant fish. It seems friendly.",
  "STS1_BIG_FISH.pages.INITIAL.options.BANANA.title": "Banana",
  "STS1_BIG_FISH.pages.INITIAL.options.BANANA.description": "Heal for 1/3 of your max HP.",
  "STS1_BIG_FISH.pages.INITIAL.options.DONUT.title": "Donut",
  "STS1_BIG_FISH.pages.INITIAL.options.DONUT.description": "Gain 5 max HP.",
  "STS1_BIG_FISH.pages.INITIAL.options.SHOE.title": "Shoe",
  "STS1_BIG_FISH.pages.INITIAL.options.SHOE.description": "Obtain 1 random relic. Obtain Regret.",
  "STS1_BIG_FISH.pages.BANANA.description": "You eat the banana. It's delicious!",
  "STS1_BIG_FISH.pages.DONUT.description": "You eat the donut. You feel stronger!",
  "STS1_BIG_FISH.pages.SHOE.description": "You take the shoe. Inside you find something useful... and something cursed."
}
```

## Example (Chinese)

```json
{
  "STS1_BIG_FISH.title": "大鱼",
  "STS1_BIG_FISH.pages.INITIAL.description": "你遇到了一条巨大的鱼。它看起来很友好。",
  "STS1_BIG_FISH.pages.INITIAL.options.BANANA.title": "香蕉",
  "STS1_BIG_FISH.pages.INITIAL.options.BANANA.description": "回复1/3最大生命值。",
  "STS1_BIG_FISH.pages.INITIAL.options.DONUT.title": "甜甜圈",
  "STS1_BIG_FISH.pages.INITIAL.options.DONUT.description": "获得5点最大生命值。",
  "STS1_BIG_FISH.pages.INITIAL.options.SHOE.title": "鞋子",
  "STS1_BIG_FISH.pages.INITIAL.options.SHOE.description": "获得1个随机遗物。获得悔恨诅咒。",
  "STS1_BIG_FISH.pages.BANANA.description": "你吃了香蕉。真好吃！",
  "STS1_BIG_FISH.pages.DONUT.description": "你吃了甜甜圈。你感觉更强壮了！",
  "STS1_BIG_FISH.pages.SHOE.description": "你拿走了鞋子。里面有些有用的东西……还有些被诅咒的东西。"
}
```
