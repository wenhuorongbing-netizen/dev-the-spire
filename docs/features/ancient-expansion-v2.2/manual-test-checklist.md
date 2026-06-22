# Ancient Expansion v2.2 Manual Test Checklist

Status: Urda eleven-blessing rows, default-on Morvi source rows, and default-on Lotha source rows are source-backed but still require live validation. Vakuu fight is hidden by default with a dedicated source enemy/scene because the reported post-victory black-screen path still needs live victory/save-load proof. Lotha Death Reprieve phase is deck-mirrored, and Urda/Morvi/Lotha encoded state mirror usage is source-guarded, but active live save/load restore remains pending. The latest player-facing polish pass removed legacy Urda option-marker wording, tightened key rich-text highlights, added Root-Sight hover explanation, moved Ancient combat-only behavior onto combat-state hooks, and added visible Trial Branch / Single Sentence counters; live hover/readability remains pending until clicked UI screenshots/logs are captured.

## 0. Planning Integrity

- [x] v2.2 design is stored outside `docs/issues.md`.
- [x] Compact issue file exists at `docs/issues/ancient-expansion-v2.2.md`.
- [x] Morvi is default-on for private-beta direct testing and can be hidden with `SPIREPLUS_DISABLE_MORVI=1`; legacy `EZMB_DISABLE_MORVI=1` still works.
- [x] Lotha is explicitly source-complete/live-pending; Vakuu fight is explicitly source-dedicated, hidden by default, and single-player only.
- [x] User approved continuing into the next development round before live testing.
- [x] Morvi/Lotha art direction is recorded in `art-direction.md`.
- [x] Morvi source image files are copied into `EZMicroBalance/images/events/` and listed in export resources before Morvi is made visible.
- [ ] Lotha custom Ancient background path is verified in game after exported assets/scenes are published.

## 0A. Ancient Clicked UI Evidence Helper

`scripts/collect-ancient-ui-evidence.ps1` prepares one forced-Ancient evidence folder, writes `ancient-ui-evidence-plan.json`, `manual-instructions.md`, `command.txt`, `environment.json`, `package-hashes.json`, and a pending `manual-rows-template.json`, runs the window preflight unless `-NoPreflight` is used, and only launches through `scripts/spire-plus-live-session.ps1` when `-Launch` is explicitly present. It now also prints a safer Spire Plus DevConsole smoke command that starts an unsaved single-player test run from the main menu and opens the requested Ancient. This helper and command prepare UI evidence; they do not prove natural routing, gameplay, save/load, or co-op by themselves.

Static resource-routing guards added on 2026-05-14 confirm current source/resource/export wiring only: Urda, Morvi, and Lotha scene files are Control-root clicked backgrounds using event art; map/run-history icons and option marker relic art remain separate exported resources; and the latest art audit reports 0 missing targets, 0 hash mismatches, and 0 missing exports. Later GPTimage2/browser art passes replaced the temporary small-art blockers for the current package. Beta.107 smoke proof captured forced Urda, Morvi, Lotha, and normal Vakuu clicked screens; keep hover/readability, relic-bar follow-through, gated Vakuu fight-option, and gameplay rows pending until live evidence covers them.

Prepare without launching:

```powershell
.\scripts\collect-ancient-ui-evidence.ps1 -Mode Prepare -Ancient URDA -MoveOtherMods -MoveCurrentRuns
.\scripts\collect-ancient-ui-evidence.ps1 -Mode Prepare -Ancient MORVI -MoveOtherMods -MoveCurrentRuns
.\scripts\collect-ancient-ui-evidence.ps1 -Mode Prepare -Ancient LOTHA -MoveOtherMods -MoveCurrentRuns
.\scripts\collect-ancient-ui-evidence.ps1 -Mode Prepare -Ancient VAKUU -MoveOtherMods -MoveCurrentRuns
.\scripts\collect-ancient-ui-evidence.ps1 -Mode Prepare -Ancient VAKUU -ForceVakuuFight -MoveOtherMods -MoveCurrentRuns
```

