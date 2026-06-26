# Migrated-Module Refactor Candidate Map — beta.135

Status: PLANNING (docs only). Companion to `refactor-entry-gate-beta135.md`.
Built by reading `EZMicroBalanceCode/**`, `docs/patch-inventory.md`, and the runtime emitters.
Reconciled against HEAD `b0f0b33`. **No `.cs` changed; nothing below is implemented.**

Ranked by **lowest runtime blast radius first**. "Blast radius" = how much a structural change
to the unit can perturb the beta.135 runtime baseline (the five diff fields in the entry gate:
godot.log events, patch-apply count = **168**, feature-gate state, loader path, key event counts).

## How the default baseline is reached (the invariant to preserve)

Default scenario = RitsuLib + EZMicroBalance only, launch STS2, reach **main menu** (no run,
no combat); debug logging OFF (`SpirePlusModConfig.ShowPreviewDebugLogs` false,
`SPIREPLUS_ENABLE_DEBUG_LOGS` / `SPIREPLUS_RELEASE_EVIDENCE_LOG` unset). At menu, only these
`[Spire Plus]`-family lines are emitted unconditionally (via `MainFile.Initialize()`):

- `RitsuLibBootstrap`: bootstrap start, **`ModPatcher applied 168 patches (… registered)`**,
  legacy-broad-discovery-disabled, framework-active.
- `FeatureRegistry.InitializeAll` + `LogFeatureSummary`: one gate line + one
  `bootstrap=…, live=…` line per feature module (6 modules).
- `ArchitectureCanaryBootstrap` reward handlers: one `RewardPipeline diagnostics observed …`
  line per feature-bootstrap event.

Everything gated by a flag/env var, or only reachable inside a run, contributes **nothing** at
menu — for those units the invariant is simply "still emits nothing at menu, count still 168".

## Blast-radius classes (legend)

- **(a) gated-off** — output suppressed behind an off-by-default flag/env var. Lowest.
- **(b) run-only** — only executes inside a run/combat, unreachable at menu. Low.
- **(c) YES-load** — registers/applies a patch at load -> counts toward the **168**. Higher.
- **(d) menu-log** — emits an unconditional log line at startup/menu. Higher (on the diff path).

---

## Candidate map (lowest -> highest runtime blast radius)

