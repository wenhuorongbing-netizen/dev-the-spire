# StS1 Event Port 严格审核 v8 + June Monthly Dev Spec + Mandatory Overnight Run

日期：2026-05-29
项目：`dev-the-spire` / `Spire Plus` / technical manifest id `EZMicroBalance`
审核对象：最新 session summary、此前 overnight gates、StS1 event port 目标与当前实现状态

---

## 0. 总结论

**没有完成。**

这次 session 可以承认一个局部完成项：**stash incident recovery / multiplayer IsShared guard recovery 基本完成**。他重新应用了 6 个 combat event 的 `IsShared => true` override，增加了 6 个 guard tests，并报告 full suite 为 `361 passed, 0 failed, 21 skipped`。这说明代码健康度和回归测试有明显进展。

但这不等于 StS1 事件迁移完成，更不等于“和杀戮尖塔 1 完全一样”。最新 summary 自己仍列出这些 pending：

- runtime gameplay verification requires game launch；
- ZHS localization 仍有 38 placeholder entries；
- combat encounter models for 7 blocked events；
- event images；
- replacement pool 只有 structure/file guard，不是 functional proof；
- 还没有 canary save/load、截图、实际 unknown room 抽取、事件池替换证明。

因此当前状态应写为：

```text
Stash recovery: completed / verified by tests.
StS1 event port: not complete.
Parity foundation: partially improved.
Runtime parity: unverified.
Full StS1 experience: not achieved.
```

---

## 1. 当前状况 vs 最终目标

### 1.1 最终目标

用户目标是：在 StS2 `Spire Plus` mod 中迁移 StS1 全事件，使事件、图片、文本、选项、奖励、伤害、A15 差异、事件池、抽取节奏、保存读取和整体体验尽量与《杀戮尖塔 1》一致。

### 1.2 当前状况

当前完成的是基础设施和部分测试防线，不是体验迁移：

| Area | 当前状态 | 审核结论 |
| --- | --- | --- |
| Stash recovery | 6 个 combat `IsShared` override 重新应用；guard tests 增加 | **局部完成** |
| Build/test | summary 报告 361 pass / 0 fail / 21 skipped | **代码健康度通过，但不是 runtime parity** |
| Feature gate | Off mode guard 存在 | **部分通过，还需 runtime registration proof** |
| Registry count | guard 写 `RegistryEntryCountIs48` | **需解释 48 runtime entries vs 52 wiki target** |
| Shared/combat IsShared | shared + combat guard 有进展 | **部分完成，仍需完整 per-event matrix** |
| Replacement prototype | 只验证 source exists / gated structure | **未完成，缺 functional proof** |
| Canary gameplay | 未报告 playable proof | **未完成** |
| ZHS localization | 38 placeholders | **未完成** |
| Event images | 未完成 | **未完成** |
| Combat events | 7 blocked encounter models | **未完成** |
| Runtime verification | 需要 game launch | **未完成** |
| Full StS1 parity | 未证明 | **0% release-ready** |

### 1.3 管理判断

**策略：优化与推进两者兼顾，但必须 gate-first。**

不是回到纯文档，也不是继续盲目批量写事件。正确路线是：

1. **先优化基础真实性**：status、matrix、feature gate、registry count、IsShared matrix、build/test evidence；
2. **同时推进有限 playable 范围**：只推进 4 个 canary + 6 个 simple batch；
3. **暂缓 full parity 宣称**：combat events、custom UI、replacement pool、images、ZHS 没完成前不得写 all done。

---

## 2. 严格逐步审核

### Step A — Stash incident recovery

**他声称：** 已恢复 6 个 combat events 的 `IsShared => true` override，并新增 guard tests。

**审核：PASS, scope-limited。**

这只说明 stash incident 相关变更已恢复，不能扩展为全事件迁移完成。

验收保留条件：

- `CombatEventsDeclareIsSharedTrue` 通过；
- `AllSharedEventModelsDeclareIsSharedTrue` 通过；
- 6 个 combat events 文件确实有 override；
- 这 6 个事件不得被标为 playable，除非 encounter model 和 combat runtime proof 完成。

### Step B — Build and test claim

**他声称：** full suite `361 passed, 0 failed, 21 skipped`。

**审核：AMBER/GREEN。**

可以接受为 automated test baseline clean，但它不能证明：

