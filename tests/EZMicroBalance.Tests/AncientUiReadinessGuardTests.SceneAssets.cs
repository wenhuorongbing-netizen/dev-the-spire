using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientUiReadinessGuardTests
{
    [Fact]
    public void ActiveAncientBackgroundScenesUseControlRootsAndEventArt()
    {
        var exportPreset = ReadRepoText("export_presets.cfg");

        foreach (var scene in ActiveAncientScenes)
        {
            AssertRepoFileExists(scene.ScenePath.Split('/'));
            AssertRepoFileExists(scene.EventArtPath.Split('/'));

            var sceneSource = ReadRepoText(scene.ScenePath.Split('/'));
            Assert.Contains($"[node name=\"{scene.RootNode}\" type=\"Control\"]", sceneSource, StringComparison.Ordinal);
            Assert.Contains("type=\"TextureRect\"", sceneSource, StringComparison.Ordinal);
            Assert.Contains($"path=\"res://{scene.EventArtPath}\"", sceneSource, StringComparison.Ordinal);
            var artworkSource = ExtractNodeBlock(sceneSource, "[node name=\"Artwork\" type=\"TextureRect\" parent=\".\"]");
            Assert.DoesNotContain("anchor_left = ", artworkSource, StringComparison.Ordinal);
            Assert.DoesNotContain("anchor_top = ", artworkSource, StringComparison.Ordinal);
            Assert.Contains("anchor_right = 1.0", artworkSource, StringComparison.Ordinal);
            Assert.Contains("anchor_bottom = 1.0", artworkSource, StringComparison.Ordinal);
            Assert.Contains("expand_mode = 1", sceneSource, StringComparison.Ordinal);
            Assert.Contains("stretch_mode = 5", sceneSource, StringComparison.Ordinal);
            Assert.DoesNotContain("stretch_mode = 6", sceneSource, StringComparison.Ordinal);
            Assert.DoesNotContain("images/ancients", sceneSource, StringComparison.Ordinal);
            Assert.DoesNotContain("map_icon", sceneSource, StringComparison.Ordinal);
            Assert.DoesNotContain("run_history", sceneSource, StringComparison.Ordinal);

            Assert.Contains($"res://{scene.ScenePath}", exportPreset, StringComparison.Ordinal);
            Assert.Contains($"res://{scene.EventArtPath}", exportPreset, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ActiveAncientArtRolesStaySeparated()
    {
        var exportPreset = ReadRepoText("export_presets.cfg");

        foreach (var roleSet in ActiveAncientArtRoles)
        {
            var source = roleSet.Ancient is "Urda" or "Morvi" or "Lotha"
                ? ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", roleSet.Ancient)
                : ReadRepoText(roleSet.SourcePath.Split('/'));
            AssertSourceContains(
                source,
                $"CustomScenePath => {roleSet.AssetPrefix}.BackgroundScene",
                $"CustomMapIconPath => {roleSet.AssetPrefix}.MapIcon",
                $"CustomMapIconOutlinePath => {roleSet.AssetPrefix}.MapIconOutline",
                $"CustomRunHistoryIconPath => {roleSet.AssetPrefix}.RunHistoryIcon",
                $"CustomRunHistoryIconOutlinePath => {roleSet.AssetPrefix}.RunHistoryIconOutline");

            foreach (var (member, path) in new[]
            {
                ("MapIcon", roleSet.MapIconPath),
                ("MapIconOutline", roleSet.MapIconOutlinePath),
                ("RunHistoryIcon", roleSet.RunHistoryIconPath),
                ("RunHistoryIconOutline", roleSet.RunHistoryIconOutlinePath)
            })
            {
                Assert.StartsWith("EZMicroBalance/images/ancients/", path, StringComparison.Ordinal);
                Assert.DoesNotContain("/images/events/", path, StringComparison.Ordinal);
                Assert.NotEqual(roleSet.EventArtPath, path);
                Assert.NotEqual(roleSet.BackgroundScenePath, path);
                Assert.Contains($"{member} => $\"{{MainFile.ResPath}}/{path["EZMicroBalance/".Length..]}\"", source, StringComparison.Ordinal);
                AssertRepoFileExists(path.Split('/'));
                Assert.Contains($"res://{path}", exportPreset, StringComparison.Ordinal);
            }

            Assert.StartsWith("EZMicroBalance/scenes/events/background_scenes/", roleSet.BackgroundScenePath, StringComparison.Ordinal);
            Assert.EndsWith(".tscn", roleSet.BackgroundScenePath, StringComparison.Ordinal);
            Assert.StartsWith("EZMicroBalance/images/events/", roleSet.EventArtPath, StringComparison.Ordinal);
            AssertRepoFileExists(roleSet.BackgroundScenePath.Split('/'));
            AssertRepoFileExists(roleSet.EventArtPath.Split('/'));
            Assert.Contains($"BackgroundScene => $\"{{MainFile.ResPath}}/{roleSet.BackgroundScenePath["EZMicroBalance/".Length..]}\"", source, StringComparison.Ordinal);
            Assert.Contains($"res://{roleSet.BackgroundScenePath}", exportPreset, StringComparison.Ordinal);
            Assert.Contains($"res://{roleSet.EventArtPath}", exportPreset, StringComparison.Ordinal);
        }
    }

    private static string ExtractNodeBlock(string sceneSource, string nodeHeader)
    {
        var start = sceneSource.IndexOf(nodeHeader, StringComparison.Ordinal);
        Assert.True(start >= 0, $"Missing scene node: {nodeHeader}");
        var next = sceneSource.IndexOf("\n[node ", start + nodeHeader.Length, StringComparison.Ordinal);
        return next < 0 ? sceneSource[start..] : sceneSource[start..next];
    }
}
