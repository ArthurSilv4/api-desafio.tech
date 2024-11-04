using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_desafio.tech.Migrations
{
    /// <inheritdoc />
    public partial class Relationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Challenges",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_UserId",
                table: "Challenges",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Users_UserId",
                table: "Challenges",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Users_UserId",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_UserId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Challenges");
        }
    }
}
