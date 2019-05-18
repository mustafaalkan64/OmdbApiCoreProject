using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmdbApi.DAL.Consts;
using OmdbApi.DAL.EFDbContext;
using OmdbApi.DAL.Models;
using OmdbApi.Domain.IServices;

namespace OmdbApi.Api.Controllers
{
    [Authorize(Roles = RoleType.User)]
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private IMovieService _movieService;
        private IMapper _mapper;
        public MovieController(IMovieService movieService, IMapper mapper)
        {
            _movieService = movieService;
            _mapper = mapper;
        }

        [HttpGet("SearchMovie")]
        public async Task<IActionResult> SearchMovie(string term, int? year)
        {
            try
            {
                var movieResult = await _movieService.SearchMovie(term, year);
                if (movieResult == null)
                    return NotFound();
                return Ok(movieResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("SearchMovieByImdbId")]
        public async Task<IActionResult> SearchMovieByImdbId(string imdbId)
        {
            try
            {
                var movieResult = await _movieService.SearchMovieByImdbId(imdbId);
                if (movieResult == null)
                    return NotFound();
                if (movieResult.Response == false)
                    return BadRequest(movieResult);
                return Ok(movieResult);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
