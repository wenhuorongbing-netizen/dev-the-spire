# Sts1Events Nullable Warning Triage Matrix

Date: 2026-06-02

## Summary

| Code | Count | Description |
|------|-------|-------------|
| CS8604 | 54 | Possible null reference argument for parameter |
| CS8602 | 34 | Dereference of possibly null reference |
| CS8625 | 1 | Cannot convert null literal to non-nullable reference type |
| **Total** | **89** | |

## Root Cause

Every warning traces to one root cause: **`EventModel.Owner` is typed `Player?` (nullable)** from the game's base class, but all event handler methods use `Owner` as if it were non-null.

**Recommended fix pattern:** Early-exit guard `if (Owner is not { } owner) return;` at the top of each handler method, then use the non-nullable `owner` local throughout. This single pattern eliminates all 89 warnings.

## Per-File Triage

### Act1 (9 warnings)

| File | Warnings | Codes | Category |
|------|----------|-------|----------|
| Sts1DeadAdventurer.cs | 2 | CS8604×2 | player/owner null arg |
| Sts1Joust.cs | 2 | CS8604×2 | player/owner null arg |
| Sts1Mushrooms.cs | 2 | CS8602×2 | dereference after null |
| Sts1ShiningLight.cs | 1 | CS8602×1 | dereference after null |
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

### Act3 (17 warnings)

| File | Warnings | Codes | Category |
|------|----------|-------|----------|
| Sts1Falling.cs | 3 | CS8604×2, CS8602×1 | both |
| Sts1MindBloom.cs | 2 | CS8602×1, CS8604×1 | both |
| Sts1MoaiHead.cs | 2 | CS8602×1, CS8604×1 | both |
| Sts1SensoryStone.cs | 1 | CS8602×1 | dereference after null |
| Sts1TombOfLordRedMask.cs | 3 | CS8604×3 | player/owner null arg |
| Sts1Transmogrifier.cs | 1 | CS8604×1 | player/owner null arg |
| Sts1UpgradeShrine.cs | 1 | CS8604×1 | player/owner null arg |
| Sts1WindingHalls.cs | 4 | CS8604×1, CS8602×3 | both |

### Shared (42 warnings)

| File | Warnings | Codes | Category |
|------|----------|-------|----------|
| Sts1BigFish.cs | 3 | CS8602×2, CS8604×1 | both |
| Sts1BonfireSpirits.cs | 2 | CS8604×1, CS8602×1 | both |
| Sts1Designer.cs | 3 | CS8604×3 | player/owner null arg |
| Sts1DivineFountain.cs | 1 | CS8602×1 | dereference after null |
| Sts1FaceTrader.cs | 2 | CS8602×1, CS8604×1 | both |
| Sts1FountainOfCleansing.cs | 2 | CS8602×2 | dereference after null |
| Sts1GoldenIdol.cs | 5 | CS8602×3, CS8604×1, CS8625×1 | all three |
| Sts1GoldenShrine.cs | 3 | CS8602×1, CS8604×2 | both |
| Sts1GoldenWing.cs | 1 | CS8602×1 | dereference after null |
| Sts1LivingWall.cs | 3 | CS8604×3 | player/owner null arg |
| Sts1OldBeggar.cs | 1 | CS8604×1 | player/owner null arg |
| Sts1Purifier.cs | 1 | CS8604×1 | player/owner null arg |
| Sts1TheCleric.cs | 2 | CS8604×2 | player/owner null arg |
| Sts1TheLab.cs | 1 | CS8604×1 | player/owner null arg |
| Sts1TheMausoleum.cs | 2 | CS8604×2 | player/owner null arg |
| Sts1TheWomanInBlue.cs | 3 | CS8604×3 | player/owner null arg |
| Sts1WheelOfChange.cs | 6 | CS8604×4, CS8602×2 | both |

## API Methods Receiving Nullable Owner

| Target Method | Parameter | CS8604 Count |
|---------------|-----------|-------------|
| `PlayerCmd.LoseGold` | `Player player` | 17 |
| `Sts1EventHelpers.OpenCardRemoval` | `Player owner` | 7 |
| `Sts1EventHelpers.OpenCardUpgrade` | `Player owner` | 6 |
| `Sts1EventHelpers.GrantRandomRelic` | `Player owner` | 6 |
| `Sts1EventHelpers.OpenCardTransform` | `Player owner` | 5 |
| `PlayerCmd.GainGold` | `Player player` | 4 |
| `Sts1EventHelpers.AddCurses<T>` | `Player owner` | 3 |
| `CardPileCmd.AddCursesToDeck` | `Player owner` | 2 |
| `Sts1EventHelpers.RemoveCardsByTag` | `Player owner` | 1 |
| `Sts1EventHelpers.RemoveAllCurses` | `Player owner` | 1 |
| `Sts1EventHelpers.GrantRandomPotion` | `Player owner` | 1 |
| `RelicFactory.PullNextRelicFromFront` | `Player player` | 1 |

## Decision

Warnings are accepted for now because Sts1Events is gated Off by default and still prototype/dev-only outside Canary/Batch1 test modes. All warnings will be resolved when Sts1Events moves toward AdditiveBatch1 readiness by applying the early-exit guard pattern.

## Diagnostics Architecture Audit

| Component | Required Posture | Actual Posture | Compliant? |
|---|---|---|---|
| RewardPipeline | Diagnostics-only | Diagnostics-only (log + evidence only) | YES |
| CardPlayContext | Allow-only | Allow-only (depth guard gate, no state mutation) | YES |
| DeathProtectionService | No-op / diagnostics-only | No-op (zero production callers) | YES |
| MultiplayerPolicy (registry) | Taxonomy / diagnostics-only | Taxonomy store (register/lookup only) | YES |
| MultiplayerFeaturePolicy (coop gates) | Behavioral safety gate | Active feature suppression in co-op | YES (intentional) |
