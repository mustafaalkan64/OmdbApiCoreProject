using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OmdbApi.Domain.IServices
{
    public interface IMovieService
    {
        Task AddMovie(Movie entity);
        Task<MovieResponse> GetFromOmdbApi(string title, int? year);
        Task<MovieResponse> GetFromOmdbApiByImdbId(string imdbId);
        Task<Movie> GetFromDb(string title, int? year);
        Task AddRating(Rating rating);
        Task Commit();
        Task UpdateAllMovies();
        Task<MovieResponse> SearchMovie(string title, int? year);
    }
}
