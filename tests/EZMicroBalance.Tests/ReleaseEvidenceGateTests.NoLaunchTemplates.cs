using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseEvidenceGateTests
{
    [Fact]
    public void EvidenceCollectionScriptsCreatePendingNoLaunchTemplates()
    {
        foreach (var scriptName in new[]
        {
            "collect-release-evidence.ps1",
            "collect-mod-settings-evidence.ps1",
            "collect-preview-tools-evidence.ps1",
            "collect-vakuu-fight-evidence.ps1",
            "collect-coop-evidence.ps1"
        })
        {
            var script = AssertRepoFileExists("scripts", scriptName);
            var source = ReadRepoText("scripts", scriptName);

            Assert.Contains("[switch]$NoLaunch", source, StringComparison.Ordinal);
            Assert.Contains(".tools\\runtime-evidence", source, StringComparison.Ordinal);
            Assert.Contains("command.txt", source, StringComparison.Ordinal);
            Assert.Contains("environment.json", source, StringComparison.Ordinal);
            Assert.Contains("package-hashes.json", source, StringComparison.Ordinal);
            Assert.Contains("manual-rows-template.json", source, StringComparison.Ordinal);
            Assert.DoesNotContain("EZFuturePeekCode", source, StringComparison.Ordinal);
            Assert.DoesNotContain("EZFuturePeek.json", source, StringComparison.Ordinal);
            Assert.DoesNotContain("EZFuturePeek.sln", source, StringComparison.Ordinal);

            var evidenceDir = RepoPath(
                ".tools",
                "runtime-evidence",
                "test-release-evidence-gate",
                Path.GetFileNameWithoutExtension(scriptName),
                Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(evidenceDir);
            try
            {
                var result = RunPowerShell(script, "-NoLaunch", "-EvidenceDir", evidenceDir);
                Assert.True(result.ExitCode == 0, $"{scriptName} -NoLaunch failed:{Environment.NewLine}{result.Output}");

                Assert.True(File.Exists(Path.Combine(evidenceDir, "command.txt")), $"{scriptName} did not write command.txt.");
                var environmentPath = Path.Combine(evidenceDir, "environment.json");
                Assert.True(File.Exists(environmentPath), $"{scriptName} did not write environment.json.");
                AssertEnvironmentIncludesGitHandoffEvidence(environmentPath);
                var packageHashesPath = Path.Combine(evidenceDir, "package-hashes.json");
                Assert.True(File.Exists(packageHashesPath), $"{scriptName} did not write package-hashes.json.");
                AssertPackageHashesUseVersionedArtifacts(packageHashesPath);
                Assert.True(File.Exists(Path.Combine(evidenceDir, "manual-rows-template.json")), $"{scriptName} did not write manual-rows-template.json.");

                using var rowsDocument = JsonDocument.Parse(File.ReadAllText(Path.Combine(evidenceDir, "manual-rows-template.json")));
                var rows = rowsDocument.RootElement.GetProperty("Rows").EnumerateArray().ToArray();
                Assert.NotEmpty(rows);
                Assert.All(rows, row =>
                {
                    var status = row.GetProperty("Status").GetString();
                    Assert.Equal("pending", status);
                    Assert.NotEqual("passed", status, StringComparer.OrdinalIgnoreCase);
                    Assert.NotEqual("pass", status, StringComparer.OrdinalIgnoreCase);
                });

                if (scriptName == "collect-mod-settings-evidence.ps1")
                {
                    var row = Assert.Single(rows);
                    Assert.Equal("mod-settings-current-display", row.GetProperty("Id").GetString());
                    Assert.Equal("clicked-ui", row.GetProperty("Kind").GetString());
                    Assert.True(File.Exists(Path.Combine(evidenceDir, "README.md")), "Mod Settings helper did not write README.md.");
                    Assert.True(File.Exists(Path.Combine(evidenceDir, "window-preflight.json")), "Mod Settings helper did not write window-preflight.json.");
                    Assert.True(File.Exists(Path.Combine(evidenceDir, "window-preflight-output.txt")), "Mod Settings helper did not retain preflight output.");
                    Assert.True(File.Exists(Path.Combine(evidenceDir, "mod-settings-checklist-template.md")), "Mod Settings helper did not write checklist template.");
                    Assert.True(File.Exists(Path.Combine(evidenceDir, "mod-settings-checklist.md")), "Mod Settings helper did not write working checklist.");
                    Assert.True(File.Exists(Path.Combine(evidenceDir, "route-note.md")), "Mod Settings helper did not write route-note.md.");
                    Assert.True(File.Exists(Path.Combine(evidenceDir, "result-note.md")), "Mod Settings helper did not write result-note.md.");
                    Assert.True(Directory.Exists(Path.Combine(evidenceDir, "screenshots")), "Mod Settings helper did not create screenshots directory.");

                    var readme = File.ReadAllText(Path.Combine(evidenceDir, "README.md"));
                    var checklist = File.ReadAllText(Path.Combine(evidenceDir, "mod-settings-checklist.md"));
                    Assert.Contains("collect-mod-settings-evidence.ps1 -NoLaunch", readme, StringComparison.Ordinal);
                    Assert.Contains("Current package version:", readme, StringComparison.Ordinal);
                    Assert.Contains("current package pass", readme, StringComparison.Ordinal);
                    Assert.Contains("-Capture List -RequireSpireForeground", readme, StringComparison.Ordinal);
                    Assert.Contains("-Capture Page -RequireSpireForeground", readme, StringComparison.Ordinal);
                    Assert.Contains("Pending scaffold only", result.Output, StringComparison.Ordinal);
                    Assert.Contains("$captureResult = Invoke-HelperScript", source, StringComparison.Ordinal);
                    Assert.DoesNotContain("$capture = Invoke-HelperScript", source, StringComparison.Ordinal);
                    Assert.Contains("ritsulib-visible-enabled", checklist, StringComparison.Ordinal);
                    Assert.Contains("spire-plus-list-display-name", checklist, StringComparison.Ordinal);
                    Assert.Contains("spire-plus-config-page-current-name", checklist, StringComparison.Ordinal);
                    Assert.Contains("ritsulib-migration-status-section", checklist, StringComparison.Ordinal);
                    Assert.Contains("ritsulib-runtime-dependency-card", checklist, StringComparison.Ordinal);
                    Assert.Contains("ritsulib-proof-boundary-card", checklist, StringComparison.Ordinal);
                    Assert.Contains("preview-tools-controls-render", checklist, StringComparison.Ordinal);
                    Assert.Contains("legacy-mod-surfaces-absent", checklist, StringComparison.Ordinal);
                    Assert.Contains("clean-log-config-registration", checklist, StringComparison.Ordinal);
                    Assert.Contains("Fill this checklist with live results before marking this row pass.", checklist, StringComparison.Ordinal);
                }

                if (scriptName == "collect-release-evidence.ps1")
                {
                    var manifestPath = Path.Combine(evidenceDir, "release-evidence-manifest.json");
                    var readmePath = Path.Combine(evidenceDir, "README.md");
                    Assert.True(File.Exists(manifestPath), "collect-release-evidence.ps1 did not write release-evidence-manifest.json.");
                    Assert.True(File.Exists(readmePath), "collect-release-evidence.ps1 did not write README.md.");

                    var rowIds = rows
                        .Select(row => row.GetProperty("Id").GetString())
                        .ToHashSet(StringComparer.Ordinal);
                    var expectedRowIds = new[]
                    {
                        "fresh-current-package-loader-smoke",
                        "mod-settings-current-display",
                        "ancient-ui-urda",
                        "ancient-ui-morvi",
                        "ancient-ui-lotha",
                        "ancient-ui-vakuu-normal",
                        "ancient-ui-vakuu-fight",
                        "ancient-reward-visible-relics",
                        "player-text-tooltip-readability",
                        "art-resource-routing-live-preview",
                        "vakuu-victory-no-black-screen",
                        "vakuu-failure-death-path",
                        "vakuu-active-fight-save-load",
                        "ancient-state-save-load",
                        "rootblight-visual-behavior",
                        "a11-natural-route-traversal",
                        "ascension-selector-localization",
                        "a19-a20-dedicated-boss-abilities",
                        "disable-mod-gameplay",
                        "preview-tools-live-proof",
                        "coop-disposition"
                    };

                    foreach (var expectedId in expectedRowIds)
                    {
                        Assert.Contains(expectedId, rowIds);
                    }

                    Assert.DoesNotContain("clicked-ancient-ui-urda-morvi-lotha-vakuu", rowIds);
                    Assert.All(rows, row => Assert.True(row.TryGetProperty("Kind", out _), "Release evidence rows must mirror verifier row Kind values."));

                    foreach (var row in rows)
                    {
                        var rowId = row.GetProperty("Id").GetString()!;
                        var rowEvidenceDir = row.GetProperty("EvidenceDir").GetString()!;
                        Assert.Equal(Path.Combine(evidenceDir, rowId), rowEvidenceDir);
                        Assert.True(Directory.Exists(rowEvidenceDir), $"Missing per-row evidence directory for {rowId}.");
                        Assert.True(File.Exists(Path.Combine(rowEvidenceDir, "README.md")), $"Missing per-row README.md for {rowId}.");
                        Assert.True(File.Exists(Path.Combine(rowEvidenceDir, "command.txt")), $"Missing per-row command.txt for {rowId}.");

                        var rowReadme = File.ReadAllText(Path.Combine(rowEvidenceDir, "README.md"));
                        Assert.Contains($"# {rowId}", rowReadme, StringComparison.Ordinal);
                        Assert.Contains("Required files for pass status:", rowReadme, StringComparison.Ordinal);
                    }

                    var loaderDir = Path.Combine(evidenceDir, "fresh-current-package-loader-smoke");
                    Assert.True(File.Exists(Path.Combine(loaderDir, "environment.json")), "Loader row did not get environment.json.");
                    Assert.True(File.Exists(Path.Combine(loaderDir, "package-hashes.json")), "Loader row did not get package-hashes.json.");
                    Assert.True(File.Exists(Path.Combine(loaderDir, "enabled-mods-template.txt")), "Loader row did not get enabled-mods-template.txt.");

                    var previewToolsDir = Path.Combine(evidenceDir, "preview-tools-live-proof");
                    Assert.True(File.Exists(Path.Combine(previewToolsDir, "environment.json")), "Preview-tools row did not get environment.json.");
                    Assert.True(File.Exists(Path.Combine(previewToolsDir, "package-hashes.json")), "Preview-tools row did not get package-hashes.json.");

                    var ancientRewardRow = rows.Single(row => row.GetProperty("Id").GetString() == "ancient-reward-visible-relics");
                    var ancientRewardFiles = ancientRewardRow
                        .GetProperty("RequiredFiles")
                        .EnumerateArray()
                        .Select(file => file.GetString())
                        .ToArray();
                    Assert.Contains("ancient-reward-relics-checklist.md", ancientRewardFiles);

                    var ancientRewardDir = Path.Combine(evidenceDir, "ancient-reward-visible-relics");
                    Assert.True(
                        File.Exists(Path.Combine(ancientRewardDir, "ancient-reward-relics-checklist-template.md")),
                        "Ancient reward row did not get a visible relic checklist template.");
                    var ancientRewardReadme = File.ReadAllText(Path.Combine(ancientRewardDir, "README.md"));
                    var ancientRewardChecklist = File.ReadAllText(Path.Combine(ancientRewardDir, "ancient-reward-relics-checklist-template.md"));
                    AssertTemplateChecklistCreated(ancientRewardChecklist, "ancient-reward-relics-checklist.md");
                    var ancientRewardWorkingChecklist = AssertWorkingChecklistCreated(
                        ancientRewardDir,
                        "ancient-reward-relics-checklist.md",
                        ["UrdaSeedBankOptionRelic", "MorviBlueprintProofOptionRelic", "LothaDeathReprieveOptionRelic", "VakuuFightOptionRelic"]);
                    Assert.Contains("Manual checkpoints:", ancientRewardReadme, StringComparison.Ordinal);
                    Assert.Contains("Every Urda, Morvi, and Lotha initial reward option is visible as an option relic", ancientRewardReadme, StringComparison.Ordinal);
                    Assert.Contains("UrdaSeedBankOptionRelic", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("MorviBlueprintProofOptionRelic", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("LothaDeathReprieveOptionRelic", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("VakuuFightOptionRelic", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("sere_talon_pickup", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("claws_maul_transform", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("Vakuu's Sere Talon / \u74e6\u5e93\u539f\u521d\u4e4b\u722a", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("Tanx Claws / \u5766\u514b\u65af\u5229\u722a", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("Maul / \u6495\u54ac", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("verify event-option art, relic-bar art, inspect art, hover text, and surface-specific log routes", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("`Ancient event option button`", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("`RelicModel packed icon texture`", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("`RelicModel big icon texture`", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("upgraded Maul", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.DoesNotContain("\u95bb", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.DoesNotContain("\u95b8", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.DoesNotContain("\u95b9", ancientRewardChecklist, StringComparison.Ordinal);
                    Assert.Contains("Fill this checklist with live results before marking this row pass.", ancientRewardWorkingChecklist, StringComparison.Ordinal);

                    var playerTextRow = rows.Single(row => row.GetProperty("Id").GetString() == "player-text-tooltip-readability");
                    var playerTextFiles = playerTextRow
                        .GetProperty("RequiredFiles")
                        .EnumerateArray()
                        .Select(file => file.GetString())
                        .ToArray();
                    Assert.Contains("player-text-qa-checklist.md", playerTextFiles);

                    var playerTextDir = Path.Combine(evidenceDir, "player-text-tooltip-readability");
                    Assert.True(
                        File.Exists(Path.Combine(playerTextDir, "player-text-qa-checklist-template.md")),
                        "Player text row did not get a QA checklist template.");
                    var playerTextReadme = File.ReadAllText(Path.Combine(playerTextDir, "README.md"));
                    var playerTextChecklist = File.ReadAllText(Path.Combine(playerTextDir, "player-text-qa-checklist-template.md"));
                    AssertTemplateChecklistCreated(playerTextChecklist, "player-text-qa-checklist.md");
                    var playerTextWorkingChecklist = AssertWorkingChecklistCreated(
                        playerTextDir,
                        "player-text-qa-checklist.md",
                        ["ascension-a11-a20", "ancient-choice-text", "preview-tools-text", "en-zhs-key-parity"]);
                    Assert.Contains("Manual checkpoints:", playerTextReadme, StringComparison.Ordinal);
                    Assert.Contains("Check English and Simplified Chinese text separately", playerTextReadme, StringComparison.Ordinal);
                    Assert.Contains("ascension-a11-a20", playerTextChecklist, StringComparison.Ordinal);
                    Assert.Contains("ancient-choice-text", playerTextChecklist, StringComparison.Ordinal);
                    Assert.Contains("preview-tools-text", playerTextChecklist, StringComparison.Ordinal);
                    Assert.Contains("en-zhs-key-parity", playerTextChecklist, StringComparison.Ordinal);
                    Assert.Contains("Fill this checklist with live results before marking this row pass.", playerTextWorkingChecklist, StringComparison.Ordinal);

                    var artRoutingRow = rows.Single(row => row.GetProperty("Id").GetString() == "art-resource-routing-live-preview");
                    var artRoutingFiles = artRoutingRow
                        .GetProperty("RequiredFiles")
                        .EnumerateArray()
                        .Select(file => file.GetString())
                        .ToArray();
                    Assert.Contains("art-resource-routing-checklist.md", artRoutingFiles);

                    var artRoutingDir = Path.Combine(evidenceDir, "art-resource-routing-live-preview");
                    Assert.True(
                        File.Exists(Path.Combine(artRoutingDir, "art-resource-routing-checklist-template.md")),
                        "Art routing row did not get a routing checklist template.");
                    var artRoutingReadme = File.ReadAllText(Path.Combine(artRoutingDir, "README.md"));
                    var artRoutingChecklist = File.ReadAllText(Path.Combine(artRoutingDir, "art-resource-routing-checklist-template.md"));
                    AssertTemplateChecklistCreated(artRoutingChecklist, "art-resource-routing-checklist.md");
                    var artRoutingWorkingChecklist = AssertWorkingChecklistCreated(
                        artRoutingDir,
                        "art-resource-routing-checklist.md",
                        ["title-home-preview", "option-relic-icons", "power-icons", "no-placeholder-or-official-art"]);
                    Assert.Contains("Manual checkpoints:", artRoutingReadme, StringComparison.Ordinal);
                    Assert.Contains("Confirm large Ancient/event art is used only on clicked Ancient or event screens", artRoutingReadme, StringComparison.Ordinal);
                    Assert.Contains("title-home-preview", artRoutingChecklist, StringComparison.Ordinal);
                    Assert.Contains("option-relic-icons", artRoutingChecklist, StringComparison.Ordinal);
                    Assert.Contains("power-icons", artRoutingChecklist, StringComparison.Ordinal);
                    Assert.Contains("no-placeholder-or-official-art", artRoutingChecklist, StringComparison.Ordinal);
                    Assert.Contains("Fill this checklist with live results before marking this row pass.", artRoutingWorkingChecklist, StringComparison.Ordinal);

                    var rootblightRow = rows.Single(row => row.GetProperty("Id").GetString() == "rootblight-visual-behavior");
                    var rootblightFiles = rootblightRow
                        .GetProperty("RequiredFiles")
                        .EnumerateArray()
                        .Select(file => file.GetString())
                        .ToArray();
                    Assert.Contains("rootblight-behavior-checklist.md", rootblightFiles);

                    var rootblightDir = Path.Combine(evidenceDir, "rootblight-visual-behavior");
                    Assert.True(
                        File.Exists(Path.Combine(rootblightDir, "rootblight-behavior-checklist-template.md")),
                        "Rootblight row did not get a behavior checklist template.");
                    var rootblightReadme = File.ReadAllText(Path.Combine(rootblightDir, "README.md"));
                    var rootblightChecklist = File.ReadAllText(Path.Combine(rootblightDir, "rootblight-behavior-checklist-template.md"));
                    AssertTemplateChecklistCreated(rootblightChecklist, "rootblight-behavior-checklist.md");
                    var rootblightWorkingChecklist = AssertWorkingChecklistCreated(
                        rootblightDir,
                        "rootblight-behavior-checklist.md",
                        ["rootblight-start-eligibility", "normal-rootblight-continuity", "boss-two-sprouts-staggered", "rootblight-save-load"]);
                    Assert.Contains("Manual checkpoints:", rootblightReadme, StringComparison.Ordinal);
                    Assert.Contains("normal fights advance existing Rootblight without expecting Blight Sprout cards", rootblightReadme, StringComparison.Ordinal);
                    Assert.Contains("Blight Sprout appears only in the current A15 Boss and A18 eligible Elite contexts", rootblightReadme, StringComparison.Ordinal);
                    Assert.Contains("rootblight-start-eligibility", rootblightChecklist, StringComparison.Ordinal);
                    Assert.Contains("normal-rootblight-continuity", rootblightChecklist, StringComparison.Ordinal);
                    Assert.DoesNotContain("normal-sprout-appearance", rootblightChecklist, StringComparison.Ordinal);
                    Assert.Contains("boss-two-sprouts-staggered", rootblightChecklist, StringComparison.Ordinal);
                    Assert.Contains("husk-exhaust-block-timing", rootblightChecklist, StringComparison.Ordinal);
                    Assert.Contains("rootblight-save-load", rootblightChecklist, StringComparison.Ordinal);
                    Assert.Contains("Fill this checklist with live results before marking this row pass.", rootblightWorkingChecklist, StringComparison.Ordinal);

                    var bossAbilityRow = rows.Single(row => row.GetProperty("Id").GetString() == "a19-a20-dedicated-boss-abilities");
                    var bossAbilityFiles = bossAbilityRow
                        .GetProperty("RequiredFiles")
                        .EnumerateArray()
                        .Select(file => file.GetString())
                        .ToArray();
                    Assert.Contains("boss-ability-checklist.md", bossAbilityFiles);

                    var bossAbilityDir = Path.Combine(evidenceDir, "a19-a20-dedicated-boss-abilities");
                    Assert.True(
                        File.Exists(Path.Combine(bossAbilityDir, "boss-ability-checklist-template.md")),
                        "A19/A20 row did not get a boss ability checklist template.");
                    var bossAbilityReadme = File.ReadAllText(Path.Combine(bossAbilityDir, "README.md"));
                    var bossAbilityChecklist = File.ReadAllText(Path.Combine(bossAbilityDir, "boss-ability-checklist-template.md"));
                    AssertTemplateChecklistCreated(bossAbilityChecklist, "boss-ability-checklist.md");
                    var bossAbilityWorkingChecklist = AssertWorkingChecklistCreated(
                        bossAbilityDir,
                        "boss-ability-checklist.md",
                        ["Martyr Oath", "Ink Return", "Time Sand Reflow", "Experimental Record"]);
                    Assert.Contains("Manual checkpoints:", bossAbilityReadme, StringComparison.Ordinal);
                    Assert.Contains("A20 Branded Form applies only to the second Act 3 Boss.", bossAbilityReadme, StringComparison.Ordinal);
                    Assert.Contains("Martyr Oath", bossAbilityChecklist, StringComparison.Ordinal);
                    Assert.Contains("Ink Return", bossAbilityChecklist, StringComparison.Ordinal);
                    Assert.Contains("Time Sand Reflow", bossAbilityChecklist, StringComparison.Ordinal);
                    Assert.Contains("Experimental Record", bossAbilityChecklist, StringComparison.Ordinal);
                    Assert.Contains("Fill this checklist with live results before marking this row pass.", bossAbilityWorkingChecklist, StringComparison.Ordinal);

                    AssertChecklistTemplate(
                        rows,
                        evidenceDir,
                        "mod-settings-current-display",
                        "mod-settings-checklist.md",
                        "mod-settings-checklist-template.md",
                        ["ritsulib-visible-enabled", "spire-plus-list-display-name", "spire-plus-config-page-current-name", "ritsulib-migration-status-section", "ritsulib-runtime-dependency-card", "ritsulib-proof-boundary-card", "preview-tools-controls-render", "technical-id-compatibility", "legacy-mod-surfaces-absent", "clean-log-config-registration"]);
                    AssertChecklistTemplate(
                        rows,
                        evidenceDir,
                        "vakuu-victory-no-black-screen",
                        "vakuu-victory-checklist.md",
                        "vakuu-victory-checklist-template.md",
                        ["fight-start-scene", "contract-turns", "locks-blood-debt", "victory-return", "non-vakuu-rewards", "no-black-screen", "log-clean"]);
                    AssertChecklistTemplate(
                        rows,
                        evidenceDir,
                        "vakuu-failure-death-path",
                        "vakuu-failure-death-checklist.md",
                        "vakuu-failure-death-checklist-template.md",
                        ["failure-path", "death-path", "room-state-after-exit", "no-softlock", "log-clean"]);
                    AssertChecklistTemplate(
                        rows,
                        evidenceDir,
                        "vakuu-active-fight-save-load",
                        "vakuu-save-load-checklist.md",
                        "vakuu-save-load-checklist-template.md",
                        ["active-combat-save", "active-combat-load", "parent-event-state", "prefinished-load", "no-duplicate-heal-or-reward"]);
                    AssertChecklistTemplate(
                        rows,
                        evidenceDir,
                        "preview-tools-live-proof",
                        "preview-tools-checklist.md",
                        "preview-tools-checklist-template.md",
                        ["crystal-sphere-button", "crystal-sphere-mask-only", "transform-preview-matches-result", "prismatic-gem-reward-hooks", "coop-gate-or-two-client-proof"]);
                    AssertChecklistTemplate(
                        rows,
                        evidenceDir,
                        "coop-disposition",
                        "coop-disposition-checklist.md",
                        "coop-disposition-checklist-template.md",
                        ["coop-host-join-clean-logs", "coop-a11-a20-selection", "coop-ancients", "coop-root-eyes", "coop-rootblight", "coop-save-load-or-reconnect", "coop-preview-tools-disposition", "coop-release-note-disposition"]);

                    var readme = File.ReadAllText(readmePath);
                    Assert.Contains("Required verifier row IDs:", readme, StringComparison.Ordinal);
                    Assert.Contains("Each verifier row has its own subfolder.", readme, StringComparison.Ordinal);
                    foreach (var expectedId in expectedRowIds)
                    {
                        Assert.Contains($"- {expectedId} ", readme, StringComparison.Ordinal);
                    }

                    Assert.DoesNotContain("Required high-level evidence:", readme, StringComparison.Ordinal);
                    Assert.DoesNotContain("- Clicked Ancient UI", readme, StringComparison.Ordinal);
                    Assert.Contains("release-evidence-verifier-pass.json", readme, StringComparison.Ordinal);

                    using var manifestDocument = JsonDocument.Parse(File.ReadAllText(manifestPath));
                    var manifest = manifestDocument.RootElement;
                    Assert.Equal(CurrentPackageZipSha256(), manifest.GetProperty("PackageSha256").GetString());
                    Assert.Equal(CurrentPackageZipRelativePath(), manifest.GetProperty("PackagePath").GetString());
                    Assert.Equal(rows.Length, manifest.GetProperty("Rows").GetArrayLength());

                    var verifier = AssertRepoFileExists("scripts", "verify-spire-plus-release-evidence.ps1");
                    var verifierResult = RunPowerShell(
                        verifier,
                        "-EvidenceRoot",
                        evidenceDir,
                        "-ManifestPath",
                        manifestPath);
                    Assert.NotEqual(0, verifierResult.ExitCode);
                    Assert.Contains("is not pass or accepted deferred", verifierResult.Output, StringComparison.Ordinal);
                    Assert.DoesNotContain("Missing release evidence manifest", verifierResult.Output, StringComparison.OrdinalIgnoreCase);

                    AssertReleaseEvidenceVerifierDeferredContract(evidenceDir, manifestPath, verifier, rows);
                }
            }
            finally
            {
                if (Directory.Exists(evidenceDir))
                {
                    Directory.Delete(evidenceDir, recursive: true);
                }
            }
        }

        var compatibilityWrapper = ReadRepoText("scripts", "collect-future-peek-evidence.ps1");
        Assert.Contains("collect-preview-tools-evidence.ps1", compatibilityWrapper, StringComparison.Ordinal);
        Assert.Contains("compatibility wrapper", compatibilityWrapper, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("EZFuturePeekCode", compatibilityWrapper, StringComparison.Ordinal);
        Assert.DoesNotContain("EZFuturePeek.json", compatibilityWrapper, StringComparison.Ordinal);
        Assert.DoesNotContain("EZFuturePeek.sln", compatibilityWrapper, StringComparison.Ordinal);
    }
}
