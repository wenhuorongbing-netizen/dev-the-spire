# Ancient Expansion v2.2 Implementation Plan

Status: Urda v2.2 is source-complete/default-on with ten blessing ids, Morvi v2.2 is source-complete/default-on for direct private-beta testing, Lotha is source-complete/default-on for direct testing, and Vakuu fight is hidden by default with a dedicated source enemy/scene plus Contract, Stolen Vault, and Blood Debt pressure. All remain live-pending.

## 0. Evidence First

For every future implementation slice:

1. Read `PROJECT_STATE.md`, `AGENTS.md`, and this feature folder.
2. Inspect local `source code/src/Core/` for the exact game flow.
3. Inspect BaseLib/RitsuLib/template APIs and prefer supported APIs over Harmony patches.
4. Record source evidence in `api-research.md`.
5. Add source guard tests before or with implementation.
6. Add manual checklist rows before claiming the feature is playable.

## 1. Current Pass Boundary

Current hard boundaries for claims:

- Do not expand Lotha beyond the current eight-blessing test slice until live UI/gameplay/save-load evidence exists.
- Do not expose or expand Vakuu fight beyond the current hidden opt-in single-player slice until live UI/gameplay/save-load/failure evidence proves the dedicated source enemy/scene path is stable.
- Do not expand Urda beyond the current eleven blessing ids in this cycle.
- Morvi is default-on in the current private-beta test slice with all eight v2.2 blessing ids, event art, option/icon art, English/zhs localization, hover powers, disable gates, force-Ancient gates, and force-blessing gates. Live gameplay, save/load, and co-op evidence remain pending.
- Do not change Ascension, Rootblight, boss dedicated abilities, Fission, or multiplayer gameplay in this v2.2 Ancient pass.
- Publish/package unless resource/localization/package inputs changed and the required build succeeds first.

Allowed current-pass Urda work:

- Fix source-level bugs in the eleven active Urda blessings.
- Add source/localization/docs guards for those ten blessings.
- Keep all live gameplay and save/load verification rows open until actually tested.

Urda current status:

- Urda appears in Act 1 by default unless `SPIREPLUS_DISABLE_URDA=1` is set. Legacy `EZMB_DISABLE_URDA=1` still works.
- `SPIREPLUS_FORCE_ANCIENT=URDA` and `SPIREPLUS_FORCE_URDA_BLESSING` support focused testing. Legacy `EZMB_*` aliases still work.
- All eleven Urda blessing ids are implemented with source guards.
- Source-safe deviations are documented in the Urda manual checklist and work log: Trial Branch uses a simple 4-card selection grid; Shallow-Root Relic has deterministic Act 2 removal/refund instead of an unproven settlement choice; Rooted Route auto-marks a reachable normal-combat node without map graph mutation; Root-Sight uses the Root Eyes relic to choose a future reachable Monster, Unknown, or Elite room; Seed Bank stores by consuming the reward.

Morvi current status:

- Morvi appears in Act 2 by default unless `SPIREPLUS_DISABLE_MORVI=1` is set. Legacy `EZMB_DISABLE_MORVI=1` still works.
- `SPIREPLUS_FORCE_ANCIENT=MORVI` and `SPIREPLUS_FORCE_MORVI_BLESSING` support focused testing. Legacy `EZMB_*` aliases still work.
- All eight v2.2 blessing ids are implemented with source guards.
- Source-safe deviations are documented in the manual checklist and work log: Forbidden Loan auto-settles after the Act 2 boss instead of opening a post-boss choice; Red Ink Overdraft uses a generated 0-cost temporary action card instead of a native combat button, skips generation when the hand is full, verifies the generated card actually lands in hand, and uses nonlethal HP fallback for unpaid debt; Open-Book sealed cards are held through the exhaust pile and return on turn 3 only if hand space allows; Blueprint Proof uses reversible source upgrade/downgrade commands where possible.

Lotha current status:

- Local Core source supports Act 3 Ancient pool inspection (`Glory.GetUnlockedAncients`) and combat hooks. Lotha now has a Control-based custom Ancient scene, original procedural event art, option marker relic art, all eight source-safe v2.2 blessings, source guards, and disable/force gates. Death Reprieve has one documented source-safe timing deviation: enemy-turn lethal starts the reprieve at the next player turn, while player-turn lethal starts immediately. Live UI/gameplay/save-load/lethal-path evidence remains pending.

Vakuu current status:

- Local Core source supports a hidden opt-in single-player slice that enters a custom `RoomType.Monster` combat from Vakuu with normal rewards disabled, resumes the parent event on victory, and offers extra non-Vakuu Act 3 Ancient blessing choices based on how many Stolen Vault locks the player breaks. The transition is awaited, normal combat rewards are disabled, a no-unclaimed-blessings fallback prevents an empty victory state, the parent event `Node` is cleared before child combat to address the reported post-victory black screen risk, and the prefinished restore path skips normal combat rewards and the duplicate reconstructed-parent Ancient heal before resuming the parent event. The fight now offers 0-cost Contract choices after the normal hand draw on turns 1, 3, and 5; broken locks create 50 loot Gold each, Blood Debt removes 15 loot Gold per stack at victory, and unpaid debt costs nonlethal HP. Cash Out is offered after a lock break, including a hand-full immediate choice fallback. The slice requires preferred `SPIREPLUS_ENABLE_VAKUU_FIGHT=1`, a force-fight gate, or legacy `EZMB_ENABLE_VAKUU_FIGHT=1`, and uses a dedicated Vakuu monster plus custom encounter scene. Save/load during the parent-linked child-combat flow remains a runtime risk; live UI/gameplay/save-load/failure and co-op evidence remain pending.

## 1.5 Next Full-Implementation Track

The next development pass should not create more audit files before improving the playable build. Use `docs/test-ready-development-goal.md` as the controlling prompt and implement toward a release-candidate-quality test build:

1. Replace temporary art with final original art:
   - bespoke Lotha option/relic/card/power art;
   - Morvi event, map, option/relic, and card/status art finalization;
   - Urda option/card polish for Withered Husk and temporary v2.2 blessing art;
   - Vakuu fight option art and bespoke Contract/power art.
2. Verify Lotha mechanics live instead of claiming release readiness from source guards:
   - Mirror Rebuttal selected-card first-turn pull and cost-0 Power fallback;
   - Mirror Hall Echo turn-end type recording and next-turn one-shot trigger;
   - Presumption Innocent state and conservative enemy attack damage break;
   - Closed Court card-reward suppression that leaves gold, potion, and relic rewards intact;
   - Deferred Verdict turn-4 Verdict cleanup;
   - Death Reprieve player-turn and enemy-turn lethal paths;
   - Single Sentence four-card cap and autoplay/clone exclusions;
   - Public Evidence non-damaging negative status handling with Poison/damage-like Debuff exclusions.
3. Add source-compatible rich text and hover explanations:
   - Gold, upgraded card, Skill/Attack/Power, cost/energy, Exhaust, temporary status, Verdict, Evidence, Debt, Seed, Root, and Ancient-specific mechanics should be highlighted or explained using the local source text policy.
4. Live-verify Morvi's already promoted source slice:
   - all eight v2.2 Morvi blessings, gates, state, localization, option art, and save/load stance are source-present;
   - preserve Power-card safety and nonrecursive extra-play rules while replacing temporary art.
5. Live-verify Urda's eleven source-backed blessings and tighten any source-safe deviation that runtime evidence proves can support richer UI.
6. Live-verify Vakuu fight failure/victory clarity, Contract hand injection, lock breaks, Blood Debt scaling, bonus Gold, and victory choice count behavior.
7. Refresh build, publish, package, and then live/manual validation evidence before any release-ready claim.

## 2. Future Task Packet Template

Each blessing implementation packet should contain:

- User-facing rule text.
- Exact source hooks and API evidence.
- Data/state model.
- Disable gate.
- Localization keys.
- Save/load plan.
- Multiplayer ownership stance.
- Source guard test.
- Manual test rows.
- Rollback plan.

## 3. Recommended Future Order

1. Fix visible art/text regressions first, because the user is testing the current build and unclear UI text blocks meaningful feedback.
2. Live-verify Lotha against the documented source-safe v2.2 behavior, including the Death Reprieve timing deviation.
3. Live-verify Morvi's full source slice, including Red Ink/Open Book restore-sensitive rows and Debt Settlement nonlethal HP fallback.
4. Live-verify Urda's eleven-blessing source slice, including option icon sizing and hover readability.
5. Revisit Vakuu/Contract/power art only if live screenshots show readability or routing problems.
6. Run live/save-load/co-op/manual validation and update release docs with actual evidence.

## 4. Acceptance Pattern

A future feature is not complete until all of these are true:

- `dotnet build EZMicroBalance.sln` passes.
- `dotnet test EZMicroBalance.sln --no-build` passes.
- Relevant localization/source guards pass.
- Manual checklist rows are updated truthfully.
- Runtime logs show no related exception.
- Release docs do not overclaim the feature.

