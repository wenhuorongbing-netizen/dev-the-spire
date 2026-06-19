# M5 Revision N Runtime Evidence Plan

Date: 2026-06-19
Status: Planned. Current beta.88 loader/registration proof exists; gameplay and release evidence remain pending.

## Current Loader Truth

Current clean loader/registration proof is beta.88 on Slay the Spire 2 `v0.107.1`:

```text
.tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/
```

Recorded scope:

- BaseLib `v3.3.0`
- STS2-RitsuLib `v0.4.24`
- Spire Plus `v0.1.0-private-beta.88`
- RitsuLib selected compat branch `0.107.0`
- 25/25 Spire Plus ModPatcher patches applied
- AdditiveBatch1 registered 10 event types through 14 calls
- main menu reached
- clean Godot log audit
- retained enabled-mode verifier 31 / 0
- runtime packet verifier 0 mismatches

This is loader and registration evidence only.

## Evidence That Must Not Be Overclaimed

- beta.85 Off and CanaryOnly logs are previous-package/game-version context.
- beta.87 AdditiveBatch1 direct proof is previous-game-version context.
- beta.87 `v0.107.1` AdditiveBatch1 attempt is failed BaseLib `v3.2.1` root-cause evidence.
- beta.88 AdditiveBatch1 proof does not prove gameplay, UI rendering, save-load, replacement behavior, multiplayer disposition, game-native AutoSlay batch stability, independent QA, release readiness, or tester handoff.

## Next Runtime Rows

Run only after the coordination pause is lifted and a single runtime lane is assigned.

1. Mod Settings current display:
   - normal Steam-client path;
   - foreground Mods list screenshot;
   - foreground Spire Plus config-page screenshot;
   - same-session `godot.log`;
   - clean `godot-log-audit.json`;
   - filled checklist row.
2. CanaryOnly current recapture before any canary gameplay claim:
   - current beta.88 package;
   - BaseLib `v3.3.0`;
   - RitsuLib `v0.4.24`;
   - expected 4 canary event types through 6 registration calls.
3. AdditiveBatch1 gameplay:
   - start from the clean beta.88 loader packet;
   - capture event encounter, EN/ZHS render, options, reward/effect, no-softlock exit, save-load, and screenshots.
4. Game-native AutoSlay / runtime monkey:
   - use only current schema-bound packets;
   - require `autoslay-plan.json` and `autoslay-summary.json` `SchemaVersion: 1`;
   - require `ExpectedAncientIds` in proof mode;
   - require sidecar and current-iteration log traversal for each expected Ancient id.
5. Co-op/fail-closed:
   - use the multiplayer runbook;
   - do not enable unverified co-op gameplay unless it is a deliberate owner-approved two-client debug run.
6. Release handoff:
   - collect release evidence with the verifier-readable manifest;
   - close or owner-defer every live row before any release-ready claim.

## Hard Stops

- No runtime/game smoke while the same-repo coordination pause is active.
- No debug expansion.
- No StS1 event formalization from loader proof alone.
- No Batch 4c migration without owner approval.
- No release-ready or live-ready claim from loader logs.
- No commit or push before coordinated validation replay and owner authorization.
