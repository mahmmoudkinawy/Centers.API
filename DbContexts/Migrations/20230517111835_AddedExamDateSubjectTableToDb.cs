using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Centers.API.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class AddedExamDateSubjectTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamDates_Centers_CenterId",
                table: "ExamDates");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamDates_Subjects_SubjectId",
                table: "ExamDates");

            migrationBuilder.DropIndex(
                name: "IX_ExamDates_CenterId",
                table: "ExamDates");

            migrationBuilder.DropIndex(
                name: "IX_ExamDates_SubjectId",
                table: "ExamDates");

            migrationBuilder.DropColumn(
                name: "CenterId",
                table: "ExamDates");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "ExamDates");

            migrationBuilder.CreateTable(
                name: "ExamDateSubjects",
                columns: table => new
                {
                    ExamDateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamDateSubjects", x => new { x.ExamDateId, x.SubjectId, x.CenterId });
                    table.ForeignKey(
                        name: "FK_ExamDateSubjects_Centers_CenterId",
                        column: x => x.CenterId,
                        principalTable: "Centers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamDateSubjects_ExamDates_ExamDateId",
                        column: x => x.ExamDateId,
                        principalTable: "ExamDates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamDateSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamDateSubjects_CenterId",
                table: "ExamDateSubjects",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamDateSubjects_SubjectId",
                table: "ExamDateSubjects",
                column: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamDateSubjects");

            migrationBuilder.AddColumn<Guid>(
                name: "CenterId",
                table: "ExamDates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "ExamDates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ExamDates_CenterId",
                table: "ExamDates",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamDates_SubjectId",
                table: "ExamDates",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamDates_Centers_CenterId",
                table: "ExamDates",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamDates_Subjects_SubjectId",
                table: "ExamDates",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
