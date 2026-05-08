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
