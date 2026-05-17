# EZMicroBalance.Tests

This test project guards source shape, localization, release documentation, package artifacts, and runtime evidence for `EZMicroBalance`.

## Test Groups

| File | Coverage |
| --- | --- |
| `AncientBehaviorGuardTests.cs` | Ancient behavior docs, localization, manual matrix, and active package shape. |
| `AncientHighRiskSourceGuardTests.cs` | High-risk source patterns for Ancient patches. |
| `AscensionFeatureGuardTests.cs` | Ascension selector, gate, docs, and source constraints. |
| `AscensionV2MilestoneGuardTests.cs` | Ascension milestone-level source/localization expectations. |
| `ReleaseArtifactTests.cs` | Installed/package artifacts and Harmony target resolution. |
| `ReleaseCoverageGuardTests.cs` | Release docs, handoff, package hash, and artifact parity. |
| `ReleaseSafetyExpandedGuardTests.cs` | Expanded package, smoke-log, and stale-claim guards. |
| `ReleaseArtifactFactAttribute.cs` | Opt-in gate for tests that require ignored local release artifacts. |
| `TestRepo.cs` | Shared repository path, game path, UTF-8 read, JSON map/value walking, source-slicing, manifest, PNG dimension, export-preset parsing, active release resource predicates, ZIP/PCK/hash, JSON normalization, exception-unwrapping, and source-evidence helpers for guard tests. |
| `TestInfrastructureGuardTests.cs` | Prevents guard-test infrastructure duplication and first-read documentation clutter from creeping back in. |

New guard files should use `TestRepo.cs` instead of copying local `FindRepoRoot`, `RepoPath`, or `ReadRepoText` helpers.

Use the shared `ReadSharedText` helper for logs or runtime evidence files that can be open for writing while tests inspect them.

Use the shared repository path assertion helpers for plain existence checks instead of copying generic `File.Exists` / `Directory.Exists` assertion wrappers.

Use the shared `AssertSourceContains` helper for source-shape evidence checks instead of redefining the same missing-snippet assertion in each guard file.

Use the shared `AssertNoMojibake` helper for common bad-encoding fragment checks; keep feature-specific fragment lists local to the test that owns them.

Use the shared `AssertLocalizedKeys` helper for bilingual key-existence and non-empty-value checks; pass a feature-specific value validator when a test also needs custom mojibake or wording checks.

Use the shared JSON/source-slicing helpers for common guard-test parsing instead of redefining `JsonStringMap`, `JsonStringValues`, `JsonKeys`, `SliceFrom`, `SliceBetween`, `AssertBefore`, or `CountOccurrences`.

Use the shared ZIP/PCK/hash helpers instead of redefining `ReadZipBytes`, `ReadZipText`, `ReadPckDirectory`, `ReadSourceTree`, `ReadAllTestSource`, or `Sha256`.

Use the shared manifest, PNG byte/dimension, JSON normalization, and exception-unwrapping helpers instead of redefining `ManifestVersion`, `ReadPngBytes`, `ReadPngDimensions`, `NormalizeJson`, or `Unwrap`.

Use the shared export-preset parser instead of redefining `ParseExportFiles`.

Use the shared active release resource predicates instead of redefining `IsActiveExportResource` or `IsActiveReleaseResource`.

## Normal Test Command

```powershell
dotnet test EZMicroBalance.sln --no-build
```

Normal runs skip tests that depend on ignored local `publish/`, installed DLL/PCK, package zip, or smoke-log artifacts.

## Release Artifact Test Command

```powershell
$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build
```

Run the opt-in suite only after `dotnet publish`, package refresh, and controlled smoke evidence are current.
