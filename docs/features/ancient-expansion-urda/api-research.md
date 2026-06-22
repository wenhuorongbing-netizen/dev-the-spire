# Urda Ancient API Research

Last updated: 2026-06-20

Current supersession note: the original May Urda research used previous package as the active template dependency. The current Spire Plus migration target is RitsuLib-only: `EZMicroBalance.csproj` references `STS2.RitsuLib` `0.4.34`, `EZMicroBalance.json` depends only on `STS2-RitsuLib >= 0.4.34`, and previous package is previous-package/other-mod context only. Reinspect local `source code/` and the installed RitsuLib package before changing Urda behavior.

## 1. Current evidence set

### 1.1 Source areas already inspected

- `source code/src/Core/Models/Ancients/`
- `source code/src/Core/Models/Events/`
- `source code/src/Core/Models/Maps/`
- `source code/src/Core/Commands/`
- `source code/src/Core/Models/Rewards/`

### 1.2 Existing related APIs from active mod

Relevant active paths already used in `EZMicroBalanceCode/Ancients`:

- `RelicModel.AfterObtained()` for relic-style reward replacement.
- `CardPileCmd.Add(...)` and `CardPileCmd.AddGeneratedCardToCombat(...)`.
- `CardCmd.Upgrade(...)`, `CardCmd.Exhaust(...)`.
- `CardSelectCmd.FromChooseACardScreen(...)`.
- `CardSelectCmd.FromBundleScreen(...)`.
- `PotionFactory`, `RelicFactory`, and `PotionCmd` reward helpers.
- `RelicCmd.Obtain(...)`, `PlayerCmd.GainGold(...)`.
- RitsuLib `SavedAttachedState<TKey,TValue>` for run/card/owner state.

### 1.3 Ancient registration status

The current local evidence indicates two families of ancient patterns:

- Direct patching of existing ancient generation flow (`AncientEventModel` and option lists).
- Historical custom ancient/model registration support exposed in previous package tutorials; current implementation should prefer local game source plus installed RitsuLib APIs where they cover the needed hook or registration shape.

Current risk:

- `CustomAncientModel`, `AncientOption<T>()`, `OptionPools`, and `MakePool(...)` support should be revalidated against local `v0.107.1` runtime signatures before finalizing Urda registration code.
- If registration is unsafe, temporary diagnostics/test-force path should remain default-off and documented.

## 2. Planned evidence questions before implementation

The table below is required before release-ready claims.

### 2.1 Registration proof

Need proof for:

- where to attach Urda into Act 1 options,
- whether Act 1 old/new surface uses the same hook as existing ancient selection,
- whether legacy custom ancient registration works without unsafe reflection.

### 2.2 Blessing hook proof

Source-backed implementation now uses:

- `AbstractModel.TryModifyCardRewardOptionsLate(...)` plus the active `CardReward.Populate` context to identify normal Act 1 combat card rewards.
- `AbstractModel.TryModifyCardRewardAlternatives(...)` and `CardRewardAlternative` for the Seedbed and Humus Pact reward-screen options.
- `AbstractModel.AfterRewardTaken(...)` for Humus Pact's third-trigger remove/payoff sequence after the card reward selection has completed.
- `AbstractModel.AfterRoomEntered(...)` for Moss Map first-room-type rewards.
- `AbstractModel.AfterActEntered(...)` for Molting cleanup at Act 2+ start.
- `CardRewardAlternative.Generate(...)` supports at most two alternatives total, so Urda alternatives must no-op when built-in or other mod alternatives already fill both slots.
- `CardReward.OnSkipped(...)` can run during reward-set abandonment or room-exit cleanup; Humus Pact must not open UI from this path.
- `Reward.SelectUnsynchronized(...)` calls `Hook.AfterRewardTaken(...)` after a reward's `OnSelect()` succeeds, then sets `SuccessfullySelected`. `RewardsSetSynchronizer.SelectRewardForPlayer(...)` completes the current reward set after that call returns. Humus Pact's third payoff therefore still needs source guards against reentry/loss even though it no longer runs from `OnSkipped`.
- `CreatureCmd.LoseMaxHp(...)` can damage before max HP clamping, so Seedbed must require max HP greater than its cost before offering or accepting.
- `CreatureCmd.GainMaxHp(...)` heals by the gained amount; Seedbed uses `SetMaxHp(...)` for its no-heal completion bonus.
- Humus Pact now generates the upgraded payoff card before opening optional deck removal, and clears `HumusCompletionPending` only after the payoff resolver succeeds. This keeps the third payoff from being marked complete if reward-card generation cannot produce a card.
- Historical previous package `previous saved-state API<TKey,TValue>` documentation said automatic save/load only worked on model types with saved properties, mainly cards and relics. Current RitsuLib `SavedAttachedState<TKey,TValue>` replaced that API, but local Core source still does not prove player-field persistence without live save/load evidence:
  - `Player.ToSerializable()` writes a fixed `SerializablePlayer` shape and does not call `SavedProperties.From(...)`.
  - `SerializablePlayer` has fixed fields such as deck, relics, potions, rng, odds, and `extra_fields`; it has no general `SavedProperties`/`Props` field.
  - `ExtraPlayerFields` serializes only built-in fixed fields.
  - `SavedProperties.From(AbstractModel)` is used by card, relic, and modifier save paths, but no inspected `Player` save path routes through it.
  Current Urda player-owned state is mirrored onto deck cards through `AncientSavedStateFields.UrdaDeckStateKey`, but save/load must remain pending until live evidence proves the Player field or card-backed recovery path survives reload.

These paths compile and are covered by source-guard tests, but they are not a substitute for live reward-screen, save/load, and UI checks.

### 2.3 Runtime proof

Need proof for:

- save/load of selected Urda blessing and counters,
- reroll behavior when reward screens rebuild,
- remove/upgrade command safety for `Withered Husk` lifecycle.

## 3. State strategy

Current design uses two layers:

- owner state: player-bound blessing selection and counters,
- blessing state: pool activation, transformation, and one-time rewards.

Preferred state strategy:

1. store Urda selection in blessing fields,
2. store per-blessing counters in stable save fields,
3. resolve UI strings from state only after safe null checks.

## 4. Command safety rule

Avoid direct mutable state edits where game command APIs exist.

- Card add/remove operations should use `CardPileCmd` and `CardSelectCmd`.
- Relic grant operations should use `RelicCmd.Obtain`.
- Gold and HP mutations should use the established command path, then verify if command path is absent.
- Act transition cleanup should use combat-start or act-start hooks, not direct deck scans.

## 5. Current blockers

Known unresolved items:

- live Act 1 Urda selection and registration proof,
- reward-screen timing for Seedbed and Humus Pact in normal Steam-client play,
- save/load of encoded Urda blessing progress in `UrdaStateKey` plus recovery from card-backed `UrdaDeckStateKey`,
- full room-type callback stability for Moss Map,
- safe `Withered Husk` end-of-turn exhaust block behavior and end-act cleanup ordering.

Source gameplay is implemented, but no live game behavior is claimed verified in this doc. Evidence must be collected before release claims.
