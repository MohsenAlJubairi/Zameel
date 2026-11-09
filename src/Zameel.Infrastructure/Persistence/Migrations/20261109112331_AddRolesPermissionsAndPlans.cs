using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zameel.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesPermissionsAndPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_universityId_role",
                table: "users");

            migrationBuilder.DropColumn(
                name: "role",
                table: "users");

            migrationBuilder.AddColumn<DateTime>(
                name: "planExpiresAt",
                table: "users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "planId",
                table: "users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    systemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    displayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    module = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "plans",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    price = table.Column<decimal>(type: "money", nullable: false, defaultValue: 0m),
                    maxActiveListings = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    durationDays = table.Column<int>(type: "int", nullable: false, defaultValue: 30),
                    hasVerifiedBadge = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    hasStoreProfile = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    isDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plans", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    normalizedName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    isSystemRole = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rolePermissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    roleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    permissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rolePermissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_rolePermissions_permissions_permissionId",
                        column: x => x.permissionId,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rolePermissions_roles_roleId",
                        column: x => x.roleId,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Insert system roles before creating roleId column on users
            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "name", "normalizedName", "description", "isSystemRole", "createdAt", "isDeleted" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "مدير النظام", "ADMIN", "كامل صلاحيات الإدارة والتحكم في منصة زميل", true, DateTime.UtcNow, false },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "مشرف المنصة", "MODERATOR", "مراجعة واعتماد الإعلانات وتوثيق الطلاب", true, DateTime.UtcNow, false },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "مكتبة / شريك معتمد", "STORE", "حساب تجاري للمكتبات الجامعية ونقاط استلام المستلزمات", true, DateTime.UtcNow, false },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "طالب موثق", "STUDENT_VERIFIED", "طالب جامعي موثق برقم القيد الجامعي (سقف 5 إعلانات)", true, DateTime.UtcNow, false },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "طالب عادي", "STUDENT_UNVERIFIED", "طالب جديد غير موثق (سقف 3 إعلانات)", true, DateTime.UtcNow, false }
                });

            // Add roleId with default to StudentUnverified
            migrationBuilder.AddColumn<Guid>(
                name: "roleId",
                table: "users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("55555555-5555-5555-5555-555555555555"));

            // Map existing demo users to their correct roles
            migrationBuilder.Sql("UPDATE [users] SET [roleId] = '11111111-1111-1111-1111-111111111111' WHERE [email] = 'admin@zameel.app';");
            migrationBuilder.Sql("UPDATE [users] SET [roleId] = '33333333-3333-3333-3333-333333333333' WHERE [email] = 'aden.bookstore@zameel.app';");
            migrationBuilder.Sql("UPDATE [users] SET [roleId] = '44444444-4444-4444-4444-444444444444' WHERE [email] = 'ahmed.student@zameel.app';");
            migrationBuilder.Sql("UPDATE [users] SET [roleId] = '55555555-5555-5555-5555-555555555555' WHERE [email] = 'sara.student@zameel.app';");

            migrationBuilder.CreateIndex(
                name: "IX_users_planId",
                table: "users",
                column: "planId");

            migrationBuilder.CreateIndex(
                name: "IX_users_roleId",
                table: "users",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_users_universityId_roleId",
                table: "users",
                columns: new[] { "universityId", "roleId" });

            migrationBuilder.CreateIndex(
                name: "IX_permissions_systemName",
                table: "permissions",
                column: "systemName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rolePermissions_permissionId",
                table: "rolePermissions",
                column: "permissionId");

            migrationBuilder.CreateIndex(
                name: "IX_rolePermissions_roleId_permissionId",
                table: "rolePermissions",
                columns: new[] { "roleId", "permissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_normalizedName",
                table: "roles",
                column: "normalizedName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_users_plans_planId",
                table: "users",
                column: "planId",
                principalTable: "plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_roleId",
                table: "users",
                column: "roleId",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_plans_planId",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_roleId",
                table: "users");

            migrationBuilder.DropTable(
                name: "plans");

            migrationBuilder.DropTable(
                name: "rolePermissions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropIndex(
                name: "IX_users_planId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_roleId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_universityId_roleId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "planExpiresAt",
                table: "users");

            migrationBuilder.DropColumn(
                name: "planId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "roleId",
                table: "users");

            migrationBuilder.AddColumn<int>(
                name: "role",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_users_universityId_role",
                table: "users",
                columns: new[] { "universityId", "role" });
        }
    }
}
