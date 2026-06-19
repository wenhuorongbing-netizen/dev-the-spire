using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class VakuuTemptationGuardTests
{
    [LocalSourceFact]
    public void CoreBattlewornDummyEventEncounterStillUsesMonsterNoRewardShape()
    {
        var battlewornDummy = ReadLocalCoreText("Models", "Encounters", "BattlewornDummyEventEncounter.cs");

        AssertSourceContains(
            battlewornDummy,
            "public override RoomType RoomType => RoomType.Monster",
            "public override bool ShouldGiveRewards => false");
    }

    [LocalSourceFact]
    public void CorePlayerTurnDrawStillPrecedesAfterPlayerTurnStart()
    {
        var combatManager = ReadLocalCoreText("Combat", "CombatManager.cs");
        var setupPlayerTurn = SliceBetween(
            combatManager,
            "private async Task SetupPlayerTurn",
            "public void SetReadyToEndTurn");

        AssertSourceContains(
            setupPlayerTurn,
            "await Hook.BeforeHandDraw(state, player, playerChoiceContext)",
            "await CardPileCmd.Draw(playerChoiceContext, handDraw, player, fromHandDraw: true)",
            "await Hook.AfterPlayerTurnStart(state, playerChoiceContext, player)");
        AssertBefore(
            setupPlayerTurn,
            "await CardPileCmd.Draw(playerChoiceContext, handDraw, player, fromHandDraw: true)",
            "await Hook.AfterPlayerTurnStart(state, playerChoiceContext, player)");
    }
}
