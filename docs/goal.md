# goal.md — 30 天发布级开发计划

> 项目：`dev-the-spire` 现有 `Spire Plus / EZMicroBalance`，并行独立 `Future Peek / EZFuturePeek`。  
> 时间：2026-05-20 至 2026-06-19。  
> 总目标：把当前“源码/自动化通过的手动测试候选包”推进到“可审计、可回滚、可联机验证、可发布决策”的 Release Candidate。  
> 核心原则：**没有 live 证据就不关闭 issue；没有联机证据就不声称联机支持；网站/宣传设计没有 traceability matrix 就不能当作已实现承诺。**

---

## 0. 当前事实基线

本计划以 2026-05-20 当前仓库状态为基线。

当前状态不是 release-ready：

- `docs/issues.md` 当前目标是 `test-ready manual build, not release-ready`。
- `docs/review.md` 说明 latest validation 中 **No game was opened**，自动化验证通过不等于 live gameplay / save-load / co-op 通过。
- `docs/release-evidence-status.md` 中 fresh current-package loader smoke、clicked Ancient UI、Ancient reward gameplay、Vakuu victory/failure/death、save/load、A11 route traversal、Rootblight combat-end、disable-mod gameplay、co-op disposition 仍为 pending。
- `docs/patch-inventory.md` 显示 Harmony patch 总量很大，且有高风险 lifecycle / room / run / lobby / multiplayer patch；发布前必须做实机验证和回归矩阵。
- `Future Peek` 已作为 `EZFuturePeek` 独立项目存在，但文档明确是 source/test ready only，Crystal Sphere 和 transform preview 仍需 live proof。
- `website/` 当前不是活跃 release surface，旧网站草稿已被删除并只在本地 `.tools/archive/local-website-preview-*` 快照中保留。网站里出现过的设计必须重新整理成正式 spec，不能直接作为“已实现”宣传。

本月计划的输出不是“多写功能”，而是：

1. 把所有设计变成可测试的 spec。
2. 把所有 spec 绑定到源码证据、实现、自动化 guard、live proof、联机 proof。
3. 把高风险 patch 解耦成可维护边界。
4. 完成缺口修复。
5. 完成单机、存读档、失败路径、联机矩阵。
6. 最终给出清晰发布判断：发布 / 延期 / 降级 / 关闭功能。

---

## 1. 一个月最终 Definition of Done

30 天结束时，必须满足下面任一结果。

### 1.1 理想结果：Release Candidate 可发布

满足全部条件：

- `Spire Plus / EZMicroBalance` 当前包有 fresh loader smoke：
  - 当前 SavedSpireField 数量和源码一致；
  - 只启用 BaseLib + Spire Plus；
  - `godot.log` clean；
  - 主菜单加载成功；
  - Mod Settings 可见。
- 每个网站/设计文档承诺的功能都在 traceability matrix 中有状态：
  - implemented + tested；或
  - intentionally excluded with release note；或
  - gated and not advertised。
- Ancient reward rebalance、Urda、Morvi、Lotha、Vakuu、Ascension A11-A20、Rootblight、Future Peek 的范围全部有 spec、source evidence、自动化 guard、manual proof。
- 联机支持完成：
  - 双客户端 host/join 测试；
  - A11-A20 host multiplayer 选择和实际战斗行为；
  - Ancient choices / reward state / Root Eyes / Rooted Route / Rootblight ownership 不 desync；
  - A20 double boss / King Brand 若发布则必须 live proof；若不能完整支持，必须不能声称完整联机支持。
- Save/load 完成：
  - Urda、Root Eyes、Seed Bank、Morvi state、Lotha Death Reprieve、Vakuu child combat、Rootblight 均有实际存读档证据。
- Vakuu fight 完成：
  - victory return 无黑屏；
  - reward choice 正常；
  - failure/death 不破坏 room/reward/combat state；
  - active/pre-finished save-load 通过；
  - co-op 通过，或明确不进发布。
- `GOV-WIP-SPLIT` 关闭：
  - 工作树按 bounded context 拆分；
  - 每个大改动都有 reviewable commit；
  - patch inventory 重新生成；
  - release evidence manifest 通过。
- `EZFuturePeek` 独立发布判断完成：
  - Crystal Sphere live proof；
  - transform preview live proof；
  - 是否 `affects_gameplay=true` 完成产品决策；
  - 不与 `EZMicroBalance` 混入。

### 1.2 可接受结果：延期但状态完全清楚

