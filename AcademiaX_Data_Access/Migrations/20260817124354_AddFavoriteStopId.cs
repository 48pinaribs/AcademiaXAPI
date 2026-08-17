using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademiaX_Data_Access.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteStopId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FavoriteStopId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FavoriteStopId",
                table: "AspNetUsers");
        }
    }
}
