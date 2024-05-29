namespace CurrencyExchangeManager.Api.Data;

public enum ResultStatus
{
    Success = 200,
    BadRequest = 400,
    ServerError = 500,
}

public class ApiActionResult
{
    public ApiActionResult()
    {
        Status = ResultStatus.Success;
        Message = string.Empty;
        Data = null;
    }

    public ResultStatus Status { get; set; }
    public string Message { get; set; }
    public object? Data { get; set; }
}