如果某些高风险内容未能证明，必须做到：

- 不能发布为 full release。
- 生成 `docs/release-blockers-final.md`，逐条列出阻塞项、剩余证据、下一步。
- 所有未证明功能必须：
  - gated；或
  - hidden；或
  - 从宣传/网站/README 中移除；或
  - 明确标为 unsupported。
- 仍可输出 `manual-test build`，但不能使用 `release-ready`、`full multiplayer support`、`feature complete` 等字眼。

---

## 2. 核心 Approach

本月采用“Spec → Source Research → Architecture Boundary → Implementation → Guard → Live Proof → Release Decision”的流水线。每一项功能都必须过同样的门。

### 2.1 每个功能必须有一个 Spec

每个 spec 使用同一模板，放入：

```text
docs/specs/<feature-id>.md
```

模板：

```markdown
# <Feature Name> Spec

## Player Promise
玩家看到什么、能做什么、结果是什么。

## Non-goals
明确不做什么，防止范围漂移。

## Source Evidence
- 游戏源码路径：source code/src/Core/...
- 当前 Mod 源码路径：EZMicroBalanceCode/... 或 EZFuturePeekCode/...
- 关键方法、状态、RNG、save/load、network surface。

## Implementation Contract
- 数据模型
- 服务边界
- Harmony patch 边界
- UI 边界
- RNG 边界
- multiplayer authority
- save/load contract

## Acceptance Criteria
- 自动化 guard
- manual single-player proof
- save/load proof
- failure/death proof
- co-op proof
- log proof
- screenshot proof

## Rollback / Gate
- env var
- config toggle
- disable path
- release-note fallback
```

### 2.2 所有源码调研先于编码

编码前必须调研：

- `source code/src/Core/**` 中对应 vanilla 逻辑；
- BaseLib / template API；
- 当前 Mod 代码；
- 当前测试和 guard；
- 当前 issue / review / release evidence。

输出：

```text
docs/source-research/<feature-id>-source-evidence.md
```

每个 evidence 文件只记录：

- 类名；
- 方法名；
- 字段名；
- 状态流；
- 风险；
- 不复制大段反编译源码。

### 2.3 先解耦，再修复，再扩展

禁止在大型 monolithic patch 中继续堆逻辑。每个高风险功能必须拆成：

```text
Patches/        只做入口和极薄转发
Services/       业务逻辑
State/          saved/transient state
Models/         marker/card/relic/power models
UI/             只做 UI 表现
Diagnostics/    日志和证据
Tests/          source guards / artifact guards
```

规则：

- UI patch 不能改变 gameplay 状态。
- gameplay service 不能直接依赖 Godot 节点。
- preview 不能消耗真实 RNG。
- 联机 gameplay 状态不能只存在本地 static 字段中。
- save/load 敏感功能不能只靠 transient state。
- 每个 Harmony patch 必须在 patch inventory 中有 owner / risk。

### 2.4 发布前必须完成 Evidence Loop

每个功能关闭前必须有：

```text
source evidence
+ automated guard
+ live manual proof
+ save/load proof if stateful
+ co-op proof if gameplay-relevant
+ release evidence row
```

Source review alone 不关闭 issue。

---

## 3. Subagent 分工

本月使用 subagent，但每个 subagent 的输出必须是文件、patch、测试或 evidence，不能只给口头建议。

### 3.1 Product Spec Curator

职责：

- 收集网站草稿、现有 docs/features、README、handoff、archived design；
- 做 `docs/specs/release-traceability-matrix.md`；
- 标记每个设计：implemented / partial / hidden / not in release / needs owner decision。

输出：

```text
docs/specs/release-traceability-matrix.md
docs/specs/website-claim-audit.md
docs/specs/release-scope-v1.md
```

关闭条件：

- 网站或宣传上任何一句功能承诺都有对应 spec 或被删掉。

### 3.2 Source Archaeologist

职责：

- 调研 vanilla 源码；
- 记录 room/event/reward/save/multiplayer/RNG 源码路径；
- 给每个高风险功能列出不可碰路径。

输出：

```text
docs/source-research/ancient-events.md
docs/source-research/vakuu-combat-room.md
docs/source-research/run-save-load.md
docs/source-research/multiplayer-lobby-run-state.md
docs/source-research/reward-card-rng.md
docs/source-research/future-peek-rng.md
```

关闭条件：

- 每个 P0/P1 patch 的 source evidence 都能追溯到 vanilla 源码。

