结论先放最前面：**当前任务没有完成。**
最新状态可以承认是 **code-side foundation advanced**：build/test/guard/default Off/ZHS placeholder/canonical matrix 等方面有明显进展；但它还不是 **StS1 event experience parity**，因为 runtime gameplay、save/load、图片渲染、ReplacementPrototype 功能验证、combat encounter models、独立 QA/Red-Team 仍然没有完成。

我把“从头开始的完整上下文 + 严格审核 + June Monthly Dev Spec + Mandatory Overnight Run v11 + subagent 工作单”整理成了文档：

[下载完整交接审查文档 v11](sandbox:/mnt/data/sts1_event_port_strict_audit_v11_full_context_monthly_spec.md)

## 1. 从头重建上下文

你的最终目标不是“生成一些事件类”，而是让 StS2 的 `Spire Plus` mod 尽量复刻《杀戮尖塔 1》的事件体验，包括 unknown room 事件池、Act bucket、选项流程、reward/card/relic/curse/potion、A15 数值、图片、EN/ZHS 文本、save/load、co-op/`IsShared` 和默认不污染现有 Spire Plus。

项目边界也很明确：当前 active deliverable 仍是单一 `Spire Plus` mod，technical manifest id 仍是 `EZMicroBalance`；代码路径是 `EZMicroBalanceCode/`，资源/本地化路径是 `EZMicroBalance/`。 发布结构也仍应是 `<GameRoot>/mods/EZMicroBalance/` 下的 `EZMicroBalance.json / EZMicroBalance.dll / EZMicroBalance.pck`。

还有两条红线必须一直保留：不要原地改 manifest id；不要随意复制原版游戏素材、数据表、大段反编译代码或未授权 art。项目规则明确要求原版 art 只有在授权确认后才能进 tracked/public files，否则只能本地抽取、使用授权/生成替代素材，或标注 non-parity placeholder。

## 2. StS1 事件目标到底是什么

StS1 Wiki 对 events 的定义是：事件来自 unknown location；事件是否出现和出现哪个事件由随机与当前 Act 决定；部分事件限定 Act，部分事件可跨 Act；Act 4 没有 unknown location/event；Ascension 15 会让部分不利事件更可能或更强。([Slay the Spire][1])

Wiki public target 不是一个简单的 “48/50/54” 数字。Wiki 页面列出：16 个 shared events、12 个 Act 1 exclusive events、16 个 Act 2 exclusive events、8 个 Act 3 exclusive events，总体 public listed target 是 52。([Slay the Spire][1])

所以现在所有数字必须用 canonical matrix 解释：

|                    数字 | 严格解释                                                                                                                  |
| --------------------: | --------------------------------------------------------------------------------------------------------------------- |
|                    52 | public Wiki listed event target                                                                                       |
|                    54 | 可能是本地 `events_complete.md`、semi-shared act membership、internal split、special/debug entry 或重复 registration call；必须逐项解释 |
|                    48 | runtime model count；必须解释哪些 event 合并、blocked、excluded 或 split                                                          |
|                    50 | registry entry count；不能直接等于 StS1 full parity                                                                          |
| 54 registration calls | 可能是 shared + act-specific registration calls；不能直接当 unique event 完成                                                    |

如果 canonical matrix 不能解释这些数字，**不能写 all events complete**。

## 3. 之前助理工作的历史问题

早期工作最大的问题是把“文档/代码脚手架存在”当成“事件完成”。当时他把 48 个 spec 文件称为覆盖 52 个清单条目，又把 `Infrastructure`、`event-specs`、`assets.md`、`localization.md`、`test-plan.md` 写成 `Done`，但同一状态里还列着 Regret、Injury、random relic helper、card UI、combat encounter models 等 blocker。

随后他写了 `Sts1EventRegistrationService.RegisterAll(ModId)` 并接入 `MainFile.Initialize()`，但最初版本有两个严重问题：第一是无条件注册，污染默认 Spire Plus；第二是 Act mapping 写成 `Underdocks=Act1, Overgrowth=Act2, Hive=Act3`，这会导致 Act 2/Act 3 事件进错章节。这个错误在早期记录里很清楚。

后来 v6 overnight 要求修正方向：默认 Off；正确 Act mapping；RitsuLib additive registration 不等于 StS1 事件体验；没有 source/API/test/screenshot/log/save-load evidence 不得标 Done；并要求 O0-O15 gate 全绿才能停止。该阶段也记录了正确映射：`Overgrowth + Underdocks = Act 1`，`Hive = Act 2`，`Glory = Act 3`。

## 4. 最新状态严格审核

