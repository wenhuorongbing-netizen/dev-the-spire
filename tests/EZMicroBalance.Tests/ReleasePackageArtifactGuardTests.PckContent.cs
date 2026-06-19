using System.IO.Compression;
using System.Text;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleasePackageArtifactGuardTests
{
    [ReleaseArtifactFact]
    public void InstalledAndPackagedPckCarrySereTalonTanxClawsSplit()
    {
        var installedPck = GamePath("mods", "EZMicroBalance", "EZMicroBalance.pck");
        Assert.True(File.Exists(installedPck), $"Missing installed PCK: {installedPck}");

        AssertSereTalonTanxClawsSplitIsPackaged(File.ReadAllBytes(installedPck), "installed PCK");

        using var archive = ZipFile.OpenRead(CurrentPackageZipPath());
        AssertSereTalonTanxClawsSplitIsPackaged(
            ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.pck"),
            "package PCK");
    }

    [ReleaseArtifactFact]
    public void InstalledAndPackagedPckCarryTrialBranchShortChoiceText()
    {
        var installedPck = GamePath("mods", "EZMicroBalance", "EZMicroBalance.pck");
        Assert.True(File.Exists(installedPck), $"Missing installed PCK: {installedPck}");

        AssertTrialBranchShortChoiceTextIsPackaged(File.ReadAllBytes(installedPck), "installed PCK");

        using var archive = ZipFile.OpenRead(CurrentPackageZipPath());
        AssertTrialBranchShortChoiceTextIsPackaged(
            ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.pck"),
            "package PCK");
    }

    private static void AssertSereTalonTanxClawsSplitIsPackaged(byte[] pckBytes, string context)
    {
        var pckText = Encoding.UTF8.GetString(pckBytes);

        Assert.Contains("\"SERE_TALON.description\": \"On pickup, choose [blue]1[/blue] of [blue]4[/blue] Curses. Add it, [blue]2[/blue] Wish, and [blue]1[/blue] Wish+ to your deck.\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"SERE_TALON.selectionScreenPrompt\": \"Choose 1 Curse.\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"SERE_TALON.title\": \"Vakuu's Sere Talon\"", pckText, StringComparison.Ordinal);
        Assert.Contains("sere_talon_spire_plus.png", pckText, StringComparison.Ordinal);
        Assert.Contains("\"SERE_TALON.description\": \"\u62fe\u53d6\u65f6\uff0c\u4ece[blue]4[/blue]\u5f20\u8bc5\u5492\u4e2d\u9009\u62e9[blue]1[/blue]\u5f20\u3002\u5c06\u5b83\u3001[blue]2[/blue]\u5f20[gold]\u8bb8\u613f[/gold]\u548c[blue]1[/blue]\u5f20[gold]\u8bb8\u613f+[/gold]\u52a0\u5165\u4f60\u7684\u724c\u7ec4\u3002\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"SERE_TALON.selectionScreenPrompt\": \"\u9009\u62e91\u5f20\u8bc5\u5492\u3002\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"SERE_TALON.title\": \"\u74e6\u5e93\u539f\u521d\u4e4b\u722a\"", pckText, StringComparison.Ordinal);

        Assert.Contains("\"CLAWS.description\": \"On pickup, transform up to [blue]{Cards}[/blue] cards into upgraded Maul.\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"CLAWS.title\": \"Tanx Claws\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"CLAWS.description\": \"\u62fe\u53d6\u65f6\uff0c\u5c06\u81f3\u591a[blue]{Cards}[/blue]\u5f20\u724c\u53d8\u5316\u4e3a\u6495\u54ac+\u3002\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"CLAWS.title\": \"\u5766\u514b\u65af\u5229\u722a\"", pckText, StringComparison.Ordinal);

        foreach (var staleFragment in new[]
                 {
                     "\"CLAWS.description\": \"Choose 1 of 4 Curses",
                     "No longer transforms deck cards",
                     "random Curses and [blue]3[/blue] Wish",
                     "\"SERE_TALON.description\": \"claws.png\"",
                     "Sere Talon\", \"CLAWS.description\"",
                     "Vakuu's Sere Talon\", \"CLAWS.description\""
                 })
        {
            Assert.DoesNotContain(staleFragment, pckText, StringComparison.Ordinal);
        }
    }

    private static void AssertTrialBranchShortChoiceTextIsPackaged(byte[] pckBytes, string context)
    {
        var pckText = Encoding.UTF8.GetString(pckBytes);

        Assert.Contains("\"EZMB_URDA.pages.INITIAL.options.urda_trial_branch.description\": \"Choose [blue]1[/blue] of [blue]4[/blue] cards.", pckText, StringComparison.Ordinal);
        Assert.Contains("\"EZMB_URDA.pages.INITIAL.options.urda_trial_branch.selectionScreenPrompt\": \"Choose [blue]1[/blue] card for [gold]Trial Branch[/gold].\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.description\": \"Choose [blue]1[/blue] of [blue]4[/blue] cards.", pckText, StringComparison.Ordinal);
        Assert.Contains("\"EZMB_URDA.pages.INITIAL.options.urda_trial_branch.description\": \"\u4ece[blue]4[/blue]\u5f20\u724c\u4e2d\u9009\u62e9[blue]1[/blue]\u5f20\u3002", pckText, StringComparison.Ordinal);
        Assert.Contains("\"EZMB_URDA.pages.INITIAL.options.urda_trial_branch.selectionScreenPrompt\": \"\u4e3a[gold]\u8bd5\u70bc\u679d\u6761[/gold]\u9009\u62e9[blue]1[/blue]\u5f20\u724c\u3002\"", pckText, StringComparison.Ordinal);
        Assert.Contains("\"EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.description\": \"\u4ece[blue]4[/blue]\u5f20\u724c\u4e2d\u9009\u62e9[blue]1[/blue]\u5f20\u3002", pckText, StringComparison.Ordinal);

        foreach (var staleFragment in new[]
                 {
                     "\"EZMB_URDA.pages.INITIAL.options.urda_trial_branch.description\": \"Choose [blue]1[/blue] of [blue]4[/blue] [gold]rare[/gold]",
                     "\"EZMB_URDA.pages.INITIAL.options.urda_trial_branch.description\": \"Choose [blue]1[/blue] [gold]rare[/gold]",
                     "\"EZMB_URDA.pages.INITIAL.options.urda_trial_branch.selectionScreenPrompt\": \"Choose [blue]1[/blue] [gold]rare[/gold]",
                     "\"EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.description\": \"Choose [blue]1[/blue] of [blue]4[/blue] [gold]rare[/gold]"
                 })
        {
            Assert.DoesNotContain(staleFragment, pckText, StringComparison.Ordinal);
        }
    }
}
