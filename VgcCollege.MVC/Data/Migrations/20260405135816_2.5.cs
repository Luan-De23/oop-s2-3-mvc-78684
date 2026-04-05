using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VgcCollege.MVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class _25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_StudentProfiles_StudentProfileId",
                table: "Exams");

            migrationBuilder.AlterColumn<int>(
                name: "StudentProfileId",
                table: "Exams",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_StudentProfiles_StudentProfileId",
                table: "Exams",
                column: "StudentProfileId",
                principalTable: "StudentProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_StudentProfiles_StudentProfileId",
                table: "Exams");

            migrationBuilder.AlterColumn<int>(
                name: "StudentProfileId",
                table: "Exams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_StudentProfiles_StudentProfileId",
                table: "Exams",
                column: "StudentProfileId",
                principalTable: "StudentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
