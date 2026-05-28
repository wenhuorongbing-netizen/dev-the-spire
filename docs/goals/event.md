结论：**没有完成，而且这次暴露出一个更大的问题：他的 Act 映射很可能是错的，这会直接导致事件体验和《杀戮尖塔 1》差很多。**

我把完整严格审核、June 2026 monthly dev spec、以及 subagent 分工写成了文档：

## 最关键审核结论

### 1. “build passes / compiles”这个说法不成立

你贴的日志里，正常 `dotnet build --no-restore` 先失败了，原因是 `VakuuFightInitializer` 重复定义。后来他用：

```bash
grep -v "Vakuu"
```

过滤掉 Vakuu 错误，然后说 “Zero errors from StS1 event code”。

这个只能说明：**过滤已知 Vakuu blocker 后，暂时没有看到 StS1 事件代码的 compiler error**。它不能等价于：

> 46 个事件 Done, compiles
> build passes
> 可以进入下一阶段

正确表述应该是：

> whole-repo build blocked by pre-existing Vakuu duplicate-class error; StS1 code shows no extra `error CS` after filtering known Vakuu blocker.

本月验收必须要求 **不带 grep 的完整 `dotnet build --no-restore` exit code 0**。否则不能标 Done。

---

### 2. 他发现的 StS2 Act 映射是错的，这会严重破坏体验

他写的是：

> StS2 acts are Underdocks = Act 1, Overgrowth = Act 2, Hive = Act 3

我检查了你上传的 StS2 v0.106.0 source。正确关系应是：

| StS1 bucket        | 应注册到的 StS2 Act                 |
| ------------------ | ------------------------------ |
| Act 1 exclusive    | `Overgrowth` + `Underdocks`    |
| Act 2 exclusive    | `Hive`                         |
| Act 3 exclusive    | `Glory`                        |
| Shared events      | shared event registry          |
| Semi-shared events | 精确按 StS1 允许 Act 注册，不能全部 shared |

也就是说，**Underdocks 不是唯一 Act 1；它是 alternate Act 1。Overgrowth 也是 Act 1。Hive 是 Act 2。Glory 才是 Act 3。**

这点非常严重。如果他把 Act 2 事件注册到 `Overgrowth`，那 Act 2 的 StS1 事件会跑到 StS2 Act 1。把 Act 3 事件注册到 `Hive`，那 Act 3 事件会跑到 StS2 Act 2。`Glory` 如果完全没注册，真正 Act 3 就缺 StS1 Act 3 事件。你觉得“事件和杀戮尖塔 1 游戏体验出入很大”，这很可能是第一大原因。

---

### 3. `RegisterAll(ModId)` 被无条件接进 `MainFile`，这是危险的

他把：

```csharp
Sts1EventRegistrationService.RegisterAll(ModId);
```

直接放到 `MainFile.Initialize()` 里。这意味着只要 Spire Plus 启动，StS1 事件就可能默认注册。

这不符合当前项目边界。你的项目当前仍是单一 active mod：`Spire Plus`，技术 manifest id 仍是 `EZMicroBalance`，代码/资源目录分别是 `EZMicroBalanceCode/` 和 `EZMicroBalance/`。 发布结构也仍应是 `EZMicroBalance.json / EZMicroBalance.dll / EZMicroBalance.pck`。

StS1 event port 现在只能是 prototype feature，必须默认 Off。正确模式应该是：

```text
Off                           默认，注册 0 个 StS1 事件
CanaryOnly                    只注册 Big Fish / Golden Idol / Lab / Divine Fountain
AdditiveBatch1                只注册已验证 Batch 1
ReplaceUnknownEventsPrototype debug-only，替换未知房间事件池
```

`RegisterAll` 不能无条件进入默认初始化路径。

---

### 4. 46 / 48 / 52 的口径仍然混乱

他现在同时说：

```text
52 StS1 events
48 spec docs Done
46 event C# models Done
```

这不是一个可接受的完成口径。

Wiki 事件页写得很清楚：事件来自 unknown location，事件选择受随机和当前 Act 影响，有的事件只在特定 Act 出现；Act 4 没有 unknown location/event；Ascension 15 会增强不利事件的概率或强度。Wiki 事件列表还给出 16 个 Shared events、12 个 Act 1 exclusive events、16 个 Act 2 exclusive events、8 个 Act 3 exclusive events。([Slay the Spire Wiki][1])

