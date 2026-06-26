using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseEvidenceGateTests
{
    private sealed record RequiredEvidence(string Key, string Description, Func<bool> IsPresent);

    private static string EscapedForPowerShellOutput(string value)
    {
        return value.Replace("\\", "\\\\", StringComparison.Ordinal);
    }

    [Fact]
    public void ReleaseEvidenceScriptsDeriveVersionedPackageArtifactPathsFromManifest()
    {
        var helper = ReadRepoText("scripts", "spire-plus-package-evidence.ps1");
        AssertSourceContains(
            helper,
            "Get-SpirePlusManifestVersion",
            "EZMicroBalance.json",
            "Resolve-SpirePlusPackagePath",
            "Get-SpirePlusPackageSha256",
            "Get-SpirePlusPackageArtifactRelativePaths",
            "Get-SpirePlusGitEvidence",
            "PushedHead",
            "HeadMatchesUpstream",
            "SpirePlus-$(Get-SpirePlusManifestVersion -RepoRoot $RepoRoot)");

        foreach (var scriptName in new[]
                 {
                     "collect-ancient-ui-evidence.ps1",
                     "collect-coop-evidence.ps1",
                     "collect-mod-settings-evidence.ps1",
                     "collect-preview-tools-evidence.ps1",
                     "collect-release-evidence.ps1",
                     "collect-vakuu-fight-evidence.ps1",
                     "verify-spire-plus-release-evidence.ps1"
                 })
        {
            var script = ReadRepoText("scripts", scriptName);
            AssertSourceContains(
                script,
                "spire-plus-package-evidence.ps1",
                "Get-SpirePlusPackageArtifactRelativePaths");
            Assert.DoesNotContain(CurrentPackageArtifactRelativePath("EZMicroBalance.dll"), script, StringComparison.Ordinal);
            Assert.DoesNotContain(CurrentPackageArtifactRelativePath("EZMicroBalance.pck"), script, StringComparison.Ordinal);
            Assert.DoesNotContain(CurrentPackageArtifactRelativePath("EZMicroBalance.json"), script, StringComparison.Ordinal);
            Assert.DoesNotContain(CurrentPackageArtifactRelativePath("README_INSTALL.txt"), script, StringComparison.Ordinal);
        }

        foreach (var scriptName in new[]
                 {
                     "collect-ancient-ui-evidence.ps1",
                     "collect-coop-evidence.ps1",
                     "collect-mod-settings-evidence.ps1",
                     "collect-preview-tools-evidence.ps1",
                     "collect-release-evidence.ps1",
                     "collect-vakuu-fight-evidence.ps1"
                 })
        {
            var script = ReadRepoText("scripts", scriptName);
            AssertSourceContains(
                script,
                "Get-SpirePlusGitEvidence -RepoRoot $repoRoot",
                "GitPushedHead",
                "GitHeadMatchesUpstream");
        }

        foreach (var scriptName in new[] { "collect-release-evidence.ps1", "verify-spire-plus-release-evidence.ps1" })
        {
            var script = ReadRepoText("scripts", scriptName);
            AssertSourceContains(
                script,
                "[string]$PackageSha256 = \"\"",
                "[string]$PackagePath = \"\"",
                "Get-SpirePlusPackageRelativePath -RepoRoot $repoRoot");
            Assert.DoesNotContain(
                CurrentPackageZipSha256(),
                script,
                StringComparison.Ordinal);
            Assert.DoesNotContain(
                $"PackagePath = \"{CurrentPackageZipRelativePath()}\"",
                script,
                StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ReleaseVerifierRejectsStalePackageHashRows()
    {
        var collector = AssertRepoFileExists("scripts", "collect-release-evidence.ps1");
        var verifier = AssertRepoFileExists("scripts", "verify-spire-plus-release-evidence.ps1");
        var evidenceDir = RepoPath(
            ".tools",
            "runtime-evidence",
            "test-release-evidence-gate",
            "stale-package-hashes",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(evidenceDir);
        try
        {
            var collectResult = RunPowerShell(collector, "-NoLaunch", "-EvidenceDir", evidenceDir);
            Assert.True(collectResult.ExitCode == 0, $"collect-release-evidence.ps1 -NoLaunch failed:{Environment.NewLine}{collectResult.Output}");

            var manifestPath = Path.Combine(evidenceDir, "release-evidence-manifest.json");
            var manifestNode = JsonNode.Parse(File.ReadAllText(manifestPath))!.AsObject();
            foreach (var rowNode in manifestNode["Rows"]!.AsArray())
            {
                var rowObject = rowNode!.AsObject();
                rowObject["Status"] = "deferred";
                rowObject["ExplicitOwnerDecision"] = true;
                rowObject["ReleaseNote"] = "Synthetic deferral for package-hash verifier contract test.";
            }

            var loaderNode = manifestNode["Rows"]!
                .AsArray()
                .Select(row => row!.AsObject())
                .Single(row => row["Id"]!.GetValue<string>() == "fresh-current-package-loader-smoke");
            loaderNode["Status"] = "pass";
            loaderNode["ResultNote"] = "Synthetic loader pass attempt with stale package-hashes rows.";
            loaderNode["ExplicitOwnerDecision"] = false;
            loaderNode["ReleaseNote"] = "";

            var loaderEvidenceDir = loaderNode["EvidenceDir"]!.GetValue<string>();
            var loaderLogPath = Path.Combine(loaderEvidenceDir, "godot.log");
            File.WriteAllText(loaderLogPath, "Synthetic loader log for package-hash verifier contract.");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "godot-log-audit.json"), CleanGodotLogAuditJson(loaderLogPath));
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "enabled-mods.txt"), "STS2-RitsuLib\r\nEZMicroBalance\r\n");

            var packageHashesPath = Path.Combine(loaderEvidenceDir, "package-hashes.json");
            var packageHashesNode = JsonNode.Parse(File.ReadAllText(packageHashesPath))!.AsObject();
            var staleFiles = new JsonArray();
            foreach (var fileNode in packageHashesNode["Files"]!.AsArray())
            {
                var fileObject = fileNode!.AsObject();
                var path = fileObject["Path"]!.GetValue<string>();
                if (path != CurrentPackageArtifactRelativePath("EZMicroBalance.dll"))
                {
                    staleFiles.Add(fileObject.DeepClone());
                }
            }

            staleFiles.Add(new JsonObject
            {
                ["Path"] = "publish\\EZMicroBalance.dll",
                ["Exists"] = false,
                ["Sha256"] = null,
                ["Length"] = null
            });
            packageHashesNode["Files"] = staleFiles;
            File.WriteAllText(
                packageHashesPath,
                packageHashesNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            File.WriteAllText(
                manifestPath,
                manifestNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var verifyResult = RunPowerShell(
                verifier,
                "-EvidenceRoot",
                evidenceDir,
                "-ManifestPath",
                manifestPath,
                "-AllowDeferred");
            Assert.NotEqual(0, verifyResult.ExitCode);
            Assert.Contains("package-hashes.json still records stale root publish artifact path", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains("publish\\\\EZMicroBalance.dll", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains("package-hashes.json is missing current package artifact row", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains(EscapedForPowerShellOutput(CurrentPackageArtifactRelativePath("EZMicroBalance.dll")), verifyResult.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(evidenceDir))
            {
                Directory.Delete(evidenceDir, recursive: true);
            }
        }
    }

    [Fact]
    public void ReleaseVerifierRejectsDuplicatePackageHashPathRows()
    {
        var verifier = AssertRepoFileExists("scripts", "verify-spire-plus-release-evidence.ps1");
        var evidenceDir = RepoPath(
            ".tools",
            "runtime-evidence",
            "test-release-evidence-gate",
            "duplicate-package-hash-path",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(evidenceDir);
        try
        {
            var (manifestPath, loaderEvidenceDir) = WriteSyntheticLoaderPassEvidence(
                evidenceDir,
                "Synthetic loader pass attempt with duplicate package-hashes path rows.");

            var packageHashesPath = Path.Combine(loaderEvidenceDir, "package-hashes.json");
            var packageHashesNode = JsonNode.Parse(File.ReadAllText(packageHashesPath))!.AsObject();
            var files = packageHashesNode["Files"]!.AsArray();
            var packageRow = files
                .Single(row => (string?)row?["Path"] == CurrentPackageZipRelativePath())!
                .AsObject();
            var staleDuplicate = packageRow.DeepClone().AsObject();
            staleDuplicate["Sha256"] = "STALE_PACKAGE_HASH";

            var duplicateFiles = new JsonArray();
            duplicateFiles.Add(staleDuplicate);
            foreach (var fileNode in files)
            {
                duplicateFiles.Add(fileNode!.DeepClone());
            }

            packageHashesNode["Files"] = duplicateFiles;
            File.WriteAllText(
                packageHashesPath,
                packageHashesNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var verifyResult = RunPowerShell(
                verifier,
                "-EvidenceRoot",
                evidenceDir,
                "-ManifestPath",
                manifestPath,
                "-AllowDeferred");

            Assert.NotEqual(0, verifyResult.ExitCode);
            Assert.Contains("package-hashes.json contains duplicate file row path", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains(EscapedForPowerShellOutput(CurrentPackageZipRelativePath()), verifyResult.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(evidenceDir))
            {
                Directory.Delete(evidenceDir, recursive: true);
            }
        }
    }

    [Fact]
    public void ReleaseVerifierRejectsCaseDriftedPackageHashArtifactRows()
    {
        var verifier = AssertRepoFileExists("scripts", "verify-spire-plus-release-evidence.ps1");
        var evidenceDir = RepoPath(
            ".tools",
            "runtime-evidence",
            "test-release-evidence-gate",
            "case-drifted-package-hash-path",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(evidenceDir);
        try
        {
            var (manifestPath, loaderEvidenceDir) = WriteSyntheticLoaderPassEvidence(
                evidenceDir,
                "Synthetic loader pass attempt with case-drifted package-hashes path rows.");

            var packagePath = CurrentPackageZipRelativePath();
            const string publishPrefix = "publish\\";
            var caseDriftedPackagePath = packagePath.StartsWith(publishPrefix, StringComparison.Ordinal)
                ? "Publish\\" + packagePath.Substring(publishPrefix.Length)
                : packagePath.ToUpperInvariant();

            var packageHashesPath = Path.Combine(loaderEvidenceDir, "package-hashes.json");
            var packageHashesNode = JsonNode.Parse(File.ReadAllText(packageHashesPath))!.AsObject();
            var packageRow = packageHashesNode["Files"]!
                .AsArray()
                .Single(row => (string?)row?["Path"] == packagePath)!
                .AsObject();
            packageRow["Path"] = caseDriftedPackagePath;
            File.WriteAllText(
                packageHashesPath,
                packageHashesNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var verifyResult = RunPowerShell(
                verifier,
                "-EvidenceRoot",
                evidenceDir,
                "-ManifestPath",
                manifestPath,
                "-AllowDeferred");

            Assert.NotEqual(0, verifyResult.ExitCode);
            Assert.Contains("package-hashes.json uses case-drifted current package artifact row", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains(EscapedForPowerShellOutput(packagePath), verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains(EscapedForPowerShellOutput(caseDriftedPackagePath), verifyResult.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(evidenceDir))
            {
                Directory.Delete(evidenceDir, recursive: true);
            }
        }
    }

    [Fact]
    public void ReleaseVerifierRejectsCleanOnlyGodotLogAuditSchema()
    {
        var collector = AssertRepoFileExists("scripts", "collect-release-evidence.ps1");
        var verifier = AssertRepoFileExists("scripts", "verify-spire-plus-release-evidence.ps1");
        var evidenceDir = RepoPath(
            ".tools",
            "runtime-evidence",
            "test-release-evidence-gate",
            "clean-only-audit-schema",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(evidenceDir);
        try
        {
            var collectResult = RunPowerShell(collector, "-NoLaunch", "-EvidenceDir", evidenceDir);
            Assert.True(collectResult.ExitCode == 0, $"collect-release-evidence.ps1 -NoLaunch failed:{Environment.NewLine}{collectResult.Output}");

            var manifestPath = Path.Combine(evidenceDir, "release-evidence-manifest.json");
            var manifestNode = JsonNode.Parse(File.ReadAllText(manifestPath))!.AsObject();
            foreach (var rowNode in manifestNode["Rows"]!.AsArray())
            {
                var rowObject = rowNode!.AsObject();
                rowObject["Status"] = "deferred";
                rowObject["ExplicitOwnerDecision"] = true;
                rowObject["ReleaseNote"] = "Synthetic deferral for clean-only audit verifier contract test.";
            }

            var loaderNode = manifestNode["Rows"]!
                .AsArray()
                .Select(row => row!.AsObject())
                .Single(row => row["Id"]!.GetValue<string>() == "fresh-current-package-loader-smoke");
            loaderNode["Status"] = "pass";
            loaderNode["ResultNote"] = "Synthetic loader pass attempt with clean-only audit JSON.";
            loaderNode["ExplicitOwnerDecision"] = false;
            loaderNode["ReleaseNote"] = "";

            var loaderEvidenceDir = loaderNode["EvidenceDir"]!.GetValue<string>();
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "godot.log"), "Synthetic loader log for clean-only audit verifier contract.");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "godot-log-audit.json"), """{ "Clean": true }""");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "enabled-mods.txt"), "STS2-RitsuLib\r\nEZMicroBalance\r\n");
            File.WriteAllText(
                manifestPath,
                manifestNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var verifyResult = RunPowerShell(
                verifier,
                "-EvidenceRoot",
                evidenceDir,
                "-ManifestPath",
                manifestPath,
                "-AllowDeferred");

            Assert.NotEqual(0, verifyResult.ExitCode);
            Assert.Contains("log audit is not clean", verifyResult.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(evidenceDir))
            {
                Directory.Delete(evidenceDir, recursive: true);
            }
        }
    }

    [Fact]
    public void ReleaseVerifierRejectsMarkerOnlyRuntimeBaselineEvidenceRows()
    {
        var collector = AssertRepoFileExists("scripts", "collect-release-evidence.ps1");
        var verifier = AssertRepoFileExists("scripts", "verify-spire-plus-release-evidence.ps1");
        var evidenceDir = RepoPath(
            ".tools",
            "runtime-evidence",
            "test-release-evidence-gate",
            "marker-only-runtime-baseline",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(evidenceDir);
        try
        {
            var collectResult = RunPowerShell(collector, "-NoLaunch", "-EvidenceDir", evidenceDir);
            Assert.True(collectResult.ExitCode == 0, $"collect-release-evidence.ps1 -NoLaunch failed:{Environment.NewLine}{collectResult.Output}");

            var manifestPath = Path.Combine(evidenceDir, "release-evidence-manifest.json");
            var manifestNode = JsonNode.Parse(File.ReadAllText(manifestPath))!.AsObject();
            foreach (var rowNode in manifestNode["Rows"]!.AsArray())
            {
                var rowObject = rowNode!.AsObject();
                rowObject["Status"] = "deferred";
                rowObject["ExplicitOwnerDecision"] = true;
                rowObject["ReleaseNote"] = "Synthetic deferral for marker-only runtime baseline verifier contract test.";
            }

            var loaderNode = manifestNode["Rows"]!
                .AsArray()
                .Select(row => row!.AsObject())
                .Single(row => row["Id"]!.GetValue<string>() == "fresh-current-package-loader-smoke");
            loaderNode["Status"] = "pass";
            loaderNode["EvidenceBoundary"] = "runtime-baseline-marker-only";
            loaderNode["LogOriginProofStatus"] = "marker-only-origin-not-proven-by-offline-checker";
            loaderNode["ResultNote"] = "Synthetic loader pass attempt with baseline log markers only.";
            loaderNode["ExplicitOwnerDecision"] = false;
            loaderNode["ReleaseNote"] = "";

            var loaderEvidenceDir = loaderNode["EvidenceDir"]!.GetValue<string>();
            var loaderLogPath = Path.Combine(loaderEvidenceDir, "godot.log");
            File.WriteAllText(loaderLogPath, "Synthetic loader log for marker-only runtime baseline verifier contract.");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "godot-log-audit.json"), CleanGodotLogAuditJson(loaderLogPath));
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "enabled-mods.txt"), "STS2-RitsuLib\r\nEZMicroBalance\r\n");
            File.WriteAllText(
                Path.Combine(loaderEvidenceDir, "run-manifest.json"),
                """
                {
                  "SchemaVersion": 1,
                  "Status": "runtime-baseline-log-markers-checked",
                  "OwnerRunRequired": true,
                  "DoesNotLaunchGame": true,
                  "LaunchMethod": "owner-steam-launch-required",
                  "RuntimeLogStatus": "provided",
                  "RuntimeAuditStatus": "checked-clean",
                  "BaselineLogCheckStatus": "pass",
                  "LogOriginProofStatus": "marker-only-origin-not-proven-by-offline-checker",
                  "ForbiddenClaims": ["release-ready", "live-gameplay-ready", "save-load-ready", "co-op-ready"]
                }
                """);
            File.WriteAllText(
                manifestPath,
                manifestNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var verifyResult = RunPowerShell(
                verifier,
                "-EvidenceRoot",
                evidenceDir,
                "-ManifestPath",
                manifestPath,
                "-AllowDeferred");

            Assert.NotEqual(0, verifyResult.ExitCode);
            Assert.Contains("uses marker-only runtime baseline evidence", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains("row.EvidenceBoundary=runtime-baseline-marker-only", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains("run-manifest.json Status=runtime-baseline-log-markers-checked", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains("run-manifest.json LogOriginProofStatus=marker-only-origin-not-proven-by-offline-checker", verifyResult.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(evidenceDir))
            {
                Directory.Delete(evidenceDir, recursive: true);
            }
        }
    }

    [Fact]
    public void ReleaseVerifierRejectsRuntimeBaselineSentinelFilesWithoutManifest()
    {
        var collector = AssertRepoFileExists("scripts", "collect-release-evidence.ps1");
        var verifier = AssertRepoFileExists("scripts", "verify-spire-plus-release-evidence.ps1");
        var evidenceDir = RepoPath(
            ".tools",
            "runtime-evidence",
            "test-release-evidence-gate",
            "runtime-baseline-sentinel-without-manifest",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(evidenceDir);
        try
        {
            var collectResult = RunPowerShell(collector, "-NoLaunch", "-EvidenceDir", evidenceDir);
            Assert.True(collectResult.ExitCode == 0, $"collect-release-evidence.ps1 -NoLaunch failed:{Environment.NewLine}{collectResult.Output}");

            var manifestPath = Path.Combine(evidenceDir, "release-evidence-manifest.json");
            var manifestNode = JsonNode.Parse(File.ReadAllText(manifestPath))!.AsObject();
            foreach (var rowNode in manifestNode["Rows"]!.AsArray())
            {
                var rowObject = rowNode!.AsObject();
                rowObject["Status"] = "deferred";
                rowObject["ExplicitOwnerDecision"] = true;
                rowObject["ReleaseNote"] = "Synthetic deferral for runtime baseline sentinel verifier contract test.";
            }

            var loaderNode = manifestNode["Rows"]!
                .AsArray()
                .Select(row => row!.AsObject())
                .Single(row => row["Id"]!.GetValue<string>() == "fresh-current-package-loader-smoke");
            loaderNode["Status"] = "pass";
            loaderNode["EvidenceBoundary"] = "live-release-row-required";
            loaderNode["LogOriginProofStatus"] = "";
            loaderNode["ResultNote"] = "Synthetic loader pass attempt with scaffold sentinel files.";
            loaderNode["ExplicitOwnerDecision"] = false;
            loaderNode["ReleaseNote"] = "";

            var loaderEvidenceDir = loaderNode["EvidenceDir"]!.GetValue<string>();
            var loaderLogPath = Path.Combine(loaderEvidenceDir, "godot.log");
            File.WriteAllText(loaderLogPath, "Synthetic loader log for runtime baseline sentinel verifier contract.");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "godot-log-audit.json"), CleanGodotLogAuditJson(loaderLogPath));
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "enabled-mods.txt"), "STS2-RitsuLib\r\nEZMicroBalance\r\n");
            File.Delete(Path.Combine(loaderEvidenceDir, "run-manifest.json"));
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "runtime-baseline-log-check.json"), """{ "ChecksPassed": true }""");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "runtime-baseline-notes.md"), "Synthetic copied baseline notes.");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "godot.log.after-launch"), "Synthetic baseline after-launch log.");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "preflight.json"), """{ "NoLaunch": true }""");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "main-menu-screenshot.png"), "not a real png");
            File.WriteAllText(
                manifestPath,
                manifestNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var verifyResult = RunPowerShell(
                verifier,
                "-EvidenceRoot",
                evidenceDir,
                "-ManifestPath",
                manifestPath,
                "-AllowDeferred");

            Assert.NotEqual(0, verifyResult.ExitCode);
            Assert.Contains("uses marker-only runtime baseline evidence", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains("baseline scaffold sentinel file present: runtime-baseline-log-check.json", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains("baseline scaffold sentinel file present: runtime-baseline-notes.md", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains("baseline scaffold sentinel file present: godot.log.after-launch", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains("baseline scaffold companion file present: main-menu-screenshot.png", verifyResult.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(evidenceDir))
            {
                Directory.Delete(evidenceDir, recursive: true);
            }
        }
    }

    [Fact]
    public void ReleaseVerifierRejectsRenamedRuntimeBaselineLogWithoutLiveOriginProof()
    {
        var collector = AssertRepoFileExists("scripts", "collect-release-evidence.ps1");
        var verifier = AssertRepoFileExists("scripts", "verify-spire-plus-release-evidence.ps1");
        var evidenceDir = RepoPath(
            ".tools",
            "runtime-evidence",
            "test-release-evidence-gate",
            "renamed-runtime-baseline-log",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(evidenceDir);
        try
        {
            var collectResult = RunPowerShell(collector, "-NoLaunch", "-EvidenceDir", evidenceDir);
            Assert.True(collectResult.ExitCode == 0, $"collect-release-evidence.ps1 -NoLaunch failed:{Environment.NewLine}{collectResult.Output}");

            var manifestPath = Path.Combine(evidenceDir, "release-evidence-manifest.json");
            var manifestNode = JsonNode.Parse(File.ReadAllText(manifestPath))!.AsObject();
            foreach (var rowNode in manifestNode["Rows"]!.AsArray())
            {
                var rowObject = rowNode!.AsObject();
                rowObject["Status"] = "deferred";
                rowObject["ExplicitOwnerDecision"] = true;
                rowObject["ReleaseNote"] = "Synthetic deferral for renamed runtime baseline log verifier contract test.";
            }

            var loaderNode = manifestNode["Rows"]!
                .AsArray()
                .Select(row => row!.AsObject())
                .Single(row => row["Id"]!.GetValue<string>() == "fresh-current-package-loader-smoke");
            loaderNode["Status"] = "pass";
            loaderNode["EvidenceBoundary"] = "live-release-row-required";
            loaderNode["LogOriginProofStatus"] = OwnerLiveReleaseLogOrigin;
            loaderNode["ResultNote"] = "Synthetic loader pass attempt using a copied after-launch log under the release log filename.";
            loaderNode["ExplicitOwnerDecision"] = false;
            loaderNode["ReleaseNote"] = "";

            var loaderEvidenceDir = loaderNode["EvidenceDir"]!.GetValue<string>();
            var loaderLogPath = Path.Combine(loaderEvidenceDir, "godot.log");
            File.WriteAllText(
                loaderLogPath,
                "Synthetic renamed beta.135 after-launch log content with clean audit shape.");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "godot-log-audit.json"), CleanGodotLogAuditJson(loaderLogPath));
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "enabled-mods.txt"), "STS2-RitsuLib\r\nEZMicroBalance\r\n");
            File.WriteAllText(
                Path.Combine(loaderEvidenceDir, "log-origin-note.md"),
                "LogOriginProofStatus: owner-live-release-log\r\nSource: TBD\r\nLog files: godot.log\r\n");
            File.Delete(Path.Combine(loaderEvidenceDir, "run-manifest.json"));
            File.WriteAllText(
                manifestPath,
                manifestNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var verifyResult = RunPowerShell(
                verifier,
                "-EvidenceRoot",
                evidenceDir,
                "-ManifestPath",
                manifestPath,
                "-AllowDeferred");

            Assert.NotEqual(0, verifyResult.ExitCode);
            Assert.Contains("log-origin-note.md Source line must not be a placeholder value", verifyResult.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("uses marker-only runtime baseline evidence", verifyResult.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(evidenceDir))
            {
                Directory.Delete(evidenceDir, recursive: true);
            }
        }
    }

    [Fact]
    public void ReleaseVerifierRejectsCopiedRuntimeBaselineOwnerRunRowFields()
    {
        var collector = AssertRepoFileExists("scripts", "collect-release-evidence.ps1");
        var verifier = AssertRepoFileExists("scripts", "verify-spire-plus-release-evidence.ps1");
        var evidenceDir = RepoPath(
            ".tools",
            "runtime-evidence",
            "test-release-evidence-gate",
            "runtime-baseline-row-fields",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(evidenceDir);
        try
        {
            var collectResult = RunPowerShell(collector, "-NoLaunch", "-EvidenceDir", evidenceDir);
            Assert.True(collectResult.ExitCode == 0, $"collect-release-evidence.ps1 -NoLaunch failed:{Environment.NewLine}{collectResult.Output}");

            var manifestPath = Path.Combine(evidenceDir, "release-evidence-manifest.json");
            var manifestNode = JsonNode.Parse(File.ReadAllText(manifestPath))!.AsObject();
            foreach (var rowNode in manifestNode["Rows"]!.AsArray())
            {
                var rowObject = rowNode!.AsObject();
                rowObject["Status"] = "deferred";
                rowObject["ExplicitOwnerDecision"] = true;
                rowObject["ReleaseNote"] = "Synthetic deferral for copied runtime baseline row-field verifier contract test.";
            }

            var loaderNode = manifestNode["Rows"]!
                .AsArray()
                .Select(row => row!.AsObject())
                .Single(row => row["Id"]!.GetValue<string>() == "fresh-current-package-loader-smoke");
            loaderNode["Status"] = "pass";
            loaderNode["EvidenceBoundary"] = "live-release-row-required";
            loaderNode["OwnerRunRequired"] = true;
            loaderNode["DoesNotLaunchGame"] = true;
            loaderNode["LaunchMethod"] = "owner-steam-launch-required";
            loaderNode["ResultNote"] = "Synthetic loader pass attempt with copied no-launch owner-run row fields.";
            loaderNode["ExplicitOwnerDecision"] = false;
            loaderNode["ReleaseNote"] = "";

            File.WriteAllText(
                manifestPath,
                manifestNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var verifyResult = RunPowerShell(
                verifier,
                "-EvidenceRoot",
                evidenceDir,
                "-ManifestPath",
                manifestPath,
                "-AllowDeferred");

            Assert.NotEqual(0, verifyResult.ExitCode);
            Assert.Contains("uses marker-only runtime baseline evidence", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains("row is a no-launch owner-run runtime baseline scaffold", verifyResult.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(evidenceDir))
            {
                Directory.Delete(evidenceDir, recursive: true);
            }
        }
    }

    [Fact]
    public void ReleaseVerifierRejectsStrippedRuntimeBaselineManifestRecords()
    {
        var collector = AssertRepoFileExists("scripts", "collect-release-evidence.ps1");
        var verifier = AssertRepoFileExists("scripts", "verify-spire-plus-release-evidence.ps1");
        var evidenceDir = RepoPath(
            ".tools",
            "runtime-evidence",
            "test-release-evidence-gate",
            "runtime-baseline-stripped-manifest",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(evidenceDir);
        try
        {
            var collectResult = RunPowerShell(collector, "-NoLaunch", "-EvidenceDir", evidenceDir);
            Assert.True(collectResult.ExitCode == 0, $"collect-release-evidence.ps1 -NoLaunch failed:{Environment.NewLine}{collectResult.Output}");

            var manifestPath = Path.Combine(evidenceDir, "release-evidence-manifest.json");
            var manifestNode = JsonNode.Parse(File.ReadAllText(manifestPath))!.AsObject();
            foreach (var rowNode in manifestNode["Rows"]!.AsArray())
            {
                var rowObject = rowNode!.AsObject();
                rowObject["Status"] = "deferred";
                rowObject["ExplicitOwnerDecision"] = true;
                rowObject["ReleaseNote"] = "Synthetic deferral for stripped runtime manifest verifier contract test.";
            }

            var loaderNode = manifestNode["Rows"]!
                .AsArray()
                .Select(row => row!.AsObject())
                .Single(row => row["Id"]!.GetValue<string>() == "fresh-current-package-loader-smoke");
            loaderNode["Status"] = "pass";
            loaderNode["EvidenceBoundary"] = "live-release-row-required";
            loaderNode["ResultNote"] = "Synthetic loader pass attempt with stripped copied manifest records.";
            loaderNode["ExplicitOwnerDecision"] = false;
            loaderNode["ReleaseNote"] = "";

            var loaderEvidenceDir = loaderNode["EvidenceDir"]!.GetValue<string>();
            File.WriteAllText(
                Path.Combine(loaderEvidenceDir, "run-manifest.json"),
                """
                {
                  "SchemaVersion": 1,
                  "GodotLogAfterLaunchRecord": {
                    "Path": "D:\\runtime\\godot.log.after-launch",
                    "Length": 42,
                    "Sha256": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
                  },
                  "RuntimeBaselineLogCheckRecord": {
                    "Path": "runtime-baseline-log-check.json",
                    "Length": 42,
                    "Sha256": "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"
                  },
                  "StartupMainMenuScreenshotStatus": "provided"
                }
                """);
            File.WriteAllText(
                manifestPath,
                manifestNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var verifyResult = RunPowerShell(
                verifier,
                "-EvidenceRoot",
                evidenceDir,
                "-ManifestPath",
                manifestPath,
                "-AllowDeferred");

            Assert.NotEqual(0, verifyResult.ExitCode);
            Assert.Contains("uses marker-only runtime baseline evidence", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains("run-manifest.json has no release-evidence provenance", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains("run-manifest.json contains beta.135 runtime baseline field: GodotLogAfterLaunchRecord", verifyResult.Output, StringComparison.Ordinal);
            Assert.Contains("run-manifest.json contains beta.135 runtime baseline-specific keys or values", verifyResult.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(evidenceDir))
            {
                Directory.Delete(evidenceDir, recursive: true);
            }
        }
    }

    [Fact]
    public void ReleaseVerifierRejectsDirtyGodotLogWithForgedCleanAudit()
    {
        var collector = AssertRepoFileExists("scripts", "collect-release-evidence.ps1");
        var verifier = AssertRepoFileExists("scripts", "verify-spire-plus-release-evidence.ps1");
        var evidenceDir = RepoPath(
            ".tools",
            "runtime-evidence",
            "test-release-evidence-gate",
            "forged-clean-audit",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(evidenceDir);
        try
        {
            var collectResult = RunPowerShell(collector, "-NoLaunch", "-EvidenceDir", evidenceDir);
            Assert.True(collectResult.ExitCode == 0, $"collect-release-evidence.ps1 -NoLaunch failed:{Environment.NewLine}{collectResult.Output}");

            var manifestPath = Path.Combine(evidenceDir, "release-evidence-manifest.json");
            var manifestNode = JsonNode.Parse(File.ReadAllText(manifestPath))!.AsObject();
            foreach (var rowNode in manifestNode["Rows"]!.AsArray())
            {
                var rowObject = rowNode!.AsObject();
                rowObject["Status"] = "deferred";
                rowObject["ExplicitOwnerDecision"] = true;
                rowObject["ReleaseNote"] = "Synthetic deferral for forged clean-audit verifier contract test.";
            }

            var loaderNode = manifestNode["Rows"]!
                .AsArray()
                .Select(row => row!.AsObject())
                .Single(row => row["Id"]!.GetValue<string>() == "fresh-current-package-loader-smoke");
            loaderNode["Status"] = "pass";
            loaderNode["ResultNote"] = "Synthetic loader pass attempt with forged clean audit.";
            loaderNode["ExplicitOwnerDecision"] = false;
            loaderNode["ReleaseNote"] = "";

            var loaderEvidenceDir = loaderNode["EvidenceDir"]!.GetValue<string>();
            var loaderLogPath = Path.Combine(loaderEvidenceDir, "godot.log");
            File.WriteAllText(
                loaderLogPath,
                "[ERROR] [EZMicroBalance] Spire Plus synthetic release verifier dirty-log contract.");
            var forgedAudit = JsonNode.Parse(CleanGodotLogAuditJson(loaderLogPath))!.AsArray();
            var forgedItem = forgedAudit[0]!.AsObject();
            forgedItem["Clean"] = true;
            foreach (var hitNode in forgedItem["SignatureHits"]!.AsArray())
            {
                hitNode!.AsObject()["Count"] = 0;
            }

            File.WriteAllText(
                Path.Combine(loaderEvidenceDir, "godot-log-audit.json"),
                forgedAudit.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "enabled-mods.txt"), "STS2-RitsuLib\r\nEZMicroBalance\r\n");
            File.WriteAllText(
                manifestPath,
                manifestNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var verifyResult = RunPowerShell(
                verifier,
                "-EvidenceRoot",
                evidenceDir,
                "-ManifestPath",
                manifestPath,
                "-AllowDeferred");

            Assert.NotEqual(0, verifyResult.ExitCode);
            Assert.Contains("log audit is not clean", verifyResult.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(evidenceDir))
            {
                Directory.Delete(evidenceDir, recursive: true);
            }
        }
    }

    private static (string ManifestPath, string LoaderEvidenceDir) WriteSyntheticLoaderPassEvidence(
        string evidenceDir,
        string resultNote)
    {
        var collector = AssertRepoFileExists("scripts", "collect-release-evidence.ps1");
        var collectResult = RunPowerShell(collector, "-NoLaunch", "-EvidenceDir", evidenceDir);
        Assert.True(collectResult.ExitCode == 0, $"collect-release-evidence.ps1 -NoLaunch failed:{Environment.NewLine}{collectResult.Output}");

        var manifestPath = Path.Combine(evidenceDir, "release-evidence-manifest.json");
        var manifestNode = JsonNode.Parse(File.ReadAllText(manifestPath))!.AsObject();
        foreach (var rowNode in manifestNode["Rows"]!.AsArray())
        {
            var rowObject = rowNode!.AsObject();
            rowObject["Status"] = "deferred";
            rowObject["ExplicitOwnerDecision"] = true;
            rowObject["ReleaseNote"] = "Synthetic deferral for release verifier package-hash contract test.";
        }

        var loaderNode = manifestNode["Rows"]!
            .AsArray()
            .Select(row => row!.AsObject())
            .Single(row => row["Id"]!.GetValue<string>() == "fresh-current-package-loader-smoke");
        loaderNode["Status"] = "pass";
        loaderNode["ResultNote"] = resultNote;
        loaderNode["ExplicitOwnerDecision"] = false;
        loaderNode["ReleaseNote"] = "";

        var loaderEvidenceDir = loaderNode["EvidenceDir"]!.GetValue<string>();
        var loaderLogPath = Path.Combine(loaderEvidenceDir, "godot.log");
        File.WriteAllText(loaderLogPath, "Synthetic loader log for package-hash verifier contract.");
        File.WriteAllText(Path.Combine(loaderEvidenceDir, "godot-log-audit.json"), CleanGodotLogAuditJson(loaderLogPath));
        File.WriteAllText(Path.Combine(loaderEvidenceDir, "enabled-mods.txt"), "STS2-RitsuLib\r\nEZMicroBalance\r\n");
        WriteOwnerLiveLogOrigin(loaderNode);

        File.WriteAllText(
            manifestPath,
            manifestNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        return (manifestPath, loaderEvidenceDir);
    }

}
