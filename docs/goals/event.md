# StS1 Event Port 严格审核 v13：当前状态、目标差距、下月开发规范、Subagent 分工与强制 Overnight Run

日期：2026-05-31
项目：`dev-the-spire` / `Spire Plus` / `EZMicroBalance`
目标：在 Slay the Spire 2 的 `Spire Plus` mod 中迁移并尽量复刻 Slay the Spire 1 的事件体验。

---

## 0. 最终判定

**当前任务没有完成。**

可以承认：代码侧基础设施、feature gate、registration guard、部分 spec/code、ZHS placeholder 清零、canonical matrix 文档、自动化 guard tests 等方向已经有明显推进。

但不能承认：StS1 event experience parity、runtime verified、full event port complete、release-ready、和杀戮尖塔 1 完全一样。

当前仍未完成或 blocked 的关键项：

- 4 个 canary events 的 runtime gameplay proof。
- 6 个 simple batch events 的 runtime gameplay proof。
- canary / simple batch 的 save/load proof。
- event image / render proof。
- ReplacementPrototype functional proof。
- combat encounter models。
- independent QA / Red-Team pass/fail。
- 52 / 54 / 48 / 50 / 54 统计口径的独立复核。
- `AdditiveBatch1` 与 `AdditiveAllDraft` 的命名与行为边界复核。
- 自动测试摘要里的 passed / skipped / total 数字一致性复核。

正确状态应写为：

> Code-side foundation advanced; runtime parity and player-experience proof are incomplete.

禁止写：

- all tasks complete
- all StS1 events complete
- full parity complete
- release-ready
- 和杀戮尖塔 1 完全一样

---

## 1. 当前目标重述

用户目标不是“生成一堆事件类文件”，也不是“让注册表里有 48/50/54 个条目”。用户目标是让 StS2 的 `Spire Plus` mod 尽量复刻 StS1 的事件体验，包括：

- unknown room 事件池节奏。
- shared / semi-common / act-exclusive bucket。
- Act 1 / Act 2 / Act 3 出现范围。
- 事件选项流程、页面跳转、锁定条件、死亡提示。
- reward / card / relic / curse / potion 效果。
- Ascension 15 数值和概率变化。
- 图片、英文与简体中文文本、动态数值渲染。
- save/load。
- co-op / `IsShared`。
- 默认不污染现有 Spire Plus。

项目边界：

- active deliverable 仍是单一 `Spire Plus` mod。
- technical manifest id 仍是 `EZMicroBalance`。
- C# 代码应在 `EZMicroBalanceCode/`。
- 资源和本地化应在 `EZMicroBalance/`。
- 不能随意复制原版游戏素材、数据表、大段反编译代码或未授权 art。

---

## 2. StS1 Wiki 事件目标基线

外部公开基线应按 StS1 Wiki 的 event 页面管理：

- Events 来自 unknown location。
- 事件不是 guaranteed，但 unknown location 最可能进入 event。
- 事件由 random chance 和当前 Act 决定。
- 部分事件 Act-exclusive，部分事件可以出现在多个 Act。
- Act 4 没有 unknown locations，因此没有 event。
- Ascension 15 会让不利事件更可能或更强。
- Wiki 列表显示：16 shared events，12 Act 1 exclusive，16 Act 2 exclusive，8 Act 3 exclusive。

因此 public target 是 **52 Wiki listed entries**。内部出现 54 / 50 / 48 / 54 等数字时，必须用 canonical matrix 解释，不能直接当作“完成”。

---

## 3. 历史问题回顾

### 3.1 早期 false Done

早期助理将 48 spec files、assets.md、localization.md、test-plan.md、Infrastructure 等标为 Done，同时 blocker 仍包括 Regret、Injury、random relic helper、card removal/transform/upgrade UI、combat encounter models。这种 Done 不可信。

正确状态必须拆成：

- planned
- spec-drafted
- source-verified
- api-verified
- implemented
- build-verified
- runtime-verified
- asset-verified
- loc-render-verified
- save-load-verified
- qa-verified
- blocked

### 3.2 早期错误 Act mapping

曾出现错误结论：

```text
Underdocks = Act 1
Overgrowth = Act 2
Hive = Act 3
```

这会直接破坏体验。正确映射应为：

```text
StS1 Act 1 -> Overgrowth + Underdocks
StS1 Act 2 -> Hive
StS1 Act 3 -> Glory
Shared -> shared registry
Semi-common -> 按 StS1 允许 Act 精确注册
```

