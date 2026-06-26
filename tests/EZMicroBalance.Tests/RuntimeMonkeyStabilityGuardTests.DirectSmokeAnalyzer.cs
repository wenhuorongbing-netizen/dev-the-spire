using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
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
                [ERROR] [previous package] HarmonyLib.HarmonyException: Patching exception in method null
                 ---> System.ArgumentException: Undefined target method for patch method static System.Void DependencyFramework.Patches.Networking.AdjustCustomMessageKeys::Fuckery()
                   at HarmonyLib.PatchClassProcessor.Patch()
                [ERROR] [previous package] HarmonyLib.HarmonyException: Patching exception in method System.Void MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection.NRelicCollectionCategory::LoadRelics(MegaCrit.Sts2.Core.Entities.Relics.RelicRarity relicRarity, MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection.NRelicCollection collection, MegaCrit.Sts2.Core.Localization.LocString header, System.Collections.Generic.HashSet`1<MegaCrit.Sts2.Core.Models.RelicModel> seenRelics, MegaCrit.Sts2.Core.Unlocks.UnlockState unlockState, System.Collections.Generic.HashSet`1<MegaCrit.Sts2.Core.Models.RelicModel> allUnlockedRelics)
                 ---> System.Exception: Failed to find match:
                [INFO] [previous package] Applied 241 patches successfully, 2 failed
                [INFO] [StS1 Events] Registering AdditiveBatch1 events
                """;
            File.WriteAllText(Path.Combine(workdir, "godot.log.before"), "");
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), currentLog);
            File.WriteAllText(currentLogPath, currentLog);

            var auditPath = Path.Combine(workdir, "godot-log-audit.json");
            var auditResult = RunPowerShell(auditScript, "-Path", currentLogPath, "-OutFile", auditPath);
            Assert.True(auditResult.ExitCode == 0, $"Audit helper failed:{Environment.NewLine}{auditResult.Output}{auditResult.Error}");
            var modeReport = WriteDirectSmokeModeVerifierReport(
                workdir,
                "AdditiveBatch1",
                currentLogPath,
                mismatchCount: 2,
                checkCount: 3,
                failedCheckCount: 2);
            var packetReport = WriteDirectSmokePacketVerifierReport(
                workdir,
                "AdditiveBatch1",
                mismatchCount: 1,
                checkCount: 2,
                failedCheckCount: 1);
            File.WriteAllText(
                Path.Combine(workdir, "direct-smoke-summary.json"),
                $$"""
                {
                  "Mode": "AdditiveBatch1",
                  "MainMenuReached": true,
                  "AuditClean": false,
                  "ModeVerifierReportPath": "enabled-mode-log-check.json",
                  "ModeVerifierReportSha256": "{{modeReport.Sha256}}",
                  "ModeVerifierMismatches": 2,
                  "ModeVerifierCheckCount": 3,
                  "ModeVerifierFailedCheckCount": 2,
                  "PacketVerifierReportPath": "runtime-evidence-packet-check.json",
                  "PacketVerifierReportSha256": "{{packetReport.Sha256}}",
                  "PacketVerifierMismatches": 1,
                  "PacketVerifierCheckCount": 2,
                  "PacketVerifierFailedCheckCount": 1,
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
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "audit:dependency framework patch failure");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "audit:Godot ERROR line");
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_mismatch" &&
                    finding.GetProperty("OwnerArea").GetString() == "PackageRuntimeDrift");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_report_binding_invalid");
            var dependencyFrameworkFailures = iteration.GetProperty("DependencyFrameworkFailures").EnumerateArray().ToArray();
            Assert.Contains(dependencyFrameworkFailures, item => item.GetProperty("FailureKind").GetString() == "Undefined target method"
                && item.GetProperty("PatchMethod").GetString() == "static System.Void DependencyFramework.Patches.Networking.AdjustCustomMessageKeys::Fuckery()");
            Assert.Contains(dependencyFrameworkFailures, item => item.GetProperty("FailureKind").GetString() == "Instruction matcher failed"
                && item.GetProperty("TargetMethod").GetString()!.Contains("NRelicCollectionCategory::LoadRelics", StringComparison.Ordinal));
            Assert.Contains(dependencyFrameworkFailures, item => item.GetProperty("FailureKind").GetString() == "Patch summary"
                && item.GetProperty("Summary").GetString() == "Dependency framework applied 241 patches successfully, 2 failed");
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
    public void RuntimeFailureAnalyzerKeepsDirectSmokeVerifierMismatchInHarnessWhenAuditIsStale()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var auditScript = AssertRepoFileExists("scripts", "audit-godot-log.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var currentLogPath = Path.Combine(workdir, "godot.log.current-iteration");
            File.WriteAllText(Path.Combine(workdir, "godot.log.before"), "");
            File.WriteAllText(currentLogPath, "[Startup] Time to main menu\r\n");
            var auditPath = Path.Combine(workdir, "godot-log-audit.json");
            var auditResult = RunPowerShell(auditScript, "-Path", currentLogPath, "-OutFile", auditPath);
            Assert.True(auditResult.ExitCode == 0, $"Audit helper failed:{Environment.NewLine}{auditResult.Output}{auditResult.Error}");

            File.WriteAllText(currentLogPath, "[Startup] Time to main menu\r\n[INFO] later direct smoke line\r\n");
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), File.ReadAllText(currentLogPath));
            var modeReport = WriteDirectSmokeModeVerifierReport(
                workdir,
                "AdditiveBatch1",
                currentLogPath,
                mismatchCount: 1,
                checkCount: 1,
                failedCheckCount: 1);
            var packetReport = WriteDirectSmokePacketVerifierReport(
                workdir,
                "AdditiveBatch1",
                mismatchCount: 0,
                checkCount: 1,
                failedCheckCount: 0);
            File.WriteAllText(
                Path.Combine(workdir, "direct-smoke-summary.json"),
                $$"""
                {
                  "Mode": "AdditiveBatch1",
                  "MainMenuReached": true,
                  "AuditClean": true,
                  "ModeVerifierReportPath": "enabled-mode-log-check.json",
                  "ModeVerifierReportSha256": "{{modeReport.Sha256}}",
                  "ModeVerifierMismatches": 1,
                  "ModeVerifierCheckCount": 1,
                  "ModeVerifierFailedCheckCount": 1,
                  "PacketVerifierReportPath": "runtime-evidence-packet-check.json",
                  "PacketVerifierReportSha256": "{{packetReport.Sha256}}",
                  "PacketVerifierMismatches": 0,
                  "PacketVerifierCheckCount": 1,
                  "PacketVerifierFailedCheckCount": 0,
                  "ExpectedGameVersion": "0.107.1",
                  "Passed": false
                }
                """);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-stale-direct-smoke-audit.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 0);
            var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "godot_log_audit_current_iteration_binding_mismatch" ||
                    finding.GetProperty("Signal").GetString() == "godot_log_audit_recomputed_mismatch");
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_mismatch" &&
                    finding.GetProperty("OwnerArea").GetString() == "RuntimeHarness");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_report_binding_invalid");
            Assert.DoesNotContain(
                root.GetProperty("PackageBlockingFindings").EnumerateArray(),
                finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_mismatch");
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
                    finding.GetProperty("OwnerArea").GetString() == "RuntimeHarness");
            Assert.DoesNotContain(
                root.GetProperty("PackageBlockingFindings").EnumerateArray(),
                finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_mismatch");
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
    public void RuntimeFailureAnalyzerFailsClosedWhenDirectSmokeVerifierReportHashDrifts()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var auditScript = AssertRepoFileExists("scripts", "audit-godot-log.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var currentLogPath = Path.Combine(workdir, "godot.log.current-iteration");
            File.WriteAllText(Path.Combine(workdir, "godot.log.before"), "");
            File.WriteAllText(currentLogPath, "[Startup] Time to main menu\r\n");
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), File.ReadAllText(currentLogPath));

            var auditPath = Path.Combine(workdir, "godot-log-audit.json");
            var auditResult = RunPowerShell(auditScript, "-Path", currentLogPath, "-OutFile", auditPath);
            Assert.True(auditResult.ExitCode == 0, $"Audit helper failed:{Environment.NewLine}{auditResult.Output}{auditResult.Error}");
            var modeReport = WriteDirectSmokeModeVerifierReport(
                workdir,
                "AdditiveBatch1",
                currentLogPath,
                mismatchCount: 1,
                checkCount: 1,
                failedCheckCount: 1);

            File.WriteAllText(
                Path.Combine(workdir, "direct-smoke-summary.json"),
                $$"""
                {
                  "Mode": "AdditiveBatch1",
                  "MainMenuReached": true,
                  "AuditClean": true,
                  "ModeVerifierReportPath": "enabled-mode-log-check.json",
                  "ModeVerifierReportSha256": "0000000000000000000000000000000000000000000000000000000000000000",
                  "ModeVerifierMismatches": 1,
                  "ModeVerifierCheckCount": 1,
                  "ModeVerifierFailedCheckCount": 1,
                  "ExpectedGameVersion": "0.107.1",
                  "Passed": false
                }
                """);

            Assert.NotEqual("0000000000000000000000000000000000000000000000000000000000000000", modeReport.Sha256);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-direct-smoke-report-hash.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 0);
            var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_report_binding_invalid");
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_mismatch" &&
                    finding.GetProperty("OwnerArea").GetString() == "RuntimeHarness");
            Assert.DoesNotContain(
                root.GetProperty("PackageBlockingFindings").EnumerateArray(),
                finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_mismatch");
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
    public void RuntimeFailureAnalyzerFailsClosedWhenDirectSmokeSummarySuppressesBoundReportMismatch()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var auditScript = AssertRepoFileExists("scripts", "audit-godot-log.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var currentLogPath = Path.Combine(workdir, "godot.log.current-iteration");
            File.WriteAllText(Path.Combine(workdir, "godot.log.before"), "");
            File.WriteAllText(currentLogPath, "[Startup] Time to main menu\r\n");
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), File.ReadAllText(currentLogPath));

            var auditPath = Path.Combine(workdir, "godot-log-audit.json");
            var auditResult = RunPowerShell(auditScript, "-Path", currentLogPath, "-OutFile", auditPath);
            Assert.True(auditResult.ExitCode == 0, $"Audit helper failed:{Environment.NewLine}{auditResult.Output}{auditResult.Error}");
            var modeReport = WriteDirectSmokeModeVerifierReport(
                workdir,
                "AdditiveBatch1",
                currentLogPath,
                mismatchCount: 1,
                checkCount: 1,
                failedCheckCount: 1);

            File.WriteAllText(
                Path.Combine(workdir, "direct-smoke-summary.json"),
                $$"""
                {
                  "Mode": "AdditiveBatch1",
                  "MainMenuReached": true,
                  "AuditClean": true,
                  "ModeVerifierReportPath": "enabled-mode-log-check.json",
                  "ModeVerifierReportSha256": "{{modeReport.Sha256}}",
                  "ModeVerifierMismatches": 0,
                  "ModeVerifierCheckCount": 1,
                  "ModeVerifierFailedCheckCount": 1,
                  "ExpectedGameVersion": "0.107.1",
                  "Passed": false
                }
                """);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-direct-smoke-suppressed-report.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 0);
            var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_report_binding_invalid" &&
                    finding.GetProperty("Rationale").GetString()!.Contains("ModeVerifierMismatches must match retained report Mismatches.Count", StringComparison.Ordinal));
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_mismatch" &&
                    finding.GetProperty("OwnerArea").GetString() == "RuntimeHarness");
            Assert.DoesNotContain(
                root.GetProperty("PackageBlockingFindings").EnumerateArray(),
                finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_mismatch");
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
    public void RuntimeFailureAnalyzerFailsClosedWhenDirectSmokeVerifierReportEscapesEvidenceRoot()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var auditScript = AssertRepoFileExists("scripts", "audit-godot-log.ps1");
        var parent = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        var workdir = Path.Combine(parent, "evidence");
        Directory.CreateDirectory(workdir);

        try
        {
            var currentLogPath = Path.Combine(workdir, "godot.log.current-iteration");
            File.WriteAllText(Path.Combine(workdir, "godot.log.before"), "");
            File.WriteAllText(currentLogPath, "[Startup] Time to main menu\r\n");
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), File.ReadAllText(currentLogPath));

            var auditPath = Path.Combine(workdir, "godot-log-audit.json");
            var auditResult = RunPowerShell(auditScript, "-Path", currentLogPath, "-OutFile", auditPath);
            Assert.True(auditResult.ExitCode == 0, $"Audit helper failed:{Environment.NewLine}{auditResult.Output}{auditResult.Error}");

            var escapedReportPath = Path.Combine(parent, "enabled-mode-log-check.json");
            var escapedReport = new JsonObject
            {
                ["Mode"] = "AdditiveBatch1",
                ["LogPath"] = Path.GetFullPath(currentLogPath),
                ["LogLength"] = new FileInfo(currentLogPath).Length,
                ["LogSha256"] = GetSha256(currentLogPath),
                ["Checks"] = CreateDirectSmokeChecks(checkCount: 1, failedCheckCount: 1),
                ["Mismatches"] = CreateDirectSmokeMismatches(mismatchCount: 1),
            };
            File.WriteAllText(escapedReportPath, escapedReport.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            File.WriteAllText(
                Path.Combine(workdir, "direct-smoke-summary.json"),
                $$"""
                {
                  "Mode": "AdditiveBatch1",
                  "MainMenuReached": true,
                  "AuditClean": true,
                  "ModeVerifierReportPath": "../enabled-mode-log-check.json",
                  "ModeVerifierReportSha256": "{{GetSha256(escapedReportPath)}}",
                  "ModeVerifierMismatches": 1,
                  "ModeVerifierCheckCount": 1,
                  "ModeVerifierFailedCheckCount": 1,
                  "ExpectedGameVersion": "0.107.1",
                  "Passed": false
                }
                """);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-direct-smoke-report-escape.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 0);
            var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_report_binding_invalid" &&
                    finding.GetProperty("Rationale").GetString()!.Contains("ModeVerifierReportPath must stay inside the DirectSmoke evidence root", StringComparison.Ordinal));
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_mismatch" &&
                    finding.GetProperty("OwnerArea").GetString() == "RuntimeHarness");
            Assert.DoesNotContain(
                root.GetProperty("PackageBlockingFindings").EnumerateArray(),
                finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_mismatch");
        }
        finally
        {
            if (Directory.Exists(parent))
            {
                Directory.Delete(parent, recursive: true);
            }
        }
    }

    [Fact]
    public void RuntimeFailureAnalyzerAnalyzesDirectSmokeEvenWithStaleAutoSlaySummary()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        File.WriteAllText(
            Path.Combine(fixture.Workdir, "direct-smoke-summary.json"),
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

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-mixed-direct-smoke.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);
        Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var directSmokeIteration = FindIteration(root, 0);
        var directSmokeFindings = directSmokeIteration.GetProperty("Findings").EnumerateArray().ToArray();

        Assert.True(root.GetProperty("AnalyzedIterationCount").GetInt32() >= 2);
        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal("DirectSmoke", directSmokeIteration.GetProperty("RunnerKind").GetString());
        Assert.Equal("direct-smoke", directSmokeIteration.GetProperty("ScenarioTag").GetString());
        Assert.Contains(directSmokeFindings, finding => finding.GetProperty("Signal").GetString() == "direct_smoke_current_iteration_log_missing");
        Assert.Contains(
            directSmokeFindings,
            finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_mismatch" &&
                finding.GetProperty("OwnerArea").GetString() == "RuntimeHarness");
        Assert.Contains(
            root.GetProperty("HarnessBlockingFindings").EnumerateArray(),
            finding => finding.GetProperty("Signal").GetString() == "direct_smoke_current_iteration_log_missing");
        Assert.DoesNotContain(
            root.GetProperty("PackageBlockingFindings").EnumerateArray(),
            finding => finding.GetProperty("Signal").GetString() == "direct_smoke_verifier_mismatch");
    }

    [Fact]
    public void RuntimeFailureAnalyzerFailsClosedOnMalformedDirectSmokeSummaryEvenWithStaleAutoSlaySummary()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        File.WriteAllText(Path.Combine(fixture.Workdir, "direct-smoke-summary.json"), "{");

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-mixed-malformed-direct-smoke.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);
        Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var directSmokeIteration = FindIteration(root, 0);
        var directSmokeFindings = directSmokeIteration.GetProperty("Findings").EnumerateArray().ToArray();

        Assert.True(root.GetProperty("AnalyzedIterationCount").GetInt32() >= 2);
        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal("DirectSmoke", directSmokeIteration.GetProperty("RunnerKind").GetString());
        Assert.Equal("direct-smoke", directSmokeIteration.GetProperty("ScenarioTag").GetString());
        Assert.Contains(directSmokeFindings, finding => finding.GetProperty("Signal").GetString() == "iteration_result_missing_or_invalid");
        Assert.Contains(
            root.GetProperty("HarnessBlockingFindings").EnumerateArray(),
            finding => finding.GetProperty("Signal").GetString() == "iteration_result_missing_or_invalid");
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsDirectSmokeOutFileOverCanonicalEvidence()
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
                  "AuditClean": true,
                  "ModeVerifierMismatches": 0,
                  "PacketVerifierMismatches": 0,
                  "ExpectedGameVersion": "0.107.1",
                  "Passed": true
                }
                """);

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-OutFile",
                Path.Combine(workdir, "direct-smoke-summary.json"));

            Assert.NotEqual(0, result.ExitCode);
            Assert.Contains("Refusing to write OutFile over canonical runtime evidence", result.Output + result.Error, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
    }

    private static (string Path, string Sha256) WriteDirectSmokeModeVerifierReport(
        string workdir,
        string mode,
        string currentLogPath,
        int mismatchCount,
        int checkCount,
        int failedCheckCount)
    {
        var reportPath = Path.Combine(workdir, "enabled-mode-log-check.json");
        var report = new JsonObject
        {
            ["Mode"] = mode,
            ["LogPath"] = Path.GetFullPath(currentLogPath),
            ["LogLength"] = new FileInfo(currentLogPath).Length,
            ["LogSha256"] = GetSha256(currentLogPath),
            ["Checks"] = CreateDirectSmokeChecks(checkCount, failedCheckCount),
            ["Mismatches"] = CreateDirectSmokeMismatches(mismatchCount),
        };

        File.WriteAllText(reportPath, report.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        return (reportPath, GetSha256(reportPath));
    }

    private static (string Path, string Sha256) WriteDirectSmokePacketVerifierReport(
        string workdir,
        string mode,
        int mismatchCount,
        int checkCount,
        int failedCheckCount)
    {
        var reportPath = Path.Combine(workdir, "runtime-evidence-packet-check.json");
        var report = new JsonObject
        {
            ["EvidenceDir"] = Path.GetFullPath(workdir),
            ["Mode"] = mode,
            ["Checks"] = CreateDirectSmokeChecks(checkCount, failedCheckCount),
            ["Mismatches"] = CreateDirectSmokeMismatches(mismatchCount),
        };

        File.WriteAllText(reportPath, report.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        return (reportPath, GetSha256(reportPath));
    }

    private static JsonArray CreateDirectSmokeChecks(int checkCount, int failedCheckCount)
    {
        var checks = new JsonArray();
        for (var index = 0; index < checkCount; index++)
        {
            checks.Add(new JsonObject
            {
                ["Name"] = $"direct_smoke_check_{index + 1}",
                ["Passed"] = index >= failedCheckCount,
                ["Detail"] = "synthetic retained DirectSmoke verifier report",
            });
        }

        return checks;
    }

    private static JsonArray CreateDirectSmokeMismatches(int mismatchCount)
    {
        var mismatches = new JsonArray();
        for (var index = 0; index < mismatchCount; index++)
        {
            mismatches.Add($"direct smoke mismatch {index + 1}");
        }

        return mismatches;
    }

    private static string GetSha256(string path)
    {
        return Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
    }
}
