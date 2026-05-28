# StS1 Event Port Strict Audit + Monthly Dev Spec + Subagent Plan

Date: 2026-05-28
Target project: Devspire / Spire Plus (`EZMicroBalance` technical manifest id)

## Executive verdict

**Not complete.** The new work is a real infrastructure increment only if the local build log is accurate: it reportedly added a `Sts1EventRegistrationService`, wired `RegisterAll` into `MainFile`, and got a build with 0 errors. That does **not** mean the StS1 event port is playable, verified, or close to full parity.

Current audited status:

- Planning / folder structure: partially complete.
- Canonical event catalog: inconsistent and must be corrected.
- Per-event specs: mostly draft/template; not verified enough to code all events.
- Code implementation: mostly stubs / TODOs; no event can be accepted as fully complete without manual evidence.
- Registration: partial and potentially dangerous if unconditional.
- Assets: not complete; extraction strategy exists, not verified event art coverage.
- Localization: draft/placeholder; not production-complete.
- Tests: build-only evidence is insufficient.
- In-game proof: missing.
- StS1-only event pool: not implemented.
- Full parity: 0%.

## Evidence reviewed

1. User-provided work log claims:
   - `Sts1EventRegistrationService.cs` was created.
   - `MainFile.cs` now calls `Sts1EventRegistrationService.RegisterAll(ModId)` after RitsuLib bootstrap.
   - `dotnet build --no-restore` reportedly ended with 0 errors but warnings.
   - The worker claims all 46 events are registered.
   - The worker then says next steps are curse model references and in-game debug spawn.

2. User-provided status-board log claims:
   - There are 48 spec files.
   - The catalog lists 52 entries but the worker calls 4 of them act-specific duplicates.
   - Infrastructure is marked `Done`.
   - Big Fish and Golden Idol Source/Loc are marked `Done` while Asset/Test remain Pending.
   - The worker claims every spec contains precise Wiki behavior, A15 deltas, StS2 implementation plan, localization keys, and dependencies.
   - The worker's next action is “implement Phase 2 simple batch events in code.”

3. Project constraints from Devspire / Spire Plus:
   - Active deliverable remains one mod, `Spire Plus`, with stable technical id `EZMicroBalance`.
   - Code/resources belong under `EZMicroBalanceCode/` and `EZMicroBalance/`.
   - Do not rename existing manifest id in place.
   - Do not copy original game assets unless redistribution permission is confirmed.
   - Do not copy large decompiled game code into the repo.
   - Update docs when behavior/build/publish/validation status changes.
   - Code changes require at least build; resource/localization/package changes require publish too.

4. StS1 Wiki baseline:
   - Events come from unknown rooms, chosen by randomness and current Act.
   - Act 4 has no events.
   - A15 makes unfavorable event outcomes more likely/intense.
   - The target surface should be explicitly defined as 52 Wiki-listed event entries unless the team formally excludes special cases.

## Step-by-step audit

### Step 1 — Canonical target definition

**Claim:** 48 specs cover all unique events; 52 catalog entries include 4 duplicates.

**Audit:** Fail / needs correction. The worker is mixing three different concepts:

- Wiki-listed event entries.
- Runtime event model classes.
- Act-bucket appearances of shared/semi-shared events.

Monthly requirement: define all three in the docs. A safe canonical policy is:

- `wiki_event_entries = 52` based on the Wiki page categories.
- `runtime_event_models = N` only after a written deduplication policy.
- `act_bucket_memberships` must list shared/common/semi-common/Act-exclusive memberships separately.

No status board may say “all events done” until these categories are separated.

### Step 2 — Specs

**Claim:** 48 spec files are done and each contains precise effects/A15/dependencies.

**Audit:** Fail / not enough evidence. A file existing is not a spec being complete. Each spec must have:

- Wiki URL.
- Eligibility: act, character restrictions, relic/card/curse conditions, one-time restrictions.
- Exact options with normal + A15 values.
- Exact reward/punishment command mapping.
- Required models: cards, curses, relics, potions, monsters, encounters, UI.
- StS2 source API evidence.
- Localization keys and final EN/ZHS text status.
- Test cases.
- Asset key.
- Runtime state/save-load notes.