你最新贴的状态是：

```text
Build: 0 errors, 0 warnings
Tests: 428 passed, 0 failed, 21 skipped (398 total)
StS1 Guard Tests: 24/24 pass

default Off=0 proof: DONE
CanaryOnly=4 + AdditiveBatch1 exact count: DONE — guard tests verify 4 canary, 54 total
canonical matrix: DONE — 54/48/50/54 reconciliation doc
runtime verify 4 canary: BLOCKED — requires game launch
runtime verify 6 simple batch: BLOCKED — requires game launch
zero ZHS placeholders: DONE — 399 keys, 0 placeholders
image/render proof: BLOCKED — no redistributable art
ReplacementPrototype functional proof: BLOCKED — requires game launch
combat events stay blocked: BLOCKED — correctly blocked, encounter models missing
QA/Red-Team independent: BLOCKED — requires independent agent
```

严格判定如下：

| 模块                   |                                           当前状态 | 严格审核                                              |
| -------------------- | ---------------------------------------------: | ------------------------------------------------- |
| Build                |                          0 errors / 0 warnings | 暂认代码侧通过，但必须保留 full unfiltered log                 |
| Tests                | 428 passed / 0 failed / 21 skipped / 398 total | 摘要数字冲突；428 + 21 不可能是 398，必须 BuildGate 复核完整 log    |
| StS1 guard tests     |                                     24/24 pass | 有进展                                               |
| Default Off          |                                   DONE claimed | 方向正确，必须保留 Off=0 registration proof                |
| CanaryOnly           |                                      4 claimed | 必须证明 exact event identity，不只是 count               |
| AdditiveBatch1       |                                     “54 total” | 命名冲突；Batch1 不应等于 all draft，54 更像 AdditiveAllDraft |
| Canonical matrix     |                                   DONE claimed | 必须 Red-Team 复核 52/54/48/50/54                     |
| Canary code          |                          code-complete claimed | 只能算 code-side claimed complete，runtime 未证明        |
| Simple batch code    |                          code-complete claimed | 只能算 code-side claimed complete，runtime 未证明        |
| ZHS                  |                       399 keys / 0 placeholder | 文件层面进展；仍需游戏内 render proof                         |
| Images               |                                        blocked | 未完成                                               |
| Runtime gameplay     |                                        blocked | 未完成                                               |
| Save/load            |                                        blocked | 未完成                                               |
| ReplacementPrototype |                                        blocked | source/guard 不等于 functional proof                 |
| Combat events        |                                        blocked | encounter models 缺失，不能计入 parity                   |
| QA/Red-Team          |                                        blocked | 必须独立验收                                            |
| Full StS1 experience |                                            未证明 | 未完成                                               |

因此，当前正确表述是：

> 当前完成了大量 code-side foundation 和 guard work；StS1 event port 的 runtime parity 仍未完成。

不能写：

```text
All tasks complete
All code-side work complete
Full parity complete
All StS1 events complete
Release-ready
和杀戮尖塔1完全一样
```

## 5. 为什么你会感觉“和杀戮尖塔 1 出入很大”

这个感觉是对的，原因不是单纯“事件数量不够”，而是体验核心还没闭环：

1. **RitsuLib additive registration 不是 StS1 event pool**：事件被注册进候选池，不代表 unknown room 只抽 StS1 事件。
2. **ReplacementPrototype 没有 functional proof**：source 文件存在或 guard 通过，不代表游戏里 unknown room 不再抽 StS2 原事件。
3. **runtime 没验证**：没有 4 canary / 6 simple batch 的截图、结果日志、save/load。
4. **图片没完成**：没有 redistributable art，也没有本地抽取/hash/render proof。
5. **ZHS 只是文件层面**：0 placeholder 不等于游戏内无 missing key、无布局溢出。
6. **combat events blocked**：没有 encounter models，不能算完成。
7. **AdditiveBatch1 命名混乱**：如果 Batch1 显示 54 total，那它很可能不是 “verified batch”，而是 “all draft”。
8. **没有独立 QA**：实现者自称完成不能替代 Red-Team 验收。

## 6. Canary 的验收标准不能降低

Big Fish 必须严格实现：Act 1 exclusive；Banana 回复 `floor(maxHP / 3)`，Donut 增加 5 Max HP，Box 给随机 common/uncommon/rare relic 并加入 Regret。([Slay the Spire][2])

Golden Idol 必须严格实现：Act 1 exclusive；Take 获得 Golden Idol 并触发陷阱；Outrun 给 Injury；Smash 造成 25% max HP 伤害，A15+ 是 35%；Hide 损失 8% max HP，A15+ 是 10%；Leave 无事发生。([Slay the Spire][3])

