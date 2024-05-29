using CurrencyExchangeManager.Api.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CurrencyExchangeManager.Api.Services;

public class CacheService : ICacheService
{
    #region Properties & Attribuites

    private readonly IConfiguration _configuration;
    private IDatabase _cacheDB;

    #endregion

    #region Public Methods

    /// <summary>
    /// Constructor with dependency injection
    /// </summary>
    public CacheService(IConfiguration configuration)
    {
        _configuration = configuration;


        var redisConnection = _configuration.GetConnectionString("RedisConnection");
        var redis = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false,connectTimeout=30000,responseTimeout=30000,sslProtocols=tls12");
        _cacheDB = redis.GetDatabase();
    }

    /// <summary>
    /// Set Data with Value and Expiration Time of Key
    /// </summary>
    /// <typeparam name="T">The object type to get</typeparam>
    /// <param name="key">The key identifying the object</param>
    /// <param name="value">The value to set</param>
    /// <param name="expirationTime">The cache exprity time</param>
    /// <returns>Boolean</returns>
    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
        return _cacheDB.StringSet(key, JsonConvert.SerializeObject(value), expiryTime);
    }

    /// <summary>
    /// Get Data using key
    /// </summary>
    /// <typeparam name="T">The object type to get</typeparam>
    /// <param name="key">The key identifying the object</param>
    /// <returns>Object</returns>
    public T GetData<T>(string key)
    {
        var value = _cacheDB.StringGet(key);

        if (!string.IsNullOrEmpty(value))
            return JsonConvert.DeserializeObject<T>(value);

        return default;
    }

    /// <summary>
    /// Remove Data
    /// </summary>
    /// <param name="key">The key identifying the object</param>
    /// <returns>object</returns>
    public object RemoveData(string key)
    {
        bool _keyExist = _cacheDB.KeyExists(key);

        if (_keyExist == true)
            return _cacheDB.KeyDelete(key);
        
        return false;
    }

    #endregion
}