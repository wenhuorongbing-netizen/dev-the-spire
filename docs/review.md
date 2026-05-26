# Current Source Review

Date: 2026-05-26
Scope: compact no-game source/resource review notes for taking `Spire Plus` to a user-test-ready build. Full historical review details are archived at `docs/archive/feature-audits/review-pre-slim-20260518.md`, `docs/archive/feature-audits/review-2026-05-23-pre-compact.md`, and `docs/archive/feature-audits/review-2026-05-24-sere-talon-pre-compact.md`.

## Current Conclusion

No current static P0/P1 source blocker is known from the latest no-game review passes. This does not prove release readiness.

Live-only blockers remain:

- Vakuu victory return/no-black-screen, failure/death path, active-fight save-load, and co-op.
- Urda Root Eyes hover/click/entry/save-load, Seed Bank click extraction, and clicked Ancient UI.
- Morvi and Lotha live gameplay, card-play freeze reports, save-load, and co-op.
- A11 route traversal, A12/A16/A19/A20 combat behavior, and Rootblight combat-end behavior.

## Latest Fixed Findings

- 2026-05-26 active issue-detail typo cleanup: `docs/issues/ancient-expansion-v2.2.md` and `docs/issues/urda.md` no longer carry OCR-style artifacts such as `SPIREPiUS`, `iive`, `iinkedRewardSet`, or `CardReward.Onokipped`; current env-var names, Core type names, and issue headings are readable again. `DocumentationCompactnessGuardTests.CurrentIssueDetailDocsAvoidReadableTypoArtifacts` guards those active issue docs. No game was opened.
- 2026-05-26 release handoff governance cleanup: current completion/audit docs no longer pin obsolete dirty-worktree snapshots or the stale beta.35 loader-pending label. `DOC-CONFLICT-GOVERNANCE` is source-fixed in `docs/issues.md`, while final release handoff still must recapture `git status --short --branch`, current HEAD, and push state. No game was opened.
- 2026-05-26 Future Peek UI-only co-op pass: the archived `future-peek-goal-20260526.md` was implemented for existing preview tools. Crystal Sphere peek no longer returns early in co-op; it logs `coop_local_ui_preview_enabled` and still only changes local `%ScryMask` alpha. Transform preview no longer returns early from co-op; it logs `prediction_prepared_multiplayer_ui_only`, uses the existing forked RNG snapshot, never adds choices or rewards, and display exceptions clear Spire Plus prediction state before returning to vanilla cycling. Map foresight and reward foresight remain unimplemented because they would change future room or reward outcomes and need a separate deterministic or host-authoritative precommit plan. No game was opened.
- 2026-05-26 Queen / Royal Decree runtime guard: user reported the Queen/Royal Decree fight could not run correctly. Source review found Queen's Core `ChainsOfBindingPower` can afflict cards with `Bound` more broadly than `RoyalDecreeEnchantment` can safely enchant, while `CardCmd.Enchant(...)` throws if the target card cannot receive that enchantment. Royal Decree now filters candidates with `ModelDb.Enchantment<RoyalDecreeEnchantment>().CanEnchant(card)` before marking, so un-enchantable Bound cards are skipped instead of crashing. The inline zhs modifier text was also repaired. No game was opened.
- 2026-05-26 Decimillipede Bulk damage polish follow-up: user reported the strengthened Decimillipede attack still showed 7 damage. Source review found the earlier elite damage polish covered Writhe and Constrict but missed `BulkDamage`, even though Core uses `BulkDamage` for both `SingleAttackIntent(BulkDamage)` and `DamageCmd.Attack(BulkDamage)` before applying Strength. Spire Plus now patches `DecimillipedeSegment.get_BulkDamage` as well, and `EnemyDamagePolishGuardTests.HighPressureEliteDamagePolishPatchesSourceDamageGetters` locks the visible-intent plus real-damage getter route. No game was opened.
- 2026-05-26 Fission Exhaust trigger clarification: user requested that Fission count as real Exhaust for effects such as Howl from Beyond and Drum of Battle. Source review confirmed Core already uses `CardKeyword.Exhaust` to choose `PileType.Exhaust`, and `OnPlayWrapper(...)` then calls `CardCmd.Exhaust(...)`, which records `CardExhausted` history and broadcasts `Hook.AfterCardExhausted(...)`. Fission text now states that Exhaust effects trigger normally, and `AscensionFeatureGuardTests.FissionUsesCanonicalExhaustPipelineAndTriggersExhaustListeners` locks the path through Drum of Battle, Howl from Beyond, Feel No Pain, Dark Embrace, and Charon's Ashes-style listeners. No game was opened.
- 2026-05-26 Soul Tide player-start Block fix: user retest still saw Artifact but no visible Block. Source review kept the `BeforeSideTurnEnd` Beckon count, but moved Block consumption out of enemy-turn end. Pending Soul Tide Block now waits through Soul Fysh's enemy turn and is granted from `BeforeSideTurnStart(... CombatSide.Player ...)`, so the next player turn starts with visible capped Block. The later player-turn-start path remains a fallback. No game was opened.
- 2026-05-26 release-evidence package-path helper cleanup: evidence collector scripts and `verify-spire-plus-release-evidence.ps1` now dot-source `scripts/spire-plus-package-evidence.ps1` and derive the versioned package artifact paths from `EZMicroBalance.json` instead of repeating the same `publish\SpirePlus-v...` DLL/PCK/manifest/README paths in every script. Generated evidence still records the current beta.38 paths, while future version bumps need fewer synchronized script edits. `ReleaseEvidenceGateTests.ReleaseEvidenceScriptsDeriveVersionedPackageArtifactPathsFromManifest` guards the helper use. No game was opened.
- 2026-05-26 governance stale-state cleanup: current docs no longer point current loader proof at beta.26, no longer say the removed root `EzDailyContent.json` still exists, and no longer copy stale beta37 commit labels into beta38 package evidence. `DocumentationCompactnessGuardTests.CurrentGovernanceDocsDoNotCarryStaleCleanupState` guards the current beta.38 loader-pending wording, current batch-classifier status, and the archived legacy-root boundary. No game was opened.
- 2026-05-26 website mechanism codex cleanup: `website/content-data.js` now generates the mechanism-codex cards from `mechanicGlossary` through `mechanicCodexItem` instead of hand-maintaining duplicate `manual(...)` entries for Blood Debt, Forge Token, Verdict, Fission, Seedbed, Firemarked Elite, Banner, and Deep Branch. `website/app.js` now shows related mechanics in the inspector through glossary terms, `relatedItemKeys`, and `relatedMechanicIds`, while skipping self-links. `WebsiteContentGuardTests.WebsiteHardcodedGameplaySummariesStayCurrent` guards the single-source generation and rejects the stale manual entries. No game was opened.
- 2026-05-26 generated C# UID cleanup: tracked `EZMicroBalanceCode/**/*.cs.uid` sidecars were removed from the active tree after source review found they are generated Godot import metadata, excluded from export by `*.cs.uid`, and not compile/package inputs. `.gitignore` now ignores regenerated source/test C# UID sidecars, and `EngineeringGovernanceGuardTests.RepositoryHygieneWorkflowAndTemplatesExist` guards that source/test UID files stay out of the active deliverable. No game was opened.

