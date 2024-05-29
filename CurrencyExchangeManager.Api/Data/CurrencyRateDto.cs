using System.Text.Json.Serialization;

namespace CurrencyExchangeManager.Api.Data;

public class CurrencyRateDto
{
    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("value")]
    public double Value { get; set; }
}
