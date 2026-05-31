结论：**当前仍未完成。**
这次最新进展比上一轮更好：unsafe mode gate、replacement fail-closed、guard tests、build/test/format/diff 都有真实推进；但它仍是 **source/doc hardening pass**，不是 **runtime parity / gameplay-ready / release-ready**。

我把这次 v15 严格审查、下月开发规范、subagent 分工、Mandatory Overnight Run 写成了完整文档：

[下载完整审查文档 v15](sandbox:/mnt/data/sts1_event_port_strict_audit_v15_monthly_overnight.md)

## 严格审核总判定

这次可以认可的进展：

| 模块                                          | 判定                                                                              |
| ------------------------------------------- | ------------------------------------------------------------------------------- |
| `AdditiveAllDraft` unsafe gate              | 有效进展；现在需要 `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`                           |
| `ReplaceUnknownEventsPrototype` fail-closed | 有效进展；正常 build 下不会误开                                                             |
| Guard tests                                 | `Sts1EventFeatureGuardTests` 31 passed                                          |
| Build                                       | `dotnet build EZMicroBalance.sln -m:1 --no-incremental` 0 errors，但有 89 warnings |
| Full tests                                  | 464 passed / 0 failed / 21 skipped / 485 total                                  |
| Format                                      | `dotnet format --verify-no-changes` passed                                      |
| Diff check                                  | passed，但仍有既有 CRLF normalization warning                                         |
| Docs                                        | 已开始避免 overclaim，加入 `hard-stop-blocker-report-v14.md`                            |
| Commit/push                                 | 没有执行，这是正确的，因为 runtime gate 还没绿                                                  |

但这些仍然没有完成：

| 模块                                    | 当前状态    |
| ------------------------------------- | ------- |
| Runtime canary proof                  | blocked |
| Runtime simple batch proof            | blocked |
| Save/load proof                       | blocked |
| Event image/render proof              | blocked |
| ReplacementPrototype functional proof | blocked |
| Multiplayer/fail-closed runtime proof | blocked |
| Independent QA/Red-Team               | blocked |
| Combat encounter models               | blocked |
| Full StS1 event experience            | 未完成     |
| Release-ready / gameplay-ready        | 未完成     |

最关键一句：**Hard Stop report 允许当前 session 暂停，但不代表功能完成。**

## 为什么仍不能算完成

StS1 事件系统不是“注册一些 `EventModel`”。Wiki 明确说明 events 来自 unknown location，事件选择由随机和当前 Act 共同决定；部分事件限定 Act，部分事件可跨 Act；Act 4 没有 unknown location/event；Ascension 15 会让部分不利事件更强或更可能发生。Wiki 事件表还分为 16 个 shared events、12 个 Act 1 exclusive events、16 个 Act 2 exclusive events、8 个 Act 3 exclusive events。([slay-the-spire.fandom.com][1])

所以现在的 `52 / 54 / 50 / 48 / 47` 必须继续用 canonical matrix 解释。`registry entries`、`registration calls`、`runtime models`、`spec files` 都不能直接等同于“全事件完成”。

Canary 标准也不能降低。Big Fish 必须是 Act 1 exclusive，Banana 回复 `floor(maxHP/3)`，Donut 增加 5 Max HP，Box 给随机 common/uncommon/rare relic 并加入 Regret。([slay-the-spire.fandom.com][2]) Golden Idol 必须是 Act 1 exclusive，Take 获得 Golden Idol 后触发陷阱；Outrun 给 Injury，Smash 造成 25%/35% max HP 伤害，Hide 损失 8%/10% max HP，Leave 无事发生。([slay-the-spire.fandom.com][3])

项目边界也不能变：当前 active deliverable 仍是单一 `Spire Plus` mod，technical manifest id 是 `EZMicroBalance`，代码和资源路径分别是 `EZMicroBalanceCode/` 与 `EZMicroBalance/`。 原版素材不能随意提交；原版 art 只有授权确认后才能进入 tracked/public files。

## 当前状态 vs 目标

目标是：

```text
StS2 mod 中尽量复刻 StS1 event experience：
- unknown room event pool
- Act bucket / semi-common / exclusive
- 选项流程和锁定条件
- rewards/cards/relics/curses/potions
- A15 数值变化
- 图片和 EN/ZHS 文本
- save/load
- multiplayer / IsShared
- 默认不污染 Spire Plus
```

