# StS1 Event Port 严格审核 v9 — Current State, Monthly Dev Spec, Subagents, Mandatory Overnight Run

日期：2026-05-29
目标项目：Devspire / Spire Plus (`EZMicroBalance`)
目标功能：在 StS2 mod 中迁移 StS1 事件体验

---

## 0. 总判定

**没有完成。**

本次报告中的“Overnight Run Complete — 20 of 25 gates GREEN, 5 gates HARD STOP BLOCKED”只能说明：

1. 本轮夜跑可以因硬阻塞而暂时停下；
2. 一部分代码、guard tests、handoff 文档已经推进；
3. 但 `O12 / O15 / O16 / O19 / O23` 仍未完成，且这些 gate 正好覆盖 runtime gameplay、save/load、replacement pool、QA/red-team、图片/外部资源等 StS1 体验核心；
4. 因此不能写“StS1 event port complete”，只能写“code-side Batch 1 foundation advanced; runtime parity blocked”。

正确结论：

```text
Stash/recovery/infrastructure: 部分完成
Canary code review: claimed complete, pending runtime proof
Simple batch code: claimed code-complete, pending runtime proof
Runtime gameplay: blocked / not verified
Images: blocked / not complete
ZHS localization: not complete unless placeholders cleared and render proof exists
Replacement pool: not complete unless functional runtime proof exists
Full StS1 event experience: not complete
```

---

## 1. 当前报告中可以接受的进展

基于本次 summary，可以暂时承认以下进展，但每一项仍需对应 evidence 文件或 git diff 复核：

| 项目 | 本次声称 | 审核判定 |
| --- | --- | --- |
| Overnight gates | 20/25 GREEN | 有进展，但不是完成；5 个 hard-stop gate 仍阻塞 |
| Canary implementation review | 4 个 canary reachable code 0 TODO | 代码层可暂认 AMBER/GREEN；需要 runtime proof |
| Simple batch specs | 6 个 simple events exact spec | 可暂认 spec-drafted/source-claimed；需 source/API/red-team 复核 |
| New model files | `Sts1Purifier.cs`, `Sts1GoldenShrine.cs` | 可暂认新增；需 build/test/runtime proof |
| Simple batch code | 6/6 code-complete | 只能算 code-claimed；runtime 未验收 |
| Guard tests | 21 → 24 | 自动化有进展；不等于 gameplay proof |
| Tests | 444 passed, 0 failed, 21 skipped | 接受为 automated test pass claimed；skipped 和 runtime gap 仍需说明 |
| Handoff docs | `o24-handoff.md`, blocker report | 正确方向；不能把 blocker gate 算完成 |

---

## 2. 当前不能接受的完成声明

以下声明不应出现在 status-board、handoff、release note、monthly review 中：

```text
All tasks are complete
All StS1 events are complete
Full parity complete
和杀戮尖塔1完全一样
release-ready
runtime verified
images complete
ZHS complete
replacement pool complete
```

允许的诚实表述：

```text
Overnight code-side pass reached 20/25 gates green.
5 gates are hard-stop blocked and documented.
Canary and simple batch code are claimed complete but runtime evidence is still pending.
The project is not StS1 parity-ready.
```

---

## 3. Step-by-step 审核

### Step 1 — Build / automated tests

**状态：AMBER。**

报告声称 tests 从 361 → 444 passed，0 failed，21 skipped。接受为自动化测试进展，但仍需：

- 最新完整 `dotnet build --no-restore` unfiltered log；
- 最新完整 test log；
- skipped tests 数量与原因；
- 证明这不是仅基于 tail/grep 的局部输出。

验收标准：

```text
build exit code = 0
full test exit code = 0
log path recorded
no grep/tail-only proof
skipped tests explained
```

### Step 2 — Gate accounting

**状态：NOT COMPLETE。**

20/25 GREEN 不等于全部完成。5 个 hard-stop gate 必须留在 monthly dev spec 中继续执行。

严禁把 hard-stop blocker report 当作 feature completion。它只是允许 overnight run 在当前环境停下。

### Step 3 — Canonical matrix / count reconciliation

**状态：AMBER/RED。**

本次指标变更：