### 3.3 Clean Code Architect

职责：

- 检查 137 个 Harmony patch 的 owner/risk；
- 重构大 service；
- 保证 feature 边界清晰；
- 防止 UI / gameplay / save / network 混在一起。

输出：

```text
docs/architecture/bounded-contexts.md
docs/architecture/patch-boundaries.md
docs/architecture/save-state-contracts.md
```

关闭条件：

- patch inventory 更新；
- 高风险 patch 都有薄入口和 service owner；
- 工作树可拆 commit。

### 3.4 Gameplay Feature Implementer

职责：

- Urda / Morvi / Lotha / Vakuu / Ancient reward rebalance；
- 修复 live/manual 中发现的 gameplay bug；
- 保持所有功能 player-facing 文本真实。

输出：

- feature code；
- guard tests；
- manual rows 更新。

关闭条件：

- 所有 Ancient 选择实机通过；
- reward visible；
- hover readable；
- no black screen。

### 3.5 Ascension Implementer

职责：

- A11-A20 所有设计落地；
- A11 map traversal；
- A12 Firemarked Elite；
- A13 Fission；
- A14/A15/A18 Rootblight；
- A16 Banner；
- A17 Deep Branch；
- A19 Royal Seal；
- A20 King Brand / double boss / courtyard。

关闭条件：

- 单机和联机都有 runbook evidence；
- A20 不再只是 downgrade warning，除非明确从发布范围移除。

### 3.6 Multiplayer / Co-op Engineer

职责：

- 设计 host authoritative contract；
- 确认哪些状态同步、哪些只本地显示；
- 消除本地-only gameplay state；
- 实施 two-client runbook。

输出：

```text
docs/specs/multiplayer-contract.md
docs/features/ascension-11-20/multiplayer-test-runbook.md 更新
docs/features/ancient-expansion-v2.2/multiplayer-test-runbook.md
```

关闭条件：

- 双客户端日志和截图；
- 无 desync；
- 若 feature 不支持 co-op，必须 gated 或 release-note 标明 unsupported。

### 3.7 Save/Load Reliability Engineer

职责：

- 检查 SavedSpireField、deck mirror、combat room serialization；
- 设计 save/load matrix；
- 专攻 Root Eyes、Seed Bank、Morvi、Lotha Death Reprieve、Vakuu child combat、Rootblight。

关闭条件：

- 每个 save/load row 有 live evidence；
- 无 orphan transient state；
- 读档后 UI、relic、marker、combat state 一致。

### 3.8 Future Peek Engineer

职责：

- 完成 `EZFuturePeek` 独立发布标准；
- Crystal Sphere 只改 UI，不触发 reveal/reward；
- Transform preview 不消耗真实 RNG，不创建真实 card；
- 评估 `affects_gameplay` 是否应设为 true；
- 明确联机公平性策略。

关闭条件：

- live proof：Crystal Sphere toggle；
- live proof：transform result matches preview；
- 单卡、多卡、战斗、非战斗、取消/重开都通过；
- 不混入 `EZMicroBalance`。

### 3.9 QA Automation Engineer

职责：

- 扩充 guard tests；
- 保证 active source manifest 不自证；
- 补 package/artifact/hash/patch inventory tests；
- 保证 release evidence verifier 严格。

关闭条件：

- default tests；
- release artifact tests；
- full local CI；
- GitHub self-hosted lane first run。

### 3.10 UI / Localization / Art Reviewer

职责：

- 检查 clicked UI；
- 检查 English / zhs；
- 检查 hover 不溢出、不遮挡；
- 检查 art path、power icon、card/relic asset 区分。

关闭条件：

- clicked screenshots；
- hover screenshots；
- zhs/eng 都可读；
- no mojibake；
- no placeholder / generic temporary。

### 3.11 Release Engineer

职责：

- package；
- hash；
- release evidence manifest；
- zip parity；
- install instructions；
- final go/no-go。

关闭条件：

- `verify-spire-plus-release-evidence.ps1` 通过；
- clean worktree；
- commit；
- release note 不夸大。

### 3.12 Red-Team Reviewer

职责：

- 每周末攻击当前实现；
- 重点攻击 save/load、death/failure、co-op desync、RNG preview 污染、UI reentry、old run restore、mod disabled。

关闭条件：

- 每周输出 `docs/reviews/red-team-week-N.md`；
- P0/P1 问题进入 issue；
- 不允许 source-only review 关闭 runtime rows。

