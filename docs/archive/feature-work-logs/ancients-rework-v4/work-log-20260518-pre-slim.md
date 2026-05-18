# EZ Micro Balance Work Log

Current status note: entries below are chronological history. Older entries may reference `EzDailyContent*` paths and smaller automated test totals from before the independent `EZMicroBalance` migration. The current active release code is under `EZMicroBalanceCode/`, active resources are under `EZMicroBalance/`, and the current automated release/source-guard suite is passing unless a later entry supersedes it.

## 2026-05-13 - No-test package refresh for Urda/Morvi hook hardening

- Ran `dotnet publish EZMicroBalance.sln --no-restore`: passed, built the Release DLL, copied the DLL/manifest to the installed `mods\EZMicroBalance` folder, and exported the selected-resource PCK. Godot printed the known project-scan warning for the nested `source code/project.godot` folder and completed `savepack`.
- Updated `README_INSTALL.txt` to state that this package includes the Urda/Morvi active-hook filtering and owned/non-removed deck-mirror recovery hardening, while `dotnet test`, live gameplay/Steam verification, and release-artifact tests were intentionally not rerun for this no-test package refresh.
- Rebuilt package staging, the versioned package folder, and `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` from the refreshed installed artifacts.
- Current hashes: DLL `C64B5787625F497E930D4470AB4758950F59D9574D22847996FBCF55E0DACF71`, JSON `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`, PCK `39F0ED5E592BC9131BE7C317450357F9ACC82D7031D97C92C71C59C8B5109736`, README `B233ADB9730DF93AC8FA291960D3DD2E647D1200B6CD52FFC8183409BA1A820B`, package zip `8AA5F65BECF6672B7B41F3B474851A828BFAF60250F04FB2C58061F52747D128`.
- Ran a no-test source-completion scan across `EZMicroBalanceCode`: no active `TODO`, `FIXME`, or `NotImplemented` markers were found; Urda/Morvi saved-field direct indexing is still blocked outside `AncientPlayerState`; remaining `pending` strings are documented live-verification notes or runtime state names.
- Ran `git diff --check`: passed with CRLF normalization warnings only.
- This closes the non-test packaging gap for the latest source hardening. Private-beta release readiness remains blocked by live Ancient reward matrix rows, save/load-sensitive rows, disable-mod gameplay, post-fix Urda/Rootblight gameplay, natural A11 traversal, co-op verification, release-artifact tests for this package, clean commit, and user-approved push.

## 2026-05-13 - Urda/Morvi source-only hook-state hardening

- Inspected local game source for hook dispatch. `RunState.IterateHookListeners` filters deck/relic/potion listeners to `player.IsActiveForHooks`, but still yields mod run-state subscribers globally through `ModHelper.IterateAllRunStateSubscribers(this)`, so mod subscribers that iterate `RunState.Players` must apply their own active-player filter.
- Inspected `Player.IsActiveForHooks`: it is initialized and restored from `Creature.IsAlive`, and can be toggled by `DeactivateHooks()` / `ActivateHooks()`.
- Hardened `AncientPlayerState.ReadFromDeck` so Urda/Morvi deck-mirror recovery only trusts cards owned by the player and not removed from state, matching the existing mirror-write filter.
- Hardened Urda's act-entry and Moss Map room-entry player loops, plus Morvi's combat-start loop, to process only players active for hooks.
- Updated source/release guards to preserve the `AncientPlayerState` deck mirror pattern and require active-player filtering in the Urda/Morvi hook loops.
- Ran `dotnet build EZMicroBalance.sln --no-restore`: passed with 0 warnings and 0 errors.
- At this source-only checkpoint, `dotnet test`, live gameplay/Steam verification, `dotnet publish`, package refresh, and release-artifact tests had not been rerun. The no-test package refresh entry above supersedes the package gap; tests and live verification remain pending.
- Private-beta release readiness remains blocked by live Ancient reward matrix rows, save/load-sensitive rows, disable-mod gameplay, post-fix Urda/Rootblight gameplay, natural A11 traversal, co-op verification, release-artifact tests for the refreshed package, clean commit, and user-approved push.

## 2026-05-13 - BaseLib-only plug-off startup/log evidence

- Added `-DisableSpirePlus` to `scripts/spire-plus-live-session.ps1` for BaseLib-only plug-off startup/log evidence. The option now requires `-MoveOtherMods`, temporarily isolates `EZMicroBalance` out of the mods folder, records `AllowedModIds`, and restores the package afterward.
- The first settings-only disabled attempt under `.tools/runtime-evidence/live-spire-plus-disabled-session-20260513-142835` is invalid plug-off evidence: the game still logged `Finished mod initialization for 'Spire Plus' (EZMicroBalance)` and `Loaded 2 mods (2 total)`.
- No-launch plug-off helper smoke under `.tools/runtime-evidence/live-spire-plus-disabled-session-20260513-142957` moved 25 entries including `EZMicroBalance`, moved 1 current-run file, restored all 25 mod entries and the run file, and restored Steam settings to the original hash.
- Normal Steam plug-off startup/log validation under `.tools/runtime-evidence/live-spire-plus-disabled-session-20260513-143020` moved 25 entries including `EZMicroBalance`, launched through Steam, reached main menu, logged `Loaded 1 mods (1 total)` and BaseLib initialization only, did not initialize Spire Plus / `EZMicroBalance`, audited clean with 0 release-blocking signatures, then stopped the game and restored settings, current-run save, and moved entries.
- This closes only current plug-off loader evidence. Disable-mod gameplay in an actual run remains pending.
- Ran `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- Ran `dotnet test EZMicroBalance.sln --no-build`: passed, 81 passed, 18 skipped after the BaseLib-only plug-off startup/log refresh.
- Ran `dotnet test EZMicroBalance.sln -c Release`: passed, 81 passed, 18 skipped after the BaseLib-only plug-off startup/log refresh.
- Ran `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed, 99 passed, 0 skipped after the BaseLib-only plug-off startup/log refresh.
- Ran `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed after the BaseLib-only plug-off startup/log refresh.
- Ran `git diff --check`: passed with CRLF normalization warnings only after the BaseLib-only plug-off startup/log refresh.
- Did not run `dotnet publish`; this pass changed scripts, docs, and tests only, so the current package artifacts remain the Urda custom Ancient asset-path package refresh artifacts.

## 2026-05-13 - Ascension live-evidence protocol guard refresh

- Added an A11-A20 live evidence protocol to `docs/features/ascension-11-20/manual-test-checklist.md`, requiring restore-safe Steam setup, foreground preflight before screenshots, copied/audited `godot.log`, and restore with `-PreserveNewCurrentRunsOnRestore` for Rootblight/Blight Sprout, map traversal, save/load, and co-op rows.
- Added release-coverage guard assertions so the Ascension checklist keeps the live-session helper, foreground preflight, log audit, restore command, and invalid covered/wrong-surface screenshot warning.
- Ran `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- Ran `dotnet test EZMicroBalance.sln --no-build`: passed, 81 passed, 17 skipped after adding the Ascension live-evidence protocol guard.
- Ran `dotnet test EZMicroBalance.sln -c Release`: passed, 81 passed, 17 skipped after adding the Ascension live-evidence protocol guard.
- Ran `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed, 98 passed, 0 skipped after adding the Ascension live-evidence protocol guard.
- Ran `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed after adding the Ascension live-evidence protocol guard.
- Ran `git diff --check`: passed with CRLF normalization warnings only after adding the Ascension live-evidence protocol guard.
- Did not run `dotnet publish`; this pass changed only docs and tests, so the current package artifacts remain the Urda custom Ancient asset-path package refresh artifacts.
- Post-fix live Urda selection, Rootblight visual/gameplay verification, Ancient reward matrix, save/load checks, multiplayer disposition, clean commit, and user-approved push remain pending.

## 2026-05-13 - Urda custom Ancient asset-path package refresh

