结论：**当前仍未完成，而且 Revision M 让审核口径必须更新。**
此前部分上传文件已经过期；这次审查基于当前 GitHub 仓库里的 `status-board.md` 和 `current-validation.md`，不是基于过期上传文件。

我已整理成新版完整审查文档：

[下载完整审查文档 v19](sandbox:/mnt/data/sts1_event_port_strict_audit_v19_revision_l_monthly_overnight.md)

## 1. 当前严格审核结论

当前不能再沿用上一轮的“v0.106.1 loader gate 基本成立”作为当前证明。现在仓库状态已经推进到 **StS2 `v0.107.0` + RitsuLib `v0.4.16`**。beta.84 Off smoke 曾经是红的；当前 beta.85 Off smoke 已 clean，但这只证明 default-Off loader path，不证明 CanaryOnly、AdditiveBatch1、gameplay、save/load、replacement、multiplayer 或 QA。

当前可以认可的进展：

| 模块                                        | 审核结论                                                                      |
| ----------------------------------------- | ------------------------------------------------------------------------- |
| Source build                              | Revision L 记录为 `0 warnings / 0 errors`；June 11 Divine Fountain、Big Fish、Golden Idol、The Lab、Old Beggar、Shining Light、Golden Shrine、The Cleric、simple-batch spec inventory changes 尚未 build 验证 |
| Tests                                     | Revision L 记录为 `464 passed / 0 failed / 21 skipped / 485 total`；June 11 guards 尚未执行 |
| Feature guard tests                       | Revision L 记录为 `31 passed / 0 failed / 0 skipped`；June 11 新增 Divine Fountain、Big Fish、Golden Idol、The Lab、Old Beggar、Shining Light、Golden Shrine、The Cleric、simple-batch spec inventory guards 后尚未重跑 |
| Format / patch inventory / worktree batch | 通过                                                                        |
| Registration source fix                   | Big Fish、Golden Idol 已改成 Act 1 buckets：`Overgrowth` + `Underdocks`        |
| Current Off loader                         | beta.85 `v0.107.0` Off smoke clean：main menu、RitsuLib compat `0.107.0`、25/25 patches、0 blocking audit hits |
| ZHS placeholder                           | 文件层面 397 keys / 0 placeholder                                             |
| Unsafe gates                              | `AdditiveAllDraft` unsafe-only；`ReplacementPrototype` debug + unsafe-only |
| Status-board                              | 已经不再用泛泛 `Done`，并明确区分 historical loader proof 与 current proof              |

这些有当前 validation 支持：June 10 记录里 build、test、format、patch inventory、worktree batch 都通过；Big Fish 和 Golden Idol 已改为注册到 StS2 Act 1 buckets；Revision L tests 是 `464 / 0 / 21 / 485`；June 11 beta.85 Off smoke 是 clean loader proof。June 11 的 Divine Fountain curse-prerequisite / Drink option source/test/doc change、Big Fish Box source/localization/test/doc change、Golden Idol trap Outrun/Smash/Hide source/localization/test/doc change、The Lab Open-only source/localization/test/doc change、Old Beggar Offer Gold affordability source/test/doc change、Shining Light random-upgrade source/test/doc change、Golden Shrine Pray/Desecrate source/localization/test/doc change、The Cleric A15 Purify/gold eligibility source/localization/test/doc change、simple-batch spec inventory source/test/doc change 比 June 10 test validation 更新，目前只有 static inspection 和 default-Off loader proof，未做 enabled-mode/gameplay proof。

但关键阻塞更严重：

| 模块                                    | 当前状态                              |
| ------------------------------------- | --------------------------------- |
| Current `v0.107.0` Off loader         | **beta.85 default-Off pass**         |
| CanaryOnly current proof              | pending，不能用历史 `v0.106.1` proof 代替 |
| AdditiveBatch1 current proof          | pending，不能用历史 `v0.106.1` proof 代替 |
| Canary gameplay                       | 未完成                               |
| Simple batch gameplay                 | 未完成                               |
| Save/load                             | 未完成                               |
| Images/render                         | 未完成，event images 仍是 0             |
| ReplacementPrototype functional proof | 未完成                               |
| Multiplayer/fail-closed               | 未完成                               |
| Combat encounters                     | blocked                           |
| QA/Red-Team                           | blocked                           |

