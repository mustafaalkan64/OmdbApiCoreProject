using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OmdbApi.DAL.Consts;
using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Models;
using OmdbApi.DAL.Services.Interfaces;

namespace OmdbApi.Api.Controllers
{
    [Authorize(Roles = RoleType.User)]
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

        [HttpGet("SearchMovie")]
        public async Task<IActionResult> SearchMovie(string title, int? year)
        {
            try
            {
                string key = $"?title={title}&year={year}";
                string obj;
                // Check Cache 
                if (!_cache.TryGetValue(key, out obj))
                {
                    // Set cache options.
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        // Keep in cache for this time, reset time if accessed.
                        .SetSlidingExpiration(TimeSpan.FromMinutes(12));

                    var resultFromDb = await _movieService.GetFromDb(title, year);
                    if (resultFromDb == null)
                    {
                        // Get Movie Data From Omdb Api
                        var result = await _movieService.GetFromOmdbApi(title, year);
                        var response = Convert.ToBoolean(result.Response);
                        if (response)
                        {
                            // Add Movie
                            await _movieService.AddMovie(result);
                            int movieId = result.Id;

                            // Add Ratings That Belong The Movie
                            var ratings = result.Ratings;
                            foreach (var rating in ratings)
                            {
                                rating.MovieId = movieId;
                                await _movieService.AddRating(rating);
                            }
                            await _movieService.Commit();

                            // Set Cache With Object That Comes Omdb Api
                            obj = JsonConvert.SerializeObject(result);
                            _cache.Set(key, obj, cacheEntryOptions);
                            return Ok(result);
                        }
                        return NotFound(result.Response);
                    }
                    else
                    {
                        // Set Cache With Object That Comes From Db
                        obj = JsonConvert.SerializeObject(resultFromDb);
                        _cache.Set(key, obj, cacheEntryOptions);
                        return Ok(resultFromDb);
                    }
                }
                else
                {
                    // Get From Cache With Key
                    string _cachedData = _cache.Get<string>(key);
                    var model = JsonConvert.DeserializeObject<MovieResponse>(_cachedData);
                    return Ok(model);
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception Error Searching Any Movie", title);
                throw e;
            }
        }
    }
}