---

## 4. 30 天日程

## Week 1：Spec 冻结、源码调研、发布边界澄清

目标：**不急着写代码，先把“到底要发布什么”定死。**

### Day 1 — Baseline Freeze

任务：

- 记录当前 HEAD、package hash、test results、patch inventory。
- 跑：

```powershell
git status --short --branch
git log -1 --oneline --decorate
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet build EZFuturePeek.sln
dotnet test EZFuturePeek.sln --no-build
```

产出：

```text
docs/month-plan/baseline-2026-05-20.md
```

验收：

- 当前 blocker 全部列入矩阵；
- 不允许把历史 loader smoke 当 current loader proof。

### Day 2 — Website / Design Claim Audit

任务：

- 恢复或读取 `.tools/archive/local-website-preview-*` 中网站草稿。
- 提取网站上所有功能承诺：Ancient、Ascension、Rootblight、Vakuu、Future Peek、联机、视觉、发布说明。
- 建立 traceability matrix。

产出：

```text
docs/specs/website-claim-audit.md
docs/specs/release-traceability-matrix.md
```

验收：

- 每条 claim 都有状态：
  - implemented；
  - partial；
  - hidden；
  - not in release；
  - needs owner decision。

### Day 3 — Spec Freeze

任务：

- 写 `docs/specs/release-scope-v1.md`。
- 规定本月发布范围：
  - Spire Plus release candidate；
  - Future Peek 是否同月发布；
  - 网站是否恢复；
  - 联机是否必须完整支持。

必须做出产品决策：

- A20 co-op 如果要发布，必须完整证明；否则不能宣传 full co-op。
- Vakuu fight 如果不能 live 证明，继续 hidden-by-default。
- Future Peek 如果影响玩家决策，要评估 `affects_gameplay=true`。

产出：

```text
docs/specs/release-scope-v1.md
```

### Day 4 — Vanilla Source Research: Run / Room / Event / Reward

任务：

- 调研 `source code/src/Core/`：
  - RunManager；
  - EventRoom；
  - CombatRoom；
  - RewardsScreen；
  - CardReward；
  - RelicReward；
  - AncientEventModel；
  - SaveManager。

产出：

```text
docs/source-research/run-room-event-reward.md
```

验收：

- Vakuu child combat、Ancient reward、Root Eyes room routing 都有 source-backed flow 图。

### Day 5 — Vanilla Source Research: Multiplayer / Save / RNG

任务：

- 调研：
  - StartRunLobby；
  - NetGameType；
  - multiplayer run start；
  - ModelDb hash / mod mismatch；
  - SavedSpireField；
  - serializable rooms；
  - RNG sets。

产出：

```text
docs/source-research/multiplayer-save-rng.md
```

验收：

- multiplayer host authority 的未知点全部列出；
- RNG preview 的安全边界写清。

### Day 6 — Architecture Boundary Design

任务：

- 设计 bounded contexts：
  - AncientRewardRebalance；
  - AncientExpansionUrda；
  - AncientExpansionMorvi；
  - AncientExpansionLotha；
  - AncientExpansionVakuu；
  - AscensionCore；
  - RootDeck；
  - FuturePeek；
  - ReleaseEvidence。

产出：

```text
docs/architecture/bounded-contexts.md
docs/architecture/patch-boundaries.md
```

验收：

- 每个高风险 Harmony patch 有 owner；
- 每个 patch 只做转发，业务逻辑进 service。

### Day 7 — Week 1 Red-Team Review

任务：

- Red-Team Reviewer 审核 spec 和 source evidence。
- 标记 P0/P1 风险。
- 决定 Week 2 代码优先级。

产出：

```text
docs/reviews/red-team-week-1.md
```

验收：

- 若 spec 不完整，不进入 Week 2 大规模编码。

---

## Week 2：实现缺口、解耦重构、自动化 guard

目标：**把 source-known blocker 修完，把架构拆到可 review。**

### Day 8 — GOV-WIP-SPLIT / Commit Boundary

任务：

- 按 bounded context 拆工作树。
- 建立 commit plan：
  1. docs/specs/source-research；
  2. governance/tests；
  3. Future Peek；
  4. Ancient fixes；
  5. Ascension fixes；
  6. release docs/package。

产出：

```text
docs/month-plan/commit-boundaries.md
```

验收：

- `GOV-WIP-SPLIT` 有可执行拆分路径。

### Day 9 — Urda: Root Eyes / Seed Bank Source Completion

