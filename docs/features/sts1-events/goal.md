# StS1 Events Migration Goal

## Objective

Long-term target: account for the Slay the Spire 1 public event baseline inside
the Spire Plus mod (`EZMicroBalance`) and prove a playable StS1-like event
experience in Slay the Spire 2. Current source status is a gated prototype, not
runtime gameplay proof.

## Current Boundary (Revision P / beta.92)

This goal is not complete. Current beta.92 proof under `.tools/runtime-evidence/v01071-beta92-ritsulib0429-additivebatch1-direct-20260621/` covers AdditiveBatch1 loader/registration shape on Slay the Spire 2 `v0.107.1` with RitsuLib `v0.4.29` / `lib\0.107.1` and no BaseLib dependency: 25/25 Spire Plus patches, 10 event types / 14 registration calls, clean audit, retained enabled-mode verifier 31 / 0, and runtime packet verifier 61 / 0. The beta.92 Off proof under `.tools/runtime-evidence/v01071-beta92-ritsulib0429-off-direct-20260621/` also passed packet verifier 43 / 0. Retained beta.85 Off/CanaryOnly, beta.87 AdditiveBatch1, beta.88, and beta.90 proof remain previous-package/game-version loader context only. These are loader/registration proofs only. They do not prove event gameplay, save-load, replacement-pool behavior, image/render, multiplayer, QA, build/publish handoff, game-native AutoSlay/monkey batch stability, or full parity. Historical `v0.106.1` CanaryOnly/AdditiveBatch1 loader evidence must stay historical, and the beta.85 AdditiveBatch1 13/14 mismatch plus beta.87 `v0.107.1` BaseLib `v3.2.1` failure remain root-cause history only.

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
- Events must compile against the current repository package reference: `STS2.RitsuLib` `0.4.29`; Spire Plus must not require BaseLib.
- Current runtime reproof targets the installed official `STS2-RitsuLib` `v0.4.29` variant pack with `lib\0.107.1`.
- Normal StS1 event models inherit `EventModel` and are registered through RitsuLib content-builder APIs.
  Ancient-style events remain outside the StS1 unknown-room prototype scope.
