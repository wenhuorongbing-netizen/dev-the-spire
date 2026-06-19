using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientUiReadinessGuardTests
{
    [Fact]
    public void VakuuFightHasDedicatedEncounterSceneMonsterAndLocalization()
    {
        var encounter = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightEncounter.cs");
        var monster = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuTrialMonster.cs");
        var assetPaths = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightAssetPaths.cs");
        var scene = ReadRepoText("EZMicroBalance", "scenes", "encounters", "ezmb_vakuu_trial.tscn");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var engMonsters = JsonStringMap("EZMicroBalance", "localization", "eng", "monsters.json");
        var zhsMonsters = JsonStringMap("EZMicroBalance", "localization", "zhs", "monsters.json");

        AssertSourceContains(
            encounter,
            "CustomScenePath => VakuuFightAssetPaths.EncounterScene",
            "HasScene => true",
            "Slots => [VakuuSlot]",
            "ModelDb.Monster<EzmbVakuuTrialMonster>()");
        Assert.DoesNotContain("OwlMagistrate", encounter, StringComparison.Ordinal);
        AssertSourceContains(
            monster,
            "CustomMonsterModel",
            "CustomVisualPath => VakuuFightAssetPaths.MonsterVisual",
            "VisualScale = 1.25f",
            "GenerateMoveStateMachine",
            "OpeningOfferMove",
            "KnifeRainMove",
            "GildedHideMove",
            "DebtCallMove");
        AssertSourceContains(
            assetPaths,
            "OptionIcon => $\"{MainFile.ResPath}/images/ancients/vakuu/options/vakuu_fight.png\"",
            "MonsterVisual => $\"{MainFile.ResPath}/images/monsters/vakuu_trial.png\"");
        AssertSourceContains(
            scene,
            "res://EZMicroBalance/images/encounters/vakuu_trial_backdrop.png",
            "[node name=\"EzmbVakuuTrialEncounter\" type=\"Control\"]",
            "offset_right = 1920.0",
            "offset_bottom = 1080.0",
            "[node name=\"Vakuu\" type=\"Marker2D\" parent=\".\"]");
        Assert.DoesNotContain("images/card_portraits/big/vakuu_temptation.png", scene, StringComparison.Ordinal);

        AssertLocalizedValue(engMonsters, "EZMB_VAKUU_TRIAL_MONSTER.name");
        AssertLocalizedValue(zhsMonsters, "EZMB_VAKUU_TRIAL_MONSTER.name");
        Assert.Contains("res://EZMicroBalance/images/monsters/vakuu_trial.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/images/encounters/vakuu_trial_backdrop.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/scenes/encounters/ezmb_vakuu_trial.tscn", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/localization/eng/monsters.json", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/localization/zhs/monsters.json", exportPreset, StringComparison.Ordinal);
    }
}
