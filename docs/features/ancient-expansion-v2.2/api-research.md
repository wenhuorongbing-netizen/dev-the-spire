# Ancient Expansion v2.2 API Research

Status: current eleven-blessing Urda source evidence plus default-on Morvi source evidence, default-on Lotha source evidence, and hidden-by-default source-dedicated Vakuu fight/Contract source evidence. Vakuu now uses a direct parent-room stack transition, explicitly clears the parent event node before combat, avoids storing `ParentEventId` on the active child combat room, records the parent only for prefinished restore, and skips the duplicate Ancient heal during prefinished parent restore. Lotha now encodes Death Reprieve phase through deck-mirrored state. Runtime gameplay, exact save/load restore, post-victory Vakuu restore, and co-op proof remain pending.

## Current Source-Backed Facts

Current Urda source files:

- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaAncient.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaBlessingIds.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaSeedling.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaSeedbed.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaRainBreath.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/WitheredHusk.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaFeatureGate.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaInitializer.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaOptionRelic.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaStandardOptionRelics.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaRootSightOptionRelic.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaSeedBankOptionRelic.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaOptionRelicClickPatch.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaRunHook.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaCombatHook.cs`

Observed implementation baseline:

- Urda is default-on unless `SPIREPLUS_DISABLE_URDA=1` is set. Legacy `EZMB_DISABLE_URDA=1` still works.
- `SPIREPLUS_FORCE_ANCIENT=URDA` focuses Urda when needed. Legacy `EZMB_FORCE_ANCIENT=URDA` still works and is not required for Urda visibility.
- Current blessing ids are `urda_seedbed`, `urda_humus_pact`, `urda_molting`, `urda_moss_map`, `urda_trial_branch`, `urda_shallow_root_relic`, `urda_rooted_route`, `urda_after_rain`, `urda_root_sight`, `urda_seed_bank`, and `urda_elite_root`.
- Current source hooks cover reward alternatives, reward-taken follow-up handling, act entry, room entry, map marker checks, death-prevention checks, reward-card storage, and Molting card setup.
- Humus Pact now uses an explicit `EZMB_URDA_HUMUS_PACT` card reward alternative instead of a global `CardReward.OnSkipped` postfix.
- Seedbed counts accepted Seedbed choices, not reward alternative generation.
- Live gameplay and save/load evidence remains pending.

Current Morvi source files:

- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviAncient.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviAncient.Options.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviBlessingIds.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviArchivePageCard.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviArchiveDrawPage.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviArchiveVeilPage.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviArchiveBurnPage.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviArchiveDiscountPage.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviArchiveBraveryPage.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviArchiveDexterityPage.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviRedInkOverdraftCard.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviWastePaper.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviFeatureGate.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviInitializer.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviOptionRelics.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviPowers.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviBlessingService.CombatLifecycle.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviCombatHook.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviRunHook.cs`

Morvi evidence recorded in this source pass:

- Local `source code/src/Core/Models/Acts/Hive.cs` shows Act 2 Ancient selection flows through `Hive.GetUnlockedAncients(...)`.
- Local `source code/src/Core/Rewards/CardReward.cs` and `source code/src/Core/Entities/CardRewardAlternatives/CardRewardAlternative.cs` show reward alternatives are generated from `Hook.ModifyCardRewardAlternatives(...)` and are limited to two visible alternatives.
- Local `source code/src/Core/Commands/CardCmd.cs` shows `CardCmd.AutoPlay(...)` goes through card play hooks and can resolve automatic Attack/Skill replays.
- Local `source code/src/Core/Commands/CardPileCmd.cs` shows `AddGeneratedCardToCombat(...)` indexes the first result from `AddGeneratedCardsToCombat(...)`. The lower-level method can return an empty list when combat is not in progress or the owner has no combat state, and `Add(...)` can return `success=false` for combat-ending/dead-owner cases. The shared `AncientCardHelpers.TryAddGeneratedCardToCombat(...)` wrapper now guards `CombatManager.Instance.IsOverOrEnding`, `!CombatManager.Instance.IsInProgress`, and missing owner combat state, calls `AddGeneratedCardsToCombat([card], ...)` directly, checks the first result for null/unsuccessful adds, and removes failed generated cards from combat state instead of leaving unpiled cards.
- Local `source code/src/Core/Commands/CardPileCmd.cs` also shows `Add(..., PileType.Hand)` redirects a full-hand add into the discard pile. Red Ink Overdraft therefore skips temporary-card generation when the hand is full, verifies the result pile is still `PileType.Hand`, and removes/logs any generated card that lands elsewhere.
- Local `source code/src/Core/Combat/CombatManager.cs` calls `Hook.AfterCombatEnd(...)` before combat victory cleanup, and local `source code/src/Core/Commands/CreatureCmd.cs` routes ordinary damage through the normal death path. Morvi combat-end debt HP fallbacks now share a nonlethal helper that caps HP loss at current HP minus 1 for both Red Ink Overdraft and Debt Settlement.
- Local `source code/src/Core/Models/AbstractModel.cs` exposes `BeforeCombatStart`, `AfterCardPlayed`, `TryModifyCardRewardOptionsLate`, `TryModifyCardRewardAlternatives`, and `AfterRewardTaken` hooks used by the Morvi source slice.
- Morvi Debt Settlement is the current v2.2 combat-end debt model: it grants 220 Gold, optional remove/upgrade selections, sets Debt to 320, then pays up to 40 Debt at each combat end. Missing Gold falls back to the shared nonlethal HP loss helper capped so the player is not reduced below 1 HP.
- Morvi is default-on for private-beta direct testing with preferred `SPIREPLUS_DISABLE_MORVI`, legacy `EZMB_DISABLE_MORVI`, and force-test gates; no live gameplay/save-load/co-op evidence is claimed.

## 2026-05-12/13 Lotha/Event Visual Evidence

Local source findings:

- `source code/src/Core/Models/Acts/Glory.cs` shows Act 3 Ancient selection flows through `Glory.GetUnlockedAncients(...)`, which returns `AllAncients.ToList()` and has no native mod extension hook.
- Lotha Act 3 insertion uses a narrow Harmony postfix on `Glory.GetUnlockedAncients(...)`, equivalent in shape to the existing Urda/Morvi insertion patches, because no safer native, RitsuLib, or template Ancient-pool API is present locally.
- `source code/src/Core/Models/EventModel.cs` shows `GetAssetPaths(...)` preloads `BackgroundScenePath` for `EventLayoutType.Ancient`.
- `source code/src/Core/Nodes/Events/NAncientEventLayout.cs` shows Ancient rooms initialize visuals by calling `CreateBackgroundScene()` and adding that scene to the Ancient background container; the normal event portrait path is not the active Ancient background path.
- Historical previous package `v3.1.4` XML/decompiled local package evidence exposed `CustomAncientModel.CustomScenePath` and patched the Ancient background scene path only for `CustomAncientModel`. Current beta.93 code no longer uses that path: Urda, Morvi, and Lotha derive from RitsuLib `ModAncientEventTemplate`, register through `SharedAncient<...>()`, and expose `CustomBackgroundScenePath`, map-icon, and run-history icon properties directly.
- Lotha now has `EZMicroBalance/images/events/ezmb_lotha.png`, `EZMicroBalance/scenes/events/background_scenes/ezmb_lotha.tscn`, separate map/run-history icons, option art, and export entries. Morvi now has `EZMicroBalance/images/events/ezmb_morvi.png`, currently sourced from the recovered user-uploaded blue-eye court image archived under `.tools/art-generation/event-background-repair-20260515-live-feedback/sources/`, plus `EZMicroBalance/scenes/events/background_scenes/ezmb_morvi.tscn`, separate map/run-history icons, option art, and export entries.
- Local card-play source supports Lotha replay safety: `CardModel.OnPlayWrapper(...)` calls `Hook.ModifyCardPlayCount(...)` for player-driven play count changes, while `CardPlay.IsAutoPlay`, `CardPlay.IsFirstInSeries`, `CardModel.IsClone`, and `CombatHistory.CardPlaysFinished` expose enough state to exclude autoplay/generated clone executions from Lotha recursion and turn-end echo tracking.
- Lotha v2.2/v3.3 corrective polish reuses source-backed command paths already inspected locally: Mirror Rebuttal uses `CardSelectCmd.FromDeckGeneric(...)`, a `SavedAttachedState<CardModel, bool>` deck-card marker, `CardModel.DeckVersion`, and `CardPileCmd.Add(..., PileType.Hand)` to move the matching combat card on the first player turn after normal draw; Attack/Skill extra plays use `ModifyCardPlayCount` rather than generated autoplay copies; Mirror Hall Echo now uses the v0.106.1 `AfterSideTurnEnd(...)` model hook plus combat history to record the last player-played non-Status Attack/Skill/Power for the next player turn; Deferred Verdict applies player-owned `LothaVerdictPower` stacks through `PowerCmd.Apply(...)` and consumes them with `PowerCmd.Decrement(...)`; Power-card replacement cost preview/payment uses `TryModifyEnergyCostInCombat(...)` and `TryModifyStarCost(...)`, then applies only the source-design draw benefit after play where that blessing still grants one.
- Lotha Presumption uses `AfterDamageReceived(...)` with a conservative enemy-attack approximation: `DamageResult.UnblockedDamage > 0`, enemy dealer, `cardSource == null`, and `ValueProp.Move`. Poison, Doom, HP-loss scripts, and self payments inspected locally do not match that full shape.
- Lotha Closed Court uses `TryModifyRewardsLate(...)` to remove only `CardReward` instances from combat rewards, leaving gold, potion, and relic reward objects intact. Its v3.3 resource plan uses `CardPileCmd.Draw(...)` and `PlayerCmd.GainEnergy(...)` on turn 1 and turn 4; the previous first-three-card temporary discount is removed.
- Lotha Death Reprieve uses the local `ShouldDieLate` / `AfterPreventingDeath` death-prevention path, modeled after local `CreatureCmd.Kill(...)` and `LizardTail` source. `CreatureCmd.Kill(force: true)` is used only at reprieve failure. source-safe deviation: local turn-flow evidence did not prove a safe immediate enemy-turn interruption into a new player turn, so enemy-turn lethal marks a pending reprieve that starts at the next player turn. Current source encodes `DeathReprieveUsed` plus `DeathReprievePhase` through the existing Lotha deck mirror. Live lethal-path and restore testing remains pending.
- Lotha Public Evidence uses `ModifyPowerAmountGivenAdditive`, `TryModifyPowerAmountReceived`, and `AfterPowerAmountChanged` to double non-damaging negative status applications and manage `LothaEnlightenmentPower`. Eligibility uses `power.GetTypeForAmount(amount) == PowerType.Debuff` as the base gate, but excludes source-proven damage/kill Debuffs. Local Core evidence: `WeakPower`, `VulnerablePower`, and `FrailPower` are non-damage Debuffs; `PoisonPower` is also a Debuff but deals unblockable/unpowered side-turn damage, so it is excluded along with Constrict, Demise, Disintegration, Doom, Magic Bomb, Strangle, and The Gambit.