这两个事件如果没有 runtime screenshot、pre/post state log、save/load proof、EN/ZHS render proof、image proof，就不能算完成。

## 7. 当前决策：继续优化 + 有限推进，两者兼顾，但优化优先

不要只优化，也不要盲目推进 52/54 个事件。正确策略是：

**继续优化：**

* build/test evidence 真实性；
* 428/21/398 test count 冲突；
* status-board 真实性；
* 52/54/48/50/54 canonical matrix；
* AdditiveBatch1 vs AdditiveAllDraft mode naming；
* asset/license strategy；
* independent QA/Red-Team。

**有限推进：**

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
- Old Beggar / Pleading Vagrant canonical mapping
- Shining Light
```

**暂停扩大范围：**

```text
combat event full implementation
custom UI full implementation
继续堆更多 draft event
full parity 宣称
release-ready 宣称
```

## 8. June Monthly Dev Spec

下个月目标名称：

**`StS1 Event Port Prototype Batch 1 — Runtime Parity Foundation`**

月末必须达成：

1. 最新 full unfiltered build exit code 0。
2. 最新 full tests exit code 0。
3. test count reconciliation：passed + skipped + failed 必须等于 total。
4. skipped tests 全部解释。
5. Default Off 注册 0 个 StS1 events。
6. CanaryOnly 精确注册 Big Fish、Golden Idol、Lab、Divine Fountain。
7. AdditiveBatch1 只包含 verified scope，不得混同 AdditiveAllDraft。
8. AdditiveAllDraft 明确 dev-only。
9. ReplacementPrototype 明确 debug-only。
10. Canonical matrix 经 Red-Team 复核。
11. 4 canary runtime verified。
12. 6 simple batch runtime verified。
13. Verified scope 的 EN/ZHS render proof 完成。
14. Verified scope 的 image render proof 完成；如果没有授权原图，必须使用 local extraction hash proof、owner licensed assets、generated replacement art，或明确 non-parity placeholder。
15. ReplacementPrototype functional proof 完成：unknown room 只抽 StS1 candidates，Act bucket 正确，event bag/save-load 正确。
16. multiplayer fail-closed 或 verified behavior 完成。
17. combat events 在 encounter models 完成前保持 blocked。
18. independent QA/Red-Team 给 pass/fail。

资源、本地化、打包或玩家可见行为改动后，还需要按项目规则 build/publish 并更新版本、package metadata、tester handoff。

## 9. Mandatory Overnight Run v11：跑完才能停止

停止条件只能是：

```text
A. O0-O42 全部 GREEN；
或
B. 输出 HARD STOP BLOCKER REPORT：
   - exact gate id
   - blocker reason
   - evidence path
   - attempted actions
   - owner action
   - why continuation is impossible in current environment
