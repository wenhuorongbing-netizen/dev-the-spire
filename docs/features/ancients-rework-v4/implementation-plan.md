# Spire Plus - Ancients Rework v4.3 Implementation Plan

Source brief: `docs/features/ancients-rework-v4/source-design.md`  
Planning date: 2026-05-05  
Status: historical implementation plan with current v4.3 addendum. The Ancient rebalance is implemented in the active independent `EZMicroBalance` project under `EZMicroBalanceCode/Ancients/`; old `EzDailyContentCode` path references in this plan describe the pre-migration implementation pass and are not current release instructions. v4.3 is current.

## 1. Intended Mod Identity

`Spire Plus` is a Slay the Spire 2 balance mod for Ancient reward choices and progression expansion testing. It should not add unrelated playable systems unless a specific rework requires one.

The design target is high-level play. The mod should make Ancient choices feel like deliberate strategic commitments instead of random punishment or obvious best picks:

- Vaku becomes a readable "demon contract": high risk, high ceiling, and planable downsides.
- Strong rewards keep their memorable payoff, but their trigger threshold, timing, or extra freebies are tuned down.
- Weak rewards gain agency, guaranteed value, or controllable deck pollution instead of flat number buffs.
- Low-frequency decisions can be deep; high-frequency combat triggers must stay quick.
- The player-facing question should become: "Can this run support this direction?" rather than "Which option hurts least?"

Repository naming constraint from the historical planning pass: the legacy `EzDailyContent` manifest id must not be changed in-place. The active private-beta project now uses the separate, stable manifest id `EZMicroBalance`.

## v4.3 Addendum

The current v4.3 implementation updates and supersedes the v4.2 adjustment points where they conflict:

- `Velvet Choker`: retains the v4.2 soft limit. It grants +1 Energy, allows every card to be played, and makes the 7th and later manual from-hand card plays each turn cost +1 after other cost changes. Copied, autoplayed, and repeated card-play instances do not advance the counter.
- `Distinguished Cape`: uses `max(ceil(currentMaxHp * 0.30), 18)` and can be selected only when current max HP is greater than that cost. It then loses max HP without damage interaction and adds three `Apparition` cards.
- `Prismatic Gem`: Every second standard card reward contains only off-color cards. The trigger decision is scoped to the `CardReward` screen so reroll cannot advance or lose the saved counter; all visible slots are replaced on trigger screens.
- Simplified Chinese player-facing number formatting removes spaces between Chinese text, numbers, and units.
- v4.2 rightmost-slot Prismatic Gem is historical only.
- v4.2 Distinguished Cape 40% min15 is historical only.

## 2. Current Repository Baseline

Original observed code state:

- `EzDailyContentCode/MainFile.cs` creates a Harmony instance and calls `PatchAll()`.
- `EzDailyContentCode/Cards`, `Powers`, and `Relics` contain abstract template scaffolding only.
- At the original planning pass, `EzDailyContentCode/AncientRewardNoopProbe.cs` was the only Ancient-specific code.
- The probe patches `AncientEventModel.GenerateInitialOptionsWrapper` as a postfix and logs generated option metadata.
- The probe began as no-op telemetry. Release readiness requires either removing it or gating it so normal player builds are not noisy.

Planning-pass probe improvement:

- The probe now logs a safe summary of the relic object behind each option: runtime type plus public `Id`, `Name`, `Entry`, `TextKey`, `LocalizationKey`, `Rarity`, and `Pool` values when present.
- `dotnet build` passed after this change on 2026-05-05 with 0 warnings and 0 errors.

Current implementation note:

- The historical `EzDailyContentCode/Ancients/AncientRewardBalancePatches.cs` and `EzDailyContentCode/Ancients/PaelsHornPhase1Patch.cs` paths are no longer active release code.
- The active release implementation is already isolated in the independent `EZMicroBalance` project under `EZMicroBalanceCode/Ancients/`.

## 3. API Discovery Needs

The current probe should answer the first discovery question: which Ancient option text keys and relic objects appear in the initial choice list. That is necessary but not sufficient for implementation.

Before gameplay changes, discover and document the following in `docs/features/ancients-rework-v4/api-discovery.md`:

1. Ancient option identity
   - Stable text keys, relic ids, and runtime types for each target reward.
   - Whether options are generated from static data, model constructors, or factory methods.
   - Whether a reward can be safely replaced, wrapped, or patched at application time.

2. Reward application flow
   - The method called when a player chooses an Ancient option.
   - Whether there are command APIs for adding cards, removing cards, upgrading cards, adding relics, granting resources, and opening selection screens.
   - How option choice history is saved.

3. Card and keyword mutation
   - How to create existing game cards by id.
   - How to mark an instance upgraded.
   - Whether keywords such as `Innate`, `Ethereal`, `Retain`, `Exhaust`, and `Eternal` are static model data or instance modifiers.
   - How to add custom curse cards only when a phase explicitly needs them.