当前 validation 明确写着：`v0.107.0` beta.84 Off smoke 到达 main menu，但不是 clean runtime proof；根源是 stale `EctoplasmGoldGatePatch` target API drift。Revision M 之后 beta.85 Off smoke 已 clean：`v0.1.0-private-beta.85`、RitsuLib `0.4.16` compat branch `0.107.0`、25/25 Spire Plus patches、main menu、`godot-log-audit.json` clean with 0 blocking signature hits。这个证据只关闭 default-Off loader blocker。

## 2. 当前 status-board 的关键变化

status-board 已在 Revision M 口径下更新到 2026-06-11，明确要求“no generic Done”，并区分 historical enabled-mode proof、current beta.85 default-Off proof、current enabled-mode proof。当前指标是：

```text
Public wiki baseline: 52
Canonical audit rows: 54
Runtime registry entries: 50
RegisterAll registration calls: 57
AdditiveBatch1: 14 calls / 10 event types
Shared event registrations: 14
Model files: 48
Compiling models: 47
Event images: 0
Build: last validated 0 errors / 0 warnings before June 11 Divine Fountain, Big Fish, Golden Idol, The Lab, Old Beggar, Shining Light, and simple-batch spec inventory changes
Tests: last validated 464 passed / 0 failed / 21 skipped / 485 total before June 11 Divine Fountain, Big Fish, Golden Idol, The Lab, Old Beggar, Shining Light, and simple-batch spec inventory guards
Current Off loader: beta.85 v0.107.0 default-Off pass only
```

这意味着上一版的 `54 calls / AdditiveBatch1 11 calls` 口径已过期；当前必须使用 **`57 RegisterAll calls` 与 `14 AdditiveBatch1 calls / 10 event types`**。status-board 解释了原因：Big Fish、Golden Idol、The Cleric、Shining Light 都注册到两个 StS2 Act 1 buckets：`Overgrowth` 和 `Underdocks`。

Canary 当前也不能写成 parity complete。status-board 对四个 canary 明确列出当前差距：Big Fish source/localization 已改为 Wiki-aligned “Box”，但 runtime UI/bucket proof 仍 pending；Golden Idol trap source/localization 已改为 Outrun/Smash/Hide，但 runtime UI/result proof 仍 pending，且尚无 Golden Idol relic model，当前给 random relic；The Lab source/localization 已改为 Open-only 并标明 3 potions / A15+ 2 potions，但 runtime UI/result proof 仍 pending；Divine Fountain 的 curse prerequisite 和 Drink option identity 已 source/localization-guarded，但 runtime selection/UI proof 仍 pending。

## 3. 为什么仍然不能算完成

StS1 的事件目标不是“注册 EventModel”。StS1 Wiki 明确说明：事件来自 unknown location；是否遇到事件以及遇到哪个事件由随机和当前 Act 决定；有些事件限定 Act，有些可跨 Act；Act 4 没有 unknown location/event；Ascension 15 会增强部分不利事件。Wiki 列表也明确给出 16 个 shared、12 个 Act 1 exclusive、16 个 Act 2 exclusive、8 个 Act 3 exclusive。([Slay the Spire Wiki][1])

所以当前这些数字：

```text
52 / 54 / 50 / 48 / 47 / 57 / 14
```

只能说明内部 canonical / registry / model / registration 结构，**不能等同于 StS1 full parity**。

更直接地说：当前 `v0.107.0` beta.85 Off loader 已经 clean，但这不是 event runtime parity。不能拿 default-Off loader proof 宣称 CanaryOnly/AdditiveBatch1 可玩，更不能宣称 event gameplay parity。status-board 也明确写了：CanaryOnly、AdditiveBatch1、事件截图、save/load、EN/ZHS render、image/license、replacement、multiplayer、QA 全部仍 blocked 或 pending。

## 4. 管理决策

**继续优化 + 有限推进，两者兼顾，但优化优先。**

现在不要继续扩大事件范围，也不要启动 broad Phase 2/3/4。当前优先级是：

```text
1. 保留 beta.85 default-Off clean loader proof，不把它扩张成 gameplay parity。
2. 重跑 current `v0.107.0` CanaryOnly smoke，证明 exact 4。
3. 重跑 current `v0.107.0` AdditiveBatch1 smoke，证明 10 event types / 14 calls。
4. 之后才做 4 canary + 6 simple batch 的 gameplay proof。
5. 最后做 ReplacementPrototype functional proof。
```

