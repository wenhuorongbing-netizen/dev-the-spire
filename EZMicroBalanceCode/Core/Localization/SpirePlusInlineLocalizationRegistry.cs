using System.Reflection;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Localization;

internal static class SpirePlusInlineLocalizationRegistry
{
    private static readonly FieldInfo? TableNameField = AccessTools.Field(typeof(LocTable), "_name");
    private static readonly Type[] PowerProviderTypes = typeof(SpirePlusInlineLocalizationRegistry).Assembly
        .GetTypes()
        .Where(type => !type.IsAbstract &&
            typeof(PowerModel).IsAssignableFrom(type) &&
            typeof(ILocalizationProvider).IsAssignableFrom(type))
        .ToArray();

    private static readonly Dictionary<(string Table, string Entry), ILocalizationProvider> Providers = new();

    public static void RegisterKnownProviders()
    {
        // RitsuLib can inject dynamic models after Spire Plus startup work, so this
        // registry intentionally retries lookup from LocTable fallbacks instead of
        // freezing a single startup-time snapshot.
        RegisterIfAvailable(ModelDb.Enchantment<JeweledMaskFreePower>);
        RegisterIfAvailable(ModelDb.Enchantment<UrdaTrialBranchEnchantment>);
        RegisterIfAvailable(ModelDb.Enchantment<FissionEnchantment>);
        RegisterIfAvailable(ModelDb.Enchantment<RoyalDecreeEnchantment>);

        RegisterIfAvailable(ModelDb.Relic<ForgeTokenRelic>);

        RegisterPowerProviders();
    }

    public static bool TryGetText(LocTable table, string key, out string text)
    {
        text = string.Empty;

        if (!TryGetTableName(table, out var tableName))
        {
            return false;
        }

        return TryGetText(tableName, key, out text);
    }

    public static bool TryGetTableName(LocTable table, out string tableName)
    {
        if (TableNameField?.GetValue(table) is string value)
        {
            tableName = value;
            return true;
        }

        tableName = string.Empty;
        return false;
    }

    public static bool TryGetText(string table, string key, out string text)
    {
        text = string.Empty;
        RegisterKnownProviders();

        var separator = key.LastIndexOf('.');
        if (separator <= 0 || separator >= key.Length - 1)
        {
            return false;
        }

        var entry = key[..separator];
        var property = key[(separator + 1)..];

        if (!Providers.TryGetValue((table, entry), out var provider))
        {
            return false;
        }

        foreach (var (candidateProperty, value) in provider.Localization ?? [])
        {
            if (candidateProperty.Equals(property, StringComparison.Ordinal))
            {
                text = value;
                return true;
            }
        }

        return false;
    }

    private static void Register(AbstractModel model)
    {
        if (model is not ILocalizationProvider provider)
        {
            return;
        }

        var table = model switch
        {
            PowerModel => "powers",
            RelicModel => "relics",
            EnchantmentModel => "enchantments",
            _ => null
        };

        if (table is not null)
        {
            Providers[(table, model.Id.Entry)] = provider;
        }
    }

    private static void RegisterIfAvailable<T>(Func<T> getModel) where T : AbstractModel
    {
        if (!ModelDb.Contains(typeof(T)))
        {
            return;
        }

        try
        {
            Register(getModel());
        }
        catch (KeyNotFoundException)
        {
            // RitsuLib injects dynamic models after the mod initializer returns.
        }
    }

    private static void RegisterPowerProviders()
    {
        foreach (var type in PowerProviderTypes)
        {
            if (!ModelDb.Contains(type))
            {
                continue;
            }

            try
            {
                Register(ModelDb.DebugPower(type));
            }
            catch (Exception ex) when (ex.GetType().Name == "ModelNotFoundException")
            {
                // The type list can lead the dynamic ModelDb injection by one startup phase.
            }
        }
    }
}
