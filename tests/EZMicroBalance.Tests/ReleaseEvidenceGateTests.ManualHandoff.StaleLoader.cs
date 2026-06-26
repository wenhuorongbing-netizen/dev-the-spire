using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseEvidenceGateTests
{
    [Fact]
    public void CurrentManualTestHandoffArchivesStaleLoaderFilesWhenHashCannotBePreserved()
    {
        var script = AssertRepoFileExists("scripts", "prepare-current-manual-test-handoff.ps1");
        var evidenceDir = RepoPath(
            ".tools",
            "runtime-evidence",
            "test-release-evidence-gate",
            "prepare-current-manual-test-handoff-stale-loader",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(evidenceDir);
        try
        {
            var result = RunPowerShell(script, "-EvidenceRoot", evidenceDir);
            Assert.True(result.ExitCode == 0, $"prepare-current-manual-test-handoff.ps1 failed:{Environment.NewLine}{result.Output}");

            var manifestPath = Path.Combine(evidenceDir, "release", "release-evidence-manifest.json");
            var loaderEvidenceDir = Path.Combine(evidenceDir, "release", "fresh-current-package-loader-smoke");
            var loaderLogPath = Path.Combine(loaderEvidenceDir, "godot.log");
            File.WriteAllText(loaderLogPath, "Stale loader log for an older package hash.");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "godot-log-audit.json"), CleanGodotLogAuditJson(loaderLogPath));
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "enabled-mods.txt"), "previous package\r\nSpire Plus\r\n");

            var manifestNode = JsonNode.Parse(File.ReadAllText(manifestPath))!.AsObject();
            var loaderRow = manifestNode["Rows"]!.AsArray()
                .Single(row => (string?)row?["Id"] == "fresh-current-package-loader-smoke")!
                .AsObject();
            loaderRow["Status"] = "pass";
            loaderRow["EvidenceDir"] = loaderEvidenceDir;
            loaderRow["ResultNote"] = "Synthetic stale loader row that should not survive a package-hash mismatch.";
            File.WriteAllText(manifestPath, manifestNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var packageHashesPath = Path.Combine(loaderEvidenceDir, "package-hashes.json");
            var packageHashes = JsonNode.Parse(File.ReadAllText(packageHashesPath))!.AsObject();
            var packageRow = packageHashes["Files"]!.AsArray()
                .Single(row => (string?)row?["Path"] == CurrentPackageZipRelativePath())!
                .AsObject();
            packageRow["Sha256"] = "STALE_PACKAGE_HASH";
            File.WriteAllText(packageHashesPath, packageHashes.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var rerunResult = RunPowerShell(script, "-EvidenceRoot", evidenceDir);
            Assert.True(rerunResult.ExitCode == 0, $"prepare-current-manual-test-handoff.ps1 failed on stale-loader rerun:{Environment.NewLine}{rerunResult.Output}");

            Assert.False(File.Exists(Path.Combine(loaderEvidenceDir, "godot.log")), "Stale godot.log should be moved out of the pending loader row.");
            Assert.False(File.Exists(Path.Combine(loaderEvidenceDir, "godot-log-audit.json")), "Stale godot-log-audit.json should be moved out of the pending loader row.");
            Assert.False(File.Exists(Path.Combine(loaderEvidenceDir, "enabled-mods.txt")), "Stale enabled-mods.txt should be moved out of the pending loader row.");

            var archiveRoot = Path.Combine(loaderEvidenceDir, ".stale-loader-evidence");
            Assert.True(Directory.Exists(archiveRoot), "Missing stale loader evidence archive.");
            var archiveDir = Assert.Single(Directory.GetDirectories(archiveRoot));
            Assert.True(File.Exists(Path.Combine(archiveDir, "godot.log")), "Archived stale godot.log missing.");
            Assert.True(File.Exists(Path.Combine(archiveDir, "godot-log-audit.json")), "Archived stale godot-log-audit.json missing.");
            Assert.True(File.Exists(Path.Combine(archiveDir, "enabled-mods.txt")), "Archived stale enabled-mods.txt missing.");
            Assert.Contains("historical context only", File.ReadAllText(Path.Combine(archiveDir, "README.md")), StringComparison.Ordinal);

            using var summaryDocument = JsonDocument.Parse(File.ReadAllText(Path.Combine(evidenceDir, "handoff-summary.json")));
            var summary = summaryDocument.RootElement;
            Assert.True(summary.GetProperty("NoLaunch").GetBoolean());
            Assert.Equal(21, summary.GetProperty("PendingVerifierFailureCount").GetInt32());
            Assert.Equal(archiveDir, summary.GetProperty("StaleCurrentLoaderArchive").GetString());

            using var rerunManifestDocument = JsonDocument.Parse(File.ReadAllText(manifestPath));
            var rerunLoaderRow = rerunManifestDocument.RootElement
                .GetProperty("Rows")
                .EnumerateArray()
                .Single(row => row.GetProperty("Id").GetString() == "fresh-current-package-loader-smoke");
            Assert.Equal("pending", rerunLoaderRow.GetProperty("Status").GetString());
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
