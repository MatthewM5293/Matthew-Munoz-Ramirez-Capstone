using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Echoes_v0._1.Migrations
{
    /// <inheritdoc />
    public partial class UName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Uname",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uname",
                table: "AspNetUsers");
        }
    }
}
