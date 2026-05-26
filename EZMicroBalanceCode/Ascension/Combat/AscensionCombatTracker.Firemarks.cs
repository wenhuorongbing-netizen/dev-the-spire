namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed partial class AscensionCombatTracker
{
    public Creature? FiremarkHost { get; set; }
    public decimal FiremarkBaseAmount { get; set; }
    public decimal FiremarkOriginalMaxHp { get; set; }
    public decimal FiremarkDamageThisPlayerTurn { get; set; }
    public decimal FiremarkDamageThisEnemyCycle { get; set; }
    public int FiremarkDamageTrackingRound { get; set; }
    public bool FiremarkCoreExposed { get; set; }
    public bool FiremarkCoreResolved { get; set; }
    public decimal FiremarkCoreDamage { get; set; }
    public decimal FiremarkCoreDamageNeeded { get; set; }
    public bool FiremarkArmorSkippedNextTurn { get; set; }
    public bool FiremarkArmorGeneratedThisTurn { get; set; }
    public int FiremarkArmorBreaks { get; set; }
}
