# Spire Plus Goal Guard

Current target: test-ready manual build, not release-ready.

## Active implementation notes

- Keep player-facing name `Spire Plus`; keep `EZMicroBalance` only where it is the stable technical manifest id, resource folder, namespace, saved-field prefix, or legacy environment alias.
- Keep Vakuu's Sere Talon separate from Tanx Claws: Sere Talon offers 4 Curses, choose 1, then adds 2 Wish and 1 Wish+; Tanx Claws remains the Maul+ transform relic.
- Keep recent source-level polish focused: A20 selector localization, direct-gain feedback, Elite Root payoff feedback, Seedbed / Planting clarity, light elite damage reduction, co-op fail-closed hardening, and Royal Decree safety.
- Crystal Sphere and transform-preview live proof inside Spire Plus is still required; source review alone does not prove multiplayer or reconnect safety.
- Archive long prompt dumps under `docs/archive/feature-inputs/`; see `goal-md-mojibake-intake-20260523.md`, `goal-coop-preview-plan-20260525.md`, and `goal-preview-plan-intake-20260526.md`.

## Closure rules

Closure rules:

- Live proof required: loader, clicked UI, gameplay, save/load, failure/death, and co-op rows close only with current-package evidence.
- Source review may close only source-level issues, source guards, docs drift, package parity, and static governance items.
- Runtime rows need game logs, screenshots, manual notes, or two-client evidence from the current beta package.
- No source-only pass may mark this goal complete.
- No release-ready claim is made while manual proof gates remain pending.