```text
Wiki entries: 52 -> 54
Runtime models: 46 -> 48
Registry entries: 48 -> 50
Shared events: 15 -> 17
Registration calls: 52 -> 54
```

这些数字不是坏事，但必须解释。StS1 Wiki visible event buckets 是 16 shared + 12 Act 1 + 16 Act 2 + 8 Act 3，总和 52。任何 `54 wiki entries` 或 `50 registry entries` 都必须在 canonical matrix 中解释为：

- duplicate act membership；
- runtime split model；
- shared/semi-shared special case；
- intentionally skipped event；
- unsupported/blocked placeholder；
- registry-only helper；
- error。

必须生成：

```csv
event_key,wiki_name,wiki_bucket,st1_acts_allowed,sts2_acts_registered,runtime_model,registry_entry,registration_call_count,is_shared,mode,has_spec,has_code,has_en,has_zhs,has_asset,has_runtime_proof,status,notes
```

### Step 4 — Feature gate / default Off

**状态：必须继续守住。**

默认模式必须是 Off，未设置 `SPIREPLUS_STS1_EVENT_MODE` 时注册 0 个 StS1 events。

允许模式：

```text
Off                         default, 0 registrations
CanaryOnly                  Big Fish / Golden Idol / Lab / Divine Fountain only
AdditiveBatch1              canary + verified simple batch only
AdditiveAllDraft            non-default dev only
ReplaceUnknownEventsPrototype debug-only, never default
```

验收：

```text
Off=0 registration test
CanaryOnly=4 registration test
AdditiveBatch1 exact-count test
AdditiveAllDraft never default
ReplacementPrototype requires explicit debug flag or compile symbol
```

### Step 5 — Canary code review

**状态：CODE-CLAIMED, RUNTIME PENDING。**

报告声称 4 个 canary reachable code 0 TODO、APIs fully implemented。可以进入 runtime verification，但不能视为完成。

Canary 必须逐项验收：

| Event | Required proof |
| --- | --- |
| Big Fish | Banana heal, Donut max HP, Box relic + Regret, A0/A15 not applicable where correct, screenshot, log, save/load |
| Golden Idol | Take/Leave, relic gain, Injury, damage branch 25/35%, max HP loss 8/10%, screenshot, log, save/load |
| Lab | exactly 3 potions or documented StS2-compatible equivalent, screenshot/log |
| Divine Fountain | only appears with curse, removes all curses, screenshot/log |

### Step 6 — Simple batch code

**状态：CODE-CLAIMED, RUNTIME PENDING。**

本次新增/完成：

```text
Purifier
Upgrade Shrine
Golden Shrine
The Cleric
Old Beggar / Pleading Vagrant mapping
Shining Light
```

必须验收：

- exact StS1 behavior；
- A15 variants；
- option lock conditions；
- dynamic text；
- EN/ZHS render；
- image path；
- debug spawn；
- result log；
- save/load where event page state exists。

### Step 7 — Runtime gameplay verification

**状态：HARD STOP BLOCKED / NOT COMPLETE。**

如果当前环境不能 launch game，则允许 hard-stop report；但 monthly dev spec 必须把它继续列为 P0。

Owner required actions：

```text
Launch game with SPIREPLUS_STS1_EVENT_MODE=CanaryOnly
Debug spawn 4 canary events
Screenshot before/after/options/rewards
Save/load during event where possible
Switch to AdditiveBatch1 or AdditiveAllDraft for simple batch proof
```

### Step 8 — Replacement pool

**状态：NOT COMPLETE。**

“ReplacementPrototypeSourceExistsWithCorrectStructure” 不等于 replacement pool functional proof。

必须证明：

```text
unknown room in replacement mode draws only StS1 candidate events
StS2 vanilla events are excluded
act bucket is correct
visited ids / no-repeat behavior works
save/load preserves bag state
multiplayer fail-closed unless explicitly enabled
```

### Step 9 — Images

**状态：NOT COMPLETE / EXTERNAL RESOURCE BLOCKED。**

如果没有 StS1 art redistribution rights，不能把原图提交到 repo。允许：

1. 本地 extraction script + local hash manifest；
2. generated/recreated replacement art；
3. owner-provided licensed assets。

月末验收不是“script exists”，而是：

