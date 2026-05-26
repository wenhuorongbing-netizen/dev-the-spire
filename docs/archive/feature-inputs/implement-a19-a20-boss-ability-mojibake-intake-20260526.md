下面是 **A19 / A20 首领专属能力 v4.2 终审修订版**。这一版重点修正你指出的几个问题：

A19 不是“所有首领都获得殉誓”，而是**每个首领获得自己的专属能力**。
A20 不是所有 Boss 都有王烙印，而是**第三幕第二个 Boss 才进入烙印形态**。
所有会改变伤害的能力，必须**正确更新敌人意图**。
多人缩放必须统一，不允许 4 人局把机制线性放大到失控。
尽量使用游戏已有关键词：**人工制品、多重护甲、无形、活力、易伤、虚弱**，不要再用一堆新名词把玩家绕晕。

---

# 0. A19 / A20 最终规则

## A19：首领专属能力

从 A19 开始，每个首领获得一个专属能力。

页面不要写成：

> 首领获得专属能力：殉誓

因为这会让人误会所有 Boss 都是殉誓。

应该写成：

> **A19：首领专属能力**
> 每个首领获得一个贴合自身机制的专属能力。

具体到 Boss 页面时写：

> **亲族祭司：殉誓**
> 随从死亡会强化亲族祭司的下一次攻击或负面状态。

> **墨影幻灵：墨返**
> 首次完全移除滑溜后，墨影幻灵会返还一部分滑溜。

> **永世沙漏：时砂回流**
> 消退后会生成时砂，玩家可通过花费能量清除。

---

## A20：烙印形态

A20 只影响第三幕双 Boss 的第二个 Boss。

| 场景          | 效果                        |
| ----------- | ------------------------- |
| 第一幕 Boss    | A19 专属能力                  |
| 第二幕 Boss    | A19 专属能力                  |
| 第三幕第一个 Boss | A19 专属能力                  |
| 第三幕第二个 Boss | A20 烙印形态，即该 Boss 的专属能力强化版 |

页面短文案：

> **A20：烙印形态**
> 第三幕第二个首领的专属能力会强化。双首领顺序会提前显示。第一个首领结束后，进入中庭，恢复部分已损生命并获得一次首领卡牌奖励。

---

# 1. 全局规则

## 1.1 伤害必须进入意图预览

所有会改变攻击伤害的专属能力，都必须更新敌人意图。

例如：

亲族祭司本来显示攻击 9。
如果有 2 枚殉誓，每枚使攻击 +3，那么意图应该直接显示：

> 攻击 15

不能显示 9，实际打 15。
这类隐藏伤害会让玩家觉得被阴。

适用能力：

* 亲族祭司：殉誓攻击加伤；
* 灵魂异鱼：如果未来改回伤害类魂潮，也必须显示；当前版本不加伤；
* 无厌沙虫：活力影响攻击时必须显示；
* 永世沙漏：眼部激光额外命中必须显示；
* 瀑布巨兽：易伤应显示在玩家状态上；
* 帝皇蟹：校准攻击额外伤害必须显示；
* 任何 A20 烙印造成的攻击变化。

---

# 2. 亲族祭司：殉誓

## 当前问题

亲族祭司只有两个随从，不会继续召唤新随从，所以最多 3 次触发不合理。
这个能力也不能写得像 A19 通用能力。

## A19：殉誓

**触发：**亲族随从死亡。

**效果：**

> 每当一名亲族随从死亡，亲族祭司获得 1 枚殉誓，最多 2 枚。
> 亲族祭司下一次施加负面状态时，消耗所有殉誓；每枚殉誓使该负面状态持续时间 +1。
> 亲族祭司下一次攻击时，消耗所有殉誓；每枚殉誓使该次攻击额外造成 3 点伤害。

## A20：殉誓烙印

> 殉誓仍最多 2 枚。
> 每枚殉誓使负面状态持续时间 +1。
> 若用于攻击，每枚殉誓额外造成 4 点伤害。
> 如果两名随从在同一回合死亡，亲族祭司获得 1 层人工制品。

## 意图显示

如果亲族祭司有殉誓，并且下一次行动是攻击，意图必须显示强化后的总伤害。

如果下一次行动是负面状态，Debuff 意图悬停说明：

> 殉誓：持续时间 +X。

## 多人缩放

殉誓是 Boss 共享计数。
最多 2 枚，不随人数增加。
A20 的人工制品最终显示 1 层，不随人数增加。

## 玩家短文案

> 随从死亡会使亲族祭司获得殉誓。殉誓会强化它的下一次负面状态或攻击。

---

# 3. 墨影幻灵：墨返

## 当前问题

固定返还 1–2 层滑溜太弱。
多人局滑溜可能非常高，返还 2 层几乎等于没有。

## A19：墨返

**触发：**墨影幻灵的滑溜第一次被完全移除。

**效果：**

> 下个敌方回合开始时，墨影幻灵返还本次被清除滑溜的 25%，向上取整。
> 最少返还 3 层，最多返还 12 层。
> 每场战斗只触发 1 次。

## A20：墨返烙印

> 返还比例提高到 35%。
> 最少返还 5 层，最多返还 18 层。
> 每场战斗只触发 1 次。

## 例子

如果单人局清掉 9 层滑溜：

* A19 返还 3 层；
* A20 返还 5 层。

如果多人局清掉 54 层滑溜：

* A19 按比例是 14 层，但封顶 12；
* A20 按比例是 19 层，但封顶 18。

## 多人缩放

按实际被清除的滑溜层数计算。
不额外按人数乘。