- A controlled current A14 Rootblight generated-art hover probe under `.tools\runtime-evidence\current-rootblight-art-hover-20260513-114103` was negative evidence, not a pass: it entered the default-on Urda Ancient event before combat and `godot-live.log` reported missing vanilla-derived Urda map icon, run-history icon, and background-scene paths.
- Fixed Urda to derive from BaseLib `CustomAncientModel` with `autoAdd: false`, custom mod-owned icon/run-history/background scene paths, and a packaged background scene at `EZMicroBalance/scenes/events/background_scenes/ezmb_urda.tscn`.
- Updated release artifact/source guards so selected-resource PCK parity covers packaged `.tscn` scenes and the Urda custom Ancient asset paths.
- Added headless installed-PCK resource-load evidence at `.tools/runtime-evidence/urda-pck-resource-load-20260513-123345`; the custom Urda scene/icon resolved with `URDA_RESOURCE_LOAD_OK` and 0 `ERROR` / `WARNING` lines.
- Added `scripts/spire-plus-live-session.ps1` for repeatable normal Steam live-test prepare/restore sessions; no-launch smoke checks restored Steam settings byte-for-byte, restored 24 temporarily moved non-BaseLib/EZMicroBalance mod entries, and confirmed current-run isolation is a clean no-op when no current-run files exist.
- Ran a helper-driven normal Steam startup/log validation under `.tools/runtime-evidence/live-spire-plus-session-20260513-125206`; the stricter rerun moved the previous log aside before launch, logged Spire Plus initialization, `Loaded 2 mods (2 total)`, `Found 16 SavedSpireFields`, and `Time to main menu: 13,849ms`, restored settings and 24 moved mod entries, and audited clean. This is loader/helper evidence, not gameplay evidence.
- Hardened `scripts/spire-plus-live-session.ps1` restore for sessions that start or continue a run by adding `-PreserveNewCurrentRunsOnRestore`. The no-launch smoke under `.tools/runtime-evidence/live-helper-preserve-current-run-smoke-20260513-133431` moved a dummy test-created `current_run.save` into evidence, restored the original current run, and restored Steam settings to the expected hash. This is tooling safety evidence, not gameplay evidence.
- Added `scripts/check-spire-window-preflight.ps1` after invalid local screenshot attempts showed another foreground application covering Slay the Spire 2. The preflight records foreground-window state and can reject screenshot collection unless Slay the Spire 2 is actually foreground; smoke evidence under `.tools/runtime-evidence/window-preflight-smoke-20260513-135402` reported `VampireSurvivors` foreground and Slay the Spire 2 not running.
- Added a release-safety guard that requires the invalid live Urda screenshot attempts `.tools/runtime-evidence/live-urda-postfix-20260513-131752` and `.tools/runtime-evidence/live-urda-continue-postfix-20260513-134337` to be referenced only as invalid or non-satisfying evidence in the private-beta release completion audit.
- Added a source guard that prevents Urda/Morvi code from directly indexing `UrdaStateKey`, `UrdaDeckStateKey`, `MorviStateKey`, or `MorviDeckStateKey` outside `AncientPlayerState`, preserving the runtime Player field plus card-backed deck mirror pattern for save/load testing.
- Added a Urda manual-test live evidence protocol and guard coverage requiring restore-safe Steam setup, foreground preflight before screenshots, copied/audited `godot.log`, and restore with `-PreserveNewCurrentRunsOnRestore`.
- Ran `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- Ran `dotnet test EZMicroBalance.sln --no-build`: passed, 81 passed, 17 skipped after adding the Urda live-evidence protocol guard.
- Ran `dotnet test EZMicroBalance.sln -c Release`: passed, 81 passed, 17 skipped after adding the Urda live-evidence protocol guard.
- Ran `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed, 98 passed, 0 skipped after adding the Urda live-evidence protocol guard.
- Ran `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed after adding the Urda live-evidence protocol guard.
- Ran `git diff --check`: passed with CRLF normalization warnings only after adding the Urda live-evidence protocol guard.
- Ran `dotnet publish EZMicroBalance.sln`: passed and refreshed the installed DLL/PCK.
- Rebuilt package staging, the versioned package folder, and `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` from the installed artifacts.
- Current hashes: DLL `8098717F2F99F12D5DA67A32046CD2460644EA9C5EC9864DE64E4A5ECCA356F0`, JSON `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`, PCK `39F0ED5E592BC9131BE7C317450357F9ACC82D7031D97C92C71C59C8B5109736`, package zip `243439C980B7B1E16F8B8B0DF561D9AE073F97C4E92F192A05563885AB2F07C8`.
- Post-fix live Urda selection, Rootblight visual/gameplay verification, Ancient reward matrix, save/load checks, multiplayer disposition, clean commit, and user-approved push remain pending.

## 2026-05-13 - Active zhs Localization Encoding Guard

- Repaired active Simplified Chinese localization in `settings_ui.json`, `card_reward_ui.json`, `events.json`, `rest_site_ui.json`, and `relics.json` after UTF-8/ANSI display checks exposed release-facing mojibake risk.
- Added a guard in `ReleaseSafetyExpandedGuardTests` that scans every active `EZMicroBalance/localization/zhs/*.json` table for known mojibake fragments, extending earlier relic/card/rest-site spot checks.
- Updated the localization validation notes and changelog; runtime language verification remains pending until an in-game English/zhs pass is performed.

## 2026-05-05 15:04:18 +02:00

Goal summary:

- Continue the Ancients rework from `implementation-plan.md`.
- Start with phase 1: change only Pael's Horn so it adds 1 `Relax` and 1 upgraded `Relax+`.
- Continue only if phase 1 completes cleanly and the next step is clearly smaller than phase 1.

Files read:

- `AGENTS.md`
- `docs/features/ancients-rework-v4/source-design.md`
- `docs/features/ancients-rework-v4/implementation-plan.md`
- `docs/features/ancients-rework-v4/external-references.md`
- `EzDailyContentCode/AncientRewardNoopProbe.cs`
- `EzDailyContent.csproj`
- `docs/dev-environment.md`

External references checked:

- `https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/docs/04-ritsulib/04-07-add-ancient/`
- `https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/docs/03-baselib/03-07-add-ancient/`

Commands run:

- `git status -sb`
- `Get-Content -Encoding UTF8 -LiteralPath AGENTS.md`
- Read required local reference files with `Get-Content`.
- Opened both live tutorial URLs with web tooling.
- `dotnet tool install ilspycmd --tool-path .tools\ilspy` failed because the latest package required unavailable tool settings for this environment.
- `dotnet tool search ilspycmd --take 5`
- `Remove-Item -LiteralPath .tools\ilspy -Recurse -Force` and recreated that exact ignored tool directory to replace the failed tool install.
- `dotnet tool install ilspycmd --version 8.2.0.7535 --tool-path .tools\ilspy`
- Used `.tools\ilspy\ilspycmd` against local `sts2.dll` to inspect `Pael`, `PaelsHorn`, `EventOption`, `CardFactory`, `CardCmd`, `CardPileCmd`, `Relax`, and `RelicModel`.
- `Get-Date -Format "yyyy-MM-dd HH:mm:ss zzz"`
- `dotnet build`
- `git status -sb`
- `git diff --check -- EzDailyContentCode\Ancients\PaelsHornPhase1Patch.cs docs\features\ancients-rework-v4\api-discovery.md docs\features\ancients-rework-v4\work-log.md docs\dev-environment.md`
- Read changed files back with `Get-Content` for review.
- `Select-String -Path docs\dev-environment.md -Pattern 'Last successful build'`

Files changed:

- `EzDailyContentCode/Ancients/PaelsHornPhase1Patch.cs`: added the phase 1 Harmony patch for Pael's Horn.
- `docs/features/ancients-rework-v4/api-discovery.md`: recorded live tutorial mismatch and local API findings.
- `docs/features/ancients-rework-v4/work-log.md`: created this work log.
- `docs/dev-environment.md`: updated the latest successful build entry.

Build/publish result:

- `dotnet build` passed on 2026-05-05 with 0 warnings and 0 errors.
- `dotnet publish` not run because this change only edits C# code and `dotnet build` already refreshed the installed DLL/manifest through the project post-build copy target.

Blockers:

- No build blockers.
- Did not continue to a second implementation step because the next listed candidates introduce new API surfaces and are not clearly smaller than phase 1:
  - Black Star needs act detection and random relic grant behavior.
  - Warhammer needs card upgrade selection UI.
  - Jeweled Box / Folly / Withered Claw need instance keyword or selection UI discovery.

Next prompt:

```text
/goal Continue EZ Micro Balance from docs/features/ancients-rework-v4/implementation-plan.md after phase 1.

Read AGENTS.md first. Preserve the dirty worktree.

Start with Phase 2A API discovery for the smallest pickup-only next step: Black Star act-3 immediate relic compensation. Re-read docs/features/ancients-rework-v4/source-design.md, implementation-plan.md, api-discovery.md, external-references.md, and work-log.md. Implement only Black Star if local compile-time evidence identifies safe APIs for current act detection and random relic granting; otherwise update api-discovery.md with blockers and stop.

Run git status -sb first. Run dotnet build after code changes. Do not run dotnet publish unless packaging/resource/manifest changes are made or refreshed installed artifacts are needed for manual verification. Update docs/features/ancients-rework-v4/work-log.md and docs/dev-environment.md before finishing.
```

## 2026-05-05 15:30:02 +02:00

Goal summary:

- User manually verified phase 1 Pael's Horn in game.
- Continue implementation in one larger batch where local API evidence is sufficient.
- Preserve dirty worktree and keep records for every implemented or deferred item.

Files read:

- `AGENTS.md`
- `docs/features/ancients-rework-v4/source-design.md`
- `docs/features/ancients-rework-v4/implementation-plan.md`
- `docs/features/ancients-rework-v4/api-discovery.md`
- `docs/features/ancients-rework-v4/work-log.md`
- `docs/dev-environment.md`
- Local decompiled type evidence from `D:\Steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll`

Commands run:

- Read required local files with `Get-Content`.
- Used `rg` to locate design sections for the target relics/cards.
- Used `.tools\ilspy\ilspycmd` against local `sts2.dll` to inspect `BlackStar`, `WarHammer`, `JewelryBox`, `PreservedFog`, `Claws`, `IronClub`, `BrilliantScarf`, `MusicBox`, `BloodSoakedRose`, `SealOfGold`, `Sozu`, `Ectoplasm`, `Debt`, `WhisperingEarring`, `PumpkinCandle`, `BeautifulBracelet`, `CardModel`, `CardCmd`, `CardPileCmd`, `CardSelectCmd`, `RelicCmd`, `RelicFactory`, `PotionCmd`, `PotionFactory`, `PlayerCmd`, `CreatureCmd`, and related enums.
- `Get-Date -Format "yyyy-MM-dd HH:mm:ss zzz"`
- `dotnet build` after initial implementation attempt; failed due to missing `CardPreviewStyle` namespace.
- `dotnet build` after adding `MegaCrit.Sts2.Core.Nodes.CommonUi`; passed with 0 warnings and 0 errors.
- `dotnet build` after splitting MusicBox Harmony patches; passed with 0 warnings and 0 errors.
- `dotnet build` after Debt exhaust handling adjustment; passed with 0 warnings and 0 errors.

Files changed:

- `EzDailyContentCode/Ancients/AncientRewardBalancePatches.cs`: added batch 2 Harmony patches.
- `docs/features/ancients-rework-v4/api-discovery.md`: recorded batch 2 API evidence, implemented scope, and remaining limits.
- `docs/features/ancients-rework-v4/work-log.md`: appended this work log entry.
- `docs/dev-environment.md`: updated latest successful build status.

Implemented behavior:

- `BlackStar`: act 3+ pickup immediately grants one random relic.
- `WarHammer`: pickup chooses up to two deck cards to upgrade; elite kill behavior remains vanilla.
- `JewelryBox`: adds `Apotheosis` without `Innate`.
- `PreservedFog`: removes up to four cards, then adds `Folly` without `Ethereal` or `Retain`.
- `IronClub`: triggers every five cards played.
- `BrilliantScarf`: makes the sixth card each turn free.
- `MusicBox`: first attack copy each turn gets temporary cost -1 plus `Ethereal` and `Exhaust`.
- `BloodSoakedRose` / `Enthralled`: `Enthralled` gains 10 block when played.
- `BeautifulBracelet`: applies `Swift 2` to three selected cards.
- `Sozu`: fills empty potion slots on pickup, then keeps future potion blocking.
- `Ectoplasm`: grants 250 gold on pickup, then keeps future gold blocking.
- `PumpkinCandle`: on act 3+ transition, extinguishes and randomly upgrades up to two deck cards.
- `SealOfGold` / `Debt`: Seal adds two playable 1-cost Exhaust `Debt` cards; `Debt` loses up to 5 gold when exhausted; Seal now provides max energy instead of per-turn gold drain.
- `WhisperingEarring`: rounds 1-3 auto-play one highest-cost playable hand card instead of playing up to 13 cards on round 1.

Deferred:

- `Claws`, `Crossbow`, `Fiddle`, `JeweledMask`, `ChoicesParadox`, `PrismaticGem`, `PaelsTooth`, rest-site changes, and reward-slot rewrites require deeper UI/state or persistent marker work.
- Localization/resource text was not changed in this batch; behavior is ahead of displayed text.

Build/publish result:

- Final `dotnet build` passed on 2026-05-05 with 0 warnings and 0 errors.
- `dotnet publish` not run because only C# changed and `dotnet build` copied the DLL/manifest to the installed mod folder.

## 2026-05-05 17:14:30 +02:00

Goal summary:

- Finish the EZ Micro Balance Ancients rework where local APIs support safe implementation.
- Stabilize batch 2 runtime risks, add localization/resource text, and document exact blockers for unsafe remaining items.

Files read:

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
- `EzDailyContent/localization/eng/relics.json`
- `EzDailyContent/localization/eng/cards.json`
- Live RitsuLib and BaseLib Ancient tutorial URLs listed in `external-references.md`.

Commands run:

- `git status -sb`
- Required local files read with `Get-Content -Encoding UTF8`.
- Used `.tools\ilspy\ilspycmd` against local `sts2.dll` to inspect `Claws`, `Crossbow`, `Fiddle`, `JeweledMask`, `ChoicesParadox`, `PrismaticGem`, `PaelsTooth`, `MeatCleaver`, `CookRestSiteOption`, `RestSiteOption`, `ToastyMittens`, `YummyCookie`, `Tezcatara`, `Folly`, `Debt`, `Enthralled`, `CardSelectorPrefs`, `CardSelectCmd`, `CardFactory`, `CardPileCmd`, `CardCmd`, `PowerCmd`, `CreatureCmd`, `LocManager`, `LocTable`, `ModelDb`, `EnchantmentModel`, and BaseLib `CustomEnchantmentModel`, `ModelLocPatch`, `PrefixIdPatch`, `TypePrefix`, and `CardModifierLoc`.
- `dotnet build` after the finish batch; failed once because `RelicModel.L10NLookup` is protected.
- `dotnet build` after replacing the protected helper with `new LocString("relics", "CHOICES_PARADOX.selectionScreenPrompt")`; passed with 0 warnings and 0 errors.
- `dotnet publish` because localization/resources changed; command returned exit code 0 and exported the Godot `.pck`. Godot printed a script-scan `FileNotFoundException` for assembly `sts2` during export but continued through `savepack` and packed `cards.json`, `relics.json`, and `rest_site_ui.json`.
- `git diff --stat -- EzDailyContentCode EzDailyContent docs\features\ancients-rework-v4 docs\dev-environment.md`
- `Get-Date -Format "yyyy-MM-dd HH:mm:ss zzz"`

Files changed:

- `EzDailyContentCode/Ancients/AncientRewardBalancePatches.cs`: added finish-batch patches and helper/custom enchantment.
- `EzDailyContent/localization/eng/relics.json`: added English text overrides for changed relics and Jeweled Mask prompt.
- `EzDailyContent/localization/eng/cards.json`: added English text overrides for `Debt`, `Enthralled`, and `Folly`.
- `EzDailyContent/localization/eng/rest_site_ui.json`: added Cook option text for the Meat Cleaver rework.
- `docs/features/ancients-rework-v4/api-discovery.md`: recorded finish-batch API evidence, implemented items, blockers, and runtime risks.
- `docs/features/ancients-rework-v4/work-log.md`: appended this entry.

Implemented behavior:

- `Claws`: chooses 1 curse from 4 and adds 2 `Wish` plus 1 upgraded `Wish+`.
- `Crossbow`: each owner turn offers one random attack; accepted cards get temporary cost -1 plus `Ethereal` and `Exhaust`, skipped cards are removed from combat state.
- `Fiddle`: draws toward a 7-card hand at turn start, no longer blocks all draw, and caps player-turn non-hand-draw effects at 7 cards in hand.
- `JeweledMask`: chooses an unenchanted deck power or drafts one generated character power; marks it with a persistent custom enchantment that sets energy cost to 0, then pulls the marked power from draw pile to hand at combat start.
- `ChoicesParadox`: offers five usable rare generated cards from all character pools plus colorless, applies `Retain`, and adds the chosen combat-temporary card to hand.
- `ToastyMittens` / source-design Baking Gloves: offers the top draw-pile card before hand draw; accepting exhausts it and grants 1 Strength, skipping keeps it.
- `MeatCleaver`: Cook removes 2 cards and loses 5 current HP; no max HP gain; option disabled if current HP is too low or fewer than 2 removable cards exist.
- `Folly`: canonical keywords now remove `Ethereal`.
- `Debt`: no longer loses gold at end of turn in hand; loses up to 5 gold only on exhaust.
- Localization/resource text was added for implemented behavior.

Deferred:

- `PrismaticGem`: deferred because the design requires a saved every-second reward counter and post-generation reward-screen replacement. Local API evidence only confirmed pre-generation card-pool modification on the vanilla relic.
- `PaelsTooth`: deferred because the design requires a saved every-2-combats counter and act-boss/act-transition clear. Local vanilla state only saves the removed-card list, not the counter.
- `Quality Blade` / source-design name-TBD item: deferred because the source design itself requires exact English name/effect verification before first-batch implementation and local type searches did not identify a confirmed target.

Build/publish result:

- `dotnet build` passed on 2026-05-05 with 0 warnings and 0 errors after the protected-helper fix.
- `dotnet publish` returned exit code 0 and exported the `.pck`; note the non-fatal Godot script-scan `sts2` assembly load exception printed during export.

Final verification:

- `git status -sb` showed the pre-existing dirty worktree plus this batch's changed localization/docs and untracked `EzDailyContentCode/Ancients/` / `docs/features/` paths.
- Final `dotnet build` passed with 0 warnings and 0 errors.
- `git diff --check -- EzDailyContentCode docs/features/ancients-rework-v4 docs/dev-environment.md EzDailyContent EzDailyContent.json` exited 0; it printed only the existing CRLF/LF warning for `docs/dev-environment.md`.
- SHA256 comparison confirmed the installed mod DLL matches the build output:
  - Build: `9D240701EF3F224A1630FEF7E5E216F8F8A03A90B9E57FBFC2EA9AEEAEA7E4C4`
  - Installed: `9D240701EF3F224A1630FEF7E5E216F8F8A03A90B9E57FBFC2EA9AEEAEA7E4C4`

Next prompt if manual runtime testing finds issues:

```text
/goal Runtime-verify and harden the EZ Micro Balance finish batch.

Read AGENTS.md and docs/features/ancients-rework-v4/api-discovery.md first. Preserve the dirty worktree.

Start from the current finish-batch implementation. Use godot.log search strings listed in the prior final response to test Claws, Crossbow, Fiddle, JeweledMask, ChoicesParadox, ToastyMittens, MeatCleaver, Debt, and Folly. If any runtime error appears, inspect the exact local API involved with ilspycmd, patch only the failing feature, update api-discovery.md/work-log.md/dev-environment.md, and run dotnet build. Run dotnet publish only if resources/localization/package contents change.
```

## 2026-05-05 18:10:54 +02:00

Goal summary:

- Finish the remaining EZ Micro Balance Ancients rework blockers completely, with implementation first and hard local API evidence for any impossible item.

Workspace verification:

- `Get-Location` returned `D:\Game\FOTN\dev-the-spire`.
- `Test-Path -LiteralPath .\EzDailyContent.csproj` returned `True`.
- `Test-Path -LiteralPath .\docs\features\ancients-rework-v4\source-design.md` returned `True`.
- `git status -sb` showed the expected dirty worktree; no unrelated changes were reverted, deleted, staged, or overwritten.

Files read:

- `AGENTS.md`
- `docs/features/ancients-rework-v4/source-design.md`
- `docs/features/ancients-rework-v4/api-discovery.md`
- `docs/features/ancients-rework-v4/work-log.md`
- `EzDailyContentCode/Ancients/AncientRewardBalancePatches.cs`
- `EzDailyContentCode/Ancients/PaelsHornPhase1Patch.cs`
- `EzDailyContent/localization/eng/relics.json`
- `EzDailyContent/localization/eng/cards.json`
- `docs/dev-environment.md`

Commands and notable command results:

- Initial `.tools\ilspycmd\ilspycmd.exe` probes failed because that path does not exist in this checkout.
- Located ILSpy at `.tools\ilspy\ilspycmd.exe`.
- Initial `sts2.dll` probes against `SlayTheSpire2_Data\Managed\sts2.dll` failed because that path does not exist for this install.
- Located local game API assembly at `D:\Steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll`.
- Used `.tools\ilspy\ilspycmd.exe` against local `sts2.dll` and BaseLib `BaseLib.dll` to inspect `CardReward`, `RewardsCmd`, `CardFactory`, `CardCreationOptions`, `CardCreationFlags`, `CardCreationResult`, `AbstractModel`, `PrismaticGem`, `PaelsTooth`, `SavedProperties`, `SavedProperty`, BaseLib `SpireField`, BaseLib `SavedSpireField`, `SavedSpireFieldPatch`, `CombatRoom`, `RoomType`, `CardSelectCmd`, `CardPileCmd`, `RelicModel`, `LocManager`, `Tezcatara`, Tezcatara relics, `RefineBlade`, `SovereignBlade`, and `ForgeCmd`.
- First blocker-batch `dotnet build` failed with compile errors: `SavedSpireField<int>` indexer returned non-null `int` so `?? 0` was invalid, and `SerializableCard` needed `MegaCrit.Sts2.Core.Saves.Runs`.
- Fixed those compile errors, then `dotnet build` passed with 0 warnings and 0 errors.
- `dotnet build` after the `SovereignBlade` patch passed with 0 warnings and 0 errors.
- `New-Item -ItemType Directory -Force -LiteralPath .\EzDailyContent\localization\zhs` failed because this PowerShell version lacks `New-Item -LiteralPath`; retried with `-Path` and created the directory.
- `dotnet build` after localization changes passed with 0 warnings and 0 errors.
- `dotnet publish` returned exit code 0, copied DLL/manifest, exported the Godot `.pck`, and packed `EzDailyContent/localization/zhs/cards.json`, `zhs/relics.json`, and `zhs/rest_site_ui.json`. Godot printed the known non-fatal script-scan `FileNotFoundException` for assembly `sts2` during export, then completed `savepack`.

Files changed:

- `EzDailyContentCode/Ancients/AncientRewardBalancePatches.cs`: implemented `PrismaticGem`, `PaelsTooth`, and generated `SovereignBlade` Exhaust behavior.
- `EzDailyContent/localization/eng/relics.json`: added English text for `BloodSoakedRose`, `PaelsTooth`, and `PrismaticGem`.
- `EzDailyContent/localization/zhs/relics.json`: added Simplified Chinese text overrides for implemented relic behavior.
- `EzDailyContent/localization/zhs/cards.json`: added Simplified Chinese card text overrides for `Debt`, `Enthralled`, and `Folly`.
- `EzDailyContent/localization/zhs/rest_site_ui.json`: added Simplified Chinese Cook option text for `MeatCleaver`.
- `docs/features/ancients-rework-v4/api-discovery.md`: replaced prior blockers with hard local API evidence and implemented-result notes.
- `docs/features/ancients-rework-v4/work-log.md`: appended this entry.
- `docs/dev-environment.md`: updated latest build/publish status.

Implemented behavior:

- `PrismaticGem`: preserves +1 Energy but skips vanilla all-slot card-pool broadening. A save-backed normal reward counter now replaces every slot of every second normal monster card reward with unlocked off-color cards, preserving each slot's rarity when available. It skips no-pool/no-model modification rewards, custom pools, filtered pools, colorless pools, elites, bosses, shops, events, and other special reward sources.
- `PaelsTooth`: keeps vanilla pickup removal and saved removed-card list. A save-backed non-boss combat counter now waits two non-boss combats, then lets the player choose one stored removed card to return upgraded. Remaining stored cards are cleared after act boss combat or act transition.
- `Quality Blade` / name-TBD: resolved by local API evidence as generated `SovereignBlade` from `ForgeCmd.Forge(...)`, while `RefineBlade` is a permanent common skill that triggers Forge. Forged temporary `SovereignBlade` cards with `CreatedThroughForge` now gain `Exhaust`; permanent `RefineBlade` and non-forged copies are not altered.
- Simplified Chinese localization was added under `EzDailyContent/localization/zhs` for all implemented EZ Micro Balance relic/card/rest-site text listed in the goal.

Deferred:

- No remaining source-design item is deferred for lack of local compile-time API evidence after this pass.

Build/publish result:

- Latest `dotnet build` passed with 0 warnings and 0 errors.
- Latest `dotnet publish` returned exit code 0 and exported the `.pck`; note the non-fatal Godot script-scan `sts2` assembly load exception printed during export.

Manual runtime test focus:

- `PrismaticGem`: historical v4.1/v4.2 test focus was the rightmost-slot reward replacement. This is superseded by v4.3; current manual checks must verify every visible slot is off-color on trigger screens.
- `PaelsTooth`: test pickup removal, one non-boss combat with no return, second non-boss combat with choice and upgraded return, and act boss clear.
- Chinese text: launch with language `zhs` and inspect relic/card descriptions plus the Cook rest-site option.

Final verification:

- Final `dotnet build` passed with 0 warnings and 0 errors after code, localization, and documentation updates.
- `git diff --check -- EzDailyContentCode EzDailyContent docs/features/ancients-rework-v4 docs/dev-environment.md EzDailyContent.json` exited 0; it printed only the existing CRLF/LF warning for `docs/dev-environment.md`.
- SHA256 comparison confirmed the installed mod DLL matches the build output:
  - Build: `B4C8436174F77E07B0CB0779A08A3C5342D7269188CCF5AE59FF02CA151BC19F`
  - Installed: `B4C8436174F77E07B0CB0779A08A3C5342D7269188CCF5AE59FF02CA151BC19F`

## 2026-05-05 18:59:08 +02:00

Goal summary:

- Fix the remaining Prismatic Gem reroll acceptance bug for EZ Micro Balance.
- Preserve the dirty worktree and change only the Prismatic Gem reroll/counter behavior plus required feature docs.

Workspace verification:

- `git status -sb` was run first and showed the existing dirty worktree, including modified `AGENTS.md`, docs/localization files, and untracked `EzDailyContentCode/Ancients/` plus `docs/features/`.
- No unrelated dirty files were reverted, deleted, staged, or overwritten.

Files read:

- `AGENTS.md`
- `docs/features/ancients-rework-v4/source-design.md`
- `docs/features/ancients-rework-v4/implementation-plan.md`
- `docs/features/ancients-rework-v4/api-discovery.md`
- `docs/features/ancients-rework-v4/work-log.md`
- `EzDailyContentCode/Ancients/AncientRewardBalancePatches.cs`
- `docs/dev-environment.md`

Commands and notable command results:

- Used `rg` to locate Prismatic Gem, reward, reroll, and API-discovery references.
- Used `.tools\ilspy\ilspycmd.exe` against local `D:\Steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll` to inspect `CardReward` and `CardFactory`.
- Confirmed `CardReward.Reroll()` clears the existing card list and calls `Populate()` again on the same `CardReward` instance.
- Confirmed `CardFactory.CreateForReward(...)` calls `Hook.TryModifyCardRewardOptions(...)` after creating reward cards, so rerolls re-enter the Prismatic Gem patch.
- `dotnet build` passed after the code change with 0 warnings and 0 errors.
- Final `dotnet build` passed after documentation updates with 0 warnings and 0 errors.

Files changed:

- `EzDailyContentCode/Ancients/AncientRewardBalancePatches.cs`: added `CardReward.Populate()` screen context and screen-scoped Prismatic Gem trigger state.
- `docs/features/ancients-rework-v4/api-discovery.md`: recorded reroll API evidence and the chosen saved-counter plus per-screen state strategy.
- `docs/features/ancients-rework-v4/work-log.md`: appended this entry.
- `docs/dev-environment.md`: updated latest build status and publish decision.

Implemented behavior:

- Prismatic Gem's saved normal reward counter now increments once when an eligible normal card reward screen is first generated.
- The same reward screen stores whether it is a trigger. Rerolls reuse that decision instead of consuming the next saved counter value.
- Historical behavior for this 2026-05-05 pass: trigger screens replaced only the rightmost reward slot with an unlocked off-color card on initial generation and every reroll, preserving that slot's original rarity when a same-rarity replacement was available. v4.3 later superseded this with all-slot replacement.
- Non-trigger and ineligible screens remain non-trigger across rerolls.
- Existing exclusions remain in force: non-normal rewards, elites, bosses, events, custom pools, filtered pools, colorless-only pools, no-pool/no-model-modification rewards, and non-screen reward modifications do not increment or replace.

Build/publish result:

- `dotnet build` passed on 2026-05-05 with 0 warnings and 0 errors.
- `dotnet publish` was not run because no resource, localization, manifest, or package artifacts needed refresh.

## 2026-05-05 20:42:21 +02:00

Goal summary:

- Entered build mode for the one-month private beta completion cycle.
- Preserve the dirty worktree while making `EZ Micro Balance` independently buildable and publishable.
- Do not implement Ascension 11-20-30 or custom-character work.

Phases completed:

- Phase 0: reran `git status -sb` and preserved the existing dirty worktree.
- Phase 1: documented the independent-mod architecture with stable manifest id `EZMicroBalance`; legacy id `EzDailyContent` remains unchanged.
- Phase 2: updated project docs to reflect active EZ Micro Balance work and archived historical roadmap/research docs under `docs/archive/legacy-planning/`.
- Phase 3: created the independent `EZMicroBalance` project, manifest, resource folder, code folder, solution, and active build path.
- Phase 4: split active Ancient patches into grouped `EZMicroBalanceCode/Ancients/Common/` and `EZMicroBalanceCode/Ancients/Patches/` files.
- Phase 5: ensured release behavior does not compile the no-op probe; legacy probe is gated by `EZ_MICRO_BALANCE_DEBUG_PROBES=1`.
- Phase 6: reviewed and hardened high-risk patches, including Jeweled Mask cost setting, Music Box patch scope, Pael's Tooth reflection guard, and asynchronous Debt exhaust handling.
- Phase 7: validated English and Simplified Chinese localization JSON, matched key sets, and updated Prismatic Gem text.
- Phase 8: ran `dotnet build` and `dotnet publish`; switched Godot export to selected resources and audited the PCK.
- Phase 9: prepared the manual runtime verification matrix with explicit Prismatic Gem reroll tests.
- Phase 10: updated the private beta release checklist, known issues, and unsupported cases.

Build/publish result:

- `dotnet build` passed with 0 warnings and 0 errors.
- `dotnet publish` passed and produced `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance\EZMicroBalance.dll`, `.json`, and `.pck`.
- PCK audit parsed 31 entries and found 0 entries from legacy `EzDailyContent`, C# source folders, docs, art, asset, or archive folders.
- Godot headless export still prints a non-fatal `sts2` assembly load exception while scanning C# scripts; savepack completes.

Remaining release gates:

- Independent `EZMicroBalance` runtime load is not yet verified in a fresh game session.
- Manual feature matrix results are pending.
- Save/load behavior and disabled-mod behavior are pending runtime verification.
- Worktree is not clean, no commit has been created, and push to `origin/main` still requires explicit user approval.

## 2026-05-05 20:55:12 +02:00

Goal summary:

- Continue the build/test/review loop after the initial release-readiness blocker.
- Attempt to gather fresh runtime load evidence without changing repository source state beyond documentation.

Runtime smoke results:

- Verified no `SlayTheSpire2` process was running before smoke tests.
- Direct `SlayTheSpire2.exe` launch created a fresh `godot.log` but failed before mod loading with Steamworks `No appID found`.
- Direct launch with a temporary `steam_appid.txt` value `2868840` created a fresh `godot.log` but failed before mod loading with Steamworks `ConnectToGlobalUser failed`; the temporary file was removed after the attempt.
- `D:\Steam\steam.exe -applaunch 2868840` did not start a detectable `SlayTheSpire2` process during the bounded smoke-test window.

Review/build status:

- Static review of active project metadata, manifest, export preset, and high-risk patch files did not identify a new compile-time fix.
- Runtime Mod Settings and feature verification remain blocked until the game is launched through the normal Steam client path.

## 2026-05-05 21:04:51 +02:00

Goal summary:

- Continue the test/review/build loop after discovering the Steam startup bypass.
- Use a reversible controlled smoke profile to verify actual mod loading and fix initializer failures.

Local API evidence:

- `NGame.InitializePlatform()` supports `--force-steam off`, which skips Steam initialization before `GameStartup()`. This allowed a local smoke test without Steamworks `ConnectToGlobalUser`.
- `PlayerCombatState.MaxEnergy` calls `Hook.ModifyMaxEnergy(...)`, which calls `AbstractModel.ModifyMaxEnergy(Player, decimal)`.
- `AbstractModel.BeforeSideTurnStart(PlayerChoiceContext, CombatSide, ICombatState)` is the inherited Crossbow offer hook; `RelicModel` does not define that method.

Files changed:

- `EZMicroBalanceCode/Ancients/Patches/SealOfGoldPatches.cs`: retargeted max-energy Harmony patch from nonexistent `RelicModel.ModifyMaxEnergy` to `AbstractModel.ModifyMaxEnergy`.
- `EZMicroBalanceCode/Ancients/Patches/TurnOfferAndRestPatches.cs`: retargeted Crossbow offer Harmony patch from nonexistent `RelicModel.BeforeSideTurnStart` to `AbstractModel.BeforeSideTurnStart`.
- Runtime evidence docs updated in `docs/dev-environment.md`, `docs/release-checklist.md`, and `manual-verification-matrix.md`.

Controlled smoke result:

- A temporary default-profile `settings.save` edit enabled only `BaseLib` and `EZMicroBalance`; all other discovered local mods, including legacy `EzDailyContent`, were disabled for the smoke test. Original settings were restored in `finally`.
- First smoke found `SealOfGoldMaxEnergyPatch` undefined target.
- Second smoke found `CrossbowOfferPatch` undefined target.
- Final smoke loaded `BaseLib.dll`, `BaseLib.pck`, `EZMicroBalance.dll`, and `EZMicroBalance.pck`; `Finished mod initialization for 'EZ Micro Balance' (EZMicroBalance)` appeared in `godot.log`; game reached main menu.

Remaining release gates:

- Historical at this point in the log: normal Steam-client Mod Settings verification was still pending; later RC1 evidence supersedes this for the Mod Settings gate.
- Manual reward behavior, Prismatic Gem reroll, save/load, and disable-mod behavior tests are still pending.

Final automated checks after the runtime hook fixes:

- `dotnet build`: passed with 0 warnings and 0 errors.
- `dotnet test --no-build`: exit code 0.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: exit code 0.
- `dotnet publish`: exit code 0; copied DLL and manifest.
- Standalone .NET 9 Harmony audit: `PatchAll OK` for `EZMicroBalance.dll`.
- JSON parse: all `EZMicroBalance/localization/**/*.json` parsed as UTF-8 JSON.
- PCK audit: 31 entries, 0 legacy/source/docs/art/archive entries.
- DLL audit: build output and installed `EZMicroBalance.dll` SHA256 hashes matched.
- `git diff --check`: exit code 0 with only the existing CRLF normalization warning for `docs/dev-environment.md`.
- Controlled disable smoke: with BaseLib enabled and EZ Micro Balance disabled, `godot.log` showed `Skipping loading mod EZMicroBalance, it is set to disabled in settings`, no `EZMicroBalance.dll` load line, and main menu reached.

## 2026-05-05 21:14:01 +02:00

Goal summary:

- Continue the build/test/review loop by making `dotnet test` meaningful for private beta release readiness.
- Preserve the dirty worktree and avoid resource/package changes.

Files changed:

- `tests/EZMicroBalance.Tests/EZMicroBalance.Tests.csproj`: added the xUnit release artifact test project.
- `tests/EZMicroBalance.Tests/ReleaseArtifactTests.cs`: added tests for manifest identity, localization JSON/key parity, selected-resource PCK contents, installed DLL parity, and Harmony patch target resolution against the installed game API.
- `EZMicroBalance.sln`: added the test project.
- `docs/dev-environment.md`, `docs/test-plan.md`, `docs/PROJECT_MAP.md`, and `docs/release-checklist.md`: recorded the new test suite and latest pass result.

Commands and results:

- `git status -sb`: showed the expected dirty worktree; no unrelated changes were reverted.
- `dotnet build EZMicroBalance.sln`: first failed because `ToSortedSet(...)` was unavailable in the test target; fixed with explicit `SortedSet<string>` construction.
- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors after the test helper fix.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 5 tests total.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `dotnet build`: passed with 0 warnings and 0 errors after documentation updates.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 5 tests total.
- `git diff --check -- EzDailyContentCode EzDailyContent docs/features/ancients-rework-v4 docs/dev-environment.md EzDailyContent.json`: passed with only the existing CRLF normalization warning for `docs/dev-environment.md`.
- `git diff --check -- EZMicroBalanceCode EZMicroBalance tests EZMicroBalance.csproj EZMicroBalance.sln EZMicroBalance.json docs/PROJECT_MAP.md docs/test-plan.md docs/release-checklist.md`: passed.

Publish decision:

- `dotnet publish` was not rerun in this step because the only source changes were tests and docs; no resource, localization, manifest, project packaging, DLL, or PCK artifact required refresh.

## 2026-05-05 21:18:25 +02:00

Goal summary:

- Perform the completion audit for the active build/test/review goal against the actual current tree.

Commands and evidence:

- `git status -sb`: worktree remains dirty with the expected independent-project migration, documentation archive, and test-project changes.
- `dotnet build`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 5 tests total.
- `rg` over `docs/release-checklist.md`, `docs/test-plan.md`, `docs/dev-environment.md`, and `manual-verification-matrix.md`: confirmed normal Steam-client Mod Settings verification, manual feature matrix results, save/load checks, disable gameplay checks, clean commit, and push are still pending.
- Read `AGENTS.md`, `docs/release-checklist.md`, and `manual-verification-matrix.md` for release-gate evidence.

Files changed:

- `AGENTS.md`: corrected the current setup status to distinguish controlled `--force-steam off` smoke-load success from still-pending normal Steam-client and gameplay verification.

Completion decision:

- The build and automated test portions are green.
- The active goal is not complete because the review found release gates that remain unverified or require user action.
- `update_goal` was not called.

## 2026-05-05 21:20:04 +02:00

Goal summary:

- Continue the build/test/review loop and verify the full publish path after the test project addition.

Review finding:

- `dotnet publish` initially published `EZMicroBalance.Tests` because the test project was part of the solution and did not opt out of publishing.
- This did not affect the installed mod artifacts, but it made the root publish output noisy and less release-focused.

Files changed:

- `tests/EZMicroBalance.Tests/EZMicroBalance.Tests.csproj`: added `<IsPublishable>false</IsPublishable>`.
- `docs/dev-environment.md` and this work log: recorded the publish behavior fix and latest verification.

Commands and results:

- `git status -sb`: showed the expected dirty worktree.
- `dotnet publish`: reproduced the noisy test-project publish before the fix.
- `dotnet build`: passed with 0 warnings and 0 errors after the fix.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 5 tests total.
- `dotnet publish`: passed and published only `EZMicroBalance`.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 5 tests total after publish.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain outside what this non-interactive static/build loop can honestly close.

## 2026-05-05 21:22:55 +02:00

Goal summary:

- Continue the review loop over solution and release configuration.

Review findings:

- `Release|Any CPU` in `EZMicroBalance.sln` mapped the active mod project to `Debug|Any CPU`, so root `dotnet publish` produced the mod DLL from the Debug output path.
- `InstalledDllMatchesBuildOutput` assumed the installed DLL always matched the Debug output, which failed after an explicit Release build copied the Release DLL into the installed mod folder.

Files changed:

- `EZMicroBalance.sln`: mapped the active mod project's Release configurations to `Release|Any CPU`.
- `tests/EZMicroBalance.Tests/ReleaseArtifactTests.cs`: made DLL parity accept any current `Debug` or `Release` build output and changed the Harmony patch audit to load the installed mod DLL.
- `docs/dev-environment.md` and this work log: recorded the release-configuration fix and latest verification.

Commands and results:

- `dotnet build EZMicroBalance.csproj -c Release`: passed with 0 warnings and 0 errors, proving the active mod builds in Release.
- `dotnet test EZMicroBalance.sln --no-build`: failed once after the Release build because the DLL parity test was hardcoded to Debug.
- `dotnet build`: passed with 0 warnings and 0 errors after the test and solution fixes.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 5 tests total, after the default Debug build.
- `dotnet publish`: passed, built `EZMicroBalance` in Release, and skipped publishing the test project.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 5 tests total, against the installed Release DLL after publish.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check -- EzDailyContentCode EzDailyContent docs/features/ancients-rework-v4 docs/dev-environment.md EzDailyContent.json`: passed with only the existing CRLF normalization warning for `docs/dev-environment.md`.
- `git diff --check -- EZMicroBalanceCode EZMicroBalance tests EZMicroBalance.csproj EZMicroBalance.sln EZMicroBalance.json docs/PROJECT_MAP.md docs/test-plan.md docs/release-checklist.md AGENTS.md`: passed.

