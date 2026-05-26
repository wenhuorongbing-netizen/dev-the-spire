using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class ReleaseEvidenceGateTests
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

    private static void AssertPackageHashesUseVersionedArtifacts(string packageHashesPath)
    {
        using var packageDocument = JsonDocument.Parse(File.ReadAllText(packageHashesPath));
        var files = packageDocument.RootElement
            .GetProperty("Files")
            .EnumerateArray()
            .ToArray();
        var paths = files
            .Select(file => file.GetProperty("Path").GetString())
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .ToHashSet(StringComparer.Ordinal);

        Assert.Contains(CurrentPackageZipRelativePath(), paths);
        Assert.Contains(CurrentPackageArtifactRelativePath("EZMicroBalance.dll"), paths);
        Assert.Contains(CurrentPackageArtifactRelativePath("EZMicroBalance.pck"), paths);
        Assert.Contains(CurrentPackageArtifactRelativePath("EZMicroBalance.json"), paths);
        Assert.Contains(CurrentPackageArtifactRelativePath("README_INSTALL.txt"), paths);
        Assert.DoesNotContain("publish\\EZMicroBalance.dll", paths);
        Assert.DoesNotContain("publish\\EZMicroBalance.pck", paths);
        Assert.DoesNotContain("publish\\EZMicroBalance.json", paths);

        foreach (var file in files.Where(file => file.GetProperty("Path").GetString()?.StartsWith($"publish\\{CurrentPackageName()}", StringComparison.Ordinal) == true))
        {
            Assert.True(file.GetProperty("Exists").GetBoolean(), $"Package hash row points at a missing package artifact: {file.GetProperty("Path").GetString()}");
        }
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
            "PendingVerifierRequiredRowCount=20",
            "PendingVerifierFailureCount=20",
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
            Assert.Equal(20, summary.GetProperty("PendingVerifierRequiredRowCount").GetInt32());
            Assert.Equal(20, summary.GetProperty("PendingVerifierFailureCount").GetInt32());
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
            Assert.Contains("`PendingVerifierRequiredRowCount=20`.", startHere, StringComparison.Ordinal);
            Assert.Contains("`PendingVerifierFailureCount=20`.", startHere, StringComparison.Ordinal);
            Assert.Contains("`PendingVerifierWarningCount=0`.", startHere, StringComparison.Ordinal);
            Assert.Contains("These numbers mean the scaffold is expected to fail until live evidence is filled.", startHere, StringComparison.Ordinal);
            Assert.Contains("Recommended order", startHere, StringComparison.Ordinal);
            Assert.Contains(".\\scripts\\check-installed-spire-plus-package.ps1 -ModDirectory \"D:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\EZMicroBalance\"", startHere, StringComparison.Ordinal);
            Assert.Contains("It should fail closed with 20 pending live rows", startHere, StringComparison.Ordinal);
            Assert.Contains("release/fresh-current-package-loader-smoke/", startHere, StringComparison.Ordinal);
            Assert.Contains("The Mods list should show `Spire Plus`; `EZMicroBalance` should appear only as the technical folder/id in paths or logs.", startHere, StringComparison.Ordinal);
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
            Assert.Contains("`handoff-summary.json` records `PendingVerifierRequiredRowCount=20`, `PendingVerifierFailureCount=20`, and `PendingVerifierWarningCount=0`.", readme, StringComparison.Ordinal);
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
            Assert.Equal(20, preservedSummary.GetProperty("PendingVerifierRequiredRowCount").GetInt32());
            Assert.Equal(19, preservedSummary.GetProperty("PendingVerifierFailureCount").GetInt32());
            Assert.Equal(19, preservedSummary.GetProperty("CurrentVerifierFailureCount").GetInt32());
            Assert.Equal(loaderEvidenceDir, preservedSummary.GetProperty("CurrentLoaderEvidenceDir").GetString());

            using var preservedManifestDocument = JsonDocument.Parse(File.ReadAllText(manifestPath));
            var preservedRows = preservedManifestDocument.RootElement.GetProperty("Rows").EnumerateArray().ToArray();
            var preservedLoaderRow = preservedRows.Single(row => row.GetProperty("Id").GetString() == "fresh-current-package-loader-smoke");
            Assert.Equal("pass", preservedLoaderRow.GetProperty("Status").GetString());
            Assert.Equal(loaderEvidenceDir, preservedLoaderRow.GetProperty("EvidenceDir").GetString());

            var preservedStartHere = File.ReadAllText(startHerePath);
            var preservedReadme = File.ReadAllText(readmePath);
            Assert.Contains("The current-package loader row is filled", preservedStartHere, StringComparison.Ordinal);
            Assert.Contains("It should fail closed with 19 pending live rows", preservedStartHere, StringComparison.Ordinal);
            Assert.Contains("current failure count `19`", preservedReadme, StringComparison.Ordinal);
            Assert.DoesNotContain("It should fail closed with 20 pending live rows", preservedStartHere, StringComparison.Ordinal);
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
            Assert.Equal(20, summary.GetProperty("PendingVerifierFailureCount").GetInt32());
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

    [Fact]
    public void EvidenceCollectionScriptsCreatePendingNoLaunchTemplates()
    {
        foreach (var scriptName in new[]
        {
            "collect-release-evidence.ps1",
            "collect-preview-tools-evidence.ps1",
            "collect-vakuu-fight-evidence.ps1",
            "collect-coop-evidence.ps1"
        })
        {
            var script = AssertRepoFileExists("scripts", scriptName);
            var source = ReadRepoText("scripts", scriptName);

            Assert.Contains("[switch]$NoLaunch", source, StringComparison.Ordinal);
            Assert.Contains(".tools\\runtime-evidence", source, StringComparison.Ordinal);
            Assert.Contains("command.txt", source, StringComparison.Ordinal);
            Assert.Contains("environment.json", source, StringComparison.Ordinal);
            Assert.Contains("package-hashes.json", source, StringComparison.Ordinal);
            Assert.Contains("manual-rows-template.json", source, StringComparison.Ordinal);
            Assert.DoesNotContain("EZFuturePeekCode", source, StringComparison.Ordinal);
            Assert.DoesNotContain("EZFuturePeek.json", source, StringComparison.Ordinal);
            Assert.DoesNotContain("EZFuturePeek.sln", source, StringComparison.Ordinal);

            var evidenceDir = RepoPath(
                ".tools",
                "runtime-evidence",
                "test-release-evidence-gate",
                Path.GetFileNameWithoutExtension(scriptName),
                Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(evidenceDir);
            try
            {
                var result = RunPowerShell(script, "-NoLaunch", "-EvidenceDir", evidenceDir);
                Assert.True(result.ExitCode == 0, $"{scriptName} -NoLaunch failed:{Environment.NewLine}{result.Output}");

                Assert.True(File.Exists(Path.Combine(evidenceDir, "command.txt")), $"{scriptName} did not write command.txt.");
                Assert.True(File.Exists(Path.Combine(evidenceDir, "environment.json")), $"{scriptName} did not write environment.json.");
                var packageHashesPath = Path.Combine(evidenceDir, "package-hashes.json");
                Assert.True(File.Exists(packageHashesPath), $"{scriptName} did not write package-hashes.json.");
                AssertPackageHashesUseVersionedArtifacts(packageHashesPath);
                Assert.True(File.Exists(Path.Combine(evidenceDir, "manual-rows-template.json")), $"{scriptName} did not write manual-rows-template.json.");

                using var rowsDocument = JsonDocument.Parse(File.ReadAllText(Path.Combine(evidenceDir, "manual-rows-template.json")));
                var rows = rowsDocument.RootElement.GetProperty("Rows").EnumerateArray().ToArray();
                Assert.NotEmpty(rows);
                Assert.All(rows, row =>
                {
                    var status = row.GetProperty("Status").GetString();
                    Assert.Equal("pending", status);
                    Assert.NotEqual("passed", status, StringComparer.OrdinalIgnoreCase);
                    Assert.NotEqual("pass", status, StringComparer.OrdinalIgnoreCase);
                });

                if (scriptName == "collect-release-evidence.ps1")
                {
                    var manifestPath = Path.Combine(evidenceDir, "release-evidence-manifest.json");
                    var readmePath = Path.Combine(evidenceDir, "README.md");
                    Assert.True(File.Exists(manifestPath), "collect-release-evidence.ps1 did not write release-evidence-manifest.json.");
                    Assert.True(File.Exists(readmePath), "collect-release-evidence.ps1 did not write README.md.");

                    var rowIds = rows
                        .Select(row => row.GetProperty("Id").GetString())
                        .ToHashSet(StringComparer.Ordinal);
                    var expectedRowIds = new[]
                    {
                        "fresh-current-package-loader-smoke",
                        "ancient-ui-urda",
                        "ancient-ui-morvi",
                        "ancient-ui-lotha",
                        "ancient-ui-vakuu-normal",
                        "ancient-ui-vakuu-fight",
                        "ancient-reward-visible-relics",
                        "player-text-tooltip-readability",
                        "art-resource-routing-live-preview",
                        "vakuu-victory-no-black-screen",
                        "vakuu-failure-death-path",
                        "vakuu-active-fight-save-load",
                        "ancient-state-save-load",
                        "rootblight-visual-behavior",
                        "a11-natural-route-traversal",
                        "ascension-selector-localization",
                        "a19-a20-dedicated-boss-abilities",
                        "disable-mod-gameplay",
                        "preview-tools-live-proof",
                        "coop-disposition"
                    };

                    foreach (var expectedId in expectedRowIds)
                    {
                        Assert.Contains(expectedId, rowIds);
                    }

                    Assert.DoesNotContain("clicked-ancient-ui-urda-morvi-lotha-vakuu", rowIds);
                    Assert.All(rows, row => Assert.True(row.TryGetProperty("Kind", out _), "Release evidence rows must mirror verifier row Kind values."));

                    foreach (var row in rows)
                    {
                        var rowId = row.GetProperty("Id").GetString()!;
                        var rowEvidenceDir = row.GetProperty("EvidenceDir").GetString()!;
                        Assert.Equal(Path.Combine(evidenceDir, rowId), rowEvidenceDir);
                        Assert.True(Directory.Exists(rowEvidenceDir), $"Missing per-row evidence directory for {rowId}.");
                        Assert.True(File.Exists(Path.Combine(rowEvidenceDir, "README.md")), $"Missing per-row README.md for {rowId}.");
                        Assert.True(File.Exists(Path.Combine(rowEvidenceDir, "command.txt")), $"Missing per-row command.txt for {rowId}.");

                        var rowReadme = File.ReadAllText(Path.Combine(rowEvidenceDir, "README.md"));
                        Assert.Contains($"# {rowId}", rowReadme, StringComparison.Ordinal);
                        Assert.Contains("Required files for pass status:", rowReadme, StringComparison.Ordinal);
                    }

                    var loaderDir = Path.Combine(evidenceDir, "fresh-current-package-loader-smoke");
                    Assert.True(File.Exists(Path.Combine(loaderDir, "environment.json")), "Loader row did not get environment.json.");
                    Assert.True(File.Exists(Path.Combine(loaderDir, "package-hashes.json")), "Loader row did not get package-hashes.json.");
                    Assert.True(File.Exists(Path.Combine(loaderDir, "enabled-mods-template.txt")), "Loader row did not get enabled-mods-template.txt.");

                    var previewToolsDir = Path.Combine(evidenceDir, "preview-tools-live-proof");
                    Assert.True(File.Exists(Path.Combine(previewToolsDir, "environment.json")), "Preview-tools row did not get environment.json.");
                    Assert.True(File.Exists(Path.Combine(previewToolsDir, "package-hashes.json")), "Preview-tools row did not get package-hashes.json.");

                    var ancientRewardRow = rows.Single(row => row.GetProperty("Id").GetString() == "ancient-reward-visible-relics");
                    var ancientRewardFiles = ancientRewardRow
                        .GetProperty("RequiredFiles")
                        .EnumerateArray()
                        .Select(file => file.GetString())
                        .ToArray();
                    Assert.Contains("ancient-reward-relics-checklist.md", ancientRewardFiles);

                    var ancientRewardDir = Path.Combine(evidenceDir, "ancient-reward-visible-relics");
                    Assert.True(
                        File.Exists(Path.Combine(ancientRewardDir, "ancient-reward-relics-checklist-template.md")),
                        "Ancient reward row did not get a visible relic checklist template.");
                    var ancientRewardReadme = File.ReadAllText(Path.Combine(ancientRewardDir, "README.md"));
                    var ancientRewardChecklist = File.ReadAllText(Path.Combine(ancientRewardDir, "ancient-reward-relics-checklist-template.md"));
                    AssertTemplateChecklistCreated(ancientRewardChecklist, "ancient-reward-relics-checklist.md");
                    var ancientRewardWorkingChecklist = AssertWorkingChecklistCreated(
                        ancientRewardDir,
                        "ancient-reward-relics-checklist.md",
                        ["UrdaSeedBankOptionRelic", "MorviBlueprintProofOptionRelic", "LothaDeathReprieveOptionRelic", "VakuuFightOptionRelic"]);
                    Assert.Contains("Manual checkpoints:", ancientRewardReadme, StringComparison.Ordinal);
                    Assert.Contains("Every Urda, Morvi, and Lotha initial reward option is visible as an option relic", ancientRewardReadme, StringComparison.Ordinal);
                    Assert.Contains("UrdaSeedBankOptionRelic", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("MorviBlueprintProofOptionRelic", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("LothaDeathReprieveOptionRelic", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("VakuuFightOptionRelic", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("sere_talon_pickup", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("claws_maul_transform", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("Vakuu's Sere Talon / \u74e6\u5e93\u539f\u521d\u4e4b\u722a", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("Tanx Claws / \u5766\u514b\u65af\u5229\u722a", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("Maul / \u6495\u54ac", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("verify event-option art, relic-bar art, inspect art, hover text, and surface-specific log routes", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("`Ancient event option button`", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("`RelicModel packed icon texture`", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("`RelicModel big icon texture`", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("upgraded Maul", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.DoesNotContain("\u95bb", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.DoesNotContain("\u95b8", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.DoesNotContain("\u95b9", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("Fill this checklist with live results before marking this row pass.", ancientRewardWorkingChecklist, StringComparison.Ordinal);

                    var playerTextRow = rows.Single(row => row.GetProperty("Id").GetString() == "player-text-tooltip-readability");
                    var playerTextFiles = playerTextRow
                        .GetProperty("RequiredFiles")
                        .EnumerateArray()
                        .Select(file => file.GetString())
                        .ToArray();
                    Assert.Contains("player-text-qa-checklist.md", playerTextFiles);

                    var playerTextDir = Path.Combine(evidenceDir, "player-text-tooltip-readability");
                    Assert.True(
                        File.Exists(Path.Combine(playerTextDir, "player-text-qa-checklist-template.md")),
                        "Player text row did not get a QA checklist template.");
                    var playerTextReadme = File.ReadAllText(Path.Combine(playerTextDir, "README.md"));
                    var playerTextChecklist = File.ReadAllText(Path.Combine(playerTextDir, "player-text-qa-checklist-template.md"));
                    AssertTemplateChecklistCreated(playerTextChecklist, "player-text-qa-checklist.md");
                    var playerTextWorkingChecklist = AssertWorkingChecklistCreated(
                        playerTextDir,
                        "player-text-qa-checklist.md",
                        ["ascension-a11-a20", "ancient-choice-text", "preview-tools-text", "en-zhs-key-parity"]);
                    Assert.Contains("Manual checkpoints:", playerTextReadme, StringComparison.Ordinal);
                    Assert.Contains("Check English and Simplified Chinese text separately", playerTextReadme, StringComparison.Ordinal);
                    Assert.Contains("ascension-a11-a20", playerTextChecklist, StringComparison.Ordinal);
                    Assert.Contains("ancient-choice-text", playerTextChecklist, StringComparison.Ordinal);
                    Assert.Contains("preview-tools-text", playerTextChecklist, StringComparison.Ordinal);
                    Assert.Contains("en-zhs-key-parity", playerTextChecklist, StringComparison.Ordinal);
                    Assert.Contains("Fill this checklist with live results before marking this row pass.", playerTextWorkingChecklist, StringComparison.Ordinal);

                    var artRoutingRow = rows.Single(row => row.GetProperty("Id").GetString() == "art-resource-routing-live-preview");
                    var artRoutingFiles = artRoutingRow
                        .GetProperty("RequiredFiles")
                        .EnumerateArray()
                        .Select(file => file.GetString())
                        .ToArray();
                    Assert.Contains("art-resource-routing-checklist.md", artRoutingFiles);

                    var artRoutingDir = Path.Combine(evidenceDir, "art-resource-routing-live-preview");
                    Assert.True(
                        File.Exists(Path.Combine(artRoutingDir, "art-resource-routing-checklist-template.md")),
                        "Art routing row did not get a routing checklist template.");
                    var artRoutingReadme = File.ReadAllText(Path.Combine(artRoutingDir, "README.md"));
                    var artRoutingChecklist = File.ReadAllText(Path.Combine(artRoutingDir, "art-resource-routing-checklist-template.md"));
                    AssertTemplateChecklistCreated(artRoutingChecklist, "art-resource-routing-checklist.md");
                    var artRoutingWorkingChecklist = AssertWorkingChecklistCreated(
                        artRoutingDir,
                        "art-resource-routing-checklist.md",
                        ["title-home-preview", "option-relic-icons", "power-icons", "no-placeholder-or-official-art"]);
                    Assert.Contains("Manual checkpoints:", artRoutingReadme, StringComparison.Ordinal);
                    Assert.Contains("Confirm large Ancient/event art is used only on clicked Ancient or event screens", artRoutingReadme, StringComparison.Ordinal);
                    Assert.Contains("title-home-preview", artRoutingChecklist, StringComparison.Ordinal);
                    Assert.Contains("option-relic-icons", artRoutingChecklist, StringComparison.Ordinal);
                    Assert.Contains("power-icons", artRoutingChecklist, StringComparison.Ordinal);
                    Assert.Contains("no-placeholder-or-official-art", artRoutingChecklist, StringComparison.Ordinal);
                    Assert.Contains("Fill this checklist with live results before marking this row pass.", artRoutingWorkingChecklist, StringComparison.Ordinal);

                    var rootblightRow = rows.Single(row => row.GetProperty("Id").GetString() == "rootblight-visual-behavior");
                    var rootblightFiles = rootblightRow
                        .GetProperty("RequiredFiles")
                        .EnumerateArray()
                        .Select(file => file.GetString())
                        .ToArray();
                    Assert.Contains("rootblight-behavior-checklist.md", rootblightFiles);

                    var rootblightDir = Path.Combine(evidenceDir, "rootblight-visual-behavior");
                    Assert.True(
                        File.Exists(Path.Combine(rootblightDir, "rootblight-behavior-checklist-template.md")),
                        "Rootblight row did not get a behavior checklist template.");
                    var rootblightReadme = File.ReadAllText(Path.Combine(rootblightDir, "README.md"));
                    var rootblightChecklist = File.ReadAllText(Path.Combine(rootblightDir, "rootblight-behavior-checklist-template.md"));
                    AssertTemplateChecklistCreated(rootblightChecklist, "rootblight-behavior-checklist.md");
                    var rootblightWorkingChecklist = AssertWorkingChecklistCreated(
                        rootblightDir,
                        "rootblight-behavior-checklist.md",
                        ["rootblight-start-eligibility", "normal-rootblight-continuity", "boss-two-sprouts-staggered", "rootblight-save-load"]);
                    Assert.Contains("Manual checkpoints:", rootblightReadme, StringComparison.Ordinal);
                    Assert.Contains("normal fights advance existing Rootblight without expecting Blight Sprout cards", rootblightReadme, StringComparison.Ordinal);
                    Assert.Contains("Blight Sprout appears only in the current A15 Boss and A18 eligible Elite contexts", rootblightReadme, StringComparison.Ordinal);
                    Assert.Contains("rootblight-start-eligibility", rootblightChecklist, StringComparison.Ordinal);
                    Assert.Contains("normal-rootblight-continuity", rootblightChecklist, StringComparison.Ordinal);
                    Assert.DoesNotContain("normal-sprout-appearance", rootblightChecklist, StringComparison.Ordinal);
                    Assert.Contains("boss-two-sprouts-staggered", rootblightChecklist, StringComparison.Ordinal);
                    Assert.Contains("husk-exhaust-block-timing", rootblightChecklist, StringComparison.Ordinal);
                    Assert.Contains("rootblight-save-load", rootblightChecklist, StringComparison.Ordinal);
                    Assert.Contains("Fill this checklist with live results before marking this row pass.", rootblightWorkingChecklist, StringComparison.Ordinal);

                    var bossAbilityRow = rows.Single(row => row.GetProperty("Id").GetString() == "a19-a20-dedicated-boss-abilities");
                    var bossAbilityFiles = bossAbilityRow
                        .GetProperty("RequiredFiles")
                        .EnumerateArray()
                        .Select(file => file.GetString())
                        .ToArray();
                    Assert.Contains("boss-ability-checklist.md", bossAbilityFiles);

                    var bossAbilityDir = Path.Combine(evidenceDir, "a19-a20-dedicated-boss-abilities");
                    Assert.True(
                        File.Exists(Path.Combine(bossAbilityDir, "boss-ability-checklist-template.md")),
                        "A19/A20 row did not get a boss ability checklist template.");
                    var bossAbilityReadme = File.ReadAllText(Path.Combine(bossAbilityDir, "README.md"));
                    var bossAbilityChecklist = File.ReadAllText(Path.Combine(bossAbilityDir, "boss-ability-checklist-template.md"));
                    AssertTemplateChecklistCreated(bossAbilityChecklist, "boss-ability-checklist.md");
                    var bossAbilityWorkingChecklist = AssertWorkingChecklistCreated(
                        bossAbilityDir,
                        "boss-ability-checklist.md",
                        ["Martyr Oath", "Ink Return", "Time Sand Reflow", "Experimental Record"]);
                    Assert.Contains("Manual checkpoints:", bossAbilityReadme, StringComparison.Ordinal);
                    Assert.Contains("A20 Branded Form applies only to the second Act 3 Boss.", bossAbilityReadme, StringComparison.Ordinal);
                    Assert.Contains("Martyr Oath", bossAbilityChecklist, StringComparison.Ordinal);
                    Assert.Contains("Ink Return", bossAbilityChecklist, StringComparison.Ordinal);
                    Assert.Contains("Time Sand Reflow", bossAbilityChecklist, StringComparison.Ordinal);
                    Assert.Contains("Experimental Record", bossAbilityChecklist, StringComparison.Ordinal);
                    Assert.Contains("Fill this checklist with live results before marking this row pass.", bossAbilityWorkingChecklist, StringComparison.Ordinal);

                    AssertChecklistTemplate(
                        rows,
                        evidenceDir,
                        "vakuu-victory-no-black-screen",
                        "vakuu-victory-checklist.md",
                        "vakuu-victory-checklist-template.md",
                        ["fight-start-scene", "contract-turns", "locks-blood-debt", "victory-return", "non-vakuu-rewards", "no-black-screen", "log-clean"]);
                    AssertChecklistTemplate(
                        rows,
                        evidenceDir,
                        "vakuu-failure-death-path",
                        "vakuu-failure-death-checklist.md",
                        "vakuu-failure-death-checklist-template.md",
                        ["failure-path", "death-path", "room-state-after-exit", "no-softlock", "log-clean"]);
                    AssertChecklistTemplate(
                        rows,
                        evidenceDir,
                        "vakuu-active-fight-save-load",
                        "vakuu-save-load-checklist.md",
                        "vakuu-save-load-checklist-template.md",
                        ["active-combat-save", "active-combat-load", "parent-event-state", "prefinished-load", "no-duplicate-heal-or-reward"]);
                    AssertChecklistTemplate(
                        rows,
                        evidenceDir,
                        "preview-tools-live-proof",
                        "preview-tools-checklist.md",
                        "preview-tools-checklist-template.md",
                        ["crystal-sphere-button", "crystal-sphere-mask-only", "transform-preview-matches-result", "prismatic-gem-reward-hooks", "coop-gate-or-two-client-proof"]);
                    AssertChecklistTemplate(
                        rows,
                        evidenceDir,
                        "coop-disposition",
                        "coop-disposition-checklist.md",
                        "coop-disposition-checklist-template.md",
                        ["coop-host-join-clean-logs", "coop-a11-a20-selection", "coop-ancients", "coop-root-eyes", "coop-rootblight", "coop-save-load-or-reconnect", "coop-preview-tools-disposition", "coop-release-note-disposition"]);

                    var readme = File.ReadAllText(readmePath);
                    Assert.Contains("Required verifier row IDs:", readme, StringComparison.Ordinal);
                    Assert.Contains("Each verifier row has its own subfolder.", readme, StringComparison.Ordinal);
                    foreach (var expectedId in expectedRowIds)
                    {
                        Assert.Contains($"- {expectedId} ", readme, StringComparison.Ordinal);
                    }

                    Assert.DoesNotContain("Required high-level evidence:", readme, StringComparison.Ordinal);
                    Assert.DoesNotContain("- Clicked Ancient UI", readme, StringComparison.Ordinal);
                    Assert.Contains("release-evidence-verifier-pass.json", readme, StringComparison.Ordinal);

                    using var manifestDocument = JsonDocument.Parse(File.ReadAllText(manifestPath));
                    var manifest = manifestDocument.RootElement;
                    Assert.Equal(CurrentPackageZipSha256(), manifest.GetProperty("PackageSha256").GetString());
                    Assert.Equal(CurrentPackageZipRelativePath(), manifest.GetProperty("PackagePath").GetString());
                    Assert.Equal(rows.Length, manifest.GetProperty("Rows").GetArrayLength());

                    var verifier = AssertRepoFileExists("scripts", "verify-spire-plus-release-evidence.ps1");
                    var verifierResult = RunPowerShell(
                        verifier,
                        "-EvidenceRoot",
                        evidenceDir,
                        "-ManifestPath",
                        manifestPath);
                    Assert.NotEqual(0, verifierResult.ExitCode);
                    Assert.Contains("is not pass or accepted deferred", verifierResult.Output, StringComparison.Ordinal);
                    Assert.DoesNotContain("Missing release evidence manifest", verifierResult.Output, StringComparison.OrdinalIgnoreCase);

                    var manifestNode = JsonNode.Parse(File.ReadAllText(manifestPath))!.AsObject();
                    var ancientRewardNode = manifestNode["Rows"]!
                        .AsArray()
                        .Select(row => row!.AsObject())
                        .Single(row => row["Id"]!.GetValue<string>() == "ancient-reward-visible-relics");
                    var playerTextNode = manifestNode["Rows"]!
                        .AsArray()
                        .Select(row => row!.AsObject())
                        .Single(row => row["Id"]!.GetValue<string>() == "player-text-tooltip-readability");
                    var artRoutingNode = manifestNode["Rows"]!
                        .AsArray()
                        .Select(row => row!.AsObject())
                        .Single(row => row["Id"]!.GetValue<string>() == "art-resource-routing-live-preview");
                    var rootblightNode = manifestNode["Rows"]!
                        .AsArray()
                        .Select(row => row!.AsObject())
                        .Single(row => row["Id"]!.GetValue<string>() == "rootblight-visual-behavior");
                    var bossAbilityNode = manifestNode["Rows"]!
                        .AsArray()
                        .Select(row => row!.AsObject())
                        .Single(row => row["Id"]!.GetValue<string>() == "a19-a20-dedicated-boss-abilities");
                    var vakuuVictoryNode = manifestNode["Rows"]!
                        .AsArray()
                        .Select(row => row!.AsObject())
                        .Single(row => row["Id"]!.GetValue<string>() == "vakuu-victory-no-black-screen");
                    var vakuuFailureDeathNode = manifestNode["Rows"]!
                        .AsArray()
                        .Select(row => row!.AsObject())
                        .Single(row => row["Id"]!.GetValue<string>() == "vakuu-failure-death-path");
                    var vakuuSaveLoadNode = manifestNode["Rows"]!
                        .AsArray()
                        .Select(row => row!.AsObject())
                        .Single(row => row["Id"]!.GetValue<string>() == "vakuu-active-fight-save-load");
                    var previewToolsNode = manifestNode["Rows"]!
                        .AsArray()
                        .Select(row => row!.AsObject())
                        .Single(row => row["Id"]!.GetValue<string>() == "preview-tools-live-proof");
                    var coopNode = manifestNode["Rows"]!
                        .AsArray()
                        .Select(row => row!.AsObject())
                        .Single(row => row["Id"]!.GetValue<string>() == "coop-disposition");

                    foreach (var deferredRow in manifestNode["Rows"]!.AsArray())
                    {
                        var rowObject = deferredRow!.AsObject();
                        rowObject["Status"] = "deferred";
                        rowObject["ExplicitOwnerDecision"] = true;
                        rowObject["ReleaseNote"] = "Synthetic accepted deferral for verifier pass-marker contract test.";
                    }

                    var ancientRewardEvidenceDir = ancientRewardNode["EvidenceDir"]!.GetValue<string>();
                    File.WriteAllText(
                        Path.Combine(ancientRewardEvidenceDir, "godot.log"),
                        "Synthetic live log for Ancient reward verifier contract.");
                    File.WriteAllText(
                        Path.Combine(ancientRewardEvidenceDir, "godot-log-audit.json"),
                        """{ "Clean": true }""");
                    File.WriteAllText(
                        Path.Combine(ancientRewardEvidenceDir, "result-note.md"),
                        "Synthetic Ancient reward row result note for verifier contract.");
                    File.Copy(
                        Path.Combine(ancientRewardEvidenceDir, "ancient-reward-relics-checklist-template.md"),
                        Path.Combine(ancientRewardEvidenceDir, "ancient-reward-relics-checklist.md"),
                        overwrite: true);

                    ancientRewardNode["Status"] = "pass";
                    ancientRewardNode["ResultNote"] = "Synthetic pass attempt with an unfilled Ancient reward checklist.";
                    ancientRewardNode["ExplicitOwnerDecision"] = false;
                    ancientRewardNode["ReleaseNote"] = "";

                    var playerTextEvidenceDir = playerTextNode["EvidenceDir"]!.GetValue<string>();
                    File.WriteAllText(
                        Path.Combine(playerTextEvidenceDir, "godot.log"),
                        "Synthetic live log for player text verifier contract.");
                    File.WriteAllText(
                        Path.Combine(playerTextEvidenceDir, "godot-log-audit.json"),
                        """{ "Clean": true }""");
                    File.WriteAllText(
                        Path.Combine(playerTextEvidenceDir, "result-note.md"),
                        "Synthetic player text row result note for verifier contract.");
                    File.Copy(
                        Path.Combine(playerTextEvidenceDir, "player-text-qa-checklist-template.md"),
                        Path.Combine(playerTextEvidenceDir, "player-text-qa-checklist.md"),
                        overwrite: true);

                    playerTextNode["Status"] = "pass";
                    playerTextNode["ResultNote"] = "Synthetic pass attempt with an unfilled player text QA checklist.";
                    playerTextNode["ExplicitOwnerDecision"] = false;
                    playerTextNode["ReleaseNote"] = "";

                    var artRoutingEvidenceDir = artRoutingNode["EvidenceDir"]!.GetValue<string>();
                    File.WriteAllText(
                        Path.Combine(artRoutingEvidenceDir, "godot.log"),
                        "Synthetic live log for art routing verifier contract.");
                    File.WriteAllText(
                        Path.Combine(artRoutingEvidenceDir, "godot-log-audit.json"),
                        """{ "Clean": true }""");
                    File.WriteAllText(
                        Path.Combine(artRoutingEvidenceDir, "route-note.md"),
                        "Synthetic art routing row route note for verifier contract.");
                    File.WriteAllText(
                        Path.Combine(artRoutingEvidenceDir, "window-preflight.json"),
                        """{ "SpireForeground": true }""");
                    File.Copy(
                        Path.Combine(artRoutingEvidenceDir, "art-resource-routing-checklist-template.md"),
                        Path.Combine(artRoutingEvidenceDir, "art-resource-routing-checklist.md"),
                        overwrite: true);

                    artRoutingNode["Status"] = "pass";
                    artRoutingNode["ResultNote"] = "Synthetic pass attempt with an unfilled art routing checklist.";
                    artRoutingNode["ScreenshotFile"] = "screenshot.png";
                    artRoutingNode["ExplicitOwnerDecision"] = false;
                    artRoutingNode["ReleaseNote"] = "";
                    WriteTinyPng(Path.Combine(artRoutingEvidenceDir, "screenshot.png"), width: 800, height: 450);

                    var rootblightEvidenceDir = rootblightNode["EvidenceDir"]!.GetValue<string>();
                    File.WriteAllText(
                        Path.Combine(rootblightEvidenceDir, "godot.log"),
                        "Synthetic live log for Rootblight verifier contract.");
                    File.WriteAllText(
                        Path.Combine(rootblightEvidenceDir, "godot-log-audit.json"),
                        """{ "Clean": true }""");
                    File.WriteAllText(
                        Path.Combine(rootblightEvidenceDir, "result-note.md"),
                        "Synthetic Rootblight row result note for verifier contract.");
                    File.Copy(
                        Path.Combine(rootblightEvidenceDir, "rootblight-behavior-checklist-template.md"),
                        Path.Combine(rootblightEvidenceDir, "rootblight-behavior-checklist.md"),
                        overwrite: true);

                    rootblightNode["Status"] = "pass";
                    rootblightNode["ResultNote"] = "Synthetic pass attempt with an unfilled Rootblight behavior checklist.";
                    rootblightNode["ExplicitOwnerDecision"] = false;
                    rootblightNode["ReleaseNote"] = "";

                    var bossAbilityEvidenceDir = bossAbilityNode["EvidenceDir"]!.GetValue<string>();
                    File.WriteAllText(
                        Path.Combine(bossAbilityEvidenceDir, "godot.log"),
                        "Synthetic live log for A19/A20 verifier contract.");
                    File.WriteAllText(
                        Path.Combine(bossAbilityEvidenceDir, "godot-log-audit.json"),
                        """{ "Clean": true }""");
                    File.WriteAllText(
                        Path.Combine(bossAbilityEvidenceDir, "result-note.md"),
                        "Synthetic A19/A20 row result note for verifier contract.");
                    File.Copy(
                        Path.Combine(bossAbilityEvidenceDir, "boss-ability-checklist-template.md"),
                        Path.Combine(bossAbilityEvidenceDir, "boss-ability-checklist.md"),
                        overwrite: true);

                    bossAbilityNode["Status"] = "pass";
                    bossAbilityNode["ResultNote"] = "Synthetic pass attempt with an unfilled A19/A20 checklist.";
                    bossAbilityNode["ExplicitOwnerDecision"] = false;
                    bossAbilityNode["ReleaseNote"] = "";

                    PrepareChecklistPassAttempt(
                        vakuuVictoryNode,
                        "vakuu-victory-checklist-template.md",
                        "vakuu-victory-checklist.md",
                        requiredNoteFile: "result-note.md",
                        noteText: "Synthetic Vakuu victory row result note for verifier contract.",
                        resultNote: "Synthetic pass attempt with an unfilled Vakuu victory checklist.");
                    PrepareChecklistPassAttempt(
                        vakuuFailureDeathNode,
                        "vakuu-failure-death-checklist-template.md",
                        "vakuu-failure-death-checklist.md",
                        requiredNoteFile: "result-note.md",
                        noteText: "Synthetic Vakuu failure/death row result note for verifier contract.",
                        resultNote: "Synthetic pass attempt with an unfilled Vakuu failure/death checklist.");
                    PrepareChecklistPassAttempt(
                        vakuuSaveLoadNode,
                        "vakuu-save-load-checklist-template.md",
                        "vakuu-save-load-checklist.md",
                        requiredNoteFile: "save-load-note.md",
                        noteText: "Synthetic Vakuu save-load row note for verifier contract.",
                        resultNote: "Synthetic pass attempt with an unfilled Vakuu save-load checklist.");
                    PrepareChecklistPassAttempt(
                        previewToolsNode,
                        "preview-tools-checklist-template.md",
                        "preview-tools-checklist.md",
                        requiredNoteFile: "result-note.md",
                        noteText: "Synthetic preview tools row result note for verifier contract.",
                        resultNote: "Synthetic pass attempt with an unfilled preview tools checklist.");
                    PrepareChecklistPassAttempt(
                        coopNode,
                        "coop-disposition-checklist-template.md",
                        "coop-disposition-checklist.md",
                        requiredNoteFile: "result-note.md",
                        noteText: "Synthetic co-op row result note for verifier contract.",
                        resultNote: "Synthetic pass attempt with an unfilled co-op checklist.");
                    var coopEvidenceDir = coopNode["EvidenceDir"]!.GetValue<string>();
                    File.WriteAllText(
                        Path.Combine(coopEvidenceDir, "host-godot.log"),
                        "Synthetic host live log for co-op verifier contract.");
                    File.WriteAllText(
                        Path.Combine(coopEvidenceDir, "host-godot-log-audit.json"),
                        """{ "Clean": true }""");
                    File.WriteAllText(
                        Path.Combine(coopEvidenceDir, "client-godot.log"),
                        "Synthetic client live log for co-op verifier contract.");
                    File.WriteAllText(
                        Path.Combine(coopEvidenceDir, "client-godot-log-audit.json"),
                        """{ "Clean": true }""");

                    File.WriteAllText(
                        manifestPath,
                        manifestNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

                    var blankChecklistResult = RunPowerShell(
                        verifier,
                        "-EvidenceRoot",
                        evidenceDir,
                        "-ManifestPath",
                        manifestPath,
                        "-AllowDeferred");
                    Assert.NotEqual(0, blankChecklistResult.ExitCode);
                    Assert.Contains("boss-ability-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("row for Ceremonial Beast has no filled Live result cell", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("ancient-reward-relics-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("row for Urda / seedbed has no filled Screen option result cell", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("player-text-qa-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("row for ascension-a11-a20 has no filled EN result cell", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("art-resource-routing-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("row for title-home-preview has no filled Live result cell", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("rootblight-behavior-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("row for rootblight-start-eligibility has no filled Live result cell", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("vakuu-victory-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("row for fight-start-scene has no filled Live result cell", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("vakuu-failure-death-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("vakuu-save-load-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("preview-tools-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("row for crystal-sphere-button has no filled Live result cell", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("coop-disposition-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
                    Assert.Contains("row for coop-host-join-clean-logs has no filled Live result cell", blankChecklistResult.Output, StringComparison.Ordinal);

                    var filledAncientRewardChecklist = CreateFilledAncientRewardRelicsChecklist();
                    Assert.Contains("Vakuu's Sere Talon / \u74e6\u5e93\u539f\u521d\u4e4b\u722a", filledAncientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("Tanx Claws / \u5766\u514b\u65af\u5229\u722a", filledAncientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("Maul / \u6495\u54ac", filledAncientRewardChecklist, StringComparison.Ordinal);
                    Assert.DoesNotContain("\u95bb", filledAncientRewardChecklist, StringComparison.Ordinal);
                    Assert.DoesNotContain("\u95b8", filledAncientRewardChecklist, StringComparison.Ordinal);
                    Assert.DoesNotContain("\u95b9", filledAncientRewardChecklist, StringComparison.Ordinal);
                    File.WriteAllText(
                        Path.Combine(ancientRewardEvidenceDir, "ancient-reward-relics-checklist.md"),
                        filledAncientRewardChecklist);
                    File.WriteAllText(
                        Path.Combine(playerTextEvidenceDir, "player-text-qa-checklist.md"),
                        CreateFilledPlayerTextQaChecklist());
                    File.WriteAllText(
                        Path.Combine(artRoutingEvidenceDir, "art-resource-routing-checklist.md"),
                        CreateFilledArtResourceRoutingChecklist());
                    File.WriteAllText(
                        Path.Combine(rootblightEvidenceDir, "rootblight-behavior-checklist.md"),
                        CreateFilledRootblightBehaviorChecklist());
                    File.WriteAllText(
                        Path.Combine(bossAbilityEvidenceDir, "boss-ability-checklist.md"),
                        CreateFilledBossAbilityChecklist());
                    File.WriteAllText(
                        Path.Combine(vakuuVictoryNode["EvidenceDir"]!.GetValue<string>(), "vakuu-victory-checklist.md"),
                        CreateFilledSimpleChecklist("Vakuu Victory / No Black Screen Checklist", RequiredVakuuVictoryRows()));
                    File.WriteAllText(
                        Path.Combine(vakuuFailureDeathNode["EvidenceDir"]!.GetValue<string>(), "vakuu-failure-death-checklist.md"),
                        CreateFilledSimpleChecklist("Vakuu Failure / Death Checklist", RequiredVakuuFailureDeathRows()));
                    File.WriteAllText(
                        Path.Combine(vakuuSaveLoadNode["EvidenceDir"]!.GetValue<string>(), "vakuu-save-load-checklist.md"),
                        CreateFilledSimpleChecklist("Vakuu Save / Load Checklist", RequiredVakuuSaveLoadRows()));
                    File.WriteAllText(
                        Path.Combine(previewToolsNode["EvidenceDir"]!.GetValue<string>(), "preview-tools-checklist.md"),
                        CreateFilledSimpleChecklist("Preview Tools Checklist", RequiredPreviewToolsRows()));
                    File.WriteAllText(
                        Path.Combine(coopNode["EvidenceDir"]!.GetValue<string>(), "coop-disposition-checklist.md"),
                        CreateFilledSimpleChecklist("Co-op Disposition Checklist", RequiredCoopRows()));
                    ancientRewardNode["ResultNote"] = "Synthetic pass attempt with every Ancient reward relic row filled.";
                    playerTextNode["ResultNote"] = "Synthetic pass attempt with every player text QA row filled.";
                    artRoutingNode["ResultNote"] = "Synthetic pass attempt with every art routing surface row filled.";
                    rootblightNode["ResultNote"] = "Synthetic pass attempt with every Rootblight behavior row filled.";
                    bossAbilityNode["ResultNote"] = "Synthetic pass attempt with every A19/A20 Boss row filled.";
                    vakuuVictoryNode["ResultNote"] = "Synthetic pass attempt with every Vakuu victory row filled.";
                    vakuuFailureDeathNode["ResultNote"] = "Synthetic pass attempt with every Vakuu failure/death row filled.";
                    vakuuSaveLoadNode["ResultNote"] = "Synthetic pass attempt with every Vakuu save-load row filled.";
                    previewToolsNode["ResultNote"] = "Synthetic pass attempt with every preview tools row filled.";
                    coopNode["ResultNote"] = "Synthetic pass attempt with every co-op disposition row filled.";
                    File.WriteAllText(
                        manifestPath,
                        manifestNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

                    var passMarkerPath = Path.Combine(evidenceDir, "release-evidence-verifier-pass.json");
                    var passResult = RunPowerShell(
                        verifier,
                        "-EvidenceRoot",
                        evidenceDir,
                        "-ManifestPath",
                        manifestPath,
                        "-AllowDeferred",
                        "-WritePassMarker");
                    Assert.True(passResult.ExitCode == 0, $"Verifier did not accept explicit deferred manifest:{Environment.NewLine}{passResult.Output}");
                    Assert.True(File.Exists(passMarkerPath), "Verifier did not write release-evidence-verifier-pass.json.");

                    using var markerDocument = JsonDocument.Parse(File.ReadAllText(passMarkerPath));
                    var marker = markerDocument.RootElement;
                    Assert.Equal("pass", marker.GetProperty("Status").GetString());
                    Assert.Contains("verify-spire-plus-release-evidence.ps1", marker.GetProperty("Verifier").GetString(), StringComparison.Ordinal);
                    Assert.True(marker.GetProperty("AllowDeferred").GetBoolean());
                    Assert.Equal(rows.Length, marker.GetProperty("RequiredRowCount").GetInt32());
                }
            }
            finally
            {
                if (Directory.Exists(evidenceDir))
                {
                    Directory.Delete(evidenceDir, recursive: true);
                }
            }
        }

        var compatibilityWrapper = ReadRepoText("scripts", "collect-future-peek-evidence.ps1");
        Assert.Contains("collect-preview-tools-evidence.ps1", compatibilityWrapper, StringComparison.Ordinal);
        Assert.Contains("compatibility wrapper", compatibilityWrapper, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("EZFuturePeekCode", compatibilityWrapper, StringComparison.Ordinal);
        Assert.DoesNotContain("EZFuturePeek.json", compatibilityWrapper, StringComparison.Ordinal);
        Assert.DoesNotContain("EZFuturePeek.sln", compatibilityWrapper, StringComparison.Ordinal);
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

    [Fact]
    public void AncientUiEvidenceHelperCreatesPendingPreparePlansWithoutLaunching()
    {
        var script = AssertRepoFileExists("scripts", "collect-ancient-ui-evidence.ps1");
        foreach (var testCase in new[]
        {
            new { Ancient = "URDA", ForceFight = false, ExpectedOptions = 4, Command = "spireplus_test_ancient URDA confirm" },
            new { Ancient = "MORVI", ForceFight = false, ExpectedOptions = 3, Command = "spireplus_test_ancient MORVI confirm" },
            new { Ancient = "LOTHA", ForceFight = false, ExpectedOptions = 3, Command = "spireplus_test_ancient LOTHA confirm" },
            new { Ancient = "VAKUU", ForceFight = false, ExpectedOptions = 3, Command = "spireplus_test_ancient VAKUU confirm" },
            new { Ancient = "VAKUU", ForceFight = true, ExpectedOptions = 1, Command = "spireplus_test_ancient VAKUU confirm fight" }
        })
        {
            var evidenceDir = RepoPath(
                ".tools",
                "runtime-evidence",
                "test-release-evidence-gate",
                "collect-ancient-ui-evidence",
                $"{testCase.Ancient.ToLowerInvariant()}-{testCase.ForceFight}-{Guid.NewGuid():N}");
            Directory.CreateDirectory(evidenceDir);
            try
            {
                var arguments = new List<string>
                {
                    "-Mode",
                    "Prepare",
                    "-Ancient",
                    testCase.Ancient,
                    "-EvidenceDir",
                    evidenceDir,
                    "-NoPreflight"
                };
                if (testCase.ForceFight)
                {
                    arguments.Add("-ForceVakuuFight");
                }

                var result = RunPowerShell(script, arguments.ToArray());
                Assert.True(result.ExitCode == 0, $"collect-ancient-ui-evidence.ps1 failed:{Environment.NewLine}{result.Output}");

                var planPath = Path.Combine(evidenceDir, "ancient-ui-evidence-plan.json");
                var instructionsPath = Path.Combine(evidenceDir, "manual-instructions.md");
                var commandPath = Path.Combine(evidenceDir, "command.txt");
                var environmentPath = Path.Combine(evidenceDir, "environment.json");
                var packageHashesPath = Path.Combine(evidenceDir, "package-hashes.json");
                var manualRowsPath = Path.Combine(evidenceDir, "manual-rows-template.json");
                Assert.True(File.Exists(planPath), "Ancient UI helper did not write ancient-ui-evidence-plan.json.");
                Assert.True(File.Exists(instructionsPath), "Ancient UI helper did not write manual-instructions.md.");
                Assert.True(File.Exists(commandPath), "Ancient UI helper did not write command.txt.");
                Assert.True(File.Exists(environmentPath), "Ancient UI helper did not write environment.json.");
                Assert.True(File.Exists(packageHashesPath), "Ancient UI helper did not write package-hashes.json.");
                AssertPackageHashesUseVersionedArtifacts(packageHashesPath);
                Assert.True(File.Exists(manualRowsPath), "Ancient UI helper did not write manual-rows-template.json.");

                using var plan = JsonDocument.Parse(File.ReadAllText(planPath));
                var root = plan.RootElement;
                Assert.Equal(testCase.Ancient, root.GetProperty("Ancient").GetString());
                Assert.Equal(testCase.ExpectedOptions, root.GetProperty("ExpectedOptionCountForThisRun").GetInt32());
                Assert.Equal(testCase.Command, root.GetProperty("PreferredUnsavedDevConsoleCommand").GetString());
                Assert.Equal(testCase.Command, root.GetProperty("ExpectedDevConsoleCommand").GetString());
                Assert.StartsWith("ancient ", root.GetProperty("LegacyActiveRunDevConsoleCommand").GetString(), StringComparison.Ordinal);
                Assert.False(root.GetProperty("LaunchRequested").GetBoolean());
                Assert.True(root.GetProperty("NoPreflight").GetBoolean());
                Assert.Equal("This helper prepares evidence. It does not prove clicked UI by itself.", root.GetProperty("PendingNotice").GetString());
                Assert.Equal(testCase.Ancient, root.GetProperty("ForceEnvironment").GetProperty("SPIREPLUS_FORCE_ANCIENT").GetString());
                Assert.Equal(testCase.Ancient, root.GetProperty("ForceEnvironment").GetProperty("EZMB_FORCE_ANCIENT").GetString());
                Assert.Equal(testCase.ForceFight, root.GetProperty("ForceVakuuFight").GetBoolean());
                if (testCase.ForceFight)
                {
                    Assert.Equal("1", root.GetProperty("ForceEnvironment").GetProperty("SPIREPLUS_FORCE_VAKUU_FIGHT").GetString());
                    Assert.Equal("1", root.GetProperty("ForceEnvironment").GetProperty("EZMB_FORCE_VAKUU_FIGHT").GetString());
                }

                var requiredFiles = root.GetProperty("RequiredEvidenceFiles")
                    .EnumerateArray()
                    .Select(item => item.GetString())
                    .ToArray();
                Assert.Contains($"01-{testCase.Ancient.ToLowerInvariant()}-clicked-ui.png", requiredFiles);
                Assert.Contains("window-preflight.json", requiredFiles);
                Assert.Contains("godot.log", requiredFiles);
                Assert.Contains("godot-log-audit.json", requiredFiles);
                Assert.Contains("route-note.md", requiredFiles);

                using var environment = JsonDocument.Parse(File.ReadAllText(environmentPath));
                var environmentRoot = environment.RootElement;
                Assert.Equal("ancient-ui-clicked-evidence", environmentRoot.GetProperty("EvidenceKind").GetString());
                Assert.Equal(testCase.Ancient, environmentRoot.GetProperty("Ancient").GetString());
                Assert.Equal(testCase.Command, environmentRoot.GetProperty("PreferredUnsavedDevConsoleCommand").GetString());
                Assert.Equal(testCase.Command, environmentRoot.GetProperty("ExpectedDevConsoleCommand").GetString());
                Assert.StartsWith("ancient ", environmentRoot.GetProperty("LegacyActiveRunDevConsoleCommand").GetString(), StringComparison.Ordinal);
                Assert.False(environmentRoot.GetProperty("LaunchRequested").GetBoolean());
                Assert.True(environmentRoot.GetProperty("NoLaunch").GetBoolean());

                using var hashes = JsonDocument.Parse(File.ReadAllText(packageHashesPath));
                var files = hashes.RootElement.GetProperty("Files").EnumerateArray().ToArray();
                Assert.Contains(files, file => file.GetProperty("Path").GetString() == "EZMicroBalance.json");
                Assert.Contains(files, file => file.GetProperty("Path").GetString() == CurrentPackageZipRelativePath());

                using var rowsDocument = JsonDocument.Parse(File.ReadAllText(manualRowsPath));
                var rows = rowsDocument.RootElement.GetProperty("Rows").EnumerateArray().ToArray();
                var expectedRowId = testCase.Ancient == "VAKUU"
                    ? testCase.ForceFight ? "ancient-ui-vakuu-fight" : "ancient-ui-vakuu-normal"
                    : $"ancient-ui-{testCase.Ancient.ToLowerInvariant()}";
                var row = Assert.Single(rows);
                Assert.Equal(expectedRowId, row.GetProperty("Id").GetString());
                Assert.Equal("pending", row.GetProperty("Status").GetString());
                Assert.Equal($"01-{testCase.Ancient.ToLowerInvariant()}-clicked-ui.png", row.GetProperty("ScreenshotFile").GetString());
                var rowEvidence = row.GetProperty("RequiredEvidence")
                    .EnumerateArray()
                    .Select(item => item.GetString())
                    .ToArray();
                Assert.Contains("command.txt", rowEvidence);
                Assert.Contains("window-preflight.json", rowEvidence);
                Assert.Contains("godot.log", rowEvidence);
                Assert.Contains("godot-log-audit.json", rowEvidence);
                Assert.Contains("route-note.md", rowEvidence);

                var commandText = File.ReadAllText(commandPath);
                Assert.Contains("Tester launch command:", commandText, StringComparison.Ordinal);
                Assert.Contains("Restore command:", commandText, StringComparison.Ordinal);
                Assert.Contains("Legacy active-run render-smoke command:", commandText, StringComparison.Ordinal);
                Assert.Contains(testCase.Command, commandText, StringComparison.Ordinal);

                var instructions = File.ReadAllText(instructionsPath);
                Assert.Contains("Known pending result: this helper prepares evidence. It does not prove clicked UI by itself.", instructions, StringComparison.Ordinal);
                Assert.Contains("Do not mark clicked Ancient UI verified until", instructions, StringComparison.Ordinal);
                Assert.Contains("preferred Spire Plus process environment variables", instructions, StringComparison.Ordinal);
                Assert.Contains("SPIREPLUS_FORCE_ANCIENT", instructions, StringComparison.Ordinal);
                Assert.Contains("EZMB_FORCE_ANCIENT", instructions, StringComparison.Ordinal);
                Assert.Contains("The legacy aliases below are also set for compatibility", instructions, StringComparison.Ordinal);
                AssertBefore(
                    instructions,
                    "preferred Spire Plus process environment variables",
                    "The legacy aliases below are also set for compatibility");
                AssertBefore(instructions, "SPIREPLUS_FORCE_ANCIENT", "EZMB_FORCE_ANCIENT");
                Assert.Contains("Legacy active-run DevConsole render-smoke command:", instructions, StringComparison.Ordinal);
                Assert.Contains(testCase.Command, instructions, StringComparison.Ordinal);
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

    [Fact]
    public void CoopEvidenceTemplateCoversTwoClientRiskRows()
    {
        var source = ReadRepoText("scripts", "collect-coop-evidence.ps1");
        var scriptsReadme = ReadRepoText("scripts", "README.md");

        AssertSourceContains(
            source,
            "manual-instructions.md",
            "SPIREPLUS_RELEASE_EVIDENCE_LOG",
            "EZMB_RELEASE_EVIDENCE_LOG",
            "coop-host-join-clean-logs",
            "coop-a11-a20-selection",
            "coop-ancients",
            "coop-root-eyes",
            "coop-rootblight",
            "coop-save-load-or-reconnect",
            "coop-preview-tools-disposition",
            "Do not mark co-op supported from lobby selection alone.");

        AssertSourceContains(
            scriptsReadme,
            "`collect-coop-evidence.ps1`",
            "two-client co-op evidence",
            "does not auto-launch a two-client session");
    }

    [Fact]
    public void VakuuFightEvidenceTemplateCoversVictoryDeathAndSaveLoad()
    {
        var source = ReadRepoText("scripts", "collect-vakuu-fight-evidence.ps1");
        AssertSourceContains(
            source,
            "SPIREPLUS_FORCE_ANCIENT",
            "SPIREPLUS_FORCE_VAKUU_FIGHT",
            "SPIREPLUS_RELEASE_EVIDENCE_LOG",
            "EZMB_RELEASE_EVIDENCE_LOG",
            "vakuu-victory-no-black-screen",
            "vakuu-failure-death",
            "vakuu-active-save-load",
            "vakuu-prefinished-save-load",
            "non-Vakuu Ancient rewards");
    }

    [Fact]
    public void DefaultModeDoesNotRequireLiveEvidenceButBlocksReleaseReadyClaims()
    {
        var enforce = string.Equals(
            Environment.GetEnvironmentVariable("EZMB_ENFORCE_RELEASE_READY"),
            "1",
            StringComparison.Ordinal);

        if (enforce)
        {
            var missing = RequiredReleaseEvidence()
                .Where(requirement => !requirement.IsPresent())
                .Select(requirement => $"{requirement.Key}: {requirement.Description}")
                .ToArray();

            Assert.True(
                missing.Length == 0,
                "EZMB_ENFORCE_RELEASE_READY=1 requires complete live release evidence. Missing:" +
                Environment.NewLine +
                string.Join(Environment.NewLine, missing));
            return;
        }

        var currentDocs = ReadCurrentFacingDocs(
            "PROJECT_STATE.md",
            "README.md",
            "docs/release-checklist.md",
            "docs/release-evidence-status.md",
            "docs/specs/release-traceability-matrix.md",
            "docs/private-beta-release-completion-audit.md",
            "docs/private-beta-verification-handoff.md");

        Assert.Contains("fresh-current-package-loader-smoke", currentDocs, StringComparison.Ordinal);
        Assert.Contains("ancient-ui-urda", currentDocs, StringComparison.Ordinal);
        Assert.Contains("vakuu-active-fight-save-load", currentDocs, StringComparison.Ordinal);
        Assert.Contains("ancient-state-save-load", currentDocs, StringComparison.Ordinal);
        Assert.Contains("a19-a20-dedicated-boss-abilities", currentDocs, StringComparison.Ordinal);
        Assert.Contains("preview-tools-live-proof", currentDocs, StringComparison.Ordinal);
        Assert.Contains("coop-disposition", currentDocs, StringComparison.Ordinal);
        foreach (var falseClaim in new[]
        {
            "currently release-ready",
            "is release-ready",
            "release-ready: true",
            "publish-proven: true",
            "full co-op support: true",
            "full multiplayer support: true"
        })
        {
            Assert.DoesNotContain(falseClaim, currentDocs, StringComparison.OrdinalIgnoreCase);
        }

        var projectState = ReadRepoText("PROJECT_STATE.md");
        Assert.Contains("current source defines 30 SavedSpireFields", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("beta.59 Lotha Death Reprieve turn split package sync", projectState, StringComparison.Ordinal);
        Assert.Contains("Found 30 SavedSpireFields", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Found 22 SavedSpireFields", projectState, StringComparison.OrdinalIgnoreCase);
    }

    private static RequiredEvidence[] RequiredReleaseEvidence()
    {
        return
        [
            new(
                "fresh-loader-smoke",
                "Fresh current-package loader smoke with current package hashes and clean log audit",
                () => HasEvidenceDirectory(
                    "release-evidence-*",
                    "command.txt",
                    "environment.json",
                    "package-hashes.json",
                    "godot.log",
                    "godot-log-audit.json")),
            new(
                "clicked-ancient-ui",
                "Clicked Urda/Morvi/Lotha/Vakuu Ancient UI screenshots plus foreground/log evidence",
                () => HasEvidenceDirectory(
                    "ancient-ui-click-*",
                    "command.txt",
                    "window-preflight.json",
                    "godot.log",
                    "godot-log-audit.json",
                    "route-note.md") &&
                    Directory.GetFiles(RuntimeEvidenceRoot(), "ancient-ui-click-*", SearchOption.AllDirectories)
                        .Any(path => path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))),
            new(
                "save-load",
                "Ancient and gameplay save/load evidence",
                () => HasEvidenceDirectoryContaining("save-load", "godot.log", "godot-log-audit.json")),
            new(
                "vakuu",
                "Vakuu victory/no-black-screen/failure/death evidence",
                () => HasEvidenceDirectoryContaining("vakuu", "godot.log", "godot-log-audit.json", "vakuu-release-evidence-pass.json")),
            new(
                "coop",
                "Two-client host/client co-op evidence",
                () => HasEvidenceDirectory(
                    "coop-evidence-*",
                    "host/command.txt",
                    "host/godot.log",
                    "host/godot-log-audit.json",
                    "client/command.txt",
                    "client/godot.log",
                    "client/godot-log-audit.json")),
            new(
                "preview-tools",
                "Live Preview tools evidence for Crystal Sphere and transform preview",
                () => HasEvidenceDirectory(
                    "preview-tools-evidence-*",
                    "command.txt",
                    "environment.json",
                    "package-hashes.json",
                    "godot.log",
                    "godot-log-audit.json")),
            new(
                "release-evidence-verifier",
                "verify-spire-plus-release-evidence.ps1 pass marker",
                HasVerifierPassMarker)
        ];
    }

    private static bool HasEvidenceDirectory(string searchPattern, params string[] requiredRelativeFiles)
    {
        var root = RuntimeEvidenceRoot();
        if (!Directory.Exists(root))
        {
            return false;
        }

        return Directory.GetDirectories(root, searchPattern, SearchOption.AllDirectories)
            .Any(directory => requiredRelativeFiles.All(relativeFile => File.Exists(Path.Combine(directory, relativeFile))));
    }

    private static bool HasEvidenceDirectoryContaining(string directoryNameFragment, params string[] requiredRelativeFiles)
    {
        var root = RuntimeEvidenceRoot();
        if (!Directory.Exists(root))
        {
            return false;
        }

        return Directory.GetDirectories(root, "*", SearchOption.AllDirectories)
            .Where(directory => directory.Contains(directoryNameFragment, StringComparison.OrdinalIgnoreCase))
            .Any(directory => requiredRelativeFiles.All(relativeFile => File.Exists(Path.Combine(directory, relativeFile))));
    }

    private static bool HasVerifierPassMarker()
    {
        var root = RuntimeEvidenceRoot();
        if (!Directory.Exists(root))
        {
            return false;
        }

        foreach (var marker in Directory.GetFiles(root, "release-evidence-verifier-pass.json", SearchOption.AllDirectories))
        {
            try
            {
                using var document = JsonDocument.Parse(File.ReadAllText(marker));
                var rootElement = document.RootElement;
                if (rootElement.TryGetProperty("Status", out var status) &&
                    string.Equals(status.GetString(), "pass", StringComparison.OrdinalIgnoreCase) &&
                    rootElement.TryGetProperty("Verifier", out var verifier) &&
                    verifier.GetString()?.Contains("verify-spire-plus-release-evidence.ps1", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return true;
                }
            }
            catch (JsonException)
            {
            }
        }

        return false;
    }

    private static string RuntimeEvidenceRoot()
    {
        return RepoPath(".tools", "runtime-evidence");
    }

    private static (int ExitCode, string Output) RunPowerShell(string scriptPath, params string[] arguments)
    {
        var executable = OperatingSystem.IsWindows() ? "powershell.exe" : "pwsh";
        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            WorkingDirectory = Root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-ExecutionPolicy");
        startInfo.ArgumentList.Add("Bypass");
        startInfo.ArgumentList.Add("-File");
        startInfo.ArgumentList.Add(scriptPath);
        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = Process.Start(startInfo);
        Assert.NotNull(process);
        var output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
        Assert.True(process.WaitForExit(30_000), $"Timed out running {scriptPath}.");
        return (process.ExitCode, output);
    }

    private static void AssertChecklistTemplate(
        JsonElement[] rows,
        string evidenceDir,
        string rowId,
        string requiredChecklist,
        string templateFile,
        string[] expectedFragments)
    {
        var row = rows.Single(row => row.GetProperty("Id").GetString() == rowId);
        var requiredFiles = row
            .GetProperty("RequiredFiles")
            .EnumerateArray()
            .Select(file => file.GetString())
            .ToArray();
        Assert.Contains(requiredChecklist, requiredFiles);

        var rowDir = Path.Combine(evidenceDir, rowId);
        var templatePath = Path.Combine(rowDir, templateFile);
        Assert.True(File.Exists(templatePath), $"{rowId} did not get {templateFile}.");
        var rowReadme = File.ReadAllText(Path.Combine(rowDir, "README.md"));
        var checklist = File.ReadAllText(templatePath);
        Assert.Contains("Manual checkpoints:", rowReadme, StringComparison.Ordinal);
        AssertTemplateChecklistCreated(checklist, requiredChecklist);
        foreach (var expectedFragment in expectedFragments)
        {
            Assert.Contains(expectedFragment, checklist, StringComparison.Ordinal);
        }

        AssertWorkingChecklistCreated(rowDir, requiredChecklist, expectedFragments);
    }

    private static string AssertWorkingChecklistCreated(
        string rowDir,
        string checklistFile,
        string[] expectedFragments)
    {
        var workingPath = Path.Combine(rowDir, checklistFile);
        Assert.True(File.Exists(workingPath), $"Missing generated working checklist: {checklistFile}.");
        var workingChecklist = File.ReadAllText(workingPath);
        Assert.Contains("Fill this checklist with live results before marking this row pass.", workingChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Copy this file to", workingChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Template reference for", workingChecklist, StringComparison.Ordinal);
        foreach (var expectedFragment in expectedFragments)
        {
            Assert.Contains(expectedFragment, workingChecklist, StringComparison.Ordinal);
        }

        return workingChecklist;
    }

    private static void AssertTemplateChecklistCreated(string templateChecklist, string checklistFile)
    {
        Assert.Contains($"Template reference for `{checklistFile}`.", templateChecklist, StringComparison.Ordinal);
        Assert.Contains($"Fill the working `{checklistFile}` with live results before marking this row pass.", templateChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Copy this file to", templateChecklist, StringComparison.Ordinal);
    }

    private static void PrepareChecklistPassAttempt(
        JsonObject rowNode,
        string templateFile,
        string checklistFile,
        string requiredNoteFile,
        string noteText,
        string resultNote)
    {
        var evidenceDir = rowNode["EvidenceDir"]!.GetValue<string>();
        File.WriteAllText(
            Path.Combine(evidenceDir, "godot.log"),
            $"Synthetic live log for {checklistFile} verifier contract.");
        File.WriteAllText(
            Path.Combine(evidenceDir, "godot-log-audit.json"),
            """{ "Clean": true }""");
        File.WriteAllText(Path.Combine(evidenceDir, requiredNoteFile), noteText);
        File.Copy(
            Path.Combine(evidenceDir, templateFile),
            Path.Combine(evidenceDir, checklistFile),
            overwrite: true);

        rowNode["Status"] = "pass";
        rowNode["ResultNote"] = resultNote;
        rowNode["ExplicitOwnerDecision"] = false;
        rowNode["ReleaseNote"] = "";
    }

    private static string CreateFilledBossAbilityChecklist()
    {
        var bosses = new[]
        {
            ("Ceremonial Beast", "Holy Daze caps stun hits and grants Strength.", "Branded Form grants the higher Strength value."),
            ("The Kin", "Martyr Oath consumes up to 2 follower-death stacks and updates attack intent.", "Same-turn double follower death grants exactly 1 Artifact; attack bonus is higher."),
            ("Vantom", "Ink Return restores a percentage of cleared Slippery once.", "Higher restore percentage/caps apply."),
            ("Lagavulin Matriarch", "Plating Wake grants Multiplating based on wake source and Soul Siphon reduces it.", "Branded Form values and reduction differ as documented."),
            ("Soul Fysh", "Soul Tide counts unanswered Beckons at player turn end, applies capped Block after Soul Fysh's turn, and grants Artifact on Intangible.", "Higher per-Beckon Block and cap apply."),
            ("Waterfall Giant", "Unweakenable clears Weak/negative Strength for the explosion and applies Vulnerable to affected players.", "Vulnerable duration is higher."),
            ("Crab", "Claw Calibration reacts to claw HP-ratio gaps and updates attack intent.", "Lower threshold and higher attack bonus apply."),
            ("Knowledge Demon", "Marginal Note and Deep Thought add side costs without hard-locking Sloth/Waste Away.", "Deep Thought cap and side-cost rules match v4.1."),
            ("Insatiable Sandworm", "Escape Fatigue grants Vigor after generated Escape cards.", "Higher Vigor applies with team cap."),
            ("Aeonglass", "Time Sand Reflow adds Wither after Fade and clears by spent energy.", "Eye Lasers extra hit appears in intent only while Time Sand remains."),
            ("Queen", "Royal Decree marks one Bound card and team-caps Majesty/Torch Head Strength.", "Majesty cap and spend limit are higher."),
            ("Test Subject", "Experimental Record shows a phase-change notice and residual sample power.", "Two different samples appear on phase change.")
        };

        var lines = new List<string>
        {
            "# A19/A20 Dedicated Boss Ability Checklist",
            "",
            "| Boss | A19 ability | A20 Branded Form check | Live result | Evidence file(s) |",
            "| --- | --- | --- | --- | --- |"
        };

        lines.AddRange(bosses.Select(boss =>
            $"| {boss.Item1} | {boss.Item2} | {boss.Item3} | PASS: synthetic verifier contract row filled. | godot.log; result-note.md |"));

        return string.Join(Environment.NewLine, lines);
    }

    private static IReadOnlyList<(string Id, string Expected)> RequiredVakuuVictoryRows()
    {
        return
        [
            ("fight-start-scene", "Dedicated Vakuu monster and encounter scene appear, not a placeholder or normal Ancient screen."),
            ("contract-turns", "Contract choices appear on expected turns and do not softlock the hand."),
            ("locks-blood-debt", "Stolen Vault locks, broken-lock count, Blood Debt, Gold/HP settlement, and lethal-hit lock counting are visible and coherent."),
            ("victory-return", "Winning returns to a usable event/reward/map state."),
            ("non-vakuu-rewards", "Victory offers non-Vakuu Ancient reward choices and no normal combat card reward."),
            ("no-black-screen", "The screen does not go black, freeze, or require force quit after victory."),
            ("log-clean", "Logs contain no release-blocking exception, stale parent event, room stack, or reward-screen error.")
        ];
    }

    private static IReadOnlyList<(string Id, string Expected)> RequiredVakuuFailureDeathRows()
    {
        return
        [
            ("fight-start-scene", "Dedicated Vakuu fight starts before the failure/death path is tested."),
            ("failure-path", "A non-death failure path exits cleanly if the design exposes one. If not reachable, record why."),
            ("death-path", "Death reaches the expected run-end/game-over flow without stale Ancient UI or hidden reward screens."),
            ("room-state-after-exit", "Room, event, reward, and map state remain coherent after failure/death."),
            ("no-softlock", "The game remains responsive or reaches the expected terminal run state."),
            ("log-clean", "Logs contain no release-blocking exception, stale parent event, room stack, or reward-screen error.")
        ];
    }

    private static IReadOnlyList<(string Id, string Expected)> RequiredVakuuSaveLoadRows()
    {
        return
        [
            ("active-combat-save", "Save succeeds during active Vakuu child combat."),
            ("active-combat-load", "Reload restores the active fight, Vakuu state, contracts, locks, and Blood Debt coherently."),
            ("parent-event-state", "Parent event/room stack state is not lost or duplicated after active-fight reload."),
            ("prefinished-save", "Save succeeds around post-combat reward/return after Vakuu is defeated."),
            ("prefinished-load", "Reload restores the reward/return state without black screen or stale combat room."),
            ("no-duplicate-heal-or-reward", "Reload does not duplicate Ancient heal, Vakuu rewards, normal combat rewards, or parent event cleanup."),
            ("log-clean", "Logs contain no release-blocking save/load, room stack, parent event, or reward-screen error.")
        ];
    }

    private static IReadOnlyList<(string Id, string Expected)> RequiredPreviewToolsRows()
    {
        return
        [
            ("crystal-sphere-button", "Crystal Sphere screen shows the Spire Plus peek control in the expected UI area."),
            ("crystal-sphere-mask-only", "Toggling peek only changes ScryMask visibility/opacity and does not call cell clear/reveal/claim behavior."),
            ("crystal-sphere-no-reward-claim", "No relic, potion, card, curse, or gold reward is granted or consumed by previewing."),
            ("transform-preview-visible", "Transform preview displays a concrete predicted replacement instead of cycling fake random cards."),
            ("transform-preview-matches-result", "The shown transform preview matches the actual transformed card result for the tested card(s)."),
            ("transform-preview-no-state-mutation", "Previewing does not advance real RNG, create real mutable replacement cards, or change deck/reward state before confirmation."),
            ("prismatic-gem-reward-hooks", "Prismatic Gem preview reflects reward-modifying hooks without suppressing later Core reward changes."),
            ("save-reopen-stability", "Save/reopen around preview screens does not change the previewed result or corrupt the reward/room state."),
            ("coop-gate-or-two-client-proof", "Multiplayer behavior is either gated with a clear warning/log or proven with two-client evidence."),
            ("log-clean", "Logs contain no preview-tool exception, RNG drift warning, reward-state error, or co-op desync marker.")
        ];
    }

    private static IReadOnlyList<(string Id, string Expected)> RequiredCoopRows()
    {
        return
        [
            ("coop-host-join-clean-logs", "Host and client load exactly BaseLib plus Spire Plus with matching package hashes and clean logs."),
            ("coop-a11-a20-selection", "A11-A20 selection/start-run behavior is recorded on host and client; selection visibility alone is not gameplay support."),
            ("coop-ancients", "Urda, Morvi, Lotha, and gated Vakuu have explicit host/client disposition notes for reward state and relic visibility."),
            ("coop-root-eyes", "Root Eyes map preview either stays gated in co-op or shows host/client-consistent map state with no desync."),
            ("coop-rootblight", "Rootblight ownership, combat/deck state, and Sprout growth are visible and consistent on host and client."),
            ("coop-save-load-or-reconnect", "Save/load or reconnect behavior is proven with host/client before-after logs, or explicitly deferred by owner."),
            ("coop-preview-tools-disposition", "Crystal Sphere, transform preview, and Prismatic Gem preview have a fairness/disposition note and no desync evidence."),
            ("coop-release-note-disposition", "Final co-op wording is explicit: supported with evidence, gated, unsupported, or owner-approved deferred.")
        ];
    }

    private static string CreateFilledSimpleChecklist(
        string title,
        IReadOnlyList<(string Id, string Expected)> rows)
    {
        var lines = new List<string>
        {
            $"# {title}",
            "",
            "| Scenario ID | Expected behavior | Live result | Evidence file(s) |",
            "| --- | --- | --- | --- |"
        };

        lines.AddRange(rows.Select(row =>
            $"| {row.Id} | {row.Expected} | PASS: synthetic verifier contract row filled. | godot.log; result-note.md |"));

        return string.Join(Environment.NewLine, lines);
    }

    private static string CreateFilledAncientRewardRelicsChecklist()
    {
        var rows = new[]
        {
            ("Urda", "seedbed", "UrdaSeedbedOptionRelic"),
            ("Urda", "humus_pact", "UrdaHumusPactOptionRelic"),
            ("Urda", "molting", "UrdaMoltingOptionRelic"),
            ("Urda", "moss_map", "UrdaMossMapOptionRelic"),
            ("Urda", "trial_branch", "UrdaTrialBranchOptionRelic"),
            ("Urda", "shallow_root_relic", "UrdaShallowRootRelicOptionRelic"),
            ("Urda", "elite_root", "UrdaEliteRootOptionRelic"),
            ("Urda", "rooted_route", "UrdaRootedRouteOptionRelic"),
            ("Urda", "after_rain", "UrdaAfterRainOptionRelic"),
            ("Urda", "root_sight", "UrdaRootSightOptionRelic"),
            ("Urda", "seed_bank", "UrdaSeedBankOptionRelic"),
            ("Morvi", "forbidden_loan", "MorviForbiddenLoanOptionRelic"),
            ("Morvi", "misprint_press", "MorviMisprintPressOptionRelic"),
            ("Morvi", "red_ink_overdraft", "MorviRedInkOverdraftOptionRelic"),
            ("Morvi", "overdue_library", "MorviOverdueLibraryOptionRelic"),
            ("Morvi", "open_book_exam", "MorviOpenBookExamOptionRelic"),
            ("Morvi", "paperstorm", "MorviPaperstormOptionRelic"),
            ("Morvi", "blueprint_proof", "MorviBlueprintProofOptionRelic"),
            ("Morvi", "debt_settlement", "MorviDebtSettlementOptionRelic"),
            ("Lotha", "mirror_rebuttal", "LothaMirrorRebuttalOptionRelic"),
            ("Lotha", "mirror_hall_echo", "LothaMirrorHallEchoOptionRelic"),
            ("Lotha", "presumption", "LothaPresumptionOptionRelic"),
            ("Lotha", "closed_court", "LothaClosedCourtOptionRelic"),
            ("Lotha", "deferred_verdict", "LothaDeferredVerdictOptionRelic"),
            ("Lotha", "death_reprieve", "LothaDeathReprieveOptionRelic"),
            ("Lotha", "single_sentence", "LothaSingleSentenceOptionRelic"),
            ("Lotha", "public_evidence", "LothaPublicEvidenceOptionRelic"),
            ("Vakuu", "fight_option", "VakuuFightOptionRelic"),
            ("Vakuu", "victory_non_vakuu_choices", "Non-Vakuu Act 3 Ancient reward relic choices after winning Vakuu"),
            ("Vakuu event", "sere_talon_pickup", "Vakuu's Sere Talon / \u74e6\u5e93\u539f\u521d\u4e4b\u722a lets the player choose 1 of 4 Curses, then adds that Curse, 2 Wish, and 1 Wish+; verify event-option art, relic-bar art, inspect art, hover text, and surface-specific log routes such as `Ancient event option button`, `RelicModel packed icon texture`, and `RelicModel big icon texture` are not Tanx Claws."),
            ("Tanx event", "claws_maul_transform", "Tanx Claws / \u5766\u514b\u65af\u5229\u722a transforms cards into upgraded Maul / \u6495\u54ac+ cards.")
        };

        var lines = new List<string>
        {
            "# Ancient Reward Visible Relics Checklist",
            "",
            "| Ancient | Reward ID | Expected option/relic | Screen option visible | Relic bar / hover result | Evidence file(s) |",
            "| --- | --- | --- | --- | --- | --- |"
        };

        lines.AddRange(rows.Select(row =>
            $"| {row.Item1} | {row.Item2} | {row.Item3} | PASS: synthetic option relic visible. | PASS: synthetic relic bar hover readable. | godot.log; result-note.md |"));

        return string.Join(Environment.NewLine, lines);
    }

    private static string CreateFilledPlayerTextQaChecklist()
    {
        var rows = new[]
        {
            ("ascension-a11-a20", "A11-A20 descriptions are short, concrete, use Dedicated Ability/Branded Form wording, and avoid stale Royal Seal/King Brand terms."),
            ("firemark-and-banner", "Firemark and Banner text shows current-act values, explains Host/Overflow/Forge Armor/Shieldwall clearly, and avoids slash-table wording in live hovers."),
            ("boss-dedicated-abilities", "A19/A20 Boss power hovers explain the matching Boss ability with final damage/intent implications and multiplayer caps where relevant."),
            ("ancient-choice-text", "Urda, Morvi, Lotha, Vakuu option rows explain what the player gains or risks without implementation terms."),
            ("ancient-relic-hover", "Option relic and selected-relic hover text is readable from the relic bar and matches the actual active reward."),
            ("cards-status-curses", "Blight Sprout, Rootblight, Husk, Seedbed, Contract, Rain Breath, Marginal Note, and generated temporary cards use clear card/status wording."),
            ("map-hover-stacks", "Root Eyes, Firemarked Elite, Banner, Deep Branch, Boss ability, and Branded Form map hovers stack without hiding each other."),
            ("preview-tools-text", "Crystal Sphere peek, transform preview, and Prismatic Gem preview text explain preview behavior without implying reward claim or RNG mutation."),
            ("vakuu-contracts", "Vakuu contracts, Blood Debt, locks, Cash Out, and victory reward choices explain the greed/stop decision and post-fight settlement."),
            ("en-zhs-key-parity", "EN and ZHS keys, dynamic variables, and rich-text tags match for tested surfaces; no mojibake or missing localization appears.")
        };

        var lines = new List<string>
        {
            "# Player Text / Tooltip QA Checklist",
            "",
            "| Surface ID | Expected text quality | EN result | ZHS result | Evidence file(s) |",
            "| --- | --- | --- | --- | --- |"
        };

        lines.AddRange(rows.Select(row =>
            $"| {row.Item1} | {row.Item2} | PASS: synthetic EN text QA row filled. | PASS: synthetic ZHS text QA row filled. | godot.log; result-note.md |"));

        return string.Join(Environment.NewLine, lines);
    }

    private static string CreateFilledArtResourceRoutingChecklist()
    {
        var rows = new[]
        {
            ("title-home-preview", "Spire Plus title/home preview image fits the UI frame and does not stretch, crop critical subject matter, or use stale pre-refresh branding."),
            ("urda-clicked-background", "Urda large background appears only on the clicked Ancient screen/event surface and fits behind option rows."),
            ("morvi-clicked-background", "Morvi large background appears only on the clicked Ancient screen/event surface and fits behind option rows."),
            ("lotha-clicked-background", "Lotha large background appears only on the clicked Ancient screen/event surface and fits behind option rows."),
            ("vakuu-clicked-background", "Vakuu normal and force-fight clicked screens use the intended large art without hiding option text."),
            ("map-icons", "Ancient, Root Eyes, Firemark, Banner, Deep Branch, and other map markers use readable small icons, not full-size event art."),
            ("run-history-icons", "Run-history icons use small-format art and remain distinguishable from map icons and clicked-screen backgrounds."),
            ("option-relic-icons", "Ancient option relic choices use option relic icons and do not reuse clicked-screen backgrounds or placeholder crops."),
            ("lasting-relic-icons", "Selected lasting Ancient rewards appear in the relic bar with readable small icons and hover art/text."),
            ("card-art", "Rootblight, Blight Sprout, Husk, Contract, Rain Breath, Seedbed-related cards, and preview cards use the expected card portraits."),
            ("power-icons", "Firemark, Banner, A19/A20 dedicated abilities, Vakuu powers, Seedbed, and Rootblight powers use visible non-NOPE icons."),
            ("no-placeholder-or-official-art", "No tested surface shows NOPE, generic temporary art, placeholder crops, stale logo text, or copied official non-art source material.")
        };

        var lines = new List<string>
        {
            "# Art / Resource Routing Checklist",
            "",
            "| Surface ID | Expected routing | Live result | Evidence file(s) |",
            "| --- | --- | --- | --- |"
        };

        lines.AddRange(rows.Select(row =>
            $"| {row.Item1} | {row.Item2} | PASS: synthetic art routing verifier contract row filled. | screenshot.png; godot.log; route-note.md |"));

        return string.Join(Environment.NewLine, lines);
    }

    private static string CreateFilledRootblightBehaviorChecklist()
    {
        var rows = new[]
        {
            ("rootblight-start-eligibility", "A14+ run starts/repairs Rootblight setup only after a real deck card exists; no silent permanent disable if the deck is temporarily unavailable."),
            ("normal-rootblight-continuity", "Ordinary normal combats do not add Blight Sprout in the current design; they should still mark existing Rootblight, show it in the deck/hand flow, and resolve combat-end growth/cap rules."),
            ("elite-single-sprout", "Elite combat adds exactly one Blight Sprout at the expected timing and does not duplicate across reload/reentry."),
            ("boss-two-sprouts-staggered", "Act 2/3 Boss combat adds two Blight Sprouts on the staggered turns, with both cards visible when expected."),
            ("husk-exhaust-block-timing", "Withered Husk grants exactly 3 Block when it is Exhausted, not when it merely has Ethereal/Void text or sits in hand."),
            ("combat-end-growth", "An unresolved Blight Sprout grows into Rootblight after combat; handled/planted Sprouts do not grow."),
            ("rootblight-cap-four", "Rootblight respects the current maximum and Rootblight III split/growth rule without exceeding 4 cards."),
            ("rootblight-save-load", "Save/load before Sprout entry, during combat, and after combat preserves pending markers and deck state."),
            ("ui-hover-art-readability", "Blight Sprout, Rootblight, Seedbed/Husk interactions, card art, and EN/ZHS hover text are visible and readable.")
        };

        var lines = new List<string>
        {
            "# Rootblight / Blight Sprout Behavior Checklist",
            "",
            "| Scenario ID | Expected behavior | Live result | Evidence file(s) |",
            "| --- | --- | --- | --- |"
        };

        lines.AddRange(rows.Select(row =>
            $"| {row.Item1} | {row.Item2} | PASS: synthetic Rootblight verifier contract row filled. | godot.log; result-note.md |"));

        return string.Join(Environment.NewLine, lines);
    }

    private static void WriteTinyPng(string path, int width, int height)
    {
        static byte[] UInt32BigEndian(int value)
        {
            return
            [
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF)
            ];
        }

        var bytes = new List<byte>
        {
            137,
            80,
            78,
            71,
            13,
            10,
            26,
            10,
            0,
            0,
            0,
            13,
            (byte)'I',
            (byte)'H',
            (byte)'D',
            (byte)'R'
        };
        bytes.AddRange(UInt32BigEndian(width));
        bytes.AddRange(UInt32BigEndian(height));
        bytes.AddRange(new byte[] { 8, 6, 0, 0, 0, 0, 0, 0, 0 });
        File.WriteAllBytes(path, bytes.ToArray());
    }
}
