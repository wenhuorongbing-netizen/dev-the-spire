先说明：之前部分上传文件已经过期，我现在不能再读取旧附件内容。如果你要我复核旧 log、旧截图或旧 Codex 报告，需要重新上传。下面这份审查只基于 **当前 GitHub `main` 仓库状态**、你这轮给出的 “Still Blocked” 摘要、以及我刚刚重新读取的仓库文件。

# 1. 严格总评

**任务没有完成，但方向是正确的。**

当前最准确状态是：

```text
RitsuLib migration：进行中
Batch 4a/4b source-level closure：完成
Architecture canary / diagnostics layer：部分完成
Sts1Events governance：source-level 完成，runtime 未证明
Runtime smoke：已尝试但未通过有效 Spire Plus proof
Batch 4c：必须继续 blocked
Release-ready：否
```

当前战略应是：

```text
继续优化为主，有限推进为辅。
```

不能继续无条件推进 Batch 4c，也不能新增 gameplay，也不能说 runtime-safe / live-ready / release-ready。当前 STS2-RitsuLib 已在 E-drive 本地安装，当前最强 loader evidence 已能到主菜单并加载 BaseLib、RitsuLib、Spire Plus，但 audit 不是 clean：仍有 11 个 Godot ERROR hits，包括 `ritsulib-variants.json` manifest parsing 和 optional Spire Plus ModPatcher failures。补充的 Revision J retry 也无效：直接启动失败于 Steam init，Steam `-applaunch` 只证明 RitsuLib/BaseLib 加载，`EZMicroBalance` 被 settings 跳过。因此只能继续做低风险架构、文档真相、测试稳定性和治理工作。

---

# 2. 当前 GitHub 状态校准

远程 `main` 的最新提交已经是：

```text
6b149ba — sprint 2
```

它位于 `8782030 sprint 1` 之后。

这很重要，因为当前 `docs/reviews/current-validation.md` 仍记录的是：

```text
HEAD: 87820303
Worktree: dirty before and after this pass
Runtime smoke: hard blocked
```



也就是说，**当前 validation 文档已经不是最新 HEAD 的完整验证记录**。它对 `87820303` 的状态有效，但不能直接当作 `6b149ba` 的最终绿灯。

另外，你贴的 “Still Blocked / No commit or push was performed” 更像是某次本地 pass 的摘要；但 GitHub 远程现在确实已有新的 `sprint 2` 提交。因此当前审核应以远程最新状态为准，而不是只按那段本地总结判断。

---

# 3. 当前验证状态逐步审查

## 3.1 Build / Test / Format / Diff

`docs/reviews/current-validation.md` 已新增 Revision J 记录，基线是 `6b149ba0`：

```text
dotnet clean：PASS
dotnet build EZMicroBalance.sln：PASS，0 errors，89 warnings
dotnet test EZMicroBalance.sln：PASS，464 passed / 0 failed / 21 skipped / 485 total
dotnet test EZMicroBalance.sln --no-build：PASS，464 passed / 0 failed / 21 skipped / 485 total
dotnet format：PASS
git diff --check：初次发现 4 个 trailing whitespace，已修复，需 final rerun
```



它还记录了测试主机曾经崩溃、stale `testhost` 锁文件干扰、最终通过清理 stale PIDs 和降低并行后跑通。

**判定：部分完成。**

理由：

```text
[✓] 87820303 的 no-build test 通过。
[✓] 6b149ba0 的 clean/build/full-test/no-build-test/format 已重放。
[✓] testhost 干扰被记录并处理；本轮测试未出现 assertion failure。
[ ] current-validation 仍正确记录 worktree dirty。
[ ] 仍有 89 warnings。
```

下一步必须完成 final rerun 并保持最新 HEAD 记录：

