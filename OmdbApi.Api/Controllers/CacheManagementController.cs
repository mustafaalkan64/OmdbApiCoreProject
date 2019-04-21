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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CacheManagementController : ControllerBase
    {
        private readonly ICacheManagementService _cacheManagementService;

        public CacheManagementController(ICacheManagementService cacheManagementService)
        {
            _cacheManagementService = cacheManagementService;
        }

        [HttpGet("Clear")]
        public async Task<IActionResult> Clear()
        {
            try
            {
                await _cacheManagementService.Clear();
                return Ok();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
