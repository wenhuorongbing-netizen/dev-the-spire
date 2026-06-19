using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientUiReadinessGuardTests
{
    [Fact]
    public void AncientAndVakuuArtAssetsUseStableUiSizedRoles()
    {
        foreach (var scene in ActiveAncientScenes)
        {
            Assert.Equal((1920, 1080), ReadPngDimensions(RepoPath(scene.EventArtPath.Split('/'))));
        }

        foreach (var roleSet in ActiveAncientArtRoles)
        {
            Assert.Equal((128, 128), ReadPngDimensions(RepoPath(roleSet.MapIconPath.Split('/'))));
            Assert.Equal((128, 128), ReadPngDimensions(RepoPath(roleSet.MapIconOutlinePath.Split('/'))));
            Assert.Equal((128, 128), ReadPngDimensions(RepoPath(roleSet.RunHistoryIconPath.Split('/'))));
            Assert.Equal((128, 128), ReadPngDimensions(RepoPath(roleSet.RunHistoryIconOutlinePath.Split('/'))));
        }

        foreach (var marker in OptionMarkers)
        {
            Assert.Equal((128, 128), ReadPngDimensions(RepoPath(marker.AssetPath.Split('/'))));
        }

        Assert.Equal((1920, 1080), ReadPngDimensions(RepoPath("EZMicroBalance", "images", "encounters", "vakuu_trial_backdrop.png")));
        Assert.Equal((512, 384), ReadPngDimensions(RepoPath("EZMicroBalance", "images", "monsters", "vakuu_trial.png")));
        Assert.Equal((250, 190), ReadPngDimensions(RepoPath("EZMicroBalance", "images", "card_portraits", "vakuu_temptation.png")));
        Assert.Equal((1000, 760), ReadPngDimensions(RepoPath("EZMicroBalance", "images", "card_portraits", "big", "vakuu_temptation.png")));
    }

    [Fact]
    public void OptionMarkerRelicsHaveArtAndBilingualLocalizationCoverage()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        foreach (var marker in OptionMarkers)
        {
            AssertSourceContains(source, marker.RelicClass, marker.AssetMember);
            Assert.StartsWith("EZMicroBalance/images/ancients/", marker.AssetPath, StringComparison.Ordinal);
            Assert.DoesNotContain("/images/events/", marker.AssetPath, StringComparison.Ordinal);
            Assert.DoesNotContain("map_icon", marker.AssetPath, StringComparison.Ordinal);
            Assert.DoesNotContain("run_history", marker.AssetPath, StringComparison.Ordinal);
            Assert.NotEqual("EZMicroBalance/images/relics/relic.png", marker.AssetPath);
            Assert.DoesNotContain($"{marker.AssetMember} => $\"{{MainFile.ResPath}}/images/relics/relic.png\"", source, StringComparison.Ordinal);
            AssertRepoFileExists(marker.AssetPath.Split('/'));
            Assert.Contains($"res://{marker.AssetPath}", exportPreset, StringComparison.Ordinal);

            foreach (var suffix in new[] { ".title", ".description", ".flavor" })
            {
                AssertLocalizedValue(engRelics, marker.RelicKey + suffix);
                AssertLocalizedValue(zhsRelics, marker.RelicKey + suffix);
            }
        }
    }
}