有限推进范围仍然只允许：

```text
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

## 5. 下个月开发规范

目标名称：

**`StS1 Event Port Prototype Batch 1 — v0.107 Runtime Parity Foundation`**

月末 Go/No-Go 标准：

1. Build：`0 errors / 0 warnings`，保存完整 log。
2. Tests：`464 / 0 / 21 / 485` 或更新后的准确总数，保存完整 log。
3. 21 skipped tests 逐条解释。
4. Format、diff、patch inventory、worktree batch classification 全部通过。
5. Dirty worktree 需要 clean，或 owner-approved exact scope。
6. 修复 stale `EctoplasmGoldGatePatch` target API drift。
7. 重新 publish / reinstall fixed package，并记录 package SHA。
8. Current `v0.107.0` Off smoke clean：0 Godot ERROR、0 Spire Plus error/exception、0 MissingMethodException、0 TypeLoadException、无 stale target API drift。（beta.85 default-Off proof exists; preserve as evidence, rerun only when package/source changes require it。）
9. Current `v0.107.0` CanaryOnly smoke clean：exact 4。
10. Current `v0.107.0` AdditiveBatch1 smoke clean：10 event types / 14 calls。
11. `AdditiveAllDraft` 继续 unsafe-only。
12. `ReplacementPrototype` 继续 debug + unsafe-only。
13. Count matrix 经 Red-Team 复核：52 public wiki baseline、54 canonical rows、50 registry entries、48 model files、47 compiling models、57 RegisterAll calls、14 AdditiveBatch1 calls。
14. 4 个 canary 完成 runtime proof：screenshots、result logs、pre/post state、save/load、EN/ZHS render、image/license/render。
15. 6 个 simple batch 完成 runtime proof。
16. ReplacementPrototype functional proof：unknown rooms only draw StS1 candidates、act bucket correct、event bag/no-repeat、save/load。
17. Multiplayer/fail-closed runtime proof。
18. Combat events 继续 blocked，直到 encounter models 存在。
19. Temporary substitutes 继续标 non-parity。
20. Independent QA/Red-Team 逐 gate pass/fail。
21. current-validation、status-board、monthly review、handoff docs 全部更新。
22. 不 commit/push，除非 evidence 支持 exact scope。

项目边界仍然不变：当前 active deliverable 是 `Spire Plus`，technical id 仍是 `EZMicroBalance`。 项目 release policy 也明确要求不要复制原版资产或大段反编译代码。

## 6. Mandatory Overnight Run v19

停止条件只有两个：

```text
A. O0-O76 全部 GREEN
B. 输出 HARD STOP BLOCKER REPORT
```

Hard stop report 必须包含：

```text
exact gate id
blocker reason
evidence path
attempted actions
owner action
why continuation is impossible in current environment
```

Hard stop 只允许暂停，**不代表完成**。

不能因为这些停止：

```text
build passes
tests pass
format passes
guard tests pass
historical v0.106 proof exists
source files exist
status-board updated
canonical matrix exists
hard-stop report exists
all code-side work complete
```

核心 gates：

| Gate    | 必须结果                                                                                                                                        |
| ------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| O0-O10  | worktree、build、tests、skips、zero-warning、format、diff、patch inventory、dirty-worktree owner decision                                           |
| O11-O20 | status-board、canonical matrix、Act mapping、feature gate、Off/Canary/Additive source guard                                                     |
| O21-O29 | v0.107 paths、fixed package SHA、godot.log、Off/Canary/Additive clean loader audits                                                            |
| O30-O41 | 4 canary code review、runtime screenshots、result logs、pre/post state、save/load、EN/ZHS、image/license、parity gaps                              |
| O42-O52 | 6 simple batch spec/code/runtime/save-load/EN-ZHS/image proof                                                                               |
| O53-O57 | ReplacementPrototype source guard、unknown-room proof、Act bucket proof、event bag proof、save/load                                             |
| O58-O64 | multiplayer、IsShared、combat blockers、temporary substitutes、content parity、asset/license、ZHS screenshots                                     |
| O65-O76 | independent QA、current-validation、status-board、monthly review、handoff、owner actions、no unsupported commit/push、最终 summary 诚实列 blocked gates |

## 7. 必须使用 subagent

这次要新增一个专门的 **API Drift Fix Subagent**。完整 subagent 分工：

1. **BuildGate / Repo Health**：build/test/format/diff/patch/worktree、skipped tests、zero-warning proof。
2. **Runtime Environment Bootstrap**：v0.107 game-root、BaseLib、RitsuLib v0.4.16、EZMicroBalance package SHA、godot.log、loader audit。
3. **API Drift Fix Subagent**：修 stale `EctoplasmGoldGatePatch` target API、optional ModPatcher failures、republish/reinstall。
4. **Wiki Parity Spec Auditor**：52 public events、54 canonical rows、exact options、A15、semi-common membership。
5. **StS2 Source/API Auditor**：EventModel、ActModel、RitsuLib、card/relic/potion/gold/HP/save/replacement APIs。
6. **Feature Gate / Registration Engineer**：Off、CanaryOnly、AdditiveBatch1、AdditiveAllDraft、ReplacementPrototype。
7. **Canary Gameplay Subagent**：Big Fish、Golden Idol、Lab、Divine Fountain runtime proof。
8. **Simple Batch Gameplay Subagent**：Purifier、Upgrade Shrine、Golden Shrine、The Cleric、Old Beggar/Pleading Vagrant、Shining Light runtime proof。
9. **Asset + Localization Subagent**：EN/ZHS render、missing-key scan、image/license/render decision。
10. **Event Pool / RNG / Save Subagent**：replacement pool、seeded unknown rooms、event bag、visited ids、save/load。
11. **Multiplayer / IsShared Subagent**：per-event IsShared、combat true、fail-closed multiplayer proof。
12. **Content Parity Subagent**：Bite、face relics、Golden/Bloody Idol、Parasite/Madness、combat encounter models、temporary substitutes。
13. **QA / Red-Team Subagent**：独立逐 gate pass/fail，不写实现。
14. **Release Documentation Subagent**：status-board、current-validation、monthly review、handoff、release evidence、owner actions。

## 8. 直接发给他的指令

```text
当前状态不能标完成。

