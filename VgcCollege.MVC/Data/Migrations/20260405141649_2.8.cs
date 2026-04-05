using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VgcCollege.MVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class _28 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentProfileId",
                table: "Exams",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exams_StudentProfileId",
                table: "Exams",
                column: "StudentProfileId");

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

            migrationBuilder.DropIndex(
                name: "IX_Exams_StudentProfileId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "StudentProfileId",
                table: "Exams");
        }
    }
}
