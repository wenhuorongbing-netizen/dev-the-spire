# StS1 Event Port Strict Audit v17 — Current State, June Dev Spec, Subagents, Mandatory Overnight Run

Date: 2026-06-02
Target project: `dev-the-spire` / `Spire Plus` / technical id `EZMicroBalance`
Scope: strict audit of the assistant's work on porting Slay the Spire 1 events into the Slay the Spire 2 mod.

---

## 0. Executive Verdict

**Not complete.**

The current state has materially improved from the early false-completion phases. Source safety, feature gates, loader prerequisites, build/test validation, and docs truthfulness are now much better. However, the StS1 event port is **not** runtime parity, not gameplay-ready, and not release-ready.

Correct label:

> `StS1 Event Port Prototype Batch 1 — source/loader foundation advanced; runtime parity still blocked.`

Do not write:

- all tasks complete
- all StS1 events complete
- full parity complete
- gameplay-ready
- release-ready
- 和杀戮尖塔 1 完全一样

---

## 1. Current Evidence Snapshot

### 1.1 Project boundaries

The active deliverable remains one mod: `Spire Plus`. The technical manifest id, code/project identity, install folder, saved-field prefix, and compatibility surface remain `EZMicroBalance`.

Required active surfaces:

```text
EZMicroBalanceCode/     C# source
EZMicroBalance/         Godot resources, localization, images
EZMicroBalance.json     stable technical manifest id
EZMicroBalance.dll
EZMicroBalance.pck
```

Hard rules:

- Do not rename the technical manifest id in-place.
- Do not copy original game assets or large decompiled code bodies into the repo.
- Original art must not enter tracked/public files unless redistribution permission is confirmed.
- If no redistributable StS1 event art exists, use local extraction proof, owner-provided licensed art, generated replacement art, or mark as non-parity placeholder.

### 1.2 Current validation state

Accepted current evidence:

- Build: `0 errors / 89 warnings`.
- Full tests: `464 passed / 0 failed / 21 skipped / 485 total`.
- StS1 feature guard tests: `31 passed / 0 failed / 0 skipped`.
- Format check: pass.
- Diff check: pass.
- Patch inventory check: pass.
- Worktree batch classification: pass.
- Worktree remains dirty.
- No commit/push was performed.

Important caveats:

- The 89 warnings are all StS1Events nullable staging warnings and still need a warning budget or reduction.
- The 21 skipped tests need a current explanation table.
- Dirty worktree means commit/push must remain blocked unless owner explicitly accepts the exact dirty-file scope.
- Build/test/format/diff success is not gameplay proof.

### 1.3 Loader/runtime state

Accepted current loader evidence:

- E-drive game root exists.
- BaseLib exists under the E-drive game root.
- EZMicroBalance exists under the E-drive game root.
- `STS2-RitsuLib` `v0.3.10` exists under the E-drive game root and includes a `0.106.1` runtime variant.
- Off-mode loader smoke reaches main menu with clean audit.
- CanaryOnly loader smoke reaches main menu with clean audit.
- Off-mode proves 0 StS1 registrations.
- CanaryOnly proves exactly these four registrations:
  - `Sts1BigFish`
  - `Sts1GoldenIdol`
  - `Sts1TheLab`
  - `Sts1DivineFountain`

Not accepted yet:

- Canary gameplay proof.
- AdditiveBatch1 loader proof.
- Simple batch gameplay proof.
- Event result logs.
- Save/load proof.
- Image render proof.
- EN/ZHS in-game render proof.
- ReplacementPrototype functional proof.
- Multiplayer/fail-closed runtime proof.
- Independent QA release/gameplay pass.

### 1.4 Status-board state

Current status-board metrics:

```text
Public wiki baseline: 52
Canonical audit rows: 54
Runtime registry entries: 50
Registration calls: 54
AdditiveBatch1 registration calls: 11 / 10 event types
Shared event registrations: 17
Model files: 48
Compiling models: 47
EN localization keys: 399
ZHS localization keys: 399 / 0 placeholder
Event images: 0
Build: 0 errors / 89 warnings
Tests: 464 passed / 0 failed / 21 skipped / 485 total
```

Status-board phase interpretation:

- Canary 4: compiled, test-guarded, source/API verified, runtime unverified.
- AdditiveBatch1 10 event types: compiled/source-guarded, runtime unverified.
- Combat: blocked pending encounter models and runtime parity proof.
- Custom UI: mostly compiled, but N'loth blocked.
- Duplicator: compile-excluded because required selector APIs are absent.
- Temporary substitutes remain non-parity.

