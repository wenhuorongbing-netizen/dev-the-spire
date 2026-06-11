# 严格审核结论

**当前不能判定完成。**
最新仓库状态已经不是“loader gate 已稳定通过”的状态，而是：

> **No-game validation 通过；当前 installed game 是 `v0.107.0`，beta.85 Off loader smoke 已经 clean。这个 proof 只关闭 startup/default-Off patch application；CanaryOnly/AdditiveBatch1、gameplay、save-load、co-op、QA、handoff 仍未完成。Batch 4c 仍然 proposal-only，不能当作 tester-ready 或 release-ready。**

最新 `docs/reviews/current-validation.md` 显示：当前 HEAD 是 `f32c6767` 前的工作状态，本轮源代码修复适配了 installed game DLL API；build/test/format/patch inventory/worktree classifier 都通过，其中 build 是 **0 warnings / 0 errors**，solution test 是 **464 passed / 0 failed / 21 skipped / 485 total**。

但同一份最新验证也明确写：**当前没有 gameplay、event UI、save-load、co-op、release package 或 live-ready evidence**；本地 installed game 已是 `v0.107.0`，RitsuLib 更新到官方 `v0.4.16` + `lib\0.107.0`。beta.84 Off smoke 的 17/25 patches 和 stale `EctoplasmGoldGatePatch` target API drift 是 root-cause history；beta.85 Off smoke 已经到达 main menu、25/25 patches、clean audit。

所以本轮结论是：

```text
No-game validation: PASS
Current v0.107.0 Off loader proof: PASS for beta.85 startup/default-Off
Gameplay proof: NOT DONE
Save/load proof: NOT DONE
Co-op proof: NOT DONE
Package handoff: NOT DONE
Batch 4c: DO NOT EXECUTE
Release-ready / live-ready: NO
```

---

## 1. Step-by-step 审核

| Step                              | 目标                     | 当前证据                                                                                                                                                                                                 | 严格判定                   |
| --------------------------------- | ---------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------- |
| Source API compatibility          | 适配当前 installed DLL API | 当前 pass 修复为 `AbstractModel.ModifyPowerAmountGivenAdditive(...)`、`Ectoplasm.ModifyGoldGained(...)`、`CookRestSiteOption.get_IsEnabled`。                                                                | **完成本轮源码修复**           |
| Build                             | 0 errors               | `dotnet build EZMicroBalance.sln -m:1 --no-incremental` PASS，0 warnings / 0 errors。                                                                                                                  | **PASS**               |
| Focused Sts1Event tests           | Sts1Event guard green  | 31 passed / 0 failed / 0 skipped。                                                                                                                                                                    | **PASS**               |
| Full test project                 | 0 failed               | 464 passed / 0 failed / 21 skipped / 485 total。                                                                                                                                                      | **PASS**               |
| Solution-level tests              | 0 failed               | solution-level test also 464 / 0 / 21 / 485。                                                                                                                                                         | **PASS**               |
| Format / patch inventory          | clean                  | format PASS，generate-patch-inventory PASS。                                                                                                                                                           | **PASS**               |
| Diff check                        | whitespace clean       | PASS，但有 CRLF normalization warnings only。                                                                                                                                                            | **PASS with note**     |
| Worktree classifier               | 无 unclassified         | PASS，130 dirty entries / 0 unclassified。                                                                                                                                                             | **分类通过，但 worktree 未清** |
| Runtime smoke on current v0.107.0 | clean runtime proof    | 最新 v0.107.0 beta.84 Off smoke reached main menu but failed clean runtime proof：11 Godot ERROR、1 Spire Plus error/exception、8 optional ModPatcher failures、`EctoplasmGoldGatePatch` target API drift。 | **FAIL / BLOCKED**     |
| Gameplay / UI / save-load / co-op | live proof             | 最新 pass 明确没有 gameplay、event UI、save-load、co-op、release package 或 live-ready evidence。                                                                                                                | **NOT DONE**           |
| Historical June 2 smoke           | prior loader proof     | 历史 Off / CanaryOnly / AdditiveBatch1 smoke 在旧环境可用，但 current-validation 明确历史段落不能不经比较当作当前 refactor truth。                                                                                              | **只能做历史参考**            |
| Batch 4c                          | 迁移更多 patch             | 当前 runtime blocked，不能推进。                                                                                                                                                                             | **MUST NOT START**     |
| Release-ready / live-ready        | 可发布                    | no live proof，runtime blocked。                                                                                                                                                                       | **NO**                 |