A spec becomes `source-verified` only after a source/API auditor signs off. It becomes `implemented` only after code and automated tests. It becomes `manual-verified` only after screenshot/log evidence.

### Step 3 — RitsuLib registration

**Claim:** Registration is wired and build passes.

**Audit:** Partial pass. This is useful infrastructure, but it is not full event implementation.

Major risks:

- The log says `RegisterAll(ModId)` is called directly after bootstrap. If this is unconditional, prototype events will enter the mod as soon as RitsuLib is active. That violates the intended “prototype off = zero behavior impact” rule.
- RitsuLib registration is additive; it appends/queues events into StS2 event enumerations. It does not by itself replace the StS2 unknown-room event pool with a StS1-only event pool.
- The worker says all 46 events are registered, while the status board says 48 specs and the target catalog says 52 entries. That mismatch is a release blocker.
- The act mapping must be rechecked. The worker says `Underdocks=Act1, Overgrowth=Act2, Hive=Act3`; source review suggests default act order is not that simple and may include `Overgrowth`, `Hive`, `Glory`, with `Underdocks` conditionally replacing an act. This must be written in `source-research/sts2-act-event-registration.md` before registration can be trusted.

Required fix: wrap all registration behind `Sts1EventFeatureGate` and add modes:

- `Off` — default, registers nothing.
- `CanaryOnly` — registers only Big Fish / Golden Idol / Lab / Divine Fountain.
- `AdditiveAllDraft` — dev-only, registers all implemented event classes for API smoke.
- `ReplaceUnknownEventsPrototype` — dev-only, fail-closed in multiplayer.

### Step 4 — Build status

**Claim:** Build succeeds with 0 errors.

**Audit:** Partial pass only. A build with warnings is useful but insufficient.

Needs follow-up:

- Capture full build log, not only tail output.
- Record whether the build used prototype symbols and RitsuLib reference paths.
- Run both feature-gate OFF and CanaryOnly builds.
- Run tests.
- Run publish if localization/resources changed.
- Capture normal Steam-client launch/log proof if claiming runtime load.

### Step 5 — Canary events

**Claim:** Big Fish / Golden Idol source and localization are Done.

**Audit:** Fail. They are not complete until the missing dependencies are resolved and verified.

Big Fish acceptance requirements:

- Banana heals floor(max HP / 3).
- Donut gives +5 Max HP and heals the gained HP if StS1 behavior requires it.
- Box grants a random common/uncommon/rare relic and adds Regret, subject to Omamori/curse-equivalent handling if implemented.
- Image exists and loads.
- EN/ZHS text final.
- Save/load after each branch.
- Screenshot/log evidence.

Golden Idol acceptance requirements:

- Take grants Golden Idol relic.
- Take transitions to trap page.
- Outrun adds Injury.
- Smash deals 25% max HP damage, 35% at A15+.
- Hide loses 8% max HP, 10% at A15+.
- Leave does nothing.
- Golden Idol must interact with Forgotten Altar / Moai Head in later phases.
- Image exists and loads.
- EN/ZHS text final.
- Save/load after each branch.
- Screenshot/log evidence.

### Step 6 — Assets

**Claim:** Asset docs are Done.

**Audit:** Fail as completion claim. Asset strategy can be `drafted`; assets are not complete until all implemented event art paths are extracted from a local StS1 install, copied to the correct ignored/tracked policy path, validated by script, and loaded in game.

Important: do not commit original StS1 art unless permission/redistribution policy is explicitly approved. Prefer local extraction + ignored/generated local assets for private testing.

### Step 7 — Localization

**Claim:** Loc EN/ZHS Done for canary.

**Audit:** Fail unless full final strings are present and in-game rendered. Placeholder text or partial option labels are not Done.

Required statuses:

- `loc-keyed` — keys exist.
- `loc-drafted` — English/ZHS draft exists.
- `loc-source-verified` — text matches StS1 behavior without overquoting copyrighted text.
- `loc-render-verified` — in-game screenshot confirms no missing keys/format issues.

