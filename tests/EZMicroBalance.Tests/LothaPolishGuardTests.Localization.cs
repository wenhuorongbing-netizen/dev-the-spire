using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class LothaPolishGuardTests
{
    private static readonly (string AncientKey, string RelicKey)[] LothaDescriptionKeys =
    [
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description", "EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.description", "EZMICROBALANCE-LOTHA_MIRROR_HALL_ECHO_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_presumption.description", "EZMICROBALANCE-LOTHA_PRESUMPTION_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_closed_court.description", "EZMICROBALANCE-LOTHA_CLOSED_COURT_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.description", "EZMICROBALANCE-LOTHA_DEFERRED_VERDICT_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_death_reprieve.description", "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description", "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description", "EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.description")
    ];

    private static readonly string[] MojibakeFragments =
    [
        "\uFFFD",
        "\u951F?"
    ];

    [Fact]
    public void LothaLocalizationHoverAndRichTextAreReadable()
    {
        var ancient = ReadLothaSource();
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engPowers = JsonStringMap("EZMicroBalance", "localization", "eng", "powers.json");
        var zhsPowers = JsonStringMap("EZMicroBalance", "localization", "zhs", "powers.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        AssertSourceContains(
            ancient,
            "HoverTipFactory.FromPower<LothaPresumptionPower>()",
            "HoverTipFactory.FromPower<LothaVerdictPower>()",
            "HoverTipFactory.FromPower<LothaDeathReprievePower>()",
            "HoverTipFactory.FromPower<LothaEnlightenmentPower>()",
            "HoverTipFactory.Static(StaticHoverTip.ReplayStatic)",
            "HoverTipFactory.Static(StaticHoverTip.Energy)",
            "HoverTipFactory.Static(StaticHoverTip.Block)");

        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.title",
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.description",
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.smartDescription",
                "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.title",
                "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.description",
                "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.smartDescription",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.title",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.description",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.smartDescription",
                "EZMICROBALANCE-LOTHA_VERDICT_POWER.title",
                "EZMICROBALANCE-LOTHA_VERDICT_POWER.description",
                "EZMICROBALANCE-LOTHA_VERDICT_POWER.smartDescription"
            ],
            engPowers,
            zhsPowers,
            "Lotha power localization");

        foreach (var (ancientKey, relicKey) in LothaDescriptionKeys)
        {
            Assert.Equal(engAncients[ancientKey], engRelics[relicKey]);
            Assert.Equal(zhsAncients[ancientKey], zhsRelics[relicKey]);
        }

        foreach (var value in LothaOptionValues(zhsAncients, zhsRelics, zhsPowers))
        {
            AssertNoMojibake(value, MojibakeFragments);
            Assert.DoesNotContain("\uFFFD", value, StringComparison.Ordinal);
            Assert.DoesNotContain("閺€", value, StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]能力牌[/gold]改为获得[blue]1[/blue]点[gold]能量[/gold]并抽[blue]1[/blue]张牌", value, StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]能力牌[/gold]改为获得[blue]2[/blue]点[gold]能量[/gold]并抽[blue]2[/blue]张牌", value, StringComparison.Ordinal);
        }

        foreach (var value in LothaOptionValues(engAncients, engRelics, engPowers))
        {
            Assert.DoesNotContain("option art marker", value, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Every third", value, StringComparison.Ordinal);
            Assert.DoesNotContain("replayed [blue]2[/blue] times as [gold]Exhaust[/gold] copies", value, StringComparison.Ordinal);
            Assert.DoesNotContain("25%", value, StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]Power[/gold] cards instead gain [blue]1[/blue] [gold]Energy[/gold]", value, StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]Power[/gold] instead grants [blue]1[/blue] [gold]Energy[/gold]", value, StringComparison.Ordinal);
            Assert.DoesNotContain("gain [blue]10[/blue] [gold]Block[/gold]", value, StringComparison.Ordinal);
            Assert.DoesNotContain("exactly one card", value, StringComparison.OrdinalIgnoreCase);
        }

        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description"],
            "[gold]Attack[/gold]",
            "[gold]Skill[/gold]",
            "[gold]Power[/gold]",
            "[blue]1[/blue]",
            "[blue]0[/blue]");
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.description"],
            "[gold]Verdict[/gold]",
            "[gold]Attack[/gold]",
            "[gold]Skill[/gold]",
            "[gold]Power[/gold]",
            "[gold]Energy[/gold]");
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"],
            "[gold]negative status[/gold]",
            "[gold]Enlightenment[/gold]",
            "[gold]Block[/gold]",
            "[blue]3[/blue]");
        AssertSourceContains(
            zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description"],
            "[gold]攻击牌[/gold]",
            "[gold]技能牌[/gold]",
            "[gold]能力牌[/gold]");
        AssertSourceContains(
            zhsPowers["EZMICROBALANCE-LOTHA_VERDICT_POWER.description"],
            "[gold]裁决[/gold]",
            "[gold]攻击牌[/gold]",
            "[gold]技能牌[/gold]",
            "[gold]能力牌[/gold]");
        AssertSourceContains(
            zhsPowers["EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.description"],
            "[gold]开悟[/gold]",
            "[gold]格挡[/gold]");
    }
    private static IEnumerable<string> LothaOptionValues(params IReadOnlyDictionary<string, string>[] maps)
    {
        foreach (var map in maps)
        {
            foreach (var (key, value) in map)
            {
                if (key.Contains("LOTHA", StringComparison.Ordinal) || key.Contains("lotha", StringComparison.Ordinal))
                {
                    yield return value;
                }
            }
        }
    }
}
