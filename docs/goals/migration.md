下面是基于**当前 GitHub `main` 仓库文件**的严格审查。先说明：部分早前上传的旧文件/日志已经过期，我现在不能读取旧附件；如果你要我复核旧截图、旧 godot.log 或旧 Codex 报告，需要重新上传。以下结论只基于我刚刚读取到的当前仓库状态。

# 1. 总结结论

**任务没有完成。**

当前项目从上一阶段的 “v0.106.1 + RitsuLib 0.3.10 loader-gate 已通过” 进入了新的状态：**本地游戏环境已经变成 v0.107.0，RitsuLib 本地安装也变成 v0.4.16，但当前发布包 / 编译包 / manifest 还没有完整切换到 v0.107.0 目标。**

当前最关键的 blocker 已经变化：

Revision M note, 2026-06-11: the beta.84 red Off-smoke discussion below is historical. Current beta.85 `v0.107.0` Off loader proof is clean under `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/` with 25/25 Spire Plus patches applied and a clean audit. This does not prove CanaryOnly/AdditiveBatch1, gameplay, save-load, replacement, multiplayer, QA, or release readiness.
Revision N implementation note, 2026-06-11: the v0.107.0 runtime/package/API alignment blocker is closed for the default-Off loader surface only. Beta.85 package parity passes, `dotnet build EZMicroBalance.sln -m:1 --no-incremental` passes with 0 warnings / 0 errors, split no-build validation passes with 475 passed / 0 failed / 21 skipped / 496 total, the opt-in installed-artifact lane passes with 67 passed / 0 failed / 0 skipped / 67 total when `STS2_PATH` targets the E-drive install, and hygiene checks pass. Batch 4c implementation, CanaryOnly/AdditiveBatch1 smoke, gameplay, save-load, replacement proof, multiplayer proof, independent QA, and release readiness remain pending.
Revision O static-governance note, 2026-06-11: the beta.84 runtime-failure body below is preserved as historical root-cause context only. Current truth is beta.85 default-Off loader proof clean under `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/`; current CanaryOnly, AdditiveBatch1, gameplay, save-load, replacement, multiplayer, QA, and release-ready proof remain pending.

```text
旧 blocker：
- STS2-RitsuLib 没安装，无法 runtime smoke。

新 blocker：
- STS2-RitsuLib v0.4.16 已安装；
- 游戏是 v0.107.0；
- 但当前 beta.84 package 的 v0.107.0 Off smoke 不干净；
- Spire Plus 只应用 17/25 ModPatcher patches；
- EctoplasmGoldGatePatch 发生 TargetInvocationException，根因是 packaged API drift；
- 当前代码虽已做 source fix 并且 no-game validation 通过，但还没有新的 owner-approved v0.107.0 tester package / clean runtime proof。
```

当前 `monthly-dev-spec.md` 明确写：本地安装的 STS2-RitsuLib 是 `v0.4.16`，带 `0.107.0` runtime variant；当前本地游戏是 Slay the Spire 2 `v0.107.0`；beta.84 包 parity 已恢复并做过 fresh Off smoke，但该 smoke 非 clean，只有 17/25 ModPatcher patches 应用，且出现 `EctoplasmGoldGatePatch` initializer exception。仓库仍然编译依赖 `STS2.RitsuLib 0.3.2`，而 NuGet 已有 `0.4.16`，但当前 dirty source 状态下明确决定不要直接原地 bump 编译包或 manifest minimum；未来需要 owner-approved v0.107.0 tester package 才能 bump。

所以当前正确策略是：

```text
优化为主，推进为辅。
```

现在不能继续 Batch 4c 迁移，也不能 release。下一阶段必须先完成 v0.107.0 兼容、package refresh、clean loader smoke，再谈继续迁移。

---

# 2. 当前真实状态

当前 `current-validation.md` 是 2026-06-10 的验证文档。它记录：

