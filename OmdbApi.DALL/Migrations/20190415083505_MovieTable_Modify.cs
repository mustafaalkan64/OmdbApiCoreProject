using Microsoft.EntityFrameworkCore.Migrations;

namespace OmdbApi.DAL.Migrations
{
    public partial class MovieTable_Modify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Movies_MovieId",
                table: "Rating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rating",
                table: "Rating");

            migrationBuilder.DropColumn(
                name: "MovieName",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Movies");

            migrationBuilder.RenameTable(
                name: "Rating",
                newName: "Ratings");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_MovieId",
                table: "Ratings",
                newName: "IX_Ratings_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Movies_MovieId",
                table: "Ratings",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Movies_MovieId",
                table: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings");

            migrationBuilder.RenameTable(
                name: "Ratings",
                newName: "Rating");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_MovieId",
                table: "Rating",
                newName: "IX_Rating_MovieId");

            migrationBuilder.AddColumn<string>(
                name: "MovieName",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rating",
                table: "Rating",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Movies_MovieId",
                table: "Rating",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
