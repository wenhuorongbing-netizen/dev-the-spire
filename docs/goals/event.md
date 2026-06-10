结论：**当前仍未完成。**
但这次要更新判断：项目已经从早期“错误声称完成/注册脚手架阶段”推进到 **source/doc safety + Off/Canary loader gate 明显改善** 的阶段。现在正确决策是：**继续优化 + 有限推进，两者兼顾，但优化优先**。不要扩大事件范围；先把 4 个 canary、6 个 simple batch、replacement pool 的 runtime 证据跑绿。

我整理了新版完整审查文档：

[下载完整审查文档 v18](sandbox:/mnt/data/sts1_event_port_strict_audit_v18_monthly_overnight.md)

## 2026-06-10 current status override

This goal document is historical below this override unless a row is repeated in current validation docs.
Use `PROJECT_STATE.md`, `docs/reviews/current-validation.md`, and `docs/goals/m5-revision-l-*` as the current source of truth.

- Current dirty-source no-game validation passes: solution build has 0 errors and 0 warnings after expanded Sts1Events owner guards; test-project and exact solution-level no-build lanes both report 464 passed / 0 failed / 21 skipped / 485 total; format, diff-check, patch-inventory, and batch-classifier checks pass.
- Historical Off, CanaryOnly, and AdditiveBatch1 loader-gate evidence remains useful for the `v0.106.1` setup only.
- Current runtime proof is blocked on local Slay the Spire 2 `v0.107.0`: installed RitsuLib `v0.4.16` / `lib\0.107.0` is present and installed beta.84 package parity is restored, but the fresh beta.84 Off smoke failed clean audit on stale package API targets.
- Do not claim tester-ready, live-ready, runtime-ready, or release-ready from this document.

## 严格审核总判定

当前可以认可的进展：

| 模块                              | 审核结论                                                      |
| ------------------------------- | --------------------------------------------------------- |
| Build                           | 通过，当前记录为 `0 errors / 0 warnings`                          |
| Tests                           | 通过，当前记录为 `464 passed / 0 failed / 21 skipped / 485 total` |
| StS1 guard tests                | 通过，当前记录为 `31 passed / 0 failed / 0 skipped`               |
| Format / diff / patch inventory | 通过                                                        |
| Runtime dependency              | E 盘 game-root 下 BaseLib、EZMicroBalance、STS2-RitsuLib 均存在  |
| Off loader smoke                | 通过，达到 main menu，clean audit，StS1 默认 Off                   |
| CanaryOnly loader smoke         | 通过，达到 main menu，clean audit，注册 4 个 canary                 |
| Unsafe gates                    | 已加强，`AdditiveAllDraft` 和 `ReplacementPrototype` 不会普通模式误开  |
| Status board                    | 已从泛泛 `Done` 改成更接近证据状态                                     |

这些结论有当前仓库验证记录支持：v15 continuation build/test/format/diff 均通过，StS1 feature guard tests 为 31 passed；Off 和 CanaryOnly target-fix runtime smokes clean，并且 CanaryOnly 注册了 Big Fish、Golden Idol、The Lab、Divine Fountain。

但还不能认可的部分更关键：

| 模块                         | 当前状态                                                |
| -------------------------- | --------------------------------------------------- |
| Canary gameplay            | 未完成：缺事件内截图、结果日志、pre/post state、save/load            |
| Simple batch gameplay      | 未完成：历史 AdditiveBatch1 loader proof 已有；缺当前 `v0.107.0` loader 复验、事件内截图、结果日志、pre/post state、EN/ZHS render、image/license/render，必要时 save/load |
| Event images               | 未完成：status board 仍是 0 张 redistributable art         |
| ReplacementPrototype       | 未完成：source/fail-closed 不等于 unknown room 功能验证        |
| Multiplayer / fail-closed  | 未完成：仍需 runtime proof                                |
| Combat events              | blocked：缺 encounter models                          |
| QA / Red-Team              | 只能 conditional loader pass，不是 release/gameplay pass |
| Full StS1 event experience | 未完成                                                 |

