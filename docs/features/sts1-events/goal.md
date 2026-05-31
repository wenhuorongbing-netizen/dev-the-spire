# StS1 Events Migration Goal

## Objective

Long-term target: account for the Slay the Spire 1 public event baseline inside
the Spire Plus mod (`EZMicroBalance`) and prove a playable StS1-like event
experience in Slay the Spire 2. Current source status is a gated prototype, not
runtime gameplay proof.

## Success Criteria

- [ ] Public 52-event baseline is reconciled with the current source counts (`54` canonical rows, `50` registry identities, `48` model files, `47` compiling models)
- [ ] Each event has correct StS1 behavior (options, rewards, conditions)
- [ ] Ascension 15 differences are applied where applicable
- [ ] Events are registered via RitsuLib `RegisterActEvent`/`RegisterSharedEvent`
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
- Events must compile against existing RitsuLib 0.3.2 + BaseLib 3.1.4
- Use RitsuLib `ModEventTemplate` for normal events, `ModAncientEventTemplate`
  for ancient-style events
