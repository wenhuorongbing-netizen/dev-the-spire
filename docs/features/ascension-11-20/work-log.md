# Ascension 11-20 Work Log

Project: EZ Micro Balance  
Manifest id: EZMicroBalance

Current status note: entries below are chronological history. The current automated test baseline is recorded in `docs/features/ancients-rework-v4/completion-audit.md` after each validation refresh; earlier 24-test, 28-test, 34-test, 46-test, 56-test, 57-test, 58-test, 63-test, and 64-test entries are retained as historical evidence from before later guard additions.

## 2026-05-08 - Multiplayer A11-A20 P0 Triage: HP0/Neow Blocked / Save-Quit / Black Screen

Scope:

- Added gated multiplayer diagnostics (`EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS=1`) with Harmony patches on:
  - `StartRunLobby.BeginRunForAllPlayers` — lobby state before run start
  - `StartRunLobby.BeginRunLocally` — ascension/player count at local launch
  - `StartRunLobby.UpdateMaxMultiplayerAscension` — ascension cap computation
  - `NGame.StartNewMultiplayerRun` — RunState player HP post-creation
  - `RunManager.EnterAct` — player HP before and after act entry
  - `AncientEventModel.BeforeEventStarted` — player HP before/after Neow healing
  - `RunManager.SaveRun`, `NGame.ReturnToMainMenu`, `NGame.Quit` — save/quit/disconnect logging
- All patches default off; no gameplay changes.
- Added `EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS` env var to `AscensionFeatureGate`.
- Added P0 issues to `docs/issues.md`:
  - `ISSUE-2026-05-08-MULTIPLAYER-A11-A20-RUN-START-HP0-NEOW-BLOCKED`
  - `ISSUE-2026-05-08-MULTIPLAYER-SAVE-QUIT-NOT-PROPAGATING`
  - `ISSUE-2026-05-08-MULTIPLAYER-RUN-START-BLACK-SCREEN`
- Added P0 triage matrix to `multiplayer-test-runbook.md` with rows A-F.
- Source evidence reviewed in `NGame.cs`, `StartRunLobby.cs`, `RunManager.cs`, `AncientEventModel.cs`, `Neow.cs`, `AscensionManager.cs`, `Player.cs`, `LobbyPlayer.cs`.

Key source findings:

1. **Neow HP healing flow** (`AncientEventModel.BeforeEventStarted`, line 143-156):
   - Sets player HP to 0 via `SetCurrentHpInternal(0m)`.
   - Heals to full (MaxHp - 0) or 80% for A2+ (WearyTraveler).
   - For A20: expected heal = 64 HP (80% of 80), not 0.
   - `CreatureCmd.Heal` calls `creature.HealInternal(amount)` directly — no queue dependency.
   - No EZMB code touches HP during this flow.

2. **Vanilla AscensionManager** (`AscensionManager.cs`):
   - `maxAscensionAllowed = 10` (const, used only for progress clamping).
   - Constructor accepts `int level` directly — no clamping.
   - `HasLevel(AscensionLevel)` checks `_level >= (int)level` — works for values > 10.
   - `ApplyEffectsTo(player)` only handles A4 (TightBelt -1 potion) and A10 (AscendersBane). No HP effects.

3. **Run start flow** (`NGame.StartNewMultiplayerRun`):
   - `RunState.CreateForNewRun()` with `ascensionLevel` from lobby → `Player.CreateForNewRun()` uses `character.StartingHp` for both current and max HP.
   - `RunManager.SetUpNewMultiPlayer()` → `InitializeNewRun()` → `ApplyAscensionEffects()` (no HP change).
   - `StartRun()` → `RunManager.Instance.EnterAct(0, doTransition: false)` → Neow event starts → `BeforeEventStarted` fires.

4. **Save/quit** (`NSaveAndQuitButton.cs`):
   - Local source snapshot file is empty (1 byte). Cannot analyze from source alone.
   - `RunManager.CleanUp()` calls `NetService.Disconnect(NetError.Quit, !graceful)`.
   - `NGame.Quit()` saves settings/progress and calls `GetTree().Quit()` — does not send network disconnect.

5. **Player HP initialization**:
   - `Player.CreateForNewRun(CharacterModel character, ...)` constructor: `new Player(character, netId, character.StartingHp, character.StartingHp, ...)`.
   - No ascension-based HP reduction in vanilla or EZMB code.
   - Rootblight card additions do not affect HP.

Hypotheses (ranked):
1. (Most likely) Vanilla `CreatureCmd.Heal` or `AncientEventModel.BeforeEventStarted` skips execution for non-host players in multiplayer when `RunState.AscensionLevel > 10`, possibly due to `State.ExtraFields.StartedWithNeow` flag mismatch or `ActionExecutor` not yet unpaused.
2. (Less likely) v0.105.0 API drift: `AncientEventModel` or `CreatureCmd` changed in a way that breaks the heal path for ascension > 10.
3. (Less likely) EZMB patch corruption: our patches on `UpdateMaxMultiplayerAscension`, `BeginRunLocally`, etc. corrupt some lobby/run state before multiplayer run start.
4. (Unlikely) Neow event type-load failure: if `Neow` or its base classes fail to load for a non-host player, `BeforeEventStarted` never fires.

Required next steps:
- Run live co-op triage rows A-F from multiplayer-test-runbook.md with `EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS=1`.
- Collect host/client `godot.log` and analyze HP values at each diagnostic point.
- Source evidence is from v0.104.0 snapshot; game is now v0.105.0. Confirm no API drift in `AncientEventModel.BeforeEventStarted`, `CreatureCmd.Heal`, `Creature.HealInternal`.

Scope:

- Migrated the old Root/Deep Root/Root Bud prototype toward v2.0 Rootblight I/II/III and Blight Sprout semantics.
- Kept edits to root-related Ascension code, card/Ascension localization, and Ascension root docs.
- Did not edit Banner, Firemark, Fission, Boss Seal, Ancient, release packaging, or test files.

Implemented:

- Added saved per-player Rootblight level state while retaining the one-time starter marker for migration safety.
- Added Rootblight III and changed the legacy Root/Deep Root classes into Rootblight I/II display states.
- Rootblight play now lowers saved level by 1 only from card play; discard and non-play exhaust do not lower the level.
- Combat end sync updates the one master-deck Rootblight card to the current level or removes it at level 0.
- Rootblight deck sync enforces one master-deck Rootblight card max and suppresses its own deck-removal clear hook.
- Real rest heal clears Rootblight; smith does not.
- Normal deck-removal APIs, including shop removal, clear Rootblight through `BeforeCardRemoved`.
- Boss Blight Sprout is now restricted to Act 2/3 bosses; Act 1 elites remain excluded from elite Blight Sprout.
- Blight Sprout growth now raises Rootblight level +1 up to III.
- English and Simplified Chinese card/Ascension localization now uses Rootblight/Blight Sprout terminology.

Validation:

- Localization JSON parse check passed for `eng`/`zhs` card and Ascension tables.
- `dotnet build EZMicroBalance.sln` was attempted and is currently blocked by unrelated non-root Banner errors in `EZMicroBalanceCode/Ascension/AscensionCombatModifierService.cs` and `EZMicroBalanceCode/Ascension/Powers/BannerPowers.cs`.
- The build output did not report Rootblight/Blight Sprout compile errors before failing on those Banner sources.

Files changed:

- `EZMicroBalanceCode/Ascension/AscensionDiagnostics.cs`
- `EZMicroBalanceCode/Ascension/AscensionSavedStateFields.cs`
- `EZMicroBalanceCode/Ascension/Cards/RootCards.cs`
- `EZMicroBalanceCode/Ascension/RootBudCombatHook.cs`
- `EZMicroBalanceCode/Ascension/RootDeckService.cs`
- `EZMicroBalanceCode/Ascension/RootRunHook.cs`
- `EZMicroBalance/localization/eng/ascension.json`
- `EZMicroBalance/localization/eng/cards.json`
- `EZMicroBalance/localization/zhs/ascension.json`
- `EZMicroBalance/localization/zhs/cards.json`
- `docs/features/ascension-11-20/api-research.md`
- `docs/features/ascension-11-20/implementation-plan.md`
- `docs/features/ascension-11-20/manual-test-checklist.md`
- `docs/features/ascension-11-20/work-log.md`

Remaining:

- Refresh source guards for the renamed player-facing Rootblight/Blight Sprout expectations.
- Fix or isolate the unrelated Banner compile errors before claiming a successful build.
- Re-run build, publish after a successful build, and perform live save/load, rest, shop-removal, non-play exhaust, boss, and elite verification.

## 2026-05-07 - v2.0 Development Checklist Intake

Scope:

- Added the user-provided A11-A20 v2.0 feature GDD as the forward-looking development checklist.
- Did not change gameplay code in this intake pass.

Added:

- `docs/features/ascension-11-20/development-checklist-v2.md`

Planning impact:

