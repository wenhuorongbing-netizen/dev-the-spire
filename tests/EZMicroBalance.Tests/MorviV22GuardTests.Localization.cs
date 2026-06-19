using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class MorviV22GuardTests
{
    [Fact]
    public void MorviLocalizationAssetsAndHoverSupportArePresentAndReadable()
    {
        var ancient = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi");
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviPowers.cs");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var engPowers = JsonStringMap("EZMicroBalance", "localization", "eng", "powers.json");
        var zhsPowers = JsonStringMap("EZMicroBalance", "localization", "zhs", "powers.json");

        AssertSourceContains(
            ancient,
            "HoverTipFactory.FromPower<MorviOverdraftPower>()",
            "HoverTipFactory.FromPower<MorviOpenBookPower>()",
            "HoverTipFactory.FromPower<MorviPaperstormPower>()",
            "HoverTipFactory.FromPower<MorviProofreadPower>()",
            "HoverTipFactory.FromPower<MorviDebtPower>()",
            "HoverTipFactory.Static(StaticHoverTip.Energy)",
            "HoverTipFactory.Static(StaticHoverTip.Block)");
        AssertSourceContains(
            powers,
            "internal sealed class MorviDebtPower",
            "internal sealed class MorviProofreadPower",
            "internal sealed class MorviOpenBookPower",
            "internal sealed class MorviOverdraftPower",
            "internal sealed class MorviPaperstormPower");

        foreach (var id in BlessingIds)
        {
            var key = $"EZMB_MORVI.pages.INITIAL.options.{id}.description";
            Assert.True(engAncients.TryGetValue(key, out var engDescription), $"Missing English Morvi ancient localization: {key}");
            Assert.True(zhsAncients.TryGetValue(key, out var zhsDescription), $"Missing zhs Morvi ancient localization: {key}");
            AssertNoMojibake(engDescription, MojibakeFragments);
            AssertNoMojibake(zhsDescription, MojibakeFragments);
            Assert.Contains("[blue]", engDescription, StringComparison.Ordinal);
            Assert.Contains("[blue]", zhsDescription, StringComparison.Ordinal);
        }

        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_misprint_press.description"],
            "Once each turn",
            "manually played deck",
            "[gold]Attack[/gold]",
            "[gold]Skill[/gold]",
            "[gold]Power[/gold]",
            "[gold]Energy[/gold]",
            "generated cards do not trigger");
        Assert.DoesNotContain("Borrow one upgraded", engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_forbidden_loan.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("借一张", zhsAncients["EZMB_MORVI.pages.INITIAL.options.morvi_forbidden_loan.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("借来的牌", zhsRelics["EZMICROBALANCE-MORVI_FORBIDDEN_LOAN_OPTION_RELIC.description"], StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_red_ink_overdraft.description"],
            "[gold]Overdraft[/gold]",
            "[gold]Energy[/gold]",
            "nonlethal HP");
        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_blueprint_proof.description"],
            "manually played deck cards",
            "draw [blue]1[/blue]",
            "gain [blue]4[/blue] [gold]Block[/gold]",
            "[gold]Proofread[/gold]",
            "[gold]Block[/gold]");
        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.description"],
            "Take [blue]320[/blue] [gold]Debt[/gold]",
            "repay [blue]40[/blue] [gold]Gold[/gold]",
            "for each [blue]10[/blue] short",
            "lose [blue]3[/blue] nonlethal HP");
        AssertSourceContains(
            zhsAncients["EZMB_MORVI.pages.INITIAL.options.morvi_misprint_press.description"],
            "每回合一次",
            "手动打出",
            "生成牌不触发");
        AssertSourceContains(
            zhsAncients["EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.description"],
            "获得[blue]320[/blue]点[gold]债务[/gold]",
            "每场战斗后偿还[blue]40[/blue][gold]金币[/gold]",
            "每短缺[blue]10[/blue][gold]金币[/gold]",
            "失去[blue]3[/blue]点非致命生命");
        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_open_book_exam.description"],
            "sealed in the [gold]Exhaust Pile[/gold]");

        AssertLocalizedKeys(MorviRelicKeys(), engRelics, zhsRelics, "Morvi option relic localization", value => AssertNoMojibake(value, MojibakeFragments));
        AssertLocalizedKeys(MorviPowerKeys(), engPowers, zhsPowers, "Morvi power localization", value => AssertNoMojibake(value, MojibakeFragments));

        foreach (var relativePath in MorviResourcePaths())
        {
            AssertRepoFileExists(relativePath.Split('/'));
            Assert.Contains($"res://{relativePath}", exportPreset, StringComparison.Ordinal);
        }
    }

    private static IEnumerable<string> MorviRelicKeys()
    {
        foreach (var key in new[]
        {
            "FORBIDDEN_LOAN",
            "MISPRINT_PRESS",
            "RED_INK_OVERDRAFT",
            "OVERDUE_LIBRARY",
            "OPEN_BOOK_EXAM",
            "PAPERSTORM",
            "BLUEPRINT_PROOF",
            "DEBT_SETTLEMENT"
        })
        {
            yield return $"EZMICROBALANCE-MORVI_{key}_OPTION_RELIC.title";
            yield return $"EZMICROBALANCE-MORVI_{key}_OPTION_RELIC.description";
            yield return $"EZMICROBALANCE-MORVI_{key}_OPTION_RELIC.flavor";
        }
    }

    private static IEnumerable<string> MorviPowerKeys()
    {
        foreach (var key in new[]
        {
            "DEBT",
            "PROOFREAD",
            "OPEN_BOOK",
            "OVERDRAFT",
            "PAPERSTORM"
        })
        {
            yield return $"EZMICROBALANCE-MORVI_{key}_POWER.title";
            yield return $"EZMICROBALANCE-MORVI_{key}_POWER.description";
            yield return $"EZMICROBALANCE-MORVI_{key}_POWER.smartDescription";
        }
    }

    private static IEnumerable<string> MorviResourcePaths()
    {
        yield return "EZMicroBalance/images/events/ezmb_morvi.png";
        yield return "EZMicroBalance/images/ancients/morvi/ezmb_morvi_map_icon.png";
        yield return "EZMicroBalance/images/ancients/morvi/ezmb_morvi_map_icon_outline.png";
        yield return "EZMicroBalance/images/ancients/morvi/ezmb_morvi_run_history_icon.png";
        yield return "EZMicroBalance/images/ancients/morvi/ezmb_morvi_run_history_icon_outline.png";
        foreach (var id in BlessingIds)
        {
            yield return $"EZMicroBalance/images/ancients/morvi/options/{id}.png";
        }

        yield return "EZMicroBalance/scenes/events/background_scenes/ezmb_morvi.tscn";
    }
}