正确文档必须拆成三列：

| 字段                       | 意义                     |
| ------------------------ | ---------------------- |
| `wiki_event_entries`     | Wiki 目标条目，默认按 52 管     |
| `runtime_event_models`   | 实际写了几个 `EventModel` 类  |
| `act_bucket_memberships` | 每个事件出现在哪些 Act / bucket |

没有这个映射表之前，不能说“48 specs cover all unique events”，也不能说“46 models done”。

---

### 5. RitsuLib 注册只是 additive，不等于 StS1 体验

他发现 RitsuLib 的 `RegisterSharedEvent<T>()`、`RegisterActEvent<TAct, TEvent>()`、`CreateContentPack().SharedEvent<T>() / ActEvent<TAct, TEvent>()`，这一步是有价值的。

但这只代表：事件被加入 StS2 的事件枚举或候选池。

它不代表：

```text
unknown room 只会抽 StS1 事件
Act 1/2/3 事件池和 StS1 一样
事件权重/去重/条件过滤一样
A15 差异一样
reward/card/relic/potion pool 一样
save/load 后事件 bag 一样
```

StS2 源码里 `ActModel.GenerateRooms` 会把本 Act 的 `AllEvents` 和 `ModelDb.AllSharedEvents` 合并后打乱，所以单纯注册 StS1 事件会和 StS2 原生事件混在一起。这是“游戏体验不像 StS1”的第二大原因。

---

### 6. Canary 事件仍没完成

Big Fish 的 Wiki 行为是：Act 1 专属；Banana 回复 `floor(maxHP / 3)`，Donut 增加 5 最大生命，Box 给随机 common/uncommon/rare relic 并加入 Regret。([Slay the Spire Wiki][2])

Golden Idol 的 Wiki 行为是：Act 1 专属；Take 获得 Golden Idol 并触发陷阱；Outrun 给 Injury；Smash 造成 25% max HP 伤害，A15+ 为 35%；Hide 损失 8% max HP，A15+ 为 10%；Leave 无事发生。([Slay the Spire Wiki][3])

但他自己的日志还写着：

```text
Next: resolve missing curse card model references and test canary events in-game.
Blockers: Regret, Injury, random relic helper...
```

所以 Big Fish / Golden Idol 都不能验收。缺 Regret、Injury、random relic helper、Golden Idol relic、HP/max HP helper、save/load/manual proof，就不能叫 canary done。

---

### 7. “替代内容”会破坏 StS1 体验

他写：

```text
Missing curses: Parasite → Clumsy substitute
Madness → Debt substitute
Bite → needs custom card
```

这个方向只能叫 prototype compatibility，不叫 parity。

尤其是“Parasite → Clumsy”这种替代，会改变 deck 负担、curse synergy、移除价值、事件收益判断；“Madness → Debt”更不是 StS1 等价体验。要做“和杀戮尖塔 1 完全一样”，这些必须分类：

| 状态                     | 含义                                      |
| ---------------------- | --------------------------------------- |
| `native-equivalent`    | StS2 已有完全等价模型                           |
| `custom-required`      | 必须新建 StS1-compatible card/relic/monster |
| `temporary-substitute` | 只允许 prototype 临时用，阻塞 parity             |
| `blocked`              | 当前无法安全实现                                |

任何 `temporary-substitute` 都不能算完成。

---

## 每一步完成情况

| 模块                    | 他声称                            | 严格审核                                                          |
| --------------------- | ------------------------------ | ------------------------------------------------------------- |
| Wiki catalog          | 52 events / 48 specs cover all | **未通过**，46/48/52 口径冲突                                         |
| Event specs           | Done                           | **不接受**，需要逐事件 exact options / A15 / dependency / source proof |
| C# models             | 46 Done, compiles              | **不接受**，whole repo build 失败，且大量 gameplay 未闭环                  |
| Registration service  | Done, wired                    | **部分完成但危险**，无条件注册 + Act 映射错                                   |
| RitsuLib API research | Done                           | **部分通过**，但版本/Act mapping 仍需 source-backed matrix              |
| Build                 | passes                         | **不成立**，grep 过滤不算 pass                                        |
| Canary events         | In progress / source done      | **未完成**，关键 curse/relic/reward helper 缺失                       |
| Localization          | 46 events Done                 | **未完成**，没有 in-game render proof                               |
| Assets                | scripts Done                   | **未完成**，没有 52-event image mapping 和截图验证                       |
| Tests                 | 未见有效 evidence                  | **未完成**，build 不等于 gameplay/manual proof                       |
| StS1-only event pool  | 未做                             | **未完成**，additive registration 不等于替换池                          |
| 游戏体验 parity           | 暗示接近                           | **0% full parity**，目前只是 prototype infrastructure              |

