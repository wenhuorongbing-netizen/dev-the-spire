using System;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AscensionV2MilestoneGuardTests
{
    [Fact]
    public void Milestones2To4GuardFiremarksForgeTokenAndFission()
    {
        var metadata = ReadRepoText("EZMicroBalanceCode", "Ascension", "Map", "AscensionNodeMetadata.cs");
        var mapService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Map");
        var combatService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Combat");
        var forgeService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var forgeRelic = ReadRepoText("EZMicroBalanceCode", "Ascension", "Relics", "ForgeTokenRelic.cs");
        var rewardService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");

        AssertSourceContains(metadata, "Might", "Giant", "ForgeArmor", "ConstantHeal");
        AssertSourceContains(
            mapService,
            "ActOneFiremarkedEliteTargetCount = 2",
            "LaterActFiremarkedEliteTargetCount = 3",
            "MinimumFiremarkedEliteFallbackCount = 2",
            "HasHardFiremarkPlacementConflict",
            "IsAfterActOneFirstRestSite",
            "HasPathAvoiding(map.StartingMapPoint, map.BossMapPoint, point)",
            "KeepsFiremarksOptional(start, boss, selected, point)");

        AssertSourceContains(
            combatService,
            "FindFiremarkHost(combatState)",
            "tracker.FiremarkHost = host",
            "GetMightFiremarkStrength(combatState)",
            "GetMightOverflowStrength(combatState)",
            "GetGiantFiremarkMaxHpPercent(combatState)",
            "GetGiantOverflowDamage(combatState)",
            "GetForgeArmorBlock(combatState)",
            "GetForgeArmorOverflowBlock(combatState)",
            "GetConstantHealAmount(combatState)",
            "GetConstantHealOverflowHeal(combatState)",
            "AddFiremarkHeat(host, tracker)",
            "ApplyMightOverflow(combatState, tracker)",
            "TrackMoltenCoreDamage(combatState, tracker, host",
            "ApplyGiantOverflowDamage(combatState, tracker, host)",
            "ResolveForgeArmorShatter(tracker)",
            "ApplyForgeArmorOverflow(combatState, tracker)",
            "ResolveConstantHeal(combatState, tracker)",
            "ApplyConstantHealOverflow(combatState, tracker)",
            "PowerCmd.Apply<MightMarkFiremarkPower>",
            "PowerCmd.Apply<GiantMarkFiremarkPower>",
            "PowerCmd.Apply<ForgeArmorMarkFiremarkPower>",
            "PowerCmd.Apply<ConstantHealMarkFiremarkPower>",
            "PowerCmd.Apply<FiremarkMightOverflowPower>",
            "FiremarkOverflowCandidates(combatState, tracker)");

        AssertSourceContains(
            rewardService,
            "FiremarkedEliteRewardTargetOptionCount = 4",
            "var duplicateTokenReward = ForgeTokenService.HasToken(player)",
            "Where(card => !duplicateTokenReward || card.IsUpgradable)",
            "CardCmd.Upgrade(extraCard)",
            "NormalFissionChancePercent = 10",
            "BannerFissionChancePercent = 15",
            "FiremarkedEliteFissionChancePercent = 20",
            "BossFissionChancePercent = 5",
            "cardRewardOptions.Any(option => option.Card.Enchantment is FissionEnchantment)",
            "card.Type is CardType.Attack or CardType.Skill",
            "IsFissionEligibleRarity(card.Rarity)",
            "!card.EnergyCost.CostsX",
            "!card.HasStarCostX",
            "card.EnergyCost.Canonical > 0",
            "card.EnergyCost.GetWithModifiers(CostModifiers.None) > 0",
            "!card.Keywords.Contains(CardKeyword.Exhaust)",
            "!card.ExhaustOnNextPlay",
            "card.Enchantment == null");
        Assert.Contains("var modifiedCard = player.RunState.CloneCard(candidate.Card)", rewardService, StringComparison.Ordinal);
        Assert.DoesNotContain("Where(option => !option.HasBeenModified)", rewardService, StringComparison.Ordinal);

        AssertSourceContains(
            forgeService,
            "DuplicateTokenGoldAmount",
            "internal static bool HasToken(Player player)",
            "AscensionSavedStateFields.ForgeTokenHeld[player]",
            "SpecialRestSiteActionPayoutEnabled = false",
            "SpecialRestSiteHealAmount = 5m",
            "await RelicCmd.Obtain<ForgeTokenRelic>(player)",
            "await RelicCmd.Remove(token)",
            "await PlayerCmd.GainGold",
            "CardCmd.Upgrade",
            "player.Relics.OfType<ForgeTokenRelic>().ToList()");
        AssertSourceContains(forgeRelic, "ShowCounter => true", "DisplayAmount => 1", "Max [blue]1[/blue].");
    }
}
