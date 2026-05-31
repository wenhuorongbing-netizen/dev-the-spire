# 从头整理：当前项目上下文、严格审核、下月开发规范与 Overnight Run

下面这份可以直接作为“新窗口继承上下文包”。我会先把当前事实源、目标、已完成、未完成、风险、下一步路线全部讲清楚，然后给出 **Monthly Dev Spec + 必须持续到完成的 Overnight Run**。

---

## 0. 当前最重要的结论

**不能判定整个任务完成。**
当前应判定为：

```text
Phase 0/1 structural refactor: 已完成
Evidence hardening / no-game validation: 阶段性完成
RitsuLib patch migration source-level guard: 阶段性完成
RitsuLib runtime proof: 未完成 / blocked
Sts1Events default safety: 已完成 source-level gate
Sts1Events playable content: 未完成
Architecture skeletons: 已创建
Architecture skeletons 接入真实业务: 大多未完成
Release-ready / live-ready: 不可以 claim
下一步决策: 优化为主，有限推进
```

当前项目最大的 blocker 不是代码能不能 build，而是：

> **RitsuLib runtime smoke 没过，STS2-RitsuLib 本地未安装或未验证，所以 Batch 4c、高风险 patch migration、release-ready、live-ready 都不能继续 claim。**

`runtime-smoke-checklist.md` 明确写当前状态是 **BLOCKED**：`<GameRoot>\mods\STS2-RitsuLib` 未安装，当前 Spire Plus package 声明依赖 `STS2-RitsuLib >= 0.3.2`，runtime smoke 不能继续，Batch 4c blocked。

---

# 1. 事实源与版本基线

## 1.1 上传上下文里的任务目标

这次上传文件定义的目标是：

```text
Implement migration.md and refactor.md:
RitsuLib Runtime Proof + Test Truth Reconciliation + Architecture Foundation Overnight Run
7 phases:
1. validation truth
2. runtime smoke
3. Sts1Events governance
4. FeatureRegistry hardening
5. Architecture canary integration
6. State / Death / Multiplayer foundation
7. docs update

Not Batch 4c.
No new gameplay.
No release-ready claims.
```

同时上传上下文要求：

* Green stop only when all phases done + build/test/format/diff-check clean。
* Hard block stop only with blocker report。
* No soft stop。
* No false green。
* Runtime smoke 必须捕获 `SPIREPLUS_STS1_EVENT_MODE=Off` 和 `=CanaryOnly` 的真实 game logs。
* 不许启动 Batch 4c。
* 不许迁移 high-risk patches。
* 不许加新 gameplay。
* 测试数字必须来自 raw test logs。
* `canary_only_calls` 不是 additive with `register_all_calls`。
* 每次 test 前必须 `dotnet clean && dotnet build`，因为本 session 曾经被 incremental DLL cache 误导。

这就是当前最高优先级工作约束。

---

## 1.2 当前 repo 文档里的最新状态

仓库当前 `docs/features/ritsulib-migration/next-overnight-run.md` 记录：

* HEAD: `aed2a498`
* Build: 0 errors, 92 warnings
* Tests: 444 passed, 0 failed, 21 skipped, 465 total
* 25 patches migrated
* 142 raw Harmony remaining
* Architecture canary integration complete
* DeathProtectionService + MultiplayerPolicy stubs created
* Runtime smoke: **BLOCKED** because STS2-RitsuLib not installed

这比上传上下文中的 `d290598c`、428 pass 更晚。因此：

> **如果以当前 GitHub main 为准，应使用 `aed2a498 / 444 pass / 0 fail / 21 skip / 465 total / runtime blocked` 作为最新事实。**

但是上传文件也记录了本 session 里的关键事实：

* 当前 HEAD 曾是 `d290598c "debugging"`。
* working tree dirty。
* build: 0 errors, 93 warnings。
* test: 428 pass / 0 fail / 21 skip / 449 total。
* runtime smoke blocked because STS2-RitsuLib not installed locally。
* DeathProtectionService.cs / MultiplayerPolicy.cs untracked。
* next steps 是 Phase 2 runtime smoke, Phase 3 Sts1Events governance, Phase 4 FeatureRegistry hardening, Phase 5 Architecture canary integration, Phase 6 stubs guard tests, Phase 7 docs number update。