```powershell
git status --short --branch
git log -1 --oneline --decorate
dotnet clean
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

并保持 `docs/reviews/current-validation.md` 与 final rerun 一致。

---

## 3.2 Build warnings

当前 clean build warning truth 是：

```text
89 warnings
codes: CS8602, CS8604, CS8625
scope: EZMicroBalanceCode/Sts1Events/Models/
```

并且文档明确说这些 warnings 是 issue-worthy，只是因为 Sts1Events 默认 Off、仍是 prototype/dev-only，所以暂时接受。

**判定：未完成。**

当前可以暂时接受，但不能长期保留。只要 Sts1Events 要进入 CanaryOnly runtime proof，至少 CanaryOnly 涉及的 event path 应逐步做到 warning-free 或明确 guarded。

---

## 3.3 Runtime smoke

这是当前最大 blocker。

Revision J `current-validation.md` 记录：

```text
E:\Steam...\Slay the Spire 2 和 mods 存在
E:\Steam...\mods\BaseLib 存在
E:\Steam...\mods\EZMicroBalance 存在
E:\Steam...\mods\STS2-RitsuLib 存在，version 0.3.10，compat branch 0.106.1
D:\Steam... 路径不存在
fresh godot.log 已捕获；当前最强 evidence 到达主菜单并加载 Spire Plus，但 audit 非 clean；补充 retry 中 EZMicroBalance 被 settings 跳过
```

因此决策是：

```text
Hard Block Stop
Batch 4c remains blocked
Off=0 / CanaryOnly=4 / runtime safety / release-readiness not claimed
```



`next-overnight-run.md` 同样写明：runtime smoke 是 critical path blocker，Batch 4c 不能继续。当前下一步不是安装 STS2-RitsuLib，而是处理 controlled-loader audit errors，并修正 supplemental live-session isolation/settings，让 BaseLib + STS2-RitsuLib + Spire Plus 三者同时启用并通过 clean loader smoke。

**判定：未完成 / hard blocked。**

没有 clean Spire Plus loader `godot.log`，不能说：

```text
RitsuLib runtime-safe
ModPatcher runtime-safe
Off=0 runtime-proven
CanaryOnly=4 runtime-proven
Batch 4c-ready
release-ready
```

---

# 4. RitsuLib migration 审查

## 4.1 依赖和 hybrid bootstrap

RitsuLib dependency 和 manifest dependency 已经存在；当前 migration spec 也明确：

```text
25 patches migrated to RitsuLib IPatchMethod
142 raw Harmony declarations remaining
tracked patch units total = 167
hybrid bootstrap active
```



**判定：阶段性完成。**

但这只是 RitsuLib migration 的一部分。还没有完成：

```text
RitsuLib lifecycle migration
RitsuLib DataStore migration
RitsuLib settings migration
RitsuLib content pack migration
full ModPatcher migration
```

---

## 4.2 Batch 4a/4b patch closure

当前 monthly spec 记录：

```text
25 patches migrated
142 raw Harmony declarations
tracked patch units total = 167
Batch 4c, high-risk migration blocked until runtime smoke passes
```



**判定：Batch 4a/4b source-level closure 完成。**

但继续迁 patch 仍必须 blocked：

```text
Batch 4c 只有 runtime smoke pass 之后才允许评估。
```

---

## 4.3 Patch inventory / double-patch guard

当前 monthly spec 说明 double-patch guard 已完成 source-level 检查，migrated patches 位于 RitsuLib namespace 并实现 `IPatchMethod`，raw Harmony patches 保留 `[HarmonyPatch]`。

**判定：source-level 完成；runtime-level 未证明。**

下一步不是继续迁 patch，而是先让 game runtime 证明 ModPatcher 没有启动失败、TypeLoadException、MissingMethodException 或重复 patch 行为。

---

# 5. Sts1Events 审查

当前 Sts1Events issue 明确写：

```text
Open — governance hardened, content incomplete.
Default Off is safe.
CanaryOnly and AdditiveBatch1 are controlled source-test modes.
AdditiveAllDraft and ReplaceUnknownEventsPrototype are dev-only/unsafe.
```



当前 5 模式：

```text
Off：0 registrations，safe
CanaryOnly：4 registrations / 4 event types
AdditiveBatch1：11 registrations / 10 event types
AdditiveAllDraft：54 calls / 47 unique events，unsafe/dev-only
ReplaceUnknownEventsPrototype：debug-only / unsafe
```



风险表已列出：

```text
Dead Adventurer：combat path no-op
Scorpion Nest：combat path no-op
Treasure Ooze：combat path no-op
Masked Bandits：fight path no-op
Mind Bloom：WAR option blocked/no-op
Mysterious Sphere：combat path no-op
N'loth：relic-select no-op
Vampires：no Bite cards
```



**判定：治理完成，内容未完成。**

正确状态是：

```text
Default Off 只有 source-level fail-closed/source-guard evidence；Off=0 runtime proof 仍缺 clean Spire Plus `godot.log`。
CanaryOnly 只能进入 runtime-proof attempt；CanaryOnly=4 尚未 runtime-proven。
AdditiveBatch1 是 controlled prototype。
AdditiveAllDraft 需要 `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`，不能用于 tester/release path。
ReplaceUnknownEventsPrototype 需要 `REPLACEMENT_PROTOTYPE_ENABLED` + `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`，normal build fail-closed，不能用于 tester/release path。
```

---

# 6. FeatureRegistry 审查

当前 monthly spec 记录：

```text
IFeatureModule metadata
FeatureBootstrapRecord
LiveStatus enum
truthy env overrides before bootstrap record creation
```



当前 validation 也记录架构状态：RewardPipeline diagnostics 已接入 FeatureRegistry bootstrap events，ArchitectureCanaryBootstrap 注册 FeatureRegistry 和 reward diagnostics，multiplayer policy records 也被注册。

**判定：第一层 hardening 完成。**

但还不是完整治理系统。仍缺：

```text
feature dependency graph
runtime evidence status
settings/UI status export
real multiplayer policy enforcement
```

---

# 7. UrdaStateCodec / RewardPipeline / CardPlayContext / DeathProtection / MultiplayerPolicy

## 7.1 UrdaStateCodec

Monthly spec 记录：

```text
UrdaStateCodec V1 encode/decode/legacy compat
current full positional decode
legacy full positional decode
null-string encode behavior
edge cases
```



**判定：第一层完成。**

它仍然是 SavedSpireField string bridge，不是 RitsuLib DataStore migration。

---

## 7.2 RewardPipeline

Current validation 记录：

```text
RewardPipeline diagnostics wired into FeatureRegistry bootstrap events
low-risk AscensionRewardService reward/card-reward surfaces as no-mutation diagnostics
```



**判定：canary 完成，不是完整 reward pipeline。**

---

## 7.3 CardPlayContext

Current validation 记录：

```text
Lotha extra-play paths touch CardPlayContextCanary through a single-depth adapter that returns Allow; play counts and gameplay branches unchanged.
```



**判定：canary 完成，不是完整 extra-play governance。**

---

## 7.4 DeathProtectionService

Current monthly spec 记录：

```text
DeathProtectionService diagnostics-only, provider-testable service, not wired into gameplay death prevention
```



**判定：stub 完成，不是 Lotha DeathReprieve fix。**

---

## 7.5 MultiplayerPolicy

Current monthly spec 记录：

```text
MultiplayerPolicy diagnostics-only registry/taxonomy with active-system records and co-op evidence metadata; not gameplay enforcement.
```



**判定：taxonomy 完成，不是 multiplayer safety proof。**

---

# 8. 当前任务是否完成？

按目标逐项判定：

| 目标                                | 状态                                    | 结论         |
| --------------------------------- | ------------------------------------- | ---------- |
| RitsuLib dependency               | 已完成                                   | PASS       |
| Batch 4a/4b source migration      | 已完成                                   | PASS       |
| Double-patch source guard         | 已完成                                   | PASS       |
| Current validation                | 有记录，但 stale relative to latest commit | PARTIAL    |
| Runtime smoke                     | 未完成                                   | HARD BLOCK |
| Sts1Events default safety         | 已完成                                   | PASS       |
| Sts1Events content                | 未完成                                   | OPEN       |
| FeatureRegistry hardening         | 第一层完成                                 | PARTIAL    |
| UrdaStateCodec                    | 第一层完成                                 | PARTIAL    |
| RewardPipeline/CardPlayContext    | canary 完成                             | PARTIAL    |
| DeathProtection/MultiplayerPolicy | diagnostics-only                      | PARTIAL    |
| Batch 4c                          | 阻塞                                    | CORRECT    |
| Release-ready                     | 否                                     | CORRECT    |

**总判定：未完成。**

---

# 9. 当前决策：优化、推进，还是两者兼顾？

**结论：优化为主，有限推进为辅。**

理由：

```text
Runtime smoke 未完成；
latest HEAD 没有 canonical validation；
Sts1Events still prototype/dev-only；
architecture canary 还未形成真实 enforcement；
high-risk patch migration 仍不安全。
```

建议比例：

```text
85% 优化 / 验证 / 稳定化
15% 有限推进 / canary integration
```

当前不允许：

```text
Batch 4c
high-risk migration
Sts1Events AllDraft live
new gameplay
release-ready claim
```

当前允许：

```text
runtime smoke
validation truth
Sts1Events CanaryOnly runtime proof
FeatureRegistry runtime log
RewardPipeline/CardPlayContext no-behavior diagnostics
DeathProtection/MultiplayerPolicy provider/policy records
```

---

# 10. 下个月开发规范 Monthly Dev Spec

## 月度主题

```text
Runtime Proof + Governance Closure Month
```

## 月度目标

```text
1. 完成 BaseLib + STS2-RitsuLib + Spire Plus runtime smoke。
2. 统一最新 HEAD 的 validation truth。
3. 完成 Sts1Events Off / CanaryOnly runtime proof。
4. 继续把 architecture canary 接入低风险真实 surface。
5. 维持 Batch 4c blocked，除非 runtime smoke passed。
```

---

## Week 1：Latest HEAD Validation + Runtime Smoke

任务：

```text
[ ] git status --short --branch
[ ] git log -1 --oneline --decorate
[ ] dotnet clean
[ ] dotnet build EZMicroBalance.sln
[ ] dotnet test EZMicroBalance.sln
[ ] dotnet test EZMicroBalance.sln --no-build
[ ] dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
[ ] git diff --check
```

更新：

```text
docs/reviews/current-validation.md
```

必须记录：

```text
actual HEAD
worktree status
build warnings
test passed/failed/skipped
format result
diff result
publish/package status
```

runtime smoke：

```text
[x] 安装 STS2-RitsuLib 到 E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib（v0.3.10）
[x] 确认 BaseLib
[x] 确认 EZMicroBalance / Spire Plus 文件夹存在
[!] Controlled loader 已加载 BaseLib + STS2-RitsuLib + Spire Plus 并到达主菜单，但 audit 仍有 11 个 Godot ERROR hits；补充 retry 仍发现 stale/duplicate mod manifest rows，且 `EZMicroBalance` 被 settings 跳过
[x] 启动 Steam 客户端 / `-applaunch 2868840`
[x] 捕获 godot.log
[x] 运行 audit-godot-log；结果非 clean
```

验收：

```text
[x] RitsuLib bootstrap starting
[ ] Spire Plus ModPatcher applied 25 patches cleanly（未证明；controlled loader 有 optional ModPatcher failures）
[x] RitsuLib framework active
[x] BaseLib initialized
[x] Spire Plus initialized in controlled loader log
[x] no MissingMethodException
[x] no TypeLoadException
[ ] no manifest dependency failure / audit clean（未通过；controlled loader audit 有 11 个 Godot ERROR lines）
```

---

## Week 2：Sts1Events Runtime Gates

任务：

```text
[ ] Off mode：runtime proof 0 registrations
[ ] CanaryOnly：runtime proof exactly 4 registrations
[ ] 若可行，debug-spawn 4 canary events
[ ] Canary event completion save/load smoke
[ ] EN/ZHS render proof
```

继续保持：

```text
AdditiveBatch1 = controlled prototype
AdditiveAllDraft = unsafe/dev-only
ReplaceUnknownEventsPrototype = unsafe/debug-only
```

验收：

```text
[ ] Off / CanaryOnly 从 source-proof 变成 runtime-proof
[ ] AllDraft 继续 blocked from tester/release path
```

---

## Week 3：Architecture Runtime Diagnostics

任务：

```text
[ ] FeatureRegistry summary 出现在 runtime log
[ ] RewardPipeline diagnostics 出现在 runtime log
[ ] CardPlayContextCanary 出现在 low-risk path log
[ ] DeathProtectionService no-op provider evidence
[ ] MultiplayerPolicy records 出现在 co-op evidence payload / logs
```

要求：

```text
不改变 gameplay behavior
不迁 high-risk patches
```

验收：

```text
[ ] runtime log 里能看到 architecture diagnostics
[ ] tests 全绿
[ ] no behavior change
```

---

## Week 4：Batch 4c Decision Gate

如果 runtime smoke passed：

```text
[ ] 提出 5–10 个 low-risk candidates
[ ] 每个 candidate 有 source target / risk / rollback plan
[ ] 不迁 high-risk patch
[ ] 不迁 run/map/save/multiplayer/death/lobby patch
```

如果 runtime smoke 未通过：

```text
[ ] Batch 4c remains blocked
[ ] 修 runtime blocker
[ ] 不迁 patch
```

---

# 11. 子代理要求

必须使用 subagents，否则容易又只做一块就总结完成。

## Subagent A — Runtime/Test Truth Agent

负责：

```text
validation commands
runtime smoke
godot.log
audit-godot-log
current-validation.md
```

## Subagent B — Docs Truth Agent

负责：

```text
清理 stale HEAD
清理旧测试数
统一 warning/runtime status
防止 release-ready 假 claim
```

## Subagent C — Sts1Events Governance Agent

负责：

```text
Off
CanaryOnly
AdditiveBatch1
AdditiveAllDraft
ReplaceUnknownEventsPrototype
risk table
runtime evidence rows
```

## Subagent D — Architecture Runtime Agent

负责：

```text
FeatureRegistry runtime log
RewardPipeline diagnostics
CardPlayContextCanary
DeathProtection no-op provider
MultiplayerPolicy records
```

## Subagent E — Patch Gate Agent

负责：

```text
阻止 Batch 4c
除非 runtime smoke passed
提出 low-risk candidate list
禁止 high-risk migration
```

## Subagent F — Release Gate Agent

负责：

```text
不允许 release-ready
不允许 runtime-safe 假 claim
不允许 AllDraft 进入 release
不允许 package refresh 冒充 runtime proof
```

---

# 12. Overnight Run Spec：必须跑完才能停止

下面这段可以直接给 Codex：

```text
你现在在 D:\Game\FOTN\dev-the-spire。