status board 当前也明确写着：event images 为 0；Canary runtime、simple batch runtime、replacement functional proof、multiplayer、QA Red-Team 都仍 blocked 或 unverified。

## 与目标对比

我们的目标不是“事件类能编译”，而是让 StS2 mod 里的 unknown room 事件体验尽量接近《杀戮尖塔 1》：Act bucket、shared/semi-common/exclusive 规则、事件选项、奖励、诅咒、遗物、药水、A15 差异、图片、本地化、save/load、co-op/`IsShared`、replacement event pool 都要闭环。

StS1 Wiki 对 events 的定义很明确：事件来自 unknown location，事件由随机和当前 Act 决定；有些事件只在特定 Act 出现，有些可跨 Act；Act 4 没有 unknown location/event；Ascension 15 会让部分不利事件更可能或更强。Wiki 还列出 16 个 shared events、12 个 Act 1 exclusive、16 个 Act 2 exclusive、8 个 Act 3 exclusive。([slay-the-spire.fandom.com][1])

所以，当前这些数字：

```text
52 public wiki baseline
54 canonical rows
50 registry entries
56 registration calls
48 model files
47 compiling models
```

只能说明内部映射结构复杂，**不能直接等同于“StS1 全事件完成”**。status board 当前也把这些数字分开列了，说明它们不是同一个完成口径。

Canary 的标准也不能降低。Big Fish 必须是 Act 1 事件，并实现 Banana 回复 1/3 最大生命且向下取整、Donut +5 最大生命、Box 给随机 common/uncommon/rare relic 并加入 Regret。Golden Idol 必须是 Act 1 事件，Take 后获得 Golden Idol 并触发陷阱；Outrun 给 Injury，Smash 造成 25%/35% 最大生命伤害，Hide 损失 8%/10% 最大生命，Leave 无事发生。这里的精确数值必须继续用 Wiki/spec/source 三方复核，不得只靠代码注释验收。

## 历史问题必须继续防回归

早期记录显示，助理曾经把错误 Act 映射写入注册服务注释，把 `Sts1EventRegistrationService.RegisterAll(ModId)` 接进 `MainFile.Initialize()`，并声称“46 event code files、48 spec docs、localization、RitsuLib registration are done—build passes”。这些都是过度声明或高风险实现。

早期状态板也曾把 `Infrastructure`、`event-specs`、`assets.md`、`localization.md`、`test-plan.md` 写成 `Done`，同时仍列着 Regret、Injury、random relic helper、card UI、combat encounter models 等 blocker。这个历史问题说明，接下来必须坚持“没有 source/API/test/screenshot/log/save-load 证据，不得标完成”。

## 管理决策

**继续优化 + 有限推进，两者兼顾，但优化优先。**

继续优化：

```text
- warning budget regression guard（当前 dirty source 已清到 0 warnings）
- 21 skipped tests 解释
- dirty worktree classification
- status-board 证据状态
- 52 / 54 / 50 / 48 / 47 canonical matrix
- AdditiveBatch1 vs AdditiveAllDraft 命名边界
- image/license strategy
- runtime proof
- independent QA
```

有限推进：

```text
只推进 verified scope：

4 canary:
- Big Fish
- Golden Idol
- The Lab
- Divine Fountain

6 simple batch:
- Purifier
- Upgrade Shrine
- Golden Shrine
- The Cleric
- Old Beggar / Pleading Vagrant
- Shining Light
```

暂停扩大：

```text
- broad Phase 2/3/4 扩张
- combat full implementation
- custom UI full implementation
- full parity 宣称
- release-ready 宣称
```

## 下个月开发规范

目标名称：

**`StS1 Event Port Prototype Batch 1 — Runtime Parity Foundation`**

月末 Go/No-Go 标准：

