using OmdbApi.DAL.EFDbContext;

namespace OmdbApi.DAL.Models
{
    public class MovieResponse: Movie
    {
        public string Error { get; set; }
        public string Response { get; set; }
    }
}
