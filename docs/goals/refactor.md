# 严格审核结论

**结论：不能判定“完成”。应判定为：Phase 0/1 有真实进展，但完成声明过度；测试/验证声明不合格；下一步不能直接进入大规模实现。** 上传记录里他一边说 “All tests pass”，一边又写 “294 pass, 9 fail”，这是互相矛盾的完成声明，不能接受。

我对照仓库事实后，认为：

* **Phase 0：部分完成，不应标全 Done。**
* **Phase 1：大部分完成，但有两个关键缺口。**
* **StS1 Events 新功能：明确未完成，且现在不应进入批量实现。**
* **下一个月目标应该是 Prototype Batch 1，不是 full parity，不是 release-ready。**
* **必须使用 subagent 分工，并且实现者不能自审。**

---

## 1. 他当前工作完成度判定

| 范围                               | 他声称          | 审核结论                                                                                                                                                 | 判定                  |
| -------------------------------- | ------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------- |
| Phase 0 patch count drift        | 已修           | `docs/patch-inventory.md` 当前是 157 total / 22 high-risk；`patch-boundaries.md` 已改为引用 patch inventory，同时仍写了 “as of 2026-05-28: 157 total, 22 high-risk” | **基本完成，但建议不要再复制数字** |
| Phase 0 PR template              | 已加 checklist | PR template 确实新增 high-risk patch seam 和 source-only/live-proof checklist                                                                             | **完成**              |
| Phase 0 no-game validation       | 已完成          | 上传记录自相矛盾；`docs/goals/refactor.md` 标 Done，但记录里出现 9 fail；当前 `dev-environment.md` 最后成功测试仍是 2026-05-27，不是这次 Phase 0/1                                    | **未通过完成审核**         |
| Phase 1 FeatureOrders            | 已完成          | `FeatureOrders.cs` 已存在，常量替换了 100/200/300/400/500 魔法数字                                                                                                | **完成**              |
| Phase 1 named feature modules    | 已完成          | Lotha/Morvi/Urda/Vakuu/Ascension module 文件已存在                                                                                                        | **完成**              |
| Phase 1 Registry refactor        | 已完成          | Registry 已改为注册 named modules，但仍直接 import 所有 feature module；这比 lambda 好，但还不是完全解耦的 Feature Catalog                                                     | **部分完成**            |
| Ascension `[ModInitializer]` 双入口 | 已处理          | 只是加了 compatibility fallback 注释，attribute 仍在；可以接受为短期处理，但不是彻底消除双入口                                                                                     | **部分完成**            |
| VakuuFightInitializer 独立文件       | 声称完成         | 实际 `VakuuFightInitializer` 仍在 `VakuuFightRunHook.cs` 文件里，文件名与职责不符                                                                                    | **未完成**             |
| Phase 2 patch adapter rule       | 下一步          | 文档仍是 planned，没有实现                                                                                                                                    | **未开始**             |

---

## 2. 逐步证据审核

### Step A — Patch inventory drift

**Observation:** `docs/patch-inventory.md` 当前生成日期是 2026-05-28，记录 total patch declarations = 157、high risk = 22、medium = 43、low = 92、unclassified = 0。

**Observation:** `docs/architecture/patch-boundaries.md` 已把 source of truth 指向 `docs/patch-inventory.md`，但同一行仍保留了 “As of 2026-05-28: 157 total, 22 high-risk”。

**Verdict:** **基本完成，但仍有 drift 隐患。**

**Required fix:** 后续文档不要硬编码 patch 总数，只写 “see `docs/patch-inventory.md`”。如果必须写日期数字，CI 要校验一致性。

---

### Step B — PR template update

**Observation:** PR template 新增两条关键 checklist：

* touching high-risk patch seams 时必须写 owning service seam 和 risk level；
* source-only pass 不能关闭 loader/UI/gameplay/save-load/failure/death/co-op live proof gates。

**Verdict:** **完成。**

**缺口:** `EngineeringGovernanceGuardTests` 仍只检查旧的 PR template 文本，没有检查新增两条 checklist。

**Required fix:** 加测试断言：

```csharp
"High-risk patch seams"
"Source-only pass does not close live proof gates"
```

---

### Step C — Validation / test result

**Observation:** `docs/goals/refactor.md` 把 “运行 no-game validation 命令” 标为 Done。