## 2026-05-14 Ancient UI/Art Resource Routing Evidence

Local source findings:

- `source code/src/Core/Nodes/Events/NAncientEventLayout.cs` adds `AncientEventModel.CreateBackgroundScene().Instantiate<Control>(...)` into the Ancient background container, so custom clicked-Ancient backgrounds must be `Control`-root scene resources rather than raw image paths or `Node2D` scenes.
- `source code/src/Core/Models/EventModel.cs` preloads `BackgroundScenePath` for `EventLayoutType.Ancient` through `GetAssetPaths(...)`, which supports keeping event background scenes separate from map and run-history icons.
- `source code/src/Core/Events/EventOption.cs` shows `EventOption.FromRelic(...)` and `EventOption.WithRelic<T>(Player?)` use mutable relic instances and relic hover tips for option art/hover presentation. Current Urda, Morvi, and Lotha options use `WithRelic<T>` marker relics; Vakuu fight uses `EventOption.FromRelic(...)`.
- `source code/src/Core/Models/RelicModel.cs` loads `PackedIconPath`, `PackedIconOutlinePath`, and `BigIconPath` separately. Current marker relics override their packed, outline, and big icon paths to the option art path so the option button is not dependent on the generic shared relic fallback.
- `source code/src/Core/Helpers/ImageHelper.cs` and `source code/src/Core/Nodes/Screens/RunHistoryScreen/NMapPointHistoryEntry.cs` route Ancient map/run-history images through room-icon lookups, not through the clicked Ancient background scene. Current Urda, Morvi, and Lotha map/run-history paths therefore stay under `EZMicroBalance/images/ancients/**` and not `EZMicroBalance/images/events/**`.
- Current guard/audit coverage now checks source role separation, option-marker resource/localization coverage, manifest-target file/hash presence, and manifest-target export coverage. This is static source/resource evidence only; it is not clicked live UI proof.