这说明项目在多轮 overnight 里不断推进，但**事实数字一直在变**。因此下一个助理必须先做 **validation truth reconciliation**，不能直接相信任何单行 summary。

---

# 2. 项目 Inventory

## 2.1 Project Type

这是一个 **Slay the Spire 2 mod workspace**，玩家可见名是 `Spire Plus`，技术 manifest id 保持 `EZMicroBalance`。项目是一个 C#/.NET/Godot/BaseLib/RitsuLib mod，不是普通 Web app。

## 2.2 Tech Stack

| 项                | 当前上下文                                                                        |
| ---------------- | ---------------------------------------------------------------------------- |
| Language         | C#                                                                           |
| Runtime          | .NET 9 / Godot .NET                                                          |
| Game target      | Slay the Spire 2 v0.106.1                                                    |
| Mod framework    | BaseLib v3.1.4                                                               |
| Patch frameworks | Harmony + RitsuLib ModPatcher hybrid                                         |
| RitsuLib         | STS2.RitsuLib 0.3.2 compile dependency; runtime install blocked / unverified |
| Test framework   | xUnit                                                                        |
| Build            | `dotnet build EZMicroBalance.sln`                                            |
| Test             | `dotnet test EZMicroBalance.sln`                                             |
| Format           | `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`          |
| Runtime proof    | Steam client launch + `godot.log`                                            |
| Key risk         | RitsuLib runtime proof missing                                               |

## 2.3 Main Directories / Modules

| Path                                            | Role                                                                                 |
| ----------------------------------------------- | ------------------------------------------------------------------------------------ |
| `EZMicroBalanceCode/Core/Features`              | feature registry, feature modules, bootstrap records                                 |
| `EZMicroBalanceCode/Core/Integrations/RitsuLib` | RitsuLib bootstrap and migrated patch registration                                   |
| `EZMicroBalanceCode/Core/Architecture`          | RewardPipeline, CardPlayContext, DeathProtectionService, MultiplayerPolicy skeletons |
| `EZMicroBalanceCode/Ancients`                   | Ancient reward rebalance and expansion features                                      |
| `EZMicroBalanceCode/Ascension`                  | A11-A20 map/combat/reward/selection systems                                          |
| `EZMicroBalanceCode/Preview`                    | Crystal Sphere / transform preview tools                                             |
| `EZMicroBalanceCode/Sts1Events`                 | StS1 event port prototype, feature-gated Off by default                              |
| `EZMicroBalance/localization`                   | EN/ZHS localization                                                                  |
| `tests/EZMicroBalance.Tests`                    | source guards, behavior guards, release artifact guards                              |
| `docs/features/ritsulib-migration`              | RitsuLib migration planning and runtime smoke                                        |
| `docs/goals`                                    | current long-running task specs / overnight specs                                    |
| `docs/architecture`                             | patch boundaries, save-state contracts, architecture specs                           |
| `scripts`                                       | validation and repo hygiene scripts                                                  |

---

# 3. Current Architecture Assessment

## 3.1 Current Architecture Pattern

当前项目是：

```text
Modular Monolith in progress
+ Patch-heavy Adapter Architecture
+ Hybrid Harmony/RitsuLib bootstrap
+ Source-guard-heavy test governance
+ Feature-gated prototype surface for Sts1Events
```

优点：

* 已经有 feature registry。
* Sts1Events 默认 Off，避免默认行为污染。
* Patch inventory 和 migration guard 越来越清晰。
* RitsuLib migration 使用 hybrid bootstrap，避免一次性迁移所有 patch。
* Source guard tests 很强，能防止文档漂移、patch double-apply、manifest/source mismatch。

缺点：