### 3.3 无条件 RegisterAll 风险

曾把 `Sts1EventRegistrationService.RegisterAll(ModId)` 直接接进 `MainFile.Initialize()`。这会让未验证 StS1 事件默认进入 Spire Plus，污染现有 mod。正确要求：

```text
Off                           default，注册 0 个 StS1 events
CanaryOnly                    只注册 4 个 canary
AdditiveBatch1                只注册 4 canary + 6 simple 的 source-guarded prototype scope；runtime 未验证
AdditiveAllDraft              dev-only，全量 draft，不得作为玩家默认模式
ReplaceUnknownEventsPrototype debug-only，替换 unknown room pool；需 debug symbol 与 runtime proof
```

---

## 4. 最新状态逐项审核

根据已有上下文，历史摘要曾声称：

```text
Build: historical stale claim said 0 errors / 0 warnings
Tests: 428 passed, 0 failed, 21 skipped (398 total)
StS1 Guard Tests: historical stale claim said 24/24 pass

v9/v10 priorities:
1 default Off=0 proof claimed
2 CanaryOnly=4 + AdditiveBatch1 exact count claimed, but stale summary mixed Batch1 with 54 total
3 canonical matrix explaining number drift claimed
4 runtime verify 4 canary blocked — requires game launch
5 runtime verify 6 simple batch blocked — requires game launch
6 zero ZHS placeholders claimed — 399 keys, 0 placeholders
7 image/render proof blocked — no redistributable art
8 ReplaceUnknownEventsPrototype functional proof blocked — requires game launch
9 combat events stay blocked — encounter models missing for actual combat-entry events
10 QA/Red-Team independent blocked/fail until runtime evidence exists
```

当前 v13 重新采集的 no-game truth 是：

```text
HEAD: 24d4fe9a
Build: 0 errors, 89 Sts1Events nullable warnings
Tests: 461 passed, 0 failed, 21 skipped, 482 total
StS1 guard tests: 28/28 pass after v13 non-combat registry guard is included
Runtime: blocked; no canary/simple/replacement/save-load/image gameplay proof
```

### 4.1 Build

**状态：AMBER/GREEN**

可暂认代码侧 build 有进展，但必须保存 full unfiltered build log 和 exit code。不能只用摘要、tail 或 grep。

验收要求：

```powershell
dotnet clean EZMicroBalance.sln -m:1 > .tools/runtime-evidence/sts1-events-v13/o1-clean-full.log 2>&1
dotnet build EZMicroBalance.sln -m:1 > .tools/runtime-evidence/sts1-events-v13/o1-build-full.log 2>&1
```

必须记录：

- command
- exit code
- full log path
- commit / branch / diff 状态

### 4.2 Tests

**状态：AMBER**

摘要存在数字冲突：

```text
428 passed + 21 skipped != 398 total
```

可能是“398 total”来自旧摘要、某个 test group、或 typo。BuildGate subagent 必须复核完整 test log。

验收要求：

- passed + failed + skipped = total。
- 21 skipped 逐条解释。
- skipped 是否 release-blocking 必须标注。

### 4.3 StS1 Guard Tests

**状态：GREEN for guard layer only**

28/28 pass 是 guard 层进展，但 guard tests 不能替代 runtime proof。

### 4.4 Default Off

**状态：SOURCE-LEVEL / RUNTIME BLOCKED**

方向正确，但当前只到 source/guard 层；runtime 仍必须证明：

- env var 未设置时注册 0 个 StS1 events。
- default Spire Plus 行为不变。
- AdditiveAllDraft / ReplacementPrototype 不是默认。

### 4.5 CanaryOnly=4

**状态：AMBER**

count 为 4 不够，必须证明 exact identity：

```text
Big Fish
Golden Idol
Lab
Divine Fountain
```

### 4.6 AdditiveBatch1 vs AdditiveAllDraft

**状态：RED/AMBER**

摘要写：

```text
CanaryOnly=4 + AdditiveBatch1 exact count DONE — guard tests verify 4 canary, 54 total
```

这非常可疑。**Batch1 不应是 54 total。** 54 更像 AdditiveAllDraft 或 internal all-entry mode。

必须澄清：

```text
AdditiveBatch1 = 4 canary + 6 simple batch = 10 source-guarded prototype events / 11 registration calls，runtime 未验证
AdditiveAllDraft = 54 registration calls / all draft entries, dev-only
```

