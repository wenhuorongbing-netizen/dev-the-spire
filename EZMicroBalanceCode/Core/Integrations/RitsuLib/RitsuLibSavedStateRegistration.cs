using System.Reflection;
using System.Runtime.CompilerServices;
using STS2RitsuLib.Utils;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

internal static class RitsuLibSavedStateRegistration
{
    public static string[] EnsureRegistered(Type ownerType)
    {
        // SavedAttachedState registers through static field construction, so
        // initialization must happen before RitsuLib closes saved properties.
        RuntimeHelpers.RunClassConstructor(ownerType.TypeHandle);

        var savedStateFields = ownerType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => IsSavedAttachedState(field.FieldType))
            .OrderBy(field => field.Name, StringComparer.Ordinal)
            .ToArray();

        if (savedStateFields.Length == 0)
        {
            throw new InvalidOperationException($"{ownerType.FullName} does not declare any RitsuLib SavedAttachedState fields.");
        }

        foreach (var field in savedStateFields)
        {
            if (field.GetValue(null) is null)
            {
                throw new InvalidOperationException($"{ownerType.FullName}.{field.Name} did not initialize its SavedAttachedState.");
            }
        }

        return savedStateFields.Select(field => field.Name).ToArray();
    }

    private static bool IsSavedAttachedState(Type fieldType) =>
        fieldType.IsGenericType &&
        fieldType.GetGenericTypeDefinition() == typeof(SavedAttachedState<,>);
}