* runtime proof 明显不足。
* source-level green 经常被误写成 runtime/live green。
* 高风险 patch 仍多。
* Architecture skeletons 多，但接入真实业务少。
* Sts1Events 已经编译进项目，但 nullability warnings 很多。
* Docs 里 test count / warning count / patch count 多次漂移。

---

# 4. 严格逐项审核

## 4.1 RitsuLib dependency

**Observation:**
RitsuLib compile dependency 和 manifest dependency 已加入，monthly spec 也记录当前 hybrid bootstrap active：`ModPatcher.PatchAll()` for migrated patches, `Harmony.PatchAll()` for remaining raw patches。

**Inference:**
这是迁移基础完成，但不是 runtime-safe proof。

**Verdict:**
**PASS for source/compile dependency; FAIL/BLOCKED for runtime proof.**

---

## 4.2 Patch migration count

**Observation:**
当前 monthly spec 记录 25 patches migrated，142 raw Harmony patches remaining，22 high-risk / 35 medium / 85 low-risk。

**Observation:**
前一版 patch inventory 记录 migrated 25、raw remaining 142、high risk raw 22。

**Inference:**
Batch 4a/4b source-level closure 可以接受，但 Batch 4c 不能开始，因为 runtime smoke blocked。

**Verdict:**
**PASS for Batch 4a/4b source truth; Batch 4c BLOCKED.**

---

## 4.3 Double-patch guard

**Observation:**
monthly spec 说明 double-patch guard 的设计：migrated patch classes 在 `RitsuLib/` namespace，implement `IPatchMethod`；raw Harmony patches 保持 `[HarmonyPatch]`；guard tests 验证同一 class 不会两边都出现。

**Verdict:**
**PASS source-level.**

---

## 4.4 Validation truth

**Observation:**
上传上下文记录 raw clean-build count 为 `428 pass / 0 fail / 21 skip / 449 total`，并要求这是本 session 的 precedence。

**Observation:**
当前 repo 最新 next-overnight-run 记录为 `444 passed / 0 failed / 21 skipped / 465 total`。

**Inference:**
测试数量已继续增长，不能继续引用旧的 `387 pass`、`428 pass` 作为当前最终事实。必须每轮以 raw log 为准。

**Verdict:**
**PARTIAL PASS; truth reconciliation 必须作为下一轮第一步。**

---

## 4.5 Build warnings

**Observation:**
上传上下文明确：clean build 是 0 errors, 93 warnings，92 个 Sts1Events nullable，1 个 xUnit2009。

**Observation:**
最新 repo next-overnight-run 写 0 errors, 92 warnings。

**Inference:**
warnings 仍未被治理。不能写 “build clean = 0 warnings”，除非明确是 incremental build 或某一轮 raw log。

**Verdict:**
**OPEN. P1 warning governance required.**

---

## 4.6 Runtime smoke

**Observation:**
runtime smoke checklist 当前状态是 BLOCKED，因为 STS2-RitsuLib 没安装在 `<GameRoot>\mods\STS2-RitsuLib`，Batch 4c blocked until runtime smoke passes。

**Observation:**
next-overnight-run 也明确：Runtime smoke remains critical path blocker，Batch 4c cannot proceed until STS2-RitsuLib is installed and runtime smoke passes。

**Verdict:**
**FAIL / BLOCKED. This is the current P0.**

---

## 4.7 Sts1Events safety

**Observation:**
Sts1Events feature gate 默认 Off；CanaryOnly = 4 events；AdditiveAllDraft 和 ReplaceUnknownEventsPrototype 是 dev/debug-only。上传上下文也明确 Off / CanaryOnly safe，其他 unsafe/dev-only。

**Observation:**
monthly spec 记录 Sts1Events source compiles and is registered in feature registry with gate defaulting Off；四种 modes validated；Off 直接 return，CanaryOnly 4 events。

**Verdict:**
**PASS for default safety; FAIL/OPEN for content completeness and runtime proof.**

---

## 4.8 FeatureRegistry hardening

