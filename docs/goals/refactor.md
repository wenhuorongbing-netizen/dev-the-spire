# 严格审核结论

**当前不能接受他一句“Phase 0/1 全部完成并通过验证”作为最终结论。** 更严谨的判定是：

> **Phase 0/1 的结构性代码和文档工作基本完成；但他的执行日志仍有严重口径冲突，所以“验证完成”只能算条件通过，必须补 raw final transcript 才能完全关闭。StS1Events 仍是 prototype，不是完成 feature。**

仓库当前 `docs/goals/refactor.md` 已经把 Phase 0 / Phase 1 标成 Done，并记录 build 0 errors、tests 303 pass / 0 fail / 21 skip、format clean、diff clean。 `docs/issues.md` 也新增了 `REFACTOR-PHASE0-1-VALIDATION`，写明 Phase 0/1 structural work complete，Phase 2 patch adapter rule not started。

但是你贴出的 worker 日志里，前后仍然出现了 `失败 ...` 测试列表、`all 8 remaining failures are pre-existing`、后面又写 `All tests pass`，最后又写 `Tests: 294 pass, 9 fail`。这不是合格的最终验证口径。

---

## 1. Step-by-step 审核

| Step       | 工作项                                                         | 当前证据                                                                                                                                                               | 严格判定                                   |
| ---------- | ----------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------ | -------------------------------------- |
| 0.1        | patch count drift 修复                                        | `docs/patch-inventory.md` 当前记录 141 total / 22 high-risk / 0 unclassified。 `docs/architecture/patch-boundaries.md` 同步引用 patch inventory，写 141 total / 22 high-risk。 | **完成**                                 |
| 0.2        | PR template 新增 high-risk seam / source-only live-proof gate | `docs/goals/refactor.md` Phase 0 记录 Done。                                                                                                                          | **完成**                                 |
| 0.3        | guard test 覆盖 PR template 新项                                | `docs/goals/refactor.md` 记录 guard tests added。                                                                                                                     | **完成，但仍建议保留 raw test transcript**      |
| 0.4        | no-game validation                                          | 仓库文档写 303 / 0 / 21；worker 日志又写 294 / 9。                                                                                                                            | **条件通过；必须补唯一最终验证口径**                   |
| 1.1        | FeatureOrders                                               | `docs/goals/refactor.md` 记录 `FeatureOrders.cs` 完成。                                                                                                                 | **完成**                                 |
| 1.2        | 5 个 named feature modules                                   | `docs/goals/refactor.md` 记录 Lotha/Morvi/Urda/Vakuu/Ascension module 完成。                                                                                            | **完成**                                 |
| 1.3        | Registry 去掉 inline lambda                                   | `docs/goals/refactor.md` 记录 registry refactor 完成。                                                                                                                  | **完成；但只是 named module，不是自动 discovery** |
| 1.4        | VakuuFightInitializer 拆文件                                   | `docs/goals/refactor.md` 记录 move done。                                                                                                                             | **完成**                                 |
| 1.5        | AscensionInitializer fallback comment                       | `docs/goals/refactor.md` 记录完成。                                                                                                                                     | **短期可接受**                              |
| 2          | Phase 2 Patch Adapter Rule                                  | `docs/goals/refactor.md` 明确 Not Started。                                                                                                                           | **未开始**                                |
| 3          | Extract Seams                                               | PreviewTransformPolicy、Banner/Firemark、RootSight、VakuuFightFlow、Ascension selection 均 Not Started。                                                                 | **未开始**                                |
| 4          | State/Save Cleanup                                          | UrdaProgress sub-states、codec tests 等 Not Started。                                                                                                                 | **未开始**                                |
| 5          | Behavior Test Upgrade                                       | 行为测试计划列出但未实现。                                                                                                                                                      | **未开始**                                |
| StS1Events | prototype completion                                        | `docs/goals/refactor.md` 明确约束：StS1Events is prototype, not complete feature。                                                                                       | **未完成**                                |

---

## 2. 当前完成状态修正版

