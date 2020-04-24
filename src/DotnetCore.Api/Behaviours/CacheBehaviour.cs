using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DotnetCore.Core.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace DotnetCore.Api.Behaviours
{
    public class CacheBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IMemoryCache _cache;
        private readonly IDistributedCache _distributedCache;

        public CacheBehaviour(IMemoryCache cache, IDistributedCache distributedCache)
        {
            _cache = cache;
            _distributedCache = distributedCache;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            if (!(request is ICacheable cacheable))
            {
                return await next();
            }

            if (request is INoCache noCache && noCache.NoCache) return await next();

            switch (cacheable.CacheOption)
            {
                case CacheOption.None:
                    return await next();
                case CacheOption.Memory:
                    return await GetFromMemoryCache(cacheable, next);
                case CacheOption.Distributed:
                    return await GetFromDistributedCache(cacheable, cancellationToken, next);
                case CacheOption.Multi:
                    throw new NotImplementedException("Bunu unutma");
            }

            return await next();
        }

        private async Task<TResponse> GetFromMemoryCache(ICacheable cacheable, RequestHandlerDelegate<TResponse> next)
        {
            var isExist = _cache.TryGetValue(cacheable.CacheSettings.Key, out TResponse response);
            if (isExist) return response;

            response = await next();
            if (response == null) return default;

            _cache.Set(cacheable.CacheSettings.Key, response, cacheable.CacheSettings.Value);

            return response;
        }

        private async Task<TResponse> GetFromDistributedCache(ICacheable cacheable, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var cacheKey = cacheable.CacheSettings.Key;

            var value = await _distributedCache.GetAsync(cacheKey, cancellationToken);
            if (value != null)
            {
                //return JsonSerializer.Deserialize<TResponse>(value);
            }

            var response = await next();
            if (response == null)
            {
                return default;
            }

            var option = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheable.CacheSettings.Value
            };
            await _distributedCache.SetAsync(cacheKey, JsonSerializer.SerializeToUtf8Bytes(response), option, cancellationToken);

            return response;
        }
    }
}