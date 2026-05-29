# 严格审核结论

**不能判定为完全完成。**
当前最准确的结论是：

> **Phase 0/1 的结构性工作可以判定完成；但“最终验证完成”不能完全关闭，因为他的 handoff 仍然自相矛盾：同一批日志里既出现测试失败，又出现 “All tests pass”。因此必须进入 overnight run，把验证口径、Phase 2 patch adapter rule、StS1Events feature gate 一次跑到可停止条件。**

仓库当前 `docs/goals/refactor.md` 已把 Phase 0/1 标为 Done，并记录 build 0 errors、tests 303 pass / 0 fail / 21 skip、format clean、diff clean。 `docs/issues.md` 也新增 `REFACTOR-PHASE0-1-VALIDATION`，说明 Phase 0/1 structural work complete，Phase 2 patch adapter rule not started。

但你贴出的 worker 日志里仍然有失败测试列表、`All tests pass`、以及 `Tests: 294 pass, 9 fail` 同时存在。   这说明他的**报告口径仍不合格**。结构完成不等于验证可信。

---

## 1. Step-by-step 审核

| Step       | 工作项                                                   | 严格判定            | 证据                                                                                         |
| ---------- | ----------------------------------------------------- | --------------- | ------------------------------------------------------------------------------------------ |
| 0.1        | patch count drift 修复                                  | **完成**          | `patch-inventory.md` 当前是 141 total / 22 high-risk / 0 unclassified。                        |
| 0.2        | patch-boundaries 同步                                   | **完成**          | `patch-boundaries.md` 指向 patch inventory source of truth，并同步 141 total / 22 high-risk。     |
| 0.3        | PR template high-risk seam / source-only gate         | **完成**          | Phase 0 表中记录 PR template checklist 和 guard tests Done。                                     |
| 0.4        | no-game validation                                    | **条件通过，不能完全关闭** | repo 文档写 303/0/21；用户贴出的执行日志仍写 294/9。                                                       |
| 1.1        | `FeatureOrders.cs`                                    | **完成**          | Phase 1 记录 Done。                                                                           |
| 1.2        | named feature modules                                 | **完成**          | Lotha/Morvi/Urda/Vakuu/Ascension feature modules 记录 Done。                                  |
| 1.3        | `SpirePlusFeatureRegistry` 改为 named modules           | **完成，但只是第一步**   | 记录 registry refactor Done。                                                                 |
| 1.4        | `VakuuFightInitializer` 独立文件                          | **完成**          | Phase 1 记录 split Done。                                                                     |
| 1.5        | `AscensionInitializer` compatibility fallback comment | **短期可接受**       | Phase 1 记录 Done。                                                                           |
| 2          | Patch Adapter Rule                                    | **未开始**         | Phase 2 明确 Not Started，目标是 high-risk patches 变成 thin adapters。                             |
| 3          | Extract Highest-Value Seams                           | **未开始**         | PreviewTransformPolicy、Banner/Firemark、RootSight、VakuuFightFlow、Ascension selection 均只是计划。 |
| 4          | State/Save Cleanup                                    | **未开始**         | UrdaProgress sub-state、codec tests、save docs 均 Not Started。                                |
| 5          | Behavior Test Upgrade                                 | **未开始**         | 行为测试仍是计划表。                                                                                 |
| StS1Events | prototype completion                                  | **未完成**         | `refactor.md` 明确写 StS1Events is prototype, not complete feature。                           |

---

## 2. 当前完成状态

