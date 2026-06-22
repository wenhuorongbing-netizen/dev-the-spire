using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
{
    [Fact]
    public void PrismaticGemRerollStateIsScreenScopedCounterSafeAndReplacesAllSlots()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var apiDiscovery = ReadRepoText("docs", "features", "ancients-rework-v4", "api-discovery.md");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");

        Assert.Equal("Gain 1 Energy. Every second standard card reward contains only off-color cards.", relics["PRISMATIC_GEM.description"]);

        AssertSourceContains(
            source,
            "[ThreadStatic]",
            "Stack<CardReward>",
            "HarmonyFinalizer",
            "PrismaticGemRewardScreenContextPatch.CurrentReward",
            "HarmonyPrefix",
            "player.Relics.OfType<PrismaticGem>().FirstOrDefault(relic => !relic.IsMelted)",
            "foreach (var listener in runState.IterateHookListeners(null))",
            "listener.TryModifyCardRewardOptions(player, cardRewardOptions, creationOptions)",
            "if (listenerModified)",
            "modifiers.Add(listener)",
            "Prismatic replacement sits between Core's early and late reward hooks",
            "TryReplaceNormalRewardScreen(prismaticGem, player, cardRewardOptions, creationOptions)",
            "listener.TryModifyCardRewardOptionsLate(player, cardRewardOptions, creationOptions)",
            "CleanupSupersededPrismaticReplacements(cardRewardOptions)",
            "ConditionalWeakTable<CardReward, RewardScreenState>",
            "public CardModel? PrismaticReplacement { get; set; }",
            "var madeTriggerDecision = !screenState.HasTriggerDecision",
            "if (madeTriggerDecision)",
            "screenState.CounterAtDecision = AncientSavedStateFields.PrismaticGemNormalRewardCounter[prismaticGem] + 1",
            "AncientSavedStateFields.PrismaticGemNormalRewardCounter[prismaticGem] = screenState.CounterAtDecision",
            "screenState.ShouldReplaceAllSlots = screenState.CounterAtDecision % 2 == 0",
            "else if (!isNormalCardReward)",
            "creationOptions.Source == CardCreationSource.Encounter",
            "creationOptions.RarityOdds == CardRarityOddsType.RegularEncounter",
            "creationOptions.CustomCardPool == null",
            "creationOptions.CardPoolFilter == null",
            "!creationOptions.CardPools.All(pool => pool.IsColorless)",
            "ReplaceAllRewardSlots",
            "for (var slotIndex = 0; slotIndex < cardRewardOptions.Count; slotIndex++)",
            "RestoreCounterAfterFailedReplacement(prismaticGem, screenState)",
            "reward.ModifyCard(replacement, prismaticGem)",
            "TrackPrismaticReplacement(reward, replacement)",
            "RewardResultHints.GetValue(reward, _ => new RewardResultHintState()).PrismaticReplacement = replacement",
            "if (!RewardResultHints.TryGetValue(reward, out var hintState)",
            "ReferenceEquals(reward.Card, hintState.PrismaticReplacement)",
            "AncientCardHelpers.RemoveUnpiledRunCard(hintState.PrismaticReplacement)",
            "excludedIds.Add(replacement.Id)",
            "player.RunState.RemoveCard(originalCard)",
            "RemoveUnpiledReplacements(replacements)",
            "AncientCardHelpers.RemoveUnpiledRunCard(replacement)",
            ".Where(card => type == null || card.Type == type)",
            "GetOffColorRewardPool(player, originalCard.Rarity, originalCard.Type, excludedIds)",
            "GetOffColorRewardPool(player, null, originalCard.Type, excludedIds)",
            "GetOffColorRewardPool(player, originalCard.Rarity, null, excludedIds)",
            "GetOffColorRewardPool(player, null, null, excludedIds)");
        AssertBefore(
            source,
            "listener.TryModifyCardRewardOptions(player, cardRewardOptions, creationOptions)",
            "TryReplaceNormalRewardScreen(prismaticGem, player, cardRewardOptions, creationOptions)");
        AssertBefore(
            source,
            "TryReplaceNormalRewardScreen(prismaticGem, player, cardRewardOptions, creationOptions)",
            "listener.TryModifyCardRewardOptionsLate(player, cardRewardOptions, creationOptions)");
        AssertBefore(
            source,
            "listener.TryModifyCardRewardOptionsLate(player, cardRewardOptions, creationOptions)",
            "CleanupSupersededPrismaticReplacements(cardRewardOptions)");

        AssertSourceContains(
            source,
            "[HarmonyPatch(typeof(RelicModel), \"get_HoverTips\")]",
            "PRISMATIC_GEM.countHint.title",
            "PRISMATIC_GEM.countHint.nextNormal",
            "PRISMATIC_GEM.countHint.nextOffColor",
            "NCardRewardSelectionScreen",
            "internal sealed partial class PrismaticGemRewardScreenHintPatch : IPatchMethod",
            "IPatchMethod.PatchId => \"prismatic-gem-reward-screen-hint\"",
            "new ModPatchTarget(",
            "nameof(NCardRewardSelectionScreen.RefreshOptions)",
            "typeof(IReadOnlyList<CardRewardAlternative>)",
            "PRISMATIC_GEM.rewardScreenHint",
            "BannerNodePath = \"UI/Banner\"",
            "TryGetCompatibleBannerField",
            "typeof(MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner).IsAssignableFrom(BannerField.FieldType)",
            "TryApplyBannerNodeHint",
            "GetNodeOrNull<MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner>(BannerNodePath)",
            "InfoOnce(",
            "WarnOnce(",
            "visible all-off-color cards and the Prismatic Gem relic hover count remain available");

        Assert.DoesNotContain("ShouldReplaceRightmostSlot", source, StringComparison.Ordinal);
        Assert.DoesNotContain("var slotIndex = cardRewardOptions.Count - 1", source, StringComparison.Ordinal);
        Assert.DoesNotContain("[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.TryModifyCardRewardOptions))]", source, StringComparison.Ordinal);

        AssertSourceContains(
            apiDiscovery,
            "CardReward.Reroll()",
            "Rerolls reuse the same `CardReward` state",
            "eligible normal rewards increment the saved counter once",
            "ineligible rewards store a non-trigger decision and do not increment",
            "Trigger screens regenerate all-slot off-color replacements",
            "reward-screen banner hint");
    }

    [Fact]
    public void PrismaticGemRewardScreenHintHasGuardedBannerFallbackDiagnostics()
    {
        var hintPatch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PrismaticGemRewardScreenHintPatch.cs");
        var sharedBanner = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PrismaticGemRewardScreenHintBanner.cs");
        var fieldSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PrismaticGemRewardScreenHintBanner.Field.cs");
        var nodeSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PrismaticGemRewardScreenHintBanner.Node.cs");
        var source = string.Join(Environment.NewLine, hintPatch, sharedBanner, fieldSource, nodeSource);
        var applyHint = SliceFrom(hintPatch, "private static void ApplyRewardScreenHint");
        var fieldFallback = SliceBetween(
            fieldSource,
            "private static bool TryApplyBannerFieldHint(",
            "private static bool TryGetCompatibleBannerField(");
        var nodeFallback = SliceBetween(
            nodeSource,
            "private static bool TryApplyBannerNodeHint(",
            "private static void ConfirmBannerNodeHintAfterFieldSuccess(");
        var testPlan = ReadRepoText("docs", "test-plan.md");
        var manualChecklist = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-test-checklist.md");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            source,
            "private static readonly System.Reflection.FieldInfo? BannerField",
            "if (TryApplyBannerFieldHint(screen, hintText))",
            "if (TryApplyBannerNodeHint(screen, hintText))",
            "TryGetCompatibleBannerField(out var bannerField, out var reason)",
            "if (BannerField == null)",
            "typeof(MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner).IsAssignableFrom(BannerField.FieldType)",
            "bannerField.GetValue(screen)",
            "screen.GetNodeOrNull<MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner>(BannerNodePath)",
            "private _banner field unavailable",
            "private _banner field resolved but did not contain a banner instance",
            "private _banner access failed",
            "fallback applied through {BannerNodePath} node lookup",
            "fallback unavailable",
            "reward-screen hint unavailable",
            "visible all-off-color cards and the Prismatic Gem relic hover count remain available");

        Assert.Equal(Regex.Matches(fieldFallback, @"return false;").Count, Regex.Matches(fieldFallback, @"WarnOnce\(").Count);
        Assert.Equal(Regex.Matches(nodeFallback, @"return false;").Count, Regex.Matches(nodeFallback, @"WarnOnce\(").Count);
        Assert.Contains("WarnOnce(", applyHint, StringComparison.Ordinal);
        Assert.Contains("InfoOnce(", fieldFallback, StringComparison.Ordinal);
        Assert.Contains("catch (Exception exception)", fieldFallback, StringComparison.Ordinal);
        Assert.Contains("catch (Exception exception)", nodeFallback, StringComparison.Ordinal);
        Assert.DoesNotContain("BannerField!.GetValue", source, StringComparison.Ordinal);
        Assert.DoesNotContain("BannerField.GetValue(screen)", source, StringComparison.Ordinal);
        Assert.DoesNotContain("catch {", source, StringComparison.Ordinal);

        Assert.Contains("banner fallback diagnostics", testPlan, StringComparison.Ordinal);
        Assert.Contains("manual-test coverage", testPlan, StringComparison.Ordinal);
        Assert.Contains("the reward-screen hint logs a fallback if the banner cannot be updated", testPlan, StringComparison.Ordinal);
        Assert.Contains("If the trigger reward banner is not visible", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("PrismaticGem reward-screen hint fallback", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("PrismaticGem reward-screen hint fallback", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("relic hover count plus every visible reward card being off-color", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("visible all-off-color cards and relic hover count remain the available confirmation surfaces", manualMatrix, StringComparison.Ordinal);
    }
}
