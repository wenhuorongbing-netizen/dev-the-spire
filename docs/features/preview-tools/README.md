# Spire Plus Preview Tools

Preview tools are part of the single `Spire Plus / EZMicroBalance` mod.

## Current Target

- Manifest id: `EZMicroBalance`
- Display name: `Spire Plus`
- Source path: `EZMicroBalanceCode/Preview/`
- Settings path: `EZMicroBalance/localization/*/settings_ui.json`
- First test scope: Crystal Sphere peek and deterministic transform preview
- Product decision: these tools create an information advantage, so they ship inside the gameplay-affecting `Spire Plus` manifest instead of a separate non-gameplay helper mod.
- Multiplayer fairness risk: preview tools are not advertised as multiplayer-safe until live two-client evidence proves they do not create desync or hidden-state disagreement.

## Source Rules

- Crystal Sphere peek may only change UI visibility, currently by changing `%ScryMask` alpha.
- Crystal Sphere peek must not call `ClearCell`, `RevealItem`, `CellClicked`, or reward-granting paths.
- Transform preview may use a copied RNG snapshot and choose a `CardModel`.
- Transform preview must not call `CardTransformation.GetReplacement`, `CardFactory.CreateRandomCardForTransform`, `RunState.CreateCard`, or `CombatState.CreateCard`.
- Preview code stays in `EZMicroBalanceCode/Preview/` so the player sees one mod while source ownership remains clear.

## Manual Test Checklist

- Open Crystal Sphere and confirm a `Peek` / `预知` button appears.
- Toggle the button; hidden item icons should become readable through the mask.
- Toggle it off; the mask should return to its original opacity.
- Confirm peeking does not spend divination charges or grant rewards.
- Open a transform preview; the right-side card should stop cycling and show a fixed predicted card.
- Confirm the actual transform result matches the preview.
- Cancel and reopen transform preview; previewing alone should not advance the transform RNG.
- Test single-card, multi-card, combat, and non-combat transforms.

Live verification remains pending until these rows are captured in runtime evidence.