- The existing Ascension code remains prototype state.
- Future implementation work should audit/migrate Root-family, Firemarked Elite, Forge Token, Fission, Banner Room, Boss Seal, and A20 behavior against the v2.0 checklist before expanding scope or claiming readiness.
- The v2.0 plan introduces Rootblight levels I/II/III, Blight Sprout terminology, stronger host-only Firemarks, explicit Fission probability targets, first-batch Banner Rooms, Deep Branch milestones, Boss-specific Royal Seals, Dual King Brands, telemetry, and milestone acceptance cases.

Docs updated:

- `README.md`
- `docs/PROJECT_MAP.md`
- `docs/features/ascension-11-20/implementation-plan.md`
- `docs/features/ascension-11-20/source-design.md`
- `docs/features/ascension-11-20/work-log.md`

## 2026-05-07 - Root Bud and Fission Bugfix Pass

Scope:

- Fixed live-tester feedback for Root Bud growth and Fission card text.
- Did not change A12 Firemarked Elite strength tuning; tester said new tuning would come later.
- Kept changes inside Ascension code/docs/tests except for shared release/hash docs.

Implemented:

- Root Bud entered-hand tracking now also listens to `AfterCardDrawn(...)`, so normal draws mark the bud even if pile-change timing misses the hand transition.
- Root growth now attempts `CardCmd.Transform(root, deepRoot, ...)` first, with remove/add fallback, so a deck Root visibly becomes Deep Root instead of only updating internal state.
- Fission extra card text no longer uses `{energyPrefix:energyIcons(1)}` and no longer repeats the added Exhaust keyword line. The body text is plain energy-cost wording; the Exhaust keyword supplies the single added Exhaust line.
- Fission reward eligibility source guards now explicitly require no existing enchantment and no existing/self-exhausting behavior.

Validation:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 69/69 after the smoke-log guard was corrected to use recent controlled smoke logs rather than the user's volatile live `godot.log`.
- `dotnet publish EZMicroBalance.sln`: passed; installed DLL/manifest refreshed and PCK hash stayed current.
- Package zip SHA256: `65277A910077FA655C9F8D5FD15C2A6B4515228FDD27BD916BF85A7CEEB33FB4`.
- Installed DLL SHA256: `022A1ADFD2CDD9C3755ED248FE087220E32539C4EEC1BECAECF5A3A9BF612365`.
- Installed PCK SHA256: `BD14EB5924F852873DAFA570162BE039366BB13334B737E0792A3F9B0B1F59AA`.

Remaining:

- Root/Deep Root/Root Bud, Fission reward pickup/save-load, and all other Ascension slices still need live gameplay verification.

## 2026-05-07 - A12 Player Text and Ascension Icon Art Pass

Scope:

- Removed the public A12 description's route-exclusivity promise while keeping route-exclusive placement as an internal implementation strategy.
- Reworked the A12/A13 small icon assets after source-art style review.
- Did not launch the game.

Implemented:

- `LEVEL_12.description` now says that about 3 optional elites are Firemarked and that defeating one grants a Forge Token for the next Rest Site.
- The Firemarked Elite indicator, Forge Token status relic, and Fission enchantment icon were redrawn as original transparent 64x64 assets.
- Added an automated localization guard so the public A12 description does not mention route limiting again.

Asset hashes:

- `firemarked_elite_indicator.png`: `8613ECAD4FFC1677FE04EAEE1C67706005535E1F7CF74A234D2496F31ED21958`.
- `forge_token_status.png`: `98E80C181021E6B6DD4D0E01706FE39A656B3079E295C62591C44F4DA98C8E74`.
- `fission_enchantment_icon.png`: `5309096E1AD87A0C5AD97848018985DF61A9D02D5515BC61E78D293423DF09DC`.

Validation:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet publish EZMicroBalance.sln`: passed and reimported the three Ascension image resources.
- Package zip SHA256: `378A7F8F0571CA179BCF65E63684E6219479D35A7968FC131CB57F1A98D867DA`.
- Installed DLL SHA256: `8BDA03860BF4483A22222DDC0CE5F5F7DE09C6C7F371E030634B86C320364C9C`.
- Installed PCK SHA256: `BD14EB5924F852873DAFA570162BE039366BB13334B737E0792A3F9B0B1F59AA`.

## 2026-05-06 - A12/A13 Player-Facing Indicator and Rules Pass

Scope:

- Tightened the current A12/A13 development slices after live tester feedback.
- Stayed inside Ascension code/resources/docs/tests.
- Did not add game-launch/runtime testing in this pass.

Implemented:

- Added original Ascension image assets for the firemarked elite map indicator, Forge Token status relic, and Fission enchantment icon.
- A12 Firemarked Elite now targets route-spread optional elite nodes when enough safe candidates exist, while retaining the Act 1 first-rest-site restriction and safe alternate-boss-route check.
- Firemarked Elite uses a dedicated `FiremarkedEliteMapQuestMarker` and a narrow `NNormalMapPoint.RefreshMarkedIconVisibility` postfix to swap only firemarked quest icons away from the generic Fur Coat / spoils-style marker.
- Firemark combat type is shown as a visible enemy power on the selected host.
- Forge Token is now mirrored as a visible one-count Event-rarity status relic with hover text; duplicates convert to 15 gold.
- Forge Token rest payout now randomly upgrades one upgradable common/uncommon card on Rest, heals 7 after Smith, and fallback-heals 5 if Rest has no valid target.
- Heal rest options add player-facing Forge Token extra text before selection.
- A13 Fission now has a dedicated enchantment icon and Exhaust hover tip.
- Fission eligibility now excludes Powers, X-cost cards, star-cost cards, zero-energy cards, cards with Exhaust, cards already set to exhaust on next play, quest/special cards, and incompatible existing enchantments.
- English/ZHS Ascension text was refreshed; ZHS Fission wording uses "耗能" and no longer uses "费用" for this mechanic.

Validation:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors after API fixes.
- Guard tests were expanded for firemark icon routing, visible token relic behavior, random token upgrade targeting, Fission icon/text/eligibility, and zhs wording.
- `dotnet publish EZMicroBalance.sln`: passed and exported the three new Ascension image resources.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 69/69.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with existing CRLF normalization warnings for `EzDailyContent.json` and `docs/dev-environment.md`.
- Package zip SHA256: `378A7F8F0571CA179BCF65E63684E6219479D35A7968FC131CB57F1A98D867DA`.
- Installed DLL SHA256: `8BDA03860BF4483A22222DDC0CE5F5F7DE09C6C7F371E030634B86C320364C9C`.
- Installed PCK SHA256: `BD14EB5924F852873DAFA570162BE039366BB13334B737E0792A3F9B0B1F59AA`.

## 2026-05-06 - Single-Player A11-A20 Original-UI Selector Expansion

Scope:

- Implemented the requested development-test path for selecting A11-A20 through the original single-player Ascension UI.
- Kept the patch limited to `StartRunLobby` single-player, non-daily selection/start paths.
- Did not patch `NAscensionPanel`, the global `CharacterStats.MaxAscension` getter, `ProgressState`, `ProgressSaveManager`, or `AscensionManager.maxAscensionAllowed`.
- Did not implement A11 map geometry, A17 branch insertion, A20 intermission, A21-A30, custom-character work, or multiplayer support.

Implemented:

- `AscensionSelectionPatches` expands the single-player lobby `MaxAscension` backing field to 20 after character selection.
- A11-A20 selected run levels now activate the existing Ascension slices through `RunState.AscensionLevel`.
- `BeginRunLocally` temporarily raises local `CharacterStats.MaxAscension` only while launching an A11-A20 run, then restores it through a Harmony finalizer.
- `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` disables the selector expansion for comparison testing.
- Added English and Simplified Chinese `ascension.json` keys for `LEVEL_11` through `LEVEL_20` and exported them into the PCK.

Validation:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet publish EZMicroBalance.sln`: passed; installed DLL/manifest refreshed and PCK contains 33 entries including both Ascension localization files.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 68/68.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with existing CRLF normalization warnings for `EzDailyContent.json` and `docs/dev-environment.md`.
- Package zip SHA256: `F8DC81CCB663CB4E8B5536BA51A0A27DB9D9F4EE2C4510F707A7A1B582472268`.
- Installed DLL SHA256: `9EB20A0CCDB986E54FD16542284D9E1C68993D4DD50DD566EFA93A38FC1F75EE`.
- Installed PCK SHA256: `9EC5B4320C55B5EC501834321368CF9DFAA6F2F8817519E829B21D178CD59234`.

Remaining:

- Live Steam-client selection/start verification is still pending.
- Historical note: this entry is superseded by the later A11 geometry passes. Current A11 geometry is width +1 plus Act 1 +1 route row, Act 2 +1 route row, and Act 3 +2 route rows, with no A11-specific marker/icon/hover tooltip.

## 2026-05-06 - Research Spec Mode

Mode:

- Research spec only.
- No gameplay implementation.
- No cards, patches, map mutation, reward mutation, combat mutation, rest-site mutation, save mutation, or Ascension behavior changes.

User request:

- Start Ascension 11-20 expansion research.
- Read project instructions and local source design.
- Inspect current EZ Micro Balance architecture.
- Run git status and build if safe.
- Produce API research, implementation plan, manual test checklist, and work log.

Files read:

- `AGENTS.md`
- `docs/features/ascension-11-20/source-design.md`
- `EZMicroBalance.sln`
- `EZMicroBalance.csproj`
- `EZMicroBalance.json`
- `EZMicroBalanceCode/MainFile.cs`
- `docs/architecture-ez-micro-balance.md`
- Existing Ancient implementation and API discovery docs for local patterns.

Architecture notes:

- Active private beta manifest id remains `EZMicroBalance`.
- Code is compiled from `EZMicroBalanceCode/**/*.cs`.
- Resources and localization are packaged from `EZMicroBalance/**`.
- Existing Ancient reward rebalance work remains isolated and was not modified.

Git status:

- Ran `git status --short --branch`.
- Worktree already had many existing modified, deleted, and untracked files from the independent EZ Micro Balance setup and documentation reorganization.
- No attempt was made to revert or clean unrelated changes.

Build safety:

- Checked for `SlayTheSpire2.exe`.
- The process was not running.
- Used normal solution build path.

Build:

- Command: `dotnet build EZMicroBalance.sln`
- Result: succeeded.
- Warnings: 0.
- Errors: 0.

API inspection:

- Inspected local BaseLib v3.1.0 signatures.
- Inspected local StS2 public beta v0.104.0 signatures.
- Used signature/high-level relationship inspection only.
- Did not copy decompiled game method bodies into repository docs.

Research outputs created:

- `docs/features/ascension-11-20/api-research.md`
- `docs/features/ascension-11-20/implementation-plan.md`
- `docs/features/ascension-11-20/manual-test-checklist.md`
- `docs/features/ascension-11-20/work-log.md`

Key findings:

- Current run Ascension is represented as `RunState.AscensionLevel` and serialized as an integer.
- Public max Ascension is hard-bounded to 10 in progress/UI-related paths.
- Run hooks, combat hooks, card commands, reward hooks, rest-site structures, map structures, enchantments, and multiplayer ownership APIs are available for staged implementation research.
- A14 Root closed-loop MVP appears plausible behind a debug/internal gate, but public A14 selection is not yet safe.
- Custom card pool/visual behavior remains a specific proof item before implementing Root.
- Map dimension changes, A20 intermission, and public A11-A20 unlock/display are high-risk and deferred.

Limitations:

- No game launch or live gameplay verification was performed in this research pass.
- No publish was run because no resources or packaging behavior were changed before the docs-only update.
- Multiplayer behavior was inspected by signatures only and remains untested for Ascension features.
- Harmony patch points remain candidate-only; none were implemented.

Next requested transition:

- Stay in Research Spec Mode until the user says exactly `build-ascension-mvp`.

## 2026-05-06 - Subagent D Gated Root-Family Build Slice

Mode:

- Implementation within Subagent D ownership scope.
- No edits to tests.
- No manifest id changes.
- No public A11-A20 selector/progress patch.
- No map, reward, rest-site, boss seal, A20 intermission, Ancient, legacy scaffold, or test edits.
- No new Harmony patches.

Files changed:

- `EZMicroBalanceCode/Ascension/AscensionFeatureGate.cs`
- `EZMicroBalanceCode/Ascension/AscensionInitializer.cs`
- `EZMicroBalanceCode/Ascension/AscensionSavedStateFields.cs`
- `EZMicroBalanceCode/Ascension/Cards/RootCards.cs`
- `EZMicroBalanceCode/Ascension/RootDeckService.cs`
- `EZMicroBalanceCode/Ascension/RootRunHook.cs`
- `EZMicroBalanceCode/Ascension/RootBudCombatHook.cs`
- `EZMicroBalance/localization/eng/cards.json`
- `EZMicroBalance/localization/zhs/cards.json`
- `docs/features/ascension-11-20/api-research.md`
- `docs/features/ascension-11-20/implementation-plan.md`
- `docs/features/ascension-11-20/manual-test-checklist.md`
- `docs/features/ascension-11-20/work-log.md`

Implemented:

- Default-off internal/debug gate:
  - `EZMB_ASCENSION_DEBUG_LEVEL=14` enables A14 Root Begins for internal testing.
  - `EZMB_ASCENSION_DEBUG_LEVEL=15` also enables boss Root Bud.
  - `EZMB_ASCENSION_DEBUG_LEVEL=18` also enables elite Root Bud.
  - `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` exists only as a future public-gate switch and remains off by default.
- `Root` custom card:
  - Cost 2.
  - Curse-rarity curse-type card using the existing placeholder card portrait resources.
  - Exhaust keyword.
  - On play, permanently removes the linked master-deck `DeckVersion` through `CardPileCmd.RemoveFromDeck`.
- `DeepRoot` custom card:
  - Cost 3.
  - Same removal behavior as Root.
  - Used as the capped Root growth state.
- Root start insertion:
  - `ModHelper.SubscribeForRunStateHooks`.
  - `AfterActEntered`.
  - Act 1 only.
  - One Root-family card max per active player by deterministic deck scan.
- `RootBud` custom temporary combat card:
  - Cost 2.
  - Added to discard at boss combat start when the A15 gate is active.
  - Added to discard at elite combat start when the A18 gate is active.
  - Sprouts to top of draw pile on/after round 3 if it has not entered hand.
  - Tracks entered-hand, played, and sprouted flags with `SavedSpireField<RootBud,bool>`.
  - If it entered hand and was not played before combat end, grows Root:
    - no Root -> add Root;
    - Root -> replace with Deep Root;
    - Deep Root -> cap, no further growth.
- English and Simplified Chinese Ascension card localization keys:
  - `EZMB_ROOT.*`
  - `EZMB_DEEP_ROOT.*`
  - `EZMB_ROOT_BUD.*`

API evidence used:

- `ModHelper.SubscribeForRunStateHooks(string, RunHookSubscriptionDelegate)`.
- `ModHelper.SubscribeForCombatStateHooks(string, CombatHookSubscriptionDelegate)`.
- `AbstractModel.AfterActEntered`, `BeforeCombatStart`, `AfterPlayerTurnStart`, `AfterCardChangedPiles`, `AfterCardPlayed`, and `AfterCombatEnd` are virtual hook methods.
- `AbstractModel.ShouldReceiveCombatHooks` is abstract and was implemented explicitly on hook models.
- `RunState.CreateCard<T>(Player)` and `ICardScope.CreateCard<T>(Player)`.
- `CombatState.CreateCard<T>(Player)`.
- `CardPileCmd.Add(...)`.
- `CardPileCmd.AddGeneratedCardToCombat(...)`.
- `CardPileCmd.RemoveFromDeck(CardModel, bool)`.
- `CardModel.DeckVersion`.
- `CardModel.OnPlay(...)`, `CardKeyword.Exhaust`, and `CardModel.ExhaustOnNextPlay`.
- `BaseLib.Utils.Attributes.CustomIDAttribute`.
- `BaseLib.Utils.PoolAttribute`.
- `SavedSpireField<TKey,TValue>`.

Commands:

- `dotnet build EZMicroBalance.sln`
  - First result: failed.
  - Evidence:
    - `AbstractModel.ShouldReceiveCombatHooks` required explicit implementation.
    - `CustomIDAttribute` namespace is `BaseLib.Utils.Attributes`.
    - `CardPlay.ResultPile` is init-only and cannot be set during `OnPlay`.
  - Fix:
    - Implemented `ShouldReceiveCombatHooks`.
    - Added the exact `CustomIDAttribute` namespace.
    - Switched from setting `CardPlay.ResultPile` to `ExhaustOnNextPlay` plus `CardKeyword.Exhaust`.
- `dotnet build EZMicroBalance.sln`
  - Result: succeeded.
  - Warnings: 0.
  - Errors: 0.
- `dotnet test EZMicroBalance.sln`
  - Result: succeeded.
  - Tests: 24 passed, 0 failed, 0 skipped.
- Checked for `SlayTheSpire2.exe` before publish.
  - Result: process not running.
- `dotnet publish EZMicroBalance.sln`
  - Result: succeeded.
  - Copied release DLL and manifest to the mods folder.
  - Exported the Godot `.pck`.
  - Godot emitted the known non-fatal script-scan `FileNotFoundException` for assembly `sts2` during export, then completed `savepack`.
- `dotnet test EZMicroBalance.sln`
  - Final package verification result: succeeded.
  - Tests: 24 passed, 0 failed, 0 skipped.

Blockers / deferred slices:

- Public A11-A20 selector/progress remains unsafe because vanilla max Ascension/progress paths are still hard-bounded to 10.
- Forge Token is blocked on safe map marking, firemarked elite persistence, and rest-site payout hook proof.
- Fission is blocked on reward-card mutation/reroll/save-load proof.
- Firemarked Elite and Banner Rooms are blocked on visible map-node marking and node-to-combat metadata persistence proof.
- Deep Branches remain high risk because map dimension/edge insertion and save/load invariants are not proven.
- Boss Seals and A20 intermission remain high risk because boss reward/transition mutation is not proven.
- Root-family runtime behavior, custom card registration/visuals, mid-combat Root Bud save/load, and multiplayer synchronization remain unverified until live game testing.

## 2026-05-06 - Main-Agent Ascension Review Integration

Scope:

- Integrated read-only Ascension reviewer findings from Subagent E.
- Kept the implementation isolated under `EZMicroBalanceCode/Ascension/**`.
- Added no public Ascension selector/progress patch, no map/reward/rest-site/boss-flow mutation, and no new Harmony patch.

Findings addressed:

- Root starter seeding needed persisted one-time state. Added `SavedSpireField<Player,bool>` `RootBeginsApplied` and retained the Root-family deck scan as duplicate protection.
- Root Bud combat seeding needed protection against hook re-entry after reload/scene restore. Added an active combat-pile scan before seeding so each active player gets at most one Root Bud from the hook.
- Root-family cards remain in `CurseCardPool` for BaseLib registration. Known generation flags are disabled and automated source guards now track that constraint; live random transform/reward behavior still needs runtime verification.

Files changed:

- `EZMicroBalanceCode/Ascension/AscensionSavedStateFields.cs`
- `EZMicroBalanceCode/Ascension/RootDeckService.cs`
- `EZMicroBalanceCode/Ascension/RootBudCombatHook.cs`
- `tests/EZMicroBalance.Tests/AscensionFeatureGuardTests.cs`
- `docs/features/ascension-11-20/api-research.md`
- `docs/features/ascension-11-20/implementation-plan.md`
- `docs/features/ascension-11-20/manual-test-checklist.md`
- `docs/features/ascension-11-20/work-log.md`

Commands:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 28 tests total.

Remaining blockers:

- Public A11-A20 selector/progress remains blocked by hard max-10 UI/progress evidence.
- Root/Deep Root/Root Bud runtime behavior, card registration/visuals, save/load, and multiplayer behavior still require live game verification.

## 2026-05-06 - Publish Status After Gated Slice

Validation:

- `dotnet publish EZMicroBalance.sln`: passed after the gated Ascension slice and localization/resource changes.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 28 tests total.
- The final private-beta zip was rebuilt after publish; installed, staging, and versioned package DLL/JSON/PCK hashes match.

Release posture:

- The Root-family slice remains internal/debug-only by default.
- Public A11-A20 selection/progress remains unsupported.
- Live Ascension gameplay, card visuals/localization in game, save/load, and multiplayer checks remain pending manual verification.

## 2026-05-06 - Startup Smoke Constructor Fix

Finding:

- The first bounded `--force-steam off` smoke after publishing the Ascension slice failed before main menu with `System.MissingMethodException` for `EZMicroBalance.EZMicroBalanceCode.Ascension.RootBudCombatHook`.
- The stack showed StS2 model database startup dynamically creating concrete `AbstractModel` instances.

Fix:

- Added parameterless constructors to `RootRunHook` and `RootBudCombatHook`.
- Parameterless instances remain inactive; actual subscribed hook instances still receive `RunState` or `CombatState`.
- Added automated source guards for the constructors and inactive combat-hook state.

Validation:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 28 tests total.
- `dotnet publish EZMicroBalance.sln`: passed and exported the selected-resource PCK.
- Final bounded `--force-steam off` smoke initialized BaseLib and EZ Micro Balance, reported 7 SavedSpireFields, and reached main menu.
- Later package-refresh smoke initialized only BaseLib and EZ Micro Balance, reported 8 SavedSpireFields after the broader A11-A20 gated slice, and reached main menu.

Remaining:

- The Root-family gameplay itself is still not live-verified. Public A11-A20 selection/progress remains unsupported.

## 2026-05-06 - Subagent D Read-Only Diagnostics Slice

Scope:

- Verified the current Root/Deep Root/Root Bud slice against the Ascension source design, API research, implementation plan, manual checklist, and existing `EZMicroBalanceCode/Ascension/**`.
- Added only a default-off, non-mutating diagnostics slice under Subagent D's Ascension ownership scope.
- Did not change manifest ids.
- Did not touch Ancient code.
- Did not edit tests.
- Did not add public A11-A20 selector/progress support.
- Did not mutate map, reward, rest-site, boss-flow, save serialization, or progress systems.

Files changed:

- `EZMicroBalanceCode/Ascension/AscensionDiagnostics.cs`
- `EZMicroBalanceCode/Ascension/AscensionFeatureGate.cs`
- `EZMicroBalanceCode/Ascension/AscensionInitializer.cs`
- `EZMicroBalanceCode/Ascension/RootRunHook.cs`
- `EZMicroBalanceCode/Ascension/RootBudCombatHook.cs`
- `docs/features/ascension-11-20/api-research.md`
- `docs/features/ascension-11-20/implementation-plan.md`
- `docs/features/ascension-11-20/manual-test-checklist.md`
- `docs/features/ascension-11-20/work-log.md`

Implemented:

- New explicit internal diagnostics gate:
  - `EZMB_ASCENSION_DIAGNOSTICS=1`.
  - Default off.
  - Does not enable any gameplay system by itself.
- Diagnostic hook registration:
  - Run/combat hook subscribers are created when either gameplay gates are active or diagnostics are active.
  - Parameterless hook constructors remain intact for model database startup.
- Diagnostic logging:
  - Run Ascension level.
  - Act index.
  - Debug level.
  - Public gate state.
  - Root starter saved marker.
  - Root and Deep Root deck counts per active player.
  - Combat room type.
  - Combat round.
  - Boss/elite Root Bud gate state.
  - Combat Root Bud counts per active player.

Verification:

- `dotnet build EZMicroBalance.sln`
  - First result: failed with one diagnostics helper type mismatch because `CombatState.RunState` was exposed through `IRunState` in that call path.
  - Fix: changed the diagnostics helper to accept `IRunState` for shared player-slot logging.
- `dotnet build EZMicroBalance.sln`
  - Result: succeeded.
  - Warnings: 0.
  - Errors: 0.
- `dotnet test EZMicroBalance.sln --no-build`
  - First result: failed one existing source-guard test because the Root run hook no longer contained the literal `runState != null` snippet after refactoring.
  - Fix: reshaped the null guard in `RootRunHook` without changing behavior and without editing tests.
- `dotnet build EZMicroBalance.sln`
  - Result: succeeded.
  - Warnings: 0.
  - Errors: 0.
- `dotnet test EZMicroBalance.sln --no-build`
  - Result: succeeded.
  - Tests: 34 passed, 0 failed, 0 skipped.

Completion assessment:

- Root/Deep Root/Root Bud remain compile- and source-guard-proven only. Existing startup smoke evidence remains valid, but this pass did not run a new live game smoke.
- The new diagnostics slice is compile/test proven and ready for live log verification.

Remaining unsafe/deferred:

- Public A11-A20 selector/progress remains unsafe because vanilla UI/progress paths are documented hard-bounded to 10.
- Firemarked Elite, Banner Rooms, and Deep Branches remain blocked on safe visible map marking and map invariant proof.
- Forge Token remains blocked on firemarked elite persistence and rest-site payout hook proof.
- Fission remains blocked on reward-card mutation, reroll persistence, and picked-card save/load proof.
- Boss Seals and A20 intermission remain blocked on boss-flow/reward transition proof.
- Root-family gameplay, diagnostics log output, card registration/visuals, save/load behavior, and multiplayer behavior still require live manual verification.

## 2026-05-06 - Subagent C A11-A20 Default-Off Implementation Pass

Scope:

- Implemented within Ascension 11-20 code/docs only.
- Did not change manifest ids.
- Did not touch Ancient behavior, legacy scaffold behavior, public selector/progress, A21-A30, or custom-character code.
- Added no new Harmony patches.

Files changed:

- `EZMicroBalanceCode/Ascension/AscensionFeatureGate.cs`
- `EZMicroBalanceCode/Ascension/AscensionSavedStateFields.cs`
- `EZMicroBalanceCode/Ascension/RootRunHook.cs`
- `EZMicroBalanceCode/Ascension/RootBudCombatHook.cs`
- `EZMicroBalanceCode/Ascension/AscensionNodeMetadata.cs`
- `EZMicroBalanceCode/Ascension/AscensionMapQuestMarker.cs`
- `EZMicroBalanceCode/Ascension/AscensionMapService.cs`
- `EZMicroBalanceCode/Ascension/AscensionCombatTracker.cs`
- `EZMicroBalanceCode/Ascension/AscensionCombatModifierService.cs`
- `EZMicroBalanceCode/Ascension/FissionEnchantment.cs`
- `EZMicroBalanceCode/Ascension/AscensionRewardService.cs`
- `EZMicroBalanceCode/Ascension/ForgeTokenService.cs`
- `docs/features/ascension-11-20/api-research.md`
- `docs/features/ascension-11-20/implementation-plan.md`
- `docs/features/ascension-11-20/manual-test-checklist.md`
- `docs/features/ascension-11-20/work-log.md`

