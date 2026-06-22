# Ancient Expansion v2.2 Work Log

Current status, 2026-05-17: this active work log is a compact current-facing summary. The older raw chronological log was archived to `docs/archive/feature-work-logs/ancient-expansion-v2.2/work-log-20260517-pre-cleanup.md` because its inline-code delimiters were corrupted and made the active documentation hard to read.

## Current Implementation

- Urda is default-on for private-beta testing with eleven source-backed blessings, visible option relics, custom Ancient scene/icon routing, disable/force gates, and source-safe deviations documented in the source design and issue notes.
- Morvi is default-on with all eight v2.2 blessing ids. Misprint Press, Forbidden Loan, Debt Settlement, Overdue Library, Open Book, Paperstorm, Red Ink, and Blueprint Proof have source guards and player-facing localization.
- Lotha is default-on with all eight v2.2 blessings. Power-card fallback behavior is source-backed: Mirror Hall Echo, Deferred Verdict, and Single Sentence make eligible Powers cost 0 for that play and draw 1 with no Energy gain.
- Vakuu's dedicated fight is hidden by default and explicit-gated for local testing. The active combat room no longer stores `ParentEventId`; parent recording happens only after victory for prefinished serialization. Live victory return, no-black-screen proof, failure/death path, and save/load remain pending.
- Ancient reward choices now grant visible marker relics so players can inspect the effect from the relic bar where the game UI supports relic hover.

## Current Art And UI State

- Restored the user-accepted 16:9 Urda root-mother background is the active Urda event art.
- Recovered the user-uploaded Morvi blue-eye court background is the active Morvi event art.
- Recovered the correct user-uploaded horizontal mirror-ensemble image is the active Lotha event art.
- Event backgrounds, map icons, run-history icons, option relic icons, power icons, card portraits, and Vakuu fight art use separate active resource paths. Map and run-history icon pairs intentionally share small-icon bytes; event backgrounds are not used as option relic art.
- The latest art audit reports 95 manifest assets, 0 missing targets, 0 missing exports, 0 hash mismatches, 0 invalid generation modes, and 0 missing final assets.

## Current No-Game Review Notes

- Root Eyes source path: clicking the Root Eyes relic starts map selection, only reachable Act 1 Monster, Unknown, or Elite nodes are valid, Unknown outcomes exclude Shop, Treasure, Rest Site, and Boss, and the chosen concrete encounter/event preview is saved for that node.
- Seed Bank source path: stored cards appear on relic hover, the relic shows a counter, and clicking it lets the player take up to two stored cards. The first chosen card is upgraded, then the relic is used up.
- Seedbed source path: Seedbed now sets combat slots that plant later Blight Sprouts and generated Temporary Status/Curse cards entering hand. It excludes Rootblight, permanent Curses, Withered Husk, and beneficial temporary pages; planted cards use `CardPileCmd.RemoveFromCombat(...)`, add Withered Husk, and suppress `AfterCardDrawn` hooks.
- Morvi source path: Overdue Library generated pages use valid dynamic variables and icons; Blueprint Proof initializes at combat start and has a late guard; Debt Settlement records 320 Debt, pays 40 Gold after combat, and converts each missing 10 Gold into 3 nonlethal HP loss.
- Ascension v3.2 source path: Withered Husk is 3 Block, Banner timing/scaling and single-enemy fallback are source-guarded, and Firemarked Elites expose counterplay windows through powers and map hover text.

## Validation Baseline

Most recent completed no-game validation recorded in `docs/review.md`:

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
dotnet publish EZMicroBalance.sln
.\scripts\package-spire-plus.ps1
$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'; dotnet test EZMicroBalance.sln --no-build
```

Beta.105 clicked Ancient UI smoke is recorded elsewhere; this work log does not claim live gameplay, hover/readability follow-through, save-load, failure/death path, or co-op proof.