Remaining blocker:

- The same live verification gates remain: normal Steam-client Mod Settings verification, manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push.

## 2026-05-05 21:27:31 +02:00

Goal summary:

- Re-run controlled runtime smoke after Release publish to verify the installed Release DLL loads.

Commands and results:

- Located default profile settings at `%APPDATA%\SlayTheSpire2\default\1\settings.save` and current log at `%APPDATA%\SlayTheSpire2\logs\godot.log`.
- First Release-DLL smoke reached main menu and initialized EZ Micro Balance, but loaded extra local mods because some local mod manifests were malformed or nested and not discovered by simple JSON scanning.
- Second isolated smoke combined JSON-discovered ids, Steam-profile mod ids, and explicit ids observed in `godot.log`; temporary settings enabled only `BaseLib` and `EZMicroBalance`.
- Isolated smoke with `--force-steam off` loaded exactly 2 mods, finished BaseLib initialization, loaded installed `EZMicroBalance.dll` and `.pck`, finished EZ Micro Balance initialization, and reached main menu.
- Original default-profile settings were restored; `mod_settings` returned to `null`.

Observed unrelated local issues:

- `RouteSuggest-v1.9.0\RouteSuggestConfig.json` and `sts2-heybox-support\mod_mainfest.json` still emit invalid-manifest scan errors before disabled-mod filtering. These are local third-party mod files and not part of `EZMicroBalance`.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-06 - Ancients v4.2 Final Validation And Package Refresh