| Area                       | Status                             | 严格判定                                                 |
| -------------------------- | ---------------------------------- | ---------------------------------------------------- |
| Phase 0 structural work    | Done                               | **Pass**                                             |
| Phase 1 structural work    | Done                               | **Pass**                                             |
| Phase 0/1 validation       | repo docs 记 303/0，但执行日志仍有 294/9 冲突 | **Conditional Pass / final raw transcript required** |
| Phase 2 patch adapter rule | Not Started                        | **未开始**                                              |
| Phase 3 seam extraction    | Not Started                        | **未开始**                                              |
| Phase 4 state/save cleanup | Not Started                        | **未开始**                                              |
| Phase 5 behavior tests     | Not Started                        | **未开始**                                              |
| StS1Events                 | prototype, not complete            | **未完成**                                              |
| Release-ready              | no live proof                      | **No**                                               |
| Test-ready                 | 仅 Phase 0/1 structural ready       | **Partial**                                          |

---

## 3. 最严重问题

### Issue 1: 最终验证口径仍不可信

**Observation:** 仓库文档写 303 pass / 0 fail / 21 skip。但 worker 的日志里仍同时出现 fail 列表、pre-existing failures、All tests pass、294 pass / 9 fail。

**Inference:** 他在报告里混用了 touched-scope tests、full tests、pre-existing failures 和最终验证结果。

**Recommendation:** 最终交付必须只保留一个 validation block：

```text
Final validation transcript:
- git status --short:
- dotnet build EZMicroBalance.sln:
- dotnet test EZMicroBalance.sln --no-build:
- dotnet format EZMicroBalance.sln --verify-no-changes --no-restore:
- git diff --check:
- skipped tests:
- failed tests:
```

**Acceptance:** `failed tests > 0` 时，必须写 `Overall tests: fail`，不能写 `All tests pass`。

---

### Issue 2: Phase 2 才是真正降低架构风险的开始

**Observation:** Phase 2 的目标是 high-risk patches 变成 thin adapters：gate、source object lookup、call service、return。

**Inference:** Phase 0/1 只是治理和 bootstrap cleanup。真正影响 save/load、co-op、black screen、reward sync 的风险还在 high-risk patch groups。

**Recommendation:** 下一轮必须以 Phase 2 为 engineering track，不允许只做 StS1Events 内容扩张。

---

### Issue 3: StS1Events 不许跳过 gate 进入批量实现

**Observation:** `docs/goals/refactor.md` 明确写 StS1Events 是 prototype，不是 complete feature。

**Inference:** 任何 “all events registered / all specs done / full parity” 都是不合格 claim。

**Recommendation:** StS1Events 的下一步只能是 `Off` / `CanaryOnly` feature gate + source/API verification + canary manual evidence。

---

# Overnight Monthly Dev Spec

下面这份是给 worker 的 **overnight run spec**。它不是“做一点就停”的 checklist，而是**必须跑到 stop condition 成立才允许停止**。

## Overnight Run Rule

**他今晚不能在以下条件前停止：**

1. **Green Stop：** 所有 Overnight Acceptance Pack 项目完成，并且 build/test/format/diff-check 有唯一最终口径。
2. **Hard Block Stop：** 出现无法继续的阻断项，且必须留下 blocker report、失败命令、日志路径、下一步 owner、未完成项列表。
3. **禁止 Soft Stop：** 不允许因为“时间到了”“大概完成”“先总结一下”“还有 pre-existing failure 但我改动通过”而停止。
4. **禁止 False Green：** 只要 full tests 失败，就不能写 All tests pass。
5. **禁止自审：** 实现 subagent 不能批准自己的工作；QA / Red-Team subagent 必须独立复核。

---

## Overnight Run Objective

> **完成 Refactor Phase 2 Foundation + StS1Events Feature Gate Safety，不做 full parity。**

Tonight’s accepted end state:

* Phase 0/1 validation 口径清理完成。
* Phase 2 patch adapter rule 有 owner/seam/checklist 初版。
* StS1Events 默认 `Off`，注册 0 个 events。
* `CanaryOnly` mode 定义完成，目标只允许 4 个 canary events。
* Subagent outputs 全部落文档。
* 没有 “All tests pass” 与 “failures remain” 同时出现。
* 不 claim release-ready、不 claim full parity、不关闭 live proof gates。

---

# Overnight Run Pack

## Pack 0 — Preflight Truth Gate

**Owner:** QA / Red-Team Auditor
**不得跳过。**

Run and record:

```powershell
git status --short
git log -1 --oneline --decorate
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

Acceptance:

* 输出写入 `docs/reviews/overnight-run-20260528.md`。
* 如果 tests 是 303 / 0 / 21，就删除旧的 294/9、295/8 口径。
* 如果 tests 失败，创建 issue row，不允许继续写 green summary。
* `git diff --check` 必须 clean。
* `docs/goals/refactor.md` 与 `docs/issues.md` 的 validation 状态一致。

Stop rule:

* 这一步失败时不能进入 implementation，只能进入 blocker report 修复。

---

## Pack 1 — Phase 2 Patch Adapter Rule Foundation

**Owner:** Patch Adapter Auditor + Release Documentation Agent

Required work:

1. 为每个 high-risk patch group 建 owner/seam/checklist 表：

   * Vakuu child combat → `VakuuFightService` / future `VakuuFightFlow`
   * Urda Root Eyes → `UrdaBlessingService.RootSight*` / future `RootSightPreviewPolicy`
   * A20 dual boss → A20 services/events
   * Ascension selector → selector gate / lobby adapter
   * Ascension map → `AscensionMapService`
   * Ascension combat → Banner/Firemark/boss services
   * Preview tools → preview-only policy
2. 更新 `docs/architecture/patch-boundaries.md`，但不要再复制多个 patch count source。
3. 增加 “No move + behavior change in same PR” enforcement note。
4. 若新增或移动 patch，必须 regenerate `docs/patch-inventory.md`。

Acceptance:

* `docs/architecture/patch-boundaries.md` 每个 high-risk group 都有 owner + required seam。
* `docs/goals/refactor.md` Phase 2 状态可以从 `Not Started` 改为 `Started / adapter checklist drafted`，不能写 Done。
* 没有 runtime/live proof 被关闭。

---

## Pack 2 — StS1Events Feature Gate Safety

**Owner:** Feature Gate / Registration Engineer + StS2 Source/API Auditor

Required work:

1. 建立 `Sts1EventFeatureGate`，支持：

   * `Off`
   * `CanaryOnly`
   * `AdditiveAllDraft`
   * `ReplaceUnknownEventsPrototype`
2. 默认 Off 必须注册 0 个 StS1 events。
3. CanaryOnly 目标只能是：

   * Big Fish
   * Golden Idol
   * Lab
   * Divine Fountain
4. `RegisterAll` 不允许无条件进入 default mod init path。
5. 如果 `Sts1EventRegistrationService` 仍被 compile remove，就必须明确写：

   * source present
   * registration inactive
   * feature gate not build-active
   * next activation step

Acceptance:

* 自动测试证明 Off = 0 registrations。
* 自动测试证明 CanaryOnly = exactly 4 planned canaries，或者如果尚未 build-active，则写 blocked reason。
* `MainFile.Initialize()` 不能无条件调用 all-event registration。
* 文档不能写 all events done / full parity。

---

## Pack 3 — StS1Events Source/API Evidence

**Owner:** StS2 Source/API Auditor + Wiki Spec Auditor

Required docs:

```text
docs/features/sts1-events/source-research/sts2-act-event-registration.md
docs/features/sts1-events/source-research/api-command-matrix.md
docs/features/sts1-events/wiki-event-catalog.md
```

Required decisions:

* 解决 46 / 48 / 52 mismatch：

  * `wiki_event_entries`
  * `runtime_event_models`
  * `act_bucket_memberships`
* 验证 act mapping，不允许再写未经证实的 `Underdocks=Act1, Overgrowth=Act2, Hive=Act3`。
* API matrix 覆盖：

  * HP heal / damage / max HP gain / max HP loss
  * add/remove cards and curses
  * relic grant / relic pool draw
  * potion grant
  * card select/remove/upgrade/transform UI
  * event option lock / damage / max HP tooltip
  * save/load event state

Acceptance:

* 每条 API 有 exact file/class/method evidence。
* 没有 source evidence 的 API 不允许进入 canary implementation。
* wiki/event/runtime/act bucket 三种数量不再混用。

---

## Pack 4 — Canary Spec Readiness, Not Implementation

**Owner:** Wiki Spec Auditor + Localization Agent + Asset Pipeline Agent

Tonight only requires canary specs ready, not all canary implementation.

Canary events:

* Big Fish
* Golden Idol
* Lab
* Divine Fountain

For each event, produce:

```text
- Wiki behavior summary, rewritten, not copied verbatim
- Normal values
- A15 values
- option table
- dependencies: relic/card/curse/potion/monster/save state
- localization key plan
- asset path plan
- manual evidence checklist
```

Acceptance:

* 每个 canary spec 从 `spec-drafted` 到 `source-verified`，或者明确 `blocked`。
* 不得标 `implemented`，除非 code + tests + manual evidence 都完成。
* 不得标 `loc-render-verified`，除非有 in-game screenshot。

---

## Pack 5 — Final Overnight Validation

**Owner:** QA / Red-Team Auditor，不能是 implementation agent。

Run:

```powershell
git status --short
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

