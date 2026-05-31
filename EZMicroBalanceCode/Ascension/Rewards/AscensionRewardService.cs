using EZMicroBalance.EZMicroBalanceCode.Core.Architecture;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionRewardService
{
    public static bool TryModifyCardRewardOptionsLate(
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions)
    {
        RewardPipeline.Diagnose(new RewardPipelineContext
        {
            Feature = "AscensionRewards",
            EventName = "card_options_surface_entered",
            NetMode = MultiplayerFeaturePolicy.DescribeNetMode(player.RunState),
            Data = new Dictionary<string, object?>
            {
                ["optionCount"] = cardRewardOptions.Count
            }
        });

        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopGameplay(
                player.RunState,
                "AscensionRewards",
                "A11-A20 card reward mutations, including Firemarked rewards, Boss rewards, and Fission, are disabled in co-op until reward sync is proven."))
        {
            RewardPipeline.Diagnose(new RewardPipelineContext
            {
                Feature = "AscensionRewards",
                EventName = "card_options_surface_coop_disabled",
                NetMode = MultiplayerFeaturePolicy.DescribeNetMode(player.RunState),
                Data = new Dictionary<string, object?>
                {
                    ["optionCount"] = cardRewardOptions.Count
                }
            });
            return false;
        }

        var modified = false;

        if (AscensionFeatureGate.IsFiremarkedEliteEnabled(player.RunState))
        {
            modified |= TryAddFiremarkedEliteRewardOption(player, cardRewardOptions, creationOptions);
        }

        if (AscensionFeatureGate.IsBossSealsEnabled(player.RunState))
        {
            modified |= TryAddBossSealRewardOption(player, cardRewardOptions, creationOptions);
        }

        if (AscensionFeatureGate.IsFissionEnabled(player.RunState))
        {
            modified |= TryApplyFission(player, cardRewardOptions, creationOptions);
        }

        RewardPipeline.Diagnose(new RewardPipelineContext
        {
            Feature = "AscensionRewards",
            EventName = "card_options_surface_completed",
            NetMode = MultiplayerFeaturePolicy.DescribeNetMode(player.RunState),
            Data = new Dictionary<string, object?>
            {
                ["modified"] = modified,
                ["optionCount"] = cardRewardOptions.Count
            }
        });

        return modified;
    }

    public static bool TryModifyRewardsLate(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        RewardPipeline.Diagnose(new RewardPipelineContext
        {
            Feature = "AscensionRewards",
            EventName = "room_rewards_surface_entered",
            NetMode = MultiplayerFeaturePolicy.DescribeNetMode(player.RunState),
            Data = new Dictionary<string, object?>
            {
                ["rewardCount"] = rewards.Count,
                ["room"] = room?.GetType().Name ?? "none"
            }
        });

        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopGameplay(
                player.RunState,
                "AscensionRewards",
                "A11-A20 room reward mutations are disabled in co-op until reward sync is proven."))
        {
            RewardPipeline.Diagnose(new RewardPipelineContext
            {
                Feature = "AscensionRewards",
                EventName = "room_rewards_surface_coop_disabled",
                NetMode = MultiplayerFeaturePolicy.DescribeNetMode(player.RunState),
                Data = new Dictionary<string, object?>
                {
                    ["rewardCount"] = rewards.Count,
                    ["room"] = room?.GetType().Name ?? "none"
                }
            });
            return false;
        }

        var modified = TryAddA20BossOneCardReward(player, rewards, room) ||
            TryAddDeepBranchTreasureReward(player, rewards, room);

        RewardPipeline.Diagnose(new RewardPipelineContext
        {
            Feature = "AscensionRewards",
            EventName = "room_rewards_surface_completed",
            NetMode = MultiplayerFeaturePolicy.DescribeNetMode(player.RunState),
            Data = new Dictionary<string, object?>
            {
                ["modified"] = modified,
                ["rewardCount"] = rewards.Count,
                ["room"] = room?.GetType().Name ?? "none"
            }
        });

        return modified;
    }

}