任务：

- 按 spec 审核 Root Eyes：
  - map selection；
  - future reachable Monster/Unknown/Elite；
  - stale preview refund；
  - save/load marker restore；
  - co-op host authority。
- 审核 Seed Bank：
  - relic hover；
  - extraction；
  - boss transition；
  - save/load。

产出：

- code fixes；
- guard tests；
- manual rows 更新。

验收：

- source tests 覆盖所有 state path；
- co-op contract 明确。

### Day 10 — Morvi / Lotha Source Completion

任务：

- Morvi：Forbidden Loan、Misprint Press、Red Ink、Overdue Library、Blueprint Proof、Open Book、Debt Settlement。
- Lotha：Single Sentence、Death Reprieve、Public Evidence、Mirror Rebuttal、combat lifecycle、freeze report。
- 检查是否存在只靠 local transient state 的 gameplay path。

产出：

- code fixes；
- `docs/source-research/morvi-lotha-state.md`。

验收：

- save/load contract 明确；
- card-play freeze 有可验证路径。

### Day 11 — Vakuu Fight Hardening

任务：

- 审核 child combat flow；
- 审核 victory no-reward resume；
- 审核 fallback map exit；
- 审核 failure/death；
- 审核 pre-finished save/load；
- 审核 co-op。

产出：

```text
docs/specs/vakuu-fight-spec.md
docs/source-research/vakuu-combat-room.md
```

验收：

- 代码不再依赖不安全 active ParentEventId；
- 所有 fallback 都有 log。

### Day 12 — Ascension A11-A20 Completion Pass

任务：

- A11 natural traversal；
- A12 Firemarked Elite；
- A13 Fission reward；
- A14/A15/A18 Rootblight；
- A16 Banner；
- A17 Deep Branch；
- A19 Royal Seal；
- A20 King Brand / double boss / courtyard。

产出：

```text
docs/specs/ascension-11-20-release-spec.md
```

验收：

- 每个 Ascension level 有：single-player proof row + co-op proof row + save/load row。

### Day 13 — Future Peek Stabilization

任务：

- Crystal Sphere：
  - 只改 `%ScryMask` opacity；
  - OnMinigameFinished restore；
  - UI 不 reentry 泄漏。
- Transform preview：
  - RNG source contexts；
  - stale context cleanup；
  - 多卡顺序；
  - Astrolabe upgraded preview；
  - 不创建真实卡。
- 评估 `affects_gameplay`。

产出：

```text
docs/specs/future-peek-release-spec.md
```

验收：

- Future Peek 独立，不污染 Spire Plus；
- live test checklist ready。

### Day 14 — Week 2 Automated Gate + Red-Team

