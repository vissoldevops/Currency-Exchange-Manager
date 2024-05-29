using CurrencyExchangeManager.Api.Data;
using CurrencyExchangeManager.Api.Interfaces;
using CurrencyExchangeManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ferdi.Assessment.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyExchangeController : ControllerBase
    {
        #region Properties & Attribuites

        private readonly ILogger<CurrencyExchangeController> _logger;
        private readonly IValidationService _validationService;
        private readonly IExchangeRateService _exchangeRateService;

        #endregion

        #region Public Methods

        public CurrencyExchangeController(ILogger<CurrencyExchangeController> logger,
                                          IValidationService validationService,
                                          IExchangeRateService exchangeRateService)
        {
            _logger = logger;
            _validationService = validationService;
            _exchangeRateService = exchangeRateService;
        }

        /// <summary>
        /// Convert a specified amount from base rate to target currency rate
        /// </summary>
        /// <param name="baseRate">The base currency rate to convert from</param>
        /// <param name="targetRate">The base currency rate to convert from</param>
        /// <param name="amount">The base currency rate to convert from</param>
        /// <response code="200">Successfully enriched the data for a single company</response>
        /// <response code="400">Request failed due to a data validation error</response>
        /// <response code="401">Un-Authorised</response>
        /// <response code="500">Request failed due to a internal server error</response>
        /// <response code="503">Request failed due to a service unavailable error</response>
        /// <returns>IActionResult</returns>
        [HttpGet("convert/{baseRate}/{targetRate}/{amount}")]
        public async Task<IActionResult> Convert(string baseRate, string targetRate, double amount)
        {
            try
            {
                // Validate the requiured conversion data
                ApiActionResult result = _validationService.ValidateConversionData(baseRate, targetRate, amount);

                if (result.Status != ResultStatus.Success)
                    return BadRequest(result);

                await _exchangeRateService.Convert(baseRate, targetRate, amount);

                //switch (result.status)
                //{
                //    case TDDBActionResult.ResultStatus.BadRequest:
                //    case TDDBActionResult.ResultStatus.Failed:
                //        return BadRequest(result);
                //    default:
                return Ok(result.Data);
                //}
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Read all the currency rate history linked to the specified date
        /// </summary>
        /// <param name="date">The date linked to the currency rate history</param>
        /// <response code="200">Successfully enriched the data for a single company</response>
        /// <response code="400">Request failed due to a data validation error</response>
        /// <response code="401">Un-Authorised</response>
        /// <response code="500">Request failed due to a internal server error</response>
        /// <response code="503">Request failed due to a service unavailable error</response>
        /// <returns>IActionResult</returns>
        [HttpGet("history/{date}")]
        public async Task<IActionResult> GetHistory(DateTime date)
        {
            try
            {
                ApiActionResult result = await _exchangeRateService.GetHistoryAsync(date);

                switch (result.Status)
                {
                    case ResultStatus.BadRequest:
                    case ResultStatus.ServerError:
                        return BadRequest(result);
                    default:
                        return Ok(result.Data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}