- 游戏能启动；
- unknown room 抽取正确；
- 图片加载正确；
- EN/ZHS 渲染正确；
- save/load 正确；
- multiplayer voting / independent choice 正确；
- StS1 rewards / relics / curses / card pools 体验一致。

必须保留完整 evidence：

```powershell
dotnet build --no-restore *> .tools/runtime-evidence/sts1-events-overnight-202606/o1-build-full.log
dotnet test --no-build *> .tools/runtime-evidence/sts1-events-overnight-202606/o1-test-full.log
```

### Step C — Status-board rewrite

**他声称：** status-board 已更新。

**审核：AMBER。**

必须检查是否还存在泛化 `Done`。允许状态只能是：

```text
planned
spec-drafted
wiki-verified
source-verified
api-verified
implemented
compiled
test-guarded
asset-mapped
asset-verified
loc-filed
loc-render-verified
manual-verified
save-load-verified
blocked
temporary-substitute
```

禁止：

```text
Code Done
Localization Done
Assets Done
All events Done
Build passes => complete
```

### Step D — Registry count and canonical target

**他当前 guard：** `RegistryEntryCountIs48`。

**审核：AMBER/RED until explained。**

StS1 Wiki target 应以 52 listed event entries 管理；runtime model/registry 可以合并 shared/semi-common 或特殊 entries，但必须解释。不能用 48 registry count 取代 52 target。

必须建立：

```text
docs/features/sts1-events/canonical-event-matrix.csv
```

必填字段：

```csv
wiki_entry_id,wiki_name,wiki_bucket,st1_acts_allowed,runtime_model,registry_entries,sts2_acts_registered,is_shared,status,parity_gap,proof_paths
```

验收：

- 52 wiki entries 全部出现；
- 48 registry entries 的合并逻辑逐项解释；
- 任何 skipped/special/non-runtime entry 必须有 owner decision；
- tests 不得把 48 误写为 full parity。

### Step E — Feature gate

**他当前进展：** Off mode returns immediately with zero registrations。

**审核：PARTIAL PASS。**

还需要证明：

```text
Off -> 0 registrations, default behavior zero impact
CanaryOnly -> exactly 4 events
AdditiveBatch1/AdditiveAllDraft -> only explicitly enabled
ReplacementPrototype -> debug-only, multiplayer fail-closed
```

验收测试必须覆盖 registration count 和 mode parsing。runtime log 也要打印 mode。

### Step F — Act mapping

正确 mapping 仍必须固定为：

```text
StS1 Act 1 -> Overgrowth + Underdocks
StS1 Act 2 -> Hive
StS1 Act 3 -> Glory
Shared -> shared registry
Semi-common -> exact allowed act memberships
```

任何 `Underdocks=Act1, Overgrowth=Act2, Hive=Act3` 的旧注释或文档都必须删除。

### Step G — IsShared / multiplayer

**当前进展：** shared models 和 6 combat events 有 guard。

**审核：PARTIAL PASS。**

必须输出完整 matrix：

```text
docs/features/sts1-events/multiplayer-is-shared-matrix.md
```

每个事件逐项写：

```text
is_shared=true/false
reason
co-op behavior
RNG ownership
save/load impact
test evidence
```

Combat events 必须 `IsShared=true`，但这不等于 combat event playable。

### Step H — Canary playable proof

必须先验收 4 个 canary：

- Big Fish；
- Golden Idol；
- Lab；
- Divine Fountain。

每个 canary 必须有：

```text
source/API proof
implemented code
EN/ZHS rendered text screenshot
image screenshot
option result log
save/load proof
manual verification row
```

没有截图和 run log，不得标 `manual-verified`。

### Step I — ZHS localization

**当前 pending：** 38 placeholder entries。

**审核：RED。**

所有 `待翻译` 必须清零。ZHS 不是文件存在就完成；必须做到：

- no placeholder test；
- in-game render screenshot；
- dynamic variables render correctly；
- option text length fits UI。

### Step J — Event images

**当前 pending：** event images。

**审核：RED。**

因为项目约束不允许随意提交原版素材，必须走本地抽取或可再分发替代素材路径。图片完成需要：

```text
asset manifest row
local extraction/copy proof
file existence/hash proof
runtime load screenshot
fallback placeholder documented
```

### Step K — Combat encounter models

**当前 pending：** 7 blocked combat encounter models。

**审核：RED/BLOCKED。**

这些事件只能写：