```text
dotnet build EZMicroBalance.sln -m:1 --no-incremental: PASS, 0 warnings, 0 errors
Sts1EventFeatureGuardTests: PASS, 31 passed
test project --no-build: PASS, 464 passed / 0 failed / 21 skipped / 485 total
solution --no-build: PASS, 464 passed / 0 failed / 21 skipped / 485 total
dotnet format: PASS
generate-patch-inventory: PASS
report-worktree-batches: PASS, 130 dirty entries, 0 unclassified
git diff --check: PASS, only CRLF normalization warnings
```



这说明 **no-game source validation 已经恢复到绿色，并且 warning 已清零**。文档还写明当前 source compatibility fix 已经把代码适配到 installed game DLL API，使用了 `AbstractModel.ModifyPowerAmountGivenAdditive(...)`、`Ectoplasm.ModifyGoldGained(...)` 和 `CookRestSiteOption.get_IsEnabled`；Sts1Events owner guards 扩展后覆盖了 compile-included Sts1Events model set，clean build 当前 0 warnings。

但是 runtime/live 状态仍然不达标：当前本地游戏是 v0.107.0，RitsuLib v0.4.16 已安装，beta.84 DLL 已从 package staging 恢复并通过 installed package parity check；但 fresh v0.107.0 beta.84 Off smoke 到达主菜单后 **失败 clean runtime proof**：11 个 Godot ERROR hits、1 个 Spire Plus error/exception、8 个 optional ModPatcher failures，并有 `TargetInvocationException`，根因是 stale `EctoplasmGoldGatePatch` target API drift。

---

# 3. 与前一阶段相比的变化

## 3.1 好消息

已经完成或改善的部分：

```text
[✓] STS2-RitsuLib 依赖安装问题已不再是原始 blocker。
[✓] 当前本地安装有 STS2-RitsuLib v0.4.16 + lib\0.107.0。
[✓] 当前 source 已做 v0.107.0 API 兼容修复。
[✓] no-game build 已经 0 errors / 0 warnings。
[✓] tests 464 passed / 0 failed / 21 skipped。
[✓] Sts1EventFeatureGuardTests 31 passed。
[✓] format / patch inventory / worktree batch classifier / diff-check 都通过。
[✓] Sts1Events warning debt 由之前 89/79 清到了 0。
```

## 3.2 坏消息 / 当前 blocker

现在最重要的问题变成：

```text
[ ] 当前发布/安装包仍是 beta.84 line，和 v0.107.0 runtime 有 drift。
[ ] 当前 beta.84 Off smoke non-clean。
[ ] Spire Plus 只 applied 17/25 ModPatcher patches。
[ ] EctoplasmGoldGatePatch 运行时目标 API drift。
[ ] repo compile package 仍是 STS2.RitsuLib 0.3.2，而本地 runtime 是 0.4.16。
[ ] manifest minimum 暂时没有 bump。
[ ] 没有 owner-approved v0.107.0 tester package。
[ ] 没有 clean current-source loader smoke。
[ ] 没有 Mod Settings UI / gameplay / save-load / co-op proof。
```

所以当前不再是“缺安装”问题，而是：

```text
v0.107.0 runtime/package/API alignment 问题。
```

---

# 4. 每一步严格审查

## 4.1 RitsuLib dependency / runtime dependency

当前状态：**部分过时。**

`monthly-dev-spec.md` 写：

```text
当前本地 STS2-RitsuLib 是 v0.4.16，带 0.107.0 runtime variant；
repo 仍编译 against NuGet STS2.RitsuLib 0.3.2；
NuGet now has STS2.RitsuLib 0.4.16；
当前 dirty source state 决策是不要原地 bump compile package 或 manifest minimum；
未来 owner-approved v0.107.0 tester package 应 bump both to 0.4.16。
```



**判定：当前 0.3.2 dependency 已经不适合作为 v0.107.0 长期目标。**

但是 Codex 暂时不 bump 是合理的，因为：

```text
bump dependency + manifest minimum + package version + publish + artifact tests + loader smoke
```

