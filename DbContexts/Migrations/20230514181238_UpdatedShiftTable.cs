using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Centers.API.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedShiftTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_AspNetUsers_AdminId",
                table: "Shifts");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "Shifts",
                newName: "CenterId");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_AdminId",
                table: "Shifts",
                newName: "IX_Shifts_CenterId");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Shifts",
                type: "bit",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Centers_CenterId",
                table: "Shifts",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Centers_CenterId",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Shifts");

            migrationBuilder.RenameColumn(
                name: "CenterId",
                table: "Shifts",
                newName: "AdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_CenterId",
                table: "Shifts",
                newName: "IX_Shifts_AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_AspNetUsers_AdminId",
                table: "Shifts",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
