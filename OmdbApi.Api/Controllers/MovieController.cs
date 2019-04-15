using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Services.Interfaces;

namespace OmdbApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private IMovieService _movieService;
        private readonly ILogger<MovieController> _logger;
        private IMemoryCache _cache;
        public MovieController(IMovieService movieService, ILogger<MovieController> logger, IMemoryCache cache)
        {
            _movieService = movieService;
            _logger = logger;
            _cache = cache;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            try
            {
                _logger.LogInformation("Could break here :(");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "It broke :(");
                throw e;
            }

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Movie movie)
        {
            try
            {
                await _movieService.AddMovie(movie);
                _logger.LogInformation("Movie Created Successfuly", movie);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception Error During Add Movie", movie);
                throw e;
            }

        }

        [HttpGet("GetFromOmdbApi")]
        public async Task<IActionResult> GetFromOmdbApi(string title, int? year)
        {
            try
            {
                var result = await _movieService.GetFromOmdb(title, year);
                var response = Convert.ToBoolean(result.Response);
                if (response)
                    return Ok(result);
                else
                    return NotFound(result.Error);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception Error Searching Any Movie", title);
                throw e;
            }
        }
    }
}
