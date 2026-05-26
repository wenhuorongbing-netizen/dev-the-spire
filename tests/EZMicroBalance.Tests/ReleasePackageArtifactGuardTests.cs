using System.IO.Compression;
using System.Text;
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

    [ReleaseArtifactFact]
    public void PackageStagingVersionedZipAndInstalledArtifactsHaveMatchingHashes()
    {
        var packageName = CurrentPackageName();
        var installedDir = GamePath("mods", "EZMicroBalance");
        var stagingDir = RepoPath("publish", "package-staging", "EZMicroBalance");
        var versionedDir = RepoPath("publish", packageName, "EZMicroBalance");
        var zipPath = CurrentPackageZipPath();
        var legacyZipPath = RepoPath("publish", $"EZMicroBalance-{ManifestVersion()}.zip");

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
        Assert.DoesNotContain("EZMicroBalance/BaseLib.dll", zipEntries);
        Assert.DoesNotContain("EZMicroBalance/0Harmony.dll", zipEntries);
        Assert.DoesNotContain("EZMicroBalance/sts2.dll", zipEntries);

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

        var packageReadme = File.ReadAllText(Path.Combine(stagingDir, "README_INSTALL.txt"));
        Assert.Contains("Display name: Spire Plus", packageReadme, StringComparison.Ordinal);
        Assert.Contains("Technical compatibility id: EZMicroBalance", packageReadme, StringComparison.Ordinal);
        Assert.Contains("EZMicroBalance is a technical folder/id only; player-facing screens should say Spire Plus.", packageReadme, StringComparison.Ordinal);
        Assert.DoesNotContain("Compatibility id: EZMicroBalance", packageReadme, StringComparison.Ordinal);
        Assert.Contains("boss dedicated abilities", packageReadme, StringComparison.Ordinal);
        Assert.Contains("Branded Form", packageReadme, StringComparison.Ordinal);
        Assert.DoesNotContain("Royal Seal", packageReadme, StringComparison.Ordinal);
        Assert.DoesNotContain("Royal Seals", packageReadme, StringComparison.Ordinal);
        Assert.DoesNotContain("King Brand", packageReadme, StringComparison.Ordinal);
        Assert.DoesNotContain("King Brands", packageReadme, StringComparison.Ordinal);
    }

    [ReleaseArtifactFact]
    public void CurrentDocsMatchReleaseHashesAndAvoidPinnedStaleTestTotals()
    {
        var packageHash = CurrentPackageZipSha256();
        var modImageHash = Sha256(RepoPath("EZMicroBalance", "mod_image.png"));

        Assert.False(Directory.Exists(RepoPath("EzDailyContent")), "Legacy EzDailyContent resources should not return to the active root.");

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
        Assert.Contains("Fresh loader smoke for the current beta.71 package hash is pending", docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.Contains("This is loader/startup evidence, not gameplay proof", docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.Contains("current-spire-plus-modsettings-20260513-111342", docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.Contains("Earlier page-level Mod Settings evidence predates the display-name refresh", docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.Contains("Manual feature results are pending", docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.DoesNotContain("private beta ready", combinedDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("release ready", combinedDocs, StringComparison.OrdinalIgnoreCase);
    }

    [ReleaseArtifactFact]
    public void PrivateBetaVerificationHandoffCarriesCurrentArtifactsAndManualBlockers()
    {
        var packageHash = CurrentPackageZipSha256();
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
        Assert.Contains("Historical normal Steam-client startup/log verification passed for the beta.17 package hash", handoff, StringComparison.Ordinal);
        Assert.Contains("Historical normal Steam-client startup/log evidence confirms the display name", handoff, StringComparison.Ordinal);
        Assert.Contains("Current Mod Settings list screenshot shows `Spire Plus`", handoff, StringComparison.Ordinal);
        Assert.Contains("current-spire-plus-modsettings-20260513-111342", handoff, StringComparison.Ordinal);
        Assert.Contains("Live Ancient reward gameplay, broader save/load, disable-gameplay, and multiplayer checks are still pending", handoff, StringComparison.Ordinal);
        Assert.Contains("A11-A20 selection is default-on only for single-player standard lobbies", handoff, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1", handoff, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1", handoff, StringComparison.Ordinal);
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
        Assert.Contains("Push after validation and an intentional commit", handoff, StringComparison.Ordinal);
    }

    [ReleaseArtifactFact]
    public void InstalledAndPackagedPckCarrySereTalonTanxClawsSplit()
    {
        var installedPck = GamePath("mods", "EZMicroBalance", "EZMicroBalance.pck");
        Assert.True(File.Exists(installedPck), $"Missing installed PCK: {installedPck}");

        AssertSereTalonTanxClawsSplitIsPackaged(File.ReadAllBytes(installedPck), "installed PCK");

        using var archive = ZipFile.OpenRead(CurrentPackageZipPath());
        AssertSereTalonTanxClawsSplitIsPackaged(
            ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.pck"),
            "package PCK");
    }

    [ReleaseArtifactFact]
    public void InstalledAndPackagedPckCarryTrialBranchShortChoiceText()
    {
        var installedPck = GamePath("mods", "EZMicroBalance", "EZMicroBalance.pck");
        Assert.True(File.Exists(installedPck), $"Missing installed PCK: {installedPck}");

        AssertTrialBranchShortChoiceTextIsPackaged(File.ReadAllBytes(installedPck), "installed PCK");

        using var archive = ZipFile.OpenRead(CurrentPackageZipPath());
        AssertTrialBranchShortChoiceTextIsPackaged(
            ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.pck"),
            "package PCK");
    }

    [Fact]
    public void PackageScriptRejectsNoRefreshWhenStagingArtifactsAreMissing()
    {
        var script = ReadRepoText("scripts", "package-spire-plus.ps1");

        Assert.Contains("$requiredArtifactFiles = @('EZMicroBalance.dll', 'EZMicroBalance.json', 'EZMicroBalance.pck')", script, StringComparison.Ordinal);
        Assert.Contains("function Assert-RequiredArtifactFilesPresent", script, StringComparison.Ordinal);
        Assert.Contains("Installed artifact missing", script, StringComparison.Ordinal);
        Assert.Contains("NoRefreshFromInstalled uses existing package staging, but required artifact is missing", script, StringComparison.Ordinal);
        Assert.Contains("function Assert-StagedManifestMatchesRepository", script, StringComparison.Ordinal);
        Assert.Contains("foreach ($propertyName in @('id', 'name', 'version'))", script, StringComparison.Ordinal);
        Assert.Contains("Staged manifest $propertyName mismatch", script, StringComparison.Ordinal);
        Assert.Contains("Assert-StagedManifestMatchesRepository `", script, StringComparison.Ordinal);
        Assert.Contains("Assert-RequiredArtifactFilesPresent `", script, StringComparison.Ordinal);
        Assert.Contains("foreach ($fileName in $requiredArtifactFiles)", script, StringComparison.Ordinal);
    }

    [Fact]
    public void ReleaseArtifactAndRuntimeEvidenceTestsAreExplicitlyOptIn()
    {
        var testSource = ReadAllTestSource().Replace("\r\n", "\n");
        var testPlan = ReadRepoText("docs", "test-plan.md");
        var testReadme = ReadRepoText("tests", "EZMicroBalance.Tests", "README.md");
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var handoff = ReadRepoText("docs", "private-beta-verification-handoff.md");

        Assert.Contains("ReleaseArtifactFactAttribute", testSource, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS", testSource, StringComparison.Ordinal);
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
            "InstalledAndPackagedPckCarrySereTalonTanxClawsSplit",
            "InstalledAndPackagedPckCarryTrialBranchShortChoiceText",
            "ActiveCoverArtAndInactiveModRealPolicyMatchExportPckAndPackage",
            "ExportedResourcesInstalledPckAndPackagePckStayInParity",
            "CurrentReleaseHashClaimsMatchInstalledStagingVersionedAndZipArtifacts",
            "RecentSmokeLogSupportsControlledSmokeClaims",
            "DisabledSpirePlusPlugOffEvidenceSupportsDocs"
        })
        {
            Assert.Contains($"[ReleaseArtifactFact]\n    public void {methodName}", testSource, StringComparison.Ordinal);
        }

        Assert.Contains("SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1", testPlan, StringComparison.Ordinal);
        Assert.Contains("EZMB_RUN_RELEASE_ARTIFACT_TESTS=1", testPlan, StringComparison.Ordinal);
        Assert.Contains("$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'", testReadme, StringComparison.Ordinal);
        Assert.Contains("legacy `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` alias", testReadme, StringComparison.Ordinal);
        Assert.DoesNotContain("$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'", testReadme, StringComparison.Ordinal);
        Assert.Contains("skipped in normal developer test runs", testPlan, StringComparison.Ordinal);
        Assert.Contains("Release artifact tests are opt-in", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1", handoff, StringComparison.Ordinal);
        Assert.Contains("EZMB_RUN_RELEASE_ARTIFACT_TESTS=1", handoff, StringComparison.Ordinal);
    }

    private static void AssertSereTalonTanxClawsSplitIsPackaged(byte[] pckBytes, string context)
    {
        var pckText = Encoding.UTF8.GetString(pckBytes);

        Assert.Contains("\"SERE_TALON.description\": \"On pickup, choose [blue]1[/blue] of [blue]4[/blue] Curses. Add it, [blue]2[/blue] Wish, and [blue]1[/blue] Wish+ to your deck.\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"SERE_TALON.selectionScreenPrompt\": \"Choose 1 Curse.\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"SERE_TALON.title\": \"Vakuu's Sere Talon\"", pckText, StringComparison.Ordinal);
        Assert.Contains("sere_talon_spire_plus.png", pckText, StringComparison.Ordinal);
        Assert.Contains("\"SERE_TALON.description\": \"\u62fe\u53d6\u65f6\uff0c\u4ece[blue]4[/blue]\u5f20\u8bc5\u5492\u4e2d\u9009\u62e9[blue]1[/blue]\u5f20\u3002\u5c06\u5b83\u3001[blue]2[/blue]\u5f20[gold]\u8bb8\u613f[/gold]\u548c[blue]1[/blue]\u5f20[gold]\u8bb8\u613f+[/gold]\u52a0\u5165\u4f60\u7684\u724c\u7ec4\u3002\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"SERE_TALON.selectionScreenPrompt\": \"\u9009\u62e91\u5f20\u8bc5\u5492\u3002\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"SERE_TALON.title\": \"\u74e6\u5e93\u539f\u521d\u4e4b\u722a\"", pckText, StringComparison.Ordinal);

        Assert.Contains("\"CLAWS.description\": \"On pickup, transform up to [blue]{Cards}[/blue] cards into upgraded Maul.\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"CLAWS.title\": \"Tanx Claws\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"CLAWS.description\": \"\u62fe\u53d6\u65f6\uff0c\u5c06\u81f3\u591a[blue]{Cards}[/blue]\u5f20\u724c\u53d8\u5316\u4e3a\u6495\u54ac+\u3002\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"CLAWS.title\": \"\u5766\u514b\u65af\u5229\u722a\"", pckText, StringComparison.Ordinal);

        foreach (var staleFragment in new[]
                 {
                     "\"CLAWS.description\": \"Choose 1 of 4 Curses",
                     "No longer transforms deck cards",
                     "random Curses and [blue]3[/blue] Wish",
                     "\"SERE_TALON.description\": \"claws.png\"",
                     "Sere Talon\", \"CLAWS.description\"",
                     "Vakuu's Sere Talon\", \"CLAWS.description\""
                 })
        {
            Assert.DoesNotContain(staleFragment, pckText, StringComparison.Ordinal);
        }
    }

    private static void AssertTrialBranchShortChoiceTextIsPackaged(byte[] pckBytes, string context)
    {
        var pckText = Encoding.UTF8.GetString(pckBytes);

        Assert.Contains("\"EZMB_URDA.pages.INITIAL.options.urda_trial_branch.description\": \"Choose [blue]1[/blue] of [blue]4[/blue] cards.", pckText, StringComparison.Ordinal);
        Assert.Contains("\"EZMB_URDA.pages.INITIAL.options.urda_trial_branch.selectionScreenPrompt\": \"Choose [blue]1[/blue] card for [gold]Trial Branch[/gold].\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.description\": \"Choose [blue]1[/blue] of [blue]4[/blue] cards.", pckText, StringComparison.Ordinal);
        Assert.Contains("\"EZMB_URDA.pages.INITIAL.options.urda_trial_branch.description\": \"\u4ece[blue]4[/blue]\u5f20\u724c\u4e2d\u9009\u62e9[blue]1[/blue]\u5f20\u3002", pckText, StringComparison.Ordinal);
        Assert.Contains("\"EZMB_URDA.pages.INITIAL.options.urda_trial_branch.selectionScreenPrompt\": \"\u4e3a[gold]\u8bd5\u70bc\u679d\u6761[/gold]\u9009\u62e9[blue]1[/blue]\u5f20\u724c\u3002\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.description\": \"\u4ece[blue]4[/blue]\u5f20\u724c\u4e2d\u9009\u62e9[blue]1[/blue]\u5f20\u3002", pckText, StringComparison.Ordinal);

        foreach (var staleFragment in new[]
                 {
                     "\"EZMB_URDA.pages.INITIAL.options.urda_trial_branch.description\": \"Choose [blue]1[/blue] of [blue]4[/blue] [gold]rare[/gold]",
                     "\"EZMB_URDA.pages.INITIAL.options.urda_trial_branch.description\": \"Choose [blue]1[/blue] [gold]rare[/gold]",
                     "\"EZMB_URDA.pages.INITIAL.options.urda_trial_branch.selectionScreenPrompt\": \"Choose [blue]1[/blue] [gold]rare[/gold]",
                     "\"EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.description\": \"Choose [blue]1[/blue] of [blue]4[/blue] [gold]rare[/gold]"
                 })
        {
            Assert.DoesNotContain(staleFragment, pckText, StringComparison.Ordinal);
        }
    }
}
