using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class RuntimeMonkeyStabilityGuardTests
{
    [Fact]
    public void RuntimeFailureAnalyzerRoutesDirectSmokeDirtyAuditToPackageRuntimeDrift()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var auditScript = AssertRepoFileExists("scripts", "audit-godot-log.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var currentLogPath = Path.Combine(workdir, "godot.log.current-iteration");
            var currentLog = """
                [INFO] [EZMicroBalance] [Patcher - SpirePlus] Patch application complete: 25 applied, 0 ignored, 0 failed, 25 total
                [ERROR] [BaseLib] HarmonyLib.HarmonyException: Patching exception in method null
                 ---> System.ArgumentException: Undefined target method for patch method static System.Void BaseLib.Patches.Networking.AdjustCustomMessageKeys::Fuckery()
                   at HarmonyLib.PatchClassProcessor.Patch()
                [ERROR] [BaseLib] HarmonyLib.HarmonyException: Patching exception in method System.Void MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection.NRelicCollectionCategory::LoadRelics(MegaCrit.Sts2.Core.Entities.Relics.RelicRarity relicRarity, MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection.NRelicCollection collection, MegaCrit.Sts2.Core.Localization.LocString header, System.Collections.Generic.HashSet`1<MegaCrit.Sts2.Core.Models.RelicModel> seenRelics, MegaCrit.Sts2.Core.Unlocks.UnlockState unlockState, System.Collections.Generic.HashSet`1<MegaCrit.Sts2.Core.Models.RelicModel> allUnlockedRelics)
                 ---> System.Exception: Failed to find match:
                [INFO] [BaseLib] Applied 241 patches successfully, 2 failed
                [INFO] [StS1 Events] Registering AdditiveBatch1 events
                """;
            File.WriteAllText(Path.Combine(workdir, "godot.log.before"), "");
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), currentLog);
            File.WriteAllText(currentLogPath, currentLog);

            var auditPath = Path.Combine(workdir, "godot-log-audit.json");
            var auditResult = RunPowerShell(auditScript, "-Path", currentLogPath, "-OutFile", auditPath);
            Assert.True(auditResult.ExitCode == 0, $"Audit helper failed:{Environment.NewLine}{auditResult.Output}{auditResult.Error}");
            File.WriteAllText(
                Path.Combine(workdir, "direct-smoke-summary.json"),
                """
                {
                  "Mode": "AdditiveBatch1",
                  "MainMenuReached": true,
                  "AuditClean": false,
                  "ModeVerifierMismatches": 2,
                  "PacketVerifierMismatches": 1,
                  "ExpectedGameVersion": "0.107.1",
                  "Passed": false
                }
                """);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(document.RootElement, 0);
            var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

            Assert.Equal(1, root.GetProperty("AnalyzedIterationCount").GetInt32());
            Assert.Equal("PackageRuntimeDrift", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal("DirectSmoke", iteration.GetProperty("RunnerKind").GetString());
            Assert.Equal("direct-smoke", iteration.GetProperty("ScenarioTag").GetString());
            Assert.Equal("PackageRuntimeDrift", iteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "audit:BaseLib patch failure");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "audit:Godot ERROR line");
            var baseLibPatchFailures = iteration.GetProperty("BaseLibPatchFailures").EnumerateArray().ToArray();
            Assert.Contains(baseLibPatchFailures, item => item.GetProperty("FailureKind").GetString() == "Undefined target method"
                && item.GetProperty("PatchMethod").GetString() == "static System.Void BaseLib.Patches.Networking.AdjustCustomMessageKeys::Fuckery()");
            Assert.Contains(baseLibPatchFailures, item => item.GetProperty("FailureKind").GetString() == "Instruction matcher failed"
                && item.GetProperty("TargetMethod").GetString()!.Contains("NRelicCollectionCategory::LoadRelics", StringComparison.Ordinal));
            Assert.Contains(baseLibPatchFailures, item => item.GetProperty("FailureKind").GetString() == "Patch summary"
                && item.GetProperty("Summary").GetString() == "BaseLib applied 241 patches successfully, 2 failed");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "iteration_result_missing_or_invalid");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "current_iteration_log_offset_binding_missing");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "coop_override_enabled_runtime_failure");
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
    }

    [Fact]
    public void RuntimeFailureAnalyzerFailsClosedOnIncompleteDirectSmokeSummaryEvidence()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            File.WriteAllText(
                Path.Combine(workdir, "direct-smoke-summary.json"),
                """
                {
                  "Mode": "AdditiveBatch1",
                  "MainMenuReached": true,
                  "AuditClean": false,
                  "ModeVerifierMismatches": 2,
                  "PacketVerifierMismatches": 1,
                  "ExpectedGameVersion": "0.107.1",
                  "Passed": false
                }
                """);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 0);
            var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

            Assert.Equal(1, root.GetProperty("AnalyzedIterationCount").GetInt32());
            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal("DirectSmoke", iteration.GetProperty("RunnerKind").GetString());
            Assert.Equal("direct-smoke", iteration.GetProperty("ScenarioTag").GetString());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "direct_smoke_current_iteration_log_missing");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "direct_smoke_godot_log_audit_missing");
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_mismatch" &&
                    finding.GetProperty("OwnerArea").GetString() == "PackageRuntimeDrift");
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
    }
}