当前实际是：

```text
source/doc safety: 有明显进展
automated guard: 有明显进展
default unsafe protection: 有明显进展
runtime gameplay: 未验证
save/load: 未验证
image/render: 未完成
replacement pool: 未功能验证
multiplayer/fail-closed: 未运行时验证
combat events: blocked
QA/Red-Team: blocked
```

因此管理决策是：**继续优化 + 有限推进，两者兼顾，但优化优先。**

不要继续扩大事件范围。只推进 verified scope：

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

## 下个月开发规范：June 2026

目标名称：

**`StS1 Event Port Prototype Batch 1 — Runtime Parity Foundation`**

月末 Go/No-Go 标准：

1. Full build exit code 0，并保存完整 log。
2. 89 warnings 要么减少，要么形成 warning budget 文档。
3. Full test exit code 0。
4. 21 skipped tests 逐条解释。
5. Runtime dependency installed：`STS2-RitsuLib` 必须在 checked game-root mod path 中。
6. 生成 active `godot.log`。
7. Default Off 注册 0 个 StS1 events。
8. CanaryOnly 精确注册 Big Fish、Golden Idol、Lab、Divine Fountain。
9. AdditiveBatch1 精确为 verified scope：10 event types / 11 calls。
10. AdditiveAllDraft 必须 unsafe-only。
11. ReplacementPrototype 必须 debug-only + unsafe-only。
12. `52 / 54 / 50 / 48 / 47` canonical matrix 经 Red-Team 复核。
13. 4 个 canary runtime proof：截图、结果日志、pre/post state、save/load、EN/ZHS、image render。
14. 6 个 simple batch runtime proof：截图、结果日志、EN/ZHS、image render。
15. ReplacementPrototype functional proof：unknown room 只抽 StS1 candidates，Act bucket 正确，event bag/save-load 正确。
16. Multiplayer/fail-closed 证明完成。
17. Combat events 在 encounter models 完成前保持 blocked。
18. Independent QA/Red-Team 给 pass/fail。
19. Docs 更新：status-board、current-validation、handoff、monthly review。
20. 不 commit/push，除非验证证据支持当前 scope 的真实声明。

## Mandatory Overnight Run v15

允许停止条件只有两个：

```text
A. O0-O56 全部 GREEN；
或
B. 写出 HARD STOP BLOCKER REPORT：
   - exact gate id
   - blocker reason
   - evidence path
   - attempted actions
   - owner action
   - why continuation is impossible in current environment
```

Hard Stop 只允许暂停，**不代表完成**。

不能因为这些就停止：

```text
build passes
tests pass
format passes
guard tests pass
unsafe gates exist
status-board updated
canonical matrix exists
source files exist
asset scripts exist
replacement source exists
hard-stop report exists
all code-side work complete
```

夜跑最关键的新门槛是：

```text
O21 Runtime environment path report
O22 STS2-RitsuLib installed in checked path
O23 active godot.log generated
O24 loader proof: BaseLib/RitsuLib/Spire Plus load state
O25-O32 canary runtime / save-load / render proof
O33-O42 simple batch runtime / render proof
O43-O46 replacement functional / act bucket / save-load proof
O47 multiplayer fail-closed proof
O51 independent QA/Red-Team report
O56 final summary states incomplete gates honestly
```

## 必须使用 subagent

这次需要新增一个专门解决阻塞的 subagent：**Runtime Environment Bootstrap**。完整 subagent 分工如下：

1. **BuildGate / Repo Health**：build/test/format/diff logs、warnings、skipped tests。
2. **Runtime Environment Bootstrap**：确认 checked game-root paths，安装/验证 `STS2-RitsuLib`，生成 active `godot.log`。
3. **Wiki Parity Spec Auditor**：复核 52 public target、54 internal entries、exact options、A15、semi-common。
4. **StS2 Source/API Auditor**：ActModel、EventModel、RitsuLib、card/relic/potion/save/replacement API。
5. **Feature Gate / Registration Engineer**：Off、CanaryOnly、AdditiveBatch1、AdditiveAllDraft、ReplacementPrototype。
6. **Canary Gameplay Subagent**：Big Fish、Golden Idol、Lab、Divine Fountain runtime proof。
7. **Simple Batch Gameplay Subagent**：6 个 simple batch runtime proof。
8. **Asset + Localization Subagent**：ZHS render、missing-key scan、image/license decision。
9. **Event Pool / RNG / Save Subagent**：replacement pool、event bag、visited ids、save/load。
10. **Multiplayer / IsShared Subagent**：per-event `IsShared`、combat true、fail-closed co-op。
11. **Content Parity Subagent**：Bite、face relics、StS1 curses、Golden/Bloody Idol、combat encounter models。
12. **QA / Red-Team Subagent**：只验收，不实现，逐 gate pass/fail。
13. **Release Documentation Subagent**：status-board、current-validation、handoff、monthly review、owner actions。

