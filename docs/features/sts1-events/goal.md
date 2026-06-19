# StS1 Events Migration Goal

## Objective

Long-term target: account for the Slay the Spire 1 public event baseline inside
the Spire Plus mod (`EZMicroBalance`) and prove a playable StS1-like event
experience in Slay the Spire 2. Current source status is a gated prototype, not
runtime gameplay proof.

## Current Boundary (Revision N / beta.88)

This goal is not complete. Current beta.88 proof under `.tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/` covers AdditiveBatch1 loader/registration shape on Slay the Spire 2 `v0.107.1` with BaseLib `v3.3.0`: 25/25 Spire Plus patches, 10 event types / 14 registration calls, clean audit, retained enabled-mode verifier 31 / 0, and runtime packet verifier 0 mismatches. Retained beta.85 Off/CanaryOnly and beta.87 AdditiveBatch1 proof remain previous-package/game-version loader context only. These are loader/registration proofs only. They do not prove event gameplay, save-load, replacement-pool behavior, image/render, multiplayer, QA, build/publish handoff, game-native AutoSlay/monkey batch stability, or full parity. Historical `v0.106.1` CanaryOnly/AdditiveBatch1 loader evidence must stay historical, and the beta.85 AdditiveBatch1 13/14 mismatch plus beta.87 `v0.107.1` BaseLib `v3.2.1` failure remain root-cause history only.

## Success Criteria

- [ ] Public 52-event baseline is reconciled with the current source counts (`54` canonical rows, `50` registry identities, `48` model files, `47` compiling models)
- [ ] Each event has correct StS1 behavior (options, rewards, conditions)
- [ ] Ascension 15 differences are applied where applicable
- [ ] Events are registered through RitsuLib `content.ActEvent<TAct,TEvent>()` / `content.SharedEvent<TEvent>()`
- [ ] Localization exists in both English and Simplified Chinese
- [ ] Event images are extracted from local StS1 installation (not committed)
- [ ] StS1-only event pool replaces StS2 events in Unknown rooms
- [ ] All canary events pass debug-spawn testing
- [ ] Build succeeds with `dotnet build`
- [ ] Publish succeeds with `dotnet publish`

## Non-Goals

- Custom minigame UI (Match and Keep, Wheel of Change) — use simplified
  option-based fallbacks until custom UI is feasible
- Distributing original StS1 art assets — use local extraction scripts
- StS1 exact pixel-perfect UI reproduction — use StS2 event layout system

## Constraints

- Events must live inside `EZMicroBalanceCode/Sts1Events/` (no separate mod)
- Localization must live inside `EZMicroBalance/localization/`
- No original StS1 art committed to the repository
- Events must compile against the current repository package references: `STS2.RitsuLib` `0.4.24` and BaseLib `3.3.0`.
- Current runtime reproof targets the installed official `STS2-RitsuLib` `v0.4.24` variant pack with `lib\0.107.0`.
- Normal StS1 event models inherit `EventModel` and are registered through RitsuLib content-builder APIs.
  Ancient-style events remain outside the StS1 unknown-room prototype scope.
