using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseCoverageGuardTests
{
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