## 玩家短文案

> 首次完全移除滑溜后，墨影幻灵会在下个敌方回合返还一部分滑溜。每场只触发一次。

---

# 4. 乐加维林族母：多重护甲苏醒

## 当前问题

不要再造“梦壳、甲壳、护壳”这些词。
如果游戏已有 **多重护甲**，就直接使用多重护甲。

## A19：多重护甲苏醒

**触发：**乐加维林族母被唤醒。

**效果：**

> 若被玩家提前打醒，获得 4 层多重护甲。
> 若自然醒来，获得 8 层多重护甲。
> 第一次摄魂后，当前多重护甲减少一半，向下取整。

## A20：多重护甲苏醒烙印

> 若被玩家提前打醒，获得 6 层多重护甲。
> 若自然醒来，获得 10 层多重护甲。
> 第一次摄魂后，当前多重护甲只减少三分之一，向下取整。

## 多人缩放

这里的 4 / 8 / 6 / 10 是单人基准。
多人模式下，如果游戏本身会缩放多重护甲，则按游戏已有规则最终显示。

页面要写：

> 多人模式下，多重护甲会按首领战规则缩放。

## 玩家短文案

> 族母醒来时获得多重护甲。自然醒来获得更多；摄魂会削去一部分多重护甲。

---

# 5. 灵魂异鱼：魂潮

## 当前问题

A20 魂潮“上限提高但多一个限制”看起来像削弱。
而且呼唤本来就会让玩家掉血，再额外加伤容易过压。
所以灵魂异鱼不走加伤，回到更稳的格挡节奏。

## A19：魂潮

**触发：**进入无形；呼唤在手牌中结算。

**效果：**

> 灵魂异鱼进入无形时，获得 1 层人工制品。
> 回合结束时，每有 1 张呼唤在玩家手牌中结算，灵魂异鱼下回合开始获得 2 点格挡。
> 每回合最多获得一定上限的格挡。

## A19 格挡上限

| 玩家数 | 每回合上限 |
| --- | ----: |
| 1   |     8 |
| 2   |    12 |
| 3–4 |    16 |

## A20：魂潮烙印

> 灵魂异鱼进入无形时，获得 1 层人工制品。
> 每张呼唤结算提供 3 点格挡。
> 每回合格挡上限提高。

## A20 格挡上限

| 玩家数 | 每回合上限 |
| --- | ----: |
| 1   |    12 |
| 2   |    16 |
| 3–4 |    20 |

## 多人缩放

每名玩家的呼唤独立结算，但最终格挡有团队上限。
人工制品最终显示 1 层，不随人数增加。

## 玩家短文案

> 呼唤结算会让灵魂异鱼下回合获得格挡。进入无形时，它获得人工制品。

---

# 6. 瀑布巨兽：不可削弱

## 当前问题

10 层人工制品和 99 层人工制品对爆发回合差别不大。
如果 Boss 爆发后就死，99 层只是数字变大，没有玩法意义。

你建议给玩家易伤，这更有效。

## A19：不可削弱

**触发：**瀑布巨兽进入爆发回合。

**效果：**

> 清除自身虚弱和攻击降低。
> 本回合爆发伤害不受虚弱、力量降低或攻击降低影响。
> 获得足够人工制品，持续到爆发结算后移除。
> 爆发回合开始时，所有会受到爆发伤害的玩家获得 1 回合易伤。

## A20：不可削弱烙印

> 爆发回合开始时，所有会受到爆发伤害的玩家获得 2 回合易伤。
> 爆发仍不受虚弱、力量降低或攻击降低影响。
> 不提高爆发基础伤害。

## 多人缩放

如果爆发是全体伤害，全体获得易伤。
如果爆发只影响部分玩家，只给受影响玩家易伤。
易伤持续时间不随人数增加。

## 玩家短文案

> 爆发回合无法被虚弱或降攻削弱，并会使受爆发影响的玩家易伤。

---

# 7. 帝皇蟹：错壳校准

## 当前问题

背击一定会频繁发生，用背击触发惩罚会让玩家觉得自己正常打法被罚。
所以改成看两只爪子的血线差。

## A19：错壳校准

**触发：**玩家回合结束时。

**效果：**

> 检查两只爪的生命百分比。
> 如果两只爪生命百分比相差 35% 或更多，生命百分比较高的那只爪获得 1 层校准。
> 校准达到 2 层时，清除校准；该爪下一次攻击额外造成 4 点伤害。
> 每只爪每场最多触发 1 次校准攻击。

## A20：错壳校准烙印

> 生命百分比相差 30% 或更多时触发校准。
> 校准攻击额外造成 5 点伤害。
> 每只爪每场最多触发 1 次。

## 意图显示

如果某只爪已经因为校准获得下一次攻击加伤，攻击意图必须显示最终伤害。

## 多人缩放

团队共享检查。
每个玩家回合结束只检查一次。
不按人数增加触发次数。

## 玩家短文案

> 两只爪血线差距过大时，生命较高的爪会校准。校准满后，它的下一次攻击更强。

---

# 8. 知识恶魔：旁注

## 当前问题

之前深思强化懒惰和枯竭太赖皮。
懒惰本来就限制每回合出牌数，不能继续把它压到 1。
枯竭也不能把最大能量进一步压到极低。
所以深思不再强化核心硬限制，而是加附加代价。

## A19：旁注

**触发：**知识诅咒二选一后。

**效果：**

> 未选择的诅咒会变成 1 张旁注，洗入弃牌堆。

## 旁注

费用：1
类型：临时技能
关键词：保留，消耗，临时

