# Card And Power Safety Rules

Status: mandatory planning rule for Ancient Expansion v2.2 implementation.

## Rule Summary

Extra-play, copy, reprint, verdict, echo, and replay effects may affect Attack and Skill cards. Power cards are not copied, extra-played, or replayed by default.

## Power Fallbacks

When a future effect would otherwise act on a Power card, it must choose one explicitly documented fallback:

- Make the next eligible card cost 0 for that play.
- Draw cards.
- Gain energy.
- Wait for the next Attack or Skill.

The fallback must be visible in text and covered by tests before the blessing enters the active pool.

## Recursion Guard

Extra-played or copied cards must not recursively trigger the same blessing that created them. Future implementations should use an explicit transient marker, scope guard, or command-path flag and add a source guard test for it.

## Current Morvi Prototype

The default-off Morvi Misprint Press prototype follows this rule by replaying only the first Attack or Skill each combat as a generated Exhausting copy. It ignores Power cards, clone cards, and copies created while the blessing is already resolving. If the generated copy cannot enter combat, helper cleanup removes the unpiled clone before the blessing returns.

## Future Implementation Requirements

Each Morvi or Lotha blessing that touches card play must document:

- Eligible card types.
- Power-card fallback.
- Recursion guard.
- Whether generated cards are temporary, copied, transformed, or played through command APIs.
- Save/load stance for any stored card reference.
- Multiplayer ownership stance.

No implementation should rely on broad string checks for card type when the game exposes a structured card type or model API.
