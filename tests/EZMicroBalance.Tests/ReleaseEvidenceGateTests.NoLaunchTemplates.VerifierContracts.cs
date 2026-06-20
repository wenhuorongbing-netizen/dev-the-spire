using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseEvidenceGateTests
{
    private static void AssertReleaseEvidenceVerifierDeferredContract(string evidenceDir, string manifestPath, string verifier, JsonElement[] rows)
    {
        var manifestNode = JsonNode.Parse(File.ReadAllText(manifestPath))!.AsObject();
        var ancientRewardNode = manifestNode["Rows"]!
            .AsArray()
            .Select(row => row!.AsObject())
            .Single(row => row["Id"]!.GetValue<string>() == "ancient-reward-visible-relics");
        var playerTextNode = manifestNode["Rows"]!
            .AsArray()
            .Select(row => row!.AsObject())
            .Single(row => row["Id"]!.GetValue<string>() == "player-text-tooltip-readability");
        var artRoutingNode = manifestNode["Rows"]!
            .AsArray()
            .Select(row => row!.AsObject())
            .Single(row => row["Id"]!.GetValue<string>() == "art-resource-routing-live-preview");
        var modSettingsNode = manifestNode["Rows"]!
            .AsArray()
            .Select(row => row!.AsObject())
            .Single(row => row["Id"]!.GetValue<string>() == "mod-settings-current-display");
        var rootblightNode = manifestNode["Rows"]!
            .AsArray()
            .Select(row => row!.AsObject())
            .Single(row => row["Id"]!.GetValue<string>() == "rootblight-visual-behavior");
        var bossAbilityNode = manifestNode["Rows"]!
            .AsArray()
            .Select(row => row!.AsObject())
            .Single(row => row["Id"]!.GetValue<string>() == "a19-a20-dedicated-boss-abilities");
        var vakuuVictoryNode = manifestNode["Rows"]!
            .AsArray()
            .Select(row => row!.AsObject())
            .Single(row => row["Id"]!.GetValue<string>() == "vakuu-victory-no-black-screen");
        var vakuuFailureDeathNode = manifestNode["Rows"]!
            .AsArray()
            .Select(row => row!.AsObject())
            .Single(row => row["Id"]!.GetValue<string>() == "vakuu-failure-death-path");
        var vakuuSaveLoadNode = manifestNode["Rows"]!
            .AsArray()
            .Select(row => row!.AsObject())
            .Single(row => row["Id"]!.GetValue<string>() == "vakuu-active-fight-save-load");
        var previewToolsNode = manifestNode["Rows"]!
            .AsArray()
            .Select(row => row!.AsObject())
            .Single(row => row["Id"]!.GetValue<string>() == "preview-tools-live-proof");
        var coopNode = manifestNode["Rows"]!
            .AsArray()
            .Select(row => row!.AsObject())
            .Single(row => row["Id"]!.GetValue<string>() == "coop-disposition");

        foreach (var deferredRow in manifestNode["Rows"]!.AsArray())
        {
            var rowObject = deferredRow!.AsObject();
            rowObject["Status"] = "deferred";
            rowObject["ExplicitOwnerDecision"] = true;
            rowObject["ReleaseNote"] = "Synthetic accepted deferral for verifier pass-marker contract test.";
        }

        var ancientRewardEvidenceDir = ancientRewardNode["EvidenceDir"]!.GetValue<string>();
        File.WriteAllText(
            Path.Combine(ancientRewardEvidenceDir, "godot.log"),
            "Synthetic live log for Ancient reward verifier contract.");
        File.WriteAllText(
            Path.Combine(ancientRewardEvidenceDir, "godot-log-audit.json"),
            """{ "Clean": true }""");
        File.WriteAllText(
            Path.Combine(ancientRewardEvidenceDir, "result-note.md"),
            "Synthetic Ancient reward row result note for verifier contract.");
        File.Copy(
            Path.Combine(ancientRewardEvidenceDir, "ancient-reward-relics-checklist-template.md"),
            Path.Combine(ancientRewardEvidenceDir, "ancient-reward-relics-checklist.md"),
            overwrite: true);

        ancientRewardNode["Status"] = "pass";
        ancientRewardNode["ResultNote"] = "Synthetic pass attempt with an unfilled Ancient reward checklist.";
        ancientRewardNode["ExplicitOwnerDecision"] = false;
        ancientRewardNode["ReleaseNote"] = "";

        var playerTextEvidenceDir = playerTextNode["EvidenceDir"]!.GetValue<string>();
        File.WriteAllText(
            Path.Combine(playerTextEvidenceDir, "godot.log"),
            "Synthetic live log for player text verifier contract.");
        File.WriteAllText(
            Path.Combine(playerTextEvidenceDir, "godot-log-audit.json"),
            """{ "Clean": true }""");
        File.WriteAllText(
            Path.Combine(playerTextEvidenceDir, "result-note.md"),
            "Synthetic player text row result note for verifier contract.");
        File.Copy(
            Path.Combine(playerTextEvidenceDir, "player-text-qa-checklist-template.md"),
            Path.Combine(playerTextEvidenceDir, "player-text-qa-checklist.md"),
            overwrite: true);

        playerTextNode["Status"] = "pass";
        playerTextNode["ResultNote"] = "Synthetic pass attempt with an unfilled player text QA checklist.";
        playerTextNode["ExplicitOwnerDecision"] = false;
        playerTextNode["ReleaseNote"] = "";

        var artRoutingEvidenceDir = artRoutingNode["EvidenceDir"]!.GetValue<string>();
        File.WriteAllText(
            Path.Combine(artRoutingEvidenceDir, "godot.log"),
            "Synthetic live log for art routing verifier contract.");
        File.WriteAllText(
            Path.Combine(artRoutingEvidenceDir, "godot-log-audit.json"),
            """{ "Clean": true }""");
        File.WriteAllText(
            Path.Combine(artRoutingEvidenceDir, "route-note.md"),
            "Synthetic art routing row route note for verifier contract.");
        File.WriteAllText(
            Path.Combine(artRoutingEvidenceDir, "window-preflight.json"),
            """{ "SpireForeground": true }""");
        File.Copy(
            Path.Combine(artRoutingEvidenceDir, "art-resource-routing-checklist-template.md"),
            Path.Combine(artRoutingEvidenceDir, "art-resource-routing-checklist.md"),
            overwrite: true);

        artRoutingNode["Status"] = "pass";
        artRoutingNode["ResultNote"] = "Synthetic pass attempt with an unfilled art routing checklist.";
        artRoutingNode["ScreenshotFile"] = "screenshot.png";
        artRoutingNode["ExplicitOwnerDecision"] = false;
        artRoutingNode["ReleaseNote"] = "";
        WriteTinyPng(Path.Combine(artRoutingEvidenceDir, "screenshot.png"), width: 800, height: 450);

        var rootblightEvidenceDir = rootblightNode["EvidenceDir"]!.GetValue<string>();
        File.WriteAllText(
            Path.Combine(rootblightEvidenceDir, "godot.log"),
            "Synthetic live log for Rootblight verifier contract.");
        File.WriteAllText(
            Path.Combine(rootblightEvidenceDir, "godot-log-audit.json"),
            """{ "Clean": true }""");
        File.WriteAllText(
            Path.Combine(rootblightEvidenceDir, "result-note.md"),
            "Synthetic Rootblight row result note for verifier contract.");
        File.Copy(
            Path.Combine(rootblightEvidenceDir, "rootblight-behavior-checklist-template.md"),
            Path.Combine(rootblightEvidenceDir, "rootblight-behavior-checklist.md"),
            overwrite: true);

        rootblightNode["Status"] = "pass";
        rootblightNode["ResultNote"] = "Synthetic pass attempt with an unfilled Rootblight behavior checklist.";
        rootblightNode["ExplicitOwnerDecision"] = false;
        rootblightNode["ReleaseNote"] = "";

        var bossAbilityEvidenceDir = bossAbilityNode["EvidenceDir"]!.GetValue<string>();
        File.WriteAllText(
            Path.Combine(bossAbilityEvidenceDir, "godot.log"),
            "Synthetic live log for A19/A20 verifier contract.");
        File.WriteAllText(
            Path.Combine(bossAbilityEvidenceDir, "godot-log-audit.json"),
            """{ "Clean": true }""");
        File.WriteAllText(
            Path.Combine(bossAbilityEvidenceDir, "result-note.md"),
            "Synthetic A19/A20 row result note for verifier contract.");
        File.Copy(
            Path.Combine(bossAbilityEvidenceDir, "boss-ability-checklist-template.md"),
            Path.Combine(bossAbilityEvidenceDir, "boss-ability-checklist.md"),
            overwrite: true);

        bossAbilityNode["Status"] = "pass";
        bossAbilityNode["ResultNote"] = "Synthetic pass attempt with an unfilled A19/A20 checklist.";
        bossAbilityNode["ExplicitOwnerDecision"] = false;
        bossAbilityNode["ReleaseNote"] = "";

        PrepareChecklistPassAttempt(
            modSettingsNode,
            "mod-settings-checklist-template.md",
            "mod-settings-checklist.md",
            requiredNoteFile: "route-note.md",
            noteText: "Synthetic Mod Settings route note for verifier contract.",
            resultNote: "Synthetic pass attempt with an unfilled Mod Settings checklist.");
        var modSettingsEvidenceDir = modSettingsNode["EvidenceDir"]!.GetValue<string>();
        File.WriteAllText(
            Path.Combine(modSettingsEvidenceDir, "window-preflight.json"),
            """{ "SpireForeground": true }""");
        modSettingsNode["ScreenshotFile"] = "mod-settings-list.png";
        WriteTinyPng(Path.Combine(modSettingsEvidenceDir, "mod-settings-list.png"), width: 800, height: 450);

        PrepareChecklistPassAttempt(
            vakuuVictoryNode,
            "vakuu-victory-checklist-template.md",
            "vakuu-victory-checklist.md",
            requiredNoteFile: "result-note.md",
            noteText: "Synthetic Vakuu victory row result note for verifier contract.",
            resultNote: "Synthetic pass attempt with an unfilled Vakuu victory checklist.");
        PrepareChecklistPassAttempt(
            vakuuFailureDeathNode,
            "vakuu-failure-death-checklist-template.md",
            "vakuu-failure-death-checklist.md",
            requiredNoteFile: "result-note.md",
            noteText: "Synthetic Vakuu failure/death row result note for verifier contract.",
            resultNote: "Synthetic pass attempt with an unfilled Vakuu failure/death checklist.");
        PrepareChecklistPassAttempt(
            vakuuSaveLoadNode,
            "vakuu-save-load-checklist-template.md",
            "vakuu-save-load-checklist.md",
            requiredNoteFile: "save-load-note.md",
            noteText: "Synthetic Vakuu save-load row note for verifier contract.",
            resultNote: "Synthetic pass attempt with an unfilled Vakuu save-load checklist.");
        PrepareChecklistPassAttempt(
            previewToolsNode,
            "preview-tools-checklist-template.md",
            "preview-tools-checklist.md",
            requiredNoteFile: "result-note.md",
            noteText: "Synthetic preview tools row result note for verifier contract.",
            resultNote: "Synthetic pass attempt with an unfilled preview tools checklist.");
        PrepareChecklistPassAttempt(
            coopNode,
            "coop-disposition-checklist-template.md",
            "coop-disposition-checklist.md",
            requiredNoteFile: "result-note.md",
            noteText: "Synthetic co-op row result note for verifier contract.",
            resultNote: "Synthetic pass attempt with an unfilled co-op checklist.");
        var coopEvidenceDir = coopNode["EvidenceDir"]!.GetValue<string>();
        File.WriteAllText(
            Path.Combine(coopEvidenceDir, "host-godot.log"),
            "Synthetic host live log for co-op verifier contract.");
        File.WriteAllText(
            Path.Combine(coopEvidenceDir, "host-godot-log-audit.json"),
            """{ "Clean": true }""");
        File.WriteAllText(
            Path.Combine(coopEvidenceDir, "client-godot.log"),
            "Synthetic client live log for co-op verifier contract.");
        File.WriteAllText(
            Path.Combine(coopEvidenceDir, "client-godot-log-audit.json"),
            """{ "Clean": true }""");

        File.WriteAllText(
            manifestPath,
            manifestNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var blankChecklistResult = RunPowerShell(
            verifier,
            "-EvidenceRoot",
            evidenceDir,
            "-ManifestPath",
            manifestPath,
            "-AllowDeferred");
        Assert.NotEqual(0, blankChecklistResult.ExitCode);
        Assert.Contains("boss-ability-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("row for Ceremonial Beast has no filled Live result cell", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("ancient-reward-relics-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("row for Urda / seedbed has no filled Screen option result cell", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("player-text-qa-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("row for ascension-a11-a20 has no filled EN result cell", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("art-resource-routing-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("row for title-home-preview has no filled Live result cell", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("rootblight-behavior-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("row for rootblight-start-eligibility has no filled Live result cell", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("mod-settings-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("row for ritsulib-visible-enabled has no filled Live result cell", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("vakuu-victory-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("row for fight-start-scene has no filled Live result cell", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("vakuu-failure-death-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("vakuu-save-load-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("preview-tools-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("row for crystal-sphere-button has no filled Live result cell", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("coop-disposition-checklist.md still contains the unfilled template instruction", blankChecklistResult.Output, StringComparison.Ordinal);
        Assert.Contains("row for coop-host-join-clean-logs has no filled Live result cell", blankChecklistResult.Output, StringComparison.Ordinal);

        var filledAncientRewardChecklist = CreateFilledAncientRewardRelicsChecklist();
        Assert.Contains("Vakuu's Sere Talon / \u74e6\u5e93\u539f\u521d\u4e4b\u722a", filledAncientRewardChecklist, StringComparison.Ordinal);
        Assert.Contains("Tanx Claws / \u5766\u514b\u65af\u5229\u722a", filledAncientRewardChecklist, StringComparison.Ordinal);
        Assert.Contains("Maul / \u6495\u54ac", filledAncientRewardChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("\u95bb", filledAncientRewardChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("\u95b8", filledAncientRewardChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("\u95b9", filledAncientRewardChecklist, StringComparison.Ordinal);
        File.WriteAllText(
            Path.Combine(ancientRewardEvidenceDir, "ancient-reward-relics-checklist.md"),
            filledAncientRewardChecklist);
        File.WriteAllText(
            Path.Combine(playerTextEvidenceDir, "player-text-qa-checklist.md"),
            CreateFilledPlayerTextQaChecklist());
        File.WriteAllText(
            Path.Combine(artRoutingEvidenceDir, "art-resource-routing-checklist.md"),
            CreateFilledArtResourceRoutingChecklist());
        File.WriteAllText(
            Path.Combine(rootblightEvidenceDir, "rootblight-behavior-checklist.md"),
            CreateFilledRootblightBehaviorChecklist());
        File.WriteAllText(
            Path.Combine(bossAbilityEvidenceDir, "boss-ability-checklist.md"),
            CreateFilledBossAbilityChecklist());
        File.WriteAllText(
            Path.Combine(modSettingsEvidenceDir, "mod-settings-checklist.md"),
            CreateFilledSimpleChecklist("Mod Settings Current Display Checklist", RequiredModSettingsRows()));
        File.WriteAllText(
            Path.Combine(vakuuVictoryNode["EvidenceDir"]!.GetValue<string>(), "vakuu-victory-checklist.md"),
            CreateFilledSimpleChecklist("Vakuu Victory / No Black Screen Checklist", RequiredVakuuVictoryRows()));
        File.WriteAllText(
            Path.Combine(vakuuFailureDeathNode["EvidenceDir"]!.GetValue<string>(), "vakuu-failure-death-checklist.md"),
            CreateFilledSimpleChecklist("Vakuu Failure / Death Checklist", RequiredVakuuFailureDeathRows()));
        File.WriteAllText(
            Path.Combine(vakuuSaveLoadNode["EvidenceDir"]!.GetValue<string>(), "vakuu-save-load-checklist.md"),
            CreateFilledSimpleChecklist("Vakuu Save / Load Checklist", RequiredVakuuSaveLoadRows()));
        File.WriteAllText(
            Path.Combine(previewToolsNode["EvidenceDir"]!.GetValue<string>(), "preview-tools-checklist.md"),
            CreateFilledSimpleChecklist("Preview Tools Checklist", RequiredPreviewToolsRows()));
        File.WriteAllText(
            Path.Combine(coopNode["EvidenceDir"]!.GetValue<string>(), "coop-disposition-checklist.md"),
            CreateFilledSimpleChecklist("Co-op Disposition Checklist", RequiredCoopRows()));
        ancientRewardNode["ResultNote"] = "Synthetic pass attempt with every Ancient reward relic row filled.";
        playerTextNode["ResultNote"] = "Synthetic pass attempt with every player text QA row filled.";
        artRoutingNode["ResultNote"] = "Synthetic pass attempt with every art routing surface row filled.";
        rootblightNode["ResultNote"] = "Synthetic pass attempt with every Rootblight behavior row filled.";
        bossAbilityNode["ResultNote"] = "Synthetic pass attempt with every A19/A20 Boss row filled.";
        modSettingsNode["ResultNote"] = "Synthetic pass attempt with every Mod Settings row filled.";
        vakuuVictoryNode["ResultNote"] = "Synthetic pass attempt with every Vakuu victory row filled.";
        vakuuFailureDeathNode["ResultNote"] = "Synthetic pass attempt with every Vakuu failure/death row filled.";
        vakuuSaveLoadNode["ResultNote"] = "Synthetic pass attempt with every Vakuu save-load row filled.";
        previewToolsNode["ResultNote"] = "Synthetic pass attempt with every preview tools row filled.";
        coopNode["ResultNote"] = "Synthetic pass attempt with every co-op disposition row filled.";
        File.WriteAllText(
            manifestPath,
            manifestNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var passMarkerPath = Path.Combine(evidenceDir, "release-evidence-verifier-pass.json");
        var passResult = RunPowerShell(
            verifier,
            "-EvidenceRoot",
            evidenceDir,
            "-ManifestPath",
            manifestPath,
            "-AllowDeferred",
            "-WritePassMarker");
        Assert.True(passResult.ExitCode == 0, $"Verifier did not accept explicit deferred manifest:{Environment.NewLine}{passResult.Output}");
        Assert.True(File.Exists(passMarkerPath), "Verifier did not write release-evidence-verifier-pass.json.");

        using var markerDocument = JsonDocument.Parse(File.ReadAllText(passMarkerPath));
        var marker = markerDocument.RootElement;
        Assert.Equal("pass", marker.GetProperty("Status").GetString());
        Assert.Contains("verify-spire-plus-release-evidence.ps1", marker.GetProperty("Verifier").GetString(), StringComparison.Ordinal);
        Assert.True(marker.GetProperty("AllowDeferred").GetBoolean());
        Assert.Equal(rows.Length, marker.GetProperty("RequiredRowCount").GetInt32());
    }
}