Implemented behind default-off internal gate:

- A11: map dimension audit logging only; no geometry mutation.
- A12: optional existing-node Firemarked Elite marking, firemark combat modifiers, saved Forge Token, duplicate-token gold conversion, heal-rest upgrade/fallback heal, and smith-rest heal.
- A13: Fission custom enchantment and eligible encounter reward-card mutation.
- A14/A15/A18: retained existing Root/Deep Root/Root Bud behavior.
- A16: optional existing-node Banner Room marking and Shield/Rage/Chaos combat modifiers.
- A17: Deep Branch audit logging only; no branch insertion.
- A19: boss seal metadata/effects and fourth boss card reward option.
- A20 then had light second-boss seal metadata only when the map already had a vanilla/proven second boss point; later BossSeal/A20 v2.0 work replaced this with Boss 2 Brand metadata plus blocked runtime markers.

Exact deferrals:

- Public A11-A20 selector/progress remains unsupported because documented vanilla UI/progress paths are hard-bounded to max 10.
- Historical note: A11 wider/longer map geometry was deferred in this pass because `StandardActMap` fixed/private generation and path assignment were not safely replaceable through direct internals. This was superseded by the later `SerializableActMap` / `SavedActMap` route-row insertion approach.
- A12 special/generic rest action payout was deferred in this pass. A later 2026-05-07 Firemark/Forge Token source patch briefly wrapped `RestSiteSynchronizer.ChooseOption`, but that wrapper was removed again by the later rest-site hardening pass.
- A17 Deep Branch insertion is deferred. API evidence: `SerializableActMap`/`SavedActMap` can represent arbitrary dimensions, but inserted node/edge UI, save/load, and multiplayer route voting are unproven.
- A20 double-boss creation is deferred. API evidence: vanilla second boss setup occurs during `RunManager.GenerateRooms()` before the current run hook path can affect `StandardActMap`.
- A20 intermission is deferred. API evidence: `RunManager.EndCombatInternal()` owns boss victory, rewards, and terminal flow; no safe BaseLib hook has been proven for heal/reward insertion between boss 1 and boss 2.

Verification:

- `dotnet build EZMicroBalance.sln`: first Subagent C build failed on installed-assembly signature mismatches for `CardPileCmd.AddToCombatAndPreview` and `PowerCmd.Apply`; fixed by using the current command signatures.
- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: first run failed one release guard due exact A20 manual-checklist wording; fixed docs only.
- A later package guard run briefly failed while package README/hash docs were being synced after the new DLL/PCK artifacts; current release-facing docs now contain the updated package hash.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 46 tests total.

Remaining:

- All new A11-A20 prototype slices are compile/source-guard proven only. Live map visibility, reward reroll/pick/save-load, rest payout behavior, combat effects, second-boss partial behavior, and multiplayer behavior still require manual in-game verification before private beta readiness claims.

## 2026-05-06 - Main-Agent Release Guard And Package Sync

Scope:

- Integrated the A12/A19 tuning fixes and expanded release safety guards.
- Kept public A11-A20 selector/progress unsupported.
- Changed the active project so default Debug `dotnet build` no longer overwrites installed release artifacts; Release build/publish remains the installed-mod copy path.
- Rebuilt the private-beta package from the current installed artifacts after the release DLL changed.

Validation:

- `dotnet test EZMicroBalance.sln --no-build` after package/doc hash refresh: passed, 56 tests total.

Remaining:

- Normal Steam-client Mod Settings verification, live gameplay matrix, save/load, and multiplayer verification remain pending.

## 2026-05-06 - Reviewer P1 Map Metadata Hardening

Finding:

- Reviewer found that A12/A16/A19/A20 map-node metadata was stored in side tables and combat effects read that metadata from the current map point. If a gated run was saved and loaded before entering a marked node, the loaded map could lose side-table metadata unless it was rebuilt before combat lookup.

Fix:

- Added `AscensionMapService.TryGetCurrentMetadata(...)`, which re-applies deterministic metadata to the current `runState.Map` before reading `runState.CurrentMapPoint`.
- Routed combat modifier lookup through that method and added source guards.

Remaining:

- This is compile/source-guard proof only. Live marker visibility, save/load, and combat behavior still require manual runtime verification.

## 2026-05-06 - Ascension Builder Hardening Pass

Scope:

- Reviewed default-off A11-A20 slices against `source-design.md`, `api-research.md`, `implementation-plan.md`, and `manual-test-checklist.md`.
- Stayed within Ascension code, Ascension tests, and Ascension docs.
- Did not touch Ancient code, art assets, release packaging, or manifest ids.

Changes:

- A12 Firemarked Elite candidate selection now excludes Act 1 elite nodes at or before the first rest-site row.
- A12 firemark combat behavior now matches the documented prototype values for that pass.
- A19 Rage Seal now applies the documented +1 Strength on rounds 5 and 8.
- Added a source guard for Act 1 first-rest-site firemark placement and +1 Strength tuning.
- Added the Act 1 first-rest-site firemark check to the manual checklist and refreshed implementation/API notes.

Verification:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build --filter FullyQualifiedName~AscensionFeatureGuardTests`: passed, 6 tests total.
- `dotnet test EZMicroBalance.sln --no-build`: failed 1 package hash guard because the local build updated the installed mod DLL while package-staging/versioned zip artifacts still had the previous hash. Release packaging refresh is outside this pass.
- `dotnet test EZMicroBalance.sln --no-build --filter "FullyQualifiedName!~PackageStagingVersionedZipAndInstalledArtifactsHaveMatchingHashes"`: passed, 46 tests total.

## 2026-05-07 - Fission And Banner v2.0 Patch

Scope:

- Owned only Fission reward logic and A16 Banner Room map/combat/reward surfaces.
- Left concurrent Firemark, Deep Branch, Rootblight, boss, package, and Ancient work intact.

Changes:

- A13 Fission now rolls by reward source: normal combat 10%, Banner Room 15%, Firemarked Elite 20%, Boss 5%.
- A13 Fission still applies at most once per reward screen and keeps the strict Attack/Skill, non-X, non-star, non-zero-cost, non-Exhaust, non-special, no-existing-enchantment eligibility filter.
- A16 Banner Rooms now use `BannerRoomMapQuestMarker` and map hover text for Vanguard, Shield Formation, and Bounty public rules.
- A16 first-batch banner combat behavior is now Vanguard temporary Strength removed on round 3, Shield Formation bannerbearer Block support, and Bounty target/deadline/reward handling.
- Bounty success adds a 15 Gold room-end reward through `CombatRoom.AddExtraReward`; no monster action table edits were used.

Evidence:

- Local Core source inspected for `CardReward`, `CardFactory`, `CardCreationOptions`, `CardCreationResult`, `RewardsSet`, `CombatRoom.AddExtraReward`, `NNormalMapPoint`, `MapPoint.AddQuest`, `NHoverTipSet`, `PowerCmd`, `CreatureCmd`, and `Rng`.

Verification:

- `dotnet build EZMicroBalance.sln`: blocked before Fission/Banner compile proof by a concurrent non-owned `RootBudCombatHook.AfterSideTurnStart(CombatSide, CombatState)` override. The local source tree contains that virtual method, but the installed referenced StS2 assembly used by the project does not, so this is an installed-API/source-drift issue outside the Fission/Banner-owned files.

Remaining:

- Fission/Banner compile and source-guard verification remains pending until the non-owned hook mismatch is resolved.
- Live map hover placement, Bounty reward settlement, save/load, reroll/pickup persistence, and multiplayer behavior remain manual verification items.

## 2026-05-07 - Firemark/Forge Token v2.0 Patch

Scope:

- Owned only A12 Firemarked Elite and Forge Token code, localization, tests, and docs.
- Left Root, Fission, Banner, Boss, package, and Ancient work intact except for shared A12 guard snippets.

Changes:

- A12 Firemarked Elite now targets 2 candidates in Act 1 and 3 in later acts, with Act 1 first-rest-site gating, same-floor/direct-adjacency exclusion, route spread where possible, and optional-route safety.
- Firemarked elite combat now chooses one Firemark Host and applies Might, Giant, Forge Armor, or Constant Heal to that host only.
- Firemarked elite card rewards gain one extra card option.
- Forge Token remains max 1; duplicate awards convert to gold, extra visible token relic copies are removed, and Rest/Smith pay out. The special rest-site action payout from this pass was later removed by the rest-site hardening pass.
- English and Simplified Chinese A12/Forge Token player-facing text was refreshed for the v2.0 behavior.

Evidence:

- Local Core source inspected for `RestSiteSynchronizer.ChooseOption`, `RestSiteOption` option ids, Heal/Smith/Mend rest behavior, `CardFactory.CreateForReward`, `CardCreationOptions`, `CardCreationResult`, `CreatureCmd`, and combat power hooks.

Verification:

- `dotnet test tests/EZMicroBalance.Tests/EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~MapAndCombatSlicesStayWithinDocumentedA12AndA19Tuning|FullyQualifiedName~FiremarkTokenAndFissionPlayerFacingSurfacesAreGuarded"`: passed, 2 tests total.
- `dotnet build`: blocked before full source compile by a non-owned Root Bud API drift error: `RootBudCombatHook.AfterSideTurnStart(CombatSide, CombatState)` has no referenced-assembly override target.
- Full no-build guard test run remains red from non-owned Root/A11/release guard drift.

