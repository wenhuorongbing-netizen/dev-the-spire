using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private const int EliteRootHeal = 10;

    private static async Task HealAfterEliteVictory(Player player, CombatRoom room)
    {
        if (room.RoomType != RoomType.Elite)
        {
            return;
        }

        if (player.Creature.IsDead)
        {
            return;
        }

        var before = player.Creature.CurrentHp;
        player.Relics.OfType<UrdaEliteRootOptionRelic>().FirstOrDefault()?.Flash();
        await CreatureCmd.Heal(player.Creature, EliteRootHeal);
        var healed = Math.Max(0, player.Creature.CurrentHp - before);
        ReleaseEvidenceLog.Log(
            "UrdaEliteRoot",
            "elite_victory_heal",
            player,
            new Dictionary<string, object?>
            {
                ["roomType"] = room.RoomType.ToString(),
                ["healCap"] = EliteRootHeal,
                ["actualHeal"] = healed,
                ["currentHp"] = player.Creature.CurrentHp,
                ["maxHp"] = player.Creature.MaxHp
            });
        MainFile.Logger.Info(
            $"[Spire Plus] Urda Elite Root applied after Elite combat: healed {healed}/{EliteRootHeal} HP.");
    }
}
