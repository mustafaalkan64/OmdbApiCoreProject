﻿using OmdbApi.DAL.EFDbContext;
using OmdbApi.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OmdbApi.Domain.IServices
{
    public interface IMovieService
    {
        Task AddMovie(Movie entity);
        Task<MovieCollectionResponse> GetFromOmdbApi(string title, int? year);
        Task<MovieResponse> GetFromOmdbApiByImdbId(string imdbId);
        Task AddRating(Rating rating);
        Task UpdateAllMovies();
        Task<MovieCollectionResponse> SearchMovie(string title, int? year);
        Task<IEnumerable<Movie>> GetMoviesFromDb(string title, int? year);
        Task<MovieResponse> SearchMovieByImdbId(string imdbId);
    }
}