## 可以直接发给他的指令

```text
当前状态不能标完成。

你这轮 source/doc hardening 有价值：unsafe modes 现在需要显式 override，replacement prototype 在 normal build 下 fail-closed，guard tests 通过，build 0 errors，full tests 通过，format 和 diff check 通过。

但是 O0-O56 没有全绿。当前 STS2-RitsuLib 已安装，且 v15 已生成 active `godot.log` 并到达 main menu；但 loader audit 仍是红灯，包含 11 个 `Godot ERROR` 命中、`ritsulib-variants.json` manifest parsing，以及 8 个 optional Spire Plus ModPatcher failures。因此 runtime/art/replacement/multiplayer/QA gates 仍 blocked，不能写 runtime parity、release-ready、gameplay-ready 或“和杀戮尖塔1完全一样”。

继续 Mandatory Overnight Run v15。只能在 O0-O56 全绿后停止；如果当前环境确实无法继续，必须写 HARD STOP BLOCKER REPORT。Hard stop 不代表完成。

优先级：
1. 保存 full build/test/format/diff evidence，包括 89 warnings budget 和 21 skipped tests explanation。
2. 先处理 runtime blocker：STS2-RitsuLib install 和 active godot.log 已有证据；现在必须 resolve 或 explicitly disposition loader errors，然后重新跑 clean/accepted loader proof。
3. 证明 default Off=0、CanaryOnly exact 4、AdditiveBatch1 exact 10 event types / 11 calls、AdditiveAllDraft unsafe-only、ReplacementPrototype debug/unsafe-only。
4. Red-Team 52 / 54 / 50 / 48 / 47 count reconciliation。
5. 运行游戏验证 4 canary：screenshots、result logs、save/load、EN/ZHS、image render。
6. 运行游戏验证 6 simple batch：screenshots、result logs、EN/ZHS、image render。
7. 完成 ReplacementPrototype functional proof：unknown rooms 只抽 StS1 candidates，Act bucket 正确，event bag/visited ids 持久化。
8. Combat events 在 encounter models 完成前继续 blocked。
9. QA/Red-Team 必须独立验收，不能实现者自验。
10. 不要 commit/push，除非 scope 真实且 validation evidence 支持声明。
```

管理红线：**不要把 source/doc completion 当 gameplay completion；不要把 automated tests 当 runtime proof；不要把 registry counts 当 StS1 parity；不要把 hard-stop blocker report 当完成；不要继续扩大范围。先把 runtime environment、4 canary、6 simple batch、replacement pool、save/load、image/localization、QA 跑绿。**

[1]: https://slay-the-spire.fandom.com/wiki/Events "Events | Slay the Spire Wiki | Fandom"
[2]: https://slay-the-spire.fandom.com/wiki/Big_Fish "Big Fish | Slay the Spire Wiki | Fandom"
[3]: https://slay-the-spire.fandom.com/wiki/Golden_Idol_%28Event%29 "Golden Idol (Event) | Slay the Spire Wiki | Fandom"
# StS1 Event Port Strict Audit v15 — Monthly Dev Spec + Mandatory Overnight Run

Date: 2026-05-31
Scope: review the latest assistant/session report, decide completion status, compare current state against the goal of StS1-like event experience in StS2, define next-month development spec, enforce subagent workflow, and set an overnight run that may only stop at green gates or a documented hard blocker.

---

## 0. Executive verdict

**Not complete.**

The latest report is a meaningful source/doc hardening pass:

