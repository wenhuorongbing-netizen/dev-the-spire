## 严格验收结论

**没有全部达成我们定义的 RitsuLib 迁移目标。**

更准确地说：

```text
GitHub main 上目前完成了：
- 一部分 FeatureRegistry / 初始化解耦；
- 一部分项目结构和文档整理；
- 当前 Spire Plus 大模块仍能通过原 initializer 路径启动。

GitHub main 上没有完成：
- RitsuLib 依赖接入；
- RitsuLib runtime dependency；
- RitsuLib lifecycle smoke；
- RitsuLib DataStore POC；
- RitsuLib migration docs；
- 真正意义上的 RitsuLib-first architecture。
```

另外，你贴的 Codex 报告说有一个本地分支：

```text
refactor/integrate-harness-ritsulib-cleanup
```

并列出 `737acab / 29d1b93 / 7d6d736 ...` 这些 commit。但我检查 GitHub 时，这个 branch/ref 没有找到，`737acab` 这个 commit 也不在 GitHub 可见仓库状态里。也就是说：**Codex 汇报的那组 7 个 commit 目前无法按 GitHub 远程状态验收**。我只能验收当前 GitHub `main`。

---

# 1. 当前 GitHub main 实际情况

## 1.1 当前项目已经有 FeatureRegistry 雏形

`MainFile.Initialize()` 现在确实不再直接调用所有 `LothaInitializer / MorviInitializer / UrdaInitializer / VakuuFightInitializer / AscensionInitializer`，而是调用：

```csharp
SpirePlusFeatureRegistry.CreateDefault().InitializeAll();
```

这说明 **初始化入口的第一层解耦已经发生**。

`SpirePlusFeatureRegistry` 目前注册了：

```text
Ancients.Lotha
Ancients.Morvi
Ancients.Urda
Ancients.VakuuFight
Ascension.A11A20
```

并且每个模块仍然是通过 delegate 调原来的 `Initializer.Initialize()`。

`FeatureRegistry` 会按 `InitOrder` 排序，打印 enabled/disabled 状态，然后执行 `module.Initialize()`。这比原先直接写在 `MainFile` 里好。

`IFeatureModule` 目前很小，只包含：

```text
Id
InitOrder
EvaluateGate()
Initialize()
```



`FeatureGateResult` 也只是 `bool IsEnabled + string Reason`。

### 判定

```text
FeatureRegistry：部分完成。
真正模块化：未完成。
RitsuLib Feature Registry：未完成。
```

它现在是一个轻量 wrapper，不是完整的 feature module architecture。

---

## 1.2 当前没有接入 RitsuLib NuGet

`EZMicroBalance.csproj` 当前依赖仍然是：

```xml
<PackageReference Include="Alchyr.Sts2.BaseLib" Version="3.1.4" PrivateAssets="All"/>
<PackageReference Include="Krafs.Publicizer" Version="2.3.0" PrivateAssets="All"/>
<PackageReference Include="Alchyr.Sts2.ModAnalyzers" Version="0.1.9" />
```

没有：

```xml
<PackageReference Include="STS2.RitsuLib" />
```



### 判定

```text
RitsuLib compile-time dependency：未完成。
```

---

## 1.3 当前 manifest 没有 RitsuLib runtime dependency

`EZMicroBalance.json` 当前 dependencies 只有 BaseLib：

```json
"dependencies": [
  {
    "id": "BaseLib",
    "min_version": "v3.1.4"
  }
]
```

没有：

```json
{ "id": "STS2-RitsuLib" }
```



### 判定

```text
RitsuLib runtime dependency：未完成。
```

---

## 1.4 当前 Project Map 没有 RitsuLib migration / Core integrations 入口

`docs/PROJECT_MAP.md` 现在的 active source surface 仍主要是：

```text
Ancients/Common
Ancients/Expansion/Urda
Ancients/Expansion/Morvi
Ancients/Expansion/Lotha
Ancients/Expansion/Vakuu
Ascension
Preview
```

它没有列出：

```text
Core/Integrations/RitsuLib
docs/features/ritsulib-migration
```

并且 active mod surface 里也没有 `Core/Features`，虽然源码里已经有 `EZMicroBalanceCode/Core/Features`。

### 判定

```text
目录文档没有完全同步。
RitsuLib migration docs：未在 main 上存在。
Project map 对 Core/Features 也有轻微 stale。
```

---

# 2. Codex 报告逐项审核

## 2.1 “Codex harness templates, RitsuLib staging, refactor-map, migration plan”

**GitHub main 未见到完整结果。**

