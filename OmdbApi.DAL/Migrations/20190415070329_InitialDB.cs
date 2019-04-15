using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OmdbApi.DAL.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Actors",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Awards",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BoxOffice",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DVD",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Director",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metascore",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Plot",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Poster",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Production",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rated",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Released",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Runtime",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Writer",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "imdbID",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "imdbRating",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "imdbVotes",
                table: "Movies",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Source = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    MovieId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rating_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rating_MovieId",
                table: "Rating",
                column: "MovieId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropColumn(
                name: "Actors",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Awards",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "BoxOffice",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "DVD",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Director",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Genre",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Metascore",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Plot",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Poster",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Production",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Rated",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Released",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Response",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Runtime",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Writer",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "imdbID",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "imdbRating",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "imdbVotes",
                table: "Movies");
        }
    }
}
