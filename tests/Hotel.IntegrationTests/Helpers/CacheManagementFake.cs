using Hotel.Common;

namespace Hotel.IntegrationTests.Helpers;

public class CacheManagementFake : ICacheManagement
{
  IDictionary<string, object> _cache = new Dictionary<string, object>();
  public async Task<T?> GetAsync<T>(string cacheKey, CancellationToken cancellationToken = default) where T : class
  {
    if (_cache.ContainsKey(cacheKey))
      return await Task.FromResult<T?>((T)_cache[cacheKey]);

    return null;
  }

  public Task RemoveAsync(string key, CancellationToken token = default)
  {
    _cache.Clear();
    return Task.CompletedTask;
  }

  public async Task SetAsync<T>(string cacheKey, T value, CancellationToken token = default) where T : class
  {
    if (_cache.ContainsKey(cacheKey))
      _cache[cacheKey] = value;
    else
      _cache.Add(cacheKey, value);
    await Task.CompletedTask;
  }
}