> 打出：抽 1 张牌。
> 如果旁注在手牌中回合结束时没有被打出，知识恶魔获得 1 层深思，并消耗旁注。

## 深思

> 下次知识诅咒时，每层深思会让诅咒选项附带额外代价。
> 使用知识诅咒后，清除所有深思。
> 深思最多 2 层。

## 深思强化表

| 诅咒   | 不再做什么     | 每层深思的新效果              |
| ---- | --------- | --------------------- |
| 崩解   | 不大幅提高伤害   | 崩解每次触发时，额外造成 1 点伤害    |
| 心智腐烂 | 不额外减少抽牌   | 选择后，将 1 张 Dazed 洗入弃牌堆 |
| 懒惰   | 不降低可出牌数   | 选择后，本回合下一张牌费用 +1      |
| 枯竭   | 不额外降低最大能量 | 选择后，本回合失去 1 点能量       |

## A20：旁注烙印

> 深思最多 3 层。
> 但懒惰和枯竭的附加代价每次知识诅咒最多只结算 1 层。
> 崩解和心智腐烂可以按深思层数结算。

## 多人缩放

每名玩家可以独立获得旁注。
Boss 共享深思。
每回合最多因旁注增加 2 层深思。
A19 深思上限 2，A20 上限 3。

## 玩家短文案

> 未选诅咒会变成旁注。若旁注留在手牌回合结束，知识恶魔会深思，让下次知识诅咒附带额外代价。

---

# 9. 无厌沙虫：逃亡疲劳

## 当前问题

“饥噬”如果只是加伤，其实可以直接使用已有 **活力**。
不要重复造新词。

## A19：逃亡疲劳

**触发：**玩家打出由该专属能力生成的狂乱逃离。

**效果：**

> 玩家每打出第 3 张由该能力生成的狂乱逃离，无厌沙虫获得 2 点活力。

## A20：逃亡疲劳烙印

> 玩家每打出第 3 张由该能力生成的狂乱逃离，无厌沙虫获得 3 点活力。

## 多人缩放

逃离牌按玩家个人统计。
Boss 获得活力是团队共享效果。
每个玩家回合最多触发 1 次逃亡疲劳。

## 玩家短文案

> 打出多张由首领生成的狂乱逃离，会使无厌沙虫获得活力。

---

# 10. 永世沙漏：时砂回流

## 源码机制理解

根据目前源码，永世沙漏主要动作包括：

* **消退**：攻击，并给玩家施加消退，临时降低力量和敏捷，玩家回合结束后恢复；
* **眼部激光**：多段攻击，当前是 2 次命中；
* **加大力度**：向弃牌堆加入 Wither，升级已有 Wither，并让永世沙漏获得力量和格挡。

所以“加大力度”不是一个已经存在的通用 Power，而是一个行动。
王印设计应该挂在这个行动上，而不是说“获得 1 层加大力度”。

---

## A19：时砂回流

**触发：**永世沙漏使用消退后。

**效果：**

> 生成 2 枚时砂。
> 下个玩家回合中，玩家每花费 1 点能量，移除 1 枚时砂。
> 回合结束时，每剩余 1 枚时砂，使下一次加大力度额外加入 1 张 Wither。

## A20：时砂回流烙印

> 消退后生成 3 枚时砂。
> 回合结束时，每剩余 1 枚时砂，使下一次加大力度额外加入 1 张 Wither。
> 如果眼部激光回合开始时仍有时砂，眼部激光额外命中 1 次。
> 每场最多触发 2 次额外命中。

## 多人缩放

时砂是团队共享计数。
每名玩家花费能量都可以移除时砂。
时砂数量默认不随玩家数增加。

如果 3–4 人过于容易清完，后续再调成：

| 玩家数 | A19 时砂 | A20 时砂 |
| --- | -----: | -----: |
| 1–2 |      2 |      3 |
| 3–4 |      3 |      4 |

首版先不建议加。

## 玩家短文案

> 消退后生成时砂。下个回合每花费 1 点能量会移除 1 枚；剩余时砂会让下一次加大力度加入更多 Wither。

---

# 11. 女王：御令

## A19：御令

**触发：**女王施加束缚时。

**效果：**

> 被束缚的牌中随机 1 张获得御令标记。

本回合结算：

| 玩家行为      | 结果                    |
| --------- | --------------------- |
| 打出御令牌     | 无额外惩罚                 |
| 打出非御令束缚牌  | 女王获得 1 层威仪            |
| 没有打出任何束缚牌 | 女王获得 1 层威仪，火炬头获得 1 力量 |

## 威仪

> 女王下一次防御或屏障类动作额外获得 8 格挡。
> 触发后清除所有威仪。
> 威仪最多 2 层。

## A20：御令烙印

> 威仪最多 3 层。
> 女王每次防御或屏障类动作最多消耗 2 层威仪。

## 多人缩放

每名玩家独立判断是否遵从御令。
女王每回合最多获得 2 层威仪。
火炬头每回合最多因此获得 2 力量。

## UI 要求

御令牌必须有明显标记。
悬停说明：

> 本回合打出这张牌可避免御令惩罚。

## 玩家短文案

> 束缚牌中会出现御令。打出御令可避免惩罚；错打或不打会让女王积累威仪。

---

# 12. 实验体：实验记录

## 当前问题

这个设计确实复杂。
保留“实验体会记录上一阶段”的概念，但必须降低复杂度，并且 UI 必须承担解释。

---

## A19：实验记录

**触发：**实验体进入新阶段时。

**效果：**

> 实验体记录上一阶段最明显的战斗特征，并获得 1 份残留样本。

