# StS1 Event Port 严格审查 v14：当前状态、目标差距、下月开发规范、Subagent 分工与 Mandatory Overnight Run

## Revision I 当前真相（2026-05-31）

```text
HEAD：87820303 (HEAD -> main, origin/main, origin/HEAD) sprint 1
Worktree：dirty；本文件是 current/governance doc，不是 release proof
Build：PASS，0 errors / 89 Sts1Events nullable warnings
Tests：project no-build PASS，464 passed / 0 failed / 21 skipped / 485 total
Runtime：HARD BLOCKED，STS2-RitsuLib 缺失，godot.log 缺失
StS1Events：staging-only；Off=0 和 CanaryOnly=4 仍缺 runtime log proof
Batch 4c / high-risk migration / new gameplay：blocked
Release-ready：no
```

下面旧审查中的 428/449、0-warning、GREEN 口径只作为待纠正历史风险；当前状态以上方和 `docs/reviews/current-validation.md` 为准。

## 0. 总结判定

当前任务 **未完成**。

可以承认的进展是：代码侧基础设施、feature gate、部分注册 guard、部分事件模型、部分 helper、ZHS placeholder 清理与自动测试有明显推进。

不能承认的是：StS1 event experience parity、runtime gameplay、save/load、图片/授权/渲染、ReplacementPrototype 功能验证、combat encounter models、独立 QA/Red-Team 尚未完成。

因此，本轮管理决策是：**继续优化 + 有限推进，两者兼顾，但优化优先**。

不要继续扩大到更多 draft 事件。先把 verified scope 做成真正可玩的、可截图验证的 StS1-like experience：

- 4 个 Canary：Big Fish、Golden Idol、Lab、Divine Fountain
- 6 个 Simple Batch：Purifier、Upgrade Shrine、Golden Shrine、The Cleric、Old Beggar / Pleading Vagrant、Shining Light

## 1. 项目边界

当前 active deliverable 仍是单一 Spire Plus mod；technical manifest id 仍是 `EZMicroBalance`。代码路径保持在 `EZMicroBalanceCode/`，资源和本地化路径保持在 `EZMicroBalance/`。

严禁把 StS1 event port 作为默认污染 Spire Plus 的功能。默认模式必须是 Off，除非显式设置 `SPIREPLUS_STS1_EVENT_MODE`。

资源红线：不要把未授权的原版 StS1 / StS2 素材直接提交到 tracked/public files。没有授权时，只允许：

1. 本地抽取并只在本机使用；
2. owner 提供授权素材；
3. 使用生成/自有/可再分发替代素材；
4. 明确标注 non-parity placeholder。

## 2. StS1 目标定义

StS1 事件不是“注册 N 个 EventModel”这么简单。目标是复刻 StS1 事件体验：

- unknown room 事件池；
- shared / semi-common / act-exclusive bucket；
- Act 1 / Act 2 / Act 3 出现范围；
- 选项流程、页面跳转、锁定条件；
- 奖励、卡牌、遗物、诅咒、药水；
- A15 数值变化；
- 图片和 EN/ZHS 文本；
- save/load 与 event bag / visited ids；
- multiplayer / IsShared；
- 默认不污染 Spire Plus。

Wiki public target 按 52 listed event entries 管理：16 shared、12 Act 1、16 Act 2、8 Act 3。内部 54 / 48 / 50 / 54 统计必须用 canonical matrix 解释，不能直接当作“全事件完成”。

## 3. 历史问题和当前风险

### 3.1 错误完成口径

早期状态板把 Infrastructure、event-specs、assets.md、localization.md、test-plan.md 标成 Done，但同一状态里仍列着 Regret、Injury、random relic helper、card UI、combat encounter models 等 blocker。这个 Done 口径不可信，必须废弃。

### 3.2 无条件注册风险

早期把 `Sts1EventRegistrationService.RegisterAll(ModId)` 接进 `MainFile.Initialize()`，这会让未验证的 StS1 事件默认进入 Spire Plus。现在必须继续保持 feature gate：

- Off：默认，注册 0 个；
- CanaryOnly：只注册 4 个 canary；
- AdditiveBatch1：只注册 verified scope 的 10 个；
- AdditiveAllDraft：dev-only，必须额外设置 `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`，不是玩家默认；
- ReplacementPrototype：debug-only，必须有 `REPLACEMENT_PROTOTYPE_ENABLED` 编译符号和 `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`，不是默认。

### 3.3 Act mapping 风险

早期错误写法是 `Underdocks = Act 1, Overgrowth = Act 2, Hive = Act 3`。正确方向应是：

