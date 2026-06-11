using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
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
        "Brilliant Flame / Brightest Flame",
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
            "- [x] Fresh loader smoke for the current beta.85 ZIP hash is clean",
            "- [x] Latest loader smoke for the current beta.85 package hash reached main menu",
            "- [x] `godot.log` reviewed after fresh beta.85 normal Steam-client isolated startup/log verification.",
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
            "Fresh loader smoke for the current beta.85 package hash is clean",
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

    private static void AssertNoRawEnglishInZhsFallback(string value)
    {
        var visibleValue = Regex.Replace(value, @"\{[^{}]*\}", string.Empty, RegexOptions.CultureInvariant);
        Assert.DoesNotMatch(@"[A-Za-z]{2,}", visibleValue);
    }
}
