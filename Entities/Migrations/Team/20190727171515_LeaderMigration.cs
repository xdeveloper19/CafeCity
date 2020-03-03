using Microsoft.EntityFrameworkCore.Migrations;

namespace Entities.Migrations.Team
{
    public partial class LeaderMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Leader",
                table: "Teams",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Leader",
                table: "Teams");
        }
    }
}
