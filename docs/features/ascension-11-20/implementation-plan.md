# Ascension 11-20 Implementation Plan

Project: Spire Plus (`EZMicroBalance` manifest id)
Manifest id: EZMicroBalance  
Status: A11-A20 single-player and host-multiplayer selector expansion and prototype slices added; selection is default-on for the private-beta multiplayer test candidate; A11 map geometry has source-boundary and optional-route graph proof; live gameplay verification pending
Last updated: 2026-05-14

## Current Development Checklist

The current forward-looking development checklist is `docs/features/ascension-11-20/development-checklist-v2.md`.

That v2.0 checklist defines the next target design for:

- Rootblight / Blight Sprout as a v2.2 seed/root pollution system with up to four master-deck Rootblight cards.
- Firemarked Elite hosts, stronger Firemark types, and Firemark reward settlement.
- Forge Token as upgrade-tempo protection.
- Fission reward rates, UI, and eligibility.
- Banner Rooms with Vanguard, Shield Formation, and Bounty first.
- Deep Branches as optional Act 2/3 high-risk routes.
- Boss-specific Royal Seals.
- A20 Dual King Brands and the fixed courtyard.

Current code under `EZMicroBalanceCode/Ascension/` has been migrated toward this v2.0 checklist where safe APIs exist. Milestones 0-6 are build/source-guard proven but still need live runtime verification; Milestone 7 now has source-guarded boss-specific Royal Seal hooks plus Boss-map hover text pending live boss verification; Milestone 8 now reuses the vanilla double-boss map path for final-act Boss 1/Boss 2 reveal, Boss 2 Brand metadata/parameters and hover text, Boss 1 post-combat recovery, one Boss card reward after Boss 1, Boss 1 reward-screen intermission wording, and a fixed default-layout courtyard event before Boss 2. A bespoke full custom intermission screen remains deferred.

## Current Decision

Do not claim A11-A20 release readiness as a single feature block.

The vanilla game exposes a hard max Ascension of 10 through progress and lobby/UI paths. The current development build adds narrow single-player and host-multiplayer selector/start patches so A11-A20 can be tested through the original Ascension UI without globally patching progress validation.

The current implementation uses the selected run Ascension level after the selector expansion:

- A11 Wide Tower, Long Road / 宽塔长路 expands vanilla maps by 1 column, inserts a reachable optional route in the inserted column, and inserts late route rows by act (Act 1 +1, Act 2 +1, Act 3 +2) through `SerializableActMap` / `SavedActMap`. Source guards now require both an inserted-column route choice and a preserved start-to-boss route that avoids the inserted column. Ordinary A11 route nodes do not receive a dedicated marker or hover tooltip.
- A12 also enables firemarked elite marking, generic firemark combat modifiers, and Forge Token grant/heal/smith payout. Special rest-site payout is disabled until a safe runtime API is proven.
- A13 also enables Fission reward-card enchantment.
- A14 also enables Rootblight Begins.
- A15 also enables boss Blight Sprout.
- A16 also enables Banner Room marking and generic banner combat modifiers.
- A17 inserts one optional 3-4 node Deep Branch in Acts 2/3 for single-player runs when safe saved-map geometry is available; enhanced treasure nodes add a guarded Uncommon relic reward.
- A18 also enables elite Blight Sprout.
- A19 also enables Boss Seal definition lookup, source-guarded boss-specific Royal Seal hooks, and a fourth boss card reward option; all boss-specific mechanics still require live verification.
- A20 also enables the single-player vanilla double-boss map path, Boss 2 Brand metadata/parameters independent of the A19 Boss Seal feature flag, Boss 1 post-combat recovery, one Boss card reward before Boss 2, narrow Boss 1 reward-screen intermission wording, and a fixed default-layout courtyard event before Boss 2 with an immediate pre-finished-room save. It does not create a bespoke full custom intermission screen.
- A11-A20 selection is now default-on in this private-beta multiplayer test candidate for standard single-player and host-multiplayer lobbies.
- `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` restores vanilla A1-A10 selection for comparison testing.
- `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` disables only host-multiplayer A11-A20 selection while leaving single-player A11-A20 available.
- `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.
- `EZMB_ASCENSION_DEBUG_LEVEL=11` through `20` can still force slice gates for internal checks.
- `EZMB_ASCENSION_DIAGNOSTICS=1` enables read-only internal run/combat diagnostics without enabling gameplay systems. It must not mutate restored Blight Sprout card state or raise Rootblight by itself.
- Host multiplayer A20 selection/start now logs a development-testing warning because A20 Dual King Brands / second-boss Brand gameplay remains single-player gated. A20 multiplayer selection is not full A20 co-op support.
- Normal Steam-client Mod Settings has separate RC1 evidence; controlled smoke passed is not the same as live co-op verification.

The diagnostics gate is intentionally non-mutating. It logs hook reachability and Rootblight state only.

## Phase 0: Research and Architecture

Status: complete for initial spec.

Completed:

- Read `AGENTS.md`.
- Read `docs/features/ascension-11-20/source-design.md`.
- Inspected `EZMicroBalance.sln`, `EZMicroBalance.csproj`, `EZMicroBalance.json`, `EZMicroBalanceCode/MainFile.cs`, current Ancient module layout, and architecture docs.
- Ran `git status --short --branch`; the worktree already contains broad existing project changes.
- Checked `SlayTheSpire2.exe`; it was not running.
- Ran `dotnet build EZMicroBalance.sln`; build succeeded with 0 warnings and 0 errors.
- Inspected StS2/BaseLib signatures for Ascension, cards, commands, hooks, rest sites, map, rewards, enchantments, boss flow, and multiplayer state.

Outputs:

- `api-research.md`
- `implementation-plan.md`
- `manual-test-checklist.md`
- `work-log.md`

## Phase 1: Ascension Gate and Max-Level Spike

Status: implemented for single-player development testing; live verification pending.

Goal:

- Support A11-A20 selection, display, and single-player run start through the original Ascension UI.

Evidence:

- `AscensionManager.maxAscensionAllowed = 10`.
- Progress unlock methods guard with `< 10`.
- Lobby and `NAscensionPanel` use a max Ascension value for selection.
- `RunState.AscensionLevel` stores an `int`, so run state can represent values beyond the enum names.
- `AscensionSelectionPatches` expands `StartRunLobby` single-player max selection to 20 and temporarily raises local `CharacterStats.MaxAscension` only while an A11-A20 run starts.

Plan:

1. Keep English and Simplified Chinese `ascension.json` keys for A11-A20 packaged.
2. Guard the selector patch so it stays on `StartRunLobby` single-player paths only.
3. Avoid global `CharacterStats` getter, `ProgressState`, `ProgressSaveManager`, `NAscensionPanel`, and `AscensionManager.maxAscensionAllowed` patches.
4. Keep host multiplayer A20 warning logs on selection and run start until a safe UI hint or live co-op support is proven.
5. Live-test A11 and A20 run start and inspect `godot.log`.

Stop conditions:

- Any evidence of progress save corruption risk.
- Any multiplayer lobby desync risk.
- Any need to mutate manifest id or existing Ancient feature behavior.

## Phase 1A: Read-Only Internal Diagnostics

Status: implemented; live log verification pending.

Scope:

- `EZMB_ASCENSION_DIAGNOSTICS=1`.
- Register the same proven run/combat hook subscriber path even when gameplay debug levels are off.
- Log run Ascension, act index, debug/public gate state, Rootblight level/card counts, current combat room type, round, boss/elite Blight Sprout gate state, and combat Blight Sprout counts.
- No deck, map, reward, rest-site, boss-flow, progress, selector, or manifest mutation.

Purpose:

- Prove run/combat hook reachability and live state shape before adding any next gameplay slice.
- Support manual Rootblight/Blight Sprout verification and A11-A20 selector debugging.

Stop conditions:

- Any diagnostics log exception or hook-registration issue during startup or run entry.
- Any evidence that registering diagnostic-only hook subscribers changes gameplay state.

## Phase 1B: A11 Wider/Longer Map

Status: source-patched with optional-route graph proof; live route/UI/save-load verification pending.

Exact deferral:

- A11 does not patch `StandardActMap` internals. Exact API evidence: `StandardActMap` generation uses private width/path internals and `ActModel.GetNumberOfRooms(...)` controls length before hook code sees the map. `ActModel.CreateMap(RunState,bool)` is now also patched as an earlier source boundary, and `RunManager.GenerateMap()` still accepts replacement maps from `ModifyGeneratedMap(...)` and `ModifyGeneratedMapLate(...)` before `NMapScreen.SetMap(...)` reads dimensions and child edges. `SerializableActMap` / `SavedActMap` can represent arbitrary saved dimensions. The implementation serializes the generated map, shifts columns at and after the inserted column to open an 8th column, adds a reachable optional Monster node in the inserted column while keeping the original safe parent-to-reconnect path, shifts the final rest/boss rows down by Act 1 +1, Act 2 +1, and Act 3 +2, bridges every affected route into the shifted rest row, leaves ordinary A11 route nodes unmarked, and returns a `SavedActMap`. Source-level graph tests now prove valid optional inserted-column route evidence, reject inserted-column chokepoints, and keep already-target-sized maps idempotent only when both inserted-column route and original-route-preserved evidence are true. Next proof step: live-test visible width/rows, natural route-click reachability, save/load, and map UI rendering before calling the slice release-ready.

## Phase 2: A14 Rootblight Closed-Loop MVP

Status: Rootblight v2.2 source migration implemented; build/source-guard validation and runtime verification pending.

Scope:

- Add Rootblight I at Act 1 start when A14+ is selected or forced by debug level.
- Treat Rootblight as real master-deck pollution with a four-card cap. A14 starts with Rootblight I; ignored Rootblight I/II worsens after combat; ignored Rootblight III stays at III and, the first time per card, adds one Rootblight I; played Rootblight cards remove their master-deck version and queue the downgraded replacement after combat.
- Use Rootblight I/II/III display cards, with costs 2/3/4.
- Playing Rootblight exhausts the combat copy, removes its master-deck card, and queues the downgrade card after combat when applicable.
- Combat end upgrades only Rootblight cards that were present at combat start; newly added Rootblight cards do not grow until the next combat. Ignored Rootblight III has no IV path.
- Rootblight start seeding uses `SavedSpireField<Player,bool>`, `SavedSpireField<Player,int>`, and a Rootblight deck scan to avoid re-adding after clearance and to migrate prototype Root/Deep Root saves.
- Rest heal removes exactly one highest-stage Rootblight, oldest first when tied; smith does not remove Rootblight.
- Shop/card-removal APIs remove the selected Rootblight through `BeforeCardRemoved` without clearing other Rootblight cards.
- Non-play exhaust does not lower Rootblight level.
- No map changes.
- No reward-generation changes.
- No boss seals.
- No A20 intermission.
Gate:

- A14+ through the original single-player Ascension UI.
- Internal force gate: `EZMB_ASCENSION_DEBUG_LEVEL=14`.

Required exact APIs:

| Need | API/path |
| --- | --- |
| Current Ascension | `RunState.AscensionLevel` |
| Run hook | `ModHelper.SubscribeForRunStateHooks(...)` plus `RootRunHook.AfterActEntered()`; hook model also has a parameterless constructor for StS2 model database startup |
| Add master-deck card | `RunState.CreateCard<Root>(player)` / `CreateCard<DeepRoot>(player)` / `CreateCard<RootblightIII>(player)` then `CardPileCmd.Add(card, PileType.Deck, ...)` |
| Level state | `SavedSpireField<Player,int>` diagnostic Rootblight level, `SavedSpireField<Player,bool>` one-time starter marker, and per-card saved fields for combat-start presence, one-time split state, and Blight Sprout round |
| Play downgrade | In combat card play, remove the matching master-deck Rootblight card and queue the downgraded replacement; do not listen to non-play exhaust |
| Combat-end sync | `RootBudCombatHook.AfterCombatEnd(...)` calls `RootDeckService.ResolveCombatEndRootblight(...)` before adding Rootblight I from unplayed Blight Sprout cards, capped at four Rootblight cards |
| Removal/clear | `AfterRestSiteHeal` clears on real rest; `BeforeCardRemoved` clears for normal deck-removal APIs; sync-owned removals suppress the clear hook |
| Card play behavior | Custom `CardModel.OnPlay(...)`, `CardKeyword.Exhaust`, and `ExhaustOnNextPlay` |
| Card removability | Do not add `CardKeyword.Eternal`; verify `CardModel.IsRemovable` stays true |
| Localization | `EZMicroBalance/localization/eng/cards.json` and `EZMicroBalance/localization/zhs/cards.json` |
| Packaging | `EZMicroBalance.csproj` packages `EZMicroBalance/**` |

Implementation sketch:

1. Add isolated Ascension module under `EZMicroBalanceCode/Ascension/`.
2. Add Rootblight display cards under the `Cards` subfolder.
3. Add a run hook listener that inserts Rootblight I for each active player on a new run when the gate is active.
4. Add starter protection through saved Rootblight state and a deterministic deck scan by Rootblight family id.
5. Implement Rootblight play behavior to exhaust the combat copy, remove the matching master-deck card, and queue a downgrade.
6. Resolve master-deck Rootblight upgrades/downgrades at combat end.
7. Clear Rootblight on real rest and on deck-removal APIs.
8. Keep all logging/dev notices behind the same debug gate or existing mod logging style.
9. Add English and Simplified Chinese card localization.
10. Run `dotnet build EZMicroBalance.sln`.
11. Because localization/resources change, run `dotnet publish` after build succeeds. If the Steam mod DLL is locked by the game, publish with a temporary `ModsPath` or ask the user to close the game.

Runtime stop conditions:

- If custom card registration, pool, or portrait behavior fails in-game.
- If the run hook fires on save load and re-adds Rootblight after clearance.
- If `DeckVersion` is unavailable or unreliable in combat.
- If combat-end sync fails to update or remove the master-deck Rootblight card.

Rollback plan:

- Disable the debug/internal gate.
- Remove only the new isolated Ascension files and localization keys.
- Do not touch `EZMicroBalance.json`, Ancient modules, or legacy scaffold modules.

## Phase 3: A15 Boss Blight Sprout MVP

Status: Blight Sprout terminology and Rootblight v2.2 growth are migrated in root files; runtime verification pending.

Scope:

- Act 2 and Act 3 boss combats only for the current A15 slice.
- Add two temporary Blight Sprouts to each relevant player's discard pile for Act 2/3 boss fights.
- Before seeding, scan that player's active combat piles for an existing Blight Sprout so hook re-entry or mid-combat reload does not add a duplicate.
- Boss Blight Sprouts sprout on rounds 3 and 4 by moving to top of draw pile if they have not entered hand; elite Blight Sprout uses round 3.
- If it entered hand and was not played before combat end, add one Rootblight I after combat, capped by the 4-card Rootblight limit.
- A18 elite Blight Sprout is also implemented behind `EZMB_ASCENSION_DEBUG_LEVEL=18`, but only for Acts 2/3 elites.
- If a player dies during the combat, that combat's Blight Sprout does not raise Rootblight for that player after the game's pre-end revive path.

Required proof:

- Temporary cards created through `CombatState.CreateCard<T>(player)`.
- Insertion through `CardPileCmd.AddGeneratedCardToCombat(..., PileType.Discard, ...)`.
- Turn tracking through combat hooks.
- Enter-hand tracking through `AfterCardChangedPiles` or `AfterCardDrawn`.
- Combat-end growth through `AfterCombatEnd`.
- Parameterless hook constructor for `RootBudCombatHook`, with inactive nullable state for model database startup instances.
- Multiplayer ownership and target limits.

Current limitations:

- Blight Sprout live combat flow, mid-combat save/load, and multiplayer synchronization are unverified. Saved per-card flags and existing-pile scans are not a substitute for live runtime verification.
- No player-facing combat notice beyond card text and log messages is implemented yet.

## Phase 4: A12 Firemarked Elite and Forge Token

Status: partially implemented behind default-off gate; live verification pending.

Scope:

- About 3 visible/logged firemarked elite candidates per act when enough safe route-exclusive candidates exist.
- A normal route should contain at most one firemarked elite where route geometry allows it.
- Defeating it grants one player-owned Forge Token, max 1.
- Forge Token pays out at the next heal or smith rest-site action. Special rest-site payout is disabled until a safe runtime API is proven.

Required proof:

- Safe existing-node map marking with no route break.
- Combat room/elite identity.
- Generic combat modifiers through powers/hooks.
- Saved run-state field for Forge Token.
- Rest-site option selected event/hook.

Implemented:

- `AscensionMapService` targets 2 optional existing elites in Act 1 and 3 in later acts when another boss path remains available, using same-floor and direct-adjacency constraints plus route spread where possible.
- Firemarked elites use `FiremarkedEliteMapQuestMarker` plus a narrow normal-map UI postfix to show a dedicated firemark indicator instead of the generic quest marker.
- `AscensionMapService` keeps Act 1 firemarked elite candidates after the first rest-site row.
- `AscensionCombatModifierService` assigns one Firemark Host per firemarked elite combat and applies Might, Giant, Forge Armor, or Constant Heal with generic command APIs and visible host powers.
- Firemarked elite card rewards gain one extra card option.
- `ForgeTokenService` grants max-one saved token on firemarked elite victory; duplicate tokens convert to 15 gold, and the Firemarked Elite fourth reward option is upgraded when an upgradable candidate exists.
- `ForgeTokenRelic` mirrors the saved token as a visible one-count Event relic and provides player-facing hover/rest text.
- Heal rest spends the token to randomly upgrade an upgradable common/uncommon card or fallback-heal; smith rest spends it to heal 7 HP.

Remaining deferrals:

- Firemarked marker visibility, placement constraints, save/load restoration, node-to-combat metadata, and heal/smith payout are source-patched only until live verification can run. Special rest-site payout remains deferred because the available private `RestSiteSynchronizer.ChooseOption` wrapper was not proven safe.

## Phase 5: A13 Fission Enchantment

Status: implemented behind default-off gate; live verification pending.

Scope:

- Modify eligible reward cards only.
- Energy cost -1 and gains Exhaust.
- Strict eligibility filter.

Required proof:

- Reward modification through `TryModifyCardRewardOptions` or reward creation hooks.
- Custom enchantment serialization and tooltip/localization.
- Reroll and selected-card save/load behavior.

Implemented:

- `FissionEnchantment` is a custom enchantment with English and Simplified Chinese provider strings, a dedicated icon, and an Exhaust hover tip.
- `AscensionRewardService` rolls source-specific Fission chance through `TryModifyCardRewardOptionsLate`: normal combat 10%, Banner Room 15%, Firemarked Elite 20%, and Boss 5%.
- Each reward screen can receive at most one Fission card; the service clones one eligible reward card and applies `CardCmd.Enchant<FissionEnchantment>`.
- Eligibility excludes Powers, X-cost, star-cost, cards whose canonical or current unmodified energy cost is 0, cards with Exhaust, cards already set to exhaust on next play, quest/special/story rarities, unmodifiable cards, and incompatible existing enchantments.

Remaining deferrals:

- Reward reroll behavior, selected-card save/load, card text display, and multiplayer reward synchronization remain unverified. Next proof step: live-test `EZMB_ASCENSION_DEBUG_LEVEL=13` across reward generation, reroll, pick, save/load, and combat play.

## Phase 6: A16 Banner Rooms

Status: partially implemented behind default-off gate; live verification pending.

Scope:

- Visible enhanced normal combats.
- Vanguard, Shield Formation, and Bounty first-batch rules.
- Public hover/rule surfaces.
- Bounty bonus reward without changing monster action tables.

Required proof:

- Map marking for normal combats.
- Combat-room metadata or saved field linking node to banner type.
- Generic combat modifiers, Bounty reward settlement, and multiplayer target caps.

Implemented:

- `AscensionMapService` marks one optional existing monster node in Act 1 and two in later acts when another boss path remains available.
- `BannerRoomMapQuestMarker` marks banner nodes, and `AscensionMapUiPatches` shows Vanguard, Shield Formation, or Bounty rule text on map hover.
- `AscensionCombatModifierService` applies Vanguard temporary Strength, Shield Formation bannerbearer Block once per round, and Bounty target/penalty behavior through command APIs.
- Bounty success adds a 15 Gold room-end reward through `CombatRoom.AddExtraReward`; no monster action table edits are used.

Remaining deferrals:

- Banner marker visibility, hover placement, save/load restoration, node-to-combat metadata persistence, Bounty reward settlement, and multiplayer target caps are unverified. Next proof step: live-test route visibility, enter marked/unmarked combat, save/load before the marked node, and inspect logs/effects/rewards.

## Phase 7: A17 Deep Branches

Status: source-patched for single-player; live route/UI/save-load verification pending.

Scope:

- Optional high-risk/high-reward branch in Acts 2/3.
- Preserve a safer parallel route.

Required proof before release claims:

- Map invariant audit.
- Safe edge/node insertion or map replacement.
- Save/load of modified map.
- Multiplayer route voting behavior.

Implemented:

- A17 uses `SerializableActMap` / `SavedActMap` replacement instead of patching `StandardActMap` internals.
- Branch insertion is restricted to Acts 2/3 and single-player maps.
- The branch planner uses the A11 inserted column, creates one 3-4 node chain, adds risk rooms before the reward node, reconnects to an existing route, and verifies the original safe route from branch parent to reconnect still exists without the branch.
- Branch nodes are marked non-modifiable, and metadata restoration labels risk vs enhanced reward nodes after map hook re-entry.

Remaining:

- Live map UI placement, route traversal, save/load restoration, and metadata/marker visibility are not verified.
- Multiplayer route voting is not proven; branch insertion is skipped in multiplayer until a separate proof exists.

## Phase 8: A19 Boss Seals and A20

Status: A19 BossSeal data table plus source-guarded boss-specific runtime hooks and Boss-map hover text are implemented behind the A19 gate; A20 uses the vanilla double-boss map path for final-act Boss 1/Boss 2 reveal, Boss 2 Brand metadata/parameters and hover text, Boss 1 post-combat recovery, one Boss card reward after Boss 1, Boss 1 reward-screen intermission wording, and a fixed default-layout courtyard event before Boss 2. A bespoke full custom intermission screen remains deferred and high risk.

Scope:

- A19 boss-specific Royal Seal definitions and improved boss card reward options.
- A20 revealed double boss, Boss 2 Brand mode, and courtyard/intermission between bosses.

Implemented:

- `BossSealDefinition` and `BossSealCatalog` define all active v2.0 Boss Royal Seals by boss encounter: Holy Daze, Martyr Oath, Ink Return, Startled Shell, Soul Tide, Boiling Critical, Misaligned Shell, Marginal Note, Struggle Bait, Aeonglass Strength, Chosen Decree, and Residual Sample. Door Wedge is removed from active v0.105.0 scope because Doormaker was replaced by Aeonglass.
- A19 boss nodes receive boss-specific Royal Seal metadata from the active boss encounter instead of the older generic Armor/Rage/Barrier/Chaos placeholder set.
- A19 combat logs the armed Royal Seal and source evidence, then applies only the current source-guarded hook paths; live runtime verification is still required before release-readiness claims.
- A19 boss card rewards add a fourth option through `CardFactory.CreateForReward` with hook recursion disabled.
- A20 sets a final-act second Boss through a `RunManager.GenerateRooms()` postfix when the single-player vanilla double-boss path has not already done so; `StandardActMap` then exposes `SecondBossMapPoint`, Boss 2 receives Brand metadata plus source-guarded Brand parameter variants even if the A19 Boss Seal feature flag is disabled, Boss-map hover text, Boss 1 post-combat recovery restores 25% missing HP, Boss 1 terminal rewards include one Boss card reward, the Boss 1 reward screen uses second-Boss header/proceed wording, and `RunManager.ProceedFromTerminalRewardsScreen` opens `A20Courtyard` through `EnterRoomWithoutExitingCurrentRoom` followed by `SaveRun(eventRoom, saveProgress: false)`. A bespoke full custom intermission screen remains deferred.

Required proof:

- Boss identity/order trace in Act 3 A10.
- Safe boss-specific Royal Seal display/notice.
- Runtime evidence for each boss-specific trigger before applying mechanics.
- Safe intermission insertion point after boss 1 and before boss 2.
- Reward/heal flow that does not duplicate or skip room-end rewards.

Exact deferrals:

- A20 creates/reveals the second Boss by reusing exact vanilla timing: `RunManager.GenerateRooms()` sets `SecondBossEncounter` before `StandardActMap.CreateFor(...)`, and `NBossMapPoint` renders Boss 1/Boss 2 icons from the act encounters. EZ Micro Balance's postfix only fills `SecondBossEncounter` when A20 is enabled, the run is single-player, and vanilla did not already choose a second Boss.
- A full custom A20 intermission screen is not implemented. Exact API evidence: `NRewardsScreen.OnProceedButtonPressed()` already owns the safe Boss 1 terminal-reward branch to the second Boss when `SecondBossMapPoint` exists, while replacing that flow with bespoke UI would touch `RunManager.ProceedFromTerminalRewardsScreen()` / `CombatManager.EndCombatInternal()` and risks duplicate rewards or skipped terminal behavior. Current implementation changes Boss 1 reward-screen wording inside the proven vanilla pause and opens a default-layout `A20Courtyard` event with a native pre-finished-room save before Boss 2. Next proof step: live-test Boss 1 reward to courtyard to Boss 2 with save/load and defeat/victory checks, then prototype any bespoke full intermission in a disposable branch only if needed.

## Documentation Requirements Per Slice

For every implementation slice:

- Update `api-research.md` if new API evidence is discovered.
- Update this plan if scope or sequencing changes.
- Update `manual-test-checklist.md` with exact verification steps.
- Update `work-log.md` with commands run, results, and limitations.
- Update release docs if player-facing behavior changes.

## Build and Publish Requirements

- After code changes: run `dotnet build EZMicroBalance.sln`.
- After localization/resource/packaging changes: run `dotnet publish` after build succeeds.
- If `SlayTheSpire2.exe` is running and the Steam mod DLL may be locked, use a temporary `ModsPath` for build/test or ask the user to close the game before publishing.
