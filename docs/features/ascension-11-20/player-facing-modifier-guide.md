# Ascension Modifier Preview Guide

Status: player-facing guide for the current A12/A16/A19/A20 source pass. Live UI verification is still pending.

## Firemarked Elite

Firemarked Elite nodes use the Firemarked Elite map marker. Hover the node before choosing the route to see the exact Firemark:

- Might: the Firemark Host starts with Strength and can overflow temporary Strength to one attacking secondary enemy.
- Giant: the Firemark Host starts with increased max HP; breaking Molten Core splashes one secondary enemy.
- Forge Armor: the Firemark Host gains Molten Armor at player turn start. Clear all host Block by turn end to skip the next armor gain.
- Constant Heal: the Firemark Host heals at enemy turn end. If that heal succeeds, one damaged secondary enemy also heals.

The combat effect should match the hover text. Defeating the node still grants the Firemarked Elite reward upgrades documented in the manual checklist.

## Banner Room

Banner Rooms are marked normal combats. Hover the node before route commitment to see the exact Banner:

- Vanguard: enemies start with temporary Strength, which expires at round 3.
- Shield Formation: one bannerbearer protects other enemies while it lives.
- Bounty: kill the marked target before the deadline for extra Gold; if it survives, it gains protection.

Unmarked normal combats should not receive Banner effects.

## Boss Dedicated Ability

At A19, Boss map nodes preview their dedicated ability on hover. The hover should show the ability name and the matching summary before combat. Aeonglass uses Time Sand Reflow: after Ebb, shared Time Sand appears; spending energy clears it, and any remaining Time Sand makes the next Increasing Intensity add extra Wither.

## Branded Form

At A20 single-player, the second Boss enters Branded Form. Hover the second Boss node to see the stronger ability summary before committing.

A20 multiplayer selection is still a development test surface and is not full Branded Form co-op support.

## Fission Reward Enchantment

Fission reward enchantment appears only on eligible reward cards.

Fission is reward-only and remains probabilistic:

- normal combat: 25%
- Banner Room: 35%
- Firemarked Elite: 40%
- Boss: 15%

Fission can only appear on eligible Attack or Skill rewards. With `EZMB_ASCENSION_DIAGNOSTICS=1`, logs show source label, chance, eligible candidate count, roll, applied yes/no, and card id when applied.

## Map Hover Previews

Map hover previews are the source of truth before entering a marked room:

- Firemarked Elite hover shows the exact Firemark kind.
- Banner Room hover shows the exact Banner kind.
- Boss hover shows the dedicated ability or Branded Form summary.

If a marker appears but hover text is missing, raw, or disagrees with combat behavior, record the screenshot and `godot.log` as manual evidence.
