using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Entities.Migrations.Team
{
    public partial class FixKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBadges_Badges_BadgeId",
                table: "UserBadges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSlivers_Slivers_SliverId",
                table: "UserSlivers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSlivers",
                table: "UserSlivers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBadges",
                table: "UserBadges");

            migrationBuilder.AlterColumn<Guid>(
                name: "SliverId",
                table: "UserSlivers",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "BadgeId",
                table: "UserBadges",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSlivers",
                table: "UserSlivers",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBadges",
                table: "UserBadges",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBadges_BadgeId",
                table: "UserBadges",
                column: "BadgeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBadges_Badges_BadgeId",
                table: "UserBadges",
                column: "BadgeId",
                principalTable: "Badges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSlivers_Slivers_SliverId",
                table: "UserSlivers",
                column: "SliverId",
                principalTable: "Slivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBadges_Badges_BadgeId",
                table: "UserBadges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSlivers_Slivers_SliverId",
                table: "UserSlivers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSlivers",
                table: "UserSlivers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBadges",
                table: "UserBadges");

            migrationBuilder.DropIndex(
                name: "IX_UserBadges_BadgeId",
                table: "UserBadges");

            migrationBuilder.AlterColumn<Guid>(
                name: "SliverId",
                table: "UserSlivers",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BadgeId",
                table: "UserBadges",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSlivers",
                table: "UserSlivers",
                columns: new[] { "UserId", "SliverId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBadges",
                table: "UserBadges",
                columns: new[] { "BadgeId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserBadges_Badges_BadgeId",
                table: "UserBadges",
                column: "BadgeId",
                principalTable: "Badges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSlivers_Slivers_SliverId",
                table: "UserSlivers",
                column: "SliverId",
                principalTable: "Slivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