任务：

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet build EZFuturePeek.sln
dotnet test EZFuturePeek.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
dotnet format EZFuturePeek.sln --verify-no-changes --no-restore
git diff --check
.\scripts\generate-patch-inventory.ps1
.\scripts\validate-repository-hygiene.ps1
```

产出：

```text
docs/reviews/red-team-week-2.md
docs/month-plan/week-2-validation.md
```

验收：

- source P0/P1 blocker 为 0；
- runtime blocker 仍不得关闭。

---

## Week 3：Live 单机、Save/Load、失败路径、Future Peek 实机

目标：**把“源码正确”变成“游戏里真的正确”。**

### Day 15 — Fresh Current-Package Loader Smoke

任务：

- 重新 publish/package。
- 正常 Steam-client 启动。
- 只启用 BaseLib + Spire Plus。
- 检查 current SavedSpireFields 数量。
- `godot.log` audit。

产出：

```text
.tools/runtime-evidence/live-spire-plus-session-<date>/
docs/month-plan/fresh-loader-smoke.md
```

验收：

- release-evidence-status 中 Fresh current-package loader smoke 可关闭。

### Day 16 — Clicked Ancient UI Proof

任务：

- Urda、Morvi、Lotha、Vakuu normal、Vakuu fight。
- 每个截图必须包含：
  - event background；
  - option art；
  - dialogue；
  - hover；
  - relic marker visibility。

产出：

```text
.tools/runtime-evidence/ancient-ui-click-smoke-<date>/
docs/month-plan/ancient-ui-proof.md
```

验收：

- clicked UI row 关闭或列出修复项。

### Day 17 — Ancient Gameplay Matrix

任务：

- Urda：Root Eyes、Seed Bank、Trial Branch、Seedbed、Humus Pact、Moss Map、Rooted Route、After Rain、Molting。
- Morvi：八个 blessing。
- Lotha：八个 blessing。
- Vanilla rebalance：Cape、Gem、Choker 等关键 Ancient reward。

产出：

```text
docs/month-plan/ancient-gameplay-proof.md
```

验收：

- 每个 reward 有 result note、screenshot/log。

### Day 18 — Save/Load Matrix

任务：

- 每个 stateful feature：保存、退出、重进、继续操作。
- 重点：
  - Root Eyes saved preview；
  - Seed Bank storage；
  - Morvi debt/open-book/blueprint；
  - Lotha Death Reprieve；
  - Vakuu active/pre-finished child combat；
  - Rootblight deck state。

产出：

```text
docs/month-plan/save-load-proof.md
```

验收：

- save/load row 可关闭；或新 issue 进入 Week 4 修复。

### Day 19 — Vakuu Victory / Failure / Death

任务：

- enabled gate；
- start fight；
- victory return；
- reward choice；
- no black screen；
- failure/death；
- log audit。

产出：

```text
docs/month-plan/vakuu-live-proof.md
```

验收：

- 如果失败，Vakuu 保持 hidden-by-default，不能 release 公开。

### Day 20 — Ascension Single-Player Matrix

任务：

- A11 natural route traversal；
- A12 Firemarked Elite；
- A13 Fission；
- A16 Banner；
- A17 Deep Branch；
- A19 Royal Seal；
- A20 double boss / courtyard；
- Rootblight combat-end。

产出：

```text
docs/month-plan/ascension-single-player-proof.md
```

验收：

- A11-A20 不再只靠 source guards。

### Day 21 — Future Peek Live Proof

任务：

- Crystal Sphere：按钮出现、toggle on/off、no charges spent、no reward、minigame finish restore。
- Transform preview：
  - single-card；
  - multi-card；
  - combat transform；
  - non-combat transform；
  - cancel/reopen；
  - actual result matches preview；
  - RNG 不推进。

产出：

```text
docs/month-plan/future-peek-live-proof.md
```

验收：

- 决定 Future Peek 是否可单独 beta；
- 决定 `affects_gameplay`。

---

## Week 4：联机、回归、发布证据、最终审核

目标：**两客户端验证、清理、打包、发布决策。**

### Day 22 — Multiplayer Contract Implementation Review

任务：

- 审核所有 gameplay-relevant state：
  - host authoritative；
  - sync path；
  - save path；
  - RNG path；
  - UI-only path。
- 若发现 local-only gameplay state，必须修复或 gate。

产出：

```text
docs/specs/multiplayer-contract.md
```

验收：

- co-op runbook 可执行。

### Day 23 — Two-Client Co-op: Lobby / Start / A11-A20

任务：

- host/join；
- A11-A20 selection；
- A20 warning 是否仍存在；
- start run；
- map traversal；
- combat modifiers。

产出：

```text
.tools/runtime-evidence/co-op-ascension-<date>/
docs/month-plan/co-op-ascension-proof.md
```

验收：

- 如果 A20 still downgraded，则不能宣传 full A20 co-op。

### Day 24 — Two-Client Co-op: Ancient / Root Eyes / Rooted Route

任务：

- Ancient event 出现与选择；
- reward state；
- Root Eyes map preview；
- Rooted Route map movement；
- marker/hover 是否同步或正确本地化；
- 不 desync。

产出：

```text
.tools/runtime-evidence/co-op-ancients-<date>/
docs/month-plan/co-op-ancient-proof.md
```

验收：

- Ancient co-op disposition 关闭或明确 unsupported。

### Day 25 — Two-Client Co-op: Rootblight / Save / Death

任务：

- Rootblight ownership；
- combat-end behavior；
- save/load in co-op；
- death/failure paths；
- reconnect if applicable。

产出：

```text
docs/month-plan/co-op-save-death-proof.md
```

验收：

- co-op row 可关闭；或 release note 明确不支持。

### Day 26 — Fix Week 3/4 Findings

任务：

- 只修 live evidence 中暴露的问题。
- 不添加新设计。
- 每个修复必须有：
  - source evidence；
  - test；
  - retest。

产出：

```text
docs/month-plan/live-finding-fixes.md
```

验收：

- P0/P1 live bug 清零，或功能 gate。

### Day 27 — Clean Code Final Pass

任务：

- 移除 dead code；
- 拆分 oversized services；
- 检查 patch inventory；
- 检查 docs bloat；
- 检查 legacy / website / archive 边界；
- 检查 no large decompiled source copied。

命令：

```powershell
.\scripts\generate-patch-inventory.ps1
.\scripts\validate-repository-hygiene.ps1
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
dotnet format EZFuturePeek.sln --verify-no-changes --no-restore
git diff --check
```

产出：

```text
docs/reviews/clean-code-final-review.md
```

验收：

- 架构 reviewer 签字；
- GOV-WIP-SPLIT 关闭或解释为何仍阻塞发布。

### Day 28 — Release Package RC

任务：

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet publish EZMicroBalance.sln
.\scripts\package-spire-plus.ps1
$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build
Remove-Item Env:\EZMB_RUN_RELEASE_ARTIFACT_TESTS

dotnet build EZFuturePeek.sln
dotnet test EZFuturePeek.sln --no-build
dotnet publish EZFuturePeek.sln
```

