using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Services.Interfaces;

namespace OmdbApi.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(string userName, string password)
        {
            try
            {
                var user = await _userService.Authenticate(userName, password);

                if (user == null)
                    return BadRequest(new { message = "Username or password is incorrect" });

                return Ok(user.Token);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An Error Occured While Authanticate" });
            }

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]User userParam)
        {
            try
            {
                var token = await _userService.Register(userParam);

                if (string.IsNullOrEmpty(token))
                    return BadRequest(new { message = "Register Is Failed" });

                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An Error Occured While Register" });
            }

        }
    }
}
