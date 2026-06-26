# beta.135 Runtime Baseline Capture — Turnkey Prep PRD

> Lane: debug. Scope: make the owner's beta.135 in-game runtime baseline capture
> turnkey. This is non-owner-run prep only. The actual game launch is owner-only
> and must NOT be attempted by this lane.

## 1. Problem / Objective

The only open gate for beta.135 is that the in-game runtime proof has never been
captured. The evidence tooling already exists
(`scripts/new-beta135-runtime-baseline-evidence.ps1`,
`scripts/check-beta135-runtime-baseline-log.ps1`,
`scripts/run-spire-plus-monkey-stability.ps1`,
`docs/testing/beta135-runtime-baseline.md`), but the owner-facing capture path is
spread across several scripts with different parameter shapes and several
patch-count caveats. The owner should be able to:

1. Trust that everything that can be checked WITHOUT launching the game is green.
2. Run one minimal StartupOnly smoke command, then two follow-up commands, to
   produce a verifiable evidence packet.

"Turnkey" = the owner copies an exact command sequence from a single RUNBOOK and
does not have to reason about parameters, paths, or patch-count nuances.

## 2. In Scope

- RUN the no-launch preflight/parity checkers and report PASS/BLOCKED per check
  with evidence:
  - `scripts/check-sts1-runtime-preflight.ps1` (versions, manifests, RitsuLib
    layout, expected-shape) — no launch.
  - `scripts/check-installed-spire-plus-package.ps1` (installed DLL/JSON/PCK/
    README hash parity + package zip + PCK content) — no launch.
- Confirm: beta.135 packaged + installed under `<GameRoot>\mods\EZMicroBalance`,
  versions match (game `0.107.1` / RitsuLib `0.4.34` / package
  `v0.1.0-private-beta.135`), `Directory.Build.props` `Sts2Path` set, STS2-RitsuLib
  present.
- Produce/update a concise RUNBOOK with the EXACT owner command sequence:
  StartupOnly smoke (via `run-spire-plus-monkey-stability.ps1`) → feed
  `godot.log.after-launch` + screenshot into
  `new-beta135-runtime-baseline-evidence.ps1` → verify with
  `check-beta135-runtime-baseline-log.ps1`.

## 3. Out of Scope / Non-Goals (HARD)

- Do NOT launch Steam or the game (no `-Launch` on the monkey runner; no
  `spire-plus-live-session.ps1 -Launch`).
- Do NOT commit or push (coordinator integrates centrally).
- Do NOT fabricate runtime PASS. Startup / main-menu / screenshot / runtime-patch
  rows stay `pending-owner-run` until the owner supplies a real log.
- Do NOT change product logic, `Sts1Events/`, `MainFile.cs`, `Core/Features`,
  `*.csproj`, or manifests. Allowed files: `scripts/**`, `tests/**`,
  `docs/testing/**` (read anything).
- Avoid `dotnet build`/`dotnet test` unless necessary; if a build hits a
  file-in-use lock, wait and retry.

## 4. Current-Repo Reconciliation (read now, verified on disk)

- `Directory.Build.props` `Sts2Path = E:/Steam/steamapps/common/Slay the Spire 2`
  (set, present on disk).
- Installed `mods\EZMicroBalance`: `EZMicroBalance.dll`, `.json`, `.pck`, `.pdb`,
  `README_INSTALL.txt` present. Manifest reports `EZMicroBalance` / `Spire Plus`
  / `v0.1.0-private-beta.135`.