---

## 2. 当前状态与目标对比

你的目标是：严格确认是否完成、对比项目目标、决定继续优化还是推进，并设置夜间运行直到完成。当前目标的关键门槛不是“测试是否绿”，而是 **RitsuLib runtime proof + StS1Events runtime gate + gameplay/save-load/co-op evidence**。

| 目标                                | 当前状态                                  | 差距                |
| --------------------------------- | ------------------------------------- | ----------------- |
| No-game validation                | 通过                                    | 可保留               |
| 当前 v0.107.0 source API 适配         | 部分修复，build/test 通过                    | 还要验证 runtime      |
| RitsuLib runtime proof            | 当前 Off smoke 非 clean                  | P0 blocker        |
| Sts1Events Off / CanaryOnly proof | 历史 v0.106.1 有证据；当前 v0.107.0 未重新 clean | 需要重跑              |
| ModPatcher patch proof            | 当前有 8 optional failures               | 需要修 / 分类          |
| EctoplasmGoldGatePatch            | 当前 runtime API drift root             | 需要立即修             |
| Gameplay proof                    | 无                                     | 未完成               |
| Save/load proof                   | 无                                     | 未完成               |
| Co-op proof                       | 无                                     | 未完成               |
| Worktree                          | 130 dirty entries classified          | 需要 owner decision |
| Package handoff                   | 无                                     | 未完成               |
| Release-ready                     | no                                    | 正确，不可 claim       |

**结论：现在不是推进 Batch 4c 的时机。当前必须继续优化 / 修复 runtime blocker。**

---

# 3. 主要问题

## Issue 1: 当前 v0.107.0 runtime proof 失败

* **Severity:** P0 / Critical
* **Evidence:** 最新 runtime status 明确：v0.107.0 beta.84 Off smoke 到 main menu 但失败，有 11 Godot ERROR、1 Spire Plus error/exception、8 optional ModPatcher failures，根因之一是 stale `EctoplasmGoldGatePatch` target API drift。
* **Impact:** 不能说 runtime-safe，不能启动 Batch 4c，不能 tester-ready。
* **Required fix:** 立刻创建 runtime blocker issue：`V01070-RUNTIME-ECTOPLASM-GOLD-GATE-API-DRIFT`。
* **Acceptance:** fresh v0.107.0 Off smoke clean audit：0 Godot ERROR、0 Spire Plus error/exception、0 release-blocking hits；25/25 mandatory patches pass；optional failures either fixed or explicitly downgraded with evidence。

---

## Issue 2: 旧 runtime smoke 不能替代当前环境

* **Severity:** P0
* **Evidence:** `current-validation.md` 明确说 historical sections are dated evidence records，不要把旧 warning counts、runtime version 或 pass/fail 当作 current truth。
* **Impact:** 不能用 6 月 2 日 v0.106.1 的 Off/CanaryOnly 成功证明当前 v0.107.0 可用。
* **Required fix:** 重跑当前环境：

  * Off
  * CanaryOnly
  * AdditiveBatch1 only if Off / CanaryOnly clean
* **Acceptance:** 三个 mode 都有 current v0.107.0 evidence，且 Off / CanaryOnly 至少 clean。

---

## Issue 3: Worktree dirty 数量变大

* **Severity:** P1
* **Evidence:** 当前 classifier PASS 但有 130 dirty entries / 0 unclassified。
* **Impact:** 不能安全 handoff / package；也难以判断哪些是本轮修复、哪些是积压。
* **Required fix:** Worktree Governance Agent 生成 dirty ledger。
* **Acceptance:** 每个 dirty batch 标注 owner / commit / defer / archive / local-only；Green Stop 前 clean 或 owner-approved dirty state。

---

## Issue 4: Optional ModPatcher failures 需要分类

* **Severity:** P1
* **Evidence:** 当前 runtime smoke 有 8 optional ModPatcher failures。
* **Impact:** “optional” 不等于安全；可能掩盖真实 feature 失效。
* **Required fix:** 列出 8 个 failure 的 patch id、target method、feature impact、是否可降级、是否测试覆盖。
* **Acceptance:** 每个 failure 有 `fix / defer / disable / document` 决策。

---

## Issue 5: Gameplay / save-load / co-op 仍未开始

