using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OmdbApi.DAL.Entities;
using System;

namespace OmdbApi.DAL
{
    public class OmdApiDbContext : DbContext
    {
        public OmdApiDbContext(DbContextOptions<OmdApiDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var configuration = new ConfigurationBuilder()
        //       .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        //       .AddJsonFile("appsettings.json")
        //       .Build();
        //    //optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        //    optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        //}
    }
}