Commands and results:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 62 tests total before publish.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `Get-Process SlayTheSpire2 -ErrorAction SilentlyContinue`: no running game process before publish.
- `dotnet publish EZMicroBalance.sln`: passed; built Release, copied DLL/manifest, and exported the selected-resource PCK. Godot still printed the known non-fatal `sts2` assembly scan exception during export.
- Rebuilt `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` from the final installed `mods\EZMicroBalance` artifacts.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 62 tests total after publish/package/hash doc refresh.
- `git diff --check`: exit code 0 with the existing `EzDailyContent.json` CRLF normalization warning.

Final artifact hashes:

- Zip SHA256: `93F67E3B7542EFA7A3B0EED55510C1A714810035DFF525C1E62D3EE9B11382D8`.
- Installed/staging/versioned/extracted zip DLL SHA256: `2E869A0C6F22845AE150D35B64B508A6B7B84DE191D7FFA47AE29599F286D651`.
- Installed/staging/versioned/extracted zip PCK SHA256: `D043D5F06440ACA128AA7153BE0B7C0B1DB7F95AD4DE8197826A0A1F07BFBF1D`.
- Installed/staging/versioned/extracted zip manifest SHA256: `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-06 - Ancients v4.2 Implementation Pass

Goal summary:

- Implement only Ancient v4.2 changes for the active `EZMicroBalance` project.
- Preserve existing manifest ids and keep Ascension 21-30/custom-character work out of scope.
- Archive the local v4.2 next-plan input into the feature docs.

Files changed:

- `docs/features/ancients-rework-v4/sts2_ancients_rework_v4_2_next_plan.md`: archived the v4.2 next-plan file from `C:\Users\Jack\Downloads`.
- `EZMicroBalanceCode/Ancients/Patches/VakuRewardPatches.cs`: added Velvet Choker soft-limit cost/counting hooks and Distinguished Cape v4.2 max-HP loss.
- `EZMicroBalanceCode/Ancients/Patches/PrismaticGemPatches.cs`: changed trigger rewards from all-slot replacement to rightmost-slot-only replacement with screen-scoped reroll state.
- `EZMicroBalanceCode/Ancients/Patches/TurnOfferAndRestPatches.cs`: suppresses Velvet Choker tax while evaluating and spending for Whispering Earring autoplay.
- `EZMicroBalanceCode/Ancients/Common/JeweledMaskFreePower.cs` and active zhs localization JSON: normalized player-facing Simplified Chinese numbers with no spaces around digits.
- `tests/EZMicroBalance.Tests/*`: expanded Ancient v4.2 source guards, zhs numeric-format guards, Prismatic rightmost-only checks, package/export parity coverage references, stale-doc protection, and unsupported-completion-claim guards.
- Current Ancient docs, release checklist, test plan, API discovery, completion audit, localization validation, manual matrix, and handoff docs now describe the v4.2 behavior and keep manual runtime gates pending.

Validation status:

- `dotnet build EZMicroBalance.sln` passed once after the first code pass with 0 warnings and 0 errors.
- The full required build/test/format/publish/test/diff-check and final package hash refresh are still pending for this entry and will supersede the older package hashes below.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-06 01:25:00 +02:00

Goal summary:

- Close private-beta hardening gates for the current EZ Micro Balance Ancient reward rebalance pass.
- Revalidate Jewelry Box actual-deck `Apotheosis` / `缁佺偛瀵瞏 behavior, Simplified Chinese localization guards, Prismatic Gem source/manual coverage, and release packaging.

Jewelry Box review:

- `VakuRewardPatches.cs` now scopes the non-Innate behavior to Jewelry Box-created `Apotheosis` instances via `JewelryBoxApotheosisMarker`.
- The `Apotheosis.CanonicalKeywords` postfix removes `CardKeyword.Innate` only when `JewelryBoxApotheosisMarker.IsMarked(__instance)` is true, so other `Apotheosis` sources keep base-game Innate behavior.
- The marker is written to `SerializableCard.Props.bools` in `CardModel.ToSerializable()` and restored in `CardModel.FromSerializable()`, so save/load and room-transition paths have a persistence hook.
- `JeweledMaskFreePower.cs` contains a no-op saved-property-name registration for `EZMicroBalanceNonInnateApotheosis`; this is required so the saved-property name is known to the game's packet serialization map and does not change Jeweled Mask gameplay.

Localization and Prismatic Gem review:

- Active zhs localization scan found no raw `Swift`, `Apotheosis`, `Enthralled`, `Wish`, `Relax`, `Folly`, `Debt`, `Boss`, `Innate`, `Exhaust`, `Ethereal`, `Retain`, or `Eternal` in player-facing zhs localization.
- Beautiful Bracelet zhs text contains `鏉╁懘鈧?`.
- Jewelry Box zhs text contains `缁佺偛瀵瞏 and `娑撳秴鍙块張澶婃祼閺堝ˇ.
- Prismatic Gem source still uses `CardReward.Populate()` state plus `ConditionalWeakTable<CardReward, RewardScreenState>` so reroll reuses the same trigger state; manual matrix keeps first reward, second reward/reroll, and non-normal reward exclusion checks.

Files changed:

- `docs/features/ancients-rework-v4/manual-test-checklist.md`: tightened Jewelry Box save/load and package checklist items.
- `docs/features/ancients-rework-v4/manual-verification-matrix.md`: recorded automated gate results, Jewelry Box marked-only source guard, package path, and save/load row.
- `docs/features/ancients-rework-v4/completion-audit.md`: updated final automated evidence, Jewelry Box gate, zhs localization gate, test count, and package hash.
- `docs/features/ancients-rework-v4/work-log.md`: recorded this closure pass.

Commands and results:

- Closed `SlayTheSpire2.exe` before publish.
- `dotnet publish EZMicroBalance.sln`: passed and installed the Release DLL/manifest/PCK. Godot printed the known non-fatal `sts2` assembly lookup exception during headless scan, then completed `savepack`.
- Historical result for this pass: `dotnet test EZMicroBalance.sln --no-build` passed with the then-current 13 tests. Current status is superseded by the later 28-test overnight refresh.
- Package created at `publish/EZMicroBalance-v0.1.0-private-beta.0.zip`.
- Historical package SHA256 for this pass: `0FC0011E701A8734C7BF1EEB3AFD132F8DD5F2030BF604B9E15BF8205DD91E79`. Current package hash is superseded by the later overnight refresh.

Remaining blocker:

- Manual in-game verification is still required for normal Steam-client Mod Settings, Jewelry Box deck/opening-hand/save-load behavior, Prismatic Gem reroll/non-normal reward behavior, Simplified Chinese text/tooltip inspection, live gameplay matrix, clean commit, and user-approved push.

## 2026-05-06 Jewelry Box non-Innate regression fix

Goal summary:

- Fix Jewelry Box so the actual `Apotheosis` / `缁佺偛瀵瞏 card added to the deck is non-Innate, not only the relic tooltip or hover preview.

Finding:

- Instance-level `CardCmd.RemoveKeyword(card, CardKeyword.Innate)` was not sufficient as a release guarantee because `Apotheosis` declares `Innate` in `CanonicalKeywords`, and card serialization does not persist arbitrary keyword removals.
- This could allow the Jewelry Box `Apotheosis` to regain `Innate` after deck serialization, room transition, save/load, or other keyword recalculation paths.

Files changed:

- `EZMicroBalanceCode/Ancients/Patches/VakuRewardPatches.cs`: added an instance marker for Jewelry Box `Apotheosis` and filters `Innate` only for marked `Apotheosis` canonical keywords.
- `tests/EZMicroBalance.Tests/ReleaseArtifactTests.cs`: added a source/documentation regression guard for the Jewelry Box non-Innate marker and serialization hooks.
- `docs/features/ancients-rework-v4/manual-verification-matrix.md` and `docs/features/ancients-rework-v4/manual-test-checklist.md`: added focused manual verification for deck inspection, combat start, and save/load behavior.

Expected manual result:

- Jewelry Box adds `Apotheosis` / `缁佺偛瀵瞏.
- That specific card does not show `Innate` / `閸ョ儤婀乣 in deck inspection.
- That specific card is not forced into the opening hand by `Innate` after entering combat in the same run session.
- Other non-Jewelry Box `Apotheosis` sources should keep the base game's normal `Innate` behavior unless a future design changes them explicitly.

Follow-up:

- Save/load persistence for the Jewelry Box non-Innate marker remains intentionally unaccepted until a supported serialization strategy is verified. Avoid custom `SavedProperties` name injection for this marker because StS2 maps saved property names through a fixed cache and unverified custom names can create startup/save/sync instability.

## 2026-05-06 01:03:14 +02:00

Goal summary:

- Complete a strict Simplified Chinese localization and tooltip pass for EZ Micro Balance without gameplay, manifest, package version, website, Ascension, or new-character changes.

Files changed:

- `EZMicroBalance/localization/zhs/relics.json`: removed raw English leftovers and corrected `Debt` wording to `閸婂搫濮焋.
- `EZMicroBalance/localization/eng/cards.json` and `EZMicroBalance/localization/zhs/cards.json`: added `DEBT`, `ENTHRALLED`, and `FOLLY` title overrides for parity; tightened `ENTHRALLED.description` zhs wording.
- `EzDailyContent/localization/eng/cards.json`, `EzDailyContent/localization/zhs/cards.json`, and `EzDailyContent/localization/zhs/relics.json`: mirrored localization-only text for legacy consistency.
- `EZMicroBalanceCode/Ancients/Common/JeweledMaskFreePower.cs`: added zhs `CardModifierLoc` strings for the custom Jeweled Mask enchantment tooltip.
- `tests/EZMicroBalance.Tests/ReleaseArtifactTests.cs`: added zhs banned-English guards, a Beautiful Bracelet `Swift` regression test, and a Jeweled Mask custom-enchantment zhs guard.
- `docs/features/ancients-rework-v4/localization-validation.md`: replaced the older note with the glossary, evidence, exact replacements, tooltip findings, and automated guard summary.
- `docs/features/ancients-rework-v4/manual-test-checklist.md` and `docs/features/ancients-rework-v4/manual-verification-matrix.md`: added zhs spot checks for the affected relic/card/tooltip surfaces.

Localization evidence:

- Local base-game zhs resources from `SlayTheSpire2.pck` were consulted during the localization sprint, but the historical extracted text in this log was mojibake and is no longer kept as terminology evidence.
- No official zhs `off-color` match was found locally at the time of the sprint; keep the project term under review before public release if the base game adds an official term.
- Base zhs still exposed `ROOM_BOSS.title` as raw `Boss` at the time of the sprint; do not use this historical log as current localization authority.

Tooltip findings:

- Keyword tooltips come from `card_keywords.json`; no override is needed for the keywords used by this mod.
- Swift is an enchantment, not a keyword/static hover tip; its official zhs tooltip source is base `enchantments.json`.
- `DynamicVar("Swift", 2m)` remains a numeric variable provider and is not the localized display term.
- BaseLib `ILocalizationProvider` strings are injected into the active localization table, so Jeweled Mask's custom enchantment needed language-aware code strings.

Commands and results:

- `rg` over active and legacy zhs localization for the sprint banned terms plus `濞嗙姵顑檂: no matches.
- `SlayTheSpire2.exe` was running as PID 45312, so build/publish/test validation used temporary `ModsPath` and `STS2_PATH` under `%TEMP%\EZMicroBalanceBuildSmoke`.
- `dotnet build EZMicroBalance.sln /p:ModsPath="$env:TEMP\EZMicroBalanceBuildSmoke\mods\"`: passed with 0 warnings and 0 errors.
- `dotnet publish EZMicroBalance.sln /p:ModsPath="$env:TEMP\EZMicroBalanceBuildSmoke\mods\"`: exited 0 and exported the temp PCK; Godot printed a non-fatal `sts2` assembly lookup exception during headless project scan.
- `dotnet test EZMicroBalance.sln --no-build` with `STS2_PATH=%TEMP%\EZMicroBalanceBuildSmoke`: passed, 12 tests total.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.

Remaining blocker:

- Manual in-game Simplified Chinese inspection is still required for Beautiful Bracelet/Swift tooltip, Jeweled Mask custom enchantment tooltip, changed relic/card text, Mod Settings load, live gameplay matrix, save/load checks, clean commit, and user-approved push.

## 2026-05-05 21:29:45 +02:00

Goal summary:

- Add an automated guard for the original Prismatic Gem reroll-fix documentation and manual-test deliverables.

Files changed:

- `tests/EZMicroBalance.Tests/ReleaseArtifactTests.cs`: added `PrismaticGemRerollFixHasDocumentedEvidenceAndManualCoverage`.
- `docs/dev-environment.md`, `docs/test-plan.md`, `docs/release-checklist.md`, and this work log: updated latest automated test count and coverage description.

Historical test coverage added:

- At that time, verified the Prismatic Gem source contained the `CardReward.Populate()` context patch, `ConditionalWeakTable<CardReward, RewardScreenState>`, one-time saved counter increment, rightmost-slot replacement, same-rarity off-color candidate search, and normal encounter reward filter. v4.3 supersedes the rightmost-slot expectation with all-slot replacement.
- Verifies `api-discovery.md` still records `CardReward.Reroll()` evidence, per-screen state strategy, one-time counter increment, and reroll state reuse.
- Verifies `manual-verification-matrix.md` still contains the exact manual Prismatic Gem test sections: first normal reward reroll, second normal reward reroll, and non-normal reward exclusions.

Commands and results:

- `dotnet build` and `dotnet test EZMicroBalance.sln --no-build` were first run in parallel and produced transient process-lock warnings from `testhost`; this was discarded as final evidence.
- Sequential `dotnet build`: passed with 0 warnings and 0 errors.
- Sequential `dotnet test EZMicroBalance.sln --no-build`: passed, 6 tests total.
- `dotnet publish`: passed and reinstalled the Release `EZMicroBalance.dll`.
- Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed, 6 tests total against the installed Release DLL.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check -- EzDailyContentCode EzDailyContent docs/features/ancients-rework-v4 docs/dev-environment.md EzDailyContent.json`: passed with only the existing CRLF normalization warning for `docs/dev-environment.md`.
- `git diff --check -- EZMicroBalanceCode EZMicroBalance tests EZMicroBalance.csproj EZMicroBalance.sln EZMicroBalance.json docs/PROJECT_MAP.md docs/test-plan.md docs/release-checklist.md AGENTS.md`: passed.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-05 21:31:41 +02:00

Goal summary:

- Audit the dirty tree for non-release local material before any future clean commit.

Finding:

- `art_pipeline/` and `asset/` are local/generated art and calibration folders. The release docs already describe them as outside the active package, and `export_presets.cfg` excludes them from the PCK.
- They were still untracked in `git status`, which made them easy to accidentally include in a future release commit.

Files changed:

- `.gitignore`: added `/art_pipeline/` and `/asset/`.
- `docs/PROJECT_MAP.md`: clarified that these folders are ignored local material, not untracked release candidates.
- This work log: recorded the dirty-tree review decision.

Verification:

- `git check-ignore -v art_pipeline asset art_pipeline/generated/ancient_lotha_bg_v1_v001.png asset/ancient_calibration_pack/ancient_visual_bible_v1.md`: confirmed both folders and nested files are ignored by the new `.gitignore` entries.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-05 21:33:00 +02:00

Goal summary:

- Add a durable completion audit instead of relying only on chat summaries.

Files changed:

- `docs/features/ancients-rework-v4/completion-audit.md`: added a prompt-to-artifact checklist mapping the active private beta/build-review requirements to direct evidence and remaining blockers.
- `docs/PROJECT_MAP.md`: linked the new completion audit document.

Completion decision:

- The audit confirms build, test, publish, formatting, package, Harmony target, Prismatic Gem documentation coverage, and controlled smoke gates are green.
- The audit also confirms normal Steam-client Mod Settings verification, manual gameplay matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.
- `update_goal` was not called.

## 2026-05-05 21:34:38 +02:00

Goal summary:

- Verify that deleted historical planning docs were actually preserved in the archive.

Commands and results:

- `git diff --name-only --diff-filter=D -- docs`: listed 13 deleted tracked planning/research docs.
- Archive existence check: every deleted doc had a corresponding file under `docs/archive/legacy-planning/`.
- Raw blob hash comparison did not match because archive files differ in encoding/line-ending bytes.
- Normalized line-content comparison using `git show HEAD:<path>` versus each archive file passed for all 13 files.

Files changed:

- `docs/features/ancients-rework-v4/completion-audit.md`: recorded the archive preservation evidence.
- This work log: recorded the archive integrity review.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-05 21:35:47 +02:00

Goal summary:

- Audit untracked/new files for accidental large binaries, build output, or non-release package material before any future commit.

Commands and results:

- `git ls-files --others --exclude-standard`: reported 98 untracked files after ignoring `art_pipeline/` and `asset/`.
- Largest untracked files were source/docs/template images; no oversized release-risk binary was found.
- Extension/path scan found no untracked `.dll`, `.pck`, `.exe`, archives, `bin/`, `obj/`, `.godot/`, `.tools/`, `publish/`, or `packages/` entries.
- Active `EZMicroBalance/` resource scan found only images, `.import` metadata, and localization JSON; no source, docs, DLLs, PCKs, executables, or archives.

Files changed:

- `docs/features/ancients-rework-v4/completion-audit.md`: recorded the untracked-file/package-material audit.
- This work log: recorded the commit-readiness review.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-05 21:36:56 +02:00

Goal summary:

- Remove the `.gitignore` CRLF normalization warning introduced during the local-art ignore update.

Files changed:

- `.gitignore`: normalized to UTF-8 without BOM and LF line endings while preserving the new `/art_pipeline/` and `/asset/` ignore rules.
- This work log: recorded the hygiene cleanup.

Verification:

- `git diff --check -- .gitignore docs/features/ancients-rework-v4/completion-audit.md docs/features/ancients-rework-v4/work-log.md`: passed without warnings.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-05 21:38:02 +02:00

Goal summary:

- Remove the remaining `docs/dev-environment.md` normalization warning from the required diff check.

Files changed:

- `docs/dev-environment.md`: normalized to UTF-8 without BOM and LF line endings.
- `docs/features/ancients-rework-v4/completion-audit.md`: updated the current diff-check evidence to state that required and active release path checks pass with no warnings.
- This work log: recorded the hygiene cleanup.

Verification:

- `git diff --check -- EzDailyContentCode EzDailyContent docs/features/ancients-rework-v4 docs/dev-environment.md EzDailyContent.json`: passed with no warnings.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-05 21:39:21 +02:00

Goal summary:

- Audit `.cs.uid` tracking consistency before any future commit.

Commands and results:

- Checked every `.cs` file under `EZMicroBalanceCode/` and `EzDailyContentCode/` for a matching `.cs.uid` companion.
- Result: every Godot-imported C# source in those two trees has a `.cs.uid` file.
- `tests/EZMicroBalance.Tests/ReleaseArtifactTests.cs` has no `.cs.uid`, intentionally, because the test project is not a Godot-imported script tree.

Files changed:

- `docs/features/ancients-rework-v4/completion-audit.md`: recorded the `.cs.uid` policy evidence.
- This work log: recorded the UID audit.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-05 21:41:56 +02:00

Goal summary:

- Audit current-facing docs for stale setup-era `EzDailyContent` language.

Findings:

- `docs/BETA_COMPATIBILITY.md` still described Mod Settings verification for `EzDailyContent` without distinguishing legacy from active release.
- `docs/REMOTE_DEVELOPMENT_SETUP.md` still listed `EzDailyContent` as the project and expected published output.
- `docs/SETUP_SPEC.md` still intentionally records the original setup baseline, but it did not say it was historical.

Files changed:

- `docs/BETA_COMPATIBILITY.md`: updated active compatibility status and update procedure for `EZMicroBalance`, while preserving the legacy baseline result.
- `docs/REMOTE_DEVELOPMENT_SETUP.md`: updated active project name, expected published output, Mod Settings checks, and legacy-disable note.
- `docs/SETUP_SPEC.md`: added a historical note pointing readers to current release docs.
- `docs/features/ancients-rework-v4/completion-audit.md`: recorded the stale-doc audit.

Verification:

- `git diff --check -- docs/BETA_COMPATIBILITY.md docs/REMOTE_DEVELOPMENT_SETUP.md docs/SETUP_SPEC.md`: passed with no warnings.
- Stale-current-doc scan for `EzDailyContent appears`, `EzDailyContent is enabled`, and `dotnet list EzDailyContent` now only reports the historical `SETUP_SPEC.md` Mod Settings lines.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-05 21:43:51 +02:00

Goal summary:

- Convert the stale-current-doc scan into an automated regression guard.

Files changed:

- `tests/EZMicroBalance.Tests/ReleaseArtifactTests.cs`: added `CurrentSetupDocsPointAtActiveMod`.
- `docs/dev-environment.md`, `docs/test-plan.md`, `docs/features/ancients-rework-v4/completion-audit.md`, and this work log: updated the automated test count and coverage description.

Test coverage added:

- Verifies `docs/BETA_COMPATIBILITY.md` points update commands at `EZMicroBalance.csproj`.
- Verifies `docs/REMOTE_DEVELOPMENT_SETUP.md` identifies `EZMicroBalance` as the active project and lists `mods\EZMicroBalance` outputs.
- Verifies those two current-facing docs no longer contain the stale active-project strings for `EzDailyContent`.
- Verifies `docs/SETUP_SPEC.md` explicitly marks itself as historical.

Commands and results:

- `dotnet build`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 7 tests total.
- `dotnet publish`: passed and reinstalled the Release `EZMicroBalance.dll`.
- Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed, 7 tests total against the installed Release DLL.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check -- tests docs/BETA_COMPATIBILITY.md docs/REMOTE_DEVELOPMENT_SETUP.md docs/SETUP_SPEC.md docs/dev-environment.md docs/test-plan.md docs/features/ancients-rework-v4 .gitignore`: passed with no warnings.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-05 21:45:46 +02:00

Goal summary:

- Add an automated guard for active project/export isolation from legacy `EzDailyContent` sources.

Files changed:

- `tests/EZMicroBalance.Tests/ReleaseArtifactTests.cs`: added `ActiveProjectDoesNotCompileOrPackageLegacySources`.
- `docs/dev-environment.md`, `docs/test-plan.md`, `docs/features/ancients-rework-v4/completion-audit.md`, and this work log: updated automated test count and coverage description.

Test coverage added:

- Verifies `EZMicroBalance.csproj` compiles `EZMicroBalanceCode/**/*.cs`, loads `EZMicroBalance` localization as additional files, and does not include `EzDailyContentCode` or `EzDailyContent` paths.
- Verifies `EZMicroBalance.sln` contains `EZMicroBalance` plus tests and does not contain `EzDailyContent.csproj`.
- Verifies `export_presets.cfg` uses selected-resource export, includes active `EZMicroBalance` resources, and excludes legacy/source/docs/archive paths.

Commands and results:

- `dotnet build`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 8 tests total.
- `dotnet publish`: passed and reinstalled the Release `EZMicroBalance.dll`.
- Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed, 8 tests total against the installed Release DLL.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check -- tests docs/dev-environment.md docs/test-plan.md docs/features/ancients-rework-v4`: passed with no warnings.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-05 21:48:48 +02:00

