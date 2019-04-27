using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmdbApi.DAL.Consts;
using OmdbApi.Domain.IServices;

namespace OmdbApi.Api.Controllers
{
    [Authorize(Roles = RoleType.User)]
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