产出：

```text
docs/month-plan/rc-package-report.md
```

验收：

- hashes 更新；
- package parity；
- install folder 正确；
- no stale hash claims。

### Day 29 — Release Evidence Manifest

任务：

- 填写 release evidence manifest。
- 每个 passed row 必须有：
  - command.txt；
  - screenshot/log/result notes；
  - clean godot-log-audit.json；
  - row kind 正确；
  - evidence dir 在 root 内。

命令：

```powershell
.\scripts\verify-spire-plus-release-evidence.ps1
```

产出：

```text
docs/release-evidence-status.md 更新
docs/month-plan/release-evidence-final.md
```

验收：

- verifier 通过；
- 若使用 `-AllowDeferred`，必须有 owner-approved release note。

### Day 30 — Final Go / No-Go Review

任务：

- 所有 subagent 提交 final report。
- Red-Team Reviewer 最后攻击。
- Release Engineer 给出发布判断。
- Product Spec Curator 确认网站/README/宣传无夸大。

产出：

```text
docs/reviews/final-release-readiness-review.md
docs/release-notes.md
```

最终判断只能是下面之一：

```text
GO: Release Candidate can be published.
NO-GO: release blocked, manual-test build only.
GO-WITH-GATES: release only with disabled/hidden/unsupported features clearly documented.
```

---

## 5. Feature-by-Feature 发布门槛

## 5.1 Ancient Reward Rebalance

必须证明：

- Distinguished Cape 数值、支付门槛、同池替换；
- Prismatic Gem 每第二个标准 card reward 全 off-color，跳过 custom/filtered/colorless/no-pool/elite/boss/event；
- Velvet Choker soft limit；
- zhs number formatting；
- save/load 后 relic state 正确；
- co-op 下 reward state 不 desync。

## 5.2 Urda

必须证明：

- 十个 blessing 都能选择；
- Root Eyes 选择未来 Monster/Unknown/Elite node；
- preview 与实际进入一致；
- stale preview refund；
- Seed Bank store / hover / extract / boss transition；
- Trial Branch 3 combat prove/remove；
- Rooted Route 路线和奖励；
- save/load；
- co-op。

## 5.3 Morvi

必须证明：

- 八个 blessing 都能选择；
- Forbidden Loan eligibility；
- Misprint Press replay；
- Red Ink debt；
- Overdue Library pages；
- Blueprint Proof timing；
- Open Book restore；
- Debt Settlement 数值；
- save/load；
- co-op。

## 5.4 Lotha

必须证明：

- 八个 blessing 都能选择；
- Single Sentence；
- Death Reprieve；
- Public Evidence；
- Mirror Rebuttal；
- combat lifecycle reset；
- no freeze；
- lethal path；
- save/load；
- co-op。

## 5.5 Vakuu

必须证明：

- hidden-by-default gate 正常；
- fight gate 正常；
- dedicated enemy/scene 正常；
- victory return 无黑屏；
- no-normal-reward resume；
- reward choices；
- failure/death path；
- save/load active/pre-finished；
- co-op。

若任一项失败，Vakuu fight 不进入公开发布。

## 5.6 Ascension A11-A20

必须证明：

- A11 map 宽度、路线、自然点击通关；
- A12 Firemarked Elite；
- A13 Fission；
- A14/A15/A18 Rootblight；
- A16 Banner Room；
- A17 Deep Branch；
- A19 Royal Seal；
- A20 double boss / King Brand / courtyard；
- save/load；
- co-op。

