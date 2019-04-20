using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OmdbApi.DAL.Consts;
using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Helpers;
using OmdbApi.DAL.Services.Interfaces;
using OmdbApi.DAL.Uow;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Services
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