```text
blocked: missing encounter model
```

不能写 implemented/playable。要进入 playable 必须补：

- encounter model；
- monster models；
- start combat path；
- reward/return path；
- IsShared co-op proof；
- save/load proof。

### Step L — Replacement pool / StS1 experience

**当前进展：** only prototype source exists with correct structure。

**审核：RED。**

文件存在不等于 replacement works。必须证明 debug mode 下 unknown room 只抽 StS1 event pool，不混入 StS2 原事件。

验收：

```text
seeded run proof
act bucket proof
visited/no-repeat proof
save/load bag proof
multiplayer fail-closed proof
```

---

## 3. June 2026 Monthly Dev Spec

### 3.1 月目标名称

```text
StS1 Event Port Prototype Batch 1 — Parity Foundation
```

禁止使用：

```text
full parity
all 52 events complete
release-ready
和杀戮尖塔1完全一样
```

### 3.2 月末验收标准

必须全部满足：

1. full unfiltered build exit code 0；
2. full automated suite 0 failed，skipped tests 有解释；
3. default Off 对 Spire Plus 零影响；
4. CanaryOnly 精确注册 4 个事件；
5. 52-entry canonical matrix 完成，并解释 48 runtime registry count；
6. Act mapping 有 test guard；
7. per-event IsShared matrix 完成；
8. 4 个 canary playable + save/load + image + EN/ZHS render proof；
9. 6 个 simple batch playable：Purifier、Upgrade Shrine、Golden Shrine、The Cleric、Old Beggar/Pleading Vagrant、Shining Light；
10. ZHS placeholders 清零；
11. image manifest + local extraction proof 完成；
12. ReplacementPrototype functional proof：unknown room 不抽 StS2 原事件；
13. combat blocked events 有 blocker report，不得假装完成；
14. QA/Red-Team 独立验收；
15. docs/status/monthly review/handoff 全更新。

### 3.3 周计划

#### Week 0 — 2026-05-29 至 2026-05-31：Truth and Gate Cleanup

目标：停止错误完成口径。

交付：

- `canonical-event-matrix.csv`；
- `multiplayer-is-shared-matrix.md`；
- cleaned `status-board.md`；
- `registration-mode-test-report.md`；
- full build/test evidence。

验收：

- 无泛化 `Done`；
- 48 vs 52 解释清楚；
- Off=0、CanaryOnly=4 有测试；
- old wrong act mapping 彻底移除。

#### Week 1 — 2026-06-01 至 2026-06-07：Canary Runtime Proof

目标：4 个 canary 从 code 走到 runtime evidence。

交付：

- Big Fish proof；
- Golden Idol proof；
- Lab proof；
- Divine Fountain proof；
- screenshots/logs/save-load files。

验收：

- 每个 option 的效果有 run log；
- EN/ZHS 文本显示；
- 图片加载；
- 保存读取后状态正确。

#### Week 2 — 2026-06-08 至 2026-06-14：Simple Batch Implementation

目标：6 个简单事件 playable。

事件：

- Purifier；
- Upgrade Shrine；
- Golden Shrine；
- The Cleric；
- Old Beggar/Pleading Vagrant；
- Shining Light。

验收：

- exact options；
- dynamic values；
- EN/ZHS render；
- image proof；
- save/load proof。

#### Week 3 — 2026-06-15 至 2026-06-21：Content Parity Foundation

目标：解决体验差异最大的内容依赖。

交付：

- curse/relic/card equivalence matrix；
- Regret/Injury/Golden Idol/Bloody Idol/Bite/Parasite/Madness/Face relics gap decisions；
- temporary substitutes 全部标红，不能计入 parity。

验收：

- native equivalent / custom required / temporary substitute / blocked 分类完成；
- canary 与 simple batch 不再依赖未说明替代。

#### Week 4 — 2026-06-22 至 2026-06-28：Replacement Pool Prototype

目标：解决“体验不像 StS1”的核心池问题。

交付：

- debug-only replacement pool；
- act bucket tests；
- seeded run proof；
- visited/no-repeat proof；
- save/load bag proof；
- multiplayer fail-closed proof。

验收：

- replacement mode 不抽 StS2 原事件；
- default Off；
- co-op 不开放未验证模式。

#### Week 5 — 2026-06-29 至 2026-06-30：Monthly Handoff

交付：