| # | Module / path (under `EZMicroBalanceCode/`) | Current RitsuLib patch class (PatchId) | Runtime-baseline dependency | Expected "no behavior change" observable | Slice size | Rollback path |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | `Preview/TransformPredictionService.cs` (`TransformPredictionService`, `PreviewTransformPolicy`) | **None** — pure `internal static`, no `IPatchMethod`, no PatchId | **(b) run-only.** No logging at all; only runs inside a card-transform preview during a run. 1 caller (`Preview/TransformPreviewPatch.cs`, same module). | Menu `godot.log` byte-identical (no Preview/transform lines pre-run) AND `ModPatcher applied 168 patches` unchanged. | XS (1 file, ~86 lines, 1 caller) | `git checkout -- EZMicroBalanceCode/Preview/TransformPredictionService.cs` |
| 2 | `Core/Architecture/RewardPipelineDiagnosticsContracts.cs` (+ `DeathProtectionDiagnosticsContracts.cs`, `MultiplayerPolicyDiagnosticsContracts.cs`) | None — enums/interface/record only, no body | **(b) types-only.** Declarative; no execution, no patch. Trap: enum string values (e.g. `PreGeneration`, `single`) feed the menu canary lines — preserve them. | Startup `Feature …` + `RewardPipeline diagnostics observed …` block byte-identical (enum names unchanged). | XS (3 small files) | `git checkout --` the three files |
| 3 | `Preview/PreviewLog.cs` (`PreviewLog`) | None | **(a) gated-off.** `Debug` gated by `ShowPreviewDebugLogs` (false); `Warn` only fires mid-run on a missing node. Silent at menu. 6 callers. | No `[Spire Plus] Preview:` lines in menu log (none before/after). | XS (1 file, ~19 lines) | `git checkout -- …/Preview/PreviewLog.cs` |
| 4 | `Core/Architecture/CardPlayContext.cs` (+ `CardPlayContextCanary.Diagnostics.cs`) | None | **(b) run-only / (a) gated.** "Skeleton" depth-guard; `EvaluateSingleExtraPlay` invoked only in gameplay; only sink is gated `ReleaseEvidenceLog`. 2 callers. | Menu log unchanged; no `CardPlayContextCanary` line (evidence env off). | S (2 files, ~150 lines) | `git checkout --` both files |
| 5 | `Core/Localization/InlineLocalizationTypes.cs` (`PowerLoc`/`RelicLoc`/`CardModifierLoc`) | None — data list-builders | **(b) data, on-miss-only.** Used only on a localization-table miss; no menu emission, no patch. Trap: `PowerLoc` has ~16 consumers — keep ctor signature + entry order. | `ModPatcher applied 168 patches` unchanged; localized strings on any opened panel identical. | S (1 file, ~73 lines, high fan-in on PowerLoc) | `git checkout -- …/Core/Localization/InlineLocalizationTypes.cs` |
| 6 | `Diagnostics/ReleaseEvidenceLog.cs` (`ReleaseEvidenceLog`) | None | **(a) gated-off** by `SPIREPLUS_RELEASE_EVIDENCE_LOG`. Produces nothing at default. Ranked here (not top) due to **~47-file fan-in** — restrict to private-formatter internals; do not touch the public `Log(...)` signatures. | Env unset -> zero `[SPIREPLUS-EVIDENCE]` lines before/after; menu log identical. | S body / L surface (47 callers) | `git checkout -- …/Diagnostics/ReleaseEvidenceLog.cs` |
| 7 | `Diagnostics/SpirePlusDebug.cs` (`SpirePlusDebug`) | None | **(a) gated-off** by `SPIREPLUS_ENABLE_DEBUG_LOGS`. Silent at menu. ~5 callers. Keep `[Spire Plus] [{category}]` format + truthiness rules byte-stable. | No gated debug lines appear (env unset); menu log identical. | S (1 file, ~57 lines) | `git checkout -- …/Diagnostics/SpirePlusDebug.cs` |
| 8 | `Preview/TransformPredictionRngContext.cs` (`TransformPredictionRngContext`) | None — `ConditionalWeakTable` store | **(b) run-only.** No patch, no menu emission. Slightly higher than #1: ~5 files (the RNG-source patch pairs) consume its `Register`/`TryConsume`/`Clear` API — that surface is load-bearing across the module. | Menu log identical; no RNG-context lines pre-run; count 168. | S (1 file, ~114 lines, 5 callers) | `git checkout -- …/Preview/TransformPredictionRngContext.cs` |
| 9 | `Core/Localization/SpirePlusInlineLocalizationRegistry.cs` (`SpirePlusInlineLocalizationRegistry`) | None — static registry (reflection) | **(b)+load side-effect.** `RegisterKnownProviders` runs at load but emits **no log** and applies **no patch**; resolution only on table miss. Underpins 4 active localization patches — a logic slip changes whether a key resolves. | `ModPatcher applied 168 patches` unchanged AND every currently-resolving key still resolves (panel text identical). | M (1 file, ~149 lines, load-time reflection) | `git checkout -- …/Core/Localization/SpirePlusInlineLocalizationRegistry.cs` |
| 10 | `Modding/ModInfoLocalizationPatches.cs` | **`ModInfoLocalizationPatches`** (`spire-plus-mod-info-localization`), registered in `Core/Integrations/RitsuLib/SpirePlusRitsuLibPatchRegistry.HostUi.cs` | **(c) YES-load.** Counts toward the **168**; body runs only when the Mod-Info panel opens. Clean internal target (extract the two big description constants) **without** touching the patch target/attribute. | `ModPatcher applied 168 patches` unchanged AND rendered mod description (EN/zhs) byte-identical when panel opened. | M (1 file, ~64 lines, IS a patch) | `git checkout -- …/Modding/ModInfoLocalizationPatches.cs` |

### Also-considered, deliberately NOT shortlisted (higher blast radius)

- `Core/Architecture/ArchitectureCanaryBootstrap.cs`, `RewardPipeline.cs`,
  `MultiplayerPolicy.cs`, `DeathProtectionService.cs` — **(d) menu-log.**
  `ArchitectureCanaryBootstrap.Initialize()` runs at load and its handlers emit the
  unconditional `RewardPipeline diagnostics observed …` menu lines (via
  `FeatureRegistry.InitializeAll` -> `RewardPipeline.Diagnose`). Their "stub/diagnostics-only"
  comments are misleading — they are squarely on the baseline diff path. Not safe-first.
