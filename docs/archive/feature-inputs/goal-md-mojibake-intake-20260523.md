# Spire Plus Goal Guard

This file may carry long `/goal` intake notes below, but the current guardrail is compact:

- Current target: keep `Spire Plus / EZMicroBalance` as a test-ready manual build, not a release-ready build.
- live proof required before closing runtime rows.
- source review may close only source-level issues such as compile errors, stale API signatures, localization drift, missing resource paths, manifest drift, hash drift, and guard-test failures.
- runtime rows need game logs, screenshots, manual notes, or two-client evidence. They include live loader parity, clicked Ancient UI, Urda/Morvi/Lotha/Vakuu gameplay, save/load, Vakuu victory/no-black-screen, A11-A20 traversal/combat behavior, co-op ownership/desync, and Crystal Sphere and transform-preview live proof inside Spire Plus.
- No release-ready claim is made until those runtime rows have direct evidence.

明白，我按你刚才的修正重新校准判断：

```text
不再要求把 Morvi / Lotha 默认关掉。
不再把“已经超前实现”当作错误本身。
不再把 DLL 打包授权当作合规 blocker。
现在重点改成：这些新实现已经存在，接下来要让它们能稳定测、能解释清楚、能避免递归/软锁/多人/save-load 问题。
```

当前 GitHub 最新状态也确实支持你的说法：项目已经进入 **Spire Plus / v0.106.0 / BaseLib 3.1.4** 阶段，`PROJECT_STATE.md` 记录当前 active 内容包括 Ancient reward rebalance、Ascension 11-20、Rootblight、Urda、Morvi、Lotha、隐藏 Vakuu fight、Preview tools 等；同时它也明确说 live gameplay、clicked UI、save-load、death/failure path、co-op verification 仍 pending。

`MainFile.Initialize()` 现在也确实初始化了 `LothaInitializer`、`MorviInitializer`、`UrdaInitializer`、`VakuuFightInitializer` 和 `AscensionInitializer`，所以这些已经不是纯文档 planning，而是 active source surface。 Morvi / Lotha 的 gate 也是默认开启、通过 `EZMB_DISABLE_MORVI` / `EZMB_DISABLE_LOTHA` 或 SpirePlus 对应变量关闭，这和你说“默认开启没问题”一致。

下面我给出新的推进方案。

---

# 现在要解决的具体问题

## P0：v0.106 / BaseLib 3.1.4 当前状态要闭环

当前 `PROJECT_STATE.md` 写的是：

```text
Slay the Spire 2 game snapshot: v0.106.0
BaseLib runtime/project package: v3.1.4
```

并且说明本地 `source code/` 已经从 `SlayTheSpire2.pck` + `sts2.dll` 重新生成。 这说明接下来所有源码证据都要改口径：**不要再按 v0.105.0 / v0.105.1 推理。**

需要 Codex 具体做：

```text
[ ] 刷新所有仍写 v0.105.x 的 release-facing docs
[ ] 确认 BaseLib 3.1.4 是唯一当前目标
[ ] 用 v0.106 source code / sourcecodeonlyaianalysis 检查所有 Hook
[ ] 记录 v0.106 API drift：CardPileCmd.Add clonedBy、side-turn hook、AncientEventModel、reward alternatives、death hook、multiplayer lobby
```

---

## P0：Morvi / Lotha default-on 不是问题，但必须变成“可测状态”

你说默认开没问题，那就不关。但默认开以后，必须补“测试入口、快速回滚、失败定位”。

当前 Morvi 和 Lotha gate 都是默认开启、通过 disable env var 关闭。  这可以保留。

需要补的是：

```text
[ ] 每个 Ancient 必须能强制出现：SPIREPLUS_FORCE_ANCIENT / EZMB_FORCE_ANCIENT
[ ] 每个祝福必须能强制选择：SPIREPLUS_FORCE_*_BLESSING / EZMB_FORCE_*_BLESSING
[ ] 每个 Ancient 必须能单独关闭：SPIREPLUS_DISABLE_MORVI / EZMB_DISABLE_MORVI 等
[ ] docs/test-ready-development-goal.md 里列出每个强制测试命令
[ ] log 里打印选中的 Ancient / blessing / player slot / run id
```

这样默认开启也安全，因为测试时可以单独定位。

---

