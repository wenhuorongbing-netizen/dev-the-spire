using Xunit;

namespace EZMicroBalance.Tests;

public sealed class SourceApiDriftAuditGuardTests
{
    [Fact]
    public void V0106SourceApiDriftAuditRecordsCurrentPrimaryEvidence()
    {
        var audit = ReadRepoText("docs", "audits", "v0.106-source-api-drift.md");
        var docsIndex = ReadRepoText("docs", "README.md");

        AssertSourceContains(
            audit,
            "v0.106.1",
            "Alchyr.Sts2.BaseLib` `v3.1.4",
            "`source code/src/Core/**`",
            "`sourcecodeonlyaianalysis/**` is not present",
            "do not use `v0.105.x` notes as the sole implementation basis",
            "`source code/src/Core/Commands/CardPileCmd.cs`",
            "clonedBy",
            "`Hook.AfterCardChangedPiles`",
            "`source code/src/Core/Entities/CardRewardAlternatives/CardRewardAlternative.cs`",
            "throws if alternatives exceed two",
            "`source code/src/Core/Rewards/CardReward.cs`",
            "`CardReward.OnSkipped()`",
            "`source code/src/Core/Models/AncientEventModel.cs`",
            "`Hook.ShouldAllowAncient`",
            "`source code/src/Core/MonsterMoves/Intents/AttackIntent.cs`",
            "`Hook.ModifyDamage",
            "ModifyDamageHookType.All",
            "`source code/src/Core/Runs/RunManager.cs`",
            "`act.SetSecondBossEncounter",
            "`RunManager.ProceedFromTerminalRewardsScreen()`",
            "`source code/src/Core/Commands/CreatureCmd.cs`",
            "`Hook.ShouldDie`",
            "`Hook.AfterPreventingDeath`",
            "`LizardTail`",
            "`FairyInABottle`",
            "`source code/src/Core/Multiplayer/Game/JoinFlow.cs`",
            "`source code/src/Core/Multiplayer/Game/Lobby/StartRunLobby.cs`",
            "`SyncAscensionChange`",
            "`source code/src/Core/Debug/ReleaseInfoManager.cs`",
            "`ModConfigRegistry.Register",
            "`Alchyr.Sts2.BaseLib` `3.1.4`",
            "no-black-screen victory return remains live-only",
            "two-client proof");

        Assert.Contains("`audits/v0.106-source-api-drift.md`", docsIndex, StringComparison.Ordinal);
    }

    [Fact]
    public void LocalCoreSnapshotStillExposesAuditedApiShapes()
    {
        var cardPileCmd = ReadRepoText("source code", "src", "Core", "Commands", "CardPileCmd.cs");
        var cardRewardAlternative = ReadRepoText("source code", "src", "Core", "Entities", "CardRewardAlternatives", "CardRewardAlternative.cs");
        var cardReward = ReadRepoText("source code", "src", "Core", "Rewards", "CardReward.cs");
        var ancientEventModel = ReadRepoText("source code", "src", "Core", "Models", "AncientEventModel.cs");
        var attackIntent = ReadRepoText("source code", "src", "Core", "MonsterMoves", "Intents", "AttackIntent.cs");
        var runManager = ReadRepoText("source code", "src", "Core", "Runs", "RunManager.cs");
        var creatureCmd = ReadRepoText("source code", "src", "Core", "Commands", "CreatureCmd.cs");
        var startRunLobby = ReadRepoText("source code", "src", "Core", "Multiplayer", "Game", "Lobby", "StartRunLobby.cs");
        var joinFlow = ReadRepoText("source code", "src", "Core", "Multiplayer", "Game", "JoinFlow.cs");
        var project = ReadRepoText("EZMicroBalance.csproj");
        var mainFile = ReadRepoText("EZMicroBalanceCode", "MainFile.cs");

        AssertSourceContains(
            cardPileCmd,
            "AbstractModel? clonedBy = null",
            "Hook.AfterCardChangedPiles",
            "AddGeneratedCardToCombat",
            "Hook.AfterCardGeneratedForCombat");

        AssertSourceContains(
            cardRewardAlternative,
            "Hook.ModifyCardRewardAlternatives",
            "More than 2 card reward alternatives are not supported.");

        AssertSourceContains(
            cardReward,
            "PostAlternateCardRewardAction.EndSelectionAndCompleteReward",
            "public override void OnSkipped()");

        AssertSourceContains(
            ancientEventModel,
            "protected override async Task BeforeEventStarted",
            "RunManager.Instance.HasAscension(AscensionLevel.WearyTraveler)",
            "GeneratedOptions = GenerateInitialOptions().ToList()",
            "Hook.ShouldAllowAncient");

        AssertSourceContains(
            attackIntent,
            "Hook.ModifyDamage",
            "ModifyDamageHookType.All",
            "CardPreviewMode.None");

        AssertSourceContains(
            runManager,
            "AscensionLevel.DoubleBoss",
            "act.SetSecondBossEncounter",
            "ProceedFromTerminalRewardsScreen",
            "ShouldResumeParentEventAfterCombat");

        AssertSourceContains(
            creatureCmd,
            "Hook.ShouldDie",
            "Hook.AfterPreventingDeath",
            "force || creature.MaxHp <= 0",
            "recursion >= 10");

        AssertSourceContains(
            startRunLobby,
            "maxMultiplayerAscensionUnlocked",
            "PreferredMultiplayerAscension",
            "SyncAscensionChange");

        AssertSourceContains(
            joinFlow,
            "Version mismatch",
            "Mod mismatch",
            "Model ID hash");

        AssertSourceContains(
            project,
            "Alchyr.Sts2.BaseLib\" Version=\"3.1.4",
            "Include=\"0Harmony\"",
            "Include=\"sts2\"");

        Assert.Contains("ModConfigRegistry.Register(ModId, new SpirePlusModConfig())", mainFile, StringComparison.Ordinal);
    }

}