### 4.7 Canonical matrix

**状态：AMBER**

他说 matrix done，但仍需 Red-Team 复核。

必须解释：

```text
52 Wiki listed entries
54 internal/reconciled entries
48 runtime models
50 registry entries
54 registration calls
399 loc keys
```

每一项必须能追到具体 event / runtime model / registration call / act membership / reason。

### 4.8 Canary runtime

**状态：RED / BLOCKED**

4 个 canary runtime 未验证，不能算完成。

Canary 必须有：

- debug spawn screenshot。
- pre-state / post-state result log。
- exact option branch verification。
- A15 verification where relevant。
- save/load proof。
- EN/ZHS render screenshot。
- image render proof。

### 4.9 Simple batch runtime

**状态：RED / BLOCKED**

6 个 simple batch code-complete claimed 不能算 runtime complete。必须逐个游戏内验证：

- Purifier
- Upgrade Shrine
- Golden Shrine
- The Cleric
- Old Beggar / Pleading Vagrant canonical mapping
- Shining Light

### 4.10 ZHS placeholder

**状态：AMBER/GREEN for file layer only**

399 keys、0 placeholders 是进展，但仍需：

- missing key scan。
- 游戏内 render proof。
- 动态数值替换 proof。
- 长文本/换行/按钮溢出检查。

### 4.11 Image/render proof

**状态：RED / BLOCKED**

没有 redistributable art，不能算图片完成。

可接受路线：

1. owner 提供授权 art；
2. 本地 StS1 安装目录抽取，并提供 hash proof，但不提交原图；
3. 使用 generated / mod-owned replacement art，并标 non-identical；
4. 临时 placeholder，但明确 non-parity。

### 4.12 ReplacementPrototype

**状态：RED / BLOCKED**

source / guard 存在不等于 functional proof。

必须证明：

- unknown room 不抽 StS2 原事件。
- Act 1/2/3 bucket 正确。
- shared / semi-common / exclusive 规则正确。
- visited / no-repeat / event bag 保存读取正确。
- mode debug-only。

### 4.13 Combat events

**状态：RED / BLOCKED**

combat events 必须保持 blocked，直到 encounter models 完成。不能把 IsShared=true 或 guard test 当成 combat event completion。

### 4.14 QA / Red-Team

**状态：RED / BLOCKED**

必须独立验收。实现者不能自验。

---

## 5. 当前完成度评分

| 维度 | 评分 | 说明 |
|---|---:|---|
| Build/code foundation | 75% | 最新声称 0 errors，但需 full log 和 test count 复核 |
| Guard tests | 80% | 28/28 StS1 guard pass 是 source-level 进展 |
| Feature gate | 70% | default Off 方向对，但 AdditiveBatch1 命名和 exact scope 需修 |
| Canonical matrix | 60% | claimed done，但必须 red-team 52/54/48/50/54 |
| Canary implementation | 55% | code-side claimed complete；runtime 0% |
| Simple batch implementation | 45% | code-side claimed complete；runtime 0% |
| ZHS localization | 65% | placeholder 清零；render 未证 |
| Assets/images | 0–15% | 无 redistributable art / no render proof |
| Replacement pool | 20% | source/guard 可能有；functional proof 没有 |
| Combat events | 0–20% | blocked by encounter models |
| QA/Red-Team | 0% | independent review blocked |
| Full StS1 experience parity | 0–10% | 体验核心仍未 runtime verified |

---

## 6. 与目标对比后的管理决策

**决策：继续优化 + 有限推进，两者兼顾，但优化优先。**

### 6.1 继续优化

必须优先优化：

- build/test evidence 真实性。
- `428 passed / 21 skipped / 398 total` 数字冲突。
- AdditiveBatch1 vs AdditiveAllDraft mode 命名。
- canonical matrix red-team。
- status-board 无 false Done。
- asset/license policy。
- independent QA/Red-Team。

### 6.2 有限推进

只推进 verified scope：

```text
4 canary:
- Big Fish
- Golden Idol
- Lab
- Divine Fountain

6 simple batch:
- Purifier
- Upgrade Shrine
- Golden Shrine
- The Cleric
- Old Beggar / Pleading Vagrant
- Shining Light
```

### 6.3 暂停扩大范围

暂停：

- 继续批量实现更多 draft event。
- combat event full implementation，直到 encounter models 存在。
- custom UI 全量实现。
- full parity 宣称。
- release-ready 宣称。

