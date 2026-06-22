# Current Source Review

Date: 2026-05-26; latest addendum: 2026-06-22
Scope: compact no-game source/resource review notes for taking `Spire Plus` to a user-test-ready build.

Full historical review details are archived at `docs/archive/feature-audits/review-pre-slim-20260518.md`, `docs/archive/feature-audits/review-2026-05-23-pre-compact.md`, `docs/archive/feature-audits/review-2026-05-24-sere-talon-pre-compact.md`, `docs/archive/feature-audits/review-2026-05-26-beta54-pass-history.md`, and `docs/archive/feature-audits/review-current-fixed-findings-history-20260622.md`.

## Current Conclusion

No current static P0/P1 source blocker is known from the latest no-game review passes. This does not prove release readiness.

Beta.122 RitsuLib-only package parity, runtime preflight, source-workspace validation, and clicked Ancient UI smoke are current. The beta.122 smoke passed 4 / 4 forced iterations for Urda, Morvi, Lotha, and normal Vakuu under `.tools/runtime-evidence/monkey-stability-beta122-20260622-230109/` with packet verification 1621 / 0 and all 127 migrated Spire Plus patches applied. This closes smoke-level clicked Ancient UI migration proof only; gameplay, gated Vakuu fight-option/victory return, save-load, current enabled-mode proof, replacement, multiplayer, QA, and handoff proof remain pending.

Clicked UI is limited to beta.122 forced Ancient smoke. Current smoke proof covers forced Urda, Morvi, Lotha, and normal Vakuu only; gated Vakuu fight-option UI, relic hover/readability, and gameplay follow-through remain pending.

## Current Migration Evidence

- Spire Plus targets `STS2.RitsuLib` `0.4.34`; `EZMicroBalance.json` declares only `STS2-RitsuLib >= 0.4.34` as the runtime dependency.
- Current source has completed the clicked/input UI migration, visual-hover UI getter migration, rest-site Meat Cleaver UI migration, Neow/Vakuu event-option UI migration, A20 courtyard portrait migration, Batch 4c ascension-localization fallback migration, core inline-localization fallback migration, Ancient reward getter/relic hook migration, Aeonglass intent UI migration, and Enemy Damage polish getter migration to RitsuLib `IPatchMethod` / `ModPatcher`: 127 migrated patch classes and 43 raw Harmony declarations remain in `docs/patch-inventory.md`.
- `check-local-godot-source-workspace.ps1` verifies the unpacked local source snapshot, installed game identity, package parity, and installed `STS2-RitsuLib.xml` markers for the RitsuLib APIs Spire Plus uses. The current refreshed run passed 57 / 0 with the retained GDRE warnings only.
- The repository entry docs are free of the retired framework name and route future implementation through RitsuLib docs/XML plus the unpacked local game source under `source code/src/Core/`.
- The beta.122 package proof covers the refreshed package after later RitsuLib source migrations. Current beta.122 clicked UI smoke applied all 127 migrated Spire Plus ModPatcher patches; gameplay and handoff proof remain separate pending gates.
- Previous enabled-mode packets remain historical or previous-package context and do not close current beta.122 enabled-mode gameplay proof.
- AutoSlay and runtime-monkey verifier hardening is methodology evidence only. Proof-mode packets still require `-ExpectedAncientIds` to match retained plan/summary/traversal state, and those verifier rules do not close current beta.122 gameplay or handoff proof.

## Current Manual-Proof Focus

- Vakuu's Sere Talon must offer 4 Curses, choose 1, then add the selected Curse, 2 Wish, and 1 Wish+; its event option, relic bar, inspect screen, hover text, and log routes must not appear as Tanx Claws.
- Tanx Claws must stay on the Tanx route and transform selected cards into upgraded Maul+ / 撕咬+.
- Direct Golden Idol localization cleanup remains a localization-only note; it does not prove gameplay or replace verifier reports.
- Save-load, death/failure, co-op, hover, map traversal, preview tools, gameplay, and gated Vakuu fight-option evidence remain manual rows under `docs/issues.md`, `docs/toreview.md`, and the generated handoff.

## Still Not Claimed

- No live save-load, death/failure, co-op, hover, map traversal, preview-tools, or gameplay proof was produced.
- Current state remains a manual-test candidate, not release-ready.
