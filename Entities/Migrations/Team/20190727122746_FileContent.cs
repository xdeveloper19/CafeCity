using Microsoft.EntityFrameworkCore.Migrations;

namespace Entities.Migrations.Team
{
    public partial class FileContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Imagine",
                table: "Slivers",
                newName: "FileContent");

            migrationBuilder.RenameColumn(
                name: "Imagine",
                table: "Badges",
                newName: "FileContent");

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "Slivers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "Badges",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileType",
                table: "Slivers");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "Badges");

            migrationBuilder.RenameColumn(
                name: "FileContent",
                table: "Slivers",
                newName: "Imagine");

            migrationBuilder.RenameColumn(
                name: "FileContent",
                table: "Badges",
                newName: "Imagine");
        }
    }
}