- All `*Patch.cs` under `Preview/`, `Map/SpirePlusMapPointHoverComposer.cs`,
  `Core/Localization/SpirePlusInlineLocalizationPatches.cs`, and every other `IPatchMethod`
  class — **(c) YES-load**, each risks the **168** count / a `PatchId`. Patch-seam work needs
  owner approval per the entry gate.
- `Diagnostics/SpirePlusAncientLiveTestConsoleCmd*.cs` — not in the RitsuLib registry (it is an
  `AbstractConsoleCmd`, reflection-discovered by the game DevConsole), so it does not affect
  168, but it is a game-facing command type, not a private helper.
- `Core/Features/**` — owner-LOCKED and the source of the feature-gate log lines. Excluded.

---

## First candidate proposal (PROPOSAL ONLY — do NOT implement until baseline)

> Implementation is blocked: the beta.135 runtime baseline is `pending-owner-run`. This is a
> proposal of *what* the first slice would be and *what* must stay unchanged — not a change.

### Chosen unit: `EZMicroBalanceCode/Preview/TransformPredictionService.cs`
(the `TransformPredictionService` and `PreviewTransformPolicy` static classes)

### Why it is the lowest-risk first slice
1. **Not a patch.** No `IPatchMethod`, no `PatchId`, no entry in `SpirePlusRitsuLibPatchRegistry`.
   It therefore cannot move the **168** applied-patch count under any structural change — the
   single most load-bearing baseline field is untouchable here. (Verified: `IPatchMethod` count
   in the file = 0.)
2. **Not on the menu path.** It has zero logging and only executes deep inside a card-transform
   *preview during a run* (it consumes `RunState`, `IsInCombat`, combat option filters). At main
   menu it never runs, so it contributes nothing to the baseline `godot.log` — the invariant is
   simply "still emits nothing at menu", which a pure refactor preserves trivially.
3. **Single in-module caller.** Referenced by exactly one other *production* file — its sibling
   `Preview/TransformPreviewPatch.cs` (verified by grep; the only other hits are guard tests that
   read the file as text, not runtime callers). No cross-lane / public-API surface, so no BOARD
   "API 变更" row and no downstream-lane risk.
4. **Self-contained pure logic.** `internal static` helpers over MegaCrit card/RNG types; the
   refactor space is method extraction, naming, and reshaping `FilterLikeVanilla` — all inert to
   observable behavior as long as the RNG draw order and the returned option set are preserved.
5. **Tiny + trivially reversible.** ~86 lines, one file; rollback is a one-path `git checkout`.

### The one explicit guard (so this stays a refactor, not a behavior change)
`PredictReplacementModel` forks/consumes the caller-owned `Rng` and `FilterLikeVanilla` only
*reduces* the option set. The refactor **must preserve the RNG consumption order and the exact
filtered option set** (rarity gate, in-combat gate, self-exclusion, multiplayer-constraint
filter). Changing how many times `rng` is drawn, or the option ordering before `rng.NextItem`,
would change a *prediction* a player sees — that is a behavior change, not a refactor, and is
out of scope. (This is exactly the kind of "behavior assumption smuggled into a structural
proposal" the gate's review step must catch.)

### Expected unchanged observables for the before/after runtime diff
Same scenario both captures (RitsuLib + mod, reach main menu):

| Diff field | Expected |
| --- | --- |
| godot.log events | **Byte-identical** at main menu — no Preview/transform line is emitted pre-run before or after. |
| Patch-apply count | `ModPatcher applied 168 patches` — **unchanged** (this unit is not a patch). |
| Feature-gate state | All 6 modules' `bootstrap=…, live=…` lines **identical**. |
| Loader path | `MainFile.Initialize()` marker sequence **identical**; no new loader/initializer exception. |
| Key event counts | `godot-log-audit.json` signature/array/bool/int shape **identical**; manifest id set unchanged. |

Because this unit is run-only, the menu baseline alone cannot exercise its logic. If/when the
owner wants behavior-level assurance beyond "menu baseline unchanged", the optional deeper
check is: with the transform-preview feature on, trigger a card transform via the auto-play
harness on a fixed seed before and after, and confirm the predicted replacement card is the
same — but the **gate requirement** for merging this structural slice is the five-field menu
diff above, all unchanged.

### Status
**PROPOSAL ONLY. Do not implement until debug delivers the canonical, owner-approved beta.135
runtime baseline.** This is Slice D in the round plan and is correctly `BLOCKED-on-baseline`.