Remaining:

- Full compile/publish and live map/combat/reward/rest-site verification remain pending until the non-owned Root/A11 guard drift is resolved.

## 2026-05-07 - A11 Map Shape And A17 Deep Branch Pass

Scope:

- Owned only A11 map geometry and A17 Deep Branch insertion/metadata, plus related localization, docs, and source guards.
- Left concurrent Banner combat, Rootblight, boss, package, Ancient, and release-art work intact.

Core/API evidence:

- Local Core `ActMap`, `StandardActMap`, `SavedActMap`, `SerializableActMap`, `SerializableMapPoint`, `MapPoint`, `RunManager.GenerateMap`, and `NMapScreen` were reviewed before patching.
- `StandardActMap` still owns private vanilla generation, so the implementation does not patch its internals.
- `SerializableActMap` / `SavedActMap` can represent arbitrary dimensions and child edges, and map hooks can return the replacement map.
- `NMapScreen` renders by exposed map dimensions, point coordinates, and child edges, so the source shape supports saved-map replacement, pending live visual verification.

Changes:

- A11 now expands vanilla 7-column maps to 8 columns and adds one late route row in Acts 2/3 before the boss rest row.
- A17 now inserts one optional 3-4 node Deep Branch in Acts 2/3 for single-player maps when a safe plan exists.
- Deep Branch insertion preserves the original safe route from branch parent to reconnect, adds risk rooms before the enhanced reward node, reconnects to the main route, and skips multiplayer until route voting is proven.
- A11 and A17 now use their independent feature gates: `EZMB_ASCENSION_ENABLE_MAP_GEOMETRY=0` and `EZMB_ASCENSION_ENABLE_DEEP_BRANCHES=0` can disable them separately.

Verification:

- Source guards were updated for the new A11 width/Act 2-3 row constants, A17 branch length/reconnect/safe-route evidence, and the non-deferred manual checklist.
- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors after the A11/A17 map patch.
- `dotnet publish`: passed after the ascension localization resource update. Godot still emitted the known non-fatal `sts2` assembly scan warning during PCK export.
- `dotnet test EZMicroBalance.sln --no-build --filter FullyQualifiedName~A11AndA17MapGeometryStayGatedOptionalAndRouteSafe`: passed, 1 test.
- `dotnet test EZMicroBalance.sln --no-build`: failed outside this slice on concurrent/stale Rootblight, Fission/Banner/Boss Seal, zhs-root-localization, source-manifest, and package-hash guards. The publish refreshed installed artifacts, so package staging/versioned zip/hash docs also need a later release-engineering refresh before full guard parity can pass.
- Live map UI, route traversal, save/load restoration, and multiplayer behavior remain pending before any release-readiness claim.

## 2026-05-07 - BossSeal/A20 v2.0 Safety Pass

Scope:

- Owned only Boss Seal definition/status data, minimal A19/A20 metadata/runtime handling, and Boss/A20 docs.
- Left A11-A18, Ancient, packaging, legacy manifest, A21-A30, and custom-character work untouched.

Core/API evidence:

- Local Core `RunManager.GenerateRooms()`, `StandardActMap`, `ActModel`, `RoomSet`, and `CombatManager.EndCombatInternal()` were reviewed before patching.
- `RunManager.GenerateRooms()` sets a second boss only during room generation when the vanilla Ascension manager reports Double Boss.
- `StandardActMap.CreateFor(...)` passes `runState.Act.HasSecondBoss`; the map constructor only creates `SecondBossMapPoint` when that value is already true.
- `CombatManager.EndCombatInternal()` owns combat-end hooks, room pre-finish/save/progress, and final-boss victory timing, so no safe courtyard/intermission insertion point is proven.

Changes:

- Added `EZMicroBalanceCode/Ascension/BossSealDefinition.cs` with boss-specific v2.0 Royal Seal definitions for all current bosses.
- Replaced generic A19 Armor/Rage/Barrier/Chaos metadata assignment with boss-specific `BossSealCatalog` lookup.
- Changed A19/A20 combat handling to log blocked Royal Seal / Brand definitions instead of applying unproven mechanics.
- Kept A20 from creating a second boss, revealing boss order, creating a courtyard, or adding intermission rewards/heal.

Blocked:

- A20 double-boss creation, early reveal, fixed courtyard, and intermission remain blocked pending boss-flow proof.
- Boss-specific Royal Seal mechanics remain blocked until each trigger has runtime evidence: first stun, follower/minion death, Slippery removal, wake-up source, Beckon settlement, Steam Eruption/explosion timing, claw death/back-attack identity, curse-choice menus, Insatiable self-enhancement, Doormaker reveal, Bound-card tagging, and Test Subject phase changes.
- Superseded note: later 2026-05-07 entries replace these blocked-marker-only statements with source-guarded A19/A20 Royal Seal and Brand hooks, while live boss verification and custom A20 courtyard/intermission proof remain pending.

Verification:

- `dotnet build EZMicroBalance.sln`: succeeded.
- `dotnet test EZMicroBalance.sln --no-build`: failed 14/75. BossSeal-related failures are expected guard drift from replacing `BossSealKind` Armor/Rage/Barrier/Chaos with `BossSealDefinition` blocked markers. The remaining failures are outside this BossSeal/A20 write set: Root/Blight Sprout guard text drift, Firemark ZHS title expectation drift, release artifact hash drift, ZHS Roman numeral guard drift, and existing release coverage guard text drift.
## 2026-05-07 - A19/A20 Source-Guarded Runtime Hook Pass

- Audited the current A11-A20 v2.0 state, rebuilt successfully at baseline, and confirmed the pre-pass guard suite previously passed 75/75.
- Replaced A19/A20 BossSeal blocked markers with source-guarded boss-specific hook implementations for Holy Daze, Martyr Oath, Ink Return, Startled Shell, Soul Tide, Boiling Critical, Misaligned Shell, Marginal Note, Struggle Bait, Door Wedge, Chosen Decree, and Residual Sample.
- Added the Marginal Note status card, Royal Decree enchantment, and Royal Seal powers using supported damage/death/card/turn hooks, command APIs, and state scans instead of unsupported power-amount overrides.
- Added Boss 2 Brand parameters where implemented and a guarded Boss 1 post-combat recovery of 25% missing HP when vanilla/proven second-boss map flow exists.
- Added an A20 `RunManager.GenerateRooms()` postfix that reuses the vanilla double-boss timing to set a final-act second Boss when A20 is enabled and vanilla did not already set one; this lets `StandardActMap` and `NBossMapPoint` provide the Boss 1/Boss 2 map reveal through existing game UI.
- Added a Boss 1 terminal reward hook that offers one Boss card reward before Boss 2 when the A20 second-boss map path is active.
- Added `NBossMapPoint.OnFocus` hover text for A19 Royal Seal and A20 King Brand map nodes, with English and Simplified Chinese localization.
- Left a custom fixed courtyard map node and custom intermission UI/reward flow deferred because no safe runtime insertion point has been proven.
- Fixed hardening gaps: selector backing-field null guard, current-map replacement assignment during metadata restoration, Deep Branch marker gating when Firemark/Banner systems are disabled, and Deep Branch enhanced treasure reward settlement.
- Added distinct original Banner Room and Boss Seal PNG indicators plus export/package coverage.
- Updated source/resource/localization/package guards and player-facing docs so source-guarded and pending-live-verification states are not described as finished release readiness.
- Added Boss-map Royal Seal / King Brand hover localization and source guards, then rebuilt installed artifacts and the private-beta package.
- Validation after the hover pass: `dotnet build EZMicroBalance.sln` passed with 0 warnings and 0 errors; `dotnet test EZMicroBalance.sln --no-build` passed 75/75; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `dotnet publish EZMicroBalance.sln` exited 0 with the known non-fatal Godot `sts2` scan exception; `git diff --check` returned 0 with only existing CRLF warnings for unrelated files.
- Current installed/staging/versioned/extracted hashes: DLL `40835868CD7EA62384E4F616E1AB1C2136B23FF1448F4021EA4290EEDEB4B9EC`, JSON `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`, PCK `88EA54A24619C9C8F356C5FD4530A2C43085F5617F91CF36D1701485A5F02973`; package zip `B649002273E8D64129F3B11DA2E851618F2DCCCBAAE4311B1A6C9341F1225106`.

