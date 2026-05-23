# Current Source Review

Date: 2026-05-23

Scope: current no-game source/resource review notes for taking `Spire Plus` to a user-test-ready build. Full historical review details are archived at `docs/archive/feature-audits/review-pre-slim-20260518.md`.

## Current Conclusion

No current static P0/P1 source blocker is known from the latest no-game review passes. This does not prove release readiness.

Live-only blockers remain:

- Vakuu victory return/no-black-screen, failure/death path, active-fight save-load, and co-op.
- Urda Root Eyes hover/click/entry/save-load, Seed Bank click extraction, and clicked Ancient UI.
- Morvi and Lotha live gameplay, card-play freeze reports, save-load, and co-op.
- A11 route traversal, A12/A16/A19/A20 combat behavior, Rootblight combat-end behavior, and fresh current-package loader proof.

## Latest Fixed Findings

- 2026-05-23 A19/A20 Boss Seal v4.1 pass: replaced the older repeated Block/Strength/Artifact-style seal designs with boss-specific dedicated abilities and explicit multiplayer rules. Current source now uses Martyr Oath, percentage Slippery Ink Return, Plating Wake with Core multiplayer scaling, Soul Tide Block caps, Waterfall explosion Vulnerable, Kaiser Claw Calibration, Marginal Note/Deep Thought side costs, Escape Fatigue/Vigor, Aeonglass Time Sand Reflow into extra Wither plus intent-visible Eye Laser echo, Queen Royal Decree/Majesty, and Test Subject Experimental Record samples. EN/ZHS hovers and no-game guards were updated; live boss-by-boss proof remains pending.
- 2026-05-23 Firemark overflow pass: A12 Firemarked Elites now keep one Firemark Host with the full mark while overflow affects at most one secondary non-summon enemy. Might overflow grants temporary Strength to one attacker; Giant core break splashes one target; Forge Armor protects one target and now starts on player turns; Constant Heal uses the v3.3.1 interrupt line 12/24/48 and splashes one damaged ally only when the host successfully heals. Live multi-enemy elite proof remains pending.
- 2026-05-23 Ink Return / Plating / Martyr source-alignment pass: Core `SlipperyPower`, `PlatingPower`, and `ArtifactPower` scale in multiplayer. v4.1 restores Slippery by percentage after final displayed removal, lets Plating use Core multiplayer scaling, and keeps Artifact fixed. Core `TheKinBoss` still proves exactly two followers, so Martyr Oath remains capped at two real deaths.
- 2026-05-23 Pael's Horn text pass: EN/ZHS relic descriptions and current manual verification docs now say Pael's Horn adds 1 `Relax` / `放松` and 1 `Relax+` / `放松+` without the redundant "upgraded Relax+" wording. A guard now locks the Simplified Chinese wording. Live hover proof remains pending.
- 2026-05-23 Rootblight starter repair: A14 starter Rootblight no longer treats `CurrentMapCoord`, `ActFloor`, or map history as proof that Root Begins already applied. The run hook now retries before room entry, so first-room combat setup can copy Rootblight into combat piles, and the saved applied marker is written only after a real Rootblight card exists or the add succeeds. This should also repair affected live runs on the next room/act hook; live confirmation remains pending.
- 2026-05-22 Royal Seal text and Ancient reroll pass: Boss Seal / King Brand map and combat hover text now states concrete numbers for each seal. Urda, Morvi, and Lotha initial Ancient reward screens now include a one-use dice-style reroll option that refreshes the visible choices, records the spend in saved player state, and keeps run-history choice data aligned after reroll. Live clicked-UI proof remains pending.
- 2026-05-22 Temporary/Seedbed pass: added custom `Temporary` and `Plant` hover tips, updated EN/ZHS text, and rebuilt Urda Seedbed as a combat slot state. Seedbed now plants Blight Sprout and generated temporary Status/Curse cards that enter hand, skips Rootblight, permanent Curses, Withered Husk, and beneficial temporary pages, adds Withered Husk per planted card, persists slot count for save hydration, clears it at combat boundaries, and suppresses `AfterCardDrawn` for planted cards. Live tooltip and combat timing proof remain pending.
- 2026-05-22 v3.3 design pass closeout: implemented the current source-solvable v3.3 issues for Vakuu, Lotha, Urda, and localization. Final subagent review found no P0/P1. Follow-up findings were fixed: Vakuu damage-lock tracking now uses `AfterDamageGiven` so lethal player hits still count, Seedbed text now says "up to" to match the optional selector, Cash Out has a hand-full immediate-choice fallback, Mirror Rebuttal no longer says "draw" when it moves the card, and the visible Vakuu text now says Contract choices appear on turns 1/3/5. Live proof is still required.
- 2026-05-22 Aeonglass Hourglass text alignment: superseded by the 2026-05-23 v4.1 Time Sand Reflow pass. Aeonglass now converts uncleared Time Sand into extra Wither on the next Increasing Intensity.
- 2026-05-22 Root Eyes marker composition pass: replaced competing NNormalMapPoint hover patches with one shared map hover composer. Root Eyes, Firemarked Elite, Banner, and Deep Branch each contribute their own hover entry; marked or selectable Root Eyes nodes can coexist with existing quest markers, and a small Root Eyes badge appears when the main quest icon is already occupied. Multiplayer preview queue mutation remains gated and needs live two-client proof.
- 2026-05-22 localization QA pass: added missing Simplified Chinese `card_keywords.json`, exported it in `export_presets.cfg`, corrected active style-guide mojibake terms, tightened Misprint Press wording in EN/ZHS, aligned Ascension rich-text highlights, and added a bilingual localization parity guard for file/key coverage, dynamic variables, empty values, and balanced `[blue]`/`[gold]` tags.
- 2026-05-22 strict preview/relic/seal pass: subagents reviewed Root Eyes/Crystal Sphere/transform preview multiplayer risk, new reward-card/relic hook interactions, and Firemark/Banner/Boss Seal visibility against local Core source. Fixed Root Eyes marked-node conflicts, hover-side mutations, and multiplayer queue mutation; fixed Prismatic Gem listener tracking and documented its Core hook order; added combat-start Boss Seal / King Brand marker powers for previously hidden seal effects; routed Shieldwall and Last Stand to dedicated banner icons. Existing Boss Seal indicator art was sufficient, so no new generated buff image was required.
- 2026-05-22 issues sprint: fixed source-solvable player findings for Husk exhaust timing, Root Eyes encounter/event queue preview, current-act Firemark/Banner hovers, A20 boss sprouts, Constant-Heal dynamic threshold text, Waterfall explosion handling, the older Aeonglass Hourglass design, Queen Royal Decree, and Forge Armor tuning/tooltip behavior. The Aeonglass entry is now superseded by the 2026-05-23 Time Sand Reflow pass. The Constant-Heal hover now follows the later v3.3.1 12/24/48 threshold.
- 2026-05-22 package closeout: `package-spire-plus.ps1` now syncs `README_INSTALL.txt` to the installed game-root mod folder, and `check-installed-ezmb-package.ps1` verifies the installed README hash alongside DLL, manifest, and PCK.
- 2026-05-20 source/API audit pass: Firemark powers now use Core-visible counter display semantics, Forge Armor tracks generated Molten Armor separately from unrelated Block, A20 co-op gating logs before the single-player shape return, and Vakuu fight custom state writes culture-invariant save values.
- 2026-05-20 issue.md pass: expanded opt-in `ReleaseEvidenceLog` markers for Preview tools, Seed Bank extraction/cancel/failure, Root Eyes selection cleanup, Rootblight combat start/end, A20 map markers, and co-op gates; added a source guard for those marker surfaces. This is evidence collection support, not live proof.
- 2026-05-20 governance pass: added CI-safe repository hygiene workflow, issue/PR templates, ADR template, committed `.editorconfig`, generated `docs/patch-inventory.md`, added `docs/release-evidence-status.md`, and guarded these with `EngineeringGovernanceGuardTests`.
- 2026-05-20 governance pass: added self-hosted `.github/workflows/full-local-validation.yml` and `scripts/ci-full-validation.ps1` for full no-game validation with explicit `STS2_PATH` and `GODOT_PATH`; the script passed locally, and first GitHub self-hosted workflow run evidence remains pending.
- 2026-05-20 release-planning pass: converted `docs/goal.md` into no-game baseline, release scope, website claim audit, traceability matrix, source-research, architecture-boundary, save-state, and commit-boundary docs. The pass keeps live/manual rows open.
- 2026-05-20 governance pass: updated `scripts/verify-spire-plus-release-evidence.ps1` and `ReleaseSafetyExpandedGuardTests` so the verifier default package hash matches the current `EFDF43EBAD1A8A6AD9263E971B5CC0366E823739BA2660A69BDD747DF3425686` package.
- 2026-05-20 subagent review pass: current smoke-log parity now computes the expected `SavedSpireField` count from source and rejects historical 22-field logs as current package evidence.
- 2026-05-20 subagent review pass: tightened current smoke-log parity to count only static `SavedSpireField` declarations, so helper method generic references do not inflate the expected runtime loader count.
- 2026-05-20 subagent review pass: active-source coverage no longer lets `ActiveSourceManifestGuardTests.cs` satisfy itself; every active source file must map to an independent guard root.
- 2026-05-20 subagent review pass: patch-inventory freshness checks now ignore the generated date, fail if the inventory is missing, and CI whitespace checks inspect committed/PR changes rather than an empty working tree.
- 2026-05-20 subagent review pass: Forge Armor shatter now uses the host's pre-Molten-Armor Block baseline instead of subtracting shared `BlockedDamage`.
- 2026-05-20 subagent review pass: fixed low-risk lifetime/scope issues in the transform-preview RNG context, Urda Root Eyes transient selection state, Root Eyes failure logging, and Vakuu pre-finished parent restore heal skips.
- 2026-05-20 subagent review pass: Root Eyes now refunds previews that become unreachable after the player chooses another map branch, including marker restore and hover cleanup paths.
- 2026-05-20 goal guard pass: added completion-claim and save-state contract guard tests plus `docs/reviews/red-team-goal-implementation-pass-1.md`; this is not a release-ready claim and live loader, clicked UI, save-load, Vakuu, co-op, and preview proof remain pending.
- Seedbed now catches eligible cards that enter the hand through Urda's hand-change hook, not only through the RootBud combat hook.
- Lotha Death Reprieve save hydration now restores pending-start state from the saved phase instead of inferring it from the current power list.
- Urda Molting act-entry cleanup clears its active flag after removing generated husks.
- Firemark Giant's Molten Core window no longer counts the threshold-crossing hit as window damage.
- Banner and Forge Token target selection no longer consumes live run RNG for source-testable deterministic cases.
- Multi-enemy-only banner map previews now use generic banner text/icon until combat knows the enemy count.
- Preview tools are integrated under Spire Plus; Crystal Sphere preview restores/hides its UI after the minigame finishes, and transform preview remains preview-only.
- Morvi, Lotha, and Vakuu combat powers now use dedicated 64px/256px power art paths instead of option, card, or fallback art.
- `export_presets.cfg` was restored to UTF-8 without BOM after Godot rejected the export preset during publish.