1. Full build exit code 0，并保存完整 log。
2. Build warnings must remain at 0, or any regression must have a documented warning budget before handoff.
3. Full tests exit code 0。
4. 21 skipped tests 逐条解释。
5. Worktree 状态明确：clean，或 dirty 但 owner-approved。
6. Off / CanaryOnly / AdditiveBatch1 都有 clean loader proof；现有 clean 证据只适用于历史 `v0.106.1` 环境。当前 `v0.107.0` 已有 RitsuLib/runtime 兼容和 beta.84 安装包 hash parity，但 Off smoke clean audit 失败；必须修复 package/source drift 后重跑。
7. Default Off runtime proof：0 个 StS1 registration。
8. CanaryOnly runtime proof：精确 4 个 canary。
9. AdditiveBatch1 runtime proof：10 event types / 13 calls。
10. AdditiveAllDraft 只能 unsafe-only。
11. ReplacementPrototype 只能 debug + unsafe-only。
12. `52 / 54 / 50 / 48 / 47` matrix 经 Red-Team 复核。
13. 4 个 canary 完成 runtime proof：截图、结果日志、pre/post state、save/load、EN/ZHS、image/license/render。
14. 6 个 simple batch 完成 runtime proof：截图、结果日志、EN/ZHS、image/license/render，必要时 save/load。
15. ReplacementPrototype 完成功能证明：unknown room 只抽 StS1 candidates，Act bucket 正确，event bag/no-repeat 和 save/load 正确。
16. Multiplayer/fail-closed runtime proof 完成。
17. Combat events 在 encounter models 存在前继续 blocked。
18. Temporary substitutes 必须继续标 `temporary-substitute` 和 non-parity。
19. Independent QA/Red-Team 对每个 gate 给 pass/fail。
20. Monthly review 和 handoff docs 更新。
21. 不 commit/push，除非验证证据支持本次 scope 的真实声明。

项目边界仍然不变：当前 active deliverable 是 `Spire Plus`，技术 id 仍是 `EZMicroBalance`；发布和代码资源路径仍按 `EZMicroBalance` 体系维护。 原版素材和大段反编译代码不能随意提交；项目 README 也明确要求不要复制原版资产或大段反编译代码。

## Mandatory Overnight Run v18

停止条件只能是：

```text
A. O0-O70 全部 GREEN；
或
B. 输出 HARD STOP BLOCKER REPORT：
   - exact gate id
   - blocker reason
   - evidence path
   - attempted actions
   - owner action
   - why continuation is impossible in current environment
```

Hard stop 只能暂停，**不代表完成**。

不能因为这些就停止：

```text
build passes
tests pass
format passes
guard tests pass
loader reaches menu
Off/Canary loader proof exists
source files exist
localization JSON exists
status-board updated
canonical matrix exists
hard-stop report exists
all code-side work complete
```

核心 gates：

| Gate    | 必须结果                                                                                                                                                     |
| ------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- |
| O0-O10  | worktree、build、tests、skips、warnings、format、diff、patch inventory、batch classification、dirty worktree owner decision 全部记录                                  |
| O11-O15 | status-board、canonical matrix、Act mapping、feature gate 全部过审                                                                                              |
| O16-O23 | Off、CanaryOnly、AdditiveBatch1、AdditiveAllDraft、ReplacementPrototype 的 source/runtime gate 证明                                                             |
| O24-O28 | BaseLib/RitsuLib/Spire Plus 路径、godot.log、Off/Canary/AdditiveBatch1 clean loader audit                                                                    |
| O29-O36 | 4 个 canary 的 code review、runtime screenshot、result log、pre/post state、save/load、EN/ZHS、image/license/render proof                                        |
| O37-O47 | 6 个 simple batch 的 spec/code/runtime/save-load/localization/image proof                                                                                  |
| O48-O52 | ReplacementPrototype source guard、unknown room proof、Act bucket proof、event bag proof、save/load proof                                                    |
| O53-O59 | multiplayer、IsShared、combat blockers、temporary substitutes、content parity gaps、asset/license、ZHS screenshots                                             |
| O60-O70 | independent QA、monthly review、current-validation、status-board、handoff、owner actions、no unsupported commit/push、最终 summary 必须诚实列出 remaining blocked gates |

