using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkingHUB.Migrations
{
    /// <inheritdoc />
    public partial class Update6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOcuppied",
                table: "ParkingVehicles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberParking",
                table: "ParkingVehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOcuppied",
                table: "ParkingVehicles");

            migrationBuilder.DropColumn(
                name: "NumberParking",
                table: "ParkingVehicles");
        }
    }
}
