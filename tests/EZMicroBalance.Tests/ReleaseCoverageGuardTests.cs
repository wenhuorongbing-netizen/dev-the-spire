using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class ReleaseCoverageGuardTests
{
    private static readonly string[] InstallableArtifactFiles =
    [
        "EZMicroBalance.dll",
        "EZMicroBalance.json",
        "EZMicroBalance.pck"
    ];

    private static readonly string[] PackagedFiles =
    [
        "EZMicroBalance.dll",
        "EZMicroBalance.json",
        "EZMicroBalance.pck",
        "README_INSTALL.txt"
    ];

    private static readonly string[] CurrentFacingDocs =
    [
        "README.md",
        "docs/dev-environment.md",
        "docs/private-beta-verification-handoff.md",
        "docs/test-plan.md",
        "docs/release-checklist.md",
        "docs/features/ancients-rework-v4/completion-audit.md",
        "docs/features/ancients-rework-v4/manual-verification-matrix.md",
        "docs/features/ascension-11-20/api-research.md",
        "docs/features/ascension-11-20/manual-test-checklist.md"
    ];

    private static readonly string[] ExpectedActiveSourceFiles =
    [
        "EZMicroBalanceCode/Ancients/Common/AncientCardHelpers.cs",
        "EZMicroBalanceCode/Ancients/Common/AncientSavedStateFields.cs",
        "EZMicroBalanceCode/Ancients/Common/JeweledMaskFreePower.cs",
        "EZMicroBalanceCode/Ancients/PaelsHornPhase1Patch.cs",
        "EZMicroBalanceCode/Ancients/Patches/DebtAndCardPatches.cs",
        "EZMicroBalanceCode/Ancients/Patches/PaelsToothAndForgePatches.cs",
        "EZMicroBalanceCode/Ancients/Patches/PickupRewardPatches.cs",
        "EZMicroBalanceCode/Ancients/Patches/PrismaticGemPatches.cs",
        "EZMicroBalanceCode/Ancients/Patches/SealOfGoldPatches.cs",
        "EZMicroBalanceCode/Ancients/Patches/TurnOfferAndRestPatches.cs",
        "EZMicroBalanceCode/Ancients/Patches/VakuRewardPatches.cs",
        "EZMicroBalanceCode/Ancients/BrightestFlameExhaustDrawPatch.cs",
        "EZMicroBalanceCode/Ascension/AscensionA20Patches.cs",
        "EZMicroBalanceCode/Ascension/AscensionA20RewardScreenPatches.cs",
        "EZMicroBalanceCode/Ascension/AscensionAssetPaths.cs",
        "EZMicroBalanceCode/Ascension/AscensionCombatModifierService.cs",
        "EZMicroBalanceCode/Ascension/AscensionCombatTracker.cs",
        "EZMicroBalanceCode/Ascension/AscensionDiagnostics.cs",
        "EZMicroBalanceCode/Ascension/AscensionExpansionConfig.cs",
        "EZMicroBalanceCode/Ascension/AscensionFeatureGate.cs",
        "EZMicroBalanceCode/Ascension/AscensionInitializer.cs",
        "EZMicroBalanceCode/Ascension/AscensionMapQuestMarker.cs",
        "EZMicroBalanceCode/Ascension/AscensionMapService.cs",
        "EZMicroBalanceCode/Ascension/AscensionMapUiPatches.cs",
        "EZMicroBalanceCode/Ascension/AscensionNodeMetadata.cs",
        "EZMicroBalanceCode/Ascension/AscensionRewardService.cs",
        "EZMicroBalanceCode/Ascension/AscensionSavedStateFields.cs",
        "EZMicroBalanceCode/Ascension/AscensionSelectionPatches.cs",
        "EZMicroBalanceCode/Ascension/BannerRoomMapQuestMarker.cs",
        "EZMicroBalanceCode/Ascension/BossSealDefinition.cs",
        "EZMicroBalanceCode/Ascension/Cards/BossSealCards.cs",
        "EZMicroBalanceCode/Ascension/Cards/RootCards.cs",
        "EZMicroBalanceCode/Ascension/Events/A20Courtyard.cs",
        "EZMicroBalanceCode/Ascension/FiremarkedEliteMapQuestMarker.cs",
        "EZMicroBalanceCode/Ascension/FissionEnchantment.cs",
        "EZMicroBalanceCode/Ascension/ForgeTokenService.cs",
        "EZMicroBalanceCode/Ascension/MultiplayerDiagnostics.cs",
        "EZMicroBalanceCode/Ascension/Powers/BannerPowers.cs",
        "EZMicroBalanceCode/Ascension/Powers/BossSealPowers.cs",
        "EZMicroBalanceCode/Ascension/Powers/FiremarkPowers.cs",
        "EZMicroBalanceCode/Ascension/Relics/ForgeTokenRelic.cs",
        "EZMicroBalanceCode/Ascension/RoyalDecreeEnchantment.cs",
        "EZMicroBalanceCode/Ascension/RootBudCombatHook.cs",
        "EZMicroBalanceCode/Ascension/RootDeckService.cs",
        "EZMicroBalanceCode/Ascension/RootRunHook.cs",
        "EZMicroBalanceCode/MainFile.cs"
    ];

    private sealed record AncientSystemGuard(
        string ManualRow,
        string[] RelicKeys,
        string[] CardKeys,
        string[] RestSiteKeys,
        string[] SourceSnippets);

    private sealed record GatedAscensionSliceGuard(
        string ManualSectionStart,
        string ManualSectionEnd,
        string[] SourceSnippets,
        string[] ApiSnippets,
        string[] ManualSnippets);

    private static readonly AncientSystemGuard[] ImplementedAncientSystems =
    [
        new("Pael's Horn", ["PAELS_HORN.description"], [], [], ["PaelsHorn", "CreateCard<Relax>", "CardCmd.Upgrade(upgradedRelax)"]),
        new("Black Star", ["BLACK_STAR.description"], [], [], ["BlackStar", "RelicFactory.PullNextRelicFromFront"]),
        new("War Hammer", ["WAR_HAMMER.description"], [], [], ["WarHammer", "CardSelectCmd.FromDeckForUpgrade", "CardCmd.Upgrade(cards, CardPreviewStyle.HorizontalLayout)"]),
        new("Jewelry Box", ["JEWELRY_BOX.description"], [], [], ["JewelryBox", "CreateNonInnateApotheosis", "Apotheosis"]),
        new("Preserved Fog / Folly", ["PRESERVED_FOG.description"], ["FOLLY.title", "FOLLY.description"], [], ["PreservedFog", "Folly", "FollyKeywordsPatch"]),
        new("Claws", ["CLAWS.description"], [], [], ["Claws", "CreateClawsCurseDraft", "CreateCard<Wish>"]),
        new("Choices Paradox", ["CHOICES_PARADOX.description"], [], [], ["ChoicesParadox", "ChooseRareTemporaryCard", "CardKeyword.Retain"]),
        new("Jeweled Mask", ["JEWELED_MASK.description", "JEWELED_MASK.ezSelectionScreenPrompt"], [], [], ["JeweledMask", "JeweledMaskFreePower", "CardCmd.Enchant<JeweledMaskFreePower>"]),
        new("Prismatic Gem", ["PRISMATIC_GEM.description"], [], [], ["PrismaticGem", "RewardScreenState", "GetOffColorRewardPool"]),
        new("Distinguished Cape", ["DISTINGUISHED_CAPE.description", "DISTINGUISHED_CAPE.eventDescription"], [], [], ["DistinguishedCape", "CalculateMaxHpLoss", "CreateCard<Apparition>"]),
        new("Velvet Choker", ["VELVET_CHOKER.description"], [], [], ["VelvetChoker", "VelvetChokerSoftLimitTracker", "CardEnergyCost.GetWithModifiers"]),
        new("Pael's Tooth", ["PAELS_TOOTH.description"], [], [], ["PaelsTooth", "PaelsToothNonBossCombatCounter", "ChooseAndReturnStoredCard"]),
        new("Sovereign Blade / Forge", [], [], [], ["ForgeCmd", "SovereignBlade", "CreatedThroughForge"]),
        new("Seal of Gold / Debt", ["SEAL_OF_GOLD.description"], ["DEBT.title", "DEBT.description"], [], ["SealOfGold", "DebtCardPatch", "CreateCard<Debt>"]),
        new("Sozu", ["SOZU.description"], [], [], ["Sozu", "InitialPotionFillOwners", "PotionCmd.TryToProcure"]),
        new("Ectoplasm", ["ECTOPLASM.description"], [], [], ["Ectoplasm", "InitialGoldOwners", "PlayerCmd.GainGold"]),
        new("Fiddle", ["FIDDLE.description"], [], [], ["Fiddle", "FiddleHandLimit", "FiddleDrawCapPatch"]),
        new("Iron Club", ["IRON_CLUB.description"], [], [], ["IronClub", "IronClubVarsPatch", "new CardsVar(5)"]),
        new("Brilliant Scarf", ["BRILLIANT_SCARF.description"], [], [], ["BrilliantScarf", "BrilliantScarfVarsPatch", "new CardsVar(6)"]),
        new("Beautiful Bracelet", ["BEAUTIFUL_BRACELET.description"], [], [], ["BeautifulBracelet", "ModelDb.Enchantment<Swift>", "AddSwiftTwo"]),
        new("Music Box", ["MUSIC_BOX.description"], [], [], ["MusicBox", "MusicBoxStateTracker", "CreateClone"]),
        new("Crossbow", ["CROSSBOW.description"], [], [], ["Crossbow", "OfferTemporaryAttack", "CardSelectCmd.FromChooseACardScreen"]),
        new("Toasty Mittens", ["TOASTY_MITTENS.description"], [], [], ["ToastyMittens", "OfferTopCardExhaust", "StrengthPower"]),
        new("Whispering Earring", ["WHISPERING_EARRING.description"], [], [], ["WhisperingEarring", "AutoPlayOneHighestCostCard", "AutoPlayType.Default"]),
        new("Meat Cleaver", ["MEAT_CLEAVER.description"], [], ["OPTION_COOK.ezDescription", "OPTION_COOK.ezDescriptionDisabled"], ["MeatCleaver", "CookRestSiteOption", "CardsToRemove = 2"]),
        new("Blood-Soaked Rose / Enthralled", ["BLOOD_SOAKED_ROSE.description"], ["ENTHRALLED.title", "ENTHRALLED.description"], [], ["Enthralled", "PlayEnthralled", "GainBlock"])
    ];

    private static readonly GatedAscensionSliceGuard[] ImplementedGatedAscensionSlices =
    [
        new(
            "## A12 Firemarked Elite and Forge Token",
            "## A13 Fission Enchantment",
            [
                "FiremarkedEliteLevel = 12",
                "MarkFiremarkedElite",
                "FiremarkKind.Might",
                "FiremarkKind.Giant",
                "FiremarkKind.ForgeArmor",
                "FiremarkKind.ConstantHeal",
                "AscensionMapQuestMarker",
                "FiremarkedEliteMapQuestMarker",
                "ActOneFiremarkedEliteTargetCount = 2",
                "LaterActFiremarkedEliteTargetCount = 3",
                "PickFiremarkedElitesByAct",
                "FiremarkedEliteMapIconPatch",
                "MightMarkFiremarkPower",
                "GiantMarkFiremarkPower",
                "ForgeArmorMarkFiremarkPower",
                "ConstantHealMarkFiremarkPower",
                "FiremarkedEliteRewardTargetOptionCount = 4",
                "ForgeTokenHeld",
                "ForgeTokenRelic",
                "ForgeTokenService.GrantAfterFiremarkedElite",
                "ApplyAfterRestSiteHeal",
                "ApplyAfterRestSiteSmith",
                "DuplicateTokenGoldAmount"
            ],
            [
                "Firemarked Elite and Forge Token are implemented for the A12 Ascension-level gate.",
                "One Firemark Host receives Might, Giant, Forge Armor, or Constant Heal",
                "Special rest-site action payout is disabled"
            ],
            [
                "Gated implementation present; live testing pending.",
                "Firemarked elite is visible before route commitment.",
                "Defeating firemarked elite grants one visible Forge Token status relic with counter 1.",
                "Forge Token save/load behavior is stable."
            ]),
        new(
            "## A13 Fission Enchantment",
            "## A16 Banner Rooms",
            [
                "FissionLevel = 13",
                "FissionEnchantment",
                "ILocalizationProvider",
                "LocManager.Instance.Language == \"zhs\"",
                "CustomIconPath => AscensionAssetPaths.FissionEnchantmentIcon",
                "TryApplyFission",
                "CardCmd.Enchant<FissionEnchantment>",
                "IsFissionEligible",
                "!card.ExhaustOnNextPlay",
                "CardKeyword.Exhaust"
            ],
            [
                "Fission is implemented for the A13 Ascension-level gate.",
                "Fission reward mutation is source-patched; reward reroll, pickup, localization rendering, and save/load are pending."
            ],
            [
                "Gated implementation present; live testing pending.",
                "Fission appears only on eligible reward cards.",
                "Tooltip/card text is correct in English and Simplified Chinese, uses energy-cost wording, does not show raw `{energyPrefix:energyIcons(...)}` templates, does not duplicate the added Exhaust line, and does not use the Chinese word \"费用\" for Fission.",
                "Picked Fission cards save/load correctly."
            ]),
        new(
            "## A16 Banner Rooms",
            "## A17 Deep Branches",
            [
                "BannerRoomLevel = 16",
                "MarkBannerRooms",
                "BannerRoomMapQuestMarker",
                "BannerKind.Vanguard",
                "BannerKind.ShieldFormation",
                "BannerKind.Bounty",
                "ApplyBannerCombatStart",
                "HasActiveBanner"
            ],
            [
                "Banner Rooms are implemented for the A16 Ascension-level gate.",
                "Banner node marking and combat modifiers are source-patched; live route visibility, persistence, reward settlement, and combat cleanup are pending."
            ],
            [
                "Gated implementation present; live testing pending.",
                "Banner rooms are visible before route commitment.",
                "Banner modifiers apply only to the intended combat.",
                "Banner modifiers do not persist into later combats."
            ]),
        new(
            "## A19/A20 Boss Systems",
            "## Disable and Uninstall",
            [
                "BossSealsLevel = 19",
                "DoubleRoyalBrandLevel = 20",
                "MarkBossSeals",
                "BossSealDefinition",
                "BossSealCatalog",
                "BossSealImplementationStatus.SourceGuardedPendingLiveVerification",
                "HolyDaze",
                "HolyDazePower",
                "MarginalNote",
                "RoyalDecreeEnchantment",
                "AeonglassStrength",
                "IsBossBrand",
                "AscensionA20GenerateRoomsPatch",
                "AscensionA20CourtyardProceedPatch",
                "A20Courtyard",
                "AscensionA20RewardScreenReadyPatch",
                "A20_INTERMISSION_HEADER",
                "BossMapPointHoverPatch",
                "BOSS_KING_BRAND",
                "TryAddBossSealRewardOption",
                "BossRewardTargetOptionCount = 4",
                "TryAddA20BossOneCardReward"
            ],
            [
                "`BossSealDefinition` / `BossSealCatalog` now map active boss encounters to the v2.0 Royal Seal set",
                "source-guarded through supported hooks",
                "Boss 1 post-combat recovery",
                "fixed courtyard event",
                "vanilla double-boss map path"
            ],
            [
                "Gated implementation present as BossSeal definitions plus source-guarded runtime hooks; live testing pending.",
                "A19 boss-specific Royal Seal metadata is assigned at map generation.",
                "Boss card rewards improve as documented.",
                "Boss 1 reward screen opens the A20 courtyard event before the second Boss."
            ])
    ];

    [Fact]
    public void ActiveSourceFilesAreCoveredByTheGuardManifest()
    {
        var activeSources = Directory.GetFiles(RepoPath("EZMicroBalanceCode"), "*.cs", SearchOption.AllDirectories)
            .Select(path => ToRepoRelativePath(path))
            .Where(path => path != "EZMicroBalanceCode/Ancients/GlobalUsings.cs")
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(ExpectedActiveSourceFiles.OrderBy(path => path, StringComparer.Ordinal), activeSources);

        var testSource = ReadAllTestSource();
        foreach (var activeSource in activeSources)
        {
            Assert.Contains(Path.GetFileName(activeSource), testSource, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ImplementedAncientSystemsHaveSourceDocsAndLocalizationCoverage()
    {
        var allAncientSource = ReadSourceTree("EZMicroBalanceCode", "Ancients");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");
        var englishRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var simplifiedChineseRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var englishCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var simplifiedChineseCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");
        var englishRestSite = JsonStringMap("EZMicroBalance", "localization", "eng", "rest_site_ui.json");
        var simplifiedChineseRestSite = JsonStringMap("EZMicroBalance", "localization", "zhs", "rest_site_ui.json");

        foreach (var system in ImplementedAncientSystems)
        {
            Assert.Contains($"| {system.ManualRow} |", manualMatrix, StringComparison.Ordinal);
            AssertLocalizedKeys(system.RelicKeys, englishRelics, simplifiedChineseRelics, $"relic localization for {system.ManualRow}");
            AssertLocalizedKeys(system.CardKeys, englishCards, simplifiedChineseCards, $"card localization for {system.ManualRow}");
            AssertLocalizedKeys(system.RestSiteKeys, englishRestSite, simplifiedChineseRestSite, $"rest-site localization for {system.ManualRow}");

            foreach (var snippet in system.SourceSnippets)
            {
                Assert.Contains(snippet, allAncientSource, StringComparison.Ordinal);
            }
        }
    }

    [Fact]
    public void GatedRootFamilySystemsHaveSourceDocsAndLocalizationCoverage()
    {
        var allAscensionSource = ReadSourceTree("EZMicroBalanceCode", "Ascension");
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");
        var englishCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var simplifiedChineseCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");

        AssertSourceContains(
            allAscensionSource,
            "RootBeginsLevel = 14",
            "BossRootBudLevel = 15",
            "EliteRootBudLevel = 18",
            "DebugLevelEnvironmentVariable = \"EZMB_ASCENSION_DEBUG_LEVEL\"",
            "return 0;",
            "RootFamilyCard",
            "RootBud",
            "RootblightIII",
            "RootblightLevel",
            "RootDeckService.AddRootblightI",
            "RootDeckService.ResolveCombatEndRootblight",
            "CardPileCmd.RemoveFromDeck(card, showPreview: false)");

        AssertLocalizedKeys(
            [
                "EZMB_ROOT.title",
                "EZMB_ROOT.description",
                "EZMB_DEEP_ROOT.title",
                "EZMB_DEEP_ROOT.description",
                "EZMB_ROOTBLIGHT_III.title",
                "EZMB_ROOTBLIGHT_III.description",
                "EZMB_ROOT_BUD.title",
                "EZMB_ROOT_BUD.description"
            ],
            englishCards,
            simplifiedChineseCards,
            "Root-family card localization");

        Assert.Contains("A14 Rootblight MVP", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("A15 Boss Blight Sprout MVP", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("A18 Elite Blight Sprout MVP", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Rootblight I/II/III and Blight Sprout are implemented for A14/A15/A18 after the current standard-lobby selector expansion.", manualChecklist, StringComparison.Ordinal);

        Assert.Contains("A11-A20 selection is now default-on in this private-beta multiplayer test candidate", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("`EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.", releaseChecklist, StringComparison.Ordinal);
    }

    [Fact]
    public void GatedAscensionSlicesHaveSourceDocsAndManualCoverage()
    {
        var allAscensionSource = ReadSourceTree("EZMicroBalanceCode", "Ascension");
        var apiResearch = ReadRepoText("docs", "features", "ascension-11-20", "api-research.md");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");

        AssertSourceContains(
            allAscensionSource,
            "WiderLongerMapLevel = 11",
            "A11ExtraMapColumns = 1",
            "A11ActOneExtraMapRows = 1",
            "A11ActTwoExtraMapRows = 1",
            "A11ActThreeExtraMapRows = 2",
            "AscensionFeatureGate.IsMapGeometryEnabled(runState)",
            "DeepBranchesLevel = 17",
            "AscensionFeatureGate.IsDeepBranchesEnabled(runState)",
            "DeepBranchMinLength = 3",
            "DeepBranchMaxLength = 4",
            "TryInsertDeepBranch",
            "safe-route reconnect",
            "canBeModified: false",
            "DeepBranchNodeKind.EnhancedReward",
            "HasPathAvoiding(parent, reconnect, existingBranchPoints)",
            "Ascension A11 applied: expanded map width",
            "reachable optional route",
            "TryInsertA11WidthChoice(saved)",
            "new SavedActMap(saved)",
            "A17 gate active: Deep Branch already present or unsupported for safe insertion");
        Assert.Contains("A11 converts the generated map", apiResearch, StringComparison.Ordinal);
        Assert.Contains("No A11-specific marker", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("LONG_ROAD_NODE", allAscensionSource, StringComparison.Ordinal);
        Assert.DoesNotContain("LongRoad", allAscensionSource, StringComparison.Ordinal);
        Assert.Contains("A17 uses the same saved-map replacement path", apiResearch, StringComparison.Ordinal);

        foreach (var slice in ImplementedGatedAscensionSlices)
        {
            AssertSourceContains(allAscensionSource, slice.SourceSnippets);
            AssertSourceContains(apiResearch, slice.ApiSnippets);

            var manualSection = SliceBetween(manualChecklist, slice.ManualSectionStart, slice.ManualSectionEnd);
            AssertSourceContains(manualSection, slice.ManualSnippets);
            Assert.DoesNotContain("- [x]", manualSection, StringComparison.Ordinal);
            Assert.DoesNotContain("Release-ready", manualSection, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void SourceDeclaredCustomLocalizationKeysExistInEnglishAndSimplifiedChinese()
    {
        var allSource = ReadSourceTree("EZMicroBalanceCode");
        var failures = new List<string>();

        foreach (var match in Regex.Matches(allSource, @"new\s+LocString\(\s*""(?<table>[^""]+)""\s*,\s*""(?<key>[^""]+)""").Cast<Match>())
        {
            var table = match.Groups["table"].Value;
            var key = match.Groups["key"].Value;

            var english = JsonStringMap("EZMicroBalance", "localization", "eng", $"{table}.json");
            var simplifiedChinese = JsonStringMap("EZMicroBalance", "localization", "zhs", $"{table}.json");
            if (!english.ContainsKey(key) || !simplifiedChinese.ContainsKey(key))
            {
                failures.Add($"{table}:{key}");
            }
        }

        foreach (var id in Regex.Matches(allSource, @"public\s+const\s+string\s+CardId\s*=\s*""(?<id>EZMB_[^""]+)""").Cast<Match>().Select(match => match.Groups["id"].Value))
        {
            var english = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
            var simplifiedChinese = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");
            foreach (var suffix in new[] { "title", "description" })
            {
                var key = $"{id}.{suffix}";
                if (!english.ContainsKey(key) || !simplifiedChinese.ContainsKey(key))
                {
                    failures.Add($"cards:{key}");
                }
            }
        }

        Assert.True(failures.Count == 0, "Missing active localization keys:" + Environment.NewLine + string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void ExportPresetTracksEveryActiveResourceAndExcludesNonReleaseFolders()
    {
        var exportPreset = ReadRepoText("export_presets.cfg");
        var exported = ParseExportFiles(exportPreset);
        var expected = Directory.GetFiles(RepoPath("EZMicroBalance"), "*", SearchOption.AllDirectories)
            .Where(path => !path.EndsWith(".import", StringComparison.Ordinal))
            .Where(IsActiveExportResource)
            .Select(path => "res://" + ToRepoRelativePath(path))
            .Concat(["res://EZMicroBalance.json"])
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expected, exported.OrderBy(path => path, StringComparer.Ordinal));
        Assert.DoesNotContain("res://EZMicroBalance/mod_real.png", exported);

        Assert.Contains("export_filter=\"resources\"", exportPreset, StringComparison.Ordinal);
        Assert.Contains("include_filter=\"EZMicroBalance.json,EZMicroBalance/localization/*/*.json\"", exportPreset, StringComparison.Ordinal);
        AssertSourceContains(
            exportPreset,
            "EzDailyContent/*",
            "EzDailyContentCode/*",
            "EZMicroBalanceCode/*",
            "art_pipeline/*",
            "asset/*",
            "source code/*",
            "docs/*",
            "legacy/*");
    }

    private static bool IsActiveExportResource(string path)
    {
        var relativePath = ToRepoRelativePath(path);
        if (relativePath.Equals("EZMicroBalance/mod_real.png", StringComparison.Ordinal) ||
            relativePath.Equals("EZMicroBalance/mod_real.png.import", StringComparison.Ordinal))
        {
            return false;
        }

        return Path.GetExtension(path) is ".json" or ".png";
    }

    [ReleaseArtifactFact]
    public void PackageStagingVersionedZipAndInstalledArtifactsHaveMatchingHashes()
    {
        var version = ManifestVersion();
        var packageName = $"EZMicroBalance-{version}";
        var installedDir = GamePath("mods", "EZMicroBalance");
        var stagingDir = RepoPath("publish", "package-staging", "EZMicroBalance");
        var versionedDir = RepoPath("publish", packageName, "EZMicroBalance");
        var zipPath = RepoPath("publish", $"{packageName}.zip");

        AssertPackageDirectory(stagingDir);
        AssertPackageDirectory(versionedDir);
        Assert.True(File.Exists(zipPath), $"Missing package zip: {zipPath}");

        using var archive = ZipFile.OpenRead(zipPath);
        var zipEntries = archive.Entries
            .Where(entry => !string.IsNullOrEmpty(entry.Name))
            .Select(entry => entry.FullName.Replace('\\', '/'))
            .OrderBy(entry => entry, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(PackagedFiles.Select(file => $"EZMicroBalance/{file}").OrderBy(file => file, StringComparer.Ordinal), zipEntries);

        foreach (var fileName in InstallableArtifactFiles)
        {
            var installedHash = Sha256(Path.Combine(installedDir, fileName));
            Assert.Equal(installedHash, Sha256(Path.Combine(stagingDir, fileName)));
            Assert.Equal(installedHash, Sha256(Path.Combine(versionedDir, fileName)));
            Assert.Equal(installedHash, Sha256(ReadZipBytes(archive, $"EZMicroBalance/{fileName}")));
        }

        var stagingReadmeHash = Sha256(Path.Combine(stagingDir, "README_INSTALL.txt"));
        Assert.Equal(stagingReadmeHash, Sha256(Path.Combine(versionedDir, "README_INSTALL.txt")));
        Assert.Equal(stagingReadmeHash, Sha256(ReadZipBytes(archive, "EZMicroBalance/README_INSTALL.txt")));
    }

    [ReleaseArtifactFact]
    public void CurrentDocsMatchReleaseHashesAndAvoidPinnedStaleTestTotals()
    {
        var packageHash = Sha256(RepoPath("publish", $"EZMicroBalance-{ManifestVersion()}.zip"));
        var modImageHash = Sha256(RepoPath("EZMicroBalance", "mod_image.png"));
        var legacyModImageHash = Sha256(RepoPath("EzDailyContent", "mod_image.png"));

        Assert.NotEqual(legacyModImageHash, modImageHash);

        var docsByPath = CurrentFacingDocs.ToDictionary(path => path, path => ReadRepoText(path.Split('/')), StringComparer.Ordinal);
        var combinedDocs = string.Join(Environment.NewLine, docsByPath.Values);

        Assert.Contains(packageHash, docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.Contains(packageHash, docsByPath["docs/dev-environment.md"], StringComparison.Ordinal);
        Assert.Contains(packageHash, docsByPath["docs/features/ancients-rework-v4/completion-audit.md"], StringComparison.Ordinal);
        Assert.Contains(modImageHash, docsByPath["docs/dev-environment.md"], StringComparison.Ordinal);

        Assert.DoesNotMatch(@"\b(?:24|28|34)\s*/\s*(?:24|28|34)\b", combinedDocs);
        Assert.DoesNotMatch(@"\b(?:24|28|34)\s+tests?\b", combinedDocs);
        Assert.DoesNotMatch(@"passed\s+(?:24|28|34)\b", combinedDocs);
        Assert.DoesNotContain("failed 5 tests", combinedDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("invalidated by later source/art changes", combinedDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("not current for release", combinedDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("hash parity is broken", combinedDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("release pass is blocked", combinedDocs, StringComparison.OrdinalIgnoreCase);

        Assert.Contains("manual feature verification", docsByPath["README.md"], StringComparison.OrdinalIgnoreCase);
        Assert.Contains("still pending", docsByPath["README.md"], StringComparison.OrdinalIgnoreCase);
        Assert.Contains("normal Steam-client Mod Settings verification is still pending", docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.Contains("Manual feature results are pending", docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.DoesNotContain("private beta ready", combinedDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("release ready", combinedDocs, StringComparison.OrdinalIgnoreCase);
    }

    [ReleaseArtifactFact]
    public void PrivateBetaVerificationHandoffCarriesCurrentArtifactsAndManualBlockers()
    {
        var packageHash = Sha256(RepoPath("publish", $"EZMicroBalance-{ManifestVersion()}.zip"));
        var installedDir = GamePath("mods", "EZMicroBalance");
        var dllHash = Sha256(Path.Combine(installedDir, "EZMicroBalance.dll"));
        var manifestHash = Sha256(Path.Combine(installedDir, "EZMicroBalance.json"));
        var pckHash = Sha256(Path.Combine(installedDir, "EZMicroBalance.pck"));
        var handoff = ReadRepoText("docs", "private-beta-verification-handoff.md");

        Assert.Contains(packageHash, handoff, StringComparison.Ordinal);
        Assert.Contains(dllHash, handoff, StringComparison.Ordinal);
        Assert.Contains(manifestHash, handoff, StringComparison.Ordinal);
        Assert.Contains(pckHash, handoff, StringComparison.Ordinal);
        Assert.Contains("Record results in `docs/features/ancients-rework-v4/manual-verification-matrix.md`", handoff, StringComparison.Ordinal);
        Assert.Contains("update `docs/release-checklist.md`", handoff, StringComparison.Ordinal);
        Assert.Contains("Normal Steam-client Mod Settings verification is still pending", handoff, StringComparison.Ordinal);
        Assert.Contains("Live Ancient reward gameplay, save/load, disable-gameplay, and multiplayer checks are still pending", handoff, StringComparison.Ordinal);
        Assert.Contains("A11-A20 selection is now default-on in this private-beta multiplayer test candidate", handoff, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1", handoff, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1", handoff, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required", handoff, StringComparison.Ordinal);
        Assert.Contains("docs/features/ascension-11-20/multiplayer-test-runbook.md", handoff, StringComparison.Ordinal);
        Assert.Contains("Live co-op selection and desync verification are still pending", handoff, StringComparison.Ordinal);
        Assert.Contains("AUTHOR_NAME_REPLACE_ME", handoff, StringComparison.Ordinal);
        Assert.Contains("Current git status at this handoff refresh", handoff, StringComparison.Ordinal);
        Assert.Contains("77da0ed (HEAD -> main, origin/main, origin/HEAD) fix2", handoff, StringComparison.Ordinal);
        Assert.Contains("Proposed commit scope", handoff, StringComparison.Ordinal);
        Assert.Contains("Do not include", handoff, StringComparison.Ordinal);
        Assert.Contains("Directory.Build.props", handoff, StringComparison.Ordinal);
        Assert.Contains("art_pipeline/`, `asset/`, or `source code/`", handoff, StringComparison.Ordinal);
        Assert.Contains("Push only after explicit user approval", handoff, StringComparison.Ordinal);
    }

    [Fact]
    public void ReleaseArtifactAndRuntimeEvidenceTestsAreExplicitlyOptIn()
    {
        var testSource = ReadAllTestSource().Replace("\r\n", "\n");
        var testPlan = ReadRepoText("docs", "test-plan.md");
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var handoff = ReadRepoText("docs", "private-beta-verification-handoff.md");
        var issues = ReadRepoText("docs", "issues.md");

        Assert.Contains("ReleaseArtifactFactAttribute", testSource, StringComparison.Ordinal);
        Assert.Contains("EZMB_RUN_RELEASE_ARTIFACT_TESTS", testSource, StringComparison.Ordinal);
        Assert.Contains("Skipping release artifact/runtime checks", testSource, StringComparison.Ordinal);

        foreach (var methodName in new[]
        {
            "PrivateBetaZipContainsOnlyInstallableActiveModFiles",
            "PackageContainsCurrentAscensionLocalization",
            "ActiveReleaseArtMatchesAuditedNoTextNoLogoAsset",
            "PublishedPckContainsOnlyActiveReleaseResources",
            "InstalledDllMatchesABuildOutput",
            "InstalledManifestMatchesRepositoryManifest",
            "HarmonyPatchesResolveAgainstInstalledGameApi",
            "PrismaticGemRewardBannerContractMatchesInstalledGameApi",
            "PackageStagingVersionedZipAndInstalledArtifactsHaveMatchingHashes",
            "CurrentDocsMatchReleaseHashesAndAvoidPinnedStaleTestTotals",
            "PrivateBetaVerificationHandoffCarriesCurrentArtifactsAndManualBlockers",
            "ActiveCoverArtAndInactiveModRealPolicyMatchExportPckAndPackage",
            "ExportedResourcesInstalledPckAndPackagePckStayInParity",
            "CurrentReleaseHashClaimsMatchInstalledStagingVersionedAndZipArtifacts",
            "RecentSmokeLogSupportsControlledSmokeClaims"
        })
        {
            Assert.Contains($"[ReleaseArtifactFact]\n    public void {methodName}", testSource, StringComparison.Ordinal);
        }

        Assert.Contains("EZMB_RUN_RELEASE_ARTIFACT_TESTS=1", testPlan, StringComparison.Ordinal);
        Assert.Contains("skipped in normal developer test runs", testPlan, StringComparison.Ordinal);
        Assert.Contains("Release artifact tests are opt-in", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("EZMB_RUN_RELEASE_ARTIFACT_TESTS=1", handoff, StringComparison.Ordinal);
        Assert.Contains("Normal `dotnet test` no longer requires ignored publish/package artifacts", issues, StringComparison.Ordinal);
    }

    [Fact]
    public void MultiplayerTestRunbookCoversDefaultOnGateControlsAndLiveMatrix()
    {
        var runbook = ReadRepoText("docs", "features", "ascension-11-20", "multiplayer-test-runbook.md");

        AssertSourceContains(
            runbook,
            "A11-A20 selection is now default-on in this private-beta multiplayer test candidate.",
            "Two physical PCs.",
            "Same-PC multi-open is not reliable for real Steam multiplayer and should not be the primary release test.",
            "`--force-steam off` is valid for controlled loader smoke only.",
            "EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1",
            "EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1",
            "EZMB_ASCENSION_DIAGNOSTICS=1",
            "EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1",
            "[Environment]::SetEnvironmentVariable('EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION','1','User')",
            "[Environment]::SetEnvironmentVariable('EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION',$null,'User')",
            "fully restart Steam and the game",
            "Gate Default-On Checks",
            "Gate-Off Comparison Checks",
            "Multiplayer-Only Disable Checks",
            "A11 Map Checks",
            "A12 Firemarked Elite Marker Checks",
            "A16 Banner Marker / Hover Checks",
            "A14/A15/A18 Rootblight / Blight Sprout Ownership Checks",
            "A20 Warning / Downgrade Checks",
            "Save / Load Checks",
            "godot.log Checks",
            "Date/time:",
            "Pass/fail/blocker:");

        Assert.Contains("Dual King Brands / second-boss Brand gameplay is currently disabled or downgraded in co-op pending live verification.", runbook, StringComparison.Ordinal);
    }

    [Fact]
    public void PublicAscensionSelectorExpandsStandardLobbiesAndAvoidsGlobalProgressPatches()
    {
        var allSource = ReadSourceTree("EZMicroBalanceCode");
        var selectorPatch = ReadRepoText("EZMicroBalanceCode", "Ascension", "AscensionSelectionPatches.cs");

        var ascensionSource = ReadSourceTree("EZMicroBalanceCode", "Ascension");
        AssertSourceContains(
            selectorPatch,
            "HarmonyPatch(typeof(StartRunLobby), \"SetSingleplayerAscensionAfterCharacterChanged\")",
            "HarmonyPatch(typeof(StartRunLobby), \"BeginRunLocally\")",
            "HarmonyPatch(typeof(StartRunLobby), \"UpdateMaxMultiplayerAscension\")",
            "HarmonyPatch(typeof(StartRunLobby), \"UpdatePreferredAscension\")",
            "AccessTools.Field(typeof(StartRunLobby), \"<MaxAscension>k__BackingField\")",
            "if (MaxAscensionBackingField == null)",
            "Ascension selector expansion skipped",
            "lobby.NetService.Type == NetGameType.Singleplayer",
            "lobby.NetService.Type == NetGameType.Host",
            "lobby.GameMode != GameMode.Daily",
            "!AscensionFeatureGate.IsMultiplayerSelectionDisabled",
            "TemporarilyExpandMultiplayerUnlocks",
            "maxMultiplayerAscensionUnlocked = AscensionFeatureGate.MaxSupportedAscensionLevel",
            "RestoreMultiplayerUnlocks",
            "ShouldSkipVanillaPreferredAscensionSave",
            "not writing it to vanilla progress",
            "ProgressMaxAscensionOverride",
            "OriginalMaxAscension",
            "__state.Stats.MaxAscension = __state.OriginalMaxAscension");

        Assert.DoesNotContain("NAscensionPanel", allSource, StringComparison.Ordinal);
        Assert.DoesNotContain("ProgressSaveManager", allSource, StringComparison.Ordinal);
        Assert.DoesNotContain("HarmonyPatch(typeof(CharacterStats", ascensionSource, StringComparison.Ordinal);
        Assert.DoesNotContain("HarmonyPatch(typeof(ProgressState", allSource, StringComparison.Ordinal);
        Assert.DoesNotContain("maxAscensionAllowed", allSource, StringComparison.Ordinal);
        Assert.DoesNotContain("[HarmonyPatch(typeof(Ascension", allSource, StringComparison.Ordinal);
    }

    [Fact]
    public void UnsupportedAscensionAndFutureSystemsAreNotMarkedComplete()
    {
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");
        var currentDocs = string.Join(
            Environment.NewLine,
            CurrentFacingDocs.Select(path => ReadRepoText(path.Split('/'))));

        var allSource = ReadSourceTree("EZMicroBalanceCode");
        foreach (var sourceSnippet in new[] { "A21", "Ascension21", "CustomCharacter" })
        {
            Assert.DoesNotContain(sourceSnippet, allSource, StringComparison.Ordinal);
        }

        foreach (var (start, end) in new[]
        {
            ("## A12 Firemarked Elite and Forge Token", "## A13 Fission Enchantment"),
            ("## A13 Fission Enchantment", "## A16 Banner Rooms"),
            ("## A16 Banner Rooms", "## A17 Deep Branches"),
            ("## A19/A20 Boss Systems", "## Disable and Uninstall")
        })
        {
            var section = SliceBetween(manualChecklist, start, end);
            Assert.Contains("Gated implementation present", section, StringComparison.Ordinal);
            Assert.Contains("live testing pending", section, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Release-ready", section, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("- [x]", section, StringComparison.Ordinal);
        }

        var deepBranchesSection = SliceBetween(manualChecklist, "## A17 Deep Branches", "## A19/A20 Boss Systems");
        Assert.Contains("Gated implementation present; live testing pending.", deepBranchesSection, StringComparison.Ordinal);
        Assert.Contains("safe-route reconnect", deepBranchesSection, StringComparison.Ordinal);
        Assert.Contains("Multiplayer branch insertion is skipped", deepBranchesSection, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x]", deepBranchesSection, StringComparison.Ordinal);

        Assert.Contains("A11-A20 selection is now default-on in this private-beta multiplayer test candidate", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("A11 widens maps by 1 column, inserts a reachable optional route node in the new column, and adds route rows by act: Act 1 +1, Act 2 +1, Act 3 +2 without A11-specific map markers or hover tips.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("A17 inserts one optional 3-4 node Deep Branch in Acts 2/3", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("A20 uses the vanilla double-boss map path to create/reveal the final-act second Boss", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("A20 inserts a fixed courtyard event between Boss 1 rewards and Boss 2", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("Ascension 21-30 and custom-character content are not included.", releaseChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Ascension 21-30 implementation complete", currentDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("custom character implementation complete", currentDocs, StringComparison.OrdinalIgnoreCase);
    }

    private static void AssertPackageDirectory(string packageDir)
    {
        Assert.True(Directory.Exists(packageDir), $"Missing package directory: {packageDir}");
        var entries = Directory.GetFiles(packageDir, "*", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(packageDir, path).Replace('\\', '/'))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(PackagedFiles.OrderBy(file => file, StringComparer.Ordinal), entries);
    }

    private static void AssertLocalizedKeys(
        IEnumerable<string> keys,
        IReadOnlyDictionary<string, string> english,
        IReadOnlyDictionary<string, string> simplifiedChinese,
        string context)
    {
        foreach (var key in keys)
        {
            Assert.True(english.TryGetValue(key, out var englishValue), $"Missing English {context}: {key}");
            Assert.True(simplifiedChinese.TryGetValue(key, out var zhsValue), $"Missing zhs {context}: {key}");
            Assert.False(string.IsNullOrWhiteSpace(englishValue), $"Empty English {context}: {key}");
            Assert.False(string.IsNullOrWhiteSpace(zhsValue), $"Empty zhs {context}: {key}");
        }
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

    private static string[] ParseExportFiles(string exportPreset)
    {
        var match = Regex.Match(exportPreset, @"export_files=PackedStringArray\((?<files>[^)]*)\)");
        Assert.True(match.Success, "Could not find export_files in export_presets.cfg.");

        return Regex.Matches(match.Groups["files"].Value, @"""(?<path>[^""]+)""")
            .Cast<Match>()
            .Select(match => match.Groups["path"].Value)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
    }

    private static void AssertSourceContains(string source, params string[] snippets)
    {
        var missing = snippets
            .Where(snippet => !source.Contains(snippet, StringComparison.Ordinal))
            .ToArray();

        Assert.True(missing.Length == 0, "Missing source evidence:" + Environment.NewLine + string.Join(Environment.NewLine, missing));
    }

    private static string SliceBetween(string source, string startMarker, string endMarker)
    {
        var start = source.IndexOf(startMarker, StringComparison.Ordinal);
        Assert.True(start >= 0, $"Missing start marker: {startMarker}");

        var end = source.IndexOf(endMarker, start + startMarker.Length, StringComparison.Ordinal);
        Assert.True(end >= 0, $"Missing end marker: {endMarker}");

        return source[start..end];
    }

    private static string ReadAllTestSource()
    {
        return string.Join(
            Environment.NewLine,
            Directory.GetFiles(RepoPath("tests", "EZMicroBalance.Tests"), "*.cs", SearchOption.TopDirectoryOnly)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => File.ReadAllText(path, Encoding.UTF8)));
    }

    private static string ReadSourceTree(params string[] parts)
    {
        var root = RepoPath(parts);
        return string.Join(
            Environment.NewLine,
            Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => File.ReadAllText(path, Encoding.UTF8)));
    }

    private static byte[] ReadZipBytes(ZipArchive archive, string entryName)
    {
        var entry = archive.Entries.FirstOrDefault(candidate =>
            candidate.FullName.Replace('\\', '/').Equals(entryName, StringComparison.Ordinal));
        Assert.NotNull(entry);

        using var stream = entry.Open();
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }

    private static string ManifestVersion()
    {
        using var document = JsonDocument.Parse(ReadRepoText("EZMicroBalance.json"));
        return document.RootElement.GetProperty("version").GetString() ?? throw new InvalidOperationException("Missing manifest version.");
    }

    private static string Sha256(string path)
    {
        Assert.True(File.Exists(path), $"Missing file to hash: {path}");
        using var stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }

    private static string Sha256(byte[] bytes)
    {
        return Convert.ToHexString(SHA256.HashData(bytes));
    }

    private static string ToRepoRelativePath(string path)
    {
        return Path.GetRelativePath(FindRepoRoot(), path).Replace('\\', '/');
    }

    private static string ReadRepoText(params string[] parts)
    {
        return File.ReadAllText(RepoPath(parts), Encoding.UTF8);
    }

    private static string RepoPath(params string[] parts)
    {
        return Path.Combine(new[] { FindRepoRoot() }.Concat(parts).ToArray());
    }

    private static string GamePath(params string[] parts)
    {
        var root = Environment.GetEnvironmentVariable("STS2_PATH");
        if (string.IsNullOrWhiteSpace(root))
        {
            root = @"D:\Steam\steamapps\common\Slay the Spire 2";
        }

        return Path.Combine(new[] { root }.Concat(parts).ToArray());
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