Goal summary:

- Add an automated installed-manifest parity guard.

Files changed:

- `tests/EZMicroBalance.Tests/ReleaseArtifactTests.cs`: added `InstalledManifestMatchesRepositoryManifest`.
- `docs/dev-environment.md`, `docs/test-plan.md`, `docs/features/ancients-rework-v4/completion-audit.md`, and this work log: updated automated test count and artifact coverage description.

Test coverage added:

- Verifies `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance\EZMicroBalance.json` exists and JSON-normalizes to the same content as repository `EZMicroBalance.json`.

Commands and results:

- `dotnet build`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 9 tests total.
- `dotnet publish`: passed and reinstalled the Release `EZMicroBalance.dll` plus manifest.
- Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed, 9 tests total against installed release artifacts.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed across the full tracked diff with no warnings.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-06 00:19:08 +02:00

Goal summary:

- Continue content/release hardening without starting website work.
- Correct stale current-facing manual verification docs that still targeted legacy `Easy Content` / `EzDailyContent`.

Finding:

- `docs/features/ancients-rework-v4/manual-test-checklist.md` still described the old single-mod architecture, expected `<GameRoot>\mods\EzDailyContent`, and asked testers to enable Easy Content / EzDailyContent.
- This conflicted with the independent private-beta target `EZMicroBalance` and could cause duplicate Ancient patches if the legacy scaffold was enabled during manual testing.

