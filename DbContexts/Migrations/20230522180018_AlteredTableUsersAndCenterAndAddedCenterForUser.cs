using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Centers.API.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AlteredTableUsersAndCenterAndAddedCenterForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Centers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CenterId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Centers_OwnerId",
                table: "Centers",
                column: "OwnerId",
                unique: true,
                filter: "[OwnerId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Centers_AspNetUsers_OwnerId",
                table: "Centers",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Centers_AspNetUsers_OwnerId",
                table: "Centers");

            migrationBuilder.DropIndex(
                name: "IX_Centers_OwnerId",
                table: "Centers");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Centers");

            migrationBuilder.DropColumn(
                name: "CenterId",
                table: "AspNetUsers");
        }
    }
}
