using Microsoft.Extensions.Configuration;
using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Models;
using OmdbApi.DAL.Services.Interfaces;
using OmdbApi.DAL.Uow;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Services
{
    public class MovieService: IMovieService
    {
        private readonly IUnitOfWork _uow;
        private IConfiguration _configuration;

        public MovieService(IUnitOfWork unit, IConfiguration configuration)
        {
            _uow = unit;
            _configuration = configuration;
        }

        public async Task AddMovie(Movie entity)
        {
            await _uow.MovieRepository.Add(entity);
            await _uow.Commit();
        }

        public async Task AddRating(Rating rating)
        {
            await _uow.RatingRepository.Add(rating);
        }

        public async Task Commit()
        {
            await _uow.Commit();
        }

        public async Task<MovieResponse> GetFromOmdbApi(string title, int? year)
        {
            var apikey = _configuration.GetValue<string>("omdbapikey");
            string URL = "http://www.omdbapi.com/";
            string urlParameters = $"?t={title}&plot=full&apikey={apikey}";
            if(year != null)
                urlParameters += $"&y={year}";

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

        public async Task<Movie> GetFromDb(string title, int? year)
        {
            return await _uow.MovieRepository.FindBy((x => x.Title.Contains(title) || x.Year == year.ToString()), a => a.Ratings);
        }
    }
}
