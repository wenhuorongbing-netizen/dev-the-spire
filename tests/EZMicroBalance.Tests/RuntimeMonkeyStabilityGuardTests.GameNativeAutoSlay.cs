using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class RuntimeMonkeyStabilityGuardTests
{
    [Fact]
    public void GameNativeAutoSlayPacketVerifierSourceShapeRequiresNativeRunnerAndEventTraversal()
    {
        var verifier = AssertRepoFileExists("scripts", "check-spire-plus-autoslay-packet.ps1");
        var verifierText = File.ReadAllText(verifier);

        AssertSourceContains(
            verifierText,
            "function Get-NormalizedAncientIdTokens",
            "$sts1EnabledModeLogVerifierScript",
            "Invoke-RecomputedSts1ModeLogCheck",
            "sts1_mode_log_check_recomputed_from_current_iteration_log",
            "sts1_mode_log_check_recomputed_mismatches_empty",
            "sts1_mode_log_check_checks_match_recomputed",
            "ForEach-Object { $_.ToUpperInvariant() }",
            "$expectedAncientIdsForCoverage = @(Get-NormalizedAncientIdTokens -Value $ExpectedAncientIds)",
            "$planExpectedAncientIdsForCoverage = @(Get-NormalizedAncientIdTokens -Value (Get-JsonValue -Object $plan -Name 'ExpectedAncientIds' -DefaultValue @()))",
            "$observedAncientIdSet.Add($normalizedAncientId)",
            "$traversedAncientIdSet.Add($ancientId.Trim().ToUpperInvariant())",
            "runtime_probe_samples_sha256_matches_retained_file",
            "run_result_runtime_probe_samples_sha256_matches_summary",
            "run_result_sha256_matches_retained_file",
            "summary_passed_matches_runs_array",
            "summary_failed_runs_matches_runs_array",
            "$summaryProblemRunRows = @($summaryRuns | Where-Object {",
            "problemRows=$($summaryProblemRunRows.Count)",
            "summary_retained_run_directory_count_matches_runs",
            "summary_retained_run_directories_match_expected_sequence",
            "function Get-RunDirectorySortKey",
            "[long]::TryParse",
            "[System.Globalization.NumberStyles]::None",
            "[long]::MaxValue",
            "$retainedRunDirectoryNames = @(Get-ChildItem -LiteralPath $resolvedEvidenceDir -Directory -Filter 'run-*'",
            "function Test-NormalizedPathEquals",
            "function Test-ExistingPathChainHasNoReparsePoint",
            "function Test-AllExistingPathChainsHaveNoReparsePoint",
            "evidence_dir_reparse_point_free",
            "top_level_canonical_artifact_paths_reparse_point_free",
            "plan_launcher_path_reparse_point_free",
            "plan_source_workspace_check_reparse_point_free",
            "canonical_artifact_paths_reparse_point_free",
            "run_result_path_matches_expected_per_seed_dir",
            "runtime_probe_samples_path_matches_expected_per_seed_file",
            "autoslay_log_path_matches_expected_per_seed_file",
            "before_log_path_matches_expected_per_seed_file",
            "after_launch_log_path_matches_expected_per_seed_file",
            "current_iteration_log_path_matches_expected_per_seed_file",
            "audit_path_matches_expected_per_seed_file",
            "sts1_mode_check_path_matches_expected_per_seed_file",
            "plan_expected_patch_count_positive",
            "plan_expected_patch_count_matches_expected",
            "summary_package_version_matches_plan",
            "summary_expected_patch_count_matches_plan",
            "summary_expected_ancient_ids_match_plan",
            "expected_package_version_parameter_provided",
            "expected_game_version_parameter_provided",
            "expected_ritsu_lib_version_parameter_provided",
            "expected_ritsu_compat_branch_parameter_provided",
            "expected_patch_count_parameter_provided",
            "function Assert-OutFileDoesNotOverwriteCanonicalEvidence",
            "Refusing to write OutFile over canonical AutoSlay evidence",
            "${runName}_run_result_ancient_id_matches_summary");
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierAcceptsCanonicalNativeRunnerPacket()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var result = fixture.RunVerifier(
            "-ExpectedAncientIds",
            "VAKUU",
            "-OutFile",
            Path.Combine(fixture.Workdir, "autoslay-packet-check.json"),
            "-FailOnMismatch");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier failed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("autoslay_plan_schema_version_one status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("autoslay_summary_schema_version_one status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_runner_kind_is_game_native_autoslay status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_invocation_calls_autoslayer_start status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_launcher_sha256_matches status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_hook_id_present status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_source_workspace_schema_version_one status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_source_workspace_policy_no_launch status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_source_workspace_policy_runtime_proof_still_requires_launch status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_source_workspace_report_matches_installed_game status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_run_seeds_match_plan_seeds status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_package_version_matches_plan status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_expected_patch_count_matches_plan status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_expected_ancient_ids_match_plan status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("expected_ancient_ids_required_for_proof_mode status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("expected_ancient_ids_unique status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_expected_ancient_ids_match_parameter status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_expected_patch_count_positive status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_expected_patch_count_matches_expected status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_passed_matches_runs_array status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_failed_runs_matches_runs_array status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_retained_run_directory_count_matches_runs status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_retained_run_directories_match_expected_sequence status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_expected_ancient_ids_observed status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_ancient_id_counts_match_runs status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_sha256_recorded status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_path_matches_expected_per_seed_dir status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_path_matches_expected_per_seed_file status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_autoslay_log_path_matches_expected_per_seed_file status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_current_iteration_log_path_matches_expected_per_seed_file status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_sha256_matches_retained_file status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_sha256_matches_retained_file status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_runtime_probe_samples_sha256_matches_summary status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_process_id_matches_runtime_probe_samples status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_all_match_expected_identity status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_current_iteration_log_matches_after_launch_slice status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_audit_recomputed_clean status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_sts1_mode_log_check_recomputed_from_current_iteration_log status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_sts1_mode_log_check_checks_match_recomputed status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_autoslay_log_event_sequence_observed status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_current_log_event_sequence_observed status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_event_room_traversal_observed status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("expected_ancient_ids_have_event_traversal status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("allow_missing_event_traversal_not_proof_mode status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("mismatches=0", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsExtraRetainedRunDirectory()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        Directory.CreateDirectory(Path.Combine(fixture.Workdir, "run-0002"));

        var result = fixture.RunVerifier(
            "-ExpectedAncientIds",
            "VAKUU",
            "-FailOnMismatch");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("summary_retained_run_directory_count_matches_runs status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_retained_run_directories_match_expected_sequence status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("expected=1, actual=2", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsNonStandardRetainedRunDirectoryName()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var nonStandardRunDir = Path.Combine(fixture.Workdir, "run-foo");
        Directory.Move(fixture.RunDir, nonStandardRunDir);

        var result = fixture.RunVerifier(
            "-ExpectedAncientIds",
            "VAKUU",
            "-FailOnMismatch");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("summary_retained_run_directory_count_matches_runs status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_retained_run_directories_match_expected_sequence status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("expected=run-0001 actual=run-foo", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_exists status=fail", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierSortsNumericRunDirectoryNamesBeforeSequenceComparison()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        Directory.CreateDirectory(Path.Combine(fixture.Workdir, "run-10"));
        Directory.CreateDirectory(Path.Combine(fixture.Workdir, "run-2"));

        var result = fixture.RunVerifier(
            "-ExpectedAncientIds",
            "VAKUU",
            "-FailOnMismatch");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("summary_retained_run_directory_count_matches_runs status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_retained_run_directories_match_expected_sequence status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("expected=run-0001 actual=run-0001,run-2,run-10", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsMissingExpectedRunDirectory()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        Directory.Delete(fixture.RunDir, recursive: true);

        var result = fixture.RunVerifier(
            "-ExpectedAncientIds",
            "VAKUU",
            "-FailOnMismatch");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("summary_retained_run_directory_count_matches_runs status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_retained_run_directories_match_expected_sequence status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_exists status=fail", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsOutFileOverCanonicalEvidence()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();

        var result = fixture.RunVerifier(
            "-ExpectedAncientIds",
            "VAKUU",
            "-OutFile",
            fixture.SummaryPath);

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("Refusing to write OutFile over canonical AutoSlay evidence", result.Output + result.Error, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsProofModeWhenCurrentTargetParametersAreOmitted()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var result = fixture.RunVerifierWithoutDefaultTargetArguments(
            "-ExpectedAncientIds",
            "VAKUU",
            "-FailOnMismatch");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("expected_package_version_parameter_provided status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("expected_game_version_parameter_provided status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("expected_ritsu_lib_version_parameter_provided status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("expected_ritsu_compat_branch_parameter_provided status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("expected_patch_count_parameter_provided status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("expected_ancient_ids_required_for_proof_mode status=pass", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierBindsSummaryBatchMetadataToPlan()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var tamperedSummary = fixture.OriginalSummaryJson
            .Replace("\"PackageVersion\": \"v0.1.0-private-beta.87\"", "\"PackageVersion\": \"v0.1.0-private-beta.86\"", StringComparison.Ordinal)
            .Replace("\"ExpectedPatchCount\": 25", "\"ExpectedPatchCount\": 24", StringComparison.Ordinal)
            .Replace("\"ExpectedAncientIds\": [\"VAKUU\"]", "\"ExpectedAncientIds\": [\"URDA\"]", StringComparison.Ordinal);
        Assert.NotEqual(fixture.OriginalSummaryJson, tamperedSummary);
        File.WriteAllText(fixture.SummaryPath, tamperedSummary);

        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("summary_package_version_matches_plan status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_expected_patch_count_matches_plan status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_expected_ancient_ids_match_plan status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("missing=VAKUU unexpected=URDA", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsSts1ReportAndOrderedSelectionDrift()
    {
        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(
                fixture.AutoSlayLogPath,
                "11:59:58.000 [INFO] [AutoSlay] Action: Selecting event option: VAKUU (stale pre-run line)" + Environment.NewLine +
                fixture.OriginalAutoSlayLog.Replace("Selecting event option: VAKUU", "Selecting event option: URDA", StringComparison.Ordinal));
            var staleCurrentLog =
                "11:59:58.000 [INFO] [AutoSlay] Action: Selecting event option: VAKUU (stale pre-run line)" + Environment.NewLine +
                fixture.OriginalCurrentLog.Replace("Selecting event option: VAKUU", "Selecting event option: URDA", StringComparison.Ordinal);
            File.WriteAllText(fixture.CurrentLogPath, staleCurrentLog);
            File.WriteAllText(fixture.AfterLogPath, fixture.BeforeLog + staleCurrentLog);

            var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_autoslay_log_selects_ancient_id status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_current_log_selects_ancient_id status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_autoslay_log_event_sequence_observed status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_current_log_event_sequence_observed status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_event_room_traversal_observed status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("expected_ancient_ids_have_event_traversal status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            var forgedCurrentLog = fixture.OriginalCurrentLog.Replace(
                "Feature Sts1Events bootstrap=disabled, live=Disabled",
                "Feature Sts1Events bootstrap=enabled, live=Enabled  ",
                StringComparison.Ordinal);
            var forgedAfterLog = fixture.BeforeLog + forgedCurrentLog;
            File.WriteAllText(fixture.CurrentLogPath, forgedCurrentLog);
            File.WriteAllText(fixture.AfterLogPath, forgedAfterLog);
            var forgedCurrentLogLength = new FileInfo(fixture.CurrentLogPath).Length;
            var forgedAfterLogLength = new FileInfo(fixture.AfterLogPath).Length;
            var forgedCurrentLogHash = Sha256File(fixture.CurrentLogPath);
            var forgedAfterLogHash = Sha256File(fixture.AfterLogPath);
            File.WriteAllText(fixture.AuditPath, ToBoundAuditJson(fixture.CurrentLogPath, """{"SignatureHits":[]}"""));
            File.WriteAllText(
                fixture.Sts1ModeCheckPath,
                $$"""
                {
                  "Mode": "Off",
                  "LogPath": {{JsonSerializer.Serialize(fixture.CurrentLogPath)}},
                  "LogLength": {{forgedCurrentLogLength}},
                  "LogSha256": {{JsonSerializer.Serialize(forgedCurrentLogHash)}},
                  "Mismatches": [],
                  "Checks": [
                    {
                      "Name": "off_feature_line_disabled",
                      "Passed": true,
                      "Detail": "forged retained report"
                    }
                  ]
                }
                """);
            File.WriteAllText(fixture.SummaryPath, fixture.RewriteLogMetadata(fixture.OriginalSummaryJson, forgedCurrentLogLength, forgedCurrentLogHash, forgedAfterLogLength, forgedAfterLogHash));
            File.WriteAllText(fixture.RunResultPath, fixture.RewriteLogMetadata(fixture.OriginalRunResultJson, forgedCurrentLogLength, forgedCurrentLogHash, forgedAfterLogLength, forgedAfterLogHash));

            var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_sts1_mode_log_check_log_path_matches_current_iteration_log status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_sts1_mode_log_check_recomputed_from_current_iteration_log status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_sts1_mode_log_check_recomputed_mismatches_empty status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_sts1_mode_log_check_recomputed_all_checks_passed status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_sts1_mode_log_check_mismatches_match_recomputed status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_sts1_mode_log_check_checks_match_recomputed status=fail", result.Output, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierNormalizesAncientCoverageCase()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        File.WriteAllText(fixture.PlanPath, fixture.OriginalPlanJson.Replace("\"ExpectedAncientIds\": [\"VAKUU\"]", "\"ExpectedAncientIds\": [\"vakuu\"]", StringComparison.Ordinal));
        File.WriteAllText(fixture.SummaryPath, fixture.OriginalSummaryJson.Replace("\"AncientId\": \"VAKUU\"", "\"AncientId\": \"Vakuu\"", StringComparison.Ordinal));
        File.WriteAllText(fixture.RunResultPath, fixture.OriginalRunResultJson.Replace("\"AncientId\": \"VAKUU\"", "\"AncientId\": \"Vakuu\"", StringComparison.Ordinal));
        fixture.RefreshSummaryRunResultHash();

        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU", "-FailOnMismatch");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier should normalize AncientId target coverage case:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("plan_expected_ancient_ids_match_parameter status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_expected_ancient_ids_observed status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_ancient_id_counts_match_runs status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("expected_ancient_ids_have_event_traversal status=pass", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsScalarExpectedAncientIdsShapeEvenWhenCoverageMatches()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var planJson = JsonNode.Parse(File.ReadAllText(fixture.PlanPath))!.AsObject();
        planJson["ExpectedAncientIds"] = "VAKUU";
        File.WriteAllText(fixture.PlanPath, planJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        summaryJson["ExpectedAncientIds"] = "VAKUU";
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("plan_expected_ancient_ids_array status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_expected_ancient_ids_array status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_expected_ancient_ids_match_parameter status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_expected_ancient_ids_match_plan status=pass", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsMissingExpectedAncientCoverage()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        File.WriteAllText(
            fixture.PlanPath,
            fixture.OriginalPlanJson.Replace("\"ExpectedAncientIds\": [\"VAKUU\"]", "\"ExpectedAncientIds\": [\"VAKUU\", \"URDA\"]", StringComparison.Ordinal));
        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU,URDA");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("expected_ancient_ids_unique status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_expected_ancient_ids_match_parameter status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_expected_ancient_ids_observed status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_expected_ancient_id_counts_positive status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("expected_ancient_ids_have_event_traversal status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("ExpectedAncientIds missing=URDA", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRequiresExpectedAncientIdsInProofMode()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var result = fixture.RunVerifier("-FailOnMismatch");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("expected_ancient_ids_required_for_proof_mode status=fail", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsMissingTraversalBypassInProofMode()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU", "-AllowMissingEventTraversal", "-FailOnMismatch");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("allow_missing_event_traversal_not_proof_mode status=fail", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsAncientIdCountMismatches()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        File.WriteAllText(
            fixture.SummaryPath,
            fixture.OriginalSummaryJson.Replace("\"AncientIdCounts\": { \"VAKUU\": 1 }", "\"AncientIdCounts\": { \"VAKUU\": 0 }", StringComparison.Ordinal));
        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("summary_ancient_id_counts_present status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_ancient_id_counts_valid status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_ancient_id_counts_match_runs status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_ancient_id_counts_total_matches_runs status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_expected_ancient_id_counts_positive status=fail", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsExtraZeroAncientIdCounts()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        File.WriteAllText(
            fixture.SummaryPath,
            fixture.OriginalSummaryJson.Replace("\"AncientIdCounts\": { \"VAKUU\": 1 }", "\"AncientIdCounts\": { \"VAKUU\": 1, \"URDA\": 0 }", StringComparison.Ordinal));
        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("summary_ancient_id_counts_match_runs status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("URDA:extra_summary=0", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_ancient_id_counts_total_matches_runs status=pass", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsUndersizedRunCounts()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var result = fixture.RunVerifier("-MinRuns", "2", "-ExpectedAncientIds", "VAKUU", "-FailOnMismatch");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("expected_ancient_ids_required_for_proof_mode status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_seed_count_meets_minimum status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_total_runs_meets_minimum status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_event_room_traversal_observed status=pass", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsNonPositiveMinRuns()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var result = fixture.RunVerifier("-MinRuns", "0", "-ExpectedAncientIds", "VAKUU", "-FailOnMismatch");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("min_runs_positive status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_seed_count_meets_minimum status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_total_runs_meets_minimum status=pass", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsStaleSummaryPassAndFailureSignals()
    {
        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            var staleSummaryPassedRow = Regex.Replace(
                fixture.OriginalSummaryJson,
                "(\"Seed\":\\s*" + Regex.Escape(JsonSerializer.Serialize(fixture.Seed)) + "\\s*,\\s*)\"Passed\": true",
                "$1\"Passed\": false",
                RegexOptions.CultureInvariant);
            Assert.NotEqual(fixture.OriginalSummaryJson, staleSummaryPassedRow);
            File.WriteAllText(fixture.SummaryPath, staleSummaryPassedRow);
            var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("summary_passed status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_failed_runs_zero status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_passed_matches_runs_array status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_failed_runs_matches_runs_array status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("problemRows=1", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_summary_run_passed_true status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            var summaryWithProblemSignals = fixture.OriginalSummaryJson
                .Replace("\"FailureReasonCodes\": []", "\"FailureReasonCodes\": [\"autoslay_seed_failed\"]", StringComparison.Ordinal)
                .Replace("\"HangSignals\": []", "\"HangSignals\": [\"runtime_not_responding\"]", StringComparison.Ordinal);
            Assert.NotEqual(fixture.OriginalSummaryJson, summaryWithProblemSignals);
            File.WriteAllText(fixture.SummaryPath, summaryWithProblemSignals);
            var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("summary_passed status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_failed_runs_zero status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_passed_matches_runs_array status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_failed_runs_matches_runs_array status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("problemRows=1", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_summary_run_passed_true status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_summary_run_failure_reason_codes_empty status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_summary_run_hang_signals_empty status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_failure_reason_codes_match_summary status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_hang_signals_match_summary status=fail", result.Output, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsMalformedNumericEvidenceWithoutCrashing()
    {
        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(
                fixture.SummaryPath,
                fixture.OriginalSummaryJson.Replace("\"ExitCode\": 0", "\"ExitCode\": \"oops\"", StringComparison.Ordinal));
            var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_exit_code_zero status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(
                fixture.RunResultPath,
                fixture.OriginalRunResultJson.Replace(
                    "\"GodotLogBeforeLengthBytes\": " + fixture.BeforeLogLength,
                    "\"GodotLogBeforeLengthBytes\": \"oops\"",
                    StringComparison.Ordinal));
            fixture.RefreshSummaryRunResultHash();
            var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_run_result_sha256_matches_retained_file status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_before_log_length_matches_summary status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            var planJson = JsonNode.Parse(File.ReadAllText(fixture.PlanPath))!.AsObject();
            planJson["SchemaVersion"] = "1";
            planJson["ExpectedPatchCount"] = GameNativeAutoSlayFixture.ExpectedPatchCount.ToString(CultureInfo.InvariantCulture);
            File.WriteAllText(fixture.PlanPath, planJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
            summaryJson["SchemaVersion"] = "1";
            summaryJson["ExpectedPatchCount"] = GameNativeAutoSlayFixture.ExpectedPatchCount.ToString(CultureInfo.InvariantCulture);
            summaryJson["TotalRuns"] = "1";
            summaryJson["FailedRuns"] = "0";
            summaryJson["AncientIdCounts"]!.AsObject()["VAKUU"] = "1";
            var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
            summaryRunJson["ExitCode"] = "0";
            summaryRunJson["GodotLogBeforeLengthBytes"] = fixture.BeforeLogLength.ToString(CultureInfo.InvariantCulture);
            summaryRunJson["GodotLogAfterLaunchLengthBytes"] = fixture.AfterLaunchLogLength.ToString(CultureInfo.InvariantCulture);
            summaryRunJson["GodotLogCurrentIterationLengthBytes"] = fixture.CurrentLogLength.ToString(CultureInfo.InvariantCulture);

            var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
            runResultJson["SchemaVersion"] = "1";
            runResultJson["ProcessId"] = "4242";
            runResultJson["ExitCode"] = "0";
            runResultJson["StaleProcessCount"] = "0";
            runResultJson["GodotLogBeforeLengthBytes"] = fixture.BeforeLogLength.ToString(CultureInfo.InvariantCulture);
            runResultJson["GodotLogAfterLaunchLengthBytes"] = fixture.AfterLaunchLogLength.ToString(CultureInfo.InvariantCulture);
            runResultJson["GodotLogCurrentIterationLengthBytes"] = fixture.CurrentLogLength.ToString(CultureInfo.InvariantCulture);
            runResultJson["MainMenuObservation"]!.AsObject()["MaxStaleProcessCount"] = "0";
            var runtimeObservationJson = runResultJson["RuntimeObservation"]!.AsObject();
            runtimeObservationJson["MaxStaleProcessCount"] = "0";
            runtimeObservationJson["LogInitialLengthBytes"] = fixture.BeforeLogLength.ToString(CultureInfo.InvariantCulture);
            runtimeObservationJson["LogFinalLengthBytes"] = fixture.AfterLaunchLogLength.ToString(CultureInfo.InvariantCulture);

            var probeSamples = JsonNode.Parse(File.ReadAllText(fixture.RuntimeProbeSamplesPath))!.AsArray();
            foreach (var sampleNode in probeSamples)
            {
                var sample = sampleNode!.AsObject();
                sample["LogLengthBytes"] = JsonScalarValueToInvariantString(sample["LogLengthBytes"]);
                sample["ProcessId"] = JsonScalarValueToInvariantString(sample["ProcessId"]);
                sample["ExpectedGameProcessId"] = JsonScalarValueToInvariantString(sample["ExpectedGameProcessId"]);
                sample["StaleProcessCount"] = JsonScalarValueToInvariantString(sample["StaleProcessCount"]);
                sample["CurrentProcessCount"] = JsonScalarValueToInvariantString(sample["CurrentProcessCount"]);
                sample["UnknownStartTimeProcessCount"] = JsonScalarValueToInvariantString(sample["UnknownStartTimeProcessCount"]);
                sample["AmbiguousCurrentProcessCount"] = JsonScalarValueToInvariantString(sample["AmbiguousCurrentProcessCount"]);
            }

            File.WriteAllText(fixture.RuntimeProbeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            var probeHash = Sha256File(fixture.RuntimeProbeSamplesPath);
            runResultJson["RuntimeProbeSamplesSha256"] = probeHash;
            summaryRunJson["RuntimeProbeSamplesSha256"] = probeHash;

            File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            summaryRunJson["RunResultSha256"] = Sha256File(fixture.RunResultPath);
            File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var auditJsonRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!.AsArray();
            var auditJson = auditJsonRoot[0]!.AsObject();
            auditJson["Length"] = JsonScalarValueToInvariantString(auditJson["Length"]);
            auditJson["SignatureHits"] = new JsonObject
            {
                ["Name"] = "fixture-zero-count",
                ["Count"] = "0",
            };
            File.WriteAllText(fixture.AuditPath, auditJsonRoot.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var sts1ModeLogCheckJson = JsonNode.Parse(File.ReadAllText(fixture.Sts1ModeCheckPath))!.AsObject();
            sts1ModeLogCheckJson["LogLength"] = JsonScalarValueToInvariantString(sts1ModeLogCheckJson["LogLength"]);
            File.WriteAllText(fixture.Sts1ModeCheckPath, sts1ModeLogCheckJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("autoslay_plan_schema_version_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("plan_expected_patch_count_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("autoslay_summary_schema_version_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_expected_patch_count_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_total_runs_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_failed_runs_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_ancient_id_counts_valid status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_exit_code_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_godot_log_before_length_bytes_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_godot_log_after_launch_length_bytes_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_godot_log_current_iteration_length_bytes_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_log_length_bytes_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_process_id_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_expected_game_process_id_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_stale_process_count_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_current_process_count_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_unknown_start_time_process_count_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_ambiguous_current_process_count_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_audit_array_fields_native status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_audit_numeric_fields_native status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_sts1_mode_log_check_log_length_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_schema_version_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_process_id_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_exit_code_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_stale_process_count_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_godot_log_before_length_bytes_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_godot_log_after_launch_length_bytes_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_godot_log_current_iteration_length_bytes_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_main_menu_observation_max_stale_process_count_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_runtime_observation_max_stale_process_count_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_runtime_observation_log_initial_length_bytes_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_runtime_observation_log_final_length_bytes_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_sha256_matches_retained_file status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_sha256_matches_retained_file status=pass", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(
                fixture.PlanPath,
                fixture.OriginalPlanJson
                    .Replace("\"SchemaVersion\": 1", "\"SchemaVersion\": 1.0", StringComparison.Ordinal)
                    .Replace("\"ExpectedPatchCount\": 25", "\"ExpectedPatchCount\": 25.0", StringComparison.Ordinal));
            File.WriteAllText(
                fixture.SummaryPath,
                fixture.OriginalSummaryJson
                    .Replace("\"SchemaVersion\": 1", "\"SchemaVersion\": 1.0", StringComparison.Ordinal)
                    .Replace("\"ExpectedPatchCount\": 25", "\"ExpectedPatchCount\": 25.0", StringComparison.Ordinal)
                    .Replace("\"TotalRuns\": 1", "\"TotalRuns\": 1.0", StringComparison.Ordinal)
                    .Replace("\"FailedRuns\": 0", "\"FailedRuns\": 0.0", StringComparison.Ordinal)
                    .Replace("\"VAKUU\": 1", "\"VAKUU\": 1.0", StringComparison.Ordinal));

            var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("autoslay_plan_schema_version_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("plan_expected_patch_count_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("autoslay_summary_schema_version_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_expected_patch_count_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_total_runs_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_failed_runs_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_ancient_id_counts_valid status=fail", result.Output, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsCaseDriftedEvidenceFieldNames()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var planJson = JsonNode.Parse(File.ReadAllText(fixture.PlanPath))!.AsObject();
        var expectedPatchCount = planJson["ExpectedPatchCount"]!.DeepClone();
        planJson.Remove("ExpectedPatchCount");
        planJson["expectedpatchcount"] = expectedPatchCount;
        File.WriteAllText(fixture.PlanPath, planJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("plan_expected_patch_count_integer status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_expected_patch_count_positive status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_expected_patch_count_matches_expected status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_expected_patch_count_matches_plan status=fail", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsMalformedBooleanEvidenceWithoutFailingOpen()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();

        var planJson = JsonNode.Parse(File.ReadAllText(fixture.PlanPath))!.AsObject();
        var sourceWorkspaceJson = planJson["SourceWorkspace"]!.AsObject();
        sourceWorkspaceJson["Checked"] = "false";
        sourceWorkspaceJson["MatchesInstalledGame"] = "false";
        sourceWorkspaceJson["OriginMatchesInstalledGamePck"] = "";
        sourceWorkspaceJson["NotRuntimeProof"] = "true";
        File.WriteAllText(fixture.PlanPath, planJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        summaryJson["Passed"] = "true";
        summaryJson["Runs"]!.AsArray()[0]!.AsObject()["Passed"] = "true";

        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        runResultJson["Launch"] = "true";
        runResultJson["Passed"] = "true";
        var mainMenuObservation = runResultJson["MainMenuObservation"]!.AsObject();
        mainMenuObservation["Passed"] = "true";
        mainMenuObservation["MainMenuReached"] = "true";
        mainMenuObservation["ProcessObserved"] = "true";
        mainMenuObservation["ProcessExitedAfterObservation"] = null;
        mainMenuObservation["HungWindowDetected"] = "false";
        mainMenuObservation["StaleProcessObserved"] = "";
        mainMenuObservation["NoLogGrowthTimeoutExceeded"] = "false";
        var runtimeObservation = runResultJson["RuntimeObservation"]!.AsObject();
        runtimeObservation["Passed"] = "true";
        runtimeObservation["ProcessObserved"] = "true";
        runtimeObservation["ProcessExitedAfterObservation"] = null;
        runtimeObservation["HungWindowDetected"] = "false";
        runtimeObservation["StaleProcessObserved"] = "";
        runtimeObservation["NoLogGrowthTimeoutExceeded"] = "false";
        runtimeObservation["LogGrew"] = "true";

        var probeSamples = JsonNode.Parse(File.ReadAllText(fixture.RuntimeProbeSamplesPath))!.AsArray();
        foreach (var sampleNode in probeSamples)
        {
            var sample = sampleNode!.AsObject();
            sample["ProcessObserved"] = "true";
            sample["ProcessIdMatchesExpected"] = "true";
            sample["ProcessStartTimeMatchesExpected"] = "true";
            sample["ProcessPathMatchesExpected"] = "true";
            sample["ProcessIdentityMatchesExpected"] = "true";
            sample["HungWindow"] = "";
            sample["Responding"] = "false";
            sample["LogExists"] = "true";
        }

        File.WriteAllText(fixture.RuntimeProbeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        var probeHash = Sha256File(fixture.RuntimeProbeSamplesPath);
        runResultJson["RuntimeProbeSamplesSha256"] = probeHash;
        summaryJson["Runs"]!.AsArray()[0]!.AsObject()["RuntimeProbeSamplesSha256"] = probeHash;

        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        summaryJson["Runs"]!.AsArray()[0]!.AsObject()["RunResultSha256"] = Sha256File(fixture.RunResultPath);
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var auditRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!;
        var auditJson = GetSingleJsonObject(auditRoot);
        auditJson["Clean"] = "true";
        File.WriteAllText(fixture.AuditPath, auditRoot.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var sts1ModeLogCheckJson = JsonNode.Parse(File.ReadAllText(fixture.Sts1ModeCheckPath))!.AsObject();
        foreach (var checkNode in sts1ModeLogCheckJson["Checks"]!.AsArray())
        {
            checkNode!.AsObject()["Passed"] = "true";
        }

        File.WriteAllText(fixture.Sts1ModeCheckPath, sts1ModeLogCheckJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("plan_source_workspace_checked status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_source_workspace_checked_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_source_workspace_matches_installed_game status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_source_workspace_matches_installed_game_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_source_workspace_origin_matches_installed_game_pck status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_source_workspace_origin_matches_installed_game_pck_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("plan_source_workspace_not_runtime_proof_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_passed status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("summary_passed_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_summary_run_passed_true status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_summary_run_passed_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_launch_true status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_launch_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_passed_true status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_passed_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_main_menu_observation_passed status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_main_menu_observation_passed_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_main_menu_observation_process_exited_after_observation_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_main_menu_observation_hung_window_detected_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_main_menu_observation_stale_process_observed_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_main_menu_observation_no_log_growth_timeout_exceeded_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_main_menu_no_process_exit status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_main_menu_no_hung_window status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_runtime_observation_passed status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_runtime_observation_passed_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_runtime_observation_process_exited_after_observation_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_runtime_observation_hung_window_detected_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_runtime_observation_stale_process_observed_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_runtime_observation_no_log_growth_timeout_exceeded_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_runtime_observation_log_grew_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_runtime_log_grew status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_process_observed status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_process_observed_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_process_id_matches_expected_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_process_start_time_matches_expected_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_process_path_matches_expected_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_process_identity_matches_expected_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_log_exists_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_hung_window_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_responding_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_no_hung_window status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_no_not_responding status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_audit_clean_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_audit_clean status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_sts1_mode_log_check_checks_passed_bool status=fail", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsObjectRootAuditEvidence()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var auditRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!.AsArray();
        File.WriteAllText(fixture.AuditPath, auditRoot[0]!.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("run_0001_audit_array_fields_native status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_audit_clean status=fail", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsAuditMissingSchemaVersion()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var auditRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!;
        var auditJson = GetSingleJsonObject(auditRoot);
        auditJson.Remove("AuditSchemaVersion");
        File.WriteAllText(fixture.AuditPath, auditRoot.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("run_0001_audit_schema_fields_current status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_audit_has_single_schema_version status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_audit_clean status=fail", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsAuditWrongSignatureSetHash()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var auditRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!;
        var auditJson = GetSingleJsonObject(auditRoot);
        auditJson["SignatureSetSha256"] = new string('0', 64);
        File.WriteAllText(fixture.AuditPath, auditRoot.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("run_0001_audit_signature_set_matches_recomputed status=fail", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsAuditEmptySignatureHitVector()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var auditRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!;
        var auditJson = GetSingleJsonObject(auditRoot);
        auditJson["SignatureHits"] = new JsonArray();
        File.WriteAllText(fixture.AuditPath, auditRoot.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("run_0001_audit_signature_counts_match_recomputed status=fail", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsAuditSignatureHitMissingCount()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var auditRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!;
        var auditJson = GetSingleJsonObject(auditRoot);
        auditJson["SignatureHits"] = new JsonArray(
            new JsonObject
            {
                ["Name"] = "Godot ERROR line",
            });
        File.WriteAllText(fixture.AuditPath, auditRoot.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("run_0001_audit_numeric_fields_native status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_audit_clean status=fail", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsAuditSignatureHitStringCount()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var auditRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!;
        var auditJson = GetSingleJsonObject(auditRoot);
        auditJson["SignatureHits"] = new JsonArray(
            new JsonObject
            {
                ["Name"] = "Godot ERROR line",
                ["Count"] = "0",
            });
        File.WriteAllText(fixture.AuditPath, auditRoot.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("run_0001_audit_numeric_fields_native status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_audit_clean status=fail", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsAuditMissingLength()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var auditRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!;
        var auditJson = GetSingleJsonObject(auditRoot);
        auditJson.Remove("Length");
        File.WriteAllText(fixture.AuditPath, auditRoot.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("run_0001_audit_numeric_fields_native status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_audit_clean status=fail", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsMalformedSignalArrayEvidence()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();

        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
        summaryRunJson["FailureReasonCodes"] = "process_unresponsive";
        summaryRunJson["HangSignals"] = "process_unresponsive";

        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        runResultJson["FailureReasonCodes"] = "process_unresponsive";
        runResultJson["HangSignals"] = "process_unresponsive";
        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        summaryRunJson["RunResultSha256"] = Sha256File(fixture.RunResultPath);
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("summary_runs_array status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_summary_run_failure_reason_codes_array status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_summary_run_hang_signals_array status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_failure_reason_codes_array status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_hang_signals_array status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_failure_reason_codes_match_summary status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_run_result_hang_signals_match_summary status=pass", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsObjectShapedRuntimeProbeSamples()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();

        File.WriteAllText(
            fixture.RuntimeProbeSamplesPath,
            """
            {
              "Phase": "main-menu",
              "SampledAt": "2026-06-23T12:00:00.0000000Z",
              "LogExists": true,
              "LogLengthBytes": 1
            }
            """);

        var probeHash = Sha256File(fixture.RuntimeProbeSamplesPath);
        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        runResultJson["RuntimeProbeSamplesSha256"] = probeHash;
        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
        summaryRunJson["RuntimeProbeSamplesSha256"] = probeHash;
        summaryRunJson["RunResultSha256"] = Sha256File(fixture.RunResultPath);
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("run_0001_runtime_probe_samples_array status=fail", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierAllowsNullRespondingBeforeMainWindowObserved()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        var probeSamples = JsonNode.Parse(File.ReadAllText(fixture.RuntimeProbeSamplesPath))!.AsArray();
        var startupSample = probeSamples[0]!.AsObject();
        startupSample["MainWindowObserved"] = false;
        startupSample["Responding"] = null;

        File.WriteAllText(fixture.RuntimeProbeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        var probeHash = Sha256File(fixture.RuntimeProbeSamplesPath);
        runResultJson["RuntimeProbeSamplesSha256"] = probeHash;
        summaryRunJson["RuntimeProbeSamplesSha256"] = probeHash;
        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        summaryRunJson["RunResultSha256"] = Sha256File(fixture.RunResultPath);
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU", "-FailOnMismatch");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier should allow Responding=null before a main window is observed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("run_0001_runtime_probe_samples_responding_field_present status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_responding_bool status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_no_not_responding status=pass", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsNullRespondingAfterMainWindowObserved()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        var probeSamples = JsonNode.Parse(File.ReadAllText(fixture.RuntimeProbeSamplesPath))!.AsArray();
        var runtimeSample = probeSamples[1]!.AsObject();
        runtimeSample["MainWindowObserved"] = true;
        runtimeSample["Responding"] = null;

        File.WriteAllText(fixture.RuntimeProbeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        var probeHash = Sha256File(fixture.RuntimeProbeSamplesPath);
        runResultJson["RuntimeProbeSamplesSha256"] = probeHash;
        summaryRunJson["RuntimeProbeSamplesSha256"] = probeHash;
        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        summaryRunJson["RunResultSha256"] = Sha256File(fixture.RunResultPath);
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var result = fixture.RunVerifier("-ExpectedAncientIds", "VAKUU");

        Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
        Assert.Contains("run_0001_runtime_probe_samples_responding_field_present status=pass", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_responding_bool status=fail", result.Output, StringComparison.Ordinal);
        Assert.Contains("run_0001_runtime_probe_samples_no_not_responding status=fail", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlayProbeObjectShapeEvidence()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        File.WriteAllText(
            fixture.RuntimeProbeSamplesPath,
            """
            {
              "Phase": "main-menu",
              "SampledAt": "2026-06-23T12:00:00.0000000Z",
              "LogExists": true,
              "LogLengthBytes": 1
            }
            """);

        var probeHash = Sha256File(fixture.RuntimeProbeSamplesPath);
        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        runResultJson["RuntimeProbeSamplesSha256"] = probeHash;
        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
        summaryRunJson["RuntimeProbeSamplesSha256"] = probeHash;
        summaryRunJson["RunResultSha256"] = Sha256File(fixture.RunResultPath);
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-probe-object-shape.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_samples_shape_invalid");
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsScalarGameNativeAutoSlayExpectedAncientIds()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        var planJson = JsonNode.Parse(File.ReadAllText(fixture.PlanPath))!.AsObject();
        planJson["ExpectedAncientIds"] = "VAKUU";
        File.WriteAllText(fixture.PlanPath, planJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        summaryJson["ExpectedAncientIds"] = "VAKUU";
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-expected-ancient-ids-shape.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();
        var malformedFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "autoslay_expected_ancient_ids_array_malformed");
        var summaryPlanFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "autoslay_summary_plan_mismatch");

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("Sts1ModeLogCheckTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Contains("ExpectedAncientIds", malformedFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
        Assert.Contains("ExpectedAncientIdsArrayFieldsMalformed", summaryPlanFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlaySummaryRunIdentityDrift()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
        summaryRunJson["Seed"] = "STALESEED";
        summaryRunJson["EventKind"] = "StaleEvent";
        summaryRunJson["AncientId"] = "URDA";
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-summary-run-identity-drift.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();
        var identityFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "autoslay_summary_run_identity_mismatch");

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Equal("RuntimeHarness", identityFinding.GetProperty("OwnerArea").GetString());
        Assert.Equal("blocking", identityFinding.GetProperty("Severity").GetString());
        Assert.Contains("Seed:summary='STALESEED'", identityFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
        Assert.Contains("EventKind:summary='StaleEvent'", identityFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
        Assert.Contains("AncientId:summary='URDA'", identityFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlayMalformedAuditSchema()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var auditRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!;
        var auditJson = auditRoot is JsonArray auditArray
            ? auditArray[0]!.AsObject()
            : auditRoot.AsObject();
        auditJson["Clean"] = "true";
        auditJson["Length"] = auditJson["Length"]!.ToString();
        auditJson["SignatureHits"] = new JsonObject
        {
            ["Name"] = "fixture-zero-count",
            ["Count"] = 0,
        };
        File.WriteAllText(fixture.AuditPath, auditRoot.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-malformed-audit-schema.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();
        var arrayFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "godot_log_audit_array_malformed");
        var boolFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "godot_log_audit_boolean_malformed");
        var numericFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "godot_log_audit_numeric_malformed");

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AuditTrustedForOwner").GetBoolean());
        Assert.Equal("Runtime.Unknown", iteration.GetProperty("OwnerAreaFromLog").GetString());
        Assert.Equal("RuntimeHarness", arrayFinding.GetProperty("OwnerArea").GetString());
        Assert.Equal("RuntimeHarness", boolFinding.GetProperty("OwnerArea").GetString());
        Assert.Equal("RuntimeHarness", numericFinding.GetProperty("OwnerArea").GetString());
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlayObjectRootAuditEvidence()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var auditRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!.AsArray();
        File.WriteAllText(fixture.AuditPath, auditRoot[0]!.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-object-root-audit.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();
        var arrayFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "godot_log_audit_array_malformed");

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AuditTrustedForOwner").GetBoolean());
        Assert.Equal("Runtime.Unknown", iteration.GetProperty("OwnerAreaFromLog").GetString());
        Assert.Equal("RuntimeHarness", arrayFinding.GetProperty("OwnerArea").GetString());
        Assert.Contains("root and SignatureHits must be retained as native JSON arrays", arrayFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlayAuditMissingSchemaVersion()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var auditRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!;
        var auditJson = GetSingleJsonObject(auditRoot);
        auditJson.Remove("AuditSchemaVersion");
        File.WriteAllText(fixture.AuditPath, auditRoot.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-audit-missing-schema-version.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();
        var schemaFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "godot_log_audit_schema_malformed");

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AuditTrustedForOwner").GetBoolean());
        Assert.Equal("Runtime.Unknown", iteration.GetProperty("OwnerAreaFromLog").GetString());
        Assert.Contains("AuditSchemaVersion must be native integer 2", schemaFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlayAuditWrongSignatureSetHash()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var auditRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!;
        var auditJson = GetSingleJsonObject(auditRoot);
        auditJson["SignatureSetSha256"] = new string('0', 64);
        File.WriteAllText(fixture.AuditPath, auditRoot.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-audit-wrong-signature-set.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();
        var mismatchFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "godot_log_audit_recomputed_mismatch");

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AuditTrustedForOwner").GetBoolean());
        Assert.Equal("Runtime.Unknown", iteration.GetProperty("OwnerAreaFromLog").GetString());
        Assert.Contains("rule-set hash", mismatchFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlayAuditEmptySignatureHitVector()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var auditRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!;
        var auditJson = GetSingleJsonObject(auditRoot);
        auditJson["SignatureHits"] = new JsonArray();
        File.WriteAllText(fixture.AuditPath, auditRoot.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-audit-empty-signature-vector.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();
        var mismatchFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "godot_log_audit_recomputed_mismatch");

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AuditTrustedForOwner").GetBoolean());
        Assert.Equal("Runtime.Unknown", iteration.GetProperty("OwnerAreaFromLog").GetString());
        Assert.Contains("signature vector", mismatchFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlayAuditSignatureHitMissingCount()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var auditRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!;
        var auditJson = GetSingleJsonObject(auditRoot);
        auditJson["SignatureHits"] = new JsonArray(
            new JsonObject
            {
                ["Name"] = "Godot ERROR line",
            });
        File.WriteAllText(fixture.AuditPath, auditRoot.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-audit-missing-count.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();
        var numericFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "godot_log_audit_numeric_malformed");

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AuditTrustedForOwner").GetBoolean());
        Assert.Equal("Runtime.Unknown", iteration.GetProperty("OwnerAreaFromLog").GetString());
        Assert.Equal("RuntimeHarness", numericFinding.GetProperty("OwnerArea").GetString());
        Assert.Contains("SignatureHits[].Count must be retained as native JSON integers", numericFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlayAuditSignatureHitStringCount()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var auditRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!;
        var auditJson = GetSingleJsonObject(auditRoot);
        auditJson["SignatureHits"] = new JsonArray(
            new JsonObject
            {
                ["Name"] = "Godot ERROR line",
                ["Count"] = "0",
            });
        File.WriteAllText(fixture.AuditPath, auditRoot.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-audit-string-count.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();
        var numericFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "godot_log_audit_numeric_malformed");

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AuditTrustedForOwner").GetBoolean());
        Assert.Equal("Runtime.Unknown", iteration.GetProperty("OwnerAreaFromLog").GetString());
        Assert.Equal("RuntimeHarness", numericFinding.GetProperty("OwnerArea").GetString());
        Assert.Contains("Length and SignatureHits[].Count must be retained as native JSON integers", numericFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlayAuditMissingLength()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var auditRoot = JsonNode.Parse(File.ReadAllText(fixture.AuditPath))!;
        var auditJson = GetSingleJsonObject(auditRoot);
        auditJson.Remove("Length");
        File.WriteAllText(fixture.AuditPath, auditRoot.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-audit-missing-length.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();
        var numericFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "godot_log_audit_numeric_malformed");

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AuditTrustedForOwner").GetBoolean());
        Assert.Equal("Runtime.Unknown", iteration.GetProperty("OwnerAreaFromLog").GetString());
        Assert.Equal("RuntimeHarness", numericFinding.GetProperty("OwnerArea").GetString());
        Assert.Contains("Length and SignatureHits[].Count must be retained as native JSON integers", numericFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerAllowsGameNativeAutoSlayNullRespondingBeforeMainWindowObserved()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        var probeSamples = JsonNode.Parse(File.ReadAllText(fixture.RuntimeProbeSamplesPath))!.AsArray();
        var startupSample = probeSamples[0]!.AsObject();
        startupSample["MainWindowObserved"] = false;
        startupSample["Responding"] = null;

        File.WriteAllText(fixture.RuntimeProbeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        var probeHash = Sha256File(fixture.RuntimeProbeSamplesPath);
        runResultJson["RuntimeProbeSamplesSha256"] = probeHash;
        summaryRunJson["RuntimeProbeSamplesSha256"] = probeHash;
        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        summaryRunJson["RunResultSha256"] = Sha256File(fixture.RunResultPath);
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-null-responding-before-window.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

        Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_boolean_malformed");
        Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_not_responding");
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlayNullRespondingAfterMainWindowObserved()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        var probeSamples = JsonNode.Parse(File.ReadAllText(fixture.RuntimeProbeSamplesPath))!.AsArray();
        var runtimeSample = probeSamples[1]!.AsObject();
        runtimeSample["MainWindowObserved"] = true;
        runtimeSample["Responding"] = null;

        File.WriteAllText(fixture.RuntimeProbeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        var probeHash = Sha256File(fixture.RuntimeProbeSamplesPath);
        runResultJson["RuntimeProbeSamplesSha256"] = probeHash;
        summaryRunJson["RuntimeProbeSamplesSha256"] = probeHash;
        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        summaryRunJson["RunResultSha256"] = Sha256File(fixture.RunResultPath);
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-null-responding-after-window.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_boolean_malformed");
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_not_responding");
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlayProbeHungAndNotRespondingEvidence()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        var probeSamples = JsonNode.Parse(File.ReadAllText(fixture.RuntimeProbeSamplesPath))!.AsArray();
        var runtimeSample = probeSamples[1]!.AsObject();
        runtimeSample["HungWindow"] = true;
        runtimeSample["Responding"] = false;

        File.WriteAllText(fixture.RuntimeProbeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        var probeHash = Sha256File(fixture.RuntimeProbeSamplesPath);
        runResultJson["RuntimeProbeSamplesSha256"] = probeHash;
        summaryRunJson["RuntimeProbeSamplesSha256"] = probeHash;
        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        summaryRunJson["RunResultSha256"] = Sha256File(fixture.RunResultPath);
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-probe-hung-not-responding.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_hung_window");
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_not_responding");
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlayProbeTimelineDefects()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        var probeSamples = JsonNode.Parse(File.ReadAllText(fixture.RuntimeProbeSamplesPath))!.AsArray();
        var firstSample = probeSamples[0]!.AsObject();
        var secondSample = probeSamples[1]!.AsObject();
        firstSample["Phase"] = "runtime";
        firstSample["SampledAt"] = "2026-06-18T10:00:25Z";
        firstSample["LogLastWriteTimeUtc"] = "2026-06-18T10:00:25Z";
        firstSample["LogLengthBytes"] = fixture.AfterLaunchLogLength;
        secondSample["Phase"] = "main-menu";
        secondSample["SampledAt"] = "2026-06-18T10:00:04Z";
        secondSample["LogLastWriteTimeUtc"] = "2026-06-18T10:00:04Z";
        secondSample["LogLengthBytes"] = 1;

        File.WriteAllText(fixture.RuntimeProbeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        var probeHash = Sha256File(fixture.RuntimeProbeSamplesPath);
        runResultJson["RuntimeProbeSamplesSha256"] = probeHash;
        summaryRunJson["RuntimeProbeSamplesSha256"] = probeHash;
        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        summaryRunJson["RunResultSha256"] = Sha256File(fixture.RunResultPath);
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-probe-timeline.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_sampled_at_order_invalid");
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_phase_order_invalid");
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_log_length_regression");
        Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_timestamp_invalid");
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlayProbeNegativeLogLength()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        var probeSamples = JsonNode.Parse(File.ReadAllText(fixture.RuntimeProbeSamplesPath))!.AsArray();
        var runtimeSample = probeSamples[1]!.AsObject();
        runtimeSample["LogLengthBytes"] = -1;

        File.WriteAllText(fixture.RuntimeProbeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        var probeHash = Sha256File(fixture.RuntimeProbeSamplesPath);
        runResultJson["RuntimeProbeSamplesSha256"] = probeHash;
        summaryRunJson["RuntimeProbeSamplesSha256"] = probeHash;
        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        summaryRunJson["RunResultSha256"] = Sha256File(fixture.RunResultPath);
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-probe-negative-log-length.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_log_length_negative");
        Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_numeric_malformed");
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsMalformedGameNativeAutoSlayBooleanEvidence()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
        summaryRunJson["Passed"] = "false";
        summaryRunJson["FailureReasonCodes"] = new JsonArray(JsonValue.Create("process_unresponsive"));
        summaryRunJson["HangSignals"] = new JsonArray(JsonValue.Create("process_unresponsive"));

        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        runResultJson["Passed"] = "false";
        runResultJson["FailureReasonCodes"] = new JsonArray(JsonValue.Create("process_unresponsive"));
        runResultJson["HangSignals"] = new JsonArray(JsonValue.Create("process_unresponsive"));
        var mainMenuObservation = runResultJson["MainMenuObservation"]!.AsObject();
        mainMenuObservation["Passed"] = "true";
        mainMenuObservation["MainMenuReached"] = "true";
        mainMenuObservation["ProcessObserved"] = "true";
        mainMenuObservation["ProcessExitedAfterObservation"] = null;
        var runtimeObservation = runResultJson["RuntimeObservation"]!.AsObject();
        runtimeObservation["Passed"] = "true";
        runtimeObservation["ProcessObserved"] = "true";
        runtimeObservation["HungWindowDetected"] = "false";
        runtimeObservation["NoLogGrowthTimeoutExceeded"] = "false";
        runtimeObservation["LogGrew"] = "true";

        var probeSamples = JsonNode.Parse(File.ReadAllText(fixture.RuntimeProbeSamplesPath))!.AsArray();
        foreach (var sampleNode in probeSamples)
        {
            var sample = sampleNode!.AsObject();
            sample["ProcessObserved"] = "false";
            sample["ProcessIdMatchesExpected"] = "true";
            sample["ProcessStartTimeMatchesExpected"] = "true";
            sample["ProcessPathMatchesExpected"] = "true";
            sample["ProcessIdentityMatchesExpected"] = "true";
            sample["HungWindow"] = "";
            sample["Responding"] = "false";
            sample["LogExists"] = "true";
        }

        File.WriteAllText(fixture.RuntimeProbeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        var probeHash = Sha256File(fixture.RuntimeProbeSamplesPath);
        runResultJson["RuntimeProbeSamplesSha256"] = probeHash;
        summaryRunJson["RuntimeProbeSamplesSha256"] = probeHash;

        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        summaryRunJson["RunResultSha256"] = Sha256File(fixture.RunResultPath);
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-malformed-autoslay-booleans.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_passed_boolean_malformed");
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_boolean_malformed");
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_main_menu_observation_unhealthy");
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_observation_unhealthy");
        Assert.Equal("RuntimeHarness", FindFindingOwner(iteration, "process_unresponsive"));
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlayRunResultBooleanMalformedWithoutSummaryRow()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        runResultJson["Passed"] = "true";
        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-direct-passed-string.json");
        var result = RunPowerShell(script, "-IterationDir", fixture.RunDir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();
        var malformedFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "autoslay_passed_boolean_malformed");

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Equal("RuntimeHarness", malformedFinding.GetProperty("OwnerArea").GetString());
        Assert.Contains("run-result.json.Passed must be retained as a native JSON boolean", malformedFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsMalformedGameNativeAutoSlayProbeNumericEvidence()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        var probeSamples = JsonNode.Parse(File.ReadAllText(fixture.RuntimeProbeSamplesPath))!.AsArray();
        foreach (var sampleNode in probeSamples)
        {
            var sample = sampleNode!.AsObject();
            sample["LogLengthBytes"] = JsonScalarValueToInvariantString(sample["LogLengthBytes"]);
            sample["ProcessId"] = JsonScalarValueToInvariantString(sample["ProcessId"]);
            sample["StaleProcessCount"] = JsonScalarValueToInvariantString(sample["StaleProcessCount"]);
            sample["CurrentProcessCount"] = JsonScalarValueToInvariantString(sample["CurrentProcessCount"]);
        }

        File.WriteAllText(fixture.RuntimeProbeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        var probeHash = Sha256File(fixture.RuntimeProbeSamplesPath);
        runResultJson["RuntimeProbeSamplesSha256"] = probeHash;
        summaryRunJson["RuntimeProbeSamplesSha256"] = probeHash;
        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        summaryRunJson["RunResultSha256"] = Sha256File(fixture.RunResultPath);
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-malformed-autoslay-probe-numeric.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_numeric_malformed");
        Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_log_length_negative");
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlayProbeIntegerValuedDecimalEvidence()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        var probeSamplesJson = File.ReadAllText(fixture.RuntimeProbeSamplesPath);
        foreach (var fieldName in new[] { "LogLengthBytes", "ProcessId", "StaleProcessCount", "CurrentProcessCount" })
        {
            probeSamplesJson = Regex.Replace(
                probeSamplesJson,
                $"\"{fieldName}\"\\s*:\\s*\\d+",
                $"\"{fieldName}\": 1.0",
                RegexOptions.CultureInvariant);
        }

        File.WriteAllText(fixture.RuntimeProbeSamplesPath, probeSamplesJson);
        var probeHash = Sha256File(fixture.RuntimeProbeSamplesPath);

        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        runResultJson["RuntimeProbeSamplesSha256"] = probeHash;
        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
        summaryRunJson["RuntimeProbeSamplesSha256"] = probeHash;
        summaryRunJson["RunResultSha256"] = Sha256File(fixture.RunResultPath);
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-decimal-autoslay-probe-numeric.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_numeric_malformed");
        Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_log_length_negative");
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsMalformedGameNativeAutoSlaySignalArrays()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
        summaryRunJson["Passed"] = false;
        summaryRunJson["FailureReasonCodes"] = "process_unresponsive";
        summaryRunJson["HangSignals"] = "process_unresponsive";

        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        runResultJson["Passed"] = false;
        runResultJson["FailureReasonCodes"] = "process_unresponsive";
        runResultJson["HangSignals"] = "process_unresponsive";
        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        summaryRunJson["RunResultSha256"] = Sha256File(fixture.RunResultPath);
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-malformed-autoslay-signal-arrays.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_signal_array_malformed");
        Assert.Equal("RuntimeHarness", FindFindingOwner(iteration, "process_unresponsive"));
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsMissingGameNativeAutoSlaySignalArrays()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        var summaryRunJson = summaryJson["Runs"]!.AsArray()[0]!.AsObject();
        Assert.True(summaryRunJson.Remove("FailureReasonCodes"));
        Assert.True(summaryRunJson.Remove("HangSignals"));

        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        Assert.True(runResultJson.Remove("FailureReasonCodes"));
        Assert.True(runResultJson.Remove("HangSignals"));
        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        summaryRunJson["RunResultSha256"] = Sha256File(fixture.RunResultPath);
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-missing-autoslay-signal-arrays.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_signal_array_malformed");
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsDirectGameNativeAutoSlayMissingSignalArrays()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        var runResultJson = JsonNode.Parse(File.ReadAllText(fixture.RunResultPath))!.AsObject();
        Assert.True(runResultJson.Remove("FailureReasonCodes"));
        Assert.True(runResultJson.Remove("HangSignals"));
        File.WriteAllText(fixture.RunResultPath, runResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-direct-autoslay-missing-signal-arrays.json");
        var result = RunPowerShell(script, "-IterationDir", fixture.RunDir, "-OutFile", outputPath);

        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_signal_array_malformed");
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsSharedMalformedAndDriftedArtifacts()
    {
        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            var rootAutoSlayLogPath = Path.Combine(fixture.Workdir, "autoslay.log");
            File.Copy(fixture.AutoSlayLogPath, rootAutoSlayLogPath, overwrite: true);
            File.WriteAllText(fixture.RunResultPath, fixture.OriginalRunResultJson.Replace("\"AutoSlayLogPath\": \"run-0001/autoslay.log\"", "\"AutoSlayLogPath\": \"autoslay.log\"", StringComparison.Ordinal));
            File.WriteAllText(fixture.SummaryPath, fixture.OriginalSummaryJson.Replace("\"AutoSlayLogPath\": \"run-0001/autoslay.log\"", "\"AutoSlayLogPath\": \"autoslay.log\"", StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_autoslay_log_under_evidence_dir status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_autoslay_log_under_run_dir status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_autoslay_log_leaf_expected status=pass", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            var rootRuntimeProbeSamplesPath = Path.Combine(fixture.Workdir, "runtime-probe-samples.json");
            File.Copy(fixture.RuntimeProbeSamplesPath, rootRuntimeProbeSamplesPath, overwrite: true);
            File.WriteAllText(fixture.RunResultPath, fixture.OriginalRunResultJson.Replace("\"RuntimeProbeSamplesPath\": \"run-0001/runtime-probe-samples.json\"", "\"RuntimeProbeSamplesPath\": \"runtime-probe-samples.json\"", StringComparison.Ordinal));
            File.WriteAllText(fixture.SummaryPath, fixture.OriginalSummaryJson.Replace("\"RuntimeProbeSamplesPath\": \"run-0001/runtime-probe-samples.json\"", "\"RuntimeProbeSamplesPath\": \"runtime-probe-samples.json\"", StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_runtime_probe_samples_under_evidence_dir status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_under_run_dir status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_leaf_expected status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_runtime_probe_samples_path_matches_summary status=pass", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            var shadowRunDir = Path.Combine(fixture.Workdir, "shadow", "run-0001");
            Directory.CreateDirectory(shadowRunDir);
            foreach (var fileName in new[]
            {
                "autoslay.log",
                "godot.log.before",
                "godot.log.after-launch",
                "godot.log.current-iteration",
                "runtime-probe-samples.json"
            })
            {
                File.Copy(Path.Combine(fixture.RunDir, fileName), Path.Combine(shadowRunDir, fileName), overwrite: true);
            }

            var shadowCurrentLogPath = Path.Combine(shadowRunDir, "godot.log.current-iteration");
            var shadowAuditPath = Path.Combine(shadowRunDir, "godot-log-audit.json");
            var shadowSts1ModeCheckPath = Path.Combine(shadowRunDir, "sts1-mode-log-check.json");
            File.WriteAllText(shadowAuditPath, ToBoundAuditJson(shadowCurrentLogPath, """{"SignatureHits":[]}"""));
            WriteSts1ModeLogCheckJson(
                "Off",
                shadowCurrentLogPath,
                shadowAuditPath,
                shadowSts1ModeCheckPath,
                expectedPackageVersion: GameNativeAutoSlayFixture.PackageVersion,
                expectedGameVersion: GameNativeAutoSlayFixture.GameVersion,
                expectedRitsuLibVersion: GameNativeAutoSlayFixture.RitsuLibVersion,
                expectedRitsuCompatBranch: GameNativeAutoSlayFixture.RitsuCompatBranch);

            var shadowRunResultPath = Path.Combine(shadowRunDir, "run-result.json");
            var shadowRunResultJson = fixture.OriginalRunResultJson.Replace("\"run-0001/", "\"shadow/run-0001/", StringComparison.Ordinal);
            File.WriteAllText(shadowRunResultPath, shadowRunResultJson);
            var shadowRunResultHash = Sha256File(shadowRunResultPath);
            var shadowSummaryJson = fixture.OriginalSummaryJson
                .Replace("\"run-0001/", "\"shadow/run-0001/", StringComparison.Ordinal)
                .Replace(JsonSerializer.Serialize(fixture.RunResultHash), JsonSerializer.Serialize(shadowRunResultHash), StringComparison.Ordinal);
            File.WriteAllText(fixture.SummaryPath, shadowSummaryJson);

            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_run_result_under_evidence_dir status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_parent_expected status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_path_matches_expected_per_seed_dir status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_autoslay_log_under_run_dir status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_current_iteration_log_under_run_dir status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_path_matches_expected_per_seed_file status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_autoslay_log_path_matches_expected_per_seed_file status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_before_log_path_matches_expected_per_seed_file status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_after_launch_log_path_matches_expected_per_seed_file status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_current_iteration_log_path_matches_expected_per_seed_file status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_audit_path_matches_expected_per_seed_file status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_sts1_mode_check_path_matches_expected_per_seed_file status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.AppendAllText(fixture.RunResultPath, " ");
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_run_result_sha256_recorded status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_sha256_matches_retained_file status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.AppendAllText(fixture.RuntimeProbeSamplesPath, " ");
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_runtime_probe_samples_sha256_recorded status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_sha256_matches_retained_file status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_runtime_probe_samples_sha256_matches_summary status=pass", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(fixture.SummaryPath, fixture.OriginalSummaryJson.Replace("\"RuntimeProbeSamplesPath\": \"run-0001/runtime-probe-samples.json\"", "\"RuntimeProbeSamplesPath\": \"\\u0000bad-runtime-probe-samples\"", StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_runtime_probe_samples_path_present status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_under_evidence_dir status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_exists status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(fixture.RunResultPath, fixture.OriginalRunResultJson.Replace("\"RuntimeProbeSamplesPath\": \"run-0001/runtime-probe-samples.json\"", "\"RuntimeProbeSamplesPath\": \"\\u0000bad-runtime-probe-samples\"", StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_run_result_runtime_probe_samples_path_matches_summary status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(
                fixture.SummaryPath,
                fixture.OriginalSummaryJson.Replace(JsonSerializer.Serialize(fixture.AfterLaunchLogHash), JsonSerializer.Serialize(new string('0', 64)), StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_after_launch_log_sha256_matches status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_after_launch_log_sha256_matches_summary status=fail", result.Output, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsRuntimeObservationDrift()
    {
        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(fixture.RunResultPath, fixture.OriginalRunResultJson.Replace("\"NoLogGrowthTimeoutExceeded\": false", "\"NoLogGrowthTimeoutExceeded\": true", StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_run_result_main_menu_no_log_growth_timeout status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_runtime_no_log_growth_timeout status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(fixture.RunResultPath, fixture.OriginalRunResultJson.Replace("\"LogGrew\": true", "\"LogGrew\": false", StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_run_result_runtime_log_grew status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(fixture.RunResultPath, fixture.OriginalRunResultJson.Replace("\"ProcessId\": 4242", "\"ProcessId\": 5151", StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_run_result_process_id_matches_runtime_probe_samples status=fail", result.Output, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsRuntimeProbeTimestampAndPhaseDrift()
    {
        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(
                fixture.RuntimeProbeSamplesPath,
                fixture.OriginalRuntimeProbeSamplesJson
                    .Replace("\"SampledAt\": \"2026-06-18T10:00:05Z\"", "\"SampledAt\": \"not-a-time\"", StringComparison.Ordinal)
                    .Replace("\"LogLastWriteTimeUtc\": \"2026-06-18T10:00:05Z\"", "\"LogLastWriteTimeUtc\": \"not-a-time\"", StringComparison.Ordinal)
                    .Replace("\"LogLastWriteTimeUtc\": \"2026-06-18T10:00:25Z\"", "\"LogLastWriteTimeUtc\": \"2999-01-01T00:00:00Z\"", StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_runtime_probe_samples_sampled_at_parseable status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_log_last_write_parseable_when_log_exists status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_log_last_write_not_after_sampled_at status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(
                fixture.RuntimeProbeSamplesPath,
                Regex.Replace(
                    fixture.OriginalRuntimeProbeSamplesJson
                        .Replace("\"Phase\": \"main-menu\"", "\"Phase\": \"__TEMP_PHASE__\"", StringComparison.Ordinal)
                        .Replace("\"Phase\": \"runtime\"", "\"Phase\": \"main-menu\"", StringComparison.Ordinal)
                        .Replace("\"Phase\": \"__TEMP_PHASE__\"", "\"Phase\": \"runtime\"", StringComparison.Ordinal)
                        .Replace("\"SampledAt\": \"2026-06-18T10:00:25Z\"", "\"SampledAt\": \"2026-06-18T10:00:04Z\"", StringComparison.Ordinal)
                        .Replace("\"LogLastWriteTimeUtc\": \"2026-06-18T10:00:25Z\"", "\"LogLastWriteTimeUtc\": \"2026-06-18T10:00:04Z\"", StringComparison.Ordinal),
                    "(\"Phase\": \"main-menu\"[\\s\\S]*?\"LogLengthBytes\": )\\d+",
                    "${1}1",
                    RegexOptions.CultureInvariant));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_runtime_probe_samples_sampled_at_parseable status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_sampled_at_nondecreasing status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_phase_ordered status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_log_length_nondecreasing_when_log_exists status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(
                fixture.RuntimeProbeSamplesPath,
                Regex.Replace(
                    fixture.OriginalRuntimeProbeSamplesJson,
                    "(\"Phase\": \"runtime\"[\\s\\S]*?\"LogLengthBytes\": )\\d+",
                    "${1}-1",
                    RegexOptions.CultureInvariant));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_runtime_probe_samples_log_length_nonnegative_when_log_exists status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            var driftedProbePath = Path.Combine(fixture.Workdir, "other", "SlayTheSpire2.exe");
            File.WriteAllText(
                fixture.RuntimeProbeSamplesPath,
                fixture.OriginalRuntimeProbeSamplesJson
                    .Replace("\"ExpectedGameProcessStartTimeUtc\": \"2026-06-18T09:59:50Z\"", "\"ExpectedGameProcessStartTimeUtc\": \"2026-06-18T09:59:51Z\"", StringComparison.Ordinal)
                    .Replace($"\"ExpectedGameProcessPath\": {JsonSerializer.Serialize(fixture.GameProcessPath)}", $"\"ExpectedGameProcessPath\": {JsonSerializer.Serialize(driftedProbePath)}", StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_run_result_process_id_matches_runtime_probe_samples status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_expected_process_start_time_matches_run_result status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_expected_process_path_matches_run_result status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(
                fixture.RuntimeProbeSamplesPath,
                Regex.Replace(
                    fixture.OriginalRuntimeProbeSamplesJson,
                    "(\"Phase\": \"runtime\"[\\s\\S]*?\"LogLengthBytes\": )\\d+",
                    "${1}1",
                    RegexOptions.CultureInvariant));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_run_result_runtime_log_length_growth_matches_log_grew status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_log_growth_matches_runtime_observation status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(fixture.RuntimeProbeSamplesPath, fixture.OriginalRuntimeProbeSamplesJson.Replace("\"Phase\": \"main-menu\"", "\"Phase\": \"startup\"", StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_runtime_probe_samples_main_menu_phase_observed status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_runtime_phase_observed status=pass", result.Output, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsRunTimestampDrift()
    {
        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(fixture.RunResultPath, fixture.OriginalRunResultJson.Replace("\"EndTimestamp\": \"2026-06-18T10:00:30Z\"", "\"EndTimestamp\": \"not-a-timestamp\"", StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_run_result_end_timestamp_parseable status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_timestamp_order_valid status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(fixture.RunResultPath, fixture.OriginalRunResultJson.Replace("\"StartTimestamp\": \"2026-06-18T10:00:00Z\"", "\"StartTimestamp\": \"2026-06-18T10:01:00Z\"", StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_run_result_start_timestamp_parseable status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_end_timestamp_parseable status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_timestamp_order_valid status=fail", result.Output, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsPlanAndSourceDrift()
    {
        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(fixture.PlanPath, fixture.OriginalPlanJson.Replace("\"ExpectedPatchCount\": 25", "\"ExpectedPatchCount\": 24", StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("plan_expected_patch_count_positive status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("plan_expected_patch_count_matches_expected status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_expected_patch_count_in_current_log status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(fixture.PlanPath, fixture.OriginalPlanJson.Replace("\"HookId\": \"SpirePlus.AutoSlayHarness.Start\"", "\"HookId\": \"\"", StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("plan_hook_id_present status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_hook_id_matches_plan status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            var schemaMissingSourceReportJson = Regex.Replace(
                fixture.OriginalSourceWorkspaceReportJson,
                "\\s*\"SchemaVersion\": 1,\\r?\\n",
                string.Empty,
                RegexOptions.CultureInvariant);
            File.WriteAllText(fixture.SourceWorkspaceReportPath, schemaMissingSourceReportJson);
            var schemaMissingSourceWorkspaceReportHash = Sha256File(fixture.SourceWorkspaceReportPath);
            File.WriteAllText(fixture.PlanPath, fixture.OriginalPlanJson.Replace(fixture.SourceWorkspaceReportHash, schemaMissingSourceWorkspaceReportHash, StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("plan_source_workspace_check_hash_matches status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("plan_source_workspace_schema_version_one status=fail", result.Output, StringComparison.Ordinal);
        }

        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(
                fixture.SourceWorkspaceReportPath,
                fixture.OriginalSourceWorkspaceReportJson.Replace("\"SchemaVersion\": 1", "\"SchemaVersion\": \"1\"", StringComparison.Ordinal));
            var stringSchemaSourceWorkspaceReportHash = Sha256File(fixture.SourceWorkspaceReportPath);
            File.WriteAllText(fixture.PlanPath, fixture.OriginalPlanJson.Replace(fixture.SourceWorkspaceReportHash, stringSchemaSourceWorkspaceReportHash, StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("plan_source_workspace_check_hash_matches status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("plan_source_workspace_schema_version_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("plan_source_workspace_schema_version_one status=pass", result.Output, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsAutoSlayTraversalDrift()
    {
        using (var fixture = CreateGameNativeAutoSlayFixture())
        {
            File.WriteAllText(
                fixture.AutoSlayLogPath,
                string.Join(
                    Environment.NewLine,
                    $"12:00:00.000 [INFO] [AutoSlay] Starting run with seed={fixture.Seed}",
                    "12:00:01.000 [INFO] [AutoSlay] Entering Event room (Act 1, Floor 2)",
                    "12:00:02.000 [INFO] [AutoSlay] Action: Selecting event option: VAKUU (option: contract)",
                    $"12:00:03.000 [INFO] [AutoSlay] Run completed successfully with seed={fixture.Seed}"));
            var updatedAutoSlayLogHash = Sha256File(fixture.AutoSlayLogPath);
            File.WriteAllText(fixture.SummaryPath, fixture.OriginalSummaryJson.Replace(fixture.AutoSlayLogHash, updatedAutoSlayLogHash, StringComparison.Ordinal));
            File.WriteAllText(fixture.RunResultPath, fixture.OriginalRunResultJson.Replace(fixture.AutoSlayLogHash, updatedAutoSlayLogHash, StringComparison.Ordinal));
            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_autoslay_log_hash_matches status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_current_log_event_sequence_observed status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_autoslay_log_event_sequence_observed status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_event_room_traversal_observed status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("batch_event_room_traversal_observed status=fail", result.Output, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void RuntimeFailureAnalyzerAnalyzesRetainedAutoSlayRunOmittedBySummary()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var run2Dir = Path.Combine(fixture.Workdir, "run-0002");
        Directory.CreateDirectory(run2Dir);
        foreach (var sourceFile in Directory.GetFiles(fixture.RunDir))
        {
            File.Copy(sourceFile, Path.Combine(run2Dir, Path.GetFileName(sourceFile)), overwrite: true);
        }

        var run2Result = JsonNode.Parse(fixture.OriginalRunResultJson.Replace("run-0001", "run-0002", StringComparison.Ordinal))!.AsObject();
        run2Result["Passed"] = false;
        run2Result["FailureReasonCodes"] = new JsonArray(JsonValue.Create("process_unresponsive"));
        run2Result["HangSignals"] = new JsonArray(JsonValue.Create("process_unresponsive"));
        File.WriteAllText(Path.Combine(run2Dir, "run-result.json"), run2Result.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-omitted-autoslay-run.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);
        Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var run2 = FindIteration(root, 2);
        var summaryFinding = run2
            .GetProperty("Findings")
            .EnumerateArray()
            .Single(item => item.GetProperty("Signal").GetString() == "autoslay_summary_shape_invalid");

        Assert.Equal(2, root.GetProperty("AnalyzedIterationCount").GetInt32());
        Assert.Contains("autoslay-summary.json omitted retained run directory run-0002", summaryFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
        Assert.False(run2.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Equal("RuntimeHarness", FindFindingOwner(run2, "process_unresponsive"));
    }

    private static GameNativeAutoSlayFixture CreateGameNativeAutoSlayFixture() => new();

    private sealed class GameNativeAutoSlayFixture : IDisposable
    {
        public const string PackageVersion = "v0.1.0-private-beta.87";
        public const string GameVersion = "0.107.0";
        public const string RitsuLibVersion = "0.4.24";
        public const string RitsuCompatBranch = "0.107.0";
        public const string ExpectedPatchCount = "25";

        public GameNativeAutoSlayFixture()
        {
            Verifier = AssertRepoFileExists("scripts", "check-spire-plus-autoslay-packet.ps1");
            Seed = "AUTOSLAYSEED1";
            Workdir = Path.Combine(Path.GetTempPath(), "autoslay-packet-verifier-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Workdir);
            SourceWorkspaceReportPath = Path.Combine(Workdir, "local-godot-source-workspace-check.json");
            LauncherPath = Path.Combine(Workdir, "autoslay-launcher-proof.json");
            RunDir = Path.Combine(Workdir, "run-0001");
            Directory.CreateDirectory(RunDir);
            PlanPath = Path.Combine(Workdir, "autoslay-plan.json");
            SummaryPath = Path.Combine(Workdir, "autoslay-summary.json");
            RunResultPath = Path.Combine(RunDir, "run-result.json");
            AutoSlayLogPath = Path.Combine(RunDir, "autoslay.log");
            BeforeLogPath = Path.Combine(RunDir, "godot.log.before");
            AfterLogPath = Path.Combine(RunDir, "godot.log.after-launch");
            CurrentLogPath = Path.Combine(RunDir, "godot.log.current-iteration");
            AuditPath = Path.Combine(RunDir, "godot-log-audit.json");
            Sts1ModeCheckPath = Path.Combine(RunDir, "sts1-mode-log-check.json");
            RuntimeProbeSamplesPath = Path.Combine(RunDir, "runtime-probe-samples.json");
            GameProcessPath = Path.Combine(Workdir, "SlayTheSpire2.exe");

            WriteSourceWorkspaceReport();
            SourceWorkspaceReportHash = Sha256File(SourceWorkspaceReportPath);
            File.WriteAllText(
                LauncherPath,
                """
                {
                  "LauncherKind": "SpirePlusDebugHook",
                  "HookId": "SpirePlus.AutoSlayHarness.Start",
                  "HookAssembly": "EZMicroBalanceCode",
                  "InvocationCommand": "SpirePlus.AutoSlayHarness.Start -> AutoSlayer.Start(seed, logFile)"
                }
                """);
            var launcherHash = Sha256File(LauncherPath);

            OriginalAutoSlayLog = string.Join(
                Environment.NewLine,
                $"12:00:00.000 [INFO] [AutoSlay] Starting run with seed={Seed}",
                "12:00:01.000 [INFO] [AutoSlay] Entering Event room (Act 1, Floor 2)",
                "12:00:01.500 [INFO] [AutoSlay] Detected Ancient event, clicking through dialogue: VAKUU",
                "12:00:02.000 [INFO] [AutoSlay] Action: Selecting event option: VAKUU (option: contract)",
                $"12:00:03.000 [INFO] [AutoSlay] Run completed successfully with seed={Seed}") + Environment.NewLine;
            BeforeLog = "11:59:59.000 [INFO] [Previous] old shared log line" + Environment.NewLine;
            OriginalCurrentLog = string.Join(
                Environment.NewLine,
                PackageVersion,
                "release = v0.107.0",
                "RitsuLib Version: 0.4.24 [compat branch: 0.107.0]",
                "[INFO] [EZMicroBalance] [Patcher - SpirePlus] Patch application complete: 25 applied, 0 ignored, 0 failed, 25 total",
                "[INFO] [EZMicroBalance] ModPatcher applied 25 patches (25 registered).",
                "StS1 events default Off; set SPIREPLUS_STS1_EVENT_MODE to enable.",
                "Feature Sts1Events bootstrap=disabled, live=Disabled",
                OriginalAutoSlayLog.TrimEnd()) + Environment.NewLine;
            OriginalAfterLog = BeforeLog + OriginalCurrentLog;
            File.WriteAllText(AutoSlayLogPath, OriginalAutoSlayLog);
            File.WriteAllText(BeforeLogPath, BeforeLog);
            File.WriteAllText(CurrentLogPath, OriginalCurrentLog);
            File.WriteAllText(AfterLogPath, OriginalAfterLog);

            BeforeLogLength = new FileInfo(BeforeLogPath).Length;
            AfterLaunchLogLength = new FileInfo(AfterLogPath).Length;
            CurrentLogLength = new FileInfo(CurrentLogPath).Length;
            AutoSlayLogHash = Sha256File(AutoSlayLogPath);
            BeforeLogHash = Sha256File(BeforeLogPath);
            AfterLaunchLogHash = Sha256File(AfterLogPath);
            CurrentLogHash = Sha256File(CurrentLogPath);

            OriginalRuntimeProbeSamplesJson = BuildRuntimeProbeSamples();
            File.WriteAllText(RuntimeProbeSamplesPath, OriginalRuntimeProbeSamplesJson);
            RuntimeProbeSamplesHash = Sha256File(RuntimeProbeSamplesPath);
            File.WriteAllText(AuditPath, ToBoundAuditJson(CurrentLogPath, """{"SignatureHits":[]}"""));
            WriteSts1ModeLogCheckJson(
                "Off",
                CurrentLogPath,
                AuditPath,
                Sts1ModeCheckPath,
                expectedPackageVersion: PackageVersion,
                expectedGameVersion: GameVersion,
                expectedRitsuLibVersion: RitsuLibVersion,
                expectedRitsuCompatBranch: RitsuCompatBranch);

            OriginalRunResultJson = BuildRunResultJson(launcherHash);
            OriginalPlanJson = BuildPlanJson(launcherHash);
            File.WriteAllText(RunResultPath, OriginalRunResultJson);
            RunResultHash = Sha256File(RunResultPath);
            OriginalSummaryJson = BuildSummaryJson();
            File.WriteAllText(PlanPath, OriginalPlanJson);
            File.WriteAllText(SummaryPath, OriginalSummaryJson);
            OriginalSourceWorkspaceReportJson = File.ReadAllText(SourceWorkspaceReportPath);
        }

        public string Verifier { get; }
        public string Seed { get; }
        public string Workdir { get; }
        public string RunDir { get; }
        public string SourceWorkspaceReportPath { get; }
        public string SourceWorkspaceReportHash { get; }
        public string LauncherPath { get; }
        public string PlanPath { get; }
        public string SummaryPath { get; }
        public string RunResultPath { get; }
        public string AutoSlayLogPath { get; }
        public string BeforeLogPath { get; }
        public string AfterLogPath { get; }
        public string CurrentLogPath { get; }
        public string AuditPath { get; }
        public string Sts1ModeCheckPath { get; }
        public string RuntimeProbeSamplesPath { get; }
        public string RuntimeProbeSamplesHash { get; }
        public string RunResultHash { get; }
        public string GameProcessPath { get; }
        public string OriginalSourceWorkspaceReportJson { get; }
        public string OriginalPlanJson { get; }
        public string OriginalSummaryJson { get; }
        public string OriginalRunResultJson { get; }
        public string OriginalRuntimeProbeSamplesJson { get; }
        public string OriginalAutoSlayLog { get; }
        public string BeforeLog { get; }
        public string OriginalCurrentLog { get; }
        public string OriginalAfterLog { get; }
        public string AutoSlayLogHash { get; }
        public string BeforeLogHash { get; }
        public string AfterLaunchLogHash { get; }
        public string CurrentLogHash { get; }
        public long BeforeLogLength { get; }
        public long AfterLaunchLogLength { get; }
        public long CurrentLogLength { get; }

        public (int ExitCode, string Output, string Error) RunVerifier(params string[] extraArguments)
        {
            var arguments = new List<string>
            {
                "-EvidenceDir",
                Workdir,
                "-ExpectedPackageVersion",
                PackageVersion,
                "-ExpectedGameVersion",
                GameVersion,
                "-ExpectedRitsuLibVersion",
                RitsuLibVersion,
                "-ExpectedRitsuCompatBranch",
                RitsuCompatBranch,
                "-ExpectedPatchCount",
                ExpectedPatchCount
            };
            arguments.AddRange(extraArguments);

            return RunPowerShell(Verifier, arguments.ToArray());
        }

        public (int ExitCode, string Output, string Error) RunVerifierWithoutDefaultTargetArguments(params string[] extraArguments)
        {
            var arguments = new List<string>
            {
                "-EvidenceDir",
                Workdir
            };
            arguments.AddRange(extraArguments);

            return RunPowerShell(Verifier, arguments.ToArray());
        }

        public string RewriteLogMetadata(
            string json,
            long currentLogLength,
            string currentLogHash,
            long afterLaunchLogLength,
            string afterLaunchLogHash) => json
                .Replace($"\"GodotLogAfterLaunchLengthBytes\": {AfterLaunchLogLength}", $"\"GodotLogAfterLaunchLengthBytes\": {afterLaunchLogLength}", StringComparison.Ordinal)
                .Replace(JsonSerializer.Serialize(AfterLaunchLogHash), JsonSerializer.Serialize(afterLaunchLogHash), StringComparison.Ordinal)
                .Replace($"\"GodotLogCurrentIterationLengthBytes\": {CurrentLogLength}", $"\"GodotLogCurrentIterationLengthBytes\": {currentLogLength}", StringComparison.Ordinal)
                .Replace(JsonSerializer.Serialize(CurrentLogHash), JsonSerializer.Serialize(currentLogHash), StringComparison.Ordinal);

        public void RefreshSummaryRunResultHash()
        {
            var summaryJson = File.ReadAllText(SummaryPath);
            File.WriteAllText(
                SummaryPath,
                summaryJson.Replace(
                    JsonSerializer.Serialize(RunResultHash),
                    JsonSerializer.Serialize(Sha256File(RunResultPath)),
                    StringComparison.Ordinal));
        }

        public void Dispose()
        {
            if (Directory.Exists(Workdir))
            {
                Directory.Delete(Workdir, recursive: true);
            }
        }

        private void WriteSourceWorkspaceReport()
        {
            File.WriteAllText(
                SourceWorkspaceReportPath,
                """
                {
                  "SchemaVersion": 1,
                  "CreatedAt": "2026-06-18T10:00:00Z",
                  "Passed": true,
                  "RepoRoot": "D:\\Game\\FOTN\\dev-the-spire",
                  "SourceRoot": "D:\\Game\\FOTN\\dev-the-spire\\source code",
                  "GameRoot": "E:\\Steam\\steamapps\\common\\Slay the Spire 2",
                  "Mismatches": [],
                  "Game": {
                    "Version": "v0.107.0",
                    "Commit": "fixture",
                    "Branch": "v0.107.0",
                    "MainAssemblyHash": "12345"
                  },
                  "RecoveredSource": {
                    "Version": "v0.107.0",
                    "Commit": "fixture",
                    "Branch": "v0.107.0",
                    "MainAssemblyHash": "12345",
                    "Disposition": "current",
                    "MatchesInstalledGame": true,
                    "OriginPckPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\SlayTheSpire2.pck",
                    "OriginMatchesInstalledGamePck": true
                  },
                  "RitsuLib": {
                    "Version": "0.4.24",
                    "CompatBranch": "0.107.0",
                    "RootPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib",
                    "ManifestPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\mod_manifest.json",
                    "ManifestSha256": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                    "VariantsPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\ritsulib-variants.json",
                    "VariantsSha256": "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
                    "ViewerPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\viewer\\index.html",
                    "VariantDirectory": "lib\\0.107.0",
                    "VariantAssembly": "STS2-RitsuLib.dll",
                    "VariantDllPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\lib\\0.107.0\\STS2-RitsuLib.dll",
                    "VariantDllSha256": "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",
                    "ExpectedVariantDllSha256": "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",
                    "CompatTargetPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\lib\\0.107.0\\compat-target.txt",
                    "CompatTargetText": "0.107.0"
                  },
                  "AutoSlay": {
                    "StartSeedLogFileSignature": true,
                    "NonInteractiveCheck": true,
                    "DebugSeedOverride": true,
                    "AutoCardSelector": true,
                    "AncientDialogueHandler": true,
                    "EventOptionSelectionLog": true,
                    "EventTriggeredCombatLog": true,
                    "EventCombatStartedLog": true
                  },
                  "EvidenceUsePolicy": {
                    "NoLaunch": true,
                    "NotRuntimeProof": true,
                    "RefreshSourceSnapshotBeforeCurrentApiClaims": false,
                    "LocalSourceReferenceOnly": true,
                    "AuthorizedLocalInstallOnly": true,
                    "AuthorizedSourceOriginVerified": true,
                    "ThirdPartyDumpsProhibited": true,
                    "RuntimeProofStillRequiresLaunchEvidence": true,
                    "GameNativeAutoSlayStillRequiresRuntimeLaunchEvidence": true
                  }
                }
                """);
        }

        private string BuildRuntimeProbeSamples() =>
            $$"""
            [
              {
                "Phase": "main-menu",
                "SampledAt": "2026-06-18T10:00:05Z",
                "ProcessId": 4242,
                "ProcessStartTimeUtc": "2026-06-18T09:59:50Z",
                "ProcessPath": {{JsonSerializer.Serialize(GameProcessPath)}},
                "ExpectedGameProcessId": 4242,
                "ExpectedGameProcessStartTimeUtc": "2026-06-18T09:59:50Z",
                "ExpectedGameProcessPath": {{JsonSerializer.Serialize(GameProcessPath)}},
                "ProcessIdMatchesExpected": true,
                "ProcessStartTimeMatchesExpected": true,
                "ProcessPathMatchesExpected": true,
                "ProcessIdentityMatchesExpected": true,
                "ProcessObserved": true,
                "MainWindowObserved": true,
                "HungWindow": false,
                "Responding": true,
                "LogExists": true,
                "LogLengthBytes": {{BeforeLogLength}},
                "LogLastWriteTimeUtc": "2026-06-18T10:00:05Z",
                "StaleProcessCount": 0,
                "CurrentProcessCount": 1,
                "UnknownStartTimeProcessCount": 0,
                "AmbiguousCurrentProcessCount": 0
              },
              {
                "Phase": "runtime",
                "SampledAt": "2026-06-18T10:00:25Z",
                "ProcessId": 4242,
                "ProcessStartTimeUtc": "2026-06-18T09:59:50Z",
                "ProcessPath": {{JsonSerializer.Serialize(GameProcessPath)}},
                "ExpectedGameProcessId": 4242,
                "ExpectedGameProcessStartTimeUtc": "2026-06-18T09:59:50Z",
                "ExpectedGameProcessPath": {{JsonSerializer.Serialize(GameProcessPath)}},
                "ProcessIdMatchesExpected": true,
                "ProcessStartTimeMatchesExpected": true,
                "ProcessPathMatchesExpected": true,
                "ProcessIdentityMatchesExpected": true,
                "ProcessObserved": true,
                "MainWindowObserved": true,
                "HungWindow": false,
                "Responding": true,
                "LogExists": true,
                "LogLengthBytes": {{AfterLaunchLogLength}},
                "LogLastWriteTimeUtc": "2026-06-18T10:00:25Z",
                "StaleProcessCount": 0,
                "CurrentProcessCount": 1,
                "UnknownStartTimeProcessCount": 0,
                "AmbiguousCurrentProcessCount": 0
              }
            ]
            """;

        private string BuildRunResultJson(string launcherHash) =>
            $$"""
            {
              "SchemaVersion": 1,
              "Launch": true,
              "RunnerKind": "GameNativeAutoSlay",
              "Invocation": "Spire Plus test hook calls AutoSlayer.Start(seed, logFile)",
              "LauncherKind": "SpirePlusDebugHook",
              "LauncherPath": "autoslay-launcher-proof.json",
              "LauncherSha256": {{JsonSerializer.Serialize(launcherHash)}},
              "HookId": "SpirePlus.AutoSlayHarness.Start",
              "HookAssembly": "EZMicroBalanceCode",
              "InvocationCommand": "SpirePlus.AutoSlayHarness.Start -> AutoSlayer.Start(seed, logFile)",
              "Seed": {{JsonSerializer.Serialize(Seed)}},
              "EventKind": "Ancient",
              "AncientId": "VAKUU",
              "Passed": true,
              "FailureReasonCodes": [],
              "HangSignals": [],
              "ProcessId": 4242,
              "ProcessStartTimeUtc": "2026-06-18T09:59:50Z",
              "ProcessPath": {{JsonSerializer.Serialize(GameProcessPath)}},
              "StartTimestamp": "2026-06-18T10:00:00Z",
              "EndTimestamp": "2026-06-18T10:00:30Z",
              "ExitCode": 0,
              "StaleProcessCount": 0,
              "RuntimeProbeSamplesPath": "run-0001/runtime-probe-samples.json",
              "RuntimeProbeSamplesSha256": {{JsonSerializer.Serialize(RuntimeProbeSamplesHash)}},
              "MainMenuObservation": {
                "Passed": true,
                "MainMenuReached": true,
                "ProcessObserved": true,
                "ProcessExitedAfterObservation": false,
                "HungWindowDetected": false,
                "StaleProcessObserved": false,
                "MaxStaleProcessCount": 0,
                "NoLogGrowthTimeoutExceeded": false,
                "LogObserved": true
              },
              "RuntimeObservation": {
                "Passed": true,
                "ProcessObserved": true,
                "ProcessExitedAfterObservation": false,
                "HungWindowDetected": false,
                "StaleProcessObserved": false,
                "MaxStaleProcessCount": 0,
                "NoLogGrowthTimeoutExceeded": false,
                "LogGrew": true,
                "LogInitialLengthBytes": {{BeforeLogLength}},
                "LogFinalLengthBytes": {{AfterLaunchLogLength}},
                "LogObserved": true
              },
              "AutoSlayLogPath": "run-0001/autoslay.log",
              "AutoSlayLogSha256": {{JsonSerializer.Serialize(AutoSlayLogHash)}},
              "GodotLogBeforePath": "run-0001/godot.log.before",
              "GodotLogBeforeLengthBytes": {{BeforeLogLength}},
              "GodotLogBeforeSha256": {{JsonSerializer.Serialize(BeforeLogHash)}},
              "GodotLogAfterLaunchPath": "run-0001/godot.log.after-launch",
              "GodotLogAfterLaunchLengthBytes": {{AfterLaunchLogLength}},
              "GodotLogAfterLaunchSha256": {{JsonSerializer.Serialize(AfterLaunchLogHash)}},
              "GodotLogCurrentIterationPath": "run-0001/godot.log.current-iteration",
              "GodotLogCurrentIterationLengthBytes": {{CurrentLogLength}},
              "GodotLogCurrentIterationSha256": {{JsonSerializer.Serialize(CurrentLogHash)}},
              "GodotLogAuditPath": "run-0001/godot-log-audit.json",
              "Sts1ModeLogCheckPath": "run-0001/sts1-mode-log-check.json"
            }
            """;

        private string BuildPlanJson(string launcherHash) =>
            $$"""
            {
              "SchemaVersion": 1,
              "RunnerKind": "GameNativeAutoSlay",
              "Invocation": "Spire Plus test hook calls AutoSlayer.Start(seed, logFile)",
              "LauncherKind": "SpirePlusDebugHook",
              "LauncherPath": "autoslay-launcher-proof.json",
              "LauncherSha256": {{JsonSerializer.Serialize(launcherHash)}},
              "HookId": "SpirePlus.AutoSlayHarness.Start",
              "HookAssembly": "EZMicroBalanceCode",
              "InvocationCommand": "SpirePlus.AutoSlayHarness.Start -> AutoSlayer.Start(seed, logFile)",
              "Sts1EventMode": "Off",
              "PackageVersion": {{JsonSerializer.Serialize(PackageVersion)}},
              "GameVersion": {{JsonSerializer.Serialize(GameVersion)}},
              "RitsuLibVersion": {{JsonSerializer.Serialize(RitsuLibVersion)}},
              "RitsuCompatBranch": {{JsonSerializer.Serialize(RitsuCompatBranch)}},
              "ExpectedPatchCount": {{ExpectedPatchCount}},
              "ExpectedAncientIds": ["VAKUU"],
              "Seeds": [{{JsonSerializer.Serialize(Seed)}}],
              "SourceWorkspaceCheckPath": "local-godot-source-workspace-check.json",
              "SourceWorkspaceCheckSha256": {{JsonSerializer.Serialize(SourceWorkspaceReportHash)}},
              "SourceWorkspace": {
                "Checked": true,
                "ReportPath": "local-godot-source-workspace-check.json",
                "ReportSha256": {{JsonSerializer.Serialize(SourceWorkspaceReportHash)}},
                "Passed": true,
                "SourceVersion": "v0.107.0",
                "SourceCommit": "fixture",
                "SourceBranch": "v0.107.0",
                "SourceMainAssemblyHash": "12345",
                "InstalledGameVersion": "v0.107.0",
                "InstalledGameCommit": "fixture",
                "InstalledGameBranch": "v0.107.0",
                "InstalledGameMainAssemblyHash": "12345",
                "Disposition": "current",
                "MatchesInstalledGame": true,
                "OriginPckPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\SlayTheSpire2.pck",
                "OriginMatchesInstalledGamePck": true,
                "RitsuLibVersion": "0.4.24",
                "RitsuLibCompatBranch": "0.107.0",
                "RitsuLibManifestPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\mod_manifest.json",
                "RitsuLibManifestSha256": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                "RitsuLibVariantsPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\ritsulib-variants.json",
                "RitsuLibVariantsSha256": "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
                "RitsuLibVariantDllPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\lib\\0.107.0\\STS2-RitsuLib.dll",
                "RitsuLibVariantDllSha256": "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",
                "RitsuLibExpectedVariantDllSha256": "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",
                "RitsuLibCompatTargetPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\lib\\0.107.0\\compat-target.txt",
                "RitsuLibCompatTargetText": "0.107.0",
                "RefreshSourceSnapshotBeforeCurrentApiClaims": false,
                "NotRuntimeProof": true,
                "AuthorizedSourceOriginVerified": true
              }
            }
            """;

        private string BuildSummaryJson() =>
            $$"""
            {
              "SchemaVersion": 1,
              "RunnerKind": "GameNativeAutoSlay",
              "Sts1EventMode": "Off",
              "PackageVersion": {{JsonSerializer.Serialize(PackageVersion)}},
              "GameVersion": {{JsonSerializer.Serialize(GameVersion)}},
              "RitsuLibVersion": {{JsonSerializer.Serialize(RitsuLibVersion)}},
              "RitsuCompatBranch": {{JsonSerializer.Serialize(RitsuCompatBranch)}},
              "ExpectedPatchCount": {{ExpectedPatchCount}},
              "ExpectedAncientIds": ["VAKUU"],
              "Passed": true,
              "TotalRuns": 1,
              "FailedRuns": 0,
              "AncientIdCounts": { "VAKUU": 1 },
              "Runs": [
                {
                  "Seed": {{JsonSerializer.Serialize(Seed)}},
                  "Passed": true,
                  "ExitCode": 0,
                  "EventKind": "Ancient",
                  "AncientId": "VAKUU",
                  "FailureReasonCodes": [],
                  "HangSignals": [],
                  "RunResultPath": "run-0001/run-result.json",
                  "RunResultSha256": {{JsonSerializer.Serialize(RunResultHash)}},
                  "RuntimeProbeSamplesPath": "run-0001/runtime-probe-samples.json",
                  "RuntimeProbeSamplesSha256": {{JsonSerializer.Serialize(RuntimeProbeSamplesHash)}},
                  "AutoSlayLogPath": "run-0001/autoslay.log",
                  "AutoSlayLogSha256": {{JsonSerializer.Serialize(AutoSlayLogHash)}},
                  "GodotLogBeforePath": "run-0001/godot.log.before",
                  "GodotLogBeforeLengthBytes": {{BeforeLogLength}},
                  "GodotLogBeforeSha256": {{JsonSerializer.Serialize(BeforeLogHash)}},
                  "GodotLogAfterLaunchPath": "run-0001/godot.log.after-launch",
                  "GodotLogAfterLaunchLengthBytes": {{AfterLaunchLogLength}},
                  "GodotLogAfterLaunchSha256": {{JsonSerializer.Serialize(AfterLaunchLogHash)}},
                  "GodotLogCurrentIterationPath": "run-0001/godot.log.current-iteration",
                  "GodotLogCurrentIterationLengthBytes": {{CurrentLogLength}},
                  "GodotLogCurrentIterationSha256": {{JsonSerializer.Serialize(CurrentLogHash)}},
                  "GodotLogAuditPath": "run-0001/godot-log-audit.json",
                  "Sts1ModeLogCheckPath": "run-0001/sts1-mode-log-check.json"
                }
              ]
            }
            """;
    }

    private static string Sha256File(string path) =>
        Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();

    private static string JsonScalarValueToInvariantString(JsonNode? node)
    {
        Assert.NotNull(node);
        var value = node!.GetValue<object?>();
        return value switch
        {
            JsonElement element => element.ValueKind == JsonValueKind.String
                ? element.GetString() ?? string.Empty
                : element.GetRawText(),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value?.ToString() ?? string.Empty,
        };
    }
}
