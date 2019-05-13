using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmdbApi.DAL.Consts;
using OmdbApi.DAL.Models;
using OmdbApi.Domain.IServices;

namespace OmdbApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]UserLoginDto user)
        {
            try
            {
                var result = await _userService.Authenticate(user);

                if (!result.Status)
                    return BadRequest(new { message = result.Response });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
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

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = RoleType.User)]
        [HttpPost("changepasssword")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordModel changePasswordModel)
        {
            try
            {
                changePasswordModel.UserId = GetUserId();
                var response = await _userService.ChangePassword(changePasswordModel);
                if (response == null)
                    return NotFound("User Not Found");

                if (!response.Status)
                    return BadRequest(new { message = response.Response });

                return Ok(response.Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
    }
}
