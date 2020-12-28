using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Entities.Migrations.Cafe
{
    public partial class NewMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GeoData_FacilityId",
                table: "GeoData");

            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "Media",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Rate",
                table: "Facilities",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Facilities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaceId",
                table: "Facilities",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GeoData_FacilityId",
                table: "GeoData",
                column: "FacilityId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GeoData_FacilityId",
                table: "GeoData");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "Facilities");

            migrationBuilder.AlterColumn<string>(
                name: "Rate",
                table: "Facilities",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.CreateIndex(
                name: "IX_GeoData_FacilityId",
                table: "GeoData",
                column: "FacilityId");
        }
    }
}
