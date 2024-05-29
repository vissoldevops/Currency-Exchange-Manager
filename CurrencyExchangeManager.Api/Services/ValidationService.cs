using CurrencyExchangeManager.Api.Data;
using CurrencyExchangeManager.Api.Interfaces;

namespace CurrencyExchangeManager.Api.Services;

public class ValidationService : IValidationService
{
    #region Public Methods

    /// <summary>
    /// Validate all the required data to process the consion request
    /// </summary>
    /// <param name="baseRate">The base currency rate to convert from</param>
    /// <param name="targetRate">The base currency rate to convert from</param>
    /// <param name="amount">The base currency rate to convert from</param>
    /// <returns>ApiActionResult</returns>
    public ApiActionResult ValidateConversionData(string baseRate, string targetRate, double amount)
    {
        ApiActionResult result = new();

        // Validate the required base rate
        if (string.IsNullOrEmpty(baseRate))
        {
            result.Status = ResultStatus.BadRequest;
            result.Message = "The base rate is required!";
            return result;
        }

        // Validate the required target rate
        if (string.IsNullOrEmpty(targetRate))
        {
            result.Status = ResultStatus.BadRequest;
            result.Message = "The target rate is required!";
            return result;
        }

        // Validate the required amount to convert
        if (amount < 0)
        {
            result.Status = ResultStatus.BadRequest;
            result.Message = "The amount to convert is required!";
            return result;
        }

        return result;
    }

    #endregion
}
