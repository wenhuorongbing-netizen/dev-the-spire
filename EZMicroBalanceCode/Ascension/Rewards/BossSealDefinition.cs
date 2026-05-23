using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal enum BossSealId
{
    HolyDaze,
    MartyrOath,
    InkReturn,
    StartledShell,
    SoulTide,
    BoilingCritical,
    MisalignedShell,
    MarginalNote,
    StruggleBait,
    ChosenDecree,
    ResidualSample,
    AeonglassHourglass
}

internal enum BossSealImplementationStatus
{
    SourceGuardedPendingLiveVerification
}

internal sealed record BossSealDefinition(
    BossSealId Id,
    string Name,
    string Summary,
    BossSealImplementationStatus Status,
    string RuntimeEvidence,
    string BrandSummary);

internal static partial class BossSealCatalog
{
    public static BossSealDefinition? TryGetForEncounter(EncounterModel? encounter)
    {
        return encounter != null && DefinitionsByEncounter.TryGetValue(encounter.Id, out var definition)
            ? definition
            : null;
    }
}