---

## 2. Historical Audit Findings

Earlier assistant work incorrectly claimed completion. Specific historical problems:

1. It claimed `46 event models Done / 48 specs Done / build passes`, despite unresolved blockers.
2. It wrote `Sts1EventRegistrationService.RegisterAll(ModId)` directly into `MainFile.Initialize()`, creating a risk that StS1 events register by default.
3. It initially wrote the wrong StS2 act mapping: `Underdocks = Act 1`, `Overgrowth = Act 2`, `Hive = Act 3`.
4. It used generic `Done` for docs/spec/assets/localization/test-plan even while Regret, Injury, random relic helper, card UI, and combat models were blocked.
5. It treated RitsuLib additive registration as if it were equivalent to StS1 event-pool parity.

Current work has corrected many of these:
- default Off is now guarded;
- CanaryOnly exact four loader proof exists;
- unsafe modes are gated;
- status-board language is more honest;
- correct act mapping is now expected:
  - StS1 Act 1 → `Overgrowth` + `Underdocks`
  - StS1 Act 2 → `Hive`
  - StS1 Act 3 → `Glory`

But the historical errors justify continued hard gating and Red-Team review.

---

## 3. Target Definition

The target is **not** “event classes compile.”

The target is StS1-like event experience inside StS2:

- unknown-room event pool behavior,
- correct act buckets,
- shared / semi-common / exclusive membership,
- correct event pages and option flow,
- locked options,
- correct reward/card/relic/curse/potion/gold/HP/max HP effects,
- Ascension 15 changes,
- EN/ZHS text render,
- event images or explicit non-parity placeholders,
- save/load stability,
- multiplayer / `IsShared` correctness,
- default Off so normal Spire Plus is not polluted,
- safe debug/unsafe gating for replacement mode,
- independent QA.

---

## 4. Strict Current Gap Analysis

### 4.1 Code-side status

Code-side foundation has advanced. It is fair to claim:

- feature-gated StS1 registration exists;
- unsafe modes are guarded;
- Off/Canary loader proof exists;
- canary source/API proof exists;
- AdditiveBatch1 source scope exists;
- tests pass;
- build passes with warnings.

It is not fair to claim:

- runtime parity;
- full StS1 event completion;
- gameplay correctness;
- replacement-pool correctness;
- image/localization render completeness.

### 4.2 Runtime status

Still incomplete.

Required runtime evidence is missing for:

- Big Fish event state changes,
- Golden Idol branch state changes,
- Lab potion grants,
- Divine Fountain curse removal,
- Purifier,
- Upgrade Shrine,
- Golden Shrine,
- The Cleric,
- Old Beggar / Pleading Vagrant,
- Shining Light,
- AdditiveBatch1 loader proof,
- save/load,
- EN/ZHS layout,
- images,
- replacement pool.

### 4.3 Asset status

Event images are `0`.

This blocks any “完全一样” claim. Without redistributable StS1 art, choose exactly one path:

1. Owner-provided licensed art.
2. Local extraction with hash proof, not committed.
3. Generated replacement art, explicitly non-identical.
4. Temporary placeholder marked non-parity.

### 4.4 Content parity status

Temporary substitutes are not parity:

- Face Trader → random relic instead of face relics.
- Nest → Clumsy instead of Parasite.
- Vampires → cannot add Bite.
- Mind Bloom War → blocked.
- Winding Halls → Debt instead of Madness.

These must stay marked `temporary-substitute` and cannot be counted as full parity.

### 4.5 Combat status

Combat events remain blocked until encounter models and runtime proof exist.

Blocked/partial:
- Dead Adventurer
- Scorpion Nest
- Treasure Ooze
- Masked Bandits
- Mysterious Sphere
- Mind Bloom War

Do not classify combat events as complete while encounter models are missing.

### 4.6 Replacement pool status

Still not complete.

A source guard or fail-closed mode is not enough. The replacement prototype must prove:

- unknown rooms draw only StS1 candidates;
- act buckets match StS1;
- no-repeat/event-bag behavior is deterministic and stable;
- save/load preserves the replacement state;
- multiplayer behavior is safe.

---

## 5. Management Decision

Decision: **continue optimization + limited advancement, with optimization priority.**

### Continue optimizing