* **Severity:** P1
* **Evidence:** 当前 pass 明确没有 gameplay、event UI、save-load、co-op evidence。
* **Impact:** 即使 loader clean，也不能 tester-ready。
* **Required fix:** runtime clean 后再跑 manual smoke。
* **Acceptance:** Mod Settings UI、run start、first combat、save/load、Canary event UI 至少部分完成。

---

# 4. 是否完成？

## 已完成

* 当前 no-game validation。
* 当前 build/test 修复。
* 当前 source API compile compatibility。
* Patch inventory freshness。
* Worktree classification 0 unclassified。
* 旧环境 loader proof 可作为历史参考。

## 未完成

* 当前 v0.107.0 clean runtime proof。
* `EctoplasmGoldGatePatch` runtime blocker 修复。
* Optional ModPatcher failure 分类。
* Off / CanaryOnly 在 v0.107.0 下重新 clean proof。
* Gameplay proof。
* Save/load proof。
* Co-op proof。
* Clean worktree / owner-approved dirty ledger。
* Tester package。
* Batch 4c。
* Release-ready / live-ready。

---

# 5. 决策：继续优化、推进，还是两者兼顾？

**结论：优化为主，推进暂停。**

建议比例：

```text
90% runtime blocker 修复 / evidence cleanup / worktree governance
10% planning only
```

现在不应该继续推进 Sts1Events 或 Batch 4c。当前唯一允许的“推进”是：

```text
- 低风险 Batch 4c candidate proposal
- 不改代码的 patch failure inventory
- runtime blocker triage
```

禁止：

```text
- Batch 4c actual migration
- high-risk patch migration
- new gameplay
- versioned tester package
- release-ready / live-ready claim
```

---

# 6. 下个月开发规范

## Monthly Dev Spec: 2026-06 — v0.107 Runtime Recovery + Evidence Governance

## 月度目标

1. 修复 v0.107.0 runtime blocker。
2. 重建 current runtime smoke proof。
3. 分类 8 个 optional ModPatcher failures。
4. 清理或 owner-approve 130 dirty entries。
5. 在 current runtime clean 后，再做 gameplay / Canary UI / save-load。
6. 不启动 Batch 4c，除非 runtime smoke clean + owner approval。
7. 不 claim release-ready / live-ready。

---

## Week 1 — v0.107 Runtime Recovery

**Required Work**

1. 建立 blocker issue：

   * `V01070-RUNTIME-ECTOPLASM-GOLD-GATE-API-DRIFT`
2. 修 `EctoplasmGoldGatePatch` target API drift。
3. 列出 8 optional ModPatcher failures：

   * patch id
   * target
   * feature
   * required / optional
   * fix/defer decision
4. Rebuild / retest。
5. Fresh Off runtime smoke。

**Acceptance Criteria**

* Off runtime smoke clean。
* 0 Godot ERROR。
* 0 Spire Plus error/exception。
* 0 MissingMethodException / TypeLoadException。
* Ectoplasm blocker closed。
* Optional failures classified。

---

## Week 2 — Current Off / CanaryOnly / AdditiveBatch1 Smoke

**Required Work**

1. Off mode:

   * default env
   * 0 Sts1Events registration
   * clean audit
2. CanaryOnly:

   * 4 canary registrations
   * clean audit
3. AdditiveBatch1:

   * 10 event types / 11 registrations
   * clean audit
   * still dev/test scope

**Acceptance Criteria**

* Current v0.107.0 evidence exists。
* Historical v0.106.1 evidence marked historical only。
* QA can reproduce log review。

---

## Week 3 — Worktree Governance

**Required Work**

* Resolve 130 dirty entries：

  * commit
  * defer
  * archive
  * local-only
  * owner-approved dirty
* Split by batch。
* No broad reset / clean without owner approval。

**Acceptance Criteria**

* clean worktree or owner-approved dirty ledger。
* no unclassified dirty files。
* docs reflect current state。

---

## Week 4 — Manual Gameplay Smoke

Only after runtime smoke clean.

**Required Work**

* Mod Settings UI screenshot。
* Start run。
* First combat。
* Shop or unknown room。
* Save/load attempt。
* CanaryOnly event UI proof if possible。

**Acceptance Criteria**

* gameplay smoke doc exists。
* logs/screenshots attached。
* no release-ready claim。

---

## Week 5 — QA / Handoff Decision

**Required Work**

* Independent QA/Red-Team rerun。
* Decide:

  * local diagnostic only
  * versioned tester package