Revision L 显示 pre-June-11 source/test 已明显改善：build 0 warnings/0 errors，tests 464/0/21/485，feature guards 31/0/0，Big Fish 和 Golden Idol 已改为 Act 1 bucket 注册。Revision M 又关闭了 default-Off loader blocker，并让 status-board 明确区分 default-Off proof 与 enabled-mode/gameplay proof。

beta.84 Off smoke 曾经是红的：11 Godot ERROR、1 Spire Plus error/exception、8 optional ModPatcher failures、EctoplasmGoldGatePatch target API drift。Revision M / beta.85 已关闭 default-Off loader blocker：current v0.107.0 Off smoke clean，25/25 patches，0 blocking audit hits。历史 v0.106.1 Canary/AdditiveBatch1 clean loader proof 只能保留为 historical-loader-verified，不能当 current enabled-mode proof。

继续 Mandatory Overnight Run v19。只能在 O0-O76 全绿后停止。若某 gate 当前环境无法完成，写 HARD STOP BLOCKER REPORT，但 blocked gate 不得标完成。

优先级：
1. 保留 beta.85 Off loader proof as default-Off only，不扩张成 event gameplay proof。
2. 跑 current v0.107 CanaryOnly smoke 到 clean，证明 exact 4。
3. 跑 current v0.107 AdditiveBatch1 smoke 到 clean，证明 10 event types / 14 calls。
4. 再做 4 canary gameplay：screenshots、result logs、pre/post state、save/load、EN/ZHS、image/license/render。
5. 再做 6 simple batch gameplay。
6. 功能性证明 ReplacementPrototype，不只是 source guard。
7. Combat events 在 encounter models 完成前继续 blocked。
8. Temporary substitutes 必须继续标 non-parity。
9. 启动 subagents，QA/Red-Team 必须独立验收。
10. 不要 commit/push，除非 validation evidence 支持本次准确 scope。
```

管理红线：**不要把 source/test pass 当 gameplay completion；不要把 beta.85 default-Off loader proof 当 CanaryOnly/AdditiveBatch1/gameplay proof；不要把 historical v0.106 enabled-mode loader proof 当 current v0.107 proof；不要把 registry count 当 StS1 experience；不要把 hard-stop report 当 completion。**

[1]: https://slay-the-spire.fandom.com/wiki/Events "Events | Slay the Spire Wiki | Fandom"