- Installed `mods\STS2-RitsuLib`: direct-NuGet layout
  (`STS2-RitsuLib.dll`, `.xml`, `mod_manifest.json` directly in the folder; no
  `lib\0.107.1\` variant subtree). Manifest reports `STS2-RitsuLib` / `0.4.34`.
- Game `release_info.json` reports `v0.107.1`.
- Repo `EZMicroBalance.json` matches installed (`v0.1.0-private-beta.135`).
- Package zip `publish\SpirePlus-v0.1.0-private-beta.135.zip` present.
- Handoff hashes for beta.135 (zip/DLL/manifest/PCK/README) are recorded in
  `docs/private-beta-verification-handoff.md` (the installed-package checker reads
  them from there).

### Patch-count caveat (must be reflected in the RUNBOOK)

There are TWO independent runtime lanes with different patch-count semantics:

- The **beta.135 runtime-baseline** lane
  (`check-beta135-runtime-baseline-log.ps1`) expects `-ExpectedPatchCount 168`
  (default compiled/applied count; 169 source classes minus the compile-gated
  `Sts1ReplacementPrototype`). This is the lane the RUNBOOK drives.
- The **runtime-monkey** lane (`check-spire-plus-runtime-monkey-packet.ps1`)
  derives `-ExpectedPatchCount` fresh from the captured current-iteration
  `ModPatcher applied N patches (N registered)` log line. Historical values such
  as `25` (runner default), `144`, or `152` (beta.128) must NOT be reused for
  beta.135.

The owner's StartupOnly smoke is the LOG SOURCE. The same captured
`godot.log.after-launch` feeds the baseline assembler/checker (which uses 168).
The monkey runner's own `-ExpectedPatchCount` default (25) is irrelevant to the
StartupOnly capture because StartupOnly sends no commands and the RUNBOOK does not
run the monkey packet checker; if the owner additionally wants a monkey packet,
they set `-ExpectedPatchCount` from the freshly captured log line.

## 5. File Plan (all within Allowed Files)

- CREATE `docs/testing/beta135-runtime-baseline-turnkey-prd.md` (this file).
- UPDATE `docs/testing/beta135-runtime-baseline.md`: add a single "Turnkey Owner
  Capture Runbook" section with the exact StartupOnly → assemble → verify command
  sequence and the patch-count caveat. Do not weaken existing no-fabrication or
  no-claim language.
- No script edits required (the existing scripts already support the no-launch
  scaffold + StartupOnly dry-run/owner-launch split). If a gap is found, scope it
  in `scripts/**` only and note it.

## 6. Acceptance Criteria ("fully implemented")

A. **Preflight reporting** — both no-launch checkers RUN by this lane, with a
   per-check PASS/BLOCKED table backed by their own stdout/JSON. The four required
   facts (packaged+installed, versions match, `Sts2Path` set, RitsuLib present)
   are each tied to a named passing check.

B. **RUNBOOK turnkey** — a reader can copy the command blocks in order and:
   (1) run the StartupOnly smoke to capture a log, (2) run ONE assemble command
   (no `-EvidenceDir`, so it creates a fresh directory) that ingests the captured
   log + optional screenshot and auto-runs the checker, (3) optionally re-verify.
   The assemble step must NOT reuse a pre-populated directory (the assembler fails
   closed on a non-empty `-EvidenceDir`), so the optional Step 1 scaffold is
   preview-only. Every owner-launch step is explicitly labelled `pending-owner-run`.

C. **No forbidden actions** — no Steam/game launch performed; no commit/push; no
   product/locked files touched; no fabricated runtime evidence.

D. **Self-review** — an independent reviewer pass confirms A–C against the actual
   files and command outputs, and confirms the assemble/verify parameter shapes
   match the real script `param(...)` blocks (no invented switches).

## 7. Risks / Dependencies / Rollback

- Dependency: actual log capture is owner-run (Steam). Everything up to that
  boundary is delivered; the launch itself is flagged `pending-owner-run`.
- Risk: patch-count confusion (168 baseline vs fresh monkey count). Mitigated by
  an explicit caveat block in the RUNBOOK.
- Risk: RitsuLib direct-NuGet layout could look like a missing `lib\<branch>\`
  variant. Mitigated: `check-sts1-runtime-preflight.ps1` already accepts either
  layout (`ritsu_direct_or_variant_layout_detected`).
- Rollback: this lane only adds one doc and one doc section; reverting those two
  edits fully restores prior state. No code paths change.
