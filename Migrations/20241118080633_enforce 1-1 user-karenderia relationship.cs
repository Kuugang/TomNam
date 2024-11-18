using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomNam.Migrations
{
    /// <inheritdoc />
    public partial class enforce11userkarenderiarelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Karenderya_AspNetUsers_UserId",
                table: "Karenderya");

            migrationBuilder.AddForeignKey(
                name: "FK_Karenderya_AspNetUsers_UserId",
                table: "Karenderya",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Karenderya_AspNetUsers_UserId",
                table: "Karenderya");

            migrationBuilder.AddForeignKey(
                name: "FK_Karenderya_AspNetUsers_UserId",
                table: "Karenderya",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
