using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
{
    [Fact]
    public void DistinguishedCapeUsesV43MaxHpMathAndCannotBeSelectedWhenUnableToPay()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        Assert.Equal("On pickup, lose 30% of current Max HP, at least 18. Add 3 Apparitions.", relics["DISTINGUISHED_CAPE.description"]);
        Assert.DoesNotContain("cannot reduce Max HP below 1", relics["DISTINGUISHED_CAPE.description"], StringComparison.Ordinal);
        Assert.Contains("Add 3 Apparitions", relics["DISTINGUISHED_CAPE.description"], StringComparison.Ordinal);

        Assert.Equal(24, DistinguishedCapeLossForTest(80));
        Assert.Equal(21, DistinguishedCapeLossForTest(70));
        Assert.Equal(18, DistinguishedCapeLossForTest(60));
        Assert.Equal(18, DistinguishedCapeLossForTest(30));
        Assert.Equal(18, DistinguishedCapeLossForTest(19));
        Assert.Equal(18, DistinguishedCapeLossForTest(18));
        Assert.Equal(18, DistinguishedCapeLossForTest(10));
        Assert.Equal(18, DistinguishedCapeLossForTest(1));
        Assert.True(CanPayDistinguishedCapeCostForTest(80));
        Assert.True(CanPayDistinguishedCapeCostForTest(19));
        Assert.False(CanPayDistinguishedCapeCostForTest(18));
        Assert.False(CanPayDistinguishedCapeCostForTest(10));

        AssertSourceContains(
            source,
            "public const decimal MaxHpLossPercent = 0.30m",
            "public const int MinimumMaxHpLoss = 18",
            "public const int ApparitionsToAdd = 3",
            "var proportionalLoss = (int)Math.Ceiling(currentMaxHp * MaxHpLossPercent)",
            "return Math.Max(proportionalLoss, MinimumMaxHpLoss)",
            "public static bool CanPayMaxHpCost(int currentMaxHp)",
            "return currentMaxHp > CalculateMaxHpLoss(currentMaxHp)",
            "static string IPatchMethod.PatchId => \"distinguished-cape-event-option\"",
            "CreateVakuuSecondPoolReplacement",
            "vakuu.AllPossibleOptions",
            "option.Relic is PreservedFog or SereTalon",
            "return options.ToArray()",
            "vakuu.Rng.NextItem(candidates)",
            "CreateLockedCapeOption",
            "DISTINGUISHED_CAPE.unpayableOption",
            "await CreatureCmd.SetCurrentHp(creature, newMaxHp)",
            "await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), creature, maxHpLoss, isFromCard: false)",
            "CreateCard<Apparition>");

        Assert.DoesNotContain("currentMaxHp - 1", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ThatWillKillPlayerIf(_ => false)", source, StringComparison.Ordinal);
        Assert.DoesNotContain(".Where(option => option.Relic is not DistinguishedCape)", source, StringComparison.Ordinal);
        Assert.DoesNotContain("AccessTools.Field(typeof(AncientEventModel)", source, StringComparison.Ordinal);

        var distinguishedCapeSection = SliceBetween(source, "internal sealed class DistinguishedCapePickupPatch", "internal sealed class PreservedFogPatch : IPatchMethod");
        Assert.DoesNotContain("CreatureCmd.Damage", distinguishedCapeSection, StringComparison.Ordinal);
        Assert.DoesNotContain("ValueProp", distinguishedCapeSection, StringComparison.Ordinal);

        Assert.Contains("| Distinguished Cape |", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("max HP loss is not damage", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("cannot be selected when current Max HP is not greater than the v4.3 cost", manualMatrix, StringComparison.Ordinal);
    }

    [Fact]
    public void DistinguishedCapeUnaffordableVakuuPathPreservesVisibleOptionCount()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "DistinguishedCapePatches.cs");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");
        var replacementBranch = SliceBetween(
            source,
            "private static IReadOnlyList<MegaCrit.Sts2.Core.Events.EventOption> Postfix(",
            "private static MegaCrit.Sts2.Core.Events.EventOption? CreateVakuuSecondPoolReplacement");
        var replacementFactory = SliceBetween(
            source,
            "private static MegaCrit.Sts2.Core.Events.EventOption? CreateVakuuSecondPoolReplacement",
            "private static bool IsPayableVakuuSecondPoolOption");
        var payablePredicate = SliceBetween(
            source,
            "private static bool IsPayableVakuuSecondPoolOption",
            "private static MegaCrit.Sts2.Core.Events.EventOption CreateLockedCapeOption");
        var lockedFallback = SliceBetween(
            source,
            "private static MegaCrit.Sts2.Core.Events.EventOption CreateLockedCapeOption",
            "class DistinguishedCapePickupPatch : IPatchMethod");

        AssertSourceContains(
            replacementBranch,
            "var options = __result.ToList();",
            "var capeIndex = options.FindIndex(option => option.Relic is DistinguishedCape);",
            "var replacement = CreateVakuuSecondPoolReplacement(__instance, options);",
            "options[capeIndex] = replacement;",
            "return options.ToArray();",
            "options[capeIndex] = CreateLockedCapeOption(__instance, options[capeIndex], owner.Creature.MaxHp);",
            "return options.ToArray();");

        Assert.Equal(2, Regex.Matches(replacementBranch, @"options\[capeIndex\]\s*=").Count);
        Assert.Equal(2, Regex.Matches(replacementBranch, @"return\s+options\.ToArray\(\);").Count);
        foreach (var countChangingApi in new[] { ".Add(", ".AddRange(", ".Insert(", ".InsertRange(", ".Clear(", ".Remove(", ".RemoveAt(", ".RemoveAll(", ".Where(", ".Take(", ".Skip(" })
        {
            Assert.DoesNotContain(countChangingApi, replacementBranch, StringComparison.Ordinal);
        }

        AssertSourceContains(
            replacementFactory,
            ".Select(option => option.TextKey)",
            ".ToHashSet(StringComparer.Ordinal)",
            "vakuu.AllPossibleOptions",
            ".Where(IsPayableVakuuSecondPoolOption)",
            ".Where(option => !currentKeys.Contains(option.TextKey))",
            "vakuu.Rng.NextItem(candidates)");

        AssertSourceContains(
            payablePredicate,
            "return option.Relic is PreservedFog or SereTalon;");

        AssertSourceContains(
            lockedFallback,
            "DISTINGUISHED_CAPE.unpayableOption",
            "description.Add(\"Cost\", (decimal)DistinguishedCapePickupPatch.CalculateMaxHpLoss(currentMaxHp))",
            "new MegaCrit.Sts2.Core.Events.EventOption(",
            "null,",
            "originalOption.Title",
            "originalOption.TextKey",
            "originalOption.HoverTips",
            "lockedOption.WithRelic(originalOption.Relic)");

        Assert.Contains("Vakuu must still show three normal reward options", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("localized locked Cape only as a defensive fallback", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("low-Max-HP Vakuu still shows three normal choices", manualMatrix, StringComparison.Ordinal);
    }

    private static int DistinguishedCapeLossForTest(int currentMaxHp)
    {
        var proportionalLoss = (int)Math.Ceiling(currentMaxHp * 0.30m);
        return Math.Max(proportionalLoss, 18);
    }

    private static bool CanPayDistinguishedCapeCostForTest(int currentMaxHp)
    {
        return currentMaxHp > DistinguishedCapeLossForTest(currentMaxHp);
    }
}
