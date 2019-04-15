using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Services.Interfaces
{
    public interface IMovieService
    {
        Task AddMovie(Movie entity);
        Task<MovieResponse> GetFromOmdbApi(string title, int? year);
        Task<MovieResponse> GetFromOmdbApiByImdbId(string imdbId);
        Task<Movie> GetFromDb(string title, int? year);
        Task<Movie> GetFromDbByImdbId(string imdbId);
        Task AddRating(Rating rating);
        Task Commit();
        Task UpdateAllMovies();
    }
}