---

## 7. 下个月开发规范：June 2026 Monthly Dev Spec

### 7.1 目标名称

**StS1 Event Port Prototype Batch 1 — Runtime Parity Foundation**

### 7.2 禁止目标名称

禁止写：

- Full parity
- All events complete
- Release-ready
- 和杀戮尖塔 1 完全一样

### 7.3 月末 Go/No-Go 标准

必须达成：

1. 最新 full unfiltered build exit code 0。
2. 最新 full test exit code 0。
3. test count reconciliation：passed + failed + skipped = total。
4. skipped tests 逐条解释。
5. Default Off 注册 0 个 StS1 events。
6. CanaryOnly 精确注册 Big Fish、Golden Idol、Lab、Divine Fountain。
7. AdditiveBatch1 只包含 verified scope：4 canary + 6 simple batch，不得混同 AdditiveAllDraft。
8. AdditiveAllDraft 明确 dev-only。
9. ReplacementPrototype 明确 debug-only。
10. canonical matrix 经 QA/Red-Team 复核。
11. 4 canary runtime verified。
12. 6 simple batch runtime verified。
13. verified scope EN/ZHS render proof 完成。
14. verified scope image render/license proof 完成。
15. ReplacementPrototype functional proof 完成。
16. replacement event bag / visited ids / no-repeat / save-load proof 完成。
17. multiplayer fail-closed 或 verified behavior 完成。
18. combat events 在 encounter models 完成前保持 blocked。
19. independent QA/Red-Team pass/fail。
20. monthly review 和 handoff docs 更新。

### 7.4 月内工作分阶段

#### Week 1：Evidence Truth + Mode Scope

目标：把“代码侧通过”和“体验完成”分开。

交付：

- full build log
- full test log
- test count reconciliation
- skipped tests explanation
- mode scope doc
- status-board truth pass

验收：

- 没有 false Done。
- AdditiveBatch1 和 AdditiveAllDraft 不再混淆。

#### Week 2：Canary Runtime Proof

目标：4 canary 变成 runtime verified。

交付：

- Big Fish runtime proof
- Golden Idol runtime proof
- Lab runtime proof
- Divine Fountain runtime proof
- screenshots
- result logs
- save/load proof
- EN/ZHS render proof
- image proof or non-parity placeholder proof

#### Week 3：Simple Batch Runtime Proof

目标：6 simple batch 变成 runtime verified。

交付：

- Purifier proof
- Upgrade Shrine proof
- Golden Shrine proof
- The Cleric proof
- Old Beggar / Pleading Vagrant proof
- Shining Light proof

#### Week 4：Replacement Pool + QA

目标：解决“体验不像 StS1”的核心池问题。

交付：

- ReplacementPrototype functional proof
- event bag/no-repeat proof
- save/load proof
- Act bucket proof
- independent QA/Red-Team report
- monthly review

---

## 8. Mandatory Overnight Run v13

### 8.1 停止条件

唯一允许停止：

```text
A. O0-O46 全部 GREEN；
或
B. 输出 HARD STOP BLOCKER REPORT：
   - exact gate id
   - blocker reason
   - evidence path
   - attempted actions
   - owner action
   - why continuation is impossible in current environment
```

Hard stop 允许暂停，**不代表完成**。

### 8.2 不允许停止的条件

不能因为以下内容停止：

- build passes
- tests pass
- StS1 guard tests pass
- ZHS placeholders = 0
- status-board updated
- canonical matrix exists
- source files exist
- asset scripts exist
- replacement source exists
- hard-stop report exists
- all code-side work complete

### 8.3 O0-O46 gates

