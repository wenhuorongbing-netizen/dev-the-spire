# Source Evidence - Multiplayer, Save, RNG

Purpose: record multiplayer/save/RNG boundaries that affect Spire Plus, Ascension, Ancient rewards, and preview tools.

## Vanilla Source Evidence

| Concern | Source path | Evidence |
| --- | --- | --- |
| Lobby start | `source code/src/Core/Multiplayer/Game/Lobby/StartRunLobby.cs` | `BeginRunLocally` builds acts from the shared seed and game type; `SyncAscensionChange` is the lobby ascension sync surface. |
| Lobby joins | `source code/src/Core/Multiplayer/JoinFlow.cs` | Initial game-info handling is a client hydration boundary; diagnostics may observe it but must not mutate run setup. |
| Net game type | `source code/src/Core/Multiplayer/Game/NetGameType.cs` | Game type controls multiplayer-specific branches. |
| Run save manager | `source code/src/Core/Saves/Managers/RunSaveManager.cs` | Save/load is mediated outside mod state; mod save fields must survive vanilla manager round trips. |
| Combat sync | `source code/src/Core/Multiplayer/CombatStateSynchronizer.cs` | Sync sends serialized players, run RNG, and shared relic bag; clients reload run RNG from the received snapshot. |
| Full combat state | `source code/src/Core/Entities/Multiplayer/NetFullCombatState.cs` | Serializes player RNG and run RNG counters; removes some reward/shop counters from per-player sync. |
| Reward sync | `source code/src/Core/Multiplayer/Game/RewardsSetSynchronizer.cs` | Local reward clicks are synchronized by reward context; mod alternatives must not rely on client-only transient objects. |
| Player RNG | `source code/src/Core/Random/PlayerRngSet.cs` | `Rewards`, `Shops`, and `Transformations` are separate counters; `LoadFromSerializable` can fast-forward/reset counters. |
| Run RNG | `source code/src/Core/Runs/RunRngSet.cs` | `UnknownMapPoint`, combat, treasure, and other run RNG counters are serialized independently. |
| Run state | `source code/src/Core/Runs/RunState.cs` | `RunState.Rng` and players are authoritative run data; preview systems should fork, not advance, these counters. |
| Unknown room odds | `source code/src/Core/Odds/UnknownMapPointOdds.cs` | Unknown room roll consumes `UnknownMapPoint` RNG after hook-modified room type candidates. |
| Click surfaces | `source code/src/Core/Nodes/GodotExtensions/NClickableControl.cs`, `source code/src/Core/Nodes/Map/NMapPoint.cs` | Map and reward clicks flow through Godot UI signals; click-intercept patches must not consume unrelated controls. |
| Transform preview UI | `source code/src/Core/Nodes/Cards/NTransformPreview.cs` | Vanilla preview cycles through candidates with `Rng.Chaotic`; it is not a real-result preview. |
| Crystal Sphere minigame | `source code/src/Core/Events/Custom/CrystalSphereEvent/CrystalSphereMinigame.cs` | `CellClicked`, `ClearCell`, `RevealItem`, and `AddReward` are gameplay mutation paths. |
| Crystal Sphere UI | `source code/src/Core/Nodes/Events/Custom/CrystalSphere/NCrystalSphereScreen.cs` | `%ScryMask` is the visual mask node; cell clicks call the minigame entity. |

## Mod Boundaries

| Feature | Mod source | Required boundary |
| --- | --- | --- |
| Root Eyes | `UrdaBlessingService.RootSight*.cs` | Preview generation must fork RNG for preview, then commit only when the marked node is actually entered. |
| Ascension selector | `AscensionSelectionPatches.cs`, `AscensionSelectionRunStartPatches.cs` | A11-A20 selection may expand local limits, but multiplayer downgrade and `SyncAscensionChange` warnings must stay explicit until two-client proof exists. |
| Ascension map markers | `AscensionMapService*.cs` | Map metadata must regenerate deterministically from run/act/coord, not from local-only UI state. |
| Banner and Firemark combat | `AscensionCombatModifierService*.cs` | Combat state must be owned by combat hooks and reset on combat lifecycle boundaries. |
| Morvi/Lotha state | `MorviBlessingService.State.cs`, `MorviBlessingService.CombatState.cs`, `LothaBlessingService.State.cs`, `LothaBlessingService.DeathReprieveState.cs` | Deck/player mirrors mitigate persistent state loss; transient combat state and Death Reprieve restore state remain lifecycle-owned and live save/load still required. |
| Crystal Sphere preview | `EZMicroBalanceCode/Preview/CrystalSpherePeekPatch.cs` | Only touch `%ScryMask` and local button state; never call `ClearCell`, `RevealItem`, `CellClicked`, or `AddReward`. |
| Transform preview | `EZMicroBalanceCode/Preview/TransformPreviewPatch.cs`, `TransformPredictionRngContext.cs`, `TransformPredictionEventRngSourcePatches.cs`, `TransformPredictionNicheRngSourcePatches.cs`, `TransformPredictionSelectionLifetimePatch.cs` | Use snapshot/forked transformation RNG; do not call real replacement creation paths for preview. |

## Desync Watchlist

- Gameplay state held only in `static` or `ConditionalWeakTable` without a saved/deck/run mirror.
- Map metadata that depends on local UI node creation order.
- Reward alternatives that depend on transient reward-screen objects across save/reopen.
- Preview systems that advance real RNG counters before the player commits.
- Co-op claims without two-client logs from the same package.

## Release Rule

A multiplayer claim requires two-client evidence. A save/load claim requires a live save/quit/load row for the same feature and package.
