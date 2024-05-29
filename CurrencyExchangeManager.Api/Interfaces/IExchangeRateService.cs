using CurrencyExchangeManager.Api.Data;

namespace CurrencyExchangeManager.Api.Interfaces;
public interface IExchangeRateService
{
    Task GetLatestAsync(CancellationToken cancellationToken = default);
    Task<ApiActionResult> GetHistoryAsync(DateTime date);
    Task<ApiActionResult> Convert(string baseRate, string targetRate, double amount);
}