## P0：Lotha 死亡保护 / 死刑缓期递归风险

你说“死亡保护递归这个让他弄”，这确实是最危险的机制之一。当前测试 guard 已经显示 Lotha 源码涉及 `ShouldDieLate`、`ShouldDie`、`AfterPreventingDeath`、`CreatureCmd.Kill(player.Creature, force: true)` 等路径。

这里必须做 source-driven 保护，不然会出现：

```text
死亡保护触发后再次触发死亡保护
强制死亡又被其他死亡保护拦截
多人里 host/client 判断死亡状态不同
敌方回合中断后行动队列继续执行
战斗已经结束但缓期回合还在跑
save/load 后 still reprieved 状态错误
```

建议 Codex 直接做一个专项：

```text
ISSUE-2026-05-23-LOTHA-DEATH-REPRIEVE-RECURSION-AND-FAILURE-PATH
```

要求：

```text
[ ] 用 v0.106 source 查 ShouldDie / ShouldDieLate / AfterPreventingDeath 调用顺序
[ ] Death Reprieve 必须有 per-run used 标记
[ ] Death Reprieve 缓期回合内必须有 inReprieveTurn 标记
[ ] 缓期失败强制死亡必须设置 cannotBePrevented / equivalent source-proven flag
[ ] force kill 后不能被 After Rain / Fairy / 其他保护二次拦截
[ ] 敌方回合触发时必须清理/中断后续敌方动作，或记录无法安全中断的 blocker
[ ] co-op 下只对对应 player 生效，不广播所有玩家
[ ] save/load 中如果处于缓期回合，要么恢复正确状态，要么禁止/安全处理保存
```

---

## P0：Morvi / Lotha 的 extra-play 递归和能力牌安全

v2.2 的核心规则是：**能力牌不能被复制 / 额外打出 / 复印 / 宣判。**

当前测试 guard 已经检查 Morvi 里不应有 `CreateClone`、`TryAddGeneratedCardToCombat(copy`、`CardCmd.AutoPlay` 等。 Lotha 也有大量 extra-play / power replacement 规则。

但 source guard 不等于实际安全。Codex 需要继续补：

```text
[ ] 每个 extra-play 入口必须检查 !card.IsClone / !cardPlay.IsAutoPlay / source-proven equivalent
[ ] 额外打出的攻击/技能不能再次触发同一 blessing
[ ] Power card fallback 不进入 extra-play path
[ ] Mirror Rebuttal / Single Sentence / Deferred Verdict / Misprint Press 不能互相无限触发
[ ] 多个 blessing 同时存在时触发顺序必须明确
[ ] 加日志：card id、source blessing、isAutoPlay、isClone、extraPlayCount
```

---

## P0：Reward UI / save-load 软锁风险

Morvi / Urda 都大量碰 reward screen。当前 Urda 使用 card reward alternative、skip patch、`ConditionalWeakTable<CardReward, CardRewardContext>` 等机制。 Morvi guard 也显示它有 reward candidates、option relic、custom reward、borrowed ancient、debt 等路径。

最容易出问题：

```text
奖励界面保存退出后 context 丢失
点击 alternative 后 reward 没 complete
alternative 重复点击复制奖励
skip reward 触发两次
Humus 第三跳过后删牌/奖励流程可重复
Seedbed accept 后 HP 扣了但卡没加
Morvi Forbidden Loan 选择后 Ancient card 没正确标记
Debt Settlement 删牌/升级/金币流程中断后状态不一致
```

需要专项：

```text
ISSUE-2026-05-23-ANCIENT-REWARD-UI-SAVELOAD-SOFTLOCK-AUDIT
```

测试要求：

```text
[ ] 在 reward screen 打开时 save/load
[ ] accept alternative 后立即 save/load
[ ] skip 后触发 Humus，删牌界面前/后 save/load
[ ] Forbidden Loan 选择后 save/load
[ ] Debt Settlement 删牌/升级中途 save/load
[ ] 所有路径不能重复给金币/卡/升级
```

---

## P1：A19/A20 Boss Seal / Brand 仍要继续修

你之前测到：

```text
第二 Boss Brand 没显示 / 没生效
A20 没有中场休息
Knowledge Demon / Kaiser Crab 看不出变化
A19/A20 加成没有显示清楚
```