| Area                       | Status                          | 严格结论                                              |
| -------------------------- | ------------------------------- | ------------------------------------------------- |
| Phase 0 structural work    | Done                            | **Pass**                                          |
| Phase 1 structural work    | Done                            | **Pass**                                          |
| Validation truth           | Single truth (324/0/21)         | **Pass** — verified 2026-05-29                     |
| Phase 2 patch adapter rule | Started / adapter checklist drafted | **Pass (Pack 1)** — not claimed Done             |
| Phase 3 seam extraction    | Not Started                     | **Not complete**                                  |
| Phase 4 state/save cleanup | Not Started                     | **Not complete**                                  |
| Phase 5 behavior tests     | Not Started                     | **Not complete**                                  |
| StS1Events                 | Prototype / feature-gated       | **Pass (Pack 2)** — default Off, 13 guard tests  |
| StS1Events source/API docs | Done (Pack 3)                   | **Pass** — act mapping, API matrix, wiki catalog  |
| StS1Events canary specs    | Source-verified (Pack 4)         | **Pass** — 4 canary specs with full fields        |
| Release-ready              | No live proof                   | **No**                                            |
| 是否可以停止                    | **可以** (Green Stop met)        | Overnight run Pack 0–5 complete; see review doc   |

---

# 3. Overnight Run：必须跑完才能停止

下面这份就是给他的 **Overnight Run Spec**。重点是：**不是跑到累了就停，而是跑到 Green Stop 或 Hard Block Stop。**

## Stop Rules

### Green Stop：允许停止

只有同时满足以下条件，才允许停止：

1. `git status --short` 已记录。
2. `dotnet build EZMicroBalance.sln` 已记录。
3. `dotnet test EZMicroBalance.sln --no-build` 已记录。
4. `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` 已记录。
5. `git diff --check` 已记录且 clean。
6. 测试结果只有一个最终口径。
7. `docs/issues.md`、`docs/goals/refactor.md`、StS1Events docs 状态一致。
8. Phase 2 patch adapter owner/seam checklist 完成。
9. StS1Events feature gate `Off / CanaryOnly` 设计或实现完成。
10. 所有 subagent 输出都有 pass/fail。
11. 没有 full parity / release-ready / live-ready 假声明。

### Hard Block Stop：允许停止

只有遇到无法继续的 blocker 才能停止，但必须留下：

```text
Blocker:
Failed command:
Exact error:
Files touched:
Current git status:
What remains:
Owner:
Next command to run:
```

### 禁止停止

以下情况都不能停止：

* “时间太晚了，先这样”
* “我的改动相关测试通过”
* “pre-existing failures，不管了”
* “All tests pass” 但日志里还有 fail
* “StS1Events 大概可以了”
* “Phase 2 已经规划了，所以算完成”

---

# 4. Overnight Run Pack

## Pack 0 — Final Validation Truth Gate

**Owner:** QA / Red-Team Auditor
**必须最先执行。**

Run:

```powershell
git status --short
git log -1 --oneline --decorate
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

Output:

```text
docs/reviews/overnight-run-20260528.md
```

Acceptance:

* 只能有一个测试口径。
* 如果是 303 pass / 0 fail / 21 skip，就删除或纠正所有 294/9、295/8 的旧口径。
* 如果仍有 fail，必须写 `Overall tests: fail`。
* `git diff --check` 必须 clean。
* `docs/issues.md` 的 validation row 必须匹配最终结果。

---

## Pack 1 — Refactor Phase 2 Patch Adapter Rule

**Owner:** Patch Adapter Auditor + Release Documentation Agent

目标：启动 Phase 2，但不能虚假标 Done。

Required work:

1. 更新 `docs/architecture/patch-boundaries.md`，为每个 high-risk patch group 标出：

   * owner
   * patch surface
   * service seam
   * forbidden behavior
   * manual evidence row
2. 不能只写“patch should be thin adapter”，必须写到 group 级别。
3. high-risk patch 不允许同 PR 同时 move + behavior change。
4. 如果新增/移动 patch，必须 regenerate `docs/patch-inventory.md`。

High-risk groups:

| Group                   | Required Seam                                                     |
| ----------------------- | ----------------------------------------------------------------- |
| Vakuu child combat      | `VakuuFightService` now, future `VakuuFightFlow`                  |
| Urda Root Eyes          | `UrdaBlessingService.RootSight*`, future `RootSightPreviewPolicy` |
| A20 dual boss           | A20 services/events                                               |
| Ascension selector      | selector gate / lobby adapter                                     |
| Ascension map           | `AscensionMapService`                                             |
| Ascension combat        | Banner / Firemark / boss combat services                          |
| Multiplayer diagnostics | diagnostics-only, no state mutation                               |
| Reward sync             | preserve vanilla reward authority                                 |
| Preview tools           | preview-only, no real mutation                                    |

Acceptance:

* Phase 2 status becomes `Started / adapter checklist drafted`，不能写 Done。
* Every high-risk patch group has owner + seam。
* No runtime proof gates are closed。
* Patch inventory remains consistent。

---

## Pack 2 — StS1Events Feature Gate Safety

**Owner:** Feature Gate / Registration Engineer + StS2 Source/API Auditor

Required work:

1. Define or implement `Sts1EventFeatureGate`:

   * `Off`
   * `CanaryOnly`
   * `AdditiveAllDraft`
   * `ReplaceUnknownEventsPrototype`
2. Default `Off` must register **0** StS1 events.
3. `CanaryOnly` must target exactly:

   * Big Fish
   * Golden Idol
   * Lab
   * Divine Fountain
4. `RegisterAll` must not be unconditionally called from default mod init.
5. If registration service remains compile-removed, document:

   * source present
   * registration inactive
   * feature gate not build-active
   * activation blocker

Acceptance:

* Off mode has a test or explicit blocker。
* CanaryOnly count has a test or explicit blocker。
* No all-event registration in default path。
* No “all events done” wording。

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

1. Resolve 46 / 48 / 52 mismatch:

   * `wiki_event_entries`
   * `runtime_event_models`
   * `act_bucket_memberships`
2. Verify act mapping. Do not keep unverified claims like:

   * `Underdocks=Act1`
   * `Overgrowth=Act2`
   * `Hive=Act3`
3. API matrix must cover:

   * HP heal / damage / max HP gain / max HP loss
   * add/remove cards and curses
   * relic grant / relic pool draw
   * potion grant
   * card select/remove/upgrade/transform UI
   * event option lock / damage / max HP tooltip
   * save/load event state

Acceptance:

* Every API has exact file/class/method evidence。
* Missing source evidence blocks implementation。
* wiki entries、runtime models、act buckets 不再混用。

---

## Pack 4 — Canary Spec Readiness

**Owner:** Wiki Spec Auditor + Localization Agent + Asset Pipeline Agent

Canary events:

* Big Fish
* Golden Idol
* Lab
* Divine Fountain

Each canary spec must include:

```text
Wiki behavior summary, rewritten not copied
Normal values
A15 values
Option table
Dependencies
Localization key plan
Asset path plan
Manual evidence checklist
Save/load notes
```

Acceptance:

* Each canary is `source-verified` or `blocked`。
* No canary may be marked `implemented` without code + tests。
* No localization may be `loc-render-verified` without screenshot。
* No asset may be `asset-verified` without path validation。

---

## Pack 5 — Final Overnight Validation

**Owner:** QA / Red-Team Auditor
Implementation agents cannot self-approve.

Run again:

```powershell
git status --short
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

Final required files:

```text
docs/reviews/overnight-run-20260528.md
docs/goals/refactor.md
docs/issues.md
docs/features/sts1-events/source-research/sts2-act-event-registration.md
docs/features/sts1-events/source-research/api-command-matrix.md
docs/features/sts1-events/wiki-event-catalog.md
```

Final acceptance:

* One validation summary only。
* Failed tests = 0, or failures issue-tracked。
* `git diff --check` clean。
* Phase 2 not marked Done。
* StS1Events not marked complete。
* Subagent output table complete。
* If validation fails, no push。

---

# 5. Monthly Dev Spec

## 2026-05-28 → 2026-06-30

## Month Objective

> **Deliver Refactor Phase 2 foundation + StS1 Event Port Prototype Batch 1.**

Not accepted:

* full parity claim
* release-ready claim
* live-ready claim without logs/screenshots
* co-op-ready claim without two-client evidence
* source-only pass closing live proof gates

Month-end accepted state:

* Phase 2 patch adapter rule started and applied to at least one low-risk group.
* High-risk patch owner/seam map complete.
* `PreviewTransformPolicy` or another low-risk seam extracted or characterized.
* StS1Events default `Off` = no behavior impact.
* `CanaryOnly` registers exactly 4 canary events.
* Four canary events playable and manually verified.
* Six simple events debug-spawn verified.
* Asset/localization validation done for monthly events.
* Status board uses only allowed statuses.
* Evidence bundle complete.

---

## Week 0 — Overnight Run

Focus:

* Final validation truth.
* Phase 2 adapter checklist.
* StS1Events feature gate safety.
* Source/API evidence skeleton.

Acceptance:

* Overnight Pack 0–5 complete.
* No false green.
* No full parity wording.
* No stop before Green Stop or Hard Block Stop.

---

## Week 1 — Gate + API Verification

Required:

* `Sts1EventFeatureGate`
* Off / CanaryOnly registration tests
* act registration source evidence
* API command matrix
* wiki catalog count policy

Acceptance:

* Off mode registers 0.
* CanaryOnly registers exactly 4 canaries.
* API matrix complete for canary work.
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

* Every canary branch debug-spawned.
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

## Week 4 — Pool Prototype + First Seam Extraction

Required:

* `Sts1EventPoolService` design doc.
* Debug-only `ReplaceUnknownEventsPrototype`, only if source evidence supports it.
* Save visited event ids and event bag state.
* Multiplayer fail-closed.
* Start `PreviewTransformPolicy` or other low-risk seam extraction.

Acceptance:

* Replacement mode disabled by default.
* Dev replacement draws only implemented StS1 events.
* Save/load does not duplicate/skip bag state.
* Multiplayer refuses replacement unless debug override.
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

* Evidence bundle includes logs、screenshots、asset validation output、test output、status board。
* Release notes say `Prototype Batch 1`, not full parity。
* No release-ready wording。
* Commit/push only after validation passes。

---

# 6. Mandatory Subagent Plan

他必须用 subagent。实现者不能自审，QA 必须独立。

| Subagent                             | Scope                                                                                                                | Output                                                      | Pass / Fail                                |
| ------------------------------------ | -------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------- | ------------------------------------------ |
| Wiki Spec Auditor                    | 核对 52 Wiki entries；拆分 event entries/runtime models/act memberships；产出 canary + simple batch exact options/A15 deltas | corrected catalog；10 monthly specs；46/48/52 mismatch report | monthly specs 有 exact options/dependencies |
| StS2 Source/API Auditor              | verify act mapping、RitsuLib registration、HP/relic/curse/card/save APIs                                               | `sts2-act-event-registration.md`、`api-command-matrix.md`    | exact file/class/method evidence           |
| Feature Gate / Registration Engineer | Off / CanaryOnly / AdditiveAllDraft / ReplaceUnknownEventsPrototype                                                  | `Sts1EventFeatureGate`、gated registration、count tests       | default Off registers nothing              |
| Patch Adapter Auditor                | high-risk patches thin adapter 审核                                                                                    | owner/seam/checklist map                                    | every high-risk group has owner + seam     |
| Canary Implementation Engineer       | Big Fish / Golden Idol / Lab / Divine Fountain                                                                       | playable canary code、helpers、branch tests                   | QA evidence 后 pass                         |
| Simple Batch Engineer                | 六个 simple events                                                                                                     | playable simple events                                      | QA evidence 后 pass                         |
| Asset Pipeline Agent                 | event id → local StS1 art paths；copyright-safe extraction；load validation                                            | asset manifest、validation output                            | path validation + screenshots              |
| Localization Agent                   | EN/ZHS text、dynamic variables、formatting                                                                             | localization json + render screenshots                      | no missing keys / no placeholders          |
| QA / Red-Team Auditor                | 独立复核；run build/test/publish；debug-spawn；save/load；gate Off                                                           | manual evidence、screenshot/log index、pass/fail table        | cannot be implementation subagent          |
| Release Documentation Agent          | README/PROJECT_MAP/status board/test plan/monthly review/release notes                                               | exact status docs、no full parity wording                    | status board matches evidence bundle       |

---

## 给他的直接 Overnight 指令

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