- `monthly-review-2026-06.md`；
- updated README / PROJECT_MAP / docs index；
- release evidence dashboard；
- blocker report；
- QA pass/fail；
- commit/push only if validation passes。

---

## 4. Mandatory Overnight Run — 跑完才能停止

### 4.1 停止规则

他不能在这些状态停止：

```text
stash recovery complete
361 tests pass
status-board updated
registry count guard exists
source file exists
localization JSON exists
asset scripts exist
replacement prototype file exists
```

唯一允许停止：

```text
A. O0-O24 全部 GREEN；
或
B. HARD STOP BLOCKER REPORT 完整写出，并说明为什么当前环境无法继续。
```

### 4.2 O0-O24 Exit Gates

| Gate | 名称 | 必须结果 |
| --- | --- | --- |
| O0 | Worktree snapshot | `git status`, `HEAD`, `diff --stat` 保存 |
| O1 | Full build | 最新 full unfiltered build exit code 0 |
| O2 | Full tests | 0 failed；skipped tests 有解释 |
| O3 | Status truth | 无 false `Done` |
| O4 | Canonical matrix | 52 entries + 48 runtime count reconciliation |
| O5 | Act mapping | 正确 mapping + guard tests |
| O6 | Feature gate | Off/CanaryOnly/Additive/Replacement modes tested |
| O7 | Registration count | Off=0, CanaryOnly=4, Additive count documented |
| O8 | IsShared matrix | 每个事件都有 reason/evidence |
| O9 | Combat IsShared | 6+ combat events true 且 tests pass |
| O10 | ZHS placeholders | `待翻译` = 0 |
| O11 | Asset manifest | 每个 target event 有 image row |
| O12 | Asset proof | canary images file/hash/runtime screenshot |
| O13 | Canary source/API | 4 canary source/API proof |
| O14 | Canary implementation | 4 canary no TODO in reachable code |
| O15 | Canary debug spawn | 4 canary runtime screenshots/logs |
| O16 | Canary save/load | 4 canary save/load proof |
| O17 | Simple batch specs | 6 simple event exact specs |
| O18 | Simple batch implementation | 6 simple events playable proof |
| O19 | Replacement functional | unknown room 不抽 StS2 原事件 proof |
| O20 | Content parity gaps | curse/relic/card gap matrix |
| O21 | Combat blockers | 7 blocked events blocker report |
| O22 | Multiplayer guard | co-op mode fail-closed unless verified |
| O23 | QA Red-Team | independent pass/fail report |
| O24 | Handoff | docs, monthly review, release evidence updated |

### 4.3 Hard Stop Blocker Report 模板

如果不能继续，必须输出：

```text
HARD STOP BLOCKER REPORT
Gate:
Exact command:
Exit code:
Error excerpt:
Files touched:
Why no safe workaround exists:
Next owner/action:
What remains red:
```

---

## 5. Subagent Work Orders

### 5.1 BuildGate / Repo Health Subagent

任务：保证最新 build/test 真实通过。

输出：

- build log；
- test log；
- skipped test explanation；
- compile/runtime blockers。

### 5.2 Wiki Parity Spec Auditor Subagent

任务：建立 52-entry canonical matrix。

输出：

- `canonical-event-matrix.csv`；
- 48 registry reconciliation；
- spec completeness report。

### 5.3 StS2 Source/API Auditor Subagent

任务：验证 ActModel、EventModel、RitsuLib、CardCmd、CardSelectCmd、Relic/Potion/Gold/HP/Save APIs。

输出：

- API matrix；
- source citations/paths；
- unsafe API list。

### 5.4 Feature Gate / Registration Engineer Subagent

任务：Off/CanaryOnly/Additive/Replacement modes。

输出：

- registration count tests；
- runtime mode log；
- no default contamination proof。

### 5.5 Multiplayer IsShared Subagent

任务：逐事件判定 `IsShared`。

输出：

- `multiplayer-is-shared-matrix.md`；
- guard tests；
- co-op risk list。

### 5.6 Canary Gameplay Engineer Subagent

任务：Big Fish / Golden Idol / Lab / Divine Fountain playable。

输出：

- screenshots；
- option result logs；
- save/load proof；
- unresolved parity gaps。

### 5.7 Simple Batch Engineer Subagent

任务：6 simple batch playable。

输出：

- implementation files；
- option proof；
- screenshots；
- save/load proof。

### 5.8 Content Parity Subagent

