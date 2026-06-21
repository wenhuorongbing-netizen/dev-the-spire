using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class RuntimeMonkeyStabilityGuardTests
{
    [Fact]
    public void LocalGodotSourceWorkspaceCheckerIsNoLaunchAndGuardsFreshness()
    {
        var checker = ReadRepoText("scripts", "check-local-godot-source-workspace.ps1");

        AssertSourceContains(
            checker,
            "[string]$SourceRoot = 'source code'",
            "[string]$GameRoot = 'E:\\Steam\\steamapps\\common\\Slay the Spire 2'",
            "[string]$GodotExe",
            "[switch]$FailOnMismatch",
            "Normalize-ComparablePath",
            "Get-ObjectPropertyString",
            "SlayTheSpire2.pck",
            "installed_game_pck_exists",
            "source_version_matches_installed_game",
            "source_commit_matches_installed_game",
            "source_branch_matches_installed_game",
            "source_main_assembly_hash_matches_installed_game",
            "source_release_identity_matches_installed_game",
            "Severity",
            "Warnings",
            "autoslay_autoslayer_source_exists",
            "autoslay_event_room_handler_exists",
            "autoslay_start_seed_logfile_signature_present",
            "public void Start(string seed, string? logFile = null)",
            "autoslay_noninteractive_mode_check_present",
            "NonInteractiveMode.AutoSlayerCheck = () => IsActive",
            "autoslay_debug_seed_override_present",
            "NGame.Instance.DebugSeedOverride = seed",
            "autoslay_card_selector_present",
            "CardSelectCmd.UseSelector(new AutoSlayCardSelector(_random))",
            "autoslay_ancient_dialogue_handler_present",
            "Detected Ancient event, clicking through dialogue",
            "autoslay_event_option_selection_logged",
            "Selecting event option:",
            "autoslay_event_triggered_combat_logged",
            "Event triggered combat, handling combat first",
            "autoslay_event_combat_started_logged",
            "Event combat started, applying buffs and killing enemies",
            "AutoSlay = [pscustomobject]$autoSlaySummary",
            "GameNativeAutoSlayStillRequiresRuntimeLaunchEvidence",
            "source_root_is_git_ignored",
            "source_root_has_no_tracked_files",
            "godot_open_command_prepared",
            "gdre_log_recovery_finished",
            "gdre_log_engine_version_godot_451",
            "gdre_opening_file_matches_installed_game_pck",
            "OpeningFileMatchesInstalledGame",
            "OriginMatchesInstalledGamePck",
            "PckSha256",
            "ritsulib_manifest_exists",
            "ritsulib_variant_matches_installed_game",
            "ritsulib_variant_dll_exists",
            "ManifestSha256",
            "VariantDllSha256",
            "ExpectedVariantDllSha256",
            "CompatTargetText",
            "ritsulib_viewer_exists",
            "RitsuLib viewer exists; it is a log viewer, not an unpacker or monkey runner",
            "OpenProjectCommand",
            "EvidenceUsePolicy",
            "NotRuntimeProof",
            "AuthorizedLocalInstallOnly",
            "AuthorizedSourceOriginVerified",
            "ThirdPartyDumpsProhibited",
            "OriginalGameSourceMustNotBeTracked",
            "RefreshSourceSnapshotBeforeCurrentApiClaims",
            "if ($FailOnMismatch -and $mismatches.Count -gt 0)");

        Assert.DoesNotContain("Start-Process", checker, StringComparison.Ordinal);
        Assert.DoesNotContain("--editor", checker, StringComparison.Ordinal);
        Assert.DoesNotContain("& dotnet", checker, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("dotnet build", checker, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("dotnet test", checker, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("dotnet publish", checker, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RuntimeMonkeyDocsKeepGameNativeAutoSlayProofSeparateFromDevConsoleHarness()
    {
        var docs = ReadRepoText("docs", "testing", "runtime-monkey-stability.md");
        var runner = ReadRepoText("scripts", "run-spire-plus-monkey-stability.ps1");

        AssertSourceContains(
            docs,
            "- Current runtime dependency: `STS2-RitsuLib v0.4.32` in direct NuGet runtime layout.",
            "Current source snapshot: `source code\\release_info.json` matches the installed",
            "`v0.107.1` game identity after the 2026-06-20 GDRE refresh",
            "commit `59260271`",
            "branch `v0.107.1`",
            "main assembly hash `-1555940892`",
            "current-source parity",
            "The previous beta.93 local state is expected to pass",
            "`source_release_identity_matches_installed_game`",
            "18 failed scripts and one parse warning",
            "recovery-quality warnings, not source version, commit, branch, hash, or origin",
            "## Game-Native AutoSlay Batch Lane",
            "`source code\\src\\Core\\AutoSlay\\AutoSlayer.cs`",
            "`AutoSlayer.Start(seed, logFile)`",
            "`NonInteractiveMode.AutoSlayerCheck = () => IsActive`",
            "`NGame.Instance.DebugSeedOverride = seed`",
            "`CardSelectCmd.UseSelector(new AutoSlayCardSelector(_random))`",
            "`source code\\src\\Core\\AutoSlay\\Handlers\\Rooms\\EventRoomHandler.cs`",
            "Detected Ancient event, clicking",
            "Selecting event option:",
            "Event triggered combat",
            "Event combat started",
            "Current `scripts\\run-spire-plus-monkey-stability.ps1` lane is not",
            "AutoSlay-backed",
            "Do not count a packet from that lane as game-native",
            "the exact launcher or mod hook that calls `AutoSlayer.Start(seed, logFile)`",
            "one `run-result.json` per seed",
            "`RecoveredSource.MatchesInstalledGame`",
            "`RecoveredSource.OriginMatchesInstalledGamePck`",
            "`RuntimeProbeSamplesPath`",
            "`MainMenuObservation`",
            "`RuntimeObservation`",
            "`Phase`",
            "`SampledAt`",
            "`LogExists`",
            "`LogLengthBytes`",
            "`LogLastWriteTimeUtc`",
            "`RuntimeObservation.LogInitialLengthBytes`",
            "`ProcessId`",
            "`ProcessStartTimeUtc`",
            "`ProcessPath`",
            "`ExpectedGameProcessId`",
            "`ExpectedGameProcessStartTimeUtc`",
            "`ExpectedGameProcessPath`",
            "`ProcessIdentityMatchesExpected`",
            "`MainWindowObserved`",
            "top-level per-seed path",
            "top-level `run-####/<standard-file>` path",
            "Ancient id, ordered",
            "start/event/Ancient-dialogue/event-option/completion markers",
            "place any per-seed artifact outside",
            "sidecar text to",
            "observed ordered event-room lines in both",
            "check-spire-plus-autoslay-packet.ps1",
            "GameNativeAutoSlay",
            "single-seed fixture packet is not batch proof",
            "verifier must fail",
            "-ExpectedAncientIds",
            "Target coverage compares expected, plan, summary, and traversed Ancient ids",
            "case-insensitively after normalizing them to uppercase",
            "`run-result.json` and `autoslay-summary.json` `AncientId` values must still",
            "-ExpectedPatchCount 25");

        Assert.DoesNotContain("Current runtime dependency: `STS2-RitsuLib v0.4.31` with `lib\\0.107.0`", docs, StringComparison.Ordinal);
        Assert.DoesNotContain("currently reports `v0.106.0`", docs, StringComparison.Ordinal);
        Assert.DoesNotContain("as warnings until `source code/`", docs, StringComparison.Ordinal);

        Assert.DoesNotContain("AutoSlayer", runner, StringComparison.Ordinal);
        Assert.DoesNotContain("AutoSlayLog", runner, StringComparison.Ordinal);
        Assert.Contains("DevConsole", runner, StringComparison.Ordinal);
    }
}