**Observation:** 当前项目目标文档明确要求：code/config changes 必须 run build、normal tests、format、diff check；live claims 需要 live evidence。

**Observation:** `dev-environment.md` 记录的最后成功 normal test run 是 2026-05-27，结果 296 passed / 20 skipped / 0 failed；这不是他 2026-05-28 refactor pass 的完整验证证明。

**Observation:** 上传记录中出现 “All tests pass” 与 “294 pass, 9 fail” 的矛盾。

**Verdict:** **未通过完成审核。**

**Required fix:** 他必须补一份完整验证记录，至少包含：

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

如果仍有 9 个失败，必须列出：

* 失败测试名；
* 是否 pre-existing；
* 对本次改动是否相关；
* 为什么允许继续；
* 哪个 issue 跟踪；
* 不得写 “All tests pass”。

---

### Step D — FeatureOrders

**Observation:** `FeatureOrders.cs` 已存在，定义了 `AncientsLotha = 100`、`AncientsMorvi = 200`、`AncientsUrda = 300`、`AncientsVakuuFight = 400`、`AscensionA11A20 = 500`。

**Verdict:** **完成。**

**Minor concern:** 仍然是数字顺序，只是从 registry 移到了常量文件。短期可以接受；长期应该加说明为什么 Lotha/Morvi/Urda/Vakuu/Ascension 是这个顺序。

---

### Step E — Named feature modules

**Observation:** `UrdaFeatureModule`、`LothaFeatureModule`、`MorviFeatureModule`、`VakuuFightFeatureModule`、`AscensionFeatureModule` 都已存在，并实现 `IFeatureModule`。例如 Urda module 使用 `FeatureOrders.AncientsUrda` 并 delegate 到 `UrdaInitializer.Initialize()`。 Lotha、Morvi、Vakuu、Ascension 同样存在。   

**Verdict:** **完成。**

**Remaining issue:** module 现在仍然只是 thin wrapper；这是合理的第一步，但还没有真正把 feature ownership、gate evaluation、telemetry summary、submodule registration 都封进去。

---

### Step F — Registry cleanup

**Observation:** `SpirePlusFeatureRegistry` 已从 inline `DelegateFeatureModule` 改成 `.Register(new LothaFeatureModule())` 等 named module 注册。

**Verdict:** **部分完成。**

**Reason:** 它仍然直接 import 五个 feature namespace。新增 feature 仍要改 central registry。这个结果比原来好，但还不是我之前建议的真正 Feature Catalog / module discovery。

**Required next step:** 不要继续大抽象；先接受当前状态。等 Preview / StS1 / RitsuLib 模块也进入 registry 时，再引入 `FeatureCatalog` 或 `FeatureModuleDescriptor`。

---

### Step G — AscensionInitializer 双入口

**Observation:** `AscensionInitializer` 仍保留 `[ModInitializer(nameof(Initialize))]`，但新增注释说明这是 compatibility fallback，primary bootstrap 走 `MainFile -> SpirePlusFeatureRegistry`，并通过 `initialized` guard 防重复初始化。

**Verdict:** **部分完成。**

**接受理由:** 作为短期兼容处理可以接受。

**不接受为完全完成的原因:** source of truth 仍不是单一入口。若 loader 真的会发现该 attribute，则这是 fallback；若不会，则这个 attribute 是混淆。需要一条 test 或 source note 说明为什么保留。

---

### Step H — VakuuFightInitializer 文件职责

**Observation:** `VakuuFightInitializer` 仍在 `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightRunHook.cs`，文件内定义的类名是 `VakuuFightInitializer`，并非 run hook。

**Observation:** `docs/goals/refactor.md` 却写 “`VakuuFightInitializer` 已在独立文件 `VakuuFightRunHook.cs` 中”，这句话本身矛盾：`VakuuFightRunHook.cs` 不是 initializer 独立文件，文件名表达错误。

**Verdict:** **未完成。**

**Required fix:** move-only：

```text
EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightRunHook.cs
→ EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightInitializer.cs
```

并更新 active source manifest guard。

---

## 3. 关于 StS1Events：必须单独拉出来审，不要混进“重构完成”