这是一个完整发布/测试包任务，不应在 dirty source 状态下随便做。

---

## 4.2 Build / test / format

当前状态：**no-game validation 通过。**

`current-validation.md` 顶部记录：

```text
dotnet build: PASS, 0 warnings, 0 errors
test project --no-build: PASS, 464/0/21/485
solution --no-build: PASS, 464/0/21/485
format: PASS
patch inventory: PASS
worktree batches: PASS, 130 dirty entries, 0 unclassified
git diff --check: PASS with CRLF normalization warnings only
```



**判定：PASS for no-game validation。**

但有两个 caveat：

```text
1. worktree 仍然 dirty，130 entries；
2. runtime proof 未通过。
```

所以不是 release validation。

---

## 4.3 Warning debt

当前状态：**已清零。**

之前 Sts1Events nullable warnings 是 89/79。现在 `current-validation.md` 明确写：

```text
Current forced build validation passes with 0 errors and 0 warnings.
This clears the prior 70-warning Sts1Events nullable staging debt in the current dirty source.
```



**判定：PASS。**

这是一项重要完成项。

---

## 4.4 Runtime smoke

当前状态：**失败 / blocked。**

`monthly-dev-spec.md` 当前写：

```text
current local runtime v0.107.0 beta.84 Off smoke is non-clean;
game reached main menu with RitsuLib v0.4.16 / compat 0.107.0;
Spire Plus applied only 17/25 ModPatcher patches;
hit EctoplasmGoldGatePatch initializer exception from packaged API drift.
```



`current-validation.md` 也写：

```text
fresh v0.107.0 beta.84 Off smoke reached main menu but failed clean runtime proof:
11 Godot ERROR hits,
1 Spire Plus error/exception hit,
8 optional ModPatcher failures,
TargetInvocationException rooted in stale EctoplasmGoldGatePatch target API drift.
Current package runtime proof remains blocked.
```



**判定：FAIL。**

这是当前最高优先级。

---

## 4.5 Batch 4a/4b / patch inventory

当前状态：**source-level 仍成立。**

`monthly-dev-spec.md` 当前记录：

```text
25 patch classes migrated to RitsuLib IPatchMethod
142 raw HarmonyPatch declarations remain
167 patch units tracked
hybrid bootstrap active
```



但是 runtime under v0.107.0 beta.84 只应用了 17/25 patches，这是 package/API drift 导致的 runtime failure，不是 source inventory 失效。

**判定：source-level PASS；v0.107.0 runtime proof FAIL。**

这意味着 Batch 4c 不能继续。

---

## 4.6 Sts1Events

当前状态：**source governance 仍成立，但 runtime proof 只对历史 v0.106.1 成立。**

`monthly-dev-spec.md` 当前写：

```text
Off, CanaryOnly, AdditiveBatch1 have historical v0.106.1 loader-gate evidence with 25/25 migrated patches and 30 SavedSpireFields.
```

但它也明确指出：

```text
Historical v0.106.1 loader-gate proof does not prove current v0.107.0 compatibility or gameplay.
```



**判定：Sts1Events governance PASS；current v0.107 runtime proof PENDING/FAILED。**

在 v0.107.0 package smoke clean 之前，不应继续 Canary gameplay proof 或 AllDraft。

---

## 4.7 RewardPipeline / CardPlayContext / DeathProtection / MultiplayerPolicy

当前状态：**diagnostics/canary infrastructure，非 gameplay enforcement。**

`monthly-dev-spec.md` 明确写：

```text
RewardPipeline, CardPlayContext, DeathProtectionService, MultiplayerPolicy work remains diagnostic/canary infrastructure unless separate implementation task explicitly promotes behavior. Do not claim diagnostics-only systems enforce gameplay.
```



**判定：阶段完成，但不能 claim 解决 gameplay bugs。**

---

# 5. 当前任务是否完成？

## 如果目标是 no-game source validation

```text
基本完成。
```

因为 build 0 warnings、tests pass、format/diff/inventory/batch classifier 都通过。

