# 严格审核结论

**不能判定“全部完成”。**
当前最准确的状态是：

> **当前 source/static/no-game 治理继续收敛，beta.85 在 v0.107.0 下的默认 Off loader proof 已经通过；但 CanaryOnly / AdditiveBatch1 当前启用模式、gameplay、Mod Settings UI、save-load、co-op、replacement、independent QA、versioned handoff 仍未完成。当前应判定为：PARTIAL PASS / RELEASE STILL BLOCKED。**

最新 `current-validation.md` 记录：6 月 15 的 addendum 没有启动 build/test/publish/package/runtime smoke/commit/push，只做 pause-safe static verification；静态套件、doc-claims、gate-ledger、static hygiene、subagent coverage 等检查为 0 mismatch / 0 suite failures；但这些检查**不关闭** O25、O33、enabled-mode、gameplay、save/load、replacement、multiplayer、image/render、QA、release、handoff gates。

当前 runtime 方面，beta.85 的 **Off loader proof** 已经可用：在 v0.107.0 / RitsuLib 0.4.16 下，25/25 ModPatcher patches applied，Sts1Events default Off，main menu reached，audit clean。 但同一份文档明确说：这不能证明 CanaryOnly、AdditiveBatch1、gameplay、save-load、replacement、multiplayer 或 QA。

---

## 1. 当前状态与目标对比

| 目标                           | 当前状态                                                                                                                                               | 审核结论       |
| ---------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- | ---------- |
| Validation truth             | 6 月 15 只做 pause-safe static verification；之前 beta.85 split validation 记录 build/test/artifact lanes 通过。                                              | **部分通过**   |
| v0.107.0 Off loader proof    | beta.85 Off loader proof clean，25/25 patches，Sts1Events default Off。                                                                               | **通过**     |
| CanaryOnly current proof     | runtime checklist 明确 CanaryOnly 当前是 pending；历史 K1 证明旧 4 canary registrations，但需在 beta.85 / v0.107.0 重跑，当前期望为 4 event types / 6 registration calls。 | **未完成**    |
| AdditiveBatch1 current proof | 当前 pending；历史 proof 是旧 10 event types / 11 calls，当前 source 期望 10 event types / 14 calls。                                                           | **未完成**    |
| Gameplay proof               | Mod Settings UI、basic gameplay、save-load、multiplayer 全部 pending。                                                                                   | **未完成**    |
| Independent QA               | 当前 gate map 仍把 QA / release / handoff 留为 pending 或 blocked。                                                                                        | **未完成**    |
| Batch 4c                     | 只允许 candidate review；monthly spec 明确 Batch 4c 是 proposal-only，迁移需要 owner approval 和 fresh validation。                                              | **不能执行迁移** |
| Release-ready / live-ready   | monthly spec 明确 release-ready 和 live-ready 仍然是 no。                                                                                                 | **不能声明**   |

---

# 2. Step-by-step 严格审核

## Step 1 — 当前 no-game / static validation

**Observation:** 6 月 15 addendum 说明没有启动 build/test/runtime/package/push，只进行了 pause-safe static verification；`check-sts1-event-static-suite.ps1`、doc claims、static-file hygiene、v19 gate ledger、subagent coverage 等检查均 0 mismatch / 0 suite failure。

**Inference:** 这证明文档和静态 guard 近期没有明显漂移，但不能替代 build/runtime/gameplay。

**Verdict:** **PASS for static verification only。**

---

## Step 2 — 当前 beta.85 Off loader proof

**Observation:** runtime checklist 记录当前 beta.85 Off smoke 在 v0.107.0 下通过：RitsuLib 0.4.16 compat branch 0.107.0，25/25 Spire Plus patches，Sts1Events default Off，main menu reached，audit clean。

**Inference:** 之前 beta.84 的 Ectoplasm / optional ModPatcher drift 已有 beta.85 修复方向，Off loader path 当前可作为有效 proof。

**Verdict:** **PASS for Off loader gate。**

---

## Step 3 — CanaryOnly enabled-mode proof

**Observation:** current checklist 明确 CanaryOnly 当前还是 pending；历史 K1 smoke 只证明旧 4 canary registrations，必须在 beta.85 / v0.107.0 重跑，当前期望是 4 event types / 6 registration calls。

**Inference:** 不能把历史 CanaryOnly proof 当作当前 beta.85 proof。

**Verdict:** **NOT COMPLETE。**

---

## Step 4 — AdditiveBatch1 enabled-mode proof

**Observation:** current checklist 明确 AdditiveBatch1 当前 pending；历史 proof 是旧 10 event types / 11 calls，而当前 source 期望 10 event types / 14 calls。

**Inference:** AdditiveBatch1 当前 proof 缺口更明显，因为 registration count 已变。

**Verdict:** **NOT COMPLETE。**

---