Rerun the printed command with `-Launch` only when ready for a live session. The helper sets `SPIREPLUS_FORCE_ANCIENT=<Ancient>` and `EZMB_FORCE_ANCIENT=<Ancient>` for the launched process; for `VAKUU -ForceVakuuFight`, it also sets `SPIREPLUS_FORCE_VAKUU_FIGHT=1` and `EZMB_FORCE_VAKUU_FIGHT=1`.

Expected visible option counts are Urda 4, Morvi 3, Lotha 3, and Vakuu 3 by default. Vakuu shows 4 only when preferred `SPIREPLUS_ENABLE_VAKUU_FIGHT=1` or legacy `EZMB_ENABLE_VAKUU_FIGHT=1` is deliberately set. Current source keeps the focused `-ForceVakuuFight` case to one fight option.

Preferred unsaved UI-smoke commands, run from the main menu after the live-session helper launches the game:

```text
spireplus_test_ancient URDA confirm
spireplus_test_ancient MORVI confirm
spireplus_test_ancient LOTHA confirm
spireplus_test_ancient VAKUU confirm
spireplus_test_ancient VAKUU confirm fight
```

The `fight` form sets the current game process's Vakuu force-fight gate before opening Vakuu. These commands use `shouldSave: false` and refuse to run while another run is already in progress, so they are safer for UI smoke than continuing a user run. They still count only as UI render smoke, not natural gameplay proof.

Legacy active-run DevConsole render-smoke commands, only after a run is already in progress. Prefer the `spireplus_test_ancient ...` commands above from the main menu. Do not run these legacy commands from the main menu: local Core `AncientConsoleCmd.Process(...)` reads `issuingPlayer.RunState` and the 2026-05-15 `.tools/runtime-evidence/ancient-ui-click-vakuu-20260515-211824` attempt confirmed a main-menu command has no player context and is invalid evidence. Use these only when natural routing would take too long and the row is marked as UI render smoke rather than gameplay proof:

```text
ancient EZMB_URDA
ancient EZMB_MORVI
ancient EZMB_LOTHA
ancient VAKUU
```

Follow the generated `manual-instructions.md` for exact screenshot/log filenames. Clicked UI evidence must include the screenshot, foreground `window-preflight.json`, copied `godot.log`, `godot-log-audit.json`, and `route-note.md` stating whether the route was natural map click or DevConsole render smoke.

Use `scripts/send-spire-dev-console-command.ps1 -Command "spireplus_test_ancient URDA confirm"` only when Slay the Spire 2 is visible and ready for keyboard input. Use `scripts/capture-spire-window.ps1 -RequireSpireForeground` after `check-spire-window-preflight.ps1` passes so screenshots cannot silently capture another window.

Restore after capture:

```powershell
.\scripts\collect-ancient-ui-evidence.ps1 -Mode Restore -EvidenceDir <evidence-dir>
```

Beta.107 already captured forced Urda, Morvi, Lotha, and normal Vakuu clicked-screen screenshots/logs. Keep the gated Vakuu fight-option, hover/readability, relic-bar follow-through, save-load, co-op, and gameplay rows pending until their matching live files exist.

## 1. Current Urda First

