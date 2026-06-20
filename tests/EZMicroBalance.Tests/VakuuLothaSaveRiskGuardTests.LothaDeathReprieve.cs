using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class VakuuLothaSaveRiskGuardTests
{
    [Fact]
    public void LothaDeathReprieveOncePerRunAndDuplicateReprieveGuardsStaySourceVisible()
    {
        var runHook = ReadLothaSource();
        var deathReprieve = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.DeathReprieve.cs");
        var combatState = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.CombatState.cs");
        var deathReprieveCombatState = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.DeathReprieveCombatState.cs");
        var deathReprieveState = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.DeathReprieveState.cs");
        var deathReprieveTurn = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.DeathReprieveTurn.cs");
        var state = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.State.cs");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var playerState = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientPlayerState.cs");
        var deathBlock = SliceBetween(deathReprieve, "public static bool ShouldDieLate(Creature creature)", "private static Dictionary<string, object?> DeathReprieveDiagnostics");
        var startBlock = SliceBetween(deathReprieveTurn, "private static async Task StartDeathReprieveTurn", "private static async Task EnsureDeathReprievePower");

        AssertSourceContains(
            savedFields,
            "SavedAttachedState<Player, string> LothaStateKey",
            "SavedAttachedState<CardModel, string> LothaDeckStateKey");
        AssertSourceContains(
            combatState,
            "private sealed partial class LothaCombatState",
            "private static readonly ConditionalWeakTable<Player, LothaCombatState> CombatStates = new();");
        AssertSourceContains(
            deathReprieveCombatState,
            "private sealed partial class LothaCombatState",
            "public bool DeathReprieveActive { get; set; }",
            "public bool DeathReprievePendingStart { get; set; }",
            "public bool DeathReprieveStarted { get; set; }");
        Assert.DoesNotContain("public bool DeathReprieveActive { get; set; }", combatState, StringComparison.Ordinal);
        Assert.DoesNotContain("public bool DeathReprievePendingStart { get; set; }", combatState, StringComparison.Ordinal);
        Assert.DoesNotContain("public bool DeathReprieveStarted { get; set; }", combatState, StringComparison.Ordinal);
        AssertSourceContains(
            playerState,
            "runtimeField[player] = deckState",
            "MirrorToDeck(player, deckField, runtimeState)",
            "deckField[card] = state");
        AssertSourceContains(
            runHook,
            "private enum DeathReprievePhase",
            "PendingStart = 1",
            "Active = 2",
            "Resolved = 3",
            "private sealed record Progress(bool DeathReprieveUsed, DeathReprievePhase DeathReprievePhase)",
            "progress.DeathReprieveUsed ? 1 : 0",
            "(int)progress.DeathReprievePhase",
            "ParseDeathReprievePhase(parts[2], used)",
            "DeathReprieveUsed = true");
        AssertSourceContains(
            deathBlock,
            "if (combatState.DeathReprieveActive || combatState.DeathReprievePendingStart)",
            "return false;",
            "return GetProgress(player).DeathReprieveUsed",
            "if (progress.DeathReprieveUsed)",
            "CreatureCmd.SetCurrentHp(creature, 1m)");
        AssertSourceContains(
            startBlock,
            "if (combatState.DeathReprieveStarted)",
            "return;",
            "combatState.DeathReprieveStarted = true",
            "combatState.DeathReprieveActive = true",
            "combatState.DeathReprievePendingStart = false",
            "DeathReprievePhase = DeathReprievePhase.Active");
        AssertSourceContains(
            deathReprieveTurn,
            "private const int DeathReprieveCards = 10",
            "private const int DeathReprieveEnergy = 10",
            "private static async Task EnsureDeathReprievePower",
            "private static async Task ResolveDeathReprieveTurnEnd",
            "private static bool IsDeathReprieveCostFree");
        AssertSourceContains(
            runHook,
            "HydrateDeathReprieveState(player, combatState)",
            "private static void HydrateDeathReprieveState(Player player, LothaCombatState combatState)",
            "progress.DeathReprievePhase == DeathReprievePhase.Active",
            "deck-mirrored blessing progress",
            "Active-turn save/load continuation remains live-pending",
            "private static void ResolveDeathReprieveProgress(Player player)",
            "DeathReprievePhase = DeathReprievePhase.Resolved");
        AssertSourceContains(
            deathReprieveState,
            "combatState.DeathReprievePendingStart = progress.DeathReprievePhase == DeathReprievePhase.PendingStart",
            "combatState.DeathReprieveStarted = progress.DeathReprievePhase == DeathReprievePhase.Active && alreadyHasPower");
        Assert.DoesNotContain("DeathReprievePendingStart = !alreadyHasPower", deathReprieveState, StringComparison.Ordinal);
        Assert.DoesNotContain("private static void HydrateDeathReprieveState", state, StringComparison.Ordinal);
    }

    [Fact]
    public void LothaDeathReprieveWritesPhaseBeforeStartingOrPendingReprieve()
    {
        var deathReprieve = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.DeathReprieve.cs");
        var deathReprieveTurn = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.DeathReprieveTurn.cs");
        var preventBlock = SliceBetween(deathReprieve, "public static async Task AfterPreventingDeath(Creature creature)", "private static Dictionary<string, object?> DeathReprieveDiagnostics");
        var playerTurnBranch = SliceBetween(preventBlock, "if (creature.CombatState?.CurrentSide == CombatSide.Player", "else");
        var pendingBranch = SliceFrom(preventBlock, "DeathReprievePhase = DeathReprievePhase.PendingStart");
        var startBlock = SliceBetween(deathReprieveTurn, "private static async Task StartDeathReprieveTurn", "private static async Task EnsureDeathReprievePower");

        AssertSourceContains(
            playerTurnBranch,
            "var activeProgress = progress with",
            "SetProgress(player, activeProgress)",
            "DeathReprieveUsed = true",
            "DeathReprievePhase = DeathReprievePhase.Active",
            "StartDeathReprieveTurn");
        AssertBefore(playerTurnBranch, "SetProgress(player, activeProgress)", "StartDeathReprieveTurn");

        AssertSourceContains(
            pendingBranch,
            "DeathReprievePhase = DeathReprievePhase.PendingStart",
            "combatState.DeathReprievePendingStart = true",
            "EnsureDeathReprievePower");
        var pendingPhaseIndex = preventBlock.IndexOf("DeathReprievePhase = DeathReprievePhase.PendingStart", StringComparison.Ordinal);
        var pendingProgressIndex = preventBlock.LastIndexOf("var pendingProgress = progress with", pendingPhaseIndex, StringComparison.Ordinal);
        var pendingSetProgressIndex = preventBlock.IndexOf("SetProgress(player, pendingProgress)", pendingPhaseIndex, StringComparison.Ordinal);
        var pendingFlagIndex = preventBlock.IndexOf("combatState.DeathReprievePendingStart = true", pendingPhaseIndex, StringComparison.Ordinal);
        Assert.True(pendingProgressIndex >= 0, "Pending reprieve branch must build progress from the prior progress record.");
        Assert.True(pendingProgressIndex < pendingPhaseIndex, "Pending reprieve progress record must wrap the PendingStart phase.");
        Assert.True(pendingSetProgressIndex >= 0, "Pending reprieve branch must write progress through SetProgress.");
        Assert.True(pendingPhaseIndex < pendingSetProgressIndex, "PendingStart phase must be part of the pending progress record before SetProgress writes it.");
        Assert.True(pendingPhaseIndex < pendingFlagIndex, "PendingStart phase must be written before transient pending-start flags are set.");
        Assert.True(pendingSetProgressIndex < pendingFlagIndex, "Pending reprieve progress must be written before transient pending-start flags are set.");

        AssertSourceContains(
            startBlock,
            "if (combatState.DeathReprieveStarted)",
            "return;",
            "combatState.DeathReprieveStarted = true",
            "var activeProgress = GetProgress(player) with",
            "SetProgress(player, activeProgress)",
            "DeathReprieveUsed = true",
            "DeathReprievePhase = DeathReprievePhase.Active");
        AssertBefore(startBlock, "SetProgress(player, activeProgress)", "CardPileCmd.Draw(choiceContext, DeathReprieveCards, player)");
    }

    [Fact]
    public void LothaDeathReprieveForceDeathAndPersistenceStanceRemainExplicit()
    {
        var runHook = ReadLothaSource();
        var deathReprieve = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.DeathReprieve.cs");
        var deathReprieveTurn = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.DeathReprieveTurn.cs");
        var apiResearch = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "api-research.md");
        var riskRegister = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "risk-register.md");
        var manualChecklist = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md");
        var deathBlock = SliceBetween(deathReprieve, "public static bool ShouldDieLate(Creature creature)", "private static Dictionary<string, object?> DeathReprieveDiagnostics");
        var resolveBlock = SliceBetween(deathReprieveTurn, "private static async Task ResolveDeathReprieveTurnEnd", "private static bool IsDeathReprieveCostFree");

        AssertSourceContains(
            resolveBlock,
            "ResolveDeathReprieveProgress(player)",
            "PowerCmd.Remove<LothaDeathReprievePower>(player.Creature)",
            "CreatureCmd.Kill(player.Creature, force: true)");
        AssertBefore(resolveBlock, "ResolveDeathReprieveProgress(player)", "PowerCmd.Remove<LothaDeathReprievePower>(player.Creature)");
        AssertBefore(resolveBlock, "PowerCmd.Remove<LothaDeathReprievePower>(player.Creature)", "CreatureCmd.Kill(player.Creature, force: true)");
        Assert.DoesNotContain("ShouldDie(player.Creature)", resolveBlock, StringComparison.Ordinal);
        Assert.DoesNotContain("AfterPreventingDeath(player.Creature)", resolveBlock, StringComparison.Ordinal);

        var hasSourceRecovery =
            runHook.Contains("RecoverDeathReprieve", StringComparison.Ordinal) ||
            runHook.Contains("RestoreDeathReprieve", StringComparison.Ordinal) ||
            runHook.Contains("HydrateDeathReprieveState", StringComparison.Ordinal) &&
            runHook.Contains("DeathReprievePhase.PendingStart", StringComparison.Ordinal) &&
            runHook.Contains("DeathReprievePhase.Active", StringComparison.Ordinal) ||
            runHook.Contains("LothaDeathReprievePower") &&
            deathBlock.Contains("GetPower<LothaDeathReprievePower>()", StringComparison.Ordinal);
        var docsStateLimitation = string.Join(Environment.NewLine, apiResearch, riskRegister, manualChecklist);
        var docsExplicitlyLimitPendingActiveSaveLoad =
            docsStateLimitation.Contains("Death Reprieve", StringComparison.Ordinal) &&
            docsStateLimitation.Contains("pending/active", StringComparison.OrdinalIgnoreCase) &&
            docsStateLimitation.Contains("save/load", StringComparison.OrdinalIgnoreCase) &&
            (docsStateLimitation.Contains("not proven", StringComparison.OrdinalIgnoreCase) ||
             docsStateLimitation.Contains("unsupported", StringComparison.OrdinalIgnoreCase) ||
             docsStateLimitation.Contains("unverified", StringComparison.OrdinalIgnoreCase));

        Assert.True(
            hasSourceRecovery || docsExplicitlyLimitPendingActiveSaveLoad,
            "Death Reprieve must either recover active/pending reprieve state from a source-supported persistent carrier " +
            "or keep active docs/manual tests explicit that pending/active reprieve save/load is not proven safe.");
    }

    [LocalSourceFact]
    public void CoreCreatureKillForceFlagStillBypassesShouldDie()
    {
        var creatureCmd = ReadLocalCoreText("Commands", "CreatureCmd.cs");

        AssertSourceContains(
            creatureCmd,
            "public static async Task Kill(Creature creature, bool force = false)",
            "if (force || creature.MaxHp <= 0 || Hook.ShouldDie(runState, combatState, creature, out preventer))");
    }

    private static string ReadLothaSource() =>
        ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha");
}
