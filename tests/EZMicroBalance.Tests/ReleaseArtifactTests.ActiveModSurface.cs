using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseArtifactTests
{
    [Fact]
    public void CurrentSetupDocsPointAtActiveMod()
    {
        var betaCompatibility = ReadRepoText("docs", "BETA_COMPATIBILITY.md");
        var remoteSetup = ReadRepoText("docs", "REMOTE_DEVELOPMENT_SETUP.md");
        var setupSpec = ReadRepoText("docs", "archive", "superseded", "setup-spec-original-scaffold.md");
        var manualChecklist = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-test-checklist.md");

        Assert.Contains("EZMicroBalance", betaCompatibility, StringComparison.Ordinal);
        Assert.Contains("dotnet list EZMicroBalance.csproj package --include-transitive", betaCompatibility, StringComparison.Ordinal);
        Assert.Contains("Active mod: `Spire Plus`", remoteSetup, StringComparison.Ordinal);
        Assert.Contains("Technical project, manifest id, and install folder: `EZMicroBalance`", remoteSetup, StringComparison.Ordinal);
        Assert.Contains(@"<GameRoot>\mods\EZMicroBalance\EZMicroBalance.dll", remoteSetup, StringComparison.Ordinal);
        Assert.Contains("manifest id `EZMicroBalance`", manualChecklist, StringComparison.Ordinal);
        Assert.Contains(@"<GameRoot>\mods\EZMicroBalance", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Confirm `Spire Plus` appears with manifest id `EZMicroBalance`.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Confirm `Spire Plus` is enabled.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Confirm legacy Easy Content / EzDailyContent is disabled or absent.", manualChecklist, StringComparison.Ordinal);

        Assert.DoesNotContain("dotnet list EzDailyContent.csproj", betaCompatibility, StringComparison.Ordinal);
        Assert.DoesNotContain("Confirm EzDailyContent appears", betaCompatibility, StringComparison.Ordinal);
        Assert.DoesNotContain("Project: `EzDailyContent`", remoteSetup, StringComparison.Ordinal);
        Assert.DoesNotContain(@"<GameRoot>\mods\EzDailyContent\EzDailyContent.dll", remoteSetup, StringComparison.Ordinal);
        Assert.DoesNotContain("current single-mod architecture", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain(@"<GameRoot>\mods\EzDailyContent", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Confirm Easy Content / EzDailyContent appears.", manualChecklist, StringComparison.Ordinal);

        Assert.Contains("Historical note: this document records the original `EzDailyContent` setup baseline", setupSpec, StringComparison.Ordinal);
        Assert.DoesNotContain("SETUP_SPEC.md", ReadRepoText("docs", "README.md"), StringComparison.Ordinal);
    }

    [Fact]
    public void ActiveProjectDoesNotCompileOrPackageLegacySources()
    {
        var project = ReadRepoText("EZMicroBalance.csproj");
        var solution = ReadRepoText("EZMicroBalance.sln");
        var exportPreset = ReadRepoText("export_presets.cfg");

        Assert.Contains("Compile Include=\"EZMicroBalanceCode/**/*.cs\"", project, StringComparison.Ordinal);
        Assert.Contains("AdditionalFiles Include=\"EZMicroBalance/localization/**/*.json\"", project, StringComparison.Ordinal);
        Assert.Contains("GodotPublishInputs Include=\"EZMicroBalance/**\"", project, StringComparison.Ordinal);
        Assert.DoesNotContain("Compile Include=\"EzDailyContentCode", project, StringComparison.Ordinal);
        Assert.DoesNotContain("AdditionalFiles Include=\"EzDailyContent", project, StringComparison.Ordinal);
        Assert.DoesNotContain("GodotPublishInputs Include=\"EzDailyContent", project, StringComparison.Ordinal);

        Assert.Contains("EZMicroBalance.csproj", solution, StringComparison.Ordinal);
        Assert.Contains("EZMicroBalance.Tests.csproj", solution, StringComparison.Ordinal);
        Assert.DoesNotContain("EzDailyContent.csproj", solution, StringComparison.Ordinal);

        Assert.Contains("export_filter=\"resources\"", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance.json", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/localization/eng/relics.json", exportPreset, StringComparison.Ordinal);
        Assert.DoesNotContain("EzDailyContent/*", exportPreset, StringComparison.Ordinal);
        Assert.DoesNotContain("EzDailyContentCode/*", exportPreset, StringComparison.Ordinal);
        Assert.Contains("EZMicroBalanceCode/*", exportPreset, StringComparison.Ordinal);
        Assert.Contains("docs/*", exportPreset, StringComparison.Ordinal);
        Assert.Contains("legacy/*", exportPreset, StringComparison.Ordinal);
        Assert.Contains("forum/*", exportPreset, StringComparison.Ordinal);
        Assert.Contains("output/*", exportPreset, StringComparison.Ordinal);
    }
}
