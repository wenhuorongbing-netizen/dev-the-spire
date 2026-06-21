# EZ Micro Balance Goal Prompts

Historical note: the prompt below is preserved as a pre-independent-project continuation prompt from the legacy `EzDailyContent` implementation phase. It is not the current active release instruction. Current work must follow `AGENTS.md`, `README.md`, `docs/PROJECT_MAP.md`, and `docs/architecture-ez-micro-balance.md`: active code lives under `EZMicroBalanceCode/`, active resources live under `EZMicroBalance/`, and the active manifest id is `EZMicroBalance` while the legacy `EzDailyContent` id remains unchanged.

## Full Finish Prompt

Historical prompt text, retained for audit trail only:

```text
/goal Finish the EZ Micro Balance Ancients rework for the Slay the Spire 2 mod in this repository.

This is an implementation goal, not an audit/report goal. Do not produce an architecture audit, project review, roadmap, or advice-only report as the final output. The goal is not achieved unless code and/or localization/resource files were changed, `dotnet build` was run, and the remaining unimplemented design items are either implemented or recorded as concrete API blockers.

This is also a finish-the-feature goal, not a one-increment goal. Do not stop merely because one relic/card is implemented. Continue until every safe remaining item in `docs/features/ancients-rework-v4/source-design.md` is implemented, documented, and build-verified, or until a specific item is blocked by local compile-time/runtime API evidence that you record.

Before doing anything else, verify you are in the correct project:
- Run `Get-Location`.
- Run `Test-Path -LiteralPath .\EzDailyContent.csproj`.
- Run `Test-Path -LiteralPath .\docs\features\ancients-rework-v4\source-design.md`.
- Run `git status -sb`.

If the current directory is not `D:\Game\FOTN\dev-the-spire`, or `EzDailyContent.csproj` is missing, stop immediately and say the goal was launched in the wrong workspace. Do not scan or audit any other project directory.

Read these first:
- `AGENTS.md`
- `docs/features/ancients-rework-v4/source-design.md`
- `docs/features/ancients-rework-v4/implementation-plan.md`
- `docs/features/ancients-rework-v4/api-discovery.md`
- `docs/features/ancients-rework-v4/work-log.md`
- `docs/features/ancients-rework-v4/external-references.md`
- `docs/dev-environment.md`
- `EzDailyContentCode/Ancients/PaelsHornPhase1Patch.cs`
- `EzDailyContentCode/Ancients/AncientRewardBalancePatches.cs`
- `EzDailyContentCode/AncientRewardNoopProbe.cs`
- `EzDailyContent.csproj`
- `EzDailyContent.json`

External references:
- User-required RitsuLib tutorial: https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/docs/04-ritsulib/04-07-add-ancient/
- Current-project previous framework tutorial: https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/docs/03-previous-framework/03-07-add-ancient/

Hard constraints:
- Preserve the dirty worktree. Never revert, overwrite, delete, or stage existing user changes unless the exact file is part of this goal and the reason is recorded.
- Keep manifest id `EzDailyContent`.
- Do not copy original game assets or long decompiled code.
- Prefer supported game/previous framework/template APIs and game command APIs over direct state mutation.
- Use local compile-time evidence from `sts2.dll` and project references before implementing any API-sensitive behavior.
- Record every implemented item, command, build result, API mismatch, and blocker in `docs/features/ancients-rework-v4/work-log.md` and `docs/features/ancients-rework-v4/api-discovery.md`.
- Update `docs/dev-environment.md` with final build/publish status.
- If a code change fails to compile, fix that before moving to another gameplay item.
- Run `dotnet build` after each meaningful batch of C# changes.
- Do not run `dotnet publish` if build fails.
- Run `dotnet publish` only if localization/resource/manifest/packaging changes require refreshed installed artifacts beyond the build copy target.
- Do not end with only a report. A report is acceptable only as the close-out after implementation/build verification.

Current known completed behavior:
- Pael's Horn: adds one `Relax` and one upgraded `Relax+`.
- Batch 2 behavior is in `EzDailyContentCode/Ancients/AncientRewardBalancePatches.cs`; audit it before extending it.

Primary remaining work:
1. Start by opening and editing the mod implementation, not by writing a standalone report. Work in `EzDailyContentCode/`, `EzDailyContent/`, and `docs/features/ancients-rework-v4/` as needed.
2. Stabilize and finish behavior already implemented in batch 2 if review finds runtime risks.
3. Add or update localization/resources so in-game relic/card text matches the implemented EZ Micro Balance behavior.
4. Finish remaining source-design items where local APIs support a safe implementation:
   - `Claws`: choose 1 curse from 4, add 2 `Wish` and 1 upgraded `Wish+`.
   - `Crossbow`: each turn offer/choose a random attack; accepted attack gets cost -1 this turn plus `Ethereal` and `Exhaust`.
   - `Fiddle`: draw toward 7 at turn start, then cap player-turn draw/hand growth at 7 without disabling all draw effects.
   - `JeweledMask`: choose or create a power target, set it permanently to 0 cost, and pull it from draw pile to hand at combat start.
   - `ChoicesParadox`: filter to 5 usable rare cards, choose 1, add `Retain`, and make it combat-temporary.
   - `PrismaticGem`: every second normal card reward replaces only the rightmost slot with an off-color card.
   - `PaelsTooth`: remove 5, store removed cards, return one upgraded removed card every 2 fights, and clear remaining cards after act boss / act transition according to the design.
   - `MeatCleaver`: rest-site cook option if supported; remove 2 cards and lose 5 HP.
   - `WhisperingEarring`, `MusicBox`, `PumpkinCandle`, `SealOfGold/Debt`, `Sozu`, `Ectoplasm`, `BeautifulBracelet`, `IronClub`, `BrilliantScarf`, `PreservedFog/Folly`, `JewelryBox`, `WarHammer`, `BlackStar`, `BloodSoakedRose/Enthralled`: re-audit against the source design, fix any mismatch, and update text/logging.
5. For any remaining item that is unsafe, do not guess. Record:
   - desired behavior,
   - exact local APIs inspected,
   - why implementation is blocked or too risky,
   - smallest next probe/manual test needed.

Implementation expectations:
- Keep patches grouped by feature or shared helper. Avoid one giant unreviewable method.
- Add helper abstractions only when they reduce real duplication or isolate save/runtime state cleanly.
- For persistent per-relic/card state, use the game's saved-property patterns where available; otherwise document why the item is deferred.
- For generated or modified card instances, prefer `RunState.CreateCard`, `CardPileCmd`, `CardCmd`, `CardSelectCmd`, `RelicCmd`, `PotionCmd`, `PlayerCmd`, and `CreatureCmd`.
- Keep behavior changes scoped to the Ancients rework. Do not add unrelated cards, relics, assets, or mod systems.

Verification required before finishing:
1. `git status -sb`
2. `dotnet build`
3. `git diff --check -- EzDailyContentCode docs/features/ancients-rework-v4 docs/dev-environment.md EzDailyContent EzDailyContent.json`
4. Verify the installed mod DLL matches the build output with SHA256 hash comparison if build copies artifacts.
5. Finish with a concise table of:
   - implemented items,
   - deferred items and blockers,
   - exact log strings to search in `godot.log`,
   - manual in-game test route for the highest-risk items.

The goal is achieved only when the implemented set plus documented blockers covers the full source design. If time or API evidence prevents full completion, stop only after recording a precise next `/goal` prompt that continues from the remaining blockers.
```