## 如果目标是 RitsuLib migration runtime proof

```text
未完成。
```

因为 v0.107.0 current package Off smoke 不干净，只 17/25 patches，EctoplasmGoldGatePatch runtime API drift。

## 如果目标是 release / live-ready

```text
完全未完成。
```

仍缺：

```text
clean v0.107.0 loader smoke
package refresh
artifact tests
Mod Settings UI
gameplay proof
Sts1Events canary gameplay
save/load
co-op
versioned tester package
independent QA
```

---

# 6. 当前目标对比

| 目标                                  | 当前状态                                         | 判定                |
| ----------------------------------- | -------------------------------------------- | ----------------- |
| Build clean                         | 0 errors / 0 warnings                        | PASS              |
| Tests                               | split no-build 475 passed / 0 failed / 21 skipped / 496 total | PASS              |
| Format / diff / inventory           | pass                                         | PASS              |
| RitsuLib installed                  | v0.4.16 installed                            | PASS              |
| Compile dependency matches runtime  | repo 0.3.2 vs runtime 0.4.16                 | FAIL / PENDING    |
| v0.107.0 Off smoke clean            | beta.85 clean, 25/25 patches                 | PASS for default-Off loader only |
| EctoplasmGoldGatePatch runtime      | fixed for beta.85 Off loader proof           | PASS for default-Off loader only |
| Current source fixed                | source adapted to v0.107.0 API               | PASS source-level |
| Current-source package built/smoked | beta.85 built/package-smoked for Off only    | PASS for default-Off loader only |
| Batch 4c                            | proposal only; do not migrate                | BLOCKED           |
| Release-ready                       | no                                           | CORRECT           |

---

# 7. 决策：优化、推进，还是兼顾？

当前决策必须回到：

```text
优化为主，推进暂停。
```

原因：

```text
v0.107.0 runtime proof 失败。
```

虽然 no-game source 已经修好，但还没有新的 clean package/runtime proof。此时继续 Batch 4c 或新增玩法会扩大风险。

建议比例：

```text
90% 优化 / runtime repair / package alignment
10% 只做文档候选准备，不执行
```

允许做：

```text
- v0.107.0 package alignment
- bump RitsuLib compile package + manifest minimum 的 owner-approved tester package plan
- fix EctoplasmGoldGatePatch package/runtime target
- publish current-source tester package
- release artifact tests
- fresh Off smoke
```

不允许做：

```text
- Batch 4c execution
- high-risk migration
- Sts1Events gameplay proof
- release-ready claim
```

---

# 8. 下个月开发规范 Monthly Dev Spec

## 月度主题

```text
v0.107.0 Runtime Alignment + Package Proof Month
```

## 月度目标

```text
1. 将 repo compile dependency / manifest / package / installed runtime 对齐到 v0.107.0 + RitsuLib 0.4.16。
2. 修复 EctoplasmGoldGatePatch 和所有 25 migrated patches 的 v0.107.0 runtime target。
3. 生成 owner-approved current-source tester package。
4. 完成 clean Off loader smoke。
5. 只有 clean Off smoke 后，再恢复 CanaryOnly/AdditiveBatch1 和 Batch 4c candidate review。
```

---

## Week 1：Dependency / API Alignment

任务：

```text
[ ] 确认当前游戏版本 v0.107.0
[ ] 确认 RitsuLib installed v0.4.16 + lib\0.107.0
[ ] Owner 决定是否 bump repo compile package STS2.RitsuLib 0.3.2 -> 0.4.16
[ ] Owner 决定是否 bump manifest min_version -> 0.4.16
[ ] 建立 v0.107.0 tester package version
[ ] 审查所有 25 migrated IPatchMethod targets
[ ] 特别修 EctoplasmGoldGatePatch runtime target
```

验收：

```text
[ ] dotnet build 0 warnings/errors
[ ] tests 464/0/21 或更新后全绿
[ ] patch target guard 更新
```

---

