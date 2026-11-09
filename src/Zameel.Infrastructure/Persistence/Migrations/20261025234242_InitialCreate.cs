using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zameel.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "universities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    city = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    logoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_universities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "faculties",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    universityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    slug = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_faculties", x => x.id);
                    table.ForeignKey(
                        name: "FK_faculties_universities_universityId",
                        column: x => x.universityId,
                        principalTable: "universities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    fullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    avatarUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    role = table.Column<int>(type: "int", nullable: false),
                    universityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    facultyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    studentIdCardUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    isVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    verifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    activeListingsCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_faculties_facultyId",
                        column: x => x.facultyId,
                        principalTable: "faculties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_universities_universityId",
                        column: x => x.universityId,
                        principalTable: "universities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "listings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    universityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    primaryFacultyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    type = table.Column<int>(type: "int", nullable: false),
                    condition = table.Column<int>(type: "int", nullable: false),
                    scope = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    reportedSoldCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    viewsCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    expiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    soldAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listings", x => x.id);
                    table.ForeignKey(
                        name: "FK_listings_faculties_primaryFacultyId",
                        column: x => x.primaryFacultyId,
                        principalTable: "faculties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_listings_universities_universityId",
                        column: x => x.universityId,
                        principalTable: "universities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_listings_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "storeProfiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    storeName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    commercialRegister = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    locationDescription = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    logoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    subscriptionExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_storeProfiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_storeProfiles_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    listingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    parentCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_comments_comments_parentCommentId",
                        column: x => x.parentCommentId,
                        principalTable: "comments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comments_listings_listingId",
                        column: x => x.listingId,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comments_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "conversations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    listingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    buyerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    isDealConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    dealConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    lastMessageAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversations", x => x.id);
                    table.ForeignKey(
                        name: "FK_conversations_listings_listingId",
                        column: x => x.listingId,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_conversations_users_buyerId",
                        column: x => x.buyerId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_conversations_users_sellerId",
                        column: x => x.sellerId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "listingFaculties",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    listingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    facultyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listingFaculties", x => x.id);
                    table.ForeignKey(
                        name: "FK_listingFaculties_faculties_facultyId",
                        column: x => x.facultyId,
                        principalTable: "faculties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_listingFaculties_listings_listingId",
                        column: x => x.listingId,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "listingImages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    listingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    imageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    displayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    isPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listingImages", x => x.id);
                    table.ForeignKey(
                        name: "FK_listingImages_listings_listingId",
                        column: x => x.listingId,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "listingReports",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    listingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    reporterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    reason = table.Column<int>(type: "int", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    isProcessed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listingReports", x => x.id);
                    table.ForeignKey(
                        name: "FK_listingReports_listings_listingId",
                        column: x => x.listingId,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_listingReports_users_reporterId",
                        column: x => x.reporterId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    listingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    reviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    revieweeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.id);
                    table.ForeignKey(
                        name: "FK_reviews_listings_listingId",
                        column: x => x.listingId,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reviews_users_revieweeId",
                        column: x => x.revieweeId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reviews_users_reviewerId",
                        column: x => x.reviewerId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    conversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    senderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    isRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    readAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_messages_conversations_conversationId",
                        column: x => x.conversationId,
                        principalTable: "conversations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_messages_users_senderId",
                        column: x => x.senderId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_comments_listingId_createdAt",
                table: "comments",
                columns: new[] { "listingId", "createdAt" });

            migrationBuilder.CreateIndex(
                name: "IX_comments_parentCommentId",
                table: "comments",
                column: "parentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_comments_userId",
                table: "comments",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_conversations_buyerId_lastMessageAt",
                table: "conversations",
                columns: new[] { "buyerId", "lastMessageAt" });

            migrationBuilder.CreateIndex(
                name: "IX_conversations_listingId_buyerId",
                table: "conversations",
                columns: new[] { "listingId", "buyerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_conversations_sellerId_lastMessageAt",
                table: "conversations",
                columns: new[] { "sellerId", "lastMessageAt" });

            migrationBuilder.CreateIndex(
                name: "IX_faculties_universityId_slug",
                table: "faculties",
                columns: new[] { "universityId", "slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_listingFaculties_facultyId",
                table: "listingFaculties",
                column: "facultyId");

            migrationBuilder.CreateIndex(
                name: "IX_listingFaculties_listingId_facultyId",
                table: "listingFaculties",
                columns: new[] { "listingId", "facultyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_listingImages_listingId_isPrimary",
                table: "listingImages",
                columns: new[] { "listingId", "isPrimary" });

            migrationBuilder.CreateIndex(
                name: "IX_listingReports_listingId_isProcessed",
                table: "listingReports",
                columns: new[] { "listingId", "isProcessed" });

            migrationBuilder.CreateIndex(
                name: "IX_listingReports_listingId_reporterId_reason",
                table: "listingReports",
                columns: new[] { "listingId", "reporterId", "reason" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_listingReports_reporterId",
                table: "listingReports",
                column: "reporterId");

            migrationBuilder.CreateIndex(
                name: "IX_listings_primaryFacultyId",
                table: "listings",
                column: "primaryFacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_listings_status_expiresAt",
                table: "listings",
                columns: new[] { "status", "expiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_listings_universityId_primaryFacultyId_status_createdAt",
                table: "listings",
                columns: new[] { "universityId", "primaryFacultyId", "status", "createdAt" });

            migrationBuilder.CreateIndex(
                name: "IX_listings_universityId_status_createdAt",
                table: "listings",
                columns: new[] { "universityId", "status", "createdAt" });

            migrationBuilder.CreateIndex(
                name: "IX_listings_userId_status",
                table: "listings",
                columns: new[] { "userId", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_messages_conversationId_createdAt",
                table: "messages",
                columns: new[] { "conversationId", "createdAt" });

            migrationBuilder.CreateIndex(
                name: "IX_messages_conversationId_isRead",
                table: "messages",
                columns: new[] { "conversationId", "isRead" });

            migrationBuilder.CreateIndex(
                name: "IX_messages_senderId",
                table: "messages",
                column: "senderId");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_listingId_reviewerId",
                table: "reviews",
                columns: new[] { "listingId", "reviewerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reviews_revieweeId_rating",
                table: "reviews",
                columns: new[] { "revieweeId", "rating" });

            migrationBuilder.CreateIndex(
                name: "IX_reviews_reviewerId",
                table: "reviews",
                column: "reviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_storeProfiles_isApproved",
                table: "storeProfiles",
                column: "isApproved");

            migrationBuilder.CreateIndex(
                name: "IX_storeProfiles_userId",
                table: "storeProfiles",
                column: "userId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_universities_name",
                table: "universities",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_facultyId",
                table: "users",
                column: "facultyId");

            migrationBuilder.CreateIndex(
                name: "IX_users_phone",
                table: "users",
                column: "phone");

            migrationBuilder.CreateIndex(
                name: "IX_users_universityId_role",
                table: "users",
                columns: new[] { "universityId", "role" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "listingFaculties");

            migrationBuilder.DropTable(
                name: "listingImages");

            migrationBuilder.DropTable(
                name: "listingReports");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "reviews");

            migrationBuilder.DropTable(
                name: "storeProfiles");

            migrationBuilder.DropTable(
                name: "conversations");

            migrationBuilder.DropTable(
                name: "listings");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "faculties");

            migrationBuilder.DropTable(
                name: "universities");
        }
    }
}
