# Ancient Expansion v2.2 Overnight Source Audit

Current note, 2026-05-13: this file is preserved as the pre-implementation source audit baseline. Later source now includes default-on Lotha and a default-on single-player Vakuu fight slice; use the parent feature docs and `docs/test-ready-completion-audit.md` for the current implementation status.

Date: 2026-05-12

Reviewed source baseline: `a2183ee (HEAD -> main, origin/main, origin/HEAD) 1`. Re-run `git log -1 --oneline --decorate` before release packaging.

Release-ready: no.

## Scope And Evidence

This is a source-driven audit and planning package. It does not implement gameplay, change manifest ids, add A21-A30, add a custom character, copy official assets, or promote Morvi/Lotha/Vakuu gameplay.

Primary evidence inspected:

- Mod source: `EZMicroBalanceCode/**`, especially `Ancients/**`, `Ancients/Expansion/Urda/**`, `Ancients/Expansion/Morvi/**`, and `Ascension/**`.
- Mod resources: `EZMicroBalance/localization/**`.
- Tests: `tests/EZMicroBalance.Tests/**`.
- Current docs: `PROJECT_STATE.md`, `README.md`, `docs/README.md`, `docs/PROJECT_MAP.md`, `docs/issues.md`, `docs/issues/**`, release handoff/checklist docs, and feature docs.
- Game source: `source code/src/Core/**`.
- Secondary code-only source: `source code/source-code-code-only-ai-analysis-20260509-012059/**`; the root path `sourcecodeonlyaianalysis/**` was not present.
- Scripts: `scripts/audit-godot-log.ps1`, `scripts/check-installed-ezmb-package.ps1`, `scripts/README.md`, `Directory.Build.props.example`, and `Sts2PathDiscovery.props`.

## Primary Answers

| Question | Source-backed answer |
| --- | --- |
| What is currently implemented? | Ancient reward rebalance v4.3 is active. Urda is default-on with four source-backed blessings: Seedbed, Humus Pact, Molting, Moss Map. Morvi has three default-off prototype blessings behind `EZMB_ENABLE_MORVI_V22=1`: Misprint Press, Open-Book Exam, Debt Settlement. A11-A20 Ascension slices are source-implemented and default-on for private-beta test scope, with disable/warning gates. |
| What is only planned? | Urda Trial Branch, Shallow-Root Relic, Rooted Route, After the Rain, Root-Sight, Seed Bank; most Morvi blessings; all Lotha blessings; Vakuu Fight; Temptation, Waste Paper, Archive Pages. |
| What cannot safely be supported yet? | Lotha and Vakuu fight lack event/background asset and death/failure-path proof. Red Ink Overdraft needs a source-proven active-button injection model. Death Reprieve needs death-prevention API proof beyond ordinary `BeforeDeath`/`AfterDeath` hooks. Player-scoped `previous saved-state API<Player,string>` persistence is not proven by local game serialization source; Urda/Morvi card-backed deck mirrors mitigate this and now recover only from owned, non-removed deck cards, but still need live save/load proof. Multiplayer authority for reward alternatives, room-entry rewards, and A20 second-boss flow is not proven. |
| What may softlock/desync/fail save-load/platform? | Card reward alternatives can softlock or duplicate if reward context is lost across save/reload or if host/client alternatives diverge. Humus/Debt payoff rewards use custom reward offers after reward completion and need live reentry checks. Moss Map now filters `RunState.Players` through `Player.IsActiveForHooks`, matching vanilla hook-listener filtering, but host/client room-entry authority still needs live proof. A12/A16/A19/A20 map metadata is runtime weak-table state and must prove deterministic regeneration. Windows helper scripts are PowerShell/Windows-path oriented. |
| Multiplayer issues before release? | Two-client Steam evidence, mod-list/ModelDb hash mismatch logs, A11-A20 selection propagation, co-op save/quit, host/client reward ownership, Rootblight/Urda per-player state, A20 downgrade/second-boss behavior, and clean host/client logs. |
| Manual/live tests required? | See `manual-test-master-matrix.md`. Required tiers cover environment, single-player smoke, Urda, Ancient reward rebalance, Ascension, multiplayer, and future v2.2 non-activation. |
| Next implementation milestone? | Do not add more content. First prove save/load carriers, reward reentry behavior, and multiplayer authority, then close current RC blockers. Only after that start a narrowly gated v2.2 milestone. |

