using System.Buffers.Binary;
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
        "docs/private-beta-release-completion-audit.md",
        "docs/test-plan.md",
        "docs/test-ready-completion-audit.md",
        "docs/release-checklist.md",
        "docs/features/ancients-rework-v4/completion-audit.md",
        "docs/features/ancients-rework-v4/manual-verification-matrix.md",
        "docs/features/ascension-11-20/api-research.md",
        "docs/features/ascension-11-20/manual-test-checklist.md"
    ];

    private static readonly string[] ExpectedActiveSourceFiles =
    [
        "EZMicroBalanceCode/Ancients/Common/AncientCardHelpers.cs",
        "EZMicroBalanceCode/Ancients/Common/AncientPlayerState.cs",
        "EZMicroBalanceCode/Ancients/Common/AncientRewardRelicService.cs",
        "EZMicroBalanceCode/Ancients/Common/AncientSavedStateFields.cs",
        "EZMicroBalanceCode/Ancients/Common/JeweledMaskFreePower.cs",
        "EZMicroBalanceCode/Ancients/Patches/PaelsHornPhase1Patch.cs",
        "EZMicroBalanceCode/Ancients/Patches/DebtAndCardPatches.cs",
        "EZMicroBalanceCode/Ancients/Patches/PaelsToothAndForgePatches.cs",
        "EZMicroBalanceCode/Ancients/Patches/PickupRewardPatches.cs",
        "EZMicroBalanceCode/Ancients/Patches/PrismaticGemPatches.cs",
        "EZMicroBalanceCode/Ancients/Patches/SealOfGoldPatches.cs",
        "EZMicroBalanceCode/Ancients/Patches/TurnOfferAndRestPatches.cs",
        "EZMicroBalanceCode/Ancients/Patches/VakuRewardPatches.cs",
        "EZMicroBalanceCode/Ancients/Patches/BrightestFlameExhaustDrawPatch.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaAncient.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaBlessingIds.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaFeatureGate.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaInitializer.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaOptionRelics.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaPowers.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaRunHook.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightEncounter.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightFeatureGate.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightOptionRelic.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightRunHook.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuTemptationCard.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviAncient.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviBlessingIds.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviCards.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviFeatureGate.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviInitializer.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviOptionRelics.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviPowers.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviRunHook.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaAncient.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaBlessingIds.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaCards.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaFeatureGate.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaInitializer.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaMapUiPatches.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaOptionRelics.cs",
        "EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaRunHook.cs",
        "EZMicroBalanceCode/Ascension/Patches/AscensionA20Patches.cs",
        "EZMicroBalanceCode/Ascension/Patches/AscensionA20RewardScreenPatches.cs",
        "EZMicroBalanceCode/Ascension/Patches/AscensionMapGenerationPatches.cs",
        "EZMicroBalanceCode/Ascension/Core/AscensionAssetPaths.cs",
        "EZMicroBalanceCode/Ascension/Combat/AscensionCombatModifierService.cs",
        "EZMicroBalanceCode/Ascension/Combat/AscensionCombatTracker.cs",
        "EZMicroBalanceCode/Ascension/Core/AscensionDiagnostics.cs",
        "EZMicroBalanceCode/Ascension/Core/AscensionExpansionConfig.cs",
        "EZMicroBalanceCode/Ascension/Core/AscensionFeatureGate.cs",
        "EZMicroBalanceCode/Ascension/Core/AscensionInitializer.cs",
        "EZMicroBalanceCode/Ascension/Map/A11MapGeometryProof.cs",
        "EZMicroBalanceCode/Ascension/Map/AscensionMapQuestMarker.cs",
        "EZMicroBalanceCode/Ascension/Map/AscensionMapService.cs",
        "EZMicroBalanceCode/Ascension/Patches/AscensionMapUiPatches.cs",
        "EZMicroBalanceCode/Ascension/Map/AscensionNodeMetadata.cs",
        "EZMicroBalanceCode/Ascension/Rewards/AscensionRewardService.cs",
        "EZMicroBalanceCode/Ascension/Core/AscensionSavedStateFields.cs",
        "EZMicroBalanceCode/Ascension/Patches/AscensionSelectionPatches.cs",
        "EZMicroBalanceCode/Ascension/Map/BannerRoomMapQuestMarker.cs",
        "EZMicroBalanceCode/Ascension/Rewards/BossSealDefinition.cs",
        "EZMicroBalanceCode/Ascension/Cards/BossSealCards.cs",
        "EZMicroBalanceCode/Ascension/Cards/RootCards.cs",
        "EZMicroBalanceCode/Ascension/Events/A20Courtyard.cs",
        "EZMicroBalanceCode/Ascension/Map/FiremarkedEliteMapQuestMarker.cs",
        "EZMicroBalanceCode/Ascension/Enchantments/FissionEnchantment.cs",
        "EZMicroBalanceCode/Ascension/Rewards/ForgeTokenService.cs",
        "EZMicroBalanceCode/Ascension/Core/MultiplayerDiagnostics.cs",
        "EZMicroBalanceCode/Ascension/Powers/BannerPowers.cs",
        "EZMicroBalanceCode/Ascension/Powers/BossSealPowers.cs",
        "EZMicroBalanceCode/Ascension/Powers/FiremarkPowers.cs",
        "EZMicroBalanceCode/Ascension/Relics/ForgeTokenRelic.cs",
        "EZMicroBalanceCode/Ascension/Enchantments/RoyalDecreeEnchantment.cs",
        "EZMicroBalanceCode/Ascension/Combat/RootBudCombatHook.cs",
        "EZMicroBalanceCode/Ascension/Rewards/RootDeckService.cs",
        "EZMicroBalanceCode/Ascension/Patches/RootRunHook.cs",
        "EZMicroBalanceCode/Config/EZMicroBalanceModConfig.cs",
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
                "ForgeTokenService.HasToken",
                "CardCmd.Upgrade(extraCard)",
                "ApplyAfterRestSiteHeal",
                "ApplyAfterRestSiteSmith",
                "DuplicateTokenGoldAmount"
            ],
            [
                "Firemarked Elite and Forge Token are implemented for the A12 Ascension-level gate.",
                "One Firemarked enemy receives Might, Giant, Forge Armor, or Constant Heal",
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
    public void CurrentStatusDocsUseLatestPackageHashes()
    {
        const string oldZipHash = "A96D592E5E244743D1DD0FC58035E34AC263743FFEC98F54CE8D4B31CD9C2432";
        const string oldDllHash = "A56CF2044A736DFF4E7BEACB55D63388C4DE72AC9C7A99418708D7F2776FE9D9";
        const string currentZipHash = "32076EE57C8FF3809F6733FED8D2C26DFF2D79488A2675083DA86BDF6D6E384B";
        const string currentDllHash = "D5852972FD5EB59CBE93B505ECEA341A30936EFFCB86A7DE2D7E1C4C4FB72BD4";
        const string currentPckHash = "CD5C9254887C30C449D195798A999E699B73CD6F62EF0D67C86F065FD074E05F";
        const string currentManifestHash = "659943569D01C1DDD8B5C351D763497F7FEE513AD0BB84903D05B69F8DBD1AB2";
        const string currentReadmeHash = "C9F19363848AEECD4B763BFF7BB2B75980A90BFE22358ACEC8FF5E9E5C129CE4";

        var currentStatusDocs = new[]
        {
            ReadRepoText("PROJECT_STATE.md"),
            ReadRepoText("docs", "issues.md"),
            ReadRepoText("docs", "dev-environment.md"),
            ReadRepoText("docs", "private-beta-verification-handoff.md"),
            ReadRepoText("docs", "private-beta-release-completion-audit.md"),
            ReadRepoText("docs", "release-checklist.md"),
            ReadRepoText("docs", "test-ready-completion-audit.md")
        };

        foreach (var doc in currentStatusDocs)
        {
            Assert.DoesNotContain(oldZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(oldDllHash, doc, StringComparison.Ordinal);
        }

        AssertSourceContains(
            ReadRepoText("docs", "issues.md"),
            currentZipHash,
            currentDllHash,
            currentPckHash,
            currentManifestHash,
            currentReadmeHash);
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
        Assert.Contains("one visible Exhaust keyword, no duplicate `Play: Exhaust` body text", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Rootblight II has one visible Exhaust keyword", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Blight Sprout has one visible Exhaust keyword", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("localized `[gold]Rootblight[/gold] added.` / `[gold]\u6839\u8680[/gold]\u5df2\u52a0\u5165\u3002` notice", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Rootblight I/II/III and Blight Sprout are implemented for A14/A15/A18 after the current standard-lobby selector expansion.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("## Live Evidence Protocol", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/spire-plus-live-session.ps1 -Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/check-spire-window-preflight.ps1 -OutFile <evidence-dir>\\window-preflight.json -RequireSpireForeground", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/audit-godot-log.ps1 -Path <evidence-dir>\\godot.log -OutFile <evidence-dir>\\godot-log-audit.json -FailOnHit", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/spire-plus-live-session.ps1 -Mode Restore -EvidenceDir <evidence-dir> -StopGameOnRestore -PreserveNewCurrentRunsOnRestore", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Covered desktop captures, wrong-surface captures, or sessions that never reach the target game surface do not satisfy Rootblight, Ascension, or gameplay rows.", manualChecklist, StringComparison.Ordinal);

        Assert.Contains("A11-A20 selection is now default-on in this private-beta multiplayer test candidate", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("`EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.", releaseChecklist, StringComparison.Ordinal);
    }

    [Fact]
    public void CardLocalizationStyleGuideIsIndexedAndCoversPreviewKeywordRules()
    {
        var guide = ReadRepoText("docs", "style", "card-localization-style-guide.md");
        var agents = ReadRepoText("AGENTS.md");
        var skill = ReadRepoText("docs", "skills", "sts2-godot-mod-development.md");
        var docsIndex = ReadRepoText("docs", "README.md");

        AssertSourceContains(
            guide,
            "CanonicalKeywords",
            "duplicate Exhaust",
            "[gold]",
            "card preview",
            "English and Simplified Chinese",
            "Rootblight = \u6839\u8680",
            "Blight Sprout / Root Bud = \u6839\u82bd",
            "HoverTipFactory.FromCard<T>()",
            "ModelDb",
            "Preview cards must");

        Assert.Contains("docs/style/card-localization-style-guide.md", agents, StringComparison.Ordinal);
        Assert.Contains("docs/style/card-localization-style-guide.md", skill, StringComparison.Ordinal);
        Assert.Contains("style/card-localization-style-guide.md", docsIndex, StringComparison.Ordinal);
    }

    [Fact]
    public void DocumentationOverhaulKeepsRepositoryRootDocsCanonicalAndArchiveMetadataVisible()
    {
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docsReadme = ReadRepoText("docs", "README.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");
        var overduePrompt = ReadRepoText("docs", "archive", "prompts", "2026-05", "codex-repo-overhaul-refactor-prompt.md");
        var urdaOvernightPrompt = ReadRepoText("docs", "archive", "prompts", "2026-05", "codex-urda-overnight-prompt.md");
        var urdaAddendum = ReadRepoText("docs", "archive", "prompts", "2026-05", "issues-urda-overnight-addendum.md");

        Assert.Contains("`../PROJECT_STATE.md`", docsReadme, StringComparison.Ordinal);
        Assert.Contains("`docs/archive/`", projectMap, StringComparison.Ordinal);
        Assert.Contains("docs/issues/waiting-tests.md", docInventory, StringComparison.Ordinal);
        Assert.Contains("# Archived prompt (2026-05)", overduePrompt, StringComparison.Ordinal);
        Assert.Contains("# Archived prompt (2026-05)", urdaOvernightPrompt, StringComparison.Ordinal);
        Assert.Contains("# Archived prompt (2026-05)", urdaAddendum, StringComparison.Ordinal);

        var rootDocs = Directory.GetFiles(RepoPath("docs"), "*.md", SearchOption.TopDirectoryOnly)
            .Select(path => Path.GetFileName(path))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var movedPrompt in new[]
        {
            "codex-repo-overhaul-refactor-prompt.md",
            "codex-urda-overnight-prompt.md",
            "issues-urda-overnight-addendum.md",
            "issues-waiting-tests.md"
        })
        {
            Assert.DoesNotContain(movedPrompt, rootDocs);
        }

        Assert.True(Directory.Exists(RepoPath("docs", "issues")), "docs/issues directory should exist.");
        Assert.True(Directory.Exists(RepoPath("docs", "archive", "prompts", "2026-05")), "Archived prompts directory should exist.");
        Assert.True(File.Exists(RepoPath("docs", "issues", "waiting-tests.md")), "Moved evidence queue should exist.");
        Assert.True(File.Exists(RepoPath("docs", "archive", "prompts", "2026-05", "codex-urda-overnight-prompt.md")), "Archived Urda prompt should exist.");
        Assert.True(File.Exists(RepoPath("docs", "archive", "prompts", "2026-05", "issues-urda-overnight-addendum.md")), "Archived Urda addendum should exist.");
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
    public void ActiveLocalizationJsonFilesAllParse()
    {
        var localizationFiles = Directory.GetFiles(RepoPath("EZMicroBalance", "localization"), "*.json", SearchOption.AllDirectories);
        var failures = new List<string>();

        foreach (var file in localizationFiles)
        {
            try
            {
                JsonDocument.Parse(File.ReadAllText(file, Encoding.UTF8)).Dispose();
            }
            catch (JsonException ex)
            {
                failures.Add($"{ToRepoRelativePath(file)}: {ex.Message}");
            }
        }

        Assert.True(failures.Count == 0, "Invalid localization JSON:" + Environment.NewLine + string.Join(Environment.NewLine, failures));
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

        return Path.GetExtension(path) is ".json" or ".png" or ".tscn";
    }

    [ReleaseArtifactFact]
    public void PackageStagingVersionedZipAndInstalledArtifactsHaveMatchingHashes()
    {
        var version = ManifestVersion();
        var packageName = $"SpirePlus-{version}";
        var installedDir = GamePath("mods", "EZMicroBalance");
        var stagingDir = RepoPath("publish", "package-staging", "EZMicroBalance");
        var versionedDir = RepoPath("publish", packageName, "EZMicroBalance");
        var zipPath = RepoPath("publish", $"{packageName}.zip");
        var legacyZipPath = RepoPath("publish", $"EZMicroBalance-{version}.zip");

        AssertPackageDirectory(stagingDir);
        AssertPackageDirectory(versionedDir);
        Assert.True(File.Exists(zipPath), $"Missing package zip: {zipPath}");
        Assert.False(File.Exists(legacyZipPath), $"Do not ship the player-facing archive under the technical id: {legacyZipPath}");

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
        var packageHash = Sha256(RepoPath("publish", $"SpirePlus-{ManifestVersion()}.zip"));
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
        Assert.Contains("Current normal Steam-client startup/log verification passed for the Spire Plus display-name package", docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.Contains("current-spire-plus-modsettings-20260513-111342", docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.Contains("RC1 normal Steam-client Mod Settings UI verification remains historical evidence for the old EZ Micro Balance display name", docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.Contains("Manual feature results are pending", docsByPath["docs/release-checklist.md"], StringComparison.Ordinal);
        Assert.DoesNotContain("private beta ready", combinedDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("release ready", combinedDocs, StringComparison.OrdinalIgnoreCase);
    }

    [ReleaseArtifactFact]
    public void PrivateBetaVerificationHandoffCarriesCurrentArtifactsAndManualBlockers()
    {
        var packageHash = Sha256(RepoPath("publish", $"SpirePlus-{ManifestVersion()}.zip"));
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
        Assert.Contains("Normal Steam-client startup/log verification passed for the current Spire Plus display-name package", handoff, StringComparison.Ordinal);
        Assert.Contains("Normal Steam-client Mod Settings UI verification now has a current `Spire Plus` list screenshot", handoff, StringComparison.Ordinal);
        Assert.Contains("current-spire-plus-modsettings-20260513-111342", handoff, StringComparison.Ordinal);
        Assert.Contains("Live Ancient reward gameplay, broader save/load, disable-gameplay, and multiplayer checks are still pending", handoff, StringComparison.Ordinal);
        Assert.Contains("A11-A20 selection is now default-on in this private-beta multiplayer test candidate", handoff, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1", handoff, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1", handoff, StringComparison.Ordinal);
        Assert.Contains("EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required", handoff, StringComparison.Ordinal);
        Assert.Contains("docs/features/ascension-11-20/multiplayer-test-runbook.md", handoff, StringComparison.Ordinal);
        Assert.Contains("scripts/audit-godot-log.ps1 -Path <copied godot.log>", handoff, StringComparison.Ordinal);
        Assert.Contains("godot-log-audit.json", handoff, StringComparison.Ordinal);
        Assert.Contains("Live co-op selection and desync verification are still pending", handoff, StringComparison.Ordinal);
        Assert.Contains("Resolved for this candidate: `EZMicroBalance.json` author is `wenhuorongbing-netizen`, taken from the local Git user name.", handoff, StringComparison.Ordinal);
        Assert.Contains("Rootblight I/II/III and Blight Sprout use original generated portrait art at the documented per-card filenames.", handoff, StringComparison.Ordinal);
        Assert.Contains("Live in-game visual verification is still pending.", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("AUTHOR_NAME_REPLACE_ME", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("This remains a release blocker unless the user either provides the desired author name or explicitly accepts that placeholder", handoff, StringComparison.Ordinal);
        Assert.Contains("A1.05.01 (`ae910e8`) is a broad engineering/review commit", handoff, StringComparison.Ordinal);
        Assert.Contains("not only a handoff and `ReleaseCoverageGuardTests` update", handoff, StringComparison.Ordinal);
        Assert.Contains("Ascension source directory reorganization", handoff, StringComparison.Ordinal);
        Assert.Contains("settings_ui` localization", handoff, StringComparison.Ordinal);
        Assert.Contains("manifest BaseLib `v3.1.2` dependency floor", handoff, StringComparison.Ordinal);
        Assert.Contains("Current git status before", handoff, StringComparison.Ordinal);
        Assert.Contains("Current git log -1 --oneline --decorate", handoff, StringComparison.Ordinal);
        Assert.Contains("Pre-commit local cleanup status summary", handoff, StringComparison.Ordinal);
        Assert.Contains("M EZMicroBalanceCode/Ascension/Rewards/RootDeckService.cs", handoff, StringComparison.Ordinal);
        Assert.Contains("M export_presets.cfg", handoff, StringComparison.Ordinal);
        Assert.Contains("?? EZMicroBalance/scenes/", handoff, StringComparison.Ordinal);
        Assert.Contains("?? docs/test-ready-completion-audit.md", handoff, StringComparison.Ordinal);
        Assert.Contains("M tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("b82023c", handoff, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("f201508", handoff, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("96bfa50", handoff, StringComparison.Ordinal);
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
            "InstalledUrdaUsesCustomAncientAssetPaths",
            "PrismaticGemRewardBannerContractMatchesInstalledGameApi",
            "PackageStagingVersionedZipAndInstalledArtifactsHaveMatchingHashes",
            "CurrentDocsMatchReleaseHashesAndAvoidPinnedStaleTestTotals",
            "PrivateBetaVerificationHandoffCarriesCurrentArtifactsAndManualBlockers",
            "ActiveCoverArtAndInactiveModRealPolicyMatchExportPckAndPackage",
            "ExportedResourcesInstalledPckAndPackagePckStayInParity",
            "CurrentReleaseHashClaimsMatchInstalledStagingVersionedAndZipArtifacts",
            "RecentSmokeLogSupportsControlledSmokeClaims",
            "DisabledSpirePlusPlugOffEvidenceSupportsDocs"
        })
        {
            Assert.Contains($"[ReleaseArtifactFact]\n    public void {methodName}", testSource, StringComparison.Ordinal);
        }

        Assert.Contains("EZMB_RUN_RELEASE_ARTIFACT_TESTS=1", testPlan, StringComparison.Ordinal);
        Assert.Contains("skipped in normal developer test runs", testPlan, StringComparison.Ordinal);
        Assert.Contains("Release artifact tests are opt-in", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("EZMB_RUN_RELEASE_ARTIFACT_TESTS=1", handoff, StringComparison.Ordinal);
    }

    [Fact]
    public void IssuesIndexIsCompactAndRoutesUrdaDetailsToFeatureIssueDocs()
    {
        var issues = ReadRepoText("docs", "issues.md");
        var urdaIssueIndex = ReadRepoText("docs", "issues", "urda.md");
        var logAuditScript = ReadRepoText("scripts", "audit-godot-log.ps1");

        AssertSourceContains(
            logAuditScript,
            "Creature\\.get_ShowsInfiniteHp",
            "BaseLib\\.Patches\\.UI\\.HealthBarForecastPatch",
            "\\[ERROR\\]",
            "TypeLoadException",
            "MissingMethodException",
            "EZMicroBalance error/exception",
            "FailOnHit",
            "ConvertTo-Json");
        Assert.DoesNotContain("BaseLib.*(?:patch|patches).*(?:failed|failure|exception)", logAuditScript, StringComparison.Ordinal);

        var godotErrorPatternMatch = Regex.Match(logAuditScript, @"Name = 'Godot ERROR line'; Pattern = '([^']+)'");
        Assert.True(godotErrorPatternMatch.Success, "Missing Godot ERROR line signature pattern.");
        var godotErrorPattern = godotErrorPatternMatch.Groups[1].Value;
        Assert.Matches(godotErrorPattern, "[ERROR] Mod manifest bad");
        Assert.Matches(godotErrorPattern, "ERROR Mod manifest bad");
        Assert.Matches(godotErrorPattern, "[Godot] ERROR Mod manifest bad");
        Assert.DoesNotMatch(godotErrorPattern, "[INFO] [BaseLib] Applied 177 patches successfully, 0 failed");

        Assert.Contains("## Active blockers", issues, StringComparison.Ordinal);
        Assert.Contains("## Issue detail links", issues, StringComparison.Ordinal);
        Assert.Contains("docs/issues/urda.md", issues, StringComparison.Ordinal);
        Assert.Contains("docs/issues/waiting-tests.md", issues, StringComparison.Ordinal);
        Assert.Contains("| ID |", issues, StringComparison.Ordinal);
        Assert.Contains("| URDA-PROTOTYPE |", issues, StringComparison.Ordinal);
        Assert.DoesNotContain("Status: resolved", issues, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\uFFFD", issues, StringComparison.Ordinal);

        Assert.Contains("Urda is default-on", urdaIssueIndex, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("EZMB_DISABLE_URDA", urdaIssueIndex, StringComparison.Ordinal);
        Assert.Contains("prototype", urdaIssueIndex, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Lotha is now default-on in the active test slice", urdaIssueIndex, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Vakuu fight", urdaIssueIndex, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("save/load", urdaIssueIndex, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AncientExpansionV22DocsTrackActiveSourceReadySlices()
    {
        var issues = ReadRepoText("docs", "issues.md");
        var v22Issues = ReadRepoText("docs", "issues", "ancient-expansion-v2.2.md");
        var featureReadme = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "README.md");
        var sourceDesign = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "source-design.md");
        var safetyRules = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "card-and-power-safety-rules.md");
        var riskRegister = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "risk-register.md");
        var projectState = ReadRepoText("PROJECT_STATE.md");
        var docsIndex = ReadRepoText("docs", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var featuresIndex = ReadRepoText("docs", "features", "README.md");
        var activeExpansionSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion");

        Assert.Contains("docs/issues/ancient-expansion-v2.2.md", issues, StringComparison.Ordinal);
        Assert.Contains("morvi_forbidden_loan", issues, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("lotha_death_reprieve", issues, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Fight Vakuu", issues, StringComparison.OrdinalIgnoreCase);

        Assert.Contains("Lotha is default-on", v22Issues, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Vakuu fight", v22Issues, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("save/load", v22Issues, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-DESIGN-DOC-INGEST", v22Issues, StringComparison.Ordinal);
        Assert.Contains("ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-CARD-POWER-SAFETY-RULES", v22Issues, StringComparison.Ordinal);
        Assert.Contains("ISSUE-2026-05-12-MORVI-V22-PLANNING", v22Issues, StringComparison.Ordinal);
        Assert.Contains("ISSUE-2026-05-12-LOTHA-V22-PLANNING", v22Issues, StringComparison.Ordinal);
        Assert.Contains("ISSUE-2026-05-12-VAKUU-FIGHT-V22-PLANNING", v22Issues, StringComparison.Ordinal);
        Assert.Contains("ISSUE-2026-05-12-URDA-V22-ALIGNMENT", v22Issues, StringComparison.Ordinal);
        Assert.Contains("ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-MILESTONE-GATES", v22Issues, StringComparison.Ordinal);

        AssertSourceContains(
            featureReadme,
            "default-on Morvi v2.2",
            "Lotha is default-on",
            "Vakuu fight",
            "Live gameplay and save/load verification for current Urda remains pending");
        AssertSourceContains(
            sourceDesign,
            "Seedbed",
            "Humus Pact",
            "Molting",
            "Moss Map",
            "Trial Branch",
            "Shallow-Root Relic",
            "Rooted Route",
            "After the Rain",
            "Root-Sight",
            "Seed Bank",
            "Morvi is default-on",
            "Lotha is default-on",
            "Vakuu fight");
        AssertSourceContains(
            safetyRules,
            "Power cards are not copied, extra-played, or replayed by default",
            "Extra-played or copied cards must not recursively trigger the same blessing",
            "Each Morvi or Lotha blessing");
        AssertSourceContains(
            riskRegister,
            "Power-card extra-play exploit",
            "Death-interrupt complexity",
            "Reward UI softlock",
            "Multiplayer ownership/desync",
            "Save/load persistence");

        Assert.Contains("ancient-expansion-v2.2", projectState, StringComparison.Ordinal);
        Assert.Contains("default-on Morvi", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Vakuu fight", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("save/load", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("features/ancient-expansion-v2.2/README.md", docsIndex, StringComparison.Ordinal);
        Assert.Contains("docs/features/ancient-expansion-v2.2/README.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("ancient-expansion-v2.2/README.md", featuresIndex, StringComparison.Ordinal);

        Assert.Contains("MorviFeatureGate", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("EZMB_DISABLE_MORVI", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_DISABLE_MORVI", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("EZMB_FORCE_MORVI_BLESSING", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("LothaFeatureGate", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("EZMB_DISABLE_LOTHA", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_DISABLE_LOTHA", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("VakuuFightFeatureGate", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("EZMB_DISABLE_VAKUU_FIGHT", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_FORCE_VAKUU_FIGHT", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("EventModel.Resume", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("EzmbVakuuTrialEncounter", activeExpansionSource, StringComparison.Ordinal);
    }

    [Fact]
    public void MorviV22IsDefaultOnGatedLocalizedAndPowerSafe()
    {
        var mainFile = ReadRepoText("EZMicroBalanceCode", "MainFile.cs");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var morviGate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviFeatureGate.cs");
        var morviInitializer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviInitializer.cs");
        var morviAncient = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviAncient.cs");
        var morviCards = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviCards.cs");
        var morviOptionRelics = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviOptionRelics.cs");
        var morviPowers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviPowers.cs");
        var morviBlessings = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingIds.cs");
        var morviRunHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviRunHook.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var zhsCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");

        Assert.Contains("MorviInitializer.Initialize();", mainFile, StringComparison.Ordinal);
        Assert.Contains("MorviStateKey", savedFields, StringComparison.Ordinal);
        Assert.Contains("EZMicroBalanceMorviStateKey", savedFields, StringComparison.Ordinal);
        Assert.Contains("MorviBorrowedAncientCard", savedFields, StringComparison.Ordinal);
        Assert.Contains("MorviOpenBookSealedCard", savedFields, StringComparison.Ordinal);

        AssertSourceContains(
            morviGate,
            "EZMB_DISABLE_MORVI",
            "SPIREPLUS_DISABLE_MORVI",
            "EZMB_FORCE_ANCIENT",
            "SPIREPLUS_FORCE_ANCIENT",
            "EZMB_FORCE_MORVI_BLESSING",
            "SPIREPLUS_FORCE_MORVI_BLESSING",
            "ShouldForceMorvi",
            "!IsTruthy(Environment.GetEnvironmentVariable(DisableEnvironmentVariable))");
        Assert.DoesNotContain("return IsTruthy(value);", morviGate, StringComparison.Ordinal);

        AssertSourceContains(
            morviInitializer,
            "ModHelper.SubscribeForRunStateHooks",
            "default-on",
            "ModelDb.GetById<MorviRunHook>");
        AssertSourceContains(
            morviAncient,
            "CustomAncientModel",
            "HarmonyPatch(typeof(Hive), nameof(Hive.GetUnlockedAncients))",
            "MorviFeatureGate.IsMorviEnabled(unlockState)",
            "MorviFeatureGate.ShouldForceMorvi",
            "ModelDb.AncientEvent<EzmbMorvi>()",
            "ExpectedInitialOptionCount = 3",
            "options.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList()",
            "OptionWithRelic<MorviForbiddenLoanOptionRelic>",
            "MorviBlessingIds.ForbiddenLoan",
            "MorviBlessingIds.MisprintPress",
            "MorviBlessingIds.RedInkOverdraft",
            "MorviBlessingIds.OverdueLibrary",
            "MorviBlessingIds.OpenBookExam",
            "MorviBlessingIds.Paperstorm",
            "MorviBlessingIds.BlueprintProof",
            "MorviBlessingIds.DebtSettlement",
            "MorviAssetPaths.MapIcon",
            "MorviAssetPaths.BackgroundScene");
        Assert.DoesNotContain("Glory.GetUnlockedAncients", morviAncient, StringComparison.Ordinal);

        AssertSourceContains(
            morviBlessings,
            "morvi_forbidden_loan",
            "morvi_misprint_press",
            "morvi_red_ink_overdraft",
            "morvi_overdue_library",
            "morvi_open_book_exam",
            "morvi_paperstorm",
            "morvi_blueprint_proof",
            "morvi_debt_settlement");
        AssertSourceContains(
            morviRunHook,
            "public override bool ShouldReceiveCombatHooks => true",
            "BeforeCombatStart",
            "player.IsActiveForHooks",
            "CardType.Attack or CardType.Skill",
            "!card.IsClone",
            "CardCmd.Upgrade(card, CardPreviewStyle.None)",
            "CardCmd.Downgrade(card)",
            "ForbiddenLoanKeepGoldCost = 180",
            "RedInkOverdraftGoldPerDebt = 12",
            "OverdueLibraryPageCount = 3",
            "OpenBookDraw = 5",
            "PaperstormWastePaperCount = 4",
            "BlueprintProofStacks = 3",
            "DebtSettlementImmediateGold = 220",
            "DebtSettlementStartingDebt = 320",
            "DebtSettlementCombatDue = 40",
            "maximumNonlethalHpLoss = Math.Max(0m, player.Creature.CurrentHp - 1m)",
            "visibleDebtCount = player.Creature.GetPower<MorviOverdraftPower>()?.Amount ?? 0",
            "FindOpenBookSealedCards(player, combatState)",
            "DebtRemaining = Math.Max(0, progress.DebtRemaining - due)");
        Assert.DoesNotContain("CreateClone", morviRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryAddGeneratedCardToCombat(copy", morviRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("CardCmd.AutoPlay", morviRunHook, StringComparison.Ordinal);

        AssertSourceContains(
            morviCards,
            "MorviArchiveDrawPage",
            "MorviArchiveVeilPage",
            "MorviArchiveBurnPage",
            "MorviArchiveDiscountPage",
            "MorviArchiveBraveryPage",
            "MorviArchiveDexterityPage",
            "MorviRedInkOverdraftCard",
            "MorviWastePaper");
        AssertSourceContains(
            morviOptionRelics,
            "MorviForbiddenLoanOptionRelic",
            "MorviDebtSettlementOptionRelic",
            "IsAllowed(IRunState runState) => false");
        AssertSourceContains(
            morviPowers,
            "MorviDebtPower",
            "MorviProofreadPower",
            "MorviOpenBookPower",
            "MorviOverdraftPower",
            "MorviPaperstormPower");

        foreach (var key in new[]
        {
            "EZMB_MORVI.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_forbidden_loan.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_misprint_press.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_red_ink_overdraft.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_overdue_library.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_open_book_exam.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_paperstorm.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_blueprint_proof.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.title"
        })
        {
            Assert.True(engAncients.ContainsKey(key), $"Missing English Morvi localization: {key}");
            Assert.True(zhsAncients.ContainsKey(key), $"Missing zhs Morvi localization: {key}");
        }

        foreach (var key in new[]
        {
            "EZMB_MORVI_ARCHIVE_DRAW_PAGE.title",
            "EZMB_MORVI_ARCHIVE_VEIL_PAGE.title",
            "EZMB_MORVI_ARCHIVE_BURN_PAGE.title",
            "EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE.title",
            "EZMB_MORVI_ARCHIVE_BRAVERY_PAGE.title",
            "EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE.title",
            "EZMB_MORVI_RED_INK_OVERDRAFT.title",
            "EZMB_MORVI_WASTE_PAPER.title"
        })
        {
            Assert.True(engCards.ContainsKey(key), $"Missing English Morvi card localization: {key}");
            Assert.True(zhsCards.ContainsKey(key), $"Missing zhs Morvi card localization: {key}");
        }
    }
    [Fact]
    public void LothaIsDefaultOnGatedLocalizedAndPowerSafe()
    {
        var mainFile = ReadRepoText("EZMicroBalanceCode", "MainFile.cs");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var lothaGate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaFeatureGate.cs");
        var lothaAncient = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaAncient.cs");
        var lothaRunHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");
        var lothaOptionRelics = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaOptionRelics.cs");
        var lothaPower = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaPowers.cs");
        var lothaScene = ReadRepoText("EZMicroBalance", "scenes", "events", "background_scenes", "ezmb_lotha.tscn");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var engPowers = JsonStringMap("EZMicroBalance", "localization", "eng", "powers.json");
        var zhsPowers = JsonStringMap("EZMicroBalance", "localization", "zhs", "powers.json");

        Assert.Contains("LothaInitializer.Initialize();", mainFile, StringComparison.Ordinal);
        AssertSourceContains(savedFields, "SavedSpireField<Player, string> LothaStateKey", "SavedSpireField<CardModel, string> LothaDeckStateKey", "SavedSpireField<CardModel, bool> LothaMirrorRebuttalCard");
        AssertSourceContains(lothaGate, "EZMB_DISABLE_LOTHA", "SPIREPLUS_DISABLE_LOTHA", "EZMB_FORCE_ANCIENT", "SPIREPLUS_FORCE_ANCIENT", "EZMB_FORCE_LOTHA_BLESSING", "SPIREPLUS_FORCE_LOTHA_BLESSING", "ShouldForceLotha", "!IsTruthy");
        AssertSourceContains(lothaAncient, "CustomAncientModel", "HarmonyPatch(typeof(Glory), nameof(Glory.GetUnlockedAncients))", "LothaFeatureGate.ShouldForceLotha", "ExpectedInitialOptionCount = 3", "options.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList()", "OptionWithRelic<LothaMirrorRebuttalOptionRelic>", "OptionWithRelic<LothaPublicEvidenceOptionRelic>", "CardSelectCmd.FromDeckGeneric", "LothaBlessingService.MarkMirrorRebuttalCard", "HoverTipFactory.FromPower<LothaPresumptionPower>()", "HoverTipFactory.FromPower<LothaVerdictPower>()", "HoverTipFactory.FromPower<LothaDeathReprievePower>()", "HoverTipFactory.FromPower<LothaEnlightenmentPower>()", "HoverTipFactory.Static(StaticHoverTip.Energy)", "HoverTipFactory.Static(StaticHoverTip.Block)", "LothaAssetPaths.MapIcon", "LothaAssetPaths.RunHistoryIcon", "LothaAssetPaths.BackgroundScene");
        AssertSourceContains(lothaRunHook, "ShouldReceiveCombatHooks => true", "public override int ModifyCardPlayCount", "public override bool ShouldPlay", "public override Task AfterTurnEnd", "public override Task AfterDamageReceived", "public override bool TryModifyRewardsLate", "public override bool TryModifyEnergyCostInCombat", "public override bool TryModifyStarCost", "public override Task AfterCombatEnd", "ModifyPowerAmountGiven", "TryModifyPowerAmountReceived", "AfterPowerAmountChanged", "LothaExtraPlayCount = 2", "SingleSentenceRemainingPlayLimit = 4", "MirrorRebuttalPowerFallbackEnergy = 2", "MirrorRebuttalPowerFallbackCards = 2", "MirrorHallEchoExtraPlayCount = 1", "ClosedCourtEnergy = 4", "ClosedCourtDiscountCount = 3", "PresumptionCards = 2", "PresumptionEnergy = 1", "PresumptionBlock = 8", "PresumptionHpLoss = 8", "DeferredVerdictTurn = 4", "DeferredVerdictStacks = 3", "DeferredVerdictEnergy = 4", "DeferredVerdictCards = 4", "DeferredVerdictExtraPlayCount = 1", "DeferredVerdictEarlyEndHeal = 4", "DeathReprieveCards = 10", "DeathReprieveEnergy = 10", "PowerFallbackCards = 1", "IsPowerReplacementCostZeroCard", "PowerReplacementCardPendingBenefit", "cost 0 and draw 1", "CardType.Attack or CardType.Skill", "!card.IsClone", "cardPlay.IsAutoPlay", "card.Type == CardType.Power && !card.IsClone", "ApplyPowerReplacementBenefit", "RecordMirrorHallEchoType", "PowerCmd.Apply<LothaPresumptionPower>", "PowerCmd.Apply<LothaVerdictPower>", "PowerCmd.Apply<LothaEnlightenmentPower>", "PowerCmd.Decrement(verdict)", "PowerCmd.ModifyAmount(choiceContext, enlightenment, -consumed", "CreatureCmd.Heal(player.Creature, DeferredVerdictEarlyEndHeal", "CreatureCmd.Damage(", "rewards.RemoveAll(reward => reward is CardReward)", "IsPublicEvidenceDebuffApplication", "IsPublicEvidenceExcludedDamageDebuff", "power is PoisonPower", "power.GetTypeForAmount(amount) == PowerType.Debuff", "ShouldDieLate(Creature creature)", "ShouldDie(Creature creature)", "AfterPreventingDeath(Creature creature)", "CreatureCmd.Kill(player.Creature, force: true)", "AncientSavedStateFields.LothaStateKey", "AncientSavedStateFields.LothaDeckStateKey", "AncientSavedStateFields.LothaMirrorRebuttalCard");
        Assert.DoesNotContain("MirrorRebuttalMinimumBlock", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryReplayMirrorRebuttalCopy", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryCreateMirrorHallEcho", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryAddGeneratedCardToCombat(copy", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("DeathReprieveHealPercent", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryBurstDeferredVerdict", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("DeferredVerdictDamagePerStack", lothaRunHook, StringComparison.Ordinal);
        Assert.Equal(8, Regex.Matches(lothaOptionRelics, @"\[Pool\(typeof\(SharedRelicPool\)\)\]").Count);
        AssertSourceContains(lothaPower, "internal sealed class LothaVerdictPower", "internal sealed class LothaPresumptionPower", "internal sealed class LothaDeathReprievePower", "internal sealed class LothaEnlightenmentPower", "PowerType.Buff", "PowerStackType.Counter", "PowerStackType.Single", "LothaAssetPaths.VerdictPowerIcon", "LothaAssetPaths.PresumptionPowerIcon", "LothaAssetPaths.DeathReprievePowerIcon", "LothaAssetPaths.EnlightenmentPowerIcon");
        AssertSourceContains(lothaScene, "[node name=\"EzmbLothaBackground\" type=\"Control\"]", "type=\"TextureRect\"", "ezmb_lotha.png");

        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-EZMB_LOTHA.title",
                "EZMICROBALANCE-EZMB_LOTHA.epithet",
                "EZMICROBALANCE-EZMB_LOTHA.talk.firstVisitEver.0-0.ancient",
                "EZMICROBALANCE-EZMB_LOTHA.talk.ANY.0-0r.ancient",
                "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.title",
                "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.selectionScreenPrompt",
                "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.description",
                "EZMB_LOTHA.pages.INITIAL.options.lotha_death_reprieve.description",
                "EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"
            ],
            engAncients,
            zhsAncients,
            "Lotha Ancient localization");
        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC.title",
                "EZMICROBALANCE-LOTHA_MIRROR_HALL_ECHO_OPTION_RELIC.title",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_OPTION_RELIC.title",
                "EZMICROBALANCE-LOTHA_CLOSED_COURT_OPTION_RELIC.title",
                "EZMICROBALANCE-LOTHA_DEFERRED_VERDICT_OPTION_RELIC.title",
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_OPTION_RELIC.title",
                "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_OPTION_RELIC.title",
                "EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.title"
            ],
            engRelics,
            zhsRelics,
            "Lotha option relic localization");
        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-LOTHA_VERDICT_POWER.title",
                "EZMICROBALANCE-LOTHA_VERDICT_POWER.description",
                "EZMICROBALANCE-LOTHA_VERDICT_POWER.smartDescription",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.title",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.description",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.smartDescription",
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.title",
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.description",
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.smartDescription",
                "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.title",
                "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.description",
                "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.smartDescription"
            ],
            engPowers,
            zhsPowers,
            "Lotha power localization");

        Assert.Contains("[gold]Attack[/gold]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]攻击牌[/gold]", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description"], StringComparison.Ordinal);
        Assert.Contains("[blue]4[/blue]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]能力牌[/gold]", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"], StringComparison.Ordinal);
        Assert.Contains("double its stacks", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.OrdinalIgnoreCase);
        Assert.Contains("non-damaging [gold]negative status[/gold]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Poison[/gold], damage-over-time, and countdown damage do not count", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.Ordinal);
        Assert.Contains("层数翻倍", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.Ordinal);
        Assert.Contains("非伤害类[gold]负面状态[/gold]", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]中毒[/gold]、持续伤害和倒计时伤害不计", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Verdict[/gold]", engPowers["EZMICROBALANCE-LOTHA_VERDICT_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]裁决[/gold]", zhsPowers["EZMICROBALANCE-LOTHA_VERDICT_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Innocent[/gold]", engPowers["EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]无罪[/gold]", zhsPowers["EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Death Reprieve[/gold]", engPowers["EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]死刑缓期[/gold]", zhsPowers["EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Enlightenment[/gold]", engPowers["EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]开悟[/gold]", zhsPowers["EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Energy[/gold]", engRelics["EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]能量[/gold]", zhsRelics["EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC.description"], StringComparison.Ordinal);
        foreach (var relativePath in new[]
        {
            "EZMicroBalance/images/events/ezmb_lotha.png",
            "EZMicroBalance/images/ancients/lotha/ezmb_lotha_map_icon.png",
            "EZMicroBalance/images/ancients/lotha/ezmb_lotha_map_icon_outline.png",
            "EZMicroBalance/images/ancients/lotha/ezmb_lotha_run_history_icon.png",
            "EZMicroBalance/images/ancients/lotha/ezmb_lotha_run_history_icon_outline.png",
            "EZMicroBalance/images/ancients/lotha/options/lotha_mirror_rebuttal.png",
            "EZMicroBalance/images/ancients/lotha/options/lotha_mirror_hall_echo.png",
            "EZMicroBalance/images/ancients/lotha/options/lotha_presumption.png",
            "EZMicroBalance/images/ancients/lotha/options/lotha_closed_court.png",
            "EZMicroBalance/images/ancients/lotha/options/lotha_deferred_verdict.png",
            "EZMicroBalance/images/ancients/lotha/options/lotha_death_reprieve.png",
            "EZMicroBalance/images/ancients/lotha/options/lotha_single_sentence.png",
            "EZMicroBalance/images/ancients/lotha/options/lotha_public_evidence.png",
            "EZMicroBalance/images/powers/lotha_verdict.png",
            "EZMicroBalance/scenes/events/background_scenes/ezmb_lotha.tscn"
        })
        {
            Assert.True(File.Exists(RepoPath(relativePath.Split('/'))), $"Missing Lotha resource: {relativePath}");
            Assert.Contains($"res://{relativePath}", exportPreset, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void VakuuFightIsSinglePlayerGatedLocalizedAndResumeSafe()
    {
        var gate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs");
        var patch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");
        var encounter = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightEncounter.cs");
        var optionRelic = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightOptionRelic.cs");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var engEncounters = JsonStringMap("EZMicroBalance", "localization", "eng", "encounters.json");
        var zhsEncounters = JsonStringMap("EZMicroBalance", "localization", "zhs", "encounters.json");

        AssertSourceContains(
            gate,
            "EZMB_DISABLE_VAKUU_FIGHT",
            "SPIREPLUS_DISABLE_VAKUU_FIGHT",
            "EZMB_FORCE_ANCIENT",
            "SPIREPLUS_FORCE_ANCIENT",
            "EZMB_FORCE_VAKUU_FIGHT",
            "SPIREPLUS_FORCE_VAKUU_FIGHT",
            "ShouldForceVakuu",
            "ShouldForceFight",
            "runState.Players.Count == 1");
        AssertSourceContains(
            patch,
            "[HarmonyPatch(typeof(Glory), nameof(Glory.GetUnlockedAncients))]",
            "ModelDb.AncientEvent<MegaCrit.Sts2.Core.Models.Events.Vakuu>()",
            "[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Models.Events.Vakuu), \"GenerateInitialOptions\")]",
            "VakuuFightFeatureGate.ShouldForceFight",
            "EventOption.FromRelic",
            "ThatWillKillPlayerIf",
            "[HarmonyPatch(typeof(EventModel), nameof(EventModel.Resume))]",
            "SetEventState",
            "RunManager.Instance.EnterRoomWithoutExitingCurrentRoom",
            "ModelDb.Encounter<EzmbVakuuTrialEncounter>()",
            "Nonupeipe",
            "Tanx",
            "RelicCmd.Obtain(relic, owner)",
            "vakuu.StartPreFinished()");
        Assert.DoesNotContain("LinkedRewardSet", patch, StringComparison.Ordinal);
        Assert.DoesNotContain("ExtraRewards", patch, StringComparison.Ordinal);
        AssertSourceContains(
            encounter,
            "CustomEncounterModel",
            "base(RoomType.Event, autoAdd: false)",
            "ShouldGiveRewards => false",
            "OwlMagistrate",
            "IsValidForAct(ActModel act) => false");
        AssertSourceContains(
            optionRelic,
            "[Pool(typeof(SharedRelicPool))]",
            "PackedIconPath => VakuuFightAssetPaths.OptionIcon",
            "IsAllowed(IRunState runState) => false",
            "IsAllowedAtNeow(Player player) => false",
            "IsAllowedInShops => false");

        AssertLocalizedKeys(
            [
                "VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.title",
                "VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description",
                "EZMB_VAKUU_FIGHT.pages.VICTORY.description",
                "EZMB_VAKUU_FIGHT.pages.DONE.description"
            ],
            engAncients,
            zhsAncients,
            "Vakuu fight Ancient localization");
        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.title",
                "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description",
                "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.flavor"
            ],
            engRelics,
            zhsRelics,
            "Vakuu fight option relic localization");
        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-EZMB_VAKUU_TRIAL_ENCOUNTER.title",
                "EZMICROBALANCE-EZMB_VAKUU_TRIAL_ENCOUNTER.loss"
            ],
            engEncounters,
            zhsEncounters,
            "Vakuu fight encounter localization");

        Assert.True(
            File.Exists(RepoPath("EZMicroBalance", "images", "ancients", "vakuu", "options", "vakuu_fight.png")),
            "Missing Vakuu fight option art.");
        Assert.Contains("res://EZMicroBalance/images/ancients/vakuu/options/vakuu_fight.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/localization/eng/encounters.json", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/localization/zhs/encounters.json", exportPreset, StringComparison.Ordinal);
    }

    [Fact]
    public void ActiveAncientExpansionEventArtIsExportedAndDocumented()
    {
        var exportPreset = ReadRepoText("export_presets.cfg");
        var artDirection = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "art-direction.md");
        var v22Issues = ReadRepoText("docs", "issues", "ancient-expansion-v2.2.md");
        var workLog = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "work-log.md");

        var morviPng = RepoPath("EZMicroBalance", "images", "events", "ezmb_morvi.png");

        Assert.True(File.Exists(morviPng), "Morvi event art should exist now that Morvi is active.");
        Assert.Contains("res://EZMicroBalance/images/events/ezmb_morvi.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/scenes/events/background_scenes/ezmb_morvi.tscn", exportPreset, StringComparison.Ordinal);
        Assert.True(
            new FileInfo(morviPng).Length > 1_000_000,
            "Morvi event art must not regress to a small geometric placeholder.");
        AssertSmallUiPngHasAlpha(
            RepoPath("EZMicroBalance", "images", "ancients", "morvi", "ezmb_morvi_map_icon.png"),
            "Morvi map icon must remain a readable transparent UI resource.");
        Assert.True(File.Exists(RepoPath("EZMicroBalance", "images", "events", "ezmb_lotha.png")), "Lotha event art should exist now that Lotha is active.");
        Assert.Contains("res://EZMicroBalance/images/events/ezmb_lotha.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/scenes/events/background_scenes/ezmb_lotha.tscn", exportPreset, StringComparison.Ordinal);
        Assert.True(
            new FileInfo(RepoPath("EZMicroBalance", "images", "events", "ezmb_lotha.png")).Length > 1_000_000,
            "Lotha event art must not regress to the small geometric placeholder.");
        AssertSmallUiPngHasAlpha(
            RepoPath("EZMicroBalance", "images", "ancients", "lotha", "ezmb_lotha_map_icon.png"),
            "Lotha map icon must remain a readable transparent UI resource.");
        foreach (var optionArt in Directory.GetFiles(RepoPath("EZMicroBalance", "images", "ancients", "lotha", "options"), "*.png"))
        {
            AssertSmallUiPngHasAlpha(optionArt, $"{optionArt} must remain a readable transparent option icon.");
        }

        AssertSourceContains(
            artDirection,
            "Active Morvi event art uses `art_pipeline/generated/ancient_morvi_bg_v1_v001.png`",
            "Active event art now uses the first user-preferred mirror-ensemble preview crop",
            "Active event art is a 2.13:1 reframe of the user-accepted Urda middle-draft",
            "Final browser GPTimage2 small art generated this pass",
            "Urda, Morvi, and Lotha option/icon art uses browser ChatGPT/GPTimage2 rebuilt transparent PNGs",
            "Custom card portraits now use browser GPTimage2 rebuilt files",
            "No `generic_temporary` or `final_required_before_release` art blockers remain",
            "Do not use placeholder art for Morvi or future active Ancients just to satisfy the export list.");
        AssertSourceContains(
            v22Issues,
            "Morvi is default-on",
            "Lotha is default-on");
        Assert.Contains("Copied the generated Morvi background", workLog, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Promoted the first user-preferred Lotha mirror-ensemble crop", workLog, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Reframed the user-accepted Urda root-mother background to 1831x859", workLog, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UrdaIsDefaultOnDisableableAndBlessingSliceSourceBacked()
    {
        var urdaGate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaFeatureGate.cs");
        var urdaAncient = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaAncient.cs");
        var urdaBlessings = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingIds.cs");
        var urdaCards = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaCards.cs");
        var urdaInitializer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaInitializer.cs");
        var urdaMapUiPatches = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaMapUiPatches.cs");
        var urdaOptionRelics = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaOptionRelics.cs");
        var urdaRunHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRunHook.cs");
        var urdaSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Urda");
        var urdaScene = ReadRepoText("EZMicroBalance", "scenes", "events", "background_scenes", "ezmb_urda.tscn");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var engCards = ReadRepoText("EZMicroBalance", "localization", "eng", "cards.json");
        var zhsCards = ReadRepoText("EZMicroBalance", "localization", "zhs", "cards.json");
        var engCardRewardUi = ReadRepoText("EZMicroBalance", "localization", "eng", "card_reward_ui.json");
        var zhsCardRewardUi = ReadRepoText("EZMicroBalance", "localization", "zhs", "card_reward_ui.json");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engCardRewardUiMap = JsonStringMap("EZMicroBalance", "localization", "eng", "card_reward_ui.json");
        var zhsCardRewardUiMap = JsonStringMap("EZMicroBalance", "localization", "zhs", "card_reward_ui.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        Assert.Contains("ForceAncientEnvironmentVariable", urdaGate, StringComparison.Ordinal);
        Assert.Contains("DisableAncientEnvironmentVariable", urdaGate, StringComparison.Ordinal);
        Assert.Contains("EZMB_DISABLE_URDA", urdaGate, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_DISABLE_URDA", urdaGate, StringComparison.Ordinal);
        Assert.Contains("ForcedAncient", urdaGate, StringComparison.Ordinal);
        Assert.Contains("ShouldForceUrda", urdaGate, StringComparison.Ordinal);
        Assert.Contains("EZMB_FORCE_ANCIENT", urdaGate, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_FORCE_ANCIENT", urdaGate, StringComparison.Ordinal);
        Assert.Contains("ForceBlessingEnvironmentVariable", urdaGate, StringComparison.Ordinal);
        Assert.Contains("EZMB_FORCE_URDA_BLESSING", urdaGate, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_FORCE_URDA_BLESSING", urdaGate, StringComparison.Ordinal);
        Assert.Contains("FirstEnvironmentValue", urdaGate, StringComparison.Ordinal);
        Assert.Contains("OrdinalIgnoreCase", urdaGate, StringComparison.Ordinal);
        Assert.Contains("string.Equals(", urdaGate, StringComparison.Ordinal);
        Assert.Contains("!IsTruthy", urdaGate, StringComparison.Ordinal);

        Assert.Contains("IsUrdaEnabled", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaFeatureGate.ShouldForceUrda", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("CustomAncientModel", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("CustomScenePath", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("CustomMapIconPath", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("CustomMapIconOutlinePath", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("CustomRunHistoryIconPath", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("CustomRunHistoryIconOutlinePath", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaAssetPaths.BackgroundScene", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaAssetPaths.MapIcon", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaAssetPaths.MapIconOutline", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaAssetPaths.RunHistoryIcon", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaAssetPaths.RunHistoryIconOutline", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaSeedbedOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaHumusPactOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaMoltingOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaMossMapOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaTrialBranchOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaShallowRootRelicOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaRootedRouteOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaAfterRainOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaRootSightOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("OptionWithRelic<UrdaSeedBankOptionRelic>", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("ExpectedInitialOptionCount = 4", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("options.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList()", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("base(autoAdd: false)", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("AllPossibleOptions", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaBlessingIds.Seedbed", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaBlessingService.SetSelectedBlessing", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaBlessingService.ApplyMolting", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaBlessingService.ApplyTrialBranch", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaBlessingService.ApplyShallowRootRelic", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaBlessingService.ApplyRootedRoute", urdaAncient, StringComparison.Ordinal);
        Assert.Contains("UrdaBlessingService.ApplyRootSight", urdaAncient, StringComparison.Ordinal);
        Assert.DoesNotContain("NeowEpoch", urdaAncient, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("urda_seedbed", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_humus_pact", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_molting", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_moss_map", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_trial_branch", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_shallow_root_relic", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_rooted_route", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_after_rain", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_root_sight", urdaBlessings, StringComparison.Ordinal);
        Assert.Contains("urda_seed_bank", urdaBlessings, StringComparison.Ordinal);
        Assert.Equal(10, Regex.Matches(urdaAncient, @"UrdaBlessingIds\.[A-Za-z]+")
            .Cast<Match>()
            .Select(match => match.Value)
            .Distinct(StringComparer.Ordinal)
            .Count());
        Assert.Contains("Done();", urdaAncient, StringComparison.Ordinal);
        Assert.DoesNotContain("CustomMapIconPath => UrdaAssetPaths.BackgroundScene", urdaAncient, StringComparison.Ordinal);
        Assert.DoesNotContain("CustomMapIconPath => UrdaAssetPaths.Icon", urdaAncient, StringComparison.Ordinal);
        Assert.DoesNotContain("CustomMapIconPath => $\"{MainFile.ResPath}/images/events/ezmb_urda.png\"", urdaAncient, StringComparison.Ordinal);
        Assert.DoesNotContain("urda_morvi", urdaBlessings, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("urda_lotha", urdaAncient, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("urda_vakuu", urdaAncient, StringComparison.OrdinalIgnoreCase);

        AssertSourceContains(
            urdaOptionRelics,
            "UrdaOptionRelic : CustomRelicModel",
            "Rarity => RelicRarity.Event",
            "IsAllowed(IRunState runState) => false",
            "IsAllowedAtNeow(Player player) => false",
            "IsAllowedInShops => false",
            "UrdaSeedbedOptionRelic",
            "UrdaHumusPactOptionRelic",
            "UrdaMoltingOptionRelic",
            "UrdaMossMapOptionRelic",
            "UrdaTrialBranchOptionRelic",
            "UrdaShallowRootRelicOptionRelic",
            "UrdaRootedRouteOptionRelic",
            "UrdaAfterRainOptionRelic",
            "UrdaRootSightOptionRelic",
            "UrdaSeedBankOptionRelic");
        Assert.Equal(10, Regex.Matches(urdaOptionRelics, @"\[Pool\(typeof\(SharedRelicPool\)\)\]").Count);

        AssertSourceContains(
            urdaScene,
            "[node name=\"EzmbUrdaBackground\" type=\"Control\"]",
            "[node name=\"Artwork\" type=\"TextureRect\" parent=\".\"]",
            "texture = ExtResource(\"1_urda\")");
        Assert.DoesNotContain("[node name=\"EzmbUrdaBackground\" type=\"Node2D\"]", urdaScene, StringComparison.Ordinal);
        Assert.DoesNotContain("type=\"Sprite2D\"", urdaScene, StringComparison.Ordinal);

        foreach (var relativePath in new[]
        {
            "EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon_outline.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon_outline.png",
            "EZMicroBalance/images/ancients/urda/options/urda_seedbed.png",
            "EZMicroBalance/images/ancients/urda/options/urda_humus_pact.png",
            "EZMicroBalance/images/ancients/urda/options/urda_molting.png",
            "EZMicroBalance/images/ancients/urda/options/urda_moss_map.png",
            "EZMicroBalance/images/ancients/urda/options/urda_trial_branch.png",
            "EZMicroBalance/images/ancients/urda/options/urda_shallow_root_relic.png",
            "EZMicroBalance/images/ancients/urda/options/urda_rooted_route.png",
            "EZMicroBalance/images/ancients/urda/options/urda_after_rain.png",
            "EZMicroBalance/images/ancients/urda/options/urda_root_sight.png",
            "EZMicroBalance/images/ancients/urda/options/urda_seed_bank.png"
        })
        {
            Assert.True(File.Exists(RepoPath(relativePath.Split('/'))), $"Missing Urda UI/art resource: {relativePath}");
            Assert.Contains($"res://{relativePath}", exportPreset, StringComparison.Ordinal);
        }

        AssertSourceContains(
            urdaInitializer,
            "ModHelper.SubscribeForRunStateHooks",
            "UrdaFeatureGate.IsUrdaEnabled(runState.UnlockState)",
            "ModelDb.GetById<UrdaRunHook>");
        AssertSourceContains(
            urdaRunHook,
            "TryModifyCardRewardAlternatives",
            "AfterRewardTaken",
            "BeforeRoomEntered",
            "AfterCardPlayed",
            "AfterCombatVictory",
            "ShouldDieLate",
            "AfterPreventingDeath",
            "PostAlternateCardRewardAction.EndSelectionAndCompleteReward",
            "AcceptSeedbed",
            "CanPaySeedbedCost",
            "CreatureCmd.LoseMaxHp",
            "CreatureCmd.SetMaxHp",
            "TryAddHumusPactAlternative",
            "ChooseHumusPact",
            "HumusCompletionPending",
            "PlayerCmd.GainGold",
            "ResolveHumusCompletion",
            "CardSelectCmd.FromDeckForRemoval",
            "WithSkippingDisallowed",
            "CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications | CardCreationFlags.NoUpgradeRoll",
            "ApplyMolting",
            "CreateCard<WitheredHusk>",
            "AfterRoomEntered",
            "player.IsActiveForHooks",
            "ApplyMossMapRoomReward",
            "PotionCmd.TryToProcure",
            "ApplyTrialBranch",
            "TrialBranchOfferCount = 4",
            "TrialBranchCombats = 3",
            "TrialBranchRequiredSuccesses = 2",
            "AncientSavedStateFields.UrdaTrialPlantCard",
            "ApplyShallowRootRelic",
            "ShallowRootRelicChoices = 2",
            "ShallowRootInitialGold = 75",
            "ShallowRootEliteGold = 35",
            "ShallowRootSettlementMaxHpLoss = 6",
            "RootedRouteMaxTargetFloor = 7",
            "RootedRouteCardRewards = 3",
            "RootedRouteWitherHpLoss = 8",
            "RootedRouteWitherGold = 25",
            "MapPointType.Monster",
            "EnsureQuestMarker<UrdaRootedRouteMapQuestMarker>",
            "AfterRainBlock = 15",
            "AfterRainDraw = 1",
            "AfterRainWounds = 2",
            "AfterRainMaxHpLoss = 3",
            "AfterRainCompensationHeal = 8",
            "AfterRainCompensationGold = 75",
            "AfterRainEliteGold = 20",
            "AfterRainEliteGoldLimit = 2",
            "RootSightStartingEyes = 5",
            "MapPointType.Boss",
            "SeedBankMaxSeeds = 3",
            "SeedBankMaxSettlementCards = 2",
            "TryAddSeedBankAlternative",
            "EZMB_URDA_SEED_BANK_STORE",
            "UrdaStateKey");
        Assert.DoesNotContain("[HarmonyPatch(typeof(CardReward), nameof(CardReward.OnSkipped))]", urdaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("OnSkipped", urdaRunHook, StringComparison.Ordinal);
        var seedbedAlternative = SliceBetween(urdaRunHook, "private static bool TryAddSeedbedAlternative", "private static bool TryAddHumusPactAlternative");
        Assert.DoesNotContain("SeedbedChecks = progress.SeedbedChecks + 1", seedbedAlternative, StringComparison.Ordinal);
        AssertSourceContains(
            seedbedAlternative,
            "progress.SeedbedAccepted >= MaxSeedbedChecks",
            "!CanPaySeedbedCost(player)",
            "PostAlternateCardRewardAction.EndSelectionAndCompleteReward");
        var seedbedAccept = SliceBetween(urdaRunHook, "private static async Task AcceptSeedbed", "private static async Task<bool> ResolveHumusCompletion");
        AssertSourceContains(
            seedbedAccept,
            "!CanPaySeedbedCost(player)",
            "CreatureCmd.LoseMaxHp",
            "SeedbedChecks = progress.SeedbedChecks + 1",
            "CreatureCmd.SetMaxHp");
        Assert.DoesNotContain("CreatureCmd.GainMaxHp", seedbedAccept, StringComparison.Ordinal);
        var chooseHumus = SliceBetween(urdaRunHook, "private static async Task ChooseHumusPact", "private static async Task ChooseSeedBankStore");
        AssertSourceContains(
            chooseHumus,
            "context.HumusPactHandled",
            "progress.HumusCompleted || progress.HumusCompletionPending",
            "progress = progress with { HumusSkips = progress.HumusSkips + 1 }",
            "HumusCompletionPending = true",
            "PlayerCmd.GainGold");
        Assert.DoesNotContain("RewardsSet", chooseHumus, StringComparison.Ordinal);
        Assert.DoesNotContain("CardSelectCmd.FromDeckForRemoval", chooseHumus, StringComparison.Ordinal);
        var humusAfterReward = SliceBetween(urdaRunHook, "public static async Task AfterRewardTaken", "private static bool TryAddSeedbedAlternative");
        AssertSourceContains(
            humusAfterReward,
            "var resolved = await ResolveHumusCompletion(player);",
            "if (!resolved)",
            "progress = GetProgress(player) with { HumusCompletionPending = false };");
        Assert.True(
            humusAfterReward.IndexOf("ResolveHumusCompletion(player)", StringComparison.Ordinal) <
            humusAfterReward.IndexOf("HumusCompletionPending = false", StringComparison.Ordinal),
            "Humus completion pending should clear only after the payoff resolver succeeds.");
        var humusCompletion = SliceBetween(urdaRunHook, "private static async Task<bool> ResolveHumusCompletion", "private static async Task ResolveTrialBranchCombat");
        AssertSourceContains(
            humusCompletion,
            "var rewardCard = CreateRandomRewardCard(player);",
            "return false;",
            "CardSelectCmd.FromDeckForRemoval",
            "WithSkippingDisallowed",
            "return true;");
        Assert.True(
            humusCompletion.IndexOf("CreateRandomRewardCard(player)", StringComparison.Ordinal) <
            humusCompletion.IndexOf("CardSelectCmd.FromDeckForRemoval", StringComparison.Ordinal),
            "Humus should generate the payoff card before optional removals so a no-card fallback cannot consume removals.");
        var trialBranch = SliceBetween(urdaRunHook, "public static async Task ApplyTrialBranch", "public static async Task ApplyShallowRootRelic");
        AssertSourceContains(
            trialBranch,
            "CreateTrialBranchOffers(player)",
            "CardSelectCmd.FromSimpleGrid",
            "CardCmd.Upgrade(selected, CardPreviewStyle.None)",
            "CardPileCmd.Add(selected, PileType.Deck)",
            "AncientSavedStateFields.UrdaTrialPlantCard[addResult.cardAdded] = true");
        var shallowRoot = SliceBetween(urdaRunHook, "public static async Task ApplyShallowRootRelic", "public static void ApplyRootedRoute");
        AssertSourceContains(
            shallowRoot,
            "RelicFactory.PullNextRelicFromFront",
            "RelicRarity.Common",
            "RelicSelectCmd.FromChooseARelicScreen",
            "RelicCmd.Obtain",
            "PlayerCmd.GainGold(ShallowRootInitialGold");
        var rootedRoute = SliceBetween(urdaRunHook, "public static void ApplyRootedRoute", "public static async Task ApplyRootSight");
        AssertSourceContains(
            rootedRoute,
            "FindRootedRouteTarget(player)",
            "EnsureQuestMarker<UrdaRootedRouteMapQuestMarker>",
            "RootedRouteCoord = FormatCoord(target.coord)");
        var rootSight = SliceBetween(urdaRunHook, "public static async Task ApplyRootSight", "private static async Task SettleSeedBankBeforeActOneBoss");
        AssertSourceContains(
            rootSight,
            "RootSightStartingEyes",
            "TryUseRootSightFallback(player, \"selection\")",
            ".Where(point => !marked.Contains(FormatCoord(point.coord)))",
            ".OrderBy(point => point.Quests.Count == 0 ? 0 : 1)",
            "EnsureQuestMarker<UrdaRootSightMapQuestMarker>");
        AssertSourceContains(
            urdaMapUiPatches,
            "UrdaRootSightMapHoverPatch",
            "HarmonyPatch(typeof(NNormalMapPoint), \"OnFocus\")",
            "UrdaRootSightMapQuestMarker",
            "NHoverTipSet.CreateAndShow",
            "EZMB_URDA.root_sight.map_hover.title",
            "EZMB_URDA.root_sight.map_hover.description");
        var afterRain = SliceBetween(urdaRunHook, "public static bool ShouldDieLate", "private static async Task AcceptSeedbed");
        AssertSourceContains(
            afterRain,
            "player.RunState.CurrentActIndex != 0",
            "return GetProgress(player).AfterRainSpent",
            "CreatureCmd.SetCurrentHp(creature, 1m)",
            "CreatureCmd.GainBlock(creature, AfterRainBlock",
            "CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), AfterRainDraw, player)",
            "CreateCard<Wound>",
            "CreatureCmd.LoseMaxHp");
        var seedBank = SliceBetween(urdaRunHook, "private static async Task ChooseSeedBankStore", "public static async Task ApplyTrialBranch");
        AssertSourceContains(
            seedBank,
            "GetSeedBankCardIds(progress)",
            "CardSelectCmd.FromSimpleGrid",
            "selected.Id.ToString()",
            "SeedBankCardIds");
        Assert.DoesNotContain("UrdaTrialPlantCard", seedBank, StringComparison.Ordinal);
        AssertSourceContains(
            urdaCards,
            "UrdaSeedling",
            "WitheredHusk",
            "CardKeyword.Ethereal",
            "CardKeyword.Unplayable",
            "AfterCardExhausted");
        AssertSourceContains(
            engCards,
            "EZMB_URDA_SEEDLING.title",
            "EZMB_WITHERED_HUSK.title");
        AssertSourceContains(
            zhsCards,
            "EZMB_URDA_SEEDLING.title",
            "EZMB_WITHERED_HUSK.title");
        Assert.Contains("OPTION_EZMB_URDA_SEEDBED.name", engCardRewardUi, StringComparison.Ordinal);
        Assert.Contains("OPTION_EZMB_URDA_SEEDBED.name", zhsCardRewardUi, StringComparison.Ordinal);
        Assert.Equal("Compost Reward", engCardRewardUiMap["OPTION_EZMB_URDA_HUMUS_PACT.name"]);
        AssertLocalizedKeys(
            [
                "OPTION_EZMB_URDA_SEEDBED.name",
                "OPTION_EZMB_URDA_HUMUS_PACT.name",
                "OPTION_EZMB_URDA_SEED_BANK_STORE.name"
            ],
            engCardRewardUiMap,
            zhsCardRewardUiMap,
            "Urda card-reward option localization");
        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.flavor",
                "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.title",
                "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.description",
                "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.flavor"
            ],
            engRelics,
            zhsRelics,
            "Urda option relic localization");
        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-EZMB_URDA.talk.firstVisitEver.0-0.ancient",
                "EZMICROBALANCE-EZMB_URDA.talk.ANY.0-0r.ancient",
                "EZMB_URDA.pages.INITIAL.options.urda_seedbed.title",
                "EZMB_URDA.pages.INITIAL.options.urda_seedbed.description",
                "EZMB_URDA.pages.INITIAL.options.urda_humus_pact.title",
                "EZMB_URDA.pages.INITIAL.options.urda_humus_pact.description",
                "EZMB_URDA.pages.INITIAL.options.urda_molting.title",
                "EZMB_URDA.pages.INITIAL.options.urda_molting.description",
                "EZMB_URDA.pages.INITIAL.options.urda_moss_map.title",
                "EZMB_URDA.pages.INITIAL.options.urda_moss_map.description",
                "EZMB_URDA.pages.INITIAL.options.urda_trial_branch.title",
                "EZMB_URDA.pages.INITIAL.options.urda_trial_branch.description",
                "EZMB_URDA.pages.INITIAL.options.urda_trial_branch.selectionScreenPrompt",
                "EZMB_URDA.pages.INITIAL.options.urda_shallow_root_relic.title",
                "EZMB_URDA.pages.INITIAL.options.urda_shallow_root_relic.description",
                "EZMB_URDA.pages.INITIAL.options.urda_rooted_route.title",
                "EZMB_URDA.pages.INITIAL.options.urda_rooted_route.description",
                "EZMB_URDA.pages.INITIAL.options.urda_after_rain.title",
                "EZMB_URDA.pages.INITIAL.options.urda_after_rain.description",
                "EZMB_URDA.pages.INITIAL.options.urda_root_sight.title",
                "EZMB_URDA.pages.INITIAL.options.urda_root_sight.description",
                "EZMB_URDA.root_sight.map_hover.title",
                "EZMB_URDA.root_sight.map_hover.description",
                "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.title",
                "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description",
                "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.storeSelectionPrompt",
                "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.settlementSelectionPrompt"
            ],
            engAncients,
            zhsAncients,
            "Urda ancient localization");
    }

    [Fact]
    public void UrdaDocsKeepLiveAndSaveLoadVerificationPending()
    {
        var issueIndex = ReadRepoText("docs", "issues.md");
        var urdaIssue = ReadRepoText("docs", "issues", "urda.md");
        var urdaReadme = ReadRepoText("docs", "features", "ancient-expansion-urda", "README.md");
        var urdaApi = ReadRepoText("docs", "features", "ancient-expansion-urda", "api-research.md");
        var urdaChecklist = ReadRepoText("docs", "features", "ancient-expansion-urda", "manual-test-checklist.md");
        var v22Readme = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "README.md");
        var v22Api = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "api-research.md");
        var v22Risk = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "risk-register.md");

        var currentUrdaDocs = string.Join(
            Environment.NewLine,
            issueIndex,
            urdaIssue,
            urdaReadme,
            urdaApi,
            urdaChecklist,
            v22Readme,
            v22Api,
            v22Risk);

        Assert.Contains("| URDA-PROTOTYPE | Ancient expansion | P0 | open |", issueIndex, StringComparison.Ordinal);
        Assert.Contains("live gameplay/save-load verification is still pending", currentUrdaDocs, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Live gameplay and save/load verification for current Urda remains pending", v22Readme, StringComparison.Ordinal);
        Assert.Contains("not source-proven as persisted", currentUrdaDocs, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("## 1A. Live evidence protocol", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/spire-plus-live-session.ps1 -Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/check-spire-window-preflight.ps1 -OutFile <evidence-dir>\\window-preflight.json -RequireSpireForeground", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/audit-godot-log.ps1 -Path <evidence-dir>\\godot.log -OutFile <evidence-dir>\\godot-log-audit.json -FailOnHit", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/spire-plus-live-session.ps1 -Mode Restore -EvidenceDir <evidence-dir> -StopGameOnRestore -PreserveNewCurrentRunsOnRestore", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("live-urda-postfix-20260513-131752", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("live-urda-continue-postfix-20260513-134337", urdaChecklist, StringComparison.Ordinal);
        Assert.Contains("do not satisfy any gameplay row", urdaChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Urda live gameplay verified", currentUrdaDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Urda save/load verified", currentUrdaDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("URDA-PROTOTYPE | Closed", currentUrdaDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("- [x]", urdaChecklist, StringComparison.Ordinal);
    }

    [Fact]
    public void ProjectStateAndHandoffClaimsTrackCurrentHeadAndNoStaleBaselineRefs()
    {
        var projectState = ReadRepoText("PROJECT_STATE.md");
        var handoff = ReadRepoText("docs", "private-beta-verification-handoff.md");
        var readme = ReadRepoText("docs", "README.md");

        Assert.Contains("Current reviewed state", projectState, StringComparison.Ordinal);
        Assert.Contains("a2183ee", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("f201508", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("b82023c", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("git log -1 --oneline --decorate", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("f201508", handoff, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("b82023c", handoff, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Current git status before", handoff, StringComparison.Ordinal);
        Assert.Contains("git diff --check", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("PROJECT_STATE.md", readme, StringComparison.Ordinal);
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
            "scripts/audit-godot-log.ps1 -Path <copied godot.log>",
            "host-godot-log-audit.json",
            "Date/time:",
            "Pass/fail/blocker:");

        Assert.Contains("Dual King Brands / second-boss Brand gameplay is currently disabled or downgraded in co-op pending live verification.", runbook, StringComparison.Ordinal);
    }

    [Fact]
    public void PublicAscensionSelectorExpandsStandardLobbiesAndAvoidsGlobalProgressPatches()
    {
        var allSource = ReadSourceTree("EZMicroBalanceCode");
        var selectorPatch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AscensionSelectionPatches.cs");

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

    private static void AssertSmallUiPngHasAlpha(string path, string message)
    {
        Assert.True(File.Exists(path), $"Missing PNG: {path}");
        var bytes = File.ReadAllBytes(path);
        Assert.True(bytes.Length >= 33, $"PNG too small to contain IHDR: {path}");
        Assert.True(bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47, $"Not a PNG file: {path}");

        var width = BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(16, 4));
        var height = BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(20, 4));
        var colorType = bytes[25];

        Assert.True(width >= 96 && height >= 96, message);
        Assert.Equal(6, colorType);
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
