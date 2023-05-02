using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Centers.API.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AlteredTableShiftsAndAddedCapacityColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Shifts",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Shifts");
        }
    }
}