```text
asset manifest complete
local extraction/run proof
image path exists for verified events
in-game screenshot renders event image
license/redistribution decision documented
```

### Step 10 — ZHS localization

**状态：不能默认 hard-stop。**

如果只是 38 placeholders，则这不应被视为外部不可解决 blocker。可以由 localization subagent 补齐草案，再由 owner 校对。只有“必须逐字使用 StS1 官方中文”才涉及授权/外部文本问题。

验收：

```text
placeholder count = 0
all verified events have zhs strings
in-game zhs render screenshot
style guide / glossary updated
```

### Step 11 — Combat events

**状态：BLOCKED / NOT COMPLETE。**

`IsShared = true` 是必要但不充分。7 个 combat encounter models blocked 仍需设计：

```text
Dead Adventurer
Scorpion Nest
Treasure Ooze
Masked Bandits
Mysterious Sphere
Mind Bloom.War
可能还包括其他 combat branch / event split
```

在 encounter models 未完成前，combat events 不得标 implemented 或 manual-verified。

### Step 12 — QA / Red-Team

**状态：NOT COMPLETE。**

QA/Red-Team 必须独立，不允许实现者自验。它要逐 gate 给出 pass/fail，并检查：

- false Done；
- count drift；
- default Off；
- asset/license risk；
- ZHS placeholder；
- runtime proof；
- StS1 behavior mismatch；
- multiplayer IsShared correctness。

---

## 4. 当前状况 vs 目标

### 目标

让 StS2 mod 中的 StS1 events 接近或复刻 StS1 体验，包括：

- 正确事件池；
- 正确 Act bucket；
- 正确出现条件；
- 正确 option flow；
- 正确 reward/card/relic/curse/potion behavior；
- A15 variants；
- 图片；
- EN/ZHS 文本；
- save/load；
- multiplayer behavior；
- replacement pool；
- runtime evidence。

### 当前状况

| 维度 | 当前状态 |
| --- | --- |
| Code foundation | 明显推进 |
| Guard tests | 推进 |
| Canary code | claimed complete |
| Simple batch code | claimed complete |
| Runtime proof | 未完成 |
| Images | 未完成 |
| ZHS | 未完成/待确认 |
| Replacement pool | 未完成 |
| Combat events | blocked |
| Full StS1 feel | 未完成 |

### 综合分析

你感觉“事件和杀戮尖塔 1 游戏体验出入很大”仍然成立。原因不是单纯事件数量，而是：

1. runtime 事件流未验证；
2. replacement pool 未证明；
3. 图片未完成；
4. ZHS 未完成；
5. combat events blocked；
6. temporary substitutes 仍可能改变 StS1 判断；
7. hard-stop gates 尚未绿；
8. 52/54/48/50 count drift 未被 canonical matrix 解释。

---

## 5. 决策：优化 + 推进并行，但优化优先

不建议纯优化，也不建议盲目推进更多事件。

决策：**两者兼顾，但以 gate-based optimization 为先。**

执行规则：

```text
先把当前 4 canary + 6 simple batch 证据跑绿。
只在它们 runtime verified 后推进更多事件。
不要继续堆空壳事件或扩大 AdditiveAllDraft。
所有推进必须通过 feature gate、runtime proof、QA proof。
```

优先级：

1. P0：runtime proof、replacement pool、canonical matrix、default Off；
2. P1：ZHS、images、simple batch runtime；
3. P2：combat events design；
4. P3：扩展到 card-service/custom-UI batches。

---

## 6. Next Month Dev Spec — June 2026

名称：`StS1 Event Port Prototype Batch 1 — Runtime Parity Foundation`

时间：2026-06-01 至 2026-06-30

### 月目标

把当前 code-side foundation 转成 runtime-verified prototype。

### 月末 Go/No-Go 标准

必须全部满足才可进入下一阶段：

1. 最新 full build exit code 0；
2. full tests 0 failed，skipped tests 有解释；
3. default Off 注册 0 个 StS1 events；
4. CanaryOnly 精确注册 4 个 events；
5. AdditiveBatch1 精确注册 10 个 verified events；
6. 52/54/48/50 count drift 完整解释；
7. 4 canary runtime verified；
8. 6 simple batch runtime verified；
9. ZHS placeholders = 0 for verified scope；
10. verified scope images render proof；
11. replacement pool functional proof；
12. save/load proof；
13. multiplayer fail-closed or verified behavior；
14. combat blockers documented；
15. QA/Red-Team independent pass。

