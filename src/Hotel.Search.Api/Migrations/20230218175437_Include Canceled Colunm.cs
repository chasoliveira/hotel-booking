using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Search.Api.Migrations
{
    /// <inheritdoc />
    public partial class IncludeCanceledColunm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Canceled",
                table: "Reservations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Canceled",
                table: "Reservations");
        }
    }
}
