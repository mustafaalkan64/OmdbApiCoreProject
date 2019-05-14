using Newtonsoft.Json;
using OmdbApi.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OmdbApi.DAL.EFDbContext
{
    public class Movie: MovieDto
    {
        public Movie()
        {
            Ratings = new List<Rating>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    } 
}
