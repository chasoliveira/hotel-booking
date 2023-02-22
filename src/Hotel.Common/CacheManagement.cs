using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Hotel.Common;

public interface ICacheManagement
{
  Task<T?> GetAsync<T>(string cacheKey, CancellationToken token = default) where T : class;
  Task SetAsync<T>(string cacheKey, T value, CancellationToken token = default) where T : class;
  Task RemoveAsync(string key, CancellationToken token = default);
}

public class CacheManagement : ICacheManagement
{
  private readonly IDistributedCache distributedCache;

  public CacheManagement(IDistributedCache distributedCache)
  {
    this.distributedCache = distributedCache;
  }

  public async Task<T?> GetAsync<T>(string cacheKey, CancellationToken token = default) where T : class
  {
    var cachedValue = await distributedCache.GetStringAsync(cacheKey, token);
    if (!string.IsNullOrEmpty(cachedValue))
      return JsonSerializer.Deserialize<T>(cachedValue) ?? Activator.CreateInstance<T>();

    return null;
  }

  public async Task SetAsync<T>(string cacheKey, T value, CancellationToken token = default) where T : class
  {
    var serializedValue = JsonSerializer.Serialize(value);
    var valueInBytes = Encoding.UTF8.GetBytes(serializedValue);
    await distributedCache.SetAsync(cacheKey, valueInBytes, token);
  }

  public Task RemoveAsync(string key, CancellationToken token = default)
    => distributedCache.RemoveAsync(key, token);
}