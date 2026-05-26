# Spire Plus Preview Tools

Preview tools are part of the single `Spire Plus` mod.

## Current Target

- Manifest id: `EZMicroBalance`
- Display name: `Spire Plus`
- Source path: `EZMicroBalanceCode/Preview/`
- Settings path: `EZMicroBalance/localization/*/settings_ui.json`
- First test scope: Crystal Sphere peek and deterministic transform preview
- Product decision: these tools create an information advantage, so they ship inside the gameplay-affecting `Spire Plus` manifest instead of a separate non-gameplay helper mod.
- Multiplayer stance: Crystal Sphere peek and transform preview now run in co-op as local UI-only previews. They still are not release-certified for multiplayer until two-client evidence proves reconnect, save-load, and host/client display agreement.

## Source Rules

- Crystal Sphere peek may only change UI visibility, currently by changing `%ScryMask` alpha.
- Crystal Sphere peek must not call `ClearCell`, `RevealItem`, `CellClicked`, or reward-granting paths.
- Transform preview may use a copied RNG snapshot and choose a `CardModel`.
- Transform preview must not call `CardTransformation.GetReplacement`, `CardFactory.CreateRandomCardForTransform`, `RunState.CreateCard`, or `CombatState.CreateCard`.
- Co-op preview patches may log evidence, but they must not add `PlayerChoice`, `CardRewardAlternative`, rewards, or any real RNG advancement.
- Transform preview display must fail open to the vanilla cycling preview if any UI lifecycle error occurs.
- Preview code stays in `EZMicroBalanceCode/Preview/` so the player sees one mod while source ownership remains clear. `TransformPreviewPatch.cs` owns the Harmony patch flow; `TransformPreviewPredictionQueue.cs` owns per-preview prediction queue state.

## Future Peek Intake

`docs/archive/feature-inputs/future-peek-goal-20260526.md` is implemented for the existing UI-only preview tools:

- Crystal Sphere peek is enabled in co-op because it only changes the local mask alpha.
- Transform preview is enabled in co-op as a local preview card backed by a forked RNG snapshot.
- Display failures clear Spire Plus prediction state and return to vanilla preview cycling.

Map foresight and reward foresight are not implemented as part of this source pass. Those systems would change future room or reward results and need a separate host-authoritative or deterministic precommit plan before they can be multiplayer-safe.

## Manual Test Checklist

- Open Crystal Sphere and confirm a `Peek` / `预知` button appears.
- Toggle the button; hidden item icons should become readable through the mask.
- Toggle it off; the mask should return to its original opacity.
- Confirm peeking does not spend divination charges or grant rewards.
- Open a transform preview; the right-side card should stop cycling and show a fixed predicted card.
- Confirm the actual transform result matches the preview.
- Cancel and reopen transform preview; previewing alone should not advance the transform RNG.
- Test single-card, multi-card, combat, and non-combat transforms.
- In co-op, confirm both preview tools remain local UI-only and do not create extra choices, reward options, or desync.

Live verification remains pending until these rows are captured in runtime evidence.
