using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkingHUB.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableSlot",
                table: "Parkings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalSlot",
                table: "Parkings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableSlot",
                table: "Parkings");

            migrationBuilder.DropColumn(
                name: "TotalSlot",
                table: "Parkings");
        }
    }
}