- unsafe StS1 modes are now guarded;
- `AdditiveAllDraft` requires `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`;
- `ReplaceUnknownEventsPrototype` now fails closed in normal builds unless both the compile-time replacement gate and unsafe override are present;
- guard tests were added for unsafe-mode and replacement fail-closed behavior;
- active StS1 docs were updated to avoid some overclaims;
- validation improved: filtered StS1 guard tests passed, full solution build passed with 0 errors, full test suite passed, format passed, diff check passed except an existing CRLF warning.

However, the latest report itself says **O0-O50 are not green** and that runtime/art/replacement/multiplayer/QA gates remain blocked. The v15 continuation has since produced an active loader log, but the audit remains red due 11 `Godot ERROR` hits, including `ritsulib-variants.json` manifest parsing and 8 optional Spire Plus ModPatcher failures. That means the StS1 event port is **not runtime parity complete**, **not release-ready**, and **not gameplay-ready**.

Correct label:

> `StS1 Event Port Prototype Batch 1 — Source/Doc Hardening Pass Completed; Runtime Parity Gates Blocked`

Incorrect labels:

- all tasks complete
- full parity complete
- all StS1 events complete
- release-ready
- gameplay-ready
- “和杀戮尖塔 1 完全一样”

---

## 1. Current report under audit

Latest user-provided report says:

```text
Changed:
- AdditiveAllDraft now requires SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1.
- ReplaceUnknownEventsPrototype now fails closed in normal builds unless REPLACEMENT_PROTOTYPE_ENABLED and unsafe override are both present.
- Added guard coverage for unsafe-mode override and replacement fail-closed behavior.
- Updated active StS1 docs/counts to avoid overclaims and align on 52 / 54 / 50 / 48 / 47, AdditiveBatch1 = 10 event types / 11 calls, and runtime-blocked status.
- Added hard-stop-blocker-report-v14.md.
- Updated current-validation.md with exact validation results and known testhost retry behavior.

Validation:
- dotnet test --filter Sts1EventFeatureGuardTests passed: 31 passed.
- dotnet test --filter PlayerFacingNameStaysSpirePlusWhileTechnicalIdRemainsStable passed.
- dotnet build EZMicroBalance.sln -m:1 --no-incremental passed after clearing stale testhost locks: 0 errors / 89 warnings.
- dotnet test EZMicroBalance.sln --no-build -- RunConfiguration.MaxCpuCount=1 passed: 464 passed / 0 failed / 21 skipped / 485 total.
- dotnet format EZMicroBalance.sln --verify-no-changes --no-restore passed.
- git diff --check passed, with only the existing CRLF normalization warning for docs/patch-inventory.md.

Hard Stop:
- O0-O50 are not green.
- Runtime/art/replacement/multiplayer/QA gates remain blocked; v15 later captured a loader log, but it is not clean and does not close runtime proof.
- No runtime parity, release-ready, or gameplay-ready claim was made.
- No commit or push was performed.
```

v15 continuation update:

- `STS2-RitsuLib` `v0.3.10` is installed in the checked E-drive game-root path.
- Active loader evidence now exists at `.tools/runtime-evidence/sts1-events-v15-loader-20260531-231135/godot.log.after-launch`.
- The log reaches main menu with BaseLib, RitsuLib, and Spire Plus loaded, and logs StS1Events default Off.
- `audit-godot-log.after-launch.json` is not clean: 11 `Godot ERROR` hits remain, including `ritsulib-variants.json` manifest parsing and 8 optional Spire Plus ModPatcher failures.
- The valid v15 hard stop is now `docs/features/sts1-events/hard-stop-blocker-report-v15.md`; this does not mark runtime parity, gameplay readiness, or release readiness complete.

---

## 2. Strict completion audit

### 2.1 What is legitimately complete

| Area | Status | Audit result |
| --- | --- | --- |
| Unsafe mode guard | Green for source-side guard | This is a strong safety improvement. `AdditiveAllDraft` and replacement mode should not be reachable by normal users. |
| Replacement fail-closed source behavior | Green for source-side guard | Source-side proof is useful, but it is not yet functional runtime replacement proof. |
| StS1 guard tests | Green for filtered test lane | 31 guard tests passed. Keep evidence path in docs. |
| Player-facing name / technical id guard | Green | Important because `Spire Plus` must remain player-facing while `EZMicroBalance` remains the technical id. |
| Full build | Green for errors, amber for warnings | 0 errors, but 89 warnings are not “clean.” Warnings need a documented budget and triage. |
| Full tests | Green for automated tests | 464 passed / 0 failed / 21 skipped / 485 total is internally consistent. Skipped tests still need explanation. |
| Format | Green | `dotnet format --verify-no-changes` passed. |
| Diff check | Amber/Green | Passed except existing CRLF normalization warning. Existing warning must remain tracked; it should not become invisible debt. |
| Docs avoiding overclaims | Improved | Current report says docs were updated to avoid overclaims. Must be checked by Red-Team, not only by implementer. |
| Commit/push | Not done | Acceptable because gates are not fully green. Do not push runtime-incomplete work as release-ready. |