**Observation:**
上传上下文记录 `IFeatureModule` 已扩展 DisplayName、Category、DisableEnvKeys、ForceEnvKeys；`FeatureBootstrapRecord` 有 Id、DisplayName、Category、Gate、LiveStatus、FailureMessage、IsActive；`FeatureRegistry` 有 BootstrapRecords 和 LogFeatureSummary。

**Observation:**
migration.md 也记录 FeatureRegistry scaffold pass，但 dependencies、runtime diagnostics、LiveStatus 真实 gameplay availability、Disable/Force env key unified gate evaluation 仍不足。

**Verdict:**
**PASS for scaffold; governance not complete.**

---

## 4.9 UrdaStateCodec

**Observation:**
migration.md 记录 UrdaStateCodec 有 Decode / Encode / legacy minimum part count / legacy-current index handling / malformed fallback / semicolon wire format / sanitize。

**Observation:**
上传上下文记录已添加 15 behavioral tests，总计 33 UrdaStateCodec tests。

**Verdict:**
**PASS for codec scaffold and initial behavior tests; not full DataStore migration.**

---

## 4.10 Architecture skeletons

**Observation:**
RewardPipeline skeleton 有 RewardPhase、IRewardHandler、RewardPipelineContext、Register、Diagnose、HandlerCount、RegisteredPhases、ClearHandlers，并明确 diagnostics-only no behavior changes。

**Observation:**
CardPlayContext skeleton 有 ExtraPlayPolicy、MaxDepth、TryIncrementDepth、DecrementDepth、Reset、IsPowerFallback，但未接入实际 Lotha extra-play。

**Observation:**
DeathProtectionService 是 spec or stub，MultiplayerPolicy 是 taxonomy/stub，enforcement not done。

**Verdict:**
**PASS for skeleton; integration not done.**

---

## 4.11 Working tree / commit status

**Observation:**
上传上下文记录 current HEAD d290598c，working tree dirty，untracked DeathProtectionService.cs / MultiplayerPolicy.cs / Stubs dir。

**Inference:**
如果当前仍 dirty，则不能 Green Stop。需要么 commit/push after validation，要么 Hard Block Stop。

**Verdict:**
**UNKNOWN / must re-check with git status.**

---

# 5. 当前成熟度评分

| 维度       |         分数 | 理由                                                                           |
| -------- | ---------: | ---------------------------------------------------------------------------- |
| 架构清晰度    |       8/12 | Bounded contexts、FeatureRegistry、architecture skeleton 已有，但 runtime proof 缺失 |
| 模块边界     |       7/12 | Sts1Events gated，RitsuLib namespace 分离，但 raw Harmony 仍 142                   |
| 领域建模     |       6/10 | Reward/CardPlay/Death/Multi skeleton 出现，但未真实接入                               |
| 代码可读性    |       6/10 | source guard 强，但 Sts1Events warnings 多                                       |
| 可维护性     |       7/12 | tests 多，docs 多，但 truth drift 严重                                              |
| 可拓展性     |       7/10 | Feature modules 和 architecture skeleton 提升扩展能力                               |
| 可测试性     |       8/10 | 400+ tests，但偏 source-shape，runtime/manual 缺口大                                |
| CI/CD    |        5/8 | no-game validation 强，runtime smoke blocked                                   |
| 项目管理     |        6/8 | overnight specs/subagents/DoD 明确，但执行中 false-green 风险反复出现                     |
| 文档       |        4/5 | 文档覆盖很强，但数字漂移                                                                 |
| 稳定性/生产准备 |        1/3 | release-ready 不成立，runtime proof 缺失                                           |
| **总分**   | **66/100** | 工程治理提升明显，但 runtime 与 warning truth 是硬伤                                       |

---

# 6. Top 10 Highest Impact Issues