### Week 1 — Truth and Runtime Setup

Deliverables：

```text
canonical-event-matrix.csv
registry-count-reconciliation.md
feature-gate-proof.md
build-test-evidence.md
status-board.md rewritten
```

Acceptance：

```text
No false Done
Off=0
CanaryOnly=4
AdditiveBatch1 exact count
52/54/48/50 explained
```

### Week 2 — Canary Runtime Proof

Deliverables：

```text
canary-runtime-evidence.md
screenshots/canary/*.png
save-load-proof-canary.md
```

Acceptance：

```text
Big Fish verified
Golden Idol verified
Lab verified
Divine Fountain verified
all EN/ZHS verified scope render
all verified images render
```

### Week 3 — Simple Batch Runtime Proof

Deliverables：

```text
simple-batch-runtime-evidence.md
screenshots/simple-batch/*.png
zhs-placeholder-audit.md
asset-proof-batch1.md
```

Acceptance：

```text
Purifier verified
Upgrade Shrine verified
Golden Shrine verified
The Cleric verified
Old Beggar/Pleading Vagrant verified
Shining Light verified
```

### Week 4 — Replacement Pool and QA

Deliverables：

```text
replacement-pool-functional-proof.md
save-load-event-bag-proof.md
qa-red-team-report.md
monthly-review-2026-06.md
```

Acceptance：

```text
ReplacementPrototype draws only StS1 events in debug mode
no vanilla StS2 events in replacement pool
act bucket correct
visited/no-repeat behavior documented
QA signs pass/fail independently
```

---

## 7. Mandatory Overnight Run v9 — Run Until Complete

### Stop rule

The assistant may stop only if:

```text
A. O0-O32 all GREEN; or
B. HARD STOP BLOCKER REPORT is written, with exact gate, reason, evidence, and owner action.
```

Important distinction：

```text
Hard-stop report allows this run to pause.
It does NOT mark the feature complete.
```

### O0-O32 Gates

| Gate | Requirement | Stop status |
| --- | --- | --- |
| O0 | Worktree snapshot | Must be GREEN |
| O1 | Latest full build exit code 0 | Must be GREEN |
| O2 | Full tests 0 failed, skipped explained | Must be GREEN |
| O3 | Status-board no false Done | Must be GREEN |
| O4 | Canonical event matrix complete | Must be GREEN |
| O5 | 52/54/48/50 reconciliation complete | Must be GREEN |
| O6 | Act mapping guarded | Must be GREEN |
| O7 | Feature gate tests pass | Must be GREEN |
| O8 | Off=0 registrations | Must be GREEN |
| O9 | CanaryOnly=4 registrations | Must be GREEN |
| O10 | AdditiveBatch1 exact registrations | Must be GREEN |
| O11 | Per-event IsShared matrix complete | Must be GREEN |
| O12 | Canary code review clean | Must be GREEN |
| O13 | Canary runtime debug spawn proof | Must be GREEN or hard-stop runtime blocker |
| O14 | Canary save/load proof | Must be GREEN or hard-stop runtime blocker |
| O15 | Canary image render proof | Must be GREEN or hard-stop asset/runtime blocker |
| O16 | Canary EN/ZHS render proof | Must be GREEN or hard-stop runtime blocker |
| O17 | Simple batch exact spec red-team pass | Must be GREEN |
| O18 | Simple batch code review clean | Must be GREEN |
| O19 | Simple batch runtime proof | Must be GREEN or hard-stop runtime blocker |
| O20 | ZHS placeholders = 0 for verified scope | Must be GREEN |
| O21 | Asset manifest for verified scope | Must be GREEN |
| O22 | ReplacementPrototype source guard | Must be GREEN |
| O23 | ReplacementPrototype functional proof | Must be GREEN or hard-stop runtime blocker |
| O24 | Event bag save/load proof | Must be GREEN or hard-stop runtime blocker |
| O25 | Multiplayer fail-closed guard | Must be GREEN |
| O26 | Combat blocker report | Must be GREEN |
| O27 | Content parity gap matrix | Must be GREEN |
| O28 | Temporary substitutes marked non-parity | Must be GREEN |
| O29 | QA/Red-Team independent report | Must be GREEN or hard-stop QA blocker |
| O30 | Monthly review updated | Must be GREEN |
| O31 | Handoff docs updated | Must be GREEN |
| O32 | Next-session owner actions listed | Must be GREEN |

