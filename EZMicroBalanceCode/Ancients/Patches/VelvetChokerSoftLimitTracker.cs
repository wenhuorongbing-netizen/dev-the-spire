namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static class VelvetChokerSoftLimitTracker
{
    public const int FreeHandPlaysPerTurn = 6;

    public const int ExtraEnergyCost = 1;

    private sealed class State
    {
        public int HandPlayedThisTurn { get; set; }
    }

    private static readonly ConditionalWeakTable<VelvetChoker, State> States = new();

    private static readonly HashSet<CardModel> SuppressedCostCards = [];

    private static readonly System.Reflection.FieldInfo EnergyCostCardField =
        AccessTools.Field(typeof(CardEnergyCost), "_card");

    private static readonly System.Reflection.MethodInfo InvokeDisplayAmountChangedMethod =
        AccessTools.Method(typeof(RelicModel), "InvokeDisplayAmountChanged");

    public static CardModel? GetCard(CardEnergyCost energyCost)
    {
        return EnergyCostCardField.GetValue(energyCost) as CardModel;
    }

    public static int HandPlayedThisTurn(VelvetChoker choker)
    {
        return States.GetOrCreateValue(choker).HandPlayedThisTurn;
    }

    public static bool ShouldTax(CardModel card)
    {
        if (!CombatManager.Instance.IsInProgress ||
            SuppressedCostCards.Contains(card) ||
            card.IsClone ||
            card.Pile?.Type != PileType.Hand)
        {
            return false;
        }

        var owner = TryGetOwner(card);
        var choker = owner?.GetRelic<VelvetChoker>();
        return choker is { IsMelted: false } &&
            HandPlayedThisTurn(choker) >= FreeHandPlaysPerTurn;
    }

    private static Player? TryGetOwner(CardModel card)
    {
        try
        {
            return card.Owner;
        }
        catch (MegaCrit.Sts2.Core.Models.Exceptions.CanonicalModelException)
        {
            return null;
        }
    }

    public static void Increment(VelvetChoker choker)
    {
        States.GetOrCreateValue(choker).HandPlayedThisTurn++;
        InvokeDisplayAmountChanged(choker);
    }

    public static void Reset(VelvetChoker choker)
    {
        States.GetOrCreateValue(choker).HandPlayedThisTurn = 0;
        InvokeDisplayAmountChanged(choker);
    }

    public static T SuppressCostFor<T>(CardModel card, Func<T> action)
    {
        SuppressedCostCards.Add(card);
        try
        {
            return action();
        }
        finally
        {
            SuppressedCostCards.Remove(card);
        }
    }

    public static async Task<T> SuppressCostFor<T>(CardModel card, Func<Task<T>> action)
    {
        SuppressedCostCards.Add(card);
        try
        {
            return await action();
        }
        finally
        {
            SuppressedCostCards.Remove(card);
        }
    }

    private static void InvokeDisplayAmountChanged(VelvetChoker choker)
    {
        InvokeDisplayAmountChangedMethod.Invoke(choker, Array.Empty<object>());
    }
}