Files changed:

- `docs/features/ancients-rework-v4/manual-test-checklist.md`: retargeted the checklist to manifest id `EZMicroBalance`, expected folder `mods\EZMicroBalance`, and added an explicit legacy-disable/absent check.
- `tests/EZMicroBalance.Tests/ReleaseArtifactTests.cs`: extended `CurrentSetupDocsPointAtActiveMod` so the manual checklist must target `EZMicroBalance` and must not reintroduce the old active `EzDailyContent` path/checks.
- `docs/test-plan.md`, `docs/dev-environment.md`, and `docs/features/ancients-rework-v4/completion-audit.md`: recorded the updated doc-targeting coverage and latest verification.

Commands and results:

- `dotnet build`: compiled projects but failed during post-build install copy because running process `SlayTheSpire2.exe` PID 45312 locked `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance\EZMicroBalance.dll`.
- `dotnet build EZMicroBalance.sln /p:ModsPath="$env:TEMP\EZMicroBalanceBuildSmoke\mods\"`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 9 tests total.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check -- docs\features\ancients-rework-v4\manual-test-checklist.md tests\EZMicroBalance.Tests\ReleaseArtifactTests.cs docs\test-plan.md docs\dev-environment.md docs\features\ancients-rework-v4\completion-audit.md docs\features\ancients-rework-v4\work-log.md`: passed with no warnings.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-06 - Overnight Hardening Integration

Goal summary:

- Integrate subagent review results for the existing Ancient reward implementation while preserving the active `EZMicroBalance` manifest id and isolated project structure.
- Expand automated source guards for release-critical Ancient behaviors and keep manual runtime gates explicit.

Findings fixed:

- Claws pickup used a dead base-relic Harmony path. Added a direct `Claws.AfterObtained` prefix that reuses the existing curse/Wish implementation.
- Jewelry Box `Apotheosis` marker relied only on an in-memory `ConditionalWeakTable`. Added `SavedSpireField<CardModel,bool>` persistence so marked non-Innate `Apotheosis` survives serialization paths.
- Whispering Earring checked all playable cards through `CanPlayTargeting`; self-targeting and no-target cards can now pass without enemy/ally targeting requirements.

Test coverage added:

- `tests/EZMicroBalance.Tests/AncientBehaviorGuardTests.cs` covers localization parity, package isolation, Harmony target declarations, Prismatic Gem reroll state, Jewelry Box marker persistence, Jeweled Mask custom enchantment, Pael's Tooth counters, Debt/Folly text/source behavior, generated-card cleanup, Meat Cleaver safety, and release checklist integrity.
- Ascension-specific guards are tracked separately in `tests/EZMicroBalance.Tests/AscensionFeatureGuardTests.cs`.

Commands:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 28 tests total after the Ancient and Ascension guard additions.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-06 - Overnight Publish And Package Refresh

Goal summary:

- Re-run release validation after code, localization/resource, Ascension, documentation, and art changes.
- Rebuild the private-beta package from the same installed artifacts used by tests.

Commands and results:

- `Get-Process SlayTheSpire2 -ErrorAction SilentlyContinue`: no running game process was found.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `dotnet publish EZMicroBalance.sln`: passed; built Release and installed `EZMicroBalance.dll` plus `EZMicroBalance.json`.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 28 tests total after publish.
- Rebuilt `publish/EZMicroBalance-v0.1.0-private-beta.0.zip` from installed artifacts.

Release artifact evidence:

- Zip SHA256: superseded by the post-smoke constructor-fix package refresh below.
- Installed/staging/versioned package DLL SHA256: superseded by the post-smoke constructor-fix package refresh below.
- Installed/staging/versioned package PCK SHA256: superseded by the post-smoke constructor-fix package refresh below.
- Installed/staging/versioned package manifest SHA256: `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`.
- Zip entries: `EZMicroBalance/EZMicroBalance.dll`, `EZMicroBalance/EZMicroBalance.json`, `EZMicroBalance/EZMicroBalance.pck`, and `EZMicroBalance/README_INSTALL.txt`.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-06 - Final Smoke Regression Fix

Finding:

- A bounded `--force-steam off` smoke exposed a startup `MissingMethodException` for `EZMicroBalanceCode.Ascension.RootBudCombatHook`.
- Evidence from `godot.log`: StS2 model database initialization dynamically creates concrete `AbstractModel` types and requires parameterless constructors.

Fix:

- Added parameterless constructors to `RootRunHook` and `RootBudCombatHook`.
- Kept the active hook instances stateful through the existing `RunState`/`CombatState` constructors.
- Made parameterless instances inactive by storing nullable state and returning `ShouldReceiveCombatHooks => combatState != null`.
- Extended `AscensionFeatureGuardTests` so future hook models keep parameterless startup-safe constructors.

Final commands and results after the fix:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 28 tests total.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `dotnet publish EZMicroBalance.sln`: passed; built Release, copied DLL/manifest, exported the selected-resource PCK, and reimported the generated `mod_image.png`.
- Rebuilt `publish/EZMicroBalance-v0.1.0-private-beta.0.zip` from installed artifacts.
- Final bounded `--force-steam off` smoke: BaseLib initialized, EZ Micro Balance initialized, BaseLib reported 7 SavedSpireFields, and the game reached main menu. The smoke process was stopped after main menu and profile settings were restored.
- Later package-refresh smoke temporarily enabled only BaseLib and EZ Micro Balance, initialized the final installed EZ Micro Balance DLL/PCK, BaseLib reported 8 SavedSpireFields, and the game reached main menu. Profile settings were restored after the smoke.

Final artifact evidence:

- Zip SHA256: `DECA87D50B574CB411BB26B696C5DFC055A8C4542D47461EA06553AAAEB9834E`.
- Installed/staging/versioned package DLL SHA256: `8983983D3C9AB814DE3D223E2836C424020D25F61BA9378CEA188B2D099C8CEB`.
- Installed/staging/versioned package PCK SHA256: `9088D820A4F676ECE01FF8405862C64831B03D15F90D7A5D082E24B0FAD557B4`.
- Installed/staging/versioned package manifest SHA256: `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live manual feature matrix, save/load checks, disable gameplay checks, clean commit, and user-approved push remain pending.

## 2026-05-06 16:59:34 +02:00

Goal summary:

- Execute the Ancient v4.2 end-to-end validation loop against the active `EZMicroBalance` solution and installed artifacts.
- Refresh current package/hash documentation without expanding Ascension scope or changing manifest ids.

Files read:

- `AGENTS.md`
- `README.md`
- `docs/features/ancients-rework-v4/source-design.md`
- `C:\Users\Jack\Downloads\sts2_ancients_rework_v4_2_next_plan.md`
- Active Ancient implementation, localization, automated tests, release docs, manual matrix, API discovery, completion audit, and package handoff docs.

Commands and results:

- Confirmed the archived v4.2 next-plan file matches the downloaded plan by SHA256.
- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 62 passed, 0 failed, 0 skipped before publish.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `dotnet publish EZMicroBalance.sln`: passed; Release DLL and manifest were copied, and the installed PCK remained current.
- Rebuilt `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` from the final installed DLL/JSON/PCK artifacts.
- First post-package `dotnet test EZMicroBalance.sln --no-build` failed only because the rebuilt zip hash was stale in docs.
- After refreshing hash-bearing docs, `dotnet test EZMicroBalance.sln --no-build`: passed, 62 passed, 0 failed, 0 skipped.
- `git diff --check`: exit code 0 with the existing `EzDailyContent.json` CRLF normalization warning.

