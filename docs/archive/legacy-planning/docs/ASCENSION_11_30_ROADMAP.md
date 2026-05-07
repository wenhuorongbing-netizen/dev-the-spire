# Ascension 11-30 Roadmap

This document is a future roadmap only. It does not authorize implementation.

## Current Status
- Current phase: Ancient reward research and design.
- Ascension 11-30 is the second major project target.
- No Ascension code, patches, localization, or balance changes should be implemented yet.

## Why Ancient Reward Tuning Comes First
Ancient rewards affect player power, run pacing, variance, and build direction. Expanding Ascension before understanding Ancient reward balance would make difficulty targets unstable.

Ancient reward work should provide:
- Better estimates of average run power.
- Known high-variance reward cases.
- A clearer view of which rewards scale too well under pressure.
- A safer baseline for later difficulty bands.

## Design Bands

### A11-A20: Pressure Without Obscurity
Goal: add readable, learnable difficulty that tests fundamentals without hiding punishment.

Potential future axes:
- More constrained resources.
- Stronger enemy pressure at clear moments.
- Reward choice tension.
- Pathing pressure.
- Boss or elite pressure with visible rules.

Avoid:
- Invisible numeric punishment stacking.
- Too many small penalties that players cannot attribute.
- Difficulty that invalidates slow or experimental decks by default.

### A21-A30: Expert Structure
Goal: create high-level difficulty that changes planning requirements without becoming arbitrary.

Potential future axes:
- Stronger act-specific constraints.
- More demanding Ancient reward decisions.
- Higher consequence for poor route planning.
- More precise deck-building pressure.
- Optional challenge variants only if technically safe.

Avoid:
- Mandatory narrow builds.
- Randomness that overrides skill.
- Unclear rule changes.
- Save/load or run-state instability.

## Future Risk List

| Risk | Why It Matters | Mitigation |
|---|---|---|
| Difficulty stacks invisibly | Players cannot learn from failure | Prefer explicit rule changes and clear UI text |
| Ancient rewards become mandatory | Difficulty assumes overtuned rewards | Tune rewards first and keep data |
| Character balance diverges | New difficulty may punish some characters disproportionately | Test by character after reward baseline |
| Public beta changes Ascension internals | Implementation may break | Research current API before implementation |
| Patching difficulty flow is unstable | Could affect run creation or saves | Prefer exposed APIs, isolate patches |
| Too much scope | A11-A30 can become a full expansion | Implement in small bands |

## Research Needed Later
- Where Ascension levels are defined.
- How current public beta handles Ascension caps.
- Whether BaseLib exposes Ascension extension points.
- How Ascension changes are displayed to the player.
- Whether save files store Ascension state in a patch-sensitive way.
- How Ascension interacts with unlocks, characters, and daily/custom modes.

## Dependency on Ancient Reward Work
Before Ascension implementation:
- Ancient reward MVP should be tested.
- Reward power outliers should be identified.
- Reward scaling and context sensitivity should be documented.
- At least one compatibility pass should be completed on the target public beta.

## No Implementation Yet
Do not create Ascension code, patches, configs, or localization from this roadmap. A separate Ascension design spec is required before implementation.
