using CurrencyExchangeManager.Api.Data;
using CurrencyExchangeManager.Api.Data.DBContext;
using CurrencyExchangeManager.Api.Interfaces;
using CurrencyExchangeManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeManager.Api.Repositories;

public class CurrencyRateHistoryRepository : ICurrencyRateHistoryRepository
{
    #region Properties & Attributes

    private readonly CurrencyExchangeDBContext _context;

    #endregion

    #region Public Methods

    /// <summary>
    /// Constructor with dependency injection
    /// </summary>
    public CurrencyRateHistoryRepository(CurrencyExchangeDBContext context)
    {
        _context = context;
    }

    #region Ceate Methods

    /// <summary>
    /// Save a new CurrencyRateHistory entry into the CurrencyRateHistory database table
    /// and save all the linked CurrencRates into the CurrencyRate database table
    /// </summary>
    /// <param name="currencyRateHistory">The currency rate history data to save</param>
    /// <returns>ApiActionResult</returns>
    public async Task<ApiActionResult> CreateAsync(CurrencyRateHistory currencyRateHistory)
    {
        ApiActionResult result = new();

        try
        {
            await _context.CurrencyRateHistory.AddAsync(currencyRateHistory);
            await _context.SaveChangesAsync();

            result.Status = ResultStatus.Success;
            result.Data = currencyRateHistory;
        }
        catch (Exception ex)
        {
            result.Status = ResultStatus.ServerError;
            result.Message = $"Saving currecy rate history failed!, Error: {ex.Message}";
        }

        return result;
    }

    #endregion

    #region Read Methods

    /// <summary>
    /// Read all the CurrencyRateHistory entries linked to the specified timestamp
    /// </summary>
    /// <param name="timeStamp">The date linked to the currency rate history data to save</param>
    /// <returns>ApiActionResult</returns>
    public async Task<ApiActionResult> ReadAsync(DateTime timeStamp)
    {
        ApiActionResult result = new();

        try
        {
            var currencyRates = await _context.CurrencyRateHistory.Where(p => p.TimeStamp >= timeStamp)
                                                                  .Include(p => p.CurrencyRates).AsNoTracking()
                                                                  .ToListAsync();

            result.Status = ResultStatus.Success;
            result.Data = currencyRates;
        }
        catch (Exception ex)
        {
            result.Status = ResultStatus.ServerError;
            result.Message = $"Reading currecy rate history for timestamp {timeStamp} failed!, Error: {ex.Message}";
        }

        return result;
    }

    #endregion

    #endregion
}
