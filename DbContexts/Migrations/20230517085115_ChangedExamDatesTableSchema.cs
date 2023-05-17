using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Centers.API.DbContexts.Migrations
{
    /// <inheritdoc />
    public partial class ChangedExamDatesTableSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
