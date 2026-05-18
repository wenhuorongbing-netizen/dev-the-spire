# A11-A20 Development Checklist

Status: compact active checklist. The full v2.0 planning document was archived to `docs/archive/feature-inputs/ascension-11-20/development-checklist-v2-full-20260518.md`.

Use this file for current development triage. Use the archived full draft only when you need old design rationale.

## Scope

- A11-A20 remain the current private-beta high-Ascension test surface.
- A11-A20 selection is default-on for the private-beta test candidate.
- `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` restores vanilla A1-A10 public selection.
- `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` disables only host-multiplayer A11-A20 selection.
- A21-A30 and custom characters are out of scope.
- Do not claim release readiness until live single-player, save-load, and co-op evidence exists.

## Current Systems

| Level | System | Current source boundary | Pending proof |
| --- | --- | --- | --- |
| A11 | Wide Tower, Long Road | Map width +1 and route rows by act. Source guards require an optional inserted-column path and a preserved route that avoids it. | Natural click-by-click traversal, save-load, and co-op. |
| A12 | Firemarked Elites | Firemark Elite candidates, rewards, and counterplay windows are source-backed. | Live combat UI timing and reward clarity. |
| A13 | Fission Enchantment | Reward cards may gain cost -1, Exhaust, and Fissure residue behavior. | Live reward readability and combat behavior. |
| A14 | Rootblight Begins | Rootblight/Blight Sprout v2.2 source path with the 4-card Rootblight cap. | Combat-end notices, visual timing, save-load. |
| A15 | Boss Blight Sprout | Act 2/3 Boss fights bury two Blight Sprouts on staggered turns. | Live Boss timing and hover clarity. |
| A16 | Banner Rooms | Banner logic is split into its own combat partial with single/multi-enemy rules and rewards. | Single-enemy and multi-enemy live proof. |
| A17 | Deep Branches | Acts 2/3 get optional high-risk route branches when map geometry is safe. | Live route-click and reward proof. |
| A18 | Elite Blight Sprout | Mid/late Act 2 and Act 3 Elites bury a Blight Sprout. | Live Elite combat proof. |
| A19 | Boss Royal Seals | Boss Seal lifecycle and effect groups are split by responsibility. | Boss-specific live combat proof. |
| A20 | Dual King Brands | Single-player double-Boss path, Boss 2 Brand metadata, Boss 1 recovery, card reward, and fixed courtyard are source-backed. | Final Act live proof; co-op remains downgraded/unverified. |

## Current Refactor Notes

- `AscensionCombatModifierService.cs` keeps lifecycle dispatch and shared modifier entrypoints.
- Banner, Firemark, Boss Seal, Boss card-pressure, Boss monster-window, and Boss phase-carryover logic live in separate partial files.
- `AscensionMapService.cs` keeps entry flow and metadata lookup.
- A11 geometry, A17 branches, and map marker assignment live in separate map partial files.
- Future refactor cuts should move only one behavior group at a time and preserve serialized state layouts.

## Manual Gates

- A11 map width/row proof through normal route clicking.
- A12 Firemark Elite counterplay windows and map/event wording.
- A14-A18 Rootblight growth, notices, visuals, and save-load.
- A16 Banner single-enemy and multi-enemy cases.
- A17 Deep Branch routing and reward payout.
- A19/A20 Boss Seal and King Brand behavior.
- Host multiplayer A20 warning and downgraded second-Boss behavior.
- Full co-op ownership/desync verification.

## No-Game Validation

After code/config changes:

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

After package/resource changes:

```powershell
dotnet publish EZMicroBalance.sln
.\scripts\package-spire-plus.ps1
$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build
Remove-Item Env:\EZMB_RUN_RELEASE_ARTIFACT_TESTS
```

These commands do not prove live gameplay, save-load, death/failure paths, or co-op behavior.
