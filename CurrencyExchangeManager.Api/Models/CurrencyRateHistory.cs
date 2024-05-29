using System.ComponentModel.DataAnnotations;

namespace CurrencyExchangeManager.Api.Models;

public class CurrencyRateHistory : BaseModel
{
    [Required]
    public DateTime TimeStamp { get; set; }

    [Required]
    public string BaseCurrency { get; set; }

    public virtual ICollection<CurrencyRate> CurrencyRates { get; set; }
}