## 2026-05-07 - A20 Reward-Screen Intermission Wording

- Rechecked local Core evidence for the A20 Boss 1 to Boss 2 transition. `NRewardsScreen.OnProceedButtonPressed()` has a proven vanilla branch from final-act Boss 1 terminal rewards to `RunManager.ProceedFromTerminalRewardsScreen()` when `SecondBossMapPoint` exists, while a custom map node or bespoke full-screen intermission remained unproven at this point.
- Added a narrow `NRewardsScreen._Ready` / `UpdateScreenState` postfix pair for only the A20 final-act Boss 1 terminal reward screen. It changes the header to "Second Boss Ahead" and the proceed button to "Face the Second Boss" after rewards are cleared.
- Added English and Simplified Chinese localization for the A20 intermission wording and source/localization guards.
- Superseded by the later fixed default-layout courtyard event: a custom map node and bespoke full-screen intermission remain deferred because they would require replacing or extending vanilla boss transition flow without proven save/load and reward-screen safety.
- Rebuilt installed artifacts and package after the reward-screen wording pass. Current installed/staging/versioned/extracted hashes after the later A11/Forge Token hardening refresh: DLL `E97A0FD91C4F2A5A83F5B7410343E9DD95C845BE574400EBAB1CF7C8CD19A7B8`, JSON `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`, PCK `8F9A3FE1F1A1184DC96B0784793350F6027AA7BB5D9D3363B91C31EEB2F1C5A4`; package zip `76D1005FDFBDB7AAC71200D9B45D7902070364E78BC9C198647B26E17132365B`.

## 2026-05-07 - Current Package Controlled Smoke Refresh

- Ran a bounded `--force-steam off` smoke for the current installed/package artifacts.
- The first smoke attempt proved EZ Micro Balance initialized and reached main menu, but the local manifest scan missed two malformed/encoding-sensitive local JSON manifests, so two unrelated mods also loaded. Settings were still restored byte-for-byte.
- Reran with regex-based manifest id discovery. Temporary settings enabled only `BaseLib` and `EZMicroBalance`, explicitly disabled 17 other local mods, and were restored byte-for-byte afterward.
- Passing smoke evidence from `godot.log`: `Loaded 2 mods (19 total)`, `Finished mod initialization for 'BaseLib' (BaseLib).`, `Finished mod initialization for 'EZ Micro Balance' (EZMicroBalance).`, `[BaseLib] Found 9 SavedSpireFields.`, `[Startup] Time to main menu: 12,820ms`, and 0 EZ Micro Balance error/exception lines.

## 2026-05-07 - A11 Visible Route And Forge Token Rest-Site Hardening

- Audited A11 map width against local Core `SavedActMap`/`NMapScreen` behavior and confirmed the prior width change could be spacing-only because no point was created in the inserted column.
- Updated A11 saved-map shaping to add a reachable optional Monster node in the inserted column while preserving the original parent-to-reconnect route; current row tuning is Act 1 +1, Act 2 +1, and Act 3 +2 late route rows before the boss rest row.
- Removed Forge Token's private `RestSiteSynchronizer.ChooseOption` wrapper for special rest-site actions after API review; Forge Token now spends only through the official Heal and Smith rest-site hooks until a safe special-action path is proven.
- Updated Forge Token hover text, source guards, and docs so special rest-site payout is not claimed as an implemented feature.
- Rebuilt installed/staging/versioned/zip artifacts after the hardening pass. Current hashes: DLL `E97A0FD91C4F2A5A83F5B7410343E9DD95C845BE574400EBAB1CF7C8CD19A7B8`, JSON `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`, PCK `8F9A3FE1F1A1184DC96B0784793350F6027AA7BB5D9D3363B91C31EEB2F1C5A4`, package zip `76D1005FDFBDB7AAC71200D9B45D7902070364E78BC9C198647B26E17132365B`.
- Reran controlled `--force-steam off` smoke against the post-hardening installed artifacts. Temporary default-profile settings enabled only BaseLib and EZ Micro Balance, explicitly disabled 17 other local mods, loaded exactly 2 mods, registered 9 SavedSpireFields, reached main menu in `12,820ms`, found 0 EZ Micro Balance error/exception lines, and restored both settings files byte-for-byte.

## 2026-05-07 - A20 Brand Parameter And Hover Hardening

- Removed the remaining `Brand parameters are not designed for A20 yet` placeholders from `BossSealDefinition`.
- Added source-guarded A20 Brand parameter branches for Martyr Oath, Ink Return, Startled Shell, Boiling Critical, Misaligned Shell, Marginal Note, and Struggle Bait; the previously implemented Holy Daze, Soul Tide, Door Wedge, Chosen Decree, and Residual Sample Brand branches remain guarded.
- Implemented Struggle Bait Brand tracking for generated Frantic Escape cards: played cards are removed from tracking, and each generated copy still unplayed after 2 player turns gives The Insatiable 5 Block once.
- Added English and Simplified Chinese per-boss Royal Seal / King Brand localization keys and wired Boss map hover text to include the matching per-boss summary.
- Hardened the A20 reward-screen wording reflection path so missing/renamed private fields fail closed with one warning instead of throwing from every reward screen.
- Validation after this pass: `dotnet build EZMicroBalance.sln` passed with 0 warnings and 0 errors; `dotnet test EZMicroBalance.sln --no-build` passed 75/75.
- Published and refreshed installed/staging/versioned/zip artifacts after the Brand/localization changes. Current hashes: DLL `F6A571C5C1FD548EA6B8ED636A1CA76011A788F7CFFCBD5FB84338286023EBCE`, JSON `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`, PCK `90905A56F1FA05F02B4D27D223A21E092DE498D4F857DBCE7D6FBCAA5734C62F`, package zip `86BEE5D591440F6D175FD5A0CABF1C96A9FE5841857C83B2088CF6FAE2676694`, package README `6CBEA8D4EC79C58BBEF082975FFCB5D6A122EC83676B76A93428EECBBF7C9E5E`.
- Reran the controlled `--force-steam off` smoke after the Brand package refresh. Temporary default-profile settings enabled only BaseLib and EZ Micro Balance, explicitly disabled 17 other local mods, loaded exactly 2 mods, registered 9 SavedSpireFields, reached main menu in `12,835ms`, found 0 EZ Micro Balance error/exception lines, and restored both settings files byte-for-byte.

## 2026-05-07 - A20 Fixed Courtyard Event

- Rechecked local Core evidence for `NRewardsScreen.OnProceedButtonPressed()`, `RunManager.ProceedFromTerminalRewardsScreen()`, `EnterRoomWithoutExitingCurrentRoom(AbstractRoom,bool)`, `EventRoom`, `EventModel`, and `NEventRoom.Proceed()` before changing the Boss 1 to Boss 2 transition.
- Added `A20Courtyard`, a non-random default-layout event room obtained with `ModelDb.Event<A20Courtyard>()`, and inserted it from the A20 Boss 1 terminal reward proceed path before vanilla map navigation to Boss 2.
- Wired the courtyard event to show the second Boss name, localized King Brand name, localized Brand summary, and the existing original Boss Seal indicator art without copying game assets.
- Decoupled Boss 2 Brand metadata from the A19 Boss Seal feature flag and unified A20 double-boss creation, Brand combat hooks, Boss 1 reward, Boss 1 recovery, reward-screen wording, and courtyard entry behind a single-player A20 gate.
- Added an immediate `SaveRun(eventRoom, saveProgress: false)` after courtyard entry so save/load can restore the event through the native pre-finished-room path.
- Kept the implementation narrow: it does not add a custom map node, rewrite Boss order, rewrite reward flow, or implement a bespoke full-screen intermission.
- Added English and Simplified Chinese `events.json` localization, export coverage, source guards, and manual-test checklist rows for the courtyard event.
- Final validation for this pass: `dotnet build EZMicroBalance.sln` passed with 0 warnings and 0 errors; `dotnet test EZMicroBalance.sln --no-build` passed 75/75; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `dotnet publish EZMicroBalance.sln` exited 0; post-publish `dotnet test EZMicroBalance.sln --no-build` passed 75/75; `git diff --check` exited 0 with the existing CRLF warnings for `EzDailyContent.json` and `docs/dev-environment.md`.
- Refreshed package hashes after the A20 fixed-courtyard pass: DLL `66084DA4B38E46F36EBA90BFB999CBA4938AB8B4AC0C01D5B2A87DF7655A3530`, JSON `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`, PCK `2F831F169A7ED099D89757DBE7768BF34174894E2DDC36858ABE9D1AFB7E392A`, package zip `98163DD931EA69908A75093DCD613A6668EA4C61B9DFBD39EDEBE677306CD641`.
- Reran controlled `--force-steam off` smoke against the current installed/package artifacts. Temporary default-profile settings enabled only BaseLib and EZ Micro Balance, loaded exactly 2 mods, registered 9 SavedSpireFields, reached main menu in `4,076ms`, found 0 EZ Micro Balance error/exception lines, and restored both settings files to their original contents.
- Live Boss 1 reward to courtyard to Boss 2 gameplay, save/load in the courtyard, and Boss 2 victory/defeat flow remain pending manual tests.