### 2.2 What is not complete

| Area | Status | Why it blocks completion |
| --- | --- | --- |
| Runtime canary verification | Blocked | No active game launch, no `godot.log`, no screenshots, no event result logs. |
| Runtime simple batch verification | Blocked | Source implementation is not gameplay proof. |
| Save/load proof | Blocked | No in-game event state persistence evidence. |
| Image/render proof | Blocked | No redistributable StS1 art and no local extraction/render proof. |
| ReplacementPrototype functional proof | Blocked | Source guard is not proof that unknown rooms only draw StS1 candidates. |
| Multiplayer/fail-closed proof | Blocked | Needs runtime/co-op or explicit fail-closed evidence. |
| Independent QA/Red-Team | Blocked | Implementer cannot self-certify parity gates. |
| Combat events | Blocked | Encounter models remain missing; these events must not be counted as parity-complete. |
| Release-ready status | Not complete | Runtime/art/QA gates are still blocked. |
| Full StS1 experience | Not complete | Event feel depends on event pool, act buckets, visuals, text, save/load, and runtime behavior. |

---

## 3. Critical historical context that still matters

Earlier work made several overclaims and unsafe assumptions:

1. A prior pass wired `Sts1EventRegistrationService.RegisterAll(ModId)` directly into `MainFile.Initialize()`, which meant StS1 draft events could pollute the normal Spire Plus startup path. It also wrote the wrong act mapping comment: `Underdocks=Act1, Overgrowth=Act2, Hive=Act3`. This was a serious cause of “event feel differs from StS1.”
2. A prior status-board wrote `Infrastructure`, `event-specs`, `assets.md`, `localization.md`, and `test-plan.md` as `Done` while still listing blockers such as Regret, Injury, random relic helper, card UI, and combat encounter models. That “Done” vocabulary must not return.
3. A prior overnight summary already established the correct guard philosophy: default Off, correct act mapping, RitsuLib registration is additive only, and nothing may be marked Done without source/API/test/screenshot/log/save-load evidence.

These historical failures justify the strict v15 gate policy.

---

## 4. Goal comparison

### 4.1 Actual project goal

The target is not “many files exist.” The target is:

```text
Port StS1 event experience into the StS2 Spire Plus mod:
- unknown room event pool behavior
- Act bucket and semi-common membership
- page flow and option locks
- rewards/cards/relics/curses/potions
- A15 numeric changes
- images and EN/ZHS text rendering
- save/load persistence
- multiplayer / IsShared behavior
- default Off so normal Spire Plus remains clean
```

### 4.2 Current state against that goal

| Goal component | Current state | Decision |
| --- | --- | --- |
| Default Off | Improved and guarded | Continue protecting it. |
| Unsafe modes | Improved and guarded | Good optimization; keep it. |
| Act mapping | Previously corrected; must remain guarded | Continue tests. |
| Canary implementation | Code-side claimed; runtime not verified | Needs runtime launch. |
| Simple batch implementation | Code-side claimed; runtime not verified | Needs runtime launch. |
| ZHS placeholders | Claimed cleared in source | Needs render proof. |
| Images | Blocked | Must choose extraction/license/generated placeholder path. |
| Replacement event pool | Source guarded only | Needs functional proof. |
| Save/load | Not verified | Needs runtime proof. |
| Combat events | Blocked | Keep blocked until encounter models exist. |
| QA | Blocked | Must use independent Red-Team. |
| Release | Not ready | No release-ready claim. |

---

## 5. Decision: optimize + limited progress, with optimization first

Decision: **both optimize and progress, but optimization comes first.**

Do not expand to more events. The verified scope remains:

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

### Optimization priorities

