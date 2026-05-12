# Ancient Expansion v2.2 API Research

Status: current Urda stabilization evidence plus default-off Morvi prototype evidence. Lotha, extra Urda blessings, and the Vakuu fight remain planning-only.

## Current Source-Backed Facts

Current Urda source files:

- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaAncient.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaBlessingIds.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaCards.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaFeatureGate.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaInitializer.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaRunHook.cs`

Observed implementation baseline:

- Urda is default-on unless `EZMB_DISABLE_URDA=1` is set.
- `EZMB_FORCE_ANCIENT=URDA` remains legacy-compatible but is not required for Urda visibility.
- Current blessing ids are `urda_seedbed`, `urda_humus_pact`, `urda_molting`, and `urda_moss_map`.
- Current source hooks cover reward alternatives, reward-taken follow-up handling, act entry, room entry, and Molting card setup.
- Humus Pact now uses an explicit `EZMB_URDA_HUMUS_PACT` card reward alternative instead of a global `CardReward.OnSkipped` postfix.
- Seedbed counts accepted Seedbed choices, not reward alternative generation.
- Live gameplay and save/load evidence remains pending.

Current Morvi prototype source files:

- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviAncient.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviBlessingIds.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviFeatureGate.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviInitializer.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviRunHook.cs`

Morvi evidence recorded in this source pass:

- Local `source code/src/Core/Models/Acts/Hive.cs` shows Act 2 Ancient selection flows through `Hive.GetUnlockedAncients(...)`.
- Local `source code/src/Core/Rewards/CardReward.cs` and `source code/src/Core/Entities/CardRewardAlternatives/CardRewardAlternative.cs` show reward alternatives are generated from `Hook.ModifyCardRewardAlternatives(...)` and are limited to two visible alternatives.
- Local `source code/src/Core/Commands/CardCmd.cs` shows `CardCmd.AutoPlay(...)` goes through card play hooks and can resolve automatic Attack/Skill replays.
- Local `source code/src/Core/Commands/CardPileCmd.cs` shows `AddGeneratedCardToCombat(...)` is the source-backed path for generated combat cards, but it indexes the first result from `AddGeneratedCardsToCombat(...)`. That lower-level method can return an empty list when combat is not in progress, and `Add(...)` can return `success=false` for combat-ending/dead-owner cases. Morvi now uses the local `AncientCardHelpers.TryAddGeneratedCardToCombat(...)` wrapper so failed generated copies are removed from combat state instead of leaving unpiled clones.
- Local `source code/src/Core/Models/AbstractModel.cs` exposes `BeforeCombatStart`, `AfterCardPlayed`, `TryModifyCardRewardOptionsLate`, `TryModifyCardRewardAlternatives`, and `AfterRewardTaken` hooks used by the Morvi prototype.
- Morvi Debt Settlement now defers the payoff to `AfterRewardTaken(...)`, offers the one-card payoff through `RewardsSet.WithCustomRewards(...).WithSkippingDisallowed().Offer()`, and clears `DebtRewardPending` only after that resolver succeeds.
- Morvi is default-off behind `EZMB_ENABLE_MORVI_V22=1`; no live gameplay/save-load/co-op evidence is claimed.

## 2026-05-12 Lotha/Event Visual Evidence

Local source findings:

- `source code/src/Core/Models/Acts/Glory.cs` shows Act 3 Ancient selection flows through `Glory.GetUnlockedAncients(...)`, which returns `AllAncients.ToList()` and has no native mod extension hook.
- A Lotha Act 3 insertion would therefore need a narrow Harmony postfix on `Glory.GetUnlockedAncients(...)`, equivalent in shape to the existing Urda/Morvi insertion patches, unless a safer BaseLib/template Ancient-pool API is introduced or adopted.
- `source code/src/Core/Models/EventModel.cs` shows `GetAssetPaths(...)` preloads `BackgroundScenePath` for `EventLayoutType.Ancient`.
- `source code/src/Core/Nodes/Events/NAncientEventLayout.cs` shows Ancient rooms initialize visuals by calling `CreateBackgroundScene()` and adding that scene to the Ancient background container; the normal event portrait path is not the active Ancient background path.
- BaseLib `v3.1.2` XML/decompiled local package evidence exposes `CustomAncientModel.CustomScenePath` and patches the Ancient background scene path only for `CustomAncientModel`.
- No explicit local source file exists for `EZMicroBalance/images/events/ezmb_morvi.png` or `EZMicroBalance/images/events/ezmb_lotha.png`, and no custom Morvi/Lotha Ancient background scene resource exists in this repo. This blocks truthful event-art/background integration and keeps Lotha gameplay planning-only in this pass.

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

- `AncientSavedStateFields.UrdaStateKey` still packs Urda progress into one `SavedSpireField<Player,string>`.
- The packed state now includes a `HumusCompletionPending` bit and can read the prior eight-field shape.
- BaseLib `SavedSpireField<TKey,TValue>` documentation says automatic save/load only works on model types that support `SavedProperty`, mainly cards and relics. `Player` persistence for this field is therefore not source-proven by this pass.
- Local Core source reinforces that pending status: `Player.ToSerializable()` writes a fixed `SerializablePlayer` shape, `SerializablePlayer` has no general `SavedProperties`/`Props` field, `ExtraPlayerFields` serializes only built-in fixed fields, and inspected `SavedProperties.From(...)` call sites are card/relic/modifier save paths rather than `Player`.
- Do not close Urda save/load checklist rows until live save/load confirms the player-owned encoded state survives reload.

Secondary tutorial reference checked only for orientation: `https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/index.html`.

## Required Future Research

Before implementing Morvi, Lotha, extra Urda blessings, or Vakuu fight, inspect local `source code/src/Core/` for:

| Area | Evidence Needed |
| --- | --- |
| Ancient selection | How act-specific Ancient pools are built and how extra options are shown. |
| Reward alternatives | How card reward alternatives complete, cancel, and save. |
| Extra play/copy | Safe command paths for playing a copy without recursive blessing triggers. |
| Power-card fallbacks | Whether cost/draw/energy fallbacks should be card commands, powers, or reward commands. |
| Active button UI | Whether Red Ink Overdraft should use relic, power, or combat UI action APIs. |
| Death interrupt | Whether Death Reprieve can safely intercept lethal damage without corrupting combat state. |
| Temporary storage | How Archive Pages and Temptation should exist in hand/draw/discard/reward zones. |
| Multiplayer | Player ownership, host/client authority, and deterministic reward mutation. |

Primary evidence remains local game source. BaseLib/RitsuLib/template APIs are preferred before Harmony. Tutorial material is secondary only: `https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/index.html`.
