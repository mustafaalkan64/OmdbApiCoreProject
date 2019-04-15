using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Models;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Services.Interfaces
{
    public interface IMovieService
    {
        Task AddMovie(Movie entity);
        Task<MovieResponse> GetFromOmdb(string title, int? year);
    }
}
