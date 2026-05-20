# Future Peek

`Future Peek` is an independent mod idea. It must stay outside `Spire Plus` / `EZMicroBalance`.

## Current Target

- Project id: `EZFuturePeek`
- Display name: `Future Peek`
- Dependencies: BaseLib and Harmony
- Not used: RitsuLib
- First test scope: Crystal Sphere peek and deterministic transform preview
- Gameplay flag: `affects_gameplay=false` while the mod only reveals already-determined information and does not lock or replace results
- Resource package: `has_pck=true` so the config and button localization files load through the normal mod resource path

## Source Rules

- Do not add Future Peek patches to `EZMicroBalanceCode/`.
- Crystal Sphere peek may only change UI visibility, currently by changing `%ScryMask` alpha.
- Crystal Sphere peek must not call `ClearCell`, `RevealItem`, `CellClicked`, or reward-granting paths.
- Transform preview may copy `PlayerRng.Transformations` and choose a `CardModel`.
- Transform preview must not call `CardTransformation.GetReplacement`, `CardFactory.CreateRandomCardForTransform`, `RunState.CreateCard`, or `CombatState.CreateCard`.

## Current State

The first source slice adds:

- `EZFuturePeek.csproj`
- `EZFuturePeek.json`
- `EZFuturePeekCode/`
- `EZFuturePeek/localization/`
- `tests/EZFuturePeek.Tests/`

This is source/test ready only. It is not live-proven. Future Peek publishes through `scripts/export-future-peek.ps1`, which builds a temporary Godot project under `.godot/` so the PCK contains Future Peek resources without pulling in Spire Plus assets from the root project.

## Manual Test Checklist

- Open Crystal Sphere and confirm a `Peek` / `预知` button appears.
- Toggle the button; hidden item icons should become readable through the mask.
- Toggle it off; the mask should return to its original opacity.
- Confirm peeking does not spend divination charges or grant rewards.
- Open a transform preview; the right-side card should stop cycling and show a fixed predicted card.
- Confirm the actual transform result matches the preview.
- Cancel and reopen transform preview; previewing alone should not advance the transform RNG.
- Test single-card, multi-card, combat, and non-combat transforms.
