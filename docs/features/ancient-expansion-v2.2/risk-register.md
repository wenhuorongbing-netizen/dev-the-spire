# Ancient Expansion v2.2 Risk Register

Status: Urda stabilization is source-backed but live-pending. Morvi has a default-off source prototype with the latest generated-copy and debt-payoff guards. Lotha, extra Urda blessings, and Vakuu fight remain planning-only. These risks must be resolved or explicitly accepted before any broader activation.

| Risk | Severity | Area | Required Mitigation |
| --- | --- | --- | --- |
| Power-card extra-play exploit | P0 | Morvi/Lotha card effects | Attack/Skill-only rule with tested Power fallback behavior. |
| Death-interrupt complexity | P0 | Lotha Death Reprieve | Inspect local lethal-damage and combat-end source before coding; add manual death-path checks. |
| Extra-play recursion | P0 | Misprint Press, Mirror Hall Echo | Mark generated/extra-played cards so they cannot retrigger the same blessing. |
| Debt accounting and HP fallback rounding | P1 | Morvi Debt Settlement | Define deterministic rounding, minimum HP protection, and save/load state. |
| Reward UI softlock | P1 | Morvi/Vakuu reward alternatives | Use proven reward completion paths; test accept, cancel, skip, and save/load. |
| Custom Ancient event art/background missing | P1 | Morvi/Lotha/Lotha activation | `NAncientEventLayout` uses Ancient background scenes, and no explicit local Morvi/Lotha art or custom scene file is present. Do not claim event-art integration or Lotha player-test readiness until real assets/scenes are added and exported. |
| Humus Pact reward reentry | P0 | Current Urda | Source-mitigated by replacing `CardReward.OnSkipped` postfix with explicit reward alternative plus `AfterRewardTaken` follow-up; live reward-screen verification pending. |
| Humus Pact third payoff loss | P0 | Current Urda | Source-mitigated by keeping `HumusCompletionPending` set until the payoff resolver succeeds and by generating the payoff card before optional removals; live reward-screen/save-load verification pending. |
| Seedbed generation accounting | P1 | Current Urda | Source-mitigated by counting accepted Seedbed choices only; reroll/reopen manual checks pending. |
| Player-field persistence not source-proven | P1 | Current Urda / Ascension-style player state | `SavedSpireField<Player,string>` is registered, but BaseLib docs only source-prove automatic persistence for saved-property model types. Keep save/load rows open until live evidence proves persistence or state is moved to a proven carrier. |
| Card storage/save-load issues | P1 | Archive Pages, Waste Paper, Temptation | Define zones and lifetime before implementation. |
| Active button implementation | P1 | Red Ink Overdraft | Source-proof action UI, disabled state, tooltip, and turn cleanup. |
| Debuff detection ambiguity | P1 | Lotha Public Evidence | Define exact source-backed debuff list and ownership. |
| Vakuu fight death/failure path | P1 | Vakuu optional fight | Prove failure transitions do not corrupt room/reward/combat state. |
| Multiplayer ownership/desync | P1 | All future Ancient systems | Use player-owned state; run host/client reward and save/load matrix. |
| Save/load persistence | P1 | All future blessings | Add serialization evidence and manual reload rows before activation. |
| Morvi Misprint target fidelity | P1 | Misprint Press | Generated-copy cleanup, clone/reentry guards, and Power-card exclusion are source-guarded; live test must still verify random/targeted cards behave acceptably through `CardCmd.AutoPlay`. |
| Morvi Debt reward reentry | P1 | Debt Settlement | Payoff is deferred to `AfterRewardTaken`, source-guarded to clear pending only after resolver success, and nonlethal HP fallback is documented; live test must verify no reward UI softlock after the third repayment. |
