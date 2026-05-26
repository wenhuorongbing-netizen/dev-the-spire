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
        "rest_site_ui.json",
        "static_hover_tips.json"
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
        "FOLLY.description",
        "SOVEREIGN_BLADE.description"
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
        "SERE_TALON.description",
        "SERE_TALON.eventDescription",
        "SERE_TALON.selectionScreenPrompt",
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
        "Vakuu's Sere Talon",
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
        var archivedV43Plan = ReadRepoText("docs", "features", "ancients-rework-v4", "reference-inputs", "sts2_ancients_rework_v4_3_adjustment_plan.md");
        var archivedPlan = ReadRepoText("docs", "archive", "feature-inputs", "ancients-rework-v4", "sts2_ancients_rework_v4_2_next_plan.md");
        var completionAudit = ReadRepoText("docs", "features", "ancients-rework-v4", "completion-audit.md");

        Assert.Contains("v4.3", archivedV43Plan, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("\u5353\u8d8a\u6597\u7bf7", archivedV43Plan, StringComparison.Ordinal);
        Assert.Contains("\u68f1\u5f69\u5b9d\u77f3", archivedV43Plan, StringComparison.Ordinal);
        Assert.Contains("sts2_ancients_rework_v4_3_adjustment_plan.md", completionAudit, StringComparison.Ordinal);
        Assert.Contains("v4.3 is current", completionAudit, StringComparison.Ordinal);

        Assert.Contains("v4.2", archivedPlan, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("\u5353\u8d8a\u6597\u7bf7", archivedPlan, StringComparison.Ordinal);
        Assert.Contains("\u68f1\u5f69\u5b9d\u77f3", archivedPlan, StringComparison.Ordinal);
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

    [Fact]
    public void SovereignBladeJadeBoonsApplyOnPlayAndAreExplainedByForge()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SovereignBladeForgePatches.cs");
        var cardsEng = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var cardsZhs = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");
        var staticEng = JsonStringMap("EZMicroBalance", "localization", "eng", "static_hover_tips.json");
        var staticZhs = JsonStringMap("EZMicroBalance", "localization", "zhs", "static_hover_tips.json");

        AssertSourceContains(
            source,
            "public const decimal Amount = 3m",
            "[HarmonyPatch(typeof(SovereignBlade), \"OnPlay\")]",
            "await original;",
            "PowerCmd.Apply<StrengthPower>(choiceContext, owner, Amount, owner, blade)",
            "PowerCmd.Apply<DexterityPower>(choiceContext, owner, Amount, owner, blade)",
            "PowerCmd.Apply<PlatingPower>(choiceContext, owner, Amount, owner, blade)",
            "PowerCmd.Apply<RegenPower>(choiceContext, owner, Amount, owner, blade)",
            "PowerCmd.Apply<VigorPower>(choiceContext, owner, Amount, owner, blade)",
            "[HarmonyPatch(typeof(CardModel), \"get_HoverTips\")]",
            "HoverTipFactory.FromPower<StrengthPower>((int)Amount)",
            "HoverTipFactory.FromPower<DexterityPower>((int)Amount)",
            "HoverTipFactory.FromPower<PlatingPower>((int)Amount)",
            "HoverTipFactory.FromPower<RegenPower>((int)Amount)",
            "HoverTipFactory.FromPower<VigorPower>((int)Amount)");

        AssertSovereignBladeText(cardsEng["SOVEREIGN_BLADE.description"], "Strength", "Dexterity", "Plating", "Regen", "Vigor");
        AssertSovereignBladeText(cardsZhs["SOVEREIGN_BLADE.description"], "\u529b\u91cf", "\u654f\u6377", "\u8986\u7532", "\u518d\u751f", "\u6d3b\u529b");
        AssertSovereignBladeText(staticEng["FORGE.description"], "Sovereign Blade", "Strength", "Dexterity", "Plating", "Regen", "Vigor");
        AssertSovereignBladeText(staticZhs["FORGE.description"], "\u541b\u738b\u4e4b\u5251", "\u529b\u91cf", "\u654f\u6377", "\u8986\u7532", "\u518d\u751f", "\u6d3b\u529b");
    }

    [ReleaseArtifactFact]
    public void PrivateBetaZipContainsOnlyInstallableActiveModFiles()
    {
        var packagePath = CurrentPackageZipPath();
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
            dependency => dependency.ValueKind == JsonValueKind.Object &&
                dependency.TryGetProperty("id", out var id) &&
                id.GetString() == "BaseLib" &&
                dependency.TryGetProperty("min_version", out var minVersion) &&
                minVersion.GetString() == "v3.1.4");

        var readme = ReadZipText(archive, "EZMicroBalance/README_INSTALL.txt");
        Assert.Contains("Spire Plus manual-test package", readme, StringComparison.Ordinal);
        Assert.Contains($"Archive: {CurrentPackageName()}.zip", readme, StringComparison.Ordinal);
        Assert.Contains("Display name: Spire Plus", readme, StringComparison.Ordinal);
        Assert.Contains("Technical compatibility id: EZMicroBalance", readme, StringComparison.Ordinal);
        Assert.Contains("Extract this archive into the Slay the Spire 2 mods folder exactly as packaged.", readme, StringComparison.Ordinal);
        Assert.Contains("If the game's Mods list shows EZMicroBalance as the mod name, the package is stale or the display-name route regressed.", readme, StringComparison.Ordinal);
        Assert.Contains("BaseLib", readme, StringComparison.Ordinal);
        Assert.Contains("EzDailyContent disabled or absent", readme, StringComparison.Ordinal);
        Assert.Contains("EZMicroBalance is a technical folder/id only; player-facing screens should say Spire Plus.", readme, StringComparison.Ordinal);
        Assert.Contains("Ancient selections now grant visible marker relics", readme, StringComparison.Ordinal);
        Assert.Contains("manual-test build, not release-ready", readme, StringComparison.Ordinal);
        Assert.Contains("Save/load, death/failure paths, and co-op still need manual proof", readme, StringComparison.Ordinal);
        Assert.Contains("Ascension 21-30 and custom-character content are not included", readme, StringComparison.Ordinal);
        Assert.DoesNotContain("source-safe", readme, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Found 16 SavedSpireFields", readme, StringComparison.Ordinal);
    }

    [Fact]
    public void HarmonyPatchTargetsAreDeclaredForImplementedAncientSurfaces()
    {
        var allSource = ReadSourceTree("EZMicroBalanceCode", "Ancients");

        AssertSourceContains(
            allSource,
            "[HarmonyPatch(typeof(PaelsHorn), nameof(PaelsHorn.AfterObtained))]",
            "[HarmonyPatch(typeof(RelicModel), nameof(RelicModel.AfterObtained))]",
            "[HarmonyPatch(typeof(RelicCmd), nameof(RelicCmd.Obtain), typeof(RelicModel), typeof(Player), typeof(int))]",
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
            "[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Hooks.Hook), nameof(MegaCrit.Sts2.Core.Hooks.Hook.TryModifyCardRewardOptions))]",
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
            "[HarmonyPatch(typeof(SovereignBlade), \"OnPlay\")]",
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
    public void VelvetChokerSoftLimitCountsOnlyManualFirstFromHandPlaysAndResetsEachTurn()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var turnSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
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
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
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

        Assert.DoesNotContain("\u8bbe\u4e3a 0", source, StringComparison.Ordinal);
        Assert.DoesNotContain("\u8d39\u7528\u4e3a 0", source, StringComparison.Ordinal);
        Assert.Contains("\u8bbe\u4e3a0", source, StringComparison.Ordinal);
        Assert.Contains("\u8d39\u7528\u4e3a0", source, StringComparison.Ordinal);
        Assert.DoesNotContain("\u80fd\u91cf[/gold]\u8d39\u7528\u964d\u4f4e [blue]1", activeCode, StringComparison.Ordinal);
        Assert.DoesNotContain("\u8017\u80fd[/gold]\u964d\u4f4e [blue]1", activeCode, StringComparison.Ordinal);
        Assert.Contains("[gold]\u8017\u80fd[/gold]\u964d\u4f4e[blue]1[/blue]", activeCode, StringComparison.Ordinal);
        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void ManualVerificationMatrixUsesUncorruptedSimplifiedChineseExpectedText()
    {
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var zhsCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");
        var zhsLocalizationText = string.Join(
            Environment.NewLine,
            Directory.GetFiles(RepoPath("EZMicroBalance", "localization", "zhs"), "*.json", SearchOption.TopDirectoryOnly)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => File.ReadAllText(path, Encoding.UTF8)));
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        Assert.DoesNotContain("\uFFFD", zhsLocalizationText, StringComparison.Ordinal);
        Assert.DoesNotContain("\u95c2", zhsLocalizationText, StringComparison.Ordinal);
        Assert.DoesNotContain("\u95c1", zhsLocalizationText, StringComparison.Ordinal);

        Assert.Contains("\u8fc5\u901f", zhsRelics["BEAUTIFUL_BRACELET.description"], StringComparison.Ordinal);
        Assert.Equal("\u5c061\u5f20\u653e\u677e\u4e0e1\u5f20\u653e\u677e+\u52a0\u5165\u4f60\u7684\u724c\u7ec4\u3002", zhsRelics["PAELS_HORN.description"]);
        Assert.DoesNotContain("\u5df2\u5347\u7ea7\u7684\u653e\u677e+", zhsRelics["PAELS_HORN.description"], StringComparison.Ordinal);
        Assert.Contains("\u795e\u5316", zhsRelics["JEWELRY_BOX.description"], StringComparison.Ordinal);
        Assert.Contains("\u56fa\u6709", zhsRelics["JEWELRY_BOX.description"], StringComparison.Ordinal);
        Assert.Contains("\u8bb8\u613f", zhsRelics["SERE_TALON.description"], StringComparison.Ordinal);
        Assert.Equal("Vakuu's Sere Talon", engRelics["SERE_TALON.title"]);
        Assert.Equal("\u74e6\u5e93\u539f\u521d\u4e4b\u722a", zhsRelics["SERE_TALON.title"]);
        Assert.Equal("Tanx Claws", engRelics["CLAWS.title"]);
        Assert.Equal("\u5766\u514b\u65af\u5229\u722a", zhsRelics["CLAWS.title"]);
        Assert.Equal("On pickup, transform up to [blue]{Cards}[/blue] cards into upgraded Maul.", engRelics["CLAWS.description"]);
        Assert.DoesNotContain("\u4f24\u5bb3+[blue]1[/blue]", zhsRelics["CLAWS.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("[blue]1[/blue] more damage", engRelics["CLAWS.description"], StringComparison.Ordinal);
        Assert.Equal("\u62fe\u53d6\u65f6\uff0c\u5c06\u81f3\u591a[blue]{Cards}[/blue]\u5f20\u724c\u53d8\u5316\u4e3a\u6495\u54ac+\u3002", zhsRelics["CLAWS.description"]);
        Assert.Contains("\u5f02\u8272\u724c", zhsRelics["PRISMATIC_GEM.description"], StringComparison.Ordinal);
        Assert.Equal("\u68f1\u5f69\u8ba1\u6570\uff1a{Count}/{Cycle}", zhsRelics["PRISMATIC_GEM.countHint.title"]);
        Assert.Equal("\u68f1\u5f69\u5956\u52b1\uff1a\u672c\u6b21\u53ea\u51fa\u73b0\u5f02\u8272\u724c\u3002", zhsRelics["PRISMATIC_GEM.rewardScreenHint"]);
        Assert.Contains("\u7075\u4f53", zhsRelics["DISTINGUISHED_CAPE.description"], StringComparison.Ordinal);
        Assert.Contains("\u975e\u9996\u9886", zhsRelics["PAELS_TOOTH.description"], StringComparison.Ordinal);
        Assert.Contains("\u9996\u9886", zhsRelics["PAELS_TOOTH.description"], StringComparison.Ordinal);
        Assert.Contains("\u83b7\u5f971\u70b9\u80fd\u91cf", zhsRelics["PRISMATIC_GEM.description"], StringComparison.Ordinal);
        Assert.Contains("\u624b\u724c", zhsRelics["FIDDLE.description"], StringComparison.Ordinal);
        Assert.Contains("\u81f3\u5c1118\u70b9", zhsRelics["DISTINGUISHED_CAPE.description"], StringComparison.Ordinal);
        Assert.Equal("\u503a\u52a1", zhsCards["DEBT.title"]);
        Assert.Equal("\u6267\u8ff7", zhsCards["ENTHRALLED.title"]);
        Assert.Equal("\u611a\u884c", zhsCards["FOLLY.title"]);
        Assert.Contains("\u6d88\u8017", zhsCards["DEBT.description"], StringComparison.Ordinal);
        Assert.Contains("\u6c38\u6052", zhsCards["ENTHRALLED.description"], StringComparison.Ordinal);
        Assert.Contains("\u4fdd\u7559", zhsRelics["CHOICES_PARADOX.description"], StringComparison.Ordinal);
        Assert.Contains("\u865a\u65e0", zhsRelics["CROSSBOW.description"], StringComparison.Ordinal);
        Assert.Contains("\u529b\u91cf", zhsRelics["TOASTY_MITTENS.description"], StringComparison.Ordinal);

        AssertSourceContains(
            manualMatrix,
            "Tanx Claws",
            "Maul+",
            "\u6495\u54ac+",
            "Vakuu's Sere Talon");
    }

    [Fact]
    public void VakuuSereTalonAndTanxClawsStayOnSeparateSourceRoutes()
    {
        var vakuuSource = ReadRepoText("source code", "src", "Core", "Models", "Events", "Vakuu.cs");
        var tanxSource = ReadRepoText("source code", "src", "Core", "Models", "Events", "Tanx.cs");
        var sereTalonSource = ReadRepoText("source code", "src", "Core", "Models", "Relics", "SereTalon.cs");
        var clawsSource = ReadRepoText("source code", "src", "Core", "Models", "Relics", "Claws.cs");
        var sereTalonPickupPatch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonPickupPatches.cs");
        var sereTalonVisualSource = ReadSereTalonVisualSource();
        var tanxClawsPatch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "TanxClawsMaulTuningPatches.cs");
        var ancientPatchSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        AssertSourceContains(
            vakuuSource,
            "RelicOption<SereTalon>()");
        Assert.DoesNotContain("RelicOption<Claws>()", vakuuSource, StringComparison.Ordinal);

        AssertSourceContains(
            tanxSource,
            "RelicOption<Claws>()");
        Assert.DoesNotContain("RelicOption<SereTalon>()", tanxSource, StringComparison.Ordinal);

        AssertSourceContains(
            sereTalonSource,
            "new DynamicVar(\"Curses\", 2m)",
            "new DynamicVar(\"Wishes\", 3m)",
            "HoverTipFactory.FromCardWithCardHoverTips<Wish>()",
            "CardPileCmd.Add(card, PileType.Deck)",
            "CardPileCmd.Add(card2, PileType.Deck)");
        Assert.DoesNotContain("Maul", sereTalonSource, StringComparison.Ordinal);

        AssertSourceContains(
            sereTalonPickupPatch,
            "[HarmonyPatch(typeof(SereTalon), nameof(SereTalon.AfterObtained))]",
            "private const int CurseOfferCount = 4",
            "private const int CursePickCount = 1",
            "private const int NormalWishCount = 2",
            "private const int UpgradedWishCount = 1",
            "ModelDb.CardPool<CurseCardPool>()",
            "owner.RunState.Rng.Niche.NextItem(availableCurses)",
            "sereTalon.Flash()",
            "CardSelectCmd.FromSimpleGrid",
            "new BlockingPlayerChoiceContext()",
            "new LocString(\"relics\", \"SERE_TALON.selectionScreenPrompt\")",
            "Cancelable = false",
            "RequireManualConfirmation = true",
            "AncientCardHelpers.RemoveUnpiledRunCard(curse)",
            "owner.RunState.CreateCard<Wish>(owner)",
            "CardCmd.Upgrade(wish, CardPreviewStyle.None)",
            "SpirePlusFeedback.PreviewDeckAdds(successfulAdds, sereTalon, 2f)");
        Assert.DoesNotContain("Claws", sereTalonPickupPatch, StringComparison.Ordinal);
        Assert.DoesNotContain("Maul", sereTalonPickupPatch, StringComparison.Ordinal);

        AssertSourceContains(
            clawsSource,
            "new CardsVar(6)",
            "HoverTipFactory.FromCardWithCardHoverTips<Maul>()",
            "CreateMaulFromOriginal",
            "CardCmd.Transform(transformations, base.Owner.PlayerRng.Transformations)");
        Assert.DoesNotContain("Wish", clawsSource, StringComparison.Ordinal);
        Assert.DoesNotContain("CurseCardPool", clawsSource, StringComparison.Ordinal);

        AssertSourceContains(
            sereTalonVisualSource,
            "internal static class SereTalonVisualAssetPaths",
            "internal static class SereTalonVisualRelicModelRoutes",
            "internal static class SereTalonVisualNodeRoutes",
            "internal static class SereTalonVisualTextures",
            "internal static class SereTalonVisualRouteLog",
            "relic is not SereTalon",
            "[HarmonyPatch(typeof(NEventOptionButton), nameof(NEventOptionButton._Ready))]",
            "TryApplyEventOptionButton",
            "button.Option?.Relic is not SereTalon",
            "GetNodeOrNull<TextureRect>(\"%RelicIcon\")",
            "Ancient event option button",
            "[HarmonyPatch(typeof(NRelic), \"Reload\")]",
            "TryApplyRelicNode",
            "IsNodeReady()",
            "InvalidOperationException",
            "NRelic small node",
            "NRelic large node",
            "RelicModel packed icon texture",
            "RelicModel big icon texture",
            "SereTalon uses Spire Plus art and Tanx Claws is untouched");
        AssertSourceContains(
            tanxClawsPatch,
            "[HarmonyPatch(typeof(Claws), nameof(Claws.AfterObtained))]",
            "owner.RunState.CreateCard<Maul>(owner)",
            "maul.UpgradeInternal()",
            "CardCmd.Upgrade(maul, CardPreviewStyle.None)");
        Assert.DoesNotContain("SereTalon", tanxClawsPatch, StringComparison.Ordinal);
        Assert.DoesNotContain("[HarmonyPatch(typeof(Maul)", ancientPatchSource, StringComparison.Ordinal);

        Assert.Equal("Vakuu's Sere Talon", engRelics["SERE_TALON.title"]);
        Assert.Equal("On pickup, choose [blue]1[/blue] of [blue]4[/blue] Curses. Add it, [blue]2[/blue] Wish, and [blue]1[/blue] Wish+ to your deck.", engRelics["SERE_TALON.description"]);
        Assert.Equal("Choose 1 Curse.", engRelics["SERE_TALON.selectionScreenPrompt"]);
        Assert.Equal("\u74e6\u5e93\u539f\u521d\u4e4b\u722a", zhsRelics["SERE_TALON.title"]);
        Assert.Equal("\u62fe\u53d6\u65f6\uff0c\u4ece[blue]4[/blue]\u5f20\u8bc5\u5492\u4e2d\u9009\u62e9[blue]1[/blue]\u5f20\u3002\u5c06\u5b83\u3001[blue]2[/blue]\u5f20[gold]\u8bb8\u613f[/gold]\u548c[blue]1[/blue]\u5f20[gold]\u8bb8\u613f+[/gold]\u52a0\u5165\u4f60\u7684\u724c\u7ec4\u3002", zhsRelics["SERE_TALON.description"]);
        Assert.Equal("\u9009\u62e91\u5f20\u8bc5\u5492\u3002", zhsRelics["SERE_TALON.selectionScreenPrompt"]);

        Assert.Equal("Tanx Claws", engRelics["CLAWS.title"]);
        Assert.Equal("On pickup, transform up to [blue]{Cards}[/blue] cards into upgraded Maul.", engRelics["CLAWS.description"]);
        Assert.Equal("\u5766\u514b\u65af\u5229\u722a", zhsRelics["CLAWS.title"]);
        Assert.Equal("\u62fe\u53d6\u65f6\uff0c\u5c06\u81f3\u591a[blue]{Cards}[/blue]\u5f20\u724c\u53d8\u5316\u4e3a\u6495\u54ac+\u3002", zhsRelics["CLAWS.description"]);
    }

    private static string ReadSereTalonVisualSource() =>
        string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualAssetPaths.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualRelicModelRoutes.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualNodeRoutes.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualTextures.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualRouteLog.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualPatches.cs"));

    [Fact]
    public void AncientDirectDeckGainFeedbackFlashesSourceRelicAndCardPreview()
    {
        var feedbackSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "SpirePlusFeedback.cs");
        var ancientSource = ReadSourceTree("EZMicroBalanceCode", "Ancients");

        AssertSourceContains(
            feedbackSource,
            "RelicTriggerSfx = \"event:/sfx/ui/relic_activate_general\"",
            "sourceRelic.Flash()",
            "NRelicFlashVfx.Create(sourceRelic)",
            "AboveTopBarVfxContainer.AddChildSafely(flashVfx)",
            "public static void ConfirmRelicPayoff(RelicModel? sourceRelic)",
            "models.Insert(0, sourceRelic)",
            "CardCmd.PreviewCardPileAdd(successfulAdds, seconds)",
            "NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short)");

        AssertSourceContains(
            ancientSource,
            "SpirePlusFeedback.PreviewDeckAdds(results, paelsHorn, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(result, jewelryBox, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(result, preservedFog, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(results, cape, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(results, sealOfGold, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, paelsTooth)",
            "SpirePlusFeedback.PreviewDeckAdds(successfulAdds, sereTalon, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResults, player.GetRelic<UrdaMoltingOptionRelic>(), 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<UrdaSeedbedOptionRelic>(), 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<UrdaTrialBranchOptionRelic>(), 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<UrdaSeedBankOptionRelic>(), 2f)",
            "SpirePlusFeedback.ConfirmRelicPayoff(eliteRoot)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<MorviForbiddenLoanOptionRelic>(), 2f)");
    }

    [Fact]
    public void CurrentAncientDocsDoNotPresentSupersededV42BehaviorAsCurrent()
    {
        var sourceDesign = ReadRepoText("docs", "features", "ancients-rework-v4", "source-design.md");
        var currentDocs = string.Join(
            Environment.NewLine,
            [
                ReadRepoText("README.md"),
                ReadRepoText("docs", "test-plan.md"),
                ReadRepoText("docs", "release-checklist.md"),
                sourceDesign,
                ReadRepoText("docs", "features", "ancients-rework-v4", "api-discovery.md"),
                ReadRepoText("docs", "features", "ancients-rework-v4", "implementation-plan.md"),
                ReadRepoText("docs", "features", "ancients-rework-v4", "high-risk-review.md"),
                ReadRepoText("docs", "features", "ancients-rework-v4", "manual-test-checklist.md"),
                ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md"),
                ReadRepoText("docs", "features", "ancients-rework-v4", "completion-audit.md"),
                ReadRepoText("docs", "features", "ancients-rework-v4", "localization-validation.md")
            ]);

        Assert.Contains("compact active source-design summary", sourceDesign, StringComparison.Ordinal);
        Assert.Contains("source-design-mojibake-pre-slim-20260518.md", sourceDesign, StringComparison.Ordinal);
        Assert.True(sourceDesign.Split('\n').Length <= 80, "Keep active Ancients v4 source-design compact; move long design history to archive/reference inputs.");
        Assert.DoesNotContain("\uFFFD", sourceDesign, StringComparison.Ordinal);
        Assert.DoesNotContain("TSpire PlusT", sourceDesign, StringComparison.Ordinal);
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
        var pickupSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var combatSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
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
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");

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
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
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
        var vakuSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
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
        var turnSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var vakuSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
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
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "MeatCleaverCookPatches.cs");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var restSite = JsonStringMap("EZMicroBalance", "localization", "eng", "rest_site_ui.json");
        var staticHovers = JsonStringMap("EZMicroBalance", "localization", "eng", "static_hover_tips.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var zhsRestSite = JsonStringMap("EZMicroBalance", "localization", "zhs", "rest_site_ui.json");
        var zhsStaticHovers = JsonStringMap("EZMicroBalance", "localization", "zhs", "static_hover_tips.json");
        var manualChecklist = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-test-checklist.md");

        Assert.Equal("Adds a [gold]Cleaver[/gold] option to rest sites: remove [blue]2[/blue] cards and lose [blue]5[/blue] HP.", relics["MEAT_CLEAVER.description"]);
        Assert.Equal("Cleaver", restSite["OPTION_COOK.name"]);
        Assert.Equal("Remove 2 cards. Lose 5 HP.", restSite["OPTION_COOK.ezDescription"]);
        Assert.Equal("Requires at least 2 removable cards and more than 5 HP.", restSite["OPTION_COOK.ezDescriptionDisabled"]);
        Assert.Equal("At a [gold]Rest Site[/gold], [gold]remove[/gold] [blue]2[/blue] cards from your [gold]Deck[/gold] and lose [blue]5[/blue] HP.", staticHovers["COOK.description"]);
        Assert.Equal("Cleaver", staticHovers["COOK.title"]);
        Assert.Equal("\u5728\u4f11\u606f\u5904\u52a0\u5165[gold]\u5207\u8089[/gold]\u9009\u9879\uff1a\u79fb\u9664[blue]2[/blue]\u5f20\u724c\u5e76\u5931\u53bb[blue]5[/blue]\u70b9\u751f\u547d\u3002", zhsRelics["MEAT_CLEAVER.description"]);
        Assert.Equal("\u5207\u8089", zhsRestSite["OPTION_COOK.name"]);
        Assert.Equal("\u79fb\u96642\u5f20\u724c\u3002\u5931\u53bb5\u70b9\u751f\u547d\u3002", zhsRestSite["OPTION_COOK.ezDescription"]);
        Assert.Equal("\u9700\u8981\u81f3\u5c112\u5f20\u53ef\u79fb\u9664\u724c\u4e14\u751f\u547d\u503c\u5927\u4e8e5\u3002", zhsRestSite["OPTION_COOK.ezDescriptionDisabled"]);
        Assert.Equal("\u5728[gold]\u4f11\u606f\u5904[/gold]\u4ece\u4f60\u7684[gold]\u724c\u7ec4[/gold]\u4e2d[gold]\u79fb\u9664[/gold][blue]2[/blue]\u5f20\u724c\uff0c\u5e76\u5931\u53bb[blue]5[/blue]\u70b9\u751f\u547d\u3002", zhsStaticHovers["COOK.description"]);
        Assert.Equal("\u5207\u8089", zhsStaticHovers["COOK.title"]);
        Assert.DoesNotContain("gain [green]9[/green] Max HP", staticHovers["COOK.description"], StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\u83b7\u5f97[green]9[/green]\u70b9\u6700\u5927\u751f\u547d", zhsStaticHovers["COOK.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("Cooking", relics["MEAT_CLEAVER.description"], StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\u95bb\u621d\u7d8a\u9288", zhsRelics["MEAT_CLEAVER.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("\u95bb\u621d\u7d8a\u9288", zhsRestSite["OPTION_COOK.name"], StringComparison.Ordinal);
        Assert.DoesNotContain("\u95bb\u621d\u7d8a\u9288", zhsStaticHovers["COOK.title"], StringComparison.Ordinal);

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
        Assert.Contains("Expected: Cleaver / \u5207\u8089 option removes 2 removable cards and costs 5 HP.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Verify no other rest-site source is affected unexpectedly.", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Expected: Cook option", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("\u70f9\u996a", manualChecklist, StringComparison.Ordinal);
    }

    [Fact]
    public void ReleaseChecklistKeepsPendingRuntimeGatesAndManualRowsExplicit()
    {
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            releaseChecklist,
            "Target manifest id: `EZMicroBalance`",
            "- [x] The active release surface is one mod: `Spire Plus`.",
            "- [x] Legacy `EzDailyContent` and standalone `EZFuturePeek` root mod surfaces have been removed from the active tree.",
            "- [x] `EZMicroBalance` has its own manifest, project, code folder, resource folder, DLL, and PCK.",
            "- [x] Manifest declares structured `BaseLib` dependency with `min_version: v3.1.4`.",
            "- [x] PCK audit packages only `EZMicroBalance` installable resources and excludes C# source, docs, art, asset, and archive folders.",
            "- [x] BaseLib appears in Mod Settings.",
            "- [x] Spire Plus appears in the current normal Steam-client manifest list and registers its config page under the refreshed display-name package.",
            "- [x] Spire Plus appears in a refreshed Mod Settings UI screenshot after the display-name refresh package is installed.",
            "current-spire-plus-modsettings-20260513-111342",
            "- [ ] Fresh loader smoke for the current beta.69 ZIP hash is pending.",
            "- [ ] Latest loader smoke for the current beta.69 package hash has not been recaptured yet.",
            "- [ ] `godot.log` reviewed after fresh beta.69 normal Steam-client isolated startup/log verification.",
            "- [ ] `godot.log` reviewed after full normal Steam-client gameplay/manual verification.",
            "- [ ] Every implemented Ancient reward change has a completed manual runtime result.",
            "- [ ] Save/load-sensitive behavior is tested.",
            "- [ ] Disable-mod gameplay behavior is tested in a run.",
            "- [x] Author placeholder is replaced for this private beta; `EZMicroBalance.json` author is `wenhuorongbing-netizen`.",
            "- [x] Rootblight I/II/III and Blight Sprout generated portrait art is integrated and packaged; live in-game visual verification remains part of the manual matrix.",
            "- [ ] Multiplayer disposition is decided: verified, or release-noted as unsupported/unverified.",
            "- [ ] Worktree is clean.",
            "- [ ] Commit is created.",
            "- [ ] Push to `origin` is performed after validation, packaging, and an intentional commit.",
            "Fresh loader smoke for the current beta.69 package hash is pending",
            "Refreshed normal Steam-client Mod Settings UI evidence at `.tools\\runtime-evidence\\current-spire-plus-modsettings-20260513-111342\\02-mod-config-list.png` shows `Spire Plus`",
            "Earlier page-level Mod Settings evidence predates the display-name refresh",
            "Manual feature results are pending",
            "Unsupported Cases",
            "A11-A20 selection is default-on only for single-player standard lobbies",
            "SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1",
            "SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1",
            "A11 widens maps by 1 column, inserts a reachable optional route node in the new column, and adds route rows by act: Act 1 +1, Act 2 +1, Act 3 +2 without A11-specific map markers or hover tips.",
            "A17 inserts one optional 3-4 node Deep Branch in Acts 2/3",
            "A20 uses the vanilla double-boss map path to create/reveal the final-act second Boss",
            "Ascension 21-30 and custom-character content are not included.",
            "Prismatic Gem intentionally skips custom pools, filtered pools, colorless-only pools");

        foreach (var row in RequiredManualMatrixRows)
        {
            Assert.Contains($"| {row} |", manualMatrix, StringComparison.Ordinal);
        }

        Assert.Contains("Status: automated gates passed; latest normal Steam-client startup/log verification is historical for the earlier 22-field package; refreshed normal Steam-client Mod Settings UI list screenshot shows Spire Plus; historical page-level Mod Settings UI passed under the old display name; A0/A10/A20 single-player DevConsole combat smoke passed; A11 Act 1 map/save-load spot check and saved-map boss-reachability graph proof passed; A11 Act 2/3 map-surface observation passed; targeted A14 Rootblight English/ZHS hover/starter-notice spot checks passed.", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Full live Ancient reward gameplay, Rootblight combat-end behavior/notices, natural route-click first-node checks beyond the A11 spot check, Ancient save/load, natural A11 click-by-click traversal, and multiplayer verification are still pending.", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Natural route-click first-node path remains pending.", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Result: pending.", manualMatrix, StringComparison.Ordinal);
    }

    private static void AssertSovereignBladeText(string value, params string[] requiredTerms)
    {
        Assert.True(CountOccurrences(value, "[blue]3[/blue]") >= 5, "Sovereign Blade text should show all five 3-point jade boons.");
        foreach (var term in requiredTerms)
        {
            Assert.Contains(term, value, StringComparison.Ordinal);
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
        if (pluralMatch.Success)
        {
            return $"{{{pluralMatch.Groups["name"].Value}:plural}}";
        }

        var chooseMatch = Regex.Match(
            placeholder,
            @"^\{(?<name>[^:{}]+):choose\((?<choice>[^)]*)\):[^{}]*\}$",
            RegexOptions.CultureInvariant);
        return chooseMatch.Success
            ? $"{{{chooseMatch.Groups["name"].Value}:choose({chooseMatch.Groups["choice"].Value})}}"
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

    private static void AssertNoRawEnglishInZhsFallback(string value)
    {
        var visibleValue = Regex.Replace(value, @"\{[^{}]*\}", string.Empty, RegexOptions.CultureInvariant);
        Assert.DoesNotMatch(@"[A-Za-z]{2,}", visibleValue);
    }

}
