using OmdbApi.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Models
{
    public class MovieResponse: Movie
    {
        public string Error { get; set; }
    }
}