```

注意：**Hard stop 只允许暂停，不代表完成。**

不能因为这些就停止：

```text
build passes
tests pass
guard tests pass
canonical matrix exists
ZHS placeholders = 0
status-board updated
source files exist
asset scripts exist
replacement source exists
hard-stop report exists
all code-side work complete
```

### O0-O42 gates

| Gate | 必须结果                                                                 |
| ---- | -------------------------------------------------------------------- |
| O0   | worktree snapshot：branch、HEAD、diff、unstaged files                    |
| O1   | full unfiltered build exit code 0，保存完整 log                           |
| O2   | full tests exit code 0，保存完整 log                                      |
| O3   | test count reconciliation：passed + skipped + failed = total          |
| O4   | skipped tests 逐条解释                                                   |
| O5   | status-board 无 false Done                                            |
| O6   | canonical matrix 完整                                                  |
| O7   | 52/54/48/50/54 reconciliation 经 Red-Team 复核                          |
| O8   | Act mapping guard passes                                             |
| O9   | feature gate tests pass                                              |
| O10  | Off=0 registration proof                                             |
| O11  | CanaryOnly=4 exact identity proof                                    |
| O12  | AdditiveBatch1 exact verified-scope proof                            |
| O13  | AdditiveAllDraft dev-only proof                                      |
| O14  | ReplacementPrototype debug-only proof                                |
| O15  | per-event IsShared matrix                                            |
| O16  | combat IsShared=true guard passes                                    |
| O17  | canary code review clean                                             |
| O18  | canary runtime screenshots complete                                  |
| O19  | canary result logs complete                                          |
| O20  | canary save/load proof complete                                      |
| O21  | canary EN/ZHS render proof complete                                  |
| O22  | canary image render/license proof complete                           |
| O23  | simple batch exact spec red-team pass                                |
| O24  | simple batch code review clean                                       |
| O25  | simple batch runtime screenshots complete                            |
| O26  | simple batch result logs complete                                    |
| O27  | simple batch save/load proof if applicable                           |
| O28  | verified-scope ZHS placeholders = 0 and render verified              |
| O29  | verified-scope asset manifest complete                               |
| O30  | verified-scope image/license decision documented                     |
| O31  | replacement source guard passes                                      |
| O32  | replacement functional proof：unknown rooms only draw StS1 candidates |
| O33  | replacement act bucket proof：Act 1/2/3 correct                       |
| O34  | event bag / visited ids / no-repeat proof                            |
| O35  | replacement save/load proof                                          |
| O36  | multiplayer fail-closed or verified proof                            |
| O37  | content parity gap matrix                                            |
| O38  | temporary substitutes marked non-parity                              |
| O39  | combat blocker report current and honest                             |
| O40  | independent QA/Red-Team report                                       |
| O41  | monthly review updated                                               |
| O42  | handoff docs updated with next owner actions                         |

## 10. 强制 subagent 分工

让助理必须启动这些 subagent；实现者不能审核自己的工作。

| Subagent                             | 职责                                                                                         | 输出                                                     |
| ------------------------------------ | ------------------------------------------------------------------------------------------ | ------------------------------------------------------ |
| BuildGate / Repo Health              | build/test logs、exit codes、skipped tests、worktree、test count reconciliation                | `build-full.log`、`test-full.log`、`skipped-tests.md`    |
| Wiki Parity Spec Auditor             | 52 public target、54 internal entries、exact options、A15、semi-common membership              | `canonical-event-matrix.csv`、`count-reconciliation.md` |
| StS2 Source/API Auditor              | ActModel、EventModel、RitsuLib、card/relic/potion/save/replacement API                        | `source-api-matrix.md`                                 |
| Feature Gate / Registration Engineer | Off、CanaryOnly、AdditiveBatch1、AdditiveAllDraft、ReplacementPrototype                        | registration mode tests                                |
| Canary Gameplay Subagent             | Big Fish、Golden Idol、Lab、Divine Fountain runtime proof                                     | screenshots、logs、save/load                             |
| Simple Batch Gameplay Subagent       | Purifier、Upgrade Shrine、Golden Shrine、The Cleric、Old Beggar/Pleading Vagrant、Shining Light | screenshots、logs                                       |
| Asset + Localization Subagent        | ZHS render、missing key scan、image/license decision                                         | render screenshots、asset manifest                      |
| Event Pool / RNG / Save Subagent     | replacement pool、event bag、visited ids、save/load                                           | functional proof                                       |
| Multiplayer / IsShared Subagent      | per-event IsShared matrix、combat true、fail-closed co-op                                    | `is-shared-matrix.csv`                                 |
| Content Parity Subagent              | Bite、face relics、StS1 curses、Golden/Bloody Idol、combat encounter models                    | gap matrix                                             |
| QA / Red-Team Subagent               | 只验收，不实现；逐 gate pass/fail                                                                   | `qa-redteam-report.md`                                 |
| Release Documentation Subagent       | status-board、monthly review、handoff、release evidence                                       | updated docs                                           |

## 11. 可以直接发给助理的指令

```text
当前状态不能标完成。你这轮 code-side work 有进展，但 runtime gameplay、save/load、event images、ReplacementPrototype functional proof、combat encounter models、independent QA 仍然 blocked 或未验证。

不要写 all tasks complete、full parity、release-ready、和杀戮尖塔1完全一样。

继续 Mandatory Overnight Run v11。你只能在 O0-O42 全绿后停止；若因为 game launch、licensed art、external QA、owner action 无法继续，必须输出 HARD STOP BLOCKER REPORT。Blocked gate 不得标完成。

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
# StS1 Event Port 严格审核 v11 — 从零上下文、当前状态、June Monthly Dev Spec、Mandatory Overnight Run

日期：2026-05-31
适用项目：`dev-the-spire` / `Spire Plus` / technical manifest id `EZMicroBalance`
目标：把《Slay the Spire 1》的事件体验迁移到 StS2 mod 中，但必须以证据、运行时验证、图片/本地化/保存读取/事件池一致性为准，不允许用“代码生成完成”替代“体验完成”。

---

## 0. 最终结论

当前任务 **未完成**。

可以承认的当前状态：