| Gate | 必须结果 |
|---|---|
| O0 | worktree snapshot：branch、HEAD、diff、unstaged files |
| O1 | full unfiltered build exit code 0，保存完整 log |
| O2 | full tests exit code 0，保存完整 log |
| O3 | test count reconciliation：passed + failed + skipped = total |
| O4 | skipped tests 逐条解释 |
| O5 | status-board 无 false Done |
| O6 | canonical matrix 完整 |
| O7 | 52/54/48/50/54 reconciliation 经 Red-Team 复核 |
| O8 | Act mapping guard passes |
| O9 | feature gate tests pass |
| O10 | Off=0 registration proof |
| O11 | CanaryOnly=4 exact identity proof |
| O12 | AdditiveBatch1 exact verified-scope proof |
| O13 | AdditiveAllDraft dev-only proof |
| O14 | ReplacementPrototype debug-only proof |
| O15 | per-event IsShared matrix |
| O16 | combat IsShared=true guard passes |
| O17 | canary code review clean |
| O18 | canary runtime screenshots complete |
| O19 | canary result logs complete |
| O20 | canary save/load proof complete |
| O21 | canary EN/ZHS render proof complete |
| O22 | canary image render/license proof complete |
| O23 | simple batch exact spec Red-Team pass |
| O24 | simple batch code review clean |
| O25 | simple batch runtime screenshots complete |
| O26 | simple batch result logs complete |
| O27 | simple batch save/load proof if applicable |
| O28 | verified-scope ZHS placeholders = 0 and render verified |
| O29 | verified-scope asset manifest complete |
| O30 | verified-scope image/license decision documented |
| O31 | replacement source guard passes |
| O32 | replacement functional proof：unknown rooms only draw StS1 candidates |
| O33 | replacement Act bucket proof：Act 1/2/3 correct |
| O34 | event bag / visited ids / no-repeat proof |
| O35 | replacement save/load proof |
| O36 | multiplayer fail-closed or verified proof |
| O37 | content parity gap matrix |
| O38 | temporary substitutes marked non-parity |
| O39 | combat blocker report current and honest |
| O40 | independent QA/Red-Team report |
| O41 | monthly review updated |
| O42 | handoff docs updated with next owner actions |
| O43 | no release-ready/full-parity claims in docs |
| O44 | owner-facing summary states incomplete runtime gates honestly |
| O45 | all hard-stop blockers map to explicit owner/external action |
| O46 | next run starts from unresolved gates, not from broad Phase 2 expansion |

---

## 9. 强制 Subagent 分工

实现者不能审核自己的工作。必须启动以下 subagents：

| Subagent | 职责 | 必须输出 |
|---|---|---|
| BuildGate / Repo Health | full build/test logs、exit codes、skipped tests、worktree、test count reconciliation | `build-full.log`、`test-full.log`、`skipped-tests.md`、`test-count-reconciliation.md` |
| Wiki Parity Spec Auditor | 52 public target、54 internal entries、exact options、A15、semi-common membership | `canonical-event-matrix.csv`、`count-reconciliation.md` |
| StS2 Source/API Auditor | ActModel、EventModel、RitsuLib、card/relic/potion/save/replacement APIs | `source-api-matrix.md` |
| Feature Gate / Registration Engineer | Off、CanaryOnly、AdditiveBatch1、AdditiveAllDraft、ReplacementPrototype | mode tests、registration identity proof |
| Canary Gameplay Subagent | Big Fish、Golden Idol、Lab、Divine Fountain runtime proof | screenshots、state logs、save/load evidence |
| Simple Batch Gameplay Subagent | Purifier、Upgrade Shrine、Golden Shrine、The Cleric、Old Beggar/Pleading Vagrant、Shining Light | screenshots、state logs、render evidence |
| Asset + Localization Subagent | ZHS render、missing key scan、image/license decision | render screenshots、asset manifest、license decision |
| Event Pool / RNG / Save Subagent | replacement pool、event bag、visited ids、save/load | functional proof logs |
| Multiplayer / IsShared Subagent | per-event IsShared matrix、combat true、fail-closed co-op | `is-shared-matrix.csv` |
| Content Parity Subagent | Bite、face relics、StS1 curses、Golden/Bloody Idol、combat encounter models | gap matrix，temporary substitute list |
| QA / Red-Team Subagent | 只验收，不实现，逐 gate pass/fail | `qa-redteam-report.md` |
| Release Documentation Subagent | status-board、monthly review、handoff、release evidence | updated docs |

---

## 10. 直接发给助理的执行指令