1. Keep the checked runtime environment stable: `STS2-RitsuLib` is installed in the checked E-drive game-root mod path.
2. Resolve or explicitly disposition the active loader-log errors, then rerun clean/accepted loader proof.
3. Preserve full build/test/format/diff evidence.
4. Red-Team the 52 / 54 / 50 / 48 / 47 count reconciliation.
5. Enforce mode naming:
   - `Off`
   - `CanaryOnly`
   - `AdditiveBatch1` = verified scope only, 10 event types / 11 calls
   - `AdditiveAllDraft` = unsafe/dev-only
   - `ReplaceUnknownEventsPrototype` = debug-only and unsafe-gated
6. Remove every false “Done” from status docs.

### Limited progress priorities

1. Runtime verify 4 canary.
2. Runtime verify 6 simple batch.
3. Functional proof for replacement pool.
4. Save/load proof.
5. Image/render proof for verified scope.
6. Independent QA/Red-Team pass/fail.

---

## 6. June 2026 Monthly Dev Spec

Monthly target name:

```text
StS1 Event Port Prototype Batch 1 — Runtime Parity Foundation
```

Forbidden claims:

- full parity
- all StS1 events complete
- release-ready
- gameplay-ready
- “和杀戮尖塔 1 完全一样”

### 6.1 Month-end Go/No-Go criteria

| # | Criterion | Required proof |
| --- | --- | --- |
| 1 | Full build passes | Full unfiltered `dotnet build EZMicroBalance.sln -m:1 --no-incremental` log, exit code 0. |
| 2 | Warning budget documented | 89 warnings either reduced or triaged in docs. |
| 3 | Full tests pass | Full `dotnet test EZMicroBalance.sln --no-build -- RunConfiguration.MaxCpuCount=1` log, 0 failed. |
| 4 | Skipped tests explained | 21 skipped tests listed with reason. |
| 5 | Format/diff checks pass | `dotnet format` and `git diff --check` evidence. |
| 6 | Runtime dependency installed | `STS2-RitsuLib` present in checked game-root mod path, with log proof. |
| 7 | Active `godot.log` exists | Launch log showing BaseLib, RitsuLib, and Spire Plus load state. |
| 8 | Default Off | 0 StS1 registrations and clean normal Spire Plus startup. |
| 9 | CanaryOnly | Exact identity proof: Big Fish, Golden Idol, Lab, Divine Fountain only. |
| 10 | AdditiveBatch1 | Exact verified-scope proof: 10 event types / 11 calls only. |
| 11 | AdditiveAllDraft unsafe | Requires `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`; never default. |
| 12 | ReplacementPrototype unsafe/debug-only | Requires `REPLACEMENT_PROTOTYPE_ENABLED` and unsafe override. |
| 13 | Canonical matrix | 52 / 54 / 50 / 48 / 47 reconciliation Red-Team reviewed. |
| 14 | 4 canary runtime proof | Screenshot, result log, pre/post state, EN/ZHS render, image render. |
| 15 | 4 canary save/load proof | Save during/after event, reload, state stable. |
| 16 | 6 simple batch runtime proof | Screenshot, result log, EN/ZHS render, image render. |
| 17 | Replacement functional proof | Unknown rooms draw only StS1 candidates in debug replacement mode. |
| 18 | Act bucket proof | Overgrowth + Underdocks = Act 1, Hive = Act 2, Glory = Act 3. |
| 19 | Event bag proof | No-repeat / visited ids / save-load behavior documented. |
| 20 | Multiplayer/fail-closed | Co-op verified or StS1 unsafe modes fail closed in multiplayer. |
| 21 | Images | Local extraction hash proof, owner-licensed art, generated replacement art, or explicit non-parity placeholder. |
| 22 | Combat blockers | Combat events stay blocked until encounter models exist. |
| 23 | Independent QA | QA/Red-Team report pass/fail, not written by implementer. |
| 24 | Docs | status-board, current-validation, handoff, monthly review updated. |
| 25 | Commit/push policy | No commit/push unless validation and scope are honest. |

---

## 7. Mandatory Overnight Run v15

The assistant must continue the overnight run until **all gates are green** or a valid hard blocker report is produced.

### 7.1 Allowed stop conditions

The run may stop only if:

```text
A. O0-O56 are all GREEN;
or
B. HARD STOP BLOCKER REPORT is written with:
   - exact gate id
   - blocker reason
   - evidence path
   - attempted actions
   - owner action
   - why continuation is impossible in the current environment
```