## 样本只保留 4 类

| 上一阶段特征         | 样本   |
| -------------- | ---- |
| Boss 结束阶段时仍有力量 | 力量残留 |
| 玩家打出技能牌最多      | 技能适应 |
| 玩家打出攻击牌最多      | 攻击适应 |
| 玩家施加过 Debuff   | 抗体样本 |

如果没有明显特征：

> 默认获得污染样本。

## 样本效果

### 力量残留

> 新阶段开始时，保留上一阶段结束时力量的 30%，向上取整。
> 第二阶段最多 3 力量，第三阶段最多 6 力量。

### 技能适应

> 本阶段第一次单回合打出第 3 张技能牌时，实验体获得 1 力量。每阶段一次。

### 攻击适应

> 本阶段第一次单回合打出第 4 张攻击牌时，实验体获得 1 层人工制品。每阶段一次。

### 抗体样本

> 本阶段第一次受到 Debuff 时，实验体获得 2 层人工制品。每阶段一次。

### 污染样本

> 本阶段第一次洗牌时，将 1 张短效状态牌加入弃牌堆。每阶段一次。

## A20：实验记录烙印

> 每次进入新阶段时，实验体获得 2 份不同残留样本。
> 力量残留仍遵守上限。

## UI 要求

阶段变化时必须弹短提示：

> 实验记录：技能适应
> 你上一阶段主要使用技能牌。实验体本阶段会适应技能牌。

Boss 状态栏显示样本图标。
悬停只显示一句话。

## 多人缩放

统计全队行为，但 A19 只生成 1 个样本。
A20 生成 2 个不同样本。
不会因为 4 人局生成 4 个样本。

## 玩家短文案

> 实验体会记录上一阶段最明显的战斗特征，并在下一阶段获得对应残留样本。

---

# 13. 页面展示建议

不要写成现在这种：

> 原版 / 当前 / A19 / A20 / 王印 / 王烙印 / 展开具体效果 / 王印数值 / 王烙印数值

这对玩家太重。

建议每个 Boss 条目改成：

## 示例：亲族祭司

**A19 专属能力：殉誓**
随从死亡会使亲族祭司获得殉誓。殉誓会强化它的下一次负面状态或攻击。

**A20 烙印形态**
殉誓攻击伤害更高；若同回合击杀两名随从，亲族祭司获得 1 层人工制品。

**详细效果**
A19：每名随从死亡获得 1 枚殉誓，最多 2 枚。下一次负面状态每枚持续时间 +1；下一次攻击每枚伤害 +3。
A20：攻击伤害提高为每枚 +4。同回合击杀两名随从时，获得 1 层人工制品。

---

# 最终结论

这一版重新解决了你指出的问题：

* A19 不再让人误会“殉誓是通用能力”；
* 伤害变化必须提前显示；
* 墨影幻灵滑溜返还按比例，不再弱到没意义；
* 乐加维林族母直接用多重护甲，不重复造词；
* 灵魂异鱼回到格挡节奏，不额外叠伤害；
* 瀑布巨兽不再给玩家压力阀，不会降低难度，而是给易伤；
* 帝皇蟹不再惩罚正常背击，改为血线校准；
* 知识恶魔深思不再把懒惰/枯竭变成死刑；
* 无厌沙虫直接用已有活力；
* 永世沙漏基于消退、眼部激光、加大力度动态实现；
* 实验体机制简化，并强制要求阶段提示 UI。

最终口径：

> **A19 是首领专属能力。A20 是第三幕第二个首领的烙印形态。每个能力都必须贴合 Boss 原机制，必须可预告、可理解、可应对。**


## 结论：你的直觉基本对，但要精确一点

这个项目不是“完全没有 OOP”。它现在有很多 C# 类，也用了 `CustomCardModel`、`CustomAncientModel`、`AbstractModel` hook、Power、Relic、Card 这类游戏本身的对象模型。但真正的问题是：

```text
它是“OOP 外壳 + 大量 static service / partial service / 字符串状态 / patch 串联”的结构。
```

所以 bug 多不只是“没继承/没 abstract”这么简单，而是下面这些问题叠在一起：

```text
1. 模块初始化没有统一注册层。
2. Feature gate 分散，每个系统自己判断开关。
3. Reward / Combat / Death / Map / SaveLoad 都由很多 static service 分散处理。
4. 状态编码大量靠 string / bool / WeakTable / CardModel field，缺少 typed state object。
5. 多个系统改同一个生命周期，没有统一 pipeline / priority / ownership。
6. tests 很多是 source-string guard，不是行为级单元测试或集成测试。
7. 多人模式没有统一 MultiplayerPolicy。
8. 大功能已经 source-implemented，但 live evidence 还没覆盖。
```

也就是说，**bug 多确实和架构耦合、状态所有权不清、缺少 OOP 边界有关**。

---

# 1. 当前项目已经是大型玩法包，不是小 mod 了

`PROJECT_STATE.md` 当前记录的 active feature areas 已经包括：

* Ancient reward rebalance v4；
* Ascension 11–20；
* Rootblight；
* Urda；
* Morvi；
* Lotha；
* hidden Vakuu fight；
* Preview tools；
* multiplayer mismatch diagnostics；
* package / website / art / release evidence。

这说明当前项目规模已经接近“多系统 DLC 包”。但现在很多代码结构仍然像“单个 mod 功能持续追加”那样写。

