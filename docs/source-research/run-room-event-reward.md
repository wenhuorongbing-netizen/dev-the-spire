# Source Evidence - Run, Room, Event, Reward

Purpose: source-backed boundary notes for high-risk Spire Plus room, event, reward, and child-combat behavior.

Do not copy large game source bodies into this repo. Record names, flow, and risks only.

## Vanilla Source Evidence

| Concern | Source path | Evidence |
| --- | --- | --- |
| Run setup and save hydration | `source code/src/Core/Runs/RunManager.cs` | `SetUpSavedSinglePlayer`, `SetUpSavedMultiPlayer`, `InitializeSavedRun`, `SavedMapsToLoad`. |
| Run save payload | `source code/src/Core/Runs/RunManager.cs` | `ToSave(AbstractRoom? preFinishedRoom)` serializes acts, players, run RNG, odds, shared relic bag, extra fields, and `PreFinishedRoom`. |
| Room entry | `source code/src/Core/Runs/RunManager.cs` | `EnterMapPointInternal`, `CreateRoom`, `RollRoomTypeFor`, `EnterRoomWithoutExitingCurrentRoom`. |
| Act room generation | `source code/src/Core/Runs/RunManager.cs`, `source code/src/Core/Models/ActModel.cs` | `RunManager.GenerateRooms` delegates to `ActModel.GenerateRooms`; A20 and A11 patches must preserve deterministic act maps. |
| Terminal reward exit | `source code/src/Core/Runs/RunManager.cs` | `ProceedFromTerminalRewardsScreen` is the normal post-reward transition; A20 courtyard and Vakuu no-reward resume patches must leave a valid next room/screen. |
| Pre-finished room restore | `source code/src/Core/Runs/RunManager.cs` | `LoadIntoLatestMapCoord` and pre-finished `ParentEventId` path restore prior event when valid. |
| Combat child event risk | `source code/src/Core/Rooms/CombatRoom.cs` | `ToSerializable` throws if `ParentEventId` is set on a non-prefinished combat room. |
| Event child combat helper | `source code/src/Core/Models/EventModel.cs` | `EnterCombatWithoutExitingEvent` sets `ShouldResumeParentEventAfterCombat` and `ParentEventId`, then enters a combat room. |
| Event resume | `source code/src/Core/Rooms/EventRoom.cs` | `Resume` delegates through event synchronizer; `ToSerializable` persists event room state. |
| Combat rewards | `source code/src/Core/Rooms/CombatRoom.cs` | `OfferRoomEndRewards` is the combat room reward boundary. |
| Reward synchronization | `source code/src/Core/Rewards/RewardsSet.cs`, `source code/src/Core/Multiplayer/Game/RewardsSetSynchronizer.cs` | Reward selection is routed through `RewardsSetSynchronizer.SelectLocalReward`; custom alternatives must keep stable reward ids and selection indexes. |
| Card reward alternatives | `source code/src/Core/Rewards/CardReward.cs` | `CardRewardAlternative.Generate`, selection index handling, and `ToSerializable` reject unsupported custom pools/filters/flags. |
| Relic rewards | `source code/src/Core/Rewards/RelicReward.cs` | `ToSerializable` persists reward model data and marks seen relics on pickup. |

## Mod Boundaries

| Feature | Mod source | Boundary decision | Runtime risk |
| --- | --- | --- | --- |
| Vakuu fight | `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs` | Avoid active `ParentEventId` on the child combat room; own custom resume/fallback logs. | Needs live victory/failure/save proof. |
| A20 dual boss/courtyard | `EZMicroBalanceCode/Ascension/Patches/AscensionA20Patches.cs`, `EZMicroBalanceCode/Ascension/Events/A20Courtyard.cs` | Patch only room generation and terminal reward transitions; courtyard entry must be deterministic and leave a legal act progression state. | Needs live A20 boss-chain and save proof. |
| Root Eyes | `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaRootSightRoomPatches.cs` | Patch `RollRoomTypeFor` and `CreateRoom` only to consume stored previews for the current point. | Needs live map click/hover/save proof. |
| Card reward alternatives | `Ancients/Patches`, `Urda`, `Morvi` | Use alternatives only where reward context is still available; avoid unsupported serialized custom reward pools. | Reward-screen save/load remains live-pending. |
| Seed Bank | `UrdaBlessingService.SeedBank*.cs` | Keep storage in player/deck mirrored state and expose extraction through relic interaction. | Boss transition and save/load need live proof. |

## Non-Closure Rule

Source evidence can justify code shape and tests. It cannot close:

- clicked Ancient UI;
- live Ancient reward gameplay;
- Vakuu victory/no-black-screen;
- failure/death paths;
- save/load rows;
- two-client co-op rows.