| StS1 bucket | StS2 ActModel |
|---|---|
| Act 1 | Overgrowth + Underdocks |
| Act 2 | Hive |
| Act 3 | Glory |

该 mapping 必须有 source/API evidence 和 guard test。没有 runtime proof 之前，不能只靠注释判断。

### 3.4 Additive registration 不等于 StS1 体验

RitsuLib additive registration 只说明事件被加入候选池，不代表 unknown room 只抽 StS1 事件，也不代表 event bag、去重、Act bucket、A15、RNG、save/load、co-op 一致。

ReplacementPrototype 必须给出功能证据，而不仅是 source file 或 guard test。

## 4. 当前逐项审核

| 模块 | 当前结论 | 严格判定 |
|---|---|---|
| Build | 声称 0 errors / 0 warnings | 可作为代码侧进展，但必须保留 full unfiltered log |
| Tests | 声称 428 passed / 0 failed / 21 skipped，但有 total 数字冲突 | 必须复核；passed + failed + skipped 必须等于 total |
| Guard tests | StS1 guard tests claimed pass | 有价值，但不能代替 runtime proof |
| Default Off | claimed DONE | 方向正确，必须保留 Off=0 registration proof |
| CanaryOnly | claimed 4 | 必须证明 exact identity，不只是 count |
| AdditiveBatch1 | 有“54 total”口径 | 可疑；Batch1 应为 verified scope 10 个，不应混同 AdditiveAllDraft |
| Canonical matrix | claimed DONE | 必须 Red-Team 复核 52/54/48/50/54 |
| Canary code | claimed complete | 只能算 code-side claimed complete，runtime 未完成 |
| Simple batch code | claimed complete | 只能算 code-side claimed complete，runtime 未完成 |
| ZHS | claimed 399 keys / 0 placeholders | 文件层面进展；仍需游戏内渲染 proof |
| Images | blocked | 未完成；无授权 art 或 render proof |
| Runtime gameplay | blocked | 未完成 |
| Save/load | blocked | 未完成 |
| ReplacementPrototype | blocked | 未完成；source/guard 不等于 functional proof |
| Combat events | blocked | encounter models 不存在前不能计入 parity |
| QA/Red-Team | blocked | 未完成；必须独立验收 |
| Full StS1 experience | 未证明 | 未完成 |

## 5. 目标对比与管理决策

目标是 StS1 event experience parity foundation；当前是 code-side foundation advanced。

差距集中在：

1. Runtime gameplay evidence；
2. Save/load evidence；
3. Replacement event pool functional proof；
4. Image/render/license proof；
5. EN/ZHS in-game render proof；
6. Combat encounter models；
7. Independent QA/Red-Team；
8. Count/mode naming truth。

因此：

- **继续优化**：build/test evidence、matrix、status truth、feature gate、mode naming、asset/license、QA。
- **有限推进**：只推进 4 canary + 6 simple batch 的 runtime proof。
- **暂停扩大范围**：combat full implementation、custom UI full implementation、继续批量生成 draft、full parity 宣称、release-ready 宣称。

## 6. 下个月开发规范

目标名称：`StS1 Event Port Prototype Batch 1 — Runtime Parity Foundation`

### 6.1 月末 Go / No-Go 标准

1. 最新 full unfiltered build exit code 0。
2. 最新 full tests exit code 0。
3. 修正 test count conflict：passed + failed + skipped = total。
4. skipped tests 逐条解释。
5. Default Off 注册 0 个 StS1 events。
6. CanaryOnly 精确注册 Big Fish、Golden Idol、Lab、Divine Fountain。
7. AdditiveBatch1 只包含 verified scope：4 canary + 6 simple batch。
8. AdditiveAllDraft 明确 dev-only。
9. ReplacementPrototype 明确 debug-only。
10. 52/54/48/50/54 canonical matrix 经 Red-Team 复核。
11. 4 个 canary runtime verified。
12. 6 个 simple batch runtime verified。
13. verified scope 的 EN/ZHS render proof 完成。
14. verified scope 的 image render/license proof 完成。
15. ReplacementPrototype functional proof 完成：unknown room 只抽 StS1 candidates，Act bucket 正确，event bag/save-load 正确。
16. Multiplayer fail-closed 或 verified behavior 完成。
17. Combat events 在 encounter models 完成前保持 blocked。
18. Independent QA/Red-Team 给 pass/fail。
19. Monthly review、handoff docs、release evidence 更新。

### 6.2 周计划

