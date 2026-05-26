# Spire Plus Test-Ready Completion Audit

Date: 2026-05-14

Current note: this audit started from the earlier packaged candidate where Lotha and Vakuu were explicitly outside the implementation target. It now records Urda eleven-blessing source implementation, Morvi source implementation, Lotha source implementation, and the hidden-by-default dedicated Vakuu fight slice as present in source.

Beta.19 loader parity is covered by `.tools/runtime-evidence/beta19-loader-smoke-20260525-213336`. Current source defines 30 SavedSpireFields. That loader smoke is clean, reports `v0.1.0-private-beta.19` and `Found 30 SavedSpireFields`, and remains historical startup evidence only. The beta.53 package still needs fresh loader proof. Installed-PCK resource smoke under `.tools/runtime-evidence/current-package-smoke-20260514-015901` remains resource-only evidence. Live gameplay, save-load, death/failure-path, co-op, and clicked Ancient UI remain pending.

This audit maps `docs/test-ready-development-goal.md` to concrete artifacts and evidence. It treats "test-ready" as that document defines it: source-complete, packaged, and ready for the user to manually test. It does not mean private-beta release-ready. Live gameplay, save/load, and co-op claims remain pending unless explicitly listed as runtime evidence.

Update note, 2026-05-13 player-facing polish pass: after this packaged-candidate audit, Ancient dialogue/option/card/relic/power text was polished, bilingual rich-text guards were added, Vakuu fight source was hardened with an awaited room transition and explicit fallback option, `dotnet build EZMicroBalance.sln --no-restore` passed, `dotnet publish EZMicroBalance.sln --no-restore` passed, package staging/versioned folder/zip were rebuilt with the player-facing `SpirePlus` archive name, and normal plus opt-in artifact tests passed. Live verification was not rerun for this package.

Update note, 2026-05-14 Vakuu Temptation pass: Vakuu now has source-backed hidden Contract cards, and the Vakuu fight offers three Contract choices after the normal hand draw on player turns 1, 3, and 5. Build, normal tests, format, diff check, publish, package refresh, and opt-in artifact tests passed. Live gameplay, save-load, and co-op verification were not run.

Update note, 2026-05-14 source red-team pass: Morvi/Open-Book saved markers, generated-card cleanup, Forbidden Loan add-result checks, Red Ink fallback settlement, nonlethal Debt Settlement HP fallback, Urda gate aliases, Seed Bank marker cleanup, and player-facing text guards were hardened. Build, normal tests, format, diff check, publish, and post-publish normal tests passed; package scripting and opt-in artifact hash checks passed in the follow-up hash refresh. Live gameplay, save-load, death/failure path, and co-op verification were not run.

Update note, 2026-05-14 package hash refresh: after the source red-team hardening pass, package staging, the versioned package folder, and `publish/SpirePlus-v0.1.0-private-beta.53.zip` were refreshed from installed artifacts. These hashes record automated source validation only; live gameplay, save-load, death/failure path, and co-op verification remain pending.

Update note, 2026-05-14 historical package smoke/log/resource pass:

- `.tools/runtime-evidence/current-package-smoke-20260514-015901` verified hash parity for the earlier 22-field package, headless installed-PCK loading for Urda/Morvi/Lotha scenes plus 43 Ancient textures, and a normal Steam helper startup with BaseLib plus Spire Plus under technical id `EZMicroBalance`.
- The historical log records BaseLib `177 patches successfully, 0 failed`, `Loaded 2 mods (2 total)`, `Found 22 SavedSpireFields`, and `Time to main menu: 14,045ms`; audit/manual scans found 0 Spire Plus error signatures for technical id `EZMicroBalance` and 0 release-blocking hits.
- Loader parity for the current ZIP is pending after the README wording refresh. The 30-field smoke covers the same DLL/PCK/manifest and remains startup context; no loader log is live gameplay/save-load/co-op/UI screenshot proof.

Update note, 2026-05-14 next test-ready player-facing polish pass: option card hovers, rich-text/localization truth, art-manifest coverage, and package hashes were refreshed. `dotnet build`, normal tests, `dotnet format`, `dotnet publish`, post-publish normal tests, `scripts/package-spire-plus.ps1`, opt-in release artifact tests, final normal tests, the Ancient art audit, and `git diff --check` all passed. Live gameplay, save-load, death/failure path, co-op, and clicked Ancient UI/manual feature verification were not run.

## Objective Restated

Concrete deliverables:

- Keep `EZMicroBalance` as the stable manifest/package id while using `Spire Plus` as the player-facing name.
- Provide one plug-in / plug-off package as the only active mod surface.
- Keep out-of-scope work out: no A21-A30, no custom character, no manifest-id migration, no official game assets, and no copied large decompiled code bodies.
- Make the current source surfaces coherent for one manual test pass: Ancient reward rebalance v4.3, Urda default-on eleven-blessing source slice, Morvi default-on eight-blessing source slice, Ascension A11-A20 slices, Rootblight/Blight Sprout behavior and art, English/zhs localization, and truthful release docs.
- Keep Vakuu limited to the current single-player source slice unless runtime evidence supports broader behavior. Morvi, Lotha, and Vakuu now have source/art evidence for default-on test slices, but runtime gameplay evidence remains pending.
- Pass automated build/test/format/package checks, refresh artifacts, and verify the current installed package can load to main menu with BaseLib.
- Record all live/manual gaps as pending instead of claiming release readiness.

