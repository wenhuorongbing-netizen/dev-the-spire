# Urda Manual Test Checklist

Project: Spire Plus (`EZMicroBalance` manifest id)
Manifest id: EZMicroBalance  
Status: ten-blessing source gameplay slice implemented, live checks pending.

## 0. Environment controls

- Urda is default-on for private-beta testing.
- `EZMB_DISABLE_URDA=1` hides Urda for comparison.
- `EZMB_FORCE_ANCIENT=URDA` is legacy-compatible and no longer required.
- `EZMB_FORCE_URDA_BLESSING=<blessing-id>` (default-off).
- `EZMB_URDA_DIAGNOSTICS=1` (default-off).
- BaseLib and Spire Plus enabled.
- Ancient Expansion v2.2 current Urda source pool contains ten blessing ids; each should remain hidden when `EZMB_DISABLE_URDA=1`.

## 1. Baseline checks

- [ ] `dotnet build EZMicroBalance.sln` succeeds.
- [ ] `dotnet publish` run only if resources/localization changed.
- [ ] `docs/issues.md` contains the Urda issues and status.
- [ ] Baseline controlled run with only BaseLib and Spire Plus / `EZMicroBalance` loads cleanly.

## 1A. Live evidence protocol

Use this protocol for post-fix Urda selection, reward-screen, save/load, and visual evidence. Invalid local screenshot attempts under `.tools/runtime-evidence/live-urda-postfix-20260513-131752` and `.tools/runtime-evidence/live-urda-continue-postfix-20260513-134337` do not satisfy any gameplay row.

- [ ] Prepare a restore-safe normal Steam session:
  `scripts/spire-plus-live-session.ps1 -Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch`
- [ ] Record the evidence directory printed in `session-state.json`.
- [ ] Before every gameplay screenshot batch, require foreground confirmation:
  `scripts/check-spire-window-preflight.ps1 -OutFile <evidence-dir>\window-preflight.json -RequireSpireForeground`
- [ ] If the preflight exits nonzero, do not capture or count screenshots; bring Slay the Spire 2 foreground first and rerun the preflight.
- [ ] Copy the live `godot.log` into the evidence directory after the gameplay row being tested.
- [ ] Audit the copied log:
  `scripts/audit-godot-log.ps1 -Path <evidence-dir>\godot.log -OutFile <evidence-dir>\godot-log-audit.json -FailOnHit`
- [ ] Restore the machine state after any run-start or continue test:
  `scripts/spire-plus-live-session.ps1 -Mode Restore -EvidenceDir <evidence-dir> -StopGameOnRestore -PreserveNewCurrentRunsOnRestore`
- [ ] Confirm restore output reports settings and moved mods restored, and any test-created `current_run*` files are preserved inside the evidence directory before the original current run is restored.

## 2. Urda registration checks

- [ ] New run can reach Act 1 Urda on the configured surface.
- [ ] Urda selection appears with EN and ZHS names.
- [ ] Exactly the ten source-backed Urda blessing ids are visible/selectable across forced runs or repeated selections.
- [ ] Trial Branch, Shallow-Root Relic, Rooted Route, After the Rain, Root-Sight, and Seed Bank appear with honest source-safe text and option relic art.
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

## 7. Trial Branch checks

- [ ] Selection offers four common/uncommon class cards through the source-safe card grid.
- [ ] Chosen card is upgraded, added to deck, and marked as Trial Plant.
- [ ] Over the next three combats, only combats where the marked card is player-played at least once count as successes.
- [ ] After three combats, two or more successful combats keep the card and clear the marker.
- [ ] After three combats, fewer than two successful combats removes the marked card from the deck.
- [ ] Save/load preserves the selected Trial Plant card marker and combat counters.

## 8. Shallow-Root Relic checks

- [ ] Selection offers two common relic choices, grants the chosen relic, and grants 75 Gold exactly once.
- [ ] Defeating an Act 1 elite roots the relic, grants 35 Gold, and prevents Act 2 fallback removal.
- [ ] If Act 2 starts without rooting, source-safe fallback removes the pending relic and refunds 75 Gold; no `lose 6 Max HP to keep it` UI is claimed in this slice.
- [ ] Save/load does not duplicate relics or gold and preserves pending/rooted state.

## 9. Rooted Route checks

- [ ] Selection automatically marks one reachable non-Boss, non-chest normal-combat node within the first seven floors.
- [ ] Map graph shape and outgoing edges are unchanged after marking.
- [ ] Reaching the Root Mark grants three card rewards or the documented source-safe equivalent, grants one random potion if a slot exists, and upgrades the first card taken from these rewards when source-safe.
- [ ] If the Root Mark becomes unreachable, it withers, the player loses 8 HP, and gains 25 Gold.
- [ ] Save/load preserves the marked coordinate and resolved/withered state.

## 10. After the Rain checks

- [ ] In Act 1 only, the first lethal damage prevents death, leaves/sets 1 HP, grants 15 Block, draws 1 card, adds two Wounds to discard, loses 3 Max HP, and spends the blessing.
- [ ] Further lethal damage after the blessing is spent is not prevented by After the Rain.
- [ ] If unused before Act 2, the player heals 8 HP and gains 75 Gold once.
- [ ] Before spending, each Act 1 elite kill grants 20 Gold, maximum two times.
- [ ] Save/load preserves spent, compensation, and elite-bonus counters.

## 11. Root-Sight checks

- [ ] Selection grants 5 Root Eyes.
- [ ] Source-safe fallback automatically marks reachable visible non-Boss rooms; no unavailable map button is shown or claimed.
- [ ] Boss rooms are never marked.
- [ ] First use grants one random potion if a slot exists.
- [ ] Save/load preserves Root Eye count, first-potion state, and marked rooms.

## 12. Seed Bank checks

- [ ] During Act 1 normal combat card rewards, `Store Seed` appears while fewer than three seeds are stored.
- [ ] Source-safe behavior is honest: storing consumes the current card reward instead of also taking a card.
- [ ] At most three Seeds can be stored.
- [ ] Before the Act 1 Boss, the settlement picker allows choosing up to two Seeds.
- [ ] First chosen Seed is upgraded and added to deck.
- [ ] Second chosen Seed, if any, is added to deck without a Trial Plant marker.
- [ ] Unchosen Seeds disappear, and settlement does not repeat.
- [ ] Save/load preserves stored Seeds and settlement state.

## 13. Release gate

- [ ] Focused Urda runs use force/disable gates as needed so Morvi, Lotha, and Vakuu do not contaminate Urda evidence.
- [ ] Active Urda blessing list in release notes matches tested live content.
- [ ] Live Steam-client logs show no Urda-related exceptions.
- [ ] If Urda registration is blocked or unstable, set `EZMB_DISABLE_URDA=1` for comparison and reopen the default-on decision.
