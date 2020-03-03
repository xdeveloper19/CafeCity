using Microsoft.EntityFrameworkCore.Migrations;

namespace Entities.Migrations.Team
{
    public partial class AddBadgesForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey("FK_UserBadges_ApplicationUser_UserId",
                "UserBadges", "UserId",
                "AspNetUsers", principalColumn: "Id",
                 onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey("FK_UserSlivers_ApplicationUser_UserId",
                "UserSlivers", "UserId",
                "AspNetUsers", principalColumn: "Id",
                 onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_UserBadges_ApplicationUser_UserId",
                "UserID", "UserBadges");

            migrationBuilder.DropForeignKey("FK_UserSlivers_ApplicationUser_UserId",
                "UserID", "UserSlivers");

        }
    }
}
