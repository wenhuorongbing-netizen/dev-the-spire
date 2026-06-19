using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientArtAssetHygieneGuardTests
{
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
            ".tools/art-generation/lotha-background-repair-20260515-feedback/sources/lotha-horizontal-mirror-ensemble-upload-source.png",
            "corrected user-uploaded horizontal mirror-ensemble source",
            "crystal-throne-of-shattered-visions.png` file is a similarly named but rejected composition",
            "Do not overcorrect toward later darker and emptier iterations",
            "Small option relic sheets should inherit the first preview's dark mirror-card finish",
            "Before any small-art review candidate is promoted to an active resource, inspect it at target size",
            "If a candidate loses its silhouette or becomes mostly black at target size, regenerate only that weak asset or weak group",
            "Identity outline icons should be simple UI glyphs with hollow centers",
            "transparent PNG with no opaque black, navy, or paper square behind the symbol",
            "flat readable color blocks with thick acrylic/marker paint",
            "low line density",
            "no paper texture, pseudo-writing, or label-like detail",
            "Keep review contact sheets and target-size audit sheets under `.tools/art-generation/chatgpt/`",
            "Prefer a 16:9 composition for clicked Ancient event backgrounds",
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
    public void FinalArtAndDuplicateGroupsAreDocumented()
    {
        using var document = JsonDocument.Parse(ReadRepoText(ManifestPath.Split('/')));
        var assets = document.RootElement.GetProperty("assets").EnumerateArray().ToArray();
        var artDirection = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "art-direction.md");

        Assert.DoesNotContain("generic_temporary", ReadRepoText(ManifestPath.Split('/')), StringComparison.Ordinal);
        Assert.Contains("Final browser GPTimage2 small art generated this pass", artDirection, StringComparison.Ordinal);
        Assert.Contains("No `generic_temporary` or `final_required_before_release` art blockers remain", artDirection, StringComparison.Ordinal);
        Assert.Contains("Custom card portraits now use browser GPTimage2 rebuilt files", artDirection, StringComparison.Ordinal);

        var duplicateGroups = assets
            .Where(asset => asset.TryGetProperty("sha256", out _))
            .GroupBy(asset => RequiredString(asset, "sha256"), StringComparer.Ordinal)
            .Where(group => group.Select(asset => RequiredString(asset, "target_path")).Distinct(StringComparer.Ordinal).Count() > 1)
            .ToArray();
        Assert.True(duplicateGroups.Length > 0, "Intentional shared map/run-history identity icon groups should remain visible and documented.");

        foreach (var group in duplicateGroups)
        {
            foreach (var asset in group)
            {
                var notes = RequiredString(asset, "notes");
                Assert.Contains("intentionally share", notes, StringComparison.OrdinalIgnoreCase);
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
            Assert.Equal("final_generated", RequiredString(asset, "source_status"));
            Assert.Equal(targetPath, RequiredString(asset, "target_path"));
            Assert.Contains("replaces the shared generic card portrait path", RequiredString(asset, "notes"), StringComparison.Ordinal);
        }
    }
}