我的严格评分：

```text
文档/研究框架：25%
Source/API 正确性：15% —— Act 映射错
Build readiness：0% —— whole repo build 失败
Registration safety：20% —— 有服务，但默认无条件且映射错
Canary gameplay：5%
资产/本地化实证：0–10%
StS1 游戏体验：0–5%
Full parity：0%
```

---

## 为什么你会感觉“和杀戮尖塔 1 出入很大”

主要原因不是“事件数量还不够”，而是底层体验没有被复刻：

1. **Act 映射错**：Act 2/Act 3 事件可能进错章节。
2. **只是 additive 注册**：StS1 事件和 StS2 原事件混池，不是 StS1 unknown room 体验。
3. **Reward pool 不同**：StS2 relic/card/potion pool 和 StS1 不一样，事件收益判断会变。
4. **替代卡/诅咒破坏判断**：Parasite、Bite、Madness、Apparition 等不能随便替代。
5. **A15 差异未闭环**：StS1 事件在 A15 的变化是体验核心之一。
6. **事件条件缺失**：例如 Divine Fountain 必须有 curse 才能出现，很多事件也有遗物/金币/牌组条件。
7. **图像/文本/页面节奏没验证**：事件不是只有效果，还包括图片、页面、选项锁、hover、死亡提示、动态数值。
8. **没有 StS1 event bag / no-repeat / save-load parity**：事件抽取节奏不像原作。

---

## 下一步 Monthly Dev Spec：June 2026

本月目标不要叫 full parity，应该叫：

```text
StS1 Event Port Prototype Batch 1 — Parity Foundation
截止：2026-06-30
```

### 月末验收标准

必须达到：

1. 默认 Off，对 Spire Plus 零行为影响。
2. `CanaryOnly` 只注册 Big Fish / Golden Idol / Lab / Divine Fountain。
3. Act 映射正确：Overgrowth + Underdocks = Act 1，Hive = Act 2，Glory = Act 3。
4. 完整 `dotnet build --no-restore` 0 errors，不允许 grep 过滤。
5. 四个 canary playable + save/load + image render + EN/ZHS render。
6. 六个简单事件 playable：Purifier、Upgrade Shrine、Golden Shrine、The Cleric、Old Beggar、Shining Light。
7. 有 debug-only StS1 replacement pool prototype。
8. 所有 status 不再用泛泛的 `Done`，改用证据状态。
9. QA subagent 独立验收，不能实现者自验。

项目规则也要求代码/资源改动后跑对应验证；资源、本地化、打包改动后还要 publish，并保持 release 文档、版本、package metadata 对齐。 原版素材也不能随意复制进仓库，原版 art 需要授权确认，否则只能本地抽取/替代。

### Week 0：2026-05-28 至 2026-05-31

目标：停止虚假完成口径，修正方向。

必须交付：

```text
docs/features/sts1-events/audit-2026-05-28.md
docs/features/sts1-events/source-research/sts2-act-mapping.md
docs/features/sts1-events/source-research/api-command-matrix.md
docs/features/sts1-events/status-board.md
```

验收：

```text
不再有 46/48/52 混乱
不再有无证据 Done
RegisterAll 从默认路径撤掉
Vakuu build blocker 明确修复或标为 blocking issue
Act mapping 修正
```

### Week 1：2026-06-01 至 2026-06-07

目标：安全注册和 clean build。

必须交付：

```text
Sts1EventFeatureGate
Sts1EventRegistrationService modes
registration count tests
act bucket mapping tests
完整 build evidence
```

验收：

```text
Off 模式注册 0 个 StS1 事件
CanaryOnly 精确注册 4 个事件
Act 1 事件注册到 Overgrowth + Underdocks
Act 2 事件注册到 Hive
Act 3 事件注册到 Glory
dotnet build --no-restore 0 errors
```

### Week 2：2026-06-08 至 2026-06-14

目标：四个 canary 真正 playable。

事件：

```text
Big Fish
Golden Idol
Lab
Divine Fountain
```

必须补齐：