- 代码侧基础设施明显推进。
- 默认 Off / feature gate / guard tests 有进展。
- Canary 和 simple batch 的 code-side claimed complete 有进展。
- ZHS placeholder 清零的文件层面进展可以暂认。
- Build/test 摘要显示 0 failed，但 test count 摘要存在数字冲突，需要 BuildGate 复核。
- 当前仍缺运行时证据、save/load 证据、图片/渲染证据、ReplacementPrototype functional proof、combat encounter models、独立 QA/Red-Team。

禁止声明：

- `All tasks complete`
- `Full parity complete`
- `All StS1 events complete`
- `Release-ready`
- `和杀戮尖塔1完全一样`
- `所有 code-side work complete`（只能限定为“当前 verified-scope 代码侧暂称完成，仍需 runtime/QA”）

正确声明：

> 当前达到 code-side foundation advanced / guard-pass claimed state；StS1 事件体验迁移仍未完成。下一阶段必须把 4 个 canary + 6 个 simple batch 跑成 runtime verified，并完成 replacement event pool、save/load、图片/本地化渲染和独立 QA。

---

## 1. 从头重建项目背景

### 1.1 项目身份

- 当前项目不是新建独立 mod，而是现有 `Spire Plus` mod。
- technical manifest id 必须继续是 `EZMicroBalance`。
- C# 代码目录应在 `EZMicroBalanceCode/`。
- 资源、本地化、图片目录应在 `EZMicroBalance/`。
- 不允许随意改 manifest id，不允许把历史 scaffold 当作新的 active deliverable。
- 发布结构仍应保持 `EZMicroBalance.json / EZMicroBalance.dll / EZMicroBalance.pck`。

### 1.2 项目硬约束

- 不要复制原版 StS2 非美术资产、源码、数据表、大段反编译代码。
- 原版 art 只有在授权确认后才能进入 tracked/public files。
- StS1 原图也不能直接随包分发；正确做法是 owner 本地抽取、授权素材、生成替代图，或明确 non-parity placeholder。
- 代码/资源/本地化/打包变化后，必须 build/publish/test，并更新 docs、version、package metadata、handoff。
- StS1 Event Port 必须默认 Off；不能污染 Spire Plus 默认体验。

---

## 2. StS1 事件目标定义

StS1 Wiki 的 public event target 是：

- 16 个 shared events。
- 12 个 Act 1 exclusive events。
- 16 个 Act 2 exclusive events。
- 8 个 Act 3 exclusive events。
- 总体 public wiki target：52 个 listed event entries。
- Events 来自 unknown location；抽取取决于随机与当前 Act。
- Act 4 没有 unknown locations / events。
- Ascension 15 会让不利事件更可能或更强。

因此必须维护 canonical matrix：

```csv
canonical_id,wiki_name,wiki_bucket,st1_acts_allowed,source_spec_entry,runtime_model,registry_entries,registration_calls,is_shared,mode,status,evidence_paths,parity_notes
```

需要特别解释数字漂移：

- `52`：public Wiki listed target。
- `54`：若来自本地 `events_complete.md` 或内部 spec/registration call，必须解释是不是 alias、special event、semi-shared act membership、debug-only split、Neow/Combat Start 之类。
- `48`：runtime model count，必须解释哪些事件合并、缺失、blocked、excluded。
- `50`：registry entry count，必须解释是不是 registry helper、internal entries、blocked placeholder、split models。
- `54 registration calls`：允许是 act memberships 或 shared + act-specific registration calls，但不能当成 unique event completion。

---

## 3. 历史工作审查脉络

### 3.1 第一阶段：调研/脚手架

早期工作创建了文档、manifest、部分事件 spec、部分 C# model、localization 和 asset scripts，但绝大多数内容只是 draft/stub。不能叫事件完成。

关键问题：

- 48 spec vs 52 catalog 混乱。
- 46 event models 被误称 Done。
- 代码中有 TODO / placeholder。
- 图片没有抽取。
- ZHS 未完成。
- canary 未 runtime verified。
- StS1-only event pool 未完成。

### 3.2 第二阶段：RitsuLib 注册

后续写入 `Sts1EventRegistrationService.RegisterAll(ModId)` 并接到 `MainFile.Initialize()`。这是有工程价值的进展，但最初版本存在两个严重问题：

- 无条件注册，污染默认 Spire Plus。
- Act mapping 写错：`Underdocks=Act1, Overgrowth=Act2, Hive=Act3`。

正确映射应是：

| StS1 bucket | StS2 ActModel |
|---|---|
| Act 1 | `Overgrowth` + `Underdocks` |
| Act 2 | `Hive` |
| Act 3 | `Glory` |
| Shared | shared registry |
| Semi-common | 按 StS1 允许 Act 精确注册 |