## 2026-05-07 - Current Issue Implementation Spec, A20 Multiplayer Warning, and Test-Gate Pass

- Added `docs/features/ascension-11-20/current-issue-implementation-spec.md` before implementation, covering stale A11 wording, A20 multiplayer warning, release artifact test gating, stale current-package smoke, and pending live co-op matrix issues.
- Rechecked local Core source for `StartRunLobby.UpdateMaxMultiplayerAscension`, `SyncAscensionChange`, `BeginRunForAllPlayers`, `BeginRunLocally`, `UpdatePreferredAscension`, `NAscensionPanel`, `LobbyPlayer.maxMultiplayerAscensionUnlocked`, `RunState.Players`, `SerializableRun.Ascension`, and `ProgressState.ClampAscension`.
- Added a log-only A20 host-multiplayer warning path in `AscensionSelectionPatches`: host A20 selection and host A20 run start now warn that multiplayer A20 selection is development testing, Dual King Brands / second-boss Brand gameplay is disabled or downgraded in co-op pending live verification, and A11-A19 inherited systems may still apply if their gates are enabled.
- Kept A20 gameplay conservative: `AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(...)` still requires a one-player run.
- Updated A11 player-facing localization and docs to say width +1, Act 1 +1 route row, Act 2 +1 route row, Act 3 +2 route rows, and no A11 marker/icon/hover tooltip. A12 Firemark, A16 Banner, A17 Deep Branch, A19 Royal Seal, and A20 Brand indicators remain separate.
- Added `ReleaseArtifactFactAttribute`; ignored publish/package/installed/runtime-smoke tests now skip unless `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` is set. Normal source/localization/docs tests should no longer require ignored `publish/` artifacts.
- Expanded the live co-op manual matrix with gate off/on, multiplayer selection disable flag, client join/clamp, A11/A12/A16 map indicators, A14/A15/A18 ownership, A20 warning, and desync/checksum log checks.
- Current-package runtime smoke, normal Steam-client Mod Settings verification, live feature verification, save/load, and live co-op verification remain pending until run in this or a later pass.

## 2026-05-08 - Current Package Smoke Refresh

- Ran `dotnet publish EZMicroBalance.sln` after the A20 warning/localization/test-gate pass and rebuilt `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` from the installed DLL/JSON/PCK artifacts.
- Historical hashes from this package refresh were superseded by the 2026-05-08 default-on multiplayer test candidate refresh below.
- Historical bounded `--force-steam off` smoke from this package refresh registered 12 SavedSpireFields and was superseded by the default-on gate smoke below.
- Historical validation from this package refresh passed; current validation status is recorded in the default-on gate entry below.
- Normal Steam-client Mod Settings verification, live feature verification, save/load, and live co-op verification remain pending.

## 2026-05-08 - A20 Host-Only Multiplayer Warning Follow-Up

- Relaxed `AscensionSelectionPatches.ShouldWarnA20MultiplayerDowngrade(...)` so host multiplayer A20 selection logs the downgrade warning even before a client joins the lobby. The log still records the current player count for diagnosis.
- Updated source guards to reject reintroducing a `lobby.Players.Count > 1` prerequisite on the warning path.
- Updated the manual co-op checklist to require host-only A20 selection warning, then a second warning when starting A20 after a client joins without changing Ascension.
- Refreshed issue/handoff/audit status to remove stale untracked-file and stale 9-SavedSpireField wording.

## 2026-05-08 - Default-On Multiplayer Test Candidate Gate Pass

- Changed `AscensionFeatureGate.IsPublicSelectionEnabled` so A11-A20 selection is now default-on in this private-beta multiplayer test candidate.
- Kept `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` as a legacy-compatible variable, but it is no longer required.
- Kept `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` as the emergency gate-off comparison switch that restores vanilla A1-A10 selection.
- Kept `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` scoped to host-multiplayer selector expansion only; single-player A11-A20 remains available when only that variable is set.
- Did not alter `EZMB_ASCENSION_DEBUG_LEVEL`, A11-A20 preferred-progress write skips, or the A20 Dual King Brands single-player gameplay gate.
- Added `docs/features/ascension-11-20/multiplayer-test-runbook.md` with recommended two-PC setup, env var commands, exact multiplayer matrix, save/load rows, log checks, and result template.
- Updated current-facing docs/tests to say default-on for multiplayer testing, while preserving the warning that A20 multiplayer selection is not full A20 co-op support and that controlled smoke is not normal Steam-client/live co-op verification.
- Ran `dotnet publish EZMicroBalance.sln` and rebuilt package staging, versioned package, and `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` from the installed artifacts.
- Current hashes after package refresh: DLL `9F68518A8083C128DB74E6BAC4C2C001B9BB2B8AA4AD89642883659A982C229F`, JSON `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`, PCK `A08D7D97D041316CCDD5D1F000BE82F545DBA845429900524D924B7A6EAD9F52`, package zip `F670B27869357DD6F7EE5549948CC9D09AD8711840B655E08896F22B41D803AF`, package README `6BD2F48F689390C7F6C53AB78A19CB6E26CEDFC22ED0CF30402C269E8699CDC1`.
- Ran bounded `--force-steam off` smoke against the refreshed installed/package artifacts. Temporary default-profile settings enabled only BaseLib and EZ Micro Balance, explicitly disabled other discovered local mods, loaded exactly 2 mods, registered 12 SavedSpireFields, logged the default-on Ascension initializer wording with 0 old `Default-off gate` lines, reached main menu in `13,201ms`, found 0 EZ Micro Balance error/exception lines, and restored both settings files byte-for-byte.
- Final validation after this pass: `dotnet build EZMicroBalance.sln` passed with 0 warnings and 0 errors; `dotnet test EZMicroBalance.sln` passed, 63 passed, 15 skipped release artifact/runtime evidence tests, 0 failed; `dotnet test EZMicroBalance.sln --no-build` passed with the same counts before and after publish; `dotnet publish EZMicroBalance.sln` passed; package refresh passed; controlled smoke passed; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed, 78 passed, 0 skipped, 0 failed; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` exited 0 with CRLF normalization warnings for touched files.

## 2026-05-08 - Multiplayer A20 Black-Screen TypeLoad Fix

- Investigated the player-reported multiplayer A20 black screen through `C:\Users\Jack\AppData\Roaming\SlayTheSpire2\logs\godot.log`.
- Direct failure evidence: host multiplayer A20 run start reached `NGame.StartNewMultiplayerRun(...)`, then `AscensionMapService.MarkBossSeals(...)` triggered `BossSealCatalog..cctor()` and threw `System.TypeLoadException: Could not load type 'MegaCrit.Sts2.Core.Models.Encounters.DoormakerBoss'`.
- Root cause: the local source snapshot includes newer Early Access boss classes, but the current installed Steam DLL does not expose every same type/member. Hard generic type references in Boss Seal startup code are unsafe across EA builds.
- Changed `BossSealCatalog` to map Boss Royal Seals by runtime-safe `ModelId` strings such as `ENCOUNTER.DOORMAKER_BOSS` instead of `ModelDb.GetId<DoormakerBoss>()`.
- Changed Door Wedge checks to use runtime `ModelId` checks for the Doormaker monster and phase powers instead of direct `Doormaker`, `HungerPower`, `ScrutinyPower`, or `GraspPower` type references.
- Adjusted adjacent compile/runtime compatibility for the current installed game API: Debt turn-end patch now uses a string target name, Pumpkin Candle active-act handling uses reflection for `ActiveAct` / `_activeAct` instead of direct property access, and Pumpkin Candle room-entry patching now falls back to `AbstractModel.AfterRoomEntered` when the subclass override is absent.
- Added source guard coverage so the Boss Seal startup path does not reintroduce hard optional Doormaker/Glory type references.
- Validation after this fix: `dotnet build EZMicroBalance.sln` passed with 0 warnings and 0 errors; `dotnet test EZMicroBalance.sln --no-build` passed, 64 passed, 15 skipped release artifact/runtime evidence tests, 0 failed; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build --filter HarmonyPatchesResolveAgainstInstalledGameApi` passed, 1/1; `dotnet publish EZMicroBalance.sln` passed and refreshed the locally installed DLL.
- Live multiplayer A20 retest remains pending: host and client still need to verify the run reaches Act 1 map with no EZ Micro Balance `TypeLoadException` in `godot.log`.
