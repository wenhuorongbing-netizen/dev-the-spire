# Ascension 11-20

This folder tracks the active Ascension 11-20 development line for `Spire Plus` (`EZMicroBalance` manifest id).

Ascension 11-20 is available by default in the current private-beta multiplayer test candidate, but it is not release-verified. Limited A11 map spot checks and targeted A14 Rootblight English/ZHS hover/starter-notice spot checks have evidence; Rootblight combat-end notices are source-hardened with an overlay path, but full live single-player, save/load, clean combat-end behavior, Blight Sprout notice timing, and co-op verification remain pending. Ascension 21-30 and custom character work are out of scope.

## Current Entry Points

| File | Purpose |
| --- | --- |
| `development-checklist-v2.md` | Current feature checklist and forward-looking design map. |
| `source-design.md` | Behavior design and scope boundaries. |
| `api-research.md` | Local v0.105.0 source evidence, safe APIs, and risky patch points. |
| `implementation-plan.md` | Implementation phases and safety strategy. |
| `manual-test-checklist.md` | Manual single-player and feature verification checklist. |
| `multiplayer-test-runbook.md` | Two-PC multiplayer setup, test matrix, and log checks. |
| `localization-review-notes.md` | Localization review notes for Ascension strings. |
| `work-log.md` | Chronological implementation and validation history. Older entries may be superseded. |

Historical prompt/spec material lives in `archive/`.

## Active Code Areas

| Code Area | Responsibility |
| --- | --- |
| `EZMicroBalanceCode/Ascension/Core/AscensionFeatureGate.cs` | Environment gates and public/multiplayer disable switches. |
| `EZMicroBalanceCode/Ascension/Core/AscensionInitializer.cs` | Feature registration and hook/model lookup. |
| `EZMicroBalanceCode/Ascension/Core/AscensionSavedStateFields.cs` | Saved run fields for Ascension systems. |
| `EZMicroBalanceCode/Ascension/Core/MultiplayerDiagnostics.cs` | Optional multiplayer diagnostics only. |
| `EZMicroBalanceCode/Ascension/Patches/AscensionSelectionPatches.cs` | A11-A20 lobby selection exposure. |
| `EZMicroBalanceCode/Ascension/Patches/AscensionA20Patches.cs` | A20 boss path/courtyard hooks. |
| `EZMicroBalanceCode/Ascension/Map/AscensionMapService.cs` | A11 map extension, A17 deep branches, A20 boss path metadata. |
| `EZMicroBalanceCode/Ascension/Combat/AscensionCombatModifierService.cs` | Combat-time Ascension modifiers and Boss Seal effects. |
| `EZMicroBalanceCode/Ascension/Rewards/AscensionRewardService.cs` | Reward and room payout helpers. |
| `EZMicroBalanceCode/Ascension/Enchantments/FissionEnchantment.cs` | Fission card enchantment. |

## Safety Rules

- Keep A11-A20 disableable with `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1`.
- Keep host-multiplayer selection separately disableable with `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`.
- Do not claim live Ascension readiness until manual single-player and co-op checks pass.
- Do not implement A21-A30 or custom character systems in this cycle.
- Prefer current local `source code/src/Core/` evidence over old notes.
- Avoid hard references to optional Early Access boss or power types unless current runtime evidence proves they are stable.