### 3.3 第三阶段：v6 Overnight 修正

v6 目标要求：

- 默认 Off。
- feature gate。
- 正确 act mapping。
- 不得用 “46 event models Done / 48 specs Done / build passes” 作为完成。
- 必须跑 O0-O15 gates。
- 必须使用 subagents。
- Combat events 必须 `IsShared = true`，因为 `EnterCombatWithoutExitingEvent` 需要 shared/voting 逻辑。

进展：

- `Sts1EventRegistrationMode`、`Sts1EventFeatureGate`、`Sts1EventsFeatureModule` 等 foundation 出现。
- 正确 act mapping 被采用。
- 一批 helper 与 card/relic/potion/gold 操作被修。
- 但当时仍有 compile errors、status false Done、canonical matrix 未完成、runtime evidence 缺失、combat blocked 等问题。

### 3.4 第四阶段：stash/recovery + IsShared guards

后续恢复了 6 个 combat events 的 `IsShared = true` override，并增加 guard tests。可承认 stash recovery / guard recovery 完成，但不能扩大为 StS1 port 完成。

### 3.5 第五阶段：v9/v10 code-side progress

最新摘要声称：

- Build 0 errors, 0 warnings。
- Tests 428 passed, 0 failed, 21 skipped，但同时写 `(398 total)`，数字冲突。
- StS1 Guard Tests 24/24 pass。
- Default Off=0 proof done。
- CanaryOnly=4 + AdditiveBatch1 exact count done，但又写 54 total，命名存在歧义。
- Canonical matrix explaining 54/48/50/54 done。
- ZHS 399 keys, 0 placeholders done。
- runtime canary blocked。
- runtime simple batch blocked。
- image/render proof blocked。
- ReplacementPrototype functional proof blocked。
- combat events blocked。
- QA/Red-Team blocked。

审查结论：当前只能称为 **code-side foundation advanced**，不是 runtime parity。

---

## 4. 当前完成度总表

| 模块 | 当前状态 | 严格审核 |
|---|---:|---|
| Build | 摘要称 0 errors / 0 warnings | 暂认，但必须保留 full unfiltered log |
| Tests | 摘要称 428 passed / 0 failed / 21 skipped / 398 total | 数字冲突；必须复核完整 log |
| StS1 guard tests | 24/24 pass | 有进展 |
| Default Off | DONE claimed | 需要保留 Off=0 registration proof |
| CanaryOnly | 4 claimed | 必须证明 exact event identity，不只是 count |
| AdditiveBatch1 | 54 total claimed | 命名冲突；Batch1 不应等于 all draft |
| Canonical matrix | DONE claimed | 必须 Red-Team 复核 52/54/48/50/54 |
| Canary code | code-complete claimed | 不能当 runtime proof |
| Simple batch code | code-complete claimed | 不能当 runtime proof |
| ZHS | 399 keys / 0 placeholder claimed | 文件层面进展；仍需 render proof |
| Images | blocked | 未完成 |
| Runtime gameplay | blocked | 未完成 |
| Save/load | blocked | 未完成 |
| ReplacementPrototype | blocked | 未完成 |
| Combat events | blocked | 未完成；不能计入 parity |
| QA/Red-Team | blocked | 未完成 |
| Full StS1 experience | 未证明 | 未完成 |

---

## 5. 与最终目标的差距

最终目标不是“生成 50 个 registry entries”，而是复刻 StS1 event experience：

1. unknown room 抽事件的体验。
2. Act bucket 和 semi-common 限制。
3. 事件选项、页面跳转、锁定条件。
4. reward / relic / card / curse / potion。
5. Ascension 15 数值变化。
6. 图片和文本。
7. EN/ZHS 渲染。
8. save/load。
9. multiplayer / `IsShared`。
10. 默认不污染 Spire Plus。

当前缺失最关键的是：

- runtime screenshots。
- pre/post result logs。
- save/load proof。
- image render proof。
- replacement pool functional proof。
- combat encounter models。
- independent QA。
- verified scope 的真实 gameplay feeling。

---

## 6. 管理决策

结论：**优化 + 有限推进，两者兼顾，但优化优先。**

### 6.1 继续优化

必须继续优化：

- test count / build evidence 真实性。
- status-board 真实性。
- 52/54/48/50/54 canonical matrix。
- AdditiveBatch1 vs AdditiveAllDraft mode naming。
- runtime evidence discipline。
- asset/license strategy。
- independent QA/Red-Team。

### 6.2 有限推进

只推进：

