# StS2 Act Event Registration — Source Evidence

Created: 2026-05-29 | Status: source-verified

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
content.SharedEvent<Sts1BigFish>();

// Act-specific event — appears only in the specified act
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
| Shared events | 15 (+1 excluded) | `content.SharedEvent<T>()` |
| Act 1 events (per act) | 7 | `content.ActEvent<Overgrowth, T>()` + `content.ActEvent<Underdocks, T>()` |
| Act 2 events | 14 | `content.ActEvent<Hive, T>()` |
| Act 3 events | 9 | `content.ActEvent<Glory, T>()` |
| **Total registration calls** | **52** | (15 shared × 1 + 7 act1 × 2 + 14 act2 × 1 + 9 act3 × 1) |
| **Excluded** | 1 | `Sts1Duplicator` — uses `CardSelectCmd`/`CardPileCmd` APIs not yet available |

### Canary Mode Registration

`CanaryOnly` mode registers exactly 4 shared events:

1. `Sts1BigFish` — `sts1_big_fish`
2. `Sts1GoldenIdol` — `sts1_golden_idol`
3. `Sts1TheLab` — `sts1_the_lab`
4. `Sts1DivineFountain` — `sts1_divine_fountain`

All 4 are shared events (appear in all acts). Verified in `Sts1EventRegistrationService.RegisterCanaryOnly()`.