A hard stop only permits pausing. It does **not** mark the blocked feature complete.

### 7.2 Things that are not stop conditions

Do not stop just because:

```text
build passes
tests pass
format passes
guard tests pass
unsafe gates exist
status-board updated
canonical matrix exists
source files exist
asset scripts exist
replacement source exists
hard-stop report exists
all code-side work complete
```

### 7.3 Gates O0-O56

| Gate | Required result |
| --- | --- |
| O0 | Worktree snapshot: branch, HEAD, diff, unstaged files. |
| O1 | Full unfiltered build log, exit code 0. |
| O2 | Warning budget report for 89 warnings. |
| O3 | Full test log, exit code 0. |
| O4 | Skipped test explanation. |
| O5 | `dotnet format` proof. |
| O6 | `git diff --check` proof and CRLF note. |
| O7 | status-board contains no false `Done`. |
| O8 | current-validation includes exact validation commands and outcomes. |
| O9 | canonical matrix complete. |
| O10 | 52 / 54 / 50 / 48 / 47 count reconciliation Red-Team reviewed. |
| O11 | act mapping guard passes. |
| O12 | feature gate tests pass. |
| O13 | Off=0 exact registration proof. |
| O14 | CanaryOnly exact 4 identity proof. |
| O15 | AdditiveBatch1 exact verified-scope proof: 10 event types / 11 calls. |
| O16 | AdditiveAllDraft unsafe override proof. |
| O17 | ReplacementPrototype unsafe + compile-define proof. |
| O18 | ReplacementPrototype fail-closed proof in normal builds. |
| O19 | per-event `IsShared` matrix complete. |
| O20 | combat `IsShared=true` guard passes. |
| O21 | Runtime environment path report: checked game-root paths. |
| O22 | `STS2-RitsuLib` installed in checked path. |
| O23 | Active `godot.log` generated. |
| O24 | Loader proof: BaseLib/RitsuLib/Spire Plus load state visible in log. |
| O25 | Canary runtime launch with `SPIREPLUS_STS1_EVENT_MODE=CanaryOnly`. |
| O26 | Big Fish screenshot + result log. |
| O27 | Golden Idol screenshot + result log. |
| O28 | Lab screenshot + result log. |
| O29 | Divine Fountain screenshot + result log. |
| O30 | Canary save/load proof. |
| O31 | Canary EN/ZHS render proof. |
| O32 | Canary image/license/render proof. |
| O33 | Simple batch launch with `AdditiveBatch1`. |
| O34 | Purifier runtime proof. |
| O35 | Upgrade Shrine runtime proof. |
| O36 | Golden Shrine runtime proof. |
| O37 | The Cleric runtime proof. |
| O38 | Old Beggar / Pleading Vagrant runtime proof. |
| O39 | Shining Light runtime proof. |
| O40 | Simple batch save/load proof if applicable. |
| O41 | Simple batch EN/ZHS render proof. |
| O42 | Simple batch image/license/render proof. |
| O43 | ReplacementPrototype functional proof: unknown rooms draw only StS1 candidates. |
| O44 | Replacement act bucket proof. |
| O45 | Event bag / visited ids / no-repeat proof. |
| O46 | Replacement save/load proof. |
| O47 | Multiplayer fail-closed proof or co-op verification. |
| O48 | Content parity gap matrix updated. |
| O49 | Temporary substitutes marked non-parity. |
| O50 | Combat blocker report current and honest. |
| O51 | Independent QA/Red-Team report. |
| O52 | QA verifies source/doc claims against runtime evidence. |
| O53 | Monthly review updated. |
| O54 | Handoff docs updated. |
| O55 | Owner actions listed. |
| O56 | Final summary says incomplete gates honestly; no release-ready claim. |

---

## 8. Mandatory subagent plan

The assistant must split work into subagents. The implementer must not self-audit.