### Things that are NOT stop conditions

```text
20/25 gates green
444 tests pass
all code compiles
handoff docs written
hard-stop blockers listed
replacement source exists
asset scripts exist
localization json exists
```

---

## 8. Required Subagents

### 1. BuildGate / Repo Health Subagent

Scope：build/test/log/skipped tests/worktree status。

Output：

```text
build log path
test log path
exit codes
skipped test explanation
modified files list
```

### 2. Wiki Parity Spec Auditor

Scope：StS1 exact behavior, A15, event conditions, canonical matrix。

Output：

```text
canonical-event-matrix.csv
count reconciliation
spec pass/fail rows
```

### 3. StS2 Source/API Auditor

Scope：EventModel, ActModel, RitsuLib, HP, card, relic, potion, save/load, combat APIs。

Output：

```text
api-command-matrix.md
unsupported API blocker list
replacement-safe API recommendations
```

### 4. Feature Gate / Registration Engineer

Scope：Off/Canary/Additive/Replacement modes。

Output：

```text
registration count tests
mode behavior proof
no default pollution proof
```

### 5. Canary Gameplay Subagent

Scope：Big Fish, Golden Idol, Lab, Divine Fountain。

Output：

```text
runtime screenshots
result logs
save/load proof
option-by-option proof
```

### 6. Simple Batch Subagent

Scope：Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar/Pleading Vagrant, Shining Light。

Output：

```text
runtime screenshots
result logs
option proof
A15 proof
```

### 7. Asset + Localization Subagent

Scope：ZHS placeholders, EN/ZHS render, image manifest, local extraction/generated art。

Output：

```text
placeholder count
render screenshots
asset manifest
hash proof
license decision
```

### 8. Event Pool / RNG / Save Subagent

Scope：ReplacementPrototype, event bag, visited ids, RNG, save/load。

Output：

```text
functional replacement proof
run log
event bag save/load proof
vanilla event exclusion proof
```

### 9. Multiplayer / IsShared Subagent

Scope：per-event IsShared, shared voting, independent selection, combat constraints。

Output：

```text
is-shared-matrix.csv
fail-closed multiplayer proof
combat event guard proof
```

### 10. Content Parity Subagent

Scope：missing cards/relics/curses/encounters, substitutes, non-parity blockers。

Output：

```text
content-parity-gap-matrix.md
custom model requirements
temporary substitute list
```

### 11. QA / Red-Team Subagent

Scope：independent gate review only, no implementation。

Output：

```text
qa-red-team-report.md
pass/fail per gate
false claim audit
release-readiness verdict
```

### 12. Release Documentation Subagent

Scope：status-board, handoff, monthly review, release notes, docs index。

Output：

```text
status-board.md
monthly-review-2026-06.md
o24-handoff.md updated
next-session-owner-actions.md
```

---

## 9. Direct Instruction to the Assistant

```text
Current status is not complete. Your latest overnight report reached 20/25 gates green, with 5 hard-stop blocked gates. That permits a pause only for blockers; it does not mean the StS1 event port is finished.

Do not claim all tasks complete, full parity, or release readiness.

Continue the Mandatory Overnight Run v9 until O0-O32 are all green. If a gate cannot be completed because it requires game launch, licensed art, owner QA, or another unavailable external resource, write a HARD STOP BLOCKER REPORT with exact gate, reason, evidence, and owner action. Do not mark blocked gates complete.

Use subagents:
1. BuildGate / Repo Health
2. Wiki Parity Spec Auditor
3. StS2 Source/API Auditor
4. Feature Gate / Registration Engineer
5. Canary Gameplay
6. Simple Batch
7. Asset + Localization
8. Event Pool / RNG / Save
9. Multiplayer / IsShared
10. Content Parity
11. QA / Red-Team
12. Release Documentation

Highest priorities:
1. Keep default Off and prove Off=0.
2. Prove CanaryOnly=4 and AdditiveBatch1 exact count.
3. Build canonical matrix and reconcile 52/54/48/50 counts.
4. Runtime-verify 4 canary events.
5. Runtime-verify 6 simple batch events.
6. Clear ZHS placeholders for verified scope.
7. Provide image/render proof for verified scope.
8. Provide functional replacement-pool proof, not only source guard.
9. Keep combat events blocked until encounter models exist.
10. QA/Red-Team must independently verify; implementer cannot self-approve.
```