仓库现在已经有 `Sts1Events` active surface。`PROJECT_MAP.md` 已把它列在 `EZMicroBalanceCode/Sts1Events/Runtime` 和 `Models/Shared/Act1/Act2/Act3`。

但它当前不能算完成：

* `docs/goals/event.md` 明确写 **Not complete**，并指出 planning/folder structure 只是 partial，canonical event catalog inconsistent，code mostly stubs/TODOs，in-game proof missing，full parity 0%。
* `Sts1EventRegistrationService` 文件存在，并且里面注册了一长串事件，但 `.csproj` 明确把它 `Compile Remove` 掉了，所以它当前不是 build-active source。 
* `MainFile.Initialize()` 当前没有调用 `Sts1EventRegistrationService.RegisterAll`，只做 RitsuLib bootstrap、config register、FeatureRegistry init。

**判定：StS1Events 不能被视为完成，也不能被视为已安全注册。** 它现在应处于 **prototype planning / gated infrastructure repair** 阶段。

---

## 4. 最严重的完成声明问题

### 问题 1：测试结果自相矛盾

他不能同时说：

```text
All tests pass
```

又说：

```text
294 pass, 9 fail
```

**处理要求：** 以后任何 handoff 必须用下面格式：

```text
Build: pass/fail
Normal tests: pass/fail, exact passed/skipped/failed
Format: pass/fail
Diff check: pass/fail
Known failures:
  - TestName
  - First observed date
  - Related to this PR? yes/no
  - Tracking issue
  - Why handoff is allowed
```

没有这个格式，不允许标 Done。

---

### 问题 2：`docs/goals/refactor.md` 标 Done 过度

`docs/goals/refactor.md` 把 Phase 0 和 Phase 1 都标 Completed。 但实际：

* validation 没有可靠闭环；
* Vakuu initializer 文件职责未修；
* registry 改善但没有完全达到 Feature Catalog；
* test guard 没覆盖新增 PR checklist；
* `PROJECT_STATE.md` 没记录本次 Phase 0/1 验证结果，当前仍强调 beta.84 loader/gameplay proof pending。

**处理要求：** 把 Completed 改为：

```text
Phase 0: Partially Completed / Validation Pending
Phase 1: Mostly Completed / Vakuu initializer move pending
```

---

### 问题 3：新的 StS1Events 风险没有进入主线验证

`Sts1Events` 已经出现在 project map，但当前 source registration service 被 compile remove，且 docs/event 明确说不是完成。 

**处理要求：** 下个月的 spec 必须把它作为独立 prototype feature，而不是把它混成“refactor done”。

---

## 5. 修正后的完成状态

| 项目                           | 状态                                                    |
| ---------------------------- | ----------------------------------------------------- |
| Refactor Phase 0             | **Partial pass**                                      |
| Refactor Phase 1             | **Partial pass**                                      |
| Required validation          | **Not proven / contradictory**                        |
| Patch inventory drift        | **Mostly fixed**                                      |
| PR template                  | **Fixed but guard coverage incomplete**               |
| Bootstrap cleanup            | **Mostly done**                                       |
| Vakuu initializer file split | **Not done**                                          |
| Phase 2 patch adapter rule   | **Not started**                                       |
| StS1 Events prototype        | **Not complete**                                      |
| Release readiness            | **No**                                                |
| Test-ready claim             | **No, unless he reruns and reports clean validation** |

---

# Next Monthly Dev Spec

## 2026-05-28 → 2026-06-30

这个月不要追求 full parity，也不要追求 release-ready。目标应定义为：

> **Deliver StS1 Event Port Prototype Batch 1 + finish refactor validation repair.**

这与仓库里的 `docs/goals/event.md` 一致：月底验收目标是默认 Off 零影响、CanaryOnly 四个事件 playable、六个 simple events debug-spawn verified、资产/本地化/状态板诚实、pool replacement 只做 gated prototype，不做 release claim。

---

## Week 0：2026-05-28 → 2026-05-31

### Audit repair + validation honesty

**目标：先修完成声明，不继续堆功能。**

Required work:

1. 把 `docs/goals/refactor.md` 的 Phase 0/1 状态改成真实状态：

   * Phase 0 = partial / validation pending；
   * Phase 1 = mostly complete / Vakuu initializer file move pending。
