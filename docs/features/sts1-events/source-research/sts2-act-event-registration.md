# StS2 Act Event Registration — Source Evidence

Created: 2026-05-29 | Status: source-verified

## 2026-06-18 Revision M Current Boundary

This source-research note documents current intended StS1 registration shape and static source evidence. It is not current `v0.107.1` gameplay proof. Previous beta.93 proves only RitsuLib-only Off and AdditiveBatch1 loader/registration behavior; beta.85/beta.87/beta.88/beta.90 rows remain previous-context evidence. CanaryOnly gameplay/runtime, save-load, replacement, multiplayer, QA, handoff, and release-ready proof still require fresh current evidence.

## Act Mapping (Verified)

StS1 act mapping to StS2 act models, verified from Ancient service Harmony patches:

| StS1 Act | StS2 Act Model(s) | Evidence |
| --- | --- | --- |
| Act 1 | `Overgrowth` + `Underdocks` | `UrdaAct1AncientService.cs:50` patches `Overgrowth.GetUnlockedAncients`; `UrdaAct1AncientService.cs:58` patches `Underdocks.GetUnlockedAncients`. |
| Act 2 | `Hive` | `MorviAct2AncientService.cs:49` patches `Hive.GetUnlockedAncients`. |
| Act 3 | `Glory` | `LothaAct3AncientService.cs:49` patches `Glory.GetUnlockedAncients`. |

### Why Both Overgrowth and Underdocks for Act 1?

StS2 splits Act 1 into two sub-areas (`Overgrowth` early, `Underdocks` late). StS1 Act 1 events should appear in both sub-areas to match the original game's "any unknown room in Act 1" behavior.

### Registration API

RitsuLib provides two registration methods:

```csharp
// Shared event — appears in all acts
content.SharedEvent<Sts1TheLab>();

// Act-specific event — appears only in the specified act
content.ActEvent<Overgrowth, Sts1BigFish>();
content.ActEvent<Underdocks, Sts1BigFish>();
content.ActEvent<Overgrowth, Sts1TheCleric>();
content.ActEvent<Underdocks, Sts1TheCleric>();
content.ActEvent<Overgrowth, Sts1ShiningLight>();
content.ActEvent<Underdocks, Sts1ShiningLight>();
```

### Source Evidence Files

| File | Line | Evidence |
| --- | --- | --- |
| `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaAct1AncientService.cs` | 50 | `[HarmonyPatch(typeof(Overgrowth), nameof(Overgrowth.GetUnlockedAncients))]` |
| `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaAct1AncientService.cs` | 58 | `[HarmonyPatch(typeof(Underdocks), nameof(Underdocks.GetUnlockedAncients))]` |
| `EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviAct2AncientService.cs` | 49 | `[HarmonyPatch(typeof(Hive), nameof(Hive.GetUnlockedAncients))]` |
| `EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaAct3AncientService.cs` | 49 | `[HarmonyPatch(typeof(Glory), nameof(Glory.GetUnlockedAncients))]` |
| `EZMicroBalanceCode/Sts1Events/Runtime/Sts1EventRegistrationService.cs` | — | `RegisterAll()` uses `ActEvent<Overgrowth, T>()`, `ActEvent<Underdocks, T>()`, `ActEvent<Hive, T>()`, `ActEvent<Glory, T>()`. |

### Event Registration Counts

| Category | Count | Registration Method |
| --- | --- | --- |
| Shared events | 14 registered (+1 compile-excluded model) | `content.SharedEvent<T>()` |
| Act 1 events (per act) | 10 | `content.ActEvent<Overgrowth, T>()` + `content.ActEvent<Underdocks, T>()` |
| Act 2 events | 14 | `content.ActEvent<Hive, T>()` |
| Act 3 events | 9 | `content.ActEvent<Glory, T>()` |
| **RegisterAll registration calls** | **57** | (14 shared x 1 + 10 act1 x 2 + 14 act2 x 1 + 9 act3 x 1) |
| **AdditiveBatch1 registration calls** | **14** | 10 verified-scope event types; Big Fish, Golden Idol, The Cleric, and Shining Light register to both Overgrowth and Underdocks |
| **Excluded** | 1 | `Sts1Duplicator` uses duplicate-selection APIs unavailable in the current game/RitsuLib API surface |

### Canary Mode Registration

`CanaryOnly` mode registers exactly 4 event types through 6 registration calls:

1. `Sts1BigFish` — `sts1_big_fish` in Overgrowth + Underdocks
2. `Sts1GoldenIdol` — `sts1_golden_idol` in Overgrowth + Underdocks
3. `Sts1TheLab` — `sts1_the_lab`
4. `Sts1DivineFountain` — `sts1_divine_fountain`

Big Fish and Golden Idol are Act 1 bucket registrations. The Lab and Divine Fountain remain shared registrations. Verified in `Sts1EventRegistrationService.RegisterCanaryOnly()`.