---

## 10. Final Management Decision

**继续优化 + 有限推进。**

Do not expand to more event families until:

```text
4 canary + 6 simple batch are runtime verified
replacement pool has functional proof
ZHS/image proof exists for verified scope
QA red-team signs off
```

After those pass, proceed to the next batch in this order:

1. Card service events;
2. Custom UI events;
3. Combat events after encounter models;
4. Full replacement pool parity;
5. Full asset polish and final localization;
6. Multiplayer proof;
7. Release-candidate audit.

结论：**没有完成。**
这次可以承认他比上一轮进展更大：`20/25 gates GREEN`、4 个 canary 做了 code review、6 个 simple batch 进入 code-complete claimed、guard tests 增加、tests 366 pass。但是 **5 个 gate 仍然 HARD STOP BLOCKED**，而且这些 blocked gate 正好覆盖 runtime gameplay、save/load、replacement pool、QA/Red-Team、图片/外部资源等核心体验验证。所以这轮夜跑最多算 **code-side foundation advanced + blocker documented**，不能算 StS1 事件迁移完成。

我整理了新版严格审核、下月 dev spec、subagent 分工和 Mandatory Overnight Run v9：

[下载：StS1 Event Port 严格审核 v9 + Monthly Dev Spec + Overnight Run](sandbox:/mnt/data/sts1_event_port_strict_audit_v9_monthly_spec_overnight.md)

## 严格审核结论

### 1. “Overnight Run Complete” 不等于任务完成

`20 of 25 gates GREEN` 说明夜跑推进了，但 **5 个 hard-stop gate 没绿**。hard-stop report 可以让本轮在当前环境暂停，但不能把被阻塞的功能计为完成。尤其是他自己列出的 owner actions 仍包括：

```text
Launch game with CanaryOnly 截图 4 个 canary events
Save/load during canary events
Switch to AdditiveAllDraft 验证 simple batch events
Enable ReplacementPrototype 验证 replacement pool
Run independent QA/Red-Team review
Obtain/create art for event images
```

这些不是边角工作，而是“像不像杀戮尖塔 1”的核心证据。

### 2. 当前可以承认的完成范围

可以暂时接受：

| 模块                                     | 审核状态                     |
| -------------------------------------- | ------------------------ |
| stash/recovery 后续修复                    | 基本完成                     |
| combat `IsShared = true` guard         | 有进展                      |
| feature gate / registration foundation | 有进展                      |
| canary reachable code review           | code-claimed complete    |
| simple batch 6 events                  | code-claimed complete    |
| tests                                  | 444 pass / 0 fail / 21 skip，自动测试进展 |
| handoff/blocker docs                   | 有进展                      |

但这些仍然不是 runtime parity。

### 3. 当前不能接受的完成声明

不能写：

```text
All tasks are complete
All StS1 events are complete
Full parity complete
和杀戮尖塔1完全一样
release-ready
runtime verified
images complete
ZHS complete
replacement pool complete
```

正确写法应该是：

```text
Overnight code-side pass reached 20/25 gates green.
5 gates are hard-stop blocked and documented.
Canary and simple batch code are claimed complete but runtime evidence is still pending.
The project is not StS1 parity-ready.
```

### 4. 52 / 54 / 48 / 50 的数字漂移必须解释

这次 metrics 变成：

```text
Wiki entries: 52 -> 54
Runtime models: 46 -> 48
Registry entries: 48 -> 50
Shared events: 15 -> 17
Registration calls: 52 -> 54
```