如果 A20 co-op 仍是 downgraded，则不能宣传完整 A20 co-op。

## 5.7 Future Peek

必须证明：

- `EZFuturePeek` 独立 manifest、DLL、PCK；
- 不修改 `EZMicroBalance` runtime folders；
- Crystal Sphere 只改 mask opacity；
- 不调用 ClearCell / RevealItem / CellClicked / AddReward；
- transform preview 不调用 GetReplacement / CreateRandomCardForTransform；
- preview 不推进真实 RNG；
- actual result matches preview；
- 决定 `affects_gameplay`。

## 5.8 Website / Public Claims

必须做到：

- 若恢复网站：
  - website source 纳入 repo；
  - Pages workflow 修复；
  - 网站 build/smoke；
  - 网站功能文案与 release scope 一致。
- 若不恢复网站：
  - 不发布网站；
  - 不把网站旧草稿当成当前宣传；
  - docs 说明 website draft archived only。

---

## 6. 每日工作节奏

每天必须输出：

```text
1. 今日目标
2. 修改文件
3. 源码证据
4. 自动化验证
5. live 验证状态
6. 新 blocker
7. 明日计划
```

每天必须避免：

- 没 spec 就写大功能；
- 没源码证据就 patch lifecycle；
- 用 source review 关闭 live rows；
- 宣称 co-op 支持但没有 two-client evidence；
- 增加未 gate 的高风险功能；
- 在 UI patch 写 gameplay；
- 在 gameplay service 直接依赖 Godot node；
- 预览功能消耗真实 RNG；
- 把网站旧草稿当发布文案。

---

## 7. Issue 关闭规则

任何 issue 关闭必须满足：

```text
Spec exists
Source evidence exists
Implementation exists
Guard test exists
Manual proof exists if live row
Save/load proof exists if stateful
Co-op proof exists if gameplay-relevant
Release evidence row updated
Docs updated
```

禁止关闭：

- “source says should work”；
- “tests passed”；
- “no blocker known”；
- “暂时看起来没问题”；
- “联机应该一样”。

---

## 8. Release Go/No-Go 表

| Area | GO 标准 | NO-GO 标准 |
| --- | --- | --- |
| Loader | fresh current-package smoke clean | 只有历史 22-field / 16-field log |
| UI | clicked screenshots + hover proof | 只有 PCK resource load |
| Gameplay | full manual matrix pass | 只有 source guard |
| Save/load | all stateful rows pass | deck mirror 未 live 证明 |
| Vakuu | victory/failure/death/save/co-op pass | hidden fight 未证明 |
| Ascension | A11-A20 single + co-op pass | A20 still downgraded but advertised |
| Co-op | two-client logs clean | 只有 host selection patch |
| Future Peek | live result matches preview | source/test only |
| Website | claims match implementation | 旧草稿未审核 |
| Governance | worktree split + clean commits | GOV-WIP-SPLIT open |

---

## 9. 最终交付物清单

月末必须有：

```text
docs/specs/release-scope-v1.md
docs/specs/release-traceability-matrix.md
docs/specs/website-claim-audit.md
docs/specs/multiplayer-contract.md
docs/specs/future-peek-release-spec.md
docs/source-research/*.md
docs/architecture/bounded-contexts.md
docs/architecture/patch-boundaries.md
docs/architecture/save-state-contracts.md
docs/reviews/red-team-week-1.md
docs/reviews/red-team-week-2.md
docs/reviews/clean-code-final-review.md
docs/reviews/final-release-readiness-review.md
docs/month-plan/*.md
docs/release-evidence-status.md
docs/release-notes.md
```

以及：

```text
fresh loader smoke evidence
clicked Ancient UI evidence
Ancient gameplay evidence
save/load evidence
Vakuu evidence
Ascension evidence
co-op evidence
Future Peek evidence
release evidence manifest
verified package hashes
clean commit plan
```

---

## 10. 本月最终原则

这个月的成功不是“写更多代码”，而是：

- 每个设计都有 spec；
- 每个 spec 都能追到源码；
- 每个实现都有 guard；
- 每个 gameplay claim 都有 live proof；
- 每个联机 claim 都有 two-client proof；
- 每个未证明功能都被 gated 或从发布文案移除；
- 每个 high-risk patch 都有 owner 和边界；
- 项目能被 review、回滚、发布。

最终只有一个标准：

> **玩家看到的承诺、源码实现、测试证据、联机行为、发布文案必须完全一致。**
