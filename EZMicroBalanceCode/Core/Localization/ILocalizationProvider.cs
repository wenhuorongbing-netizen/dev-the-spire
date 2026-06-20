namespace EZMicroBalance.EZMicroBalanceCode.Core.Localization;

internal interface ILocalizationProvider
{
    List<(string, string)>? Localization { get; }
}
