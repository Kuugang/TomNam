using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomNam.Migrations
{
    /// <inheritdoc />
    public partial class addedkarenderya : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Karenderya",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LocationStreet = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LocationBarangay = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LocationCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LocationProvince = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateFounded = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    LogoPhoto = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    CoverPhoto = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Rating = table.Column<double>(type: "double precision", precision: 1, scale: 2, nullable: true, defaultValue: 0.0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Karenderya", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Karenderya_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Karenderya_UserId",
                table: "Karenderya",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Karenderya");
        }
    }
}
