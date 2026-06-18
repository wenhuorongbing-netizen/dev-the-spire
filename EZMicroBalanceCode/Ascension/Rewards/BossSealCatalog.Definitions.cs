using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class BossSealCatalog
{
    private const string EncounterCategory = "ENCOUNTER";

    private static readonly IReadOnlyDictionary<ModelId, BossSealDefinition> DefinitionsByEncounter =
        CreateDefinitionsByEncounter();

    private static IReadOnlyDictionary<ModelId, BossSealDefinition> CreateDefinitionsByEncounter()
    {
        var definitions = new Dictionary<ModelId, BossSealDefinition>();

        AddActOneDefinitions(definitions);
        AddActTwoDefinitions(definitions);
        AddActThreeDefinitions(definitions);

        return definitions;
    }

    private static ModelId EncounterId(string entry)
    {
        return new ModelId(EncounterCategory, entry);
    }
}
