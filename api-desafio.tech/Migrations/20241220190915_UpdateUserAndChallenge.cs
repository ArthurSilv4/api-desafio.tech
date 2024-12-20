using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_desafio.tech.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserAndChallenge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SocialMedia",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChallengesCompleted",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MissionsCompleted",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ReceiveEmail",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChallengesCompleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MissionsCompleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReceiveEmail",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "SocialMedia",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
