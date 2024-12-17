using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_desafio.tech.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameInChallenge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Challenges",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Challenges");
        }
    }
}
