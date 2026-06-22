using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class RuntimeMonkeyStabilityGuardTests
{
    [Fact]
    public void MonkeyRunnerDefaultsToDryRunAndLaunchIsExplicit()
    {
        var runner = ReadRepoText("scripts", "run-spire-plus-monkey-stability.ps1");

        AssertSourceContains(
            runner,
            "[switch]$Launch",
            "[string]$Scenario = 'AncientUiSmoke'",
            "[string]$CommandSelectionMode = 'RoundRobin'",
            "[string]$CommandCorpusFile",
            "Get-ScenarioCommandCorpus",
            "AncientUiPlusVakuuFight",
            "VakuuFightSmoke",
            "StartupOnly",
            "spireplus_test_ancient VAKUU confirm fight",
            "monkey-plan.json",
            "PlannedCommands",
            "CommandScenarioMatrix",
            "ScenarioTag",
            "OwnerArea",
            "EffectiveNoDevConsoleCommands",
            "ReleaseEvidenceLogEnabled",
            "SPIREPLUS_RELEASE_EVIDENCE_LOG",
            "fight_option_shown",
            "if (-not $Launch)",
            "Dry-run only. Re-run with -Launch to start Steam sessions.",
            "exit 0",
            "Invoke-PowerShellFileToOutput",
            "'-Launch'");

        AssertBefore(runner, "if (-not $Launch)", "$prepareStarted = $true");
        Assert.DoesNotContain("Start-Process", runner, StringComparison.Ordinal);
        Assert.DoesNotContain("dotnet", runner, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MonkeyRunnerUsesRestoreSafeLiveSessionAndCanonicalLogAudit()
    {
        var runner = ReadRepoText("scripts", "run-spire-plus-monkey-stability.ps1");

        AssertSourceContains(
            runner,
            "spire-plus-live-session.ps1",
            "audit-godot-log.ps1",
            "MainMenuTimeoutSeconds",
            "'\\[Startup\\] Time to main menu'",
            "godot.log.after-launch",
            "godot.log.before",
            "godot.log.current-iteration",
            "godot-log-audit.json",
            "iteration-result.json",
            "monkey-summary.json",
            "-StopGameOnRestore",
            "-PreserveNewCurrentRunsOnRestore",
            "main menu log line missing before timeout",
            "godot.log missing or empty",
            "audit-godot-log reports release-blocking signature hits",
            "live-session restore fails");
    }

    [Fact]
    public void MonkeyRunnerRecordsHangProbeTelemetryAndCommandAcks()
    {
        var runner = ReadRepoText("scripts", "run-spire-plus-monkey-stability.ps1");

        AssertSourceContains(
            runner,
            "$hangProbeSchemaVersion = 1",
            "HangProbeSchemaVersion",
            "ObservationIntervalSeconds",
            "NoLogGrowthTimeoutSeconds",
            "RuntimeLogGrowthRequired",
            "-RequireLogGrowth $true",
            "-RequireLogGrowth $false",
            "UnresponsiveSampleThreshold",
            "SpirePlusRuntimeMonkeyNative",
            "IsHungAppWindow",
            "Test-LogContainsAfterOffset",
            "Get-SpireProcessSnapshot",
            "Add-ProbeSample",
            "runtime-probe-samples.json",
            "RuntimeProbeSamplesSha256",
            "Get-FileSha256OrEmpty -Path ([string]$result.RuntimeProbeSamplesPath)",
            "SampledAt",
            "$result.MainMenuObservation.LogScanOffsetBytes",
            "LogExists",
            "LogLengthBytes",
            "LogLastWriteTimeUtc",
            "Write-CurrentIterationLogSlice",
            "CurrentIterationLogPath",
            "ProcessProbe",
            "LogGrowthProbe",
            "FailsOnlyAfterConsecutiveUnresponsiveSamples",
            "RequiresMainWindowAfterMainMenu",
            "PreLaunchLogLengthBytes",
            "BaselineLogLengthBytes",
            "MinimumProcessStartTimeUtc",
            "Get-Process -Name SlayTheSpire2",
            "$preExistingProcesses.Count",
            "pre-existing SlayTheSpire2 process(es) before launch",
            "} else {",
            "StaleProcessObserved",
            "StaleProcessCount",
            "UnknownStartTimeProcessCount",
            "AmbiguousCurrentProcessCount",
            "CurrentProcessCount",
            "MaxConsecutiveUnresponsiveSamples",
            "shared godot.log cannot be trusted for this iteration",
            "CommandAckPatterns",
            "[switch]$CaptureWindowAfterCommand",
            "capture-spire-window.ps1",
            "CaptureWindowAfterCommand",
            "check-local-godot-source-workspace.ps1",
            "SourceWorkspaceCheckPath",
            "SourceWorkspaceCheckSha256",
            "SourceBranch",
            "SourceMainAssemblyHash",
            "InstalledGameCommit",
            "InstalledGameBranch",
            "InstalledGameMainAssemblyHash",
            "OriginMatchesInstalledGamePck",
            "AuthorizedSourceOriginVerified",
            "RefreshSourceSnapshotBeforeCurrentApiClaims",
            "RequireCurrentSourceSnapshot",
            "CommandSelectionMode",
            "RoundRobin",
            "CommandCorpusSource",
            "PlannedCommandCounts",
            "PlannedScenarioTagCounts",
            "PlannedOwnerAreaCounts",
            "PlannedVakuuFightIterationCount",
            "CommandCounts",
            "ScenarioTagCounts",
            "OwnerAreaCounts",
            "VakuuFightIterationCount",
            "Get-CommandOwnerArea",
            "Get-CommandScenarioTag",
            "Get-CommandAckPattern",
            "Starting unsaved live-test run for",
            "CommandAckObserved",
            "FailureReasonCodes",
            "[string[]]$FailureReasonCodes = @()",
            "if ($null -eq $FailureReasonCodes)",
            "$failureCodes = @(Get-FailureReasonCodes -Result ([pscustomobject]$result))",
            "$hangSignals = @(Get-HangSignals -FailureReasonCodes $failureCodes)",
            "HangSignals",
            "game_process_missing",
            "game_process_exited",
            "main_window_missing",
            "main_menu_timeout",
            "startup_log_stalled",
            "process_unresponsive",
            "stale_process_observed",
            "current_iteration_log_missing",
            "command_ack_missing",
            "FailedIterationIds",
            "FailureReasonCounts",
            "ProcessExitCount",
            "MainWindowMissingCount",
            "CurrentIterationLogMissingCount",
            "UnresponsiveIterationCount",
            "StaleProcessObservedCount",
            "LogStallIterationCount",
            "CommandAckMissingCount",
            "WindowCaptureRequired",
            "WindowCaptureSucceeded",
            "WindowCapturePath",
            "WindowCaptureSha256",
            "window_capture_missing",
            "WindowCaptureMissingCount");
        Assert.DoesNotContain("[System.Collections.Generic.List[object]]$ProbeSamples", runner, StringComparison.Ordinal);
        Assert.DoesNotContain("[System.Collections.Generic.List[object]]$Samples", runner, StringComparison.Ordinal);
        Assert.DoesNotContain("[System.Collections.Generic.List[object]]$Checks", runner, StringComparison.Ordinal);
        Assert.DoesNotContain("& $sts1ModeVerifierScript @args", runner, StringComparison.Ordinal);
    }

    [Fact]
    public void MonkeyRunnerAndPacketCheckerGuardScenarioDistribution()
    {
        var runner = ReadRepoText("scripts", "run-spire-plus-monkey-stability.ps1");
        var checker = ReadRepoText("scripts", "check-spire-plus-runtime-monkey-packet.ps1");

        AssertSourceContains(
            runner,
            "Get-ValueCounts",
            "PlannedCommandCounts",
            "PlannedScenarioTagCounts",
            "PlannedOwnerAreaCounts",
            "PlannedVakuuFightIterationCount",
            "CommandCounts",
            "ScenarioTagCounts",
            "OwnerAreaCounts",
            "VakuuFightIterationCount");

        AssertSourceContains(
            checker,
            "Test-CountMapMatches",
            "plan_planned_scenario_tag_counts_match",
            "plan_planned_owner_area_counts_match",
            "plan_planned_command_counts_match",
            "plan_vakuu_fight_planned_count_matches",
            "plan_vakuu_fight_smoke_all_iterations_are_fight",
            "plan_ancient_ui_plus_vakuu_fight_includes_fight",
            "plan_ancient_ui_plus_vakuu_fight_1000_balanced",
            "summary_scenario_tag_counts_match_results",
            "summary_owner_area_counts_match_results",
            "summary_command_counts_match_results",
            "summary_vakuu_fight_iteration_count_matches_results",
            "plan_window_capture_script_path_matches_current_helper",
            "summary_window_capture_missing_count_matches_results",
            "window_capture_hash_matches_file");
    }

    [Fact]
    public void RuntimeMonkeyRunnerChecksForPreExistingProcessesBeforeBaselineLog()
    {
        var runner = ReadRepoText("scripts", "run-spire-plus-monkey-stability.ps1");

        AssertSourceContains(
            runner,
            "$preExistingProcesses = @(Get-Process -Name SlayTheSpire2",
            "$preExistingProcesses.Count -gt 0",
            "pre-existing SlayTheSpire2 process(es) before launch",
            "Copy-BaselineGodotLog",
            "GodotLogBeforePath",
            "$preLaunchLog = Get-LogSnapshot -Path ([string]$result.GodotLogBeforePath)",
            "Invoke-PowerShellFileToOutput");
        AssertBefore(runner, "$preExistingProcesses = @(Get-Process -Name SlayTheSpire2", "$preLaunchLog = Get-LogSnapshot -Path ([string]$result.GodotLogBeforePath)");
        AssertBefore(runner, "$preExistingProcesses = @(Get-Process -Name SlayTheSpire2", "$prepareStarted = $true");
    }
}
