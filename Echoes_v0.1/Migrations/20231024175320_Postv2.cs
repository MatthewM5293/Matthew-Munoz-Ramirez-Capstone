using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Echoes_v0._1.Migrations
{
    /// <inheritdoc />
    public partial class Postv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "PostModel",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "PostModel");
        }
    }
}
