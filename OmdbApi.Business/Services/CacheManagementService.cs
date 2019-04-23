using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OmdbApi.Domain.IServices;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace OmdbApi.Business.Services
{
    public class CacheManagementService : ICacheManagementService
    {
        private IMemoryCache _cache;
        private readonly ILogger<CacheManagementService> _logger;
        public CacheManagementService(IMemoryCache cache, ILogger<CacheManagementService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<bool> Clear()
        {
            try
            {
                PropertyInfo prop = _cache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
                object innerCache = prop.GetValue(_cache);
                MethodInfo clearMethod = innerCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
                clearMethod.Invoke(innerCache, null);
                _logger.LogInformation("All Keys Removed From Cache");
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Error During Clear Cache");
                throw ex;
            }

        }
    }
}
