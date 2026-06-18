using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class LothaPolishGuardTests
{
    [Fact]
    public void PublicEvidenceUsesNonDamageDebuffPolicyAndVisibleEnlightenment()
    {
        var ancient = ReadLothaSource();
        var runHook = ReadLothaSource();
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaPowers.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var helper = SliceBetween(runHook, "private static bool IsPublicEvidenceDebuffApplication", "private static bool IsPublicEvidenceExcludedDamageDebuff");
        var publicEvidenceSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.PublicEvidence.cs");
        var excludedDamageDebuffs = SliceFrom(publicEvidenceSource, "private static bool IsPublicEvidenceExcludedDamageDebuff");
        var givenHook = SliceBetween(runHook, "public static decimal ModifyPowerAmountGivenAdditive", "public static bool TryModifyPowerAmountReceived");
        var receivedHook = SliceBetween(runHook, "public static bool TryModifyPowerAmountReceived", "public static async Task AfterPowerAmountChanged");
        var changedHook = SliceBetween(runHook, "public static async Task AfterPowerAmountChanged", "private static async Task ConsumePublicEvidenceEnlightenmentAtTurnStart");

        AssertSourceContains(
            ancient,
            "HoverTipFactory.FromPower<LothaEnlightenmentPower>()",
            "HoverTipFactory.Static(StaticHoverTip.Block)");
        AssertSourceContains(
            runHook,
            "ModifyPowerAmountGivenAdditive",
            "TryModifyPowerAmountReceived",
            "AfterPowerAmountChanged",
            "PublicEvidenceEnlightenmentGain = 1",
            "PublicEvidenceConsumeLimit = 3",
            "PublicEvidenceBlockPerEnlightenment = 4",
            "PublicEvidenceCardsPerEnlightenment = 1",
            "amount * 2m",
            "PowerCmd.Apply<LothaEnlightenmentPower>",
            "RemoveOnePublicEvidenceEnlightenment",
            "ConsumePublicEvidenceEnlightenmentAtTurnStart",
            "IsPublicEvidenceDebuffApplication(power, amount)",
            "IsPublicEvidenceDebuffApplication(canonicalPower, amount)",
            "power.GetTypeForAmount(amount) == PowerType.Debuff");
        AssertSourceContains(
            helper,
            "power.GetTypeForAmount(amount) == PowerType.Debuff",
            "!IsPublicEvidenceExcludedDamageDebuff(power)");
        AssertSourceContains(
            excludedDamageDebuffs,
            "power is PoisonPower",
            "or ConstrictPower",
            "or DemisePower",
            "or DisintegrationPower",
            "or DoomPower",
            "or MagicBombPower",
            "or StranglePower",
            "or TheGambitPower",
            "Core v0.106.1 models these as Debuffs");
        AssertSourceContains(
            powers,
            "internal sealed class LothaEnlightenmentPower",
            "PowerType.Buff",
            "PowerStackType.Counter");

        Assert.DoesNotContain("HasVisibleDebuff", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("PublicEvidenceUsedThisTurn", runHook, StringComparison.Ordinal);
        AssertSourceContains(
            givenHook,
            "target is not { IsEnemy: true }",
            "!giver.IsPlayer",
            "GetSelectedBlessing(player) != LothaBlessingIds.PublicEvidence");
        AssertSourceContains(
            receivedHook,
            "!target.IsPlayer",
            "applier is not { IsEnemy: true }",
            "GetSelectedBlessing(player) != LothaBlessingIds.PublicEvidence");
        AssertSourceContains(
            changedHook,
            "applier is { IsPlayer: true, Player: { } applyingPlayer }",
            "power.Owner.IsEnemy",
            "applier is { IsEnemy: true }",
            "power.Owner is { IsPlayer: true, Player: { } targetPlayer }");
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"],
            "Your non-damaging [gold]negative status[/gold] stacks apply twice",
            "grant [blue]1[/blue] [gold]Enlightenment[/gold]",
            "Enemy non-damaging [gold]negative status[/gold] stacks on you also apply twice",
            "remove [blue]1[/blue] [gold]Enlightenment[/gold]",
            "spend up to [blue]3[/blue] [gold]Enlightenment[/gold]",
            "each spent stack draws [blue]1[/blue] and gives [blue]4[/blue] [gold]Block[/gold]");
        AssertSourceContains(
            zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"],
            "[gold]",
            "[/gold]",
            "[blue]3[/blue]",
            "[blue]1[/blue]",
            "[blue]4[/blue]");
        Assert.Equal(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"],
            engRelics["EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.description"]);
        Assert.Equal(
            zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"],
            zhsRelics["EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.description"]);
    }

    [LocalSourceFact]
    public void CoreDebuffPowersKeepPublicEvidenceDamageAndNonDamageShapes()
    {
        var poison = ReadLocalCoreText("Models", "Powers", "PoisonPower.cs");
        var weak = ReadLocalCoreText("Models", "Powers", "WeakPower.cs");
        var vulnerable = ReadLocalCoreText("Models", "Powers", "VulnerablePower.cs");
        var frail = ReadLocalCoreText("Models", "Powers", "FrailPower.cs");

        AssertSourceContains(
            poison,
            "public override PowerType Type => PowerType.Debuff",
            "AfterSideTurnStart",
            "CreatureCmd.Damage",
            "ValueProp.Unblockable | ValueProp.Unpowered");
        AssertSourceContains(
            weak,
            "public override PowerType Type => PowerType.Debuff",
            "ModifyDamageMultiplicative",
            "PowerCmd.TickDownDuration(this)");
        AssertSourceContains(
            vulnerable,
            "public override PowerType Type => PowerType.Debuff",
            "ModifyDamageMultiplicative",
            "PowerCmd.TickDownDuration(this)");
        AssertSourceContains(
            frail,
            "public override PowerType Type => PowerType.Debuff",
            "ModifyBlockMultiplicative",
            "PowerCmd.TickDownDuration(this)");
        Assert.DoesNotContain("CreatureCmd.Damage", weak, StringComparison.Ordinal);
        Assert.DoesNotContain("CreatureCmd.Damage", vulnerable, StringComparison.Ordinal);
        Assert.DoesNotContain("CreatureCmd.Damage", frail, StringComparison.Ordinal);
    }

    [Fact]
    public void PublicEvidenceDocsCloseDebuffAmbiguityWithoutPoisonClaim()
    {
        var riskRegister = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "risk-register.md");
        var sourceDesign = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "source-design.md");
        var manualChecklist = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md");
        var issue = ReadRepoText("docs", "issues", "ancient-expansion-v2.2.md");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");

        AssertSourceContains(
            riskRegister,
            "source-closed / live-pending",
            "WeakPower",
            "VulnerablePower",
            "FrailPower",
            "PoisonPower",
            "ConstrictPower",
            "DoomPower",
            "damage/kill Debuffs");
        Assert.DoesNotContain("Define exact source-backed debuff list and ownership.", riskRegister, StringComparison.Ordinal);

        foreach (var doc in new[] { sourceDesign, manualChecklist, issue })
        {
            AssertSourceContains(
                doc,
                "non-damaging negative",
                "Weak",
                "Vulnerable",
                "Frail",
                "Poison",
                "damage-over-time",
                "countdown damage");
        }

        Assert.DoesNotContain("\uFFFD", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("\uFFFD", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.Ordinal);
    }
}
