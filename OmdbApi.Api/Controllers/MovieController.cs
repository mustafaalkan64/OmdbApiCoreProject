using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmdbApi.Business.Consts;
using OmdbApi.DAL.EFDbContext;
using OmdbApi.Domain.IServices;

namespace OmdbApi.Api.Controllers
{
    [Authorize(Roles = RoleType.User)]
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private IMovieService _movieService;
        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Movie movie)
        {
            try
            {
                await _movieService.AddMovie(movie);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("SearchMovie")]
        public async Task<IActionResult> SearchMovie(string title, int? year)
        {
            try
            {
                var movieResult = await _movieService.SearchMovie(title, year);
                if (movieResult == null)
                    return NotFound();
                return Ok(movieResult);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