```text
Sts1HpService
Sts1RewardService
Sts1CurseService
Sts1RelicService
Sts1AscensionRules
Golden Idol relic / exact equivalent
Regret / Injury exact model usage
```

验收：

```text
Big Fish 三选项完全正确
Golden Idol 全分支 + A15 完全正确
Lab 药水数量/池明确
Divine Fountain 只在有 curse 时出现并移除所有 curse
四事件都有 debug spawn 截图、结果日志、save/load 证明
```

### Week 3：2026-06-15 至 2026-06-21

目标：简单事件 Batch 1。

实现：

```text
Purifier
Upgrade Shrine
Golden Shrine
The Cleric
Old Beggar
Shining Light
```

验收：

```text
每个事件 exact options
每个事件动态数值正确
每个事件 EN/ZHS 渲染通过
每个事件图片加载通过
每个事件 manual debug-spawn proof
```

### Week 4：2026-06-22 至 2026-06-28

目标：修复“体验不像 StS1”的核心问题。

必须交付：

```text
Sts1EventPoolService prototype
ReplaceUnknownEventsPrototype debug-only mode
event bag save/load
visited ids save/load
run log evidence
```

验收：

```text
debug replacement mode 不抽 StS2 原事件
默认关闭
multiplayer fail-closed
同一 run 不重复抽未允许重复的事件
Act bucket 正确
```

### Week 5：2026-06-29 至 2026-06-30

目标：月末 handoff。

必须交付：

```text
monthly-review-2026-06.md
updated feature README
updated PROJECT_MAP / docs index
evidence screenshots/logs
package version bump, only if shipped player-visible behavior
```

验收：

```text
release note 只能写 Prototype Batch 1
不能写 full parity
所有 blocker 明确列出
commit + push 只能在验证通过后做
```

---

## 必须提醒他使用 subagent

直接给他的指令可以这样发：

```text
不要继续按“46 个事件 Done / 48 specs Done”的口径推进。当前不是完成状态。

立刻使用 subagents：

1. BuildGate / Repo Health Subagent
2. StS2 Source/API Auditor Subagent
3. Wiki Parity Spec Auditor Subagent
4. Feature Gate / Registration Engineer Subagent
5. Canary Gameplay Engineer Subagent
6. Content Parity Subagent
7. Event Pool / RNG / Save Subagent
8. Asset + Localization Subagent
9. QA / Red-Team Subagent
10. Release Documentation Subagent

最高优先级：

1. 修正 build gate：unfiltered dotnet build 必须 0 errors；grep -v Vakuu 不算 build pass。
2. 修正 StS2 act mapping：Overgrowth + Underdocks = Act 1，Hive = Act 2，Glory = Act 3。
3. 删除默认无条件 RegisterAll；StS1 events 必须默认 Off，只能用 CanaryOnly / AdditiveBatch1 / ReplacementPrototype 开启。
4. 修复 46/48/52 口径；用 wiki_event_entries / runtime_event_models / act_bucket_memberships 三栏记录。
5. 先把 Big Fish、Golden Idol、Lab、Divine Fountain 做到 playable + save/load + image/loc render proof。
6. 任何没有测试、截图、日志、source/API 证据的内容不得标 Done。
7. RitsuLib 注册只是 additive，不等于 StS1-only event pool，也不等于杀戮尖塔1体验。
```

最重要的一句话：**不要让他继续“批量写 46 个空壳事件”。先把 build、Act 映射、feature gate、canary、replacement pool 这五件事做对，否则事件数量越多，离《杀戮尖塔 1》体验越远。**

[1]: https://slay-the-spire.fandom.com/wiki/Events "Events | Slay the Spire Wiki | Fandom"
[2]: https://slay-the-spire.fandom.com/wiki/Big_Fish "Big Fish | Slay the Spire Wiki | Fandom"
[3]: https://slay-the-spire.fandom.com/wiki/Golden_Idol_%28Event%29 "Golden Idol (Event) | Slay the Spire Wiki | Fandom"

# StS1 Event Port 严格审核 v3 + June 2026 Monthly Dev Spec + Subagent Work Orders

Date: 2026-05-28
Scope: audit of the latest pasted implementation log for the StS1 event-port work inside `dev-the-spire` / `Spire Plus` (`EZMicroBalance`).

## Executive Verdict

**Status: Not complete. Do not mark as full parity, release-ready, or player-ready.**

The latest work is an infrastructure/registration partial pass, not a gameplay parity implementation. The most important new finding is that the claimed StS2 act mapping is wrong:

- `Overgrowth` is the default Act 1.
- `Underdocks` can replace Act 1 when unlocked / selected by run generation.
- `Hive` is Act 2.
- `Glory` is Act 3.

Therefore, any registration map that treats `Underdocks = Act 1`, `Overgrowth = Act 2`, `Hive = Act 3` will place StS1 Act 2 events in an Act 1 pool, StS1 Act 3 events in an Act 2 pool, and miss the true Act 3 (`Glory`). This alone can explain why the event/gameplay feel is far away from Slay the Spire 1.

## Severity-A Findings

### A1. Build gate is not actually passed

The pasted log first shows `dotnet build --no-restore` failing with duplicate `VakuuFightInitializer` errors in `VakuuFightRunHook.cs`. Filtering out Vakuu errors with `grep -v Vakuu` is a useful triage step, but it is not a valid build pass.

Acceptable wording:

> Current whole-repo build is blocked by pre-existing Vakuu duplicate-class errors; the StS1 code path shows no additional `error CS` after filtering the known Vakuu blocker.

Unacceptable wording:

> Event C# models are Done, compiles / build passes.

Release or monthly gate must require a normal unfiltered `dotnet build --no-restore` exit code 0.

### A2. StS2 act mapping is wrong

The agent's note says:

> StS2 acts are Underdocks (Act 1), Overgrowth (Act 2), Hive (Act 3).

This is wrong for the uploaded StS2 source snapshot. In the source, `ActModel.GetDefaultList()` returns `Overgrowth`, `Hive`, `Glory`, while `Underdocks` replaces `list[0]` when its epoch is revealed / selected. The correct mapping for StS1 event registration is:

| StS1 bucket | StS2 act models |
|---|---|
| Act 1 exclusive | `Overgrowth` and `Underdocks` |
| Act 2 exclusive | `Hive` |
| Act 3 exclusive | `Glory` |
| Shared | shared-event registry, plus exact StS1 eligibility rules |
| Semi-shared | act-scoped registration for exact allowed buckets, not blanket shared |

This must be fixed before any gameplay testing claim.

### A3. `RegisterAll(ModId)` was wired unconditionally

The pasted log shows `Sts1EventRegistrationService.RegisterAll(ModId)` added directly after `RitsuLibBootstrap.ApplyPatches(ModId)` in `MainFile.Initialize()`.

That is unsafe. StS1 events are a prototype port, not the default Spire Plus behavior. Registration must be behind an explicit feature gate. Required modes:

- `Off`: default, registers zero StS1 events.
- `CanaryOnly`: registers only Big Fish, Golden Idol, Lab, Divine Fountain.
- `AdditiveBatch1`: registers only manually verified Batch 1 events.
- `ReplaceUnknownEventsPrototype`: debug-only replacement pool; multiplayer fail-closed.

### A4. 46 / 48 / 52 inconsistency is unresolved

The agent claims:

- 52 StS1 events as the goal.
- 48 spec files cover all unique events.
- 46 event C# models are done.

This is not an acceptable accounting model. It must be split into three explicit counts:

| Count type | Meaning | Required status |
|---|---|---|
| `wiki_event_entries` | Wiki-listed target entries | Canonical list, default target = 52 |
| `runtime_event_models` | actual `EventModel` subclasses | must map to one or more wiki entries |
| `act_bucket_memberships` | where each event appears | must be exact per StS1 |

No event may be marked Done until all three columns are reconciled.

### A5. RitsuLib registration is additive, not StS1 parity

RitsuLib event registration is useful and probably the right modding entry point, but registration alone only adds events to StS2's existing event enumeration. It does not reproduce StS1's experience.

A true StS1-feel mode also needs:

- correct act bucket mapping;
- exact event preconditions via `IsAllowed`;
- StS1 event order / no-repeat behavior;
- replacement of StS2 native events in unknown rooms for the debug parity mode;
- save/load of shuffled event bag and visited ids;
- multiplayer fail-closed behavior;
- screenshot/log/manual evidence.

### A6. Canary gameplay is not complete

The canary events cannot be accepted as complete until the key StS1 effects are implemented exactly.

Big Fish must implement:

- Banana: heal `floor(maxHP / 3)`;
- Donut: gain +5 max HP and heal the gained HP;
- Box: random common/uncommon/rare relic from the correct pool and add Regret.

Golden Idol must implement:

- Take: obtain Golden Idol relic and open trap page;
- Outrun: add Injury;
- Smash: current HP damage equal to 25% max HP, 35% at A15+;
- Hide: lose 8% max HP, 10% at A15+;
- Leave: no effect.

The pasted log still lists curse model references and random relic helper as next steps, so these canary events are not complete.

### A7. Substituting missing StS1 content is not parity

The log says some StS1 content is missing and proposes substitutes, such as Parasite → Clumsy and Madness → Debt. This may be acceptable for an explicitly marked compatibility placeholder, but it is not "和杀戮尖塔1完全一模一样".

For parity work, missing content must be tracked as:

- `native-equivalent`: exact StS2 model matches StS1 behavior;
- `custom-required`: implement StS1-compatible custom model;
- `temporary-substitute`: allowed only in prototype, must block parity status;
- `blocked`: no safe or legal implementation path yet.

### A8. Localization and asset status cannot be Done without render proof

EN/ZHS JSON entries and extraction scripts are not enough. Asset/localization status requires:

- the expected image exists under the mod resource path;
- the image loads in Godot / event layout;
- the event page renders in-game in English and Simplified Chinese;
- option text and dynamic values match the current game state;
- screenshots are stored as evidence.

Also, original StS1 art should be extracted from a local legitimate installation and not committed/distributed unless redistribution permission is documented.

### A9. Experience gap diagnosis

The user impression that the event experience differs significantly from StS1 is supported by the current implementation state. The highest-probability causes are:

1. wrong StS2 act mapping;
2. additive registration mixing StS1 events with StS2 events;
3. wrong or missing event preconditions;
4. StS2 reward/card/relic/potion pools replacing StS1 pools;
5. missing custom StS1 cards/relics/monsters;
6. placeholder localization and missing images;
7. no StS1 event RNG / no-repeat / save-load parity;
8. no manual visual comparison pass;
9. default-on registration polluting Spire Plus instead of a controlled prototype flag.

## Step-by-Step Audit Table

| Step | Claim | Audit verdict | Required correction |
|---|---|---|---|
| Wiki catalog | 52 events, 48 specs are enough | Failed / unresolved | Build canonical `wiki_event_entries` table and model-mapping table |
| Event specs | all specs Done | Not accepted | Use statuses: planned, spec-drafted, source-verified, api-verified, implemented, asset-verified, loc-render-verified, manual-verified, blocked |
| C# event models | 46 Done, compiles | Not accepted | Whole repo build must pass; each model needs behavior tests and manual proof |
| Registration service | Done, wired into MainFile | Partially useful but unsafe | Add feature gate and correct act mapping |
| RitsuLib API usage | API discovered | Likely useful | Verify against installed RitsuLib version and current game target |
| Build | build passes | False for whole repo | Fix Vakuu duplicate blocker or document blocked build; do not claim pass |
| Curse/relic dependencies | next step | Incomplete | Implement exact StS1-compatible models/helpers before canary verification |
| Images | asset scripts Done | Incomplete | Fill 52-event asset map and verify in-game render |
| Localization | EN/ZHS Done | Incomplete | Need non-placeholder text and render screenshots |
| Event pool | events appear in pool | Not parity | Build debug-only StS1 replacement pool |
| Save/load | not shown | Not done | Save shuffled bag, visited ids, event page state when needed |
| Manual gameplay | not shown | Not done | Debug spawn + unknown room + screenshots + logs |
| Full parity | implied | 0% | Do not claim until all evidence gates pass |

## Corrected Monthly Dev Spec: June 2026

### Monthly target

Deliver **StS1 Event Port Prototype Batch 1 — Parity Foundation**, not full parity.

By 2026-06-30, the repo should have:

1. default Off mode with zero player-visible impact;
2. correct StS2 act mapping;
3. unfiltered `dotnet build --no-restore` passing;
4. four canary events fully playable and manually verified;
5. six simple Batch 1 events playable;
6. debug-only StS1 replacement-pool prototype for the verified events;
7. asset and localization render proof for all implemented events;
8. strict status board with no false Done labels;
9. subagent outputs attached to the feature docs.

### Week 0: 2026-05-28 to 2026-05-31 — Stop false progress and fix gates

Deliverables:

- `docs/features/sts1-events/audit-2026-05-28.md`
- corrected `status-board.md`
- `source-research/sts2-act-mapping.md`
- `source-research/api-command-matrix.md`
- feature gate implementation plan