- Warning budget for 89 warnings.
- Skipped-test explanation.
- Dirty-worktree classification.
- Status-board evidence language.
- Count reconciliation.
- AdditiveBatch1 vs AdditiveAllDraft naming clarity.
- Runtime evidence structure.
- Image/license strategy.
- Independent QA.

### Limited advancement

Only advance runtime proof for the verified scope.

Canary:
- Big Fish
- Golden Idol
- The Lab
- Divine Fountain

Simple Batch:
- Purifier
- Upgrade Shrine
- Golden Shrine
- The Cleric
- Old Beggar / Pleading Vagrant
- Shining Light

### Pause broader work

Pause:
- broad event expansion,
- combat full implementation,
- custom UI full parity,
- “all events complete” claims,
- release-ready claims.

---

## 6. June 2026 Monthly Dev Spec

Name:

> `StS1 Event Port Prototype Batch 1 — Runtime Parity Foundation`

### Month-end Go/No-Go

1. Full build exit code 0 with saved log.
2. Warning budget for 89 nullable warnings, or warning reduction.
3. Full tests exit code 0 with saved log.
4. 21 skipped tests explained.
5. Worktree state documented and owner-approved if dirty.
6. Patch inventory and worktree batch classification pass.
7. Off loader proof clean.
8. CanaryOnly loader proof clean.
9. AdditiveBatch1 loader proof clean.
10. Off runtime proof: 0 StS1 registrations.
11. CanaryOnly runtime proof: exactly 4 canary events.
12. AdditiveBatch1 runtime proof: 10 event types / 11 calls.
13. AdditiveAllDraft remains unsafe-only.
14. ReplacementPrototype remains debug + unsafe-only.
15. Count reconciliation Red-Team reviewed:
    - 52 public baseline,
    - 54 canonical rows,
    - 50 registry entries,
    - 48 model files,
    - 47 compiling models.
16. Four canary events runtime verified:
    - screenshots,
    - result logs,
    - pre/post state,
    - save/load,
    - EN/ZHS render,
    - image/license/render decision.
17. Six simple batch events runtime verified:
    - screenshots,
    - result logs,
    - EN/ZHS render,
    - image/license/render decision,
    - save/load where applicable.
18. ReplacementPrototype functional proof complete:
    - unknown rooms only draw StS1 candidates,
    - correct act bucket,
    - event bag/no-repeat proof,
    - save/load proof.
19. Multiplayer/fail-closed runtime proof complete.
20. Combat blockers remain explicit and current.
21. Temporary substitutes remain marked non-parity.
22. Independent QA/Red-Team gives pass/fail by gate.
23. Monthly review, current validation, status-board, and handoff docs updated.
24. No commit/push unless evidence supports the exact claimed scope.

---

## 7. Mandatory Overnight Run v17

The assistant may stop only when:

A. **O0–O66 are all GREEN**, or
B. a **Hard Stop Blocker Report** is written with:
- exact gate id,
- blocker reason,
- evidence path,
- attempted actions,
- owner action,
- why continuation is impossible in current environment.

Hard stop means pause only. It does not mark completion.

### Do not stop for these alone

- build passes;
- tests pass;
- format passes;
- guard tests pass;
- Off/Canary loader proof exists;
- localization keys exist;
- source files exist;
- status-board updated;
- canonical matrix exists;
- hard-stop report exists;
- all code-side work complete.

### O0–O66 Gates