当前 `PROJECT_STATE.md` 仍写 Ascension 11-20 live verification pending，multiplayer/co-op traversal pending。 所以这块不能关。

需要继续做：

```text
[ ] A20 second boss metadata fallback
[ ] Boss 1 -> Boss 2 intermission / recovery / warning
[ ] Boss Seal map hover
[ ] Boss Seal combat start notice
[ ] Trigger feedback for Marginal Note / Misaligned Shell / Residual Sample
[ ] A20 co-op second boss behavior
```

---

## P1：v0.106 当前 source 和 sourcecodeonlyaianalysis 的一致性

你这次上传了 v0.106.0 code-only zip，项目也记录 source code 已重新生成。 这轮要让 Codex 系统检查：

```text
source code/src/Core/**
sourcecodeonlyaianalysis/**
```

重点比对：

```text
CardPileCmd.Add clonedBy 参数
AbstractModel hook lifecycle
CardReward.OnSkipped
CardRewardAlternative
Ancient registration
RunManager second boss flow
death prevention hook
multiplayer lobby / version mismatch
ModSettings API
```

如果两个 corpus 不一致，必须写清楚：

```text
哪个是 primary
哪个只作参考
哪些 patch target 需要更新
```

---

## P1：跨平台 Windows / Mac 测试脚本

你说 Windows/Mac 都要考虑，这个要落到具体文件。当前项目有 PowerShell 脚本、Windows 路径、Steam 路径、hash 命令。需要补 Mac 等价命令：

```text
Windows:
Get-FileHash
Expand-Archive
%APPDATA%\SlayTheSpire2\logs

macOS:
shasum -a 256
unzip
~/Library/Application Support/SlayTheSpire2/logs 或实际 source-proven 路径
Steam common path
```

需要新增或更新：

```text
docs/platform-testing.md
scripts/check-installed-ezmb-package.ps1
scripts/check-installed-ezmb-package.sh  # 如果可行
```

---

## P1：官方 DLL 打包授权 OK，但仍要做“版本冲突 guard”

你说授权没有问题，那我不再把它当合规 blocker。但它仍有**技术风险**：如果 zip 里带了 `sts2.dll`、`BaseLib.dll`、`0Harmony.dll`，可能导致玩家加载错版本。当前 csproj 会把这些复制到输出和 `.godot` temp。

所以不是“不能打包”，而是：

```text
[ ] 如果决定打包官方 DLL/BaseLib DLL，必须记录为什么
[ ] 如果不打包，release artifact test 要确保 zip/PCK 不包含
[ ] 如果打包，要检测版本必须等于当前 v0.106 / BaseLib 3.1.4
[ ] 不能出现玩家 mods/BaseLib 3.1.4 但包内还有另一个 BaseLib.dll 被优先加载
```

这要 Codex 继续查 Godot export 后 zip 里到底有没有这些 DLL。

---

## P1：美术 / source-art 已授权，但要做“路径和可见性测试”

你说新东西已经抄了/授权了，那就不作为资产合规问题。现在要测的是：

```text
[ ] PCK 里有对应 art
[ ] 路径大小写 Windows/Mac 都对
[ ] Ancient UI event art 能加载
[ ] option art 能加载
[ ] run history icon / map icon 能加载
[ ] missing texture 不会让 UI 空白
```

当前 `PROJECT_STATE.md` 提到 Morvi/Lotha/Urda/Vakuu small art、event art、source-local art、final_generated assets。 这部分要做 clicked UI 实测，而不是只看文件存在。

---

# 给 Codex 的下一步大方向 Prompt

下面这段可以直接发给它。目标是**继续开发，但先做高风险系统的测试硬化 + 问题清单 + 小修，不回滚新功能**。