任务：处理 curse/relic/card/content gaps。

输出：

- equivalence matrix；
- custom-required list；
- temporary-substitute list。

### 5.9 Event Pool / RNG / Save Subagent

任务：ReplacementPrototype functional proof。

输出：

- seeded run proof；
- event bag proof；
- visited/no-repeat proof；
- save/load proof。

### 5.10 Asset + Localization Subagent

任务：ZHS 清零、图片 manifest、runtime render proof。

输出：

- no-placeholder test；
- image hashes；
- EN/ZHS screenshots；
- missing asset blocker report。

### 5.11 QA / Red-Team Subagent

任务：独立验收，不写实现。

输出：

- O0-O24 pass/fail；
- false Done audit；
- release-blocking issues。

### 5.12 Release Documentation Subagent

任务：更新所有 handoff 文档。

输出：

- monthly review；
- status-board；
- PROJECT_MAP/docs index；
- release evidence status。

---

## 6. 给执行助理的直接指令

```text
当前工作没有完成。你这次可以把 stash incident recovery 标为完成，但不能把 StS1 event port 标为完成。

不要在 “361 tests pass / 6 IsShared overrides / status-board updated / registry count guard exists” 后停止。

立刻继续 Mandatory Overnight Run。你只能在 O0-O24 全绿后停止；如果无法继续，必须输出 HARD STOP BLOCKER REPORT。

最高优先级：
1. 保留 full unfiltered build/test evidence。
2. 清理 status-board 中所有 false Done。
3. 建 52-entry canonical matrix，并解释 48 runtime registry count。
4. 确认 Off=0、CanaryOnly=4，Additive/Replacement 均非默认。
5. 完成 per-event IsShared matrix。
6. 清零 38 个 ZHS placeholder。
7. 完成 canary images + EN/ZHS render + debug spawn + save/load proof。
8. 完成 4 canary + 6 simple batch playable。
9. 让 ReplacementPrototype 提供 functional proof，不只是 source exists。
10. Combat events 没有 encounter model 前必须保持 blocked。
11. 任何没有 source/API/test/screenshot/log/save-load evidence 的项不得标 Done。
12. 启动 subagents；实现者不能自验，QA/Red-Team 必须独立给 pass/fail。
```

---

## 7. 最终决策

**继续优化 + 有限推进。**

- 优化：修真实状态、matrix、feature gate、IsShared、ZHS、assets、replacement pool proof。
- 推进：只推进 4 canary + 6 simple batch 到 runtime verified。
- 暂停：full 52-event parity、combat event playable、custom UI full parity、release-ready claim。

管理红线：

```text
不要继续堆空壳事件。
不要把 automated test pass 当 runtime proof。
不要把 registry count 当 event parity。
不要把 source file exists 当 replacement pool works。
不要把 stash recovery complete 当 StS1 port complete。
```
结论：**没有完成。**
这次可以承认一个局部成果：**stash incident recovery / `IsShared` guard 恢复基本完成**。但它不等于 StS1 全事件迁移完成，也不等于“杀戮尖塔 1 体验一致”。

我把新版严格审核、下月 dev spec、subagent 工作单、以及“必须跑完才能停”的 overnight gates 整理成文件：

[下载：StS1 Event Port 严格审核 v8 + Monthly Dev Spec + Mandatory Overnight Run](sandbox:/mnt/data/sts1_event_port_strict_audit_v8_monthly_spec_overnight.md)

## 严格审核结论

他这次报告的 `361 passed, 0 failed, 21 skipped`、6 个 combat events `IsShared = true`、6 个 guard tests、status-board 更新，**只能证明 stash recovery 和一部分自动化 guard 有进展**。这不能证明 runtime 事件体验完成，因为他自己仍列出 pending：runtime gameplay verification、ZHS localization 38 个 placeholder、7 个 combat encounter models、event images。此前 overnight 要求也明确过：不能因为 spec/model/registration/localization/asset script/build claim 就停止，必须有 source/API/test/screenshot/log/save-load evidence。

最关键的判断是：**“All recovery tasks are complete” 可以接受；“All StS1 event port tasks are complete” 不接受。**

