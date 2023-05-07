using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Centers.API.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCentersTableAndRemovedAndAddedNewColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Centers_CenterId",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_CenterId",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "CenterId",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "ClosingDate",
                table: "Centers");

            migrationBuilder.DropColumn(
                name: "OpeningDate",
                table: "Centers");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Centers",
                newName: "Zone");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Centers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Centers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationUrl",
                table: "Centers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Centers");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Centers");

            migrationBuilder.DropColumn(
                name: "LocationUrl",
                table: "Centers");

            migrationBuilder.RenameColumn(
                name: "Zone",
                table: "Centers",
                newName: "Description");

            migrationBuilder.AddColumn<Guid>(
                name: "CenterId",
                table: "Shifts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosingDate",
                table: "Centers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OpeningDate",
                table: "Centers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_CenterId",
                table: "Shifts",
                column: "CenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Centers_CenterId",
                table: "Shifts",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