```text
当前状态不能标完成。你这轮 code-side work 有进展，但 runtime gameplay、save/load、event images、ReplacementPrototype functional proof、combat encounter models、independent QA 仍然 blocked 或未验证。

不要写 all tasks complete、full parity、release-ready、和杀戮尖塔1完全一样。

继续 Mandatory Overnight Run v13。你只能在 O0-O46 全绿后停止；若因为 game launch、licensed art、external QA、owner action 无法继续，必须输出 HARD STOP BLOCKER REPORT。Blocked gate 不得标完成。

最高优先级：
1. 保存最新 full build/test evidence，并修正 tests count conflict：428 passed + 21 skipped 不能等于 398 total。
2. Red-team 52/54/48/50/54 canonical matrix。
3. 保持 default Off；证明 Off=0、CanaryOnly=4 exact identity、AdditiveBatch1 only verified scope、AdditiveAllDraft dev-only。
4. 运行游戏验证 4 canary：截图、结果日志、save/load、EN/ZHS、image render。
5. 运行游戏验证 6 simple batch：截图、结果日志、EN/ZHS、image render、必要时 save/load。
6. 完成 ReplacementPrototype functional proof：unknown room 只抽 StS1 events，Act bucket 正确，save/load 保持 event bag。
7. Combat events 在 encounter models 完成前继续 blocked，不准伪完成。
8. 没有 redistributable art 时，使用 local extraction hash proof、owner licensed assets、generated replacement art，或明确标 non-parity placeholder。
9. QA/Red-Team 必须独立，不允许实现者自验。
10. 任何没有 source/API/test/screenshot/log/save-load evidence 的项不得标 Done。
```

---

## 11. 总结

当前不是失败，而是进入了正确的分界线：

- **代码基础已经推进。**
- **runtime 体验还没有证明。**
- **不要继续扩大范围。**
- **下个月重点是把 4 canary + 6 simple batch 做到 runtime verified，并证明 replacement pool。**

最终管理红线：

> 不要把 code-side completion 当 gameplay completion；不要把 automated tests 当 runtime proof；不要把 registry count 当 event parity；不要把 hard-stop blocker report 当完成；不要继续扩大事件范围。先把 4 canary + 6 simple batch 的 runtime、图片、本地化、save/load、replacement pool 跑绿。
> 结论：**当前仍未完成。**
我把这次“当前状态重新审查 + 目标对比 + 下月开发规范 + subagent 分工 + 强制 overnight run”整理成了新版文档：

[下载完整审查文档 v13](sandbox:/mnt/data/sts1_event_port_strict_audit_v13_current_state_monthly_overnight.md)

## 严格审核总判定

现在可以承认的是：**代码侧基础设施明显推进**。例如之前记录里已经完成过 feature gate 方向、正确 Act mapping、RitsuLib API 验证、部分 helper、build/test 证据与 StS1 guard tests；同时也明确过 runtime 证据、图片、combat encounter、QA 等仍被阻塞。

但这不等于任务完成。早期还出现过两个严重问题：一是把 `Sts1EventRegistrationService.RegisterAll(ModId)` 无条件接进 `MainFile.Initialize()`；二是把 StS2 Act 映射写成 `Underdocks=Act1, Overgrowth=Act2, Hive=Act3`，这会直接把事件放错章节。 之后虽然后续方向已经改为 default Off 和正确 Act mapping，但这些历史问题说明必须继续用 guard、runtime proof、Red-Team 来兜底。

更早的 status-board 还把 `Infrastructure`、`event-specs/ (48 unique events)`、`assets.md`、`localization.md`、`test-plan.md` 标成 `Done`，同时 blocker 里还列着 Regret、Injury、random relic helper、card removal/transform/upgrade UI、combat encounter models。这个“Done”口径不可信，必须改成证据状态。

## 与最终目标的差距

StS1 Wiki 的事件系统不是“注册一些 EventModel”这么简单。事件来自 unknown location，是否进入事件以及进入哪个事件由随机和当前 Act 决定；部分事件限定 Act，部分可跨 Act；Act 4 没有 unknown location/event；Ascension 15 会让部分不利事件更强或更可能发生。Wiki 列表也明确分为 16 个 shared events、12 个 Act 1 exclusive events、16 个 Act 2 exclusive events、8 个 Act 3 exclusive events。([Slay the Spire Wiki][1])

所以，`52 / 54 / 48 / 50 / 54` 这些数字必须通过 canonical matrix 逐项解释。不能把 runtime model 数、registry entries、registration calls、spec files 直接等同于“StS1 全事件完成”。

Canary 事件也不能降低标准。Big Fish 必须是 Act 1 exclusive，Banana 回复 `floor(maxHP/3)`，Donut 增加 5 Max HP，Box 给随机 common/uncommon/rare relic 并加入 Regret。([Slay the Spire Wiki][2]) Golden Idol 必须是 Act 1 exclusive，Take 获得 Golden Idol 并触发陷阱；Outrun 给 Injury，Smash 造成 25%/35% max HP 伤害，Hide 损失 8%/10% max HP，Leave 无事发生。([Slay the Spire Wiki][3])

