# Archived prompt (2026-05)

- Original path: 'docs/codex-repo-overhaul-refactor-prompt.md'
- Archived path: 'docs/archive/prompts/2026-05/codex-repo-overhaul-refactor-prompt.md'
- Reason: Non-current repository-overhaul planning prompt.
- Archived date: 2026-05-10
- Historical archive.

---

# Codex Overnight Refactor Prompt �?Repository Diet, Architecture Cleanup, and Test-Ready Project State

You are a senior C#/.NET, Godot, Slay the Spire 2 mod engineer and release engineer.

Repository:

D:\Game\FOTN\dev-the-spire

Current observed state:

- Latest main contains `implement urda`.
- `EZMicroBalance` is the active mod id.
- Legacy `EzDailyContent` must remain unchanged in-place.
- previous framework target is v3.1.2.
- The repo now contains many current docs, issue addenda, prompt dumps, archive docs, work logs, and feature docs. Reading cost is too high.
- `UrdaAncient.cs` currently registers a new Act 1 Ancient and records selected blessing id, but the blessing behavior is not implemented as a full playable blessing system yet.
- The project needs a major cleanup that reduces context cost, improves maintainability, and preserves release/test correctness.

Goal:

Perform a **non-destructive repository overhaul** that makes the project easier for future Codex runs to understand and safer to maintain.

This is primarily a structure/docs/state-management refactor. Do **not** do new gameplay features in this pass except small code organization fixes needed to keep the current build/test green.

Do not delete useful evidence. Archive or consolidate it.

Do not claim release ready.

Hard rules:

- Do not change existing manifest id `EZMicroBalance`.
- Do not change legacy `EzDailyContent` manifest id.
- Do not implement Morvi, Lotha, or Vakuu.
- Do not implement new Urda blessing behavior in this cleanup pass unless required to stop a broken live option from appearing.
- Do not implement A21-A30.
- Do not implement custom characters.
- Do not copy official game assets.
- Do not copy large decompiled source bodies.
- Do not commit `source code/`, `.tools/`, `.godot/`, `publish/`, `bin/`, `obj/`, local game binaries, or runtime evidence binaries.
- Prefer moving/archiving over deletion.
- Keep release claims honest.
- Fix root causes, not downstream normalizers.
- Keep diffs reviewable.

Must read first:

1. AGENTS.md
2. docs/README.md
3. docs/PROJECT_MAP.md
4. EZMicroBalanceCode/README.md
5. docs/issues.md
6. docs/rc1-live-validation-log.md
7. docs/private-beta-verification-handoff.md
8. docs/release-checklist.md
9. docs/style/card-localization-style-guide.md
10. docs/skills/sts2-godot-mod-development.md
11. EZMicroBalanceCode/Ancients/UrdaAncient.cs
12. EZMicroBalanceCode/Ancients/Common/AncientSavedStateFields.cs
13. EZMicroBalance/localization/eng/ancients.json
14. EZMicroBalance/localization/zhs/ancients.json
15. tests/EZMicroBalance.Tests/**
16. export_presets.cfg
17. source code/src/Core/** as local primary source evidence, but do not commit it.

Source-code package rule:

The user supplied a code-only Slay the Spire 2 source package. Use it only as local source evidence.

- Do not copy source bodies into docs.
- Record class/method/API names and short conclusions only.
- If local `source code/src/Core/**` differs from the uploaded zip or current game version, document the mismatch and prefer the installed local source when running/building.

Definition of done:

- One compact current-state entrypoint exists.
- Docs are classified into current, feature, release, style/skill, archive.
- Prompt dumps and addenda are removed from current reading path.
- `docs/issues.md` no longer forces agents to read thousands of lines before understanding active blockers.
- Urda is classified correctly: active playable if behavior works, otherwise prototype/disabled; no misleading release claim.
- Project map and code module map reflect current layout.
- Tests/guards still pass.
- Package is not rebuilt unless code/resource/localization/package content changed.
- Final report explains exactly what was moved, merged, archived, or left pending.

Phase 0 �?Preflight and current-state snapshot

Run:

- git status --short --branch
- git log -1 --oneline --decorate
- Get-Process SlayTheSpire2 -ErrorAction SilentlyContinue

Then create or update:

- `PROJECT_STATE.md` at repo root.

`PROJECT_STATE.md` must be short and useful for future sessions. It should contain:

1. Active target: EZMicroBalance.
2. Current latest commit.
3. Game/previous framework target.
4. Current top-level status:
   - build/test/package known status
   - runtime status
   - multiplayer status
   - Urda status
   - Rootblight status
5. Current active feature areas:
   - Ancient reward rebalance v4
   - Ascension 11-20
   - Rootblight polish
   - Urda prototype/vertical slice
6. Current blockers.
7. Commands that work.
8. Next best action.

Keep it concise. This is for context compression.

Update `docs/README.md` to point to `../PROJECT_STATE.md` as the first current-state memory if created.

Phase 1 �?Markdown/document inventory

Create a script or one-time audit document:

- `docs/doc-inventory.md`

It must list markdown docs by category:

A. Current entrypoints:
- README.md
- AGENTS.md
- PROJECT_STATE.md
- docs/README.md
- docs/PROJECT_MAP.md
- docs/issues.md
- docs/release-checklist.md
- docs/private-beta-verification-handoff.md
- docs/rc1-live-validation-log.md

B. Feature docs:
- docs/features/ancients-rework-v4/
- docs/features/ascension-11-20/
- docs/features/ancient-expansion-urda/

C. Style/agent docs:
- docs/style/
- docs/skills/

D. Archive:
- docs/archive/

E. Candidate clutter / prompt dumps / duplicated issue addenda:
- docs/codex-urda-overnight-prompt.md
- docs/issues-urda-overnight-addendum.md
- docs/issues-waiting-tests.md
- any other prompt dump, one-off addendum, copied task, stale spec, duplicated handoff, or historical planning file in current docs

For each clutter candidate, decide:
- keep current
- merge into current doc
- move to archive
- delete only if fully duplicated and safe

Prefer archive.

Phase 2 �?Documentation information architecture

Apply these rules:

1. Current docs should be small and navigable.
2. Huge prompt files should not live at docs root.
3. Issue addenda should not live as standalone current docs after being merged.
4. `docs/issues.md` should be an active issue index, not a full historical transcript.
5. Feature-level details should live under `docs/features/<feature>/`.
6. Historical prompt/spec material should live under `docs/archive/`.

Required moves/merges:

A. Move prompt dumps to archive

Move, do not delete, unless duplicated:

- `docs/codex-urda-overnight-prompt.md`
  -> `docs/archive/prompts/2026-05/codex-urda-overnight-prompt.md`

- `docs/issues-urda-overnight-addendum.md`
  -> `docs/archive/prompts/2026-05/issues-urda-overnight-addendum.md`

If `docs/issues-waiting-tests.md` is just a stale split of `docs/issues.md`, either:
- merge active content into `docs/issues.md` or feature issues, then archive it:
  `docs/archive/issues/issues-waiting-tests-2026-05.md`
or
- keep it only if a current doc links to it as live evidence.

B. Split issues if necessary

If `docs/issues.md` is still enormous, split it into:

- `docs/issues.md`: compact active index, no giant implementation prose.
- `docs/issues/release-blockers.md`
- `docs/issues/ascension.md`
- `docs/issues/rootblight.md`
- `docs/issues/ancients-rework.md`
- `docs/issues/urda.md`
- `docs/issues/archive.md` or use `docs/archive/issues/`

`docs/issues.md` must include:
- current active blocker table
- links to split issue files
- how to close issues
- no release-ready false claim

If splitting breaks tests, update tests to follow the new issue file locations.

C. Update docs indexes

Update:
- docs/README.md
- docs/PROJECT_MAP.md
- docs/features/README.md
- docs/archive/README.md
- EZMicroBalanceCode/README.md
- tests/EZMicroBalance.Tests/README.md if needed

D. Add historical header to archived docs

Every moved archive file should start with:

```md
> Historical archive. Do not treat as current implementation truth. Current entrypoint: <link>.
```

Phase 3 �?Urda status cleanup

Current `EZMicroBalanceCode/Ancients/UrdaAncient.cs` appears to add Urda to Act 1 via patches and records selected blessing id, but active blessing effects are shallow or missing. This is dangerous if docs imply a playable Ancient.

Inspect:

- `EZMicroBalanceCode/Ancients/UrdaAncient.cs`
- `AncientSavedStateFields.UrdaStateKey`
- `EZMicroBalance/localization/{eng,zhs}/ancients.json`
- Urda feature docs
- tests referencing Urda

Decide one of two paths:

Path A �?Urda is prototype only:

- Keep code behind a default-off feature gate.
- Urda must not appear in normal live offer pool.
- Enable only via debug env var:
  - `EZMB_FORCE_ANCIENT=URDA`
- Update docs/issues/release checklist:
  - Urda prototype exists.
  - Blessing effects are not fully implemented.
  - Not part of release/test unless explicitly forced.
- Tests should require the gate.

Path B �?Urda is intended active/playable:

- Implement or verify at least 4 real blessing effects.
- If effects are not implemented, do not take this path.
- Active Urda blessing options must not just set a string and `Done()`.
- Every offered blessing must have gameplay behavior and save/load.
- Do not claim playable without live test.

Recommendation for this cleanup pass:
- Choose Path A unless the effects are already truly implemented.
- This is a refactor/cleanup pass, not a feature completion pass.

Move Urda source into a clear module if keeping it:

- from `EZMicroBalanceCode/Ancients/UrdaAncient.cs`
- to `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaAncient.cs`
- or `EZMicroBalanceCode/Ancients/Custom/Urda/UrdaAncient.cs`

Also move or create:
- `UrdaFeatureGate.cs`
- `UrdaBlessingIds.cs`
- `UrdaState.cs` if needed

Update namespaces, `.cs.uid` if applicable, source guard tests, and module README.

Phase 4 �?Code architecture cleanup

Do not move the entire codebase. Make targeted cleanup only.

Required improvements:

1. Keep existing Ancient reward rebalance code separate from Ancient expansion code.
   - Existing current code:
     - `Ancients/Common`
     - `Ancients/Patches`
   - New Urda/Ancient expansion code:
     - `Ancients/Expansion/Urda`
     or equivalent.

2. Keep Ascension boundaries as they are unless broken:
   - `Ascension/Core`
   - `Ascension/Map`
   - `Ascension/Combat`
   - `Ascension/Rewards`
   - `Ascension/Patches`
   - `Ascension/Cards`
   - etc.

3. Do not create generic frameworks unless a real repeated pattern exists.
4. Do not rewrite existing working patches.
5. If an abstraction is added, document why it reduces duplicated logic now.

Phase 5 �?Root cause and invariants

Add project rules based on the user's vibecoding notes.

Update `AGENTS.md` and/or `docs/skills/sts2-godot-mod-development.md` with concise rules:

- Fix root causes, not downstream normalization helpers.
- Do not add sanitizer/adapter layers to hide bad upstream state.
- Every large task must first create success criteria.
- Every feature implementation must have source evidence, tests, manual rows, and honest live status.
- Current docs first, archive docs second.
- Context management:
  - read `PROJECT_STATE.md` first
  - update `PROJECT_STATE.md` after major changes
  - do not force future agents to read archived prompt dumps

Do not bloat AGENTS. Add a short pointer to detailed docs if needed.

Phase 6 �?Test guard updates

Update tests to enforce the cleanup:

1. Current docs guard:
   - `PROJECT_STATE.md` exists.
   - `docs/README.md` points to it.
   - `docs/PROJECT_MAP.md` reflects `docs/features/ancient-expansion-urda/` if active.

2. Prompt dump guard:
   - No current root-level `docs/codex-*.md`.
   - No current root-level `docs/*addendum*.md`.
   - Prompt dumps must live in `docs/archive/prompts/`.

3. Issue split guard:
   - If issues are split, tests read the new issue files.
   - Current docs still contain required release blockers.

4. Urda safety guard:
   - If Urda is prototype, normal live gate must default off.
   - If Urda is active, tests must prove at least 4 implemented blessings with non-noop behavior.
   - No Morvi/Lotha/Vakuu active content.

5. Archive guard:
   - Archived docs have historical header or are listed in archive README.

6. Package guard:
   - PCK export preset must not include docs/archive, prompt dumps, source code, art pipeline, etc.

Phase 7 �?Validation

Run:

- git status --short --branch
- git log -1 --oneline --decorate
- dotnet build EZMicroBalance.sln
- dotnet test EZMicroBalance.sln
- dotnet test EZMicroBalance.sln --no-build
- dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
- git diff --check

If code/resources/localization/export preset changed:

- dotnet publish EZMicroBalance.sln
- dotnet test EZMicroBalance.sln --no-build
- `$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'; dotnet test EZMicroBalance.sln --no-build; Remove-Item Env:\EZMB_RUN_RELEASE_ARTIFACT_TESTS`
- refresh package staging/versioned/zip if publish/package changed
- update hashes from actual artifacts only

If docs-only:

- do not publish/package.

Phase 8 �?Final report format

Final response must include:

1. Current HEAD and git status.
2. Whether this was docs-only or code-moving.
3. Files moved to archive.
4. Files merged or deleted.
5. New current doc entrypoints.
6. Urda status:
   - prototype gated, or active playable
   - exact active blessings if active
   - disabled blessings if any
7. Tests/commands and exact results.
8. Whether package was rebuilt.
9. Remaining blockers:
   - multiplayer matrix
   - Ancient reward matrix
   - save/load
   - Urda implementation if prototype
   - Rootblight visual final checks if still pending
10. Release-ready: no, unless all release gates truly pass.

Important:

Do not just move markdown around and stop. The cleanup must reduce future reading cost and make project state truthful.

Do not close issues that still require live evidence.

Do not claim Urda is playable unless blessing behavior is implemented and tested.



