using Microsoft.EntityFrameworkCore.Migrations;

namespace Entities.Migrations.Cafe
{
    public partial class ForeignKeyForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey("FK_UserHasFacility_User_UserId",
                "UserHasFacility",
                "UserId", 
                "AspNetUsers", 
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_UserHasFacility_User_UserId", "UserHasFacility");
        }
    }
}