StS1 Wiki 的事件目标不能被 48 registry entries 简化掉。Wiki 说明事件来自 unknown location，事件由随机和当前 Act 决定，有 shared、semi-common、Act-exclusive；Act 4 没有事件；A15 会强化部分不利事件；列表包含 16 个 shared、12 个 Act 1、16 个 Act 2、8 个 Act 3。([slay-the-spire.fandom.com][1]) 所以 `RegistryEntryCountIs48` 这个 guard 只能作为 runtime registry guard，不能当作“52 Wiki event target 完成”的证明。

项目边界也不能被破坏：当前 active deliverable 仍是单一 `Spire Plus` mod，technical manifest id 是 `EZMicroBalance`，代码和资源路径分别是 `EZMicroBalanceCode/` 与 `EZMicroBalance/`。 资源、本地化、打包、版本、release evidence 也必须按项目规则更新；原版素材不能随意提交，原版 art 只有授权确认后才能进入 tracked/public files。

## 每一步完成情况

| 步骤                       | 他声称                        | 严格审核                                                           |
| ------------------------ | -------------------------- | -------------------------------------------------------------- |
| Stash recovery           | 完成                         | **局部完成**：6 个 combat `IsShared` 和 guard tests 可算恢复完成            |
| Build/tests              | 361 pass / 0 fail          | **自动测试通过，不等于 runtime proof**；21 skipped 必须解释                   |
| Status-board             | 已更新                        | **需复核**：不能再出现无证据 `Done`                                        |
| Registry count           | 48 entries                 | **未完成**：必须解释 48 runtime entries vs 52 Wiki target              |
| Shared/combat `IsShared` | guard tests 增加             | **部分完成**：还需要 per-event matrix                                  |
| Feature gate             | Off mode test              | **部分完成**：还要证明 default Off、CanaryOnly exactly 4、Replacement 非默认 |
| Canary events            | 未给 runtime proof           | **未完成**                                                        |
| ZHS localization         | 38 placeholders            | **未完成**                                                        |
| Event images             | pending                    | **未完成**                                                        |
| Combat events            | 7 encounter models blocked | **未完成 / blocked**                                              |
| Replacement pool         | source structure guard     | **未完成**：文件存在不等于 unknown room 真的只抽 StS1                         |
| Full StS1 experience     | 暗示恢复完成                     | **未完成**                                                        |

## 当前状况 vs 目标

目标是“StS1 事件体验迁移”：事件池、Act bucket、选项、奖励、A15、图片、文本、save/load、co-op、runtime flow 都要接近 StS1。当前只有基础设施、guard tests、部分 event model 和文档。你感觉“事件和杀戮尖塔 1 的游戏体验出入还是很大”，判断是对的：核心原因是 replacement event pool、内容等价、图片/文本、runtime proof、combat events 都还没闭环。

## 决策：继续优化 + 有限推进

不要只优化，也不要盲目推进 52 个事件。正确策略是：

1. **先优化基础真实性**：status-board、52-entry canonical matrix、48 registry reconciliation、feature gate、Act mapping、`IsShared` matrix、ZHS placeholder、asset manifest。
2. **同时有限推进 playable 范围**：只推进 4 个 canary + 6 个 simple batch。
3. **暂停 full parity 宣称**：combat events、custom UI、replacement pool、images、ZHS、runtime save/load 没完成前，不能写 release-ready 或“完全一样”。

## June Monthly Dev Spec

月目标名称必须是：

**`StS1 Event Port Prototype Batch 1 — Parity Foundation`**

月末必须达成：

1. full unfiltered build exit code 0；
2. full automated suite 0 failed，skipped tests 有解释；
3. default Off，对 Spire Plus 零影响；
4. CanaryOnly 精确注册 4 个事件：Big Fish、Golden Idol、Lab、Divine Fountain；
5. 52-entry canonical matrix 完成，并解释 48 runtime registry count；
6. Act mapping 有 guard：`Overgrowth + Underdocks = Act 1`，`Hive = Act 2`，`Glory = Act 3`；
7. per-event `IsShared` matrix 完成；
8. 4 个 canary playable + screenshot + save/load + EN/ZHS render + image proof；
9. 6 个 simple batch playable：Purifier、Upgrade Shrine、Golden Shrine、The Cleric、Old Beggar/Pleading Vagrant、Shining Light；
10. ZHS placeholder 清零；
11. image manifest + local extraction/hash proof；
12. debug-only ReplacementPrototype functional proof：unknown room 不抽 StS2 原事件；
13. 7 个 combat blocked events 有 blocker report，不能假装完成；
14. QA/Red-Team 独立验收。