| Priority | Issue                                                       | Area                      | Evidence                                                              | Impact                     | Recommendation                      |
| -------- | ----------------------------------------------------------- | ------------------------- | --------------------------------------------------------------------- | -------------------------- | ----------------------------------- |
| P0       | Runtime smoke blocked                                       | RitsuLib / release safety | runtime checklist BLOCKED。                                            | 不能证明 RitsuLib runtime safe | 安装 STS2-RitsuLib，跑 Off/Canary smoke |
| P0       | Batch 4c blocked                                            | Migration                 | next overnight 明确 Batch 4c cannot proceed until runtime smoke passes。 | 继续迁 patch 风险过高             | 阻止 Batch 4c                         |
| P0       | Validation truth drift                                      | Testing / docs            | 上传上下文要求 raw logs 作为来源。                                                | false green                | 每轮 clean build + raw test log       |
| P1       | 92/93 build warnings                                        | Code quality              | 上传上下文记录 Sts1Events nullable warnings。                                 | 长期污染 build truth           | 建 warning matrix + issue            |
| P1       | Sts1Events AllDraft unsafe                                  | Prototype governance      | monthly spec says AllDraft includes unsafe TODOs。                     | tester 误用风险                | Keep dev-only, add risk table       |
| P1       | Architecture skeletons not integrated                       | Architecture              | skeleton docs say no behavior changes / not integrated。               | 只是文档/骨架                    | diagnostics-only canary integration |
| P1       | DeathProtection/Multiplayer stubs not fully tracked/guarded | Architecture              | migration says code not done/enforcement not done。                    | 后续高风险功能缺治理                 | Track + guard tests + active matrix |
| P1       | Independent QA not started                                  | Process                   | 上传上下文要求 QA/Red-Team independent review。                               | 自审风险                       | 启动独立 QA subagent                    |
| P2       | FeatureRegistry metadata incomplete                         | Governance                | dependencies/runtime diagnostics/live status incomplete。              | feature state 不够可信         | Add dependency + runtime export     |
| P2       | Docs overloaded / multiple specs                            | Docs                      | migration/refactor/monthly/overnight 都在写状态                            | 新窗口难继承                     | 建 single current validation doc     |

---

# 7. 下个月开发规范

## Monthly Dev Spec: 2026-05-29 → 2026-06-30

## 主题

```text
RitsuLib Runtime Proof + Architecture Integration Month
```

## 月度目标

1. 完成 RitsuLib runtime smoke。
2. 统一 test/build/docs 数字事实。
3. 把 Sts1Events 从 default safe 推进到 CanaryOnly 可手测。
4. 把 RewardPipeline / CardPlayContext 从 skeleton 推进到 diagnostics-only canary integration。
5. 将 DeathProtectionService / MultiplayerPolicy 从 stub 推进到 tracked + tested + docs-aligned。
6. 不启动 Batch 4c，除非 runtime smoke passes。
7. 不迁移 high-risk patches。
8. 不添加新 gameplay。
9. 不 claim release-ready / live-ready。

---

## Week 1 — Runtime Proof + Warning Truth

### Required Work

1. 验证安装：

   * BaseLib v3.1.4
   * STS2-RitsuLib
   * Spire Plus
2. 只启用这三个 mod。
3. Steam client 启动。
4. 捕获 `godot.log`。
5. 跑 log audit。
6. 统一 validation truth：

   * 使用最新 raw log，比如 current repo 写 444 / 465，或新 overnight 得到的新数字。
   * 不再混用 387 / 428 / 444。
7. 建立 warning issue：

   * Sts1Events nullable warnings
   * xUnit warning

### Acceptance

* RitsuLib active in `godot.log`。
* 25 ModPatcher patches applied。
* BaseLib initialized。
* Spire Plus initialized。
* SavedSpireFields expected count。
* 0 MissingMethodException。
* 0 TypeLoadException。
* 0 manifest dependency failure。
* 若 runtime smoke 无法跑，Batch 4c remains blocked + Hard Block report。

---

## Week 2 — Sts1Events Governance

### Required Work

1. Off mode runtime proof：

   * 0 registration
   * no StS1 event appears
2. CanaryOnly runtime proof：

   * 4 registrations
   * canary logs
