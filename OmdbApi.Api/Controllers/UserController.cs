using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Models;
using OmdbApi.DAL.Services.Interfaces;

namespace OmdbApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(string userName, string password)
        {
            try
            {
                var result = await _userService.Authenticate(userName, password);

                if (!result.Status)
                    return BadRequest(new { message = result.Response });

                return Ok(result.Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An Error Occured While Authanticate" });
            }

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserDto userParam)
        {
            try
            {
                var response = await _userService.Register(userParam);

                if (!response.Status)
                    return BadRequest(new { message = response.Response });

                return Ok(response.Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An Error Occured While Register" });
            }

        }
    }
}
