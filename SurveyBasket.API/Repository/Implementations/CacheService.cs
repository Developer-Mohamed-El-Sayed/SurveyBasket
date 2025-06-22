 namespace SurveyBasket.API.Repository.Implementations;

public class CacheService(IDistributedCache distributedCache) : ICacheService
{
    private readonly IDistributedCache _distributedCache = distributedCache;

    public async Task<T?> GetAsync<T>(string cacheKey, CancellationToken cancellationToken = default) where T : class
    {
       var cachedValue = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
        return string.IsNullOrEmpty(cachedValue)
            ? null
            : JsonSerializer.Deserialize<T>(cachedValue);
    }

    public async Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(cacheKey, cancellationToken);
    }

    public async Task SetAsync<T>(string cacheKey, T value, CancellationToken cancellationToken = default) where T : class
    {
        await _distributedCache.SetStringAsync(cacheKey,JsonSerializer.Serialize(value),cancellationToken);
    }
}