- 4 canary：
  - Big Fish
  - Golden Idol
  - Lab
  - Divine Fountain

- 6 simple batch：
  - Purifier
  - Upgrade Shrine
  - Golden Shrine
  - The Cleric
  - Old Beggar / Pleading Vagrant canonical mapping
  - Shining Light

### 6.3 暂停扩大范围

暂停：

- 继续堆更多 draft event。
- combat event full implementation。
- custom UI full implementation。
- full parity 宣称。
- release-ready 宣称。

---

## 7. June Monthly Dev Spec

名称：

`StS1 Event Port Prototype Batch 1 — Runtime Parity Foundation`

时间范围：

2026-06-01 至 2026-06-30。

### 7.1 月末 Go/No-Go 标准

必须达到：

1. 最新 full unfiltered build exit code 0。
2. 最新 full tests exit code 0。
3. skipped tests 全部解释。
4. Default Off 注册 0 个 StS1 events。
5. CanaryOnly 精确注册 4 个 events：Big Fish、Golden Idol、Lab、Divine Fountain。
6. AdditiveBatch1 只包含 verified scope，不得混同 AdditiveAllDraft。
7. AdditiveAllDraft 明确 dev-only。
8. ReplacementPrototype 明确 debug-only。
9. Canonical matrix 被 Red-Team 复核。
10. 4 canary runtime verified。
11. 6 simple batch runtime verified。
12. Verified scope 的 EN/ZHS render proof 完成。
13. Verified scope 的 image render proof 完成。
14. ReplacementPrototype functional proof 完成。
15. event bag / visited ids / no-repeat / save-load proof 完成。
16. multiplayer fail-closed 或 verified behavior 完成。
17. combat events 在 encounter models 完成前保持 blocked。
18. independent QA/Red-Team 给 pass/fail。

### 7.2 禁止范围

本月不得宣称：

- full parity。
- all StS1 events complete。
- release-ready。
- 和 StS1 完全一样。
- combat events complete。
- image parity complete，除非有授权/本地抽取/渲染证据。

---

## 8. Mandatory Overnight Run v11

### 8.1 停止条件

只能在以下条件之一停止：

A. O0-O42 全部 GREEN。
B. 输出 `HARD STOP BLOCKER REPORT`，其中必须包含：
   - exact gate id。
   - blocker reason。
   - evidence path。
   - attempted actions。
   - owner action。
   - why continuation is impossible in current environment。

注意：Hard stop 允许夜跑暂停，但 **不代表 feature 完成**。

### 8.2 不能停止的条件

不能因为这些就停止：

- build passes。
- tests pass。
- guard tests pass。
- canonical matrix exists。
- ZHS placeholders = 0。
- status-board updated。
- source files exist。
- asset scripts exist。
- replacement source exists。
- hard-stop report exists。
- “all code-side work complete”。

### 8.3 Gates

| Gate | 验收标准 |
|---|---|
| O0 | worktree snapshot：branch、HEAD、diff、unstaged files |
| O1 | full unfiltered build exit code 0，保存完整 log |
| O2 | full tests exit code 0，保存完整 log |
| O3 | test count reconciliation：passed + skipped + failed = total |
| O4 | skipped tests 逐条解释 |
| O5 | status-board 无 false Done |
| O6 | canonical matrix 完整 |
| O7 | 52/54/48/50/54 reconciliation Red-Team reviewed |
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
| O23 | simple batch exact spec red-team pass |
| O24 | simple batch code review clean |
| O25 | simple batch runtime screenshots complete |
| O26 | simple batch result logs complete |
| O27 | simple batch save/load proof if applicable |
| O28 | verified-scope ZHS placeholders = 0 and render verified |
| O29 | verified-scope asset manifest complete |
| O30 | verified-scope image/license decision documented |
| O31 | replacement source guard passes |
| O32 | replacement functional proof：unknown rooms only draw StS1 candidates |
| O33 | replacement act bucket proof：Act 1/2/3 correct |
| O34 | event bag / visited ids / no-repeat proof |
| O35 | replacement save/load proof |
| O36 | multiplayer fail-closed or verified proof |
| O37 | content parity gap matrix |
| O38 | temporary substitutes marked non-parity |
| O39 | combat blocker report current and honest |
| O40 | independent QA/Red-Team report |
| O41 | monthly review updated |
| O42 | handoff docs updated with next owner actions |

---

## 9. Subagent 工作单

### 9.1 BuildGate / Repo Health Subagent

职责：

- full build。
- full tests。
- skipped tests。
- worktree snapshot。
- test count reconciliation。

输出：