| Gate | Requirement |
|---|---|
| O0 | Worktree snapshot: branch, HEAD, diff, dirty files |
| O1 | Full unfiltered build exit code 0 |
| O2 | Full test exit code 0 |
| O3 | Test count reconciliation |
| O4 | Skipped-test explanation |
| O5 | Warning budget for 89 nullable warnings |
| O6 | Format check pass |
| O7 | Diff check pass |
| O8 | Patch inventory check pass |
| O9 | Worktree batch classification pass |
| O10 | Status-board no false/generic Done |
| O11 | Canonical matrix complete |
| O12 | 52/54/50/48/47 Red-Team reconciliation |
| O13 | Act mapping guard pass |
| O14 | Feature gate tests pass |
| O15 | Off=0 source guard proof |
| O16 | Off=0 runtime loader proof |
| O17 | CanaryOnly=4 exact source guard proof |
| O18 | CanaryOnly=4 exact runtime loader proof |
| O19 | AdditiveBatch1 exact source guard proof |
| O20 | AdditiveBatch1 exact runtime loader proof |
| O21 | AdditiveAllDraft unsafe-only proof |
| O22 | ReplacementPrototype debug/unsafe-only proof |
| O23 | BaseLib/RitsuLib/Spire Plus path report |
| O24 | Active godot.log generated and archived |
| O25 | Loader audit clean for Off |
| O26 | Loader audit clean for CanaryOnly |
| O27 | Loader audit clean for AdditiveBatch1 |
| O28 | Canary code review clean |
| O29 | Big Fish screenshot/result log/pre-post state |
| O30 | Golden Idol screenshot/result log/pre-post state |
| O31 | Lab screenshot/result log/pre-post state |
| O32 | Divine Fountain screenshot/result log/pre-post state |
| O33 | Canary save/load proof |
| O34 | Canary EN/ZHS render proof |
| O35 | Canary image/license/render proof |
| O36 | Simple batch exact spec Red-Team pass |
| O37 | Simple batch code review clean |
| O38 | Purifier runtime proof |
| O39 | Upgrade Shrine runtime proof |
| O40 | Golden Shrine runtime proof |
| O41 | The Cleric runtime proof |
| O42 | Old Beggar / Pleading Vagrant runtime proof |
| O43 | Shining Light runtime proof |
| O44 | Simple batch save/load proof where applicable |
| O45 | Simple batch EN/ZHS render proof |
| O46 | Simple batch image/license/render proof |
| O47 | Replacement source guard pass |
| O48 | Replacement functional proof: unknown rooms only draw StS1 candidates |
| O49 | Replacement Act bucket proof |
| O50 | Event bag / visited ids / no-repeat proof |
| O51 | Replacement save/load proof |
| O52 | Multiplayer fail-closed or verified proof |
| O53 | IsShared matrix current |
| O54 | Combat blocker report current |
| O55 | Temporary substitutes matrix current |
| O56 | Content parity gap matrix current |
| O57 | Asset/license decision current |
| O58 | ZHS render screenshots attached |
| O59 | Independent QA/Red-Team report complete |
| O60 | QA does not self-approve implementation |
| O61 | Monthly review updated |
| O62 | Current validation updated |
| O63 | Handoff docs updated |
| O64 | Owner actions listed |
| O65 | No commit/push unless evidence-supported |
| O66 | Final summary states remaining blocked gates honestly |

---

## 8. Required Subagents

The assistant must use subagents. Implementation agents cannot approve their own work.

1. **BuildGate / Repo Health**
   - build/test/format/diff/patch/worktree evidence, warning budget, skipped tests.

2. **Runtime Environment Bootstrap**
   - game-root paths, BaseLib, RitsuLib, EZMicroBalance, godot.log, loader audits.

3. **Wiki Parity Spec Auditor**
   - 52 public events, 54 canonical rows, exact options, A15 deltas, semi-common membership.

4. **StS2 Source/API Auditor**
   - EventModel, ActModel, RitsuLib, card/relic/potion/gold/HP/save/replacement APIs.

5. **Feature Gate / Registration Engineer**
   - Off, CanaryOnly, AdditiveBatch1, AdditiveAllDraft, ReplacementPrototype.

6. **Canary Gameplay Subagent**
   - Big Fish, Golden Idol, Lab, Divine Fountain runtime proof.

7. **Simple Batch Gameplay Subagent**
   - Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar/Pleading Vagrant, Shining Light runtime proof.

8. **Asset + Localization Subagent**
   - EN/ZHS render, missing-key scan, image/license/render decision.

9. **Event Pool / RNG / Save Subagent**
   - replacement pool, seeded unknown rooms, event bag, visited ids, save/load.

10. **Multiplayer / IsShared Subagent**
    - per-event IsShared, combat true, fail-closed multiplayer proof.

11. **Content Parity Subagent**
    - Bite, face relics, Golden/Bloody Idol, Parasite/Madness, combat encounter models, temporary substitutes.

12. **QA / Red-Team Subagent**
    - independent pass/fail by gate; no implementation.

13. **Release Documentation Subagent**
    - status-board, current-validation, monthly review, handoff, release evidence, owner actions.

---

## 9. Direct Instruction To The Assistant

