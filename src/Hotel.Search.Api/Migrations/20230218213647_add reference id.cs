using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Search.Api.Migrations
{
    /// <inheritdoc />
    public partial class addreferenceid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReferenceId",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReferenceId",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReferenceRoomId",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReferenceRoomId",
                table: "Reservations");
        }
    }
}
