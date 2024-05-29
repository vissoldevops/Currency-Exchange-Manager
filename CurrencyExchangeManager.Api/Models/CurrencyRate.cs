using System.ComponentModel.DataAnnotations;

namespace CurrencyExchangeManager.Api.Models;

public class CurrencyRate : BaseModel
{   
    [Required]
    public string CurrencyCode { get; set; }

    [Required]
    public double ExchangeRate { get; set; }
}
