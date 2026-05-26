using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class ModInfoLocalizationGuardTests
{
    [Fact]
    public void GameManifestDescriptionIsStaticAndSpirePlusLocalizesTheModInfoPanel()
    {
        var gameManifestSource = ReadRepoText("source code", "src", "Core", "Modding", "ModManifest.cs");
        var gameModInfoSource = ReadRepoText("source code", "src", "Core", "Nodes", "Screens", "ModdingScreen", "NModInfoContainer.cs");
        var patchSource = ReadRepoText("EZMicroBalanceCode", "Modding", "ModInfoLocalizationPatches.cs");

        AssertSourceContains(
            gameManifestSource,
            "[JsonPropertyName(\"description\")]",
            "public string? description;");
        AssertSourceContains(
            gameModInfoSource,
            "handler.AppendFormatted(mod.manifest?.description ?? \"No description\");");
        AssertSourceContains(
            patchSource,
            "HarmonyPatch(typeof(NModInfoContainer), nameof(NModInfoContainer.Fill))",
            "mod.manifest?.id, MainFile.ModId",
            "LocManager.Instance?.Language",
            "string.Equals(language, \"zhs\", StringComparison.Ordinal)",
            "Spire Plus 是一个用于私测的《杀戮尖塔 2》单体玩法扩展",
            "Spire Plus is a single Slay the Spire 2 gameplay expansion",
            "Seedbed is the clearest example",
            "Planting means combat-only isolation",
            "种下就是本战隔离",
            "根蚀被种下后只冻结这一场",
            "[gold]作者[/gold]",
            "[gold]版本[/gold]",
            "GetNodeOrNull<MegaRichTextLabel>(\"ModDescription\")");

        AssertNoMojibake(patchSource);
        Assert.DoesNotContain("涓", patchSource, StringComparison.Ordinal);
        Assert.DoesNotContain("绉", patchSource, StringComparison.Ordinal);
        Assert.DoesNotContain("鎴", patchSource, StringComparison.Ordinal);
    }

    [Fact]
    public void ManifestKeepsReadableFallbackDescription()
    {
        using var document = JsonDocument.Parse(ReadRepoText("EZMicroBalance.json"));
        var root = document.RootElement;

        Assert.Equal("EZMicroBalance", root.GetProperty("id").GetString());
        Assert.Equal("Spire Plus", root.GetProperty("name").GetString());
        Assert.Equal("v0.1.0-private-beta.25", root.GetProperty("version").GetString());
        Assert.True(root.TryGetProperty("description", out var description));

        var manifestDescription = description.GetString() ?? string.Empty;
        Assert.Contains("Spire Plus", manifestDescription, StringComparison.Ordinal);
        Assert.Contains("Planting is combat-only isolation", manifestDescription, StringComparison.Ordinal);
        Assert.Contains("Rootblight is frozen", manifestDescription, StringComparison.Ordinal);
        Assert.Contains("The Mod Settings panel localizes this description by client language.", manifestDescription, StringComparison.Ordinal);
        Assert.DoesNotContain("涓", manifestDescription, StringComparison.Ordinal);
        Assert.DoesNotContain("绉", manifestDescription, StringComparison.Ordinal);
        AssertNoMojibake(manifestDescription);
        Assert.False(root.TryGetProperty("description_zhs", out _), "The game manifest schema does not read description_zhs; use the UI patch instead.");
    }
}
