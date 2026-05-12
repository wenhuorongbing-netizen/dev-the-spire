# Urda Manual Test Checklist

Project: EZ Micro Balance  
Manifest id: EZMicroBalance  
Status: source gameplay slice implemented, live checks pending.

## 0. Environment controls

- Urda is default-on for private-beta testing.
- `EZMB_DISABLE_URDA=1` hides Urda for comparison.
- `EZMB_FORCE_ANCIENT=URDA` is legacy-compatible and no longer required.
- `EZMB_FORCE_URDA_BLESSING=<blessing-id>` (default-off).
- `EZMB_URDA_DIAGNOSTICS=1` (default-off).
- BaseLib and EZ Micro Balance enabled.
- Ancient Expansion v2.2 contains six additional Urda blessings as planning-only future work; they should not appear in this checklist's current live pool.

## 1. Baseline checks

- [ ] `dotnet build EZMicroBalance.sln` succeeds.
- [ ] `dotnet publish` run only if resources/localization changed.
- [ ] `docs/issues.md` contains the Urda issues and status.
- [ ] Baseline controlled run with only BaseLib and EZMicro Balance loads cleanly.

## 2. Urda registration checks

- [ ] New run can reach Act 1 Urda on the configured surface.
- [ ] Urda selection appears with EN and ZHS names.
- [ ] Only implemented Urda blessings are visible and selectable.
- [ ] Trial Branch, Shallow-Root Relic, Rooted Route, After the Rain, Root-Sight, and Seed Bank do not appear in the current live pool.
- [ ] No Morvi, Lotha, or Vakuu active content appears.
- [ ] Selected blessing is preserved on save/load.
- [ ] Blessing selection logs include the selected blessing id.

## 3. Seedbed checks

- [ ] Trigger counters start at zero and progress only on normal combat card rewards.
- [ ] Each normal Act 1 combat card reward can either take its regular card reward path or the Seedbed alternative while Seedbed has remaining checks.
- [ ] Rerolling or reopening a reward does not consume a Seedbed check before accepting Seedbed.
- [ ] Seedbed alternative is absent when max HP is 2 or lower.
- [ ] Accepting consumes 2 max HP and adds one Seedling card.
- [ ] First accepted Seedling is upgraded.
- [ ] Four successful accepts set the transformed latch and grant +10 max HP without healing current HP.
- [ ] No visible `Seedbed's Herald` name is expected in the current source slice.
- [ ] Save/load preserves `UrdaSeedbed` counters and transformed state.

## 4. Humus Pact checks

- [ ] The first three explicit `Compost Reward` choices on normal Act 1 combat card rewards grant 15 gold each.
- [ ] Ordinary reward skip/proceed and room-exit cleanup do not trigger Humus Pact.
- [ ] Third trigger completes the card reward first, then opens remove flow and offers one upgraded card.
- [ ] Third trigger can be completed with 0, 1, or 2 removals.
- [ ] Upgraded payoff reward cannot be skipped from its custom reward set.
- [ ] Third payoff does not duplicate, disappear, or softlock when leaving/reopening reward flows.
- [ ] Humus Pact marks completed and does not trigger again.
- [ ] Save/load preserves skip and completion state.

## 5. Molting / Withered Husk checks

- [ ] Selecting Molting removes one removable Strike-like starter card and one removable Defend-like starter card when present.
- [ ] Molting adds two `Withered Husk` cards.
- [ ] Each Withered Husk is unplayable/ethereal and grants block when exhausted.
- [ ] All Withered Husk are removed on Act 2 start.
- [ ] Act 1 save/load preserves pending Husk cards.

## 6. Moss Map checks

- [ ] Each room type applies at most once.
- [ ] Room-type rewards match type table.
- [ ] Re-entering same room type does not duplicate rewards.
- [ ] Save/load preserves room-type reward state.
- [ ] Map-type resolution remains stable across repeated room entry.

## 7. Release gate

- [ ] Morvi, Lotha, and Vakuu content remains disabled.
- [ ] Active Urda blessing list in release notes matches tested live content.
- [ ] Live Steam-client logs show no Urda-related exceptions.
- [ ] If Urda registration is blocked or unstable, set `EZMB_DISABLE_URDA=1` for comparison and reopen the default-on decision.
