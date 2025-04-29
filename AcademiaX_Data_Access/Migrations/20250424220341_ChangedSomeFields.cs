using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademiaX_Data_Access.Migrations
{
    /// <inheritdoc />
    public partial class ChangedSomeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_TeacherId1",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_TeacherId1",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "FinalGrade",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "MakeupGrade",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "TeacherId1",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "MidtermGrade",
                table: "Grades",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "AttendanceDate",
                table: "Attendances",
                newName: "Date");

            migrationBuilder.AddColumn<int>(
                name: "ExamType",
                table: "Grades",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "TeacherId",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StudentCourses",
                columns: table => new
                {
                    CoursesId = table.Column<int>(type: "int", nullable: false),
                    StudentsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourses", x => new { x.CoursesId, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_StudentCourses_AspNetUsers_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCourses_Courses_CoursesId",
                        column: x => x.CoursesId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TeacherId",
                table: "Courses",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourses_StudentsId",
                table: "StudentCourses",
                column: "StudentsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_TeacherId",
                table: "Courses",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_TeacherId",
                table: "Courses");

            migrationBuilder.DropTable(
                name: "StudentCourses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_TeacherId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ExamType",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Grades",
                newName: "MidtermGrade");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Attendances",
                newName: "AttendanceDate");

            migrationBuilder.AddColumn<double>(
                name: "FinalGrade",
                table: "Grades",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MakeupGrade",
                table: "Grades",
                type: "float",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TeacherId",
                table: "Courses",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "TeacherId1",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TeacherId1",
                table: "Courses",
                column: "TeacherId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_TeacherId1",
                table: "Courses",
                column: "TeacherId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
