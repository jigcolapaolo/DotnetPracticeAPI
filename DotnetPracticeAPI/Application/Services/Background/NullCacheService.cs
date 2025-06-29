using Application.Interfaces.Services;

namespace Application.Services.Background;

public class NullCacheService : ICacheService
{
    public Task<T?> GetAsync<T>(string key) => Task.FromResult<T?>(default);
    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) => Task.CompletedTask;
    public Task RemoveAsync(string key) => Task.CompletedTask;
}