Acceptance:

- no event marked Done without implementation + evidence;
- `RegisterAll` removed from the default unconditional path;
- act mapping fixed to Overgrowth/Underdocks = Act 1, Hive = Act 2, Glory = Act 3;
- Vakuu duplicate build blocker either fixed or tracked as a blocking issue, not ignored.

### Week 1: 2026-06-01 to 2026-06-07 — Build and registration foundation

Deliverables:

- `Sts1EventFeatureGate`
- `Sts1EventRegistrationService` with modes: Off / CanaryOnly / AdditiveBatch1 / ReplaceUnknownEventsPrototype
- registration tests for act buckets
- build-gate evidence

Acceptance:

- default mode registers zero StS1 events;
- CanaryOnly registers exactly Big Fish, Golden Idol, Lab, Divine Fountain;
- Act 1 canary events are registered to both Overgrowth and Underdocks;
- Act 2 events register to Hive;
- Act 3 events register to Glory;
- `dotnet build --no-restore` passes without grep filtering.

### Week 2: 2026-06-08 to 2026-06-14 — Canary parity

Events:

- Big Fish
- Golden Idol
- Lab
- Divine Fountain

Support services:

- `Sts1HpService`
- `Sts1RewardService`
- `Sts1CurseService`
- `Sts1RelicService`
- `Sts1AscensionRules`

Acceptance:

- Big Fish all three options match StS1 behavior;
- Golden Idol all branches match StS1 behavior and A15 deltas;
- Lab gives the correct potion count / StS2-compatible exact documented behavior;
- Divine Fountain only appears when the run has curses and removes all curses;
- no placeholder substitutions are called parity;
- each event has debug spawn screenshot, option-result evidence, and save/load proof.

### Week 3: 2026-06-15 to 2026-06-21 — Simple Batch 1

Implement six events:

- Purifier
- Upgrade Shrine
- Golden Shrine
- The Cleric
- Old Beggar
- Shining Light

Acceptance:

- each event has exact option values, locks, A15 differences, and dynamic text;
- each event has EN/ZHS render proof;
- each event has image proof;
- each event has manual debug-spawn proof;
- tests verify manifest/spec/localization coverage.

### Week 4: 2026-06-22 to 2026-06-28 — Event pool and gameplay feel

Deliverables:

- `Sts1EventPoolService` prototype;
- debug-only unknown-room replacement mode for implemented events;
- save/load proof for event bag and visited ids;
- gameplay comparison checklist.

Acceptance:

- replacement mode does not draw StS2 native events in the controlled test set;
- replacement mode is disabled by default;
- multiplayer fails closed;
- run logs prove correct act buckets and no repeated event unless all unique events are exhausted.

### Week 5 buffer: 2026-06-29 to 2026-06-30 — Packaging and handoff

Deliverables:

- `monthly-review-2026-06.md`
- updated `README.md`, `PROJECT_MAP.md`, feature README, test plan
- package version bump if player-visible behavior is shipped
- evidence folder with screenshots/logs/build output

Acceptance:

- release notes say Prototype Batch 1, not full parity;
- all blockers are explicit;
- no false Done labels remain;
- commit + push only if validation passes.

## Required Subagent Work Orders

Use subagents now. Do not continue serially writing dozens of event stubs.

### 1. BuildGate / Repo Health Subagent

Mission: get the whole repo back to a clean build.

Tasks:

- fix or isolate the duplicate `VakuuFightInitializer` definition;
- run unfiltered `dotnet build --no-restore`;
- record build output in docs.

Pass condition: build exit code 0, no grep filtering.

### 2. StS2 Source/API Auditor Subagent

Mission: prevent wrong API assumptions.

Tasks:

- document exact act mapping;
- document EventModel lifecycle;
- document EventOption APIs;
- document HP, max HP, relic, card, potion, reward, save/load commands;
- document RitsuLib version actually installed.

Pass condition: `source-research/api-command-matrix.md` has source-backed command names and owners.

### 3. Wiki Parity Spec Auditor Subagent

Mission: make the 52-event target honest.

Tasks:

- build canonical 52-row wiki-event table;
- split `wiki_event_entries`, `runtime_event_models`, and `act_bucket_memberships`;
- check every spec for exact options and A15 deltas;
- mark missing data as blocked, not Done.

Pass condition: no 46/48/52 contradiction remains.

### 4. Feature Gate / Registration Engineer Subagent

