using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Runs;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static void MarkBossSeals(IRunState runState, ActMap map, int actIndex)
    {
        var bossSealsEnabled = AscensionFeatureGate.IsBossSealsEnabled(runState);
        var brandedFormEnabled = AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(runState);
        if (!bossSealsEnabled && !brandedFormEnabled)
        {
            return;
        }

        if (bossSealsEnabled)
        {
            var bossSeal = BossSealCatalog.TryGetForEncounter(runState.Act.BossEncounter);
            if (bossSeal == null)
            {
                MainFile.Logger.Info(
                    $"[Spire Plus] Ascension A19 gate active: no boss dedicated ability definition was found for {runState.Act.BossEncounter.Id}.");
            }
            else
            {
                var bossMetadata = GetOrCreateMetadata(map.BossMapPoint);
                bossMetadata.BossSeal = bossSeal;
                bossMetadata.IsBossBrand = false;
                MainFile.Logger.Info(
                    $"[Spire Plus] Ascension A19 armed: boss node marked with {bossSeal.Name} ({bossSeal.Id}); status={bossSeal.Status}.");
            }
        }

        if (!brandedFormEnabled)
        {
            if (AscensionFeatureGate.IsBrandedFormEnabled(runState))
            {
                ReleaseEvidenceLog.Log(
                    "A20BrandedForm",
                    "second_boss_brand_gated",
                    runState: runState,
                    data: new Dictionary<string, object?>
                    {
                        ["reason"] = "multiplayer_policy"
                    });
            }

            return;
        }

        if (map.SecondBossMapPoint == null)
        {
            MainFile.Logger.Info(
                "[Spire Plus] Ascension A20 gate active: no second boss map point exists, so Boss 2 Brand metadata, reveal, courtyard, and intermission remain inactive.");
            return;
        }

        var secondBossSeal = BossSealCatalog.TryGetForEncounter(runState.Act.SecondBossEncounter);
        if (secondBossSeal == null)
        {
            MainFile.Logger.Info(
                "[Spire Plus] Ascension A20 gate active: second boss map point exists, but no second boss dedicated ability definition was found.");
            return;
        }

        var secondBossMetadata = GetOrCreateMetadata(map.SecondBossMapPoint);
        secondBossMetadata.BossSeal = secondBossSeal;
        secondBossMetadata.IsBossBrand = true;
        ReleaseEvidenceLog.Log(
            "A20BrandedForm",
            "second_boss_brand_marked",
            runState: runState,
            data: new Dictionary<string, object?>
            {
                ["seal"] = secondBossSeal.Id.ToString()
            });
        ReleaseEvidenceLog.Log(
            "A20BrandedForm",
            "boss_marker_applied",
            runState: runState,
            data: new Dictionary<string, object?>
            {
                ["seal"] = secondBossSeal.Id.ToString(),
                ["boss"] = "second"
            });
        MainFile.Logger.Info(
            $"[Spire Plus] Ascension A20 armed: second boss node marked with {secondBossSeal.Name} Brand ({secondBossSeal.Id}); vanilla boss map icons reveal the boss order, and the fixed courtyard event is ready after Boss 1 rewards.");
    }
}
