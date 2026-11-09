using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zameel.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMessagingAndSeedComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "conversations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "conversations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    buyerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    listingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dealConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isDealConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    lastMessageAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    conversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    senderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    isRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    readAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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
        }
    }
}
