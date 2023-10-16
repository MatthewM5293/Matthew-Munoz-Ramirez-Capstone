using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Echoes_v0._1.Migrations
{
    /// <inheritdoc />
    public partial class Comments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PostModelPostId",
                table: "CommentModel",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentModel_PostModelPostId",
                table: "CommentModel",
                column: "PostModelPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentModel_PostModel_PostModelPostId",
                table: "CommentModel",
                column: "PostModelPostId",
                principalTable: "PostModel",
                principalColumn: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentModel_PostModel_PostModelPostId",
                table: "CommentModel");

            migrationBuilder.DropIndex(
                name: "IX_CommentModel_PostModelPostId",
                table: "CommentModel");

            migrationBuilder.DropColumn(
                name: "PostModelPostId",
                table: "CommentModel");
        }
    }
}
