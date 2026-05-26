using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseEvidenceGateTests
{
    private static RequiredEvidence[] RequiredReleaseEvidence()
    {
        return
        [
            new(
                "fresh-loader-smoke",
                "Fresh current-package loader smoke with current package hashes and clean log audit",
                () => HasEvidenceDirectory(
                    "release-evidence-*",
                    "command.txt",
                    "environment.json",
                    "package-hashes.json",
                    "godot.log",
                    "godot-log-audit.json")),
            new(
                "clicked-ancient-ui",
                "Clicked Urda/Morvi/Lotha/Vakuu Ancient UI screenshots plus foreground/log evidence",
                () => HasEvidenceDirectory(
                    "ancient-ui-click-*",
                    "command.txt",
                    "window-preflight.json",
                    "godot.log",
                    "godot-log-audit.json",
                    "route-note.md") &&
                    Directory.GetFiles(RuntimeEvidenceRoot(), "ancient-ui-click-*", SearchOption.AllDirectories)
                        .Any(path => path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))),
            new(
                "save-load",
                "Ancient and gameplay save/load evidence",
                () => HasEvidenceDirectoryContaining("save-load", "godot.log", "godot-log-audit.json")),
            new(
                "vakuu",
                "Vakuu victory/no-black-screen/failure/death evidence",
                () => HasEvidenceDirectoryContaining("vakuu", "godot.log", "godot-log-audit.json", "vakuu-release-evidence-pass.json")),
            new(
                "coop",
                "Two-client host/client co-op evidence",
                () => HasEvidenceDirectory(
                    "coop-evidence-*",
                    "host/command.txt",
                    "host/godot.log",
                    "host/godot-log-audit.json",
                    "client/command.txt",
                    "client/godot.log",
                    "client/godot-log-audit.json")),
            new(
                "preview-tools",
                "Live Preview tools evidence for Crystal Sphere and transform preview",
                () => HasEvidenceDirectory(
                    "preview-tools-evidence-*",
                    "command.txt",
                    "environment.json",
                    "package-hashes.json",
                    "godot.log",
                    "godot-log-audit.json")),
            new(
                "release-evidence-verifier",
                "verify-spire-plus-release-evidence.ps1 pass marker",
                HasVerifierPassMarker)
        ];
    }

    private static bool HasEvidenceDirectory(string searchPattern, params string[] requiredRelativeFiles)
    {
        var root = RuntimeEvidenceRoot();
        if (!Directory.Exists(root))
        {
            return false;
        }

        return Directory.GetDirectories(root, searchPattern, SearchOption.AllDirectories)
            .Any(directory => requiredRelativeFiles.All(relativeFile => File.Exists(Path.Combine(directory, relativeFile))));
    }

    private static bool HasEvidenceDirectoryContaining(string directoryNameFragment, params string[] requiredRelativeFiles)
    {
        var root = RuntimeEvidenceRoot();
        if (!Directory.Exists(root))
        {
            return false;
        }

        return Directory.GetDirectories(root, "*", SearchOption.AllDirectories)
            .Where(directory => directory.Contains(directoryNameFragment, StringComparison.OrdinalIgnoreCase))
            .Any(directory => requiredRelativeFiles.All(relativeFile => File.Exists(Path.Combine(directory, relativeFile))));
    }

    private static bool HasVerifierPassMarker()
    {
        var root = RuntimeEvidenceRoot();
        if (!Directory.Exists(root))
        {
            return false;
        }

        foreach (var marker in Directory.GetFiles(root, "release-evidence-verifier-pass.json", SearchOption.AllDirectories))
        {
            try
            {
                using var document = JsonDocument.Parse(File.ReadAllText(marker));
                var rootElement = document.RootElement;
                if (rootElement.TryGetProperty("Status", out var status) &&
                    string.Equals(status.GetString(), "pass", StringComparison.OrdinalIgnoreCase) &&
                    rootElement.TryGetProperty("Verifier", out var verifier) &&
                    verifier.GetString()?.Contains("verify-spire-plus-release-evidence.ps1", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return true;
                }
            }
            catch (JsonException)
            {
            }
        }

        return false;
    }

    private static string RuntimeEvidenceRoot()
    {
        return RepoPath(".tools", "runtime-evidence");
    }

    private static (int ExitCode, string Output) RunPowerShell(string scriptPath, params string[] arguments)
    {
        var executable = OperatingSystem.IsWindows() ? "powershell.exe" : "pwsh";
        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            WorkingDirectory = Root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-ExecutionPolicy");
        startInfo.ArgumentList.Add("Bypass");
        startInfo.ArgumentList.Add("-File");
        startInfo.ArgumentList.Add(scriptPath);
        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = Process.Start(startInfo);
        Assert.NotNull(process);
        var output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
        Assert.True(process.WaitForExit(30_000), $"Timed out running {scriptPath}.");
        return (process.ExitCode, output);
    }

    private static void AssertChecklistTemplate(
        JsonElement[] rows,
        string evidenceDir,
        string rowId,
        string requiredChecklist,
        string templateFile,
        string[] expectedFragments)
    {
        var row = rows.Single(row => row.GetProperty("Id").GetString() == rowId);
        var requiredFiles = row
            .GetProperty("RequiredFiles")
            .EnumerateArray()
            .Select(file => file.GetString())
            .ToArray();
        Assert.Contains(requiredChecklist, requiredFiles);

        var rowDir = Path.Combine(evidenceDir, rowId);
        var templatePath = Path.Combine(rowDir, templateFile);
        Assert.True(File.Exists(templatePath), $"{rowId} did not get {templateFile}.");
        var rowReadme = File.ReadAllText(Path.Combine(rowDir, "README.md"));
        var checklist = File.ReadAllText(templatePath);
        Assert.Contains("Manual checkpoints:", rowReadme, StringComparison.Ordinal);
        AssertTemplateChecklistCreated(checklist, requiredChecklist);
        foreach (var expectedFragment in expectedFragments)
        {
            Assert.Contains(expectedFragment, checklist, StringComparison.Ordinal);
        }

        AssertWorkingChecklistCreated(rowDir, requiredChecklist, expectedFragments);
    }

    private static string AssertWorkingChecklistCreated(
        string rowDir,
        string checklistFile,
        string[] expectedFragments)
    {
        var workingPath = Path.Combine(rowDir, checklistFile);
        Assert.True(File.Exists(workingPath), $"Missing generated working checklist: {checklistFile}.");
        var workingChecklist = File.ReadAllText(workingPath);
        Assert.Contains("Fill this checklist with live results before marking this row pass.", workingChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Copy this file to", workingChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Template reference for", workingChecklist, StringComparison.Ordinal);
        foreach (var expectedFragment in expectedFragments)
        {
            Assert.Contains(expectedFragment, workingChecklist, StringComparison.Ordinal);
        }

        return workingChecklist;
    }

    private static void AssertTemplateChecklistCreated(string templateChecklist, string checklistFile)
    {
        Assert.Contains($"Template reference for `{checklistFile}`.", templateChecklist, StringComparison.Ordinal);
        Assert.Contains($"Fill the working `{checklistFile}` with live results before marking this row pass.", templateChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Copy this file to", templateChecklist, StringComparison.Ordinal);
    }

    private static void PrepareChecklistPassAttempt(
        JsonObject rowNode,
        string templateFile,
        string checklistFile,
        string requiredNoteFile,
        string noteText,
        string resultNote)
    {
        var evidenceDir = rowNode["EvidenceDir"]!.GetValue<string>();
        File.WriteAllText(
            Path.Combine(evidenceDir, "godot.log"),
            $"Synthetic live log for {checklistFile} verifier contract.");
        File.WriteAllText(
            Path.Combine(evidenceDir, "godot-log-audit.json"),
            """{ "Clean": true }""");
        File.WriteAllText(Path.Combine(evidenceDir, requiredNoteFile), noteText);
        File.Copy(
            Path.Combine(evidenceDir, templateFile),
            Path.Combine(evidenceDir, checklistFile),
            overwrite: true);

        rowNode["Status"] = "pass";
        rowNode["ResultNote"] = resultNote;
        rowNode["ExplicitOwnerDecision"] = false;
        rowNode["ReleaseNote"] = "";
    }

    private static string CreateFilledBossAbilityChecklist()
    {
        var bosses = new[]
        {
            ("Ceremonial Beast", "Holy Daze caps stun hits and grants Strength.", "Branded Form grants the higher Strength value."),
            ("The Kin", "Martyr Oath consumes up to 2 follower-death stacks and updates attack intent.", "Same-turn double follower death grants exactly 1 Artifact; attack bonus is higher."),
            ("Vantom", "Ink Return restores a percentage of cleared Slippery once.", "Higher restore percentage/caps apply."),
            ("Lagavulin Matriarch", "Plating Wake grants Multiplating based on wake source and Soul Siphon reduces it.", "Branded Form values and reduction differ as documented."),
            ("Soul Fysh", "Soul Tide counts unanswered Beckons at player turn end, applies capped Block after Soul Fysh's turn, and grants Artifact on Intangible.", "Higher per-Beckon Block and cap apply."),
            ("Waterfall Giant", "Unweakenable clears Weak/negative Strength for the explosion and applies Vulnerable to affected players.", "Vulnerable duration is higher."),
            ("Crab", "Claw Calibration reacts to claw HP-ratio gaps and updates attack intent.", "Lower threshold and higher attack bonus apply."),
            ("Knowledge Demon", "Marginal Note and Deep Thought add side costs without hard-locking Sloth/Waste Away.", "Deep Thought cap and side-cost rules match v4.1."),
            ("Insatiable Sandworm", "Escape Fatigue grants Vigor after generated Escape cards.", "Higher Vigor applies with team cap."),
            ("Aeonglass", "Time Sand Reflow adds Wither after Fade and clears by spent energy.", "Eye Lasers extra hit appears in intent only while Time Sand remains."),
            ("Queen", "Royal Decree marks one Bound card and team-caps Majesty/Torch Head Strength.", "Majesty cap and spend limit are higher."),
            ("Test Subject", "Experimental Record shows a phase-change notice and residual sample power.", "Two different samples appear on phase change.")
        };

        var lines = new List<string>
        {
            "# A19/A20 Dedicated Boss Ability Checklist",
            "",
            "| Boss | A19 ability | A20 Branded Form check | Live result | Evidence file(s) |",
            "| --- | --- | --- | --- | --- |"
        };

        lines.AddRange(bosses.Select(boss =>
            $"| {boss.Item1} | {boss.Item2} | {boss.Item3} | PASS: synthetic verifier contract row filled. | godot.log; result-note.md |"));

        return string.Join(Environment.NewLine, lines);
    }

    private static IReadOnlyList<(string Id, string Expected)> RequiredVakuuVictoryRows()
    {
        return
        [
            ("fight-start-scene", "Dedicated Vakuu monster and encounter scene appear, not a placeholder or normal Ancient screen."),
            ("contract-turns", "Contract choices appear on expected turns and do not softlock the hand."),
            ("locks-blood-debt", "Stolen Vault locks, broken-lock count, Blood Debt, Gold/HP settlement, and lethal-hit lock counting are visible and coherent."),
            ("victory-return", "Winning returns to a usable event/reward/map state."),
            ("non-vakuu-rewards", "Victory offers non-Vakuu Ancient reward choices and no normal combat card reward."),
            ("no-black-screen", "The screen does not go black, freeze, or require force quit after victory."),
            ("log-clean", "Logs contain no release-blocking exception, stale parent event, room stack, or reward-screen error.")
        ];
    }

    private static IReadOnlyList<(string Id, string Expected)> RequiredVakuuFailureDeathRows()
    {
        return
        [
            ("fight-start-scene", "Dedicated Vakuu fight starts before the failure/death path is tested."),
            ("failure-path", "A non-death failure path exits cleanly if the design exposes one. If not reachable, record why."),
            ("death-path", "Death reaches the expected run-end/game-over flow without stale Ancient UI or hidden reward screens."),
            ("room-state-after-exit", "Room, event, reward, and map state remain coherent after failure/death."),
            ("no-softlock", "The game remains responsive or reaches the expected terminal run state."),
            ("log-clean", "Logs contain no release-blocking exception, stale parent event, room stack, or reward-screen error.")
        ];
    }

    private static IReadOnlyList<(string Id, string Expected)> RequiredVakuuSaveLoadRows()
    {
        return
        [
            ("active-combat-save", "Save succeeds during active Vakuu child combat."),
            ("active-combat-load", "Reload restores the active fight, Vakuu state, contracts, locks, and Blood Debt coherently."),
            ("parent-event-state", "Parent event/room stack state is not lost or duplicated after active-fight reload."),
            ("prefinished-save", "Save succeeds around post-combat reward/return after Vakuu is defeated."),
            ("prefinished-load", "Reload restores the reward/return state without black screen or stale combat room."),
            ("no-duplicate-heal-or-reward", "Reload does not duplicate Ancient heal, Vakuu rewards, normal combat rewards, or parent event cleanup."),
            ("log-clean", "Logs contain no release-blocking save/load, room stack, parent event, or reward-screen error.")
        ];
    }

    private static IReadOnlyList<(string Id, string Expected)> RequiredPreviewToolsRows()
    {
        return
        [
            ("crystal-sphere-button", "Crystal Sphere screen shows the Spire Plus peek control in the expected UI area."),
            ("crystal-sphere-mask-only", "Toggling peek only changes ScryMask visibility/opacity and does not call cell clear/reveal/claim behavior."),
            ("crystal-sphere-no-reward-claim", "No relic, potion, card, curse, or gold reward is granted or consumed by previewing."),
            ("transform-preview-visible", "Transform preview displays a concrete predicted replacement instead of cycling fake random cards."),
            ("transform-preview-matches-result", "The shown transform preview matches the actual transformed card result for the tested card(s)."),
            ("transform-preview-no-state-mutation", "Previewing does not advance real RNG, create real mutable replacement cards, or change deck/reward state before confirmation."),
            ("prismatic-gem-reward-hooks", "Prismatic Gem preview reflects reward-modifying hooks without suppressing later Core reward changes."),
            ("save-reopen-stability", "Save/reopen around preview screens does not change the previewed result or corrupt the reward/room state."),
            ("coop-gate-or-two-client-proof", "Multiplayer behavior is either gated with a clear warning/log or proven with two-client evidence."),
            ("log-clean", "Logs contain no preview-tool exception, RNG drift warning, reward-state error, or co-op desync marker.")
        ];
    }

    private static IReadOnlyList<(string Id, string Expected)> RequiredCoopRows()
    {
        return
        [
            ("coop-host-join-clean-logs", "Host and client load exactly BaseLib plus Spire Plus with matching package hashes and clean logs."),
            ("coop-a11-a20-selection", "A11-A20 selection/start-run behavior is recorded on host and client; selection visibility alone is not gameplay support."),
            ("coop-ancients", "Urda, Morvi, Lotha, and gated Vakuu have explicit host/client disposition notes for reward state and relic visibility."),
            ("coop-root-eyes", "Root Eyes map preview either stays gated in co-op or shows host/client-consistent map state with no desync."),
            ("coop-rootblight", "Rootblight ownership, combat/deck state, and Sprout growth are visible and consistent on host and client."),
            ("coop-save-load-or-reconnect", "Save/load or reconnect behavior is proven with host/client before-after logs, or explicitly deferred by owner."),
            ("coop-preview-tools-disposition", "Crystal Sphere, transform preview, and Prismatic Gem preview have a fairness/disposition note and no desync evidence."),
            ("coop-release-note-disposition", "Final co-op wording is explicit: supported with evidence, gated, unsupported, or owner-approved deferred.")
        ];
    }

    private static string CreateFilledSimpleChecklist(
        string title,
        IReadOnlyList<(string Id, string Expected)> rows)
    {
        var lines = new List<string>
        {
            $"# {title}",
            "",
            "| Scenario ID | Expected behavior | Live result | Evidence file(s) |",
            "| --- | --- | --- | --- |"
        };

        lines.AddRange(rows.Select(row =>
            $"| {row.Id} | {row.Expected} | PASS: synthetic verifier contract row filled. | godot.log; result-note.md |"));

        return string.Join(Environment.NewLine, lines);
    }

    private static string CreateFilledAncientRewardRelicsChecklist()
    {
        var rows = new[]
        {
            ("Urda", "seedbed", "UrdaSeedbedOptionRelic"),
            ("Urda", "humus_pact", "UrdaHumusPactOptionRelic"),
            ("Urda", "molting", "UrdaMoltingOptionRelic"),
            ("Urda", "moss_map", "UrdaMossMapOptionRelic"),
            ("Urda", "trial_branch", "UrdaTrialBranchOptionRelic"),
            ("Urda", "shallow_root_relic", "UrdaShallowRootRelicOptionRelic"),
            ("Urda", "elite_root", "UrdaEliteRootOptionRelic"),
            ("Urda", "rooted_route", "UrdaRootedRouteOptionRelic"),
            ("Urda", "after_rain", "UrdaAfterRainOptionRelic"),
            ("Urda", "root_sight", "UrdaRootSightOptionRelic"),
            ("Urda", "seed_bank", "UrdaSeedBankOptionRelic"),
            ("Morvi", "forbidden_loan", "MorviForbiddenLoanOptionRelic"),
            ("Morvi", "misprint_press", "MorviMisprintPressOptionRelic"),
            ("Morvi", "red_ink_overdraft", "MorviRedInkOverdraftOptionRelic"),
            ("Morvi", "overdue_library", "MorviOverdueLibraryOptionRelic"),
            ("Morvi", "open_book_exam", "MorviOpenBookExamOptionRelic"),
            ("Morvi", "paperstorm", "MorviPaperstormOptionRelic"),
            ("Morvi", "blueprint_proof", "MorviBlueprintProofOptionRelic"),
            ("Morvi", "debt_settlement", "MorviDebtSettlementOptionRelic"),
            ("Lotha", "mirror_rebuttal", "LothaMirrorRebuttalOptionRelic"),
            ("Lotha", "mirror_hall_echo", "LothaMirrorHallEchoOptionRelic"),
            ("Lotha", "presumption", "LothaPresumptionOptionRelic"),
            ("Lotha", "closed_court", "LothaClosedCourtOptionRelic"),
            ("Lotha", "deferred_verdict", "LothaDeferredVerdictOptionRelic"),
            ("Lotha", "death_reprieve", "LothaDeathReprieveOptionRelic"),
            ("Lotha", "single_sentence", "LothaSingleSentenceOptionRelic"),
            ("Lotha", "public_evidence", "LothaPublicEvidenceOptionRelic"),
            ("Vakuu", "fight_option", "VakuuFightOptionRelic"),
            ("Vakuu", "victory_non_vakuu_choices", "Non-Vakuu Act 3 Ancient reward relic choices after winning Vakuu"),
            ("Vakuu event", "sere_talon_pickup", "Vakuu's Sere Talon / \u74e6\u5e93\u539f\u521d\u4e4b\u722a lets the player choose 1 of 4 Curses, then adds that Curse, 2 Wish, and 1 Wish+; verify event-option art, relic-bar art, inspect art, hover text, and surface-specific log routes such as `Ancient event option button`, `RelicModel packed icon texture`, and `RelicModel big icon texture` are not Tanx Claws."),
            ("Tanx event", "claws_maul_transform", "Tanx Claws / \u5766\u514b\u65af\u5229\u722a transforms cards into upgraded Maul / \u6495\u54ac+ cards.")
        };

        var lines = new List<string>
        {
            "# Ancient Reward Visible Relics Checklist",
            "",
            "| Ancient | Reward ID | Expected option/relic | Screen option visible | Relic bar / hover result | Evidence file(s) |",
            "| --- | --- | --- | --- | --- | --- |"
        };

        lines.AddRange(rows.Select(row =>
            $"| {row.Item1} | {row.Item2} | {row.Item3} | PASS: synthetic option relic visible. | PASS: synthetic relic bar hover readable. | godot.log; result-note.md |"));

        return string.Join(Environment.NewLine, lines);
    }

    private static string CreateFilledPlayerTextQaChecklist()
    {
        var rows = new[]
        {
            ("ascension-a11-a20", "A11-A20 descriptions are short, concrete, use Dedicated Ability/Branded Form wording, and avoid stale Royal Seal/King Brand terms."),
            ("firemark-and-banner", "Firemark and Banner text shows current-act values, explains Host/Overflow/Forge Armor/Shieldwall clearly, and avoids slash-table wording in live hovers."),
            ("boss-dedicated-abilities", "A19/A20 Boss power hovers explain the matching Boss ability with final damage/intent implications and multiplayer caps where relevant."),
            ("ancient-choice-text", "Urda, Morvi, Lotha, Vakuu option rows explain what the player gains or risks without implementation terms."),
            ("ancient-relic-hover", "Option relic and selected-relic hover text is readable from the relic bar and matches the actual active reward."),
            ("cards-status-curses", "Blight Sprout, Rootblight, Husk, Seedbed, Contract, Rain Breath, Marginal Note, and generated temporary cards use clear card/status wording."),
            ("map-hover-stacks", "Root Eyes, Firemarked Elite, Banner, Deep Branch, Boss ability, and Branded Form map hovers stack without hiding each other."),
            ("preview-tools-text", "Crystal Sphere peek, transform preview, and Prismatic Gem preview text explain preview behavior without implying reward claim or RNG mutation."),
            ("vakuu-contracts", "Vakuu contracts, Blood Debt, locks, Cash Out, and victory reward choices explain the greed/stop decision and post-fight settlement."),
            ("en-zhs-key-parity", "EN and ZHS keys, dynamic variables, and rich-text tags match for tested surfaces; no mojibake or missing localization appears.")
        };

        var lines = new List<string>
        {
            "# Player Text / Tooltip QA Checklist",
            "",
            "| Surface ID | Expected text quality | EN result | ZHS result | Evidence file(s) |",
            "| --- | --- | --- | --- | --- |"
        };

        lines.AddRange(rows.Select(row =>
            $"| {row.Item1} | {row.Item2} | PASS: synthetic EN text QA row filled. | PASS: synthetic ZHS text QA row filled. | godot.log; result-note.md |"));

        return string.Join(Environment.NewLine, lines);
    }

    private static string CreateFilledArtResourceRoutingChecklist()
    {
        var rows = new[]
        {
            ("title-home-preview", "Spire Plus title/home preview image fits the UI frame and does not stretch, crop critical subject matter, or use stale pre-refresh branding."),
            ("urda-clicked-background", "Urda large background appears only on the clicked Ancient screen/event surface and fits behind option rows."),
            ("morvi-clicked-background", "Morvi large background appears only on the clicked Ancient screen/event surface and fits behind option rows."),
            ("lotha-clicked-background", "Lotha large background appears only on the clicked Ancient screen/event surface and fits behind option rows."),
            ("vakuu-clicked-background", "Vakuu normal and force-fight clicked screens use the intended large art without hiding option text."),
            ("map-icons", "Ancient, Root Eyes, Firemark, Banner, Deep Branch, and other map markers use readable small icons, not full-size event art."),
            ("run-history-icons", "Run-history icons use small-format art and remain distinguishable from map icons and clicked-screen backgrounds."),
            ("option-relic-icons", "Ancient option relic choices use option relic icons and do not reuse clicked-screen backgrounds or placeholder crops."),
            ("lasting-relic-icons", "Selected lasting Ancient rewards appear in the relic bar with readable small icons and hover art/text."),
            ("card-art", "Rootblight, Blight Sprout, Husk, Contract, Rain Breath, Seedbed-related cards, and preview cards use the expected card portraits."),
            ("power-icons", "Firemark, Banner, A19/A20 dedicated abilities, Vakuu powers, Seedbed, and Rootblight powers use visible non-NOPE icons."),
            ("no-placeholder-or-official-art", "No tested surface shows NOPE, generic temporary art, placeholder crops, stale logo text, or copied official non-art source material.")
        };

        var lines = new List<string>
        {
            "# Art / Resource Routing Checklist",
            "",
            "| Surface ID | Expected routing | Live result | Evidence file(s) |",
            "| --- | --- | --- | --- |"
        };

        lines.AddRange(rows.Select(row =>
            $"| {row.Item1} | {row.Item2} | PASS: synthetic art routing verifier contract row filled. | screenshot.png; godot.log; route-note.md |"));

        return string.Join(Environment.NewLine, lines);
    }

    private static string CreateFilledRootblightBehaviorChecklist()
    {
        var rows = new[]
        {
            ("rootblight-start-eligibility", "A14+ run starts/repairs Rootblight setup only after a real deck card exists; no silent permanent disable if the deck is temporarily unavailable."),
            ("normal-rootblight-continuity", "Ordinary normal combats do not add Blight Sprout in the current design; they should still mark existing Rootblight, show it in the deck/hand flow, and resolve combat-end growth/cap rules."),
            ("elite-single-sprout", "Elite combat adds exactly one Blight Sprout at the expected timing and does not duplicate across reload/reentry."),
            ("boss-two-sprouts-staggered", "Act 2/3 Boss combat adds two Blight Sprouts on the staggered turns, with both cards visible when expected."),
            ("husk-exhaust-block-timing", "Withered Husk grants exactly 3 Block when it is Exhausted, not when it merely has Ethereal/Void text or sits in hand."),
            ("combat-end-growth", "An unresolved Blight Sprout grows into Rootblight after combat; handled/planted Sprouts do not grow."),
            ("rootblight-cap-four", "Rootblight respects the current maximum and Rootblight III split/growth rule without exceeding 4 cards."),
            ("rootblight-save-load", "Save/load before Sprout entry, during combat, and after combat preserves pending markers and deck state."),
            ("ui-hover-art-readability", "Blight Sprout, Rootblight, Seedbed/Husk interactions, card art, and EN/ZHS hover text are visible and readable.")
        };

        var lines = new List<string>
        {
            "# Rootblight / Blight Sprout Behavior Checklist",
            "",
            "| Scenario ID | Expected behavior | Live result | Evidence file(s) |",
            "| --- | --- | --- | --- |"
        };

        lines.AddRange(rows.Select(row =>
            $"| {row.Item1} | {row.Item2} | PASS: synthetic Rootblight verifier contract row filled. | godot.log; result-note.md |"));

        return string.Join(Environment.NewLine, lines);
    }

    private static void WriteTinyPng(string path, int width, int height)
    {
        static byte[] UInt32BigEndian(int value)
        {
            return
            [
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF)
            ];
        }

        var bytes = new List<byte>
        {
            137,
            80,
            78,
            71,
            13,
            10,
            26,
            10,
            0,
            0,
            0,
            13,
            (byte)'I',
            (byte)'H',
            (byte)'D',
            (byte)'R'
        };
        bytes.AddRange(UInt32BigEndian(width));
        bytes.AddRange(UInt32BigEndian(height));
        bytes.AddRange(new byte[] { 8, 6, 0, 0, 0, 0, 0, 0, 0 });
        File.WriteAllBytes(path, bytes.ToArray());
    }
}
