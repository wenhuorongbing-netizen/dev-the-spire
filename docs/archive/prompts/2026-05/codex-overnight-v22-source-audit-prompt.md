# Codex Overnight Source-Audit Prompt — Ancient Expansion v2.2 / EZMB Stability Review

Historical archive.

Current source of truth: `docs/features/ancient-expansion-v2.2/audit/overnight-source-audit.md`.

You are a senior C#/.NET, Godot, Slay the Spire 2 mod engineer, multiplayer systems reviewer, and release-risk auditor.

Repository:

D:\Game\FOTN\dev-the-spire

Goal:

Run an overnight **strict source-driven audit** of the current EZ Micro Balance repository and the Ancient Expansion v2.2 plan.

This is an **audit / research / planning / risk-discovery pass**, not an implementation pass.

Do not implement gameplay changes unless needed to add read-only diagnostics or tests explicitly requested below. Do not add Morvi, Lotha, Vakuu, new cards, new powers, or new Ancient gameplay in this pass.

Primary output:

A complete source-backed audit package that answers:

1. What is currently implemented?
2. What is only planned?
3. What does v2.2 require that current source cannot safely support yet?
4. Which parts may softlock, desync, fail save/load, or break on Windows/Mac?
5. What multiplayer-specific issues must be fixed before release?
6. What manual/live tests are required?
7. What should the next implementation milestone be?

Hard rules:

- Do not change manifest ids.
- Do not implement A21-A30.
- Do not implement custom characters.
- Do not implement Morvi/Lotha/Vakuu gameplay in this audit pass.
- Do not copy official game assets.
- Do not copy large decompiled source bodies into docs.
- Do not claim release-ready.
- Local source is primary evidence.
- Fix root cause in future tasks; do not recommend downstream normalizer helpers that hide invalid upstream state.
- Keep current docs compact; do not paste huge design prose into `docs/issues.md`.
- Archive or link historical docs instead of forcing future agents to read prompt dumps.

Source locations to inspect:

1. Current mod repository source:
   - `EZMicroBalanceCode/**`
   - `EZMicroBalance/localization/**`
   - `tests/EZMicroBalance.Tests/**`
   - `docs/**`
   - `scripts/**`

2. Installed/local game source, primary:
   - `source code/src/Core/**`

3. Code-only AI-analysis source, secondary but useful:
   - `sourcecodeonlyaianalysis/**`

4. Uploaded/source-only zip if present locally:
   - `source-code-code-only-ai-analysis-20260509-012059.zip`
   - If not extracted, extract to a temporary ignored folder such as `.tools/sourcecodeonlyaianalysis-extracted/`
   - Do not commit extracted source.
   - Compare only API/class/method names and source behavior; do not copy large source bodies into docs.

5. BaseLib/template/RitsuLib references if local.

Current repo-state warning:

The latest GitHub/main may have moved beyond prior A1.05.08 evidence. Start by running:

- `git status --short --branch`
- `git log -1 --oneline --decorate`

Then refresh `PROJECT_STATE.md` only if clearly stale. If updating docs, use wording that will not immediately become stale after the next commit, e.g.:

- `Reviewed source baseline: <commit>`
- `Re-run git log before release packaging`

Do not pretend local package hashes are current unless release-artifact tests actually passed after the latest source/package state.

Must read first:

1. `AGENTS.md`
2. `PROJECT_STATE.md`
3. `docs/README.md`
4. `docs/PROJECT_MAP.md`
5. `docs/issues.md`
6. `docs/issues/waiting-tests.md`
7. `docs/issues/urda.md`
8. `docs/private-beta-verification-handoff.md`
9. `docs/rc1-live-validation-log.md`
10. `docs/release-checklist.md`
11. `docs/features/ancient-expansion-v2.2/**` if it exists
12. `docs/features/ancient-expansion-urda/**`
13. `docs/features/ascension-11-20/**`
14. `docs/features/ancients-rework-v4/**`
15. `docs/style/card-localization-style-guide.md`
16. `EZMicroBalanceCode/Ancients/Expansion/Urda/**`
17. `EZMicroBalanceCode/Ancients/**`
18. `EZMicroBalanceCode/Ascension/**`
19. `tests/EZMicroBalance.Tests/**`

Create audit docs:

Create directory if missing:

`docs/features/ancient-expansion-v2.2/audit/`

Create these files:

1. `docs/features/ancient-expansion-v2.2/audit/overnight-source-audit.md`
2. `docs/features/ancient-expansion-v2.2/audit/source-api-map.md`
3. `docs/features/ancient-expansion-v2.2/audit/implementation-gap-matrix.md`
4. `docs/features/ancient-expansion-v2.2/audit/multiplayer-risk-matrix.md`
5. `docs/features/ancient-expansion-v2.2/audit/save-load-risk-matrix.md`
6. `docs/features/ancient-expansion-v2.2/audit/windows-mac-platform-risk-matrix.md`
7. `docs/features/ancient-expansion-v2.2/audit/manual-test-master-matrix.md`
8. `docs/features/ancient-expansion-v2.2/audit/next-implementation-goals.md`

Update these indexes only with compact links:

- `docs/README.md`
- `docs/PROJECT_MAP.md`
- `docs/features/ancient-expansion-v2.2/README.md` if it exists
- `docs/issues.md` only if adding a compact link or one issue row

Do not put long audit text in `docs/issues.md`.

Phase 1 — current implementation inventory

Answer with source evidence:

A. Ancient systems

- What Ancient reward rebalance v4 code is active?
- What Urda code is active?
- Is Urda default-on, debug-only, or disabled?
- Which Urda blessings have actual runtime hooks?
- Which Urda blessings are only localization/design?
- Are Morvi, Lotha, Vakuu implemented anywhere in active source?
- Are any v2.2 planning docs accidentally implying Morvi/Lotha/Vakuu are live?