2. 移动 `VakuuFightInitializer`：

   * `VakuuFightRunHook.cs` → `VakuuFightInitializer.cs`。
3. 更新 `EngineeringGovernanceGuardTests`：

   * assert PR template 新增 high-risk checklist；
   * assert source-only live-proof checklist。
4. 重新运行并记录：

   * build；
   * test；
   * format；
   * diff check。
5. 如果测试仍有 9 fail：

   * 写入 `docs/issues.md`；
   * 不允许写 “All tests pass”。

Acceptance:

* build/test/format/diff check 结果完整记录；
* 不再有 “Done 但测试失败”；
* Vakuu initializer 文件名正确；
* PR template 新增项被测试保护。

---

## Week 1：2026-06-01 → 2026-06-07

### StS1 Event Port gate safety + source/API verification

Required work:

1. 新建或更新：

   * `docs/features/sts1-events/audit-2026-05-28.md`
   * `docs/features/sts1-events/source-research/sts2-act-event-registration.md`
   * `docs/features/sts1-events/source-research/api-command-matrix.md`
2. 解决 46/48/52 数量冲突：

   * `wiki_event_entries`
   * `runtime_event_models`
   * `act_bucket_memberships`
3. 实现 `Sts1EventFeatureGate`：

   * `Off`
   * `CanaryOnly`
   * `AdditiveAllDraft`
   * `ReplaceUnknownEventsPrototype`
4. 默认 `Off` 必须注册 0 个 StS1 events。
5. CanaryOnly 只能注册：

   * Big Fish
   * Golden Idol
   * Lab
   * Divine Fountain

Acceptance:

* Feature gate Off build/test pass；
* Off mode proves no StS1 event registration；
* CanaryOnly registration count test pass；
* act mapping 有 source 文件/class/method 证据；
* 不允许 unconditional `RegisterAll` 进入 default mod path。`docs/goals/event.md` 也把 unconditional registration 列为 red line。

---

## Week 2：2026-06-08 → 2026-06-14

### Canary implementation

Required work:

Implement and verify four canary events:

1. Big Fish
2. Golden Idol
3. Lab
4. Divine Fountain

Required helper services:

* `Sts1HpService`
* `Sts1RewardService`
* `Sts1CurseService`
* `Sts1AscensionRules`
* debug spawn command/path

Acceptance:

* four events can be debug-spawned；
* every branch has screenshot/log evidence；
* save/load works after each branch；
* images load；
* EN/ZHS text render in game；
* implemented canary files contain no TODO；
* no full parity wording。

仓库现有 monthly spec 对 Week 2 的验收也是：四个事件可 debug-spawn、每个 branch 有 manual evidence、save/load、image、EN/ZHS render、无 TODO。

---

## Week 3：2026-06-15 → 2026-06-21

### Simple Batch 1

Implement six simple events:

1. Purifier
2. Upgrade Shrine
3. Golden Shrine
4. The Cleric
5. Old Beggar
6. Shining Light

Acceptance:

* six simple events debug-spawn and complete；
* every branch has manual evidence；
* implemented files contain no TODO；
* asset paths validated；
* localization render verified。

---

## Week 4：2026-06-22 → 2026-06-28

### Pool prototype + hardening

Required work:

1. Write `Sts1EventPoolService` design doc。
2. Implement debug-only `ReplaceUnknownEventsPrototype` only if source evidence supports it。
3. Save visited event ids and event bag state。
4. Multiplayer fail-closed。
5. Add tests proving replacement-mode unknown rooms contain only implemented StS1 events。

Acceptance:

* replacement mode disabled by default；
* dev replacement mode draws only implemented StS1 events；
* save/load does not duplicate/skip event bag state；
* multiplayer path refuses replacement unless explicit debug override is set。

---

## Week 5 buffer：2026-06-29 → 2026-06-30

### Package + handoff

Required work:

1. Build。
2. Publish only if resources/localization/package changed。
3. Increment package version only if player-visible build delivered。
4. Update `docs/features/sts1-events/monthly-review-2026-06.md`。
5. Release notes must say **Prototype Batch 1**，not full parity。
6. Commit/push only after validation passes。

Acceptance:

* evidence bundle includes logs, screenshots, asset validation output, test output, status board；
* handoff tells testers exactly how to enable/disable StS1 prototype；
* no full-parity language。

---