这不是一定错，但必须通过 canonical matrix 解释。StS1 Wiki 的事件页列出 16 个 shared events、12 个 Act 1 exclusive events、16 个 Act 2 exclusive events、8 个 Act 3 exclusive events；事件来自 unknown location，并按随机和当前 Act 选择，Act 4 没有 events，Ascension 15 会增强部分不利事件。([slay-the-spire.fandom.com][1])

所以如果现在内部统计是 54，就必须说明哪些是 duplicate act membership、runtime split model、semi-shared registration、registry helper、blocked placeholder，还是错误。

### 5. 你感觉“游戏体验出入很大”仍然是对的

原因不是“少写几个事件”这么简单，而是这些体验核心仍未闭环：

| 体验核心                   | 当前状态                                  |
| ---------------------- | ------------------------------------- |
| runtime gameplay proof | blocked / pending                     |
| save/load proof        | blocked / pending                     |
| event images           | pending                               |
| ZHS render proof       | pending                               |
| replacement pool       | source guard 或待验证，不是 functional proof |
| combat events          | encounter models blocked              |
| 52/54 registry 口径      | 未完全解释                                 |
| QA/Red-Team            | pending                               |
| temporary substitutes  | 仍需标为 non-parity                       |

RitsuLib additive registration 仍然不等于 StS1 unknown room 体验。只有 debug/runtime 证明 unknown room 不再抽 StS2 原事件、Act bucket 正确、event bag/save-load 正确，才算迈向 StS1 事件体验。

## 当前状况 vs 最终目标

最终目标是“让 StS2 mod 中的 StS1 事件体验尽量复刻 StS1”：事件池、Act bucket、出现条件、选项 flow、reward/card/relic/curse/potion、A15、图片、EN/ZHS、save/load、multiplayer 都要接近原作。

当前状态更像：

```text
code foundation: 有进展
guard tests: 有进展
canary/simple batch code: claimed complete
runtime parity: 未完成
visual/text parity: 未完成
replacement-pool parity: 未完成
full StS1 feel: 未完成
```

因此管理决策是：**继续优化 + 有限推进并行，但优化优先。**

不要继续扩大到更多事件族。先把当前 4 canary + 6 simple batch 做到 runtime verified，然后再推进 Card Service / Custom UI / Combat。

## Next Month Dev Spec：June 2026

月目标名称：

**`StS1 Event Port Prototype Batch 1 — Runtime Parity Foundation`**

月末 Go/No-Go 标准：

1. 最新 full build exit code 0。
2. Full tests 0 failed，skipped tests 有解释。
3. Default Off 注册 0 个 StS1 events。
4. CanaryOnly 精确注册 4 个 events。
5. AdditiveBatch1 精确注册 10 个 verified events。
6. 52/54/48/50 count drift 完整解释。
7. 4 个 canary runtime verified。
8. 6 个 simple batch runtime verified。
9. Verified scope 的 ZHS placeholders = 0。
10. Verified scope 的 event images render proof 完成。
11. Replacement pool 有 functional proof，不只是 source exists。
12. Save/load proof 完成。
13. Multiplayer fail-closed 或 verified behavior 完成。
14. Combat blockers 有 blocker report，不能假装完成。
15. QA/Red-Team 独立 pass。

## Mandatory Overnight Run v9：跑完才能停

他不能因为这些完成就停：

```text
20/25 gates green
444 tests pass
all code compiles
handoff docs written
hard-stop blockers listed
replacement source exists
asset scripts exist
localization json exists
```

唯一允许停止：

```text
A. O0-O32 全部 GREEN；
或
B. HARD STOP BLOCKER REPORT 写清楚 exact gate、reason、evidence、owner action。
```

但要强调：**hard-stop report 允许夜跑暂停，不代表 feature 完成。**

核心 gates：