Required final files:

```text
docs/reviews/overnight-run-20260528.md
docs/goals/refactor.md
docs/issues.md
docs/features/sts1-events/source-research/sts2-act-event-registration.md
docs/features/sts1-events/source-research/api-command-matrix.md
docs/features/sts1-events/wiki-event-catalog.md
```

Final acceptance:

* 一份唯一 validation summary。
* 所有 failures 要么 0，要么 issue-tracked。
* `git diff --check` clean。
* `docs/goals/refactor.md` 不 claim Phase 2 Done。
* StS1Events 不 claim complete。
* subagent outputs 都有 pass/fail。
* 若 commit/push，必须 validation pass；否则只写 blocker handoff，不 push。

---

# Monthly Dev Spec: 2026-05-28 → 2026-06-30

## Month Objective

> **Deliver Refactor Phase 2/3 foundations + StS1 Event Port Prototype Batch 1.**

Not accepted:

* Full parity claim.
* Release-ready claim.
* Live-ready claim without logs/screenshots.
* Co-op-ready claim without two-client evidence.
* Source-only pass closing live proof gates.

Accepted month-end state:

* Phase 2 patch adapter rule drafted and applied to at least one low-risk group.
* `PreviewTransformPolicy` extracted or at least characterized.
* StS1Events default Off = no behavior impact.
* CanaryOnly registers exactly 4 canary events.
* Four canary events playable and manually verified.
* Six simple events debug-spawn verified.
* Asset and localization paths validated for monthly events.
* Status board uses only allowed statuses.
* Evidence bundle complete.

---

## Week 0 — Overnight Run: Validation + Gate Safety

Focus:

* Fix final validation truth.
* Start Phase 2 adapter rule.
* Establish StS1Events feature gate.
* Produce source/API docs.

Acceptance:

* Overnight Pack 0–5 complete.
* No false green.
* No full parity wording.

---

## Week 1 — Source/API Verification + CanaryOnly Registration

Required:

* Finish `Sts1EventFeatureGate`.
* Off / CanaryOnly tests.
* Source/API matrix.
* Wiki catalog count policy.
* Registration docs.

Acceptance:

* Off mode test pass.
* CanaryOnly exactly 4 canary events.
* API matrix complete for canary implementation.
* No unconditional registration.

---

## Week 2 — Canary Implementation

Implement:

* Big Fish
* Golden Idol
* Lab
* Divine Fountain

Helpers:

* `Sts1HpService`
* `Sts1RewardService`
* `Sts1CurseService`
* `Sts1AscensionRules`
* `Sts1EventDebugSpawnCommand`

Acceptance:

* Every branch debug-spawned.
* Every branch has manual evidence.
* Save/load works after every branch.
* Images load.
* EN/ZHS render verified.
* No TODO in canary files.

---

## Week 3 — Simple Batch 1

Implement:

* Purifier
* Upgrade Shrine
* Golden Shrine
* The Cleric
* Old Beggar
* Shining Light

Acceptance:

* Six events debug-spawn and complete.
* Every branch has evidence.
* Asset paths validated.
* Loc render verified.
* No TODO in implemented files.

---

## Week 4 — Pool Prototype + Refactor Seam Extraction

Required:

* `Sts1EventPoolService` design doc.
* Debug-only `ReplaceUnknownEventsPrototype`, only if source evidence supports it.
* Save visited event ids and event bag state.
* Multiplayer fail-closed.
* Start `PreviewTransformPolicy` or another low-risk seam extraction.

Acceptance:

* Replacement mode disabled by default.
* Dev replacement mode only draws implemented StS1 events.
* Save/load does not duplicate/skip bag state.
* Multiplayer refuses replacement unless explicit debug override.
* At least one behavior test added for extracted seam.

---

## Week 5 — Package / Handoff / Evidence

Required:

* Build.
* Tests.
* Format.
* Diff check.
* Publish/package only if resources/localization/package changed.
* Version bump only if player-visible build delivered.
* Monthly review.
* Tester handoff.