Current artifact evidence:

- Zip SHA256: `4E17AF7B9DBECF6F7DDFFC0AEBFD63FD8311CA41C24F78837DEB489410D8896D`.
- Installed/staging/versioned/extracted zip DLL SHA256: `2E869A0C6F22845AE150D35B64B508A6B7B84DE191D7FFA47AE29599F286D651`.
- Installed/staging/versioned/extracted zip PCK SHA256: `D043D5F06440ACA128AA7153BE0B7C0B1DB7F95AD4DE8197826A0A1F07BFBF1D`.
- Installed/staging/versioned/extracted zip manifest SHA256: `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`.

Files changed:

- `docs/dev-environment.md`, `docs/release-checklist.md`, `docs/private-beta-verification-handoff.md`, `docs/features/ancients-rework-v4/completion-audit.md`, `docs/features/ancients-rework-v4/api-discovery.md`, `docs/features/ancients-rework-v4/manual-verification-matrix.md`, and `docs/test-plan.md`: refreshed the current v4.2 validation and package hash evidence.
- This work log: recorded the validation pass and current artifact hashes.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live Ancient gameplay matrix, save/load checks, disable gameplay checks, multiplayer disposition, author decision, clean commit, and user-approved push remain pending.

## 2026-05-07 - Black Star Obtain-Hook Bugfix

Scope:

- Fixed live-tester feedback that Black Star did not grant its act-3+ immediate random relic when picked up.
- Kept the design requirement unchanged: this compensation applies only in act 3 or later.

Implemented:

- Moved Black Star compensation from the broad `RelicModel.AfterObtained` prefix to a targeted postfix on `RelicCmd.Obtain(RelicModel, Player, int)` so it runs after the relic has actually been obtained by reward/event flows.
- The compensation still checks zero-based `CurrentActIndex >= 2`, pulls one random relic from the normal front relic source, obtains it through `RelicCmd.Obtain(...)`, and logs either the applied relic id or the pre-act-3 skip reason.
- Ancient source guards now require the targeted `RelicCmd.Obtain(...)` patch.

Validation:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 69/69 after the package/hash docs were refreshed.
- `dotnet publish EZMicroBalance.sln`: passed.
- Package zip SHA256: `65277A910077FA655C9F8D5FD15C2A6B4515228FDD27BD916BF85A7CEEB33FB4`.
- DLL SHA256: `022A1ADFD2CDD9C3755ED248FE087220E32539C4EEC1BECAECF5A3A9BF612365`.
- PCK SHA256: `BD14EB5924F852873DAFA570162BE039366BB13334B737E0792A3F9B0B1F59AA`.
- Manifest SHA256: `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`.

Remaining blocker:

- Live act-3+ Black Star pickup verification is still pending.

## 2026-05-06 - Velvet Choker Card Library Crash Fix

Issue:

- Manual Steam-client testing with only BaseLib and EZ Micro Balance enabled showed the card encyclopedia / Card Library failed to display cards.
- `godot.log` reported `CanonicalModelException` from `VelvetChokerSoftLimitTracker.ShouldTax(CardModel card)` while `NCardGrid` sorted canonical card models such as `FeelNoPain` and `Enthralled`.

Implementation:

- `VelvetChokerSoftLimitTracker.ShouldTax` now rejects non-combat, suppressed, clone, and non-hand contexts before reading `CardModel.Owner`.
- Added `TryGetOwner(CardModel)` to catch canonical card models and return `null`, so library/canonical cards never receive the soft-limit tax and never crash card sorting.
- Added source guard coverage to ensure the non-combat/library rejection remains before owner access.
- Updated the manual verification matrix and test plan with a card encyclopedia regression check.

Validation:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 68 passed, 0 failed, 0 skipped before publish.
- `dotnet publish EZMicroBalance.sln`: passed; Release DLL/manifest installed.
- First post-publish `dotnet test EZMicroBalance.sln --no-build`: failed only on expected package/hash drift after the installed DLL changed.
- Rebuilt package staging, versioned package, and `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` from the published installed artifacts.
- Final post-package `dotnet test EZMicroBalance.sln --no-build`: passed, 68 passed, 0 failed, 0 skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with existing CRLF normalization warnings for `EzDailyContent.json` and `docs/dev-environment.md`.

Current artifact hashes:

- Zip SHA256: `32398CB318EC77F97F5DF8427AA9990851BE33A4722A8B9DD4D70541E51512B0`.
- DLL SHA256: `836EAF67C7E06D7E084522F4790907DF5B88CCA6AF12A121479F34E5B1DBE783`.
- PCK SHA256: `1E845AE8EE5A8456D963BADD6B09BEAA6A3E9EB64FAE1475B19C9C6D0D96B7B3`.
- Manifest SHA256: `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`.

Remaining blocker:

- Re-test the card encyclopedia / Card Library in the normal Steam client with only BaseLib and EZ Micro Balance enabled, then inspect `godot.log` for absence of `VelvetChokerSoftLimitTracker.ShouldTax` and `CanonicalModelException`.
- Normal Steam-client Mod Settings verification, live Ancient gameplay matrix, save/load checks, disable gameplay checks, multiplayer disposition, author decision, clean commit, and user-approved push remain pending.

## 2026-05-06 - Vakuu Distinguished Cape Replacement Validation Refresh

Implementation notes:

- `DistinguishedCape`: final unaffordable Vakuu path is a `Vakuu.GenerateInitialOptions` postfix that replaces a Cape roll with a payable Pool 2 option before generated choices are stored; a localized locked Cape option remains only as fallback.
- `PrismaticGem`: reward-screen hint guard remains non-silent: the `_banner` field is runtime-guarded, `UI/Banner` is the fallback path, and final diagnostics point testers to visible all-off-color cards plus relic hover count.
- Corrected the Ascension A20 manual checklist wording so the existing gated-slice guard recognizes that intermission after boss 1 remains deferred.
- Rebuilt package staging, versioned package, and `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` from the published installed artifacts after the Release DLL/PCK changed.

Validation:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --filter "FullyQualifiedName~AncientBehaviorGuardTests"`: passed, 16/16.
- `dotnet test EZMicroBalance.sln --no-build`: passed before publish/package refresh, 64/64.
- `dotnet publish EZMicroBalance.sln`: exit code 0; Godot printed the known non-fatal `sts2` assembly scan exception and completed `savepack`.
- First post-publish `dotnet test EZMicroBalance.sln --no-build`: failed on expected package/hash drift, then `dotnet test EZMicroBalance.sln` passed after package/hash doc refresh and rebuilding stale test assemblies.
- Final `dotnet test EZMicroBalance.sln --no-build`: passed, 64/64.
- Final `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- Final `git diff --check`: exit code 0 with the existing `EzDailyContent.json` CRLF normalization warning.

Current artifact hashes:

- Zip SHA256: `1527C0329639244B530B2CD8E1E6EAD1480F524CB85FE3207548F5259A09389C`.
- DLL SHA256: `B4E40612454FE2591CDB57BFDBC9BD0E4839F63E34A2A58AE818219BEFB05C60`.
- PCK SHA256: `1E845AE8EE5A8456D963BADD6B09BEAA6A3E9EB64FAE1475B19C9C6D0D96B7B3`.
- Manifest SHA256: `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`.

## 2026-05-06 - Vakuu Distinguished Cape Option Replacement

Implementation notes:

- `DistinguishedCape`: moved the low-Max-HP option guard to `Vakuu.GenerateInitialOptions`, so an unaffordable Cape roll is replaced by a payable Pool 2 option before Vakuu stores generated choices.
- Kept a localized locked Cape option as a defensive fallback if the same-pool replacement candidates are unavailable, avoiding silent option-list shrinkage.

Validation:

- Pending in this entry: targeted guard tests and build rerun.

## 2026-05-06 - Review Guard Expansion: Cape No-shrink and Prismatic Hint Fallback

Goal summary:

- Expand tests and docs so two review findings cannot regress: Distinguished Cape must not shrink Vakuu options when unaffordable, and Prismatic Gem reward-screen hints must not be silent reflection-only behavior.

Implementation notes:

- `Distinguished Cape`: changed the unaffordable Vakuu path from removing the Cape option to replacing it with a same-pool payable option, preserving three normal visible reward choices while retaining the v4.3 max-HP pay gate; a localized locked `EventOption` remains as fallback only.
- `Prismatic Gem`: kept the private `_banner` hint path, added `UI/Banner` node lookup fallback, and added log diagnostics for field success, fallback success, and missing-banner fallback evidence.

Docs/tests:

- Updated current docs and manual verification text to require Cape replacement/no-shrink behavior, with the locked reason documented only as fallback.
- Updated Prismatic Gem manual/docs to require reward-screen hint diagnostics and fallback evidence through `godot.log`, relic hover count, and visible all-off-color reward cards.
- Added source/doc regression guards in the automated test project.

## 2026-05-06 - Ancient v4.3 Exact Text and Package Refresh

Goal summary:

- Keep the already-implemented v4.3 gameplay behavior intact while tightening player-facing text and package evidence against `C:\Users\Jack\Downloads\sts2_ancients_rework_v4_3_adjustment_plan.md`.
- Do not expand Ascension, change manifest ids, or claim private beta readiness.

Files changed:

- `EZMicroBalance/localization/eng/relics.json`: changed Distinguished Cape English text to `Lose 30% of current Max HP, at least 18. Add 3 Apparitions.`
- `tests/EZMicroBalance.Tests/AncientBehaviorGuardTests.cs`: tightened the Distinguished Cape text guard to the exact v4.3 English wording.
- `docs/features/ancients-rework-v4/source-design.md` and `manual-test-checklist.md`: changed no-space zhs number-format examples from `15` to `18` where they could read as current Cape behavior.
- `docs/features/ancients-rework-v4/work-log.md`: marked old rightmost-slot Prismatic Gem entries as historical and superseded by v4.3 all-slot replacement.
- Current hash docs: refreshed package/PCK hash evidence after the localization export changed the PCK.

Validation:

- `git status --short --branch`: dirty `main...origin/main` with intended migration/docs/test/package changes still uncommitted.
- `Get-Process SlayTheSpire2 -ErrorAction SilentlyContinue`: no process.
- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 63 passed, 0 failed, 0 skipped before publish.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `dotnet publish EZMicroBalance.sln`: passed; Release DLL/manifest installed and selected-resource PCK exported. Godot printed the known non-fatal `sts2` assembly lookup exception during headless scan.
- First post-publish `dotnet test EZMicroBalance.sln --no-build`: failed only on package/hash drift after the PCK changed, 60 passed, 3 failed, 0 skipped.
- `git diff --check`: exit code 0 with the existing `EzDailyContent.json` CRLF normalization warning.
- Rebuilt `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` from the final installed DLL/JSON/PCK artifacts.
- Final post-package `dotnet test EZMicroBalance.sln --no-build`: passed, 63 passed, 0 failed, 0 skipped.
- Final `git diff --check`: exit code 0 with the same existing `EzDailyContent.json` CRLF normalization warning.

Current artifact hashes:

- Zip SHA256: `BED7E22F92FCEEAAD97CBFD2C1A71ACA81525EF8751B09C8CFA9B3389443A972`.
- DLL SHA256: `4A8E05FFA0EF76F6842C04934BBDF85C7B2D2F4B50881F0A5313ABE0EE41FE3C`.
- PCK SHA256: `58570B03E0B85654DCF78E27BBE75F3789B88C2CF9727AB92F18EF1B5DF6E91D`.
- Manifest SHA256: `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live Ancient gameplay matrix, save/load checks, disable gameplay checks, multiplayer disposition, author decision, clean commit, and user-approved push remain pending.

## 2026-05-06 - Ancient v4.3 Adjustment Pass

Goal summary:

- Implement the v4.3 adjustment plan from `C:\Users\Jack\Downloads\sts2_ancients_rework_v4_3_adjustment_plan.md` as the current Ancient source truth.
- Keep Ascension scope unchanged, preserve manifest ids, and leave manual Steam/gameplay gates pending unless actually executed.

Files read:

- `AGENTS.md`, `README.md`, source design, API discovery, implementation plan, manual checklist, manual matrix, completion audit, and the downloaded v4.3 adjustment plan.

Implementation notes:

- Archived `sts2_ancients_rework_v4_3_adjustment_plan.md` into `docs/features/ancients-rework-v4/`.
- `Distinguished Cape`: changed cost to `max(ceil(currentMaxHp * 0.30), 18)`, filtered the Vakuu option when the player cannot pay, removed the old low-max-HP allowance, kept max-HP loss on `CreatureCmd.LoseMaxHp`, and kept exactly three `Apparition` cards.
- `Prismatic Gem`: changed trigger screens so every visible standard reward option becomes off-color, kept the screen-scoped reroll decision and saved counter, preserved rarity when possible, relaxed rarity before failing, avoided visible duplicates where possible, and kept non-standard reward exclusions.
- Added Prismatic Gem count hover text and a guarded reward-screen banner hint using the local `RelicModel.HoverTips` and `NCardRewardSelectionScreen.RefreshOptions` APIs. The banner hint now validates the private `_banner` field type, falls back to `UI/Banner`, logs one-time diagnostics, and leaves visual placement for manual gameplay verification.
- Preserved v4.2 Velvet Choker behavior and retained the v4.1 decisions for Lordly Parasol, Iron Club, Brilliant Scarf, Jewelry Box, and Seal of Gold / Debt.

Docs/tests:

- Updated English and zhs relic text, including no-space zhs numeric formatting and Prismatic count/reward hints.
- Updated source design, API discovery, implementation plan, high-risk review, localization validation, manual checklist, manual matrix, completion audit, test plan, release checklist, and README so v4.3 is current.
- Expanded automated guards for Distinguished Cape v4.3 math/selectability, Prismatic Gem all-slot reroll-safe behavior, UI hint localization, zhs number formatting, and stale current-doc v4.2 behavior claims.

Validation:

- Final command sequence:
  - `git status --short --branch`: dirty `main...origin/main` with intended migration/docs/test/package changes still uncommitted.
  - `Get-Process SlayTheSpire2 -ErrorAction SilentlyContinue`: no process.
  - `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
  - `dotnet test EZMicroBalance.sln --no-build`: passed, 63 passed, 0 failed, 0 skipped before publish.
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
  - `dotnet publish EZMicroBalance.sln`: passed; Release DLL/manifest copied and PCK remained current.
  - `dotnet test EZMicroBalance.sln --no-build`: passed, 63 passed, 0 failed, 0 skipped after publish.
  - `git diff --check`: exit code 0 with the existing `EzDailyContent.json` CRLF normalization warning.
