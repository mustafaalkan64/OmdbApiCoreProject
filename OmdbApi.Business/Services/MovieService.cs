using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OmdbApi.DAL.EFDbContext;
using OmdbApi.DAL.Models;
using OmdbApi.DAL.Uow;
using OmdbApi.Domain.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OmdbApi.Business.Services
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
            try
            {
                await _uow.RatingRepository.Add(rating);
                await _uow.Commit();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception Error During Add Rating", rating);
                throw e;
            }
        }

        /// <summary>
        /// // Get Movie Data From Omdb Api With Title or Year
        /// </summary>
        /// <param name="title">Title Param</param>
        /// <param name="year">Year Param</param>
        /// <returns></returns>
        public async Task<MovieCollectionResponse> GetFromOmdbApi(string s, int? year)
        {
            var apikey = _configuration.GetValue<string>("omdbapikey");
            string URL = "http://www.omdbapi.com/";
            string urlParameters = $"?s={s}&apikey={apikey}&type=movie&Content-Type=application/json";
            if(year != null)
                urlParameters += $"&y={year}";

            return await GetMovieCollectionFromWebClient(URL, urlParameters);             
        }

        public async Task<MovieResponse> GetFromOmdbApiByImdbId(string imdbId)
        {
            var apikey = _configuration.GetValue<string>("omdbapikey");
            string URL = "http://www.omdbapi.com/";
            string urlParameters = $"?i={imdbId}&apikey={apikey}&type=movie&Content-Type=application/json";

            return await GetMovieDetailFromWebClient(URL, urlParameters);
        }

        public async Task<IEnumerable<Movie>> GetMoviesFromDb(string term, int? year)
        {
            //|| x.Actors.Contains(term)  
            //|| x.Awards.Contains(term)  
            //|| x.Website.Contains(term)  
            //|| x.Director.Contains(term)
            //|| x.Plot.Contains(term)
            //|| x.Genre.Contains(term) 
            //|| x.Production.Contains(term)

            if(year == null)
                return await _uow.MovieRepository.SearchBy((x => x.Title.Contains(term)));
            
            else
                return await _uow.MovieRepository.SearchBy(x => x.Title.Contains(term) && x.Year.Equals(year.ToString()));
        }


        /// <summary>
        /// Update All Movies In Movies Table According to Omdb Api Resource
        /// </summary>
        /// <returns></returns>
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
                        var _rating = await _uow.RatingRepository.FindBy(a => a.Source.Equals(rating.Source) && a.MovieId.Equals(movie.Id));
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

        /// <summary>
        /// Search Movie By Term and Year params From Omdb Api, After Insert to Movies and Rating Table 
        /// </summary>
        /// <param name="term"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<MovieCollectionResponse> SearchMovie(string term, int? year)
        {
            try
            {
                string key = $"?s={term}";
                if (year != null)
                    key += $"&y={year}";
                string obj;
                // Check Cache 
                if (!_cache.TryGetValue(key, out obj))
                {
                    // Set cache options.
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        // Keep in cache for this time, reset time if accessed.
                        .SetSlidingExpiration(TimeSpan.FromMinutes(20));

                    var resultFromDb = await GetMoviesFromDb(term, year);
                    if (!resultFromDb.Any())
                    {
                        
                        var result = await GetFromOmdbApi(term, year);
                        var response = result.Response;
                        var movieList = new List<Movie>();
                        var movieCollection = new MovieCollectionResponse();
                        if (response)
                        {
                            foreach (var movie in result.Search)
                            {
                                await Task.Run(async () =>
                                {
                                     var _movie = await GetFromOmdbApiByImdbId(movie.imdbID);
                                     await _uow.MovieRepository.Add(_movie);

                                     // Add Ratings That Belong The Movie
                                     var ratings = _movie.Ratings;
                                     foreach (var rating in ratings)
                                     {
                                        rating.ImdbId = _movie.imdbID;
                                        await _uow.RatingRepository.Add(rating);
                                     }
                                    _logger.LogInformation("Movie Create Operation Is Succesfull", movie);
                                    movieList.Add(_movie);

                                });
                                await _uow.Commit();
                            }

                            movieCollection.Response = true;
                            movieCollection.TotalResults = result.TotalResults;
                            movieCollection.Search = movieList;

                            // Set Cache With Object That Comes Omdb Api
                            obj = JsonConvert.SerializeObject(movieCollection);
                            _cache.Set(key, obj, cacheEntryOptions);
                            return movieCollection;
                        }
                        else
                            return result;
                    }
                    else
                    {
                        
                        var movieCollection = new MovieCollectionResponse();
                        var movieList = new List<Movie>();
                        foreach (var movie in resultFromDb)
                        {
                            var movieResponse = _mapper.Map<MovieResponse>(movie);
                            movieResponse.Response = true;
                            movieList.Add(movieResponse);
                        }
                        movieCollection.Search = movieList;
                        movieCollection.Response = true;
                        movieCollection.TotalResults = resultFromDb.ToList().Count;

                        // Set Cache With Object That Comes From Db
                        obj = JsonConvert.SerializeObject(movieCollection);
                        _cache.Set(key, obj, cacheEntryOptions);
                        return movieCollection;
                    }
                }
                else
                {
                    // Get From Cache With Key
                    string _cachedData = _cache.Get<string>(key);
                    var model = JsonConvert.DeserializeObject<MovieCollectionResponse>(_cachedData);
                    return model;
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception Error Searching Any Movie", term);
                throw e;
            }
        }


        /// <summary>
        /// Search Movies By ImdbId
        /// </summary>
        /// <param name="imdbId"></param>
        /// <returns></returns>
        public async Task<MovieResponse> SearchMovieByImdbId(string imdbId)
        {
            try
            {
                if (string.IsNullOrEmpty(imdbId))
                {
                    var movieResponse = new MovieResponse()
                    {
                        Error = "Imdb Id Can not Be Empty",
                        Response = false
                    };

                    return await Task.Run(() => movieResponse).ConfigureAwait(false);
                }

                string key = $"?i={imdbId}";
                string obj;
                // Check Cache 
                if (!_cache.TryGetValue(key, out obj))
                {

                    var _movieFromDb = await _uow.MovieRepository.FindBy(x => x.imdbID.Equals(imdbId), a => a.Ratings);
                    if(_movieFromDb == null)
                    {
                        var _movieFromOmdb = await GetFromOmdbApiByImdbId(imdbId);
                        return _movieFromOmdb;
                    }
                    var movieResponse = _mapper.Map<MovieResponse>(_movieFromDb);
                    movieResponse.Response = true;

                    // Set Cache With Object That Comes From Db
                    // Set cache options.
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromMinutes(20));

                    obj = JsonConvert.SerializeObject(movieResponse);
                    _cache.Set(key, obj, cacheEntryOptions);

                    return movieResponse;
                }
                else
                {
                    string _cachedData = _cache.Get<string>(key);
                    var model = JsonConvert.DeserializeObject<MovieResponse>(_cachedData);
                    return model;
                }
            }
            catch(Exception ex)
            {
                var movieResponse = new MovieResponse()
                {
                    Error = ex.Message,
                    Response = false
                };

                return await Task.Run(() => movieResponse).ConfigureAwait(false);
            }
        }

        private async Task<MovieCollectionResponse> GetMovieCollectionFromWebClient(string URL, string urlParameters)
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
                    var dataObject = await response.Content.ReadAsAsync<MovieCollectionResponse>();  //Make sure to add a reference to System.Net.Http.Formatting.dll
                    return dataObject;
                }
                else
                {
                    //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    return null;
                }
            }
        }

        private async Task<MovieResponse> GetMovieDetailFromWebClient(string URL, string urlParameters)
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
