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
                v0.1.0-private-beta.87
                release = v0.107.0
                RitsuLib Version: 0.4.24 [compat branch: 0.107.0]
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
                "v0.1.0-private-beta.87",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
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
                v0.1.0-private-beta.87
                release = v0.107.0
                RitsuLib Version: 0.4.24 [compat branch: 0.107.0]
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
                "v0.1.0-private-beta.87",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
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
                "v0.1.0-private-beta.87",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
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
    public void Sts1RuntimeEvidencePacketVerifierReportsMalformedMovedModPathsAsFailedRows()
    {
        var packetVerifier = AssertRepoFileExists("scripts", "check-sts1-runtime-evidence-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "sts1-runtime-packet-verifier-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteSts1RuntimePacketState(workdir, mode: "AdditiveBatch1");
            const string preLaunchPrefix = "[Startup] clean pre-launch log prefix\r\n";
            var currentSlice = BuildSts1ModeRuntimeLog("AdditiveBatch1");
            File.WriteAllText(Path.Combine(workdir, "godot.log.before"), preLaunchPrefix);
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), preLaunchPrefix + currentSlice);
            File.WriteAllText(
                Path.Combine(workdir, "session-state.json"),
                $$"""
                {
                  "AllowedModIds": ["BaseLib", "STS2-RitsuLib", "EZMicroBalance"],
                  "DisableSpirePlus": false,
                  "MoveOtherMods": true,
                  "MoveCurrentRuns": true,
                  "MovedMods": [
                    {
                      "Name": "OtherMod",
                      "From": "\u0000bad-from",
                      "To": "\u0000bad-to"
                    },
                    {
                      "Name": "MissingPaths"
                    }
                  ],
                  "MovedCurrentRuns": [],
                  "GameRoot": {{JsonSerializer.Serialize(workdir)}},
                  "ModsRoot": {{JsonSerializer.Serialize(Path.Combine(workdir, "mods"))}},
                  "LogPath": {{JsonSerializer.Serialize(Path.Combine(workdir, "godot.log.after-launch"))}},
                  "Sts1EventModeEnvironment": "AdditiveBatch1",
                  "Sts1UnsafeModeEnvironment": ""
                }
                """);
            File.WriteAllText(
                Path.Combine(workdir, "restore-state.json"),
                """
                {
                  "RestoredAt": "2026-06-18T00:00:00.0000000Z",
                  "RestoredModCount": 2,
                  "RestoredCurrentRunCount": 0,
                  "SettingsHashAfterRestore": "same",
                  "SettingsBackupHashAfterRestore": "same"
                }
                """);

            var outFile = Path.Combine(workdir, "runtime-evidence-packet-check.json");
            var result = RunPowerShell(
                packetVerifier,
                "-Mode",
                "AdditiveBatch1",
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedGameVersion",
                "0.107.0",
                "-OutFile",
                outFile);

            Assert.True(result.ExitCode == 0, $"Packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("session_moved_mod_sources_under_mods_root status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("session_moved_mod_destinations_under_isolated_mods status=fail", result.Output, StringComparison.Ordinal);
            Assert.True(File.Exists(outFile), $"Packet verifier did not retain report:{Environment.NewLine}{result.Output}{result.Error}");
            using var report = JsonDocument.Parse(File.ReadAllText(outFile));
            var checks = report.RootElement.GetProperty("Checks").EnumerateArray().ToArray();
            Assert.Contains(checks, check => check.GetProperty("Name").GetString() == "session_moved_mod_sources_under_mods_root" && !check.GetProperty("Passed").GetBoolean());
            Assert.Contains(checks, check => check.GetProperty("Name").GetString() == "session_moved_mod_destinations_under_isolated_mods" && !check.GetProperty("Passed").GetBoolean());
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
    public void Sts1RuntimeEvidencePacketVerifierRejectsAllowedMovedModSourceSpoof()
    {
        var packetVerifier = AssertRepoFileExists("scripts", "check-sts1-runtime-evidence-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "sts1-runtime-packet-verifier-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteSts1RuntimePacketState(workdir, mode: "AdditiveBatch1");
            const string preLaunchPrefix = "[Startup] clean pre-launch log prefix\r\n";
            var currentSlice = BuildSts1ModeRuntimeLog("AdditiveBatch1");
            File.WriteAllText(Path.Combine(workdir, "godot.log.before"), preLaunchPrefix);
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), preLaunchPrefix + currentSlice);

            var sourcePath = Path.Combine(workdir, "mods", "BaseLib");
            var destinationPath = Path.Combine(workdir, "isolated-mods", "OtherMod");
            File.WriteAllText(
                Path.Combine(workdir, "session-state.json"),
                $$"""
                {
                  "AllowedModIds": ["BaseLib", "STS2-RitsuLib", "EZMicroBalance"],
                  "DisableSpirePlus": false,
                  "MoveOtherMods": true,
                  "MoveCurrentRuns": true,
                  "MovedMods": [
                    {
                      "Name": "OtherMod",
                      "From": {{JsonSerializer.Serialize(sourcePath)}},
                      "To": {{JsonSerializer.Serialize(destinationPath)}}
                    }
                  ],
                  "MovedCurrentRuns": [],
                  "GameRoot": {{JsonSerializer.Serialize(workdir)}},
                  "ModsRoot": {{JsonSerializer.Serialize(Path.Combine(workdir, "mods"))}},
                  "LogPath": {{JsonSerializer.Serialize(Path.Combine(workdir, "godot.log.after-launch"))}},
                  "Sts1EventModeEnvironment": "AdditiveBatch1",
                  "Sts1UnsafeModeEnvironment": ""
                }
                """);
            File.WriteAllText(
                Path.Combine(workdir, "restore-state.json"),
                """
                {
                  "RestoredAt": "2026-06-18T00:00:00.0000000Z",
                  "RestoredModCount": 1,
                  "RestoredCurrentRunCount": 0,
                  "SettingsHashAfterRestore": "same",
                  "SettingsBackupHashAfterRestore": "same"
                }
                """);

            var outFile = Path.Combine(workdir, "runtime-evidence-packet-check.json");
            var result = RunPowerShell(
                packetVerifier,
                "-Mode",
                "AdditiveBatch1",
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedGameVersion",
                "0.107.0",
                "-OutFile",
                outFile);

            Assert.True(result.ExitCode == 0, $"Packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("session_moved_mods_do_not_include_allowed_mods status=fail", result.Output, StringComparison.Ordinal);
            Assert.True(File.Exists(outFile), $"Packet verifier did not retain report:{Environment.NewLine}{result.Output}{result.Error}");
            using var report = JsonDocument.Parse(File.ReadAllText(outFile));
            var checks = report.RootElement.GetProperty("Checks").EnumerateArray().ToArray();
            Assert.Contains(checks, check => check.GetProperty("Name").GetString() == "session_moved_mods_do_not_include_allowed_mods" && !check.GetProperty("Passed").GetBoolean());
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
    public void Sts1RuntimeEvidencePacketVerifierRejectsSpoofedModsRoot()
    {
        var packetVerifier = AssertRepoFileExists("scripts", "check-sts1-runtime-evidence-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "sts1-runtime-packet-verifier-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteSts1RuntimePacketState(workdir, mode: "Off");
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), "v0.1.0-private-beta.87\r\n");

            var spoofedModsRoot = Path.GetPathRoot(workdir) ?? workdir;
            File.WriteAllText(
                Path.Combine(workdir, "session-state.json"),
                $$"""
                {
                  "AllowedModIds": ["BaseLib", "STS2-RitsuLib", "EZMicroBalance"],
                  "DisableSpirePlus": false,
                  "MoveOtherMods": true,
                  "MoveCurrentRuns": true,
                  "MovedMods": [],
                  "MovedCurrentRuns": [],
                  "GameRoot": {{JsonSerializer.Serialize(workdir)}},
                  "ModsRoot": {{JsonSerializer.Serialize(spoofedModsRoot)}},
                  "LogPath": {{JsonSerializer.Serialize(Path.Combine(workdir, "godot.log.after-launch"))}},
                  "Sts1EventModeEnvironment": "Off",
                  "Sts1UnsafeModeEnvironment": ""
                }
                """);

            var result = RunPowerShell(
                packetVerifier,
                "-Mode",
                "Off",
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-OutFile",
                Path.Combine(workdir, "runtime-evidence-packet-check.json"));

            Assert.True(result.ExitCode == 0, $"Packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("session_has_mods_root status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("session_mods_root_matches_game_root status=fail", result.Output, StringComparison.Ordinal);
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
    public void Sts1RuntimeEvidencePacketVerifierReportsInvalidMovedCurrentRunRows()
    {
        var packetVerifier = AssertRepoFileExists("scripts", "check-sts1-runtime-evidence-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "sts1-runtime-packet-verifier-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteSts1RuntimePacketState(workdir, mode: "AdditiveBatch1");
            const string preLaunchPrefix = "[Startup] clean pre-launch log prefix\r\n";
            var currentSlice = BuildSts1ModeRuntimeLog("AdditiveBatch1");
            File.WriteAllText(Path.Combine(workdir, "godot.log.before"), preLaunchPrefix);
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), preLaunchPrefix + currentSlice);

            var steamSaves = Path.Combine(workdir, "steam-saves");
            var defaultSaves = Path.Combine(workdir, "default-saves");
            File.WriteAllText(
                Path.Combine(workdir, "session-state.json"),
                $$"""
                {
                  "AllowedModIds": ["BaseLib", "STS2-RitsuLib", "EZMicroBalance"],
                  "DisableSpirePlus": false,
                  "MoveOtherMods": true,
                  "MoveCurrentRuns": true,
                  "MovedMods": [],
                  "MovedCurrentRuns": [
                    {
                      "Name": "unexpected.save",
                      "From": "\u0000bad-run-from",
                      "To": "\u0000bad-run-to"
                    }
                  ],
                  "SteamSaves": [{{JsonSerializer.Serialize(steamSaves)}}],
                  "DefaultSaves": [{{JsonSerializer.Serialize(defaultSaves)}}],
                  "GameRoot": {{JsonSerializer.Serialize(workdir)}},
                  "ModsRoot": {{JsonSerializer.Serialize(Path.Combine(workdir, "mods"))}},
                  "LogPath": {{JsonSerializer.Serialize(Path.Combine(workdir, "godot.log.after-launch"))}},
                  "Sts1EventModeEnvironment": "AdditiveBatch1",
                  "Sts1UnsafeModeEnvironment": ""
                }
                """);
            File.WriteAllText(
                Path.Combine(workdir, "restore-state.json"),
                """
                {
                  "RestoredAt": "2026-06-18T00:00:00.0000000Z",
                  "RestoredModCount": 0,
                  "RestoredCurrentRunCount": 1,
                  "SettingsHashAfterRestore": "same",
                  "SettingsBackupHashAfterRestore": "same"
                }
                """);

            var result = RunPowerShell(
                packetVerifier,
                "-Mode",
                "AdditiveBatch1",
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedGameVersion",
                "0.107.0",
                "-OutFile",
                Path.Combine(workdir, "runtime-evidence-packet-check.json"));

            Assert.True(result.ExitCode == 0, $"Packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("session_moved_current_run_names_allowed status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("session_moved_current_run_sources_under_save_roots status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("session_moved_current_run_destinations_under_removed_runs status=fail", result.Output, StringComparison.Ordinal);
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
    public void Sts1RuntimeEvidencePacketVerifierReportsMissingMovedModsAndEmptyLogAsFailedRows()
    {
        var packetVerifier = AssertRepoFileExists("scripts", "check-sts1-runtime-evidence-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "sts1-runtime-packet-verifier-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteSts1RuntimePacketState(workdir, mode: "Off");
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), string.Empty);
            File.WriteAllText(
                Path.Combine(workdir, "session-state.json"),
                $$"""
                {
                  "AllowedModIds": ["BaseLib", "STS2-RitsuLib", "EZMicroBalance"],
                  "DisableSpirePlus": false,
                  "MoveOtherMods": true,
                  "MoveCurrentRuns": true,
                  "GameRoot": {{JsonSerializer.Serialize(workdir)}},
                  "LogPath": {{JsonSerializer.Serialize(Path.Combine(workdir, "godot.log.after-launch"))}},
                  "Sts1EventModeEnvironment": "Off",
                  "Sts1UnsafeModeEnvironment": ""
                }
                """);
            File.WriteAllText(
                Path.Combine(workdir, "restore-state.json"),
                """
                {
                  "RestoredAt": "2026-06-18T00:00:00.0000000Z",
                  "RestoredModCount": 0,
                  "RestoredCurrentRunCount": 0
                }
                """);

            var outFile = Path.Combine(workdir, "runtime-evidence-packet-check.json");
            var result = RunPowerShell(
                packetVerifier,
                "-Mode",
                "Off",
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-OutFile",
                outFile);

            Assert.True(result.ExitCode == 0, $"Packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("godot_log_non_empty status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("expected_package_version_in_log status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("session_moved_mods_field_recorded status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("session_moved_current_runs_field_recorded status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("session_has_mods_root status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("session_mods_root_matches_game_root status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("restore_hashes_recorded status=fail", result.Output, StringComparison.Ordinal);
            Assert.True(File.Exists(outFile), $"Packet verifier did not retain report:{Environment.NewLine}{result.Output}{result.Error}");
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

    [Fact]
    public void Sts1EnabledModeLogVerifierComparesRegistrationTupleMultiplicity()
    {
        var verifier = AssertRepoFileExists("scripts", "check-sts1-enabled-mode-runtime-log.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "sts1-enabled-mode-log-verifier-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var registrationServicePath = Path.Combine(workdir, "Sts1EventRegistrationService.cs");
            var logPath = Path.Combine(workdir, "godot.log.after-launch");
            File.WriteAllText(
                registrationServicePath,
                """
                public static void RegisterAdditiveBatch1()
                {
                    content.ActEvent<Overgrowth, Sts1Foo>();
                    content.ActEvent<Overgrowth, Sts1Foo>();
                    content.ActEvent<Overgrowth, Sts1Bar>();
                    content.Apply();
                }
                """);
            File.WriteAllText(
                logPath,
                """
                v0.1.0-private-beta.87
                release = v0.107.0
                RitsuLib Version: 0.4.24 [compat branch: 0.107.0]
                Feature Sts1Events bootstrap=enabled, live=Enabled
                StS1 events AdditiveBatch1 mode: registering 10 verified-scope events.
                [StS1 Events] Registering AdditiveBatch1 events
                [StS1 Events] Registered act event: Sts1Foo -> Overgrowth
                [StS1 Events] Registered act event: Sts1Bar -> Overgrowth
                [StS1 Events] Registered act event: Sts1Bar -> Overgrowth
                [StS1 Events] AdditiveBatch1 events registered successfully.
                """);

            var result = RunPowerShell(
                verifier,
                "-Mode",
                "AdditiveBatch1",
                "-LogPath",
                logPath,
                "-RegistrationServicePath",
                registrationServicePath,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedGameVersion",
                "0.107.0");

            Assert.True(result.ExitCode == 0, $"Verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("observed_registration_call_count status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("observed_event_classes_match_expected status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("observed_registration_tuples_match_expected status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("ActEvent:Overgrowth:Sts1Foo expected=2 observed=1", result.Output, StringComparison.Ordinal);
            Assert.Contains("ActEvent:Overgrowth:Sts1Bar expected=1 observed=2", result.Output, StringComparison.Ordinal);
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
    public void Sts1EnabledModeLogVerifierReportsMalformedRetainedAuditPathAsFailedRows()
    {
        var verifier = AssertRepoFileExists("scripts", "check-sts1-enabled-mode-runtime-log.ps1");
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

            var logLength = new FileInfo(logPath).Length;
            var logHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(logPath))).ToLowerInvariant();
            File.WriteAllText(
                auditPath,
                $$"""
                {
                  "Path": "\u0000bad-audit-path",
                  "Length": {{logLength}},
                  "Sha256": {{JsonSerializer.Serialize(logHash)}},
                  "Clean": true,
                  "SignatureHits": []
                }
                """);

            var result = RunPowerShell(verifier, "-Mode", "Off", "-LogPath", logPath, "-AuditPath", auditPath);
            Assert.True(result.ExitCode == 0, $"Verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("audit_has_single_scanned_path status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("audit_path_matches_log_path status=fail", result.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("mismatches=0", result.Output, StringComparison.Ordinal);
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
