using System.Text;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class AncientUiReadinessGuardTests
{
    private sealed record SceneExpectation(string Ancient, string ScenePath, string RootNode, string EventArtPath);

    private sealed record OptionMarker(
        string RelicClass,
        string AssetMember,
        string AssetPath,
        string RelicKey);

    private sealed record AncientArtRoleSet(
        string Ancient,
        string SourcePath,
        string AssetPrefix,
        string BackgroundScenePath,
        string EventArtPath,
        string MapIconPath,
        string MapIconOutlinePath,
        string RunHistoryIconPath,
        string RunHistoryIconOutlinePath);

    private static readonly SceneExpectation[] ActiveAncientScenes =
    [
        new("Urda", "EZMicroBalance/scenes/events/background_scenes/ezmb_urda.tscn", "EzmbUrdaBackground", "EZMicroBalance/images/events/ezmb_urda.png"),
        new("Morvi", "EZMicroBalance/scenes/events/background_scenes/ezmb_morvi.tscn", "EzmbMorviBackground", "EZMicroBalance/images/events/ezmb_morvi.png"),
        new("Lotha", "EZMicroBalance/scenes/events/background_scenes/ezmb_lotha.tscn", "EzmbLothaBackground", "EZMicroBalance/images/events/ezmb_lotha.png")
    ];

    private static readonly (string Name, string SourcePath, int ExpectedCount)[] SourceOptionCounts =
    [
        ("Urda", "EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaAncient.cs", 4),
        ("Morvi", "EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviAncient.cs", 3),
        ("Lotha", "EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaAncient.cs", 3)
    ];

    private static readonly AncientArtRoleSet[] ActiveAncientArtRoles =
    [
        new(
            "Urda",
            "EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaAncient.cs",
            "UrdaAssetPaths",
            "EZMicroBalance/scenes/events/background_scenes/ezmb_urda.tscn",
            "EZMicroBalance/images/events/ezmb_urda.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon_outline.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon.png",
            "EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon_outline.png"),
        new(
            "Morvi",
            "EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviAncient.cs",
            "MorviAssetPaths",
            "EZMicroBalance/scenes/events/background_scenes/ezmb_morvi.tscn",
            "EZMicroBalance/images/events/ezmb_morvi.png",
            "EZMicroBalance/images/ancients/morvi/ezmb_morvi_map_icon.png",
            "EZMicroBalance/images/ancients/morvi/ezmb_morvi_map_icon_outline.png",
            "EZMicroBalance/images/ancients/morvi/ezmb_morvi_run_history_icon.png",
            "EZMicroBalance/images/ancients/morvi/ezmb_morvi_run_history_icon_outline.png"),
        new(
            "Lotha",
            "EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaAncient.cs",
            "LothaAssetPaths",
            "EZMicroBalance/scenes/events/background_scenes/ezmb_lotha.tscn",
            "EZMicroBalance/images/events/ezmb_lotha.png",
            "EZMicroBalance/images/ancients/lotha/ezmb_lotha_map_icon.png",
            "EZMicroBalance/images/ancients/lotha/ezmb_lotha_map_icon_outline.png",
            "EZMicroBalance/images/ancients/lotha/ezmb_lotha_run_history_icon.png",
            "EZMicroBalance/images/ancients/lotha/ezmb_lotha_run_history_icon_outline.png")
    ];

    private static readonly OptionMarker[] OptionMarkers =
    [
        new("UrdaSeedbedOptionRelic", "UrdaAssetPaths.SeedbedOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_seedbed.png", "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC"),
        new("UrdaHumusPactOptionRelic", "UrdaAssetPaths.HumusPactOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_humus_pact.png", "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC"),
        new("UrdaMoltingOptionRelic", "UrdaAssetPaths.MoltingOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_molting.png", "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC"),
        new("UrdaMossMapOptionRelic", "UrdaAssetPaths.MossMapOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_moss_map.png", "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC"),
        new("UrdaTrialBranchOptionRelic", "UrdaAssetPaths.TrialBranchOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_trial_branch.png", "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC"),
        new("UrdaShallowRootRelicOptionRelic", "UrdaAssetPaths.ShallowRootRelicOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_shallow_root_relic.png", "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC"),
        new("UrdaRootedRouteOptionRelic", "UrdaAssetPaths.RootedRouteOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_rooted_route.png", "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC"),
        new("UrdaAfterRainOptionRelic", "UrdaAssetPaths.AfterRainOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_after_rain.png", "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC"),
        new("UrdaRootSightOptionRelic", "UrdaAssetPaths.RootSightOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_root_sight.png", "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC"),
        new("UrdaSeedBankOptionRelic", "UrdaAssetPaths.SeedBankOptionIcon", "EZMicroBalance/images/ancients/urda/options/urda_seed_bank.png", "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC"),
        new("MorviForbiddenLoanOptionRelic", "MorviAssetPaths.ForbiddenLoanOptionIcon", "EZMicroBalance/images/ancients/morvi/options/morvi_forbidden_loan.png", "EZMICROBALANCE-MORVI_FORBIDDEN_LOAN_OPTION_RELIC"),
        new("MorviMisprintPressOptionRelic", "MorviAssetPaths.MisprintPressOptionIcon", "EZMicroBalance/images/ancients/morvi/options/morvi_misprint_press.png", "EZMICROBALANCE-MORVI_MISPRINT_PRESS_OPTION_RELIC"),
        new("MorviRedInkOverdraftOptionRelic", "MorviAssetPaths.RedInkOverdraftOptionIcon", "EZMicroBalance/images/ancients/morvi/options/morvi_red_ink_overdraft.png", "EZMICROBALANCE-MORVI_RED_INK_OVERDRAFT_OPTION_RELIC"),
        new("MorviOverdueLibraryOptionRelic", "MorviAssetPaths.OverdueLibraryOptionIcon", "EZMicroBalance/images/ancients/morvi/options/morvi_overdue_library.png", "EZMICROBALANCE-MORVI_OVERDUE_LIBRARY_OPTION_RELIC"),
        new("MorviOpenBookExamOptionRelic", "MorviAssetPaths.OpenBookExamOptionIcon", "EZMicroBalance/images/ancients/morvi/options/morvi_open_book_exam.png", "EZMICROBALANCE-MORVI_OPEN_BOOK_EXAM_OPTION_RELIC"),
        new("MorviPaperstormOptionRelic", "MorviAssetPaths.PaperstormOptionIcon", "EZMicroBalance/images/ancients/morvi/options/morvi_paperstorm.png", "EZMICROBALANCE-MORVI_PAPERSTORM_OPTION_RELIC"),
        new("MorviBlueprintProofOptionRelic", "MorviAssetPaths.BlueprintProofOptionIcon", "EZMicroBalance/images/ancients/morvi/options/morvi_blueprint_proof.png", "EZMICROBALANCE-MORVI_BLUEPRINT_PROOF_OPTION_RELIC"),
        new("MorviDebtSettlementOptionRelic", "MorviAssetPaths.DebtSettlementOptionIcon", "EZMicroBalance/images/ancients/morvi/options/morvi_debt_settlement.png", "EZMICROBALANCE-MORVI_DEBT_SETTLEMENT_OPTION_RELIC"),
        new("LothaMirrorRebuttalOptionRelic", "LothaAssetPaths.MirrorRebuttalOptionIcon", "EZMicroBalance/images/ancients/lotha/options/lotha_mirror_rebuttal.png", "EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC"),
        new("LothaMirrorHallEchoOptionRelic", "LothaAssetPaths.MirrorHallEchoOptionIcon", "EZMicroBalance/images/ancients/lotha/options/lotha_mirror_hall_echo.png", "EZMICROBALANCE-LOTHA_MIRROR_HALL_ECHO_OPTION_RELIC"),
        new("LothaPresumptionOptionRelic", "LothaAssetPaths.PresumptionOptionIcon", "EZMicroBalance/images/ancients/lotha/options/lotha_presumption.png", "EZMICROBALANCE-LOTHA_PRESUMPTION_OPTION_RELIC"),
        new("LothaClosedCourtOptionRelic", "LothaAssetPaths.ClosedCourtOptionIcon", "EZMicroBalance/images/ancients/lotha/options/lotha_closed_court.png", "EZMICROBALANCE-LOTHA_CLOSED_COURT_OPTION_RELIC"),
        new("LothaDeferredVerdictOptionRelic", "LothaAssetPaths.DeferredVerdictOptionIcon", "EZMicroBalance/images/ancients/lotha/options/lotha_deferred_verdict.png", "EZMICROBALANCE-LOTHA_DEFERRED_VERDICT_OPTION_RELIC"),
        new("LothaDeathReprieveOptionRelic", "LothaAssetPaths.DeathReprieveOptionIcon", "EZMicroBalance/images/ancients/lotha/options/lotha_death_reprieve.png", "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_OPTION_RELIC"),
        new("LothaSingleSentenceOptionRelic", "LothaAssetPaths.SingleSentenceOptionIcon", "EZMicroBalance/images/ancients/lotha/options/lotha_single_sentence.png", "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_OPTION_RELIC"),
        new("LothaPublicEvidenceOptionRelic", "LothaAssetPaths.PublicEvidenceOptionIcon", "EZMicroBalance/images/ancients/lotha/options/lotha_public_evidence.png", "EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC"),
        new("VakuuFightOptionRelic", "VakuuFightAssetPaths.OptionIcon", "EZMicroBalance/images/ancients/vakuu/options/vakuu_fight.png", "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC")
    ];

    [Fact]
    public void ActiveAncientBackgroundScenesUseControlRootsAndEventArt()
    {
        var exportPreset = ReadRepoText("export_presets.cfg");

        foreach (var scene in ActiveAncientScenes)
        {
            Assert.True(File.Exists(RepoPath(scene.ScenePath.Split('/'))), $"Missing {scene.Ancient} background scene.");
            Assert.True(File.Exists(RepoPath(scene.EventArtPath.Split('/'))), $"Missing {scene.Ancient} event artwork.");

            var sceneSource = ReadRepoText(scene.ScenePath.Split('/'));
            Assert.Contains($"[node name=\"{scene.RootNode}\" type=\"Control\"]", sceneSource, StringComparison.Ordinal);
            Assert.Contains("type=\"TextureRect\"", sceneSource, StringComparison.Ordinal);
            Assert.Contains($"path=\"res://{scene.EventArtPath}\"", sceneSource, StringComparison.Ordinal);
            Assert.Contains("expand_mode = 1", sceneSource, StringComparison.Ordinal);
            Assert.Contains("stretch_mode = 6", sceneSource, StringComparison.Ordinal);
            Assert.DoesNotContain("stretch_mode = 5", sceneSource, StringComparison.Ordinal);
            Assert.DoesNotContain("images/ancients", sceneSource, StringComparison.Ordinal);
            Assert.DoesNotContain("map_icon", sceneSource, StringComparison.Ordinal);
            Assert.DoesNotContain("run_history", sceneSource, StringComparison.Ordinal);

            Assert.Contains($"res://{scene.ScenePath}", exportPreset, StringComparison.Ordinal);
            Assert.Contains($"res://{scene.EventArtPath}", exportPreset, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ActiveAncientArtRolesStaySeparated()
    {
        var exportPreset = ReadRepoText("export_presets.cfg");

        foreach (var roleSet in ActiveAncientArtRoles)
        {
            var source = ReadRepoText(roleSet.SourcePath.Split('/'));
            AssertSourceContains(
                source,
                $"CustomScenePath => {roleSet.AssetPrefix}.BackgroundScene",
                $"CustomMapIconPath => {roleSet.AssetPrefix}.MapIcon",
                $"CustomMapIconOutlinePath => {roleSet.AssetPrefix}.MapIconOutline",
                $"CustomRunHistoryIconPath => {roleSet.AssetPrefix}.RunHistoryIcon",
                $"CustomRunHistoryIconOutlinePath => {roleSet.AssetPrefix}.RunHistoryIconOutline");

            foreach (var (member, path) in new[]
            {
                ("MapIcon", roleSet.MapIconPath),
                ("MapIconOutline", roleSet.MapIconOutlinePath),
                ("RunHistoryIcon", roleSet.RunHistoryIconPath),
                ("RunHistoryIconOutline", roleSet.RunHistoryIconOutlinePath)
            })
            {
                Assert.StartsWith("EZMicroBalance/images/ancients/", path, StringComparison.Ordinal);
                Assert.DoesNotContain("/images/events/", path, StringComparison.Ordinal);
                Assert.NotEqual(roleSet.EventArtPath, path);
                Assert.NotEqual(roleSet.BackgroundScenePath, path);
                Assert.Contains($"{member} => $\"{{MainFile.ResPath}}/{path["EZMicroBalance/".Length..]}\"", source, StringComparison.Ordinal);
                Assert.True(File.Exists(RepoPath(path.Split('/'))), $"Missing {roleSet.Ancient} {member}: {path}");
                Assert.Contains($"res://{path}", exportPreset, StringComparison.Ordinal);
            }

            Assert.StartsWith("EZMicroBalance/scenes/events/background_scenes/", roleSet.BackgroundScenePath, StringComparison.Ordinal);
            Assert.EndsWith(".tscn", roleSet.BackgroundScenePath, StringComparison.Ordinal);
            Assert.StartsWith("EZMicroBalance/images/events/", roleSet.EventArtPath, StringComparison.Ordinal);
            Assert.True(File.Exists(RepoPath(roleSet.BackgroundScenePath.Split('/'))), $"Missing {roleSet.Ancient} background scene.");
            Assert.True(File.Exists(RepoPath(roleSet.EventArtPath.Split('/'))), $"Missing {roleSet.Ancient} event art.");
            Assert.Contains($"BackgroundScene => $\"{{MainFile.ResPath}}/{roleSet.BackgroundScenePath["EZMicroBalance/".Length..]}\"", source, StringComparison.Ordinal);
            Assert.Contains($"res://{roleSet.BackgroundScenePath}", exportPreset, StringComparison.Ordinal);
            Assert.Contains($"res://{roleSet.EventArtPath}", exportPreset, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ActiveAncientInitialOptionsHaveExpectedCountsAndLoggedFallbacks()
    {
        foreach (var (name, path, expectedCount) in SourceOptionCounts)
        {
            var source = ReadRepoText(path.Split('/'));
            AssertSourceContains(
                source,
                $"private const int ExpectedInitialOptionCount = {expectedCount};",
                "return TakeFallbackOptions(options);",
                "forced blessing",
                "did not match any option; showing fallback options.",
                "options.Count == 0",
                "event will finish instead of presenting a blank Ancient screen",
                "source-backed option(s), expected",
                "options.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList()");
            Assert.DoesNotContain("Take(3).ToList()", source, StringComparison.Ordinal);
            Assert.Contains(name, source, StringComparison.Ordinal);
        }

        var vakuuPatch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");
        var vakuuGate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs");
        AssertSourceContains(
            vakuuPatch,
            "VakuuFightFeatureGate.IsFightEnabledForRun(runState)",
            "if (VakuuFightFeatureGate.ShouldForceFight)",
            "__result = [fightOption]",
            "__result = __result.Concat([fightOption]).ToList()",
            "options.Count == 3 ? options : [CreateVictoryFallbackOption(vakuu)]",
            "CreateVictoryFallbackOption");
        AssertSourceContains(vakuuGate, "runState.Players.Count == 1", "EZMB_FORCE_VAKUU_FIGHT", "SPIREPLUS_FORCE_VAKUU_FIGHT");
    }

    [Fact]
    public void OptionMarkerRelicsHaveArtAndBilingualLocalizationCoverage()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        foreach (var marker in OptionMarkers)
        {
            AssertSourceContains(source, marker.RelicClass, marker.AssetMember);
            Assert.StartsWith("EZMicroBalance/images/ancients/", marker.AssetPath, StringComparison.Ordinal);
            Assert.DoesNotContain("/images/events/", marker.AssetPath, StringComparison.Ordinal);
            Assert.DoesNotContain("map_icon", marker.AssetPath, StringComparison.Ordinal);
            Assert.DoesNotContain("run_history", marker.AssetPath, StringComparison.Ordinal);
            Assert.NotEqual("EZMicroBalance/images/relics/relic.png", marker.AssetPath);
            Assert.DoesNotContain($"{marker.AssetMember} => $\"{{MainFile.ResPath}}/images/relics/relic.png\"", source, StringComparison.Ordinal);
            Assert.True(File.Exists(RepoPath(marker.AssetPath.Split('/'))), $"Missing option marker art: {marker.AssetPath}");
            Assert.Contains($"res://{marker.AssetPath}", exportPreset, StringComparison.Ordinal);

            foreach (var suffix in new[] { ".title", ".description", ".flavor" })
            {
                AssertLocalizedValue(engRelics, marker.RelicKey + suffix);
                AssertLocalizedValue(zhsRelics, marker.RelicKey + suffix);
            }
        }
    }

    [Fact]
    public void ForceAncientGatesAreSourceBackedAndDocumentedForManualClickedUiEvidence()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion");
        AssertSourceContains(
            source,
            "EZMB_FORCE_ANCIENT",
            "SPIREPLUS_FORCE_ANCIENT",
            "EZMB_FORCE_URDA_BLESSING",
            "SPIREPLUS_FORCE_URDA_BLESSING",
            "EZMB_FORCE_MORVI_BLESSING",
            "SPIREPLUS_FORCE_MORVI_BLESSING",
            "EZMB_FORCE_LOTHA_BLESSING",
            "SPIREPLUS_FORCE_LOTHA_BLESSING",
            "EZMB_DISABLE_URDA",
            "SPIREPLUS_DISABLE_URDA",
            "EZMB_DISABLE_MORVI",
            "SPIREPLUS_DISABLE_MORVI",
            "EZMB_DISABLE_LOTHA",
            "SPIREPLUS_DISABLE_LOTHA",
            "EZMB_DISABLE_VAKUU_FIGHT",
            "SPIREPLUS_DISABLE_VAKUU_FIGHT");

        var docs = string.Join(
            Environment.NewLine,
            ReadRepoText("docs", "private-beta-verification-handoff.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md"));
        AssertSourceContains(
            docs,
            "No safe automated clicked-Ancient UI path exists",
            "scripts/collect-ancient-ui-evidence.ps1",
            "ancient-ui-evidence-plan.json",
            "manual-instructions.md",
            "SPIREPLUS_FORCE_ANCIENT=<Ancient>",
            "EZMB_FORCE_ANCIENT=<Ancient>",
            "SPIREPLUS_FORCE_VAKUU_FIGHT=1",
            "EZMB_FORCE_VAKUU_FIGHT=1",
            "Expected visible option counts are Urda 4, Morvi 3, Lotha 3",
            "Current source changes the focused `-ForceVakuuFight` case to one fight option.",
            "ancient EZMB_URDA",
            "ancient EZMB_MORVI",
            "ancient EZMB_LOTHA",
            "ancient VAKUU");
    }

    [Fact]
    public void AncientClickedUiEvidenceHelperIsSourceGuardedAndDocumented()
    {
        var helper = ReadRepoText("scripts", "collect-ancient-ui-evidence.ps1");
        Assert.True(File.Exists(RepoPath("scripts", "collect-ancient-ui-evidence.ps1")), "Missing Ancient UI evidence helper.");
        AssertSourceContains(
            helper,
            "[ValidateSet('URDA', 'MORVI', 'LOTHA', 'VAKUU')]",
            "ancient-ui-evidence-plan.json",
            "manual-instructions.md",
            "SPIREPLUS_FORCE_ANCIENT",
            "EZMB_FORCE_ANCIENT",
            "SPIREPLUS_FORCE_VAKUU_FIGHT",
            "EZMB_FORCE_VAKUU_FIGHT",
            "URDA = 4",
            "MORVI = 3",
            "LOTHA = 3",
            "VakuuFightEnabledSinglePlayer = 4",
            "VakuuFightDisabledOrIneligible = 3",
            "VakuuForceFight = 1",
            "ExpectedOptionCountForThisRun",
            "check-spire-window-preflight.ps1",
            "spire-plus-live-session.ps1",
            "-NoPreflight",
            "-ForceVakuuFight is valid only when -Ancient VAKUU is used.",
            "-Mode', 'Restore'");

        var docs = string.Join(
            Environment.NewLine,
            ReadRepoText("scripts", "README.md"),
            ReadRepoText("PROJECT_STATE.md"),
            ReadRepoText("docs", "issues.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md"));
        AssertSourceContains(
            docs,
            "scripts/collect-ancient-ui-evidence.ps1",
            "ancient-ui-evidence-plan.json",
            "manual-instructions.md",
            "This helper prepares evidence; it does not prove clicked UI by itself.",
            "Keep this section pending until Urda, Morvi, Lotha, and Vakuu clicked-screen screenshots/logs are captured.");

        Assert.DoesNotContain("helper verifies clicked UI", docs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("helper proves clicked UI", docs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("clicked UI verified by the helper", docs, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ActiveDocsDoNotClaimClickedAncientUiVerifiedWithoutRuntimeEvidence()
    {
        var activeDocs = string.Join(
            Environment.NewLine,
            ReadRepoText("PROJECT_STATE.md"),
            ReadRepoText("docs", "issues.md"),
            ReadRepoText("docs", "test-ready-development-goal.md"),
            ReadRepoText("docs", "private-beta-verification-handoff.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "work-log.md"));

        Assert.Contains("clicked Ancient UI", activeDocs, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("pending", activeDocs, StringComparison.OrdinalIgnoreCase);
        foreach (var prohibited in new[]
        {
            "clicked Ancient UI verified",
            "clicked UI verified",
            "Ancient UI verified",
            "clicked Ancient UI passed",
            "clicked live Ancient UI passed",
            "clicked UI verification passed"
        })
        {
            Assert.DoesNotContain(prohibited, activeDocs, StringComparison.OrdinalIgnoreCase);
        }
    }

    private static void AssertLocalizedValue(IReadOnlyDictionary<string, string> values, string key)
    {
        Assert.True(values.TryGetValue(key, out var value), $"Missing localization key: {key}");
        Assert.False(string.IsNullOrWhiteSpace(value), $"Empty localization key: {key}");
        Assert.DoesNotContain("TODO", value, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\uFFFD", value, StringComparison.Ordinal);
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

    private static void AssertSourceContains(string source, params string[] snippets)
    {
        var missing = snippets
            .Where(snippet => !source.Contains(snippet, StringComparison.Ordinal))
            .ToArray();

        Assert.True(missing.Length == 0, "Missing source evidence:" + Environment.NewLine + string.Join(Environment.NewLine, missing));
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
