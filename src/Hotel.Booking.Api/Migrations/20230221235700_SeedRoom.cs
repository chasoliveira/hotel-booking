using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Booking.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Description" },
                values: new object[] { -1, "An awesome room for your vacation" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: -1);
        }
    }
}