#### Week 1：Truth + Evidence Freeze

- 固化 build/test full log。
- 修复 test count conflict。
- 清理 status-board 所有 false Done。
- Red-Team canonical matrix。
- 明确 AdditiveBatch1 vs AdditiveAllDraft。

#### Week 2：Canary Runtime Verification

- 游戏内验证 Big Fish、Golden Idol、Lab、Divine Fountain。
- 每个事件必须有 screenshot、pre/post state log、save/load proof、EN/ZHS render proof、image/render proof。

#### Week 3：Simple Batch Runtime Verification

- 验证 Purifier、Upgrade Shrine、Golden Shrine、The Cleric、Old Beggar/Pleading Vagrant、Shining Light。
- 保存截图、日志、文本渲染和必要的 save/load proof。

#### Week 4：Replacement Pool + Asset/Loc + QA

- ReplacementPrototype 功能验证。
- Event bag / visited ids / no-repeat / save-load proof。
- Asset/license 策略落地。
- Independent QA/Red-Team pass/fail。
- Handoff docs 和 monthly review 更新。

## 7. Mandatory Overnight Run v14

停止条件只能是：

A. O0-O50 全部 GREEN；

或

B. 输出 HARD STOP BLOCKER REPORT，必须包含：exact gate id、blocker reason、evidence path、attempted actions、owner action、why continuation is impossible in current environment。

Hard stop 只允许暂停，不代表完成。

### 7.1 不允许因为这些就停止

- build passes；
- tests pass；
- StS1 guard tests pass；
- ZHS placeholders = 0；
- canonical matrix exists；
- source files exist；
- asset scripts exist；
- replacement source exists；
- hard-stop report exists；
- all code-side work complete。

### 7.2 O0-O50 Gates

| Gate | 必须结果 |
|---|---|
| O0 | Worktree snapshot：branch、HEAD、diff、unstaged files |
| O1 | Full unfiltered build log，exit code 0 |
| O2 | Full test log，exit code 0 |
| O3 | Test count reconciliation：passed + failed + skipped = total |
| O4 | Skipped tests 逐条解释 |
| O5 | Status-board 无 false Done |
| O6 | Canonical matrix 完整 |
| O7 | 52/54/48/50/54 reconciliation 经 Red-Team 复核 |
| O8 | Act mapping guard passes |
| O9 | Feature gate tests pass |
| O10 | Off=0 registration proof |
| O11 | CanaryOnly=4 exact identity proof |
| O12 | AdditiveBatch1 exact verified-scope proof |
| O13 | AdditiveAllDraft dev-only proof |
| O14 | ReplacementPrototype debug-only proof |
| O15 | Per-event IsShared matrix |
| O16 | Combat IsShared=true guard passes |
| O17 | Canary code review clean |
| O18 | Canary runtime screenshots complete |
| O19 | Canary result logs complete |
| O20 | Canary save/load proof complete |
| O21 | Canary EN/ZHS render proof complete |
| O22 | Canary image render/license proof complete |
| O23 | Simple batch exact spec Red-Team pass |
| O24 | Simple batch code review clean |
| O25 | Simple batch runtime screenshots complete |
| O26 | Simple batch result logs complete |
| O27 | Simple batch save/load proof if applicable |
| O28 | Verified-scope ZHS placeholders = 0 and render verified |
| O29 | Verified-scope asset manifest complete |
| O30 | Verified-scope image/license decision documented |
| O31 | Replacement source guard passes |
| O32 | Replacement functional proof：unknown rooms only draw StS1 candidates |
| O33 | Replacement Act bucket proof：Act 1/2/3 correct |
| O34 | Event bag / visited ids / no-repeat proof |
| O35 | Replacement save/load proof |
| O36 | Multiplayer fail-closed or verified proof |
| O37 | Content parity gap matrix |
| O38 | Temporary substitutes marked non-parity |
| O39 | Combat blocker report current and honest |
| O40 | Independent QA/Red-Team report |
| O41 | Canary owner-facing screenshots bundle |
| O42 | Simple batch screenshots bundle |
| O43 | Replacement proof bundle |
| O44 | Monthly review updated |
| O45 | Handoff docs updated |
| O46 | Release evidence status updated |
| O47 | Next owner actions listed |
| O48 | No full-parity / release-ready wording remains |
| O49 | Commit boundary documented |
| O50 | Next run starts from unresolved gates, not broad Phase 2 expansion |

## 8. 必须使用 Subagent

实现者不能审核自己的工作。至少使用以下 subagents：