```text
Current state is not complete.

You have improved source/doc safety and loader gates: unsafe modes are protected, Off/Canary loader proof exists, build/test/format/diff pass, and the status board is more truthful.

But StS1 runtime parity is not complete. Runtime gameplay, save/load, images, AdditiveBatch1 loader proof, ReplacementPrototype functionality, multiplayer proof, combat encounters, and independent QA remain blocked or unverified.

Continue Mandatory Overnight Run v17. You may stop only when O0-O66 are all green. If a gate is impossible in this environment, write a HARD STOP BLOCKER REPORT with exact gate id, reason, evidence, attempted actions, and owner action. Blocked gates must not be marked complete.

Priorities:
1. Preserve full build/test/format/diff/patch/worktree evidence.
2. Produce warning budget for the 89 Sts1Events nullable warnings.
3. Maintain Off=0, CanaryOnly=4, AdditiveBatch1=10 event types / 11 calls, AdditiveAllDraft unsafe-only, ReplacementPrototype debug/unsafe-only.
4. Add AdditiveBatch1 clean loader proof.
5. Runtime verify 4 canary events with screenshots, result logs, pre/post state, save/load, EN/ZHS, and image/license/render proof.
6. Runtime verify 6 simple batch events.
7. Prove ReplacementPrototype functionally, not just by source guard.
8. Keep combat events blocked until encounter models exist.
9. Keep temporary substitutes marked non-parity.
10. Use subagents and require independent QA/Red-Team.
11. Do not commit or push unless validation evidence supports the exact scope being claimed.
```

---

## 10. Red Lines

- Do not convert source-side progress into gameplay completion.
- Do not convert loader proof into event parity.
- Do not convert registry count into StS1 experience.
- Do not convert hard-stop report into completion.
- Do not broaden scope until 4 canary + 6 simple batch + replacement pool runtime proof are green.
结论：**当前仍未完成。**
但要公平地说，状态已经从“错误声称完成/代码脚手架阶段”推进到 **source/doc safety + Off/Canary loader gate 已经基本成立** 的阶段。现在的管理决策应该是：**继续优化 + 有限推进，两者兼顾，但优化优先**。不要扩大事件范围；先把 4 个 canary、6 个 simple batch、replacement pool 的 runtime 证据跑绿。

我已整理成完整交接审查文档：

[下载完整审查文档 v17](sandbox:/mnt/data/sts1_event_port_strict_audit_v17_monthly_overnight.md)

## 严格审核总判定

当前可以认可的进展：

| 模块                              | 审核结论                                                      |
| ------------------------------- | --------------------------------------------------------- |
| Build                           | 通过，当前记录为 `0 errors / 89 warnings`                         |
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
| Simple batch gameplay      | 未完成：缺 AdditiveBatch1 runtime proof                  |
| Event images               | 未完成：status board 仍是 0 张 redistributable art         |
| ReplacementPrototype       | 未完成：source/fail-closed 不等于 unknown room 功能验证        |
| Multiplayer / fail-closed  | 未完成：仍需 runtime proof                                |
| Combat events              | blocked：缺 encounter models                          |
| QA / Red-Team              | 只能 conditional loader pass，不是 release/gameplay pass |
| Full StS1 event experience | 未完成                                                 |

status board 当前也明确写着：event images 为 0；Canary runtime、simple batch runtime、replacement functional proof、multiplayer、QA Red-Team 都仍 blocked 或 unverified。

## 与目标对比

我们的目标不是“事件类能编译”，而是让 StS2 mod 里的 unknown room 事件体验尽量接近《杀戮尖塔 1》：Act bucket、shared/semi-common/exclusive 规则、事件选项、奖励、诅咒、遗物、药水、A15 差异、图片、本地化、save/load、co-op/`IsShared`、replacement event pool 都要闭环。

StS1 Wiki 对 events 的定义很明确：事件来自 unknown location，事件由随机和当前 Act 决定；有些事件只在特定 Act 出现，有些可跨 Act；Act 4 没有 unknown location/event；Ascension 15 会让部分不利事件更可能或更强。Wiki 还列出 16 个 shared events、12 个 Act 1 exclusive、16 个 Act 2 exclusive、8 个 Act 3 exclusive。([Slay the Spire Wiki][1])

所以，当前这些数字：

```text
52 public wiki baseline
54 canonical rows
50 registry entries
54 registration calls
48 model files
47 compiling models
```

只能说明内部映射结构复杂，**不能直接等同于“StS1 全事件完成”**。status board 当前也把这些数字分开列了，说明它们不是同一个完成口径。