最明显的例子是 `MainFile.Initialize()`：它直接创建 Harmony、注册 config，然后直接调用 `LothaInitializer.Initialize()`、`MorviInitializer.Initialize()`、`UrdaInitializer.Initialize()`、`VakuuFightInitializer.Initialize()`、`AscensionInitializer.Initialize()`。

这不是灾难，但它说明当前没有一个统一的：

```text
FeatureRegistry
FeatureModule
FeatureGate
InitOrder
RuntimeStatus
DependencyGraph
```

所以随着模块增加，初始化顺序、enable/disable、诊断、测试隔离都会越来越难。

---

# 2. 和游戏源码相比，项目没有充分利用“对象生命周期模型”

我检查了你上传的 v0.106 code-only 源码包。游戏源码本身其实是很强的 OOP / lifecycle hook 风格：

* `AbstractModel` 有大量 virtual hooks，例如 `BeforeCombatStart`、`AfterCardPlayed`、`AfterCardDrawn`、`AfterCombatEnd`、`AfterActEntered` 等。
* `CardPileCmd.Add(...)` 在 v0.106 里已经有 `clonedBy` 参数，说明官方命令 API 通过 source object / command context 来传递行为来源。
* `CardReward` 有 `OnSelect()`、`OnSkipped()`、`Reroll()`、`Populate()` 等对象级生命周期。
* `StartRunLobby` 有明确 lobby / BeginRun / ascension / preferred progress / multiplayer message flow。

也就是说，游戏本身的抽象思路是：

```text
模型对象 + 生命周期 hook + command API + 明确 owner/context。
```

而我们项目里很多地方变成了：

```text
static service + static patch + shared string state + weak table context + source-string tests。
```

这就是“看起来有类，但核心逻辑不是面向对象”的地方。

---

# 3. 主要架构问题逐项分析

## 3.1 初始化耦合：MainFile 直接依赖所有大模块

当前 `MainFile.Initialize()` 直接依赖所有 feature initializer。

问题：

```text
[ ] MainFile 知道所有模块细节。
[ ] 模块之间没有统一 metadata。
[ ] 无法统一打印 feature enabled/disabled reason。
[ ] 无法优雅做 test profile，比如只开 Ascension / 只开 Morvi。
[ ] 无法统一处理初始化失败。
[ ] 未来每加一个系统都要改 MainFile。
```

应该改成：

```csharp
FeatureRegistry.Register(new AscensionFeatureModule());
FeatureRegistry.Register(new AncientRebalanceFeatureModule());
FeatureRegistry.Register(new UrdaFeatureModule());
FeatureRegistry.Register(new MorviFeatureModule());
FeatureRegistry.Register(new LothaFeatureModule());
FeatureRegistry.Register(new VakuuFeatureModule());
FeatureRegistry.Register(new PreviewToolsFeatureModule());

FeatureRegistry.InitializeAll();
```

每个 module 提供：

```csharp
interface IFeatureModule
{
    string Id { get; }
    string DisplayName { get; }
    int InitOrder { get; }
    FeatureGateResult EvaluateGate();
    void Register();
    FeatureRuntimeStatus GetStatus();
}
```

这样日志能变成：

```text
[Spire Plus] Feature Morvi: enabled=true, reason=default-on
[Spire Plus] Feature VakuuFight: enabled=false, reason=requires SPIREPLUS_ENABLE_VAKUU_FIGHT
[Spire Plus] Feature Ascension: enabled=true
```

---

## 3.2 Feature gate 分散，缺少统一语义

Morvi 和 Lotha 现在都有自己的 gate，逻辑是 default-on，disable env var 关闭。Morvi 支持 `EZMB_DISABLE_MORVI` / `SPIREPLUS_DISABLE_MORVI`，也支持 force ancient / force blessing。 Lotha 也是同样模式。

这符合你现在“默认开没问题”的方向。但问题是：这些 gate 都是手写的，各自实现。长期会出现：

```text
[ ] 某个 feature 支持 SPIREPLUS_*，另一个只支持 EZMB_*。
[ ] force gate 和 disable gate 优先级不统一。
[ ] docs 说默认开，但 source 某处实际默认关。
[ ] test 不知道该用哪个 env var。
```

建议统一成：

```csharp
FeatureGate
FeatureGateResult
FeatureGateReason
```

示例：

```csharp
FeatureGateResult EnabledByDefault(string featureId);
FeatureGateResult DisabledByEnv(string featureId, string env);
FeatureGateResult ForcedByEnv(string featureId, string env);
```

并统一 env 规则：

```text
SPIREPLUS_DISABLE_MORVI=1
EZMB_DISABLE_MORVI=1

SPIREPLUS_FORCE_ANCIENT=MORVI
EZMB_FORCE_ANCIENT=MORVI

SPIREPLUS_FORCE_MORVI_BLESSING=morvi_debt_settlement
EZMB_FORCE_MORVI_BLESSING=morvi_debt_settlement
```

---

## 3.3 状态模型太脆：字符串状态 + SavedSpireField 多，但没有 typed codec

`AncientSavedStateFields.cs` 现在有很多 `SavedSpireField`，包括 Urda/Morvi/Lotha 的 player state、deck state、card marker。 这说明项目已经在认真做存档。但问题是：很多状态最终仍是 `string`。

Urda 是典型例子。`UrdaBlessingService.State.cs` 用 `AncientPlayerState.Get(...)` 取出 string，然后用 `;` 分隔，按 index 解析一长串字段；写入时又 `string.Join(ProgressSeparator, ...)` 拼接几十个字段。

这会导致：

