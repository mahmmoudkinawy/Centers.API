using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Centers.API.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddedExamDatesTablesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExamDates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpeningDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosingDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamDates", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamDates");
        }
    }
}