## 2026-05-13/14 Vakuu Fight Source Evidence

Local source findings:

- `source code/src/Core/Models/EventModel.cs` exposes a protected `EnterCombatWithoutExitingEvent(...)` helper that clears `Node = null`, creates a `CombatRoom`, sets `ShouldResumeParentEventAfterCombat`, and assigns `ParentEventId = base.Id`, but it first requires `IsShared`. Source Vakuu is not a shared event, so current Vakuu source does not call Core's `EnterCombatWithoutExitingEvent(...)`.
- Current `VakuuFightService.StartFight(...)` hand-builds the child combat room, clears the parent event `Node` through the event backing field, sets `ShouldResumeParentEventAfterCombat = true`, and awaits direct `EnterRoomWithoutExitingCurrentRoom(...)`. This preserves the Core room-stack resume shape while avoiding the helper's non-shared-event guard.
- `source code/src/Core/Rooms/CombatRoom.cs` serializes parent-event combat rooms only after they are prefinished. The current direct room-stack path no longer assigns `ParentEventId` while the Vakuu combat room is active, avoiding the known active `ParentEventId` serialization blocker.
- A narrow `CombatRoom.ToSerializable()` postfix still records the Vakuu parent id for prefinished `EzmbVakuuTrialEncounter` combat rooms that need to resume the parent event after a restore.
- `source code/src/Core/Runs/RunManager.cs` and `source code/src/Core/Rooms/EventRoom.cs` show parent events resume after terminal combat rewards when the combat room asks to resume the parent event.
- `source code/src/Core/Events/EventModel.cs` has a protected `SetEventState(...)` path used by normal event state transitions. The implementation calls it by reflection only after the parent Vakuu event resumes from the custom combat.
- `source code/src/Core/Nodes/Combat/NCombatUi.cs` skips `CombatRoom.OfferRoomEndRewards(...)` when `Encounter.ShouldGiveRewards` is false. `source code/src/Core/Models/Encounters/BattlewornDummyEventEncounter.cs` uses `RoomType.Monster` plus `ShouldGiveRewards => false` for an event-launched combat, so the custom Vakuu encounter follows that source-backed shape instead of registering an unexpected `RoomType.Event` encounter.
- `source code/src/Core/Rooms/CombatRoom.cs` `StartPreFinishedCombat(...)` calls `CombatRoom.OfferRoomEndRewards()`, which does not itself respect `Encounter.ShouldGiveRewards`. `VakuuFightNoRewardRestorePatch` now intercepts that prefinished/no-reward restore path for `EzmbVakuuTrialEncounter` and resumes the parent Vakuu event instead of generating normal combat rewards. Vakuu still remains save/load-unverified until live restore testing proves the empty terminal reward/no-normal-reward/resume behavior in game.
- Vakuu victory choices now combine source vanilla Act 3 Ancient reward relics with custom Lotha option relics through `LothaRewardSelectionService.SelectBlessing(...)`. This keeps Lotha victory rewards visible in the relic bar and uses the same blessing-state write path as the Lotha event instead of granting a hidden saved-field-only reward.
- If filtering owned Act 3 Ancient blessings leaves no non-Vakuu options, the source falls back to a single continue option instead of presenting an empty reward choice set. If options remain, broken locks set the target choice count to 1/2/3, and each broken lock grants 50 Gold on the chosen victory option or fallback.
- Local `source code/src/Core/Commands/PlayerCmd.cs` exposes `GainEnergy(...)`, local `source code/src/Core/Commands/CardPileCmd.cs` exposes hand draw, local `source code/src/Core/Commands/CreatureCmd.cs` exposes `GainBlock(...)` and `Damage(...)`, and local `source code/src/Core/Commands/DamageCmd.cs` exposes source-backed card attacks. Vakuu Contracts use those command paths for Knife/Gold/Shelter effects.
- Local `source code/src/Core/Combat/CombatManager.cs` calls `Hook.AfterPlayerTurnStart(...)` after normal hand draw. The Vakuu combat hook uses that timing to offer contract choices after the hand draw on player turns 1, 3, and 5 when source-safe.
- Local damage hooks expose unblocked damage through `AfterDamageReceived(...)` and `DamageResult.UnblockedDamage`. Vakuu tracks player-turn unblocked damage and breaks one Stolen Vault lock when damage reaches 40 in a single player turn.
- Local `PowerModel.ModifyDamageAdditive(...)` participates in powered attack intent/damage calculation. `VakuuBloodDebtPower` uses that hook to add 2 damage per stack to each powered Vakuu attack hit.
- The hook is scoped by `combatState.Encounter is EzmbVakuuTrialEncounter` and the combat-hook subscriber is gated through `VakuuFightFeatureGate.IsFightEnabledForRun(...)`, preserving the current single-player-only stance.
- `VakuuFightFeatureGate.IsFightEnabled(...)` now requires `ShouldEnableFight`, which is true only for preferred `SPIREPLUS_ENABLE_VAKUU_FIGHT=1`, preferred `SPIREPLUS_FORCE_VAKUU_FIGHT=1`, or legacy `EZMB_*` fight aliases.
- Multiplayer authority is not source-proven. `VakuuFightFeatureGate.IsFightEnabledForRun(...)` requires `runState.Players.Count == 1`.
- Local `source code/src/Core/Rooms/CombatRoom.cs` throws in `ToSerializable()` when a combat room has `ParentEventId` and is not pre-finished. Current Vakuu source no longer stores `ParentEventId` while the combat room is active; prefinished parent recording remains isolated to the `CombatRoom.ToSerializable()` postfix. Live active-fight save/load is still pending because runtime behavior has not been exercised.
- Live UI, combat victory, save/load, failure/death, and co-op verification remain pending.