目标：Runtime Proof + Governance Closure Overnight Run。

这不是 Batch 4c。
不要迁更多 patches，除非 runtime smoke 已通过且 owner 明确接受 low-risk Batch 4c candidate。
不要新增 gameplay。
不要 claim release-ready。
必须使用 subagents。
必须跑完所有 phase；如果 blocker 存在，必须记录 issue、证据、下一步，不能直接停止。

当前审核发现：
- Latest main must be checked at start. Current validation may be stale relative to latest commit.
- RitsuLib dependency and manifest dependency exist.
- 25 patches migrated to RitsuLib ModPatcher.
- Raw Harmony count must be reconciled with latest source.
- Runtime smoke is hard blocked because the controlled loader audit is not clean, not because STS2-RitsuLib is missing.
- E-drive game root, mods, BaseLib, EZMicroBalance, and STS2-RitsuLib `v0.3.10` exist.
- Current best log exists at `.tools/runtime-evidence/sts1-events-v15-loader-20260531-231135/godot.log.after-launch`; it loads BaseLib, RitsuLib, and Spire Plus but audit is not clean. Supplemental retry logs exist under `.tools/runtime-evidence/ritsulib-runtime-proof-20260531-2304/`, but the direct-launch log failed Steam init and the Steam `-applaunch` log skipped `EZMicroBalance` as disabled.
- Sts1Events Off and CanaryOnly are source-safe only; runtime proof missing.
- AdditiveBatch1 controlled prototype; AdditiveAllDraft and ReplaceUnknownEventsPrototype unsafe/dev-only.
- FeatureRegistry, RewardPipeline, CardPlayContext, DeathProtectionService, MultiplayerPolicy are architecture/canary/diagnostics layers, not full gameplay enforcement.
- Batch 4c remains blocked.