| Subagent | Responsibility | Required output |
| --- | --- | --- |
| BuildGate / Repo Health | Build/test/format/diff logs, warnings, skipped tests, worktree. | `build-full.log`, `test-full.log`, `warning-budget.md`, `skipped-tests.md`. |
| Runtime Environment Bootstrap | Fix checked game-root paths, install/verify `STS2-RitsuLib`, produce active `godot.log`. | `runtime-loader-proof.md`, log paths. |
| Wiki Parity Spec Auditor | Verify 52 public target, 54 internal entries, exact options, A15, semi-common membership. | canonical matrix and count reconciliation. |
| StS2 Source/API Auditor | Verify ActModel, EventModel, RitsuLib, card/relic/potion/save/replacement APIs. | source/API matrix. |
| Feature Gate / Registration Engineer | Off, CanaryOnly, AdditiveBatch1, AdditiveAllDraft, ReplacementPrototype. | mode tests and registration-count tests. |
| Canary Gameplay Subagent | Runtime proof for Big Fish, Golden Idol, Lab, Divine Fountain. | screenshots, result logs, save/load. |
| Simple Batch Gameplay Subagent | Runtime proof for Purifier, Upgrade Shrine, Golden Shrine, Cleric, Old Beggar/Pleading Vagrant, Shining Light. | screenshots and result logs. |
| Asset + Localization Subagent | ZHS render, missing-key scan, image extraction/license decision. | render screenshots, asset manifest, license notes. |
| Event Pool / RNG / Save Subagent | Replacement pool, event bag, visited ids, save/load. | replacement functional proof. |
| Multiplayer / IsShared Subagent | per-event `IsShared`, combat true, fail-closed co-op. | `is-shared-matrix.csv`, co-op/fail-closed log. |
| Content Parity Subagent | Bite, face relics, StS1 curses, Golden/Bloody Idol, combat encounter models. | content gap matrix. |
| QA / Red-Team Subagent | Independent pass/fail only; no implementation. | `qa-redteam-report.md`. |
| Release Documentation Subagent | status-board, current-validation, handoff, monthly review, owner actions. | updated docs. |

---

## 9. Direct instruction to the assistant

```text
Current status is not complete.

Your latest source/doc hardening pass is useful: unsafe modes now require explicit override, replacement prototype fails closed in normal builds, guard tests pass, full build has 0 errors, full tests pass, format passes, and diff check passes except an existing CRLF note.

However, O0-O56 are not green. The v15 continuation generated an active `godot.log` and reached main menu with BaseLib, RitsuLib, and Spire Plus loaded, but runtime/art/replacement/multiplayer/QA gates remain blocked because the loader audit is not clean: 11 `Godot ERROR` hits, including `ritsulib-variants.json` manifest parsing and 8 optional Spire Plus ModPatcher failures. This means no runtime parity, no release-ready claim, and no gameplay-ready claim.

Continue Mandatory Overnight Run v15. You may stop only when O0-O56 are all green, or when you produce a valid HARD STOP BLOCKER REPORT for the exact gate that cannot continue in the current environment. A hard stop does not mark the feature complete.

Immediate priority order:
1. Preserve full build/test/format/diff evidence, including the 89-warning budget and 21 skipped test explanations.
2. Resolve or explicitly disposition the loader errors, then rerun clean/accepted loader proof with STS2-RitsuLib, BaseLib, and Spire Plus enabled.
3. Prove default Off=0, CanaryOnly exact 4, AdditiveBatch1 exact 10 event types / 11 calls, AdditiveAllDraft unsafe-only, ReplacementPrototype debug/unsafe-only.
4. Red-Team the 52 / 54 / 50 / 48 / 47 count reconciliation.
5. Runtime verify 4 canary events with screenshots, result logs, save/load, EN/ZHS render, and image/render proof.
6. Runtime verify 6 simple batch events with screenshots, result logs, EN/ZHS render, and image/render proof.
7. Provide ReplacementPrototype functional proof: unknown rooms draw only StS1 candidates, act buckets are correct, event bag and visited ids persist.
8. Keep combat events blocked until encounter models exist.
9. Use independent QA/Red-Team; implementer may not self-certify.
10. Do not commit or push unless the scope is honest and validation evidence supports the claim.
```

---

## 10. Management red lines

- Do not treat source/doc completion as gameplay completion.
- Do not treat automated tests as runtime proof.
- Do not treat registry counts as StS1 event parity.
- Do not treat a hard-stop blocker report as feature completion.
- Do not continue expanding event scope.
- Do not claim release-ready or full parity.
- First make 4 canary + 6 simple batch runtime-real, visible, localized, save/load-safe, and replacement-pool verified.