```text
你现在在仓库：

D:\Game\FOTN\dev-the-spire

目标：Spire Plus v0.106 high-risk systems hardening pass。

用户明确说明：
- Morvi / Lotha / Urda 新实现默认开启没问题，不要改回默认关闭。
- Vakuu fight 保持 hidden-by-default 没问题。
- 官方 DLL / BaseLib / 资源授权问题用户已处理，不要把它当合规 blocker。
- 当前任务不是回滚新内容，而是让这些超前实现可测试、可解释、可 debug、可逐步稳定。
- 不要新增大玩法，先做高风险路径的源码验证、测试护栏、诊断、小修、手测矩阵。

必须先读：
1. PROJECT_STATE.md
2. AGENTS.md
3. docs/issues.md
4. docs/issues/waiting-tests.md
5. docs/features/ancient-expansion-v2.2/README.md
6. docs/features/ancient-expansion-v2.2/risk-register.md
7. docs/features/ancient-expansion-v2.2/manual-test-checklist.md
8. docs/features/ancient-expansion-urda/**
9. docs/features/ascension-11-20/**
10. EZMicroBalanceCode/Ancients/Expansion/Urda/**
11. EZMicroBalanceCode/Ancients/Expansion/Morvi/**
12. EZMicroBalanceCode/Ancients/Expansion/Lotha/**
13. EZMicroBalanceCode/Ancients/Expansion/Vakuu/**
14. EZMicroBalanceCode/Ascension/**
15. source code/src/Core/**
16. sourcecodeonlyaianalysis/**
17. tests/EZMicroBalance.Tests/**

硬规则：
- 不要改 manifest id。
- 不要关闭 Morvi/Lotha 默认开启。
- 不要关闭 Urda 默认开启。
- 不要实现新 Ancient。
- 不要新增 A21-A30。
- 不要 claim release-ready。
- 不要大规模重写。
- 只做高风险路径的小修、诊断、测试和文档更新。

Phase 1：v0.106 source/API drift audit

检查 `source code/src/Core/**` 和 `sourcecodeonlyaianalysis/**` 是否一致。
重点检查：
- CardPileCmd.Add clonedBy 参数
- AbstractModel combat/run hooks
- CardReward.OnSkipped
- CardRewardAlternative
- AncientEventModel / Ancient registration
- RunManager / NGame / second boss transition
- ShouldDie / ShouldDieLate / AfterPreventingDeath
- multiplayer lobby / version mismatch
- ModSettings / BaseLib config

输出：
- docs/audits/v0.106-source-api-drift.md
- 不复制大段源码，只写 class/method 和结论。

Phase 2：Lotha Death Reprieve recursion hardening

新增/更新 issue：
ISSUE-2026-05-23-LOTHA-DEATH-REPRIEVE-RECURSION-AND-FAILURE-PATH

实现或验证：
- Death Reprieve per-run used flag.
- inReprieveTurn flag.
- forced failure death cannot be prevented by Death Reprieve again.
- forced failure death cannot be prevented by After Rain / Fairy / other protection unless source explicitly says unavoidable death cannot be blocked.
- enemy-turn trigger either safely interrupts pending enemy actions or logs blocker.
- co-op only affects the dying player.
- save/load during reprieve is handled or blocked/documented.
- Add diagnostics:
  - player slot
  - trigger source
  - current HP
  - used flag
  - inReprieveTurn flag
  - forced death flag
- Add source guards and manual test rows.

Phase 3：Extra-play / Power-card safety hardening

Audit Morvi:
- Misprint Press
- Blueprint Proof
- Open Book
- Forbidden Loan

Audit Lotha:
- Mirror Rebuttal
- Mirror Hall Echo
- Deferred Verdict
- Single Sentence

Requirements:
- Power cards never copied or extra-played.
- Attack/Skill extra plays cannot recursively trigger same blessing.
- Extra-played cards must be marked source-proven way:
  - isAutoPlay
  - IsClone
  - transient suppression flag
- Multiple blessing interactions have deterministic order.
- Add diagnostics when extra-play is attempted:
  - blessing
  - card id
  - card type
  - is clone / autoplay
  - allowed or blocked
- Add tests for “Power fallback path only”.

Phase 4：Reward UI / save-load softlock audit

新增/更新 issue：
ISSUE-2026-05-23-ANCIENT-REWARD-UI-SAVELOAD-SOFTLOCK-AUDIT

Audit and add diagnostics for:
- Urda Seedbed reward alternative
- Urda Humus Pact reward skip
- Morvi Forbidden Loan selection
- Morvi Debt Settlement
- Lotha Closed Court reward suppression
- Prismatic Gem reroll if still active

Check:
- reward alternative completes reward once.
- skip cannot double-trigger.
- save/load on reward screen does not duplicate cards/gold/HP loss.
- WeakTable-only reward context risks are documented.
- Add manual rows:
  - save on reward screen before alternative
  - save after alternative click
  - save during card removal/upgrade choice
  - continue run and verify no duplicate reward.

Phase 5：A19/A20 Boss Seal and second boss flow

Audit / fix:
- A20 second boss Brand metadata.
- Test Subject Residual Sample second boss effect.
- Knowledge Demon Marginal Note visibility.
- Kaiser Crab Misaligned Shell visibility.
- Boss 1 -> Boss 2 intermission/recovery/notice.
- Boss node hover text.
- Combat start notice.

Add diagnostics:
- current act
- current map coord
- boss map point
- second boss map point
- encounter id
- boss seal id
- is brand
- metadata source path used

Do not claim fixed unless source + manual proof.

Phase 6：Cross-platform test support

Create/update:
- docs/platform-testing.md

Must include:
- Windows installed mod hash check.
- macOS installed mod hash check.
- Windows log path.
- macOS log path.
- PowerShell vs bash env vars:
  - `$env:EZMB_DISABLE_MORVI='1'`
  - `EZMB_DISABLE_MORVI=1`
- Get-FileHash vs shasum.
- Expand-Archive vs unzip.
- Steam path examples.

If easy, add:
- scripts/check-installed-ezmb-package.sh
but do not overbuild.

Phase 7：Package DLL / dependency version guard

Because build currently copies sts2.dll / 0Harmony.dll / BaseLib.dll for Godot export:
- Decide whether final zip should contain them.
- User says authorization is handled, so do not treat this as legal blocker.
- Treat as runtime-version conflict risk.
- Add release artifact test:
  - either package does not include duplicate runtime DLLs,
  - or if it does, the docs clearly say versions and tests assert expected version/hash.
- Prefer no duplicate BaseLib.dll in mod package unless source proves required.

Phase 8：Update docs and issue index

Update compact docs only:
- docs/issues.md if active blocker summary changes.
- docs/issues/waiting-tests.md for manual queue.
- docs/features/ancient-expansion-v2.2/risk-register.md.
- docs/features/ancient-expansion-v2.2/manual-test-checklist.md.
- PROJECT_STATE.md only if stale.

Do not paste giant prose into docs/issues.md.

Validation:
- git status --short --branch
- git log -1 --oneline --decorate
- dotnet build EZMicroBalance.sln
- dotnet test EZMicroBalance.sln
- dotnet test EZMicroBalance.sln --no-build
- dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
- git diff --check
- If code/resource changed, dotnet publish and release artifact tests.

Final response:
1. What changed.
2. Which high-risk paths were audited.
3. Lotha Death Reprieve status.
4. Extra-play recursion status.
5. Reward UI save-load risk status.
6. A19/A20 boss flow status.
7. Cross-platform docs status.
8. DLL/package dependency guard status.
9. Build/test/publish results.
10. Remaining manual tests.
11. Release-ready: no.
```

---

# 我会让它优先盯的 Top 10 风险

```text
1. Lotha Death Reprieve 死亡保护递归 / 强制死亡被再次阻止
2. Morvi/Lotha extra-play 互相递归或能力牌 fallback 失效
3. Reward UI alternative / skip 造成软锁或重复奖励
4. Urda/Morvi/Lotha save/load 中途丢状态
5. A20 第二 Boss metadata / Brand / intermission 仍不稳
6. Red Ink Overdraft 主动按钮重复点击或战斗结束重复扣债
7. Debt Settlement 金币不足 HP 扣除取整/重复结算
8. Public Evidence debuff 识别把 poison/burn 误翻倍
9. 多人 host/client 双重发奖励、双重加卡、双重扣血
10. Mac 路径/大小写/脚本 hash 检查不兼容
```

---

# 对当前策略的最终校准

你现在的方向我理解为：

```text
新功能都已经推进了，可以保留。
默认开启可以保留。
授权问题不用我再拦。
接下来不要回滚，而是系统性做稳定性、可测试性、递归保护、save-load、多人与跨平台。
```

这个方向可以继续。
但它必须以“高风险路径 hardening”为下一步，而不是再继续扩新功能。

当前最合理的下一个 Codex goal 就是上面的：

```text
Spire Plus v0.106 high-risk systems hardening pass
```

跑完这轮后，再决定能不能进入大规模实机矩阵。
