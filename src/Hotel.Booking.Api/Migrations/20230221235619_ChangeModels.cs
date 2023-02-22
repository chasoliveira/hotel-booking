using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Booking.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Confirmed",
                table: "Bookings",
                newName: "IsConfirmed");

            migrationBuilder.RenameColumn(
                name: "Canceled",
                table: "Bookings",
                newName: "IsCanceled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsConfirmed",
                table: "Bookings",
                newName: "Confirmed");

            migrationBuilder.RenameColumn(
                name: "IsCanceled",
                table: "Bookings",
                newName: "Canceled");
        }
    }
}