3. AdditiveAllDraft risk table：

   * DeadAdventurer TODO
   * Joust no-gold guard
   * Vampires no Bite custom card
   * Nloth no RelicSelectCmd
   * MindBloom War blocked
4. ReplaceUnknownEventsPrototype：

   * debug-only
   * not default build

### Acceptance

* AdditiveAllDraft 不被误认为可玩。
* CanaryOnly 可以进入手测矩阵。
* docs/issues 更新。

---

## Week 3 — Architecture Skeleton Canary Integration

### RewardPipeline Canary

* 选择一个低风险 reward path。
* 只打印 phase / handler / context。
* 不改变 reward 行为。
* 证明不 softlock。

### CardPlayContext Canary

* 选择一个低风险 extra-play / fallback path。
* 记录 depth / policy / fallback。
* 不改变 card result。

### Acceptance

* source guard + behavior canary tests。
* logs prove diagnostics-only。
* no gameplay mutation。

---

## Week 4 — DeathProtection + MultiplayerPolicy Foundation

### DeathProtectionService

* tracked source。
* guard tests。
* Request / Result / Priority semantics。
* no gameplay behavior。
* Lotha DeathReprieve only as spec mapping。

### MultiplayerPolicy

* tracked source。
* active feature matrix:

  * Preview tools
  * Urda Root Eyes
  * Ascension combat
  * Sts1Events
  * Vakuu fight
* guard tests for:

  * LocalUiOnly
  * LocalPlayerOnly
  * HostAuthoritative
  * SharedRunState
  * CombatCommandReplicated
  * UnsafeInMultiplayer

### Acceptance

* every active feature has multiplayer category。
* no enforcement unless explicitly documented。
* co-op unsafe surfaces remain fail-closed。

---

## Week 5 — Consolidation / QA / Handoff

### Required Work

1. Independent QA / Red-Team review。
2. Build/test/format/diff-check。
3. Commit/push only after Green Stop。
4. Monthly review。
5. Runtime evidence status update。
6. No release-ready wording。

### Acceptance

* QA report exists。
* worktree clean。
* docs numbers unified。
* no false green。
* no Batch 4c unless runtime smoke passed。
* no high-risk patch migration。

---

# 8. Overnight Run 设置

## Runtime Proof + Governance Closure Overnight Run

必须持续到 **Green Stop** 或 **Hard Block Stop**。

## Green Stop 条件

全部满足才允许停止：

1. `git status --short` clean，或 dirty files 全部 documented。
2. `dotnet clean && dotnet build` raw log 存档。
3. `dotnet test` raw log 存档。
4. `dotnet format` clean。
5. `git diff --check` clean。
6. validation truth 只保留一个最新 raw count。
7. STS2-RitsuLib install status verified。
8. Runtime smoke Off mode 完成或 Hard Block report。
9. Runtime smoke CanaryOnly 完成或 Hard Block report。
10. Independent QA / Red-Team subagent 输出 pass/fail。
11. DeathProtectionService / MultiplayerPolicy tracked 或明确 blocked。
12. docs/issues、monthly-dev-spec、next-overnight-run、runtime-smoke-checklist 同步。
13. 不 claim release-ready / live-ready / full parity。
14. 不启动 Batch 4c。
15. 不迁移 high-risk patches。
16. 不添加新 gameplay。

## Hard Block Stop 模板

```text
Blocker:
Failed command:
Exact error:
Runtime/log evidence path:
Files touched:
Current git status:
What remains:
Owner:
Next command:
```

## 禁止停止

* “Runtime smoke 没法跑但 source proof 够了。”
* “测试通过所以 release-ready。”
* “stub 存在所以完成。”
* “QA 没做但自审通过。”
* “working tree dirty 但先停。”
* “incremental test 通过，跳过 clean build。”

---

# 9. Mandatory Subagent Plan