4. Combat hooks
   - Safe hook points for combat start, turn start, after draw before action, card played, card exhausted, elite killed, act start, rest-site open, and card reward generation.
   - Preferred command APIs over direct state mutation.
   - Save data support for counters and marked card instances.

5. UI hooks
   - Existing card/relic selection screens.
   - Lightweight binary choice screens for per-turn choices.
   - Combat-start five-choice card reward screens.
   - Tooltip/highlight surfaces for counters.

## 4. Implementation Principles

- Implement one small playable slice at a time.
- Prefer existing game cards, relics, and command flows before adding custom content.
- Do not copy decompiled game logic into this repository.
- Keep every phase buildable. Run `dotnet build` after code changes.
- Run `dotnet publish` only after resource, localization, packaging, or manifest changes, and only if build succeeds.
- Document API findings and validation status as each phase changes.
- Keep `affects_gameplay` true for this mod.

## 5. Safest First Playable Increment

Phase 1 should implement `Pael's Horn` only:

> `Pael's Horn`: add 1 `Relax` and 1 `Relax+` to the deck.

Why this is the safest first playable increment:

- It is a finalized design entry in the source brief.
- It is pickup-only and does not require combat triggers, rest-site hooks, reward-slot rewriting, map hooks, or persistent per-combat counters.
- It can use existing game card definitions if `Relax` already exists.
- It validates the core mod pipeline: identify an Ancient option, intercept or wrap its reward behavior, create an existing card, upgrade one instance, add both through supported APIs, build, publish if needed, and manually verify in-game.
- It has low save compatibility risk because it only affects new pickups.

Out of scope for phase 1:

- Vaku reward behavior.
- Custom cards such as `Debt`.
- Choice screens.
- Combat counters.
- Reward-generation replacement.
- Manifest rename or author changes.

If `Pael's Horn` cannot be safely identified or intercepted after discovery, stop and document the blocker instead of switching to a larger feature silently.

## 6. Phased Milestones

### Phase 0 - Planning and No-op Discovery Readiness

Goal: turn the source brief into implementation steps without adding balance behavior.

Deliverables:

- `implementation-plan.md`.
- No-op Ancient reward probe logs sufficient to identify generated options and associated relic objects.
- Build result recorded if code changes.

Exit gate:

- `dotnet build` passes if probe code changes.
- No Ancient reward behavior is modified.

### Phase 1 - Pickup-only First Playable Slice

Goal: implement and verify `Pael's Horn` as the smallest real balance change.

Implementation tasks:

- Re-read the source brief section `4.2 佩尔之角`.
- Capture probe logs for the Pael Ancient options.
- Document the option text key, relic id/type, and chosen patch point in `api-discovery.md`.
- Implement only the modified `Pael's Horn` reward.
- Use supported APIs to add one normal `Relax` and one upgraded `Relax` instance (`Relax+` in game display).
- Historical-only instruction: preserve the then-active `EzDailyContent` scaffold id during the original phase-1 probe. Current release work must keep the independent `EZMicroBalance` manifest id stable.

Validation:

- `dotnet build`.
- `dotnet publish` only if packaging/resource changes are made or if manual verification requires a fresh installed artifact.
- Manual test: start a run that can select `Pael's Horn`, choose it, and confirm the deck receives exactly one `Relax` and one `Relax+`.

Definition of done:

- No other Ancient rewards change.
- The implementation is documented in `api-discovery.md` and `docs/dev-environment.md`.
- Known game/API limitations are listed.

### Phase 2 - Other Low-hook Pickup Rewards

Goal: implement rewards that mainly alter pickup results and can be verified without complex combat systems.

Candidate order:

1. `Black Star`: if gained in act 3, immediately grant 1 random relic.
2. `Pael's Horn`: already completed in phase 1.
3. `Warhammer`: on pickup choose 2 cards to upgrade; keep elite-kill random upgrades unchanged.
4. `Jeweled Box / Apotheosis`: add Apotheosis without Innate, only if instance-level keyword removal is proven.
5. `Pickled Living Fog / Folly`: remove 4 cards and add Folly with Innate + Eternal but no Ethereal/Retain, only after keyword mutation is proven.
6. `SereTalon` / 瓦库原初之爪: keep the source behavior, adding 2 random Curses and 3 `Wish`. Keep `Claws` (`Tanx Claws` / 坦克斯利爪 in player text) on the Tanx Maul-transform path; selected cards now become upgraded Maul+ / 撕咬+.

Validation focus:

- Deck mutation.
- Upgrade selection.
- Existing-card creation.
- Keyword changes on card instances.
- Save/load behavior for newly added cards.

### Phase 3 - Simple Combat Counters

Goal: implement deterministic combat hooks with minimal UI.

