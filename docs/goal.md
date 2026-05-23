# Spire Plus Goal Guard

This file is a compact guardrail for current `/goal` work. The active long-form development target is `docs/test-ready-development-goal.md`.

## Current Goal

Keep `Spire Plus` as a test-ready manual build for Slay the Spire 2 `v0.106.0` with BaseLib `v3.1.4`.

The active deliverable is one mod:

- player-facing name: `Spire Plus`
- stable technical id: `EZMicroBalance`
- package: `publish/SpirePlus-v0.1.0-private-beta.0.zip`

## Runtime Rows

Runtime rows are not closed by source review. Every row below needs live proof required from game logs, screenshots, manual notes, or two-client evidence:

- live loader parity for the current package;
- clicked Ancient UI and hover readability;
- Urda, Morvi, Lotha, and Vakuu gameplay;
- save/load for Ancient state, Root Sight, Seed Bank, Rootblight, Morvi, Lotha, and Vakuu;
- Vakuu victory return, no-black-screen, failure, and death paths;
- A11-A20 map traversal and combat behavior;
- co-op ownership and desync checks;
- Crystal Sphere and transform-preview live proof inside Spire Plus.

## Source Review

Source review may close only source-level issues: compile errors, stale API signatures, broken localization keys, missing resource paths, manifest drift, hash drift, and guard-test failures.

Source review must not claim live gameplay success. No release-ready claim is made until the runtime rows above have direct evidence.

## Current Stop Line

Deliver a clean local package for user testing. Keep release, co-op, save/load, and no-black-screen claims pending until the user supplies runtime evidence.
