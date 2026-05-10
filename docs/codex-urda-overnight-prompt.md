You are a world-class software engineer and senior Slay the Spire 2 / Godot / .NET mod developer.

You are in:

D:\Game\FOTN\dev-the-spire

Goal: implement a directly playable **Urda-only Ancient expansion vertical slice** overnight.

Scope is deliberately narrow:

- Implement **one new Act 1 Ancient only**:
  - English: `Urda, Loamweaver`
  - Chinese: `息壤织母·乌尔妲`
  - Stable ancient id: `EZMB_URDA`
- Do **not** implement Morvi.
- Do **not** implement Lotha.
- Do **not** implement Vakuu fight.
- Do **not** add A21-A30.
- Do **not** add custom characters.
- Do **not** copy official game assets.
- Do **not** claim release ready unless all live gates actually pass.

The user supplied a full Ancient Expansion design, but this overnight run must only implement Urda and the code/framework necessary to make Urda playable.

Start by updating `docs/issues.md` with the Urda issues from the provided addendum. If the issues are already present, merge status carefully without duplicating sections.

Must read first:

1. AGENTS.md
2. docs/skills/sts2-godot-mod-development.md
3. docs/style/card-localization-style-guide.md
4. docs/issues.md
5. docs/rc1-live-validation-log.md
6. docs/private-beta-verification-handoff.md
7. docs/features/ancients-rework-v4/source-design.md
8. docs/features/ancients-rework-v4/api-discovery.md
9. docs/features/ancients-rework-v4/work-log.md
10. docs/features/ascension-11-20/api-research.md
11. EZMicroBalanceCode/Ancients/**
12. EZMicroBalanceCode/Ascension/**
13. EZMicroBalance/localization/**
14. tests/EZMicroBalance.Tests/**
15. source code/src/Core/** — current v0.105.0 source is primary evidence.
16. BaseLib / template references if local.

Hard rules:

- Local `source code/src/Core/` is primary evidence.
- Before patching Ancient offers, reward screens, cards, combat, map, save/load, or hooks, inspect local source and write evidence to `docs/features/ancient-expansion-urda/api-research.md`.
- Prefer BaseLib/template APIs.
- Use Harmony only when no safer API exists.
- Prefer command APIs over direct state mutation.
- Do not copy large decompiled source bodies into docs.
- Keep every unsafe blessing disabled rather than shipping a broken option.
- No release-ready claim unless build/test/package/live gates all pass.

Create feature docs:

Create `docs/features/ancient-expansion-urda/` with:

- `README.md`
- `source-design.md`
- `implementation-plan.md`
- `api-research.md`
- `manual-test-checklist.md`
- `work-log.md`

These docs must record:

- Urda-only scope.
- Morvi/Lotha/Vakuu explicitly out of scope.
- active blessing pool.
- disabled blessing pool and why.
- source evidence.
- save/load strategy.
- manual test matrix.
- final build/package/hash/runtime status.

Implementation target:

Primary target: implement all 10 Urda blessings if safe.

Minimum acceptable playable overnight target:
- Urda framework and Act 1 offer works.
- At least 4 safe implemented Urda blessings appear in Urda's live offer pool.
- Disabled or unsafe blessings do not appear.
- Every offered blessing is selectable, save/load safe, localized, and does not softlock.
- If fewer than 4 safe blessings are implemented, do not claim playable; mark blocked.

Recommended minimum active blessing pool if high-complexity features block:

1. `urda_seedbed`
2. `urda_humus_pact`
3. `urda_molting`
4. `urda_moss_map`
5. `urda_after_rain` if lethal hook is source-proven safe
6. `urda_trial_branch` if per-card marker is source-proven safe

High-complexity blessings that may stay disabled if unsafe:
- `urda_rooted_route`
- `urda_root_sight`
- `urda_seed_bank`
- `urda_shallow_root_relic` if relic removal/Act 2 choice cannot be proven safe overnight.

Architecture tasks:

1. Urda Ancient registration
   - Register `EZMB_URDA` as an Act 1 Ancient or equivalent Ancient-offer entity using source-proven APIs.
   - Choosing Urda should offer 4 active Urda blessings.
   - If the game architecture does not allow adding a new Ancient directly through BaseLib/template APIs, implement the narrowest safe patch and document the source evidence.
   - If direct new Ancient registration is unsafe, implement a debug/test-accessible Urda offer path and mark public Ancient registration blocked.

2. Blessing representation
   - Use Blessing Relic models or source-proven blessing model abstractions.
   - Each blessing must have stable id, EN/ZHS name, EN/ZHS description, and runtime state.
   - Chosen blessing persists through save/load.
   - Blessing state must use SavedSpireField or source-proven save APIs.

3. Offer safety
   - Only active implemented blessings appear.
   - Disabled blessings remain documented but absent from the live pool.
   - Add source guards so disabled blessings cannot accidentally appear.

4. Debug helpers
   - Add default-off force helpers if useful:
     - `EZMB_FORCE_ANCIENT=URDA`
     - `EZMB_FORCE_URDA_BLESSING=<id>`
     - `EZMB_URDA_DIAGNOSTICS=1`
   - Document them.
   - They must not be required for normal play if public Urda registration is implemented.

Urda blessing requirements:

A. Seedbed / 苗床 — `urda_seedbed`

Player text:
- For the first 4 normal combats in Act 1, after the normal card reward resolves, show 1 extra Seedling card.
- You may lose 2 HP to take it, or skip it.
- The first Seedling you take is upgraded.
- If you take all 4 Seedlings, Seedbed becomes `???'s Herald` / `???的使者` and you gain 10 Max HP. Do not heal current HP.

Implementation:
- Act 1 only.
- Trigger after normal combat card reward flow, not instead of it.
- Track triggers/accepted/first-upgraded/transformed.
- Safe reward UI. Do not softlock if reward screen is rerolled or skipped.
- Save/load counters.
- Manual tests for all four triggers and hidden transformation.

B. Humus Pact / 腐殖约定 — `urda_humus_pact`

Player text:
- In Act 1, the first 3 times you skip a normal combat card reward, gain 1 Humus and 15 Gold.
- At 3 Humus, remove up to 2 cards, then receive one upgraded card reward.

Implementation:
- Act 1 only.
- Listen only to normal combat card reward skips unless source proves broader reward skip is intended.
- Remove 0/1/2 cards.
- Upgraded reward can be picked or skipped.
- Completed state prevents repeat.
- Save/load counters.

C. Molting / 脱壳 — `urda_molting`

Player text:
- Remove 1 Strike and 1 Defend.
- Add 2 Withered Husk / 枯皮.
- At Act 2 start, remove all Withered Husk.

Withered Husk:
- Status
- Unplayable
- Ethereal
- When exhausted, gain 3 Block.

Implementation:
- Create card id `EZMB_WITHERED_HUSK`.
- EN/ZHS localization.
- Choose/remove Strike/Defend safely; if no valid cards, blessing disabled or fallback documented.
- Prevent Act 1 removal/transform/upgrade if source-proven safe; otherwise document as pending but still auto-remove at Act 2.
- On exhaust gain 3 Block.
- Act 2 cleanup.
- Save/load.

D. Moss Map / 苔痕地图 — `urda_moss_map`

Player text:
- In Act 1, first time you enter each room type:
  - normal combat: gain 25 Gold
  - unknown/event: heal 5 HP
  - shop: gain 1 random potion
  - elite: upgrade 1 random card
  - rest site: gain 3 Max HP
- Each type triggers once.

Implementation:
- Source-proven room-type detection.
- Act 1 only.
- Track rewarded room types.
- Do not double-trigger elite as normal combat.
- Save/load.

E. After the Rain / 雨后苏生 — `urda_after_rain`

Only implement if safe lethal-prevention hook is source-proven.

Player text:
- In Act 1, the first time you would die, instead survive at 1 HP, gain 15 Block, draw 1, add 2 Wounds to discard, and lose 3 Max HP.
- If it never triggers before Act 2, heal 8 HP and gain 75 Gold.
- Before it triggers, defeating an Act 1 elite grants 20 Gold, up to twice.

Implementation:
- Do not guess lethal prevention by direct HP mutation.
- If no safe hook, keep disabled.
- Save/load triggered and elite bonus count.

F. Trial Branch / 试种枝条 — `urda_trial_branch`

Only implement if persistent per-card marker is source-proven.

Player text:
- Choose 1 of 4 common/uncommon class cards.
- Add it upgraded.
- It has Trial Plant: in the next 3 combats, if you play it in at least 2 combats, it stays; otherwise it is removed.

Implementation:
- Track marked card, remaining combats, combats played.
- Save/load.
- If no safe per-card persistence, keep disabled.

G. Shallow-Root Relic / 浅根遗物 — `urda_shallow_root_relic`

Only implement if temporary relic marker and Act 2 choice are source-proven.

Player text:
- Choose 1 of 2 common relics; gain it and 75 Gold.
- If you defeat an Act 1 Elite, it becomes permanent and you gain 35 Gold.
- Otherwise at Act 2 start choose:
  - lose it and gain 75 Gold
  - lose 6 Max HP to keep it.

Implementation:
- Safe common relic choice.
- Mark relic.
- Elite kill listener.
- Act 2 choice.
- Save/load.
- If removing relic or choice UI is unsafe, keep disabled.

H. Rooted Route / 缠根路线 — `urda_rooted_route`

High complexity. Implement only if source-safe overnight; otherwise disabled.

Player text:
- Choose one Root Mark room.
- When reached, receive 3 card rewards and 1 random potion.
- First card taken from these rewards is upgraded.
- If the route has too few rest sites, receive a temporary campfire.
- If player routes away and makes mark unreachable, lose 8 HP, gain 25 Gold, clear mark.

Implementation:
- Requires map node marking, candidate generation, reachability, reward trigger.
- Disable if unsafe.

I. Root-Sight / 根下千里眼 — `urda_root_sight`

High complexity. Implement only if source-safe overnight; otherwise disabled.

Player text:
- Gain 5 Root Eyes.
- Spend 1 on a visible reachable room to preview it.
- First use grants 1 random potion.
- Boss cannot be previewed.

Implementation:
- Requires map UI action and content locking.
- Disable if unsafe.

J. Seed Bank / 种子银行 — `urda_seed_bank`

Medium/high complexity. Implement if reward action storage and pre-boss hook are safe; otherwise disabled.

Player text:
- In Act 1, on normal combat rewards, store 1 unchosen card as a seed, max 3.
- Before Act 1 Boss, choose up to 2 seeds:
  - first is upgraded and added.
  - second is added and receives Trial Plant.
- Others vanish.

Implementation:
- Requires reward UI extension and seed storage.
- Disable if unsafe.

Cards:

Create `WitheredHusk` only if Molting is active.

- ID: `EZMB_WITHERED_HUSK`
- EN title: `Withered Husk`
- ZHS title: `枯皮`
- Type: Status
- Unplayable
- Ethereal
- On exhaust: gain 3 Block
- EN/ZHS text must follow `docs/style/card-localization-style-guide.md`.

Do not implement UrgentDispatch, MirrorShard, Temptation. Those belong to Morvi/Lotha/Vakuu and are out of scope.

Docs and localization:

- Add EN/ZHS localization for Urda and active blessings.
- If disabled blessings have docs-only text, keep it in docs, not active localization unless needed by disabled/debug screen.
- No mojibake.
- No raw tags in live text.
- Add manual checklist rows.

Testing:

Add source guard tests:

- Urda id and name exist.
- Morvi/Lotha/Vakuu are not active.
- Active Urda pool contains only implemented blessings.
- Disabled unsafe blessings cannot be offered.
- Save fields exist for implemented blessing state.
- WitheredHusk exists if Molting active.
- EN/ZHS localization keys exist for active Urda content.
- No official assets copied.
- No source docs claim release-ready without live evidence.

Manual tests:

- Urda appears in Act 1.
- Offer count is 4.
- Each active blessing can be selected.
- Seedbed works.
- Humus Pact works.
- Molting works.
- Moss Map works if active.
- After Rain works if active.
- Save/load for each active blessing.
- Clean log audit.
- No Morvi/Lotha/Vakuu active content.

Validation commands:

- git status --short --branch
- git log -1 --oneline --decorate
- Get-Process SlayTheSpire2 -ErrorAction SilentlyContinue
- dotnet build EZMicroBalance.sln
- dotnet test EZMicroBalance.sln
- dotnet test EZMicroBalance.sln --no-build
- dotnet publish EZMicroBalance.sln
- dotnet test EZMicroBalance.sln --no-build
- $env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'; dotnet test EZMicroBalance.sln --no-build; Remove-Item Env:\EZMB_RUN_RELEASE_ARTIFACT_TESTS
- dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
- git diff --check

Runtime smoke if safe:

- normal Steam-client launch with only BaseLib + EZMicroBalance
- Mod Settings still shows EZ Micro Balance
- Start a new run and verify Urda can be reached/selected
- select at least one implemented Urda blessing
- inspect godot.log with `scripts/audit-godot-log.ps1`

Final response format:

1. Release-ready? yes/no. Do not claim yes unless live gates pass.
2. Files changed.
3. Urda framework status.
4. Active Urda blessing pool.
5. Disabled Urda blessings and why.
6. Each implemented blessing result.
7. WitheredHusk card result if implemented.
8. Save/load result.
9. Build/test/publish/release-artifact results.
10. Package hashes.
11. Runtime smoke result.
12. Remaining blockers.
13. Manual checklist for the user to playtest.
