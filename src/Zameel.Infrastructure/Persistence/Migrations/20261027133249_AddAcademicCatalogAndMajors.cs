using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zameel.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicCatalogAndMajors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_faculties_universities_universityId",
                table: "faculties");

            migrationBuilder.DropIndex(
                name: "IX_faculties_universityId_slug",
                table: "faculties");

            migrationBuilder.DropColumn(
                name: "universityId",
                table: "faculties");

            migrationBuilder.AddColumn<Guid>(
                name: "majorId",
                table: "users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "primaryMajorId",
                table: "listings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "majors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    slug = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_majors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "universityFaculties",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    universityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    facultyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_universityFaculties", x => x.id);
                    table.ForeignKey(
                        name: "FK_universityFaculties_faculties_facultyId",
                        column: x => x.facultyId,
                        principalTable: "faculties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_universityFaculties_universities_universityId",
                        column: x => x.universityId,
                        principalTable: "universities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "facultyMajors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    facultyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    majorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facultyMajors", x => x.id);
                    table.ForeignKey(
                        name: "FK_facultyMajors_faculties_facultyId",
                        column: x => x.facultyId,
                        principalTable: "faculties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_facultyMajors_majors_majorId",
                        column: x => x.majorId,
                        principalTable: "majors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "listingMajors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    listingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    majorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listingMajors", x => x.id);
                    table.ForeignKey(
                        name: "FK_listingMajors_listings_listingId",
                        column: x => x.listingId,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_listingMajors_majors_majorId",
                        column: x => x.majorId,
                        principalTable: "majors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_majorId",
                table: "users",
                column: "majorId");

            migrationBuilder.CreateIndex(
                name: "IX_users_universityId_facultyId_majorId",
                table: "users",
                columns: new[] { "universityId", "facultyId", "majorId" });

            migrationBuilder.CreateIndex(
                name: "IX_listings_primaryMajorId_status",
                table: "listings",
                columns: new[] { "primaryMajorId", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_listings_universityId_primaryFacultyId_primaryMajorId_status_createdAt",
                table: "listings",
                columns: new[] { "universityId", "primaryFacultyId", "primaryMajorId", "status", "createdAt" });

            migrationBuilder.CreateIndex(
                name: "IX_faculties_name",
                table: "faculties",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_faculties_slug",
                table: "faculties",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_facultyMajors_facultyId_majorId",
                table: "facultyMajors",
                columns: new[] { "facultyId", "majorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_facultyMajors_majorId",
                table: "facultyMajors",
                column: "majorId");

            migrationBuilder.CreateIndex(
                name: "IX_listingMajors_listingId_majorId",
                table: "listingMajors",
                columns: new[] { "listingId", "majorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_listingMajors_majorId",
                table: "listingMajors",
                column: "majorId");

            migrationBuilder.CreateIndex(
                name: "IX_majors_name",
                table: "majors",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_majors_slug",
                table: "majors",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_universityFaculties_facultyId",
                table: "universityFaculties",
                column: "facultyId");

            migrationBuilder.CreateIndex(
                name: "IX_universityFaculties_universityId_facultyId",
                table: "universityFaculties",
                columns: new[] { "universityId", "facultyId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_listings_majors_primaryMajorId",
                table: "listings",
                column: "primaryMajorId",
                principalTable: "majors",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_users_majors_majorId",
                table: "users",
                column: "majorId",
                principalTable: "majors",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_listings_majors_primaryMajorId",
                table: "listings");

            migrationBuilder.DropForeignKey(
                name: "FK_users_majors_majorId",
                table: "users");

            migrationBuilder.DropTable(
                name: "facultyMajors");

            migrationBuilder.DropTable(
                name: "listingMajors");

            migrationBuilder.DropTable(
                name: "universityFaculties");

            migrationBuilder.DropTable(
                name: "majors");

            migrationBuilder.DropIndex(
                name: "IX_users_majorId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_universityId_facultyId_majorId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_listings_primaryMajorId_status",
                table: "listings");

            migrationBuilder.DropIndex(
                name: "IX_listings_universityId_primaryFacultyId_primaryMajorId_status_createdAt",
                table: "listings");

            migrationBuilder.DropIndex(
                name: "IX_faculties_name",
                table: "faculties");

            migrationBuilder.DropIndex(
                name: "IX_faculties_slug",
                table: "faculties");

            migrationBuilder.DropColumn(
                name: "majorId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "primaryMajorId",
                table: "listings");

            migrationBuilder.AddColumn<Guid>(
                name: "universityId",
                table: "faculties",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_faculties_universityId_slug",
                table: "faculties",
                columns: new[] { "universityId", "slug" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_faculties_universities_universityId",
                table: "faculties",
                column: "universityId",
                principalTable: "universities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