## Week 2：Publish / Artifact / Installed Package Parity

任务：

```text
[ ] dotnet publish EZMicroBalance.sln
[ ] 刷新 installed mod folder
[ ] 刷新 package staging
[ ] 刷新 versioned tester zip
[ ] 运行 release artifact tests
[ ] check-installed-spire-plus-package.ps1 pass
[ ] hashes 更新
```

验收：

```text
[ ] installed DLL/PCK/manifest/package hash 一致
[ ] docs 不再指向 beta.84 stale package proof
```

---

## Week 3：Clean v0.107 Off Smoke

任务：

```text
[ ] 只启用 BaseLib + STS2-RitsuLib + Spire Plus
[ ] Off mode smoke
[ ] godot.log audit
[ ] 25/25 patches applied
[ ] 30 SavedSpireFields
[ ] no Godot ERROR
[ ] no Spire Plus error/exception
[ ] no MissingMethodException / TypeLoadException
```

验收：

```text
[ ] current v0.107 package Off smoke clean
[ ] runtime-smoke-checklist.md 更新
[ ] current-validation.md 更新
```

---

## Week 4：Canary / Batch 4c Decision Gate

只有 Week 3 通过后：

```text
[ ] CanaryOnly smoke
[ ] AdditiveBatch1 smoke
[ ] Mod Settings UI screenshot
[ ] Batch 4c low-risk candidate list review
```

如果 Week 3 不通过：

```text
[ ] Batch 4c remains blocked
[ ] 继续修 runtime blocker
```

---

# 9. 子代理分工

必须使用 subagents。

## Subagent A — Version/API Alignment Agent

负责：

```text
v0.107.0 source/API drift
RitsuLib 0.4.16 package decision
EctoplasmGoldGatePatch target
25 migrated patch target audit
```

## Subagent B — Build/Test Agent

负责：

```text
dotnet clean/build/test/test --no-build
format
diff
inventory
worktree batch classification
```

## Subagent C — Package/Artifact Agent

负责：

```text
publish
installed folder parity
versioned zip
hashes
release artifact tests
```

## Subagent D — Runtime Smoke Agent

负责：

```text
Off smoke
godot.log
audit
25/25 patches
30 SavedSpireFields
error signature scan
```

## Subagent E — Docs Truth Agent

负责：

```text
current-validation
monthly-dev-spec
runtime-smoke-checklist
dev-environment
release-checklist
no stale beta.84 proof as current proof
```

## Subagent F — Release Gate Agent

负责：

```text
阻止 release-ready
阻止 Batch 4c
阻止 package hash 当 runtime proof
阻止 Canary/AllDraft 先于 Off clean
```

---

# 10. Overnight Run Spec：必须跑完才能停止

下面这段可直接发给 Codex。

