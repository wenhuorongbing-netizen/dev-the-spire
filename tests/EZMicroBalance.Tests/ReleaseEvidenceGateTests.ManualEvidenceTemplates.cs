using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseEvidenceGateTests
{
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
                AssertEnvironmentIncludesGitHandoffEvidence(environmentPath);
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
        Assert.Contains("current source defines 30 SavedAttachedState fields", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(ManifestVersion(), projectState, StringComparison.Ordinal);
        Assert.Contains("Current installed game is `v0.107.1`.", projectState, StringComparison.Ordinal);
        Assert.Contains("Current `v0.107.0` beta.85/beta.86/beta.87 loader proof remains previous-package/game-version context", projectState, StringComparison.Ordinal);
        Assert.Contains("beta.88 AdditiveBatch1 loader proof is clean for `v0.107.1` loader/registration only but belongs to the previous BaseLib-backed package", projectState, StringComparison.Ordinal);
        Assert.Contains("applied 25/25 Spire Plus ModPatcher patches", projectState, StringComparison.Ordinal);
        Assert.Contains("Found 30 SavedSpireFields", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Found 22 SavedSpireFields", projectState, StringComparison.OrdinalIgnoreCase);
    }
}
