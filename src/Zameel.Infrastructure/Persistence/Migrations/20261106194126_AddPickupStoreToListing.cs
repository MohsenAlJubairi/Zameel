using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zameel.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPickupStoreToListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "pickupStoreId",
                table: "listings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_listings_pickupStoreId_status",
                table: "listings",
                columns: new[] { "pickupStoreId", "status" });

            migrationBuilder.AddForeignKey(
                name: "FK_listings_storeProfiles_pickupStoreId",
                table: "listings",
                column: "pickupStoreId",
                principalTable: "storeProfiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_listings_storeProfiles_pickupStoreId",
                table: "listings");

            migrationBuilder.DropIndex(
                name: "IX_listings_pickupStoreId_status",
                table: "listings");

            migrationBuilder.DropColumn(
                name: "pickupStoreId",
                table: "listings");
        }
    }
}
