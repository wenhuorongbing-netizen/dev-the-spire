# Ancient Expansion v2.2 Manual Test Checklist

Status: Urda and default-off Morvi prototype rows are source-backed but still require live validation. Lotha, Vakuu fight, and extra Urda rows remain future validation requirements; Lotha is blocked this pass by missing explicit event-art/background source files.

## 0. Planning Integrity

- [x] v2.2 design is stored outside `docs/issues.md`.
- [x] Compact issue file exists at `docs/issues/ancient-expansion-v2.2.md`.
- [x] Morvi is explicitly default-off behind `EZMB_ENABLE_MORVI_V22=1`.
- [x] Lotha and Vakuu fight are explicitly planning-only.
- [x] User approved continuing into the next development round before live testing.
- [x] Morvi/Lotha art direction is recorded in `art-direction.md`.
- [ ] Morvi/Lotha source image files are copied into `EZMicroBalance/images/events/` and verified in export resources.
- [ ] Morvi/Lotha custom Ancient background path is verified in game after real assets/scenes are exported.

## 1. Current Urda First

- [ ] Seedbed live selection and reward alternative verified, including no counter advance from reroll/reopen alone.
- [ ] Seedbed does not offer the alternative when max HP is not greater than 2.
- [ ] Seedbed fourth acceptance grants +10 max HP without healing current HP.
- [ ] Humus Pact live `Compost Reward` alternative, gold, remove flow, and upgraded-card payoff verified.
- [ ] Humus Pact third payoff does not duplicate, disappear, or softlock.
- [ ] Humus Pact does not trigger from ordinary reward-set skip/proceed or room-exit cleanup.
- [ ] Molting / Withered Husk live card behavior verified.
- [ ] Moss Map room-type reward behavior verified.
- [ ] Current Urda save/load verified; do not close from `SavedSpireField<Player,string>` source evidence alone.

## 2. Default-Off Morvi Prototype

- [ ] Morvi does not appear when `EZMB_ENABLE_MORVI_V22` is unset.
- [ ] Morvi appears in Act 2 when `EZMB_ENABLE_MORVI_V22=1`.
- [ ] `EZMB_FORCE_MORVI_BLESSING=morvi_misprint_press` limits Morvi options to Misprint Press.
- [ ] `EZMB_FORCE_MORVI_BLESSING=morvi_open_book_exam` limits Morvi options to Open-Book Exam.
- [ ] `EZMB_FORCE_MORVI_BLESSING=morvi_debt_settlement` limits Morvi options to Debt Settlement.
- [ ] Misprint Press extra-play does not recurse.
- [ ] Misprint Press ignores Power cards and generated clones.
- [ ] Misprint Press generated-copy failure path leaves no unpiled clone behind.
- [ ] Open-Book Exam upgrades only an Attack or Skill reward option in normal Act 2 combat rewards.
- [ ] Debt Settlement grants 75 Gold on selection.
- [ ] Debt Settlement `Repay Debt` alternative appears only while debt remains and the player can pay Gold or nonlethal HP.
- [ ] Debt Settlement third repayment offers an upgraded card reward and does not softlock.
- [ ] Debt Settlement debt accounting survives save/load.

## 3. Future Lotha

- [ ] Explicit `EZMicroBalance/images/events/ezmb_lotha.png` source file or custom scene path is present before Lotha is enabled for player testing.
- [ ] Lotha appears only when enabled by future gate.
- [ ] Mirror effects do not target unsupported card types.
- [ ] Deferred Verdict stacks display and clear correctly.
- [ ] Death Reprieve lethal-damage path cannot duplicate death, rewards, or room transitions.
- [ ] Public Evidence debuff detection matches source-backed rules.

## 4. Future Vakuu Fight

- [ ] Extra fight option appears only when enabled.
- [ ] Declining the fight preserves current Vakuu behavior.
- [ ] Victory offers three non-Vakuu Act 3 Ancient blessings.
- [ ] Failure/death path is correct and does not softlock.
- [ ] Temptation text and status behavior are clear if implemented.

## 5. Multiplayer / Save-Load

- [ ] Player-owned state is independent in co-op.
- [ ] Host/client reward options match.
- [ ] Save/load after selecting each blessing preserves state.
- [ ] Rejoining does not duplicate rewards, debts, verdicts, or temporary cards.
