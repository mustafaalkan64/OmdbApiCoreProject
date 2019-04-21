using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Models;
using OmdbApi.DAL.Services.Interfaces;
using OmdbApi.DAL.Uow;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Services
{
    public class MovieService: IMovieService
    {
        private readonly IUnitOfWork _uow;
        private IConfiguration _configuration;
        private IMemoryCache _cache;
        private readonly ILogger<MovieService> _logger;
        private readonly IMapper _mapper;

        public MovieService(IUnitOfWork unit, IConfiguration configuration, IMemoryCache cache, ILogger<MovieService> logger, IMapper mapper)
        {
            _uow = unit;
            _configuration = configuration;
            _cache = cache;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// // Create Movie
        /// </summary>
        /// <param name="movie">Movie Model Parameter</param>
        /// <returns></returns>
        public async Task AddMovie(Movie movie)
        {
            try
            {
                await _uow.MovieRepository.Add(movie);
                await _uow.Commit();
                _logger.LogInformation("Movie Created Successfuly", movie);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception Error During Add Movie", movie);
                throw e;
            }

        }

        public async Task AddRating(Rating rating)
        {
            await _uow.RatingRepository.Add(rating);
        }

        public async Task Commit()
        {
            await _uow.Commit();
        }

        /// <summary>
        /// // Get Movie Data From Omdb Api With Title or Year
        /// </summary>
        /// <param name="title">Title Param</param>
        /// <param name="year">Year Param</param>
        /// <returns></returns>
        public async Task<MovieResponse> GetFromOmdbApi(string title, int? year)
        {
            var apikey = _configuration.GetValue<string>("omdbapikey");
            string URL = "http://www.omdbapi.com/";
            string urlParameters = $"?t={title}&plot=full&apikey={apikey}";
            if(year != null)
                urlParameters += $"&y={year}";

            return await GetMovieFromWebClient(URL, urlParameters);             
        }

        public async Task<MovieResponse> GetFromOmdbApiByImdbId(string imdbId)
        {
            var apikey = _configuration.GetValue<string>("omdbapikey");
            string URL = "http://www.omdbapi.com/";
            string urlParameters = $"?i={imdbId}&plot=full&apikey={apikey}";

            return await GetMovieFromWebClient(URL, urlParameters);
        }

        /// <summary>
        /// // Get Movie Data From Db With Title or Year
        /// </summary>
        /// <param name="title"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<Movie> GetFromDb(string title, int? year)
        {
            return await _uow.MovieRepository.FindBy((x => x.Title.Contains(title) || x.Year == year.ToString()), a => a.Ratings);
        }

        public async Task UpdateAllMovies()
        {
            try
            {
                var movies = await _uow.MovieRepository.GetAll();
                foreach (var movie in movies)
                {
                    var movieResultFromOmdb = await GetFromOmdbApiByImdbId(movie.imdbID);
                    movieResultFromOmdb.Id = movie.Id;
                    
                    // Update Movie Up to Omdb Api
                    await _uow.MovieRepository.Update(movieResultFromOmdb);

                    var ratings = movieResultFromOmdb.Ratings;
                    foreach (var rating in ratings)
                    {
                        var _rating = await _uow.RatingRepository.FindBy(a => a.Source == rating.Source && a.MovieId == movie.Id);
                        if(_rating != null)
                        {
                            // Auto Mapper Will be Implemented to Map Operations..
                            _rating.Source = rating.Source;
                            _rating.Value = rating.Value;
                            
                            // Update Rating..
                            await _uow.RatingRepository.Update(rating);
                        }
                        else
                        {
                            // Add New Rating
                            rating.MovieId = movie.Id;
                            await _uow.RatingRepository.Add(rating);
                        }

                    }
                }
                await _uow.Commit();
            }
            catch (Exception ex) { throw ex; };
            
        }

        public async Task<MovieResponse> SearchMovie(string title, int? year)
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

                    var resultFromDb = await GetFromDb(title, year);
                    if (resultFromDb == null)
                    {
                        
                        var movie = await GetFromOmdbApi(title, year);
                        var response = Convert.ToBoolean(movie.Response);
                        if (response)
                        {
                            await AddMovie(movie);
                            int movieId = movie.Id;

                            // Add Ratings That Belong The Movie
                            var ratings = movie.Ratings;
                            foreach (var rating in ratings)
                            {
                                rating.MovieId = movieId;
                                await AddRating(rating);
                            }
                            await Commit();
                            _logger.LogInformation("Movie Create Operation Is Succesfull", movie);
                            
                            // Set Cache With Object That Comes Omdb Api
                            obj = JsonConvert.SerializeObject(movie);
                            _cache.Set(key, obj, cacheEntryOptions);
                            return movie;
                        }
                        return null;
                    }
                    else
                    {
                        // Set Cache With Object That Comes From Db
                        obj = JsonConvert.SerializeObject(resultFromDb);
                        _cache.Set(key, obj, cacheEntryOptions);
                        var movieResponse = _mapper.Map<MovieResponse>(resultFromDb);
                        return movieResponse;
                    }
                }
                else
                {
                    // Get From Cache With Key
                    string _cachedData = _cache.Get<string>(key);
                    var model = JsonConvert.DeserializeObject<MovieResponse>(_cachedData);
                    return model;
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception Error Searching Any Movie", title);
                throw e;
            }
        }

        private async Task<MovieResponse> GetMovieFromWebClient(string URL, string urlParameters)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                // List data response.
                HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body.
                    var dataObject = await response.Content.ReadAsAsync<MovieResponse>();  //Make sure to add a reference to System.Net.Http.Formatting.dll
                    return dataObject;
                }
                else
                {
                    //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    return null;
                }
            }
        }
    }
}