| Subagent                       | Scope                                                   | Output                                  | Pass / Fail                                        |
| ------------------------------ | ------------------------------------------------------- | --------------------------------------- | -------------------------------------------------- |
| Runtime Smoke Agent            | 安装/验证 STS2-RitsuLib，跑 Off / CanaryOnly game launch      | godot.log, smoke report                 | runtime log proves Off=0 / CanaryOnly=4 or blocker |
| QA / Red-Team Auditor          | 独立复核 build/test/runtime/docs/worktree                   | QA pass/fail report                     | cannot be implementation agent                     |
| Warning Triage Agent           | 分类 warnings                                             | warning matrix + issue row              | categories and owner assigned                      |
| Sts1Events Governance Agent    | AdditiveAllDraft risk table、Canary no-TODO confirmation | governance audit doc                    | AllDraft dev-only clear                            |
| FeatureRegistry Agent          | BootstrapStatus vs LiveStatus tests/logs                | guard tests + docs                      | Off/Canary/Vakuu hidden status tested              |
| Architecture Integration Agent | RewardPipeline/CardPlayContext diagnostics-only canary  | code + tests + no behavior change proof | low-risk path integrated                           |
| State/Death/Multiplayer Agent  | DeathProtection/MultiplayerPolicy tracked stubs + tests | tracked source + guard tests + matrix   | no gameplay behavior                               |
| Documentation Agent            | docs/issues/monthly spec/runtime checklist sync         | unified docs                            | no stale counts                                    |
| Release Safety Agent           | ensure no release/live/full parity claims               | release-safety checklist                | all runtime rows remain open                       |

---

# 10. 给助理的直接指令

```text
当前不能判定全部完成。你完成了 source-level / architecture skeleton / no-game validation 的大量工作，但 Runtime Proof + Governance Closure Overnight Run 还没有 Green Stop。

立即进入 Runtime Proof + Governance Closure Overnight Run，不能停止，除非 Green Stop 或 Hard Block Stop。

必须执行：
1. 先做 validation truth reconciliation：dotnet clean && dotnet build；dotnet test；dotnet format；git diff --check。raw log 数字覆盖所有旧数字。
2. 验证 STS2-RitsuLib 是否安装。未安装就写 Hard Block report；已安装就继续 runtime smoke。
3. 运行 Off mode runtime smoke：证明 0 Sts1Events registration。
4. 运行 CanaryOnly runtime smoke：证明 4 canary registrations。
5. 启动 independent QA/Red-Team subagent，不能自审。
6. 处理 dirty worktree：DeathProtectionService.cs、MultiplayerPolicy.cs、Stubs 要么 track + tests，要么写 blocker。
7. 更新 docs/features/ritsulib-migration/monthly-dev-spec.md、next-overnight-run.md、runtime-smoke-checklist.md、docs/issues.md。
8. 建 warnings matrix：Sts1Events nullable + xUnit warning。
9. 完成 Sts1Events governance audit：AdditiveAllDraft risk table、CanaryOnly no-TODO confirmation。
10. FeatureRegistry hardening：BootstrapStatus vs LiveStatus、Sts1Events Off/CanaryOnly、Vakuu hidden status tests。
11. Architecture canary integration：RewardPipeline diagnostics-only path + CardPlayContext low-risk path，不改变 gameplay。
12. MultiplayerPolicy active feature matrix + guard tests。
13. 禁止 Batch 4c，禁止 high-risk patch migration，禁止新增 gameplay，禁止 release-ready/live-ready claim。
```

---

## 最终判断

**当前应“优化为主，有限推进”。**

具体是：

* **优化优先**：runtime smoke、warning truth、docs truth、dirty worktree、independent QA。
* **有限推进**：RewardPipeline / CardPlayContext diagnostics-only canary、FeatureRegistry status refinement、Sts1Events CanaryOnly runtime proof。
* **禁止推进**：Batch 4c、大量 patch migration、高风险 patch、AdditiveAllDraft live、release packaging。

最后一句话结论：

> **这个项目当前最应该优先解决的是 RitsuLib runtime smoke 和独立 QA，因为它直接决定后续 Batch 4c、Sts1Events CanaryOnly、architecture skeleton integration 能否从 source-level 进展变成可信的 runtime-safe 交付。**
