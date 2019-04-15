using OmdbApi.DAL.Entities;
using OmdbApi.DAL.Repositories;
using System;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Uow
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Movie> MovieRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<Rating> RatingRepository { get;}
        Task Commit();
    }
}
