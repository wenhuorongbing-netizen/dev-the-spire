using System.IO.Compression;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class ReleasePackageArtifactGuardTests
{
    private static readonly string[] InstallableArtifactFiles =
    [
        "EZMicroBalance.dll",
        "EZMicroBalance.json",
        "EZMicroBalance.pck"
    ];

    private static readonly string[] PackagedFiles =
    [
        "EZMicroBalance.dll",
        "EZMicroBalance.json",
        "EZMicroBalance.pck",
        "README_INSTALL.txt"
    ];

    private static readonly string[] CurrentFacingDocs =
    [
        "README.md",
        "docs/dev-environment.md",
        "docs/private-beta-verification-handoff.md",
        "docs/private-beta-release-completion-audit.md",
        "docs/test-plan.md",
        "docs/test-ready-completion-audit.md",
        "docs/release-checklist.md",
        "docs/features/ancients-rework-v4/completion-audit.md",
        "docs/features/ancients-rework-v4/manual-verification-matrix.md",
        "docs/features/ascension-11-20/api-research.md",
        "docs/features/ascension-11-20/manual-test-checklist.md"
    ];

    [ReleaseArtifactFact]
    public void PackageStagingVersionedZipAndInstalledArtifactsHaveMatchingHashes()
    {
        var version = ManifestVersion();
        var packageName = $"SpirePlus-{version}";
        var installedDir = GamePath("mods", "EZMicroBalance");
        var stagingDir = RepoPath("publish", "package-staging", "EZMicroBalance");
        var versionedDir = RepoPath("publish", packageName, "EZMicroBalance");
        var zipPath = RepoPath("publish", $"{packageName}.zip");
        var legacyZipPath = RepoPath("publish", $"EZMicroBalance-{version}.zip");

        AssertDirectoryContainsOnlyFiles(stagingDir, PackagedFiles);
        AssertDirectoryContainsOnlyFiles(versionedDir, PackagedFiles);
        Assert.True(File.Exists(zipPath), $"Missing package zip: {zipPath}");
        Assert.False(File.Exists(legacyZipPath), $"Do not ship the player-facing archive under the technical id: {legacyZipPath}");

        using var archive = ZipFile.OpenRead(zipPath);
        var zipEntries = archive.Entries
            .Where(entry => !string.IsNullOrEmpty(entry.Name))
            .Select(entry => entry.FullName.Replace('\\', '/'))
            .OrderBy(entry => entry, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(PackagedFiles.Select(file => $"EZMicroBalance/{file}").OrderBy(file => file, StringComparer.Ordinal), zipEntries);

        foreach (var fileName in InstallableArtifactFiles)
        {
            var installedHash = Sha256(Path.Combine(installedDir, fileName));
            Assert.Equal(installedHash, Sha256(Path.Combine(stagingDir, fileName)));
            Assert.Equal(installedHash, Sha256(Path.Combine(versionedDir, fileName)));
            Assert.Equal(installedHash, Sha256(ReadZipBytes(archive, $"EZMicroBalance/{fileName}")));
        }

        var stagingReadmeHash = Sha256(Path.Combine(stagingDir, "README_INSTALL.txt"));
        Assert.Equal(stagingReadmeHash, Sha256(Path.Combine(versionedDir, "README_INSTALL.txt")));
        Assert.Equal(stagingReadmeHash, Sha256(ReadZipBytes(archive, "EZMicroBalance/README_INSTALL.txt")));
    }

    [ReleaseArtifactFact]
    public void CurrentDocsMatchReleaseHashesAndAvoidPinnedStaleTestTotals()
    {
        var packageHash = Sha256(RepoPath("publish", $"SpirePlus-{ManifestVersion()}.zip"));
        var modImageHash = Sha256(RepoPath("EZMicroBalance", "mod_image.png"));
        var legacyModImageHash = Sha256(RepoPath("EzDailyContent", "mod_image.png"));

        Assert.NotEqual(legacyModImageHash, modImageHash);

        var docsByPath = CurrentFacingDocs.ToDictionary(path => path, path => ReadRepoText(path.Split('/')), StringComparer.Ordinal);
        var combinedDocs = string.Join(Environment.NewLine, docsByPath.Values);

        Assert.Contains(packageHash, docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.Contains(packageHash, docsByPath["docs/dev-environment.md"], StringComparison.Ordinal);
        Assert.Contains(packageHash, docsByPath["docs/features/ancients-rework-v4/completion-audit.md"], StringComparison.Ordinal);
        Assert.Contains(modImageHash, docsByPath["docs/dev-environment.md"], StringComparison.Ordinal);

        Assert.DoesNotMatch(@"\b(?:24|28|34)\s*/\s*(?:24|28|34)\b", combinedDocs);
        Assert.DoesNotMatch(@"\b(?:24|28|34)\s+tests?\b", combinedDocs);
        Assert.DoesNotMatch(@"passed\s+(?:24|28|34)\b", combinedDocs);
        Assert.DoesNotContain("failed 5 tests", combinedDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("invalidated by later source/art changes", combinedDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("not current for release", combinedDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("hash parity is broken", combinedDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("release pass is blocked", combinedDocs, StringComparison.OrdinalIgnoreCase);

        Assert.Contains("manual feature verification", docsByPath["README.md"], StringComparison.OrdinalIgnoreCase);
        Assert.Contains("still pending", docsByPath["README.md"], StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Latest normal Steam-client startup/log verification is historical for the pre-review Spire Plus display-name package", docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.Contains("current-spire-plus-modsettings-20260513-111342", docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.Contains("RC1 normal Steam-client Mod Settings UI verification remains historical evidence for the old EZ Micro Balance display name", docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.Contains("Manual feature results are pending", docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.DoesNotContain("private beta ready", combinedDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("release ready", combinedDocs, StringComparison.OrdinalIgnoreCase);
    }

    [ReleaseArtifactFact]
    public void PrivateBetaVerificationHandoffCarriesCurrentArtifactsAndManualBlockers()
    {
        var packageHash = Sha256(RepoPath("publish", $"SpirePlus-{ManifestVersion()}.zip"));
        var installedDir = GamePath("mods", "EZMicroBalance");
        var dllHash = Sha256(Path.Combine(installedDir, "EZMicroBalance.dll"));
        var manifestHash = Sha256(Path.Combine(installedDir, "EZMicroBalance.json"));
        var pckHash = Sha256(Path.Combine(installedDir, "EZMicroBalance.pck"));
        var handoff = ReadRepoText("docs", "private-beta-verification-handoff.md");

        Assert.Contains(packageHash, handoff, StringComparison.Ordinal);
        Assert.Contains(dllHash, handoff, StringComparison.Ordinal);
        Assert.Contains(manifestHash, handoff, StringComparison.Ordinal);
        Assert.Contains(pckHash, handoff, StringComparison.Ordinal);
        Assert.Contains("Record results in `docs/features/ancients-rework-v4/manual-verification-matrix.md`", handoff, StringComparison.Ordinal);
        Assert.Contains("update `docs/release-checklist.md`", handoff, StringComparison.Ordinal);
        Assert.Contains("Historical normal Steam-client startup/log verification passed for an earlier Spire Plus display-name package", handoff, StringComparison.Ordinal);
        Assert.Contains("Current Mod Settings list screenshot shows `Spire Plus`", handoff, StringComparison.Ordinal);
        Assert.Contains("current-spire-plus-modsettings-20260513-111342", handoff, StringComparison.Ordinal);
        Assert.Contains("Live Ancient reward gameplay, broader save/load, disable-gameplay, and multiplayer checks are still pending", handoff, StringComparison.Ordinal);
        Assert.Contains("A11-A20 selection is now default-on in this private-beta multiplayer test candidate", handoff, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1", handoff, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1", handoff, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required", handoff, StringComparison.Ordinal);
        Assert.Contains("docs/features/ascension-11-20/multiplayer-test-runbook.md", handoff, StringComparison.Ordinal);
        Assert.Contains("scripts/audit-godot-log.ps1 -Path <copied godot.log>", handoff, StringComparison.Ordinal);
        Assert.Contains("godot-log-audit.json", handoff, StringComparison.Ordinal);
        Assert.Contains("Live co-op selection and desync verification are still pending", handoff, StringComparison.Ordinal);
        Assert.Contains("Resolved for this candidate: `EZMicroBalance.json` author is `wenhuorongbing-netizen`, taken from the local Git user name.", handoff, StringComparison.Ordinal);
        Assert.Contains("Rootblight I/II/III and Blight Sprout use original generated portrait art at the documented per-card filenames.", handoff, StringComparison.Ordinal);
        Assert.Contains("Live in-game visual verification is still pending.", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("AUTHOR_NAME_REPLACE_ME", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("This remains a release blocker unless the user either provides the desired author name or explicitly accepts that placeholder", handoff, StringComparison.Ordinal);
        Assert.Contains("## Review Scope", handoff, StringComparison.Ordinal);
        Assert.Contains("This handoff is not a commit manifest", handoff, StringComparison.Ordinal);
        Assert.Contains("Do not trust a point-in-time dirty-file list", handoff, StringComparison.Ordinal);
        Assert.Contains("git status --short --branch", handoff, StringComparison.Ordinal);
        Assert.Contains("git log -1 --oneline --decorate", handoff, StringComparison.Ordinal);
        Assert.Contains("git diff --stat", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("A1.05.01", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Current git status before", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Pre-commit local cleanup status summary", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("M EZMicroBalanceCode/Ascension/Rewards/RootDeckService.cs", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("b82023c", handoff, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("f201508", handoff, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("96bfa50", handoff, StringComparison.Ordinal);
        Assert.Contains("Proposed commit scope", handoff, StringComparison.Ordinal);
        Assert.Contains("Do not include", handoff, StringComparison.Ordinal);
        Assert.Contains("Directory.Build.props", handoff, StringComparison.Ordinal);
        Assert.Contains("archived local `art_pipeline` / `asset` material under `.tools/archive/local-art-and-calibration-20260515/`", handoff, StringComparison.Ordinal);
        Assert.Contains("`source code/` local scratch/reference folders", handoff, StringComparison.Ordinal);
        Assert.Contains("Push only after explicit user approval", handoff, StringComparison.Ordinal);
    }

    [Fact]
    public void ReleaseArtifactAndRuntimeEvidenceTestsAreExplicitlyOptIn()
    {
        var testSource = ReadAllTestSource().Replace("\r\n", "\n");
        var testPlan = ReadRepoText("docs", "test-plan.md");
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var handoff = ReadRepoText("docs", "private-beta-verification-handoff.md");

        Assert.Contains("ReleaseArtifactFactAttribute", testSource, StringComparison.Ordinal);
        Assert.Contains("EZMB_RUN_RELEASE_ARTIFACT_TESTS", testSource, StringComparison.Ordinal);
        Assert.Contains("Skipping release artifact/runtime checks", testSource, StringComparison.Ordinal);

        foreach (var methodName in new[]
        {
            "PrivateBetaZipContainsOnlyInstallableActiveModFiles",
            "PackageContainsCurrentAscensionLocalization",
            "ActiveReleaseArtMatchesAuditedNoTextNoLogoAsset",
            "PublishedPckContainsOnlyActiveReleaseResources",
            "InstalledDllMatchesABuildOutput",
            "InstalledManifestMatchesRepositoryManifest",
            "HarmonyPatchesResolveAgainstInstalledGameApi",
            "InstalledUrdaUsesCustomAncientAssetPaths",
            "PrismaticGemRewardBannerContractMatchesInstalledGameApi",
            "PackageStagingVersionedZipAndInstalledArtifactsHaveMatchingHashes",
            "CurrentDocsMatchReleaseHashesAndAvoidPinnedStaleTestTotals",
            "PrivateBetaVerificationHandoffCarriesCurrentArtifactsAndManualBlockers",
            "ActiveCoverArtAndInactiveModRealPolicyMatchExportPckAndPackage",
            "ExportedResourcesInstalledPckAndPackagePckStayInParity",
            "CurrentReleaseHashClaimsMatchInstalledStagingVersionedAndZipArtifacts",
            "RecentSmokeLogSupportsControlledSmokeClaims",
            "DisabledSpirePlusPlugOffEvidenceSupportsDocs"
        })
        {
            Assert.Contains($"[ReleaseArtifactFact]\n    public void {methodName}", testSource, StringComparison.Ordinal);
        }

        Assert.Contains("EZMB_RUN_RELEASE_ARTIFACT_TESTS=1", testPlan, StringComparison.Ordinal);
        Assert.Contains("skipped in normal developer test runs", testPlan, StringComparison.Ordinal);
        Assert.Contains("Release artifact tests are opt-in", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("EZMB_RUN_RELEASE_ARTIFACT_TESTS=1", handoff, StringComparison.Ordinal);
    }
}