| Gate | 要求                                      |
| ---- | --------------------------------------- |
| O0   | worktree snapshot                       |
| O1   | latest full build exit code 0           |
| O2   | full tests 0 failed，skipped explained   |
| O3   | status-board no false Done              |
| O4   | canonical event matrix complete         |
| O5   | 52/54/48/50 reconciliation complete     |
| O6   | Act mapping guarded                     |
| O7   | feature gate tests pass                 |
| O8   | Off=0 registrations                     |
| O9   | CanaryOnly=4 registrations              |
| O10  | AdditiveBatch1 exact registrations      |
| O11  | per-event IsShared matrix complete      |
| O12  | canary code review clean                |
| O13  | canary runtime debug spawn proof        |
| O14  | canary save/load proof                  |
| O15  | canary image render proof               |
| O16  | canary EN/ZHS render proof              |
| O17  | simple batch exact spec red-team pass   |
| O18  | simple batch code review clean          |
| O19  | simple batch runtime proof              |
| O20  | ZHS placeholders = 0 for verified scope |
| O21  | asset manifest for verified scope       |
| O22  | ReplacementPrototype source guard       |
| O23  | ReplacementPrototype functional proof   |
| O24  | event bag save/load proof               |
| O25  | multiplayer fail-closed guard           |
| O26  | combat blocker report                   |
| O27  | content parity gap matrix               |
| O28  | temporary substitutes marked non-parity |
| O29  | QA/Red-Team independent report          |
| O30  | monthly review updated                  |
| O31  | handoff docs updated                    |
| O32  | next-session owner actions listed       |

## 强制 subagent 分工

让他立刻按这些 subagent 拆：

1. **BuildGate / Repo Health**：build/test logs、exit codes、skipped tests、worktree。
2. **Wiki Parity Spec Auditor**：canonical matrix、52/54/48/50 reconciliation、exact StS1/A15。
3. **StS2 Source/API Auditor**：EventModel、ActModel、RitsuLib、HP/card/relic/potion/save/combat API。
4. **Feature Gate / Registration Engineer**：Off/Canary/Additive/Replacement 模式证明。
5. **Canary Gameplay Subagent**：Big Fish、Golden Idol、Lab、Divine Fountain runtime proof。
6. **Simple Batch Subagent**：Purifier、Upgrade Shrine、Golden Shrine、The Cleric、Old Beggar/Pleading Vagrant、Shining Light。
7. **Asset + Localization Subagent**：ZHS placeholders、图片 manifest、渲染截图、license decision。
8. **Event Pool / RNG / Save Subagent**：replacement pool、event bag、visited ids、save/load。
9. **Multiplayer / IsShared Subagent**：逐事件 IsShared matrix、combat 事件 guard、fail-closed。
10. **Content Parity Subagent**：missing curse/relic/card/encounter、temporary substitute、non-parity blockers。
11. **QA / Red-Team Subagent**：只验收，不实现，逐 gate pass/fail。
12. **Release Documentation Subagent**：status-board、monthly review、handoff、release evidence。

## 直接发给他的指令

```text
当前状态没有完成。你最新 overnight report 是 20/25 gates green，5 gates hard-stop blocked。这允许因 blocker 暂停，但不代表 StS1 event port 完成。

不要写 all tasks complete、full parity、release-ready、和杀戮尖塔1完全一样。

继续 Mandatory Overnight Run v9，直到 O0-O32 全绿。若某 gate 因 game launch、licensed art、owner QA 或不可用外部资源无法完成，必须写 HARD STOP BLOCKER REPORT，标明 exact gate、reason、evidence、owner action。Blocked gate 不得标完成。

最高优先级：
1. 保持 default Off，并证明 Off=0。
2. 证明 CanaryOnly=4，AdditiveBatch1 exact count。
3. 建 canonical matrix，解释 52/54/48/50 数字漂移。
4. Runtime 验证 4 个 canary。
5. Runtime 验证 6 个 simple batch。
6. 清零 verified scope 的 ZHS placeholders。
7. 完成 verified scope 的 image/render proof。
8. 提供 ReplacementPrototype functional proof，而不是 source guard。
9. Combat events 在 encounter models 存在前保持 blocked。
10. QA/Red-Team 必须独立验收，不能实现者自验。
```

最终管理红线：**不要把 hard-stop blocker report 当完成；不要把 tests pass 当 runtime proof；不要把 registry count 当 StS1 parity；不要继续扩大事件范围。先把 4 canary + 6 simple batch 的 runtime、图片、本地化、save/load、replacement pool 跑绿。**

[1]: https://slay-the-spire.fandom.com/wiki/Events "Events | Slay the Spire Wiki | Fandom"
