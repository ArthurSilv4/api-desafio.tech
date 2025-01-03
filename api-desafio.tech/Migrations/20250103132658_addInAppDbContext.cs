using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_desafio.tech.Migrations
{
    /// <inheritdoc />
    public partial class addInAppDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChallengeDay_Challenges_ChallengeId",
                table: "ChallengeDay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChallengeDay",
                table: "ChallengeDay");

            migrationBuilder.RenameTable(
                name: "ChallengeDay",
                newName: "ChallengeDays");

            migrationBuilder.RenameIndex(
                name: "IX_ChallengeDay_ChallengeId",
                table: "ChallengeDays",
                newName: "IX_ChallengeDays_ChallengeId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChallengeId",
                table: "ChallengeDays",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChallengeDays",
                table: "ChallengeDays",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeDays_Challenges_ChallengeId",
                table: "ChallengeDays",
                column: "ChallengeId",
                principalTable: "Challenges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChallengeDays_Challenges_ChallengeId",
                table: "ChallengeDays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChallengeDays",
                table: "ChallengeDays");

            migrationBuilder.RenameTable(
                name: "ChallengeDays",
                newName: "ChallengeDay");

            migrationBuilder.RenameIndex(
                name: "IX_ChallengeDays_ChallengeId",
                table: "ChallengeDay",
                newName: "IX_ChallengeDay_ChallengeId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChallengeId",
                table: "ChallengeDay",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChallengeDay",
                table: "ChallengeDay",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeDay_Challenges_ChallengeId",
                table: "ChallengeDay",
                column: "ChallengeId",
                principalTable: "Challenges",
                principalColumn: "Id");
        }
    }
}
