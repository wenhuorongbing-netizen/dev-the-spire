# M5 Revision M Patch Failure Ledger

Date: 2026-06-11
Source evidence: `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/godot.log.after-launch`

## Red beta.84 Off Smoke Failures

| Patch id | Runtime log failure | Current source disposition |
|---|---|---|
| `brightest-flame-keywords` | Lines 104-106: target method not found: `CardModel.get_CanonicalKeywords` | Current source targets `nameof(CardModel.CanonicalKeywords)` with `MethodType.Getter`. |
| `brightest-flame-vars` | Lines 108-110: target method not found: `BrightestFlame.get_CanonicalVars` | Current source targets `CanonicalVars` with `MethodType.Getter`. |
| `debt-keywords` | Lines 112-114: target method not found: `Debt.get_CanonicalKeywords` | Current source targets `nameof(Debt.CanonicalKeywords)` with `MethodType.Getter`. |
| `debt-turn-end-effect` | Lines 116-118: target method not found: `Debt.get_HasTurnEndInHandEffect` | Current source targets `nameof(Debt.HasTurnEndInHandEffect)` with `MethodType.Getter`. |
| `debt-vars` | Lines 120-122: target method not found: `Debt.get_CanonicalVars` | Current source targets `CanonicalVars` with `MethodType.Getter`. |
| `distinguished-cape-event-option` | Lines 124-126: no valid patch methods found in `DistinguishedCapeEventOptionPatch` | Current source unchanged; beta84/current-source getter smoke later applied 25/25 patches, so keep this under runtime replay watch. |
| `distinguished-cape-vars` | Lines 128-130: target method not found: `DistinguishedCape.get_CanonicalVars` | Current source targets `CanonicalVars` with `MethodType.Getter`. |
| `fiddle-vars` | Lines 132-134: target method not found: `Fiddle.get_CanonicalVars` | Current source targets `CanonicalVars` with `MethodType.Getter`. |

## Harmony Initializer Failure

| Patch | Runtime log failure | Current source disposition |
|---|---|---|
| `EctoplasmGoldGatePatch` | Lines 140-142: `Undefined target method for patch method ... Prefix(Ectoplasm __instance, Player player, Boolean& __result)` | Current source targets `Ectoplasm.ModifyGoldGained` and prefixes `Prefix(Ectoplasm __instance, Player player, decimal amount, ref decimal __result)`. |

## Evidence Context

- Installed game DLL audit found the `v0.107.0` API includes the property accessors and `Ectoplasm.ModifyGoldGained(Player, decimal)`.
- `.tools/runtime-evidence/v01070-current-source-getter-targets-20260610-1000/godot.log.after-launch` later applied 25/25 patches with a clean audit, but that log still identifies Spire Plus as beta.84.
- `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/godot.log.after-launch` is the current package proof: Spire Plus `v0.1.0-private-beta.87`, RitsuLib compat branch `0.107.0`, 25/25 patches, clean audit, and 10 event types / 14 registration calls. Retained beta.85/beta.86 loader proof remains previous-package context.
- The red beta.84 smoke reached main menu at lines 350-351, so this was an API/patch-cleanliness failure, not a startup crash.
- Required next proof is no longer loader smoke; run gameplay, save-load, render, replacement, multiplayer, and QA checks only if process coordination is clear.