我没有在 main 上找到 `docs/features/ritsulib-migration/README.md` 或 `migration.md`。`EZMicroBalance.csproj` 也没有 RitsuLib 包引用。

如果这部分存在，只能是在本地分支，未 push 或 ref 名不对。

### 判定

```text
不能按 GitHub 验收。
```

---

## 2.2 “Directory scaffolds: Core/Integrations/RitsuLib/, Ancients/Rebalance/, Ascension/Ui/, Ascension/Save/”

**GitHub main 只能确认部分架构目录变化，不能确认这些报告目录全部存在。**

从 main 能确认的是：

```text
EZMicroBalanceCode/Core/Features
```

因为 `MainFile` 引用了它，`FeatureRegistry` 文件也存在。 

但当前 `PROJECT_MAP.md` active surface 没有列出 `Core/Integrations/RitsuLib/`，也没有列出 `Ascension/Ui/` / `Ascension/Save/`。

### 判定

```text
部分结构存在。
报告提到的 RitsuLib staging / Ascension Ui/Save scaffolds 未能在 GitHub main 证实。
```

---

## 2.3 “README + PROJECT_MAP docs for new directories”

**部分完成，但不完整。**

`PROJECT_MAP.md` 已经非常详细，但它没有把 `EZMicroBalanceCode/Core/Features` 作为 active source surface 列出来，虽然当前 `MainFile` 已使用它。 

也没有看到 `RitsuLib` migration docs 的入口。

### 判定

```text
文档更新：部分完成。
Core/Features 和 RitsuLib 迁移入口：未完整同步。
```

---

## 2.4 “Mark PR 1-4 as done in migration.md”

**GitHub main 未找到 `migration.md`。**

所以无法验收。

### 判定

```text
未达成远程验收。
```

---

## 2.5 “Expand PR 5 version mismatch blockers / NuGet package status in ritsulib.md”

Codex 报告的 blocker 是：

```text
No RitsuLib runtime variant for game target v0.106.0
No NuGet compat package for 0.106.0 or 0.106.1
Resolution needed before adding RitsuLib as a hard dependency
```

这个判断方向是合理的，但 GitHub main 上没有我能确认的 `ritsulib.md` / migration doc。

我能确认的是：当前项目目标是 v0.106.0 + BaseLib 3.1.4。`PROJECT_STATE.md` 写明了这一点。 当前 csproj 仍只用 BaseLib 3.1.4，没有 RitsuLib 包。

### 判定

```text
PR5 blocker 逻辑合理。
但 blocker 文档未能在 GitHub main 验证。
```

---

# 3. 我们原始 RitsuLib 迁移目标达成度

| 目标                                            | 当前 GitHub main 状态                          | 判定            |
| --------------------------------------------- | ------------------------------------------ | ------------- |
| RitsuLib 作为 compile dependency                | csproj 无 `STS2.RitsuLib`                   | 未完成           |
| manifest 加 `STS2-RitsuLib` runtime dependency | manifest 只有 BaseLib                        | 未完成           |
| RitsuLib lifecycle smoke                      | 无证据                                        | 未完成           |
| RitsuLib DataStore smoke                      | 无证据                                        | 未完成           |
| FeatureRegistry                               | 有轻量 wrapper                                | 部分完成          |
| MainFile 解耦                                   | 已从直接 initializer 改为 FeatureRegistry        | 基本完成          |
| Feature module metadata                       | 只有 Id/InitOrder/EvaluateGate/Initialize    | 部分完成          |
| Feature gate 统一                               | 各 feature 仍保留自身 gate，registry 只是写说明        | 部分完成          |
| State codec                                   | Urda/Morvi/Lotha 仍以 saved string fields 为主 | 未完成           |
| Reward pipeline                               | 无 central pipeline                         | 未完成           |
| CardPlayContext                               | 无统一 context                                | 未完成           |
| DeathProtectionService                        | 无统一 service                                | 未完成           |
| MultiplayerPolicy                             | 无统一 policy                                 | 未完成           |
| Ritsu migration docs                          | main 未找到                                   | 未完成           |
| Build pass                                    | Codex 本地称 0 errors/0 warnings              | 可接受但远程无 CI 状态 |

---

# 4. 当前实现中的实际问题

## 4.1 FeatureRegistry 只是 wrapper，还没有真正解耦

`SpirePlusFeatureRegistry` 仍然手写引用所有 feature initializer：

```csharp
LothaInitializer.Initialize
MorviInitializer.Initialize
UrdaInitializer.Initialize
VakuuFightInitializer.Initialize
AscensionInitializer.Initialize
```



这意味着模块仍然不是自描述对象。它还没有：

