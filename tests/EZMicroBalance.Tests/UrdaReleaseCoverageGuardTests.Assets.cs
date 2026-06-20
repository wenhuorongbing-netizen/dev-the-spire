using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class UrdaReleaseCoverageGuardTests
{
    private static void AssertUrdaSceneAndAssetCoverage(string urdaScene, string exportPreset)
    {
        AssertSourceContains(
            urdaScene,
            "[node name=\"EzmbUrdaBackground\" type=\"Control\"]",
            "[node name=\"Artwork\" type=\"TextureRect\" parent=\".\"]",
            "texture = ExtResource(\"1_urda\")");
        Assert.DoesNotContain("[node name=\"EzmbUrdaBackground\" type=\"Node2D\"]", urdaScene, StringComparison.Ordinal);
        Assert.DoesNotContain("type=\"Sprite2D\"", urdaScene, StringComparison.Ordinal);

        foreach (var relativePath in new[]
        {
            "EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon_outline.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon_outline.png",
            "EZMicroBalance/images/ancients/urda/options/urda_seedbed.png",
            "EZMicroBalance/images/ancients/urda/options/urda_humus_pact.png",
            "EZMicroBalance/images/ancients/urda/options/urda_molting.png",
            "EZMicroBalance/images/ancients/urda/options/urda_moss_map.png",
            "EZMicroBalance/images/ancients/urda/options/urda_trial_branch.png",
            "EZMicroBalance/images/ancients/urda/options/urda_shallow_root_relic.png",
            "EZMicroBalance/images/ancients/urda/options/urda_elite_root.png",
            "EZMicroBalance/images/ancients/urda/options/urda_rooted_route.png",
            "EZMicroBalance/images/ancients/urda/options/urda_after_rain.png",
            "EZMicroBalance/images/ancients/urda/options/urda_root_sight.png",
            "EZMicroBalance/images/ancients/urda/options/urda_seed_bank.png"
        })
        {
            AssertRepoFileExists(relativePath.Split('/'));
            Assert.Contains($"res://{relativePath}", exportPreset, StringComparison.Ordinal);
        }
    }
}