## 2026-05-12 Urda Stabilization Evidence

Local source findings:

- `CardRewardAlternative.Generate(...)` creates built-in `Skip` and optional `REROLL` alternatives, then runs hook-based alternatives. It throws if more than two alternatives exist, so Urda alternatives must no-op when two buttons are already present.
- `CardReward.Populate()` and `CardReward.Reroll()` can regenerate reward options and alternatives. Urda must not advance Seedbed counters from alternative generation alone.
- `CardReward.OnSkipped()` is called when unselected rewards are skipped by reward-set completion/abandonment. It is not a safe place to open Humus removal or custom reward UI.
- `Reward.SelectUnsynchronized(...)` calls `Hook.AfterRewardTaken(...)` after a reward has successfully selected. Urda uses this later hook to resolve the third Humus Pact removal/reward flow after the card reward screen has closed.
- `RewardsSetSynchronizer.SelectRewardForPlayer(...)` completes the current reward set after `reward.SelectUnsynchronized()` returns. Humus Pact's third payoff must therefore avoid opening UI from `OnSkipped`, avoid normal reward-skip reentry, and keep a pending latch until its payoff resolver succeeds.
- `CreatureCmd.LoseMaxHp(...)` can damage the player before max HP is clamped, so Seedbed is not offered when max HP is not greater than its cost.
- `CreatureCmd.GainMaxHp(...)` also heals by the gained amount. Seedbed's completion bonus uses `CreatureCmd.SetMaxHp(...)` to match the documented "no heal" behavior.
- `CardFactory.CreateForReward(...)` can invoke card reward modification hooks unless creation flags suppress them. Humus Pact's one-card payoff now uses `NoModifyHooks`, `NoCardPoolModifications`, and `NoUpgradeRoll`, then upgrades the selected card itself.
- `RewardsSet.WithSkippingDisallowed()` is source-backed and is used for Humus Pact's payoff reward so the one-card payoff is not accidentally skipped.
- Humus Pact's payoff resolver now creates the payoff card before optional removals and clears `HumusCompletionPending` only after resolver success. If no card can be generated, no removals are consumed and the pending bit remains available for a later retry.

Save/load evidence:

- `AncientSavedStateFields.UrdaStateKey` packs Urda progress into a `SavedAttachedState<Player, string>` and `AncientSavedStateFields.UrdaDeckStateKey` mirrors that encoded string onto deck cards through `AncientPlayerState`.
- `AncientSavedStateFields.MorviStateKey` and `AncientSavedStateFields.MorviDeckStateKey` use the same Player runtime plus card-backed deck mirror pattern for Morvi source progress.
- 2026-05-14 state mirror source audit: `AncientPlayerState.Get(...)` reads runtime state first, mirrors it to owned, non-removed deck cards, and restores the runtime field from the first owned, non-removed deck mirror when runtime state is empty. `AncientPlayerState.Set(...)` writes runtime plus deck mirrors, and `AncientPlayerState.SyncDeck(...)` forces the same recovery/mirror path. Urda, Morvi, and Lotha encoded state reads/writes funnel through hook-local `GetSelectedBlessing(...)`, `GetProgress(...)`, `SetProgress(...)`/`SetState(...)`, and recurrent `AfterCardChangedPiles(...)` sync calls; active source guards reject direct indexing of `UrdaStateKey`, `UrdaDeckStateKey`, `MorviStateKey`, `MorviDeckStateKey`, `LothaStateKey`, and `LothaDeckStateKey` outside the helper.
- The packed state now includes a `HumusCompletionPending` bit and can read the prior eight-field shape.
- RitsuLib `SavedAttachedState<TKey, TValue>` replaced the previous previous package saved-field API. `Player` persistence for the attached state remains not live-proven by this pass; the deck mirror is a safer carrier for live save/load testing, not a substitute for runtime proof.
- Local Core source reinforces that pending status: `Player.ToSerializable()` writes a fixed `SerializablePlayer` shape, `SerializablePlayer` has no general `SavedProperties`/`Props` field, `ExtraPlayerFields` serializes only built-in fixed fields, and inspected `SavedProperties.From(...)` call sites are card/relic/modifier save paths rather than `Player`.
- Do not close Urda save/load checklist rows until live save/load confirms the player-owned encoded state survives reload.

## 2026-05-14 Save/Load Red-Team Evidence

Vakuu parent-linked child combat evidence:

- Local `source code/src/Core/Rooms/CombatRoom.cs` shows `ParentEventId`, `ShouldResumeParentEventAfterCombat`, `FromSerializable(...)`, `StartPreFinishedCombat()`, and `ToSerializable()`. `ToSerializable()` throws when `ParentEventId != null && !IsPreFinished`.
- Local `source code/src/Core/Runs/RunManager.cs` shows `EnterRoomWithoutExitingCurrentRoom(...)` pushes a child room without exiting the parent event and appends a room-history entry; `ProceedFromTerminalRewardsScreen()` resumes the previous room when the current room is a `CombatRoom` with `ShouldResumeParentEventAfterCombat != false`; `ResumePreviousRoom()` pops the child room and calls the parent room's `Resume(...)`.
- Local `source code/src/Core/Rooms/EventRoom.cs` resumes events through `EventSynchronizer.ResumeEvents(exitedRoom)` and saves only when Ancient event state changes mark the event pre-finished.
- Local `source code/src/Core/Runs/RunManager.cs`, `source code/src/Core/Saves/SaveManager.cs`, and `source code/src/Core/Saves/SerializableRun.cs` show run saves carry `PreFinishedRoom = preFinishedRoom?.ToSerializable()`. This is source-compatible with a pre-finished parent-linked combat room after victory and rejects active combat rooms that already carry `ParentEventId`.
- Current `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs` creates an `EzmbVakuuTrialEncounter` `CombatRoom`, sets parent resume without active `ParentEventId`, clears the parent event node, and enters the room through direct `EnterRoomWithoutExitingCurrentRoom(...)`. It does not call Core's `EnterCombatWithoutExitingEvent(...)`, and it does not store `ParentEventId` while the combat room is active.
- Current `VakuuFightPreFinishedSavePatch` patches `CombatRoom.ToSerializable()` and writes `serializableRoom.ParentEventId = ModelDb.AncientEvent<Vakuu>().Id` plus `ShouldResumeParentEvent = true` only when the room is an `EzmbVakuuTrialEncounter`, `IsPreFinished`, and still marked for parent resume.
- Local `source code/src/Core/Runs/RunManager.cs` reconstructs that parent room as a new non-prefinished `EventRoom`, and local `source code/src/Core/Models/AncientEventModel.cs` heals when `BeforeEventStarted(false)` runs. Vakuu patches the reconstructed prefinished parent-restore path to skip only that duplicate Ancient heal while leaving normal Vakuu entry unchanged.
- Conclusion: the reported black-screen risk is addressed at source level by clearing the parent event node before a direct parent-room stack transition, and the active fight no longer assigns the known Core-rejected active `ParentEventId`. Prefinished parent restore is source-shaped, skips the duplicate reconstructed-parent Ancient heal, and patches the no-normal-reward restore path, but live save/load still must prove active-fight behavior if the game permits saving there, the prefinished parent-resume path, and the final Vakuu victory choices.

