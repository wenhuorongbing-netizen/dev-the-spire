using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseArtifactParityGuardTests
{
    private static readonly string[] KnownCurrentHashDocs =
    [
        "docs/dev-environment.md",
        "docs/test-ready-completion-audit.md",
        "docs/release-checklist.md",
        "docs/features/ancients-rework-v4/completion-audit.md"
    ];

    private static readonly string[] CurrentReleaseHashClaimLineMarkers =
    [
        "zip",
        "package",
        "dll",
        "manifest",
        "json",
        "pck",
        "installed",
        "staging",
        "versioned",
        "current"
    ];

    [ReleaseArtifactFact]
    public void ActiveCoverArtAndInactiveModRealPolicyMatchExportPckAndPackage()
    {
        var activeCover = AssertRepoFileExists("EZMicroBalance", "mod_image.png");
        var auditedCover = AssertRepoFileExists("publish", "EZMicroBalance-cover-source.png");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var installedPck = GamePath("mods", "EZMicroBalance", "EZMicroBalance.pck");
        var packageZip = CurrentPackageZipPath();

        Assert.Equal(Sha256(activeCover), Sha256(auditedCover));

        AssertRepoPathDoesNotExist("EZMicroBalance", "mod_real.png");
        AssertRepoPathDoesNotExist("EZMicroBalance", "mod_real.png.import");

        var exported = ParseExportFiles(exportPreset);
        Assert.Contains("res://EZMicroBalance/mod_image.png", exported);
        Assert.DoesNotContain(exported, path => path.Contains("mod_real", StringComparison.OrdinalIgnoreCase));

        var pckEntries = ReadPckDirectory(installedPck);
        Assert.Contains("EZMicroBalance/mod_image.png", pckEntries);
        Assert.Contains("EZMicroBalance/mod_image.png.import", pckEntries);
        Assert.DoesNotContain(pckEntries, entry => entry.Contains("mod_real", StringComparison.OrdinalIgnoreCase));

        using var archive = ZipFile.OpenRead(packageZip);
        var zipEntries = archive.Entries.Select(entry => entry.FullName.Replace('\\', '/')).ToArray();
        Assert.DoesNotContain(zipEntries, entry => entry.Contains("mod_real", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(zipEntries, entry => entry.EndsWith("mod_image.png", StringComparison.OrdinalIgnoreCase));

        var zippedPckEntries = ReadPckDirectory(ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.pck"));
        Assert.Contains("EZMicroBalance/mod_image.png", zippedPckEntries);
        Assert.Contains("EZMicroBalance/mod_image.png.import", zippedPckEntries);
        Assert.DoesNotContain(zippedPckEntries, entry => entry.Contains("mod_real", StringComparison.OrdinalIgnoreCase));
    }

    [ReleaseArtifactFact]
    public void ExportedResourcesInstalledPckAndPackagePckStayInParity()
    {
        var exportedResources = ParseExportFiles(ReadRepoText("export_presets.cfg"))
            .Select(path => path["res://".Length..])
            .Concat(
                Directory.GetFiles(RepoPath("EZMicroBalance"), "*", SearchOption.AllDirectories)
                    .Select(path => ToRepoRelativePath(path))
                    .Where(IsActiveExportResource))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        var installedPck = GamePath("mods", "EZMicroBalance", "EZMicroBalance.pck");
        var installedEntries = ReadPckDirectory(installedPck);
        using var archive = ZipFile.OpenRead(CurrentPackageZipPath());
        var zippedEntries = ReadPckDirectory(ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.pck"));

        Assert.Equal(installedEntries.OrderBy(entry => entry, StringComparer.Ordinal), zippedEntries.OrderBy(entry => entry, StringComparer.Ordinal));
        AssertImportedTexturePresent(
            installedEntries,
            zippedEntries,
            "EZMicroBalance/images/relics/sere_talon_spire_plus.png");
        AssertImportedTexturePresent(
            installedEntries,
            zippedEntries,
            "EZMicroBalance/images/relics/big/sere_talon_spire_plus.png");

        foreach (var resource in exportedResources.Where(path => path.EndsWith(".json", StringComparison.Ordinal)))
        {
            Assert.Contains(resource, installedEntries);
            Assert.Contains(resource, zippedEntries);
        }

        foreach (var resource in exportedResources.Where(path => path.EndsWith(".tscn", StringComparison.Ordinal)))
        {
            Assert.True(
                installedEntries.Contains(resource) || installedEntries.Contains($"{resource}.remap"),
                $"Installed PCK is missing exported scene or remap: {resource}");
            Assert.True(
                zippedEntries.Contains(resource) || zippedEntries.Contains($"{resource}.remap"),
                $"Package PCK is missing exported scene or remap: {resource}");
        }

        foreach (var resource in exportedResources.Where(path => path.EndsWith(".png", StringComparison.Ordinal)))
        {
            AssertRepoFileExists(resource.Split('/'));
            AssertRepoFileExists((resource + ".import").Split('/'));
            Assert.Contains(resource + ".import", installedEntries);
            Assert.Contains(resource + ".import", zippedEntries);
        }

        var activeLocalizationJson = Directory.GetFiles(RepoPath("EZMicroBalance", "localization"), "*.json", SearchOption.AllDirectories)
            .Select(path => ToRepoRelativePath(path))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        var exportedLocalization = exportedResources
            .Where(path => path.StartsWith("EZMicroBalance/localization/", StringComparison.Ordinal))
            .ToArray();

        Assert.Equal(activeLocalizationJson.Length, exportedLocalization.Length);
        Assert.All(activeLocalizationJson, resource => Assert.Contains(resource, exportedLocalization));
    }

    private static void AssertImportedTexturePresent(
        IReadOnlyCollection<string> installedEntries,
        IReadOnlyCollection<string> zippedEntries,
        string sourceResource)
    {
        var importResource = sourceResource + ".import";
        AssertRepoFileExists(sourceResource.Split('/'));
        AssertRepoFileExists(importResource.Split('/'));
        Assert.Contains(importResource, installedEntries);
        Assert.Contains(importResource, zippedEntries);

        var importText = ReadRepoText(importResource.Split('/'));
        var importedTextureEntries = Regex
            .Matches(importText, "\"res://(?<path>[^\"]+\\.ctex)\"")
            .Select(match => match.Groups["path"].Value)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        Assert.NotEmpty(importedTextureEntries);
        foreach (var importedTextureEntry in importedTextureEntries)
        {
            Assert.Contains(importedTextureEntry, installedEntries);
            Assert.Contains(importedTextureEntry, zippedEntries);
        }
    }

    [ReleaseArtifactFact]
    public void CurrentReleaseHashClaimsMatchInstalledStagingVersionedAndZipArtifacts()
    {
        var packageName = CurrentPackageName();
        var installedDir = GamePath("mods", "EZMicroBalance");
        var stagingDir = RepoPath("publish", "package-staging", "EZMicroBalance");
        var versionedDir = RepoPath("publish", packageName, "EZMicroBalance");
        var zipPath = CurrentPackageZipPath();

        var dllHash = Sha256(Path.Combine(installedDir, "EZMicroBalance.dll"));
        var manifestHash = Sha256(Path.Combine(installedDir, "EZMicroBalance.json"));
        var pckHash = Sha256(Path.Combine(installedDir, "EZMicroBalance.pck"));
        var readmeHash = Sha256(Path.Combine(stagingDir, "README_INSTALL.txt"));
        var zipHash = CurrentPackageZipSha256();
        var artHash = Sha256(RepoPath("EZMicroBalance", "mod_image.png"));
        var knownCurrentHashes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            dllHash,
            manifestHash,
            pckHash,
            readmeHash,
            zipHash,
            artHash
        };

        Assert.Equal(dllHash, Sha256(Path.Combine(stagingDir, "EZMicroBalance.dll")));
        Assert.Equal(dllHash, Sha256(Path.Combine(versionedDir, "EZMicroBalance.dll")));
        Assert.Equal(manifestHash, Sha256(Path.Combine(stagingDir, "EZMicroBalance.json")));
        Assert.Equal(manifestHash, Sha256(Path.Combine(versionedDir, "EZMicroBalance.json")));
        Assert.Equal(pckHash, Sha256(Path.Combine(stagingDir, "EZMicroBalance.pck")));
        Assert.Equal(pckHash, Sha256(Path.Combine(versionedDir, "EZMicroBalance.pck")));
        Assert.Equal(readmeHash, Sha256(Path.Combine(versionedDir, "README_INSTALL.txt")));

        using (var archive = ZipFile.OpenRead(zipPath))
        {
            Assert.Equal(dllHash, Sha256(ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.dll")));
            Assert.Equal(manifestHash, Sha256(ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.json")));
            Assert.Equal(pckHash, Sha256(ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.pck")));
            Assert.Equal(readmeHash, Sha256(ReadZipBytes(archive, "EZMicroBalance/README_INSTALL.txt")));
        }

        var hashDocs = string.Join(Environment.NewLine, KnownCurrentHashDocs.Select(path => ReadRepoText(path.Split('/'))));
        Assert.Contains(dllHash, hashDocs, StringComparison.Ordinal);
        Assert.Contains(manifestHash, hashDocs, StringComparison.Ordinal);
        Assert.Contains(pckHash, hashDocs, StringComparison.Ordinal);
        Assert.Contains(zipHash, hashDocs, StringComparison.Ordinal);
        Assert.Contains(artHash, hashDocs, StringComparison.Ordinal);

        var documentedHashes = hashDocs
            .Split(["\r", "\n"], StringSplitOptions.RemoveEmptyEntries)
            .Where(line =>
                CurrentReleaseHashClaimLineMarkers.Any(marker => line.Contains(marker, StringComparison.OrdinalIgnoreCase)))
            .SelectMany(line => Regex.Matches(line, @"\b[A-Fa-f0-9]{64}\b").Cast<Match>().Select(match => match.Value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Assert.All(documentedHashes, hash => Assert.Contains(hash, knownCurrentHashes));
    }

    [ReleaseArtifactFact]
    public void CurrentRuntimeLogVersionMustMatchManifest()
    {
        var logPath = CurrentGodotLogPath();
        if (!File.Exists(logPath))
        {
            return;
        }

        var summary = SmokeLogParser.Parse(ReadSharedText(logPath));
        if (summary.EzMicroBalanceVersion is null)
        {
            return;
        }

        if (summary.EzMicroBalanceVersion == ManifestVersion())
        {
            return;
        }

        var currentDocs = ReadCurrentFacingDocs(CurrentFacingDocs);
        Assert.Contains("RitsuLib-only Off proof has been recaptured", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Previous beta.93 AdditiveBatch1 registration proof has been recaptured", currentDocs, StringComparison.Ordinal);
        Assert.Contains("loader/registration evidence, not gameplay proof", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("current beta.90 loader smoke passed", currentDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("current package smoke passed", currentDocs, StringComparison.OrdinalIgnoreCase);
    }

    [ReleaseArtifactFact]
    public void RecentRuntimeLogMustNotContainV105ApiDriftOrExternalModDependencyFailures()
    {
        var logPath = CurrentGodotLogPath();
        var logsDir = Path.GetDirectoryName(logPath);
        Assert.NotNull(logsDir);
        if (!Directory.Exists(logsDir))
        {
            return;
        }

        var recentLog = Directory
            .GetFiles(logsDir, "godot*.log", SearchOption.TopDirectoryOnly)
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .FirstOrDefault();

        if (recentLog == null)
        {
            return;
        }

        var logContent = ReadSharedText(recentLog);

        var forbiddenSignatures = new[]
        {
            "Creature.get_ShowsInfiniteHp",
            "ExternalMod.Patches.UI.HealthBarForecastPatch.RefreshForegroundOverlay",
            "DamageMeter.Scripts.CombatDataCollector.SnapshotEnemyHp",
            "Undefined target method for patch method static System.Void ExternalMod.Patches.Features",
        };

        var matches = new List<string>();
        foreach (var signature in forbiddenSignatures)
        {
            if (logContent.Contains(signature, StringComparison.Ordinal))
            {
                matches.Add(signature);
            }
        }

        Assert.True(
            matches.Count == 0,
            $"Recent runtime log {Path.GetFileName(recentLog)} contains forbidden v0.105.0 API drift or dependency failure signatures: {string.Join("; ", matches)}. " +
            "The test environment may have incompatible mods (DamageMeter, non-EZMB mods) or an incompatible ExternalMod version. " +
            "Disable all mods except STS2-RitsuLib + Spire Plus and retest. The Spire Plus technical folder/id is EZMicroBalance. See ISSUE-2026-05-08-V105-EXTERNALMOD-CREATURE-SHOWSINFINITEHP-API-DRIFT in docs/issues.md.");
    }

    [ReleaseArtifactFact]
    public void RecentSmokeLogSupportsControlledSmokeClaims()
    {
        var logPath = CurrentGodotLogPath();
        var logsDir = Path.GetDirectoryName(logPath);
        Assert.NotNull(logsDir);
        Assert.True(Directory.Exists(logsDir), $"Missing log directory: {logsDir}");

        var candidates = Directory
            .GetFiles(logsDir, "godot*.log", SearchOption.TopDirectoryOnly)
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .ToArray();
        Assert.NotEmpty(candidates);

        var passingLogs = candidates
            .Select(path => (path, summary: SmokeLogParser.Parse(ReadSharedText(path))))
            .Where(candidate => IsControlledSmokePass(candidate.summary))
            .ToArray();

        if (passingLogs.Length == 0)
        {
            var currentDocs = ReadCurrentFacingDocs(CurrentFacingDocs);
            Assert.Contains("fresh-current-package-loader-smoke", currentDocs, StringComparison.Ordinal);
            Assert.Contains("RitsuLib-only Off proof has been recaptured", currentDocs, StringComparison.Ordinal);
            Assert.Contains("SavedAttachedState", currentDocs, StringComparison.Ordinal);
            Assert.Contains("beta.107 clicked Ancient UI smoke covers only the forced UI paths.", currentDocs, StringComparison.Ordinal);
            Assert.Contains("Gameplay, save-load, current enabled-mode proof, co-op, and independent QA evidence are still required before any live-ready or release-ready claim.", currentDocs, StringComparison.Ordinal);
            Assert.Contains("Previous beta.93 AdditiveBatch1 registration proof has been recaptured", currentDocs, StringComparison.Ordinal);
            Assert.Contains("loader/registration evidence, not gameplay proof", currentDocs, StringComparison.Ordinal);
            Assert.DoesNotContain("current package smoke passed", currentDocs, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("refreshed runtime smoke passed", currentDocs, StringComparison.OrdinalIgnoreCase);
            return;
        }

        if (File.Exists(logPath))
        {
            var currentSummary = SmokeLogParser.Parse(ReadSharedText(logPath));
            Assert.Empty(currentSummary.EzMicroBalanceErrorLines);
        }
    }

    [ReleaseArtifactFact]
    public void DisabledSpirePlusPlugOffEvidenceSupportsDocs()
    {
        var evidenceDir = RepoPath(".tools", "runtime-evidence", "live-spire-plus-disabled-session-20260513-143020");
        var currentDocs = ReadCurrentFacingDocs(CurrentFacingDocs);
        if (!Directory.Exists(evidenceDir))
        {
            Assert.Contains("raw local `.tools` runtime-evidence folders may be pruned", currentDocs, StringComparison.Ordinal);
            Assert.Contains("beta.107 clicked Ancient UI smoke covers only the forced UI paths.", currentDocs, StringComparison.Ordinal);
            Assert.Contains("Gameplay, save-load, current enabled-mode proof, co-op, and independent QA evidence are still required before any live-ready or release-ready claim.", currentDocs, StringComparison.Ordinal);
            return;
        }

        using var summary = JsonDocument.Parse(ReadRepoText(".tools", "runtime-evidence", "live-spire-plus-disabled-session-20260513-143020", "disabled-startup-summary.json"));
        var root = summary.RootElement;
        Assert.True(root.GetProperty("DisableSpirePlus").GetBoolean());
        Assert.True(root.GetProperty("MovedEzmb").GetBoolean());
        Assert.True(root.GetProperty("ReachedMainMenu").GetBoolean());
        Assert.True(root.GetProperty("ContainsExternalModInitialization").GetBoolean());
        Assert.False(root.GetProperty("ContainsSpirePlusInitialization").GetBoolean());
        Assert.False(root.GetProperty("ContainsEzmbError").GetBoolean());
        Assert.Equal(["ExternalMod"], root.GetProperty("AllowedModIds").EnumerateArray().Select(value => value.GetString() ?? string.Empty).ToArray());
        Assert.Contains(root.GetProperty("LoadedLines").EnumerateArray().Select(value => value.GetString() ?? string.Empty), line => line.Contains("Loaded 1 mods (1 total)", StringComparison.Ordinal));

        var log = ReadRepoText(".tools", "runtime-evidence", "live-spire-plus-disabled-session-20260513-143020", "godot.log");
        Assert.Contains("Loaded 1 mods (1 total)", log, StringComparison.Ordinal);
        Assert.Contains("Finished mod initialization for 'ExternalMod' (ExternalMod)", log, StringComparison.Ordinal);
        Assert.DoesNotContain("Finished mod initialization for 'Spire Plus' (EZMicroBalance)", log, StringComparison.Ordinal);
        Assert.DoesNotContain("Registered config for mod EZMicroBalance", log, StringComparison.Ordinal);
        Assert.DoesNotContain("EZMicroBalance.dll", log, StringComparison.Ordinal);
        Assert.DoesNotContain("EZMicroBalance.pck", log, StringComparison.Ordinal);

        using var audit = JsonDocument.Parse(ReadRepoText(".tools", "runtime-evidence", "live-spire-plus-disabled-session-20260513-143020", "godot-log-audit.json"));
        Assert.True(audit.RootElement.GetProperty("Clean").GetBoolean());
        Assert.All(audit.RootElement.GetProperty("SignatureHits").EnumerateArray(), hit => Assert.Equal(0, hit.GetProperty("Count").GetInt32()));

        using var restore = JsonDocument.Parse(ReadRepoText(".tools", "runtime-evidence", "live-spire-plus-disabled-session-20260513-143020", "restore-output.json"));
        Assert.Equal(25, restore.RootElement.GetProperty("RestoredModCount").GetInt32());
        Assert.Equal(1, restore.RootElement.GetProperty("RestoredCurrentRunCount").GetInt32());

        Assert.Contains("live-spire-plus-disabled-session-20260513-143020", currentDocs, StringComparison.Ordinal);
        Assert.Contains("settings-only disabled attempt", currentDocs, StringComparison.Ordinal);
        Assert.Contains("This is plug-off loader evidence only; disable-mod gameplay in an actual run remains pending.", currentDocs, StringComparison.Ordinal);
    }

    private static string CurrentGodotLogPath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SlayTheSpire2",
            "logs",
            "godot.log");
    }
}
