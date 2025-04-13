using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVenuePhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VenuePhoto_Venues_VenueId",
                table: "VenuePhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VenuePhoto",
                table: "VenuePhoto");

            migrationBuilder.RenameTable(
                name: "VenuePhoto",
                newName: "VenuePhotos");

            migrationBuilder.RenameIndex(
                name: "IX_VenuePhoto_VenueId",
                table: "VenuePhotos",
                newName: "IX_VenuePhotos_VenueId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VenuePhotos",
                table: "VenuePhotos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VenuePhotos_Venues_VenueId",
                table: "VenuePhotos",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VenuePhotos_Venues_VenueId",
                table: "VenuePhotos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VenuePhotos",
                table: "VenuePhotos");

            migrationBuilder.RenameTable(
                name: "VenuePhotos",
                newName: "VenuePhoto");

            migrationBuilder.RenameIndex(
                name: "IX_VenuePhotos_VenueId",
                table: "VenuePhoto",
                newName: "IX_VenuePhoto_VenueId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VenuePhoto",
                table: "VenuePhoto",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VenuePhoto_Venues_VenueId",
                table: "VenuePhoto",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
