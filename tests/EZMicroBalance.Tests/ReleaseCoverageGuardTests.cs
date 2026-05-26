using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class ReleaseCoverageGuardTests
{
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
        new("Vakuu's Sere Talon", ["SERE_TALON.description"], [], [], ["option.Relic is PreservedFog or SereTalon"]),
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
        new("Meat Cleaver", ["MEAT_CLEAVER.description"], [], ["OPTION_COOK.name", "OPTION_COOK.ezDescription", "OPTION_COOK.ezDescriptionDisabled"], ["MeatCleaver", "CookRestSiteOption", "CardsToRemove = 2"]),
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
                "FiremarkMightOverflowPower",
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
                "One main enemy receives Might, Giant, Forge Armor, or Constant Heal",
                "Overflow affects at most one secondary enemy",
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
                "Fission reward mutation is source-patched; reward reroll, pickup, localization rendering, Exhaust payoff live behavior, and save/load are pending."
            ],
            [
                "Gated implementation present; live testing pending.",
                "Fission appears only on eligible reward cards.",
                "Tooltip/card text is correct in English and Simplified Chinese, uses energy-cost wording, does not show raw `{energyPrefix:energyIcons(...)}` templates, does not duplicate the added Exhaust line, and does not use the Chinese word \"\u8d39\u7528\" for Fission.",
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
                "BannerKind.Shieldwall",
                "BannerKind.BloodPrize",
                "BannerKind.PressingLine",
                "BannerKind.LastStand",
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
                "AeonglassHourglass",
                "AeonglassLaserEchoIntentLabelPatch",
                "IsBossBrand",
                "AscensionA20GenerateRoomsPatch",
                "AscensionA20CourtyardProceedPatch",
                "A20Courtyard",
                "AscensionA20RewardScreenReadyPatch",
                "A20_INTERMISSION_HEADER",
                "BossMapPointHoverPatch",
                "BOSS_BRANDED_FORM",
                "TryAddBossSealRewardOption",
                "BossRewardTargetOptionCount = 4",
                "TryAddA20BossOneCardReward"
            ],
            [
                "`BossSealDefinition` / `BossSealCatalog` now map active boss encounters to the v4.1 dedicated Boss ability set",
                "source-guarded through supported hooks",
                "Boss 1 post-combat recovery",
                "fixed courtyard event",
                "vanilla double-boss map path"
            ],
            [
                "Gated implementation present as BossSeal definitions plus source-guarded runtime hooks; live testing pending.",
                "A19 boss-specific dedicated ability metadata is assigned at map generation.",
                "Boss card rewards improve as documented.",
                "Boss 1 reward screen opens the A20 courtyard event before the second Boss."
            ])
    ];

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
            "DebugLevelEnvironmentVariable = \"SPIREPLUS_ASCENSION_DEBUG_LEVEL\"",
            "LegacyDebugLevelEnvironmentVariable = \"EZMB_ASCENSION_DEBUG_LEVEL\"",
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

        Assert.Contains("A11-A20 selection is default-on only for single-player standard lobbies", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("Set `SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("Set `SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection.", releaseChecklist, StringComparison.Ordinal);
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

        AssertRepoDirectoryExists("docs", "issues");
        AssertRepoDirectoryExists("docs", "archive", "prompts", "2026-05");
        AssertRepoFileExists("docs", "issues", "waiting-tests.md");
        AssertRepoFileExists("docs", "archive", "issues", "waiting-tests-pre-slim-20260518.md");
        AssertRepoFileExists("docs", "archive", "feature-audits", "review-pre-slim-20260518.md");
        AssertRepoFileExists("docs", "archive", "prompts", "2026-05", "codex-urda-overnight-prompt.md");
        AssertRepoFileExists("docs", "archive", "prompts", "2026-05", "issues-urda-overnight-addendum.md");

        var waitingTests = ReadRepoText("docs", "issues", "waiting-tests.md");
        var review = ReadRepoText("docs", "review.md");
        Assert.Contains("Compact manual evidence queue", docsReadme, StringComparison.Ordinal);
        Assert.Contains("Full historical issue text was archived", waitingTests, StringComparison.Ordinal);
        Assert.Contains("MP-MAC-MODELDB-HASH", waitingTests, StringComparison.Ordinal);
        Assert.True(waitingTests.Split('\n').Length <= 80, "Keep the active waiting-tests queue compact; archive detailed historical rows.");
        Assert.Contains("review-pre-slim-20260518.md", review, StringComparison.Ordinal);
        Assert.True(review.Split('\n').Length <= 140, "Keep docs/review.md compact; archive detailed historical review logs.");
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
            if (table == "intents")
            {
                continue;
            }

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
            "EZMicroBalanceCode/*",
            "art_pipeline/*",
            "asset/*",
            "source code/*",
            "docs/*",
            "legacy/*");
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
            "Spire Plus error/exception",
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
        Assert.Contains("Current package hashes, 2026-05-27:", issues, StringComparison.Ordinal);
        Assert.Contains("`URDA-PROTOTYPE` P0 open", issues, StringComparison.Ordinal);
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
        var implementationPlan = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "implementation-plan.md");
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
        Assert.Contains("Vakuu fight", issues, StringComparison.OrdinalIgnoreCase);

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
        var milestoneGateIssue = SliceBetween(
            v22Issues,
            "## ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-MILESTONE-GATES",
            "## ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-MORVI-LOTHA-ART-INTEGRATION");
        AssertSourceContains(
            milestoneGateIssue,
            "Status: source-governed / live-pending",
            "issue row first, source research, focused guard, implementation, manual row, review note, then validation",
            "Live-ready still requires current-package screenshots/logs/manual notes");
        Assert.DoesNotContain("Status: open", milestoneGateIssue, StringComparison.Ordinal);

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
            implementationPlan,
            "Open or update a compact issue row with acceptance criteria and the manual proof needed",
            "Record source evidence in `api-research.md`",
            "Add focused source guard tests before or with implementation",
            "`source-ready`: implementation, source evidence, focused guards, localization/text/art coverage, build, tests, format, and diff-check pass",
            "`live-ready`: current-package screenshots, `godot.log`, manual notes, save/load or two-client evidence exist",
            "Do not start a future milestone as a documentation-only audit");
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
    public void ActiveAncientExpansionEventArtIsExportedAndDocumented()
    {
        var exportPreset = ReadRepoText("export_presets.cfg");
        var artDirection = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "art-direction.md");
        var v22Issues = ReadRepoText("docs", "issues", "ancient-expansion-v2.2.md");
        var workLog = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "work-log.md");

        var morviPng = AssertRepoFileExists("EZMicroBalance", "images", "events", "ezmb_morvi.png");

        Assert.Contains("res://EZMicroBalance/images/events/ezmb_morvi.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/scenes/events/background_scenes/ezmb_morvi.tscn", exportPreset, StringComparison.Ordinal);
        Assert.True(
            new FileInfo(morviPng).Length > 1_000_000,
            "Morvi event art must not regress to a small geometric placeholder.");
        AssertSmallUiPngHasAlpha(
            RepoPath("EZMicroBalance", "images", "ancients", "morvi", "ezmb_morvi_map_icon.png"),
            "Morvi map icon must remain a readable transparent UI resource.");
        AssertRepoFileExists("EZMicroBalance", "images", "events", "ezmb_lotha.png");
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
            "Active Morvi event art uses the recovered user-uploaded blue-eye court source",
            "Active event art now uses the corrected user-uploaded horizontal mirror-ensemble source",
            "Active event art is the original user-accepted 16:9 Urda middle-draft",
            "Final browser GPTimage2 small art generated this pass",
            "Urda, Morvi, and Lotha option/icon art uses browser ChatGPT/GPTimage2 rebuilt transparent PNGs",
            "Custom card portraits now use browser GPTimage2 rebuilt files",
            "No `generic_temporary` or `final_required_before_release` art blockers remain",
            "Do not use placeholder art for Morvi or future active Ancients just to satisfy the export list.");
        AssertSourceContains(
            v22Issues,
            "Morvi is default-on",
            "Lotha is default-on");
        Assert.Contains("Recovered the user-uploaded Morvi blue-eye court background", workLog, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Recovered the correct user-uploaded horizontal mirror-ensemble image", workLog, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Restored the user-accepted 16:9 Urda root-mother background", workLog, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ProjectStateAndHandoffClaimsTrackCurrentHeadAndNoStaleBaselineRefs()
    {
        var projectState = ReadRepoText("PROJECT_STATE.md");
        var handoff = ReadRepoText("docs", "private-beta-verification-handoff.md");
        var readme = ReadRepoText("docs", "README.md");

        Assert.Contains("Current reviewed state", projectState, StringComparison.Ordinal);
        Assert.Contains("Latest pushed cleanup/package evidence baseline", projectState, StringComparison.Ordinal);
        Assert.Contains("current beta.80 AncientBehavior localization/docs guard split package sync", projectState, StringComparison.Ordinal);
        Assert.Contains("git log -1 --oneline --decorate", projectState, StringComparison.Ordinal);
        Assert.Contains("a2183ee", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("5be5c51", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Refresh beta35 package guards", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("f201508", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("b82023c", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("git log -1 --oneline --decorate", handoff, StringComparison.Ordinal);
        Assert.Contains("git status --short --branch", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("f201508", handoff, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("b82023c", handoff, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Current git status before", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("A1.05.01", handoff, StringComparison.Ordinal);
        Assert.Contains("git diff --check", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("PROJECT_STATE.md", readme, StringComparison.Ordinal);
    }

    [Fact]
    public void MultiplayerTestRunbookCoversDefaultOnGateControlsAndLiveMatrix()
    {
        var runbook = ReadRepoText("docs", "features", "ascension-11-20", "multiplayer-test-runbook.md");

        AssertSourceContains(
            runbook,
            "A11-A20 selection is default-on only for single-player standard lobbies.",
            "Two physical PCs.",
            "Same-PC multi-open is not reliable for real Steam multiplayer and should not be the primary release test.",
            "`--force-steam off` is valid for controlled loader smoke only.",
            "SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1",
            "SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1",
            "SPIREPLUS_ASCENSION_DIAGNOSTICS=1",
            "EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1",
            "[Environment]::SetEnvironmentVariable('SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION','1','User')",
            "[Environment]::SetEnvironmentVariable('SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION',$null,'User')",
            "fully restart Steam and the game",
            "Default Fail-Closed Checks",
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

        Assert.Contains("A20 Branded Form / second-boss enhanced dedicated ability gameplay is currently disabled or downgraded in co-op pending live verification.", runbook, StringComparison.Ordinal);
    }

    [Fact]
    public void PublicAscensionSelectorExpandsStandardLobbiesAndAvoidsGlobalProgressPatches()
    {
        var allSource = ReadSourceTree("EZMicroBalanceCode");
        var selectorPatch = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Patches");

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

        Assert.Contains("A11-A20 selection is default-on only for single-player standard lobbies", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("A11 widens maps by 1 column, inserts a reachable optional route node in the new column, and adds route rows by act: Act 1 +1, Act 2 +1, Act 3 +2 without A11-specific map markers or hover tips.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("A17 inserts one optional 3-4 node Deep Branch in Acts 2/3", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("A20 uses the vanilla double-boss map path to create/reveal the final-act second Boss", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("A20 inserts a fixed courtyard event between Boss 1 rewards and Boss 2", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("Ascension 21-30 and custom-character content are not included.", releaseChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Ascension 21-30 implementation complete", currentDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("custom character implementation complete", currentDocs, StringComparison.OrdinalIgnoreCase);
    }
}
