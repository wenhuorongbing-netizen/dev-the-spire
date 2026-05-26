# Ascension 11-20

This folder tracks the active Ascension 11-20 development line for `Spire Plus` (`EZMicroBalance` manifest id).

Ascension 11-20 is available by default for single-player testing, but it is not release-verified. After the 2026-05-25 crash logs, host-multiplayer A11-A20 selection/gameplay fails closed by default unless `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY=1` is deliberately set for two-client debugging. Limited A11 map spot checks and targeted A14 Rootblight English/ZHS hover/starter-notice spot checks have evidence; Rootblight combat-end notices are source-hardened with an overlay path, but full live single-player, save/load, clean combat-end behavior, Blight Sprout notice timing, and co-op verification remain pending. Ascension 21-30 and custom character work are out of scope.

## Current Entry Points

| File | Purpose |
| --- | --- |
| `development-checklist-v2.md` | Compact current checklist. Full v2.0 design draft is archived under `docs/archive/feature-inputs/ascension-11-20/`. |
| `source-design.md` | Behavior design and scope boundaries. |
| `api-research.md` | Local v0.106.0 source evidence, safe APIs, and risky patch points. |
| `implementation-plan.md` | Implementation phases and safety strategy. |
| `manual-test-checklist.md` | Manual single-player and feature verification checklist. |
| `multiplayer-test-runbook.md` | Two-PC multiplayer setup, test matrix, and log checks. |
| `localization-review-notes.md` | Localization review notes for Ascension strings. |
| `work-log.md` | Compact active summary. Long chronological history is archived under `docs/archive/feature-work-logs/ascension-11-20/`. |

Historical prompt/spec material lives in `docs/archive/feature-inputs/ascension-11-20/`.

## Active Code Areas

| Code Area | Responsibility |
| --- | --- |
| `EZMicroBalanceCode/Ascension/Core/AscensionFeatureGate.cs` | Environment gates and public/multiplayer disable switches. |
| `EZMicroBalanceCode/Ascension/Core/AscensionInitializer.cs` | Feature registration and hook/model lookup. |
| `EZMicroBalanceCode/Ascension/Core/AscensionSavedStateFields.cs` | Saved run fields for Ascension systems. |
| `EZMicroBalanceCode/Ascension/Core/MultiplayerDiagnostics*.cs` | Optional multiplayer diagnostics only. |
| `EZMicroBalanceCode/Ascension/Patches/AscensionSelectionPatches.cs` and `AscensionSelectionRunStartPatches.cs` | A11-A20 lobby selection exposure and run-start gating. |
| `EZMicroBalanceCode/Ascension/Patches/AscensionA20Patches.cs` | A20 boss path/courtyard hooks. |
| `EZMicroBalanceCode/Ascension/Map/AscensionMapService*.cs` | A11 map extension, A17 deep branches, A20 boss path metadata. |
| `EZMicroBalanceCode/Ascension/Map/AscensionMapService.MarkerSelection.cs` | Stable map marker ordering, Firemarked Elite spacing, and kind assignment. |
| `EZMicroBalanceCode/Ascension/Map/AscensionMapService.MapGraphHelpers.cs` | Shared map reachability, rest-row, and route-safety helpers. |
| `EZMicroBalanceCode/Ascension/Combat/AscensionCombatModifierService.cs` | Combat-time Ascension modifier entrypoints and node metadata refresh. |
| `EZMicroBalanceCode/Ascension/Combat/AscensionCombatModifierService.Banners*.cs` | A16 Banner Room dispatch, single-enemy fallback, per-banner combat behavior, rewards, and per-turn Banner state. |
| `EZMicroBalanceCode/Ascension/Combat/AscensionCombatModifierService.BossSeals*.cs` | A19/A20 boss dedicated ability and Branded Form lifecycle dispatch, turn flow, combat events, and effect groups. |
| `EZMicroBalanceCode/Ascension/Combat/AscensionCombatModifierService.BossSeals.HolyDaze.cs`, `.MartyrOath.cs`, `.InkReturn.cs`, and `.StartledShell.cs` | A19/A20 monster-specific dedicated ability windows and enemy pressure effects. |
| `EZMicroBalanceCode/Ascension/Combat/AscensionCombatModifierService.BossSeals.MarginalNote.cs`, `.MisalignedShell.cs`, and `.StruggleBait.cs` | A19/A20 card-pressure dedicated ability effects. |
| `EZMicroBalanceCode/Ascension/Combat/AscensionCombatModifierService.BossSeals.ResidualSample*.cs` and `.A20Courtyard.cs` | A19/A20 Residual Sample phase carryover, visible sample notice, and courtyard recovery effects. |
| `EZMicroBalanceCode/Ascension/Combat/AscensionCombatModifierService.Firemarks*.cs` | A12 Firemarked Elite host selection, per-Firemark rules, counterplay windows, and turn/damage handling. |
| `EZMicroBalanceCode/Ascension/Combat/AscensionCombatModifierService.Helpers.cs` | Shared Ascension combat activation checks, act-scaling values, enemy filters, and command helpers. |
| `EZMicroBalanceCode/Ascension/Rewards/AscensionRewardService.cs` | Reward and room payout helpers. |
| `EZMicroBalanceCode/Ascension/Rewards/RootDeckService*.cs` | A14-A18 Rootblight deck state, Rootblight lifecycle, combat-end growth, deck-removal reactions, and notices. |
| `EZMicroBalanceCode/Ascension/Enchantments/FissionEnchantment.cs` | Fission card enchantment. |

## Safety Rules

- Keep A11-A20 single-player selection disableable with `SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1`.
- Keep host-multiplayer A11-A20 selection/gameplay fail-closed by default; use `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY=1` only for deliberate two-client debugging.
- Keep host-multiplayer selection separately disableable with `SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`.
- Do not claim live Ascension readiness until manual single-player and co-op checks pass.
- Do not implement A21-A30 or custom character systems in this cycle.
- Prefer current local `source code/src/Core/` evidence over old notes.
- Avoid hard references to optional Early Access boss or power types unless current runtime evidence proves they are stable.
