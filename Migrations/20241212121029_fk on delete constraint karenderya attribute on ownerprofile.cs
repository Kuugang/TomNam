using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomNam.Migrations
{
    /// <inheritdoc />
    public partial class fkondeleteconstraintkarenderyaattributeonownerprofile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnerProfile_Karenderya_KarenderyaId",
                table: "OwnerProfile");

            migrationBuilder.DropIndex(
                name: "IX_OwnerProfile_KarenderyaId",
                table: "OwnerProfile");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerProfile_KarenderyaId",
                table: "OwnerProfile",
                column: "KarenderyaId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OwnerProfile_Karenderya_KarenderyaId",
                table: "OwnerProfile",
                column: "KarenderyaId",
                principalTable: "Karenderya",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnerProfile_Karenderya_KarenderyaId",
                table: "OwnerProfile");

            migrationBuilder.DropIndex(
                name: "IX_OwnerProfile_KarenderyaId",
                table: "OwnerProfile");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerProfile_KarenderyaId",
                table: "OwnerProfile",
                column: "KarenderyaId");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnerProfile_Karenderya_KarenderyaId",
                table: "OwnerProfile",
                column: "KarenderyaId",
                principalTable: "Karenderya",
                principalColumn: "Id");
        }
    }
}