## Package Under Test

`publish/SpirePlus-v0.1.0-private-beta.0.zip`

| Artifact | SHA256 |
| --- | --- |
| ZIP | `EFDF43EBAD1A8A6AD9263E971B5CC0366E823739BA2660A69BDD747DF3425686` |
| DLL | `BC79A2634F87314046C8C86120E2FD94030FDC650F5B432864E789AA6B888A4A` |
| PCK | `6F5080524B57EC07F750D6DCFB6D6B274C5F5780EA60DBABB0E6B254D26D01C5` |
| Manifest | `C2FB53C13AE099080AC71FF7EE2A1F217A2586549A9152DAFE0EBF512EF42FF6` |
| README_INSTALL | `C735AA228BBB5CD002BF618334A04483C0013328C82ECC33551C65B0A1165599` |

## Latest Validation

No game was opened.

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
dotnet publish EZMicroBalance.sln
.\scripts\package-spire-plus.ps1
$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'; dotnet test EZMicroBalance.sln --no-build
.\scripts\check-installed-ezmb-package.ps1
```

Results:

- Spire Plus build: 0 warnings/errors.
- Spire Plus normal tests: 235 passed / 18 skipped.
- Format check: passed.
- `git diff --check`: passed with existing CRLF/LF warnings only.
- Spire Plus publish/package: passed.
- Artifact tests: 253 passed / 0 skipped.
- Installed/staging/versioned/zip artifact parity: passed.
- Installed game-root artifact hash check: passed.
- New `scripts/ci-full-validation.ps1` lane: passed locally with explicit `STS2_PATH` and `GODOT_PATH`.

## Manual Retest Queue

Use `docs/toreview.md` as the current tester queue. Do not close those rows from source review alone. Close only after the matching live manual proof exists.

## Review Rules

- Keep source moves behavior-preserving unless the slice is explicitly a bug fix.
- Keep active docs compact and archive historical logs under `docs/archive/**`.
- Do not claim live gameplay, save-load, death/failure, co-op, or release readiness without direct game evidence.