## Current Implementation Inventory

### Ancient Reward Rebalance v4.3

Active source lives in `EZMicroBalanceCode/Ancients/Patches/**` and shared helpers under `EZMicroBalanceCode/Ancients/Common/**`.

Implemented surfaces include:

- Distinguished Cape option/pickup behavior in `VakuRewardPatches.cs`.
- Velvet Choker soft-limit/cost behavior in `VakuRewardPatches.cs`.
- Jewelry Box, Brightest Flame, Quality Flame, Pael's Tooth, Forge, Prismatic Gem, Debt Ledger, and related Ancient reward patches across `VakuRewardPatches.cs`, `PrismaticGemPatches.cs`, `PaelsToothAndForgePatches.cs`, `BrightestFlameExhaustDrawPatch.cs`, `PickupRewardPatches.cs`, and `TurnOfferAndRestPatches.cs`.
- Saved fields in `AncientSavedStateFields.cs`: Prismatic Gem counter, Pael's Tooth counter, Jewelry Box card marker, Urda player state key, Urda deck mirror state key, Morvi player state key, and Morvi deck mirror state key.

Status: source-implemented and test-guarded, but live Ancient reward gameplay/save-load rows remain pending in release docs.

### Urda

Urda is active by default for private-beta testing.

Source evidence:

- Gate: `UrdaFeatureGate.cs`; disable with `EZMB_DISABLE_URDA=1`; legacy force support remains.
- Ancient model/registration: `UrdaAncient.cs` patches Act 1 `Overgrowth.GetUnlockedAncients` and `Underdocks.GetUnlockedAncients`, and obtains canonical marker models from `ModelDb`.
- Hook model: `UrdaRunHook.cs` overrides card reward option marking, card reward alternatives, reward-taken follow-up, act enter, and room enter.
- Active blessing ids: `urda_seedbed`, `urda_humus_pact`, `urda_molting`, `urda_moss_map`.
- Cards: `UrdaCards.cs` implements Seedling and Withered Husk.
- Localization: English and Simplified Chinese entries exist for Urda, the four options, Seedling, Withered Husk, and reward alternative buttons.
- Hook-state hardening: source review showed vanilla `RunState.IterateHookListeners` filters player deck/relic/potion listeners by `Player.IsActiveForHooks` while yielding mod run-state subscribers globally; Urda's act-entry and Moss Map room-entry loops now apply the same active-player filter, and deck-mirror state recovery only reads owned, non-removed deck cards.

Runtime behavior:

- Seedbed: normal Act 1 combat card reward alternative; charges 2 max HP; adds Seedling; first Seedling is upgraded; fourth accept grants +10 max HP via `SetMaxHp` after acceptance.
- Humus Pact: explicit card reward alternative, not `OnSkipped`; grants 15 gold per compost; third compost sets payoff pending; payoff removes up to two cards and offers one upgraded card reward, and pending clears only after resolver success.
- Molting: removes one starter Strike-like and one Defend-like card, adds two Withered Husks, removes deck husks at Act 2+ entry.
- Moss Map: first Act 1 room type rewards: normal combat gold, event heal, shop potion, elite upgrade, rest max HP.

Release risk: default-on but still prototype-grade until live gameplay, save/load, and co-op ownership checks pass.

### Morvi

Morvi is default-off. It requires `EZMB_ENABLE_MORVI_V22=1`.

Source evidence:

- Gate: `MorviFeatureGate.cs`.
- Ancient model/registration: `MorviAncient.cs` patches Act 2 `Hive.GetUnlockedAncients`.
- Hook model: `MorviRunHook.cs` handles combat start, card reward options, alternatives, reward-taken follow-up, and card-play replay.
- Active prototype ids: `morvi_misprint_press`, `morvi_open_book_exam`, `morvi_debt_settlement`.
- Localization: English and Simplified Chinese strings exist for Morvi and those three prototype options.
- Hook-state hardening: Morvi's combat-start loop now filters to `Player.IsActiveForHooks`, and its deck-mirror recovery uses the shared owned/non-removed card filter in `AncientPlayerState`.

Runtime behavior:

- Misprint Press: first Attack or Skill played each combat is cloned, marked Exhaust, added to play, and autoplayed once. Power cards and clones are excluded; recursion guard exists.
- Open-Book Exam: normal Act 2 combat card rewards upgrade one Attack or Skill option.
- Debt Settlement: grants 75 gold on selection; Act 2 normal combat reward alternative pays 25 gold or nonlethal HP fallback; after three payments it offers an upgraded card reward and clears pending only after resolver success.

Release risk: not default-on and not release-ready. It still depends on unproven Player saved-field persistence, reward reentry behavior, co-op ownership, and event-art/source proof.

### Lotha And Vakuu Fight

No active Lotha gameplay source directory was found. No active Vakuu fight implementation was found beyond existing Ancient reward rebalance patches involving Vakuu reward behavior.

Source blockers:

- `Glory.GetUnlockedAncients` has no extension hook beyond Harmony patching.
- `EventModel.GetAssetPaths`, `EventModel.BackgroundScenePath`, and `NAncientEventLayout` show Ancient events use background scenes/assets.
- previous framework exposes `CustomAncientModel.CustomScenePath`, but no Morvi/Lotha event scene/image/import/export-preset files are present locally.
- Death/failure flow requires direct proof before Lotha Death Reprieve or Vakuu Fight can be implemented.

Status: planning-only and do-not-implement-yet.

### Ascension 11-20

Active source lives under `EZMicroBalanceCode/Ascension/**`.

Inventory:

- A11 map shape: `AscensionMapService.cs` modifies generated maps.
- A12 Firemarked Elite: map metadata, icon/hover patches, combat modifiers, reward option count.
- A13 Fission: reward enchantment chance and diagnostics in `AscensionRewardService.cs`.
- A14/A15/A18 Rootblight/Blight Sprout: `RootDeckService.cs`, `RootBudCombatHook.cs`, root cards, saved fields, combat-end resolver.
- A16 Banner rooms: map metadata, hover, combat modifiers, rewards.
- A17 Deep Branch: map metadata and hover.
- A19 Boss Seal: boss metadata, hover, combat modifiers, reward handling.
- A20 Brand/second-boss flow: `AscensionA20Patches.cs`, `AscensionA20RewardScreenPatches.cs`, `A20Courtyard.cs`, and single-player-gated dual-king-brand behavior.
- Multiplayer selection/warning/downgrade: selector patches plus `MultiplayerDiagnostics.cs`.

Status: source-implemented for private-beta test scope, not release-verified. A21-A30 and custom characters are out of scope.

## Packaging And Runtime State

Expected package hashes from `docs/private-beta-verification-handoff.md`:

| Artifact | SHA256 |
| --- | --- |
| zip | `8AA5F65BECF6672B7B41F3B474851A828BFAF60250F04FB2C58061F52747D128` |
| DLL | `C64B5787625F497E930D4470AB4758950F59D9574D22847996FBCF55E0DACF71` |
| manifest | `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02` |
| PCK | `39F0ED5E592BC9131BE7C317450357F9ACC82D7031D97C92C71C59C8B5109736` |

Current saved fields:

- Ancient source defines 7 active `previous saved-state API` declarations.
- Ascension source defines 9 active `previous saved-state API` declarations.
- Current active source total: 16.
- Any smoke evidence or log guard still reporting 13 fields is stale after `MorviStateKey` plus the Urda/Morvi card-backed deck mirror fields.

Version state:

- Local source snapshot docs record Slay the Spire 2 `v0.105.0`.
- `docs/dev-environment.md` records installed Steam game `release_info.json` observed as `v0.105.1` on 2026-05-11.
- previous framework project/runtime target is `v3.1.2` under `<GameRoot>\mods\previous framework`.
- Do not claim v0.105.1 source-backed behavior until local `source code/src/Core/**` is refreshed from v0.105.1.

Package evidence:

- Current handoff says build and publish passed after the latest no-test Urda/Morvi hook-state package refresh.
- The 2026-05-13 no-test package refresh updated the installed, staging, versioned, and zip artifact hashes above.
- Release readiness still requires `dotnet test`, release-artifact tests, clean installed artifact check, normal Steam log audit for the refreshed package, and manual matrix closure.

## Recent Feedback Bug/Risk Audit

| Area | Source status | Hypothesis | Evidence needed | Fix category |
| --- | --- | --- | --- | --- |
| Firemark/Banner monotony | `AscensionMapService.cs` now uses stable seed/act/coord and act-level kind ordering rather than deterministic first-kind assignment. | First marked room should now vary, but player perception depends on visible hover/preview and route choice. | New `EZMB_ASCENSION_DIAGNOSTICS=1` map logs plus live map screenshots across seeds. | Manual test plus balance/design decision. |
| Firemark/Banner balance | Giant, Might, Forge Armor, Vanguard, Shield Formation, and Bounty are source-configured in combat/reward services and localization. | Numbers may be playable but need live tuning; Might can scale hard in multi-enemy fights. | A12/A16 seeded combats with logs and outcome notes. | Design/balance decision. |
| Fission visibility | Chance constants are 10/15/20/5 by room source; eligibility filtering and diagnostics exist. | Players may not see Fission because chance is low, candidate cards are ineligible, or it rolled false. | Diagnostics rows with source label, chance, eligible count, roll, applied, card id. | Diagnostic/manual test. |
| Blight Sprout/Rootblight | Root cards, saved fields, cap, notices, and combat-end resolver are source-complete. | Previous discard-count/art/notice concerns need live verification; source uses owner/local notice guards in key paths. | A14/A15/A18 boss/elite tests, save/load, Rootblight hover/art screenshots, co-op host/client logs. | Manual test plus multiplayer blocker. |
| Boss Seal/A19/A20 | Boss metadata, hover, combat notices/effects, A20 intermission/courtyard path, and brand metadata are source-present. | A20 direct transition and reward screen intermission are highest softlock risk. | A19 boss preview/combat logs; A20 full boss1->intermission->boss2 run; clean godot logs. | Manual test plus source fix if any transition issue appears. |
| Multiplayer selection/logging | Selector patches and join mismatch diagnostics are source-present. | Vanilla "version differs" may be ModelDb hash or mod-list drift; diagnostics should now identify cause. | Two-client Steam host/client logs, save/quit propagation, black-screen reproduction or clean pass. | Multiplayer blocker. |
| Per-player Urda/Rootblight | Urda stores Player string state, mirrors it onto deck cards, and loops players for room rewards; Rootblight has more owner/local guards. | Urda reward/room hooks may apply on both host/client or to inactive players unless command replication model proves safe. | Co-op reward/room entry tests with host/client logs and deck/gold comparisons. | Multiplayer blocker/source fix. |

## Experiment And Exploit Audit

