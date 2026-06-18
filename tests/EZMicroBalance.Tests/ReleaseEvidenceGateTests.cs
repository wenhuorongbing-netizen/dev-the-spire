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
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "godot.log"), "Synthetic loader log for package-hash verifier contract.");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "godot-log-audit.json"), """{ "Clean": true }""");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "enabled-mods.txt"), "BaseLib\r\nEZMicroBalance\r\n");

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

}