## Prompt-To-Artifact Checklist

| Requirement | Evidence inspected | Status |
| --- | --- | --- |
| Required reading order exists and was used | `PROJECT_STATE.md`, `AGENTS.md`, `docs/test-ready-development-goal.md`, `docs/README.md`, `docs/PROJECT_MAP.md`, `docs/issues.md`, issue files, v2.2 audit/design/plan/roadmap/safety/risk/manual docs, Urda README, Ancient v4 README, Ascension README, localization style guide, and repo skill doc were read before edits in this pass. | Pass |
| Stable technical id and display-name split | `EZMicroBalance.json` keeps `"id": "EZMicroBalance"` and uses `"name": "Spire Plus"`; settings localization has `EZMICROBALANCE.mod_title` as `Spire Plus` in eng/zhs; tests guard id/name distinction. | Pass |
| Single package structure | `EZMicroBalance/`, `EZMicroBalanceCode/`, `EZMicroBalance.csproj`, `EZMicroBalance.json`, and `publish/SpirePlus-v0.1.0-private-beta.53.zip` with `EZMicroBalance/` as the install folder; duplicate root mod surfaces are removed from the active tree. | Pass |
| Phase 0 naming hygiene | `README.md`, `PROJECT_STATE.md`, `docs/mod-changelog.md`, release checklist, handoff, manifest, settings UI JSON, and package README all describe `Spire Plus` as display name and `EZMicroBalance` as technical id. | Pass |
| Phase 1 saved-state foundation | `AncientSavedStateFields.cs` defines 14 Ancient fields after the Morvi Open-Book sealed-card marker, `AscensionSavedStateFields.cs` defines 12 Ascension fields after Rootblight pending-downgrade persistence, and the beta.19 loader smoke reports `Found 30 SavedSpireFields`. `AncientPlayerState.cs` mirrors Urda/Morvi/Lotha player state onto deck card markers for reload-recovery testing. | Pass for source guards and loader; live save-load remains pending |
| Phase 1 reward reentry hardening | Urda Humus and Morvi Debt payoff state clears only after resolver success; Seedbed counts only accepted alternatives; Prismatic Gem reroll/screen state is guarded; source tests cover reward reentry constraints. | Pass for source guards; reward-screen save/load remains pending |
| Phase 2 Urda default-on slice | Active ids are `urda_seedbed`, `urda_humus_pact`, `urda_molting`, `urda_moss_map`, `urda_trial_branch`, `urda_shallow_root_relic`, `urda_rooted_route`, `urda_after_rain`, `urda_root_sight`, and `urda_seed_bank`; `EZMB_DISABLE_URDA=1` disables Urda; forced blessing env var remains for diagnostics; EN/zhs localization and manual rows exist; Urda uses BaseLib custom Ancient icon/background-scene paths and option relics. Source-safe deviations are documented for Trial Branch, Shallow-Root Relic, Rooted Route, Root-Sight, and Seed Bank. Prior 30-field loader proof exists for an older ZIP/DLL hash. | Pass for test-ready source; current loader/live Urda/save-load/co-op pending |
| Phase 3 Morvi default-on slice | Morvi is default-on with all eight v2.2 blessing ids, `EZMB_DISABLE_MORVI` / `SPIREPLUS_DISABLE_MORVI`, force-Ancient and force-blessing gates, custom Control-based Ancient scene, separate event/map/run-history art, option marker relics, Archive Page/Overdraft/Waste Paper cards, Debt/Proofread/Open Book/Overdraft/Paperstorm powers, English/zhs localization, and source guards. Prior 30-field loader proof exists for an older ZIP/DLL hash; live gameplay, save-load, and co-op remain pending. | Source-complete / live-pending |
| Phase 4 Lotha first slice | Lotha is default-on with all eight v2.2 blessing ids, custom Control-based Ancient scene, separate event/map/run-history art, option marker relics, chosen-card Mirror Rebuttal state, player-owned `LothaVerdictPower`, `LothaEnlightenmentPower`, `EZMB_DISABLE_LOTHA` / `SPIREPLUS_DISABLE_LOTHA`, force-Ancient and force-blessing gates, English/zhs localization, and source guards. Prior 30-field loader proof exists for an older ZIP/DLL hash; current hash, live gameplay, save-load, death-reprieve lethal-path, and co-op remain pending. | Source-complete / live-pending |
| Phase 5 Vakuu fight | A single-player source slice adds Fight Vakuu, enters a custom `RoomType.Monster` combat with normal rewards disabled, resumes the parent Vakuu event on victory, and offers three non-Vakuu Act 3 Ancient blessings. Disable/force gates, option art, localization, and static guards exist. Live UI/gameplay, save/load, failure/death, and co-op evidence remain pending. | Source-complete / live-pending |
| Phase 6 Ascension A11-A20 | `EZMicroBalanceCode/Ascension/**` implements default-on private-beta slices with public/multiplayer disable env vars, A20 co-op downgrade warnings, map/reward/combat source guards, and manual checklist/runbook rows. | Pass for test-ready source; full live/co-op pending |
| Phase 7 art and UI text | Rootblight-family generated portraits are integrated and packaged; active zhs JSON has no known mojibake fragments; EN/zhs key parity and text guards pass. Urda, Morvi, and Lotha event art now uses 1831x859 source-local middle-draft resources with full-scene cover fitting. Active Ancient option/icon/power/fight/card art, Ascension indicators, and neutral fallback power/relic assets now use browser ChatGPT/GPTimage2 rebuilt `final_generated` files with transparent icon backgrounds where applicable. Live clicked-UI preview remains pending. | Pass |
| Phase 8 required commands | Latest validation passed for source/package automation after the player-visible localization naming guard; beta.19 loader smoke is captured as historical startup evidence. Latest package refresh evidence still includes artifact tests and package/PCK text checks for Sere Talon / Tanx Claws plus Trial Branch. Gameplay, beta.53 loader, save-load, clicked UI, route-click, death/failure, and co-op remain pending. | Pass for source/package validation; live manual gameplay pending |
| Package artifacts | `publish/SpirePlus-v0.1.0-private-beta.53.zip` SHA256 `0EFDA36BB18A28C31474CA299341800F4F49B4B3F142D03E43EAAF1A52E07980`; DLL `21D58AAB002B0071E69D39EF9CDA6492F1AE81883CD94C9C8AA4DE7F1227E484`; manifest `2BD727A314A947C9B408D200933D23B59CE9640C5AF83FFB940073F5F48FDCFD`; PCK `DF865082D339F721270C5F2EA1F13EC0A34280459C29DFEAE070E4E1AA4AA58E`; README `78CEF7EFF923C502D6ACC35296008A27F1C4A996478400942B118E5A086A7D77`. The README is now a short manual-test install note and says Ancient selections grant visible marker relics. | Pass for installed/staging/versioned/zip artifact parity |
| Runtime smoke and Mod Settings UI | The beta.19 smoke at `.tools/runtime-evidence/beta19-loader-smoke-20260525-213336` loaded BaseLib plus Spire Plus under technical id `EZMicroBalance`, reported `v0.1.0-private-beta.19` and `Found 30 SavedSpireFields`, reached startup completion, and audited clean. The refreshed Mod Settings UI list screenshot at `.tools/runtime-evidence/current-spire-plus-modsettings-20260513-111342/02-mod-config-list.png` shows `Spire Plus`. | Pass for historical beta.19 startup and visible mod list; beta.53 loader plus gameplay/UI-click verification pending |
| Documentation truthfulness | `PROJECT_STATE.md`, `docs/test-plan.md`, `docs/release-checklist.md`, `docs/private-beta-verification-handoff.md`, feature docs, issues, work logs, and this audit keep live/manual gates pending. | Pass |