- `build-full.log`
- `test-full.log`
- `skipped-tests.md`
- `worktree-snapshot.md`

### 9.2 Wiki Parity Spec Auditor

职责：

- 52 public Wiki event target。
- 54 internal entries 解释。
- 每个 event exact option、A15、condition、dependency。
- semi-common membership。

输出：

- `canonical-event-matrix.csv`
- `count-reconciliation.md`
- `wiki-parity-redteam.md`

### 9.3 StS2 Source/API Auditor

职责：

- ActModel mapping。
- EventModel page/options API。
- RitsuLib registration behavior。
- Card/Relic/Potion/Gold/HP/Save APIs。
- Replacement hook API。

输出：

- `source-api-matrix.md`
- `act-mapping-proof.md`
- `command-api-proof.md`

### 9.4 Feature Gate / Registration Engineer

职责：

- Off。
- CanaryOnly。
- AdditiveBatch1。
- AdditiveAllDraft。
- ReplacementPrototype。
- mode-specific tests。

输出：

- `registration-mode-proof.md`
- `registration-count-tests`
- source changes。

### 9.5 Canary Gameplay Subagent

职责：

- Big Fish。
- Golden Idol。
- Lab。
- Divine Fountain。
- runtime screenshots。
- pre/post logs。
- save/load。

输出：

- `canary-runtime-evidence.md`
- screenshots。
- save/load log。

### 9.6 Simple Batch Gameplay Subagent

职责：

- Purifier。
- Upgrade Shrine。
- Golden Shrine。
- The Cleric。
- Old Beggar / Pleading Vagrant。
- Shining Light。

输出：

- `simple-batch-runtime-evidence.md`
- screenshots。
- result logs。

### 9.7 Asset + Localization Subagent

职责：

- ZHS render。
- missing key scan。
- image extraction/hash/local license decision。
- image render screenshots。

输出：

- `localization-render-proof.md`
- `asset-manifest.csv`
- `image-license-decision.md`
- screenshots。

### 9.8 Event Pool / RNG / Save Subagent

职责：

- ReplacementPrototype functional proof。
- event bag。
- visited ids。
- no-repeat。
- save/load。

输出：

- `replacement-functional-proof.md`
- `event-bag-save-load-proof.md`

### 9.9 Multiplayer / IsShared Subagent

职责：

- per-event `IsShared` matrix。
- combat events shared/voting。
- fail-closed co-op if not verified。

输出：

- `is-shared-matrix.csv`
- `multiplayer-fail-closed-proof.md`

### 9.10 Content Parity Subagent

职责：

- missing curses。
- Golden Idol / Bloody Idol。
- Bite。
- face relics。
- combat encounter models。
- temporary substitute tagging。

输出：

- `content-parity-gap-matrix.md`
- `temporary-substitutes.md`
- `combat-blockers.md`

### 9.11 QA / Red-Team Subagent

职责：

- 不写实现。
- 只验收。
- 逐 gate pass/fail。
- 拒绝无证据 Done。

输出：

- `qa-redteam-report.md`

### 9.12 Release Documentation Subagent

职责：

- status-board。
- monthly review。
- handoff。
- release evidence。
- owner actions。

输出：

- `monthly-review-2026-06.md`
- `handoff.md`
- `release-evidence-status.md`

---

## 10. 给执行助理的直接指令

```text
当前状态不能标完成。你这轮 code-side work 有进展，但 runtime gameplay、save/load、event images、ReplacementPrototype functional proof、combat encounter models、independent QA 仍然 blocked 或未验证。

不要写 all tasks complete、full parity、release-ready、和杀戮尖塔1完全一样。

继续 Mandatory Overnight Run v11。你只能在 O0-O42 全绿后停止；若因为 game launch、licensed art、external QA、owner action 无法继续，必须输出 HARD STOP BLOCKER REPORT。Blocked gate 不得标完成。

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

---

## 11. 管理红线

- 不要把 code-side completion 当 gameplay completion。
- 不要把 automated tests 当 runtime proof。
- 不要把 registry count 当 event parity。
- 不要把 hard-stop blocker report 当完成。
- 不要继续扩大范围。
- 不要把 AdditiveAllDraft 当 AdditiveBatch1。
- 不要把图片脚本当图片完成。
- 不要把 ZHS JSON 当渲染完成。
- 不要把 replacement source guard 当 functional proof。

最终建议：**继续优化 + 有限推进，两者兼顾，但优化优先。先把 4 canary + 6 simple batch 的 runtime、图片、本地化、save/load、replacement pool 跑绿，再谈下一批事件。**