- [ ] Urda event UI renders event art, dialogue, four options, option/relic icons, and hover tips.
- [ ] Seedbed live selection and reward alternative verified, including no counter advance from reroll/reopen alone.
- [ ] Seedbed does not offer the alternative when max HP is not greater than 2.
- [ ] Seedbed fourth acceptance grants +10 max HP without healing current HP.
- [ ] Seedbed card text and hover explain Temporary and Plant without crowding the tooltip.
- [ ] Seedbed gives 8 Block, sets 2 slots, and immediately plants 1 eligible draw/discard card; Seedbed+ gives 12 Block, sets 3 slots, and can immediately plant up to 2.
- [ ] Later Temporary Status cards, Temporary Curse cards, Blight Sprouts, and Rootblight are planted before entering hand; each planted card adds one Withered Husk.
- [ ] Permanent Curses, Withered Husk, and beneficial temporary pages are not planted.
- [ ] Planted cards do not trigger play, discard, draw, or Exhaust synergies.
- [ ] Humus Pact live `Compost Reward` alternative, gold, remove flow, and upgraded-card payoff verified.
- [ ] Humus Pact third payoff does not duplicate, disappear, or softlock.
- [ ] Humus Pact does not trigger from ordinary reward-set skip/proceed or room-exit cleanup.
- [ ] Molting / Withered Husk live card behavior verified: 0-cost Ethereal/Exhaust Curse; when exhausted it gives 3 Block.
- [ ] Moss Map room-type reward behavior verified.
- [ ] Trial Branch offers four rare class cards, upgrades and adds the chosen card, applies the visible `Trial Branch` enchantment, and shows combats left/current-combat play state/remaining required plays.
- [ ] Trial Branch success path: the chosen card is played in each of the next three combats, then the card is kept and the Trial Branch marker/enchantment are cleared.
- [ ] Trial Branch failure path: missing the chosen card in any one of the next three combats removes it immediately after that combat.
- [ ] Shallow-Root Relic offers two common relics, grants the chosen relic plus 75 Gold, roots on an Act 1 elite for 35 Gold, and otherwise uses the documented Act 2 removal/refund fallback.
- [ ] Elite Root shows as an Urda first-tier option relic, then heals 10 HP after each Elite victory. Include one Firemarked Elite in the manual check if possible.
- [ ] Rooted Route auto-marks a reachable normal-combat node within the first seven floors without changing the map graph, rewards reaching it, and withers for 8 HP loss plus 25 Gold if unreachable.
- [ ] After Rain gains 1 Rain Breath after the first unblocked enemy attack damage in each Act 1 combat; if the hand is full, verify Core's normal generated-card fallback is readable. At Act 2 start, fewer than 3 triggers gives 75 Gold, otherwise heal 8 and upgrade 1 card.
- [ ] Root-Sight starts with 5 Root Eyes. Clicking the Root Eyes relic opens map selection, highlights future reachable Monster, Unknown, or Elite rooms, stores the chosen room's concrete enemy group or event on hover, excludes Rest Sites/Shops/Treasure/Boss rooms, and grants the first-use potion if a slot exists.
- [ ] Seed Bank uses the source-safe `Store Seed` reward alternative, caps at three Seeds, and lets the player click the Seed Bank relic later to choose up to two Seeds with the first upgraded. It must not mark Seed Bank cards as Trial Branch plants.
- [ ] Current Urda save/load verified; do not close from `SavedAttachedState<Player, string>` source evidence alone.

## 2. Default-On Morvi Source Slice