### Step 8 — Tests

**Claim:** Build passes.

**Audit:** Build is not test completion. Required tests:

- manifest count/category consistency.
- every registered event has localization keys.
- every registered event has an asset manifest row.
- no implemented event contains TODO or throws NotImplemented.
- feature gate OFF registers no StS1 events.
- CanaryOnly registers exactly 4 events.
- AdditiveAllDraft registration count matches implemented classes, not spec files.
- manual debug spawn proof for every canary branch.

### Step 9 — StS1-only event pool

**Claim:** Events will appear in StS2 pool.

**Audit:** Partial and not parity. Additive appearance is not “StS1 inside StS2.” Full parity needs a pool replacement design:

- act-bucket event list.
- no StS2 vanilla events in StS1 mode.
- shuffled event bag save/load.
- one-time event behavior.
- character/relic/card condition checks.
- multiplayer fail-closed.
- deterministic RNG evidence.

### Step 10 — Release/documentation honesty

**Claim:** Infrastructure Done; specs Done.

**Audit:** Fail wording. Status board must distinguish `drafted`, `source-verified`, `implemented`, `tested`, `manual-verified`.

No item should be `Done` unless it passes all acceptance evidence. A monthly prototype may mark infrastructure `source-verified` or `build-verified`, but not `release-complete`.

## Updated completion score

| Area | Current status | Score |
|---|---|---:|
| Catalog/docs structure | partial; inconsistent counts | 35% |
| Per-event specs | draft only | 20% |
| RitsuLib registration | partial; build-claimed; needs gating | 30% |
| Canary implementation | not complete | 5–10% |
| All event implementation | stubs/TODO | 0–5% |
| Assets/images | strategy only | 0–5% |
| Localization | draft/placeholder likely | 10% |
| Tests/manual evidence | build-only | 5% |
| StS1-only event pool | not started | 0% |
| Full parity | not complete | 0% |

## Hard red lines for the worker

1. Do not claim “all events done.”
2. Do not claim “48 specs cover all unique events” without a written deduplication policy.
3. Do not keep unconditional `RegisterAll` in the default mod path.
4. Do not move to Phase 2 implementation before canary is playable and save/load verified.
5. Do not use `Done` for specs, localization, or assets without evidence.
6. Do not treat additive RitsuLib registration as pool replacement.
7. Do not commit or package original StS1 art without permission/policy approval.
8. Do not rely on tail-only build output; capture full logs.
9. Do not let implementation subagents grade their own work.
10. Do not push if validation fails or if unrelated changes are mixed in.

# Monthly Dev Spec: 2026-05-28 to 2026-06-30

## Month-level objective

Deliver **StS1 Event Port Prototype Batch 1**, not full parity.

Accepted month-end state:

- Default Spire Plus behavior unchanged when StS1 event feature gate is Off.
- CanaryOnly mode builds, registers exactly 4 events, and runs in game.
- Four canary events are playable and manually verified: Big Fish, Golden Idol, Lab, Divine Fountain.
- Six simple events are implemented and debug-spawn verified: Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar, Shining Light.
- Asset extraction/validation works for the 10 implemented events.
- Docs and status board report exact status without false Done claims.
- Pool replacement design is documented and prototype-gated, but not release-claimed.

## Week 0: 2026-05-28 to 2026-05-31 — Audit repair and gate safety

### Required work

- Add `docs/features/sts1-events/audit-2026-05-28.md`.
- Rewrite `status-board.md` with allowed statuses only:
  - `planned`
  - `spec-drafted`
  - `source-verified`
  - `api-verified`
  - `implemented`
  - `asset-verified`
  - `loc-render-verified`
  - `manual-verified`
  - `blocked`
- Rewrite catalog count policy:
  - `wiki_event_entries`
  - `runtime_event_models`
  - `act_bucket_memberships`
- Wrap `Sts1EventRegistrationService.RegisterAll` behind `Sts1EventFeatureGate`.
- Add CanaryOnly mode.
- Remove unconditional all-event registration from default mod init.
- Capture full build log for feature-gate Off.