## 当前逐项判定

| 模块                    | 当前判定                                                    |
| --------------------- | ------------------------------------------------------- |
| Build/code foundation | 有进展，但必须保留 full unfiltered build log                     |
| Tests                 | v13 已复核为 `461 passed + 0 failed + 21 skipped = 482 total`；旧 `428/398` 冲突保留为历史 false-green 风险 |
| Guard tests           | 有进展，但不能代替 runtime proof                                 |
| Default Off           | 方向正确，必须继续证明 Off=0                                       |
| CanaryOnly            | 必须证明 exact identity，不只是 count=4                         |
| AdditiveBatch1        | 命名可疑；Batch1 应是 4 canary + 6 simple batch，不应是 54 total   |
| Canonical matrix      | 声称完成，但必须 Red-Team 复核                                    |
| Canary code           | 只能算 code-side claimed complete，runtime 未完成              |
| Simple batch code     | 只能算 code-side claimed complete，runtime 未完成              |
| ZHS                   | placeholder 清零只是文件层面，仍需游戏内渲染证明                          |
| Images                | 未完成，没有 redistributable art 或 render proof               |
| ReplacementPrototype  | 未完成，source/guard 不等于 functional proof                   |
| Combat events         | blocked，encounter models 没有前不能计入 parity                 |
| QA/Red-Team           | 未完成，必须独立验收                                              |
| Full StS1 experience  | 未完成                                                     |

## 管理决策

**继续优化 + 有限推进，两者兼顾，但优化优先。**

不要继续扩大到更多 draft event。现在最重要的不是“堆更多事件类”，而是把已经进入 verified scope 的内容跑成真的 StS1-like experience：

```text
4 canary:
- Big Fish
- Golden Idol
- Lab
- Divine Fountain

6 simple batch:
- Purifier
- Upgrade Shrine
- Golden Shrine
- The Cleric
- Old Beggar / Pleading Vagrant
- Shining Light
```

暂停扩大范围：

```text
combat event full implementation
custom UI 全量实现
继续批量生成更多 draft event
full parity 宣称
release-ready 宣称
```

## 下个月开发规范

目标名称：

**`StS1 Event Port Prototype Batch 1 — Runtime Parity Foundation`**

月末 Go/No-Go 标准：

1. full unfiltered build exit code 0。
2. full test exit code 0。
3. 修正 test count：passed + failed + skipped 必须等于 total。
4. skipped tests 逐条解释。
5. Default Off 注册 0 个 StS1 events。
6. CanaryOnly 精确注册 Big Fish、Golden Idol、Lab、Divine Fountain。
7. AdditiveBatch1 只包含 verified scope：4 canary + 6 simple batch。
8. AdditiveAllDraft 明确 dev-only。
9. ReplacementPrototype 明确 debug-only。
10. `52/54/48/50/54` canonical matrix 经 Red-Team 复核。
11. 4 个 canary runtime verified。
12. 6 个 simple batch runtime verified。
13. verified scope EN/ZHS render proof 完成。
14. verified scope image render/license proof 完成。
15. ReplacementPrototype functional proof 完成：unknown room 只抽 StS1 candidates，Act bucket 正确，event bag/save-load 正确。
16. multiplayer fail-closed 或 verified behavior 完成。
17. combat events 在 encounter models 完成前保持 blocked。
18. independent QA/Red-Team 给 pass/fail。
19. monthly review 和 handoff docs 更新。

项目边界继续保持：`Spire Plus` 是单一 active mod，technical manifest id 是 `EZMicroBalance`，代码和资源路径分别是 `EZMicroBalanceCode/` 与 `EZMicroBalance/`。 原版素材不能随意提交，原版 art 只有授权确认后才能进入 tracked/public files。

## 强制 Overnight Run v13

停止条件只能是：

```text
A. O0-O46 全部 GREEN；
或
B. 输出 HARD STOP BLOCKER REPORT：
   - exact gate id
   - blocker reason
   - evidence path
   - attempted actions
   - owner action
   - why continuation is impossible in current environment
```

不能因为这些就停止：

```text
build passes
tests pass
StS1 guard tests pass
ZHS placeholders = 0
canonical matrix exists
source files exist
asset scripts exist
replacement source exists
hard-stop report exists
all code-side work complete
```

核心 gates：

