using OmdbApi.DAL.EFDbContext;
using System.Collections;
using System.Collections.Generic;

namespace OmdbApi.DAL.Models
{
    public class MovieResponse: Movie
    {
        public string Error { get; set; }
        public bool Response { get; set; }
    }


    public class MovieCollectionResponse
    {
        public MovieCollectionResponse()
        {
            Search = new List<Movie>();
        }
        public IEnumerable<Movie> Search { get; set; }
        public int TotalResults { get; set; }
        public bool Response { get; set; }
    }

}