### Acceptance

- Feature gate Off build passes.
- Feature gate Off test proves no StS1 events are registered.
- Status board contains no unsupported `Done`.
- 46/48/52 mismatch is documented and resolved.

## Week 1: 2026-06-01 to 2026-06-07 — Source/API verification

### Required work

- Create `source-research/sts2-act-event-registration.md`.
- Verify act model mapping and event registration target acts.
- Create `source-research/api-command-matrix.md` with:
  - HP heal/damage/max HP gain/max HP loss.
  - Add/remove cards and curses.
  - Relic grant and relic pool draw.
  - Potion grant.
  - Card select/remove/upgrade/transform UI.
  - Event option lock/damage/max HP tooltip.
  - Save/load event state.
- Add automated tests for registration counts and localization key coverage.

### Acceptance

- CanaryOnly build passes.
- CanaryOnly registers exactly Big Fish, Golden Idol, Lab, Divine Fountain.
- API matrix has exact class/method/file evidence for every command used by canary.
- Warnings are documented and triaged.

## Week 2: 2026-06-08 to 2026-06-14 — Canary implementation

### Required work

Implement and verify:

- Big Fish.
- Golden Idol.
- Lab.
- Divine Fountain.

Implement helpers:

- `Sts1HpService`.
- `Sts1RewardService`.
- `Sts1CurseService`.
- `Sts1AscensionRules`.
- `Sts1EventDebugSpawnCommand` or equivalent debug path.

### Acceptance

- All four events can be debug-spawned.
- Every branch has manual evidence.
- Save/load works after each branch.
- Images load for all four.
- EN/ZHS keys render in game.
- No TODO remains in the four canary event files.

## Week 3: 2026-06-15 to 2026-06-21 — Simple Batch 1

### Required work

Implement six simple events:

- Purifier.
- Upgrade Shrine.
- Golden Shrine.
- The Cleric.
- Old Beggar.
- Shining Light.

Support helpers:

- card removal select helper.
- card upgrade select helper.
- gold spend/gain helper.
- option lock helper.
- A15/normal value helper.

### Acceptance

- Six simple events debug-spawn and complete.
- Every branch has manual evidence.
- Implemented files contain no TODOs.
- Asset paths validated.
- Loc render verified.

## Week 4: 2026-06-22 to 2026-06-28 — Pool prototype and hardening

### Required work

- Create `Sts1EventPoolService` design doc.
- Implement debug-only `ReplaceUnknownEventsPrototype` mode if source evidence supports it.
- Save visited event ids and event bag state.
- Multiplayer fail-closed.
- Add tests for no vanilla StS2 events in replacement-mode unknown rooms.

### Acceptance

- Replacement mode is disabled by default.
- In dev replacement mode, unknown rooms draw only implemented StS1 events.
- Save/load does not duplicate/skip event bag state.
- Multiplayer path refuses replacement mode unless explicit debug override is set.

## Week 5 buffer: 2026-06-29 to 2026-06-30 — Package and handoff

### Required work

- Build.
- Publish if resources/localization/package changed.
- Increment package version only if player-visible build is delivered.
- Update `docs/features/sts1-events/monthly-review-2026-06.md`.
- Update release notes with `Prototype Batch 1`, not full parity.
- Commit/push only after validation passes.

### Acceptance

- Evidence bundle includes logs, screenshots, asset validation output, test output, and status board.
- Handoff tells testers exactly how to enable/disable StS1 event prototype.
- No full-parity language.

# Mandatory subagent plan

The worker must use subagents. Do not let one agent research, implement, and approve the same slice.

## Subagent 1 — Wiki Spec Auditor

Scope:

- Verify the 52 Wiki-listed event entries.
- Separate event entries, runtime models, and act bucket memberships.
- For each Week 2/3 event, produce exact option table and A15 deltas.

Output:

- `wiki-event-catalog.md` corrected.
- `event-specs/*.md` changed from draft to source-verified for the 10 monthly events.
- A mismatch report for 46/48/52.

