namespace CurrencyExchangeManager.Api.Interfaces;

public interface ICacheService
{
    /// <summary>
    /// Set Data with Value and Expiration Time of Key
    /// </summary>
    /// <typeparam name="T">The object type to get</typeparam>
    /// <param name="key">The key identifying the object</param>
    /// <param name="value">The value to set</param>
    /// <param name="expirationTime">The cache exprity time</param>
    /// <returns>Boolean</returns>
    T GetData<T>(string key);

    /// <summary>
    /// Set Data with Value and Expiration Time of Key
    /// </summary>
    /// <typeparam name="T">The object type to get</typeparam>
    /// <param name="key">The key identifying the object</param>
    /// <param name="value">The value to set</param>
    /// <param name="expirationTime">The cache exprity time</param>
    /// <returns>Boolean</returns>
    bool SetData<T>(string key, T value, DateTimeOffset expirationTime);

    /// <summary>
    /// Remove Data
    /// </summary>
    /// <param name="key">The key identifying the object</param>
    /// <returns>object</returns>
    object RemoveData(string key);
}