Subagents:

1. Runtime/Test Truth Agent
   - Run full validation.
   - Install/verify STS2-RitsuLib if available.
   - Execute runtime smoke.
   - Capture and audit godot.log.
   - Update current-validation.md.

2. Docs Truth Agent
   - Remove stale HEAD/test/warning counts.
   - Ensure docs say runtime blocked unless fresh evidence exists.
   - Ensure release-ready remains no.

3. Sts1Events Governance Agent
   - Validate Off / CanaryOnly runtime gates if runtime smoke passes.
   - Preserve AdditiveBatch1 prototype status.
   - Keep AdditiveAllDraft and ReplaceUnknownEventsPrototype unsafe/dev-only.

4. Architecture Runtime Agent
   - Verify FeatureRegistry summary log.
   - Verify RewardPipeline diagnostics log.
   - Verify CardPlayContextCanary log.
   - Verify MultiplayerPolicy records / co-op evidence metadata.
   - No gameplay behavior changes.

5. Patch Gate Agent
   - Block Batch 4c unless runtime smoke passes.
   - If runtime smoke passes, propose candidate list only.
   - No migration unless explicitly accepted.

6. Release Gate Agent
   - Prevent release-ready claim.
   - Prevent runtime-safe claim without log.
   - Prevent package refresh from being used as runtime proof.