## Step 5 — Gameplay / Mod Settings / save-load / multiplayer

**Observation:** runtime checklist 的 Mod Settings UI、basic gameplay、multiplayer disposition 均为 pending。

**Inference:** 当前最多是 loader-gate / static proof，不能说 tester-ready。

**Verdict:** **NOT COMPLETE。**

---

## Step 6 — Package / dependency alignment

**Observation:** monthly spec 说明本地 STS2-RitsuLib 是 0.4.16 + 0.107.0 runtime variant；但 repo compile package 仍是 `STS2.RitsuLib 0.3.2`，且不应在 dirty source state 原地 bump。未来 owner-approved v0.107.0 tester package pass 才应把 compile package 和 manifest minimum bump 到 0.4.16，并伴随 package-version、publish/package、artifact-test、loader-smoke。

**Inference:** 现在不应随手改依赖版本；这是一个 package pass 决策，不是普通代码 cleanup。

**Verdict:** **Owner decision pending。**

---

## Step 7 — Batch 4c

**Observation:** monthly spec 明确 Batch 4c 是 proposal-only，当前候选清单在 `batch-4c-candidates.md`，候选规则限制为 5–10 个 low-risk patch classes，禁止 run lifecycle、save/load、map generation、multiplayer/lobby、death、A20 boss flow、reward mutation with player state；迁移需要 explicit owner approval 和 fresh validation。

**Inference:** 当前可以审候选，但不能实际迁移。

**Verdict:** **PLAN ONLY。**

---

# 3. 当前最主要风险

## Issue 1: CanaryOnly / AdditiveBatch1 当前 enabled-mode proof 缺失

* **Severity:** 4
* **Priority:** P0
* **Observation:** 当前 beta.85 Off proof 通过，但 CanaryOnly 和 AdditiveBatch1 仍是 current pending；历史 proof 不适用当前 expected shape。
* **Impact:** Sts1Events 不能进入 current tester-ready，AdditiveBatch1 也不能进入 staging claim。
* **Recommendation:** 下一次 controlled runtime lane 先跑 CanaryOnly，再跑 AdditiveBatch1。
* **Acceptance:** 产生 `enabled-mode-log-check.json` 和 `runtime-evidence-packet-check.json`，CanaryOnly = 4 types / 6 calls，AdditiveBatch1 = 10 types / 14 calls。

---

## Issue 2: Off loader proof 不能扩展为 gameplay proof

* **Severity:** 4
* **Priority:** P0
* **Observation:** checklist 明确 beta.85 Off loader proof 只能证明 loader startup、RitsuLib compat、25/25 patches、default-Off state；不证明 event gameplay、screenshots、save-load、replacement、multiplayer 或 QA。
* **Impact:** 如果现在 claim live-ready / release-ready，会是 false green。
* **Recommendation:** 在 enabled-mode proof 后进入 manual gameplay pass。
* **Acceptance:** Mod Settings UI、run start、first combat、shop/unknown、save-load、multiplayer disposition 至少完成指定 rows。

---

## Issue 3: 当前验证暂停边界必须遵守

* **Severity:** 3
* **Priority:** P1
* **Observation:** monthly spec 明确同仓库 validation pause active 时，不能从该线程运行 no-game validation、package/release-evidence、runtime/game smoke、staging、commit、push；该 spec 仅用于 read-only/static planning 和已捕获证据复核。
* **Impact:** 多线程/多代理同时动仓库会导致 testhost crash、dirty drift、false evidence。
* **Recommendation:** 下次 overnight 必须指定唯一 lane 和 owner。
* **Acceptance:** 只有一个 validation lane 持有执行权；其他 subagents 只做 read-only review。

---

## Issue 4: Build/package dependency drift 需 owner-approved package pass

* **Severity:** 3
* **Priority:** P1
* **Observation:** compile package 仍 0.3.2，本地 runtime 0.4.16；monthly spec 明确不在 dirty source state 原地 bump，未来 versioned tester package 才处理。
* **Impact:** 若随意 bump，会影响 manifest dependency、package、tester install、artifact tests。
* **Recommendation:** 作为 Week 4 package decision，不作为 immediate cleanup。
* **Acceptance:** owner 决定 “local diagnostic only” 或 “v0.107.0 tester package with 0.4.16 bump”。

---

## Issue 5: Batch 4c 候选只能审，不可迁

* **Severity:** 4
* **Priority:** P0
* **Observation:** monthly spec 明确 Do not migrate Batch 4c or high-risk patches in this phase。
* **Impact:** 继续迁移会扩大未验证 runtime surface。
* **Recommendation:** Batch 4c Planning Agent 只产出候选 review。
* **Acceptance:** 无新的 IPatchMethod migration，除非 owner approval + fresh validation。

---

# 4. 当前是否完成

## 已完成

