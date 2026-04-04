using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VgcCollege.MVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class Version20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrolments_AttendaceRecords_AttendaceRecordId",
                table: "CourseEnrolments");

            migrationBuilder.DropIndex(
                name: "IX_CourseEnrolments_AttendaceRecordId",
                table: "CourseEnrolments");

            migrationBuilder.DropColumn(
                name: "AttendaceRecordId",
                table: "CourseEnrolments");

            migrationBuilder.DropColumn(
                name: "WeekDate",
                table: "AttendaceRecords");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "AttendaceRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AttendaceRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WeekNumber",
                table: "AttendaceRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "AttendaceRecords");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AttendaceRecords");

            migrationBuilder.DropColumn(
                name: "WeekNumber",
                table: "AttendaceRecords");

            migrationBuilder.AddColumn<int>(
                name: "AttendaceRecordId",
                table: "CourseEnrolments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "WeekDate",
                table: "AttendaceRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrolments_AttendaceRecordId",
                table: "CourseEnrolments",
                column: "AttendaceRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrolments_AttendaceRecords_AttendaceRecordId",
                table: "CourseEnrolments",
                column: "AttendaceRecordId",
                principalTable: "AttendaceRecords",
                principalColumn: "Id");
        }
    }
}