Phase 1 — Canonical validation

Run:
- git status --short --branch
- git log -1 --oneline --decorate
- dotnet clean
- dotnet build EZMicroBalance.sln
- dotnet test EZMicroBalance.sln
- dotnet test EZMicroBalance.sln --no-build
- dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
- git diff --check

Update docs/reviews/current-validation.md with:
- actual HEAD
- worktree status
- build warnings/errors
- test passed/failed/skipped
- format/diff status
- publish/package status

Phase 2 — Runtime smoke

If STS2-RitsuLib is installed or can be installed:
- ensure only BaseLib + STS2-RitsuLib + Spire Plus are active
- launch game
- collect godot.log
- audit log
- verify:
  - RitsuLib bootstrap starting
  - ModPatcher applied 25 patches
  - RitsuLib framework is active
  - BaseLib initialized
  - Spire Plus initialized
  - no MissingMethodException
  - no TypeLoadException
  - no manifest dependency failure

If STS2-RitsuLib is missing or the smoke is invalid:
- mark runtime blocker
- do not proceed to Batch 4c
- update runtime-smoke-checklist.md and current-validation.md with the exact invalid-smoke reason

Phase 3 — Sts1Events runtime gates

If runtime smoke passes:
- test Off mode: 0 registrations
- test CanaryOnly: exactly 4 registrations
- update issue evidence