```text
[ ] 字段顺序一错，旧存档就解析错。
[ ] 新增字段只能靠 parts.Length 猜版本。
[ ] 字段里如果包含分隔符，要靠 sanitize。
[ ] 很难单独测试某个状态。
[ ] 很难做 save migration。
[ ] 多人/断线重连时状态不一致很难排查。
```

这不是“不能用 string”，而是必须有 typed codec：

```csharp
record UrdaStateV1
{
    string SelectedBlessing;
    SeedbedState Seedbed;
    HumusState Humus;
    MoltingState Molting;
    MossMapState MossMap;
    TrialBranchState TrialBranch;
    RootSightState RootSight;
    int Version = 1;
}

interface IFeatureStateCodec<TState>
{
    string Encode(TState state);
    TState Decode(string raw);
    TState Default { get; }
}
```

然后 tests 覆盖：

```text
[ ] Decode empty string -> Default
[ ] Decode malformed string -> Default / partial fallback
[ ] Decode old version -> migrate
[ ] Encode/Decode round-trip
[ ] field with delimiter is escaped or rejected safely
```

---

## 3.4 `ConditionalWeakTable` 用作 UI 上下文，高风险

Urda 早期和当前 reward flow 都有类似：把 `CardReward` 对象作为 key，保存 reward context。`UrdaRunHook` / `UrdaBlessingService` 里曾用 `ConditionalWeakTable<CardReward, CardRewardContext>` 来识别当前卡牌奖励是不是一幕普通战斗奖励、是否已记录 Seedbed、是否已处理 Humus skip。

这是可以作为 runtime UI session cache，但不能当作 save/load 状态。

风险：

```text
[ ] 在 reward screen 保存/退出，WeakTable 消失。
[ ] Continue 后 reward object 重新构造，context 不在。
[ ] 玩家可以通过 save/load 重复触发 Seedbed/Humus。
[ ] skip / alternative 可能丢失来源信息。
```

建议把状态分层：

```text
PersistentState：必须保存到 SavedSpireField
RuntimeSessionState：可以 WeakTable，但不保证 save/load
UiRenderState：只用于显示
```

并在每个 service 文件里明确：

```csharp
// UI-session-only. Not save/load persistent.
```

同时在 manual matrix 里加：

```text
Save on reward screen before alternative.
Save after alternative click.
Save after skip before Humus completion.
Continue and verify no duplicated HP/card/gold.
```

---

## 3.5 Reward 系统缺少统一 pipeline，多个 feature 会互相踩

现在很多系统都在改 reward：

```text
Urda Seedbed
Urda Humus Pact
Morvi Forbidden Loan
Morvi Debt Settlement
Lotha Closed Court
Prismatic Gem
A13 Fission
A19 Boss reward +1 option
Ancient reward rebalance
```

Morvi 的测试 guard 里也可以看到它涉及 option relic、reward candidates、Forbidden Loan、Debt Settlement、generated-card guards、OpenBook sealed cards 等。

如果没有统一 pipeline，就会出现：

```text
[ ] Closed Court 移除 CardReward 后，Fission 还想改 CardReward。
[ ] Prismatic reroll 后，Seedbed context 指向旧 reward。
[ ] Boss reward +1 和 Fission 先后顺序不稳定。
[ ] Humus skip 和 other skip listener 同时触发。
[ ] Debt Settlement / Forbidden Loan 打开选择界面时 reward 已完成或未完成。
```

建议建立：

```csharp
EzmbRewardPipeline
RewardContext
RewardHandlerPriority
IRewardHandler
```

阶段：

```text
BeforePopulate
AfterPopulate
ModifyOptions
ModifyOptionsLate
AddAlternatives
BeforeSelect
AfterSelect
OnSkipped
OnCompleted
```

每个 feature handler 声明：

```csharp
FeatureId
Priority
CanHandle(context)
Handle(context)
```

这不是为了“抽象而抽象”，而是让 bug 可排查。log 可以写：

```text
[SpirePlus RewardPipeline]
source=Encounter
room=Monster
act=1
handlers=PrismaticGem,Fission,UrdaSeedbed
alternatives=UrdaSeedbed
skippedHandlers=HumusPact
```

---

## 3.6 Combat extra-play 需要统一 execution context

Morvi/Lotha 有大量 extra-play / replay / verdict / mirror / sentence 逻辑。测试 guard 已经检查它们避免 `CreateClone`、避免 `CardCmd.AutoPlay`、检查 `!card.IsClone`、`cardPlay.IsAutoPlay`、Power fallback 等。

但现在很多规则大概率散在各自 service 里。这会导致组合 bug：

```text
Misprint Press 额外打出的牌触发 Single Sentence。
Single Sentence 额外打出的牌触发 Mirror Rebuttal。
Mirror Rebuttal 额外打出的牌触发 Deferred Verdict。
Power fallback 触发了“本回合第一张牌”规则。
AutoPlay 被当成玩家真实打出。
```

需要一个统一对象：

```csharp
EzmbCardPlayContext
```

包含：

```text
SourceFeature
SourceEffect
IsExtraPlay
IsReplay
IsAutoPlay
IsClone
SuppressSameFeature
Depth
OriginalCardId
OriginalCardInstance
```

所有 extra-play 前都检查：

```csharp
if (context.Depth > 0 && context.SourceFeature == currentFeature) return;
if (card.Type == CardType.Power) return PowerFallback;
if (card.IsClone || cardPlay.IsAutoPlay) return;
```

这能把“能力牌安全规则”从散落 if 变成统一 policy。

---

## 3.7 Death protection 应该从 Lotha 里拆成服务

Lotha guard 显示它现在已经碰到了 `ShouldDieLate`、`ShouldDie`、`AfterPreventingDeath`、`CreatureCmd.Kill(player.Creature, force: true)`。