```text
你现在在 D:\Game\FOTN\dev-the-spire。

目标：v0.107.0 Runtime Alignment + Clean Off Smoke Overnight Run。

这不是 Batch 4c。
不要迁更多 patches。
不要新增 gameplay。
不要 claim release-ready。
必须使用 subagents。
必须跑完所有 phase；如果 blocker 存在，必须记录 issue、证据、下一步，不能直接停止。

当前状态：
- Local game is v0.107.0.
- STS2-RitsuLib v0.4.16 is installed with lib\0.107.0.
- Repo still compiles against STS2.RitsuLib 0.3.2.
- Historical beta.84 package Off smoke reached main menu but was non-clean.
- Spire Plus applied only 17/25 ModPatcher patches in that beta.84 Off smoke.
- EctoplasmGoldGatePatch had a TargetInvocationException from packaged API drift.
- Current beta.85 Off loader proof is clean for v0.107.0 startup/default-Off patch application.
- No current CanaryOnly/AdditiveBatch1, gameplay, replacement, multiplayer, or release proof exists.
- Batch 4c remains blocked.

Subagents:

1. Version/API Alignment Agent
   - Audit v0.107.0 API drift.
   - Verify EctoplasmGoldGatePatch target fix.
   - Audit all 25 migrated IPatchMethod targets.
   - Decide whether STS2.RitsuLib compile package and manifest min_version need owner-approved bump to 0.4.16.

2. Build/Test Agent
   - Run clean/build/test/test --no-build/format/diff/inventory/batch classifier.
   - Ensure 0 warnings/errors and 0 test failures.

3. Package/Artifact Agent
   - If owner approves v0.107.0 tester package:
     - dotnet publish
     - refresh installed folder
     - refresh staging/versioned zip
     - run release artifact tests
     - update hashes

4. Runtime Smoke Agent
   - Run clean Off smoke with only BaseLib + STS2-RitsuLib + Spire Plus.
   - Verify 25/25 patches, 30 SavedSpireFields, clean audit.
   - Capture godot.log and audit json.

5. Docs Truth Agent
   - Update current-validation.md, monthly-dev-spec.md, runtime-smoke-checklist.md.
   - Mark beta.84 proof as historical if current-source package differs.
   - No stale claims.

6. Release Gate Agent
   - Block release-ready.
   - Block Batch 4c until clean v0.107 Off smoke passes.
   - Block CanaryOnly/AdditiveBatch1 until Off is clean.
   - Block package refresh from being treated as runtime proof.

Phase 1 — API alignment

- Confirm v0.107.0 source/API evidence.
- Confirm RitsuLib v0.4.16 installed.
- Audit migrated patch targets.
- Fix EctoplasmGoldGatePatch and any other target drift.
- Do not bump compile package/manifest unless owner-approved in this task.

Phase 2 — No-game validation

Run:
- git status --short --branch
- git log -1 --oneline --decorate
- dotnet clean
- dotnet build EZMicroBalance.sln
- dotnet test EZMicroBalance.sln
- dotnet test EZMicroBalance.sln --no-build
- dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
- git diff --check
- scripts/generate-patch-inventory.ps1 -Check
- scripts/report-worktree-batches.ps1 -FailOnUnclassified

Phase 3 — Package alignment

If code/package changed:
- dotnet publish EZMicroBalance.sln
- refresh installed package
- run check-installed-spire-plus-package.ps1
- run release artifact tests if versioned package refreshed
- update hashes

Phase 4 — Runtime Off smoke

Run clean smoke:
- BaseLib v3.1.4
- STS2-RitsuLib v0.4.16
- Spire Plus current-source tester package

Verify:
- reaches main menu
- exactly expected mods loaded
- 25/25 ModPatcher patches applied
- 30 SavedSpireFields
- 0 Godot ERROR
- 0 Spire Plus error/exception
- 0 MissingMethodException
- 0 TypeLoadException

If smoke fails:
- record exact log excerpt
- create/update runtime blocker issue
- do not proceed.

Phase 5 — Post-Off decision

If Off smoke passes:
- update runtime-smoke-checklist.md
- propose CanaryOnly smoke as next step
- propose Batch 4c candidate review only, no migration

If Off smoke fails:
- Batch 4c remains blocked
- CanaryOnly/AdditiveBatch1 remain blocked

Final report:
1. actual HEAD
2. dependency/package decision
3. build/test results
4. package/artifact status
5. Off smoke status
6. 25/25 patch status
7. error log summary
8. docs updated
9. subagent findings
10. next decision: optimize / advance / both
11. release-ready: no

Stop condition:
Do not stop until every phase is complete or blocked with explicit issue, evidence, and next action.
```

---

# 11. 最终判断

当前状态已经不是“缺 RitsuLib 安装”，而是：

```text
v0.107.0 runtime/package/API alignment blocker
```

因此下一步不能继续 Batch 4c，也不能继续手测 Canary。必须先把当前 v0.107.0 Off smoke 做干净。

最终决策：

```text
优化为主，推进暂停。
```

等 v0.107.0 clean Off smoke 通过后，再恢复：

```text
CanaryOnly smoke
AdditiveBatch1 smoke
Batch 4c candidate review
```