* beta.85 Off loader proof for v0.107.0。
* package parity / installed package evidence 已有记录。
* static verification / doc-claim / gate-ledger / hygiene checks 已有 0 mismatch 记录。
* API drift root cause beta.84 -> beta.85 修复方向已记录。
* Release / live ready 没有被错误声明。
* Batch 4c 当前保持 proposal-only。

## 未完成

* current beta.85 CanaryOnly enabled-mode proof。
* current beta.85 AdditiveBatch1 enabled-mode proof。
* Mod Settings UI proof。
* basic gameplay proof。
* save-load proof。
* co-op / fail-closed proof。
* replacement proof。
* independent QA runtime review。
* versioned tester package decision。
* Batch 4c migration。
* release-ready / live-ready。

---

# 5. 决策：继续优化、推进，还是两者兼顾？

**结论：两者兼顾，但推进范围必须非常窄。**

当前应采用：

```text
50% current enabled-mode runtime proof
30% manual gameplay / UI / save-load evidence
20% Batch 4c candidate review + docs/QA governance
```

可以推进：

* CanaryOnly current enabled-mode smoke；
* AdditiveBatch1 current enabled-mode smoke；
* Mod Settings UI proof；
* basic gameplay smoke；
* save-load attempt；
* Batch 4c candidate review；
* package decision review。

不能推进：

* Batch 4c actual migration；
* high-risk patch migration；
* AdditiveAllDraft tester/live path；
* replacement live path；
* release-ready / live-ready claim；
* dependency bump without owner-approved package pass。

---

# 6. 下个月开发规范

## Monthly Dev Spec: 2026-06 — Enabled-Mode Runtime Proof + Tester-Readiness Gate

## 月度目标

1. 保持 beta.85 Off loader proof 有效。
2. 补 current beta.85 CanaryOnly proof。
3. 补 current beta.85 AdditiveBatch1 proof。
4. 补 Mod Settings UI / basic gameplay / save-load。
5. 完成 independent QA review。
6. 决定 package path：local diagnostic only 或 owner-approved v0.107.0 tester package。
7. 只审 Batch 4c candidates，不执行迁移。
8. 不 claim release-ready / live-ready / full parity。

---

## Week 1 — Current Enabled-Mode Runtime Proof

### Required Work

Run controlled runtime lane only after coordination pause lifted.

**CanaryOnly**

```powershell
SPIREPLUS_STS1_EVENT_MODE=CanaryOnly
```

Expected:

* package version beta.85；
* game version 0.107.0；
* RitsuLib 0.4.16；
* Ritsu compat branch 0.107.0；
* 4 event types；
* 6 registration calls；
* no unsafe-mode leakage；
* clean audit。

**AdditiveBatch1**

```powershell
SPIREPLUS_STS1_EVENT_MODE=AdditiveBatch1
```

Expected:

* 10 event types；
* 14 registration calls；
* clean audit；
* still prototype/test mode。

### Acceptance

* `enabled-mode-log-check.json` exists for both modes。
* `runtime-evidence-packet-check.json` exists for both modes。
* No mismatches。
* Historical 0.106 / 0.3.10 proof remains historical only。

---

## Week 2 — Mod Settings + Basic Gameplay

### Required Work

* Launch with default Off。
* Capture Mod Settings list screenshot。
* Open Spire Plus settings。
* Start one run。
* Complete first combat。
* Visit shop or unknown room。
* Save/load once。

### Acceptance

* `godot.log` clean。
* screenshots stored。
* save/load result recorded。
* if blocked, Hard Block report。

---

## Week 3 — Canary Event UI / Branch Proof

### Required Work

CanaryOnly mode:

* Big Fish
* Golden Idol
* The Lab
* Divine Fountain

For each:

* event UI screenshot；
* option text screenshot；
* one branch result；
* no missing localization key；
* no crash / softlock。

### Acceptance

* 4 event UI rows complete or blocked with reason。
* at least 1 save-load attempt during/after canary。
* status board updated to manual partial proof, not release proof。

---

## Week 4 — QA + Package Decision

### Required Work

Independent QA reviews:

* Off loader proof；
* CanaryOnly enabled-mode proof；
* AdditiveBatch1 enabled-mode proof；
* gameplay smoke；
* canary UI proof；
* package/version decision；
* Batch 4c candidate list。

Package decision:

1. **Local diagnostic only**；or
2. **v0.107.0 tester package**:

   * bump STS2.RitsuLib compile package / manifest minimum to 0.4.16；
   * publish；
   * package；
   * hash docs；
   * release artifact tests；
   * fresh loader smoke。

### Acceptance

* QA verdict PASS / PARTIAL / BLOCKED。
* package decision explicit。
* no release-ready claim。

---

## Week 5 — Batch 4c Candidate Review Only

### Required Work

Review 5–10 candidates:

