using System.Buffers.Binary;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class AncientArtAssetHygieneGuardTests
{
    private const string ManifestPath = "docs/features/ancient-expansion-v2.2/art-asset-manifest.json";
    private const string PromptPackPath = "docs/features/ancient-expansion-v2.2/art-generation-prompts.md";
    private const string AuditScriptPath = "scripts/audit-ancient-art-assets.ps1";
    private const string Gpt4FreeScriptPath = "scripts/invoke-ancient-art-gpt4free.ps1";
    private const string RequiredGenerationMode = "GPTimage2";

    private const string GlobalStylePhrase =
        "Slay the Spire 2 inspired dark fantasy roguelike card-game art, hand-painted 2D illustration, rough gouache and oil brush texture, painterly flat colors, strong black silhouette, uneven ink outline, muted navy-purple shadows, small saturated highlights, grotesque but charming fantasy design, readable at small size, not realistic, not anime, not 3D, not overpolished.";

    private static readonly string[] RequiredSourceStatuses =
    [
        "final_generated",
        "user_supplied",
        "source_local_background",
        "source_local_generated",
        "source_derived_temporary",
        "generic_temporary",
        "missing"
    ];

    [Fact]
    public void AncientArtManifestExistsAndIsParseable()
    {
        using var document = JsonDocument.Parse(ReadRepoText(ManifestPath.Split('/')));
        var root = document.RootElement;

        Assert.Equal(1, root.GetProperty("schema_version").GetInt32());
        Assert.Equal(RequiredGenerationMode, root.GetProperty("required_generation_mode").GetString());
        var assets = root.GetProperty("assets").EnumerateArray().ToArray();
        Assert.True(assets.Length >= 68, "Manifest should cover active Ancient backgrounds, icons, option art, power art, Rootblight portraits, and generic fallback art uses.");

        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var asset in assets)
        {
            var id = RequiredString(asset, "id");
            Assert.True(ids.Add(id), $"Duplicate manifest id: {id}");
            Assert.False(string.IsNullOrWhiteSpace(RequiredString(asset, "role")), $"Missing role for {id}");
            Assert.False(string.IsNullOrWhiteSpace(RequiredString(asset, "target_path")), $"Missing target path for {id}");
            Assert.Contains(RequiredString(asset, "source_status"), RequiredSourceStatuses);
            var finalRequiredKind = asset.GetProperty("final_required_before_release").ValueKind;
            Assert.True(
                finalRequiredKind is JsonValueKind.True or JsonValueKind.False,
                $"final_required_before_release must be a JSON boolean for {id}.");
        }

        foreach (var expectedId in new[]
        {
            "urda_event_background",
            "morvi_event_background",
            "lotha_event_background",
            "urda_map_icon",
            "morvi_run_history_outline",
            "lotha_public_evidence_option_relic",
            "vakuu_fight_option_relic",
            "lotha_verdict_power",
            "rootblight_i_card_portrait_small",
            "rootblight_iii_card_portrait_big",
            "blight_sprout_card_portrait_small",
            "generic_power_icon",
            "generic_relic_outline",
            "urda_seedling_card_portrait_small",
            "withered_husk_card_portrait_big",
            "morvi_archive_pages_card_portrait_small",
            "morvi_red_ink_overdraft_card_portrait_big",
            "morvi_waste_paper_card_portrait_small",
            "vakuu_temptation_card_portrait_big"
        })
        {
            Assert.Contains(expectedId, ids);
        }
    }

    [Fact]
    public void ManifestTracksExportedCorePortraitPowerAndRelicArt()
    {
        using var document = JsonDocument.Parse(ReadRepoText(ManifestPath.Split('/')));
        var manifestPaths = document.RootElement
            .GetProperty("assets")
            .EnumerateArray()
            .Select(asset => RequiredString(asset, "target_path"))
            .ToHashSet(StringComparer.Ordinal);

        foreach (var path in new[]
        {
            "EZMicroBalance/images/card_portraits/rootblight_i.png",
            "EZMicroBalance/images/card_portraits/rootblight_ii.png",
            "EZMicroBalance/images/card_portraits/rootblight_iii.png",
            "EZMicroBalance/images/card_portraits/blight_sprout.png",
            "EZMicroBalance/images/card_portraits/big/rootblight_i.png",
            "EZMicroBalance/images/card_portraits/big/rootblight_ii.png",
            "EZMicroBalance/images/card_portraits/big/rootblight_iii.png",
            "EZMicroBalance/images/card_portraits/big/blight_sprout.png",
            "EZMicroBalance/images/powers/power.png",
            "EZMicroBalance/images/powers/big/power.png",
            "EZMicroBalance/images/relics/relic.png",
            "EZMicroBalance/images/relics/big/relic.png",
            "EZMicroBalance/images/relics/relic_outline.png"
        })
        {
            Assert.Contains(path, manifestPaths);
        }
    }

    [Fact]
    public void ManifestRecordedDimensionsMatchCurrentPngBytesAndUiRoles()
    {
        using var document = JsonDocument.Parse(ReadRepoText(ManifestPath.Split('/')));

        foreach (var asset in document.RootElement.GetProperty("assets").EnumerateArray())
        {
            if (RequiredString(asset, "source_status") == "missing")
            {
                continue;
            }

            var targetPath = RequiredString(asset, "target_path");
            if (!targetPath.EndsWith(".png", StringComparison.Ordinal))
            {
                continue;
            }

            var (actualWidth, actualHeight) = ReadPngDimensions(RepoPath(targetPath.Split('/')));
            var recordedDimensions = asset.GetProperty("dimensions");
            Assert.Equal(actualWidth, recordedDimensions.GetProperty("width").GetInt32());
            Assert.Equal(actualHeight, recordedDimensions.GetProperty("height").GetInt32());

            switch (RequiredString(asset, "role"))
            {
                case "event_background":
                    Assert.StartsWith("EZMicroBalance/images/events/", targetPath, StringComparison.Ordinal);
                    Assert.True(actualWidth >= 1280, $"Event background is too narrow: {targetPath}");
                    Assert.True(actualHeight >= 720, $"Event background is too short: {targetPath}");
                    break;
                case "map_icon":
                case "map_icon_outline":
                case "run_history_icon":
                case "run_history_outline":
                    Assert.StartsWith("EZMicroBalance/images/ancients/", targetPath, StringComparison.Ordinal);
                    Assert.Equal(actualWidth, actualHeight);
                    Assert.True(actualWidth >= 96, $"Ancient identity icon is too small: {targetPath}");
                    Assert.DoesNotContain("/images/events/", targetPath, StringComparison.Ordinal);
                    break;
                case "option_relic":
                    Assert.StartsWith("EZMicroBalance/images/ancients/", targetPath, StringComparison.Ordinal);
                    Assert.True(actualWidth <= 512 && actualHeight <= 512, $"Option relic art should not point at a full event background: {targetPath}");
                    Assert.DoesNotContain("/images/events/", targetPath, StringComparison.Ordinal);
                    break;
            }
        }
    }

    [Fact]
    public void ManifestResourceTargetsAreCoveredByGodotExportPreset()
    {
        using var document = JsonDocument.Parse(ReadRepoText(ManifestPath.Split('/')));
        var exportPreset = ReadRepoText("export_presets.cfg");
        var exportedResources = ParseExportFiles(exportPreset).ToHashSet(StringComparer.Ordinal);

        foreach (var asset in document.RootElement.GetProperty("assets").EnumerateArray())
        {
            var sourceStatus = RequiredString(asset, "source_status");
            var targetPath = RequiredString(asset, "target_path");
            if (sourceStatus == "missing" || !RequiresExportCoverage(targetPath))
            {
                continue;
            }

            Assert.Contains($"res://{targetPath}", exportedResources);
        }
    }

    [Fact]
    public void ArtPromptPackContainsOperationalConceptsAndOutputRules()
    {
        var promptPack = ReadRepoText(PromptPackPath.Split('/'));

        AssertSourceContains(
            promptPack,
            "generation_mode: GPTimage2",
            "model: GPTimage2",
            "mode: GPTimage2",
            "semantic_model: GPTimage2",
            "If the available generator cannot select `GPTimage2`, stop and update the workflow before generating.",
            "API transport model such as `gpt-image` is acceptable only when the request still records `generation_mode`, `mode`, and `semantic_model` as `GPTimage2`",
            "Do not fall back to generic imagegen defaults",
            "scripts/invoke-ancient-art-gpt4free.ps1",
            "GPT4FREE_IMAGE_ENDPOINT",
            GlobalStylePhrase,
            "dark hand-painted roguelike card game art, Slay the Spire 2 inspired, painterly flat colors",
            "Do not use official Slay the Spire 2 assets, web images, logos, UI, release numbers, watermarks, or visible text.",
            "Source-Code Visual Calibration",
            "source code/images/events/reflections.png",
            "source code/images/events/crystal_sphere.png",
            "source code/images/packed/map/ancients/ancient_node_neow.png",
            "Primary manual ChatGPT style anchor",
            ".tools/art-generation/chatgpt/crystal-throne-of-shattered-visions.png",
            "current best user-approved style direction",
            "Do not overcorrect toward later darker and emptier iterations",
            "Small option relic sheets should inherit the first preview's dark mirror-card finish",
            "Before any small-art review candidate is promoted to an active resource, inspect it at target size",
            "If a candidate loses its silhouette or becomes mostly black at target size, regenerate only that weak asset or weak group",
            "Identity outline icons should be simple UI glyphs with hollow centers",
            "no paper texture, pseudo-writing, or label-like detail",
            "Keep review contact sheets and target-size audit sheets under `.tools/art-generation/chatgpt/`",
            "Prefer a 2.13:1 wide composition for final backgrounds",
            "upload only the intended mirror-character shape references",
            "original simplified whale-tower silhouette with a hole-punched face",
            "acrylic paint and marker texture",
            "For manual ChatGPT UI fallback, do not include target paths, filenames, or save-directory instructions in the chat prompt.",
            "60-80% quiet dark area",
            "Save PNG outputs exactly to the target paths listed under each prompt block.",
            "Record prompt id, source path, target path, dimensions, SHA256, `generation_mode`, `mode`, `semantic_model`",
            "lotha_event_background",
            "lotha_option_relics",
            "vakuu_fight_and_temptation",
            "morvi_option_relics",
            "urda_option_relics",
            "ancient_identity_icons",
            "cracked hand mirror reflecting a blade and a trial scroll",
            "forbidden contract-card clamped by a red wax seal",
            "small forked sapling branch wrapped around an upgraded card shard",
            "hooked challenge blade crossing a dark Ancient mask",
            "bitten blue flame");

        Assert.DoesNotContain("epic detailed fantasy", promptPack, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("finely rendered epic fantasy", promptPack, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TemporaryArtAndDuplicateGroupsAreDocumented()
    {
        using var document = JsonDocument.Parse(ReadRepoText(ManifestPath.Split('/')));
        var assets = document.RootElement.GetProperty("assets").EnumerateArray().ToArray();
        var artDirection = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "art-direction.md");

        Assert.Contains("generic_temporary", ReadRepoText(ManifestPath.Split('/')), StringComparison.Ordinal);
        Assert.Contains("source-local reviewed option/icon/card art", artDirection, StringComparison.Ordinal);
        Assert.Contains("Custom card portraits now use source-local reviewed files", artDirection, StringComparison.Ordinal);

        var duplicateGroups = assets
            .Where(asset => asset.TryGetProperty("sha256", out _))
            .GroupBy(asset => RequiredString(asset, "sha256"), StringComparer.Ordinal)
            .Where(group => group.Select(asset => RequiredString(asset, "target_path")).Distinct(StringComparer.Ordinal).Count() > 1)
            .ToArray();
        Assert.True(duplicateGroups.Length > 0, "Known temporary duplicate art groups should remain visible until replaced.");

        foreach (var group in duplicateGroups)
        {
            foreach (var asset in group)
            {
                var status = RequiredString(asset, "source_status");
                var notes = RequiredString(asset, "notes");
                Assert.True(
                    status.EndsWith("_temporary", StringComparison.Ordinal) ||
                    notes.Contains("Temporary", StringComparison.Ordinal) ||
                    notes.Contains("intentionally share", StringComparison.OrdinalIgnoreCase),
                    $"Duplicate art record {RequiredString(asset, "id")} must be explicitly documented.");
            }
        }

        var customCardPortraitTargets = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["urda_seedling_card_portrait_small"] = "EZMicroBalance/images/card_portraits/urda_seedling.png",
            ["urda_seedling_card_portrait_big"] = "EZMicroBalance/images/card_portraits/big/urda_seedling.png",
            ["withered_husk_card_portrait_small"] = "EZMicroBalance/images/card_portraits/withered_husk.png",
            ["withered_husk_card_portrait_big"] = "EZMicroBalance/images/card_portraits/big/withered_husk.png",
            ["morvi_archive_pages_card_portrait_small"] = "EZMicroBalance/images/card_portraits/morvi_archive_pages.png",
            ["morvi_archive_pages_card_portrait_big"] = "EZMicroBalance/images/card_portraits/big/morvi_archive_pages.png",
            ["morvi_red_ink_overdraft_card_portrait_small"] = "EZMicroBalance/images/card_portraits/morvi_red_ink_overdraft.png",
            ["morvi_red_ink_overdraft_card_portrait_big"] = "EZMicroBalance/images/card_portraits/big/morvi_red_ink_overdraft.png",
            ["morvi_waste_paper_card_portrait_small"] = "EZMicroBalance/images/card_portraits/morvi_waste_paper.png",
            ["morvi_waste_paper_card_portrait_big"] = "EZMicroBalance/images/card_portraits/big/morvi_waste_paper.png",
            ["vakuu_temptation_card_portrait_small"] = "EZMicroBalance/images/card_portraits/vakuu_temptation.png",
            ["vakuu_temptation_card_portrait_big"] = "EZMicroBalance/images/card_portraits/big/vakuu_temptation.png"
        };
        foreach (var (assetId, targetPath) in customCardPortraitTargets)
        {
            var asset = assets.Single(candidate => RequiredString(candidate, "id") == assetId);
            Assert.Equal("source_local_generated", RequiredString(asset, "source_status"));
            Assert.Equal(targetPath, RequiredString(asset, "target_path"));
            Assert.Contains("replaces the shared generic card portrait path", RequiredString(asset, "notes"), StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ActiveDocsDoNotCallTemporaryArtFinal()
    {
        var activeDocs = string.Join(
            Environment.NewLine,
            ReadRepoText("PROJECT_STATE.md"),
            ReadRepoText("docs", "issues.md"),
            ReadRepoText("docs", "test-ready-development-goal.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "art-direction.md"),
            ReadRepoText(PromptPackPath.Split('/')),
            ReadRepoText(ManifestPath.Split('/')));

        Assert.Contains("Final bespoke Image API art generated this pass: none.", activeDocs, StringComparison.Ordinal);
        Assert.Contains("not a final-art claim", activeDocs, StringComparison.Ordinal);
        Assert.Contains("must not be called final art", activeDocs, StringComparison.Ordinal);

        foreach (var prohibited in new[]
        {
            "source-derived final art",
            "generic temporary final art",
            "temporary assets are final",
            "placeholder art is final",
            "crop art is final",
            "final source-derived"
        })
        {
            Assert.DoesNotContain(prohibited, activeDocs, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void AuditScriptExistsAndDefaultsToInformationalNonDestructiveMode()
    {
        var script = ReadRepoText(AuditScriptPath.Split('/'));

        AssertSourceContains(
            script,
            "art-asset-manifest.json",
            "export_presets.cfg",
            "[switch]$FailOnMissingExport",
            "[switch]$FailOnInvalidGenerationMode",
            "[switch]$FailOnMissingFinal",
            "[switch]$FailOnHashMismatch",
            "GPTimage2",
            "required_generation_mode",
            "invalid_generation_mode_count",
            "invalid_generation_modes",
            "missing_export_count",
            "missing_exports",
            "$json",
            "if ($FailOnHashMismatch",
            "if ($FailOnMissingExport",
            "if ($FailOnInvalidGenerationMode",
            "if ($FailOnMissingFinal",
            "exit 0",
            "Set-Content -LiteralPath $outFullPath");

        Assert.DoesNotContain("Remove-Item", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Move-Item", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Copy-Item", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Start-Process", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("dotnet publish", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("package-spire-plus", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("$FailOnMissingFinal = $true", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("$FailOnHashMismatch = $true", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("$FailOnMissingExport = $true", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("$FailOnInvalidGenerationMode = $true", script, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Gpt4FreeImageRequestHelperUsesCanonicalPromptPackAndForcedMode()
    {
        var script = ReadRepoText(Gpt4FreeScriptPath.Split('/'));

        AssertSourceContains(
            script,
            "art-generation-prompts.md",
            "art-asset-manifest.json",
            "GPT4FREE_IMAGE_ENDPOINT",
            "GPT4FREE_API_KEY",
            "GPT4FREE_IMAGE_MODEL",
            "$RequiredGenerationMode = \"GPTimage2\"",
            "generation_mode = $RequiredGenerationMode",
            "mode = $RequiredGenerationMode",
            "semantic_model = $RequiredGenerationMode",
            "model = $ApiModel",
            "gpt4free_transport_model: $ApiModel",
            "Get-MarkdownSection",
            "## Prompt Block: $promptId",
            "Append this exact style suffix",
            "Manifest asset '$AssetId' has no prompt_id",
            "Invoke-RestMethod",
            "Invoke-WebRequest",
            "Get-FirstBase64Image",
            "dry-run request");

        Assert.DoesNotContain("art_pipeline/prompts", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("codex_builtin_imagegen", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("dall-e", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("epic detailed fantasy", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Remove-Item", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Move-Item", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Start-Process", script, StringComparison.OrdinalIgnoreCase);
    }

    private static bool RequiresExportCoverage(string targetPath)
    {
        foreach (var prefix in new[]
        {
            "EZMicroBalance/images/ancients/",
            "EZMicroBalance/images/events/",
            "EZMicroBalance/scenes/",
            "EZMicroBalance/images/powers/",
            "EZMicroBalance/images/card_portraits/",
            "EZMicroBalance/images/relics/"
        })
        {
            if (targetPath.StartsWith(prefix, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static string[] ParseExportFiles(string exportPreset)
    {
        var match = Regex.Match(exportPreset, @"export_files=PackedStringArray\((?<files>[^)]*)\)");
        Assert.True(match.Success, "Could not find export_files in export_presets.cfg.");

        return Regex.Matches(match.Groups["files"].Value, @"""(?<path>[^""]+)""")
            .Cast<Match>()
            .Select(match => match.Groups["path"].Value)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
    }

    private static string RequiredString(JsonElement element, string propertyName)
    {
        Assert.True(element.TryGetProperty(propertyName, out var property), $"Missing JSON property: {propertyName}");
        Assert.Equal(JsonValueKind.String, property.ValueKind);
        return property.GetString() ?? string.Empty;
    }

    private static (int Width, int Height) ReadPngDimensions(string path)
    {
        var bytes = File.ReadAllBytes(path);
        Assert.True(bytes.Length >= 24, $"PNG too small to contain IHDR: {path}");
        Assert.True(bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47, $"Not a PNG file: {path}");
        return (
            BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(16, 4)),
            BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(20, 4)));
    }

    private static void AssertSourceContains(string source, params string[] snippets)
    {
        var missing = snippets
            .Where(snippet => !source.Contains(snippet, StringComparison.Ordinal))
            .ToArray();

        Assert.True(missing.Length == 0, "Missing source evidence:" + Environment.NewLine + string.Join(Environment.NewLine, missing));
    }

    private static string ReadRepoText(params string[] parts)
    {
        return File.ReadAllText(RepoPath(parts), Encoding.UTF8);
    }

    private static string RepoPath(params string[] parts)
    {
        return Path.Combine(new[] { FindRepoRoot() }.Concat(parts).ToArray());
    }

    private static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "EZMicroBalance.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repository root from test output directory.");
    }
}
