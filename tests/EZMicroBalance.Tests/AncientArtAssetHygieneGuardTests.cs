using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientArtAssetHygieneGuardTests
{
    private const string ManifestPath = "docs/features/ancient-expansion-v2.2/art-asset-manifest.json";
    private const string PromptPackPath = "docs/features/ancient-expansion-v2.2/art-generation-prompts.md";
    private const string AuditScriptPath = "scripts/audit-ancient-art-assets.ps1";
    private const string Gpt4FreeScriptPath = "scripts/invoke-ancient-art-gpt4free.ps1";
    private const string RequiredGenerationMode = "GPTimage2";

    private const string GlobalStylePhrase =
        "Slay the Spire 2 inspired dark fantasy roguelike card-game art, hand-painted 2D illustration, rough gouache, acrylic paint, and marker texture, painterly flat colors, strong black silhouette, uneven ink outline, transparent background for UI icons, clear storybook shapes, low line density, muted navy-purple shadows, small saturated highlights, grotesque but charming fantasy design, readable at small size, not realistic, not anime, not 3D, not overpolished.";

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
            "boss_seal_holy_daze_indicator",
            "boss_seal_aeonglass_hourglass_indicator",
            "boss_seal_residual_sample_indicator",
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
                    var aspect = (double)actualWidth / actualHeight;
                    Assert.InRange(aspect, 1.76, 1.79);
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
    public void AncientCombatPowerArtUsesDedicatedPackedAndBigRoutes()
    {
        var exportPreset = ReadRepoText("export_presets.cfg");
        var exportedResources = ParseExportFiles(exportPreset).ToHashSet(StringComparer.Ordinal);
        var expectedPowerArt = new Dictionary<string, (int Width, int Height)>(StringComparer.Ordinal)
        {
            ["EZMicroBalance/images/powers/lotha_verdict.png"] = (64, 64),
            ["EZMicroBalance/images/powers/big/lotha_verdict.png"] = (256, 256),
            ["EZMicroBalance/images/powers/lotha_presumption.png"] = (64, 64),
            ["EZMicroBalance/images/powers/big/lotha_presumption.png"] = (256, 256),
            ["EZMicroBalance/images/powers/lotha_death_reprieve.png"] = (64, 64),
            ["EZMicroBalance/images/powers/big/lotha_death_reprieve.png"] = (256, 256),
            ["EZMicroBalance/images/powers/lotha_single_sentence.png"] = (64, 64),
            ["EZMicroBalance/images/powers/big/lotha_single_sentence.png"] = (256, 256),
            ["EZMicroBalance/images/powers/lotha_enlightenment.png"] = (64, 64),
            ["EZMicroBalance/images/powers/big/lotha_enlightenment.png"] = (256, 256),
            ["EZMicroBalance/images/powers/morvi_debt.png"] = (64, 64),
            ["EZMicroBalance/images/powers/big/morvi_debt.png"] = (256, 256),
            ["EZMicroBalance/images/powers/morvi_proofread.png"] = (64, 64),
            ["EZMicroBalance/images/powers/big/morvi_proofread.png"] = (256, 256),
            ["EZMicroBalance/images/powers/morvi_open_book.png"] = (64, 64),
            ["EZMicroBalance/images/powers/big/morvi_open_book.png"] = (256, 256),
            ["EZMicroBalance/images/powers/morvi_overdraft.png"] = (64, 64),
            ["EZMicroBalance/images/powers/big/morvi_overdraft.png"] = (256, 256),
            ["EZMicroBalance/images/powers/morvi_paperstorm.png"] = (64, 64),
            ["EZMicroBalance/images/powers/big/morvi_paperstorm.png"] = (256, 256),
            ["EZMicroBalance/images/powers/morvi_archive_page.png"] = (64, 64),
            ["EZMicroBalance/images/powers/big/morvi_archive_page.png"] = (256, 256),
            ["EZMicroBalance/images/powers/vakuu_stolen_vault.png"] = (64, 64),
            ["EZMicroBalance/images/powers/big/vakuu_stolen_vault.png"] = (256, 256),
            ["EZMicroBalance/images/powers/vakuu_blood_debt.png"] = (64, 64),
            ["EZMicroBalance/images/powers/big/vakuu_blood_debt.png"] = (256, 256)
        };

        foreach (var (relativePath, expectedDimensions) in expectedPowerArt)
        {
            var path = AssertRepoFileExists(relativePath.Split('/'));
            AssertRepoFileExists((relativePath + ".import").Split('/'));
            Assert.Equal(expectedDimensions, ReadPngDimensions(path));
            Assert.Contains($"res://{relativePath}", exportedResources);
        }

        var morvi = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviPowers.cs");
        var lotha = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaPowers.cs");
        var vakuu = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPowers.cs");
        var source = string.Join(Environment.NewLine, morvi, lotha, vakuu);

        AssertSourceContains(
            source,
            "MorviAssetPaths.ArchivePagePowerBigIcon",
            "LothaAssetPaths.VerdictPowerBigIcon",
            "VakuuFightAssetPaths.StolenVaultPowerBigIcon",
            "VakuuFightAssetPaths.BloodDebtPowerBigIcon");
        Assert.DoesNotContain("CustomBigIconPath => MorviAssetPaths.ArchivePagePowerIcon", source, StringComparison.Ordinal);
        Assert.DoesNotContain("VakuuFightAssetPaths.OptionIcon", vakuu, StringComparison.Ordinal);
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

        Assert.Contains("Final browser GPTimage2 small art generated this pass", activeDocs, StringComparison.Ordinal);
        Assert.Contains("No `generic_temporary` or `final_required_before_release` art blockers remain", activeDocs, StringComparison.Ordinal);
        Assert.Contains("Event backgrounds are active middle-draft resources", activeDocs, StringComparison.Ordinal);
        Assert.Contains("Beta.107 smoke-level clicked UI proof exists for forced Urda, Morvi, Lotha, and normal Vakuu", activeDocs, StringComparison.Ordinal);
        Assert.Contains("art readiness still requires live hover/readability review and gated Vakuu fight-option checks", activeDocs, StringComparison.Ordinal);

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
            "EZMicroBalance/images/relics/",
            "EZMicroBalance/images/ascension/"
        })
        {
            if (targetPath.StartsWith(prefix, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static string RequiredString(JsonElement element, string propertyName)
    {
        Assert.True(element.TryGetProperty(propertyName, out var property), $"Missing JSON property: {propertyName}");
        Assert.Equal(JsonValueKind.String, property.ValueKind);
        return property.GetString() ?? string.Empty;
    }
}