```text
O1 full unfiltered build log
O2 full test log
O3 test count reconciliation
O5 status-board no false Done
O7 52/54/48/50/54 Red-Team reconciliation
O11 CanaryOnly=4 exact identity proof
O12 AdditiveBatch1 verified-scope proof
O18-O22 canary runtime/log/save-load/loc/image proof
O25-O27 simple batch runtime/log/save-load proof
O32-O35 replacement functional/Act bucket/event bag/save-load proof
O40 independent QA/Red-Team report
O46 next run starts from unresolved gates, not broad Phase 2 expansion
```

## 必须使用 subagent

让助理必须启动这些 subagent，而且实现者不能审核自己的工作：

1. **BuildGate / Repo Health**：build/test logs、exit codes、skipped tests、test count reconciliation。
2. **Wiki Parity Spec Auditor**：52 public target、54 internal entries、exact options、A15、semi-common membership。
3. **StS2 Source/API Auditor**：ActModel、EventModel、RitsuLib、card/relic/potion/save/replacement API。
4. **Feature Gate / Registration Engineer**：Off、CanaryOnly、AdditiveBatch1、AdditiveAllDraft、ReplacementPrototype。
5. **Canary Gameplay Subagent**：Big Fish、Golden Idol、Lab、Divine Fountain runtime proof。
6. **Simple Batch Gameplay Subagent**：Purifier、Upgrade Shrine、Golden Shrine、The Cleric、Old Beggar/Pleading Vagrant、Shining Light runtime proof。
7. **Asset + Localization Subagent**：ZHS render、missing key scan、image/license decision。
8. **Event Pool / RNG / Save Subagent**：replacement pool、event bag、visited ids、save/load。
9. **Multiplayer / IsShared Subagent**：per-event `IsShared` matrix、combat true、fail-closed co-op。
10. **Content Parity Subagent**：Bite、face relics、StS1 curses、Golden/Bloody Idol、combat encounter models。
11. **QA / Red-Team Subagent**：只验收，不实现，逐 gate pass/fail。
12. **Release Documentation Subagent**：status-board、monthly review、handoff、release evidence。

## 可以直接发给他的指令

```text
当前状态不能标完成。你这轮 code-side work 有进展，但 runtime gameplay、save/load、event images、ReplacementPrototype functional proof、combat encounter models、independent QA 仍然 blocked 或未验证。

不要写 all tasks complete、full parity、release-ready、和杀戮尖塔1完全一样。

继续 Mandatory Overnight Run v13。你只能在 O0-O46 全绿后停止；若因为 game launch、licensed art、external QA、owner action 无法继续，必须输出 HARD STOP BLOCKER REPORT。Blocked gate 不得标完成。

最高优先级：
1. 保存最新 full build/test evidence，并修正 tests count conflict。
2. Red-team 52/54/48/50/54 canonical matrix。
3. 保持 default Off；证明 Off=0、CanaryOnly=4 exact identity、AdditiveBatch1 only verified scope、AdditiveAllDraft dev-only。
4. 运行游戏验证 4 canary：截图、结果日志、save/load、EN/ZHS、image render。
5. 运行游戏验证 6 simple batch：截图、结果日志、EN/ZHS、image render、必要时 save/load。
6. 完成 ReplacementPrototype functional proof：unknown room 只抽 StS1 events，Act bucket 正确，save/load 保持 event bag。
7. Combat events 在 encounter models 完成前继续 blocked，不准伪完成。
8. 没有 redistributable art 时，使用 local extraction hash proof、owner licensed assets、generated replacement art，或明确标 non-parity placeholder。
9. QA/Red-Team 必须独立，不允许实现者自验。
10. 任何没有 source/API/test/screenshot/log/save-load evidence 的项不得标 Done。
```

管理红线：**不要把 code-side completion 当 gameplay completion；不要把 automated tests 当 runtime proof；不要把 registry count 当 event parity；不要把 hard-stop blocker report 当完成；不要继续扩大范围。先把 4 canary + 6 simple batch 的 runtime、图片、本地化、save/load、replacement pool 跑绿。**

[1]: https://slay-the-spire.fandom.com/wiki/Events "Events | Slay the Spire Wiki | Fandom"
[2]: https://slay-the-spire.fandom.com/wiki/Big_Fish "Big Fish | Slay the Spire Wiki | Fandom"
[3]: https://slay-the-spire.fandom.com/wiki/Golden_Idol_%28Event%29 "Golden Idol (Event) | Slay the Spire Wiki | Fandom"
