using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zameel.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChatMessagesEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chatMessages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    senderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    receiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    listingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    content = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
                    isRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    readAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chatMessages", x => x.id);
                    table.ForeignKey(
                        name: "FK_chatMessages_listings_listingId",
                        column: x => x.listingId,
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chatMessages_users_receiverId",
                        column: x => x.receiverId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chatMessages_users_senderId",
                        column: x => x.senderId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chatMessages_listingId",
                table: "chatMessages",
                column: "listingId");

            migrationBuilder.CreateIndex(
                name: "IX_chatMessages_receiverId_isRead",
                table: "chatMessages",
                columns: new[] { "receiverId", "isRead" });

            migrationBuilder.CreateIndex(
                name: "IX_chatMessages_senderId_receiverId_createdAt",
                table: "chatMessages",
                columns: new[] { "senderId", "receiverId", "createdAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chatMessages");
        }
    }
}
