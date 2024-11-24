using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomNam.Migrations
{
    /// <inheritdoc />
    public partial class addedisverifiedfieldforkarenderyarequestdtoupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsVerified",
                table: "Karenderya",
                type: "boolean",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsVerified",
                table: "Karenderya",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true,
                oldDefaultValue: false);
        }
    }
}