| Exploit | Feature | Preconditions | Current vulnerability | Source evidence | Suggested guard/test |
| --- | --- | --- | --- | --- | --- |
| Power cards copied or extra-played | Morvi/Lotha future copy effects | Copy/replay hook allows any card type. | Morvi Misprint currently excludes Power; Lotha future effects must copy that rule. | `MorviRunHook.AfterCardPlayed` checks Attack/Skill. | Source guard tests for all replay/copy features. |
| Extra-play recursion | Morvi Misprint, future Lotha | Autoplayed copy triggers replay hook again. | Current Morvi has `IsResolvingMisprint`, `MisprintUsedThisCombat`, and clone checks. | `CombatState` weak table in `MorviRunHook.cs`. | Live combat with copy-generating cards; unit guard for clone/type checks. |
| First-card triggers duplicated | Morvi Misprint, future effects | Replay copy counts as played card. | Likely by design risk; not source-proven safe for every "first card" trigger. | Uses `CardCmd.AutoPlay`, which goes through card-play pipeline. | Manual edge-card matrix before enabling. |
| Generated temporary cards enter master deck | Morvi Misprint, Blight Sprout, future cards | Add generated cards to combat piles and fail cleanup. | Morvi uses helper and removes unpiled failures; Root services have cleanup paths, but live proof pending. | `TryAddGeneratedCardToCombat`, `RemoveUnpiledCombatCard` patterns. | Combat end/deck count tests. |
| Debt payment rounding | Morvi Debt Settlement | Player has 1-24 gold and low HP. | Source pays min gold and up to 3 nonlethal HP; design may allow partial gold plus HP discount. | `PayDebtSettlement` uses `Math.Min` and nonlethal HP cap. | Decide intended economics, then guard/localize. |
| Red Ink repeated button click | Future Morvi Red Ink | Active button injected into UI. | Not implemented; source API not proven. | No active source. | Do not implement until button lifecycle and debounce are source-proven. |
| Reward alternative double completion | Seedbed, Humus, Debt | Alternative callback fires twice or reward is reopened. | Humus/Debt have handled flags in weak-table reward context; save/load loses weak-table state. | `ConditionalWeakTable<CardReward, CardRewardContext>`. | Save/reload during reward screen, double-click/reopen tests. |
| Save/load reward duplication | Seedbed, Humus, Debt | Save before/after alternative or before payoff offer. | High risk because reward context is runtime-only and custom reward serialization is limited. | `CardReward.ToSerializable` does not serialize custom filters/pools/flags; weak tables do not persist. | Manual save/load matrix before release; avoid claiming stability. |
| Seedbed accept/skip/reopen exploit | Urda Seedbed | Reroll/reopen reward screen. | Counts accepted only, not skipped; max-HP cost checks exist; weak-table context still can be lost. | `AcceptSeedbed` increments accepted after add/cost flow. | Reward screen reroll/reopen/save tests. |
| Humus third skip duplicate payoff | Urda Humus Pact | Third compost, resolver fails or reload happens. | Pending flag remains until resolver success; Player field persistence not source-proven, deck mirror live proof pending. | `HumusCompletionPending` cleared only after `ResolveHumusCompletion`. | Resolver-fail and save/load tests. |
| Moss Map duplicate room reward | Urda Moss Map | Save/load on room entry or hook runs on host/client. | Room mask stored in Player string and mirrored to deck cards, but persistence and multiplayer authority are unproven. | `AfterRoomEntered` loops `RunState.Players`. | Save before/after first room type; co-op room entry comparison. |
| A20 boss1 reward duplicate/softlock | A20 | Reward screen/intermission transition. | High risk until full live path verified. | A20 reward screen/courtyard patches. | Full A20 run with clean logs. |
| Fission repeated enchantment | A13 | Reward screen regenerated/reopened. | Source avoids adding if any option already has Fission; reward reentry still needs live evidence. | `AscensionRewardService` checks existing Fission in options. | Reroll/reopen/save reward screen tests. |
| Rootblight downgrade/removal duplication | A14/A15/A18 | Save/reload around play, discard, combat end. | Source has cap/normalize and card markers, but live reload pending. | `RootDeckService` and `AscensionSavedStateFields`. | Save/load before play, after play, combat end. |
| Host/client both applying rewards | Urda/Morvi/Ascension | Hooks execute on all clients. | Unproven. Some services use `LocalContext.IsMe` or active-player checks; Urda Moss Map does not visibly filter active/local. | `RunState.Players` loops and reward alternatives. | Two-client host/client logs and deck/gold assertions. |

## Audit Files

- `source-api-map.md`
- `implementation-gap-matrix.md`
- `multiplayer-risk-matrix.md`
- `save-load-risk-matrix.md`
- `windows-mac-platform-risk-matrix.md`
- `manual-test-master-matrix.md`
- `next-implementation-goals.md`
