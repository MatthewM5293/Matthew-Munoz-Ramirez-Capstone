using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Echoes_v0._1.Migrations
{
    /// <inheritdoc />
    public partial class NameinPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "PostModel",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "PostModel",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PostModel",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "PostModel");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "PostModel");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PostModel");
        }
    }
}