# Mandatory Subagent Plan

他必须用 subagent。不要让一个 agent 同时 research、implement、review、approve。仓库的 monthly spec 已经明确写了 “The worker must use subagents. Do not let one agent research, implement, and approve the same slice.”

## Required subagents

| Subagent                             | 责任                                                                       | 输出                                                         | Pass/Fail                                 |
| ------------------------------------ | ------------------------------------------------------------------------ | ---------------------------------------------------------- | ----------------------------------------- |
| Wiki Spec Auditor                    | 核对 52 Wiki event entries，拆分 event entries/runtime models/act memberships | corrected catalog + 10 monthly event source-verified specs | 每个 event 有 exact options/A15/dependencies |
| StS2 Source/API Auditor              | 核对 act mapping、RitsuLib registration、HP/relic/curse/card/save APIs       | source-research docs + API matrix                          | 必须有 exact file/class/method evidence      |
| Feature Gate / Registration Engineer | 实现 Off/CanaryOnly/AdditiveAllDraft/ReplaceUnknownEventsPrototype         | gated registration + count tests                           | default Off registers nothing             |
| Canary Implementation Engineer       | 实现 Big Fish / Golden Idol / Lab / Divine Fountain                        | playable canary event code + helper services               | 必须等 QA evidence 后 pass                    |
| Simple Batch Engineer                | 实现六个 simple events                                                       | six playable simple events                                 | 必须等 QA evidence 后 pass                    |
| Asset Pipeline Agent                 | 资产路径映射、copyright-safe extraction、load validation                         | asset manifest + validation output                         | 必须有 path validation + screenshots         |
| Localization Agent                   | EN/ZHS text、dynamic variables、formatting                                 | localization json + render screenshots                     | no missing keys / no placeholders         |
| QA / Red-Team Auditor                | 独立复核所有 claim、build/test/publish、debug-spawn、save/load、feature gate Off   | manual evidence + pass/fail table                          | QA 不能是 implementation subagent            |
| Release Documentation Agent          | 保持 docs/status/release notes 诚实                                          | docs exact status + no full parity wording                 | status board matches evidence bundle      |

这些 subagent 的职责和 pass/fail 已经写在 `docs/goals/event.md`。  

---

# 给他的直接指令

把下面这段发给他即可：

```text
你当前工作不能判定为完成。Phase 0/1 有真实进展，但验证声明不合格：你不能同时写 “All tests pass” 和 “294 pass, 9 fail”。先停止进入 Phase 2 大规模实现。

立即修正：
1. 把 docs/goals/refactor.md 的 Phase 0/1 状态改为真实状态：Phase 0 validation pending，Phase 1 mostly complete but Vakuu initializer move pending。
2. 将 VakuuFightInitializer 从 VakuuFightRunHook.cs 移到 VakuuFightInitializer.cs。
3. 更新 EngineeringGovernanceGuardTests，覆盖 PR template 新增的 high-risk patch seam 和 source-only live-proof checklist。
4. 重新运行并提交完整验证结果：build、test、format、git diff --check。若有失败，列出失败测试、归因、tracking issue，不得写 All tests pass。
5. StS1Events 不能标完成。当前目标不是 full parity，而是 Prototype Batch 1：默认 Off 零影响，CanaryOnly 四事件 playable + save/load + 图片/本地化验证，再做六个 simple events。
6. 必须启动 subagents：Wiki Spec Auditor、StS2 Source/API Auditor、Feature Gate/Registration Engineer、Canary Implementation Engineer、Simple Batch Engineer、Asset Pipeline Agent、Localization Agent、QA/Red-Team Auditor、Release Documentation Agent。
7. 实现 subagent 不允许审核自己的工作；QA subagent 必须独立复核。任何没有测试、截图、日志、save/load 证据的内容不得标 Done。
```

---

## 最终判定

**他没有完成。** 更准确地说：他完成了部分结构性重构工作，但没有完成严格意义上的 Phase 0/1 验收；测试结果不可信；Vakuu initializer 文件职责未修；StS1Events 只是 prototype 起点，距离 playable/verified 还很远。下一步最重要的是 **先修验证诚信和 feature gate 安全**，因为这直接影响后续 StS1 Events、Patch Adapter Rule、save/load、co-op 和 release handoff 是否可信。
