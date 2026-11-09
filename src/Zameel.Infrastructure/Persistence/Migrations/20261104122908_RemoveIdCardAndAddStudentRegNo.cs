using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zameel.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIdCardAndAddStudentRegNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "studentIdCardUrl",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "studentRegNo",
                table: "users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_universityId_studentRegNo",
                table: "users",
                columns: new[] { "universityId", "studentRegNo" },
                unique: true,
                filter: "[studentRegNo] IS NOT NULL AND [universityId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_universityId_studentRegNo",
                table: "users");

            migrationBuilder.DropColumn(
                name: "studentRegNo",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "studentIdCardUrl",
                table: "users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
