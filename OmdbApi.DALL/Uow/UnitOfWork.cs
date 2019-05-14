using OmdbApi.DAL.EFDbContext;
using OmdbApi.DAL.Repositories;
using System;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Uow
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly OmdApiDbContext _context;
        private IGenericRepository<Movie> _movieRepository;
        private IGenericRepository<User> _userRepository;
        private IGenericRepository<Rating> _ratingRepository;

        public UnitOfWork(OmdApiDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Movie> MovieRepository
        {
            get { return _movieRepository ?? (_movieRepository = new GenericRepository<Movie>(_context)); }
        }

        public IGenericRepository<User> UserRepository
        {
            get { return _userRepository ?? (_userRepository = new GenericRepository<User>(_context)); }
        }

        public IGenericRepository<Rating> RatingRepository
        {
            get { return _ratingRepository ?? (_ratingRepository = new GenericRepository<Rating>(_context)); }
        }

        public async Task Commit()
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    _context.Dispose();
                    transaction.Rollback();
                }

            }
               
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }
    }
}
