using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientExpansionReleaseCoverageGuardTests
{
    [Fact]
    public void VakuuFightIsSinglePlayerGatedLocalizedAndResumeSafe()
    {
        var gate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs");
        var patch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");
        var command = string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Diagnostics", "SpirePlusAncientLiveTestConsoleCmd.cs"),
            ReadRepoText("EZMicroBalanceCode", "Diagnostics", "SpirePlusAncientLiveTestConsoleCmd.RunSetup.cs"));
        var vakuuSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu");
        var victory = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu");
        var noReward = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.NoRewardResume.cs");
        var encounter = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightEncounter.cs");
        var monster = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuTrialMonster.cs");
        var optionRelic = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightOptionRelic.cs");
        var ritsuRegistration = ReadSourceTree("EZMicroBalanceCode", "Core", "Integrations", "RitsuLib");
        var assetPaths = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightAssetPaths.cs");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var engEncounters = JsonStringMap("EZMicroBalance", "localization", "eng", "encounters.json");
        var zhsEncounters = JsonStringMap("EZMicroBalance", "localization", "zhs", "encounters.json");
        var engMonsters = JsonStringMap("EZMicroBalance", "localization", "eng", "monsters.json");
        var zhsMonsters = JsonStringMap("EZMicroBalance", "localization", "zhs", "monsters.json");

        AssertSourceContains(
            gate,
            "EZMB_ENABLE_VAKUU_FIGHT",
            "SPIREPLUS_ENABLE_VAKUU_FIGHT",
            "EZMB_DISABLE_VAKUU_FIGHT",
            "SPIREPLUS_DISABLE_VAKUU_FIGHT",
            "EZMB_FORCE_ANCIENT",
            "SPIREPLUS_FORCE_ANCIENT",
            "EZMB_FORCE_VAKUU_FIGHT",
            "SPIREPLUS_FORCE_VAKUU_FIGHT",
            "ShouldForceVakuu",
            "ShouldForceFight",
            "ShouldForceFightForRun",
            "ArmCommandForceFight",
            "ConsumeCommandForceFightForRun",
            "HasCommandForceFightForRun",
            "ClearCommandForceFightWhenBeginEventCompletes",
            "finally",
            "ShouldEnableFight",
            "runState.Players.Count == 1");
        AssertSourceContains(
            patch,
            "VakuuForceAncientPatch : IPatchMethod",
            "IPatchMethod.PatchId => \"vakuu-force-ancient-unlock\"",
            "new ModPatchTarget(typeof(Glory), nameof(Glory.GetUnlockedAncients))",
            "ModelDb.AncientEvent<MegaCrit.Sts2.Core.Models.Events.Vakuu>()",
            "VakuuFightOptionPatch : IPatchMethod",
            "IPatchMethod.PatchId => \"vakuu-fight-option\"",
            "new ModPatchTarget(typeof(MegaCrit.Sts2.Core.Models.Events.Vakuu), \"GenerateInitialOptions\")",
            "VakuuFightFeatureGate.ShouldForceFightForRun(runState)",
            "VakuuFightFeatureGate.ConsumeCommandForceFightForRun(runState)",
            "VakuuFightCommandForceCleanupPatch : IPatchMethod",
            "IPatchMethod.PatchId => \"vakuu-fight-command-force-cleanup\"",
            "new ModPatchTarget(typeof(EventModel), nameof(EventModel.BeginEvent), [typeof(Player), typeof(bool)])",
            "VakuuFightFeatureGate.HasCommandForceFightForRun(runState)",
            "VakuuFightFeatureGate.ClearCommandForceFightWhenBeginEventCompletes(__result, runState)",
            "VakuuFightResumePatch : IPatchMethod",
            "IPatchMethod.PatchId => \"vakuu-fight-victory-resume\"",
            "new ModPatchTarget(typeof(EventModel), nameof(EventModel.Resume), [typeof(AbstractRoom)])",
            "[HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.OfferRoomEndRewards))]");
        var vakuuCommandSource = string.Join(Environment.NewLine, command, vakuuSource);
        Assert.False(
            Regex.IsMatch(
                vakuuCommandSource,
                @"\b(?:System\s*\.\s*)?Environment\s*\.\s*SetEnvironmentVariable\s*\(",
                RegexOptions.CultureInvariant),
            "Vakuu command force fight must not mutate process environment variables.");
        Assert.DoesNotContain("ForceVakuuFightEnvironmentForCommand", vakuuCommandSource, StringComparison.Ordinal);
        AssertSourceContains(
            vakuuSource,
            "EventOption.FromRelic",
            "SetEventState",
            "EventNodeBackingField",
            "ClearEventNode(vakuu)",
            "EnterRoomWithoutExitingCurrentRoom(combatRoom, fadeToBlack: true)",
            "ModelDb.Encounter<EzmbVakuuTrialEncounter>()");
        AssertSourceContains(
            vakuuSource,
            "EnsureStolenVaultPower",
            "PowerCmd.ModifyAmount",
            "SignContract",
            "BreakLock(choiceContext, combatState, \"contract\")");
        AssertSourceContains(
            vakuuSource,
            "internal sealed class VakuuStolenVaultPower",
            "internal sealed class VakuuBloodDebtPower",
            "public override PowerStackType StackType => PowerStackType.Counter",
            "public override int DisplayAmount => Amount");
        Assert.True(
            Regex.Matches(vakuuSource, "public override int DisplayAmount => Amount").Count >= 2,
            "Vakuu fight amount-bearing powers should show their live counter values.");
        AssertSourceContains(
            noReward,
            "ProceedFromNoRewardVictory",
            "ProceedFromMissingParentStackNoRewardVictory",
            "RunManager.Instance.ProceedFromTerminalRewardsScreen()");
        AssertSourceContains(
            victory,
            "targetChoiceCount = encounter.VictoryChoiceCount",
            "choice.Relic.Owner = owner",
            "encounter.VictoryGold",
            "Nonupeipe",
            "Tanx",
            "GetLothaAct3AncientRelicChoices",
            "LothaFeatureGate.IsLothaEnabled",
            "LothaBlessingService.GetSelectedBlessing(owner)",
            "IsEligibleLothaVictoryChoice(owner, blessingId)",
            "LothaBlessingService.HasMirrorRebuttalCandidates(owner)",
            "LothaRewardSelectionService.SelectBlessing<T>",
            "LothaMirrorRebuttalOptionRelic",
            "LothaPublicEvidenceOptionRelic",
            "RelicCmd.Obtain(mutableRelic, owner)",
            "vakuu.StartPreFinished()");
        Assert.DoesNotContain("LinkedRewardSet", patch + victory, StringComparison.Ordinal);
        Assert.DoesNotContain("ExtraRewards", patch + victory, StringComparison.Ordinal);
        AssertSourceContains(
            encounter,
            "ModEncounterTemplate",
            "public override RoomType RoomType => RoomType.Monster",
            "CustomEncounterScenePath => VakuuFightAssetPaths.EncounterScene",
            "HasScene => true",
            "ShouldGiveRewards => false",
            "Slots => [VakuuSlot]",
            "ModelDb.Monster<EzmbVakuuTrialMonster>()",
            "IsValidForAct(ActModel act) => false");
        AssertSourceContains(
            monster,
            "ModMonsterTemplate",
            "public const string MonsterId = \"EZMB_VAKUU_TRIAL_MONSTER\"",
            "CustomVisualsPath => VakuuFightAssetPaths.MonsterVisual",
            "VisualScale = 1.25f",
            "public override async Task AfterAddedToRoom()",
            "VakuuFightService.EnsureStolenVaultPower(Creature)",
            "GenerateMoveStateMachine",
            "WeakPower",
            "VulnerablePower",
            "StrengthPower");
        AssertSourceContains(
            assetPaths,
            "OptionIcon => $\"{MainFile.ResPath}/images/ancients/vakuu/options/vakuu_fight.png\"",
            "MonsterVisual => $\"{MainFile.ResPath}/images/monsters/vakuu_trial.png\"");
        Assert.DoesNotContain("MonsterVisual => OptionIcon", assetPaths, StringComparison.Ordinal);
        AssertSourceContains(
            optionRelic,
            "CustomIconPath => VakuuFightAssetPaths.OptionIcon",
            "IsAllowed(IRunState runState) => false",
            "IsAllowedAtNeow(Player player) => false",
            "IsAllowedInShops => false");
        Assert.Contains("content.Relic<SharedRelicPool, VakuuFightOptionRelic>();", ritsuRegistration, StringComparison.Ordinal);
        Assert.DoesNotContain("[Pool(typeof(SharedRelicPool))]", optionRelic, StringComparison.Ordinal);

        AssertLocalizedKeys(
            [
                "VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.title",
                "VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description",
                "EZMB_VAKUU_FIGHT.pages.VICTORY.description",
                "EZMB_VAKUU_FIGHT.pages.DONE.description"
            ],
            engAncients,
            zhsAncients,
            "Vakuu fight Ancient localization");
        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.title",
                "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description",
                "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.flavor"
            ],
            engRelics,
            zhsRelics,
            "Vakuu fight option relic localization");
        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-EZMB_VAKUU_TRIAL_ENCOUNTER.title",
                "EZMICROBALANCE-EZMB_VAKUU_TRIAL_ENCOUNTER.loss"
            ],
            engEncounters,
            zhsEncounters,
            "Vakuu fight encounter localization");
        AssertLocalizedKeys(
            [
                "EZMB_VAKUU_TRIAL_MONSTER.name",
                "EZMB_VAKUU_TRIAL_MONSTER.moves.OPENING_OFFER.title",
                "EZMB_VAKUU_TRIAL_MONSTER.moves.KNIFE_RAIN.title",
                "EZMB_VAKUU_TRIAL_MONSTER.moves.GILDED_HIDE.title",
                "EZMB_VAKUU_TRIAL_MONSTER.moves.DEBT_CALL.title"
            ],
            engMonsters,
            zhsMonsters,
            "Vakuu monster localization");

        AssertRepoFileExists("EZMicroBalance", "images", "ancients", "vakuu", "options", "vakuu_fight.png");
        AssertRepoFileExists("EZMicroBalance", "images", "monsters", "vakuu_trial.png");
        AssertRepoFileExists("EZMicroBalance", "images", "encounters", "vakuu_trial_backdrop.png");
        AssertRepoFileExists("EZMicroBalance", "scenes", "encounters", "ezmb_vakuu_trial.tscn");
        Assert.Contains("res://EZMicroBalance/images/ancients/vakuu/options/vakuu_fight.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/images/monsters/vakuu_trial.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/images/encounters/vakuu_trial_backdrop.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/scenes/encounters/ezmb_vakuu_trial.tscn", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/localization/eng/encounters.json", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/localization/zhs/encounters.json", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/localization/eng/monsters.json", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/localization/zhs/monsters.json", exportPreset, StringComparison.Ordinal);
    }
}
