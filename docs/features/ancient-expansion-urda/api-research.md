# Urda Ancient API Research

Last updated: 2026-05-12

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
- `SavedSpireField<TKey,TValue>` for run/card/owner state.

### 1.3 Ancient registration status

The current local evidence indicates two families of ancient patterns:

- Direct patching of existing ancient generation flow (`AncientEventModel` and option lists).
- Custom ancient/model registration support exposed in BaseLib tutorials.

Current risk:

- `CustomAncientModel`, `AncientOption<T>()`, `OptionPools`, and `MakePool(...)` support should be revalidated against local `v0.105.0` runtime signatures before finalizing Urda registration code.
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
- `AbstractModel.TryModifyCardRewardAlternatives(...)` and `CardRewardAlternative` for the Seedbed reward-screen option.
- A narrow `CardReward.OnSkipped` postfix to process Humus Pact after the card reward is finally skipped.
- `AbstractModel.AfterRoomEntered(...)` for Moss Map first-room-type rewards.
- `AbstractModel.AfterActEntered(...)` for Molting cleanup at Act 2+ start.

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
- save/load of encoded Urda blessing progress in `UrdaStateKey`,
- full room-type callback stability for Moss Map,
- safe `Withered Husk` end-of-turn exhaust block behavior and end-act cleanup ordering.

Source gameplay is implemented, but no live game behavior is claimed verified in this doc. Evidence must be collected before release claims.
