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
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Challenges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Challenges",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_UserId1",
                table: "Challenges",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Users_UserId1",
                table: "Challenges",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Users_UserId1",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_UserId1",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Challenges");
        }
    }
}
