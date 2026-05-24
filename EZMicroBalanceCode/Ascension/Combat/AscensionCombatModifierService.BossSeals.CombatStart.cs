using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task ApplyBossSealCombatStart(CombatState combatState, AscensionNodeMetadata metadata)
    {
        var definition = metadata.BossSeal;
        if (definition == null)
        {
            return;
        }

        var mode = metadata.IsBossBrand ? "A20 Branded Form" : "A19 dedicated ability";
        var brandText = metadata.IsBossBrand
            ? $" brand={definition.BrandSummary}"
            : string.Empty;
        MainFile.Logger.Info(
            $"[Spire Plus] Ascension {mode} armed: {definition.Name} ({definition.Id}) is active for this boss. evidence={definition.RuntimeEvidence}{brandText}");

        await ApplyBossSealVisibilityMarker(combatState, definition);

    }

    private static async Task ApplyBossSealVisibilityMarker(
        CombatState combatState,
        BossSealDefinition definition)
    {
        var owner = FindBossSealVisibilityOwner(combatState, definition.Id);
        if (owner == null)
        {
            MainFile.Logger.Warn($"[Spire Plus] Ascension boss dedicated ability marker skipped: no living owner found for {definition.Id}.");
            return;
        }

        switch (definition.Id)
        {
            case BossSealId.HolyDaze:
                await PowerCmd.Apply<HolyDazeBossSealMarkerPower>(new BlockingPlayerChoiceContext(), owner, 1m, owner, null);
                break;
            case BossSealId.MartyrOath:
                await PowerCmd.Apply<MartyrOathBossSealMarkerPower>(new BlockingPlayerChoiceContext(), owner, 1m, owner, null);
                break;
            case BossSealId.InkReturn:
                await PowerCmd.Apply<InkReturnBossSealMarkerPower>(new BlockingPlayerChoiceContext(), owner, 1m, owner, null);
                break;
            case BossSealId.StartledShell:
                await PowerCmd.Apply<StartledShellBossSealMarkerPower>(new BlockingPlayerChoiceContext(), owner, 1m, owner, null);
                break;
            case BossSealId.SoulTide:
                await PowerCmd.Apply<SoulTideBossSealMarkerPower>(new BlockingPlayerChoiceContext(), owner, 1m, owner, null);
                break;
            case BossSealId.BoilingCritical:
                await PowerCmd.Apply<BoilingCriticalBossSealMarkerPower>(new BlockingPlayerChoiceContext(), owner, 1m, owner, null);
                break;
            case BossSealId.MisalignedShell:
                await PowerCmd.Apply<MisalignedShellBossSealMarkerPower>(new BlockingPlayerChoiceContext(), owner, 1m, owner, null);
                break;
            case BossSealId.MarginalNote:
                await PowerCmd.Apply<MarginalNoteBossSealMarkerPower>(new BlockingPlayerChoiceContext(), owner, 1m, owner, null);
                break;
            case BossSealId.StruggleBait:
                await PowerCmd.Apply<StruggleBaitBossSealMarkerPower>(new BlockingPlayerChoiceContext(), owner, 1m, owner, null);
                break;
            case BossSealId.ChosenDecree:
                await PowerCmd.Apply<ChosenDecreeBossSealMarkerPower>(new BlockingPlayerChoiceContext(), owner, 1m, owner, null);
                break;
            case BossSealId.ResidualSample:
                await PowerCmd.Apply<ResidualSampleBossSealMarkerPower>(new BlockingPlayerChoiceContext(), owner, 1m, owner, null);
                break;
            case BossSealId.AeonglassHourglass:
                await PowerCmd.Apply<AeonglassHourglassBossSealMarkerPower>(new BlockingPlayerChoiceContext(), owner, 1m, owner, null);
                break;
        }
    }

    private static Creature? FindBossSealVisibilityOwner(CombatState combatState, BossSealId id) =>
        id switch
        {
            BossSealId.HolyDaze => AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is CeremonialBeast),
            BossSealId.MartyrOath => AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is KinPriest),
            BossSealId.InkReturn => AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is Vantom),
            BossSealId.StartledShell => AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is LagavulinMatriarch),
            BossSealId.SoulTide => AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is SoulFysh),
            BossSealId.BoilingCritical => AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is WaterfallGiant),
            BossSealId.MisalignedShell => AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is Crusher or Rocket) ??
                PrimaryAliveEnemies(combatState).FirstOrDefault(),
            BossSealId.MarginalNote => AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is KnowledgeDemon),
            BossSealId.StruggleBait => AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TheInsatiable),
            BossSealId.ChosenDecree => AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is Queen),
            BossSealId.ResidualSample => AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TestSubject),
            _ => PrimaryAliveEnemies(combatState).FirstOrDefault()
        };
}