## Overnight Run：跑完才能停止

他不能在这些点停止：

```text
stash recovery complete
361 tests pass
status-board updated
registry count guard exists
source file exists
localization JSON exists
asset scripts exist
replacement prototype file exists
```

唯一允许停止：

```text
A. O0-O24 全部 GREEN；
或
B. 输出完整 HARD STOP BLOCKER REPORT，证明当前环境无法继续。
```

核心 gates：

| Gate    | 必须结果                                                           |
| ------- | -------------------------------------------------------------- |
| O0      | worktree snapshot 保存                                           |
| O1      | latest full unfiltered build exit code 0                       |
| O2      | full tests 0 failed，skipped 有解释                                |
| O3      | status-board 无 false `Done`                                    |
| O4      | 52-entry canonical matrix + 48 registry reconciliation         |
| O5      | Act mapping guard                                              |
| O6      | feature gate mode tests                                        |
| O7      | Off=0，CanaryOnly=4                                             |
| O8      | per-event `IsShared` matrix                                    |
| O9      | combat `IsShared` tests                                        |
| O10     | ZHS placeholders = 0                                           |
| O11     | asset manifest                                                 |
| O12     | canary image proof                                             |
| O13-O16 | 4 canary source/API、implementation、debug spawn、save/load proof |
| O17-O18 | 6 simple batch exact spec + playable proof                     |
| O19     | ReplacementPrototype functional proof                          |
| O20     | content parity gap matrix                                      |
| O21     | combat blocker report                                          |
| O22     | multiplayer fail-closed guard                                  |
| O23     | QA Red-Team pass/fail                                          |
| O24     | handoff docs updated                                           |

## 强制 subagent

直接提醒他启动这些 subagent：

1. **BuildGate / Repo Health**：build/test/skipped tests evidence。
2. **Wiki Parity Spec Auditor**：52-entry matrix，解释 48 registry。
3. **StS2 Source/API Auditor**：ActModel、EventModel、RitsuLib、Card/Relic/Potion/Save APIs。
4. **Feature Gate / Registration Engineer**：Off/CanaryOnly/Additive/Replacement。
5. **Multiplayer IsShared Subagent**：逐事件 `IsShared` 判定。
6. **Canary Gameplay Engineer**：4 个 canary runtime proof。
7. **Simple Batch Engineer**：6 个 simple event playable。
8. **Content Parity Subagent**：curse/relic/card 等价矩阵。
9. **Event Pool / RNG / Save Subagent**：replacement pool functional proof。
10. **Asset + Localization Subagent**：ZHS 清零、图片 proof、渲染截图。
11. **QA / Red-Team Subagent**：独立验收，不写实现。
12. **Release Documentation Subagent**：status、monthly review、handoff、release evidence。

## 发给他的直接指令

```text
当前工作没有完成。你这次只能把 stash incident recovery 标为完成，不能把 StS1 event port 标为完成。

不要在 “361 tests pass / 6 IsShared overrides / status-board updated / registry count guard exists” 后停止。

立刻继续 Mandatory Overnight Run。你只能在 O0-O24 全绿后停止；如果无法继续，必须输出 HARD STOP BLOCKER REPORT。

最高优先级：
1. 保留 full unfiltered build/test evidence。
2. 清理 status-board 中所有 false Done。
3. 建 52-entry canonical matrix，并解释 48 runtime registry count。
4. 确认 Off=0、CanaryOnly=4，Additive/Replacement 均非默认。
5. 完成 per-event IsShared matrix。
6. 清零 38 个 ZHS placeholder。
7. 完成 canary images + EN/ZHS render + debug spawn + save/load proof。
8. 完成 4 canary + 6 simple batch playable。
9. 让 ReplacementPrototype 提供 functional proof，不只是 source exists。
10. Combat events 没有 encounter model 前必须保持 blocked。
11. 任何没有 source/API/test/screenshot/log/save-load evidence 的项不得标 Done。
12. 启动 subagents；实现者不能自验，QA/Red-Team 必须独立给 pass/fail。
```

管理红线：**不要继续堆空壳事件；不要把 automated tests 当 runtime proof；不要把 registry count 当 event parity；不要把 stash recovery complete 当 StS1 port complete。**

[1]: https://slay-the-spire.fandom.com/wiki/Events "Events | Slay the Spire Wiki | Fandom"
