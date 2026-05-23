# Card And Power Safety Rules

Status: mandatory rule for Ancient Expansion v2.2 implementation and hardening.

## Rule Summary

Extra-play, copy, verdict, echo, and replay effects may affect Attack and Skill cards. Power cards are not copied, extra-played, or replayed by default.

## Power Fallbacks

When an effect would otherwise act on a Power card, it must choose one explicitly documented fallback:

- Make the next eligible card cost 0 for that play.
- Draw cards.
- Gain Energy.
- Wait for the next Attack or Skill.

The fallback must be visible in player-facing text and covered by tests before the blessing enters the active pool.

## Recursion Guard

Extra-played or copied cards must not recursively trigger the same blessing that created them. Implementations should use an explicit transient marker, source-supported autoplay flag, or command-path guard and add a source guard test for it.

## Current Morvi Source Slice

The default-on Morvi source slice follows this rule without generated autoplay replays:

- Forbidden Loan marks a real borrowed Ancient deck card. Borrowed Attacks and Skills lose 1 HP when played; borrowed Powers lose 8 HP and are not copied, replayed, or extra-played by Morvi systems.
- Misprint Press uses play-count modification on the first player-played Attack or Skill each turn. It ignores Power cards, Statuses, Curses, autoplay/generated cards, and recursive extra-play executions.
- Blueprint Proof can affect Power cards through temporary upgrade/cost and draw/Block benefits, but never through extra-play, copy, or replay.
- Red Ink Overdraft, Overdue Library, Open-Book Exam, Paperstorm, and Debt Settlement do not copy or extra-play player Power cards.

## Current Lotha Source Slice

The corrective Lotha slice follows this rule without generated autoplay replays:

- Mirror Rebuttal marks one real Attack, Skill, or Power deck card through a source-safe deck selector. The matching combat card is moved to hand on the first player turn after normal draw when needed. If the marked card is an Attack or Skill, `ModifyCardPlayCount` adds one play; if it is a Power, it costs 0 for that play.
- Mirror Hall Echo records the last player-played non-Status Attack, Skill, or Power at player-turn end. The next player turn's first player-played card of that type consumes the echo. Attack/Skill adds one play; Power costs 0 for that play and draws 1. Autoplay and clone plays are excluded from both recording and consuming.
- Deferred Verdict creates player-owned Verdict stacks on turn 4. Each next non-Status card consumes one stack that turn. Attacks and Skills add one play; Powers cost 0 for that play and draw 1 instead.
- Single Sentence adds two plays to the first Attack or Skill each turn, then caps the rest of that turn at four more normal player-played cards. A first Power before that ruling costs 0 for that play and draws 1 without consuming the sentence.
- Public Evidence is not a card-copy rule. It uses source power-amount hooks to double non-damaging negative status applications and manage Enlightenment, then consumes up to three Enlightenment at turn start for draw and Block. Poison, damage-over-time, countdown damage, and source-proven damage/kill Debuffs are excluded.

## Future Implementation Requirements

Each Morvi or Lotha blessing that touches card play must document:

- Eligible card types.
- Power-card fallback.
- Recursion guard.
- Whether generated cards are temporary, copied, transformed, or played through command APIs.
- Save/load stance for any stored card reference.
- Multiplayer ownership stance.

No implementation should rely on broad string checks for card type when the game exposes a structured card type or model API.