- [ ] Morvi appears in Act 2 by default and is hidden when `SPIREPLUS_DISABLE_MORVI=1`; legacy `EZMB_DISABLE_MORVI=1` still works.
- [ ] `SPIREPLUS_FORCE_ANCIENT=MORVI` focuses Act 2 testing on Morvi; legacy `EZMB_FORCE_ANCIENT=MORVI` still works.
- [ ] `SPIREPLUS_FORCE_MORVI_BLESSING` can force each id: `morvi_forbidden_loan`, `morvi_misprint_press`, `morvi_red_ink_overdraft`, `morvi_overdue_library`, `morvi_open_book_exam`, `morvi_paperstorm`, `morvi_blueprint_proof`, and `morvi_debt_settlement`. Legacy `EZMB_FORCE_MORVI_BLESSING` still works.
- [ ] Morvi event UI renders event art, dialogue, three options, option/relic icons, and hover tips.
- [ ] Forbidden Loan offers one of three class Ancient cards, adds the upgraded chosen card with the Borrowed Ancient marker, and charges 1 HP for borrowed Attack/Skill plays.
- [ ] Forbidden Loan borrowed Power play loses 8 HP and is not copied, replayed, or extra-played by Morvi v2.2 systems.
- [ ] Forbidden Loan source-safe deviation verified: after the Act 2 boss the borrowed card auto-settles by paying 180 Gold if possible, otherwise the card is removed; no post-boss choice UI is claimed.
- [ ] Misprint Press triggers once per turn from the first player-played Attack or Skill, uses play-count modification on the original card, draws 1 when the original/base Energy cost is at least 1, and creates no copied card in hand.
- [ ] Misprint Press ignores Power, Status, Curse, autoplay, generated clone, and recursive extra-play executions.
- [ ] Red Ink Overdraft source-safe UI deviation verified: a temporary 0-cost Overdraft action card is added at player-turn start only when hand space allows, stays out of discard when the hand is full, and is playable only once per turn at 0 Energy; it is not an automatic trigger or native combat button.
- [ ] Red Ink Overdraft draws 2, gains 1 Energy, records one debt, and at combat end pays 12 Gold per debt or loses 3 nonlethal HP per unpaid debt.
- [ ] Overdue Library adds three random temporary Archive Pages at combat start from Draw, Veil, Burn, Discount, Bravery, and Dexterity pages; unplayed pages carry no extra punishment and are cleaned up after combat.
- [ ] Open-Book Exam turn 1 draws up to 5 extra cards, gains 2 Energy, seals tracked Open Book cards remaining in hand at turn end, and returns them at turn 3 start with cost 0 for that turn/play.
- [ ] Open-Book Exam source-safe deviation verified: sealed cards are held through an Exhaust Pile holding path and return only when hand space allows. Save/load during the sealed-card window must prove the saved marker recovery works.
- [ ] Paperstorm shuffles four Waste Paper status cards into the Draw Pile and the first two Status cards drawn from the Draw Pile each turn are consumed for draw 1 and Energy 1.
- [ ] Blueprint Proof starts combat with 3 Proofread stacks; the first three non-Status, non-Curse player-played deck cards either temporarily upgrade and draw 1, or if already upgraded cost 1 less and grant 4 Block. Power cards are never extra-played.
- [ ] Debt Settlement immediately grants 220 Gold, removes up to 2 cards, upgrades 2 cards, sets Debt to 320, then each combat end pays due `min(40, Debt)` with Gold first and 3 nonlethal HP per 10 Gold short rounded up while Debt decreases by the full due.
- [ ] Morvi save/load after selecting each blessing preserves the selected blessing, debt/progress state, and borrowed Ancient card marker.
- [ ] Morvi co-op behavior is observed before any multiplayer-safe claim.

## 3. Default-On Lotha Source Slice

