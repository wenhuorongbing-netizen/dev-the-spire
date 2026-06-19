using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseEvidenceGateTests
{
    private static string ExpectedManualHandoffModDirectory()
    {
        var configuredPath = Environment.GetEnvironmentVariable("STS2_PATH");
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            return ConvertSts2PathToModDirectory(configuredPath);
        }

        var propsPath = Path.Combine(Root, "Directory.Build.props");
        if (File.Exists(propsPath))
        {
            try
            {
                var props = XDocument.Load(propsPath);
                var sts2Path = props
                    .Descendants("Sts2Path")
                    .Select(element => element.Value)
                    .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
                if (!string.IsNullOrWhiteSpace(sts2Path))
                {
                    return ConvertSts2PathToModDirectory(sts2Path);
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Xml.XmlException)
            {
            }
        }

        foreach (var knownRoot in new[]
        {
            @"E:\Steam\steamapps\common\Slay the Spire 2",
            @"D:\Steam\steamapps\common\Slay the Spire 2"
        })
        {
            if (Directory.Exists(knownRoot))
            {
                return ConvertSts2PathToModDirectory(knownRoot);
            }
        }

        return @"<GameRoot>\mods\EZMicroBalance";
    }

    private static string ConvertSts2PathToModDirectory(string path)
    {
        var fullPath = Path.GetFullPath(path);
        var directoryInfo = new DirectoryInfo(fullPath);
        if (string.Equals(directoryInfo.Name, "EZMicroBalance", StringComparison.Ordinal) &&
            string.Equals(directoryInfo.Parent?.Name, "mods", StringComparison.Ordinal))
        {
            return directoryInfo.FullName;
        }

        return Path.Combine(directoryInfo.FullName, "mods", "EZMicroBalance");
    }

    [Fact]
    public void CurrentManualTestHandoffScriptCreatesAllPendingEvidenceSections()
    {
        var script = AssertRepoFileExists("scripts", "prepare-current-manual-test-handoff.ps1");
        var source = ReadRepoText("scripts", "prepare-current-manual-test-handoff.ps1");
        var scriptsReadme = ReadRepoText("scripts", "README.md");

        AssertSourceContains(
            source,
            "collect-release-evidence.ps1",
            "collect-ancient-ui-evidence.ps1",
            "collect-vakuu-fight-evidence.ps1",
            "collect-preview-tools-evidence.ps1",
            "collect-coop-evidence.ps1",
            "verify-spire-plus-release-evidence.ps1",
            "TESTER_START_HERE.md",
            "Recommended order",
            "Handoff summary",
            "PendingVerifierRequiredRowCount=21",
            "PendingVerifierFailureCount=21",
            "PendingVerifierWarningCount=0",
            "Move-StaleCurrentLoaderEvidence",
            ".stale-loader-evidence",
            "Pending release evidence unexpectedly passed verification",
            "No game was launched. All live rows remain pending.");

        AssertSourceContains(
            scriptsReadme,
            "prepare-current-manual-test-handoff.ps1",
            "release, Ancient UI, Vakuu fight, preview-tools, and co-op evidence templates",
            "handoff-summary.json",
            "PendingVerifierRequiredRowCount",
            "PendingVerifierFailureCount",
            "PendingVerifierWarningCount",
            "It does not launch the game and does not mark rows as passed.");

        var evidenceDir = RepoPath(
            ".tools",
            "runtime-evidence",
            "test-release-evidence-gate",
            "prepare-current-manual-test-handoff",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(evidenceDir);
        try
        {
            var result = RunPowerShell(script, "-EvidenceRoot", evidenceDir);
            Assert.True(result.ExitCode == 0, $"prepare-current-manual-test-handoff.ps1 failed:{Environment.NewLine}{result.Output}");

            foreach (var folder in new[] { "release", "ancient-ui", "vakuu", "preview-tools", "coop" })
            {
                Assert.True(Directory.Exists(Path.Combine(evidenceDir, folder)), $"Missing handoff folder: {folder}");
            }

            foreach (var ancientFolder in new[] { "URDA", "MORVI", "LOTHA", "VAKUU", "VAKUU-FIGHT" })
            {
                var folder = Path.Combine(evidenceDir, "ancient-ui", ancientFolder);
                Assert.True(Directory.Exists(folder), $"Missing Ancient UI handoff folder: {ancientFolder}");
                Assert.True(File.Exists(Path.Combine(folder, "manual-instructions.md")), $"Missing manual instructions for {ancientFolder}.");
                Assert.True(File.Exists(Path.Combine(folder, "manual-rows-template.json")), $"Missing manual row template for {ancientFolder}.");
            }

            var summaryPath = Path.Combine(evidenceDir, "handoff-summary.json");
            var readmePath = Path.Combine(evidenceDir, "README.md");
            var startHerePath = Path.Combine(evidenceDir, "TESTER_START_HERE.md");
            Assert.True(File.Exists(summaryPath), "Missing handoff-summary.json.");
            Assert.True(File.Exists(readmePath), "Missing handoff README.md.");
            Assert.True(File.Exists(startHerePath), "Missing TESTER_START_HERE.md.");

            using var summaryDocument = JsonDocument.Parse(File.ReadAllText(summaryPath));
            var summary = summaryDocument.RootElement;
            Assert.True(summary.GetProperty("NoLaunch").GetBoolean());
            Assert.True(summary.GetProperty("PendingVerifierChecked").GetBoolean());
            Assert.True(summary.GetProperty("PendingVerifierExpectedFailure").GetBoolean());
            Assert.Equal(21, summary.GetProperty("PendingVerifierRequiredRowCount").GetInt32());
            Assert.Equal(21, summary.GetProperty("PendingVerifierFailureCount").GetInt32());
            Assert.Equal(0, summary.GetProperty("PendingVerifierWarningCount").GetInt32());
            Assert.Equal(CurrentPackageZipRelativePath(), summary.GetProperty("PackagePath").GetString());
            Assert.Equal(CurrentPackageZipSha256(), summary.GetProperty("PackageSha256").GetString());
            var expectedEvidenceRootArg = $"-EvidenceRoot '{Path.GetRelativePath(Root, Path.Combine(evidenceDir, "release"))}'";
            var expectedManifestArg = $"-ManifestPath '{Path.GetRelativePath(Root, Path.Combine(evidenceDir, "release", "release-evidence-manifest.json"))}'";
            var verifierCommand = summary.GetProperty("VerifierCommand").GetString();
            Assert.Contains(expectedEvidenceRootArg, verifierCommand, StringComparison.Ordinal);
            Assert.Contains(expectedManifestArg, verifierCommand, StringComparison.Ordinal);

            var startHere = File.ReadAllText(startHerePath);
            var readme = File.ReadAllText(readmePath);
            Assert.Contains("## Package under test", startHere, StringComparison.Ordinal);
            Assert.Contains("Player-facing mod: `Spire Plus`.", startHere, StringComparison.Ordinal);
            Assert.Contains("Install note: enable `Spire Plus` in game. The current compatibility folder inside the package is `EZMicroBalance`.", startHere, StringComparison.Ordinal);
            Assert.Contains($"ZIP: `{CurrentPackageZipRelativePath()}`.", startHere, StringComparison.Ordinal);
            Assert.Contains($"ZIP SHA256: `{CurrentPackageZipSha256()}`.", startHere, StringComparison.Ordinal);
            Assert.Contains("## Handoff summary", startHere, StringComparison.Ordinal);
            Assert.Contains("`handoff-summary.json` records this no-launch scaffold contract.", startHere, StringComparison.Ordinal);
            Assert.Contains("`PendingVerifierRequiredRowCount=21`.", startHere, StringComparison.Ordinal);
            Assert.Contains("`PendingVerifierFailureCount=21`.", startHere, StringComparison.Ordinal);
            Assert.Contains("`PendingVerifierWarningCount=0`.", startHere, StringComparison.Ordinal);
            Assert.Contains("These numbers mean the scaffold is expected to fail until live evidence is filled.", startHere, StringComparison.Ordinal);
            Assert.Contains("Recommended order", startHere, StringComparison.Ordinal);
            Assert.Contains(
                $".\\scripts\\check-installed-spire-plus-package.ps1 -ModDirectory \"{ExpectedManualHandoffModDirectory()}\"",
                startHere,
                StringComparison.Ordinal);
            Assert.DoesNotContain(
                ".\\scripts\\check-installed-spire-plus-package.ps1 -ModDirectory \"D:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\EZMicroBalance\"",
                source,
                StringComparison.Ordinal);
            Assert.Contains("It should fail closed with 21 pending live rows", startHere, StringComparison.Ordinal);
            Assert.Contains("release/fresh-current-package-loader-smoke/", startHere, StringComparison.Ordinal);
            Assert.Contains("release/mod-settings-current-display/", startHere, StringComparison.Ordinal);
            Assert.Contains("current Mods-list and Spire Plus config-page screenshots", startHere, StringComparison.Ordinal);
            Assert.Contains("## Focused current regression check", startHere, StringComparison.Ordinal);
            Assert.Contains("Vakuu event option: pick `Sere Talon`. It must be the Vakuu relic that offers 4 Curses, lets you choose 1, then adds that Curse, 2 Wish, and 1 Wish+.", startHere, StringComparison.Ordinal);
            Assert.Contains("It must not show Tanx Claws relic art, title, or Maul-transform text. If the effect is curse choice plus 2 Wish and 1 Wish+ but the art is still Tanx Claws, treat it as a Spire Plus UI/package-load issue.", startHere, StringComparison.Ordinal);
            Assert.Contains("Capture the event option, relic bar, inspect screen, and hover tooltip for Sere Talon.", startHere, StringComparison.Ordinal);
            Assert.Contains("Sere Talon route lines on `Ancient event option button`", startHere, StringComparison.Ordinal);
            Assert.Contains("`RelicModel packed icon texture`", startHere, StringComparison.Ordinal);
            Assert.Contains("`RelicModel big icon texture`", startHere, StringComparison.Ordinal);
            Assert.Contains("`NRelic small node`", startHere, StringComparison.Ordinal);
            Assert.Contains("`NRelic large node`", startHere, StringComparison.Ordinal);
            Assert.Contains("Tanx Claws should remain the Maul-transform relic and should create upgraded Maul cards.", startHere, StringComparison.Ordinal);
            Assert.Contains("Do not mark rows pass from source review.", startHere, StringComparison.Ordinal);
            Assert.Contains("verify-spire-plus-release-evidence.ps1", startHere, StringComparison.Ordinal);
            Assert.Contains(expectedEvidenceRootArg, startHere, StringComparison.Ordinal);
            Assert.Contains(expectedManifestArg, startHere, StringComparison.Ordinal);
            Assert.Contains("`handoff-summary.json` records `PendingVerifierRequiredRowCount=21`, `PendingVerifierFailureCount=21`, and `PendingVerifierWarningCount=0`.", readme, StringComparison.Ordinal);
            Assert.Contains("Those are expected no-launch values, not live proof.", readme, StringComparison.Ordinal);
            Assert.Contains(expectedEvidenceRootArg, readme, StringComparison.Ordinal);
            Assert.Contains(expectedManifestArg, readme, StringComparison.Ordinal);
            Assert.DoesNotContain("manual-test-handoff-20260523-current", startHere, StringComparison.Ordinal);

            var manifestPath = Path.Combine(evidenceDir, "release", "release-evidence-manifest.json");
            using var manifestDocument = JsonDocument.Parse(File.ReadAllText(manifestPath));
            var rows = manifestDocument.RootElement.GetProperty("Rows").EnumerateArray().ToArray();
            Assert.NotEmpty(rows);
            Assert.All(rows, row => Assert.Equal("pending", row.GetProperty("Status").GetString()));
            Assert.All(
                rows.SelectMany(row => row.GetProperty("RequiredFiles").EnumerateArray()),
                file => Assert.Equal(JsonValueKind.String, file.ValueKind));

            var loaderEvidenceDir = Path.Combine(evidenceDir, "release", "fresh-current-package-loader-smoke");
            Directory.CreateDirectory(loaderEvidenceDir);
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "command.txt"), "Synthetic current-package loader command fixture.");
            File.WriteAllText(
                Path.Combine(loaderEvidenceDir, "godot.log"),
                "BaseLib initialized. Spire Plus initialized. Loaded 2 mods. Registered config for mod EZMicroBalance. Found 30 SavedSpireFields.");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "enabled-mods.txt"), "BaseLib" + Environment.NewLine + "Spire Plus");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "godot-log-audit.json"), "{ \"Clean\": true }");

            var manifestNode = JsonNode.Parse(File.ReadAllText(manifestPath))!.AsObject();
            var manifestRows = manifestNode["Rows"]!.AsArray();
            var loaderRow = manifestRows.Single(row => (string?)row?["Id"] == "fresh-current-package-loader-smoke")!.AsObject();
            loaderRow["Status"] = "pass";
            loaderRow["EvidenceDir"] = loaderEvidenceDir;
            loaderRow["ResultNote"] = "Synthetic current-package loader preservation fixture for generator regression coverage.";
            loaderRow["ReleaseNote"] = "Loader row filled; gameplay rows remain pending.";
            loaderRow["Notes"] = "Preserve this pass row when regenerating the current handoff.";
            File.WriteAllText(manifestPath, manifestNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var preservedResult = RunPowerShell(script, "-EvidenceRoot", evidenceDir);
            Assert.True(preservedResult.ExitCode == 0, $"prepare-current-manual-test-handoff.ps1 failed while preserving loader evidence:{Environment.NewLine}{preservedResult.Output}");

            using var preservedSummaryDocument = JsonDocument.Parse(File.ReadAllText(summaryPath));
            var preservedSummary = preservedSummaryDocument.RootElement;
            Assert.False(preservedSummary.GetProperty("NoLaunch").GetBoolean());
            Assert.Equal(21, preservedSummary.GetProperty("PendingVerifierRequiredRowCount").GetInt32());
            Assert.Equal(20, preservedSummary.GetProperty("PendingVerifierFailureCount").GetInt32());
            Assert.Equal(20, preservedSummary.GetProperty("CurrentVerifierFailureCount").GetInt32());
            Assert.Equal(loaderEvidenceDir, preservedSummary.GetProperty("CurrentLoaderEvidenceDir").GetString());

            using var preservedManifestDocument = JsonDocument.Parse(File.ReadAllText(manifestPath));
            var preservedRows = preservedManifestDocument.RootElement.GetProperty("Rows").EnumerateArray().ToArray();
            var preservedLoaderRow = preservedRows.Single(row => row.GetProperty("Id").GetString() == "fresh-current-package-loader-smoke");
            Assert.Equal("pass", preservedLoaderRow.GetProperty("Status").GetString());
            Assert.Equal(loaderEvidenceDir, preservedLoaderRow.GetProperty("EvidenceDir").GetString());

            var preservedStartHere = File.ReadAllText(startHerePath);
            var preservedReadme = File.ReadAllText(readmePath);
            Assert.Contains("The current-package loader row is filled", preservedStartHere, StringComparison.Ordinal);
            Assert.Contains("It should fail closed with 20 pending live rows", preservedStartHere, StringComparison.Ordinal);
            Assert.Contains("current failure count `20`", preservedReadme, StringComparison.Ordinal);
            Assert.DoesNotContain("It should fail closed with 21 pending live rows", preservedStartHere, StringComparison.Ordinal);
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
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "godot.log"), "Stale loader log for an older package hash.");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "godot-log-audit.json"), """{ "Clean": true }""");
            File.WriteAllText(Path.Combine(loaderEvidenceDir, "enabled-mods.txt"), "BaseLib\r\nSpire Plus\r\n");

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
