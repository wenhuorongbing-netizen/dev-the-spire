using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension.Events;

internal sealed class A20Courtyard : EventModel
{
    private const string InitialDescriptionKey = "A20_COURTYARD.pages.INITIAL.description";
    private const string ReadyDescriptionKey = "A20_COURTYARD.pages.READY.description";
    private const string ContinueOptionKey = "A20_COURTYARD.pages.INITIAL.options.CONTINUE";
    private const string BrandedFormKey = "BOSS_BRANDED_FORM";

    public override bool IsAllowed(IRunState runState) => false;

    public override IEnumerable<string> GetAssetPaths(IRunState runState)
    {
        return TestMode.IsOn
            ? Array.Empty<string>()
            : new[]
            {
                "res://scenes/events/default_event_layout.tscn",
                GetSecondBossBrandIconPath(runState)
            };
    }

    protected override void SetInitialEventState(bool isPreFinished)
    {
        SetEventState(CreateDescription(InitialDescriptionKey), GenerateInitialOptionsWrapper());
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new[]
        {
            new EventOption(this, ReadyForSecondBoss, ContinueOptionKey).ThatWontSaveToChoiceHistory()
        };
    }

    private Task ReadyForSecondBoss()
    {
        SetEventFinished(CreateDescription(ReadyDescriptionKey));
        return Task.CompletedTask;
    }

    private LocString CreateDescription(string key)
    {
        var description = L10NLookup(key);
        AddBossBrandVariables(description);
        return description;
    }

    private void AddBossBrandVariables(LocString description)
    {
        description.Add("HealPercent", "25");

        var runState = Owner?.RunState;
        var secondBoss = runState?.Act.SecondBossEncounter;
        description.Add("BossName", secondBoss?.Title.GetFormattedText() ?? new LocString("ascension", $"{BrandedFormKey}.title").GetFormattedText());

        var definition = TryGetSecondBossBrandDefinition(runState);
        if (definition == null)
        {
            description.Add("SealName", new LocString("ascension", $"{BrandedFormKey}.title").GetFormattedText());
            description.Add("SealSummary", new LocString("ascension", $"{BrandedFormKey}.description").GetFormattedText());
            return;
        }

        var sealLocKey = BossSealCatalog.GetLocalizationKey(definition.Id);
        description.Add("SealName", new LocString("ascension", $"{sealLocKey}.title").GetFormattedText());
        description.Add("SealSummary", new LocString("ascension", $"{sealLocKey}.brand").GetFormattedText());
    }

    private static BossSealDefinition? TryGetSecondBossBrandDefinition(IRunState? runState)
    {
        if (runState?.Map.SecondBossMapPoint == null)
        {
            return null;
        }

        return AscensionMapService.TryGetMetadata(runState.Map.SecondBossMapPoint)?.BossSeal ??
            BossSealCatalog.TryGetForEncounter(runState.Act.SecondBossEncounter);
    }

    internal static string GetSecondBossBrandIconPath(IRunState? runState)
    {
        var definition = TryGetSecondBossBrandDefinition(runState);
        return definition == null
            ? AscensionAssetPaths.BossSealIndicator
            : AscensionAssetPaths.GetBossSealIndicator(definition.Id);
    }
}

internal static class AscensionA20CourtyardService
{
    private static readonly ConditionalWeakTable<IRunState, CourtyardLaunchMarker> LaunchMarkers = new();

    public static bool ShouldEnterCourtyard(IRunState? runState)
    {
        return runState != null &&
            AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(runState) &&
            runState.CurrentRoomCount == 1 &&
            runState.CurrentRoom?.RoomType == RoomType.Boss &&
            runState.CurrentActIndex == runState.Acts.Count - 1 &&
            runState.Map.SecondBossMapPoint != null &&
            runState.CurrentMapCoord == runState.Map.BossMapPoint.coord;
    }

    public static async Task EnterCourtyard(RunManager runManager, IRunState runState)
    {
        var marker = LaunchMarkers.GetValue(runState, _ => new CourtyardLaunchMarker());
        if (marker.Started)
        {
            MainFile.Logger.Info("[Spire Plus] Ascension A20 courtyard launch ignored because it is already in progress.");
            return;
        }

        marker.Started = true;
        try
        {
            var eventRoom = new EventRoom(ModelDb.Event<A20Courtyard>());
            await runManager.EnterRoomWithoutExitingCurrentRoom(eventRoom, fadeToBlack: true);
            await SaveManager.Instance.SaveRun(eventRoom, saveProgress: false);
            ReleaseEvidenceLog.Log("A20BrandedForm", "courtyard_entered", runState: runState);
            MainFile.Logger.Info("[Spire Plus] Ascension A20 applied: entered the fixed courtyard event between Boss 1 rewards and Boss 2.");
        }
        catch
        {
            marker.Started = false;
            throw;
        }
    }

    private sealed class CourtyardLaunchMarker
    {
        public bool Started { get; set; }
    }
}

[HarmonyPatch(typeof(EventModel), nameof(EventModel.CreateInitialPortrait))]
internal static class AscensionA20CourtyardPortraitPatch
{
    [HarmonyPrefix]
    private static bool Prefix(EventModel __instance, ref Texture2D __result)
    {
        if (__instance is not A20Courtyard)
        {
            return true;
        }

        __result = PreloadManager.Cache.GetTexture2D(A20Courtyard.GetSecondBossBrandIconPath(__instance.Owner?.RunState));
        return false;
    }
}