```text
DisplayName
DefaultEnabled
Disable env list
Force env list
RuntimeStatus
Diagnostics
Dependencies
```

`FeatureGateResult` 也只有 bool 和 reason。

这一步只能算“MainFile 清爽了”，不能算“架构完成”。

---

## 4.2 FeatureRegistry 里的 gate 可能误导

Registry 里对 Morvi/Lotha/Urda 都直接写：

```text
enabled by default
runtime gates remain in FeatureGate
```

但 `FeatureRegistry` 自身并不真正调用具体 `MorviFeatureGate.IsMorviEnabled(...)` 来决定是否 initialize，它只是初始化整个 feature 的 hooks。

这不一定错，因为 Morvi/Lotha 的内部 gate 可能控制实际 offer pool。但从架构角度看：

```text
Registry log says enabled=true，
但真实 gate 决策在内部，二者可能不一致。
```

更好的写法是：

```text
FeatureRegistry 控制“模块是否注册 hook”
FeatureGate 控制“模块是否进入 live pool”
这两个概念必须分开命名：
- ModuleInitialized
- FeatureAvailableInGame
```

否则测试员看到：

```text
Feature Morvi enabled=true
```

会以为 Morvi 一定 live，而实际上内部还可能被 gate 排除，或者反过来。

---

## 4.3 RitsuLib blocker 是真实 blocker，不能绕过

Codex 说“没有 v0.106.0 / 0.106.1 兼容 RitsuLib NuGet / runtime variant”，如果事实如此，那不能硬接入 RitsuLib。

你当前项目目标确实是 v0.106.0，BaseLib 3.1.4。 RitsuLib README 也强调当前分支和旧 API branch 用不同 package / compat package / variant pack。

所以在没有确认 RitsuLib 兼容包之前，不能做：

```xml
<PackageReference Include="STS2.RitsuLib" />
```

更不能加 runtime dependency 后发测试包。

---

# 5. 是否存在“奇怪内容”

有几个需要修正：

## 5.1 本地分支未 push 或 ref 不存在

Codex 报告的分支：

```text
refactor/integrate-harness-ritsulib-cleanup
```

我无法在 GitHub 上 fetch 到。它列的 `737acab` 等 commit 也不可见。

### 影响

```text
无法验收它说的 7 个 commit。
不能把它视为 GitHub 已完成。
```

## 5.2 项目地图没有同步 `Core/Features`

`MainFile` 已经依赖 `EZMicroBalanceCode/Core/Features`。
但 `PROJECT_MAP.md` active surface 里没有列 `Core/Features`。

这是小问题，但说明 docs scaffold 没完全同步。

## 5.3 RitsuLib 迁移命名和当前实际状态不一致

如果 Codex 在本地写了“PR1-4 done”，但 main 没有对应文档或 commits，会造成下一轮 Codex 混乱。

必须先明确：

```text
这些是本地 branch 工作，还是 main 已合并？
```

---

# 6. 现在应该定的下一个 goal

我建议下一步不是继续写 FeatureRegistry，而是先解决“RitsuLib 兼容性决策 + branch 落地”。

## Goal 名称

```text
GOAL-2026-05-26-RITSULIB-COMPATIBILITY-DECISION-AND-BRANCH-LANDING
```

## 目标

明确 RitsuLib 当前是否能作为 v0.106.0 / v0.106.1 的硬依赖；如果不能，当前 migration branch 只能作为 architecture scaffold，不能宣称 Ritsu migration complete。

---

# 7. 给 Codex 的下一步 prompt