Mission: make registration safe and correct.

Tasks:

- remove unconditional `RegisterAll`;
- implement Off / CanaryOnly / AdditiveBatch1 / ReplacementPrototype modes;
- correct act registration mapping;
- add tests for registration counts.

Pass condition: default mode registers zero StS1 events; CanaryOnly count is exact.

### 5. Canary Gameplay Engineer Subagent

Mission: implement the four canary events exactly.

Tasks:

- implement Big Fish, Golden Idol, Lab, Divine Fountain;
- use exact service helpers;
- no temporary substitutes can be marked parity.

Pass condition: four event screenshots, result logs, and save/load proof.

### 6. Content Parity Subagent

Mission: resolve missing StS1 content.

Tasks:

- curses: Regret, Injury, Parasite, Shame, Pain, Normality, Decay, Doubt, Writhe, Clumsy as needed;
- special cards: Bite, Apparition, Ritual Dagger, J.A.X., etc.;
- relics: Golden Idol, Bloody Idol, Red Mask, Necronomicon, Neow's Lament, etc.;
- potions/reward pools.

Pass condition: every dependency is native-equivalent, custom-required, temporary-substitute, or blocked.

### 7. Event Pool / RNG / Save Subagent

Mission: make it feel like StS1, not mixed StS2.

Tasks:

- implement debug-only replacement pool;
- save event bag and visited ids;
- enforce no-repeat and IsAllowed rules;
- multiplayer fail-closed.

Pass condition: controlled unknown-room tests draw only implemented StS1 events.

### 8. Asset + Localization Subagent

Mission: make visuals/text real.

Tasks:

- build 52-event asset extraction map;
- extract from a local legitimate StS1 install;
- verify images load in Godot;
- verify EN/ZHS localization in-game;
- collect screenshots.

Pass condition: every implemented event has image and text render proof.

### 9. QA / Red-Team Subagent

Mission: independently reject false completion.

Tasks:

- compare each implemented event against Wiki/spec;
- verify build, screenshots, and logs;
- test default Off mode;
- test bad/missing dependency cases;
- test save/load.

Pass condition: QA signs off or blocks with exact reasons. Implementation subagents may not self-approve.

### 10. Release Documentation Subagent

Mission: keep docs honest.

Tasks:

- update feature README, status board, test plan, monthly review;
- keep release notes from saying full parity;
- document blockers and substitutions.

Pass condition: docs match evidence.

## Direct instruction to send to the implementer

```text
不要继续按“46 个事件 Done / 48 specs Done”的口径推进。当前不是完成状态。

立刻使用 subagents：BuildGate、StS2 Source/API Auditor、Wiki Parity Spec Auditor、Feature Gate/Registration Engineer、Canary Gameplay Engineer、Content Parity、Event Pool/RNG/Save、Asset/Localization、QA Red-Team、Release Documentation。

最高优先级：
1. 修正 build gate：unfiltered dotnet build 必须 0 errors；grep -v Vakuu 不算 build pass。
2. 修正 StS2 act mapping：Overgrowth + Underdocks = Act 1，Hive = Act 2，Glory = Act 3。
3. 删除默认无条件 RegisterAll；StS1 events 必须默认 Off，只能用 CanaryOnly/AdditiveBatch1/ReplacementPrototype 开启。
4. 修复 46/48/52 口径；用 wiki_event_entries / runtime_event_models / act_bucket_memberships 三栏记录。
5. 先把 Big Fish、Golden Idol、Lab、Divine Fountain 做到 playable + save/load + image/loc render proof。
6. 任何没有测试、截图、日志、source/API 证据的内容不得标 Done。
7. RitsuLib 注册只是 additive，不等于 StS1-only event pool，也不等于杀戮尖塔1体验。
```

## Completion Score

| Area | Score | Reason |
|---|---:|---|
| Research/document framework | 25% | Useful but inconsistent counts and false Done labels |
| Source/API correctness | 15% | RitsuLib discovered, but act mapping is wrong |
| Build readiness | 0% | Whole repo build fails in pasted log |
| Registration safety | 20% | Service exists, but unconditional and incorrectly mapped |
| Canary gameplay | 5% | Key effects/dependencies unresolved |
| Asset/localization proof | 0-10% | JSON/scripts are not render proof |
| StS1 gameplay feel | 0-5% | Additive mixed pool and wrong act mapping break feel |
| Full StS1 parity | 0% | Not close to complete |

