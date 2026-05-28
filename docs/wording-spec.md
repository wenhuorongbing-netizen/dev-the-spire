# Spire Plus 文案与术语规范 (Wording & Terminology Spec)

## 目标

让 Spire Plus 的所有玩家面向文案（中/英）达到以下标准：
1. **术语与官方一致** — 中文使用《杀戮尖塔 2》官方简中本地化的精确用词
2. **简洁专业** — 像官方文案一样精炼，不说废话
3. **风格统一** — 同类内容用同一种句式和语气

---

## 1. 核心术语对照表

以下为已确认的官方术语，项目中必须统一使用：

| 英文 | 官方简中 | 项目曾用错误 | 出处 |
|------|---------|------------|------|
| Transform | 变化 | 变换 | settings_ui.json, content-data.js, app.js |
| Attack (card type) | 攻击 | — | — |
| Skill (card type) | 技能 | — | — |
| Power (card type) | 能力 | — | — |
| Status (card type) | 状态 | — | — |
| Curse (card type) | 诅咒 | — | — |
| Exhaust | 消耗 | — | — |
| Ethereal | 虚无 | — | — |
| Innate | 固有 | — | — |
| Retain | 保留 | — | — |
| Eternal | 永恒 | — | — |
| Block | 格挡 | — | — |
| Energy | 能量 | — | — |
| Draw Pile | 抽牌堆 | — | — |
| Discard Pile | 弃牌堆 | — | — |
| Exhaust Pile | 消耗牌堆 | — | — |
| Master Deck | 主牌组 | — | — |
| Max HP | 最大生命 | — | — |
| Gold | 金币 | — | — |
| Potion | 药水 | — | — |
| Relic | 遗物 | — | — |
| Elite | 精英 | — | — |
| Boss | 首领 | — | — |
| Act | 幕 | — | — |
| Rest Site | 休息处 | — | — |
| Ancient | 先古 | — | — |
| Ascension | 进阶 | — | — |
| Artifact | 人工制品 | — | — |
| Strength | 力量 | — | — |
| Dexterity | 敏捷 | — | — |
| Plating | 覆甲 | — | — |
| Regen | 再生 | — | — |
| Vigor | 活力 | — | — |
| Temporary | 临时 | — | — |
| Ethereal | 虚无 | — | — |
| Unplayable | 无法打出 | 不能被打出 | card_keywords.json |
| Sly | 奇巧 | — | — |

---

## 2. 已确认的错误与修复

### 2.1 术语错误

| 文件 | 错误 | 正确 | 说明 |
|------|------|------|------|
| `EZMicroBalance/localization/zhs/settings_ui.json` | 变换预览 | 变化预览 | 官方用"变化" |
| `website/content-data.js` 多处 | 变换 | 变化 | 官方用"变化" |
| `website/app.js` 多处 | 变换 | 变化 | 官方用"变化" |
| `EZMicroBalance/localization/eng/cards.json` | Quality Flame | Brilliant Flame | 英文名应与中文"至亮之焰"对应 |
| `EZMicroBalance/localization/eng/cards.json` | Avoid Debt | Shelter Contract | 与中文"避债契"对应 |

### 2.2 用词冗余

| 位置 | 问题 | 改进方向 |
|------|------|---------|
| website heroCopy | "把高进阶、先古奖励、原版遗物调整和预览工具整合到同一个 Mod 的私测扩展" | 精简为一句话 |
| mechanicGlossary 各条目 | bullets 过长，像技术文档而非游戏内说明 | 缩短至 1-2 句 |
| seedbed 描述 | "强度来自三段收益"等分析性语言 | 删除，只保留规则 |
| 苗床 keyword description | 太长，包含分析而非规则 | 精简至核心规则 |

### 2.3 风格不统一

| 问题 | 说明 |
|------|------|
| 描述有时用"你"有时省略主语 | 统一用"你"作为主语 |
| 中文描述句末有时有句号有时没有 | 统一加句号 |
| 英文 description 有的用 "When played" 有的用 "On play" | 统一用 "When played" |

---

## 3. 文案风格指南

### 3.1 卡牌描述（中/英）

**中文格式：**
- 第一行：打出条件或关键词（如有）
- 后续行：效果描述
- 使用官方术语，不创造新词
- 每个效果用换行分隔
- 数值用 [blue]N[/blue]，关键词用 [gold]X[/gold]

**英文格式：**
- 同上，但使用英文官方术语
- 句式与官方卡牌描述一致

**范例（中文）：**
```
[gold]消耗[/gold]。
获得{Energy:energyIcons()}。
抽{Cards:diff()}张牌。
失去{MaxHp:diff()}点最大生命。
```

**范例（英文）：**
```
[gold]Exhaust[/gold].
Gain {Energy:energyIcons()}.
Draw {Cards:diff()} cards.
Lose {MaxHp:diff()} Max HP.
```

### 3.2 遗物描述（中/英）

**格式：**
- 第一句说明核心效果
- "拾取时"效果单独一句
- 不重复说明已知规则
- flavor 文本保持诗意但不冗长

### 3.3 先古描述（中/英）

**格式：**
- 标题：角色名·称号
- epigraph：一句话诗意描述
- 选项描述：说明效果，不说分析性语言
- 叙事对话：简短、有性格

### 3.4 网站文案

**原则：**
- heroCopy：一句话说清 Mod 是什么
- summary：每条不超过 2 句
- glossary bullets：每条 1 句，只说规则不说分析
- 避免"玩家应该知道"之类的元叙述

---

## 4. 修复清单

### Phase 1: 术语修正（已完成）
- [x] settings_ui.json: 变换 → 变化
- [x] content-data.js: 所有"变换" → "变化"
- [x] app.js: 所有"变换" → "变化"
- [x] eng/cards.json: Quality Flame → Brilliant Flame
- [x] eng/cards.json: Avoid Debt → Shelter Contract
- [x] 测试文件同步更新
- [x] 文档同步更新

### Phase 2: 精简描述（已完成）
- [x] 精简 website heroCopy
- [x] 精简 mechanicGlossary 各条目 bullets（中/英）
- [x] 精简 summary 条目
- [ ] 统一卡牌描述句式（待后续）

### Phase 3: 风格统一（待后续）
- [ ] 统一中文描述主语
- [ ] 统一句末标点
- [ ] 统一英文描述句式

---

## 5. 验证方法

1. 术语：grep 搜索项目中所有"变换"，确认已全部替换
2. 简洁度：每条 glossary bullet 不超过 40 个中文字
3. 一致性：同类描述使用相同句式模板
