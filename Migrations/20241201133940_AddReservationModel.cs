using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomNam.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reservation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    KarenderyaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReserveDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Total = table.Column<double>(type: "double precision", nullable: false),
                    ModeOfPayment = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservation_CustomerProfile_CustomerProfileId",
                        column: x => x.CustomerProfileId,
                        principalTable: "CustomerProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservation_Karenderya_KarenderyaId",
                        column: x => x.KarenderyaId,
                        principalTable: "Karenderya",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_CustomerProfileId",
                table: "Reservation",
                column: "CustomerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_KarenderyaId",
                table: "Reservation",
                column: "KarenderyaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservation");
        }
    }
}
