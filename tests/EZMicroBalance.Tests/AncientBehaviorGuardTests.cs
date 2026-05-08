using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class AncientBehaviorGuardTests
{
    private static readonly string[] ImplementedLocalizationTables =
    [
        "cards.json",
        "relics.json",
        "rest_site_ui.json"
    ];

    private static readonly string[] RequiredCardLocalizationKeys =
    [
        "BRIGHTEST_FLAME.title",
        "BRIGHTEST_FLAME.description",
        "DEBT.title",
        "DEBT.description",
        "ENTHRALLED.title",
        "ENTHRALLED.description",
        "FOLLY.title",
        "FOLLY.description"
    ];

    private static readonly string[] RequiredRelicLocalizationKeys =
    [
        "BEAUTIFUL_BRACELET.description",
        "BLACK_STAR.description",
        "BLOOD_SOAKED_ROSE.description",
        "BRILLIANT_SCARF.description",
        "CHOICES_PARADOX.description",
        "CHOICES_PARADOX.selectionScreenPrompt",
        "CLAWS.description",
        "CROSSBOW.description",
        "DISTINGUISHED_CAPE.description",
        "DISTINGUISHED_CAPE.eventDescription",
        "DISTINGUISHED_CAPE.unpayableOption",
        "ECTOPLASM.description",
        "FIDDLE.description",
        "IRON_CLUB.description",
        "JEWELED_MASK.description",
        "JEWELED_MASK.ezSelectionScreenPrompt",
        "JEWELRY_BOX.description",
        "MEAT_CLEAVER.description",
        "MUSIC_BOX.description",
        "PAELS_HORN.description",
        "PAELS_TOOTH.description",
        "PRESERVED_FOG.description",
        "PRISMATIC_GEM.description",
        "PRISMATIC_GEM.countHint.title",
        "PRISMATIC_GEM.countHint.nextNormal",
        "PRISMATIC_GEM.countHint.nextOffColor",
        "PRISMATIC_GEM.rewardScreenHint",
        "SEAL_OF_GOLD.description",
        "SOZU.description",
        "TOASTY_MITTENS.description",
        "VELVET_CHOKER.description",
        "WAR_HAMMER.description",
        "WHISPERING_EARRING.description"
    ];

    private static readonly string[] RequiredManualMatrixRows =
    [
        "Pael's Horn",
        "Black Star",
        "War Hammer",
        "Jewelry Box",
        "Preserved Fog / Folly",
        "Claws",
        "Choices Paradox",
        "Jeweled Mask",
        "Prismatic Gem",
        "Distinguished Cape",
        "Velvet Choker",
        "Pael's Tooth",
        "Sovereign Blade / Forge",
        "Seal of Gold / Debt",
        "Sozu",
        "Ectoplasm",
        "Fiddle",
        "Iron Club",
        "Brilliant Scarf",
        "Beautiful Bracelet",
        "Music Box",
        "Crossbow",
        "Toasty Mittens",
        "Whispering Earring",
        "Quality Flame / Brightest Flame",
        "Meat Cleaver",
        "Blood-Soaked Rose / Enthralled"
    ];

    [Fact]
    public void V43AdjustmentPlanIsArchivedAndSupersedesV42AsCurrentTruth()
    {
        var archivedV43Plan = ReadRepoText("docs", "features", "ancients-rework-v4", "sts2_ancients_rework_v4_3_adjustment_plan.md");
        var archivedPlan = ReadRepoText("docs", "features", "ancients-rework-v4", "sts2_ancients_rework_v4_2_next_plan.md");
        var completionAudit = ReadRepoText("docs", "features", "ancients-rework-v4", "completion-audit.md");

        Assert.Contains("v4.3", archivedV43Plan, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("卓越斗篷", archivedV43Plan, StringComparison.Ordinal);
        Assert.Contains("棱彩宝石", archivedV43Plan, StringComparison.Ordinal);
        Assert.Contains("sts2_ancients_rework_v4_3_adjustment_plan.md", completionAudit, StringComparison.Ordinal);
        Assert.Contains("v4.3 is current", completionAudit, StringComparison.Ordinal);

        Assert.Contains("v4.2", archivedPlan, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("天鹅绒项圈", archivedPlan, StringComparison.Ordinal);
        Assert.Contains("卓越斗篷", archivedPlan, StringComparison.Ordinal);
        Assert.Contains("棱彩宝石", archivedPlan, StringComparison.Ordinal);
        Assert.Contains("v4.2 is historical", completionAudit, StringComparison.Ordinal);
    }

    [Fact]
    public void ImplementedLocalizationTablesHaveValueAndPlaceholderParity()
    {
        foreach (var table in ImplementedLocalizationTables)
        {
            var english = JsonStringMap("EZMicroBalance", "localization", "eng", table);
            var simplifiedChinese = JsonStringMap("EZMicroBalance", "localization", "zhs", table);

            Assert.Equal(english.Keys, simplifiedChinese.Keys);
            foreach (var key in english.Keys)
            {
                Assert.False(string.IsNullOrWhiteSpace(english[key]), $"{table}:{key} has empty English text.");
                Assert.False(string.IsNullOrWhiteSpace(simplifiedChinese[key]), $"{table}:{key} has empty zhs text.");
                Assert.Equal(Placeholders(english[key]), Placeholders(simplifiedChinese[key]));
            }
        }

        var cards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var restSite = JsonStringMap("EZMicroBalance", "localization", "eng", "rest_site_ui.json");

        foreach (var key in RequiredCardLocalizationKeys)
        {
            Assert.Contains(key, cards.Keys);
        }

        foreach (var key in RequiredRelicLocalizationKeys)
        {
            Assert.Contains(key, relics.Keys);
        }

        Assert.Contains("OPTION_COOK.ezDescription", restSite.Keys);
        Assert.Contains("OPTION_COOK.ezDescriptionDisabled", restSite.Keys);
    }

    [ReleaseArtifactFact]
    public void PrivateBetaZipContainsOnlyInstallableActiveModFiles()
    {
        var packagePath = RepoPath("publish", "EZMicroBalance-v0.1.0-private-beta.0.zip");
        Assert.True(File.Exists(packagePath), $"Missing private beta package: {packagePath}");

        using var archive = ZipFile.OpenRead(packagePath);
        var entries = archive.Entries
            .Where(entry => !string.IsNullOrEmpty(entry.Name))
            .Select(entry => entry.FullName.Replace('\\', '/'))
            .OrderBy(entry => entry, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "EZMicroBalance/EZMicroBalance.dll",
                "EZMicroBalance/EZMicroBalance.json",
                "EZMicroBalance/EZMicroBalance.pck",
                "EZMicroBalance/README_INSTALL.txt"
            ],
            entries);

        using var manifest = JsonDocument.Parse(ReadZipText(archive, "EZMicroBalance/EZMicroBalance.json"));
        Assert.Equal("EZMicroBalance", manifest.RootElement.GetProperty("id").GetString());
        Assert.Contains(
            manifest.RootElement.GetProperty("dependencies").EnumerateArray(),
            dependency => dependency.GetString() == "BaseLib");

        var readme = ReadZipText(archive, "EZMicroBalance/README_INSTALL.txt");
        Assert.Contains("Manifest id: EZMicroBalance", readme, StringComparison.Ordinal);
        Assert.Contains("BaseLib", readme, StringComparison.Ordinal);
        Assert.Contains("EzDailyContent disabled or absent", readme, StringComparison.Ordinal);
        Assert.Contains("Current controlled --force-steam off smoke passed", readme, StringComparison.Ordinal);
        Assert.Contains("Found 12 SavedSpireFields", readme, StringComparison.Ordinal);
        Assert.Contains("Normal Steam-client Mod Settings verification is still pending", readme, StringComparison.Ordinal);
        Assert.Contains("Live Ancient reward gameplay, save/load, disable-gameplay, and multiplayer checks are still pending", readme, StringComparison.Ordinal);
        Assert.Contains("A11-A20 selection is now default-on in this private-beta multiplayer test candidate", readme, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1", readme, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1", readme, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1 is legacy-compatible and no longer required", readme, StringComparison.Ordinal);
        Assert.Contains("Full live Ascension verification is pending", readme, StringComparison.Ordinal);
        Assert.Contains("Ascension 21-30 and custom-character content are not included", readme, StringComparison.Ordinal);
    }

    [Fact]
    public void HarmonyPatchTargetsAreDeclaredForImplementedAncientSurfaces()
    {
        var allSource = ReadAncientSource();

        AssertSourceContains(
            allSource,
            "[HarmonyPatch(typeof(PaelsHorn), nameof(PaelsHorn.AfterObtained))]",
            "[HarmonyPatch(typeof(RelicModel), nameof(RelicModel.AfterObtained))]",
            "[HarmonyPatch(typeof(RelicCmd), nameof(RelicCmd.Obtain), typeof(RelicModel), typeof(Player), typeof(int))]",
            "[HarmonyPatch(typeof(Claws), nameof(Claws.AfterObtained))]",
            "[HarmonyPatch(typeof(Sozu), nameof(Sozu.ShouldProcurePotion))]",
            "[HarmonyPatch(typeof(Ectoplasm), nameof(Ectoplasm.ShouldGainGold))]",
            "[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.ModifyMaxEnergy))]",
            "[HarmonyPatch(typeof(SealOfGold), nameof(SealOfGold.AfterSideTurnStart))]",
            "[HarmonyPatch(typeof(CardModel), nameof(CardModel.AfterCreated))]",
            "[HarmonyPatch(typeof(CardModel), nameof(CardModel.FromSerializable))]",
            "[HarmonyPatch(typeof(Debt), \"get_CanonicalKeywords\")]",
            "[HarmonyPatch(typeof(Debt), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(Debt), \"get_HasTurnEndInHandEffect\")]",
            "[HarmonyPatch(typeof(Debt), \"OnTurnEndInHand\")]",
            "[HarmonyPatch(typeof(CardModel), \"OnPlay\")]",
            "[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Exhaust))]",
            "[HarmonyPatch(typeof(PrismaticGem), nameof(PrismaticGem.ModifyCardRewardCreationOptions))]",
            "[HarmonyPatch(typeof(CardReward), nameof(CardReward.Populate))]",
            "[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.TryModifyCardRewardOptions))]",
            "[HarmonyPatch(typeof(DistinguishedCape), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(Vakuu), \"GenerateInitialOptions\")]",
            "[HarmonyPatch(typeof(DistinguishedCape), nameof(DistinguishedCape.AfterObtained))]",
            "[HarmonyPatch(typeof(VelvetChoker), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(VelvetChoker), \"get_DisplayAmount\")]",
            "[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.ShouldPlay))]",
            "[HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.GetWithModifiers))]",
            "[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]",
            "[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]",
            "[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterCardPlayed))]",
            "[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.BeforeSideTurnStart))]",
            "[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterRoomEntered))]",
            "[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterCombatEnd))]",
            "[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterObtained))]",
            "[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterCombatEnd))]",
            "[HarmonyPatch(typeof(ForgeCmd), nameof(ForgeCmd.Forge))]",
            "[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterActEntered))]",
            "[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.BeforeSideTurnStart))]",
            "[HarmonyPatch(typeof(Crossbow), nameof(Crossbow.AfterSideTurnStart))]",
            "[HarmonyPatch(typeof(ToastyMittens), nameof(ToastyMittens.BeforeHandDraw))]",
            "[HarmonyPatch(typeof(WhisperingEarring), nameof(WhisperingEarring.AfterAutoPrePlayPhaseEnteredLate))]",
            "[HarmonyPatch(typeof(CardModel), \"get_CanonicalKeywords\")]",
            "[HarmonyPatch(typeof(BrightestFlame), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]",
            "[HarmonyPatch(typeof(CookRestSiteOption), MethodType.Constructor, typeof(Player))]",
            "[HarmonyPatch(typeof(CookRestSiteOption), \"get_Description\")]",
            "[HarmonyPatch(typeof(CookRestSiteOption), nameof(CookRestSiteOption.OnSelect))]",
            "[HarmonyPatch(typeof(JewelryBox), nameof(JewelryBox.AfterObtained))]",
            "[HarmonyPatch(typeof(Apotheosis), \"get_CanonicalKeywords\")]",
            "[HarmonyPatch(typeof(JewelryBox), \"get_ExtraHoverTips\")]",
            "[HarmonyPatch(typeof(RelicModel), \"get_HoverTips\")]",
            "[HarmonyPatch(typeof(RelicModel), \"get_HoverTipsExcludingRelic\")]",
            "[HarmonyPatch(typeof(PreservedFog), nameof(PreservedFog.AfterObtained))]",
            "[HarmonyPatch(typeof(Folly), \"get_CanonicalKeywords\")]",
            "[HarmonyPatch(typeof(ChoicesParadox), nameof(ChoicesParadox.AfterPlayerTurnStart))]",
            "[HarmonyPatch(typeof(JeweledMask), nameof(JeweledMask.BeforeHandDraw))]",
            "[HarmonyPatch(typeof(Fiddle), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(Fiddle), nameof(Fiddle.ModifyHandDrawLate))]",
            "[HarmonyPatch(typeof(Fiddle), nameof(Fiddle.ShouldDraw))]",
            "[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Draw), typeof(PlayerChoiceContext), typeof(decimal), typeof(Player), typeof(bool))]",
            "[HarmonyPatch(typeof(IronClub), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(BrilliantScarf), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(BeautifulBracelet), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(BeautifulBracelet), nameof(BeautifulBracelet.AfterObtained))]",
            "[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.BeforeCardPlayed))]",
            "[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.AfterCardPlayed))]",
            "[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.BeforeSideTurnStart))]",
            "[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.AfterCombatEnd))]",
            "[HarmonyPatch(typeof(CardModel), \"get_CanonicalKeywords\")]",
            "[HarmonyPatch(typeof(BrightestFlame), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]");
    }

    [Fact]
    public void PrismaticGemRerollStateIsScreenScopedCounterSafeAndReplacesAllSlots()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PrismaticGemPatches.cs");
        var apiDiscovery = ReadRepoText("docs", "features", "ancients-rework-v4", "api-discovery.md");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");

        Assert.Equal("Gain 1 Energy. Every second standard card reward contains only off-color cards.", relics["PRISMATIC_GEM.description"]);

        AssertSourceContains(
            source,
            "[ThreadStatic]",
            "Stack<CardReward>",
            "HarmonyFinalizer",
            "PrismaticGemRewardScreenContextPatch.CurrentReward",
            "ConditionalWeakTable<CardReward, RewardScreenState>",
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
            "reward.ModifyCard(replacement, prismaticGem)",
            "RewardResultHints.GetValue(reward, _ => new RewardResultHintState())",
            "excludedIds.Add(replacement.Id)",
            "player.RunState.RemoveCard(originalCard)");

        AssertSourceContains(
            source,
            "[HarmonyPatch(typeof(RelicModel), \"get_HoverTips\")]",
            "PRISMATIC_GEM.countHint.title",
            "PRISMATIC_GEM.countHint.nextNormal",
            "PRISMATIC_GEM.countHint.nextOffColor",
            "NCardRewardSelectionScreen",
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
    public void VelvetChokerSoftLimitCountsOnlyManualFirstFromHandPlaysAndResetsEachTurn()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "VakuRewardPatches.cs");
        var turnSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "TurnOfferAndRestPatches.cs");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        Assert.Equal("Gain 1 Energy. Each turn, the 7th and later cards played from your hand cost 1 more.", relics["VELVET_CHOKER.description"]);

        AssertSourceContains(
            source,
            "new DynamicVar[] { new CardsVar(7), new EnergyVar(1) }",
            "VelvetChokerShouldPlayPatch",
            "__result = true",
            "CardEnergyCost.GetWithModifiers",
            "modifiers.HasFlag(CostModifiers.Global)",
            "__result += VelvetChokerSoftLimitTracker.ExtraEnergyCost",
            "PlayerCombatState.HasEnoughResourcesFor",
            "UnplayableReason.EnergyCostTooHigh",
            "CardModel.SpendResources",
            "CapturedXValue = Math.Max(0, result.Item1 - VelvetChokerSoftLimitTracker.ExtraEnergyCost)",
            "!cardPlay.IsAutoPlay",
            "cardPlay.IsFirstInSeries",
            "!cardPlay.Card.IsClone",
            "cardPlay.Card.Owner == __instance.Owner",
            "card.IsClone",
            "card.Pile?.Type != PileType.Hand",
            "HandPlayedThisTurn(choker) >= FreeHandPlaysPerTurn",
            "BeforeSideTurnStart",
            "side == __instance.Owner.Creature.Side",
            "AfterRoomEntered",
            "AfterCombatEnd");

        var shouldTax = SliceBetween(
            source,
            "public static bool ShouldTax(CardModel card)",
            "private static Player? TryGetOwner");
        var tryGetOwner = SliceBetween(
            source,
            "private static Player? TryGetOwner",
            "public static void Increment");
        AssertSourceContains(
            shouldTax,
            "if (!CombatManager.Instance.IsInProgress",
            "SuppressedCostCards.Contains(card)",
            "card.Pile?.Type != PileType.Hand",
            "var owner = TryGetOwner(card)",
            "owner?.GetRelic<VelvetChoker>()");
        Assert.True(
            shouldTax.IndexOf("CombatManager.Instance.IsInProgress", StringComparison.Ordinal) <
            shouldTax.IndexOf("TryGetOwner(card)", StringComparison.Ordinal),
            "Velvet Choker cost checks must reject non-combat/card-library contexts before reading CardModel.Owner.");
        AssertSourceContains(
            tryGetOwner,
            "return card.Owner",
            "catch (MegaCrit.Sts2.Core.Models.Exceptions.CanonicalModelException)",
            "return null");
        Assert.DoesNotContain("card.Owner?.GetRelic<VelvetChoker>()", shouldTax, StringComparison.Ordinal);

        AssertSourceContains(
            turnSource,
            "VelvetChokerSoftLimitTracker.SuppressCostFor(item.Card, item.Card.CanPlay)",
            "VelvetChokerSoftLimitTracker.SuppressCostFor(item.Card, () => AncientCardHelpers.EffectiveCost(item.Card))",
            "VelvetChokerSoftLimitTracker.SuppressCostFor(card, card.SpendResources)");

        Assert.Contains("| Velvet Choker |", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("copied, autoplayed, or repeated plays do not advance the counter", manualMatrix, StringComparison.Ordinal);
    }

    [Fact]
    public void DistinguishedCapeUsesV43MaxHpMathAndCannotBeSelectedWhenUnableToPay()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "VakuRewardPatches.cs");
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
            "ReplaceUnaffordableCapeWithPayableVakuuOption",
            "CreateVakuuSecondPoolReplacement",
            "vakuu.AllPossibleOptions",
            "option.Relic is PreservedFog or SereTalon",
            "__result = options.ToArray()",
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

        var distinguishedCapeSection = SliceBetween(source, "internal static class DistinguishedCapePickupPatch", "[HarmonyPatch(typeof(PreservedFog)");
        Assert.DoesNotContain("CreatureCmd.Damage", distinguishedCapeSection, StringComparison.Ordinal);
        Assert.DoesNotContain("ValueProp", distinguishedCapeSection, StringComparison.Ordinal);

        Assert.Contains("| Distinguished Cape |", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("max HP loss is not damage", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("cannot be selected when current Max HP is not greater than the v4.3 cost", manualMatrix, StringComparison.Ordinal);
    }

    [Fact]
    public void DistinguishedCapeUnaffordableVakuuPathPreservesVisibleOptionCount()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "VakuRewardPatches.cs");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");
        var replacementBranch = SliceBetween(
            source,
            "private static void ReplaceUnaffordableCapeWithPayableVakuuOption",
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
            "[HarmonyPatch(typeof(DistinguishedCape)");

        AssertSourceContains(
            replacementBranch,
            "var options = __result.ToList();",
            "var capeIndex = options.FindIndex(option => option.Relic is DistinguishedCape);",
            "var replacement = CreateVakuuSecondPoolReplacement(__instance, options);",
            "options[capeIndex] = replacement;",
            "__result = options.ToArray();",
            "options[capeIndex] = CreateLockedCapeOption(__instance, options[capeIndex], owner.Creature.MaxHp);",
            "__result = options.ToArray();");

        Assert.Equal(2, Regex.Matches(replacementBranch, @"options\[capeIndex\]\s*=").Count);
        Assert.Equal(2, Regex.Matches(replacementBranch, @"__result\s*=\s*options\.ToArray\(\);").Count);
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

    [Fact]
    public void PrismaticGemRewardScreenHintHasGuardedBannerFallbackDiagnostics()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PrismaticGemPatches.cs");
        var hintPatch = SliceFrom(source, "internal static class PrismaticGemRewardScreenHintPatch");
        var applyHint = SliceBetween(
            hintPatch,
            "private static void ApplyRewardScreenHint",
            "private static bool TryApplyBannerFieldHint");
        var fieldFallback = SliceBetween(
            hintPatch,
            "private static bool TryApplyBannerFieldHint(",
            "private static bool TryApplyBannerNodeHint(");
        var nodeFallback = SliceBetween(
            hintPatch,
            "private static bool TryApplyBannerNodeHint(",
            "private static bool TryGetCompatibleBannerField");
        var testPlan = ReadRepoText("docs", "test-plan.md");
        var manualChecklist = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-test-checklist.md");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            hintPatch,
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
        Assert.DoesNotContain("BannerField!.GetValue", hintPatch, StringComparison.Ordinal);
        Assert.DoesNotContain("BannerField.GetValue(screen)", hintPatch, StringComparison.Ordinal);
        Assert.DoesNotContain("catch {", hintPatch, StringComparison.Ordinal);

        Assert.Contains("banner fallback diagnostics", testPlan, StringComparison.Ordinal);
        Assert.Contains("manual-test coverage", testPlan, StringComparison.Ordinal);
        Assert.Contains("the reward-screen hint logs a fallback if the banner cannot be updated", testPlan, StringComparison.Ordinal);
        Assert.Contains("If the trigger reward banner is not visible", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("PrismaticGem reward-screen hint fallback", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("PrismaticGem reward-screen hint fallback", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("relic hover count plus every visible reward card being off-color", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("visible all-off-color cards and relic hover count remain the available confirmation surfaces", manualMatrix, StringComparison.Ordinal);
    }

    [Fact]
    public void ReviewFindingFallbackTextIsLocalizedAndSpecific()
    {
        var englishRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var simplifiedChineseRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        Assert.Equal("Max HP too low to pay this cost ({Cost}).", englishRelics["DISTINGUISHED_CAPE.unpayableOption"]);
        Assert.Equal("Prismatic reward: only off-color cards this time.", englishRelics["PRISMATIC_GEM.rewardScreenHint"]);
        Assert.Equal(["{Cost}"], Placeholders(englishRelics["DISTINGUISHED_CAPE.unpayableOption"]));
        Assert.Equal(["{Cost}"], Placeholders(simplifiedChineseRelics["DISTINGUISHED_CAPE.unpayableOption"]));

        AssertNoRawEnglishInZhsFallback(simplifiedChineseRelics["DISTINGUISHED_CAPE.unpayableOption"]);
        AssertNoRawEnglishInZhsFallback(simplifiedChineseRelics["PRISMATIC_GEM.rewardScreenHint"]);
    }

    [Fact]
    public void SimplifiedChinesePlayerFacingNumbersHaveNoSpacesAroundDigits()
    {
        var zhsRoot = RepoPath("EZMicroBalance", "localization", "zhs");
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "JeweledMaskFreePower.cs");
        var activeCode = string.Join(
            Environment.NewLine,
            Directory.GetFiles(RepoPath("EZMicroBalanceCode"), "*.cs", SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => File.ReadAllText(path, Encoding.UTF8)));
        var failures = new List<string>();

        foreach (var file in Directory.GetFiles(zhsRoot, "*.json", SearchOption.AllDirectories))
        {
            using var document = JsonDocument.Parse(File.ReadAllText(file, Encoding.UTF8));
            foreach (var (key, value) in JsonStringValues(document.RootElement))
            {
                var visibleValue = Regex.Replace(value, @"\{[^{}]*\}", string.Empty, RegexOptions.CultureInvariant);
                if (Regex.IsMatch(visibleValue, @"[\u3400-\u9fff][ \t]+[+\-]?\d|[+\-]?\d[ \t]+[\u3400-\u9fff]", RegexOptions.CultureInvariant))
                {
                    failures.Add($"{Path.GetRelativePath(zhsRoot, file)}:{key} has spaced numeric text in `{value}`");
                }
            }
        }

        Assert.DoesNotContain("设为 0", source, StringComparison.Ordinal);
        Assert.DoesNotContain("费用为 0", source, StringComparison.Ordinal);
        Assert.Contains("设为0", source, StringComparison.Ordinal);
        Assert.Contains("费用为0", source, StringComparison.Ordinal);
        Assert.DoesNotContain("耗能降低 1", activeCode, StringComparison.Ordinal);
        Assert.Contains("[gold]耗能[/gold]降低[blue]1[/blue]", activeCode, StringComparison.Ordinal);
        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void ManualVerificationMatrixUsesUncorruptedSimplifiedChineseExpectedText()
    {
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var zhsCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");
        var zhsRestSite = JsonStringMap("EZMicroBalance", "localization", "zhs", "rest_site_ui.json");
        var jeweledMaskFreePowerSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "JeweledMaskFreePower.cs");
        var zhsLocalizationText = string.Join(
            Environment.NewLine,
            zhsRelics.Values.Concat(zhsCards.Values).Concat(zhsRestSite.Values));
        var mojibakeFragments = new[]
        {
            "妫卞僵",
            "寮傝壊",
            "鏀炬澗",
            "绁炲寲",
            "璁告効",
            "鎵ц糠",
            "杩呴",
            "鍥烘湁",
            "鎰氳",
            "姘告亽",
            "瀹濈煶",
            "鐏典綋",
            "闈為",
            "棣栭",
            "鍊哄姟",
            "娆犳",
            "淇濈暀",
            "铏氭棤",
            "娑堣",
            "鍔涢噺",
            "鑾峰緱",
            "鐐硅兘",
            "鐐筦",
            "澶卞幓",
            "寮燻"
        };

        Assert.DoesNotContain("\uFFFD", zhsLocalizationText, StringComparison.Ordinal);
        Assert.Contains("迅速2", zhsRelics["BEAUTIFUL_BRACELET.description"], StringComparison.Ordinal);
        Assert.Contains("放松", zhsRelics["PAELS_HORN.description"], StringComparison.Ordinal);
        Assert.Contains("神化", zhsRelics["JEWELRY_BOX.description"], StringComparison.Ordinal);
        Assert.Contains("固有", zhsRelics["JEWELRY_BOX.description"], StringComparison.Ordinal);
        Assert.Contains("许愿", zhsRelics["CLAWS.description"], StringComparison.Ordinal);
        Assert.Contains("异色牌", zhsRelics["PRISMATIC_GEM.description"], StringComparison.Ordinal);
        Assert.Equal("棱彩计数：{Count}/{Cycle}", zhsRelics["PRISMATIC_GEM.countHint.title"]);
        Assert.Equal("棱彩奖励：本次只出现异色牌。", zhsRelics["PRISMATIC_GEM.rewardScreenHint"]);
        Assert.Contains("灵体", zhsRelics["DISTINGUISHED_CAPE.description"], StringComparison.Ordinal);
        Assert.Contains("宝石面具", jeweledMaskFreePowerSource, StringComparison.Ordinal);
        Assert.Contains("这张牌的费用已被宝石面具永久设为0。", jeweledMaskFreePowerSource, StringComparison.Ordinal);
        Assert.Contains("非首领", zhsRelics["PAELS_TOOTH.description"], StringComparison.Ordinal);
        Assert.Contains("首领", zhsRelics["PAELS_TOOTH.description"], StringComparison.Ordinal);
        Assert.Contains("获得1点能量", zhsRelics["PRISMATIC_GEM.description"], StringComparison.Ordinal);
        Assert.Contains("手牌有7张", zhsRelics["FIDDLE.description"], StringComparison.Ordinal);
        Assert.Contains("至少18点", zhsRelics["DISTINGUISHED_CAPE.description"], StringComparison.Ordinal);
        Assert.Equal("债务", zhsCards["DEBT.title"]);
        Assert.Equal("执迷", zhsCards["ENTHRALLED.title"]);
        Assert.Equal("愚行", zhsCards["FOLLY.title"]);
        Assert.Contains("消耗", zhsCards["DEBT.description"], StringComparison.Ordinal);
        Assert.Contains("永恒", zhsCards["ENTHRALLED.description"], StringComparison.Ordinal);
        Assert.Contains("保留", zhsRelics["CHOICES_PARADOX.description"], StringComparison.Ordinal);
        Assert.Contains("虚无", zhsRelics["CROSSBOW.description"], StringComparison.Ordinal);
        Assert.Contains("力量", zhsRelics["TOASTY_MITTENS.description"], StringComparison.Ordinal);

        foreach (var fragment in mojibakeFragments)
        {
            Assert.DoesNotContain(fragment, zhsLocalizationText, StringComparison.Ordinal);
            Assert.DoesNotContain(fragment, manualMatrix, StringComparison.Ordinal);
        }

        AssertSourceContains(
            manualMatrix,
            "`迅速2`, no raw `Swift`",
            "`获得1点能量`, `手牌有7张`, `至少18点`",
            "`棱彩计数：1/2` or `棱彩计数：0/2`",
            "`棱彩奖励：本次只出现异色牌。`",
            "`神化`, no raw `Apotheosis`",
            "`放松` and `放松+`, no raw `Relax`",
            "`许愿` and `许愿+`, no raw `Wish`",
            "`愚行`, no raw `Folly`",
            "`执迷`, no raw `Enthralled`",
            "`债务`, no raw `Debt`",
            "`首领`, no raw `Boss`",
            "`宝石面具` and 0-cost text",
            "`保留`, `虚无`, `消耗`, `固有`, `永恒`, `力量`");
    }

    [Fact]
    public void CurrentAncientDocsDoNotPresentSupersededV42BehaviorAsCurrent()
    {
        var currentDocs = string.Join(
            Environment.NewLine,
            [
                ReadRepoText("README.md"),
                ReadRepoText("docs", "test-plan.md"),
                ReadRepoText("docs", "release-checklist.md"),
                ReadRepoText("docs", "features", "ancients-rework-v4", "source-design.md"),
                ReadRepoText("docs", "features", "ancients-rework-v4", "api-discovery.md"),
                ReadRepoText("docs", "features", "ancients-rework-v4", "implementation-plan.md"),
                ReadRepoText("docs", "features", "ancients-rework-v4", "high-risk-review.md"),
                ReadRepoText("docs", "features", "ancients-rework-v4", "manual-test-checklist.md"),
                ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md"),
                ReadRepoText("docs", "features", "ancients-rework-v4", "completion-audit.md"),
                ReadRepoText("docs", "features", "ancients-rework-v4", "localization-validation.md")
            ]);

        Assert.Contains("v4.3 is current", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Every second standard card reward contains only off-color cards", currentDocs, StringComparison.Ordinal);
        Assert.Contains("lose 30% of current Max HP, at least 18", currentDocs, StringComparison.Ordinal);
        Assert.Contains("same-pool Vakuu replacement", currentDocs, StringComparison.Ordinal);
        Assert.Contains("locked `EventOption` fallback", currentDocs, StringComparison.Ordinal);
        Assert.Contains("private `_banner` field type is runtime-guarded", currentDocs, StringComparison.Ordinal);
        Assert.Contains("UI/Banner fallback", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Runtime visual placement still requires manual gameplay verification", currentDocs, StringComparison.Ordinal);
        Assert.Contains("v4.2 rightmost-slot Prismatic Gem is historical only", currentDocs, StringComparison.Ordinal);
        Assert.Contains("v4.2 Distinguished Cape 40% min15 is historical only", currentDocs, StringComparison.Ordinal);

        Assert.DoesNotContain("replaces only the rightmost", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("rightmost reward slot", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("ceil(40% current max HP), at least 15", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("cannot reduce Max HP below 1", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("currentMaxHp - 1", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("excludes the option when current max HP cannot pay", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotMatch(@"(?i)Prismatic Gem[^\r\n.]*banner[^\r\n.]*\b(?:passed|verified)\b", currentDocs);
        Assert.DoesNotMatch(@"(?i)reward-screen banner(?: hint)?[^\r\n.]*\b(?:passed|verified)\b", currentDocs);
    }

    [Fact]
    public void JeweledMaskCustomEnchantmentIsPowerOnlyPersistentAndCombatStartScoped()
    {
        var enchantment = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "JeweledMaskFreePower.cs");
        var pickupSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PickupRewardPatches.cs");
        var combatSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "VakuRewardPatches.cs");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            enchantment,
            "CustomEnchantmentModel, ILocalizationProvider",
            "public override bool HasExtraCardText => true",
            "return cardType == CardType.Power",
            "Card.EnergyCost.SetCustomBaseCost(0)");

        AssertSourceContains(
            pickupSource,
            "card => card.Type == CardType.Power && card.Enchantment == null",
            "DraftGeneratedPowerForJeweledMask(owner)",
            "owner.RunState.RemoveCard(unselected)",
            "await CardPileCmd.Add(selected, PileType.Deck)",
            "CardCmd.Enchant<JeweledMaskFreePower>(selected, 1m)");

        AssertSourceContains(
            combatSource,
            "[HarmonyPatch(typeof(JeweledMask), nameof(JeweledMask.BeforeHandDraw))]",
            "combatState.RoundNumber > 1",
            "AncientCardHelpers.IsJeweledMaskPower",
            "await CardPileCmd.Add(markedPower, PileType.Hand)",
            "marked power already in hand",
            "no marked power in draw pile or hand");

        Assert.Contains("Jeweled Mask free power", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Permanent 0-cost enchantment survives save/load.", manualMatrix, StringComparison.Ordinal);
    }

    [Fact]
    public void JewelryBoxApotheosisMarkerIsScopedToCreatedCardsAndHoverPreviews()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "VakuRewardPatches.cs");

        AssertSourceContains(
            source,
            "CreateNonInnateApotheosis(jewelryBox.Owner)",
            "JewelryBoxApotheosisMarker.Mark(result.cardAdded)",
            "ConditionalWeakTable<CardModel, MarkerState>",
            "AncientSavedStateFields.JewelryBoxNonInnateApotheosis[card] = true",
            "if (card is not Apotheosis)",
            "AncientCardHelpers.RemoveKeywords(card, CardKeyword.Innate)",
            "AncientSavedStateFields.JewelryBoxNonInnateApotheosis[card]",
            "[HarmonyPatch(typeof(Apotheosis), \"get_CanonicalKeywords\")]",
            "JewelryBoxApotheosisMarker.IsMarked(__instance)",
            "keyword => keyword != CardKeyword.Innate",
            "CreateNonInnateApotheosisHoverTips",
            "[HarmonyPatch(typeof(JewelryBox), \"get_ExtraHoverTips\")]",
            "[HarmonyPatch(typeof(RelicModel), \"get_HoverTips\")]",
            "[HarmonyPatch(typeof(RelicModel), \"get_HoverTipsExcludingRelic\")]");
    }

    [Fact]
    public void PaelsToothSavedCounterAndStoredCardReturnAreGuarded()
    {
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PaelsToothAndForgePatches.cs");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            savedFields,
            "SavedSpireField<PaelsTooth, int>",
            "EZMicroBalanceNonBossCombatCounter");

        AssertSourceContains(
            source,
            "AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] = 0",
            "[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterCombatEnd))]",
            "if (paelsTooth.Owner.Creature.IsDead)",
            "if (paelsTooth.SerializableCards.Count == 0)",
            "if (room.RoomType == RoomType.Boss)",
            "ClearStoredCards(paelsTooth, \"act boss combat ended\")",
            "var counter = AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] + 1",
            "if (counter < 2)",
            "ChooseAndReturnStoredCard(paelsTooth)",
            "CardModel.FromSerializable(savedCard)",
            "CardSelectCmd.FromChooseABundleScreen",
            "CardCmd.Upgrade(selected, CardPreviewStyle.MessyLayout)",
            "paelsTooth.SerializableCards.Remove(selectedPreview.Saved)",
            "ClearStoredCards(paelsTooth, \"act transition\")");

        Assert.Contains("| Pael's Tooth |", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Pael's Tooth stored cards", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Stored removed cards and combat counter survive save/load.", manualMatrix, StringComparison.Ordinal);
    }

    [Fact]
    public void DebtAndFollyPlayerTextMatchSourceBehavior()
    {
        var debtSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "DebtAndCardPatches.cs");
        var vakuSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "VakuRewardPatches.cs");
        var cards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");

        Assert.Equal("Debt", cards["DEBT.title"]);
        Assert.Equal("Exhaust. When Exhausted, lose 5 Gold.", cards["DEBT.description"]);
        Assert.DoesNotContain("turn", cards["DEBT.description"], StringComparison.OrdinalIgnoreCase);

        Assert.Equal("Folly", cards["FOLLY.title"]);
        Assert.Equal("Unplayable. Innate. Eternal.", cards["FOLLY.description"]);
        Assert.DoesNotContain("Ethereal", cards["FOLLY.description"], StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Retain", cards["FOLLY.description"], StringComparison.OrdinalIgnoreCase);

        AssertSourceContains(
            debtSource,
            "DebtFromSavePatch",
            "__result = new CardKeyword[] { CardKeyword.Exhaust }",
            "__result = new DynamicVar[] { new GoldVar(5) }",
            "DebtTurnEndEffectPatch",
            "__result = false",
            "DebtTurnEndInHandPatch",
            "__result = Task.CompletedTask",
            "debt.ExhaustOnNextPlay = true",
            "Math.Min(5, debt.Owner.Gold)",
            "PlayerCmd.LoseGold(goldToLose, debt.Owner)");

        AssertSourceContains(
            vakuSource,
            "new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 4)",
            "AncientCardHelpers.RemoveKeywords(folly, CardKeyword.Ethereal, CardKeyword.Retain)",
            "__result = new[] { CardKeyword.Unplayable, CardKeyword.Eternal, CardKeyword.Innate }");
    }

    [Fact]
    public void TemporaryGeneratedCardPathsCleanUpOrSelfExpire()
    {
        var turnSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "TurnOfferAndRestPatches.cs");
        var vakuSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "VakuRewardPatches.cs");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            turnSource,
            "CardFactory.GetDistinctForCombat(owner, attackPool, 1, owner.RunState.Rng.CombatCardGeneration)",
            "AncientCardHelpers.ApplyTemporaryCostReduction(generated, 1)",
            "AncientCardHelpers.ApplyKeywords(generated, CardKeyword.Ethereal, CardKeyword.Exhaust)",
            "await AncientCardHelpers.TryAddGeneratedCardToCombat(generated, PileType.Hand, owner)",
            "AncientCardHelpers.RemoveUnpiledCombatCard(generated, combatState)",
            "[HarmonyPatch(typeof(Crossbow), nameof(Crossbow.AfterSideTurnStart))]",
            "__result = Task.CompletedTask",
            "[HarmonyPatch(typeof(ToastyMittens), nameof(ToastyMittens.BeforeHandDraw))]",
            "CardSelectCmd.FromChooseACardScreen(choiceContext, new[] { topCard }, player, canSkip: true)",
            "if (selected != topCard)",
            "await CardCmd.Exhaust(choiceContext, topCard)",
            "PowerCmd.Apply<StrengthPower>");

        AssertSourceContains(
            vakuSource,
            "var copy = cardPlay.Card.CreateClone()",
            "AncientCardHelpers.ApplyTemporaryCostReduction(copy, 1)",
            "AncientCardHelpers.ApplyKeywords(copy, CardKeyword.Ethereal, CardKeyword.Exhaust)",
            "await AncientCardHelpers.TryAddGeneratedCardToCombat(copy, PileType.Hand, musicBox.Owner)",
            "ConditionalWeakTable<MusicBox, State>",
            "MusicBoxStateTracker.MarkUsed(musicBox)",
            "MusicBoxStateTracker.Reset(__instance)");

        Assert.Contains("skipped card does not linger", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Top draw-pile card can be exhausted for Strength or kept.", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Creates a discounted Ethereal Exhaust copy.", manualMatrix, StringComparison.Ordinal);
    }

    [Fact]
    public void MeatCleaverCookRestSiteOptionIsSafeAndScoped()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "TurnOfferAndRestPatches.cs");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var restSite = JsonStringMap("EZMicroBalance", "localization", "eng", "rest_site_ui.json");
        var manualChecklist = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-test-checklist.md");

        Assert.Equal("Adds the Cook option to rest sites. Cook: remove 2 cards and lose 5 HP.", relics["MEAT_CLEAVER.description"]);
        Assert.Equal("Remove 2 cards. Lose 5 HP.", restSite["OPTION_COOK.ezDescription"]);
        Assert.Equal("Requires at least 2 removable cards and more than 5 HP.", restSite["OPTION_COOK.ezDescriptionDisabled"]);

        AssertSourceContains(
            source,
            "public const int CardsToRemove = 2",
            "public const int HpToLose = 5",
            "owner.GetRelic<MeatCleaver>() != null && !MeatCleaverCookPatch.CanCook(owner)",
            "__instance.IsEnabled = false",
            "if (owner.GetRelic<MeatCleaver>() == null)",
            "__instance.IsEnabled ? \"OPTION_COOK.ezDescription\" : \"OPTION_COOK.ezDescriptionDisabled\"",
            "owner.Creature.CurrentHp > HpToLose",
            "Cards.Count(card => card.IsRemovable) >= CardsToRemove",
            "Cancelable = true",
            "RequireManualConfirmation = true",
            "if (cards.Count != CardsToRemove)",
            "await CardPileCmd.RemoveFromDeck(card)",
            "await CreatureCmd.SetCurrentHp(owner.Creature, owner.Creature.CurrentHp - HpToLose)");

        Assert.DoesNotContain("MaxHp", source, StringComparison.Ordinal);
        Assert.Contains("Verify option disabled when too few removable cards.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Verify option disabled when HP is not greater than 5.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Verify no other Cook source is affected unexpectedly.", manualChecklist, StringComparison.Ordinal);
    }

    [Fact]
    public void ReleaseChecklistKeepsPendingRuntimeGatesAndManualRowsExplicit()
    {
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            releaseChecklist,
            "Target manifest id: `EZMicroBalance`",
            "- [x] Existing `EzDailyContent` manifest id remains unchanged.",
            "- [x] `EZMicroBalance` has its own manifest, project, code folder, resource folder, DLL, and PCK.",
            "- [x] Manifest depends on `BaseLib`.",
            "- [x] PCK audit excludes legacy `EzDailyContent`, C# source, docs, art, asset, and archive folders.",
            "- [ ] BaseLib appears in Mod Settings.",
            "- [ ] EZ Micro Balance appears in Mod Settings.",
            "- [ ] `godot.log` reviewed after normal Steam-client manual verification.",
            "- [ ] Every implemented Ancient reward change has a completed manual runtime result.",
            "- [ ] Save/load-sensitive behavior is tested.",
            "- [ ] Disable-mod gameplay behavior is tested in a run.",
            "- [ ] Author placeholder is replaced or explicitly accepted for this private beta.",
            "- [ ] Multiplayer disposition is decided: verified, or release-noted as unsupported/unverified.",
            "- [ ] Worktree is clean.",
            "- [ ] Commit is created.",
            "- [ ] Push to `origin/main` is performed only after explicit user approval.",
            "normal Steam-client Mod Settings verification is still pending",
            "Manual feature results are pending",
            "Unsupported Cases",
            "A11-A20 selection is now default-on in this private-beta multiplayer test candidate",
            "EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1",
            "EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1",
            "A11 widens maps by 1 column, inserts a reachable optional route node in the new column, and adds route rows by act: Act 1 +1, Act 2 +1, Act 3 +2 without A11-specific map markers or hover tips.",
            "A17 inserts one optional 3-4 node Deep Branch in Acts 2/3",
            "A20 uses the vanilla double-boss map path to create/reveal the final-act second Boss",
            "Ascension 21-30 and custom-character content are not included.",
            "Prismatic Gem intentionally skips custom pools, filtered pools, colorless-only pools");

        foreach (var row in RequiredManualMatrixRows)
        {
            Assert.Contains($"| {row} |", manualMatrix, StringComparison.Ordinal);
        }

        Assert.Contains("Status: automated gates passed; live gameplay verification still pending.", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Result: pending.", manualMatrix, StringComparison.Ordinal);
    }

    private static SortedDictionary<string, string> JsonStringMap(params string[] parts)
    {
        using var document = JsonDocument.Parse(ReadRepoText(parts));
        var map = new SortedDictionary<string, string>(StringComparer.Ordinal);

        foreach (var property in document.RootElement.EnumerateObject())
        {
            Assert.Equal(JsonValueKind.String, property.Value.ValueKind);
            map.Add(property.Name, property.Value.GetString() ?? string.Empty);
        }

        return map;
    }

    private static IEnumerable<(string key, string value)> JsonStringValues(JsonElement element, string keyPrefix = "")
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                var key = string.IsNullOrEmpty(keyPrefix)
                    ? property.Name
                    : $"{keyPrefix}.{property.Name}";

                foreach (var value in JsonStringValues(property.Value, key))
                {
                    yield return value;
                }
            }

            yield break;
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            var index = 0;
            foreach (var item in element.EnumerateArray())
            {
                foreach (var value in JsonStringValues(item, $"{keyPrefix}[{index}]"))
                {
                    yield return value;
                }

                index++;
            }

            yield break;
        }

        if (element.ValueKind == JsonValueKind.String)
        {
            yield return (keyPrefix, element.GetString() ?? string.Empty);
        }
    }

    private static string[] Placeholders(string value)
    {
        return Regex.Matches(value, @"\{[^{}]+\}")
            .Select(match => NormalizePlaceholderForParity(match.Value))
            .OrderBy(match => match, StringComparer.Ordinal)
            .ToArray();
    }

    private static string NormalizePlaceholderForParity(string placeholder)
    {
        var pluralMatch = Regex.Match(
            placeholder,
            @"^\{(?<name>[^:{}]+):plural:[^{}]*\}$",
            RegexOptions.CultureInvariant);
        return pluralMatch.Success
            ? $"{{{pluralMatch.Groups["name"].Value}:plural}}"
            : placeholder;
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

    private static void AssertSourceContains(string source, params string[] snippets)
    {
        var missing = snippets
            .Where(snippet => !source.Contains(snippet, StringComparison.Ordinal))
            .ToArray();

        Assert.True(missing.Length == 0, "Missing source evidence:" + Environment.NewLine + string.Join(Environment.NewLine, missing));
    }

    private static string SliceBetween(string value, string start, string end)
    {
        var startIndex = value.IndexOf(start, StringComparison.Ordinal);
        Assert.True(startIndex >= 0, $"Missing start marker: {start}");
        var endIndex = value.IndexOf(end, startIndex, StringComparison.Ordinal);
        Assert.True(endIndex > startIndex, $"Missing end marker after {start}: {end}");
        return value[startIndex..endIndex];
    }

    private static string SliceFrom(string value, string start)
    {
        var startIndex = value.IndexOf(start, StringComparison.Ordinal);
        Assert.True(startIndex >= 0, $"Missing start marker: {start}");
        return value[startIndex..];
    }

    private static void AssertNoRawEnglishInZhsFallback(string value)
    {
        var visibleValue = Regex.Replace(value, @"\{[^{}]*\}", string.Empty, RegexOptions.CultureInvariant);
        Assert.DoesNotMatch(@"[A-Za-z]{2,}", visibleValue);
    }

    private static string ReadAncientSource()
    {
        var sourceRoot = RepoPath("EZMicroBalanceCode", "Ancients");
        return string.Join(
            Environment.NewLine,
            Directory.GetFiles(sourceRoot, "*.cs", SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => File.ReadAllText(path, Encoding.UTF8)));
    }

    private static string ReadZipText(ZipArchive archive, string entryName)
    {
        var entry = archive.Entries.FirstOrDefault(candidate =>
            candidate.FullName.Replace('\\', '/').Equals(entryName, StringComparison.Ordinal));
        Assert.NotNull(entry);

        using var stream = entry.Open();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    private static string ReadRepoText(params string[] parts)
    {
        return File.ReadAllText(RepoPath(parts), Encoding.UTF8);
    }

    private static string RepoPath(params string[] parts)
    {
        return Path.Combine(new[] { FindRepoRoot() }.Concat(parts).ToArray());
    }

    private static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "EZMicroBalance.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repository root from test output directory.");
    }
}
