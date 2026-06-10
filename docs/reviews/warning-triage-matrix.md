# Sts1Events Nullable Warning Triage Matrix

Date: 2026-06-10

## Summary

| Code | Count | Description |
|------|-------|-------------|
| CS8604 | 0 | Possible null reference argument for parameter |
| CS8602 | 0 | Dereference of possibly null reference |
| CS8625 | 0 | Cannot convert null literal to non-nullable reference type |
| **Total** | **0** | |

## Root Cause

Every warning traces to one root cause: **`EventModel.Owner` is typed `Player?` (nullable)** from the game's base class, but all event handler methods use `Owner` as if it were non-null.

**Applied fix pattern:** Early-exit guard `if (Owner is not { } owner) return;` at the top of each handler method, then use the non-nullable `owner` local throughout. This pattern has cleared the compile-included Sts1Events nullable warning debt.

## Historical Pre-Fix Per-File Triage

### Act1 (8 warnings)

| File | Warnings | Codes | Category |
|------|----------|-------|----------|
| Sts1DeadAdventurer.cs | 2 | CS8604×2 | player/owner null arg |
| Sts1Joust.cs | 2 | CS8604×2 | player/owner null arg |
| Sts1Mushrooms.cs | 2 | CS8602×2 | dereference after null |
| Sts1TheSsssserpent.cs | 1 | CS8604×1 | player/owner null arg |
| Sts1TreasureOoze.cs | 1 | CS8604×1 | player/owner null arg |

### Act2 (21 warnings)

| File | Warnings | Codes | Category |
|------|----------|-------|----------|
| Sts1Altar.cs | 2 | CS8604×2 | player/owner null arg |
| Sts1Augmenter.cs | 2 | CS8604×2 | player/owner null arg |
| Sts1AncientWriting.cs | 2 | CS8604×2 | player/owner null arg |
| Sts1CouncilOfGhosts.cs | 1 | CS8602×1 | dereference after null |
| Sts1CursedTome.cs | 1 | CS8602×1 | dereference after null |
| Sts1DrugDealer.cs | 1 | CS8604×1 | player/owner null arg |
| Sts1ForgottenAltar.cs | 4 | CS8602×2, CS8604×2 | both |
| Sts1KnowingSkull.cs | 2 | CS8602×2 | dereference after null |
| Sts1MaskedBandits.cs | 1 | CS8604×1 | player/owner null arg |
| Sts1Nest.cs | 1 | CS8604×1 | player/owner null arg |
| Sts1TheGhost.cs | 1 | CS8602×1 | dereference after null |
| Sts1TheLibrary.cs | 2 | CS8602×2 | dereference after null |
| Sts1Vampires.cs | 2 | CS8604×1, CS8602×1 | both |

### Act3 (16 warnings)

| File | Warnings | Codes | Category |
|------|----------|-------|----------|
| Sts1Falling.cs | 3 | CS8604×2, CS8602×1 | both |
| Sts1MindBloom.cs | 2 | CS8602×1, CS8604×1 | both |
| Sts1MoaiHead.cs | 2 | CS8602×1, CS8604×1 | both |
| Sts1SensoryStone.cs | 1 | CS8602×1 | dereference after null |
| Sts1TombOfLordRedMask.cs | 3 | CS8604×3 | player/owner null arg |
| Sts1Transmogrifier.cs | 1 | CS8604×1 | player/owner null arg |
| Sts1WindingHalls.cs | 4 | CS8604×1, CS8602×3 | both |

### Shared (25 warnings)

| File | Warnings | Codes | Category |
|------|----------|-------|----------|
| Sts1BonfireSpirits.cs | 2 | CS8604×1, CS8602×1 | both |
| Sts1Designer.cs | 3 | CS8604×3 | player/owner null arg |
| Sts1FaceTrader.cs | 2 | CS8602×1, CS8604×1 | both |
| Sts1FountainOfCleansing.cs | 2 | CS8602×2 | dereference after null |
| Sts1GoldenWing.cs | 1 | CS8602×1 | dereference after null |
| Sts1LivingWall.cs | 3 | CS8604×3 | player/owner null arg |
| Sts1TheMausoleum.cs | 2 | CS8604×2 | player/owner null arg |
| Sts1TheWomanInBlue.cs | 3 | CS8604×3 | player/owner null arg |
| Sts1WheelOfChange.cs | 6 | CS8604×4, CS8602×2 | both |

## API Methods Receiving Nullable Owner

| Target Method | Parameter | CS8604 Count |
|---------------|-----------|-------------|
| `PlayerCmd.LoseGold` | `Player player` | 14 |
| `Sts1EventHelpers.OpenCardRemoval` | `Player owner` | 7 |
| `Sts1EventHelpers.OpenCardUpgrade` | `Player owner` | 4 |
| `Sts1EventHelpers.GrantRandomRelic` | `Player owner` | 5 |
| `Sts1EventHelpers.OpenCardTransform` | `Player owner` | 5 |
| `PlayerCmd.GainGold` | `Player player` | 4 |
| `Sts1EventHelpers.AddCurses<T>` | `Player owner` | 3 |
| `CardPileCmd.AddCursesToDeck` | `Player owner` | 2 |
| `Sts1EventHelpers.RemoveCardsByTag` | `Player owner` | 1 |

## Decision

Nullable warnings are no longer accepted debt in the current source: the latest forced solution build is 0-warning. Sts1Events remains gated Off/default-staging until gameplay, render, save-load, replacement, multiplayer, and QA proof exist.

## Diagnostics Architecture Audit

| Component | Required Posture | Actual Posture | Compliant? |
|---|---|---|---|
| RewardPipeline | Diagnostics-only | Diagnostics-only (log + evidence only) | YES |
| CardPlayContext | Allow-only | Allow-only (depth guard gate, no state mutation) | YES |
| DeathProtectionService | No-op / diagnostics-only | No-op (zero production callers) | YES |
| MultiplayerPolicy (registry) | Taxonomy / diagnostics-only | Taxonomy store (register/lookup only) | YES |
| MultiplayerFeaturePolicy (coop gates) | Behavioral safety gate | Active feature suppression in co-op | YES (intentional) |