Acceptance:

* Evidence bundle includes logs, screenshots, asset validation output, test output, status board.
* Release notes say `Prototype Batch 1`, not full parity.
* No release-ready wording.
* Commit/push only after validation passes.

---

# Mandatory Subagent Plan

他必须使用 subagent。不要让一个 agent 同时做 research、implementation、QA、release documentation。

| Subagent                             | Scope                                                                                                                            | Output                                                                              | Pass/Fail                                            |
| ------------------------------------ | -------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------- | ---------------------------------------------------- |
| Wiki Spec Auditor                    | 核对 52 Wiki event entries；拆分 event entries / runtime models / act memberships；产出 canary + simple batch exact options 和 A15 deltas | corrected catalog；10 个 monthly event source-verified specs；46/48/52 mismatch report | 每个 monthly event spec 有 exact options / dependencies |
| StS2 Source/API Auditor              | Verify act mapping、RitsuLib registration、HP/relic/curse/card/save APIs                                                           | `sts2-act-event-registration.md`、`api-command-matrix.md`                            | 必须有 exact file/class/method evidence                 |
| Feature Gate / Registration Engineer | 实现 Off / CanaryOnly / AdditiveAllDraft / ReplaceUnknownEventsPrototype                                                           | `Sts1EventFeatureGate`、gated registration、count tests                               | default Off registers nothing                        |
| Patch Adapter Auditor                | 审核 high-risk patches 是否 thin adapter                                                                                             | patch adapter checklist、owner/seam map                                              | 每个 high-risk patch group 有 owner 和 seam              |
| Canary Implementation Engineer       | 实现 Big Fish / Golden Idol / Lab / Divine Fountain                                                                                | playable canary code、helper services、branch tests                                   | QA manual evidence 后才能 pass                          |
| Simple Batch Engineer                | 实现 6 个 simple events                                                                                                             | six playable simple events                                                          | QA evidence 后才能 pass                                 |
| Asset Pipeline Agent                 | event id → local StS1 art paths；copyright-safe extraction；load validation                                                        | asset manifest、validation output                                                    | path validation + screenshots                        |
| Localization Agent                   | EN/ZHS text、dynamic variables、formatting                                                                                         | localization json + render screenshots                                              | no missing keys / no placeholders                    |
| QA / Red-Team Auditor                | 独立复核所有 claims；run build/test/publish；debug-spawn every branch；verify save/load and gate Off                                      | manual evidence、screenshot/log index、final pass/fail table                          | QA 不能是 implementation subagent                       |
| Release Documentation Agent          | 保持 README/PROJECT_MAP/status board/test plan/monthly review/release notes 诚实                                                     | docs exact status、no full-parity wording                                            | status board matches evidence bundle                 |

---

# 给他的 Overnight 指令

直接把这段发给他：

```text
你现在进入 overnight run。不要在 acceptance pack 完成前停止。

Stop rules:
1. Green stop only: Pack 0–5 全部完成，build/test/format/diff-check 有唯一最终口径，docs/issues/refactor/sts1 docs 全部同步。
2. Hard block stop only: 出现无法继续的 blocker，并留下 blocker report、失败命令、日志、owner、下一步。
3. 禁止 soft stop：不能因为时间、疲劳、部分完成、pre-existing failure 解释就停止。
4. 禁止 false green：只要 full tests 有失败，就不能写 All tests pass。
5. 实现 subagent 不允许自审；QA / Red-Team 必须独立复核。

今晚必须完成：
- raw final validation transcript；
- Phase 2 patch adapter owner/seam checklist；
- StS1EventFeatureGate 设计或实现；
- Off mode = 0 registration 的测试或 blocker；
- CanaryOnly = Big Fish / Golden Idol / Lab / Divine Fountain 的 registration plan/test；
- source/API docs 初版；
- wiki catalog 46/48/52 mismatch policy；
- subagent pass/fail 输出；
- final handoff，不得 claim full parity / release-ready / live-ready。

如果你完成得早，继续 Week 1 的 source/API verification 和 CanaryOnly registration tests。不能停在“看起来差不多”。
```

---

## 最终一句话结论

**这个项目当前最应该优先解决的是“最终验证口径可信度 + Phase 2 patch adapter rule + StS1Events feature gate”，因为它直接影响后续 StS1 prototype、save/load、co-op、tester handoff 和 release claim 是否可信。**
