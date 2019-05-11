﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OmdbApi.DAL.EFDbContext;

namespace OmdbApi.DAL.Migrations
{
    [DbContext(typeof(OmdApiDbContext))]
    partial class OmdApiDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("OmdbApi.DAL.EFDbContext.Movie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Actors");

                    b.Property<string>("Awards");

                    b.Property<string>("BoxOffice");

                    b.Property<string>("Country");

                    b.Property<string>("DVD");

                    b.Property<string>("Director");

                    b.Property<string>("Genre");

                    b.Property<string>("Language");

                    b.Property<string>("Metascore");

                    b.Property<string>("Plot");

                    b.Property<string>("Poster");

                    b.Property<string>("Production");

                    b.Property<string>("Rated");

                    b.Property<string>("Released");

                    b.Property<string>("Runtime");

                    b.Property<string>("Title");

                    b.Property<string>("Type");

                    b.Property<string>("Website");

                    b.Property<string>("Writer");

                    b.Property<string>("Year");

                    b.Property<string>("imdbID");

                    b.Property<string>("imdbRating");

                    b.Property<string>("imdbVotes");

                    b.HasKey("Id");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("OmdbApi.DAL.EFDbContext.Rating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ImdbId");

                    b.Property<int>("MovieId");

                    b.Property<string>("Source");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("MovieId");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("OmdbApi.DAL.EFDbContext.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("Hash");

                    b.Property<string>("LastName");

                    b.Property<string>("Salt");

                    b.Property<string>("Token");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("OmdbApi.DAL.EFDbContext.Rating", b =>
                {
                    b.HasOne("OmdbApi.DAL.EFDbContext.Movie")
                        .WithMany("Ratings")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
