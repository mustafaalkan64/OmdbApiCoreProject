﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmdbApi.DAL.EFDbContext
{
    public class Rating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Source { get; set; }
        public string Value { get; set; }
        public int MovieId { get; set; }
        public string ImdbId { get; set; }
        //public Movie Movie { get; set; }
    }
}
