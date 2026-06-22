using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientUiReadinessGuardTests
{
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
            "EZMB_ENABLE_VAKUU_FIGHT",
            "SPIREPLUS_ENABLE_VAKUU_FIGHT",
            "EZMB_DISABLE_VAKUU_FIGHT",
            "SPIREPLUS_DISABLE_VAKUU_FIGHT");

        var docs = string.Join(
            Environment.NewLine,
            ReadRepoText("docs", "private-beta-verification-handoff.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md"));
        AssertSourceContains(
            docs,
            "scripts/collect-ancient-ui-evidence.ps1",
            "ancient-ui-evidence-plan.json",
            "manual-instructions.md",
            "spireplus_test_ancient URDA confirm",
            "spireplus_test_ancient MORVI confirm",
            "spireplus_test_ancient LOTHA confirm",
            "spireplus_test_ancient VAKUU confirm",
            "spireplus_test_ancient VAKUU confirm fight",
            "starts an unsaved single-player test run",
            "Expected visible option counts are Urda 4, Morvi 3, Lotha 3",
            "Vakuu 3 by default",
            "one fight option",
            "Legacy active-run DevConsole",
            "ancient EZMB_URDA",
            "ancient EZMB_MORVI",
            "ancient EZMB_LOTHA",
            "ancient VAKUU");
    }

    [Fact]
    public void AncientClickedUiEvidenceHelperIsSourceGuardedAndDocumented()
    {
        var helper = ReadRepoText("scripts", "collect-ancient-ui-evidence.ps1");
        AssertRepoFileExists("scripts", "collect-ancient-ui-evidence.ps1");
        AssertSourceContains(
            helper,
            "[ValidateSet('URDA', 'MORVI', 'LOTHA', 'VAKUU')]",
            "ancient-ui-evidence-plan.json",
            "manual-instructions.md",
            "SPIREPLUS_FORCE_ANCIENT",
            "EZMB_FORCE_ANCIENT",
            "SPIREPLUS_FORCE_VAKUU_FIGHT",
            "EZMB_FORCE_VAKUU_FIGHT",
            "PreferredUnsavedDevConsoleCommand",
            "LegacyActiveRunDevConsoleCommand",
            "Legacy active-run DevConsole render-smoke command",
            "spireplus_test_ancient URDA confirm",
            "spireplus_test_ancient VAKUU confirm fight",
            "capture-spire-window.ps1",
            "URDA = 4",
            "MORVI = 3",
            "LOTHA = 3",
            "VakuuNormal = 3",
            "VakuuFightOptInSinglePlayer = 4",
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
            "spireplus_test_ancient URDA confirm",
            "This helper and command prepare UI evidence",
            "Beta.107 already captured forced Urda, Morvi, Lotha, and normal Vakuu clicked-screen screenshots/logs.",
            "Keep the gated Vakuu fight-option, hover/readability, relic-bar follow-through, save-load, co-op, and gameplay rows pending");
        Assert.DoesNotContain(
            string.Concat(
                "Keep all ",
                "clicked UI rows below pending until ",
                "screenshots/logs prove the live screens."),
            docs,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            string.Concat(
                "Keep this section pending until Urda, Morvi, ",
                "Lotha, and Vakuu ",
                "clicked-screen screenshots/logs are captured."),
            docs,
            StringComparison.Ordinal);

        var issues = ReadRepoText("docs", "issues.md");
        AssertSourceContains(
            issues,
            "ANCIENT-CLICKED-UI/LIVE-GAMEPLAY",
            "scripts/collect-ancient-ui-evidence.ps1");

        Assert.DoesNotContain("helper verifies clicked UI", docs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("helper proves clicked UI", docs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("clicked UI verified by the helper", docs, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SpirePlusAncientUiSmokeCommandStartsOnlyUnsavedFreshRuns()
    {
        var command = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Diagnostics", "SpirePlusAncientLiveTestConsoleCmd.cs"),
            ReadRepoText("EZMicroBalanceCode", "Diagnostics", "SpirePlusAncientLiveTestConsoleCmd.RunSetup.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs"));
        AssertSourceContains(
            command,
            "SpirePlusAncientLiveTestConsoleCmd",
            "CmdName => \"spireplus_test_ancient\"",
            "ConfirmationToken = \"confirm\"",
            "FightToken = \"fight\"",
            "RunManager.Instance.IsInProgress",
            "Return to the main menu before using it.",
            "shouldSave: false",
            "ModelDb.Character<Ironclad>()",
            "ActModel.GetDefaultList()",
            "RoomType.Event",
            "MapPointType.Ancient",
            "ModelDb.AncientEvent<EzmbUrda>()",
            "ModelDb.AncientEvent<EzmbMorvi>()",
            "ModelDb.AncientEvent<EzmbLotha>()",
            "ModelDb.AncientEvent<Vakuu>()",
            "RunManager.Instance.DebugOnlyGetState()",
            "VakuuFightFeatureGate.ArmCommandForceFight(commandForceFightRunState)",
            "VakuuFightFeatureGate.ClearCommandForceFight(commandForceFightRunState)",
            "var forceFight = VakuuFightFeatureGate.ShouldForceFightForRun(runState)",
            "VakuuFightFeatureGate.IsFightEnabledForRun(runState, forceFight)",
            "VakuuFightFeatureGate.ConsumeCommandForceFightForRun(runState)",
            "[HarmonyPatch(typeof(EventModel), nameof(EventModel.BeginEvent))]",
            "VakuuFightFeatureGate.HasCommandForceFightForRun(runState)",
            "VakuuFightFeatureGate.ClearCommandForceFightWhenBeginEventCompletes(__result, runState)",
            "finally",
            "commandForcedFightRun = new WeakReference<IRunState>(runState)",
            "ReferenceEquals(target, runState)");

        var vakuuCommandAndFeatureSource = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Diagnostics", "SpirePlusAncientLiveTestConsoleCmd.cs"),
            ReadRepoText("EZMicroBalanceCode", "Diagnostics", "SpirePlusAncientLiveTestConsoleCmd.RunSetup.cs"),
            ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu"));
        AssertNoProcessEnvironmentMutationForVakuuCommand(vakuuCommandAndFeatureSource);

        Assert.DoesNotContain("shouldSave: true", command, StringComparison.Ordinal);

        var helperDocs = string.Join(
            Environment.NewLine,
            ReadRepoText("scripts", "README.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md"));
        AssertSourceContains(
            helperDocs,
            "starts an unsaved single-player test run",
            "refuses to run over an existing run",
            "only UI smoke");
    }

    private static void AssertNoProcessEnvironmentMutationForVakuuCommand(string source)
    {
        Assert.False(
            Regex.IsMatch(
                source,
                @"\b(?:System\s*\.\s*)?Environment\s*\.\s*SetEnvironmentVariable\s*\(",
                RegexOptions.CultureInvariant),
            "Vakuu command-scoped force fight must use a run-scoped marker, not process environment mutation.");
        Assert.DoesNotContain("ForceVakuuFightEnvironmentForCommand", source, StringComparison.Ordinal);
    }
}