| Subagent | 职责 | 输出 |
|---|---|---|
| BuildGate / Repo Health | build/test logs、exit codes、skipped tests、test count reconciliation | build-full.log、test-full.log、skipped-tests.md |
| Wiki Parity Spec Auditor | 52 public target、54 internal entries、exact options、A15、semi-common membership | canonical-event-matrix.csv、count-reconciliation.md |
| StS2 Source/API Auditor | ActModel、EventModel、RitsuLib、card/relic/potion/save/replacement API | source-api-matrix.md |
| Feature Gate / Registration Engineer | Off、CanaryOnly、AdditiveBatch1、AdditiveAllDraft、ReplacementPrototype | registration mode tests |
| Canary Gameplay Subagent | Big Fish、Golden Idol、Lab、Divine Fountain runtime proof | screenshots、logs、save/load |
| Simple Batch Gameplay Subagent | Purifier、Upgrade Shrine、Golden Shrine、The Cleric、Old Beggar/Pleading Vagrant、Shining Light | screenshots、logs |
| Asset + Localization Subagent | ZHS render、missing key scan、image/license decision | render screenshots、asset manifest |
| Event Pool / RNG / Save Subagent | replacement pool、event bag、visited ids、save/load | functional proof |
| Multiplayer / IsShared Subagent | per-event IsShared matrix、combat true、fail-closed co-op | is-shared-matrix.csv |
| Content Parity Subagent | Bite、face relics、StS1 curses、Golden/Bloody Idol、combat encounter models | gap matrix |
| QA / Red-Team Subagent | 只验收，不实现，逐 gate pass/fail | qa-redteam-report.md |
| Release Documentation Subagent | status-board、monthly review、handoff、release evidence | updated docs |

## 9. 直接发给助理的指令

