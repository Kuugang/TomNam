using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomNam.Migrations
{
    /// <inheritdoc />
    public partial class addproofofbusinessmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProofOfBusiness",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KarenderyaId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerValidID1 = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    OwnerValidID2 = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    BusinessPermit = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    BIRPermit = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProofOfBusiness", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProofOfBusiness_Karenderya_KarenderyaId",
                        column: x => x.KarenderyaId,
                        principalTable: "Karenderya",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProofOfBusiness_KarenderyaId",
                table: "ProofOfBusiness",
                column: "KarenderyaId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProofOfBusiness");
        }
    }
}
