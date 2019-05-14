using Microsoft.EntityFrameworkCore.Migrations;

namespace OmdbApi.DAL.Migrations
{
    public partial class Movie_Delete_Response : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Response",
                table: "Movies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "Movies",
                nullable: true);
        }
    }
}
