using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Content;

/// <summary>
/// Custom Golden Idol relic for the StS1 Golden Idol event.
///
/// StS2 has no native Golden Idol relic, so the event previously granted a random
/// relic. This model lets the Take branch grant the named Golden Idol reward,
/// matching StS1. It is an event-only marker relic: it never appears in relic
/// pools, shops, or Neow, so it can only be obtained through the StS1 Golden Idol
/// event (which itself is gated behind the CanaryOnly StS1 event mode).
///
/// Art status: ART PENDING. Uses the shared generic relic placeholder textures
/// shipped under res://EZMicroBalance/images/relics/. No original game art is
/// copied. A real icon can be dropped at <see cref="Sts1GoldenIdolRelicAssetPaths"/>
/// later without touching this class.
/// </summary>
internal sealed class Sts1GoldenIdolRelic : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Event;

    // Event-only reward: never enter pools, shops, or the Neow pool. The relic
    // is granted exclusively by the StS1 Golden Idol event Take branch.
    public override bool IsAllowed(IRunState runState) => false;

    public override bool IsAllowedAtNeow(Player player) => false;

    public override bool IsAllowedInShops => false;

    public override string CustomIconPath => Sts1GoldenIdolRelicAssetPaths.Icon;

    public override string CustomBigIconPath => Sts1GoldenIdolRelicAssetPaths.BigIcon;

    public override string CustomIconOutlinePath => Sts1GoldenIdolRelicAssetPaths.IconOutline;
}

/// <summary>
/// Asset paths for <see cref="Sts1GoldenIdolRelic"/>.
/// ART PENDING: points at the shared generic relic placeholder textures. Replace
/// these with a dedicated Golden Idol icon when redistributable art is available.
/// </summary>
internal static class Sts1GoldenIdolRelicAssetPaths
{
    public static string Icon => $"{MainFile.ResPath}/images/relics/relic.png";

    public static string IconOutline => $"{MainFile.ResPath}/images/relics/relic_outline.png";

    public static string BigIcon => $"{MainFile.ResPath}/images/relics/big/relic.png";
}