## 必须使用 subagent

这次必须明确要求助理使用 subagent，且实现者不能审核自己的实现：

1. **BuildGate / Repo Health**：build/test/format/diff logs、warning budget、skipped tests、worktree。
2. **Runtime Environment Bootstrap**：game-root paths、BaseLib、RitsuLib、EZMicroBalance、godot.log、loader audits。
3. **Wiki Parity Spec Auditor**：52 public target、54 canonical rows、exact options、A15、semi-common membership。
4. **StS2 Source/API Auditor**：EventModel、ActModel、RitsuLib、card/relic/potion/gold/HP/save/replacement API。
5. **Feature Gate / Registration Engineer**：Off、CanaryOnly、AdditiveBatch1、AdditiveAllDraft、ReplacementPrototype。
6. **Canary Gameplay Subagent**：Big Fish、Golden Idol、Lab、Divine Fountain runtime proof。
7. **Simple Batch Gameplay Subagent**：Purifier、Upgrade Shrine、Golden Shrine、The Cleric、Old Beggar/Pleading Vagrant、Shining Light runtime proof。
8. **Asset + Localization Subagent**：EN/ZHS render、missing key scan、image/license/render decision。
9. **Event Pool / RNG / Save Subagent**：replacement pool、seeded unknown rooms、event bag、visited ids、save/load。
10. **Multiplayer / IsShared Subagent**：per-event IsShared、combat true、fail-closed multiplayer proof。
11. **Content Parity Subagent**：Bite、face relics、Golden/Bloody Idol、Parasite/Madness、combat encounter models、temporary substitutes。
12. **QA / Red-Team Subagent**：只验收，不实现；逐 gate pass/fail。
13. **Release Documentation Subagent**：status-board、current-validation、monthly review、handoff、release evidence、owner actions。

## 直接发给他的指令

```text
当前状态不能标完成。

你已经推进了 source/doc safety 和 loader gates：unsafe modes 有保护，Off/Canary loader proof 已有，build/test/format/diff 通过，status-board 更接近证据状态。

但是 StS1 runtime parity 没完成。runtime gameplay、save/load、images、AdditiveBatch1 loader proof、ReplacementPrototype 功能证明、multiplayer proof、combat encounters、independent QA 仍然 blocked 或 unverified。

继续 Mandatory Overnight Run v18。只能在 O0-O70 全绿后停止。若某 gate 在当前环境无法完成，写 HARD STOP BLOCKER REPORT，但 blocked gate 不得标完成。

优先级：
1. 保存 full build/test/format/diff/patch/worktree evidence。
2. 为 89 个 Sts1Events nullable warnings 制定 warning budget。
3. 保持 Off=0、CanaryOnly=4 event types / 6 calls、AdditiveBatch1=10 event types / 13 calls、AdditiveAllDraft unsafe-only、ReplacementPrototype debug/unsafe-only。
4. 补当前 `v0.107.0` 的 Off / CanaryOnly / AdditiveBatch1 clean loader proof；历史 AdditiveBatch1 loader proof 只能作为旧环境证据。
5. 运行游戏验证 4 canary：screenshots、result logs、save/load、EN/ZHS、image/license/render。
6. 运行游戏验证 6 simple batch。
7. 功能性证明 ReplacementPrototype，而不是只证明 source guard。
8. Combat events 在 encounter models 完成前继续 blocked。
9. Temporary substitutes 必须继续标 non-parity。
10. 启动 subagents，QA/Red-Team 必须独立验收，不能实现者自验。
11. 不要 commit/push，除非 validation evidence 支持本次准确 scope。
```

管理红线：**不要把 source-side 进展当 gameplay completion；不要把 loader proof 当 event parity；不要把 registry count 当 StS1 experience；不要把 hard-stop report 当完成；不要继续扩大范围。先把 4 canary + 6 simple batch + replacement pool 的 runtime proof 跑绿。**

[1]: https://slay-the-spire.fandom.com/wiki/Events "Events | Slay the Spire Wiki | Fandom"