- [x] `EZMicroBalance/images/events/ezmb_lotha.png` and custom scene path are present before Lotha is enabled for player testing.
- [ ] Lotha appears in Act 3 by default and is hidden when `SPIREPLUS_DISABLE_LOTHA=1`; legacy `EZMB_DISABLE_LOTHA=1` still works.
- [ ] `SPIREPLUS_FORCE_ANCIENT=LOTHA` focuses Act 3 testing on Lotha; legacy `EZMB_FORCE_ANCIENT=LOTHA` still works.
- [ ] Lotha event UI renders event art, dialogue, three options, option/relic icons, and hover tips.
- [ ] Mirror Rebuttal selection screen chooses exactly one Attack, Skill, or Power deck card.
- [ ] Mirror Rebuttal moves the selected combat card to hand on the first player turn after normal draw when it starts in a combat pile outside the hand; if the hand is full, it goes to the top of the draw pile.
- [ ] Mirror Rebuttal first selected Attack/Skill play adds one extra play, does not recurse from autoplay/generated executions, and selected Power costs 0 for that play instead of extra-playing.
- [ ] Mirror Hall Echo records the last player-played non-Status Attack/Skill/Power at player-turn end and only the next turn's first matching player-played card consumes the echo.
- [ ] Mirror Hall Echo Attack/Skill consumes the echo for one extra play; Power consumes it by costing 0 for that play and drawing 1, with no Energy gain; autoplay/generated cards do not set or consume it.
- [ ] Presumption applies visible Innocent state at combat start and each turn while active draws 2, grants 1 Energy, and grants 8 Block.
- [ ] Presumption breaks only from unblocked enemy attack damage in normal combat testing, then removes Innocent, applies immediate 8 HP loss, and does not return this combat.
- [ ] Closed Court removes post-combat card rewards for the rest of the run while gold, potions, and relic rewards still appear.
- [ ] Closed Court turn 1 draws 4 and grants 2 Energy; turn 4 draws 2 and grants 2 Energy.
- [ ] Deferred Verdict turn 4 grants draw 4, Energy 4, and 3 player-owned Verdict; each next non-Status card consumes 1 Verdict.
- [ ] Deferred Verdict Attack/Skill adds one play, Power costs 0 for that play and draws 1 with no Energy gain, Verdict clears after turn 4/combat, and combat ending before turn 4 heals 4 HP.
- [ ] Death Reprieve player-turn lethal trigger sets HP to 1 and starts the reprieve immediately in the current player turn.
- [ ] Death Reprieve enemy-turn lethal trigger sets HP to 1 and starts the reprieve on the next player turn; this is the documented source-safe deviation from immediate turn interruption.
- [ ] During Death Reprieve, draw 10, gain 10 Energy, all cards cost 0, and further damage/HP loss cannot kill the player.
- [ ] Death Reprieve victory during the reprieve turn continues the run if all enemies die before turn end.
- [ ] Death Reprieve failure after the reprieve turn kills the player with enemies alive and does not duplicate rewards or room transitions.
- [ ] Death Reprieve interaction with other death-prevention effects is tested, including trigger order and no repeated once-per-run activation.
- [ ] Death Reprieve save/load before any lethal trigger preserves the selected blessing and leaves the once-per-run reprieve available.
- [ ] Death Reprieve save/load after lethal prevention records no duplicate trigger. Current source persists `DeathReprieveUsed` plus `DeathReprievePhase` through Lotha player/deck state, but this row remains pending until live restore proves it.
- [ ] Death Reprieve save/load while reprieve start is pending or the reprieve turn is active remains a blocking live row. Current source can rehydrate pending/active protection state from the deck-mirrored phase and logs the restored phase/power state, but exact active-turn hand/energy/pile/power continuation is not source-proven; do not count this path as save-safe without direct live proof.
- [ ] Single Sentence first player-played Attack/Skill each turn adds two plays, then only four more normal player-played cards can be played that turn.
- [ ] Single Sentence visible Power/counter starts at 5 while the ruling is ready, becomes 4 after the Attack/Skill ruling, counts down each later normal play, and reaches 0 when additional card plays are blocked.
- [ ] Single Sentence cap does not count extra-play executions, autoplay/generated cards, clones, or blocked play attempts; first Power before the sentence costs 0 for that play and draws 1 without consuming it.
- [ ] Public Evidence doubles only non-damaging negative statuses in both directions, grants/removes Enlightenment, and consumes up to 3 Enlightenment at turn start for draw and Block. Verify Weak, Vulnerable, and Frail count; verify Poison, damage-over-time, countdown damage, and source-proven damage/kill Debuffs such as Constrict, Demise, Disintegration, Doom, Magic Bomb, Strangle, and The Gambit do not count.
- [ ] Lotha save/load after selecting each blessing preserves the selected blessing and any persistent once-per-run/deck-card state.
- [ ] Lotha co-op behavior is observed before any multiplayer-safe claim.

## 4. Vakuu Fight Unfinished Opt-In Slice