这是最需要 OOP/架构保护的地方。死亡保护不是普通 blessing effect，它是全局规则，应该由：

```text
EzmbDeathProtectionService
```

统一管理。

需要处理：

```text
[ ] priority：Lotha Death Reprieve、Urda After Rain、可能未来 Fairy / vanilla
[ ] once-per-run
[ ] once-per-combat
[ ] in-death-resolution flag
[ ] force-unpreventable death
[ ] enemy-turn interruption
[ ] co-op player ownership
[ ] save/load while reprieved
```

否则最容易出现：

```text
[ ] 缓期失败死亡又被缓期保护
[ ] 强制死亡被其他保护救下
[ ] 敌方行动队列继续执行，玩家第二次死亡
[ ] 多人里一个玩家死亡，另一个也被错误影响
[ ] save/load 后 inReprieve 状态丢失
```

---

## 3.8 Multiplayer 缺少 policy，导致每个功能自己猜

`PROJECT_STATE.md` 仍明确说 multiplayer co-op verification matrix pending，A11-A20 co-op fail-closed / diagnostics 仍在验证中。

现在这些功能都可能改共享状态：

```text
Urda：奖励、牌组、房间奖励、HP、金币
Morvi：债务、牌组、临时牌、能量、HP
Lotha：死亡、出牌限制、奖励抑制、debuff
Rootblight：牌组、生成牌、战后状态
Ascension：地图、Boss、奖励、战斗增强
```

应该给每个 effect 标注：

```text
LocalUiOnly
LocalPlayerOnly
HostAuthoritative
SharedRunState
CombatCommandReplicated
UnsafeInMultiplayer
```

然后 code review 时要求：

```text
[ ] 改 deck/gold/hp 必须有 policy。
[ ] 本地 UI 提示必须 LocalContext.IsMe 或等价。
[ ] map metadata 必须 host authoritative。
[ ] reward mutation 不能 host/client 双触发。
[ ] combat generated card 必须通过 command API。
```

这是多人稳定的前提。

---

## 3.9 Tests 过度依赖 source-string guard

`AncientExpansionReleaseCoverageGuardTests.cs` 里大量使用 `Assert.Contains(...)` 去检查源码字符串，例如 Morvi 的 blessing ids、Power 名、常量、`!card.IsClone`、`CardType.Attack or CardType.Skill`、`CreateClone` 不存在等。

这类测试有价值，能防止明显删错。但它的问题是：

```text
[ ] 代码里有字符串，不代表行为正确。
[ ] 顺序不对也能过。
[ ] state 保存坏了也能过。
[ ] UI softlock 也能过。
[ ] 多人双触发也能过。
[ ] 文案说完成，但 live 没测也能过。
```

下一层需要补：

```text
unit tests for state codec
simulation tests for reward pipeline
simulation tests for card play context
diagnostic logs for live runs
manual-test evidence required for issue closure
```

---

# 重构路线：不要重写，做 Strangler Refactor

不要一次性重写全部。建议用“绞杀式重构”：新框架慢慢包住旧 service，每次只搬一个高风险区域。

## Milestone A：FeatureRegistry + Gate 统一

目标：

```text
MainFile 不直接知道所有模块细节。
```

改动：

```text
Core/Features/IFeatureModule.cs
Core/Features/FeatureRegistry.cs
Core/Features/FeatureGateResult.cs
```

验收：

```text
[ ] MainFile 只注册 modules
[ ] 每个 module log enabled reason
[ ] Morvi/Lotha/Urda 默认开启不变
[ ] Vakuu hidden 不变
[ ] Disable/force gate 全部照旧
```

---

## Milestone B：State Codec

目标：

```text
把复杂 string state 变成 typed state + codec。
```

先做 Urda，因为 Urda state 现在最明显。`UrdaBlessingService.State.cs` 当前一长串 `string.Join` 和 index parse 是很典型的重构对象。

验收：

```text
[ ] UrdaStateV1 Encode/Decode
[ ] malformed fallback
[ ] old short state migration
[ ] round-trip tests
[ ] progress fields named, not index-only
```

然后再 Morvi / Lotha。

---

## Milestone C：RewardPipeline

目标：

```text
统一 reward 修改顺序。
```

先不搬所有，只建 pipeline docs + diagnostics，然后逐步把 Urda / Fission / Prismatic / ClosedCourt 纳入。

验收：

```text
[ ] reward source/room/act/handlers log
[ ] handler priority documented
[ ] skipped/select/complete phase 分清
[ ] save/load weak context risk 写清
```

---

## Milestone D：CardPlayContext

目标：

```text
统一 extra-play / replay / verdict / fallback 规则。
```

先覆盖：

```text
Morvi Misprint Press
Lotha Mirror Rebuttal
Lotha Single Sentence
Lotha Deferred Verdict
```

验收：

```text
[ ] Power fallback 不进 extra-play
[ ] AutoPlay/Clone 不触发 first-card rules
[ ] 同 feature 不递归
[ ] depth guard
[ ] diagnostics
```

---

## Milestone E：DeathProtectionService

目标：

```text
Lotha Death Reprieve 不再孤立处理死亡。
```

验收：

```text
[ ] force death unblockable
[ ] in-death-resolution flag
[ ] co-op player ownership
[ ] save/load policy
[ ] source evidence for ShouldDie/ShouldDieLate order
```

---

## Milestone F：MultiplayerPolicy annotations

目标：

```text
每个高风险 effect 都声明多人策略。
```

验收：