Pass/fail:

- Pass only when each monthly event spec has exact options and dependencies.

## Subagent 2 — StS2 Source/API Auditor

Scope:

- Verify act mapping.
- Verify RitsuLib registration behavior.
- Verify command APIs for HP, relics, curses, potions, card operations, event options, and save/load.

Output:

- `source-research/sts2-act-event-registration.md`.
- `source-research/api-command-matrix.md`.
- API-safe recommendations for implementation.

Pass/fail:

- Pass only with exact file/class/method evidence.

## Subagent 3 — Feature Gate and Registration Engineer

Scope:

- Make registration safe.
- Implement Off / CanaryOnly / AdditiveAllDraft / ReplaceUnknownEventsPrototype modes.
- Add tests proving mode behavior.

Output:

- `Sts1EventFeatureGate`.
- `Sts1EventRegistrationService` gated.
- Tests for registration counts.

Pass/fail:

- Pass only if default Off registers nothing.

## Subagent 4 — Canary Implementation Engineer

Scope:

- Implement Big Fish, Golden Idol, Lab, Divine Fountain.
- Remove TODOs in those files.
- Use API matrix only; no guessed APIs.

Output:

- playable canary event code.
- helper services.
- branch-level automated tests where possible.

Pass/fail:

- Pass only after QA subagent manual evidence.

## Subagent 5 — Simple Batch Engineer

Scope:

- Implement Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar, Shining Light.

Output:

- six playable simple events.
- no TODOs in implemented files.

Pass/fail:

- Pass only after QA subagent manual evidence.

## Subagent 6 — Asset Pipeline Agent

Scope:

- Map event ids to local StS1 art extraction paths.
- Keep copyrighted art out of tracked/public repo unless explicitly approved.
- Validate extracted files and in-game load paths.

Output:

- `asset_manifest.csv` with monthly event coverage.
- extraction script fixes.
- validation output.

Pass/fail:

- Pass only with path validation and screenshots.

## Subagent 7 — Localization Agent

Scope:

- Create EN/ZHS text for monthly events.
- Avoid over-copying copyrighted StS1 prose.
- Verify dynamic variables and formatting.

Output:

- `EZMicroBalance/localization/eng/sts1_events.json`.
- `EZMicroBalance/localization/zhs/sts1_events.json`.
- in-game render screenshots.

Pass/fail:

- Pass only when no missing keys and no placeholder text remains for monthly events.

## Subagent 8 — QA / Red-Team Auditor

Scope:

- Review claims from all other subagents.
- Run build/test/publish as required.
- Debug-spawn every branch.
- Verify save/load.
- Verify feature gate Off path.

Output:

- `manual-evidence/2026-06/*.md`.
- screenshot/log index.
- final pass/fail table.

Pass/fail:

- QA subagent cannot be the implementation subagent.

## Subagent 9 — Release Documentation Agent

Scope:

- Keep docs honest.
- Update README, PROJECT_MAP, feature README, status board, test plan, monthly review, release notes.

Output:

- docs updated with exact status.
- no full-parity wording.
- package handoff if delivered.

Pass/fail:

- Pass only if status board matches evidence bundle.

## One-message instruction to send to the worker

```text
你现在不要继续 Phase 2 批量写事件。先启动 subagents：Wiki Spec Auditor、StS2 Source/API Auditor、Feature Gate/Registration Engineer、Canary Implementation Engineer、Asset Pipeline Agent、Localization Agent、QA Red-Team Auditor、Release Documentation Agent。

每个 subagent 必须输出：修改文件、证据路径、阻塞项、pass/fail。实现 subagent 不允许审核自己的工作；QA subagent 必须独立复核。

本月目标不是 full parity，而是 StS1 Event Port Prototype Batch 1：默认 Off 零影响，CanaryOnly 四事件 playable + save/load + 图片/本地化验证，再做六个简单事件。任何没有截图/日志/测试证据的内容不得标 Done。RitsuLib 注册只是 additive，不等于 StS1-only event pool。RegisterAll 不允许无条件进入默认初始化路径。
```
