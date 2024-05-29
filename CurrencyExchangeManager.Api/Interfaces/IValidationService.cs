using CurrencyExchangeManager.Api.Data;

namespace CurrencyExchangeManager.Api.Interfaces;
public interface IValidationService
{
    ApiActionResult ValidateConversionData(string baseRate, string targetRate, double amount);
}