## Missing Or Weakly Verified Items

These do not block the source-complete test-ready package because the goal doc explicitly separates test-ready from release-ready. They still block private beta release readiness and any live gameplay claim:

- Historical beta.19 normal Steam-client startup/log verification reports `Found 30 SavedSpireFields`, and the refreshed Mod Settings UI list capture now shows `Spire Plus`; beta.53 loader, gameplay, clicked UI, save-load, and co-op rows remain pending.
- Full Ancient reward runtime matrix is pending.
- Ancient/Urda/Morvi reward-screen save/load rows are pending.
- Disable-mod gameplay behavior in an actual run is pending.
- Natural A11 click-by-click traversal remains pending; saved-map boss-reachability proof exists for the current A11 Act 1 spot-check save.
- Full Rootblight combat-end behavior, generated-art in-game visual check, and co-op ownership/desync checks are pending.
- The 2026-05-13 A14 Rootblight art-hover probe found pre-fix Urda missing asset paths before combat. The source/package fix is in place and `.tools/runtime-evidence/urda-pck-resource-load-20260513-123345` verifies installed-PCK resource resolution for the custom Urda scene/icon, but post-fix live Urda and Rootblight visual/gameplay checks remain pending.
- Urda, Morvi, Lotha, and Vakuu source/art/localization are implemented, and historical beta.19 loader plus resource smoke exists, but beta.53 loader, live gameplay, clicked Ancient UI, save-load, death/failure-path, and co-op checks are pending.
- Two-client multiplayer matrix is pending.
- Worktree is not clean and no commit/push has been performed; push is expected after validation and an intentional commit.

## Conclusion

Under the current documented scope, the audited tree is source-complete for a manual test-ready candidate with `SpirePlus` archive naming and stable `EZMicroBalance` install identity, and automated package/artifact validation is clean. It is not private-beta release-ready until the pending live/manual gates above and live Urda/Morvi/Lotha/Vakuu gameplay validation are executed or explicitly deferred.