```text
你现在在仓库：

D:\Game\FOTN\dev-the-spire

目标：RitsuLib compatibility decision + branch landing。

当前审核结果：
- GitHub main 已有轻量 FeatureRegistry，MainFile 调用 `SpirePlusFeatureRegistry.CreateDefault().InitializeAll()`。
- GitHub main 没有 `STS2.RitsuLib` NuGet package reference。
- GitHub main manifest 只依赖 BaseLib，没有 STS2-RitsuLib。
- GitHub main 未找到 `docs/features/ritsulib-migration/README.md` 或 `migration.md`。
- 你汇报的 branch `refactor/integrate-harness-ritsulib-cleanup` 和 commit `737acab` 等没有在 GitHub main 可见，也无法远程验收。
- 你汇报的 PR5 blocker “no RitsuLib runtime variant / NuGet compat for v0.106.x” 需要正式记录并决策。

不要实现新 gameplay。
不要关闭 Morvi/Lotha/Urda 默认开启。
不要强行添加 RitsuLib 依赖，除非兼容性证明完成。
不要 claim Ritsu migration complete。

必须做：

1. Git branch/source truth
   - 运行：
     - git status --short --branch
     - git log -10 --oneline --decorate
     - git branch --show-current
     - git branch -a | findstr ritsu
   - 明确：
     - `refactor/integrate-harness-ritsulib-cleanup` 是否本地存在？
     - 是否已 push？
     - 7 个 commit 是否在远程？
     - 如果未 push，不要说 GitHub 已完成。

2. Main branch docs sync
   - 更新 `docs/PROJECT_MAP.md`，把 `EZMicroBalanceCode/Core/Features/` 列为 current active source。
   - 更新 `EZMicroBalanceCode/README.md`，说明 `Core/Features` 是 lightweight registry scaffold, not RitsuLib integration.
   - 如果 `docs/features/ritsulib-migration/` 不存在，创建它。
   - 如果本地 branch 有 migration docs，把它们同步到 main or provide exact branch/ref.

3. RitsuLib compatibility decision doc
   创建：
   - `docs/features/ritsulib-migration/README.md`
   - `docs/features/ritsulib-migration/compatibility-decision.md`
   - `docs/features/ritsulib-migration/migration-plan.md`
   - `docs/features/ritsulib-migration/work-log.md`

   必须写清：
   - Current game target: v0.106.0
   - BaseLib: 3.1.4
   - RitsuLib current available package/runtime status
   - Whether STS2.RitsuLib has v0.106.0 / v0.106.1 compatible NuGet package
   - Whether runtime variant pack supports v0.106.0
   - If not compatible, RitsuLib hard dependency is blocked.
   - Current FeatureRegistry is a local architecture scaffold, not RitsuLib migration.
   - Migration remains planned, not implemented.

4. Tests / guards
   Add or update tests:
   - If `STS2.RitsuLib` is not in csproj, docs must not claim hard dependency added.
   - If manifest has no `STS2-RitsuLib`, docs must not claim runtime RitsuLib dependency.
   - `PROJECT_MAP.md` must mention `Core/Features`.
   - `docs/features/ritsulib-migration/compatibility-decision.md` must mention v0.106.0 and blocker status.
   - MainFile must use FeatureRegistry, as already implemented.

5. Do not add RitsuLib package yet
   Unless you can prove:
   - NuGet package compatible with v0.106.0 exists,
   - runtime mod variant supports v0.106.0,
   - clean loader smoke can run with BaseLib + RitsuLib + Spire Plus.

6. Validation
   Run:
   - dotnet build EZMicroBalance.sln
   - dotnet test EZMicroBalance.sln --no-build
   - dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
   - git diff --check

   Do not publish/package if only docs/tests changed.

Final response:
- Whether branch was pushed.
- Whether RitsuLib dependency was actually added.
- Whether RitsuLib compatibility is blocked.
- Files changed.
- Tests run.
- Next recommended action:
  A. wait for v0.106 compatible RitsuLib,
  B. target v0.106.1 if compatible,
  C. build/obtain compatible RitsuLib,
  D. continue local architecture scaffold without hard dependency.
- Release-ready: no.
```

---

# 8. 下一阶段后续 goal

如果上面完成并确认 RitsuLib 兼容性仍 blocked，下一个合理 goal 是：

```text
GOAL-2026-05-26-FEATURE-REGISTRY-HARDENING-WITHOUT-RITSULIB
```

内容：

```text
[ ] 扩展 IFeatureModule：DisplayName / EnvGate / RuntimeStatus / Dependencies
[ ] FeatureRegistry 调用真实 gate，不只是写 reason
[ ] FeatureRegistry 记录 ModuleInitialized vs FeatureLiveAvailable
[ ] Feature status 输出到 log
[ ] 更新 tests
```

也就是说，在 RitsuLib 可用前，先把本地 architecture scaffold 做扎实。

如果确认 RitsuLib v0.106 compatible 可用，下一个 goal 才是：

```text
GOAL-2026-05-26-RITSULIB-POC-LOAD-LIFECYCLE-DATASTORE
```

---

# 最终判断

Codex 这次报告里的“Migration plan implemented”是**过度表述**。

严格按 GitHub main：

```text
FeatureRegistry scaffold：部分实现。
RitsuLib migration：未实现。
RitsuLib dependency：未添加。
RitsuLib runtime dependency：未添加。
RitsuLib POC：未完成。
Migration docs：未在 main 可见。
PR5 blocker：逻辑合理，但需要正式落地到 docs/issues。
```

所以下一个 goal 应该是 **RitsuLib 兼容性决策 + 分支/文档落地**，不是继续声称 migration 已完成。