Lotha Death Reprieve persistence evidence:

- Current Lotha transient combat state lives in `LothaBlessingService.CombatState.cs`: live booleans `DeathReprieveActive`, `DeathReprievePendingStart`, and `DeathReprieveStarted` remain in a private `LothaCombatState` held by `ConditionalWeakTable<Player, LothaCombatState>`, while durable progress still lives in `LothaBlessingService.State.cs` as `Progress(bool DeathReprieveUsed, DeathReprievePhase DeathReprievePhase)`.
- `LothaBlessingService.State.cs` encodes `DeathReprievePhase.None`, `PendingStart`, `Active`, and `Resolved` through `AncientSavedStateFields.LothaStateKey` and `AncientSavedStateFields.LothaDeckStateKey`; `SetProgress(...)` writes the used flag and phase before starting or pending the reprieve, and `LothaBlessingService.DeathReprieveState.cs` rebuilds pending/active protection state from that encoded progress.
- `EZMicroBalanceCode/Ancients/Common/AncientPlayerState.cs` mirrors encoded Ancient state from the `Player` field onto deck cards and can rebuild runtime state from the first nonempty deck-card mirror.
- Local `source code/src/Core/Entities/Players/Player.cs` and `source code/src/Core/Saves/Runs/SerializablePlayer.cs` show player save data contains fixed HP, max HP, deck, relics, potions, RNG/odds, unlock state, discoveries, and fixed extra fields. It does not expose a general player `SavedProperties`/`Props` field.
- Local `source code/src/Core/Models/CardModel.cs` and `source code/src/Core/Models/RelicModel.cs` save `SavedProperties.From(this)`, while local `source code/src/Core/Models/PowerModel.cs` has no corresponding `ToSerializable()` path. Local `Creature` power lists are runtime state; no source evidence here proves combat powers survive a run save/load.
- Local `source code/src/Core/Commands/CreatureCmd.cs` shows non-forced death runs through `Hook.ShouldDie(...)` and `Hook.AfterPreventingDeath(...)`, while `CreatureCmd.Kill(..., force: true)` bypasses the death-prevention check. Current Lotha failure intentionally uses `force: true`.
- Local `source code/src/Core/Combat/CombatManager.cs` shows normal player-turn start and turn-end hooks drive the reprieve start/end sequence (`AfterPlayerTurnStart`, `AfterSideTurnEnd` model dispatch through Core's turn-end hook flow). Current Lotha source uses those hooks to start a rehydrated pending reprieve and to mark the phase `Resolved` at turn end, combat end, and before the forced failure death path.
- Conclusion: the current source reduces duplicate-trigger and lost-protection risk by persisting the once-per-run used flag plus pending/active/resolved phase through the deck-card mirror. Exact save/load continuation of an already active reprieve turn is not fully source-proven because Core run saves do not persist the complete active combat hand/energy/pile/power state here; keep live restore rows open.

Secondary tutorial reference checked only for orientation: `https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/index.html`.

## Required Future Research

Before claiming runtime-ready behavior or broadening Vakuu behavior beyond the current Contract/lock/debt hook, inspect local `source code/src/Core/` for:

| Area | Evidence Needed |
| --- | --- |
| Ancient selection | How act-specific Ancient pools are built and how extra options are shown. |
| Reward alternatives | How card reward alternatives complete, cancel, and save. |
| Extra play/copy | Safe command paths for playing a copy without recursive blessing triggers. |
| Power-card fallbacks | Whether cost/draw/energy fallbacks should be card commands, powers, or reward commands. |
| Active button UI | Whether Red Ink Overdraft should use relic, power, or combat UI action APIs. |
| Death interrupt | Whether Death Reprieve can safely intercept lethal damage without corrupting combat state. |
| Temporary storage | Whether save/load and restore preserve generated Contract cards correctly during the custom parent-event combat. |
| Multiplayer | Player ownership, host/client authority, and deterministic reward mutation. |

Primary evidence remains local game source. Prefer native game command APIs, RitsuLib APIs, and template-supported APIs before Harmony. previous package material in this file is historical migration evidence only unless the owner explicitly approves reintroducing a extra shared framework dependency. Tutorial material is secondary only: `https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/index.html`.
