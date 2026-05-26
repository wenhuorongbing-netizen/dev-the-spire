namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed partial class AscensionCombatTracker
{
    public bool Seeded { get; set; }
    public HashSet<RootBud> Buds { get; } = [];
    public HashSet<Player> DiedPlayers { get; } = [];

    public bool CombatModifiersInitialized { get; set; }
    public AscensionNodeMetadata? NodeMetadata { get; set; }
    public bool ForgeTokenAwarded { get; set; }
    public HashSet<Creature> ThresholdShieldedEnemies { get; } = [];
}
