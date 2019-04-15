using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Models;
using OmdbApi.DAL.Services.Interfaces;

namespace OmdbApi.Api.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CacheManagementController : ControllerBase
    {
        private IMemoryCache _cache;
        private readonly ILogger<CacheManagementController> _logger;
        public CacheManagementController( IMemoryCache cache, ILogger<CacheManagementController> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        [HttpGet("Clear")]
        public async Task<IActionResult> Clear()
        {
            try
            {
                PropertyInfo prop = _cache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
                object innerCache = prop.GetValue(_cache);
                MethodInfo clearMethod = innerCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
                clearMethod.Invoke(innerCache, null);
                _logger.LogInformation("All Keys Removed From Cache");
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception Error During Clear Cache");
                throw e;
            }
        }
    }
}