## Recent Historical Context

Detailed pre-current pass notes remain in the archive files listed above. This active review keeps only context that still guides current manual testing and prevents stale release claims.

- 2026-05-25 loader/startup context: historical beta.19 loader smoke reached the main menu with only BaseLib and Spire Plus enabled, registered `EZMicroBalance`, found 30 SavedSpireFields, and had a clean log audit. It is historical startup context only; beta.38 still needs fresh loader proof.
- 2026-05-25 co-op fail-closed pass: multiplayer gameplay mutations, combat hooks, Ancient reward/run hooks, Ascension reward/gameplay hooks, and Urda reward alternatives fail closed by default unless explicit opt-in environment variables are set. The two crash logs remain useful co-op evidence, but they do not prove current-package co-op behavior. Preview tools were later narrowed to local UI-only behavior and still need live two-client proof.
- 2026-05-25 player-facing polish: Seedbed / Planting, Seed Bank hover, A20 selector localization, Ancient direct-gain feedback, Fission Exhaust text, Soul Tide timing, Neow/Act 1 Ancient reroll, Elite Root, and high-pressure elite damage tuning are source/package-fixed and live-pending.
- 2026-05-24 Sere Talon / Tanx Claws lineage: source, package, art-route, handoff, website, and installed-package checks were hardened across multiple passes. Historical command logs are archive/context evidence, not a substitute for current live UI proof.

## Current Manual-Proof Focus

- Vakuu's Sere Talon must offer 4 Curses, choose 1, then add the selected Curse, 2 Wish, and 1 Wish+; its event option, relic bar, inspect screen, hover text, and log routes must not appear as Tanx Claws.
- Tanx Claws must stay on the Tanx route and transform selected cards into upgraded Maul+ / 撕咬+.
- Current-package Steam-client loader proof for beta.38 is pending; historical beta.19 and beta.17 loader rows are context only.
- Save/load, death/failure, co-op, clicked UI, hover, map traversal, preview tools, and gameplay evidence remain manual rows under `docs/issues.md`, `docs/toreview.md`, and the generated handoff.

## Still Not Claimed

- No live save/load, death/failure, co-op, clicked UI, hover, map traversal, or gameplay proof was produced; current state remains a manual-test candidate, not release-ready.