Candidate order:

1. `Iron Club`: every 5 cards played, draw 1; no per-turn cap.
2. `Brilliant Scarf`: every turn, the 6th card is free.
3. `Music Box`: first player attack each turn creates a temporary discounted copy.
4. `Bloodstained Rose / Enthralled`: add 10 block to `Enthralled` when played while preserving forced-priority behavior.
5. `Golden Seal / Debt`: add `Debt` only after custom curse registration and exhaust-listener safety are proven.

Validation focus:

- Card played event order.
- Generated card ownership and cleanup.
- Cost modification duration.
- Draw command behavior.
- Counter UI or tooltip state.

### Phase 4 - Choice UI and Marked-card Systems

Goal: implement effects that require player selection, card marking, or per-combat choice screens.

Candidate order:

1. `Jeweled Mask`: choose or draft a power card, set it to 0 cost, and move it from draw pile to hand at combat start.
2. `Choice Paradox`: combat-start five-choice filtered rare card draft, temporary retain, remove at combat end.
3. `Baking Gloves`: per-turn top-deck consume/keep choice.
4. `Crossbow`: per-turn take/skip random attack.
5. `Pael's Tooth`: remove 5, return one upgraded removed card every 2 fights, clear remaining after act boss.

Validation focus:

- Choice-screen APIs.
- Marked card persistence.
- Temporary card cleanup.
- Combat-start timing.
- Repeated-choice fatigue.

### Phase 5 - Reward, Rest-site, Act, and Resource Hooks

Goal: implement features that change systems outside normal pickup/combat card flow.

Candidate order:

1. `Sozu` class: fill empty potion slots on pickup, then block future potion gain.
2. `Ectoplasm` class: grant 250 gold on pickup, then block future gold gain.
3. `Pumpkin Candle`: no active EZMB override. Vanilla behavior is restored; keep only a no-override spot check in the manual matrix.
4. `Meat Cleaver`: rest-site Cleaver / 切肉 action removes 2 cards and costs 5 HP.
5. `Prismatic Gem`: every second standard card reward changes all visible card options to off-color cards.

Validation focus:

- Resource gain hooks.
- Act transition hooks.
- Rest-site custom action lifecycle.
- Card reward generation and reroll behavior.
- Prismatic Gem balance telemetry because the v4.3 all-off-color trigger remains marked as重点待测 in the source brief.

### Phase 6 - Integration, Balance, and Release Readiness

Goal: verify the mod as `Spire Plus` across all implemented phases while preserving manifest id `EZMicroBalance`.

Deliverables:

- Updated player-facing localization for changed reward text.
- Updated `docs/test-plan.md` or feature-specific test matrix.
- Updated `docs/dev-environment.md` with latest build/publish results.
- Manual verification notes for representative runs.
- Release checklist updates if the user asks to package or publish.

Validation:

- `dotnet build`.
- `dotnet publish`.
- Manual game launch and Mod Settings verification.
- In-game smoke tests for every implemented reward.
- Log inspection for `EZMicroBalance`, `BaseLib`, `error`, and `exception`; legacy `EzDailyContent` should be disabled or absent during private-beta testing.

## 7. Historical Phase-1 /goal Prompt

The prompt below is preserved for traceability only. Do not execute it as current release work; it predates the independent `EZMicroBalance` migration and the completed v4.3 implementation.

```text
/goal Implement phase 1 of docs/features/ancients-rework-v4/implementation-plan.md for the Slay the Spire 2 mod named "Spire Plus" with manifest id "EZMicroBalance".

Read AGENTS.md first. Preserve the dirty worktree. Do not implement any behavior outside phase 1.

Phase 1 target: the safest first playable increment, Pael's Horn. Change only that Ancient reward so it adds 1 Relax and 1 Relax+ to the deck.

Tasks:
1. Re-read docs/features/ancients-rework-v4/source-design.md and docs/features/ancients-rework-v4/implementation-plan.md.
2. Use or extend no-op logging only as needed to identify the Pael's Horn Ancient option/reward id, text key, runtime type, and reward application hook.
3. Create or update docs/features/ancients-rework-v4/api-discovery.md with the actual API findings and chosen patch point.
4. Implement only the Pael's Horn phase 1 behavior through supported game/BaseLib/template APIs; prefer command APIs over direct state mutation.
5. Historical instruction from the scaffold phase: keep manifest id EzDailyContent and do not add unrelated cards, relics, assets, or other Ancient reward changes. Current release work instead keeps manifest id EZMicroBalance.
6. Run dotnet build. Do not continue to publish if build fails.
7. Run dotnet publish only if packaging/resource/manifest changes are made or if manual game verification needs refreshed installed artifacts.
8. Update docs/dev-environment.md with build/publish status and blockers.
9. Finish with concise manual test steps for verifying Pael's Horn in game.
```