- Package hashes:
  - Zip SHA256: `BED7E22F92FCEEAAD97CBFD2C1A71ACA81525EF8751B09C8CFA9B3389443A972`.
  - DLL SHA256: `4A8E05FFA0EF76F6842C04934BBDF85C7B2D2F4B50881F0A5313ABE0EE41FE3C`.
  - PCK SHA256: `58570B03E0B85654DCF78E27BBE75F3789B88C2CF9727AB92F18EF1B5DF6E91D`.
  - Manifest SHA256: `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`.

Follow-up validation rerun:

- Re-ran the required v4.3 sequence after confirming the archived v4.3 plan is byte-identical to the downloaded UTF-8 source plan.
- `dotnet build EZMicroBalance.sln`, both `dotnet test EZMicroBalance.sln --no-build` runs, `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `dotnet publish EZMicroBalance.sln`, and `git diff --check` all passed.
- Post-publish package/hash guards passed without drift; the private-beta zip was not rebuilt in this rerun.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live Ancient gameplay matrix, save/load checks, disable gameplay checks, multiplayer disposition, author decision, clean commit, and user-approved push remain pending.

## 2026-05-06 - Vakuu Distinguished Cape Replacement Final Refresh

Implementation notes:

- `DistinguishedCape`: final unaffordable Vakuu path is a `Vakuu.GenerateInitialOptions` postfix that replaces a Cape roll with a payable Pool 2 option before generated choices are stored; a localized locked Cape option remains only as fallback.
- Rebuilt package staging, versioned package, and `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` from the published installed artifacts after the Release DLL/PCK changed.

Validation:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --filter "FullyQualifiedName~AncientBehaviorGuardTests"`: passed, 16/16.
- `dotnet test EZMicroBalance.sln --no-build`: passed before publish/package refresh, 64/64.
- `dotnet publish EZMicroBalance.sln`: exit code 0; Godot printed the known non-fatal `sts2` assembly scan exception and completed `savepack`.
- First post-publish `dotnet test EZMicroBalance.sln --no-build`: failed on expected package/hash drift, then `dotnet test EZMicroBalance.sln` passed after package/hash doc refresh and rebuilding stale test assemblies.
- Final `dotnet test EZMicroBalance.sln --no-build`: passed, 64/64 after the Ascension manual-checklist wording was corrected.
- Final `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- Final `git diff --check`: exit code 0 with the existing `EzDailyContent.json` CRLF normalization warning.

Current artifact hashes:

- Zip SHA256: `1527C0329639244B530B2CD8E1E6EAD1480F524CB85FE3207548F5259A09389C`.
- DLL SHA256: `B4E40612454FE2591CDB57BFDBC9BD0E4839F63E34A2A58AE818219BEFB05C60`.
- PCK SHA256: `1E845AE8EE5A8456D963BADD6B09BEAA6A3E9EB64FAE1475B19C9C6D0D96B7B3`.
- Manifest SHA256: `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live Ancient gameplay matrix, save/load checks, disable gameplay checks, multiplayer disposition, author decision, clean commit, and user-approved push remain pending.

## 2026-05-06 - No-Game Cape/Prismatic Hardening Pass

Goal summary:

- Resolve the latest review's code-fixable Cape and Prismatic blockers without launching Slay the Spire 2 or using Steam-client testing.
- Keep private beta readiness unclaimed; runtime/gameplay verification remains pending.

Subagent findings integrated:

- Vaku Builder confirmed the existing `Vakuu.GenerateInitialOptions` postfix replaces an unaffordable Distinguished Cape slot with a payable Vaku Pool 2 option and does not shrink the option list.
- Prismatic Builder hardened reward-screen hint handling: detached private `_banner` instances are rejected, `UI/Banner` is updated when available, and one-time diagnostics now state that visual placement still requires manual gameplay verification.
- Test Builder strengthened source/documentation guards for Cape option-count preservation and Prismatic fallback evidence.
- Spec Auditor found stale source-doc wording around standard card rewards, same-pool Cape replacement, and v4.3 implementation status; those docs were corrected.
- Localization/UI Text Reviewer found stale Prismatic hover-count manual steps; the manual matrix now expects `1/2` after the first standard reward opens and `0/2` after the second opens. Active zhs JSON stayed clean; long English descriptions remain a runtime UI-wrap risk.

Files changed in this pass:

- `EZMicroBalanceCode/Ancients/Patches/PrismaticGemPatches.cs`
- `tests/EZMicroBalance.Tests/AncientBehaviorGuardTests.cs`
- `docs/features/ancients-rework-v4/source-design.md`
- `docs/features/ancients-rework-v4/sts2_ancients_rework_v4_3_adjustment_plan.md`
- `docs/features/ancients-rework-v4/api-discovery.md`
- `docs/features/ancients-rework-v4/manual-verification-matrix.md`
- `docs/features/ancients-rework-v4/completion-audit.md`
- `docs/features/ancients-rework-v4/README.md`
- current package/hash docs

Validation:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 67 passed, 0 failed, 0 skipped before publish.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `dotnet publish EZMicroBalance.sln`: passed; Release DLL/manifest installed. PCK hash stayed unchanged.
- First post-publish `dotnet test EZMicroBalance.sln --no-build`: failed only on expected package/hash drift after the installed DLL changed.
- Rebuilt package staging, versioned package, and `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` from the published installed artifacts.
- Final post-package `dotnet test EZMicroBalance.sln --no-build`: passed, 67 passed, 0 failed, 0 skipped.

Current artifact hashes:

- Zip SHA256: `4667B4D9B9938F85C651CE2C12DC6ACD5AEB60A96BA5BB3E819EBCDF647E2F29`.
- DLL SHA256: `0B07428096137CA8A46194971CC1DD964618D1B5B967BB903DED085918AB8982`.
- PCK SHA256: `1E845AE8EE5A8456D963BADD6B09BEAA6A3E9EB64FAE1475B19C9C6D0D96B7B3`.
- Manifest SHA256: `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`.

Remaining blocker:

- Normal Steam-client Mod Settings verification, live Ancient gameplay matrix, save/load checks, disable gameplay checks, multiplayer disposition, author decision, clean commit, and user-approved push remain pending.

## 2026-05-08 - Pumpkin Candle Rollback and Quality Flame Hardening

- Restored Pumpkin Candle to vanilla behavior for the active EZMB scope: no `PumpkinCandlePatch`, no `ExtinguishedSentinel`, and no active `PUMPKIN_CANDLE.description` override under `EZMicroBalance/localization/**`.
- Rechecked v0.105.0 source for `MegaCrit.Sts2.Core.Models.Cards.BrightestFlame`: vanilla uses `CardsVar(2)`, `EnergyVar(2)`, `MaxHpVar(1)`, and upgrades both Energy and Cards by +1.
- Changed Quality Flame implementation to patch `BrightestFlame.CanonicalVars` so draw is vanilla +1 at the dynamic variable source, giving draw 3 unupgraded and draw 4 upgraded while preserving vanilla play order.
- Added a `BrightestFlame.CanonicalKeywords` patch so Exhaust is visible on the card, with the existing `ExhaustOnNextPlay` play wrapper kept only as a behavior backstop.
- Updated active card localization to `BRIGHTEST_FLAME.title` / `BRIGHTEST_FLAME.description` with `{Cards:diff()}` instead of fixed "Draw 3" text.
- Fixed stale guard/docs drift for Pumpkin Candle rollback and Quality Flame dynamic text, including the Simplified Chinese manual-matrix strings.
- Final validation for this pass: `dotnet build EZMicroBalance.sln` passed with 0 warnings and 0 errors; `dotnet test EZMicroBalance.sln` passed, 66 passed, 16 skipped, 0 failed; `dotnet test EZMicroBalance.sln --no-build` passed, 66 passed, 16 skipped, 0 failed; `dotnet publish EZMicroBalance.sln` passed; package staging/versioned/zip were refreshed; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed, 82 passed, 0 skipped, 0 failed; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with only CRLF normalization warnings.
- Live gameplay verification remains pending.

## 2026-05-08 - RC1 Normal Steam Mod Settings Recheck

- Added a no-op BaseLib config page for EZ Micro Balance so the mod has a visible Mod Settings entry without exposing gameplay options.
- Rebuilt the package and refreshed hashes. Current package zip SHA256 is `BE05559B4EA1180FB88129235A980978B1E2498187F1CB665882EC7DCC1CD314`.
- Normal Steam-client isolated recheck `095137` showed BaseLib and EZ Micro Balance loaded, the localized EZMB page `寰钩琛 with `鏃犲彲閰嶇疆閫夐」銆俙, `Registered config for mod EZMicroBalance`, `Loaded 2 mods (2 total)`, 0 `ERROR` lines, and 0 release-blocking signatures.
- Live Ancient gameplay matrix, save/load checks, disable gameplay checks, multiplayer disposition, author decision, clean commit, and user-approved push remain pending.

## 2026-05-09 - Package README Decision-Blocker Refresh

- Updated package-facing `README_INSTALL.txt` in package staging and the versioned package folder to call out the unresolved manifest author placeholder and Rootblight-family card-art decision blockers.
- Rebuilt `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` from the versioned package folder. Package zip SHA256 for this README-only refresh was `F82330F5FBAAD5ADAD581861FC2981071DEA9ABD5BD0BFFDCB27694DC5460156`.
- DLL, manifest, and PCK hashes are unchanged from the Rootblight top-level notice hardening pass.
- Per the current implementation-only direction, tests, format, smoke, and live verification were not rerun for this README-only package refresh.

## 2026-05-09 - Rootblight Optional Portrait Fallback

- Added `RootPortraitPaths` so Rootblight I/II/III and Blight Sprout try the documented per-card portrait filenames and fall back to the current generic card portrait while those files are absent.
- Updated source guard expectations and docs to distinguish the implemented optional path resolver from the still-pending final art decision.
- Ran `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- Ran `dotnet publish EZMicroBalance.sln`: passed and refreshed the installed DLL/manifest/PCK artifacts.
- Rebuilt package staging, versioned package, and `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` from the installed artifacts while preserving the package README decision-blocker notes. Current hashes: DLL `9A0E750122D3AEBE449D2D95A20AED84657AFF6D169079E0F0184CC7084A70DF`, JSON `479C6AC4C5F9FD5B739C0A2E4442ADD7C0B12FC0514C7CF2153F12553F70FA84`, PCK `253E1310D8357EEB4D099F34BFA8785A66FEE77576BDA59A4D34277874696C25`, README `F2C2E7642ADACB4D6DF81E7D4D8CF12113FA7630132800B486031FC07704C1DE`, package zip `1699D7BEC6C1A0BD02223E45E4B90399C7BFBB20D4E95236F9ED1E08A795AF8F`.
- Per the current implementation-only direction, tests, format, smoke, and live verification were not rerun after this source/package refresh.

## 2026-05-09 - Manifest Author and Generated Rootblight Art Integration

- Replaced the active `EZMicroBalance.json` author placeholder with `wenhuorongbing-netizen`, taken from the local Git user name. The legacy `EzDailyContent` scaffold manifest remains unchanged.
- Added original generated portrait art for Rootblight I/II/III and Blight Sprout at the documented small and big portrait paths under `EZMicroBalance/images/card_portraits/`.
- Updated `export_presets.cfg` so all 8 new portrait PNG resources are included in the selected-resource PCK export. Publish imported and packed the matching `.ctex`/`.import` resources.
- Updated the package-facing `README_INSTALL.txt` to record the resolved author and generated-art status while keeping live generated-art visual verification pending.
- Ran `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- Ran `dotnet publish EZMicroBalance.sln`: passed and refreshed the installed DLL/manifest/PCK artifacts.
- Rebuilt package staging, the versioned package folder, and `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` from the installed artifacts. Current hashes: DLL `9A0E750122D3AEBE449D2D95A20AED84657AFF6D169079E0F0184CC7084A70DF`, JSON `479C6AC4C5F9FD5B739C0A2E4442ADD7C0B12FC0514C7CF2153F12553F70FA84`, PCK `253E1310D8357EEB4D099F34BFA8785A66FEE77576BDA59A4D34277874696C25`, README `F2C2E7642ADACB4D6DF81E7D4D8CF12113FA7630132800B486031FC07704C1DE`, package zip `1699D7BEC6C1A0BD02223E45E4B90399C7BFBB20D4E95236F9ED1E08A795AF8F`.
- Per the current implementation-only direction, tests, format, smoke, and live verification were not rerun after this source/resource/package refresh.

## 2026-05-09 - Resolved-Status Guard and Documentation Consistency

- Updated release guard expectations so the private-beta handoff now requires the resolved `wenhuorongbing-netizen` author and generated Rootblight-family art status instead of the earlier placeholder/art-decision blocker language.
- Updated release-facing docs to keep generated-art visual verification pending while no longer treating the source art decision itself as unresolved.
- Ran `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- Per the current implementation-only direction, `dotnet test`, opt-in release artifact tests, format, smoke, live verification, publish, and package refresh were not rerun for this docs/test-source consistency pass.