- [ ] In single-player, normal Vakuu shows only its three standard options by default.
- [ ] `SPIREPLUS_ENABLE_VAKUU_FIGHT=1` adds the gated fight as a fourth option; `SPIREPLUS_DISABLE_VAKUU_FIGHT=1` hides it again. Legacy `EZMB_ENABLE_VAKUU_FIGHT=1` and `EZMB_DISABLE_VAKUU_FIGHT=1` still work.
- [ ] `SPIREPLUS_FORCE_ANCIENT=VAKUU` focuses Act 3 testing on Vakuu; legacy `EZMB_FORCE_ANCIENT=VAKUU` still works.
- [ ] `SPIREPLUS_FORCE_VAKUU_FIGHT=1` limits Vakuu to the fight option for focused testing; legacy `EZMB_FORCE_VAKUU_FIGHT=1` still works.
- [ ] Declining the fight preserves current Vakuu behavior.
- [ ] Selecting Fight Vakuu enters the dedicated Vakuu trial combat and the option text explains Stolen Locks, Contracts, Blood Debt, Cash Out, no normal rewards, and death risk.
- [ ] On turns 1, 3, and 5, after the normal hand draw, Vakuu shows contract choices; the chosen Contract is added to hand if there is hand space.
- [ ] Contract cards show 0-cost Skill token behavior with Ethereal and Exhaust hover tips, plus Stolen Vault and Blood Debt hover tips, and no duplicated keyword body text.
- [ ] Playing Knife Contract and Gold Contract costs HP, breaks one Stolen Vault lock if any remain, adds one Blood Debt, and resolves its listed effect without softlock.
- [ ] Playing Shelter Contract grants Block and removes Blood Debt; playing Fraud Contract breaks a lock, adds two Blood Debt, and applies one-turn Backlash.
- [ ] Breaking at least one lock adds Cash Out when there is hand space; playing it ends the fight through the normal Vakuu victory path.
- [ ] Dealing at least 40 unblocked damage to Vakuu in one player turn breaks one Stolen Vault lock once for that turn, including lethal hits.
- [ ] Blood Debt increases each of Vakuu's powered attack hits by 2 damage per stack and the intent updates accordingly.
- [ ] Victory offers 1/2/3 non-Vakuu Act 3 Ancient blessing choices based on broken locks when enough unclaimed choices remain; otherwise the fallback continue option appears.
- [ ] Victory grants 50 loot Gold per broken lock, then Blood Debt removes 15 loot Gold per stack before the remainder is awarded; unpaid debt costs nonlethal HP.
- [ ] Combat victory does not show a normal combat reward screen before the Vakuu victory blessing choice.
- [ ] Combat victory returns to the Vakuu event without a black screen. This row specifically verifies the direct parent-room stack transition and parent event `Node` cleanup added after the 2026-05-15 report.
- [ ] Failure/death path is correct and does not softlock.
- [ ] Save/load before choosing Fight Vakuu preserves the normal Vakuu event and choice availability.
- [ ] Save/load during active Vakuu child combat remains live-pending: the direct parent-room stack transition no longer stores active `ParentEventId`, avoiding local `CombatRoom.ToSerializable()`'s known active-parent exception. If the game permits saving during this active fight, verify reload behavior directly; do not close this row until no-normal-reward flow and parent resume are proven.
- [ ] Save/load after Vakuu combat victory/resume preserves the no-normal-reward victory flow and either the broken-lock-based non-Vakuu Act 3 blessing choices or the fallback continue option, without applying a second Ancient heal from the reconstructed parent event. Source logs the explicit ownerless fallback path if restore reaches it, but that log is not live save/load proof by itself.
- [ ] Co-op does not show the fight option unless a future explicit multiplayer-safe design replaces the current single-player gate.

## 5. Multiplayer / Save-Load

- [ ] Player-owned state is independent in co-op.
- [ ] Host/client reward options match.
- [ ] Save/load after selecting each blessing preserves state.
- [ ] Rejoining does not duplicate rewards, debts, verdicts, or temporary cards.