* If package:

  * publish
  * package
  * hashes
  * artifact tests
* If no package:

  * document no tester handoff。

**Acceptance Criteria**

* QA verdict no longer blocked by current runtime issue。
* package decision explicit。
* Batch 4c decision explicit。

---

# 7. Overnight Run 设置

## v0.107 Runtime Recovery Overnight Run

**必须持续运行到 Green Stop 或 Hard Block Stop。不能 soft stop。**

## Green Stop 条件

全部满足才允许停止：

1. `EctoplasmGoldGatePatch` runtime blocker fixed or hard-blocked with exact evidence。
2. 8 optional ModPatcher failures listed and classified。
3. build/test/format/diff-check green。
4. fresh v0.107 Off smoke clean。
5. fresh v0.107 CanaryOnly smoke clean，或 explicitly blocked after Off clean。
6. `godot-log-audit.json` clean。
7. `git status --short` clean，或 dirty ledger owner-approved。
8. QA/Red-Team subagent rerun。
9. no Batch 4c execution。
10. no release-ready / live-ready claim。

## Hard Block Stop 模板

```text
Blocker:
Failed command:
Exact error:
Target API / patch id:
Runtime/log path:
Current git status:
Dirty count:
Files touched:
What remains:
Owner:
Next command:
```

## 禁止停止

* main menu reached 但 audit 不 clean就停。
* optional patch failures 不分类就停。
* 用历史 v0.106 proof 代替当前 v0.107 proof。
* Dirty 130 entries 不处理就 handoff。
* QA 不 rerun 就说完成。
* Batch 4c 偷跑。

---

# 8. Subagent Plan

| Subagent                  | Scope                                                  | Output                      | Pass/Fail                     |
| ------------------------- | ------------------------------------------------------ | --------------------------- | ----------------------------- |
| Runtime Blocker Agent     | 修 Ectoplasm/API drift + runtime smoke                  | blocker fix report + logs   | Off smoke clean or hard block |
| Patch Failure Auditor     | 分类 8 optional ModPatcher failures                      | failure matrix              | every failure has decision    |
| Runtime Smoke Agent       | Off / CanaryOnly / AdditiveBatch1 current v0.107 smoke | godot logs + audit json     | PASS / BLOCKED                |
| Worktree Governance Agent | 130 dirty entries 分类                                   | dirty ledger                | owner-approved                |
| QA / Red-Team Auditor     | 独立复核 runtime/docs/worktree                             | QA verdict                  | cannot self-audit             |
| Documentation Agent       | current-validation/issues/runtime checklist sync       | unified docs                | no stale v0.106 claim         |
| Release Safety Agent      | 禁止 release/live/full parity claim                      | safety checklist            | runtime rows remain open      |
| Batch 4c Planning Agent   | 只做候选，不执行                                               | low-risk candidate proposal | owner approval required       |

---

## 给助理的直接指令

```text
当前不能判定完成。No-game validation 是绿的，但当前 v0.107.0 runtime proof 失败：Off smoke 到了 main menu 但 audit 不干净，有 11 Godot ERROR、1 Spire Plus error/exception、8 optional ModPatcher failures，且 TargetInvocationException 根因是 EctoplasmGoldGatePatch target API drift。

现在进入 v0.107 Runtime Recovery Overnight Run：

1. 先修 EctoplasmGoldGatePatch target API drift，或写 hard-block report。
2. 列出 8 optional ModPatcher failures，逐个分类 fix/defer/disable/document。
3. 重新跑 build/test/format/diff-check。
4. 重新跑 v0.107 Off smoke，必须 clean audit。
5. Off clean 后再跑 CanaryOnly smoke，验证 exactly 4 canary registrations。
6. 不允许用 v0.106 历史 smoke 当当前证明。
7. 处理 130 dirty entries：clean 或 owner-approved dirty ledger。
8. 启动 QA/Red-Team subagent，不能自审。
9. 禁止 Batch 4c，禁止 high-risk migration，禁止新 gameplay，禁止 release-ready/live-ready claim。
10. 只有 Green Stop 或 Hard Block Stop 才能停止。
```

---

## 最终一句话结论

**当前最应该优先解决的是 v0.107.0 runtime blocker 和 optional ModPatcher failures，因为 no-game validation 已经通过，但当前实际 runtime proof 失败；在 clean runtime proof 之前，不应继续迁 patch、扩功能或交付 tester package。**
