using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class DocumentationCompactnessGuardTests
{
    [Fact]
    public void PlayerFacingNameStaysSpirePlusWhileTechnicalIdRemainsStable()
    {
        using var manifest = JsonDocument.Parse(ReadRepoText("EZMicroBalance.json"));
        Assert.Equal("EZMicroBalance", manifest.RootElement.GetProperty("id").GetString());
        Assert.Equal("Spire Plus", manifest.RootElement.GetProperty("name").GetString());

        Assert.Equal(
            "Spire Plus",
            JsonStringMap("EZMicroBalance", "localization", "eng", "settings_ui.json")["EZMICROBALANCE.mod_title"]);
        Assert.Equal(
            "Spire Plus",
            JsonStringMap("EZMicroBalance", "localization", "zhs", "settings_ui.json")["EZMICROBALANCE.mod_title"]);

        var godotProject = ReadRepoText("project.godot");
        Assert.Contains("config/name=\"Spire Plus\"", godotProject, StringComparison.Ordinal);
        Assert.Contains("project/assembly_name=\"EZMicroBalance\"", godotProject, StringComparison.Ordinal);
        Assert.DoesNotContain("config/name=\"EZMicroBalance\"", godotProject, StringComparison.Ordinal);

        var projectFile = ReadRepoText("EZMicroBalance.csproj");
        Assert.Contains("Copying Spire Plus compatibility DLL and manifest", projectFile, StringComparison.Ordinal);
        Assert.Contains("Exporting Spire Plus compatibility Godot .pck", projectFile, StringComparison.Ordinal);
        Assert.DoesNotContain("Copying EZMicroBalance Release", projectFile, StringComparison.Ordinal);
        Assert.DoesNotContain("Exporting EZMicroBalance Godot", projectFile, StringComparison.Ordinal);

        var currentMarkdownFiles = Directory
            .GetFiles(Root, "*.md", SearchOption.AllDirectories)
            .Select(ToRepoRelativePath)
            .Where(path =>
                !path.StartsWith("docs/archive/", StringComparison.Ordinal) &&
                !path.StartsWith(".tools/", StringComparison.Ordinal) &&
                !path.StartsWith("publish/", StringComparison.Ordinal) &&
                !path.StartsWith("source code/", StringComparison.Ordinal) &&
                !path.Contains("/bin/", StringComparison.Ordinal) &&
                !path.Contains("/obj/", StringComparison.Ordinal))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.NotEmpty(currentMarkdownFiles);
        var offenders = currentMarkdownFiles
            .Select(path => new { Path = path, Text = ReadRepoText(path.Split('/')) })
            .Where(file =>
                file.Text.Contains("EZ Micro Balance", StringComparison.Ordinal) ||
                file.Text.Contains("EZ Microbalance", StringComparison.Ordinal) ||
                file.Text.Contains("EZmicrobalance", StringComparison.Ordinal))
            .Select(file => file.Path)
            .ToArray();

        Assert.True(
            offenders.Length == 0,
            "Current player/tester-facing markdown must use Spire Plus, not the old display name. Offenders:"
                + Environment.NewLine
                + string.Join(Environment.NewLine, offenders));

        var legacyShorthandOffenders = currentMarkdownFiles
            .Select(path => new { Path = path, Text = ReadRepoText(path.Split('/')) })
            .SelectMany(file => new[]
                {
                    "EZMB-only",
                    "previous package+EZMB",
                    "non-previous package/EZMB",
                    "previous package/EZMB",
                    "no-op EZMB config",
                    "previous package + EZMicroBalance",
                    "Spire Plus / `EZMicroBalance`",
                    "Spire Plus / EZMicroBalance"
                }
                .Where(fragment => file.Text.Contains(fragment, StringComparison.Ordinal))
                .Select(fragment => $"{file.Path}:{fragment}"))
            .ToArray();

        Assert.True(
            legacyShorthandOffenders.Length == 0,
            "Current markdown should say Spire Plus for player/tester-facing setup shorthand; keep EZMicroBalance only for exact technical ids, paths, artifacts, and legacy env-var aliases. Offenders:"
                + Environment.NewLine
                + string.Join(Environment.NewLine, legacyShorthandOffenders));

        var remoteSetup = ReadRepoText("docs", "REMOTE_DEVELOPMENT_SETUP.md");
        var docsIndex = ReadRepoText("docs", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");

        AssertSourceContains(
            remoteSetup,
            "Active mod: `Spire Plus`",
            "Technical project, manifest id, and install folder: `EZMicroBalance`");
        Assert.DoesNotContain("Active project: `EZMicroBalance`", remoteSetup, StringComparison.Ordinal);
        Assert.Contains(
            "Why `Spire Plus` keeps the stable `EZMicroBalance` technical id.",
            docsIndex,
            StringComparison.Ordinal);
        Assert.Contains(
            "Independent `Spire Plus` project created on the stable `EZMicroBalance` technical id",
            projectMap,
            StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerVisibleLocalizationValuesAvoidTechnicalAndLegacyModNames()
    {
        var forbiddenFragments = new[]
        {
            "EZMicroBalance",
            "EZMB",
            "EZ Micro Balance",
            "EZ Microbalance",
            "EZmicrobalance",
            "Easy Content",
            "EzDailyContent",
            "Future Peek",
            "EZFuturePeek"
        };
        var localizationRoots = new[]
            {
                RepoPath("EZMicroBalance", "localization", "eng"),
                RepoPath("EZMicroBalance", "localization", "zhs"),
                RepoPath("website", "assets", "localization", "eng"),
                RepoPath("website", "assets", "localization", "zhs")
            }
            .Where(Directory.Exists)
            .ToArray();

        Assert.NotEmpty(localizationRoots);

        var offenders = localizationRoots
            .SelectMany(root => Directory.GetFiles(root, "*.json", SearchOption.TopDirectoryOnly))
            .OrderBy(path => path, StringComparer.Ordinal)
            .SelectMany(file => JsonStringMap(file)
                .SelectMany(entry => forbiddenFragments
                    .Where(fragment => entry.Value.Contains(fragment, StringComparison.Ordinal))
                    .Select(fragment => $"{ToRepoRelativePath(file)}:{entry.Key}:{fragment}")))
            .ToArray();

        Assert.True(
            offenders.Length == 0,
            "Player-visible localization values must say Spire Plus. Technical ids may remain in keys, paths, manifest id, saved fields, and legacy env-var aliases only. Offenders:"
                + Environment.NewLine
                + string.Join(Environment.NewLine, offenders));
    }
}
