using System.Globalization;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal sealed partial class EzmbVakuuTrialEncounter
{
    private const string BrokenLocksKey = "BrokenLocks";
    private const string BloodDebtKey = "BloodDebt";
    private const string DamageRoundKey = "DamageRound";
    private const string DamageThisRoundKey = "DamageThisRound";
    private const string DamageLockRoundKey = "DamageLockRound";
    private const string CashOutOfferedLockKey = "CashOutOfferedLock";
    private const string CashedOutKey = "CashedOut";

    public override Dictionary<string, string> SaveCustomState()
    {
        return new Dictionary<string, string>
        {
            [BrokenLocksKey] = BrokenLocks.ToString(),
            [BloodDebtKey] = BloodDebt.ToString(),
            [DamageRoundKey] = DamageRound.ToString(),
            [DamageThisRoundKey] = DamageThisRound.ToString(CultureInfo.InvariantCulture),
            [DamageLockRoundKey] = DamageLockRound.ToString(),
            [CashOutOfferedLockKey] = CashOutOfferedLock.ToString(),
            [CashedOutKey] = CashedOut ? "1" : "0"
        };
    }

    public override void LoadCustomState(Dictionary<string, string> state)
    {
        BrokenLocks = ReadInt(state, BrokenLocksKey);
        BloodDebt = ReadInt(state, BloodDebtKey);
        DamageRound = ReadInt(state, DamageRoundKey, -1);
        DamageThisRound = ReadDecimal(state, DamageThisRoundKey);
        DamageLockRound = ReadInt(state, DamageLockRoundKey, -1);
        CashOutOfferedLock = ReadInt(state, CashOutOfferedLockKey);
        CashedOut = ReadBool(state, CashedOutKey);
    }

    private static int ReadInt(IReadOnlyDictionary<string, string> state, string key, int fallback = 0) =>
        state.TryGetValue(key, out var value) && int.TryParse(value, out var parsed)
            ? parsed
            : fallback;

    private static decimal ReadDecimal(IReadOnlyDictionary<string, string> state, string key) =>
        state.TryGetValue(key, out var value) &&
        decimal.TryParse(value, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : 0m;

    private static bool ReadBool(IReadOnlyDictionary<string, string> state, string key) =>
        state.TryGetValue(key, out var value) &&
        (value == "1" || bool.TryParse(value, out var parsed) && parsed);
}
