using System.Security.Cryptography;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class RuntimeMonkeyStabilityGuardTests
{
    [Fact]
    public void Beta135RuntimeBaselineScaffoldKeepsOwnerRunBoundary()
    {
        var scaffold = ReadRepoText("scripts", "new-beta135-runtime-baseline-evidence.ps1");
        var docs = ReadRepoText("docs", "testing", "beta135-runtime-baseline.md");
        var releaseVerifier = ReadRepoText("scripts", "verify-spire-plus-release-evidence.ps1");

        AssertSourceContains(
            scaffold,
            "pending-owner-run",
            "Set-ObjectProperty -Object $manifest -Name 'OwnerRunRequired' -Value $true",
            "DoesNotLaunchGame = $true",
            "godot.log.after-launch",
            "main-menu-screenshot.png",
            "Set-ObjectProperty -Object $manifest -Name 'GodotLogAfterLaunchRecord'",
            "Set-ObjectProperty -Object $manifest -Name 'MainMenuScreenshotRecord'",
            "Set-ObjectProperty -Object $manifest -Name 'RuntimeBaselineLogCheckRecord'",
            "Set-ObjectProperty -Object $manifest -Name 'DoesNotLaunchGame' -Value $true",
            "Get-Sts2PathFromDirectoryBuildProps",
            "Resolve-ConfiguredGameRoot",
            "$resolvedGameRoot = Resolve-ConfiguredGameRoot -Path $GameRoot",
            "Set-ObjectProperty -Object $manifest -Name 'GameRoot'",
            "Set-ObjectProperty -Object $manifest -Name 'SpirePlusInstallDir'",
            "Set-ObjectProperty -Object $manifest -Name 'RitsuLibInstallDir'",
            "Set-ObjectProperty -Object $manifest -Name 'PackageZip'",
            "Set-ObjectProperty -Object $manifest -Name 'GameRootAnchorMode'",
            "Set-ObjectProperty -Object $manifest -Name 'PackageAnchorMode'",
            "Set-ObjectProperty -Object $manifest -Name 'TrustAnchorMode'",
            "noncanonical-override-test-only",
            "Set-ObjectProperty -Object $manifest -Name 'SpirePlusFiles'",
            "Set-ObjectProperty -Object $manifest -Name 'RitsuLibFiles'",
            "Get-GitEvidence",
            "Git = $gitEvidence",
            "Set-ObjectProperty -Object $manifest -Name 'Git'",
            "OutRoot must stay under",
            "Refusing to reuse non-empty evidence directory",
            "Evidence path must not pass through a reparse point",
            "Evidence path must stay under repo root while checking reparse points",
            "function Assert-NoReparsePointInExistingPath",
            "Assert-NoReparsePointInExistingPath -Path $resolved -Description 'Artifact source path'",
            "Assert-LeafIsNotReparsePoint -Path $resolvedDestination -Description 'Artifact destination'",
            "runtime-baseline-log-markers-checked",
            "Checker did not record successful runtime baseline status in run-manifest.json",
            "$preserveCheckerValidatedAnchors = $false",
            "if (-not $preserveCheckerValidatedAnchors)",
            "Do not refresh scaffold trust anchors after checker success",
            "ExpectedModManifestIds = @('EZMicroBalance', 'STS2-RitsuLib')",
            "-FailOnMismatch",
            "-GameRoot $resolvedGameRoot",
            "ForbiddenClaims = @('release-ready', 'live-gameplay-ready', 'save-load-ready', 'co-op-ready')");
        Assert.DoesNotContain("owner-log-provided-checked", scaffold, StringComparison.OrdinalIgnoreCase);

        AssertSourceContains(
            docs,
            "Codex prepares the evidence scaffold and offline log checker.",
            "Do not fabricate:",
            "Expected runtime patch count `168`",
            "169 migrated RitsuLib patch",
            "discovered mod manifest id set",
            "refreshes `run-manifest.json`",
            "The log checker has a no-launch self-test",
            "refuses direct log-only evidence",
            "current complete audit signature name set",
            "beta.135 package zip hash record",
            "hash-bound installed-file records",
            "Directory.Build.props",
            "scaffold and checker both read",
            "fail closed",
            "Git HEAD",
            "worktree status",
            "checker-validated package, install, and Git trust anchors",
            "copied owner log or screenshot source path",
            "marker-only-origin-not-proven-by-offline-checker",
            "recomputes the current package zip",
            "fabricated manifest records",
            "It still must not claim release, gameplay, save-load, or co-op readiness.");
        AssertSourceContains(
            docs,
            "TrustAnchorMode = noncanonical-override-test-only",
            "must not be described as canonical beta.135 package/install proof",
            "synthetic override roots are visible as test-only",
            "TrustAnchorMode = canonical-configured",
            "fixture/checker evidence");
        AssertSourceContains(
            releaseVerifier,
            "noncanonical-override-test-only",
            "'TrustAnchorMode'",
            "'GameRootAnchorMode'",
            "'PackageAnchorMode'");
        Assert.DoesNotContain("BaseLib", scaffold, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Enable only STS2-RitsuLib and BaseLib", docs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("BaseLib loaded successfully", docs, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Beta135RuntimeBaselineLogCheckerGuardsVersionAuditAndPatchCount()
    {
        var checker = ReadRepoText("scripts", "check-beta135-runtime-baseline-log.ps1");

        AssertSourceContains(
            checker,
            "[string]$ExpectedPackageVersion = 'v0.1.0-private-beta.135'",
            "[string]$ExpectedGameVersion = '0.107.1'",
            "[string]$ExpectedRitsuLibVersion = '0.4.34'",
            "[int]$ExpectedPatchCount = 168",
            "[string]$GameRoot = ''",
            "[switch]$SelfTest",
            "Get-Sts2PathFromDirectoryBuildProps",
            "audit-godot-log.ps1",
            "[AllowEmptyCollection()][System.Collections.Generic.List[object]]$Checks",
            "godot_log_audit_clean_bool",
            "godot_log_audit_signature_hits_array",
            "godot_log_audit_numeric_fields_native",
            "godot_log_audit_schema_fields_current",
            "godot_log_audit_file_binding_current",
            "function Get-JsonProperty",
            "MemberType -eq [System.Management.Automation.PSMemberTypes]::NoteProperty",
            "function Test-NativeJsonInt32Value",
            "function Test-NativeJsonInt64Value",
            "Test-NativeJsonInt32Value -Value $item.AuditSchemaVersion",
            "Test-NativeJsonInt64Value -Value $item.Length",
            "Test-NativeJsonInt32Value -Value $hit.Count",
            "godot_log_audit_has_single_schema_version",
            "godot_log_audit_has_single_signature_set_sha256",
            "godot_log_audit_signature_vector_present",
            "godot_log_audit_signature_names_current",
            "godot_log_audit_clean",
            "scaffold_manifest_present",
            "scaffold_manifest_owner_run_required",
            "scaffold_manifest_does_not_launch_game",
            "scaffold_manifest_package_targets_match",
            "scaffold_manifest_git_head_bound",
            "scaffold_manifest_install_paths_match_configured_game_root",
            "scaffold_manifest_package_zip_hash_bound",
            "scaffold_manifest_spire_plus_files_hash_bound",
            "scaffold_manifest_ritsu_lib_files_hash_bound",
            "ritsu_only_mod_manifest_set_exact",
            "no_base_lib_marker",
            "ExpectedModManifestIds = @($expectedModManifestIds)",
            "ObservedModManifestIds = @($modManifestIds)",
            "Set-ObjectProperty -Object $manifest -Name 'RuntimeBaselineLogCheckRecord'",
            "Evidence path must not pass through a reparse point",
            "Evidence path must stay under repo root while checking reparse points",
            "Assert-LeafIsNotReparsePoint -Path $resolvedLogPath -Description 'LogPath'",
            "Assert-NoReparsePointInPath -Path $auditOutputPath",
            "Assert-NoReparsePointInPath -Path $reportPath",
            "Assert-NoReparsePointInPath -Path $manifestPath",
            "Assert-LeafIsNotReparsePoint -Path $reportPath -Description 'Runtime baseline log check report'",
            "function Initialize-WindowsFileIdentityType",
            "GetLinkCount",
            "Assert-OutputFileDoesNotAliasProtectedEvidence",
            "must not be a hardlink with multiple filesystem names",
            "scaffold_manifest_trust_anchor_mode_bound",
            "GameRootAnchorMode = $gameRootAnchorMode",
            "PackageAnchorMode = $packageAnchorMode",
            "TrustAnchorMode = $trustAnchorMode",
            "noncanonical-override-test-only",
            "EvidenceDir must stay under",
            "AuditSchemaVersion",
            "SignatureSetSha256",
            "SignatureHitNames",
            "$expectedAuditSignatureNames = @(",
            "MalformedFileBindingValues",
            "Test-FileRecordMatchesCurrentFile",
            "Test-ManifestDirectoryFileRecordMatchesCurrentFile",
            "Test-DirectoryPathBound",
            "Test-GitEvidenceMatches",
            "Set-ObjectProperty -Object $manifest -Name 'LastCheckedGit'",
            "CurrentGit = $currentGitEvidence",
            "ConfiguredGameRoot",
            "expected explicit release/host version marker",
            "[regex]::IsMatch($raw, '\"SignatureHits\"\\s*:\\s*\\{')",
            "$signatureHits -is [System.Array]",
            "Read-AuditSummary -Path $auditOutputPath -ExpectedLogPath $resolvedLogPath",
            "Patch application complete:",
            "ModPatcher applied",
            "MissingMethodException",
            "TypeLoadException",
            "TargetInvocationException",
            "runtime-baseline-log-check.json");
        Assert.DoesNotContain("baselib_loaded_marker", checker, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("BaseLib loaded successfully", checker, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Beta135RuntimeBaselineCheckerRejectsExtraModAndBaseLibMarkers()
    {
        var script = AssertRepoFileExists("scripts", "check-beta135-runtime-baseline-log.ps1");
        var fixture = CreateBeta135RuntimeBaselineScaffold();

        try
        {
            var logPath = WriteBeta135RuntimeBaselineLog(fixture.EvidenceDir, includeBaseLib: true);
            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                fixture.EvidenceDir,
                "-LogPath",
                logPath,
                "-GameRoot",
                fixture.GameRoot,
                "-PackageZipPath",
                fixture.PackageZipPath,
                "-FailOnMismatch");

            Assert.NotEqual(0, result.ExitCode);
            Assert.Contains("ritsu_only_mod_manifest_set_exact status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("no_base_lib_marker status=fail", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            fixture.Delete();
        }
    }

    [Fact]
    public void Beta135RuntimeBaselineCheckerWritesManifestForCleanLog()
    {
        var script = AssertRepoFileExists("scripts", "check-beta135-runtime-baseline-log.ps1");
        var fixture = CreateBeta135RuntimeBaselineScaffold();

        try
        {
            var logPath = WriteBeta135RuntimeBaselineLog(fixture.EvidenceDir, includeBaseLib: false);
            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                fixture.EvidenceDir,
                "-LogPath",
                logPath,
                "-GameRoot",
                fixture.GameRoot,
                "-PackageZipPath",
                fixture.PackageZipPath,
                "-FailOnMismatch");

            Assert.True(result.ExitCode == 0, $"Checker failed:{Environment.NewLine}{result.Output}{result.Error}");
            using var manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(fixture.EvidenceDir, "run-manifest.json")));
            var root = manifest.RootElement;

            Assert.Equal("runtime-baseline-log-markers-checked", root.GetProperty("Status").GetString());
            Assert.Equal("pass", root.GetProperty("BaselineLogCheckStatus").GetString());
            Assert.Equal("checked-clean", root.GetProperty("RuntimeAuditStatus").GetString());
            Assert.Equal("marker-only-origin-not-proven-by-offline-checker", root.GetProperty("LogOriginProofStatus").GetString());
            Assert.True(root.GetProperty("GodotLogAfterLaunchRecord").GetProperty("Exists").GetBoolean());
            Assert.True(root.GetProperty("RuntimeBaselineLogCheckRecord").GetProperty("Exists").GetBoolean());
            Assert.True(root.GetProperty("PackageZip").GetProperty("Exists").GetBoolean());
            Assert.NotEmpty(root.GetProperty("SpirePlusFiles").EnumerateArray());
            Assert.NotEmpty(root.GetProperty("RitsuLibFiles").EnumerateArray());
            Assert.True(root.GetProperty("Git").GetProperty("Available").GetBoolean());
            Assert.False(string.IsNullOrWhiteSpace(root.GetProperty("Git").GetProperty("HeadSha").GetString()));
            Assert.True(root.GetProperty("LastCheckedGit").GetProperty("Available").GetBoolean());
            var expectedIds = root.GetProperty("ExpectedModManifestIds").EnumerateArray().Select(item => item.GetString()).ToArray();
            Assert.Equal(new[] { "EZMicroBalance", "STS2-RitsuLib" }, expectedIds);
        }
        finally
        {
            fixture.Delete();
        }
    }

    [Fact]
    public void Beta135RuntimeBaselineCheckerRejectsStaleGitManifestBinding()
    {
        var script = AssertRepoFileExists("scripts", "check-beta135-runtime-baseline-log.ps1");
        var fixture = CreateBeta135RuntimeBaselineScaffold();

        try
        {
            WriteStaleBeta135RuntimeBaselineGitEvidence(fixture.EvidenceDir);
            var logPath = WriteBeta135RuntimeBaselineLog(fixture.EvidenceDir, includeBaseLib: false);
            var firstResult = RunPowerShell(
                script,
                "-EvidenceDir",
                fixture.EvidenceDir,
                "-LogPath",
                logPath,
                "-GameRoot",
                fixture.GameRoot,
                "-PackageZipPath",
                fixture.PackageZipPath,
                "-FailOnMismatch");

            Assert.NotEqual(0, firstResult.ExitCode);
            Assert.Contains("scaffold_manifest_git_head_bound status=fail", firstResult.Output, StringComparison.Ordinal);
            using (var manifestAfterFailure = JsonDocument.Parse(File.ReadAllText(Path.Combine(fixture.EvidenceDir, "run-manifest.json"))))
            {
                Assert.Equal("0000000000000000000000000000000000000000", manifestAfterFailure.RootElement.GetProperty("Git").GetProperty("HeadSha").GetString());
            }

            var secondResult = RunPowerShell(
                script,
                "-EvidenceDir",
                fixture.EvidenceDir,
                "-LogPath",
                logPath,
                "-GameRoot",
                fixture.GameRoot,
                "-PackageZipPath",
                fixture.PackageZipPath,
                "-FailOnMismatch");

            Assert.NotEqual(0, secondResult.ExitCode);
            Assert.Contains("scaffold_manifest_git_head_bound status=fail", secondResult.Output, StringComparison.Ordinal);
        }
        finally
        {
            fixture.Delete();
        }
    }

    [Fact]
    public void Beta135RuntimeBaselineScaffoldAcceptsAlreadyCapturedLogAfterWritingManifest()
    {
        var script = AssertRepoFileExists("scripts", "new-beta135-runtime-baseline-evidence.ps1");
        var sourceDir = CreateBeta135RuntimeBaselineEvidenceDir();
        var fixture = CreateBeta135RuntimeBaselineFixture();

        try
        {
            var logPath = WriteBeta135RuntimeBaselineLog(sourceDir, includeBaseLib: false);
            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                fixture.EvidenceDir,
                "-GodotLogAfterLaunchPath",
                logPath,
                "-GameRoot",
                fixture.GameRoot,
                "-PackageZipPath",
                fixture.PackageZipPath);

            Assert.True(result.ExitCode == 0, $"Scaffold with captured log failed:{Environment.NewLine}{result.Output}{result.Error}");
            using var manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(fixture.EvidenceDir, "run-manifest.json")));
            Assert.Equal("runtime-baseline-log-markers-checked", manifest.RootElement.GetProperty("Status").GetString());
            Assert.Equal("pass", manifest.RootElement.GetProperty("BaselineLogCheckStatus").GetString());
        }
        finally
        {
            DeleteDirectoryIfExists(sourceDir);
            fixture.Delete();
        }
    }

    [Fact]
    public void Beta135RuntimeBaselineCheckerRejectsLogOnlyEvidenceWithoutScaffoldManifest()
    {
        var script = AssertRepoFileExists("scripts", "check-beta135-runtime-baseline-log.ps1");
        var fixture = CreateBeta135RuntimeBaselineFixture(createEvidenceDir: true);

        try
        {
            var logPath = WriteBeta135RuntimeBaselineLog(fixture.EvidenceDir, includeBaseLib: false);
            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                fixture.EvidenceDir,
                "-LogPath",
                logPath,
                "-GameRoot",
                fixture.GameRoot,
                "-PackageZipPath",
                fixture.PackageZipPath,
                "-FailOnMismatch");

            Assert.NotEqual(0, result.ExitCode);
            Assert.Contains("scaffold_manifest_present status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("scaffold_manifest_package_zip_hash_bound status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("scaffold_manifest_spire_plus_files_hash_bound status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("scaffold_manifest_ritsu_lib_files_hash_bound status=fail", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            fixture.Delete();
        }
    }

    [Fact]
    public void Beta135RuntimeBaselineCheckerRejectsFabricatedManifestFileRecords()
    {
        var script = AssertRepoFileExists("scripts", "check-beta135-runtime-baseline-log.ps1");
        var fixture = CreateBeta135RuntimeBaselineFixture(createEvidenceDir: true);

        try
        {
            var logPath = WriteBeta135RuntimeBaselineLog(fixture.EvidenceDir, includeBaseLib: false);
            WriteFabricatedBeta135RuntimeBaselineManifest(fixture.EvidenceDir);
            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                fixture.EvidenceDir,
                "-LogPath",
                logPath,
                "-GameRoot",
                fixture.GameRoot,
                "-PackageZipPath",
                fixture.PackageZipPath,
                "-FailOnMismatch");

            Assert.NotEqual(0, result.ExitCode);
            Assert.Contains("scaffold_manifest_install_paths_match_configured_game_root status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("scaffold_manifest_package_zip_hash_bound status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("scaffold_manifest_spire_plus_files_hash_bound status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("scaffold_manifest_ritsu_lib_files_hash_bound status=fail", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            fixture.Delete();
        }
    }

    [Fact]
    public void Beta135RuntimeBaselineCheckerRejectsLogWithoutGameVersionMarker()
    {
        var script = AssertRepoFileExists("scripts", "check-beta135-runtime-baseline-log.ps1");
        var fixture = CreateBeta135RuntimeBaselineScaffold();

        try
        {
            var logPath = WriteBeta135RuntimeBaselineLog(fixture.EvidenceDir, includeBaseLib: false, includeGameVersionMarker: false);
            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                fixture.EvidenceDir,
                "-LogPath",
                logPath,
                "-GameRoot",
                fixture.GameRoot,
                "-PackageZipPath",
                fixture.PackageZipPath,
                "-FailOnMismatch");

            Assert.NotEqual(0, result.ExitCode);
            Assert.Contains("game_version_marker status=fail", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            fixture.Delete();
        }
    }

    [Fact]
    public void Beta135RuntimeBaselineScaffoldRejectsNonEmptyForceDirectory()
    {
        var script = AssertRepoFileExists("scripts", "new-beta135-runtime-baseline-evidence.ps1");
        var evidenceDir = CreateBeta135RuntimeBaselineEvidenceDir();

        try
        {
            File.WriteAllText(Path.Combine(evidenceDir, "stale.txt"), "old evidence");
            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                evidenceDir,
                "-Force");

            Assert.NotEqual(0, result.ExitCode);
            Assert.Contains("Refusing to reuse non-empty evidence directory", result.Output + result.Error, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(evidenceDir);
        }
    }

    [Fact]
    public void Beta135RuntimeBaselineCheckerRejectsOutputLeafReparsePointWhenAvailable()
    {
        var script = AssertRepoFileExists("scripts", "check-beta135-runtime-baseline-log.ps1");
        var fixture = CreateBeta135RuntimeBaselineScaffold();
        var externalTarget = Path.Combine(Path.GetTempPath(), "beta135-runtime-baseline-report-target-" + Guid.NewGuid().ToString("N") + ".json");

        try
        {
            var logPath = WriteBeta135RuntimeBaselineLog(fixture.EvidenceDir, includeBaseLib: false);
            File.WriteAllText(externalTarget, "{}");
            var reportPath = Path.Combine(fixture.EvidenceDir, "runtime-baseline-log-check.json");
            if (!TryCreateFileSymlink(reportPath, externalTarget))
            {
                return;
            }

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                fixture.EvidenceDir,
                "-LogPath",
                logPath,
                "-GameRoot",
                fixture.GameRoot,
                "-PackageZipPath",
                fixture.PackageZipPath,
                "-FailOnMismatch");

            Assert.NotEqual(0, result.ExitCode);
            Assert.Contains("reparse point", result.Output + result.Error, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            fixture.Delete();
            if (File.Exists(externalTarget))
            {
                File.Delete(externalTarget);
            }
        }
    }

    [Fact]
    public void Beta135RuntimeBaselineScaffoldRejectsCopiedArtifactSourceReparsePathWhenAvailable()
    {
        var script = AssertRepoFileExists("scripts", "new-beta135-runtime-baseline-evidence.ps1");
        var fixture = CreateBeta135RuntimeBaselineFixture();

        try
        {
            var realArtifactDir = Path.Combine(fixture.Root, "owner-artifacts-real");
            var linkedArtifactDir = Path.Combine(fixture.Root, "owner-artifacts-link");
            Directory.CreateDirectory(realArtifactDir);
            _ = WriteBeta135RuntimeBaselineLog(realArtifactDir, includeBaseLib: false);
            if (!TryCreateDirectorySymlink(linkedArtifactDir, realArtifactDir))
            {
                return;
            }

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                fixture.EvidenceDir,
                "-GodotLogAfterLaunchPath",
                Path.Combine(linkedArtifactDir, "godot.log.after-launch"),
                "-GameRoot",
                fixture.GameRoot,
                "-PackageZipPath",
                fixture.PackageZipPath);

            Assert.NotEqual(0, result.ExitCode);
            Assert.Contains("Artifact source path must not pass through a reparse point", result.Output + result.Error, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            fixture.Delete();
        }
    }

    private static string CreateBeta135RuntimeBaselineEvidenceDir()
    {
        var evidenceDir = Path.Combine(Root, ".tools", "runtime-evidence", "beta135-baseline-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(evidenceDir);
        return evidenceDir;
    }

    private static Beta135RuntimeBaselineFixture CreateBeta135RuntimeBaselineScaffold()
    {
        var script = AssertRepoFileExists("scripts", "new-beta135-runtime-baseline-evidence.ps1");
        var fixture = CreateBeta135RuntimeBaselineFixture();
        var result = RunPowerShell(
            script,
            "-EvidenceDir",
            fixture.EvidenceDir,
            "-GameRoot",
            fixture.GameRoot,
            "-PackageZipPath",
            fixture.PackageZipPath);
        Assert.True(result.ExitCode == 0, $"Scaffold failed:{Environment.NewLine}{result.Output}{result.Error}");
        return fixture;
    }

    private static Beta135RuntimeBaselineFixture CreateBeta135RuntimeBaselineFixture(bool createEvidenceDir = false)
    {
        var fixtureRoot = Path.Combine(Root, ".tools", "runtime-evidence", "beta135-baseline-fixture-" + Guid.NewGuid().ToString("N"));
        var evidenceDir = Path.Combine(fixtureRoot, "evidence");
        var gameRoot = Path.Combine(fixtureRoot, "game", "Slay the Spire 2");
        var spirePlusDir = Path.Combine(gameRoot, "mods", "EZMicroBalance");
        var ritsuDir = Path.Combine(gameRoot, "mods", "STS2-RitsuLib");
        var packageDir = Path.Combine(fixtureRoot, "publish");
        var packageZipPath = Path.Combine(packageDir, "SpirePlus-v0.1.0-private-beta.135.zip");

        Directory.CreateDirectory(spirePlusDir);
        Directory.CreateDirectory(ritsuDir);
        Directory.CreateDirectory(packageDir);
        if (createEvidenceDir)
        {
            Directory.CreateDirectory(evidenceDir);
        }

        File.WriteAllText(packageZipPath, "controlled package zip");
        WriteFakeFile(spirePlusDir, "EZMicroBalance.dll");
        WriteFakeFile(spirePlusDir, "EZMicroBalance.json");
        WriteFakeFile(spirePlusDir, "EZMicroBalance.pck");
        WriteFakeFile(ritsuDir, "STS2-RitsuLib.dll");
        WriteFakeFile(ritsuDir, "STS2-RitsuLib.xml");

        return new Beta135RuntimeBaselineFixture(fixtureRoot, evidenceDir, gameRoot, packageZipPath);
    }

    private static string WriteBeta135RuntimeBaselineLog(string evidenceDir, bool includeBaseLib, bool includeGameVersionMarker = true)
    {
        var extraModLines = includeBaseLib
            ? $"{Environment.NewLine}Found mod manifest file E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\BaseLib\\mod_manifest.json{Environment.NewLine}Loaded BaseLib"
            : string.Empty;
        var gameVersionMarker = includeGameVersionMarker
            ? "release 0.107.1"
            : string.Empty;
        var logPath = Path.Combine(evidenceDir, "godot.log.after-launch");
        File.WriteAllText(
            logPath,
            $"""
            {gameVersionMarker}
            RitsuLib Version: 0.4.34
            compat branch: 0.107.1
            Loaded STS2-RitsuLib
            Loaded Spire Plus EZMicroBalance package v0.1.0-private-beta.135
            Found mod manifest file E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance\mod_manifest.json
            Found mod manifest file E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib\mod_manifest.json{extraModLines}
            Patch application complete: 168 applied, 0 ignored, 0 failed, 168 total
            ModPatcher applied 168 patches (168 registered)
            Time to main menu
            """);
        return logPath;
    }

    private static void WriteStaleBeta135RuntimeBaselineGitEvidence(string evidenceDir)
    {
        var manifestPath = Path.Combine(evidenceDir, "run-manifest.json");
        using var document = JsonDocument.Parse(File.ReadAllText(manifestPath));
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

        writer.WriteStartObject();
        foreach (var property in document.RootElement.EnumerateObject())
        {
            if (property.NameEquals("Git"))
            {
                writer.WritePropertyName("Git");
                writer.WriteStartObject();
                writer.WriteBoolean("Available", true);
                writer.WriteString("RepoRoot", Root);
                writer.WriteString("HeadSha", "0000000000000000000000000000000000000000");
                writer.WriteString("HeadDecorated", "0000000 stale beta.135 baseline source binding");
                writer.WriteString("Branch", "stale-baseline");
                writer.WriteString("Upstream", string.Empty);
                writer.WritePropertyName("StatusShortBranch");
                writer.WriteStartArray();
                writer.WriteStringValue("## stale-baseline");
                writer.WriteEndArray();
                writer.WriteBoolean("Dirty", false);
                writer.WriteEndObject();
                continue;
            }

            property.WriteTo(writer);
        }

        writer.WriteEndObject();
        writer.Flush();
        File.WriteAllBytes(manifestPath, stream.ToArray());
    }

    private static void WriteFabricatedBeta135RuntimeBaselineManifest(string evidenceDir)
    {
        var fakeRoot = Path.Combine(evidenceDir, "fake-root");
        var fakePackageDir = Path.Combine(fakeRoot, "publish");
        var fakeSpirePlusDir = Path.Combine(fakeRoot, "mods", "EZMicroBalance");
        var fakeRitsuDir = Path.Combine(fakeRoot, "mods", "STS2-RitsuLib");
        Directory.CreateDirectory(fakePackageDir);
        Directory.CreateDirectory(fakeSpirePlusDir);
        Directory.CreateDirectory(fakeRitsuDir);
        var fakePackagePath = Path.Combine(fakePackageDir, "SpirePlus-v0.1.0-private-beta.135.zip");
        File.WriteAllText(fakePackagePath, "fake package zip");
        WriteFakeFile(fakeSpirePlusDir, "EZMicroBalance.dll");
        WriteFakeFile(fakeSpirePlusDir, "EZMicroBalance.json");
        WriteFakeFile(fakeSpirePlusDir, "EZMicroBalance.pck");
        WriteFakeFile(fakeRitsuDir, "STS2-RitsuLib.dll");
        WriteFakeFile(fakeRitsuDir, "STS2-RitsuLib.xml");

        var manifest = new
        {
            SchemaVersion = 1,
            Status = "pending-owner-run",
            EvidenceDir = evidenceDir,
            OwnerRunRequired = true,
            DoesNotLaunchGame = true,
            LaunchMethod = "owner-steam-launch-required",
            PackageVersion = "v0.1.0-private-beta.135",
            GameVersion = "0.107.1",
            RitsuLibVersion = "0.4.34",
            RitsuCompatBranch = "0.107.1",
            ExpectedRuntimePatchCount = 168,
            GameRoot = fakeRoot,
            ModsRoot = Path.Combine(fakeRoot, "mods"),
            SpirePlusInstallDir = fakeSpirePlusDir,
            RitsuLibInstallDir = fakeRitsuDir,
            PackageZip = FileRecord(fakePackagePath),
            SpirePlusFiles = new[]
            {
                FileRecord(Path.Combine(fakeSpirePlusDir, "EZMicroBalance.dll"), "EZMicroBalance.dll"),
                FileRecord(Path.Combine(fakeSpirePlusDir, "EZMicroBalance.json"), "EZMicroBalance.json"),
                FileRecord(Path.Combine(fakeSpirePlusDir, "EZMicroBalance.pck"), "EZMicroBalance.pck"),
            },
            RitsuLibFiles = new[]
            {
                FileRecord(Path.Combine(fakeRitsuDir, "STS2-RitsuLib.dll"), "STS2-RitsuLib.dll"),
                FileRecord(Path.Combine(fakeRitsuDir, "STS2-RitsuLib.xml"), "STS2-RitsuLib.xml"),
            },
        };

        File.WriteAllText(
            Path.Combine(evidenceDir, "run-manifest.json"),
            JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));
    }

    private static void WriteFakeFile(string directory, string fileName)
    {
        File.WriteAllText(Path.Combine(directory, fileName), $"fake {fileName}");
    }

    private static object FileRecord(string path, string? relativePath = null)
    {
        var file = new FileInfo(path);
        var sha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
        if (relativePath is null)
        {
            return new { Path = file.FullName, Exists = true, Length = file.Length, Sha256 = sha256 };
        }

        return new { Path = file.FullName, RelativePath = relativePath, Length = file.Length, Sha256 = sha256 };
    }

    private sealed class Beta135RuntimeBaselineFixture
    {
        public Beta135RuntimeBaselineFixture(string root, string evidenceDir, string gameRoot, string packageZipPath)
        {
            Root = root;
            EvidenceDir = evidenceDir;
            GameRoot = gameRoot;
            PackageZipPath = packageZipPath;
        }

        public string Root { get; }

        public string EvidenceDir { get; }

        public string GameRoot { get; }

        public string PackageZipPath { get; }

        public void Delete()
        {
            DeleteDirectoryIfExists(Root);
        }
    }

    private static bool TryCreateDirectorySymlink(string linkPath, string targetPath)
    {
        try
        {
            Directory.CreateSymbolicLink(linkPath, targetPath);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (PlatformNotSupportedException)
        {
            return false;
        }
    }
}