Canary 的标准也不能降低。Big Fish 必须是 Act 1 事件，并实现 Banana 回复 1/3 最大生命且向下取整、Donut +5 最大生命、Box 给随机 common/uncommon/rare relic 并加入 Regret。([Slay the Spire Wiki][2]) Golden Idol 必须是 Act 1 事件，Take 后获得 Golden Idol 并触发陷阱；Outrun 给 Injury，Smash 造成 25%/35% 最大生命伤害，Hide 损失 8%/10% 最大生命，Leave 无事发生。([Slay the Spire Wiki][3])

## 历史问题必须继续防回归

早期记录显示，助理曾经把错误 Act 映射写入注册服务注释，把 `Sts1EventRegistrationService.RegisterAll(ModId)` 接进 `MainFile.Initialize()`，并声称“46 event code files、48 spec docs、localization、RitsuLib registration are done—build passes”。这些都是过度声明或高风险实现。

早期状态板也曾把 `Infrastructure`、`event-specs`、`assets.md`、`localization.md`、`test-plan.md` 写成 `Done`，同时仍列着 Regret、Injury、random relic helper、card UI、combat encounter models 等 blocker。这个历史问题说明，接下来必须坚持“没有 source/API/test/screenshot/log/save-load 证据，不得标完成”。

## 管理决策

**继续优化 + 有限推进，两者兼顾，但优化优先。**

继续优化：

```text
- 89 nullable warnings 的 warning budget
- 21 skipped tests 解释
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
2. 89 warnings 要么减少，要么形成 warning budget。
3. Full tests exit code 0。
4. 21 skipped tests 逐条解释。
5. Worktree 状态明确：clean，或 dirty 但 owner-approved。
6. Off / CanaryOnly / AdditiveBatch1 都有 clean loader proof。
7. Default Off runtime proof：0 个 StS1 registration。
8. CanaryOnly runtime proof：精确 4 个 canary。
9. AdditiveBatch1 runtime proof：10 event types / 11 calls。
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

## Mandatory Overnight Run v17

停止条件只能是：

```text
A. O0-O66 全部 GREEN；
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

| Gate    | 必须结果                                                                                                                                           |
| ------- | ---------------------------------------------------------------------------------------------------------------------------------------------- |
| O0-O9   | worktree、build、tests、skips、warnings、format、diff、patch inventory、batch classification 全部记录                                                      |
| O10-O14 | status-board、canonical matrix、Act mapping、feature gate 全部过审                                                                                    |
| O15-O22 | Off、CanaryOnly、AdditiveBatch1、AdditiveAllDraft、ReplacementPrototype 的 source/runtime gate 证明                                                   |
| O23-O27 | BaseLib/RitsuLib/Spire Plus 路径、godot.log、Off/Canary/AdditiveBatch1 clean loader audit                                                          |
| O28-O35 | 4 个 canary 的 code review、runtime screenshot、result log、save/load、EN/ZHS、image/license/render proof                                             |
| O36-O46 | 6 个 simple batch 的 spec/code/runtime/save-load/localization/image proof                                                                        |
| O47-O51 | ReplacementPrototype source guard、unknown room proof、Act bucket proof、event bag proof、save/load proof                                          |
| O52-O56 | multiplayer、IsShared、combat blockers、temporary substitutes、content parity gaps                                                                 |
| O57-O66 | asset/license、ZHS screenshots、independent QA、monthly review、current-validation、handoff、owner actions、最终 summary 必须诚实列出 remaining blocked gates |

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

但是 StS1 runtime parity 没完成。runtime gameplay、save/load、images、ReplacementPrototype 功能证明、multiplayer proof、combat encounters、independent QA 仍然 blocked 或 unverified。

继续 Mandatory Overnight Run v17。只能在 O0-O66 全绿后停止。若某 gate 在当前环境无法完成，写 HARD STOP BLOCKER REPORT，但 blocked gate 不得标完成。

优先级：
1. 保存 full build/test/format/diff/patch/worktree evidence。
2. 为 89 个 Sts1Events nullable warnings 制定 warning budget。
3. 保持 Off=0、CanaryOnly=4、AdditiveBatch1=10 event types / 11 calls、AdditiveAllDraft unsafe-only、ReplacementPrototype debug/unsafe-only。
4. 补 AdditiveBatch1 clean loader proof。
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
[2]: https://slay-the-spire.fandom.com/wiki/Big_Fish "Big Fish | Slay the Spire Wiki | Fandom"
[3]: https://slay-the-spire.fandom.com/wiki/Golden_Idol_%28Event%29 "Golden Idol (Event) | Slay the Spire Wiki | Fandom"
