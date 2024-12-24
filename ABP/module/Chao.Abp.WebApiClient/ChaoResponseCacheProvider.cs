using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using WebApiClientCore;

namespace Chao.Abp.WebApiClient;

public class ChaoResponseCacheProvider(IDistributedCache<ResponseCacheEntry> distributedCache, IOptions<ChaoAbpWebApiClientOption> options) : IResponseCacheProvider
{
    public virtual string Name { get; } = nameof(ChaoResponseCacheProvider);

    public virtual async Task<ResponseCacheResult> GetAsync(string key)
    {
        var result = await distributedCache.GetAsync(key);
        if (result == null)
        {
            return ResponseCacheResult.NoValue;
        }
        else
        {
            return new ResponseCacheResult(result, true);
        }
    }

    public virtual async Task SetAsync(string key, ResponseCacheEntry entry, TimeSpan expiration)
    {
        if (options.Value.HttpStatusCodes.Contains(entry.StatusCode) == true)
        {
            await distributedCache.SetAsync(key, entry, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = expiration
            });
        }
    }
}