```text
当前状态不能标完成。code-side work 有进展，但 runtime gameplay、save/load、event images、ReplacementPrototype functional proof、combat encounter models、independent QA 仍然 blocked 或未验证。

不要写 all tasks complete、full parity、release-ready、和杀戮尖塔1完全一样。

继续 Mandatory Overnight Run v14。你只能在 O0-O50 全绿后停止；若因为 game launch、licensed art、external QA、owner action 无法继续，必须输出 HARD STOP BLOCKER REPORT。Blocked gate 不得标完成。

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

## 10. 最终管理红线

不要把 code-side completion 当 gameplay completion；不要把 automated tests 当 runtime proof；不要把 registry count 当 event parity；不要把 hard-stop blocker report 当完成；不要继续扩大范围。先把 4 canary + 6 simple batch 的 runtime、图片、本地化、save/load、replacement pool 跑绿。
结论：**当前仍未完成。**
我把这次“严格审查 + 目标对比 + 下月开发规范 + subagent 分工 + 强制 Overnight Run v14”整理成了完整文档：

[下载完整审查文档 v14](sandbox:/mnt/data/sts1_event_port_strict_audit_v14_monthly_overnight.md)

## 严格审核总判定

当前状态只能算 **code-side foundation 有进展**，不能算 **StS1 事件体验迁移完成**。

可以暂时认可的进展：

| 模块               | 当前判断                                                                    |
| ---------------- | ----------------------------------------------------------------------- |
| Feature gate     | 已有正确方向：`SPIREPLUS_STS1_EVENT_MODE`，默认 Off                               |
| Act mapping      | 后续方向已修正为 `Overgrowth + Underdocks = Act 1, Hive = Act 2, Glory = Act 3` |
| RitsuLib API     | 已验证 additive 注册路径                                                       |
| Helper 层         | potion、gold loss、card removal/upgrade/transform、rare card/relic 等有实现进展  |
| Guard tests      | 有推进                                                                     |
| ZHS placeholder  | 文件层面声称已清零                                                               |
| Canonical matrix | 声称已完成，但仍需 Red-Team 复核                                                   |

不能认可为完成的部分：

| 模块                   | 严格判定                                       |
| -------------------- | ------------------------------------------ |
| Runtime gameplay     | 未完成，需要游戏内截图和结果日志                           |
| Save/load            | 未完成                                        |
| Images/render proof  | 未完成，没有 redistributable art 或本地抽取/render 证据 |
| ReplacementPrototype | 未完成，source/guard 不等于 functional proof      |
| Combat events        | blocked，encounter models 缺失前不能算 parity     |
| QA/Red-Team          | 未完成，必须独立验收                                 |
| Full StS1 experience | 未证明，不能宣称完成                                 |

早期状态里曾把 `Infrastructure`、`event-specs`、`assets.md`、`localization.md`、`test-plan.md` 标成 `Done`，但同一状态里还列着 Regret、Injury、random relic helper、card UI、combat encounter models 等 blocker；这个 `Done` 口径必须废弃。 早期还出现过无条件把 `Sts1EventRegistrationService.RegisterAll(ModId)` 接进 `MainFile.Initialize()`，并且把 StS2 Act 映射写错成 `Underdocks=Act1, Overgrowth=Act2, Hive=Act3`；这些历史错误说明必须继续用 feature gate、source guard、runtime proof 和 Red-Team 验收兜底。

## 与最终目标的差距

StS1 事件不是“注册一些 EventModel”。Wiki 明确说明事件来自 unknown location，事件是否出现和出现哪个事件由随机与当前 Act 决定；有些事件只在特定 Act 出现，有些可在多个 Act 出现；Act 4 没有 unknown location/event；Ascension 15 会让部分不利事件更可能或更强。Wiki 列表还给出 16 个 shared events、12 个 Act 1 exclusive events、16 个 Act 2 exclusive events、8 个 Act 3 exclusive events。([slay-the-spire.fandom.com][1])

所以，`52 / 54 / 48 / 50 / 54` 这些数字必须由 canonical matrix 逐项解释。不能把 runtime model 数、registry entries、registration calls、spec files 直接等同于“StS1 全事件完成”。

Canary 的标准也不能降低。Big Fish 必须是 Act 1 exclusive，并实现 Banana 回复 `floor(maxHP/3)`、Donut 增加 5 Max HP、Box 给随机 common/uncommon/rare relic 并加入 Regret。([slay-the-spire.fandom.com][2]) Golden Idol 必须是 Act 1 exclusive，并实现 Take 获得 Golden Idol 后触发陷阱；Outrun 给 Injury，Smash 造成 25%/35% max HP 伤害，Hide 损失 8%/10% max HP，Leave 无事发生。([slay-the-spire.fandom.com][3])

## 当前逐项判定

| 模块                   | 当前判定                                                    |
| -------------------- | ------------------------------------------------------- |
| Build                | 有进展，但必须保存 full unfiltered build log                     |
| Tests                | 有进展，但 `428 passed + 21 skipped != 398 total` 这类数字冲突必须复核 |
| Guard tests          | 有进展，但不能代替 runtime proof                                 |
| Default Off          | 方向正确，必须继续证明 Off=0                                       |
| CanaryOnly           | 必须证明 exact identity，不只是 count=4                         |
| AdditiveBatch1       | 命名仍可疑；Batch1 应是 4 canary + 6 simple batch，不应是 54 total  |
| Canonical matrix     | 声称完成，但必须 Red-Team 复核                                    |
| Canary code          | 只能算 code-side claimed complete，runtime 未完成              |
| Simple batch code    | 只能算 code-side claimed complete，runtime 未完成              |
| ZHS                  | placeholder 清零只是文件层面，仍需游戏内渲染证明                          |
| Images               | 未完成，没有 redistributable art 或 render proof               |
| ReplacementPrototype | 未完成，source/guard 不等于 functional proof                   |
| Combat events        | blocked，encounter models 没有前不能计入 parity                 |
| QA/Red-Team          | 未完成，必须独立验收                                              |
| Full StS1 experience | 未完成                                                     |

## 管理决策

**继续优化 + 有限推进，两者兼顾，但优化优先。**

不要继续扩大到更多 draft event。现在最重要的不是“堆更多事件类”，而是把已经进入 verified scope 的内容跑成真正的 StS1-like experience：

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

## Mandatory Overnight Run v14

停止条件只能是：

```text
A. O0-O50 全部 GREEN；
或
B. 输出 HARD STOP BLOCKER REPORT：
   - exact gate id
   - blocker reason
   - evidence path
   - attempted actions
   - owner action
   - why continuation is impossible in current environment
```

Hard stop 只允许暂停，**不代表完成**。

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
O50 next run starts from unresolved gates, not broad Phase 2 expansion
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
当前状态不能标完成。code-side work 有进展，但 runtime gameplay、save/load、event images、ReplacementPrototype functional proof、combat encounter models、independent QA 仍然 blocked 或未验证。

不要写 all tasks complete、full parity、release-ready、和杀戮尖塔1完全一样。

继续 Mandatory Overnight Run v14。你只能在 O0-O50 全绿后停止；若因为 game launch、licensed art、external QA、owner action 无法继续，必须输出 HARD STOP BLOCKER REPORT。Blocked gate 不得标完成。

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
