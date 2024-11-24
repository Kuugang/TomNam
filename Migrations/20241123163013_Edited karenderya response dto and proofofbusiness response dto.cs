using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TomNam.Migrations
{
    /// <inheritdoc />
    public partial class Editedkarenderyaresponsedtoandproofofbusinessresponsedto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Karenderya",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Karenderya",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Karenderya");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Karenderya",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