* patch class；
* target method；
* risk；
* prohibited-surface check；
* expected seam；
* rollback；
* tests；
* manual evidence requirement。

### Acceptance

* proposal only。
* no migration unless owner explicitly approves。
* no high-risk patch migration。

---

# 7. Overnight Run 设置

## Enabled-Mode Runtime Proof Overnight Run

**必须持续到 Green Stop 或 Hard Block Stop。不得 soft stop。**

## Green Stop 条件

全部满足才可以停止：

1. coordination pause lifted / one execution lane assigned。
2. `git status --short` clean，或 dirty ledger owner-approved。
3. no-game validation raw logs current。
4. CanaryOnly beta.85 / v0.107.0 enabled-mode smoke completed。
5. CanaryOnly `enabled-mode-log-check.json` 0 mismatch。
6. CanaryOnly `runtime-evidence-packet-check.json` 0 mismatch。
7. AdditiveBatch1 enabled-mode smoke completed or Hard Block。
8. AdditiveBatch1 verifier outputs 0 mismatch or blocker。
9. Mod Settings UI proof captured or scheduled as next Green Stop item。
10. Independent QA subagent reviewed runtime evidence。
11. no Batch 4c migration。
12. no release-ready / live-ready / full parity claim。

## Hard Block Stop 模板

```text
Blocker:
Mode:
Failed command:
Exact error:
Evidence folder:
Audit output:
Current git status:
Files touched:
What remains:
Owner:
Next command:
```

## 禁止停止

* 用历史 K1 CanaryOnly proof 代替 beta.85 proof。
* 只跑 Off 就 claim enabled-mode safe。
* enabled-mode log 没有 verifier JSON。
* evidence packet 缺 session/restore/isolation metadata。
* QA 没 review 就停止。
* Batch 4c 偷跑。
* 任何 release/live/full parity claim。

---

# 8. Subagent Plan

| Subagent                   | Scope                                                                     | Output                      | Pass/Fail                 |
| -------------------------- | ------------------------------------------------------------------------- | --------------------------- | ------------------------- |
| Runtime Enabled-Mode Agent | 跑 CanaryOnly / AdditiveBatch1 beta.85 smokes                              | log + audit + verifier JSON | PASS / BLOCKED            |
| Runtime Packet Auditor     | 验证 evidence folder metadata、session、restore、isolation                     | packet-check JSON           | 0 mismatch required       |
| QA / Red-Team Auditor      | 独立复核 evidence，不改代码                                                        | QA verdict                  | cannot self-audit         |
| Gameplay Smoke Agent       | Mod Settings / run start / save-load                                      | screenshots + logs          | PASS / BLOCKED            |
| Sts1 Canary Evidence Agent | 4 canary event UI/branch proof                                            | evidence doc                | PASS / PARTIAL / BLOCKED  |
| Package Decision Agent     | local diagnostic vs tester package                                        | decision doc                | owner-approved            |
| Batch 4c Planning Agent    | 候选审查，不执行                                                                  | candidate proposal          | owner approval required   |
| Documentation Agent        | current-validation / runtime checklist / monthly spec / status-board sync | unified docs                | no stale historical claim |
| Release Safety Agent       | 防止 release/live/full parity claim                                         | safety checklist            | runtime rows remain open  |

---

## 给助理的直接指令

```text
当前不能判定完成。beta.85 Off loader proof 已经通过，但 CanaryOnly 和 AdditiveBatch1 的 current beta.85 enabled-mode proof 仍 pending；gameplay、Mod Settings UI、save-load、co-op、QA、handoff 都未完成。

现在进入 Enabled-Mode Runtime Proof Overnight Run：

1. 等 coordination pause lifted，只允许一个 execution lane。
2. 保持 Off proof，不再重复用历史 K1 CanaryOnly。
3. 跑 CanaryOnly beta.85 / v0.107.0 smoke，验证 4 event types / 6 registration calls。
4. 生成 enabled-mode-log-check.json 和 runtime-evidence-packet-check.json，必须 0 mismatch。
5. 跑 AdditiveBatch1 beta.85 / v0.107.0 smoke，验证 10 event types / 14 registration calls，或写 Hard Block。
6. QA/Red-Team subagent 独立复核 evidence。
7. 不启动 Batch 4c actual migration，只允许候选审查。
8. 不 claim release-ready、live-ready、full parity。
9. 只有 Green Stop 或 Hard Block Stop 才能停止。
```

---

# 9. 最终判断

**当前应“两者兼顾”：继续推进 current enabled-mode runtime proof，同时继续优化证据治理和 QA。**

最终一句话：

**当前最该优先解决的是 beta.85 CanaryOnly / AdditiveBatch1 的 current enabled-mode proof，因为 Off loader proof 已经成立，但 Sts1Events 测试模式、gameplay evidence 和 tester-readiness 仍没有当前 runtime 证据支撑。**