Inspect:
- `EZMicroBalanceCode/Ancients/**`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/**`
- `EZMicroBalance/localization/*/ancients.json`
- `docs/issues/urda.md`
- `PROJECT_STATE.md`

B. Ascension systems

Inventory current A11-A20 slices:
- A11 map shape
- A12 Firemarked Elite
- A13 Fission
- A14/A15/A18 Rootblight/Blight Sprout
- A16 Banner rooms
- A17 Deep Branch
- A19 Boss Seal
- A20 Brand / second boss flow
- multiplayer selection/warning/downgrade

Inspect:
- `EZMicroBalanceCode/Ascension/**`
- `docs/features/ascension-11-20/**`

C. Packaging/runtime state

- Current expected hashes from handoff.
- Current SavedSpireField expected count.
- Current BaseLib version.
- Whether docs say v0.105.0 or v0.105.1.
- Whether package evidence matches latest code or is stale.

Phase 2 — compare v2.2 design against current code

Build a matrix with rows:

- Urda Trial Branch
- Urda Shallow-Root Relic
- Urda Rooted Route
- Urda Seedbed
- Urda Molting
- Urda After the Rain
- Urda Root-Sight
- Urda Humus Pact
- Urda Seed Bank
- Urda Moss Map
- Morvi Forbidden Loan
- Morvi Misprint Press
- Morvi Red Ink Overdraft
- Morvi Overdue Library
- Morvi Open-Book Exam
- Morvi Paperstorm
- Morvi Blueprint Proof
- Morvi Debt Settlement
- Lotha Mirror Rebuttal
- Lotha Mirror Hall Echo
- Lotha Presumption
- Lotha Closed Court
- Lotha Deferred Verdict
- Lotha Death Reprieve
- Lotha Single Sentence
- Lotha Public Evidence
- Vakuu Fight
- Temptation
- Withered Husk
- Waste Paper
- Archive Pages

Columns:

- Design status
- Source implementation status
- Needed hooks
- Current available source API
- Risk level
- Multiplayer risk
- Save/load risk
- UI/preview risk
- Platform risk
- Recommended milestone
- Blockers

Use statuses:
- `implemented-source`
- `implemented-partial`
- `planning-only`
- `blocked-by-source-api`
- `blocked-by-live-evidence`
- `unsafe-without-multiplayer-model`
- `do-not-implement-yet`

Phase 3 — source API map

For each required hook, inspect local game source and record source-backed API notes:

Required hook surfaces:

- Ancient offer generation / Ancient registration
- Ancient option selection
- Card reward generated
- Card reward alternatives
- Card reward skipped
- Custom reward offering
- Card selection from deck
- Card remove / transform / upgrade
- Combat start
- Turn start
- Turn end
- Card played
- Before card played / cost mutation
- Card drawn
- Card changed piles
- Status/curses/powers added
- Damage received
- Unblocked attack damage taken
- Before death / death prevention
- Combat end
- Act enter
- Room enter
- Boss defeated
- Map node hover
- Map node selected
- Save/load fields
- Multiplayer owner/local-player checks
- UI button injection
- Pile count/UI refresh
- Platform file paths and log locations

For each:
- Source file path
- Class/method name
- What it supports
- What it does not support
- Whether safe for single-player
- Whether safe for multiplayer
- Whether save/load stable
- Whether likely Windows/Mac issue exists

Do not copy large method bodies.

Phase 4 — bug/risk audit from recent player feedback

Audit these concrete recent issues:

1. Firemark/Banner monotony:
   - deterministic Might/Vanguard first issue
   - whether latest source changed it
   - whether first marked room can now vary
   - whether preview exists

2. Firemark/Banner balance:
   - Giant HP tuning
   - Forge Armor block tuning and first-turn timing
   - Might multi-enemy scaling
   - Vanguard/Shield/Bounty clarity and numbers

3. Fission:
   - chance math
   - eligible filtering
   - diagnostics
   - why player may not see it

4. Blight Sprout / Rootblight:
   - boss/elite insertion conditions
   - discard pile count mismatch
   - Rootblight II/III played purge vs downgrade
   - combat-end notices
   - art visibility
   - co-op owner safety

5. Boss Seal / A19/A20:
   - map preview
   - combat-start notice
   - Knowledge Demon/Marginal Note visibility
   - Kaiser Crab/Misaligned Shell visibility
   - Test Subject/Residual Sample behavior
   - A20 second boss Brand metadata and effect
   - A20 intermission after boss 1
   - direct transition risk

6. Multiplayer:
   - A11-A20 selection
   - HP0/Neow
   - save & quit propagation
   - black screen
   - host/client log evidence
   - ModelDb hash and mod-list mismatch diagnostics
   - second boss/co-op brand gating
   - per-player state for Rootblight and Urda

For each:
- Source status
- Hypothesis
- Evidence needed
- Proposed fix category:
  - source fix
  - diagnostic only
  - manual test only
  - design/balance decision
  - multiplayer blocker

Phase 5 — multiplayer deep audit

Create `multiplayer-risk-matrix.md`.

For every active or planned feature, mark:

- Host-only safe?
- Client-safe?
- Does it mutate shared run state?
- Does it mutate local UI only?
- Does it depend on `LocalContext.IsMe`?
- Does it use `Player.IsActiveForHooks`?
- Does it use `RunState.Players` safely?
- Does it require network command replication?
- Does it use direct field mutation that might desync?
- Does it add cards/rewards/powers from host only or all clients?
- Does it alter map metadata consistently host/client?
- Does it affect save/load?
- Test row needed?

Pay special attention to:
- Urda Seedbed reward alternatives
- Urda Humus card reward skip
- Urda Molting deck mutation
- Urda Moss Map room entry rewards
- RootDeckService / Rootblight
- Blight Sprout generated cards
- Firemark/Banner map metadata
- A19/A20 Boss Seal metadata
- A20 Boss 1 -> Boss 2 flow
- Red Ink Overdraft active button if implemented later
- Death Reprieve / death prevention if implemented later

Phase 6 — Windows/Mac/platform audit

Create `windows-mac-platform-risk-matrix.md`.

Check:

A. Paths and scripts:
- `scripts/audit-godot-log.ps1`
- `scripts/check-installed-ezmb-package.ps1`
- hardcoded `D:\Steam\...`
- `%APPDATA%\SlayTheSpire2\logs`
- use of backslashes in docs/scripts
- PowerShell availability on macOS
- command examples with Windows-only paths
- package install paths on Windows vs macOS
- Steam app path differences

B. Build tooling:
- `Directory.Build.props.example`
- `GodotPath`
- `Sts2Path`
- `dotnet publish` on Windows vs macOS
- path separators
- shell variable syntax:
  - PowerShell `$env:VAR`
  - bash/zsh `VAR=value`

C. Runtime:
- local log path
- Steam launch
- mod folder path
- file hash commands:
  - Windows `Get-FileHash`
  - macOS `shasum -a 256`
- zip extraction commands:
  - Windows `Expand-Archive`
  - macOS `unzip`

Output:
- Windows test commands
- macOS equivalent commands
- docs that need both variants
- scripts that are Windows-only
- whether a cross-platform bash/python helper should be added later

Phase 7 — save/load risk audit

Create `save-load-risk-matrix.md`.

For active and planned features, list:

- state data
- where saved
- whether SavedSpireField exists
- card instance markers
- map metadata
- reward screen context
- temporary combat-only zones
- replay/continue edge cases
- if current system uses weak tables that will not survive save/load
- if state only exists in runtime UI object and will be lost

Pay special attention to:
- Urda progress string in `UrdaStateKey`
- Urda card reward context `ConditionalWeakTable<CardReward, CardRewardContext>`
- Urda Seedbed reward alternatives after save/load on reward screen
- Humus skip state
- Molting Withered Husk act cleanup
- Moss Map room mask
- Rootblight card state and master-deck changes
- Blight Sprout runtime cards
- A11 map modifications
- A12/A16/A19 map metadata stored in `ConditionalWeakTable`
- A20 second boss metadata
- future Morvi/Lotha per-card markers and debt state

Phase 8 — experiment loopholes and exploit audit

Create a section in `overnight-source-audit.md`.

Audit likely exploits:

- Power cards copied/extra-played by future Morvi/Lotha effects.
- Extra-play recursion.
- Copies triggering “first card each turn” effects multiple times.
- Temporary generated cards entering master deck.
- Debt payment rounding exploitation.
- Red Ink Overdraft repeated button clicking.
- Card reward alternatives double-completing reward.
- Save/load before/after reward alternative to duplicate card/gold.
- Urda Seedbed accept/skip/reopen exploit.
- Humus third skip remove/reward duplication.
- Moss Map room entry duplicate on save/load.
- A20 Boss 1 reward -> second boss transition duplication.
- Fission repeated enchantment on same reward screen.
- Rootblight played/removal/downgrade duplication.
- Multiplayer host/client both applying rewards.

For each exploit:
- Feature
- Preconditions
- Whether current source is vulnerable
- Source evidence
- Suggested guard/test

Phase 9 — test plan consolidation

Create `manual-test-master-matrix.md`.

It must unify and prioritize manual tests:

Tier 0 — environment:
- clean BaseLib + EZMB only
- installed hash check
- clean log audit
- SavedSpireField count
- Windows/macOS command variants

Tier 1 — single-player smoke:
- A0/A10/A20 first combat
- Mod Settings
- no errors

Tier 2 — current active Urda:
- Seedbed
- Humus Pact
- Molting / Withered Husk
- Moss Map
- save/load
- disable Urda gate

Tier 3 — Ancient reward rebalance:
- Velvet Choker
- Distinguished Cape
- Prismatic Gem
- Quality Flame
- Pumpkin Candle vanilla
- save/load rows

Tier 4 — Ascension:
- A11 natural traversal
- A12 Firemark variety/preview
- A13 Fission diagnostics
- A14/A15/A18 Rootblight/Blight Sprout
- A16 Banner variety/preview
- A19 Boss Seal preview
- A20 second boss/intermission/brand

Tier 5 — multiplayer:
- A10 control
- A11 default
- A12
- A14
- A16
- A20
- save/quit
- host/client logs
- ownership/desync

Tier 6 — future v2.2 planning:
- Morvi/Lotha/Vakuu not active
- source API blockers only

Phase 10 — next implementation goals

Create `next-implementation-goals.md`.

Produce a prioritized list:

1. Critical safety fixes before more content.
2. Current RC blockers that must be closed.
3. Active Urda verification/fixes.
4. Ascension preview/balance/final polish.
5. Multiplayer blocking investigation.
6. Only then Ancient Expansion v2.2 Milestone 1.

For each goal:
- title
- why
- files likely to change
- source evidence needed
- implementation sketch
- acceptance criteria
- tests/commands
- manual evidence

Do not recommend implementing Morvi/Lotha/Vakuu before current RC blockers are acknowledged.

Phase 11 — optional helper scripts

Only if low risk:

- Improve `scripts/check-installed-ezmb-package.ps1` docs.
- Add macOS equivalent notes, not necessarily a new script.
- Do not break Windows scripts.

Phase 12 — validation

Run:

- `git status --short --branch`
- `git log -1 --oneline --decorate`
- `dotnet build EZMicroBalance.sln`
- `dotnet test EZMicroBalance.sln --no-build`
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`
- `git diff --check`

If docs/tests only:
- Do not publish/package.

If code/scripts changed:
- run full build/test; publish only if code/resources/package state changed.

Final response format:

1. Summary: audit completed / incomplete.
2. Current HEAD and git status.
3. Files created/changed.
4. Source folders inspected:
   - `source code/src/Core/**`
   - `sourcecodeonlyaianalysis/**`
   - mod source paths
5. Major findings:
   - implementation gaps
   - source API blockers
   - multiplayer risks
   - save/load risks
   - Windows/Mac risks
   - exploit risks
6. Top 10 next fixes.
7. What not to implement yet.
8. Validation command results.
9. Release-ready: no.