```text
[ ] mutate HP/gold/deck/reward 必须有 policy
[ ] local UI only 使用 local player guard
[ ] shared map metadata host authoritative
[ ] co-op test matrix 自动生成
```

---

# 给 Codex 的重构 prompt

```text
你现在在仓库 D:\Game\FOTN\dev-the-spire。

目标：Spire Plus architecture decoupling pass。不要新增 gameplay。不要关闭 Morvi/Lotha/Urda 默认开启。不要回滚已有内容。只做架构解耦、状态 codec、pipeline、执行上下文、死亡保护、多人策略、测试护栏。

必须先读：
1. PROJECT_STATE.md
2. AGENTS.md
3. EZMicroBalanceCode/MainFile.cs
4. EZMicroBalanceCode/Ancients/Common/AncientSavedStateFields.cs
5. EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaBlessingService.State.cs
6. EZMicroBalanceCode/Ancients/Expansion/Urda/**
7. EZMicroBalanceCode/Ancients/Expansion/Morvi/**
8. EZMicroBalanceCode/Ancients/Expansion/Lotha/**
9. EZMicroBalanceCode/Ancients/Expansion/Vakuu/**
10. EZMicroBalanceCode/Ascension/**
11. tests/EZMicroBalance.Tests/AncientExpansionReleaseCoverageGuardTests.cs
12. source code/src/Core/**
13. sourcecodeonlyaianalysis/**

硬规则：
- 不要实现新祝福。
- 不要改 manifest id。
- 不要默认关闭当前默认开启功能。
- 不要大规模重写。
- 每次只做可验证的小步。
- 不要 claim release-ready。

Phase 1：FeatureRegistry

新增：
- EZMicroBalanceCode/Core/Features/IFeatureModule.cs
- EZMicroBalanceCode/Core/Features/FeatureRegistry.cs
- EZMicroBalanceCode/Core/Features/FeatureGateResult.cs

重构 MainFile：
- MainFile 不再直接调用所有 Initializer。
- MainFile 注册 feature modules。
- FeatureRegistry 负责按 InitOrder 初始化。
- 每个 feature log id/enabled/reason/env gates。
- 现有默认开启/隐藏逻辑不能改变。

Phase 2：UrdaState codec

新增：
- UrdaStateV1
- UrdaStateCodec

替换 UrdaBlessingService.State.cs 中 index-heavy parse/string.Join 逻辑。
保留 SavedSpireField<string>，但通过 codec 读写。
支持：
- empty state
- malformed state
- old short state
- full current state
- round-trip tests

Phase 3：Reward pipeline diagnostics

新增轻量：
- EzmbRewardPipeline docs 或 code wrapper
- Reward handler priority map

先记录，不强行搬完所有逻辑。
必须列出：
- Urda Seedbed
- Urda Humus
- Prismatic Gem
- Fission
- Lotha Closed Court
- Morvi Forbidden Loan/Debt Settlement

加 diagnostics:
- reward source
- room type
- active handlers
- alternatives
- skip handlers

Phase 4：CardPlayContext

新增：
- EzmbCardPlayContext
- ExtraPlayPolicy

先覆盖 Morvi/Lotha extra-play 入口。
要求：
- Power card fallback only
- AutoPlay/clone 不递归
- same feature depth guard
- source diagnostics

Phase 5：DeathProtectionService

新增：
- EzmbDeathProtectionService

把 Lotha Death Reprieve 最危险的 flags/forced death logic 集中。
要求：
- used flag
- inReprieve flag
- forced unavoidable death flag
- co-op player identity
- source evidence docs

Phase 6：MultiplayerPolicy docs + annotations

新增：
- docs/features/multiplayer-safety-policy.md

每个高风险 feature 标注：
- LocalUiOnly
- LocalPlayerOnly
- HostAuthoritative
- SharedRunState
- CombatCommandReplicated
- UnsafeInMultiplayer

至少覆盖：
- Urda
- Morvi
- Lotha
- Vakuu
- Rootblight
- Ascension BossSeal/Fission/Banner/Firemark

Phase 7：tests

新增/更新 tests：
- MainFile should use FeatureRegistry
- UrdaStateCodec roundtrip/malformed/old version
- ExtraPlayPolicy blocks Power copy/replay
- DeathProtectionService has forced death guard
- multiplayer policy doc exists and covers active features
- source-string tests can stay, but do not rely only on them for new codec/policy tests

Validation:
- dotnet build EZMicroBalance.sln
- dotnet test EZMicroBalance.sln
- dotnet test EZMicroBalance.sln --no-build
- dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
- git diff --check
- publish only if source/resources changed and package refresh is needed

Final report:
- What was decoupled
- What remained old/static
- Which modules now use FeatureRegistry
- UrdaState codec status
- Reward pipeline status
- ExtraPlay context status
- Death protection status
- Multiplayer policy status
- tests results
- remaining architecture debt
- release-ready: no
```

---

## 最后判断

你这个判断是对的：

```text
bug 多，确实有一部分来自架构不够解耦、状态模型太脆、hook 顺序不统一、缺少执行上下文和多人策略。
```

但不要简单理解成“少 abstract / 少 inheritance”。真正要补的是：

```text
Feature module abstraction
State codec abstraction
Reward pipeline abstraction
Card play execution context
Death protection service
Multiplayer policy annotation
Runtime diagnostics
Behavior tests beyond source-string guard
```

这几个做完，项目的可维护性会明显提升；否则继续加内容，bug 会越来越像“随机出现”，因为根本不知道是谁在哪个生命周期里改了哪个状态。
