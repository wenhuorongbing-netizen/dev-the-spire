using System.Security.Cryptography;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class RuntimeMonkeyStabilityGuardTests
{
    [Fact]
    public void Sts1RuntimeEvidencePacketVerifierRejectsStaleFullLogPrefixForEnabledMode()
    {
        var packetVerifier = AssertRepoFileExists("scripts", "check-sts1-runtime-evidence-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "sts1-runtime-packet-verifier-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteSts1RuntimePacketState(workdir, mode: "AdditiveBatch1");
            var stalePrefix = BuildSts1ModeRuntimeLog("AdditiveBatch1");
            var currentOffSlice = """
                v0.1.0-private-beta.86
                release = v0.107.0
                RitsuLib Version: 0.4.16 [compat branch: 0.107.0]
                Feature Sts1Events bootstrap=disabled, live=Disabled
                """;
            File.WriteAllText(Path.Combine(workdir, "godot.log.before"), stalePrefix);
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), stalePrefix + currentOffSlice);

            var result = RunPowerShell(
                packetVerifier,
                "-Mode",
                "AdditiveBatch1",
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.86",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.16",
                "-ExpectedGameVersion",
                "0.107.0",
                "-OutFile",
                Path.Combine(workdir, "runtime-evidence-packet-check.json"));

            Assert.True(result.ExitCode == 0, $"Packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("enabled_mode_log_verifier_uses_current_slice status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("full_log_not_used_as_canonical_verifier_input status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("current_slice_derived_from_before_after status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("enabled_mode_log_verifier_clean status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("log_verifier log_path=", result.Output, StringComparison.Ordinal);
            Assert.Contains("godot.log.current-iteration", result.Output, StringComparison.Ordinal);
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
    public void Sts1RuntimeEvidencePacketVerifierRejectsRetainedCurrentSliceThatDoesNotMatchBeforeAfter()
    {
        var packetVerifier = AssertRepoFileExists("scripts", "check-sts1-runtime-evidence-packet.ps1");
        var auditScript = AssertRepoFileExists("scripts", "audit-godot-log.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "sts1-runtime-packet-verifier-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteSts1RuntimePacketState(workdir, mode: "AdditiveBatch1");
            const string preLaunchPrefix = "[Startup] retained pre-launch log prefix\r\n";
            var actualOffSlice = """
                v0.1.0-private-beta.86
                release = v0.107.0
                RitsuLib Version: 0.4.16 [compat branch: 0.107.0]
                Feature Sts1Events bootstrap=disabled, live=Disabled
                """;
            var retainedStaleCurrentSlice = BuildSts1ModeRuntimeLog("AdditiveBatch1");
            var retainedCurrentSlicePath = Path.Combine(workdir, "godot.log.current-iteration");
            var retainedCurrentAuditPath = Path.Combine(workdir, "godot-log-current-iteration-audit.json");
            File.WriteAllText(Path.Combine(workdir, "godot.log.before"), preLaunchPrefix);
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), preLaunchPrefix + actualOffSlice);
            File.WriteAllText(retainedCurrentSlicePath, retainedStaleCurrentSlice);

            var auditResult = RunPowerShell(auditScript, "-Path", retainedCurrentSlicePath, "-OutFile", retainedCurrentAuditPath);
            Assert.True(auditResult.ExitCode == 0, $"Audit failed:{Environment.NewLine}{auditResult.Output}{auditResult.Error}");

            var result = RunPowerShell(
                packetVerifier,
                "-Mode",
                "AdditiveBatch1",
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.86",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.16",
                "-ExpectedGameVersion",
                "0.107.0",
                "-OutFile",
                Path.Combine(workdir, "runtime-evidence-packet-check.json"));

            Assert.True(result.ExitCode == 0, $"Packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("current_slice_matches_before_after status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("enabled_mode_log_verifier_uses_current_slice status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("enabled_mode_log_verifier_clean status=pass", result.Output, StringComparison.Ordinal);

            using var report = JsonDocument.Parse(File.ReadAllText(Path.Combine(workdir, "runtime-evidence-packet-check.json")));
            Assert.False(report.RootElement.GetProperty("CurrentSliceMatchesBeforeAfter").GetBoolean());
            Assert.Contains("current slice", report.RootElement.GetProperty("CurrentSliceBindingDetail").GetString(), StringComparison.Ordinal);
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
    public void Sts1RuntimeEvidencePacketVerifierDerivesAndAuditsCurrentSlice()
    {
        var packetVerifier = AssertRepoFileExists("scripts", "check-sts1-runtime-evidence-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "sts1-runtime-packet-verifier-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteSts1RuntimePacketState(workdir, mode: "AdditiveBatch1");
            const string stalePrefix = "[Startup] stale pre-launch log prefix without StS1 registrations\r\n";
            var currentSlice = BuildSts1ModeRuntimeLog("AdditiveBatch1");
            File.WriteAllText(Path.Combine(workdir, "godot.log.before"), stalePrefix);
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), stalePrefix + currentSlice);

            var result = RunPowerShell(
                packetVerifier,
                "-Mode",
                "AdditiveBatch1",
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.86",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.16",
                "-ExpectedGameVersion",
                "0.107.0",
                "-OutFile",
                Path.Combine(workdir, "runtime-evidence-packet-check.json"),
                "-FailOnMismatch");

            Assert.True(result.ExitCode == 0, $"Packet verifier failed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("derived_current_slice_audit_generated status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("enabled_mode_log_verifier_clean status=pass", result.Output, StringComparison.Ordinal);

            using var report = JsonDocument.Parse(File.ReadAllText(Path.Combine(workdir, "runtime-evidence-packet-check.json")));
            Assert.True(report.RootElement.GetProperty("CurrentSliceDerivedFromBeforeAfter").GetBoolean());
            Assert.EndsWith("godot.log.current-iteration", report.RootElement.GetProperty("CanonicalLogPath").GetString(), StringComparison.OrdinalIgnoreCase);
            Assert.EndsWith("godot-log-current-iteration-audit.json", report.RootElement.GetProperty("CanonicalAuditPath").GetString(), StringComparison.OrdinalIgnoreCase);
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
    public void Sts1EnabledModeLogVerifierRecomputesAuditFromCopiedLog()
    {
        var verifier = AssertRepoFileExists("scripts", "check-sts1-enabled-mode-runtime-log.ps1");
        var auditScript = AssertRepoFileExists("scripts", "audit-godot-log.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "sts1-enabled-mode-log-verifier-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var logPath = Path.Combine(workdir, "godot.log.after-launch");
            var auditPath = Path.Combine(workdir, "godot-log-audit.json");
            var cleanLog = """
                StS1 events default Off; set SPIREPLUS_STS1_EVENT_MODE to enable.
                Feature Sts1Events bootstrap=disabled, live=Disabled
                """;
            File.WriteAllText(logPath, cleanLog);

            var cleanAudit = RunPowerShell(auditScript, "-Path", logPath, "-OutFile", auditPath);
            Assert.True(cleanAudit.ExitCode == 0, $"Audit failed:{Environment.NewLine}{cleanAudit.Output}{cleanAudit.Error}");

            var cleanResult = RunPowerShell(verifier, "-Mode", "Off", "-LogPath", logPath, "-AuditPath", auditPath);
            Assert.True(cleanResult.ExitCode == 0, $"Verifier crashed:{Environment.NewLine}{cleanResult.Output}{cleanResult.Error}");
            Assert.Contains("mismatches=0", cleanResult.Output, StringComparison.Ordinal);

            var dirtyLog = cleanLog + Environment.NewLine + "[ERROR] TypeLoadException" + Environment.NewLine;
            File.WriteAllText(logPath, dirtyLog);
            var dirtyLogLength = new FileInfo(logPath).Length;
            var dirtyLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(logPath))).ToLowerInvariant();
            File.WriteAllText(
                auditPath,
                $$"""
                {
                  "Path": {{JsonSerializer.Serialize(logPath)}},
                  "Length": {{dirtyLogLength}},
                  "Sha256": {{JsonSerializer.Serialize(dirtyLogHash)}},
                  "Clean": true,
                  "SignatureHits": []
                }
                """);

            var dirtyResult = RunPowerShell(verifier, "-Mode", "Off", "-LogPath", logPath, "-AuditPath", auditPath);
            Assert.True(dirtyResult.ExitCode == 0, $"Verifier crashed:{Environment.NewLine}{dirtyResult.Output}{dirtyResult.Error}");
            Assert.Contains("audit_recomputed_clean status=fail", dirtyResult.Output, StringComparison.Ordinal);
            Assert.Contains("audit_signature_counts_match_recomputed status=fail", dirtyResult.Output, StringComparison.Ordinal);
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
