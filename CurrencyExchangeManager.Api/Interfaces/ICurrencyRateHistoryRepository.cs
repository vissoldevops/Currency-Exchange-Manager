using CurrencyExchangeManager.Api.Data;
using CurrencyExchangeManager.Api.Models;

namespace CurrencyExchangeManager.Api.Interfaces;

public interface ICurrencyRateHistoryRepository
{
    Task<ApiActionResult> CreateAsync(CurrencyRateHistory currencyRateHistory);
    Task<ApiActionResult> ReadAsync(DateTime timeStamp);
}
