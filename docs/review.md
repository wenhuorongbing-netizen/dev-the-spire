# Current Source Review

Date: 2026-05-26
Scope: compact no-game source/resource review notes for taking `Spire Plus` to a user-test-ready build. Full historical review details are archived at `docs/archive/feature-audits/review-pre-slim-20260518.md`, `docs/archive/feature-audits/review-2026-05-23-pre-compact.md`, `docs/archive/feature-audits/review-2026-05-24-sere-talon-pre-compact.md`, and `docs/archive/feature-audits/review-2026-05-26-beta54-pass-history.md`.

## Current Conclusion

No current static P0/P1 source blocker is known from the latest no-game review passes. This does not prove release readiness.

Live-only blockers remain:

- Vakuu victory return/no-black-screen, failure/death path, active-fight save-load, and co-op.
- Urda Root Eyes hover/click/entry/save-load, Seed Bank click extraction, and clicked Ancient UI.
- Morvi and Lotha live gameplay, card-play freeze reports, save-load, and co-op.
- A11 route traversal, A12/A16/A19/A20 combat behavior, and Rootblight combat-end behavior.

## Latest Fixed Findings

- 2026-05-26 RootDeck combat lifecycle split: `RootDeckService.Lifecycle.cs` now keeps run/start Rootblight seeding and explicit Rootblight I additions, while `RootDeckService.CombatLifecycle.cs` owns combat-start Rootblight marking plus combat-end growth, split, and pending downgrade resolution. Public method names, hook callers, saved-field names, and player-visible behavior are unchanged. No game was opened.
- 2026-05-26 Rootblight starter combat-start repair: `RootBudCombatHook.BeforeCombatStart()` now retries `RootDeckService.EnsureStartingRoot(...)` before marking combat-start Rootblight, so a missed run/room hook no longer leaves A14 without a starter card for combat-end growth bookkeeping. The played/planted Blight Sprout exclusion remains guarded. No game was opened.
- 2026-05-26 Chosen Decree card-state split: `AscensionCombatModifierService.BossSeals.ChosenDecree.cs` now keeps Queen/Torch Head settlement and per-round cap behavior, while `AscensionCombatModifierService.BossSeals.ChosenDecree.Cards.cs` owns Bound-card marking, play tracking, visible-card hydration, and marker cleanup. Public method names, hook callers, saved-field names, and player-visible behavior are unchanged. No game was opened.
- 2026-05-26 A20 Courtyard recovery split: `AscensionCombatModifierService.BossSeals.PhaseCarryover.cs` now owns only Test Subject / Residual Sample phase-carryover behavior, while `AscensionCombatModifierService.BossSeals.A20Courtyard.cs` owns the post-Boss-1 A20 courtyard healing hook. Method names, hook callers, and player-visible behavior are unchanged. No game was opened.
- 2026-05-26 Aeonglass Hourglass split: `AscensionCombatModifierService.BossSeals.AeonglassHourglass.cs` now owns only Time Sand / Wither settlement, while `AscensionCombatModifierService.BossSeals.AeonglassHourglass.State.cs` owns state hydration and enemy-move tracking and `AscensionCombatModifierService.BossSeals.AeonglassHourglass.LaserEcho.cs` owns Eye Lasers preview/counter behavior. Method names, hook callers, and player-visible behavior are unchanged. No game was opened.
- 2026-05-26 beta.56 package sync: `EZMicroBalance.json`, current package hash docs, release guards, website download metadata, and `website/README.md` point at `SpirePlus-v0.1.0-private-beta.56.zip` with ZIP SHA256 `43D6186B1C9E06400E47514203EE65028E478E18DE971B76370C4EF6542972C3`. Build, publish, isolated publish, package refresh, default tests, opt-in artifact tests, website syntax checks, format, diff-check, installed-package check, batch classification, generated-sidecar prune, and stale publish prune passed. No game was opened.
- 2026-05-26 preview-tool public-claim alignment: current Future Peek compatibility docs, website copy, and guard tests now describe only the integrated Spire Plus preview tools. Crystal Sphere remains a local fog/mask preview; transform preview uses a forked RNG snapshot and does not promise relic transforms or reward-choice foresight. No game was opened.
- 2026-05-26 source-boundary cleanup rollup: Morvi/Lotha hook ownership, Ascension localization bridge, preview transform RNG patches, Urda option relics, Root Sight selection, Trial Branch, Rooted Route, Seed Bank, Seedbed, state codecs, multiplayer policy gates, FeatureRegistry bootstrap, Boss seal marker/runtime powers, Vakuu child-combat services, combat trackers, and card-model aggregates were split into focused owners without changing public hook class names, save-field formats, or player-visible behavior. The detailed per-file list is preserved in `docs/archive/feature-audits/review-2026-05-26-beta54-pass-history.md`.

## Recent Historical Context

Detailed pre-current pass notes remain in the archive files listed above. This active review keeps only context that still guides current manual testing and prevents stale release claims.

- 2026-05-25 loader/startup context: historical beta.19 loader smoke reached the main menu with only BaseLib and Spire Plus enabled, registered `EZMicroBalance`, found 30 SavedSpireFields, and had a clean log audit. It is historical startup context only; beta.56 still needs fresh loader proof.
- 2026-05-25 co-op fail-closed pass: multiplayer gameplay mutations, combat hooks, Ancient reward/run hooks, Ascension reward/gameplay hooks, and Urda reward alternatives fail closed by default unless explicit opt-in environment variables are set. The two crash logs remain useful co-op evidence, but they do not prove current-package co-op behavior. Preview tools were later narrowed to local UI-only behavior and still need live two-client proof.
- 2026-05-25 player-facing polish: Seedbed / Planting, Seed Bank hover, A20 selector localization, Ancient direct-gain feedback, Fission Exhaust text, Soul Tide timing, Neow/Act 1 Ancient reroll, Elite Root, and high-pressure elite damage tuning are source/package-fixed and live-pending.
- 2026-05-24 Sere Talon / Tanx Claws lineage: source, package, art-route, handoff, website, and installed-package checks were hardened across multiple passes. Historical command logs are archive/context evidence, not a substitute for current live UI proof.

## Current Manual-Proof Focus

- Vakuu's Sere Talon must offer 4 Curses, choose 1, then add the selected Curse, 2 Wish, and 1 Wish+; its event option, relic bar, inspect screen, hover text, and log routes must not appear as Tanx Claws.
- Tanx Claws must stay on the Tanx route and transform selected cards into upgraded Maul+ / 撕咬+.
- Current-package Steam-client loader proof for beta.56 is pending; historical beta.19 and beta.17 loader rows are context only.
- Save/load, death/failure, co-op, clicked UI, hover, map traversal, preview tools, and gameplay evidence remain manual rows under `docs/issues.md`, `docs/toreview.md`, and the generated handoff.

## Still Not Claimed

- No live save/load, death/failure, co-op, clicked UI, hover, map traversal, or gameplay proof was produced; current state remains a manual-test candidate, not release-ready.
