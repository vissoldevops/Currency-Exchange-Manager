using CurrencyExchangeManager.Api.Data;
using CurrencyExchangeManager.Api.Interfaces;
using CurrencyExchangeManager.Api.Models;
using OpenExchangeRates;

namespace CurrencyExchangeManager.Api.Services;

public class ExchangeRateService : IExchangeRateService
{
    #region Properties & Attribuites
    
    private readonly ILogger<ExchangeRateService> _logger;
    private readonly IConfiguration _configuration;
    private readonly ICurrencyRateHistoryRepository _currencyRateHistoryRepository;
    private readonly ICacheService _cacheService;

    #endregion

    #region Public Methods

    /// <summary>
    /// Constructor with dependency injection
    /// </summary>
    /// <param name="configuration">The injected configuration service</param>
    public ExchangeRateService(ILogger<ExchangeRateService> logger,
                               IConfiguration configuration,
                               ICurrencyRateHistoryRepository currencyRateHistoryRepository,
                               ICacheService cacheService)
    {
        _logger = logger;
        _configuration = configuration;
        _currencyRateHistoryRepository = currencyRateHistoryRepository;
        _cacheService = cacheService;

        // Get the latest exchange rates
        GetLatestAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get the latest exchange rates from the ExchangeRate API and then
    /// 1. Update the CurrencyRateHistory and CurrencyRate database tables
    /// 2. Save the exchange rates to Redis cache
    /// </summary>
    /// <returns>Task</returns>
    public async Task GetLatestAsync(CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["ApplicationSettings:ExchangeRateApiKey"];

        try
        {
            var cachedRates = _cacheService.GetData<IEnumerable<CurrencyRateDto>>("currencyRates");

            // Only get the rates if cache is empty
            if (cachedRates is null || cachedRates.Count() == 0)
            {
                List<CurrencyRateDto> currencyRatesToCache = new();
                var expiryTime = DateTimeOffset.Now.AddMinutes(15);

                // Make call to the exchange rate API
                var client = new OpenExchangeRatesClient(apiKey);
                var apiResponse = await client.GetLatestRatesAsync();

                foreach (var rate in apiResponse.Rates)
                {
                    currencyRatesToCache.Add(new CurrencyRateDto
                    {
                        Key = $"currencyRate-{rate.Key}",
                        Value = (double)rate.Value
                    });
                }

                // Save currency rates to the history table in the database
                await _saveToDatabaseAsync(apiResponse);

                // Save the currency rates to cache
                _cacheService.SetData<IEnumerable<CurrencyRateDto>>("currencyRates", currencyRatesToCache, expiryTime);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Reading latest exchange rates failed!");
        }
    }

    /// <summary>
    /// Read all the exchange rates linked to the specified date 
    /// from the CurrencyRateHistory database table
    /// </summary>
    /// <param name="date">The date linked to the exchange rates</param>
    /// <returns>ApiActionResult</returns>
    public async Task<ApiActionResult> GetHistoryAsync(DateTime date)
    {
        return await _currencyRateHistoryRepository.ReadAsync(date);
    }

    /// <summary>
    /// Convert the specified value from base currency to target rate
    /// </summary>
    /// <param name="baseRate">The base rate</param>
    /// <param name="targetRate">The target rate</param>
    /// <param name="amount">The value to convert</param>
    /// <returns>ApiActionResult</returns>
    public async Task<ApiActionResult> Convert(string baseRate, string targetRate, double amount)
    {
        ApiActionResult result = new();

        try
        {
            var apiKey = _configuration["ApplicationSettings:ExchangeRateApiKey"];
            var expiryTime = DateTimeOffset.Now.AddMinutes(15);

            var currencyRate = _cacheService.GetData<CurrencyRateDto>($"currencyRate-{targetRate}");

            if (currencyRate == null)
            {
                // Make call to the exchange rate API
                await GetLatestAsync();
            }

            currencyRate = _cacheService.GetData<CurrencyRateDto>($"currencyRate-{targetRate}");

            if (currencyRate == null)
            {
                result.Status = ResultStatus.BadRequest;
                result.Message = $"Currency rate {targetRate} not found!";
            }
 
            result.Data = amount / currencyRate.Value;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Currency rate conversion failed!");
            result.Status = ResultStatus.ServerError;
            result.Message = $"Currency rate conversion error! {ex.Message}";
            return result;
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Update the CurrencyRateHistory and CurrencyRate database tables
    /// </summary>
    /// <param name="exchangeRates">The exchange rates to save</param>
    /// <returns>Task</returns>
    private async Task _saveToDatabaseAsync(RatesResponse exchangeRates)
    {
        // Create a new currency rate history object
        CurrencyRateHistory currencyRateHistory = new();
        currencyRateHistory.TimeStamp = exchangeRates.Timestamp.DateTime;
        currencyRateHistory.BaseCurrency = exchangeRates.BaseCurrency;
        currencyRateHistory.CreatedBy = "System";
        currencyRateHistory.CreatedAt = DateTime.Now;
        currencyRateHistory.CurrencyRates = new List<CurrencyRate>();

        foreach (var rate in exchangeRates.Rates)
        {
            currencyRateHistory.CurrencyRates.Add(new CurrencyRate
            {
                CurrencyCode = rate.Key,
                ExchangeRate = (double)rate.Value,
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow
            });
        }

        await _currencyRateHistoryRepository.CreateAsync(currencyRateHistory);
    }

    #endregion
}
