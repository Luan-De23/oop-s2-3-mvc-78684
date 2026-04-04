using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VgcCollege.MVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeResultIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_AssignmentResults_AssigmentResultId",
                table: "Assignments");

            migrationBuilder.AlterColumn<int>(
                name: "AssigmentResultId",
                table: "Assignments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_AssignmentResults_AssigmentResultId",
                table: "Assignments",
                column: "AssigmentResultId",
                principalTable: "AssignmentResults",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_AssignmentResults_AssigmentResultId",
                table: "Assignments");

            migrationBuilder.AlterColumn<int>(
                name: "AssigmentResultId",
                table: "Assignments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_AssignmentResults_AssigmentResultId",
                table: "Assignments",
                column: "AssigmentResultId",
                principalTable: "AssignmentResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