If runtime smoke unavailable:
- keep source-only status
- runtime proof remains pending

Phase 4 — Architecture diagnostics

If runtime smoke passes:
- verify FeatureRegistry summary log
- verify RewardPipeline diagnostics log
- verify CardPlayContext canary log
- verify MultiplayerPolicy metadata in evidence payloads

If runtime smoke unavailable:
- keep architecture status as source/canary only

Phase 5 — Batch 4c decision

If runtime smoke passes:
- propose 5–10 low-risk candidates only
- do not migrate unless owner accepts

If runtime smoke blocked or failed:
- Batch 4c remains blocked

Phase 6 — Monthly spec update

Update:
- docs/features/ritsulib-migration/monthly-dev-spec.md
- docs/features/ritsulib-migration/next-overnight-run.md
- docs/reviews/current-validation.md
- runtime-smoke-checklist.md
- Sts1Events issue if evidence changes

Final report must include:
1. actual HEAD
2. validation result
3. runtime smoke status
4. Sts1Events runtime/source status
5. architecture diagnostics status
6. Batch 4c decision
7. subagent findings
8. files changed
9. decision: optimize / advance / both
10. release-ready: no

Stop condition:
Do not stop until every phase is complete or blocked with explicit issue, evidence, and next action.
```

---

# 13. 最终判断

当前工作有进展，而且方向正确。它已经完成了很多“源码层面的架构治理”：

```text
RitsuLib patch migration guard
Sts1Events governance
FeatureRegistry hardening
UrdaStateCodec
RewardPipeline/CardPlayContext canary
DeathProtection/MultiplayerPolicy stubs
```

但它仍没有完成最关键的 runtime 阶段：

```text
STS2-RitsuLib 未安装
无 fresh godot.log
无 runtime smoke
无 Off=0 runtime proof
无 CanaryOnly=4 runtime proof
```

因此当前决策是：

```text
继续优化为主，有限推进为辅。
```

下一步唯一主线：

```text
Runtime smoke + governance closure
```

Batch 4c 必须继续阻塞，直到 runtime smoke 通过。
