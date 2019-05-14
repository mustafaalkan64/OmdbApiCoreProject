using Microsoft.EntityFrameworkCore.Migrations;

namespace OmdbApi.DAL.Migrations
{
    public partial class RatingsImdbId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImdbId",
                table: "Ratings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImdbId",
                table: "Ratings");
        }
    }
}
